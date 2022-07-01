using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_inventory_itemsController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_inventory_items
        public ActionResult Index(int? id_planta, int? tipo_hardware, string hostname, string description, string model, bool? active, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listado = db.IT_inventory_items
                    .Where(x =>
                   (x.id_planta == id_planta || id_planta == null)
                    && (x.hostname.Contains(hostname) || String.IsNullOrEmpty(hostname))
                      && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                      && (x.descripcion.Contains(description) || String.IsNullOrEmpty(description))
                    && (x.active == active || active == null)
                    && (x.id_inventory_type == tipo_hardware && tipo_hardware.HasValue)
                    )
                  .OrderByDescending(x => x.id_planta)
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_items
                      .Where(x =>
                   (x.id_planta == id_planta || id_planta == null)
                    && (x.hostname.Contains(hostname) || String.IsNullOrEmpty(hostname))
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                        && (x.descripcion.Contains(description) || String.IsNullOrEmpty(description))
                    && (x.active == active || active == null)
                    && (x.id_inventory_type == tipo_hardware && tipo_hardware.HasValue)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_planta"] = id_planta;
                routeValues["hostname"] = hostname;
                routeValues["description"] = description;
                routeValues["model"] = model;
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
                ViewBag.tipo_hardware = AddFirstItem(new SelectList(db.IT_inventory_hardware_type.Where(x => x.activo), "id", "descripcion"), textoPorDefecto: "-- Select --", selected: id_planta.ToString());
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: IT_inventory_items/Create
        public ActionResult Create(int? id, int? id_hardware_type, int? _id_planta, string _hostname, bool? _active, int? pagina, string _model)
        {
            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            if (id_hardware_type == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_hardware_type type = db.IT_inventory_hardware_type.Find(id_hardware_type);
            if (type == null)
            {
                return View("../Error/NotFound");
            }

            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
           

            string brand = string.Empty;
            if (type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.LAPTOP || type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.DESKTOP)
                brand = "HP";

            //modelo por defecto
            IT_inventory_items model = new IT_inventory_items
            {
                active = true,
                filtro_activo = _active,
                filtro_hostname = _hostname,
                filtro_id_planta = _id_planta,
                filtro_model = _model,
                filtro_pagina = pagina,
                brand = brand
            };

            //en caso de ser un clon copia los valores y deja hostname vacio
            if (iT_inventory_items != null)
            {
                model = iT_inventory_items;
                model.hostname = null;

                ViewBag.EsClone = true;
            }

            ViewBag.type = type;
            ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --", selected: model.bits_operation_system.ToString());
            ViewBag.physical_server = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x=>x.active ==true && x.IT_inventory_hardware_type.descripcion == IT_Tipos_Hardware.SERVER), "id", "ConcatInfoGeneral"), textoPorDefecto: "-- Seleccionar --", selected: model.physical_server.ToString());
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: model.id_planta.ToString());

            //asigna los filtros
            return View(model);

        }

        // POST: IT_inventory_items/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IT_inventory_items iT_inventory_items, FormCollection collection)
        {

            List<IT_inventory_hard_drives> drives = new List<IT_inventory_hard_drives>();
            iT_inventory_items.IT_inventory_hard_drives = drives; //vacia la lista que viene del form (solo detecta los agregados con js)

            #region obtiene y valida drives

            //obtiene los drives del formcollection
            foreach (string key in collection.AllKeys.Where(x => x.StartsWith("IT_inventory_hard_drives") && x.EndsWith(".disk_name")))
            {
                int index = -1;
                decimal total_drive_space_gb = 0;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    //obtiene el index
                    int.TryParse(m.Value, out index);

                    //obtiene los valores del form para el index asociado
                    string disk_name = collection["IT_inventory_hard_drives[" + index + "].disk_name"].ToUpper();
                    string type_drive = collection["IT_inventory_hard_drives[" + index + "].type_drive"];
                    string driveSpace = collection["IT_inventory_hard_drives[" + index + "].total_drive_space_gb"];

                    Decimal.TryParse(collection["IT_inventory_hard_drives[" + index + "].total_drive_space_gb"], out total_drive_space_gb);

                    //agrega el drive
                    drives.Add(
                        new IT_inventory_hard_drives
                        {
                            disk_name = disk_name,
                            type_drive = type_drive,
                            total_drive_space_gb = total_drive_space_gb,
                        }
                    );

                }
            }

            //determina si hay drives repetidos
            var letters = drives.Select(x => x.disk_name).Distinct().ToList();

            foreach (var item in letters)
            {
                char letter = Char.Parse(item);
                if (!Char.IsLetter(letter))
                    ModelState.AddModelError("", "The name for hard drive " + item + " is not valid.");

                int num = iT_inventory_items.IT_inventory_hard_drives.Where(x => x.disk_name == item).Count();

                if (num > 1)
                    ModelState.AddModelError("", "The hard drive " + item + " is duplicated.");

            }

            //verifica que no exista el hostname
            if (db.IT_inventory_items.Any(x => x.hostname == iT_inventory_items.hostname && !String.IsNullOrEmpty(x.hostname) && x.id_inventory_type == iT_inventory_items.id_inventory_type && x.active == true))
                ModelState.AddModelError("", "An active record with the same hostname already exists.");


            #endregion

            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                db.IT_inventory_items.Add(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("index", new
                {
                    tipo_hardware = iT_inventory_items.id_inventory_type,
                    id_planta = iT_inventory_items.filtro_id_planta,
                    //hostname = iT_inventory_items.filtro_hostname,
                    //active = iT_inventory_items.filtro_activo,
                    //pagina = iT_inventory_items.filtro_pagina,
                    //model = iT_inventory_items.filtro_model,
                });
            }
            //busca el tipo inventario para desktop
            var type = db.IT_inventory_hardware_type.Find(iT_inventory_items.id_inventory_type);

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.bits_operation_system.ToString());
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
            ViewBag.physical_server = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x=>x.active ==true && x.IT_inventory_hardware_type.descripcion == IT_Tipos_Hardware.SERVER), "id", "ConcatInfoGeneral"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.physical_server.ToString());
      

            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Edit
        public ActionResult Edit(int? id, int? _id_planta, string _hostname, bool? _active, int? pagina, string _model)
        {
            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            ViewBag.type = iT_inventory_items.IT_inventory_hardware_type;
            ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.bits_operation_system.ToString());
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
            ViewBag.physical_server = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x => x.active == true && x.IT_inventory_hardware_type.descripcion == IT_Tipos_Hardware.SERVER), "id", "ConcatInfoGeneral"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.physical_server.ToString());


            //asigna los filtros
            iT_inventory_items.filtro_activo = _active;
            iT_inventory_items.filtro_hostname = _hostname;
            iT_inventory_items.filtro_id_planta = _id_planta;
            iT_inventory_items.filtro_model = _model;
            iT_inventory_items.filtro_pagina = pagina;

            return View(iT_inventory_items);

        }

        // POST: IT_inventory_items/Edit
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IT_inventory_items iT_inventory_items, FormCollection collection)
        {

            List<IT_inventory_hard_drives> drives = new List<IT_inventory_hard_drives>();
            iT_inventory_items.IT_inventory_hard_drives = drives; //vacia la lista que viene del form (solo detecta los agregados con js)

            #region obtiene y valida drives

            //obtiene los drives del formcollection
            foreach (string key in collection.AllKeys.Where(x => x.StartsWith("IT_inventory_hard_drives") && x.EndsWith(".disk_name")))
            {
                int index = -1;
                decimal total_drive_space_gb = 0;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    //obtiene el index
                    int.TryParse(m.Value, out index);

                    //obtiene los valores del form para el index asociado
                    string disk_name = collection["IT_inventory_hard_drives[" + index + "].disk_name"].ToUpper();
                    string type_drive = collection["IT_inventory_hard_drives[" + index + "].type_drive"];
                    decimal.TryParse(collection["IT_inventory_hard_drives[" + index + "].total_drive_space_gb"], out total_drive_space_gb);

                    //agrega el drive
                    drives.Add(
                        new IT_inventory_hard_drives
                        {
                            id_inventory_item = iT_inventory_items.id,
                            disk_name = disk_name,
                            type_drive = type_drive,
                            total_drive_space_gb = total_drive_space_gb,
                        }
                    );

                }
            }

            //determina si hay drives repetidos
            var letters = drives.Select(x => x.disk_name).Distinct().ToList();

            foreach (var item in letters)
            {
                char letter = Char.Parse(item);
                if (!Char.IsLetter(letter))
                    ModelState.AddModelError("", "The name for hard drive " + item + " is not valid.");

                int num = iT_inventory_items.IT_inventory_hard_drives.Where(x => x.disk_name == item).Count();

                if (num > 1)
                    ModelState.AddModelError("", "The hard drive " + item + " is duplicated.");

            }

            //verifica que no exista el hostname
            if (db.IT_inventory_items.Any(x => x.hostname == iT_inventory_items.hostname && !String.IsNullOrEmpty(x.hostname) && x.id_inventory_type == iT_inventory_items.id_inventory_type && x.id != iT_inventory_items.id && x.active == true))
                ModelState.AddModelError("", "An active record with the same hostname already exists.");


            #endregion

            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                //elimina los items previos
                var listHardDrives = db.IT_inventory_hard_drives.Where(x => x.id_inventory_item == iT_inventory_items.id);
                db.IT_inventory_hard_drives.RemoveRange(listHardDrives);

                //agrega los nuevos harddrives
                foreach (IT_inventory_hard_drives drive in iT_inventory_items.IT_inventory_hard_drives)
                    db.IT_inventory_hard_drives.Add(drive);


                // Activity already exist in database and modify it
                db.Entry(db.IT_inventory_items.Find(iT_inventory_items.id)).CurrentValues.SetValues(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("index", new
                {
                    tipo_hardware = iT_inventory_items.id_inventory_type,
                    id_planta = iT_inventory_items.filtro_id_planta,
                    //hostname = iT_inventory_items.filtro_hostname,
                    //active = iT_inventory_items.filtro_activo,
                    //pagina = iT_inventory_items.filtro_pagina,
                    //model = iT_inventory_items.filtro_model,
                });
            }
            //busca el tipo inventario para desktop
            var type = db.IT_inventory_hardware_type.Find(iT_inventory_items.id_inventory_type);

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.bits_operation_system.ToString());
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
            ViewBag.physical_server = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x => x.active == true && x.IT_inventory_hardware_type.descripcion == IT_Tipos_Hardware.SERVER), "id", "ConcatInfoGeneral"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.physical_server.ToString());


            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Export/5
        public ActionResult Export(int? id_planta, int? tipo_hardware, string hostname, string description, string model, bool? active)
        {
            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            if (tipo_hardware == null)
            {
                return View("../Error/BadRequest");
            }

            IT_inventory_hardware_type type = db.IT_inventory_hardware_type.Find(tipo_hardware);
            if (type == null)
            {
                return View("../Error/NotFound");
            }

            switch (type.descripcion)
            {
                case Bitacoras.Util.IT_Tipos_Hardware.LAPTOP:
                    return RedirectToAction("ExportarLaptop", new { id_planta = id_planta, hostname = hostname, active = active });
                case Bitacoras.Util.IT_Tipos_Hardware.DESKTOP:
                    return RedirectToAction("ExportarDesktop", new { id_planta = id_planta, hostname = hostname, active = active });
                case Bitacoras.Util.IT_Tipos_Hardware.SERVER:
                    return RedirectToAction("Exportarserver", new { id_planta = id_planta, hostname = hostname, active = active });
                case Bitacoras.Util.IT_Tipos_Hardware.VIRTUAL_SERVER:
                    return RedirectToAction("Exportarvirtualserver", new { id_planta = id_planta, hostname = hostname, active = active });
                case Bitacoras.Util.IT_Tipos_Hardware.MONITOR:
                    return RedirectToAction("Exportarmonitor", new { id_planta = id_planta, model = model, active = active });
                case Bitacoras.Util.IT_Tipos_Hardware.PRINTER:
                    return RedirectToAction("Exportarprinter", new { id_planta = id_planta, model = model, active = active });
                case Bitacoras.Util.IT_Tipos_Hardware.LABEL_PRINTER:
                    return RedirectToAction("Exportarlabel_printer", new { id_planta = id_planta, model = model, active = active });
                case Bitacoras.Util.IT_Tipos_Hardware.PDA:
                    return RedirectToAction("Exportarpda", new { id_planta = id_planta, model = model, active = active });
                case Bitacoras.Util.IT_Tipos_Hardware.SCANNERS:
                    return RedirectToAction("Exportarscanner", new { id_planta = id_planta, model = model, active = active });
                case Bitacoras.Util.IT_Tipos_Hardware.TABLET:
                    return RedirectToAction("Exportartablet", new { id_planta = id_planta, model = model, active = active });
                case Bitacoras.Util.IT_Tipos_Hardware.RADIO:
                    return RedirectToAction("Exportarradio", new { id_planta = id_planta, model = model, active = active });
                case Bitacoras.Util.IT_Tipos_Hardware.AP:
                    return RedirectToAction("Exportarap", new { id_planta = id_planta, model = model, active = active });
                case Bitacoras.Util.IT_Tipos_Hardware.SMARTPHONE:
                    return RedirectToAction("Exportarsmartphone", new { id_planta = id_planta, model = model, active = active });
                case Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES:
                    return RedirectToAction("ExportarAccessory", new { id_planta = id_planta, description = description, active = active });

                default:
                    return View("../Error/NotFound");
                    ;
            }

        }

        // GET: IT_inventory_items/Smartphone/5
        public ActionResult Smartphone()
        {
            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            //obtiene el id de celulares
            var cel = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion == Bitacoras.Util.IT_Tipos_Hardware.SMARTPHONE);

            return RedirectToAction("Index", new { tipo_hardware = cel.id });
        }

        #region ExportacionExcel

        public ActionResult ExportarDesktop(int? id_planta, string hostname, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para desktop
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("desktop"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.hostname.Contains(hostname) || String.IsNullOrEmpty(hostname))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();


                byte[] stream = ExcelUtil.GeneraReporteITDesktopExcel(listado, type.descripcion);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_Desktop_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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
        public ActionResult ExportarLaptop(int? id_planta, string hostname, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para laptop
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("laptop"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.hostname.Contains(hostname) || String.IsNullOrEmpty(hostname))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();

                //** DE MOMENTO ES EL MISMO QUE DESKTOP ***//
                byte[] stream = ExcelUtil.GeneraReporteITDesktopExcel(listado, type.descripcion);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_Laptop_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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
        public ActionResult Exportarmonitor(int? id_planta, string model, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para monitor
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("monitor"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();

                //** DE MOMENTO ES EL MISMO QUE DESKTOP ***//
                byte[] stream = ExcelUtil.GeneraReporteITMonitorExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_monitor_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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

        public ActionResult ExportarAccessory(int? id_planta, string description, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para monitor
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion == Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES);

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.descripcion.Contains(description) || String.IsNullOrEmpty(description))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();

                //** DE MOMENTO ES EL MISMO QUE DESKTOP ***//
                byte[] stream = ExcelUtil.GeneraReporteITAccessoryExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_accessories_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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

        public ActionResult Exportarprinter(int? id_planta, string model, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para printer
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("printer"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();

                byte[] stream = ExcelUtil.GeneraReporteITPrinterExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_printer_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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
        public ActionResult Exportarlabel_printer(int? id_planta, string model, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para label_printer
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("label") && x.descripcion.Contains("printer"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();

                byte[] stream = ExcelUtil.GeneraReporteITLabelPrinterExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_label_printer_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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
        public ActionResult Exportarpda(int? id_planta, string model, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para pda
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("pda"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();

                //** DE MOMENTO ES EL MISMO QUE DESKTOP ***//
                byte[] stream = ExcelUtil.GeneraReporteITPDAExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_pda_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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
        public ActionResult Exportarscanner(int? id_planta, string model, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para scanner
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("scanner"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();

                //** DE MOMENTO ES EL MISMO QUE DESKTOP ***//
                byte[] stream = ExcelUtil.GeneraReporteITscannerExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_scanner_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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
        public ActionResult Exportarserver(int? id_planta, string hostname, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para server
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("server"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.hostname.Contains(hostname) || String.IsNullOrEmpty(hostname))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();

                //** DE MOMENTO ES EL MISMO QUE DESKTOP ***//
                byte[] stream = ExcelUtil.GeneraReporteITDesktopExcel(listado, type.descripcion);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_server_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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
        public ActionResult Exportarvirtualserver(int? id_planta, string hostname, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para server
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion == Bitacoras.Util.IT_Tipos_Hardware.VIRTUAL_SERVER);

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.hostname.Contains(hostname) || String.IsNullOrEmpty(hostname))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();

                //** DE MOMENTO ES EL MISMO QUE DESKTOP ***//
                byte[] stream = ExcelUtil.GeneraReporteITDesktopExcel(listado, type.descripcion);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_virtual_server_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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
        public ActionResult Exportartablet(int? id_planta, string model, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para tablet
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("tablet"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();

                //** DE MOMENTO ES EL MISMO QUE DESKTOP ***//
                byte[] stream = ExcelUtil.GeneraReporteITTabletExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_tablet_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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
        public ActionResult Exportarradio(int? id_planta, string model, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para radio
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("radio"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();

                byte[] stream = ExcelUtil.GeneraReporteITRadioExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_radio_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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
        public ActionResult Exportarap(int? id_planta, string model, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para ap
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.StartsWith("ap"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();

                byte[] stream = ExcelUtil.GeneraReporteITAPExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_ap_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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
        public ActionResult Exportarsmartphone(int? id_planta, string model, bool? active)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //busca el tipo inventario para smartphone
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("smartphone"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                var listado = db.IT_inventory_items
                    .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                  .OrderByDescending(x => x.id_planta)
               .ToList();

                //** DE MOMENTO ES EL MISMO QUE DESKTOP ***//
                byte[] stream = ExcelUtil.GeneraReporteITSmartphoneExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_smartphone_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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

        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [NonAction]
        protected SelectList SelectBitsOS()
        {
            //crea un Select  list para las opciones
            List<SelectListItem> newList = new List<SelectListItem>();

            newList.Add(new SelectListItem()
            {
                Text = "64",
                Value = "64"
            });

            newList.Add(new SelectListItem()
            {
                Text = "32",
                Value = "32"
            });

            SelectList selectListItems = new SelectList(newList, "Value", "Text");

            return selectListItems;

        }
    }
}
