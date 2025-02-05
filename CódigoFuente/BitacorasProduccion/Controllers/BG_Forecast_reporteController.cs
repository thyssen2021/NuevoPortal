using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
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
        public ActionResult EditarReporteGeneral(BG_Forecast_reporte bG_Forecast_reporte)
        {

            if (db.BG_Forecast_reporte.Any(x => x.fecha == bG_Forecast_reporte.fecha
            && x.id != bG_Forecast_reporte.id
            && x.descripcion == bG_Forecast_reporte.descripcion))
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores.");

            if (bG_Forecast_reporte.BG_Forecast_item.Count == 0)
                ModelState.AddModelError("", "No se han agregado elementos al reporte.");

            if (ModelState.IsValid)
            {
                //si existe lo modifica
                BG_Forecast_reporte reporteBD = db.BG_Forecast_reporte.Find(bG_Forecast_reporte.id);

                int bdIHSVersion = reporteBD.id_ihs_version;

                // Activity already exist in database and modify it
                db.Entry(reporteBD).CurrentValues.SetValues(bG_Forecast_reporte);

                //obtiene un listado con los item del reporte nuevo
                var newList = bG_Forecast_reporte.BG_Forecast_item.Where(x => x.id == 0).ToList();

                //valida si hubo cambio de IHS y no se agregaron nuevos elementos
                if (bG_Forecast_reporte.id_ihs_version != bdIHSVersion && newList.Count == 0)
                {
                    //borra los anteriores
                    var list = db.BG_Forecast_item.Where(x => x.id_bg_forecast_reporte == bG_Forecast_reporte.id);
                    db.BG_Forecast_item.RemoveRange(list);

                }

                //busca si es posible relaciona con un elemento del ihs (en caso de que se haya reemplado el reporte)
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

                //si hay nuevos borra todos los anteriores
                if (newList.Count > 0)
                {
                    //borra los anteriores
                    var list = db.BG_Forecast_item.Where(x => x.id_bg_forecast_reporte == bG_Forecast_reporte.id);
                    db.BG_Forecast_item.RemoveRange(list);
                }

                //agrega los conceptos (que no tengan id)
                reporteBD.BG_Forecast_item = bG_Forecast_reporte.BG_Forecast_item.Where(x => x.id == 0).ToList();

                //si es cabio de IHS, agrega todos los conceptos que tengan id
                if (bG_Forecast_reporte.id_ihs_version != bdIHSVersion && newList.Count == 0)
                {
                    reporteBD.BG_Forecast_item = bG_Forecast_reporte.BG_Forecast_item.Where(x => x.id != 0).ToList();
                }

                db.Entry(reporteBD).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert("Se ha modificado el registro correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.Reemplazar = true;
            ViewBag.id_ihs_version = AddFirstItem(new SelectList(db.BG_IHS_versiones.Where(x => x.activo == true).OrderByDescending(x => x.id), nameof(BG_IHS_versiones.id), nameof(BG_IHS_versiones.ConcatVersion), selectedValue: bG_Forecast_reporte.id_ihs_version.ToString()),
              textoPorDefecto: "-- Seleccione un valor --");
            return View("Create", bG_Forecast_reporte);
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
                // Obtiene el contexto del Hub de SignalR
                var context = GlobalHost.ConnectionManager.GetHubContext<BudgetForecastHub>();

                // Inicia el proceso de generación del reporte en una tarea asincrónica

                try
                {
                    // Notifica el inicio del proceso
                    context.Clients.All.recibirProgresoExcel(0, 0, 100, "Reporte");


                    // Genera el archivo de reporte
                    byte[] reporteBytes = ExcelUtil.GeneraReporteBudgetForecast(model, db, context);
                    string tempPath = Server.MapPath("~/TempReports");
                    if (!Directory.Exists(tempPath))
                        Directory.CreateDirectory(tempPath);

                    string fileName = $"Reporte_Forecast_{model.demanda}_{DateTime.Now.Ticks}.xlsx";
                    string fullPath = Path.Combine(tempPath, fileName);
                    System.IO.File.WriteAllBytes(fullPath, reporteBytes);

                    // Notifica el inicio del proceso
                    context.Clients.All.recibirProgresoExcel(100, 0, 100, "Se ha terminado de descargar el reporte.");
                    // Notifica al cliente que el archivo está listo
                    context.Clients.All.reporteListo($"/TempReports/{fileName}");

                    // Retorna una respuesta JSON con la ruta del archivo
                    return Json(new
                    {
                        status = "success",
                        message = "Reporte generado correctamente.",
                        filePath = $"/TempReports/{fileName}"
                    });
                }
                catch (Exception ex)
                {
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
