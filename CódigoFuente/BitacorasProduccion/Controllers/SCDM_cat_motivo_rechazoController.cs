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
    public class SCDM_cat_motivo_rechazoController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: SCDM_cat_motivo_rechazo
        public ActionResult Index()
        {
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            return View(db.SCDM_cat_motivo_rechazo.ToList());
        }



        // GET: SCDM_cat_motivo_rechazo/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            return View();
        }

        // POST: SCDM_cat_motivo_rechazo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SCDM_cat_motivo_rechazo SCDM_cat_motivo_rechazo)
        {


            //verifica que no exista otro registro con la misma clave
            if (db.SCDM_cat_motivo_rechazo.Any(x => x.descripcion.ToUpper() == SCDM_cat_motivo_rechazo.descripcion.ToUpper()))
                ModelState.AddModelError("", "Ya existe un registro con la misma descripción.");

            if (ModelState.IsValid)
            {
                SCDM_cat_motivo_rechazo.activo = true;
                db.SCDM_cat_motivo_rechazo.Add(SCDM_cat_motivo_rechazo);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha agregado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            return View(SCDM_cat_motivo_rechazo);
        }

        // GET: SCDM_cat_motivo_rechazo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_motivo_rechazo SCDM_cat_motivo_rechazo = db.SCDM_cat_motivo_rechazo.Find(id);
            if (SCDM_cat_motivo_rechazo == null)
            {
                return HttpNotFound();
            }
            return View(SCDM_cat_motivo_rechazo);
        }

        // POST: SCDM_cat_motivo_rechazo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SCDM_cat_motivo_rechazo SCDM_cat_motivo_rechazo)
        {

            //verifica que no exista otro registro con la misma clave
            if (db.SCDM_cat_motivo_rechazo.Any(x => x.descripcion.ToUpper() == SCDM_cat_motivo_rechazo.descripcion.ToUpper() && x.id != SCDM_cat_motivo_rechazo.id))
                ModelState.AddModelError("", "Ya existe un registro con la misma clave.");

            if (ModelState.IsValid)
            {
                db.Entry(SCDM_cat_motivo_rechazo).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha editado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }
            return View(SCDM_cat_motivo_rechazo);
        }

        // GET: SCDM_cat_motivo_rechazo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_motivo_rechazo SCDM_cat_motivo_rechazo = db.SCDM_cat_motivo_rechazo.Find(id);
            if (SCDM_cat_motivo_rechazo == null)
            {
                return HttpNotFound();
            }
            return View(SCDM_cat_motivo_rechazo);
        }

        // POST: SCDM_cat_motivo_rechazo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SCDM_cat_motivo_rechazo SCDM_cat_motivo_rechazo = db.SCDM_cat_motivo_rechazo.Find(id);
            db.SCDM_cat_motivo_rechazo.Remove(SCDM_cat_motivo_rechazo);
            db.SaveChanges();
            TempData["Mensaje"] = new MensajesSweetAlert("Se ha borrado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

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
