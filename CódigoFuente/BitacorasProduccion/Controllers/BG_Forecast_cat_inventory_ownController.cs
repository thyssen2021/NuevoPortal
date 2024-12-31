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
    public class BG_Forecast_cat_inventory_ownController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_Forecast_cat_inventory_own
        public ActionResult Index(int? id_reporte)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var bG_Forecast_cat_inventory_own = db.BG_Forecast_cat_inventory_own.Where(x => x.id_bg_forecast_reporte == id_reporte).OrderBy(x => x.orden);

            ViewBag.reporte = db.BG_Forecast_reporte.Find(id_reporte);

            return View(bG_Forecast_cat_inventory_own.ToList());
        }

        // GET: BG_Forecast_cat_inventory_own/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BG_Forecast_reporte reporte = db.BG_Forecast_reporte.Find(id);
            if (reporte == null)
            {
                return HttpNotFound();
            }
            //ViewBag.id_bg_forecast_reporte = new SelectList(db.BG_Forecast_reporte, "id", "descripcion", reporte.id_bg_forecast_reporte);
            return View(reporte);
        }

        // POST: BG_Forecast_cat_inventory_own/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BG_Forecast_reporte reporte)
        {
            bool valido = true;

            //agregar comprobaciones al modelo en caso de ser necesario

            if (valido)
            {
                var listBD = db.BG_Forecast_cat_inventory_own.Where(x => x.id_bg_forecast_reporte == reporte.id);

                foreach (var item in reporte.BG_Forecast_cat_inventory_own)
                {
                    var itemBD = listBD.FirstOrDefault(x => x.id == item.id);
                    itemBD.cantidad = item.cantidad;
                }

                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert("El catálogo de Inventory OWN se actualizó correctamente", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index", new { id_reporte = reporte.id });
            }

            return View(db.BG_Forecast_reporte.Find(reporte.id));
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
