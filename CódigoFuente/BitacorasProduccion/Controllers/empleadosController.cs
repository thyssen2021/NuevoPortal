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
    public class empleadosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: empleados
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.RH))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var empleados = db.empleados.Include(e => e.plantas).Include(e => e.puesto1);
                return View(empleados.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


          
        }

        // GET: empleados/Details/5
        public ActionResult Details(int? id)
        {

            if (TieneRol(TipoRoles.RH))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                empleados empleados = db.empleados.Find(id);
                if (empleados == null)
                {
                    return HttpNotFound();
                }
                return View(empleados);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // GET: empleados/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.RH))
            {
                ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.areaClave = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");                
                ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
          
        }

        // POST: empleados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nueva_fecha_nacimiento,planta_clave,clave,activo,numeroEmpleado,nombre,apellido1,apellido2,nacimientoFecha,correo,telefono,extension,celular,nivel,puesto,compania,ingresoFecha,bajaFecha, C8ID")] empleados empleados, FormCollection collection)
        {
           
            //valores enviados previamente
            int c_planta = 0;
            if(!String.IsNullOrEmpty(collection["planta_clave"])) 
                Int32.TryParse(collection["planta_clave"], out c_planta);
            int c_area = 0;
            if (!String.IsNullOrEmpty(collection["areaClave"]))
                Int32.TryParse(collection["areaClave"].ToString(), out c_area);
            int c_puesto = 0;
            if (!String.IsNullOrEmpty(collection["puesto"]))
                Int32.TryParse(collection["puesto"].ToString(), out c_puesto);

            if (ModelState.IsValid)
            {
                //busca si ya existe un empleado con ese numero de empleado
                empleados empleadoBusca = db.empleados.Where(s => s.numeroEmpleado == empleados.numeroEmpleado)
                                        .FirstOrDefault();
                //no existe el num empleado
                if (empleadoBusca == null)
                {
                    //busca por 8ID
                    empleadoBusca = db.empleados.Where(s => s.C8ID == empleados.C8ID)
                                        .FirstOrDefault();

                    if (empleadoBusca == null)    {

                        empleados.activo = true;
                        //convierte a mayúsculas
                        empleados.nombre = empleados.nombre.ToUpper();
                        empleados.apellido1 = empleados.apellido1.ToUpper();
                        empleados.apellido2 = empleados.apellido2.ToUpper();
                        db.empleados.Add(empleados);
                        db.SaveChanges();
                        TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                        return RedirectToAction("Index");
                    }
                    else {
                        ModelState.AddModelError("", "Ya existe un registro con el mismo 8ID.");
                        ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                        ViewBag.areaClave = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
                        ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
                        //claves seleccionadas
                        ViewBag.c_planta = c_planta;
                        ViewBag.c_area = c_area;
                        ViewBag.c_puesto = c_puesto;
                        return View(empleados);
                    }
                }
                else {
                    ModelState.AddModelError("", "Ya existe un registro con el mismo número de empleado. ");
                    ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                    ViewBag.areaClave = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
                    ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
                    //claves seleccionadas
                    ViewBag.c_planta = c_planta;
                    ViewBag.c_area = c_area;
                    ViewBag.c_puesto = c_puesto;
                    return View(empleados);
                }
            }

            ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.areaClave = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
            //claves seleccionadas
            ViewBag.c_planta = c_planta;
            ViewBag.c_area = c_area;
            ViewBag.c_puesto = c_puesto;
            return View(empleados);
        }

        // GET: empleados/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.RH))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                empleados empleados = db.empleados.Find(id);
                if (empleados == null)
                {
                    return HttpNotFound();
                }

                //valores enviados previamente
                int c_area = 0;
                if (empleados.puesto1 != null && empleados.puesto1.areaClave.HasValue)
                    c_area = empleados.puesto1.areaClave.Value;
               
                ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.areaClave = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
                //claves seleccionadas
                ViewBag.c_planta = empleados.planta_clave;
                ViewBag.c_area = c_area;
                ViewBag.c_puesto = empleados.puesto;
                return View(empleados);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

            
        }

        // POST: empleados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nueva_fecha_nacimiento,planta_clave,clave,activo,numeroEmpleado,nombre,apellido1,apellido2,nacimientoFecha,correo,telefono,extension,celular,nivel,puesto,compania,ingresoFecha,bajaFecha,C8ID")] empleados empleados, FormCollection collection)
        {
            //valores enviados previamente
            int c_planta = 0;
            if (!String.IsNullOrEmpty(collection["planta_clave"]))
                Int32.TryParse(collection["planta_clave"], out c_planta);
            int c_area = 0;
            if (!String.IsNullOrEmpty(collection["areaClave"]))
                Int32.TryParse(collection["areaClave"].ToString(), out c_area);
            int c_puesto = 0;
            if (!String.IsNullOrEmpty(collection["puesto"]))
                Int32.TryParse(collection["puesto"].ToString(), out c_puesto);

            if (ModelState.IsValid)
            {
                //busca si ya existe un empleado con ese numero de empleado
                empleados empleadoBusca = db.empleados.Where(s => s.numeroEmpleado == empleados.numeroEmpleado && s.id != empleados.id)
                                        .FirstOrDefault();

                //no existe otro empleado con el mismo num empleado
                if (empleadoBusca == null)
                {

                    //busca por 8ID
                    empleadoBusca = db.empleados.Where(s => s.C8ID == empleados.C8ID && s.id != empleados.id)
                                        .FirstOrDefault();

                    if (empleadoBusca == null)
                    {
                        empleados.nombre = empleados.nombre.ToUpper();
                        empleados.apellido1 = empleados.apellido1.ToUpper();
                        empleados.apellido2 = empleados.apellido2.ToUpper();
                        db.Entry(empleados).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                        return RedirectToAction("Index");
                    }
                    else {
                        ModelState.AddModelError("", "Ya existe un registro con el mismo 8ID. ");
                        ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                        ViewBag.areaClave = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
                        ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
                        //claves seleccionadas
                        ViewBag.c_planta = c_planta;
                        ViewBag.c_area = c_area;
                        ViewBag.c_puesto = c_puesto;
                        return View(empleados);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe un registro con el mismo número de empleado. ");
                    ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                    ViewBag.areaClave = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
                    ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
                    //claves seleccionadas
                    ViewBag.c_planta = c_planta;
                    ViewBag.c_area = c_area;
                    ViewBag.c_puesto = c_puesto;
                    return View(empleados);
                }

                
            }
            ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.areaClave = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
            //claves seleccionadas
            ViewBag.c_planta = c_planta;
            ViewBag.c_area = c_area;
            ViewBag.c_puesto = c_puesto;
            return View(empleados);
        }

        // GET: Empleados/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.RH))
            {               
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                empleados empleado = db.empleados.Find(id);
                if (empleado == null)
                {
                    return HttpNotFound();
                }
                return View(empleado);
               
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Empleados/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            empleados empleado = db.empleados.Find(id);
            empleado.activo = false;

            db.Entry(empleado).State = EntityState.Modified;
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

        // GET: Empleados/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.RH))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                empleados empleado = db.empleados.Find(id);
                if (empleado == null)
                {
                    return HttpNotFound();
                }
                return View(empleado);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Empleados/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            empleados empleado = db.empleados.Find(id);
            empleado.activo = true;

            db.Entry(empleado).State = EntityState.Modified;
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
