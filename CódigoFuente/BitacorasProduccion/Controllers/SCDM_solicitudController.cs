using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class SCDM_solicitudController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: SCDM_solicitud
        public ActionResult Index(int pagina = 1)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) && !TieneRol(TipoRoles.SCDM_MM_APROBACION_SOLICITUDES) &&
                !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES) && !TieneRol(TipoRoles.SCDM_MM_REPORTES))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro

            var listado = db.SCDM_solicitud
                   // .Where(x => x.id_elaborador == empleado.id && x.estatus == PM_Status.CREADO)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.SCDM_solicitud
                //.Where(x => x.id_elaborador == empleado.id && x.estatus == PM_Status.CREADO)
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
                      
            var sCDM_solicitud = db.SCDM_solicitud;
            return View(sCDM_solicitud.ToList());
        }

        // GET: SCDM_solicitud/Details/5
        public ActionResult Details(int? id)
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

        // GET: SCDM_solicitud/Create
        public ActionResult Create()
        {
            ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado");
            ViewBag.id_prioridad = new SelectList(db.SCDM_cat_prioridad, "id", "descripcion");
            ViewBag.id_tipo_cambio = new SelectList(db.SCDM_cat_tipo_cambio, "id", "descripcion");
            ViewBag.id_tipo_solicitud = new SelectList(db.SCDM_cat_tipo_solicitud, "id", "descripcion");
            return View();
        }

        // POST: SCDM_solicitud/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_tipo_solicitud,id_tipo_cambio,id_prioridad,id_solicitante,descripcion,justificacion,fecha_creacion,on_hold,activo")] SCDM_solicitud sCDM_solicitud)
        {
            if (ModelState.IsValid)
            {
                db.SCDM_solicitud.Add(sCDM_solicitud);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", sCDM_solicitud.id_solicitante);
            ViewBag.id_prioridad = new SelectList(db.SCDM_cat_prioridad, "id", "descripcion", sCDM_solicitud.id_prioridad);
            ViewBag.id_tipo_cambio = new SelectList(db.SCDM_cat_tipo_cambio, "id", "descripcion", sCDM_solicitud.id_tipo_cambio);
            ViewBag.id_tipo_solicitud = new SelectList(db.SCDM_cat_tipo_solicitud, "id", "descripcion", sCDM_solicitud.id_tipo_solicitud);
            return View(sCDM_solicitud);
        }

        // GET: SCDM_solicitud/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", sCDM_solicitud.id_solicitante);
            ViewBag.id_prioridad = new SelectList(db.SCDM_cat_prioridad, "id", "descripcion", sCDM_solicitud.id_prioridad);
            ViewBag.id_tipo_cambio = new SelectList(db.SCDM_cat_tipo_cambio, "id", "descripcion", sCDM_solicitud.id_tipo_cambio);
            ViewBag.id_tipo_solicitud = new SelectList(db.SCDM_cat_tipo_solicitud, "id", "descripcion", sCDM_solicitud.id_tipo_solicitud);
            return View(sCDM_solicitud);
        }

        // POST: SCDM_solicitud/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_tipo_solicitud,id_tipo_cambio,id_prioridad,id_solicitante,descripcion,justificacion,fecha_creacion,on_hold,activo")] SCDM_solicitud sCDM_solicitud)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sCDM_solicitud).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", sCDM_solicitud.id_solicitante);
            ViewBag.id_prioridad = new SelectList(db.SCDM_cat_prioridad, "id", "descripcion", sCDM_solicitud.id_prioridad);
            ViewBag.id_tipo_cambio = new SelectList(db.SCDM_cat_tipo_cambio, "id", "descripcion", sCDM_solicitud.id_tipo_cambio);
            ViewBag.id_tipo_solicitud = new SelectList(db.SCDM_cat_tipo_solicitud, "id", "descripcion", sCDM_solicitud.id_tipo_solicitud);
            return View(sCDM_solicitud);
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
    }
}
