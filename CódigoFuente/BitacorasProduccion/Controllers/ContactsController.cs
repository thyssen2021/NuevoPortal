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
    public class ContactsController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: Contacts
        public ActionResult Index(string nombre, string num_empleado, int? id_jefe_directo, int? id_area, int planta_clave = 0, int pagina = 1)
        {

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            if (!db.Area.Any(x => x.clave == id_area && x.plantaClave == planta_clave))
                id_area = null;

            var cantidadRegistrosPorPagina = 12; // parámetro

            var listado = db.empleados
                   .Where(x =>
                       ((x.nombre + " " + x.apellido1 + " " + x.apellido2).Contains(nombre) || String.IsNullOrEmpty(nombre))
                       && (x.numeroEmpleado.Contains(num_empleado) || String.IsNullOrEmpty(num_empleado))
                       && (x.planta_clave == planta_clave || planta_clave == 0)
                       && (id_area==null || x.id_area == id_area)
                       && (x.activo.HasValue && x.activo.Value) //si está activo
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
                       && (id_area == null || x.id_area == id_area)
                       && (x.activo.HasValue && x.activo.Value) //si está activo
                       && (id_jefe_directo == null || x.id_jefe_directo == id_jefe_directo)
                       )
                .Count();

            //para paginación

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["nombre"] = nombre;
            routeValues["planta_clave"] = planta_clave;            
            routeValues["id_area"] = id_area;            

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.planta_clave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", planta_clave.ToString()), textoPorDefecto: "-- Todas --");
            ViewBag.id_area = AddFirstItem(new SelectList(db.Area.Where(p => p.activo == true && ( p.plantaClave == planta_clave)), nameof(Area.clave), nameof(Area.descripcion), planta_clave.ToString()), textoPorDefecto: "-- Todas --");
            ViewBag.Paginacion = paginacion;

            ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
           textoPorDefecto: "-- Seleccione un valor --", selected: id_jefe_directo.ToString());

            return View(listado);

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
