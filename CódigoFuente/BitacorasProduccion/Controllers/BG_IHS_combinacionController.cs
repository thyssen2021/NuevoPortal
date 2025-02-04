using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using DocumentFormat.OpenXml.Bibliography;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BG_IHS_combinacionController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_IHS_combinacion
        public ActionResult Index(int? id, int? version, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var listadoBD = db.BG_IHS_combinacion.Where(x => x.id_ihs_version == version);

            var listado = listadoBD
                .Where(x => x.id == id || id == null)
                .OrderByDescending(x => x.id)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = listadoBD
                .Where(x => x.id == id || id == null)
                .Count();

            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id"] = id;
            routeValues["version"] = version;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.Paginacion = paginacion;
            ViewBag.id = AddFirstItem(new SelectList(listadoBD, nameof(BG_IHS_combinacion.id), nameof(BG_IHS_combinacion.vehicle)), textoPorDefecto: "-- Todos --");

            ViewBag.VersionIHS = db.BG_IHS_versiones.Find(version);
            return View(listado);
        }

        // GET: BG_IHS_combinacion/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_combinacion bG_IHS_combinacion = db.BG_IHS_combinacion.Find(id);
            if (bG_IHS_combinacion == null)
            {
                return HttpNotFound();
            }
            return View(bG_IHS_combinacion);
        }

        // GET: BG_IHS_combinacion/Create
        public ActionResult Create(int? version)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            List<BG_IHS_item> listadoIHSItems = db.BG_IHS_item.Where(x => x.id_ihs_version == version).ToList();
            ViewBag.listadoIHSItems = listadoIHSItems;

            var model = new BG_IHS_combinacion();
            model.id_ihs_version = version.HasValue ? version.Value : 0;
            model.porcentaje_scrap = (decimal?)0.03;
            ViewBag.VersionIHS = db.BG_IHS_versiones.Find(version);

            //envia el select list por viewbag
            List<string> listaManufacturer = db.BG_IHS_item.Where(x => x.id_ihs_version == version).Select(x => x.manufacturer).Distinct().ToList();

            //lista para manufacturer
            List<SelectListItem> newSelectListManufacturer = new List<SelectListItem>();
            foreach (var m in listaManufacturer)
            {
                newSelectListManufacturer.Add(new SelectListItem()
                {
                    Text = m,
                    Value = m
                });
            }

            //lista para planta
            List<SelectListItem> newSelectListPlanta = new List<SelectListItem>();
            //lista para brand
            List<SelectListItem> newSelectListBrand = new List<SelectListItem>();
            //lista nameplate
            List<SelectListItem> newSelectListNameplate = new List<SelectListItem>();


            ViewBag.manufacturer_group = AddFirstItem(new SelectList(newSelectListManufacturer, "Value", "Text"), textoPorDefecto: "-- Seleccione --");
            ViewBag.production_plant = AddFirstItem(new SelectList(newSelectListPlanta, "Value", "Text"), textoPorDefecto: "-- Seleccione --");
            ViewBag.production_brand = AddFirstItem(new SelectList(newSelectListBrand, "Value", "Text"), textoPorDefecto: "-- Seleccione --");
            ViewBag.production_nameplate = AddFirstItem(new SelectList(newSelectListNameplate, "Value", "Text"), textoPorDefecto: "-- Seleccione --");

            return View(model);
        }

        // POST: BG_IHS_combinacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BG_IHS_combinacion bG_IHS_combinacion)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (bG_IHS_combinacion.BG_IHS_rel_combinacion.Count == 0)
                ModelState.AddModelError("", "Debe agregar al menos un elemento de IHS.");

            if (db.BG_IHS_combinacion.Any(x => x.vehicle == bG_IHS_combinacion.vehicle && x.id_ihs_version == bG_IHS_combinacion.id_ihs_version))
                ModelState.AddModelError("", "Ya existe una combinación con la misma clave de vehículo.");

            var codigosIHS = db.BG_IHS_item
                .Select(x => new
                {
                    x.mnemonic_vehicle_plant,
                    x.vehicle,
                    x.production_plant,
                    x.sop_start_of_production
                })
                .AsEnumerable() // 🔹 Convierte la consulta a memoria para usar ToString()
                .Select(x =>
                    $"{x.mnemonic_vehicle_plant}_{x.vehicle}{{{x.production_plant}}}" +
                    (x.sop_start_of_production.HasValue ? x.sop_start_of_production.Value.ToString("yyyy-MM") : ""))
                .ToHashSet(); // 🔹 Usamos HashSet para mejorar búsquedas

            if (codigosIHS.Contains(bG_IHS_combinacion.vehicle))
            {
                ModelState.AddModelError("", "Ya existe un registro de IHS con la misma clave de vehículo.");
            }           

            //verificar si no hay ids repetidos
            var query = bG_IHS_combinacion.BG_IHS_rel_combinacion.GroupBy(x => x.id_ihs_item)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            if (query.Count > 0)
                ModelState.AddModelError("", "Existen elementos repetidos en la combinación de IHS, favor de verificarlo.");

            if (ModelState.IsValid)
            {
                bG_IHS_combinacion.activo = true;
                db.BG_IHS_combinacion.Add(bG_IHS_combinacion);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se han guardado los cambios correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index", new { version = bG_IHS_combinacion.id_ihs_version });
            }

            //envia el select list por viewbag
            List<string> listaManufacturer = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version).Select(x => x.manufacturer).Distinct().ToList();

            //lista para manufacturer
            List<SelectListItem> newSelectListManufacturer = new List<SelectListItem>();
            foreach (var m in listaManufacturer)
            {
                newSelectListManufacturer.Add(new SelectListItem()
                {
                    Text = m,
                    Value = m
                });
            }

            //lista para planta
            List<SelectListItem> newSelectListPlanta = new List<SelectListItem>();
            List<string> listaPlanta = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version && x.manufacturer == bG_IHS_combinacion.manufacturer_group).Select(x => x.production_plant).Distinct().ToList();
            foreach (var m in listaPlanta)
            {
                newSelectListPlanta.Add(new SelectListItem()
                {
                    Text = m,
                    Value = m
                });
            }

            //lista para brand
            List<SelectListItem> newSelectListBrand = new List<SelectListItem>();

            List<string> listaBrand = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version && x.manufacturer == bG_IHS_combinacion.manufacturer_group && x.production_plant == bG_IHS_combinacion.production_plant).Select(x => x.production_brand).Distinct().ToList();
            foreach (var m in listaBrand)
            {
                newSelectListBrand.Add(new SelectListItem()
                {
                    Text = m,
                    Value = m
                });
            }

            //lista para nameplate
            List<SelectListItem> newSelectListNameplate = new List<SelectListItem>();

            List<string> listaNameplate = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version && x.manufacturer == bG_IHS_combinacion.manufacturer_group && x.production_plant == bG_IHS_combinacion.production_plant).Select(x => x.production_nameplate).Distinct().ToList();
            foreach (var m in listaNameplate)
            {
                newSelectListNameplate.Add(new SelectListItem()
                {
                    Text = m,
                    Value = m
                });
            }


            ViewBag.manufacturer_group = AddFirstItem(new SelectList(newSelectListManufacturer, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: bG_IHS_combinacion.manufacturer_group);
            ViewBag.production_plant = AddFirstItem(new SelectList(newSelectListPlanta, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: bG_IHS_combinacion.production_plant);
            ViewBag.production_brand = AddFirstItem(new SelectList(newSelectListBrand, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: bG_IHS_combinacion.production_brand);
            ViewBag.production_nameplate = AddFirstItem(new SelectList(newSelectListNameplate, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: bG_IHS_combinacion.production_nameplate);


            List<BG_IHS_item> listadoIHSItems = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version).ToList();
            ViewBag.listadoIHSItems = listadoIHSItems;
            ViewBag.VersionIHS = db.BG_IHS_versiones.Find(bG_IHS_combinacion.id_ihs_version);

            return View(bG_IHS_combinacion);
        }

        // GET: BG_IHS_combinacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_combinacion bG_IHS_combinacion = db.BG_IHS_combinacion.Find(id);
            if (bG_IHS_combinacion == null)
            {
                return HttpNotFound();
            }


            List<BG_IHS_item> listadoIHSItems = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version).ToList();
            ViewBag.listadoIHSItems = listadoIHSItems;
            ViewBag.VersionIHS = db.BG_IHS_versiones.Find(bG_IHS_combinacion.id_ihs_version);

            //envia el select list por viewbag
            List<string> listaManufacturer = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version).Select(x => x.manufacturer).Distinct().ToList();

            //lista para manufacturer
            List<SelectListItem> newSelectListManufacturer = new List<SelectListItem>();
            foreach (var m in listaManufacturer)
            {
                newSelectListManufacturer.Add(new SelectListItem()
                {
                    Text = m,
                    Value = m
                });
            }

            //lista para planta
            List<SelectListItem> newSelectListPlanta = new List<SelectListItem>();
            List<string> listaPlanta = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version && x.manufacturer == bG_IHS_combinacion.manufacturer_group).Select(x => x.production_plant).Distinct().ToList();
            foreach (var m in listaPlanta)
            {
                newSelectListPlanta.Add(new SelectListItem()
                {
                    Text = m,
                    Value = m
                });
            }

            //lista para brand
            List<SelectListItem> newSelectListBrand = new List<SelectListItem>();

            List<string> listaBrand = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version && x.manufacturer == bG_IHS_combinacion.manufacturer_group && x.production_plant == bG_IHS_combinacion.production_plant).Select(x => x.production_brand).Distinct().ToList();
            foreach (var m in listaBrand)
            {
                newSelectListBrand.Add(new SelectListItem()
                {
                    Text = m,
                    Value = m
                });
            }

            //lista para nameplate
            List<SelectListItem> newSelectListNameplate = new List<SelectListItem>();

            List<string> listaNameplate = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version && x.manufacturer == bG_IHS_combinacion.manufacturer_group && x.production_plant == bG_IHS_combinacion.production_plant).Select(x => x.production_nameplate).Distinct().ToList();
            foreach (var m in listaNameplate)
            {
                newSelectListNameplate.Add(new SelectListItem()
                {
                    Text = m,
                    Value = m
                });
            }


            ViewBag.manufacturer_group = AddFirstItem(new SelectList(newSelectListManufacturer, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: bG_IHS_combinacion.manufacturer_group);
            ViewBag.production_plant = AddFirstItem(new SelectList(newSelectListPlanta, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: bG_IHS_combinacion.production_plant);
            ViewBag.production_brand = AddFirstItem(new SelectList(newSelectListBrand, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: bG_IHS_combinacion.production_brand);
            ViewBag.production_nameplate = AddFirstItem(new SelectList(newSelectListNameplate, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: bG_IHS_combinacion.production_nameplate);


            return View(bG_IHS_combinacion);
        }

        // POST: BG_IHS_combinacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BG_IHS_combinacion bG_IHS_combinacion)
        {
            if (bG_IHS_combinacion.BG_IHS_rel_combinacion.Count == 0)
                ModelState.AddModelError("", "Debe agregar al menos un elemento de IHS.");

            if (db.BG_IHS_combinacion.Any(x => x.vehicle == bG_IHS_combinacion.vehicle && x.id != bG_IHS_combinacion.id && x.id_ihs_version == bG_IHS_combinacion.id_ihs_version))
                ModelState.AddModelError("", "Ya existe una combinación con la misma clave de vehículo.");

            //compara vehiculo con concatCodigo
            var codigosIHS = db.BG_IHS_item
               .Select(x => new
               {
                   x.mnemonic_vehicle_plant,
                   x.vehicle,
                   x.production_plant,
                   x.sop_start_of_production
               })
               .AsEnumerable() // 🔹 Convierte la consulta a memoria para usar ToString()
               .Select(x =>
                   $"{x.mnemonic_vehicle_plant}_{x.vehicle}{{{x.production_plant}}}" +
                   (x.sop_start_of_production.HasValue ? x.sop_start_of_production.Value.ToString("yyyy-MM") : ""))
               .ToHashSet(); // 🔹 Usamos HashSet para mejorar búsquedas

            if (codigosIHS.Contains(bG_IHS_combinacion.vehicle))
            {
                ModelState.AddModelError("", "Ya existe un registro de IHS con la misma clave de vehículo.");
            }


            if (db.BG_IHS_rel_division.Any(x => x.vehicle == bG_IHS_combinacion.vehicle))
                ModelState.AddModelError("", "Ya existe una división con la misma clave de vehículo.");

            //verificar si no hay ids repetidos
            var query = bG_IHS_combinacion.BG_IHS_rel_combinacion.GroupBy(x => x.id_ihs_item)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            if (query.Count > 0)
                ModelState.AddModelError("", "Existen elementos repetidos en la combinación de IHS, favor de verificarlo.");

            if (ModelState.IsValid)
            {
                //borra los conceptos anteriores
                var list = db.BG_IHS_rel_combinacion.Where(x => x.id_ihs_combinacion == bG_IHS_combinacion.id);
                foreach (BG_IHS_rel_combinacion item in list)
                    db.BG_IHS_rel_combinacion.Remove(item);
                //                db.SaveChanges();

                //los nuevos conceptos se agregarán automáticamente
                //si existe lo modifica
                BG_IHS_combinacion combinacion = db.BG_IHS_combinacion.Find(bG_IHS_combinacion.id);
                // Activity already exist in database and modify it
                db.Entry(combinacion).CurrentValues.SetValues(bG_IHS_combinacion);
                //agrega los conceptos 
                combinacion.BG_IHS_rel_combinacion = bG_IHS_combinacion.BG_IHS_rel_combinacion;

                db.Entry(combinacion).State = EntityState.Modified;
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                db.SaveChanges();
                return RedirectToAction("Index", new { version = bG_IHS_combinacion.id_ihs_version });
            }
            //en caso de error en el modelo
            List<BG_IHS_item> listadoIHSItems = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version).ToList();
            ViewBag.listadoIHSItems = listadoIHSItems;
            ViewBag.VersionIHS = db.BG_IHS_versiones.Find(bG_IHS_combinacion.id_ihs_version);

            //envia el select list por viewbag
            List<string> listaManufacturer = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version).Select(x => x.manufacturer).Distinct().ToList();

            //lista para manufacturer
            List<SelectListItem> newSelectListManufacturer = new List<SelectListItem>();
            foreach (var m in listaManufacturer)
            {
                newSelectListManufacturer.Add(new SelectListItem()
                {
                    Text = m,
                    Value = m
                });
            }

            //lista para planta
            List<SelectListItem> newSelectListPlanta = new List<SelectListItem>();
            List<string> listaPlanta = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version && x.manufacturer == bG_IHS_combinacion.manufacturer_group).Select(x => x.production_plant).Distinct().ToList();
            foreach (var m in listaPlanta)
            {
                newSelectListPlanta.Add(new SelectListItem()
                {
                    Text = m,
                    Value = m
                });
            }

            //lista para brand
            List<SelectListItem> newSelectListBrand = new List<SelectListItem>();

            List<string> listaBrand = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version && x.manufacturer == bG_IHS_combinacion.manufacturer_group && x.production_plant == bG_IHS_combinacion.production_plant).Select(x => x.production_brand).Distinct().ToList();
            foreach (var m in listaBrand)
            {
                newSelectListBrand.Add(new SelectListItem()
                {
                    Text = m,
                    Value = m
                });
            }

            //lista para nameplate
            List<SelectListItem> newSelectListNameplate = new List<SelectListItem>();

            List<string> listaNameplate = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_combinacion.id_ihs_version && x.manufacturer == bG_IHS_combinacion.manufacturer_group && x.production_plant == bG_IHS_combinacion.production_plant).Select(x => x.production_nameplate).Distinct().ToList();
            foreach (var m in listaNameplate)
            {
                newSelectListNameplate.Add(new SelectListItem()
                {
                    Text = m,
                    Value = m
                });
            }


            ViewBag.manufacturer_group = AddFirstItem(new SelectList(newSelectListManufacturer, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: bG_IHS_combinacion.manufacturer_group);
            ViewBag.production_plant = AddFirstItem(new SelectList(newSelectListPlanta, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: bG_IHS_combinacion.production_plant);
            ViewBag.production_brand = AddFirstItem(new SelectList(newSelectListBrand, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: bG_IHS_combinacion.production_brand);
            ViewBag.production_nameplate = AddFirstItem(new SelectList(newSelectListNameplate, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: bG_IHS_combinacion.production_nameplate);


            return View(bG_IHS_combinacion);
        }

        // GET: GV_tipo_gastos_viaje/Disable/5
        public ActionResult Disable(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_combinacion bG_IHS_combinacion = db.BG_IHS_combinacion.Find(id);
            if (bG_IHS_combinacion == null)
            {
                return HttpNotFound();
            }
            return View(bG_IHS_combinacion);
        }

        // POST: GV_tipo_gastos_viaje/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            BG_IHS_combinacion item = db.BG_IHS_combinacion.Find(id);
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
                return RedirectToAction("Index", new { version = item.id_ihs_version });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Index", new { version = item.id_ihs_version });
            }
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.DISABLED, TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("Index", new { version = item.id_ihs_version });
        }

        // GET: GV_tipo_gastos_viaje/Enable/5
        public ActionResult Enable(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_combinacion bG_IHS_combinacion = db.BG_IHS_combinacion.Find(id);
            if (bG_IHS_combinacion == null)
            {
                return HttpNotFound();
            }
            return View(bG_IHS_combinacion);
        }

        // POST: GV_tipo_gastos_viaje/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            BG_IHS_combinacion item = db.BG_IHS_combinacion.Find(id);
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
                return RedirectToAction("Index", new { version = item.id_ihs_version });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Index", new { version = item.id_ihs_version });
            }
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.ENABLED, TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("Index", new { version = item.id_ihs_version });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Método que retorna las filas de las tablas de combinacion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult GetRows(int[] data, string demanda, float? porcentaje)
        {
            var cabeceraDemanda = Portal_2_0.Models.BG_IHS_UTIL.GetCabecera();
            var cabeceraCuartos = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraCuartos();
            var cabeceraAnios = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAnios();
            var cabeceraAniosFY = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAniosFY();

            if (porcentaje == null)
                porcentaje = 0;

            float porcentaje_scrap = 1 + (porcentaje.Value / 100.0f);

            float[] totales = new float[cabeceraDemanda.Count + cabeceraCuartos.Count + cabeceraAnios.Count + cabeceraAniosFY.Count];
            int x = 0;

            //  List<int> list = data.ToList().Distinct().ToList();
            List<int> list = data.ToList();

            string resultString = string.Empty;

            foreach (var item in list)
            {
                x = 0;
                var ihs = db.BG_IHS_item.Find(item);

                var demandaMeses = ihs.GetDemanda(cabeceraDemanda, demanda);

                if (ihs != null)
                    resultString += @"<tr>
                                        <td>" + (list.IndexOf(item) + 1) + @"</td>
                                        <td>" + ihs.origen + @"</td>
                                        <td>" + ihs.manufacturer_group + @"</td>
                                        <td>" + ihs.production_plant + @"</td>
                                        <td>" + ihs.production_brand + @"</td>
                                        <td>" + ihs.program + @"</td>
                                        <td>" + ihs.production_nameplate + @"</td>
                                        <td>" + ihs.vehicle + @"</td>
                                        <td>" + (ihs.sop_start_of_production.HasValue ? ihs.sop_start_of_production.Value.ToShortDateString() : String.Empty) + @"</td>                                        
                                        <td>" + (ihs.eop_end_of_production.HasValue ? ihs.eop_end_of_production.Value.ToShortDateString() : String.Empty) + @"</td>
                                        <td>" + porcentaje + @"%</td>";

                //obtiene la demanda por meses 
                foreach (var item_demanda in demandaMeses)
                {

                    resultString += @"<td class=" + (
                                    item_demanda != null ? item_demanda.origen_datos == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER ? "fondo-customer"
                                    : item_demanda.origen_datos == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL ? "fondo-original"
                                    : "fondo-default"
                                    : "fondo-default"
                                    ) + @">" +
                                      (item_demanda != null ? (item_demanda.cantidad * porcentaje_scrap).ToString() : String.Empty)
                                    + "</td>";

                    totales[x++] += item_demanda != null ? item_demanda.cantidad.HasValue ? (item_demanda.cantidad.Value * porcentaje_scrap) : 0 : 0;
                }
                //obtiene la demanda por cuartos 
                foreach (var item_cuarto in ihs.GetCuartos(demandaMeses, cabeceraCuartos, demanda))
                {

                    resultString += @"<td class=" + (
                                    item_cuarto != null ? item_cuarto.origen_datos == Enum_BG_origen_cuartos.Calculado ? "fondo-cuarto-calculado"
                                    : item_cuarto.origen_datos == Enum_BG_origen_cuartos.IHS ? "fondo-cuarto-ihs"
                                    : "fondo-default"
                                    : "fondo-default"
                                    ) + @">" +
                                      (item_cuarto != null ? (item_cuarto.cantidad * porcentaje_scrap).ToString() : String.Empty)
                                    + "</td>";

                    totales[x++] += item_cuarto != null ? item_cuarto.cantidad.HasValue ? (item_cuarto.cantidad.Value * porcentaje_scrap) : 0 : 0;
                }
                //obtiene la demanda por años Ene-Dic 
                foreach (var item_anio in ihs.GetAnios(demandaMeses, cabeceraAnios, demanda))
                {

                    resultString += @"<td class=" + (
                                    item_anio != null ? item_anio.origen_datos == Enum_BG_origen_anios.Calculado ? "fondo-cuarto-calculado"
                                    : item_anio.origen_datos == Enum_BG_origen_anios.IHS ? "fondo-cuarto-ihs"
                                    : "fondo-default"
                                    : "fondo-default"
                                    ) + @">" +
                                      (item_anio != null ? (item_anio.cantidad * porcentaje_scrap).ToString() : String.Empty)
                                    + "</td>";

                    totales[x++] += item_anio != null ? item_anio.cantidad.HasValue ? (item_anio.cantidad.Value * porcentaje_scrap) : 0 : 0;
                }
                //obtiene la demanda por años FY 
                foreach (var item_anio in ihs.GetAniosFY(demandaMeses, cabeceraAniosFY, demanda))
                {

                    resultString += @"<td class=" + (
                                    item_anio != null ? item_anio.origen_datos == Enum_BG_origen_anios.Calculado ? "fondo-cuarto-calculado"
                                    : item_anio.origen_datos == Enum_BG_origen_anios.IHS ? "fondo-cuarto-ihs"
                                    : "fondo-default"
                                    : "fondo-default"
                                    ) + @">" +
                                         (item_anio != null ? (item_anio.cantidad * porcentaje_scrap).ToString() : String.Empty)
                                    + "</td>";

                    totales[x++] += item_anio != null ? item_anio.cantidad.HasValue ? (item_anio.cantidad.Value * porcentaje_scrap) : 0 : 0;
                }

                resultString += "</tr>";


            }

            //agrega la sumatoria de los datos obtenidos
            resultString += "<tr  style=\"background-color:dodgerblue; font-weight:bold; color:#FFFFFF\">" +
                "<td colspan=" + 11 + ">Totales</td>";

            foreach (float total in totales)
                resultString += "<td>" + total + "</td>";

            resultString += "</tr>";

            //inicializa la lista de objetos
            var result = new object[1];

            result[0] = new { value = resultString };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
