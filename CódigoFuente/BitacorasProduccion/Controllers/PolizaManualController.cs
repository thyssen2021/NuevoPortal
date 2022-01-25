using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class PolizaManualController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();


        #region ViewsCapturista
        // GET: PolizaManual/CaptutistaCreadas
        public ActionResult CapturistaCreadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.poliza_manual
                    .Where(x => x.id_elaborador == empleado.id && x.estatus == PM_Status.CREADO )
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => x.id_elaborador == empleado.id && x.estatus == PM_Status.CREADO)
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones
                ViewBag.Edit = true;
                ViewBag.Details = true;
                ViewBag.EnviarValidacion = true;
                ViewBag.Title = "Listado Pólizas Creadas";
                ViewBag.SegundoNivel = "PM_registro";
                ViewBag.Create = true;

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: PolizaManual/CapturistaPendientes
        public ActionResult CapturistaPendientes(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.poliza_manual
                    .Where(x => x.id_elaborador == empleado.id 
                    && (x.estatus == PM_Status.ENVIADO_A_AREA || x.estatus == PM_Status.ENVIADO_A_CONTABILIDAD || x.estatus == PM_Status.ENVIADO_SEGUNDA_VALIDACION || x.estatus == PM_Status.AUTORIZADO_SEGUNDA_VALIDACION || x.estatus==PM_Status.VALIDADO_POR_AREA
                    || x.estatus == PM_Status.AUTORIZADO_SEGUNDA_VALIDACION) )
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => x.id_elaborador == empleado.id
                    && (x.estatus == PM_Status.ENVIADO_A_AREA || x.estatus == PM_Status.ENVIADO_A_CONTABILIDAD || x.estatus == PM_Status.ENVIADO_SEGUNDA_VALIDACION || x.estatus == PM_Status.AUTORIZADO_SEGUNDA_VALIDACION || x.estatus == PM_Status.VALIDADO_POR_AREA
                    || x.estatus == PM_Status.AUTORIZADO_SEGUNDA_VALIDACION))
                   .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones
                ViewBag.Details = true;
                ViewBag.Title = "Listado Pólizas Pendientes";
                ViewBag.SegundoNivel = "PM_pendientes";
                ViewBag.Create = true;

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: PolizaManual/CapturistaRechazadas
        public ActionResult CapturistaRechazadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.poliza_manual
                    .Where(x => x.id_elaborador == empleado.id && (x.estatus == PM_Status.RECHAZADO_VALIDADOR || x.estatus == PM_Status.RECHAZADO_AUTORIZADOR) )
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => x.id_elaborador == empleado.id && (x.estatus == PM_Status.RECHAZADO_VALIDADOR || x.estatus == PM_Status.RECHAZADO_AUTORIZADOR))
                    .Count();
                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones
                ViewBag.Edit = true;
                ViewBag.Details = true;
                ViewBag.EnviarValidacion = true;
                ViewBag.Title = "Listado Pólizas Rechazadas";
                ViewBag.SegundoNivel = "PM_rechazadas";
                ViewBag.Create = true;

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: PolizaManual/CapturistaFinalizadas
        public ActionResult CapturistaFinalizadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.poliza_manual
                    .Where(x => x.id_elaborador == empleado.id && x.estatus == PM_Status.FINALIZADO)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => x.id_elaborador == empleado.id && x.estatus == PM_Status.FINALIZADO)
                 .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones
               
                ViewBag.Details = true;
                ViewBag.Title = "Listado Pólizas Finalizadas";
                ViewBag.SegundoNivel = "PM_finalizadas";
                ViewBag.Create = true;

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        #endregion

        #region viewsValidador
        // GET: PolizaManual/ValidadorPendientes
        public ActionResult ValidadorPendientes(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_VALIDAR_POR_AREA))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //busca el validador por el id de empleado
                //PM_validadores validador = db.PM_validadores.FirstOrDefault(x => x.id_empleado == empleado.id);                
                //int idValidador = validador == null? 0: validador.id;
                
                var listado = db.poliza_manual
                    .Where(x =>  x.estatus == PM_Status.ENVIADO_A_AREA && x.id_validador==empleado.id)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => x.estatus == PM_Status.ENVIADO_A_AREA && x.id_validador == empleado.id)
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones

                ViewBag.Details = true;
                ViewBag.Validar = true;
                ViewBag.Title = "Listado Pólizas Pendientes";
                ViewBag.SegundoNivel = "PM_validar_area_pendientes";                

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }


        public ActionResult ValidadorRechazadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_VALIDAR_POR_AREA))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //busca el validador por el id de empleado
                //PM_validadores validador = db.PM_validadores.FirstOrDefault(x => x.id_empleado == empleado.id);
                //int idValidador = validador == null ? 0 : validador.id;

                var listado = db.poliza_manual
                    .Where(x => (x.estatus == PM_Status.RECHAZADO_VALIDADOR || x.estatus == PM_Status.RECHAZADO_AUTORIZADOR) && x.id_validador == empleado.id)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => (x.estatus == PM_Status.RECHAZADO_VALIDADOR || x.estatus == PM_Status.RECHAZADO_AUTORIZADOR) && x.id_validador == empleado.id)
                    .Count();
                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones

                ViewBag.Details = true;
                ViewBag.Title = "Listado Pólizas Rechazadas";
                ViewBag.SegundoNivel = "PM_validar_area_rechazadas";

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        public ActionResult ValidadorEnviadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_VALIDAR_POR_AREA))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //busca el validador por el id de empleado
                //PM_validadores validador = db.PM_validadores.FirstOrDefault(x => x.id_empleado == empleado.id);
                //int idValidador = validador == null ? 0 : validador.id;

                var listado = db.poliza_manual
                    .Where(x => (x.estatus == PM_Status.ENVIADO_SEGUNDA_VALIDACION || x.estatus == PM_Status.ENVIADO_A_CONTABILIDAD)  && x.id_validador == empleado.id)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => (x.estatus == PM_Status.ENVIADO_SEGUNDA_VALIDACION || x.estatus == PM_Status.ENVIADO_A_CONTABILIDAD) && x.id_validador == empleado.id)
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones

                ViewBag.Details = true;
                ViewBag.Title = "Listado Pólizas Enviadas";
                ViewBag.SegundoNivel = "PM_validar_area_enviadas";

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }


        public ActionResult ValidadorFinalizadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_VALIDAR_POR_AREA))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //busca el validador por el id de empleado
                //PM_validadores validador = db.PM_validadores.FirstOrDefault(x => x.id_empleado == empleado.id);
                //int idValidador = validador == null ? 0 : validador.id;

                var listado = db.poliza_manual
                    .Where(x => (x.estatus == PM_Status.FINALIZADO ) && x.id_validador == empleado.id)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => (x.estatus == PM_Status.FINALIZADO) && x.id_validador == empleado.id)
                    .Count();
                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones

                ViewBag.Details = true;
                ViewBag.Title = "Listado Pólizas Finalizadas";
                ViewBag.SegundoNivel = "PM_validar_area_finalizadas";

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        #endregion

        #region ViewsAutorizadorControlling

        // GET: PolizaManual/AutorizadorPendientes
        public ActionResult AutorizadorPendientes(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_AUTORIZAR_CONTROLLING))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.poliza_manual
                    .Where(x => x.estatus == PM_Status.ENVIADO_SEGUNDA_VALIDACION && x.id_autorizador == empleado.id)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => x.estatus == PM_Status.ENVIADO_SEGUNDA_VALIDACION && x.id_autorizador == empleado.id)
                    .Count();
                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones

                ViewBag.Details = true;
                ViewBag.Autorizar = true;
                ViewBag.Title = "Listado Pólizas Pendientes";
                ViewBag.SegundoNivel = "PM_autorizar_controlling_pendientes";

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        public ActionResult AutorizadorRechazadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_AUTORIZAR_CONTROLLING))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();                             

                var listado = db.poliza_manual
                    .Where(x => x.estatus == PM_Status.RECHAZADO_AUTORIZADOR && x.id_autorizador.HasValue && x.id_autorizador == empleado.id)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => x.estatus == PM_Status.RECHAZADO_AUTORIZADOR && x.id_autorizador.HasValue && x.id_autorizador == empleado.id)
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones

                ViewBag.Details = true;
                ViewBag.Title = "Listado Pólizas Rechazadas";
                ViewBag.SegundoNivel = "PM_autorizar_controlling_rechazadas";

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        public ActionResult AutorizadorEnviadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_AUTORIZAR_CONTROLLING))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.poliza_manual
                    .Where(x => x.estatus == PM_Status.ENVIADO_A_CONTABILIDAD && x.id_autorizador == empleado.id)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => x.estatus == PM_Status.ENVIADO_A_CONTABILIDAD && x.id_autorizador == empleado.id)
                    .Count();
                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones

                ViewBag.Details = true;
                ViewBag.Title = "Listado Pólizas Enviadas";
                ViewBag.SegundoNivel = "PM_autorizar_controlling_enviadas";

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        public ActionResult AutorizadorRegistradas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_AUTORIZAR_CONTROLLING))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();                           

                var listado = db.poliza_manual
                    .Where(x => x.estatus == PM_Status.FINALIZADO && x.id_autorizador == empleado.id)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => x.estatus == PM_Status.FINALIZADO && x.id_autorizador == empleado.id)
                    .Count();
                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones

                ViewBag.Details = true;
                ViewBag.Title = "Listado Pólizas Finalizadas";
                ViewBag.SegundoNivel = "PM_autorizar_controlling_registradas";

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }


        #endregion

        #region ViewsContabilidad
        public ActionResult ContabilidadPendientes(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_CONTABILIDAD))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listado = db.poliza_manual
                    .Where(x => x.estatus == PM_Status.ENVIADO_A_CONTABILIDAD)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => x.estatus == PM_Status.ENVIADO_A_CONTABILIDAD)
                    .Count();
                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones

                ViewBag.Details = true;
                ViewBag.Finalizar = true;
                ViewBag.Title = "Listado Pólizas Pendientes";
                ViewBag.SegundoNivel = "PM_contabilidad_pendientes";

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }
        public ActionResult ContabilidadRegistradas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_CONTABILIDAD))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listado = db.poliza_manual
                     .Where(x => x.estatus == PM_Status.FINALIZADO)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Where(x => x.estatus == PM_Status.FINALIZADO)
                    .Count();
                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones

                ViewBag.Details = true;
                ViewBag.Title = "Listado Pólizas Registradas";
                ViewBag.SegundoNivel = "PM_contabilidad_registradas";

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        #endregion


        #region Envio Validacion
        // GET: PolizaManual/EnviarParaValidacion/5
        public ActionResult EnviarParaValidacion(int? id)
        {
            if (TieneRol(TipoRoles.PM_REGISTRO) )
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                poliza_manual poliza_manual = db.poliza_manual.Find(id);
                if (poliza_manual == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si se puede enviar para validacion
                if (poliza_manual.estatus != PM_Status.CREADO && poliza_manual.estatus != PM_Status.RECHAZADO_VALIDADOR && poliza_manual.estatus != PM_Status.RECHAZADO_AUTORIZADOR)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede enviar esta Póliza!";
                    ViewBag.Descripcion = "No se puede enviar una póliza que ya ha sido enviada, aprobada o finalizada.";

                    return View("../Home/ErrorGenerico");
                }
                return View(poliza_manual);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: PremiumFreightAproval/Send/5
        [HttpPost, ActionName("EnviarParaValidacion")]
        [ValidateAntiForgeryToken]
        public ActionResult EnviarParaValidacionConfirmed(int id)
        {
            poliza_manual pm = db.poliza_manual.Find(id);
            string estatusAnterior = pm.estatus;
            pm.estatus = PM_Status.ENVIADO_A_AREA;
            pm.fecha_validacion = null;

            db.Entry(pm).State = EntityState.Modified;
            try
            {
                db.SaveChanges();


                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO
                correos.Add("alfredo.xochitemol@lagermex.com.mx");

                //if (!String.IsNullOrEmpty(pm.PM_validadores.empleados.correo))
                //    correos.Add(pm.PM_validadores.empleados.correo); //agrega correo de autorizador

                envioCorreo.SendEmailAsync(correos, "Ha recibido una Póliza Manual para su aprobación.", envioCorreo.getBodyPMSendValidador(pm));
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                if (estatusAnterior == PM_Status.RECHAZADO_VALIDADOR || estatusAnterior == PM_Status.RECHAZADO_AUTORIZADOR)
                    return RedirectToAction("CapturistaRechazadas");
                return RedirectToAction("CapturistaCreadas");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("CapturistaCreadas");
            }

            TempData["Mensaje"] = new MensajesSweetAlert("La Póliza Manual ha sido enviada", TipoMensajesSweetAlerts.SUCCESS);

            if (estatusAnterior == PM_Status.RECHAZADO_VALIDADOR)
                return RedirectToAction("CapturistaRechazadas");

            return RedirectToAction("CapturistaCreadas");
        }

        #endregion

        #region validar_rechazar_area

        // GET: PolizaManual/ValidarArea/5
        public ActionResult ValidarArea(int? id)
        {
            if (TieneRol(TipoRoles.PM_VALIDAR_POR_AREA))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                poliza_manual poliza_manual = db.poliza_manual.Find(id);
                if (poliza_manual == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si se puede editar
                if (poliza_manual.estatus != PM_Status.ENVIADO_A_AREA)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede validar esta Póliza!";
                    ViewBag.Descripcion = "No se puede validar una póliza que ha sido validada, rechazada o finalizada.";

                    return View("../Home/ErrorGenerico");
                }

                return View(poliza_manual);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }


        // POST: PremiumFreightAproval/Rechazar/5
        [HttpPost, ActionName("RechazarAreaPM")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarAreaPMConfirmed(FormCollection collection)
        {

            int id = 0;
            if (!String.IsNullOrEmpty(collection["id"]))
                Int32.TryParse(collection["id"], out id);

            String razonRechazo = collection["comentario_rechazo"];


            poliza_manual poliza = db.poliza_manual.Find(id);
            poliza.estatus = PM_Status.RECHAZADO_VALIDADOR;
            poliza.comentario_rechazo = razonRechazo;
            poliza.fecha_validacion = null; //borra la fecha de validación de área si existiera

            db.Entry(poliza).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                correos.Add("alfredo.xochitemol@lagermex.com.mx");

                //if (!String.IsNullOrEmpty(poliza.empleados.correo))
                 //   correos.Add(poliza.empleados.correo);

                envioCorreo.SendEmailAsync(correos, "La Póliza Manual #" + poliza.id + " ha sido Rechazada.", envioCorreo.getBodyPMRechazada(poliza, poliza.empleados3.ConcatNombre));

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("ValidadorPendientes");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("ValidadorPendientes");
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La Poliza ha sido rechazada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("ValidadorPendientes");
        }

        // POST: PremiumFreightAproval/ValidarAreaPM/5
        [HttpPost, ActionName("ValidarAreaPM")]
        [ValidateAntiForgeryToken]
        public ActionResult ValidarAreaPMConfirmed(FormCollection collection)
        {

            int id = 0;
            if (!String.IsNullOrEmpty(collection["id"]))
                Int32.TryParse(collection["id"], out id);


            poliza_manual poliza = db.poliza_manual.Find(id);
            poliza.estatus = PM_Status.ENVIADO_SEGUNDA_VALIDACION;
            poliza.fecha_validacion= DateTime.Now;
           // poliza.id_autorizador = Convert.ToInt32(collection["id_autorizador"]);

            db.Entry(poliza).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                correos.Add("alfredo.xochitemol@lagermex.com.mx");

                // *** LEER EMAIL DE AUTORIZADOR ******
               // if (!String.IsNullOrEmpty(poliza.empleados.correo))
               //     correos.Add(poliza.empleados.correo);

                envioCorreo.SendEmailAsync(correos, "La Poliza Manual #"+poliza.id+" ha sido válidada por el área.", envioCorreo.getBodyPMValidadoPorArea(poliza));

                /* ANALIZAR SI NECESARIO MANDAR CORREO TANTO AL AUTORIZADOR COMO AL USUARIO */
                correos = new List<string>();
                correos.Add("alfredo.xochitemol@lagermex.com.mx");
                envioCorreo.SendEmailAsync(correos, "Ha recibido una póliza manual para su autorización.", envioCorreo.getBodyPMSendAutorizador(poliza));

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("ValidadorPendientes");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("ValidadorPendientes");
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La póliza ha sido validada y enviada para su autorización.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("ValidadorPendientes");
        }

        #endregion

        #region autorizar_rechazar_controlling

        // GET: PolizaManual/AutorizarControlling/5
        public ActionResult AutorizarControlling(int? id)
        {
            if (TieneRol(TipoRoles.PM_AUTORIZAR_CONTROLLING))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                poliza_manual poliza_manual = db.poliza_manual.Find(id);
                if (poliza_manual == null)
                {
                    return View("../Error/NotFound");
                }
             
                //verifica si se puede autorizar
                if (poliza_manual.estatus != PM_Status.ENVIADO_SEGUNDA_VALIDACION)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede validar esta Póliza!";
                    ViewBag.Descripcion = "No se puede validar una póliza que ya ha sido validada, rechazada o finalizada.";

                    return View("../Home/ErrorGenerico");
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                ViewBag.Empleado = empleado;

                return View(poliza_manual);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: PremiumFreightAproval/Rechazar/5
        [HttpPost, ActionName("RechazarControlling")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarControllingConfirmed(FormCollection collection)
        {

            int id = 0;
            if (!String.IsNullOrEmpty(collection["id"]))
                Int32.TryParse(collection["id"], out id);

            String razonRechazo = collection["comentario_rechazo"];


            poliza_manual poliza = db.poliza_manual.Find(id);
            poliza.estatus = PM_Status.RECHAZADO_AUTORIZADOR;
            poliza.comentario_rechazo = razonRechazo;
            //poliza.fecha_validacion = null; //borra la fecha de validación de área
            poliza.fecha_autorizacion = null; //borra la fecha de autorización si existiera  
            
            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //asigna el id de empleado del autorizador
            if (empleado.id > 0)
                poliza.id_autorizador = empleado.id;


            db.Entry(poliza).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                correos.Add("alfredo.xochitemol@lagermex.com.mx");

                //if (!String.IsNullOrEmpty(poliza.empleados.correo))
                //   correos.Add(poliza.empleados.correo);

                envioCorreo.SendEmailAsync(correos, "La Póliza Manual #" + poliza.id + " ha sido Rechazada.", envioCorreo.getBodyPMRechazada(poliza, empleado.ConcatNombre));

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("AutorizadorPendientes");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("AutorizadorPendientes");
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La Poliza ha sido rechazada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("AutorizadorPendientes");
        }

        // POST: PremiumFreightAproval/ValidarAreaPM/5
        [HttpPost, ActionName("AutorizarControlling")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AutorizarControllingConfirmed(FormCollection collection)
        {

            int id = 0;
            if (!String.IsNullOrEmpty(collection["id"]))
                Int32.TryParse(collection["id"], out id);            

            poliza_manual poliza = db.poliza_manual.Find(id);
            poliza.estatus = PM_Status.ENVIADO_A_CONTABILIDAD;
            poliza.fecha_autorizacion = DateTime.Now;

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //asigna el id de empleado del autorizador
            if (empleado.id > 0)
                poliza.id_autorizador = empleado.id;
            

            db.Entry(poliza).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                correos.Add("alfredo.xochitemol@lagermex.com.mx");

                // *** LEER EMAIL DE ELABORADOR ******
                // if (!String.IsNullOrEmpty(poliza.empleados.correo))
                //     correos.Add(poliza.empleados.correo);

                envioCorreo.SendEmailAsync(correos, "La Poliza Manual #" + poliza.id + " ha sido autorizada por Controlling.", envioCorreo.getBodyPMValidadoPorControlling(poliza));

              
                List<IdentitySample.Models.ApplicationUser> usuarios = await _userManager.Users.ToListAsync();

                usuarios = usuarios.Where(x => _userManager.IsInRoleAsync(x.Id, TipoRoles.PM_CONTABILIDAD).Result == true).ToList();

                correos = new List<string>();
                correos.Add("alfredo.xochitemol@lagermex.com.mx");

                foreach (var item in usuarios)
                {
                    //PM_usuarios_capturistas user = null;
                    ////busca el empleado
                    //empleados emp = db.empleados.Find(item.IdEmpleado);

                    ////busca el usuario capturista
                    //if (emp != null)
                    //     user = db.PM_usuarios_capturistas.FirstOrDefault(x => x.id_empleado == emp.id);

                    ////si existe el usuario capturista y además esta activo
                    //if(user!=null && user.activo==true)
                    //    correos.Add(item.Email);
                }

                envioCorreo.SendEmailAsync(correos, "La Poliza Manual #" + poliza.id + " está en espera de ser registrada en SAP.", envioCorreo.getBodyPMSendContabilidad(poliza));

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("AutorizadorPendientes");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("AutorizadorPendientes");
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La póliza ha sido autorizada.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("AutorizadorPendientes");
        }

        #endregion

        #region contabilidad finalizar

        public ActionResult FinalizarContabilidad(int? id)
        {
            if (TieneRol(TipoRoles.PM_CONTABILIDAD))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                poliza_manual poliza_manual = db.poliza_manual.Find(id);
                if (poliza_manual == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si se puede autorizar
                if (poliza_manual.estatus != PM_Status.ENVIADO_A_CONTABILIDAD)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede modificar esta Póliza!";
                    ViewBag.Descripcion = "No se puede modificar una poliza que haya sido rechazada o finalizada.";

                    return View("../Home/ErrorGenerico");
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                ViewBag.Empleado = empleado;

                return View(poliza_manual);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: PolizaManual/FinalizarContabilidad/5
        [HttpPost, ActionName("FinalizarContabilidad")]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizarContabilidadConfirmed(poliza_manual poliza_manual)
        {

            poliza_manual poliza = db.poliza_manual.Find(poliza_manual.id);
            poliza.estatus = PM_Status.FINALIZADO;

            //pone la fecha actual
            poliza.fecha_registro = DateTime.Now; 

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //asigna el id de empleado del autorizador
            if (empleado.id > 0)
                poliza.id_contabilidad = empleado.id;

            poliza.numero_documento_sap = poliza_manual.numero_documento_sap;

            //verifica si el archivo es válido
            if (poliza_manual.PostedFileRegistro != null && poliza_manual.PostedFileRegistro.InputStream.Length > 5242880)
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 5MB.");
            else if (poliza_manual.PostedFileRegistro != null)
            { //verifica la extensión del archivo
                string extension = Path.GetExtension(poliza_manual.PostedFileRegistro.FileName);
                if (extension.ToUpper() != ".XLS"   //si no contiene una extensión válida
                               && extension.ToUpper() != ".XLSX"
                               && extension.ToUpper() != ".DOC"
                               && extension.ToUpper() != ".DOCX"
                               && extension.ToUpper() != ".PDF"
                               && extension.ToUpper() != ".PNG"
                               && extension.ToUpper() != ".JPG"
                               && extension.ToUpper() != ".JPEG"
                               && extension.ToUpper() != ".RAR"
                               && extension.ToUpper() != ".ZIP"
                               )
                    ModelState.AddModelError("", "Sólo se permiten archivos con extensión .xls, .xlsx, .doc, .docx, .pdf, .png, .jpg, .jpeg, .rar, .zip.");
                else
                { //si la extension y el tamaño son válidos
                    String nombreArchivo = poliza_manual.PostedFileRegistro.FileName;

                    //recorta el nombre del archivo en caso de ser necesario
                    if (nombreArchivo.Length > 80)
                        nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                    //lee el archivo en un arreglo de bytes
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(poliza_manual.PostedFileRegistro.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(poliza_manual.PostedFileRegistro.ContentLength);
                    }

                    //genera el archivo de biblioce digital
                    biblioteca_digital archivo = new biblioteca_digital
                    {
                        Nombre = nombreArchivo,
                        MimeType = UsoStrings.RecortaString(poliza_manual.PostedFileRegistro.ContentType, 80),
                        Datos = fileData
                    };

                    //update en BD
                    db.biblioteca_digital.Add(archivo);
                    db.SaveChanges();

                    //relaciona el archivo con la poliza (despues se guarda en BD)
                    poliza.id_documento_registro = archivo.Id;  //documento soporte

                    //!!DEBERIA ELIMINAR ARCHIVO SI YA EXISTE?

                }
            }

            db.Entry(poliza).State = EntityState.Modified;
            //en caso de que el modelo sea válido guarda en BD
            if (ModelState.IsValid)
            {
                try
                {
                    db.SaveChanges();

                    //envia correo electronico
                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                    List<String> correos = new List<string>(); //correos TO

                    correos.Add("alfredo.xochitemol@lagermex.com.mx");

                    //AGREGAR cORREOS A TODOS LOS INVOLUNCRADOS

                    envioCorreo.SendEmailAsync(correos, "La Póliza Manual #" + poliza.id + " ha sido registrada correctamente.", envioCorreo.getBodyPMRegistradoPorContabilidad(poliza));

                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                    TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                    return RedirectToAction("ContabilidadPendientes");

                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("ContabilidadPendientes");
                }
            }
            else {
                //en caso de que el modelo no sea válido
                ViewBag.Empleado = empleado;
                return View(db.poliza_manual.Find(poliza_manual.id));
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La Poliza ha sido registrada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("ContabilidadPendientes");
        }

        #endregion



        // GET: PolizaManual/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.PM_AUTORIZAR_CONTROLLING) || TieneRol(TipoRoles.PM_VALIDAR_POR_AREA)
              || TieneRol(TipoRoles.PM_CONTABILIDAD) || TieneRol(TipoRoles.PM_REGISTRO))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                poliza_manual poliza_manual = db.poliza_manual.Find(id);
                if (poliza_manual == null)
                {
                    return View("../Error/NotFound");
                }
                return View(poliza_manual);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }
        // GET: PolizaManual/Create
        public ActionResult Create()
        {

            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();
                ViewBag.Solicitante = empleado;

                //obtiene el usuario capturista
                PM_usuarios_capturistas user = db.PM_usuarios_capturistas.Where(x => x.id_empleado == empleado.id && x.activo==true).FirstOrDefault();

                //verifica que el usuario este registrado como capturista
                if (user == null) {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede acceder a esta Sección!";
                    ViewBag.Descripcion = "Este usuario no se encuentra registrado como capturista, favor de contactar al Administrador del Sitio para solicitar el permiso.";

                    return View("../Home/ErrorGenerico");
                }


                ViewBag.currency_iso = AddFirstItem(new SelectList(db.currency.Where(x => x.activo), "CurrencyISO", "CocatCurrency"), selected: "USD");
                ViewBag.id_PM_tipo_poliza = AddFirstItem(new SelectList(db.PM_tipo_poliza.Where(x => x.activo), "id", "descripcion"));
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"));                
                ViewBag.id_validador = AddFirstItem(new SelectList(db.PM_departamentos.Where(x => x.id==user.id_departamento), "id_empleado_jefe", "empleados.ConcatNombre"),selected:user.PM_departamentos.id_empleado_jefe.ToString());

                poliza_manual poliza_model = new poliza_manual();

                //aqui hacer consulta para obtener el modelo de poliza
                List<PM_conceptos> conceptos = new List<PM_conceptos>();

                conceptos.Add(new PM_conceptos { cuenta ="700800", cc="4100", concepto= "INC PROV SCRAP - ALLGAIER" });
                conceptos.Add(new PM_conceptos { cuenta = "211220",  concepto = "INC PROV SCRAP - ALLGAIER" });
                conceptos.Add(new PM_conceptos { cuenta = "700800", cc = "6100", concepto = "INC PROV SCRAP - ARCELOR / IN KOTE" });
                conceptos.Add(new PM_conceptos { cuenta = "211220", concepto = "INC PROV SCRAP - ARCELOR / IN KOTE" });
                conceptos.Add(new PM_conceptos { cuenta = "700800", cc = "6100", concepto = "INC PROV SCRAP - ARCELOR / INDIANA HARBOR" });
                conceptos.Add(new PM_conceptos { cuenta = "211220", concepto = "INC PROV SCRAP - ARCELOR / INDIANA HARBOR" });
                conceptos.Add(new PM_conceptos { cuenta = "700800", cc = "6100", concepto = "INC PROV SCRAP - AUTOTEK" });
                conceptos.Add(new PM_conceptos { cuenta = "211220", concepto = "INC PROV SCRAP - AUTOTEK" });
                conceptos.Add(new PM_conceptos { cuenta = "700800", cc = "4100", concepto = "INC PROV SCRAP - AUTOTEK" });
                conceptos.Add(new PM_conceptos { cuenta = "211220", concepto = "INC PROV SCRAP - AUTOTEK" });
                conceptos.Add(new PM_conceptos { cuenta = "700800", cc = "4100", concepto = "INC PROV SCRAP - BENMEX" });
                conceptos.Add(new PM_conceptos { cuenta = "211220", concepto = "INC PROV SCRAP - BENMEX" });
                conceptos.Add(new PM_conceptos { cuenta = "700800", cc = "6100", concepto = "INC PROV SCRAP - CALVERT" });
                conceptos.Add(new PM_conceptos { cuenta = "211220", concepto = "INC PROV SCRAP - CALVERT" });
                conceptos.Add(new PM_conceptos { cuenta = "700800", cc = "4100", concepto = "INC PROV SCRAP - FNG" });
                conceptos.Add(new PM_conceptos { cuenta = "211220", concepto = "INC PROV SCRAP - FNG" });
                conceptos.Add(new PM_conceptos { cuenta = "700800", cc = "4100", concepto = "INC PROV SCRAP - GENI" });
                conceptos.Add(new PM_conceptos { cuenta = "211220", concepto = "INC PROV SCRAP - GENI" });
                conceptos.Add(new PM_conceptos { cuenta = "700800", cc = "6100", concepto = "INC PROV SCRAP - GESTAMP AGUASCALIENTES" });
                conceptos.Add(new PM_conceptos { cuenta = "211220", concepto = "INC PROV SCRAP - GESTAMP AGUASCALIENTES" });


                poliza_model.PM_conceptos = conceptos;

                return View(poliza_model);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }



        }

        // POST: PolizaManual/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(poliza_manual poliza_manual)
        {

            //determina si hay conceptos
            if (poliza_manual.PM_conceptos.Count == 0)
                ModelState.AddModelError("", "No se ingresaron conceptos para la póliza.");

            decimal debe = 0;
            decimal haber = 0;
            bool existenConceptosVacios = false;

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();
            ViewBag.Solicitante = empleado;

            //obtiene el usuario capturista
            PM_usuarios_capturistas user = db.PM_usuarios_capturistas.Where(x => x.id_empleado == empleado.id && x.activo == true).FirstOrDefault();

            //verifica que el usuario este registrado como capturista
            if (user == null)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede acceder a esta Sección!";
                ViewBag.Descripcion = "Este usuario no se encuentra registrado como capturista, favor de contactar al Administrador del Sitio para solicitar el permiso.";

                return View("../Home/ErrorGenerico");
            }

            //determina si las sumas son iguales
            foreach (PM_conceptos item in poliza_manual.PM_conceptos)
            {
                //suma el valor de debe
                if (item.debe.HasValue)
                    debe += item.debe.Value;
                //suma el valor de haber
                if (item.haber.HasValue)
                    haber += item.haber.Value;

                //verifica si extisten conceptos vacios
                if (!item.debe.HasValue && !item.haber.HasValue)
                    existenConceptosVacios = true;
            }

            //agrega error si los campos estan vacíos
            if (existenConceptosVacios)
                ModelState.AddModelError("", "Verifique que todos los conceptos tengan al menos un valor asignado.");

            //verifica si las sumas son diferentes
            if (debe != haber)
                ModelState.AddModelError("", "Las sumas no son iguales.");

            //verifica si el tamaño del archivo es válido
            if (poliza_manual.PostedFileSoporte != null && poliza_manual.PostedFileSoporte.InputStream.Length > 10485760)
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 10MB.");
            else if (poliza_manual.PostedFileSoporte != null)
            { //verifica la extensión del archivo
                string extension = Path.GetExtension(poliza_manual.PostedFileSoporte.FileName);
                if (extension.ToUpper() != ".XLS"   //si no contiene una extensión válida
                               && extension.ToUpper() != ".XLSX"
                               && extension.ToUpper() != ".DOC"
                               && extension.ToUpper() != ".DOCX"
                               && extension.ToUpper() != ".PDF"
                               && extension.ToUpper() != ".PNG"
                               && extension.ToUpper() != ".JPG"
                               && extension.ToUpper() != ".JPEG"
                               && extension.ToUpper() != ".RAR"
                               && extension.ToUpper() != ".ZIP"
                               && extension.ToUpper() != ".EML"
                               )
                    ModelState.AddModelError("", "Sólo se permiten archivos con extensión .xls, .xlsx, .doc, .docx, .pdf, .png, .jpg, .jpeg, .rar, .zip., y .eml.");
                else { //si la extension y el tamaño son válidos
                    String nombreArchivo = poliza_manual.PostedFileSoporte.FileName;

                    //recorta el nombre del archivo en caso de ser necesario
                    if (nombreArchivo.Length > 80)                    
                        nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                    //lee el archivo en un arreglo de bytes
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(poliza_manual.PostedFileSoporte.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(poliza_manual.PostedFileSoporte.ContentLength);
                    }

                    //genera el archivo de biblioce digital
                    biblioteca_digital archivo = new biblioteca_digital
                    {
                        Nombre = nombreArchivo,
                        MimeType = UsoStrings.RecortaString(poliza_manual.PostedFileSoporte.ContentType, 80),
                        Datos = fileData
                    };

                    //relaciona el archivo con la poliza (despues se guarda en BD)
                    poliza_manual.biblioteca_digital1 = archivo;  //documento soporte

                }
            }

            if (ModelState.IsValid)
            {
               
                poliza_manual.fecha_creacion = DateTime.Now;
                poliza_manual.estatus = PM_Status.CREADO;
                //agrega el id del departamento que autoriza
                poliza_manual.id_autorizador = user.PM_departamentos.PM_departamentos2.id_empleado_jefe;


                db.poliza_manual.Add(poliza_manual);

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);


                db.SaveChanges();
                return RedirectToAction("CapturistaCreadas");
            }

            

            ViewBag.currency_iso = AddFirstItem(new SelectList(db.currency.Where(x => x.activo), "CurrencyISO", "CocatCurrency"));
            ViewBag.id_PM_tipo_poliza = AddFirstItem(new SelectList(db.PM_tipo_poliza.Where(x => x.activo), "id", "descripcion"));
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"));
            ViewBag.id_validador = AddFirstItem(new SelectList(db.PM_departamentos.Where(x => x.id == user.id_departamento), "id_empleado_jefe", "empleados.ConcatNombre"), selected: user.PM_departamentos.id_empleado_jefe.ToString());

            return View(poliza_manual);
        }

        // GET: PolizaManual/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.PFA_REGISTRO))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                poliza_manual poliza_manual = db.poliza_manual.Find(id);
                if (poliza_manual == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si se puede editar
                if (poliza_manual.estatus != PM_Status.CREADO && poliza_manual.estatus != PM_Status.RECHAZADO_VALIDADOR && poliza_manual.estatus != PM_Status.RECHAZADO_AUTORIZADOR)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede modificar esta Póliza!";
                    ViewBag.Descripcion = "No se puede modificar una póliza que ha sido enviada, aprobada o finalizada.";

                    return View("../Home/ErrorGenerico");
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();
                ViewBag.Solicitante = empleado;

                //obtiene el usuario capturista
                PM_usuarios_capturistas user = db.PM_usuarios_capturistas.Where(x => x.id_empleado == empleado.id && x.activo == true).FirstOrDefault();

                //verifica que el usuario este registrado como capturista
                if (user == null)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede acceder a esta Sección!";
                    ViewBag.Descripcion = "Este usuario no se encuentra registrado como capturista, favor de contactar al Administrador del Sitio para solicitar el permiso.";

                    return View("../Home/ErrorGenerico");
                }


                ViewBag.currency_iso = AddFirstItem(new SelectList(db.currency.Where(x => x.activo), "CurrencyISO", "CocatCurrency"), selected: poliza_manual.currency_iso);
                ViewBag.id_PM_tipo_poliza = AddFirstItem(new SelectList(db.PM_tipo_poliza.Where(x => x.activo), "id", "descripcion"), selected: poliza_manual.id_PM_tipo_poliza.ToString());
                ViewBag.id_validador = AddFirstItem(new SelectList(db.PM_departamentos.Where(x => x.id == user.id_departamento), "id_empleado_jefe", "empleados.ConcatNombre"), selected: poliza_manual.id_validador.ToString());
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), selected: poliza_manual.id_planta.ToString()); 

                return View(poliza_manual);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: PolizaManual/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(poliza_manual poliza_manual)
        {
            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();
            ViewBag.Solicitante = empleado;

            //obtiene el usuario capturista
            PM_usuarios_capturistas user = db.PM_usuarios_capturistas.Where(x => x.id_empleado == empleado.id && x.activo == true).FirstOrDefault();

            //verifica que el usuario este registrado como capturista
            if (user == null)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede acceder a esta Sección!";
                ViewBag.Descripcion = "Este usuario no se encuentra registrado como capturista, favor de contactar al Administrador del Sitio para solicitar el permiso.";

                return View("../Home/ErrorGenerico");
            }

            biblioteca_digital archivo = new biblioteca_digital { };

            //determina si hay conceptos
            if (poliza_manual.PM_conceptos.Count == 0)
                ModelState.AddModelError("", "No se ingresaron conceptos para la póliza.");

            decimal debe = 0;
            decimal haber = 0;
            bool existenConceptosVacios = false;

            //determina si las sumas son iguales
            foreach (PM_conceptos item in poliza_manual.PM_conceptos)
            {
                //suma el valor de debe
                if (item.debe.HasValue)
                    debe += item.debe.Value;
                //suma el valor de haber
                if (item.haber.HasValue)
                    haber += item.haber.Value;

                //verifica si extisten conceptos vacios
                if (!item.debe.HasValue && !item.haber.HasValue)
                    existenConceptosVacios = true;
            }

            //agrega error si los campos estan vacíos
            if (existenConceptosVacios)
                ModelState.AddModelError("", "Verifique que todos los conceptos tengan al menos un valor asignado.");

            //verifica si las sumas son diferentes
            if (debe != haber)
                ModelState.AddModelError("", "Las sumas no son iguales.");

            //verifica si el tamaño del archivo es válido
            if (poliza_manual.PostedFileSoporte != null && poliza_manual.PostedFileSoporte.InputStream.Length > 5242880)
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 5MB.");
            else if (poliza_manual.PostedFileSoporte != null)
            { //verifica la extensión del archivo
                string extension = Path.GetExtension(poliza_manual.PostedFileSoporte.FileName);
                if (extension.ToUpper() != ".XLS"   //si no contiene una extensión válida
                               && extension.ToUpper() != ".XLSX"
                               && extension.ToUpper() != ".DOC"
                               && extension.ToUpper() != ".DOCX"
                               && extension.ToUpper() != ".PDF"
                               && extension.ToUpper() != ".PNG"
                               && extension.ToUpper() != ".JPG"
                               && extension.ToUpper() != ".JPEG"
                               && extension.ToUpper() != ".RAR"
                               && extension.ToUpper() != ".ZIP"
                               )
                    ModelState.AddModelError("", "Sólo se permiten archivos con extensión .xls, .xlsx, .doc, .docx, .pdf, .png, .jpg, .jpeg, .rar, .zip.");
                else
                { //si la extension y el tamaño son válidos
                    String nombreArchivo = poliza_manual.PostedFileSoporte.FileName;

                    //recorta el nombre del archivo en caso de ser necesario
                    if (nombreArchivo.Length > 80)
                        nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                    //lee el archivo en un arreglo de bytes
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(poliza_manual.PostedFileSoporte.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(poliza_manual.PostedFileSoporte.ContentLength);
                    }

                    //si tiene archivo hace un update sino hace un create
                    if (poliza_manual.id_documento_soporte.HasValue)//si tiene valor hace un update
                    {
                        archivo = db.biblioteca_digital.Find(poliza_manual.id_documento_soporte.Value);
                        archivo.Nombre = nombreArchivo;
                        archivo.MimeType = UsoStrings.RecortaString(poliza_manual.PostedFileSoporte.ContentType, 80);
                        archivo.Datos = fileData;

                        db.Entry(archivo).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    { //si no tiene hace un create 

                        //genera el archivo de biblioteca digital
                        archivo = new biblioteca_digital
                        {
                            Nombre = nombreArchivo,
                            MimeType = UsoStrings.RecortaString(poliza_manual.PostedFileSoporte.ContentType, 80),
                            Datos = fileData
                        };

                        //update en BD
                        db.biblioteca_digital.Add(archivo);
                        db.SaveChanges();
                    }

                }
            }

            if (ModelState.IsValid)
            {
                //verifica si se creo un archivo
                if (archivo.Id > 0) //si existe algún archivo en biblioteca digital
                    poliza_manual.id_documento_soporte = archivo.Id;

                //borra los conceptos anteriores
                var list = db.PM_conceptos.Where(x => x.id_poliza == poliza_manual.id);
                foreach (PM_conceptos item in list)
                    db.PM_conceptos.Remove(item);
                db.SaveChanges();

                //los nuevos conceptos se agregan automáticamente

                //si existe lo modifica
                poliza_manual poliza = db.poliza_manual.Find(poliza_manual.id);
                // Activity already exist in database and modify it
                db.Entry(poliza).CurrentValues.SetValues(poliza_manual);
                //agrega los conceptos 
                poliza.PM_conceptos = poliza_manual.PM_conceptos;
                //agrega el id del departamento que autoriza
                poliza.id_autorizador = user.PM_departamentos.PM_departamentos2.id_empleado_jefe;

                db.Entry(poliza).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                if (poliza.estatus == PM_Status.RECHAZADO_VALIDADOR || poliza.estatus == PM_Status.RECHAZADO_AUTORIZADOR)
                    return RedirectToAction("CapturistaRechazadas");

                return RedirectToAction("CapturistaCreadas");
            }
            //obtiene el registro original
            poliza_manual poliza_Manual_original = db.poliza_manual.Find(poliza_manual.id);
            poliza_manual.empleados = poliza_Manual_original.empleados;
            poliza_manual.empleados1 = poliza_Manual_original.empleados1;
            poliza_manual.empleados2 = poliza_Manual_original.empleados2;
            poliza_manual.empleados3 = poliza_Manual_original.empleados3;


            //cargar el documento de soporte
            if (poliza_manual.id_documento_soporte.HasValue) 
                poliza_manual.biblioteca_digital1 = db.biblioteca_digital.Find(poliza_manual.id_documento_soporte);

            //cargar el documento de registro
            if (poliza_manual.id_documento_registro.HasValue)
                poliza_manual.biblioteca_digital = db.biblioteca_digital.Find(poliza_manual.id_documento_registro);

            poliza_manual.empleados = db.empleados.Find(poliza_manual.id_elaborador);

            ViewBag.currency_iso = AddFirstItem(new SelectList(db.currency.Where(x => x.activo), "CurrencyISO", "CocatCurrency"));
            ViewBag.id_PM_tipo_poliza = AddFirstItem(new SelectList(db.PM_tipo_poliza.Where(x => x.activo), "id", "descripcion"));
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"));
            ViewBag.id_validador = AddFirstItem(new SelectList(db.PM_departamentos.Where(x => x.id == user.id_departamento), "id_empleado_jefe", "empleados.ConcatNombre"), selected: poliza_manual.id_validador.ToString());

            return View(poliza_manual);
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
