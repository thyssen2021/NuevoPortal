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
    public class PremiumFreightAprovalController : Controller
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: PremiumFreightAproval
        public ActionResult Index()
        {
            var pFA = db.PFA.Include(p => p.empleados).Include(p => p.empleados1).Include(p => p.PFA_Recovered_cost).Include(p => p.PFA_Border_port).Include(p => p.PFA_Department).Include(p => p.PFA_Destination_plant).Include(p => p.PFA_Reason).Include(p => p.PFA_Responsible_cost).Include(p => p.PFA_Type_shipment).Include(p => p.PFA_Volume);
            return View(pFA.ToList());
        }

        // GET: PremiumFreightAproval/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PFA pFA = db.PFA.Find(id);
            if (pFA == null)
            {
                return HttpNotFound();
            }
            return View(pFA);
        }

        // GET: PremiumFreightAproval/Create
        public ActionResult Create()
        {
            ViewBag.id_PFA_autorizador = new SelectList(db.empleados, "id", "numeroEmpleado");
            ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado");
            ViewBag.id_PFA_recovered_cost = new SelectList(db.PFA_Recovered_cost, "id", "descripcion");
            ViewBag.id_PFA_border_port = new SelectList(db.PFA_Border_port, "id", "descripcion");
            ViewBag.id_PFA_Department = new SelectList(db.PFA_Department, "id", "descripcion");
            ViewBag.id_PFA_destination_plant = new SelectList(db.PFA_Destination_plant, "id", "descripcion");
            ViewBag.id_PFA_reason = new SelectList(db.PFA_Reason, "id", "descripcion");
            ViewBag.id_PFA_responsible_cost = new SelectList(db.PFA_Responsible_cost, "id", "descripcion");
            ViewBag.id_PFA_type_shipment = new SelectList(db.PFA_Type_shipment, "id", "descripcion");
            ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume, "id", "descripcion");
            return View();
        }

        // POST: PremiumFreightAproval/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_solicitante,id_PFA_Department,id_PFA_volume,id_PFA_border_port,id_PFA_destination_plant,id_PFA_reason,id_PFA_type_shipment,id_PFA_responsible_cost,id_PFA_recovered_cost,id_PFA_autorizador,date_request,sap_part_number,customer_part_number,volume,mill,customer,total_cost,total_pf_cost,promise_recovering_date,comentarios,razon_rechazo,estatus,fecha_aprobacion,activo")] PFA pFA)
        {
            if (ModelState.IsValid)
            {
                db.PFA.Add(pFA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_PFA_autorizador = new SelectList(db.empleados, "id", "numeroEmpleado", pFA.id_PFA_autorizador);
            ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", pFA.id_solicitante);
            ViewBag.id_PFA_recovered_cost = new SelectList(db.PFA_Recovered_cost, "id", "descripcion", pFA.id_PFA_recovered_cost);
            ViewBag.id_PFA_border_port = new SelectList(db.PFA_Border_port, "id", "descripcion", pFA.id_PFA_border_port);
            ViewBag.id_PFA_Department = new SelectList(db.PFA_Department, "id", "descripcion", pFA.id_PFA_Department);
            ViewBag.id_PFA_destination_plant = new SelectList(db.PFA_Destination_plant, "id", "descripcion", pFA.id_PFA_destination_plant);
            ViewBag.id_PFA_reason = new SelectList(db.PFA_Reason, "id", "descripcion", pFA.id_PFA_reason);
            ViewBag.id_PFA_responsible_cost = new SelectList(db.PFA_Responsible_cost, "id", "descripcion", pFA.id_PFA_responsible_cost);
            ViewBag.id_PFA_type_shipment = new SelectList(db.PFA_Type_shipment, "id", "descripcion", pFA.id_PFA_type_shipment);
            ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume, "id", "descripcion", pFA.id_PFA_volume);
            return View(pFA);
        }

        // GET: PremiumFreightAproval/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PFA pFA = db.PFA.Find(id);
            if (pFA == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_PFA_autorizador = new SelectList(db.empleados, "id", "numeroEmpleado", pFA.id_PFA_autorizador);
            ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", pFA.id_solicitante);
            ViewBag.id_PFA_recovered_cost = new SelectList(db.PFA_Recovered_cost, "id", "descripcion", pFA.id_PFA_recovered_cost);
            ViewBag.id_PFA_border_port = new SelectList(db.PFA_Border_port, "id", "descripcion", pFA.id_PFA_border_port);
            ViewBag.id_PFA_Department = new SelectList(db.PFA_Department, "id", "descripcion", pFA.id_PFA_Department);
            ViewBag.id_PFA_destination_plant = new SelectList(db.PFA_Destination_plant, "id", "descripcion", pFA.id_PFA_destination_plant);
            ViewBag.id_PFA_reason = new SelectList(db.PFA_Reason, "id", "descripcion", pFA.id_PFA_reason);
            ViewBag.id_PFA_responsible_cost = new SelectList(db.PFA_Responsible_cost, "id", "descripcion", pFA.id_PFA_responsible_cost);
            ViewBag.id_PFA_type_shipment = new SelectList(db.PFA_Type_shipment, "id", "descripcion", pFA.id_PFA_type_shipment);
            ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume, "id", "descripcion", pFA.id_PFA_volume);
            return View(pFA);
        }

        // POST: PremiumFreightAproval/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_solicitante,id_PFA_Department,id_PFA_volume,id_PFA_border_port,id_PFA_destination_plant,id_PFA_reason,id_PFA_type_shipment,id_PFA_responsible_cost,id_PFA_recovered_cost,id_PFA_autorizador,date_request,sap_part_number,customer_part_number,volume,mill,customer,total_cost,total_pf_cost,promise_recovering_date,comentarios,razon_rechazo,estatus,fecha_aprobacion,activo")] PFA pFA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pFA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_PFA_autorizador = new SelectList(db.empleados, "id", "numeroEmpleado", pFA.id_PFA_autorizador);
            ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", pFA.id_solicitante);
            ViewBag.id_PFA_recovered_cost = new SelectList(db.PFA_Recovered_cost, "id", "descripcion", pFA.id_PFA_recovered_cost);
            ViewBag.id_PFA_border_port = new SelectList(db.PFA_Border_port, "id", "descripcion", pFA.id_PFA_border_port);
            ViewBag.id_PFA_Department = new SelectList(db.PFA_Department, "id", "descripcion", pFA.id_PFA_Department);
            ViewBag.id_PFA_destination_plant = new SelectList(db.PFA_Destination_plant, "id", "descripcion", pFA.id_PFA_destination_plant);
            ViewBag.id_PFA_reason = new SelectList(db.PFA_Reason, "id", "descripcion", pFA.id_PFA_reason);
            ViewBag.id_PFA_responsible_cost = new SelectList(db.PFA_Responsible_cost, "id", "descripcion", pFA.id_PFA_responsible_cost);
            ViewBag.id_PFA_type_shipment = new SelectList(db.PFA_Type_shipment, "id", "descripcion", pFA.id_PFA_type_shipment);
            ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume, "id", "descripcion", pFA.id_PFA_volume);
            return View(pFA);
        }

        // GET: PremiumFreightAproval/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PFA pFA = db.PFA.Find(id);
            if (pFA == null)
            {
                return HttpNotFound();
            }
            return View(pFA);
        }

        // POST: PremiumFreightAproval/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PFA pFA = db.PFA.Find(id);
            db.PFA.Remove(pFA);
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
