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
    public class ProduccionTurnosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: ProduccionTurnos
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var produccion_turnos = db.produccion_turnos.Include(p => p.plantas);
                return View(produccion_turnos.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // GET: ProduccionTurnos/Details/5
        public ActionResult Details(int? id)
        {

            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                produccion_turnos produccion_turnos = db.produccion_turnos.Find(id);
                if (produccion_turnos == null)
                {
                    return HttpNotFound();
                }
                return View(produccion_turnos);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

            
        }

        // GET: ProduccionTurnos/Create
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

        // POST: ProduccionTurnos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,clave_planta,valor,descripcion,hora_inicio,hora_fin,activo")] produccion_turnos produccion_turnos)
        {
            if (ModelState.IsValid)
            {
                produccion_turnos.activo = true;
                db.produccion_turnos.Add(produccion_turnos);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", produccion_turnos.clave_planta);
            return View(produccion_turnos);
        }

        // GET: ProduccionTurnos/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                produccion_turnos produccion_turnos = db.produccion_turnos.Find(id);
                if (produccion_turnos == null)
                {
                    return HttpNotFound();
                }
                ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", produccion_turnos.clave_planta);
                return View(produccion_turnos);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }           
        }

        // POST: ProduccionTurnos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,clave_planta,valor,descripcion,hora_inicio,hora_fin,activo")] produccion_turnos produccion_turnos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(produccion_turnos).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", produccion_turnos.clave_planta);
            return View(produccion_turnos);
        }

        // GET: ProduccionTurnos/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                produccion_turnos turno = db.produccion_turnos.Find(id);
                if (turno == null)
                {
                    return HttpNotFound();
                }
                return View(turno);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: ProduccionTurnos/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            produccion_turnos turno = db.produccion_turnos.Find(id);
            turno.activo = false;

            db.Entry(turno).State = EntityState.Modified;
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

        // GET: puestos/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                produccion_turnos turno = db.produccion_turnos.Find(id);
                if (turno == null)
                {
                    return HttpNotFound();
                }
                return View(turno);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Plantas/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            produccion_turnos turno = db.produccion_turnos.Find(id);
            turno.activo = true;

            db.Entry(turno).State = EntityState.Modified;
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
