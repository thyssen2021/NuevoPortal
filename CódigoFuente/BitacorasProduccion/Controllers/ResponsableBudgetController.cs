﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class ResponsableBudgetController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: ResponsableBudget/Actual
        public ActionResult Actual()
        {
            if (TieneRol(TipoRoles.BG_RESPONSABLE) )
            {
                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var centros = db.budget_centro_costo.Where(x=>x.id_responsable==empleado.id);
                return View(centros.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }
        // GET: ResponsableBudget/DetailsActual
        public ActionResult DetailsActual(int? id)
        {

            if (TieneRol(TipoRoles.BG_RESPONSABLE))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }

                //obtiene centro de costo
                budget_centro_costo centroCosto = db.budget_centro_costo.Find(id);
                if(centroCosto==null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal anterior
                budget_anio_fiscal anio_Fiscal_anterior = GetAnioFiscal(DateTime.Now.AddYears(-1));
                if (anio_Fiscal_anterior == null)
                    return View("../Error/NotFound");

                //busca el año fiscal del centro correspondiente al año anterior
                var budget_rel_anio_fiscal_centro = db.budget_rel_anio_fiscal_centro.Where(
                        x=>x.id_centro_costo==centroCosto.id 
                        && x.id_anio_fiscal == anio_Fiscal_anterior.id
                        && x.tipo == BG_Status.ACTUAL       //busca unicamente ACTUAL (real{pasado})
                    ).FirstOrDefault();

                if (budget_rel_anio_fiscal_centro == null)
                {
                    //en caso de que no haya centro_x_anio
                    budget_rel_anio_fiscal_centro= new budget_rel_anio_fiscal_centro { 
                        id=0,
                        id_centro_costo=id.Value,
                        budget_anio_fiscal = anio_Fiscal_anterior,
                        budget_centro_costo = centroCosto
                    };
                }

                var valoresList = db.budget_valores.Where(x => x.id_rel_anio_centro == budget_rel_anio_fiscal_centro.id).ToList();

                ViewBag.Anio_Centro = budget_rel_anio_fiscal_centro;
                return View(valoresList);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        //// GET: ResponsableBudget/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    budget_rel_anio_fiscal_centro budget_rel_anio_fiscal_centro = db.budget_rel_anio_fiscal_centro.Find(id);
        //    if (budget_rel_anio_fiscal_centro == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(budget_rel_anio_fiscal_centro);
        //}

        //// GET: ResponsableBudget/Create
        //public ActionResult Create()
        //{
        //    ViewBag.id_anio_fiscal = new SelectList(db.budget_anio_fiscal, "id", "descripcion");
        //    ViewBag.id_centro_costo = new SelectList(db.budget_centro_costo, "id", "num_centro_costo");
        //    return View();
        //}

        //// POST: ResponsableBudget/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id,id_anio_fiscal,id_centro_costo,tipo,estatus")] budget_rel_anio_fiscal_centro budget_rel_anio_fiscal_centro)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.budget_rel_anio_fiscal_centro.Add(budget_rel_anio_fiscal_centro);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.id_anio_fiscal = new SelectList(db.budget_anio_fiscal, "id", "descripcion", budget_rel_anio_fiscal_centro.id_anio_fiscal);
        //    ViewBag.id_centro_costo = new SelectList(db.budget_centro_costo, "id", "num_centro_costo", budget_rel_anio_fiscal_centro.id_centro_costo);
        //    return View(budget_rel_anio_fiscal_centro);
        //}

        //// GET: ResponsableBudget/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    budget_rel_anio_fiscal_centro budget_rel_anio_fiscal_centro = db.budget_rel_anio_fiscal_centro.Find(id);
        //    if (budget_rel_anio_fiscal_centro == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.id_anio_fiscal = new SelectList(db.budget_anio_fiscal, "id", "descripcion", budget_rel_anio_fiscal_centro.id_anio_fiscal);
        //    ViewBag.id_centro_costo = new SelectList(db.budget_centro_costo, "id", "num_centro_costo", budget_rel_anio_fiscal_centro.id_centro_costo);
        //    return View(budget_rel_anio_fiscal_centro);
        //}

        //// POST: ResponsableBudget/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "id,id_anio_fiscal,id_centro_costo,tipo,estatus")] budget_rel_anio_fiscal_centro budget_rel_anio_fiscal_centro)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(budget_rel_anio_fiscal_centro).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.id_anio_fiscal = new SelectList(db.budget_anio_fiscal, "id", "descripcion", budget_rel_anio_fiscal_centro.id_anio_fiscal);
        //    ViewBag.id_centro_costo = new SelectList(db.budget_centro_costo, "id", "num_centro_costo", budget_rel_anio_fiscal_centro.id_centro_costo);
        //    return View(budget_rel_anio_fiscal_centro);
        //}

        //// GET: ResponsableBudget/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    budget_rel_anio_fiscal_centro budget_rel_anio_fiscal_centro = db.budget_rel_anio_fiscal_centro.Find(id);
        //    if (budget_rel_anio_fiscal_centro == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(budget_rel_anio_fiscal_centro);
        //}

        //// POST: ResponsableBudget/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    budget_rel_anio_fiscal_centro budget_rel_anio_fiscal_centro = db.budget_rel_anio_fiscal_centro.Find(id);
        //    db.budget_rel_anio_fiscal_centro.Remove(budget_rel_anio_fiscal_centro);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        [NonAction]
        protected budget_anio_fiscal GetAnioFiscal(DateTime fechaBusqueda) {

            //obtiene la fecha y la coloca al inicio del mes          
            var fecha = new DateTime(fechaBusqueda.Year, fechaBusqueda.Month, 1, 0, 0, 0);
            //encuentra el año fiscal anterior

            var anio_fiscal_list = db.budget_anio_fiscal.Where(x => x.anio_inicio == fecha.Year || x.anio_fin == fecha.Year).ToList();

            foreach (budget_anio_fiscal anioFiscal in anio_fiscal_list)
            {
                DateTime inicio = new DateTime(anioFiscal.anio_inicio, anioFiscal.mes_inicio, 1, 0, 0, 0);
                DateTime fin = new DateTime(anioFiscal.anio_fin, anioFiscal.mes_fin, 1, 0, 0, 0);

                if (fecha >= inicio && fecha <= fin)
                    return anioFiscal;
            }

            return null;
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
