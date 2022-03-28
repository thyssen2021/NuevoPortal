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
    public class BG_CentroCostoController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_CentroCosto
        public ActionResult Index()
        {

            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                var budget_centro_costo = db.budget_centro_costo;
                return View(budget_centro_costo.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

          
        }

        // GET: BG_CentroCosto/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                budget_centro_costo budget_centro_costo = db.budget_centro_costo.Find(id);
                if (budget_centro_costo == null)
                {
                    return View("../Error/NotFound");
                }
                return View(budget_centro_costo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: BG_CentroCosto/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                ViewBag.plantaClave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                ViewBag.id_area = AddFirstItem(new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                ViewBag.id_responsable = AddFirstItem(new SelectList(db.empleados.Where(p => p.activo == true), "id", "ConcatNumEmpleadoNombre"), textoPorDefecto: "-- Seleccionar --");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
           
        }

        // POST: BG_CentroCosto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_area,id_responsable,num_centro_costo")] budget_centro_costo item)
        {
           
            if (ModelState.IsValid)
            {
                //busca si tipo poliza con la misma descripcion
                budget_centro_costo item_busca = db.budget_centro_costo.Where(s => s.budget_departamentos.id == item.budget_departamentos.id )
                                        .FirstOrDefault();

                //busca si tipo poliza con la misma descripcion
                budget_centro_costo item_busca_num = db.budget_centro_costo.Where(s => s.num_centro_costo == item.num_centro_costo)
                                        .FirstOrDefault();

                if (item_busca!=null)
                    ModelState.AddModelError("", "Ya existe un centro de costo asociado a este departamento.");

                if (item_busca_num != null)
                    ModelState.AddModelError("", "Ya existe el número de centro de costo.");

                if (item_busca == null && item_busca_num == null)
                { //Si no existe
                  
                    db.budget_centro_costo.Add(item);
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
            }

            item.budget_departamentos = db.budget_departamentos.Find(item.budget_departamentos.id);

            ViewBag.id_planta = AddFirstItem(new SelectList(db.budget_plantas.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            ViewBag.id_budget_departamento = AddFirstItem(new SelectList(db.budget_departamentos.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
           
            return View(item);
        }

        // GET: BG_CentroCosto/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                budget_centro_costo item = db.budget_centro_costo.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }
                ViewBag.plantaClave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                ViewBag.id_area = AddFirstItem(new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

           
        }

        // POST: BG_CentroCosto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_area,id_responsable,num_centro_costo")] budget_centro_costo item)
        {
            if (ModelState.IsValid)
            {
                //busca si tipo poliza con la misma descripcion
                budget_centro_costo item_busca = db.budget_centro_costo.Where(s => s.budget_departamentos.id == item.budget_departamentos.id)
                                        .FirstOrDefault();

                //busca si tipo poliza con la misma descripcion
                budget_centro_costo item_busca_num = db.budget_centro_costo.Where(s => s.num_centro_costo == item.num_centro_costo && s.id != item.id)
                                        .FirstOrDefault();

                if (item_busca != null)
                    ModelState.AddModelError("", "Ya existe un centro de costo asociado a este departamento.");

                if (item_busca_num != null)
                    ModelState.AddModelError("", "Ya existe el número de centro de costo.");

                if (item_busca == null && item_busca_num == null)
                {
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            item.budget_departamentos = db.budget_departamentos.Find(item.budget_departamentos.id);

            ViewBag.id_planta = AddFirstItem(new SelectList(db.budget_plantas.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            ViewBag.id_budget_departamento = AddFirstItem(new SelectList(db.budget_departamentos.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");

            return View(item);
        }

        // GET: BG_CentroCosto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            budget_centro_costo budget_centro_costo = db.budget_centro_costo.Find(id);
            if (budget_centro_costo == null)
            {
                return HttpNotFound();
            }
            return View(budget_centro_costo);
        }

        // POST: BG_CentroCosto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            budget_centro_costo budget_centro_costo = db.budget_centro_costo.Find(id);
            db.budget_centro_costo.Remove(budget_centro_costo);
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
