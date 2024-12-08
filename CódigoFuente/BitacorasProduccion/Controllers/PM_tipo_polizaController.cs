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
    public class PM_tipo_polizaController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: PM_tipo_poliza
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.PM_CATALOGOS))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                return View(db.PM_tipo_poliza.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: PM_tipo_poliza/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.PM_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                PM_tipo_poliza item = db.PM_tipo_poliza.Find(id);
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

        // GET: PM_tipo_poliza/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.PM_CATALOGOS))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: PM_tipo_poliza/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,activo")] PM_tipo_poliza item)
        {

            if (ModelState.IsValid)
            {
                //busca si tipo poliza con la misma descripcion
                PM_tipo_poliza item_busca = db.PM_tipo_poliza.Where(s => s.descripcion.ToUpper() == item.descripcion.ToUpper() && !String.IsNullOrEmpty(item.descripcion))
                                        .FirstOrDefault();

                if (item_busca == null)
                { //Si no existe
                    item.descripcion = item.descripcion.ToUpper();
                    item.activo = true;
                    db.PM_tipo_poliza.Add(item);
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

        // GET: PM_tipo_poliza/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.PM_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                PM_tipo_poliza item = db.PM_tipo_poliza.Find(id);
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

        // POST: PM_tipo_poliza/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,activo")] PM_tipo_poliza item)
        {

            if (ModelState.IsValid)
            {

                //busca si existe in departamento con la misma descripcion
                PM_tipo_poliza item_busca = db.PM_tipo_poliza.Where(s => s.descripcion.ToUpper() == item.descripcion.ToUpper() && !String.IsNullOrEmpty(item.descripcion) && s.id != item.id)
                                        .FirstOrDefault();


                if (item_busca == null)
                { //Si no existe
                    item.descripcion = item.descripcion.ToUpper();
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

        // GET: PM_tipo_poliza/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.PM_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                PM_tipo_poliza item = db.PM_tipo_poliza.Find(id);
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

        // POST: PM_tipo_poliza/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            PM_tipo_poliza item = db.PM_tipo_poliza.Find(id);
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

        // GET: PM_tipo_poliza/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.PM_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                PM_tipo_poliza item = db.PM_tipo_poliza.Find(id);
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

        // POST: PM_tipo_poliza/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            PM_tipo_poliza item = db.PM_tipo_poliza.Find(id);
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
