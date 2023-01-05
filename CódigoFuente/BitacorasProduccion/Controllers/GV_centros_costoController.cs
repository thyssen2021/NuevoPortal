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
    public class GV_centros_costoController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: GV_centros_costo
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.GV_CATALOGOS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var listado = db.GV_centros_costo.OrderBy(x => x.clave_planta);
            return View(listado.ToList());
        }

        // GET: GV_centros_costo/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.GV_CATALOGOS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }

            GV_centros_costo GV_centros_costo = db.GV_centros_costo.Find(id);
            if (GV_centros_costo == null)
            {
                return View("../Error/NotFound");
            }
            return View(GV_centros_costo);
        }

        // GET: GV_centros_costo/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.GV_CATALOGOS))
            {
                ViewBag.clave_planta = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: GV_centros_costo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GV_centros_costo gV_centros_costo)
        {
            //busca si ya exite un GV_centros_costo con esos valores
            GV_centros_costo ccBusca = db.GV_centros_costo.Where(s => s.clave_planta == gV_centros_costo.clave_planta && s.centro_costo.ToUpper() == gV_centros_costo.centro_costo.ToUpper())
                                    .FirstOrDefault();

            if (ccBusca != null)
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores.");

            if (ModelState.IsValid)
            {
                gV_centros_costo.activo = true;
                db.GV_centros_costo.Add(gV_centros_costo);
                db.SaveChanges();

                //para mostrar la alerta de exito
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            ViewBag.clave_planta = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --", selected: gV_centros_costo.clave_planta.ToString());

            return View(gV_centros_costo);
        }

        // GET: GV_centros_costo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.GV_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_centros_costo gV_centros_costo = db.GV_centros_costo.Find(id);
                if (gV_centros_costo == null)
                {
                    return View("../Error/NotFound");
                }

                ViewBag.clave_planta = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --", selected: gV_centros_costo.clave_planta.ToString());


                return View(gV_centros_costo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: GV_centros_costo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GV_centros_costo gV_centros_costo)
        {

            //busca si ya exite un GV_centros_costo con esos valores
            GV_centros_costo ccBusca = db.GV_centros_costo.Where(s => s.clave_planta == gV_centros_costo.clave_planta && s.centro_costo.ToUpper() == gV_centros_costo.centro_costo.ToUpper()
                                     && s.id != gV_centros_costo.id
                                    )
                                    .FirstOrDefault();

            if (ccBusca != null)
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores.");

            if (ModelState.IsValid)
            {

                db.Entry(gV_centros_costo).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.clave_planta = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --", selected: gV_centros_costo.clave_planta.ToString());

            return View(gV_centros_costo);
        }

        // GET: GV_centros_costo/Disable/5
        public ActionResult Disable(int? id)
        {

            if (TieneRol(TipoRoles.GV_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_centros_costo GV_centros_costo = db.GV_centros_costo.Find(id);
                if (GV_centros_costo == null)
                {
                    return View("../Error/NotFound");
                }
                return View(GV_centros_costo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: GV_centros_costo/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            GV_centros_costo GV_centros_costo = db.GV_centros_costo.Find(id);
            GV_centros_costo.activo = false;

            db.Entry(GV_centros_costo).State = EntityState.Modified;
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

        // GET: GV_centros_costo/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.GV_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_centros_costo GV_centros_costo = db.GV_centros_costo.Find(id);
                if (GV_centros_costo == null)
                {
                    return View("../Error/NotFound");
                }
                return View(GV_centros_costo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: GV_centros_costo/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            GV_centros_costo GV_centros_costo = db.GV_centros_costo.Find(id);
            GV_centros_costo.activo = true;

            db.Entry(GV_centros_costo).State = EntityState.Modified;
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
