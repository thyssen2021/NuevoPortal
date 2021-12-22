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
    public class PolizaManualController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: PolizaManual/CaptutistaCreadas
        public ActionResult CapturistaCreadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.poliza_manual.Include(p => p.biblioteca_digital).Include(p => p.biblioteca_digital1).Include(p => p.currency).Include(p => p.empleados).Include(p => p.plantas).Include(p => p.PM_autorizadores).Include(p => p.PM_tipo_poliza).Include(p => p.PM_validadores)
                    .Where(x => x.id_elaborador == empleado.id && x.estatus == PM_Status.CREADO )
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual.Include(p => p.biblioteca_digital).Include(p => p.biblioteca_digital1).Include(p => p.currency).Include(p => p.empleados).Include(p => p.plantas).Include(p => p.PM_autorizadores).Include(p => p.PM_tipo_poliza).Include(p => p.PM_validadores).Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones
                ViewBag.Edit = true;
                ViewBag.Details = true;
                ViewBag.EnviarValidacion = true;
                ViewBag.Title = "Listado Pólizas Creadas";
                ViewBag.SegundoNivel = "PM_registro";
                ViewBag.Create = true;

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: PolizaManual/CapturistaPendientes
        public ActionResult CapturistaPendientes(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.poliza_manual.Include(p => p.biblioteca_digital).Include(p => p.biblioteca_digital1).Include(p => p.currency).Include(p => p.empleados).Include(p => p.plantas).Include(p => p.PM_autorizadores).Include(p => p.PM_tipo_poliza).Include(p => p.PM_validadores)
                    .Where(x => x.id_elaborador == empleado.id 
                    && (x.estatus == PM_Status.ENVIADO_A_AREA || x.estatus == PM_Status.ENVIADO_A_CONTABILIDAD || x.estatus == PM_Status.ENVIADO_A_CONTROLLING
                    || x.estatus == PM_Status.AUTORIZADO_CONTROLLING) )
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual.Include(p => p.biblioteca_digital).Include(p => p.biblioteca_digital1).Include(p => p.currency).Include(p => p.empleados).Include(p => p.plantas).Include(p => p.PM_autorizadores).Include(p => p.PM_tipo_poliza).Include(p => p.PM_validadores).Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones
                ViewBag.Details = true;
                ViewBag.Title = "Listado Pólizas Pendientes";
                ViewBag.SegundoNivel = "PM_pendientes";
                ViewBag.Create = true;

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: PolizaManual/CapturistaRechazadas
        public ActionResult CapturistaRechazadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.poliza_manual.Include(p => p.biblioteca_digital).Include(p => p.biblioteca_digital1).Include(p => p.currency).Include(p => p.empleados).Include(p => p.plantas).Include(p => p.PM_autorizadores).Include(p => p.PM_tipo_poliza).Include(p => p.PM_validadores)
                    .Where(x => x.id_elaborador == empleado.id && x.estatus == PM_Status.RECHAZADO )
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual.Include(p => p.biblioteca_digital).Include(p => p.biblioteca_digital1).Include(p => p.currency).Include(p => p.empleados).Include(p => p.plantas).Include(p => p.PM_autorizadores).Include(p => p.PM_tipo_poliza).Include(p => p.PM_validadores).Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones
                ViewBag.Edit = true;
                ViewBag.Details = true;
                ViewBag.EnviarValidacion = true;
                ViewBag.Title = "Listado Pólizas Rechazadas";
                ViewBag.SegundoNivel = "PM_rechazadas";
                ViewBag.Create = true;

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: PolizaManual/CapturistaFinalizadas
        public ActionResult CapturistaFinalizadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.poliza_manual.Include(p => p.biblioteca_digital).Include(p => p.biblioteca_digital1).Include(p => p.currency).Include(p => p.empleados).Include(p => p.plantas).Include(p => p.PM_autorizadores).Include(p => p.PM_tipo_poliza).Include(p => p.PM_validadores)
                    .Where(x => x.id_elaborador == empleado.id && x.estatus == PM_Status.FINALIZADO)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual.Include(p => p.biblioteca_digital).Include(p => p.biblioteca_digital1).Include(p => p.currency).Include(p => p.empleados).Include(p => p.plantas).Include(p => p.PM_autorizadores).Include(p => p.PM_tipo_poliza).Include(p => p.PM_validadores).Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones
               
                ViewBag.Details = true;
                ViewBag.Title = "Listado Pólizas Finalizadas";
                ViewBag.SegundoNivel = "PM_finalizadas";
                ViewBag.Create = true;

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: PolizaManual/ValidadorPendientes
        public ActionResult ValidadorPendientes(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_VALIDAR_POR_AREA))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //busca el validador por el id de empleado
                PM_validadores validador = db.PM_validadores.FirstOrDefault(x => x.id_empleado == empleado.id);                
                int idValidador = validador == null? 0: validador.id;
                
                var listado = db.poliza_manual.Include(p => p.biblioteca_digital).Include(p => p.biblioteca_digital1).Include(p => p.currency).Include(p => p.empleados).Include(p => p.plantas).Include(p => p.PM_autorizadores).Include(p => p.PM_tipo_poliza).Include(p => p.PM_validadores)
                    .Where(x => x.id_elaborador == empleado.id && x.estatus == PM_Status.ENVIADO_A_AREA && x.id_validador==idValidador)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual.Include(p => p.biblioteca_digital).Include(p => p.biblioteca_digital1).Include(p => p.currency).Include(p => p.empleados).Include(p => p.plantas).Include(p => p.PM_autorizadores).Include(p => p.PM_tipo_poliza).Include(p => p.PM_validadores).Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones

                ViewBag.Details = true;
                ViewBag.Validar = true;
                ViewBag.Title = "Listado Pólizas Pendientes";
                ViewBag.SegundoNivel = "PM_validar_area_pendientes";                

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: PolizaManual/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.PM_AUTORIZAR_CONTROLLING) || TieneRol(TipoRoles.PM_VALIDAR_POR_AREA)
              || TieneRol(TipoRoles.PM_CONTABILIDAD)|| TieneRol(TipoRoles.PM_REGISTRO))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                poliza_manual poliza_manual = db.poliza_manual.Find(id);
                if (poliza_manual == null)
                {
                    return View("../Error/NotFound");
                }
                return View(poliza_manual);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: PolizaManual/EnviarParaValidacion/5
        public ActionResult EnviarParaValidacion(int? id)
        {
            if (TieneRol(TipoRoles.PM_REGISTRO) )
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                poliza_manual poliza_manual = db.poliza_manual.Find(id);
                if (poliza_manual == null)
                {
                    return View("../Error/NotFound");
                }
                return View(poliza_manual);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: PremiumFreightAproval/Send/5
        [HttpPost, ActionName("EnviarParaValidacion")]
        [ValidateAntiForgeryToken]
        public ActionResult EnviarParaValidacionConfirmed(int id)
        {
            poliza_manual pm = db.poliza_manual.Find(id);
            string estatusAnterior = pm.estatus;
            pm.estatus = PM_Status.ENVIADO_A_CONTROLLING;

            db.Entry(pm).State = EntityState.Modified;
            try
            {
                db.SaveChanges();


                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO
                correos.Add("alfredo.xochitemol@lagermex.com.mx");

                //if (!String.IsNullOrEmpty(pm.PM_validadores.empleados.correo))
                //    correos.Add(pm.PM_validadores.empleados.correo); //agrega correo de autorizador

                envioCorreo.SendEmailAsync(correos, "Ha recibido una Póliza Manual para su aprobación.", envioCorreo.getBodyPMSendValidador(pm));
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
                return RedirectToAction("CapturistaCreadas");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("CapturistaCreadas");
            }

            TempData["Mensaje"] = new MensajesSweetAlert("La Póliza Manual ha sido enviada", TipoMensajesSweetAlerts.SUCCESS);

            if (estatusAnterior == PM_Status.RECHAZADO)
                return RedirectToAction("CapturistaRechazadas");

            return RedirectToAction("CapturistaCreadas");
        }

        // GET: PolizaManual/ValidarArea/5
        public ActionResult ValidarArea(int? id)
        {
            if (TieneRol(TipoRoles.PM_REGISTRO))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                poliza_manual poliza_manual = db.poliza_manual.Find(id);
                if (poliza_manual == null)
                {
                    return View("../Error/NotFound");
                }
                return View(poliza_manual);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }


        // POST: PremiumFreightAproval/Rechazar/5
        [HttpPost, ActionName("RechazarAreaPM")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarAreaPMConfirmed(FormCollection collection)
        {

            int id = 0;
            if (!String.IsNullOrEmpty(collection["id"]))
                Int32.TryParse(collection["id"], out id);

            String razonRechazo = collection["comentario_rechazo"];


            poliza_manual poliza = db.poliza_manual.Find(id);
            poliza.estatus = PM_Status.RECHAZADO;
            poliza.comentario_rechazo = razonRechazo;

            db.Entry(poliza).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                correos.Add("alfredo.xochitemol@lagermex.com.mx");

                //if (!String.IsNullOrEmpty(poliza.empleados.correo))
                 //   correos.Add(poliza.empleados.correo);

                envioCorreo.SendEmailAsync(correos, "Su Póliza Manual ha sido Rechazada", "envioCorreo.getBodyPolizaRechazado(poliza)");

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
                return RedirectToAction("ValidadorPendientes");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("ValidadorPendientes");
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La Poliza ha sido rechazada correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("ValidadorPendientes");
        }

        // POST: PremiumFreightAproval/ValidarAreaPM/5
        [HttpPost, ActionName("ValidarAreaPM")]
        [ValidateAntiForgeryToken]
        public ActionResult ValidarAreaPMConfirmed(FormCollection collection)
        {

            int id = 0;
            if (!String.IsNullOrEmpty(collection["id"]))
                Int32.TryParse(collection["id"], out id);


            poliza_manual poliza = db.poliza_manual.Find(id);
            poliza.estatus = PM_Status.VALIDADO_POR_AREA;
            poliza.fecha_validacion= DateTime.Now;
            //***** AGREGAR CAMPO DE SELECCIÓN DE AUTORIZADOR ********

            db.Entry(poliza).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                correos.Add("alfredo.xochitemol@lagermex.com.mx");

                // *** LEER EMAIL DE AUTORIZADOR ******
               // if (!String.IsNullOrEmpty(poliza.empleados.correo))
               //     correos.Add(poliza.empleados.correo);

                envioCorreo.SendEmailAsync(correos, "La poliza ha sido válidada por el área.", "envioCorreo.getBodyPolizaManualAutorizado(poliza)");

                /* ANALIZAR SI NECESARIO MANDAR CORREO TANTO AL AUTORIZADOR COMO AL USUARIO */


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
                return RedirectToAction("ValidadorPendientes");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("ValidadorPendientes");
            }
            TempData["Mensaje"] = new MensajesSweetAlert("La póliza ha sido autorizada.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("ValidadorPendientes");
        }


        // GET: PolizaManual/Create
        public ActionResult Create()
        {

            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();
                ViewBag.Solicitante = empleado;


                ViewBag.currency_iso = AddFirstItem(new SelectList(db.currency.Where(x => x.activo), "CurrencyISO", "CocatCurrency"), selected: "USD");
                ViewBag.id_PM_tipo_poliza = AddFirstItem(new SelectList(db.PM_tipo_poliza.Where(x => x.activo), "id", "descripcion"));
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"));
                ViewBag.id_validador = AddFirstItem(new SelectList(db.PM_validadores.Where(x => x.activo), "id", "ConcatNameValidador"));

                return View(new poliza_manual());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }



        }

        // POST: PolizaManual/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(poliza_manual poliza_manual)
        {

            //determina si hay conceptos
            if (poliza_manual.PM_conceptos.Count == 0)
                ModelState.AddModelError("", "No se ingresaron conceptos para la póliza.");

            decimal debe = 0;
            decimal haber = 0;

            //determina si las sumas son iguales
            foreach (PM_conceptos item in poliza_manual.PM_conceptos)
            {
                //suma el valor de debe
                if (item.debe.HasValue)
                    debe += item.debe.Value;
                //suma el valor de haber
                if (item.haber.HasValue)
                    haber += item.haber.Value;
            }

            //verifica si las sumas son diferentes
            if (debe != haber)
                ModelState.AddModelError("", "Las sumas no son iguales.");

            //verifica si el tamaño del archivo es válido
            if (poliza_manual.PostedFileSoporte != null && poliza_manual.PostedFileSoporte.InputStream.Length > 5242880)
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 5MB.");
            else if (poliza_manual.PostedFileSoporte != null)
            { //verifica la extensión del archivo
                string extension = Path.GetExtension(poliza_manual.PostedFileSoporte.FileName);
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
                    ModelState.AddModelError("", "Sólo se permiten archivos con extensión .xls, .xlsx, .doc, .docx, .pdf, .png, .jpg, .jpeg, .rar, .zip.");
                else { //si la extension y el tamaño son válidos
                    String nombreArchivo = poliza_manual.PostedFileSoporte.FileName;

                    //recorta el nombre del archivo en caso de ser necesario
                    if (nombreArchivo.Length > 80)                    
                        nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                    //lee el archivo en un arreglo de bytes
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(poliza_manual.PostedFileSoporte.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(poliza_manual.PostedFileSoporte.ContentLength);
                    }

                    //genera el archivo de biblioce digital
                    biblioteca_digital archivo = new biblioteca_digital
                    {
                        Nombre = nombreArchivo,
                        MimeType = UsoStrings.RecortaString(poliza_manual.PostedFileSoporte.ContentType, 80),
                        Datos = fileData
                    };

                    //relaciona el archivo con la poliza (despues se guarda en BD)
                    poliza_manual.biblioteca_digital1 = archivo;  //documento soporte

                }
            }

            if (ModelState.IsValid)
            {
               
                poliza_manual.fecha_creacion = DateTime.Now;
                poliza_manual.estatus = PM_Status.CREADO;

                db.poliza_manual.Add(poliza_manual);

                TempData["Mensaje"] = new MensajesSweetAlert("Se ha actualizado el registro correctamente", TipoMensajesSweetAlerts.SUCCESS);


                db.SaveChanges();
                return RedirectToAction("CapturistaCreadas");
            }

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();
            ViewBag.Solicitante = empleado;

            ViewBag.currency_iso = AddFirstItem(new SelectList(db.currency.Where(x => x.activo), "CurrencyISO", "CocatCurrency"));
            ViewBag.id_PM_tipo_poliza = AddFirstItem(new SelectList(db.PM_tipo_poliza.Where(x => x.activo), "id", "descripcion"));
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"));
            ViewBag.id_validador = AddFirstItem(new SelectList(db.PM_validadores.Where(x => x.activo), "id", "ConcatNameValidador"));


            return View(poliza_manual);
        }

        // GET: PolizaManual/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.PFA_REGISTRO))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                poliza_manual poliza_manual = db.poliza_manual.Find(id);
                if (poliza_manual == null)
                {
                    return View("../Error/NotFound");
                }


                ViewBag.currency_iso = AddFirstItem(new SelectList(db.currency.Where(x => x.activo), "CurrencyISO", "CocatCurrency"), selected: poliza_manual.currency_iso);
                ViewBag.id_PM_tipo_poliza = AddFirstItem(new SelectList(db.PM_tipo_poliza.Where(x => x.activo), "id", "descripcion"), selected: poliza_manual.id_PM_tipo_poliza.ToString());
                ViewBag.id_validador = AddFirstItem(new SelectList(db.PM_validadores.Where(x => x.activo), "id", "ConcatNameValidador"), selected: poliza_manual.id_validador.ToString());
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"), selected: poliza_manual.id_planta.ToString()); 

                return View(poliza_manual);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: PolizaManual/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(poliza_manual poliza_manual)
        {

            biblioteca_digital archivo = new biblioteca_digital { };

            //determina si hay conceptos
            if (poliza_manual.PM_conceptos.Count == 0)
                ModelState.AddModelError("", "No se ingresaron conceptos para la póliza.");

            decimal debe = 0;
            decimal haber = 0;

            //determina si las sumas son iguales
            foreach (PM_conceptos item in poliza_manual.PM_conceptos)
            {
                //suma el valor de debe
                if (item.debe.HasValue)
                    debe += item.debe.Value;
                //suma el valor de haber
                if (item.haber.HasValue)
                    haber += item.haber.Value;
            }

            //verifica si las sumas son diferentes
            if (debe != haber)
                ModelState.AddModelError("", "Las sumas no son iguales.");

            //verifica si el tamaño del archivo es válido
            if (poliza_manual.PostedFileSoporte != null && poliza_manual.PostedFileSoporte.InputStream.Length > 5242880)
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 5MB.");
            else if (poliza_manual.PostedFileSoporte != null)
            { //verifica la extensión del archivo
                string extension = Path.GetExtension(poliza_manual.PostedFileSoporte.FileName);
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
                    ModelState.AddModelError("", "Sólo se permiten archivos con extensión .xls, .xlsx, .doc, .docx, .pdf, .png, .jpg, .jpeg, .rar, .zip.");
                else
                { //si la extension y el tamaño son válidos
                    String nombreArchivo = poliza_manual.PostedFileSoporte.FileName;

                    //recorta el nombre del archivo en caso de ser necesario
                    if (nombreArchivo.Length > 80)
                        nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                    //lee el archivo en un arreglo de bytes
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(poliza_manual.PostedFileSoporte.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(poliza_manual.PostedFileSoporte.ContentLength);
                    }

                    //si tiene archivo hace un update sino hace un create
                    if (poliza_manual.id_documento_soporte.HasValue)//si tiene valor hace un update
                    {
                        archivo = db.biblioteca_digital.Find(poliza_manual.id_documento_soporte.Value);
                        archivo.Nombre = nombreArchivo;
                        archivo.MimeType = UsoStrings.RecortaString(poliza_manual.PostedFileSoporte.ContentType, 80);
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
                            MimeType = UsoStrings.RecortaString(poliza_manual.PostedFileSoporte.ContentType, 80),
                            Datos = fileData
                        };

                        //update en BD
                        db.biblioteca_digital.Add(archivo);
                        db.SaveChanges();
                    }

                }
            }

            if (ModelState.IsValid)
            {
                //verifica si se creo un archivo
                if (archivo.Id > 0) //si existe algún archivo en biblioteca digital
                    poliza_manual.id_documento_soporte = archivo.Id;

                //borra los conceptos anteriores
                var list = db.PM_conceptos.Where(x => x.id_poliza == poliza_manual.id);
                foreach (PM_conceptos item in list)
                    db.PM_conceptos.Remove(item);
                db.SaveChanges();

                //los nuevos conceptos se agregan automáticamente

                //si existe lo modifica
                poliza_manual poliza = db.poliza_manual.Find(poliza_manual.id);
                // Activity already exist in database and modify it
                db.Entry(poliza).CurrentValues.SetValues(poliza_manual);
                //agrega los conceptos 
                poliza.PM_conceptos = poliza_manual.PM_conceptos;

                db.Entry(poliza).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("CapturistaCreadas");
            }

            //cargar el documento de soporte
            if (poliza_manual.id_documento_soporte.HasValue) 
                poliza_manual.biblioteca_digital1 = db.biblioteca_digital.Find(poliza_manual.id_documento_soporte);

            //cargar el documento de registro
            if (poliza_manual.id_documento_registro.HasValue)
                poliza_manual.biblioteca_digital = db.biblioteca_digital.Find(poliza_manual.id_documento_registro);

            poliza_manual.empleados = db.empleados.Find(poliza_manual.id_elaborador);

            ViewBag.currency_iso = AddFirstItem(new SelectList(db.currency.Where(x => x.activo), "CurrencyISO", "CocatCurrency"));
            ViewBag.id_PM_tipo_poliza = AddFirstItem(new SelectList(db.PM_tipo_poliza.Where(x => x.activo), "id", "descripcion"));
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"));
            ViewBag.id_validador = AddFirstItem(new SelectList(db.PM_validadores.Where(x => x.activo), "id", "ConcatNameValidador"));

            return View(poliza_manual);
        }

        // GET: PolizaManual/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            poliza_manual poliza_manual = db.poliza_manual.Find(id);
            if (poliza_manual == null)
            {
                return View("../Error/NotFound");
            }
            return View(poliza_manual);
        }

        // POST: PolizaManual/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            poliza_manual poliza_manual = db.poliza_manual.Find(id);
            db.poliza_manual.Remove(poliza_manual);
            db.SaveChanges();
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
