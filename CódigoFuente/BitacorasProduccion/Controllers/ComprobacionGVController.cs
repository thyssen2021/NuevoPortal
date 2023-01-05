using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class ComprobacionGVController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();


        // GET: ComprobacionGV    
        public ActionResult Solicitudes(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            //en caso de solicitudes pendientes manda a otra vista
            if (estatus == "PENDIENTES")
                return RedirectToAction("Pendientes", new { estatus = estatus });


            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();

            var listado = db.GV_comprobacion
                .Where(x => (x.GV_solicitud.id_empleado == empleado.id || x.GV_solicitud.id_solicitante == empleado.id)
                    && (
                    x.estatus == estatus ||
                    (estatus == GV_comprobacion_estatus.CREADO && (x.estatus == GV_comprobacion_estatus.CREADO
                                                    ))
                                    || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                    || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                    || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                    || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                    ))
                                    || (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_A_JEFE
                                                    || x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTROLLING
                                                    || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD))
                                                    )
                )
                .OrderByDescending(x => x.GV_solicitud.fecha_solicitud)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_comprobacion
                   .Where(x => (x.GV_solicitud.id_empleado == empleado.id || x.GV_solicitud.id_solicitante == empleado.id)
                    && (
                    x.estatus == estatus ||
                    (estatus == GV_comprobacion_estatus.CREADO && (x.estatus == GV_comprobacion_estatus.CREADO
                                                    ))
                                    || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                    || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                    || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                    || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                    ))
                                    || (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_A_JEFE
                                                    || x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTROLLING
                                                    || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD))
                                                    )
                )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual


            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = "Pendientes", Value = "PENDIENTES" });
            newList.Add(new SelectListItem() { Text = "Por enviar", Value = GV_comprobacion_estatus.CREADO });
            newList.Add(new SelectListItem() { Text = "En proceso", Value = "EN_PROCESO" });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = "RECHAZADAS" });
            newList.Add(new SelectListItem() { Text = "Finalizadas", Value = GV_comprobacion_estatus.FINALIZADO });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;
            ViewBag.SegundoNivel = "GV_solicitud";
            ViewBag.Title = "Mis Solicitudes";


            //ordenar por fecha de solicitud más reciente
            return View("solicitudes", listado);

        }

        public ActionResult Pendientes(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            //en caso de solicitudes no pendientes manda a otra vista
            if (estatus != "PENDIENTES")
                return RedirectToAction("Solicitudes", new { estatus = estatus });

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();

            var listado = db.GV_solicitud
                .Where(x => (x.id_empleado == empleado.id || x.id_solicitante == empleado.id)
                    && x.estatus == GV_solicitud_estatus.FINALIZADO && x.GV_comprobacion == null
                )
                .OrderByDescending(x => x.fecha_solicitud)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_solicitud
                   .Where(x => (x.id_empleado == empleado.id || x.id_solicitante == empleado.id)
                    && x.estatus == GV_solicitud_estatus.FINALIZADO && x.GV_comprobacion == null
                )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual


            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = "Pendientes", Value = "PENDIENTES" });
            newList.Add(new SelectListItem() { Text = "Por enviar", Value = GV_comprobacion_estatus.CREADO });
            newList.Add(new SelectListItem() { Text = "En proceso", Value = "EN_PROCESO" });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = "RECHAZADAS" });
            newList.Add(new SelectListItem() { Text = "Finalizadas", Value = GV_comprobacion_estatus.FINALIZADO });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;

            //ordenar por fecha de solicitud más reciente
            return View("pendientes", listado);

        }

        // GET: GV_solicitud/Details/5
        public ActionResult Details(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD)) //Todos los usuarios van a tener permiso para crear solicitudes de GV, Por lo que solo se necesita validar este permiso
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }


            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            //ordena los rel de la comprobacion
            gV_solicitud.GV_comprobacion_rel_gastos = gV_solicitud.GV_comprobacion_rel_gastos.OrderByDescending(x => x.uuid).ThenBy(x => x.orden).ToList();

            ViewBag.ListaCuentas = db.GV_comprobacion_tipo_gastos_viaje.Where(x => x.activo).ToList();
            ViewBag.ListaTipoPago = db.GV_comprobacion_tipo_pago.Where(x => x.activo).ToList();
            ViewBag.ListaCentroCosto = db.GV_centros_costo.Where(x => x.activo).OrderBy(x => x.plantas.clave).ToList();
            return View(gV_solicitud);
        }
        // GET: GV_solicitud/EnviarAJefeDirecto/5
        public ActionResult EnviarAJefeDirecto(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD)) //Todos los usuarios van a tener permiso para crear solicitudes de GV, Por lo que solo se necesita validar este permiso
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }


            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            //ordena los rel de la comprobacion
            gV_solicitud.GV_comprobacion_rel_gastos = gV_solicitud.GV_comprobacion_rel_gastos.OrderByDescending(x => x.uuid).ThenBy(x => x.orden).ToList();

            ViewBag.ListaCuentas = db.GV_comprobacion_tipo_gastos_viaje.Where(x => x.activo).ToList();
            ViewBag.ListaTipoPago = db.GV_comprobacion_tipo_pago.Where(x => x.activo).ToList();
            ViewBag.ListaCentroCosto = db.GV_centros_costo.Where(x => x.activo).OrderBy(x => x.plantas.clave).ToList();
            return View(gV_solicitud);
        }

        // POST: PremiumFreightAproval/Send/5
        [HttpPost, ActionName("EnviarAJefeDirecto")]
        [ValidateAntiForgeryToken]
        public ActionResult EnviarAJefeDirectoConfirmed(int id)
        {
            //obtiene una referencia al objeto de la base de datos
            GV_comprobacion solicitudBD = db.GV_comprobacion.Find(id);
            string estatusAnterior = solicitudBD.estatus;
            // Activity already exist in database and modify it
            solicitudBD.estatus = Bitacoras.Util.GV_comprobacion_estatus.ENVIADO_A_JEFE;
            //asigna el mismo jefe directo de la solicitud original
            solicitudBD.id_jefe_directo = solicitudBD.GV_solicitud.id_jefe_directo;

            //db.Entry(solicitudBD).CurrentValues.SetValues(solicitud.GV_comprobacion);
            db.Entry(solicitudBD).State = EntityState.Modified;

            try
            {
                db.SaveChanges();


                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO             

                //obtiene el correo electrónico del jefe directo
                if (!String.IsNullOrEmpty(solicitudBD.empleados2.correo))
                    correos.Add(solicitudBD.empleados2.correo); //agrega correo de validador

                envioCorreo.SendEmailAsync(correos, "Ha recibido una Comprobación de Gastos de Viaje para su aprobación.", envioCorreo.getBodyGVComprobacionNotificacionEnvioJefe(solicitudBD));
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
                if (estatusAnterior == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD || estatusAnterior == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                    || estatusAnterior == GV_comprobacion_estatus.RECHAZADO_JEFE || estatusAnterior == GV_comprobacion_estatus.RECHAZADO_NOMINA)
                    return RedirectToAction("Solicitudes", new { estatus = "RECHAZADAS" });
                return RedirectToAction("Solicitudes", new { estatus = "CREADO" });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                if (estatusAnterior == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD || estatusAnterior == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                  || estatusAnterior == GV_comprobacion_estatus.RECHAZADO_JEFE || estatusAnterior == GV_comprobacion_estatus.RECHAZADO_NOMINA)
                    return RedirectToAction("Solicitudes", new { estatus = "RECHAZADAS" });
                return RedirectToAction("Solicitudes", new { estatus = "CREADO" });
            }

            TempData["Mensaje"] = new MensajesSweetAlert("La comprobación de gastos ha sido enviada.", TipoMensajesSweetAlerts.SUCCESS);

            if (estatusAnterior == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD || estatusAnterior == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                   || estatusAnterior == GV_comprobacion_estatus.RECHAZADO_JEFE || estatusAnterior == GV_comprobacion_estatus.RECHAZADO_NOMINA)
                return RedirectToAction("Solicitudes", new { estatus = "RECHAZADAS" });
            return RedirectToAction("Solicitudes", new { estatus = "CREADO" });
        }

        // GET: SolicitudesJefeDirecto
        public ActionResult SolicitudesJefeDirecto(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_JEFE_DIRECTO))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();
            var listado = db.GV_comprobacion
                           .Where(x => (x.GV_solicitud.id_jefe_directo == empleado.id)
                               && (
                                x.estatus == estatus ||
                               (estatus == "PENDIENTES" && (x.estatus == GV_comprobacion_estatus.ENVIADO_A_JEFE
                                                               ))
                                               || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                               ))
                                               || (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD))
                                                               )
                           )
                           .OrderByDescending(x => x.GV_solicitud.fecha_solicitud)
                           .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                          .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_comprobacion
                     .Where(x => (x.GV_solicitud.id_jefe_directo == empleado.id)
                               && (
                                x.estatus == estatus ||
                               (estatus == "PENDIENTES" && (x.estatus == GV_comprobacion_estatus.ENVIADO_A_JEFE
                                                               ))
                                               || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                               ))
                                               || (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD))
                                                               )
                           )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = "Pendientes", Value = "PENDIENTES" });
            //newList.Add(new SelectListItem() { Text = "Por enviar", Value = GV_comprobacion_estatus.CREADO });
            newList.Add(new SelectListItem() { Text = "En proceso", Value = "EN_PROCESO" });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = "RECHAZADAS" });
            newList.Add(new SelectListItem() { Text = "Finalizadas", Value = GV_comprobacion_estatus.FINALIZADO });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;
            ViewBag.SegundoNivel = "solicitudes_jefe_directo";
            ViewBag.Title = "Solicitudes (Jefe Dierecto)";

            //ordenar por fecha de solicitud más reciente
            return View("solicitudes", listado);

        }

        // GET: GV_solicitud/AutorizarJefeDirecto/5
        public ActionResult AutorizarJefeDirecto(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_JEFE_DIRECTO)) //Todos los usuarios van a tener permiso para crear solicitudes de GV, Por lo que solo se necesita validar este permiso
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //verifica si se puede autorizar
            if (gV_solicitud.GV_comprobacion.estatus != GV_comprobacion_estatus.ENVIADO_A_JEFE)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede validar esta solicitud!";
                ViewBag.Descripcion = "No se puede autorizar una solicitud que ya ha sido validada, rechazada o finalizada.";

                return View("../Home/ErrorGenerico");
            }

            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            //ordena los rel de la comprobacion
            gV_solicitud.GV_comprobacion_rel_gastos = gV_solicitud.GV_comprobacion_rel_gastos.OrderByDescending(x => x.uuid).ThenBy(x => x.orden).ToList();

            ViewBag.ListaCuentas = db.GV_comprobacion_tipo_gastos_viaje.Where(x => x.activo).ToList();
            ViewBag.ListaTipoPago = db.GV_comprobacion_tipo_pago.Where(x => x.activo).ToList();
            ViewBag.ListaCentroCosto = db.GV_centros_costo.Where(x => x.activo).OrderBy(x => x.plantas.clave).ToList();
            //obtiene la lista de usuarios asociados a controlling
            var listUsuarios = db.GV_usuarios.Where(x => x.departamento == Bitacoras.Util.GV_tipo_departamento.CONTROLLING).Select(x => x.id_empleado).ToList();
            ViewBag.id_controlling = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value && listUsuarios.Contains(x.id)), "id", "ConcatNumEmpleadoNombre")
                , selected: "168"); //id de ismael

            return View(gV_solicitud);
        }

        // POST: PremiumFreightAproval/Send/5
        [HttpPost, ActionName("AutorizarJefeDirecto")]
        [ValidateAntiForgeryToken]
        public ActionResult AutorizarJefeDirectoConfirmed(int? id, int? id_controlling, string s_estatus)
        {

            GV_comprobacion solicitud = db.GV_comprobacion.Find(id);
            if (solicitud == null)
            {
                return View("../Error/NotFound");
            }

            ////necesario para poder guardar los cambios
            //solicitud.medio_transporte_aplica_otro = !solicitud.id_medio_transporte.HasValue;

            solicitud.estatus = GV_comprobacion_estatus.ENVIADO_CONTROLLING;
            solicitud.id_controlling = id_controlling;

            //establece las fechas 
            solicitud.fecha_aceptacion_jefe_area = DateTime.Now;
            solicitud.fecha_aceptacion_controlling = null;
            solicitud.fecha_aceptacion_contabilidad = null;
            solicitud.fecha_aceptacion_nomina = null;

            db.Entry(solicitud).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                empleados controlling = db.empleados.Find(id_controlling);

                if (controlling != null && !String.IsNullOrEmpty(controlling.correo))
                    correos.Add(controlling.correo);
                envioCorreo.SendEmailAsync(correos, "Ha recibido una Comprobación Gastos de Viaje.", envioCorreo.getBodyGVComprobacionNotificacionEnvioControlling(solicitud));

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
                return RedirectToAction("SolicitudesJefeDirecto", new { estatus = s_estatus });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("SolicitudesJefeDirecto", new { estatus = s_estatus });
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido autorizada.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("SolicitudesJefeDirecto", new { estatus = s_estatus });

        }


        // POST: GV_solicitud/AutorizarJefeDirecto/5
        [HttpPost, ActionName("RechazarJefeDirecto")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarJefeDirectoConfirmed(int? id, string s_estatus, [Bind(Prefix = "GV_comprobacion.comentario_rechazo")] string comentario_rechazo)
        {

            GV_comprobacion solicitud = db.GV_comprobacion.Find(id);
            solicitud.estatus = GV_comprobacion_estatus.RECHAZADO_JEFE;
            solicitud.comentario_rechazo = comentario_rechazo;
            //borra las fechas 
            solicitud.fecha_aceptacion_jefe_area = null;
            solicitud.fecha_aceptacion_controlling = null;
            solicitud.fecha_aceptacion_nomina = null;
            solicitud.fecha_aceptacion_contabilidad = null;

            //necesario para poder guardar los cambios
            //solicitud.medio_transporte_aplica_otro = !solicitud.id_medio_transporte.HasValue;
            //db.Entry(solicitud).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                if (!String.IsNullOrEmpty(solicitud.GV_solicitud.empleados5.correo))
                    correos.Add(solicitud.GV_solicitud.empleados5.correo); //agrega correo de solicitantes

                envioCorreo.SendEmailAsync(correos, "Se ha rechazado su Comprobación de Gastos de Viaje.", envioCorreo.getBodyGVComprobacionNotificacionRechazo(solicitud, solicitud.empleados2.ConcatNombre));

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
                return RedirectToAction("SolicitudesJefeDirecto", new { estatus = s_estatus });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("SolicitudesJefeDirecto", new { estatus = s_estatus });
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido rechazada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("SolicitudesJefeDirecto", new { estatus = s_estatus });
        }

        // GET: SolicitudesControlling
        public ActionResult SolicitudesControlling(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_CONTROLLING))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();
            var listado = db.GV_comprobacion
                           .Where(x => (x.GV_solicitud.id_controlling == empleado.id)
                               && (
                               x.estatus == estatus ||
                               (estatus == "PENDIENTES" && (x.estatus == GV_comprobacion_estatus.ENVIADO_CONTROLLING
                                                               ))
                                               || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                               ))
                                               || (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD))
                                                               )
                           )
                           .OrderByDescending(x => x.GV_solicitud.fecha_solicitud)
                           .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                          .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_comprobacion
                    .Where(x => (x.GV_solicitud.id_controlling == empleado.id)
                               && (
                               x.estatus == estatus ||
                               (estatus == "PENDIENTES" && (x.estatus == GV_comprobacion_estatus.ENVIADO_CONTROLLING
                                                               ))
                                               || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                               ))
                                               || (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD))
                                                               )
                           )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = "Pendientes", Value = "PENDIENTES" });
            //newList.Add(new SelectListItem() { Text = "Por enviar", Value = GV_comprobacion_estatus.CREADO });
            newList.Add(new SelectListItem() { Text = "En proceso", Value = "EN_PROCESO" });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = "RECHAZADAS" });
            newList.Add(new SelectListItem() { Text = "Finalizadas", Value = GV_comprobacion_estatus.FINALIZADO });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;
            ViewBag.SegundoNivel = "solicitudes_controlling";
            ViewBag.Title = "Solicitudes (Controlling)";

            //ordenar por fecha de solicitud más reciente
            return View("solicitudes", listado);

        }

        // GET: GV_solicitud/AutorizarControlling/5
        public ActionResult AutorizarControlling(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_CONTROLLING)) //Todos los usuarios van a tener permiso para crear solicitudes de GV, Por lo que solo se necesita validar este permiso
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //verifica si se puede autorizar
            if (gV_solicitud.GV_comprobacion.estatus != GV_comprobacion_estatus.ENVIADO_CONTROLLING)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede validar esta solicitud!";
                ViewBag.Descripcion = "No se puede autorizar una solicitud que ya ha sido validada, rechazada o finalizada.";

                return View("../Home/ErrorGenerico");
            }

            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            //ordena los rel de la comprobacion
            gV_solicitud.GV_comprobacion_rel_gastos = gV_solicitud.GV_comprobacion_rel_gastos.OrderByDescending(x => x.uuid).ThenBy(x => x.orden).ToList();

            ViewBag.ListaCuentas = db.GV_comprobacion_tipo_gastos_viaje.Where(x => x.activo).ToList();
            ViewBag.ListaTipoPago = db.GV_comprobacion_tipo_pago.Where(x => x.activo).ToList();
            ViewBag.ListaCentroCosto = db.GV_centros_costo.Where(x => x.activo).OrderBy(x => x.plantas.clave).ToList();

            //obtiene la lista de usuarios asociados a Cuentas x pagar (contabilidad)
            var listUsuarios = db.GV_usuarios.Where(x => x.departamento == Bitacoras.Util.GV_tipo_departamento.CUENTASPORPAGAR).Select(x => x.id_empleado).ToList();
            ViewBag.id_contabilidad = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value && listUsuarios.Contains(x.id)), "id", "ConcatNumEmpleadoNombre"));


            return View(gV_solicitud);
        }

        // POST: PremiumFreightAproval/Send/5
        [HttpPost, ActionName("AutorizarControlling")]
        [ValidateAntiForgeryToken]
        public ActionResult AutorizarControllingConfirmed(int? id, int? id_contabilidad, string s_estatus)
        {

            GV_comprobacion solicitud = db.GV_comprobacion.Find(id);
            if (solicitud == null)
            {
                return View("../Error/NotFound");
            }

            ////necesario para poder guardar los cambios
            //solicitud.medio_transporte_aplica_otro = !solicitud.id_medio_transporte.HasValue;

            solicitud.estatus = GV_comprobacion_estatus.ENVIADO_CONTABILIDAD;
            solicitud.id_contabilidad = id_contabilidad;

            //establece las fechas 
            // solicitud.fecha_aceptacion_jefe_area = DateTime.Now;
            solicitud.fecha_aceptacion_controlling = DateTime.Now;
            solicitud.fecha_aceptacion_contabilidad = null;
            solicitud.fecha_aceptacion_nomina = null;

            db.Entry(solicitud).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                empleados contabilidad = db.empleados.Find(id_contabilidad);

                if (contabilidad != null && !String.IsNullOrEmpty(contabilidad.correo))
                    correos.Add(contabilidad.correo);
                envioCorreo.SendEmailAsync(correos, "Ha recibido una Comprobación Gastos de Viaje.", envioCorreo.getBodyGVComprobacionNotificacionEnvioContabilidad(solicitud));

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
                return RedirectToAction("SolicitudesControlling", new { estatus = s_estatus });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("SolicitudesControlling", new { estatus = s_estatus });
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido autorizada.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("SolicitudesControlling", new { estatus = s_estatus });

        }


        // POST: GV_solicitud/RechazarControlling/5
        [HttpPost, ActionName("RechazarControlling")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarControllingConfirmed(int? id, string s_estatus, [Bind(Prefix = "GV_comprobacion.comentario_rechazo")] string comentario_rechazo)
        {

            GV_comprobacion solicitud = db.GV_comprobacion.Find(id);
            solicitud.estatus = GV_comprobacion_estatus.RECHAZADO_CONTROLLING;
            solicitud.comentario_rechazo = comentario_rechazo;
            //borra las fechas 
            solicitud.fecha_aceptacion_jefe_area = null;
            solicitud.fecha_aceptacion_controlling = null;
            solicitud.fecha_aceptacion_nomina = null;
            solicitud.fecha_aceptacion_contabilidad = null;

            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                if (!String.IsNullOrEmpty(solicitud.GV_solicitud.empleados5.correo))
                    correos.Add(solicitud.GV_solicitud.empleados5.correo); //agrega correo de solicitantes

                envioCorreo.SendEmailAsync(correos, "Se ha rechazado su Comprobación de Gastos de Viaje.", envioCorreo.getBodyGVComprobacionNotificacionRechazo(solicitud, solicitud.empleados1.ConcatNombre));

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
                return RedirectToAction("SolicitudesControlling", new { estatus = s_estatus });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("SolicitudesControlling", new { estatus = s_estatus });
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido rechazada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("SolicitudesControlling", new { estatus = s_estatus });
        }



        // GET: SolicitudesContabilidad
        public ActionResult SolicitudesContabilidad(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_CONTABILIDAD))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();
            var listado = db.GV_comprobacion
                             .Where(x => (x.id_contabilidad == empleado.id)
                               && (
                               x.estatus == estatus ||
                               (estatus == "PENDIENTES" && (x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD
                                                               ))
                                               || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                               ))
                                               || (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD))
                                                               )
                           )
                           .OrderByDescending(x => x.GV_solicitud.fecha_solicitud)
                           .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                          .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_comprobacion
                   .Where(x => (x.id_contabilidad == empleado.id)
                               && (
                               x.estatus == estatus ||
                               (estatus == "PENDIENTES" && (x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD
                                                               ))
                                               || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                               ))
                                               || (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.ENVIADO_CONTABILIDAD))
                                                               )
                           )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = "Pendientes", Value = "PENDIENTES" });
            //newList.Add(new SelectListItem() { Text = "Por enviar", Value = GV_comprobacion_estatus.CREADO });
            newList.Add(new SelectListItem() { Text = "En proceso", Value = "EN_PROCESO" });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = "RECHAZADAS" });
            newList.Add(new SelectListItem() { Text = "Finalizadas", Value = GV_comprobacion_estatus.FINALIZADO });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;
            ViewBag.SegundoNivel = "solicitudes_contabilidad";
            ViewBag.Title = "Solicitudes (Cuentas por Pagar)";

            //ordenar por fecha de solicitud más reciente
            return View("solicitudes", listado);

        }

        // GET: GV_solicitud/AutorizarContabilidad/5
        public ActionResult AutorizarContabilidad(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_CONTABILIDAD)) //Todos los usuarios van a tener permiso para crear solicitudes de GV, Por lo que solo se necesita validar este permiso
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //verifica si se puede autorizar
            if (gV_solicitud.GV_comprobacion.estatus != GV_comprobacion_estatus.ENVIADO_CONTABILIDAD)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede validar esta solicitud!";
                ViewBag.Descripcion = "No se puede autorizar una solicitud que ya ha sido validada, rechazada o finalizada.";

                return View("../Home/ErrorGenerico");
            }

            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            //ordena los rel de la comprobacion
            gV_solicitud.GV_comprobacion_rel_gastos = gV_solicitud.GV_comprobacion_rel_gastos.OrderByDescending(x => x.uuid).ThenBy(x => x.orden).ToList();

            ViewBag.ListaCuentas = db.GV_comprobacion_tipo_gastos_viaje.Where(x => x.activo).ToList();
            ViewBag.ListaTipoPago = db.GV_comprobacion_tipo_pago.Where(x => x.activo).ToList();
            ViewBag.ListaCentroCosto = db.GV_centros_costo.Where(x => x.activo).OrderBy(x => x.plantas.clave).ToList();

            //obtiene la lista de usuarios asociados a Cuentas x pagar (contabilidad)
            var listUsuarios = db.GV_usuarios.Where(x => x.departamento == Bitacoras.Util.GV_tipo_departamento.NOMINA).Select(x => x.id_empleado).ToList();
            ViewBag.id_nomina = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value && listUsuarios.Contains(x.id)), "id", "ConcatNumEmpleadoNombre"));

            return View(gV_solicitud);
        }

        // POST: PremiumFreightAproval/Send/5
        [HttpPost, ActionName("AutorizarContabilidad")]
        [ValidateAntiForgeryToken]
        public ActionResult AutorizarContabilidadConfirmed(int? id, int? id_nomina, string s_estatus)
        {

            GV_comprobacion solicitud = db.GV_comprobacion.Find(id);
            if (solicitud == null)
            {
                return View("../Error/NotFound");
            }

            solicitud.estatus = GV_comprobacion_estatus.ENVIADO_NOMINA;
            solicitud.id_nomina = id_nomina;

            //establece las fechas 
            // solicitud.fecha_aceptacion_jefe_area = DateTime.Now;
            //solicitud.fecha_aceptacion_controlling = DateTime.Now;
            solicitud.fecha_aceptacion_contabilidad = DateTime.Now;
            solicitud.fecha_aceptacion_nomina = null;

            db.Entry(solicitud).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                empleados nominas = db.empleados.Find(id_nomina);

                if (nominas != null && !String.IsNullOrEmpty(nominas.correo))
                    correos.Add(nominas.correo);
                envioCorreo.SendEmailAsync(correos, "Ha recibido una Comprobación Gastos de Viaje.", envioCorreo.getBodyGVComprobacionNotificacionEnvioNomina(solicitud));

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
                return RedirectToAction("SolicitudesContabilidad", new { estatus = s_estatus });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("SolicitudesContabilidad", new { estatus = s_estatus });
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido autorizada.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("SolicitudesContabilidad", new { estatus = s_estatus });

        }


        // POST: GV_solicitud/RechazarContabilidad/5
        [HttpPost, ActionName("RechazarContabilidad")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarContabilidadConfirmed(int? id, string s_estatus, [Bind(Prefix = "GV_comprobacion.comentario_rechazo")] string comentario_rechazo)
        {

            GV_comprobacion solicitud = db.GV_comprobacion.Find(id);
            solicitud.estatus = GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD;
            solicitud.comentario_rechazo = comentario_rechazo;
            //borra las fechas 
            solicitud.fecha_aceptacion_jefe_area = null;
            solicitud.fecha_aceptacion_controlling = null;
            solicitud.fecha_aceptacion_nomina = null;
            solicitud.fecha_aceptacion_contabilidad = null;

            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                if (!String.IsNullOrEmpty(solicitud.GV_solicitud.empleados5.correo))
                    correos.Add(solicitud.GV_solicitud.empleados5.correo); //agrega correo de solicitantes

                envioCorreo.SendEmailAsync(correos, "Se ha rechazado su Comprobación de Gastos de Viaje.", envioCorreo.getBodyGVComprobacionNotificacionRechazo(solicitud, solicitud.empleados.ConcatNombre));

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
                return RedirectToAction("SolicitudesContabilidad", new { estatus = s_estatus });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("SolicitudesContabilidad", new { estatus = s_estatus });
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido rechazada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("SolicitudesContabilidad", new { estatus = s_estatus });
        }

        // GET: SolicitudesNomina
        public ActionResult SolicitudesNomina(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_NOMINA))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();
            var listado = db.GV_comprobacion
                           .Where(x => (x.GV_solicitud.id_nomina == empleado.id)
                               && (
                               x.estatus == estatus ||
                               (estatus == "PENDIENTES" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               ))
                                               || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                               ))
                                                               //|| (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               //                //|| x.estatus == GV_comprobacion_estatus.ENVIADO_Nomina)
                                                               //                )
                                                               //                )
                                                               )
                           )
                           .OrderByDescending(x => x.GV_solicitud.fecha_solicitud)
                           .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                          .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_comprobacion
                    .Where(x => (x.GV_solicitud.id_nomina == empleado.id)
                               && (
                               x.estatus == estatus ||
                               (estatus == "PENDIENTES" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               ))
                                               || (estatus == "RECHAZADAS" && (x.estatus == GV_comprobacion_estatus.RECHAZADO_JEFE
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTROLLING
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_NOMINA
                                                               || x.estatus == GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                                                               ))
                                                               //|| (estatus == "EN_PROCESO" && (x.estatus == GV_comprobacion_estatus.ENVIADO_NOMINA
                                                               //                //|| x.estatus == GV_comprobacion_estatus.ENVIADO_Nomina)
                                                               //                )
                                                               //                )
                                                               )
                           )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = "Pendientes", Value = "PENDIENTES" });
            //newList.Add(new SelectListItem() { Text = "Por enviar", Value = GV_comprobacion_estatus.CREADO });
            //newList.Add(new SelectListItem() { Text = "En proceso", Value = "EN_PROCESO" });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = "RECHAZADAS" });
            newList.Add(new SelectListItem() { Text = "Finalizadas", Value = GV_comprobacion_estatus.FINALIZADO });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;
            ViewBag.SegundoNivel = "solicitudes_nomina";
            ViewBag.Title = "Solicitudes (Nómina)";

            //ordenar por fecha de solicitud más reciente
            return View("solicitudes", listado);

        }

        // GET: GV_solicitud/AutorizarNomina/5
        public ActionResult AutorizarNomina(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_NOMINA)) //Todos los usuarios van a tener permiso para crear solicitudes de GV, Por lo que solo se necesita validar este permiso
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //verifica si se puede autorizar
            if (gV_solicitud.GV_comprobacion.estatus != GV_comprobacion_estatus.ENVIADO_NOMINA)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede validar esta solicitud!";
                ViewBag.Descripcion = "No se puede autorizar una solicitud que ya ha sido validada, rechazada o finalizada.";

                return View("../Home/ErrorGenerico");
            }

            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            //ordena los rel de la comprobacion
            gV_solicitud.GV_comprobacion_rel_gastos = gV_solicitud.GV_comprobacion_rel_gastos.OrderByDescending(x => x.uuid).ThenBy(x => x.orden).ToList();

            ViewBag.ListaCuentas = db.GV_comprobacion_tipo_gastos_viaje.Where(x => x.activo).ToList();
            ViewBag.ListaTipoPago = db.GV_comprobacion_tipo_pago.Where(x => x.activo).ToList();
            ViewBag.ListaCentroCosto = db.GV_centros_costo.Where(x => x.activo).OrderBy(x => x.plantas.clave).ToList();

            //obtiene la lista de usuarios asociados a Cuentas x pagar (Nomina)
            //var listUsuarios = db.GV_usuarios.Where(x => x.departamento == Bitacoras.Util.GV_tipo_departamento.NOMINA).Select(x => x.id_empleado).ToList();
            //ViewBag.id_nomina = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value && listUsuarios.Contains(x.id)), "id", "ConcatNumEmpleadoNombre"));

            return View(gV_solicitud);
        }

        // POST: PremiumFreightAproval/Send/5
        [HttpPost, ActionName("AutorizarNomina")]
        [ValidateAntiForgeryToken]
        public ActionResult AutorizarNominaConfirmed(int? id, int? id_nomina, string s_estatus)
        {

            GV_comprobacion solicitud = db.GV_comprobacion.Find(id);
            if (solicitud == null)
            {
                return View("../Error/NotFound");
            }

            solicitud.estatus = GV_comprobacion_estatus.FINALIZADO;
            //solicitud.id_nomina = id_nomina;

            //establece las fechas 
            // solicitud.fecha_aceptacion_jefe_area = DateTime.Now;
            //solicitud.fecha_aceptacion_controlling = DateTime.Now;
            //solicitud.fecha_aceptacion_contabilidad = DateTime.Now;
            solicitud.fecha_aceptacion_nomina = DateTime.Now;

            db.Entry(solicitud).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO
                //agrega los correos de todos los involucrados
                //if (!String.IsNullOrEmpty(solicitud.empleados.correo)) //contabilidad
                //    correos.Add(solicitud.empleados.correo);
                if (!String.IsNullOrEmpty(solicitud.empleados1.correo)) //controlling
                    correos.Add(solicitud.empleados1.correo);
                //if (!String.IsNullOrEmpty(solicitud.empleados2.correo)) //jefeDirecto
                //    correos.Add(solicitud.empleados2.correo);
                //if (!String.IsNullOrEmpty(solicitud.empleados3.correo))     //nominas
                //    correos.Add(solicitud.empleados3.correo);
                //if (!String.IsNullOrEmpty(solicitud.GV_solicitud.empleados2.correo)) //empleado
                //    correos.Add(solicitud.GV_solicitud.empleados2.correo);
                if (!String.IsNullOrEmpty(solicitud.GV_solicitud.empleados5.correo)) //solicitante
                    correos.Add(solicitud.GV_solicitud.empleados5.correo);

                //elimina los correos repetidos
                correos = correos.Distinct().ToList();

                envioCorreo.SendEmailAsync(correos, "El proceso de Comprobación de Gastos para el folio #"+solicitud.id_gv_solicitud+" ha finalizado.", envioCorreo.getBodyGVComprobacionNotificacionFinalizado(solicitud));

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
                return RedirectToAction("SolicitudesNomina", new { estatus = s_estatus });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("SolicitudesNomina", new { estatus = s_estatus });
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido autorizada.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("SolicitudesNomina", new { estatus = s_estatus });

        }


        // POST: GV_solicitud/RechazarNomina/5
        [HttpPost, ActionName("RechazarNomina")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarNominaConfirmed(int? id, string s_estatus, [Bind(Prefix = "GV_comprobacion.comentario_rechazo")] string comentario_rechazo)
        {

            GV_comprobacion solicitud = db.GV_comprobacion.Find(id);
            solicitud.estatus = GV_comprobacion_estatus.RECHAZADO_NOMINA;
            solicitud.comentario_rechazo = comentario_rechazo;
            //borra las fechas 
            solicitud.fecha_aceptacion_jefe_area = null;
            solicitud.fecha_aceptacion_controlling = null;
            solicitud.fecha_aceptacion_nomina = null;
            solicitud.fecha_aceptacion_contabilidad = null;

            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                if (!String.IsNullOrEmpty(solicitud.GV_solicitud.empleados5.correo))
                    correos.Add(solicitud.GV_solicitud.empleados5.correo); //agrega correo de solicitantes

                envioCorreo.SendEmailAsync(correos, "Se ha rechazado su Comprobación de Gastos de Viaje.", envioCorreo.getBodyGVComprobacionNotificacionRechazo(solicitud, solicitud.empleados3.ConcatNombre));

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
                return RedirectToAction("SolicitudesNomina", new { estatus = s_estatus });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("SolicitudesNomina", new { estatus = s_estatus });
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La solicitud ha sido rechazada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("SolicitudesNomina", new { estatus = s_estatus });
        }

        // GET: ComprobacionGV/Details/5
        public ActionResult ComprobacionGastos(int? id)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD)) //Todos los usuarios van a tener permiso para crear solicitudes de GV, Por lo que solo se necesita validar este permiso
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            ViewBag.ListaCuentas = db.GV_comprobacion_tipo_gastos_viaje.Where(x => x.activo).ToList();
            ViewBag.ListaTipoPago = db.GV_comprobacion_tipo_pago.Where(x => x.activo).ToList();
            ViewBag.ListaCentroCosto = db.GV_centros_costo.Where(x => x.activo).OrderBy(x => x.plantas.clave).ToList();
            return View(gV_solicitud);
        }


        // POST: OrdenesTrabajo/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ComprobacionGastos(GV_solicitud solicitud)
        {
            if (solicitud.GV_comprobacion_rel_gastos.Count == 0)
                ModelState.AddModelError("", "No se agregaron conceptos a la comprobación de gastos.");

            //obtiene todos los posibles archivos

            //valida archivos, en caso de existir
            foreach (var concepto in solicitud.GV_comprobacion_rel_gastos)
            {
                string msj = string.Empty;

                //1.- valida archivo xml
                if (concepto.PostedFileXML != null)
                {
                    biblioteca_digital xml = ValidaArchivo(concepto.PostedFileXML, 2, new List<string> { ".xml" }, ref msj);
                    if (xml == null)//hubo un error
                        ModelState.AddModelError("", msj);
                    else //hubo exito
                        concepto.biblioteca_digital2 = xml;  //documento soporte
                }
                //2.- valida archivo pdf
                if (concepto.PostedFilePDF != null)
                {
                    biblioteca_digital pdf = ValidaArchivo(concepto.PostedFilePDF, 4, new List<string> { ".pdf" }, ref msj);
                    if (pdf == null)//hubo un error
                        ModelState.AddModelError("", msj);
                    else //hubo exito
                        concepto.biblioteca_digital1 = pdf;  //documento soporte
                }
                //3.- valida archivo extranjero
                if (concepto.PostedFileExtranjero != null)
                {
                    biblioteca_digital archivoExtranjero = ValidaArchivo(concepto.PostedFileExtranjero, 5, new List<string> { ".png", ".jpeg", ".png", ".rar", ".zip", ".pdf", ".xml" }, ref msj);
                    if (archivoExtranjero == null)//hubo un error
                        ModelState.AddModelError("", msj);
                    else //hubo exito
                        concepto.biblioteca_digital = archivoExtranjero;  //documento soporte
                }


            }

            //ModelState.AddModelError("", "Error prueba.");
            if (ModelState.IsValid)
            {
                solicitud.GV_comprobacion.fecha_comprobacion = DateTime.Now;
                solicitud.GV_comprobacion.estatus = Bitacoras.Util.GV_comprobacion_estatus.CREADO;

                //si existe lo modifica
                GV_solicitud solicitudBD = db.GV_solicitud.Find(solicitud.id);
                solicitud.GV_comprobacion.id_jefe_directo = solicitudBD.id_jefe_directo;
                solicitudBD.GV_comprobacion = solicitud.GV_comprobacion;
                solicitudBD.GV_comprobacion_rel_gastos = solicitud.GV_comprobacion_rel_gastos;
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert("Se ha creado el registro correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Solicitudes", new { estatus = "CREADO" });
            }

            //En caso de que el modelo no sea válido
            //guarda de forma temporal la comprobacion recibida en el formulario y lo asigna el modelo completo
            GV_comprobacion gV_ComprobacionRecibida = solicitud.GV_comprobacion;

            //guarda temporalmente los conceptos 
            var gV_Comprobacion_Rel_Gastos = solicitud.GV_comprobacion_rel_gastos;

            //asigna los valores te
            solicitud = db.GV_solicitud.Find(solicitud.id);
            solicitud.GV_comprobacion = gV_ComprobacionRecibida;
            solicitud.GV_comprobacion_rel_gastos = gV_Comprobacion_Rel_Gastos;

            //determina si aplica otro o no
            solicitud.medio_transporte_aplica_otro = !solicitud.id_medio_transporte.HasValue;

            ViewBag.ListaCuentas = db.GV_comprobacion_tipo_gastos_viaje.Where(x => x.activo).ToList();
            ViewBag.ListaTipoPago = db.GV_comprobacion_tipo_pago.Where(x => x.activo).ToList();
            ViewBag.ListaCentroCosto = db.GV_centros_costo.Where(x => x.activo).OrderBy(x => x.plantas.clave).ToList();

            return View(solicitud);
        }

        // GET: GV_solicitud/Edit/5
        public ActionResult Edit(int? id, string s_estatus)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD)) //Todos los usuarios van a tener permiso para crear solicitudes de GV, Por lo que solo se necesita validar este permiso
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //verifica si se puede editar
            if (gV_solicitud.GV_comprobacion.estatus != GV_comprobacion_estatus.CREADO && gV_solicitud.GV_comprobacion.estatus != GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD
                && gV_solicitud.GV_comprobacion.estatus != GV_comprobacion_estatus.RECHAZADO_CONTROLLING && gV_solicitud.GV_comprobacion.estatus != GV_comprobacion_estatus.RECHAZADO_JEFE
                && gV_solicitud.GV_comprobacion.estatus != GV_comprobacion_estatus.RECHAZADO_NOMINA
                )
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede editar esta solicitud!";
                ViewBag.Descripcion = "No se puede editar una solicitud que ya ha sido validada, rechazada o finalizada.";

                return View("../Home/ErrorGenerico");
            }


            empleados empleado = obtieneEmpleadoLogeado();
            //verifica si se puede editar
            if (gV_solicitud.id_empleado != empleado.id && gV_solicitud.id_solicitante != empleado.id)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede editar esta solicitud!";
                ViewBag.Descripcion = "No se puede editar una solicitud de otro solicitante.";

                return View("../Home/ErrorGenerico");
            }

            //envia los filtros de búsqueda previos
            gV_solicitud.s_estatus = s_estatus;

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;

            //ordena los rel de la comprobacion
            gV_solicitud.GV_comprobacion_rel_gastos = gV_solicitud.GV_comprobacion_rel_gastos.OrderByDescending(x => x.uuid).ThenBy(x => x.orden).ToList();


            ViewBag.ListaCuentas = db.GV_comprobacion_tipo_gastos_viaje.Where(x => x.activo).ToList();
            ViewBag.ListaTipoPago = db.GV_comprobacion_tipo_pago.Where(x => x.activo).ToList();
            ViewBag.ListaCentroCosto = db.GV_centros_costo.Where(x => x.activo).OrderBy(x => x.plantas.clave).ToList();
            return View(gV_solicitud);
        }

        // POST: OrdenesTrabajo/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GV_solicitud solicitud)
        {
            if (solicitud.GV_comprobacion_rel_gastos.Count == 0)
                ModelState.AddModelError("", "No se agregaron conceptos a la comprobación de gastos.");

            //obtiene todos los posibles archivos

            //valida y asigna archivos, en caso de existir
            foreach (var concepto in solicitud.GV_comprobacion_rel_gastos)
            {
                string msj = string.Empty;
                //relaciona cada concepto con la solicitud actual
                concepto.id_gv_solicitud = solicitud.id;

                //1.- valida archivo xml
                if (concepto.PostedFileXML != null)
                {
                    biblioteca_digital xml = ValidaArchivo(concepto.PostedFileXML, 2, new List<string> { ".xml" }, ref msj);
                    if (xml == null)//hubo un error
                        ModelState.AddModelError("", msj);
                    else //hubo exito
                        concepto.biblioteca_digital2 = xml;  //documento soporte
                }
                //2.- valida archivo pdf
                if (concepto.PostedFilePDF != null)
                {
                    biblioteca_digital pdf = ValidaArchivo(concepto.PostedFilePDF, 4, new List<string> { ".pdf" }, ref msj);
                    if (pdf == null)//hubo un error
                        ModelState.AddModelError("", msj);
                    else //hubo exito
                        concepto.biblioteca_digital1 = pdf;  //documento soporte
                }
                //3.- valida archivo extranjero
                if (concepto.PostedFileExtranjero != null)
                {
                    biblioteca_digital archivoExtranjero = ValidaArchivo(concepto.PostedFileExtranjero, 5, new List<string> { ".png", ".jpeg", ".png", ".rar", ".zip", ".pdf", ".xml" }, ref msj);
                    if (archivoExtranjero == null)//hubo un error
                        ModelState.AddModelError("", msj);
                    else //hubo exito
                        concepto.biblioteca_digital = archivoExtranjero;  //documento soporte
                }
                //4.- en caso de que exista id, asocia el documento de biblioteca digital existentes
                if (concepto.id > 0)
                {
                    var c = db.GV_comprobacion_rel_gastos.Find(concepto.id);

                    concepto.id_archivo_pdf = c.id_archivo_pdf;
                    concepto.id_archivo_xml = c.id_archivo_xml;
                    concepto.id_archivo_comprobante_extranjero = c.id_archivo_comprobante_extranjero;
                    //concepto.biblioteca_digital = c.biblioteca_digital;
                    //concepto.biblioteca_digital1 = c.biblioteca_digital1;
                    //concepto.biblioteca_digital2 = c.biblioteca_digital2;
                }
            }

            //ModelState.AddModelError("", "Error prueba.");
            if (ModelState.IsValid)
            {
                //borra los rel gastos que no aparecen en la lista recibida
                //todos los id de la bd
                var list = db.GV_comprobacion_rel_gastos.Where(x => x.id_gv_solicitud == solicitud.id);
                foreach (GV_comprobacion_rel_gastos item in list)
                {
                    //si el de la BD no está en los recibidos lo borra
                    if (!solicitud.GV_comprobacion_rel_gastos.Any(x => x.id == item.id))
                    {
                        //borra los archivos de la biblioteca digital si existen
                        if (item.id_archivo_pdf > 0)
                            db.biblioteca_digital.Remove(db.biblioteca_digital.Find(item.id_archivo_pdf));
                        if (item.id_archivo_xml > 0)
                            db.biblioteca_digital.Remove(db.biblioteca_digital.Find(item.id_archivo_xml));
                        if (item.id_archivo_comprobante_extranjero > 0)
                            db.biblioteca_digital.Remove(db.biblioteca_digital.Find(item.id_archivo_comprobante_extranjero));

                        //borrra la solicitud en general
                        db.GV_comprobacion_rel_gastos.Remove(item);
                    }

                }

                //actualiza o crea los rels recibidos
                foreach (var item in solicitud.GV_comprobacion_rel_gastos)
                {
                    //acctualiza
                    if (item.id > 0)
                    {
                        //obtiene una referencia al objeto de la base de datos
                        GV_comprobacion_rel_gastos comprobacionBD = db.GV_comprobacion_rel_gastos.Find(item.id);
                        // Activity already exist in database and modify it
                        db.Entry(comprobacionBD).CurrentValues.SetValues(item);
                        db.Entry(comprobacionBD).State = EntityState.Modified;
                    }
                    else
                    { //crea un registo nuevo                    
                        db.GV_comprobacion_rel_gastos.Add(item);
                    }
                }

                //obtiene una referencia al objeto de la base de datos
                GV_comprobacion solicitudBD = db.GV_comprobacion.Find(solicitud.id);
                // Activity already exist in database and modify it
                db.Entry(solicitudBD).CurrentValues.SetValues(solicitud.GV_comprobacion);
                db.Entry(solicitudBD).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert("Se ha editado el registro correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                if (solicitudBD.estatus.Contains("RECHAZADO"))
                    return RedirectToAction("Solicitudes", new { estatus = "RECHAZADAS" });
                else
                    return RedirectToAction("Solicitudes", new { estatus = "CREADO" });
            }

            //En caso de que el modelo no sea válido
            //guarda de forma temporal la comprobacion recibida en el formulario y lo asigna el modelo completo
            GV_comprobacion gV_ComprobacionRecibida = solicitud.GV_comprobacion;

            //guarda temporalmente los conceptos 
            var gV_Comprobacion_Rel_Gastos = solicitud.GV_comprobacion_rel_gastos;

            //asigna los valores te
            solicitud = db.GV_solicitud.Find(solicitud.id);
            solicitud.GV_comprobacion = gV_ComprobacionRecibida;
            solicitud.GV_comprobacion_rel_gastos = gV_Comprobacion_Rel_Gastos;

            //determina si aplica otro o no
            solicitud.medio_transporte_aplica_otro = !solicitud.id_medio_transporte.HasValue;

            ViewBag.ListaCuentas = db.GV_comprobacion_tipo_gastos_viaje.Where(x => x.activo).ToList();
            ViewBag.ListaTipoPago = db.GV_comprobacion_tipo_pago.Where(x => x.activo).ToList();
            ViewBag.ListaCentroCosto = db.GV_centros_costo.Where(x => x.activo).OrderBy(x => x.plantas.clave).ToList();

            return View(solicitud);
        }

        /// <summary>
        /// Método que lee el contenido de una factura xml
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult LeeArchivoXML(int numConcepto = 0)
        {
            var result = new object[1];

            int indexKey = Request.Files.AllKeys.ToList().IndexOf("archivo_nacional_xml");
            HttpPostedFileBase fileXML = Request.Files[indexKey];

            MemoryStream newStream = new MemoryStream();
            fileXML.InputStream.CopyTo(newStream);

            //Valida que se haya enviado un archivo
            if (fileXML.FileName == "")
            {
                result[0] = new { status = "ERROR", value = "Seleccione un archivo .xml" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //valida que sea un archivo en excel
            string extensionXML = Path.GetExtension(fileXML.FileName);

            if (extensionXML.ToUpper() != ".XML")
            {
                result[0] = new { status = "ERROR", value = "Sólo se permiten archivos con extensión .xml." };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            #region LeeXML

            string estatus = string.Empty;
            string msj = string.Empty;
            Comprobante factura_3_3 = null;
            Bitacoras.CFDI_4_0.Comprobante factura_4_0 = null;
            //primero trata de leer la factura 3.3
            try
            {
                fileXML.InputStream.Position = 0;
                factura_3_3 = XmlUtil.LeeXML_3_3(fileXML.InputStream);

            }
            catch (Exception e)
            {
                //si falla al leer la 3.3, trata de leer la 4.0
                try
                {
                    newStream.Position = 0;
                    factura_4_0 = XmlUtil.LeeXML_4_0(newStream);
                }
                catch (Exception ex)
                {
                    estatus = "ERROR";
                    msj = "Ocurrió un error al leer la estructura del XML: " + ex.Message;
                }

            }

            //si existe en cofidi, sustituye el valor de la clase comprobante
            if (ExisteEnCofidi(factura_3_3, factura_4_0))
            {
                string uuid = string.Empty;
                CFDProveedor factDB = null;

                if (factura_3_3 != null)
                    uuid = factura_3_3.TimbreFiscalDigital.UUID;
                if (factura_4_0 != null)
                    uuid = factura_4_0.TimbreFiscalDigital.UUID;

                //busca en cofidi
                string xmlText = String.Empty;
                using (var db = new ATEBCOFIDIEntities())
                {
                    factDB = db.CFDProveedor.Where(x => x.UUID == uuid).FirstOrDefault();

                    if (factDB == null)
                    {
                        estatus = "404";
                        msj = "No se encontró una factura asociada.";
                    }
                    else
                    { //si se encontro el uuid
                      //primero trata de leer la factura 3.3
                      // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(factDB.CFDOriginal);
                        //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
                        MemoryStream stream2 = new MemoryStream(byteArray);
                        MemoryStream newStream2 = new MemoryStream();
                        stream2.CopyTo(newStream2);

                        try
                        {
                            stream2.Position = 0;
                            factura_3_3 = XmlUtil.LeeXML_3_3(stream2);
                        }
                        catch (Exception e)
                        {
                            //si falla al leer la 3.3, trata de leer la 4.0
                            try
                            {
                                newStream2.Position = 0;
                                factura_4_0 = XmlUtil.LeeXML_4_0(newStream2);
                            }
                            catch (Exception ex)
                            {
                                estatus = "ERROR";
                                msj = "Ocurrió un error al leer la estructura del XML: " + ex.Message;
                            }

                        }

                    }
                }
            }

            #endregion

            //si hubo algún error al leer el archivo
            if (estatus.ToUpper() == "ERROR")
            {
                result[0] = new { status = estatus, value = msj };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //si el archivo se lee correctamente               
                //llama al método que lee la factura
                result = GetRowsFactura(factura_3_3, factura_4_0, numConcepto: numConcepto);
                var json = Json(result, JsonRequestBehavior.AllowGet);
                return json;
            }
        }


        /// <summary>
        /// Método que busca y lee el contenido de una factura xml desde cofidi
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult BuscaCOFIDI(string uuid_field, int numConcepto = 0)
        {
            var result = new object[1];

            string estatus = string.Empty;
            string msj = string.Empty;
            Comprobante factura_3_3 = null;
            Bitacoras.CFDI_4_0.Comprobante factura_4_0 = null;

            CFDProveedor factDB = null;

            //valida uuid
            if (String.IsNullOrEmpty(uuid_field))
            {
                estatus = "ERROR";
                msj = "El campo UUID es requerido.";
            }
            else if (uuid_field.Contains("_"))
            {
                estatus = "ERROR";
                msj = "El campo UUID no es válido. Favor de verificarlo.";
            }
            else //la longitud es valida
            {
                //busca en cofidi
                string xmlText = String.Empty;
                using (var db = new ATEBCOFIDIEntities())
                {
                    factDB = db.CFDProveedor.Where(x => x.UUID == uuid_field).FirstOrDefault();

                    if (factDB == null)
                    {
                        estatus = "404";
                        msj = "No se encontró una factura asociada.";
                    }
                    else
                    { //si se encontro el uuid
                      //primero trata de leer la factura 3.3
                      // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(factDB.CFDOriginal);
                        //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
                        MemoryStream stream = new MemoryStream(byteArray);
                        MemoryStream newStream = new MemoryStream();
                        stream.CopyTo(newStream);

                        try
                        {
                            stream.Position = 0;
                            factura_3_3 = XmlUtil.LeeXML_3_3(stream);
                        }
                        catch (Exception e)
                        {
                            //si falla al leer la 3.3, trata de leer la 4.0
                            try
                            {
                                newStream.Position = 0;
                                factura_4_0 = XmlUtil.LeeXML_4_0(newStream);
                            }
                            catch (Exception ex)
                            {
                                estatus = "ERROR";
                                msj = "Ocurrió un error al leer la estructura del XML: " + ex.Message;
                            }

                        }

                    }
                }
            }

            //si hubo algún error al leer el archivo
            if (estatus.ToUpper() == "ERROR" || estatus.ToUpper() == "404")
            {
                result[0] = new { status = estatus, value = msj };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            { //si el archivo se lee correctamente               
                //llama al método que lee la factura
                result = GetRowsFactura(factura_3_3, factura_4_0, numConcepto: numConcepto);
                var json = Json(result, JsonRequestBehavior.AllowGet);
                return json;
            }

        }

        [NonAction]
        public bool ExisteEnCofidi(Comprobante factura_3_3 = null, Bitacoras.CFDI_4_0.Comprobante factura_4_0 = null)
        {
            string uuid = string.Empty;

            if (factura_3_3 != null)
                uuid = factura_3_3.TimbreFiscalDigital.UUID;
            if (factura_4_0 != null)
                uuid = factura_4_0.TimbreFiscalDigital.UUID;

            using (var db = new ATEBCOFIDIEntities())
            {
                var factDB = db.CFDProveedor.Where(x => x.UUID == uuid).FirstOrDefault();

                //si no se encontro la factura en COFIDI
                if (factDB == null)
                {
                    return false;
                }
                else //si es diferente de null, existe la factura
                {
                    return true;
                }
            }

        }

        [NonAction]
        public object[] GetRowsFactura(Comprobante factura_3_3 = null, Bitacoras.CFDI_4_0.Comprobante factura_4_0 = null, int numConcepto = 0)
        {
            var result = new object[1];
            string body = string.Empty;
            string div_ocultos = string.Empty;
            int cantidadConceptos = 0;
            string estatus = string.Empty;
            string msj = string.Empty;
            string uuid = string.Empty;
            string inputFiles = string.Empty;

            bool existeEnCOFIDI = ExisteEnCofidi(factura_3_3, factura_4_0);

            if (existeEnCOFIDI)
            {
                msj = " La factura existe en COFIDI, Se tomaron los datos desde CODIFI.";
            }

            List<GV_comprobacion_tipo_gastos_viaje> listaCuentas = db.GV_comprobacion_tipo_gastos_viaje.Where(x => x.activo).ToList();
            List<GV_comprobacion_tipo_pago> listaTipoPago = db.GV_comprobacion_tipo_pago.Where(x => x.activo).ToList();
            List<GV_centros_costo> listaCentroCosto = db.GV_centros_costo.Where(x => x.activo).OrderBy(x => x.plantas.clave).ToList();

            //verifica Timbrado
            if ((factura_3_3 != null && factura_3_3.TimbreFiscalDigital == null) || (factura_4_0 != null && factura_4_0.TimbreFiscalDigital == null))
            {
                result[0] = new { status = "ERROR", value = "La factura no está timbrada", num_conceptos = cantidadConceptos, uuid = uuid, msj = msj, div_ocultos = div_ocultos };
                return result;
            }


            if (factura_3_3 != null)
            {
                uuid = factura_3_3.TimbreFiscalDigital.UUID;

                //crea los imput para los archivos en caso de no existir en COFIDI
                if (!existeEnCOFIDI)
                    inputFiles = @" <label for=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFileXML"" class=""control-label col-md-1"">XML</label>
                                    <div class=""col-md-11"">
                                        <div class=""input-group"">
                                            <div class=""custom-file"">
                                                <input type =""file"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFileXML"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFileXML"" class=""custom-file-input"" accept="".xml,.XML"" required/>
                                                <label class=""custom-file-label"" for=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFileXML"" style=""text-align:left"">Seleccione un archivo...</label>
                                            </div>
                                        </div>
                                        <span class=""field-validation-valid text-danger control-label"" data-valmsg-for=GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFileXML data-valmsg-replace=""true""></span>
                                    </div> 
                                    <label for=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFilePDF"" class=""control-label col-md-1"">PDF</label>
                                    <div class=""col-md-11"">
                                        <div class=""input-group"">
                                            <div class=""custom-file"">
                                                <input type =""file"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFilePDF"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFilePDF"" class=""custom-file-input"" accept="".pdf,.PDF"" required/>
                                                <label class=""custom-file-label"" for=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFilePDF"" style=""text-align:left"">Seleccione un archivo...</label>
                                            </div>                                                            
                                        </div>
                                        <span class=""field-validation-valid text-danger control-label"" data-valmsg-for=GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFilePDF data-valmsg-replace=""true""></span>
                                    </div> 
                                    ";
                string btnXML = @"<a href=""/Combos/MuestraArchivo/?uuid=" + factura_3_3.TimbreFiscalDigital.UUID + @""" class=""btn btn-info"" title=""Ver PDF"" target=""_blank""><i class=""fa-solid fa-file-pdf""></i></a>
                                  <a href=""/Combos/MuestraArchivoXML/?uuid=" + factura_3_3.TimbreFiscalDigital.UUID + @""" class=""btn btn-info"" title=""Ver XML"" target=""_blank""><i class=""fa-solid fa-code""></i></a>
                                  ";

                //obtiene el valor de los impuestos locales trasladados de la factura
                decimal impuestosLocales = 0;
                if (factura_3_3.ImpuestosLocales != null)
                    impuestosLocales = factura_3_3.ImpuestosLocales.TotaldeTraslados;
                estatus = "OK";

                //lee CABECERA de la factura
                body = String.Format(@"<tr style=""background-color:#FFEB9C"" class=""div_" + factura_3_3.TimbreFiscalDigital.UUID + @""">
                                                <input type=""hidden"" name=""GV_comprobacion_rel_gastos.Index"" id=""GV_comprobacion_rel_gastos.Index"" value=""" + numConcepto + @""" />
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" value=""" + (existeEnCOFIDI ? GV_comprobacion_origen.COFIDI_RESUMEN : GV_comprobacion_origen.XML_RESUMEN) + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" value=""" + factura_3_3.SubTotal + @""">                                                
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" value=""" + factura_3_3.GetTotalIVAImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" value=""" + factura_3_3.GetTotalISRImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" value=""" + factura_3_3.GetTotalIEPSImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" value=""" + (factura_3_3.Impuestos != null ? factura_3_3.Impuestos.TotalImpuestosRetenidos : 0) + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].impuestos_locales"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].impuestos_locales"" value=""" + impuestosLocales + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" value=""" + factura_3_3.Total + @""">
                                                <td class=""input-contador-conceptos""></td> 
                                                <td>" + Bitacoras.Util.GV_comprobacion_origen.DescripcionOrigen((existeEnCOFIDI ? GV_comprobacion_origen.COFIDI_RESUMEN : GV_comprobacion_origen.XML_RESUMEN)) + @" </td> 
                                                <td><b>Fecha:</b> {0}</td> 
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].fecha_factura"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].fecha_factura"" value=""" + factura_3_3.Fecha + @""">
                                                <td colspan=""2"" nowrap><b>UUID:</b> <custom-div class=""class-uuid"">{1}</custom-div></td> 
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" value=""" + factura_3_3.TimbreFiscalDigital.UUID + @""">
                                                <td colspan=""1""> <b>Tipo de Cambio:</b> {2}</td> 
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].tipo_cambio"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].tipo_cambio"" value=""{2}"">
                                                <td colspan=""2""> <b>Moneda:</b> {3}</td>                                                
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].currency_iso"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].currency_iso"" value=""" + factura_3_3.Moneda + @""">
                                                <td colspan=""8"">{6}{4}</td>                                               
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td>
                                                    <textarea style=""font-size:12px;"" type=""text"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].comentario"" id=""GV_comprobacion_rel_gastos[@contador].comentario"" class=""form-control col-md-12"" autocomplete=""off"" maxlength=""150"" rows=""5""></textarea>
                                                    <span class=""field-validation-valid text-danger"" data-valmsg-for=""GV_comprobacion_rel_gastos[" + numConcepto + @"].comentario"" data-valmsg-replace=""true""></span>
                                                </td>
                                                <td>{5}</td>
                                                {7}
                                            </tr>
                    ", factura_3_3.Fecha.ToShortDateString(), factura_3_3.TimbreFiscalDigital.UUID
                 , factura_3_3.TipoCambio == 0 ? 1 : factura_3_3.TipoCambio, factura_3_3.Moneda
                , existeEnCOFIDI ? btnXML : String.Empty
                , @"<input type=""button"" value=""Borrar"" class=""btn btn-danger"" onclick=""borrarConcepto('" + factura_3_3.TimbreFiscalDigital.UUID + @"'); return false; "">"
                , inputFiles
                , @"<input type = ""hidden"" id = ""GV_comprobacion_rel_gastos[" + numConcepto + @"].ExisteEnCOFIDI"" name = ""GV_comprobacion_rel_gastos[" + numConcepto + @"].ExisteEnCOFIDI"" value = """ + (existeEnCOFIDI ? "true" : "false") + @""" >"
                );

                //aumenta en 1 el número de concepto (debido a la cabecera)
                numConcepto++;

                #region selects
                //crea el select para el tipo de cuenta
                string selectCuenta = @"<select name = 'GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_gastos_viaje' id = 'GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_gastos_viaje' class=""form-control select2bs4"" style=""width:100%"" required>
                                                <option value = '' > --Seleccione un valor --</option>";

                foreach (var cuenta in listaCuentas)
                    selectCuenta += @"<option value = """ + cuenta.id + @""">" + cuenta.ConcatCuenta + @"</option>";

                selectCuenta += @"</select><span class=""field-validation-valid text-danger"" data-valmsg-for=GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_gastos_viaje data-valmsg-replace=""true""></span> ";

                //crea el select para el tipo de pago
                string selectTipoPago = @"<select name = 'GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_pago' id = 'GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_pago' class=""form-control select2bs4"" style=""width:100%"" required>
                                                <option value = '' > --Seleccione un valor --</option>";

                foreach (var tipo in listaTipoPago)
                    selectTipoPago += @"<option value = """ + tipo.id + @""">" + tipo.descripcion + @"</option>";

                selectTipoPago += @"</select><span class=""field-validation-valid text-danger"" data-valmsg-for=GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_pago data-valmsg-replace=""true""></span> ";

                //crea el select para el centro d costo
                string selectCentroCosto = @"<select name = 'GV_comprobacion_rel_gastos[#ID].id_centro_costo' id = 'GV_comprobacion_rel_gastos[#ID].id_centro_costo' class=""form-control select2bs4"" style=""width:100%"" required>
                                                <option value = '' > --Seleccione un valor --</option>";

                foreach (var planta in listaCentroCosto.Select(x => x.plantas).Distinct())
                {
                    selectCentroCosto += @"<optgroup label= """ + planta.descripcion + @""">";
                    foreach (var cc in listaCentroCosto.Where(x => x.clave_planta == planta.clave))
                        selectCentroCosto += @"<option value = """ + cc.id + @""">" + cc.ConcatCentroDeptoPlanta + @"</option>";
                    selectCentroCosto += "</optgroup>";
                }
                selectCentroCosto += @"</select><span class=""field-validation-valid text-danger"" data-valmsg-for=GV_comprobacion_rel_gastos[#ID].id_centro_costo data-valmsg-replace=""true""></span> ";

                #endregion

                //agrega info para cada uno de los conceptos
                foreach (var concepto in factura_3_3.Conceptos)
                {
                    body += String.Format(@"<tr style=""background-color:#C9DCC1"" class=""div_" + factura_3_3.TimbreFiscalDigital.UUID + @""">   
                                                <input type=""hidden"" name=""GV_comprobacion_rel_gastos.Index"" id=""GV_comprobacion_rel_gastos.Index"" value=""" + numConcepto + @""" />
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" value=""" + (existeEnCOFIDI ? GV_comprobacion_origen.COFIDI_CONCEPTO : GV_comprobacion_origen.XML_CONCEPTO) + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" value=""" + factura_3_3.TimbreFiscalDigital.UUID + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].descripcion"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].descripcion"" value=""" + UsoStrings.RecortaString(concepto.Descripcion, 350) + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].cantidad"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].cantidad"" value=""" + concepto.Cantidad + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].precio_unitario"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].precio_unitario"" value=""" + concepto.ValorUnitario + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" value=""" + concepto.Importe + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_porcentaje"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_porcentaje"" value=""" + concepto.GetIVATasa() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" value=""" + concepto.GetIVAImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_porcentaje"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_porcentaje"" value=""" + concepto.GetISRTasa() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" value=""" + concepto.GetISRImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_porcentaje"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_porcentaje"" value=""" + concepto.GetIEPSTasa() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" value=""" + concepto.GetIEPSImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" value=""" + concepto.GetTotalRetenciones() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" value=""" + concepto.GetTotalImporteConTransladosyRetenciones() + @""">
                                                <td colspan=""1""></td>
                                                <td colspan=""1""></td>
                                                <td nowrap>{12}</td>
                                                <td nowrap>{13}</td>
                                                <td>{0}</td> 
                                                <td>{14}</td> 
                                                <td style=""text-align:right"">{1}</td> 
                                                <td nowrap style=""text-align:right"" >{2}</td> 
                                                <td nowrap style=""text-align:right"">{3}</td> 
                                                <td nowrap style=""text-align:right"">{4}</td> 
                                                <td nowrap style=""text-align:right"">{5}</td> 
                                                <td nowrap style=""text-align:right"">{6}</td> 
                                                <td nowrap style=""text-align:right"">{7}</td> 
                                                <td nowrap style=""text-align:right"">{8}</td> 
                                                <td nowrap style=""text-align:right"">{9}</td>   
                                                <td nowrap style=""color:#C10000;text-align:right"">{10}</td> 
                                                <td></td>
                                                <td nowrap style=""text-align:right"">{11}</td> 
                                                <td colspan=""3""></td>
                                            </tr>
                            ", UsoStrings.RecortaString(concepto.Descripcion, 350), concepto.Cantidad.ToString("0.00"), concepto.ValorUnitario.ToString("$ #,##0.00"), concepto.Importe.ToString("$ #,##0.00")
                        , concepto.GetIVATasa() == 0 ? "--" : (concepto.GetIVATasa()).ToString("0.00 %"), concepto.GetIVAImporte() == 0 ? "--" : concepto.GetIVAImporte().ToString("$ #,##0.00")
                        , concepto.GetISRTasa() == 0 ? "--" : (concepto.GetISRTasa()).ToString("0.00 %"), concepto.GetISRImporte() == 0 ? "--" : concepto.GetISRImporte().ToString("$ #,##0.00")
                        , concepto.GetIEPSTasa() == 0 ? "--" : (concepto.GetIEPSTasa()).ToString("0.00 %"), concepto.GetIEPSImporte() == 0 ? "--" : concepto.GetIEPSImporte().ToString("$ #,##0.00")
                        //, concepto.GetTotalImpuestos() == 0 ? "--" : concepto.GetTotalImpuestos().ToString("$ #,##0.00")
                        , concepto.GetTotalRetenciones() == 0 ? "--" : concepto.GetTotalRetenciones().ToString("$ #,##0.00")
                        , concepto.GetTotalImporteConTransladosyRetenciones().ToString("$ #,##0.00")
                        , selectCuenta.Replace("#ID", numConcepto.ToString())
                        , selectCentroCosto.Replace("#ID", numConcepto.ToString())
                        , selectTipoPago.Replace("#ID", numConcepto.ToString())
                        );

                    //aumenta en 1 el número de concepto (por cada concepto dentro de la factura)
                    numConcepto++;
                }

                //agrega los TOTALES de la factura
                body += String.Format(@"<tr style=""background-color:#AAE0FF"" class=""div_" + factura_3_3.TimbreFiscalDigital.UUID + @""">
                                                <input type=""hidden"" name=""GV_comprobacion_rel_gastos.Index"" id=""GV_comprobacion_rel_gastos.Index"" value=""" + numConcepto + @""" />
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" value=""" + (existeEnCOFIDI ? GV_comprobacion_origen.COFIDI_TOTALES : GV_comprobacion_origen.XML_TOTALES) + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" value=""" + factura_3_3.SubTotal + @""">                                                
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" value=""" + factura_3_3.GetTotalIVAImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" value=""" + factura_3_3.GetTotalISRImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" value=""" + factura_3_3.GetTotalIEPSImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" value=""" + (factura_3_3.Impuestos != null ? factura_3_3.Impuestos.TotalImpuestosRetenidos : 0) + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].impuestos_locales"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].impuestos_locales"" value=""" + impuestosLocales + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" value=""" + factura_3_3.Total + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].fecha_factura"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].fecha_factura"" value=""" + factura_3_3.Fecha + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" value=""" + factura_3_3.TimbreFiscalDigital.UUID + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].currency_iso"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].currency_iso"" value=""" + factura_3_3.Moneda + @""">
                                                <td></td> 
                                                <td colspan=""7"" style=""text-align:right""><b>Totales Factura:</b></td> 
                                                <td nowrap style=""text-align:right"">{0}</td> 
                                                <td></td> 
                                                <td nowrap style=""text-align:right"">{1}</td> 
                                                <td></td> 
                                                <td nowrap style=""text-align:right"">{2}</td> 
                                                <td></td> 
                                                <td nowrap style=""text-align:right"">{3}</td>      
                                                <td nowrap style=""color:#C10000;text-align:right"">{4}</td> 
                                                <td nowrap style=""text-align:right"">{5}</td>
                                                <td nowrap style=""text-align:right"">{6}</td> 
                                                <td nowrap class=""item-total-mxn"" style=""text-align:right; font-weight:bold;"">{7}</td>
                                                <td colspan=""2""></td>
                                            </tr>
                    "
                , factura_3_3.SubTotal.ToString("$ #,##0.00")
                , factura_3_3.GetTotalIVAImporte() == 0 ? "--" : factura_3_3.GetTotalIVAImporte().ToString("$ #,##0.00")
                , factura_3_3.GetTotalISRImporte() == 0 ? "--" : factura_3_3.GetTotalISRImporte().ToString("$ #,##0.00")
                , factura_3_3.GetTotalIEPSImporte() == 0 ? "--" : factura_3_3.GetTotalIEPSImporte().ToString("$ #,##0.00")
                //, factura_3_3.Impuestos.TotalImpuestosTrasladados == 0 ? "--" : factura_3_3.Impuestos.TotalImpuestosTrasladados.ToString("$ #,##0.00")
                , (factura_3_3.Impuestos == null || factura_3_3.Impuestos.TotalImpuestosRetenidos == 0) ? "--" : factura_3_3.Impuestos.TotalImpuestosRetenidos.ToString("$ #,##0.00")
                , factura_3_3.ImpuestosLocales == null ? "--" : factura_3_3.ImpuestosLocales.TotaldeTraslados.ToString("$ #,##0.00")
                , factura_3_3.Total.ToString("$ #,##0.00")
                , factura_3_3.Total.ToString("$ #,##0.00")
                );

                //aumenta en 1 el número de concepto (por cada concepto dentro de la factura)
                numConcepto++;

                //indica el número de conceptos de la factura + la cabecera
                cantidadConceptos = factura_3_3.Conceptos.Count() + 2;
            }
            else if (factura_4_0 != null)
            {
                uuid = factura_4_0.TimbreFiscalDigital.UUID;

                //crea los imput para los archivos en caso de no existir en COFIDI
                if (!existeEnCOFIDI)
                    inputFiles = @" <label for=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFileXML"" class=""control-label col-md-1"">XML</label>
                                    <div class=""col-md-11"">
                                        <div class=""input-group"">
                                            <div class=""custom-file"">
                                                <input type =""file"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFileXML"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFileXML"" class=""custom-file-input"" accept="".xml,.XML"" required/>
                                                <label class=""custom-file-label"" for=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFileXML"" style=""text-align:left"">Seleccione un archivo...</label>
                                            </div>
                                        </div>
                                        <span class=""field-validation-valid text-danger control-label"" data-valmsg-for=GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFileXML data-valmsg-replace=""true""></span>
                                    </div> 
                                    <label for=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFilePDF"" class=""control-label col-md-1"">PDF</label>
                                    <div class=""col-md-11"">
                                        <div class=""input-group"">
                                            <div class=""custom-file"">
                                                <input type =""file"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFilePDF"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFilePDF"" class=""custom-file-input"" accept="".pdf,.PDF"" required/>
                                                <label class=""custom-file-label"" for=""GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFilePDF"" style=""text-align:left"">Seleccione un archivo...</label>
                                            </div>                                                            
                                        </div>
                                        <span class=""field-validation-valid text-danger control-label"" data-valmsg-for=GV_comprobacion_rel_gastos[" + numConcepto + @"].PostedFilePDF data-valmsg-replace=""true""></span>
                                    </div> 
                                    ";
                string btnXML = @"<a href=""/Combos/MuestraArchivo/?uuid=" + factura_4_0.TimbreFiscalDigital.UUID + @""" class=""btn btn-info"" title=""Ver PDF"" target=""_blank""><i class=""fa-solid fa-file-pdf""></i></a>
                                  <a href=""/Combos/MuestraArchivoXML/?uuid=" + factura_4_0.TimbreFiscalDigital.UUID + @""" class=""btn btn-info"" title=""Ver XML"" target=""_blank""><i class=""fa-solid fa-code""></i></a>
                                  ";

                //obtiene el valor de los impuestos locales trasladados de la factura
                decimal impuestosLocales = 0;
                if (factura_4_0.ImpuestosLocales != null)
                    impuestosLocales = factura_4_0.ImpuestosLocales.TotaldeTraslados;
                estatus = "OK";

                //lee CABECERA de la factura
                body = String.Format(@"<tr style=""background-color:#FFEB9C"" class=""div_" + factura_4_0.TimbreFiscalDigital.UUID + @""">
                                                <input type=""hidden"" name=""GV_comprobacion_rel_gastos.Index"" id=""GV_comprobacion_rel_gastos.Index"" value=""" + numConcepto + @""" />
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" value=""" + (existeEnCOFIDI ? GV_comprobacion_origen.COFIDI_RESUMEN : GV_comprobacion_origen.XML_RESUMEN) + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" value=""" + factura_4_0.SubTotal + @""">                                                
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" value=""" + factura_4_0.GetTotalIVAImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" value=""" + factura_4_0.GetTotalISRImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" value=""" + factura_4_0.GetTotalIEPSImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" value=""" + (factura_4_0.Impuestos != null ? factura_4_0.Impuestos.TotalImpuestosRetenidos : 0) + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].impuestos_locales"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].impuestos_locales"" value=""" + impuestosLocales + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" value=""" + factura_4_0.Total + @""">
                                                <td class=""input-contador-conceptos""></td> 
                                                <td>" + Bitacoras.Util.GV_comprobacion_origen.DescripcionOrigen(existeEnCOFIDI ? GV_comprobacion_origen.COFIDI_RESUMEN : GV_comprobacion_origen.XML_RESUMEN) + @" </td> 
                                                <td><b>Fecha:</b> {0}</td> 
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].fecha_factura"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].fecha_factura"" value=""" + factura_4_0.Fecha + @""">
                                                <td colspan=""2"" nowrap><b>UUID:</b> <custom-div class=""class-uuid"">{1}</custom-div></td> 
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" value=""" + factura_4_0.TimbreFiscalDigital.UUID + @""">
                                                <td colspan=""1""> <b>Tipo de Cambio:</b> {2}</td> 
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].tipo_cambio"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].tipo_cambio"" value=""{2}"">
                                                <td colspan=""2""> <b>Moneda:</b> {3}</td>                                                
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].currency_iso"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].currency_iso"" value=""" + factura_4_0.Moneda + @""">
                                                <td colspan=""8"">{6}{4}</td>                                               
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td>
                                                    <textarea style=""font-size:12px;"" type=""text"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].comentario"" id=""GV_comprobacion_rel_gastos[@contador].comentario"" class=""form-control col-md-12"" autocomplete=""off"" maxlength=""150"" rows=""5""></textarea>
                                                    <span class=""field-validation-valid text-danger"" data-valmsg-for=""GV_comprobacion_rel_gastos[" + numConcepto + @"].comentario"" data-valmsg-replace=""true""></span>
                                                </td>
                                                <td>{5}</td>
                                                {7}
                                            </tr>
                    ", factura_4_0.Fecha.ToShortDateString(), factura_4_0.TimbreFiscalDigital.UUID
                 , factura_4_0.TipoCambio == 0 ? 1 : factura_4_0.TipoCambio, factura_4_0.Moneda
                , existeEnCOFIDI ? btnXML : String.Empty
                , @"<input type=""button"" value=""Borrar"" class=""btn btn-danger"" onclick=""borrarConcepto('" + factura_4_0.TimbreFiscalDigital.UUID + @"'); return false; "">"
                , inputFiles
                , @"<input type = ""hidden"" id = ""GV_comprobacion_rel_gastos[" + numConcepto + @"].ExisteEnCOFIDI"" name = ""GV_comprobacion_rel_gastos[" + numConcepto + @"].ExisteEnCOFIDI"" value = """ + (existeEnCOFIDI ? "true" : "false") + @""" >"
                );

                //aumenta en 1 el número de concepto (debido a la cabecera)
                numConcepto++;

                #region selects
                //crea el select para el tipo de cuenta
                string selectCuenta = @"<select name = 'GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_gastos_viaje' id = 'GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_gastos_viaje' class=""form-control select2bs4"" style=""width:100%"" required>
                                                <option value = '' > --Seleccione un valor --</option>";

                foreach (var cuenta in listaCuentas)
                    selectCuenta += @"<option value = """ + cuenta.id + @""">" + cuenta.ConcatCuenta + @"</option>";

                selectCuenta += @"</select><span class=""field-validation-valid text-danger"" data-valmsg-for=GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_gastos_viaje data-valmsg-replace=""true""></span> ";

                //crea el select para el tipo de pago
                string selectTipoPago = @"<select name = 'GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_pago' id = 'GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_pago' class=""form-control select2bs4"" style=""width:100%"" required>
                                                <option value = '' > --Seleccione un valor --</option>";

                foreach (var tipo in listaTipoPago)
                    selectTipoPago += @"<option value = """ + tipo.id + @""">" + tipo.descripcion + @"</option>";

                selectTipoPago += @"</select><span class=""field-validation-valid text-danger"" data-valmsg-for=GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_pago data-valmsg-replace=""true""></span> ";

                //crea el select para el centro d costo
                string selectCentroCosto = @"<select name = 'GV_comprobacion_rel_gastos[#ID].id_centro_costo' id = 'GV_comprobacion_rel_gastos[#ID].id_centro_costo' class=""form-control select2bs4"" style=""width:100%"" required>
                                                <option value = '' > --Seleccione un valor --</option>";

                foreach (var planta in listaCentroCosto.Select(x => x.plantas).Distinct())
                {
                    selectCentroCosto += @"<optgroup label= """ + planta.descripcion + @""">";
                    foreach (var cc in listaCentroCosto.Where(x => x.clave_planta == planta.clave))
                        selectCentroCosto += @"<option value = """ + cc.id + @""">" + cc.ConcatCentroDeptoPlanta + @"</option>";
                    selectCentroCosto += "</optgroup>";
                }
                selectCentroCosto += @"</select><span class=""field-validation-valid text-danger"" data-valmsg-for=GV_comprobacion_rel_gastos[#ID].id_centro_costo data-valmsg-replace=""true""></span> ";

                #endregion

                //agrega info para cada uno de los conceptos
                foreach (var concepto in factura_4_0.Conceptos)
                {
                    body += String.Format(@"<tr style=""background-color:#C9DCC1"" class=""div_" + factura_4_0.TimbreFiscalDigital.UUID + @""">   
                                                <input type=""hidden"" name=""GV_comprobacion_rel_gastos.Index"" id=""GV_comprobacion_rel_gastos.Index"" value=""" + numConcepto + @""" />
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" value=""" + (existeEnCOFIDI ? GV_comprobacion_origen.COFIDI_CONCEPTO : GV_comprobacion_origen.XML_CONCEPTO) + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" value=""" + factura_4_0.TimbreFiscalDigital.UUID + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].descripcion"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].descripcion"" value=""" + UsoStrings.RecortaString(concepto.Descripcion, 350) + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].cantidad"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].cantidad"" value=""" + concepto.Cantidad + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].precio_unitario"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].precio_unitario"" value=""" + concepto.ValorUnitario + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" value=""" + concepto.Importe + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_porcentaje"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_porcentaje"" value=""" + concepto.GetIVATasa() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" value=""" + concepto.GetIVAImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_porcentaje"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_porcentaje"" value=""" + concepto.GetISRTasa() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" value=""" + concepto.GetISRImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_porcentaje"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_porcentaje"" value=""" + concepto.GetIEPSTasa() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" value=""" + concepto.GetIEPSImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" value=""" + concepto.GetTotalRetenciones() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" value=""" + concepto.GetTotalImporteConTransladosyRetenciones() + @""">
                                                <td colspan=""1""></td>
                                                <td colspan=""1""></td>
                                                <td nowrap>{12}</td>
                                                <td nowrap>{13}</td>
                                                <td>{0}</td> 
                                                <td>{14}</td> 
                                                <td style=""text-align:right"">{1}</td> 
                                                <td nowrap style=""text-align:right"" >{2}</td> 
                                                <td nowrap style=""text-align:right"">{3}</td> 
                                                <td nowrap style=""text-align:right"">{4}</td> 
                                                <td nowrap style=""text-align:right"">{5}</td> 
                                                <td nowrap style=""text-align:right"">{6}</td> 
                                                <td nowrap style=""text-align:right"">{7}</td> 
                                                <td nowrap style=""text-align:right"">{8}</td> 
                                                <td nowrap style=""text-align:right"">{9}</td>   
                                                <td nowrap style=""color:#C10000;text-align:right"">{10}</td> 
                                                <td></td>
                                                <td nowrap style=""text-align:right"">{11}</td> 
                                                <td colspan=""3""></td>
                                            </tr>
                            ", UsoStrings.RecortaString(concepto.Descripcion, 350), concepto.Cantidad.ToString("0.00"), concepto.ValorUnitario.ToString("$ #,##0.00"), concepto.Importe.ToString("$ #,##0.00")
                        , concepto.GetIVATasa() == 0 ? "--" : (concepto.GetIVATasa()).ToString("0.00 %"), concepto.GetIVAImporte() == 0 ? "--" : concepto.GetIVAImporte().ToString("$ #,##0.00")
                        , concepto.GetISRTasa() == 0 ? "--" : (concepto.GetISRTasa()).ToString("0.00 %"), concepto.GetISRImporte() == 0 ? "--" : concepto.GetISRImporte().ToString("$ #,##0.00")
                        , concepto.GetIEPSTasa() == 0 ? "--" : (concepto.GetIEPSTasa()).ToString("0.00 %"), concepto.GetIEPSImporte() == 0 ? "--" : concepto.GetIEPSImporte().ToString("$ #,##0.00")
                        //, concepto.GetTotalImpuestos() == 0 ? "--" : concepto.GetTotalImpuestos().ToString("$ #,##0.00")
                        , concepto.GetTotalRetenciones() == 0 ? "--" : concepto.GetTotalRetenciones().ToString("$ #,##0.00")
                        , concepto.GetTotalImporteConTransladosyRetenciones().ToString("$ #,##0.00")
                        , selectCuenta.Replace("#ID", numConcepto.ToString())
                        , selectCentroCosto.Replace("#ID", numConcepto.ToString())
                        , selectTipoPago.Replace("#ID", numConcepto.ToString())
                        );

                    //aumenta en 1 el número de concepto (por cada concepto dentro de la factura)
                    numConcepto++;
                }

                //agrega los TOTALES de la factura
                body += String.Format(@"<tr style=""background-color:#AAE0FF"" class=""div_" + factura_4_0.TimbreFiscalDigital.UUID + @""">
                                                <input type=""hidden"" name=""GV_comprobacion_rel_gastos.Index"" id=""GV_comprobacion_rel_gastos.Index"" value=""" + numConcepto + @""" />
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" value=""" + (existeEnCOFIDI ? GV_comprobacion_origen.COFIDI_TOTALES : GV_comprobacion_origen.XML_TOTALES) + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" value=""" + factura_4_0.SubTotal + @""">                                                
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" value=""" + factura_4_0.GetTotalIVAImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" value=""" + factura_4_0.GetTotalISRImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" value=""" + factura_4_0.GetTotalIEPSImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" value=""" + (factura_4_0.Impuestos != null ? factura_4_0.Impuestos.TotalImpuestosRetenidos : 0) + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].impuestos_locales"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].impuestos_locales"" value=""" + impuestosLocales + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" value=""" + factura_4_0.Total + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].fecha_factura"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].fecha_factura"" value=""" + factura_4_0.Fecha + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" value=""" + factura_4_0.TimbreFiscalDigital.UUID + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].currency_iso"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].currency_iso"" value=""" + factura_4_0.Moneda + @""">
                                                <td></td> 
                                                <td colspan=""7"" style=""text-align:right""><b>Totales Factura:</b></td> 
                                                <td nowrap style=""text-align:right"">{0}</td> 
                                                <td></td> 
                                                <td nowrap style=""text-align:right"">{1}</td> 
                                                <td></td> 
                                                <td nowrap style=""text-align:right"">{2}</td> 
                                                <td></td> 
                                                <td nowrap style=""text-align:right"">{3}</td>      
                                                <td nowrap style=""color:#C10000;text-align:right"">{4}</td> 
                                                <td nowrap style=""text-align:right"">{5}</td>
                                                <td nowrap style=""text-align:right"">{6}</td> 
                                                <td nowrap class=""item-total-mxn"" style=""text-align:right; font-weight:bold;"">{7}</td>
                                                <td colspan=""2""></td>
                                            </tr>
                    "
                , factura_4_0.SubTotal.ToString("$ #,##0.00")
                , factura_4_0.GetTotalIVAImporte() == 0 ? "--" : factura_4_0.GetTotalIVAImporte().ToString("$ #,##0.00")
                , factura_4_0.GetTotalISRImporte() == 0 ? "--" : factura_4_0.GetTotalISRImporte().ToString("$ #,##0.00")
                , factura_4_0.GetTotalIEPSImporte() == 0 ? "--" : factura_4_0.GetTotalIEPSImporte().ToString("$ #,##0.00")
                //, factura_4_0.Impuestos.TotalImpuestosTrasladados == 0 ? "--" : factura_4_0.Impuestos.TotalImpuestosTrasladados.ToString("$ #,##0.00")
                , (factura_4_0.Impuestos == null || factura_4_0.Impuestos.TotalImpuestosRetenidos == 0) ? "--" : factura_4_0.Impuestos.TotalImpuestosRetenidos.ToString("$ #,##0.00")
                , factura_4_0.ImpuestosLocales == null ? "--" : factura_4_0.ImpuestosLocales.TotaldeTraslados.ToString("$ #,##0.00")
                , factura_4_0.Total.ToString("$ #,##0.00")
                , factura_4_0.Total.ToString("$ #,##0.00")
                );

                //aumenta en 1 el número de concepto (por cada concepto dentro de la factura)
                numConcepto++;

                //indica el número de conceptos de la factura + la cabecera
                cantidadConceptos = factura_4_0.Conceptos.Count() + 2;
            }
            result[0] = new { status = estatus, value = body, num_conceptos = cantidadConceptos, uuid = uuid, msj = msj, div_ocultos = div_ocultos };
            return result;
        }

        [NonAction]
        public biblioteca_digital ValidaArchivo(HttpPostedFileBase archivo, float megas, List<string> extensionesPermitidas, ref string msj)
        {
            int mega = 1048576;

            biblioteca_digital biblioteca = null;

            //valida archivo
            if (archivo == null)
            {
                msj = "El archivo está vacío. Archivo: " + archivo.FileName;
                return null;
            }
            //valida peso
            if (archivo.InputStream.Length > (int)(mega * megas))
            {
                msj = "Sólo se permiten archivos menores a " + megas + "MB. Archivo: " + archivo.FileName;
                return null;
            }

            //verifica la extensión del archivo
            string extension = Path.GetExtension(archivo.FileName);
            String nombreArchivo = archivo.FileName;

            if (!extensionesPermitidas.Contains(extension.ToUpper()) && !extensionesPermitidas.Contains(extension.ToLower()))
            {  //si no contiene una extensión válida    
                msj = "Sólo se permiten archivos con extensión: " + String.Join(", ", extensionesPermitidas.ToArray()) + ". Error en Archivo: " + archivo.FileName;
                return null;
            }

            //recorta el nombre del archivo en caso de ser necesario
            if (nombreArchivo.Length > 80)
                nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

            //lee el archivo en un arreglo de bytes
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(archivo.InputStream))
            {
                fileData = binaryReader.ReadBytes(archivo.ContentLength);
            }

            //genera el archivo de biblioce digital
            biblioteca = new biblioteca_digital
            {
                Nombre = nombreArchivo,
                MimeType = UsoStrings.RecortaString(archivo.ContentType, 80),
                Datos = fileData
            };

            //relaciona el archivo con la poliza (despues se guarda en BD)   
            return biblioteca;

        }

    }
}
