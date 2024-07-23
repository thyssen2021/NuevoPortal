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
    public class SCDM_cat_commodityController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: SCDM_cat_commodity
        public ActionResult Index()
        {
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            return View(db.SCDM_cat_commodity.ToList());
        }



        // GET: SCDM_cat_commodity/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            return View();
        }

        // POST: SCDM_cat_commodity/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SCDM_cat_commodity SCDM_cat_commodity)
        {
            SCDM_cat_commodity.clave = SCDM_cat_commodity.clave.ToUpper();

            //verifica que no exista otro registro con la misma clave
            if (db.SCDM_cat_commodity.Any(x => x.clave == SCDM_cat_commodity.clave))
                ModelState.AddModelError("", "Ya existe un registro con la misma clave.");

            if (ModelState.IsValid)
            {
                SCDM_cat_commodity.activo = true;
                db.SCDM_cat_commodity.Add(SCDM_cat_commodity);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha agregado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            return View(SCDM_cat_commodity);
        }

        // GET: SCDM_cat_commodity/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_commodity SCDM_cat_commodity = db.SCDM_cat_commodity.Find(id);
            if (SCDM_cat_commodity == null)
            {
                return HttpNotFound();
            }
            return View(SCDM_cat_commodity);
        }

        // POST: SCDM_cat_commodity/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SCDM_cat_commodity SCDM_cat_commodity)
        {

            SCDM_cat_commodity.clave = SCDM_cat_commodity.clave.ToUpper();

            //verifica que no exista otro registro con la misma clave
            if (db.SCDM_cat_commodity.Any(x => x.clave == SCDM_cat_commodity.clave && x.id != SCDM_cat_commodity.id))
                ModelState.AddModelError("", "Ya existe un registro con la misma clave.");



            if (ModelState.IsValid)
            {
                db.Entry(SCDM_cat_commodity).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha editado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }
            return View(SCDM_cat_commodity);
        }

        // GET: SCDM_cat_commodity/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_commodity SCDM_cat_commodity = db.SCDM_cat_commodity.Find(id);
            if (SCDM_cat_commodity == null)
            {
                return HttpNotFound();
            }
            return View(SCDM_cat_commodity);
        }

        // POST: SCDM_cat_commodity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SCDM_cat_commodity SCDM_cat_commodity = db.SCDM_cat_commodity.Find(id);
            db.SCDM_cat_commodity.Remove(SCDM_cat_commodity);
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
