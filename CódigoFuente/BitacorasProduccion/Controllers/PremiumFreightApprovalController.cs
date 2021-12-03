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
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class PremiumFreightApprovalController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: PremiumFreightAproval
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.PFA_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();


                var pFA = db.PFA.Include(p => p.empleados).Include(p => p.empleados1).Include(p => p.PFA_Recovered_cost).Include(p => p.PFA_Border_port).Include(p => p.PFA_Department).Include(p => p.PFA_Destination_plant).Include(p => p.PFA_Reason).Include(p => p.PFA_Responsible_cost).Include(p => p.PFA_Type_shipment).Include(p => p.PFA_Volume)
                    .Where(x => x.estatus == PFA_Status.CREADO && x.id_solicitante == empleado.id)
                    .OrderByDescending(x => x.date_request);

                //configura los parametros de la vista
                ViewBag.Title = "Listado Solicitudes Creadas";
                ViewBag.SegundoNivel = "PFA_registro";
                ViewBag.Edit = true;
                ViewBag.Details = true;
                ViewBag.SendToAuthorizer = true;
                ViewBag.AuthorizarRechazar = false;
                ViewBag.Create = true;
                return View("ListadoSolicitudes", pFA.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        //Solicitudes enviadas y pendientes de aprobación
        // GET: PremiumFreightAproval
        public ActionResult PendientesCapturista()
        {
            if (TieneRol(TipoRoles.PFA_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                empleados empleado = obtieneEmpleadoLogeado();

                var pFA = db.PFA.Include(p => p.empleados).Include(p => p.empleados1).Include(p => p.PFA_Recovered_cost).Include(p => p.PFA_Border_port).Include(p => p.PFA_Department).Include(p => p.PFA_Destination_plant).Include(p => p.PFA_Reason).Include(p => p.PFA_Responsible_cost).Include(p => p.PFA_Type_shipment).Include(p => p.PFA_Volume)
                    .Where(x => x.estatus == PFA_Status.ENVIADO && x.id_solicitante == empleado.id)
                    .OrderByDescending(x => x.date_request);

                //configura los parametros de la vista
                ViewBag.Title = "Listado Solicitudes Pendientes";
                ViewBag.SegundoNivel = "PFA_registro";
                ViewBag.Edit = false;
                ViewBag.Details = true;
                ViewBag.SendToAuthorizer = false;
                ViewBag.AuthorizarRechazar = false;
                ViewBag.Create = true;
                return View("ListadoSolicitudes", pFA.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        //Solicitudes enviadas y pendientes de aprobación
        // GET: PremiumFreightAproval
        public ActionResult AutorizadasCapturista()
        {
            if (TieneRol(TipoRoles.PFA_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                empleados empleado = obtieneEmpleadoLogeado();

                var pFA = db.PFA.Include(p => p.empleados).Include(p => p.empleados1).Include(p => p.PFA_Recovered_cost).Include(p => p.PFA_Border_port).Include(p => p.PFA_Department).Include(p => p.PFA_Destination_plant).Include(p => p.PFA_Reason).Include(p => p.PFA_Responsible_cost).Include(p => p.PFA_Type_shipment).Include(p => p.PFA_Volume)
                    .Where(x => x.estatus == PFA_Status.APROBADO && x.id_solicitante == empleado.id)
                    .OrderByDescending(x => x.date_request);


                //configura los parametros de la vista
                ViewBag.Title = "Listado Solicitudes Aprobadas";
                ViewBag.SegundoNivel = "PFA_registro";
                ViewBag.Edit = false;
                ViewBag.EditCredit = true;
                ViewBag.Details = true;
                ViewBag.SendToAuthorizer = false;
                ViewBag.AuthorizarRechazar = false;
                ViewBag.Create = true;
                return View("ListadoSolicitudes", pFA.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        //Solicitudes enviadas y pendientes de aprobación
        // GET: PremiumFreightAproval
        public ActionResult RechazadasCapturista()
        {
            if (TieneRol(TipoRoles.PFA_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                empleados empleado = obtieneEmpleadoLogeado();

                var pFA = db.PFA.Include(p => p.empleados).Include(p => p.empleados1).Include(p => p.PFA_Recovered_cost).Include(p => p.PFA_Border_port).Include(p => p.PFA_Department).Include(p => p.PFA_Destination_plant).Include(p => p.PFA_Reason).Include(p => p.PFA_Responsible_cost).Include(p => p.PFA_Type_shipment).Include(p => p.PFA_Volume)
                    .Where(x => x.estatus == PFA_Status.RECHAZADO && x.id_solicitante == empleado.id)
                    .OrderByDescending(x => x.date_request);

                //configura los parametros de la vista
                ViewBag.Title = "Listado Solicitudes Rechazadas";
                ViewBag.SegundoNivel = "PFA_registro";
                ViewBag.Edit = true;
                ViewBag.Details = true;
                ViewBag.SendToAuthorizer = true;
                ViewBag.AuthorizarRechazar = false;
                ViewBag.Create = true;
                return View("ListadoSolicitudes", pFA.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        //Solicitudes enviadas y pendientes de aprobación
        // GET: PremiumFreightAproval
        public ActionResult AutorizadorPendientes()
        {
            if (TieneRol(TipoRoles.PFA_AUTORIZACION))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                empleados empleado = obtieneEmpleadoLogeado();

                var pFA = db.PFA.Include(p => p.empleados).Include(p => p.empleados1).Include(p => p.PFA_Recovered_cost).Include(p => p.PFA_Border_port).Include(p => p.PFA_Department).Include(p => p.PFA_Destination_plant).Include(p => p.PFA_Reason).Include(p => p.PFA_Responsible_cost).Include(p => p.PFA_Type_shipment).Include(p => p.PFA_Volume)
                     .Where(x => x.estatus == PFA_Status.ENVIADO && x.id_PFA_autorizador == empleado.id)
                     .OrderByDescending(x => x.date_request);


                //configura los parametros de la vista
                ViewBag.Title = "Listado Solicitudes Pendientes";
                ViewBag.SegundoNivel = "PFA_autorizar";
                ViewBag.Edit = false;
                ViewBag.Details = false;
                ViewBag.SendToAuthorizer = false;
                ViewBag.AuthorizarRechazar = true;
                return View("ListadoSolicitudes", pFA.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        //Solicitudes enviadas y pendientes de aprobación
        // GET: PremiumFreightAproval
        public ActionResult AutorizadorAutorizadas()
        {
            if (TieneRol(TipoRoles.PFA_AUTORIZACION))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                empleados empleado = obtieneEmpleadoLogeado();

                var pFA = db.PFA.Include(p => p.empleados).Include(p => p.empleados1).Include(p => p.PFA_Recovered_cost).Include(p => p.PFA_Border_port).Include(p => p.PFA_Department).Include(p => p.PFA_Destination_plant).Include(p => p.PFA_Reason).Include(p => p.PFA_Responsible_cost).Include(p => p.PFA_Type_shipment).Include(p => p.PFA_Volume)
                    .Where(x => x.estatus == PFA_Status.APROBADO && x.id_PFA_autorizador == empleado.id)
                    .OrderByDescending(x => x.date_request);

                //configura los parametros de la vista
                ViewBag.Title = "Listado Solicitudes Autorizadas";
                ViewBag.SegundoNivel = "PFA_autorizar";
                ViewBag.Edit = false;
                ViewBag.EditCredit = true;
                ViewBag.Details = true;
                ViewBag.SendToAuthorizer = false;
                ViewBag.AuthorizarRechazar = false;
                return View("ListadoSolicitudes", pFA.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        //Solicitudes enviadas y pendientes de aprobación
        // GET: PremiumFreightAproval
        public ActionResult AutorizadorRechazadas()
        {
            if (TieneRol(TipoRoles.PFA_AUTORIZACION))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                empleados empleado = obtieneEmpleadoLogeado();

                var pFA = db.PFA.Include(p => p.empleados).Include(p => p.empleados1).Include(p => p.PFA_Recovered_cost).Include(p => p.PFA_Border_port).Include(p => p.PFA_Department).Include(p => p.PFA_Destination_plant).Include(p => p.PFA_Reason).Include(p => p.PFA_Responsible_cost).Include(p => p.PFA_Type_shipment).Include(p => p.PFA_Volume)
                    .Where(x => x.estatus == PFA_Status.RECHAZADO && x.id_PFA_autorizador == empleado.id)
                    .OrderByDescending(x => x.date_request);

                //configura los parametros de la vista
                ViewBag.Title = "Listado Solicitudes Rechazadas";
                ViewBag.SegundoNivel = "PFA_autorizar";
                ViewBag.Edit = false;
                ViewBag.Details = true;
                ViewBag.SendToAuthorizer = false;
                ViewBag.AuthorizarRechazar = false;
                return View("ListadoSolicitudes", pFA.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        //Solicitudes enviadas y pendientes de aprobación
        // GET: PremiumFreightAproval
        public ActionResult AutorizadorFinalizadas()
        {
            if (TieneRol(TipoRoles.PFA_AUTORIZACION))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                empleados empleado = obtieneEmpleadoLogeado();

                var pFA = db.PFA.Include(p => p.empleados).Include(p => p.empleados1).Include(p => p.PFA_Recovered_cost).Include(p => p.PFA_Border_port).Include(p => p.PFA_Department).Include(p => p.PFA_Destination_plant).Include(p => p.PFA_Reason).Include(p => p.PFA_Responsible_cost).Include(p => p.PFA_Type_shipment).Include(p => p.PFA_Volume)
                    .Where(x => x.estatus == PFA_Status.FINALIZADO && x.id_PFA_autorizador == empleado.id)
                    .OrderByDescending(x => x.date_request);

                //configura los parametros de la vista
                ViewBag.Title = "Listado Solicitudes Autorizadas";
                ViewBag.SegundoNivel = "PFA_autorizar";
                ViewBag.Edit = false;
                ViewBag.Details = true;
                ViewBag.SendToAuthorizer = false;
                ViewBag.AuthorizarRechazar = false;
                return View("ListadoSolicitudes", pFA.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        //Generación de Reportes
        // GET: PremiumFreightAproval
        public ActionResult ReportesPFA(int? id_PFA_user, int? id_PFA_reason, int? id_PFA_responsible_cost, string fecha_inicial, string fecha_final, string status, int pagina = 1)
        {
            if (TieneRol(TipoRoles.PFA_VISUALIZACION))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
                DateTime dateFinal = DateTime.Now;          //fecha final por defecto

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

                var listado = db.PFA.Include(p => p.empleados).Include(p => p.empleados1).Include(p => p.PFA_Recovered_cost).Include(p => p.PFA_Border_port).Include(p => p.PFA_Department).Include(p => p.PFA_Destination_plant).Include(p => p.PFA_Reason).Include(p => p.PFA_Responsible_cost).Include(p => p.PFA_Type_shipment).Include(p => p.PFA_Volume)
                   .Where(x =>
                   (id_PFA_user == null || x.id_solicitante == id_PFA_user)
                   && (id_PFA_reason == null || x.id_PFA_reason == id_PFA_reason)
                   && (id_PFA_responsible_cost == null || x.id_PFA_responsible_cost == id_PFA_responsible_cost)
                   && x.date_request >= dateInicial && x.date_request <= dateFinal
                   && (String.IsNullOrEmpty(status) || x.estatus == status)
                   )
                   .OrderBy(x => x.id)
                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.PFA.Include(p => p.empleados).Include(p => p.empleados1).Include(p => p.PFA_Recovered_cost).Include(p => p.PFA_Border_port).Include(p => p.PFA_Department).Include(p => p.PFA_Destination_plant).Include(p => p.PFA_Reason).Include(p => p.PFA_Responsible_cost).Include(p => p.PFA_Type_shipment).Include(p => p.PFA_Volume)
                   .Where(x =>
                    (id_PFA_user == null || x.id_solicitante == id_PFA_user)
                   && (id_PFA_reason == null || x.id_PFA_reason == id_PFA_reason)
                   && (id_PFA_responsible_cost == null || x.id_PFA_responsible_cost == id_PFA_responsible_cost)
                   && x.date_request >= dateInicial && x.date_request <= dateFinal
                   && (String.IsNullOrEmpty(status) || x.estatus == status)
                   ).Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id_PFA_user"] = id_PFA_user;
                routeValues["id_PFA_reason"] = id_PFA_reason;
                routeValues["id_PFA_responsible_cost"] = id_PFA_responsible_cost;
                routeValues["fecha_inicial"] = fecha_inicial;
                routeValues["fecha_final"] = fecha_final;
                routeValues["status"] = status;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                List<int> idsCapturistas = db.PFA.Select(x => x.id_solicitante).Distinct().ToList();
                List<string> estatusList = db.PFA.Select(x => x.estatus).Distinct().ToList();

                //crea un Select  list para el estatus
                List<SelectListItem> newList = new List<SelectListItem>();

                foreach (string statusItem in estatusList)
                {
                    newList.Add(new SelectListItem()
                    {
                        Text = statusItem,
                        Value = statusItem
                    });
                }

                SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", string.Empty);


                ViewBag.id_PFA_user = AddFirstItem(new SelectList(db.empleados.Where(x => idsCapturistas.Contains(x.id)), "id", "ConcatNombre"), "-- Todos --");
                ViewBag.id_PFA_reason = AddFirstItem(new SelectList(db.PFA_Reason, "id", "descripcion"), "-- Todos --");
                ViewBag.status = AddFirstItem(selectListItemsStatus, "-- Todos --");
                ViewBag.id_PFA_responsible_cost = AddFirstItem(new SelectList(db.PFA_Responsible_cost, "id", "descripcion"), "-- Todos --");
                ViewBag.Paginacion = paginacion;
                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: PremiumFreightAproval/Details/5
        public ActionResult Details(int? id)
        {

            if (TieneRol(TipoRoles.PFA_REGISTRO) || TieneRol(TipoRoles.PFA_AUTORIZACION)
                || TieneRol(TipoRoles.PFA_VISUALIZACION))
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PFA pFA = db.PFA.Find(id);
                if (pFA == null)
                {
                    return HttpNotFound();
                }
                return View(pFA);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // GET: PremiumFreightAproval/Create
        public ActionResult Create()
        {

            if (TieneRol(TipoRoles.PFA_REGISTRO))
            {
                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();
                ViewBag.Solicitante = empleado;

                //obtiene el listado de ids de empleado de los autorizadores
                List<int?> idsAutorizadores = db.PFA_Autorizador.Select(x => x.id_empleado).ToList();

                ViewBag.id_PFA_autorizador = AddFirstItem(new SelectList(db.empleados.Where(x => idsAutorizadores.Contains(x.id)), "id", "ConcatNombre"));
                ViewBag.id_PFA_recovered_cost = AddFirstItem(new SelectList(db.PFA_Recovered_cost.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_border_port = AddFirstItem(new SelectList(db.PFA_Border_port.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_Department = AddFirstItem(new SelectList(db.PFA_Department.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_destination_plant = AddFirstItem(new SelectList(db.PFA_Destination_plant.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_reason = AddFirstItem(new SelectList(db.PFA_Reason.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_responsible_cost = AddFirstItem(new SelectList(db.PFA_Responsible_cost.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_type_shipment = AddFirstItem(new SelectList(db.PFA_Type_shipment.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume.Where(x => x.activo == true), "id", "descripcion");

                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: PremiumFreightAproval/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PFA pFA)
        {
            if (ModelState.IsValid)
            {


                try
                {
                    biblioteca_digital archivo = new biblioteca_digital { };

                    if (pFA.PostedFile != null) //en caso de haya un archivo seleccionado
                    {
                        if (pFA.PostedFile.InputStream.Length > 5242880)
                        {
                            throw new Exception("Sólo se permiten archivos menores a 5MB");
                        }
                        else //si tiene la longitud correcta
                        {
                            string extension = Path.GetExtension(pFA.PostedFile.FileName);
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
                                )
                            {
                                throw new Exception("Sólo se permiten archivos con extensión .xls, .xlsx, .doc, .docx, .pdf, .png, .jpg, .jpeg, .rar, .zip");
                            }
                            else
                            { //si tiene la extension correcta GUARDA EN BASE DE DATOS

                                String nombreArchivo = pFA.PostedFile.FileName;

                                //recorta el nombre del archivo en caso de ser necesario
                                if (nombreArchivo.Length > 80)
                                {
                                    nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;
                                }

                                //lee el archivo en un arreglo de bytes
                                byte[] fileData = null;
                                using (var binaryReader = new BinaryReader(pFA.PostedFile.InputStream))
                                {
                                    fileData = binaryReader.ReadBytes(pFA.PostedFile.ContentLength);
                                }

                                //genera el archivo de biblioce digital
                                archivo = new biblioteca_digital
                                {
                                    Nombre = nombreArchivo,
                                    MimeType = UsoStrings.RecortaString(pFA.PostedFile.ContentType, 80),
                                    Datos = fileData
                                };

                                //guarda en BD
                                db.biblioteca_digital.Add(archivo);
                                db.SaveChanges();

                            }
                        }
                    }


                    pFA.estatus = PFA_Status.CREADO;
                    pFA.date_request = DateTime.Now;
                    pFA.activo = true;

                    if (archivo.Id > 0) //si existe algún archivo en biblioteca digital
                        pFA.id_document_support = archivo.Id;

                    db.PFA.Add(pFA);
                    db.SaveChanges();

                    //if (pFA.estatus == PFA_Status.RECHAZADO)
                    //    return RedirectToAction("RechazadasCapturista");

                    //agrega el mensaje para sweetalert
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);


                }
                catch (Exception e)
                {

                    ModelState.AddModelError("", e.Message);

                    empleados empleado2 = obtieneEmpleadoLogeado();

                    ViewBag.Solicitante = empleado2;

                    //obtiene el listado de ids de empleado de los autorizadores
                    var idsAutorizadores2 = db.PFA_Autorizador.Select(x => x.id_empleado).ToList();

                    ViewBag.id_PFA_autorizador = AddFirstItem(new SelectList(db.empleados.Where(x => idsAutorizadores2.Contains(x.id)), "id", "ConcatNombre"));
                    ViewBag.id_PFA_recovered_cost = AddFirstItem(new SelectList(db.PFA_Recovered_cost.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_border_port = AddFirstItem(new SelectList(db.PFA_Border_port.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_Department = AddFirstItem(new SelectList(db.PFA_Department.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_destination_plant = AddFirstItem(new SelectList(db.PFA_Destination_plant.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_reason = AddFirstItem(new SelectList(db.PFA_Reason.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_responsible_cost = AddFirstItem(new SelectList(db.PFA_Responsible_cost.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_type_shipment = AddFirstItem(new SelectList(db.PFA_Type_shipment.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume.Where(x => x.activo == true), "id", "descripcion");
                    return View(pFA);
                }



                return RedirectToAction("Index");
            }

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            ViewBag.Solicitante = empleado;

            //obtiene el listado de ids de empleado de los autorizadores
            List<int?> idsAutorizadores = db.PFA_Autorizador.Select(x => x.id_empleado).ToList();

            ViewBag.id_PFA_autorizador = AddFirstItem(new SelectList(db.empleados.Where(x => idsAutorizadores.Contains(x.id)), "id", "ConcatNombre"));
            ViewBag.id_PFA_recovered_cost = AddFirstItem(new SelectList(db.PFA_Recovered_cost.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_border_port = AddFirstItem(new SelectList(db.PFA_Border_port.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_Department = AddFirstItem(new SelectList(db.PFA_Department.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_destination_plant = AddFirstItem(new SelectList(db.PFA_Destination_plant.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_reason = AddFirstItem(new SelectList(db.PFA_Reason.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_responsible_cost = AddFirstItem(new SelectList(db.PFA_Responsible_cost.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_type_shipment = AddFirstItem(new SelectList(db.PFA_Type_shipment.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume.Where(x => x.activo == true), "id", "descripcion");
            return View(pFA);
        }

        // GET: PremiumFreightAproval/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.PFA_REGISTRO))
            {
                //obtiene el usuario logeado

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PFA pFA = db.PFA.Find(id);
                if (pFA == null)
                {
                    return HttpNotFound();
                }

                //verifica si se puede editar
                if (pFA.estatus != PFA_Status.CREADO && pFA.estatus != PFA_Status.RECHAZADO)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede modificar esta solicitud!";
                    ViewBag.Descripcion = "No se puede modificar una solicitud que ha sido enviada, aprobada o finalizada.";

                    return View("../Home/ErrorGenerico");
                }

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();
                ViewBag.Solicitante = empleado;

                //obtiene el listado de ids de empleado de los autorizadores
                List<int?> idsAutorizadores = db.PFA_Autorizador.Select(x => x.id_empleado).ToList();

                ViewBag.id_PFA_autorizador = AddFirstItem(new SelectList(db.empleados.Where(x => idsAutorizadores.Contains(x.id)), "id", "ConcatNombre"));
                ViewBag.id_PFA_recovered_cost = AddFirstItem(new SelectList(db.PFA_Recovered_cost.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_border_port = AddFirstItem(new SelectList(db.PFA_Border_port.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_Department = AddFirstItem(new SelectList(db.PFA_Department.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_destination_plant = AddFirstItem(new SelectList(db.PFA_Destination_plant.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_reason = AddFirstItem(new SelectList(db.PFA_Reason.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_responsible_cost = AddFirstItem(new SelectList(db.PFA_Responsible_cost.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_type_shipment = AddFirstItem(new SelectList(db.PFA_Type_shipment.Where(x => x.activo == true), "id", "descripcion"));
                ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume.Where(x => x.activo == true), "id", "descripcion");
                return View(pFA);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: PremiumFreightAproval/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PFA pFA)
        {
            if (ModelState.IsValid)
            {
                biblioteca_digital archivo = new biblioteca_digital { };

                try
                {

                    if (pFA.PostedFile != null) //en caso de haya un archivo seleccionado
                    {
                        if (pFA.PostedFile.InputStream.Length > 5242880)
                        {
                            throw new Exception("Sólo se permiten archivos menores a 5MB");
                        }
                        else //si tiene la longitud correcta
                        {
                            string extension = Path.GetExtension(pFA.PostedFile.FileName);
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
                                )
                            {
                                throw new Exception("Sólo se permiten archivos con extensión .xls, .xlsx, .doc, .docx, .pdf, .png, .jpg, .jpeg, .rar, .zip");
                            }
                            else
                            { //si tiene la extension correcta GUARDA EN BASE DE DATOS

                                String nombreArchivo = pFA.PostedFile.FileName;

                                //recorta el nombre del archivo en caso de ser necesario
                                if (nombreArchivo.Length > 80)
                                {
                                    nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;
                                }

                                //lee el archivo en un arreglo de bytes
                                byte[] fileData = null;
                                using (var binaryReader = new BinaryReader(pFA.PostedFile.InputStream))
                                {
                                    fileData = binaryReader.ReadBytes(pFA.PostedFile.ContentLength);
                                }


                                //si tiene archivo hace un update sino hace un create
                                if (pFA.id_document_support.HasValue)//si tiene valor hace un update
                                {
                                    archivo = db.biblioteca_digital.Find(pFA.id_document_support.Value);
                                    archivo.Nombre = nombreArchivo;
                                    archivo.MimeType = UsoStrings.RecortaString(pFA.PostedFile.ContentType, 80);
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
                                        MimeType = UsoStrings.RecortaString(pFA.PostedFile.ContentType, 80),
                                        Datos = fileData
                                    };

                                    //update en BD
                                    db.biblioteca_digital.Add(archivo);
                                    db.SaveChanges();
                                }

                            }
                        }
                    }
                    //en caso de que no se envie archivo y cost is accepted es false
                    else if (pFA.id_PFA_responsible_cost<=1||( pFA.cost_is_accepted.HasValue && !pFA.cost_is_accepted.Value && pFA.id_document_support.HasValue))
                    {
                        //busca archivo y lo elimina
                        archivo = db.biblioteca_digital.Find(pFA.id_document_support.Value);

                        if (archivo != null)
                        {
                            //coloca como null el idDocumento y guarda para evitar conflictos de sql
                            pFA.id_document_support = null;
                            db.Entry(pFA).State = EntityState.Modified;
                            db.SaveChanges();

                            db.biblioteca_digital.Remove(archivo);
                            db.SaveChanges();

                            archivo = new biblioteca_digital { }; 

                        }

                    }

                    if (archivo.Id > 0) //si existe algún archivo en biblioteca digital
                        pFA.id_document_support = archivo.Id;

                    db.Entry(pFA).State = EntityState.Modified;
                    db.SaveChanges();

                    //agrega el mensaje para sweetalert
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                    if (pFA.estatus == PFA_Status.RECHAZADO)
                        return RedirectToAction("RechazadasCapturista");

                }
                catch (Exception e)
                {

                    ModelState.AddModelError("", e.Message);

                    empleados empleado2 = obtieneEmpleadoLogeado();

                    ViewBag.Solicitante = empleado2;

                    //obtiene el listado de ids de empleado de los autorizadores
                    var idsAutorizadores2 = db.PFA_Autorizador.Select(x => x.id_empleado).ToList();

                    ViewBag.id_PFA_autorizador = AddFirstItem(new SelectList(db.empleados.Where(x => idsAutorizadores2.Contains(x.id)), "id", "ConcatNombre"));
                    ViewBag.id_PFA_recovered_cost = AddFirstItem(new SelectList(db.PFA_Recovered_cost.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_border_port = AddFirstItem(new SelectList(db.PFA_Border_port.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_Department = AddFirstItem(new SelectList(db.PFA_Department.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_destination_plant = AddFirstItem(new SelectList(db.PFA_Destination_plant.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_reason = AddFirstItem(new SelectList(db.PFA_Reason.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_responsible_cost = AddFirstItem(new SelectList(db.PFA_Responsible_cost.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_type_shipment = AddFirstItem(new SelectList(db.PFA_Type_shipment.Where(x => x.activo == true), "id", "descripcion"));
                    ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume.Where(x => x.activo == true), "id", "descripcion");
                    return View(pFA);
                }



                return RedirectToAction("Index");
            }
            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();
            ViewBag.Solicitante = empleado;

            //obtiene el listado de ids de empleado de los autorizadores
            List<int?> idsAutorizadores = db.PFA_Autorizador.Select(x => x.id_empleado).ToList();

            ViewBag.id_PFA_autorizador = AddFirstItem(new SelectList(db.empleados.Where(x => idsAutorizadores.Contains(x.id)), "id", "ConcatNombre"));
            ViewBag.id_PFA_recovered_cost = AddFirstItem(new SelectList(db.PFA_Recovered_cost.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_border_port = AddFirstItem(new SelectList(db.PFA_Border_port.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_Department = AddFirstItem(new SelectList(db.PFA_Department.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_destination_plant = AddFirstItem(new SelectList(db.PFA_Destination_plant.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_reason = AddFirstItem(new SelectList(db.PFA_Reason.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_responsible_cost = AddFirstItem(new SelectList(db.PFA_Responsible_cost.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_type_shipment = AddFirstItem(new SelectList(db.PFA_Type_shipment.Where(x => x.activo == true), "id", "descripcion"));
            ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume.Where(x => x.activo == true), "id", "descripcion");

            return View(pFA);
        }

        // GET: PremiumFreightAproval/Edit/5
        public ActionResult EditCreditNote(int? id)
        {

            if (TieneRol(TipoRoles.PFA_AUTORIZACION) || TieneRol(TipoRoles.PFA_REGISTRO))
            {
                //obtiene el usuario logeado

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PFA pFA = db.PFA.Find(id);
                if (pFA == null)
                {
                    return HttpNotFound();
                }

                //verifica si se puede editar
                if (pFA.estatus != PFA_Status.APROBADO)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede modificar esta solicitud!";
                    ViewBag.Descripcion = "Aún no se puede modificar una solicitud, espere a que sea aprobada.";

                    return View("../Home/ErrorGenerico");
                }

                return View(pFA);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: PremiumFreightAproval/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCreditNote(PFA pFA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pFA).State = EntityState.Modified;
                db.SaveChanges();

                //agrega el mensaje para sweetalert
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha actualizado la solicitud num: " + pFA.id, TipoMensajesSweetAlerts.SUCCESS);

                if (TieneRol(TipoRoles.PFA_REGISTRO))
                    return RedirectToAction("AutorizadasCapturista");

                return RedirectToAction("AutorizadorAutorizadas");
            }
            PFA pFA1 = db.PFA.Find(pFA.id);
            pFA1.credit_debit_note_number = pFA.credit_debit_note_number;

            return View(pFA1);
        }

        // GET: PremiumFreightAproval/AutorizarRechazar/5
        public ActionResult AutorizarRechazar(int? id)
        {
            if (TieneRol(TipoRoles.PFA_AUTORIZACION))
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PFA pFA = db.PFA.Find(id);
                if (pFA == null)
                {
                    return HttpNotFound();
                }
                return View(pFA);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: PremiumFreightAproval/Send/5
        [HttpPost, ActionName("Autorizar")]
        [ValidateAntiForgeryToken]
        public ActionResult AutorizarConfirmed(FormCollection collection)
        {

            int id = 0;
            if (!String.IsNullOrEmpty(collection["id"]))
                Int32.TryParse(collection["id"], out id);


            PFA pfa = db.PFA.Find(id);
            pfa.estatus = PFA_Status.APROBADO;
            pfa.fecha_aprobacion = DateTime.Now;

            db.Entry(pfa).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO
                correos.Add("alfredo.xochitemol@lagermex.com.mx");

                //if (!String.IsNullOrEmpty(pfa.empleados1.correo))
                //    correos.Add(pfa.empleados1.correo);

                envioCorreo.SendEmailAsync(correos, "Su solicitud de PFA ha sido autorizada.", envioCorreo.getBodyPFAAutorizado(pfa));

                //envia correo de notificacion
                correos = new List<string>();
                notificaciones_correo notificaciones_Correo = db.notificaciones_correo.FirstOrDefault(x => x.descripcion == NotificacionesCorreo.PFA_CC_INFO);

                if (notificaciones_Correo != null)
                {
                    empleados emp = db.empleados.Find(notificaciones_Correo.id_empleado);

                    if (emp != null && !String.IsNullOrEmpty(emp.correo))
                    {
                        correos.Add(emp.correo);
                        envioCorreo.SendEmailAsync(correos, "Se ha autorizado una solicitud de PFA", envioCorreo.getBodyPFAAutorizadoInfo(pfa));
                    }
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

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Index");
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido aprobada.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("AutorizadorPendientes");
        }

        // POST: PremiumFreightAproval/Rechazar/5
        [HttpPost, ActionName("Rechazar")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarConfirmed(FormCollection collection)
        {

            int id = 0;
            if (!String.IsNullOrEmpty(collection["id"]))
                Int32.TryParse(collection["id"], out id);

            String razonRechazo = collection["razon_rechazo"];


            PFA pfa = db.PFA.Find(id);
            pfa.estatus = PFA_Status.RECHAZADO;
            pfa.razon_rechazo = razonRechazo;

            db.Entry(pfa).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO
                correos.Add("alfredo.xochitemol@lagermex.com.mx");

                //if (!String.IsNullOrEmpty(pfa.empleados1.correo))
                //    correos.Add(pfa.empleados1.correo);

                envioCorreo.SendEmailAsync(correos, "Su solicitud de PFA ha sido Rechazada", envioCorreo.getBodyPFARechazado(pfa));

                //envia correo de notificacion
                correos = new List<string>();
                notificaciones_correo notificaciones_Correo = db.notificaciones_correo.FirstOrDefault(x => x.descripcion == NotificacionesCorreo.PFA_CC_INFO);

                if (notificaciones_Correo != null)
                {
                    empleados emp = db.empleados.Find(notificaciones_Correo.id_empleado);

                    if (emp != null && !String.IsNullOrEmpty(emp.correo))
                    {
                        correos.Add(emp.correo);
                        envioCorreo.SendEmailAsync(correos, "Se ha rechazado una solicitud de PFA", envioCorreo.getBodyPFARechazadoInfo(pfa));
                    }
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

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Index");
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido rechazada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("AutorizadorPendientes");
        }

        // GET: PremiumFreightAproval/Send/5
        public ActionResult Send(int? id)
        {
            if (TieneRol(TipoRoles.PFA_REGISTRO))
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PFA pFA = db.PFA.Find(id);
                if (pFA == null)
                {
                    return HttpNotFound();
                }
                return View(pFA);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: PremiumFreightAproval/Send/5
        [HttpPost, ActionName("Send")]
        [ValidateAntiForgeryToken]
        public ActionResult SendConfirmed(int id)
        {
            PFA pfa = db.PFA.Find(id);
            string estatusAnterior = pfa.estatus;
            pfa.estatus = PFA_Status.ENVIADO;

            db.Entry(pfa).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO
                correos.Add("alfredo.xochitemol@lagermex.com.mx");

                //if (!String.IsNullOrEmpty(pfa.empleados.correo))
                //    correos.Add(pfa.empleados.correo); //agrega correo de autorizador

                envioCorreo.SendEmailAsync(correos, "Ha recibido una solicitud PFA, para su aprobación.", envioCorreo.getBodyPFAEnviado(pfa));
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

            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido enviada", TipoMensajesSweetAlerts.SUCCESS);

            if (estatusAnterior == PFA_Status.RECHAZADO)
                return RedirectToAction("RechazadasCapturista");

            return RedirectToAction("Index");
        }

        public ActionResult Exportar(int? id_PFA_user, int? id_PFA_reason, int? id_PFA_responsible_cost, string fecha_inicial, string fecha_final, string status)
        {
            if (TieneRol(TipoRoles.PFA_VISUALIZACION))
            {
                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
                DateTime dateFinal = DateTime.Now;          //fecha final por defecto

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



                var listado = db.PFA.Include(p => p.empleados).Include(p => p.empleados1).Include(p => p.PFA_Recovered_cost).Include(p => p.PFA_Border_port).Include(p => p.PFA_Department).Include(p => p.PFA_Destination_plant).Include(p => p.PFA_Reason).Include(p => p.PFA_Responsible_cost).Include(p => p.PFA_Type_shipment).Include(p => p.PFA_Volume)
                  .Where(x =>
                  (id_PFA_user == null || x.id_solicitante == id_PFA_user)
                  && (id_PFA_reason == null || x.id_PFA_reason == id_PFA_reason)
                  && (id_PFA_responsible_cost == null || x.id_PFA_responsible_cost == id_PFA_responsible_cost)
                  && x.date_request >= dateInicial && x.date_request <= dateFinal
                   && (String.IsNullOrEmpty(status) || x.estatus == status)
                  )
                  .OrderBy(x => x.id)
                  .ToList();

                byte[] stream = ExcelUtil.GeneraReportePFAExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Reporte_PFA_" + fecha_inicial + "_" + dateFinal.ToString("yyyy-MM-dd") + ".xlsx",

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
