using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using Bitacoras.Util;
using Clases.Util;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Ajax.Utilities;
using Org.BouncyCastle.Math;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class SCDM_solicitudController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: SCDM_solicitud
        public ActionResult Index(string estatus, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) && !TieneRol(TipoRoles.SCDM_MM_APROBACION_SOLICITUDES) &&
                !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES) && !TieneRol(TipoRoles.SCDM_MM_REPORTES))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();
            var listTotal = db.SCDM_solicitud.Where(x => x.id_solicitante == empleado.id).ToList();

            //listado de creadas sin enviar
            var listCreadas = listTotal.Where(x => !x.SCDM_solicitud_asignaciones.Any()).ToList();
            //listado Asignadas a SCDM                       
            var listAsignadasSCDM = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null
                && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM)).ToList();
            //listado asignadas a otros Departamentos
            var listAsignadasOtrosDepartamentos = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null
                && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO)).ToList();
            //rechazadas
            var listRechazadasSolicitante = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null
                && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE)).ToList();
            //finalizadas
            var listFinalizadas = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any() && !x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null)).ToList();


            List<SCDM_solicitud> listado = new List<SCDM_solicitud> { };

            //stringdetermina la lista a buscar
            switch (estatus)
            {
                case "CREADO":
                    listado = listCreadas;
                    break;
                case "SCDM":
                    listado = listAsignadasSCDM;
                    break;
                case "ASIGNADA":
                    listado = listAsignadasOtrosDepartamentos;
                    break;
                case "RECHAZADA_SCDM":
                    listado = listRechazadasSolicitante;
                    break;
                case "FINALIZADA":
                    listado = listFinalizadas;
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
            estatusMap.Add(Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.CREADO), "Creadas (Sin enviar)");
            estatusMap.Add(Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.SCDM), "Asignadas a SCDM");
            estatusMap.Add(Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.ASIGNADA), "Asignadas a otros deptos.");
            estatusMap.Add(Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.RECHAZADA_SCDM), "Rechazadas");
            estatusMap.Add(Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.FINALIZADA), "Finalizadas");

            ViewBag.estatusMap = estatusMap;

            //map para cantidad de status
            Dictionary<string, int> estatusAmount = new Dictionary<string, int>
            {
                { "", listTotal.Count() },
                //obtiene las solicitudes abiertas para el depto de scdm
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.SCDM), listAsignadasSCDM.Count() },
                //obtiene las solicitudes abiertas para cualquier otro departamento
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.ASIGNADA), listAsignadasOtrosDepartamentos.Count()},
                //obtiene las solicitudes cuyo ultima asignación esta rechazada por scdm
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.RECHAZADA_SCDM), listRechazadasSolicitante.Count()},
                //obtiene las sulicitudes cuya ultima asignación ha sido rechazada
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.FINALIZADA), listFinalizadas.Count()},
                //obtiene las sulicitudes sin enviar
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.CREADO), listCreadas.Count()}
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
        public ActionResult Estatus(string estatus, int pagina = 1)
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

            var empleado = obtieneEmpleadoLogeado();
            var listTotal = db.SCDM_solicitud.Where(x => x.activo).ToList().Where(x => x.SCDM_solicitud_asignaciones.Any() // ya fue asignado al menos una vez
                                        ).ToList();
            //listado sin finalizar 
            var listEnProceso = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null)).ToList();
            //listado finalizadas 
            var listFinalizadas = listTotal.Where(x => !x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null)).ToList();

            List<SCDM_solicitud> listado = new List<SCDM_solicitud> { };

            //stringdetermina la lista a buscar
            switch (estatus)
            {
                case "ASIGNADA":
                    listado = listEnProceso;
                    break;
                case "FINALIZADA":
                    listado = listFinalizadas;
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
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.ASIGNADA), "En Proceso" },
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.FINALIZADA), "Finalizadas" }
            };

            ViewBag.estatusMap = estatusMap;

            //map para cantidad de status
            Dictionary<string, int> estatusAmount = new Dictionary<string, int>
            {
                { "", listTotal.Count() },          
                //obtiene las solicitudes abiertas para cualquier otro departamento
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.ASIGNADA), listEnProceso.Count() },
                //obtiene las sulicitudes cuya ultima asignación ha sido rechazada
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.FINALIZADA), listFinalizadas.Count()},
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

            ViewBag.ListadoDepartamentos = db.SCDM_cat_departamentos_asignacion.Where(x => x.activo).ToList();

            return View(listado);
        }

        // GET: SCDM_solicitud
        public ActionResult SolicitudesSCDM(string estatus, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro

            var listTotal = db.SCDM_solicitud.ToList();

            //listado de creadas sin enviar
            var listCreadas = listTotal.Where(x => !x.SCDM_solicitud_asignaciones.Any()).ToList();
            //listado Asignadas a SCDM                       
            var listAsignadasSCDM = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null
                && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM)).ToList();
            //listado asignadas a otros Departamentos
            var listAsignadasOtrosDepartamentos = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null
                && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO)).ToList();
            //rechazadas
            var listRechazadasSolicitante = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null
                && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE)).ToList();
            //finalizadas
            var listFinalizadas = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any() && !x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null)).ToList();

            List<SCDM_solicitud> listado = new List<SCDM_solicitud> { };

            //stringdetermina la lista a buscar
            switch (estatus)
            {
                case "CREADO":
                    listado = listCreadas;
                    break;
                case "SCDM":
                    listado = listAsignadasSCDM;
                    break;
                case "ASIGNADA":
                    listado = listAsignadasOtrosDepartamentos;
                    break;
                case "RECHAZADA_SCDM":
                    listado = listRechazadasSolicitante;
                    break;
                case "FINALIZADA":
                    listado = listFinalizadas;
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
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.SCDM), "Asignadas a SCDM" },
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.ASIGNADA), "Asignadas a otros deptos." },
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.RECHAZADA_SCDM), "Rechazadas (reenviadas)" },
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.FINALIZADA), "Finalizadas" },
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.CREADO), "Creadas (sin enviar)" }
            };

            ViewBag.estatusMap = estatusMap;

            //map para cantidad de status
            Dictionary<string, int> estatusAmount = new Dictionary<string, int>
            {
                { "", listTotal.Count() },
                //obtiene las solicitudes abiertas para el depto de scdm
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.SCDM), listAsignadasSCDM.Count()},
                //obtiene las solicitudes abiertas para cualquier otro departamento
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.ASIGNADA), listAsignadasOtrosDepartamentos.Count()},
                //obtiene las solicitudes cuyo ultima asignación esta rechazada por scdm
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.RECHAZADA_SCDM),  listRechazadasSolicitante.Count()},
                //obtiene las sulicitudes cuya ultima asignación ha sido rechazada
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.FINALIZADA), listFinalizadas.Count() },
                //obtiene las sulicitudes creadas sin enviar
                { Enum.GetName(typeof(SCMD_solicitud_estatus_enum), (int)SCMD_solicitud_estatus_enum.CREADO), listCreadas.Count()}
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
        public ActionResult SolicitudesDepartamento(string estatus, int pagina = 1)
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


            var listTotal = db.SCDM_solicitud.Where(x => x.SCDM_solicitud_asignaciones.Any(y =>
                         y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO)
                        && x.SCDM_rel_solicitud_plantas.Any(y => plantas_asignadas.Contains(y.id_planta)) //verifica que el empleado este asignado a esta planta
            ).ToList();

            //listado de solicitudes pendientes
            var listPendientes = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null && y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO)).ToList();
            //listado solicitudes aprobadas
            var listAprobadas = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Last(y => y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO).fecha_cierre != null).ToList();
            var listRechazadas = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Last(y => y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO).fecha_rechazo != null).ToList();
            List<SCDM_solicitud> listado = new List<SCDM_solicitud> { };

            //determina la lista a buscar
            switch (estatus)
            {
                case "Pendientes":
                    listado = listPendientes;
                    break;
                case "Rechazadas":
                    listado = listRechazadas;
                    break;
                case "Finalizadas":
                    listado = listAprobadas;
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
                {"Finalizadas", "Finalizadas" },
                {"Rechazadas", "Rechazadas" },
            };

            ViewBag.estatusMap = estatusMap;

            //map para cantidad de status
            Dictionary<string, int> estatusAmount = new Dictionary<string, int>
            {
                { "", listTotal.Count() },
                { "Pendientes", listPendientes.Count() },
                { "Finalizadas", listAprobadas.Count() },
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
        /// <param name="pagina"></param>
        /// <returns></returns>
        public ActionResult SolicitudesRevisionInicial(string estatus, int pagina = 1)
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
            var listTotal = db.SCDM_solicitud.Where(x => x.SCDM_solicitud_asignaciones.Any(y => //y.fecha_cierre == null && y.fecha_rechazo == null
                         y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL
                        && y.id_empleado == empleado.id // verifica que el empleado asignado sea quien inicia sesión
                        )
                        && x.SCDM_rel_solicitud_plantas.Any(y => plantas_asignadas.Contains(y.id_planta)) //verifica que el empleado este asignado a esta planta
            ).ToList();
            //listado de solicitudes pendientes
            var listPendientes = listTotal.Where(x => x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null && y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL)).ToList();
            //listado solicitudes aprobadas
            var listAprobadas = listTotal.Where(x => !x.SCDM_solicitud_asignaciones.Any(y => y.fecha_cierre == null && y.fecha_rechazo == null && y.id_departamento_asignacion == id_depto_scdm
                        && y.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL)).ToList();

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
            };

            ViewBag.estatusMap = estatusMap;

            //map para cantidad de status
            Dictionary<string, int> estatusAmount = new Dictionary<string, int>
            {
                { "", listTotal.Count() },
                  //obtiene las solicitudes abiertas para cualquier otro departamento
                { "Pendientes", listPendientes.Count() },
                { "Aprobadas", listAprobadas.Count() },

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
            ViewBag.EmpleadoDepartamento = id_departamento;
            ViewBag.ListadoDepartamentos = db.SCDM_cat_departamentos_asignacion.Where(x => x.activo).ToList();
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
        public ActionResult Create(SCDM_solicitud sCDM_solicitud, FormCollection collection, string[] SelectedMateriales, string[] SelectedPlantas)
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
                            Nombre = nombreArchivo,
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

            if (SelectedMateriales == null && (sCDM_solicitud.id_tipo_solicitud == 1 || sCDM_solicitud.id_tipo_solicitud == 2 || sCDM_solicitud.id_tipo_solicitud == 5))
                ModelState.AddModelError("", "Seleccione los materiales deseados para la solicitud.");

            if (SelectedPlantas == null)
                ModelState.AddModelError("", "Seleccione las plantas para las cuales aplica la solicitud.");

            //si es creación o extensión, al menos un archivo es requerido
            if ((sCDM_solicitud.id_tipo_solicitud == 1 || sCDM_solicitud.id_tipo_solicitud == 5) && archivosForm.Count == 0)
                ModelState.AddModelError("", "Agregue al menos un archivo de soporte, factibilidad, desviación, etc.");

            //si no es cambios
            if (sCDM_solicitud.id_tipo_solicitud != 3)
                sCDM_solicitud.id_tipo_cambio = null;

            //ModelState.AddModelError("", "Error para depuración.");

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


                                if (seccion != 0)
                                    sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                                    {
                                        id_seccion = seccion
                                    });
                            }
                            break;

                        case SCDMTipoSolicitudENUM.CREACION_REFERENCIA:
                            sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                            {
                                id_seccion = (int)SCDMSeccionesSolicitud.CREACION_REFERENCIA
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
                            break;
                        case SCDMTipoSolicitudENUM.CREAR_MRO:
                        case SCDMTipoSolicitudENUM.CREACION_SERVICIOS:

                            sCDM_solicitud.SCDM_rel_solicitud_secciones_activas.Add(new SCDM_rel_solicitud_secciones_activas
                            {
                                id_seccion = (int)SCDMSeccionesSolicitud.FORMATO_COMPRA
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
            ViewBag.listTipoMateriales = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList();

            ViewBag.id_prioridad = AddFirstItem(new SelectList(db.SCDM_cat_prioridad.Where(x => x.activo == true), nameof(SCDM_cat_prioridad.id), nameof(SCDM_cat_prioridad.descripcion)));
            ViewBag.id_tipo_cambio = AddFirstItem(new SelectList(db.SCDM_cat_tipo_cambio.Where(x => x.activo == true), nameof(SCDM_cat_tipo_cambio.id), nameof(SCDM_cat_tipo_cambio.descripcion)));
            ViewBag.id_tipo_solicitud = AddFirstItem(new SelectList(db.SCDM_cat_tipo_solicitud.Where(x => x.activo == true), nameof(SCDM_cat_tipo_solicitud.id), nameof(SCDM_cat_tipo_solicitud.descripcion)));
            return View(sCDM_solicitud);
        }

        // GET: SCDM_solicitud/Edit/5
        public ActionResult Edit(int? id)
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
        public ActionResult Edit(SCDM_solicitud sCDM_solicitud, FormCollection collection, string[] SelectedMateriales, string[] SelectedPlantas)
        {
            var solicitante = obtieneEmpleadoLogeado();
            List<int> idsArchivos = new List<int>();

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
                            Nombre = nombreArchivo,
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

            if (SelectedMateriales == null && (sCDM_solicitud.id_tipo_solicitud == 1 || sCDM_solicitud.id_tipo_solicitud == 2 || sCDM_solicitud.id_tipo_solicitud == 5))
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
                    SCDM_solicitud solicitudBD = db.SCDM_solicitud.Find(sCDM_solicitud.id);
                    //copia los valores del formulario al modelo
                    solicitudBD.id_tipo_solicitud = sCDM_solicitud.id_tipo_solicitud;
                    solicitudBD.id_tipo_cambio = sCDM_solicitud.id_tipo_cambio;
                    solicitudBD.id_prioridad = sCDM_solicitud.id_prioridad;
                    solicitudBD.descripcion = sCDM_solicitud.descripcion;
                    solicitudBD.justificacion = sCDM_solicitud.justificacion;

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
                    return RedirectToAction("EditarSolicitud", new { id = sCDM_solicitud.id });
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
            ViewBag.listTipoMateriales = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList();

            ViewBag.id_prioridad = AddFirstItem(new SelectList(db.SCDM_cat_prioridad.Where(x => x.activo == true), nameof(SCDM_cat_prioridad.id), nameof(SCDM_cat_prioridad.descripcion)));
            ViewBag.id_tipo_cambio = AddFirstItem(new SelectList(db.SCDM_cat_tipo_cambio.Where(x => x.activo == true), nameof(SCDM_cat_tipo_cambio.id), nameof(SCDM_cat_tipo_cambio.descripcion)));
            ViewBag.id_tipo_solicitud = AddFirstItem(new SelectList(db.SCDM_cat_tipo_solicitud.Where(x => x.activo == true), nameof(SCDM_cat_tipo_solicitud.id), nameof(SCDM_cat_tipo_solicitud.descripcion)));
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
            var id_depto_scdm = solicitante.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault().id_departamento;

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
                    case 12: //Puebla      //solo asigna a ventas en caso de ventas e ingenieria Puebla
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
            ViewBag.EmpleadoDepartamento = id_depto_scdm;
            ViewBag.ListadoDepartamentos = db.SCDM_cat_departamentos_asignacion.Where(x => x.activo).ToList();


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
            ViewBag.listDepartamentos = db.SCDM_cat_departamentos_asignacion.Where(x => x.activo && x.id != 9).ToList();
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
                    bool enviar = bool.Parse(array[3]);

                    //si no se ha agregado una asignación al departamento
                    if (!asignaciones.Any(x => x.id_departamento_asignacion == Int32.Parse(array[0]))
                        && !solicitud.SCDM_solicitud_asignaciones.Any(x => x.id_cierre == null && x.id_rechazo == null
                            && x.id_departamento_asignacion == Int32.Parse(array[0]))
                            && enviar
                        )
                    {

                        //obtiene el primer comentario
                        string cometarioSCDM = null;
                        string[] lineaComentario = dataListFromTable.FirstOrDefault(x => x[0] == array[0] && !string.IsNullOrEmpty(x[9]) && x[9] != "---");

                        if (lineaComentario != null && !string.IsNullOrEmpty(lineaComentario[9]))
                            cometarioSCDM = lineaComentario[9];

                        asignaciones.Add(new SCDM_solicitud_asignaciones()
                        {
                            id_solicitud = id.Value,
                            id_empleado = Int32.Parse(array[1]),
                            id_departamento_asignacion = Int32.Parse(array[0]),
                            fecha_asignacion = DateTime.Now,
                            descripcion = Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO,
                            comentario_scdm = !String.IsNullOrEmpty(cometarioSCDM) ? cometarioSCDM : null
                        }
                        );
                    }
                    else if (enviar)
                    { // si ya existe una asignacion se guarda el depto para manda
                        idsDepartamentoRecordatorios.Add(array[4]);
                    }
                }
                db.SCDM_solicitud_asignaciones.AddRange(asignaciones);
                db.SaveChanges();

                //obtiene el listado de correos
                foreach (var array in dataListFromTable)
                {
                    string departamento = array[4];
                    string correo = array[7];
                    bool enviar = bool.Parse(array[3]);

                    //si no existe lo crea
                    if (!listCorreo.Any(x => x.departamento == departamento))
                    {
                        listCorreo.Add(new EnvioCorreoAsignacionSCDM { departamento = departamento });
                    }
                    //busca el list actual
                    var correoN = listCorreo.FirstOrDefault(x => x.departamento == departamento);
                    //busca si no ya existe el correo en el depto actual
                    if (!correoN.correos.Any(x => x == correo) && enviar)
                    {
                        correoN.correos.Add(correo);
                    }
                }

                //envia correos
                foreach (var item in listCorreo)
                {
                    if (!idsDepartamentoRecordatorios.Any(x => x == item.departamento))
                        System.Diagnostics.Debug.WriteLine("Enviando a Departamento: " + item.departamento + ", correos:" + String.Join(" - ", item.correos));
                    else
                        System.Diagnostics.Debug.WriteLine("Enviando Recordatorio a : " + item.departamento + ", correos:" + String.Join(" - ", item.correos));

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
        public ActionResult EnviaSolicitudInicio(int? id, int? revisaDepartamento, string revisaFormato)
        {

            //obtiene el id_empleado según 
            var revisaCorreo = db.notificaciones_correo.Where(x => x.descripcion == revisaFormato);

            //actualiza la asignacion del usuario en caso de existir
            var sCDM_solicitudBD = db.SCDM_solicitud.Find(id);
            var empleado = obtieneEmpleadoLogeado();
            var idDepartamento = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault(x => x.id_empleado == empleado.id).id_departamento;
            var asignacionAnterior = sCDM_solicitudBD.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_departamento_asignacion == idDepartamento && (x.fecha_cierre == null && x.fecha_rechazo == null) && x.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE);

            if (asignacionAnterior != null)
            {
                asignacionAnterior.fecha_cierre = DateTime.Now;
                asignacionAnterior.id_cierre = empleado.id;
            }

            //crea una asignacion para SCDM o Asignación inicial
            var asignacion = new SCDM_solicitud_asignaciones()
            {
                id_solicitud = id.Value,
                id_empleado = revisaCorreo.FirstOrDefault().empleados.id,
                id_departamento_asignacion = revisaDepartamento.Value,
                fecha_asignacion = DateTime.Now,
                descripcion = revisaDepartamento.Value == (int)SCDM_departamentos_AsignacionENUM.SCDM ? Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM : Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL,
            };

            try
            {
                db.SCDM_solicitud_asignaciones.Add(asignacion);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha enviado la solicitud correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>(); //correos TO
                foreach (var item in revisaCorreo)
                    correos.Add(item.empleados.correo);
                envioCorreo.SendEmailAsync(correos, "Se te ha asignado la solicitud: " + id, "Se te ha asignado la solicitud " + id + " para tu revisión.");

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
            var idDepartamento = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault(x => x.id_empleado == empleado.id).id_departamento;
            var id_asignación = sCDM_solicitudBD.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_departamento_asignacion == idDepartamento
                && (x.fecha_cierre == null && x.fecha_rechazo == null)
                && ((viewUser == (int)SCDM_tipo_view_edicionENUM.DEPARTAMENTO && x.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO)
                || (viewUser == (int)SCDM_tipo_view_edicionENUM.DEPARTAMENTO_INICIAL && x.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL)
                )
            ).id;

            //var solicitudesPendientesNum = sCDM_solicitudBD.SCDM_solicitud_asignaciones.Where(x => x.fecha_cierre == null && x.fecha_rechazo == null).Count();

            var asignacionAnterior = db.SCDM_solicitud_asignaciones.Find(id_asignación);
            asignacionAnterior.fecha_cierre = DateTime.Now;
            asignacionAnterior.id_cierre = empleado.id;

            //obtiene el id_empleado según 
            var revisaCorreo = db.notificaciones_correo.Where(x => x.descripcion == revisaFormato);

            //si no existe lo agrega
            SCDM_solicitud_asignaciones asignacion = null;

            if (!sCDM_solicitudBD.SCDM_solicitud_asignaciones.Any(x => x.fecha_cierre == null && x.fecha_rechazo == null
                && x.id_departamento_asignacion == (int)SCDM_departamentos_AsignacionENUM.SCDM))
            {
                // si no hay mas asignaciones a otros departamentos
                if (!sCDM_solicitudBD.SCDM_solicitud_asignaciones.Any(x => x.fecha_cierre == null && x.fecha_rechazo == null && x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO))
                    asignacion = new SCDM_solicitud_asignaciones()
                    {
                        id_solicitud = id.Value,
                        id_empleado = revisaCorreo.FirstOrDefault().empleados.id,
                        id_departamento_asignacion = revisaDepartamento.Value,
                        fecha_asignacion = DateTime.Now,
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
                envioCorreo.SendEmailAsync(correos, "Se te ha asignado la solicitud: " + id, "Se te ha asignado la solicitud " + id + " para tu revisión.");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);
            }

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

            // Register List of Languages     

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
            ViewBag.IHSArray = db.SCDM_cat_ihs.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ModeloNegocioArray = db.SCDM_cat_modelo_negocio.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TransitoArray = db.SCDM_cat_tipo_transito.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.FormaArray = db.SCDM_cat_forma_material.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
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

            //ViewBag.TipoVentaArray = db.SCDM_cat_tipo_venta.Where(x => x.activo).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.TipoMaterialArray = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.PlantaArray = db.plantas.Where(x => x.activo == true && idPlantas.Contains(x.clave)).ToList().Select(x => x.ConcatPlantaSap.Trim()).ToArray();
            //ViewBag.MotivoCreacionArray = db.SCDM_cat_motivo_creacion.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.UnidadMedidaArray = db.SCDM_cat_unidades_medida.Where(x => x.activo == true).ToList().Select(x => x.codigo.Trim()).ToArray();
            ViewBag.CommodityArray = db.SCDM_cat_commodity.Where(x => x.activo == true).ToList().Select(x => x.ConcatCommodity.Trim()).ToArray();
            ViewBag.GradoCalidadArray = db.SCDM_cat_grado_calidad.Where(x => x.activo == true).ToList().Select(x => x.grado_calidad).ToArray();
            ViewBag.SuperficieArray = db.SCDM_cat_superficie.Where(x => x.activo == true).ToList().Select(x => x.descripcion).ToArray();
            ViewBag.TratamientoSuperficialArray = db.SCDM_cat_tratamiento_superficial.Where(x => x.activo == true).ToList().Select(x => x.descripcion).ToArray();
            ViewBag.PesoRecubrimientoArray = db.SCDM_cat_peso_recubrimiento.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.MolinosArray = db.SCDM_cat_molino.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.FormaArray = db.SCDM_cat_forma_material.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ClientesArray = db.clientes.Where(x => x.activo == true).ToList().Select(x => x.ConcatClienteSAP.Trim()).ToArray();

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

            //Viewbag para dropdowns
            ViewBag.TipoOCArray = db.SCDM_cat_po_existente.Where(x => x.activo).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.ProveedoresArray = db.proveedores.Where(x => x.activo == true).ToList().Select(x => x.ConcatproveedoresAP.Trim()).ToArray();
            ViewBag.CentroReciboArray = db.SCDM_cat_centro_recibo.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.IncotermArray = db.SCDM_cat_incoterm.Where(x => x.activo == true).ToList().Select(x => x.ConcatCodigoDescripcion.Trim()).ToArray();
            ViewBag.FronteraPuertoArray = db.SCDM_cat_frontera_puerto_planta.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.CondicionesPagoArray = db.SCDM_cat_po_condiciones_pago.Where(x => x.activo == true).ToList().Select(x => x.ConcatCodigoDescripcion.Trim()).ToArray();
            ViewBag.TransporteArray = db.SCDM_cat_po_transporte.Where(x => x.activo == true).ToList().Select(x => x.descripcion.Trim()).ToArray();
            ViewBag.MonedaArray = db.SCDM_cat_moneda.Where(x => x.activo == true).ToList().Select(x => x.clave_iso.Trim()).ToArray();
            ViewBag.UnidadMedidaArray = db.SCDM_cat_unidades_medida.Where(x => x.activo == true).ToList().Select(x => x.codigo.Trim()).ToArray();
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
        public ActionResult AddSeccion(int? id, int? secciones)
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

            return RedirectToAction("EditarSolicitud", new { id = id });
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
            var idDepartamento = empleado.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault(x => x.id_empleado == empleado.id).id_departamento;
            var asignacion = sCDM_solicitudBD.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_departamento_asignacion == idDepartamento && (x.fecha_cierre == null && x.fecha_rechazo == null));


            if (asignacion != null)
            {
                asignacion.fecha_rechazo = DateTime.Now;
                asignacion.comentario_rechazo = solicitud.comentario_rechazo;
                asignacion.id_motivo_rechazo = solicitud.id_motivo_rechazo;
                asignacion.id_rechazo = empleado.id;

                //crea una asignacion en caso de que sea la asinación inicial o rechazo SCDM                                                                                                                                 
                if (asignacion.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL ||
                    asignacion.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM ||
                    asignacion.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO
                    )
                {

                    var empleadoSolicitante = db.empleados.Find(sCDM_solicitudBD.id_solicitante);
                    var idDepartamentoSolicitante = empleadoSolicitante.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault(x => x.id_empleado == empleadoSolicitante.id).id_departamento;

                    int departamentoAsignacion = asignacion.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO ?
                                        (int)SCDM_departamentos_AsignacionENUM.SCDM : idDepartamentoSolicitante;

                    string descripcion = asignacion.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO ?
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
                            id_empleado = asignacion.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO ?
                                            447 : sCDM_solicitudBD.id_solicitante,
                            fecha_asignacion = DateTime.Now,
                            descripcion = descripcion
                        });
                }

            }
            //si es SCDM quien rechaza y no existe una asignación previa, envia rechazo a Solicitante
            else if (idDepartamento == (int)Bitacoras.Util.SCDM_departamentos_AsignacionENUM.SCDM)
            {

                var idDepartamentoSolicitante = sCDM_solicitudBD.empleados.SCDM_cat_rel_usuarios_departamentos.FirstOrDefault(x => x.id_empleado == sCDM_solicitudBD.id_solicitante).id_departamento;

                db.SCDM_solicitud_asignaciones.Add(new SCDM_solicitud_asignaciones
                {
                    id_solicitud = sCDM_solicitudBD.id,
                    id_departamento_asignacion = idDepartamentoSolicitante,
                    id_empleado = sCDM_solicitudBD.id_solicitante,
                    fecha_asignacion = DateTime.Now,
                    descripcion = SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE
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
                envioCorreo.SendEmailAsync(correos, "Se ha rechazado la solicitud: " + sCDM_solicitudBD.id, "Se te ha asignado la solicitud " + sCDM_solicitudBD.id + " para tu revisión.");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);
            }

            if (idDepartamento == (int)SCDM_departamentos_AsignacionENUM.SCDM) //SCDM
                return RedirectToAction("SolicitudesSCDM");
            else
            {
                //determinar vista a la que dirigir cuando no sea SCDM
                return RedirectToAction("SolicitudesDepartamento");
            }
        }

        // POST: SCDM_solicitud/FinalizarSolicitud/5
        [HttpPost, ActionName("FinalizarSolicitud")]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizarSolicitud(SCDM_solicitud solicitud)
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
                asignacion.fecha_cierre = DateTime.Now;
                asignacion.id_cierre = empleado.id;
            }

            try
            {
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se finalizó la solicitud correctamente", TipoMensajesSweetAlerts.SUCCESS);
                //ENVIAR SOLICITUD DE RECHAZO EMAIL
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>
                {
                    sCDM_solicitudBD.empleados.correo
                }; //correos TO
                envioCorreo.SendEmailAsync(correos, "Se ha finalizado la solicitud: " + sCDM_solicitudBD.id, "La solicitud " + sCDM_solicitudBD.id + " ha sido finalizada.");
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
                var toDeleteList = db.SCDM_solicitud_rel_creacion_referencia.ToList().Where(x => !cambioIngenieriaList.Any(y => y.id == x.id) && x.id_solicitud == id.Value).ToList();
                db.SCDM_solicitud_rel_creacion_referencia.RemoveRange(toDeleteList);
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


        public JsonResult ObtieneCorreos(bool isSelected, int idDepartamento, int idSolicitud)
        {
            //obtiene las plantas asociadas a la solicitud
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);
            List<plantas> plantas = solicitud.SCDM_rel_solicitud_plantas.Select(x => x.plantas).ToList();

            //obtiene todos los usuarios activos para las plantas de la solicitud, segun el depto
            var data = db.SCDM_cat_usuarios_revision_departamento.ToList().Where(x => x.activo && x.SCDM_cat_rel_usuarios_departamentos.id_departamento == idDepartamento && plantas.Any(y => y == x.plantas) && x.envia_correo).ToList();

            //var data = db.SCDM_solicitud_rel_item_material.Where(x => x.id_solicitud == id_solicitud && x.id_tipo_material == Bitacoras.Util.SCDM_solicitud_rel_item_material_tipo.ROLLO).ToList();

            var jsonData = new object[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {

                jsonData[i] = new[] {
                    data[i].SCDM_cat_rel_usuarios_departamentos.id_departamento.ToString(),
                    data[i].SCDM_cat_rel_usuarios_departamentos.id_empleado.ToString(),
                    data[i].id.ToString(), //SCDM_cat_usuarios_revision_departamento
                    "true",
                    data[i].SCDM_cat_rel_usuarios_departamentos.SCDM_cat_departamentos_asignacion.descripcion,
                    data[i].plantas.ConcatPlantaSap,
                    data[i].SCDM_cat_rel_usuarios_departamentos.empleados.ConcatNombre,
                    data[i].SCDM_cat_rel_usuarios_departamentos.empleados.correo,
                    data[i].tipo,
                    i==0? "" : "---",
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

                    };

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
            var dataMateriales = db.SCDM_solicitud_rel_item_material.Where(x => x.id_solicitud == id_solicitud).ToList();
            var dataCreacion = db.SCDM_solicitud_rel_creacion_referencia.Where(x => x.id_solicitud == id_solicitud).ToList();

            //inicializa el arreglo
            var jsonData = new object[dataMateriales.Count() + dataCreacion.Count()];

            //llena los valores para los materiales
            for (int i = 0; i < dataMateriales.Count(); i++)
            {

                jsonData[i] = new[] {
                    dataMateriales[i].numero_material,
                    dataMateriales[i].SCDM_cat_tipo_materiales_solicitud!=null? dataMateriales[i].SCDM_cat_tipo_materiales_solicitud.descripcion:string.Empty,
                    !string.IsNullOrEmpty(dataMateriales[i].tipo_venta)?  dataMateriales[i].tipo_venta : string.Empty,
                    dataMateriales[i].descripcion_material_es,
                    dataMateriales[i].numero_parte,
                    dataMateriales[i].peso_bruto.ToString(),
                    dataMateriales[i].peso_neto.ToString(),
                    !string.IsNullOrEmpty(dataMateriales[i].unidad_medida_inventario)?  dataMateriales[i].unidad_medida_inventario : string.Empty,
                    dataMateriales[i].numero_cintas_resultantes.ToString(),
                    !string.IsNullOrEmpty(dataMateriales[i].clase_aprovisionamiento)?  dataMateriales[i].clase_aprovisionamiento : string.Empty,
                    dataMateriales[i].fecha_validez.HasValue? dataMateriales[i].fecha_validez.Value.ToShortDateString():string.Empty,
                    };

            }
            //llena los valores para los materiales de cración con referencia
            for (int i = dataMateriales.Count; i < dataMateriales.Count + dataCreacion.Count(); i++)
            {
                int c = i - dataMateriales.Count;

                jsonData[i] = new[] {
                    dataCreacion[c].nuevo_material,
                    dataCreacion[c].SCDM_cat_tipo_materiales_solicitud!=null? dataCreacion[c].SCDM_cat_tipo_materiales_solicitud.descripcion:string.Empty,
                    dataCreacion[c].SCDM_cat_tipo_venta!=null? dataCreacion[c].SCDM_cat_tipo_venta.descripcion:string.Empty,
                    string.Empty, //dataMateriales[i].descripcion_material_es,
                    dataCreacion[c].numero_parte,
                    dataCreacion[c].peso_bruto.ToString(),
                    dataCreacion[c].peso_neto.ToString(),
                    !string.IsNullOrEmpty(dataCreacion[c].unidad_medida_inventario)?  dataCreacion[c].unidad_medida_inventario : string.Empty,
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
                    data[i].requiere_consiliacion_puntas_colar.HasValue? data[i].requiere_consiliacion_puntas_colar.Value?"SÍ":"NO":string.Empty,
                    data[i].scrap_permitido_puntas_colas.ToString(),
                    data[i].fecha_validez.HasValue?data[i].fecha_validez.Value.ToString("dd/MM/yyyy"):string.Empty,

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
            "Planicidad (mm)", "MSA (Honda)", "Req. conciliación Puntas y colas", "Scrap permitido (%)", "Fecha validez", "Disponente asignado"
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
            "Planicidad (mm)", "MSA (Honda)", "Req. conciliación Puntas y colas", "Scrap permitido (%)", "Fecha validez",
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
                    avance_mm = ancho_mm, //data[23]
                    avance_tolerancia_negativa_mm = ancho_tolerancia_negativa_mm, //data[24]
                    avance_tolerancia_positiva_mm = ancho_tolerancia_positiva_mm, //data[25]
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
                "ID", "Nuevo Material", "Material Existente", "Tipo de Material", "Planta", "Motivo de la creacion", "Selling Type (Budget)",
                "Núm. antigüo material", "Peso Bruto (KG)", "Peso Neto (KG)", "Unidad Base Medida", "Commodity", "Grado/Calidad",
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

                resultado.Add(new SCDM_solicitud_rel_creacion_referencia
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
                    numero_antiguo_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. antigüo material")]) ? array[Array.IndexOf(encabezados, "Núm. antigüo material")] : null,  //data[42]
                    peso_bruto = peso_bruto,
                    peso_neto = peso_neto,
                    unidad_medida_inventario = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Unidad Base Medida")]) ? array[Array.IndexOf(encabezados, "Unidad Base Medida")] : null, //data[11]
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

                });
            }

            return resultado;
        }
        private List<SCDM_solicitud_rel_cambio_ingenieria> ConvierteArrayACambiosIngenieria(List<string[]> data, int? id_solicitud)
        {
            List<SCDM_solicitud_rel_cambio_ingenieria> resultado = new List<SCDM_solicitud_rel_cambio_ingenieria> { };

            //variables globales para el metodo
            var BD_SCDM_planta = db.plantas.Where(x => x.activo == true).ToList();
            var BD_SCDM_cat_tipo_material = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList();

            //listado de encabezados
            string[] encabezados = {
                "ID",  "Material Existente", "Tipo de Material", "Planta",
                "Núm. antigüo material", "Peso Bruto (KG)", "Peso Neto (KG)", "Unidad Base Medida", "Commodity", "Grado/Calidad",
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
                int? id_planta = null, id_tipo_material = null;
                int id_creacion_referencia = 0;
                double? peso_bruto = null, peso_neto = null, espesor_mm = null, espesor_tolerancia_negativa_mm = null, espesor_tolerancia_positiva_mm = null,
                    ancho_mm = null, ancho_tolerancia_negativa_mm = null, ancho_tolerancia_positiva_mm = null,
                    avance_mm = null, avance_tolerancia_negativa_mm = null, avance_tolerancia_positiva_mm = null,
                    planicidad_mm = null, diametro_interior = null, diametro_exterior = null;

                #region Asignacion de variables
                //id_creacion
                if (int.TryParse(array[Array.IndexOf(encabezados, "ID")], out int id_cr))
                    id_creacion_referencia = id_cr;

                //planta
                var tempPlanta = array[Array.IndexOf(encabezados, "Planta")];
                if (tempPlanta != null && !string.IsNullOrEmpty(tempPlanta) && BD_SCDM_planta.Any(x => x.ConcatPlantaSap.Trim() == tempPlanta))
                    id_planta = BD_SCDM_planta.FirstOrDefault(x => x.ConcatPlantaSap.Trim() == tempPlanta).clave;


                //tipo material
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

                resultado.Add(new SCDM_solicitud_rel_cambio_ingenieria
                {
                    num_fila = data.IndexOf(array),
                    id_solicitud = id_solicitud.Value,
                    id = id_creacion_referencia,
                    material_existente = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Material Existente")]) ? array[Array.IndexOf(encabezados, "Material Existente")] : null,
                    id_tipo_material = id_tipo_material,
                    id_planta = id_planta,
                    numero_antiguo_material = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Núm. antigüo material")]) ? array[Array.IndexOf(encabezados, "Núm. antigüo material")] : null,  //data[42]
                    peso_bruto = peso_bruto,
                    peso_neto = peso_neto,
                    unidad_medida_inventario = !String.IsNullOrEmpty(array[Array.IndexOf(encabezados, "Unidad Base Medida")]) ? array[Array.IndexOf(encabezados, "Unidad Base Medida")] : null, //data[11]
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

                });
            }

            return resultado;
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
            "Email<br>(N/A para C&B)", "Requerimientos específicos", "Molino <br>(N/A para procesadores Ext)", "País origen material <br>(N/A para procesadores Ext)"
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
                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    !string.IsNullOrEmpty(data[i].nuevo_material)? data[i].nuevo_material:string.Empty,
                    !string.IsNullOrEmpty(data[i].material_existente)? data[i].material_existente:string.Empty,
                    data[i].SCDM_cat_tipo_materiales_solicitud !=null? data[i].SCDM_cat_tipo_materiales_solicitud.descripcion.Trim():string.Empty,
                    data[i].plantas !=null? data[i].plantas.ConcatPlantaSap.Trim():string.Empty,
                    !string.IsNullOrEmpty(data[i].motivo_creacion)? data[i].motivo_creacion:string.Empty,
                    data[i].SCDM_cat_tipo_venta !=null? data[i].SCDM_cat_tipo_venta.descripcion.Trim():string.Empty,
                    !String.IsNullOrEmpty(data[i].numero_antiguo_material)? data[i].numero_antiguo_material : string.Empty,
                    data[i].peso_bruto.ToString(),
                    data[i].peso_neto.ToString(),
                    !string.IsNullOrEmpty(data[i].unidad_medida_inventario)?  data[i].unidad_medida_inventario : string.Empty,
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
                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    !string.IsNullOrEmpty(data[i].material_existente)? data[i].material_existente:string.Empty,
                    data[i].SCDM_cat_tipo_materiales_solicitud !=null? data[i].SCDM_cat_tipo_materiales_solicitud.descripcion.Trim():string.Empty,
                    data[i].plantas !=null? data[i].plantas.ConcatPlantaSap.Trim():string.Empty,
                    !String.IsNullOrEmpty(data[i].numero_antiguo_material)? data[i].numero_antiguo_material : string.Empty,
                    data[i].peso_bruto.ToString(),
                    data[i].peso_neto.ToString(),
                    !string.IsNullOrEmpty(data[i].unidad_medida_inventario)?  data[i].unidad_medida_inventario : string.Empty,
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
                    };
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
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
                    !string.IsNullOrEmpty(data[i].mensaje_sap)? data[i].mensaje_sap:string.Empty,
                    };
            }
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

                //obtiene todos los posibles valores
                SCDM_solicitud_rel_item_material resultado = db.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.id_solicitud == id_solicitud && x.numero_material == tempResultado);
                SCDM_solicitud_rel_item_material componente = db.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.id_solicitud == id_solicitud && x.numero_material == tempComponente);

                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    !string.IsNullOrEmpty(data[i].resultado)? data[i].resultado:string.Empty,
                    resultado != null &&  resultado.SCDM_cat_tipo_materiales_solicitud != null  ? resultado.SCDM_cat_tipo_materiales_solicitud.descripcion : data[i].resultado.StartsWith("SM")? "Maquila":"--",
                    resultado != null &&  !string.IsNullOrEmpty(resultado.tipo_venta) ? resultado.tipo_venta : "--",
                    resultado != null && resultado.peso_bruto.HasValue ? resultado.peso_bruto.ToString() : "--",
                    resultado != null && resultado.peso_neto.HasValue ? resultado.peso_neto.ToString() : "--",
                    resultado != null &&  !string.IsNullOrEmpty(resultado.unidad_medida_inventario) ? resultado.unidad_medida_inventario : "--",
                    data[i].sobrante.HasValue ? data[i].sobrante.Value.ToString() : string.Empty,
                    !string.IsNullOrEmpty(data[i].componente)? data[i].componente:string.Empty,
                    componente != null &&  componente.SCDM_cat_tipo_materiales_solicitud != null  ? componente.SCDM_cat_tipo_materiales_solicitud.descripcion : "--",
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
                    data[i].pais_origen
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
            //inicializa la lista de objetos
            var objeto = new object[1];

            if (!numero_material.Contains("CreacionReferencia"))
            {
                //obtiene todos los posibles valores
                SCDM_solicitud_rel_item_material item = db.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.id_solicitud == id_solicitud && x.numero_material == numero_material);

                //inicializa objeto principal
                if (item == null)
                {
                    item = new SCDM_solicitud_rel_item_material();
                }

                objeto[0] = new
                {
                    tipo_material = item.SCDM_cat_tipo_materiales_solicitud != null ? item.SCDM_cat_tipo_materiales_solicitud.descripcion : numero_material.StartsWith("SM") ? "Maquila" : "--",
                    peso_bruto_platina = item.peso_bruto.HasValue ? item.peso_bruto.ToString() : "--",
                    peso_neto_platina = item.peso_neto.HasValue ? item.peso_neto.ToString() : "--",
                    unidad_medida = !string.IsNullOrEmpty(item.unidad_medida_inventario) ? item.unidad_medida_inventario : "--",
                    SCDM_cat_tipo_venta = !string.IsNullOrEmpty(item.tipo_venta) ? item.tipo_venta : "--",
                };
            }
            else
            { //es creación con referencia
                SCDM_solicitud_rel_creacion_referencia item = db.SCDM_solicitud_rel_creacion_referencia.FirstOrDefault(x => x.id_solicitud == id_solicitud && x.nuevo_material == numero_material);

                //inicializa objeto principal
                if (item == null)
                {
                    item = new SCDM_solicitud_rel_creacion_referencia();
                }

                objeto[0] = new
                {
                    tipo_material = item.SCDM_cat_tipo_materiales_solicitud != null ? item.SCDM_cat_tipo_materiales_solicitud.descripcion : numero_material.StartsWith("SM") ? "Maquila" : "--",
                    peso_bruto_platina = item.peso_bruto.HasValue ? item.peso_bruto.ToString() : "--",
                    peso_neto_platina = item.peso_neto.HasValue ? item.peso_neto.ToString() : "--",
                    unidad_medida = !string.IsNullOrEmpty(item.unidad_medida_inventario) ? item.unidad_medida_inventario : "--",
                    SCDM_cat_tipo_venta = item.SCDM_cat_tipo_venta != null ? item.SCDM_cat_tipo_venta.descripcion : "--"
                };


            }


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
                envioCorreo.SendEmailAsync(correos, "Se cerró la actividad de la solicitud " + asignacion.id_solicitud, "El usuario " + usuarioLogeado.ConcatNombre + " ha cerrado la actividad a tu nombre para la solicitud " + asignacion.id);


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


    }
}
