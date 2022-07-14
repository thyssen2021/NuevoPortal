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
    public class IT_inventory_items_genericosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_inventory_items_genericos
        public ActionResult Index(int? tipo_hardware, int? id_tipo_accesorio, bool? active, int pagina = 1)
        {

            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var tipoHardware = db.IT_inventory_hardware_type.Find(tipo_hardware);
            bool esAccesorio = false;

            if (tipoHardware != null && tipoHardware.descripcion == Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES)
                esAccesorio = true;
            else
                //si no es accesorio borra el tipo de accesorio
                id_tipo_accesorio = null;

            ViewBag.EsAccesorio = esAccesorio;

            var listado = db.IT_inventory_items_genericos
                   .Where(x =>
                   (x.id_tipo_accesorio == id_tipo_accesorio || id_tipo_accesorio == null)
                   && (x.active == active || active == null)
                   && (x.id_inventory_type == tipo_hardware || tipo_hardware == null)
                   )
                 .OrderByDescending(x => x.id_inventory_type)
                 .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.IT_inventory_items_genericos
                   .Where(x =>
                   (x.id_tipo_accesorio == id_tipo_accesorio || id_tipo_accesorio == null)
                   && (x.active == active || active == null)
                   && (x.id_inventory_type == tipo_hardware || tipo_hardware == null)
                   )
                     .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id_tipo_accesorio"] = id_tipo_accesorio;
            routeValues["tipo_hardware"] = tipo_hardware;
            routeValues["active"] = active;
            routeValues["pagina"] = pagina;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };


            ViewBag.Paginacion = paginacion;
            ViewBag.tipo_hardware = AddFirstItem(new SelectList(db.IT_inventory_hardware_type.Where(x => x.activo && x.puede_asignarse), "id", "descripcion"), textoPorDefecto: "-- Select --", selected: tipo_hardware.ToString());
            ViewBag.id_tipo_accesorio = AddFirstItem(new SelectList(db.IT_inventory_tipos_accesorios, "id", "descripcion"), textoPorDefecto: "-- All --", selected: id_tipo_accesorio.ToString());

            return View(listado);

        }

        // GET: IT_inventory_items_genericos/Details/5
        public ActionResult Details(int? id)
        {

            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_inventory_items_genericos iT_inventory_items_genericos = db.IT_inventory_items_genericos.Find(id);
            if (iT_inventory_items_genericos == null)
            {
                return HttpNotFound();
            }
            return View(iT_inventory_items_genericos);
        }

        // GET: IT_inventory_items_genericos/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            ViewBag.id_inventory_type = AddFirstItem(new SelectList(db.IT_inventory_hardware_type.Where(x => x.activo == true && x.puede_asignarse), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            ViewBag.id_tipo_accesorio = AddFirstItem(new SelectList(db.IT_inventory_tipos_accesorios.Where(x => x.activo == true), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");

            return View();
        }

        // POST: IT_inventory_items_genericos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( IT_inventory_items_genericos iT_inventory_items_genericos)
        {
            if (ModelState.IsValid)
            {
                iT_inventory_items_genericos.active = true;
                db.IT_inventory_items_genericos.Add(iT_inventory_items_genericos);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            ViewBag.id_inventory_type = AddFirstItem(new SelectList(db.IT_inventory_hardware_type.Where(x => x.activo == true && x.puede_asignarse), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items_genericos.id_inventory_type.ToString());
            ViewBag.id_tipo_accesorio = AddFirstItem(new SelectList(db.IT_inventory_tipos_accesorios.Where(x => x.activo == true), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items_genericos.id_tipo_accesorio.ToString());


            return View(iT_inventory_items_genericos);
        }

        // GET: IT_inventory_items_genericos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_inventory_items_genericos iT_inventory_items_genericos = db.IT_inventory_items_genericos.Find(id);
            if (iT_inventory_items_genericos == null)
            {
                return HttpNotFound();
            }

            ViewBag.id_inventory_type = AddFirstItem(new SelectList(db.IT_inventory_hardware_type.Where(x => x.activo == true && x.puede_asignarse), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items_genericos.id_inventory_type.ToString());
            ViewBag.id_tipo_accesorio = AddFirstItem(new SelectList(db.IT_inventory_tipos_accesorios.Where(x => x.activo == true), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items_genericos.id_tipo_accesorio.ToString());


            return View(iT_inventory_items_genericos);
        }

        // POST: IT_inventory_items_genericos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IT_inventory_items_genericos iT_inventory_items_genericos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iT_inventory_items_genericos).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.id_inventory_type = AddFirstItem(new SelectList(db.IT_inventory_hardware_type.Where(x => x.activo == true && x.puede_asignarse), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items_genericos.id_inventory_type.ToString());
            ViewBag.id_tipo_accesorio = AddFirstItem(new SelectList(db.IT_inventory_tipos_accesorios.Where(x => x.activo == true), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items_genericos.id_tipo_accesorio.ToString());

            return View(iT_inventory_items_genericos);
        }

        // GET: IT_inventory_items_genericos/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items_genericos item = db.IT_inventory_items_genericos.Find(id);
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

        // POST: IT_inventory_items_genericos/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            IT_inventory_items_genericos item = db.IT_inventory_items_genericos.Find(id);
            item.active = false;

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

        // GET: IT_inventory_items_genericos/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items_genericos item = db.IT_inventory_items_genericos.Find(id);
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

        // POST: IT_inventory_items_genericos/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            IT_inventory_items_genericos item = db.IT_inventory_items_genericos.Find(id);
            item.active = true;

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
