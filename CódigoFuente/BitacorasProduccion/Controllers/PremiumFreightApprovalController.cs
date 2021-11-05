using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
                    .OrderByDescending(x=> x.date_request);
                
                //configura los parametros de la vista
                ViewBag.Title = "Listado Solicitudes Creadas";
                ViewBag.SegundoNivel = "PFA_registro";
                ViewBag.Edit = true;
                ViewBag.Details = true;
                ViewBag.SendToAuthorizer = true;
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
                ViewBag.Details = true;
                ViewBag.SendToAuthorizer = false;
                ViewBag.AuthorizarRechazar = false;
                return View("ListadoSolicitudes",pFA.ToList());
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


        // GET: PremiumFreightAproval/Details/5
        public ActionResult Details(int? id)
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

        // GET: PremiumFreightAproval/Create
        public ActionResult Create()
        {

            if (TieneRol(TipoRoles.PFA_REGISTRO))
            {
                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();
                ViewBag.Solicitante = empleado;

                //obtiene el listado de ids de empleado de los autorizadores
                List<int?> idsAutorizadores = db.PFA_Autorizador.Select(x=> x.id_empleado).ToList();

                ViewBag.id_PFA_autorizador = AddFirstItem(new SelectList(db.empleados.Where(x => idsAutorizadores.Contains(x.id)), "id", "ConcatNombre"));
                ViewBag.id_PFA_recovered_cost = AddFirstItem(new SelectList(db.PFA_Recovered_cost, "id", "descripcion"));
                ViewBag.id_PFA_border_port = AddFirstItem(new SelectList(db.PFA_Border_port, "id", "descripcion"));
                ViewBag.id_PFA_Department = AddFirstItem(new SelectList(db.PFA_Department, "id", "descripcion")); 
                ViewBag.id_PFA_destination_plant = AddFirstItem(new SelectList(db.PFA_Destination_plant, "id", "descripcion"));
                ViewBag.id_PFA_reason = AddFirstItem(new SelectList(db.PFA_Reason, "id", "descripcion"));
                ViewBag.id_PFA_responsible_cost = AddFirstItem(new SelectList(db.PFA_Responsible_cost, "id", "descripcion"));
                ViewBag.id_PFA_type_shipment = AddFirstItem(new SelectList(db.PFA_Type_shipment, "id", "descripcion"));
                ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume, "id", "descripcion");
              
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
                pFA.estatus = PFA_Status.CREADO;
                pFA.date_request = DateTime.Now;
                pFA.activo = true;

                db.PFA.Add(pFA);
                db.SaveChanges();

                //agrega el mensaje para sweetalert
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            ViewBag.Solicitante = empleado;

            //obtiene el listado de ids de empleado de los autorizadores
            List<int?> idsAutorizadores = db.PFA_Autorizador.Select(x => x.id_empleado).ToList();

            ViewBag.id_PFA_autorizador = AddFirstItem(new SelectList(db.empleados.Where(x => idsAutorizadores.Contains(x.id)), "id", "ConcatNombre"));
            ViewBag.id_PFA_recovered_cost = AddFirstItem(new SelectList(db.PFA_Recovered_cost, "id", "descripcion"));
            ViewBag.id_PFA_border_port = AddFirstItem(new SelectList(db.PFA_Border_port, "id", "descripcion"));
            ViewBag.id_PFA_Department = AddFirstItem(new SelectList(db.PFA_Department, "id", "descripcion"));
            ViewBag.id_PFA_destination_plant = AddFirstItem(new SelectList(db.PFA_Destination_plant, "id", "descripcion"));
            ViewBag.id_PFA_reason = AddFirstItem(new SelectList(db.PFA_Reason, "id", "descripcion"));
            ViewBag.id_PFA_responsible_cost = AddFirstItem(new SelectList(db.PFA_Responsible_cost, "id", "descripcion"));
            ViewBag.id_PFA_type_shipment = AddFirstItem(new SelectList(db.PFA_Type_shipment, "id", "descripcion"));
            ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume, "id", "descripcion");
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
                if (pFA.estatus != PFA_Status.CREADO && pFA.estatus != PFA_Status.RECHAZADO) {
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
                ViewBag.id_PFA_recovered_cost = AddFirstItem(new SelectList(db.PFA_Recovered_cost, "id", "descripcion"));
                ViewBag.id_PFA_border_port = AddFirstItem(new SelectList(db.PFA_Border_port, "id", "descripcion"));
                ViewBag.id_PFA_Department = AddFirstItem(new SelectList(db.PFA_Department, "id", "descripcion"));
                ViewBag.id_PFA_destination_plant = AddFirstItem(new SelectList(db.PFA_Destination_plant, "id", "descripcion"));
                ViewBag.id_PFA_reason = AddFirstItem(new SelectList(db.PFA_Reason, "id", "descripcion"));
                ViewBag.id_PFA_responsible_cost = AddFirstItem(new SelectList(db.PFA_Responsible_cost, "id", "descripcion"));
                ViewBag.id_PFA_type_shipment = AddFirstItem(new SelectList(db.PFA_Type_shipment, "id", "descripcion"));
                ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume, "id", "descripcion");
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
                db.Entry(pFA).State = EntityState.Modified;
                db.SaveChanges();

                //agrega el mensaje para sweetalert
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();
            ViewBag.Solicitante = empleado;

            //obtiene el listado de ids de empleado de los autorizadores
            List<int?> idsAutorizadores = db.PFA_Autorizador.Select(x => x.id_empleado).ToList();

            ViewBag.id_PFA_autorizador = AddFirstItem(new SelectList(db.empleados.Where(x => idsAutorizadores.Contains(x.id)), "id", "ConcatNombre"));
            ViewBag.id_PFA_recovered_cost = AddFirstItem(new SelectList(db.PFA_Recovered_cost, "id", "descripcion"));
            ViewBag.id_PFA_border_port = AddFirstItem(new SelectList(db.PFA_Border_port, "id", "descripcion"));
            ViewBag.id_PFA_Department = AddFirstItem(new SelectList(db.PFA_Department, "id", "descripcion"));
            ViewBag.id_PFA_destination_plant = AddFirstItem(new SelectList(db.PFA_Destination_plant, "id", "descripcion"));
            ViewBag.id_PFA_reason = AddFirstItem(new SelectList(db.PFA_Reason, "id", "descripcion"));
            ViewBag.id_PFA_responsible_cost = AddFirstItem(new SelectList(db.PFA_Responsible_cost, "id", "descripcion"));
            ViewBag.id_PFA_type_shipment = AddFirstItem(new SelectList(db.PFA_Type_shipment, "id", "descripcion"));
            ViewBag.id_PFA_volume = new SelectList(db.PFA_Volume, "id", "descripcion");

            return View(pFA);
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
            pfa.estatus = PFA_Status.ENVIADO;

            db.Entry(pfa).State = EntityState.Modified;
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
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido enviada", TipoMensajesSweetAlerts.SUCCESS);
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
