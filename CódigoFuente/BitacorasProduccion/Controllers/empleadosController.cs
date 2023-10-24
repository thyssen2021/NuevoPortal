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
using Bitacoras.Util;
using Clases.Util;
using Newtonsoft.Json.Linq;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class empleadosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();
        private string jsonOrganigrama = String.Empty;

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


            ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados, nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
                    textoPorDefecto: "-- Seleccione un valor --");
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
            //verifica si es shared services
            empleados.shared_services = empleados.Area != null && empleados.Area.shared_services;

            ViewBag.PrimerNivel = "recursos_humanos";
            ViewBag.SegundoNivel = "empleados";
            ViewBag.ControllerName = "Empleados";
            return View(empleados);

        }
        public ActionResult Organigrama()
        {

            if (!TieneRol(TipoRoles.RH) && !TieneRol(TipoRoles.RH_DETALLES_EMPLEADOS))
                return View("../Home/ErrorPermisos");

            List<NodoOrganigrama> nodos = new List<NodoOrganigrama>();

            foreach (var empleado in db.empleados.Where(x => x.activo.HasValue && x.activo.Value))
            {
                List<NodoOrganigrama> hijos = new List<NodoOrganigrama>();
                //obtiene los hijos
                foreach (var item in empleado.empleados1.Where(x => x.id != empleado.id && x.activo.HasValue && x.activo.Value))
                {
                    hijos.Add(new NodoOrganigrama
                    {
                        ID = item.id,
                        ClassName = "thyssen-level",
                        NodeContent = item.ConcatNombre + "     ",
                        NodeTitle = item.puesto1 != null ? item.puesto1.descripcion : String.Empty

                    });
                }

                nodos.Add(new NodoOrganigrama
                {
                    ID = empleado.id,
                    ClassName = "thyssen-level",
                    NodeContent = empleado.ConcatNombre + "     ",
                    NodeTitle = empleado.puesto1 != null ? empleado.puesto1.descripcion : String.Empty,
                    Childs = hijos
                });
            }

            jsonOrganigrama = String.Empty;
            //recorre el arbol (comenzando con OLAF 448 )
            RecorreNodos(nodos.FirstOrDefault(x => x.ID == 448), nodos);

            ViewBag.JsonOrganigrama = jsonOrganigrama;

            return View();

        }

        [NonAction]
        public void RecorreNodos(NodoOrganigrama r, List<NodoOrganigrama> nodos)
        {
            jsonOrganigrama += @"{'name': '" + r.NodeContent + @"',
                            'title': '" + r.NodeTitle + "'";

            if (r.Childs.Count > 0)
                jsonOrganigrama += @",";

            //recorre los hijos directos del nodo
            foreach (var item in r.Childs)
            {
                int index = r.Childs.IndexOf(item);

                if (index == 0)
                    jsonOrganigrama += @"'children': [";

                var itemBD = nodos.FirstOrDefault(x => x.ID == item.ID);
                item.Childs = itemBD.Childs;

                RecorreNodos(itemBD, nodos);

                //si hay mas hijos agrega una coma
                if (r.Childs.IndexOf(item) != r.Childs.Count - 1)
                    jsonOrganigrama += @",";
                else
                    jsonOrganigrama += @"]";
            }
            jsonOrganigrama += @"}";


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

                ViewBag.SubordinadosSeleccionados = new List<int>();
                ViewBag.TotalEmpleados = db.empleados.Where(x => x.activo == true).ToList();
                empleados empleado = new empleados
                {
                    tipo_empleado = Bitacoras.Util.Empleados_tipo.EMPLEADO
                };

                return View(empleado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: empleados/CambioJefe
        public ActionResult CambioJefe(int? id_jefe_directo)
        {
            if (!TieneRol(TipoRoles.RH))
                return View("../Home/ErrorPermisos");
            if (id_jefe_directo == null)
                return View("../Error/BadRequest");

            var jefe = db.empleados.Find(id_jefe_directo);

            //crea modelo
            CambioJefeViewModel model = new CambioJefeViewModel
            {
                id_jefe_actual = id_jefe_directo.Value,
                JefeActual = jefe
            };

            ViewBag.id_nuevo_jefe = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
                    textoPorDefecto: "-- Seleccione un valor --");
            ViewBag.subordinados = jefe.empleados1.ToList();

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambioJefe(CambioJefeViewModel model, int[] subordinados)
        {

            if (subordinados == null || subordinados.Length == 0)
            {
                ModelState.AddModelError("", "No se seleccionaron empleados.");
            }

            if (ModelState.IsValid)
            {
                //realiza cambios en la BD
                var listEmpleados = db.empleados.Where(x => subordinados.Contains(x.id));
                foreach (var item in listEmpleados)
                {
                    item.id_jefe_directo = model.id_nuevo_jefe;
                }

                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se actualizó el jefe directo correctamente", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");

            }

            var jefe = db.empleados.Find(model.id_jefe_actual);
            model.JefeActual = jefe;

            ViewBag.id_nuevo_jefe = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
                     textoPorDefecto: "-- Seleccione un valor --");
            ViewBag.subordinados = jefe.empleados1.ToList();


            return View(model);
        }


        // POST: empleados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(empleados empleados, FormCollection collection, int[] subordinados)
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
            if (!Int32.TryParse(empleados.numeroEmpleado, out int num) && empleados.numeroEmpleado.ToUpper() != "N/A" && empleados.numeroEmpleado.ToUpper() != "P99999")
                ModelState.AddModelError("", "El número de empleado no es válido. Ingrese sólo números o los valores \"N/A\" ó P99999 ");

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
                else
                    empleados.apellido2 = null;
                empleados.numeroEmpleado = empleados.numeroEmpleado.ToUpper(); //para n/a

                //agrega el id_area
                if (c_area > 0)
                    empleados.id_area = c_area;

                db.empleados.Add(empleados);
                db.SaveChanges();

                //actualiza los subordinados
                if (subordinados != null)
                    for (int i = 0; i < subordinados.Length; i++)
                    {
                        var subordinado = db.empleados.Find(subordinados[i]);
                        subordinado.id_jefe_directo = empleados.id;
                    }

                try
                {
                    //quita cualquier validacion del modelo
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Print(e.Message);
                }
                finally
                {
                    db.Configuration.ValidateOnSaveEnabled = true;
                }

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");

            }

            //determina las areas a mostrar
            var listArea = db.Area.Where(p => p.activo == true && !p.shared_services && p.plantaClave == empleados.planta_clave);
            if (empleados.shared_services)
                listArea = db.Area.Where(p => p.activo == true && p.shared_services);

            ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", selectedValue: empleados.planta_clave.ToString());
            ViewBag.id_area = new SelectList(listArea, "clave", "descripcion", selectedValue: empleados.id_area.ToString());
            ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true && p.areaClave == empleados.id_area), "clave", "descripcion", selectedValue: empleados.puesto.ToString());
            ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
            textoPorDefecto: "-- Seleccione un valor --", selected: empleados.id_jefe_directo.ToString());

            ViewBag.TotalEmpleados = db.empleados.Where(x => x.activo == true).ToList();
            List<int> SubordinadosSeleccionados = new List<int>();
            if (subordinados != null)
                SubordinadosSeleccionados = subordinados.ToList();
            ViewBag.SubordinadosSeleccionados = SubordinadosSeleccionados;


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

                //verifica si es shared services
                empleados.shared_services = empleados.Area != null && empleados.Area.shared_services;

                //determina las areas a mostrar
                var listArea = db.Area.Where(p => p.activo == true && !p.shared_services && p.plantaClave == empleados.planta_clave);
                if (empleados.shared_services)
                    listArea = db.Area.Where(p => p.activo == true && p.shared_services);

                ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", selectedValue: empleados.planta_clave.ToString());
                ViewBag.id_area = new SelectList(listArea, "clave", "descripcion", selectedValue: empleados.id_area.ToString());
                ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true && p.areaClave == empleados.id_area), "clave", "descripcion", selectedValue: empleados.puesto.ToString());
                ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
                textoPorDefecto: "-- Seleccione un valor --", selected: empleados.id_jefe_directo.ToString());

                ViewBag.TotalEmpleados = db.empleados.Where(x => x.activo == true).ToList();

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
        public async Task<ActionResult> Edit(empleados empleados, FormCollection collection, bool elimina_documento, int[] subordinados)
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
            if (!Int32.TryParse(empleados.numeroEmpleado, out int num) && empleados.numeroEmpleado.ToUpper() != "N/A" && empleados.numeroEmpleado.ToUpper() != "P99999")
                ModelState.AddModelError("", "El número de empleado no es válido. Ingrese sólo números o los valores \"N/A\" ó P99999 ");

            //busca si ya existe un empleado con ese numero de empleado

            // existe el num empleado
            if (db.empleados.Any(s => !string.IsNullOrEmpty(empleados.numeroEmpleado) && empleados.numeroEmpleado.ToUpper() != "N/A" && s.numeroEmpleado == empleados.numeroEmpleado && s.id != empleados.id))
                ModelState.AddModelError("", "Ya existe un registro con el mismo número de empleado. ");


            if (ModelState.IsValid)
            {
                #region subordinados
                //borra todos los subordinados del empleado
                var subs = db.empleados.Where(x => x.id_jefe_directo == empleados.id).Select(x => x.id).ToList();
                List<empleados> subordinadosAnteriores = new List<empleados>();
                foreach (var s in subs)
                {
                    var emp = db.empleados.Find(s);
                    //  subordinadosAnteriores.Add(emp);
                    emp.id_jefe_directo = null;

                    try
                    {
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.Print(e.Message);
                    }
                    finally
                    {
                        db.Configuration.ValidateOnSaveEnabled = true;
                        db.Entry(emp).State = EntityState.Detached;
                    }
                }

                //actualiza los subordinados
                if (subordinados != null)
                    for (int i = 0; i < subordinados.Length; i++)
                    {
                        var subordinado = db.empleados.Find(subordinados[i]);
                        subordinado.id_jefe_directo = empleados.id;

                        if (subordinado.id == empleados.id)
                            empleados.id_jefe_directo = empleados.id;

                        try
                        {
                            db.Configuration.ValidateOnSaveEnabled = false;
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Print(e.Message);
                        }
                        finally
                        {
                            db.Entry(subordinado).State = EntityState.Detached;
                            db.Configuration.ValidateOnSaveEnabled = true;
                        }


                    }

                #endregion 

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
                if (string.IsNullOrEmpty(empleados.apellido2))
                    empleados.apellido2 = null;
                else
                    empleados.apellido2 = empleados.apellido2.ToUpper();
                empleados.numeroEmpleado = empleados.numeroEmpleado.ToUpper(); //para n/a

                //agrega el id_area
                if (c_area > 0)
                    empleados.id_area = c_area;

                db.Entry(empleados).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                }
                catch (Exception ex)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Error: " + ex.Message, TipoMensajesSweetAlerts.ERROR);
                    ModelState.AddModelError("", "Error al guardar en BD_ " + ex.Message);
                }

                return RedirectToAction("Index");

            }

            //determina las areas a mostrar
            var listArea = db.Area.Where(p => p.activo == true && !p.shared_services && p.plantaClave == empleados.planta_clave);
            if (empleados.shared_services)
                listArea = db.Area.Where(p => p.activo == true && p.shared_services);

            ViewBag.planta_clave = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", selectedValue: empleados.planta_clave.ToString());
            ViewBag.id_area = new SelectList(listArea, "clave", "descripcion", selectedValue: empleados.id_area.ToString());
            ViewBag.puesto = new SelectList(db.puesto.Where(p => p.activo == true && p.areaClave == empleados.id_area), "clave", "descripcion", selectedValue: empleados.puesto.ToString());
            ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
            textoPorDefecto: "-- Seleccione un valor --", selected: empleados.id_jefe_directo.ToString());
            ViewBag.TotalEmpleados = db.empleados.Where(x => x.activo == true).ToList();


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
                FileName = "Reporte_empleados_tkmm_8ID_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

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

                //verifica si es shared services
                empleado.shared_services = empleado.Area != null && empleado.Area.shared_services;

                ViewBag.id_nuevo_jefe = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)),
                    textoPorDefecto: "-- Seleccione un valor --");
                ViewBag.subordinados = empleado.empleados1.ToList();
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
        public ActionResult DisableConfirmed(int id, FormCollection collection, int[] subordinados, int id_nuevo_jefe = 0)
        {
            empleados empleado = db.empleados.Find(id);
            //empleado.activo = false;
            var solicitante = obtieneEmpleadoLogeado(); 

            DateTime bajaFecha = DateTime.Now;
            string stringFecha = collection["bajaFecha"];

            //bool notificacionIT = true;
            //if (collection.AllKeys.Any(x => x == "notificacion_it"))
            //    notificacionIT = true;

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

            //deshabilita del lado del servidor la validacion 
            db.Configuration.ValidateOnSaveEnabled = false;

            if (ModelState.IsValid)
            {
                try
                {
                    //realiza cambios en la BD
                    if (subordinados != null && subordinados.Length > 0)
                    {
                        var listEmpleados = db.empleados.Where(x => subordinados.Contains(x.id));
                        foreach (var item in listEmpleados)
                        {
                            item.id_jefe_directo = id_nuevo_jefe;
                        }
                    }

                    //crea la solicitud en el sistema
                    IT_matriz_requerimientos matriz = new IT_matriz_requerimientos()
                    {
                        id_empleado = empleado.id,
                        id_solicitante = solicitante.id,
                        id_jefe_directo = empleado.empleados2.id,
                        estatus = Bitacoras.Util.IT_MR_Status.ENVIADO_A_JEFE,
                        fecha_solicitud = DateTime.Now,
                        comentario_rechazo = null,
                        fecha_aprobacion_jefe = null,   
                        id_internet_tipo = 1,
                        tipo = IT_MR_tipo.BAJA
                    };

                    db.IT_matriz_requerimientos.Add(matriz);

                    db.SaveChanges();

                    //OBTIENE EL CORREO DE NOTIFICACION
                    //var itTicketEmail = db.notificaciones_correo.Where(x => x.descripcion == "IT_TICKET").FirstOrDefault();
                    //var itEmail = db.notificaciones_correo.Where(x => x.descripcion == "IT_TKMM").FirstOrDefault();
                    if (!string.IsNullOrEmpty(empleado.correo))
                    {
                        //envia correo electronico
                        EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                        List<String> correos = new List<string>
                        {
                            empleado.correo //agrega correo de jefe
                        }; //correos TO

                        //manda copia al usuario actual
                        //empleados empleadoActualRH = obtieneEmpleadoLogeado();
                        //if (!String.IsNullOrEmpty(empleadoActualRH.correo))
                        //    correos.Add(empleadoActualRH.correo);

                        envioCorreo.SendEmailAsync(correos, "TKMM-LOCAL - Notificación de Baja de Empleado", envioCorreo.getBodyITBajaEmpleado(empleado));
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
                //verifica si es shared services
                empleado.shared_services = empleado.Area != null && empleado.Area.shared_services;

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
