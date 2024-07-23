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
    public class SCDM_cat_grado_calidadController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: SCDM_cat_grado_calidad
        public ActionResult Index()
        {
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            return View(db.SCDM_cat_grado_calidad.ToList());
        }

    

        // GET: SCDM_cat_grado_calidad/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            return View();
        }

        // POST: SCDM_cat_grado_calidad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SCDM_cat_grado_calidad SCDM_cat_grado_calidad)
        {
            //convierte clave a numero de minimo dos digitos
            if (!int.TryParse(SCDM_cat_grado_calidad.clave, out int clave))
                ModelState.AddModelError("", "La clave no es válida.");          
           

            //verifica que no exista otro grado/calidad con la misma clave
            if (db.SCDM_cat_grado_calidad.Any(x=>x.clave == SCDM_cat_grado_calidad.clave))
                ModelState.AddModelError("", "Ya existe un grado/calidad con la misma clave.");

            if (ModelState.IsValid)
            {
                SCDM_cat_grado_calidad.activo = true;
                db.SCDM_cat_grado_calidad.Add(SCDM_cat_grado_calidad);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha agregado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            return View(SCDM_cat_grado_calidad);
        }

        // GET: SCDM_cat_grado_calidad/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_grado_calidad SCDM_cat_grado_calidad = db.SCDM_cat_grado_calidad.Find(id);
            if (SCDM_cat_grado_calidad == null)
            {
                return HttpNotFound();
            }
            return View(SCDM_cat_grado_calidad);
        }

        // POST: SCDM_cat_grado_calidad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SCDM_cat_grado_calidad SCDM_cat_grado_calidad)
        {

            //convierte clave a numero de minimo dos digitos
            if (!int.TryParse(SCDM_cat_grado_calidad.clave, out int clave))
                ModelState.AddModelError("", "La clave no es válida.");
       
            //verifica que no exista otro grado/calidad con la misma clave
            if (db.SCDM_cat_grado_calidad.Any(x => x.clave == SCDM_cat_grado_calidad.clave && x.id!= SCDM_cat_grado_calidad.id))
                ModelState.AddModelError("", "Ya existe un grado/calidad con la misma clave.");


            if (ModelState.IsValid)
            {
                db.Entry(SCDM_cat_grado_calidad).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha editado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }
            return View(SCDM_cat_grado_calidad);
        }

        // GET: SCDM_cat_grado_calidad/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_grado_calidad SCDM_cat_grado_calidad = db.SCDM_cat_grado_calidad.Find(id);
            if (SCDM_cat_grado_calidad == null)
            {
                return HttpNotFound();
            }
            return View(SCDM_cat_grado_calidad);
        }

        // POST: SCDM_cat_grado_calidad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SCDM_cat_grado_calidad SCDM_cat_grado_calidad = db.SCDM_cat_grado_calidad.Find(id);
            db.SCDM_cat_grado_calidad.Remove(SCDM_cat_grado_calidad);
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
