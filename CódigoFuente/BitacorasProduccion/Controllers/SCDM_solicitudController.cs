using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Windows.Media.Media3D;
using Bitacoras.Util;
using Clases.Util;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Org.BouncyCastle.Math;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class SCDM_solicitudController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: SCDM_solicitud
        public ActionResult Index(string estatus, int? id_solicitud, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) && !TieneRol(TipoRoles.SCDM_MM_APROBACION_SOLICITUDES) &&
                !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES) && !TieneRol(TipoRoles.SCDM_MM_REPORTES))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();
            var listTotal = db.SCDM_solicitud.Where(x => x.id_solicitante == empleado.id
                            && (id_solicitud == null || (id_solicitud.HasValue && x.id == id_solicitud))
                            ).ToList();

            //listado de creadas sin enviar
            var listCreadas = listTotal.Where(x => x.EstatusSolicitud == SCMD_solicitud_estatus_enum.CREADO).ToList();
            //listado en progreso                    
            var listEnProceso = listTotal.Where(x => x.EstatusSolicitud == SCMD_solicitud_estatus_enum.EN_REVISION_INICIAL
                        || x.EstatusSolicitud == SCMD_solicitud_estatus_enum.ASIGNADA_A_SCDM
                        || x.EstatusSolicitud == SCMD_solicitud_estatus_enum.ASIGNADA_A_DEPARTAMENTOS
                        || x.EstatusSolicitud == SCMD_solicitud_estatus_enum.ASIGNADA_A_SCDM_Y_DEPARTAMENTOS
                        || x.EstatusSolicitud == SCMD_solicitud_estatus_enum.RECHAZADA_ASIGNADA_A_SCDM
            ).ToList();
            //listado rechadas (para usuario)
            var listRechazadas = listTotal.Where(x =>
                         x.EstatusSolicitud == SCMD_solicitud_estatus_enum.RECHAZADA_ASIGNADA_A_SOLICITANTE
            ).ToList();
            //finalizadas
            var listFinalizadas = listTotal.Where(x => x.EstatusSolicitud == SCMD_solicitud_estatus_enum.FINALIZADA).ToList();
            //canceladas
            var listCanceladas = listTotal.Where(x => x.EstatusSolicitud == SCMD_solicitud_estatus_enum.CANCELADA).ToList();

            List<SCDM_solicitud> listado = new List<SCDM_solicitud> { };

            //stringdetermina la lista a buscar
            switch (estatus)
            {
                case "CREADAS":
                    listado = listCreadas;
                    break;
                case "EN_PROCESO":
                    listado = listEnProceso;
                    break;
                case "RECHAZADAS":
                    listado = listRechazadas;
                    break;
                case "FINALIZADAS":
                    listado = listFinalizadas;
                    break;
                case "CANCELADAS":
                    listado = listCanceladas;
                    break;
                default:
                    listado = listTotal;
                    break;
            }

            var totalDeRegistros = listado
                .Count();

            listado = listado
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();



            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["pagina"] = pagina;

            //map para estatus
            Dictionary<string, string> estatusMap = new Dictionary<string, string>();
            estatusMap.Add("", "Todas");
            estatusMap.Add("CREADAS", "Creadas (Sin enviar)");
            estatusMap.Add("EN_PROCESO", "En Proceso");
            estatusMap.Add("RECHAZADAS", "Rechazadas");
            estatusMap.Add("FINALIZADAS", "Finalizadas");
            estatusMap.Add("CANCELADAS", "Canceladas");

            ViewBag.estatusMap = estatusMap;

            //map para cantidad de status
            Dictionary<string, int> estatusAmount = new Dictionary<string, int>
            {
                { "", listTotal.Count() },
                //obtiene las solicitudeS CREADAS
                { "CREADAS", listCreadas.Count()},
                //obtiene las solicitudes en proceso
                { "EN_PROCESO", listEnProceso.Count()},
                //obtiene las solicitudes en proceso
                { "RECHAZADAS", listRechazadas.Count()},
                //obtiene las sulicitudes cuya ultima asignación ha sido rechazada
                { "FINALIZADAS", listFinalizadas.Count()},
                //canceladas
                { "CANCELADAS", listCanceladas.Count()},
            };

            ViewBag.estatusAmount = estatusAmount;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.Paginacion = paginacion;

            return View(listado);
        }

        /// <summary>
        /// Obtiene el estatus y asignaciones actuales de la solicitud
        /// </summary>
        /// <param name="estatus"></param>
        /// <param name="pagina"></param>
        /// <returns></returns>
        public ActionResult Estatus(string estatus, int? id_solicitud, string fecha_inicio, string fecha_fin)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) && !TieneRol(TipoRoles.SCDM_MM_APROBACION_SOLICITUDES) &&
                !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES) && !TieneRol(TipoRoles.SCDM_MM_REPORTES)
                && !TieneRol(TipoRoles.SCDM_MM_APROBACION_INICIAL)
                )
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //convierte fechas
            DateTime fechaActual = DateTime.Now;
            DateTime dateInicial = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day, 0, 0, 0).AddMonths(-2);  //fecha inicial por defecto
            DateTime dateFinal = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day, 23, 59, 59);          //fecha final por defecto

            try
            {
                if (!String.IsNullOrEmpty(fecha_inicio))
                    dateInicial = Convert.ToDateTime(fecha_inicio);
                if (!String.IsNullOrEmpty(fecha_fin))
                {
                    dateFinal = Convert.ToDateTime(fecha_fin);
                    dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine("Error de Formato: " + e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al convertir: " + ex.Message);
            }

            var empleado = obtieneEmpleadoLogeado();
            var listTotal = db.SCDM_solicitud.Where(x => x.activo && x.fecha_creacion >= dateInicial && x.fecha_creacion <= dateFinal && (id_solicitud == null || (id_solicitud.HasValue && x.id == id_solicitud))).ToList().Where(x => x.SCDM_solicitud_asignaciones.Any() // ya fue asignado al menos una vez
                                        ).ToList();
            //listado sin finalizar 
            var listEnProceso = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null)).ToList();
            //listado finalizadas 
            var listFinalizadas = listTotal.Where(x => !x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null)).ToList();


            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["fecha_inicio"] = fecha_inicio;
            routeValues["fecha_fin"] = fecha_fin;    

            ViewBag.Fecha_inicio = dateInicial;
            ViewBag.Fecha_fin = dateFinal;

            //map para estatus
            Dictionary<string, string> estatusMap = new Dictionary<string, string>
            {
                { "", "Todas" },
                { "ASIGNADA", "En Proceso" },
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.FINALIZADA), "Finalizadas" }
            };

            ViewBag.estatusMap = estatusMap;

            //map para cantidad de status
            Dictionary<string, int> estatusAmount = new Dictionary<string, int>
            {
                { "", listTotal.Count() },          
                //obtiene las solicitudes abiertas para cualquier otro departamento
                { "ASIGNADA", listEnProceso.Count() },
                //obtiene las sulicitudes cuya ultima asignación ha sido rechazada
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.FINALIZADA), listFinalizadas.Count()},
            };

            ViewBag.estatusAmount = estatusAmount;

            return View(new List<SCDM_solicitud> { });
        }
        public ActionResult MaterialesPorSolicitud(string material, int? id_solicitud, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) && !TieneRol(TipoRoles.SCDM_MM_APROBACION_SOLICITUDES) &&
                !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES) && !TieneRol(TipoRoles.SCDM_MM_REPORTES)
                && !TieneRol(TipoRoles.SCDM_MM_APROBACION_INICIAL)
                )
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro

            var listTotal = db.view_SCDM_materiales_x_solicitud.Where(x =>
                                        (id_solicitud == null || x.numero_solicitud == id_solicitud)
                                        && (string.IsNullOrEmpty(material) || x.numero_material == material)
                                        );

            List<view_SCDM_materiales_x_solicitud> listado = new List<view_SCDM_materiales_x_solicitud> { };

            var totalDeRegistros = listTotal
               .Count();

            listado = listTotal
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["material"] = material;
            routeValues["id_solicitud"] = material;
            routeValues["pagina"] = pagina;


            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.Paginacion = paginacion;
            return View(listado);
        }

        // GET: SCDM_solicitud
        public ActionResult SolicitudesSCDM(string estatus, int? id_solicitud, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro

            var listTotal = db.SCDM_solicitud.Where(x => id_solicitud == null || (id_solicitud.HasValue && x.id == id_solicitud)).ToList();

            //listado de creadas sin enviar
            var listCreadas = listTotal.Where(x => x.EstatusSolicitud == SCMD_solicitud_estatus_enum.CREADO).ToList();
            //listado Asignadas a SCDM                       
            var listAsignadasSCDM = listTotal.Where(x =>
                           x.EstatusSolicitud == SCMD_solicitud_estatus_enum.ASIGNADA_A_SCDM
                        || x.EstatusSolicitud == SCMD_solicitud_estatus_enum.ASIGNADA_A_SCDM_Y_DEPARTAMENTOS
                        || x.EstatusSolicitud == SCMD_solicitud_estatus_enum.RECHAZADA_ASIGNADA_A_SCDM
            ).ToList();
            //listado asignadas a otros Departamentos
            var listAsignadasOtrosDepartamentos = listTotal.Where(x =>
                           x.EstatusSolicitud == SCMD_solicitud_estatus_enum.EN_REVISION_INICIAL
                        || x.EstatusSolicitud == SCMD_solicitud_estatus_enum.ASIGNADA_A_SCDM_Y_DEPARTAMENTOS
                        || x.EstatusSolicitud == SCMD_solicitud_estatus_enum.ASIGNADA_A_DEPARTAMENTOS
            ).ToList();
            //rechazadas
            var listRechazadasSolicitante = listTotal.Where(x =>
                           x.EstatusSolicitud == SCMD_solicitud_estatus_enum.RECHAZADA_ASIGNADA_A_SOLICITANTE
            ).ToList();

            //rechazadas SCDM
            var listRechazadasSCDM = listTotal.Where(x =>
                           x.EstatusSolicitud == SCMD_solicitud_estatus_enum.RECHAZADA_ASIGNADA_A_SCDM
            ).ToList();

            //finalizadas
            var listFinalizadas = listTotal.Where(x =>
                           x.EstatusSolicitud == SCMD_solicitud_estatus_enum.FINALIZADA
            ).ToList();

            //canceladas
            var listCanceladas = listTotal.Where(x =>
                           x.EstatusSolicitud == SCMD_solicitud_estatus_enum.CANCELADA
            ).ToList();

            List<SCDM_solicitud> listado = new List<SCDM_solicitud> { };

            //stringdetermina la lista a buscar
            switch (estatus)
            {
                case "CREADAS":
                    listado = listCreadas;
                    break;
                case "ASIGNADAS_A_SCDM":
                    listado = listAsignadasSCDM;
                    break;
                case "ASIGNADAS_A_DEPARTAMENTO":
                    listado = listAsignadasOtrosDepartamentos;
                    break;
                case "RECHAZADA_SCDM":
                    listado = listRechazadasSCDM;
                    break;
                case "RECHAZADA_SOLICITANTE":
                    listado = listRechazadasSolicitante;
                    break;
                case "FINALIZADA":
                    listado = listFinalizadas;
                    break;
                case "CANCELADA":
                    listado = listCanceladas;
                    break;
                default:
                    listado = listTotal;
                    break;
            }


            var totalDeRegistros = listado
               .Count();

            listado = listado
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["pagina"] = pagina;

            //map para estatus
            Dictionary<string, string> estatusMap = new Dictionary<string, string>
            {
                { "", "Todas" },
                { "CREADAS", "Creadas" },
                { "ASIGNADAS_A_SCDM", "Asignadas a SCDM" },
                { "ASIGNADAS_A_DEPARTAMENTO", "Asignadas a Deptos." },
                { "RECHAZADA_SCDM", "Rechazadas - SCDM" },
                { "RECHAZADA_SOLICITANTE", "Rechazadas - Solicitante" },
                { "FINALIZADA", "Finalizadas" },
                { "CANCELADA", "Canceladas" },

            };

            ViewBag.estatusMap = estatusMap;

            //map para cantidad de status
            Dictionary<string, int> estatusAmount = new Dictionary<string, int>
            {
                { "", listTotal.Count() },
                { "CREADAS", listCreadas.Count()},
                { "ASIGNADAS_A_SCDM", listAsignadasSCDM.Count()},
                { "ASIGNADAS_A_DEPARTAMENTO", listAsignadasOtrosDepartamentos.Count()},
                { "RECHAZADA_SCDM", listRechazadasSCDM.Count()},
                { "RECHAZADA_SOLICITANTE", listRechazadasSolicitante.Count()},
                { "FINALIZADA", listFinalizadas.Count()},
                { "CANCELADA", listCanceladas.Count()}
            };

            ViewBag.estatusAmount = estatusAmount;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.Paginacion = paginacion;

            return View(listado);
        }
        // GET: SolicitudesDepartamento
        public ActionResult SolicitudesDepartamento(string estatus, string cliente, int? id_solicitud, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_APROBACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro

            //obtiene el departamento del empleado que inicia seción
            var empleado = obtieneEmpleadoLogeado();

            //ERROR GENERICO
            if (empleado.SCDM_cat_rel_usuarios_departamentos.Count == 0
                )
            {
                ViewBag.Titulo = "El usuario no se encuentra asignado a un departamento.";
                ViewBag.Descripcion = "Su usuario no se encuentra asignado a ningún departamento. Contacte a Sistemas para continuar.";

                return View("../Home/ErrorGenerico");
            }

            //obtiene el departamento del empleado
            var id_depto_scdm = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault().id_departamento;

            //obtiene los departamentos asociados al empleado
            var plantas_asignadas = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault().SCDM_cat_usuarios_revision_departamento.Select(x => x.id_planta_solicitud).Distinct();


            var listTotal = db.SCDM_solicitud.ToList().Where(x => x.activo && x.SCDM_solicitud_asignaciones.Any(y =>
                         y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO)
                        && (id_solicitud == null || (id_solicitud.HasValue && x.id == id_solicitud)) //aplica fitro de num solicitud
                        && (String.IsNullOrEmpty(cliente) || (!String.IsNullOrEmpty(cliente) && x.ExisteClienteEnSolicitud(cliente))) //aplica fitro de cliente
                        && x.SCDM_rel_solicitud_plantas.Any(y => plantas_asignadas.Contains(y.id_planta)) //verifica que el empleado este asignado a esta planta
            ).ToList();

            //listado de solicitudes pendientes
            var listPendientes = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null && y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO)).ToList();
            //listado solicitudes aprobadas
            var listAprobadas = listTotal.Where(x => !x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null && y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO) //Solicitudes procesadas
                        && x.SCDM_solicitud_asignaciones.Any(y => y.id_cierre != null && y.id_departamento_asignacion == id_depto_scdm
                             && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO)
            ).ToList();

            //listado solicitudes rechazadas
            var listRechazadas = listTotal.Where(x => !x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null && y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO) //Solicitudes procesadas
                        && !x.SCDM_solicitud_asignaciones.Any(y => y.id_cierre != null && y.id_departamento_asignacion == id_depto_scdm
                             && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO)
            ).ToList();

            List<SCDM_solicitud> listado = new List<SCDM_solicitud> { };

            //determina la lista a buscar
            switch (estatus)
            {
                case "Pendientes":
                    listado = listPendientes;
                    break;
                case "Aprobadas":
                    listado = listAprobadas;
                    break;
                case "Rechazadas":
                    listado = listRechazadas;
                    break;
                default:
                    listado = listTotal;
                    break;
            }

            var totalDeRegistros = listado
               .Count();

            listado = listado
                   .OrderByDescending(x => x.fecha_creacion)
                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["pagina"] = pagina;

            //map para estatus
            Dictionary<string, string> estatusMap = new Dictionary<string, string>
            {
                { "", "Todas" },
                {"Pendientes", "Pendientes" },
                {"Aprobadas", "Aprobadas" },
                {"Rechazadas", "Rechazadas" },
            };

            ViewBag.estatusMap = estatusMap;

            //map para cantidad de status
            Dictionary<string, int> estatusAmount = new Dictionary<string, int>
            {
                { "", listTotal.Count() },
                { "Pendientes", listPendientes.Count() },
                { "Aprobadas", listAprobadas.Count() },
                { "Rechazadas", listRechazadas.Count() },

            };
            ViewBag.estatusAmount = estatusAmount;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.Paginacion = paginacion;
            ViewBag.EmpleadoID = empleado.id;
            ViewBag.EmpleadoDepartamento = id_depto_scdm;


            return View(listado);
        }

        /// <summary>
        ///  Obtiene únicamente las solicitudes que tienen asignación iniciacial pendiente
        /// </summary>
        /// <param name="estatus"></param>
        /// <param name="pagina"></param>cccccbhctrnrjdlffndghgcdtlkcfjlllhgvubvljeef
        /// 
        /// <returns></returns>
        public ActionResult SolicitudesRevisionInicial(string estatus, int? id_solicitud, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_APROBACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro

            //obtiene el departamento del empleado que inicia seción
            var empleado = obtieneEmpleadoLogeado();

            //ERROR GENERICO
            if (empleado.SCDM_cat_rel_usuarios_departamentos.Count == 0
                )
            {
                ViewBag.Titulo = "El usuario no se encuentra asignado a un departamento.";
                ViewBag.Descripcion = "Su usuario no se encuentra asignado a ningún departamento. Contacte a Sistemas para continuar.";

                return View("../Home/ErrorGenerico");
            }

            //obtiene el departamento del empleado
            var id_depto_scdm = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault().id_departamento;

            //obtiene los departamentos asociados al empleado
            var plantas_asignadas = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault().SCDM_cat_usuarios_revision_departamento.Select(x => x.id_planta_solicitud).Distinct();

            //cuenta la totalidad de los registros
            var listTotal = db.SCDM_solicitud.Where(x => x.activo && x.SCDM_solicitud_asignaciones.Any(y => //y.fecha_cierre == null && y.fecha_rechazo == null
                         y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL
                        && y.id_empleado == empleado.id // verifica que el empleado asignado sea quien inicia sesión
                        && (id_solicitud == null || (id_solicitud.HasValue && x.id == id_solicitud)) //aplica fitro de num solicitud
                        )
                        && x.SCDM_rel_solicitud_plantas.Any(y => plantas_asignadas.Contains(y.id_planta)) //verifica que el empleado este asignado a esta planta
            ).ToList();
            //listado de solicitudes pendientes
            var listPendientes = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null && y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL)).ToList();
            //listado solicitudes aprobadas
            var listAprobadas = listTotal.Where(x => !x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null && y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL) //Solicitudes procesadas
                        && x.SCDM_solicitud_asignaciones.Any(y => y.id_cierre != null && y.id_departamento_asignacion == id_depto_scdm
                             && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL)
            ).ToList();

            //listado solicitudes rechazadas
            var listRechazadas = listTotal.Where(x => !x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null && y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL) //Solicitudes procesadas
                        && !x.SCDM_solicitud_asignaciones.Any(y => y.id_cierre != null && y.id_departamento_asignacion == id_depto_scdm
                             && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL)
            ).ToList();

            List<SCDM_solicitud> listado = new List<SCDM_solicitud> { };

            //determina la lista a buscar
            switch (estatus)
            {
                case "Pendientes":
                    listado = listPendientes;
                    break;
                case "Aprobadas":
                    listado = listAprobadas;
                    break;
                case "Rechazadas":
                    listado = listRechazadas;
                    break;
                default:
                    listado = listTotal;
                    break;
            }

            var totalDeRegistros = listado
                .Count();

            listado = listado
                   .OrderByDescending(x => x.fecha_creacion)
                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();


            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["pagina"] = pagina;

            //map para estatus
            Dictionary<string, string> estatusMap = new Dictionary<string, string>
            {
                { "", "Todas" },
                {"Pendientes", "Pendientes" },
                {"Aprobadas", "Aprobadas" },
                {"Rechazadas", "Rechazadas" },
            };

            ViewBag.estatusMap = estatusMap;

            //map para cantidad de status
            Dictionary<string, int> estatusAmount = new Dictionary<string, int>
            {
                { "", listTotal.Count() },
                  //obtiene las solicitudes abiertas para cualquier otro departamento
                { "Pendientes", listPendientes.Count() },
                { "Aprobadas", listAprobadas.Count() },
                { "Rechazadas", listRechazadas.Count() },

            };

            ViewBag.estatusAmount = estatusAmount;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.Paginacion = paginacion;
            ViewBag.EmpleadoID = empleado.id;
            ViewBag.EmpleadoDepartamento = id_depto_scdm;


            return View(listado);
        }

        // GET: SCDM_solicitud/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
                    !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES) &&
                    !TieneRol(TipoRoles.SCDM_MM_APROBACION_SOLICITUDES) &&
                    !TieneRol(TipoRoles.SCDM_MM_APROBACION_INICIAL)
                    )
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //obtiene el nombre del empleado al que se enviará para validación Inicial
            string revisaFormato = Bitacoras.Util.MM_revisa_formato_departamento.MM_REVISA_FORMATO_SCDM; //valor por defecto
            int id_departamento = (int)SCDM_departamentos_AsignacionENUM.SCDM;
            //obtiene los empleados, según las notificaciones de correo en el sistema
            string revisaNobre = string.Empty;

            var revisaCorreo = db.notificaciones_correo.Where(x => x.descripcion == revisaFormato).OrderByDescending(x => x.id).ToList().Select(x => x.empleados.ConcatNumEmpleadoNombrePlanta).ToList();

            //envia el motivo de rechazo 
            var rechazoAsign = sCDM_solicitud.SCDM_solicitud_asignaciones.LastOrDefault(x => x.fecha_rechazo != null);
            if (rechazoAsign != null)
            {
                sCDM_solicitud.comentario_rechazo = rechazoAsign.comentario_rechazo;
                sCDM_solicitud.comentario_rechazo_departamento = rechazoAsign.comentario_rechazo;
                sCDM_solicitud.id_motivo_rechazo = rechazoAsign.id_motivo_rechazo.Value;
                sCDM_solicitud.id_motivo_rechazo_departamento = rechazoAsign.id_motivo_rechazo.Value;
            }

            //determina la asignación que abre la solicitud
            ViewBag.revisaNombre = String.Join("<br> ", revisaCorreo);
            ViewBag.revisaFormato = revisaFormato;
            ViewBag.revisaDepartamento = id_departamento;
            ViewBag.secciones = AddFirstItem(new SelectList(db.SCDM_cat_secciones.Where(x => x.activo == true && x.aplica_solicitud == true), nameof(SCDM_cat_secciones.id), nameof(SCDM_cat_secciones.descripcion)));
            ViewBag.id_motivo_rechazo = AddFirstItem(new SelectList(db.SCDM_cat_motivo_rechazo.Where(x => x.activo == true), nameof(SCDM_cat_motivo_rechazo.id), nameof(SCDM_cat_motivo_rechazo.descripcion)), selected: (rechazoAsign != null ? rechazoAsign.id_motivo_rechazo.ToString() : string.Empty));
            ViewBag.id_motivo_asignacion_incorrecta = AddFirstItem(new SelectList(db.SCDM_cat_motivo_asignacion_incorrecta.Where(x => x.activo == true), nameof(SCDM_cat_motivo_asignacion_incorrecta.id), nameof(SCDM_cat_motivo_asignacion_incorrecta.descripcion)));
            ViewBag.EmpleadoDepartamento = id_departamento;
            var deptos = new List<SCDM_cat_departamentos_asignacion> { new SCDM_cat_departamentos_asignacion { descripcion = "Solicitante", id = 99 } };
            deptos.AddRange(db.SCDM_cat_departamentos_asignacion.Where(x => x.activo).ToList());
            ViewBag.ListadoDepartamentos = deptos;
            ViewBag.Details = true;

            return View("EditarSolicitud", sCDM_solicitud);
        }

        // GET: SCDM_solicitud/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
                !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            var solicitante = obtieneEmpleadoLogeado();

            SCDM_solicitud solicitud = new SCDM_solicitud
            {
                id_solicitante = solicitante.id,
                empleados = solicitante,
                fecha_creacion = DateTime.Now,
                activo = true,
                on_hold = false,
            };

            ViewBag.listPlantas = db.plantas.Where(x => x.aplica_solicitud_scdm == true).ToList();
            ViewBag.listTipoMateriales = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true && x.id != 7).ToList(); //menos 7= Barreno
            ViewBag.id_prioridad = AddFirstItem(new SelectList(db.SCDM_cat_prioridad.Where(x => x.activo == true), nameof(SCDM_cat_prioridad.id), nameof(SCDM_cat_prioridad.descripcion)));
            ViewBag.id_tipo_cambio = AddFirstItem(new SelectList(db.SCDM_cat_tipo_cambio.Where(x => x.activo == true), nameof(SCDM_cat_tipo_cambio.id), nameof(SCDM_cat_tipo_cambio.descripcion)));
            ViewBag.id_tipo_solicitud = AddFirstItem(new SelectList(db.SCDM_cat_tipo_solicitud.Where(x => x.activo == true), nameof(SCDM_cat_tipo_solicitud.id), nameof(SCDM_cat_tipo_solicitud.descripcion)));
            return View(solicitud);
        }

        // POST: SCDM_solicitud/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SCDM_solicitud sCDM_solicitud, FormCollection collection, string[] SelectedMateriales, string[] SelectedPlantas, string[] SelectedPlantasOrigen)
        {
            var solicitante = obtieneEmpleadoLogeado();

            #region asignacion_listas

            //lista de key del collection
            List<string> keysCollection = collection.AllKeys.ToList();

            //crea los objetos para Materiales
            if (SelectedMateriales != null && (sCDM_solicitud.id_tipo_solicitud == 1 || sCDM_solicitud.id_tipo_solicitud == 2 || sCDM_solicitud.id_tipo_solicitud == 5))
                foreach (string material_string in SelectedMateriales)
                {
                    //obtiene el id
                    Match m = Regex.Match(material_string, @"\d+");
                    int id_material_int = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_material_int);

                    sCDM_solicitud.SCDM_rel_solicitud_materiales_solicitados.Add(new SCDM_rel_solicitud_materiales_solicitados { id_tipo_material = id_material_int });
                }

            if (SelectedPlantas != null)
                foreach (string planta_string in SelectedPlantas)
                {
                    //obtiene el id
                    Match m = Regex.Match(planta_string, @"\d+");
                    int id_planta_int = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_planta_int);

                    sCDM_solicitud.SCDM_rel_solicitud_plantas.Add(new SCDM_rel_solicitud_plantas { id_planta = id_planta_int });
                    sCDM_solicitud.planta_solicitud = id_planta_int;
                }

            if (SelectedPlantasOrigen != null)
                foreach (string planta_string in SelectedPlantasOrigen)
                {
                    //obtiene el id
                    Match m = Regex.Match(planta_string, @"\d+");
                    int id_planta_int = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_planta_int);

                    //agrega la planta de origen, solo si se trata de extensón
                    if (sCDM_solicitud.id_tipo_solicitud == (int)Bitacoras.Util.SCDMTipoSolicitudENUM.EXTENSION)
                        sCDM_solicitud.planta_origen_extension = id_planta_int;
                }

            #endregion

            #region validacion de archivos
            List<HttpPostedFileBase> archivosForm = new List<HttpPostedFileBase>();
            //agrega archivos enviados
            if (sCDM_solicitud.PostedFileSolicitud_1 != null)
                archivosForm.Add(sCDM_solicitud.PostedFileSolicitud_1);
            if (sCDM_solicitud.PostedFileSolicitud_2 != null)
                archivosForm.Add(sCDM_solicitud.PostedFileSolicitud_2);
            if (sCDM_solicitud.PostedFileSolicitud_3 != null)
                archivosForm.Add(sCDM_solicitud.PostedFileSolicitud_3);

            #region lectura de archivos

            foreach (HttpPostedFileBase httpPostedFileBase in archivosForm)
            {
                //verifica si el tamaño del archivo es OT 1
                if (httpPostedFileBase != null && httpPostedFileBase.InputStream.Length > 10485760)
                    ModelState.AddModelError("", "Sólo se permiten archivos menores a 10MB: " + httpPostedFileBase.FileName + ".");
                else if (httpPostedFileBase != null)
                { //verifica la extensión del archivo
                    string extension = Path.GetExtension(httpPostedFileBase.FileName);
                    if (extension.ToUpper() == ".EXE"   //si contiene una extensión inválida
                                   )
                        ModelState.AddModelError("", "No se permiten archivos con extensión " + extension + ": " + httpPostedFileBase.FileName + ".");
                    else
                    { //si la extension y el tamaño son válidos
                        String nombreArchivo = httpPostedFileBase.FileName;

                        //recorta el nombre del archivo en caso de ser necesario
                        if (nombreArchivo.Length > 80)
                            nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                        //lee el archivo en un arreglo de bytes
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(httpPostedFileBase.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(httpPostedFileBase.ContentLength);
                        }

                        //genera el archivo de biblioce digital
                        biblioteca_digital archivo = new biblioteca_digital
                        {
                            Nombre = Regex.Replace(nombreArchivo, @"[^\w\s.]+", ""),
                            MimeType = UsoStrings.RecortaString(httpPostedFileBase.ContentType, 80),
                            Datos = fileData
                        };
                        sCDM_solicitud.SCDM_rel_solicitud_archivos.Add(new SCDM_rel_solicitud_archivos { biblioteca_digital = archivo });
                    }
                }
            }


            #endregion


            #endregion
            //validaciones de las listas

            if (SelectedMateriales == null && (sCDM_solicitud.id_tipo_solicitud == (int)SCDMTipoSolicitudENUM.CREACION_MATERIALES || sCDM_solicitud.id_tipo_solicitud == (int)SCDMTipoSolicitudENUM.CREACION_REFERENCIA))
                ModelState.AddModelError("", "Seleccione los materiales deseados para la solicitud.");

            if (SelectedPlantasOrigen == null && sCDM_solicitud.id_tipo_solicitud == (int)SCDMTipoSolicitudENUM.EXTENSION)
                ModelState.AddModelError("", "Seleccione la planta origen de la solicitud.");

            if (SelectedPlantas == null)
                ModelState.AddModelError("", "Seleccione la planta destino de la solicitud.");

            if (sCDM_solicitud.id_prioridad == 2 && string.IsNullOrEmpty(sCDM_solicitud.justificacion)) //critico
            {
                ModelState.AddModelError("", "La justificación es necesaria en prioridad crítica.");
                ModelState.AddModelError(nameof(SCDM_solicitud.justificacion), "El campo de justificación es obligatorio.");
            }

            //si es creación o creacion referencia, al menos un archivo es requerido
            if ((sCDM_solicitud.id_tipo_solicitud == (int)SCDMTipoSolicitudENUM.CREACION_MATERIALES /*|| sCDM_solicitud.id_tipo_solicitud == (int)SCDMTipoSolicitudENUM.CREACION_REFERENCIA*/) && archivosForm.Count == 0)
                ModelState.AddModelError("", "Agregue al menos un archivo de soporte, factibilidad, desviación, etc.");

            //si no es cambios
            if (sCDM_solicitud.id_tipo_solicitud != 3)
                sCDM_solicitud.id_tipo_cambio = null;

            // ModelState.AddModelError("", "Error para depuración.");

            if (ModelState.IsValid)
            {
                sCDM_solicitud.fecha_creacion = DateTime.Now;

                db.SCDM_solicitud.Add(sCDM_solicitud);
                try
                {
                    //agrega los componentes según el tipo de solocitud

                    switch ((SCDMTipoSolicitudENUM)sCDM_solicitud.id_tipo_solicitud)
                    {

                        case SCDMTipoSolicitudENUM.CREACION_MATERIALES:

                            bool activaLista = false;
                            foreach (var item in sCDM_solicitud.SCDM_rel_solicitud_materiales_solicitados)
                            {
                                //determina la seccion a mostrar según el tipo de material solicitado
                                int seccion = 0;

                                if (item.id_tipo_material == (int)SCDM_solicitud_rel_item_material_tipo.ROLLO)
                                    seccion = (int)SCDMSeccionesSolicitud.ROLLO;

                                if (item.id_tipo_material == (int)SCDM_solicitud_rel_item_material_tipo.CINTA)
                                    seccion = (int)SCDMSeccionesSolicitud.CINTA;

                                if (item.id_tipo_material == (int)SCDM_solicitud_rel_item_material_tipo.PLATINA)
                                    seccion = (int)SCDMSeccionesSolicitud.PLATINA;

                                if (item.id_tipo_material == (int)SCDM_solicitud_rel_item_material_tipo.PLATINA_SOLDADA)
                                    seccion = (int)SCDMSeccionesSolicitud.PLATINA_SOLDADA;

                                if (item.id_tipo_material == (int)SCDM_solicitud_rel_item_material_tipo.SHEARING)
                                    seccion = (int)SCDMSeccionesSolicitud.SHEARING;

                                if (item.id_tipo_material == (int)SCDM_solicitud_rel_item_material_tipo.C_B)
                                    seccion = (int)SCDMSeccionesSolicitud.C_AND_B;

                                if (item.id_tipo_material == (int)SCDM_solicitud_rel_item_material_tipo.PLATINA
                                    || item.id_tipo_material == (int)SCDM_solicitud_rel_item_material_tipo.PLATINA_SOLDADA
                                    || item.id_tipo_material == (int)SCDM_solicitud_rel_item_material_tipo.SHEARING)
                                    activaLista = true;


                                if (seccion != 0)
                                    sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                                    {
                                        id_seccion = seccion
                                    });
                            }

                            if (activaLista)
                                sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                                {
                                    id_seccion = (int)SCDMSeccionesSolicitud.LISTA_TECNICA
                                });

                            break;

                        case SCDMTipoSolicitudENUM.CREACION_REFERENCIA:
                            sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                            {
                                id_seccion = (int)SCDMSeccionesSolicitud.CREACION_REFERENCIA
                            });
                            break;
                        case SCDMTipoSolicitudENUM.CREACION_CB:
                            sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                            {
                                id_seccion = (int)SCDMSeccionesSolicitud.C_AND_B
                            });
                            break;
                        case SCDMTipoSolicitudENUM.CAMBIOS:
                            //agrega el elemento de Cambio de Ingenieria
                            if (sCDM_solicitud.id_tipo_cambio.HasValue && sCDM_solicitud.id_tipo_cambio.Value == (int)SCDMTipoCambioENUM.CAMBIOS_INGENIERIA)
                                sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                                {
                                    id_seccion = (int)SCDMSeccionesSolicitud.CAMBIO_INGENIERIA
                                });
                            //agrega el elemento de Cambio de Estatus
                            if (sCDM_solicitud.id_tipo_cambio.HasValue && (sCDM_solicitud.id_tipo_cambio.Value == (int)SCDMTipoCambioENUM.ACTIVAR_MATERIAL
                                || sCDM_solicitud.id_tipo_cambio.Value == (int)SCDMTipoCambioENUM.CAMBIOS_OBSOLETAR_MATERIALES
                                ))
                                sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                                {
                                    id_seccion = (int)SCDMSeccionesSolicitud.CAMBIO_ESTATUS
                                });
                            //agrega el elemento de Cambio de Estatus
                            if (sCDM_solicitud.id_tipo_cambio.HasValue && sCDM_solicitud.id_tipo_cambio.Value == (int)SCDMTipoCambioENUM.CAMBIOS_ORDEN_COMPRA)

                                sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                                {
                                    id_seccion = (int)SCDMSeccionesSolicitud.FORMATO_COMPRA
                                });
                            //agrega el elemento de Cambio en valores de Budget
                            if (sCDM_solicitud.id_tipo_cambio.HasValue && sCDM_solicitud.id_tipo_cambio.Value == (int)SCDMTipoCambioENUM.CAMBIOS_DATOS_BUDGET)

                                sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                                {
                                    id_seccion = (int)SCDMSeccionesSolicitud.CAMBIO_BUDGET
                                });
                            //agrega el elemento de Cambio lista tecnica
                            if (sCDM_solicitud.id_tipo_cambio.HasValue && sCDM_solicitud.id_tipo_cambio.Value == (int)SCDMTipoCambioENUM.CAMBIOS_LISTA_TECNICA)

                                sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                                {
                                    id_seccion = (int)SCDMSeccionesSolicitud.LISTA_TECNICA
                                });
                            break;
                        case SCDMTipoSolicitudENUM.CREAR_MRO:
                        case SCDMTipoSolicitudENUM.CREACION_SERVICIOS:

                            sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                            {
                                id_seccion = (int)SCDMSeccionesSolicitud.FORMATO_COMPRA
                            });
                            break;
                        case SCDMTipoSolicitudENUM.EXTENSION:

                            sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                            {
                                id_seccion = (int)SCDMSeccionesSolicitud.EXTENSION
                            });
                            break;

                    }

                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ha comenzado la solicitud correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("EditarSolicitud", new { id = sCDM_solicitud.id, viewUser = (int)SCDM_tipo_view_edicionENUM.SOLICITANTE });
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);

                }
                return RedirectToAction("Index");
            }

            sCDM_solicitud.fecha_creacion = DateTime.Now;
            sCDM_solicitud.empleados = solicitante;

            ViewBag.listPlantas = db.plantas.Where(x => x.aplica_solicitud_scdm == true).ToList();
            ViewBag.listTipoMateriales = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true && x.id != 7).ToList(); //menos barreno
            ViewBag.id_prioridad = AddFirstItem(new SelectList(db.SCDM_cat_prioridad.Where(x => x.activo == true), nameof(SCDM_cat_prioridad.id), nameof(SCDM_cat_prioridad.descripcion)));
            ViewBag.id_tipo_cambio = AddFirstItem(new SelectList(db.SCDM_cat_tipo_cambio.Where(x => x.activo == true), nameof(SCDM_cat_tipo_cambio.id), nameof(SCDM_cat_tipo_cambio.descripcion)));
            ViewBag.id_tipo_solicitud = AddFirstItem(new SelectList(db.SCDM_cat_tipo_solicitud.Where(x => x.activo == true), nameof(SCDM_cat_tipo_solicitud.id), nameof(SCDM_cat_tipo_solicitud.descripcion)));
            return View(sCDM_solicitud);
        }

        // GET: SCDM_solicitud/Edit/5
        public ActionResult Edit(int? id, string viewUser)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
               !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud solicitud = db.SCDM_solicitud.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }
            var solicitante = obtieneEmpleadoLogeado();

            solicitud.viewUser = viewUser;

            ViewBag.listPlantas = db.plantas.Where(x => x.aplica_solicitud_scdm == true).ToList();
            ViewBag.listTipoMateriales = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList();
            ViewBag.id_prioridad = AddFirstItem(new SelectList(db.SCDM_cat_prioridad.Where(x => x.activo == true), nameof(SCDM_cat_prioridad.id), nameof(SCDM_cat_prioridad.descripcion)), selected: solicitud.id_prioridad.ToString());
            ViewBag.id_tipo_cambio = AddFirstItem(new SelectList(db.SCDM_cat_tipo_cambio.Where(x => x.activo == true), nameof(SCDM_cat_tipo_cambio.id), nameof(SCDM_cat_tipo_cambio.descripcion)), selected: solicitud.id_tipo_cambio.ToString());
            ViewBag.id_tipo_solicitud = AddFirstItem(new SelectList(db.SCDM_cat_tipo_solicitud.Where(x => x.activo == true), nameof(SCDM_cat_tipo_solicitud.id), nameof(SCDM_cat_tipo_solicitud.descripcion)), selected: solicitud.id_tipo_solicitud.ToString());
            return View(solicitud);
        }

        // POST: SCDM_solicitud/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SCDM_solicitud sCDM_solicitud, FormCollection collection, string[] SelectedMateriales, string[] SelectedPlantas, string[] SelectedPlantasOrigen)
        {
            var solicitante = obtieneEmpleadoLogeado();
            List<int> idsArchivos = new List<int>();

            SCDM_solicitud solicitudBD = db.SCDM_solicitud.Find(sCDM_solicitud.id);

            #region asignacion_listas

            //lista de key del collection
            List<string> keysCollection = collection.AllKeys.ToList();

            //busca si hay archivos 
            foreach (string key in keysCollection.Where(x => x.Contains("id_archivo_")))
            {
                int number;
                if (int.TryParse(collection[key], out number))
                {
                    idsArchivos.Add(number);
                }
            }

            //crea los objetos para Materiales
            if (SelectedMateriales != null && (sCDM_solicitud.id_tipo_solicitud == 1 || sCDM_solicitud.id_tipo_solicitud == 2 || sCDM_solicitud.id_tipo_solicitud == 5))
                foreach (string material_string in SelectedMateriales)
                {
                    //obtiene el id
                    Match m = Regex.Match(material_string, @"\d+");
                    int id_material_int = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_material_int);

                    sCDM_solicitud.SCDM_rel_solicitud_materiales_solicitados.Add(new SCDM_rel_solicitud_materiales_solicitados { id_tipo_material = id_material_int, id_solicitud = sCDM_solicitud.id });
                }

            if (SelectedPlantas != null)
                foreach (string planta_string in SelectedPlantas)
                {
                    //obtiene el id
                    Match m = Regex.Match(planta_string, @"\d+");
                    int id_planta_int = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_planta_int);

                    sCDM_solicitud.SCDM_rel_solicitud_plantas.Add(new SCDM_rel_solicitud_plantas { id_planta = id_planta_int, id_solicitud = sCDM_solicitud.id });
                    sCDM_solicitud.planta_solicitud = id_planta_int;

                }

            if (SelectedPlantasOrigen != null)
                foreach (string planta_string in SelectedPlantasOrigen)
                {
                    //obtiene el id
                    Match m = Regex.Match(planta_string, @"\d+");
                    int id_planta_int = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_planta_int);

                    //agrega la planta de origen, solo si se trata de extensón
                    if (sCDM_solicitud.id_tipo_solicitud == (int)Bitacoras.Util.SCDMTipoSolicitudENUM.EXTENSION)
                        sCDM_solicitud.planta_origen_extension = id_planta_int;
                }


            #endregion

            #region validacion de archivos
            List<HttpPostedFileBase> archivosForm = new List<HttpPostedFileBase>();
            //agrega archivos enviados
            if (sCDM_solicitud.PostedFileSolicitud_1 != null)
                archivosForm.Add(sCDM_solicitud.PostedFileSolicitud_1);
            if (sCDM_solicitud.PostedFileSolicitud_2 != null)
                archivosForm.Add(sCDM_solicitud.PostedFileSolicitud_2);
            if (sCDM_solicitud.PostedFileSolicitud_3 != null)
                archivosForm.Add(sCDM_solicitud.PostedFileSolicitud_3);

            #region lectura de archivos

            foreach (HttpPostedFileBase httpPostedFileBase in archivosForm)
            {
                //verifica si el tamaño del archivo es OT 1
                if (httpPostedFileBase != null && httpPostedFileBase.InputStream.Length > 10485760)
                    ModelState.AddModelError("", "Sólo se permiten archivos menores a 10MB: " + httpPostedFileBase.FileName + ".");
                else if (httpPostedFileBase != null)
                { //verifica la extensión del archivo
                    string extension = Path.GetExtension(httpPostedFileBase.FileName);
                    if (extension.ToUpper() == ".EXE"   //si contiene una extensión inválida
                                   )
                        ModelState.AddModelError("", "No se permiten archivos con extensión " + extension + ": " + httpPostedFileBase.FileName + ".");
                    else
                    { //si la extension y el tamaño son válidos
                        String nombreArchivo = httpPostedFileBase.FileName;

                        //recorta el nombre del archivo en caso de ser necesario
                        if (nombreArchivo.Length > 80)
                            nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                        //lee el archivo en un arreglo de bytes
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(httpPostedFileBase.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(httpPostedFileBase.ContentLength);
                        }

                        //genera el archivo de biblioce digital
                        biblioteca_digital archivo = new biblioteca_digital
                        {
                            Nombre = Regex.Replace(nombreArchivo, @"[^\w\s.]+", ""),
                            MimeType = UsoStrings.RecortaString(httpPostedFileBase.ContentType, 80),
                            Datos = fileData
                        };
                        sCDM_solicitud.SCDM_rel_solicitud_archivos.Add(new SCDM_rel_solicitud_archivos { biblioteca_digital = archivo, id_solicitud = sCDM_solicitud.id });
                    }
                }
            }


            #endregion


            #endregion
            //validaciones de las listas
            if (SelectedMateriales == null && (sCDM_solicitud.id_tipo_solicitud == 1 || sCDM_solicitud.id_tipo_solicitud == 2))
                ModelState.AddModelError("", "Seleccione los materiales deseados para la solicitud.");

            if (SelectedPlantas == null)
                ModelState.AddModelError("", "Seleccione las plantas para las cuales aplica la solicitud.");

            //si no es cambios
            if (sCDM_solicitud.id_tipo_solicitud != 3)
                sCDM_solicitud.id_tipo_cambio = null;

            //ModelState.AddModelError("", "Error para depuración.");

            if (ModelState.IsValid)
            {
                sCDM_solicitud.fecha_creacion = DateTime.Now;

                try
                {
                    //copia los valores del formulario al modelo
                    solicitudBD.id_tipo_solicitud = sCDM_solicitud.id_tipo_solicitud;
                    solicitudBD.id_tipo_cambio = sCDM_solicitud.id_tipo_cambio;
                    solicitudBD.id_prioridad = sCDM_solicitud.id_prioridad;
                    solicitudBD.descripcion = sCDM_solicitud.descripcion;
                    solicitudBD.justificacion = sCDM_solicitud.justificacion;
                    solicitudBD.planta_solicitud = sCDM_solicitud.planta_solicitud;
                    solicitudBD.planta_origen_extension = sCDM_solicitud.planta_origen_extension;

                    //elimina los rel plantas previos
                    db.SCDM_rel_solicitud_plantas.RemoveRange(db.SCDM_rel_solicitud_plantas.Where(x => x.id_solicitud == sCDM_solicitud.id));
                    //recorre las plantas para saber si es necesario crearlas
                    foreach (var planta in sCDM_solicitud.SCDM_rel_solicitud_plantas)
                        db.SCDM_rel_solicitud_plantas.Add(planta);

                    //elimina los rel materiales previos
                    db.SCDM_rel_solicitud_materiales_solicitados.RemoveRange(db.SCDM_rel_solicitud_materiales_solicitados.Where(x => x.id_solicitud == sCDM_solicitud.id));
                    //recorre las plantas para saber si es necesario crearlas
                    foreach (var material in sCDM_solicitud.SCDM_rel_solicitud_materiales_solicitados)
                        db.SCDM_rel_solicitud_materiales_solicitados.Add(material);

                    //elimina todos rel archivos, exepto los ids que se encuentren en idsArchivos
                    db.SCDM_rel_solicitud_archivos.RemoveRange(db.SCDM_rel_solicitud_archivos.Where(x => x.id_solicitud == sCDM_solicitud.id && !idsArchivos.Contains(x.id_archivo)));
                    //eliminar archivos de biblioteca digital
                    db.biblioteca_digital.RemoveRange(db.SCDM_rel_solicitud_archivos.Where(x => x.id_solicitud == sCDM_solicitud.id && !idsArchivos.Contains(x.id_archivo)).Select(x => x.biblioteca_digital));
                    //creacion de archivos
                    db.SCDM_rel_solicitud_archivos.AddRange(sCDM_solicitud.SCDM_rel_solicitud_archivos);

                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ha editado correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("EditarSolicitud", new { id = sCDM_solicitud.id, viewUser =  sCDM_solicitud.viewUser});
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);

                }
                //Error en la solicitud
                return RedirectToAction("Index");
            }

            sCDM_solicitud.fecha_creacion = DateTime.Now;
            sCDM_solicitud.empleados = solicitante;

            ViewBag.listPlantas = db.plantas.Where(x => x.aplica_solicitud_scdm == true).ToList();
            ViewBag.listTipoMateriales = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList();

            ViewBag.id_prioridad = AddFirstItem(new SelectList(db.SCDM_cat_prioridad.Where(x => x.activo == true), nameof(SCDM_cat_prioridad.id), nameof(SCDM_cat_prioridad.descripcion)));
            ViewBag.id_tipo_cambio = AddFirstItem(new SelectList(db.SCDM_cat_tipo_cambio.Where(x => x.activo == true), nameof(SCDM_cat_tipo_cambio.id), nameof(SCDM_cat_tipo_cambio.descripcion)));
            ViewBag.id_tipo_solicitud = AddFirstItem(new SelectList(db.SCDM_cat_tipo_solicitud.Where(x => x.activo == true), nameof(SCDM_cat_tipo_solicitud.id), nameof(SCDM_cat_tipo_solicitud.descripcion)));

            //manda lios archivos existentes al formulario
            sCDM_solicitud.SCDM_rel_solicitud_archivos = solicitudBD.SCDM_rel_solicitud_archivos;

            return View(sCDM_solicitud);
        }

        // GET: SCDM_solicitud/Edit/5
        public ActionResult EditarSolicitud(int? id, int viewUser = 0)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
                !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES) &&
                !TieneRol(TipoRoles.SCDM_MM_APROBACION_SOLICITUDES) &&
                !TieneRol(TipoRoles.SCDM_MM_APROBACION_INICIAL)
                )
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            empleados solicitante = obtieneEmpleadoLogeado();
            //obtiene el departamento del empleado
            int id_depto_solicitante = solicitante.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault() != null ? solicitante.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault().id_departamento : 99;

            //ERROR GENERICO
            if (
                viewUser == (int)Bitacoras.Util.SCDM_tipo_view_edicionENUM.DEPARTAMENTO_INICIAL
                && sCDM_solicitud.SCDM_solicitud_asignaciones.Last(x => x.fecha_cierre == null && x.fecha_rechazo == null && x.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL).id_empleado != solicitante.id
                && !TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR)
                )
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No puede autorizar esta solicitud!";
                ViewBag.Descripcion = "Sólo la persona asignada a esta solicitud o SCDM pueden aprobar está solicitud.";

                return View("../Home/ErrorGenerico");
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];
            if (TempData["MensajeError"] != null)
                ViewBag.MensajeError = TempData["MensajeError"];

            //obtiene el nombre del empleado al que se enviará para validación Inicial
            string revisaFormato = Bitacoras.Util.MM_revisa_formato_departamento.MM_REVISA_FORMATO_SCDM; //valor por defecto
            int id_departamento = (int)SCDM_departamentos_AsignacionENUM.SCDM;
            //en caso de que sea solitante y no haya más asignaciones
            if ((sCDM_solicitud.id_tipo_solicitud == (int)SCDMTipoSolicitudENUM.CREACION_MATERIALES || sCDM_solicitud.id_tipo_solicitud == (int)SCDMTipoSolicitudENUM.CREACION_REFERENCIA) //CREACION MATERIALES
                && viewUser == (int)SCDM_tipo_view_edicionENUM.SOLICITANTE
                && !sCDM_solicitud.SCDM_solicitud_asignaciones.Any(x => x.id_departamento_asignacion == (int)SCDM_departamentos_AsignacionENUM.SCDM) //cuando no hay asignaciones previas
                )
            {
                switch (solicitante.id_area)
                {
                    //disposicion
                    //case 16: //Shared services RMP & ILL
                    //    revisaFormato = Bitacoras.Util.MM_revisa_formato_departamento.MM_REVISA_FORMATO_DISPOSICION;
                    //    id_departamento = (int)SCDM_departamentos_AsignacionENUM.DISPOSICION; //disposicion
                    //    break;
                    //Engineering
                    //case 12: //Puebla      //solo asigna a ventas en caso de ventas e ingenieria Puebla
                    //case 46: //SLP
                    //case 39: //Silao
                    //ventas
                    case 9: //shared Services
                        revisaFormato = Bitacoras.Util.MM_revisa_formato_departamento.MM_REVISA_FORMATO_VENTAS;
                        id_departamento = (int)SCDM_departamentos_AsignacionENUM.VENTAS; //ventas
                        break;
                    //C&B
                    //case 51: //saltillo
                    //    revisaFormato = Bitacoras.Util.MM_revisa_formato_departamento.MM_REVISA_FORMATO_CB;
                    //    id_departamento = 3; //controlling
                    //    break;
                    default:
                        revisaFormato = Bitacoras.Util.MM_revisa_formato_departamento.MM_REVISA_FORMATO_SCDM;
                        id_departamento = (int)SCDM_departamentos_AsignacionENUM.SCDM; //SCDM
                        break;
                }
            }
            //obtiene los empleados, según las notificaciones de correo en el sistema
            string revisaNobre = string.Empty;

            var revisaCorreo = db.notificaciones_correo.Where(x => x.descripcion == revisaFormato).OrderBy(x => x.id).ToList().Select(x => x.empleados.ConcatNumEmpleadoNombrePlanta).ToList();

            //envia el motivo de rechazo 
            var rechazoAsign = sCDM_solicitud.SCDM_solicitud_asignaciones.LastOrDefault(x => x.fecha_rechazo != null);
            if (rechazoAsign != null)
            {
                sCDM_solicitud.comentario_rechazo = rechazoAsign.comentario_rechazo;
                sCDM_solicitud.comentario_rechazo_departamento = rechazoAsign.comentario_rechazo;
                sCDM_solicitud.id_motivo_rechazo = rechazoAsign.id_motivo_rechazo.Value;
                sCDM_solicitud.id_motivo_rechazo_departamento = rechazoAsign.id_motivo_rechazo.Value;
            }
            //determina las secciones que se muestran dependiendo del tipo de solicitud y tipo de cambio
            var relSeccionesSolicitud = db.SCDM_rel_secciones_por_tipo_solicitud;
            var seccionesDisponibles = db.SCDM_cat_secciones.Where(x => x.activo == true && x.aplica_solicitud == true
                    && relSeccionesSolicitud.Any(y => y.id_seccion == x.id && y.id_tipo_solicitud == sCDM_solicitud.id_tipo_solicitud && y.id_tipo_cambio == sCDM_solicitud.id_tipo_cambio));


            //determina la asignación que abre la solicitud
            ViewBag.idEmpleadoLogeado = solicitante.id;
            ViewBag.revisaNombre = String.Join("<br> ", revisaCorreo);
            ViewBag.revisaFormato = revisaFormato;
            ViewBag.revisaDepartamento = id_departamento;
            ViewBag.secciones = AddFirstItem(new SelectList(seccionesDisponibles, nameof(SCDM_cat_secciones.id), nameof(SCDM_cat_secciones.descripcion)));
            ViewBag.id_motivo_rechazo = AddFirstItem(new SelectList(db.SCDM_cat_motivo_rechazo.Where(x => x.activo == true), nameof(SCDM_cat_motivo_rechazo.id), nameof(SCDM_cat_motivo_rechazo.descripcion)), selected: (rechazoAsign != null ? rechazoAsign.id_motivo_rechazo.ToString() : string.Empty));
            ViewBag.id_motivo_asignacion_incorrecta = AddFirstItem(new SelectList(db.SCDM_cat_motivo_asignacion_incorrecta.Where(x => x.activo == true), nameof(SCDM_cat_motivo_asignacion_incorrecta.id), nameof(SCDM_cat_motivo_asignacion_incorrecta.descripcion)));
            ViewBag.EmpleadoDepartamento = id_depto_solicitante;
            
            ViewBag.SolicitudCerrada = !sCDM_solicitud.SCDM_solicitud_asignaciones.Any(x => x.id_departamento_asignacion == id_depto_solicitante && x.fecha_cierre == null && x.fecha_rechazo == null
                && x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO
            ) && viewUser != (int)Bitacoras.Util.SCDM_tipo_view_edicionENUM.DEPARTAMENTO_INICIAL;


            var deptos = new List<SCDM_cat_departamentos_asignacion> { new SCDM_cat_departamentos_asignacion { descripcion = "Solicitante", id = 99 } };
            deptos.AddRange(db.SCDM_cat_departamentos_asignacion.Where(x => x.activo).ToList());
            ViewBag.ListadoDepartamentos = deptos;


            return View(sCDM_solicitud);
        }

        // GET: SCDM_solicitud/Edit/5
        public ActionResult EditarAsignaciones(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            empleados solicitante = obtieneEmpleadoLogeado();


            return View(sCDM_solicitud);
        }


        // GET: SCDM_solicitud/AsignarTareas/5
        public ActionResult AsignarTareas(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            empleados solicitante = obtieneEmpleadoLogeado();

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //obtiene los departamentos diferentes a SCDM
            var rechazo = sCDM_solicitud.SCDM_solicitud_asignaciones.OrderByDescending(x => x.fecha_rechazo).FirstOrDefault(x => x.fecha_rechazo != null);

            if (rechazo != null)
                sCDM_solicitud.comentario_rechazo = rechazo.comentario_rechazo;

            ViewBag.listDepartamentos = db.SCDM_cat_departamentos_asignacion.Where(x => x.activo && x.id != 9).ToList();
            ViewBag.id_motivo_rechazo = AddFirstItem(new SelectList(db.SCDM_cat_motivo_rechazo.Where(x => x.activo == true), nameof(SCDM_cat_motivo_rechazo.id), nameof(SCDM_cat_motivo_rechazo.descripcion)), selected: rechazo != null ? rechazo.id_motivo_rechazo.ToString() : string.Empty);

            //envia object para gantt


            return View(sCDM_solicitud);
        }

        // POST: SCDM_solicitud/AsignarTareasForm/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.      
        public ActionResult AsignarTareasForm(int? id, List<string[]> dataListFromTable)
        {
            //inicializa la lista de objetos
            var list = new object[1];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            ////  List<SCDM_solicitud_rel_item_material> rollos = ConvierteArrayARollo(dataListFromTable, id);
            if (dataListFromTable == null || dataListFromTable.Count == 0)
            {
                list[0] = new { result = "warning", message = "No se seleccionaron departamentos." };
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            List<EnvioCorreoAsignacionSCDM> listCorreo = new List<EnvioCorreoAsignacionSCDM>();
            List<string> idsDepartamentoRecordatorios = new List<string>();

            var solicitud = db.SCDM_solicitud.Find(id);

            //cierra la asignación actual (scdm)
            var asignacion_scdm = solicitud.SCDM_solicitud_asignaciones.FirstOrDefault(x => x.fecha_rechazo == null && x.fecha_cierre == null
            && x.id_departamento_asignacion == (int)SCDM_departamentos_AsignacionENUM.SCDM);

            var empleado = obtieneEmpleadoLogeado();

            try
            {
                //cierra la solicitud de SCDM
                if (asignacion_scdm != null)
                {
                    asignacion_scdm.fecha_cierre = DateTime.Now;
                    asignacion_scdm.id_cierre = empleado.id;
                }

                //crea las nuevas asignaciones
                List<SCDM_solicitud_asignaciones> asignaciones = new List<SCDM_solicitud_asignaciones>();
                foreach (var array in dataListFromTable)
                {
                    //indica si se debe enviar o no
                    bool enviar = bool.Parse(array[4]);

                    //si no se ha agregado una asignación al departamento
                    if (!asignaciones.Any(x => x.id_departamento_asignacion == Int32.Parse(array[0])) && enviar)
                    {

                        //obtiene el primer comentario
                        string cometarioSCDM = null;
                        string[] lineaComentario = dataListFromTable.FirstOrDefault(x => x[0] == array[0] && !string.IsNullOrEmpty(x[10]) && x[10] != "---");

                        if (lineaComentario != null && !string.IsNullOrEmpty(lineaComentario[10]))
                            cometarioSCDM = lineaComentario[10];

                        //si no existe una asignacion abierta para el departamento
                        if (!solicitud.SCDM_solicitud_asignaciones.Any(x => x.id_cierre == null && x.id_rechazo == null
                            && x.id_departamento_asignacion == Int32.Parse(array[0]))
                        )
                        {

                            asignaciones.Add(new SCDM_solicitud_asignaciones()
                            {
                                id_solicitud = id.Value,
                                id_empleado = Int32.Parse(array[1]),
                                id_departamento_asignacion = Int32.Parse(array[0]),
                                fecha_asignacion = DateTime.Now,
                                descripcion = Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO,
                                comentario_scdm = !String.IsNullOrEmpty(cometarioSCDM) ? cometarioSCDM : null
                            });
                        }
                        else
                        { // si ya existe una asignacion se guarda el depto para manda
                            idsDepartamentoRecordatorios.Add(array[5]);
                        }
                    }

                }
                db.SCDM_solicitud_asignaciones.AddRange(asignaciones);
                //habilita la solicitud en asignaciones 
                solicitud.activo = true;
                db.SaveChanges();

                //obtiene el listado de correos
                foreach (var array in dataListFromTable)
                {
                    int id_departamento = int.Parse(array[0]);
                    string departamento = array[5];
                    string correo = array[8];
                    string tipo = array[9];
                    string comentario = array[10];
                    bool enviar = bool.Parse(array[4]);

                    //si no existe lo crea
                    if (!listCorreo.Any(x => x.departamento == departamento))
                    {
                        listCorreo.Add(new EnvioCorreoAsignacionSCDM { departamento = departamento, comentario = comentario, id_departamento = id_departamento });
                    }
                    //busca el list actual
                    var correoN = listCorreo.FirstOrDefault(x => x.departamento == departamento);

                    //busca si no ya existe el correo en el depto actual
                    if (enviar)
                    {
                        if (tipo == "PRIMARIO" && !correoN.correosTO.Any(x => x == correo))
                            correoN.correosTO.Add(correo);
                        else if (tipo == "SECUNDARIO" && !correoN.correosCC.Any(x => x == correo))
                        {
                            correoN.correosCC.Add(correo);
                        }
                    }
                }

                //obtiene los correos de SCDM
                List<string> correosSCDM = db.SCDM_cat_rel_usuarios_departamentos.Where(x => x.id_departamento == (int)Bitacoras.Util.SCDM_departamentos_AsignacionENUM.SCDM).Select(x => x.empleados.correo).Distinct().ToList();

                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>(); //correos TO

                List<string> nuevosDepartamentosAsignado = new List<string>();

                //envia correos
                foreach (var item in listCorreo)
                {
                    //agrega CC al solicitante
                    if (!string.IsNullOrEmpty(solicitud.empleados.correo))
                        item.correosCC.Add(solicitud.empleados.correo);

                    //agrega los correos de SCDM a CC
                    item.correosCC.AddRange(correosSCDM);

                    if (!idsDepartamentoRecordatorios.Any(x => x == item.departamento))
                    {
                        envioCorreo.SendEmailAsync(item.correosTO, "MDM - Solicitud: " + solicitud.id + " --> Se ha asignado una actividad para " + item.departamento.ToUpper() + ".",
                            envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.ASIGNACION_SOLICITUD_A_DEPARTAMENTO, empleado, solicitud, SCDM_tipo_view_edicionENUM.DEPARTAMENTO, departamento: item.departamento.ToUpper(), comentario: item.comentario)
                            , item.correosCC);
                        nuevosDepartamentosAsignado.Add(item.departamento);
                    }
                    else
                        envioCorreo.SendEmailAsync(item.correosTO, "MDM - Solicitud: " + solicitud.id + " --> 🔔 Recordatorio: Tienes una actividad pendiente; " + item.departamento.ToUpper() + ".",
                            envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.RECORDATORIO, empleado, solicitud, SCDM_tipo_view_edicionENUM.DEPARTAMENTO, departamento: item.departamento.ToUpper(), comentario: item.comentario, id_departamento: item.id_departamento)
                            , item.correosCC);
                }

                //envia notificacion al usuario de asignación a otros departamentos
                if (nuevosDepartamentosAsignado.Count > 0)
                {
                    ////envia correo de notificación al solicitante
                    //envioCorreo.SendEmailAsync(correos, "MDM - Solicitud: " + solicitud.id + " --> Tu solicitud ha sido asignada.",
                    //    envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.NOTIFICACION_A_USUARIO, empleado, solicitud, SCDM_tipo_view_edicionENUM.SOLICITANTE, departamento: String.Join(", ", nuevosDepartamentosAsignado), tipoNotificacionUsuario: SCDM_tipo_correo_notificacionENUM.ASIGNACION_SOLICITUD_A_DEPARTAMENTO));

                }


                list[0] = new { result = "success", message = "Se asignó correctamente la solicitud" };
            }
            catch (Exception e)
            {
                list[0] = new { result = "error", message = e.Message };
            }


            return Json(list, JsonRequestBehavior.AllowGet);
        }


        // POST: SCDM_solicitud/EnviaSolicitudInicio/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnviaSolicitudInicio(int? id, int? revisaDepartamento, string revisaFormato, string viewUser)
        {
            //ejemplo de Error al Enviar la solitud
            bool isValid = true;

            //obtiene el id_empleado según 
            var revisaCorreo = db.notificaciones_correo.Where(x => x.descripcion == revisaFormato);

            //actualiza la asignacion del usuario en caso de existir
            var solicitud = db.SCDM_solicitud.Find(id);

            #region Verifica Solicitud Vacia

            string mensajeError = string.Empty;

            switch (solicitud.id_tipo_solicitud)
            {
                //Creacion de Materiales
                case (int)Bitacoras.Util.SCDMTipoSolicitudENUM.CREACION_MATERIALES:

                    foreach (var item in solicitud.SCDM_rel_solicitud_materiales_solicitados)
                    {

                        if (!solicitud.SCDM_solicitud_rel_item_material.Any(x => x.id_tipo_material == item.id_tipo_material))
                        {
                            mensajeError = "Ingrese los componentes para " + item.SCDM_cat_tipo_materiales_solicitud.descripcion + ". ";
                            isValid = false;
                        }//valida la lista tecnica en caso de platina, ps o shearing
                        else if ((item.id_tipo_material == SCDM_solicitud_rel_item_material_tipo.PLATINA
                            || item.id_tipo_material == SCDM_solicitud_rel_item_material_tipo.PLATINA_SOLDADA
                            || item.id_tipo_material == SCDM_solicitud_rel_item_material_tipo.SHEARING
                            ) && !solicitud.SCDM_solicitud_rel_lista_tecnica.Any()
                            )
                        {
                            mensajeError = "En caso de platina, platina soldada o shearing. La Lista Ténica es obligatoria.";
                            isValid = false;
                        }


                    }
                    break;
                //Creacion de Materiales con referencia
                case (int)Bitacoras.Util.SCDMTipoSolicitudENUM.CREACION_REFERENCIA:
                    if (!solicitud.SCDM_solicitud_rel_creacion_referencia.Any())
                    {
                        mensajeError = "Ingrese los componentes para los materiales para la creación de referencia.";
                        isValid = false;
                    }
                    break;
                //Cambios
                case (int)Bitacoras.Util.SCDMTipoSolicitudENUM.CAMBIOS:
                    //valida según el tipo de cambio
                    switch (solicitud.id_tipo_cambio)
                    {
                        case (int)Bitacoras.Util.SCDMTipoCambioENUM.ACTIVAR_MATERIAL:
                        case (int)Bitacoras.Util.SCDMTipoCambioENUM.CAMBIOS_OBSOLETAR_MATERIALES:
                            if (!solicitud.SCDM_solicitud_rel_activaciones.Any())
                            {
                                mensajeError = "Ingrese los materiales para cambio de estatus.";
                                isValid = false;
                            }
                            break;
                        case (int)Bitacoras.Util.SCDMTipoCambioENUM.CAMBIOS_ORDEN_COMPRA:
                            if (!solicitud.SCDM_solicitud_rel_orden_compra.Any())
                            {
                                mensajeError = "Ingrese los datos para la orden de compra.";
                                isValid = false;
                            }
                            break;
                        case (int)Bitacoras.Util.SCDMTipoCambioENUM.CAMBIOS_INGENIERIA:
                            if (!solicitud.SCDM_solicitud_rel_cambio_ingenieria.Any())
                            {
                                mensajeError = "Ingrese los materiales para el cambio de ingeniería.";
                                isValid = false;
                            }
                            break;
                        case (int)Bitacoras.Util.SCDMTipoCambioENUM.CAMBIOS_LISTA_TECNICA:
                            if (!solicitud.SCDM_solicitud_rel_lista_tecnica.Any())
                            {
                                mensajeError = "Ingrese los materiales para lista técnica.";
                                isValid = false;
                            }
                            break;
                        case (int)Bitacoras.Util.SCDMTipoCambioENUM.CAMBIOS_DATOS_BUDGET:
                            if (!solicitud.SCDM_solicitud_rel_cambio_budget.Any())
                            {
                                mensajeError = "Ingrese los materiales para el cambio de datos de Budget.";
                                isValid = false;
                            }
                            break;

                    }
                    break;
                //Creacion de Servicios
                case (int)Bitacoras.Util.SCDMTipoSolicitudENUM.CREACION_SERVICIOS:
                    if (!solicitud.SCDM_solicitud_rel_orden_compra.Any())
                    {
                        mensajeError = "Ingrese los datos para la creación de servicios.";
                        isValid = false;
                    }
                    break;
                //Extensión
                case (int)Bitacoras.Util.SCDMTipoSolicitudENUM.EXTENSION:
                    if (!solicitud.SCDM_solicitud_rel_extension_usuario.Any())
                    {
                        mensajeError = "Ingrese los materiales a Extender.";
                        isValid = false;
                    }
                    break;
                //c&b
                case (int)Bitacoras.Util.SCDMTipoSolicitudENUM.CREACION_CB:
                    if (!solicitud.SCDM_solicitud_rel_item_material.Any(x => x.id_tipo_material == Bitacoras.Util.SCDM_solicitud_rel_item_material_tipo.C_B))
                    {
                        mensajeError = "Ingrese los materiales para C&B.";
                        isValid = false;
                    }
                    break;
            }

            if (!isValid)
            {
                TempData["MensajeError"] = "La solicitud no se envío. " + mensajeError;
                //TempData["Mensaje"] = new MensajesSweetAlert("La solicitud no se envío. "+ mensajeError, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("EditarSolicitud", new { id = id, viewUser = viewUser });
            }

            #endregion


            DateTime fechaActual = DateTime.Now;
            var empleado = obtieneEmpleadoLogeado();
            var idDepartamento = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault() != null ? empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault().id_departamento : 99;
            var asignacionAnterior = solicitud.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_departamento_asignacion == idDepartamento && (x.fecha_cierre == null && x.fecha_rechazo == null) && x.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE);

            if (asignacionAnterior != null)
            {
                asignacionAnterior.fecha_cierre = fechaActual;
                asignacionAnterior.id_cierre = empleado.id;
            }

            //crea una asignacion para SCDM o Asignación inicial
            var asignacion = new SCDM_solicitud_asignaciones()
            {
                id_solicitud = id.Value,
                id_empleado = revisaCorreo.FirstOrDefault().empleados.id,
                id_departamento_asignacion = revisaDepartamento.Value,
                fecha_asignacion = fechaActual,
                descripcion = revisaDepartamento.Value == (int)SCDM_departamentos_AsignacionENUM.SCDM ? Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM : Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL,
            };

            try
            {
                //solamente agrega la asiganacion, en caso de no hay ya una asignación abierta a SCDM
                if (!solicitud.SCDM_solicitud_asignaciones.Any(x => x.id_departamento_asignacion == (int)SCDM_departamentos_AsignacionENUM.SCDM && x.fecha_cierre == null && x.fecha_rechazo == null))
                    db.SCDM_solicitud_asignaciones.Add(asignacion);

                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha enviado la solicitud correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>(); //correos TO
                foreach (var item in revisaCorreo)
                    correos.Add(item.empleados.correo);

                SCDM_tipo_view_edicionENUM vista = SCDM_tipo_view_edicionENUM.SOLICITANTE;

                if (MM_revisa_formato_departamento.MM_REVISA_FORMATO_SCDM == revisaFormato)
                    vista = SCDM_tipo_view_edicionENUM.SCDM;
                else
                    vista = SCDM_tipo_view_edicionENUM.DEPARTAMENTO_INICIAL;

                envioCorreo.SendEmailAsync(correos, "MDM - Solicitud: " + solicitud.id + " --> Se te ha asignado una actividad. ", envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.ENVIA_SOLICITUD, empleado, solicitud, vista));

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);
            }

            return RedirectToAction("Index");
        }

        // POST: SCDM_solicitud/ApruebaSolicitudDepartamento/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApruebaSolicitudDepartamento(int? id, int? revisaDepartamento, string revisaFormato, int viewUser = 0)
        {
            //encuentra la asignacion correspondiente
            SCDM_solicitud sCDM_solicitudBD = db.SCDM_solicitud.Find(id);
            var empleado = obtieneEmpleadoLogeado();
            var idDepartamento = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault() != null ? empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault().id_departamento : 99;
            var id_asignación = sCDM_solicitudBD.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_departamento_asignacion == idDepartamento
                && (x.fecha_cierre == null && x.fecha_rechazo == null)
                && ((viewUser == (int)SCDM_tipo_view_edicionENUM.DEPARTAMENTO && x.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO)
                || (viewUser == (int)SCDM_tipo_view_edicionENUM.DEPARTAMENTO_INICIAL && x.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL)
                )
            ).id;

            //var solicitudesPendientesNum = sCDM_solicitudBD.SCDM_solicitud_asignaciones.Where(x => x.fecha_cierre == null && x.fecha_rechazo == null).Count();
            DateTime fechaActual = DateTime.Now;

            var asignacionAnterior = db.SCDM_solicitud_asignaciones.Find(id_asignación);
            asignacionAnterior.fecha_cierre = fechaActual;
            asignacionAnterior.id_cierre = empleado.id;

            //obtiene el id_empleado según 
            var revisaCorreo = db.notificaciones_correo.Where(x => x.descripcion == revisaFormato);

            //si no existe lo agrega
            SCDM_solicitud_asignaciones asignacion = null;

            if (!sCDM_solicitudBD.SCDM_solicitud_asignaciones.Any(x => x.fecha_cierre == null && x.fecha_rechazo == null
                && x.id_departamento_asignacion == (int)SCDM_departamentos_AsignacionENUM.SCDM))
            {
                // si no hay mas asignaciones a otros departamentos ni al solicitante
                if (!sCDM_solicitudBD.SCDM_solicitud_asignaciones.Any(x => x.fecha_cierre == null && x.fecha_rechazo == null && (x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO || x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE)))
                    asignacion = new SCDM_solicitud_asignaciones()
                    {
                        id_solicitud = id.Value,
                        id_empleado = revisaCorreo.FirstOrDefault().empleados.id,
                        id_departamento_asignacion = revisaDepartamento.Value,
                        fecha_asignacion = fechaActual,
                        descripcion = revisaDepartamento.Value == (int)SCDM_departamentos_AsignacionENUM.SCDM ? Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM : Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL,
                    };
            }

            try
            {
                if (asignacion != null)
                    db.SCDM_solicitud_asignaciones.Add(asignacion);

                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha aprobado la solicitud correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>(); //correos TO
                foreach (var item in revisaCorreo)
                    correos.Add(item.empleados.correo);

                List<string> correosCC = new List<string>();
                if (!string.IsNullOrEmpty(empleado.correo))
                    correosCC.Add(empleado.correo);

                SCDM_tipo_correo_notificacionENUM notificacionUsuario = SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_INICIAL;

                //aprueba solicitud Revisión inicial 
                if (asignacionAnterior.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL)
                {
                    envioCorreo.SendEmailAsync(correos, "MDM - Solicitud: " + sCDM_solicitudBD.id + " --> Se te ha asignado una actividad.", envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_INICIAL, empleado, sCDM_solicitudBD, SCDM_tipo_view_edicionENUM.SCDM), emailsCC: correosCC);
                    notificacionUsuario = SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_INICIAL;
                }
                //aprueba solicitud Departamento y hay departamentos pendientes
                if (asignacionAnterior.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO
                    && sCDM_solicitudBD.SCDM_solicitud_asignaciones.Any(x => x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO && x.fecha_cierre == null && x.fecha_rechazo == null))
                {
                    envioCorreo.SendEmailAsync(correos, "MDM - Solicitud: " + sCDM_solicitudBD.id + " --> El departamento " + asignacionAnterior.SCDM_cat_departamentos_asignacion.descripcion.ToUpper() + ", ha finalizado una actividad.",
                        envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_DEPARTAMENTO_PENDIENTES, empleado, sCDM_solicitudBD, SCDM_tipo_view_edicionENUM.SCDM, departamento: asignacionAnterior.SCDM_cat_departamentos_asignacion.descripcion.ToUpper()), emailsCC: correosCC);
                    notificacionUsuario = SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_DEPARTAMENTO_PENDIENTES;
                }

                //aprueba solicitud y ya NO hay departamentos pendientes
                if (asignacionAnterior.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO
                    && !sCDM_solicitudBD.SCDM_solicitud_asignaciones.Any(x => x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO && x.fecha_cierre == null && x.fecha_rechazo == null)
                    )
                {
                    envioCorreo.SendEmailAsync(correos, "MDM - Solicitud: " + sCDM_solicitudBD.id + " --> El departamento " + asignacionAnterior.SCDM_cat_departamentos_asignacion.descripcion.ToUpper() + ", ha finalizado una actividad.",
                        envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_DEPARTAMENTO_FINAL, empleado, sCDM_solicitudBD, SCDM_tipo_view_edicionENUM.SCDM, departamento: asignacionAnterior.SCDM_cat_departamentos_asignacion.descripcion.ToUpper()), emailsCC: correosCC);
                    notificacionUsuario = SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_DEPARTAMENTO_FINAL;
                }

                //Aviso para Solicitante
                correos = new List<string> //correos TO
                {
                    sCDM_solicitudBD.empleados.correo
                };

                //envia correo de notificación al solicitante
                envioCorreo.SendEmailAsync(correos, "MDM - Solicitud: " + sCDM_solicitudBD.id + " --> Se ha registrado un cambio en tu solicitud.",
                    envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.NOTIFICACION_A_USUARIO, empleado, sCDM_solicitudBD, SCDM_tipo_view_edicionENUM.SOLICITANTE, departamento: asignacionAnterior.SCDM_cat_departamentos_asignacion.descripcion.ToUpper(), tipoNotificacionUsuario: notificacionUsuario));

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);
            }

            if (viewUser == (int)SCDM_tipo_view_edicionENUM.DEPARTAMENTO_INICIAL)
                return RedirectToAction("SolicitudesRevisionInicial");
            else
                return RedirectToAction("SolicitudesDepartamento");

        }

        public ActionResult EditRollo(int? id)
        {

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
              !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //Viewbag para dropdowns

            // Register List of Languages     

            ViewBag.TipoVentaArray = db.SCDM_cat_tipo_venta.Where(x => x.activo).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ClientesArray = db.clientes.Where(x => x.activo == true).ToList().Select(x => x.ConcatClienteSAP.Trim()).ToArray();
            ViewBag.ProveedoresArray = db.proveedores.Where(x => x.activo == true).ToList().Select(x => x.ConcatproveedoresAP.Trim()).ToArray();
            ViewBag.MolinosArray = db.SCDM_cat_molino.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TipoMetalArray = db.SCDM_cat_tipo_metal.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.UnidadMedidaArray = db.SCDM_cat_unidades_medida.Where(x => x.activo == true && (x.codigo == "KG" || x.codigo == "LB")).ToList().Select(x => x.codigo.Trim()).ToArray();
            ViewBag.TipoMaterialArray = db.SCDM_cat_tipo_recubrimiento.Where(x => x.activo == true).ToList().Select(x => x.ConcatRecubrimiento.Trim()).ToArray();
            ViewBag.TipoAprovisionamientoArray = db.SCDM_cat_clase_aprovisionamiento.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.PesoRecubrimientoArray = db.SCDM_cat_peso_recubrimiento.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TipoIntExtArray = db.SCDM_cat_parte_interior_exterior.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.PosicionRolloArray = db.SCDM_cat_posicion_rollo_embarques.Where(x => x.activo == true).ToList().Select(x => x.ConcatPosicion.Trim()).ToArray();
            ViewBag.IHSArray = db.SCDM_cat_ihs.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ModeloNegocioArray = db.SCDM_cat_modelo_negocio.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TransitoArray = db.SCDM_cat_tipo_transito.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.DisponentesArray = db.SCDM_cat_disponentes.Where(x => x.activo == true).ToList().Select(x => x.empleados.ConcatNumEmpleadoNombre.Trim()).ToArray();
            ViewBag.DiametroInteriorArray = db.SCDM_cat_diametro_interior.Where(x => x.activo == true).ToList().Select(x => x.valor.ToString()).ToArray();
            ViewBag.GradoCalidadArray = db.SCDM_cat_grado_calidad.Where(x => x.activo == true).ToList().Select(x => x.grado_calidad).ToArray();

            return View(sCDM_solicitud);
        }

        public ActionResult EditCintas(int? id)
        {

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
              !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //Viewbag para dropdowns

            // Register List of Languages     

            ViewBag.TipoVentaArray = db.SCDM_cat_tipo_venta.Where(x => x.activo).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ClientesArray = db.clientes.Where(x => x.activo == true).ToList().Select(x => x.ConcatClienteSAP.Trim()).ToArray();
            ViewBag.ProveedoresArray = db.proveedores.Where(x => x.activo == true).ToList().Select(x => x.ConcatproveedoresAP.Trim()).ToArray();
            ViewBag.MolinosArray = db.SCDM_cat_molino.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TipoMetalArray = db.SCDM_cat_tipo_metal.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.UnidadMedidaArray = db.SCDM_cat_unidades_medida.Where(x => x.activo == true).ToList().Select(x => x.codigo.Trim()).ToArray();
            ViewBag.TipoMaterialArray = db.SCDM_cat_tipo_recubrimiento.Where(x => x.activo == true).ToList().Select(x => x.ConcatRecubrimiento.Trim()).ToArray();
            ViewBag.TipoAprovisionamientoArray = db.SCDM_cat_clase_aprovisionamiento.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.PesoRecubrimientoArray = db.SCDM_cat_peso_recubrimiento.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TipoIntExtArray = db.SCDM_cat_parte_interior_exterior.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.PosicionRolloArray = db.SCDM_cat_posicion_rollo_embarques.Where(x => x.activo == true).ToList().Select(x => x.ConcatPosicion.Trim()).ToArray();
            ViewBag.IHSArray = db.SCDM_cat_ihs.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ModeloNegocioArray = db.SCDM_cat_modelo_negocio.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TransitoArray = db.SCDM_cat_tipo_transito.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.DisponentesArray = db.SCDM_cat_disponentes.Where(x => x.activo == true).ToList().Select(x => x.empleados.ConcatNumEmpleadoNombre.Trim()).ToArray();
            ViewBag.DiametroInteriorArray = db.SCDM_cat_diametro_interior.Where(x => x.activo == true).ToList().Select(x => x.valor.ToString()).ToArray();
            ViewBag.GradoCalidadArray = db.SCDM_cat_grado_calidad.Where(x => x.activo == true).ToList().Select(x => x.grado_calidad).ToArray();

            return View(sCDM_solicitud);
        }

        public ActionResult EditCopperAndBrass(int? id)
        {

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
              !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //Viewbag para dropdowns          

            ViewBag.TipoVentaArray = db.SCDM_cat_tipo_venta.Where(x => x.activo).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ClientesArray = db.clientes.Where(x => x.activo == true).ToList().Select(x => x.ConcatClienteSAP.Trim()).ToArray();
            ViewBag.ProveedoresArray = db.proveedores.Where(x => x.activo == true).ToList().Select(x => x.ConcatproveedoresAP.Trim()).ToArray();
            ViewBag.MolinosArray = db.SCDM_cat_molino.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ClaseMaterialArray = db.SCDM_cat_materia_prima_producto_terminado.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.UnidadMedidaArray = db.SCDM_cat_unidades_medida.Where(x => x.activo == true).ToList().Select(x => x.codigo.Trim()).ToArray();
            ViewBag.TipoMetalArray = db.SCDM_cat_tipo_metal_cb.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TipoAprovisionamientoArray = db.SCDM_cat_clase_aprovisionamiento.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.AleacionArray = db.SCDM_cat_aleacion.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ModeloNegocioArray = db.SCDM_cat_modelo_negocio.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TransitoArray = db.SCDM_cat_tipo_transito.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.MonedaArray = db.SCDM_cat_moneda.Where(x => x.activo == true).ToList().Select(x => x.clave_iso.Trim()).ToArray();
            ViewBag.IncotermArray = db.SCDM_cat_incoterm.Where(x => x.activo == true).ToList().Select(x => x.ConcatCodigoDescripcion.Trim()).ToArray();
            ViewBag.TerminosPagoArray = db.SCDM_cat_terminos_pago.Where(x => x.activo == true).ToList().Select(x => x.ConcatCodigoDescripcion.Trim()).ToArray();
            ViewBag.GradoCalidadArray = db.SCDM_cat_grado_calidad.Where(x => x.activo == true).ToList().Select(x => x.grado_calidad).ToArray();

            return View(sCDM_solicitud);
        }


        public ActionResult EditPlatinas(int? id, int tipoPlatina = 0)
        {

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
              !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //Lista para claves permitidas
            List<string> listClavesFormas = new List<string> {
                "2", //rectangular              
                "18", //configurado 
                "19"  //recto
            };

            //agrega trapezoide si no se trata de shearing
            if (tipoPlatina != Bitacoras.Util.SCDM_solicitud_rel_item_material_tipo.SHEARING)
                listClavesFormas.Add("3");  //trapezoide)

            // Register List of Languages     
            var listFormaBD = db.SCDM_cat_forma_material.Where(x => x.activo == true
                && listClavesFormas.Contains(x.clave)
            ).ToList();


            ViewBag.TipoVentaArray = db.SCDM_cat_tipo_venta.Where(x => x.activo).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ClientesArray = db.clientes.Where(x => x.activo == true).ToList().Select(x => x.ConcatClienteSAP.Trim()).ToArray();
            ViewBag.ProveedoresArray = db.proveedores.Where(x => x.activo == true).ToList().Select(x => x.ConcatproveedoresAP.Trim()).ToArray();
            ViewBag.MolinosArray = db.SCDM_cat_molino.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TipoMetalArray = db.SCDM_cat_tipo_metal.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.UnidadMedidaArray = db.SCDM_cat_unidades_medida.Where(x => x.activo == true && (x.codigo == "PC")).ToList().Select(x => x.codigo.Trim()).ToArray();
            ViewBag.TipoMaterialArray = db.SCDM_cat_tipo_recubrimiento.Where(x => x.activo == true).ToList().Select(x => x.ConcatRecubrimiento.Trim()).ToArray();
            ViewBag.TipoAprovisionamientoArray = db.SCDM_cat_clase_aprovisionamiento.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.PesoRecubrimientoArray = db.SCDM_cat_peso_recubrimiento.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TipoIntExtArray = db.SCDM_cat_parte_interior_exterior.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.IHSArray = db.SCDM_cat_ihs.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ModeloNegocioArray = db.SCDM_cat_modelo_negocio.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TransitoArray = db.SCDM_cat_tipo_transito.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.FormaArray = listFormaBD.Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.GradoCalidadArray = db.SCDM_cat_grado_calidad.Where(x => x.activo == true).ToList().Select(x => x.grado_calidad).ToArray();

            ViewBag.TipoPlatina = tipoPlatina;

            return View(sCDM_solicitud);
        }



        public ActionResult EditCreacionReferencia(int? id)
        {

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
             !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //Viewbag para dropdowns
            List<int> idPlantas = sCDM_solicitud.SCDM_rel_solicitud_plantas.Select(x => x.id_planta).ToList();


            ViewBag.TipoVentaArray = db.SCDM_cat_tipo_venta.Where(x => x.activo).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TipoMaterialArray = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.PlantaArray = db.plantas.Where(x => x.activo == true && idPlantas.Contains(x.clave)).ToList().Select(x => x.ConcatPlantaSap.Trim()).ToArray();
            ViewBag.MotivoCreacionArray = db.SCDM_cat_motivo_creacion.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.UnidadMedidaArray = db.SCDM_cat_unidades_medida.Where(x => x.activo == true).ToList().Select(x => x.codigo.Trim()).ToArray();
            ViewBag.CommodityArray = db.SCDM_cat_commodity.Where(x => x.activo == true).ToList().Select(x => x.ConcatCommodity.Trim()).ToArray();
            ViewBag.GradoCalidadArray = db.SCDM_cat_grado_calidad.Where(x => x.activo == true).ToList().Select(x => x.grado_calidad).ToArray();
            ViewBag.SuperficieArray = db.SCDM_cat_superficie.Where(x => x.activo == true).ToList().Select(x => x.descripcion).ToArray();
            ViewBag.TratamientoSuperficialArray = db.SCDM_cat_tratamiento_superficial.Where(x => x.activo == true).ToList().Select(x => x.descripcion).ToArray();
            ViewBag.PesoRecubrimientoArray = db.SCDM_cat_peso_recubrimiento.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.MolinosArray = db.SCDM_cat_molino.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.FormaArray = db.SCDM_cat_forma_material.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ClientesArray = db.clientes.Where(x => x.activo == true).ToList().Select(x => x.ConcatClienteSAP.Trim()).ToArray();
            ViewBag.DiametroInteriorArray = db.SCDM_cat_diametro_interior.Where(x => x.activo == true).ToList().Select(x => x.valor.ToString()).ToArray();

            //tipo de metal
            List<string> tipoMetal = db.SCDM_cat_tipo_metal.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToList();
            tipoMetal.AddRange(db.SCDM_cat_tipo_metal_cb.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToList());
            tipoMetal = tipoMetal.Distinct().ToList();
            ViewBag.TipoMetalArray = tipoMetal.ToArray();


            return View(sCDM_solicitud);
        }

        public ActionResult EditCambioIngenieria(int? id)
        {

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
             !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //Viewbag para dropdowns
            List<int> idPlantas = sCDM_solicitud.SCDM_rel_solicitud_plantas.Select(x => x.id_planta).ToList();

            ViewBag.TipoVentaArray = db.SCDM_cat_tipo_venta.Where(x => x.activo).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TipoMaterialArray = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.PlantaArray = db.plantas.Where(x => x.activo == true && idPlantas.Contains(x.clave)).ToList().Select(x => x.ConcatPlantaSap.Trim()).ToArray();
            ViewBag.MotivoCreacionArray = db.SCDM_cat_motivo_creacion.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.UnidadMedidaArray = db.SCDM_cat_unidades_medida.Where(x => x.activo == true).ToList().Select(x => x.codigo.Trim()).ToArray();
            ViewBag.CommodityArray = db.SCDM_cat_commodity.Where(x => x.activo == true).ToList().Select(x => x.ConcatCommodity.Trim()).ToArray();
            ViewBag.GradoCalidadArray = db.SCDM_cat_grado_calidad.Where(x => x.activo == true).ToList().Select(x => x.grado_calidad).ToArray();
            ViewBag.SuperficieArray = db.SCDM_cat_superficie.Where(x => x.activo == true).ToList().Select(x => x.descripcion).ToArray();
            ViewBag.TratamientoSuperficialArray = db.SCDM_cat_tratamiento_superficial.Where(x => x.activo == true).ToList().Select(x => x.descripcion).ToArray();
            ViewBag.PesoRecubrimientoArray = db.SCDM_cat_peso_recubrimiento.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.MolinosArray = db.SCDM_cat_molino.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.FormaArray = db.SCDM_cat_forma_material.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ClientesArray = db.clientes.Where(x => x.activo == true).ToList().Select(x => x.ConcatClienteSAP.Trim()).ToArray();
            ViewBag.DiametroInteriorArray = db.SCDM_cat_diametro_interior.Where(x => x.activo == true).ToList().Select(x => x.valor.ToString()).ToArray();
            //tipo de metal
            List<string> tipoMetal = db.SCDM_cat_tipo_metal.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToList();
            tipoMetal.AddRange(db.SCDM_cat_tipo_metal_cb.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToList());
            tipoMetal = tipoMetal.Distinct().ToList();
            ViewBag.TipoMetalArray = tipoMetal.ToArray();

            return View(sCDM_solicitud);
        }
        public ActionResult EditBudget(int? id)
        {

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
             !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //Viewbag para dropdowns
            List<int> idPlantas = sCDM_solicitud.SCDM_rel_solicitud_plantas.Select(x => x.id_planta).ToList();

            ViewBag.TipoVentaArray = db.SCDM_cat_tipo_venta.Where(x => x.activo).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TipoMaterialArray = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.PosicionRolloArray = db.SCDM_cat_posicion_rollo_embarques.Where(x => x.activo == true).ToList().Select(x => x.ConcatPosicion.Trim()).ToArray();
            ViewBag.IHSArray = db.SCDM_cat_ihs.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ModeloNegocioArray = db.SCDM_cat_modelo_negocio.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();

            //tipo de metal
            List<string> tipoMetal = db.SCDM_cat_tipo_metal.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToList();
            tipoMetal.AddRange(db.SCDM_cat_tipo_metal_cb.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToList());
            tipoMetal = tipoMetal.Distinct().ToList();
            ViewBag.TipoMetalArray = tipoMetal.ToArray();

            return View(sCDM_solicitud);
        }

        public ActionResult EditCambioEstatus(int? id)
        {

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
             !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //Viewbag para dropdowns
            List<int> idPlantas = sCDM_solicitud.SCDM_rel_solicitud_plantas.Select(x => x.id_planta).ToList();

            ViewBag.PlantaArray = db.plantas.Where(x => x.activo == true && idPlantas.Contains(x.clave)).ToList().Select(x => x.ConcatPlantaSap.Trim()).ToArray();

            return View(sCDM_solicitud);
        }
        public ActionResult EditExtension(int? id)
        {

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
             !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //Viewbag para dropdowns
            List<int> idPlantas = sCDM_solicitud.SCDM_rel_solicitud_plantas.Select(x => x.id_planta).ToList();

            ViewBag.PlantaArray = db.plantas.Where(x => x.activo == true && idPlantas.Contains(x.clave)).ToList().Select(x => x.ConcatPlantaSap.Trim()).ToArray();
            ViewBag.listAmacenesVirtuales = db.SCDM_cat_almacenes.Where(x => x.es_virtual == true).Select(x => x.warehouse).Distinct().ToList();

            return View(sCDM_solicitud);
        }

        public ActionResult EditFormatoCompra(int? id)
        {

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
             !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            string acopioExtAprovisonamiento = db.SCDM_cat_clase_aprovisionamiento.Find(1).descripcion;

            //determina si hay elementos que cargar
            var materialesCount = db.SCDM_solicitud_rel_item_material.Where(x => x.id_solicitud == id
            && x.clase_aprovisionamiento == acopioExtAprovisonamiento //acopio externo
             ).Count();

            var materialesReferenciaCount = db.SCDM_solicitud_rel_creacion_referencia.Where(x => x.id_solicitud == id).Count();

            ViewBag.MaterialesCount = materialesCount + materialesReferenciaCount;

            //agrega TO
            var medidasArray = db.SCDM_cat_unidades_medida.Where(x => x.activo == true).ToList().Select(x => x.codigo.Trim()).ToArray();
            medidasArray = medidasArray.Append("TO").ToArray();

            //Viewbag para dropdowns
            ViewBag.TipoOCArray = db.SCDM_cat_po_existente.Where(x => x.activo).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ProveedoresArray = db.proveedores.Where(x => x.activo == true).ToList().Select(x => x.ConcatproveedoresAP.Trim()).ToArray();
            ViewBag.CentroReciboArray = db.SCDM_cat_centro_recibo.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.IncotermArray = db.SCDM_cat_incoterm.Where(x => x.activo == true).ToList().Select(x => x.ConcatCodigoDescripcion.Trim()).ToArray();
            ViewBag.FronteraPuertoArray = db.SCDM_cat_frontera_puerto_planta.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.CondicionesPagoArray = db.SCDM_cat_po_condiciones_pago.Where(x => x.activo == true).ToList().Select(x => x.ConcatCodigoDescripcion.Trim()).ToArray();
            ViewBag.TransporteArray = db.SCDM_cat_po_transporte.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.MonedaArray = db.SCDM_cat_moneda.Where(x => x.activo == true).ToList().Select(x => x.clave_iso.Trim()).ToArray();
            ViewBag.UnidadMedidaArray = medidasArray;
            ViewBag.TipoVentaArray = db.SCDM_cat_tipo_venta.Where(x => x.activo).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.MolinosArray = db.SCDM_cat_molino.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();

            return View(sCDM_solicitud);
        }
        public ActionResult EditListaTecnica(int? id)
        {

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
             !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //Viewbag para dropdowns
            var materialesList = db.SCDM_solicitud_rel_item_material.Where(x => x.id_solicitud == id).ToList().Select(x => x.numero_material.Trim()).ToArray();
            var creacionReferenciaList = db.SCDM_solicitud_rel_creacion_referencia.Where(x => x.id_solicitud == id).ToList().Select(x => x.nuevo_material.Trim()).ToArray();


            ViewBag.MaterialesSolicitudArray = materialesList.Concat(creacionReferenciaList).ToArray();

            return View(sCDM_solicitud);
        }

        public ActionResult EditFacturacion(int? id)
        {

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
             !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];


            return View(sCDM_solicitud);
        }


        // POST: SCDM_solicitud/AddSeccion/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSeccion(int? id, int? secciones, string viewUser)
        {
            if (ModelState.IsValid)
            {
                var rel = new SCDM_rel_solicitud_secciones_activas()
                {
                    id_solicitud = id.Value,
                    id_seccion = secciones.Value,
                };


                try
                {

                    if (db.SCDM_rel_solicitud_secciones_activas.Any(x => x.id_solicitud == id && x.id_seccion == secciones))
                    {
                        TempData["Mensaje"] = new MensajesSweetAlert("La sección ya se encuentra agregada a la solicitud.", TipoMensajesSweetAlerts.WARNING);
                    }
                    else
                    {
                        db.SCDM_rel_solicitud_secciones_activas.Add(rel);
                        db.SaveChanges();
                        TempData["Mensaje"] = new MensajesSweetAlert("Se ha agregado la sección correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                    }
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);

                }
            }

            return RedirectToAction("EditarSolicitud", new { id = id, viewUser = viewUser });
        }

        // GET: SCDM_solicitud/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }
            return View(sCDM_solicitud);
        }
        /*

        // POST: SCDM_solicitud/Delete/5
        [HttpPost, ActionName("RechazarSolicitud")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarSolicitud(SCDM_solicitud solicitud)
        {
            SCDM_solicitud sCDM_solicitudBD = db.SCDM_solicitud.Find(solicitud.id);

            //encuentra la asignacion correspondiente
            var empleado = obtieneEmpleadoLogeado();
            var idDepartamento = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault(x => x.id_empleado == empleado.id).id_departamento;

            var asignacion = sCDM_solicitudBD.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_departamento_asignacion == idDepartamento && (x.fecha_cierre == null && x.fecha_rechazo == null));
            //encaso de que haya una asignación activa
            if (asignacion != null)
            {
                var id_asignación = asignacion.id;

                asignacion.fecha_rechazo = DateTime.Now;
                asignacion.comentario_rechazo = solicitud.comentario_rechazo;
                asignacion.id_motivo_rechazo = solicitud.id_motivo_rechazo;
                asignacion.id_rechazo = empleado.id;
            }
            else
            { //crea una de scdm 
                db.SCDM_solicitud_asignaciones.Add(new SCDM_solicitud_asignaciones
                {
                    id_solicitud = solicitud.id,
                    id_departamento_asignacion = (int)SCDM_departamentos_AsignacionENUM.SCDM,
                    id_empleado = 447, //AXC
                    fecha_asignacion = sCDM_solicitudBD.SCDM_solicitud_asignaciones.LastOrDefault(x => x.fecha_rechazo != null) != null ?
                    sCDM_solicitudBD.SCDM_solicitud_asignaciones.LastOrDefault(x => x.fecha_rechazo != null).fecha_rechazo.Value : DateTime.Now,
                    fecha_rechazo = DateTime.Now,
                    descripcion = Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM,
                    comentario_rechazo = solicitud.comentario_rechazo,
                    id_motivo_rechazo = solicitud.id_motivo_rechazo,
                    id_rechazo = empleado.id,
                });
            }
            try
            {
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se rechazó la solicitud correctamente", TipoMensajesSweetAlerts.SUCCESS);
                //ENVIAR SOLICITUD DE RECHAZO EMAIL
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>(); //correos TO
                correos.Add(sCDM_solicitudBD.empleados.correo);
                envioCorreo.SendEmailAsync(correos, "Se ha rechazado la solicitud: " + sCDM_solicitudBD.id, "Se te ha rechazado la solicitud " + sCDM_solicitudBD.id + " para tu revisión.");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Se ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);
            }

            if (idDepartamento == 9) //SCDM
                return RedirectToAction("SolicitudesSCDM");
            else
            {
                //determinar vista a la que dirigir cuando no sea SCDM
                return RedirectToAction("Index");
            }
        }

        // POST: SCDM_solicitud/RechazarSolicitudDepartamento/5
        [HttpPost, ActionName("RechazarSolicitudDepartamento")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarSolicitudDepartamento(SCDM_solicitud solicitud)
        {
            SCDM_solicitud sCDM_solicitudBD = db.SCDM_solicitud.Find(solicitud.id);

            //encuentra la asignacion correspondiente
            var empleado = obtieneEmpleadoLogeado();
            var idDepartamento = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault(x => x.id_empleado == empleado.id).id_departamento;
            var idsAsignacionesList = sCDM_solicitudBD.SCDM_solicitud_asignaciones.Where(x => (x.fecha_cierre == null && x.fecha_rechazo == null)).Select(x => x.id);

            foreach (var id_asignación in idsAsignacionesList)
            {
                var asignacion = db.SCDM_solicitud_asignaciones.Find(id_asignación);
                asignacion.fecha_rechazo = DateTime.Now;
                asignacion.comentario_rechazo = solicitud.comentario_rechazo_departamento;
                asignacion.id_motivo_rechazo = solicitud.id_motivo_rechazo_departamento;
                asignacion.id_rechazo = empleado.id;
            }

            try
            {
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se rechazó la solicitud correctamente", TipoMensajesSweetAlerts.SUCCESS);
                //ENVIAR SOLICITUD DE RECHAZO EMAIL
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>
                {
                    sCDM_solicitudBD.empleados.correo
                }; //correos TO
                envioCorreo.SendEmailAsync(correos, "Se ha rechazado la solicitud: " + sCDM_solicitudBD.id, "Se te ha rechazado la solicitud " + sCDM_solicitudBD.id + " para tu revisión.");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Se ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);
            }

            if (idDepartamento == (int)SCDM_departamentos_AsignacionENUM.SCDM) //SCDM
                return RedirectToAction("SolicitudesSCDM");
            else
            {
                //determinar vista a la que dirigir cuando no sea SCDM
                return RedirectToAction("SolicitudesDepartamento");
            }
        } */

        // POST: SCDM_solicitud/RechazarSolicitudDepartamento/5
        [HttpPost, ActionName("RechazarSolicitud")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarSolicitud(SCDM_solicitud solicitud)
        {
            SCDM_solicitud sCDM_solicitudBD = db.SCDM_solicitud.Find(solicitud.id);

            //encuentra la asignacion correspondiente
            var empleado = obtieneEmpleadoLogeado();
            var idDepartamento = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault() != null ? empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault().id_departamento : 99;
            var asignacionBD = sCDM_solicitudBD.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_departamento_asignacion == idDepartamento && (x.fecha_cierre == null && x.fecha_rechazo == null));

            //Fecha de Rechazo 
            DateTime fechaActual = DateTime.Now;

            asignacionBD.fecha_rechazo = fechaActual;
            asignacionBD.comentario_rechazo = solicitud.comentario_rechazo;
            asignacionBD.id_motivo_rechazo = solicitud.id_motivo_rechazo;
            asignacionBD.id_rechazo = empleado.id;

            //crea una asignacion en caso de que sea la asignación inicial o rechazo SCDM                                                                                                                                 
            if (asignacionBD.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL ||
                asignacionBD.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM ||
                asignacionBD.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO
                )
            {

                var empleadoSolicitante = db.empleados.Find(sCDM_solicitudBD.id_solicitante);
                var idDepartamentoSolicitante = empleadoSolicitante.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault() != null ? empleadoSolicitante.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault().id_departamento : 99;

                int departamentoAsignacion = asignacionBD.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO ?
                                    (int)SCDM_departamentos_AsignacionENUM.SCDM : idDepartamentoSolicitante;

                string descripcion = asignacionBD.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO ?
                                        Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM
                                        : Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE;

                // si no existe ya una solicitud, crea una nueva
                bool existeAsignacionPrevia = sCDM_solicitudBD.SCDM_solicitud_asignaciones.Any
                    (x => x.id_departamento_asignacion == departamentoAsignacion && (x.fecha_cierre == null && x.fecha_rechazo == null) && x.descripcion == descripcion
                    );

                if (!existeAsignacionPrevia)
                    db.SCDM_solicitud_asignaciones.Add(new SCDM_solicitud_asignaciones
                    {
                        id_solicitud = solicitud.id,
                        id_departamento_asignacion = departamentoAsignacion,
                        id_empleado = asignacionBD.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO ?
                                        447 : sCDM_solicitudBD.id_solicitante,
                        fecha_asignacion = fechaActual,
                        descripcion = descripcion
                    });
            }

            try
            {
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se rechazó la solicitud correctamente", TipoMensajesSweetAlerts.SUCCESS);
                //ENVIAR SOLICITUD DE RECHAZO EMAIL
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>
                {
                    sCDM_solicitudBD.empleados.correo
                }; //correos TO

                List<string> correosSCDM = db.SCDM_cat_rel_usuarios_departamentos.Where(x => x.id_departamento == (int)Bitacoras.Util.SCDM_departamentos_AsignacionENUM.SCDM).Select(x => x.empleados.correo).Distinct().ToList();


                switch (asignacionBD.descripcion)
                {
                    case Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL: //envia a solicitante
                        envioCorreo.SendEmailAsync(correos, "MDM - Solicitud: " + sCDM_solicitudBD.id + " -->  Tu solicitud ha sido rechazada en la revisión inicial.",
                            envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.RECHAZA_SOLICITUD_INICIAL_A_SOLICITANTE, empleado, sCDM_solicitudBD, SCDM_tipo_view_edicionENUM.SOLICITANTE, comentarioRechazo: asignacionBD.comentario_rechazo, departamento: asignacionBD.SCDM_cat_departamentos_asignacion.descripcion.ToUpper())
                            );
                        break;
                    case Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO: //envia a SCDM
                        envioCorreo.SendEmailAsync(correosSCDM, "MDM - Solicitud: " + sCDM_solicitudBD.id + " -->  La solicitud ha sido rechazada por " + asignacionBD.SCDM_cat_departamentos_asignacion.descripcion.ToUpper() + ".",
                            envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.RECHAZA_SOLICITUD_DEPARTAMENTO_A_SCDM, empleado, sCDM_solicitudBD, SCDM_tipo_view_edicionENUM.SCDM, comentarioRechazo: asignacionBD.comentario_rechazo, departamento: asignacionBD.SCDM_cat_departamentos_asignacion.descripcion.ToUpper())
                            );
                        break;
                    case Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM:    //envia a solicitante
                        envioCorreo.SendEmailAsync(correos, "MDM - Solicitud: " + sCDM_solicitudBD.id + " -->  Tu solicitud ha sido rechazada por " + asignacionBD.SCDM_cat_departamentos_asignacion.descripcion.ToUpper() + ".",
                            envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.RECHAZA_SOLICITUD_SCDM_A_SOLICITANTE, empleado, sCDM_solicitudBD, SCDM_tipo_view_edicionENUM.SOLICITANTE, comentarioRechazo: asignacionBD.comentario_rechazo, departamento: asignacionBD.SCDM_cat_departamentos_asignacion.descripcion.ToUpper())
                            , emailsCC: correosSCDM); //Envia copia a SCDM
                        break;
                }

            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);
            }

            if (idDepartamento == (int)SCDM_departamentos_AsignacionENUM.SCDM) //SCDM
                return RedirectToAction("SolicitudesSCDM");
            else if (asignacionBD.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL)
            {
                //muestra vista de Inicial
                return RedirectToAction("SolicitudesRevisionInicial");
            }
            else
            {
                //muestra vista de departamentos
                return RedirectToAction("SolicitudesDepartamento");
            }
        }
        // POST: SCDM_solicitud/AsignacionIncorrecta/5
        [HttpPost, ActionName("AsignacionIncorrecta")]
        [ValidateAntiForgeryToken]
        public ActionResult AsignacionIncorrecta(SCDM_solicitud solicitud)
        {
            SCDM_solicitud sCDM_solicitudBD = db.SCDM_solicitud.Find(solicitud.id);

            //encuentra la asignacion correspondiente
            var empleado = obtieneEmpleadoLogeado();
            var idDepartamento = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault() != null ? empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault().id_departamento : 99; ;
            var asignacionBD = sCDM_solicitudBD.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_departamento_asignacion == idDepartamento && (x.fecha_cierre == null && x.fecha_rechazo == null));

            //Fecha de Rechazo 
            DateTime fechaActual = DateTime.Now;

            //agrega una fecha de cierre
            asignacionBD.fecha_cierre = fechaActual;
            asignacionBD.comentario_asignacion_incorrecta = solicitud.comentario_asignacion_incorrecta;
            asignacionBD.id_motivo_asignacion_incorrecta = solicitud.id_motivo_asignacion_incorrecta;
            asignacionBD.id_cierre = empleado.id;

            //obtiene el id de SCDM primario
            int id_empleado_scdm = db.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault(x => x.id_departamento == (int)SCDM_departamentos_AsignacionENUM.SCDM).id;

            //si no existe lo agrega
            SCDM_solicitud_asignaciones asignacion = null;

            //si no existe una asignacion A SCDM
            if (!sCDM_solicitudBD.SCDM_solicitud_asignaciones.Any(x => x.fecha_cierre == null && x.fecha_rechazo == null
                && x.id_departamento_asignacion == (int)SCDM_departamentos_AsignacionENUM.SCDM))
            {
                // si no hay mas asignaciones a otros departamentos ni al solicitante
                if (!sCDM_solicitudBD.SCDM_solicitud_asignaciones.Any(x => x.fecha_cierre == null && x.fecha_rechazo == null && (x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO || x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE)))
                    asignacion = new SCDM_solicitud_asignaciones()
                    {
                        id_solicitud = solicitud.id,
                        id_empleado = id_empleado_scdm,
                        id_departamento_asignacion = (int)SCDM_departamentos_AsignacionENUM.SCDM,
                        fecha_asignacion = fechaActual,
                        descripcion = Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM
                    };
            }

            try
            {
                if (asignacion != null)
                    db.SCDM_solicitud_asignaciones.Add(asignacion);

                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha aprobado la solicitud correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                //agrega los correos de SCDM
                List<string> correosSCDM = db.SCDM_cat_rel_usuarios_departamentos.Where(x => x.id_departamento == (int)Bitacoras.Util.SCDM_departamentos_AsignacionENUM.SCDM).Select(x => x.empleados.correo).Distinct().ToList();


                envioCorreo.SendEmailAsync(correosSCDM, "MDM - Solicitud: " + sCDM_solicitudBD.id + " --> Asignación Incorrecta para el departamento de " + asignacionBD.SCDM_cat_departamentos_asignacion.descripcion + ".", envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.ASIGNACION_INCORRECTA, empleado, sCDM_solicitudBD, SCDM_tipo_view_edicionENUM.SCDM,
                     motivoAsignacionIncorrecta: db.SCDM_cat_motivo_asignacion_incorrecta.Find(solicitud.id_motivo_asignacion_incorrecta).descripcion, comentarioAsignacionIncorrecta: solicitud.comentario_asignacion_incorrecta, departamento: asignacionBD.SCDM_cat_departamentos_asignacion.descripcion));


            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);
            }

            return RedirectToAction("SolicitudesDepartamento");
        }

        // POST: SCDM_solicitud/FinalizarSolicitud/5
        [HttpPost, ActionName("FinalizarSolicitud")]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizarSolicitud(SCDM_solicitud solicitud)
        {
            SCDM_solicitud sCDM_solicitudBD = db.SCDM_solicitud.Find(solicitud.id);

            //encuentra la asignacion correspondiente
            var empleado = obtieneEmpleadoLogeado();
            var idDepartamento = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault() != null ? empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault().id_departamento : 99; ;

            var asignacion = sCDM_solicitudBD.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_departamento_asignacion == idDepartamento && (x.fecha_cierre == null && x.fecha_rechazo == null));
            //encaso de que haya una asignación activa
            if (asignacion != null)
            {
                var id_asignación = asignacion.id;
                asignacion.fecha_cierre = DateTime.Now;
                asignacion.id_cierre = empleado.id;
            }

            try
            {
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se finalizó la solicitud correctamente", TipoMensajesSweetAlerts.SUCCESS);

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>
                {
                    sCDM_solicitudBD.empleados.correo //solicitante
                }; //correos TO

                //obtiene los correos de SCDM
                List<string> correosSCDM = db.SCDM_cat_rel_usuarios_departamentos.Where(x => x.id_departamento == (int)Bitacoras.Util.SCDM_departamentos_AsignacionENUM.SCDM).Select(x => x.empleados.correo).Distinct().ToList();

                envioCorreo.SendEmailAsync(correos, "MDM - Solicitud: " + solicitud.id + " --> La solictud ha sido cerrada.",
                          envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.FINALIZA_SOLICITUD, empleado, sCDM_solicitudBD, SCDM_tipo_view_edicionENUM.SOLICITANTE)
                          , correosSCDM);

            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Se ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);
            }

            if (idDepartamento == 9) //SCDM
                return RedirectToAction("Estatus");
            else
            {
                //determinar vista a la que dirigir cuando no sea SCDM
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ActionName("CancelarSolicitud")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelarSolicitud(SCDM_solicitud solicitud)
        {
            SCDM_solicitud sCDM_solicitudBD = db.SCDM_solicitud.Find(solicitud.id);


            try
            {
                //Deshabilita la solicitud
                sCDM_solicitudBD.activo = false;

                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se canceló la solicitud correctamente", TipoMensajesSweetAlerts.SUCCESS);

                ////envia correo electronico
                //EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                //List<String> correos = new List<string>
                //{
                //    sCDM_solicitudBD.empleados.correo //solicitante
                //}; //correos TO

                ////obtiene los correos de SCDM
                //List<string> correosSCDM = db.SCDM_cat_rel_usuarios_departamentos.Where(x => x.id_departamento == (int)Bitacoras.Util.SCDM_departamentos_AsignacionENUM.SCDM).Select(x => x.empleados.correo).Distinct().ToList();

                //envioCorreo.SendEmailAsync(correos, "MDM - Solicitud: " + solicitud.id + " --> La solictud ha sido cerrada.",
                //          envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.FINALIZA_SOLICITUD, empleado, sCDM_solicitudBD, SCDM_tipo_view_edicionENUM.SOLICITANTE)
                //          , correosSCDM);

            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Se ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);
            }

            return RedirectToAction("Estatus");

        }


        // POST: SCDM_solicitud/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            db.SCDM_solicitud.Remove(sCDM_solicitud);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region llamadas ajax
        ///<summary>
        ///Obtiene las lineas de los rollos
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public ActionResult EnviaRollosForm(int? id, List<string[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[dataListFromTable.Count];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            List<SCDM_solicitud_rel_item_material> rollos = ConvierteArrayARollo(dataListFromTable, id);
            if (rollos.Count == 0)
                list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //crea, modifica o elimina rollo
            try
            {
                for (int i = 0; i < rollos.Count; i++)
                {
                    if (rollos[i].id == 0) //si no existe el rollo
                    {
                        db.SCDM_solicitud_rel_item_material.Add(rollos[i]);
                        //debe guardarlo para obtener el id
                        try
                        {
                            db.SaveChanges();
                            list[i] = new { result = "OK", icon = "success", fila = rollos[i].num_fila, id = rollos[i].id, operacion = "CREATE", message = "Se guardaron los cambio correctamente" };
                        }
                        catch (Exception e)
                        {
                            list[0] = new { result = "ERROR", icon = "error", fila = rollos[i].num_fila, operacion = "CREATE", message = e.Message };
                        }
                    }
                    else //si ya existe es una modificacion
                    {

                        db.Entry(rollos[i]).State = EntityState.Modified;

                        db.SaveChanges();
                        list[i] = new { result = "OK", icon = "success", fila = rollos[i].num_fila, id = rollos[i].id, operacion = "UPDATE", message = "Se guardaron los cambio correctamente" };
                    }

                }
                //elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = db.SCDM_solicitud_rel_item_material.ToList().Where(x => !rollos.Any(y => y.id == x.id) && x.id_solicitud == id.Value && x.id_tipo_material == Bitacoras.Util.SCDM_solicitud_rel_item_material_tipo.ROLLO).ToList();
                db.SCDM_solicitud_rel_item_material.RemoveRange(toDeleteList);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }



            return Json(list, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene las cintas enviadas
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public ActionResult EnviaCintasForm(int? id, List<string[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[dataListFromTable.Count];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            List<SCDM_solicitud_rel_item_material> cintas = ConvierteArrayACinta(dataListFromTable, id);
            if (cintas.Count == 0)
                list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //crea, modifica o elimina cinta
            try
            {
                for (int i = 0; i < cintas.Count; i++)
                {
                    if (cintas[i].id == 0) //si no existe el rollo
                    {
                        db.SCDM_solicitud_rel_item_material.Add(cintas[i]);
                        //debe guardarlo para obtener el id
                        try
                        {
                            db.SaveChanges();
                            list[i] = new { result = "OK", icon = "success", fila = cintas[i].num_fila, id = cintas[i].id, operacion = "CREATE", message = "Se guardaron los cambios correctamente" };
                        }
                        catch (Exception e)
                        {
                            list[0] = new { result = "ERROR", icon = "error", fila = cintas[i].num_fila, operacion = "CREATE", message = e.Message };
                        }
                    }
                    else //si ya existe es una modificacion
                    {
                        db.Entry(cintas[i]).State = EntityState.Modified;
                        db.SaveChanges();
                        list[i] = new { result = "OK", icon = "success", fila = cintas[i].num_fila, id = cintas[i].id, operacion = "UPDATE", message = "Se guardaron los cambios correctamente" };
                    }

                }
                //elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = db.SCDM_solicitud_rel_item_material.ToList().Where(x => !cintas.Any(y => y.id == x.id) && x.id_solicitud == id.Value && x.id_tipo_material == Bitacoras.Util.SCDM_solicitud_rel_item_material_tipo.CINTA).ToList();
                db.SCDM_solicitud_rel_item_material.RemoveRange(toDeleteList);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Envia desde HT los valores para CB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataListFromTable"></param>
        /// <returns></returns>
        public ActionResult EnviaCBForm(int? id, List<string[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[dataListFromTable.Count];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            List<SCDM_solicitud_rel_item_material> items = ConvierteArrayACB(dataListFromTable, id);
            if (items.Count == 0)
                list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //crea, modifica o elimina cinta
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].id == 0) //si no existe el rollo
                    {
                        db.SCDM_solicitud_rel_item_material.Add(items[i]);
                        //debe guardarlo para obtener el id
                        try
                        {
                            db.SaveChanges();
                            list[i] = new { result = "OK", icon = "success", fila = items[i].num_fila, id = items[i].id, operacion = "CREATE", message = "Se guardaron los cambios correctamente" };
                        }
                        catch (Exception e)
                        {
                            list[0] = new { result = "ERROR", icon = "error", fila = items[i].num_fila, operacion = "CREATE", message = e.Message };
                        }
                    }
                    else //si ya existe es una modificacion
                    {
                        db.Entry(items[i]).State = EntityState.Modified;
                        db.SaveChanges();
                        list[i] = new { result = "OK", icon = "success", fila = items[i].num_fila, id = items[i].id, operacion = "UPDATE", message = "Se guardaron los cambios correctamente" };
                    }

                }
                //elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = db.SCDM_solicitud_rel_item_material.ToList().Where(x => !items.Any(y => y.id == x.id) && x.id_solicitud == id.Value && x.id_tipo_material == Bitacoras.Util.SCDM_solicitud_rel_item_material_tipo.C_B).ToList();
                db.SCDM_solicitud_rel_item_material.RemoveRange(toDeleteList);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        ///<summary>
        ///Obtiene las platinas Enviadas
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public ActionResult EnviaPlatinasForm(int? id, List<string[]> dataListFromTable, int tipoPlatina = 0)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[dataListFromTable.Count];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            List<SCDM_solicitud_rel_item_material> platinas = ConvierteArrayAPlatina(dataListFromTable, id, tipoPlatina);
            if (platinas.Count == 0)
                list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //crea, modifica o elimina cinta
            try
            {
                for (int i = 0; i < platinas.Count; i++)
                {
                    if (platinas[i].id == 0) //si no existe el rollo
                    {
                        db.SCDM_solicitud_rel_item_material.Add(platinas[i]);
                        //debe guardarlo para obtener el id
                        try
                        {
                            db.SaveChanges();
                            list[i] = new { result = "OK", icon = "success", fila = platinas[i].num_fila, id = platinas[i].id, operacion = "CREATE", message = "Se guardaron los cambios correctamente" };
                        }
                        catch (Exception e)
                        {
                            list[0] = new { result = "ERROR", icon = "error", fila = platinas[i].num_fila, operacion = "CREATE", message = e.Message };
                        }
                    }
                    else //si ya existe es una modificacion
                    {
                        db.Entry(platinas[i]).State = EntityState.Modified;
                        db.SaveChanges();
                        list[i] = new { result = "OK", icon = "success", fila = platinas[i].num_fila, id = platinas[i].id, operacion = "UPDATE", message = "Se guardaron los cambios correctamente" };
                    }

                }
                //elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = db.SCDM_solicitud_rel_item_material.ToList().Where(x => !platinas.Any(y => y.id == x.id) && x.id_solicitud == id.Value
                && x.id_tipo_material == tipoPlatina).ToList();
                db.SCDM_solicitud_rel_item_material.RemoveRange(toDeleteList);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        ///<summary>
        ///Obtiene los valores de la lista tecnica Enviados
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public ActionResult EnviaListaTecnicaForm(int? id, List<string[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[dataListFromTable.Count];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            List<SCDM_solicitud_rel_lista_tecnica> lista_item = ConvierteArrayAListaTecnica(dataListFromTable, id);
            if (lista_item.Count == 0)
                list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //crea, modifica o elimina cinta
            try
            {
                for (int i = 0; i < lista_item.Count; i++)
                {
                    if (lista_item[i].id == 0) //si no existe el rollo
                    {
                        db.SCDM_solicitud_rel_lista_tecnica.Add(lista_item[i]);
                        //debe guardarlo para obtener el id
                        try
                        {
                            db.SaveChanges();
                            list[i] = new { result = "OK", icon = "success", fila = lista_item[i].num_fila, id = lista_item[i].id, operacion = "CREATE", message = "Se guardaron los cambios correctamente" };
                        }
                        catch (Exception e)
                        {
                            list[0] = new { result = "ERROR", icon = "error", fila = lista_item[i].num_fila, operacion = "CREATE", message = e.Message };
                        }
                    }
                    else //si ya existe es una modificacion
                    {
                        db.Entry(lista_item[i]).State = EntityState.Modified;
                        db.SaveChanges();
                        list[i] = new { result = "OK", icon = "success", fila = lista_item[i].num_fila, id = lista_item[i].id, operacion = "UPDATE", message = "Se guardaron los cambios correctamente" };
                    }

                }
                //elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = db.SCDM_solicitud_rel_lista_tecnica.ToList().Where(x => !lista_item.Any(y => y.id == x.id) && x.id_solicitud == id.Value).ToList();
                db.SCDM_solicitud_rel_lista_tecnica.RemoveRange(toDeleteList);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Envia los datos de facturaciín
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataListFromTable"></param>
        /// <returns></returns>
        public ActionResult EnviaFacturacion(int? id, List<string[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[dataListFromTable.Count];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            List<SCDM_solicitud_rel_facturacion> lista_item = ConvierteArrayAFacturacion(dataListFromTable, id);
            if (lista_item.Count == 0)
                list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //crea, modifica o elimina cinta
            try
            {
                for (int i = 0; i < lista_item.Count; i++)
                {
                    if (lista_item[i].id == 0) //si no existe el rollo
                    {
                        //db.SCDM_solicitud_rel_facturacion.Add(lista_item[i]);
                        ////debe guardarlo para obtener el id
                        //try
                        //{
                        //    db.SaveChanges();
                        //    list[i] = new { result = "OK", icon = "success", fila = lista_item[i].num_fila, id = lista_item[i].id, operacion = "CREATE", message = "Se guardaron los cambios correctamente" };
                        //}
                        //catch (Exception e)
                        //{
                        //    list[0] = new { result = "ERROR", icon = "error", fila = lista_item[i].num_fila, operacion = "CREATE", message = e.Message };
                        //}
                    }
                    else //si ya existe es una modificacion
                    {
                        var itemDB = db.SCDM_solicitud_rel_facturacion.Find(lista_item[i].id);

                        itemDB.uso_CFDI_01 = lista_item[i].uso_CFDI_01;
                        itemDB.uso_CFDI_02 = lista_item[i].uso_CFDI_02;
                        itemDB.uso_CFDI_03 = lista_item[i].uso_CFDI_03;
                        itemDB.uso_CFDI_04 = lista_item[i].uso_CFDI_04;
                        itemDB.uso_CFDI_05 = lista_item[i].uso_CFDI_05;
                        itemDB.uso_CFDI_06 = lista_item[i].uso_CFDI_06;
                        itemDB.uso_CFDI_07 = lista_item[i].uso_CFDI_07;
                        itemDB.uso_CFDI_08 = lista_item[i].uso_CFDI_08;
                        itemDB.uso_CFDI_09 = lista_item[i].uso_CFDI_09;
                        itemDB.uso_CFDI_10 = lista_item[i].uso_CFDI_10;

                        //db.Entry(lista_item[i]).State = EntityState.Modified;
                        db.SaveChanges();
                        list[i] = new { result = "OK", icon = "success", fila = lista_item[i].num_fila, id = lista_item[i].id, operacion = "UPDATE", message = "Se guardaron los cambios correctamente" };
                    }

                }
                //**elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = db.SCDM_solicitud_rel_facturacion.ToList().Where(x => !lista_item.Any(y => y.id == x.id)
                && (x.SCDM_solicitud_rel_item_material != null && x.SCDM_solicitud_rel_item_material.id_solicitud == id.Value
                    || x.SCDM_solicitud_rel_creacion_referencia != null && x.SCDM_solicitud_rel_creacion_referencia.id_solicitud == id.Value)
                ).ToList();

                db.SCDM_solicitud_rel_facturacion.RemoveRange(toDeleteList);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Recibe las ordenes de compra y las guarda en BD
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataListFromTable"></param>
        /// <returns></returns>
        public ActionResult EnviaOrdenCompaForm(int? id, List<string[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[dataListFromTable.Count];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_orden_compra
            List<SCDM_solicitud_rel_orden_compra> lista_item = ConvierteArrayAOrdenesCompra(dataListFromTable, id);
            if (lista_item.Count == 0)
                list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //crea, modifica o elimina cinta
            try
            {
                for (int i = 0; i < lista_item.Count; i++)
                {
                    if (lista_item[i].id == 0) //si no existe el rollo
                    {
                        db.SCDM_solicitud_rel_orden_compra.Add(lista_item[i]);
                        //debe guardarlo para obtener el id
                        try
                        {
                            db.SaveChanges();
                            list[i] = new { result = "OK", icon = "success", fila = lista_item[i].num_fila, id = lista_item[i].id, operacion = "CREATE", message = "Se guardaron los cambios correctamente" };
                        }
                        catch (Exception e)
                        {
                            list[0] = new { result = "ERROR", icon = "error", fila = lista_item[i].num_fila, operacion = "CREATE", message = e.Message };
                        }
                    }
                    else //si ya existe es una modificacion
                    {
                        db.Entry(lista_item[i]).State = EntityState.Modified;
                        db.SaveChanges();
                        list[i] = new { result = "OK", icon = "success", fila = lista_item[i].num_fila, id = lista_item[i].id, operacion = "UPDATE", message = "Se guardaron los cambios correctamente" };
                    }

                }
                //elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = db.SCDM_solicitud_rel_orden_compra.ToList().Where(x => !lista_item.Any(y => y.id == x.id) && x.id_solicitud == id.Value).ToList();
                db.SCDM_solicitud_rel_orden_compra.RemoveRange(toDeleteList);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene creaciones Referencia
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public ActionResult EnviaCreacionReferenciaForm(int? id, List<string[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[dataListFromTable.Count];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            List<SCDM_solicitud_rel_creacion_referencia> creacionReferenciasList = ConvierteArrayACreacionReferencia(dataListFromTable, id);
            if (creacionReferenciasList.Count == 0)
                list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //crea, modifica o elimina cinta
            try
            {
                for (int i = 0; i < creacionReferenciasList.Count; i++)
                {
                    if (creacionReferenciasList[i].id == 0) //si no existe la creacion referencia
                    {
                        db.SCDM_solicitud_rel_creacion_referencia.Add(creacionReferenciasList[i]);
                        //debe guardarlo para obtener el id
                        try
                        {
                            db.SaveChanges();
                            list[i] = new { result = "OK", icon = "success", fila = creacionReferenciasList[i].num_fila, id = creacionReferenciasList[i].id, operacion = "CREATE", message = "Se guardaron los cambios correctamente" };
                        }
                        catch (Exception e)
                        {
                            list[0] = new { result = "ERROR", icon = "error", fila = creacionReferenciasList[i].num_fila, operacion = "CREATE", message = e.Message };
                        }
                    }
                    else //si ya existe es una modificacion
                    {
                        //guarda los datos que no vienen en el formulario
                        var refBD = db.SCDM_solicitud_rel_creacion_referencia.Find(creacionReferenciasList[i].id);
                        creacionReferenciasList[i].resultado = refBD.resultado;
                        creacionReferenciasList[i].ejecucion_correcta = refBD.ejecucion_correcta;
                        creacionReferenciasList[i].resultado_update = refBD.resultado_update;

                        db.Entry(refBD).State = EntityState.Detached;
                        // dbSet.Attach(entity);

                        db.Entry(creacionReferenciasList[i]).State = EntityState.Modified;
                        db.SaveChanges();
                        list[i] = new { result = "OK", icon = "success", fila = creacionReferenciasList[i].num_fila, id = creacionReferenciasList[i].id, operacion = "UPDATE", message = "Se guardaron los cambioas correctamente" };
                    }

                }
                //elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = db.SCDM_solicitud_rel_creacion_referencia.ToList().Where(x => !creacionReferenciasList.Any(y => y.id == x.id) && x.id_solicitud == id.Value).ToList();
                db.SCDM_solicitud_rel_creacion_referencia.RemoveRange(toDeleteList);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        ///<summary>
        ///Obtiene creaciones Referencia
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public ActionResult EnviaCambioIngenieriaForm(int? id, List<string[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[dataListFromTable.Count];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            List<SCDM_solicitud_rel_cambio_ingenieria> cambioIngenieriaList = ConvierteArrayACambiosIngenieria(dataListFromTable, id);
            if (cambioIngenieriaList.Count == 0)
                list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //crea, modifica o elimina cinta
            try
            {
                for (int i = 0; i < cambioIngenieriaList.Count; i++)
                {
                    if (cambioIngenieriaList[i].id == 0) //si no existe la creacion referencia
                    {
                        db.SCDM_solicitud_rel_cambio_ingenieria.Add(cambioIngenieriaList[i]);
                        //debe guardarlo para obtener el id
                        try
                        {
                            db.SaveChanges();
                            list[i] = new { result = "OK", icon = "success", fila = cambioIngenieriaList[i].num_fila, id = cambioIngenieriaList[i].id, operacion = "CREATE", message = "Se guardaron los cambios correctamente" };
                        }
                        catch (Exception e)
                        {
                            list[0] = new { result = "ERROR", icon = "error", fila = cambioIngenieriaList[i].num_fila, operacion = "CREATE", message = e.Message };
                        }
                    }
                    else //si ya existe es una modificacion
                    {
                        //guarda los datos que no vienen en el formulario
                        var refBD = db.SCDM_solicitud_rel_cambio_ingenieria.Find(cambioIngenieriaList[i].id);
                        cambioIngenieriaList[i].resultado = refBD.resultado;
                        cambioIngenieriaList[i].ejecucion_correcta = refBD.ejecucion_correcta;

                        db.Entry(refBD).State = EntityState.Detached;
                        // dbSet.Attach(entity);

                        db.Entry(cambioIngenieriaList[i]).State = EntityState.Modified;
                        db.SaveChanges();
                        list[i] = new { result = "OK", icon = "success", fila = cambioIngenieriaList[i].num_fila, id = cambioIngenieriaList[i].id, operacion = "UPDATE", message = "Se guardaron los cambioas correctamente" };
                    }

                }
                //elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = db.SCDM_solicitud_rel_cambio_ingenieria.ToList().Where(x => !cambioIngenieriaList.Any(y => y.id == x.id) && x.id_solicitud == id.Value).ToList();
                db.SCDM_solicitud_rel_cambio_ingenieria.RemoveRange(toDeleteList);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Guarda en BD los valores ingresados en cambios de Budget
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataListFromTable"></param>
        /// <returns></returns>
        public ActionResult EnviaBudgetForm(int? id, List<string[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[dataListFromTable.Count];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            List<SCDM_solicitud_rel_cambio_budget> cambioBudgetList = ConvierteArrayACambiosBudget(dataListFromTable, id);
            if (cambioBudgetList.Count == 0)
                list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //crea, modifica o elimina cinta
            try
            {
                for (int i = 0; i < cambioBudgetList.Count; i++)
                {
                    if (cambioBudgetList[i].id == 0) //si no existe la creacion referencia
                    {
                        db.SCDM_solicitud_rel_cambio_budget.Add(cambioBudgetList[i]);
                        //debe guardarlo para obtener el id
                        try
                        {
                            db.SaveChanges();
                            list[i] = new { result = "OK", icon = "success", fila = cambioBudgetList[i].num_fila, id = cambioBudgetList[i].id, operacion = "CREATE", message = "Se guardaron los cambios correctamente" };
                        }
                        catch (Exception e)
                        {
                            list[0] = new { result = "ERROR", icon = "error", fila = cambioBudgetList[i].num_fila, operacion = "CREATE", message = e.Message };
                        }
                    }
                    else //si ya existe es una modificacion
                    {
                        //guarda los datos que no vienen en el formulario
                        var refBD = db.SCDM_solicitud_rel_cambio_budget.Find(cambioBudgetList[i].id);
                        cambioBudgetList[i].resultado = refBD.resultado;
                        cambioBudgetList[i].ejecucion_correcta = refBD.ejecucion_correcta;

                        db.Entry(refBD).State = EntityState.Detached;
                        // dbSet.Attach(entity);

                        db.Entry(cambioBudgetList[i]).State = EntityState.Modified;
                        db.SaveChanges();
                        list[i] = new { result = "OK", icon = "success", fila = cambioBudgetList[i].num_fila, id = cambioBudgetList[i].id, operacion = "UPDATE", message = "Se guardaron los cambioas correctamente" };
                    }

                }
                //elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = db.SCDM_solicitud_rel_cambio_budget.ToList().Where(x => !cambioBudgetList.Any(y => y.id == x.id) && x.id_solicitud == id.Value).ToList();
                db.SCDM_solicitud_rel_cambio_budget.RemoveRange(toDeleteList);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Convierte el array recibido en el formulario a un objeto de cambio de estatus
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataListFromTable"></param>
        /// <returns></returns>
        public ActionResult EnviaCambioEstatusForm(int? id, List<string[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[dataListFromTable.Count];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            List<SCDM_solicitud_rel_activaciones> activacionesList = ConvierteArrayACambiosEstatus(dataListFromTable, id);
            if (activacionesList.Count == 0)
                list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //crea, modifica o elimina cinta
            try
            {
                for (int i = 0; i < activacionesList.Count; i++)
                {
                    if (activacionesList[i].id == 0) //si no existe la creacion referencia
                    {
                        db.SCDM_solicitud_rel_activaciones.Add(activacionesList[i]);
                        //debe guardarlo para obtener el id
                        try
                        {
                            db.SaveChanges();
                            list[i] = new { result = "OK", icon = "success", fila = activacionesList[i].num_fila, id = activacionesList[i].id, operacion = "CREATE", message = "Se guardaron los cambios correctamente" };
                        }
                        catch (Exception e)
                        {
                            list[0] = new { result = "ERROR", icon = "error", fila = activacionesList[i].num_fila, operacion = "CREATE", message = e.Message };
                        }
                    }
                    else //si ya existe es una modificacion
                    {
                        //guarda los datos que no vienen en el formulario
                        var refBD = db.SCDM_solicitud_rel_activaciones.Find(activacionesList[i].id);
                        activacionesList[i].mensaje_sap = refBD.mensaje_sap;
                        activacionesList[i].ejecucion_correcta = refBD.ejecucion_correcta;

                        db.Entry(refBD).State = EntityState.Detached;
                        // dbSet.Attach(entity);

                        db.Entry(activacionesList[i]).State = EntityState.Modified;
                        db.SaveChanges();
                        list[i] = new { result = "OK", icon = "success", fila = activacionesList[i].num_fila, id = activacionesList[i].id, operacion = "UPDATE", message = "Se guardaron los cambioas correctamente" };
                    }

                }
                //elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = db.SCDM_solicitud_rel_activaciones.ToList().Where(x => !activacionesList.Any(y => y.id == x.id) && x.id_solicitud == id.Value).ToList();
                db.SCDM_solicitud_rel_activaciones.RemoveRange(toDeleteList);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Convierte el array recibido en el formulario a un objeto de cambio de estatus
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataListFromTable"></param>
        /// <returns></returns>
        public ActionResult EnviaMaterialesExtensionForm(int? id, List<string[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[dataListFromTable.Count];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            List<SCDM_solicitud_rel_extension_usuario> materialesExtensionList = ConvierteArrayAMaterialesExtension(dataListFromTable, id);
            if (materialesExtensionList.Count == 0)
                list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //crea, modifica o elimina cinta
            try
            {
                for (int i = 0; i < materialesExtensionList.Count; i++)
                {
                    if (materialesExtensionList[i].id == 0) //si no existe la creacion referencia
                    {
                        db.SCDM_solicitud_rel_extension_usuario.Add(materialesExtensionList[i]);
                        //debe guardarlo para obtener el id
                        try
                        {
                            db.SaveChanges();
                            list[i] = new { result = "OK", icon = "success", fila = materialesExtensionList[i].num_fila, id = materialesExtensionList[i].id, operacion = "CREATE", message = "Se guardaron los cambios correctamente" };
                        }
                        catch (Exception e)
                        {
                            list[0] = new { result = "ERROR", icon = "error", fila = materialesExtensionList[i].num_fila, operacion = "CREATE", message = e.Message };
                        }
                    }
                    else //si ya existe es una modificacion
                    {
                        //guarda los datos que no vienen en el formulario
                        var refBD = db.SCDM_solicitud_rel_extension_usuario.Find(materialesExtensionList[i].id);
                        materialesExtensionList[i].mensaje_sap = refBD.mensaje_sap;
                        materialesExtensionList[i].ejecucion_correcta = refBD.ejecucion_correcta;

                        db.Entry(refBD).State = EntityState.Detached;
                        // dbSet.Attach(entity);

                        db.Entry(materialesExtensionList[i]).State = EntityState.Modified;
                        db.SaveChanges();
                        list[i] = new { result = "OK", icon = "success", fila = materialesExtensionList[i].num_fila, id = materialesExtensionList[i].id, operacion = "UPDATE", message = "Se guardaron los cambioas correctamente" };
                    }

                }
                //elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = db.SCDM_solicitud_rel_extension_usuario.ToList().Where(x => !materialesExtensionList.Any(y => y.id == x.id) && x.id_solicitud == id.Value).ToList();
                db.SCDM_solicitud_rel_extension_usuario.RemoveRange(toDeleteList);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObtieneCorreos(bool isSelected, int idDepartamento, int idSolicitud)
        {
            //obtiene las plantas asociadas a la solicitud
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);
            List<plantas> plantas = solicitud.SCDM_rel_solicitud_plantas.Select(x => x.plantas).ToList();

            //obtiene todos los usuarios activos para las plantas de la solicitud, segun el depto
            var data = db.SCDM_cat_usuarios_revision_departamento.ToList().Where(x => x.activo && x.SCDM_cat_rel_usuarios_departamentos.id_departamento == idDepartamento && plantas.Any(y => y == x.plantas) && x.envia_correo).ToList();

            //var data = db.SCDM_solicitud_rel_item_material.Where(x => x.id_solicitud == id_solicitud && x.id_tipo_material == Bitacoras.Util.SCDM_solicitud_rel_item_material_tipo.ROLLO).ToList();

            var jsonData = new object[data.Count()];

            //valida si exite un asignacion previa para el departamento
            var asignacionPrevia = solicitud.SCDM_solicitud_asignaciones.FirstOrDefault(x => x.id_departamento_asignacion == idDepartamento && x.fecha_cierre == null && x.fecha_rechazo == null);

            bool esVentas = idDepartamento == (int)Bitacoras.Util.SCDM_departamentos_AsignacionENUM.VENTAS;

            //obtiene los clientes de la solicitud
            List<string> clientesSolicitud = db.view_SCDM_clientes_por_solictud.Where(x => x.id_solicitud == idSolicitud).Select(x => x.sap_cliente).ToList();
            //obtiene los gerentes de cuenta, segun los clientes
            List<SCDM_cat_rel_gerentes_clientes> gerentesCuenta = new List<SCDM_cat_rel_gerentes_clientes>();
            if (esVentas)
            {
                foreach (var claveSAP in clientesSolicitud)
                {
                    gerentesCuenta.AddRange(db.SCDM_cat_rel_gerentes_clientes.Where(x => x.clientes.claveSAP == claveSAP));
                }
            }
            //elimina repetidos
            gerentesCuenta = gerentesCuenta.Distinct().ToList();

            for (int i = 0; i < data.Count(); i++)
            {

                jsonData[i] = new[] {
                    data[i].SCDM_cat_rel_usuarios_departamentos.id_departamento.ToString(),
                    data[i].SCDM_cat_rel_usuarios_departamentos.id_empleado.ToString(),
                    data[i].id.ToString(), //SCDM_cat_usuarios_revision_departamento
                    asignacionPrevia != null ? "Recordatorio" : "Asignación",
                    esVentas && !gerentesCuenta.Any(x=> x.id_empleado == data[i].SCDM_cat_rel_usuarios_departamentos.id_empleado) ? "false" :"true",
                    data[i].SCDM_cat_rel_usuarios_departamentos.SCDM_cat_departamentos_asignacion.descripcion,
                    data[i].plantas.ConcatPlantaSap,
                    data[i].SCDM_cat_rel_usuarios_departamentos.empleados.ConcatNombre,
                    data[i].SCDM_cat_rel_usuarios_departamentos.empleados.correo,
                    data[i].tipo,
                    i==0 ? asignacionPrevia != null ? asignacionPrevia.comentario_scdm : esVentas ? "Cliente: "+solicitud.ClienteString: string.Empty : "---",
                    };

            }



            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Retorna un json con los datos de la solicitud
        /// </summary>
        /// <returns></returns>      
        public JsonResult CargaRollos(int id_solicitud = 0)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var data = db.SCDM_solicitud_rel_item_material.Where(x => x.id_solicitud == id_solicitud && x.id_tipo_material == Bitacoras.Util.SCDM_solicitud_rel_item_material_tipo.ROLLO).ToList();

            var jsonData = new object[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {

                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    !string.IsNullOrEmpty(data[i].numero_material) ? data[i].numero_material:string.Empty,
                    data[i].material_del_cliente.HasValue? data[i].material_del_cliente.Value?"SÍ":"NO":string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_venta)?  data[i].tipo_venta : string.Empty,
                    !string.IsNullOrEmpty(data[i].numero_odc_cliente)?  data[i].numero_odc_cliente : string.Empty,
                    !string.IsNullOrEmpty(data[i].cliente)?  data[i].cliente : string.Empty,
                    data[i].requiere_ppaps.HasValue? data[i].requiere_ppaps.Value?"SÍ":"NO":string.Empty,
                    data[i].requiere_imds.HasValue? data[i].requiere_imds.Value?"SÍ":"NO":string.Empty,
                    !string.IsNullOrEmpty(data[i].proveedor)?  data[i].proveedor : string.Empty,
                    !string.IsNullOrEmpty(data[i].molino)?  data[i].molino : string.Empty,
                    !string.IsNullOrEmpty(data[i].metal)?  data[i].metal : string.Empty,
                    !string.IsNullOrEmpty(data[i].unidad_medida_inventario)?  data[i].unidad_medida_inventario : string.Empty,
                    !string.IsNullOrEmpty(data[i].grado_calidad) ? data[i].grado_calidad : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_material_es) ? data[i].descripcion_material_es : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_material_en) ? data[i].descripcion_material_en : string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_recubrimiento)?  data[i].tipo_recubrimiento : string.Empty,
                    !string.IsNullOrEmpty(data[i].clase_aprovisionamiento)?  data[i].clase_aprovisionamiento : string.Empty,
                    !string.IsNullOrEmpty(data[i].numero_parte) ? data[i].numero_parte : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_numero_parte) ? data[i].descripcion_numero_parte : string.Empty,
                    !string.IsNullOrEmpty(data[i].norma_referencia) ? data[i].norma_referencia : string.Empty,
                    data[i].espesor_mm.ToString(),
                    data[i].espesor_tolerancia_negativa_mm.ToString(),
                    data[i].espesor_tolerancia_positiva_mm.ToString(),
                    data[i].ancho_mm.ToString(),
                    data[i].ancho_tolerancia_negativa_mm.ToString(),
                    data[i].ancho_tolerancia_positiva_mm.ToString(),
                    data[i].diametro_interior.ToString(),
                    data[i].diametro_exterior_maximo_mm.ToString(),
                    data[i].peso_min_kg.ToString(),
                    data[i].peso_max_kg.ToString(),
                    !string.IsNullOrEmpty(data[i].peso_recubrimiento)?  data[i].peso_recubrimiento : string.Empty,
                    !string.IsNullOrEmpty(data[i].parte_interior_exterior)?  data[i].parte_interior_exterior : string.Empty,
                    !string.IsNullOrEmpty(data[i].posicion_rollo)?  data[i].posicion_rollo : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_1)?  data[i].ihs_1 : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_2)?  data[i].ihs_2 : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_3)?  data[i].ihs_3 : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_4)?  data[i].ihs_4 : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_5)?  data[i].ihs_5 : string.Empty,
                    !string.IsNullOrEmpty(data[i].modelo_negocio)?  data[i].modelo_negocio : string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_transito)?  data[i].tipo_transito : string.Empty,
                    data[i].aplica_procesador_externo.HasValue? data[i].aplica_procesador_externo.Value?"SÍ":"NO":string.Empty,
                    !string.IsNullOrEmpty(data[i].procesador_externo_nombre)?  data[i].procesador_externo_nombre : string.Empty,
                    !String.IsNullOrEmpty(data[i].numero_antiguo_material)? data[i].numero_antiguo_material : string.Empty,
                    data[i].planicidad_mm.ToString(),
                    !String.IsNullOrEmpty(data[i].msa_hoda)?data[i].msa_hoda:string.Empty ,
                    data[i].requiere_consiliacion_puntas_colar.HasValue? data[i].requiere_consiliacion_puntas_colar.Value?"SÍ":"NO":string.Empty,
                    data[i].scrap_permitido_puntas_colas.ToString(),
                    data[i].fecha_validez.HasValue?data[i].fecha_validez.Value.ToString("dd/MM/yyyy"):string.Empty,
                    !string.IsNullOrEmpty(data[i].disponente)?  data[i].disponente : string.Empty,
                    !string.IsNullOrEmpty(data[i].unidad_medida_ventas)?  data[i].unidad_medida_ventas : string.Empty,

                    };

            }



            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }
        public JsonResult CargaGantt(int id_solicitud = 0)
        {
            //strings para colores
            string pendienteColor = "#fae517";
            string finalizadoColor = "#0cab1b";
            string RechazadoColor = "#FF0000";
            string AsignacionIncorrectaColor = "#cccdce";

            string formatDate = "yyyy-MM-dd HH:mm:ss";

            //obtiene la solicitud
            SCDM_solicitud solicitud = db.SCDM_solicitud.Find(id_solicitud);
            //obtiene los departamentos
            var listDepartamentos = db.SCDM_cat_departamentos_asignacion.Where(x => x.activo).ToList();

            //obtiene el listado de item tipo rollo de la solicitud
            var jsonData = new object[2 + listDepartamentos.Count()];

            var jsonPeridosInicial = new object[1];

            DateTime now = DateTime.Now;


            //1.- calcula el tiempo del solicitante hasta que lo envia
            DateTime fechaAsignacion = solicitud.fecha_creacion;
            DateTime fechaTermino = solicitud.SCDM_solicitud_asignaciones.Any() ? solicitud.SCDM_solicitud_asignaciones.OrderBy(x => x.fecha_asignacion).FirstOrDefault().fecha_asignacion : now;

            //valida si hay asignaciones a solicitante
            int asignacionesSolicitante = solicitud.SCDM_solicitud_asignaciones.Where(x => x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE).Count();

            var jsonPeridosSolicitante = new object[asignacionesSolicitante + 1];
            //crea los periodos (1 de momento)
            jsonPeridosSolicitante[0] = new { id = "1_0", start = fechaAsignacion.ToString(formatDate), end = fechaTermino.ToString(formatDate), fill = fechaTermino == now ? pendienteColor : finalizadoColor };

            //recorre el restro de asignaciones a solicitante
            int i = 1;
            foreach (var item in solicitud.SCDM_solicitud_asignaciones.Where(x => x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE))
            {
                fechaAsignacion = item.fecha_asignacion;
                fechaTermino = item.fecha_cierre != null ? item.fecha_cierre.Value : item.fecha_rechazo != null ? item.fecha_rechazo.Value : now;

                jsonPeridosSolicitante[i] = new
                {
                    id = $"1_{i}",
                    start = fechaAsignacion.ToString(formatDate),
                    end = fechaTermino.ToString(formatDate)
                    ,
                    fill = item.fecha_cierre != null ? finalizadoColor : item.fecha_rechazo != null ? RechazadoColor : pendienteColor
                };
                i++;
            }

            //agrega al JSON principal
            jsonData[0] = new { id = "1", name = "Solicitante", periods = jsonPeridosSolicitante };

            //2.- calcula el tiempo para el aprobador
            var asignacionTemp = solicitud.SCDM_solicitud_asignaciones.FirstOrDefault(x => x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL);
            if (asignacionTemp != null)
            {
                fechaAsignacion = asignacionTemp.fecha_asignacion;
                fechaTermino = asignacionTemp.fecha_cierre != null ? asignacionTemp.fecha_cierre.Value : asignacionTemp.fecha_rechazo != null ? asignacionTemp.fecha_rechazo.Value : now;
                jsonPeridosInicial[0] = new
                {
                    id = "2_0",
                    start = fechaAsignacion.ToString(formatDate),
                    end = fechaTermino.ToString(formatDate)
                    ,
                    fill = asignacionTemp.fecha_cierre != null ? finalizadoColor : asignacionTemp.fecha_rechazo != null ? RechazadoColor : pendienteColor
                };
                jsonData[1] = new { id = "2", name = "Aprobación Inicial", periods = jsonPeridosInicial };

            }
            else
            { //envia vacio 
                jsonData[1] = new { id = "2", name = "Aprobación Inicial" };
            }

            //3.- Obtiene el las asignaciones para el resto de departamentos 
            foreach (var itemDepartamento in listDepartamentos)
            {
                int index = 2 + listDepartamentos.IndexOf(itemDepartamento);


                var jsonPeriodosDepto = new object[solicitud.SCDM_solicitud_asignaciones.Where(x => x.id_departamento_asignacion == itemDepartamento.id
                        && (x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO || x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM)).Count()];

                //recorre las asignaciones al departamento
                int b = 0;
                foreach (var itemAsignacion in solicitud.SCDM_solicitud_asignaciones.Where(x => x.id_departamento_asignacion == itemDepartamento.id
                        && (x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO || x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM)))
                {
                    fechaAsignacion = itemAsignacion.fecha_asignacion;
                    fechaTermino = itemAsignacion.fecha_cierre != null ? itemAsignacion.fecha_cierre.Value : itemAsignacion.fecha_rechazo != null ? itemAsignacion.fecha_rechazo.Value : now;

                    jsonPeriodosDepto[b] = new
                    {
                        id = $"{index}_{b}",
                        start = fechaAsignacion.ToString(formatDate),
                        end = fechaTermino.ToString(formatDate),
                        fill = itemAsignacion.fecha_cierre != null ? finalizadoColor : itemAsignacion.fecha_rechazo != null ? RechazadoColor : pendienteColor
                    };
                    b++;
                }

                //crea variables para los periodos
                var jsonPeridosDepto = new object[asignacionesSolicitante + 1];


                //agrega el departamento con todos los periodos
                jsonData[index] = new { id = index.ToString(), name = itemDepartamento.descripcion, periods = jsonPeriodosDepto };
            }


            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Retorna un json con los datos de la solicitud
        /// </summary>
        /// <returns></returns>      
        public JsonResult CargaMaterialesListaTecnica(int id_solicitud = 0)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var dataCreacionMateriales = db.SCDM_solicitud_rel_item_material.Where(x => x.id_solicitud == id_solicitud).ToList();
            var dataCreacionReferencia = db.SCDM_solicitud_rel_creacion_referencia.Where(x => x.id_solicitud == id_solicitud).ToList();

            //inicializa el arreglo
            var jsonData = new object[dataCreacionMateriales.Count() + dataCreacionReferencia.Count()];

            //llena los valores para los materiales
            for (int i = 0; i < dataCreacionMateriales.Count(); i++)
            {

                jsonData[i] = new[] {
                    dataCreacionMateriales[i].numero_material,
                    dataCreacionMateriales[i].SCDM_cat_tipo_materiales_solicitud!=null? dataCreacionMateriales[i].SCDM_cat_tipo_materiales_solicitud.descripcion:string.Empty,
                    !string.IsNullOrEmpty(dataCreacionMateriales[i].tipo_venta)?  dataCreacionMateriales[i].tipo_venta : string.Empty,
                    dataCreacionMateriales[i].descripcion_material_es,
                    dataCreacionMateriales[i].numero_parte,
                    dataCreacionMateriales[i].peso_bruto.ToString(),
                    dataCreacionMateriales[i].peso_neto.ToString(),
                    !string.IsNullOrEmpty(dataCreacionMateriales[i].unidad_medida_inventario)?  dataCreacionMateriales[i].unidad_medida_inventario : string.Empty,
                    dataCreacionMateriales[i].numero_cintas_resultantes.ToString(),
                    !string.IsNullOrEmpty(dataCreacionMateriales[i].clase_aprovisionamiento)?  dataCreacionMateriales[i].clase_aprovisionamiento : string.Empty,
                    dataCreacionMateriales[i].fecha_validez.HasValue? dataCreacionMateriales[i].fecha_validez.Value.ToShortDateString():string.Empty,
                    };

            }
            //llena los valores para los materiales de cración con referencia
            for (int i = dataCreacionMateriales.Count; i < dataCreacionMateriales.Count + dataCreacionReferencia.Count(); i++)
            {
                int c = i - dataCreacionMateriales.Count;

                jsonData[i] = new[] {
                    dataCreacionReferencia[c].nuevo_material,
                    dataCreacionReferencia[c].tipo_material_text,
                    dataCreacionReferencia[c].SCDM_cat_tipo_venta!=null? dataCreacionReferencia[c].SCDM_cat_tipo_venta.descripcion:string.Empty,
                    string.Empty, //dataMateriales[i].descripcion_material_es,
                    dataCreacionReferencia[c].numero_parte,
                    dataCreacionReferencia[c].peso_bruto.ToString(),
                    dataCreacionReferencia[c].peso_neto.ToString(),
                    !string.IsNullOrEmpty(dataCreacionReferencia[c].unidad_medida_inventario)?  dataCreacionReferencia[c].unidad_medida_inventario : string.Empty,
                    string.Empty, //dataMateriales[i].numero_cintas_resultantes.ToString(),
                    string.Empty,//!string.IsNullOrEmpty(dataMateriales[i].clase_aprovisionamiento)?  dataMateriales[i].clase_aprovisionamiento : string.Empty,
                    string.Empty,//dataMateriales[i].fecha_validez.HasValue? dataMateriales[i].fecha_validez.Value.ToShortDateString():string.Empty,
                    };

            }



            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Retorna un json con los datos de la solicitud
        /// </summary>
        /// <returns></returns>      
        public JsonResult CargaCintas(int id_solicitud = 0)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var data = db.SCDM_solicitud_rel_item_material.Where(x => x.id_solicitud == id_solicitud && x.id_tipo_material == Bitacoras.Util.SCDM_solicitud_rel_item_material_tipo.CINTA).ToList();

            var jsonData = new object[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {

                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    !string.IsNullOrEmpty(data[i].numero_material)? data[i].numero_material:string.Empty,
                    data[i].material_del_cliente.HasValue? data[i].material_del_cliente.Value?"SÍ":"NO":string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_venta)?  data[i].tipo_venta : string.Empty,
                    !string.IsNullOrEmpty(data[i].numero_odc_cliente)?  data[i].numero_odc_cliente : string.Empty,
                    !string.IsNullOrEmpty(data[i].cliente)?  data[i].cliente : string.Empty,
                    data[i].requiere_ppaps.HasValue? data[i].requiere_ppaps.Value?"SÍ":"NO":string.Empty,
                    data[i].requiere_imds.HasValue? data[i].requiere_imds.Value?"SÍ":"NO":string.Empty,
                    !string.IsNullOrEmpty(data[i].proveedor)?  data[i].proveedor : string.Empty,
                    !string.IsNullOrEmpty(data[i].molino)?  data[i].molino : string.Empty,
                    !string.IsNullOrEmpty(data[i].metal)?  data[i].metal : string.Empty,
                    !string.IsNullOrEmpty(data[i].unidad_medida_inventario)?  data[i].unidad_medida_inventario : string.Empty,
                    !string.IsNullOrEmpty(data[i].grado_calidad) ? data[i].grado_calidad : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_material_es) ? data[i].descripcion_material_es : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_material_en) ? data[i].descripcion_material_en : string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_recubrimiento)?  data[i].tipo_recubrimiento : string.Empty,
                    !string.IsNullOrEmpty(data[i].clase_aprovisionamiento)?  data[i].clase_aprovisionamiento : string.Empty,
                    !string.IsNullOrEmpty(data[i].numero_parte) ? data[i].numero_parte : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_numero_parte) ? data[i].descripcion_numero_parte : string.Empty,
                    !string.IsNullOrEmpty(data[i].norma_referencia) ? data[i].norma_referencia : string.Empty,
                    data[i].numero_cintas_resultantes.ToString(),
                    data[i].espesor_mm.ToString(),
                    data[i].espesor_tolerancia_negativa_mm.ToString(),
                    data[i].espesor_tolerancia_positiva_mm.ToString(),
                    data[i].ancho_mm.ToString(),
                    data[i].ancho_entrega_cinta_mm.ToString(),
                    data[i].ancho_tolerancia_negativa_mm.ToString(),
                    data[i].ancho_tolerancia_positiva_mm.ToString(),
                    data[i].diametro_interior.ToString(),
                    data[i].diametro_interior_salida.ToString(),
                    data[i].diametro_exterior_maximo_mm.ToString(),
                    data[i].peso_max_kg.ToString(),
                    !string.IsNullOrEmpty(data[i].peso_recubrimiento)?  data[i].peso_recubrimiento : string.Empty,
                    !string.IsNullOrEmpty(data[i].parte_interior_exterior)?  data[i].parte_interior_exterior : string.Empty,
                    !string.IsNullOrEmpty(data[i].posicion_rollo)?  data[i].posicion_rollo : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_1)?  data[i].ihs_1 : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_2)?  data[i].ihs_2 : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_3)?  data[i].ihs_3 : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_4)?  data[i].ihs_4 : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_5)?  data[i].ihs_5 : string.Empty,
                    !string.IsNullOrEmpty(data[i].modelo_negocio)?  data[i].modelo_negocio : string.Empty,
                    data[i].aplica_procesador_externo.HasValue? data[i].aplica_procesador_externo.Value?"SÍ":"NO":string.Empty,
                    !string.IsNullOrEmpty(data[i].procesador_externo_nombre)?  data[i].procesador_externo_nombre : string.Empty,
                    !String.IsNullOrEmpty(data[i].numero_antiguo_material)? data[i].numero_antiguo_material : string.Empty,
                    data[i].planicidad_mm.ToString(),
                    !String.IsNullOrEmpty(data[i].msa_hoda)?data[i].msa_hoda:string.Empty ,
                     data[i].fecha_validez.HasValue?data[i].fecha_validez.Value.ToString("dd/MM/yyyy"):string.Empty,
                    data[i].requiere_consiliacion_puntas_colar.HasValue? data[i].requiere_consiliacion_puntas_colar.Value?"SÍ":"NO":string.Empty,
                    data[i].scrap_permitido_puntas_colas.ToString(),
                    !string.IsNullOrEmpty(data[i].unidad_medida_ventas)?  data[i].unidad_medida_ventas : string.Empty,

                    };
            }

            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Retorna un json con los datos de la solicitud
        /// </summary>
        /// <returns></returns>      
        public JsonResult CargaCB(int id_solicitud = 0)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var data = db.SCDM_solicitud_rel_item_material.Where(x => x.id_solicitud == id_solicitud && x.id_tipo_material == Bitacoras.Util.SCDM_solicitud_rel_item_material_tipo.C_B).ToList();

            var jsonData = new object[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {

                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    !string.IsNullOrEmpty(data[i].numero_material)? data[i].numero_material:string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_venta)?  data[i].tipo_venta : string.Empty,
                    !string.IsNullOrEmpty(data[i].molino)?  data[i].molino : string.Empty,
                    !string.IsNullOrEmpty(data[i].cliente)?  data[i].cliente : string.Empty,
                    data[i].material_compra_tkmm.HasValue? data[i].material_compra_tkmm.Value?"SÍ":"NO":string.Empty,
                    !string.IsNullOrEmpty(data[i].materia_prima_producto_terminado)?  data[i].materia_prima_producto_terminado : string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_metal_cb)?  data[i].tipo_metal_cb : string.Empty,
                    !string.IsNullOrEmpty(data[i].unidad_medida_inventario)?  data[i].unidad_medida_inventario : string.Empty,
                    !string.IsNullOrEmpty(data[i].numero_parte) ? data[i].numero_parte : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_numero_parte) ? data[i].descripcion_numero_parte : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_material_es) ? data[i].descripcion_material_es : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_material_en) ? data[i].descripcion_material_en : string.Empty,
                    !string.IsNullOrEmpty(data[i].grado_calidad) ? data[i].grado_calidad : string.Empty,
                    !string.IsNullOrEmpty(data[i].clase_aprovisionamiento)?  data[i].clase_aprovisionamiento : string.Empty,
                    data[i].espesor_mm.ToString(),
                    data[i].espesor_tolerancia_negativa_mm.ToString(),
                    data[i].espesor_tolerancia_positiva_mm.ToString(),
                    data[i].ancho_mm.ToString(),
                    data[i].ancho_tolerancia_negativa_mm.ToString(),
                    data[i].ancho_tolerancia_positiva_mm.ToString(),
                    data[i].avance_mm.ToString(),
                    data[i].avance_tolerancia_negativa_mm.ToString(),
                    data[i].avance_tolerancia_positiva_mm.ToString(),
                    data[i].peso_bruto.ToString(),
                    data[i].peso_neto.ToString(),
                    !string.IsNullOrEmpty(data[i].aleacion)?  data[i].aleacion : string.Empty,
                    !string.IsNullOrEmpty(data[i].modelo_negocio)?  data[i].modelo_negocio : string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_transito)?  data[i].tipo_transito : string.Empty,
                    !string.IsNullOrEmpty(data[i].proveedor)?  data[i].proveedor : string.Empty,
                     data[i].precio.ToString(),
                      !string.IsNullOrEmpty(data[i].moneda)?  data[i].moneda : string.Empty,
                      !string.IsNullOrEmpty(data[i].incoterm)?  data[i].incoterm : string.Empty,
                      !string.IsNullOrEmpty(data[i].terminos_pago)?  data[i].terminos_pago : string.Empty,
                      data[i].aplica_tasa_iva.HasValue? data[i].aplica_tasa_iva.Value?"SÍ":"NO":string.Empty,

                    };
            }

            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Retorna un json con los datos de la solicitud
        /// </summary>
        /// <returns></returns>      
        public JsonResult CargaPlatinas(int tipoPlatina = 0, int id_solicitud = 0)
        {
            //obtiene el listado de item tipo rollo de la solicitud
            var data = db.SCDM_solicitud_rel_item_material.Where(x => x.id_solicitud == id_solicitud && x.id_tipo_material == tipoPlatina).ToList();

            var jsonData = new object[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {

                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    !string.IsNullOrEmpty(data[i].numero_material)? data[i].numero_material:string.Empty,
                    data[i].material_del_cliente.HasValue? data[i].material_del_cliente.Value?"SÍ":"NO":string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_venta)?  data[i].tipo_venta : string.Empty,
                    !string.IsNullOrEmpty(data[i].numero_odc_cliente)?  data[i].numero_odc_cliente : string.Empty,
                    !string.IsNullOrEmpty(data[i].cliente)?  data[i].cliente : string.Empty,
                    data[i].requiere_ppaps.HasValue? data[i].requiere_ppaps.Value?"SÍ":"NO":string.Empty,
                    data[i].requiere_imds.HasValue? data[i].requiere_imds.Value?"SÍ":"NO":string.Empty,
                    !string.IsNullOrEmpty(data[i].metal)?  data[i].metal : string.Empty,
                    !string.IsNullOrEmpty(data[i].unidad_medida_inventario)?  data[i].unidad_medida_inventario : string.Empty,
                    !string.IsNullOrEmpty(data[i].grado_calidad) ? data[i].grado_calidad : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_material_es) ? data[i].descripcion_material_es : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_material_en) ? data[i].descripcion_material_en : string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_recubrimiento)?  data[i].tipo_recubrimiento : string.Empty,
                    !string.IsNullOrEmpty(data[i].clase_aprovisionamiento)?  data[i].clase_aprovisionamiento : string.Empty,
                    !string.IsNullOrEmpty(data[i].numero_parte) ? data[i].numero_parte : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_numero_parte) ? data[i].descripcion_numero_parte : string.Empty,
                    !string.IsNullOrEmpty(data[i].norma_referencia) ? data[i].norma_referencia : string.Empty,
                    data[i].espesor_mm.ToString(),
                    data[i].espesor_tolerancia_negativa_mm.ToString(),
                    data[i].espesor_tolerancia_positiva_mm.ToString(),
                    data[i].ancho_mm.ToString(),
                    data[i].ancho_tolerancia_negativa_mm.ToString(),
                    data[i].ancho_tolerancia_positiva_mm.ToString(),
                    data[i].avance_mm.ToString(),
                    data[i].avance_tolerancia_negativa_mm.ToString(),
                    data[i].avance_tolerancia_positiva_mm.ToString(),
                    !string.IsNullOrEmpty(data[i].forma)?  data[i].forma : string.Empty,
                    data[i].piezas_por_golpe.ToString(),
                    data[i].piezas_por_paquete.ToString(),
                    data[i].piezas_por_auto.ToString(),
                    data[i].peso_bruto.ToString(),
                    data[i].peso_neto.ToString(),
                    !string.IsNullOrEmpty(data[i].peso_recubrimiento)?  data[i].peso_recubrimiento : string.Empty,
                    !string.IsNullOrEmpty(data[i].parte_interior_exterior)?  data[i].parte_interior_exterior : string.Empty,
                    data[i].peso_inicial.ToString(),
                    data[i].porcentaje_scrap_puntas_colas.ToString(),
                    !string.IsNullOrEmpty(data[i].ihs_1)?  data[i].ihs_1 : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_2)?  data[i].ihs_2 : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_3)?  data[i].ihs_3 : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_4)?  data[i].ihs_4 : string.Empty,
                    !string.IsNullOrEmpty(data[i].ihs_5)?  data[i].ihs_5 : string.Empty,
                    !string.IsNullOrEmpty(data[i].molino)?  data[i].molino : string.Empty,
                    !string.IsNullOrEmpty(data[i].modelo_negocio)?  data[i].modelo_negocio : string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_transito)?  data[i].tipo_transito : string.Empty,
                    data[i].aplica_procesador_externo.HasValue? data[i].aplica_procesador_externo.Value?"SÍ":"NO":string.Empty,
                    !string.IsNullOrEmpty(data[i].procesador_externo_nombre)?  data[i].procesador_externo_nombre : string.Empty,
                    !String.IsNullOrEmpty(data[i].numero_antiguo_material)? data[i].numero_antiguo_material : string.Empty,
                    data[i].planicidad_mm.ToString(),
                    !String.IsNullOrEmpty(data[i].msa_hoda)?data[i].msa_hoda:string.Empty ,
                    data[i].requiere_consiliacion_puntas_colar.HasValue? data[i].requiere_consiliacion_puntas_colar.Value?"SÍ":"NO":string.Empty,
                    data[i].scrap_permitido_puntas_colas.ToString(),
                    data[i].conciliacion_scrap_ingenieria.HasValue? data[i].conciliacion_scrap_ingenieria.Value?"SÍ":"NO":string.Empty,
                    data[i].fecha_validez.HasValue?data[i].fecha_validez.Value.ToString("dd/MM/yyyy"):string.Empty,
                    data[i].angulo_a.ToString(),
                    data[i].angulo_b.ToString(),

                    };
            }

            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Obtiene los datos del material segun el material y planta
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public JsonResult ObtieneValoresMaterialReferencia(string material, string plantaSolicitud)
        {

            material = material.ToUpper();

            mm_v3 mm = null;

            //obtiene todos los posibles valores
            var mmList = db.mm_v3.Where(x => x.Material == material);

            //trata de obtener el valor que corresponde a la planta
            if (mmList.Any(x => x.Plnt == plantaSolicitud))
                mm = mmList.FirstOrDefault(x => x.Plnt == plantaSolicitud);
            else
            {
                mm = mmList.FirstOrDefault();
            }


            class_v3 class_v3 = db.class_v3.FirstOrDefault(x => x.Object == material);

            //declatracion de variables
            string commodityString = string.Empty;
            string shapeString = string.Empty;
            string clienteString = string.Empty;
            string tipoMetalString = string.Empty;
            string tipoMaterialString = String.Empty;
            string typeSellingString = String.Empty;
            string modeloNegocioString = String.Empty;
            string posicionRolloString = String.Empty;
            string IHS1String = String.Empty;
            string IHS2String = String.Empty;
            string IHS3String = String.Empty;
            string IHS4String = String.Empty;
            string IHS5String = String.Empty;
            string espesor = string.Empty;
            string espesor_tolerancia_negativa = string.Empty;
            string espesor_tolerancia_positiva = string.Empty;
            string ancho = string.Empty;
            string avance = string.Empty;
            string plantaString = string.Empty;
            string piezas_por_paquete_string = string.Empty;

            //inicializa la lista de objetos
            var objeto = new object[1];

            //retorna que no se encontró el material
            if (mm == null)
            {
                objeto[0] = new
                {
                    existe = "0",
                    mensaje = "No se encontró referencia al material " + material + "." +
                    "",
                };

                return Json(objeto, JsonRequestBehavior.AllowGet);
            }


            if (class_v3 == null)
                class_v3 = new class_v3
                {
                    Object = mm.Material,
                    Grade = string.Empty,
                    Customer = string.Empty,
                    Shape = string.Empty,
                    Customer_part_number = string.Empty,
                    Surface = string.Empty,
                    Gauge___Metric = string.Empty,
                    Mill = string.Empty,
                    Width___Metr = string.Empty,
                    Length_mm_ = string.Empty,
                    activo = true,
                    commodity = string.Empty,
                    flatness_metric = string.Empty,
                    surface_treatment = string.Empty,
                    coating_weight = string.Empty,
                    customer_part_msa = string.Empty,
                    outer_diameter_coil = string.Empty,
                    inner_diameter_coil = string.Empty
                };
            else //establece los valores 
            {
                SCDM_cat_commodity commodity = db.SCDM_cat_commodity.FirstOrDefault(x => x.descripcion == class_v3.commodity);
                commodityString = commodity != null ? commodity.ConcatCommodity : string.Empty;

                //SHAPE
                SCDM_cat_forma_material shape = db.SCDM_cat_forma_material.FirstOrDefault(x => x.descripcion_en == class_v3.Shape);
                shapeString = shape != null ? shape.descripcion : string.Empty;

                //Cliente
                clientes cliente = db.clientes.FirstOrDefault(x => x.claveSAP == class_v3.Customer);
                clienteString = cliente != null ? cliente.ConcatClienteSAP : string.Empty;

                //type of selling
                SCDM_cat_tipo_venta tipoVenta = db.SCDM_cat_tipo_venta.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.Type_of_Selling.ToUpper());
                typeSellingString = tipoVenta != null ? tipoVenta.descripcion : mm.Type_of_Selling;

                //modelo de negocio
                SCDM_cat_modelo_negocio modeloNegocio = db.SCDM_cat_modelo_negocio.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.Business_Model.ToUpper());
                modeloNegocioString = modeloNegocio != null ? modeloNegocio.descripcion : mm.Business_Model;

                //posicion
                SCDM_cat_posicion_rollo_embarques posicionRollo = db.SCDM_cat_posicion_rollo_embarques.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.coil_position.ToUpper());
                posicionRolloString = posicionRollo != null ? posicionRollo.descripcion : mm.coil_position;

                //tipo de metal
                SCDM_cat_tipo_metal tipoMetal = db.SCDM_cat_tipo_metal.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.Type_of_Metal.ToUpper());
                if (tipoMetal != null)
                {
                    tipoMetalString = tipoMetal.descripcion;
                }
                else
                {
                    SCDM_cat_tipo_metal_cb tipoMetalCB = db.SCDM_cat_tipo_metal_cb.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.Type_of_Metal.ToUpper());
                    tipoMetalString = tipoMetalCB != null ? tipoMetalCB.descripcion : string.Empty;
                }

                //IHS 1
                SCDM_cat_ihs ihs1 = db.SCDM_cat_ihs.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.IHS_number_1.ToUpper());
                IHS1String = ihs1 != null ? ihs1.descripcion : mm.IHS_number_1;
                //IHS 2
                SCDM_cat_ihs ihs2 = db.SCDM_cat_ihs.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.IHS_number_2.ToUpper());
                IHS2String = ihs2 != null ? ihs2.descripcion : mm.IHS_number_2;
                //IHS 3
                SCDM_cat_ihs ihs3 = db.SCDM_cat_ihs.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.IHS_number_3.ToUpper());
                IHS3String = ihs3 != null ? ihs3.descripcion : mm.IHS_number_3;
                //IHS 4
                SCDM_cat_ihs ihs4 = db.SCDM_cat_ihs.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.IHS_number_4.ToUpper());
                IHS4String = ihs4 != null ? ihs4.descripcion : mm.IHS_number_4;
                //IHS 5
                SCDM_cat_ihs ihs5 = db.SCDM_cat_ihs.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.IHS_number_5.ToUpper());
                IHS5String = ihs5 != null ? ihs5.descripcion : mm.IHS_number_5;


                //obtiene dimensiones
                string[] dimensiones = !string.IsNullOrEmpty(mm.size_dimensions) ? mm.size_dimensions.Replace(" ", string.Empty).ToUpper().Split('X') : new string[0];

                if (dimensiones.Length >= 1)
                    espesor = dimensiones[0];
                if (dimensiones.Length >= 2)
                    ancho = dimensiones[1];
                if (dimensiones.Length >= 3)
                    avance = dimensiones[2];

                if (int.TryParse(mm.Package_Pieces, out int piezas_por_paquete_result))
                    piezas_por_paquete_string = piezas_por_paquete_result.ToString();
            }

            //valor para planta
            var planta = db.plantas.FirstOrDefault(x => x.codigoSap == mm.Plnt);
            plantaString = planta != null ? planta.ConcatPlantaSap : string.Empty;

            objeto[0] = new
            {
                existe = "1",
                mensaje = "Se cargó correctamente",
                planta = mm.Plnt,
                planta_string = plantaString,
                numero_antiguo_material = mm.Old_material_no_,
                peso_bruto = mm.Gross_weight.ToString(),
                peso_neto = mm.Net_weight.ToString(),
                unidad_base_medida = mm.unidad_medida,
                commodity = commodityString,
                grado = class_v3.Grade,
                espesor = espesor,
                espesor_min = getTolerancia(class_v3.Gauge___Metric, espesor, "min"),
                espesor_max = getTolerancia(class_v3.Gauge___Metric, espesor, "max"),
                ancho = ancho,
                ancho_min = getTolerancia(class_v3.Width___Metr, ancho, "min"),
                ancho_max = getTolerancia(class_v3.Width___Metr, ancho, "max"),
                avance = avance,
                avance_min = getTolerancia(class_v3.Length_mm_, avance, "min"),
                avance_max = getTolerancia(class_v3.Length_mm_, avance, "max"),
                planicidad = String.Concat(class_v3.flatness_metric.Where(x => x == '.' || Char.IsDigit(x))),
                superficie = class_v3.Surface,
                tratamiento_superficial = class_v3.surface_treatment,
                peso_recubrimiento = class_v3.coating_weight,
                molino = class_v3.Mill,
                forma = shapeString,
                cliente = clienteString,
                numero_parte_cliente = class_v3.Customer_part_number,
                msa = class_v3.customer_part_msa,
                diametro_interior = !string.IsNullOrEmpty(class_v3.inner_diameter_coil) ? float.Parse(String.Concat(class_v3.inner_diameter_coil.Where(x => x == '.' || Char.IsDigit(x)))).ToString() : string.Empty,
                diametro_exterior = String.Concat(class_v3.outer_diameter_coil.Where(x => x == '.' || Char.IsDigit(x))),
                descripcion_es = mm.material_descripcion_es,
                descripcion_en = mm.Material_Description,
                tipo_metal = tipoMetalString,
                tipo_material = mm.Type_of_Material,
                type_selling = typeSellingString,
                modelo_negocio = modeloNegocioString,
                posicionRollo = posicionRolloString,
                ihs_1 = IHS1String,
                ihs_2 = IHS2String,
                ihs_3 = IHS3String,
                ihs_4 = IHS4String,
                ihs_5 = IHS5String,
                peso_bruto_real_bascula = mm.real_gross_weight == 0 ? string.Empty : mm.real_gross_weight.ToString(),
                peso_neto_real_bascula = mm.real_net_weight == 0 ? string.Empty : mm.real_net_weight.ToString(),
                angulo_a = mm.angle_a == 0 ? string.Empty : mm.angle_a.ToString(),
                angulo_b = mm.angle_b == 0 ? string.Empty : mm.angle_b.ToString(),
                scrap_permitido_puntas_colas = mm.Head_and_Tail_allowed_scrap == 0 ? string.Empty : mm.Head_and_Tail_allowed_scrap.ToString(),
                piezas_dobles = mm.double_pieces,
                reaplicacion = !string.IsNullOrEmpty(mm.Re_application) ? "true" : "false",
                conciliacion_puntas_colas = !string.IsNullOrEmpty(mm.Head_and_Tails_Scrap_Conciliation) ? "true" : "false",
                conciliacion_scrap_ingenieria = !string.IsNullOrEmpty(mm.Engineering_Scrap_conciliation) ? "true" : "false",
                piezas_por_auto = mm.Pieces_per_car == 0 ? string.Empty : mm.Pieces_per_car.ToString(),
                piezas_por_golpe = mm.num_piezas_golpe == 0 ? string.Empty : mm.num_piezas_golpe.ToString(),
                piezas_por_paquete = piezas_por_paquete_string == "0" ? string.Empty : piezas_por_paquete_string,
                peso_inicial = mm.Initial_Weight == 0 ? string.Empty : mm.Initial_Weight.ToString(),
                peso_maximo = mm.Maximum_Weight == 0 ? string.Empty : mm.Maximum_Weight.ToString(),
                peso_maximo_tolerancia_positiva = mm.maximum_weight_tol_positive.ToString(),
                peso_maximo_tolerancia_negativa = mm.maximum_weight_tol_negative.ToString(),
                peso_minimo = mm.Min_Weight == 0 ? string.Empty : mm.Min_Weight.ToString(),
                peso_minimo_tolerancia_positiva = mm.minimum_weight_tol_positive.ToString(),
                peso_minimo_tolerancia_negativa = mm.minimum_weight_tol_negative.ToString(),
            };

            return Json(objeto, JsonRequestBehavior.AllowGet);

        }

        [NonAction]
        public string getTolerancia(string tolerancias, string valor, string tipo)
        {
            string resultado = string.Empty;
            string[] toleranciasSplit = !string.IsNullOrEmpty(tolerancias) ? tolerancias.Replace(" ", string.Empty).ToUpper().Split('-') : new string[0];

            switch (tipo)
            {
                case "min":
                    if (toleranciasSplit.Count() >= 1)
                    {
                        string soloNumero = String.Concat(toleranciasSplit[0].Where(x => x == '.' || Char.IsDigit(x)));
                        bool convierteValor = double.TryParse(valor, out double valorDouble);
                        bool convierteTolerancia = double.TryParse(soloNumero, out double toleranciaDouble);

                        if (convierteValor && convierteTolerancia)
                        {
                            return Math.Round(toleranciaDouble - valorDouble, 3).ToString();
                        }
                    }
                    break;
                case "max":

                    if (toleranciasSplit.Count() >= 2)
                    {
                        string soloNumero = String.Concat(toleranciasSplit[1].Where(x => x == '.' || Char.IsDigit(x)));
                        bool convierteValor = double.TryParse(valor, out double valorDouble);
                        bool convierteTolerancia = double.TryParse(soloNumero, out double toleranciaDouble);

                        if (convierteValor && convierteTolerancia)
                        {
                            return Math.Round(toleranciaDouble - valorDouble, 3).ToString();
                        }
                    }//si sólo hay una tolerancia toma la primera
                    else if (toleranciasSplit.Count() == 1)
                    {
                        return "0";
                    }
                    break;
                default:
                    return string.Empty;

            }

            return resultado;
        }

        [NonAction]

        private List<SCDM_solicitud_rel_item_material> ConvierteArrayARollo(List<string[]> data, int? id_solicitud)
        {
            List<SCDM_solicitud_rel_item_material> resultado = new List<SCDM_solicitud_rel_item_material> { };


            //listado de encabezados
            string[] encabezados = {"ID", "Número Material", "Material del cliente", "Tipo de Venta", "Núm. ODC cliente", "Núm. cliente", "Requiere PPAPs", "Req. IMDS", "Proveedor", "Nombre Molino",
            "Tipo Metal", "Unidad Base Medida","Grado/Calidad", "Descripción Material (ES)", "Descripción Material (EN)",  "Tipo de Material", "¿Aprovisionamiento?", "Núm. parte del cliente",
            "Descripción núm. de parte", "Norma de referencia", "Espesor (mm)", "Tolerancia espesor negativa (mm)", "Tolerancia espesor positiva (mm)", "Ancho (mm)", "Tolerancia ancho negativa (mm)",
            "Tolerancia ancho positiva (mm)", "Diametro interior (mm)", "Diametro exterior máximo (mm)", "Peso Min (KG)", "Peso Max (KG)", "Peso del recubrimiento", "Parte Int/Ext", "Posición del Rollo para embarque",
            "Programa IHS 1", "Programa IHS 2", "Programa IHS 3", "Programa IHS 4", "Programa IHS 5", "Modelo de negocio", "Transito", "Procesadores Ext.", "Número procesador Ext.", "Núm. antigüo material",
            "Planicidad (mm)", "MSA (Honda)", "Req. conciliación Puntas y colas", "Scrap permitido (%)", "Fecha validez", "Disponente asignado", "Unidad Medida Ventas"
            };



            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                int? diametro_interior = null;
                int id_rollo = 0;
                bool? material_del_cliente = null, requiere_ppaps = null, requiere_imds = null, procesador_externo = null, requiere_consiliacion_puntas_colar = null;
                double? espesor_mm = null, espesor_tolerancia_negativa_mm = null, espesor_tolerancia_positiva_mm = null, ancho_mm = null, ancho_tolerancia_negativa_mm = null, ancho_tolerancia_positiva_mm = null,
                    diametro_exterior_maximo_mm = null, peso_min_kg = null, peso_max_kg = null, planicidad_mm = null, scrap_permitido_puntas_colas = null;
                DateTime? fecha_validez = null;

                #region Asignacion de variables
                //id_rollo
                if (int.TryParse(array[Array.IndexOf(encabezados, "ID")], out int id_rol))
                    id_rollo = id_rol;

                //valida el material del cliente
                if (array[Array.IndexOf(encabezados, "Material del cliente")] == "SÍ")
                    material_del_cliente = true;
                else if (array[Array.IndexOf(encabezados, "Material del cliente")] == "NO")
                    material_del_cliente = false;

                //requiere ppaps
                //valida el material del cliente
                if (array[Array.IndexOf(encabezados, "Requiere PPAPs")] == "SÍ")
                    requiere_ppaps = true;
                else if (array[Array.IndexOf(encabezados, "Requiere PPAPs")] == "NO")
                    requiere_ppaps = false;

                //requiere requiere_imds
                //valida el material del cliente
                if (array[Array.IndexOf(encabezados, "Req. IMDS")] == "SÍ")
                    requiere_imds = true;
                else if (array[Array.IndexOf(encabezados, "Req. IMDS")] == "NO")
                    requiere_imds = false;

                //diametro interior
                if (int.TryParse(array[Array.IndexOf(encabezados, "Diametro interior (mm)")], out int diametro_interior_result))
                    diametro_interior = diametro_interior_result;

                //espesor_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Espesor (mm)")], out double espesor_mm_result))
                    espesor_mm = espesor_mm_result;

                //espesor_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia espesor negativa (mm)")], out double espesor_tolerancia_negativa_mm_result))
                    espesor_tolerancia_negativa_mm = espesor_tolerancia_negativa_mm_result;

                //espesor_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia espesor positiva (mm)")], out double espesor_tolerancia_positiva_mm_result))
                    espesor_tolerancia_positiva_mm = espesor_tolerancia_positiva_mm_result;

                //ancho_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Ancho (mm)")], out double ancho_mm_result))
                    ancho_mm = ancho_mm_result;

                //ancho_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia ancho negativa (mm)")], out double ancho_tolerancia_negativa_mm_result))
                    ancho_tolerancia_negativa_mm = ancho_tolerancia_negativa_mm_result;

                //ancho_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia ancho positiva (mm)")], out double ancho_tolerancia_positiva_mm_result))
                    ancho_tolerancia_positiva_mm = ancho_tolerancia_positiva_mm_result;

                //diametro_exterior_maximo_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Diametro exterior máximo (mm)")], out double diametro_exterior_maximo_mm_result))
                    diametro_exterior_maximo_mm = diametro_exterior_maximo_mm_result;
                //peso_min_kg
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Min (KG)")], out double peso_min_kg_result))
                    peso_min_kg = peso_min_kg_result;
                //peso_max_kg
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Max (KG)")], out double peso_max_kg_result))
                    peso_max_kg = peso_max_kg_result;


                //valida el procesador externo
                if (array[Array.IndexOf(encabezados, "Procesadores Ext.")] == "SÍ")
                    procesador_externo = true;
                else if (array[Array.IndexOf(encabezados, "Procesadores Ext.")] == "NO")
                    procesador_externo = false;


                //planicidad
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Planicidad (mm)")], out double planicidad_mm_result))
                    planicidad_mm = planicidad_mm_result;

                //req. conciliación puntas y colas
                if (array[Array.IndexOf(encabezados, "Req. conciliación Puntas y colas")] == "SÍ")
                    requiere_consiliacion_puntas_colar = true;
                else if (array[Array.IndexOf(encabezados, "Req. conciliación Puntas y colas")] == "NO")
                    requiere_consiliacion_puntas_colar = false;

                //scrap_permitido_puntas_colas
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Scrap permitido (%)")], out double scrap_permitido_puntas_colas_result))
                    scrap_permitido_puntas_colas = scrap_permitido_puntas_colas_result;

                //fecha_validez
                if (DateTime.TryParseExact(array[Array.IndexOf(encabezados, "Fecha validez")], "dd/MM/yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out DateTime fecha_validez_result))
                    fecha_validez = fecha_validez_result;

                #endregion

                //conserva el valor del tratamiento ingresado desde el archivo excel
                string tratamiento = null;
                using (var dbEntity = new Portal_2_0Entities())
                {
                    if (id_rollo > 0)
                        tratamiento = dbEntity.SCDM_solicitud_rel_item_material.Find(id_rollo).tratamiento_superficial;
                }

                resultado.Add(new SCDM_solicitud_rel_item_material
                {
                    num_fila = data.IndexOf(array),
                    id_solicitud = id_solicitud.Value,
                    id_tipo_material = 1,   // 1 --> ROLLO
                    id = id_rollo,                        //id = data[0],
                    numero_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Número Material")]) ? array[Array.IndexOf(encabezados, "Número Material")] : null,  //data[9] readonly
                    material_del_cliente = material_del_cliente,  //data[1]
                    tipo_venta = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de Venta")]) ? array[Array.IndexOf(encabezados, "Tipo de Venta")] : null,              //data[2]
                    numero_odc_cliente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. ODC cliente")]) ? array[Array.IndexOf(encabezados, "Núm. ODC cliente")] : null,                  //data[3]
                    cliente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. cliente")]) ? array[Array.IndexOf(encabezados, "Núm. cliente")] : null,        //data[4]
                    requiere_ppaps = requiere_ppaps,  //data[5]
                    requiere_imds = requiere_imds,  //data[6]
                    proveedor = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Proveedor")]) ? array[Array.IndexOf(encabezados, "Proveedor")] : null, //data[7]
                    molino = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Nombre Molino")]) ? array[Array.IndexOf(encabezados, "Nombre Molino")] : null, //data[8]
                    metal = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo Metal")]) ? array[Array.IndexOf(encabezados, "Tipo Metal")] : null, //data[10]
                    unidad_medida_inventario = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Unidad Base Medida")]) ? array[Array.IndexOf(encabezados, "Unidad Base Medida")] : null, //data[11]
                    descripcion_material_es = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción Material (ES)")]) ? array[Array.IndexOf(encabezados, "Descripción Material (ES)")] : null,  //data[12] readonly
                    descripcion_material_en = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción Material (EN)")]) ? array[Array.IndexOf(encabezados, "Descripción Material (EN)")] : null,  //data[13] readonly
                    grado_calidad = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Grado/Calidad")]) ? array[Array.IndexOf(encabezados, "Grado/Calidad")] : null,  //data[14] 
                    tipo_recubrimiento = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de Material")]) ? array[Array.IndexOf(encabezados, "Tipo de Material")] : null, //data[15]
                    clase_aprovisionamiento = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "¿Aprovisionamiento?")]) ? array[Array.IndexOf(encabezados, "¿Aprovisionamiento?")] : null,  //data[16]
                    numero_parte = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. parte del cliente")]) ? array[Array.IndexOf(encabezados, "Núm. parte del cliente")] : null,  //data[17]
                    descripcion_numero_parte = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción núm. de parte")]) ? array[Array.IndexOf(encabezados, "Descripción núm. de parte")] : null,  //data[18]
                    norma_referencia = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Norma de referencia")]) ? array[Array.IndexOf(encabezados, "Norma de referencia")] : null,  //data[19]
                    espesor_mm = espesor_mm, //data[20]
                    espesor_tolerancia_negativa_mm = espesor_tolerancia_negativa_mm, //data[21]
                    espesor_tolerancia_positiva_mm = espesor_tolerancia_positiva_mm, //data[22]
                    ancho_mm = ancho_mm, //data[23]
                    ancho_tolerancia_negativa_mm = ancho_tolerancia_negativa_mm, //data[24]
                    ancho_tolerancia_positiva_mm = ancho_tolerancia_positiva_mm, //data[25]
                    diametro_interior = diametro_interior, //data[26]
                    diametro_exterior_maximo_mm = diametro_exterior_maximo_mm,//data[27]
                    peso_min_kg = peso_min_kg, //data[28]
                    peso_max_kg = peso_max_kg, //data[29]
                    peso_recubrimiento = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Peso del recubrimiento")]) ? array[Array.IndexOf(encabezados, "Peso del recubrimiento")] : null, //data[30]
                    parte_interior_exterior = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Parte Int/Ext")]) ? array[Array.IndexOf(encabezados, "Parte Int/Ext")] : null, //data[31]
                    posicion_rollo = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Posición del Rollo para embarque")]) ? array[Array.IndexOf(encabezados, "Posición del Rollo para embarque")] : null, //data[32]
                    ihs_1 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 1")]) ? array[Array.IndexOf(encabezados, "Programa IHS 1")] : null, //data[33]
                    ihs_2 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 2")]) ? array[Array.IndexOf(encabezados, "Programa IHS 2")] : null, //data[33]
                    ihs_3 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 3")]) ? array[Array.IndexOf(encabezados, "Programa IHS 3")] : null, //data[33]
                    ihs_4 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 4")]) ? array[Array.IndexOf(encabezados, "Programa IHS 4")] : null, //data[33]
                    ihs_5 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 5")]) ? array[Array.IndexOf(encabezados, "Programa IHS 5")] : null, //data[33]       
                    modelo_negocio = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Modelo de negocio")]) ? array[Array.IndexOf(encabezados, "Modelo de negocio")] : null, //data[38]
                    tipo_transito = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Transito")]) ? array[Array.IndexOf(encabezados, "Transito")] : null, //data[39]
                    aplica_procesador_externo = procesador_externo, //data[40]
                    procesador_externo_nombre = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Número procesador Ext.")]) ? array[Array.IndexOf(encabezados, "Número procesador Ext.")] : null, //data[41]
                    numero_antiguo_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. antigüo material")]) ? array[Array.IndexOf(encabezados, "Núm. antigüo material")] : null,  //data[42]
                    planicidad_mm = planicidad_mm, //data[43]
                    msa_hoda = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "MSA (Honda)")]) ? array[Array.IndexOf(encabezados, "MSA (Honda)")] : null,  //data[44]
                    requiere_consiliacion_puntas_colar = requiere_consiliacion_puntas_colar, //data[45]
                    scrap_permitido_puntas_colas = scrap_permitido_puntas_colas, //data[46]
                    fecha_validez = fecha_validez, //data[47]
                    disponente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Disponente asignado")]) ? array[Array.IndexOf(encabezados, "Disponente asignado")] : null, //data[48]
                    unidad_medida_ventas = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Unidad Medida Ventas")]) ? array[Array.IndexOf(encabezados, "Unidad Medida Ventas")] : null, //data[11]
                    tratamiento_superficial = tratamiento,
                });
            }
            return resultado;
        }

        private List<SCDM_solicitud_rel_item_material> ConvierteArrayAPlatina(List<string[]> data, int? id_solicitud, int tipoPlatina)
        {
            List<SCDM_solicitud_rel_item_material> resultado = new List<SCDM_solicitud_rel_item_material> { };


            //listado de encabezados
            string[] encabezados = {"ID", "Número Material", "Material del cliente", "Tipo de Venta", "Núm. ODC cliente", "Núm. cliente", "Requiere PPAP's", "Req. IMDS",
            "Tipo Metal", "Unidad Base Medida", "Grado/Calidad", "Descripción Material (ES)", "Descripción Material (EN)",  "Tipo de Material", "¿Aprovisionamiento?",
            "Núm. parte del cliente", "Descripción núm. de parte", "Norma de referencia", "Espesor (mm)", "Tolerancia espesor negativa (mm)",
            "Tolerancia espesor positiva (mm)", "Ancho(mm)", "Tolerancia ancho negativa (mm)",
            "Tolerancia ancho positiva (mm)", "Avance (mm)", "Tolerancia avance negativa (mm)", "Tolerancia avance positiva (mm)", "Forma", "Pzas por Golpe",
            "Pzas por paquete", "Piezas por auto", "Peso Bruto (KG)", "Peso Neto (KG)", "Peso del recubrimiento", "Parte Int/Ext", "Peso Inicial",
            "% de Scrap (puntas y colas)",  "Programa IHS 1", "Programa IHS 2", "Programa IHS 3", "Programa IHS 4", "Programa IHS 5",
            "Nombre Molino", "Modelo de negocio", "Transito" , "Procesadores Ext.", "Número procesador Ext.", "Núm. antigüo material",
            "Planicidad (mm)", "MSA (Honda)", "Req. conciliación Puntas y colas", "Scrap permitido (%)","Conciliacion Scrap Ingeniería", "Fecha validez",
            "Trapecio: ángulo A", "Trapecio: ángulo B"
            };

            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                //declaración de variables
                int? piezas_por_golpe = null, piezas_por_paquete = null, piezas_por_auto = null, angulo_a = null, angulo_b = null;
                int id_platina = 0;
                bool? material_del_cliente = null, requiere_ppaps = null, requiere_imds = null, procesador_externo = null, requiere_consiliacion_puntas_colar = null,
                    scrap_conciliacion_ingenieria = null;
                double? espesor_mm = null, espesor_tolerancia_negativa_mm = null, espesor_tolerancia_positiva_mm = null, ancho_mm = null, ancho_tolerancia_negativa_mm = null, ancho_tolerancia_positiva_mm = null,
                    avance_mm = null, avance_tolerancia_negativa_mm = null, avance_tolerancia_positiva_mm = null,
                    planicidad_mm = null, scrap_permitido_puntas_colas = null, peso_bruto = null, peso_neto = null,
                    peso_inicial = null, porcentaje_scrap_puntas_colas = null
                    ;
                DateTime? fecha_validez = null;

                #region Asignacion de variables
                //id_platina
                if (int.TryParse(array[Array.IndexOf(encabezados, "ID")], out int id_x))
                    id_platina = id_x;

                //valida el material del cliente
                if (array[Array.IndexOf(encabezados, "Material del cliente")] == "SÍ")
                    material_del_cliente = true;
                else if (array[Array.IndexOf(encabezados, "Material del cliente")] == "NO")
                    material_del_cliente = false;


                //requiere ppaps
                //valida el material del cliente
                if (array[Array.IndexOf(encabezados, "Requiere PPAP's")] == "SÍ")
                    requiere_ppaps = true;
                else if (array[Array.IndexOf(encabezados, "Requiere PPAP's")] == "NO")
                    requiere_ppaps = false;

                //requiere requiere_imds
                //valida el material del cliente
                if (array[Array.IndexOf(encabezados, "Req. IMDS")] == "SÍ")
                    requiere_imds = true;
                else if (array[Array.IndexOf(encabezados, "Req. IMDS")] == "NO")
                    requiere_imds = false;
                //espesor_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Espesor (mm)")], out double espesor_mm_result))
                    espesor_mm = espesor_mm_result;

                //espesor_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia espesor negativa (mm)")], out double espesor_tolerancia_negativa_mm_result))
                    espesor_tolerancia_negativa_mm = espesor_tolerancia_negativa_mm_result;

                //espesor_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia espesor positiva (mm)")], out double espesor_tolerancia_positiva_mm_result))
                    espesor_tolerancia_positiva_mm = espesor_tolerancia_positiva_mm_result;

                //ancho_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Ancho(mm)")], out double ancho_mm_result))
                    ancho_mm = ancho_mm_result;

                //ancho_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia ancho negativa (mm)")], out double ancho_tolerancia_negativa_mm_result))
                    ancho_tolerancia_negativa_mm = ancho_tolerancia_negativa_mm_result;

                //ancho_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia ancho positiva (mm)")], out double ancho_tolerancia_positiva_mm_result))
                    ancho_tolerancia_positiva_mm = ancho_tolerancia_positiva_mm_result;

                //avance_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Avance (mm)")], out double avance_mm_result))
                    avance_mm = avance_mm_result;

                //avance_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia avance negativa (mm)")], out double avance_tolerancia_negativa_mm_result))
                    avance_tolerancia_negativa_mm = avance_tolerancia_negativa_mm_result;

                //avance_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia avance positiva (mm)")], out double avance_tolerancia_positiva_mm_result))
                    avance_tolerancia_positiva_mm = avance_tolerancia_positiva_mm_result;

                //piezas por golpe
                if (int.TryParse(array[Array.IndexOf(encabezados, "Pzas por Golpe")], out int piezas_por_golpe_result))
                    piezas_por_golpe = piezas_por_golpe_result;

                //piezas por paquete
                if (int.TryParse(array[Array.IndexOf(encabezados, "Pzas por paquete")], out int piezas_por_paquete_result))
                    piezas_por_paquete = piezas_por_paquete_result;

                //piezas por auto
                if (int.TryParse(array[Array.IndexOf(encabezados, "Piezas por auto")], out int piezas_por_auto_result))
                    piezas_por_auto = piezas_por_auto_result;

                //peso_bruto
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Bruto (KG)")], out double peso_bruto_result))
                    peso_bruto = peso_bruto_result;

                //peso_neto
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Neto (KG)")], out double peso_neto_result))
                    peso_neto = peso_neto_result;


                //peso_inicial
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Inicial")], out double peso_inicial_result))
                    peso_inicial = peso_inicial_result;

                //porcentaje_scrap_puntas_colas
                if (Double.TryParse(array[Array.IndexOf(encabezados, "% de Scrap (puntas y colas)")], out double porcentaje_scrap_puntas_colas_result))
                    porcentaje_scrap_puntas_colas = porcentaje_scrap_puntas_colas_result;


                //valida el procesador externo
                if (array[Array.IndexOf(encabezados, "Procesadores Ext.")] == "SÍ")
                    procesador_externo = true;
                else if (array[Array.IndexOf(encabezados, "Procesadores Ext.")] == "NO")
                    procesador_externo = false;


                //planicidad
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Planicidad (mm)")], out double planicidad_mm_result))
                    planicidad_mm = planicidad_mm_result;

                //req. conciliación puntas y colas
                if (array[Array.IndexOf(encabezados, "Req. conciliación Puntas y colas")] == "SÍ")
                    requiere_consiliacion_puntas_colar = true;
                else if (array[Array.IndexOf(encabezados, "Req. conciliación Puntas y colas")] == "NO")
                    requiere_consiliacion_puntas_colar = false;


                //scrap_permitido_puntas_colas
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Scrap permitido (%)")], out double scrap_permitido_puntas_colas_result))
                    scrap_permitido_puntas_colas = scrap_permitido_puntas_colas_result;

                //Conciliacion Scrap Ingeniería
                if (array[Array.IndexOf(encabezados, "Conciliacion Scrap Ingeniería")] == "SÍ")
                    scrap_conciliacion_ingenieria = true;
                else if (array[Array.IndexOf(encabezados, "Conciliacion Scrap Ingeniería")] == "NO")
                    scrap_conciliacion_ingenieria = false;

                //fecha_validez
                if (DateTime.TryParseExact(array[Array.IndexOf(encabezados, "Fecha validez")], "dd/MM/yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out DateTime fecha_validez_result))
                    fecha_validez = fecha_validez_result;

                //Trapecio: ángulo A
                if (int.TryParse(array[Array.IndexOf(encabezados, "Trapecio: ángulo A")], out int angulo_a_result))
                    angulo_a = angulo_a_result;

                //Trapecio: ángulo B
                if (int.TryParse(array[Array.IndexOf(encabezados, "Trapecio: ángulo B")], out int angulo_b_result))
                    angulo_b = angulo_b_result;

                #endregion

                //conserva el valor del tratamiento ingresado desde el archivo excel
                string tratamiento = null;
                using (var dbEntity = new Portal_2_0Entities())
                {
                    if (id_platina > 0)
                        tratamiento = dbEntity.SCDM_solicitud_rel_item_material.Find(id_platina).tratamiento_superficial;
                }


                resultado.Add(new SCDM_solicitud_rel_item_material
                {
                    num_fila = data.IndexOf(array),
                    id_solicitud = id_solicitud.Value,
                    id_tipo_material = tipoPlatina,   // 3 --> tipo Platina
                    id = id_platina,                        //id = data[0],
                    material_del_cliente = material_del_cliente,  //data[1]
                    tipo_venta = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de Venta")]) ? array[Array.IndexOf(encabezados, "Tipo de Venta")] : null,                  //data[2]
                    numero_odc_cliente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. ODC cliente")]) ? array[Array.IndexOf(encabezados, "Núm. ODC cliente")] : null,                  //data[3]
                    cliente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. cliente")]) ? array[Array.IndexOf(encabezados, "Núm. cliente")] : null,      //data[4]
                    requiere_ppaps = requiere_ppaps,  //data[5]
                    requiere_imds = requiere_imds,  //data[6]
                    numero_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Número Material")]) ? array[Array.IndexOf(encabezados, "Número Material")] : null,  //data[9] readonly
                    metal = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo Metal")]) ? array[Array.IndexOf(encabezados, "Tipo Metal")] : null, //data[10]
                    unidad_medida_inventario = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Unidad Base Medida")]) ? array[Array.IndexOf(encabezados, "Unidad Base Medida")] : null, //data[11]
                    descripcion_material_es = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción Material (ES)")]) ? array[Array.IndexOf(encabezados, "Descripción Material (ES)")] : null,  //data[12] readonly
                    descripcion_material_en = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción Material (EN)")]) ? array[Array.IndexOf(encabezados, "Descripción Material (EN)")] : null,  //data[13] readonly
                    grado_calidad = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Grado/Calidad")]) ? array[Array.IndexOf(encabezados, "Grado/Calidad")] : null,  //data[14] 
                    tipo_recubrimiento = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de Material")]) ? array[Array.IndexOf(encabezados, "Tipo de Material")] : null, //data[15]
                    clase_aprovisionamiento = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "¿Aprovisionamiento?")]) ? array[Array.IndexOf(encabezados, "¿Aprovisionamiento?")] : null,  //data[16]
                    numero_parte = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. parte del cliente")]) ? array[Array.IndexOf(encabezados, "Núm. parte del cliente")] : null,  //data[17]
                    descripcion_numero_parte = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción núm. de parte")]) ? array[Array.IndexOf(encabezados, "Descripción núm. de parte")] : null,  //data[18]
                    norma_referencia = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Norma de referencia")]) ? array[Array.IndexOf(encabezados, "Norma de referencia")] : null,  //data[19]
                    espesor_mm = espesor_mm, //data[21]
                    espesor_tolerancia_negativa_mm = espesor_tolerancia_negativa_mm, //data[21]
                    espesor_tolerancia_positiva_mm = espesor_tolerancia_positiva_mm, //data[22]
                    ancho_mm = ancho_mm, //data[23]                    
                    ancho_tolerancia_negativa_mm = ancho_tolerancia_negativa_mm, //data[24]
                    ancho_tolerancia_positiva_mm = ancho_tolerancia_positiva_mm, //data[25]
                    avance_mm = avance_mm, //data[23]                    
                    avance_tolerancia_negativa_mm = avance_tolerancia_negativa_mm, //data[24]
                    avance_tolerancia_positiva_mm = avance_tolerancia_positiva_mm, //data[25]
                    forma = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Forma")]) ? array[Array.IndexOf(encabezados, "Forma")] : null,
                    piezas_por_golpe = piezas_por_golpe,
                    piezas_por_paquete = piezas_por_paquete,
                    piezas_por_auto = piezas_por_auto,
                    peso_bruto = peso_bruto,
                    peso_neto = peso_neto,
                    peso_recubrimiento = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Peso del recubrimiento")]) ? array[Array.IndexOf(encabezados, "Peso del recubrimiento")] : null, //data[30]
                    parte_interior_exterior = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Parte Int/Ext")]) ? array[Array.IndexOf(encabezados, "Parte Int/Ext")] : null, //data[31]
                    peso_inicial = peso_inicial,
                    porcentaje_scrap_puntas_colas = porcentaje_scrap_puntas_colas,
                    ihs_1 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 1")]) ? array[Array.IndexOf(encabezados, "Programa IHS 1")] : null, //data[33]
                    ihs_2 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 1")]) ? array[Array.IndexOf(encabezados, "Programa IHS 2")] : null, //data[33]
                    ihs_3 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 1")]) ? array[Array.IndexOf(encabezados, "Programa IHS 3")] : null, //data[33]
                    ihs_4 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 1")]) ? array[Array.IndexOf(encabezados, "Programa IHS 4")] : null, //data[33]
                    ihs_5 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 1")]) ? array[Array.IndexOf(encabezados, "Programa IHS 5")] : null, //data[33]
                    molino = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Nombre Molino")]) ? array[Array.IndexOf(encabezados, "Nombre Molino")] : null,
                    modelo_negocio = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Modelo de negocio")]) ? array[Array.IndexOf(encabezados, "Modelo de negocio")] : null, //data[38]
                    tipo_transito = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Transito")]) ? array[Array.IndexOf(encabezados, "Transito")] : null,
                    aplica_procesador_externo = procesador_externo, //data[40]
                    procesador_externo_nombre = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Número procesador Ext.")]) ? array[Array.IndexOf(encabezados, "Número procesador Ext.")] : null, //data[41]
                    numero_antiguo_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. antigüo material")]) ? array[Array.IndexOf(encabezados, "Núm. antigüo material")] : null,  //data[42]
                    planicidad_mm = planicidad_mm, //data[43]
                    msa_hoda = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "MSA (Honda)")]) ? array[Array.IndexOf(encabezados, "MSA (Honda)")] : null,  //data[44]
                    requiere_consiliacion_puntas_colar = requiere_consiliacion_puntas_colar, //data[45]
                    scrap_permitido_puntas_colas = scrap_permitido_puntas_colas, //data[46]
                    conciliacion_scrap_ingenieria = scrap_conciliacion_ingenieria,
                    fecha_validez = fecha_validez, //data[47]
                    angulo_a = angulo_a,
                    angulo_b = angulo_b,
                    tratamiento_superficial = tratamiento,
                });
            }

            return resultado;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id_solicitud"></param>
        /// <returns></returns>
        private List<SCDM_solicitud_rel_item_material> ConvierteArrayACinta(List<string[]> data, int? id_solicitud)
        {
            List<SCDM_solicitud_rel_item_material> resultado = new List<SCDM_solicitud_rel_item_material> { };

            //listado de encabezados
            string[] encabezados = {"ID", "Número Material", "Material del cliente", "Tipo de Venta", "Núm. ODC cliente", "Núm. cliente", "Requiere PPAP's", "Req. IMDS", "Proveedor", "Nombre Molino",
            "Tipo Metal", "Unidad Base Medida","Grado/Calidad", "Descripción Material (ES)", "Descripción Material (EN)",  "Tipo de Material", "¿Aprovisionamiento?", "Núm. parte del cliente",
            "Descripción núm. de parte", "Norma de referencia", "Cintas resultantes por rollo",
            "Espesor (mm)", "Tolerancia espesor negativa (mm)", "Tolerancia espesor positiva (mm)", "Ancho (mm)", "Ancho entrega Cinta(mm)","Tolerancia ancho negativa (mm)",
            "Tolerancia ancho positiva (mm)", "Diametro interior entrada (mm)", "Diametro interior salida (mm)", "Diametro exterior cinta Max (mm)", "Peso Max. entrega cinta (KG)", "Peso del recubrimiento", "Parte Int/Ext", "Posición del Rollo para embarque",
            "Programa IHS 1", "Programa IHS 2", "Programa IHS 3", "Programa IHS 4", "Programa IHS 5", "Modelo de negocio", "Procesadores Ext.", "Número procesador Ext.", "Núm. antigüo material",
            "Planicidad (mm)", "MSA (Honda)","Fecha validez", "Req. conciliación Puntas y colas", "Scrap permitido (%)", "Unidad Medida Ventas"
            };

            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                //declaración de variables
                int? cintas_resultantes_rollo = null, diametro_interior_entrada = null, diametro_interior_salida = null;
                int id_cinta = 0;
                bool? material_del_cliente = null, requiere_ppaps = null, requiere_imds = null, procesador_externo = null, requiere_consiliacion_puntas_colar = null;
                double? espesor_mm = null, espesor_tolerancia_negativa_mm = null, espesor_tolerancia_positiva_mm = null, ancho_mm = null, ancho_entrega_cinta_mm = null, ancho_tolerancia_negativa_mm = null, ancho_tolerancia_positiva_mm = null,
                    diametro_exterior_maximo_mm = null, peso_max_kg = null, planicidad_mm = null, scrap_permitido_puntas_colas = null;
                DateTime? fecha_validez = null;

                #region Asignacion de variables
                //id_rollo
                if (int.TryParse(array[Array.IndexOf(encabezados, "ID")], out int id_cin))
                    id_cinta = id_cin;

                //valida el material del cliente
                if (array[Array.IndexOf(encabezados, "Material del cliente")] == "SÍ")
                    material_del_cliente = true;
                else if (array[Array.IndexOf(encabezados, "Material del cliente")] == "NO")
                    material_del_cliente = false;

                //requiere ppaps
                //valida el material del cliente
                if (array[Array.IndexOf(encabezados, "Requiere PPAP's")] == "SÍ")
                    requiere_ppaps = true;
                else if (array[Array.IndexOf(encabezados, "Requiere PPAP's")] == "NO")
                    requiere_ppaps = false;

                //requiere requiere_imds
                //valida el material del cliente
                if (array[Array.IndexOf(encabezados, "Req. IMDS")] == "SÍ")
                    requiere_imds = true;
                else if (array[Array.IndexOf(encabezados, "Req. IMDS")] == "NO")
                    requiere_imds = false;

                //cintas resultantes x rollo
                if (Int32.TryParse(array[Array.IndexOf(encabezados, "Cintas resultantes por rollo")], out int cintas_x_rollo_result))
                    cintas_resultantes_rollo = cintas_x_rollo_result;

                //espesor_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Espesor (mm)")], out double espesor_mm_result))
                    espesor_mm = espesor_mm_result;

                //espesor_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia espesor negativa (mm)")], out double espesor_tolerancia_negativa_mm_result))
                    espesor_tolerancia_negativa_mm = espesor_tolerancia_negativa_mm_result;

                //espesor_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia espesor positiva (mm)")], out double espesor_tolerancia_positiva_mm_result))
                    espesor_tolerancia_positiva_mm = espesor_tolerancia_positiva_mm_result;

                //ancho_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Ancho (mm)")], out double ancho_mm_result))
                    ancho_mm = ancho_mm_result;

                //ancho_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Ancho entrega Cinta(mm)")], out double ancho_entrega_cinta_mm_result))
                    ancho_entrega_cinta_mm = ancho_entrega_cinta_mm_result;

                //ancho_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia ancho negativa (mm)")], out double ancho_tolerancia_negativa_mm_result))
                    ancho_tolerancia_negativa_mm = ancho_tolerancia_negativa_mm_result;

                //ancho_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia ancho positiva (mm)")], out double ancho_tolerancia_positiva_mm_result))
                    ancho_tolerancia_positiva_mm = ancho_tolerancia_positiva_mm_result;

                //diametro_exterior_maximo_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Diametro exterior cinta Max (mm)")], out double diametro_exterior_maximo_mm_result))
                    diametro_exterior_maximo_mm = diametro_exterior_maximo_mm_result;

                //peso_max_kg
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Max. entrega cinta (KG)")], out double peso_max_kg_result))
                    peso_max_kg = peso_max_kg_result;

                //valida el procesador externo
                if (array[Array.IndexOf(encabezados, "Procesadores Ext.")] == "SÍ")
                    procesador_externo = true;
                else if (array[Array.IndexOf(encabezados, "Procesadores Ext.")] == "NO")
                    procesador_externo = false;

                //planicidad
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Planicidad (mm)")], out double planicidad_mm_result))
                    planicidad_mm = planicidad_mm_result;

                //req. conciliación puntas y colas
                if (array[Array.IndexOf(encabezados, "Req. conciliación Puntas y colas")] == "SÍ")
                    requiere_consiliacion_puntas_colar = true;
                else if (array[Array.IndexOf(encabezados, "Req. conciliación Puntas y colas")] == "NO")
                    requiere_consiliacion_puntas_colar = false;

                //scrap_permitido_puntas_colas
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Scrap permitido (%)")], out double scrap_permitido_puntas_colas_result))
                    scrap_permitido_puntas_colas = scrap_permitido_puntas_colas_result;

                //fecha_validez
                if (DateTime.TryParseExact(array[Array.IndexOf(encabezados, "Fecha validez")], "dd/MM/yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out DateTime fecha_validez_result))
                    fecha_validez = fecha_validez_result;


                //diametro interior entrada
                if (int.TryParse(array[Array.IndexOf(encabezados, "Diametro interior entrada (mm)")], out int diametro_interior_entrada_result))
                    diametro_interior_entrada = diametro_interior_entrada_result;

                //diametro interior salida
                if (int.TryParse(array[Array.IndexOf(encabezados, "Diametro interior salida (mm)")], out int diametro_interior_salida_result))
                    diametro_interior_salida = diametro_interior_salida_result;

                #endregion

                //conserva el valor del tratamiento ingresado desde el archivo excel
                string tratamiento = null;
                using (var dbEntity = new Portal_2_0Entities())
                {
                    if (id_cinta > 0)
                        tratamiento = dbEntity.SCDM_solicitud_rel_item_material.Find(id_cinta).tratamiento_superficial;
                }


                resultado.Add(new SCDM_solicitud_rel_item_material
                {
                    num_fila = data.IndexOf(array),
                    id_solicitud = id_solicitud.Value,
                    id_tipo_material = Bitacoras.Util.SCDM_solicitud_rel_item_material_tipo.CINTA,   // 2 --> CINTA
                    id = id_cinta,                        //id = data[0],
                    material_del_cliente = material_del_cliente,  //data[1]
                    tipo_venta = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de Venta")]) ? array[Array.IndexOf(encabezados, "Tipo de Venta")] : null,                  //data[2]
                    numero_odc_cliente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. ODC cliente")]) ? array[Array.IndexOf(encabezados, "Núm. ODC cliente")] : null,                  //data[3]
                    cliente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. cliente")]) ? array[Array.IndexOf(encabezados, "Núm. cliente")] : null,        //data[4]
                    requiere_ppaps = requiere_ppaps,  //data[5]
                    requiere_imds = requiere_imds,  //data[6]
                    proveedor = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Proveedor")]) ? array[Array.IndexOf(encabezados, "Proveedor")] : null, //data[7]
                    molino = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Nombre Molino")]) ? array[Array.IndexOf(encabezados, "Nombre Molino")] : null, //data[8]
                    numero_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Número Material")]) ? array[Array.IndexOf(encabezados, "Número Material")] : null,  //data[9] readonly
                    metal = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo Metal")]) ? array[Array.IndexOf(encabezados, "Tipo Metal")] : null, //data[10]
                    unidad_medida_inventario = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Unidad Base Medida")]) ? array[Array.IndexOf(encabezados, "Unidad Base Medida")] : null, //data[11]
                    descripcion_material_es = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción Material (ES)")]) ? array[Array.IndexOf(encabezados, "Descripción Material (ES)")] : null,  //data[12] readonly
                    descripcion_material_en = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción Material (EN)")]) ? array[Array.IndexOf(encabezados, "Descripción Material (EN)")] : null,  //data[13] readonly
                    grado_calidad = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Grado/Calidad")]) ? array[Array.IndexOf(encabezados, "Grado/Calidad")] : null,  //data[14] 
                    tipo_recubrimiento = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de Material")]) ? array[Array.IndexOf(encabezados, "Tipo de Material")] : null, //data[15]
                    clase_aprovisionamiento = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "¿Aprovisionamiento?")]) ? array[Array.IndexOf(encabezados, "¿Aprovisionamiento?")] : null,  //data[16]
                    numero_parte = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. parte del cliente")]) ? array[Array.IndexOf(encabezados, "Núm. parte del cliente")] : null,  //data[17]
                    descripcion_numero_parte = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción núm. de parte")]) ? array[Array.IndexOf(encabezados, "Descripción núm. de parte")] : null,  //data[18]
                    norma_referencia = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Norma de referencia")]) ? array[Array.IndexOf(encabezados, "Norma de referencia")] : null,  //data[19]
                    numero_cintas_resultantes = cintas_resultantes_rollo, //data[20]
                    espesor_mm = espesor_mm, //data[21]
                    espesor_tolerancia_negativa_mm = espesor_tolerancia_negativa_mm, //data[21]
                    espesor_tolerancia_positiva_mm = espesor_tolerancia_positiva_mm, //data[22]
                    ancho_mm = ancho_mm, //data[23]
                    ancho_entrega_cinta_mm = ancho_entrega_cinta_mm, //data[23]
                    ancho_tolerancia_negativa_mm = ancho_tolerancia_negativa_mm, //data[24]
                    ancho_tolerancia_positiva_mm = ancho_tolerancia_positiva_mm, //data[25]
                    diametro_interior = diametro_interior_entrada, //data[26]
                    diametro_interior_salida = diametro_interior_salida, //data[26]
                    diametro_exterior_maximo_mm = diametro_exterior_maximo_mm,//data[27]
                    peso_max_kg = peso_max_kg, //data[29]
                    peso_recubrimiento = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Peso del recubrimiento")]) ? array[Array.IndexOf(encabezados, "Peso del recubrimiento")] : null, //data[30]
                    parte_interior_exterior = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Parte Int/Ext")]) ? array[Array.IndexOf(encabezados, "Parte Int/Ext")] : null, //data[31]
                    posicion_rollo = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Posición del Rollo para embarque")]) ? array[Array.IndexOf(encabezados, "Posición del Rollo para embarque")] : null, //data[32]
                    ihs_1 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 1")]) ? array[Array.IndexOf(encabezados, "Programa IHS 1")] : null, //data[33]
                    ihs_2 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 2")]) ? array[Array.IndexOf(encabezados, "Programa IHS 2")] : null, //data[33]
                    ihs_3 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 3")]) ? array[Array.IndexOf(encabezados, "Programa IHS 3")] : null, //data[33]
                    ihs_4 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 4")]) ? array[Array.IndexOf(encabezados, "Programa IHS 4")] : null, //data[33]
                    ihs_5 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Programa IHS 5")]) ? array[Array.IndexOf(encabezados, "Programa IHS 5")] : null, //data[33]
                    modelo_negocio = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Modelo de negocio")]) ? array[Array.IndexOf(encabezados, "Modelo de negocio")] : null, //data[38]
                    aplica_procesador_externo = procesador_externo, //data[40]
                    procesador_externo_nombre = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Número procesador Ext.")]) ? array[Array.IndexOf(encabezados, "Número procesador Ext.")] : null, //data[41]
                    numero_antiguo_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. antigüo material")]) ? array[Array.IndexOf(encabezados, "Núm. antigüo material")] : null,  //data[42]
                    planicidad_mm = planicidad_mm, //data[43]
                    msa_hoda = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "MSA (Honda)")]) ? array[Array.IndexOf(encabezados, "MSA (Honda)")] : null,  //data[44]
                    requiere_consiliacion_puntas_colar = requiere_consiliacion_puntas_colar, //data[45]
                    scrap_permitido_puntas_colas = scrap_permitido_puntas_colas, //data[46]
                    fecha_validez = fecha_validez, //data[47]
                    tratamiento_superficial = tratamiento,
                    unidad_medida_ventas = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Unidad Medida Ventas")]) ? array[Array.IndexOf(encabezados, "Unidad Medida Ventas")] : null, //data[11]

                });
            }
            return resultado;
        }
        private List<SCDM_solicitud_rel_item_material> ConvierteArrayACB(List<string[]> data, int? id_solicitud)
        {
            List<SCDM_solicitud_rel_item_material> resultado = new List<SCDM_solicitud_rel_item_material> { };


            //listado de encabezados
            string[] encabezados = {"ID", "Numero de Material", "Tipo de Venta", "Nombre de Molino", "Número de Cliente", "Material de <br>Compra a TKMM",  "Clase de Material",
                "Tipo de metal", "Unidad Base de Medida", "Número de parte <br>del cliente", "Descripción del número de parte", "Descripción del material (ES)", "Descripción del material (EN)",
            "Grado/Calidad", "Aprovisionamiento", "Espesor(mm)", "Tolerancia Espesor <br>Negativa(mm)", "Tolerancia Espesor <br>Positiva(mm)", "Ancho(mm)", "Tolerancia Ancho <br>Negativa(mm)",
            "Tolerancia Ancho <br>Positiva(mm)", "Largo(mm)", "Tolerancia Largo <br>Negativa(mm)", "Tolerancia Largo <br>Positiva(mm)", "Peso Bruto (LB)", "Peso Neto (LB)",
            "Aleación", "Modelo de Negocio", "Transito", "Proveedor", "Precio", "Moneda", "Incoterm", "Terminos de pago", "¿Aplica tasa de IVA?"
            };

            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                //declaración de variables
                int id_cb = 0;
                double? espesor_mm = null, espesor_tolerancia_negativa_mm = null, espesor_tolerancia_positiva_mm = null, ancho_mm = null, ancho_tolerancia_negativa_mm = null, ancho_tolerancia_positiva_mm = null,
                   largo_mm = null, largo_tolerancia_negativa_mm = null, largo_tolerancia_positiva_mm = null, peso_bruto = null, peso_neto = null, precio = null;
                bool? material_compra = null, aplica_iva = null;

                //bool? material_del_cliente = null, requiere_ppaps = null, requiere_imds = null, procesador_externo = null, requiere_consiliacion_puntas_colar = null;
                //double? espesor_mm = null, espesor_tolerancia_negativa_mm = null, espesor_tolerancia_positiva_mm = null, ancho_mm = null, ancho_entrega_cinta_mm = null, ancho_tolerancia_negativa_mm = null, ancho_tolerancia_positiva_mm = null,
                //    diametro_exterior_maximo_mm = null, peso_max_kg = null, planicidad_mm = null, scrap_permitido_puntas_colas = null;
                //DateTime? fecha_validez = null;

                #region Asignacion de variables
                //id_cb
                if (int.TryParse(array[Array.IndexOf(encabezados, "ID")], out int id_cin))
                    id_cb = id_cin;


                //Material de compra
                if (array[Array.IndexOf(encabezados, "Material de <br>Compra a TKMM")] == "SÍ")
                    material_compra = true;
                else if (array[Array.IndexOf(encabezados, "Material de <br>Compra a TKMM")] == "NO")
                    material_compra = false;


                //espesor_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Espesor(mm)")], out double espesor_mm_result))
                    espesor_mm = espesor_mm_result;

                //espesor_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia Espesor <br>Negativa(mm)")], out double espesor_tolerancia_negativa_mm_result))
                    espesor_tolerancia_negativa_mm = espesor_tolerancia_negativa_mm_result;

                //espesor_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia Espesor <br>Positiva(mm)")], out double espesor_tolerancia_positiva_mm_result))
                    espesor_tolerancia_positiva_mm = espesor_tolerancia_positiva_mm_result;

                //ancho_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Ancho(mm)")], out double ancho_mm_result))
                    ancho_mm = ancho_mm_result;

                //ancho_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia Ancho <br>Negativa(mm)")], out double ancho_tolerancia_negativa_mm_result))
                    ancho_tolerancia_negativa_mm = ancho_tolerancia_negativa_mm_result;

                //ancho_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia Ancho <br>Positiva(mm)")], out double ancho_tolerancia_positiva_mm_result))
                    ancho_tolerancia_positiva_mm = ancho_tolerancia_positiva_mm_result;

                //largo_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Largo(mm)")], out double largo_mm_result))
                    largo_mm = largo_mm_result;

                //largo_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia Largo <br>Negativa(mm)")], out double largo_tolerancia_negativa_mm_result))
                    largo_tolerancia_negativa_mm = largo_tolerancia_negativa_mm_result;

                //largo_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia Largo <br>Positiva(mm)")], out double largo_tolerancia_positiva_mm_result))
                    largo_tolerancia_positiva_mm = largo_tolerancia_positiva_mm_result;

                //peso_bruto
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Bruto (LB)")], out double peso_bruto_result))
                    peso_bruto = peso_bruto_result;

                //peso_neto
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Neto (LB)")], out double peso_neto_result))
                    peso_neto = peso_neto_result;


                //precio
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Precio")], out double precio_result))
                    precio = precio_result;

                //Material de compra
                if (array[Array.IndexOf(encabezados, "¿Aplica tasa de IVA?")] == "SÍ")
                    aplica_iva = true;
                else if (array[Array.IndexOf(encabezados, "¿Aplica tasa de IVA?")] == "NO")
                    aplica_iva = false;

                #endregion

                resultado.Add(new SCDM_solicitud_rel_item_material
                {
                    num_fila = data.IndexOf(array),
                    id_solicitud = id_solicitud.Value,
                    id_tipo_material = Bitacoras.Util.SCDM_solicitud_rel_item_material_tipo.C_B,
                    id = id_cb,
                    tipo_venta = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de Venta")]) ? array[Array.IndexOf(encabezados, "Tipo de Venta")] : null,
                    molino = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Nombre de Molino")]) ? array[Array.IndexOf(encabezados, "Nombre de Molino")] : null,
                    cliente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Número de Cliente")]) ? array[Array.IndexOf(encabezados, "Número de Cliente")] : null,
                    material_compra_tkmm = material_compra,
                    numero_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Numero de Material")]) ? array[Array.IndexOf(encabezados, "Numero de Material")] : null,
                    materia_prima_producto_terminado = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Clase de Material")]) ? array[Array.IndexOf(encabezados, "Clase de Material")] : null,
                    tipo_metal_cb = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de metal")]) ? array[Array.IndexOf(encabezados, "Tipo de metal")] : null,
                    unidad_medida_inventario = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Unidad Base de Medida")]) ? array[Array.IndexOf(encabezados, "Unidad Base de Medida")] : null,
                    numero_parte = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Número de parte <br>del cliente")]) ? array[Array.IndexOf(encabezados, "Número de parte <br>del cliente")] : null,
                    descripcion_numero_parte = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción del número de parte")]) ? array[Array.IndexOf(encabezados, "Descripción del número de parte")] : null,  //data[18]
                    descripcion_material_es = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción del material (ES)")]) ? array[Array.IndexOf(encabezados, "Descripción del material (ES)")] : null,  //data[12] readonly
                    descripcion_material_en = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción del material (EN)")]) ? array[Array.IndexOf(encabezados, "Descripción del material (EN)")] : null,  //data[13] readonly
                    grado_calidad = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Grado/Calidad")]) ? array[Array.IndexOf(encabezados, "Grado/Calidad")] : null,
                    clase_aprovisionamiento = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Aprovisionamiento")]) ? array[Array.IndexOf(encabezados, "Aprovisionamiento")] : null,
                    espesor_mm = espesor_mm, //data[21]
                    espesor_tolerancia_negativa_mm = espesor_tolerancia_negativa_mm, //data[21]
                    espesor_tolerancia_positiva_mm = espesor_tolerancia_positiva_mm, //data[22]
                    ancho_mm = ancho_mm, //data[23]
                    ancho_tolerancia_negativa_mm = ancho_tolerancia_negativa_mm, //data[24]
                    ancho_tolerancia_positiva_mm = ancho_tolerancia_positiva_mm, //data[25]
                    avance_mm = largo_mm, //data[23]
                    avance_tolerancia_negativa_mm = largo_tolerancia_negativa_mm, //data[24]
                    avance_tolerancia_positiva_mm = largo_tolerancia_positiva_mm, //data[25]
                    peso_bruto = peso_bruto,
                    peso_neto = peso_neto,
                    aleacion = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Aleación")]) ? array[Array.IndexOf(encabezados, "Aleación")] : null,
                    modelo_negocio = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Modelo de Negocio")]) ? array[Array.IndexOf(encabezados, "Modelo de Negocio")] : null, //data[38]
                    tipo_transito = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Transito")]) ? array[Array.IndexOf(encabezados, "Transito")] : null,
                    proveedor = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Proveedor")]) ? array[Array.IndexOf(encabezados, "Proveedor")] : null,
                    precio = precio,
                    moneda = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Moneda")]) ? array[Array.IndexOf(encabezados, "Moneda")] : null,
                    incoterm = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Incoterm")]) ? array[Array.IndexOf(encabezados, "Incoterm")] : null,
                    terminos_pago = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Terminos de pago")]) ? array[Array.IndexOf(encabezados, "Terminos de pago")] : null,
                    aplica_tasa_iva = aplica_iva
                });
            }
            return resultado;
        }
        private List<SCDM_solicitud_rel_creacion_referencia> ConvierteArrayACreacionReferencia(List<string[]> data, int? id_solicitud)
        {
            List<SCDM_solicitud_rel_creacion_referencia> resultado = new List<SCDM_solicitud_rel_creacion_referencia> { };

            //variables globales para el metodo
            var BD_SCDM_cat_tipo_venta = db.SCDM_cat_tipo_venta.Where(x => x.activo == true).ToList();
            var BD_SCDM_planta = db.plantas.Where(x => x.activo == true).ToList();
            var BD_SCDM_cat_tipo_material = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList();


            //listado de encabezados
            string[] encabezados = {
                "ID", "Cambios","Nuevo Material", "Material Existente", "Tipo de Material", "Planta", "Motivo de la creacion", "Tipo Metal", "Selling Type (Budget)",
                "Núm. antigüo material", "Peso Bruto (KG)", "Peso Neto (KG)", "Unidad Base Medida","Descripción ES (Original)","Descripción EN (Original)", "Descripción (ES)", "Descripción (EN)",
                "Commodity", "Grado/Calidad",
                "Espesor (mm)", "Tolerancia espesor negativa (mm)", "Tolerancia espesor positiva (mm)",
                "Ancho (mm)", "Tolerancia ancho negativa (mm)", "Tolerancia ancho positiva (mm)",
                "Avance (mm)", "Tolerancia avance negativa (mm)", "Tolerancia avance positiva (mm)",
                "Planicidad (mm)", "Superficie", "Tratamiento Superficial", "Peso del recubrimiento", "Nombre Molino", "Forma", "Núm. cliente", "Núm. parte del cliente",
                "MSA (Honda)","Diametro Exterior", "Diametro Interior",
                "Otro Dato", "Comentario Adicional"
            };

            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                //declaración de variables
                int? id_tipo_venta = null, id_planta = null, id_tipo_material = null;
                int id_creacion_referencia = 0;
                double? peso_bruto = null, peso_neto = null, espesor_mm = null, espesor_tolerancia_negativa_mm = null, espesor_tolerancia_positiva_mm = null,
                    ancho_mm = null, ancho_tolerancia_negativa_mm = null, ancho_tolerancia_positiva_mm = null,
                    avance_mm = null, avance_tolerancia_negativa_mm = null, avance_tolerancia_positiva_mm = null,
                    planicidad_mm = null, diametro_interior = null, diametro_exterior = null;

                #region Asignacion de variables
                //id_creacion
                if (int.TryParse(array[Array.IndexOf(encabezados, "ID")], out int id_cr))
                    id_creacion_referencia = id_cr;

                //tipo de venta
                var tempTipoVenta = array[Array.IndexOf(encabezados, "Selling Type (Budget)")];
                if (tempTipoVenta != null && !string.IsNullOrEmpty(tempTipoVenta) && BD_SCDM_cat_tipo_venta.Any(x => x.descripcion.Trim() == tempTipoVenta))
                    id_tipo_venta = BD_SCDM_cat_tipo_venta.FirstOrDefault(x => x.descripcion.Trim() == tempTipoVenta).id;

                //planta
                var tempPlanta = array[Array.IndexOf(encabezados, "Planta")];
                if (tempPlanta != null && !string.IsNullOrEmpty(tempPlanta) && BD_SCDM_planta.Any(x => x.ConcatPlantaSap.Trim() == tempPlanta))
                    id_planta = BD_SCDM_planta.FirstOrDefault(x => x.ConcatPlantaSap.Trim() == tempPlanta).clave;


                //motivo material
                var tempTipoMaterial = array[Array.IndexOf(encabezados, "Tipo de Material")];
                if (tempTipoMaterial != null && !string.IsNullOrEmpty(tempTipoMaterial) && BD_SCDM_cat_tipo_material.Any(x => x.descripcion.Trim() == tempTipoMaterial))
                    id_tipo_material = BD_SCDM_cat_tipo_material.FirstOrDefault(x => x.descripcion.Trim() == tempTipoMaterial).id;

                //peso_bruto
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Bruto (KG)")], out double peso_bruto_result))
                    peso_bruto = peso_bruto_result;

                //peso_neto
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Neto (KG)")], out double peso_neto_result))
                    peso_neto = peso_neto_result;

                //espesor_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Espesor (mm)")], out double espesor_mm_result))
                    espesor_mm = espesor_mm_result;

                //espesor_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia espesor negativa (mm)")], out double espesor_tolerancia_negativa_mm_result))
                    espesor_tolerancia_negativa_mm = espesor_tolerancia_negativa_mm_result;

                //espesor_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia espesor positiva (mm)")], out double espesor_tolerancia_positiva_mm_result))
                    espesor_tolerancia_positiva_mm = espesor_tolerancia_positiva_mm_result;

                //ancho_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Ancho (mm)")], out double ancho_mm_result))
                    ancho_mm = ancho_mm_result;

                //ancho_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia ancho negativa (mm)")], out double ancho_tolerancia_negativa_mm_result))
                    ancho_tolerancia_negativa_mm = ancho_tolerancia_negativa_mm_result;

                //ancho_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia ancho positiva (mm)")], out double ancho_tolerancia_positiva_mm_result))
                    ancho_tolerancia_positiva_mm = ancho_tolerancia_positiva_mm_result;

                //avance_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Avance (mm)")], out double avance_mm_result))
                    avance_mm = avance_mm_result;

                //avance_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia avance negativa (mm)")], out double avance_tolerancia_negativa_mm_result))
                    avance_tolerancia_negativa_mm = avance_tolerancia_negativa_mm_result;

                //avance_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia avance positiva (mm)")], out double avance_tolerancia_positiva_mm_result))
                    avance_tolerancia_positiva_mm = avance_tolerancia_positiva_mm_result;

                //planicidad
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Planicidad (mm)")], out double planicidad_mm_result))
                    planicidad_mm = planicidad_mm_result;

                //diametro exterior
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Diametro Exterior")], out double diametro_exterior_result))
                    diametro_exterior = diametro_exterior_result;

                //Diametro Interior
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Diametro Interior")], out double diametro_interior_result))
                    diametro_interior = diametro_interior_result;


                #endregion

                SCDM_solicitud_rel_creacion_referencia creacionReferencia = new SCDM_solicitud_rel_creacion_referencia
                {
                    num_fila = data.IndexOf(array),
                    id_solicitud = id_solicitud.Value,
                    id = id_creacion_referencia,
                    id_tipo_venta = id_tipo_venta,
                    material_existente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Material Existente")]) ? array[Array.IndexOf(encabezados, "Material Existente")] : null,
                    nuevo_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Nuevo Material")]) ? array[Array.IndexOf(encabezados, "Nuevo Material")] : null,
                    id_tipo_material = id_tipo_material,
                    id_planta = id_planta,
                    motivo_creacion = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Motivo de la creacion")]) ? array[Array.IndexOf(encabezados, "Motivo de la creacion")] : null,
                    tipo_metal = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo Metal")]) ? array[Array.IndexOf(encabezados, "Tipo Metal")] : null,
                    numero_antiguo_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. antigüo material")]) ? array[Array.IndexOf(encabezados, "Núm. antigüo material")] : null,  //data[42]
                    peso_bruto = peso_bruto,
                    peso_neto = peso_neto,
                    unidad_medida_inventario = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Unidad Base Medida")]) ? array[Array.IndexOf(encabezados, "Unidad Base Medida")] : null, //data[11]
                    descripcion_es = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción (ES)")]) ? array[Array.IndexOf(encabezados, "Descripción (ES)")] : null, //data[11]
                    descripcion_en = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción (EN)")]) ? array[Array.IndexOf(encabezados, "Descripción (EN)")] : null, //data[11]
                    commodity = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Commodity")]) ? array[Array.IndexOf(encabezados, "Commodity")] : null,  //data[14] 
                    grado_calidad = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Grado/Calidad")]) ? array[Array.IndexOf(encabezados, "Grado/Calidad")] : null,  //data[14] 
                    espesor_mm = espesor_mm, //data[21]
                    espesor_tolerancia_negativa_mm = espesor_tolerancia_negativa_mm, //data[21]
                    espesor_tolerancia_positiva_mm = espesor_tolerancia_positiva_mm, //data[22]
                    ancho_mm = ancho_mm, //data[23]                    
                    ancho_tolerancia_negativa_mm = ancho_tolerancia_negativa_mm, //data[24]
                    ancho_tolerancia_positiva_mm = ancho_tolerancia_positiva_mm, //data[25]
                    avance_mm = avance_mm, //data[23]                    
                    avance_tolerancia_negativa_mm = avance_tolerancia_negativa_mm, //data[24]
                    avance_tolerancia_positiva_mm = avance_tolerancia_positiva_mm, //data[25]
                    planicidad_mm = planicidad_mm, //data[43]
                    superficie = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Superficie")]) ? array[Array.IndexOf(encabezados, "Superficie")] : null,  //data[14] 
                    tratamiento_superficial = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tratamiento Superficial")]) ? array[Array.IndexOf(encabezados, "Tratamiento Superficial")] : null,  //data[14] 
                    peso_recubrimiento = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Peso del recubrimiento")]) ? array[Array.IndexOf(encabezados, "Peso del recubrimiento")] : null, //data[30]
                    molino = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Nombre Molino")]) ? array[Array.IndexOf(encabezados, "Nombre Molino")] : null,
                    forma = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Forma")]) ? array[Array.IndexOf(encabezados, "Forma")] : null,
                    cliente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. cliente")]) ? array[Array.IndexOf(encabezados, "Núm. cliente")] : null,      //data[4]
                    numero_parte = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. parte del cliente")]) ? array[Array.IndexOf(encabezados, "Núm. parte del cliente")] : null,  //data[17]
                    msa_honda = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "MSA (Honda)")]) ? array[Array.IndexOf(encabezados, "MSA (Honda)")] : null,  //data[44]
                    diametro_interior = diametro_interior,
                    diametro_exterior = diametro_exterior,
                    comentarios = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Comentario Adicional")]) ? array[Array.IndexOf(encabezados, "Comentario Adicional")] : null,
                    nuevo_dato = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Otro Dato")]) ? array[Array.IndexOf(encabezados, "Otro Dato")] : null,
                    tipo_material_text = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de Material")]) ? array[Array.IndexOf(encabezados, "Tipo de Material")] : null,
                };


                //actualiza los cambios vs el material de referencia (BD)
                string cambiosString = GetCambiosCreacionReferencia(creacionReferencia);
                if (!string.IsNullOrEmpty(cambiosString))
                    creacionReferencia.cambios = UsoStrings.RecortaString(cambiosString, 2000);

                resultado.Add(creacionReferencia);
            }

            return resultado;
        }
        private List<SCDM_solicitud_rel_cambio_ingenieria> ConvierteArrayACambiosIngenieria(List<string[]> data, int? id_solicitud)
        {
            List<SCDM_solicitud_rel_cambio_ingenieria> resultado = new List<SCDM_solicitud_rel_cambio_ingenieria> { };

            //variables globales para el metodo
            var BD_SCDM_cat_tipo_venta = db.SCDM_cat_tipo_venta.Where(x => x.activo == true).ToList();
            var BD_SCDM_planta = db.plantas.Where(x => x.activo == true).ToList();
            var BD_SCDM_cat_tipo_material = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList();


            //listado de encabezados
            string[] encabezados = {
                "ID", "Cambios", "Material Existente", "Tipo de Material", "Planta", "Tipo Metal", "Selling Type (Budget)",
                "Núm. antigüo material", "Peso Bruto (KG)", "Peso Neto (KG)", "Unidad Base Medida","Descripción ES (Original)","Descripción EN (Original)", "Descripción (ES)", "Descripción (EN)",
                "Commodity", "Grado/Calidad",
                "Espesor (mm)", "Tolerancia espesor negativa (mm)", "Tolerancia espesor positiva (mm)",
                "Ancho (mm)", "Tolerancia ancho negativa (mm)", "Tolerancia ancho positiva (mm)",
                "Avance (mm)", "Tolerancia avance negativa (mm)", "Tolerancia avance positiva (mm)",
                "Planicidad (mm)", "Superficie", "Tratamiento Superficial", "Peso del recubrimiento", "Nombre Molino", "Forma", "Núm. cliente", "Núm. parte del cliente",
                "MSA (Honda)","Diametro Exterior", "Diametro Interior",
                "Otro Dato", "Comentario Adicional"
            };

            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                //declaración de variables
                int? id_tipo_venta = null, id_planta = null, id_tipo_material = null;
                int id_item = 0;
                double? peso_bruto = null, peso_neto = null, espesor_mm = null, espesor_tolerancia_negativa_mm = null, espesor_tolerancia_positiva_mm = null,
                    ancho_mm = null, ancho_tolerancia_negativa_mm = null, ancho_tolerancia_positiva_mm = null,
                    avance_mm = null, avance_tolerancia_negativa_mm = null, avance_tolerancia_positiva_mm = null,
                    planicidad_mm = null, diametro_interior = null, diametro_exterior = null;

                #region Asignacion de variables
                //id_creacion
                if (int.TryParse(array[Array.IndexOf(encabezados, "ID")], out int id_cr))
                    id_item = id_cr;

                //tipo de venta
                var tempTipoVenta = array[Array.IndexOf(encabezados, "Selling Type (Budget)")];
                if (tempTipoVenta != null && !string.IsNullOrEmpty(tempTipoVenta) && BD_SCDM_cat_tipo_venta.Any(x => x.descripcion.Trim() == tempTipoVenta))
                    id_tipo_venta = BD_SCDM_cat_tipo_venta.FirstOrDefault(x => x.descripcion.Trim() == tempTipoVenta).id;

                //planta
                var tempPlanta = array[Array.IndexOf(encabezados, "Planta")];
                if (tempPlanta != null && !string.IsNullOrEmpty(tempPlanta) && BD_SCDM_planta.Any(x => x.ConcatPlantaSap.Trim() == tempPlanta))
                    id_planta = BD_SCDM_planta.FirstOrDefault(x => x.ConcatPlantaSap.Trim() == tempPlanta).clave;


                //motivo material
                var tempTipoMaterial = array[Array.IndexOf(encabezados, "Tipo de Material")];
                if (tempTipoMaterial != null && !string.IsNullOrEmpty(tempTipoMaterial) && BD_SCDM_cat_tipo_material.Any(x => x.descripcion.Trim() == tempTipoMaterial))
                    id_tipo_material = BD_SCDM_cat_tipo_material.FirstOrDefault(x => x.descripcion.Trim() == tempTipoMaterial).id;

                //peso_bruto
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Bruto (KG)")], out double peso_bruto_result))
                    peso_bruto = peso_bruto_result;

                //peso_neto
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Neto (KG)")], out double peso_neto_result))
                    peso_neto = peso_neto_result;

                //espesor_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Espesor (mm)")], out double espesor_mm_result))
                    espesor_mm = espesor_mm_result;

                //espesor_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia espesor negativa (mm)")], out double espesor_tolerancia_negativa_mm_result))
                    espesor_tolerancia_negativa_mm = espesor_tolerancia_negativa_mm_result;

                //espesor_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia espesor positiva (mm)")], out double espesor_tolerancia_positiva_mm_result))
                    espesor_tolerancia_positiva_mm = espesor_tolerancia_positiva_mm_result;

                //ancho_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Ancho (mm)")], out double ancho_mm_result))
                    ancho_mm = ancho_mm_result;

                //ancho_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia ancho negativa (mm)")], out double ancho_tolerancia_negativa_mm_result))
                    ancho_tolerancia_negativa_mm = ancho_tolerancia_negativa_mm_result;

                //ancho_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia ancho positiva (mm)")], out double ancho_tolerancia_positiva_mm_result))
                    ancho_tolerancia_positiva_mm = ancho_tolerancia_positiva_mm_result;

                //avance_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Avance (mm)")], out double avance_mm_result))
                    avance_mm = avance_mm_result;

                //avance_tolerancia_negativa_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia avance negativa (mm)")], out double avance_tolerancia_negativa_mm_result))
                    avance_tolerancia_negativa_mm = avance_tolerancia_negativa_mm_result;

                //avance_tolerancia_positiva_mm
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Tolerancia avance positiva (mm)")], out double avance_tolerancia_positiva_mm_result))
                    avance_tolerancia_positiva_mm = avance_tolerancia_positiva_mm_result;

                //planicidad
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Planicidad (mm)")], out double planicidad_mm_result))
                    planicidad_mm = planicidad_mm_result;

                //diametro exterior
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Diametro Exterior")], out double diametro_exterior_result))
                    diametro_exterior = diametro_exterior_result;

                //Diametro Interior
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Diametro Interior")], out double diametro_interior_result))
                    diametro_interior = diametro_interior_result;



                #endregion

                SCDM_solicitud_rel_cambio_ingenieria cambio = new SCDM_solicitud_rel_cambio_ingenieria
                {
                    num_fila = data.IndexOf(array),
                    id_solicitud = id_solicitud.Value,
                    id = id_item,
                    //id_tipo_venta = id_tipo_venta,
                    tipo_venta = tempTipoVenta,
                    material_existente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Material Existente")]) ? array[Array.IndexOf(encabezados, "Material Existente")] : null,
                    //nuevo_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Nuevo Material")]) ? array[Array.IndexOf(encabezados, "Nuevo Material")] : null,
                    id_tipo_material = id_tipo_material,
                    id_planta = id_planta,
                    //motivo_creacion = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Motivo de la creacion")]) ? array[Array.IndexOf(encabezados, "Motivo de la creacion")] : null,
                    tipo_metal = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo Metal")]) ? array[Array.IndexOf(encabezados, "Tipo Metal")] : null,
                    numero_antiguo_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. antigüo material")]) ? array[Array.IndexOf(encabezados, "Núm. antigüo material")] : null,  //data[42]
                    peso_bruto = peso_bruto,
                    peso_neto = peso_neto,
                    unidad_medida_inventario = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Unidad Base Medida")]) ? array[Array.IndexOf(encabezados, "Unidad Base Medida")] : null, //data[11]
                    descripcion_es = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción (ES)")]) ? array[Array.IndexOf(encabezados, "Descripción (ES)")] : null, //data[11]
                    descripcion_en = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción (EN)")]) ? array[Array.IndexOf(encabezados, "Descripción (EN)")] : null, //data[11]
                    commodity = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Commodity")]) ? array[Array.IndexOf(encabezados, "Commodity")] : null,  //data[14] 
                    grado_calidad = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Grado/Calidad")]) ? array[Array.IndexOf(encabezados, "Grado/Calidad")] : null,  //data[14] 
                    espesor_mm = espesor_mm, //data[21]
                    espesor_tolerancia_negativa_mm = espesor_tolerancia_negativa_mm, //data[21]
                    espesor_tolerancia_positiva_mm = espesor_tolerancia_positiva_mm, //data[22]
                    ancho_mm = ancho_mm, //data[23]                    
                    ancho_tolerancia_negativa_mm = ancho_tolerancia_negativa_mm, //data[24]
                    ancho_tolerancia_positiva_mm = ancho_tolerancia_positiva_mm, //data[25]
                    avance_mm = avance_mm, //data[23]                    
                    avance_tolerancia_negativa_mm = avance_tolerancia_negativa_mm, //data[24]
                    avance_tolerancia_positiva_mm = avance_tolerancia_positiva_mm, //data[25]
                    planicidad_mm = planicidad_mm, //data[43]
                    superficie = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Superficie")]) ? array[Array.IndexOf(encabezados, "Superficie")] : null,  //data[14] 
                    tratamiento_superficial = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tratamiento Superficial")]) ? array[Array.IndexOf(encabezados, "Tratamiento Superficial")] : null,  //data[14] 
                    peso_recubrimiento = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Peso del recubrimiento")]) ? array[Array.IndexOf(encabezados, "Peso del recubrimiento")] : null, //data[30]
                    molino = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Nombre Molino")]) ? array[Array.IndexOf(encabezados, "Nombre Molino")] : null,
                    forma = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Forma")]) ? array[Array.IndexOf(encabezados, "Forma")] : null,
                    cliente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. cliente")]) ? array[Array.IndexOf(encabezados, "Núm. cliente")] : null,      //data[4]
                    numero_parte = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. parte del cliente")]) ? array[Array.IndexOf(encabezados, "Núm. parte del cliente")] : null,  //data[17]
                    msa_honda = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "MSA (Honda)")]) ? array[Array.IndexOf(encabezados, "MSA (Honda)")] : null,  //data[44]
                    diametro_interior = diametro_interior,
                    diametro_exterior = diametro_exterior,
                    comentarios = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Comentario Adicional")]) ? array[Array.IndexOf(encabezados, "Comentario Adicional")] : null,
                    nuevo_dato = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Otro Dato")]) ? array[Array.IndexOf(encabezados, "Otro Dato")] : null,
                    tipo_material_text = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de Material")]) ? array[Array.IndexOf(encabezados, "Tipo de Material")] : null,
                };

                //actualiza los cambios vs el material de referencia (BD)
                string cambiosString = GetCambiosCambioIngenieria(cambio);
                if (!string.IsNullOrEmpty(cambiosString))
                    cambio.cambios = UsoStrings.RecortaString(cambiosString, 2000);

                resultado.Add(cambio);
            }

            return resultado;
        }
        private List<SCDM_solicitud_rel_cambio_budget> ConvierteArrayACambiosBudget(List<string[]> data, int? id_solicitud)
        {
            List<SCDM_solicitud_rel_cambio_budget> resultado = new List<SCDM_solicitud_rel_cambio_budget> { };

            //variables globales para el metodo
            var BD_SCDM_cat_tipo_venta = db.SCDM_cat_tipo_venta.Where(x => x.activo == true).ToList();
            var BD_SCDM_planta = db.plantas.Where(x => x.activo == true).ToList();
            var BD_SCDM_cat_tipo_material = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList();


            //listado de encabezados
            string[] encabezados = {
               "ID", "Material Existente", "Tipo Material", "Planta","Peso bruto REAL bascula (kg)","Peso neto REAL bascula (kg)","Ángulo A", "Ángulo B", "Scrap permitido Puntas y Colas"
               ,"Piezas Dobles", "Reaplicación", "Conciliación puntas y colas", "Conciliación Scrap Ingenieria", "Tipo de Metal", "Tipo de Material", "Tipo de Venta", "Modelo de Negocio", "Posición de Rollo"
               , "IHS num 1", "IHS num 2", "IHS num 3", "IHS num 4", "IHS num 5"
               , "Piezas por auto", "Piezas por golpe", "Piezas por paquete", "Peso Inicial", "Peso Máximo", "Peso Maximo Tolerancia Positiva", "Peso Maximo Tolerancia Negativa"
               , "Peso Minimo", "Peso Mínimo Tolerancia Positiva", "Peso Mínimo Tolerancia Negativa"
               ,"Valido"
            };

            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                //declaración de variables
                int? id_planta = null;
                int id_item = 0;
                double? peso_bruto_real_bascula = null, peso_neto_real_bascula = null, angulo_a = null, angulo_b = null, scrap_permitido_puntas_colas = null,
                    piezas_por_auto = null, piezas_por_golpe = null, piezas_por_paquete = null, peso_inicial = null, peso_maximo = null, peso_maximo_tolerancia_positiva = null,
                    peso_maximo_tolerancia_negativa = null, peso_minimo = null, peso_minimo_tolerancia_positiva = null, peso_minimo_tolerancia_negativa = null;
                bool reaplicacion = false, conciliacion_puntas_colas = false, conciliacion_scrap_ingenieria = false;

                #region Asignacion de variables
                //id_creacion
                if (int.TryParse(array[Array.IndexOf(encabezados, "ID")], out int id_cr))
                    id_item = id_cr;

                //planta
                var tempPlanta = array[Array.IndexOf(encabezados, "Planta")];
                if (tempPlanta != null && !string.IsNullOrEmpty(tempPlanta) && BD_SCDM_planta.Any(x => x.ConcatPlantaSap.Trim() == tempPlanta))
                    id_planta = BD_SCDM_planta.FirstOrDefault(x => x.ConcatPlantaSap.Trim() == tempPlanta).clave;

                //peso_bruto
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso bruto REAL bascula (kg)")], out double peso_bruto_result))
                    peso_bruto_real_bascula = peso_bruto_result;

                //peso_neto
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso neto REAL bascula (kg)")], out double peso_neto_result))
                    peso_neto_real_bascula = peso_neto_result;

                //angulo a
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Ángulo A")], out double angulo_a_result))
                    angulo_a = angulo_a_result;

                //angulo b
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Ángulo B")], out double angulo_b_result))
                    angulo_b = angulo_b_result;

                //Scrap permitido Puntas y Colas
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Scrap permitido Puntas y Colas")], out double scrap_permitido_puntas_colas_result))
                    scrap_permitido_puntas_colas = scrap_permitido_puntas_colas_result;

                //reaplicacion
                if (bool.TryParse(array[Array.IndexOf(encabezados, "Reaplicación")], out bool reaplicacion_result))
                    reaplicacion = reaplicacion_result;

                //Conciliación puntas y colas
                if (bool.TryParse(array[Array.IndexOf(encabezados, "Conciliación puntas y colas")], out bool conciliacion_puntas_colas_result))
                    conciliacion_puntas_colas = conciliacion_puntas_colas_result;

                //reaplicacion
                if (bool.TryParse(array[Array.IndexOf(encabezados, "Conciliación Scrap Ingenieria")], out bool conciliacion_scrap_ingenieria_result))
                    conciliacion_scrap_ingenieria = conciliacion_scrap_ingenieria_result;

                //Piezas por auto
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Piezas por auto")], out double piezas_por_auto_result))
                    piezas_por_auto = piezas_por_auto_result;

                //Piezas por golpe
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Piezas por golpe")], out double piezas_por_golpe_result))
                    piezas_por_golpe = piezas_por_golpe_result;

                //Piezas por paquete
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Piezas por paquete")], out double piezas_por_paquete_result))
                    piezas_por_paquete = piezas_por_paquete_result;

                //Peso Inicial
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Inicial")], out double peso_inicial_result))
                    peso_inicial = peso_inicial_result;

                //Peso Máximo
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Máximo")], out double peso_maximo_result))
                    peso_maximo = peso_maximo_result;

                //Peso Maximo Tolerancia Positiva
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Maximo Tolerancia Positiva")], out double peso_maximo_tolerancia_positiva_result))
                    peso_maximo_tolerancia_positiva = peso_maximo_tolerancia_positiva_result;

                //Peso Maximo Tolerancia Negativa
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Maximo Tolerancia Negativa")], out double peso_maximo_tolerancia_negativa_result))
                    peso_maximo_tolerancia_negativa = peso_maximo_tolerancia_negativa_result;

                //Peso Minimo
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Minimo")], out double peso_minimo_result))
                    peso_minimo = peso_minimo_result;

                //Peso Mínimo Tolerancia Positiva
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Mínimo Tolerancia Positiva")], out double peso_minimo_tolerancia_positiva_result))
                    peso_minimo_tolerancia_positiva = peso_minimo_tolerancia_positiva_result;

                //Peso Mínimo Tolerancia Negativa
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Peso Mínimo Tolerancia Negativa")], out double peso_minimo_tolerancia_negativa_result))
                    peso_minimo_tolerancia_negativa = peso_minimo_tolerancia_negativa_result;


                #endregion

                SCDM_solicitud_rel_cambio_budget cambio = new SCDM_solicitud_rel_cambio_budget
                {
                    num_fila = data.IndexOf(array),
                    id_solicitud = id_solicitud.Value,
                    id = id_item,
                    id_planta = id_planta,
                    material_existente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Material Existente")]) ? array[Array.IndexOf(encabezados, "Material Existente")].ToUpper() : null,
                    peso_bruto_real_bascula = peso_bruto_real_bascula,
                    peso_neto_real_bascula = peso_neto_real_bascula,
                    angulo_a = angulo_a,
                    angulo_b = angulo_b,
                    scrap_permitido_puntas_colas = scrap_permitido_puntas_colas,
                    pieza_doble = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Piezas Dobles")]) ? array[Array.IndexOf(encabezados, "Piezas Dobles")] : null,
                    reaplicacion = reaplicacion,
                    conciliacion_puntas_colas = conciliacion_puntas_colas,
                    conciliacion_scrap_ingenieria = conciliacion_scrap_ingenieria,
                    tipo_metal = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de Metal")]) ? array[Array.IndexOf(encabezados, "Tipo de Metal")].ToString() : null,
                    tipo_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de Material")]) ? array[Array.IndexOf(encabezados, "Tipo de Material")].ToString() : null,
                    tipo_venta = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de Venta")]) ? array[Array.IndexOf(encabezados, "Tipo de Venta")].ToString() : null,
                    modelo_negocio = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Modelo de Negocio")]) ? array[Array.IndexOf(encabezados, "Modelo de Negocio")].ToString() : null,
                    posicion_rollo = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Posición de Rollo")]) ? array[Array.IndexOf(encabezados, "Posición de Rollo")].ToString() : null,
                    IHS_num_1 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "IHS num 1")]) ? array[Array.IndexOf(encabezados, "IHS num 1")].ToString() : null,
                    IHS_num_2 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "IHS num 2")]) ? array[Array.IndexOf(encabezados, "IHS num 2")].ToString() : null,
                    IHS_num_3 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "IHS num 3")]) ? array[Array.IndexOf(encabezados, "IHS num 3")].ToString() : null,
                    IHS_num_4 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "IHS num 4")]) ? array[Array.IndexOf(encabezados, "IHS num 4")].ToString() : null,
                    IHS_num_5 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "IHS num 5")]) ? array[Array.IndexOf(encabezados, "IHS num 5")].ToString() : null,
                    piezas_por_auto = piezas_por_auto,
                    piezas_por_golpe = piezas_por_golpe,
                    piezas_por_paquete = piezas_por_paquete,
                    peso_inicial = peso_inicial,
                    peso_maximo = peso_maximo,
                    peso_maximo_tolerancia_positiva = peso_maximo_tolerancia_positiva,
                    peso_maximo_tolerancia_negativa = peso_maximo_tolerancia_negativa,
                    peso_minimo = peso_minimo,
                    peso_minimo_tolerancia_positiva = peso_minimo_tolerancia_positiva,
                    peso_minimo_tolerancia_negativa = peso_minimo_tolerancia_negativa,
                };


                resultado.Add(cambio);
            }

            return resultado;
        }

        private string GetCambiosCambioIngenieria(SCDM_solicitud_rel_cambio_ingenieria cambio)
        {
            string result = string.Empty;
            //declaracion de variables
            string commodityString = string.Empty;
            string shapeString = string.Empty;
            string clienteString = string.Empty;
            string tipoMetalString = string.Empty;
            string tipoMaterialString = String.Empty;
            string typeSellingString = String.Empty;
            string espesor = string.Empty;
            string espesor_tolerancia_negativa = string.Empty;
            string espesor_tolerancia_positiva = string.Empty;
            string ancho = string.Empty;
            string avance = string.Empty;
            string plantaString = string.Empty;

            //obtiene la planta de la solicitud
            plantas planta = db.plantas.Find(cambio.id_planta);

            //obtiene el valor de mm
            mm_v3 mm = null;

            //obtiene todos los posibles valores
            var mmList = db.mm_v3.Where(x => x.Material == cambio.material_existente);

            //trata de obtener el valor que corresponde a la planta
            if (mmList.Any(x => x.Plnt == planta.codigoSap))
                mm = mmList.FirstOrDefault(x => x.Plnt == planta.codigoSap);
            else
            {
                mm = mmList.FirstOrDefault();
            }

            class_v3 class_v3 = db.class_v3.FirstOrDefault(x => x.Object == cambio.material_existente);

            if (class_v3 == null)
                class_v3 = new class_v3
                {
                    Object = mm.Material,
                    Grade = string.Empty,
                    Customer = string.Empty,
                    Shape = string.Empty,
                    Customer_part_number = string.Empty,
                    Surface = string.Empty,
                    Gauge___Metric = string.Empty,
                    Mill = string.Empty,
                    Width___Metr = string.Empty,
                    Length_mm_ = string.Empty,
                    activo = true,
                    commodity = string.Empty,
                    flatness_metric = string.Empty,
                    surface_treatment = string.Empty,
                    coating_weight = string.Empty,
                    customer_part_msa = string.Empty,
                    outer_diameter_coil = string.Empty,
                    inner_diameter_coil = string.Empty
                };
            else //establece los valores 
            {
                SCDM_cat_commodity commodity = db.SCDM_cat_commodity.FirstOrDefault(x => x.descripcion == class_v3.commodity);
                commodityString = commodity != null ? commodity.ConcatCommodity : string.Empty;

                //SHAPE
                SCDM_cat_forma_material shape = db.SCDM_cat_forma_material.FirstOrDefault(x => x.descripcion_en == class_v3.Shape);
                shapeString = shape != null ? shape.descripcion : string.Empty;

                //Cliente
                clientes cliente = db.clientes.FirstOrDefault(x => x.claveSAP == class_v3.Customer);
                clienteString = cliente != null ? cliente.ConcatClienteSAP : string.Empty;

                //type of selling
                SCDM_cat_tipo_venta tipoVenta = db.SCDM_cat_tipo_venta.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.Type_of_Selling.ToUpper());
                typeSellingString = tipoVenta != null ? tipoVenta.descripcion : mm.Type_of_Selling;

                //tipo de metal
                SCDM_cat_tipo_metal tipoMetal = db.SCDM_cat_tipo_metal.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.Type_of_Metal.ToUpper());
                if (tipoMetal != null)
                {
                    tipoMetalString = tipoMetal.descripcion;
                }
                else
                {
                    SCDM_cat_tipo_metal_cb tipoMetalCB = db.SCDM_cat_tipo_metal_cb.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.Type_of_Metal.ToUpper());
                    tipoMetalString = tipoMetalCB != null ? tipoMetalCB.descripcion : string.Empty;
                }

                //obtiene dimensiones
                string[] dimensiones = !string.IsNullOrEmpty(mm.size_dimensions) ? mm.size_dimensions.Replace(" ", string.Empty).ToUpper().Split('X') : new string[0];

                if (dimensiones.Length >= 1)
                    espesor = dimensiones[0];
                if (dimensiones.Length >= 2)
                    ancho = dimensiones[1];
                if (dimensiones.Length >= 3)
                    avance = dimensiones[2];
            }
            try
            {

                //crea la cadena para cambios
                result += tipoMetalString != cambio.tipo_metal ? " [Tipo Metal] ANT: " + tipoMetalString + "  NUEVO: " + cambio.tipo_metal + "|" : string.Empty;
                result += typeSellingString != cambio.tipo_venta ? " [Tipo Venta] ANT: " + typeSellingString + "  NUEVO: " + cambio.tipo_venta + "|" : string.Empty;
                result += mm.Old_material_no_ != cambio.numero_antiguo_material ? " [Núm Antigüo] ANT: " + mm.Old_material_no_ + "  NUEVO: " + cambio.numero_antiguo_material + "|" : string.Empty;
                result += mm.Gross_weight.ToString() != cambio.peso_bruto.ToString() ? " [Peso Bruto] ANT: " + mm.Gross_weight.ToString() + "  NUEVO: " + cambio.peso_bruto.ToString() + "|" : string.Empty;
                result += mm.Net_weight.ToString() != cambio.peso_neto.ToString() ? " [Peso Neto] ANT: " + mm.Net_weight.ToString() + "  NUEVO: " + cambio.peso_neto.ToString() + "|" : string.Empty;
                result += mm.unidad_medida != cambio.unidad_medida_inventario ? " [Unidad Medida] ANT: " + mm.unidad_medida + "  NUEVO: " + cambio.unidad_medida_inventario + "|" : string.Empty;
                result += mm.Material_Description != cambio.descripcion_en ? " [Descripción EN] ANT: " + mm.Material_Description + "  NUEVO: " + cambio.descripcion_en + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(commodityString) || !string.IsNullOrEmpty(cambio.commodity)) && commodityString != cambio.commodity ? " [Commodity] ANT: " + commodityString + "  NUEVO: " + cambio.commodity + "|" : string.Empty;
                result += class_v3.Grade != cambio.grado_calidad ? " [Grado/calidad] ANT: " + class_v3.Grade + "  NUEVO: " + cambio.grado_calidad + "|" : string.Empty;
                //espesor
                result += (!string.IsNullOrEmpty(espesor) || !string.IsNullOrEmpty(cambio.espesor_mm.ToString())) && espesor != cambio.espesor_mm.ToString() ? " [Espesor] ANT: " + espesor + "  NUEVO: " + cambio.espesor_mm.ToString() + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(getTolerancia(class_v3.Gauge___Metric, espesor, "min")) || !string.IsNullOrEmpty(cambio.espesor_tolerancia_negativa_mm.ToString())) && getTolerancia(class_v3.Gauge___Metric, espesor, "min") != cambio.espesor_tolerancia_negativa_mm.ToString() ? " [Espesor (-)] ANT: " + getTolerancia(class_v3.Gauge___Metric, espesor, "min") + "  NUEVO: " + cambio.espesor_tolerancia_negativa_mm.ToString() + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(getTolerancia(class_v3.Gauge___Metric, espesor, "max")) || !string.IsNullOrEmpty(cambio.espesor_tolerancia_positiva_mm.ToString())) && getTolerancia(class_v3.Gauge___Metric, espesor, "max") != cambio.espesor_tolerancia_positiva_mm.ToString() ? " [Espesor (+)] ANT: " + getTolerancia(class_v3.Gauge___Metric, espesor, "max") + "  NUEVO: " + cambio.espesor_tolerancia_positiva_mm.ToString() + "|" : string.Empty;
                //ancho
                result += (!string.IsNullOrEmpty(ancho) || !string.IsNullOrEmpty(cambio.ancho_mm.ToString())) && ancho != cambio.ancho_mm.ToString() ? " [Ancho] ANT: " + ancho + "  NUEVO: " + cambio.ancho_mm.ToString() + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(getTolerancia(class_v3.Width___Metr, ancho, "min")) || !string.IsNullOrEmpty(cambio.ancho_tolerancia_negativa_mm.ToString())) && getTolerancia(class_v3.Width___Metr, ancho, "min") != cambio.ancho_tolerancia_negativa_mm.ToString() ? " [Ancho (-)] ANT: " + getTolerancia(class_v3.Width___Metr, ancho, "min") + "  NUEVO: " + cambio.ancho_tolerancia_negativa_mm.ToString() + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(getTolerancia(class_v3.Width___Metr, ancho, "max")) || !string.IsNullOrEmpty(cambio.ancho_tolerancia_positiva_mm.ToString())) && getTolerancia(class_v3.Width___Metr, ancho, "max") != cambio.ancho_tolerancia_positiva_mm.ToString() ? " [Ancho (+)] ANT: " + getTolerancia(class_v3.Width___Metr, ancho, "max") + "  NUEVO: " + cambio.ancho_tolerancia_positiva_mm.ToString() + "|" : string.Empty;
                //avance
                result += (!string.IsNullOrEmpty(avance) || !string.IsNullOrEmpty(cambio.avance_mm.ToString())) && avance != cambio.avance_mm.ToString() ? " [Avance] ANT: " + avance + "  NUEVO: " + cambio.avance_mm.ToString() + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(getTolerancia(class_v3.Length_mm_, avance, "min")) || !string.IsNullOrEmpty(cambio.avance_tolerancia_negativa_mm.ToString())) && getTolerancia(class_v3.Length_mm_, avance, "min") != cambio.avance_tolerancia_negativa_mm.ToString() ? " [Avance (-)] ANT: " + getTolerancia(class_v3.Length_mm_, avance, "min") + "  NUEVO: " + cambio.avance_tolerancia_negativa_mm.ToString() + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(getTolerancia(class_v3.Length_mm_, avance, "max")) || !string.IsNullOrEmpty(cambio.avance_tolerancia_positiva_mm.ToString())) && getTolerancia(class_v3.Length_mm_, avance, "max") != cambio.avance_tolerancia_positiva_mm.ToString() ? " [Avance (+)] ANT: " + getTolerancia(class_v3.Length_mm_, avance, "max") + "  NUEVO: " + cambio.avance_tolerancia_positiva_mm.ToString() + "|" : string.Empty;
                //planicidad
                bool planicidadIguales = false;
                if (double.TryParse(String.Concat(class_v3.flatness_metric.Where(x => x == '.' || Char.IsDigit(x))), out double o_planinidad) && double.TryParse(cambio.planicidad_mm.ToString(), out double c_planicidad))
                {
                    planicidadIguales = o_planinidad == c_planicidad;
                }
                result += (!string.IsNullOrEmpty(String.Concat(class_v3.flatness_metric.Where(x => x == '.' || Char.IsDigit(x)))) || !string.IsNullOrEmpty(cambio.planicidad_mm.ToString())) && String.Concat(class_v3.flatness_metric.Where(x => x == '.' || Char.IsDigit(x))) != cambio.planicidad_mm.ToString() && !planicidadIguales ? " [Planicidad] ANT: " + String.Concat(class_v3.flatness_metric.Where(x => x == '.' || Char.IsDigit(x))) + "  NUEVO: " + cambio.planicidad_mm.ToString() + "|" : string.Empty;
                //superficie
                result += (!string.IsNullOrEmpty(class_v3.Surface) || !string.IsNullOrEmpty(cambio.superficie)) && class_v3.Surface != cambio.superficie ? " [Superficie] ANT: " + class_v3.Surface + "  NUEVO: " + cambio.superficie + "|" : string.Empty;
                //tratamiento superficial
                result += (!string.IsNullOrEmpty(class_v3.surface_treatment) || !string.IsNullOrEmpty(cambio.tratamiento_superficial)) && class_v3.surface_treatment != cambio.tratamiento_superficial ? " [Tratamiento Superficial] ANT: " + class_v3.surface_treatment + "  NUEVO: " + cambio.tratamiento_superficial + "|" : string.Empty;
                //Peso Recubrimiento
                result += (!string.IsNullOrEmpty(class_v3.coating_weight) || !string.IsNullOrEmpty(cambio.peso_recubrimiento)) && class_v3.coating_weight != cambio.peso_recubrimiento ? " [Peso Recubrimiento] ANT: " + class_v3.coating_weight + "  NUEVO: " + cambio.peso_recubrimiento + "|" : string.Empty;
                //Molino
                result += (!string.IsNullOrEmpty(class_v3.Mill) || !string.IsNullOrEmpty(cambio.molino)) && class_v3.Mill != cambio.molino ? " [Molino] ANT: " + class_v3.Mill + "  NUEVO: " + cambio.molino + "|" : string.Empty;
                //Forma
                result += (!string.IsNullOrEmpty(shapeString) || !string.IsNullOrEmpty(cambio.forma)) && shapeString != cambio.forma ? " [Forma] ANT: " + shapeString + "  NUEVO: " + cambio.forma + "|" : string.Empty;
                //clienteString
                result += (!string.IsNullOrEmpty(clienteString) || !string.IsNullOrEmpty(cambio.cliente)) && clienteString != cambio.cliente ? " [Cliente] ANT: " + clienteString + "  NUEVO: " + cambio.cliente + "|" : string.Empty;
                //numero parte
                result += (!string.IsNullOrEmpty(class_v3.Customer_part_number) || !string.IsNullOrEmpty(cambio.numero_parte)) && class_v3.Customer_part_number != cambio.numero_parte ? " [Núm. Parte] ANT: " + class_v3.Customer_part_number + "  NUEVO: " + cambio.numero_parte + "|" : string.Empty;
                //msa
                result += (!string.IsNullOrEmpty(class_v3.customer_part_msa) || !string.IsNullOrEmpty(cambio.msa_honda)) && class_v3.customer_part_msa != cambio.msa_honda ? " [MSA] ANT: " + class_v3.customer_part_msa + "  NUEVO: " + cambio.msa_honda + "|" : string.Empty;

                //diametro exterior
                string diametro_exterior = String.Concat(class_v3.outer_diameter_coil.Where(x => x == '.' || Char.IsDigit(x)));
                bool diametroIntIguales = false;
                if (double.TryParse(diametro_exterior, out double o_exterior) && double.TryParse(cambio.diametro_exterior.ToString(), out double c_exterior))
                {
                    diametroIntIguales = o_exterior == c_exterior;
                }
                result += (!string.IsNullOrEmpty(diametro_exterior) || !string.IsNullOrEmpty(cambio.diametro_exterior.ToString())) && diametro_exterior != cambio.diametro_exterior.ToString() && !diametroIntIguales ? " [Diametro Ext.] ANT: " + diametro_exterior + "  NUEVO: " + cambio.diametro_exterior.ToString() + "|" : string.Empty;

                //diametro interior
                string diametro_interior = !string.IsNullOrEmpty(class_v3.inner_diameter_coil) ? float.Parse(String.Concat(class_v3.inner_diameter_coil.Where(x => x == '.' || Char.IsDigit(x)))).ToString() : string.Empty;
                result += (!string.IsNullOrEmpty(diametro_interior) || !string.IsNullOrEmpty(cambio.diametro_interior.ToString())) && diametro_interior != cambio.diametro_interior.ToString() ? " [Diametro Int.] ANT: " + diametro_interior + "  NUEVO: " + cambio.diametro_interior.ToString() + "|" : string.Empty;



                //elimina los espacios al inicio y final
                result = result.Trim();
                return result;
            }
            catch (Exception)
            {
                return "Error al obtener los cambios.";
            };
        }
        private string GetCambiosCreacionReferencia(SCDM_solicitud_rel_creacion_referencia creacionR)
        {
            string result = string.Empty;
            //declaracion de variables
            string commodityString = string.Empty;
            string shapeString = string.Empty;
            string clienteString = string.Empty;
            string tipoMetalString = string.Empty;
            string tipoMaterialString = String.Empty;
            string typeSellingString = String.Empty;
            string espesor = string.Empty;
            string espesor_tolerancia_negativa = string.Empty;
            string espesor_tolerancia_positiva = string.Empty;
            string ancho = string.Empty;
            string avance = string.Empty;
            string plantaString = string.Empty;
            string tipoVentaCreacionText = string.Empty;


            //obtiene la planta de la solicitud
            plantas planta = db.plantas.Find(creacionR.id_planta);

            //obtiene el valor de mm
            mm_v3 mm = null;

            //obtiene todos los posibles valores
            var mmList = db.mm_v3.Where(x => x.Material == creacionR.material_existente);

            //trata de obtener el valor que corresponde a la planta
            if (mmList.Any(x => x.Plnt == planta.codigoSap))
                mm = mmList.FirstOrDefault(x => x.Plnt == planta.codigoSap);
            else
            {
                mm = mmList.FirstOrDefault();
            }

            class_v3 class_v3 = db.class_v3.FirstOrDefault(x => x.Object == creacionR.material_existente);

            if (class_v3 == null)
                class_v3 = new class_v3
                {
                    Object = mm.Material,
                    Grade = string.Empty,
                    Customer = string.Empty,
                    Shape = string.Empty,
                    Customer_part_number = string.Empty,
                    Surface = string.Empty,
                    Gauge___Metric = string.Empty,
                    Mill = string.Empty,
                    Width___Metr = string.Empty,
                    Length_mm_ = string.Empty,
                    activo = true,
                    commodity = string.Empty,
                    flatness_metric = string.Empty,
                    surface_treatment = string.Empty,
                    coating_weight = string.Empty,
                    customer_part_msa = string.Empty,
                    outer_diameter_coil = string.Empty,
                    inner_diameter_coil = string.Empty
                };
            else //establece los valores 
            {
                SCDM_cat_commodity commodity = db.SCDM_cat_commodity.FirstOrDefault(x => x.descripcion == class_v3.commodity);
                commodityString = commodity != null ? commodity.ConcatCommodity : string.Empty;

                //SHAPE
                SCDM_cat_forma_material shape = db.SCDM_cat_forma_material.FirstOrDefault(x => x.descripcion_en == class_v3.Shape);
                shapeString = shape != null ? shape.descripcion : string.Empty;

                //Cliente
                clientes cliente = db.clientes.FirstOrDefault(x => x.claveSAP == class_v3.Customer);
                clienteString = cliente != null ? cliente.ConcatClienteSAP : string.Empty;

                //type of selling
                SCDM_cat_tipo_venta tipoVenta = db.SCDM_cat_tipo_venta.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.Type_of_Selling.ToUpper());
                typeSellingString = tipoVenta != null ? tipoVenta.descripcion : mm.Type_of_Selling;

                //tipo de venta de la creacion
                SCDM_cat_tipo_venta tipoVentaCreacion = db.SCDM_cat_tipo_venta.Find(creacionR.id_tipo_venta);

                if (tipoVentaCreacion != null)
                    tipoVentaCreacionText = tipoVentaCreacion.descripcion;


                //tipo de metal
                SCDM_cat_tipo_metal tipoMetal = db.SCDM_cat_tipo_metal.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.Type_of_Metal.ToUpper());
                if (tipoMetal != null)
                {
                    tipoMetalString = tipoMetal.descripcion;
                }
                else
                {
                    SCDM_cat_tipo_metal_cb tipoMetalCB = db.SCDM_cat_tipo_metal_cb.ToList().FirstOrDefault(x => x.descripcion.ToUpper() == mm.Type_of_Metal.ToUpper());
                    tipoMetalString = tipoMetalCB != null ? tipoMetalCB.descripcion : string.Empty;
                }

                //obtiene dimensiones
                string[] dimensiones = !string.IsNullOrEmpty(mm.size_dimensions) ? mm.size_dimensions.Replace(" ", string.Empty).ToUpper().Split('X') : new string[0];

                if (dimensiones.Length >= 1)
                    espesor = dimensiones[0];
                if (dimensiones.Length >= 2)
                    ancho = dimensiones[1];
                if (dimensiones.Length >= 3)
                    avance = dimensiones[2];
            }
            try
            {

                //crea la cadena para cambios
                result += tipoMetalString != creacionR.tipo_metal ? " [Tipo Metal] ANT: " + tipoMetalString + "  NUEVO: " + creacionR.tipo_metal + "|" : string.Empty;
                result += typeSellingString != tipoVentaCreacionText ? " [Tipo Venta] ANT: " + typeSellingString + "  NUEVO: " + tipoVentaCreacionText + "|" : string.Empty;
                result += mm.Old_material_no_ != creacionR.numero_antiguo_material ? " [Núm Antigüo] ANT: " + mm.Old_material_no_ + "  NUEVO: " + creacionR.numero_antiguo_material + "|" : string.Empty;
                result += mm.Gross_weight.ToString() != creacionR.peso_bruto.ToString() ? " [Peso Bruto] ANT: " + mm.Gross_weight.ToString() + "  NUEVO: " + creacionR.peso_bruto.ToString() + "|" : string.Empty;
                result += mm.Net_weight.ToString() != creacionR.peso_neto.ToString() ? " [Peso Neto] ANT: " + mm.Net_weight.ToString() + "  NUEVO: " + creacionR.peso_neto.ToString() + "|" : string.Empty;
                result += mm.unidad_medida != creacionR.unidad_medida_inventario ? " [Unidad Medida] ANT: " + mm.unidad_medida + "  NUEVO: " + creacionR.unidad_medida_inventario + "|" : string.Empty;


                result += mm.Material_Description != creacionR.descripcion_en ? " [Descripción EN] ANT: " + mm.Material_Description + "  NUEVO: " + creacionR.descripcion_en + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(commodityString) || !string.IsNullOrEmpty(creacionR.commodity)) && commodityString != creacionR.commodity ? " [Commodity] ANT: " + commodityString + "  NUEVO: " + creacionR.commodity + "|" : string.Empty;
                result += class_v3.Grade != creacionR.grado_calidad ? " [Grado/calidad] ANT: " + class_v3.Grade + "  NUEVO: " + creacionR.grado_calidad + "|" : string.Empty;
                //espesor
                result += !NumerosIguales(espesor, creacionR.espesor_mm.ToString()) ? " [Espesor] ANT: " + espesor + "  NUEVO: " + creacionR.espesor_mm.ToString() + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(getTolerancia(class_v3.Gauge___Metric, espesor, "min")) || !string.IsNullOrEmpty(creacionR.espesor_tolerancia_negativa_mm.ToString())) && getTolerancia(class_v3.Gauge___Metric, espesor, "min") != creacionR.espesor_tolerancia_negativa_mm.ToString() ? " [Espesor (-)] ANT: " + getTolerancia(class_v3.Gauge___Metric, espesor, "min") + "  NUEVO: " + creacionR.espesor_tolerancia_negativa_mm.ToString() + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(getTolerancia(class_v3.Gauge___Metric, espesor, "max")) || !string.IsNullOrEmpty(creacionR.espesor_tolerancia_positiva_mm.ToString())) && getTolerancia(class_v3.Gauge___Metric, espesor, "max") != creacionR.espesor_tolerancia_positiva_mm.ToString() ? " [Espesor (+)] ANT: " + getTolerancia(class_v3.Gauge___Metric, espesor, "max") + "  NUEVO: " + creacionR.espesor_tolerancia_positiva_mm.ToString() + "|" : string.Empty;
                //ancho
                result += !NumerosIguales(ancho, creacionR.ancho_mm.ToString()) ? " [Ancho] ANT: " + ancho + "  NUEVO: " + creacionR.ancho_mm.ToString() + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(getTolerancia(class_v3.Width___Metr, ancho, "min")) || !string.IsNullOrEmpty(creacionR.ancho_tolerancia_negativa_mm.ToString())) && getTolerancia(class_v3.Width___Metr, ancho, "min") != creacionR.ancho_tolerancia_negativa_mm.ToString() ? " [Ancho (-)] ANT: " + getTolerancia(class_v3.Width___Metr, ancho, "min") + "  NUEVO: " + creacionR.ancho_tolerancia_negativa_mm.ToString() + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(getTolerancia(class_v3.Width___Metr, ancho, "max")) || !string.IsNullOrEmpty(creacionR.ancho_tolerancia_positiva_mm.ToString())) && getTolerancia(class_v3.Width___Metr, ancho, "max") != creacionR.ancho_tolerancia_positiva_mm.ToString() ? " [Ancho (+)] ANT: " + getTolerancia(class_v3.Width___Metr, ancho, "max") + "  NUEVO: " + creacionR.ancho_tolerancia_positiva_mm.ToString() + "|" : string.Empty;
                //avance
                result += !NumerosIguales(avance, creacionR.avance_mm.ToString()) ? " [Espesor] ANT: " + avance + "  NUEVO: " + creacionR.avance_mm.ToString() + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(getTolerancia(class_v3.Length_mm_, avance, "min")) || !string.IsNullOrEmpty(creacionR.avance_tolerancia_negativa_mm.ToString())) && getTolerancia(class_v3.Length_mm_, avance, "min") != creacionR.avance_tolerancia_negativa_mm.ToString() ? " [Avance (-)] ANT: " + getTolerancia(class_v3.Length_mm_, avance, "min") + "  NUEVO: " + creacionR.avance_tolerancia_negativa_mm.ToString() + "|" : string.Empty;
                result += (!string.IsNullOrEmpty(getTolerancia(class_v3.Length_mm_, avance, "max")) || !string.IsNullOrEmpty(creacionR.avance_tolerancia_positiva_mm.ToString())) && getTolerancia(class_v3.Length_mm_, avance, "max") != creacionR.avance_tolerancia_positiva_mm.ToString() ? " [Avance (+)] ANT: " + getTolerancia(class_v3.Length_mm_, avance, "max") + "  NUEVO: " + creacionR.avance_tolerancia_positiva_mm.ToString() + "|" : string.Empty;
                //planicidad
                bool planicidadIguales = false;
                if (double.TryParse(String.Concat(class_v3.flatness_metric.Where(x => x == '.' || Char.IsDigit(x))), out double o_planinidad) && double.TryParse(creacionR.planicidad_mm.ToString(), out double c_planicidad))
                {
                    planicidadIguales = o_planinidad == c_planicidad;
                }
                result += (!string.IsNullOrEmpty(String.Concat(class_v3.flatness_metric.Where(x => x == '.' || Char.IsDigit(x)))) || !string.IsNullOrEmpty(creacionR.planicidad_mm.ToString())) && String.Concat(class_v3.flatness_metric.Where(x => x == '.' || Char.IsDigit(x))) != creacionR.planicidad_mm.ToString() && !planicidadIguales ? " [Planicidad] ANT: " + String.Concat(class_v3.flatness_metric.Where(x => x == '.' || Char.IsDigit(x))) + "  NUEVO: " + creacionR.planicidad_mm.ToString() + "|" : string.Empty;
                //superficie
                result += (!string.IsNullOrEmpty(class_v3.Surface) || !string.IsNullOrEmpty(creacionR.superficie)) && class_v3.Surface != creacionR.superficie ? " [Superficie] ANT: " + class_v3.Surface + "  NUEVO: " + creacionR.superficie + "|" : string.Empty;
                //tratamiento superficial
                result += (!string.IsNullOrEmpty(class_v3.surface_treatment) || !string.IsNullOrEmpty(creacionR.tratamiento_superficial)) && class_v3.surface_treatment != creacionR.tratamiento_superficial ? " [Tratamiento Superficial] ANT: " + class_v3.surface_treatment + "  NUEVO: " + creacionR.tratamiento_superficial + "|" : string.Empty;
                //Peso Recubrimiento
                result += (!string.IsNullOrEmpty(class_v3.coating_weight) || !string.IsNullOrEmpty(creacionR.peso_recubrimiento)) && class_v3.coating_weight != creacionR.peso_recubrimiento ? " [Peso Recubrimiento] ANT: " + class_v3.coating_weight + "  NUEVO: " + creacionR.peso_recubrimiento + "|" : string.Empty;
                //Molino
                result += (!string.IsNullOrEmpty(class_v3.Mill) || !string.IsNullOrEmpty(creacionR.molino)) && class_v3.Mill != creacionR.molino ? " [Molino] ANT: " + class_v3.Mill + "  NUEVO: " + creacionR.molino + "|" : string.Empty;
                //Forma
                result += (!string.IsNullOrEmpty(shapeString) || !string.IsNullOrEmpty(creacionR.forma)) && shapeString != creacionR.forma ? " [Forma] ANT: " + shapeString + "  NUEVO: " + creacionR.forma + "|" : string.Empty;
                //clienteString
                result += (!string.IsNullOrEmpty(clienteString) || !string.IsNullOrEmpty(creacionR.cliente)) && clienteString != creacionR.cliente ? " [Cliente] ANT: " + clienteString + "  NUEVO: " + creacionR.cliente + "|" : string.Empty;
                //numero parte
                result += (!string.IsNullOrEmpty(class_v3.Customer_part_number) || !string.IsNullOrEmpty(creacionR.numero_parte)) && class_v3.Customer_part_number != creacionR.numero_parte ? " [Núm. Parte] ANT: " + class_v3.Customer_part_number + "  NUEVO: " + creacionR.numero_parte + "|" : string.Empty;
                //msa
                result += (!string.IsNullOrEmpty(class_v3.customer_part_msa) || !string.IsNullOrEmpty(creacionR.msa_honda)) && class_v3.customer_part_msa != creacionR.msa_honda ? " [MSA] ANT: " + class_v3.customer_part_msa + "  NUEVO: " + creacionR.msa_honda + "|" : string.Empty;

                //diametro exterior
                string diametro_exterior = String.Concat(class_v3.outer_diameter_coil.Where(x => x == '.' || Char.IsDigit(x)));
                bool diametroIntIguales = false;
                if (double.TryParse(diametro_exterior, out double o_exterior) && double.TryParse(creacionR.diametro_exterior.ToString(), out double c_exterior))
                {
                    diametroIntIguales = o_exterior == c_exterior;
                }
                result += (!string.IsNullOrEmpty(diametro_exterior) || !string.IsNullOrEmpty(creacionR.diametro_exterior.ToString())) && diametro_exterior != creacionR.diametro_exterior.ToString() && !diametroIntIguales ? " [Diametro Ext.] ANT: " + diametro_exterior + "  NUEVO: " + creacionR.diametro_exterior.ToString() + "|" : string.Empty;

                //diametro interior
                string diametro_interior = !string.IsNullOrEmpty(class_v3.inner_diameter_coil) ? float.Parse(String.Concat(class_v3.inner_diameter_coil.Where(x => x == '.' || Char.IsDigit(x)))).ToString() : string.Empty;
                result += (!string.IsNullOrEmpty(diametro_interior) || !string.IsNullOrEmpty(creacionR.diametro_interior.ToString())) && diametro_interior != creacionR.diametro_interior.ToString() ? " [Diametro Int.] ANT: " + diametro_interior + "  NUEVO: " + creacionR.diametro_interior.ToString() + "|" : string.Empty;



                //elimina los espacios al inicio y final
                result = result.Trim();
                return result;
            }
            catch (Exception)
            {
                return "Error al obtener los cambios.";
            };
        }

        [NonAction]
        private bool NumerosIguales(string num_1, string num_2)
        {

            float f1 = 0;
            float f2 = 0;

            if (float.TryParse(num_1, out float result_1))
                f1 = result_1;
            if (float.TryParse(num_2, out float result_2))
                f2 = result_2;

            return f1 == f2;
        }
        private List<SCDM_solicitud_rel_activaciones> ConvierteArrayACambiosEstatus(List<string[]> data, int? id_solicitud)
        {
            List<SCDM_solicitud_rel_activaciones> resultado = new List<SCDM_solicitud_rel_activaciones> { };

            //variables globales para el metodo
            var BD_SCDM_planta = db.plantas.Where(x => x.activo == true).ToList();

            //listado de encabezados
            string[] encabezados = {
                "ID", "Material", "Planta","Sales Org", "Estatus - Planta", "Estatus - Dchain", "Fecha"
            };

            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                //declaración de variables
                int id_rel_cambio = 0;
                DateTime fecha = DateTime.Now;
                string codigoPlanta = string.Empty;

                //id_creacion
                if (int.TryParse(array[Array.IndexOf(encabezados, "ID")], out int id_cr))
                    id_rel_cambio = id_cr;

                //fecha
                if (DateTime.TryParseExact(array[Array.IndexOf(encabezados, "Fecha")], "dd/MM/yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out DateTime fecha_result))
                    fecha = fecha_result;

                //planta
                var tempPlanta = array[Array.IndexOf(encabezados, "Planta")];
                if (tempPlanta != null && !string.IsNullOrEmpty(tempPlanta) && BD_SCDM_planta.Any(x => x.ConcatPlantaSap.Trim() == tempPlanta))
                    codigoPlanta = BD_SCDM_planta.FirstOrDefault(x => x.ConcatPlantaSap.Trim() == tempPlanta).codigoSap;



                resultado.Add(new SCDM_solicitud_rel_activaciones
                {
                    num_fila = data.IndexOf(array),
                    id_solicitud = id_solicitud.Value,
                    id = id_rel_cambio,
                    material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Material")]) ? array[Array.IndexOf(encabezados, "Material")] : null,
                    planta = codigoPlanta,
                    sales_org = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Sales Org")]) ? array[Array.IndexOf(encabezados, "Sales Org")] : null,
                    estatus_planta = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Estatus - Planta")]) ? array[Array.IndexOf(encabezados, "Estatus - Planta")] : null,
                    estatus_dchain = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Estatus - Dchain")]) ? array[Array.IndexOf(encabezados, "Estatus - Dchain")] : null,
                    fecha = fecha,

                });
            }

            return resultado;
        }
        private List<SCDM_solicitud_rel_extension_usuario> ConvierteArrayAMaterialesExtension(List<string[]> data, int? id_solicitud)
        {
            List<SCDM_solicitud_rel_extension_usuario> resultado = new List<SCDM_solicitud_rel_extension_usuario> { };

            //variables globales para el metodo
            var BD_SCDM_planta = db.plantas.Where(x => x.activo == true).ToList();

            //listado de encabezados
            string[] encabezados = {
                "ID", "Material", "Planta Referencia","Planta Destino",
            };

            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                //declaración de variables
                int id_rel_extension_usuario = 0;
                DateTime fecha = DateTime.Now;
                string codigoPlantaDestino = string.Empty;
                string codigoPlantaReferencia = string.Empty;

                //id_creacion
                if (int.TryParse(array[Array.IndexOf(encabezados, "ID")], out int id_cr))
                    id_rel_extension_usuario = id_cr;

                //planta destino
                var tempPlantaDestino = array[Array.IndexOf(encabezados, "Planta Destino")];
                if (tempPlantaDestino != null && !string.IsNullOrEmpty(tempPlantaDestino) && BD_SCDM_planta.Any(x => x.ConcatPlantaSap.Trim() == tempPlantaDestino))
                    codigoPlantaDestino = BD_SCDM_planta.FirstOrDefault(x => x.ConcatPlantaSap.Trim() == tempPlantaDestino).codigoSap;

                //planta referencia
                var tempPlantaReferencia = array[Array.IndexOf(encabezados, "Planta Referencia")];
                if (tempPlantaReferencia != null && !string.IsNullOrEmpty(tempPlantaReferencia) && BD_SCDM_planta.Any(x => x.ConcatPlantaSap.Trim() == tempPlantaReferencia))
                    codigoPlantaReferencia = BD_SCDM_planta.FirstOrDefault(x => x.ConcatPlantaSap.Trim() == tempPlantaReferencia).codigoSap;

                resultado.Add(new SCDM_solicitud_rel_extension_usuario
                {
                    num_fila = data.IndexOf(array),
                    id_solicitud = id_solicitud.Value,
                    id = id_rel_extension_usuario,
                    material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Material")]) ? array[Array.IndexOf(encabezados, "Material")].ToUpper() : null,
                    planta_destino = codigoPlantaDestino,
                    planta_referencia = codigoPlantaReferencia,

                });
            }

            return resultado;
        }

        private List<SCDM_solicitud_rel_lista_tecnica> ConvierteArrayAListaTecnica(List<string[]> data, int? id_solicitud)
        {
            List<SCDM_solicitud_rel_lista_tecnica> resultado = new List<SCDM_solicitud_rel_lista_tecnica> { };


            //listado de encabezados
            string[] encabezados = {
                "ID", "Resultado", "Tipo Material<br>(resultado)","Tipo de Venta", "Peso Bruto<br>(platina)", "Peso Neto<br>(platina)", "Unidad de <br>medida", "Sobrante - mm<br>(platina)"
            , "Componente", "Tipo Material<br>(componente)", "Cantidad platinas<br>(platinas soldadas)", "Cantidad cintas <br>resultantes (cintas)", "Fecha validez<br>(reaplicaciones)"
            };

            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                //declaración de variables
                int id_lista_tecnica = 0;
                int? cantidad_platinas = null, cantidad_cintas = null;
                double? sobrante_mm = null;
                DateTime? fecha_validez = null;

                #region Asignacion de variables
                //id_creacion
                if (int.TryParse(array[Array.IndexOf(encabezados, "ID")], out int id_cr))
                    id_lista_tecnica = id_cr;

                //sobrante
                if (Double.TryParse(array[Array.IndexOf(encabezados, "Sobrante - mm<br>(platina)")], out double sobrante_mm_result))
                    sobrante_mm = sobrante_mm_result;

                //cantidad_platinas
                if (int.TryParse(array[Array.IndexOf(encabezados, "Cantidad platinas<br>(platinas soldadas)")], out int cantidad_platinas_result))
                    cantidad_platinas = cantidad_platinas_result;

                //cantidad_cintas
                if (int.TryParse(array[Array.IndexOf(encabezados, "Cantidad cintas <br>resultantes (cintas)")], out int cantidad_cintas_result))
                    cantidad_cintas = cantidad_cintas_result;

                //fecha_validez
                if (DateTime.TryParseExact(array[Array.IndexOf(encabezados, "Fecha validez<br>(reaplicaciones)")], "dd/MM/yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out DateTime fecha_validez_result))
                    fecha_validez = fecha_validez_result;

                #endregion

                resultado.Add(new SCDM_solicitud_rel_lista_tecnica
                {
                    num_fila = data.IndexOf(array),
                    id_solicitud = id_solicitud.Value,
                    id = id_lista_tecnica,
                    resultado = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Resultado")]) ? array[Array.IndexOf(encabezados, "Resultado")] : null,
                    componente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Componente")]) ? array[Array.IndexOf(encabezados, "Componente")] : null,
                    sobrante = sobrante_mm,
                    cantidad_cintas = cantidad_cintas,
                    cantidad_platinas = cantidad_platinas,
                    fecha_validez_reaplicacion = fecha_validez,
                });
            }



            return resultado;
        }
        private List<SCDM_solicitud_rel_facturacion> ConvierteArrayAFacturacion(List<string[]> data, int? id_solicitud)
        {
            List<SCDM_solicitud_rel_facturacion> resultado = new List<SCDM_solicitud_rel_facturacion> { };


            //listado de encabezados
            string[] encabezados = {
                "ID", "Número Material", "Planta","Unidad de Medida", "Clave Producto Servicio", "Cliente", "Descripcion", "Uso CFDI 1 G01<br>Adquisición Mercancia"
            , "Uso CFDI 2 G02<br>Devolucion/Descuento", "Uso de CFDI 3 G02<br>Gastos en General", "Uso de CFDI 4 I02<br>Mobiliario y Equipo", "Uso de CFDI 5 I03<br>Equipo de transporte"
            , "Uso de CFDI 6 I04<br>Equipo de computo", "Uso de CFDI 7 I05<br>Herramental general", "Uso de CFDI 8 I06<br>Comunicaciones", "Uso de CFDI 9 P01<br>Por definir",
            "Uso de CFDI 10 S01<br>Sin Efectos Fiscales"
            };

            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                int id_rel_facturacion = 0;
                bool? cfdi_01 = false, cfdi_02 = false, cfdi_03 = false, cfdi_04 = false, cfdi_05 = false, cfdi_06 = false, cfdi_07 = false, cfdi_08 = false, cfdi_09 = false, cfdi_10 = false;

                #region AsignacionVariables
                //id_creacion
                if (int.TryParse(array[Array.IndexOf(encabezados, "ID")], out int id_cr))
                    id_rel_facturacion = id_cr;
                //Cfdi
                if (array[Array.IndexOf(encabezados, "Uso CFDI 1 G01<br>Adquisición Mercancia")] == "True")
                    cfdi_01 = true;
                if (array[Array.IndexOf(encabezados, "Uso CFDI 2 G02<br>Devolucion/Descuento")] == "True")
                    cfdi_02 = true;
                if (array[Array.IndexOf(encabezados, "Uso de CFDI 3 G02<br>Gastos en General")] == "True")
                    cfdi_03 = true;
                if (array[Array.IndexOf(encabezados, "Uso de CFDI 4 I02<br>Mobiliario y Equipo")] == "True")
                    cfdi_04 = true;
                if (array[Array.IndexOf(encabezados, "Uso de CFDI 5 I03<br>Equipo de transporte")] == "True")
                    cfdi_05 = true;
                if (array[Array.IndexOf(encabezados, "Uso de CFDI 6 I04<br>Equipo de computo")] == "True")
                    cfdi_06 = true;
                if (array[Array.IndexOf(encabezados, "Uso de CFDI 7 I05<br>Herramental general")] == "True")
                    cfdi_07 = true;
                if (array[Array.IndexOf(encabezados, "Uso de CFDI 8 I06<br>Comunicaciones")] == "True")
                    cfdi_08 = true;
                if (array[Array.IndexOf(encabezados, "Uso de CFDI 9 P01<br>Por definir")] == "True")
                    cfdi_09 = true;
                if (array[Array.IndexOf(encabezados, "Uso de CFDI 10 S01<br>Sin Efectos Fiscales")] == "True")
                    cfdi_10 = true;


                #endregion

                resultado.Add(new SCDM_solicitud_rel_facturacion
                {
                    num_fila = data.IndexOf(array),
                    id = id_rel_facturacion,
                    uso_CFDI_01 = cfdi_01,
                    uso_CFDI_02 = cfdi_02,
                    uso_CFDI_03 = cfdi_03,
                    uso_CFDI_04 = cfdi_04,
                    uso_CFDI_05 = cfdi_05,
                    uso_CFDI_06 = cfdi_06,
                    uso_CFDI_07 = cfdi_07,
                    uso_CFDI_08 = cfdi_08,
                    uso_CFDI_09 = cfdi_09,
                    uso_CFDI_10 = cfdi_10,

                });
            }



            return resultado;
        }



        private List<SCDM_solicitud_rel_orden_compra> ConvierteArrayAOrdenesCompra(List<string[]> data, int? id_solicitud)
        {
            List<SCDM_solicitud_rel_orden_compra> resultado = new List<SCDM_solicitud_rel_orden_compra> { };


            //listado de encabezados
            string[] encabezados = {
            "ID", "Tipo de OC", "Núm. de PO<br>(Si es nueva línea)", "¿Procesador <br>externo?", "Proveedor <br>(Procesador Externo)", "Centro de<br> Recibo", "Días para la <br>entrega", "Cantidad <br>Estándar", "Cantidad <br>Mínima",
            "Cantidad <br>Máxima", "Número Proveedor", "Nombre Fiscal (Proveedor)", "¿Aplica IVA?", "Vigencia de Precio", "Incoterm", "Frontera/Puerto/Planta", "Condiciones de Pago", "Transporte 1", "Transporte 2",
            "Número de <br>material", "Núm. parte cliente", "Dimenciones<br>tolerancias", "Precio", "Moneda", "Unidad de <br>Medida", "Cantidad estimada de compra de material<br>por periodo de vigencia",
            "Descripción <br>(N/A para C&B)", "Peso Mínimo <br>(N/A para C&B)", "Peso Máx. bobinas de Acero KG<br>(N/A para C&B)", "Tipo Compra", "Contacto<br>(N/A para C&B)", "Teléfono<br>(N/A para C&B)",
            "Email<br>(N/A para C&B)", "Requerimientos específicos", "Molino <br>(N/A para procesadores Ext)", "País origen material <br>(N/A para procesadores Ext)", "Centro de <br>entrega","Almacen de <br>entrega"
            };

            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                //declaración de variables
                int? numero_dias = null;
                bool? aplica_procesador_externo = null, aplica_iva = null;
                int id_orden = 0;


                #region Asignacion de variables
                //id_creacion
                if (int.TryParse(array[Array.IndexOf(encabezados, "ID")], out int id_cr))
                    id_orden = id_cr;

                //valida procesador externo
                if (array[Array.IndexOf(encabezados, "¿Procesador <br>externo?")] == "SÍ")
                    aplica_procesador_externo = true;
                else if (array[Array.IndexOf(encabezados, "¿Procesador <br>externo?")] == "NO")
                    aplica_procesador_externo = false;

                //valida iva
                if (array[Array.IndexOf(encabezados, "¿Aplica IVA?")] == "SÍ")
                    aplica_iva = true;
                else if (array[Array.IndexOf(encabezados, "¿Aplica IVA?")] == "NO")
                    aplica_iva = false;

                //piezas por golpe
                if (int.TryParse(array[Array.IndexOf(encabezados, "Días para la <br>entrega")], out int numero_dias_result))
                    numero_dias = numero_dias_result;

                #endregion

                resultado.Add(new SCDM_solicitud_rel_orden_compra
                {
                    num_fila = data.IndexOf(array),
                    id_solicitud = id_solicitud.Value,
                    id = id_orden,
                    oc_existente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo de OC")]) ? array[Array.IndexOf(encabezados, "Tipo de OC")] : null,
                    numero_orden_compra_nueva_linea = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. de PO<br>(Si es nueva línea)")]) ? array[Array.IndexOf(encabezados, "Núm. de PO<br>(Si es nueva línea)")] : null,
                    aplica_procesador_externo = aplica_procesador_externo,
                    num_proveedor_procesador_externo = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Proveedor <br>(Procesador Externo)")]) ? array[Array.IndexOf(encabezados, "Proveedor <br>(Procesador Externo)")] : null,
                    centro_recibo = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Centro de<br> Recibo")]) ? array[Array.IndexOf(encabezados, "Centro de<br> Recibo")] : null,
                    numero_dias_para_entrega = numero_dias,
                    cantidad_estandar = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Cantidad <br>Estándar")]) ? array[Array.IndexOf(encabezados, "Cantidad <br>Estándar")] : null,
                    cantidad_minima = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Cantidad <br>Mínima")]) ? array[Array.IndexOf(encabezados, "Cantidad <br>Mínima")] : null,
                    cantidad_maxima = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Cantidad <br>Máxima")]) ? array[Array.IndexOf(encabezados, "Cantidad <br>Máxima")] : null,
                    num_proveedor_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Número Proveedor")]) ? array[Array.IndexOf(encabezados, "Número Proveedor")] : null,
                    nombre_fiscal_proveedor = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Nombre Fiscal (Proveedor)")]) ? array[Array.IndexOf(encabezados, "Nombre Fiscal (Proveedor)")] : null,
                    aplica_iva = aplica_iva,
                    vigencia_precio = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Vigencia de Precio")]) ? array[Array.IndexOf(encabezados, "Vigencia de Precio")] : null,
                    incoterm = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Incoterm")]) ? array[Array.IndexOf(encabezados, "Incoterm")] : null,
                    frontera_puerto = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Frontera/Puerto/Planta")]) ? array[Array.IndexOf(encabezados, "Frontera/Puerto/Planta")] : null,
                    condiciones_pago = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Condiciones de Pago")]) ? array[Array.IndexOf(encabezados, "Condiciones de Pago")] : null,
                    transporte_1 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Transporte 1")]) ? array[Array.IndexOf(encabezados, "Transporte 1")] : null,
                    transporte_2 = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Transporte 2")]) ? array[Array.IndexOf(encabezados, "Transporte 2")] : null,
                    num_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Número de <br>material")]) ? array[Array.IndexOf(encabezados, "Número de <br>material")] : null,
                    numero_parte_cliente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. parte cliente")]) ? array[Array.IndexOf(encabezados, "Núm. parte cliente")] : null,
                    dimensiones = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Dimenciones<br>tolerancias")]) ? array[Array.IndexOf(encabezados, "Dimenciones<br>tolerancias")] : null,
                    precio = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Precio")]) ? array[Array.IndexOf(encabezados, "Precio")] : null,
                    moneda = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Moneda")]) ? array[Array.IndexOf(encabezados, "Moneda")] : null,
                    unidad_medida = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Unidad de <br>Medida")]) ? array[Array.IndexOf(encabezados, "Unidad de <br>Medida")] : null,
                    cantidad_estimada_compra = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Cantidad estimada de compra de material<br>por periodo de vigencia")]) ? array[Array.IndexOf(encabezados, "Cantidad estimada de compra de material<br>por periodo de vigencia")] : null,
                    descripcion = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Descripción <br>(N/A para C&B)")]) ? array[Array.IndexOf(encabezados, "Descripción <br>(N/A para C&B)")] : null,
                    peso_minimo = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Peso Mínimo <br>(N/A para C&B)")]) ? array[Array.IndexOf(encabezados, "Peso Mínimo <br>(N/A para C&B)")] : null,
                    peso_maximo = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Peso Máx. bobinas de Acero KG<br>(N/A para C&B)")]) ? array[Array.IndexOf(encabezados, "Peso Máx. bobinas de Acero KG<br>(N/A para C&B)")] : null,
                    tipo_compra = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Tipo Compra")]) ? array[Array.IndexOf(encabezados, "Tipo Compra")] : null,
                    contacto = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Contacto<br>(N/A para C&B)")]) ? array[Array.IndexOf(encabezados, "Contacto<br>(N/A para C&B)")] : null,
                    telefono = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Teléfono<br>(N/A para C&B)")]) ? array[Array.IndexOf(encabezados, "Teléfono<br>(N/A para C&B)")] : null,
                    correo_electronico = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Email<br>(N/A para C&B)")]) ? array[Array.IndexOf(encabezados, "Email<br>(N/A para C&B)")] : null,
                    requerimientos_especificos = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Requerimientos específicos")]) ? array[Array.IndexOf(encabezados, "Requerimientos específicos")] : null,
                    molino = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Molino <br>(N/A para procesadores Ext)")]) ? array[Array.IndexOf(encabezados, "Molino <br>(N/A para procesadores Ext)")] : null,
                    pais_origen = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "País origen material <br>(N/A para procesadores Ext)")]) ? array[Array.IndexOf(encabezados, "País origen material <br>(N/A para procesadores Ext)")] : null,
                    centro_entrega = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Centro de <br>entrega")]) ? array[Array.IndexOf(encabezados, "Centro de <br>entrega")] : null,
                    almacen_entrega = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Almacen de <br>entrega")]) ? array[Array.IndexOf(encabezados, "Almacen de <br>entrega")] : null,

                });
            }



            return resultado;
        }

        public JsonResult CargaCreacionReferencia(int id_solicitud = 0)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var data = db.SCDM_solicitud_rel_creacion_referencia.Where(x => x.id_solicitud == id_solicitud).ToList();

            var jsonData = new object[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {
                //obtiene la descripción original
                string material = data[i].material_existente;
                mm_v3 mm = db.mm_v3.FirstOrDefault(x => x.Material == material);
                string descripcionOriginal = string.Empty;
                descripcionOriginal = mm != null ? mm.Material_Description : string.Empty;
                string descripcionOriginalES = string.Empty;
                descripcionOriginalES = mm != null ? mm.material_descripcion_es : string.Empty;



                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    cambioToHTMLButton(data[i].cambios, data[i].id),
                    !string.IsNullOrEmpty(data[i].nuevo_material)? data[i].nuevo_material:string.Empty,
                    !string.IsNullOrEmpty(data[i].material_existente)? data[i].material_existente:string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_material_text)? data[i].tipo_material_text:string.Empty,
                    //data[i].SCDM_cat_tipo_materiales_solicitud !=null? data[i].SCDM_cat_tipo_materiales_solicitud.descripcion.Trim():string.Empty,
                    data[i].plantas !=null? data[i].plantas.ConcatPlantaSap.Trim():string.Empty,
                    !string.IsNullOrEmpty(data[i].motivo_creacion)? data[i].motivo_creacion:string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_metal)? data[i].tipo_metal:string.Empty,
                    data[i].SCDM_cat_tipo_venta !=null? data[i].SCDM_cat_tipo_venta.descripcion.Trim():string.Empty,
                    !String.IsNullOrEmpty(data[i].numero_antiguo_material)? data[i].numero_antiguo_material : string.Empty,
                    data[i].peso_bruto.ToString(),
                    data[i].peso_neto.ToString(),
                    !string.IsNullOrEmpty(data[i].unidad_medida_inventario)?  data[i].unidad_medida_inventario : string.Empty,
                    descripcionOriginalES, //descripcion original es
                    descripcionOriginal, //descripcion original en
                    !string.IsNullOrEmpty(data[i].descripcion_es)?  data[i].descripcion_es : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_en)?  data[i].descripcion_en : string.Empty,
                    !string.IsNullOrEmpty(data[i].commodity)?  data[i].commodity : string.Empty,
                    !string.IsNullOrEmpty(data[i].grado_calidad) ? data[i].grado_calidad : string.Empty,
                    data[i].espesor_mm.ToString(),
                    data[i].espesor_tolerancia_negativa_mm.ToString(),
                    data[i].espesor_tolerancia_positiva_mm.ToString(),
                    data[i].ancho_mm.ToString(),
                    data[i].ancho_tolerancia_negativa_mm.ToString(),
                    data[i].ancho_tolerancia_positiva_mm.ToString(),
                    data[i].avance_mm.ToString(),
                    data[i].avance_tolerancia_negativa_mm.ToString(),
                    data[i].avance_tolerancia_positiva_mm.ToString(),
                    data[i].planicidad_mm.ToString(),
                    !string.IsNullOrEmpty(data[i].superficie) ? data[i].superficie : string.Empty,
                    !string.IsNullOrEmpty(data[i].tratamiento_superficial) ? data[i].tratamiento_superficial : string.Empty,
                    !string.IsNullOrEmpty(data[i].peso_recubrimiento) ? data[i].peso_recubrimiento : string.Empty,
                    !string.IsNullOrEmpty(data[i].molino)?  data[i].molino : string.Empty,
                    !string.IsNullOrEmpty(data[i].forma)?  data[i].forma : string.Empty,
                    !string.IsNullOrEmpty(data[i].cliente)?  data[i].cliente : string.Empty,
                    !string.IsNullOrEmpty(data[i].numero_parte) ? data[i].numero_parte : string.Empty,
                    !String.IsNullOrEmpty(data[i].msa_honda)?data[i].msa_honda:string.Empty,
                    data[i].diametro_exterior.ToString(),
                    data[i].diametro_interior.ToString(),
                    !string.IsNullOrEmpty(data[i].nuevo_dato)? data[i].nuevo_dato:string.Empty,
                    !string.IsNullOrEmpty(data[i].comentarios)? data[i].comentarios:string.Empty,
                    "true"
                    };

            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CargaCambiosIngenieria(int id_solicitud = 0)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var data = db.SCDM_solicitud_rel_cambio_ingenieria.Where(x => x.id_solicitud == id_solicitud).ToList();

            var jsonData = new object[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {
                //obtiene la descripción original
                string material = data[i].material_existente;
                mm_v3 mm = db.mm_v3.FirstOrDefault(x => x.Material == material);
                string descripcionOriginal = string.Empty;
                descripcionOriginal = mm != null ? mm.Material_Description : string.Empty;
                string descripcionOriginalES = string.Empty;
                descripcionOriginalES = mm != null ? mm.material_descripcion_es : string.Empty;

                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    cambioToHTMLButton(data[i].cambios, data[i].id),
                    //!string.IsNullOrEmpty(data[i].nuevo_material)? data[i].nuevo_material:string.Empty,
                    !string.IsNullOrEmpty(data[i].material_existente)? data[i].material_existente:string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_material_text)? data[i].tipo_material_text:string.Empty,
                    //data[i].SCDM_cat_tipo_materiales_solicitud !=null? data[i].SCDM_cat_tipo_materiales_solicitud.descripcion.Trim():string.Empty,
                    data[i].plantas !=null? data[i].plantas.ConcatPlantaSap.Trim():string.Empty,
                    //!string.IsNullOrEmpty(data[i].motivo_creacion)? data[i].motivo_creacion:string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_metal)? data[i].tipo_metal:string.Empty,
                    !string.IsNullOrEmpty(data[i].tipo_venta)? data[i].tipo_venta:string.Empty,
                    !String.IsNullOrEmpty(data[i].numero_antiguo_material)? data[i].numero_antiguo_material : string.Empty,
                    data[i].peso_bruto.ToString(),
                    data[i].peso_neto.ToString(),
                    !string.IsNullOrEmpty(data[i].unidad_medida_inventario)?  data[i].unidad_medida_inventario : string.Empty,
                    descripcionOriginalES, //descripcion original es
                    descripcionOriginal, //descripcion original en
                    !string.IsNullOrEmpty(data[i].descripcion_es)?  data[i].descripcion_es : string.Empty,
                    !string.IsNullOrEmpty(data[i].descripcion_en)?  data[i].descripcion_en : string.Empty,
                    !string.IsNullOrEmpty(data[i].commodity)?  data[i].commodity : string.Empty,
                    !string.IsNullOrEmpty(data[i].grado_calidad) ? data[i].grado_calidad : string.Empty,
                    data[i].espesor_mm.ToString(),
                    data[i].espesor_tolerancia_negativa_mm.ToString(),
                    data[i].espesor_tolerancia_positiva_mm.ToString(),
                    data[i].ancho_mm.ToString(),
                    data[i].ancho_tolerancia_negativa_mm.ToString(),
                    data[i].ancho_tolerancia_positiva_mm.ToString(),
                    data[i].avance_mm.ToString(),
                    data[i].avance_tolerancia_negativa_mm.ToString(),
                    data[i].avance_tolerancia_positiva_mm.ToString(),
                    data[i].planicidad_mm.ToString(),
                    !string.IsNullOrEmpty(data[i].superficie) ? data[i].superficie : string.Empty,
                    !string.IsNullOrEmpty(data[i].tratamiento_superficial) ? data[i].tratamiento_superficial : string.Empty,
                    !string.IsNullOrEmpty(data[i].peso_recubrimiento) ? data[i].peso_recubrimiento : string.Empty,
                    !string.IsNullOrEmpty(data[i].molino)?  data[i].molino : string.Empty,
                    !string.IsNullOrEmpty(data[i].forma)?  data[i].forma : string.Empty,
                    !string.IsNullOrEmpty(data[i].cliente)?  data[i].cliente : string.Empty,
                    !string.IsNullOrEmpty(data[i].numero_parte) ? data[i].numero_parte : string.Empty,
                    !String.IsNullOrEmpty(data[i].msa_honda)?data[i].msa_honda:string.Empty,
                    data[i].diametro_exterior.ToString(),
                    data[i].diametro_interior.ToString(),
                    !string.IsNullOrEmpty(data[i].nuevo_dato)? data[i].nuevo_dato:string.Empty,
                    !string.IsNullOrEmpty(data[i].comentarios)? data[i].comentarios:string.Empty,
                    "true"

            };

            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CargaCambiosBudget(int id_solicitud = 0)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var data = db.SCDM_solicitud_rel_cambio_budget.Where(x => x.id_solicitud == id_solicitud).ToList();

            var jsonData = new object[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {
                //obtiene la descripción original
                string material = data[i].material_existente;
                mm_v3 mm = db.mm_v3.FirstOrDefault(x => x.Material == material);

                jsonData[i] = new[] {
                    data[i].id.ToString(),                    
                    //!string.IsNullOrEmpty(data[i].nuevo_material)? data[i].nuevo_material:string.Empty,
                    !string.IsNullOrEmpty(data[i].material_existente)? data[i].material_existente:string.Empty,
                    mm!= null? mm.Type_of_Material.ToString() : string.Empty,
                    data[i].plantas !=null? data[i].plantas.ConcatPlantaSap.Trim():string.Empty,
                    data[i].peso_bruto_real_bascula.ToString(),
                    data[i].peso_neto_real_bascula.ToString(),
                    data[i].angulo_a.ToString(),
                    data[i].angulo_b.ToString(),
                    data[i].scrap_permitido_puntas_colas.ToString(),
                   !string.IsNullOrEmpty(data[i].pieza_doble)? data[i].pieza_doble:string.Empty,
                    data[i].reaplicacion.HasValue && data[i].reaplicacion.Value? "true":"false",
                    data[i].conciliacion_puntas_colas.HasValue && data[i].conciliacion_puntas_colas.Value? "true":"false",
                    data[i].conciliacion_scrap_ingenieria.HasValue && data[i].conciliacion_scrap_ingenieria.Value? "true":"false",
                   !string.IsNullOrEmpty(data[i].tipo_metal)? data[i].tipo_metal:string.Empty,
                   !string.IsNullOrEmpty(data[i].tipo_material)? data[i].tipo_material:string.Empty,
                   !string.IsNullOrEmpty(data[i].tipo_venta)? data[i].tipo_venta:string.Empty,
                   !string.IsNullOrEmpty(data[i].modelo_negocio)? data[i].modelo_negocio:string.Empty,
                   !string.IsNullOrEmpty(data[i].posicion_rollo)? data[i].posicion_rollo:string.Empty,
                   !string.IsNullOrEmpty(data[i].IHS_num_1)? data[i].IHS_num_1:string.Empty,
                   !string.IsNullOrEmpty(data[i].IHS_num_2)? data[i].IHS_num_2:string.Empty,
                   !string.IsNullOrEmpty(data[i].IHS_num_3)? data[i].IHS_num_3:string.Empty,
                   !string.IsNullOrEmpty(data[i].IHS_num_4)? data[i].IHS_num_4:string.Empty,
                   !string.IsNullOrEmpty(data[i].IHS_num_5)? data[i].IHS_num_5:string.Empty,
                   data[i].piezas_por_auto.ToString(),
                   data[i].piezas_por_golpe.ToString(),
                   data[i].piezas_por_paquete.ToString(),
                   data[i].peso_inicial.ToString(),
                   data[i].peso_maximo.ToString(),
                   data[i].peso_maximo_tolerancia_positiva.ToString(),
                   data[i].peso_maximo_tolerancia_negativa.ToString(),
                   data[i].peso_minimo.ToString(),
                   data[i].peso_minimo_tolerancia_positiva.ToString(),
                   data[i].peso_minimo_tolerancia_negativa.ToString(),
                    "true"
            };

            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        private string cambioToHTMLButton(string cambio, int id)
        {
            cambio = cambio ?? string.Empty;

            string result = string.Empty;

            cambio = cambio.Replace("ANT:", "<span style='color:red'>ANT:</span>"); //escapa los posibles '
            cambio = cambio.Replace("NUEVO:", "<span style='color:green'>Nuevo:</span>"); //escapa los posibles '
            cambio = cambio.Replace("\'", "\\'"); //escapa los posibles '

            string[] cambiosList = cambio.Split('|');

            result = "<button class='btn-cambios' onclick=\"muestraCambios(" + id + ", '";

            foreach (var item in cambiosList)
            {
                string temp = UsoStrings.ReplaceFirst(item, "[", "<b style=\\'color:#000099\\'>");
                temp = UsoStrings.ReplaceFirst(temp, "]", ":</b>");
                result += "<p style=\\'text-align:left\\'>" + temp + "</p>";
            }

            result += "')\"> Ver cambios </button>";


            return result;
        }
        public JsonResult CargaCambiosEstatus(int id_solicitud = 0)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var data = db.SCDM_solicitud_rel_activaciones.Where(x => x.id_solicitud == id_solicitud).ToList();

            var BD_SCDM_planta = db.plantas.Where(x => x.activo == true).ToList();

            var jsonData = new object[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {
                string concatPlanta = string.Empty;

                if (BD_SCDM_planta.Any(x => x.codigoSap == data[i].planta))
                    concatPlanta = BD_SCDM_planta.FirstOrDefault(x => x.codigoSap == data[i].planta).ConcatPlantaSap;

                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    !string.IsNullOrEmpty(data[i].material)? data[i].material:string.Empty,
                    concatPlanta,
                    !string.IsNullOrEmpty(data[i].sales_org)? data[i].sales_org:string.Empty,
                    !string.IsNullOrEmpty(data[i].estatus_planta)? data[i].estatus_planta:string.Empty,
                    !string.IsNullOrEmpty(data[i].estatus_dchain)? data[i].estatus_dchain:string.Empty,
                    data[i].fecha.ToString("dd/MM/yyyy"),
                    !string.IsNullOrEmpty(data[i].ejecucion_correcta)? data[i].ejecucion_correcta:string.Empty,
                    !string.IsNullOrEmpty(data[i].mensaje_sap)? data[i].mensaje_sap:string.Empty
                    //"true"
                    };
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //carga los datos para la vista de estatus
        public JsonResult CargaEstatus(string estatus, string fecha_inicio, string fecha_fin)
        {
            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();
            Debug.WriteLine($"inicia CargaEstatus()");

            string[] headers = {"Acciones","Folio", "Tipo Solicitud", "Planta", "Nombre Solicitante", "Prioridad", "Solicitante", "Solicitante_estatus", "Aprobación", "Aprobación_estatus",
                "Facturación", "Facturacion_estatus", "Compras", "Compras_estatus", "Controlling", "Controlling_estatus", "Ingeniería", "Ingeniería_estatus", "Calidad",
                "Calidad_estatus", "C. MRO", "Compras_MRO_estatus", "Ventas", "Ventas_estatus", "SCDM", "SCDM_estatus", "Estado"
            };

            //convierte fechas
            DateTime fechaActual = DateTime.Now;
            DateTime dateInicial = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day, 0, 0, 0).AddMonths(-1);  //fecha inicial por defecto
            DateTime dateFinal = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day, 23, 59, 59);          //fecha final por defecto

            try
            {
                if (!String.IsNullOrEmpty(fecha_inicio))
                    dateInicial = Convert.ToDateTime(fecha_inicio);
                if (!String.IsNullOrEmpty(fecha_fin))
                {
                    dateFinal = Convert.ToDateTime(fecha_fin);
                    dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine("Error de Formato: " + e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al convertir: " + ex.Message);
            }


            //obtiene el listado de item tipo rollo de la solicitud
            List<SCDM_solicitud> data = new List<SCDM_solicitud> { };

            //determina que tipo de solicitud debe mostrar
            switch (estatus)
            {
                case "ASIGNADA":
                    data = db.SCDM_solicitud.Where(x => x.activo && x.fecha_creacion >= dateInicial && x.fecha_creacion <=dateFinal && x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null)).OrderByDescending(x => x.id).ToList();
                    break;
                case "Enum.GetName(typeof(SCMD_solicitud_estatus_enum)":
                    data = db.SCDM_solicitud.Where(x => x.activo && x.fecha_creacion >= dateInicial && x.fecha_creacion <= dateFinal && x.SCDM_solicitud_asignaciones.Any() && !x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null)).OrderByDescending(x => x.id).ToList();
                    break;
                default:
                    data = db.SCDM_solicitud.Where(x => x.activo && x.fecha_creacion >= dateInicial && x.fecha_creacion <= dateFinal && x.SCDM_solicitud_asignaciones.Any()).OrderByDescending(x => x.id).ToList();
                    break;
            }

            //dias festivos en BD
            List<DateTime> listDiasFestivos = db.SCDM_cat_dias_feriados.Select(x => x.fecha).ToList();

            var jsonData = new object[data.Count()];
            var jsonDataComments = new List<object>();


            int i = 0;
            foreach (var item in data)
            {
                //obtiene el detalle de los tiempos de la asignacion
                DetalleAsignacion detalleSolicitante = item.GetTiempoUltimaAsignacion(99, listDiasFestivos);        // 99 -> Solicitante
                DetalleAsignacion detalleAprobacionInicial = item.GetTiempoUltimaAsignacion(88, listDiasFestivos);  // 88 -> Inicial
                DetalleAsignacion detalleFacturacion = item.GetTiempoUltimaAsignacion((int)SCDM_departamentos_AsignacionENUM.FACTURACION, listDiasFestivos);  // 88 -> Inicial
                DetalleAsignacion detalleCompras = item.GetTiempoUltimaAsignacion((int)SCDM_departamentos_AsignacionENUM.COMPRAS, listDiasFestivos);  // 88 -> Inicial
                DetalleAsignacion detalleControlling = item.GetTiempoUltimaAsignacion((int)SCDM_departamentos_AsignacionENUM.CONTROLLING, listDiasFestivos);  // 88 -> Inicial
                DetalleAsignacion detalleIngenieria = item.GetTiempoUltimaAsignacion((int)SCDM_departamentos_AsignacionENUM.INGENIERIA, listDiasFestivos);  // 88 -> Inicial
                DetalleAsignacion detalleCalidad = item.GetTiempoUltimaAsignacion((int)SCDM_departamentos_AsignacionENUM.CALIDAD, listDiasFestivos);  // 88 -> Inicial
                DetalleAsignacion detalleComprasMRO = item.GetTiempoUltimaAsignacion((int)SCDM_departamentos_AsignacionENUM.COMPRAS_MRO, listDiasFestivos);  // 88 -> Inicial
                DetalleAsignacion detalleVentas = item.GetTiempoUltimaAsignacion((int)SCDM_departamentos_AsignacionENUM.VENTAS, listDiasFestivos);  // 88 -> Inicial
                DetalleAsignacion detalleSCDM = item.GetTiempoUltimaAsignacion((int)SCDM_departamentos_AsignacionENUM.SCDM, listDiasFestivos);  // 88 -> Inicial

                string botones = string.Empty;

                //si tiene el rol de scdm admin
                if (TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                {
                    botones += @"<a href=""AsignarTareas/" + item.id.ToString() + @" "" style=""color:#555555;"" class=""btn btn-warning btn-sm"" data-toggle=""tooltip"" data-placement=""top"" title=""Administrar""><i class=""fa-solid fa-gear""></i></a>";
                    botones += @"<a href=""EditarSolicitud/" + item.id.ToString() + @" "" style=""color:white;"" class=""btn btn-primary btn-sm"" data-toggle=""tooltip"" data-placement=""top"" title=""Editar""><i class=""fa-regular fa-pen-to-square""></i></a>";
                }
                //agrega el botón de detalles
                botones += @"<a href=""Details/" + item.id.ToString() + @" "" style=""color:white;"" class=""btn btn-info btn-sm"" data-toggle=""tooltip"" data-placement=""top"" title=""Detalles""><i class=""fa-solid fa-circle-info""></i></a>";

                jsonData[i] = new[] {
                    botones,
                    item.id.ToString(),
                    item.SCDM_cat_tipo_solicitud.descripcion + (item.id_tipo_cambio.HasValue ? " ["+item.SCDM_cat_tipo_cambio.descripcion+"]": string.Empty),
                    item.plantas1.ConcatPlantaSap,
                    item.empleados.ConcatNombre,
                    item.SCDM_cat_prioridad.descripcion,                    
                    //solicitante
                    detalleSolicitante.tiempoString,
                    detalleSolicitante.estatus.ToString(),
                    //inicial
                    detalleAprobacionInicial.tiempoString,
                    detalleAprobacionInicial.estatus.ToString(),
                    //facturacion
                    detalleFacturacion.tiempoString,
                    detalleFacturacion.estatus.ToString(),
                    //compras
                    detalleCompras.tiempoString,
                    detalleCompras.estatus.ToString(),
                    //controlling
                    detalleControlling.tiempoString,
                    detalleControlling.estatus.ToString(),
                    //ingenieria
                    detalleIngenieria.tiempoString,
                    detalleIngenieria.estatus.ToString(),
                    //Calidad
                    detalleCalidad.tiempoString,
                    detalleCalidad.estatus.ToString(),
                    //Compras MRO
                    detalleComprasMRO.tiempoString,
                    detalleComprasMRO.estatus.ToString(),
                    //Ventas
                    detalleVentas.tiempoString,
                    detalleVentas.estatus.ToString(),
                    //SCDM
                    detalleSCDM.tiempoString,
                    detalleSCDM.estatus.ToString(),                    
                    //estado
                    item.estatusTexto,
                    //botones de acción
                    //@"<button class=""btn btn-warning btn-sm"" onclick=""muestraAdmin("+item.id+@")"" ><i class=""fa-solid fa-gear""></i></button><button class=""btn btn-primary btn-sm"" onclick=""muestraEdit("+item.id+@")"" ><i class=""fa-regular fa-pen-to-square""></i></button><button class=""btn btn-info btn-sm"" onclick=""muestraInfo("+item.id+@")"" ><i class=""fa-solid fa-circle-info""></i></button>    
                    //"

                    };

                #region comentarios 
                //agrega el comentario solicitante
                if (detalleSolicitante.tiempoTimeSpan.HasValue)
                    jsonDataComments.Add(new
                    {
                        row = i,
                        col = Array.IndexOf(headers, "Solicitante"),
                        comment = new { value = $"Asignada: {detalleSolicitante.fecha_asignacion.ToString()}\nCerrada: " +
                        $"{(detalleSolicitante.fecha_cierre.HasValue? detalleSolicitante.fecha_cierre.ToString():"--")}\nCerrada Por: " +
                        $"{detalleSolicitante.cerrado_por}", readOnly = true }
                    });

                //agrega el comentario aprobación inicial
                if (detalleAprobacionInicial.tiempoTimeSpan.HasValue)
                    jsonDataComments.Add(new
                    {
                        row = i,
                        col = Array.IndexOf(headers, "Aprobación"),
                        comment = new { value = $"Asignada: {detalleAprobacionInicial.fecha_asignacion.ToString()}\nCerrada: " +
                        $"{(detalleAprobacionInicial.fecha_cierre.HasValue? detalleAprobacionInicial.fecha_cierre.ToString():"--")}\nCerrada Por: " +
                        $"{detalleAprobacionInicial.cerrado_por}", readOnly = true }
                    });

                 //agrega el comentario aprobación facturación
                if (detalleFacturacion.tiempoTimeSpan.HasValue)
                    jsonDataComments.Add(new
                    {
                        row = i,
                        col = Array.IndexOf(headers, "Facturación"),
                        comment = new { value = $"Asignada: {detalleFacturacion.fecha_asignacion.ToString()}\nCerrada: " +
                        $"{(detalleFacturacion.fecha_cierre.HasValue? detalleFacturacion.fecha_cierre.ToString():"--")}\nCerrada Por: " +
                        $"{detalleFacturacion.cerrado_por}", readOnly = true }
                    });

                //agrega el comentario aprobación detalleCompras
                if (detalleCompras.tiempoTimeSpan.HasValue)
                    jsonDataComments.Add(new
                    {
                        row = i,
                        col = Array.IndexOf(headers, "Compras"),
                        comment = new { value = $"Asignada: {detalleCompras.fecha_asignacion.ToString()}\nCerrada: " +
                        $"{(detalleCompras.fecha_cierre.HasValue? detalleCompras.fecha_cierre.ToString():"--")}\nCerrada Por: " +
                        $"{detalleCompras.cerrado_por}", readOnly = true }
                    });

                //agrega el comentario aprobación Controlling
                if (detalleControlling.tiempoTimeSpan.HasValue)
                    jsonDataComments.Add(new
                    {
                        row = i,
                        col = Array.IndexOf(headers, "Controlling"),
                        comment = new { value = $"Asignada: {detalleControlling.fecha_asignacion.ToString()}\nCerrada: " +
                        $"{(detalleControlling.fecha_cierre.HasValue? detalleControlling.fecha_cierre.ToString():"--")}\nCerrada Por: " +
                        $"{detalleControlling.cerrado_por}", readOnly = true }
                    });

                //agrega el comentario aprobación Ingeniería
                if (detalleIngenieria.tiempoTimeSpan.HasValue)
                    jsonDataComments.Add(new
                    {
                        row = i,
                        col = Array.IndexOf(headers, "Ingeniería"),
                        comment = new { value = $"Asignada: {detalleIngenieria.fecha_asignacion.ToString()}\nCerrada: " +
                        $"{(detalleIngenieria.fecha_cierre.HasValue? detalleIngenieria.fecha_cierre.ToString():"--")}\nCerrada Por: " +
                        $"{detalleIngenieria.cerrado_por}", readOnly = true }
                    });

                //agrega el comentario aprobación Calidad
                if (detalleCalidad.tiempoTimeSpan.HasValue)
                    jsonDataComments.Add(new
                    {
                        row = i,
                        col = Array.IndexOf(headers, "Calidad"),
                        comment = new { value = $"Asignada: {detalleCalidad.fecha_asignacion.ToString()}\nCerrada: " +
                        $"{(detalleCalidad.fecha_cierre.HasValue? detalleCalidad.fecha_cierre.ToString():"--")}\nCerrada Por: " +
                        $"{detalleCalidad.cerrado_por}", readOnly = true }
                    });

                //agrega el comentario aprobación C. MRO
                if (detalleComprasMRO.tiempoTimeSpan.HasValue)
                    jsonDataComments.Add(new
                    {
                        row = i,
                        col = Array.IndexOf(headers, "C. MRO"),
                        comment = new { value = $"Asignada: {detalleComprasMRO.fecha_asignacion.ToString()}\nCerrada: " +
                        $"{(detalleComprasMRO.fecha_cierre.HasValue? detalleComprasMRO.fecha_cierre.ToString():"--")}\nCerrada Por: " +
                        $"{detalleComprasMRO.cerrado_por}", readOnly = true }
                    });

                //agrega el comentario aprobación Ventas
                if (detalleVentas.tiempoTimeSpan.HasValue)
                    jsonDataComments.Add(new
                    {
                        row = i,
                        col = Array.IndexOf(headers, "Ventas"),
                        comment = new { value = $"Asignada: {detalleVentas.fecha_asignacion.ToString()}\nCerrada: " +
                        $"{(detalleVentas.fecha_cierre.HasValue? detalleVentas.fecha_cierre.ToString():"--")}\nCerrada Por: " +
                        $"{detalleVentas.cerrado_por}", readOnly = true }
                    });
                //agrega el comentario aprobación SCDM
                if (detalleSCDM.tiempoTimeSpan.HasValue)
                    jsonDataComments.Add(new
                    {
                        row = i,
                        col = Array.IndexOf(headers, "SCDM"),
                        comment = new { value = $"Asignada: {detalleSCDM.fecha_asignacion.ToString()}\nCerrada: " +
                        $"{(detalleSCDM.fecha_cierre.HasValue? detalleSCDM.fecha_cierre.ToString():"--")}\nCerrada Por: " +
                        $"{detalleSCDM.cerrado_por}", readOnly = true }
                    });



                #endregion

                i++;
            }

            timeMeasure.Stop();
            Debug.WriteLine($"Tiempo: {timeMeasure.Elapsed.TotalMilliseconds / 1000} s");


            //divide el json en dos
            var jsonDataFinal = new object[2];
            jsonDataFinal[0] = jsonData;

            jsonDataFinal[1] = jsonDataComments.ToArray();

            //Se crear una referencia a JavaScriptSerializer
            var serializer = new JavaScriptSerializer();
            //Se cambia el Length directo a nuestra referencia
            serializer.MaxJsonLength = 500000000;

            var json = Json(jsonDataFinal, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;

            return json;
        }
        public JsonResult CargaMaterialesExtension(int id_solicitud = 0)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var data = db.SCDM_solicitud_rel_extension_usuario.Where(x => x.id_solicitud == id_solicitud).ToList();

            var BD_SCDM_planta = db.plantas.Where(x => x.activo == true).ToList();

            var jsonData = new object[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {
                string concatPlantaReferencia = string.Empty;
                string concatPlantaDestino = string.Empty;

                if (BD_SCDM_planta.Any(x => x.codigoSap == data[i].planta_referencia))
                    concatPlantaReferencia = BD_SCDM_planta.FirstOrDefault(x => x.codigoSap == data[i].planta_referencia).ConcatPlantaSap;
                if (BD_SCDM_planta.Any(x => x.codigoSap == data[i].planta_destino))
                    concatPlantaDestino = BD_SCDM_planta.FirstOrDefault(x => x.codigoSap == data[i].planta_destino).ConcatPlantaSap;

                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    !string.IsNullOrEmpty(data[i].material)? data[i].material:string.Empty,
                    concatPlantaReferencia,
                    concatPlantaDestino,
                    "true"

                };
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GuardaAlmacenesVirtuales(string[] almacenes, int id_solicitud = 0)
        {

            //obtiene la solicitud
            var solicitud = db.SCDM_solicitud.Find(id_solicitud);

            //obtiene el listado de item tipo rollo de la solicitud
            if (almacenes != null)
            {
                foreach (var item in almacenes)
                {
                    //agrega el almacen en caso de no existir
                    if (!solicitud.SCDM_rel_solicitud_extension_almacenes_virtuales.Any(x => x.almacen_virtual == item))
                    {
                        db.SCDM_rel_solicitud_extension_almacenes_virtuales.Add(
                            new SCDM_rel_solicitud_extension_almacenes_virtuales
                            {
                                id_solicitud = id_solicitud,
                                almacen_virtual = item,
                            });
                    }
                    //si existe no hace nada
                }
            }

            //Borra los almacenes que no existan
            db.SCDM_rel_solicitud_extension_almacenes_virtuales.RemoveRange(solicitud.SCDM_rel_solicitud_extension_almacenes_virtuales.Where(x => almacenes == null || !almacenes.Contains(x.almacen_virtual)));

            var jsonData = new object[1];
            try
            {
                db.SaveChanges();
                jsonData[0] = new { resultado = "Correcto.", };
            }
            catch (Exception e)
            {
                jsonData[0] = new { resultado = "Error.", mensaje = e.Message };
            }

            System.Diagnostics.Debug.WriteLine("Guardando almacenes virtuales: " + almacenes);

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Carga los datos iniciales de la lista tecnica (Rels)
        /// </summary>
        /// <param name="id_solicitud"></param>
        /// <returns></returns>
        public JsonResult CargaListaTecnica(int id_solicitud = 0)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var data = db.SCDM_solicitud_rel_lista_tecnica.Where(x => x.id_solicitud == id_solicitud).ToList();

            var jsonData = new object[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {

                var tempResultado = data[i].resultado;
                var tempComponente = data[i].componente;
                //inicia variables
                string tipo_material_resultado = "--";
                string tipo_material_componente = "--";
                string peso_bruto_platina = "--";
                string peso_neto_platina = "--";
                string unidad_medida = "--";
                string SCDM_cat_tipo_venta = "--";

                //obtiene todos los posibles valores
                SCDM_solicitud_rel_item_material itemMaterialResultado = db.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.id_solicitud == id_solicitud && x.numero_material == tempResultado);
                SCDM_solicitud_rel_creacion_referencia itemCreacionReferenciaResultado = db.SCDM_solicitud_rel_creacion_referencia.FirstOrDefault(x => x.id_solicitud == id_solicitud && x.nuevo_material == tempResultado);
                SCDM_solicitud_rel_item_material itemMaterialComponente = db.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.id_solicitud == id_solicitud && x.numero_material == tempComponente);
                SCDM_solicitud_rel_creacion_referencia itemCreacionReferenciaComponente = db.SCDM_solicitud_rel_creacion_referencia.FirstOrDefault(x => x.id_solicitud == id_solicitud && x.nuevo_material == tempComponente);

                //si existe en la material de solicitud
                if (itemMaterialResultado != null)
                {
                    tipo_material_resultado = itemMaterialResultado.SCDM_cat_tipo_materiales_solicitud != null ? itemMaterialResultado.SCDM_cat_tipo_materiales_solicitud.descripcion : tempResultado.StartsWith("SM") ? "Maquila" : "--";
                    peso_bruto_platina = itemMaterialResultado.peso_bruto.HasValue ? itemMaterialResultado.peso_bruto.ToString() : "--";
                    peso_neto_platina = itemMaterialResultado.peso_neto.HasValue ? itemMaterialResultado.peso_neto.ToString() : "--";
                    unidad_medida = !string.IsNullOrEmpty(itemMaterialResultado.unidad_medida_inventario) ? itemMaterialResultado.unidad_medida_inventario : "--";
                    SCDM_cat_tipo_venta = !string.IsNullOrEmpty(itemMaterialResultado.tipo_venta) ? itemMaterialResultado.tipo_venta : "--";
                }
                //si existe en la creacion con referencia
                if (itemCreacionReferenciaResultado != null)
                {
                    tipo_material_resultado = tempResultado.StartsWith("SM") ? "Maquila" : itemCreacionReferenciaResultado.tipo_material_text;
                    peso_bruto_platina = itemCreacionReferenciaResultado.peso_bruto.HasValue ? itemCreacionReferenciaResultado.peso_bruto.ToString() : "--";
                    peso_neto_platina = itemCreacionReferenciaResultado.peso_neto.HasValue ? itemCreacionReferenciaResultado.peso_neto.ToString() : "--";
                    unidad_medida = !string.IsNullOrEmpty(itemCreacionReferenciaResultado.unidad_medida_inventario) ? itemCreacionReferenciaResultado.unidad_medida_inventario : "--";
                    SCDM_cat_tipo_venta = itemCreacionReferenciaResultado.SCDM_cat_tipo_venta != null ? itemCreacionReferenciaResultado.SCDM_cat_tipo_venta.descripcion : "--";
                }

                //si no se encuentra en ninguna de las anteriores, busca en el catálogo de MM
                if (itemMaterialResultado == null && itemCreacionReferenciaResultado == null)
                {
                    mm_v3 mm = db.mm_v3.FirstOrDefault(x => x.Material == tempResultado);

                    if (mm != null)
                    {
                        tipo_material_resultado = tempResultado.StartsWith("SM") ? "Maquila" : mm.Type_of_Material;
                        peso_bruto_platina = mm.Gross_weight.HasValue ? mm.Gross_weight.ToString() : "--";
                        peso_neto_platina = mm.Net_weight.HasValue ? mm.Net_weight.ToString() : "--";
                        unidad_medida = !string.IsNullOrEmpty(mm.unidad_medida) ? mm.unidad_medida : "--";
                        SCDM_cat_tipo_venta = mm.Type_of_Selling != null ? mm.Type_of_Selling : "--";
                    }
                }

                //determina el tipo de material del componente
                if (itemMaterialComponente != null)
                    tipo_material_componente = itemMaterialComponente.SCDM_cat_tipo_materiales_solicitud != null ? itemMaterialComponente.SCDM_cat_tipo_materiales_solicitud.descripcion : tempComponente.StartsWith("SM") ? "Maquila" : "--";

                if (itemCreacionReferenciaComponente != null)
                    tipo_material_componente = tempComponente.StartsWith("SM") ? "Maquila" : itemCreacionReferenciaComponente.tipo_material_text;

                if (itemMaterialComponente == null && itemCreacionReferenciaComponente == null)
                {
                    mm_v3 mm = db.mm_v3.FirstOrDefault(x => x.Material == tempComponente);

                    if (mm != null)
                        tipo_material_componente = tempComponente.StartsWith("SM") ? "Maquila" : mm.Type_of_Material;
                }

                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    !string.IsNullOrEmpty(data[i].resultado)? data[i].resultado:string.Empty,
                    tipo_material_resultado,
                    SCDM_cat_tipo_venta,
                    peso_bruto_platina,
                    peso_neto_platina,
                    unidad_medida,
                    data[i].sobrante.HasValue ? data[i].sobrante.Value.ToString() : string.Empty,
                    !string.IsNullOrEmpty(data[i].componente)? data[i].componente:string.Empty,
                    tipo_material_componente,
                    data[i].cantidad_platinas.HasValue ? data[i].cantidad_platinas.Value.ToString() : string.Empty,
                    data[i].cantidad_cintas.HasValue ? data[i].cantidad_cintas.Value.ToString() : string.Empty,
                     data[i].fecha_validez_reaplicacion.HasValue?data[i].fecha_validez_reaplicacion.Value.ToString("dd/MM/yyyy"):string.Empty,

                    };
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Carga los materiales y su uso de CFDI
        /// </summary>
        /// <param name="id_solicitud"></param>
        /// <returns></returns>
        public JsonResult CargaFacturacion(int id_solicitud = 0)
        {

            //obtiene el listado de facturacion que ya se agregaron en el excel
            var dataCreacionMateriales = db.SCDM_solicitud_rel_item_material.Where(x => x.id_solicitud == id_solicitud && x.SCDM_solicitud_rel_facturacion.Any()).ToList();
            var dataCreacionReferencia = db.SCDM_solicitud_rel_creacion_referencia.Where(x => x.id_solicitud == id_solicitud && x.SCDM_solicitud_rel_facturacion.Any()).ToList();

            var jsonData = new object[dataCreacionMateriales.Count() + dataCreacionReferencia.Count()];

            var dataList = db.SCDM_solicitud_rel_facturacion.Where(x =>
                                                (x.SCDM_solicitud_rel_item_material != null && x.SCDM_solicitud_rel_item_material.id_solicitud == id_solicitud) ||
                                            (x.SCDM_solicitud_rel_creacion_referencia != null && x.SCDM_solicitud_rel_creacion_referencia.id_solicitud == id_solicitud)).ToList();


            int i = 0;

            foreach (var item in dataCreacionMateriales)
            {
                //obtiene el objeto de item de materiales
                var rel = dataList.FirstOrDefault(x => x.id_solicitud_rel_item_material == item.id);

                jsonData[i] = new[] {
                    rel != null ? rel.id.ToString() : "0",
                    item.numero_material,
                    item.SCDM_solicitud.SCDM_rel_solicitud_plantas.FirstOrDefault().plantas.codigoSap,
                    rel != null ? rel.unidad_medida : string.Empty,
                    rel != null ? rel.clave_producto_servicio : string.Empty,
                    rel != null ? rel.cliente : string.Empty,
                    rel != null ? rel.descripcion_en : string.Empty,
                    rel != null && rel.uso_CFDI_01.HasValue && rel.uso_CFDI_01.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_02.HasValue && rel.uso_CFDI_02.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_03.HasValue && rel.uso_CFDI_03.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_04.HasValue && rel.uso_CFDI_04.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_05.HasValue && rel.uso_CFDI_05.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_06.HasValue && rel.uso_CFDI_06.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_07.HasValue && rel.uso_CFDI_07.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_08.HasValue && rel.uso_CFDI_08.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_09.HasValue && rel.uso_CFDI_09.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_10.HasValue && rel.uso_CFDI_10.Value ? "True"  : "False",
                    };
                i++;
            }

            foreach (var item in dataCreacionReferencia)
            {
                //obtiene el objeto de creacio con referencia
                var rel = dataList.FirstOrDefault(x => x.id_solicitud_rel_item_creacion_referencia == item.id);

                jsonData[i] = new[] {
                    rel != null ? rel.id.ToString() : "0",
                    item.nuevo_material,
                    item.plantas.codigoSap,
                    rel != null ? rel.unidad_medida : string.Empty,
                    rel != null ? rel.clave_producto_servicio : string.Empty,
                    rel != null ? rel.cliente : string.Empty,
                    rel != null ? rel.descripcion_en : string.Empty,
                    rel != null && rel.uso_CFDI_01.HasValue && rel.uso_CFDI_01.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_02.HasValue && rel.uso_CFDI_02.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_03.HasValue && rel.uso_CFDI_03.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_04.HasValue && rel.uso_CFDI_04.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_05.HasValue && rel.uso_CFDI_05.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_06.HasValue && rel.uso_CFDI_06.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_07.HasValue && rel.uso_CFDI_07.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_08.HasValue && rel.uso_CFDI_08.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_09.HasValue && rel.uso_CFDI_09.Value ? "True"  : "False",
                    rel != null && rel.uso_CFDI_10.HasValue && rel.uso_CFDI_10.Value ? "True"  : "False",

                    };
                i++;
            }


            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Carga las OC de la solicitud indicada
        /// </summary>
        /// <param name="id_solicitud"></param>
        /// <returns></returns>
        public JsonResult CargaOrdenesCompra(int id_solicitud = 0)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var data = db.SCDM_solicitud_rel_orden_compra.Where(x => x.id_solicitud == id_solicitud).ToList();

            var jsonData = new object[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {


                //obtiene todos los posibles valores

                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    data[i].oc_existente,
                    data[i].numero_orden_compra_nueva_linea,
                    data[i].aplica_procesador_externo.HasValue? data[i].aplica_procesador_externo.Value?"SÍ":"NO":string.Empty,
                    data[i].num_proveedor_procesador_externo,
                    data[i].centro_recibo,
                    data[i].numero_dias_para_entrega.ToString(),
                    data[i].cantidad_estandar,
                    data[i].cantidad_minima,
                    data[i].cantidad_maxima,
                    data[i].num_proveedor_material,
                    data[i].nombre_fiscal_proveedor,
                    data[i].aplica_iva.HasValue? data[i].aplica_iva.Value?"SÍ":"NO":string.Empty,
                    data[i].vigencia_precio,
                    data[i].incoterm,
                    data[i].frontera_puerto,
                    data[i].condiciones_pago,
                    data[i].transporte_1,
                    data[i].transporte_2,
                    data[i].num_material,
                    data[i].numero_parte_cliente,
                    data[i].dimensiones,
                    data[i].precio,
                    data[i].moneda,
                    data[i].unidad_medida,
                    data[i].cantidad_estimada_compra,
                    data[i].descripcion,
                    data[i].peso_minimo,
                    data[i].peso_maximo,
                    data[i].tipo_compra,
                    data[i].contacto,
                    data[i].telefono,
                    data[i].correo_electronico,
                    data[i].requerimientos_especificos,
                    data[i].molino,
                    data[i].pais_origen,
                    data[i].centro_entrega,
                    data[i].almacen_entrega
                    };
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Carga las OC de la solicitud, segun las hojas de rollos, platinas, cintas, etc-
        /// </summary>
        /// <param name="id_solicitud"></param>
        /// <returns></returns>
        public JsonResult CargaOrdenesCompraSegunSolicitud(int id_solicitud = 0)
        {

            string acopioExternoText = db.SCDM_cat_clase_aprovisionamiento.Find(1).descripcion;

            //obtiene el listado de items de la solicitud donde es acopio externo
            var dataMateriales = db.SCDM_solicitud_rel_item_material.Where(x => x.id_solicitud == id_solicitud
                && x.clase_aprovisionamiento == acopioExternoText //acopio externo
            ).ToList();
            //obtiene los valores de las referencias
            var referenciaData = db.SCDM_solicitud_rel_creacion_referencia.Where(x => x.id_solicitud == id_solicitud).ToList();

            var jsonData = new object[dataMateriales.Count() + referenciaData.Count()];

            for (int i = 0; i < dataMateriales.Count(); i++)
            {

                string dimensiones = string.Empty;

                switch (dataMateriales[i].id_tipo_material)
                {
                    case (int)SCDM_solicitud_rel_item_material_tipo.ROLLO:
                        dimensiones = string.Format("{0}(+{1}/{2}) X {3}(+{4}/{5}) mm {6} {7} {8}",
                            dataMateriales[i].espesor_mm,
                            dataMateriales[i].espesor_tolerancia_positiva_mm,
                            dataMateriales[i].espesor_tolerancia_negativa_mm,
                            dataMateriales[i].ancho_mm,
                            dataMateriales[i].ancho_tolerancia_positiva_mm,
                            dataMateriales[i].ancho_tolerancia_negativa_mm,
                            dataMateriales[i].grado_calidad,
                            dataMateriales[i].tipo_recubrimiento != null && !string.IsNullOrEmpty(dataMateriales[i].tipo_recubrimiento) ? dataMateriales[i].tipo_recubrimiento.Substring(2, 2) : "--",
                            dataMateriales[i].peso_recubrimiento != null && !string.IsNullOrEmpty(dataMateriales[i].peso_recubrimiento) ? dataMateriales[i].peso_recubrimiento : "--"
                            );
                        break;
                    case (int)SCDM_solicitud_rel_item_material_tipo.CINTA:
                        dimensiones = string.Format("{0}X{1}",
                            dataMateriales[i].espesor_mm,
                            dataMateriales[i].ancho_entrega_cinta_mm
                            );
                        break;
                    case (int)SCDM_solicitud_rel_item_material_tipo.PLATINA:
                    case (int)SCDM_solicitud_rel_item_material_tipo.PLATINA_SOLDADA:
                    case (int)SCDM_solicitud_rel_item_material_tipo.SHEARING:
                    case (int)SCDM_solicitud_rel_item_material_tipo.C_B:
                        dimensiones = string.Format("{0}X{1}X{2}",
                            dataMateriales[i].espesor_mm,
                            dataMateriales[i].ancho_mm,
                            dataMateriales[i].avance_mm
                            );
                        break;
                }

                //obtiene todos los posibles valores
                jsonData[i] = new[] {
                    "0", //nuevo elemento
                    string.Empty,
                    string.Empty,
                    dataMateriales[i].aplica_procesador_externo != null && dataMateriales[i].aplica_procesador_externo.Value ? "SÍ" : "NO",
                    dataMateriales[i].procesador_externo_nombre != null && !string.IsNullOrEmpty(dataMateriales[i].procesador_externo_nombre) ? dataMateriales[i].procesador_externo_nombre : string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    dataMateriales[i].proveedor != null && !string.IsNullOrEmpty(dataMateriales[i].proveedor) ? dataMateriales[i].proveedor : string.Empty, //NUM
                    dataMateriales[i].proveedor != null && !string.IsNullOrEmpty(dataMateriales[i].proveedor) ? dataMateriales[i].proveedor : string.Empty, //nombre fiscal                                       
                    dataMateriales[i].aplica_tasa_iva.HasValue? dataMateriales[i].aplica_tasa_iva.Value?"SÍ":"NO":string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    dataMateriales[i].numero_material,
                    !string.IsNullOrEmpty(dataMateriales[i].numero_parte)?dataMateriales[i].numero_parte: string.Empty,
                    dimensiones, //DIMENSIONES
                    string.Empty,
                    string.Empty,
                    dataMateriales[i].unidad_medida_inventario != null && !string.IsNullOrEmpty(dataMateriales[i].unidad_medida_inventario) ? dataMateriales[i].unidad_medida_inventario : string.Empty, //nombre fiscal  
                    string.Empty,
                    dataMateriales[i].descripcion_material_es,
                    dataMateriales[i].peso_min_kg.ToString(),
                    dataMateriales[i].peso_max_kg.ToString(),
                    dataMateriales[i].tipo_venta != null && !string.IsNullOrEmpty(dataMateriales[i].tipo_venta) ? dataMateriales[i].tipo_venta : string.Empty, //nombre fiscal 
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    dataMateriales[i].molino != null && !string.IsNullOrEmpty(dataMateriales[i].molino) ? dataMateriales[i].molino : string.Empty, //nombre fiscal 
                    string.Empty,
                    };
            }

            //debe crear un renglon con los datos de la creación de Materiales          
            for (int i = dataMateriales.Count; i < dataMateriales.Count + referenciaData.Count(); i++)
            {
                int c = i - dataMateriales.Count;

                jsonData[i] = new[] {
                         "0", //nuevo elemento
                    string.Empty,
                    string.Empty,
                    string.Empty, //dataMateriales[i].aplica_procesador_externo != null && dataMateriales[i].aplica_procesador_externo.Value ? "SÍ" : "NO",
                    string.Empty, //dataMateriales[i].procesador_externo_nombre != null && !string.IsNullOrEmpty(dataMateriales[i].procesador_externo_nombre) ? dataMateriales[i].procesador_externo_nombre : string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty, //dataMateriales[i].proveedor != null && !string.IsNullOrEmpty(dataMateriales[i].proveedor) ? dataMateriales[i].proveedor : string.Empty, //NUM
                    string.Empty, //dataMateriales[i].proveedor != null && !string.IsNullOrEmpty(dataMateriales[i].proveedor) ? dataMateriales[i].proveedor : string.Empty, //nombre fiscal                                       
                    string.Empty, //dataMateriales[i].aplica_tasa_iva.HasValue? dataMateriales[i].aplica_tasa_iva.Value?"SÍ":"NO":string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    !string.IsNullOrEmpty(referenciaData[c].nuevo_material)?referenciaData[c].nuevo_material: string.Empty,
                    !string.IsNullOrEmpty(referenciaData[c].numero_parte)?referenciaData[c].numero_parte: string.Empty,
                    string.Empty, //dimensiones, //DIMENSIONES
                    string.Empty,
                    string.Empty,
                    referenciaData[c].unidad_medida_inventario != null && !string.IsNullOrEmpty(referenciaData[c].unidad_medida_inventario) ? referenciaData[c].unidad_medida_inventario : string.Empty, //nombre fiscal  
                    string.Empty,
                    string.Empty,//dataMateriales[i].descripcion_material_es,
                    string.Empty,//dataMateriales[i].peso_min_kg.ToString(),
                    string.Empty,//dataMateriales[i].peso_max_kg.ToString(),
                    referenciaData[c].SCDM_cat_tipo_venta != null ? referenciaData[c].SCDM_cat_tipo_venta.descripcion : string.Empty, //nombre fiscal  
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    referenciaData[c].molino != null && !string.IsNullOrEmpty(referenciaData[c].molino) ? referenciaData[c].molino : string.Empty, //nombre fiscal 
                    string.Empty,

                    };

            }

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Carga las OC de la solicitud, segun las hojas de rollos, platinas, cintas, etc-
        /// </summary>
        /// <param name="id_solicitud"></param>
        /// <returns></returns>
        public JsonResult BuscaSMenOC(int id_solicitud = 0)
        {

            //obtiene el listado oc de la solicitud donde aplica procesador externo
            var data = db.SCDM_solicitud_rel_orden_compra.Where(x => x.id_solicitud == id_solicitud
                && x.aplica_procesador_externo.HasValue && x.aplica_procesador_externo.Value//acopio externo
            ).ToList();

            var listSM = db.SCDM_cat_materiales_maquila.Where(x => x.activo).ToList();

            var jsonData = new object[data.Count()];

            //obtiene la planta de la solicitud
            string plantaSol = db.SCDM_solicitud.Find(id_solicitud).SCDM_rel_solicitud_plantas.FirstOrDefault().plantas.codigoSap.ToString();

            for (int i = 0; i < data.Count(); i++)
            {
                string sm = "SM_PENDIENTE";

                var smItem = listSM.Where(x => x.planta == plantaSol && x.StandardPrice.ToString() == data[i].precio && x.StandardPrice > 0).FirstOrDefault();
                if (smItem != null)
                    sm = smItem.numero_material;

                //obtiene todos los posibles valores
                jsonData[i] = new[] {
                    sm,
                    data[i].num_material,
                    };
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDatosResultado(int? id_solicitud, string numero_material = "")
        {
            //inicia variables
            string tipo_material = "--";
            string peso_bruto_platina = "--";
            string peso_neto_platina = "--";
            string unidad_medida = "--";
            string SCDM_cat_tipo_venta = "--";

            //inicializa la lista de objetos
            var objeto = new object[1];
            SCDM_solicitud_rel_item_material itemMaterial = db.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.id_solicitud == id_solicitud && x.numero_material == numero_material);
            SCDM_solicitud_rel_creacion_referencia itemCreacionReferencia = db.SCDM_solicitud_rel_creacion_referencia.FirstOrDefault(x => x.id_solicitud == id_solicitud && x.nuevo_material == numero_material);

            //si existe en la material de solicitud
            if (itemMaterial != null)
            {
                tipo_material = itemMaterial.SCDM_cat_tipo_materiales_solicitud != null ? itemMaterial.SCDM_cat_tipo_materiales_solicitud.descripcion : numero_material.StartsWith("SM") ? "Maquila" : "--";
                peso_bruto_platina = itemMaterial.peso_bruto.HasValue ? itemMaterial.peso_bruto.ToString() : "--";
                peso_neto_platina = itemMaterial.peso_neto.HasValue ? itemMaterial.peso_neto.ToString() : "--";
                unidad_medida = !string.IsNullOrEmpty(itemMaterial.unidad_medida_inventario) ? itemMaterial.unidad_medida_inventario : "--";
                SCDM_cat_tipo_venta = !string.IsNullOrEmpty(itemMaterial.tipo_venta) ? itemMaterial.tipo_venta : "--";
            }

            //si existe en la creacion con referencia
            if (itemCreacionReferencia != null)
            {
                tipo_material = numero_material.StartsWith("SM") ? "Maquila" : itemCreacionReferencia.tipo_material_text;
                peso_bruto_platina = itemCreacionReferencia.peso_bruto.HasValue ? itemCreacionReferencia.peso_bruto.ToString() : "--";
                peso_neto_platina = itemCreacionReferencia.peso_neto.HasValue ? itemCreacionReferencia.peso_neto.ToString() : "--";
                unidad_medida = !string.IsNullOrEmpty(itemCreacionReferencia.unidad_medida_inventario) ? itemCreacionReferencia.unidad_medida_inventario : "--";
                SCDM_cat_tipo_venta = itemCreacionReferencia.SCDM_cat_tipo_venta != null ? itemCreacionReferencia.SCDM_cat_tipo_venta.descripcion : "--";
            }

            //si no se encuentra en ninguna de las anteriores, busca en el catálogo de MM
            if (itemMaterial == null && itemCreacionReferencia == null)
            {
                mm_v3 mm = db.mm_v3.FirstOrDefault(x => x.Material == numero_material);

                if (mm != null)
                {
                    tipo_material = numero_material.StartsWith("SM") ? "Maquila" : mm.Type_of_Material;
                    peso_bruto_platina = mm.Gross_weight.HasValue ? mm.Gross_weight.ToString() : "--";
                    peso_neto_platina = mm.Net_weight.HasValue ? mm.Net_weight.ToString() : "--";
                    unidad_medida = !string.IsNullOrEmpty(mm.unidad_medida) ? mm.unidad_medida : "--";
                    SCDM_cat_tipo_venta = mm.Type_of_Selling != null ? mm.Type_of_Selling : "--";
                }
            }


            objeto[0] = new
            {
                tipo_material,
                peso_bruto_platina,
                peso_neto_platina,
                unidad_medida,
                SCDM_cat_tipo_venta,
            };


            return Json(objeto, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GuardaComentario(int? id_solicitud, int? id_seccion, string comentario = "")
        {
            //inicia variables
            string resultado = "--";

            SCDM_rel_solicitud_secciones_activas seccion = db.SCDM_rel_solicitud_secciones_activas.FirstOrDefault(x => x.id_solicitud == id_solicitud && x.id_seccion == id_seccion);


            if (seccion != null)
            {
                //guarda el comentario
                if (string.IsNullOrEmpty(comentario))
                    comentario = null;

                seccion.comentario = comentario;

                try
                {
                    db.SaveChanges();
                    resultado = "Se guardó el comentario en BD.";
                }
                catch (Exception e)
                {
                    resultado = "Error al guardar comentario: " + e.Message;
                }
            }
            else
            {
                resultado = seccion == null ? "No se encuntró la sección en BD" : resultado;
            }


            //inicializa la lista de objetos
            var objeto = new object[1];

            objeto[0] = new
            {
                resultado
            };


            return Json(objeto, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene el puesto de un empleado
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult eliminaAsignacion(int id = 0)
        {
            //obtiene todos los posibles valores
            SCDM_solicitud_asignaciones asignacion = db.SCDM_solicitud_asignaciones.Find(id);


            if (asignacion != null)
            {
                //si ya no hay más departamentos asigna a SCDM
                if (asignacion.SCDM_solicitud.SCDM_solicitud_asignaciones.Count(x => x.fecha_cierre == null && x.fecha_rechazo == null) == 1) // es la última solicitud

                    db.SCDM_solicitud_asignaciones.Add(new SCDM_solicitud_asignaciones()
                    {
                        id_solicitud = asignacion.id_solicitud,
                        id_empleado = 447, //raul
                        id_departamento_asignacion = (int)SCDM_departamentos_AsignacionENUM.SCDM,
                        fecha_asignacion = DateTime.Now,
                        descripcion = Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM,
                    });



                db.SCDM_solicitud_asignaciones.Remove(asignacion);
                db.SaveChanges();
            }


            //inicializa la lista de objetos
            var resultado = new object[1];

            resultado[0] = new
            {
                correcto = true
            };

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Se cierra una asignacion en nombre de otra persona
        /// </summary>
        /// <param name="id_solicitud"></param>
        /// <param name="id_usuario"></param>
        /// <returns></returns>
        public JsonResult cierraAsignacionOtroUsuario(int id_asignacion, int id_empleado)
        {
            //obtiene la asignacion
            SCDM_solicitud_asignaciones asignacion = db.SCDM_solicitud_asignaciones.Find(id_asignacion);

            //obtiene el usuario logeado
            var usuarioLogeado = obtieneEmpleadoLogeado();

            if (asignacion != null)
            {
                //si ya no hay más departamentos asigna a SCDM
                if (asignacion.SCDM_solicitud.SCDM_solicitud_asignaciones.Count(x => x.fecha_cierre == null && x.fecha_rechazo == null) == 1) // es la última solicitud

                    db.SCDM_solicitud_asignaciones.Add(new SCDM_solicitud_asignaciones()
                    {
                        id_solicitud = asignacion.id_solicitud,
                        id_empleado = 447, //raul
                        id_departamento_asignacion = (int)SCDM_departamentos_AsignacionENUM.SCDM,
                        fecha_asignacion = DateTime.Now,
                        descripcion = Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM,
                    });

                //asigna los valores para el cierre de la asignacion
                asignacion.fecha_cierre = DateTime.Now;
                asignacion.id_cierre = id_empleado;

                //envia correo indicando que se cerro la actividad a nombre de otra persona
                empleados usarioCierre = db.empleados.Find(id_empleado);

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>(); //correos TO
                if (!string.IsNullOrEmpty(usarioCierre.correo))
                    correos.Add(usarioCierre.correo);

                //envia el correo a los usuarios utilizados
                //envioCorreo.SendEmailAsync(correos, "SCDM cerró la actividad de la solicitud " + asignacion.id_solicitud, "El usuario " + usuarioLogeado.ConcatNombre + " ha cerrado la actividad a tu nombre para la solicitud " + asignacion.id_solicitud);

                db.SaveChanges();
            }


            //inicializa la lista de objetos
            var resultado = new object[1];

            resultado[0] = new
            {
                correcto = true
            };

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Se cierra una asignacion en nombre de otra persona
        /// </summary>
        /// <param name="id_solicitud"></param>
        /// <param name="id_usuario"></param>
        /// <returns></returns>
        public JsonResult rechazo_scdm(int id_solicitud, string destinatario, int? id_motivo_rechazo, string comentario,
                        int? id_departamento, int[] usuarios)
        {
            //obtiene la asignacion
            SCDM_solicitud solicitud = db.SCDM_solicitud.Find(id_solicitud);

            //obtiene el usuario logeado
            var usuarioLogeado = obtieneEmpleadoLogeado();

            //inicializa la lista de objetos
            var resultado = new object[1];
            DateTime fechaActual = DateTime.Now;

            //variable para correosTO de 
            List<String> correosTO = new List<string> { }; //correos TO
            EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
            List<string> correosSCDM = db.SCDM_cat_rel_usuarios_departamentos.Where(x => x.id_departamento == (int)Bitacoras.Util.SCDM_departamentos_AsignacionENUM.SCDM).Select(x => x.empleados.correo).Distinct().ToList();



            //cierra asignación a SCDM, en caso de existir
            var asignacionSCDM = solicitud.SCDM_solicitud_asignaciones.Where(x => x.id_departamento_asignacion == (int)SCDM_departamentos_AsignacionENUM.SCDM && x.fecha_cierre == null && x.fecha_rechazo == null).FirstOrDefault();

            if (asignacionSCDM != null)
            {
                asignacionSCDM.fecha_rechazo = fechaActual;
                asignacionSCDM.comentario_rechazo = comentario;
                asignacionSCDM.id_motivo_rechazo = id_motivo_rechazo;
                asignacionSCDM.id_rechazo = usuarioLogeado.id;
            }

            if (destinatario == "solicitante")
            {
                //Envia a solicitante 
                var idDepartamentoSolicitante = solicitud.empleados.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault() != null ? solicitud.empleados.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault().id_departamento : 99;

                db.SCDM_solicitud_asignaciones.Add(
                    new SCDM_solicitud_asignaciones
                    {
                        id_solicitud = solicitud.id,
                        id_departamento_asignacion = idDepartamentoSolicitante,
                        id_empleado = solicitud.id_solicitante,
                        fecha_asignacion = fechaActual,
                        descripcion = Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE
                    }
                );
                //agrega el correo del solicitante
                correosTO.Add(solicitud.empleados.correo);

                try
                {
                    db.SaveChanges();
                    envioCorreo.SendEmailAsync(correosTO, "MDM - Solicitud: " + solicitud.id + " -->  Tu solicitud ha sido rechazada por SCDM.",
                          envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.RECHAZA_SOLICITUD_SCDM_A_SOLICITANTE, usuarioLogeado, solicitud, SCDM_tipo_view_edicionENUM.SOLICITANTE, comentarioRechazo: comentario, departamento: "SCDM")
                          , emailsCC: correosSCDM); //Envia copia a SCDM

                    resultado[0] = new
                    {
                        correcto = true,
                        mensaje = "Se realizó la asignación de forma correcta.",
                    };
                }
                catch (Exception ex)
                {
                    resultado[0] = new
                    {
                        correcto = false,
                        mensaje = "Ocurrió un error: " + ex.Message,
                    };
                }
            }
            else if (destinatario == "departamento")
            {
                //obtiene el departamento
                var depto = db.SCDM_cat_departamentos_asignacion.Find(id_departamento);

                //envia a departamento
                db.SCDM_solicitud_asignaciones.Add(new SCDM_solicitud_asignaciones
                {
                    id_solicitud = solicitud.id,
                    id_departamento_asignacion = id_departamento.Value,
                    id_empleado = usuarios[0],
                    fecha_asignacion = fechaActual,
                    descripcion = Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO
                });

                //agrega los correos del Departamento
                foreach (var id_emp in usuarios)
                {
                    var emp = db.empleados.Find(id_emp);
                    if (emp != null)
                        correosTO.Add(emp.correo);
                }

                correosSCDM.Add(solicitud.empleados.correo);

                try
                {
                    db.SaveChanges();
                    envioCorreo.SendEmailAsync(correosTO, "MDM - Solicitud: " + solicitud.id + " --> Se ha asignado una actividad para " + depto.descripcion.ToUpper() + ".",
                           envioCorreo.getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM.ASIGNACION_SOLICITUD_A_DEPARTAMENTO, usuarioLogeado, solicitud, SCDM_tipo_view_edicionENUM.DEPARTAMENTO, departamento: depto.descripcion.ToUpper(), comentario: comentario)
                           , emailsCC: correosSCDM);

                    resultado[0] = new
                    {
                        correcto = true,
                        mensaje = "Se realizó la asignación de forma correcta.",
                    };
                }
                catch (Exception ex)
                {
                    resultado[0] = new
                    {
                        correcto = false,
                        mensaje = "Ocurrió un error: " + ex.Message,
                    };
                }

            }



            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GeneraArchivoFacturacion(int? id_solicitud)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_APROBACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");


            List<SCDM_solicitud_rel_facturacion> listado = db.SCDM_solicitud_rel_facturacion.Where(x => (x.SCDM_solicitud_rel_item_material != null && x.SCDM_solicitud_rel_item_material.id_solicitud == id_solicitud)
                                                                || (x.SCDM_solicitud_rel_creacion_referencia != null && x.SCDM_solicitud_rel_creacion_referencia.id_solicitud == id_solicitud)).ToList();

            byte[] stream = ExcelUtil.SCDM_GeneraArchivoFacturacion(id_solicitud, listado);


            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = "SCDM_Facturacion_solicitud_" + id_solicitud + ".xlsm",

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(stream, "application/vnd.ms-excel.sheet.macroEnabled.12");


        }
        #endregion

        /// <summary>
        /// Muestra el manual de usuario
        /// </summary>
        /// <returns></returns>
        public ActionResult ManualUsuario()
        {

            String ruta = System.Web.HttpContext.Current.Server.MapPath("~/Content/manuales/Manual_Maestro_Materiales.pdf");

            //byte[] array = System.IO.File.ReadAllBytes(ruta);

            FileInfo archivo = new FileInfo(ruta);

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = archivo.Name,
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(ruta, "application/pdf");


        }

    }
}
