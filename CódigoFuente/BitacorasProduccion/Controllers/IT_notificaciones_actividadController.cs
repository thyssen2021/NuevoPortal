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
    public class IT_notificaciones_actividadController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_notificaciones_actividad
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.IT_NOTIFICACIONES))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            return View(db.IT_notificaciones_actividad.ToList());
        }

        // GET: IT_notificaciones_actividad/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_notificaciones_actividad iT_notificaciones_actividad = db.IT_notificaciones_actividad.Find(id);
            if (iT_notificaciones_actividad == null)
            {
                return HttpNotFound();
            }
            return View(iT_notificaciones_actividad);
        }

        // GET: IT_notificaciones_actividad/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IT_notificaciones_actividad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,periodo,tipo_periodo,es_recurrente,mensaje,asunto")] IT_notificaciones_actividad iT_notificaciones_actividad)
        {
            if (ModelState.IsValid)
            {
                db.IT_notificaciones_actividad.Add(iT_notificaciones_actividad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(iT_notificaciones_actividad);
        }

        // GET: IT_notificaciones_actividad/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_notificaciones_actividad iT_notificaciones_actividad = db.IT_notificaciones_actividad.Find(id);
            if (iT_notificaciones_actividad == null)
            {
                return HttpNotFound();
            }
            return View(iT_notificaciones_actividad);
        }

        // POST: IT_notificaciones_actividad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,periodo,tipo_periodo,es_recurrente,mensaje,asunto")] IT_notificaciones_actividad iT_notificaciones_actividad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iT_notificaciones_actividad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(iT_notificaciones_actividad);
        }

        // GET: IT_notificaciones_actividad/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_notificaciones_actividad iT_notificaciones_actividad = db.IT_notificaciones_actividad.Find(id);
            if (iT_notificaciones_actividad == null)
            {
                return HttpNotFound();
            }
            return View(iT_notificaciones_actividad);
        }

        // POST: IT_notificaciones_actividad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IT_notificaciones_actividad iT_notificaciones_actividad = db.IT_notificaciones_actividad.Find(id);
            db.IT_notificaciones_actividad.Remove(iT_notificaciones_actividad);
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
