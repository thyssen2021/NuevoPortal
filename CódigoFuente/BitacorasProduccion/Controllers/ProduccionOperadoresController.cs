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
    public class ProduccionOperadoresController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: ProduccionOperadores
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var produccion_operadores = db.produccion_operadores.Include(p => p.empleados).Include(p => p.produccion_lineas);
                return View(produccion_operadores.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // GET: ProduccionOperadores/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                produccion_operadores produccion_operadores = db.produccion_operadores.Find(id);
                if (produccion_operadores == null)
                {
                    return View("../Error/NotFound");
                }
                return View(produccion_operadores);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }            
        }

        // GET: ProduccionOperadores/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                ViewBag.id_empleado = new SelectList(db.empleados.Where(p => p.activo == true), "id", "numeroEmpleado");
                ViewBag.id_linea = new SelectList(db.produccion_lineas.Where(p => p.activo == true), "id", "linea");
                ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
           
        }

        // POST: ProduccionOperadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_empleado,id_linea,activo")] produccion_operadores produccion_operadores, FormCollection collection)
        {

            //valores enviados previamente
            int c_planta = 0;
            if (!String.IsNullOrEmpty(collection["clave_planta"]))
                Int32.TryParse(collection["clave_planta"].ToString(), out c_planta);

            //valores enviados previamente
            int id_empleado = 0;
            if (!String.IsNullOrEmpty(collection["id_empleado"]))
                Int32.TryParse(collection["id_empleado"].ToString(), out id_empleado);

            int id_linea = 0;
            if (!String.IsNullOrEmpty(collection["id_linea"]))
                Int32.TryParse(collection["id_linea"].ToString(), out id_linea);

            if (c_planta != 0 && id_empleado != 0 && id_linea != 0)
            {

                if (ModelState.IsValid)
                {
                    //busca si ya existe un empleado con ese numero de empleado
                    produccion_operadores operadorBusca = db.produccion_operadores.Where(s => s.id_empleado == produccion_operadores.id_empleado && s.id_linea ==produccion_operadores.id_linea)
                                            .FirstOrDefault();
                    //no existe el num empleado
                    if (operadorBusca == null)
                    {

                        produccion_operadores.activo = true;
                        db.produccion_operadores.Add(produccion_operadores);
                        db.SaveChanges();
                        TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                        return RedirectToAction("Index");
                    }
                    else {
                        ModelState.AddModelError("", "Este empleado ya se encuentra registado para esta línea de producción.");
                    }
                }
            }
            else {
                ModelState.AddModelError("", "Complete todos los campos para continuar.");
            }
            

            ViewBag.id_empleado = new SelectList(db.empleados.Where(p => p.activo == true), "id", "numeroEmpleado", produccion_operadores.id_empleado);
            ViewBag.id_linea = new SelectList(db.produccion_lineas.Where(p => p.activo == true), "id", "linea", produccion_operadores.id_linea);
            ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.C_Planta = c_planta;
            ViewBag.IdEmpleado = id_empleado;
            ViewBag.IdLinea = id_linea;
            return View(produccion_operadores);


        }

        // GET: ProduccionOperadores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                produccion_operadores produccion_operadores = db.produccion_operadores.Find(id);
                if (produccion_operadores == null)
                {
                    return View("../Error/NotFound");
                }
                ViewBag.id_empleado = new SelectList(db.empleados.Where(p => p.activo == true), "id", "numeroEmpleado", produccion_operadores.id_empleado);
                ViewBag.id_linea = new SelectList(db.produccion_lineas.Where(p => p.activo == true), "id", "linea", produccion_operadores.id_linea);
                ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.C_Planta = produccion_operadores.produccion_lineas.clave_planta;
                ViewBag.IdEmpleado = produccion_operadores.id_empleado;
                ViewBag.IdLinea = produccion_operadores.id_linea;
                return View(produccion_operadores);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
           
        }

        // POST: ProduccionOperadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_empleado,id_linea,activo")] produccion_operadores produccion_operadores, FormCollection collection)
        {

            //valores enviados previamente
            int c_planta = 0;
            if (!String.IsNullOrEmpty(collection["clave_planta"]))
                Int32.TryParse(collection["clave_planta"].ToString(), out c_planta);

            //valores enviados previamente
            int id_empleado = 0;
            if (!String.IsNullOrEmpty(collection["id_empleado"]))
                Int32.TryParse(collection["id_empleado"].ToString(), out id_empleado);

            int id_linea = 0;
            if (!String.IsNullOrEmpty(collection["id_linea"]))
                Int32.TryParse(collection["id_linea"].ToString(), out id_linea);
            if (c_planta != 0 && id_empleado != 0 && id_linea != 0)
            {

                if (ModelState.IsValid)
                {
                    //busca si ya existe un empleado con ese numero de empleado
                    produccion_operadores operadorBusca = db.produccion_operadores.Where(s => s.id_empleado == produccion_operadores.id_empleado && s.id_linea == produccion_operadores.id_linea && produccion_operadores.id != s.id)
                                            .FirstOrDefault();
                    //no existe el num empleado
                    if (operadorBusca == null)
                    {
                        db.Entry(produccion_operadores).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Este empleado ya se encuentra registado para esta línea de producción.");
                    }

                }
            }
            else
            {
                ModelState.AddModelError("", "Complete todos los campos para continuar.");
            }

            ViewBag.id_empleado = new SelectList(db.empleados.Where(p => p.activo == true), "id", "numeroEmpleado", produccion_operadores.id_empleado);
            ViewBag.id_linea = new SelectList(db.produccion_lineas.Where(p => p.activo == true), "id", "linea", produccion_operadores.id_linea);
            ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.C_Planta = c_planta;
            ViewBag.IdEmpleado = id_empleado;
            ViewBag.IdLinea = id_linea;
            return View(produccion_operadores);
        }

        // GET: ProduccionOperadores/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                produccion_operadores valor = db.produccion_operadores.Find(id);
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

        // POST: ProduccionOperadores/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            produccion_operadores valor = db.produccion_operadores.Find(id);
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

        // GET: ProduccionOperadores/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                produccion_operadores valor = db.produccion_operadores.Find(id);
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

        // POST: ProduccionOperadores/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            produccion_operadores valor = db.produccion_operadores.Find(id);
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
