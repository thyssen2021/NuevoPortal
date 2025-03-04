using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Newtonsoft.Json;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class budget_cantidad_forecastController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: budget_cantidad_forecast
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.BG_CONTROLLING))
                return View("../Home/ErrorPermisos");

            // Cargar datos para el combo de Año Fiscal
            var fiscalYears = db.budget_anio_fiscal
                                .ToList();
            // Cargar datos para el combo de Centro de Costo (se filtran los activos)
            var centrosCosto = db.budget_centro_costo
                                 .Where(c => c.activo)
                                 .ToList();

            ViewBag.FiscalYears = new SelectList(fiscalYears, "id", nameof(budget_anio_fiscal.ConcatAnio));
            ViewBag.CentrosCosto = new SelectList(centrosCosto, "id", nameof(budget_centro_costo.ConcatCentro));

            return View();
        }


        // GET: ResponsableBudget/EditCentroPresente
        public ActionResult EditCentro(int? id_centro_costo, int? id_fiscal_year, bool proximo = false, bool info = false, bool controlling = true, bool import = false)
        {

            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id_centro_costo == null)  //corre
                {
                    return View("../Error/BadRequest");
                }

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }
                //obtiene centro de costo
                budget_centro_costo centroCosto = db.budget_centro_costo.Find(id_centro_costo);
                if (centroCosto == null)
                    return View("../Error/NotFound");

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();
                //verifica que el usuario este registrado como capturista
                if (!db.budget_responsables.Any(x => x.id_responsable == empleado.id && centroCosto.id == x.id_budget_centro_costo) && !TieneRol(TipoRoles.BG_CONTROLLING))
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede acceder a esta Sección!";
                    ViewBag.Descripcion = "Este usuario no se encuentra asociado a este centro de costo.";

                    return View("../Home/ErrorGenerico");
                }

                //obtiene el año fiscal actual (forecast)
                budget_anio_fiscal anio_Fiscal_actual = db.budget_anio_fiscal.Find(id_fiscal_year);
                if (anio_Fiscal_actual == null)
                    return View("../Error/NotFound");



                //obtiene el id_rel_centro_costo del forecast
                budget_rel_fy_centro rel_fy_centro_presente = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_actual.id).FirstOrDefault();
                if (rel_fy_centro_presente == null)
                {
                    //si no existe crea el rel anio forecast
                    rel_fy_centro_presente = new budget_rel_fy_centro
                    {
                        id_anio_fiscal = anio_Fiscal_actual.id,
                        id_centro_costo = centroCosto.id,
                        estatus = true //activado por defecto
                    };

                    db.budget_rel_fy_centro.Add(rel_fy_centro_presente);
                    //guarda en base de datos el centro creado
                    db.SaveChanges();
                }

                rel_fy_centro_presente.budget_anio_fiscal = anio_Fiscal_actual;


                #region cabeceras HT
                //cabeceras HT FORECAST
                List<string> headersForecast = new List<string> { "SAP Account", "Name", "Mapping" };
                var cabeceraObject = new object[13];
                var cabeceraObjectActual = new object[13];
                var cabeceraObjectBudget = new object[13];
                cabeceraObject[0] = new
                {
                    label = "SAP ACCOUNT",
                    colspan = 3
                };
                cabeceraObjectActual[0] = cabeceraObject[0];
                cabeceraObjectBudget[0] = cabeceraObject[0];
                DateTime fecha = new DateTime(rel_fy_centro_presente.budget_anio_fiscal.anio_inicio, rel_fy_centro_presente.budget_anio_fiscal.mes_inicio, 1);
                for (int i = 0; i < 12; i++)
                {
                    //agrega cabecera para tipo moneda
                    headersForecast.Add("MXN");
                    headersForecast.Add("USD");
                    headersForecast.Add("EUR");
                    headersForecast.Add("Local (USD)");
                    //agrega cabecera para meses
                    cabeceraObject[i + 1] = new
                    {
                        label = string.Format("{0} {1}", rel_fy_centro_presente.budget_anio_fiscal.isActual(fecha.Month), fecha.ToString("MMM yy").ToUpper()),
                        colspan = 4
                    };
                    cabeceraObjectActual[i + 1] = new
                    {
                        label = string.Format("ACT {0}", fecha.AddYears(-1).ToString("MMM yy").ToUpper()),
                        colspan = 4
                    };
                    cabeceraObjectBudget[i + 1] = new
                    {
                        label = string.Format("BG {0}", fecha.AddYears(1).ToString("MMM yy").ToUpper()),
                        colspan = 4
                    };
                    fecha = fecha.AddMonths(1);
                }
                headersForecast.Add("Total (MXN)");
                headersForecast.Add("Total (USD)");
                headersForecast.Add("Total (EUR)");
                headersForecast.Add("Total (Local - USD)");
                headersForecast.Add("Target");
                headersForecast.Add("Comentarios");
                headersForecast.Add("AplicaFormula");
                headersForecast.Add("aplicaMXN");
                headersForecast.Add("AplicaUSD");
                headersForecast.Add("AplicaEUR");
                headersForecast.Add("Soporte");
                headersForecast.Add("AplicaGastos");

                ViewBag.HeadersForecast1_actual = JsonConvert.SerializeObject(cabeceraObjectActual);
                ViewBag.HeadersForecast1_budget = JsonConvert.SerializeObject(cabeceraObjectBudget);
                ViewBag.HeadersForecast1 = JsonConvert.SerializeObject(cabeceraObject);
                ViewBag.HeadersForecast2 = headersForecast.ToArray();

                //obtiene todos lo valores sugeridos
                List<string[]> listaSugeridos = new List<string[]>();
                List<int> listMeses = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                List<string> listMoneda = new List<string> { "MXN", "USD", "EUR" };

                foreach (var sapAccount in rel_fy_centro_presente.budget_cantidad_forecast.Where(x => x.budget_cuenta_sap.aplica_gastos_mantenimiento).Select(x => x.budget_cuenta_sap).Distinct())
                {
                    foreach (var mes in listMeses)
                    {
                        foreach (var moneda in listMoneda)
                        {
                            double sugerido = 0;

                            //obtiene los ultimos tres gastos
                            var listGastos = sapAccount.budget_conceptos_mantenimiento.Where(x => x.budget_rel_fy_centro.id_centro_costo == rel_fy_centro_presente.id_centro_costo && x.mes == mes
                            && x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio < rel_fy_centro_presente.budget_anio_fiscal.anio_inicio && x.gasto != 0
                            && x.moneda == moneda
                            ).OrderByDescending(x => x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio).Take(3).ToList();

                            sugerido = listGastos.Count == 0 ? 0 : listGastos.Sum(x => x.gasto - (x.one_time.HasValue ? x.one_time.Value : 0)) / listGastos.Count;

                            listaSugeridos.Add(new string[4] { sapAccount.sap_account, moneda, mes.ToString(), Math.Round(sugerido, 2).ToString() });
                        }
                    }
                }
                ViewBag.GastosSugeridos = listaSugeridos.ToArray();

                #endregion

                ViewBag.centroCosto = centroCosto;
                ViewBag.rel_presente = rel_fy_centro_presente;
                ViewBag.numCuentasSAP = db.budget_cuenta_sap.Where(x => x.activo).Count();


                ViewBag.controlling = controlling;
                ViewBag.info = info;
                return View(centroCosto); //presente  editable

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }


        /// <summary>
        /// Carga los datos iniciales del año indicado
        /// </summary>
        /// <param name="id_fy"></param>
        /// <returns></returns>
        public JsonResult CargaForecastFY(int? id_fy, int? id_centro_costo)
        {

            var valoresListAnioActual = db.view_valores_forecast.Where(x => x.id_anio_fiscal == id_fy && x.id_centro_costo == id_centro_costo).ToList();
            valoresListAnioActual = AgregaCuentasSAPFaltantes(valoresListAnioActual, id_fy.Value, id_centro_costo.Value).OrderBy(x => x.sap_account).ToList();
            var fy = db.budget_anio_fiscal.Find(id_fy);

            var jsonData = new List<object>();

            var sapAccountsList = db.budget_cuenta_sap.ToList();

            //obtiene el id fy cc
            var fy_cc = fy.budget_rel_fy_centro.FirstOrDefault(x => x.id_anio_fiscal == id_fy && x.id_centro_costo == id_centro_costo);

            //obtiene los documento
            var doctosList = fy_cc.budget_rel_documento;

            //obtiene el listado de los CeCo en los que aplica gastos de manto
            List<int> ccListManto = db.budget_conceptos_mantenimiento.Select(x => x.budget_rel_fy_centro.id_centro_costo).Distinct().ToList();
            List<string> cuentasListManto = db.budget_conceptos_mantenimiento.Select(x => x.budget_cuenta_sap.sap_account).Distinct().ToList();

            // 1. Obtén todos los registros de tipo de cambio para el año fiscal actual (para meses 1 a 12)
            var tiposCambio = fy.budget_rel_tipo_cambio_fy_forecast
                .Where(x => x.mes >= 1 && x.mes <= 12)
                .ToList();

            // 2. Crea un diccionario que asocie a cada mes los valores de USD/MXN y EUR/USD.
            Dictionary<int, (decimal usd_mxn, decimal eur_usd)> tipoCambioPorMes = new Dictionary<int, (decimal, decimal)>();
            for (int mes = 1; mes <= 12; mes++)
            {
                // Busca para cada mes el tipo de cambio con id_tipo_cambio=1 y 2
                var cambioUsdMxn = tiposCambio.FirstOrDefault(x => x.id_tipo_cambio == 1 && x.mes == mes);
                var cambioEurUsd = tiposCambio.FirstOrDefault(x => x.id_tipo_cambio == 2 && x.mes == mes);
                decimal usd_mxn = cambioUsdMxn != null ? (decimal)cambioUsdMxn.cantidad : 0;
                decimal eur_usd = cambioEurUsd != null ? (decimal)cambioEurUsd.cantidad : 0;
                tipoCambioPorMes[mes] = (usd_mxn, eur_usd);
            }

            //id_centro_costo y fy.id son fijos para el bucle.
            var targets = db.budget_target
                .Where(t => t.id_centro_costo == id_centro_costo &&
                            t.id_anio_fiscal == fy.id &&
                            t.activado)
                .ToList();

            // Crear un diccionario para búsquedas rápidas.
            var targetDictionary = targets.ToDictionary(
                t => t.id_cuenta_sap,  // llave: id_cuenta_sap (ajusta si la combinación debe incluir más de un valor)
                t => t.target);

            // 3.Al generar la data para cada cuenta(por ejemplo, en el bucle que recorre 'valoresListAnioActual'),
            // suponiendo que ya tienes la lógica para obtener el mes correspondiente de la propiedad de la cuenta.
            // Por ejemplo, si tienes propiedades llamadas "OCTUBRE_MXN", "OCTUBRE", "OCTUBRE_EUR", y asumes que OCTUBRE corresponde al mes 10,
            // entonces, para ese grupo, usarías:

            for (int i = 0; i < valoresListAnioActual.Count(); i++)
            {

                var rowData = new List<string>();
                var v = valoresListAnioActual[i];

                // Las primeras tres columnas: SAP Account, Name y Mapping
                rowData.Add(v.sap_account);
                rowData.Add(v.name);
                rowData.Add(v.mapping_bridge);

                // Supongamos que recorres los 12 meses en orden fiscal.
                // Aquí se define un arreglo de nombres de meses (en mayúsculas) según el idioma, por ejemplo:
                string[] nombresMeses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                               "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
                // Aquí deberás definir el orden según el mes de inicio de tu año fiscal. Para este ejemplo, supongamos que es de octubre:
                int[] mesesOrdenados = { 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                // Para cada mes, agrega 4 columnas: MXN, USD, EUR y Local (USD)
                int fixedColumns = 3; // columnas fijas (A, B, C)

                for (int j = 0; j < 12; j++)
                {
                    int mesActual = mesesOrdenados[j];
                    // Obtén el nombre en español para el mes. (Puedes ajustar si las propiedades de tu objeto usan otro formato)
                    string nombreMes = nombresMeses[mesActual - 1];

                    // Se asume que en tu objeto 'v' tienes propiedades nombradas como: 
                    // "OCTUBRE_MXN", "OCTUBRE", "OCTUBRE_EUR" para el mes de octubre, etc.
                    string valorMXN = v.GetType().GetProperty(nombreMes + "_MXN")?.GetValue(v, null)?.ToString() ?? "";
                    string valorUSD = v.GetType().GetProperty(nombreMes)?.GetValue(v, null)?.ToString() ?? "";
                    string valorEUR = v.GetType().GetProperty(nombreMes + "_EUR")?.GetValue(v, null)?.ToString() ?? "";

                    rowData.Add(valorMXN);
                    rowData.Add(valorUSD);
                    rowData.Add(valorEUR);

                    // Calcula el índice base para este mes:
                    int baseColIndex = fixedColumns + (j * 4) + 1;
                    // Así, para j=0, baseColIndex = 4 (columna D)
                    // para j=1, baseColIndex = 8 (columna H), etc.

                    string colMXN = ResponsableBudgetController.GetExcelColumnName(baseColIndex);      // Ej.: "D" o "H"
                    string colUSD = ResponsableBudgetController.GetExcelColumnName(baseColIndex + 1);    // Ej.: "E" o "I"
                    string colEUR = ResponsableBudgetController.GetExcelColumnName(baseColIndex + 2);    // Ej.: "F" o "J"

                    // Genera la fórmula para la columna "Local (USD)" según el mes
                    string formulaLocal = "";
                    // Verifica si para este mes existen ambos tipos de cambio definidos (o con valor distinto de 0)
                    if (tipoCambioPorMes.ContainsKey(mesActual) &&
                        tipoCambioPorMes[mesActual].usd_mxn != 0 && tipoCambioPorMes[mesActual].eur_usd != 0)
                    {
                        // Usa la fila i+1 (suponiendo que 'i' es el índice de la fila actual) para generar la fórmula
                        formulaLocal = string.Format("=ROUND(({0}{1}/{2})+{3}{1}+({4}{1}*{5}),2)",
                            colMXN,
                            i + 1,
                            tipoCambioPorMes[mesActual].usd_mxn,
                            colUSD,
                            colEUR,
                            tipoCambioPorMes[mesActual].eur_usd);
                    }
                    else
                    { // Si no se tienen ambos tipos de cambio, se deja la fórmula vacía para que el usuario ingrese el dato manualmente.
                        formulaLocal = v.GetType().GetProperty(nombreMes + "_USD_LOCAL")?.GetValue(v, null)?.ToString() ?? ""; ;
                    }

                    rowData.Add(formulaLocal);
                }

                rowData.Add(string.Format("= D{0} + H{0} + L{0} + P{0} + T{0} + X{0} + AB{0} + AF{0} + AJ{0} + AN{0} + AR{0} + AV{0}", i + 1));
                rowData.Add(string.Format("= E{0} + I{0} + M{0} + Q{0} + U{0} + Y{0} + AC{0} + AG{0} + AK{0} + AO{0} + AS{0} + AW{0}", i + 1));
                rowData.Add(string.Format("= F{0} + J{0} + N{0} + R{0} + V{0} + Z{0} + AD{0} + AH{0} + AL{0} + AP{0} + AT{0} + AX{0}", i + 1));
                rowData.Add(string.Format("= G{0} + K{0} + O{0} + S{0} + W{0} + AA{0} + AE{0} + AI{0} + AM{0} + AQ{0} + AU{0} + AY{0}", i + 1));

                // Si la llave es solo id_cuenta_sap y asumes que para cada cuenta hay un solo target
                var targetValue = targetDictionary.ContainsKey(v.id_cuenta_sap)
                    ? targetDictionary[v.id_cuenta_sap]
                    : default(double); // o 0, según convenga

                // Agregar targetValue al arreglo de datos para la fila, 
                // asegurándote de colocarlo en la posición correcta.
                rowData.Add(targetValue.ToString());
                rowData.Add(v.Comentario);
                rowData.Add(sapAccountsList.Any(x => x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_formula == true).ToString());

                bool editableCtaManto = cuentasListManto.Any(x => x == valoresListAnioActual[i].sap_account) && !ccListManto.Any(x => x == id_centro_costo) ? false : true;

                rowData.Add(!editableCtaManto ? editableCtaManto.ToString() : sapAccountsList.Any(x => x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_mxn == true).ToString());
                rowData.Add(!editableCtaManto ? editableCtaManto.ToString() : sapAccountsList.Any(x => x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_usd == true).ToString());
                rowData.Add(!editableCtaManto ? editableCtaManto.ToString() : sapAccountsList.Any(x => x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_eur == true).ToString());

                //valor por defecto
                string btnDoc = "<button class='btn-documento-pendiente' onclick=\"muestraSoporte(" + fy_cc.id + ", " +
                    valoresListAnioActual[i].id_cuenta_sap
                    + ")\">" + (fy_cc.estatus ? "Agregar" : "Ver") + "</button>";

                //valida si ya existe un documento para esta cuenta sap y rel fy cc
                if (doctosList.Any(x => x.id_cuenta_sap == valoresListAnioActual[i].id_cuenta_sap))
                {
                    btnDoc = "<button class='btn-documento-agregado' onclick=\"muestraSoporte(" + fy_cc.id + ", " +
                        valoresListAnioActual[i].id_cuenta_sap
                        + ")\">" + (fy_cc.estatus ? "Modificar" : "Ver") + "</button>";
                }

                rowData.Add(btnDoc);
                rowData.Add(sapAccountsList.Any(x => x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_gastos_mantenimiento == true).ToString());

                jsonData.Add(rowData.ToArray());

            }
            int filas = valoresListAnioActual.Count();
            //fila  para sumatorias
            jsonData.Add(new[] {
                string.Empty,
                string.Empty,
                string.Empty,
                string.Format("=SUM({0}{1}:{0}{2})","D", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","E", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","F", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","G", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","H", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","I", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","J", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","K", 1, filas),//  
                string.Format("=SUM({0}{1}:{0}{2})","L", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","M", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","N", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","O", 1, filas),// 
                string.Format("=SUM({0}{1}:{0}{2})","P", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","Q", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","R", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","S", 1, filas),//    
                string.Format("=SUM({0}{1}:{0}{2})","T", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","U", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","V", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","W", 1, filas),//    
                string.Format("=SUM({0}{1}:{0}{2})","X", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","Y", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","Z", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AA", 1, filas),//    
                string.Format("=SUM({0}{1}:{0}{2})","AB", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AC", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AD", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AE", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","AF", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AG", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AH", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AI", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","AJ", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AK", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AL", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AM", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","AN", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AO", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AP", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AQ", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","AR", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AS", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AT", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AU", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","AV", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AW", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AX", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AY", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","AZ", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","BA", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","BB", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","BC", 1, filas),

            });

            return Json(jsonData.ToArray(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditCentroHT(int? id, List<object[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var response = new object[1];

            List<budget_rel_comentarios> lista_comentarios_form = new List<budget_rel_comentarios>();

            //convierte el list de arrays en objetos budget_cantidad_forecast
            List<budget_cantidad_forecast> lista_cantidad_form = ConvierteArrayACantidades(dataListFromTable, id, lista_comentarios_form);

            //obtiene el listado actual de la BD
            List<budget_cantidad_forecast> lista_cantidad_BD = db.budget_cantidad_forecast.Where(x => x.id_budget_rel_fy_centro == id).ToList();

            //obtiene el listado actual de la BD
            List<budget_rel_comentarios> lista_comentarios_BD = db.budget_rel_comentarios.Where(x => x.id_budget_rel_fy_centro == id).ToList();

            //borra todos los comentarios, (se agregan en la segunda llamada ajax)
            lista_cantidad_BD = lista_cantidad_BD
                    .Select(x =>
                    {
                        x.comentario = null;
                        return x;
                    }).ToList();

            //crea, modifica o elimina cantidad
            try
            {
                response[0] = new { result = "OK", icon = "success", message = "Se guardaron los cambios correctamente." };

                //Recorre todas las cantidades de la tabla
                for (int i = 0; i < lista_cantidad_form.Count; i++)
                {
                    budget_cantidad_forecast itemBD = lista_cantidad_BD.FirstOrDefault(x =>
                                                x.id_cuenta_sap == lista_cantidad_form[i].id_cuenta_sap
                                                && x.mes == lista_cantidad_form[i].mes
                                                && x.currency_iso == lista_cantidad_form[i].currency_iso
                                                && x.moneda_local_usd == lista_cantidad_form[i].moneda_local_usd
                                            );
                    //EXISTE
                    if (itemBD != null)
                    {
                        //UPDATE
                        if (itemBD.cantidad != lista_cantidad_form[i].cantidad)
                            itemBD.cantidad = lista_cantidad_form[i].cantidad;
                    }
                    else  //CREATE
                    {
                        db.budget_cantidad_forecast.Add(lista_cantidad_form[i]);
                    }
                }

                //DELETE
                //elimina aquellos que no aparezcan en los enviados
                List<budget_cantidad_forecast> toDeleteList = lista_cantidad_BD.Where(x => !lista_cantidad_form.Any(y => y.id_cuenta_sap == x.id_cuenta_sap
                                && y.mes == x.mes
                                && y.currency_iso == x.currency_iso
                                && y.moneda_local_usd == x.moneda_local_usd
                                )).ToList();

                //borra los conceptos que inpiden el borrado
                foreach (var item in toDeleteList)
                    db.budget_rel_conceptos_cantidades_forecast.RemoveRange(item.budget_rel_conceptos_cantidades_forecast);

                db.budget_cantidad_forecast.RemoveRange(toDeleteList);


                /*
                //crea, modifica o elimina COMENTARIOS generales
                for (int i = 0; i < lista_comentarios_form.Count; i++)
                {
                    var itemBD = lista_comentarios_BD.FirstOrDefault(x => x.id_cuenta_sap == lista_comentarios_form[i].id_cuenta_sap);
                    //EXISTE
                    if (itemBD != null)
                    {
                        //UPDATE
                        if (itemBD.comentarios != lista_comentarios_form[i].comentarios)
                            itemBD.comentarios = lista_comentarios_form[i].comentarios;
                    }
                    else  //CREATE
                    {
                        db.budget_rel_comentarios.Add(lista_comentarios_form[i]);
                    }
                }

                //DELETE
                //elimina aquellos que no aparezcan en los enviados
                List<budget_rel_comentarios> toDeleteListComentarios = lista_comentarios_BD.Where(x => !lista_comentarios_form.Any(y => y.id_cuenta_sap == x.id_cuenta_sap)).ToList();
                db.budget_rel_comentarios.RemoveRange(toDeleteListComentarios);
                */


                try
                {
                    db.SaveChanges();
                    response[0] = new { result = "OK", icon = "success", message = "Se guardaron los cambios correctamente" };
                }
                catch (Exception e)
                {
                    response[0] = new { result = "ERROR", icon = "error", message = e.Message };
                }


            }
            catch (Exception e)
            {
                response[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private List<budget_cantidad_forecast> ConvierteArrayACantidades(List<object[]> data, int? id_rel_fy_cc, List<budget_rel_comentarios> comentarios_general)
        {
            var rel_fy_cc = db.budget_rel_fy_centro.Find(id_rel_fy_cc);

            var cuestasSAPBD = db.budget_cuenta_sap.ToList();

            List<budget_cantidad_forecast> resultado = new List<budget_cantidad_forecast> { };

            #region cabeceras HT
            //cabeceras HT FORECAST
            List<string> headersForecast = new List<string> { "SAP_ACCOUNT", "Name", "Mapping" };


            DateTime fecha = new DateTime(rel_fy_cc.budget_anio_fiscal.anio_inicio, rel_fy_cc.budget_anio_fiscal.mes_inicio, 1);
            for (int i = 0; i < 12; i++)
            {
                //agrega cabecera para tipo moneda
                headersForecast.Add("MXN");
                headersForecast.Add("USD");
                headersForecast.Add("EUR");
                headersForecast.Add("USD");

                fecha = fecha.AddMonths(1);
            }
            headersForecast.Add("Total (MXN)");
            headersForecast.Add("Total (USD)");
            headersForecast.Add("Total (EUR)");
            headersForecast.Add("Total (Local USD)"); headersForecast.Add("Totals");
            headersForecast.Add("Target");
            headersForecast.Add("Comments");
            headersForecast.Add("AplicaFormula");

            #endregion

            //listado de encabezados
            string[] encabezados = headersForecast.ToArray();

            //recorre todos los arrays recibidos
            foreach (var array in data)
            {

                //variables generales
                string sap_account = array[Array.IndexOf(encabezados, "SAP_ACCOUNT")].ToString();
                int id_sap_account = !string.IsNullOrEmpty(sap_account) ? cuestasSAPBD.FirstOrDefault(x => x.sap_account == sap_account).id : 0;
                DateTime fechaArray = new DateTime(rel_fy_cc.budget_anio_fiscal.anio_inicio, rel_fy_cc.budget_anio_fiscal.mes_inicio, 1);
                int col = 3;

                //recorre cada uno de los meses
                if (!string.IsNullOrEmpty(sap_account))
                    for (int i = 0; i < 12; i++)
                    {

                        decimal cantidadTemporal = 0;

                        for (int j = 0; j < 4; j++)
                        {
                            bool puedeConvertir = decimal.TryParse(array[col + j] != null ? array[col + j].ToString() : string.Empty, out cantidadTemporal);

                            if (array[col + j] != null && !string.IsNullOrEmpty(array[col + j].ToString()) && cantidadTemporal != 0)
                            {
                                resultado.Add(new budget_cantidad_forecast
                                {
                                    id_budget_rel_fy_centro = rel_fy_cc.id,
                                    id_cuenta_sap = id_sap_account,
                                    mes = fecha.Month,
                                    currency_iso = encabezados[col + j],
                                    cantidad = decimal.Round(cantidadTemporal, 2),
                                    //comentario
                                    moneda_local_usd = j == 3 ? true : false
                                });
                            }

                        }

                        //aumenta variables
                        col += 4;
                        fecha = fecha.AddMonths(1);
                    }

                int comentarioIndex = headersForecast.IndexOf("Comments") - 1;

                //agrega el comentario general en caso de existir
                if (array[comentarioIndex] != null && !string.IsNullOrEmpty(array[comentarioIndex].ToString()))
                    comentarios_general.Add(new budget_rel_comentarios
                    {
                        id_budget_rel_fy_centro = id_rel_fy_cc.Value,
                        id_cuenta_sap = id_sap_account,
                        comentarios = array[comentarioIndex].ToString()
                    });

            }


            return resultado;
        }


        [NonAction]
        public static List<view_valores_forecast> AgregaCuentasSAPFaltantes(List<view_valores_forecast> listValores, int id_anio_fiscal, int id_centro_costo, bool soloCuentasActivas = false)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            List<budget_cuenta_sap> listCuentas = new List<budget_cuenta_sap>();

            budget_centro_costo centro = db.budget_centro_costo.Find(id_centro_costo);

            if (soloCuentasActivas) //carga sólo las cuentas activas
                listCuentas = db.budget_cuenta_sap.Where(x => x.activo == true).ToList();
            else //carga todas las cuentas
                listCuentas = db.budget_cuenta_sap.ToList();

            //Agrega cuentas vacias

            List<view_valores_forecast> listViewCuentas = new List<view_valores_forecast>();

            foreach (budget_cuenta_sap cuenta in listCuentas)
            {
                //agragega un objeto de tipo view_valores_anio_fiscal por cada cuenta existente
                listViewCuentas.Add(new view_valores_forecast
                {
                    id_anio_fiscal = id_anio_fiscal,
                    id_centro_costo = id_centro_costo,
                    id_cuenta_sap = cuenta.id,
                    sap_account = cuenta.sap_account,
                    name = cuenta.name,
                    mapping = cuenta.budget_mapping.descripcion,
                    mapping_bridge = cuenta.budget_mapping.budget_mapping_bridge.descripcion,
                    currency_iso = "USD",
                    class_1 = centro.class_1,
                    class_2 = centro.class_2,
                });
            }

            //obtiene las cuentas que no se encuentran en el listado original
            List<view_valores_forecast> listDiferencias = listViewCuentas.Except(listValores).ToList();

            //suba la lista original con la lista de excepciones
            listValores.AddRange(listDiferencias);


            return listValores.OrderBy(x => x.id_cuenta_sap).ToList();
        }

        [HttpPost]
        public JsonResult SaveConceptosCapturados(List<budget_rel_conceptos_cantidades_forecast> conceptos_form, int? id_rel)
        {

            if (conceptos_form == null)
                conceptos_form = new List<budget_rel_conceptos_cantidades_forecast>();

            var response = new object[1];
            response[0] = new { result = "OK", icon = "success", message = "Se guardaron los cambios correctamente." };

            //obtiene el listado actual de la BD
            List<budget_rel_conceptos_cantidades_forecast> lista_conceptos_BD = db.budget_rel_conceptos_cantidades_forecast.Where(x => x.budget_cantidad_forecast.id_budget_rel_fy_centro == id_rel).ToList();
            //obtiene listado de cantidades
            List<budget_cantidad_forecast> lista_cantidades_bd = db.budget_cantidad_forecast.Where(x => x.id_budget_rel_fy_centro == id_rel).ToList();


            foreach (var item in conceptos_form)
            {

                var itemBD = lista_conceptos_BD.FirstOrDefault(x => x.id_budget_cantidad_forecast == item.id_budget_cantidad_forecast && x.id_rel_conceptos == item.id_rel_conceptos);
                //EXISTE
                if (itemBD != null)
                {
                    //UPDATE
                    if (itemBD.cantidad != item.cantidad || itemBD.comentario != item.comentario)
                    {
                        itemBD.cantidad = item.cantidad;
                        itemBD.comentario = item.comentario;
                    }
                }
                else  //CREATE
                {
                    var cantidad = lista_cantidades_bd.FirstOrDefault(x =>
                                                x.budget_cuenta_sap.sap_account == item.cuenta_sap
                                                && x.mes == item.mes
                                                && x.currency_iso == item.currency
                                                && !x.moneda_local_usd
                                                );
                    if (cantidad != null)
                        db.budget_rel_conceptos_cantidades_forecast.Add(new budget_rel_conceptos_cantidades_forecast
                        {
                            id_budget_cantidad_forecast = cantidad.id,
                            id_rel_conceptos = item.id_rel_conceptos,
                            cantidad = item.cantidad,
                            comentario = item.comentario
                        });
                }
            }

            /* DELETE */

            try
            {
                db.SaveChanges();
                response[0] = new { result = "OK", icon = "success", message = "Se guardaron los cambios correctamente" };
            }
            catch (Exception e)
            {
                response[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }

            return Json(response, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Carga los datos iniciales del año indicado
        /// </summary>
        /// <param name="id_fy"></param>
        /// <returns></returns>
        public JsonResult CargaFormFormula(int? row, int? column, string cuenta_sap, int? id_bd_fy_centro, int? mes, string currency,
            bool datosPrevios, bool readOnly, string valor_actual, string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, string k, string m, string l,
             string c_a, string c_b, string c_c, string c_d, string c_e, string c_f, string c_g, string c_h, string c_i, string c_j, string c_k, string c_m, string c_l
            )
        {
            //en caso de readonly, deshabilita los datos previos
            if (readOnly)
                datosPrevios = false;

            var formulario = new object[1];

            var sapAccountList = db.budget_cuenta_sap;
            var sapAccount = sapAccountList.Where(x => x.sap_account == cuenta_sap).FirstOrDefault();
            var rel_fy_cc = db.budget_rel_fy_centro.Find(id_bd_fy_centro);
            var bgCantidad = db.budget_cantidad_forecast.Where(x => x.id_budget_rel_fy_centro == id_bd_fy_centro
                                    && x.budget_cuenta_sap.sap_account == cuenta_sap
                                    && x.mes == mes
                                    && x.currency_iso == currency
                                    && !x.moneda_local_usd
                                    ).FirstOrDefault();

            string id_cantidad = bgCantidad == null ? "0" : bgCantidad.id.ToString();

            string html = @" 
                <input type=""hidden"" name=""_readonly"" id=""_readonly"" value=""" + readOnly + @""">
                <input type=""hidden"" name=""_cuenta_sap"" id=""_cuenta_sap"" value=""" + cuenta_sap + @""">
                <input type=""hidden"" name=""_id_bd_fy_centro"" id=""_id_bd_fy_centro"" value=""" + id_bd_fy_centro + @""">
                <input type=""hidden"" name=""_mes"" id=""_mes"" value=""" + mes + @""">
                <input type=""hidden"" name=""_currency"" id=""_currency"" value=""" + currency + @""">
                <input type=""hidden"" name=""id_budget_cantidad"" id=""id_budget_cantidad"" value=""" + id_cantidad + @""">
                <input type=""hidden"" name=""formula"" id=""formula"" value=""" + sapAccount.formula + @""">
                <input type=""hidden"" name=""row_formula"" id=""row_formula"" value=""" + row + @""">
                <input type=""hidden"" name=""column_formula"" id=""column_formula"" value=""" + column + @""">";

            #region form gastos mantenimientos
            if (sapAccountList.Any(x => x.sap_account == cuenta_sap && x.aplica_gastos_mantenimiento))
            {
                double sugerido = 0;
                decimal? real = null;

                //obtiene el valor real
                if (decimal.TryParse(valor_actual, out decimal decimalResult))
                    real = decimalResult;

                //obtiene el valor sugerido
                //obtiene los ultimos tres gastos
                var listGastos = sapAccount.budget_conceptos_mantenimiento.Where(x => x.budget_rel_fy_centro.id_centro_costo == rel_fy_cc.id_centro_costo && x.mes == mes
                && x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio < rel_fy_cc.budget_anio_fiscal.anio_inicio && x.gasto != 0
                && x.moneda == currency
                ).OrderByDescending(x => x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio).Take(3).ToList();

                //realiaza la suma o toma el valor de 0
                sugerido = listGastos.Count == 0 ? 0 : listGastos.Sum(x => x.gasto - (x.one_time.HasValue ? x.one_time.Value : 0)) / listGastos.Count;

                foreach (var item in listGastos.OrderBy(x => x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio))
                {
                    html += @"
                        <div class=""form-group row"">
                            <label class=""control-label col-md-2 col-sm-2"" for=""anio_1"" style=""text-align:right"">FY " + (item.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio - 2000) + "/" + (item.budget_rel_fy_centro.budget_anio_fiscal.anio_fin - 2000) + @":</label>
                            <div class=""col-md-4"">
                                <input type=""text"" class=""form-control concepto-formula"" name=""anio_1"" id=""val_sugerido_{0}"" value=""" + (item.gasto) + @"""  readonly />
                            </div>
                            <label class=""control-label col-md-2 col-sm-2"" for=""anio_1"" style=""text-align:right"">One Time:</label>
                            <div class=""col-md-4"">
                                <input type=""text"" class=""form-control concepto-formula"" name=""anio_1"" id=""val_sugerido_{0}"" value=""" + (item.one_time) + @"""  readonly />
                            </div>
                        </div>";
                }



                html += String.Format(@" 
                <hr/>
                <div class=""form-group row"">
                    <label class=""control-label col-md-8 col-sm-8"" for=""val_sugerido_{0}"" style=""text-align:right"">Promedio normalizado sugerido:</label>
                    <div class=""col-md-4"">
                        <input type=""text"" class=""form-control concepto-formula"" name=""val_sugerido_{0}"" id=""val_sugerido_{0}"" value=""{1}""  readonly />
                    </div>
                </div>
                <div class=""form-group row"">
                    <label class=""control-label col-md-8 col-sm-8"" for=""val_{0}"" style=""text-align:right"">Mantto. mayor:</label>
                    <div class=""col-md-4"">
                        <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" value=""{2}""  {3}/>
                        <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                    </div>
                </div>", "result", String.Format("{0:0.##}", Math.Round(sugerido, 2)), String.Format("{0:0.##}", real), readOnly ? "readonly" : string.Empty);

                //envia form de gastos
                formulario[0] = new
                {
                    estatus = "OK",
                    html = html,
                    tieneComentarios = "false"
                };

                return Json(formulario, JsonRequestBehavior.AllowGet);
            }
            #endregion


            #region form html conceptos


            if (sapAccount.budget_rel_conceptos_formulas.Count == 0)
            {
                formulario[0] = new { estatus = "ERROR" };
                return Json(formulario, JsonRequestBehavior.AllowGet);
            }

            bool tieneComentarios = sapAccount.budget_rel_conceptos_formulas.Any(x => x.aplica_comentario);

            foreach (var concepto in sapAccount.budget_rel_conceptos_formulas)
            {
                //valor por defecto,  al cargar el formulario por primera vez
                string valor = currency == "MXN" ? concepto.valor_defecto_mxn.ToString() : currency == "USD" ? concepto.valor_defecto_usd.ToString() : currency == "EUR" ? concepto.valor_defecto_eur.ToString() : "0.0";

                string comentario = string.Empty;

                //valor formulario
                if (datosPrevios && (!concepto.valor_fijo.HasValue || !concepto.valor_fijo.Value))
                    switch (concepto.clave)
                    {
                        case "a":
                            valor = a;
                            comentario = c_a;
                            break;
                        case "b":
                            valor = b;
                            comentario = c_b;
                            break;
                        case "c":
                            valor = c;
                            comentario = c_c;
                            break;
                        case "d":
                            valor = d;
                            comentario = c_d;
                            break;
                        case "e":
                            valor = e;
                            comentario = c_e;
                            break;
                        case "f":
                            valor = f;
                            comentario = c_f;
                            break;
                        case "g":
                            valor = g;
                            comentario = c_g;
                            break;
                        case "h":
                            valor = h;
                            comentario = c_h;
                            break;
                        case "i":
                            valor = i;
                            comentario = c_i;
                            break;
                        case "j":
                            valor = j;
                            comentario = c_j;
                            break;
                        case "k":
                            valor = k;
                            comentario = c_k;
                            break;
                        case "l":
                            valor = l;
                            comentario = c_l;
                            break;
                        case "m":
                            valor = m;
                            comentario = c_m;
                            break;

                    }
                //si no toma el valor desde bd
                else if (bgCantidad != null && bgCantidad.budget_rel_conceptos_cantidades_forecast.Count > 0)
                {
                    valor = bgCantidad.budget_rel_conceptos_cantidades_forecast.FirstOrDefault(x => x.budget_rel_conceptos_formulas.clave == concepto.clave) != null ?
                        bgCantidad.budget_rel_conceptos_cantidades_forecast.FirstOrDefault(x => x.budget_rel_conceptos_formulas.clave == concepto.clave).cantidad.ToString() : "0";

                    comentario = bgCantidad.budget_rel_conceptos_cantidades_forecast.FirstOrDefault(x => x.budget_rel_conceptos_formulas.clave == concepto.clave) != null ?
                        bgCantidad.budget_rel_conceptos_cantidades_forecast.FirstOrDefault(x => x.budget_rel_conceptos_formulas.clave == concepto.clave).comentario : string.Empty;

                    //si no es read only toma el valor por defecto y no el de la BD
                    if (!readOnly && concepto.valor_fijo.HasValue && concepto.valor_fijo.Value)
                        valor = currency == "MXN" ? concepto.valor_defecto_mxn.ToString() : currency == "USD" ? concepto.valor_defecto_usd.ToString() : currency == "EUR" ? concepto.valor_defecto_eur.ToString() : "0.0";
                }

                //habilita o deshabilita conceptos segun cuenta sap
                bool readonlyConcepto = false;
                if ((cuenta_sap == "610030" || cuenta_sap == "610040") && (concepto.clave == "d" || concepto.clave == "e" || concepto.clave == "f") && currency == "MXN")
                    readonlyConcepto = true;
                if ((cuenta_sap == "610030" || cuenta_sap == "610040") && (concepto.clave == "a" || concepto.clave == "b" || concepto.clave == "c") && currency == "USD")
                    readonlyConcepto = true;
                if ((cuenta_sap == "652100") && (concepto.clave == "a" || concepto.clave == "b" || concepto.clave == "c" || concepto.clave == "d" || concepto.clave == "e" || concepto.clave == "f"
                    || concepto.clave == "g" || concepto.clave == "h" || concepto.clave == "k" || concepto.clave == "l"
                    ) && currency == "MXN")
                    readonlyConcepto = true;
                if ((cuenta_sap == "652100") && (concepto.clave == "i" || concepto.clave == "j") && currency == "USD")
                    readonlyConcepto = true;



                //determina si aplica comentario o no
                if (tieneComentarios)
                {
                    if (concepto.aplica_comentario)
                    {
                        if (cuenta_sap == "610091") // Si es festejos
                        {
                            //determina la letra del evento
                            string eventoClave = string.Empty;
                            switch (concepto.clave)
                            {
                                case "a":
                                case "b":
                                    eventoClave = "A";
                                    break;
                                case "c":
                                case "d":
                                    eventoClave = "B";
                                    break;
                                case "e":
                                case "f":
                                    eventoClave = "C";
                                    break;
                                case "g":
                                case "h":
                                    eventoClave = "Otro";
                                    break;
                            }

                            //determina si mostra un select o campo abierto
                            string select = string.Empty;
                            if (concepto.clave != "g") // g = clave para otro
                            {
                                select = string.Format(@" <select class=""form-control select2bs4"" name=""comentario_{0}"" id=""comentario_{0}"" style =""width:100%"" {1}>
                                                           <option value="""">-- Seleccione --</option>
                                                           <option value=""Rosca de reyes"" " + (comentario == "Rosca de reyes" ? "selected" : string.Empty) + @">Rosca de reyes</option>
                                                           <option value=""Día del amor y la amistad"" " + (comentario == "Día del amor y la amistad" ? "selected" : string.Empty) + @">Día del amor y la amistad</option>
                                                           <option value=""Día de la mujer"" " + (comentario == "Día de la mujer" ? "selected" : string.Empty) + @">Día de la mujer</option>
                                                           <option value=""Día de la familia""  " + (comentario == "Día de la familia" ? "selected" : string.Empty) + @">Día de la familia</option>
                                                           <option value=""Día de las madres""  " + (comentario == "Día de las madres" ? "selected" : string.Empty) + @">Día de las madres</option>
                                                           <option value=""Día del padre"" " + (comentario == "Día del padre" ? "selected" : string.Empty) + @">Día del padre</option>
                                                           <option value=""Semana de la salud""  " + (comentario == "Semana de la salud" ? "selected" : string.Empty) + @">Semana de la salud</option>
                                                           <option value=""Torneo de fútbol"" " + (comentario == "Torneo de fútbol" ? "selected" : string.Empty) + @">Torneo de fútbol</option>
                                                           <option value=""Kermés (día de la independencia)""  " + (comentario == "Kermés (día de la independencia)" ? "selected" : string.Empty) + @">Kermés (día de la independencia)</option>
                                                           <option value=""Día de muertos"" " + (comentario == "Día de muertos" ? "selected" : string.Empty) + @">Día de muertos</option>
                                                           <option value=""Fiesta fin de año"" " + (comentario == "Fiesta fin de año" ? "selected" : string.Empty) + @">Fiesta fin de año</option>
                                                         </select>", concepto.clave, readOnly ? "disabled" : string.Empty, comentario);
                            }
                            else
                            {//es otro 
                                select = string.Format(@"<input type=""text"" class=""form-control"" name=""comentario_{0}"" id=""comentario_{0}""  value=""{2}"" maxlength=""80"" {1}/>"
                                                        , concepto.clave, readOnly ? "disabled" : string.Empty, comentario);
                            }

                            //agrega los input correspondientes
                            html += String.Format(@"
                                                <input type=""hidden"" name=""id_rel_concepto_{0}"" id=""id_rel_concepto_{0}"" value=""" + concepto.id + @""">
                                                <input type=""hidden"" name=""concepto_clave_{0}"" id=""concepto_clave_{0}"" value=""" + concepto.clave + @""">
                                                <div class=""form-group row"" style=""{5}"">
                                                    <label class=""control-label col-md-4 col-sm-4"" for=""val_{0}"" style=""text-align:right"">{1}:</label>
                                                    <div class=""col-md-2"">
                                                        <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" {2} value=""{3}"" {4}/>
                                                        <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                                                    </div>
                                                    <label class=""control-label col-md-2 col-sm-2"" for=""comentario_{0}"" style=""text-align:right"">Evento {6}:</label>
                                                    <div class=""col-md-4"">
                                                         {7}             
                                                        <span class=""field-validation-valid text-danger"" data-valmsg-for=""comentario_{0}"" data-valmsg-replace=""true""></span>
                                                    </div>
                                                </div>                                                
                                            ", concepto.clave, concepto.descripcion, concepto.valor_fijo.HasValue && concepto.valor_fijo.Value ? "readonly" : string.Empty
                                                            , valor, readOnly ? "readonly" : string.Empty, readonlyConcepto ? "display:none" : string.Empty,
                                                            eventoClave, select
                                                            );
                        }
                        else  //si aplica comentario, pero no es festojos
                        {
                            html += String.Format(@"
                        <input type=""hidden"" name=""id_rel_concepto_{0}"" id=""id_rel_concepto_{0}"" value=""" + concepto.id + @""">
                        <input type=""hidden"" name=""concepto_clave_{0}"" id=""concepto_clave_{0}"" value=""" + concepto.clave + @""">
                        <div class=""form-group row"" style=""{5}"">
                            <label class=""control-label col-md-4 col-sm-4"" for=""val_{0}"" style=""text-align:right"">{1}:</label>
                            <div class=""col-md-2"">
                                <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" {2} value=""{3}"" {4}/>
                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                            </div>
                            <label class=""control-label col-md-1 col-sm-1"" for=""comentario_{0}"" style=""text-align:right"">Descrip.:</label>
                            <div class=""col-md-5"">
                                <input type=""text"" class=""form-control"" name=""comentario_{0}"" id=""comentario_{0}"" {2} value=""{6}"" maxlength=""80"" {4}/>
                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""comentario_{0}"" data-valmsg-replace=""true""></span>
                            </div>
                        </div>
                        ", concepto.clave, concepto.descripcion, concepto.valor_fijo.HasValue && concepto.valor_fijo.Value ? "readonly" : string.Empty
                                    , valor, readOnly ? "readonly" : string.Empty, readonlyConcepto ? "display:none" : string.Empty, comentario);
                        }
                    }
                    else  //este campo no tiene comentarios, pero si hay comentarios en otros campos
                    {
                        html += String.Format(@"
                        <input type=""hidden"" name=""id_rel_concepto_{0}"" id=""id_rel_concepto_{0}"" value=""" + concepto.id + @""">
                        <input type=""hidden"" name=""concepto_clave_{0}"" id=""concepto_clave_{0}"" value=""" + concepto.clave + @""">
                        <div class=""form-group row"" style=""{5}"">
                            <label class=""control-label col-md-4 col-sm-4"" for=""val_{0}"" style=""text-align:right"">{1}:</label>
                            <div class=""col-md-2"">
                                <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" {2} value=""{3}"" {4}/>
                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                            </div>                            
                        </div>
                         {6}"
                        , concepto.clave, concepto.descripcion, concepto.valor_fijo.HasValue && concepto.valor_fijo.Value ? "readonly" : string.Empty
                                   , valor, readOnly ? "readonly" : string.Empty, readonlyConcepto ? "display:none" : string.Empty
                                   , cuenta_sap == "610091" ? "<hr />" : string.Empty //si es el último campo de cada evento de festejos agrega una division
                                   );
                    }
                }
                else //no hay comentarios en todo el form
                {
                    html += String.Format(@"
                        <input type=""hidden"" name=""id_rel_concepto_{0}"" id=""id_rel_concepto_{0}"" value=""" + concepto.id + @""">
                        <input type=""hidden"" name=""concepto_clave_{0}"" id=""concepto_clave_{0}"" value=""" + concepto.clave + @""">
                        <div class=""form-group row"" style=""{5}"">
                            <label class=""control-label col-md-6 col-sm-6"" for=""val_{0}"" style=""text-align:right"">{1}:</label>
                            <div class=""col-md-6"">
                                <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" {2} value=""{3}"" {4}/>
                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                            </div>
                        </div>
                       ", concepto.clave, concepto.descripcion, concepto.valor_fijo.HasValue && concepto.valor_fijo.Value ? "readonly" : string.Empty
                            , valor, readOnly ? "readonly" : string.Empty, readonlyConcepto ? "display:none" : string.Empty
                            );
                }
            }

            html += String.Format(@" <div class=""form-group row"">
                    <label class=""control-label col-md-{2} col-sm-{2}"" for=""val_{0}"" style=""text-align:right"">{1}:</label>
                    <div class=""col-md-{3}"">
                        <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" readonly />
                        <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                    </div>
                </div>", "result", "Total", tieneComentarios ? 4 : 6, tieneComentarios ? 2 : 6);

            #endregion
            formulario[0] = new
            {
                estatus = "OK",
                html = html,
                tieneComentarios = tieneComentarios ? "true" : "false"
            };

            return Json(formulario, JsonRequestBehavior.AllowGet);
        }

        // GET: Bom/CargaBom/5
        public ActionResult CargaConcentrado()
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Dante/CargaConcentrado/5
        [HttpPost]
        public ActionResult CargaConcentrado(ExcelViewModel excelViewModel, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                string msjError = "No se ha podido leer el archivo seleccionado.";

                try
                {
                    HttpPostedFileBase stream = Request.Files["PostedFile"];

                    // Validación de tamaño y extensión
                    if (stream.InputStream.Length > 8388608)
                    {
                        msjError = "Sólo se permiten archivos con peso menor a 8 MB.";
                        throw new Exception(msjError);
                    }
                    else
                    {
                        string extension = Path.GetExtension(excelViewModel.PostedFile.FileName);
                        if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                        {
                            msjError = "Sólo se permiten archivos Excel.";
                            throw new Exception(msjError);
                        }
                    }

                    bool estructuraValida = false;
                    int noEncontrados = 0;
                    List<budget_rel_comentarios> lista_comentarios_form = new List<budget_rel_comentarios>();
                    List<int> idRels = new List<int>();

                    // Lee el archivo Excel y obtiene la lista de cantidades (incluye registros con moneda_local_usd = true)
                    List<budget_cantidad> lista_cantidad_formTemp = UtilExcel.BudgetLeeConcentrado(
                        stream, 4, ref estructuraValida, ref msjError, ref noEncontrados,
                        ref lista_comentarios_form, ref idRels);


                    //convierte de budget_cantidad a budget_Cantidad_forecast
                    List<budget_cantidad_forecast> lista_cantidad_form =
                        lista_cantidad_formTemp
                        .Select(bc => new budget_cantidad_forecast(bc))
                        .ToList();

                    // Obtiene de la BD sólo los registros correspondientes a los FY/CC enviados
                    List<budget_cantidad_forecast> lista_cantidad_BD = db.budget_cantidad_forecast
                        .Where(x => idRels.Contains(x.id_budget_rel_fy_centro))
                        .ToList();


                    // Contadores para las operaciones de budget_cantidad
                    int addedCount = 0, updatedCount = 0, deletedCount = 0;

                    if (!estructuraValida)
                    {
                        msjError = "No cumple con la estructura válida. " + msjError;
                        throw new Exception(msjError);
                    }
                    else if (lista_cantidad_form.Count > 0)
                    {
                        // Procesa cada registro enviado: UPDATE o CREATE
                        foreach (var nuevo in lista_cantidad_form)
                        {
                            // La clave compuesta:
                            // (id_cuenta_sap, id_budget_rel_fy_centro, mes, currency_iso, moneda_local_usd)
                            var regBD = lista_cantidad_BD.FirstOrDefault(x =>
                                x.id_cuenta_sap == nuevo.id_cuenta_sap &&
                                x.id_budget_rel_fy_centro == nuevo.id_budget_rel_fy_centro &&
                                x.mes == nuevo.mes &&
                                x.currency_iso == nuevo.currency_iso &&
                                x.moneda_local_usd == nuevo.moneda_local_usd);

                            if (regBD != null)
                            {
                                // UPDATE: si la cantidad es distinta
                                if (regBD.cantidad != nuevo.cantidad)
                                {
                                    regBD.cantidad = nuevo.cantidad;
                                    updatedCount++;
                                }
                            }
                            else  // CREATE
                            {
                                db.budget_cantidad_forecast.Add(nuevo);
                                addedCount++;
                            }
                        }

                        // DELETE: eliminar aquellos registros en BD que no estén en la nueva plantilla
                        var toDelete = lista_cantidad_BD.Where(x =>
                            !lista_cantidad_form.Any(y =>
                                y.id_cuenta_sap == x.id_cuenta_sap &&
                                y.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro &&
                                y.mes == x.mes &&
                                y.currency_iso == x.currency_iso &&
                                y.moneda_local_usd == x.moneda_local_usd
                            )).ToList();

                        // Si un registro tiene dependencias, se eliminan primero esas relaciones.
                        foreach (var item in toDelete.Where(x => x.budget_rel_conceptos_cantidades_forecast.Any()))
                            db.budget_rel_conceptos_cantidades_forecast.RemoveRange(item.budget_rel_conceptos_cantidades_forecast);

                        db.budget_cantidad_forecast.RemoveRange(toDelete);
                        deletedCount = toDelete.Count;


                        try
                        {
                            db.SaveChanges();

                            // Construir un mensaje resumen
                            string mensaje = $"Se han actualizado {updatedCount} registros, agregado {addedCount} y eliminado {deletedCount} en cantidades.";
                            //mensaje += $" Además, se han actualizado {updatedComments} comentarios, agregado {addedComments} y eliminado {deletedComments}.";

                            if (noEncontrados > 0)
                                mensaje += $" Algunas cuentas SAP no se encontraron: {noEncontrados}.";

                            TempData["Mensaje"] = new MensajesSweetAlert(mensaje, TipoMensajesSweetAlerts.INFO);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Error: " + ex.Message);

                            return View(excelViewModel);
                        }

                        return RedirectToAction("CargaConcentrado");
                    }
                }
                catch (Exception e)
                {

                    ModelState.AddModelError("", msjError + ": " + e.Message);
                    return View(excelViewModel);
                }
            }

            return View(excelViewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
