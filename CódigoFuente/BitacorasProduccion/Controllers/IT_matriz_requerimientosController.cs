using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_matriz_requerimientosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_matriz_requerimientos/ListadoUsuarios
        public ActionResult ListadoUsuarios(string nombre, string num_empleado, int planta_clave = 0, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_CREAR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                var listado = db.empleados
                       .Where(x =>
                       ((x.nombre + " " + x.apellido1 + " " + x.apellido2).Contains(nombre) || String.IsNullOrEmpty(nombre))
                       && (x.numeroEmpleado.Contains(num_empleado) || String.IsNullOrEmpty(num_empleado))
                       && (x.planta_clave == planta_clave || planta_clave == 0)
                       && x.activo == true
                       )
                       .OrderBy(x => x.id)
                       .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                      .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.empleados
                      .Where(x =>
                        ((x.nombre + " " + x.apellido1 + " " + x.apellido2).Contains(nombre) || String.IsNullOrEmpty(nombre))
                          && (x.numeroEmpleado.Contains(num_empleado) || String.IsNullOrEmpty(num_empleado))
                       && (x.planta_clave == planta_clave || planta_clave == 0)
                       && x.activo == true
                       )
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["nombre"] = nombre;
                routeValues["planta_clave"] = planta_clave;
                routeValues["num_empleado"] = num_empleado;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.planta_clave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", planta_clave.ToString()), textoPorDefecto: "-- Todas --");
                ViewBag.Paginacion = paginacion;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        #region listado usuarios
        // GET: IT_matriz_requerimientos/SolicitudesEnProceso
        public ActionResult SolicitudesEnProceso(int pagina = 1)
        {

            if (TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_CREAR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listado = db.IT_matriz_requerimientos
                    .Where(x => (x.estatus == IT_MR_Status.ENVIADO_A_JEFE || x.estatus == IT_MR_Status.ENVIADO_A_IT || x.estatus == IT_MR_Status.CREADO || x.estatus == IT_MR_Status.EN_PROCESO))
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_matriz_requerimientos
                    .Where(x => (x.estatus == IT_MR_Status.ENVIADO_A_JEFE || x.estatus == IT_MR_Status.ENVIADO_A_IT || x.estatus == IT_MR_Status.CREADO || x.estatus == IT_MR_Status.EN_PROCESO))
                   .Count();

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
                ViewBag.Title = "Listado de Solicitudes en Proceso";
                ViewBag.PrimerNivel = "recursos_humanos";
                ViewBag.SegundoNivel = "SolicitudesEnProceso";
                ViewBag.Create = true;



                return View("ListadoSolicitudes", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: IT_matriz_requerimientos/SolicitudesRechazadas
        public ActionResult SolicitudesRechazadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_CREAR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listado = db.IT_matriz_requerimientos
                    .Where(x => (x.estatus == IT_MR_Status.RECHAZADO))
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_matriz_requerimientos
                   .Where(x => (x.estatus == IT_MR_Status.RECHAZADO))
                   .Count();

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
                ViewBag.Edit = true;
                ViewBag.Title = "Listado de Solicitudes Rechazadas";
                ViewBag.PrimerNivel = "recursos_humanos";
                ViewBag.SegundoNivel = "SolicitudesRechazadas";
                ViewBag.Create = true;



                return View("ListadoSolicitudes", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: IT_matriz_requerimientos/SolicitudesFinalizadas
        public ActionResult SolicitudesFinalizadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_CREAR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listado = db.IT_matriz_requerimientos
                    .Where(x => (x.estatus == IT_MR_Status.FINALIZADO))
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_matriz_requerimientos
                   .Where(x => (x.estatus == IT_MR_Status.FINALIZADO))
                   .Count();

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
                ViewBag.Edit = true;
                ViewBag.Title = "Listado de Solicitudes Finalizadas";
                ViewBag.PrimerNivel = "recursos_humanos";
                ViewBag.SegundoNivel = "SolicitudesFinalizadas";
                ViewBag.Create = true;



                return View("ListadoSolicitudes", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        #endregion

        #region listados autorizador
        // GET: IT_matriz_requerimientos/solicitudes_pendientes_autorizador
        public ActionResult solicitudes_pendientes_autorizador(int pagina = 1)
        {

            if (TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_AUTORIZAR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.IT_matriz_requerimientos
                    .Where(x => x.estatus == IT_MR_Status.ENVIADO_A_JEFE && x.id_jefe_directo == empleado.id)
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_matriz_requerimientos
                     .Where(x => x.estatus == IT_MR_Status.ENVIADO_A_JEFE && x.id_jefe_directo == empleado.id)
                   .Count();

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
                ViewBag.Title = "Listado de Solicitudes Pendientes";
                ViewBag.PrimerNivel = "autorizar_matriz";
                ViewBag.SegundoNivel = "solicitudes_pendientes_autorizador";
                ViewBag.Autorizar = true;



                return View("ListadoSolicitudes", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: IT_matriz_requerimientos/solicitudes_rechazadas_autorizador
        public ActionResult solicitudes_rechazadas_autorizador(int pagina = 1)
        {

            if (TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_AUTORIZAR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.IT_matriz_requerimientos
                    .Where(x => x.estatus == IT_MR_Status.RECHAZADO && x.id_jefe_directo == empleado.id)
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_matriz_requerimientos
                     .Where(x => x.estatus == IT_MR_Status.RECHAZADO && x.id_jefe_directo == empleado.id)
                   .Count();

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
                ViewBag.Title = "Listado de Solicitudes Pendientes";
                ViewBag.PrimerNivel = "autorizar_matriz";
                ViewBag.SegundoNivel = "solicitudes_rechazadas_autorizador";

                return View("ListadoSolicitudes", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: IT_matriz_requerimientos/solicitudes_autorizadas_autorizador
        public ActionResult solicitudes_autorizadas_autorizador(int pagina = 1)
        {

            if (TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_AUTORIZAR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.IT_matriz_requerimientos
                    .Where(x => (x.estatus == IT_MR_Status.ENVIADO_A_IT || x.estatus == IT_MR_Status.FINALIZADO || x.estatus == IT_MR_Status.EN_PROCESO) && x.id_jefe_directo == empleado.id)
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_matriz_requerimientos
                           .Where(x => (x.estatus == IT_MR_Status.ENVIADO_A_IT || x.estatus == IT_MR_Status.FINALIZADO || x.estatus == IT_MR_Status.EN_PROCESO) && x.id_jefe_directo == empleado.id)
                   .Count();

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
                ViewBag.Title = "Listado de Solicitudes Pendientes";
                ViewBag.PrimerNivel = "autorizar_matriz";
                ViewBag.SegundoNivel = "solicitudes_autorizadas_autorizador";

                return View("ListadoSolicitudes", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        #endregion

        #region listados sistemas

        // GET: IT_matriz_requerimientos/solicitudes_sistemas
        public ActionResult solicitudes_sistemas(string estatus, string nombre, int pagina = 1)
        {

            if (TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_CERRAR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.IT_matriz_requerimientos
                    .Where(x => (String.IsNullOrEmpty(estatus) || x.estatus.Contains(estatus)) && ((x.empleados.nombre + " " + x.empleados.apellido1 + " " + x.empleados.apellido2).Contains(nombre) || String.IsNullOrEmpty(nombre)))
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_matriz_requerimientos
                      .Where(x => (String.IsNullOrEmpty(estatus) || x.estatus.Contains(estatus)) && ((x.empleados.nombre + " " + x.empleados.apellido1 + " " + x.empleados.apellido2).Contains(nombre) || String.IsNullOrEmpty(nombre)))
                   .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["estatus"] = estatus;
                routeValues["nombre"] = nombre;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                List<string> estatusList = db.IT_matriz_requerimientos.Select(x => x.estatus).Distinct().ToList();
                //crea un Select  list para el estatus
                List<SelectListItem> newList = new List<SelectListItem>();

                foreach (string statusItem in estatusList)
                {
                    newList.Add(new SelectListItem()
                    {
                        Text = IT_MR_Status.DescripcionStatus(statusItem),
                        Value = statusItem
                    });
                }

                SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
                ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Todos --");

                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones
                ViewBag.Details = true;
                ViewBag.Sistemas = true;
                ViewBag.Title = "Listado de Solicitudes";
                ViewBag.PrimerNivel = "sistemas";
                ViewBag.SegundoNivel = "solicitudes_usuarios_sistemas";

                return View("ListadoSolicitudes", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }



        #endregion

        // GET: IT_matriz_requerimientos/Details
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_CREAR) || TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_DETALLES))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_matriz_requerimientos matriz = db.IT_matriz_requerimientos.Find(id);
                if (matriz == null)
                {
                    return View("../Error/NotFound");

                }

                ViewBag.listHardware = db.IT_hardware_tipo.Where(x => x.activo == true).ToList();
                ViewBag.listSoftware = db.IT_software_tipo.Where(x => x.activo == true).ToList();
                ViewBag.listComunicaciones = db.IT_comunicaciones_tipo.Where(x => x.activo == true).ToList();
                ViewBag.listCarpetas = db.IT_carpetas_red.Where(x => x.activo == true).ToList();

                return View(matriz);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: IT_matriz_requerimientos/CrearMatriz
        public ActionResult CrearMatriz(int? id)
        {
            if (TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_CREAR))
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

                IT_matriz_requerimientos matriz = db.IT_matriz_requerimientos.Where(x => x.id_empleado == id).FirstOrDefault();
                if (matriz == null)
                {
                    //crea una matriz nueva con el empleado asociado
                    matriz = new IT_matriz_requerimientos
                    {
                        id_empleado = empleados.id,
                        empleados = empleados,
                    };

                }
                else
                {
                    //verifica si una matriz puede editarse
                    if (matriz.estatus != IT_MR_Status.RECHAZADO && matriz.estatus != IT_MR_Status.FINALIZADO)
                    {
                        ViewBag.Titulo = "¡Lo sentimos!¡No se puede modificar esta solicitud!";
                        ViewBag.Descripcion = "Sólo pueden modificarse solicitudes que han sido Rechazadas o Finalizadas.";

                        return View("../Home/ErrorGenerico");
                    }
                }



                //obtiene la lista de hardware
                ViewBag.listHardware = db.IT_hardware_tipo.Where(x => x.activo == true).ToList();
                ViewBag.listSoftware = db.IT_software_tipo.Where(x => x.activo == true).ToList();
                ViewBag.id_internet_tipo = AddFirstItem(new SelectList(db.IT_internet_tipo.Where(p => p.activo == true), "id", "descripcion"), selected: matriz.id_internet_tipo.ToString());
                ViewBag.listCarpetas = db.IT_carpetas_red.Where(x => x.activo == true).ToList();
                ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(p => p.activo == true), "id", "ConcatNumEmpleadoNombre"), selected: matriz.id_jefe_directo.ToString());
                ViewBag.listComunicaciones = db.IT_comunicaciones_tipo.Where(x => x.activo == true).ToList();
                return View(matriz);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }



        // POST: IT_matriz_requerimientos/CrearMatriz
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearMatriz(IT_matriz_requerimientos matriz, FormCollection collection, string[] SelectedHardware, string[] SelectedSoftware, string[] SelectedComunicaciones, string[] SelectedCarpetas)
        {

            //obtien el id de Matriz
            int.TryParse(collection["id_matriz"], out int id_matriz);
            matriz.id = id_matriz;

            string statusAnterior = matriz.estatus;

            //lista de key del collection
            List<string> keysCollection = collection.AllKeys.ToList();

            #region Asignación de Objetos

            //crea los objetos para hardware
            if (SelectedHardware != null)
                foreach (string id_hardware_string in SelectedHardware)
                {
                    //obtiene el id
                    Match m = Regex.Match(id_hardware_string, @"\d+");
                    int id_hardware = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_hardware);

                    string keyDescription = "hardware_" + id_hardware + "_descripcion";
                    String descripcionHardware = null;

                    //busca si existe una descripcion
                    if (keysCollection.Contains(keyDescription))
                        descripcionHardware = collection[keyDescription];

                    matriz.IT_matriz_hardware.Add(new IT_matriz_hardware { id_matriz_requerimientos = id_matriz, id_it_hardware = id_hardware, descripcion = descripcionHardware });
                }

            //crea los objetos para software
            if (SelectedSoftware != null)
                foreach (string id_software_string in SelectedSoftware)
                {
                    //obtiene el id
                    Match m = Regex.Match(id_software_string, @"\d+");
                    int id_software = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_software);

                    string keyDescription = "software_" + id_software + "_descripcion";
                    String descripcionSoftware = null;

                    if (keysCollection.Contains(keyDescription))
                        descripcionSoftware = collection[keyDescription];

                    matriz.IT_matriz_software.Add(new IT_matriz_software { id_matriz_requerimientos = id_matriz, id_it_software = id_software, descripcion = descripcionSoftware });
                }

            //crea los objetos para comunicaciones
            if (SelectedComunicaciones != null)
                foreach (string id_comunicaciones_string in SelectedComunicaciones)
                {
                    //obtiene el id
                    Match m = Regex.Match(id_comunicaciones_string, @"\d+");
                    int id_comunicaciones = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_comunicaciones);

                    string keyDescription = "comunicaciones_" + id_comunicaciones + "_descripcion";
                    String descripcionComunicaciones = null;

                    if (keysCollection.Contains(keyDescription))
                        descripcionComunicaciones = collection[keyDescription];

                    matriz.IT_matriz_comunicaciones.Add(new IT_matriz_comunicaciones { id_matriz_requerimientos = id_matriz, id_it_comunicaciones = id_comunicaciones, descripcion = descripcionComunicaciones });
                }

            //crea los objetos para las carpetas
            if (SelectedCarpetas != null)
                foreach (string id_carpetas_string in SelectedCarpetas)
                {
                    //obtiene el id
                    Match m = Regex.Match(id_carpetas_string, @"\d+");
                    int id_carpetas = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_carpetas);

                    string keyDescription = "carpetas_" + id_carpetas + "_descripcion";
                    String descripcionCarpetas = null;

                    if (keysCollection.Contains(keyDescription))
                        descripcionCarpetas = collection[keyDescription];

                    matriz.IT_matriz_carpetas.Add(new IT_matriz_carpetas { id_matriz_requerimientos = id_matriz, id_it_carpeta_red = id_carpetas, descripcion = descripcionCarpetas });
                }
            #endregion

            if (ModelState.IsValid)
            {
                empleados solicitante = obtieneEmpleadoLogeado();

                //campos obligatorios 
                matriz.fecha_solicitud = DateTime.Now;
                matriz.estatus = IT_MR_Status.ENVIADO_A_JEFE;
                matriz.id_solicitante = solicitante.id;

                string mensaje = "Se ha enviado la solicitud correctamente.";
                TipoMensajesSweetAlerts tipoMensaje = TipoMensajesSweetAlerts.SUCCESS;

                //si existe un registro con el mismo id empleado 
                if (!db.IT_matriz_requerimientos.Any(x => x.id_empleado == matriz.id_empleado))
                {
                    db.IT_matriz_requerimientos.Add(matriz);
                }
                else
                { //se trata de una modificación o rechazo
                  //si existe lo modifica
                    IT_matriz_requerimientos matrizOld = db.IT_matriz_requerimientos.Find(id_matriz);
                    //borra valores en caso de que sea una solicitud de cambio (solicitud ya finalizada)
                    matriz.fecha_aprobacion_jefe = null;
                    matriz.fecha_cierre = null;
                    if (statusAnterior == IT_MR_Status.FINALIZADO)
                        matriz.comentario_rechazo = null;


                    //borra los conceltos anteriornes
                    db.IT_matriz_software.RemoveRange(matrizOld.IT_matriz_software);
                    db.IT_matriz_hardware.RemoveRange(matrizOld.IT_matriz_hardware);
                    db.IT_matriz_carpetas.RemoveRange(matrizOld.IT_matriz_carpetas);
                    db.IT_matriz_comunicaciones.RemoveRange(matrizOld.IT_matriz_comunicaciones);

                    //agrega los nuevos conceptos
                    db.IT_matriz_software.AddRange(matriz.IT_matriz_software);
                    db.IT_matriz_hardware.AddRange(matriz.IT_matriz_hardware);
                    db.IT_matriz_carpetas.AddRange(matriz.IT_matriz_carpetas);
                    db.IT_matriz_comunicaciones.AddRange(matriz.IT_matriz_comunicaciones);

                    //establece los valores principales
                    db.Entry(matrizOld).CurrentValues.SetValues(matriz);

                    db.Entry(matrizOld).State = EntityState.Modified;
                }

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    mensaje = "Error al guardar en BD.";
                    tipoMensaje = TipoMensajesSweetAlerts.ERROR;
                    EscribeExcepcion(ex, Clases.Models.EntradaRegistroEvento.TipoEntradaRegistroEvento.Error);
                }

                try
                {
                    //envia correo electronico
                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                    List<String> correos = new List<string>(); //correos TO

                    //obtiene el empleado asociado
                    empleados emp = db.empleados.Find(matriz.id_jefe_directo);

                    if (emp != null && !String.IsNullOrEmpty(emp.correo))
                        correos.Add(emp.correo); //agrega correo de validador

                    //agrega las referencias al empleados y empleados2
                    matriz.empleados = db.empleados.Find(matriz.id_empleado);
                    matriz.empleados3 = solicitante;

                    envioCorreo.SendEmailAsync(correos, "Ha recibido una Solicitud de Requerimiento de Usuario para su aprobación.", envioCorreo.getBody_IT_MR_Notificacion_Jefe_Directo(matriz));
                }
                catch (Exception e)
                {
                    mensaje = "Se ha enviado correctamente la solicitud, pero ha surgido un error al mandar el correo electrónico.";
                    tipoMensaje = TipoMensajesSweetAlerts.WARNING;
                    EscribeExcepcion(e, Clases.Models.EntradaRegistroEvento.TipoEntradaRegistroEvento.Error);
                }

                TempData["Mensaje"] = new MensajesSweetAlert(mensaje, tipoMensaje);

                return RedirectToAction("ListadoUsuarios");
            }

            empleados empleados = db.empleados.Find(matriz.id_empleado);

            matriz.empleados = empleados;


            ViewBag.listHardware = db.IT_hardware_tipo.Where(x => x.activo == true).ToList();
            ViewBag.listSoftware = db.IT_software_tipo.Where(x => x.activo == true).ToList();
            ViewBag.id_internet_tipo = AddFirstItem(new SelectList(db.IT_internet_tipo.Where(p => p.activo == true), "id", "descripcion"));
            ViewBag.listComunicaciones = db.IT_comunicaciones_tipo.Where(x => x.activo == true).ToList();
            ViewBag.listCarpetas = db.IT_carpetas_red.Where(x => x.activo == true).ToList();
            ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(p => p.activo == true), "id", "ConcatNumEmpleadoNombre"));
            return View(matriz);

        }

        #region Autorizador

        // GET: IT_matriz_requerimientos/Autorizar
        public ActionResult Autorizar(int? id)
        {
            if (TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_AUTORIZAR))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_matriz_requerimientos matriz = db.IT_matriz_requerimientos.Find(id);
                if (matriz == null)
                {
                    return View("../Error/NotFound");
                }
                //verifica si el id_jefe pertenece al usuario que inició sesión
                empleados emp = obtieneEmpleadoLogeado();

                if (emp.id != matriz.id_jefe_directo)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede autorizar esta solicitud!";
                    ViewBag.Descripcion = "No se puede autorizar una solicitud a la que no se encuentra asignado.";

                    return View("../Home/ErrorGenerico");
                }


                ViewBag.listHardware = db.IT_hardware_tipo.Where(x => x.activo == true).ToList();
                ViewBag.listSoftware = db.IT_software_tipo.Where(x => x.activo == true).ToList();
                ViewBag.listComunicaciones = db.IT_comunicaciones_tipo.Where(x => x.activo == true).ToList();
                ViewBag.listCarpetas = db.IT_carpetas_red.Where(x => x.activo == true).ToList();

                return View(matriz);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: PremiumFreightAproval/ValidarAreaPM/5
        [HttpPost, ActionName("Autorizar")]
        [ValidateAntiForgeryToken]
        public ActionResult AutorizarConfirmed(FormCollection collection)
        {

            int id = 0;
            if (!String.IsNullOrEmpty(collection["id"]))
                Int32.TryParse(collection["id"], out id);


            IT_matriz_requerimientos matriz = db.IT_matriz_requerimientos.Find(id);
            matriz.estatus = IT_MR_Status.ENVIADO_A_IT;
            matriz.fecha_aprobacion_jefe = DateTime.Now;
            // poliza.id_autorizador = Convert.ToInt32(collection["id_autorizador"]);

            db.Entry(matriz).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //NOTIFICACIÓN A SOLICITANTE
                //envia correo electronico notificación solicitante
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                if (!String.IsNullOrEmpty(matriz.empleados3.correo))
                    correos.Add(matriz.empleados3.correo); //agrega correo de elaborador

                envioCorreo.SendEmailAsync(correos, "La Solicitud de Requerimientos de usuarios #" + matriz.id + " ha sido válidada por el Jefe Directo.", envioCorreo.getBody_IT_MR_Notificacion_Autorizado_Solicitante(matriz));

                //se restablece el listado de correos
                correos = new List<string>();

                //NOTIFICACION A SISTEMAS (Todos los que tengan el permiso de cerrar)

                //---INICIO POR ROL                    
                //recorre los usuarios con el permiso de cerrar
                AspNetRoles rol = db.AspNetRoles.Where(x => x.Name == TipoRoles.IT_MATRIZ_REQUERIMIENTOS_CERRAR).FirstOrDefault();
                List<AspNetUsers> usuariosInRole = new List<AspNetUsers>();
                if (rol != null)
                    usuariosInRole = rol.AspNetUsers.ToList();

                List<int> idsCerrar = usuariosInRole.Select(x => x.IdEmpleado).Distinct().ToList();

                List<empleados> listEmpleados = db.empleados.Where(x => x.activo == true && idsCerrar.Contains(x.id) == true).ToList();

                foreach (var e in listEmpleados)
                    if (!String.IsNullOrEmpty(e.correo))
                        correos.Add(e.correo);

                //---FIN POR ROL

                envioCorreo.SendEmailAsync(correos, "Se ha recibido una Solicitud de Requerimientos. Folio: " + matriz.id, envioCorreo.getBody_IT_MR_Notificacion_Autorizado_Sistemas(matriz));

                TempData["Mensaje"] = new MensajesSweetAlert("Se ha autorizado la solicitud correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("solicitudes_pendientes_autorizador");

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
                return RedirectToAction("solicitudes_pendientes_autorizador");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("solicitudes_pendientes_autorizador");
            }

        }

        // POST: IT_matriz_requerimientos/RechazarConfirmed/5
        [HttpPost, ActionName("Rechazar")]
        [ValidateAntiForgeryToken]
        public ActionResult RechazarConfirmed(FormCollection collection)
        {

            int id = 0;
            if (!String.IsNullOrEmpty(collection["id"]))
                Int32.TryParse(collection["id"], out id);

            String razonRechazo = collection["comentario_rechazo"];

            IT_matriz_requerimientos matriz = db.IT_matriz_requerimientos.Find(id);
            matriz.estatus = IT_MR_Status.RECHAZADO;
            matriz.comentario_rechazo = razonRechazo;

            db.Entry(matriz).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                //NOTIFICACIÓN A SOLICITANTE
                //envia correo electronico notificación solicitante
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                List<String> correos = new List<string>(); //correos TO

                if (!String.IsNullOrEmpty(matriz.empleados3.correo))
                    correos.Add(matriz.empleados3.correo); //agrega correo de elaborador

                envioCorreo.SendEmailAsync(correos, "La Solicitud de Requerimientos de usuarios #" + matriz.id + " ha sido rechazada por el Jefe Directo.", envioCorreo.getBody_IT_MR_Notificacion_Rechazado_Solicitante(matriz));


                TempData["Mensaje"] = new MensajesSweetAlert("Se ha rechazado la solicitud correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("solicitudes_pendientes_autorizador");


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
                return RedirectToAction("solicitudes_pendientes_autorizador");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("solicitudes_pendientes_autorizador");
            }

        }

        #endregion

        #region sistemas
        // GET: IT_matriz_requerimientos/Cerrar
        public ActionResult Cerrar(int? id)
        {
            if (TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_CERRAR))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_matriz_requerimientos matriz = db.IT_matriz_requerimientos.Find(id);
                if (matriz == null)
                {
                    return View("../Error/NotFound");
                }


                return View(new IT_matriz_requerimientosCerrarModel { matriz = matriz, id = matriz.id, correo = matriz.empleados.correo, C8ID = matriz.empleados.C8ID, comentario_cierre = matriz.comentario_cierre });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_matriz_requerimientos/Cerrar
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cerrar(IT_matriz_requerimientosCerrarModel matrizModel, FormCollection collection)
        {
            IT_matriz_requerimientos matriz = db.IT_matriz_requerimientos.Find(matrizModel.id);

            //lista de key del collection
            List<string> keysCollection = collection.AllKeys.ToList();

            string tipoSolicitud = collection["tipo_form"];

            #region Asignación de Objetos
          
            //crea los objetos para hardware
            foreach (string hardware_string in keysCollection.Where(x => x.Contains("hardware") && x.Contains("estado")))
            {
                //obtiene el id
                Match m = Regex.Match(hardware_string, @"\d+");
                int id_hardware = 0;

                if (m.Success)//si tiene un numero                
                    int.TryParse(m.Value, out id_hardware);

                //obtiene el estado
                bool completado = Convert.ToBoolean(collection["hardware_" + id_hardware + "_estado"]);
                string comentario = null;

                //comentario
                if (keysCollection.Contains("hardware_" + id_hardware + "_comentarios") && !String.IsNullOrEmpty(collection["hardware_" + id_hardware + "_comentarios"]))
                     comentario = collection["hardware_" + id_hardware + "_comentarios"];

                //obtiene el objeto asociado
                IT_matriz_hardware item = matriz.IT_matriz_hardware.Where(x => x.id == id_hardware).FirstOrDefault();

                if (item != null)
                {
                    item.completado = completado;
                    item.comentario = comentario;                        
                }
            }

            //crea los objetos para software
            foreach (string software_string in keysCollection.Where(x => x.Contains("software") && x.Contains("estado")))
            {
                //obtiene el id
                Match m = Regex.Match(software_string, @"\d+");
                int id_software = 0;

                if (m.Success)//si tiene un numero                
                    int.TryParse(m.Value, out id_software);

                //obtiene el estado
                bool completado = Convert.ToBoolean(collection["software_" + id_software + "_estado"]);
                string comentario = null;

                //comentario
                if (keysCollection.Contains("software_" + id_software + "_comentarios") && !String.IsNullOrEmpty(collection["software_" + id_software + "_comentarios"]))
                    comentario = collection["software_" + id_software + "_comentarios"];

                //obtiene el objeto asociado
                IT_matriz_software item = matriz.IT_matriz_software.Where(x => x.id == id_software).FirstOrDefault();

                if (item != null)
                {
                    item.completado = completado;
                    item.comentario = comentario;
                }
            }

            //crea los objetos para comunicaciones
            foreach (string comunicaciones_string in keysCollection.Where(x => x.Contains("comunicaciones") && x.Contains("estado")))
            {
                //obtiene el id
                Match m = Regex.Match(comunicaciones_string, @"\d+");
                int id_comunicaciones = 0;

                if (m.Success)//si tiene un numero                
                    int.TryParse(m.Value, out id_comunicaciones);

                //obtiene el estado
                bool completado = Convert.ToBoolean(collection["comunicaciones_" + id_comunicaciones + "_estado"]);
                string comentario = null;

                //comentario
                if (keysCollection.Contains("comunicaciones_" + id_comunicaciones + "_comentarios") && !String.IsNullOrEmpty(collection["comunicaciones_" + id_comunicaciones + "_comentarios"]))
                    comentario = collection["comunicaciones_" + id_comunicaciones + "_comentarios"];

                //obtiene el objeto asociado
                IT_matriz_comunicaciones item = matriz.IT_matriz_comunicaciones.Where(x => x.id == id_comunicaciones).FirstOrDefault();

                if (item != null)
                {
                    item.completado = completado;
                    item.comentario = comentario;
                }
            }

            //crea los objetos para las carpetas
            foreach (string carpetas_string in keysCollection.Where(x => x.Contains("carpetas") && x.Contains("estado")))
            {
                //obtiene el id
                Match m = Regex.Match(carpetas_string, @"\d+");
                int id_carpetas = 0;

                if (m.Success)//si tiene un numero                
                    int.TryParse(m.Value, out id_carpetas);

                //obtiene el estado
                bool completado = Convert.ToBoolean(collection["carpetas_" + id_carpetas + "_estado"]);
                string comentario = null;

                //comentario
                if (keysCollection.Contains("carpetas_" + id_carpetas + "_comentarios") && !String.IsNullOrEmpty(collection["carpetas_" + id_carpetas + "_comentarios"]))
                    comentario = collection["carpetas_" + id_carpetas + "_comentarios"];

                //obtiene el objeto asociado
                IT_matriz_carpetas item = matriz.IT_matriz_carpetas.Where(x => x.id == id_carpetas).FirstOrDefault();

                if (item != null)
                {
                    item.completado = completado;
                    item.comentario = comentario;
                }
            }


            #endregion

            //verifica 8ID y correo
            if(db.empleados.Any(x=> x.correo == matrizModel.correo && x.id != matriz.id_empleado) && !String.IsNullOrEmpty(matrizModel.correo))
                ModelState.AddModelError("", "Ya existe un registro con el mismo correo electrónico.");

            if (db.empleados.Any(x => x.C8ID == matrizModel.C8ID && x.id != matriz.id_empleado) && !String.IsNullOrEmpty(matrizModel.C8ID))
                ModelState.AddModelError("", "Ya existe un registro con el mismo 8ID.");


            if (ModelState.IsValid)
            {
                string mensaje = "Se ha enviado la solicitud correctamente.";
                TipoMensajesSweetAlerts tipoMensaje = TipoMensajesSweetAlerts.SUCCESS;

                // 1. actualiza matriz
                empleados sistemas = obtieneEmpleadoLogeado();

                //campos obligatorios                
                matriz.id_sistemas = sistemas.id;
                matriz.comentario_cierre = matrizModel.comentario_cierre;

                matriz.empleados.correo = matrizModel.correo;
                matriz.empleados.C8ID = matrizModel.C8ID;

                //actualiza el estado de la solicitud según el tipo de formulario enviado
                switch (tipoSolicitud.ToUpper()) {
                    case "CIERRE":
                        matriz.estatus = IT_MR_Status.FINALIZADO;
                        matriz.fecha_cierre = DateTime.Now;
                        break;
                    case "PROGRESO":
                        matriz.estatus = IT_MR_Status.EN_PROCESO;
                        break;
                    default:
                        matriz.estatus = IT_MR_Status.ENVIADO_A_IT;
                        break;
                }

                AspNetUsers user = db.AspNetUsers.Where(x => x.IdEmpleado == matriz.empleados.id).FirstOrDefault();
                if (user != null) {
                    user.Email = matrizModel.correo;
                    db.Entry(user).State = EntityState.Modified;
                }

                db.Entry(matriz).State = EntityState.Modified;


                try
                {
                    db.SaveChanges();

                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                    List<String> correos = new List<string>(); //correos TO

                    if (!String.IsNullOrEmpty(matriz.empleados1.correo)) //jefe directo
                        correos.Add(matriz.empleados1.correo); //agrega correo de elaborador
                    if (!String.IsNullOrEmpty(matriz.empleados3.correo)) //Solicitante
                        correos.Add(matriz.empleados3.correo); //agrega correo de elaborador


                    //envía notificacion de solicitud de usuario
                    if (matriz.estatus == IT_MR_Status.EN_PROCESO)
                    {
                        envioCorreo.SendEmailAsync(correos, "La Solicitud de Requerimientos de Usuarios #" + matriz.id + " ha sido actualizada.", envioCorreo.getBody_IT_MR_Notificacion_En_Proceso(matriz));
                        TempData["Mensaje"] = new MensajesSweetAlert("Se ha actualizado la solicitud correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                    } else if (matriz.estatus == IT_MR_Status.FINALIZADO) {
                        envioCorreo.SendEmailAsync(correos, "La Solicitud de Requerimientos de usuarios #" + matriz.id + " ha sido cerrada.", envioCorreo.getBody_IT_MR_Notificacion_Cierre(matriz));
                        TempData["Mensaje"] = new MensajesSweetAlert("Se ha cerrado la solicitud correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                    }
                    return RedirectToAction("solicitudes_sistemas");                  
                }
                catch (Exception ex)
                {
                    mensaje = "Error al guardar en BD.";
                    tipoMensaje = TipoMensajesSweetAlerts.ERROR;
                    EscribeExcepcion(ex, Clases.Models.EntradaRegistroEvento.TipoEntradaRegistroEvento.Error);
                }

                TempData["Mensaje"] = new MensajesSweetAlert(mensaje, tipoMensaje);

                return RedirectToAction("solicitudes_sistemas");


            }

            matrizModel.matriz = matriz;
            return View(matrizModel);

        }

        #endregion
    }
}