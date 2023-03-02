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
        [AllowAnonymous]
        public ActionResult Index(string nombre, int? id_area, bool? incluye_telefono, bool? busqueda, int planta_clave = 0, int pagina = 1)
        {

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            //verifica si debe establecer como null el area
            if (!db.Area.Any(x => x.clave == id_area && x.plantaClave == planta_clave))
                id_area = null;

            //establece el valor por defecto para incluye_telefono
            if (!busqueda.HasValue)
                incluye_telefono = true;

            var cantidadRegistrosPorPagina = 12; // parámetro

            //primero obtiene el listado de empleado sin el filtro de líneas

            List<empleados> listado = new List<empleados>();

            var listadoTemporal = db.empleados
                   .Where(x =>
                       ((x.nombre + " " + x.apellido1 + " " + x.apellido2).Contains(nombre) || String.IsNullOrEmpty(nombre))
                       && (x.planta_clave == planta_clave || planta_clave == 0)
                       && (id_area == null || x.id_area == id_area)
                       && (x.activo.HasValue && x.activo.Value) //si está activo
                       ).ToList();

            //recorre todos los empleados y determina quién si tiene lineas o no        
            foreach (var e in listadoTemporal)
            {
                if (incluye_telefono == null || (!String.IsNullOrEmpty(e.extension) || e.GetIT_Inventory_Cellular_LinesActivas().Count > 0))
                    listado.Add(e);
            }

            //debe ir antes de obtener unicamente tomar los registros de las páginas
            var totalDeRegistros = listado
                 .Count();

            listado = listado
            .OrderBy(x => x.id)
            .Skip((pagina - 1) * cantidadRegistrosPorPagina)
           .Take(cantidadRegistrosPorPagina).ToList();

           

            //para paginación

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["nombre"] = nombre;
            routeValues["planta_clave"] = planta_clave;
            routeValues["id_area"] = id_area;
            routeValues["incluye_telefono"] = incluye_telefono;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.planta_clave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", planta_clave.ToString()), textoPorDefecto: "-- Todas --");
            ViewBag.id_area = AddFirstItem(new SelectList(db.Area.Where(p => p.activo == true && (p.plantaClave == planta_clave)), nameof(Area.clave), nameof(Area.descripcion), planta_clave.ToString()), textoPorDefecto: "-- Todas --");
            ViewBag.Paginacion = paginacion;

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
