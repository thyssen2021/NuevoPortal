using Bitacoras.DBUtil;
using Clases.Util;
using DocumentFormat.OpenXml.Drawing.Charts;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data.Entity;

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
            var listado = ObtieneReporte(cliente, dateInicial, dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59), id_planta, material, muestra);

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
        public List<ReportePesada> ObtieneReporte(string cliente, DateTime fecha_inicio, DateTime fecha_final, int? planta, string material, int muestra)
        {
            db.Database.CommandTimeout = 300; // Reducir el timeout a 4 minutos si las consultas se optimizan correctamente

            List<ReportePesada> listado = new List<ReportePesada>();
            string client = string.IsNullOrEmpty(cliente) ? string.Empty : cliente;
            string cadenaConexion = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            // Filtrar los registros de la vista antes de pasar al procedimiento almacenado
            var registrosPesadasQuery = db.view_datos_base_reporte_pesadas.AsNoTracking()
                .Where(r => r.clave_planta == planta && r.fecha >= fecha_inicio && r.fecha <= fecha_final);

            if (!string.IsNullOrEmpty(cliente))
            {
                registrosPesadasQuery = registrosPesadasQuery.Where(r => r.invoiced_to == cliente);
            }

            if (!string.IsNullOrEmpty(material))
            {
                registrosPesadasQuery = registrosPesadasQuery.Where(r => r.sap_platina == material);
            }

            // Ejecutar el procedimiento almacenado optimizado
            using (var conn = new SqlConnection(cadenaConexion))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_reporte_pesadas", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 300; // Tiempo de espera reducido

                        cmd.Parameters.AddWithValue("@cliente", client);
                        cmd.Parameters.AddWithValue("@fecha_inicio", fecha_inicio);
                        cmd.Parameters.AddWithValue("@fecha_fin", fecha_final);
                        cmd.Parameters.AddWithValue("@id_planta", planta ?? 0);
                        cmd.Parameters.AddWithValue("@material", string.IsNullOrEmpty(material) ? string.Empty : material);
                        cmd.Parameters.AddWithValue("@muestra", muestra);

                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var pesada = new ReportePesada
                                {
                                    id = Convert.ToInt32(dr["id"]),
                                    Cliente = Convert.ToString(dr["invoiced_to"]),
                                    SAP_Platina = Convert.ToString(dr["sap_platina"]),
                                    SAP_Rollo = Convert.ToString(dr["sap_rollo"]),
                                    Type_of_Metal = Convert.ToString(dr["tipo_metal"]),
                                    Peso_Neto_SAP = GetNullableDouble(dr, "peso_neto_sap"),
                                    Peso_Neto__mean_ = GetNullableDouble(dr, "peso_neto_mean"),
                                    Peso_Neto_stdev = GetNullableDouble(dr, "peso_neto_stdev"),
                                    count = GetNullableInt(dr, "count"),
                                    Thickness = GetNullableDouble(dr, "Thickness"),
                                    Width = GetNullableDouble(dr, "Width"),
                                    Advance = GetNullableDouble(dr, "Advance"),
                                    peso_teorico_acero = GetNullableDouble(dr, "peso_teorico"),
                                    Peso_Bruto_SAP = GetNullableDouble(dr, "peso_bruto_sap"),
                                    Peso_Bruto__mean_ = GetNullableDouble(dr, "peso_bruto_mean"),
                                    Total_de_piezas = GetNullableDouble(dr, "total_piezas"),
                                    Peso_Etiqueta = GetNullableDouble(dr, "peso_etiqueta"),
                                    count_muestra = GetNullableInt(dr, "tamano_muestra"),
                                    tamano_pieza = Convert.ToString(dr["tamano_pieza"]),
                                    porcentaje_tolerancia = GetNullableDouble(dr, "porcentaje_tolerancia"),
                                    porcentaje_diferencia_muestra = GetNullableDouble(dr, "diferencia_porcentaje_muestra_rollo"),
                                    porcentaje_diferencia_total = GetNullableDouble(dr, "diferencia_porcentaje_total_rollos"),
                                    peso_neto_mean_muestra = GetNullableDouble(dr, "peso_neto_mean_muestra"),
                                    muestra_dentro_rango = GetNullableBool(dr, "muestra_dentro_del_rango"),
                                    fecha_inicio_validez_peso_bom = Convert.ToDateTime(dr["fecha_inicio_validez_peso_bom"]),
                                    fecha_fin_validez_peso_bom = Convert.ToDateTime(dr["fecha_fin_validez_peso_bom"]),
                                    total_piezas_muestra = GetNullableDouble(dr, "total_piezas_muestra")
                                };


                                listado.Add(pesada);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("No se puede realizar la conexion a la base de datos por: " + e.Message);
                }
            }

            // Realizar los cálculos fuera del ciclo de lectura
            foreach (var pesada in listado)
            {
                var registrosPesadas = registrosPesadasQuery
                    .Where(r => r.invoiced_to == pesada.Cliente && r.sap_platina == pesada.SAP_Platina)
                    .Where(r => r.fecha >= pesada.fecha_inicio_validez_peso_bom && r.fecha <= pesada.fecha_fin_validez_peso_bom) // buscar registros dentro del periodo de validez
                    .AsNoTracking() // No seguimiento para mejor rendimiento
                    .ToList();

                if (registrosPesadas.Any())
                {
                    // Calcular las diferencias
                    var diferencias = registrosPesadas.Select(d => d.peso_real_pieza_neto - d.net_weight).OrderBy(d => d).ToList();
                    int n = diferencias.Count;
                    decimal q1 = (decimal)diferencias[(int)(n * 0.25)];
                    decimal q3 = (decimal)diferencias[(int)(n * 0.75)];
                    decimal iqr = q3 - q1;

                    // Rango para detectar valores atípicos
                    decimal limiteInferior = q1 - 1.5m * iqr;
                    decimal limiteSuperior = q3 + 1.5m * iqr;

                    // Filtrar los datos sin valores atípicos
                    var datosSinOutliers = registrosPesadas.Where(r =>
                        ((decimal)r.peso_real_pieza_neto - (decimal)r.net_weight) >= limiteInferior &&
                        ((decimal)r.peso_real_pieza_neto - (decimal)r.net_weight) <= limiteSuperior
                    ).ToList();

                    // Calcular `PromedioPorcentajeDiferencia`
                    decimal promedioPorcentajeDiferencia = datosSinOutliers.Any() ?
                        (decimal)datosSinOutliers.Average(r => ((r.peso_real_pieza_neto - r.net_weight) / r.net_weight) * 100) : 0;

                    // Calcular `AdvertenciaCambioPeso`
                    bool advertenciaCambioPeso = false;
                    decimal pesoSAPPromedio = datosSinOutliers.Any() ? (decimal)datosSinOutliers.Average(r => r.net_weight) : 0;

                    if ((pesoSAPPromedio < 8 && Math.Abs(promedioPorcentajeDiferencia) > 2) ||
                        (pesoSAPPromedio >= 8 && pesoSAPPromedio < 20 && Math.Abs(promedioPorcentajeDiferencia) > 1.5m) ||
                        (pesoSAPPromedio >= 20 && Math.Abs(promedioPorcentajeDiferencia) > 1))
                    {
                        advertenciaCambioPeso = true;
                    }

                    // Calcular las piezas x diferencia sin valores atípicos
                    double piezas_x_diferencias = datosSinOutliers.Sum(item => (double)(((item.peso_real_pieza_neto ?? 0) - (item.net_weight ?? 0)) * (item.total_piezas ?? 0)));

                    // Asignar valores
                    pesada.PromedioPorcentajeDiferencia = (double)promedioPorcentajeDiferencia;
                    pesada.AdvertenciaCambioPeso = advertenciaCambioPeso;
                    pesada.piezas_x_diferencia_sin_atipicos = piezas_x_diferencias;
                }
            }

            return listado;
        }

        [NonAction]
        private double? GetNullableDouble(SqlDataReader dr, string columnName)
        {
            return dr[columnName] != DBNull.Value ? (double?)Convert.ToDouble(dr[columnName]) : null;
        }

        [NonAction]
        private int? GetNullableInt(SqlDataReader dr, string columnName)
        {
            return dr[columnName] != DBNull.Value ? (int?)Convert.ToInt32(dr[columnName]) : null;
        }

        [NonAction]
        private bool? GetNullableBool(SqlDataReader dr, string columnName)
        {
            return dr[columnName] != DBNull.Value ? (bool?)Convert.ToBoolean(dr[columnName]) : null;
        }

        [NonAction]
        private bool TryParseDate(string dateString, out DateTime date)
        {
            return DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        [NonAction]
        private SelectList ObtenerClientesSelectList(int? id_planta)
        {
            db.Database.CommandTimeout = 180; // Tiempo de espera en segundos

            var clientes = db.view_datos_base_reporte_pesadas
                             .Where(x => x.clave_planta == id_planta)
                             .Select(x => x.invoiced_to)
                             .Distinct()
                             .OrderBy(x => x)
                             .ToList();

            return new SelectList(clientes.Select(p => new SelectListItem { Text = p, Value = p }), "Value", "Text");
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
                Fecha = r.fecha?.ToString("yyyy-MM-ddTHH:mm:ss") ?? string.Empty,
                PesoReal = (decimal)r.peso_real_pieza_neto,
                PesoSAP = (decimal)r.net_weight,
                Diferencia = (decimal)(r.peso_real_pieza_neto - r.net_weight),
                PorcentajeVariacion = (decimal)(((r.peso_real_pieza_neto - r.net_weight) / r.net_weight) * 100),
                EsAtipico = ((decimal)r.peso_real_pieza_neto - (decimal)r.net_weight) < limiteInferior || ((decimal)r.peso_real_pieza_neto - (decimal)r.net_weight) > limiteSuperior,
                TotalPiezas = r.total_piezas ?? 0
            }).ToList();


            #region curva normal
            var datosSinOutliers = datosGrafica.Where(d => !d.EsAtipico).ToList();

            // Calcular la media y desviación estándar solo con los valores no atípicos
            double media = datosSinOutliers.Average(d => (double)d.PesoReal);
            double desviacionEstandar = Math.Sqrt(datosSinOutliers.Average(v => Math.Pow((double)v.PesoReal - media, 2)));

            // Calcular la mediana del peso real neto
            var pesosRealesMediana = datosSinOutliers.Select(x => x.PesoReal).OrderBy(p => p).ToList(); // valores atipicos
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

            // Calcular el total de piezas de todos los valores atípicos
            var totalPiezasSinAtipicos = datosSinOutliers.Sum(d => d.TotalPiezas);

            // Multiplica el toal de piezas por la diferencia y la suma
            var totalPiezasSinAtipicosXdiferencia = datosSinOutliers.Sum(d => d.Diferencia * (decimal)d.TotalPiezas);


            // Calcular el promedio del porcentaje de variación sin valores atípicos
            decimal promedioPorcentajeDiferencia = datosSinOutliers.Any() ? datosSinOutliers.Average(d => d.PorcentajeVariacion) : 0;

            // Indicador definitivo basado en el promedio de los porcentajes de diferencia
            bool advertenciaCambioPeso = false;
            decimal pesoSAPPromedio = datosSinOutliers.Any() ? datosSinOutliers.Average(d => d.PesoSAP) : 0;

            if ((pesoSAPPromedio < 8 && Math.Abs(promedioPorcentajeDiferencia) > 2) ||
                (pesoSAPPromedio >= 8 && pesoSAPPromedio < 20 && Math.Abs(promedioPorcentajeDiferencia) > 1.5m) ||
                (pesoSAPPromedio >= 20 && Math.Abs(promedioPorcentajeDiferencia) > 1))
            {
                advertenciaCambioPeso = true;
            }

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
                Maximo = pesoRealMaximo ?? 0,
                Minimo = pesoRealMinimo ?? 0,
                DesviacionEstandar = desviacionEstandar,
                TotalPiezasSinAtipicos = totalPiezasSinAtipicos,
                TotalPiezasSinAtipicosXdiferencia = totalPiezasSinAtipicosXdiferencia,
                PromedioPorcentajeDiferencia = promedioPorcentajeDiferencia,
                AdvertenciaCambioPeso = advertenciaCambioPeso

            };

            return View(viewModel);
        }

        // GET: Analisis
        public AnalisisViewModel ObtieneDatosAnalisis(string id_planta, string cliente, string fecha_inicial, string fecha_final, string numero_sap, string fecha_inicio_validez, string fecha_fin_validez)
        {

            // Convertir id_planta a int para compararlo correctamente
            if (!int.TryParse(id_planta, out int plantaId))
            {
                //return new HttpStatusCodeResult(400, "ID de planta inválido");
                return new AnalisisViewModel();
            }

            //obtiene la planta de BD
            var plantaBD = db.plantas.Find(plantaId);

            // Convertir fechas a DateTime antes de ejecutar la consulta
            if (!DateTime.TryParse(fecha_inicial, out DateTime fechaInicio) || !DateTime.TryParse(fecha_final, out DateTime fechaFin))
            {
                return new AnalisisViewModel();
                // return new HttpStatusCodeResult(400, "Fechas inválidas");
            }

            if (!DateTime.TryParse(fecha_inicio_validez, out DateTime fechaInicioValidez) || !DateTime.TryParse(fecha_fin_validez, out DateTime fechaFinValidez))
            {
                return new AnalisisViewModel();
                // return new HttpStatusCodeResult(400, "Fechas de validez inválidas");
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
                EsAtipico = ((decimal)r.peso_real_pieza_neto - (decimal)r.net_weight) < limiteInferior || ((decimal)r.peso_real_pieza_neto - (decimal)r.net_weight) > limiteSuperior,
                TotalPiezas = r.total_piezas ?? 0
            }).ToList();

            var datosSinOutliers = datosGrafica.Where(d => !d.EsAtipico).ToList();

            // Calcular el promedio del porcentaje de variación sin valores atípicos
            decimal promedioPorcentajeDiferencia = datosSinOutliers.Any() ? datosSinOutliers.Average(d => d.PorcentajeVariacion) : 0;

            // Indicador definitivo basado en el promedio de los porcentajes de diferencia
            bool advertenciaCambioPeso = false;
            decimal pesoSAPPromedio = datosSinOutliers.Any() ? datosSinOutliers.Average(d => d.PesoSAP) : 0;

            if ((pesoSAPPromedio < 8 && Math.Abs(promedioPorcentajeDiferencia) > 2) ||
                (pesoSAPPromedio >= 8 && pesoSAPPromedio < 20 && Math.Abs(promedioPorcentajeDiferencia) > 1.5m) ||
                (pesoSAPPromedio >= 20 && Math.Abs(promedioPorcentajeDiferencia) > 1))
            {
                advertenciaCambioPeso = true;
            }

            //crea el modelo, para enviar los datos a la vista
            var viewModel = new AnalisisViewModel
            {
                PromedioPorcentajeDiferencia = promedioPorcentajeDiferencia,
                AdvertenciaCambioPeso = advertenciaCambioPeso
            };

            return viewModel;
        }
    }
}
