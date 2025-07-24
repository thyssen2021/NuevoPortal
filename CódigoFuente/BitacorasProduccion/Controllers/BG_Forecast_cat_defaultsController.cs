using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BG_Forecast_cat_defaultsController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_Forecast_cat_defaults
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            BG_Forecast_cat_defaults bG_Forecast_cat_defaults = db.BG_Forecast_cat_defaults.Find(1);
            if (bG_Forecast_cat_defaults == null)
            {
                return HttpNotFound();
            }
            return View(bG_Forecast_cat_defaults);
        }


        // GET: BG_Forecast_cat_defaults/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BG_Forecast_cat_defaults bG_Forecast_cat_defaults = db.BG_Forecast_cat_defaults.Find(id);
        //    if (bG_Forecast_cat_defaults == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bG_Forecast_cat_defaults);
        //}

        //// GET: BG_Forecast_cat_defaults/Create
        //public ActionResult Create()
        //{
        //    if (!TieneRol(TipoRoles.BUDGET_IHS))
        //        return View("../Home/ErrorPermisos");

        //    return View();
        //}

        //// POST: BG_Forecast_cat_defaults/Create
        //// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        //// más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(BG_Forecast_cat_defaults bG_Forecast_cat_defaults)
        //{
        //    if (!TieneRol(TipoRoles.BUDGET_IHS))
        //        return View("../Home/ErrorPermisos");

        //    if (ModelState.IsValid)
        //    {
        //        db.BG_Forecast_cat_defaults.Add(bG_Forecast_cat_defaults);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(bG_Forecast_cat_defaults);
        //}

        // GET: BG_Forecast_cat_defaults/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_cat_defaults bG_Forecast_cat_defaults = db.BG_Forecast_cat_defaults.Find(id);
            if (bG_Forecast_cat_defaults == null)
            {
                return HttpNotFound();
            }
            return View(bG_Forecast_cat_defaults);
        }

        // POST: BG_Forecast_cat_defaults/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BG_Forecast_cat_defaults bG_Forecast_cat_defaults)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bG_Forecast_cat_defaults).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            return View(bG_Forecast_cat_defaults);
        }

        // GET: BG_Forecast_cat_defaults/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BG_Forecast_cat_defaults bG_Forecast_cat_defaults = db.BG_Forecast_cat_defaults.Find(id);
        //    if (bG_Forecast_cat_defaults == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bG_Forecast_cat_defaults);
        //}

        //// POST: BG_Forecast_cat_defaults/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    BG_Forecast_cat_defaults bG_Forecast_cat_defaults = db.BG_Forecast_cat_defaults.Find(id);
        //    db.BG_Forecast_cat_defaults.Remove(bG_Forecast_cat_defaults);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
