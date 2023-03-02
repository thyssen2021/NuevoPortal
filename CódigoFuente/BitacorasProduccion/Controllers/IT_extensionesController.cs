using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_extensionesController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_extensiones
        public ActionResult Index(string nombre, string num_empleado, int? id_jefe_directo, int planta_clave = 0, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
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

            if (TieneRol(TipoRoles.IT_INVENTORY))
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
                ViewBag.PrimerNivel = "sistemas";
                ViewBag.SegundoNivel = "IT_Catalogos_inventory_telefonia";
                ViewBag.TercerNivel = "IT_extensiones";
                ViewBag.ControllerName = "IT_extensiones";
                return View("../empleados/details", empleados);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: empleados/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.IT_INVENTORY))
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

        // POST: empleados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(empleados empleados)
        {                                 
            
            //if (ModelState.IsValid)
            //{
                var empBD = db.empleados.Find(empleados.id);

                empBD.extension = empleados.extension;
                empBD.mostrar_telefono = empleados.mostrar_telefono;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar en BD_ " + ex.Message);
                }
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            //}

             // return View(empleados);
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
