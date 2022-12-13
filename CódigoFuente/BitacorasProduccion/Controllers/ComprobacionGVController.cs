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

            //en caso de solicitudes pendientes manda a otra vista
            if (estatus == "PENDIENTES")
                return RedirectToAction("Pendientes", new { estatus = estatus });


            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();

            var listado = db.GV_comprobacion
                .Where(x => (x.GV_solicitud.id_empleado == empleado.id || x.GV_solicitud.id_solicitante == empleado.id)
                    && (x.estatus == estatus || estatus == "ALL"
                                    || (estatus == "EN_PROCESO" && (x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                    ))
                                    || (estatus == "RECHAZADAS" && (x.estatus == GV_solicitud_estatus.RECHAZADO_JEFE
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_CONTROLLING
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_NOMINA || x.estatus == GV_solicitud_estatus.RECHAZADO_CONTABILIDAD))
                                                    )
                )
                .OrderByDescending(x => x.GV_solicitud.fecha_solicitud)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_comprobacion
                  .Where(x => (x.GV_solicitud.id_empleado == empleado.id || x.GV_solicitud.id_solicitante == empleado.id)
                    && (x.estatus == estatus || estatus == "ALL"
                                    || (estatus == "EN_PROCESO" && (x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                    ))
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
            newList.Add(new SelectListItem() { Text = "Pendientes", Value = "PENDIENTES" });
            newList.Add(new SelectListItem() { Text = "En proceso", Value = "EN_PROCESO" });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = "RECHAZADAS" });
            newList.Add(new SelectListItem() { Text = "Finalizadas", Value = Bitacoras.Util.GV_comprobacion_estatus.FINALIZADO });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;
            ViewBag.SegundoNivel = "GV_solicitud";
            ViewBag.Title = "Mis Solicitudes";


            //ordenar por fecha de solicitud más reciente
            return View("solicitudes", listado);

        }

        public ActionResult Pendientes(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            //en caso de solicitudes pendientes manda a otra vista
            if (estatus != "PENDIENTES")
                return RedirectToAction("Solicitudes", new { estatus = estatus });

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();

            var listado = db.GV_solicitud
                .Where(x => (x.id_empleado == empleado.id || x.id_solicitante == empleado.id)
                    && x.estatus == GV_solicitud_estatus.FINALIZADO && x.GV_comprobacion == null
                )
                .OrderByDescending(x => x.fecha_solicitud)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_solicitud
                   .Where(x => (x.id_empleado == empleado.id || x.id_solicitante == empleado.id)
                    && x.estatus == GV_solicitud_estatus.FINALIZADO && x.GV_comprobacion == null
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
            newList.Add(new SelectListItem() { Text = "Pendientes", Value = "PENDIENTES" });
            newList.Add(new SelectListItem() { Text = "En proceso", Value = "EN_PROCESO" });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = "RECHAZADAS" });
            newList.Add(new SelectListItem() { Text = "Finalizadas", Value = Bitacoras.Util.GV_comprobacion_estatus.FINALIZADO });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;


            //ordenar por fecha de solicitud más reciente
            return View("pendientes", listado);

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
            ViewBag.plantaClave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccione un valor --");
            ViewBag.id_centro_costo =  AddFirstItem(new SelectList(db.GV_centros_costo.Where(p => p.activo == true), nameof(GV_centros_costo.id), nameof(GV_centros_costo.ConcatCentroDepto)), textoPorDefecto: "-- Seleccione un valor --");

            return View(gV_solicitud); ;
        }

        // POST: OrdenesTrabajo/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ComprobacionGastos(GV_solicitud solicitud)
        {

           // ModelState.AddModelError("", "Error prueba.");
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha creado el registro correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Solicitudes", new { estatus="EN_PROCESO"});
            }

            //En caso de que el modelo no sea válido
            //guarda de forma temporal la comprobacion recibida en el formulario y lo asigna el modelo completo
            GV_comprobacion gV_ComprobacionRecibida = solicitud.GV_comprobacion;
            solicitud = db.GV_solicitud.Find(solicitud.id);
            solicitud.GV_comprobacion = gV_ComprobacionRecibida;

            //determina si aplica otro o no
            solicitud.medio_transporte_aplica_otro = !solicitud.id_medio_transporte.HasValue;
     
            ViewBag.plantaClave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccione un valor --", selected: gV_ComprobacionRecibida.clave_planta.ToString());
            ViewBag.id_centro_costo = AddFirstItem(new SelectList(db.GV_centros_costo.Where(p => p.activo == true && p.clave_planta == solicitud.GV_comprobacion.clave_planta), nameof(GV_centros_costo.id), nameof(GV_centros_costo.ConcatCentroDepto)), textoPorDefecto: "-- Seleccione un valor --", selected: gV_ComprobacionRecibida.id_centro_costo.ToString());

            return View(solicitud);
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
