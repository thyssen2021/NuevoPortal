using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BG_IHS_itemController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_IHS_item
        public ActionResult ListadoIHS(string country_territory, string manufacturer, string production_plant, string vehicle, string origen, string demanda = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro

            //verifica que el elemento este relacionado con el elmento anterior
            if (
                !db.BG_IHS_item.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory) && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)) && x.manufacturer == manufacturer))
                manufacturer = String.Empty;

            if (
                !db.BG_IHS_item.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory))
                                            && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                                            && x.production_plant == production_plant
                                            && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                                            ))
                production_plant = String.Empty;

            if (
                !db.BG_IHS_item.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory))
                                            && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                                            && (x.production_plant == production_plant || String.IsNullOrEmpty(production_plant))
                                            && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                                            && vehicle == x.vehicle))
                vehicle = String.Empty;

            var listado = db.BG_IHS_item
                .Where(x =>
                    (x.country_territory == country_territory || String.IsNullOrEmpty(country_territory))
                    && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                    && (x.production_plant == production_plant || String.IsNullOrEmpty(production_plant))
                    && (x.vehicle == vehicle || String.IsNullOrEmpty(vehicle))
                    && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                    )
               .OrderBy(x => x.id)
               .Skip((pagina - 1) * cantidadRegistrosPorPagina)
              .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.BG_IHS_item
                .Where(x =>
                    (x.country_territory == country_territory || String.IsNullOrEmpty(country_territory))
                    && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                    && (x.production_plant == production_plant || String.IsNullOrEmpty(production_plant))
                    && (x.vehicle == vehicle || String.IsNullOrEmpty(vehicle))
                    && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                    )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["country_territory"] = country_territory;
            routeValues["manufacturer"] = manufacturer;
            routeValues["production_plant"] = production_plant;
            routeValues["vehicle"] = vehicle;
            routeValues["origen"] = origen;
            routeValues["demanda"] = demanda;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };
            ViewBag.Paginacion = paginacion;

            //listas para combos
            // -- Country --
            List<SelectListItem> selectListCountry = new List<SelectListItem>();
            List<string> countriesList = db.BG_IHS_item.Where(x => (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION) && !String.IsNullOrEmpty(x.country_territory)).Select(x => x.country_territory).Distinct().ToList();
            foreach (var itemList in countriesList)
                selectListCountry.Add(new SelectListItem() { Text = itemList, Value = itemList });
            ViewBag.country_territory = AddFirstItem(new SelectList(selectListCountry, "Value", "Text", country_territory), textoPorDefecto: "-- Todos --");

            // -- manufacturer --
            List<SelectListItem> selectListManufacturer = new List<SelectListItem>();
            List<string> manufacturerList = db.BG_IHS_item.Where(x => (x.country_territory == country_territory || string.IsNullOrEmpty(country_territory)) && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION) && !String.IsNullOrEmpty(x.manufacturer)).Select(x => x.manufacturer).Distinct().ToList();
            foreach (var itemList in manufacturerList)
                selectListManufacturer.Add(new SelectListItem() { Text = itemList, Value = itemList });
            ViewBag.manufacturer = AddFirstItem(new SelectList(selectListManufacturer, "Value", "Text", manufacturer), textoPorDefecto: "-- Todos --");

            // -- production_plant --
            List<SelectListItem> selectListProduction_plant = new List<SelectListItem>();
            List<string> production_plantList = db.BG_IHS_item.Where(x =>
                  (x.country_territory == country_territory || string.IsNullOrEmpty(country_territory))
               && (x.manufacturer == manufacturer || string.IsNullOrEmpty(manufacturer))
               && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                && !String.IsNullOrEmpty(x.production_plant)
                ).Select(x => x.production_plant).Distinct().ToList();
            foreach (var itemList in production_plantList)
                selectListProduction_plant.Add(new SelectListItem() { Text = itemList, Value = itemList });
            ViewBag.production_plant = AddFirstItem(new SelectList(selectListProduction_plant, "Value", "Text", production_plant), textoPorDefecto: "-- Todos --");

            // -- vehicle --
            List<SelectListItem> selectListVehicle = new List<SelectListItem>();
            List<string> vehicleList = db.BG_IHS_item.Where(x =>
             (x.country_territory == country_territory || string.IsNullOrEmpty(country_territory))
               && (x.manufacturer == manufacturer || string.IsNullOrEmpty(manufacturer))
                && (x.production_plant == production_plant || string.IsNullOrEmpty(production_plant))
                && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                 && !String.IsNullOrEmpty(x.vehicle)
                ).Select(x => x.vehicle).Distinct().ToList();
            foreach (var itemList in vehicleList)
                selectListVehicle.Add(new SelectListItem() { Text = itemList, Value = itemList });
            ViewBag.vehicle = AddFirstItem(new SelectList(selectListVehicle, "Value", "Text", vehicle), textoPorDefecto: "-- Todos --");

            // -- tipo de listado ORIGEN
            List<SelectListItem> selectListTipo = new List<SelectListItem>();
            selectListTipo.Add(new SelectListItem() { Text = Bitacoras.Util.BG_IHS_Origen.IHS, Value = Bitacoras.Util.BG_IHS_Origen.IHS });
            selectListTipo.Add(new SelectListItem() { Text = Bitacoras.Util.BG_IHS_Origen.USER, Value = Bitacoras.Util.BG_IHS_Origen.USER });
            selectListTipo.Add(new SelectListItem() { Text = Bitacoras.Util.BG_IHS_Origen.UNION, Value = Bitacoras.Util.BG_IHS_Origen.UNION });
            ViewBag.origen = AddFirstItem(new SelectList(selectListTipo, "Value", "Text", origen), textoPorDefecto: "-- Seleccionar --");

            // -- tipo de listado ORIGEN
            List<SelectListItem> selectListDemanda = new List<SelectListItem>();
            selectListDemanda.Add(new SelectListItem() { Text = Bitacoras.Util.BG_IHS_tipo_demanda.DescripcionStatus(Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL), Value = Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL });
            selectListDemanda.Add(new SelectListItem() { Text = Bitacoras.Util.BG_IHS_tipo_demanda.DescripcionStatus(Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER), Value = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER });
            ViewBag.demanda = new SelectList(selectListDemanda, "Value", "Text", demanda);


            //Envia el titulo para la vista
            if (!String.IsNullOrEmpty(origen) && origen == Bitacoras.Util.BG_IHS_Origen.IHS)
                ViewBag.Title = "Listado de IHS (Archivo)";
            else if (!String.IsNullOrEmpty(origen) && origen == Bitacoras.Util.BG_IHS_Origen.USER)
                ViewBag.Title = "Listado de IHS (Usuario)";
            else if (!String.IsNullOrEmpty(origen) && origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                ViewBag.Title = "Listado de IHS (Unión)";
            else
                ViewBag.Title = "Listado de IHS";

            return View(listado);
        }

        // GET: Bom/CargaIHS/5
        public ActionResult CargaIHS()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            return View();

        }

        // POST: Bom/CargaIHS/5
        [HttpPost]
        public ActionResult CargaIHS(ExcelViewModel excelViewModel, FormCollection collection)
        {
            if (ModelState.IsValid)
            {

                string msjError = "No se ha podido leer el archivo seleccionado.";

                //lee el archivo seleccionado
                try
                {
                    HttpPostedFileBase stream = Request.Files["PostedFile"];


                    if (stream.InputStream.Length > 5242880)
                    {
                        msjError = "Sólo se permiten archivos con peso menor a 5 MB.";
                        throw new Exception(msjError);
                    }
                    else
                    {
                        string extension = Path.GetExtension(excelViewModel.PostedFile.FileName);
                        if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                        {
                            msjError = "Sólo se permiten archivos Excel";
                            throw new Exception(msjError);
                        }
                    }

                    bool estructuraValida = false;

                    //el archivo es válido
                    List<BG_IHS_item> lista = UtilExcel.LeeIHS(excelViewModel.PostedFile, ref estructuraValida, ref msjError);

                    if (!estructuraValida)
                    {
                        // msjError = "No cumple con la estructura válida.";
                        throw new Exception(msjError);
                    }
                    else
                    {
                        int actualizados = 0;
                        int creados = 0;
                        int error = 0;
                        int demandaAct = 0;
                        int demandaCreate = 0;
                        int quarterAct = 0;
                        int quarterCreate = 0;

                        //obtiene el listado actual de la BD
                        List<BG_IHS_item> listAnteriorBG = db.BG_IHS_item.ToList();
                        //obtiene el listado actual de la BD rels demanda
                        List<BG_IHS_rel_demanda> listAnteriorRelDemanda = db.BG_IHS_rel_demanda.ToList();
                        //obtiene el listado actual de la BD rels cuartos
                        List<BG_IHS_rel_cuartos> listAnteriorRelCuartos = db.BG_IHS_rel_cuartos.ToList();

                        //obtiene el listado de diferencias
                        //List<BG_IHS_item> listDiferencias = lista.Except(listAnterior).ToList();

                        foreach (BG_IHS_item ihs in lista)
                        {
                            try
                            {
                                //obtiene el elemento de BD, en base al primary key
                                BG_IHS_item item = listAnteriorBG.FirstOrDefault(x => x.mnemonic_vehicle_plant == ihs.mnemonic_vehicle_plant);

                                //si existe actualiza o lo ignora
                                if (item != null)
                                {
                                    //si es diferente a BD lo actualiza
                                    if (!ihs.Equals(item))
                                    {
                                        ihs.id = item.id;
                                        db.Entry(item).CurrentValues.SetValues(ihs);
                                        actualizados++;
                                    }
                                    //tanto si es diferente o es igual actualiza los rel demanda
                                    //recorre todos los items de la demanda
                                    foreach (var rel in ihs.BG_IHS_rel_demanda)
                                    {
                                        //busca el item en bd
                                        BG_IHS_rel_demanda itemRel = listAnteriorRelDemanda.FirstOrDefault(x => x.id_ihs_item == item.id && x.fecha == rel.fecha);

                                        if (itemRel != null) //existe
                                        {
                                            //actualiza
                                            if (itemRel.cantidad != rel.cantidad)
                                            {
                                                rel.id = itemRel.id;
                                                rel.id_ihs_item = item.id;
                                                db.Entry(itemRel).CurrentValues.SetValues(rel);
                                                demandaAct++;
                                            }
                                            //ignora

                                        }
                                        else
                                        { //lo crea en la BD
                                            rel.id_ihs_item = item.id;
                                            db.BG_IHS_rel_demanda.Add(rel);
                                            demandaCreate++;
                                        }
                                    }

                                    //tanto si es diferente o es igual actualiza los rel demanda
                                    //recorre todos los items de cuartos
                                    foreach (var rel in ihs.BG_IHS_rel_cuartos)
                                    {
                                        //busca el item en bd
                                        BG_IHS_rel_cuartos itemRel = listAnteriorRelCuartos.FirstOrDefault(x => x.id_ihs_item == item.id && x.cuarto == rel.cuarto && x.anio == rel.anio);

                                        if (itemRel != null) //existe
                                        {
                                            //actualiza
                                            if (itemRel.cantidad != rel.cantidad)
                                            {
                                                rel.id = itemRel.id;
                                                rel.id_ihs_item = item.id;
                                                db.Entry(itemRel).CurrentValues.SetValues(rel);
                                                quarterAct++;
                                            }
                                            //ignora

                                        }
                                        else
                                        { //lo crea en la BD
                                            rel.id_ihs_item = item.id;
                                            db.BG_IHS_rel_cuartos.Add(rel);
                                            quarterCreate++;
                                        }
                                    }

                                }
                                else //si no existe lo crea
                                {
                                    //crea un nuevo registro, con todo y la demanda asociada
                                    db.BG_IHS_item.Add(ihs);
                                    creados++;
                                }
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                error++;
                            }

                        }

                        TempData["Mensaje"] = new MensajesSweetAlert("Actualizados: " + actualizados + "; Creados: " + creados 
                            + "; DemandasActualizadas: " + demandaAct + "; DemandasCrea: " + demandaCreate 
                            + "; CuartosActualizados: " + quarterAct + "; CuartosCreados: " + quarterCreate + "; Errores: " + error, TipoMensajesSweetAlerts.INFO);
                        return RedirectToAction("ListadoIHS", new { origen = Bitacoras.Util.BG_IHS_Origen.IHS });
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", msjError);
                    return View(excelViewModel);
                }

            }
            return View(excelViewModel);
        }

        // GET: BG_IHS_item/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_item bG_IHS_item = db.BG_IHS_item.Find(id);
            if (bG_IHS_item == null)
            {
                return HttpNotFound();
            }
            return View(bG_IHS_item);
        }

        // GET: BG_IHS_item/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");


            BG_IHS_item model = new BG_IHS_item { porcentaje_scrap = 3.0M };

            //obtiene el menor año de los archivos cargados de ihs
            int anoInicio = 2019, anoFin = 2030;
            try
            {
                anoInicio = db.BG_IHS_rel_demanda.OrderBy(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
                anoFin = db.BG_IHS_rel_demanda.OrderByDescending(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
            }
            catch (Exception) { /* do nothing */ }

            for (int i = anoInicio; i <= anoFin; i++)
            {
                for (int j = 1; j <= 12; j++)
                {
                    model.BG_IHS_rel_demanda.Add(new BG_IHS_rel_demanda
                    {
                        //cantidad = 0,
                        fecha = new DateTime(i, j, 1),
                        tipo = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER
                    });
                }
            }

            return View(model);
        }

        // POST: BG_IHS_item/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BG_IHS_item bG_IHS_item)
        {
            if (db.BG_IHS_item.Any(x => x.mnemonic_vehicle_plant == bG_IHS_item.mnemonic_vehicle_plant))
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores para Mnemonic-Vehicle/Plant, Vehicle y SOP " + bG_IHS_item.mnemonic_vehicle_plant);

            if (ModelState.IsValid)
            {
                //coloca el porcentaje en decimales
                bG_IHS_item.porcentaje_scrap = bG_IHS_item.porcentaje_scrap / 100; 
                db.BG_IHS_item.Add(bG_IHS_item);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("ListadoIHS", new { origen = Bitacoras.Util.BG_IHS_Origen.USER });
            }

            return View(bG_IHS_item);
        }

        // GET: BG_IHS_item/Edit/5
        public ActionResult Edit(int? id, string s_country_territory, string s_manufacturer, string s_production_plant, string s_vehicle, string s_origen, string s_demanda)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_item bG_IHS_item = db.BG_IHS_item.Find(id);
            if (bG_IHS_item == null)
            {
                return HttpNotFound();
            }

            //determina si es necesario agregar más items en rels
            //obtiene el menor año de los archivos cargados de ihs
            int anoInicio = 2019, anoFin = 2030;
            try
            {
                anoInicio = db.BG_IHS_rel_demanda.OrderBy(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
                anoFin = db.BG_IHS_rel_demanda.OrderByDescending(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
            }
            catch (Exception) { /* do nothing */ }

            for (int i = anoInicio; i <= anoFin; i++)
            {
                for (int j = 1; j <= 12; j++)
                {
                    DateTime fecha = new DateTime(i, j, 1);
                    //si no existe un campo con los mismos valores lo agrega
                    if (!bG_IHS_item.BG_IHS_rel_demanda.Any(x => x.fecha == fecha && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER))
                    {

                        bG_IHS_item.BG_IHS_rel_demanda.Add(new BG_IHS_rel_demanda
                        {
                            // cantidad = bG_IHS_item.BG_IHS_rel_demanda.Where(x => x.fecha == fecha && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL).Select(x=> x.cantidad).FirstOrDefault(),
                            id_ihs_item = bG_IHS_item.id,
                            fecha = fecha,
                            tipo = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER
                        });
                    }
                }
            }

            //viewbag para redirect
            ViewBag.s_origen = s_origen;
            ViewBag.s_demanda = s_demanda;
            ViewBag.s_country_territory = s_country_territory;
            ViewBag.s_manufacturer = s_manufacturer;
            ViewBag.s_production_plant = s_production_plant;
            ViewBag.s_vehicle = s_vehicle;

            return View(bG_IHS_item);
        }

        // POST: BG_IHS_item/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BG_IHS_item bG_IHS_item, string s_country_territory, string s_manufacturer, string s_production_plant, string s_vehicle, string s_origen, string s_demanda)
        {
            if (ModelState.IsValid)
            {
                var rels = bG_IHS_item.BG_IHS_rel_demanda;

                bG_IHS_item.BG_IHS_rel_demanda =new List<BG_IHS_rel_demanda>();
                //coloca el porcentaje en decimales
                bG_IHS_item.porcentaje_scrap = bG_IHS_item.porcentaje_scrap / 100;

                db.Entry(bG_IHS_item).State = EntityState.Modified;
                try
                {

                    //obtiene los rels actuales en la bd
                    var rels_demanda_customer = db.BG_IHS_rel_demanda.Where(x => x.id_ihs_item == bG_IHS_item.id && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER);

                    //recorre los rels y los actualiza o crea
                    foreach (var rel in rels)
                    {
                        
                        //busca el elemento
                        var item = rels_demanda_customer.Where(x => x.fecha == rel.fecha).FirstOrDefault();

                        if (item !=null)//si ya existe lo actualiza
                        {
                            rel.id = item.id;
                            db.Entry(item).CurrentValues.SetValues(rel);
                        }
                        else { //si no existe, lo crea
                            db.BG_IHS_rel_demanda.Add(rel);
                        }
                    }
                    db.SaveChanges();                    

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("ListadoIHS", new { origen = s_origen, country_territory = s_country_territory, manufacturer= s_manufacturer, production_plant = s_production_plant, vehicle = s_vehicle, demanda= s_demanda });
                }
                catch(Exception e) {
                    TempData["Mensaje"] = new MensajesSweetAlert(e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("ListadoIHS", new { origen = s_origen, country_territory = s_country_territory, manufacturer = s_manufacturer, production_plant = s_production_plant, vehicle = s_vehicle, demanda = s_demanda });
                }
            }

            //viewbag para redirect
            ViewBag.s_origen = s_origen;
            ViewBag.s_demanda = s_demanda;
            ViewBag.s_country_territory = s_country_territory;
            ViewBag.s_manufacturer = s_manufacturer;
            ViewBag.s_production_plant = s_production_plant;
            ViewBag.s_vehicle = s_vehicle;

            var rels_demanda_original = db.BG_IHS_rel_demanda.Where(x => x.id_ihs_item == bG_IHS_item.id && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL);
            foreach (var item in rels_demanda_original)
                bG_IHS_item.BG_IHS_rel_demanda.Add(item);

            return View(bG_IHS_item);
        }

        // GET: BG_IHS_item/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_item bG_IHS_item = db.BG_IHS_item.Find(id);
            if (bG_IHS_item == null)
            {
                return HttpNotFound();
            }
            return View(bG_IHS_item);
        }

        // POST: BG_IHS_item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BG_IHS_item bG_IHS_item = db.BG_IHS_item.Find(id);
            db.BG_IHS_item.Remove(bG_IHS_item);
            db.SaveChanges();
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
