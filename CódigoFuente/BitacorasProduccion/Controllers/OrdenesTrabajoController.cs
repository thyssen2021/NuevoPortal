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

                    //relaciona el archivo con la OT (despues se guarda en BD)
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

        #region Listados Responsables
        // GET: ListadoResponsablePendientes
        public ActionResult ListadoResponsablePendientes(int pagina = 1)
        {
            if (TieneRol(TipoRoles.OT_RESPONSABLE))
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
                    .Where(x => x.estatus == OT_Status.ASIGNADO && x.id_responsable == empleado.id
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                     .Where(x => x.estatus == OT_Status.ASIGNADO && x.id_responsable == empleado.id
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
        public ActionResult ListadoResponsableEnProceso(int pagina = 1)
        {
            if (TieneRol(TipoRoles.OT_RESPONSABLE))
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
                    .Where(x => x.estatus == OT_Status.EN_PROCESO && x.id_responsable == empleado.id
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                     .Where(x => x.estatus == OT_Status.EN_PROCESO && x.id_responsable == empleado.id
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
        public ActionResult ListadoResponsableTerminadas(int pagina = 1)
        {
            if (TieneRol(TipoRoles.OT_RESPONSABLE))
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
                    .Where(x => x.estatus == OT_Status.CERRADO && x.id_responsable == empleado.id
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                     .Where(x => x.estatus == OT_Status.CERRADO && x.id_responsable == empleado.id
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
                ModelState.AddModelError("", "Sólo se pueden asignar órdenes con estatus ABIERTO.");


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

                    //envia correo electronico
                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                    List<String> correos = new List<string>(); //correos TO

                    empleados responsable = db.empleados.Find(orden_trabajo.id_responsable);

                    if (responsable != null && !String.IsNullOrEmpty(responsable.correo))
                        correos.Add(responsable.correo); //agrega correo de elaborador

                    envioCorreo.SendEmailAsync(correos, "Se le ha asignado la poliza orden de trabajo #" + orden_trabajo.id + ".", "Correo de asignación...");


                    return RedirectToAction("ListadoAsignacionPendientes");
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

        // GET: OrdenesTrabajo/CambiarEstatus/5
        public ActionResult CambiarEstatus(int? id)
        {
            if (TieneRol(TipoRoles.OT_RESPONSABLE))
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

                if (orden_trabajo.id_responsable != empleado.id)
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

                envioCorreo.SendEmailAsync(correos, "Su solicicitud de Orden de trabajo ha cambiado de estatus #" + orden_trabajo.id + ".", "Su orden de solicitud ha sido marcada como EN PROCESO.");

                
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
            if (TieneRol(TipoRoles.OT_RESPONSABLE))
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

                if (orden_trabajo.id_responsable != empleado.id)
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

            //verifica si el tamaño del archivo es válido
            if (orden.PostedFileCierre != null && orden.PostedFileCierre.InputStream.Length > 10485760)
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 10MB.");
            else if (orden.PostedFileCierre != null)
            { //verifica la extensión del archivo
                string extension = Path.GetExtension(orden.PostedFileCierre.FileName);
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
                    String nombreArchivo = orden.PostedFileCierre.FileName;

                    //recorta el nombre del archivo en caso de ser necesario
                    if (nombreArchivo.Length > 80)
                        nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                    //lee el archivo en un arreglo de bytes
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(orden.PostedFileCierre.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(orden.PostedFileCierre.ContentLength);
                    }

                    //si tiene archivo hace un update sino hace un create
                    if (ordenOld.id_documento_cierre.HasValue)//si tiene valor hace un update
                    {
                        archivo = db.biblioteca_digital.Find(ordenOld.id_documento_cierre.Value);
                        archivo.Nombre = nombreArchivo;
                        archivo.MimeType = UsoStrings.RecortaString(orden.PostedFileCierre.ContentType, 80);
                        archivo.Datos = fileData;

                        db.Entry(archivo).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    { //si no tiene hace un create 

                        //genera el archivo de biblioteca digital
                        archivo = new biblioteca_digital
                        {
                            Nombre = nombreArchivo,
                            MimeType = UsoStrings.RecortaString(orden.PostedFileCierre.ContentType, 80),
                            Datos = fileData
                        };

                        //update en BD
                        db.biblioteca_digital.Add(archivo);
                        db.SaveChanges();
                    }
                    ordenOld.id_documento_cierre = archivo.Id;

                }
            }


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

                db.Entry(ordenOld).State = EntityState.Modified;

                try
                {


                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                    //envia correo electronico
                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                    List<String> correos = new List<string>(); //correos TO

                    empleados emp = db.empleados.Find(orden.id_solicitante);

                    if (emp != null && !String.IsNullOrEmpty(emp.correo))
                        correos.Add(emp.correo); //agrega correo de elaborador

                    envioCorreo.SendEmailAsync(correos, "Su solicicitud de Orden de trabajo ha cambiado de estatus #" + orden.id + ".", "Su orden de solicitud ha sido marcada como FINALIZADA.");

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
