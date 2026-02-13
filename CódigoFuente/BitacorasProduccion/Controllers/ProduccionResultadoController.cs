using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using DocumentFormat.OpenXml.Spreadsheet;
using Portal_2_0.Models;
using SpreadsheetLight;

namespace Portal_2_0.Controllers
{
    [Microsoft.AspNet.SignalR.Authorize]
    public class ProduccionResultadoController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();


        // GET: ProduccionReportes
        public ActionResult Index(int? clave_planta, int? id_linea, string platina, string numero_parte, string fecha_inicial, string fecha_final, string tipo_reporte, string fecha_turno, int? turno, int pagina = 1)
        {

            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                empleados emp = obtieneEmpleadoLogeado();

                ////muestra error en caso de querer solicitar una planta distinta
                if (!TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE_ALL_ACCESS) && clave_planta != null && clave_planta != emp.planta_clave)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede acceder a la información solicitada!";
                    ViewBag.Descripcion = "No puede consultar la información de la planta solicitada.";

                    return View("../Home/ErrorGenerico");
                }


                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
                DateTime dateFinal = DateTime.Now;          //fecha final por defecto
                DateTime dateTurno = DateTime.Now;          //fecha turno por defecto

                try
                {
                    if (!String.IsNullOrEmpty(fecha_inicial))
                        dateInicial = Convert.ToDateTime(fecha_inicial);
                    if (!String.IsNullOrEmpty(fecha_final))
                    {
                        dateFinal = Convert.ToDateTime(fecha_final);
                        dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
                    if (!String.IsNullOrEmpty(fecha_turno))
                        dateTurno = Convert.ToDateTime(fecha_turno);
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Error de Formato: " + e.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al convertir: " + ex.Message);
                }

                //valor por defecto
                int clave = 0;

                if (clave_planta != null)
                {
                    clave = clave_planta.Value;
                }
                plantas planta = db.plantas.Find(clave);
                if (planta == null)
                {
                    planta = new plantas
                    {
                        clave = 0
                    };
                }


                //valor por defecto linea
                int linea = 0;

                if (id_linea != null)
                {
                    linea = id_linea.Value;
                }

                produccion_lineas produccion_Lineas = db.produccion_lineas.Find(linea);
                if (produccion_Lineas == null)
                {
                    produccion_Lineas = new produccion_lineas
                    {
                        id = 0,
                    };
                }

                //valor por defecto para turno
                int id_turno = 0;

                if (turno != null)
                    id_turno = turno.Value;

                produccion_turnos turno1 = db.produccion_turnos.Find(id_turno);

                //si no hay turno lo inicializa
                if (turno1 == null)
                    turno1 = new produccion_turnos { id = 0 };


                var cantidadRegistrosPorPagina = 20; // parámetro

                //REALIZA LA CONSULTA DEPENDIENDO DEL TIPO
                String tipoR = "sabana";  //valor por defecto 
                if (!String.IsNullOrEmpty(tipo_reporte))
                {
                    tipoR = tipo_reporte;
                }

                List<view_historico_resultado> listado = new List<view_historico_resultado>();
                int totalDeRegistros = 0;

                if (clave_planta == null)
                {
                    listado = new List<view_historico_resultado> { };
                    totalDeRegistros = 0;
                }
                else
                {
                    //aumenta el tiempo de espera
                    db.Database.CommandTimeout = 240;
                    if (tipoR.Contains("sabana"))
                    {             //BUSCA POR SÁBANA
                        var listadoBD = db.view_historico_resultado.Where(
                            x =>
                            x.Planta.ToUpper().Contains(planta.descripcion.ToUpper())
                            && (x.Linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                            && x.Fecha >= dateInicial && x.Fecha <= dateFinal
                            && (x.Número_de_Parte__de_cliente == numero_parte || x.Número_de_Parte_de_Cliente_platina2 == numero_parte || String.IsNullOrEmpty(numero_parte))
                            && (x.SAP_Platina == platina || x.SAP_Platina_2 == platina || String.IsNullOrEmpty(platina))
                            // && !x.SAP_Platina.ToUpper().Contains("TEMPORAL")
                            // && !x.SAP_Rollo.ToUpper().Contains("TEMPORAL")
                            );

                        listado = listadoBD
                            .OrderBy(x => x.id)
                            .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                            .Take(cantidadRegistrosPorPagina).ToList();

                        totalDeRegistros = listadoBD.Count();
                    }
                    else if (tipoR.Contains("turno"))
                    { //BUSCA POR TURNO
                      //determina la hora inicial y final del turno                   
                        DateTime fecha_fin_turno = dateTurno.Add(turno1.hora_fin);
                        //hora inicial
                        dateTurno = dateTurno.Add(turno1.hora_inicio);

                        //si la hora fin es menor a la hora inicio es otro dia
                        if (TimeSpan.Compare(turno1.hora_inicio, turno1.hora_fin) == 1)
                            fecha_fin_turno = fecha_fin_turno.AddDays(1);

                        var listadoBD = db.view_historico_resultado.Where(
                            x =>
                             x.Planta.ToUpper().Contains(planta.descripcion.ToUpper())
                           && (x.Linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                           && (x.Turno.ToUpper().Contains(turno1.descripcion.ToUpper()) || x.Turno.ToUpper().Contains(turno1.valor.ToString()) || id_turno == 0)
                           && x.Fecha >= dateTurno && x.Fecha <= fecha_fin_turno
                             && (x.Número_de_Parte__de_cliente == numero_parte || x.Número_de_Parte_de_Cliente_platina2 == numero_parte || String.IsNullOrEmpty(numero_parte))
                            && (x.SAP_Platina == platina || x.SAP_Platina_2 == platina || String.IsNullOrEmpty(platina))

                            // && !x.SAP_Platina.ToUpper().Contains("TEMPORAL")
                            // && !x.SAP_Rollo.ToUpper().Contains("TEMPORAL")
                            );


                        listado = listadoBD
                           .OrderBy(x => x.id)
                           .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                           .Take(cantidadRegistrosPorPagina).ToList();

                        totalDeRegistros = listadoBD.Count();

                    }
                }

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["clave_planta"] = clave_planta;
                routeValues["id_linea"] = id_linea;
                routeValues["platina"] = platina;
                routeValues["numero_parte"] = numero_parte;
                routeValues["fecha_inicial"] = fecha_inicial;
                routeValues["fecha_final"] = fecha_final;
                routeValues["tipo_reporte"] = tipo_reporte;
                routeValues["fecha_turno"] = fecha_turno;
                routeValues["turno"] = turno;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                //crea el select List para todos los numeros de parte
                List<string> listaNumeroParte = new List<string>();
                List<string> listaNumeroPartePlatina2 = new List<string>();
                //listas para num platina
                List<string> listaPlatina1 = new List<string>();
                List<string> listaPlatina2 = new List<string>();


                if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE_ALL_ACCESS))
                {
                    listaNumeroParte = db.view_historico_resultado.Select(x => x.Número_de_Parte__de_cliente).Distinct().ToList();
                    listaPlatina1 = db.view_historico_resultado.Select(x => x.SAP_Platina).Distinct().ToList();
                }
                else
                {
                    listaNumeroParte = db.view_historico_resultado.Where(x => x.Planta.ToUpper().Contains(emp.plantas.descripcion.ToUpper())).Select(x => x.Número_de_Parte__de_cliente).Distinct().ToList();
                    listaPlatina1 = db.view_historico_resultado.Where(x => x.Planta.ToUpper().Contains(emp.plantas.descripcion.ToUpper())).Select(x => x.SAP_Platina).Distinct().ToList();
                }
                if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE_ALL_ACCESS))
                {
                    listaNumeroPartePlatina2 = db.view_historico_resultado.Select(x => x.Número_de_Parte_de_Cliente_platina2).Distinct().ToList();
                    listaPlatina2 = db.view_historico_resultado.Select(x => x.SAP_Platina_2).Distinct().ToList();
                }
                else
                {
                    listaNumeroPartePlatina2 = db.view_historico_resultado.Where(x => x.Planta.ToUpper().Contains(emp.plantas.descripcion.ToUpper())).Select(x => x.Número_de_Parte_de_Cliente_platina2).Distinct().ToList();
                    listaPlatina2 = db.view_historico_resultado.Where(x => x.Planta.ToUpper().Contains(emp.plantas.descripcion.ToUpper())).Select(x => x.SAP_Platina_2).Distinct().ToList();
                }
                //une con numero de parte de platina 2

                //Une ambas listas
                listaNumeroParte.AddRange(listaNumeroPartePlatina2);
                //quita elementos vacios
                listaNumeroParte.RemoveAll(x => string.IsNullOrEmpty(x));
                //elimina duplicados
                listaNumeroParte = listaNumeroParte.Distinct().ToList();

                //une ambas listas y elimina duplicados
                listaPlatina1.AddRange(listaPlatina2);
                //quita elementos vacios
                listaPlatina1.RemoveAll(x => string.IsNullOrEmpty(x));
                //elimina duplicados
                listaPlatina1 = listaPlatina1.Distinct().ToList();



                //creamos una lista tipo SelectListItem
                List<SelectListItem> lst = new List<SelectListItem>();

                foreach (var item in listaNumeroParte)
                    lst.Add(new SelectListItem() { Text = item, Value = item });

                //Agregamos la lista a nuestro SelectList
                SelectList listNumParte = new SelectList(lst, "Value", "Text");



                List<SelectListItem> lstP = new List<SelectListItem>();
                foreach (var item in listaPlatina1)
                    lstP.Add(new SelectListItem() { Text = item, Value = item });

                SelectList listPlatina = new SelectList(lstP, "Value", "Text");


                ViewBag.id_linea = new SelectList(db.produccion_lineas.Where(p => p.activo == true), "id", "linea");
                ViewBag.turno = new SelectList(db.produccion_turnos.Where(p => p.activo.HasValue && p.activo.Value && p.clave_planta == emp.planta_clave), "id", "descripcion");
                ViewBag.numero_parte = listNumParte;
                ViewBag.platina = listPlatina;

                if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE_ALL_ACCESS))
                    ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                else
                    ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true && p.clave == emp.planta_clave), "clave", "descripcion");
                ViewBag.Paginacion = paginacion;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }


        public ActionResult Exportar(int? clave_planta, int? id_linea, string platina, string numero_parte, string fecha_inicial, string fecha_final, string tipo_reporte, string fecha_turno, int? turno, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE))
            {
                return View("../Home/ErrorPermisos");
            }

            // Configuración inicial
            db.Database.CommandTimeout = 300;

            // Parse de fechas con valores por defecto
            DateTime dateInicial = ParseDateOrDefault(fecha_inicial, new DateTime(2000, 1, 1));
            DateTime dateFinal = ParseDateOrDefault(fecha_final, DateTime.Now).AddHours(23).AddMinutes(59).AddSeconds(59);
            DateTime dateTurno = ParseDateOrDefault(fecha_turno, DateTime.Now);

            // Obtener datos de planta, línea y turno
            plantas planta = GetEntityOrDefault(db.plantas, clave_planta, new plantas { clave = 0 });
            produccion_lineas produccion_Lineas = GetEntityOrDefault(db.produccion_lineas, id_linea, new produccion_lineas { id = 0 });
            produccion_turnos turno1 = GetEntityOrDefault(db.produccion_turnos, turno, new produccion_turnos { id = 0 });

            // Determinar tipo de reporte
            string tipoR = !string.IsNullOrEmpty(tipo_reporte) ? tipo_reporte : "sabana";
            bool porturno = tipoR.Contains("turno");
            List<view_historico_resultado> listado = GetListadoHistorico(db.view_historico_resultado, tipoR, planta, produccion_Lineas, turno1, dateInicial, dateFinal, dateTurno, platina, numero_parte, porturno);

            // Generar archivo Excel
            byte[] stream = ExcelUtil.GeneraReporteBitacorasExcel(listado, planta, porturno);
            string claveDoc = planta.clave == 2 ? "PRF005-04" : "PRF014-04";

            // Configurar respuesta
            string fileName = Server.UrlEncode($"{claveDoc}_Bitácora_de_Producción_{planta.descripcion}.xlsx");
            Response.AppendHeader("Content-Disposition", new System.Net.Mime.ContentDisposition { FileName = fileName, Inline = false }.ToString());
            return File(stream, "application/vnd.ms-excel");
        }

        // Métodos auxiliares

        private static DateTime ParseDateOrDefault(string dateStr, DateTime defaultValue)
        {
            return DateTime.TryParse(dateStr, out var parsedDate) ? parsedDate : defaultValue;
        }

        private static T GetEntityOrDefault<T>(DbSet<T> dbSet, int? id, T defaultEntity) where T : class
        {
            return id.HasValue ? dbSet.Find(id) ?? defaultEntity : defaultEntity;
        }

        private static List<view_historico_resultado> GetListadoHistorico(DbSet<view_historico_resultado> viewHistorico, string tipoR, plantas planta, produccion_lineas produccionLineas, produccion_turnos turno, DateTime dateInicial, DateTime dateFinal, DateTime dateTurno, string platina, string numeroParte, bool porturno)
        {
            IQueryable<view_historico_resultado> query = viewHistorico.AsQueryable();

            // Calcular las fechas finales fuera del query
            DateTime fechaFinTurno = dateTurno;
            if (porturno)
            {
                fechaFinTurno = dateTurno.Add(turno.hora_fin);
                dateTurno = dateTurno.Add(turno.hora_inicio);

                // Si la hora fin es menor que la hora inicio, ajustar al siguiente día
                if (TimeSpan.Compare(turno.hora_inicio, turno.hora_fin) > 0)
                {
                    fechaFinTurno = fechaFinTurno.AddDays(1);
                }
            }

            // Filtrado base
            query = query.Where(x => x.Fecha >= (porturno ? dateTurno : dateInicial) && x.Fecha <= (porturno ? fechaFinTurno : dateFinal));

            // Condición por planta
            if (!string.IsNullOrEmpty(planta.descripcion))
                query = query.Where(x => x.Planta.Contains(planta.descripcion));

            // Condición por línea
            if (!string.IsNullOrEmpty(produccionLineas.linea) && produccionLineas.id != 0)
                query = query.Where(x => x.Linea.Contains(produccionLineas.linea));

            // Condición por número de parte
            if (!string.IsNullOrEmpty(numeroParte))
                query = query.Where(x => x.Número_de_Parte__de_cliente == numeroParte || x.Número_de_Parte_de_Cliente_platina2 == numeroParte);

            // Condición por platina
            if (!string.IsNullOrEmpty(platina))
                query = query.Where(x => x.SAP_Platina == platina || x.SAP_Platina_2 == platina);

            // Condición por turno (si aplica)
            if (porturno)
            {
                query = query.Where(x =>
                    x.Turno.Contains(turno.descripcion) ||
                    x.Turno.Contains(turno.valor.ToString()) ||
                    turno.id == 0);
            }

            return query.OrderBy(x => x.id).ToList();
        }

        #region REPORTE SCRAP

        // ==============================================================================================
        // INICIO: MÓDULO REPORTE DE BALANCE DE MATERIALES (SCRAP)
        // ==============================================================================================

        // 1. Método para cargar la vista base (El esqueleto de la pantalla)
        [HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ReporteBalanceScrap()
        {
            // Nota: Descomenta la seguridad cuando pases a producción
            // if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE) || TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE_ALL_ACCESS) || TieneRol(TipoRoles.ADMIN))
            // {
            return View();
            // }
            // else
            // {
            //     return View("../Home/ErrorPermisos");
            // }
        }

        // 2. Método AJAX corregido para paginación REAL en Base de Datos
        [HttpPost]
        public ActionResult GetReporteScrapData(string fechaInicio, string fechaFin)
        {
            try
            {
                // Parámetros de DataTables
                var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
                var start = Request.Form.GetValues("start")?.FirstOrDefault();
                var length = Request.Form.GetValues("length")?.FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]")?.FirstOrDefault();
                var sortColumnIndex = Request.Form.GetValues("order[0][column]")?.FirstOrDefault();
                var sortDirection = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                using (var db_sap = new Portal_2_0_ServicesEntities())
                {
                    // CAMBIO CLAVE: Usamos la VISTA, no el SP.
                    // Al usar la vista como Queryable, el filtrado y paginado se hace en SQL Server, no en RAM.
                    var query = db_sap.vw_ReporteBalanceMateriales.AsNoTracking().AsQueryable();

                    // 1. FILTRO DE FECHAS (Se traduce a WHERE en SQL)
                    DateTime fInicio = DateTime.Now.AddDays(-30);
                    DateTime fFin = DateTime.Now.AddHours(23).AddMinutes(59);

                    if (!string.IsNullOrEmpty(fechaInicio)) DateTime.TryParse(fechaInicio, out fInicio);
                    if (!string.IsNullOrEmpty(fechaFin))
                    {
                        DateTime.TryParse(fechaFin, out fFin);
                        fFin = fFin.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    query = query.Where(x => x.Fecha_Produccion >= fInicio && x.Fecha_Produccion <= fFin);

                    // 2. FILTRO DE TEXTO (Se traduce a LIKE en SQL)
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        searchValue = searchValue.ToLower();
                        query = query.Where(x =>
                            (x.Numero_Parte != null && x.Numero_Parte.ToLower().Contains(searchValue)) ||
                            (x.Business_Model != null && x.Business_Model.ToLower().Contains(searchValue)) ||
                            (x.Tipo_Metal != null && x.Tipo_Metal.ToLower().Contains(searchValue)) ||
                            (x.Orden_Produccion != null && x.Orden_Produccion.ToString().Contains(searchValue)) ||
                            (x.Planta != null && x.Planta.ToString().Contains(searchValue))
                        );
                    }

                    // Contamos el total filtrado directamente en SQL
                    recordsTotal = query.Count();

                    // 3. ORDENAMIENTO (Se traduce a ORDER BY en SQL)
                    if (!string.IsNullOrEmpty(sortColumnIndex) && !string.IsNullOrEmpty(sortDirection))
                    {
                        bool isDesc = sortDirection == "desc";
                        switch (sortColumnIndex)
                        {
                            case "0": query = isDesc ? query.OrderByDescending(x => x.Orden_Produccion) : query.OrderBy(x => x.Orden_Produccion); break;
                            case "1": query = isDesc ? query.OrderByDescending(x => x.Planta) : query.OrderBy(x => x.Planta); break;
                            case "2": query = isDesc ? query.OrderByDescending(x => x.Numero_Parte) : query.OrderBy(x => x.Numero_Parte); break;
                            case "3": query = isDesc ? query.OrderByDescending(x => x.Business_Model) : query.OrderBy(x => x.Business_Model); break;
                            case "4": query = isDesc ? query.OrderByDescending(x => x.Tipo_Metal) : query.OrderBy(x => x.Tipo_Metal); break;
                            case "5": query = isDesc ? query.OrderByDescending(x => x.Rollos_Usados) : query.OrderBy(x => x.Rollos_Usados); break;
                            case "6": query = isDesc ? query.OrderByDescending(x => x.Tons_Usadas) : query.OrderBy(x => x.Tons_Usadas); break;
                            case "7": query = isDesc ? query.OrderByDescending(x => x.Total_Kg_Used) : query.OrderBy(x => x.Total_Kg_Used); break;
                            case "8": query = isDesc ? query.OrderByDescending(x => x.Peso_Platina_KG) : query.OrderBy(x => x.Peso_Platina_KG); break;
                            case "9": query = isDesc ? query.OrderByDescending(x => x.Platinas_Producidas) : query.OrderBy(x => x.Platinas_Producidas); break;
                            case "10": query = isDesc ? query.OrderByDescending(x => x.Platinas_Vendidas) : query.OrderBy(x => x.Platinas_Vendidas); break;
                            case "11": query = isDesc ? query.OrderByDescending(x => x.Diferencia_Inventario_Teorica) : query.OrderBy(x => x.Diferencia_Inventario_Teorica); break;
                            case "12": query = isDesc ? query.OrderByDescending(x => x.Merma_Almacen) : query.OrderBy(x => x.Merma_Almacen); break;
                            case "13": query = isDesc ? query.OrderByDescending(x => x.Scrap_OffFall_KG) : query.OrderBy(x => x.Scrap_OffFall_KG); break;
                            case "14": query = isDesc ? query.OrderByDescending(x => x.Scrap_PC_KG) : query.OrderBy(x => x.Scrap_PC_KG); break;
                            case "15": query = isDesc ? query.OrderByDescending(x => x.Scrap_Merma_KG) : query.OrderBy(x => x.Scrap_Merma_KG); break;
                            case "16": query = isDesc ? query.OrderByDescending(x => x.Porcentaje_Scrap_OffFall) : query.OrderBy(x => x.Porcentaje_Scrap_OffFall); break;
                            case "17": query = isDesc ? query.OrderByDescending(x => x.Porcentaje_Scrap_PC) : query.OrderBy(x => x.Porcentaje_Scrap_PC); break;
                            case "18": query = isDesc ? query.OrderByDescending(x => x.Porcentaje_Scrap_Merma) : query.OrderBy(x => x.Porcentaje_Scrap_Merma); break;
                            default: query = query.OrderByDescending(x => x.Orden_Produccion); break;
                        }
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.Orden_Produccion);
                    }

                    // 4. PAGINACIÓN (Se traduce a OFFSET / FETCH en SQL)
                    // Aquí ocurre la magia: Solo viajan por la red 50 registros, no 35,000
                    var data = query.Skip(skip).Take(pageSize).ToList();

                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                EscribeExcepcion(ex, Clases.Models.EntradaRegistroEvento.TipoEntradaRegistroEvento.Error);
                return Json(new { error = "Ocurrió un error: " + ex.Message });
            }
        }

        // 3. Método de Exportación corregido para usar el SP
        [HttpPost]
        public ActionResult ExportarBalanceScrapExcel(string searchTerm, string fechaInicio, string fechaFin)
        {
            try
            {
                using (var db_sap = new Portal_2_0_ServicesEntities())
                {
                    // 1. Usamos la VISTA con AsNoTracking para máxima velocidad de lectura
                    var query = db_sap.vw_ReporteBalanceMateriales.AsNoTracking().AsQueryable();

                    // 2. Filtro de Fechas
                    DateTime fInicio = DateTime.Now.AddDays(-30);
                    DateTime fFin = DateTime.Now.AddHours(23).AddMinutes(59);

                    if (!string.IsNullOrEmpty(fechaInicio)) DateTime.TryParse(fechaInicio, out fInicio);
                    if (!string.IsNullOrEmpty(fechaFin))
                    {
                        DateTime.TryParse(fechaFin, out fFin);
                        fFin = fFin.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    query = query.Where(x => x.Fecha_Produccion >= fInicio && x.Fecha_Produccion <= fFin);

                    // 3. Filtro de Texto (Mismo que en la tabla)
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        searchTerm = searchTerm.ToLower();
                        query = query.Where(x =>
                            (x.Numero_Parte != null && x.Numero_Parte.ToLower().Contains(searchTerm)) ||
                            (x.Business_Model != null && x.Business_Model.ToLower().Contains(searchTerm)) ||
                            (x.Tipo_Metal != null && x.Tipo_Metal.ToLower().Contains(searchTerm)) ||
                            (x.Orden_Produccion != null && x.Orden_Produccion.ToString().Contains(searchTerm)) ||
                            (x.Planta != null && x.Planta.ToString().Contains(searchTerm))
                        );
                    }

                    // 4. Proyección Ligera (Select)
                    // Traemos solo los datos necesarios para el Excel, evitando cargar todo el objeto de EF
                    var datosExportar = query
                        .OrderByDescending(x => x.Orden_Produccion)
                        .Select(item => new
                        {
                            item.Orden_Produccion,
                            item.Planta,
                            item.Numero_Parte,
                            item.Business_Model,
                            item.Tipo_Metal,
                            item.Rollos_Usados,
                            item.Tons_Usadas,
                            item.Total_Kg_Used,
                            item.Peso_Platina_KG,
                            item.Platinas_Producidas,
                            item.Platinas_Vendidas,
                            item.Diferencia_Inventario_Teorica,
                            item.Merma_Almacen,
                            item.Scrap_OffFall_KG,
                            item.Scrap_PC_KG,
                            item.Scrap_Merma_KG,
                            item.Porcentaje_Scrap_OffFall,
                            item.Porcentaje_Scrap_PC,
                            item.Porcentaje_Scrap_Merma
                        })
                        .ToList(); // Ejecuta la consulta optimizada

                    // ==========================================
                    // IMPLEMENTACIÓN SPREADSHEETLIGHT
                    // ==========================================
                    SLDocument oSLDocument = new SLDocument();
                    System.Data.DataTable dt = new System.Data.DataTable();

                    // Pre-asignar capacidad mejora el rendimiento
                    dt.MinimumCapacity = datosExportar.Count;

                    dt.Columns.Add("Orden Producción", typeof(string));
                    dt.Columns.Add("Planta", typeof(string));
                    dt.Columns.Add("Número Parte", typeof(string));
                    dt.Columns.Add("Business Model", typeof(string));
                    dt.Columns.Add("Tipo Metal", typeof(string));
                    dt.Columns.Add("Rollos Usados", typeof(double));
                    dt.Columns.Add("Tons Usadas", typeof(double));
                    dt.Columns.Add("Total KG Usados", typeof(double));
                    dt.Columns.Add("Peso Platina (KG)", typeof(double));
                    dt.Columns.Add("Platinas Producidas", typeof(double));
                    dt.Columns.Add("Platinas Vendidas", typeof(double));
                    dt.Columns.Add("Dif. Inventario Teórica", typeof(double));
                    dt.Columns.Add("Merma Almacén", typeof(double));
                    dt.Columns.Add("Scrap OffFall (KG)", typeof(double));
                    dt.Columns.Add("Scrap P&C (KG)", typeof(double));
                    dt.Columns.Add("Scrap Merma (KG)", typeof(double));
                    dt.Columns.Add("Scrap (Off-all)%", typeof(double));
                    dt.Columns.Add("Scrap (P&C)%", typeof(double));
                    dt.Columns.Add("Scrap (Merma)%", typeof(double));

                    // Llenado de filas
                    foreach (var item in datosExportar)
                    {
                        dt.Rows.Add(
                            item.Orden_Produccion?.ToString(),
                            item.Planta?.ToString(),
                            item.Numero_Parte,
                            item.Business_Model ?? "",
                            item.Tipo_Metal ?? "",
                            item.Rollos_Usados ?? 0,
                            item.Tons_Usadas.HasValue ? Math.Round(item.Tons_Usadas.Value, 3) : 0,
                            item.Total_Kg_Used.HasValue ? Math.Round(item.Total_Kg_Used.Value, 3) : 0,
                            item.Peso_Platina_KG.HasValue ? Math.Round(item.Peso_Platina_KG.Value, 3) : 0,
                            item.Platinas_Producidas ?? 0,
                            item.Platinas_Vendidas ?? 0,
                            item.Diferencia_Inventario_Teorica ?? 0,
                            item.Merma_Almacen ?? 0,
                            item.Scrap_OffFall_KG.HasValue ? Math.Round(item.Scrap_OffFall_KG.Value, 2) : 0,
                            item.Scrap_PC_KG.HasValue ? Math.Round(item.Scrap_PC_KG.Value, 2) : 0,
                            item.Scrap_Merma_KG.HasValue ? Math.Round(item.Scrap_Merma_KG.Value, 2) : 0,
                            item.Porcentaje_Scrap_OffFall.HasValue ? Math.Round(item.Porcentaje_Scrap_OffFall.Value, 2) : 0,
                            item.Porcentaje_Scrap_PC.HasValue ? Math.Round(item.Porcentaje_Scrap_PC.Value, 2) : 0,
                            item.Porcentaje_Scrap_Merma.HasValue ? Math.Round(item.Porcentaje_Scrap_Merma.Value, 2) : 0
                        );
                    }

                    oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Balance Materiales");
                    oSLDocument.ImportDataTable(1, 1, dt, true);

                    // Estilos
                    SLStyle styleHeader = oSLDocument.CreateStyle();
                    styleHeader.Font.FontName = "Calibri";
                    styleHeader.Font.FontSize = 11;
                    styleHeader.Font.FontColor = System.Drawing.Color.White;
                    styleHeader.Font.Bold = true;
                    styleHeader.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    styleHeader.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#009ff5"), System.Drawing.ColorTranslator.FromHtml("#009ff5"));

                    oSLDocument.SetRowStyle(1, styleHeader);
                    oSLDocument.FreezePanes(1, 0);
                    oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
                    oSLDocument.AutoFitColumn(1, dt.Columns.Count);

                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                    {
                        oSLDocument.SaveAs(stream);
                        byte[] array = stream.ToArray();
                        return File(array, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Balance_Materiales_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                EscribeExcepcion(ex, Clases.Models.EntradaRegistroEvento.TipoEntradaRegistroEvento.Error);
                TempData["Mensaje"] = new MensajesSweetAlert("Error al generar el Excel: " + ex.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("ReporteBalanceScrap");
            }
        }
        // ==============================================================================================
        // FIN: MÓDULO REPORTE DE BALANCE DE MATERIALES (SCRAP)
        // ==============================================================================================

        #endregion


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
