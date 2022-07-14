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
    public class IT_inventory_cellular_lineController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_inventory_cellular_line
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                return View(db.IT_inventory_cellular_line.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: IT_inventory_cellular_line/Details/5
        public ActionResult Details(int? id)
        {

            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_cellular_line IT_inventory_cellular_line = db.IT_inventory_cellular_line.Find(id);
                if (IT_inventory_cellular_line == null)
                {
                    return View("../Error/NotFound");
                }
                return View(IT_inventory_cellular_line);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: IT_inventory_cellular_line/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --");
                ViewBag.id_inventory_celullar_plan = AddFirstItem(new SelectList(db.IT_inventory_cellular_plans.Where(x => x.activo), nameof(IT_inventory_cellular_plans.id), nameof(IT_inventory_cellular_plans.nombre_plan)), textoPorDefecto: "-- Seleccionar --");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_cellular_line/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IT_inventory_cellular_line item)
        {
            //busca si hay otro departamento con la misma descripción
            if (db.IT_inventory_cellular_line.Any(x => x.numero_celular == item.numero_celular))
                ModelState.AddModelError("", "El número de celular ya existe.");

            if (ModelState.IsValid)
            {
                item.activo = true;
                db.IT_inventory_cellular_line.Add(item);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --");
            ViewBag.id_inventory_celullar_plan = AddFirstItem(new SelectList(db.IT_inventory_cellular_plans.Where(x => x.activo), nameof(IT_inventory_cellular_plans.id), nameof(IT_inventory_cellular_plans.nombre_plan)), textoPorDefecto: "-- Seleccionar --");

            return View(item);
        }

        // GET: IT_inventory_cellular_line/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_cellular_line item = db.IT_inventory_cellular_line.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --", selected: item.id_planta.ToString());
                ViewBag.id_inventory_celullar_plan = AddFirstItem(new SelectList(db.IT_inventory_cellular_plans.Where(x => x.activo), nameof(IT_inventory_cellular_plans.id), nameof(IT_inventory_cellular_plans.nombre_plan)), textoPorDefecto: "-- Seleccionar --", selected: item.id_inventory_celullar_plan.ToString());
                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_cellular_line/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IT_inventory_cellular_line item)
        {
            //busca si hay otro departamento con la misma descripción
            if (db.IT_inventory_cellular_line.Any(x => x.numero_celular == item.numero_celular && item.id != x.id))
                ModelState.AddModelError("", "El número de celular ya existe.");

            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");

            }
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --", selected: item.id_planta.ToString());
            ViewBag.id_inventory_celullar_plan = AddFirstItem(new SelectList(db.IT_inventory_cellular_plans.Where(x => x.activo), nameof(IT_inventory_cellular_plans.id), nameof(IT_inventory_cellular_plans.nombre_plan)), textoPorDefecto: "-- Seleccionar --", selected: item.id_inventory_celullar_plan.ToString());
          
            return View(item);
        }

        // GET: IT_inventory_cellular_line/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_cellular_line item = db.IT_inventory_cellular_line.Find(id);
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

        // POST: IT_inventory_cellular_line/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            IT_inventory_cellular_line item = db.IT_inventory_cellular_line.Find(id);
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

        // GET: IT_inventory_cellular_line/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_cellular_line item = db.IT_inventory_cellular_line.Find(id);
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

        // POST: IT_inventory_cellular_line/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            IT_inventory_cellular_line item = db.IT_inventory_cellular_line.Find(id);
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
