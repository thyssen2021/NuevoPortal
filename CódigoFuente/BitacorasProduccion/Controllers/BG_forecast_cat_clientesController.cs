using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    public class BG_forecast_cat_clientesController : Controller
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_forecast_cat_clientes
        public ActionResult Index()
        {
            return View(db.BG_forecast_cat_clientes.ToList());
        }

        // GET: BG_forecast_cat_clientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_forecast_cat_clientes bG_forecast_cat_clientes = db.BG_forecast_cat_clientes.Find(id);
            if (bG_forecast_cat_clientes == null)
            {
                return HttpNotFound();
            }
            return View(bG_forecast_cat_clientes);
        }

        // GET: BG_forecast_cat_clientes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BG_forecast_cat_clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,activo")] BG_forecast_cat_clientes bG_forecast_cat_clientes)
        {
            if (ModelState.IsValid)
            {
                db.BG_forecast_cat_clientes.Add(bG_forecast_cat_clientes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bG_forecast_cat_clientes);
        }

        // GET: BG_forecast_cat_clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_forecast_cat_clientes bG_forecast_cat_clientes = db.BG_forecast_cat_clientes.Find(id);
            if (bG_forecast_cat_clientes == null)
            {
                return HttpNotFound();
            }
            return View(bG_forecast_cat_clientes);
        }

        // POST: BG_forecast_cat_clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,activo")] BG_forecast_cat_clientes bG_forecast_cat_clientes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bG_forecast_cat_clientes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bG_forecast_cat_clientes);
        }

        // GET: BG_forecast_cat_clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_forecast_cat_clientes bG_forecast_cat_clientes = db.BG_forecast_cat_clientes.Find(id);
            if (bG_forecast_cat_clientes == null)
            {
                return HttpNotFound();
            }
            return View(bG_forecast_cat_clientes);
        }

        // POST: BG_forecast_cat_clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BG_forecast_cat_clientes bG_forecast_cat_clientes = db.BG_forecast_cat_clientes.Find(id);
            db.BG_forecast_cat_clientes.Remove(bG_forecast_cat_clientes);
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
