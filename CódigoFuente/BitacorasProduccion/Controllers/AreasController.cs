﻿using System;
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
    public class AreasController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: Areas
        public ActionResult Index(int? clave_planta)
        {
            if (TieneRol(TipoRoles.RH))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var area = db.Area.Where(x => ((clave_planta == null || (clave_planta == x.plantaClave && !x.shared_services)))
                                        || (clave_planta != null && clave_planta == 99 && x.shared_services)
                ).ToList();

                //busca todas las plantas que tengan un area 
                List<plantas> plantasList = db.Area.Where(x => x.plantas != null).Select(x => x.plantas).Distinct().ToList();
                //crea el select list para plantas
                List<SelectListItem> newList = new List<SelectListItem>();
                foreach (var p in plantasList)
                {
                    newList.Add(new SelectListItem()
                    {
                        Text = p.descripcion,
                        Value = p.clave.ToString()
                    });
                }

                //agrega el valor paras shared services
                newList.Add(new SelectListItem()
                {
                    Text = "SHARED SERVICES",
                    Value = "99"    //valor para shared services
                });

                //envia el select list por viewbag
                ViewBag.clave_planta = AddFirstItem(new SelectList(newList, "Value", "Text", clave_planta.ToString()), textoPorDefecto: "-- Todas --");

                return View(area);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: Areas/Details/5
        public ActionResult Details(int? id)
        {

            if (TieneRol(TipoRoles.RH))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                plantas plantas = db.plantas.Find(id);
                Area area = db.Area.Find(id);
                if (area == null)
                {
                    return View("../Error/NotFound");
                }
                return View(area);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // GET: Areas/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.RH))
            {
                ViewBag.plantaClave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: Areas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Area area)
        {

            if (ModelState.IsValid)
            {

                //busca si ya exite un area con esos valores
                Area areaBusca = db.Area.Where(s => s.plantaClave == area.plantaClave && s.descripcion.ToUpper() == area.descripcion.ToUpper()
                                            && s.shared_services == area.shared_services
                                            //&& s.numero_centro_costo == area.numero_centro_costo
                                            )
                                        .FirstOrDefault();

                if (areaBusca == null) //no existe el area
                {
                    area.activo = true;
                    area.descripcion = area.descripcion.ToUpper();

                    db.Area.Add(area);
                    db.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe un registro con los mismos valores.");
                    ViewBag.plantaClave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                    return View(area);
                }

                //para mostrar la alerta de exito
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            ViewBag.plantaClave = new SelectList(db.plantas, "clave", "descripcion", area.plantaClave);
            return View(area);
        }

        // GET: Areas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.RH))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                Area area = db.Area.Find(id);
                if (area == null)
                {
                    return View("../Error/NotFound");
                }

                ViewBag.plantaClave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.pClave = area.plantaClave;
                return View(area);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Areas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Area area)
        {

            if (db.Area.Any(s => s.plantaClave == area.plantaClave && s.descripcion.ToUpper() == area.descripcion.ToUpper()
                                            && s.shared_services == area.shared_services
                                            && s.clave != area.clave
                                            ))
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores.");

            if (ModelState.IsValid)
            {
                area.descripcion = area.descripcion.ToUpper();

                db.Entry(area).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.plantaClave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.pClave = area.plantaClave;
            return View(area);
        }

        // GET: Areas/Disable/5
        public ActionResult Disable(int? id)
        {

            if (TieneRol(TipoRoles.RH))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                Area area = db.Area.Find(id);
                if (area == null)
                {
                    return View("../Error/NotFound");
                }
                return View(area);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Areas/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            Area area = db.Area.Find(id);
            area.activo = false;

            db.Entry(area).State = EntityState.Modified;
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

        // GET: Areas/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.RH))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                Area area = db.Area.Find(id);
                if (area == null)
                {
                    return View("../Error/NotFound");
                }
                return View(area);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Areas/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            Area area = db.Area.Find(id);
            area.activo = true;

            db.Entry(area).State = EntityState.Modified;
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
