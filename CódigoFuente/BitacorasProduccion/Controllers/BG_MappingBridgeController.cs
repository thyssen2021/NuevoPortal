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
    public class BG_MappingBridgeController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_MappingBridge
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                return View(db.budget_mapping_bridge.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: BG_MappingBridge/Details/5
        public ActionResult Details(int? id)
        {

            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                budget_mapping_bridge budget_mapping_bridge = db.budget_mapping_bridge.Find(id);
                if (budget_mapping_bridge == null)
                {
                    return View("../Error/NotFound");
                }
                return View(budget_mapping_bridge);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
          
        }

        // GET: BG_MappingBridge/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: BG_MappingBridge/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,activo")] budget_mapping_bridge item)
        {
            if (ModelState.IsValid)
            {
                //busca si tipo poliza con la misma descripcion
                budget_mapping_bridge item_busca = db.budget_mapping_bridge.Where(s => s.descripcion.ToUpper() == item.descripcion.ToUpper() && !String.IsNullOrEmpty(item.descripcion))
                                        .FirstOrDefault();

                if (item_busca == null)
                { //Si no existe
                    item.activo = true;
                    db.budget_mapping_bridge.Add(item);
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe un registro con los mismos valores");
                    return View(item);
                }

            }

            return View(item);
        }

        // GET: BG_MappingBridge/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                budget_mapping_bridge item = db.budget_mapping_bridge.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }
                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: BG_MappingBridge/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,activo")] budget_mapping_bridge item)
        {
            if (ModelState.IsValid)
            {
                //busca si existe in departamento con la misma descripcion
                budget_mapping_bridge item_busca = db.budget_mapping_bridge.Where(s => s.descripcion.ToUpper() == item.descripcion.ToUpper() && !String.IsNullOrEmpty(item.descripcion) && s.id != item.id)
                                        .FirstOrDefault();


                if (item_busca == null)
                { //Si no existe
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe un registro con los mismos valores");
                    return View(item);
                }

            }
            return View(item);
        }

        // GET: BG_MappingBridge/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                budget_mapping_bridge item = db.budget_mapping_bridge.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }
                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: BG_MappingBridge/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            budget_mapping_bridge item = db.budget_mapping_bridge.Find(id);
            item.activo = false;

            db.Entry(item).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Index");
            }
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.DISABLED, TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("Index");
        }

        // GET: BG_MappingBridge/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                budget_mapping_bridge item = db.budget_mapping_bridge.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }
                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: BG_MappingBridge/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            budget_mapping_bridge item = db.budget_mapping_bridge.Find(id);
            item.activo = true;

            db.Entry(item).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Index");
            }
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.ENABLED, TipoMensajesSweetAlerts.SUCCESS);
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
