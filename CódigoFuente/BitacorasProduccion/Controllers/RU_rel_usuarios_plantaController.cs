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
    public class RU_rel_usuarios_plantaController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: RU_rel_usuarios_planta
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var rU_rel_usuarios_planta = db.RU_rel_usuarios_planta.Include(r => r.AspNetUsers).Include(r => r.plantas);
            return View(rU_rel_usuarios_planta.ToList());
        }

        // GET: RU_rel_usuarios_planta/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RU_rel_usuarios_planta rU_rel_usuarios_planta = db.RU_rel_usuarios_planta.Find(id);
            if (rU_rel_usuarios_planta == null)
            {
                return HttpNotFound();
            }
            return View(rU_rel_usuarios_planta);
        }

        // GET: RU_rel_usuarios_planta/Create
        public ActionResult Create()
        {
            

            ViewBag.id_usuario = AddFirstItem(new SelectList(db.AspNetUsers, nameof(AspNetUsers.Id), nameof(AspNetUsers.UserName)));
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, nameof(plantas.clave), nameof(plantas.descripcion)));


            return View();
        }

        // POST: RU_rel_usuarios_planta/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RU_rel_usuarios_planta rU_rel_usuarios_planta)
        {

            bool existe = db.RU_rel_usuarios_planta.Any(x => x.id_planta == rU_rel_usuarios_planta.id_planta && x.id_usuario == rU_rel_usuarios_planta.id_usuario);
            if(existe)
                ModelState.AddModelError("", "Ya existe la combinacion Usuario-Planta");


            if (ModelState.IsValid)
            {
                db.RU_rel_usuarios_planta.Add(rU_rel_usuarios_planta);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se agregó correctamente el usuario.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario = AddFirstItem(new SelectList(db.AspNetUsers, nameof(AspNetUsers.Id), nameof(AspNetUsers.UserName)), selected: rU_rel_usuarios_planta.id_usuario);
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, nameof(plantas.clave), nameof(plantas.descripcion)), selected: rU_rel_usuarios_planta.id_planta.ToString());
            return View(rU_rel_usuarios_planta);
        }

        // GET: RU_rel_usuarios_planta/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RU_rel_usuarios_planta rU_rel_usuarios_planta = db.RU_rel_usuarios_planta.Find(id);
            if (rU_rel_usuarios_planta == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario = AddFirstItem(new SelectList(db.AspNetUsers, nameof(AspNetUsers.Id), nameof(AspNetUsers.UserName)), selected: rU_rel_usuarios_planta.id_usuario);
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, nameof(plantas.clave), nameof(plantas.descripcion)), selected: rU_rel_usuarios_planta.id_planta.ToString());
            return View(rU_rel_usuarios_planta);
        }

        // POST: RU_rel_usuarios_planta/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( RU_rel_usuarios_planta rU_rel_usuarios_planta)
        {

            bool existe = db.RU_rel_usuarios_planta.Any(x => x.id_planta == rU_rel_usuarios_planta.id_planta && x.id_usuario == rU_rel_usuarios_planta.id_usuario && x.id != rU_rel_usuarios_planta.id);
            if (existe)
                ModelState.AddModelError("", "Ya existe la combinacion Usuario-Planta");

            if (ModelState.IsValid)
            {
                db.Entry(rU_rel_usuarios_planta).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se editó correctamente el usuario.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }
            ViewBag.id_usuario = AddFirstItem(new SelectList(db.AspNetUsers, nameof(AspNetUsers.Id), nameof(AspNetUsers.UserName)), selected: rU_rel_usuarios_planta.id_usuario);
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, nameof(plantas.clave), nameof(plantas.descripcion)), selected: rU_rel_usuarios_planta.id_planta.ToString());
            return View(rU_rel_usuarios_planta);
        }

        // GET: RU_rel_usuarios_planta/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RU_rel_usuarios_planta rU_rel_usuarios_planta = db.RU_rel_usuarios_planta.Find(id);
            if (rU_rel_usuarios_planta == null)
            {
                return HttpNotFound();
            }
            return View(rU_rel_usuarios_planta);
        }

        // POST: RU_rel_usuarios_planta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RU_rel_usuarios_planta rU_rel_usuarios_planta = db.RU_rel_usuarios_planta.Find(id);
            db.RU_rel_usuarios_planta.Remove(rU_rel_usuarios_planta);
            db.SaveChanges();
            TempData["Mensaje"] = new MensajesSweetAlert("Se eliminó correctamente el usuario.", TipoMensajesSweetAlerts.SUCCESS);

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
