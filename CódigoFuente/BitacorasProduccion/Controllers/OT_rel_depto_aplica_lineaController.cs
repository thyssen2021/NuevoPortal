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
    public class OT_rel_depto_aplica_lineaController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: OT_rel_depto_aplica_linea
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                var oT_rel_depto_aplica_linea = db.OT_rel_depto_aplica_linea.Include(o => o.Area);
                return View(oT_rel_depto_aplica_linea.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
           
        }

        

        // GET: OT_rel_depto_aplica_linea/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                ViewBag.id_area = new SelectList(db.Area, "clave", "ConcatDeptoPlanta");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }            
        }

        // POST: OT_rel_depto_aplica_linea/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_area")] OT_rel_depto_aplica_linea oT_rel_depto_aplica_linea)
        {

            //busca si existe in departamento con la misma descripcion
            bool existePrevio = db.OT_rel_depto_aplica_linea.Any(s => s.id_area == oT_rel_depto_aplica_linea.id_area);

            if (existePrevio)
                ModelState.AddModelError("", "Ya existe este departamento");

            if (ModelState.IsValid)
            {

                db.OT_rel_depto_aplica_linea.Add(oT_rel_depto_aplica_linea);

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_area = new SelectList(db.Area, "clave", "descripcion", oT_rel_depto_aplica_linea.id_area);
            return View(oT_rel_depto_aplica_linea);
        }

      

        // GET: OT_rel_depto_aplica_linea/Delete/5
        public ActionResult Delete(int? id)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OT_rel_depto_aplica_linea oT_rel_depto_aplica_linea = db.OT_rel_depto_aplica_linea.Find(id);
                if (oT_rel_depto_aplica_linea == null)
                {
                    return HttpNotFound();
                }
                return View(oT_rel_depto_aplica_linea);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // POST: OT_rel_depto_aplica_linea/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OT_rel_depto_aplica_linea oT_rel_depto_aplica_linea = db.OT_rel_depto_aplica_linea.Find(id);
            db.OT_rel_depto_aplica_linea.Remove(oT_rel_depto_aplica_linea);
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
