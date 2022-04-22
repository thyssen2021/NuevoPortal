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
    public class OT_GruposController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: OT_grupo_trabajo
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.OT_CATALOGOS))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                return View(db.OT_grupo_trabajo.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: OT_grupo_trabajo/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.OT_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                OT_grupo_trabajo OT_grupo_trabajo = db.OT_grupo_trabajo.Find(id);
                if (OT_grupo_trabajo == null)
                {
                    return View("../Error/NotFound");
                }
                return View(OT_grupo_trabajo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // GET: OT_grupo_trabajo/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.OT_CATALOGOS))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: OT_grupo_trabajo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,activo")] OT_grupo_trabajo OT_grupo_trabajo)
        {

            if (ModelState.IsValid)
            {
                //busca si existe in departamento con la misma descripcion
                OT_grupo_trabajo pfa_busca = db.OT_grupo_trabajo.Where(s => s.descripcion.ToUpper() == OT_grupo_trabajo.descripcion.ToUpper() && !String.IsNullOrEmpty(OT_grupo_trabajo.descripcion))
                                        .FirstOrDefault();

                if (pfa_busca == null)
                { //Si no existe
                    OT_grupo_trabajo.activo = true;
                    //OT_grupo_trabajo.descripcion = OT_grupo_trabajo.descripcion.ToUpper();
                    db.OT_grupo_trabajo.Add(OT_grupo_trabajo);
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe un registro con los mismos valores");
                    return View(OT_grupo_trabajo);
                }
            }
            return View(OT_grupo_trabajo);

        }

        // GET: OT_grupo_trabajo/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.OT_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                OT_grupo_trabajo OT_grupo_trabajo = db.OT_grupo_trabajo.Find(id);
                if (OT_grupo_trabajo == null)
                {
                    return View("../Error/NotFound");
                }
                return View(OT_grupo_trabajo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: OT_grupo_trabajo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,activo")] OT_grupo_trabajo OT_grupo_trabajo)
        {

            if (ModelState.IsValid)
            {

                //busca si existe in departamento con la misma descripcion
                OT_grupo_trabajo pfa_busca = db.OT_grupo_trabajo.Where(s => s.descripcion.ToUpper() == OT_grupo_trabajo.descripcion.ToUpper() && !String.IsNullOrEmpty(OT_grupo_trabajo.descripcion) && s.id != OT_grupo_trabajo.id)
                                        .FirstOrDefault();



                if (pfa_busca == null)
                { //Si no existe
                    //OT_grupo_trabajo.descripcion = OT_grupo_trabajo.descripcion.ToUpper();
                    db.Entry(OT_grupo_trabajo).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe un registro con los mismos valores");
                    return View(OT_grupo_trabajo);
                }


            }


            return View(OT_grupo_trabajo);
        }

        // GET: PFA_Departmet/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.OT_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                OT_grupo_trabajo item = db.OT_grupo_trabajo.Find(id);
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

        // POST: Plantas/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            OT_grupo_trabajo item = db.OT_grupo_trabajo.Find(id);
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

        // GET: Plantas/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.OT_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                OT_grupo_trabajo item = db.OT_grupo_trabajo.Find(id);
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

        // POST: Plantas/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            OT_grupo_trabajo item = db.OT_grupo_trabajo.Find(id);
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
