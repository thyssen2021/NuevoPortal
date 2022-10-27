using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class ComprobacionGVController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: ComprobacionGV    
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
                PaginaActual = 1,
                TotalDeRegistros = 1,
                RegistrosPorPagina = 20,
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
        // GET: ComprobacionGV/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        
        // GET: ComprobacionGV/Details/5
        public ActionResult ComprobacionGastos(int? id)
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

            return View(gV_solicitud); ;
        }

        // GET: ComprobacionGV/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ComprobacionGV/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ComprobacionGV/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ComprobacionGV/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ComprobacionGV/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ComprobacionGV/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
