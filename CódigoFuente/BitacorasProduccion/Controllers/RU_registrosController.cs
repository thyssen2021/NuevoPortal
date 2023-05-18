using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class RU_registrosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: RU_registros
        public ActionResult Index(int? id, string estatus, string carga, string linea, string placas, string fecha_inicial, string fecha_final, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.RU_VIGILANCIA)
                && !TieneRol(TipoRoles.RU_ALMACEN_LIBERACION)
                && !TieneRol(TipoRoles.RU_ALMACEN_RECEPCION))
                return View("../Home/ErrorPermisos");

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
                .Where(x => (id == null || id == x.id)
                && (string.IsNullOrEmpty(estatus) || (estatus == "TERMINADO" && x.hora_vigilancia_salida.HasValue)
                    || (estatus == "CANCELADO" && x.hora_cancelacion.HasValue)
                    || (estatus == "EN_TRANSITO" && !x.hora_cancelacion.HasValue && !x.hora_vigilancia_salida.HasValue)
                )
                && (string.IsNullOrEmpty(carga) || (carga == "CARGA" && x.carga) || (carga == "DESCARGA" && x.descarga))
                && (string.IsNullOrEmpty(linea) || x.linea_transporte.Contains(linea))
                && (string.IsNullOrEmpty(placas) || x.placas_tractor.Contains(placas))
                && x.fecha_ingreso_vigilancia >= dateInicial && x.fecha_ingreso_vigilancia <= dateFinal
                && x.activo
                )
                .OrderByDescending(x => x.id)
                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.RU_registros
                  .Where(x => (id == null || id == x.id)
                && (string.IsNullOrEmpty(estatus) || (estatus == "TERMINADO" && x.hora_vigilancia_salida.HasValue)
                    || (estatus == "CANCELADO" && x.hora_cancelacion.HasValue)
                    || (estatus == "EN_TRANSITO" && !x.hora_cancelacion.HasValue && !x.hora_vigilancia_salida.HasValue)
                )
                && (string.IsNullOrEmpty(carga) || (carga == "CARGA" && x.carga) || (carga == "DESCARGA" && x.descarga))
                && (string.IsNullOrEmpty(linea) || x.linea_transporte.Contains(linea))
                && (string.IsNullOrEmpty(placas) || x.placas_tractor.Contains(placas))
                && x.fecha_ingreso_vigilancia >= dateInicial && x.fecha_ingreso_vigilancia <= dateFinal
                && x.activo
                )
                .Count();

            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id"] = id;
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
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.RU_VIGILANCIA))
                return View("../Home/ErrorPermisos");

            RU_registros model = new RU_registros
            {
                //fecha_ingreso_vigilancia = DateTime.Now
            };

            ViewBag.id_vigilancia_ingreso = AddFirstItem(new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true), nameof(RU_usuarios_vigilancia.id), nameof(RU_usuarios_vigilancia.nombre)));

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

                db.RU_registros.Add(rU_registros);

                ViewBag.Message = "El registro se ha creado correctamente.";
                ViewBag.Tipo = TipoMensajesSweetAlerts.SUCCESS;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se creó el registro correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                return View("Message");
            }

            ViewBag.id_vigilancia_ingreso = AddFirstItem(new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true), nameof(RU_usuarios_vigilancia.id), nameof(RU_usuarios_vigilancia.nombre)));
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

            var listUsuarios = db.RU_usuarios_embarques.Where(x => x.recibe && x.activo).Select(x => x.empleados);
            ViewBag.id_embarques_recepcion = AddFirstItem(new SelectList(listUsuarios, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)));

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
            var listUsuarios = db.RU_usuarios_embarques.Where(x => x.recibe && x.activo).Select(x => x.empleados);
            ViewBag.id_embarques_recepcion = AddFirstItem(new SelectList(listUsuarios, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), selected: rU_registros.id_embarques_recepcion.ToString());
            return View(rU_registros);
        }

        // GET: RU_registros/confirmarLiberacionEmbarques/5
        public ActionResult confirmarLiberacionEmbarques(int? id)
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

            var listUsuarios = db.RU_usuarios_embarques.Where(x => x.libera && x.activo).Select(x => x.empleados);
            ViewBag.id_embarques_liberacion = AddFirstItem(new SelectList(listUsuarios, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)));

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

                db.SaveChanges();
                ViewBag.Message = "Se liberó la unidad de Embarques.";
                ViewBag.Tipo = TipoMensajesSweetAlerts.SUCCESS;
                TempData["Mensaje"] = new MensajesSweetAlert("Se liberó la unidad de Embarques.", TipoMensajesSweetAlerts.SUCCESS);
                return View("Message");
            }
            var listUsuarios = db.RU_usuarios_embarques.Where(x => x.recibe && x.activo).Select(x => x.empleados);
            ViewBag.id_embarques_recepcion = AddFirstItem(new SelectList(listUsuarios, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), selected: rU_registros.id_embarques_recepcion.ToString());
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

            ViewBag.id_vigilancia_liberacion = AddFirstItem(new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true), nameof(RU_usuarios_vigilancia.id), nameof(RU_usuarios_vigilancia.nombre)));

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
            ViewBag.id_vigilancia_liberacion = AddFirstItem(new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true), nameof(RU_usuarios_vigilancia.id), nameof(RU_usuarios_vigilancia.nombre)), selected: rU_registros.id_vigilancia_liberacion.ToString());
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

            ViewBag.id_vigilancia_salida = AddFirstItem(new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true), nameof(RU_usuarios_vigilancia.id), nameof(RU_usuarios_vigilancia.nombre)));

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
                registroBD.id_vigilancia_salida = rU_registros.id_vigilancia_salida;
                registroBD.comentarios_vigilancia_salida = rU_registros.comentarios_vigilancia_salida;

                db.SaveChanges();
                ViewBag.Message = "Se registró la salida de planta de la unidad.";
                ViewBag.Tipo = TipoMensajesSweetAlerts.SUCCESS;
                TempData["Mensaje"] = new MensajesSweetAlert("Se registró la salida de planta de la unidad.", TipoMensajesSweetAlerts.SUCCESS);
                return View("Message");
            }
            ViewBag.id_vigilancia_salida = AddFirstItem(new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true), nameof(RU_usuarios_vigilancia.id), nameof(RU_usuarios_vigilancia.nombre)), selected: rU_registros.id_vigilancia_salida.ToString());
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
            var vigilanciaList = db.RU_usuarios_vigilancia.Where(x => x.activo == true);
            var embarquesList = db.RU_usuarios_embarques.Where(x => x.recibe && x.activo);
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
            var vigilanciaList = new SelectList(db.RU_usuarios_vigilancia.Where(x => x.activo == true), nameof(RU_usuarios_vigilancia.nombre), nameof(RU_usuarios_vigilancia.nombre));
            var embarquesList = new SelectList(db.RU_usuarios_embarques.Where(x => x.recibe && x.activo).Select(x => x.empleados), nameof(empleados.ConcatNombre), nameof(empleados.ConcatNumEmpleadoNombre));
            var joinList = new SelectList(vigilanciaList.Concat(embarquesList));
            ViewBag.nombre_cancelacion = AddFirstItem(joinList, textoPorDefecto: "-- Todos --");
            return View(rU_registros);
        }

        public ActionResult Exportar(int? id, string estatus, string carga, string linea, string placas, string fecha_inicial, string fecha_final)
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
                .Where(x => (id == null || id == x.id)
                && (string.IsNullOrEmpty(estatus) || (estatus == "TERMINADO" && x.hora_vigilancia_salida.HasValue)
                    || (estatus == "CANCELADO" && x.hora_cancelacion.HasValue)
                    || (estatus == "EN_TRANSITO" && !x.hora_cancelacion.HasValue && !x.hora_vigilancia_salida.HasValue)
                )
                && (string.IsNullOrEmpty(carga) || (carga == "CARGA" && x.carga) || (carga == "DESCARGA" && x.descarga))
                && (string.IsNullOrEmpty(linea) || x.linea_transporte.Contains(linea))
                && (string.IsNullOrEmpty(placas) || x.placas_tractor.Contains(placas))
                && x.fecha_ingreso_vigilancia >= dateInicial && x.fecha_ingreso_vigilancia <= dateFinal
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
