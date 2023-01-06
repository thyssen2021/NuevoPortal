using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class GV_solicitudController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: GV_solicitud
        public ActionResult Solicitudes(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();

            var listado = db.GV_solicitud
                .Where(x => (x.id_empleado == empleado.id || x.id_solicitante == empleado.id)
                    && (x.estatus == estatus || estatus == "ALL"
                                    || (estatus == "EN_PROCESO" && (x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                    ))
                                    || (estatus == "POR_CONFIRMAR" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                    ) && !x.fecha_confirmacion_usuario.HasValue)
                                    || (estatus == "RECHAZADAS" && (x.estatus == GV_solicitud_estatus.RECHAZADO_JEFE
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_CONTROLLING
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_NOMINA || x.estatus == GV_solicitud_estatus.RECHAZADO_CONTABILIDAD))
                                                    )
                )
                .OrderByDescending(x => x.fecha_solicitud)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_solicitud
                  .Where(x => (x.id_empleado == empleado.id || x.id_solicitante == empleado.id)
                    && (x.estatus == estatus || estatus == "ALL"
                                    || (estatus == "EN_PROCESO" && (x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                     || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                    ))
                                    || (estatus == "POR_CONFIRMAR" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                    ) && !x.fecha_confirmacion_usuario.HasValue)
                                    || (estatus == "RECHAZADAS" && (x.estatus == GV_solicitud_estatus.RECHAZADO_JEFE
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_CONTROLLING
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_NOMINA || x.estatus == GV_solicitud_estatus.RECHAZADO_CONTABILIDAD))
                                                    )
                )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual


            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = "Todas", Value = "ALL" });
            newList.Add(new SelectListItem() { Text = "Creadas (sin enviar)", Value = Bitacoras.Util.GV_solicitud_estatus.CREADO });
            newList.Add(new SelectListItem() { Text = "Por confirmar", Value = "POR_CONFIRMAR" });
            newList.Add(new SelectListItem() { Text = "En proceso", Value = "EN_PROCESO" });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = "RECHAZADAS" });
            newList.Add(new SelectListItem() { Text = "Finalizadas", Value = Bitacoras.Util.GV_solicitud_estatus.FINALIZADO });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;
            ViewBag.SegundoNivel = "GV_solicitud";
            ViewBag.Title = "Mis Solicitudes";


            //ordenar por fecha de solicitud más reciente
            return View("solicitudes", listado);

        }

        // GET: SolicitudesJefeDirecto
        public ActionResult SolicitudesJefeDirecto(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_JEFE_DIRECTO))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();

            var listado = db.GV_solicitud
                .Where(x => (x.id_jefe_directo == empleado.id)
                    && (x.estatus == estatus || (estatus == "ALL" && x.estatus != GV_solicitud_estatus.CREADO) ||
                    (estatus == "AUTORIZADAS_JEFE" && (x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING || x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                     || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.FINALIZADO))
                    )
                )
                .OrderByDescending(x => x.fecha_solicitud)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_solicitud
                 .Where(x => (x.id_jefe_directo == empleado.id)
                    && (x.estatus == estatus || (estatus == "ALL" && x.estatus != GV_solicitud_estatus.CREADO) ||
                    (estatus == "AUTORIZADAS_JEFE" && (x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING || x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                     || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                    || x.estatus == GV_solicitud_estatus.FINALIZADO))
                    )
                )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };


            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = "Todas", Value = "ALL" });
            newList.Add(new SelectListItem() { Text = "Pendientes", Value = Bitacoras.Util.GV_solicitud_estatus.ENVIADO_A_JEFE });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = Bitacoras.Util.GV_solicitud_estatus.RECHAZADO_JEFE });
            newList.Add(new SelectListItem() { Text = "Autorizadas", Value = "AUTORIZADAS_JEFE" });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;
            ViewBag.SegundoNivel = "solicitudes_jefe_directo";
            ViewBag.Title = "Solicitudes Jefe Directo";

            //ordenar por fecha de solicitud más reciente
            return View("solicitudes", listado);

        }

        // GET: SolicitudesJefeDirecto
        public ActionResult Autorizaciones(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_AUTORIZACION))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();

            var listado = db.GV_solicitud
                 .Where(x =>
                     ((x.estatus == estatus) ||
                        (estatus == "AUTORIZADAS_JEFE" && (x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING || x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.FINALIZADO) && x.id_jefe_directo == empleado.id)
                    )
                )
                .OrderByDescending(x => x.fecha_solicitud)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_solicitud
                  .Where(x =>
                     ((x.estatus == estatus) ||
                        (estatus == "AUTORIZADAS_JEFE" && (x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING || x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.FINALIZADO) && x.id_jefe_directo == empleado.id)
                    )
                )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };


            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            // newList.Add(new SelectListItem() { Text = "Todas", Value = "ALL" });
            newList.Add(new SelectListItem() { Text = "Pendientes", Value = Bitacoras.Util.GV_solicitud_estatus.ENVIADO_A_JEFE });
            // newList.Add(new SelectListItem() { Text = "Rechazadas", Value = Bitacoras.Util.GV_solicitud_estatus.RECHAZADO_JEFE });
            newList.Add(new SelectListItem() { Text = "Autorizadas", Value = "AUTORIZADAS_JEFE" });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;
            ViewBag.SegundoNivel = "gv_autorizaciones";
            ViewBag.Title = "Autorización de solicitudes (casos especiales)";

            //ordenar por fecha de solicitud más reciente
            return View("solicitudes", listado);

        }
        // GET: SolicitudesControlling
        public ActionResult SolicitudesControlling(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_CONTROLLING))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();

            var listado = db.GV_solicitud
                 .Where(x => ((x.id_controlling.HasValue && estatus != "PENDIENTES_PROPIAS") || (estatus == "PENDIENTES_PROPIAS" && x.id_controlling == empleado.id))
                    && (x.estatus == estatus || (estatus == "ALL") ||//debe tener id_controlling 
                    (estatus == "PENDIENTES_PROPIAS" && x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING) ||
                    (estatus == "AUTORIZADAS_CONTROLLING" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                     || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.FINALIZADO))
                    )
                )
                .OrderByDescending(x => x.fecha_solicitud)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_solicitud
                      .Where(x => ((x.id_controlling.HasValue && estatus != "PENDIENTES_PROPIAS") || (estatus == "PENDIENTES_PROPIAS" && x.id_controlling == empleado.id))
                    && (x.estatus == estatus || (estatus == "ALL") ||//debe tener id_controlling 
                    (estatus == "PENDIENTES_PROPIAS" && x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING) ||
                    (estatus == "AUTORIZADAS_CONTROLLING" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                     || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.FINALIZADO))
                    )
                )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };


            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = "Todas", Value = "ALL" });
            newList.Add(new SelectListItem() { Text = "Asignaciones Pendientes", Value = "PENDIENTES_PROPIAS" });
            newList.Add(new SelectListItem() { Text = "Pendientes General", Value = Bitacoras.Util.GV_solicitud_estatus.ENVIADO_CONTROLLING });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = Bitacoras.Util.GV_solicitud_estatus.RECHAZADO_CONTROLLING });
            newList.Add(new SelectListItem() { Text = "Autorizadas", Value = "AUTORIZADAS_CONTROLLING" });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;
            ViewBag.SegundoNivel = "solicitudes_controlling";
            ViewBag.Title = "Solicitudes Controlling";

            //ordenar por fecha de solicitud más reciente
            return View("solicitudes", listado);

        }

        // GET: SolicitudesNomina
        public ActionResult SolicitudesNomina(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_NOMINA))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();

            var listado = db.GV_solicitud
                 .Where(x => ((x.id_nomina.HasValue && estatus != "PENDIENTES_PROPIAS") || (estatus == "PENDIENTES_PROPIAS" && x.id_nomina == empleado.id))
                    && (x.estatus == estatus || (estatus == "ALL") ||//debe tener id_controlling 
                    (estatus == "PENDIENTES_PROPIAS" && (x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO))
                    || (estatus == "PENDIENTES_GENERAL" && (x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO))
                    || (estatus == "AUTORIZADAS_NOMINA" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                    || x.estatus == GV_solicitud_estatus.FINALIZADO))
                    )
                )
                .OrderByDescending(x => x.fecha_solicitud)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_solicitud
                 .Where(x => ((x.id_nomina.HasValue && estatus != "PENDIENTES_PROPIAS") || (estatus == "PENDIENTES_PROPIAS" && x.id_nomina == empleado.id))
                    && (x.estatus == estatus || (estatus == "ALL") ||//debe tener id_controlling 
                    (estatus == "PENDIENTES_PROPIAS" && (x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO))
                    || (estatus == "PENDIENTES_GENERAL" && (x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO))
                    || (estatus == "AUTORIZADAS_NOMINA" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                    || x.estatus == GV_solicitud_estatus.FINALIZADO))
                    )
                )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };


            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = "Todas", Value = "ALL" });
            newList.Add(new SelectListItem() { Text = "Asignaciones Pendientes", Value = "PENDIENTES_PROPIAS" });
            newList.Add(new SelectListItem() { Text = "Pendientes General", Value = "PENDIENTES_GENERAL" });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = Bitacoras.Util.GV_solicitud_estatus.RECHAZADO_NOMINA });
            newList.Add(new SelectListItem() { Text = "Confirmadas", Value = "AUTORIZADAS_NOMINA" });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;
            ViewBag.SegundoNivel = "solicitudes_nomina";
            ViewBag.Title = "Solicitudes Nómina";

            //ordenar por fecha de solicitud más reciente
            return View("solicitudes", listado);

        }
        // GET: SolicitudesContabilidad
        public ActionResult SolicitudesContabilidad(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_CONTABILIDAD))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();

            var listado = db.GV_solicitud
                .Where(x => ((x.id_contabilidad.HasValue && estatus != "PENDIENTES_PROPIAS") || (estatus == "PENDIENTES_PROPIAS" && x.id_contabilidad == empleado.id))
                    && (x.estatus == estatus || (estatus == "ALL") ||//debe tener id_controlling 
                    (estatus == "PENDIENTES_PROPIAS" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO) && !x.fecha_aceptacion_contabilidad.HasValue)
                   || (estatus == "PENDIENTES_GENERAL" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO) && !x.fecha_aceptacion_contabilidad.HasValue)
                   || (estatus == "CONFIRMADAS" && (x.estatus == GV_solicitud_estatus.FINALIZADO || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD))
                    )
                )
                .OrderByDescending(x => x.fecha_solicitud)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_solicitud
                   .Where(x => ((x.id_contabilidad.HasValue && estatus != "PENDIENTES_PROPIAS") || (estatus == "PENDIENTES_PROPIAS" && x.id_contabilidad == empleado.id))
                    && (x.estatus == estatus || (estatus == "ALL") ||//debe tener id_controlling 
                    (estatus == "PENDIENTES_PROPIAS" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO) && !x.fecha_aceptacion_contabilidad.HasValue)
                   || (estatus == "PENDIENTES_GENERAL" && (x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO) && !x.fecha_aceptacion_contabilidad.HasValue)
                   || (estatus == "CONFIRMADAS" && (x.estatus == GV_solicitud_estatus.FINALIZADO || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD))
                    )
                )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };


            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = "Todas", Value = "ALL" });
            newList.Add(new SelectListItem() { Text = "Asignaciones Pendientes", Value = "PENDIENTES_PROPIAS" });
            newList.Add(new SelectListItem() { Text = "Pendientes General", Value = "PENDIENTES_GENERAL" });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = Bitacoras.Util.GV_solicitud_estatus.RECHAZADO_CONTABILIDAD });
            newList.Add(new SelectListItem() { Text = "Confirmadas", Value = "CONFIRMADAS" });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;
            ViewBag.SegundoNivel = "solicitudes_contabilidad";
            ViewBag.Title = "Solicitudes Cuentas por Pagar";

            //ordenar por fecha de solicitud más reciente
            return View("solicitudes", listado);

        }


        // GET: GV_solicitud/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD)) //Todos los usuarios van a tener permiso para crear solicitudes de GV, Por lo que solo se necesita validar este permiso
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            return View(gV_solicitud);
        }

        // GET: GV_solicitud/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD))
                return View("../Home/ErrorPermisos");

            //obtiene el usuario logeado
            empleados solicitante = obtieneEmpleadoLogeado();

            GV_solicitud solicitud = new GV_solicitud
            {
                id_solicitante = solicitante.id,
                empleados5 = solicitante,
                fecha_salida = DateTime.Now,
                fecha_regreso = DateTime.Now,
            };

            //agrega elementos para los rel checklist            
            foreach (var item in db.GV_tipo_gastos_viaje.Where(x => x.activo))
            {
                //solo agrega si no existe previamente 
                if (!solicitud.GV_rel_gastos_solicitud.Any(x => x.id_tipo_gastos_viaje == item.id))
                    solicitud.GV_rel_gastos_solicitud.Add(new GV_rel_gastos_solicitud
                    {
                        id_tipo_gastos_viaje = item.id,
                        currency_iso = "MXN",
                        GV_tipo_gastos_viaje = item,
                    });
            }

            ViewBag.iso_moneda_extranjera = AddFirstItem(new SelectList(db.currency.Where(x => x.activo && (x.CurrencyISO == "USD" || x.CurrencyISO == "EUR")), "CurrencyISO", "CocatCurrency"), selected: "USD");
            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value), "id", "ConcatNumEmpleadoNombre"), selected: solicitante.id.ToString());
            ViewBag.id_medio_transporte = AddFirstItem(new SelectList(db.GV_medios_transporte.Where(x => x.activo), "id", "descripcion"));

            return View(solicitud);
        }

        // POST: GV_solicitud/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GV_solicitud solicitud)
        {
            //obtiene el usuario logeado
            empleados solicitante = obtieneEmpleadoLogeado();

            //verifica que se haya ingresado al menos una cantidad
            if (solicitud.GV_rel_gastos_solicitud.Sum(x => x.importe) <= 0)
                ModelState.AddModelError("", "No se agregaron gastos a la estimación.");

            //busca coceptos en cero
            int i = 1;
            foreach (var item in solicitud.GV_rel_gastos_solicitud)
            {
                if (item.cantidad == 0 && item.importe != 0)
                    ModelState.AddModelError("", "Estimación de Gastos (Moneda Nacional): si se específica un importe la cantidad no puede ser cero (fila " + i + ").");
                if (item.cantidad != 0 && item.importe == 0)
                    ModelState.AddModelError("", "Estimación de Gastos (Moneda Nacional): si se específica una cantidad el importe no puede ser cero (fila " + i + ").");
                i++;
            }

            if (ModelState.IsValid)
            {
                //borra los conceptos anteriores
                var list = db.GV_rel_gastos_solicitud.Where(x => x.id_gv_solicitud == solicitud.id);
                foreach (var itemRemove in list)
                    db.GV_rel_gastos_solicitud.Remove(itemRemove);

                //agrega los nuevos items rel 
                foreach (var iteamAdd in solicitud.GV_rel_gastos_solicitud)
                    db.GV_rel_gastos_solicitud.Add(iteamAdd);

                solicitud.fecha_solicitud = DateTime.Now;
                solicitud.estatus = Bitacoras.Util.GV_solicitud_estatus.CREADO;

                db.GV_solicitud.Add(solicitud);

                try
                {
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Solicitudes", new { estatus = Bitacoras.Util.GV_solicitud_estatus.CREADO });
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert(e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Solicitudes", new { estatus = Bitacoras.Util.GV_solicitud_estatus.CREADO });
                }
            }
            //relaciona el solicitante con la solitud
            solicitud.empleados5 = solicitante;

            //obtiene la propiedades para rels
            foreach (var rel in solicitud.GV_rel_gastos_solicitud)
                rel.GV_tipo_gastos_viaje = db.GV_tipo_gastos_viaje.Find(rel.id_tipo_gastos_viaje);

            ViewBag.iso_moneda_extranjera = AddFirstItem(new SelectList(db.currency.Where(x => x.activo && (x.CurrencyISO == "USD" || x.CurrencyISO == "EUR")), "CurrencyISO", "CocatCurrency"), selected: solicitud.iso_moneda_extranjera);
            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value), "id", "ConcatNumEmpleadoNombre"), selected: solicitud.id_empleado.ToString());
            ViewBag.id_medio_transporte = AddFirstItem(new SelectList(db.GV_medios_transporte.Where(x => x.activo), "id", "descripcion"), selected: solicitud.id_medio_transporte.ToString());

            return View(solicitud);
        }

        // GET: GV_solicitud/Edit/5
        public ActionResult Edit(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD)) //Todos los usuarios van a tener permiso para crear solicitudes de GV, Por lo que solo se necesita validar este permiso
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //verifica si se puede editar
            if (gV_solicitud.estatus != GV_solicitud_estatus.CREADO && gV_solicitud.estatus != GV_solicitud_estatus.RECHAZADO_CONTABILIDAD
                && gV_solicitud.estatus != GV_solicitud_estatus.RECHAZADO_CONTROLLING && gV_solicitud.estatus != GV_solicitud_estatus.RECHAZADO_JEFE
                && gV_solicitud.estatus != GV_solicitud_estatus.RECHAZADO_NOMINA && gV_solicitud.estatus != GV_solicitud_estatus.RECHAZADO_USUARIO
                )
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede editar esta solicitud!";
                ViewBag.Descripcion = "No se puede editar una solicitud que ya ha sido validada, rechazada o finalizada.";

                return View("../Home/ErrorGenerico");
            }

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;
            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //asocia el empleado actual como el solicitante
            //obtiene el usuario logeado
            empleados solicitante = obtieneEmpleadoLogeado();
            gV_solicitud.empleados5 = solicitante;
            gV_solicitud.id_solicitante = solicitante.id;

            ViewBag.iso_moneda_extranjera = AddFirstItem(new SelectList(db.currency.Where(x => x.activo && (x.CurrencyISO == "USD" || x.CurrencyISO == "EUR")), "CurrencyISO", "CocatCurrency"), selected: gV_solicitud.iso_moneda_extranjera);
            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value), "id", "ConcatNumEmpleadoNombre"), selected: gV_solicitud.id_empleado.ToString());
            ViewBag.id_medio_transporte = AddFirstItem(new SelectList(db.GV_medios_transporte.Where(x => x.activo), "id", "descripcion"), selected: gV_solicitud.id_medio_transporte.ToString());

            return View(gV_solicitud);
        }

        // POST: GV_solicitud/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GV_solicitud gV_solicitud)
        {
            //obtiene el usuario logeado
            empleados solicitante = obtieneEmpleadoLogeado();

            //verifica que se haya ingresado al menos una cantidad
            if (gV_solicitud.GV_rel_gastos_solicitud.Sum(x => x.importe) <= 0)
                ModelState.AddModelError("", "No se agregaron gastos a la estimación.");

            //busca coceptos en cero
            int i = 1;
            foreach (var item in gV_solicitud.GV_rel_gastos_solicitud)
            {
                if (item.cantidad == 0 && item.importe != 0)
                    ModelState.AddModelError("", "Estimación de Gastos (Moneda Nacional): si se específica un importe la cantidad no puede ser cero (fila " + i + ").");
                if (item.cantidad != 0 && item.importe == 0)
                    ModelState.AddModelError("", "Estimación de Gastos (Moneda Nacional): si se específica una cantidad el importe no puede ser cero (fila " + i + ").");
                i++;
            }

            if (ModelState.IsValid)
            {
                //borra los conceptos anteriores
                var list = db.GV_rel_gastos_solicitud.Where(x => x.id_gv_solicitud == gV_solicitud.id);
                foreach (var itemRemove in list)
                    db.GV_rel_gastos_solicitud.Remove(itemRemove);

                //agrega los nuevos items rel 
                foreach (var iteamAdd in gV_solicitud.GV_rel_gastos_solicitud)
                    db.GV_rel_gastos_solicitud.Add(iteamAdd);

                try
                {
                    db.Entry(gV_solicitud).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Solicitudes", new { estatus = gV_solicitud.s_estatus });
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert(e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Solicitudes", new { estatus = gV_solicitud.s_estatus });
                }
            }
            //relaciona el solicitante con la solitud
            gV_solicitud.empleados5 = solicitante;

            //obtiene la propiedades para rels
            foreach (var rel in gV_solicitud.GV_rel_gastos_solicitud)
                rel.GV_tipo_gastos_viaje = db.GV_tipo_gastos_viaje.Find(rel.id_tipo_gastos_viaje);

            ViewBag.iso_moneda_extranjera = AddFirstItem(new SelectList(db.currency.Where(x => x.activo && (x.CurrencyISO == "USD" || x.CurrencyISO == "EUR")), "CurrencyISO", "CocatCurrency"), selected: gV_solicitud.iso_moneda_extranjera);
            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value), "id", "ConcatNumEmpleadoNombre"), selected: gV_solicitud.id_empleado.ToString());
            ViewBag.id_medio_transporte = AddFirstItem(new SelectList(db.GV_medios_transporte.Where(x => x.activo), "id", "descripcion"), selected: gV_solicitud.id_medio_transporte.ToString());


            return View(gV_solicitud);
        }

        #region JEFE_DIRECTO

        // GET: GV_solicitud/EnviarAJefeDirecto/5
        public ActionResult EnviarAJefeDirecto(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            return View(gV_solicitud);
        }

        // POST: GV_solicitud/EnviarAJefeDirecto/5
        [HttpPost, ActionName("EnviarAJefeDirecto")]
        [ValidateAntiForgeryToken]
        public ActionResult EnviarAJefeDirectoConfirmed(int id, string s_estatus)
        {
            GV_solicitud gv = db.GV_solicitud.Find(id);

            //necesario para poder guardar los cambios
            gv.medio_transporte_aplica_otro = !gv.id_medio_transporte.HasValue;

            gv.estatus = GV_solicitud_estatus.ENVIADO_A_JEFE;
            //borra las fechas 
            gv.fecha_aceptacion_jefe_area = null;
            gv.fecha_aceptacion_controlling = null;
            gv.fecha_aceptacion_contabilidad = null;
            gv.fecha_aceptacion_nomina = null;

            db.Entry(gv).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO
                //correos.Add("alfredo.xochitemol@lagermex.com.mx");

                if (!String.IsNullOrEmpty(gv.empleados3.correo))
                    correos.Add(gv.empleados3.correo); //agrega correo de jefe directo

                envioCorreo.SendEmailAsync(correos, "Ha recibido una Solicitud de Gastos de Viaje para su aprobación.", envioCorreo.getBodyGVNotificacionEnvioJefe(gv));


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
                return RedirectToAction("Solicitudes", new { estatus = s_estatus });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Solicitudes", new { estatus = s_estatus });
            }

            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido enviada", TipoMensajesSweetAlerts.SUCCESS);

            return RedirectToAction("Solicitudes", new { estatus = s_estatus });
        }


        // GET: GV_solicitud/AutorizarJefeDirecto/5
        public ActionResult AutorizarJefeDirecto(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_JEFE_DIRECTO) && !TieneRol(TipoRoles.GV_AUTORIZACION))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //verifica si se puede autorizar
            if (gV_solicitud.estatus != GV_solicitud_estatus.ENVIADO_A_JEFE)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede validar esta solicitud!";
                ViewBag.Descripcion = "No se puede autorizar una solicitud que ya ha sido validada, rechazada o finalizada.";

                return View("../Home/ErrorGenerico");
            }

            //verifica si es el usuario asignado o si tiene el permiso especial
            var empleado = obtieneEmpleadoLogeado();
            if (gV_solicitud.id_jefe_directo != empleado.id && !TieneRol(TipoRoles.GV_AUTORIZACION))
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede validar esta solicitud!";
                ViewBag.Descripcion = "No se puede autorizar esta solicitud, ya que no se encuentra asignado a la solicitud actual.";

                return View("../Home/ErrorGenerico");
            }

            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            //obtiene la lista de usuarios asociados a controlling
            var listUsuarios = db.GV_usuarios.Where(x => x.departamento == Bitacoras.Util.GV_tipo_departamento.CONTROLLING).Select(x => x.id_empleado).ToList();
            ViewBag.id_controlling = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value && listUsuarios.Contains(x.id)), "id", "ConcatNumEmpleadoNombre")
                , selected: "168"); //id de ismael

            return View(gV_solicitud);
        }

        // GET: solicitudManual/AutorizarJefeDirecto/5
        [HttpPost, ActionName("AutorizarJefeDirecto")]
        [ValidateAntiForgeryToken]
        public ActionResult AutorizarJefeDirectoConfirmed(int? id, int? id_controlling, string s_estatus)
        {
            if (TieneRol(TipoRoles.GV_JEFE_DIRECTO) || TieneRol(TipoRoles.GV_AUTORIZACION))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_solicitud solicitud = db.GV_solicitud.Find(id);
                if (solicitud == null)
                {
                    return View("../Error/NotFound");
                }

                //determina si viene de una autorización especial
                bool esAutorizacionEspecial = s_estatus == "Autorizacion";

                //sustituye el id_jefe_directo por el del usuario actual
                var empleado = obtieneEmpleadoLogeado();
                solicitud.id_jefe_directo = empleado.id;

                //necesario para poder guardar los cambios
                solicitud.medio_transporte_aplica_otro = !solicitud.id_medio_transporte.HasValue;

                solicitud.estatus = GV_solicitud_estatus.ENVIADO_CONTROLLING;
                solicitud.id_controlling = id_controlling;

                //establece las fechas 
                solicitud.fecha_aceptacion_jefe_area = DateTime.Now;
                solicitud.fecha_aceptacion_controlling = null;
                solicitud.fecha_aceptacion_contabilidad = null;
                solicitud.fecha_aceptacion_nomina = null;


                db.Entry(solicitud).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    //envia correo electronico
                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                    List<String> correos = new List<string>(); //correos TO

                    if (!String.IsNullOrEmpty(solicitud.empleados1.correo))
                        correos.Add(solicitud.empleados1.correo);

                    envioCorreo.SendEmailAsync(correos, "Ha recibido una solicitud de Anticipo de Gastos de Viaje.", envioCorreo.getBodyGVNotificacionEnvioControlling(solicitud));

                    //en caso de ser autorización especial envía notificacion al JEFE
                    if (esAutorizacionEspecial)
                        envioCorreo.SendEmailAsync(correos, "Se ha autorizado una solicitud de Anticipo de Gastos de Viaje pendiente.", envioCorreo.getBodyGVNotificacionJefeDirectoEspecial(solicitud));

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
                    if (esAutorizacionEspecial)
                        return RedirectToAction("Autorizaciones", new { estatus = Bitacoras.Util.GV_solicitud_estatus.ENVIADO_A_JEFE });
                    else
                        return RedirectToAction("SolicitudesJefeDirecto", new { estatus = s_estatus });

                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    if (esAutorizacionEspecial)
                        return RedirectToAction("Autorizaciones", new { estatus = Bitacoras.Util.GV_solicitud_estatus.ENVIADO_A_JEFE });
                    else
                        return RedirectToAction("SolicitudesJefeDirecto", new { estatus = s_estatus });
                }
                TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido autorizada.", TipoMensajesSweetAlerts.SUCCESS);
                if (esAutorizacionEspecial)
                    return RedirectToAction("Autorizaciones", new { estatus = Bitacoras.Util.GV_solicitud_estatus.ENVIADO_A_JEFE });
                else
                    return RedirectToAction("SolicitudesJefeDirecto", new { estatus = s_estatus });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: GV_solicitud/AutorizarJefeDirecto/5
        [HttpPost, ActionName("RechazarJefeDirecto")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarJefeDirectoConfirmed(int? id, string s_estatus, string comentario_rechazo)
        {

            //determina si viene de una autorización especial
            bool esAutorizacionEspecial = s_estatus == "Autorizacion";          

            GV_solicitud gv = db.GV_solicitud.Find(id);
            gv.estatus = GV_solicitud_estatus.RECHAZADO_JEFE;
            gv.comentario_rechazo = comentario_rechazo;
            //borra las fechas 
            gv.fecha_aceptacion_jefe_area = null;
            gv.fecha_aceptacion_controlling = null;
            gv.fecha_aceptacion_contabilidad = null;
            gv.fecha_aceptacion_nomina = null;

            //sustituye el id_jefe_directo por el del usuario actual
            var empleado = obtieneEmpleadoLogeado();
            //gv.id_jefe_directo = empleado.id;

            //necesario para poder guardar los cambios
            gv.medio_transporte_aplica_otro = !gv.id_medio_transporte.HasValue;

            db.Entry(gv).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO


                if (!String.IsNullOrEmpty(gv.empleados5.correo))
                    correos.Add(gv.empleados5.correo); //agrega correo de solicitantes

                envioCorreo.SendEmailAsync(correos, "Se ha rechazado su solicitud de anticipo gastos de viaje.", envioCorreo.getBodyGVNotificacionRechazo(gv, empleado.ConcatNombre));

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
                if (esAutorizacionEspecial)
                    return RedirectToAction("Autorizaciones", new { estatus = Bitacoras.Util.GV_solicitud_estatus.ENVIADO_A_JEFE });
                else
                    return RedirectToAction("SolicitudesJefeDirecto", new { estatus = s_estatus });
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                if (esAutorizacionEspecial)
                    return RedirectToAction("Autorizaciones", new { estatus = Bitacoras.Util.GV_solicitud_estatus.ENVIADO_A_JEFE });
                else
                    return RedirectToAction("SolicitudesJefeDirecto", new { estatus = s_estatus });
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido rechazada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            if (esAutorizacionEspecial)
                return RedirectToAction("Autorizaciones", new { estatus = Bitacoras.Util.GV_solicitud_estatus.ENVIADO_A_JEFE });
            else
                return RedirectToAction("SolicitudesJefeDirecto", new { estatus = s_estatus });
        }
        #endregion

        #region CONTROLLING  

        // GET: GV_solicitud/AutorizarControlling/5
        public ActionResult AutorizarControlling(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_CONTROLLING))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            //obtiene la lista de usuarios asociados a controlling
            var listUsuarios = db.GV_usuarios.Where(x => x.departamento == Bitacoras.Util.GV_tipo_departamento.NOMINA).Select(x => x.id_empleado).ToList();
            ViewBag.id_nomina = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value && listUsuarios.Contains(x.id)), "id", "ConcatNumEmpleadoNombre")); //id de ismael

            return View(gV_solicitud);
        }

        // GET: solicitudManual/AutorizarJefeDirecto/5
        [HttpPost, ActionName("AutorizarControlling")]
        [ValidateAntiForgeryToken]
        public ActionResult AutorizarControllingConfirmed(int? id, int? id_nomina, string s_estatus)
        {
            if (TieneRol(TipoRoles.GV_CONTROLLING))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_solicitud solicitud = db.GV_solicitud.Find(id);
                if (solicitud == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si se puede autorizar
                if (solicitud.estatus != GV_solicitud_estatus.ENVIADO_CONTROLLING)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede validar esta solicitud!";
                    ViewBag.Descripcion = "No se puede autorizar una solicitud que ya ha sido validada, rechazada o finalizada.";

                    return View("../Home/ErrorGenerico");
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //necesario para poder guardar los cambios
                solicitud.medio_transporte_aplica_otro = !solicitud.id_medio_transporte.HasValue;

                solicitud.estatus = GV_solicitud_estatus.ENVIADO_NOMINA;
                solicitud.id_nomina = id_nomina;
                solicitud.id_controlling = empleado.id;

                //establece las fechas 
                solicitud.fecha_aceptacion_controlling = DateTime.Now;
                solicitud.fecha_aceptacion_contabilidad = null;
                solicitud.fecha_aceptacion_nomina = null;


                db.Entry(solicitud).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    //envia correo electronico
                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                    List<String> correos = new List<string>(); //correos TO


                    if (!String.IsNullOrEmpty(solicitud.empleados4.correo))
                        correos.Add(solicitud.empleados4.correo);

                    envioCorreo.SendEmailAsync(correos, "Ha recibido una solicitud de Anticipo de Gastos de Viaje.", envioCorreo.getBodyGVNotificacionEnvioNominas(solicitud));


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
                    return RedirectToAction("SolicitudesControlling", new { estatus = s_estatus });

                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("SolicitudesControlling", new { estatus = s_estatus });
                }
                TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido autorizada.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("SolicitudesControlling", new { estatus = s_estatus });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: GV_solicitud/RechazarControlling/5
        [HttpPost, ActionName("RechazarControlling")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarControllingConfirmed(int? id, string s_estatus, string comentario_rechazo)
        {

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            GV_solicitud gv = db.GV_solicitud.Find(id);
            gv.estatus = GV_solicitud_estatus.RECHAZADO_CONTROLLING;
            gv.comentario_rechazo = comentario_rechazo;
            //borra las fechas 
            gv.fecha_aceptacion_jefe_area = null;
            gv.fecha_aceptacion_controlling = null;
            gv.fecha_aceptacion_contabilidad = null;
            gv.fecha_aceptacion_nomina = null;

            gv.id_controlling = empleado.id;

            //necesario para guardar en BD
            gv.medio_transporte_aplica_otro = !gv.id_medio_transporte.HasValue;

            db.Entry(gv).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                if (!String.IsNullOrEmpty(gv.empleados5.correo))
                    correos.Add(gv.empleados5.correo); //agrega correo de solicitantes

                envioCorreo.SendEmailAsync(correos, "Se ha rechazado su solicitud de anticipo gastos de viaje.", envioCorreo.getBodyGVNotificacionRechazo(gv, gv.empleados1.ConcatNombre));

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
                return RedirectToAction("SolicitudesControlling", new { estatus = s_estatus });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("SolicitudesControlling", new { estatus = s_estatus });
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido rechazada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("SolicitudesControlling", new { estatus = s_estatus });
        }
        #endregion

        #region NOMINA  

        // GET: GV_solicitud/AutorizarNomina/5
        public ActionResult AutorizarNomina(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_NOMINA))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            //obtiene la lista de usuarios asociados a controlling
            var listUsuarios = db.GV_usuarios.Where(x => x.departamento == Bitacoras.Util.GV_tipo_departamento.CUENTASPORPAGAR).Select(x => x.id_empleado).ToList();
            ViewBag.id_contabilidad = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value && listUsuarios.Contains(x.id)), "id", "ConcatNumEmpleadoNombre")); //id de ismael

            return View(gV_solicitud);
        }

        // GET: GV_solicitud/AutorizarNomina/5
        [HttpPost, ActionName("AutorizarNomina")]
        [ValidateAntiForgeryToken]
        public ActionResult AutorizarNominaConfirmed(int? id, int? id_contabilidad, string s_estatus)
        {
            if (TieneRol(TipoRoles.GV_NOMINA))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_solicitud solicitud = db.GV_solicitud.Find(id);
                if (solicitud == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si se puede autorizar
                if (solicitud.estatus != GV_solicitud_estatus.ENVIADO_NOMINA && solicitud.estatus != GV_solicitud_estatus.RECHAZADO_USUARIO)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede confirmar esta solicitud!";
                    ViewBag.Descripcion = "No se puede autorizar una solicitud que ya ha sido confirmada o finalizada.";

                    return View("../Home/ErrorGenerico");
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //necesario para poder guardar los cambios
                solicitud.medio_transporte_aplica_otro = !solicitud.id_medio_transporte.HasValue;

                solicitud.estatus = GV_solicitud_estatus.CONFIRMADO_NOMINA;
                solicitud.id_contabilidad = id_contabilidad;
                solicitud.id_nomina = empleado.id;

                //establece las fechas 
                solicitud.fecha_aceptacion_nomina = DateTime.Now;
                solicitud.fecha_aceptacion_contabilidad = null;


                db.Entry(solicitud).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    //envia correo electronico
                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                    List<String> correos = new List<string>(); //correos TO  //Cuentas por pagar 

                    // ENVÍO A CONTABILIDAD
                    if (!String.IsNullOrEmpty(solicitud.empleados.correo))
                        correos.Add(solicitud.empleados.correo);
                    envioCorreo.SendEmailAsync(correos, "Ha recibido una solicitud de Anticipo de Gastos de Viaje.", envioCorreo.getBodyGVNotificacionEnvioConfirmacionDeposito(solicitud, "AutorizarContabilidad"));

                    correos = new List<string>(); //correos TO   //usuario   

                    // ENVÍO A USUARIO
                    if (!String.IsNullOrEmpty(solicitud.empleados5.correo))
                        correos.Add(solicitud.empleados5.correo);
                    envioCorreo.SendEmailAsync(correos, "Confirmación de depósito correspondiente a solicitud de Acticipo de Gastos de Viaje #" + solicitud.id + ".", envioCorreo.getBodyGVNotificacionEnvioConfirmacionDeposito(solicitud, "ConfirmarUsuario"));

                    correos = new List<string>(); //correos TO   //usuario   

                    // ENVÍO A CONTROLLING
                    if (!String.IsNullOrEmpty(solicitud.empleados1.correo))
                        correos.Add(solicitud.empleados1.correo);
                    envioCorreo.SendEmailAsync(correos, "Confirmación de depósito correspondiente a solicitud de Acticipo de Gastos de Viaje #" + solicitud.id + ".", envioCorreo.getBodyGVNotificacionEnvioConfirmacionDeposito(solicitud, "Details"));


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
                    return RedirectToAction("SolicitudesNomina", new { estatus = s_estatus });

                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("SolicitudesNomina", new { estatus = s_estatus });
                }
                TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido confirmada.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("SolicitudesNomina", new { estatus = s_estatus });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: GV_solicitud/RechazarNomina/5
        [HttpPost, ActionName("RechazarNomina")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarNominaConfirmed(int? id, string s_estatus, string comentario_rechazo)
        {

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            GV_solicitud gv = db.GV_solicitud.Find(id);
            gv.estatus = GV_solicitud_estatus.RECHAZADO_NOMINA;
            gv.comentario_rechazo = comentario_rechazo;
            //borra las fechas 
            gv.fecha_aceptacion_jefe_area = null;
            gv.fecha_aceptacion_controlling = null;
            gv.fecha_aceptacion_contabilidad = null;
            gv.fecha_aceptacion_nomina = null;

            gv.id_nomina = empleado.id;

            //necesario para guardar en BD
            gv.medio_transporte_aplica_otro = !gv.id_medio_transporte.HasValue;

            db.Entry(gv).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO


                if (!String.IsNullOrEmpty(gv.empleados5.correo))
                    correos.Add(gv.empleados5.correo); //agrega correo de solicitantes

                envioCorreo.SendEmailAsync(correos, "Se ha rechazado su solicitud de anticipo gastos de viaje.", envioCorreo.getBodyGVNotificacionRechazo(gv, gv.empleados4.ConcatNombre));

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
                return RedirectToAction("SolicitudesNomina", new { estatus = s_estatus });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("SolicitudesNomina", new { estatus = s_estatus });
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido rechazada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("SolicitudesNomina", new { estatus = s_estatus });
        }
        #endregion

        #region CONTABILIDAD  

        // GET: GV_solicitud/AutorizarContabilidad/5
        public ActionResult AutorizarContabilidad(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_CONTABILIDAD))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            //obtiene la lista de usuarios asociados a controlling
            //var listUsuarios = db.GV_usuarios.Where(x => x.departamento == Bitacoras.Util.GV_tipo_departamento.CONTABILIDAD).Select(x => x.id_empleado).ToList();
            //ViewBag.id_contabilidad = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value && listUsuarios.Contains(x.id)), "id", "ConcatNumEmpleadoNombre")); //id de ismael

            return View(gV_solicitud);
        }

        // GET: GV_solicitud/AutorizarContabilidad/5
        [HttpPost, ActionName("AutorizarContabilidad")]
        [ValidateAntiForgeryToken]
        public ActionResult AutorizarContabilidadConfirmed(int? id, int? id_contabilidad, string s_estatus)
        {
            if (TieneRol(TipoRoles.GV_CONTABILIDAD))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_solicitud solicitud = db.GV_solicitud.Find(id);
                if (solicitud == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si se puede autorizar
                if (solicitud.estatus != GV_solicitud_estatus.CONFIRMADO_NOMINA && solicitud.estatus != GV_solicitud_estatus.CONFIRMADO_USUARIO)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede validar esta solicitud!";
                    ViewBag.Descripcion = "No se puede autorizar una solicitud que ya ha sido validada, rechazada o finalizada.";

                    return View("../Home/ErrorGenerico");
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //necesario para poder guardar los cambios
                solicitud.medio_transporte_aplica_otro = !solicitud.id_medio_transporte.HasValue;

                if (solicitud.fecha_confirmacion_usuario.HasValue)
                    solicitud.estatus = GV_solicitud_estatus.FINALIZADO;
                else
                    solicitud.estatus = GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD;

                solicitud.id_contabilidad = empleado.id;

                //establece las fechas 
                solicitud.fecha_aceptacion_contabilidad = DateTime.Now;


                db.Entry(solicitud).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    //envia correo electronico
                    if (solicitud.estatus == GV_solicitud_estatus.FINALIZADO)
                    {
                        //envia correo electronico
                        EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                        List<String> correos = new List<string>(); //correos TO


                        if (!String.IsNullOrEmpty(solicitud.empleados5.correo))
                            correos.Add(solicitud.empleados5.correo);   //correo del solictante
                        if (!String.IsNullOrEmpty(solicitud.empleados1.correo))
                            correos.Add(solicitud.empleados1.correo);   //correo de controlling

                        envioCorreo.SendEmailAsync(correos, "Se ha completado el proceso de la solicitud de Anticipo de Gastos de Viaje #" + solicitud.id + ".", envioCorreo.getBodyGVNotificacionFinalizado(solicitud));

                    }


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
                    return RedirectToAction("SolicitudesContabilidad", new { estatus = s_estatus });

                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("SolicitudesContabilidad", new { estatus = s_estatus });
                }
                TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido autorizada.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("SolicitudesContabilidad", new { estatus = s_estatus });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: GV_solicitud/RechazarContabilidad/5
        [HttpPost, ActionName("RechazarContabilidad")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarContabilidadConfirmed(int? id, string s_estatus, string comentario_rechazo)
        {

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            GV_solicitud gv = db.GV_solicitud.Find(id);
            gv.estatus = GV_solicitud_estatus.RECHAZADO_CONTABILIDAD;
            gv.comentario_rechazo = comentario_rechazo;
            //borra las fechas 
            gv.fecha_aceptacion_jefe_area = null;
            gv.fecha_aceptacion_controlling = null;
            gv.fecha_aceptacion_contabilidad = null;
            gv.fecha_aceptacion_nomina = null;
            gv.fecha_confirmacion_usuario = null;

            gv.id_contabilidad = empleado.id;

            //necesario para guardar en BD
            gv.medio_transporte_aplica_otro = !gv.id_medio_transporte.HasValue;

            db.Entry(gv).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO


                if (!String.IsNullOrEmpty(gv.empleados5.correo))
                    correos.Add(gv.empleados5.correo); //agrega correo de solicitantes

                envioCorreo.SendEmailAsync(correos, "Se ha rechazado su solicitud de anticipo gastos de viaje.", envioCorreo.getBodyGVNotificacionRechazo(gv, gv.empleados.ConcatNombre));

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
                return RedirectToAction("SolicitudesContabilidad", new { estatus = s_estatus });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("SolicitudesContabilidad", new { estatus = s_estatus });
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido rechazada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("SolicitudesContabilidad", new { estatus = s_estatus });
        }
        #endregion

        #region USUARIO  

        // GET: GV_solicitud/ConfirmarUsuario/5
        public ActionResult ConfirmarUsuario(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            //obtiene la lista de usuarios asociados a controlling
            //var listUsuarios = db.GV_usuarios.Where(x => x.departamento == Bitacoras.Util.GV_tipo_departamento.CONTABILIDAD).Select(x => x.id_empleado).ToList();
            //ViewBag.id_contabilidad = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value && listUsuarios.Contains(x.id)), "id", "ConcatNumEmpleadoNombre")); //id de ismael

            return View(gV_solicitud);
        }

        // GET: GV_solicitud/ConfirmarUsuario/5
        [HttpPost, ActionName("ConfirmarUsuario")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarUsuarioConfirmed(int? id, int? id_contabilidad, string s_estatus)
        {
            if (TieneRol(TipoRoles.GV_SOLICITUD))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_solicitud solicitud = db.GV_solicitud.Find(id);
                if (solicitud == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si se puede autorizar
                if (solicitud.estatus != GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD && solicitud.estatus != GV_solicitud_estatus.CONFIRMADO_NOMINA)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede confirmar esta solicitud!";
                    ViewBag.Descripcion = "No se puede autorizar una solicitud que ya ha sido confirmada, rechazada o finalizada.";

                    return View("../Home/ErrorGenerico");
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //necesario para poder guardar los cambios
                solicitud.medio_transporte_aplica_otro = !solicitud.id_medio_transporte.HasValue;

                if (solicitud.fecha_aceptacion_contabilidad.HasValue)
                    solicitud.estatus = GV_solicitud_estatus.FINALIZADO;
                else
                    solicitud.estatus = GV_solicitud_estatus.CONFIRMADO_USUARIO;


                //establece las fechas 
                solicitud.fecha_confirmacion_usuario = DateTime.Now;


                db.Entry(solicitud).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();

                    if (solicitud.estatus == GV_solicitud_estatus.FINALIZADO)
                    {
                        //envia correo electronico
                        EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                        List<String> correos = new List<string>(); //correos TO


                        if (!String.IsNullOrEmpty(solicitud.empleados5.correo))
                            correos.Add(solicitud.empleados5.correo);   //correo del solictante
                        if (!String.IsNullOrEmpty(solicitud.empleados1.correo))
                            correos.Add(solicitud.empleados1.correo);   //correo de controlling

                        envioCorreo.SendEmailAsync(correos, "Se ha completado el proceso de la solicitud de Anticipo de Gastos de Viaje #" + solicitud.id + ".", envioCorreo.getBodyGVNotificacionFinalizado(solicitud));

                    }

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
                    return RedirectToAction("Solicitudes", new { estatus = s_estatus });

                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Solicitudes", new { estatus = s_estatus });
                }
                TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido confirmada.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Solicitudes", new { estatus = s_estatus });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: GV_solicitud/RechazarUsuario/5
        [HttpPost, ActionName("RechazarUsuario")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarUsuarioConfirmed(int? id, string s_estatus, string comentario_rechazo)
        {

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            GV_solicitud gv = db.GV_solicitud.Find(id);
            gv.estatus = GV_solicitud_estatus.RECHAZADO_USUARIO;
            gv.comentario_rechazo = comentario_rechazo;
            //borra las fechas 
            //conserva las fechas de jefe y controlling
            //gv.fecha_aceptacion_jefe_area = null;
            //gv.fecha_aceptacion_controlling = null;
            gv.fecha_aceptacion_contabilidad = null;
            gv.fecha_aceptacion_nomina = null;
            gv.fecha_confirmacion_usuario = null;


            //necesario para guardar en BD
            gv.medio_transporte_aplica_otro = !gv.id_medio_transporte.HasValue;

            db.Entry(gv).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO


                if (!String.IsNullOrEmpty(gv.empleados4.correo))
                    correos.Add(gv.empleados4.correo); //agrega correo de solicitantes

                envioCorreo.SendEmailAsync(correos, "El usuario no ha confirmado el depósito de la solicitud de Anticipo de Gastos de Viaje #" + gv.id + "", envioCorreo.getBodyGVNotificacionRechazadoPorUsuario(gv));

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
                return RedirectToAction("Solicitudes", new { estatus = s_estatus });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Solicitudes", new { estatus = s_estatus });
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido rechazada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("Solicitudes", new { estatus = s_estatus });
        }
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
