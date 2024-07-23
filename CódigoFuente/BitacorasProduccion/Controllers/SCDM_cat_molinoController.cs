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
    public class SCDM_cat_molinoController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: SCDM_cat_molino
        public ActionResult Index()
        {
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            return View(db.SCDM_cat_molino.ToList());
        }

    

        // GET: SCDM_cat_molino/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            return View();
        }

        // POST: SCDM_cat_molino/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SCDM_cat_molino sCDM_cat_molino)
        {
            //convierte clave a numero de minimo dos digitos
            if (!int.TryParse(sCDM_cat_molino.clave, out int clave))
                ModelState.AddModelError("", "La clave no es válida.");
            else  
                sCDM_cat_molino.clave = clave.ToString("D2");


            //verifica que no exista otro registro con la misma clave
            if (db.SCDM_cat_molino.Any(x=>x.clave == sCDM_cat_molino.clave))
                ModelState.AddModelError("", "Ya existe un registro con la misma clave.");

            if (ModelState.IsValid)
            {
                sCDM_cat_molino.activo = true;
                db.SCDM_cat_molino.Add(sCDM_cat_molino);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha agregado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            return View(sCDM_cat_molino);
        }

        // GET: SCDM_cat_molino/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_molino sCDM_cat_molino = db.SCDM_cat_molino.Find(id);
            if (sCDM_cat_molino == null)
            {
                return HttpNotFound();
            }
            return View(sCDM_cat_molino);
        }

        // POST: SCDM_cat_molino/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SCDM_cat_molino sCDM_cat_molino)
        {

            //convierte clave a numero de minimo dos digitos
            if (!int.TryParse(sCDM_cat_molino.clave, out int clave))
                ModelState.AddModelError("", "La clave no es válida.");
            else
                sCDM_cat_molino.clave = clave.ToString("D2");

            //verifica que no exista otro registro con la misma clave
            if (db.SCDM_cat_molino.Any(x => x.clave == sCDM_cat_molino.clave && x.id!= sCDM_cat_molino.id))
                ModelState.AddModelError("", "Ya existe un registro con la misma clave.");



            if (ModelState.IsValid)
            {
                db.Entry(sCDM_cat_molino).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha editado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }
            return View(sCDM_cat_molino);
        }

        // GET: SCDM_cat_molino/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_molino sCDM_cat_molino = db.SCDM_cat_molino.Find(id);
            if (sCDM_cat_molino == null)
            {
                return HttpNotFound();
            }
            return View(sCDM_cat_molino);
        }

        // POST: SCDM_cat_molino/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SCDM_cat_molino sCDM_cat_molino = db.SCDM_cat_molino.Find(id);
            db.SCDM_cat_molino.Remove(sCDM_cat_molino);
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
