using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class empleadosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();


        // GET: empleados
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.RH) && !TieneRol(TipoRoles.RH_DETALLES_EMPLEADOS))
                return View("../Home/ErrorPermisos");
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var listado = db.empleados
                   .OrderBy(x => x.id).ToList();

            return View(listado);

        }

        // GET: empleados/Details/5
        public ActionResult Details(int? id)
        {

            if (!TieneRol(TipoRoles.RH) && !TieneRol(TipoRoles.RH_DETALLES_EMPLEADOS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            empleados empleados = db.empleados.Find(id);
            if (empleados == null)
            {
                return View("../Error/NotFound");
            }
            ViewBag.PrimerNivel = "recursos_humanos";
            ViewBag.SegundoNivel = "empleados";
            ViewBag.ControllerName = "Empleados";
            return View(empleados);

        }

        // GET: empleados/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.RH))
            {
                ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.id_area = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
                    textoPorDefecto: "-- Seleccione un valor --");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: empleados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(empleados empleados, FormCollection collection)
        {

            //valores enviados previamente
            int c_planta = 0;
            if (!String.IsNullOrEmpty(collection["planta_clave"]))
                Int32.TryParse(collection["planta_clave"], out c_planta);
            int c_area = 0;
            if (!String.IsNullOrEmpty(collection["id_area"]))
                Int32.TryParse(collection["id_area"].ToString(), out c_area);
            int c_puesto = 0;
            if (!String.IsNullOrEmpty(collection["puesto"]))
                Int32.TryParse(collection["puesto"].ToString(), out c_puesto);

            //valida el archivo de imagen
            if (empleados.ArchivoImagen != null)
            {

                //valida el tamaño del archivo
                if (empleados.ArchivoImagen.InputStream.Length > (1048576 * 4))
                    ModelState.AddModelError("", "Sólo se permiten archivos menores a 4MB");
                else
                {
                    //valida que la extensión del archivo sea válida
                    string extension = Path.GetExtension(empleados.ArchivoImagen.FileName);
                    if (extension.ToUpper() != ".PNG"   //si no contiene una extensión válida
                        && extension.ToUpper() != ".JPEG"
                        && extension.ToUpper() != ".JPG"
                        && extension.ToUpper() != ".GIF"
                        && extension.ToUpper() != ".BMP"
                        )
                    {
                        ModelState.AddModelError("", "Sólo se permiten archivos con extensión .png, .jpeg, .jpg, .gif, .bmp");
                    }
                    else
                    {
                        //La extensión y el tamaño son válidos
                        String nombreArchivo = empleados.ArchivoImagen.FileName;

                        //recorta el nombre del archivo en caso de ser necesario
                        if (nombreArchivo.Length > 80)
                        {
                            nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;
                        }

                        //lee el archivo en un arreglo de bytes
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(empleados.ArchivoImagen.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(empleados.ArchivoImagen.ContentLength);
                        }

                        //genera el archivo de biblioce digital
                        empleados.biblioteca_digital = new biblioteca_digital
                        {
                            Nombre = nombreArchivo,
                            MimeType = UsoStrings.RecortaString(empleados.ArchivoImagen.ContentType, 80),
                            Datos = fileData
                        };

                    }
                }

            }

            //valida el numero de empleado
            if (!Int32.TryParse(empleados.numeroEmpleado, out int num) && empleados.numeroEmpleado.ToUpper() != "N/A")
                ModelState.AddModelError("", "El número de empleado no es válido. Ingrese sólo números o el valor \"N/A\"");

            //busca si ya existe un empleado con ese numero de empleado

            // existe el num empleado
            if (db.empleados.Any(s => !string.IsNullOrEmpty(empleados.numeroEmpleado) && empleados.numeroEmpleado.ToUpper() != "N/A" && s.numeroEmpleado == empleados.numeroEmpleado))
                ModelState.AddModelError("", "Ya existe un registro con el mismo número de empleado. ");

            if (ModelState.IsValid)
            {
                empleados.activo = true;
                empleados.mostrar_telefono = true;
                //convierte a mayúsculas
                empleados.nombre = empleados.nombre.ToUpper();
                empleados.apellido1 = empleados.apellido1.ToUpper();
                if (!String.IsNullOrEmpty(empleados.apellido2))
                    empleados.apellido2 = empleados.apellido2.ToUpper();
                empleados.numeroEmpleado = empleados.numeroEmpleado.ToUpper(); //para n/a

                //agrega el id_area
                if (c_area > 0)
                    empleados.id_area = c_area;

                db.empleados.Add(empleados);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");

            }

            ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.id_area = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
                  textoPorDefecto: "-- Seleccione un valor --", selected: empleados.id_jefe_directo.ToString());

            //claves seleccionadas
            ViewBag.c_planta = c_planta;
            ViewBag.c_area = c_area;
            ViewBag.c_puesto = c_puesto;
            return View(empleados);
        }

        // GET: empleados/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.RH))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                empleados empleados = db.empleados.Find(id);
                if (empleados == null)
                {
                    return View("../Error/NotFound");
                }

                ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.id_area = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
                textoPorDefecto: "-- Seleccione un valor --", selected: empleados.id_jefe_directo.ToString());


                //claves seleccionadas
                ViewBag.c_planta = empleados.planta_clave;
                ViewBag.c_area = empleados.id_area;
                ViewBag.c_puesto = empleados.puesto;

                return View(empleados);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: empleados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(empleados empleados, FormCollection collection, bool elimina_documento)
        {
            //valores enviados previamente
            int c_planta = 0;
            if (!String.IsNullOrEmpty(collection["planta_clave"]))
                Int32.TryParse(collection["planta_clave"], out c_planta);
            int c_area = 0;
            if (!String.IsNullOrEmpty(collection["id_area"]))
                Int32.TryParse(collection["id_area"].ToString(), out c_area);
            int c_puesto = 0;
            if (!String.IsNullOrEmpty(collection["puesto"]))
                Int32.TryParse(collection["puesto"].ToString(), out c_puesto);

            //valida el archivo de imagen
            if (empleados.ArchivoImagen != null)        //se remplaza o se sube nuevo
            {
                //valida el tamaño del archivo
                if (empleados.ArchivoImagen.InputStream.Length > (1048576 * 4))
                    ModelState.AddModelError("", "Sólo se permiten archivos menores a 4MB");
                else
                {
                    //valida que la extensión del archivo sea válida
                    string extension = Path.GetExtension(empleados.ArchivoImagen.FileName);
                    if (extension.ToUpper() != ".PNG"   //si no contiene una extensión válida
                        && extension.ToUpper() != ".JPEG"
                        && extension.ToUpper() != ".JPG"
                        && extension.ToUpper() != ".GIF"
                        && extension.ToUpper() != ".BMP"
                        )
                    {
                        ModelState.AddModelError("", "Sólo se permiten archivos con extensión .png, .jpeg, .jpg, .gif, .bmp");
                    }
                    else
                    {
                        //La extensión y el tamaño son válidos
                        String nombreArchivo = empleados.ArchivoImagen.FileName;

                        //recorta el nombre del archivo en caso de ser necesario
                        if (nombreArchivo.Length > 80)
                        {
                            nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;
                        }

                        //lee el archivo en un arreglo de bytes
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(empleados.ArchivoImagen.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(empleados.ArchivoImagen.ContentLength);
                        }

                        //se reemplaza el ya existente
                        if (empleados.id_fotografia.HasValue)
                        {
                            //modifica los valores del archivo anterior
                            var archivoAnteriorBD = db.biblioteca_digital.Find(empleados.id_fotografia.Value);
                            archivoAnteriorBD.Nombre = nombreArchivo;
                            archivoAnteriorBD.MimeType = UsoStrings.RecortaString(empleados.ArchivoImagen.ContentType, 80);
                            archivoAnteriorBD.Datos = fileData;
                        }
                        //se crea nuevo archivo
                        else
                        {
                            //genera el archivo de biblioce digital
                            var archivo = new biblioteca_digital
                            {
                                Nombre = nombreArchivo,
                                MimeType = UsoStrings.RecortaString(empleados.ArchivoImagen.ContentType, 80),
                                Datos = fileData
                            };
                            //guarda el archivo en la BD
                            try
                            {
                                db.biblioteca_digital.Add(archivo);
                                db.SaveChanges();
                                empleados.id_fotografia = archivo.Id;
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError("", "Error al guardar en BD_ " + ex.Message);
                            }


                        }



                    }
                }
            }
            //en caso de que no se haya enviado archivo lo elimina de la BD
            else
            {
                //elimina el archivo en caso de existir
                if (empleados.id_fotografia.HasValue && elimina_documento)
                {
                    var archivoAnteriorBD = db.biblioteca_digital.Find(empleados.id_fotografia.Value);
                    db.biblioteca_digital.Remove(archivoAnteriorBD);

                    empleados.id_fotografia = null;
                }
            }

            //valida el numero de empleado
            if (!Int32.TryParse(empleados.numeroEmpleado, out int num) && empleados.numeroEmpleado.ToUpper() != "N/A")
                ModelState.AddModelError("", "El número de empleado no es válido. Ingrese sólo números o el valor \"N/A\"");

            //busca si ya existe un empleado con ese numero de empleado

            // existe el num empleado
            if (db.empleados.Any(s => !string.IsNullOrEmpty(empleados.numeroEmpleado) && empleados.numeroEmpleado.ToUpper() != "N/A" && s.numeroEmpleado == empleados.numeroEmpleado && s.id != empleados.id))
                ModelState.AddModelError("", "Ya existe un registro con el mismo número de empleado. ");


            if (ModelState.IsValid)
            {

                //actualiza el correo electronico en la tabla de usuarios
                var user = _userManager.Users.Where(u => u.IdEmpleado == empleados.id).FirstOrDefault();
                if (user != null)
                {
                    //actualiza el usuario  
                    user.Email = empleados.correo;
                    //guarda el usuario en BD
                    var result = await _userManager.UpdateAsync(user);
                }

                empleados.nombre = empleados.nombre.ToUpper();
                empleados.apellido1 = empleados.apellido1.ToUpper();
                empleados.apellido2 = empleados.apellido2.ToUpper();
                empleados.numeroEmpleado = empleados.numeroEmpleado.ToUpper(); //para n/a

                //agrega el id_area
                if (c_area > 0)
                    empleados.id_area = c_area;

                db.Entry(empleados).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar en BD_ " + ex.Message);
                }
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");

            }
            ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.id_area = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true), "clave", "descripcion");
            ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
            textoPorDefecto: "-- Seleccione un valor --", selected: empleados.id_jefe_directo.ToString());

            //claves seleccionadas
            ViewBag.c_planta = c_planta;
            ViewBag.c_area = c_area;
            ViewBag.c_puesto = c_puesto;
            return View(empleados);
        }

        public ActionResult Exportar(string numeroEmpleado, string nombre, string planta, string departamento, string puesto, string jefe_directo, string C8ID, string correo, string activo)
        {
            if (!TieneRol(TipoRoles.RH) && !TieneRol(TipoRoles.RH_DETALLES_EMPLEADOS))
                return View("../Home/ErrorPermisos");

            var listado = db.empleados
                    .ToList()
                    .Where(x =>
                        (string.IsNullOrEmpty(numeroEmpleado) || x.numeroEmpleado.IndexOf(numeroEmpleado, StringComparison.OrdinalIgnoreCase) >= 0)
                       && (string.IsNullOrEmpty(nombre) || x.ConcatNombre.IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0)
                       && (string.IsNullOrEmpty(planta) || (x.plantas != null && x.plantas.descripcion.IndexOf(planta, StringComparison.OrdinalIgnoreCase) >= 0))
                       && (string.IsNullOrEmpty(departamento) || (x.Area != null && x.Area.descripcion.IndexOf(departamento, StringComparison.OrdinalIgnoreCase) >= 0))
                       && (string.IsNullOrEmpty(puesto) || (x.puesto1 != null && x.puesto1.descripcion.IndexOf(puesto, StringComparison.OrdinalIgnoreCase) >= 0))
                       && (string.IsNullOrEmpty(jefe_directo) || (x.empleados2 != null && x.empleados2.ConcatNombre.IndexOf(jefe_directo, StringComparison.OrdinalIgnoreCase) >= 0))
                       && (string.IsNullOrEmpty(C8ID) || x.C8ID.IndexOf(C8ID, StringComparison.OrdinalIgnoreCase) >= 0)
                       && (string.IsNullOrEmpty(correo) || x.correo.IndexOf(correo, StringComparison.OrdinalIgnoreCase) >= 0)
                       && (string.IsNullOrEmpty(activo) || (x.activo.HasValue && x.activo.Value && "SI".IndexOf(activo, StringComparison.OrdinalIgnoreCase) >= 0) || (x.activo.HasValue && !x.activo.Value && "NO".IndexOf(activo, StringComparison.OrdinalIgnoreCase) >= 0))
                    )
                    .OrderBy(x => x.id)
                  .ToList();

            byte[] stream = ExcelUtil.GeneraReporteEmpleados(listado);


            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = "Reporte_empleados_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(stream, "application/vnd.ms-excel");


        }
        public ActionResult Exportartkmm(string numeroEmpleado, string nombre, string planta, string departamento, string puesto, string jefe_directo, string C8ID, string correo, string activo)
        {
            if (!TieneRol(TipoRoles.RH) && !TieneRol(TipoRoles.RH_DETALLES_EMPLEADOS))
                return View("../Home/ErrorPermisos");

            var listado = db.empleados
                    .ToList()
                    .Where(x =>
                        (string.IsNullOrEmpty(numeroEmpleado) || x.numeroEmpleado.IndexOf(numeroEmpleado, StringComparison.OrdinalIgnoreCase) >= 0)
                       && (string.IsNullOrEmpty(nombre) || x.ConcatNombre.IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0)
                       && (string.IsNullOrEmpty(planta) || (x.plantas != null && x.plantas.descripcion.IndexOf(planta, StringComparison.OrdinalIgnoreCase) >= 0))
                       && (string.IsNullOrEmpty(departamento) || (x.Area != null && x.Area.descripcion.IndexOf(departamento, StringComparison.OrdinalIgnoreCase) >= 0))
                       && (string.IsNullOrEmpty(puesto) || (x.puesto1 != null && x.puesto1.descripcion.IndexOf(puesto, StringComparison.OrdinalIgnoreCase) >= 0))
                       && (string.IsNullOrEmpty(jefe_directo) || (x.empleados2 != null && x.empleados2.ConcatNombre.IndexOf(jefe_directo, StringComparison.OrdinalIgnoreCase) >= 0))
                       && (string.IsNullOrEmpty(C8ID) || x.C8ID.IndexOf(C8ID, StringComparison.OrdinalIgnoreCase) >= 0)
                       && (string.IsNullOrEmpty(correo) || x.correo.IndexOf(correo, StringComparison.OrdinalIgnoreCase) >= 0)
                       && (string.IsNullOrEmpty(activo) || (x.activo.HasValue && x.activo.Value && "SI".IndexOf(activo, StringComparison.OrdinalIgnoreCase) >= 0) || (x.activo.HasValue && !x.activo.Value && "NO".IndexOf(activo, StringComparison.OrdinalIgnoreCase) >= 0))
                    )
                    .OrderBy(x => x.numeroEmpleado)
                  .ToList();

            byte[] stream = ExcelUtil.GeneraReporteEmpleadostkmmnet(listado);


            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = "Reporte_empleados_tkmmnet_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(stream, "application/vnd.ms-excel");


        }

        // GET: Empleados/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.RH))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                empleados empleado = db.empleados.Find(id);
                if (empleado == null)
                {
                    return View("../Error/NotFound");
                }
                return View(empleado);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Empleados/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id, FormCollection collection)
        {
            empleados empleado = db.empleados.Find(id);
            empleado.activo = false;

            DateTime bajaFecha = DateTime.Now;
            string stringFecha = collection["bajaFecha"];

            bool notificacionIT = false;
            if (collection.AllKeys.Any(x => x == "notificacion_it"))
                notificacionIT = true;



            try
            {
                if (!String.IsNullOrEmpty(stringFecha))
                {
                    bajaFecha = Convert.ToDateTime(stringFecha);
                    empleado.bajaFecha = bajaFecha;
                }
            }
            catch (FormatException e)
            {
                ModelState.AddModelError("", "Error de formato de fecha: " + e.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al convertir: " + ex.Message);
            }

            //deshabilita del lado del servidor la validadcion 
            db.Configuration.ValidateOnSaveEnabled = false;

            //db.Entry(empleado).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //se envia notificación a IT en caso de haber seleccionado la opción
                if (notificacionIT)
                {

                    //OBTIENE EL CORREO DE NOTIFICACION
                    var itEmail = db.notificaciones_correo.Where(x => x.descripcion == "IT_TKMM").FirstOrDefault();
                    if (itEmail != null)
                    {

                        //envia correo electronico
                        EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                        List<String> correos = new List<string>(); //correos TO

                        if (!String.IsNullOrEmpty(itEmail.correo))
                            correos.Add(itEmail.correo); //agrega correo de validador

                        envioCorreo.SendEmailAsync(correos, "Notificación de Baja de Empleado", envioCorreo.getBodyITBajaEmpleado(empleado));
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
            finally
            {
                //vuelve a activa la validadcion de la entidad
                db.Configuration.ValidateOnSaveEnabled = true;
            }

            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.DISABLED, TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("Index");
        }

        // GET: Empleados/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.RH))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                empleados empleado = db.empleados.Find(id);
                if (empleado == null)
                {
                    return View("../Error/NotFound");
                }
                return View(empleado);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Empleados/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            empleados empleado = db.empleados.Find(id);
            empleado.activo = true;
            empleado.bajaFecha = null;

            //deshabilita del lado del servidor la validadcion 
            db.Configuration.ValidateOnSaveEnabled = false;

            db.Entry(empleado).State = EntityState.Modified;
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
            finally
            {
                //vuelve a activa la validadcion de la entidad
                db.Configuration.ValidateOnSaveEnabled = true;
            }


            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.ENABLED, TipoMensajesSweetAlerts.SUCCESS);
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
