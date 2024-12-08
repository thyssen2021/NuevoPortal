using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class OT_zona_fallaController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: OT_zona_falla
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.OT_CATALOGOS))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                empleados emp = obtieneEmpleadoLogeado();

                return View(db.OT_zona_falla.Where(x => x.produccion_lineas.clave_planta == emp.planta_clave));
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: OT_zona_falla/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.OT_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                OT_zona_falla OT_zona_falla = db.OT_zona_falla.Find(id);
                if (OT_zona_falla == null)
                {
                    return View("../Error/NotFound");
                }
                return View(OT_zona_falla);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: OT_zona_falla/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.OT_CATALOGOS))
            {
                empleados emp = obtieneEmpleadoLogeado();
                ViewBag.id_linea = AddFirstItem(new SelectList(db.produccion_lineas.Where(x => x.activo.HasValue && x.activo.Value && x.clave_planta == emp.planta_clave), "id", "linea"));
                ViewBag.planta = emp.plantas;
                return View(new OT_zona_falla { });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: OT_zona_falla/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OT_zona_falla OT_zona_falla)
        {
            bool existe = db.OT_zona_falla.Any(x => x.id_linea == OT_zona_falla.id_linea && x.zona_falla.ToUpper() == OT_zona_falla.zona_falla.ToUpper());
            if (existe)
                ModelState.AddModelError("", "Esta Zona de Falla ya se encuentra registrada para esta Línea de Producción.");

            if (ModelState.IsValid)
            {
                OT_zona_falla.zona_falla = OT_zona_falla.zona_falla.ToUpper();
                db.OT_zona_falla.Add(OT_zona_falla);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            empleados emp = obtieneEmpleadoLogeado();

            ViewBag.id_linea = AddFirstItem(new SelectList(db.produccion_lineas.Where(x => x.activo.HasValue && x.activo.Value && x.clave_planta == emp.planta_clave), "id", "linea"));
            ViewBag.planta = emp.plantas;

            return View(OT_zona_falla);
        }

        // GET: OT_zona_falla/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.OT_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                OT_zona_falla OT_zona_falla = db.OT_zona_falla.Find(id);
                if (OT_zona_falla == null)
                {
                    return View("../Error/NotFound");
                }

                empleados emp = obtieneEmpleadoLogeado();
                ViewBag.id_linea = AddFirstItem(new SelectList(db.produccion_lineas.Where(x => x.activo.HasValue && x.activo.Value && x.clave_planta == emp.planta_clave), "id", "linea"), selected: OT_zona_falla.id_linea.ToString()); ViewBag.planta = emp.plantas;
                ViewBag.planta = emp.plantas;

                return View(OT_zona_falla);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: OT_zona_falla/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OT_zona_falla OT_zona_falla)
        {
            bool existe = db.OT_zona_falla.Any(x => x.id_linea == OT_zona_falla.id_linea && x.zona_falla.ToUpper() == OT_zona_falla.zona_falla.ToUpper() && x.id != OT_zona_falla.id);
            if (existe)
                ModelState.AddModelError("", "Esta Zona de Falla ya se encuentra registrada para esta Línea de Producción.");


            if (ModelState.IsValid)
            {
                OT_zona_falla.zona_falla = OT_zona_falla.zona_falla.ToUpper(); 
                db.Entry(OT_zona_falla).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            empleados emp = obtieneEmpleadoLogeado();
            ViewBag.id_linea = AddFirstItem(new SelectList(db.produccion_lineas.Where(x => x.activo.HasValue && x.activo.Value && x.clave_planta == emp.planta_clave), "id", "linea"),selected: OT_zona_falla.id_linea.ToString());
            ViewBag.planta = emp.plantas;
            return View(OT_zona_falla);
        }



        // GET: OT_zona_falla/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.OT_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                OT_zona_falla item = db.OT_zona_falla.Find(id);
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
            OT_zona_falla item = db.OT_zona_falla.Find(id);
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
                OT_zona_falla item = db.OT_zona_falla.Find(id);
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
            OT_zona_falla item = db.OT_zona_falla.Find(id);
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
