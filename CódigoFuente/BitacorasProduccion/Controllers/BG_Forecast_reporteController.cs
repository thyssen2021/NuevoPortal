using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Clases.Util;
using ExcelDataReader;
using Microsoft.AspNet.SignalR;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [System.Web.Mvc.Authorize]
    public class BG_Forecast_reporteController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        private List<BG_Forecast_cat_inventory_own> listadoInventoryOwnDefault = new List<BG_Forecast_cat_inventory_own>() {
            new BG_Forecast_cat_inventory_own{ orden = 1, mes = 10, cantidad = 3.80},
            new BG_Forecast_cat_inventory_own{ orden = 2, mes = 11, cantidad = 3.80},
            new BG_Forecast_cat_inventory_own{ orden = 3, mes = 12, cantidad = 2.40},
            new BG_Forecast_cat_inventory_own{ orden = 4, mes = 1, cantidad = 3.20},
            new BG_Forecast_cat_inventory_own{ orden = 5, mes = 2, cantidad = 3.80},
            new BG_Forecast_cat_inventory_own{ orden = 6, mes = 3, cantidad = 3.80},
            new BG_Forecast_cat_inventory_own{ orden = 7, mes = 4, cantidad = 2.80},
            new BG_Forecast_cat_inventory_own{ orden = 8, mes = 5, cantidad = 3.80},
            new BG_Forecast_cat_inventory_own{ orden = 9, mes = 6, cantidad = 3.80},
            new BG_Forecast_cat_inventory_own{ orden = 10, mes = 7, cantidad = 3.60},
            new BG_Forecast_cat_inventory_own{ orden = 11, mes = 8, cantidad = 4.10},
            new BG_Forecast_cat_inventory_own{ orden = 12, mes = 9, cantidad = 3.70},
        };

        // GET: reporte
        public ActionResult Index(int? id, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro

            //verifica que el elemento este relacionado con el elmento anterior

            var listado = db.BG_Forecast_reporte
               .Where(x =>
                        (x.id == id || id == null)
                      )
               .OrderByDescending(x => x.id)
               .Skip((pagina - 1) * cantidadRegistrosPorPagina)
              .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.BG_Forecast_reporte
               .Where(x =>
                        (x.id == id || id == null)
                      )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id"] = id;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };
            ViewBag.Paginacion = paginacion;

            ViewBag.id = AddFirstItem(new SelectList(db.BG_Forecast_reporte.Where(p => p.activo == true), nameof(BG_Forecast_reporte.id), nameof(BG_Forecast_reporte.ConcatCodigo)), textoPorDefecto: "-- Todos --");

            return View(listado);
        }

        // GET: reporte/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte bG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (bG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }
            return View(bG_Forecast_reporte);
        }

        // GET: reporte/ClonarReporte/5
        public ActionResult ClonarReporte(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte bG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (bG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }

            var model = new ClonarReporteViewModel
            {
                id_ihs_version = bG_Forecast_reporte.id_ihs_version,
                id_reporte = bG_Forecast_reporte.id,
                descripcion = bG_Forecast_reporte.descripcion,
                fecha = DateTime.Now,
                reporte = bG_Forecast_reporte,
                activo = bG_Forecast_reporte.activo
            };

            ViewBag.id_reporte = AddFirstItem(new SelectList(db.BG_Forecast_reporte.Where(x => x.activo == true).OrderByDescending(x => x.id), nameof(BG_Forecast_reporte.id), nameof(BG_Forecast_reporte.ConcatCodigo)), selected: model.id_reporte.ToString());

            return View(model);
        }

        // POST: reporte/ClonarReporte/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClonarReporte(ClonarReporteViewModel model)
        {
            if (db.BG_Forecast_reporte.Any(x => x.fecha == model.fecha && x.descripcion == model.descripcion))
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores.");

            var reporteBD = db.BG_Forecast_reporte.Find(model.id_reporte);

            if (ModelState.IsValid)
            {
                //comienza a contruir el objeto que se guardará en BD
                var reporte = new BG_Forecast_reporte
                {
                    id_ihs_version = model.id_ihs_version,
                    fecha = model.fecha,
                    descripcion = model.descripcion,
                    activo = model.activo
                };

                //copia los elementos a un nuevo reporte
                foreach (var reporteItem in reporteBD.BG_Forecast_item)
                    reporte.BG_Forecast_item.Add(new BG_Forecast_item
                    {
                        id_ihs_item = reporteItem.id_ihs_item,
                        id_ihs_combinacion = reporteItem.id_ihs_combinacion,
                        id_ihs_rel_division = reporteItem.id_ihs_rel_division,
                        pos = reporteItem.pos,
                        business_and_plant = reporteItem.business_and_plant,
                        calculo_activo = reporteItem.calculo_activo,
                        sap_invoice_code = reporteItem.sap_invoice_code,
                        invoiced_to = reporteItem.invoiced_to,
                        number_sap_client = reporteItem.number_sap_client,
                        shipped_to = reporteItem.shipped_to,
                        own_cm = reporteItem.own_cm,
                        route = reporteItem.route,
                        plant = reporteItem.plant,
                        external_processor = reporteItem.external_processor,
                        mill = reporteItem.mill,
                        sap_master_coil = reporteItem.sap_master_coil,
                        part_description = reporteItem.part_description,
                        part_number = reporteItem.part_number,
                        production_line = reporteItem.production_line,
                        vehicle = reporteItem.vehicle,
                        parts_auto = reporteItem.parts_auto,
                        strokes_auto = reporteItem.strokes_auto,
                        material_type = reporteItem.material_type,
                        shape = reporteItem.shape,
                        initial_weight_part = reporteItem.initial_weight_part,
                        net_weight_part = reporteItem.net_weight_part,
                        scrap_consolidation = reporteItem.scrap_consolidation,
                        ventas_part = reporteItem.ventas_part,
                        material_cost_part = reporteItem.material_cost_part,
                        cost_of_outside_processor = reporteItem.cost_of_outside_processor,
                        additional_material_cost_part = reporteItem.additional_material_cost_part,
                        outgoing_freight_part = reporteItem.outgoing_freight_part,
                        freights_income = reporteItem.freights_income,
                        outgoing_freight = reporteItem.outgoing_freight,
                        cat_1 = reporteItem.cat_1,
                        cat_2 = reporteItem.cat_2,
                        cat_3 = reporteItem.cat_3,
                        cat_4 = reporteItem.cat_4,
                        freights_income_usd_part = reporteItem.freights_income_usd_part,
                        maniobras_usd_part = reporteItem.maniobras_usd_part,
                        customs_expenses = reporteItem.customs_expenses,
                        mostrar_advertencia = reporteItem.mostrar_advertencia,
                    });

                //copia los catálogos
                foreach (var io in reporteBD.BG_Forecast_cat_inventory_own)
                    reporte.BG_Forecast_cat_inventory_own.Add(new BG_Forecast_cat_inventory_own
                    {
                        orden = io.orden,
                        mes = io.mes,
                        cantidad = io.cantidad,
                    });

                //guarda el reporte en BD
                db.BG_Forecast_reporte.Add(reporte);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert("El reporte ha sido copiado correctamente", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            model.reporte = reporteBD;
            ViewBag.id_reporte = AddFirstItem(new SelectList(db.BG_Forecast_reporte.Where(x => x.activo == true).OrderByDescending(x => x.id), nameof(BG_Forecast_reporte.id), nameof(BG_Forecast_reporte.ConcatCodigo)), selected: model.id_reporte.ToString());

            return View(model);
        }

        /// <summary>
        /// Descarga la plantilla de ejemplo
        /// </summary>
        /// <returns></returns>
        public ActionResult FormatoPlantilla()
        {
            String ruta = System.Web.HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/PlantillaCargaForecast.xlsx");

            //byte[] array = System.IO.File.ReadAllBytes(ruta);

            FileInfo archivo = new FileInfo(ruta);

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = archivo.Name,
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(ruta, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }


        /// <summary>
        /// Edita elmento de reporte
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: reporte/EditElemento/5
        public ActionResult EditElemento(int? id, string vehicle)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_item item = db.BG_Forecast_item.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            ViewBag.s_vehicle = vehicle;
            return View(item);
        }

        // POST: reporte/EditElemento/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditElemento(BG_Forecast_item model, string s_vehicle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("El reporte se actualizó correctamente", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Edit", new { id = model.id_bg_forecast_reporte, vehicle = s_vehicle });
            }

            ViewBag.s_vehicle = s_vehicle;
            return View(model);
        }

        // GET: reporte/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            BG_Forecast_reporte model = new BG_Forecast_reporte { fecha = DateTime.Now };

            ViewBag.id_ihs_version = AddFirstItem(new SelectList(db.BG_IHS_versiones.Where(x => x.activo == true).OrderByDescending(x => x.id), nameof(BG_IHS_versiones.id), nameof(BG_IHS_versiones.ConcatVersion)),
                textoPorDefecto: "-- Seleccione un valor --");

            return View(model);
        }

        // POST: reporte/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BG_Forecast_reporte bG_Forecast_reporte)
        {
            if (db.BG_Forecast_reporte.Any(x => x.fecha == bG_Forecast_reporte.fecha && x.descripcion == bG_Forecast_reporte.descripcion))
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores.");

            if (bG_Forecast_reporte.BG_Forecast_item.Count == 0)
                ModelState.AddModelError("", "No se han agregado elementos al reporte.");

            if (ModelState.IsValid)
            {
                //busca si es posible relaciona con un elemento del ihs
                foreach (var item in bG_Forecast_reporte.BG_Forecast_item)
                {
                    //obtiene la parte del campo vehicle correspondiente al mnemonic plant 
                    string mnemonic = item.mnemonic_vehicle_plant;

                    // busca en ihs item
                    var IHS = db.BG_IHS_item.FirstOrDefault(x => x.id_ihs_version == bG_Forecast_reporte.id_ihs_version
                                                        && x.mnemonic_vehicle_plant == mnemonic);
                    if (IHS != null)
                    {
                        item.id_ihs_item = IHS.id;
                        continue; // pasa a la siguiente iteracion
                    }

                    // busca en combinaciones
                    var Combinacioneslist = db.BG_IHS_combinacion.Where(x => x.id_ihs_version == bG_Forecast_reporte.id_ihs_version && x.vehicle == item.mnemonic_vehicle_plant);
                    if (Combinacioneslist.Count() == 1)
                    {
                        item.id_ihs_combinacion = Combinacioneslist.FirstOrDefault().id;
                        continue; // pasa a la siguiente iteracion
                    }

                    // busca en divisiones
                    var Divisioneslist = db.BG_IHS_rel_division.Where(x => x.BG_IHS_division.id_ihs_version == bG_Forecast_reporte.id_ihs_version && x.vehicle == item.mnemonic_vehicle_plant);
                    if (Divisioneslist.Count() == 1)
                    {
                        item.id_ihs_rel_division = Divisioneslist.FirstOrDefault().id;
                        continue; // pasa a la siguiente iteracion
                    }
                }

                //agrega los valores por defecto de los catálogos
                bG_Forecast_reporte.BG_Forecast_cat_inventory_own = listadoInventoryOwnDefault;

                this.db.Database.CommandTimeout = 300;

                db.BG_Forecast_reporte.Add(bG_Forecast_reporte);
                TempData["Mensaje"] = new MensajesSweetAlert("El reporte se cargó correctamente", TipoMensajesSweetAlerts.SUCCESS);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_ihs_version = AddFirstItem(new SelectList(db.BG_IHS_versiones.Where(x => x.activo == true).OrderByDescending(x => x.id), nameof(BG_IHS_versiones.id), nameof(BG_IHS_versiones.ConcatVersion), selectedValue: bG_Forecast_reporte.id_ihs_version.ToString()),
              textoPorDefecto: "-- Seleccione un valor --");

            return View(bG_Forecast_reporte);
        }

        // GET: reporte/EditarReporteGeneral
        public ActionResult EditarReporteGeneral(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte model = db.BG_Forecast_reporte.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            ViewBag.Reemplazar = true;

            ViewBag.id_ihs_version = AddFirstItem(new SelectList(db.BG_IHS_versiones.Where(x => x.activo == true).OrderByDescending(x => x.id), nameof(BG_IHS_versiones.id), nameof(BG_IHS_versiones.ConcatVersion), selectedValue: model.id_ihs_version.ToString()),
              textoPorDefecto: "-- Seleccione un valor --");

            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarReporteGeneral(BG_Forecast_reporte reporte)
        {
            // Validación de duplicados y de que existan elementos en el reporte
            if (db.BG_Forecast_reporte.Any(x =>
                   x.fecha == reporte.fecha &&
                   x.id != reporte.id &&
                   x.descripcion == reporte.descripcion))
            {
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores.");
            }

            if (reporte.BG_Forecast_item == null || !reporte.BG_Forecast_item.Any())
            {
                ModelState.AddModelError("", "No se han agregado elementos al reporte.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Reemplazar = true;
                ViewBag.id_ihs_version = AddFirstItem(
                    new SelectList(db.BG_IHS_versiones.Where(x => x.activo == true)
                                                   .OrderByDescending(x => x.id),
                                   nameof(BG_IHS_versiones.id),
                                   nameof(BG_IHS_versiones.ConcatVersion),
                                   selectedValue: reporte.id_ihs_version.ToString()),
                    textoPorDefecto: "-- Seleccione un valor --");
                return View("Create", reporte);
            }

            // Recupera el reporte existente desde la base de datos y guarda la versión IHS actual
            var reporteBD = db.BG_Forecast_reporte.Find(reporte.id);
            int bdIHSVersion = reporteBD.id_ihs_version;

            // Actualiza las propiedades simples del reporte
            db.Entry(reporteBD).CurrentValues.SetValues(reporte);

            // Pre-cargar en memoria la información relacionada al IHS para evitar queries en el bucle
            int currentIHSVersion = reporte.id_ihs_version;

            // Diccionario para items IHS (clave: mnemonic_vehicle_plant, comparación sin distinguir mayúsculas/minúsculas)
            var ihsItemsDict = db.BG_IHS_item
                                 .Where(x => x.id_ihs_version == currentIHSVersion)
                                 .ToList()
                                 .ToDictionary(x => x.mnemonic_vehicle_plant?.Trim(), StringComparer.OrdinalIgnoreCase);

            // Agrupar las combinaciones por vehículo (mnemonic_vehicle_plant)
            var combinacionesDict = db.BG_IHS_combinacion
                                      .Where(x => x.id_ihs_version == currentIHSVersion)
                                      .ToList()
                                      .GroupBy(x => x.vehicle?.Trim(), StringComparer.OrdinalIgnoreCase)
                                      .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            // Agrupar las divisiones por vehículo
            var divisionesDict = db.BG_IHS_rel_division
                                   .Where(x => x.BG_IHS_division.id_ihs_version == currentIHSVersion)
                                   .ToList()
                                   .GroupBy(x => x.vehicle?.Trim(), StringComparer.OrdinalIgnoreCase)
                                   .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            // Separa los nuevos items (aquellos cuyo id es 0)
            var newItems = reporte.BG_Forecast_item.Where(x => x.id == 0).ToList();

            // Si se cambió la versión IHS o se agregaron nuevos items, se eliminan los items existentes para el reporte
            if (reporte.id_ihs_version != bdIHSVersion || newItems.Any())
            {
                var itemsToRemove = db.BG_Forecast_item.Where(x => x.id_bg_forecast_reporte == reporte.id);
                db.BG_Forecast_item.RemoveRange(itemsToRemove);
            }

            // Recorrer cada item para asignarle los identificadores provenientes del IHS
            foreach (var item in reporte.BG_Forecast_item)
            {
                var mnemonic = item.mnemonic_vehicle_plant?.Trim();
                if (!string.IsNullOrEmpty(mnemonic))
                {
                    // Buscar en IHS_item
                    if (ihsItemsDict.TryGetValue(mnemonic, out var ihs))
                    {
                        item.id_ihs_item = ihs.id;
                        continue;
                    }
                    // Buscar en combinaciones (solo si se encontró exactamente una coincidencia)
                    if (combinacionesDict.TryGetValue(mnemonic, out var combList) && combList.Count == 1)
                    {
                        item.id_ihs_combinacion = combList.First().id;
                        continue;
                    }
                    // Buscar en divisiones (solo si se encontró exactamente una coincidencia)
                    if (divisionesDict.TryGetValue(mnemonic, out var divList) && divList.Count == 1)
                    {
                        item.id_ihs_rel_division = divList.First().id;
                        continue;
                    }
                }
            }

            // Asignar al reporteBD los items correspondientes
            // Si la versión IHS cambió y no se agregaron nuevos items se asignan los items que ya tenían id (actualizados)
            // En otro caso, se asignan los nuevos items (con id == 0)
            if (reporte.id_ihs_version != bdIHSVersion && !newItems.Any())
            {
                reporteBD.BG_Forecast_item = reporte.BG_Forecast_item.Where(x => x.id != 0).ToList();
            }
            else
            {
                reporteBD.BG_Forecast_item = reporte.BG_Forecast_item.Where(x => x.id == 0).ToList();
            }

            // Marcar el reporteBD como modificado y guardar los cambios en la base de datos
            db.Entry(reporteBD).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Mensaje"] = new MensajesSweetAlert("Se ha modificado el registro correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("Index");
        }




        // GET: reporte/Edit/5
        public ActionResult Edit(int? id, string vehicle, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte bG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (bG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro


            var listado = db.BG_Forecast_item
               .Where(x =>
                        (x.id_bg_forecast_reporte == id)
                        && (x.vehicle == vehicle || string.IsNullOrEmpty(vehicle))
                      )
               .OrderBy(x => x.id)
               .Skip((pagina - 1) * cantidadRegistrosPorPagina)
              .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.BG_Forecast_item
             .Where(x =>
                        (x.id_bg_forecast_reporte == id)
                        && (x.vehicle == vehicle || string.IsNullOrEmpty(vehicle))
                      )
             .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id"] = id;
            routeValues["vehicle"] = vehicle;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };
            ViewBag.Paginacion = paginacion;

            // -- vehicle --
            List<SelectListItem> selectListVehicle = new List<SelectListItem>();
            List<string> vehicleList = db.BG_Forecast_item
               .Where(x =>
                        (x.id_bg_forecast_reporte == id)
                      )
               .Select(x => x.vehicle).Distinct().ToList();

            foreach (var itemList in vehicleList)
                selectListVehicle.Add(new SelectListItem() { Text = itemList, Value = itemList });
            ViewBag.vehicle = AddFirstItem(new SelectList(selectListVehicle, "Value", "Text", vehicle), textoPorDefecto: "-- Todos --");

            return View(listado);
        }

        // GET: reporte/EditarReporte/5
        public ActionResult EditarReporte(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte BG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (BG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }

            var listado = BG_Forecast_reporte.BG_Forecast_item.ToList();

            return View(listado);
        }


        // GET: reporte/EditarReporte/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarReporte(List<BG_Forecast_item> listado)
        {
            var result = new object[1];

            try
            {

                int idReporte = 0;
                //valida que haya elementos
                if (listado.Count > 0)
                    idReporte = listado[0].id_bg_forecast_reporte;


                if (ModelState.IsValid)
                {
                    var listadoBD = db.BG_Forecast_item.Where(x => x.id_bg_forecast_reporte == idReporte).ToList();
                    List<BG_Forecast_item> diferencias = listado.Except(listadoBD).ToList();

                    foreach (var dif in diferencias)
                    {
                        var difBD = listadoBD.FirstOrDefault(x => x.id == dif.id);

                        //busca una nueva asociación sólo en caso de que haya cambiado el mnemonic
                        if (difBD.mnemonic_vehicle_plant != dif.mnemonic_vehicle_plant)
                        {
                            string mnemonic = dif.mnemonic_vehicle_plant;
                            //borra cualquier asignacion existente
                            dif.id_asociacion_ihs = null;
                            dif.id_ihs_combinacion = null;
                            dif.id_ihs_rel_division = null;
                            // busca en ihs item
                            var IHS = db.BG_IHS_item.FirstOrDefault(x => x.id_ihs_version == difBD.BG_Forecast_reporte.id_ihs_version
                                                                && x.mnemonic_vehicle_plant == mnemonic);
                            if (IHS != null)
                                dif.id_ihs_item = IHS.id;

                            // busca en combinaciones
                            var Combinacioneslist = db.BG_IHS_combinacion.Where(x => x.id_ihs_version == difBD.BG_Forecast_reporte.id_ihs_version
                                                && x.vehicle == mnemonic);

                            if (Combinacioneslist.Count() == 1)
                                dif.id_ihs_combinacion = Combinacioneslist.FirstOrDefault().id;

                            // busca en divisiones
                            var Divisioneslist = db.BG_IHS_rel_division.Where(x => x.BG_IHS_division.id_ihs_version == difBD.BG_Forecast_reporte.id_ihs_version && x.vehicle == mnemonic);
                            if (Divisioneslist.Count() == 1)
                                dif.id_ihs_rel_division = Divisioneslist.FirstOrDefault().id;


                        }

                        db.Entry(difBD).CurrentValues.SetValues(dif);



                    }

                    db.SaveChanges();
                    //TempData["Mensaje"] = new MensajesSweetAlert("Se actualizó el reporte correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                    result[0] = new { status = "OK", value = "Se actualizó el reporte correctamente." };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                //TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message , TipoMensajesSweetAlerts.ERROR);
                result[0] = new { status = "ERROR", value = "Error: " + e.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            //TempData["Mensaje"] = new MensajesSweetAlert("Error: No se completó el proceso. Verifique los datos.", TipoMensajesSweetAlerts.ERROR);
            result[0] = new { status = "ERROR", value = "No se completó el proceso." };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // GET: reporte/EditarAsociacionIHS/5
        public ActionResult EditarAsociacionIHS(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte BG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (BG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }

            //viewbag para listado de elemento de IHS
            List<BG_IHS_item> vehicleListIHS = db.BG_IHS_item.Where(x => x.id_ihs_version == BG_Forecast_reporte.id_ihs_version).ToList();
            List<BG_IHS_combinacion> vehicleListCombinacion = db.BG_IHS_combinacion.Where(x => x.id_ihs_version == BG_Forecast_reporte.id_ihs_version && x.activo).ToList();
            List<BG_IHS_rel_division> vehicleListDivision = db.BG_IHS_rel_division.Where(x => x.BG_IHS_division.id_ihs_version == BG_Forecast_reporte.id_ihs_version).ToList();
            List<BG_ihs_vehicle_custom> vehicleListHechizos = db.BG_ihs_vehicle_custom.ToList();

            ViewBag.vehicleListIHS = vehicleListIHS;
            ViewBag.vehicleListCombinacion = vehicleListCombinacion;
            ViewBag.vehicleListDivision = vehicleListDivision;
            ViewBag.vehicleListHechizos = vehicleListHechizos;

            var listado = BG_Forecast_reporte.BG_Forecast_item.ToList();

            return View(listado);
        }


        // GET: reporte/EditarReporte/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAsociacionIHS(List<BG_Forecast_item> listado)
        {
            var result = new object[1];

            try
            {
                int idReporte = 0;
                //valida que haya elementos
                if (listado.Count > 0)
                    idReporte = listado[0].id_bg_forecast_reporte;


                var listadoBD = db.BG_Forecast_item.Where(x => x.id_bg_forecast_reporte == idReporte).ToList();

                //actualiza el id de la asociacion 
                foreach (var item in listado)
                {
                    Match m = Regex.Match(!String.IsNullOrEmpty(item.p_id_asociacion_ihs) ? item.p_id_asociacion_ihs : String.Empty, @"\d+");

                    int num_id = 0;
                    if (m.Success)
                    {  //si tiene un numero
                        int.TryParse(m.Value, out num_id);

                        //obtiene el valor de la bd
                        var forecastItem = listadoBD.Where(x => x.id == item.id).FirstOrDefault();
                        //borra la asociacion
                        forecastItem.id_ihs_item = null;
                        forecastItem.id_ihs_combinacion = null;
                        forecastItem.id_ihs_rel_division = null;
                        forecastItem.id_ihs_custom = null;


                        //separa la clave del id
                        switch (item.p_id_asociacion_ihs[0])
                        {
                            case 'I':
                                forecastItem.id_ihs_item = num_id;
                                break;
                            case 'C':
                                forecastItem.id_ihs_combinacion = num_id;
                                break;
                            case 'D':
                                forecastItem.id_ihs_rel_division = num_id;
                                break;
                            case 'H':
                                forecastItem.id_ihs_custom = num_id;
                                break;
                            default: //no hay asociacion
                                forecastItem.id_ihs_item = null;
                                forecastItem.id_ihs_combinacion = null;
                                forecastItem.id_ihs_rel_division = null;
                                forecastItem.id_ihs_custom = null;
                                break;
                        }
                    }
                    else
                    { //si no se puede convertir quita todas las asociaciones
                        //obtiene el valor de la bd
                        var forecastItem = listadoBD.Where(x => x.id == item.id).FirstOrDefault();
                        //borra la asociacion
                        forecastItem.id_ihs_item = null;
                        forecastItem.id_ihs_combinacion = null;
                        forecastItem.id_ihs_rel_division = null;
                        forecastItem.id_ihs_custom = null;
                    }

                }

                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se actualizó el reporte correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                result[0] = new { status = "OK", value = "Se actualizó el reporte correctamente." };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                //TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message , TipoMensajesSweetAlerts.ERROR);
                result[0] = new { status = "ERROR", value = "Error: " + e.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var reporte = new BG_Forecast_reporte();
            //valida que haya elementos
            if (listado.Count > 0)
                reporte = db.BG_Forecast_reporte.Find(listado[0].id_bg_forecast_reporte);

            //viewbag para listado de elemento de IHS
            List<BG_IHS_item> vehicleListIHS = db.BG_IHS_item.Where(x => x.id_ihs_version == reporte.id_ihs_version).ToList();
            List<BG_IHS_combinacion> vehicleListCombinacion = db.BG_IHS_combinacion.Where(x => x.id_ihs_version == reporte.id_ihs_version && x.activo).ToList();
            List<BG_IHS_rel_division> vehicleListDivision = db.BG_IHS_rel_division.Where(x => x.BG_IHS_division.id_ihs_version == reporte.id_ihs_version).ToList();


            ViewBag.vehicleListIHS = vehicleListIHS;
            ViewBag.vehicleListCombinacion = vehicleListCombinacion;
            ViewBag.vehicleListDivision = vehicleListDivision;

            //TempData["Mensaje"] = new MensajesSweetAlert("Error: No se completó el proceso. Verifique los datos.", TipoMensajesSweetAlerts.ERROR);
            result[0] = new { status = "ERROR", value = "No se completó el proceso." };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: reporte/ActualizaReferencias/5
        public ActionResult ActualizaReferencias(int? id)
        {
            // Verifica si el usuario tiene el rol requerido
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            // Verifica si el ID proporcionado es nulo
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Busca el reporte correspondiente en la base de datos
            var bG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (bG_Forecast_reporte == null)
                return HttpNotFound();

            int cambios = 0;

            // Obtiene los items que no tienen referencias en ihs, combinaciones o divisiones
            var itemsReporte = bG_Forecast_reporte.BG_Forecast_item
                // .Where(x => x.id_ihs_item == null && x.id_ihs_rel_division == null && x.id_ihs_combinacion == null && x.id_ihs_custom == null)
                .ToList();

            // Obtiene todos los items de IHS correspondientes a la versión del reporte
            var ihsItems = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_Forecast_reporte.id_ihs_version).ToDictionary(x => x.mnemonic_vehicle_plant, x => x.id);
            // Obtiene todas las combinaciones correspondientes a la versión del reporte
            var combinaciones = db.BG_IHS_combinacion.Where(x => x.id_ihs_version == bG_Forecast_reporte.id_ihs_version).ToDictionary(x => x.vehicle, x => x.id);
            // Obtiene todas las divisiones correspondientes a la versión del reporte
            var divisiones = db.BG_IHS_rel_division.Where(x => x.BG_IHS_division.id_ihs_version == bG_Forecast_reporte.id_ihs_version).ToDictionary(x => x.vehicle, x => x.id);
            //obtiene las referencias a los hechizos
            var hechizos = db.BG_ihs_vehicle_custom.ToDictionary(x => x.Vehicle, x => x.Id);

            // Itera sobre los items sin referencias y busca posibles relaciones
            foreach (var item in itemsReporte)
            {
                string mnemonic = item.mnemonic_vehicle_plant;

                // Busca si existe un item en IHS que coincida con el mnemonic del vehículo
                if (!string.IsNullOrEmpty(mnemonic) && ihsItems.TryGetValue(mnemonic, out var ihsItemId))
                {
                    if (ihsItemId == item.id_ihs_item)
                        continue;

                    item.id_ihs_item = ihsItemId;
                    item.id_ihs_combinacion = null;
                    item.id_ihs_rel_division = null;
                    item.id_ihs_custom = null;

                    cambios++;
                    continue;
                }

                // Busca si existe una combinación que coincida con el vehículo del item
                if (!string.IsNullOrEmpty(item.vehicle) && combinaciones.TryGetValue(item.vehicle, out var combinacionId))
                {
                    if (combinacionId == item.id_ihs_combinacion)
                        continue;

                    item.id_ihs_item = null;
                    item.id_ihs_combinacion = combinacionId;
                    item.id_ihs_rel_division = null;
                    item.id_ihs_custom = null;
                    cambios++;
                    continue;
                }

                // Busca si existe una división que coincida con el vehículo del item
                if (!string.IsNullOrEmpty(item.vehicle) && divisiones.TryGetValue(item.vehicle, out var divisionId) && divisionId != item.id_ihs_rel_division)
                {
                    if (divisionId == item.id_ihs_rel_division)
                        continue;

                    item.id_ihs_item = null;
                    item.id_ihs_combinacion = null;
                    item.id_ihs_rel_division = divisionId;
                    item.id_ihs_custom = null;
                    cambios++;
                    continue;
                }

                // Busca si existe un hechizo que coincida con el vehículo del item
                if (!string.IsNullOrEmpty(item.vehicle) && hechizos.TryGetValue(item.vehicle, out var hechizoID) && hechizoID != item.id_ihs_custom)
                {
                    if (hechizoID == item.id_ihs_custom)
                        continue;

                    item.id_ihs_item = null;
                    item.id_ihs_combinacion = null;
                    item.id_ihs_rel_division = null;
                    item.id_ihs_custom = hechizoID;
                    cambios++;
                    continue;
                }


            }

            // Si hubo cambios, guarda los cambios en la base de datos y muestra un mensaje de éxito
            if (cambios > 0)
            {
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert($"Se actualizaron: {cambios} registros, para el reporte: {bG_Forecast_reporte.descripcion}.", TipoMensajesSweetAlerts.SUCCESS);
            }
            else
                TempData["Mensaje"] = new MensajesSweetAlert($"No se detectaron cambios en el reporte: {bG_Forecast_reporte.descripcion}.", TipoMensajesSweetAlerts.INFO);


            // Redirige a la acción "Index"
            return RedirectToAction("Index");
        }


        // GET: reporte/Disable/5
        public ActionResult Disable(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte BG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (BG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }
            return View(BG_Forecast_reporte);
        }

        // POST: reporte/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            BG_Forecast_reporte item = db.BG_Forecast_reporte.Find(id);
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

        // GET: reporte/Enable/5
        public ActionResult Enable(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte BG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (BG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }
            return View(BG_Forecast_reporte);
        }

        // POST: reporte/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            BG_Forecast_reporte item = db.BG_Forecast_reporte.Find(id);
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



        /// <summary>
        /// Método que retorna las filas de las tablas de combinacion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult GetReportRows()
        {
            var result = new object[1];

            HttpPostedFileBase file = Request.Files[0];

            //Valida que se haya enviado un archivo
            if (file.FileName == "")
            {
                result[0] = new { status = "ERROR", value = "Seleccione un archivo." };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //valida que sea un archivo en excel
            string extension = Path.GetExtension(file.FileName);
            if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
            {
                result[0] = new { status = "ERROR", value = "Sólo se permiten archivos en Excel." };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            string estatus = string.Empty;
            string msj = string.Empty;

            //el archivo es válido
            List<BG_Forecast_item> lista = UtilExcel.LeeReporteBudget(file, "Plantilla", ref estatus, ref msj);

            //si hubo algún error al leer el archivo
            if (estatus.ToUpper() == "ERROR")
            {
                result[0] = new { status = estatus, value = msj };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            { //si el archivo se lee correctamente

                string body = string.Empty;
                string div_ocultos = string.Empty;

                foreach (var item in lista)
                {
                    int index = lista.IndexOf(item);

                    div_ocultos += @"
                            <input type=""hidden"" name=""BG_Forecast_item.Index"" id=""BG_Forecast_item.Index"" value=""" + index + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.mostrar_advertencia) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.mostrar_advertencia) + @""" value=""" + item.mostrar_advertencia + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.inicio_demanda) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.inicio_demanda) + @""" value=""" + item.inicio_demanda + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.fin_demanda) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.fin_demanda) + @""" value=""" + item.fin_demanda + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.id_bg_forecast_reporte) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.id_bg_forecast_reporte) + @""" value=""" + item.id_bg_forecast_reporte + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.pos) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.pos) + @""" value=""" + item.pos + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.business_and_plant) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.business_and_plant) + @""" value=""" + item.business_and_plant + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.calculo_activo) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.calculo_activo) + @""" value=""" + item.calculo_activo + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.sap_invoice_code) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.sap_invoice_code) + @""" value=""" + item.sap_invoice_code + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.invoiced_to) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.invoiced_to) + @""" value=""" + item.invoiced_to + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.number_sap_client) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.number_sap_client) + @""" value=""" + item.number_sap_client + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.shipped_to) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.shipped_to) + @""" value=""" + item.shipped_to + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.own_cm) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.own_cm) + @""" value=""" + item.own_cm + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.route) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.route) + @""" value=""" + item.route + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.plant) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.plant) + @""" value=""" + item.plant + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.external_processor) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.external_processor) + @""" value=""" + item.external_processor + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.mill) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.mill) + @""" value=""" + item.mill + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.sap_master_coil) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.sap_master_coil) + @""" value=""" + item.sap_master_coil + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.part_description) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.part_description) + @""" value=""" + item.part_description + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.part_number) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.part_number) + @""" value=""" + item.part_number + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.production_line) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.production_line) + @""" value=""" + item.production_line + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.vehicle) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.vehicle) + @""" value=""" + item.vehicle + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.parts_auto) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.parts_auto) + @""" value=""" + item.parts_auto + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.strokes_auto) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.strokes_auto) + @""" value=""" + item.strokes_auto + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.material_type) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.material_type) + @""" value=""" + item.material_type + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.shape) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.shape) + @""" value=""" + item.shape + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.initial_weight_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.initial_weight_part) + @""" value=""" + item.initial_weight_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.net_weight_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.net_weight_part) + @""" value=""" + item.net_weight_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.scrap_consolidation) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.scrap_consolidation) + @""" value=""" + item.scrap_consolidation + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.ventas_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.ventas_part) + @""" value=""" + item.ventas_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.material_cost_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.material_cost_part) + @""" value=""" + item.material_cost_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cost_of_outside_processor) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cost_of_outside_processor) + @""" value=""" + item.cost_of_outside_processor + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.additional_material_cost_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.additional_material_cost_part) + @""" value=""" + item.additional_material_cost_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.outgoing_freight_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.outgoing_freight_part) + @""" value=""" + item.outgoing_freight_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.freights_income) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.freights_income) + @""" value=""" + item.freights_income + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.outgoing_freight) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.outgoing_freight) + @""" value=""" + item.outgoing_freight + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_1) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_1) + @""" value=""" + item.cat_1 + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_2) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_2) + @""" value=""" + item.cat_2 + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_3) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_3) + @""" value=""" + item.cat_3 + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_4) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_4) + @""" value=""" + item.cat_4 + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.freights_income_usd_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.freights_income_usd_part) + @""" value=""" + item.freights_income_usd_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.maniobras_usd_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.maniobras_usd_part) + @""" value=""" + item.maniobras_usd_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.customs_expenses) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.customs_expenses) + @""" value=""" + item.customs_expenses + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.previous_sap_invoice_code) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.previous_sap_invoice_code) + @""" value=""" + item.previous_sap_invoice_code + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.mnemonic_vehicle_plant) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.mnemonic_vehicle_plant) + @""" value=""" + item.mnemonic_vehicle_plant + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.propulsion_system_type) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.propulsion_system_type) + @""" value=""" + item.propulsion_system_type + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.trans_silao_slp) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.trans_silao_slp) + @""" value=""" + item.trans_silao_slp + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.wooden_pallet_usd_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.wooden_pallet_usd_part) + @""" value=""" + item.wooden_pallet_usd_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.packaging_price_usd_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.packaging_price_usd_part) + @""" value=""" + item.packaging_price_usd_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.neopreno_usd_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.neopreno_usd_part) + @""" value=""" + item.neopreno_usd_part + @""" />
";

                    body += @"                            
                             <tr> 
                                <td>" + item.pos + "</td>" +
                                "<td>" + item.mostrar_advertencia + "</td>" +
                                "<td>" + (item.inicio_demanda.HasValue ? item.inicio_demanda.Value.ToString("yyyy-MM") : String.Empty) + "</td>" +
                                "<td>" + (item.fin_demanda.HasValue ? item.fin_demanda.Value.ToString("yyyy-MM") : String.Empty) + "</td>" +
                                "<td>" + item.cat_1 + "</td>" +
                                "<td>" + item.business_and_plant + "</td>" +
                                "<td>" + item.cat_2 + "</td>" +
                                "<td>" + item.cat_3 + "</td>" +
                                "<td>" + item.cat_4 + "</td>" +
                                "<td>" + item.calculo_activo + "</td>" +
                                "<td>" + item.sap_invoice_code + "</td>" +
                                "<td>" + item.previous_sap_invoice_code + "</td>" +
                                "<td>" + item.invoiced_to + "</td>" +
                                "<td>" + item.number_sap_client + "</td>" +
                                "<td>" + item.shipped_to + "</td>" +
                                "<td>" + item.own_cm + "</td>" +
                                "<td>" + item.route + "</td>" +
                                "<td>" + item.plant + "</td>" +
                                "<td>" + item.external_processor + "</td>" +
                                "<td>" + item.mill + "</td>" +
                                "<td>" + item.sap_master_coil + "</td>" +
                                "<td>" + item.part_description + "</td>" +
                                "<td>" + item.part_number + "</td>" +
                                "<td>" + item.production_line + "</td>" +
                                "<td>" + item.mnemonic_vehicle_plant + "</td>" +
                                "<td>" + item.vehicle + "</td>" +
                                "<td>" + item.ihs_concatenado + "</td>" +
                                "<td>" + item.production_nameplate + "</td>" +
                                "<td>" + item.propulsion_system_type + "</td>" +
                                "<td>" + item.oem + "</td>" +
                                "<td>" + item.parts_auto + "</td>" +
                                "<td>" + item.strokes_auto + "</td>" +
                                "<td>" + item.material_type + "</td>" +
                                "<td>" + item.material_short + "</td>" +
                                "<td>" + item.shape + "</td>" +
                                "<td>" + (item.initial_weight_part.HasValue ? item.initial_weight_part.Value.ToString("0.000") : String.Empty) + "</td>" +
                                "<td>" + (item.net_weight_part.HasValue ? item.net_weight_part.Value.ToString("0.000") : String.Empty) + "</td>" +
                                "<td>" + (item.eng_scrap_part.HasValue ? item.eng_scrap_part.Value.ToString("0.000") : String.Empty) + "</td>" +
                                "<td>" + item.scrap_consolidation + "</td>" +
                                "<td>" + "$ " + (item.ventas_part.HasValue ? item.ventas_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.material_cost_part.HasValue ? item.material_cost_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.cost_of_outside_processor.HasValue ? item.cost_of_outside_processor.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.vas_part.HasValue ? item.vas_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.additional_material_cost_part.HasValue ? item.additional_material_cost_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.outgoing_freight_part.HasValue ? item.outgoing_freight_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + item.trans_silao_slp + "</td>" +
                                "<td>" + "$ " + (item.vas_to.HasValue ? item.vas_to.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + item.freights_income + "</td>" +
                                "<td>" + item.outgoing_freight + "</td>" +
                                "<td>" + "$ " + (item.freights_income_usd_part.HasValue ? item.freights_income_usd_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.maniobras_usd_part.HasValue ? item.maniobras_usd_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.customs_expenses.HasValue ? item.customs_expenses.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.wooden_pallet_usd_part.HasValue ? item.wooden_pallet_usd_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.packaging_price_usd_part.HasValue ? item.packaging_price_usd_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.neopreno_usd_part.HasValue ? item.neopreno_usd_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                            @"</tr>";


                }
                body += "</tr>";

                /*
                 * enviar el contenido del body de la tabla
                 * Aumenta la longitud máxima por defecto del serializador
                 */
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                result[0] = new { status = estatus, value = body, div_ocultos = div_ocultos };
                var json = Json(result, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = 500000000;
                return json;
            }


        }
        /// <summary>
        /// Método que retorna el nombre de las hojas del archivo
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult GetReportSheetNames()
        {
            var result = new object[1];

            HttpPostedFileBase file = Request.Files[0];

            //Valida que se haya enviado un archivo
            if (file.FileName == "")
            {
                result[0] = new { status = "ERROR", value = "Seleccione un archivo." };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //valida que sea un archivo en excel
            string extension = Path.GetExtension(file.FileName);
            if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
            {
                result[0] = new { status = "ERROR", value = "Sólo se permiten archivos en Excel." };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            string estatus = string.Empty;
            string msj = string.Empty;

            //el archivo es válido
            List<string> lista = UtilExcel.LeeHojasArchivo(file, ref estatus, ref msj);

            //si hubo algún error al leer el archivo
            if (estatus == "ERROR")
            {
                result[0] = new { status = estatus, value = "<option value=\"\" selected>Seleccione</option>" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            { //si el archivo se lee correctamente

                string selects = "<option value=\"\" selected>Seleccione</option>";

                foreach (var h in lista)
                {
                    selects += "<option value=\"" + h + "\">" + h + "</option>";
                }

                result[0] = new { status = "OK", value = selects };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: reporte/GenerarReporte/
        public ActionResult GenerarReporte(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            var model = new ReporteBudgetForecastViewModel { };

            model.reporte = db.BG_Forecast_reporte.Find(id);
            model.id_reporte = id.HasValue ? id.Value : 0;
            model.nombreReporte = model.reporte != null ? ("{"+model.id_reporte+"}"+ model.reporte.descripcion) : string.Empty;

            // -- tipo de listado ORIGEN
            List<SelectListItem> selectListDemanda = new List<SelectListItem>();
            selectListDemanda.Add(new SelectListItem() { Text = Bitacoras.Util.BG_IHS_tipo_demanda.DescripcionStatus(Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL), Value = Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL });
            selectListDemanda.Add(new SelectListItem() { Text = Bitacoras.Util.BG_IHS_tipo_demanda.DescripcionStatus(Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER), Value = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER });
            ViewBag.demanda = AddFirstItem(new SelectList(selectListDemanda, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER);
            //ViewBag.id_reporte = AddFirstItem(new SelectList(db.BG_Forecast_reporte.Where(p => p.activo == true), nameof(BG_Forecast_reporte.id), nameof(BG_Forecast_reporte.ConcatCodigo)), textoPorDefecto: "-- Seleccionar --", selected: id.ToString());

            return View(model);
        }

        // GET: reporte/GenerarReporte/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GenerarReporte(ReporteBudgetForecastViewModel model)
        {
            if (ModelState.IsValid)
            {
                // --- INICIO DE LA MEDICIÓN ---
                var totalWatch = Stopwatch.StartNew();
                var stepWatch = new Stopwatch();
                Debug.WriteLine("*************************************************");
                Debug.WriteLine($"****** INICIANDO GenerarReporte ( {DateTime.Now} ) ******");

                // Obtiene el contexto del Hub de SignalR
                var context = GlobalHost.ConnectionManager.GetHubContext<BudgetForecastHub>();

                try
                {
                    // Notifica el inicio del proceso
                    context.Clients.All.recibirProgresoExcel(0, 0, 100, "Reporte");

                    // --- Medición 1: Generación del archivo Excel en memoria ---
                    Debug.WriteLine("--- INICIO: Generacion Excel (ExcelUtil.GeneraReporteBudgetForecast) ---");
                    stepWatch.Start();

                    // Genera el archivo de reporte
                    byte[] reporteBytes = ExcelUtil.GeneraReporteBudgetForecast(model, db, context);

                    context.Clients.All.recibirProgresoExcel(97, 97, 100, $"Resumen Excel procesado, creando enlace de descarga.");

                    stepWatch.Stop();
                    Debug.WriteLine($"--- FIN: Generacion Excel --- Tiempo: {stepWatch.Elapsed.TotalSeconds:F2} segundos.");
                    stepWatch.Reset();

                    // --- Medición 2: Escritura del archivo en disco ---
                    Debug.WriteLine("--- INICIO: Escritura de archivo en disco (File.WriteAllBytes) ---");
                    stepWatch.Start();

                    string tempPath = Server.MapPath("~/TempReports");
                    if (!Directory.Exists(tempPath))
                        Directory.CreateDirectory(tempPath);

                    string fileName = $"Reporte_Forecast_{model.demanda}_{DateTime.Now.Ticks}.xlsx";
                    string fullPath = Path.Combine(tempPath, fileName);
                    System.IO.File.WriteAllBytes(fullPath, reporteBytes);

                    stepWatch.Stop();
                    Debug.WriteLine($"--- FIN: Escritura de archivo --- Tiempo: {stepWatch.Elapsed.TotalSeconds:F2} segundos.");

                    // Notifica el fin del proceso
                    context.Clients.All.recibirProgresoExcel(100, 0, 100, "Se ha terminado de descargar el reporte.");
                    context.Clients.All.reporteListo($"/TempReports/{fileName}");

                    // --- FIN DE LA MEDICIÓN TOTAL ---
                    totalWatch.Stop();
                    Debug.WriteLine($"****** FINALIZADO GenerarReporte CON ÉXITO ******");
                    Debug.WriteLine($"****** Tiempo Total: {totalWatch.Elapsed.TotalSeconds:F2} segundos ******");
                    Debug.WriteLine("*************************************************");

                    return Json(new
                    {
                        status = "success",
                        message = "Reporte generado correctamente.",
                        filePath = $"/TempReports/{fileName}"
                    });
                }
                catch (Exception ex)
                {
                    // --- FIN DE LA MEDICIÓN POR ERROR ---
                    totalWatch.Stop();
                    Debug.WriteLine($"!!!!!! FINALIZADO GenerarReporte CON ERROR !!!!!!");
                    Debug.WriteLine($"!!!!!! Tiempo Total hasta el error: {totalWatch.Elapsed.TotalSeconds:F2} segundos !!!!!!");
                    Debug.WriteLine($"!!!!!! Mensaje de Error: {ex.Message} !!!!!!");
                    Debug.WriteLine("*************************************************");

                    // Notifica cualquier error al cliente
                    context.Clients.All.recibirError($"Error generando el reporte: {ex.Message}");

                    return Json(new
                    {
                        status = "error",
                        message = $"Error generando el reporte: {ex.Message}"
                    });
                }
            }

            return Json(new
            {
                status = "error",
                message = "El modelo enviado es inválido."
            });
        }

        [HttpGet]
        public ActionResult DescargarReporte(string filePath)
        {
            try
            {
                string tempFolderPath = Server.MapPath("~/TempReports");
                string fullPath = Server.MapPath(filePath);

                // Verifica si el archivo existe
                if (!System.IO.File.Exists(fullPath))
                {
                    return Json(new { status = "error", message = "El archivo no existe." }, JsonRequestBehavior.AllowGet);
                }

                // Lee el archivo en bytes
                byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);

                // Obtén el nombre del archivo
                string fileName = Path.GetFileName(fullPath);

                // Limpia todos los archivos en la carpeta temporal
                LimpiarCarpetaTemporal(tempFolderPath);

                // Devuelve el archivo como una respuesta de descarga
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = $"Error al descargar el archivo: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Elimina todos los archivos en la carpeta temporal.
        /// </summary>
        /// <param name="folderPath">Ruta de la carpeta temporal.</param>
        private void LimpiarCarpetaTemporal(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                var archivos = Directory.GetFiles(folderPath);

                foreach (var archivo in archivos)
                {
                    try
                    {
                        System.IO.File.Delete(archivo); // Elimina el archivo
                    }
                    catch (Exception ex)
                    {
                        // Manejo de errores opcional (puedes registrar el error si lo deseas)
                        System.Diagnostics.Debug.WriteLine($"Error al eliminar el archivo {archivo}: {ex.Message}");
                    }
                }
            }
        }
        // ... justo después del método Index() ...

        // GET: /BG_Forecast_reporte/GestionarCargas
        public ActionResult GestionarCargas()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            // Preparamos el ViewModel que necesita la vista
            var viewModel = new GestionarCargasViewModel
            {
                // 1. Obtenemos el historial de cargas para mostrar en la tabla
                CargasRealizadas = db.BG_CargaExcel_Cargas
                                     .Include(c => c.BG_Forecast_reporte) // Incluimos el reporte para mostrar su descripción
                                     .OrderByDescending(c => c.ID_Carga)
                                     .ToList(),


            };

            // Pasamos el mensaje de la operación anterior (si existe)
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            return View(viewModel);
        }


        // POST: /BG_Forecast_reporte/ProcesarCargaExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcesarCargaExcel(GestionarCargasViewModel model)
        {
            // 1. --- Validación del Modelo y del Archivo ---
            if (model.ArchivoExcel == null || model.ArchivoExcel.ContentLength == 0)
                ModelState.AddModelError("ArchivoExcel", "No se ha seleccionado ningún archivo.");
            else if (!Path.GetExtension(model.ArchivoExcel.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError("ArchivoExcel", "El archivo debe tener formato .xlsx.");

            if (!ModelState.IsValid)
            {
                model.CargasRealizadas = db.BG_CargaExcel_Cargas.Include(c => c.BG_Forecast_reporte).OrderByDescending(c => c.ID_Carga).ToList();
                return View("GestionarCargas", model);
            }

            try
            {
                // --- Paso A: Preparar estructuras en memoria ---
                // Usamos un Diccionario para garantizar que cada POS (posición) solo se procese una vez.
                var uniqueItems = new Dictionary<int, BG_CargaExcel_Items>();
                var datosMensualesParaGuardar = new List<BG_CargaExcel_DatosMensuales>();
                var resumenParaGuardar = new List<BG_CargaExcel_ResumenPeriodo>();

                int idForecastReporte;
                List<System.Data.DataTable> hojasValidas;

                // --- Lectura del Excel ---
                using (var reader = ExcelReaderFactory.CreateReader(model.ArchivoExcel.InputStream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = false }
                    });
                    hojasValidas = result.Tables.Cast<System.Data.DataTable>()
                        .Where(t => Regex.IsMatch(t.TableName, @"^FY \d{2}-\d{2} by Month$")).ToList();

                    if (!hojasValidas.Any())
                        throw new Exception("El archivo no contiene hojas con el formato de nombre esperado (ej. 'FY 23-24 by Month').");

                    var primeraHoja = hojasValidas.First();
                    string celdaReporte = primeraHoja.Rows[1][1]?.ToString();
                    if (string.IsNullOrWhiteSpace(celdaReporte))
                        throw new Exception($"La celda B2 de la hoja '{primeraHoja.TableName}' está vacía. Debe contener el ID del reporte.");

                    Match match = Regex.Match(celdaReporte, @"\{(\d+)\}");
                    if (!match.Success || !int.TryParse(match.Groups[1].Value, out idForecastReporte))
                        throw new Exception($"No se pudo extraer un ID de reporte válido de la celda B2 ('{celdaReporte}'). Formato esperado: 'Texto {{ID}} Texto'.");

                    if (!db.BG_Forecast_reporte.Any(r => r.id == idForecastReporte))
                        throw new Exception($"El ID de reporte {idForecastReporte} extraído del archivo no existe en la base de datos.");
                }

                // Pre-cargamos los catálogos de la BD para optimizar
                var aniosFiscales = db.BG_CargaExcel_AniosFiscales.ToList();
                var metricasDict = db.BG_CargaExcel_Metricas.ToDictionary(m => m.NombreMetrica.ToUpper().Trim(), m => m.ID_Metrica);

                // --- Paso B: PRIMERA PASADA - Consolidar Items Únicos por POS ---
                foreach (var table in hojasValidas)
                {
                    for (int i = 4; i < table.Rows.Count; i++) // Los datos inician en la fila 5 (índice 4)
                    {
                        DataRow row = table.Rows[i];
                        int? pos = GetInt(row, 0);

                        if (!pos.HasValue) continue;

                        if (!uniqueItems.ContainsKey(pos.Value))
                        {
                            var item = new BG_CargaExcel_Items
                            {
                                Item_No = GetInt(row, 0),
                                Vehicle = GetString(row, 1),
                                SAP_Invoice_Code = GetString(row, 2),
                                Is_Active = GetBool(row, 3), // Asume A=Activo(true), D=Inactivo(false)
                                Inicio_Demanda = GetDate(row, 4),
                                Fin_Demanda = GetDate(row, 5),
                                Business_And_Plant = GetString(row, 6),
                                Business = GetString(row, 7),
                                Previous_SAP_Invoice_Code = GetString(row, 8),
                                Mnemonic_Vehicle_Plant = GetString(row, 9),
                                Invoiced_To = GetString(row, 10),
                                SAP_Client_Number = GetString(row, 11),
                                Shipped_To = GetString(row, 12),
                                Own_CM = GetString(row, 13),
                                Route = GetString(row, 14),
                                Plant_Number = GetString(row, 15),
                                External_Processor = GetString(row, 16),
                                Mill = GetString(row, 17),
                                SAP_Master_Coil = GetString(row, 18),
                                Part_Description = GetString(row, 19),
                                Part_Number = GetString(row, 20),
                                Production_Line = GetString(row, 21),
                                Production_Nameplate = GetString(row, 22),
                                Propulsion_System_Type = GetString(row, 23),
                                OEM = GetString(row, 24),
                                Parts_Auto = GetDouble(row, 25),
                                Stroke_Auto = GetDouble(row, 26),
                                Material_Type = GetString(row, 27),
                                Material = GetString(row, 28),
                                Shape = GetString(row, 29),
                                Initial_Weight_Part_KG = GetDouble(row, 30),
                                Net_Weight_Part_KG = GetDouble(row, 31),
                                Eng_Scrap_Part_KG = GetDouble(row, 32),
                                Scrap_Consolidation = GetBool(row, 33),
                                Ventas_Part_USD = GetDouble(row, 34),
                                Material_Cost_Part_USD = GetDouble(row, 35),
                                Cost_Outside_Proccessor_USD = GetDouble(row, 36),
                                VAS_Part_USD = GetDouble(row, 37),
                                Additional_Material_Cost_Part_USD = GetDouble(row, 38),
                                Outgoing_Freight_PART_USD = GetDouble(row, 39),
                                Trans_Silao_SLP = GetString(row, 40),
                                Vas_TO = GetDouble(row, 41),
                                Freights_Income = GetString(row, 44),
                                Outgoing_Freight = GetString(row, 45),
                                Coils_And_Slitter = GetString(row, 46),
                                Additional_Processes = GetBool(row, 47),
                                Production_Processes = GetString(row, 48),
                                Freights_Income_USD_Per_Part = GetDouble(row, 234),
                                Maniobras_USD_Per_Part = GetDouble(row, 248),
                                Customs_Expenses_USD_Per_Part = GetDouble(row, 262),
                                Wooden_Pallets_USD_Per_Part = GetDouble(row, 328),
                                Standard_Packaging_USD_Per_Part = GetDouble(row, 342),
                                Plastic_Strips_USD_Per_Part = GetDouble(row, 356),
                                // Aquí se podrían agregar las demás columnas si es necesario
                            };
                            uniqueItems.Add(pos.Value, item);
                        }
                    }
                }

                // --- SEGUNDA PASADA: Recolectar Datos Mensuales y Resúmenes ---
                foreach (var table in hojasValidas)
                {
                    string nombreAnioFiscal = Regex.Match(table.TableName, @"(FY \d{2}-\d{2})").Groups[1].Value;
                    var anioFiscalActual = aniosFiscales.FirstOrDefault(af => af.NombreAnioFiscal.Equals(nombreAnioFiscal, StringComparison.OrdinalIgnoreCase));
                    if (anioFiscalActual == null) continue;

                    // --- Lógica para Mapear Encabezados de Métricas y Meses ---
                    var monthlyColumnMap = new List<(int ColIndex, int MetricaId, int Mes, int Anio)>();
                    DataRow metricRow = table.Rows[2]; // Fila 3 con nombres de métricas
                    DataRow monthRow = table.Rows[3];  // Fila 4 con nombres de meses
                    string currentMetricName = "";

                    System.Diagnostics.Debug.WriteLine("--- Mapeando encabezados de métricas y meses... ---");
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        string metricCell = metricRow[col]?.ToString().Trim().ToUpper();
                        if (!string.IsNullOrEmpty(metricCell))
                        {
                            currentMetricName = metricCell;
                            System.Diagnostics.Debug.WriteLine($"  [Col {col}] Nueva métrica detectada: '{currentMetricName}'");
                        }

                        if (string.IsNullOrEmpty(currentMetricName))
                        {
                            // Omitimos columnas iniciales que no tienen una métrica asignada (ej. POS, Vehicle, etc.)
                            continue;
                        }

                        if (!metricasDict.ContainsKey(currentMetricName))
                        {
                            System.Diagnostics.Debug.WriteLine($"  [Col {col}] ADVERTENCIA: La métrica '{currentMetricName}' no existe en el catálogo de la BD. Se omitirá esta columna.");
                            continue;
                        }

                        int metricaId = metricasDict[currentMetricName];
                        string monthName = monthRow[col]?.ToString();
                        int mes = ParseMes(monthName);

                        if (mes > 0)
                        {
                            int anio = (mes >= 10) ? anioFiscalActual.FechaInicio.Year : anioFiscalActual.FechaFin.Year;
                            monthlyColumnMap.Add((col, metricaId, mes, anio));
                            System.Diagnostics.Debug.WriteLine($"    -> MAPEO EXITOSO: Columna {col} = Métrica '{currentMetricName}' (ID:{metricaId}), Mes: {mes}, Año: {anio}");
                        }
                        else
                        {
                            // Esto es útil para saber si hay columnas bajo una métrica que no son meses válidos.
                            System.Diagnostics.Debug.WriteLine($"    -> MAPEO FALLIDO: Columna {col} bajo métrica '{currentMetricName}'. No se pudo reconocer el mes '{monthName}'.");
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"--- Mapeo finalizado. Se encontraron {monthlyColumnMap.Count} columnas de datos mensuales. ---");


                    // --- Procesar las filas de datos para obtener valores ---
                    for (int i = 4; i < table.Rows.Count; i++)
                    {
                        DataRow row = table.Rows[i];
                        int? pos = GetInt(row, 0);
                        if (!pos.HasValue || !uniqueItems.ContainsKey(pos.Value)) continue;

                        var itemPadre = uniqueItems[pos.Value];

                        // 1. Leer Datos Mensuales usando el mapa de columnas
                        foreach (var map in monthlyColumnMap)
                        {
                            decimal? valor = GetDecimal(row, map.ColIndex);
                            if (valor.HasValue)
                            {
                                datosMensualesParaGuardar.Add(new BG_CargaExcel_DatosMensuales
                                {
                                    BG_CargaExcel_Items = itemPadre, // Vinculamos al objeto padre
                                    ID_AnioFiscal = anioFiscalActual.ID_AnioFiscal,
                                    ID_Metrica = map.MetricaId,
                                    Anio = map.Anio,
                                    Mes = map.Mes,
                                    Valor = valor.Value
                                });
                            }
                        }

                        // 2. Leer Datos de Resumen
                        var resumen = new BG_CargaExcel_ResumenPeriodo
                        {
                            BG_CargaExcel_Items = itemPadre, // Vinculamos al objeto padre
                            ID_AnioFiscal = anioFiscalActual.ID_AnioFiscal,
                            Gross_Profit_Outgoing_Freight_Part_USD = GetDouble(row, 41), // Columna 42 es índice 41
                            Gross_Profit_Outgoing_Freight_TO_USD = GetDouble(row, 42)  // Columna 43 es índice 42
                        };
                        resumenParaGuardar.Add(resumen);
                    }
                }

                // --- Paso D: Operaciones NATIVAS de EF6 ---
                using (var transaction = db.Database.BeginTransaction())
                {
                    // 1. Creamos el objeto padre
                    var nuevaCarga = new BG_CargaExcel_Cargas
                    {
                        ID_Forecast_Reporte = idForecastReporte,
                        NombreArchivo = Path.GetFileName(model.ArchivoExcel.FileName),
                        UsuarioCarga = User.Identity.Name,
                        Comentarios = model.Comentarios,
                        FechaCarga = DateTime.Now
                    };

                    // 2. Preparamos la lista de Items (que ya contienen a sus hijos)
                    var itemsFinales = uniqueItems.Values.ToList();

                    // 3. Asignamos el padre a TODOS los hijos.
                    //    (EF necesita esto para entender la relación en cascada)
                    itemsFinales.ForEach(item => item.BG_CargaExcel_Cargas = nuevaCarga);

                    // 4. Agregamos la lista de Items al contexto. 
                    //    EF automáticamente detectará que nuevaCarga es nuevo y que
                    //    todos los 'datosMensuales' y 'resumen' también son nuevos.
                    db.BG_CargaExcel_Items.AddRange(itemsFinales);

                    // 5. ¡Guardamos TODO de una sola vez! (Este es el paso lento)
                    //    EF6 se encargará de:
                    //    a. Insertar 'nuevaCarga' y obtener su ID.
                    //    b. Insertar TODOS los 'itemsFinales', asignando el ID_Carga.
                    //    c. Obtener TODOS los nuevos ID_Item de esos items.
                    //    d. Insertar TODOS los 'datosMensuales' y 'resumen', asignando los ID_Item correctos.
                    db.SaveChanges();

                    // Si todo salió bien, confirmamos la transacción.
                    transaction.Commit();
                }


                TempData["AlertMessage"] = $"Archivo procesado exitosamente. Se encontraron y guardaron {uniqueItems.Count} items únicos.";
                TempData["AlertType"] = "success";
                return RedirectToAction("GestionarCargas");
            }
            catch (Exception ex)
            {
                model.CargasRealizadas = db.BG_CargaExcel_Cargas.Include(c => c.BG_Forecast_reporte).OrderByDescending(c => c.ID_Carga).ToList();
                model.AlertMessage = $"Error al procesar el archivo: {ex.Message}";
                model.AlertType = "danger";
                return View("GestionarCargas", model);
            }
        }

        // Método de ayuda para parsear el nombre del mes
        private int ParseMes(string monthName)
        {
            if (string.IsNullOrWhiteSpace(monthName)) return 0;

            // Extrae las primeras tres letras y las convierte a mayúsculas
            string monthAbbreviation = monthName.Trim().Substring(0, 3).ToUpper();

            switch (monthAbbreviation)
            {
                case "ENE": // Español
                case "JAN": // Inglés
                    return 1;
                case "FEB":
                    return 2;
                case "MAR":
                    return 3;
                case "ABR": // Español
                case "APR": // Inglés
                    return 4;
                case "MAY":
                    return 5;
                case "JUN":
                    return 6;
                case "JUL":
                    return 7;
                case "AGO": // Español
                case "AUG": // Inglés
                    return 8;
                case "SEP":
                    return 9;
                case "OCT":
                    return 10;
                case "NOV":
                    return 11;
                case "DIC": // Español
                case "DEC": // Inglés
                    return 12;
                default:
                    return 0; // Si no coincide con ninguno, no es un mes válido
            }
        }

        #region Métodos de Ayuda para Lectura de Excel

        private string GetString(DataRow row, int index)
        {
            // Devuelve el valor de la celda como texto o null si está vacía.
            return row.Table.Columns.Count > index && row[index] != DBNull.Value ? row[index].ToString().Trim() : null;
        }

        private int? GetInt(DataRow row, int index)
        {
            // Intenta convertir el valor de la celda a un número entero.
            if (row.Table.Columns.Count > index && row[index] != DBNull.Value && int.TryParse(row[index].ToString(), out int result))
            {
                return result;
            }
            return null;
        }


        private decimal? GetDecimal(DataRow row, int index)
        {
            if (index < 0 || row.Table.Columns.Count <= index || row[index] == DBNull.Value) return null;
            if (decimal.TryParse(row[index].ToString(), out decimal result)) return result;
            return null;
        }
        private double? GetDouble(DataRow row, int index)
        {
            // Intenta convertir el valor de la celda a un número con decimales.
            if (row.Table.Columns.Count > index && row[index] != DBNull.Value && double.TryParse(row[index].ToString(), out double result))
            {
                return result;
            }
            return null;
        }

        private DateTime? GetDate(DataRow row, int index)
        {
            // Intenta convertir el valor de la celda a una fecha.
            if (row.Table.Columns.Count > index && row[index] != DBNull.Value && DateTime.TryParse(row[index].ToString(), out DateTime result))
            {
                return result;
            }
            return null;
        }

        private bool? GetBool(DataRow row, int index)
        {
            // Intenta convertir el valor de la celda a un booleano (verdadero/falso).
            if (row.Table.Columns.Count > index && row[index] != DBNull.Value)
            {
                string val = row[index].ToString().Trim().ToUpper();
                if (val == "TRUE" || val == "VERDADERO" || val == "1" || val == "A" || val == "YES")
                {
                    return true;
                }
                if (val == "FALSE" || val == "FALSO" || val == "0" || val == "D" || val == "NO")
                {
                    return false;
                }
            }
            return null;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    public class GestionarCargasViewModel
    {
        // Para mostrar la tabla del historial de cargas
        public IEnumerable<BG_CargaExcel_Cargas> CargasRealizadas { get; set; }

        // --- Propiedades para el Formulario de Carga ---
        [Required(ErrorMessage = "Debe seleccionar un archivo Excel.")]
        [Display(Name = "Archivo Excel (.xlsx)")]
        public HttpPostedFileBase ArchivoExcel { get; set; }

        [Display(Name = "Comentarios")]
        public string Comentarios { get; set; }

        // --- Propiedades para Alertas ---
        public string AlertMessage { get; set; }
        public string AlertType { get; set; } // "success", "danger", "warning", "info"
    }
}
