﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;
using Portal_2_0.Models.PDFHandlers;
using System.Text.RegularExpressions;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_asignacion_hardwareController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_asignacion_hardware
        public ActionResult Index(string nombre, string num_empleado, int planta_clave = 0, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.IT_ASIGNACION_HARDWARE))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var listado = db.empleados
                   .Where(x =>
                   ((x.nombre + " " + x.apellido1 + " " + x.apellido2).Contains(nombre) || String.IsNullOrEmpty(nombre))
                   && (x.numeroEmpleado.Contains(num_empleado) || String.IsNullOrEmpty(num_empleado))
                   && (x.planta_clave == planta_clave || planta_clave == 0)
                   && x.activo == true
                   )
                   .OrderBy(x => x.id)
                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.empleados
                  .Where(x =>
                    ((x.nombre + " " + x.apellido1 + " " + x.apellido2).Contains(nombre) || String.IsNullOrEmpty(nombre))
                      && (x.numeroEmpleado.Contains(num_empleado) || String.IsNullOrEmpty(num_empleado))
                   && (x.planta_clave == planta_clave || planta_clave == 0)
                   && x.activo == true
                   )
                .Count();

            //para paginación

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["nombre"] = nombre;
            routeValues["planta_clave"] = planta_clave;
            routeValues["num_empleado"] = num_empleado;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.planta_clave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", planta_clave.ToString()), textoPorDefecto: "-- Todas --");
            ViewBag.Paginacion = paginacion;

            return View(listado);
        }

        // GET: IT_asignacion_hardware/ListadoAsignaciones/5
        public ActionResult ListadoAsignaciones(int? id)
        {
            if (!TieneRol(TipoRoles.IT_ASIGNACION_HARDWARE))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            empleados empleado = db.empleados.Find(id);
            if (empleado == null)
            {
                return View("../Error/NotFound");
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            //envia un list con todos los tipos de hardware
            ViewBag.TiposHardware = db.IT_inventory_hardware_type.ToList();
            ViewBag.id_tipo_hardware = AddFirstItem(new SelectList(db.IT_inventory_hardware_type.Where(x => x.activo && x.puede_asignarse), "id", "descripcion"), textoPorDefecto: "-- Seleccione un valor --");

            return View(empleado);
        }

        // GET: IT_asignacion_hardware/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.IT_ASIGNACION_HARDWARE))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_asignacion_hardware iT_asignacion_hardware = db.IT_asignacion_hardware.Find(id);
            if (iT_asignacion_hardware == null)
            {
                return HttpNotFound();
            }
            return View(iT_asignacion_hardware);
        }

        // GET: IT_asignacion_hardware/DetailsEmpleado/5
        public ActionResult DetailsEmpleado(int? id)
        {
            if (!TieneRol(TipoRoles.IT_ASIGNACION_HARDWARE))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            empleados empleado = db.empleados.Find(id);
            if (empleado == null)
            {
                return View("../Error/NotFound");
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            //envia un list con todos los tipos de hardware
            ViewBag.TiposHardware = db.IT_inventory_hardware_type.ToList();

            return View(empleado);
        }

        // GET: IT_asignacion_hardware/Asignar
        public ActionResult Asignar(int? id_empleado, int? id_tipo_hardware)
        {

            if (!TieneRol(TipoRoles.IT_ASIGNACION_HARDWARE))
                return View("../Home/ErrorPermisos");

            if (id_empleado == null || id_tipo_hardware == null)
            {
                return View("../Error/BadRequest");
            }
            empleados empleado = db.empleados.Find(id_empleado);
            if (empleado == null)
            {
                return View("../Error/NotFound");
            }
            IT_inventory_hardware_type tipoHardware = db.IT_inventory_hardware_type.Find(id_tipo_hardware);
            if (empleado == null)
            {
                return View("../Error/NotFound");
            }

            //obtiene el modelo y establece propiedades
            IT_asignacion_hardware model = new IT_asignacion_hardware();

            empleados sistemas = obtieneEmpleadoLogeado();
            model.id_empleado = empleado.id;
            model.empleados = empleado;
            model.id_sistemas = sistemas.id;
            model.empleados2 = sistemas;
            model.fecha_asignacion = DateTime.Now;
            model.es_asignacion_actual = true;
            model.tipo_hardware = id_tipo_hardware.Value;


            string campoSelect = String.Empty;

            switch (tipoHardware.descripcion)
            {
                case Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES:
                    campoSelect = nameof(IT_inventory_items.ConcatAccesoriesInfo);
                    ViewBag.Multiple = true; //si se trata de accesorios se pueden enviar varios hardware
                    break;
                default:
                    //si no se trata de accesorio limita a un sólo registro
                    ViewBag.Multiple = false;
                    campoSelect = nameof(IT_inventory_items.ConcatInfoGeneral);
                    model.IT_asignacion_hardware_rel_items.Add(new IT_asignacion_hardware_rel_items
                    {
                        id_it_inventory_item = 0
                    });
                    break;
            }


            //genera el select list para hardware activas
            //ViewBag.id_it_inventory_item = AddFirstItem(new SelectList(
            //    db.IT_inventory_items
            //    .Where(x => x.id_inventory_type == id_tipo_hardware && x.active == true)
            //    , "id", campoSelect));

            ViewBag.ListadoHardwareInventario = db.IT_inventory_items.Where(x => x.id_inventory_type == id_tipo_hardware && x.active == true).ToList();
            ViewBag.ListadoHardwareGenerico = db.IT_inventory_items_genericos.Where(x => x.id_inventory_type == id_tipo_hardware && x.active == true).ToList();
            ViewBag.ListadoTiposAccesorios = db.IT_inventory_tipos_accesorios.ToList();

            //obtiene la primera linea asignada
            int numcel = 0;

            var linea = empleado.GetIT_Inventory_Cellular_LinesActivas().FirstOrDefault();
            if (linea != null)
                numcel = linea.id;

            //envia el tipo de hardware
            ViewBag.TipoHardware = tipoHardware;
            ViewBag.tipo_hardware = AddFirstItem(new SelectList(db.IT_inventory_hardware_type.Where(x => x.activo), "id", "descripcion"), textoPorDefecto: "-- Select --", selected: id_tipo_hardware.ToString());
            ViewBag.id_cellular_line = AddFirstItem(new SelectList(db.IT_inventory_cellular_line.Where(x => x.id_planta == empleado.planta_clave), "id", "ConcatDetalles"), textoPorDefecto: "-- Seleccionar --", selected: numcel.ToString());


            return View(model);
        }

        // POST: IT_asignacion_hardware/Asignar
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Asignar(IT_asignacion_hardware iT_asignacion_hardware, FormCollection collection)
        {

            //primero contrulle los objetos desde el form collection
            //crea lista vacia
            List<IT_asignacion_hardware_rel_items> listaItems = new List<IT_asignacion_hardware_rel_items>();

            //lee los datos del collection
            foreach (string key in collection.AllKeys.Where(x => x.StartsWith("es_de_inventario_")))
            {
                int index = -1;

                int? id_it_inventory_item = null;
                int id_inventory = 0;

                int? id_it_inventory_item_generico = null;
                int id_inventory_generico = 0;

                string comentarios = null;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    int.TryParse(m.Value, out index);

                    int.TryParse(collection["hardware_inventario_" + index], out id_inventory);
                    int.TryParse(collection["hardware_generico_" + index], out id_inventory_generico);
                    comentarios = collection["comentario_" + index];

                    if (id_inventory > 0)
                        id_it_inventory_item = id_inventory;
                    else
                        id_it_inventory_item = null;

                    if (id_inventory_generico > 0)
                        id_it_inventory_item_generico = id_inventory_generico;
                    else
                        id_it_inventory_item_generico = null;

                    listaItems.Add(
                        new IT_asignacion_hardware_rel_items
                        {
                            id_it_inventory_item = id_it_inventory_item,
                            id_it_inventory_generico = id_it_inventory_item_generico,
                            comments = comentarios,
                        }
                    );
                }
            }

            //asocia los items obtenidos con el modelo
            iT_asignacion_hardware.IT_asignacion_hardware_rel_items = listaItems;

            //verifica que este ninguno de los equipos se encuentre ya asignado al usuario actual
            foreach (var item in listaItems)
            {
                if (db.IT_asignacion_hardware_rel_items.Any(x => x.id_it_inventory_item == item.id_it_inventory_item
                && x.id_it_inventory_generico == item.id_it_inventory_generico && iT_asignacion_hardware.es_asignacion_actual == true
                && x.IT_asignacion_hardware.es_asignacion_actual == true && x.IT_asignacion_hardware.id_empleado == iT_asignacion_hardware.id_empleado))
                {

                    var item_inventory = db.IT_inventory_items.Find(item.id_it_inventory_item);
                    var item_generico = db.IT_inventory_items_genericos.Find(item.id_it_inventory_generico);


                    if (item_inventory != null)
                    {
                        ModelState.AddModelError("", "El equipo " + item_inventory.ConcatInfoGeneral + " ya se encuentra asignado a este usuario.");
                    }
                    else if (item_generico != null)
                    {
                        ModelState.AddModelError("", "El equipo " + item_generico.model + " ya se encuentra asignado a este usuario.");
                    }
                }
            }

            //verifica si la lista harware está vacia
            if (listaItems.Count == 0)
                ModelState.AddModelError("", "La lista de Hardwares no puede estra vacía.");

            //verifica si algun harware se repite
            bool repetido = false;
            foreach (var li in listaItems)
            {
                if (listaItems.Where(x =>
                    x.id_it_inventory_item == li.id_it_inventory_item
                    && x.id_it_inventory_generico == li.id_it_inventory_generico).ToList().Count>1)
                {
                    repetido = true;
                }
            }

            if(repetido)
                ModelState.AddModelError("", "Existe Hardware repetido, verifique que cada equipo se indique una sóla vez.");

            var relItem = listaItems.FirstOrDefault();


            //trata de obtener la version asociada al documento
            string tipoDocumento = String.Empty;

            //obtiene el item 
            var tipoHardware = db.IT_inventory_hardware_type.Find(iT_asignacion_hardware.tipo_hardware);

            switch (tipoHardware.descripcion)
            {
                case Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES:
                    tipoDocumento = Bitacoras.Util.DocumentosProcesos.RESPONSIVA_ACCESORIOS; //responsivas para accesorios
                    break;
                case Bitacoras.Util.IT_Tipos_Hardware.SMARTPHONE:
                    tipoDocumento = Bitacoras.Util.DocumentosProcesos.RESPONSIVA_CELULAR; //responsivas para celulares
                    break;
                case Bitacoras.Util.IT_Tipos_Hardware.LAPTOP:
                case Bitacoras.Util.IT_Tipos_Hardware.DESKTOP:
                default:
                    tipoDocumento = Bitacoras.Util.DocumentosProcesos.RESPONSIVA_LAPTOP; //responsivas para laptos, pcs y demas
                    break;
            }

            //primero obtiene los documentos asociados al proceso y de la planta del usuario de sistemas
            empleados empleado_sistemas = db.empleados.Find(iT_asignacion_hardware.id_sistemas);
            var iatf_docto = db.IATF_documentos.Where(x => x.proceso == tipoDocumento && x.id_planta == empleado_sistemas.planta_clave).FirstOrDefault();

            //if (iatf_docto == null)
            //{ 
            //    iatf_docto = db.IATF_documentos
            //}

            if (iatf_docto != null && iatf_docto.IATF_revisiones.Count == 0)
                ModelState.AddModelError("", "No se encontraron revisiones asociadas al documento IATF.");

            //selecciona la revision más reciente
            if (iatf_docto != null)
            {
                int id_version_iatf = iatf_docto.IATF_revisiones.OrderByDescending(x => x.fecha_revision).Take(1).Select(x => x.id).FirstOrDefault();
                iT_asignacion_hardware.id_iatf_version = id_version_iatf;
            }

            //aplica cuando es diferente de accesorio
            if (ModelState.IsValid)
            {

                //busca registro para saber si debe ser responsable actual (Sólo para cuando el hardware es diferente de accesorio) 
                var asignacion_previa = db.IT_asignacion_hardware.FirstOrDefault(x =>
                    x.IT_asignacion_hardware_rel_items.Any(y => y.id_it_inventory_item == relItem.id_it_inventory_item)
                    && relItem.id_it_inventory_item != null
                    && x.es_asignacion_actual == true
                    && x.id_responsable_principal == x.id_empleado);

                //si no existe un registro con  responsable principal y asignación actual para este item, el resposable principal es true}}
                if (asignacion_previa == null && iT_asignacion_hardware.es_asignacion_actual && tipoHardware != null
                     && Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES != tipoHardware.descripcion
                    //&& listaItems.Count == 1
                    )
                {
                    iT_asignacion_hardware.id_responsable_principal = iT_asignacion_hardware.id_empleado;
                    //obtiene las asignaciones previas (solo para hardware items)
                    var asignaciones_previas = db.IT_asignacion_hardware.Where(x =>
                    x.IT_asignacion_hardware_rel_items.Any(y => y.id_it_inventory_item == relItem.id_it_inventory_item && relItem.id_it_inventory_item.HasValue)
                    && x.es_asignacion_actual == true);

                    //crea una nueva asignacion para todas las asignaciones previas, unicamente cambiando el id responsable
                    foreach (var asg in asignaciones_previas)
                    {
                        db.IT_asignacion_hardware.Add(new IT_asignacion_hardware()
                        {
                            id_iatf_version = asg.id_iatf_version,
                            id_empleado = asg.id_empleado,
                            id_sistemas = asg.id_sistemas,
                            fecha_asignacion = asg.fecha_asignacion,
                            es_asignacion_actual = asg.es_asignacion_actual,
                            id_responsable_principal = iT_asignacion_hardware.id_responsable_principal,
                            IT_asignacion_hardware_rel_items = new List<IT_asignacion_hardware_rel_items> {
                                new IT_asignacion_hardware_rel_items {
                                        id_it_inventory_item = relItem.id_it_inventory_item,
                                        id_it_inventory_generico = relItem.id_it_inventory_generico,
                                        comments = relItem.comments
                                    }
                            }
                        });
                    }

                    //actualiza todos los registros anteriores para que dejen de ser la asignación actual
                    foreach (var asg in asignaciones_previas)
                    {
                        asg.es_asignacion_actual = false;
                        asg.fecha_desasignacion = DateTime.Now;
                        db.Entry(asg).State = EntityState.Modified;
                    }


                }
                else if (!iT_asignacion_hardware.es_asignacion_actual)
                {
                    iT_asignacion_hardware.id_responsable_principal = iT_asignacion_hardware.id_empleado;
                }
                else if (iT_asignacion_hardware.es_asignacion_actual && asignacion_previa != null)//si ya existe, responsable principal es false y se copia el id responsiva
                {
                    iT_asignacion_hardware.id_responsable_principal = asignacion_previa.id_responsable_principal;
                    iT_asignacion_hardware.id_biblioteca_digital = asignacion_previa.id_biblioteca_digital;
                }


                //si es accesorio, siempre va a ser responsable principal
                if(Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES == tipoHardware.descripcion)
                    iT_asignacion_hardware.id_responsable_principal = iT_asignacion_hardware.id_empleado;


                //indica si la asignacion de la linea es la actual
                if (iT_asignacion_hardware.id_cellular_line.HasValue && iT_asignacion_hardware.es_asignacion_actual)
                    iT_asignacion_hardware.es_asignacion_linea_actual = true;



                /*                
                //en caso de que no exista una asignación igual
                if (iT_asignacion_hardware.id_cellular_line.HasValue && !db.IT_asignacion_cellular_line.Any(x =>
                    x.id_empleado == iT_asignacion_hardware.id_empleado
                    && x.es_asignacion_actual
                    && x.id_inventory_cellular_line == iT_asignacion_hardware.id_inventory_cellular_line))
                    db.IT_asignacion_cellular_line.Add(asignacionCelular);
                */


                //iT_asignacion_hardware.es_asignacion_actual = true;
                iT_asignacion_hardware.fecha_asignacion = DateTime.Now;
                db.IT_asignacion_hardware.Add(iT_asignacion_hardware);

                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha generado la asignación correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("ListadoAsignaciones", new { id = iT_asignacion_hardware.id_empleado });
            }

            iT_asignacion_hardware.empleados = db.empleados.Find(iT_asignacion_hardware.id_empleado);
            iT_asignacion_hardware.empleados2 = db.empleados.Find(iT_asignacion_hardware.id_sistemas);


            //envia el tipo de hardware
            ViewBag.TipoHardware = tipoHardware;
            ViewBag.tipo_hardware = AddFirstItem(new SelectList(db.IT_inventory_hardware_type.Where(x => x.activo), "id", "descripcion"), textoPorDefecto: "-- Select --", selected: iT_asignacion_hardware.tipo_hardware.ToString());
            ViewBag.id_cellular_line = AddFirstItem(new SelectList(db.IT_inventory_cellular_line.Where(x => x.id_planta == iT_asignacion_hardware.empleados.planta_clave), "id", "ConcatDetalles"), textoPorDefecto: "-- Seleccionar --", selected: iT_asignacion_hardware.id_cellular_line.ToString());

            ViewBag.ListadoHardwareInventario = db.IT_inventory_items.Where(x => x.id_inventory_type == iT_asignacion_hardware.tipo_hardware && x.active == true).ToList();
            ViewBag.ListadoHardwareGenerico = db.IT_inventory_items_genericos.Where(x => x.id_inventory_type == iT_asignacion_hardware.tipo_hardware && x.active == true).ToList();
            ViewBag.ListadoTiposAccesorios = db.IT_inventory_tipos_accesorios.ToList();

            //determina si permite enviar multiples hardware items
            if (tipoHardware.descripcion == Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES)
                ViewBag.Multiple = true;
            else
                ViewBag.Multiple = false;

            return View(iT_asignacion_hardware);
        }


        // GET: IT_asignacion_hardware/CargarResponsiva
        public ActionResult CargarResponsiva(int? id)
        {

            if (!TieneRol(TipoRoles.IT_ASIGNACION_HARDWARE))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_asignacion_hardware model = db.IT_asignacion_hardware.Find(id);
            if (model == null)
            {
                return View("../Error/NotFound");
            }

            return View(model);
        }

        // POST: IT_asignacion_hardware/CargarResponsiva
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CargarResponsiva(IT_asignacion_hardware iT_asignacion_hardware)
        {

            IT_asignacion_hardware item = db.IT_asignacion_hardware.Find(iT_asignacion_hardware.id);

            if (iT_asignacion_hardware.PostedFileResponsiva != null && iT_asignacion_hardware.PostedFileResponsiva.InputStream.Length > 5242880)
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 5MB.");
            else if (iT_asignacion_hardware.PostedFileResponsiva != null)
            { //verifica la extensión del archivo
                string extension = System.IO.Path.GetExtension(iT_asignacion_hardware.PostedFileResponsiva.FileName);
                if (extension.ToUpper() != ".XLS"   //si no contiene una extensión válida
                               && extension.ToUpper() != ".XLSX"
                               && extension.ToUpper() != ".DOC"
                               && extension.ToUpper() != ".DOCX"
                               && extension.ToUpper() != ".PDF"
                               && extension.ToUpper() != ".PNG"
                               && extension.ToUpper() != ".JPG"
                               && extension.ToUpper() != ".JPEG"
                               )
                    ModelState.AddModelError("", "Sólo se permiten archivos con extensión .xls, .xlsx, .doc, .docx, .pdf, .png, .jpg, .jpeg.");
                else
                { //si la extension y el tamaño son válidos
                    String nombreArchivo = iT_asignacion_hardware.PostedFileResponsiva.FileName;

                    //recorta el nombre del archivo en caso de ser necesario
                    if (nombreArchivo.Length > 80)
                        nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                    //lee el archivo en un arreglo de bytes
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(iT_asignacion_hardware.PostedFileResponsiva.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(iT_asignacion_hardware.PostedFileResponsiva.ContentLength);
                    }

                    //genera el archivo de biblioce digital
                    biblioteca_digital archivo = new biblioteca_digital
                    {
                        Nombre = nombreArchivo,
                        MimeType = UsoStrings.RecortaString(iT_asignacion_hardware.PostedFileResponsiva.ContentType, 80),
                        Datos = fileData
                    };

                    //en caso de exister un archivo realiza un update
                    if (item.biblioteca_digital != null)
                    {
                        item.biblioteca_digital.Nombre = archivo.Nombre;
                        item.biblioteca_digital.MimeType = archivo.MimeType;
                        item.biblioteca_digital.Datos = archivo.Datos;

                        db.Entry(item.biblioteca_digital).State = EntityState.Modified;
                    }
                    else
                    {      //si no existe el archivo lo crea
                        item.biblioteca_digital = archivo;
                    }

                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();

                    //actualiza el numero de responsiva a los demas involucrados
                    //busca registro para saber si debe ser responsable actual

                    //obtiene el primer valor de las responsiva (solo habra uno a excepcion de accesorios)
                    var firstItem = item.IT_asignacion_hardware_rel_items.FirstOrDefault();

                    //busca todas las responsivas donde el objeto esta asignado
                    var asignacion_previa = db.IT_asignacion_hardware.Where(x => x.IT_asignacion_hardware_rel_items.Any(y => y.id_it_inventory_item == firstItem.id_it_inventory_item) && x.es_asignacion_actual == true);

                    //solamente copia a  las demas asignaciones se se trata de una asignacion actual y proviene de inventarios
                    if (item.es_asignacion_actual && firstItem.EsInventario() && firstItem.GetHardwareType().descripcion != Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES)
                        foreach (var p in asignacion_previa)
                        {
                            p.id_biblioteca_digital = item.id_biblioteca_digital;
                            db.Entry(p).State = EntityState.Modified;
                        }

                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert("Se ha subido la responsiva correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("ListadoAsignaciones", new { id = iT_asignacion_hardware.id_empleado });
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
                    return RedirectToAction("ListadoAsignaciones", new { id = iT_asignacion_hardware.id_empleado });

                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("ListadoAsignaciones", new { id = iT_asignacion_hardware.id_empleado });
                }
            }
            return View(item);
        }

        // GET: IT_asignacion_hardware/DesvincularLinea/5
        public ActionResult DesvincularLinea(int? id, int? id_empleado)
        {
            if (!TieneRol(TipoRoles.IT_ASIGNACION_HARDWARE))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_inventory_cellular_line line = db.IT_inventory_cellular_line.Find(id);
            if (line == null)
            {
                return View("../Error/NotFound");
            }

            if (id_empleado == null)
            {
                return View("../Error/BadRequest");
            }
            empleados empleado = db.empleados.Find(id_empleado);
            if (empleado == null)
            {
                return View("../Error/NotFound");
            }

            //busca lasasig naciones donde la linea este activa
            var asignaciones = db.IT_asignacion_hardware.Where(x =>
            x.id_empleado == id_empleado
            && x.id_cellular_line == line.id
            && x.es_asignacion_linea_actual
            );

            //modifica el estado de cada asignacion
            foreach (var item in asignaciones)
            {
                item.es_asignacion_linea_actual = false;
                db.Entry(item).State = EntityState.Modified;
            }
            db.SaveChanges();

            TempData["Mensaje"] = new MensajesSweetAlert("Se ha desvinculado la línea celular.", TipoMensajesSweetAlerts.SUCCESS);

            return RedirectToAction("ListadoAsignaciones", new { id = id_empleado });
        }

        // GET: IT_asignacion_hardware/DesasociarEquipo/5
        public ActionResult DesasociarEquipo(int? id)
        {
            if (!TieneRol(TipoRoles.IT_ASIGNACION_HARDWARE))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_asignacion_hardware iT_asignacion_hardware = db.IT_asignacion_hardware.Find(id);
            if (iT_asignacion_hardware == null)
            {
                return View("../Error/NotFound");
            }

            if (iT_asignacion_hardware.fecha_desasignacion == null)
                iT_asignacion_hardware.fecha_desasignacion = DateTime.Now;

            return View(iT_asignacion_hardware);
        }

        // POST: IT_asignacion_hardware/Delete/5
        [HttpPost, ActionName("DesasociarEquipo")]
        [ValidateAntiForgeryToken]
        public ActionResult DesasociarEquipoConfirmed(IT_asignacion_hardware iT_asignacion_hardware)
        {

            IT_asignacion_hardware iT_asignacion_hardware_old = db.IT_asignacion_hardware.Find(iT_asignacion_hardware.id);
            iT_asignacion_hardware_old.fecha_desasignacion = iT_asignacion_hardware.fecha_desasignacion;
            iT_asignacion_hardware_old.es_asignacion_actual = false;

            //si se leccionó la opcion de desasociar lineas
            if (iT_asignacion_hardware.desasociar_linea)
            {
                var asignaciones = db.IT_asignacion_hardware.Where(x =>
                x.id_empleado == iT_asignacion_hardware_old.id_empleado
                && x.id_cellular_line == iT_asignacion_hardware_old.id_cellular_line
                && x.es_asignacion_linea_actual
                );

                //modifica el estado de cada asignacion
                foreach (var item in asignaciones)
                {
                    item.es_asignacion_linea_actual = false;
                    db.Entry(item).State = EntityState.Modified;
                }

            }

            db.Entry(iT_asignacion_hardware_old).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Mensaje"] = new MensajesSweetAlert("Se ha desvinculado el equipo correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("ListadoAsignaciones", new { id = iT_asignacion_hardware.id_empleado });
        }



        //genera el PDF
        public ActionResult GenerarPlantillaResponsiva(int? id, bool inline = true)
        {

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_asignacion_hardware item = db.IT_asignacion_hardware.Find(id);
            if (item == null)
            {
                return View("../Error/NotFound");
            }
            var firstItem = item.IT_asignacion_hardware_rel_items.FirstOrDefault();

            IT_inventory_hardware_type tipoHardware = firstItem.GetHardwareType();

            byte[] pdfBytes;
            using (var stream = new MemoryStream())
            using (var wri = new PdfWriter(stream))
            using (var pdf = new PdfDocument(wri))
            using (var doc = new Document(pdf, PageSize.LETTER))
            {
                //fuente principal
                PdfFont font = PdfFontFactory.CreateFont(Server.MapPath("/fonts/tkmm/TKTypeMedium.ttf"));
                var thyssenColor = new DeviceRgb(0, 159, 245);

                //márgenes del documento
                doc.SetMargins(75, 35, 75, 35);

                //imagen para encabezado
                Image img = new Image(ImageDataFactory.Create(Server.MapPath("/Content/images/logo_1.png")));
                //maneja los eventos de encabezado y pie de página
                pdf.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandlerPDF_Responsiva(img, font, item));
                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandlerPDF_Responsiva(font, item));

                //Empieza contenido personalizado

                //estilo fuente
                Style styleFuenteThyssen = new Style().SetFont(font);

                //estilo para encabezados
                Style fuenteThyssen = new Style().SetFont(font).SetFontSize(14).SetTextAlignment(TextAlignment.CENTER);
                Style fuenteThyssenBold = new Style().SetFont(font).SetFontSize(10).SetTextAlignment(TextAlignment.LEFT).SetBold();

                //estilo para encabezados
                Style encabezado = new Style().SetFont(font).SetFontSize(10).SetFontColor(ColorConstants.WHITE).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBackgroundColor(thyssenColor).SetBold();

                //lineaEn blanco
                Paragraph newline = new Paragraph(new Text("\n"));

                //nombre documento                
                doc.Add(new Paragraph(item.IATF_revisiones.IATF_documentos.nombre_documento + ": " + tipoHardware.descripcion).AddStyle(fuenteThyssen));


                fuenteThyssen.SetFontSize(10).SetTextAlignment(TextAlignment.RIGHT);
                doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToShortDateString()).AddStyle(fuenteThyssen));

                //Crea el parráfo que funciona como título
                Paragraph pTitle = new Paragraph("").Add(new Tab());
                pTitle.AddTabStops(new TabStop(85, TabAlignment.RIGHT));
                pTitle.Add("Datos del Personal").AddStyle(encabezado);
                doc.Add(pTitle);

                fuenteThyssen.SetTextAlignment(TextAlignment.LEFT);

                //crea tabla pra datos del personal
                float[] cellWidth = { 30f, 70f };
                Table table = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();

                string planta = "--", departamento = "--", puesto = "--";

                if (item.empleados.plantas != null)
                    planta = item.empleados.plantas.descripcion;
                if (item.empleados.Area != null)
                    departamento = item.empleados.Area.descripcion;
                if (item.empleados.puesto1 != null)
                    puesto = item.empleados.puesto1.descripcion;


                table.AddCell(new Cell().Add(new Paragraph("Nombre:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(item.empleados.ConcatNombre)).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("8ID:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.empleados.C8ID) ? item.empleados.C8ID : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Núm. Empleado:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.empleados.numeroEmpleado) ? item.empleados.numeroEmpleado : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Correo:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.empleados.correo) ? item.empleados.correo : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Planta:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(planta)).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Departamento:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(departamento)).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Puesto:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(puesto)).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));

                doc.Add(table);

                int c = 1;

                //agrega los datos de cada item en la responsiva 
                foreach (var relItem in item.IT_asignacion_hardware_rel_items)
                {

                    string encabezadoEquipo = "Datos del Equipo";

                    if (item.IT_asignacion_hardware_rel_items.Count > 1)
                        encabezadoEquipo = "Datos del Equipo " + (c++) + ":";

                    //crea tabla para datos del equipo
                    pTitle = new Paragraph("").Add(new Tab());
                    pTitle.AddTabStops(new TabStop(85, TabAlignment.RIGHT));
                    pTitle.Add(encabezadoEquipo).AddStyle(encabezado);
                    doc.Add(pTitle);


                    table = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();

                    var _item = relItem.GetGenericObject();

                    if (_item.IT_inventory_hardware_type.descripcion != Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES)
                    {
                        table.AddCell(new Cell().Add(new Paragraph("Tipo:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                        table.AddCell(new Cell().Add(new Paragraph(_item.IT_inventory_hardware_type.descripcion)).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                    }

                    if (_item.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.LAPTOP
                        || _item.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.DESKTOP)
                    {
                        table.AddCell(new Cell().Add(new Paragraph("Hostname:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                        table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(_item.hostname) ? _item.hostname : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                    }

                    if (_item.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES)
                    {
                        table.AddCell(new Cell().Add(new Paragraph("Tipo Accesorio:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                        table.AddCell(new Cell().Add(new Paragraph(_item.IT_inventory_tipos_accesorios != null ? _item.IT_inventory_tipos_accesorios.descripcion : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                    }


                    table.AddCell(new Cell().Add(new Paragraph("Marca:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(_item.brand) ? _item.brand : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph("Modelo:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(_item.model) ? _item.model : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph("Núm. Serie:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(_item.serial_number) ? _item.serial_number : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));

                    if (_item.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.SMARTPHONE)
                    {
                        table.AddCell(new Cell().Add(new Paragraph("Imei:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                        table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(_item.imei_1) ? _item.imei_1 : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));


                        table.AddCell(new Cell().Add(new Paragraph("Número de Teléfono:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                        table.AddCell(new Cell().Add(new Paragraph(item.IT_inventory_cellular_line != null ? item.IT_inventory_cellular_line.numero_celular : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));

                        table.AddCell(new Cell().Add(new Paragraph("PLAN:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                        table.AddCell(new Cell().Add(new Paragraph(item.IT_inventory_cellular_line != null ? item.IT_inventory_cellular_line.IT_inventory_cellular_plans.nombre_plan : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));

                    }

                    //agrega comentarios
                    table.AddCell(new Cell().Add(new Paragraph("Comentarios:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(relItem.comments) ? relItem.comments : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                    doc.Add(table);

                }

                //crea parrafo de detalles
                fuenteThyssen.SetTextAlignment(TextAlignment.JUSTIFIED);
                pTitle = new Paragraph("\n\n").Add(new Tab());
                pTitle.AddTabStops(new TabStop(1550, TabAlignment.RIGHT));
                pTitle.Add("Se hace entrega del equipo de cómputo, así como accesorios propiedad de thyssenkrupp Materials de México S.A. de C.V. al empleado inscrito en este documento" +
                    " para utilizarse como herramienta de trabajo en el desempeño de sus funciones, el cual deberá regirse bajo el Reglamento del Grupo de seguridad de la " +
                    "información (RE-CO-GPI-0216-V01-ES) y la Política interna de IT-tkMM (ITE001) vigentes.")
                    .AddStyle(fuenteThyssen);
                doc.Add(pTitle);

                //si es celular no agrega los espacios en blanco, para que no se corte la hoja
                if (item.IT_asignacion_hardware_rel_items.FirstOrDefault().GetHardwareType().descripcion != Bitacoras.Util.IT_Tipos_Hardware.SMARTPHONE)
                {
                    //agrega tres lineas en blanco
                    doc.Add(newline);
                    doc.Add(newline);
                }


                //crea tabla para firmas
                float[] cellWidth2 = { 50f, 50f };
                table = new Table(UnitValue.CreatePercentArray(cellWidth2)).UseAllAvailableWidth();
                table.AddCell(new Cell().Add(new Paragraph("Elaboró:")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1)));
                table.AddCell(new Cell().Add(new Paragraph("Recibió:")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));

                string elaboro = String.Empty, recibe = String.Empty;

                if (item.empleados2 != null)
                    elaboro = item.empleados2.ConcatNombre.ToUpper();
                if (item.empleados2.puesto1 != null)
                    elaboro += "\n" + item.empleados2.puesto1.descripcion.ToUpper();
                if (item.empleados2.Area != null)
                    elaboro += "\n" + item.empleados2.Area.descripcion.ToUpper();

                if (item.empleados != null)
                    recibe = item.empleados.ConcatNombre.ToUpper();
                if (item.empleados.puesto1 != null)
                    recibe += "\n" + item.empleados.puesto1.descripcion.ToUpper();
                if (item.empleados.Area != null)
                    recibe += "\n" + item.empleados.Area.descripcion.ToUpper();

                table.AddCell(new Cell().Add(new Paragraph(elaboro)
                    .SetTextAlignment(TextAlignment.CENTER)).AddStyle(fuenteThyssen)
                    .SetHeight(120)
                    .SetBorder(Border.NO_BORDER)
                    .SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1))
                    .SetVerticalAlignment(VerticalAlignment.BOTTOM)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                    );

                table.AddCell(new Cell().Add(new Paragraph(recibe).SetTextAlignment(TextAlignment.CENTER)).AddStyle(fuenteThyssen)
                    .SetHeight(120)
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.BOTTOM)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER));

                table.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                //evita que la tabla se divida
                table.SetKeepTogether(true);

                doc.Add(table);


                //salto de página
                doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                //tabla de movimientos         
                doc.Add(new Paragraph("Control de cambios").AddStyle(encabezado));

                float[] cellWidth3 = { 10f, 26.6f, 10f, 53.4f };

                Style encabezadoTabla = new Style().SetFontSize(10).SetFontColor(ColorConstants.BLACK).SetBorder(new SolidBorder(ColorConstants.GRAY, 1)).SetBackgroundColor(ColorConstants.LIGHT_GRAY);

                table = new Table(UnitValue.CreatePercentArray(cellWidth3)).UseAllAvailableWidth();
                table.AddCell(new Cell().Add(new Paragraph("Fecha")).AddStyle(encabezadoTabla));
                table.AddCell(new Cell().Add(new Paragraph("Autor")).AddStyle(encabezadoTabla));
                //table.AddCell(new Cell().Add(new Paragraph("Puesto")).AddStyle(encabezadoTabla));
                table.AddCell(new Cell().Add(new Paragraph("Revisión")).AddStyle(encabezadoTabla));
                table.AddCell(new Cell().Add(new Paragraph("Descripción")).AddStyle(encabezadoTabla));

                foreach (var revision in item.IATF_revisiones.IATF_documentos.IATF_revisiones.OrderBy(x => x.numero_revision))
                {
                    table.AddCell(new Cell().Add(new Paragraph(revision.fecha_revision.ToShortDateString()).AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                    table.AddCell(new Cell().Add(new Paragraph(revision.responsable).AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                    //  table.AddCell(new Cell().Add(new Paragraph(String.IsNullOrEmpty(revision.puesto_responsable) ? String.Empty : revision.puesto_responsable).AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                    table.AddCell(new Cell().Add(new Paragraph(revision.numero_revision.ToString()).SetTextAlignment(TextAlignment.CENTER).AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                    table.AddCell(new Cell().Add(new Paragraph(revision.descripcion).AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                }

                doc.Add(table);
                doc.Close();
                doc.Flush();
                pdfBytes = stream.ToArray();
            }
            // return new FileContentResult(pdfBytes, "application/pdf");

            string filename = IT_matriz_requerimientosController.itfNumber + "_Responsiva de Equipo_" + item.empleados.ConcatNombre.Trim() + ".pdf";

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = filename,
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = inline,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(pdfBytes, "application/pdf");

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
