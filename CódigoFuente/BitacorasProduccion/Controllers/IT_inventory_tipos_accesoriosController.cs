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
    public class IT_inventory_tipos_accesoriosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_inventory_tipos_accesorios
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                return View(db.IT_inventory_tipos_accesorios.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: IT_inventory_tipos_accesorios/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_tipos_accesorios IT_inventory_tipos_accesorios = db.IT_inventory_tipos_accesorios.Find(id);
                if (IT_inventory_tipos_accesorios == null)
                {
                    return View("../Error/NotFound");
                }
                return View(IT_inventory_tipos_accesorios);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // GET: IT_inventory_tipos_accesorios/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_tipos_accesorios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,activo")] IT_inventory_tipos_accesorios IT_inventory_tipos_accesorios)
        {

            if (ModelState.IsValid)
            {
                //busca si existe in departamento con la misma descripcion
                IT_inventory_tipos_accesorios pfa_busca = db.IT_inventory_tipos_accesorios.Where(s => s.descripcion.ToUpper() == IT_inventory_tipos_accesorios.descripcion.ToUpper() && !String.IsNullOrEmpty(IT_inventory_tipos_accesorios.descripcion))
                                        .FirstOrDefault();

                if (pfa_busca == null)
                { //Si no existe
                    IT_inventory_tipos_accesorios.activo = true;
                    //IT_inventory_tipos_accesorios.descripcion = IT_inventory_tipos_accesorios.descripcion.ToUpper();
                    db.IT_inventory_tipos_accesorios.Add(IT_inventory_tipos_accesorios);
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe un registro con los mismos valores");
                    return View(IT_inventory_tipos_accesorios);
                }
            }
            return View(IT_inventory_tipos_accesorios);

        }

        // GET: IT_inventory_tipos_accesorios/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_tipos_accesorios IT_inventory_tipos_accesorios = db.IT_inventory_tipos_accesorios.Find(id);
                if (IT_inventory_tipos_accesorios == null)
                {
                    return View("../Error/NotFound");
                }
                return View(IT_inventory_tipos_accesorios);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_tipos_accesorios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,activo")] IT_inventory_tipos_accesorios IT_inventory_tipos_accesorios)
        {

            if (ModelState.IsValid)
            {

                //busca si existe in departamento con la misma descripcion
                IT_inventory_tipos_accesorios pfa_busca = db.IT_inventory_tipos_accesorios.Where(s => s.descripcion.ToUpper() == IT_inventory_tipos_accesorios.descripcion.ToUpper() && !String.IsNullOrEmpty(IT_inventory_tipos_accesorios.descripcion) && s.id != IT_inventory_tipos_accesorios.id)
                                        .FirstOrDefault();



                if (pfa_busca == null)
                { //Si no existe
                    //IT_inventory_tipos_accesorios.descripcion = IT_inventory_tipos_accesorios.descripcion.ToUpper();
                    db.Entry(IT_inventory_tipos_accesorios).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe un registro con los mismos valores");
                    return View(IT_inventory_tipos_accesorios);
                }


            }


            return View(IT_inventory_tipos_accesorios);
        }

        // GET: PFA_Departmet/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_tipos_accesorios item = db.IT_inventory_tipos_accesorios.Find(id);
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
            IT_inventory_tipos_accesorios item = db.IT_inventory_tipos_accesorios.Find(id);
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
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_tipos_accesorios item = db.IT_inventory_tipos_accesorios.Find(id);
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
            IT_inventory_tipos_accesorios item = db.IT_inventory_tipos_accesorios.Find(id);
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
