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
    public class ProduccionDatosEntradaController : Controller
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: ProduccionDatosEntrada
        public ActionResult Index()
        {
            var produccion_datos_entrada = db.produccion_datos_entrada.Include(p => p.produccion_registros);
            return View(produccion_datos_entrada.ToList());
        }

        // GET: ProduccionDatosEntrada/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            produccion_datos_entrada produccion_datos_entrada = db.produccion_datos_entrada.Find(id);
            if (produccion_datos_entrada == null)
            {
                return HttpNotFound();
            }
            return View(produccion_datos_entrada);
        }

        // GET: ProduccionDatosEntrada/Create
        public ActionResult Create()
        {
            ViewBag.id_produccion_registro = new SelectList(db.produccion_registros, "id", "sap_platina");
            return View();
        }

        // POST: ProduccionDatosEntrada/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_produccion_registro,peso_real_pieza_neto,orden_sap,orden_sap_2,piezas_por_golpe,numero_rollo,lote_rollo,peso_etiqueta,peso_regreso_rollo_real,peso_bascula_kgs,peso_despunte_kgs,peso_cola_kgs,total_piezas_ajuste,ordenes_por_pieza")] produccion_datos_entrada produccion_datos_entrada)
        {
            if (ModelState.IsValid)
            {
                db.produccion_datos_entrada.Add(produccion_datos_entrada);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_produccion_registro = new SelectList(db.produccion_registros, "id", "sap_platina", produccion_datos_entrada.id_produccion_registro);
            return View(produccion_datos_entrada);
        }

        // GET: ProduccionDatosEntrada/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            produccion_datos_entrada produccion_datos_entrada = db.produccion_datos_entrada.Find(id);
            if (produccion_datos_entrada == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_produccion_registro = new SelectList(db.produccion_registros, "id", "sap_platina", produccion_datos_entrada.id_produccion_registro);
            return View(produccion_datos_entrada);
        }

        // POST: ProduccionDatosEntrada/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_produccion_registro,peso_real_pieza_neto,orden_sap,orden_sap_2,piezas_por_golpe,numero_rollo,lote_rollo,peso_etiqueta,peso_regreso_rollo_real,peso_bascula_kgs,peso_despunte_kgs,peso_cola_kgs,total_piezas_ajuste,ordenes_por_pieza")] produccion_datos_entrada produccion_datos_entrada)
        {
            if (ModelState.IsValid)
            {
                db.Entry(produccion_datos_entrada).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_produccion_registro = new SelectList(db.produccion_registros, "id", "sap_platina", produccion_datos_entrada.id_produccion_registro);
            return View(produccion_datos_entrada);
        }

        // GET: ProduccionDatosEntrada/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            produccion_datos_entrada produccion_datos_entrada = db.produccion_datos_entrada.Find(id);
            if (produccion_datos_entrada == null)
            {
                return HttpNotFound();
            }
            return View(produccion_datos_entrada);
        }

        // POST: ProduccionDatosEntrada/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            produccion_datos_entrada produccion_datos_entrada = db.produccion_datos_entrada.Find(id);
            db.produccion_datos_entrada.Remove(produccion_datos_entrada);
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
