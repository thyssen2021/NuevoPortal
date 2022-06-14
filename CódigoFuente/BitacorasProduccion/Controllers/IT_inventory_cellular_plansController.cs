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
    public class IT_inventory_cellular_plansController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_inventory_cellular_plans
        public ActionResult Index(string num_telefono, bool? activo, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listado = db.IT_inventory_cellular_plans
                    .Where(x =>
                    (x.num_telefono.Contains(num_telefono) || String.IsNullOrEmpty(num_telefono))
                    && (x.activo == activo || activo == null)
                    )
                  //porteriormente ordenar por id_planta
                  .OrderByDescending(x => x.id)
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_cellular_plans
                      .Where(x =>
                    (x.num_telefono.Contains(num_telefono) || String.IsNullOrEmpty(num_telefono))
                    && (x.activo == activo || activo == null)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["num_telefono"] = num_telefono;
                routeValues["activo"] = activo;
                routeValues["pagina"] = pagina;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;

                /*Pendiente hasta realizar la asignacion de empleados */
                //ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        public ActionResult Exportar(string num_telefono, bool? activo)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para desktop
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("desktop"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();


                var listado = db.IT_inventory_cellular_plans
                    .Where(x =>
                    (x.num_telefono.Contains(num_telefono) || String.IsNullOrEmpty(num_telefono))
                    && (x.activo == activo || activo == null)
                    )
                  //porteriormente ordenar por id_planta
                  .OrderByDescending(x => x.id)
                 .ToList();


                byte[] stream = ExcelUtil.GeneraReporteITPlanesTelefoniaExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventorio_Planes_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(stream, "application/vnd.ms-excel");
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: IT_inventory_cellular_plans/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_cellular_plans iT_inventory_cellular_plans = db.IT_inventory_cellular_plans.Find(id);
                if (iT_inventory_cellular_plans == null)
                {
                    return View("../Error/NotFound");
                }
                return View(iT_inventory_cellular_plans);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: IT_inventory_cellular_plans/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                decimal porcentaje_iva = 16.0M;

                //busca el tipo inventario para desktop
                //ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());
                var type = GetHardware_TypeCell();
                ViewBag.id_it_inventory_items = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x => x.id_inventory_type == type.id && x.active == true), "id", "ConcatInfoSmartphone"));
                return View(new IT_inventory_cellular_plans { activo = true, porcentaje_iva = porcentaje_iva });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_cellular_plans/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IT_inventory_cellular_plans iT_inventory_cellular_plans)
        {
            // ModelState.AddModelError("", "Ejemplo de error.");

            if (db.IT_inventory_cellular_plans.Any(x => x.id_it_inventory_items == iT_inventory_cellular_plans.id_it_inventory_items && iT_inventory_cellular_plans.id_it_inventory_items > 0))
                ModelState.AddModelError("", "El equipo seleccionado ya se encuentra asignado a otro plan.");

            if (db.IT_inventory_cellular_plans.Any(x => x.num_telefono == iT_inventory_cellular_plans.num_telefono && !String.IsNullOrEmpty(iT_inventory_cellular_plans.num_telefono)))
                ModelState.AddModelError("", "El número de celular ya se encuentra asignado a otro plan.");

            if (ModelState.IsValid)
            {
                db.IT_inventory_cellular_plans.Add(iT_inventory_cellular_plans);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            var type = GetHardware_TypeCell();
            ViewBag.id_it_inventory_items = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x => x.id_inventory_type == type.id && x.active == true), "id", "ConcatInfoSmartphone"), selected: iT_inventory_cellular_plans.id_it_inventory_items.ToString());
            return View(iT_inventory_cellular_plans);
        }

        // GET: IT_inventory_cellular_plans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_cellular_plans iT_inventory_cellular_plans = db.IT_inventory_cellular_plans.Find(id);
                if (iT_inventory_cellular_plans == null)
                {
                    return View("../Error/NotFound");
                }
                var type = GetHardware_TypeCell();
                ViewBag.id_it_inventory_items = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x => x.id_inventory_type == type.id && x.active == true), "id", "ConcatInfoSmartphone"), selected: iT_inventory_cellular_plans.id_it_inventory_items.ToString());
                return View(iT_inventory_cellular_plans);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_cellular_plans/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IT_inventory_cellular_plans iT_inventory_cellular_plans)
        {
            if (db.IT_inventory_cellular_plans.Any(x => x.id_it_inventory_items == iT_inventory_cellular_plans.id_it_inventory_items && iT_inventory_cellular_plans.id_it_inventory_items > 0 && x.id != iT_inventory_cellular_plans.id))
                ModelState.AddModelError("", "El equipo seleccionado ya se encuentra asignado a otro plan.");

            if (db.IT_inventory_cellular_plans.Any(x => x.num_telefono == iT_inventory_cellular_plans.num_telefono && !String.IsNullOrEmpty(iT_inventory_cellular_plans.num_telefono) && x.id != iT_inventory_cellular_plans.id))
                ModelState.AddModelError("", "El número de celular ya se encuentra asignado a otro plan.");

            if (ModelState.IsValid)
            {
                db.Entry(iT_inventory_cellular_plans).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            var type = GetHardware_TypeCell();
            ViewBag.id_it_inventory_items = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x => x.id_inventory_type == type.id && x.active == true), "id", "ConcatInfoSmartphone"), selected: iT_inventory_cellular_plans.id_it_inventory_items.ToString());
            return View(iT_inventory_cellular_plans);
        }

        // GET: PFA_Departmet/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_cellular_plans item = db.IT_inventory_cellular_plans.Find(id);
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
            IT_inventory_cellular_plans item = db.IT_inventory_cellular_plans.Find(id);
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
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_cellular_plans item = db.IT_inventory_cellular_plans.Find(id);
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
            IT_inventory_cellular_plans item = db.IT_inventory_cellular_plans.Find(id);
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

        [NonAction]
        protected IT_inventory_hardware_type GetHardware_TypeCell()
        {
            //busca el tipo inventario para smartphones
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("smartphone"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            return type;
        }
    }
}
