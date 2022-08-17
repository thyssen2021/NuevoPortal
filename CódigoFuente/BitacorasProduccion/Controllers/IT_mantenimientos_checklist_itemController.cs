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
    public class IT_mantenimientos_checklist_itemController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_mantenimientos_checklist_item
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.IT_MANTENIMIENTO_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var IT_mantenimientos_checklist_item = db.IT_mantenimientos_checklist_item.OrderBy(x=>x.id_IT_mantenimientos_checklist_categorias);
                return View(IT_mantenimientos_checklist_item.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
           
        }

        // GET: IT_mantenimientos_checklist_item/Details/5
        public ActionResult Details(int? id)
        {

            if (TieneRol(TipoRoles.IT_MANTENIMIENTO_REGISTRO))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                plantas plantas = db.plantas.Find(id);
                IT_mantenimientos_checklist_item IT_mantenimientos_checklist_item = db.IT_mantenimientos_checklist_item.Find(id);
                if (IT_mantenimientos_checklist_item == null)
                {
                    return View("../Error/NotFound");
                }
                return View(IT_mantenimientos_checklist_item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

           
        }

        // GET: IT_mantenimientos_checklist_item/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.IT_MANTENIMIENTO_REGISTRO))
            {
                ViewBag.id_IT_mantenimientos_checklist_categorias = AddFirstItem(new SelectList(db.IT_mantenimientos_checklist_categorias.Where(x => x.activo == true ), nameof(IT_mantenimientos_checklist_categorias.id), nameof(IT_mantenimientos_checklist_categorias.descripcion))
                    , textoPorDefecto: "-- Seleccionar --");

                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // POST: IT_mantenimientos_checklist_item/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IT_mantenimientos_checklist_item IT_mantenimientos_checklist_item)
        {
            if (ModelState.IsValid)
            {

                //busca si ya exite un IT_mantenimientos_checklist_item con esos valores
                IT_mantenimientos_checklist_item IT_mantenimientos_checklist_itemBusca = db.IT_mantenimientos_checklist_item.Where(
                    s => s.id_IT_mantenimientos_checklist_categorias == IT_mantenimientos_checklist_item.id_IT_mantenimientos_checklist_categorias && s.descripcion.ToUpper() == IT_mantenimientos_checklist_item.descripcion.ToUpper()
                    )
                                        .FirstOrDefault();

                if (IT_mantenimientos_checklist_itemBusca == null) //no existe el IT_mantenimientos_checklist_item
                {
                    IT_mantenimientos_checklist_item.activo = true;
                      
                    db.IT_mantenimientos_checklist_item.Add(IT_mantenimientos_checklist_item);
                    db.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe un registro con los mismos valores.");
                    ViewBag.id_IT_mantenimientos_checklist_categorias = AddFirstItem(new SelectList(db.IT_mantenimientos_checklist_categorias.Where(x => x.activo == true), nameof(IT_mantenimientos_checklist_categorias.id), nameof(IT_mantenimientos_checklist_categorias.descripcion))
                , textoPorDefecto: "-- Seleccionar --", selected: IT_mantenimientos_checklist_item.id_IT_mantenimientos_checklist_categorias.ToString());
                    return View(IT_mantenimientos_checklist_item);
                }

                //para mostrar la alerta de exito
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
               
                return RedirectToAction("Index");
            }

            ViewBag.id_IT_mantenimientos_checklist_categorias = AddFirstItem(new SelectList(db.IT_mantenimientos_checklist_categorias.Where(x => x.activo == true), nameof(IT_mantenimientos_checklist_categorias.id), nameof(IT_mantenimientos_checklist_categorias.descripcion))
                          , textoPorDefecto: "-- Seleccionar --", selected: IT_mantenimientos_checklist_item.id_IT_mantenimientos_checklist_categorias.ToString());
            return View(IT_mantenimientos_checklist_item);
        }

        // GET: IT_mantenimientos_checklist_item/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.IT_MANTENIMIENTO_REGISTRO))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_mantenimientos_checklist_item IT_mantenimientos_checklist_item = db.IT_mantenimientos_checklist_item.Find(id);
                if (IT_mantenimientos_checklist_item == null)
                {
                    return View("../Error/NotFound");
                }

                ViewBag.id_IT_mantenimientos_checklist_categorias = AddFirstItem(new SelectList(db.IT_mantenimientos_checklist_categorias.Where(x => x.activo == true), nameof(IT_mantenimientos_checklist_categorias.id), nameof(IT_mantenimientos_checklist_categorias.descripcion))
                  , textoPorDefecto: "-- Seleccionar --", selected: IT_mantenimientos_checklist_item.id_IT_mantenimientos_checklist_categorias.ToString());

                return View(IT_mantenimientos_checklist_item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_mantenimientos_checklist_item/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IT_mantenimientos_checklist_item IT_mantenimientos_checklist_item)
        {
            //busca si ya exite un IT_mantenimientos_checklist_item con esos valores
            IT_mantenimientos_checklist_item IT_mantenimientos_checklist_itemBusca = db.IT_mantenimientos_checklist_item.Where(
                s => s.id_IT_mantenimientos_checklist_categorias == IT_mantenimientos_checklist_item.id_IT_mantenimientos_checklist_categorias && s.descripcion.ToUpper() == IT_mantenimientos_checklist_item.descripcion.ToUpper()
                && s.id != IT_mantenimientos_checklist_item.id
                )
                .FirstOrDefault();

            if(IT_mantenimientos_checklist_itemBusca != null)
                ModelState.AddModelError("", "Ya existe un valor con los mismos valores.");

            if (ModelState.IsValid)
            {
               
                db.Entry(IT_mantenimientos_checklist_item).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.id_IT_mantenimientos_checklist_categorias = AddFirstItem(new SelectList(db.IT_mantenimientos_checklist_categorias.Where(x => x.activo == true), nameof(IT_mantenimientos_checklist_categorias.id), nameof(IT_mantenimientos_checklist_categorias.descripcion))
                        , textoPorDefecto: "-- Seleccionar --", selected: IT_mantenimientos_checklist_item.id_IT_mantenimientos_checklist_categorias.ToString());
            return View(IT_mantenimientos_checklist_item);
        }

        // GET: IT_mantenimientos_checklist_item/Disable/5
        public ActionResult Disable(int? id)
        {

            if (TieneRol(TipoRoles.IT_MANTENIMIENTO_REGISTRO))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }               
                IT_mantenimientos_checklist_item IT_mantenimientos_checklist_item = db.IT_mantenimientos_checklist_item.Find(id);
                if (IT_mantenimientos_checklist_item == null)
                {
                    return View("../Error/NotFound");
                }
                return View(IT_mantenimientos_checklist_item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }           
        }

        // POST: IT_mantenimientos_checklist_item/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            IT_mantenimientos_checklist_item IT_mantenimientos_checklist_item = db.IT_mantenimientos_checklist_item.Find(id);
            IT_mantenimientos_checklist_item.activo = false;

            db.Entry(IT_mantenimientos_checklist_item).State = EntityState.Modified;
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

        // GET: IT_mantenimientos_checklist_item/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.IT_MANTENIMIENTO_REGISTRO))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_mantenimientos_checklist_item IT_mantenimientos_checklist_item = db.IT_mantenimientos_checklist_item.Find(id);
                if (IT_mantenimientos_checklist_item == null)
                {
                    return View("../Error/NotFound");
                }
                return View(IT_mantenimientos_checklist_item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_mantenimientos_checklist_item/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            IT_mantenimientos_checklist_item IT_mantenimientos_checklist_item = db.IT_mantenimientos_checklist_item.Find(id);
            IT_mantenimientos_checklist_item.activo = true;

            db.Entry(IT_mantenimientos_checklist_item).State = EntityState.Modified;
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
