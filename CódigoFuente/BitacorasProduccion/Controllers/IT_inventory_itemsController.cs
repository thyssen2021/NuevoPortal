using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_inventory_itemsController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_inventory_items
        public ActionResult Index()
        {
            var iT_inventory_items = db.IT_inventory_items.Include(i => i.IT_inventory_hardware_type).Include(i => i.plantas);
            return View(iT_inventory_items.ToList());
        }

        #region Desktop 
        // GET: IT_inventory_items/desktop
        public ActionResult desktop(int? id_planta, string hostname, bool? active, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

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
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_items
                      .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.hostname.Contains(hostname) || String.IsNullOrEmpty(hostname))
                    && (x.active == active || active == null)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_planta"] = id_planta;
                routeValues["hostname"] = hostname;
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
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());


                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

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

        // GET: IT_inventory_items/Details/5
        public ActionResult DetailsDesktop(int? id)
        {
            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            //verifica si el item pertenece a tipo desktop
            //busca el tipo inventario para desktop
            var typeDesktop = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("desktop"));
            if (typeDesktop == null || iT_inventory_items.id_inventory_type != typeDesktop.id)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                return View("../Home/ErrorGenerico");
            }


            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/CreateDesktop
        public ActionResult CreateDesktop()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //busca el tipo inventario para desktop
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("desktop"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                ViewBag.type = type;
                ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --");
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(new IT_inventory_items { active = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_items/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDesktop(IT_inventory_items iT_inventory_items, FormCollection collection)
        {

            List<IT_inventory_hard_drives> drives = new List<IT_inventory_hard_drives>();
            iT_inventory_items.IT_inventory_hard_drives = drives; //vacia la lista que viene del form (solo detecta los agregados con js)

            #region obtiene y valida drives

            //obtiene los drives del formcollection
            foreach (string key in collection.AllKeys.Where(x => x.StartsWith("IT_inventory_hard_drives") && x.EndsWith(".disk_name")))
            {
                int index = -1;
                int total_drive_space_mb = 0;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    //obtiene el index
                    int.TryParse(m.Value, out index);

                    //obtiene los valores del form para el index asociado
                    string disk_name = collection["IT_inventory_hard_drives[" + index + "].disk_name"].ToUpper();
                    string type_drive = collection["IT_inventory_hard_drives[" + index + "].type_drive"];
                    int.TryParse(collection["IT_inventory_hard_drives[" + index + "].total_drive_space_mb"], out total_drive_space_mb);

                    //agrega el drive
                    drives.Add(
                        new IT_inventory_hard_drives
                        {
                            disk_name = disk_name,
                            type_drive = type_drive,
                            total_drive_space_mb = total_drive_space_mb,
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
            if (db.IT_inventory_items.Any(x => x.hostname == iT_inventory_items.hostname && !String.IsNullOrEmpty(x.hostname) && x.id_inventory_type == iT_inventory_items.id_inventory_type && x.active==true))
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

                return RedirectToAction("desktop");
            }
            //busca el tipo inventario para desktop
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("desktop"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.bits_operation_system.ToString());
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/EditDesktop/5
        public ActionResult EditDesktop(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
                if (iT_inventory_items == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si el item pertenece a tipo desktop
                //busca el tipo inventario para desktop
                var typeDesktop = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("desktop"));
                if (typeDesktop == null || iT_inventory_items.id_inventory_type != typeDesktop.id)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                    ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                    return View("../Home/ErrorGenerico");
                }

                ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.bits_operation_system.ToString());
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
                return View(iT_inventory_items);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_items/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDesktop(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            List<IT_inventory_hard_drives> drives = new List<IT_inventory_hard_drives>();
            iT_inventory_items.IT_inventory_hard_drives = drives; //vacia la lista que viene del form (solo detecta los agregados con js)

            #region obtiene y valida drives

            //obtiene los drives del formcollection
            foreach (string key in collection.AllKeys.Where(x => x.StartsWith("IT_inventory_hard_drives") && x.EndsWith(".disk_name")))
            {
                int index = -1;
                int total_drive_space_mb = 0;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    //obtiene el index
                    int.TryParse(m.Value, out index);

                    //obtiene los valores del form para el index asociado
                    string disk_name = collection["IT_inventory_hard_drives[" + index + "].disk_name"].ToUpper();
                    string type_drive = collection["IT_inventory_hard_drives[" + index + "].type_drive"];
                    int.TryParse(collection["IT_inventory_hard_drives[" + index + "].total_drive_space_mb"], out total_drive_space_mb);

                    //agrega el drive
                    drives.Add(
                        new IT_inventory_hard_drives
                        {
                            id_inventory_item = iT_inventory_items.id,
                            disk_name = disk_name,
                            type_drive = type_drive,
                            total_drive_space_mb = total_drive_space_mb,
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

                //agrega los valores de redireccionamiento
                List<string> keys = collection.AllKeys.ToList();

                string string_id_planta = String.Empty;
                string string_pagina = String.Empty;
                string string_hostname = String.Empty;
                string string_active = String.Empty;

                if (keys.Contains("_id_planta"))
                    string_id_planta = collection["_id_planta"];
                if (keys.Contains("_pagina"))
                    string_pagina = collection["_pagina"];
                if (keys.Contains("_hostname"))
                    string_hostname = collection["_hostname"];
                if (keys.Contains("_active"))
                    string_active = collection["_active"];

                return RedirectToAction("desktop", new { id_planta = string_id_planta, pagina = string_pagina, hostname = string_hostname, active = string_active });
            }
            //busca el tipo inventario para desktop
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("desktop"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            iT_inventory_items.IT_inventory_hardware_type = type;
            ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.bits_operation_system.ToString());
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        #endregion


        #region Laptop 
        // GET: IT_inventory_items/laptop
        public ActionResult laptop(int? id_planta, string hostname, bool? active, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

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
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_items
                      .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.hostname.Contains(hostname) || String.IsNullOrEmpty(hostname))
                    && (x.active == active || active == null)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_planta"] = id_planta;
                routeValues["hostname"] = hostname;
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
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());


                return View(listado);
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

        // GET: IT_inventory_items/DetailsLaptop/5
        public ActionResult DetailsLaptop(int? id)
        {
            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            //verifica si el item pertenece a tipo laptop
            //busca el tipo inventario para laptop
            var typeLaptop = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("laptop"));
            if (typeLaptop == null || iT_inventory_items.id_inventory_type != typeLaptop.id)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                return View("../Home/ErrorGenerico");
            }


            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/CreateLaptop
        public ActionResult CreateLaptop()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //busca el tipo inventario para laptop
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("laptop"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                ViewBag.type = type;
                ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --");
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(new IT_inventory_items { active = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_items/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLaptop(IT_inventory_items iT_inventory_items, FormCollection collection)
        {

            List<IT_inventory_hard_drives> drives = new List<IT_inventory_hard_drives>();
            iT_inventory_items.IT_inventory_hard_drives = drives; //vacia la lista que viene del form (solo detecta los agregados con js)

            #region obtiene y valida drives

            //obtiene los drives del formcollection
            foreach (string key in collection.AllKeys.Where(x => x.StartsWith("IT_inventory_hard_drives") && x.EndsWith(".disk_name")))
            {
                int index = -1;
                int total_drive_space_mb = 0;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    //obtiene el index
                    int.TryParse(m.Value, out index);

                    //obtiene los valores del form para el index asociado
                    string disk_name = collection["IT_inventory_hard_drives[" + index + "].disk_name"].ToUpper();
                    string type_drive = collection["IT_inventory_hard_drives[" + index + "].type_drive"];
                    int.TryParse(collection["IT_inventory_hard_drives[" + index + "].total_drive_space_mb"], out total_drive_space_mb);

                    //agrega el drive
                    drives.Add(
                        new IT_inventory_hard_drives
                        {
                            disk_name = disk_name,
                            type_drive = type_drive,
                            total_drive_space_mb = total_drive_space_mb,
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

                return RedirectToAction("laptop");
            }
            //busca el tipo inventario para laptop
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("laptop"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.bits_operation_system.ToString());
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/EditLaptop/5
        public ActionResult EditLaptop(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
                if (iT_inventory_items == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si el item pertenece a tipo laptop
                //busca el tipo inventario para laptop
                var typeLaptop = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("laptop"));
                if (typeLaptop == null || iT_inventory_items.id_inventory_type != typeLaptop.id)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                    ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                    return View("../Home/ErrorGenerico");
                }

                ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.bits_operation_system.ToString());
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
                return View(iT_inventory_items);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_items/EditLaptop/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLaptop(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            List<IT_inventory_hard_drives> drives = new List<IT_inventory_hard_drives>();
            iT_inventory_items.IT_inventory_hard_drives = drives; //vacia la lista que viene del form (solo detecta los agregados con js)

            #region obtiene y valida drives

            //obtiene los drives del formcollection
            foreach (string key in collection.AllKeys.Where(x => x.StartsWith("IT_inventory_hard_drives") && x.EndsWith(".disk_name")))
            {
                int index = -1;
                int total_drive_space_mb = 0;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    //obtiene el index
                    int.TryParse(m.Value, out index);

                    //obtiene los valores del form para el index asociado
                    string disk_name = collection["IT_inventory_hard_drives[" + index + "].disk_name"].ToUpper();
                    string type_drive = collection["IT_inventory_hard_drives[" + index + "].type_drive"];
                    int.TryParse(collection["IT_inventory_hard_drives[" + index + "].total_drive_space_mb"], out total_drive_space_mb);

                    //agrega el drive
                    drives.Add(
                        new IT_inventory_hard_drives
                        {
                            id_inventory_item = iT_inventory_items.id,
                            disk_name = disk_name,
                            type_drive = type_drive,
                            total_drive_space_mb = total_drive_space_mb,
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

                //agrega los valores de redireccionamiento
                List<string> keys = collection.AllKeys.ToList();

                string string_id_planta = String.Empty;
                string string_pagina = String.Empty;
                string string_hostname = String.Empty;
                string string_active = String.Empty;

                if (keys.Contains("_id_planta"))
                    string_id_planta = collection["_id_planta"];
                if (keys.Contains("_pagina"))
                    string_pagina = collection["_pagina"];
                if (keys.Contains("_hostname"))
                    string_hostname = collection["_hostname"];
                if (keys.Contains("_active"))
                    string_active = collection["_active"];

                return RedirectToAction("laptop", new { id_planta = string_id_planta, pagina = string_pagina, hostname = string_hostname, active = string_active });
            }
            //busca el tipo inventario para laptop
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("laptop"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            iT_inventory_items.IT_inventory_hardware_type = type;
            ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.bits_operation_system.ToString());
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        #endregion

        #region Monitor 
        // GET: IT_inventory_items/monitor
        public ActionResult monitor(int? id_planta, string model, bool? active, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

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
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_items
                      .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_planta"] = id_planta;
                routeValues["model"] = model;
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
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());


                return View(listado);
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

        // GET: IT_inventory_items/Detailsmonitor/5
        public ActionResult Detailsmonitor(int? id)
        {
            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            //verifica si el item pertenece a tipo monitor
            //busca el tipo inventario para monitor
            var typemonitor = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("monitor"));
            if (typemonitor == null || iT_inventory_items.id_inventory_type != typemonitor.id)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                return View("../Home/ErrorGenerico");
            }


            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Createmonitor
        public ActionResult Createmonitor()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //busca el tipo inventario para monitor
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("monitor"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                ViewBag.type = type;
                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(new IT_inventory_items { active = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_items/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Createmonitor(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                db.IT_inventory_items.Add(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("monitor");
            }
            //busca el tipo inventario para monitor
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("monitor"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Editmonitor/5
        public ActionResult Editmonitor(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
                if (iT_inventory_items == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si el item pertenece a tipo monitor
                //busca el tipo inventario para monitor
                var typemonitor = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("monitor"));
                if (typemonitor == null || iT_inventory_items.id_inventory_type != typemonitor.id)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                    ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                    return View("../Home/ErrorGenerico");
                }

                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
                return View(iT_inventory_items);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_items/Editmonitor/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editmonitor(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                // Activity already exist in database and modify it
                db.Entry(db.IT_inventory_items.Find(iT_inventory_items.id)).CurrentValues.SetValues(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                //agrega los valores de redireccionamiento
                List<string> keys = collection.AllKeys.ToList();

                string string_id_planta = String.Empty;
                string string_pagina = String.Empty;
                string string_model = String.Empty;
                string string_active = String.Empty;

                if (keys.Contains("_id_planta"))
                    string_id_planta = collection["_id_planta"];
                if (keys.Contains("_pagina"))
                    string_pagina = collection["_pagina"];
                if (keys.Contains("_model"))
                    string_model = collection["_model"];
                if (keys.Contains("_active"))
                    string_active = collection["_active"];

                return RedirectToAction("monitor", new { id_planta = string_id_planta, pagina = string_pagina, model = string_model, active = string_active });
            }
            //busca el tipo inventario para monitor
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("monitor"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            iT_inventory_items.IT_inventory_hardware_type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        #endregion

        #region printer 
        // GET: IT_inventory_items/printer
        public ActionResult printer(int? id_planta, string model, bool? active, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

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
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_items
                      .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_planta"] = id_planta;
                routeValues["model"] = model;
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
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());


                return View(listado);
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

        // GET: IT_inventory_items/Detailsprinter/5
        public ActionResult Detailsprinter(int? id)
        {
            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            //verifica si el item pertenece a tipo printer
            //busca el tipo inventario para printer
            var typeprinter = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("printer"));
            if (typeprinter == null || iT_inventory_items.id_inventory_type != typeprinter.id)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                return View("../Home/ErrorGenerico");
            }


            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Createprinter
        public ActionResult Createprinter()
        {

            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //busca el tipo inventario para printer
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("printer"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                ViewBag.type = type;
                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(new IT_inventory_items { active = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_items/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Createprinter(IT_inventory_items iT_inventory_items, FormCollection collection)
        {

            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                db.IT_inventory_items.Add(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("printer");
            }
            //busca el tipo inventario para printer
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("printer"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Editprinter/5
        public ActionResult Editprinter(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
                if (iT_inventory_items == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si el item pertenece a tipo printer
                //busca el tipo inventario para printer
                var typeprinter = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("printer"));
                if (typeprinter == null || iT_inventory_items.id_inventory_type != typeprinter.id)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                    ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                    return View("../Home/ErrorGenerico");
                }

                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
                return View(iT_inventory_items);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_items/Editprinter/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editprinter(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                // Activity already exist in database and modify it
                db.Entry(db.IT_inventory_items.Find(iT_inventory_items.id)).CurrentValues.SetValues(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                //agrega los valores de redireccionamiento
                List<string> keys = collection.AllKeys.ToList();

                string string_id_planta = String.Empty;
                string string_pagina = String.Empty;
                string string_model = String.Empty;
                string string_active = String.Empty;

                if (keys.Contains("_id_planta"))
                    string_id_planta = collection["_id_planta"];
                if (keys.Contains("_pagina"))
                    string_pagina = collection["_pagina"];
                if (keys.Contains("_model"))
                    string_model = collection["_model"];
                if (keys.Contains("_active"))
                    string_active = collection["_active"];

                return RedirectToAction("printer", new { id_planta = string_id_planta, pagina = string_pagina, model = string_model, active = string_active });
            }
            //busca el tipo inventario para printer
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("printer"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            iT_inventory_items.IT_inventory_hardware_type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        #endregion

        #region label_printer 
        // GET: IT_inventory_items/label_printer
        public ActionResult label_printer(int? id_planta, string model, bool? active, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

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
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_items
                      .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_planta"] = id_planta;
                routeValues["model"] = model;
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
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());


                return View(listado);
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

        // GET: IT_inventory_items/Detailslabel_printer/5
        public ActionResult Detailslabel_printer(int? id)
        {
            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            //verifica si el item pertenece a tipo label_printer
            //busca el tipo inventario para label_printer
            var typelabel_printer = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("label") && x.descripcion.Contains("printer"));
            if (typelabel_printer == null || iT_inventory_items.id_inventory_type != typelabel_printer.id)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                return View("../Home/ErrorGenerico");
            }


            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Createlabel_printer
        public ActionResult Createlabel_printer()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //busca el tipo inventario para label_printer
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("label") && x.descripcion.Contains("printer"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                ViewBag.type = type;
                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(new IT_inventory_items { active = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_items/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Createlabel_printer(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                db.IT_inventory_items.Add(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("label_printer");
            }
            //busca el tipo inventario para label_printer
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("label") && x.descripcion.Contains("printer"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Editlabel_printer/5
        public ActionResult Editlabel_printer(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
                if (iT_inventory_items == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si el item pertenece a tipo label_printer
                //busca el tipo inventario para label_printer
                var typelabel_printer = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("label") && x.descripcion.Contains("printer"));
                if (typelabel_printer == null || iT_inventory_items.id_inventory_type != typelabel_printer.id)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                    ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                    return View("../Home/ErrorGenerico");
                }

                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
                return View(iT_inventory_items);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_items/Editlabel_printer/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editlabel_printer(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {


                // Activity already exist in database and modify it
                db.Entry(db.IT_inventory_items.Find(iT_inventory_items.id)).CurrentValues.SetValues(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                //agrega los valores de redireccionamiento
                List<string> keys = collection.AllKeys.ToList();

                string string_id_planta = String.Empty;
                string string_pagina = String.Empty;
                string string_model = String.Empty;
                string string_active = String.Empty;

                if (keys.Contains("_id_planta"))
                    string_id_planta = collection["_id_planta"];
                if (keys.Contains("_pagina"))
                    string_pagina = collection["_pagina"];
                if (keys.Contains("_model"))
                    string_model = collection["_model"];
                if (keys.Contains("_active"))
                    string_active = collection["_active"];

                return RedirectToAction("label_printer", new { id_planta = string_id_planta, pagina = string_pagina, model = string_model, active = string_active });
            }
            //busca el tipo inventario para label_printer
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("label") && x.descripcion.Contains("printer"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            iT_inventory_items.IT_inventory_hardware_type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        #endregion

        #region pda 
        // GET: IT_inventory_items/pda
        public ActionResult pda(int? id_planta, string model, bool? active, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

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
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_items
                      .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_planta"] = id_planta;
                routeValues["model"] = model;
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
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());


                return View(listado);
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

        // GET: IT_inventory_items/Detailspda/5
        public ActionResult Detailspda(int? id)
        {
            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            //verifica si el item pertenece a tipo pda
            //busca el tipo inventario para pda
            var typepda = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("pda"));
            if (typepda == null || iT_inventory_items.id_inventory_type != typepda.id)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                return View("../Home/ErrorGenerico");
            }


            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Createpda
        public ActionResult Createpda()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //busca el tipo inventario para pda
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("pda"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                ViewBag.type = type;
                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(new IT_inventory_items { active = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_items/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Createpda(IT_inventory_items iT_inventory_items, FormCollection collection)
        {

            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                db.IT_inventory_items.Add(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("pda");
            }
            //busca el tipo inventario para pda
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("pda"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Editpda/5
        public ActionResult Editpda(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
                if (iT_inventory_items == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si el item pertenece a tipo pda
                //busca el tipo inventario para pda
                var typepda = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("pda"));
                if (typepda == null || iT_inventory_items.id_inventory_type != typepda.id)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                    ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                    return View("../Home/ErrorGenerico");
                }

                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
                return View(iT_inventory_items);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_items/Editpda/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editpda(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                // Activity already exist in database and modify it
                db.Entry(db.IT_inventory_items.Find(iT_inventory_items.id)).CurrentValues.SetValues(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                //agrega los valores de redireccionamiento
                List<string> keys = collection.AllKeys.ToList();

                string string_id_planta = String.Empty;
                string string_pagina = String.Empty;
                string string_model = String.Empty;
                string string_active = String.Empty;

                if (keys.Contains("_id_planta"))
                    string_id_planta = collection["_id_planta"];
                if (keys.Contains("_pagina"))
                    string_pagina = collection["_pagina"];
                if (keys.Contains("_model"))
                    string_model = collection["_model"];
                if (keys.Contains("_active"))
                    string_active = collection["_active"];

                return RedirectToAction("pda", new { id_planta = string_id_planta, pagina = string_pagina, model = string_model, active = string_active });
            }
            //busca el tipo inventario para pda
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("pda"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            iT_inventory_items.IT_inventory_hardware_type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        #endregion

        #region scanner 
        // GET: IT_inventory_items/scanner
        public ActionResult scanner(int? id_planta, string model, bool? active, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

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
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_items
                      .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_planta"] = id_planta;
                routeValues["model"] = model;
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
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());


                return View(listado);
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

        // GET: IT_inventory_items/Detailsscanner/5
        public ActionResult Detailsscanner(int? id)
        {
            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            //verifica si el item pertenece a tipo scanner
            //busca el tipo inventario para scanner
            var typescanner = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("scanner"));
            if (typescanner == null || iT_inventory_items.id_inventory_type != typescanner.id)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                return View("../Home/ErrorGenerico");
            }


            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Createscanner
        public ActionResult Createscanner()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //busca el tipo inventario para scanner
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("scanner"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                ViewBag.type = type;
                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(new IT_inventory_items { active = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_items/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Createscanner(IT_inventory_items iT_inventory_items, FormCollection collection)
        {

            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                db.IT_inventory_items.Add(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("scanner");
            }
            //busca el tipo inventario para scanner
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("scanner"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Editscanner/5
        public ActionResult Editscanner(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
                if (iT_inventory_items == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si el item pertenece a tipo scanner
                //busca el tipo inventario para scanner
                var typescanner = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("scanner"));
                if (typescanner == null || iT_inventory_items.id_inventory_type != typescanner.id)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                    ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                    return View("../Home/ErrorGenerico");
                }

                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
                return View(iT_inventory_items);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_items/Editscanner/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editscanner(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                // Activity already exist in database and modify it
                db.Entry(db.IT_inventory_items.Find(iT_inventory_items.id)).CurrentValues.SetValues(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                //agrega los valores de redireccionamiento
                List<string> keys = collection.AllKeys.ToList();

                string string_id_planta = String.Empty;
                string string_pagina = String.Empty;
                string string_model = String.Empty;
                string string_active = String.Empty;

                if (keys.Contains("_id_planta"))
                    string_id_planta = collection["_id_planta"];
                if (keys.Contains("_pagina"))
                    string_pagina = collection["_pagina"];
                if (keys.Contains("_model"))
                    string_model = collection["_model"];
                if (keys.Contains("_active"))
                    string_active = collection["_active"];

                return RedirectToAction("scanner", new { id_planta = string_id_planta, pagina = string_pagina, model = string_model, active = string_active });
            }
            //busca el tipo inventario para scanner
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("scanner"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            iT_inventory_items.IT_inventory_hardware_type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        #endregion

        #region server 
        // GET: IT_inventory_items/server
        public ActionResult server(int? id_planta, string hostname, bool? active, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

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
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_items
                      .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.hostname.Contains(hostname) || String.IsNullOrEmpty(hostname))
                    && (x.active == active || active == null)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_planta"] = id_planta;
                routeValues["hostname"] = hostname;
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
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());


                return View(listado);
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

        // GET: IT_inventory_items/Detailsserver/5
        public ActionResult Detailsserver(int? id)
        {
            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            //verifica si el item pertenece a tipo server
            //busca el tipo inventario para server
            var typeserver = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("server"));
            if (typeserver == null || iT_inventory_items.id_inventory_type != typeserver.id)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                return View("../Home/ErrorGenerico");
            }


            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Createserver
        public ActionResult Createserver()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //busca el tipo inventario para server
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("server"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                ViewBag.type = type;
                ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --");
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(new IT_inventory_items { active = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_items/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Createserver(IT_inventory_items iT_inventory_items, FormCollection collection)
        {

            List<IT_inventory_hard_drives> drives = new List<IT_inventory_hard_drives>();
            iT_inventory_items.IT_inventory_hard_drives = drives; //vacia la lista que viene del form (solo detecta los agregados con js)

            #region obtiene y valida drives

            

            //obtiene los drives del formcollection
            foreach (string key in collection.AllKeys.Where(x => x.StartsWith("IT_inventory_hard_drives") && x.EndsWith(".disk_name")))
            {
                int index = -1;
                int total_drive_space_mb = 0;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    //obtiene el index
                    int.TryParse(m.Value, out index);

                    //obtiene los valores del form para el index asociado
                    string disk_name = collection["IT_inventory_hard_drives[" + index + "].disk_name"].ToUpper();
                    string type_drive = collection["IT_inventory_hard_drives[" + index + "].type_drive"];
                    int.TryParse(collection["IT_inventory_hard_drives[" + index + "].total_drive_space_mb"], out total_drive_space_mb);

                    //agrega el drive
                    drives.Add(
                        new IT_inventory_hard_drives
                        {
                            disk_name = disk_name,
                            type_drive = type_drive,
                            total_drive_space_mb = total_drive_space_mb,
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

                return RedirectToAction("server");
            }
            //busca el tipo inventario para server
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("server"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.bits_operation_system.ToString());
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Editserver/5
        public ActionResult Editserver(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
                if (iT_inventory_items == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si el item pertenece a tipo server
                //busca el tipo inventario para server
                var typeserver = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("server"));
                if (typeserver == null || iT_inventory_items.id_inventory_type != typeserver.id)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                    ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                    return View("../Home/ErrorGenerico");
                }

                ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.bits_operation_system.ToString());
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
                return View(iT_inventory_items);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_items/Editserver/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editserver(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            List<IT_inventory_hard_drives> drives = new List<IT_inventory_hard_drives>();
            iT_inventory_items.IT_inventory_hard_drives = drives; //vacia la lista que viene del form (solo detecta los agregados con js)

            #region obtiene y valida drives

            //obtiene los drives del formcollection
            foreach (string key in collection.AllKeys.Where(x => x.StartsWith("IT_inventory_hard_drives") && x.EndsWith(".disk_name")))
            {
                int index = -1;
                int total_drive_space_mb = 0;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    //obtiene el index
                    int.TryParse(m.Value, out index);

                    //obtiene los valores del form para el index asociado
                    string disk_name = collection["IT_inventory_hard_drives[" + index + "].disk_name"].ToUpper();
                    string type_drive = collection["IT_inventory_hard_drives[" + index + "].type_drive"];
                    int.TryParse(collection["IT_inventory_hard_drives[" + index + "].total_drive_space_mb"], out total_drive_space_mb);

                    //agrega el drive
                    drives.Add(
                        new IT_inventory_hard_drives
                        {
                            id_inventory_item = iT_inventory_items.id,
                            disk_name = disk_name,
                            type_drive = type_drive,
                            total_drive_space_mb = total_drive_space_mb
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

                //agrega los valores de redireccionamiento
                List<string> keys = collection.AllKeys.ToList();

                string string_id_planta = String.Empty;
                string string_pagina = String.Empty;
                string string_hostname = String.Empty;
                string string_active = String.Empty;

                if (keys.Contains("_id_planta"))
                    string_id_planta = collection["_id_planta"];
                if (keys.Contains("_pagina"))
                    string_pagina = collection["_pagina"];
                if (keys.Contains("_hostname"))
                    string_hostname = collection["_hostname"];
                if (keys.Contains("_active"))
                    string_active = collection["_active"];

                return RedirectToAction("server", new { id_planta = string_id_planta, pagina = string_pagina, hostname = string_hostname, active = string_active });
            }
            //busca el tipo inventario para server
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("server"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            iT_inventory_items.IT_inventory_hardware_type = type;
            ViewBag.bits_operation_system = AddFirstItem(SelectBitsOS(), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.bits_operation_system.ToString());
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        #endregion

        #region tablet 
        // GET: IT_inventory_items/tablet
        public ActionResult tablet(int? id_planta, string model, bool? active, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

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
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_items
                      .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_planta"] = id_planta;
                routeValues["model"] = model;
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
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());


                return View(listado);
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

        // GET: IT_inventory_items/Detailstablet/5
        public ActionResult Detailstablet(int? id)
        {
            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            //verifica si el item pertenece a tipo tablet
            //busca el tipo inventario para tablet
            var typetablet = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("tablet"));
            if (typetablet == null || iT_inventory_items.id_inventory_type != typetablet.id)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                return View("../Home/ErrorGenerico");
            }


            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Createtablet
        public ActionResult Createtablet()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //busca el tipo inventario para tablet
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("tablet"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                ViewBag.type = type;
                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(new IT_inventory_items { active = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_items/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Createtablet(IT_inventory_items iT_inventory_items, FormCollection collection)
        {

            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");


            if (ModelState.IsValid)
            {

                db.IT_inventory_items.Add(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("tablet");
            }
            //busca el tipo inventario para tablet
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("tablet"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Edittablet/5
        public ActionResult Edittablet(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
                if (iT_inventory_items == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si el item pertenece a tipo tablet
                //busca el tipo inventario para tablet
                var typetablet = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("tablet"));
                if (typetablet == null || iT_inventory_items.id_inventory_type != typetablet.id)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                    ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                    return View("../Home/ErrorGenerico");
                }

                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
                return View(iT_inventory_items);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_items/Edittablet/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edittablet(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {
                // Activity already exist in database and modify it
                db.Entry(db.IT_inventory_items.Find(iT_inventory_items.id)).CurrentValues.SetValues(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                //agrega los valores de redireccionamiento
                List<string> keys = collection.AllKeys.ToList();

                string string_id_planta = String.Empty;
                string string_pagina = String.Empty;
                string string_model = String.Empty;
                string string_active = String.Empty;

                if (keys.Contains("_id_planta"))
                    string_id_planta = collection["_id_planta"];
                if (keys.Contains("_pagina"))
                    string_pagina = collection["_pagina"];
                if (keys.Contains("_model"))
                    string_model = collection["_model"];
                if (keys.Contains("_active"))
                    string_active = collection["_active"];

                return RedirectToAction("tablet", new { id_planta = string_id_planta, pagina = string_pagina, model = string_model, active = string_active });
            }
            //busca el tipo inventario para tablet
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("tablet"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            iT_inventory_items.IT_inventory_hardware_type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        #endregion

        #region radio 
        // GET: IT_inventory_items/radio
        public ActionResult radio(int? id_planta, string model, bool? active, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

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
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_items
                      .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_planta"] = id_planta;
                routeValues["model"] = model;
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
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());


                return View(listado);
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

        // GET: IT_inventory_items/Detailsradio/5
        public ActionResult Detailsradio(int? id)
        {
            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            //verifica si el item pertenece a tipo radio
            //busca el tipo inventario para radio
            var typeradio = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("radio"));
            if (typeradio == null || iT_inventory_items.id_inventory_type != typeradio.id)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                return View("../Home/ErrorGenerico");
            }


            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Createradio
        public ActionResult Createradio()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //busca el tipo inventario para radio
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("radio"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                ViewBag.type = type;
                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(new IT_inventory_items { active = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_items/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Createradio(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                db.IT_inventory_items.Add(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("radio");
            }
            //busca el tipo inventario para radio
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("radio"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Editradio/5
        public ActionResult Editradio(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
                if (iT_inventory_items == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si el item pertenece a tipo radio
                //busca el tipo inventario para radio
                var typeradio = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("radio"));
                if (typeradio == null || iT_inventory_items.id_inventory_type != typeradio.id)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                    ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                    return View("../Home/ErrorGenerico");
                }

                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
                return View(iT_inventory_items);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_items/Editradio/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editradio(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {


                // Activity already exist in database and modify it
                db.Entry(db.IT_inventory_items.Find(iT_inventory_items.id)).CurrentValues.SetValues(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                //agrega los valores de redireccionamiento
                List<string> keys = collection.AllKeys.ToList();

                string string_id_planta = String.Empty;
                string string_pagina = String.Empty;
                string string_model = String.Empty;
                string string_active = String.Empty;

                if (keys.Contains("_id_planta"))
                    string_id_planta = collection["_id_planta"];
                if (keys.Contains("_pagina"))
                    string_pagina = collection["_pagina"];
                if (keys.Contains("_model"))
                    string_model = collection["_model"];
                if (keys.Contains("_active"))
                    string_active = collection["_active"];

                return RedirectToAction("radio", new { id_planta = string_id_planta, pagina = string_pagina, model = string_model, active = string_active });
            }
            //busca el tipo inventario para radio
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("radio"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            iT_inventory_items.IT_inventory_hardware_type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        #endregion

        #region ap 
        // GET: IT_inventory_items/ap
        public ActionResult ap(int? id_planta, string model, bool? active, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

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
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_items
                      .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_planta"] = id_planta;
                routeValues["model"] = model;
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
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());


                return View(listado);
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

        // GET: IT_inventory_items/Detailsap/5
        public ActionResult Detailsap(int? id)
        {
            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            //verifica si el item pertenece a tipo ap
            //busca el tipo inventario para ap
            var typeap = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.StartsWith("ap"));
            if (typeap == null || iT_inventory_items.id_inventory_type != typeap.id)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                return View("../Home/ErrorGenerico");
            }


            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Createap
        public ActionResult Createap()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //busca el tipo inventario para ap
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.StartsWith("ap"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                ViewBag.type = type;
                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(new IT_inventory_items { active = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_items/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Createap(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica que no exista el hostname
            if (db.IT_inventory_items.Any(x => x.hostname == iT_inventory_items.hostname && !String.IsNullOrEmpty(x.hostname) && x.id_inventory_type == iT_inventory_items.id_inventory_type && x.active == true))
                ModelState.AddModelError("", "An active record with the same hostname already exists.");

            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                db.IT_inventory_items.Add(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("ap");
            }
            //busca el tipo inventario para ap
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.StartsWith("ap"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Editap/5
        public ActionResult Editap(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
                if (iT_inventory_items == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si el item pertenece a tipo ap
                //busca el tipo inventario para ap
                var typeap = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.StartsWith("ap"));
                if (typeap == null || iT_inventory_items.id_inventory_type != typeap.id)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                    ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                    return View("../Home/ErrorGenerico");
                }

                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
                return View(iT_inventory_items);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_items/Editap/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editap(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica que no exista el hostname
            if (db.IT_inventory_items.Any(x => x.hostname == iT_inventory_items.hostname && !String.IsNullOrEmpty(x.hostname) && x.id_inventory_type == iT_inventory_items.id_inventory_type && x.id != iT_inventory_items.id && x.active == true))
                ModelState.AddModelError("", "An active record with the same hostname already exists.");

            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                // Activity already exist in database and modify it
                db.Entry(db.IT_inventory_items.Find(iT_inventory_items.id)).CurrentValues.SetValues(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                //agrega los valores de redireccionamiento
                List<string> keys = collection.AllKeys.ToList();

                string string_id_planta = String.Empty;
                string string_pagina = String.Empty;
                string string_model = String.Empty;
                string string_active = String.Empty;

                if (keys.Contains("_id_planta"))
                    string_id_planta = collection["_id_planta"];
                if (keys.Contains("_pagina"))
                    string_pagina = collection["_pagina"];
                if (keys.Contains("_model"))
                    string_model = collection["_model"];
                if (keys.Contains("_active"))
                    string_active = collection["_active"];

                return RedirectToAction("ap", new { id_planta = string_id_planta, pagina = string_pagina, model = string_model, active = string_active });
            }
            //busca el tipo inventario para ap
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.StartsWith("ap"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            iT_inventory_items.IT_inventory_hardware_type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        #endregion

        #region smartphone 
        // GET: IT_inventory_items/smartphone
        public ActionResult smartphone(int? id_planta, string model, bool? active, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

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
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_items
                      .Where(x =>
                    x.id_inventory_type == type.id
                    && (x.id_planta == id_planta || id_planta == null)
                    && (x.model.Contains(model) || String.IsNullOrEmpty(model))
                    && (x.active == active || active == null)
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_planta"] = id_planta;
                routeValues["model"] = model;
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
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), textoPorDefecto: "-- All --", selected: id_planta.ToString());


                return View(listado);
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

        // GET: IT_inventory_items/Detailssmartphone/5
        public ActionResult Detailssmartphone(int? id)
        {
            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
            if (iT_inventory_items == null)
            {
                return View("../Error/NotFound");
            }

            //verifica si el item pertenece a tipo smartphone
            //busca el tipo inventario para smartphone
            var typesmartphone = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("smartphone"));
            if (typesmartphone == null || iT_inventory_items.id_inventory_type != typesmartphone.id)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                return View("../Home/ErrorGenerico");
            }


            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Createsmartphone
        public ActionResult Createsmartphone()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //busca el tipo inventario para smartphone
                var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("smartphone"));

                //si es nulo inicializa un objeto vacio
                if (type == null)
                    type = new IT_inventory_hardware_type();

                ViewBag.type = type;
                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(new IT_inventory_items { active = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_items/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Createsmartphone(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                db.IT_inventory_items.Add(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("smartphone");
            }
            //busca el tipo inventario para smartphone
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("smartphone"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            ViewBag.type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        // GET: IT_inventory_items/Editsmartphone/5
        public ActionResult Editsmartphone(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
                if (iT_inventory_items == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si el item pertenece a tipo smartphone
                //busca el tipo inventario para smartphone
                var typesmartphone = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("smartphone"));
                if (typesmartphone == null || iT_inventory_items.id_inventory_type != typesmartphone.id)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se ver el detalle de este elemento!";
                    ViewBag.Descripcion = "El tipo de Hardware no corresponde con el de la vista solicitada.";

                    return View("../Home/ErrorGenerico");
                }

                //agregar --firtsitem
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --", selected: iT_inventory_items.id_planta.ToString());
                return View(iT_inventory_items);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_items/Editsmartphone/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editsmartphone(IT_inventory_items iT_inventory_items, FormCollection collection)
        {
            //verifica purchasedate sea menor a end warranty
            if (iT_inventory_items.purchase_date.HasValue && iT_inventory_items.end_warranty.HasValue && iT_inventory_items.purchase_date > iT_inventory_items.end_warranty)
                ModelState.AddModelError("", "End Warranty must be greater than Purchase Date.");

            if (ModelState.IsValid)
            {

                // Activity already exist in database and modify it
                db.Entry(db.IT_inventory_items.Find(iT_inventory_items.id)).CurrentValues.SetValues(iT_inventory_items);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                //agrega los valores de redireccionamiento
                List<string> keys = collection.AllKeys.ToList();

                string string_id_planta = String.Empty;
                string string_pagina = String.Empty;
                string string_model = String.Empty;
                string string_active = String.Empty;

                if (keys.Contains("_id_planta"))
                    string_id_planta = collection["_id_planta"];
                if (keys.Contains("_pagina"))
                    string_pagina = collection["_pagina"];
                if (keys.Contains("_model"))
                    string_model = collection["_model"];
                if (keys.Contains("_active"))
                    string_active = collection["_active"];

                return RedirectToAction("smartphone", new { id_planta = string_id_planta, pagina = string_pagina, model = string_model, active = string_active });
            }
            //busca el tipo inventario para smartphone
            var type = db.IT_inventory_hardware_type.FirstOrDefault(x => x.descripcion.Contains("smartphone"));

            //si es nulo inicializa un objeto vacio
            if (type == null)
                type = new IT_inventory_hardware_type();

            iT_inventory_items.IT_inventory_hardware_type = type;
            //agregar --firtsitem
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, "clave", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            return View(iT_inventory_items);
        }

        #endregion


        //// GET: IT_inventory_items/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
        //    if (iT_inventory_items == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(iT_inventory_items);
        //}

        //// POST: IT_inventory_items/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    IT_inventory_items iT_inventory_items = db.IT_inventory_items.Find(id);
        //    db.IT_inventory_items.Remove(iT_inventory_items);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
