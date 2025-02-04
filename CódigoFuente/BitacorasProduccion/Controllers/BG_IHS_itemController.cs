using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Ajax.Utilities;
using Portal_2_0.Models;
using WebGrease;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BG_IHS_itemController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        private List<string> IHSRegionesDefault = new List<string> { "CENTER", "EXPORT", "NORTH", "NORTH-WEST", "SOUTH" };
        private List<BG_IHS_plantas> IHSPlantasDefault = new List<BG_IHS_plantas> {
                 new BG_IHS_plantas { mnemonic_plant = "120005", descripcion = "Aguascalientes (A1)" },
                 new BG_IHS_plantas { mnemonic_plant = "121032", descripcion = "Aguascalientes (A2)" },
                 new BG_IHS_plantas { mnemonic_plant = "120980", descripcion = "Celaya" },
                 new BG_IHS_plantas { mnemonic_plant = "120203", descripcion = "Cuautitlan" },
                 new BG_IHS_plantas { mnemonic_plant = "120205", descripcion = "Cuernavaca #1" },
                 new BG_IHS_plantas { mnemonic_plant = "120206", descripcion = "Cuernavaca #2" },         
                 //"EL Salto"
                 new BG_IHS_plantas { mnemonic_plant = "120325", descripcion = "Hermosillo" },
                 new BG_IHS_plantas { mnemonic_plant = "121116", descripcion = "Monterrey" },
                 new BG_IHS_plantas { mnemonic_plant = "120602", descripcion = "Puebla #1" },
                 new BG_IHS_plantas { mnemonic_plant = "120621", descripcion = "Ramos Arizpe #2" },
                 new BG_IHS_plantas { mnemonic_plant = "120481", descripcion = "Salamanca" },
                 new BG_IHS_plantas { mnemonic_plant = "121085", descripcion = "Guanajuato" },
                 new BG_IHS_plantas { mnemonic_plant = "120649", descripcion = "Saltillo Truck" },
                 new BG_IHS_plantas { mnemonic_plant = "121081", descripcion = "Saltillo Van" },
                 new BG_IHS_plantas { mnemonic_plant = "121046", descripcion = "San Jose Chiapa" },
                 new BG_IHS_plantas { mnemonic_plant = "121084", descripcion = "San Luis Potosi" },
                 new BG_IHS_plantas { mnemonic_plant = "120661", descripcion = "San Luis Potosi" },
                 new BG_IHS_plantas { mnemonic_plant = "120723", descripcion = "Silao" },
                 new BG_IHS_plantas { mnemonic_plant = "120820", descripcion = "Tijuana" },
                 new BG_IHS_plantas { mnemonic_plant = "120828", descripcion = "Toluca" },
                 new BG_IHS_plantas { mnemonic_plant = "121357", descripcion = "Sahagun" },
                 new BG_IHS_plantas { mnemonic_plant = "121146", descripcion = "Aguascalientes" },
                 new BG_IHS_plantas { mnemonic_plant = "120751", descripcion = "Sterling Heights" }, 
                 //"Ingersoll #2",
                 new BG_IHS_plantas { mnemonic_plant = "121395", descripcion = "Fremont" },
                 //Ridgeville
                 new BG_IHS_plantas { mnemonic_plant = "120448", descripcion = "Louisville" },  
                 //"Chattanooga"
                 new BG_IHS_plantas { mnemonic_plant = "120739", descripcion = "Spartanburg North" },
                 new BG_IHS_plantas { mnemonic_plant = "120874", descripcion = "Warren Truck" },  
                 //"Cami"
                 new BG_IHS_plantas { mnemonic_plant = "121136", descripcion = "Charleston" },
                 new BG_IHS_plantas { mnemonic_plant = "120153", descripcion = "Charleston" },
                 new BG_IHS_plantas { mnemonic_plant = "120843", descripcion = "Tuscaloosa #1" },  
                 //"Gigafactory Texas"
                 new BG_IHS_plantas { mnemonic_plant = "120551", descripcion = "Orion" },
                 new BG_IHS_plantas { mnemonic_plant = "120737", descripcion = "Spartanburg" },
                 new BG_IHS_plantas { mnemonic_plant = "121421", descripcion = "Spartanburg" },
                 new BG_IHS_plantas { mnemonic_plant = "120341", descripcion = "CAMI #2" },
                 new BG_IHS_plantas { mnemonic_plant = "121398", descripcion = "Gigafactory Texas #2" },
                 new BG_IHS_plantas { mnemonic_plant = "121368", descripcion = "Gigafactory Texas #1" },
                 new BG_IHS_plantas { mnemonic_plant = "120248", descripcion = "Fremont #1" },
                 new BG_IHS_plantas { mnemonic_plant = "121473", descripcion = "Fremont #2" },
                 new BG_IHS_plantas { mnemonic_plant = "120154", descripcion = "Chattanooga #1" } };

        private List<IHSRelRegionPlantaString> IHSRelRegionPlantaDefault = new List<IHSRelRegionPlantaString> {
            new IHSRelRegionPlantaString { planta = "120005", region = "CENTER" }, //aguascaliente a1
            new IHSRelRegionPlantaString { planta = "121032", region = "CENTER" }, //aguascalientes a2
            new IHSRelRegionPlantaString { planta = "120980", region = "CENTER" }, //Celaya
            new IHSRelRegionPlantaString { planta = "120203", region = "SOUTH" }, //Cuautitlan
            new IHSRelRegionPlantaString { planta = "120205", region = "SOUTH" }, //Cuernavaca #1
            new IHSRelRegionPlantaString { planta = "120206", region = "SOUTH" }, //Cuernavaca #2
            new IHSRelRegionPlantaString { planta = "120325", region = "NORTH-WEST" }, //Hermosillo
            new IHSRelRegionPlantaString { planta = "121116", region = "NORTH" }, //Monterrey
            new IHSRelRegionPlantaString { planta = "120602", region = "SOUTH" }, //Puebla #1
            new IHSRelRegionPlantaString { planta = "120621", region = "NORTH" }, //Ramos Arizpe #2
            new IHSRelRegionPlantaString { planta = "120481", region = "CENTER" }, //Salamanca
            new IHSRelRegionPlantaString { planta = "121085", region = "CENTER" }, //Guanajuato
            new IHSRelRegionPlantaString { planta = "120649", region = "NORTH" }, //Saltillo Truck
            new IHSRelRegionPlantaString { planta = "121081", region = "NORTH" }, //Saltillo Van
            new IHSRelRegionPlantaString { planta = "121046", region = "SOUTH" }, //San Jose Chiapa
            new IHSRelRegionPlantaString { planta = "121084", region = "CENTER" }, //San Luis Potosi
            new IHSRelRegionPlantaString { planta = "120661", region = "CENTER" }, //San Luis Potosi
            new IHSRelRegionPlantaString { planta = "120723", region = "CENTER" }, //Silao
            new IHSRelRegionPlantaString { planta = "120820", region = "NORTH-WEST" }, //Tijuana
            new IHSRelRegionPlantaString { planta = "120828", region = "SOUTH" }, //Toluca
            new IHSRelRegionPlantaString { planta = "121357", region = "SOUTH" }, //Sahagun
            new IHSRelRegionPlantaString { planta = "121146", region = "CENTER" }, //Aguascalientes
            new IHSRelRegionPlantaString { planta = "120751", region = "EXPORT" }, //Sterling Heights
            new IHSRelRegionPlantaString { planta = "121395", region = "EXPORT" }, //Fremont
            new IHSRelRegionPlantaString { planta = "120448", region = "EXPORT" }, //Louisville
            new IHSRelRegionPlantaString { planta = "120739", region = "EXPORT" }, //Spartanburg North
            new IHSRelRegionPlantaString { planta = "120874", region = "EXPORT" }, //Warren Truck
            new IHSRelRegionPlantaString { planta = "121136", region = "EXPORT" }, //Charleston
            new IHSRelRegionPlantaString { planta = "120153", region = "EXPORT" }, //Charleston
            new IHSRelRegionPlantaString { planta = "120843", region = "EXPORT" }, //Tuscaloosa #1
            new IHSRelRegionPlantaString { planta = "120551", region = "EXPORT" }, //Orion
            new IHSRelRegionPlantaString { planta = "120737", region = "EXPORT" }, //Spartanburg
            new IHSRelRegionPlantaString { planta = "121421", region = "EXPORT" }, //Spartanburg
            new IHSRelRegionPlantaString { planta = "120341", region = "EXPORT" }, //CAMI #2
            new IHSRelRegionPlantaString { planta = "121398", region = "EXPORT" }, //Gigafactory Texas #2
            new IHSRelRegionPlantaString { planta = "121368", region = "EXPORT" }, //Gigafactory Texas #1
            new IHSRelRegionPlantaString { planta = "120248", region = "EXPORT" }, //Fremont #1
            new IHSRelRegionPlantaString { planta = "121473", region = "EXPORT" }, //Fremont #2
            new IHSRelRegionPlantaString { planta = "120154", region = "EXPORT" }, //Chattanooga #1
            
        };

        private List<BG_IHS_segmentos> IHSSegmentosDefault = new List<BG_IHS_segmentos> {
            new BG_IHS_segmentos { global_production_segment="A-Segment",  flat_rolled_steel_usage = 650, blanks_usage_percent = 0.75m },
            new BG_IHS_segmentos { global_production_segment="B-Segment",  flat_rolled_steel_usage = 720, blanks_usage_percent = 0.75m },
            new BG_IHS_segmentos { global_production_segment="Compact Full-Frame",  flat_rolled_steel_usage = 800, blanks_usage_percent = 0.75m },
            new BG_IHS_segmentos { global_production_segment="C-Segment",  flat_rolled_steel_usage = 800, blanks_usage_percent = 0.75m },
            new BG_IHS_segmentos { global_production_segment="D-Segment",  flat_rolled_steel_usage = 850, blanks_usage_percent = 0.75m },
            new BG_IHS_segmentos { global_production_segment="E-Segment",  flat_rolled_steel_usage = 900, blanks_usage_percent = 0.75m },
            new BG_IHS_segmentos { global_production_segment="Full-Size Full-Frame",  flat_rolled_steel_usage = 1000, blanks_usage_percent = 0.75m },
        };

        // GET: BG_IHS_item
        public ActionResult ListadoIHS(string country_territory, string manufacturer, string production_plant, string mnemonic_vehicle_plant, string origen, int? version, string demanda = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 10; // parámetro

            var listadoBD = db.BG_IHS_item.Where(x => x.id_ihs_version == version);

            //verifica que el elemento este relacionado con el elmento anterior
            if (
                !listadoBD.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory) && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)) && x.manufacturer == manufacturer))
                manufacturer = String.Empty;

            if (
                !listadoBD.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory))
                                            && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                                            && x.production_plant == production_plant
                                            && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                                            ))
                production_plant = String.Empty;

            if (
                !listadoBD.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory))
                                            && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                                            && (x.production_plant == production_plant || String.IsNullOrEmpty(production_plant))
                                            && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                                            && mnemonic_vehicle_plant == x.mnemonic_vehicle_plant))
                mnemonic_vehicle_plant = String.Empty;

            var listado = listadoBD
                .Where(x =>
                    (x.country_territory == country_territory || String.IsNullOrEmpty(country_territory))
                    && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                    && (x.production_plant == production_plant || String.IsNullOrEmpty(production_plant))
                    && (x.mnemonic_vehicle_plant == mnemonic_vehicle_plant || String.IsNullOrEmpty(mnemonic_vehicle_plant))
                    && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                    && (x.id_ihs_version == version)
                    )
               .OrderBy(x => x.id)
               .Skip((pagina - 1) * cantidadRegistrosPorPagina)
              .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = listadoBD
                .Where(x =>
                    (x.country_territory == country_territory || String.IsNullOrEmpty(country_territory))
                    && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                    && (x.production_plant == production_plant || String.IsNullOrEmpty(production_plant))
                    && (x.mnemonic_vehicle_plant == mnemonic_vehicle_plant || String.IsNullOrEmpty(mnemonic_vehicle_plant))
                    && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                    )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["country_territory"] = country_territory;
            routeValues["manufacturer"] = manufacturer;
            routeValues["production_plant"] = production_plant;
            routeValues["mnemonic_vehicle_plant"] = mnemonic_vehicle_plant;
            routeValues["origen"] = origen;
            routeValues["demanda"] = demanda;
            routeValues["version"] = version;

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
            List<string> countriesList = listadoBD.Where(x => (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION) && !String.IsNullOrEmpty(x.country_territory)).Select(x => x.country_territory).Distinct().ToList();
            foreach (var itemList in countriesList)
                selectListCountry.Add(new SelectListItem() { Text = itemList, Value = itemList });
            ViewBag.country_territory = AddFirstItem(new SelectList(selectListCountry, "Value", "Text", country_territory), textoPorDefecto: "-- Todos --");

            // -- manufacturer --
            List<SelectListItem> selectListManufacturer = new List<SelectListItem>();
            List<string> manufacturerList = listadoBD.Where(x => (x.country_territory == country_territory || string.IsNullOrEmpty(country_territory)) && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION) && !String.IsNullOrEmpty(x.manufacturer)).Select(x => x.manufacturer).Distinct().ToList();
            foreach (var itemList in manufacturerList)
                selectListManufacturer.Add(new SelectListItem() { Text = itemList, Value = itemList });
            ViewBag.manufacturer = AddFirstItem(new SelectList(selectListManufacturer, "Value", "Text", manufacturer), textoPorDefecto: "-- Todos --");

            // -- production_plant --
            List<SelectListItem> selectListProduction_plant = new List<SelectListItem>();
            List<string> production_plantList = listadoBD.Where(x =>
                  (x.country_territory == country_territory || string.IsNullOrEmpty(country_territory))
               && (x.manufacturer == manufacturer || string.IsNullOrEmpty(manufacturer))
               && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                && !String.IsNullOrEmpty(x.production_plant)
                ).Select(x => x.production_plant).Distinct().ToList();
            foreach (var itemList in production_plantList)
                selectListProduction_plant.Add(new SelectListItem() { Text = itemList, Value = itemList });
            ViewBag.production_plant = AddFirstItem(new SelectList(selectListProduction_plant, "Value", "Text", production_plant), textoPorDefecto: "-- Todos --");

            // -- mnemonic_vehicle_plant --
            List<SelectListItem> selectListVehicle = new List<SelectListItem>();
            List<BG_IHS_item> vehicleList = listadoBD.Where(x =>
             (x.country_territory == country_territory || string.IsNullOrEmpty(country_territory))
               && (x.manufacturer == manufacturer || string.IsNullOrEmpty(manufacturer))
                && (x.production_plant == production_plant || string.IsNullOrEmpty(production_plant))
                && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                 && !String.IsNullOrEmpty(x.vehicle)
                ).ToList();
            foreach (var itemList in vehicleList)
                selectListVehicle.Add(new SelectListItem() { Text = itemList.ConcatCodigo, Value = itemList.mnemonic_vehicle_plant });
            ViewBag.mnemonic_vehicle_plant = AddFirstItem(new SelectList(selectListVehicle, "Value", "Text", mnemonic_vehicle_plant), textoPorDefecto: "-- Todos --");

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

            //obtiene la lista de regiones
            List<String> listRegiones = db.BG_IHS_regiones.Select(x => x.descripcion).Distinct().ToList();
            listRegiones.Add("SIN DEFINIR");
            ViewBag.ListRegiones = listRegiones;

            //Envia el titulo para la vista
            if (!String.IsNullOrEmpty(origen) && origen == Bitacoras.Util.BG_IHS_Origen.IHS)
                ViewBag.Title = "Listado de IHS (Archivo)";
            else if (!String.IsNullOrEmpty(origen) && origen == Bitacoras.Util.BG_IHS_Origen.USER)
                ViewBag.Title = "Listado de IHS (Usuario)";
            else if (!String.IsNullOrEmpty(origen) && origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                ViewBag.Title = "Listado de IHS (Unión)";
            else
                ViewBag.Title = "Listado de IHS";

            //envia objeto con la versión de IHS
            ViewBag.VersionIHS = db.BG_IHS_versiones.Find(version);

            return View(listado);
        }

        // GET: BG_IHS_item/ListadoVersionesIHS/
        public ActionResult ListadoVersionesIHS()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            return View(db.BG_IHS_versiones.Where(x => x.activo).OrderByDescending(x => x.periodo));

        }

        // GET: BG_IHS_item/CargaIHS/5
        public ActionResult CargaIHS(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //variables
            BG_IHS_versiones ihs_version = db.BG_IHS_versiones.Find(id);
            int? ID_version = null;
            string descripcion_version = string.Empty;
            DateTime periodo = new DateTime(2000, 01, 01);

            if (ihs_version != null)
            {
                ID_version = ihs_version.id;
                descripcion_version = ihs_version.nombre;
                periodo = ihs_version.periodo;
            }
            else
            {
                descripcion_version = DateTime.Now.ToString("MMMM yyyy").ToUpper();
            }

            ArchivoIHSViewModel model = new ArchivoIHSViewModel()
            {
                id = ID_version,
                nombre = descripcion_version,
                periodo = periodo,
                usarReporteBase = true,
            };

            ViewBag.idReporteBase = AddFirstItem(new SelectList(db.BG_IHS_versiones.Where(x => x.activo == true).OrderByDescending(x => x.id), nameof(BG_IHS_versiones.id), nameof(BG_IHS_versiones.ConcatVersion)));

            return View(model);

        }

        // POST: BG_IHS_item/CargaIHS/5
        [HttpPost]
        public ActionResult CargaIHS(ArchivoIHSViewModel viewModel, FormCollection collection)
        {

            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();
            Debug.Print(timeMeasure.Elapsed.Minutes + "m " + timeMeasure.Elapsed.Seconds % 60 + "s -> Inicia CargaIHS(): ");

            //if (viewModel.usarReporteBase && viewModel.idReporteBase == 0)
            //    ModelState.AddModelError(nameof(ArchivoIHSViewModel.idReporteBase), "El reporte base es requerido.");

            if (ModelState.IsValid)
            {
                string msjError = "No se ha podido leer el archivo seleccionado.";

                //lee el archivo seleccionado
                try
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    HttpPostedFileBase stream = Request.Files["PostedFile"];


                    if (stream.InputStream.Length > 5242880)
                    {
                        msjError = "Sólo se permiten archivos con peso menor a 5 MB.";
                        throw new Exception(msjError);
                    }
                    else
                    {
                        string extension = Path.GetExtension(viewModel.PostedFile.FileName);
                        if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                        {
                            msjError = "Sólo se permiten archivos Excel";
                            throw new Exception(msjError);
                        }
                    }

                    bool estructuraValida = false;

                    Debug.Print(timeMeasure.Elapsed.Minutes + "m " + timeMeasure.Elapsed.Seconds % 60 + "s -> Comienza UtilExcel.LeeIHS(): ");

                    //lee y convierte periodo

                    ////verifica si ya existe un elemento con la misma descripción y periodo
                    //viewModel.nombre = viewModel.nombre.ToUpper();
                    //if (db.BG_IHS_versiones.Any(x => x.periodo == viewModel.periodo && x.activo))
                    //    ModelState.AddModelError("", "Ya existe un registro para el periodo indicado: .");

                    DateTime periodo = new DateTime(2000, 01, 01); //valor del periodo por defecto

                    //el archivo es válido
                    List<BG_IHS_item> lista = UtilExcel.LeeIHS(viewModel.PostedFile, ref estructuraValida, ref msjError, ref periodo);

                    //verifica que no exista otro IHS con la misma fecha
                    if (db.BG_IHS_versiones.Any(x => x.periodo == periodo && x.activo && (!viewModel.id.HasValue || x.id != viewModel.id)))
                    {
                        msjError = "Ya existe un registro con el ForecastRelease: " + periodo.ToString("MMMM yyyy");
                        throw new Exception("Ya existe un registro con el ForecastRelease: " + periodo.ToLongDateString());
                    }


                    Debug.Print(timeMeasure.Elapsed.Minutes + "m " + timeMeasure.Elapsed.Seconds % 60 + "s -> Termina UtilExcel.LeeIHS(): ");

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
                        List<BG_IHS_item> listAnteriorBG = db.BG_IHS_item.Where(x => x.id_ihs_version == viewModel.idReporteBase).ToList();
                        //obtiene el listado actual de la BD rels demanda
                        List<BG_IHS_rel_demanda> listAnteriorRelDemanda = db.BG_IHS_rel_demanda.Where(x => x.BG_IHS_item.id_ihs_version == viewModel.idReporteBase).ToList();
                        //obtiene el listado actual de la BD rels cuartos
                        List<BG_IHS_rel_cuartos> listAnteriorRelCuartos = db.BG_IHS_rel_cuartos.Where(x => x.BG_IHS_item.id_ihs_version == viewModel.idReporteBase).ToList();
                        //obtiene el listado de combinaciones asociados a la versión anterior
                        List<BG_IHS_combinacion> listAnteriorCombinaciones = db.BG_IHS_combinacion.Where(x => x.id_ihs_version == viewModel.idReporteBase).ToList();
                        //obtiene el listado de divisiones asociadas a la versión anterior
                        List<BG_IHS_division> listAnteriorDivisiones = db.BG_IHS_division.Where(x => x.id_ihs_version == viewModel.idReporteBase).ToList();
                        //obtiene el listado de regiones asociadas a la versión anterior
                        List<BG_IHS_regiones> listAnteriorRegiones = db.BG_IHS_regiones.Where(x => x.id_ihs_version == viewModel.idReporteBase).ToList();
                        //obtiene el listado de plantas asociadas a la versión anterior
                        List<BG_IHS_plantas> listAnteriorPlantas = db.BG_IHS_plantas.Where(x => x.id_ihs_version == viewModel.idReporteBase).ToList();
                        //obtiene el listado del rel regiones-plantas asociadas a la versión anterior
                        List<BG_IHS_rel_regiones> listAnteriorRelRegionesPlantas = db.BG_IHS_rel_regiones.Where(x => x.id_ihs_version == viewModel.idReporteBase).ToList();
                        //obtiene el listado para los segementos asociados a la versión anterior
                        List<BG_IHS_segmentos> listAnteriorSegmentos = db.BG_IHS_segmentos.Where(x => x.id_ihs_version == viewModel.idReporteBase).ToList();


                        //listado de segmentos
                        var listRelSegmentosBD = db.BG_IHS_segmentos.Select(x => x.global_production_segment).ToList();

                        //obtiene el listado de diferencias
                        //List<BG_IHS_item> listDiferencias = lista.Except(listAnterior).ToList();


                        Debug.Print(timeMeasure.Elapsed.Minutes + "m " + timeMeasure.Elapsed.Seconds % 60 + "s -> Inicia Procesado : " + lista.Count);

                        //crea nueva version de IHS
                        BG_IHS_versiones versionIHS = new BG_IHS_versiones
                        {
                            periodo = periodo,
                            nombre = viewModel.nombre,
                            activo = true,
                        };


                        //crea los que no estén en IHS anterior
                        foreach (var anterior in listAnteriorBG.Where(x => !lista.Any(y => y.mnemonic_vehicle_plant == x.mnemonic_vehicle_plant)))
                        {
                            //crea un nuevo registro, con todo y la demanda asociada (cuartos y cantidad)

                            var bdNew = new BG_IHS_item { };

                            bdNew.Update(anterior);
                            bdNew.UpdateCuartos(anterior);
                            bdNew.UpdateDemanda(anterior);
                            bdNew.id_ihs_version = versionIHS.id;

                            versionIHS.BG_IHS_item.Add(bdNew);
                            creados++;
                        }

                        //si no utiliza reporte, toma los valores por defecto
                        if (!viewModel.usarReporteBase)
                        {
                            //agrega plantas por defecto
                            foreach (var pt in IHSPlantasDefault)
                            {
                                versionIHS.BG_IHS_plantas.Add(new BG_IHS_plantas
                                {
                                    mnemonic_plant = pt.mnemonic_plant,
                                    descripcion = pt.descripcion,
                                    activo = true,
                                });
                            }
                            //agrega regiones por defecto
                            foreach (var reg in IHSRegionesDefault)
                            {
                                versionIHS.BG_IHS_regiones.Add(new BG_IHS_regiones
                                {
                                    descripcion = reg,
                                    activo = true,
                                });
                            }
                            //agrega rels por defecto
                            foreach (var relRP in IHSRelRegionPlantaDefault)
                            {
                                versionIHS.BG_IHS_rel_regiones.Add(new BG_IHS_rel_regiones
                                {
                                    BG_IHS_plantas = versionIHS.BG_IHS_plantas.FirstOrDefault(x => x.mnemonic_plant == relRP.planta), //Planta = mnemonic_plant
                                    BG_IHS_regiones = versionIHS.BG_IHS_regiones.FirstOrDefault(x => x.descripcion == relRP.region),
                                });
                            }
                            //agrega segementos por defecto
                            foreach (var seg in IHSSegmentosDefault)
                            {
                                versionIHS.BG_IHS_segmentos.Add(new BG_IHS_segmentos
                                {
                                    global_production_segment = seg.global_production_segment,
                                    flat_rolled_steel_usage = seg.flat_rolled_steel_usage,
                                    blanks_usage_percent = seg.blanks_usage_percent,
                                });
                            }

                        }
                        else //si tiene reporte base
                        {
                            //copia las plantas del reporte base a la nueva version
                            foreach (var reg in listAnteriorPlantas)
                            {
                                versionIHS.BG_IHS_plantas.Add(new BG_IHS_plantas
                                {
                                    mnemonic_plant = reg.mnemonic_plant,
                                    descripcion = reg.descripcion,
                                    activo = reg.activo,
                                });
                            }
                            //copia las regiones del reporte base a la nueva version
                            foreach (var reg in listAnteriorRegiones)
                            {
                                versionIHS.BG_IHS_regiones.Add(new BG_IHS_regiones
                                {
                                    descripcion = reg.descripcion,
                                    activo = reg.activo,
                                });
                            }
                            //copia las rel regiones-plantas del reporte base a la nueva version
                            foreach (var relRP in listAnteriorRelRegionesPlantas)
                            {
                                versionIHS.BG_IHS_rel_regiones.Add(new BG_IHS_rel_regiones
                                {
                                    //cambiar por un número clave
                                    BG_IHS_plantas = versionIHS.BG_IHS_plantas.FirstOrDefault(x => x.mnemonic_plant == relRP.BG_IHS_plantas.mnemonic_plant),
                                    BG_IHS_regiones = relRP.BG_IHS_regiones != null ? versionIHS.BG_IHS_regiones.FirstOrDefault(x => x.descripcion == relRP.BG_IHS_regiones.descripcion) : null,
                                });
                            }

                            //copia los segmentos del reporte base a la nueva version
                            foreach (var seg in listAnteriorSegmentos)
                            {
                                versionIHS.BG_IHS_segmentos.Add(new BG_IHS_segmentos
                                {
                                    global_production_segment = seg.global_production_segment,
                                    flat_rolled_steel_usage = seg.flat_rolled_steel_usage,
                                    blanks_usage_percent = seg.blanks_usage_percent,
                                });
                            }
                        }


                        int i = 0;
                        //trata los de la lista
                        foreach (BG_IHS_item ihs in lista)
                        {
                            Debug.Print(timeMeasure.Elapsed.Minutes + "m " + timeMeasure.Elapsed.Seconds % 60 + "s -> Procesando: " + (++i) + "/" + lista.Count);

                            try
                            {
                                //obtiene el elemento de BD, en base al primary key
                                BG_IHS_item item = listAnteriorBG.FirstOrDefault(x => x.mnemonic_vehicle_plant == ihs.mnemonic_vehicle_plant);

                                //crea, modifica, lo elementos del IHS
                                //si es diferente a BD lo actualiza antes de agregar
                                if (item != null && !ihs.Equals(item))
                                {
                                    //modifica los valores que pudieron haber cambiado
                                    ihs.Update(item);
                                    actualizados++;

                                    //tanto si es diferente o es igual actualiza los rel demanda
                                    //recorre todos los items de la demanda
                                    foreach (var rel in ihs.BG_IHS_rel_demanda)
                                    {
                                        //busca el divBD en bd
                                        BG_IHS_rel_demanda itemRel = listAnteriorRelDemanda.FirstOrDefault(x => x.id_ihs_item == item.id && x.fecha == rel.fecha);

                                        if (itemRel != null) //existe
                                        {
                                            //actualiza
                                            if (itemRel.cantidad != rel.cantidad)
                                            {
                                                itemRel.cantidad = rel.cantidad;
                                                //rel.id = itemRel.id;
                                                //rel.id_ihs_item = divBD.id;
                                                //db.Entry(itemRel).CurrentValues.SetValues(rel);
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

                                    //tanto si es diferente o es igual actualiza los rel CUARTOS
                                    //recorre todos los items de cuartos
                                    foreach (var rel in ihs.BG_IHS_rel_cuartos)
                                    {
                                        //busca el divBD en bd
                                        BG_IHS_rel_cuartos itemRel = listAnteriorRelCuartos.FirstOrDefault(x => x.id_ihs_item == item.id && x.cuarto == rel.cuarto && x.anio == rel.anio);

                                        if (itemRel != null) //existe
                                        {
                                            //actualiza
                                            if (itemRel.cantidad != rel.cantidad)
                                            {
                                                itemRel.cantidad = rel.cantidad;
                                                //rel.id = itemRel.id;
                                                //rel.id_ihs_item = divBD.id;
                                                //db.Entry(itemRel).CurrentValues.SetValues(rel);
                                                quarterAct++;
                                            }

                                        }
                                        else
                                        { //lo crea en la BD
                                            rel.id_ihs_item = item.id;
                                            db.BG_IHS_rel_cuartos.Add(rel);
                                            quarterCreate++;
                                        }
                                    }
                                }

                                /* DEBE BUSCAR SEGMENTOS, REGIONES Y PLANTAS AUN Y CUANDO NO SE HAYA SELECCIONADO REPORTE BASE */

                                ///primero busca planta si no la agrega al catalogo
                                if (!versionIHS.BG_IHS_plantas.Any(x => x.mnemonic_plant == ihs.mnemonic_plant))
                                    versionIHS.BG_IHS_plantas.Add(new BG_IHS_plantas
                                    {
                                        mnemonic_plant = ihs.mnemonic_plant,
                                        descripcion = ihs.production_plant,
                                        activo = true,
                                    });
                                ///dos. busca rel para la plantad , si no la agrega al catálogo
                                if (!versionIHS.BG_IHS_rel_regiones.Any(x => x.BG_IHS_plantas.mnemonic_plant == ihs.mnemonic_plant))
                                {
                                    var refPlant = versionIHS.BG_IHS_plantas.FirstOrDefault(x => x.mnemonic_plant == ihs.mnemonic_plant); //Planta = mnemonic_plant
                                    versionIHS.BG_IHS_rel_regiones.Add(new BG_IHS_rel_regiones
                                    {
                                        BG_IHS_plantas = refPlant,
                                    });
                                }
                                //busca si no existe un segemento para el global segment actual
                                if (!versionIHS.BG_IHS_segmentos.Any(x => x.global_production_segment == ihs.global_production_segment))
                                {
                                    versionIHS.BG_IHS_segmentos.Add(new BG_IHS_segmentos
                                    {
                                        global_production_segment = ihs.global_production_segment,
                                        blanks_usage_percent = 0,
                                    });
                                }

                                //crea un nuevo registro, con todo y la demanda asociada (cuartos y cantidad)
                                versionIHS.BG_IHS_item.Add(ihs);
                                creados++;

                            }
                            catch (Exception e)
                            {
                                error++;
                            }
                        }

                        //agrega combinaciones
                        foreach (var cb in listAnteriorCombinaciones)
                        {
                            var comb = new BG_IHS_combinacion
                            {
                                vehicle = cb.vehicle,
                                production_brand = cb.production_brand,
                                production_plant = cb.production_plant,
                                manufacturer_group = cb.manufacturer_group,
                                sop_start_of_production = cb.sop_start_of_production,
                                eop_end_of_production = cb.eop_end_of_production,
                                comentario = cb.comentario,
                                porcentaje_scrap = cb.porcentaje_scrap,
                                activo = cb.activo,
                            };

                            //agrega los rel de combinación
                            foreach (var rel in cb.BG_IHS_rel_combinacion)
                            {
                                comb.BG_IHS_rel_combinacion.Add(new BG_IHS_rel_combinacion
                                {
                                    BG_IHS_item = versionIHS.BG_IHS_item.FirstOrDefault(x => x.mnemonic_vehicle_plant == rel.BG_IHS_item.mnemonic_vehicle_plant)
                                });
                            }

                            versionIHS.BG_IHS_combinacion.Add(comb);
                        }

                        //agrega divisiones
                        foreach (var divBD in listAnteriorDivisiones)
                        {
                            //crea una copia de la división original
                            var div = new BG_IHS_division
                            {
                                BG_IHS_item = versionIHS.BG_IHS_item.FirstOrDefault(x => x.mnemonic_vehicle_plant == divBD.BG_IHS_item.mnemonic_vehicle_plant),
                                comentario = divBD.comentario,
                                porcentaje_scrap = divBD.porcentaje_scrap,
                                activo = divBD.activo
                            };

                            //crea los rel de cada division
                            foreach (var relBD in divBD.BG_IHS_rel_division)
                            {
                                div.BG_IHS_rel_division.Add(new BG_IHS_rel_division
                                {
                                    vehicle = relBD.vehicle,
                                    production_nameplate = relBD.production_nameplate,
                                    porcentaje = relBD.porcentaje,
                                    activo = relBD.activo,
                                });
                            }
                            versionIHS.BG_IHS_division.Add(div);
                        }

                        //guarda IHS en BD
                        try
                        {

                           
                            db.BG_IHS_versiones.Add(versionIHS);

                            //elimina los registros asociados
                            if (viewModel.id.HasValue)
                            {

                                var oldIHS = db.BG_IHS_versiones
                                    //.Include(x => x.BG_Forecast_reporte)
                                    //.Include(x => x.BG_IHS_combinacion)
                                    //.Include(x => x.BG_IHS_division)
                                    //.Include(x => x.BG_IHS_item)
                                    //.Include(x => x.BG_IHS_plantas)
                                    //.Include(x => x.BG_IHS_regiones)
                                    //.Include(x => x.BG_IHS_rel_regiones)
                                    //.Include(x => x.BG_IHS_segmentos)
                                    .FirstOrDefault(x => x.id == viewModel.id);

                                //desactiva el anterior
                                oldIHS.activo = false;

                                //db.BG_IHS_versiones.Remove(oldIHS);
                                //viewModel.id = null;
                            }
                            db.Configuration.ValidateOnSaveEnabled = false; // Deshabilita la validación

                            db.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                            TempData["Mensaje"] = new MensajesSweetAlert("Error: " + ex.Message, TipoMensajesSweetAlerts.ERROR);
                            return RedirectToAction("ListadoVersionesIHS");
                        }
                        finally
                        {
                            db.Configuration.AutoDetectChangesEnabled = true;
                            db.Configuration.ValidateOnSaveEnabled = true; // Habilita la validación nuevamente

                        }

                        Debug.Print(timeMeasure.Elapsed.Minutes + "m " + timeMeasure.Elapsed.Seconds % 60 + "s -> Procesando Terminado: " + (++i) + "/" + lista.Count);

                        TempData["Mensaje"] = new MensajesSweetAlert("Actualizados: " + actualizados + "; Creados: " + creados
                            + "; DemandasActualizadas: " + demandaAct + "; DemandasCrea: " + demandaCreate
                            + "; CuartosActualizados: " + quarterAct + "; CuartosCreados: " + quarterCreate + "; Errores: " + error, TipoMensajesSweetAlerts.INFO);
                        return RedirectToAction("ListadoIHS", new { version = versionIHS.id, origen = Bitacoras.Util.BG_IHS_Origen.UNION });
                    }

                }
                catch (Exception e)
                {
                    ViewBag.idReporteBase = AddFirstItem(new SelectList(db.BG_IHS_versiones.Where(x => x.activo == true), nameof(BG_IHS_versiones.id), nameof(BG_IHS_versiones.nombre)));
                    ModelState.AddModelError("", msjError);

                    return View(viewModel);
                }
                finally
                {
                    db.Configuration.AutoDetectChangesEnabled = true;
                }

            }
            ViewBag.idReporteBase = AddFirstItem(new SelectList(db.BG_IHS_versiones.Where(x => x.activo == true).OrderByDescending(x => x.id), nameof(BG_IHS_versiones.id), nameof(BG_IHS_versiones.ConcatVersion)));

            return View(viewModel);
        }

        // GET: BG_IHS_item/carga_masiva_ihs/5
        public ActionResult CargaMasiva(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //variables
            BG_IHS_versiones ihs_version = db.BG_IHS_versiones.Find(id);
            int? ID_version = null;
            string descripcion_version = string.Empty;
            DateTime periodo = new DateTime(2000, 01, 01);

            //mensaje en caso de crear, editar, etc
            if (TempData["MensajeDetalle"] != null)
                ViewBag.MensajeDetalle = TempData["MensajeDetalle"];

            if (ihs_version != null)
            {
                ID_version = ihs_version.id;
                descripcion_version = ihs_version.nombre;
                periodo = ihs_version.periodo;
            }
            else
            {
                descripcion_version = DateTime.Now.ToString("MMMM yyyy").ToUpper();
            }

            ArchivoIHSViewModel model = new ArchivoIHSViewModel()
            {
                id = ID_version,
                nombre = descripcion_version,
                periodo = periodo,
                usarReporteBase = true,
            };

            ViewBag.idReporteBase = AddFirstItem(new SelectList(db.BG_IHS_versiones.Where(x => x.activo == true).OrderByDescending(x => x.id), nameof(BG_IHS_versiones.id), nameof(BG_IHS_versiones.ConcatVersion)));

            return View(model);

        }

        [HttpPost]
        public ActionResult CargaMasiva(HttpPostedFileBase[] files)
        {
            string mensajeFinal = string.Empty;
            //Ensure model state is valid  
            if (ModelState.IsValid)
            {   //iterating through multiple file collection   
                foreach (HttpPostedFileBase file in files)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        HttpPostedFileBase stream = file;


                        if (stream.InputStream.Length > 8388608)
                        {
                            mensajeFinal += "(" + stream.FileName + "): Sólo se permiten archivos con peso menor a 8 MB. <br />";
                            continue;
                        }
                        else
                        {
                            string extension = Path.GetExtension(stream.FileName);
                            if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                            {
                                mensajeFinal += "(" + stream.FileName + "): Sólo se permiten archivos con extensión .xlsx <br />";
                                continue;
                            }
                        }
                        bool estructuraValida = false;
                        string msjError = string.Empty;
                        DateTime periodo = new DateTime(2000, 01, 01); //valor del periodo por defecto
                        //el archivo es válido
                        List<BG_IHS_item> lista = UtilExcel.LeeIHS(file, ref estructuraValida, ref msjError, ref periodo);

                        if (!string.IsNullOrEmpty(msjError) && !estructuraValida)
                            mensajeFinal += stream.FileName + ": " + msjError;

                        //verifica que no exista otro IHS con la misma fecha
                        if (db.BG_IHS_versiones.Any(x => x.periodo == periodo))
                        {
                            msjError = "Ya existe un registro con el ForecastRelease: " + periodo.ToString("MMMM yyyy");
                            mensajeFinal += "(" + stream.FileName + "): " + msjError + "<br />";
                            estructuraValida = false;
                        }

                        if (!estructuraValida)
                        {
                            // msjError = "No cumple con la estructura válida.";
                            mensajeFinal += "(" + stream.FileName + "): Estructura no válida <br />";
                        }
                        else
                        {
                            int creados = 0;
                            int error = 0;

                            //listado de segmentos
                            var listRelSegmentosBD = db.BG_IHS_segmentos.Select(x => x.global_production_segment).ToList();

                            //obtiene el listado de diferencias
                            //List<BG_IHS_item> listDiferencias = lista.Except(listAnterior).ToList();


                            //crea nueva version de IHS
                            BG_IHS_versiones versionIHS = new BG_IHS_versiones
                            {
                                periodo = periodo,
                                nombre = periodo.ToString("MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX")),
                                activo = true,
                            };


                            //si no utiliza reporte, toma los valores por defecto

                            //agrega plantas por defecto
                            foreach (var pt in IHSPlantasDefault)
                            {
                                versionIHS.BG_IHS_plantas.Add(new BG_IHS_plantas
                                {
                                    mnemonic_plant = pt.mnemonic_plant,
                                    descripcion = pt.descripcion,
                                    activo = true,
                                });
                            }
                            //agrega regiones por defecto
                            foreach (var reg in IHSRegionesDefault)
                            {
                                versionIHS.BG_IHS_regiones.Add(new BG_IHS_regiones
                                {
                                    descripcion = reg,
                                    activo = true,
                                });
                            }
                            //agrega rels por defecto
                            foreach (var relRP in IHSRelRegionPlantaDefault)
                            {
                                versionIHS.BG_IHS_rel_regiones.Add(new BG_IHS_rel_regiones
                                {
                                    BG_IHS_plantas = versionIHS.BG_IHS_plantas.FirstOrDefault(x => x.mnemonic_plant == relRP.planta), //Planta = mnemonic_plant
                                    BG_IHS_regiones = versionIHS.BG_IHS_regiones.FirstOrDefault(x => x.descripcion == relRP.region),
                                });
                            }
                            //agrega segementos por defecto
                            foreach (var seg in IHSSegmentosDefault)
                            {
                                versionIHS.BG_IHS_segmentos.Add(new BG_IHS_segmentos
                                {
                                    global_production_segment = seg.global_production_segment,
                                    flat_rolled_steel_usage = seg.flat_rolled_steel_usage,
                                    blanks_usage_percent = seg.blanks_usage_percent,
                                });
                            }


                            int i = 0;
                            //trata los de la lista
                            foreach (BG_IHS_item ihs in lista)
                            {
                                try
                                {

                                    /* DEBE BUSCAR SEGMENTOS, REGIONES Y PLANTAS AUN Y CUANDO NO SE HAYA SELECCIONADO REPORTE BASE */

                                    ///primero busca planta si no la agrega al catalogo
                                    if (!versionIHS.BG_IHS_plantas.Any(x => x.mnemonic_plant == ihs.mnemonic_plant))
                                        versionIHS.BG_IHS_plantas.Add(new BG_IHS_plantas
                                        {
                                            mnemonic_plant = ihs.mnemonic_plant,
                                            descripcion = ihs.production_plant,
                                            activo = true,
                                        });
                                    ///dos. busca rel para la plantad , si no la agrega al catálogo
                                    if (!versionIHS.BG_IHS_rel_regiones.Any(x => x.BG_IHS_plantas.mnemonic_plant == ihs.mnemonic_plant))
                                    {
                                        var refPlant = versionIHS.BG_IHS_plantas.FirstOrDefault(x => x.mnemonic_plant == ihs.mnemonic_plant); //Planta = mnemonic_plant
                                        versionIHS.BG_IHS_rel_regiones.Add(new BG_IHS_rel_regiones
                                        {
                                            BG_IHS_plantas = refPlant,
                                        });
                                    }
                                    //busca si no existe un segemento para el global segment actual
                                    if (!versionIHS.BG_IHS_segmentos.Any(x => x.global_production_segment == ihs.global_production_segment))
                                    {
                                        versionIHS.BG_IHS_segmentos.Add(new BG_IHS_segmentos
                                        {
                                            global_production_segment = ihs.global_production_segment,
                                            blanks_usage_percent = 0,
                                        });
                                    }

                                    //crea un nuevo registro, con todo y la demanda asociada (cuartos y cantidad)
                                    versionIHS.BG_IHS_item.Add(ihs);
                                    creados++;

                                }
                                catch (Exception e)
                                {
                                    error++;
                                }
                            }

                            //guarda IHS en BD
                            try
                            {
                                db.BG_IHS_versiones.Add(versionIHS);

                                db.SaveChanges();

                                mensajeFinal += "(" + stream.FileName + "): correcto. <br />";

                            }
                            catch (Exception ex)
                            {
                                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + ex.Message, TipoMensajesSweetAlerts.ERROR);
                                return RedirectToAction("ListadoVersionesIHS");
                            }
                            finally
                            {
                                db.Configuration.AutoDetectChangesEnabled = true;
                            }


                            //return RedirectToAction("CargaMasiva");
                        }

                    }

                }
            }

            TempData["MensajeDetalle"] = mensajeFinal;
            // TempData["Detalles"] = ;
            return RedirectToAction("CargaMasiva");
        }

        // GET: BG_IHS_item/CargaMasivaCliente/5
        public ActionResult CargaMasivaCliente()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //variables
            //   BG_IHS_versiones ihs_version = db.BG_IHS_versiones.Find(id);
            int? ID_version = null;
            string descripcion_version = string.Empty;
            DateTime periodo = new DateTime(2000, 01, 01);

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //if (ihs_version != null)
            //{
            //    ID_version = ihs_version.id;
            //    descripcion_version = ihs_version.nombre;
            //    periodo = ihs_version.periodo;
            //}
            //else
            //{
            //    descripcion_version = DateTime.Now.ToString("MMMM yyyy").ToUpper();
            //}

            ArchivoIHSViewModel model = new ArchivoIHSViewModel()
            {
                id = ID_version,
                nombre = descripcion_version,
                periodo = periodo,
                usarReporteBase = true,
            };

            //ViewBag.idReporteBase = AddFirstItem(new SelectList(db.BG_IHS_versiones.Where(x => x.activo == true).OrderByDescending(x => x.id), nameof(BG_IHS_versiones.id), nameof(BG_IHS_versiones.ConcatVersion)));

            return View(model);

        }

        [HttpPost]
        public ActionResult CargaMasivaCliente(HttpPostedFileBase[] files)
        {
            string mensajeFinal = string.Empty;
            //Ensure model state is valid  
            if (ModelState.IsValid)
            {   //iterating through multiple file collection   
                foreach (HttpPostedFileBase file in files)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        HttpPostedFileBase stream = file;

                        if (stream.InputStream.Length > 8388608)
                        {
                            mensajeFinal += "(" + stream.FileName + "): Sólo se permiten archivos con peso menor a 8 MB. <br />";
                            continue;
                        }
                        else
                        {
                            string extension = Path.GetExtension(stream.FileName);
                            if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                            {
                                mensajeFinal += "(" + stream.FileName + "): Sólo se permiten archivos con extensión .xlsx <br />";
                                continue;

                            }
                        }
                        bool estructuraValida = false;
                        string msjError = string.Empty;

                        //el archivo es válido
                        List<BG_IHS_rel_demanda> demandaList = UtilExcel.LeePlantillaDemanda(file, ref estructuraValida, ref msjError);
                        List<int> listaIds = demandaList.Select(x => x.id_ihs_item).Distinct().ToList();
                        //quita los repetidos
                        demandaList = demandaList.Distinct().ToList();

                        if (!string.IsNullOrEmpty(msjError) && !estructuraValida)
                        {
                            mensajeFinal += stream.FileName + ": " + msjError;
                            TempData["Mensaje"] = new MensajesSweetAlert(mensajeFinal, TipoMensajesSweetAlerts.WARNING);
                            // TempData["Detalles"] = ;
                            return RedirectToAction("CargaMasivaCliente");
                        }

                        //verifica que exista el IHS para el mes deseado
                        //if (db.BG_IHS_versiones.Any(x => x.periodo == periodo))
                        //{
                        //    msjError = "Ya existe un registro con el ForecastRelease: " + periodo.ToString("MMMM yyyy") + "<br />";
                        //    mensajeFinal += "(" + stream.FileName + "): " + msjError + "<br />";
                        //}

                        if (!estructuraValida)
                        {
                            // msjError = "No cumple con la estructura válida.";
                            mensajeFinal += "(" + stream.FileName + "): Estructura no válida <br />";
                        }
                        else
                        {
                            int actualizados = 0;
                            int creados = 0;

                            // identificar cuales son update, create, delete y sin cambios

                            //obtiene los valores actuales en BD
                            List<BG_IHS_rel_demanda> valoresEnBD = db.BG_IHS_rel_demanda.Where(x => listaIds.Contains(x.id_ihs_item)).ToList();

                            foreach (var item in demandaList)
                            {

                                // if (demandaList.IndexOf(item) % 50 == 0)
                                //    System.Diagnostics.Debug.WriteLine("Create: " + demandaList.IndexOf(item) + "/" + demandaList.Count);

                                //obtiene el valor de la demanda de la BD
                                var demandaCliente = valoresEnBD.FirstOrDefault(x => x.fecha == item.fecha && x.id_ihs_item == item.id_ihs_item && x.tipo == item.tipo);

                                if (demandaCliente == null)
                                {
                                    //busca una demanda original
                                    var demandaOriginal = valoresEnBD.FirstOrDefault(x => x.fecha == item.fecha && x.id_ihs_item == item.id_ihs_item && x.tipo == "ORIGINAL");

                                    if ((demandaOriginal == null && item.cantidad > 0) || (demandaOriginal != null && demandaOriginal.cantidad != item.cantidad))
                                    {
                                        db.BG_IHS_rel_demanda.Add(item);

                                        try
                                        {
                                            db.SaveChanges();
                                            creados++;

                                        }
                                        catch (Exception ex)
                                        {
                                            System.Diagnostics.Debug.WriteLine(ex.Message);
                                        }
                                    }

                                }
                                else
                                { //existe
                                    //actualiza
                                    if (demandaCliente.cantidad != item.cantidad)
                                    {
                                        demandaCliente.cantidad = item.cantidad;
                                        try
                                        {
                                            db.SaveChanges();
                                            actualizados++;
                                        }
                                        catch (Exception ex)
                                        {
                                            System.Diagnostics.Debug.WriteLine(ex.Message);
                                        }
                                    }
                                }
                            }


                            //return RedirectToAction("CargaMasiva");
                        }

                    }
                    else
                    {
                        TempData["Mensaje"] = new MensajesSweetAlert("No se detectó archivo.", TipoMensajesSweetAlerts.WARNING);
                        // TempData["Detalles"] = ;
                        return RedirectToAction("CargaMasivaCliente");
                    }

                }
            }

            TempData["Mensaje"] = new MensajesSweetAlert("Se proceso el archivo.", TipoMensajesSweetAlerts.SUCCESS);
            // TempData["Detalles"] = ;
            return RedirectToAction("CargaMasivaCliente");
        }



        // GET: BG_IHS_item/EditIHS/5
        public ActionResult EditIHS(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            var version = db.BG_IHS_versiones.Find(id);

            return View(version);

        }

        /// <summary>
        /// Descarga la plantilla de ejemplo
        /// </summary>
        /// <returns></returns>
        public ActionResult FormatoPlantillaCargaMasiva()
        {
            String ruta = System.Web.HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_historicos_req_cliente.xlsx");

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

        // POST: BG_IHS_item/EditIHS/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditIHS(BG_IHS_versiones model)
        {
            //verifica si ya existe un elemento con la misma descripción y periodo         
            if (db.BG_IHS_versiones.Any(x => x.nombre == model.nombre && x.periodo == model.periodo && x.id != model.id && x.activo))
                ModelState.AddModelError("", "Ya existe un registro con el mismo nombre y periodo en la Base de Datos.");

            if (ModelState.IsValid)
            {
                try
                {
                    //obtiene el valor actual de la BD
                    var version = db.BG_IHS_versiones.Find(model.id);
                    //cambia los valores de IHS
                    version.periodo = model.periodo;
                    version.nombre = model.nombre;
                    version.activo = model.activo;
                    //guarda los valores en BD
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("ListadoVersionesIHS");
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert(e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("ListadoVersionesIHS");
                }
            }

            return View(model);
        }


        // GET: BG_IHS_item/CargaDemandaCliente/5
        public ActionResult CargaDemandaCliente(string country_territory, string manufacturer, string production_plant, string mnemonic_vehicle_plant, string origen, int? version, string demanda = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            var listadoBD = db.BG_IHS_item.Where(x => x.id_ihs_version == version);

            //verifica que el elemento este relacionado con el elmento anterior
            if (
                !listadoBD.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory) && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)) && x.manufacturer == manufacturer))
                manufacturer = String.Empty;

            if (
                !listadoBD.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory))
                                            && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                                            && x.production_plant == production_plant
                                            && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                                            ))
                production_plant = String.Empty;

            if (
                !listadoBD.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory))
                                            && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                                            && (x.production_plant == production_plant || String.IsNullOrEmpty(production_plant))
                                            && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                                            && mnemonic_vehicle_plant == x.mnemonic_vehicle_plant))
                mnemonic_vehicle_plant = String.Empty;

            //obtiene el menor año de los archivos cargados de ihs
            int anoInicio = 2019, anoFin = 2030;
            try
            {
                anoInicio = db.BG_IHS_rel_demanda.OrderBy(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
                anoFin = db.BG_IHS_rel_demanda.OrderByDescending(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
            }
            catch (Exception) { /* do nothing */ }


            var listado = listadoBD
                .Where(x =>
                    (x.country_territory == country_territory || String.IsNullOrEmpty(country_territory))
                    && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                    && (x.production_plant == production_plant || String.IsNullOrEmpty(production_plant))
                    && (x.mnemonic_vehicle_plant == mnemonic_vehicle_plant || String.IsNullOrEmpty(mnemonic_vehicle_plant))
                    && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                    ).ToList();

            List<BG_IHS_rel_demanda> listDemanda = new List<BG_IHS_rel_demanda>();

            foreach (var ihs in listado)
            {
                //crea un rel para cada año existente en la bd
                for (int i = anoInicio; i <= anoFin; i++)
                {
                    for (int j = 1; j <= 12; j++)
                    {
                        DateTime fecha = new DateTime(i, j, 1);

                        var dem = ihs.BG_IHS_rel_demanda.Where(x => x.fecha == fecha && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER).FirstOrDefault();
                        //si no existe un campo con los mismos valores lo agrega
                        if (dem == null)
                        {
                            listDemanda.Add(new BG_IHS_rel_demanda
                            {
                                // cantidad = bG_IHS_item.BG_IHS_rel_demanda.Where(x => x.fecha == fecha && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL).Select(x=> x.cantidad).FirstOrDefault(),
                                id = 0,
                                id_ihs_item = ihs.id,
                                fecha = fecha,
                                tipo = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER
                            });
                        }
                        else
                        { //agrega el de BD 
                            listDemanda.Add(dem);
                        }
                    }
                }
            }

            //solo manda los de tipo customer
            listDemanda = listDemanda.Where(x => x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER).ToList();
            ViewBag.IHS_Items = listado;

            return View(listDemanda);

        }

        // POST: BG_IHS_item/CargaDemandaCliente/5
        [HttpPost]
        public ActionResult CargaDemandaCliente(List<BG_IHS_rel_demanda> demandaList, string country_territory, string manufacturer, string production_plant, string mnemonic_vehicle_plant, string origen, string demanda, int? version)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int actualizados = 0;
                    int creados = 0;

                    //Listado de la demanda actual en la BD
                    List<BG_IHS_rel_demanda> demandaBD = db.BG_IHS_rel_demanda.Where(x => x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER).ToList();

                    //determina cuales de los rel demanda no han sido creados, deben tener id=0
                    List<BG_IHS_rel_demanda> demandaCreate = demandaList.Where(x => x.id == 0).ToList();

                    //obtiene el listado de los registros que deben actualizarse
                    List<BG_IHS_rel_demanda> demandaUpdate = demandaList.Where(x => demandaBD.Any(y => x.id == y.id && x.cantidad != y.cantidad)).ToList();

                    //crea un registro en BD por cada elemento
                    foreach (var item in demandaCreate)
                    {
                        db.BG_IHS_rel_demanda.Add(item);
                        creados++;
                    }

                    //actualiza los registros que tuvieron cambios
                    foreach (var item in demandaUpdate)
                    {
                        var d = demandaBD.FirstOrDefault(x => x.id == item.id);
                        d.cantidad = item.cantidad;

                        actualizados++;
                    }

                    //guarda los cambios en bd
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert("Los datos se guardaron correctamente. Creados: " + creados + ", Actualizados: " + actualizados, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("ListadoIHS", new { origen = origen, country_territory = country_territory, manufacturer = manufacturer, production_plant = production_plant, mnemonic_vehicle_plant = mnemonic_vehicle_plant, demanda = demanda, version = version });
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ocurrió un error:" + e.Message, TipoMensajesSweetAlerts.WARNING);
                    return RedirectToAction("ListadoIHS", new { origen = origen, country_territory = country_territory, manufacturer = manufacturer, production_plant = production_plant, mnemonic_vehicle_plant = mnemonic_vehicle_plant, demanda = demanda, version = version });
                }
            }
            return RedirectToAction("ListadoIHS", new { origen = origen, country_territory = country_territory, manufacturer = manufacturer, production_plant = production_plant, mnemonic_vehicle_plant = mnemonic_vehicle_plant, demanda = demanda, version = version });
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

        // GET: BG_IHS_item/DetallesAsociacion/5
        public ActionResult DetallesAsociacion(string id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Match m = Regex.Match(id, @"\d+");

            int num_id = 0;
            if (m.Success) //si tiene un numero
            {
                bool valido = int.TryParse(m.Value, out num_id);

                if (!valido)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //separa la clave del id
            switch (id[0])
            {
                case 'I':
                    return RedirectToAction("details", new { id = num_id });
                case 'C':
                    return RedirectToAction("details", "BG_IHS_combinacion", new { id = num_id });
                case 'D':
                    var rel_div = db.BG_IHS_rel_division.Find(num_id);
                    return RedirectToAction("details", "BG_IHS_division", new { id = rel_div.BG_IHS_division.id });
                case 'H':
                    return RedirectToAction("details", "BG_ihs_vehicle_custom", new { id = num_id });

            }

            return RedirectToAction("details", new { id = num_id });
        }



        // GET: BG_IHS_item/Create
        public ActionResult Create(int? id, int version = 0)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            BG_IHS_item model;

            //obtiene el menor año de los archivos cargados de ihs
            int anoInicio = 2019, anoFin = 2030;
            try
            {
                anoInicio = db.BG_IHS_rel_demanda.OrderBy(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
                anoFin = db.BG_IHS_rel_demanda.OrderByDescending(x => x.fecha).Select(x => x.fecha).FirstOrDefault().Year;
            }
            catch (Exception) { /* do nothing */ }


            if (id == null) //si no hay id, crea un nuevo modelo
            {
                model = new BG_IHS_item { porcentaje_scrap = 0.03M };

                //crea un rel para cada año existente en la bd
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
            }
            else
            {
                //si hay id crea una copia del modelo
                var m = db.BG_IHS_item.Find(id);

                //si hay coincidencia de modelo
                if (m != null)
                {
                    model = m;
                    //model.mnemonic_vehicle_plant = String.Empty;
                    model.vehicle = String.Empty;

                    List<BG_IHS_rel_demanda> nuevaLista = new List<BG_IHS_rel_demanda>();
                    //recorre los rel demanda y agrega un divBD si es necesario
                    for (int i = anoInicio; i <= anoFin; i++)
                    {
                        for (int j = 1; j <= 12; j++)
                        {
                            DateTime fecha = new DateTime(i, j, 1);

                            var item = model.BG_IHS_rel_demanda.Where(x => x.fecha == fecha && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL).FirstOrDefault();

                            //existe un rel
                            if (item != null)
                            {
                                //toma el valor del original pero lo convierte en customer
                                item.tipo = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER;
                                nuevaLista.Add(item);
                            }
                            else
                            { //no existe divBD
                                nuevaLista.Add(new BG_IHS_rel_demanda
                                {
                                    //cantidad = 0,
                                    fecha = fecha,
                                    tipo = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER
                                });

                            }
                        }
                    }

                    //reemplaza los rel con la nueva lista
                    model.BG_IHS_rel_demanda = nuevaLista;

                }
                else
                { //si no hay modelo
                    return View("../Error/BadRequest");
                }

            }

            model.versionIHS = version;
            model.id_ihs_version = version;

            // -- listado de plantas
            List<SelectListItem> selectListPlant = new List<SelectListItem>();
            var plantas = db.BG_IHS_plantas.Where(x => x.id_ihs_version == version).Distinct();
            foreach (var p in plantas)
                selectListPlant.Add(new SelectListItem() { Text = p.ConcatPlanta, Value = p.mnemonic_plant });

            ViewBag.mnemonic_plant = AddFirstItem(new SelectList(selectListPlant, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: model.mnemonic_plant);

            // -- listado de segmentos
            List<SelectListItem> selectListSegment = new List<SelectListItem>();
            var segments = db.BG_IHS_segmentos.Where(x => x.id_ihs_version == version).Select(x => x.global_production_segment).Distinct();
            foreach (var s in segments)
                selectListSegment.Add(new SelectListItem() { Text = s, Value = s });

            ViewBag.global_production_segment = AddFirstItem(new SelectList(selectListSegment, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: model.global_production_segment);

            //envia objeto con la versión de IHS
            ViewBag.VersionIHS = db.BG_IHS_versiones.Find(version);

            return View(model);
        }

        // POST: BG_IHS_item/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BG_IHS_item bG_IHS_item)
        {

            //ModelState.AddModelError("", "Demo error");

            //if (db.BG_IHS_item.Any(x => x.id_ihs_version == bG_IHS_item.id_ihs_version && x.mnemonic_vehicle_plant == bG_IHS_item.mnemonic_vehicle_plant && x.mnemonic_vehicle_plant == bG_IHS_item.mnemonic_vehicle_plant && x.sop_start_of_production == bG_IHS_item.sop_start_of_production))
            //    ModelState.AddModelError("", "Ya existe un registro con los mismos valores para Mnemonic-Vehicle/Plant, Vehicle y SOP " + bG_IHS_item.mnemonic_vehicle_plant);
            if (db.BG_IHS_item.Any(x => x.id_ihs_version == bG_IHS_item.id_ihs_version && x.mnemonic_vehicle_plant == bG_IHS_item.mnemonic_vehicle_plant))
                ModelState.AddModelError("", "Ya existe un registro con el mismo Mnemonic Vehicle Plant en esta versión de IHS.");

            if (ModelState.IsValid)
            {
                //coloca el porcentaje en decimales
                bG_IHS_item.porcentaje_scrap = bG_IHS_item.porcentaje_scrap / 100;
                bG_IHS_item.id_ihs_version = bG_IHS_item.versionIHS;
                db.BG_IHS_item.Add(bG_IHS_item);

                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("ListadoIHS", new { version = bG_IHS_item.versionIHS, origen = Bitacoras.Util.BG_IHS_Origen.UNION });
            }

            // -- listado de plantas
            List<SelectListItem> selectListPlant = new List<SelectListItem>();
            var plantas = db.BG_IHS_plantas.Where(x => x.id_ihs_version == bG_IHS_item.id_ihs_version).Distinct();
            foreach (var p in plantas)
                selectListPlant.Add(new SelectListItem() { Text = p.ConcatPlanta, Value = p.mnemonic_plant });

            ViewBag.mnemonic_plant = AddFirstItem(new SelectList(selectListPlant, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: bG_IHS_item.mnemonic_plant);

            // -- listado de segmentos
            List<SelectListItem> selectListSegment = new List<SelectListItem>();
            var segments = db.BG_IHS_segmentos.Where(x => x.id_ihs_version == bG_IHS_item.id_ihs_version).Select(x => x.global_production_segment).Distinct();
            foreach (var s in segments)
                selectListSegment.Add(new SelectListItem() { Text = s, Value = s });

            ViewBag.global_production_segment = AddFirstItem(new SelectList(selectListSegment, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: bG_IHS_item.global_production_segment);

            //envia objeto con la versión de IHS
            ViewBag.VersionIHS = db.BG_IHS_versiones.Find(bG_IHS_item.versionIHS);

            return View(bG_IHS_item);
        }

        // GET: BG_IHS_item/Edit/5
        public ActionResult Edit(int? id, string s_country_territory, string s_manufacturer, string s_production_plant, string s_mnemonic_vehicle_plant, string s_origen, string s_demanda)
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
            ViewBag.s_mnemonic_vehicle_plant = s_mnemonic_vehicle_plant;

            //enlazar mnemonic_vehicle_plant plant con la descripcion de la planta
            //var plantaSelected = db.BG_IHS_plantas.FirstOrDefault(x=>x.descripcion = bG_IHS_item.production_plant && x);

            // -- listado de plantas
            List<SelectListItem> selectListPlant = new List<SelectListItem>();
            var plantas = db.BG_IHS_plantas.Where(x => x.id_ihs_version == bG_IHS_item.id_ihs_version).Distinct();
            foreach (var p in plantas)
                selectListPlant.Add(new SelectListItem() { Text = p.ConcatPlanta, Value = p.mnemonic_plant });

            ViewBag.mnemonic_plant = AddFirstItem(new SelectList(selectListPlant, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: bG_IHS_item.mnemonic_plant);

            // -- listado de segmentos
            List<SelectListItem> selectListSegment = new List<SelectListItem>();
            var segments = db.BG_IHS_segmentos.Where(x => x.id_ihs_version == bG_IHS_item.id_ihs_version).Select(x => x.global_production_segment).Distinct();
            foreach (var s in segments)
                selectListSegment.Add(new SelectListItem() { Text = s, Value = s });

            ViewBag.global_production_segment = AddFirstItem(new SelectList(selectListSegment, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: bG_IHS_item.global_production_segment);

            return View(bG_IHS_item);
        }

        // POST: BG_IHS_item/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BG_IHS_item bG_IHS_item, string s_country_territory, string s_manufacturer, string s_production_plant, string s_mnemonic_vehicle_plant, string s_origen, string s_demanda)
        {

            //if (db.BG_IHS_item.Any(x => x.id_ihs_version == bG_IHS_item.id_ihs_version && x.mnemonic_vehicle_plant == bG_IHS_item.mnemonic_vehicle_plant && x.mnemonic_vehicle_plant == bG_IHS_item.mnemonic_vehicle_plant
            //                && x.sop_start_of_production == bG_IHS_item.sop_start_of_production && x.id != bG_IHS_item.id))
            //    ModelState.AddModelError("", "Ya existe un registro con los mismos valores para Mnemonic-Vehicle/Plant, Vehicle y SOP " + bG_IHS_item.mnemonic_vehicle_plant);

            if (db.BG_IHS_item.Any(x => x.id_ihs_version == bG_IHS_item.id_ihs_version && x.mnemonic_vehicle_plant == bG_IHS_item.mnemonic_vehicle_plant && x.id != bG_IHS_item.id))
                ModelState.AddModelError("", "Ya existe un registro con el mismo Mnemonic Vehicle Plant en esta versión de IHS.");

            if (ModelState.IsValid)
            {
                var rels = bG_IHS_item.BG_IHS_rel_demanda;

                bG_IHS_item.BG_IHS_rel_demanda = new List<BG_IHS_rel_demanda>();
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

                        if (item != null)//si ya existe lo actualiza
                        {
                            rel.id = item.id;
                            db.Entry(item).CurrentValues.SetValues(rel);
                        }
                        else
                        { //si no existe, lo crea
                            db.BG_IHS_rel_demanda.Add(rel);
                        }
                    }

                    //////busca si existe un rel region para la planta de producción actual
                    ////if (!db.BG_IHS_rel_regiones.Any(x => x.production_plant == bG_IHS_item.production_plant))
                    ////{
                    ////    db.BG_IHS_rel_regiones.Add(new BG_IHS_rel_regiones
                    ////    {
                    ////        production_plant = bG_IHS_item.production_plant,
                    ////    });
                    ////}

                    ////busca si existe un rel segmento para el global segment actual
                    //if (!db.BG_IHS_segmentos.Any(x => x.global_production_segment == bG_IHS_item.global_production_segment))
                    //{
                    //    db.BG_IHS_segmentos.Add(new BG_IHS_segmentos
                    //    {
                    //        global_production_segment = bG_IHS_item.global_production_segment,
                    //    });
                    //}

                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("ListadoIHS", new { version = bG_IHS_item.id_ihs_version, origen = s_origen, country_territory = s_country_territory, manufacturer = s_manufacturer, production_plant = s_production_plant, mnemonic_vehicle_plant = s_mnemonic_vehicle_plant, demanda = s_demanda });
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert(e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("ListadoIHS", new { version = bG_IHS_item.id_ihs_version, origen = s_origen, country_territory = s_country_territory, manufacturer = s_manufacturer, production_plant = s_production_plant, mnemonic_vehicle_plant = s_mnemonic_vehicle_plant, demanda = s_demanda });
                }
            }

            //viewbag para redirect
            ViewBag.s_origen = s_origen;
            ViewBag.s_demanda = s_demanda;
            ViewBag.s_country_territory = s_country_territory;
            ViewBag.s_manufacturer = s_manufacturer;
            ViewBag.s_production_plant = s_production_plant;
            ViewBag.s_mnemonic_vehicle_plant = s_mnemonic_vehicle_plant;

            // -- listado de plantas
            List<SelectListItem> selectListPlant = new List<SelectListItem>();
            var plantas = db.BG_IHS_plantas.Where(x => x.id_ihs_version == bG_IHS_item.id_ihs_version).Distinct();
            foreach (var p in plantas)
                selectListPlant.Add(new SelectListItem() { Text = p.ConcatPlanta, Value = p.mnemonic_plant });

            ViewBag.mnemonic_plant = AddFirstItem(new SelectList(selectListPlant, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: bG_IHS_item.mnemonic_plant);

            // -- listado de segmentos
            List<SelectListItem> selectListSegment = new List<SelectListItem>();
            var segments = db.BG_IHS_segmentos.Where(x => x.id_ihs_version == bG_IHS_item.id_ihs_version).Select(x => x.global_production_segment).Distinct();
            foreach (var s in segments)
                selectListSegment.Add(new SelectListItem() { Text = s, Value = s });

            ViewBag.global_production_segment = AddFirstItem(new SelectList(selectListSegment, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: bG_IHS_item.global_production_segment);

            var rels_demanda_original = db.BG_IHS_rel_demanda.Where(x => x.id_ihs_item == bG_IHS_item.id && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL);
            foreach (var item in rels_demanda_original)
                bG_IHS_item.BG_IHS_rel_demanda.Add(item);

            return View(bG_IHS_item);
        }

        public ActionResult Exportar(string country_territory, string manufacturer, string production_plant, string mnemonic_vehicle_plant, string origen, int? version, string demanda = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER)
        {
            if (TieneRol(TipoRoles.BUDGET_IHS))
            {
                var listadoBD = db.BG_IHS_item.Where(x => x.id_ihs_version == version);

                //verifica que el elemento este relacionado con el elmento anterior
                if (
                    !listadoBD.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory) && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)) && x.manufacturer == manufacturer))
                    manufacturer = String.Empty;

                if (
                    !listadoBD.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory))
                                                && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                                                && x.production_plant == production_plant
                                                && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                                                ))
                    production_plant = String.Empty;

                if (
                    !listadoBD.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory))
                                                && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                                                && (x.production_plant == production_plant || String.IsNullOrEmpty(production_plant))
                                                && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                                                && mnemonic_vehicle_plant == x.mnemonic_vehicle_plant))
                    mnemonic_vehicle_plant = String.Empty;

                var listado = listadoBD
                    .Where(x =>
                        (x.country_territory == country_territory || String.IsNullOrEmpty(country_territory))
                        && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                        && (x.production_plant == production_plant || String.IsNullOrEmpty(production_plant))
                        && (x.mnemonic_vehicle_plant == mnemonic_vehicle_plant || String.IsNullOrEmpty(mnemonic_vehicle_plant))
                        && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                        ).ToList();

                var combinaciones = db.BG_IHS_combinacion.Where(x => x.activo && x.id_ihs_version == version).ToList();
                var divisiones = db.BG_IHS_division.Where(x => x.activo && x.id_ihs_version == version).ToList();

                byte[] stream = ExcelUtil.GeneraReporteBudgetIHS(listado, combinaciones, divisiones, demanda);

                var versionBD = db.BG_IHS_versiones.Find(version);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Reporte_IHS_" + demanda + "_" + (versionBD != null ? versionBD.ConcatVersion : String.Empty) + ".xlsx",

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
        public ActionResult GeneraPlantillaDemanda(string country_territory, string manufacturer, string production_plant, string mnemonic_vehicle_plant, string origen, int? version, string demanda = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER)
        {
            if (TieneRol(TipoRoles.BUDGET_IHS))
            {
                var listadoBD = db.BG_IHS_item.Where(x => x.id_ihs_version == version);

                //verifica que el elemento este relacionado con el elmento anterior
                if (
                    !listadoBD.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory) && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)) && x.manufacturer == manufacturer))
                    manufacturer = String.Empty;

                if (
                    !listadoBD.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory))
                                                && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                                                && x.production_plant == production_plant
                                                && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                                                ))
                    production_plant = String.Empty;

                if (
                    !listadoBD.Any(x => (country_territory == x.country_territory || String.IsNullOrEmpty(country_territory))
                                                && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                                                && (x.production_plant == production_plant || String.IsNullOrEmpty(production_plant))
                                                && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                                                && mnemonic_vehicle_plant == x.mnemonic_vehicle_plant))
                    mnemonic_vehicle_plant = String.Empty;

                var listado = listadoBD
                    .Where(x =>
                        (x.country_territory == country_territory || String.IsNullOrEmpty(country_territory))
                        && (x.manufacturer == manufacturer || String.IsNullOrEmpty(manufacturer))
                        && (x.production_plant == production_plant || String.IsNullOrEmpty(production_plant))
                        && (x.mnemonic_vehicle_plant == mnemonic_vehicle_plant || String.IsNullOrEmpty(mnemonic_vehicle_plant))
                        && (x.origen == origen || origen == Bitacoras.Util.BG_IHS_Origen.UNION)
                        ).ToList();

                var combinaciones = db.BG_IHS_combinacion.Where(x => x.activo && x.id_ihs_version == version).ToList();
                var divisiones = db.BG_IHS_division.Where(x => x.activo && x.id_ihs_version == version).ToList();

                byte[] stream = ExcelUtil.GeneraPlantillaDemandaIHS(listado, combinaciones, divisiones, demanda);

                var versionBD = db.BG_IHS_versiones.Find(version);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Plantilla_Demanda_IHS_" + demanda + "_" + (versionBD != null ? versionBD.ConcatVersion : String.Empty) + ".xlsx",

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

        // GET: Bom/CargaPlantillaDemanda/5
        public ActionResult CargaPlantillaDemanda(string country_territory, string manufacturer, string production_plant, string mnemonic_vehicle_plant, string origen, int? version)
        {
            if (TieneRol(TipoRoles.BUDGET_IHS))
            {
                return View(new ExcelViewModel { version = version });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Dante/CargaPlantillaDemanda/5
        [HttpPost]
        public ActionResult CargaPlantillaDemanda(string country_territory, string manufacturer, string production_plant, string mnemonic_vehicle_plant, string origen, string demanda, int? version, ExcelViewModel excelViewModel, FormCollection collection)
        {
            if (ModelState.IsValid)
            {


                string msjError = "No se ha podido leer el archivo seleccionado.";

                //lee el archivo seleccionado
                try
                {
                    HttpPostedFileBase stream = Request.Files["PostedFile"];


                    if (stream.InputStream.Length > 8388608)
                    {
                        msjError = "Sólo se permiten archivos con peso menor a 8 MB.";
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
                    string msjErrorPlantilla = string.Empty;
                    //el archivo es válido
                    List<BG_IHS_rel_demanda> demandaList = UtilExcel.LeePlantillaDemanda(excelViewModel.PostedFile, ref estructuraValida, ref msjErrorPlantilla);

                    List<int> listaIds = demandaList.Select(x => x.id_ihs_item).Distinct().ToList();

                    //quita los repetidos
                    demandaList = demandaList.Distinct().ToList();

                    if (!estructuraValida)
                    {
                        msjError = "No cumple con la estructura válida.";
                        throw new Exception(msjError);
                    }
                    else
                    {
                        int actualizados = 0;
                        int creados = 0;

                        //Listado de la demanda actual del cliente de la BD
                        List<BG_IHS_rel_demanda> demandaBDCUSTOMER = db.BG_IHS_rel_demanda.Where(x => listaIds.Contains(x.id_ihs_item) && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER).ToList();
                        //Listado de la demanda actual original del cliente
                        List<BG_IHS_rel_demanda> demandaOriginalBDIHS = db.BG_IHS_rel_demanda.Where(x => listaIds.Contains(x.id_ihs_item) && x.tipo == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL).ToList();

                        //determina cuales de los rel demanda no se encuentran en la BD de cliente
                        List<BG_IHS_rel_demanda> demandaCreate = demandaList.Where(x => !demandaBDCUSTOMER.Any(y => y.id_ihs_item == x.id_ihs_item && x.fecha == y.fecha) && x.cantidad.HasValue).ToList();

                        //obtiene el listado de los registros que deben actualizarse
                        List<BG_IHS_rel_demanda> demandaUpdate = demandaList.Where(x => demandaBDCUSTOMER.Any(y => y.id_ihs_item == x.id_ihs_item && x.fecha == y.fecha)).ToList();

                        //crea un registro en BD por cada elemento
                        foreach (var item in demandaCreate)
                        {
                            //encaso de no existir verifica que el valor sea distindo al de la demanda original IHS
                            var o = demandaOriginalBDIHS.FirstOrDefault(x => x.fecha == item.fecha && x.id_ihs_item == item.id_ihs_item);
                            //si la cantidad es la misma entonces lo ignora
                            if (o == null || o.cantidad != item.cantidad)
                            {
                                db.BG_IHS_rel_demanda.Add(item);
                                creados++;
                            }
                        }

                        //acualiza los registros que tuvieron cambio
                        foreach (var item in demandaUpdate)
                        {
                            var c = demandaBDCUSTOMER.FirstOrDefault(x => x.fecha == item.fecha && x.id_ihs_item == item.id_ihs_item);
                            var o = demandaOriginalBDIHS.FirstOrDefault(x => x.fecha == item.fecha && x.id_ihs_item == item.id_ihs_item);

                            if (c.cantidad != item.cantidad/* && (o == null || item.cantidad != o.cantidad)*/)
                            {
                                c.cantidad = item.cantidad;
                                actualizados++;
                            }

                            //si la original es igual a la cliente, elimina la del cliente
                            if (o != null && item.cantidad == o.cantidad)
                            {
                                try
                                {
                                    db.BG_IHS_rel_demanda.Remove(c);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }

                        }

                        //guarda los cambios en bd
                        db.SaveChanges();

                        TempData["Mensaje"] = new MensajesSweetAlert("Los datos se guardaron correctamente. Creados: " + creados + ", Actualizados: " + actualizados, TipoMensajesSweetAlerts.SUCCESS);
                        return RedirectToAction("ListadoIHS", new { origen = origen, country_territory = country_territory, manufacturer = manufacturer, production_plant = production_plant, mnemonic_vehicle_plant = mnemonic_vehicle_plant, demanda = demanda, version = excelViewModel.version });
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", msjError);
                    TempData["Mensaje"] = new MensajesSweetAlert("Ocurrió un error: " + msjError, TipoMensajesSweetAlerts.ERROR);

                    return RedirectToAction("ListadoIHS", new { origen = origen, country_territory = country_territory, manufacturer = manufacturer, production_plant = production_plant, mnemonic_vehicle_plant = mnemonic_vehicle_plant, demanda = demanda, version = excelViewModel.version });
                }

            }
            TempData["Mensaje"] = new MensajesSweetAlert("Ocurrió un error al guardar en BD.", TipoMensajesSweetAlerts.ERROR);

            return RedirectToAction("ListadoIHS", new { origen = origen, country_territory = country_territory, manufacturer = manufacturer, production_plant = production_plant, mnemonic_vehicle_plant = mnemonic_vehicle_plant, demanda = demanda, version = excelViewModel.version });
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

    class IHSRelRegionPlantaString
    {
        public string planta { get; set; }
        public string region { get; set; }
    }

}
