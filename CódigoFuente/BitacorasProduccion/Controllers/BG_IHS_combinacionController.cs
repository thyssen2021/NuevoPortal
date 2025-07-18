using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
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


        // El método principal ahora no necesita el parámetro 'demanda'
        public async Task<JsonResult> GetRows(List<IhsItemConPorcentaje> data, float? porcentaje)
        {
            // 1. Iniciar el cronómetro al principio del método
            var watch = Stopwatch.StartNew();

            // --- Tu lógica optimizada ---
            // (Esta es la versión que devuelve ambos resultados en un solo JSON)
            string originalHtml = await GenerarHtmlTablaAsync(data, porcentaje, Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL);
            string customerHtml = await GenerarHtmlTablaAsync(data, porcentaje, Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER);
            var jsonData = new { original = originalHtml, customer = customerHtml };

            // 2. Detener el cronómetro justo antes de terminar
            watch.Stop();

            // 3. Escribir el tiempo transcurrido en la ventana de "Resultados" de Visual Studio
            //    El formato "N2" es para mostrarlo con 2 decimales.
            Debug.WriteLine($"--- TIEMPO TOTAL GetRows: {watch.Elapsed.TotalMilliseconds:N2} ms ---");

            // 4. Devolver el resultado como siempre
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Método que retorna las filas de las tablas de combinacion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        // --- OPTIMIZACIÓN 1: El método ahora es asíncrono ---
        // Asegúrate de tener esta línea al inicio de tu archivo .cs para poder usar Stopwatch y Debug


        private async Task<string> GenerarHtmlTablaAsync(List<IhsItemConPorcentaje> data, float? porcentaje, string demanda)
        {
            // --- PASO 1: INICIALIZAR HERRAMIENTAS DE MEDICIÓN ---


            // --- MEDICIÓN 1: OBTENCIÓN DE CABECERAS ---
            var cabeceraDemanda = Portal_2_0.Models.BG_IHS_UTIL.GetCabecera();
            var cabeceraCuartos = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraCuartos();
            var cabeceraAnios = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAnios();
            var cabeceraAniosFY = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAniosFY();
            

            // --- LÓGICA ORIGINAL (Sin cambios) ---
            if (porcentaje == null)
                porcentaje = 0;
            float porcentaje_scrap = 1 + (porcentaje.Value / 100.0f);
            float[] totales = new float[cabeceraDemanda.Count + cabeceraCuartos.Count + cabeceraAnios.Count + cabeceraAniosFY.Count];
            var resultBuilder = new StringBuilder();

            if (data == null || !data.Any())
            {
                // Construimos el string con el mensaje.
                resultBuilder.Append("<tr><td colspan='" + (12 + totales.Length) + "' style='text-align:center;'>No hay elementos IHS seleccionados.</td></tr>");

                // Devolvemos directamente el string. Ahora el método siempre devuelve lo que promete.
                return resultBuilder.ToString();
            }

            // --- MEDICIÓN 2: CONSULTA PRINCIPAL A LA BASE DE DATOS ---
            var ids = data.Select(d => d.id).ToList();
            var ihsItems = await db.BG_IHS_item
            .Include(i => i.BG_IHS_rel_cuartos) // Carga los cuartos
            .Include(i => i.BG_IHS_versiones.BG_IHS_rel_regiones.Select(r => r.BG_IHS_regiones)) // Carga la región
            .Include(i => i.BG_IHS_versiones.BG_IHS_rel_regiones.Select(r => r.BG_IHS_plantas)) // Carga la planta para el filtro
            .Include(i => i.BG_IHS_rel_demanda)
            .Where(i => ids.Contains(i.id))
            .ToListAsync();
                      

            // --- LÓGICA ORIGINAL (Sin cambios) ---
            var dicPorcentajes = data.ToDictionary(d => d.id, d => d.porcentaje);

            // --- PREPARACIÓN PARA MEDICIÓN DENTRO DEL BUCLE ---
            // Se declaran variables para sumar los tiempos de cada método llamado dentro del bucle.
            TimeSpan tiempoAcumulado_GetDemanda = TimeSpan.Zero;
            TimeSpan tiempoAcumulado_GetCuartos = TimeSpan.Zero;
            TimeSpan tiempoAcumulado_GetAnios = TimeSpan.Zero;
            TimeSpan tiempoAcumulado_GetAniosFY = TimeSpan.Zero;
            TimeSpan tiempoAcumulado_StringBuilding = TimeSpan.Zero;

            // --- MEDICIÓN 3: BUCLE PRINCIPAL COMPLETO ---
            foreach (var ihs in ihsItems)
            {
                int x = 0;

                // Medición Acumulada: GetDemanda
                var demandaMeses = ihs.GetDemanda(cabeceraDemanda, demanda);

                // --- LÓGICA ORIGINAL (Sin cambios) ---
                decimal porcentajeItem;
                dicPorcentajes.TryGetValue(ihs.id, out porcentajeItem);
                float factorAjusteItem = (float)porcentajeItem / 100.0f;

                // Medición Acumulada: Construcción de la primera parte del HTML de la fila
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

                // --- LÓGICA ORIGINAL (Bucles internos) ---
                foreach (var item_demanda in demandaMeses)
                {
                    float cantidadOriginal = item_demanda?.cantidad ?? 0;
                    float cantidadAjustada = cantidadOriginal * factorAjusteItem * porcentaje_scrap;
                    resultBuilder.Append(@"<td class='" + (item_demanda != null ? (item_demanda.origen_datos == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER ? "fondo-customer" : "fondo-original") : "fondo-default") + @"'>")
                                 .Append(cantidadAjustada.ToString("N0"))
                                 .Append("</td>");
                    totales[x++] += cantidadAjustada;
                }

                // Medición Acumulada: GetCuartos
                var cuartos = ihs.GetCuartos(demandaMeses, cabeceraCuartos, demanda);
                foreach (var item_cuarto in cuartos)
                {
                    float cantidadOriginal = item_cuarto?.cantidad ?? 0;
                    float cantidadAjustada = cantidadOriginal * factorAjusteItem * porcentaje_scrap;
                    resultBuilder.Append(@"<td class='" + (item_cuarto != null ? (item_cuarto.origen_datos == Enum_BG_origen_cuartos.Calculado ? "fondo-cuarto-calculado" : "fondo-cuarto-ihs") : "fondo-default") + @"'>")
                                 .Append(cantidadAjustada.ToString("N0"))
                                 .Append("</td>");
                    totales[x++] += cantidadAjustada;
                }

                // Medición Acumulada: GetAnios
                var anios = ihs.GetAnios(demandaMeses, cabeceraAnios, demanda);
                foreach (var item_anio in anios)
                {
                    float cantidadOriginal = item_anio?.cantidad ?? 0;
                    float cantidadAjustada = cantidadOriginal * factorAjusteItem * porcentaje_scrap;
                    resultBuilder.Append(@"<td class='" + (item_anio != null ? (item_anio.origen_datos == Enum_BG_origen_anios.Calculado ? "fondo-cuarto-calculado" : "fondo-cuarto-ihs") : "fondo-default") + @"'>")
                                 .Append(cantidadAjustada.ToString("N0"))
                                 .Append("</td>");
                    totales[x++] += cantidadAjustada;
                }

                // Medición Acumulada: GetAniosFY
                var aniosFY = ihs.GetAniosFY(demandaMeses, cabeceraAniosFY, demanda);
                foreach (var item_anio in aniosFY)
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
           

            // --- LÓGICA ORIGINAL (Fila de totales) ---
            resultBuilder.Append("<tr style='background-color:dodgerblue; font-weight:bold; color:#FFFFFF'>")
                         .Append("<td colspan='12'>Totales</td>");

            foreach (float total in totales)
                resultBuilder.Append("<td>" + total.ToString("N0") + "</td>");

            resultBuilder.Append("</tr>");
                      

            // --- RETORNO DE DATOS (Sin cambios) ---
            return resultBuilder.ToString();
        }

    }

    public class IhsItemConPorcentaje
    {
        public int id { get; set; }
        public decimal porcentaje { get; set; }
    }
}
