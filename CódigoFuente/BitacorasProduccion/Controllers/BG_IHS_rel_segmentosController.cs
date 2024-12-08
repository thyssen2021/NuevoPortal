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
    public class BG_IHS_rel_segmentosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_IHS_rel_segmentos
        public ActionResult Index(string estado)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS_CATALOGOS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var BG_IHS_rel_segmentos = db.BG_IHS_rel_segmentos.Where(x => String.IsNullOrEmpty(estado)
                                                            || (estado == "PENDIENTE" && (!x.flat_rolled_steel_usage.HasValue || !x.blanks_usage_percent.HasValue))
                                                            || (estado == "ASIGNADO" && (x.flat_rolled_steel_usage.HasValue || x.blanks_usage_percent.HasValue))
                                                            );


            // -- tipo de listado ORIGEN
            List<SelectListItem> selectListEstado = new List<SelectListItem>();
            selectListEstado.Add(new SelectListItem() { Text = "Asignado", Value = "ASIGNADO" });
            selectListEstado.Add(new SelectListItem() { Text = "Pendiente", Value = "PENDIENTE" });
            ViewBag.estado = AddFirstItem(new SelectList(selectListEstado, "Value", "Text", estado), textoPorDefecto: "-- Todos --");

            return View(BG_IHS_rel_segmentos.ToList());
        }

        // GET: BG_IHS_rel_segmentos/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS_CATALOGOS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_rel_segmentos BG_IHS_rel_segmentos = db.BG_IHS_rel_segmentos.Find(id);
            if (BG_IHS_rel_segmentos == null)
            {
                return HttpNotFound();
            }
            return View(BG_IHS_rel_segmentos);
        }



        // GET: BG_IHS_rel_segmentos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS_CATALOGOS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_rel_segmentos BG_IHS_rel_segmentos = db.BG_IHS_rel_segmentos.Find(id);
            if (BG_IHS_rel_segmentos == null)
            {
                return HttpNotFound();
            }
            return View(BG_IHS_rel_segmentos);
        }

        // POST: BG_IHS_rel_segmentos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BG_IHS_rel_segmentos BG_IHS_rel_segmentos)
        {
            if (ModelState.IsValid)
            {
                //coloca el porcentaje en decimales
                BG_IHS_rel_segmentos.blanks_usage_percent = BG_IHS_rel_segmentos.blanks_usage_percent / 100;

                db.Entry(BG_IHS_rel_segmentos).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            return View(BG_IHS_rel_segmentos);
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
