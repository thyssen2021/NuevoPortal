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
    public class GV_comprobacion_tipo_gastos_viajeController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: GV_comprobacion_tipo_gastos_viaje
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.GV_CATALOGOS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var listado = db.GV_comprobacion_tipo_gastos_viaje;
            return View(listado.ToList());
        }

        // GET: GV_comprobacion_tipo_gastos_viaje/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.GV_CATALOGOS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }

            GV_comprobacion_tipo_gastos_viaje GV_comprobacion_tipo_gastos_viaje = db.GV_comprobacion_tipo_gastos_viaje.Find(id);
            if (GV_comprobacion_tipo_gastos_viaje == null)
            {
                return View("../Error/NotFound");
            }
            return View(GV_comprobacion_tipo_gastos_viaje);
        }

        // GET: GV_comprobacion_tipo_gastos_viaje/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.GV_CATALOGOS))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: GV_comprobacion_tipo_gastos_viaje/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GV_comprobacion_tipo_gastos_viaje gV_comprobacion_tipo_gastos_viaje)
        {
            //busca si ya exite un GV_comprobacion_tipo_gastos_viaje con esos valores
            GV_comprobacion_tipo_gastos_viaje ccBusca = db.GV_comprobacion_tipo_gastos_viaje.Where(s => s.cuenta == gV_comprobacion_tipo_gastos_viaje.cuenta)
                                    .FirstOrDefault();

            if (ccBusca != null)
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores.");

            if (ModelState.IsValid)
            {
                gV_comprobacion_tipo_gastos_viaje.activo = true;
                db.GV_comprobacion_tipo_gastos_viaje.Add(gV_comprobacion_tipo_gastos_viaje);
                db.SaveChanges();

                //para mostrar la alerta de exito
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }


            return View(gV_comprobacion_tipo_gastos_viaje);
        }

        // GET: GV_comprobacion_tipo_gastos_viaje/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.GV_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_comprobacion_tipo_gastos_viaje gV_comprobacion_tipo_gastos_viaje = db.GV_comprobacion_tipo_gastos_viaje.Find(id);
                if (gV_comprobacion_tipo_gastos_viaje == null)
                {
                    return View("../Error/NotFound");
                }

                return View(gV_comprobacion_tipo_gastos_viaje);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: GV_comprobacion_tipo_gastos_viaje/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GV_comprobacion_tipo_gastos_viaje gV_comprobacion_tipo_gastos_viaje)
        {

            //busca si ya exite un GV_comprobacion_tipo_gastos_viaje con esos valores
            GV_comprobacion_tipo_gastos_viaje ccBusca = db.GV_comprobacion_tipo_gastos_viaje.Where(s => s.cuenta == gV_comprobacion_tipo_gastos_viaje.cuenta && s.id != gV_comprobacion_tipo_gastos_viaje.id)
                                    .FirstOrDefault();

            if (ccBusca != null)
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores.");

            if (ModelState.IsValid)
            {

                db.Entry(gV_comprobacion_tipo_gastos_viaje).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
          
            return View(gV_comprobacion_tipo_gastos_viaje);
        }

        // GET: GV_comprobacion_tipo_gastos_viaje/Disable/5
        public ActionResult Disable(int? id)
        {

            if (TieneRol(TipoRoles.GV_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_comprobacion_tipo_gastos_viaje GV_comprobacion_tipo_gastos_viaje = db.GV_comprobacion_tipo_gastos_viaje.Find(id);
                if (GV_comprobacion_tipo_gastos_viaje == null)
                {
                    return View("../Error/NotFound");
                }
                return View(GV_comprobacion_tipo_gastos_viaje);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: GV_comprobacion_tipo_gastos_viaje/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            GV_comprobacion_tipo_gastos_viaje GV_comprobacion_tipo_gastos_viaje = db.GV_comprobacion_tipo_gastos_viaje.Find(id);
            GV_comprobacion_tipo_gastos_viaje.activo = false;

            db.Entry(GV_comprobacion_tipo_gastos_viaje).State = EntityState.Modified;
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

        // GET: GV_comprobacion_tipo_gastos_viaje/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.GV_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_comprobacion_tipo_gastos_viaje GV_comprobacion_tipo_gastos_viaje = db.GV_comprobacion_tipo_gastos_viaje.Find(id);
                if (GV_comprobacion_tipo_gastos_viaje == null)
                {
                    return View("../Error/NotFound");
                }
                return View(GV_comprobacion_tipo_gastos_viaje);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: GV_comprobacion_tipo_gastos_viaje/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            GV_comprobacion_tipo_gastos_viaje GV_comprobacion_tipo_gastos_viaje = db.GV_comprobacion_tipo_gastos_viaje.Find(id);
            GV_comprobacion_tipo_gastos_viaje.activo = true;

            db.Entry(GV_comprobacion_tipo_gastos_viaje).State = EntityState.Modified;
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
