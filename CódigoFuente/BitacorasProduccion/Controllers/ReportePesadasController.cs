using Clases.Util;
using DocumentFormat.OpenXml.Drawing.Charts;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class ReportePesadasController : BaseController
    {
        // GET: ReportePesadas/Silao
        private Portal_2_0Entities db = new Portal_2_0Entities();

        public ActionResult Index(string cliente, string fecha_inicial, string fecha_final, int? id_planta, string material, int muestra = 5)
        {
            if (!TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE))
            {
                return View("../Home/ErrorPermisos");
            }

            // Mensaje en caso de crear, editar, etc.
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            DateTime dateInicial, dateFinal;
            if (!TryParseDate(fecha_inicial, out dateInicial))
            {
                dateInicial = DateTime.Now;
            }
            if (!TryParseDate(fecha_final, out dateFinal))
            {
                dateFinal = DateTime.Now;
            }

            // Obtener el listado de reportes de pesadas
            var listado = Bitacoras.DBUtil.ReportesPesadasDBUtil.ObtieneReportePuebla(cliente, dateInicial, dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59), id_planta, material, muestra);

            // Rutas para los filtros
            var routeValues = new System.Web.Routing.RouteValueDictionary
            {
                ["cliente"] = cliente,
                ["fecha_inicial"] = fecha_inicial,
                ["fecha_final"] = fecha_final
            };

            ViewBag.Filtros = routeValues;
            ViewBag.cliente = AddFirstItem(ObtenerClientesSelectList(id_planta), textoPorDefecto: "-- Seleccione --");
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --");

            return View(listado);
        }

        [NonAction]
        private bool TryParseDate(string dateString, out DateTime date)
        {
            return DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        [NonAction]
        private SelectList ObtenerClientesSelectList(int? id_planta)
        {
            var clientes = db.view_datos_base_reporte_pesadas
                             .Where(x => x.clave_planta == id_planta)
                             .Select(x => x.invoiced_to)
                             .Distinct()
                             .OrderBy(x => x)
                             .ToList();

            var newList = clientes.Select(p => new SelectListItem { Text = p, Value = p }).ToList();
            return new SelectList(newList, "Value", "Text");
        }

        // GET: Analisis
        public ActionResult Analisis(string id_planta, string cliente, string fecha_inicial, string fecha_final, string numero_sap, string fecha_inicio_validez, string fecha_fin_validez)
        {
            ViewBag.Planta = id_planta;
            ViewBag.Cliente = cliente;
            ViewBag.FechaInicial = fecha_inicial;
            ViewBag.FechaFinal = fecha_final;
            ViewBag.NumeroSAP = numero_sap;
            ViewBag.FechaInicioValidez = fecha_inicio_validez;
            ViewBag.FechaFinValidez = fecha_fin_validez;


            // Convertir id_planta a int para compararlo correctamente
            if (!int.TryParse(id_planta, out int plantaId))
            {
                return new HttpStatusCodeResult(400, "ID de planta inválido");
            }

            //obtiene la planta de BD
            var plantaBD = db.plantas.Find(plantaId);

            // Convertir fechas a DateTime antes de ejecutar la consulta
            if (!DateTime.TryParse(fecha_inicial, out DateTime fechaInicio) || !DateTime.TryParse(fecha_final, out DateTime fechaFin))
            {
                return new HttpStatusCodeResult(400, "Fechas inválidas");
            }

            if (!DateTime.TryParse(fecha_inicio_validez, out DateTime fechaInicioValidez) || !DateTime.TryParse(fecha_fin_validez, out DateTime fechaFinValidez))
            {
                return new HttpStatusCodeResult(400, "Fechas de validez inválidas");
            }

            // Obtener los registros relacionados con los filtros proporcionados desde la vista 'view_datos_base_reporte_pesadas'
            var registrosPesadas = db.view_datos_base_reporte_pesadas
                .Where(r => r.clave_planta == plantaId && r.invoiced_to == cliente && r.sap_platina == numero_sap)
                .Where(r => r.fecha >= fechaInicio && r.fecha <= fechaFin)
                .Where(r => r.fecha >= fechaInicioValidez && r.fecha <= fechaFinValidez) //busca registros dentro del periodo de validez
                .ToList();

            #region Valores atipicos
            //obtiene las diferencias
            var diferencias = registrosPesadas.Select(d => d.peso_real_pieza_neto - d.net_weight).OrderBy(d => d).ToList();
            //obtiene los pesos reales
            var pesosReales = registrosPesadas.Select(d => d.peso_real_pieza_neto).ToList();


            // Cálculo de Q1, Q3 y IQR
            int n = diferencias.Count;
            decimal q1 = (decimal)diferencias[(int)(n * 0.25)];
            decimal q3 = (decimal)diferencias[(int)(n * 0.75)];
            decimal iqr = q3 - q1;

            // Rango para detectar valores atípicos
            decimal limiteInferior = q1 - 1.5m * iqr;
            decimal limiteSuperior = q3 + 1.5m * iqr;
            #endregion
                    

            // Preparar los datos para la gráfica utilizando un ViewModel
            var datosGrafica = registrosPesadas.Select(r => new PesadaViewModel
            {
                Fecha = r.fecha?.ToString("yyyy-MM-dd") ?? string.Empty,
                PesoReal = (decimal)r.peso_real_pieza_neto,
                PesoSAP = (decimal)r.net_weight,
                Diferencia = (decimal)(r.peso_real_pieza_neto - r.net_weight),
                PorcentajeVariacion = (decimal)(((r.peso_real_pieza_neto - r.net_weight) / r.net_weight) * 100),
                EsAtipico = ((decimal)r.peso_real_pieza_neto - (decimal)r.net_weight) < limiteInferior || ((decimal)r.peso_real_pieza_neto - (decimal)r.net_weight) > limiteSuperior

            }).ToList();


            #region curva normal
            var datosSinOutliers = datosGrafica.Where(d => !d.EsAtipico).ToList();

            // Calcular la media y desviación estándar solo con los valores no atípicos
            double media = datosSinOutliers.Average(d => (double)d.PesoReal);
            double desviacionEstandar = Math.Sqrt(datosSinOutliers.Average(v => Math.Pow((double)v.PesoReal - media, 2)));

            // Calcular la mediana del peso real neto
            var pesosRealesMediana = registrosPesadas.Select(d => d.peso_real_pieza_neto).OrderBy(p => p).ToList();
            double medianaPesoReal;
            int count = pesosRealesMediana.Count;
            if (count % 2 == 0)
            {
                // Si el número de elementos es par, la mediana es el promedio de los dos valores centrales
                medianaPesoReal = ((double)pesosRealesMediana[count / 2 - 1] + (double)pesosRealesMediana[count / 2]) / 2;
            }
            else
            {
                // Si el número de elementos es impar, la mediana es el valor central
                medianaPesoReal = (double)pesosRealesMediana[count / 2];
            }

            // Calcular el peso real neto mínimo y máximo
            var pesoRealMinimo = pesosReales.Min();
            var pesoRealMaximo = pesosReales.Max();
            #endregion


            //obtiene el peso sap
            var pesoNetoSAPPromedio = registrosPesadas.Average(d => d.net_weight);
          
            //crea el modelo, para enviar los datos a la vista
            var viewModel = new AnalisisViewModel
            {
                DatosGrafica = datosGrafica,
                Planta = id_planta,
                Cliente = cliente,
                Material = numero_sap,
                FechaInicial = fecha_inicial,
                FechaFinal = fecha_final,
                TotalRegistros = registrosPesadas.Count(),
                NombrePlanta = plantaBD.descripcion,
                PesoNetoSAP = pesoNetoSAPPromedio ?? 0, //si pesoNetoSAPPromedio es null, el valor será 0
                Media = media,
                Mediana = medianaPesoReal,
             //   Maximo = pesoRealMaximo,
              //  Minimo = pesoRealMinimo,
                DesviacionEstandar = desviacionEstandar
            };

            return View(viewModel);
        }
    }
}
