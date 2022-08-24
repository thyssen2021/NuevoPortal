using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using IdentitySample.Models;
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

        // GET: SolicitudesDepartamento
        public ActionResult SolicitudesDepartamento(string estatus, int? id_solicitante, int pagina = 1)
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
                    .Where(
                        x => (
                            x.id_area == empleado.id_area //todas las solicitudes del área
                            || x.empleados2.id_jefe_directo == empleado.id  //todas donde el solicitante es subordinado
                        )
                        && (id_solicitante == null || x.id_solicitante == id_solicitante)
                        && (String.IsNullOrEmpty(estatus) || x.estatus.Contains(estatus))
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                    .Where(
                        x => (
                            x.id_area == empleado.id_area //todas las solicitudes del área
                            || x.empleados2.id_jefe_directo == empleado.id  //todas donde el solicitante es subordinado
                        )
                        && (id_solicitante == null || x.id_solicitante == id_solicitante)
                        && (String.IsNullOrEmpty(estatus) || x.estatus.Contains(estatus))
                    )
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_solicitante"] = id_solicitante;
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

                //List Solicitante
                List<empleados> solicitanteList = db.empleados.Where(x =>
                x.orden_trabajo2.Any(y =>
                    y.id_area == empleado.id_area
                    || y.empleados2.id_jefe_directo == empleado.id))
                .ToList();
                ViewBag.id_solicitante = AddFirstItem(new SelectList(solicitanteList, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), textoPorDefecto: "-- Todos --", selected: id_solicitante.ToString());


                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones               
                ViewBag.Title = "Listado Solicitudes Creadas Por Departamento";
                ViewBag.SegundoNivel = "solicitudes_departamento";
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
            || TieneRol(TipoRoles.OT_REPORTE) || TieneRol(TipoRoles.OT_CATALOGOS))
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

                //verifica si la OT no pertenece al depto ni es jefe directo ni si tiene alguno de los roles suguientes
                if (orden_trabajo.id_area != empleado.id_area && orden_trabajo.empleados2.id_jefe_directo != empleado.id 
                    && !TieneRol(TipoRoles.OT_REPORTE)
                    && !TieneRol(TipoRoles.OT_CATALOGOS)
                    && !TieneRol(TipoRoles.OT_ASIGNACION)
                    && !TieneRol(TipoRoles.OT_RESPONSABLE)
                    ) {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede visualizar esta solicitud!";
                    ViewBag.Descripcion = "No se puede visualizar esta orden de trabajo ya que pertenece a un departemento distinto.";

                    return View("../Home/ErrorGenerico");
                }

                //verifica si se puede visualizar
                if (orden_trabajo.Area.plantaClave != empleado.planta_clave && orden_trabajo.empleados2.id_jefe_directo != empleado.id)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede visualizar esta solicitud!";
                    ViewBag.Descripcion = "No se puede visualizar una orden de trabajo de otra planta.";

                    return View("../Home/ErrorGenerico");
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

                //obtiene los departamentos donde aplica linea de producción
                List<int> listAreasIds = db.OT_rel_depto_aplica_linea.Where(x => x.id_area > 0).Select(x => x.id_area).Distinct().ToList();

                if (empleado.id_area.HasValue && listAreasIds.Contains(empleado.id_area.Value))
                    ViewBag.MuestraLineas = true;

                //crea el select list para status
                List<SelectListItem> selectListUrgencia = obtieneSelectListEstatus();

                SelectList selectListItemsUrgencia = new SelectList(selectListUrgencia, "Value", "Text");
                ViewBag.nivel_urgencia = AddFirstItem(selectListItemsUrgencia, textoPorDefecto: "-- Seleccionar --");
                ViewBag.Solicitante = empleado;
                ViewBag.id_linea = AddFirstItem(new SelectList(db.produccion_lineas.Where(x => x.clave_planta == empleado.planta_clave && x.activo == true), "id", "linea"));
                // GET: OrdenesTrabajo/Create encia list de zona vacío
                ViewBag.id_zona_falla = AddFirstItem(new SelectList(db.OT_zona_falla.Where(x => false), "id", "zona_falla"), textoPorDefecto: "-- N/A --");
                ViewBag.id_grupo_trabajo = AddFirstItem(new SelectList(db.OT_grupo_trabajo.Where(x => x.activo == true), "id", "descripcion"));
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

            List<HttpPostedFileBase> archivosForm = new List<HttpPostedFileBase>();
            //agrega archivos enviados
            if (orden_trabajo.PostedFileSolicitud_1 != null)
                archivosForm.Add(orden_trabajo.PostedFileSolicitud_1);
            if (orden_trabajo.PostedFileSolicitud_2 != null)
                archivosForm.Add(orden_trabajo.PostedFileSolicitud_2);
            if (orden_trabajo.PostedFileSolicitud_3 != null)
                archivosForm.Add(orden_trabajo.PostedFileSolicitud_3);
            if (orden_trabajo.PostedFileSolicitud_4 != null)
                archivosForm.Add(orden_trabajo.PostedFileSolicitud_4);

            #region lectura de archivos

            foreach (HttpPostedFileBase httpPostedFileBase in archivosForm)
            {
                //verifica si el tamaño del archivo es OT 1
                if (httpPostedFileBase != null && httpPostedFileBase.InputStream.Length > 10485760)
                    ModelState.AddModelError("", "Sólo se permiten archivos menores a 10MB: " + httpPostedFileBase.FileName + ".");
                else if (httpPostedFileBase != null)
                { //verifica la extensión del archivo
                    string extension = Path.GetExtension(httpPostedFileBase.FileName);
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
                        ModelState.AddModelError("", "Sólo se permiten archivos con extensión .xls, .xlsx, .doc, .docx, .pdf, .png, .jpg, .jpeg, .rar, .zip., y .eml.: " + httpPostedFileBase.FileName + ".");
                    else
                    { //si la extension y el tamaño son válidos
                        String nombreArchivo = httpPostedFileBase.FileName;

                        //recorta el nombre del archivo en caso de ser necesario
                        if (nombreArchivo.Length > 80)
                            nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                        //lee el archivo en un arreglo de bytes
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(httpPostedFileBase.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(httpPostedFileBase.ContentLength);
                        }

                        //genera el archivo de biblioce digital
                        biblioteca_digital archivo = new biblioteca_digital
                        {
                            Nombre = nombreArchivo,
                            MimeType = UsoStrings.RecortaString(httpPostedFileBase.ContentType, 80),
                            Datos = fileData
                        };
                        orden_trabajo.OT_rel_archivos.Add(new OT_rel_archivos { tipo = OT_tipo_documento.SOLICITUD, biblioteca_digital = archivo });
                    }
                }
            }


            #endregion

            if (ModelState.IsValid)
            {
                orden_trabajo.fecha_solicitud = DateTime.Now;
                orden_trabajo.estatus = OT_Status.ABIERTO;

                if (!orden_trabajo.tpm)
                {
                    orden_trabajo.numero_tarjeta = null;
                    orden_trabajo.id_grupo_trabajo = null;
                }
                db.orden_trabajo.Add(orden_trabajo);
                try
                {
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                    //envia correo electronico
                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                    List<String> correos = new List<string>(); //correos TO

                    //---INICIO POR ROL                    
                    //recorre los responsables con el permiso de asignar
                    AspNetRoles rol = db.AspNetRoles.Where(x => x.Name == TipoRoles.OT_ASIGNACION).FirstOrDefault();
                    List<AspNetUsers> usuariosInRole = new List<AspNetUsers>();
                    if (rol != null)
                        usuariosInRole = rol.AspNetUsers.ToList();

                    List<int> idsAsignacion = usuariosInRole.Select(x => x.IdEmpleado).Distinct().ToList();

                    List<empleados> listEmpleados = db.empleados.Where(x => x.activo == true && idsAsignacion.Contains(x.id) == true && x.planta_clave == empleado.planta_clave).ToList();

                    foreach (var e in listEmpleados)
                        if (!String.IsNullOrEmpty(e.correo))
                            correos.Add(e.correo);

                    //---FIN POR ROL

                    //-- INICIO POR TABLA NOTIFICACION

                    //List<notificaciones_correo> listadoNotificaciones = db.notificaciones_correo.Where(x => x.descripcion == NotificacionesCorreo.PO_ASIGNACION).ToList();
                    //foreach (var n in listadoNotificaciones)
                    //{
                    //    //verificar si es necesario filtrar por planta
                    //    if (!String.IsNullOrEmpty(n.correo) && !n.id_empleado.HasValue && n.clave_planta == empleado.planta_clave) //clave planta es opcional según la situación
                    //        correos.Add(n.correo);
                    //    else if (n.empleados != null && !String.IsNullOrEmpty(n.empleados.correo) && n.empleados.planta_clave == empleado.planta_clave)
                    //        correos.Add(n.empleados.correo);
                    //}
                    //-- FIN POR TABLA NOTIFICACION

                    orden_trabajo.empleados2 = db.empleados.Find(orden_trabajo.id_solicitante);
                    envioCorreo.SendEmailAsync(correos, "Se ha creado la orden de trabajo #" + orden_trabajo.id + " y se encuentra en espera de asignación.", envioCorreo.getBodyCreacionOT(orden_trabajo));

                    return RedirectToAction("ListadoUsuario");
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("ListadoUsuario");
                }
            }

            //En caso de que el modelo no sea válido

            //obtiene los departamentos donde aplica linea de producción
            List<int> listAreasIds = db.OT_rel_depto_aplica_linea.Where(x => x.id_area > 0).Select(x => x.id_area).Distinct().ToList();

            if (empleado.id_area.HasValue && listAreasIds.Contains(empleado.id_area.Value))
                ViewBag.MuestraLineas = true;

            //crea el select list para status
            List<SelectListItem> selectListUrgencia = obtieneSelectListEstatus();

            SelectList selectListItemsUrgencia = new SelectList(selectListUrgencia, "Value", "Text");
            ViewBag.nivel_urgencia = AddFirstItem(selectListItemsUrgencia, textoPorDefecto: "-- Seleccionar --", selected: orden_trabajo.estatus);
            ViewBag.Solicitante = empleado;
            ViewBag.id_linea = AddFirstItem(new SelectList(db.produccion_lineas.Where(x => x.clave_planta == empleado.planta_clave && x.activo == true), "id", "linea"), selected: orden_trabajo.id_linea.ToString());
            ViewBag.id_grupo_trabajo = AddFirstItem(new SelectList(db.OT_grupo_trabajo.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_zona_falla = AddFirstItem(new SelectList(db.OT_zona_falla.Where(x => x.id_linea == orden_trabajo.id_linea && x.activo), "id", "zona_falla"), textoPorDefecto: "-- N/A --", selected: orden_trabajo.id_zona_falla.ToString());
            return View(orden_trabajo);
        }

        // GET: ListadoAsignacionPendientes
        public ActionResult ListadoAsignacionPendientes(int pagina = 1)
        {
            if (TieneRol(TipoRoles.OT_ASIGNACION))
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
                    .Where(x => x.estatus == OT_Status.ABIERTO
                    && x.empleados2.planta_clave == empleado.planta_clave
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                     .Where(x => x.estatus == OT_Status.ABIERTO
                     && x.empleados2.planta_clave == empleado.planta_clave
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
        public ActionResult ListadoAsignacionAsignadas(string estatus, string prioridad, int? id_responsable, int? id_solicitante, int pagina = 1)
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
                    && x.empleados2.planta_clave == empleado.planta_clave
                    && (String.IsNullOrEmpty(estatus) || x.estatus.Contains(estatus))
                    && (String.IsNullOrEmpty(prioridad) || x.nivel_urgencia.Contains(prioridad))
                    && (id_responsable == null || x.id_responsable == id_responsable)
                    && (id_solicitante == null || x.id_solicitante == id_solicitante)
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                     .Where(x => x.estatus != OT_Status.ABIERTO
                     && x.empleados2.planta_clave == empleado.planta_clave
                     && (String.IsNullOrEmpty(estatus) || x.estatus.Contains(estatus))
                    && (String.IsNullOrEmpty(prioridad) || x.nivel_urgencia.Contains(prioridad))
                    && (id_responsable == null || x.id_responsable == id_responsable)
                    && (id_solicitante == null || x.id_solicitante == id_solicitante)
                    )
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["estatus"] = estatus;
                routeValues["prioridad"] = prioridad;
                routeValues["id_responsable"] = id_responsable;
                routeValues["id_solicitante"] = id_solicitante;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;

                //List Estatus
                List<string> estatusList = db.orden_trabajo.Select(x => x.estatus).Distinct().ToList();
                //crea un Select  list para el estatus
                List<SelectListItem> newListEstatus = new List<SelectListItem>();

                foreach (string statusItem in estatusList)
                {
                    newListEstatus.Add(new SelectListItem()
                    {
                        Text = OT_Status.DescripcionStatus(statusItem),
                        Value = statusItem
                    });
                }

                //List Prioridad
                List<string> prioridadList = db.orden_trabajo.Select(x => x.nivel_urgencia).Distinct().ToList();
                List<SelectListItem> newListPrioridad = new List<SelectListItem>();

                foreach (string pr in prioridadList)
                {
                    newListPrioridad.Add(new SelectListItem()
                    {
                        Text = OT_nivel_urgencia.DescripcionStatus(pr),
                        Value = pr
                    });
                }

                //List Solicitante
                List<empleados> solicitanteList = db.empleados.Where(x => x.orden_trabajo2.Any(y => y.estatus != Bitacoras.Util.OT_Status.ABIERTO) && x.planta_clave == empleado.planta_clave).ToList();
                //List Responsable
                List<empleados> reponsableList = db.empleados.Where(x => x.orden_trabajo1.Any(y => y.estatus != Bitacoras.Util.OT_Status.ABIERTO) && x.planta_clave == empleado.planta_clave).ToList();

                //creación de selectList
                SelectList selectListItemsStatus = new SelectList(newListEstatus, "Value", "Text", estatus);
                SelectList selectListItemsPrioridad = new SelectList(newListPrioridad, "Value", "Text", prioridad);

                //ViewBags
                ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Todos --");
                ViewBag.prioridad = AddFirstItem(selectListItemsPrioridad, textoPorDefecto: "-- Todos --");
                ViewBag.id_solicitante = AddFirstItem(new SelectList(solicitanteList, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), textoPorDefecto: "-- Todos --", selected: id_solicitante.ToString());
                ViewBag.id_responsable = AddFirstItem(new SelectList(reponsableList, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), textoPorDefecto: "-- Todos --", selected: id_responsable.ToString());

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        //// GET: OrdenesTrabajo/Edit
        public ActionResult Edit(int? id, string redirect = "")
        {

            if (TieneRol(TipoRoles.OT_ASIGNACION)|| TieneRol(TipoRoles.OT_ADMINISTRADOR))
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

                //verifica si se puede visualizar
                if (orden_trabajo.Area.plantaClave != empleado.planta_clave)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede visualizar esta solicitud!";
                    ViewBag.Descripcion = "No se puede editar una orden de trabajo de otra planta.";

                    return View("../Home/ErrorGenerico");
                }


                //obtiene los departamentos donde aplica linea de producción
                List<int> listAreasIds = db.OT_rel_depto_aplica_linea.Where(x => x.id_area > 0).Select(x => x.id_area).Distinct().ToList();

                if (orden_trabajo.empleados2.id_area.HasValue && listAreasIds.Contains(orden_trabajo.empleados2.id_area.Value))
                    ViewBag.MuestraLineas = true;

                //crea el select list para status
                List<SelectListItem> selectListUrgencia = obtieneSelectListEstatus();

                ViewBag.redirect = redirect;

                SelectList selectListItemsUrgencia = new SelectList(selectListUrgencia, "Value", "Text");
                ViewBag.nivel_urgencia = AddFirstItem(selectListItemsUrgencia, textoPorDefecto: "-- Seleccionar --", selected: orden_trabajo.nivel_urgencia);
                ViewBag.Solicitante = orden_trabajo.empleados2;
                ViewBag.id_linea = AddFirstItem(new SelectList(db.produccion_lineas.Where(x => x.clave_planta == orden_trabajo.empleados2.planta_clave && x.activo == true), "id", "linea"), selected: orden_trabajo.id_linea.ToString()); ;
                ViewBag.id_grupo_trabajo = AddFirstItem(new SelectList(db.OT_grupo_trabajo.Where(x => x.activo == true), "id", "descripcion"), selected: orden_trabajo.id_grupo_trabajo.ToString());
                ViewBag.id_zona_falla = AddFirstItem(new SelectList(db.OT_zona_falla.Where(x => x.id_linea == orden_trabajo.id_linea && x.activo), "id", "zona_falla"), textoPorDefecto: "-- N/A --", selected: orden_trabajo.id_zona_falla.ToString());
                return View(orden_trabajo);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: OrdenesTrabajo/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(orden_trabajo orden_trabajo, string redirect = "")
        {

            if (ModelState.IsValid)
            {

                if (!orden_trabajo.tpm)
                {
                    orden_trabajo.numero_tarjeta = null;
                    orden_trabajo.id_grupo_trabajo = null;
                }
                //db.orden_trabajo.Add(orden_trabajo);
                try
                {
                    orden_trabajo item = db.orden_trabajo.Find(orden_trabajo.id);

                    item.id_linea = orden_trabajo.id_linea;
                    item.id_grupo_trabajo = orden_trabajo.id_grupo_trabajo;
                    item.nivel_urgencia = orden_trabajo.nivel_urgencia;
                    item.titulo = orden_trabajo.titulo;
                    item.descripcion = orden_trabajo.descripcion;
                    item.tpm = orden_trabajo.tpm;
                    item.numero_tarjeta = orden_trabajo.numero_tarjeta;
                    item.id_zona_falla = orden_trabajo.id_zona_falla;

                    db.Entry(item).State = EntityState.Modified;

                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ha actualizado la Orden de Trabajo correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                    if (String.IsNullOrEmpty(redirect))
                        redirect = "ListadoAsignacionPendientes";

                    return RedirectToAction(redirect);
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("ListadoAsignacionPendientes");
                }
            }

            //En caso de que el modelo no sea válido

            //obtiene los departamentos donde aplica linea de producción
            List<int> listAreasIds = db.OT_rel_depto_aplica_linea.Where(x => x.id_area > 0).Select(x => x.id_area).Distinct().ToList();

            orden_trabajo.empleados2 = db.empleados.Find(orden_trabajo.id_solicitante);

            if (orden_trabajo.empleados2.id_area.HasValue && listAreasIds.Contains(orden_trabajo.empleados2.id_area.Value))
                ViewBag.MuestraLineas = true;

            ViewBag.redirect = redirect;

            //crea el select list para status
            List<SelectListItem> selectListUrgencia = obtieneSelectListEstatus();

            SelectList selectListItemsUrgencia = new SelectList(selectListUrgencia, "Value", "Text");
            ViewBag.nivel_urgencia = AddFirstItem(selectListItemsUrgencia, textoPorDefecto: "-- Seleccionar --", selected: orden_trabajo.nivel_urgencia);
            ViewBag.Solicitante = orden_trabajo.empleados2;
            ViewBag.id_linea = AddFirstItem(new SelectList(db.produccion_lineas.Where(x => x.clave_planta == orden_trabajo.empleados2.planta_clave && x.activo == true), "id", "linea"), selected: orden_trabajo.id_linea.ToString());
            ViewBag.id_grupo_trabajo = AddFirstItem(new SelectList(db.OT_grupo_trabajo.Where(x => x.activo == true), "id", "descripcion"), selected: orden_trabajo.id_grupo_trabajo.ToString());
            ViewBag.id_zona_falla = AddFirstItem(new SelectList(db.OT_zona_falla.Where(x => x.id_linea == orden_trabajo.id_linea && x.activo), "id", "zona_falla"), textoPorDefecto: "-- N/A --", selected: orden_trabajo.id_zona_falla.ToString());
            return View(orden_trabajo);
        }


        #region Listados Responsables
        // GET: ListadoResponsablePendientes
        public ActionResult ListadoResponsablePendientes(int? id_responsable, int? id_solicitante, int pagina = 1)
        {
            if (TieneRol(TipoRoles.OT_RESPONSABLE) || TieneRol(TipoRoles.OT_ADMINISTRADOR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el id de supervisor y tecnicos
                var responsable = db.OT_responsables.Where(x => x.id_empleado == empleado.id).FirstOrDefault();

                List<int> idsResponsables = new List<int>();

                if (responsable != null)
                    idsResponsables = db.OT_responsables.Where(x =>
                        (x.id_empleado == responsable.id_empleado || (x.id_empleado == responsable.id_supervisor && responsable.id_supervisor.HasValue) ||
                        x.id_supervisor == responsable.id_empleado || (x.id_supervisor == responsable.id_supervisor && responsable.id_supervisor.HasValue)
                        )).Select(x => x.id_empleado).Distinct().ToList();


                var listado = db.orden_trabajo
                    .Where(x => x.estatus == OT_Status.ASIGNADO
                    && idsResponsables.Contains(x.id_responsable.Value)
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                     .Where(x => x.estatus == OT_Status.ASIGNADO
                     && idsResponsables.Contains(x.id_responsable.Value)
                    )
                    .Count();

                //realiza nuevamente la búsqueda sí es Administrador
                if (TieneRol(TipoRoles.OT_ADMINISTRADOR))
                {
                    listado = db.orden_trabajo
                     .Where(x => x.estatus == OT_Status.ASIGNADO
                     && (id_responsable == null || x.id_responsable == id_responsable)
                     && (id_solicitante == null || x.id_solicitante == id_solicitante)
                     && x.empleados2.planta_clave == empleado.planta_clave
                     )
                     .OrderByDescending(x => x.fecha_solicitud)
                     .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                    totalDeRegistros = db.orden_trabajo
                     .Where(x => x.estatus == OT_Status.ASIGNADO
                     && (id_responsable == null || x.id_responsable == id_responsable)
                     && (id_solicitante == null || x.id_solicitante == id_solicitante)
                     && x.empleados2.planta_clave == empleado.planta_clave
                     )
                     .Count();

                }

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_responsable"] = id_responsable;
                routeValues["id_solicitante"] = id_solicitante;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                //List Solicitante
                List<empleados> solicitanteList = db.empleados.Where(x => x.orden_trabajo2.Any(y => y.estatus == Bitacoras.Util.OT_Status.ASIGNADO) && x.planta_clave == empleado.planta_clave).ToList();
                //List Responsable
                List<empleados> reponsableList = db.empleados.Where(x => x.orden_trabajo1.Any(y => y.estatus == Bitacoras.Util.OT_Status.ASIGNADO) && x.planta_clave == empleado.planta_clave).ToList();

                ViewBag.id_solicitante = AddFirstItem(new SelectList(solicitanteList, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), textoPorDefecto: "-- Todos --", selected: id_solicitante.ToString());
                ViewBag.id_responsable = AddFirstItem(new SelectList(reponsableList, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), textoPorDefecto: "-- Todos --", selected: id_responsable.ToString());

                ViewBag.Title = "Listado de Solicitudes Asignadas";
                ViewBag.SegundoNivel = "ListadoAginacionPendientes";
                ViewBag.Info = true;
                ViewBag.CambioEstatus = true;

                ViewBag.Paginacion = paginacion;

                return View("ListadoResponsable", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }


        // GET: ListadoResponsableEnProceso
        public ActionResult ListadoResponsableEnProceso(int? id_responsable, int? id_solicitante, int pagina = 1)
        {
            if (TieneRol(TipoRoles.OT_RESPONSABLE) || TieneRol(TipoRoles.OT_ADMINISTRADOR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el id de supervisor y tecnicos
                var responsable = db.OT_responsables.Where(x => x.id_empleado == empleado.id).FirstOrDefault();

                List<int> idsResponsables = new List<int>();

                if (responsable != null)
                    idsResponsables = db.OT_responsables.Where(x =>
                        (x.id_empleado == responsable.id_empleado || (x.id_empleado == responsable.id_supervisor && responsable.id_supervisor.HasValue) ||
                        x.id_supervisor == responsable.id_empleado || (x.id_supervisor == responsable.id_supervisor && responsable.id_supervisor.HasValue)
                        )).Select(x => x.id_empleado).Distinct().ToList();

                var listado = db.orden_trabajo
                    .Where(x => x.estatus == OT_Status.EN_PROCESO && idsResponsables.Contains(x.id_responsable.Value)
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                 .Where(x => x.estatus == OT_Status.EN_PROCESO && idsResponsables.Contains(x.id_responsable.Value)
                )
                .Count();

                //realiza nuevamente la búsqueda sí es Administrador
                if (TieneRol(TipoRoles.OT_ADMINISTRADOR))
                {
                    listado = db.orden_trabajo
                     .Where(x => x.estatus == OT_Status.EN_PROCESO
                     && (id_responsable == null || x.id_responsable == id_responsable)
                     && (id_solicitante == null || x.id_solicitante == id_solicitante)
                     && x.empleados2.planta_clave == empleado.planta_clave
                     )
                     .OrderByDescending(x => x.fecha_solicitud)
                     .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                    totalDeRegistros = db.orden_trabajo
                     .Where(x => x.estatus == OT_Status.EN_PROCESO
                     && (id_responsable == null || x.id_responsable == id_responsable)
                     && (id_solicitante == null || x.id_solicitante == id_solicitante)
                     && x.empleados2.planta_clave == empleado.planta_clave
                     )
                     .Count();

                }

                //para paginación
                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_responsable"] = id_responsable;
                routeValues["id_solicitante"] = id_solicitante;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                //List Solicitante
                List<empleados> solicitanteList = db.empleados.Where(x => x.orden_trabajo2.Any(y => y.estatus == Bitacoras.Util.OT_Status.EN_PROCESO) && x.planta_clave == empleado.planta_clave).ToList();
                //List Responsable
                List<empleados> reponsableList = db.empleados.Where(x => x.orden_trabajo1.Any(y => y.estatus == Bitacoras.Util.OT_Status.EN_PROCESO) && x.planta_clave == empleado.planta_clave).ToList();

                ViewBag.id_solicitante = AddFirstItem(new SelectList(solicitanteList, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), textoPorDefecto: "-- Todos --", selected: id_solicitante.ToString());
                ViewBag.id_responsable = AddFirstItem(new SelectList(reponsableList, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), textoPorDefecto: "-- Todos --", selected: id_responsable.ToString());

                ViewBag.Title = "Listado de Solicitudes en Proceso";
                ViewBag.SegundoNivel = "ListadoResponsableEnProceso";
                ViewBag.Paginacion = paginacion;
                ViewBag.Info = true;
                ViewBag.Cerrar = true;

                return View("ListadoResponsable", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: ListadoResponsableTerminadas
        public ActionResult ListadoResponsableTerminadas(int? id_responsable, int? id_solicitante, int pagina = 1)
        {
            if (TieneRol(TipoRoles.OT_RESPONSABLE) || TieneRol(TipoRoles.OT_ADMINISTRADOR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el id de supervisor y tecnicos
                var responsable = db.OT_responsables.Where(x => x.id_empleado == empleado.id).FirstOrDefault();

                List<int> idsResponsables = new List<int>();

                if (responsable != null)
                    idsResponsables = db.OT_responsables.Where(x =>
                        (x.id_empleado == responsable.id_empleado || (x.id_empleado == responsable.id_supervisor && responsable.id_supervisor.HasValue) ||
                        x.id_supervisor == responsable.id_empleado || (x.id_supervisor == responsable.id_supervisor && responsable.id_supervisor.HasValue)
                        )).Select(x => x.id_empleado).Distinct().ToList();

                var listado = db.orden_trabajo
                    .Where(x => x.estatus == OT_Status.CERRADO && idsResponsables.Contains(x.id_responsable.Value)
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                     .Where(x => x.estatus == OT_Status.CERRADO && idsResponsables.Contains(x.id_responsable.Value)
                    )
                    .Count();

                //realiza nuevamente la búsqueda sí es Administrador
                if (TieneRol(TipoRoles.OT_ADMINISTRADOR))
                {
                    listado = db.orden_trabajo
                     .Where(x => x.estatus == OT_Status.CERRADO
                     && (id_responsable == null || x.id_responsable == id_responsable)
                     && (id_solicitante == null || x.id_solicitante == id_solicitante)
                     && x.empleados2.planta_clave == empleado.planta_clave
                     )
                     .OrderByDescending(x => x.fecha_solicitud)
                     .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                    totalDeRegistros = db.orden_trabajo
                     .Where(x => x.estatus == OT_Status.CERRADO
                     && (id_responsable == null || x.id_responsable == id_responsable)
                     && (id_solicitante == null || x.id_solicitante == id_solicitante)
                     && x.empleados2.planta_clave == empleado.planta_clave
                     )
                     .Count();

                }

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_responsable"] = id_responsable;
                routeValues["id_solicitante"] = id_solicitante;


                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                //List Solicitante
                List<empleados> solicitanteList = db.empleados.Where(x => x.orden_trabajo2.Any(y => y.estatus == Bitacoras.Util.OT_Status.EN_PROCESO) && x.planta_clave == empleado.planta_clave).ToList();
                //List Responsable
                List<empleados> reponsableList = db.empleados.Where(x => x.orden_trabajo1.Any(y => y.estatus == Bitacoras.Util.OT_Status.EN_PROCESO) && x.planta_clave == empleado.planta_clave).ToList();

                ViewBag.id_solicitante = AddFirstItem(new SelectList(solicitanteList, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), textoPorDefecto: "-- Todos --", selected: id_solicitante.ToString());
                ViewBag.id_responsable = AddFirstItem(new SelectList(reponsableList, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), textoPorDefecto: "-- Todos --", selected: id_responsable.ToString());


                ViewBag.Title = "Listado de Solicitudes Terminadas";
                ViewBag.SegundoNivel = "ListadoResponsableTerminadas";
                ViewBag.Paginacion = paginacion;
                ViewBag.Info = true;

                return View("ListadoResponsable", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        #endregion

        #region reportes

        // GET: ReporteGeneral
        public ActionResult ReporteGeneral(string estatus, string nivel_urgencia, string fecha_inicial, string fecha_final, bool? tpm, int? id, int? id_responsable, int? id_solicitante, int pagina = 1)
        {
            if (TieneRol(TipoRoles.OT_REPORTE))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                //convierte las fechas recibidas
                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
                DateTime dateFinal = DateTime.Now;          //fecha final por defecto
                DateTime dateTurno = DateTime.Now;          //fecha turno por defecto

                try
                {
                    if (!String.IsNullOrEmpty(fecha_inicial))
                        dateInicial = Convert.ToDateTime(fecha_inicial);
                    if (!String.IsNullOrEmpty(fecha_final))
                    {
                        dateFinal = Convert.ToDateTime(fecha_final);
                        dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                }
                catch (FormatException e)
                {
                    Console.WriteLine("Error de Formato: " + e.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al convertir: " + ex.Message);
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();


                var listado = db.orden_trabajo
                    .Where(x =>
                       (id == null || x.id == id)
                            && (x.empleados2.planta_clave == empleado.planta_clave) //filtra por planta
                            && (String.IsNullOrEmpty(estatus) || x.estatus.Contains(estatus))
                            && (x.tpm == tpm || tpm.HasValue == false)
                            && (id_responsable == null || x.id_responsable == id_responsable)
                            && (id_solicitante == null || x.id_solicitante == id_solicitante)
                            && (String.IsNullOrEmpty(nivel_urgencia) || x.nivel_urgencia.Contains(nivel_urgencia))
                            && x.fecha_solicitud >= dateInicial && x.fecha_solicitud <= dateFinal
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                   .Where(x =>
                       (id == null || x.id == id)
                            && (x.empleados2.planta_clave == empleado.planta_clave) //filtra por planta
                            && (String.IsNullOrEmpty(estatus) || x.estatus.Contains(estatus))
                            && (x.tpm == tpm || tpm.HasValue == false)
                            && (id_responsable == null || x.id_responsable == id_responsable)
                            && (id_solicitante == null || x.id_solicitante == id_solicitante)
                            && (String.IsNullOrEmpty(nivel_urgencia) || x.nivel_urgencia.Contains(nivel_urgencia))
                            && x.fecha_solicitud >= dateInicial && x.fecha_solicitud <= dateFinal
                    )
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id"] = id;
                routeValues["tpm"] = tpm;
                routeValues["estatus"] = estatus;
                routeValues["nivel_urgencia"] = nivel_urgencia;
                routeValues["fecha_inicial"] = fecha_inicial;
                routeValues["fecha_final"] = fecha_final;
                routeValues["id_responsable"] = id_responsable;
                routeValues["id_solicitante"] = id_solicitante;


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

                List<string> urgenciaList = db.orden_trabajo.Select(x => x.nivel_urgencia).Distinct().ToList();
                //crea un Select  list para el estatus
                List<SelectListItem> newListUrgencia = new List<SelectListItem>();

                foreach (string item in urgenciaList)
                {
                    newListUrgencia.Add(new SelectListItem()
                    {
                        Text = OT_nivel_urgencia.DescripcionStatus(item),
                        Value = item
                    });
                }

                //crea un select List para tpm
                List<SelectListItem> newListTpm = new List<SelectListItem>();

                newListTpm.Add(new SelectListItem()
                {
                    Text = "Sí",
                    Value = "true"
                });
                newListTpm.Add(new SelectListItem()
                {
                    Text = "No",
                    Value = "false"
                });

                //List Solicitante
                List<empleados> solicitanteList = db.empleados.Where(x => x.orden_trabajo2.Any() && x.planta_clave == empleado.planta_clave).ToList();
                //List Responsable
                List<empleados> reponsableList = db.empleados.Where(x => x.orden_trabajo1.Any() && x.planta_clave == empleado.planta_clave).ToList();



                SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
                SelectList selectListItemsNivelUrgencia = new SelectList(newListUrgencia, "Value", "Text", nivel_urgencia);
                SelectList selectListItemsTPM = new SelectList(newListTpm, "Value", "Text", tpm);

                ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Todos --");
                ViewBag.nivel_urgencia = AddFirstItem(selectListItemsNivelUrgencia, textoPorDefecto: "-- Todos --");
                ViewBag.tpm = AddFirstItem(selectListItemsTPM, textoPorDefecto: "-- Todos --");
                ViewBag.id_solicitante = AddFirstItem(new SelectList(solicitanteList, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), textoPorDefecto: "-- Todos --", selected: id_solicitante.ToString());
                ViewBag.id_responsable = AddFirstItem(new SelectList(reponsableList, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), textoPorDefecto: "-- Todos --", selected: id_responsable.ToString());

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones               
                ViewBag.Title = "Reporte de Órdenes de Trabajo";
                ViewBag.SegundoNivel = "ReporteGeneral";
                ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true && p.clave == empleado.planta_clave), "clave", "descripcion");

                // ViewBag.Create = true;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }


        public ActionResult Exportar(string estatus, string nivel_urgencia, string fecha_inicial, string fecha_final, bool? tpm, int? id, int? id_responsable, int? id_solicitante)
        {
            if (TieneRol(TipoRoles.OT_REPORTE))
            {
                //convierte las fechas recibidas
                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
                DateTime dateFinal = DateTime.Now;          //fecha final por defecto
                DateTime dateTurno = DateTime.Now;          //fecha turno por defecto

                try
                {
                    if (!String.IsNullOrEmpty(fecha_inicial))
                        dateInicial = Convert.ToDateTime(fecha_inicial);
                    if (!String.IsNullOrEmpty(fecha_final))
                    {
                        dateFinal = Convert.ToDateTime(fecha_final);
                        dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                }
                catch (FormatException e)
                {
                    Console.WriteLine("Error de Formato: " + e.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al convertir: " + ex.Message);
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.orden_trabajo
                   .Where(x =>
                      (id == null || x.id == id)
                        && (x.empleados2.planta_clave == empleado.planta_clave) //filtra por planta
                        && (String.IsNullOrEmpty(estatus) || x.estatus.Contains(estatus))
                        && (x.tpm == tpm || tpm.HasValue == false)
                        && (id_responsable == null || x.id_responsable == id_responsable)
                        && (id_solicitante == null || x.id_solicitante == id_solicitante)
                        && (String.IsNullOrEmpty(nivel_urgencia) || x.nivel_urgencia.Contains(nivel_urgencia))
                        && x.fecha_solicitud >= dateInicial && x.fecha_solicitud <= dateFinal
                   )
                   .ToList();

                byte[] stream = ExcelUtil.GeneraReporteOrdenesTrabajo(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Reporte_Ordenes_Trabajo_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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

        // GET: OrdenesTrabajo/Asignar/5
        public ActionResult Asignar(int? id)
        {
            if (TieneRol(TipoRoles.OT_ASIGNACION))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                orden_trabajo orden_trabajo = db.orden_trabajo.Find(id);
                if (orden_trabajo == null)
                {
                    return View("../Error/NotFound");
                }

                //verifica si se puede asignar
                if (orden_trabajo.estatus == OT_Status.CERRADO)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede asignar o reasignar esta solicitud!";
                    ViewBag.Descripcion = "No se puede asignar o reasignar una solicitud con estatus CERRADO.";

                    return View("../Home/ErrorGenerico");
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //verifica si se puede asignar
                if (orden_trabajo.empleados2.planta_clave != empleado.planta_clave)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede asignar o reasignar esta solicitud!";
                    ViewBag.Descripcion = "No se puede asignar una orden de trabajo de otra planta.";

                    return View("../Home/ErrorGenerico");
                }



                //---INICIO POR ROL                    
                //recorre los responsables con el permiso de asignar
                AspNetRoles rol = db.AspNetRoles.Where(x => x.Name == TipoRoles.OT_RESPONSABLE).FirstOrDefault();
                List<AspNetUsers> usuariosInRole = new List<AspNetUsers>();
                if (rol != null)
                    usuariosInRole = rol.AspNetUsers.ToList();

                List<int> idsResponsables = usuariosInRole.Select(x => x.IdEmpleado).Distinct().ToList();

                ViewBag.id_responsable = AddFirstItem(new SelectList(db.empleados.Where(x =>
                idsResponsables.Contains(x.id) && x.planta_clave == empleado.planta_clave
                ), "id", "ConcatNumEmpleadoNombre"), selected: orden_trabajo.id_responsable.ToString());

                //--fin por rol

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

            bool reasignacion = ordenOld.id_responsable.HasValue;

            if (ordenOld == null)
                ModelState.AddModelError("", "No se encontró el orden del trabajo asociada.");

            if (ordenOld.id_responsable.HasValue && ordenOld.id_responsable.Value == orden_trabajo.id_responsable)
                ModelState.AddModelError("", "No se puede reasignar una solicitud al mismo responsable. Seleccione un responsable diferente.");

            if (ModelState.IsValid)
            {
                ordenOld.id_asignacion = empleado.id;
                ordenOld.id_responsable = orden_trabajo.id_responsable;
                ordenOld.fecha_asignacion = DateTime.Now;
                ordenOld.estatus = OT_Status.ASIGNADO;
                ordenOld.fecha_en_proceso = null;

                db.Entry(ordenOld).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                    //envia correo electronico
                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                    List<String> correos = new List<string>(); //correos TO

                    empleados responsable = db.empleados.Find(orden_trabajo.id_responsable);

                    if (responsable != null && !String.IsNullOrEmpty(responsable.correo))
                        correos.Add(responsable.correo); //agrega correo de elaborador

                    envioCorreo.SendEmailAsync(correos, "Se le ha asignado la Orden de Trabajo #" + orden_trabajo.id + ".", envioCorreo.getBodyAsignacionOT(ordenOld));

                    if (reasignacion)
                        return RedirectToAction("ListadoAsignacionAsignadas");
                    else
                        return RedirectToAction("ListadoAsignacionPendientes");
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    if (reasignacion)
                        return RedirectToAction("ListadoAsignacionAsignadas");
                    else
                        return RedirectToAction("ListadoAsignacionPendientes");
                }
            }

            //En caso de que el modelo no sea válido

            //---INICIO POR ROL                    
            //recorre los responsables con el permiso de asignar
            AspNetRoles rol = db.AspNetRoles.Where(x => x.Name == TipoRoles.OT_RESPONSABLE).FirstOrDefault();
            List<AspNetUsers> usuariosInRole = new List<AspNetUsers>();
            if (rol != null)
                usuariosInRole = rol.AspNetUsers.ToList();

            List<int> idsResponsables = usuariosInRole.Select(x => x.IdEmpleado).Distinct().ToList();

            ViewBag.id_responsable = AddFirstItem(new SelectList(db.empleados.Where(x =>
            idsResponsables.Contains(x.id) && x.planta_clave == empleado.planta_clave
            ), "id", "ConcatNumEmpleadoNombre"), selected: orden_trabajo.id_responsable.ToString());
            //--fin por rol

            return View(ordenOld);
        }

        // GET: OrdenesTrabajo/CambiarEstatus/5
        public ActionResult CambiarEstatus(int? id)
        {
            if (TieneRol(TipoRoles.OT_RESPONSABLE) || TieneRol(TipoRoles.OT_ADMINISTRADOR))
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

                //verifica si se puede enviar para validacion
                if (orden_trabajo.estatus != OT_Status.ASIGNADO)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡Sólo se pueden marcar como EN PROCESO, aquellas solicitudes con estatus de ASIGNADA!";
                    ViewBag.Descripcion = "Esta solicitud se encuentra en un estatus diferente a ASIGNADA y no puede modificarse.";

                    return View("../Home/ErrorGenerico");
                }

                //obtiene el id de supervisor y tecnicos
                var responsable = db.OT_responsables.Where(x => x.id_empleado == empleado.id).FirstOrDefault();

                List<int> idsResponsables = new List<int>();

                if (responsable != null)
                    idsResponsables = db.OT_responsables.Where(x =>
                        (x.id_empleado == responsable.id_empleado || (x.id_empleado == responsable.id_supervisor && responsable.id_supervisor.HasValue) ||
                        x.id_supervisor == responsable.id_empleado || (x.id_supervisor == responsable.id_supervisor && responsable.id_supervisor.HasValue)
                        )).Select(x => x.id_empleado).Distinct().ToList();

                if ((!idsResponsables.Contains(empleado.id) && !TieneRol(TipoRoles.OT_ADMINISTRADOR)) || empleado.planta_clave != orden_trabajo.empleados2.planta_clave)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡Esta solicitud se encuentra asignada a otro usuario o supervisor!";
                    ViewBag.Descripcion = "No se puede modificar una solicitud que ha sido asignada a otro usuario o supervisor.";

                    return View("../Home/ErrorGenerico");
                }



                return View(orden_trabajo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }


        // POST: OrdenesTrabajo/CambiarEstatus
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("CambiarEstatus")]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstatusConfirmed(int? id)
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

            //verifica que existan objetos asociados
            orden_trabajo ordenOld = db.orden_trabajo.Find(orden_trabajo.id);

            ordenOld.fecha_en_proceso = DateTime.Now;
            ordenOld.estatus = OT_Status.EN_PROCESO;
            ordenOld.id_responsable = empleado.id;

            db.Entry(ordenOld).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>(); //correos TO

                empleados emp = db.empleados.Find(orden_trabajo.id_solicitante);
                if (emp != null && !String.IsNullOrEmpty(emp.correo))
                    correos.Add(emp.correo); //agrega correo de elaborador

                empleados emp2 = db.empleados.Find(orden_trabajo.id_asignacion);
                if (emp2 != null && !String.IsNullOrEmpty(emp2.correo))
                    correos.Add(emp2.correo); //agrega correo de elaborador

                envioCorreo.SendEmailAsync(correos, "La solicitud de Orden de Trabajo #" + orden_trabajo.id + " se encuentra en proceso.", envioCorreo.getBodyEnProcesoOT(ordenOld));


            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }

            return RedirectToAction("ListadoResponsablePendientes");

        }


        // GET: OrdenesTrabajo/CerrarOrden/5
        public ActionResult CerrarOrden(int? id)
        {
            if (TieneRol(TipoRoles.OT_RESPONSABLE) || TieneRol(TipoRoles.OT_ADMINISTRADOR))
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

                //verifica si se puede enviar para validacion
                if (orden_trabajo.estatus != OT_Status.EN_PROCESO)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡Sólo se pueden Cerrar aquellas solicitudes con estatus de EN PROCESO!";
                    ViewBag.Descripcion = "Esta solicitud se encuentra en un estatus diferente a EN PROCESO y no puede modificarse.";

                    return View("../Home/ErrorGenerico");
                }

                //obtiene el id de supervisor y tecnicos
                var responsable = db.OT_responsables.Where(x => x.id_empleado == empleado.id).FirstOrDefault();

                List<int> idsResponsables = new List<int>();

                if (responsable != null)
                    idsResponsables = db.OT_responsables.Where(x =>
                        (x.id_empleado == responsable.id_empleado || (x.id_empleado == responsable.id_supervisor && responsable.id_supervisor.HasValue) ||
                        x.id_supervisor == responsable.id_empleado || (x.id_supervisor == responsable.id_supervisor && responsable.id_supervisor.HasValue)
                        )).Select(x => x.id_empleado).Distinct().ToList();

                if ((!idsResponsables.Contains(empleado.id) && !TieneRol(TipoRoles.OT_ADMINISTRADOR)) || empleado.planta_clave != orden_trabajo.empleados2.planta_clave)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡Esta solicitud se encuentra asignada a otro usuario!";
                    ViewBag.Descripcion = "No se puede modificar una solicitud que ha sido asignada a otro usuario.";

                    return View("../Home/ErrorGenerico");
                }

                return View(orden_trabajo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: OrdenesTrabajo/CambiarEstatus
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("CerrarOrden")]
        [ValidateAntiForgeryToken]
        public ActionResult CerrarOrdenConfirmed(orden_trabajo orden)
        {
            //verifica que existan objetos asociados
            orden_trabajo ordenOld = db.orden_trabajo.Find(orden.id);

            biblioteca_digital archivo = new biblioteca_digital { };

            List<HttpPostedFileBase> archivosForm = new List<HttpPostedFileBase>();
            //agrega archivos enviados
            if (orden.PostedFileCierre_1 != null)
                archivosForm.Add(orden.PostedFileCierre_1);
            if (orden.PostedFileCierre_2 != null)
                archivosForm.Add(orden.PostedFileCierre_2);
            if (orden.PostedFileCierre_3 != null)
                archivosForm.Add(orden.PostedFileCierre_3);
            if (orden.PostedFileCierre_4 != null)
                archivosForm.Add(orden.PostedFileCierre_4);

            #region lectura de archivos

            foreach (HttpPostedFileBase httpPostedFileBase in archivosForm)
            {
                //verifica si el tamaño del archivo es válido
                if (httpPostedFileBase != null && httpPostedFileBase.InputStream.Length > 10485760)
                    ModelState.AddModelError("", "Sólo se permiten archivos menores a 10MB.");
                else if (httpPostedFileBase != null)
                { //verifica la extensión del archivo
                    string extension = Path.GetExtension(httpPostedFileBase.FileName);
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
                        String nombreArchivo = httpPostedFileBase.FileName;

                        //recorta el nombre del archivo en caso de ser necesario
                        if (nombreArchivo.Length > 80)
                            nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                        //lee el archivo en un arreglo de bytes
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(httpPostedFileBase.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(httpPostedFileBase.ContentLength);
                        }

                        //genera el archivo de biblioteca digital
                        archivo = new biblioteca_digital
                        {
                            Nombre = nombreArchivo,
                            MimeType = UsoStrings.RecortaString(httpPostedFileBase.ContentType, 80),
                            Datos = fileData
                        };

                        ////update en BD
                        //db.biblioteca_digital.Add(archivo);
                        //db.SaveChanges();

                        ordenOld.OT_rel_archivos.Add(new OT_rel_archivos { tipo = OT_tipo_documento.CIERRE, biblioteca_digital = archivo });


                    }
                }
            }
            #endregion


            //agrega los refacciones
            foreach (OT_refacciones item in orden.OT_refacciones)
            {
                try
                {
                    //Values check list
                    if (item.id > 0)
                    {
                        //si existe lo modifica
                        OT_refacciones item_exists = db.OT_refacciones.Find(item.id);
                        // Activity already exist in database and modify it
                        db.Entry(item_exists).CurrentValues.SetValues(item);
                        db.Entry(item_exists).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {//si no existe lo crea 
                        db.OT_refacciones.Add(item);
                        db.SaveChanges();
                    }

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

                    ModelState.AddModelError("", ex.Message);

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }

            }

            if (ModelState.IsValid)
            {

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                ordenOld.fecha_cierre = DateTime.Now;
                ordenOld.estatus = OT_Status.CERRADO;
                ordenOld.comentario = orden.comentario;
                ordenOld.id_responsable = empleado.id;

                db.Entry(ordenOld).State = EntityState.Modified;

                try
                {


                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                    //envia correo electronico
                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                    List<String> correos = new List<string>(); //correos TO

                    empleados emp = db.empleados.Find(ordenOld.id_solicitante);

                    if (emp != null && !String.IsNullOrEmpty(emp.correo))
                        correos.Add(emp.correo); //agrega correo de elaborador

                    empleados emp2 = db.empleados.Find(ordenOld.id_asignacion);
                    if (emp2 != null && !String.IsNullOrEmpty(emp2.correo))
                        correos.Add(emp2.correo); //agrega correo de elaborador

                    envioCorreo.SendEmailAsync(correos, "La solicitud de Orden de Trabajo #" + ordenOld.id + " ha sido cerrada.", envioCorreo.getBodyCerrarOT(ordenOld));

                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                }

                return RedirectToAction("ListadoResponsableEnProceso");
            }
            //si el modelo no es válido
            ordenOld.OT_refacciones = orden.OT_refacciones;

            return View(ordenOld);

        }

        public ActionResult GraficaGeneral()
        {
            if (TieneRol(TipoRoles.OT_REPORTE))
            {
                //obtiene la fecha actual
                DateTime fechaActual = DateTime.Now;

                //crea una fecha al inicio del mes
                DateTime fechaInicial = new DateTime(fechaActual.Year, fechaActual.Month, 1);

                //Determina el último día del mes
                DateTime fechaFinal = fechaInicial.AddMonths(1).AddDays(-1);

                ViewBag.FechaInicial = fechaInicial;
                ViewBag.FechaFinal = fechaFinal;

                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        public ActionResult GraficaTPM()
        {
            if (TieneRol(TipoRoles.OT_REPORTE))
            {
                //obtiene la fecha actual
                DateTime fechaActual = DateTime.Now;

                //crea una fecha al inicio del mes
                DateTime fechaInicial = new DateTime(fechaActual.Year, fechaActual.Month, 1);

                //Determina el último día del mes
                DateTime fechaFinal = fechaInicial.AddMonths(1).AddDays(-1);


                ViewBag.FechaInicial = fechaInicial;
                ViewBag.FechaFinal = fechaFinal;

                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }
        /// <summary>
        /// Muestra el manual de usuario
        /// </summary>
        /// <returns></returns>
        public ActionResult ManualUsuario()
        {

            String ruta = System.Web.HttpContext.Current.Server.MapPath("~/Content/manuales/Manual_Usuario_OT.pdf");

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

            return File(ruta, "application/pdf");


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
