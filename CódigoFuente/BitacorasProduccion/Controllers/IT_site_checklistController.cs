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
    public class IT_site_checklistController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_site_checklist
        public ActionResult Index(int? id_site, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.IT_CHECKLIST_SITE))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var listado = db.IT_site_checklist
               .Where(x => x.id_site == id_site)
               .OrderByDescending(x => x.fecha)
               .ThenByDescending(x => x.id)
               .Skip((pagina - 1) * cantidadRegistrosPorPagina)
              .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.IT_site_checklist
                .Where(x => x.id_site == id_site)
               .Count();

            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id_site"] = id_site;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };
            ViewBag.Paginacion = paginacion;

            ViewBag.id_site = AddFirstItem(new SelectList(db.IT_site.Where(x => x.activo == true), nameof(IT_site.id), nameof(IT_site.ConcatSite)), selected: id_site.ToString(), textoPorDefecto: "-- Seleccionar --");

            return View(listado);
        }

        // GET: IT_site_checklist/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.IT_CHECKLIST_SITE))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_site_checklist iT_site_checklist = db.IT_site_checklist.Find(id);
            if (iT_site_checklist == null)
            {
                return HttpNotFound();
            }
            return View(iT_site_checklist);
        }

        // GET: IT_site_checklist/Create
        public ActionResult Create(int? id_site)
        {
            if (!TieneRol(TipoRoles.IT_CHECKLIST_SITE))
                return View("../Home/ErrorPermisos");

            if (id_site == null)
                return View("../Error/BadRequest");

            IT_site site = db.IT_site.Find(id_site);
            if (site == null)
                return View("../Error/NotFound");

            var sistemas = obtieneEmpleadoLogeado();

            IT_site_checklist model = new IT_site_checklist();
            model.id_sistemas = sistemas.id;
            model.empleados = sistemas;
            model.estatus = Bitacoras.Util.IT_site_checklist_estatus.EN_PROCESO;
            model.id_site = id_site.Value;
            model.IT_site = site;
            model.fecha = DateTime.Now;

            //agrega las actividades al modelo 
            List<IT_site_actividades> listActividades = db.IT_site_actividades.Where(x => x.activo == true).ToList();

            foreach (var item in listActividades)
            {
                model.IT_site_checklist_rel_actividades.Add(new IT_site_checklist_rel_actividades
                {
                    id_it_site_actividad = item.id,
                    estatus = Bitacoras.Util.IT_site_actividad_estatus.OK,
                    IT_site_actividades = item
                });
            }

            return View(model);
        }

        // POST: IT_site_checklist/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IT_site_checklist iT_site_checklist)
        {

            if (ModelState.IsValid)
            {
                db.IT_site_checklist.Add(iT_site_checklist);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index", new { id_site = iT_site_checklist.id_site });
            }
            //asigna los objetos de los valores por defecto
            var sistemas = obtieneEmpleadoLogeado();
            iT_site_checklist.empleados = sistemas;
            iT_site_checklist.IT_site = db.IT_site.Find(iT_site_checklist.id_site);

            //obtiene y asigna los objetos de las actividades
            foreach (var item in iT_site_checklist.IT_site_checklist_rel_actividades)
            {
                item.IT_site_actividades = db.IT_site_actividades.Find(item.id_it_site_actividad);
            }

            return View(iT_site_checklist);
        }

        // GET: IT_site_checklist/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.IT_CHECKLIST_SITE))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_site_checklist iT_site_checklist = db.IT_site_checklist.Find(id);
            if (iT_site_checklist == null)
            {
                return HttpNotFound();
            }
           
            return View(iT_site_checklist);
        }

        // POST: IT_site_checklist/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IT_site_checklist iT_site_checklist)
        {
           
            if (ModelState.IsValid)
            {
                //borra los conceptos anteriores
                var list = db.IT_site_checklist_rel_actividades.Where(x => x.id_site_checklist == iT_site_checklist.id);
                foreach (var itemRemove in list)
                    db.IT_site_checklist_rel_actividades.Remove(itemRemove);

                //agrega los nuevos items rel 
                foreach (var iteamAdd in iT_site_checklist.IT_site_checklist_rel_actividades)
                    db.IT_site_checklist_rel_actividades.Add(iteamAdd);

                db.Entry(iT_site_checklist).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index", new { id_site = iT_site_checklist.id_site });
            }

            //asigna los objetos de los valores por defecto
            var sistemas = obtieneEmpleadoLogeado();
            iT_site_checklist.empleados = sistemas;
            iT_site_checklist.IT_site = db.IT_site.Find(iT_site_checklist.id_site);

            //obtiene y asigna los objetos de las actividades
            foreach (var item in iT_site_checklist.IT_site_checklist_rel_actividades)
            {
                item.IT_site_actividades = db.IT_site_actividades.Find(item.id_it_site_actividad);
            }

            return View(iT_site_checklist);
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
