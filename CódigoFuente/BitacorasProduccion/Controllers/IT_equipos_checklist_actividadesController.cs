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
    public class IT_equipos_checklist_actividadesController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_equipos_checklist_actividades
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.IT_CHECKLIST_EQUIPOS))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                return View(db.IT_equipos_checklist_actividades.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: IT_equipos_checklist_actividades/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.IT_CHECKLIST_EQUIPOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_equipos_checklist_actividades item = db.IT_equipos_checklist_actividades.Find(id);
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

        // GET: IT_equipos_checklist_actividades/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.IT_CHECKLIST_EQUIPOS))
            {

                ViewBag.id_categoria_ck = AddFirstItem(new SelectList(db.IT_equipos_checklist_categorias.Where(x => x.activo == true), nameof(IT_equipos_checklist_categorias.id), nameof(IT_equipos_checklist_categorias.descripcion))
                    , selected: String.Empty, textoPorDefecto: "-- Seleccionar --");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_equipos_checklist_actividades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IT_equipos_checklist_actividades item)
        {

            if (ModelState.IsValid)
            {
                //busca si tipo poliza con la misma descripcion
                IT_equipos_checklist_actividades item_busca = db.IT_equipos_checklist_actividades.Where(s => s.descripcion.ToUpper() == item.descripcion.ToUpper() && s.id_categoria_ck == item.id_categoria_ck && !String.IsNullOrEmpty(item.descripcion))
                                        .FirstOrDefault();

                if (item_busca == null)
                { //Si no existe
                 //   item.descripcion = item.descripcion.ToUpper();
                    item.activo = true;
                    db.IT_equipos_checklist_actividades.Add(item);
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe un registro con los mismos valores");
                    //return View(item);
                }
            }

            ViewBag.id_categoria_ck = AddFirstItem(new SelectList(db.IT_equipos_checklist_categorias.Where(x => x.activo == true), nameof(IT_equipos_checklist_categorias.id), nameof(IT_equipos_checklist_categorias.descripcion))
                  , selected: item.id_categoria_ck.ToString(), textoPorDefecto: "-- Seleccionar --");
          
            return View(item);

        }

        // GET: IT_equipos_checklist_actividades/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.IT_CHECKLIST_EQUIPOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_equipos_checklist_actividades item = db.IT_equipos_checklist_actividades.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }

                ViewBag.id_categoria_ck = AddFirstItem(new SelectList(db.IT_equipos_checklist_categorias.Where(x => x.activo == true), nameof(IT_equipos_checklist_categorias.id), nameof(IT_equipos_checklist_categorias.descripcion))
              , selected: item.id_categoria_ck.ToString(), textoPorDefecto: "-- Seleccionar --");

                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_equipos_checklist_actividades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IT_equipos_checklist_actividades item)
        {

            if (ModelState.IsValid)
            {

                //busca si existe in departamento con la misma descripcion
                IT_equipos_checklist_actividades item_busca = db.IT_equipos_checklist_actividades.Where(s => s.descripcion.ToUpper() == item.descripcion.ToUpper() && s.id_categoria_ck == item.id_categoria_ck && !String.IsNullOrEmpty(item.descripcion) && s.id != item.id)
                                        .FirstOrDefault();

                if (item_busca == null)
                { //Si no existe
                 //   item.descripcion = item.descripcion.ToUpper();
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe un registro con los mismos valores");
                    //return View(item);
                }
            }

            ViewBag.id_categoria_ck = AddFirstItem(new SelectList(db.IT_equipos_checklist_categorias.Where(x => x.activo == true), nameof(IT_equipos_checklist_categorias.id), nameof(IT_equipos_checklist_categorias.descripcion))
                , selected: item.id_categoria_ck.ToString(), textoPorDefecto: "-- Seleccionar --");
            return View(item);
        }

        // GET: IT_equipos_checklist_actividades/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.IT_CHECKLIST_EQUIPOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_equipos_checklist_actividades item = db.IT_equipos_checklist_actividades.Find(id);
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

        // POST: IT_equipos_checklist_actividades/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            IT_equipos_checklist_actividades item = db.IT_equipos_checklist_actividades.Find(id);
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

        // GET: IT_equipos_checklist_actividades/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.IT_CHECKLIST_EQUIPOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_equipos_checklist_actividades item = db.IT_equipos_checklist_actividades.Find(id);
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

        // POST: IT_equipos_checklist_actividades/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            IT_equipos_checklist_actividades item = db.IT_equipos_checklist_actividades.Find(id);
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
