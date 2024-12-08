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
    public class ProduccionSupervisoresController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: ProduccionSupervisores
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var produccion_supervisores = db.produccion_supervisores.Include(p => p.empleados).Include(p => p.plantas);
                return View(produccion_supervisores.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // GET: ProduccionSupervisores/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                produccion_supervisores produccion_supervisores = db.produccion_supervisores.Find(id);
                if (produccion_supervisores == null)
                {
                    return View("../Error/NotFound");
                }
                return View(produccion_supervisores);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // GET: ProduccionSupervisores/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                ViewBag.id_empleado = new SelectList(db.empleados.Where(p => p.activo == true), "id", "numeroEmpleado");
                ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
           
        }

        // POST: ProduccionSupervisores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_empleado,clave_planta,activo")] produccion_supervisores produccion_supervisores, FormCollection collection)
        {
            //valores enviados previamente
            int c_planta = 0;
            if (!String.IsNullOrEmpty(collection["clave_planta"]))
                Int32.TryParse(collection["clave_planta"].ToString(), out c_planta);

            //valores enviados previamente
            int id_empleado = 0;
            if (!String.IsNullOrEmpty(collection["id_empleado"]))
                Int32.TryParse(collection["id_empleado"].ToString(), out id_empleado);

            if (ModelState.IsValid)
            {

                //busca si ya existe un empleado con ese numero de empleado
                produccion_supervisores supervisorBusca = db.produccion_supervisores.Where(s => s.id_empleado == produccion_supervisores.id_empleado && s.clave_planta == produccion_supervisores.clave_planta)
                                        .FirstOrDefault();
                //no existe el num empleado
                if (supervisorBusca == null)
                {

                    produccion_supervisores.activo = true;
                    db.produccion_supervisores.Add(produccion_supervisores);
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Este empleado ya se encuentra registado como supervisor para esta planta.");
                }

               
            }

            ViewBag.id_empleado = new SelectList(db.empleados.Where(p => p.activo == true), "id", "numeroEmpleado", produccion_supervisores.id_empleado);
            ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", produccion_supervisores.clave_planta);
            ViewBag.C_Planta = c_planta;
            ViewBag.IdEmpleado = id_empleado;
            return View(produccion_supervisores);
        }

        // GET: ProduccionSupervisores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                produccion_supervisores produccion_supervisores = db.produccion_supervisores.Find(id);
                if (produccion_supervisores == null)
                {
                    return View("../Error/NotFound");
                }
                ViewBag.id_empleado = new SelectList(db.empleados.Where(p => p.activo == true), "id", "numeroEmpleado", produccion_supervisores.id_empleado);
                ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", produccion_supervisores.clave_planta);
                ViewBag.C_Planta = produccion_supervisores.clave_planta;
                ViewBag.IdEmpleado = produccion_supervisores.id_empleado;
                return View(produccion_supervisores);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
           
        }

        // POST: ProduccionSupervisores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_empleado,clave_planta,activo")] produccion_supervisores produccion_supervisores, FormCollection collection)
        {
            //valores enviados previamente
            int c_planta = 0;
            if (!String.IsNullOrEmpty(collection["clave_planta"]))
                Int32.TryParse(collection["clave_planta"].ToString(), out c_planta);

            //valores enviados previamente
            int id_empleado = 0;
            if (!String.IsNullOrEmpty(collection["id_empleado"]))
                Int32.TryParse(collection["id_empleado"].ToString(), out id_empleado);

            if (ModelState.IsValid)
            {
                db.Entry(produccion_supervisores).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.id_empleado = new SelectList(db.empleados.Where(p => p.activo == true), "id", "numeroEmpleado", produccion_supervisores.id_empleado);
            ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", produccion_supervisores.clave_planta);
            ViewBag.C_Planta = c_planta;
            ViewBag.IdEmpleado = id_empleado;
            return View(produccion_supervisores);
        }

        // GET: ProduccionSupervisores/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                produccion_supervisores valor = db.produccion_supervisores.Find(id);
                if (valor == null)
                {
                    return View("../Error/NotFound");
                }
                return View(valor);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: ProduccionSupervisores/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            produccion_supervisores valor = db.produccion_supervisores.Find(id);
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

        // GET: ProduccionSupervisores/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                produccion_supervisores valor = db.produccion_supervisores.Find(id);
                if (valor == null)
                {
                    return View("../Error/NotFound");
                }
                return View(valor);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: ProduccionSupervisores/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            produccion_supervisores valor = db.produccion_supervisores.Find(id);
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
