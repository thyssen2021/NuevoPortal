using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class OrdenesTrabajoController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: ListadoUsuario
        public ActionResult ListadoUsuario(string estatus, int? id, int pagina = 1)
        {
            if (TieneRol(TipoRoles.OT_SOLICITUD))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.orden_trabajo
                    .Where(x => x.id_solicitante == empleado.id
                      && (id == null || x.id == id)
                      && (String.IsNullOrEmpty(estatus) || x.estatus.Contains(estatus))
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                   .Where(x => x.id_solicitante == empleado.id
                      && (id == null || x.id == id)
                      && (String.IsNullOrEmpty(estatus) || x.estatus.Contains(estatus))
                    )
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id"] = id;
                routeValues["estatus"] = estatus;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                List<string> estatusList = db.orden_trabajo.Select(x => x.estatus).Distinct().ToList();
                //crea un Select  list para el estatus
                List<SelectListItem> newList = new List<SelectListItem>();

                foreach (string statusItem in estatusList)
                {
                    newList.Add(new SelectListItem()
                    {
                        Text = OT_Status.DescripcionStatus(statusItem),
                        Value = statusItem
                    });
                }

                SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
                ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Todos --");
                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones               
                ViewBag.Title = "Listado Solicitudes Creadas";
                ViewBag.SegundoNivel = "mis_solicitudes";
                // ViewBag.Create = true;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: OrdenesTrabajo/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.OT_SOLICITUD) || TieneRol(TipoRoles.OT_ASIGNACION) || TieneRol(TipoRoles.OT_RESPONSABLE)
            || TieneRol(TipoRoles.OT_REPORTE))
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                orden_trabajo orden_trabajo = db.orden_trabajo.Find(id);
                if (orden_trabajo == null)
                {
                    return HttpNotFound();
                }
                return View(orden_trabajo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        //// GET: OrdenesTrabajo/Create
        public ActionResult Create()
        {

            if (TieneRol(TipoRoles.OT_SOLICITUD))
            {
                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //verifica si es necesario mostrar el combo de lineas
                if (empleado.Area != null && (empleado.Area.descripcion.ToUpper().Contains("PRODUCCION") || empleado.Area.descripcion.ToUpper().Contains("PRODUCCIÓN")))
                    ViewBag.MuestraLineas = true;

                //crea el select list para status
                List<SelectListItem> selectListUrgencia = obtieneSelectListEstatus();

                SelectList selectListItemsUrgencia = new SelectList(selectListUrgencia, "Value", "Text");
                ViewBag.nivel_urgencia = AddFirstItem(selectListItemsUrgencia, textoPorDefecto: "-- Seleccionar --");
                ViewBag.Solicitante = empleado;
                ViewBag.id_linea = AddFirstItem(new SelectList(db.produccion_lineas.Where(x => x.clave_planta == empleado.planta_clave && x.activo == true), "id", "linea"));
                return View();

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: OrdenesTrabajo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(orden_trabajo orden_trabajo)
        {
            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //verificA SI TIENE un área asiganda
            if (!(orden_trabajo.id_area > 0))
                ModelState.AddModelError("", "El usuario no tiene un área asignada, contacte al administrador del sitio para poder continuar.");

            //verifica si el tamaño del archivo es válido
            if (orden_trabajo.PostedFileSolicitud != null && orden_trabajo.PostedFileSolicitud.InputStream.Length > 10485760)
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 10MB.");
            else if (orden_trabajo.PostedFileSolicitud != null)
            { //verifica la extensión del archivo
                string extension = Path.GetExtension(orden_trabajo.PostedFileSolicitud.FileName);
                if (extension.ToUpper() != ".XLS"   //si no contiene una extensión válida
                               && extension.ToUpper() != ".XLSX"
                               && extension.ToUpper() != ".DOC"
                               && extension.ToUpper() != ".DOCX"
                               && extension.ToUpper() != ".PDF"
                               && extension.ToUpper() != ".PNG"
                               && extension.ToUpper() != ".JPG"
                               && extension.ToUpper() != ".JPEG"
                               && extension.ToUpper() != ".RAR"
                               && extension.ToUpper() != ".ZIP"
                               && extension.ToUpper() != ".EML"
                               )
                    ModelState.AddModelError("", "Sólo se permiten archivos con extensión .xls, .xlsx, .doc, .docx, .pdf, .png, .jpg, .jpeg, .rar, .zip., y .eml.");
                else
                { //si la extension y el tamaño son válidos
                    String nombreArchivo = orden_trabajo.PostedFileSolicitud.FileName;

                    //recorta el nombre del archivo en caso de ser necesario
                    if (nombreArchivo.Length > 80)
                        nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                    //lee el archivo en un arreglo de bytes
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(orden_trabajo.PostedFileSolicitud.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(orden_trabajo.PostedFileSolicitud.ContentLength);
                    }

                    //genera el archivo de biblioce digital
                    biblioteca_digital archivo = new biblioteca_digital
                    {
                        Nombre = nombreArchivo,
                        MimeType = UsoStrings.RecortaString(orden_trabajo.PostedFileSolicitud.ContentType, 80),
                        Datos = fileData
                    };

                    //relaciona el archivo con la poliza (despues se guarda en BD)
                    orden_trabajo.biblioteca_digital1 = archivo;  //documento soporte

                }
            }

            if (ModelState.IsValid)
            {
                orden_trabajo.fecha_solicitud = DateTime.Now;
                orden_trabajo.estatus = OT_Status.ABIERTO;

                db.orden_trabajo.Add(orden_trabajo);
                try
                {
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("ListadoUsuario");
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("ListadoUsuario");
                }
            }

            //En caso de que el modelo no sea válido

            //verifica si es necesario mostrar el combo de lineas
            if (empleado.Area != null && (empleado.Area.descripcion.ToUpper().Contains("PRODUCCION") || empleado.Area.descripcion.ToUpper().Contains("PRODUCCIÓN")))
                ViewBag.MuestraLineas = true;

            //crea el select list para status
            List<SelectListItem> selectListUrgencia = obtieneSelectListEstatus();

            SelectList selectListItemsUrgencia = new SelectList(selectListUrgencia, "Value", "Text");
            ViewBag.nivel_urgencia = AddFirstItem(selectListItemsUrgencia, textoPorDefecto: "-- Seleccionar --", selected: orden_trabajo.estatus);
            ViewBag.Solicitante = empleado;
            ViewBag.id_linea = AddFirstItem(new SelectList(db.produccion_lineas.Where(x => x.clave_planta == empleado.planta_clave && x.activo == true), "id", "linea"), selected: orden_trabajo.estatus);

            return View(orden_trabajo);
        }

        // GET: ListadoUsuario
        public ActionResult ListadoAginacionPendientes(int pagina = 1)
        {
            if (TieneRol(TipoRoles.OT_ASIGNACION))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listado = db.orden_trabajo
                    .Where(x => x.estatus == OT_Status.ABIERTO
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                     .Where(x => x.estatus == OT_Status.ABIERTO
                    )
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["id"] = id;
                //routeValues["estatus"] = estatus;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;


                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: ListadoAsignacionAsignadas
        public ActionResult ListadoAsignacionAsignadas(int pagina = 1)
        {
            if (TieneRol(TipoRoles.OT_ASIGNACION))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listado = db.orden_trabajo
                    .Where(x => x.estatus != OT_Status.ABIERTO
                    && x.id_asignacion == empleado.id
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                     .Where(x => x.estatus != OT_Status.ABIERTO
                    && x.id_asignacion == empleado.id
                    )
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["id"] = id;
                //routeValues["estatus"] = estatus;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;


                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: OrdenesTrabajo/Asignar/5
        public ActionResult Asignar(int? id)
        {
            if (TieneRol(TipoRoles.OT_ASIGNACION))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                orden_trabajo orden_trabajo = db.orden_trabajo.Find(id);
                if (orden_trabajo == null)
                {
                    return HttpNotFound();
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //obtiene el listado de capturistas de contabilidad
                List<int> idsPersonalMantenimiento = db.OT_personal_mantenimiento
                    .Where(x => x.empleados.planta_clave == empleado.planta_clave && x.activo == true)
                    .Select(x => x.empleados.id).Distinct().ToList();

                ViewBag.id_responsable = AddFirstItem(new SelectList(db.empleados.Where(x =>
                idsPersonalMantenimiento.Contains(x.id)

                ), "id", "ConcatNumEmpleadoNombre"), selected: orden_trabajo.id_responsable.ToString());

                return View(orden_trabajo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }


        // POST: OrdenesTrabajo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Asignar(orden_trabajo orden_trabajo)
        {
            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //verifica que existan objetos asociados
            orden_trabajo ordenOld = db.orden_trabajo.Find(orden_trabajo.id);

            if (ordenOld == null)
                ModelState.AddModelError("", "No se encontró el orden del trabajo asociada.");

            if (ordenOld.estatus != OT_Status.ABIERTO)
                ModelState.AddModelError("", "Sólo se pueden asingnar órdenes con estatus ABIERTO.");


            if (ModelState.IsValid)
            {
                ordenOld.id_asignacion = empleado.id;
                ordenOld.id_responsable = orden_trabajo.id_responsable;
                ordenOld.fecha_asignacion = DateTime.Now;
                ordenOld.estatus = OT_Status.ASIGNADO;

                db.Entry(ordenOld).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("ListadoAginacionPendientes");
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("ListadoAginacionPendientes");
                }
            }

            //En caso de que el modelo no sea válido

            //obtiene el listado de capturistas de contabilidad
            List<int> idsPersonalMantenimiento = db.OT_personal_mantenimiento
                .Where(x => x.empleados.planta_clave == empleado.planta_clave && x.activo == true)
                .Select(x => x.empleados.id).Distinct().ToList();

            ViewBag.id_responsable = AddFirstItem(new SelectList(db.empleados.Where(x =>
            idsPersonalMantenimiento.Contains(x.id)

            ), "id", "ConcatNumEmpleadoNombre"), selected: orden_trabajo.id_responsable.ToString());

            return View(orden_trabajo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [NonAction]
        protected List<SelectListItem> obtieneSelectListEstatus()
        {
            List<orden_trabajo> estatusList = new List<orden_trabajo>();
            estatusList.Add(new orden_trabajo { descripcion = OT_nivel_urgencia.DescripcionStatus(OT_nivel_urgencia.ALTA), estatus = OT_nivel_urgencia.ALTA });
            estatusList.Add(new orden_trabajo { descripcion = OT_nivel_urgencia.DescripcionStatus(OT_nivel_urgencia.MEDIA), estatus = OT_nivel_urgencia.MEDIA });
            estatusList.Add(new orden_trabajo { descripcion = OT_nivel_urgencia.DescripcionStatus(OT_nivel_urgencia.BAJA), estatus = OT_nivel_urgencia.BAJA });

            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            foreach (orden_trabajo item in estatusList)
            {
                newList.Add(new SelectListItem()
                {
                    Text = item.descripcion,
                    Value = item.estatus
                });
            }

            return newList;
        }
    }
}
