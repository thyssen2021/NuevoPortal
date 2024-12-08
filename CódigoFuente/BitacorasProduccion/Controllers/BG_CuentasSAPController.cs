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
    public class BG_CuentasSAPController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_CuentasSAP
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                return View(db.budget_cuenta_sap.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: BG_CuentasSAP/Details/5
        public ActionResult Details(int? id)
        {

            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                budget_cuenta_sap budget_cuenta_sap = db.budget_cuenta_sap.Find(id);
                if (budget_cuenta_sap == null)
                {
                    return View("../Error/NotFound");
                }
                return View(budget_cuenta_sap);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: BG_CuentasSAP/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                ViewBag.id_mapping_bridge = AddFirstItem(new SelectList(db.budget_mapping_bridge.Where(x => x.activo), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                ViewBag.id_mapping = AddFirstItem(new SelectList(db.budget_mapping.Where(x => x.activo), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: BG_CuentasSAP/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(budget_cuenta_sap item)
        {
            string mensajeError = "Ya existe un registro con los mismos valores";
            if (ModelState.IsValid)
            {
                //busca si tipo cuenta con la misma descripcion
                budget_cuenta_sap item_busca = db.budget_cuenta_sap.Where(s => s.name.ToUpper() == item.name.ToUpper()  &&
                !String.IsNullOrEmpty(item.name) && s.id_mapping == item.id_mapping)
                                        .FirstOrDefault();

                budget_cuenta_sap item_busca_cuenta = db.budget_cuenta_sap.Where(s => s.sap_account.ToUpper() == item.sap_account.ToUpper())
                                        .FirstOrDefault();

                if (item_busca_cuenta != null)
                    mensajeError = "Ya existe el número de cuenta";

                if (item_busca == null && item_busca_cuenta == null)
                { //Si no existe
                    item.activo = true;
                    db.budget_cuenta_sap.Add(item);
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
               
            }

            item.budget_mapping = db.budget_mapping.Find(item.id_mapping);

            ModelState.AddModelError("", mensajeError);
            ViewBag.id_mapping_bridge = AddFirstItem(new SelectList(db.budget_mapping_bridge.Where(x => x.activo), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            ViewBag.id_mapping = AddFirstItem(new SelectList(db.budget_mapping.Where(x => x.activo), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");

            return View(item);
        }

        // GET: BG_CuentasSAP/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                budget_cuenta_sap item = db.budget_cuenta_sap.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }
                ViewBag.id_mapping_bridge = AddFirstItem(new SelectList(db.budget_mapping_bridge.Where(x => x.activo), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: item.budget_mapping.budget_mapping_bridge.id.ToString());
                ViewBag.id_mapping = AddFirstItem(new SelectList(db.budget_mapping.Where(x => x.activo), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: item.budget_mapping.id_mapping_bridge.ToString());
                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: BG_CuentasSAP/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(budget_cuenta_sap item)
        {
            string mensajeError = "Ya existe un registro con los mismos valores";

            if (ModelState.IsValid)
            {

                //busca si tipo cuenta con la misma descripcion
                budget_cuenta_sap item_busca = db.budget_cuenta_sap.Where(s => s.name.ToUpper() == item.name.ToUpper() &&
                !String.IsNullOrEmpty(item.name) && s.id_mapping == item.id_mapping && s.id != item.id)
                                        .FirstOrDefault();

                budget_cuenta_sap item_busca_cuenta = db.budget_cuenta_sap.Where(s => s.sap_account.ToUpper() == item.sap_account.ToUpper() && s.id != item.id)
                                        .FirstOrDefault();

                if (item_busca_cuenta != null)
                    mensajeError = "Ya existe el número de cuenta";


                if (item_busca == null && item_busca_cuenta == null)
                { //Si no existe
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }                

            }

            item.budget_mapping = db.budget_mapping.Find(item.id_mapping);

            ModelState.AddModelError("", mensajeError);
            ViewBag.id_mapping_bridge = AddFirstItem(new SelectList(db.budget_mapping_bridge.Where(x => x.activo), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            ViewBag.id_mapping = AddFirstItem(new SelectList(db.budget_mapping.Where(x => x.activo), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");


            return View(item);
        }

        // GET: BG_CuentasSAP/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                budget_cuenta_sap item = db.budget_cuenta_sap.Find(id);
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

        // POST: BG_CuentasSAP/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            budget_cuenta_sap item = db.budget_cuenta_sap.Find(id);
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

        // GET: BG_CuentasSAP/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                budget_cuenta_sap item = db.budget_cuenta_sap.Find(id);
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

        // POST: BG_CuentasSAP/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            budget_cuenta_sap item = db.budget_cuenta_sap.Find(id);
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
