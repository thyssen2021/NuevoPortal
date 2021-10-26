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
    public class ProduccionLineasController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: ProduccionLineas
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }
                var produccion_lineas = db.produccion_lineas.Include(p => p.plantas);
                return View(produccion_lineas.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // GET: ProduccionLineas/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                produccion_lineas produccion_lineas = db.produccion_lineas.Find(id);
                if (produccion_lineas == null)
                {
                    return HttpNotFound();
                }
                return View(produccion_lineas);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // GET: ProduccionLineas/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // POST: ProduccionLineas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,linea,clave_planta,activo,ip")] produccion_lineas produccion_lineas)
        {
            if (ModelState.IsValid)
            {
                produccion_lineas.activo = true;
                db.produccion_lineas.Add(produccion_lineas);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", produccion_lineas.clave_planta);
            return View(produccion_lineas);
        }

        // GET: ProduccionLineas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                produccion_lineas produccion_lineas = db.produccion_lineas.Find(id);
                if (produccion_lineas == null)
                {
                    return HttpNotFound();
                }
                ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", produccion_lineas.clave_planta);
                return View(produccion_lineas);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }          
        }

        // POST: ProduccionLineas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,linea,clave_planta,activo,ip")] produccion_lineas produccion_lineas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(produccion_lineas).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", produccion_lineas.clave_planta);
            return View(produccion_lineas);
        }

        // GET: ProduccionLineas/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                produccion_lineas valor = db.produccion_lineas.Find(id);
                if (valor == null)
                {
                    return HttpNotFound();
                }
                return View(valor);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: ProduccionLineas/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            produccion_lineas valor = db.produccion_lineas.Find(id);
            valor.activo = false;

            db.Entry(valor).State = EntityState.Modified;
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

        // GET: ProduccionLineas/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                produccion_lineas valor = db.produccion_lineas.Find(id);
                if (valor == null)
                {
                    return HttpNotFound();
                }
                return View(valor);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: ProduccionLineas/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            produccion_lineas valor = db.produccion_lineas.Find(id);
            valor.activo = true;

            db.Entry(valor).State = EntityState.Modified;
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
