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
using Clases.Util;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNet.Identity;
using Portal_2_0.Models;
using Portal_2_0.Models.PDFHandlers;





namespace Portal_2_0.Controllers
{
    [Authorize]
    public class RU_registrosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: RU_registros
        public ActionResult Index(string folio, int? id_planta, string estatus, string carga, string linea, string placas, string fecha_inicial, string fecha_final, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.RU_VIGILANCIA)
                && !TieneRol(TipoRoles.RU_ALMACEN_LIBERACION)
                && !TieneRol(TipoRoles.RU_ALMACEN_RECEPCION))
                return View("../Home/ErrorPermisos");

            #region valida planta asignada 

            //valida que el usuario esté asignado al menos a una planta
            string userId = User.Identity.GetUserId();
            var usuario = db.AspNetUsers.Find(userId);

            //si el usuario no está asignado a la planta de búsqueda
            if (!usuario.RU_rel_usuarios_planta.Any())
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede mostrar la página!";
                ViewBag.Descripcion = "El usuario no se encuentra asignado a ninguna Planta. Contácta a sistemas para solicitar la asignación a la planta correspondiente.";
                return View("../Home/ErrorGenerico");
            }

            //carga la primera planta que tenga por defecto
            if (id_planta == null)
            {
                int p = usuario.RU_rel_usuarios_planta.FirstOrDefault().id_planta;
                return RedirectToAction("Index", new { id_planta = p });
            }

            //valida que se haya seleccionado una planta
            var planta = db.plantas.Find(id_planta);
            //valida a qué planta está signado del empleado           
            //si el usuario no está asignado a la planta de búsqueda
            if (!usuario.RU_rel_usuarios_planta.Any(x => x.id_planta == id_planta) && planta != null)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede mostrar la página!";
                ViewBag.Descripcion = "El usuario no se encuentra asignado a la planta " + planta.descripcion + ". Contácta a sistemas para que se asigne a la planta deseada.";
                return View("../Home/ErrorGenerico");
            }

            #endregion

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            //tranforma los valores de las fechas
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


            var listado = db.RU_registros
                .Where(x => (string.IsNullOrEmpty(folio) || x.folio.Contains(folio))
                && (string.IsNullOrEmpty(estatus) || (estatus == "TERMINADO" && x.hora_vigilancia_salida.HasValue)
                    || (estatus == "CANCELADO" && x.hora_cancelacion.HasValue)
                    || (estatus == "EN_TRANSITO" && !x.hora_cancelacion.HasValue && !x.hora_vigilancia_salida.HasValue)
                )
                && (string.IsNullOrEmpty(carga) || (carga == "CARGA" && x.carga) || (carga == "DESCARGA" && x.descarga))
                && (string.IsNullOrEmpty(linea) || x.linea_transporte.Contains(linea))
                && (string.IsNullOrEmpty(placas) || x.placas_tractor.Contains(placas))
                && x.fecha_ingreso_vigilancia >= dateInicial && x.fecha_ingreso_vigilancia <= dateFinal
                && x.id_planta == id_planta
                && x.activo
                )
                .OrderByDescending(x => x.id)
                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.RU_registros
                  .Where(x => (string.IsNullOrEmpty(folio) || x.folio.Contains(folio))
                && (string.IsNullOrEmpty(estatus) || (estatus == "TERMINADO" && x.hora_vigilancia_salida.HasValue)
                    || (estatus == "CANCELADO" && x.hora_cancelacion.HasValue)
                    || (estatus == "EN_TRANSITO" && !x.hora_cancelacion.HasValue && !x.hora_vigilancia_salida.HasValue)
                )
                && (string.IsNullOrEmpty(carga) || (carga == "CARGA" && x.carga) || (carga == "DESCARGA" && x.descarga))
                && (string.IsNullOrEmpty(linea) || x.linea_transporte.Contains(linea))
                && (string.IsNullOrEmpty(placas) || x.placas_tractor.Contains(placas))
                && x.fecha_ingreso_vigilancia >= dateInicial && x.fecha_ingreso_vigilancia <= dateFinal
                       && x.id_planta == id_planta
                && x.activo
                )
                .Count();

            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["folio"] = folio;
            routeValues["id_planta"] = id_planta;
            routeValues["linea"] = linea;
            routeValues["placas"] = placas;
            routeValues["fecha_inicial"] = fecha_inicial;
            routeValues["fecha_final"] = fecha_final;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.Paginacion = paginacion;

            //list para estatus
            List<SelectListItem> newList = new List<SelectListItem>{
                new SelectListItem(){Text = "En Transito",Value = "EN_TRANSITO"},
                new SelectListItem(){Text = "Terminado",Value = "TERMINADO"},
                new SelectListItem(){Text = "Cancelado",Value = "CANCELADO"}};
            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Todos --");

            //list para carga descarga
            List<SelectListItem> newListCarga = new List<SelectListItem>{
                new SelectListItem(){Text = "Carga",Value = "CARGA"},
                new SelectListItem(){Text = "Descarga",Value = "DESCARGA"}};
            SelectList selectListItemsCarga = new SelectList(newListCarga, "Value", "Text", carga);
            ViewBag.carga = AddFirstItem(selectListItemsCarga, textoPorDefecto: "-- Cualquiera --");

            var plantasSelect = usuario.RU_rel_usuarios_planta.Select(x => x.plantas);
            ViewBag.id_planta = new SelectList(plantasSelect, "clave", "descripcion");


            return View(listado);
        }

        // GET: RU_registros/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.RU_VIGILANCIA)
              && !TieneRol(TipoRoles.RU_ALMACEN_LIBERACION)
              && !TieneRol(TipoRoles.RU_ALMACEN_RECEPCION))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RU_registros rU_registros = db.RU_registros.Find(id);
            if (rU_registros == null)
            {
                return HttpNotFound();
            }
            return View(rU_registros);
        }

        // GET: RU_registros/Create
        public ActionResult Create(int? id_planta)
        {
            if (!TieneRol(TipoRoles.RU_VIGILANCIA))
                return View("../Home/ErrorPermisos");

            //si no se envio una planta en la solicitud
            if (id_planta == null)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede mostrar la página!";
                ViewBag.Descripcion = "No se encontró una planta en la solicitud. Intenta ingresar nuevamente desde el menú principal.";
                return View("../Home/ErrorGenerico");
            }

            RU_registros model = new RU_registros
            {
                id_planta = id_planta.Value
            };

            ViewBag.id_vigilancia_ingreso = AddFirstItem(new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true && x.id_planta == id_planta), nameof(RU_usuarios_vigilancia.id), nameof(RU_usuarios_vigilancia.nombre)));

            //si solo hay una opcion manda por defecto la entrada
            if (db.RU_accesos.Where(x => x.activo == true && x.id_planta == id_planta).Count() == 1)
                ViewBag.id_acceso = new SelectList(db.RU_accesos.Where(x => x.activo == true && x.id_planta == id_planta), nameof(RU_accesos.id), nameof(RU_accesos.descripcion));
            else
                //si hay mas de una entrada agrega la opcion "Seleccionar"
                ViewBag.id_acceso = AddFirstItem(new SelectList(db.RU_accesos.Where(x => x.activo == true && x.id_planta == id_planta), nameof(RU_accesos.id), nameof(RU_accesos.descripcion)));

            return View(model);
        }

        // POST: RU_registros/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RU_registros rU_registros)
        {
            //valida si carga o descarga esta seleccionado
            if (!rU_registros.carga && !rU_registros.descarga)
                ModelState.AddModelError("", "Seleccione si la unidad ingresa a carga o descarga de materiales.");

            if (ModelState.IsValid)
            {

                rU_registros.fecha_ingreso_vigilancia = DateTime.Now;
                rU_registros.activo = true;
                rU_registros.folio = string.Empty; // evita error de folio obligatorio

                db.RU_registros.Add(rU_registros);

                ViewBag.Message = "El registro se ha creado correctamente.";
                ViewBag.Tipo = TipoMensajesSweetAlerts.SUCCESS;
                try
                {
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert("Se creó el registro correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                }
                catch (Exception ex)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ocurrió un error al guardar: " + ex.Message, TipoMensajesSweetAlerts.ERROR);

                }
                return View("Message");
            }

            ViewBag.id_vigilancia_ingreso = AddFirstItem(new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta), nameof(RU_usuarios_vigilancia.id), nameof(RU_usuarios_vigilancia.nombre)));
            //si solo hay una opcion manda por defecto la entrada
            if (db.RU_accesos.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta).Count() == 1)
                ViewBag.id_acceso = new SelectList(db.RU_accesos.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta), nameof(RU_accesos.id), nameof(RU_accesos.descripcion));
            else
                //si hay mas de una entrada agrega la opcion "Seleccionar"
                ViewBag.id_acceso = AddFirstItem(new SelectList(db.RU_accesos.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta), nameof(RU_accesos.id), nameof(RU_accesos.descripcion)));

            return View(rU_registros);
        }

        // GET: RU_registros/confirmarRecepcion/5
        public ActionResult confirmarRecepcion(int? id)
        {
            if (!TieneRol(TipoRoles.RU_ALMACEN_RECEPCION))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RU_registros rU_registros = db.RU_registros.Find(id);
            if (rU_registros == null)
            {
                return HttpNotFound();
            }

            //determina si hay un empleado logeado o un usuario genérico
            var empleado = obtieneEmpleadoLogeado();

            if (empleado.id == 0) // si no hay usuarios logeados obtiene la lista generica
            {
                var listUsuarios = db.RU_usuarios_embarques.Where(x => x.recibe && x.activo && x.id_planta == rU_registros.id_planta).Select(x => x.empleados);
                ViewBag.id_embarques_recepcion = AddFirstItem(new SelectList(listUsuarios, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)));
            }
            else {//si hay usuario logeado, obtiene su propio nombre
                ViewBag.id_embarques_recepcion = AddFirstItem(new SelectList(db.empleados.Where(x => x.id == empleado.id), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),selected:empleado.id.ToString());
                rU_registros.id_embarques_recepcion = empleado.id;
            }

            return View(rU_registros);
        }

        // POST: RU_registros/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult confirmarRecepcion(RU_registros rU_registros)
        {
            if (ModelState.IsValid)
            {
                RU_registros registroBD = db.RU_registros.Find(rU_registros.id);
                //aplica los cambios del formulario 
                registroBD.hora_embarques_recepcion = DateTime.Now;
                registroBD.id_embarques_recepcion = rU_registros.id_embarques_recepcion;
                registroBD.comentarios_embarques_recepcion = rU_registros.comentarios_embarques_recepcion;

                db.SaveChanges();
                ViewBag.Message = "Se confirmó la recepción de la unidad en embarques.";
                ViewBag.Tipo = TipoMensajesSweetAlerts.SUCCESS;
                TempData["Mensaje"] = new MensajesSweetAlert("Se confirmó la recepción de la unidad en embarques.", TipoMensajesSweetAlerts.SUCCESS);
                return View("Message");
            }
            //determina si hay un empleado logeado o un usuario genérico
            var empleado = obtieneEmpleadoLogeado();

            if (empleado.id == 0) // si no hay usuarios logeados obtiene la lista generica
            {
                var listUsuarios = db.RU_usuarios_embarques.Where(x => x.recibe && x.activo && x.id_planta == rU_registros.id_planta).Select(x => x.empleados);
                ViewBag.id_embarques_recepcion = AddFirstItem(new SelectList(listUsuarios, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),selected: rU_registros.id_embarques_recepcion.ToString());
            }
            else
            {//si hay usuario logeado, obtiene su propio nombre
                ViewBag.id_embarques_recepcion = AddFirstItem(new SelectList(db.empleados.Where(x => x.id == empleado.id), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), selected: rU_registros.id_embarques_recepcion.ToString());
                rU_registros.id_embarques_recepcion = empleado.id;
            }

            return View(rU_registros);
        }

        // GET: RU_registros/confirmarLiberacionEmbarques/5
        public ActionResult confirmarLiberacionEmbarques(int? id)
        {
            if (!TieneRol(TipoRoles.RU_ALMACEN_LIBERACION))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RU_registros rU_registros = db.RU_registros.Find(id);
            if (rU_registros == null)
            {
                return HttpNotFound();
            }

            //determina si hay un empleado logeado o un usuario genérico
            var empleado = obtieneEmpleadoLogeado();

            if (empleado.id == 0) // si no hay usuarios logeados obtiene la lista generica
            {
                var listUsuarios = db.RU_usuarios_embarques.Where(x => x.libera && x.activo && x.id_planta == rU_registros.id_planta).Select(x => x.empleados);
                ViewBag.id_embarques_liberacion = AddFirstItem(new SelectList(listUsuarios, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)));
            }
            else
            {//si hay usuario logeado, obtiene su propio nombre
                ViewBag.id_embarques_liberacion = AddFirstItem(new SelectList(db.empleados.Where(x => x.id == empleado.id), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), selected: empleado.id.ToString());
                rU_registros.id_embarques_liberacion = empleado.id;
            }

            return View(rU_registros);
        }

        // POST: RU_registros/confirmarLiberacionEmbarques/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult confirmarLiberacionEmbarques(RU_registros rU_registros)
        {
            if (ModelState.IsValid)
            {
                RU_registros registroBD = db.RU_registros.Find(rU_registros.id);
                //aplica los cambios del formulario 
                registroBD.hora_embarques_liberacion = DateTime.Now;
                registroBD.id_embarques_liberacion = rU_registros.id_embarques_liberacion;
                registroBD.comentarios_embarques_liberacion = rU_registros.comentarios_embarques_liberacion;

                //en caso de que sea solamente descarga...
                if (registroBD.descarga && !registroBD.carga)
                {
                    registroBD.hora_vigilancia_liberacion = DateTime.Now;
                    registroBD.comentarios_vigilancia_liberacion = "No aplica";
                }
                db.SaveChanges();
                ViewBag.Message = "Se liberó la unidad de Embarques.";
                ViewBag.Tipo = TipoMensajesSweetAlerts.SUCCESS;
                TempData["Mensaje"] = new MensajesSweetAlert("Se liberó la unidad de Embarques.", TipoMensajesSweetAlerts.SUCCESS);
                return View("Message");
            }

            //determina si hay un empleado logeado o un usuario genérico
            var empleado = obtieneEmpleadoLogeado();

            if (empleado.id == 0) // si no hay usuarios logeados obtiene la lista generica
            {
                var listUsuarios = db.RU_usuarios_embarques.Where(x => x.libera && x.activo && x.id_planta == rU_registros.id_planta).Select(x => x.empleados);
                ViewBag.id_embarques_liberacion = AddFirstItem(new SelectList(listUsuarios, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), selected: rU_registros.id_embarques_liberacion.ToString());
            }
            else
            {//si hay usuario logeado, obtiene su propio nombre
                ViewBag.id_embarques_liberacion = AddFirstItem(new SelectList(db.empleados.Where(x => x.id == empleado.id), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), selected: rU_registros.id_embarques_liberacion.ToString());
                rU_registros.id_embarques_liberacion = empleado.id;
            }

           return View(rU_registros);
        }
        // GET: RU_registros/confirmarLiberacionVigilancia/5
        public ActionResult confirmarLiberacionVigilancia(int? id)
        {
            if (!TieneRol(TipoRoles.RU_VIGILANCIA))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RU_registros rU_registros = db.RU_registros.Find(id);
            if (rU_registros == null)
            {
                return HttpNotFound();
            }

            ViewBag.id_vigilancia_liberacion = AddFirstItem(new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta), nameof(RU_usuarios_vigilancia.id), nameof(RU_usuarios_vigilancia.nombre)));

            return View(rU_registros);
        }

        // POST: RU_registros/confirmarLiberacionVigilancia/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult confirmarLiberacionVigilancia(RU_registros rU_registros)
        {
            if (ModelState.IsValid)
            {
                RU_registros registroBD = db.RU_registros.Find(rU_registros.id);
                //aplica los cambios del formulario 
                registroBD.hora_vigilancia_liberacion = DateTime.Now;
                registroBD.id_vigilancia_liberacion = rU_registros.id_vigilancia_liberacion;
                registroBD.comentarios_vigilancia_liberacion = rU_registros.comentarios_vigilancia_liberacion;

                db.SaveChanges();
                ViewBag.Message = "Se liberó la unidad  (Vigilancia).";
                ViewBag.Tipo = TipoMensajesSweetAlerts.SUCCESS;
                TempData["Mensaje"] = new MensajesSweetAlert("Se liberó la unidad (Vigilancia).", TipoMensajesSweetAlerts.SUCCESS);
                return View("Message");
            }
            ViewBag.id_vigilancia_liberacion = AddFirstItem(new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta), nameof(RU_usuarios_vigilancia.id), nameof(RU_usuarios_vigilancia.nombre)), selected: rU_registros.id_vigilancia_liberacion.ToString());
            return View(rU_registros);
        }

        // GET: RU_registros/confirmarSalidaVigilancia/5
        public ActionResult confirmarSalidaVigilancia(int? id)
        {
            if (!TieneRol(TipoRoles.RU_VIGILANCIA))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RU_registros rU_registros = db.RU_registros.Find(id);
            if (rU_registros == null)
            {
                return HttpNotFound();
            }

            ViewBag.id_vigilancia_salida = AddFirstItem(new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta), nameof(RU_usuarios_vigilancia.id), nameof(RU_usuarios_vigilancia.nombre)));

            //si solo hay una opcion manda por defecto la entrada
            if (db.RU_accesos.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta).Count() == 1)
                ViewBag.id_salida = new SelectList(db.RU_accesos.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta), nameof(RU_accesos.id), nameof(RU_accesos.descripcion));
            else
                //si hay mas de una entrada agrega la opcion "Seleccionar"
                ViewBag.id_salida = AddFirstItem(new SelectList(db.RU_accesos.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta), nameof(RU_accesos.id), nameof(RU_accesos.descripcion)));


            return View(rU_registros);
        }

        // POST: RU_registros/confirmarSalidaVigilancia/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult confirmarSalidaVigilancia(RU_registros rU_registros)
        {
            if (ModelState.IsValid)
            {
                RU_registros registroBD = db.RU_registros.Find(rU_registros.id);
                //aplica los cambios del formulario 
                registroBD.hora_vigilancia_salida = DateTime.Now;
                registroBD.id_salida = rU_registros.id_salida;
                registroBD.id_vigilancia_salida = rU_registros.id_vigilancia_salida;
                registroBD.comentarios_vigilancia_salida = rU_registros.comentarios_vigilancia_salida;

                db.SaveChanges();
                ViewBag.Message = "Se registró la salida de planta de la unidad.";
                ViewBag.Tipo = TipoMensajesSweetAlerts.SUCCESS;
                TempData["Mensaje"] = new MensajesSweetAlert("Se registró la salida de planta de la unidad.", TipoMensajesSweetAlerts.SUCCESS);
                return View("Message");
            }
            ViewBag.id_vigilancia_salida = AddFirstItem(new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta), nameof(RU_usuarios_vigilancia.id), nameof(RU_usuarios_vigilancia.nombre)), selected: rU_registros.id_vigilancia_salida.ToString());
            if (db.RU_accesos.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta).Count() == 1)
                ViewBag.id_salida = new SelectList(db.RU_accesos.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta), nameof(RU_accesos.id), nameof(RU_accesos.descripcion));
            else
                //si hay mas de una entrada agrega la opcion "Seleccionar"
                ViewBag.id_salida = AddFirstItem(new SelectList(db.RU_accesos.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta), nameof(RU_accesos.id), nameof(RU_accesos.descripcion)));


            return View(rU_registros);
        }

        // GET: RU_registros/Cancel/5
        public ActionResult Cancel(int? id)
        {
            if (!TieneRol(TipoRoles.RU_VIGILANCIA) && !TieneRol(TipoRoles.RU_ALMACEN_RECEPCION))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RU_registros rU_registros = db.RU_registros.Find(id);
            if (rU_registros == null)
            {
                return HttpNotFound();
            }

            //agrega el nombre de cancelacion
            var vigilanciaList = db.RU_usuarios_vigilancia.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta);
            var embarquesList = db.RU_usuarios_embarques.Where(x => x.recibe && x.activo && x.id_planta == rU_registros.id_planta);
            //crea un Select  list para las opciones
            List<SelectListItem> newList = new List<SelectListItem>();
            foreach (var vigilancia in vigilanciaList)
                newList.Add(new SelectListItem()
                {
                    Text = "Vigilancia: " + vigilancia.nombre,
                    Value = vigilancia.nombre
                });
            foreach (var embarques in embarquesList)
                newList.Add(new SelectListItem()
                {
                    Text = "Embarques: " + embarques.empleados.ConcatNombre,
                    Value = embarques.empleados.ConcatNombre
                });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text");

            ViewBag.nombre_cancelacion = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Todos --");

            return View(rU_registros);
        }

        // POST: RU_registros/confirmarSalidaVigilancia/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(RU_registros rU_registros)
        {
            if (ModelState.IsValid)
            {
                RU_registros registroBD = db.RU_registros.Find(rU_registros.id);
                //aplica los cambios del formulario 
                registroBD.hora_cancelacion = DateTime.Now;
                registroBD.nombre_cancelacion = rU_registros.nombre_cancelacion;
                registroBD.comentario_cancelacion = rU_registros.comentario_cancelacion;

                db.SaveChanges();
                ViewBag.Message = "Se canceló el registro de la unidad.";
                ViewBag.Tipo = TipoMensajesSweetAlerts.SUCCESS;
                TempData["Mensaje"] = new MensajesSweetAlert("Se canceló el registro la unidad.", TipoMensajesSweetAlerts.SUCCESS);
                return View("Message");
            }
            //agrega el nombre de cancelacion
            var vigilanciaList = new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true && x.id_planta == rU_registros.id_planta), nameof(RU_usuarios_vigilancia.nombre), nameof(RU_usuarios_vigilancia.nombre));
            var embarquesList = new SelectList(db.RU_usuarios_embarques.Where(x => x.recibe && x.activo && x.id_planta == rU_registros.id_planta).Select(x => x.empleados), nameof(empleados.ConcatNombre), nameof(empleados.ConcatNumEmpleadoNombre));
            var joinList = new SelectList(vigilanciaList.Concat(embarquesList));
            ViewBag.nombre_cancelacion = AddFirstItem(joinList, textoPorDefecto: "-- Todos --");
            return View(rU_registros);
        }

        public ActionResult Exportar(string folio, string estatus, string carga, string linea, string placas, string fecha_inicial, string fecha_final, int? id_planta)
        {
            if (!TieneRol(TipoRoles.RU_VIGILANCIA)
                && !TieneRol(TipoRoles.RU_ALMACEN_LIBERACION)
                && !TieneRol(TipoRoles.RU_ALMACEN_RECEPCION))
                return View("../Home/ErrorPermisos");

            //convierte las fechas recibidas
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

            var listado = db.RU_registros
                .Where(x => (string.IsNullOrEmpty(folio) || x.folio.Contains(folio))
                && (string.IsNullOrEmpty(estatus) || (estatus == "TERMINADO" && x.hora_vigilancia_salida.HasValue)
                    || (estatus == "CANCELADO" && x.hora_cancelacion.HasValue)
                    || (estatus == "EN_TRANSITO" && !x.hora_cancelacion.HasValue && !x.hora_vigilancia_salida.HasValue)
                )
                && (string.IsNullOrEmpty(carga) || (carga == "CARGA" && x.carga) || (carga == "DESCARGA" && x.descarga))
                && (string.IsNullOrEmpty(linea) || x.linea_transporte.Contains(linea))
                && (string.IsNullOrEmpty(placas) || x.placas_tractor.Contains(placas))
                && x.fecha_ingreso_vigilancia >= dateInicial && x.fecha_ingreso_vigilancia <= dateFinal
                && x.id_planta == id_planta
                && x.activo
                )
                .OrderByDescending(x => x.id)
                   .ToList();

            byte[] stream = ExcelUtil.GeneraReporteRU(listado);


            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = "Reporte_Registro_Unidades_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(stream, "application/vnd.ms-excel");

        }


        public ActionResult GenerarPDF(int? id, bool inline = true)
        {
            if (id == null)
                return View("../Error/BadRequest");

            // Helpers para sanitizar nulos
            Func<string, string> S = s => string.IsNullOrWhiteSpace(s) ? "--" : s;
            Func<DateTime?, string> D = d => d.HasValue ? d.Value.ToString("g") : "--";

            // 1) Carga el registro
            var item = db.RU_registros
                .Include(r => r.plantas)
                .Include(r => r.RU_accesos)
                .Include(r => r.RU_accesos1)
                .Include(r => r.RU_usuarios_vigilancia)
                .Include(r => r.RU_usuarios_vigilancia1)
                .Include(r => r.RU_usuarios_vigilancia2)
                .Include(r => r.empleados)
                .Include(r => r.empleados1)
                .FirstOrDefault(r => r.id == id.Value);
            if (item == null)
                return View("../Error/NotFound");

            byte[] pdfBytes;
            using (var ms = new MemoryStream())
            {
                using (var writer = new PdfWriter(ms))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var doc = new Document(pdf, PageSize.LETTER);
                        try
                        {
                            // 2) Márgenes: top aumentado a 100 para no solapar el header
                            doc.SetMargins(100, 35, 60, 35);

                            // 3) Fuente y logo
                            var font = PdfFontFactory.CreateFont(Server.MapPath("/fonts/tkmm/TKTypeMedium.ttf"));
                            var imgD = ImageDataFactory.Create(Server.MapPath("/Content/images/logo_1.png"));
                            var logo = new Image(imgD)
                                            .ScaleToFit(80, 80)
                                            .SetFixedPosition(36, PageSize.LETTER.GetTop() - 80);

                            // 4) Header / Footer
                            pdf.AddEventHandler(PdfDocumentEvent.START_PAGE,
                                new HeaderEventHandlerRU(logo, font, "Registro de Vigilancia"));
                            pdf.AddEventHandler(PdfDocumentEvent.END_PAGE,
                                new FooterEventHandlerRU(font));

                            // 5) Estilos
                            var sectionStyle = new Style()
                                .SetFont(font).SetFontSize(12).SetBold()
                                .SetFontColor(new DeviceRgb(0, 80, 159))
                                .SetBackgroundColor(new DeviceRgb(230, 230, 230))
                                .SetPadding(4).SetMarginTop(10).SetMarginBottom(4);

                            var labelStyle = new Style()
                                .SetFont(font).SetBold().SetFontSize(9)
                                .SetFontColor(new DeviceRgb(0, 80, 159))
                                .SetBorder(Border.NO_BORDER);

                            var valueStyle = new Style()
                                .SetFont(font).SetFontSize(9)
                                .SetFontColor(DeviceGray.BLACK)
                                .SetBorder(Border.NO_BORDER);

                            // Helper para secciones
                            Action<string, Action<Table>> AddSection = (title, fill) =>
                            {
                                doc.Add(new Paragraph(title).AddStyle(sectionStyle));
                                var tbl = new Table(UnitValue.CreatePercentArray(new float[] { 20, 30, 20, 30 }))
                                    .UseAllAvailableWidth()
                                    .SetMarginBottom(8);
                                fill(tbl);
                                doc.Add(tbl);
                            };

                            // --- Datos Generales (4 columnas) ---
                            AddSection("Datos Generales", tbl =>
                            {
                                tbl.AddCell(CreateCell("Folio", labelStyle)); tbl.AddCell(CreateCell(S(item.folio), valueStyle));
                                tbl.AddCell(CreateCell("Estado", labelStyle)); tbl.AddCell(CreateCell(S(item.EstadoString), valueStyle));

                                tbl.AddCell(CreateCell("Ingreso (Vigilancia)", labelStyle));
                                tbl.AddCell(CreateCell(D(item.fecha_ingreso_vigilancia), valueStyle));
                                tbl.AddCell(CreateCell("Nombre Operador", labelStyle));
                                tbl.AddCell(CreateCell(S(item.nombre_operador), valueStyle));

                                tbl.AddCell(CreateCell("Línea Transporte", labelStyle));
                                tbl.AddCell(CreateCell(S(item.linea_transporte), valueStyle));
                                tbl.AddCell(CreateCell("Placas Tractor", labelStyle));
                                tbl.AddCell(CreateCell(S(item.placas_tractor), valueStyle));

                                tbl.AddCell(CreateCell("Placas Plataforma 1", labelStyle));
                                tbl.AddCell(CreateCell(S(item.placa_plataforma_uno), valueStyle));
                                tbl.AddCell(CreateCell("Placas Plataforma 2", labelStyle));
                                tbl.AddCell(CreateCell(S(item.placa_plataforma_dos), valueStyle));

                                tbl.AddCell(CreateCell("¿Carga?", labelStyle)); tbl.AddCell(CreateCell(item.carga ? "SÍ" : "NO", valueStyle));
                                tbl.AddCell(CreateCell("¿Descarga?", labelStyle)); tbl.AddCell(CreateCell(item.descarga ? "SÍ" : "NO", valueStyle));
                            });

                            // --- Vigilancia (4 columnas, oculta comentarios si no hay) ---
                            AddSection("Vigilancia", tbl =>
                            {
                                tbl.AddCell(CreateCell("Vigilante", labelStyle)); tbl.AddCell(CreateCell(S(item.RU_usuarios_vigilancia?.nombre), valueStyle));
                                tbl.AddCell(CreateCell("Acceso", labelStyle)); tbl.AddCell(CreateCell(S(item.RU_accesos.descripcion), valueStyle));

                                if (!string.IsNullOrWhiteSpace(item.comentarios_vigilancia_ingreso))
                                {
                                    tbl.AddCell(CreateCell("Comentarios", labelStyle));
                                    tbl.AddCell(CreateCell(S(item.comentarios_vigilancia_ingreso), valueStyle));
                                    // completamos dos celdas vacías para mantener 4-col
                                    tbl.AddCell(new Cell(1, 2).SetBorder(Border.NO_BORDER));
                                }
                            });

                            // --- Resto de secciones igual, pero si prefieres pueden seguir en 4-columas ---
                            if (item.hora_embarques_recepcion.HasValue)
                                AddSection("Registro de Oficina", tbl =>
                                {
                                    tbl.AddCell(CreateCell("Recepción", labelStyle)); tbl.AddCell(CreateCell(D(item.hora_embarques_recepcion), valueStyle));
                                    tbl.AddCell(CreateCell("Recibe", labelStyle)); tbl.AddCell(CreateCell(S(item.empleados1.ConcatNumEmpleadoNombre), valueStyle));
                                    if (!string.IsNullOrWhiteSpace(item.comentarios_embarques_recepcion))
                                    {
                                        tbl.AddCell(CreateCell("Comentarios", labelStyle));
                                        tbl.AddCell(CreateCell(S(item.comentarios_embarques_recepcion), valueStyle));
                                        tbl.AddCell(new Cell(1, 2).SetBorder(Border.NO_BORDER));
                                    }
                                });

                            if (item.hora_embarques_liberacion.HasValue)
                                AddSection("Liberación Embarques", tbl =>
                                {
                                    tbl.AddCell(CreateCell("Liberación", labelStyle)); tbl.AddCell(CreateCell(D(item.hora_embarques_liberacion), valueStyle));
                                    tbl.AddCell(CreateCell("Libera", labelStyle)); tbl.AddCell(CreateCell(S(item.empleados.ConcatNumEmpleadoNombre), valueStyle));
                                    if (!string.IsNullOrWhiteSpace(item.comentarios_embarques_liberacion))
                                    {
                                        tbl.AddCell(CreateCell("Comentarios", labelStyle));
                                        tbl.AddCell(CreateCell(S(item.comentarios_embarques_liberacion), valueStyle));
                                        tbl.AddCell(new Cell(1, 2).SetBorder(Border.NO_BORDER));
                                    }
                                });

                            if (item.hora_vigilancia_liberacion.HasValue)
                                AddSection("Liberación Vigilancia", tbl =>
                                {
                                    tbl.AddCell(CreateCell("Liberación", labelStyle)); tbl.AddCell(CreateCell(D(item.hora_vigilancia_liberacion), valueStyle));
                                    tbl.AddCell(CreateCell("Libera", labelStyle)); tbl.AddCell(CreateCell(S(item.RU_usuarios_vigilancia1?.nombre), valueStyle));
                                    if (!string.IsNullOrWhiteSpace(item.comentarios_vigilancia_liberacion))
                                    {
                                        tbl.AddCell(CreateCell("Comentarios", labelStyle));
                                        tbl.AddCell(CreateCell(S(item.comentarios_vigilancia_liberacion), valueStyle));
                                        tbl.AddCell(new Cell(1, 2).SetBorder(Border.NO_BORDER));
                                    }
                                });

                            if (item.hora_vigilancia_salida.HasValue)
                                AddSection("Salida de Planta", tbl =>
                                {
                                    tbl.AddCell(CreateCell("Salida", labelStyle)); tbl.AddCell(CreateCell(D(item.hora_vigilancia_salida), valueStyle));
                                    tbl.AddCell(CreateCell("Vigilante", labelStyle)); tbl.AddCell(CreateCell(S(item.RU_usuarios_vigilancia2?.nombre), valueStyle));
                                    tbl.AddCell(CreateCell("Acceso Salida", labelStyle)); tbl.AddCell(CreateCell(S(item.RU_accesos1.descripcion), valueStyle));
                                    if (!string.IsNullOrWhiteSpace(item.comentarios_vigilancia_salida))
                                    {
                                        tbl.AddCell(CreateCell("Comentarios", labelStyle));
                                        tbl.AddCell(CreateCell(S(item.comentarios_vigilancia_salida), valueStyle));
                                        tbl.AddCell(new Cell(1, 2).SetBorder(Border.NO_BORDER));
                                    }
                                });

                            if (item.hora_cancelacion.HasValue)
                                AddSection("Cancelación", tbl =>
                                {
                                    tbl.AddCell(CreateCell("Fecha Cancelación", labelStyle)); tbl.AddCell(CreateCell(D(item.hora_cancelacion), valueStyle));
                                    tbl.AddCell(CreateCell("Cancela", labelStyle)); tbl.AddCell(CreateCell(S(item.nombre_cancelacion), valueStyle));
                                    tbl.AddCell(CreateCell("Comentarios", labelStyle));
                                    tbl.AddCell(CreateCell(S(item.comentario_cancelacion), valueStyle));
                                });
                        }
                        finally
                        {
                            doc.Close();
                        }
                    }
                }

                pdfBytes = ms.ToArray();
            }

            // 6) Envío al cliente
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = $"Registro_{item.id}_{item.folio}.pdf",
                Inline = inline
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(pdfBytes, "application/pdf");
        }

        // Helper para crear celdas con padding uniforme
        private static Cell CreateCell(string text, Style style)
        {
            return new Cell().Add(new Paragraph(text))
                             .AddStyle(style)
                             .SetPadding(4);
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

    public class HeaderEventHandlerRU : IEventHandler
    {
        private readonly Image imgLogo;
        private readonly PdfFont font;
        private readonly string titulo;

        public HeaderEventHandlerRU(Image imgLogo, PdfFont font, string titulo)
        {
            this.imgLogo = imgLogo;
            this.font = font;
            this.titulo = titulo;
        }

        public void HandleEvent(Event currentEvent)
        {
            var evt = (PdfDocumentEvent)currentEvent;
            var page = evt.GetPage();
            var pageSize = page.GetPageSize();

            // Definimos un rectángulo en la zona superior de la página
            float headerHeight = 60;
            var area = new Rectangle(
                35,                          // x: margen izquierdo
                pageSize.GetTop() - headerHeight, // y: desde arriba
                pageSize.GetWidth() - 70,    // ancho: ancho total menos márgenes
                headerHeight                 // alto
            );

            // Creamos el layout Canvas sobre esa área
            var canvas = new Canvas(page, area);

            // Montamos una tabla 3 columnas: logo | título | fecha
            float[] colWidths = { 20f, 60f, 20f };
            var table = new Table(UnitValue.CreatePercentArray(colWidths))
                            .UseAllAvailableWidth()
                            .SetBorder(Border.NO_BORDER);

            // 1) Logo
            table.AddCell(new Cell()
                .Add(imgLogo.SetAutoScale(true))
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            // 2) Título centrado
            table.AddCell(new Cell()
                .Add(new Paragraph(titulo)
                    .SetFont(font)
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            // 3) Fecha a la derecha
            table.AddCell(new Cell()
                .Add(new Paragraph(DateTime.Now.ToString("dd/MM/yyyy"))
                    .SetFont(font)
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.RIGHT))
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            // Lo añadimos y cerramos
            canvas.Add(table);
            canvas.Close();
        }
    }


    public class FooterEventHandlerRU : IEventHandler
    {
        private readonly PdfFont font;

        public FooterEventHandlerRU(PdfFont font)
        {
            this.font = font;
        }

        public void HandleEvent(Event currentEvent)
        {
            var evt = (PdfDocumentEvent)currentEvent;
            var pdf = evt.GetDocument();
            var page = evt.GetPage();
            var pageSize = page.GetPageSize();

            // Área de pie de página (desde el fondo)
            float footerHeight = 50;
            var area = new Rectangle(
                35,
                pageSize.GetBottom(),
                pageSize.GetWidth() - 70,
                footerHeight
            );

            var canvas = new Canvas(page, area);

            // Tabla con 2 columnas: Página X de Y | Texto derecho
            float[] colWidths = { 50f, 50f };
            var table = new Table(UnitValue.CreatePercentArray(colWidths))
                            .UseAllAvailableWidth()
                            .SetBorder(Border.NO_BORDER);

            // Página X de Y
            table.AddCell(new Cell()
                .Add(new Paragraph($"Página {pdf.GetPageNumber(page)} de {pdf.GetNumberOfPages()}")
                    .SetFont(font)
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));

            // Un texto fijo o vacío (ajusta a tu necesidad)
            table.AddCell(new Cell()
                .Add(new Paragraph("thyssenkrupp")
                    .SetFont(font)
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.RIGHT))
                .SetBorder(Border.NO_BORDER));

            canvas.Add(table);
            canvas.Close();
        }
    }

}
