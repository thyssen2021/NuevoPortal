using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

       

        // --- AÑADIR ESTE NUEVO MÉTODO ---
        // Este será el ÚNICO método GET para mostrar el formulario.
        public async Task<ActionResult> Upsert(int? id, int? version, string modo = "Create")
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            // Prepara el modelo
            var model = new BG_IHS_combinacion();

            if (id.HasValue) // Si hay un ID, estamos en modo Edit o Details
            {
                model = await db.BG_IHS_combinacion
                                .Include(c => c.BG_IHS_rel_combinacion)
                                .FirstOrDefaultAsync(c => c.id == id);
                if (model == null) return HttpNotFound();

                // El modo ("Edit" o "Details") se pasará en la URL
                ViewBag.ModoVista = modo;
            }
            else // Si no hay ID, estamos en modo Create
            {
                if (!version.HasValue)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Se requiere una versión de IHS para crear.");

                model.id_ihs_version = version.Value;
                model.porcentaje_scrap = 0.03m;
                model.BG_IHS_rel_combinacion = new List<BG_IHS_rel_combinacion>();
                ViewBag.ModoVista = "Create";
            }

            // Llama al método ayudante para cargar los dropdowns
            await CargarDatosParaVista(model);

            // Siempre renderiza la misma vista "Upsert.cshtml"
            return View("Upsert", model);
        }


        // --- MODIFICAR ESTOS MÉTODOS ---

        // GET: BG_IHS_combinacion/Details/5
        public ActionResult Details(int? id)
        {
            // Redirige a la nueva acción Upsert en modo "Details"
            return RedirectToAction("Upsert", new { id = id, modo = "Details" });
        }

        // GET: BG_IHS_combinacion/Create
        public ActionResult Create(int? version)
        {
            // Redirige a la nueva acción Upsert en modo "Create"
            return RedirectToAction("Upsert", new { version = version });
        }

        // GET: BG_IHS_combinacion/Edit/5
        public ActionResult Edit(int? id)
        {
            // Redirige a la nueva acción Upsert en modo "Edit"
            return RedirectToAction("Upsert", new { id = id, modo = "Edit" });
        }


        // POST: BG_IHS_combinacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BG_IHS_combinacion bG_IHS_combinacion)
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

            ViewBag.ModoVista = "Create"; // No olvides re-establecer el modo
            await CargarDatosParaVista(bG_IHS_combinacion);
            return View("Upsert", bG_IHS_combinacion);
        }


        // POST: BG_IHS_combinacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BG_IHS_combinacion bG_IHS_combinacion)
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


            ViewBag.ModoVista = "Edit"; // No olvides re-establecer el modo
            await CargarDatosParaVista(bG_IHS_combinacion);
            return View("Upsert", bG_IHS_combinacion);
        }

        // --- AÑADIR ESTE NUEVO MÉTODO PRIVADO ---
        private async Task CargarDatosParaVista(BG_IHS_combinacion model)
        {
            // Obtiene la versión del IHS
            var version = await db.BG_IHS_versiones.FindAsync(model.id_ihs_version);
            ViewBag.VersionIHS = version;

            // Carga la lista completa de items para los dropdowns de las filas
            var listadoIHSItems = await db.BG_IHS_item
                .Where(x => x.id_ihs_version == model.id_ihs_version)
                .ToListAsync();
            ViewBag.listadoIHSItems = listadoIHSItems;

            // Carga las listas para los dropdowns del encabezado
            // --- INICIO DE LA CORRECCIÓN ---

            // Se obtienen las listas de la misma forma
            var listaManufacturer = listadoIHSItems.Select(x => x.manufacturer).Distinct().ToList();
            var listaPlanta = string.IsNullOrEmpty(model.manufacturer_group)
                ? new List<string>()
                : listadoIHSItems.Where(x => x.manufacturer == model.manufacturer_group)
                                .Select(x => x.production_plant).Distinct().ToList();
            var listaBrand = string.IsNullOrEmpty(model.production_plant)
                ? new List<string>()
                : listadoIHSItems.Where(x => x.manufacturer == model.manufacturer_group && x.production_plant == model.production_plant)
                                .Select(x => x.production_brand).Distinct().ToList();
            var listaNameplate = string.IsNullOrEmpty(model.production_plant)
                ? new List<string>()
                : listadoIHSItems.Where(x => x.manufacturer == model.manufacturer_group && x.production_plant == model.production_plant)
                                .Select(x => x.production_nameplate).Distinct().ToList();

            // Se crean los SelectList pasando el valor seleccionado del MODELO.
            // Y se guardan en el ViewBag con nombres DIFERENTES.
            ViewBag.ManufacturerList = new SelectList(listaManufacturer, model.manufacturer_group);
            ViewBag.ProductionPlantList = new SelectList(listaPlanta, model.production_plant);
            ViewBag.ProductionBrandList = new SelectList(listaBrand, model.production_brand);
            ViewBag.ProductionNameplateList = new SelectList(listaNameplate, model.production_nameplate);

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
        // --- OPTIMIZACIÓN 1: El método ahora es asíncrono ---
        public async Task<JsonResult> GetRows(List<IhsItemConPorcentaje> data, string demanda, float? porcentaje)
        {
            // OBTENCIÓN DE CABECERAS (Sin cambios)
            var cabeceraDemanda = Portal_2_0.Models.BG_IHS_UTIL.GetCabecera();
            var cabeceraCuartos = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraCuartos();
            var cabeceraAnios = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAnios();
            var cabeceraAniosFY = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAniosFY();

            // CÁLCULO DEL SCRAP GLOBAL (Sin cambios)
            if (porcentaje == null)
                porcentaje = 0;
            float porcentaje_scrap = 1 + (porcentaje.Value / 100.0f);

            // INICIALIZACIÓN DE VARIABLES
            float[] totales = new float[cabeceraDemanda.Count + cabeceraCuartos.Count + cabeceraAnios.Count + cabeceraAniosFY.Count];

            // --- OPTIMIZACIÓN 2: Se reemplaza 'string' por 'StringBuilder' ---
            var resultBuilder = new StringBuilder();

            // VALIDACIÓN DE DATOS DE ENTRADA (Modificado para usar StringBuilder)
            if (data == null || !data.Any())
            {
                resultBuilder.Append("<tr><td colspan='" + (12 + totales.Length) + "' style='text-align:center;'>No hay elementos IHS seleccionados.</td></tr>");
                var resultVacio = new object[1];
                resultVacio[0] = new { value = resultBuilder.ToString() };
                return Json(resultVacio, JsonRequestBehavior.AllowGet);
            }

            // CONSULTA A BASE DE DATOS
            var ids = data.Select(d => d.id).ToList();

            // --- OPTIMIZACIÓN 3: La consulta a la BD ahora es asíncrona ---
            var ihsItems = await db.BG_IHS_item.Where(i => ids.Contains(i.id)).ToListAsync();

            var dicPorcentajes = data.ToDictionary(d => d.id, d => d.porcentaje);

            // BUCLE PRINCIPAL PARA CONSTRUIR LAS FILAS
            foreach (var ihs in ihsItems)
            {
                int x = 0;
                var demandaMeses = ihs.GetDemanda(cabeceraDemanda, demanda);   
                // Si no se encuentra el ID en el diccionario (o si el valor enviado no es válido),
                // 'porcentajeItem' tomará automáticamente el valor por defecto de un decimal, que es 0.0m.
                decimal porcentajeItem;
                dicPorcentajes.TryGetValue(ihs.id, out porcentajeItem);
                float factorAjusteItem = (float)porcentajeItem / 100.0f;

                // --- OPTIMIZACIÓN 4: Se usa 'resultBuilder.Append' en lugar de '+=' ---
                resultBuilder.Append(@"<tr>
                                <td>" + (ihsItems.IndexOf(ihs) + 1) + @"</td>
                                <td>" + ihs.origen + @"</td>
                                <td>" + ihs.manufacturer_group + @"</td>
                                <td>" + ihs.production_plant + @"</td>
                                <td>" + ihs.production_brand + @"</td>
                                <td>" + ihs.program + @"</td>
                                <td>" + ihs.production_nameplate + @"</td>
                                <td>" + ihs.vehicle + @"</td>
                                <td>" + (ihs.sop_start_of_production.HasValue ? ihs.sop_start_of_production.Value.ToShortDateString() : String.Empty) + @"</td>
                                <td>" + (ihs.eop_end_of_production.HasValue ? ihs.eop_end_of_production.Value.ToShortDateString() : String.Empty) + @"</td>
                                <td>" + porcentaje + @"%</td>");

                resultBuilder.Append("<td>" + porcentajeItem.ToString("N2") + "%</td>");


                // Bucle de Meses
                foreach (var item_demanda in demandaMeses)
                {
                    float cantidadOriginal = item_demanda?.cantidad ?? 0;
                    float cantidadAjustada = cantidadOriginal * factorAjusteItem * porcentaje_scrap;
                    resultBuilder.Append(@"<td class='" + (item_demanda != null ? (item_demanda.origen_datos == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER ? "fondo-customer" : "fondo-original") : "fondo-default") + @"'>")
                                 .Append(cantidadAjustada.ToString("N0"))
                                 .Append("</td>");
                    totales[x++] += cantidadAjustada;
                }

                // Bucle de Cuartos
                foreach (var item_cuarto in ihs.GetCuartos(demandaMeses, cabeceraCuartos, demanda))
                {
                    float cantidadOriginal = item_cuarto?.cantidad ?? 0;
                    float cantidadAjustada = cantidadOriginal * factorAjusteItem * porcentaje_scrap;
                    resultBuilder.Append(@"<td class='" + (item_cuarto != null ? (item_cuarto.origen_datos == Enum_BG_origen_cuartos.Calculado ? "fondo-cuarto-calculado" : "fondo-cuarto-ihs") : "fondo-default") + @"'>")
                                 .Append(cantidadAjustada.ToString("N0"))
                                 .Append("</td>");
                    totales[x++] += cantidadAjustada;
                }

                // Bucle de Años
                foreach (var item_anio in ihs.GetAnios(demandaMeses, cabeceraAnios, demanda))
                {
                    float cantidadOriginal = item_anio?.cantidad ?? 0;
                    float cantidadAjustada = cantidadOriginal * factorAjusteItem * porcentaje_scrap;
                    resultBuilder.Append(@"<td class='" + (item_anio != null ? (item_anio.origen_datos == Enum_BG_origen_anios.Calculado ? "fondo-cuarto-calculado" : "fondo-cuarto-ihs") : "fondo-default") + @"'>")
                                 .Append(cantidadAjustada.ToString("N0"))
                                 .Append("</td>");
                    totales[x++] += cantidadAjustada;
                }

                // Bucle de Años Fiscales
                foreach (var item_anio in ihs.GetAniosFY(demandaMeses, cabeceraAniosFY, demanda))
                {
                    float cantidadOriginal = item_anio?.cantidad ?? 0;
                    float cantidadAjustada = cantidadOriginal * factorAjusteItem * porcentaje_scrap;
                    resultBuilder.Append(@"<td class='" + (item_anio != null ? (item_anio.origen_datos == Enum_BG_origen_anios.Calculado ? "fondo-cuarto-calculado" : "fondo-cuarto-ihs") : "fondo-default") + @"'>")
                                 .Append(cantidadAjustada.ToString("N0"))
                                 .Append("</td>");
                    totales[x++] += cantidadAjustada;
                }

                resultBuilder.Append("</tr>");
            }

            // CONSTRUCCIÓN DE LA FILA DE TOTALES
            resultBuilder.Append("<tr style='background-color:dodgerblue; font-weight:bold; color:#FFFFFF'>")
                       .Append("<td colspan='12'>Totales</td>");

            foreach (float total in totales)
                resultBuilder.Append("<td>" + total.ToString("N0") + "</td>");

            resultBuilder.Append("</tr>");

            // RETORNO DE DATOS
            var result = new object[1];
            result[0] = new { value = resultBuilder.ToString() }; // Se convierte el StringBuilder a string al final
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }

    public class IhsItemConPorcentaje
    {
        public int id { get; set; }
        public decimal porcentaje { get; set; }
    }
}
