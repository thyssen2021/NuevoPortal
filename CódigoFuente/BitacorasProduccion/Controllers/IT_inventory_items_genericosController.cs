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
    public class IT_inventory_items_genericosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_inventory_items_genericos
        public ActionResult Index()
        {
            var iT_inventory_items_genericos = db.IT_inventory_items_genericos.Include(i => i.IT_inventory_hardware_type).Include(i => i.IT_inventory_tipos_accesorios);
            return View(iT_inventory_items_genericos.ToList());
        }

        // GET: IT_inventory_items_genericos/Details/5
        public ActionResult Details(int? id)
        {

            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_inventory_items_genericos iT_inventory_items_genericos = db.IT_inventory_items_genericos.Find(id);
            if (iT_inventory_items_genericos == null)
            {
                return HttpNotFound();
            }
            return View(iT_inventory_items_genericos);
        }

        // GET: IT_inventory_items_genericos/Create
        public ActionResult Create()
        {
            ViewBag.id_inventory_type = new SelectList(db.IT_inventory_hardware_type, "id", "descripcion");
            ViewBag.id_tipo_accesorio = new SelectList(db.IT_inventory_tipos_accesorios, "id", "descripcion");
            return View();
        }

        // POST: IT_inventory_items_genericos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_inventory_type,id_tipo_accesorio,brand,model,comments,active")] IT_inventory_items_genericos iT_inventory_items_genericos)
        {
            if (ModelState.IsValid)
            {
                db.IT_inventory_items_genericos.Add(iT_inventory_items_genericos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_inventory_type = new SelectList(db.IT_inventory_hardware_type, "id", "descripcion", iT_inventory_items_genericos.id_inventory_type);
            ViewBag.id_tipo_accesorio = new SelectList(db.IT_inventory_tipos_accesorios, "id", "descripcion", iT_inventory_items_genericos.id_tipo_accesorio);
            return View(iT_inventory_items_genericos);
        }

        // GET: IT_inventory_items_genericos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_inventory_items_genericos iT_inventory_items_genericos = db.IT_inventory_items_genericos.Find(id);
            if (iT_inventory_items_genericos == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_inventory_type = new SelectList(db.IT_inventory_hardware_type, "id", "descripcion", iT_inventory_items_genericos.id_inventory_type);
            ViewBag.id_tipo_accesorio = new SelectList(db.IT_inventory_tipos_accesorios, "id", "descripcion", iT_inventory_items_genericos.id_tipo_accesorio);
            return View(iT_inventory_items_genericos);
        }

        // POST: IT_inventory_items_genericos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_inventory_type,id_tipo_accesorio,brand,model,comments,active")] IT_inventory_items_genericos iT_inventory_items_genericos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iT_inventory_items_genericos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_inventory_type = new SelectList(db.IT_inventory_hardware_type, "id", "descripcion", iT_inventory_items_genericos.id_inventory_type);
            ViewBag.id_tipo_accesorio = new SelectList(db.IT_inventory_tipos_accesorios, "id", "descripcion", iT_inventory_items_genericos.id_tipo_accesorio);
            return View(iT_inventory_items_genericos);
        }

        // GET: IT_inventory_items_genericos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_inventory_items_genericos iT_inventory_items_genericos = db.IT_inventory_items_genericos.Find(id);
            if (iT_inventory_items_genericos == null)
            {
                return HttpNotFound();
            }
            return View(iT_inventory_items_genericos);
        }

        // POST: IT_inventory_items_genericos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IT_inventory_items_genericos iT_inventory_items_genericos = db.IT_inventory_items_genericos.Find(id);
            db.IT_inventory_items_genericos.Remove(iT_inventory_items_genericos);
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
