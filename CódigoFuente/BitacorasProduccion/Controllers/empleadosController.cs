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
    public class empleadosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

       
        // GET: empleados
        public ActionResult Index(string nombre, string num_empleado,int? id_jefe_directo ,int planta_clave = 0, int pagina = 1)
        {
            if (TieneRol(TipoRoles.RH))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                var listado = db.empleados
                       .Where(x =>
                           ((x.nombre + " " + x.apellido1 + " " + x.apellido2).Contains(nombre) || String.IsNullOrEmpty(nombre))
                           && (x.numeroEmpleado.Contains(num_empleado) || String.IsNullOrEmpty(num_empleado))
                           && (x.planta_clave == planta_clave || planta_clave == 0)
                           && (id_jefe_directo == null || x.id_jefe_directo == id_jefe_directo)
                           )
                       .OrderBy(x => x.id)
                       .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                      .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.empleados
                      .Where(x =>
                        ((x.nombre + " " + x.apellido1 + " " + x.apellido2).Contains(nombre) || String.IsNullOrEmpty(nombre))
                        && (x.numeroEmpleado.Contains(num_empleado) || String.IsNullOrEmpty(num_empleado))
                        && (x.planta_clave == planta_clave || planta_clave == 0)
                        && (id_jefe_directo == null || x.id_jefe_directo == id_jefe_directo)
                       )
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["nombre"] = nombre;
                routeValues["planta_clave"] = planta_clave;
                routeValues["num_empleado"] = num_empleado;
                routeValues["id_jefe_directo"] = id_jefe_directo;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.planta_clave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", planta_clave.ToString()), textoPorDefecto: "-- Todas --");
                ViewBag.Paginacion = paginacion;

                ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
               textoPorDefecto: "-- Seleccione un valor --", selected: id_jefe_directo.ToString());

                return View(listado);
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
                    return View("../Error/BadRequest");
                }
                empleados empleados = db.empleados.Find(id);
                if (empleados == null)
                {
                    return View("../Error/NotFound");
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
                ViewBag.id_area = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
                    textoPorDefecto: "-- Seleccione un valor --");
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
        public ActionResult Create(empleados empleados, FormCollection collection)
        {

            //valores enviados previamente
            int c_planta = 0;
            if (!String.IsNullOrEmpty(collection["planta_clave"]))
                Int32.TryParse(collection["planta_clave"], out c_planta);
            int c_area = 0;
            if (!String.IsNullOrEmpty(collection["id_area"]))
                Int32.TryParse(collection["id_area"].ToString(), out c_area);
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
                    empleadoBusca = db.empleados.Where(s => s.C8ID == empleados.C8ID && !String.IsNullOrEmpty(empleados.C8ID))
                                        .FirstOrDefault();

                    if (empleadoBusca == null)
                    {

                        empleados.activo = true;
                        //convierte a mayúsculas
                        empleados.nombre = empleados.nombre.ToUpper();
                        empleados.apellido1 = empleados.apellido1.ToUpper();

                        if (!String.IsNullOrEmpty(empleados.apellido2))
                            empleados.apellido2 = empleados.apellido2.ToUpper();
                        //agrega el id_area
                        if (c_area > 0)
                            empleados.id_area = c_area;

                        db.empleados.Add(empleados);
                        db.SaveChanges();
                        TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ya existe un registro con el mismo 8ID.");
                        ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                        ViewBag.id_area = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
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
                    ViewBag.id_area = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
                    ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
                    //claves seleccionadas
                    ViewBag.c_planta = c_planta;
                    ViewBag.c_area = c_area;
                    ViewBag.c_puesto = c_puesto;
                    return View(empleados);
                }
            }

            ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.id_area = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
                  textoPorDefecto: "-- Seleccione un valor --", selected: empleados.id_jefe_directo.ToString());

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
                    return View("../Error/BadRequest");
                }
                empleados empleados = db.empleados.Find(id);
                if (empleados == null)
                {
                    return View("../Error/NotFound");
                }

                ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.id_area = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
                textoPorDefecto: "-- Seleccione un valor --", selected: empleados.id_jefe_directo.ToString());


                //claves seleccionadas
                ViewBag.c_planta = empleados.planta_clave;
                ViewBag.c_area = empleados.id_area;
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
        public async Task<ActionResult> Edit(empleados empleados, FormCollection collection)
        {
            //valores enviados previamente
            int c_planta = 0;
            if (!String.IsNullOrEmpty(collection["planta_clave"]))
                Int32.TryParse(collection["planta_clave"], out c_planta);
            int c_area = 0;
            if (!String.IsNullOrEmpty(collection["id_area"]))
                Int32.TryParse(collection["id_area"].ToString(), out c_area);
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
                    empleadoBusca = db.empleados.Where(s => s.C8ID == empleados.C8ID && s.id != empleados.id && !String.IsNullOrEmpty(empleados.C8ID))
                                        .FirstOrDefault();

                    if (empleadoBusca == null)
                    {

                        //actualiza el correo electronico en la tabla de usuarios
                        var user = _userManager.Users.Where(u => u.IdEmpleado == empleados.id).FirstOrDefault();
                        if (user != null)
                        {
                            //actualiza el usuario  
                            user.Email = empleados.correo;
                            //guarda el usuario en BD
                            var result = await _userManager.UpdateAsync(user);
                        }

                        empleados.nombre = empleados.nombre.ToUpper();
                        empleados.apellido1 = empleados.apellido1.ToUpper();
                        empleados.apellido2 = empleados.apellido2.ToUpper();

                        //agrega el id_area
                        if (c_area > 0)
                            empleados.id_area = c_area;

                        db.Entry(empleados).State = EntityState.Modified;

                        db.SaveChanges();
                        TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ya existe un registro con el mismo 8ID. ");
                        ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                        ViewBag.id_area = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
                        ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
                        ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
                        textoPorDefecto: "-- Seleccione un valor --", selected: empleados.id_jefe_directo.ToString());


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
                    ViewBag.id_area = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
                    ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
                    //claves seleccionadas
                    ViewBag.c_planta = c_planta;
                    ViewBag.c_area = c_area;
                    ViewBag.c_puesto = c_puesto;
                    return View(empleados);
                }


            }
            ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.id_area = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
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
                    return View("../Error/BadRequest");
                }
                empleados empleado = db.empleados.Find(id);
                if (empleado == null)
                {
                    return View("../Error/NotFound");
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
        public ActionResult DisableConfirmed(int id, FormCollection collection)
        {
            empleados empleado = db.empleados.Find(id);
            empleado.activo = false;

            DateTime bajaFecha = DateTime.Now;
            string stringFecha = collection["bajaFecha"];

            try
            {
                if (!String.IsNullOrEmpty(stringFecha))
                {
                    bajaFecha = Convert.ToDateTime(stringFecha);
                    empleado.bajaFecha = bajaFecha;
                }
            }
            catch (FormatException e)
            {
                ModelState.AddModelError("", "Error de formato de fecha: " + e.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al convertir: " + ex.Message);
            }


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
                    return View("../Error/BadRequest");
                }
                empleados empleado = db.empleados.Find(id);
                if (empleado == null)
                {
                    return View("../Error/NotFound");
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
            empleado.bajaFecha = null;

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
