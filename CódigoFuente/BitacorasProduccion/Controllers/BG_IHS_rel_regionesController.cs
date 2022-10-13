
using Clases.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Models
{
    [Authorize]
    public class BG_IHS_rel_regionesController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_IHS_rel_regiones
        public ActionResult Index(string estado)
        {
            if (!TieneRol(TipoRoles.GV_CATALOGOS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var bG_IHS_rel_regiones = db.BG_IHS_rel_regiones.Where(x => String.IsNullOrEmpty(estado)
                                                            || (estado == "PENDIENTE" && !x.id_bg_ihs_region.HasValue)
                                                            || (estado == "ASIGNADO" && x.id_bg_ihs_region.HasValue)
                                                            );

            // -- tipo de listado ORIGEN
            List<SelectListItem> selectListEstado = new List<SelectListItem>();
            selectListEstado.Add(new SelectListItem() { Text = "Asignado", Value = "ASIGNADO" });
            selectListEstado.Add(new SelectListItem() { Text = "Pendiente", Value = "PENDIENTE" });
            ViewBag.estado = AddFirstItem(new SelectList(selectListEstado, "Value", "Text", estado), textoPorDefecto: "-- Todos --");

            return View(bG_IHS_rel_regiones.ToList());
        }

        // GET: BG_IHS_rel_regiones/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.GV_CATALOGOS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_rel_regiones bG_IHS_rel_regiones = db.BG_IHS_rel_regiones.Find(id);
            if (bG_IHS_rel_regiones == null)
            {
                return HttpNotFound();
            }
            return View(bG_IHS_rel_regiones);
        }



        // GET: BG_IHS_rel_regiones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.GV_CATALOGOS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_rel_regiones bG_IHS_rel_regiones = db.BG_IHS_rel_regiones.Find(id);
            if (bG_IHS_rel_regiones == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_bg_ihs_region = AddFirstItem(new SelectList(db.BG_IHS_regiones.Where(x => x.activo == true), nameof(BG_IHS_regiones.id), nameof(BG_IHS_regiones.descripcion), bG_IHS_rel_regiones.id_bg_ihs_region.ToString()), textoPorDefecto: "-- Seleccionar --");
            return View(bG_IHS_rel_regiones);
        }

        // POST: BG_IHS_rel_regiones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BG_IHS_rel_regiones bG_IHS_rel_regiones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bG_IHS_rel_regiones).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }
            ViewBag.id_bg_ihs_region = AddFirstItem(new SelectList(db.BG_IHS_regiones.Where(x => x.activo == true), nameof(BG_IHS_regiones.id), nameof(BG_IHS_regiones.descripcion), bG_IHS_rel_regiones.id_bg_ihs_region.ToString()), textoPorDefecto: "-- Seleccionar --");

            return View(bG_IHS_rel_regiones);
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
