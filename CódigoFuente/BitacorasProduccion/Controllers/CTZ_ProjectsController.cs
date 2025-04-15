using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using Portal_2_0.Models;
using SpreadsheetLight;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class CTZ_ProjectsController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        public ActionResult Index(
             string searchProject = null,
             string searchClient = "0",
             string searchFacility = "0",
             string searchStatus = "0",
             string searchCreatedBy = "0",
             DateTime? searchDateStart = null,
             DateTime? searchDateEnd = null)
        {
            // Validar que el usuario tenga el rol ADMIN, de lo contrario mostrar error de permisos
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            // Construir la lista de clientes a partir de CTZ_Clients y agregar la opción "All"
            var clientListFromClients = db.CTZ_Clients
            .AsEnumerable() // Bring data into memory so that ConcatSAPName is computed.
            .Select(c => new SelectListItem { Value = c.Client_Name, Text = c.ConcatSAPName })
            .ToList();

            // Get the other client names from CTZ_Projects where Cliente_Otro is not empty.
            var clientListFromProjects = db.CTZ_Projects
                .Where(p => !string.IsNullOrEmpty(p.Cliente_Otro))
                .Select(p => p.Cliente_Otro)
                .Distinct()
                .AsEnumerable()
                .Select(c => new SelectListItem { Value = c, Text = c.ToUpper() })
                .ToList();

            // Combine both lists (use Union or Concat then Distinct)
            var clientList = clientListFromClients
                .Union(clientListFromProjects, new SelectListItemComparer())
                .ToList();

            // Insert "All" option at the beginning.
            clientList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            ViewBag.ClientList = clientList;

            // Construir la lista de facilities a partir de CTZ_plants y agregar la opción "All"
            var facilityList = db.CTZ_plants
                .Select(f => new SelectListItem { Value = f.Description, Text = f.Description })
                .ToList();
            facilityList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            ViewBag.FacilityList = facilityList;

            // Construir la lista de status a partir de CTZ_Project_Status y agregar la opción "All"
            var statusList = db.CTZ_Project_Status
                .AsEnumerable()
                .Select(s => new SelectListItem { Value = s.Description, Text = s.ConcatStatus })
                .ToList();
            statusList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            ViewBag.StatusList = statusList;

            // Construir la lista de usuarios que han creado proyectos ("Created By")
            var createdByList = db.CTZ_Projects
                .Include(p => p.empleados)
                .Select(p => new SelectListItem
                {
                    Value = (p.empleados != null && !string.IsNullOrEmpty(p.empleados.nombre)) ? p.empleados.nombre : "Unknown",
                    Text = (p.empleados != null && !string.IsNullOrEmpty(p.empleados.nombre)) ? p.empleados.nombre : "Unknown"
                })
                .Distinct()
                .ToList();
            createdByList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            ViewBag.CreatedByList = createdByList;

            // Guardar los valores de búsqueda en ViewBag para que se repoblen en el formulario
            ViewBag.SearchProject = searchProject;
            ViewBag.SearchClient = searchClient;
            ViewBag.SearchFacility = searchFacility;
            ViewBag.SearchStatus = searchStatus;
            ViewBag.SearchCreatedBy = searchCreatedBy;
            ViewBag.SearchDateStart = searchDateStart.HasValue ? searchDateStart.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.SearchDateEnd = searchDateEnd.HasValue ? searchDateEnd.Value.ToString("yyyy-MM-dd") : "";

            // Construir la consulta de proyectos con las inclusiones necesarias
            var query = db.CTZ_Projects
                .Include(p => p.CTZ_Clients)
                .Include(p => p.CTZ_plants)
                .Include(p => p.CTZ_Project_Status)
                .Include(p => p.empleados)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchProject))
            {
                string searchLower = searchProject.ToLower();
                query = query.AsEnumerable()
                             .Where(p => p.ConcatQuoteID.ToLower().Contains(searchLower))
                             .AsQueryable();
            }

            // Aplicar filtro por Client
            if (!string.IsNullOrEmpty(searchClient) && searchClient != "0")
            {
                query = query.Where(p =>
                    (p.CTZ_Clients != null && p.CTZ_Clients.Client_Name == searchClient) ||
                    (!string.IsNullOrEmpty(p.Cliente_Otro) && p.Cliente_Otro == searchClient)
                );
            }

            // Aplicar filtro por Facility
            if (!string.IsNullOrEmpty(searchFacility) && searchFacility != "0")
            {
                query = query.Where(p => p.CTZ_plants != null && p.CTZ_plants.Description == searchFacility);
            }

            // Aplicar filtro por Status
            if (!string.IsNullOrEmpty(searchStatus) && searchStatus != "0")
            {
                query = query.Where(p => p.CTZ_Project_Status != null && p.CTZ_Project_Status.Description == searchStatus);
            }

            // Aplicar filtro por Created By
            if (!string.IsNullOrEmpty(searchCreatedBy) && searchCreatedBy != "0")
            {
                query = query.Where(p => p.empleados != null && p.empleados.nombre == searchCreatedBy);
            }

            // Aplicar filtro por rango de fechas en Creted_Date
            if (searchDateStart.HasValue)
            {
                query = query.Where(p => p.Creted_Date >= searchDateStart.Value);
            }
            if (searchDateEnd.HasValue)
            {
                query = query.Where(p => p.Creted_Date <= searchDateEnd.Value);
            }

            // Ejecutar la consulta y obtener la lista de proyectos filtrados
            var projects = query.OrderByDescending(x => x.Creted_Date).ToList();

            // Retornar la vista con los proyectos (filtrados o no, según los parámetros)
            return View(projects);
        }


        // GET: CTZ_Projects/Create
        public ActionResult Create()
        {
            // Validar que el usuario tenga el rol ADMIN; si no, mostrar error de permisos.
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            // Obtener el empleado logueado y asignarlo al ViewBag.
            empleados empleadoLogeado = obtieneEmpleadoLogeado();
            ViewBag.EmpleadoLogeado = empleadoLogeado;

            // Construir las listas para los dropdowns.
            ViewBag.ID_Client = new SelectList(db.CTZ_Clients, "ID_Cliente", "ConcatSAPName");
            ViewBag.ID_Material_Owner = new SelectList(db.CTZ_Material_Owner, "ID_Owner", "Owner_Key");
            ViewBag.ID_OEM = new SelectList(db.CTZ_OEMClients, "ID_OEM", "Client_Name");
            ViewBag.ID_Plant = new SelectList(db.CTZ_plants, "ID_Plant", "Description");
            ViewBag.ID_VehicleType = new SelectList(db.CTZ_Vehicle_Types, "ID_VehicleType", "VehicleType_Name");
            ViewBag.ID_Status = new SelectList(db.CTZ_Project_Status, nameof(CTZ_Project_Status.ID_Status), nameof(CTZ_Project_Status.ConcatStatus));

            // En el GET, por defecto, los checkboxes estarán sin marcar.
            ViewBag.OtherClient = false;
            ViewBag.OtherOEM = false;

            // Enviar un modelo nuevo para evitar errores.
            return View(new CTZ_Projects());
        }

        // POST: CTZ_Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CTZ_Projects cTZ_Projects)
        {
            // Validar que el usuario tenga el rol ADMIN.
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            // Capturar el estado de los checkboxes.
            bool otherClient = !string.IsNullOrEmpty(Request["chkOtherClient"]);
            bool otherOEM = !string.IsNullOrEmpty(Request["chkOtherOEM"]);
            // Guardarlos en el ViewBag para que la vista los mantenga.
            ViewBag.OtherClient = otherClient;
            ViewBag.OtherOEM = otherOEM;

            // Validación para Client:
            if (otherClient)
            {
                if (string.IsNullOrWhiteSpace(cTZ_Projects.Cliente_Otro))
                {
                    ModelState.AddModelError("Cliente_Otro", "Other Client is required when 'Other Client' is checked.");
                }
                cTZ_Projects.ID_Client = null;
            }
            else
            {
                if (!cTZ_Projects.ID_Client.HasValue || cTZ_Projects.ID_Client.Value == 0)
                {
                    ModelState.AddModelError("ID_Client", "Client selection is required.");
                }
                // Borra lo que pudiera haber en cliente otro.
                cTZ_Projects.Cliente_Otro = null;
            }

            // Verificar el tipo de vehículo:
            if (cTZ_Projects.ID_VehicleType != 1)
            {
                // Si el vehículo no es "Automotriz", se ignoran los campos OEM
                cTZ_Projects.ID_OEM = null;
                cTZ_Projects.OEM_Otro = null;
            }
            else
            {
                // Validación para OEM / Final Client cuando el vehículo es "Automotriz":
                if (otherOEM)
                {
                    if (string.IsNullOrWhiteSpace(cTZ_Projects.OEM_Otro))
                    {
                        ModelState.AddModelError("OEM_Otro", "Other OEM / Final Client is required when 'Other OEM / Final Client' is checked.");
                    }
                    cTZ_Projects.ID_OEM = null;
                }
                else
                {
                    if (!cTZ_Projects.ID_OEM.HasValue || cTZ_Projects.ID_OEM.Value == 0)
                    {
                        ModelState.AddModelError("ID_OEM", "OEM selection is required.");
                    }
                    // Borra lo que pudiera haber en OEM otro.
                    cTZ_Projects.OEM_Otro = null;
                }
            }

            if (ModelState.IsValid)
            {
                empleados empleadoLogeado = obtieneEmpleadoLogeado();

                // Asignar los campos gestionados en el servidor.
                DateTime horaActual = DateTime.Now;
                cTZ_Projects.Creted_Date = horaActual;
                cTZ_Projects.Update_Date = horaActual;
                cTZ_Projects.ID_Created_By = empleadoLogeado.id; // Ajusta según tu modelo

                db.CTZ_Projects.Add(cTZ_Projects);
                db.SaveChanges();

                // Crear el registro en CTZ_Projects_Versions para el nuevo proyecto.
                // Para un proyecto nuevo, la versión inicial es "0.1".
                var version = new CTZ_Projects_Versions
                {
                    ID_Project = cTZ_Projects.ID_Project,
                    ID_Created_by = empleadoLogeado.id,
                    Version_Number = "0.1", // Versión inicial.
                    Creation_Date = horaActual,
                    Is_Current = true,
                    Comments = "Project created.",
                    ID_Status_Project = cTZ_Projects.ID_Status
                };

                db.CTZ_Projects_Versions.Add(version);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert("Se creado el proyecto correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("EditProject", new { id = cTZ_Projects.ID_Project });
            }

            // Si hay errores, repoblar los dropdowns y el empleado logueado.
            ViewBag.ID_Client = new SelectList(db.CTZ_Clients, "ID_Cliente", "ConcatSAPName", cTZ_Projects.ID_Client);
            ViewBag.ID_Material_Owner = new SelectList(db.CTZ_Material_Owner, "ID_Owner", "Owner_Key", cTZ_Projects.ID_Material_Owner);
            ViewBag.ID_OEM = new SelectList(db.CTZ_OEMClients, "ID_OEM", "Client_Name", cTZ_Projects.ID_OEM);
            ViewBag.ID_Plant = new SelectList(db.CTZ_plants, "ID_Plant", "Description", cTZ_Projects.ID_Plant);
            ViewBag.ID_Status = new SelectList(db.CTZ_Project_Status, nameof(CTZ_Project_Status.ID_Status), nameof(CTZ_Project_Status.ConcatStatus), cTZ_Projects.ID_Status);
            ViewBag.ID_VehicleType = new SelectList(db.CTZ_Vehicle_Types, "ID_VehicleType", "VehicleType_Name", cTZ_Projects.ID_VehicleType);

            ViewBag.EmpleadoLogeado = obtieneEmpleadoLogeado();

            return View(cTZ_Projects);
        }


        // GET: CTZ_Projects/EditProject/{id}
        public ActionResult EditProject(int id, string expandedSection = "collapseOne")
        {
            // Validar que el usuario tenga el rol necesario, por ejemplo ADMIN
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            // Cargar el proyecto, incluyendo las propiedades relacionadas (ajusta según tu modelo)
            var project = db.CTZ_Projects
                .Include(p => p.CTZ_Clients)
                .Include(p => p.CTZ_OEMClients)
                .Include(p => p.CTZ_plants)
                .FirstOrDefault(p => p.ID_Project == id);

            if (project == null)
            {
                return HttpNotFound();
            }

            ViewBag.ExpandedSection = expandedSection; // Determina la sección expandida

            return View(project);
        }

        // GET: CTZ_Projects/Edit/{id}
        public ActionResult Edit(int id)
        {
            // Validar permisos: si el usuario no tiene rol ADMIN, mostrar error.
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            // Buscar el proyecto por id.
            var project = db.CTZ_Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            // Cargar las listas para los dropdowns con el valor seleccionado.
            ViewBag.ID_Client = new SelectList(db.CTZ_Clients, "ID_Cliente", "Client_Name", project.ID_Client);
            ViewBag.ID_Material_Owner = new SelectList(db.CTZ_Material_Owner, "ID_Owner", "Owner_Key", project.ID_Material_Owner);
            ViewBag.ID_OEM = new SelectList(db.CTZ_OEMClients, "ID_OEM", "Client_Name", project.ID_OEM);
            ViewBag.ID_Plant = new SelectList(db.CTZ_plants, "ID_Plant", "Description", project.ID_Plant);
            ViewBag.ID_Status = new SelectList(db.CTZ_Project_Status, nameof(CTZ_Project_Status.ID_Status), nameof(CTZ_Project_Status.ConcatStatus), project.ID_Status);
            ViewBag.ID_VehicleType = new SelectList(db.CTZ_Vehicle_Types, "ID_VehicleType", "VehicleType_Name", project.ID_VehicleType);

            // Establecer el estado de los checkboxes: si Cliente_Otro o OEM_Otro tienen valor, marcar "Other"
            ViewBag.OtherClient = !string.IsNullOrEmpty(project.Cliente_Otro);
            ViewBag.OtherOEM = !string.IsNullOrEmpty(project.OEM_Otro);

            return View(project);
        }

        // POST: CTZ_Projects/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CTZ_Projects project)
        {
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            // Capturar el estado de los checkboxes
            bool otherClient = !string.IsNullOrEmpty(Request["chkOtherClient"]);
            bool otherOEM = !string.IsNullOrEmpty(Request["chkOtherOEM"]);
            ViewBag.OtherClient = otherClient;
            ViewBag.OtherOEM = otherOEM;

            // Validación para Client:
            if (otherClient)
            {
                if (string.IsNullOrWhiteSpace(project.Cliente_Otro))
                {
                    ModelState.AddModelError("Cliente_Otro", "Other Client is required when 'Other Client' is checked.");
                }
                project.ID_Client = null;
            }
            else
            {
                if (!project.ID_Client.HasValue || project.ID_Client.Value == 0)
                {
                    ModelState.AddModelError("ID_Client", "Client selection is required.");
                }
                else
                {
                    // Si no se seleccionó "Other", se borra cualquier valor en Cliente_Otro.
                    project.Cliente_Otro = null;
                }
            }


            // Verificar el tipo de vehículo:
            if (project.ID_VehicleType != 1)
            {
                // Si el vehículo no es "Automotriz", se ignoran los campos OEM
                project.ID_OEM = null;
                project.OEM_Otro = null;
            }
            else
            {
                // Validación para OEM / Final Client:
                if (otherOEM)
                {
                    if (string.IsNullOrWhiteSpace(project.OEM_Otro))
                    {
                        ModelState.AddModelError("OEM_Otro", "Other OEM / Final Client is required when 'Other OEM / Final Client' is checked.");
                    }
                    project.ID_OEM = null;
                }
                else
                {
                    if (!project.ID_OEM.HasValue || project.ID_OEM.Value == 0)
                    {
                        ModelState.AddModelError("ID_OEM", "OEM selection is required.");
                    }
                    else
                    {
                        project.OEM_Otro = null;
                    }
                }
            }

            if (ModelState.IsValid)
            {

                #region Cambia propiedades
                //obtiene el objeto de la BD, para unicamente cambiar las propiedades deseadas
                CTZ_Projects projectDB = db.CTZ_Projects.Find(project.ID_Project);

                //cambia valores de cliente
                if (otherClient)
                {
                    projectDB.Cliente_Otro = project.Cliente_Otro;
                    projectDB.ID_Client = null;
                }
                else
                { //si sí hay cliente
                    projectDB.ID_Client = project.ID_Client;
                    projectDB.Cliente_Otro = null;
                }
                if (project.ID_VehicleType != 1)
                {
                    // Si el vehículo no es "Automotriz", se ignoran los campos OEM
                    projectDB.ID_OEM = null;
                    projectDB.OEM_Otro = null;
                }
                else
                {
                    if (otherOEM)
                    {
                        projectDB.OEM_Otro = project.OEM_Otro;
                        projectDB.ID_OEM = null;
                    }
                    else
                    { //si sí hay oem
                        projectDB.ID_OEM = project.ID_OEM;
                        projectDB.OEM_Otro = null;
                    }
                }

                projectDB.ID_Material_Owner = project.ID_Material_Owner;
                projectDB.ID_Plant = project.ID_Plant; //facility
                projectDB.Comments = project.Comments;
                projectDB.ID_VehicleType = project.ID_VehicleType;
                projectDB.ID_Status = project.ID_Status;
                projectDB.ImportRequired = project.ImportRequired;

                projectDB.ID_Updated_By = obtieneEmpleadoLogeado().id;
                // Actualizar campos gestionados en el servidor.
                projectDB.Update_Date = DateTime.Now;

                #endregion

                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert("Project updated successfully.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("EditProject", new { id = projectDB.ID_Project });
            }

            // Si hay errores, repoblar las listas y el empleado logueado.
            ViewBag.ID_Client = new SelectList(db.CTZ_Clients, "ID_Cliente", "Client_Name", project.ID_Client);
            ViewBag.ID_Material_Owner = new SelectList(db.CTZ_Material_Owner, "ID_Owner", "Owner_Key", project.ID_Material_Owner);
            ViewBag.ID_OEM = new SelectList(db.CTZ_OEMClients, "ID_OEM", "Client_Name", project.ID_OEM);
            ViewBag.ID_Plant = new SelectList(db.CTZ_plants, "ID_Plant", "Description", project.ID_Plant);
            ViewBag.ID_Status = new SelectList(db.CTZ_Project_Status, nameof(CTZ_Project_Status.ID_Status), nameof(CTZ_Project_Status.ConcatStatus), project.ID_Status);
            ViewBag.ID_VehicleType = new SelectList(db.CTZ_Vehicle_Types, "ID_VehicleType", "VehicleType_Name", project.ID_VehicleType);

            return View(project);
        }

        // GET: CTZ_Projects/EditClientPartInformation/{id}
        public ActionResult EditClientPartInformation(int id)
        {
            // Validar permisos: si el usuario no tiene rol ADMIN, mostrar error.
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            // Buscar el proyecto por id junto con sus materiales relacionados.
            var project = db.CTZ_Projects
                .Include(p => p.CTZ_Project_Materials.Select(m => m.CTZ_Route))
                .Include(p => p.CTZ_Clients)
                .Include(p => p.CTZ_OEMClients)
                .Include(p => p.CTZ_Material_Owner)
                .Include(p => p.CTZ_Project_Status)
                .Include(p => p.empleados)
                .FirstOrDefault(p => p.ID_Project == id);

            if (project == null)
            {
                return HttpNotFound();
            }


            #region Combo Lists
            // Traer todos los registros de CTZ_Temp_IHS
            var tempIHSList = db.CTZ_Temp_IHS.ToList();

            // Traer todas las producciones y agruparlas por ID_IHS en un diccionario
            var productionLookup = db.CTZ_Temp_IHS_Production
                .Select(p => new { p.ID_IHS, p.Production_Year, p.Production_Month, p.Production_Amount })
                .ToList()
                .GroupBy(p => p.ID_IHS)
                .ToDictionary(g => g.Key, g => g.ToList());

            var vehicles = tempIHSList.Select(x => new VehicleItem
            {
                Value = x.ConcatCodigo,
                Text = x.ConcatCodigo,
                SOP = x.SOP.HasValue ? x.SOP.Value.ToString("yyyy-MM") : "",
                EOP = x.EOP.HasValue ? x.EOP.Value.ToString("yyyy-MM") : "",
                Program = x.Program,
                MaxProduction = x.Max_Production.ToString(),
                ProductionDataJson = productionLookup.ContainsKey(x.ID_IHS)
                    ? JsonConvert.SerializeObject(productionLookup[x.ID_IHS])
                    : "[]"
            }).ToList();
            ViewBag.VehicleList = vehicles;

            var qualityList = db.SCDM_cat_grado_calidad
              .Where(q => q.activo)
              .Select(q => new
              {
                  id = q.grado_calidad, // o puedes usar q.clave si lo prefieres
                  text = q.grado_calidad
              })
              .ToList();
            ViewBag.QualityList = qualityList;

            // En tu acción GET, antes de retornar la vista
            var routes = db.CTZ_Route
                .Where(r => r.Active)  // si solo deseas rutas activas
                .Select(r => new SelectListItem
                {
                    Value = r.ID_Route.ToString(),
                    Text = r.Route_Name
                })
                .ToList();

            ViewBag.ID_RouteList = routes;

            // ID_Plant
            int plantId = project.ID_Plant;

            // Obtener los IDs de líneas de producción para esa planta
            var productionLineIds = db.CTZ_Production_Lines
                .Where(l => l.ID_Plant == plantId)
                .Select(l => l.ID_Line)
                .ToList();

            // Obtener los IDs de Material Type asociados a esas líneas
            var materialTypeIds = db.CTZ_Material_Type_Lines
                .Where(mt => productionLineIds.Contains(mt.ID_Line))
                .Select(mt => mt.ID_Material_Type)
                .Distinct();

            // Obtener la lista de tipos de material disponibles
            var availableMaterialTypes = db.CTZ_Material_Type
                .Where(mt => materialTypeIds.Contains(mt.ID_Material_Type) && mt.Active)
                .Select(mt => new
                {
                    Value = mt.ID_Material_Type,
                    Text = mt.Material_Name
                })
                .ToList();

            // Crear el SelectList y asignarlo al ViewBag
            ViewBag.MaterialTypeList = new SelectList(availableMaterialTypes, "Value", "Text");


            var shapeList = db.SCDM_cat_forma_material
                        .Where(s => new int[] { 19, 18, 3 }.Contains(s.id)) //19 = Recto, 18 = configurado, 3 = Trapecio 
                        .ToList()
                        .Select(s => new
                        {
                            Value = s.id,
                            Text = s.ConcatKey
                        })
                        .ToList();

            ViewBag.ShapeList = new SelectList(shapeList, "Value", "Text");

            // Obtener la lista de lineas de produccion según la planta
            var LinesList = db.CTZ_Production_Lines.Where(x => x.Active).ToList()
                .Select(s => new
                {
                    Value = s.ID_Line,
                    Text = s.Description
                })
                .ToList();
            ViewBag.LinesList = new SelectList(LinesList, "Value", "Text");


            #endregion
            // Retornar la vista con el proyecto cargado y sus materiales.
            return View(project);
        }

        // POST: CTZ_Projects/EditClientPartInformation/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClientPartInformation(CTZ_Projects project, List<CTZ_Project_Materials> materials, HttpPostedFileBase archivo)
        {
            // Validar permisos
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            if (ModelState.IsValid)
            {
                // Obtener el proyecto existente
                var existingProject = db.CTZ_Projects
                    .Include(p => p.CTZ_Project_Materials)
                    .FirstOrDefault(p => p.ID_Project == project.ID_Project);

                if (existingProject == null)
                {
                    return HttpNotFound();
                }


                // (Opcional) Procesar el archivo si se recibió
                int? newFileId = null;

                if (archivo != null && archivo.ContentLength > 0)
                {
                    // Leer el stream y convertirlo a arreglo de bytes
                    byte[] fileData = null;
                    using (var binaryReader = new System.IO.BinaryReader(archivo.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(archivo.ContentLength);
                    }

                    // Obtener el nombre original sin ruta
                    string originalFileName = System.IO.Path.GetFileName(archivo.FileName);

                    // Separar el nombre base y la extensión
                    string extension = System.IO.Path.GetExtension(originalFileName); // incluye el punto
                    string baseName = System.IO.Path.GetFileNameWithoutExtension(originalFileName);

                    // Eliminar caracteres no permitidos (por ejemplo, sólo se permiten letras, números, guiones y guión bajo)
                    baseName = Regex.Replace(baseName, @"[^A-Za-z0-9_\-]", "");

                    // Calcula la longitud máxima permitida para el nombre base, de forma que la longitud total no supere 80 caracteres.
                    int allowedBaseLength = 80 - extension.Length;
                    if (baseName.Length > allowedBaseLength)
                    {
                        baseName = baseName.Substring(0, allowedBaseLength);
                    }

                    // Reconstruir el nombre final
                    string newFileName = baseName + extension;

                    // Crear el registro para CTZ_Files utilizando el nuevo nombre
                    CTZ_Files newFile = new CTZ_Files
                    {
                        Name = newFileName,
                        MineType = archivo.ContentType,
                        Data = fileData
                    };

                    db.CTZ_Files.Add(newFile);
                    db.SaveChanges(); // Guarda para que se asigne el ID automáticamente
                    newFileId = newFile.ID_File;
                }

                //obtiene los ID_CAD_file que seran eliminados
                // (1) Obtener la lista de IDs de archivos de los materiales actuales (antes de eliminarlos)
                var oldFileIds = existingProject.CTZ_Project_Materials
                    .Where(m => m.ID_File_CAD_Drawing.HasValue)
                    .Select(m => m.ID_File_CAD_Drawing.Value)
                    .Distinct()
                    .ToList();

                // (2) Obtener la lista de IDs de archivos de los materiales nuevos (los que vienen en "materials")
                var newFileIds = materials != null
                    ? materials.Where(m => m.ID_File_CAD_Drawing.HasValue)
                               .Select(m => m.ID_File_CAD_Drawing.Value)
                               .Distinct()
                               .ToList()
                    : new List<int>();

                // (3) Calcular los IDs que estaban en los materiales previos pero ya no aparecen en los nuevos
                var fileIdsToDelete = oldFileIds.Except(newFileIds).ToList();

                // Eliminar los registros anteriores de CTZ_Project_Materials
                db.CTZ_Project_Materials.RemoveRange(existingProject.CTZ_Project_Materials);

                // Agregar los materiales recibidos en la lista "materials" 
                if (materials != null)
                {
                    foreach (var material in materials)
                    {
                        material.ID_Project = project.ID_Project;

                        // Si la fecha tiene valor, convertirla para que el día sea 1.
                        if (material.SOP_SP.HasValue)
                        {
                            var sop = material.SOP_SP.Value;
                            material.SOP_SP = new DateTime(sop.Year, sop.Month, 1);
                        }
                        if (material.EOP_SP.HasValue)
                        {
                            var eop = material.EOP_SP.Value;
                            material.EOP_SP = new DateTime(eop.Year, eop.Month, 1);
                        }

                        // Si el material tiene la bandera IsFile true, es el material que debe recibir el nuevo archivo.
                        if (material.IsFile.HasValue && material.IsFile == true)
                        {
                            // Si el material ya tenía un archivo asignado y es distinto al nuevo,
                            // verificar si ese archivo no se está referenciando en otros registros.
                            if (material.ID_File_CAD_Drawing.HasValue &&
                                material.ID_File_CAD_Drawing != newFileId)
                            {
                                int oldFileId = material.ID_File_CAD_Drawing.Value;
                                bool isStillReferenced = db.CTZ_Project_Materials
                                    .Any(m => m.ID_File_CAD_Drawing == oldFileId && m.ID_Material != material.ID_Material);

                                // Si el archivo antiguo no se está usando, eliminar el registro.
                                if (!isStillReferenced)
                                {
                                    var oldFile = db.CTZ_Files.Find(oldFileId);
                                    if (oldFile != null)
                                    {
                                        db.CTZ_Files.Remove(oldFile);
                                    }
                                }
                            }
                            // Asigna el nuevo id de archivo a este material
                            material.ID_File_CAD_Drawing = newFileId;
                        }
                        db.CTZ_Project_Materials.Add(material);
                    }
                }

                // Guardar cambios, antes de validar referencias al ID file
                db.SaveChanges();

                // (4) Verificar globalmente que esos archivos ya no sean referenciados en otros materiales y eliminarlos
                foreach (var fileId in fileIdsToDelete)
                {
                    // Se comprueba en toda la tabla de materiales si hay algún registro con ese fileId.
                    bool isStillReferenced = db.CTZ_Project_Materials.Any(m => m.ID_File_CAD_Drawing == fileId);
                    if (!isStillReferenced)
                    {
                        var oldFile = db.CTZ_Files.Find(fileId);
                        if (oldFile != null)
                        {
                            db.CTZ_Files.Remove(oldFile);
                        }
                    }
                }
                //guarda cambios
                db.SaveChanges();


                TempData["SuccessMessage"] = "Material saved successfully!";
                return RedirectToAction("EditClientPartInformation", new { id = project.ID_Project });
            }

            // Si hay errores de validación, recargar la vista con el modelo enviado

            var vehicles = db.CTZ_Temp_IHS
                .AsEnumerable()  // Materializa la consulta para usar propiedades calculadas en memoria
                .Select(x => new VehicleItem
                {
                    Value = x.ConcatCodigo,
                    Text = x.ConcatCodigo,
                    SOP = x.SOP.HasValue ? x.SOP.Value.ToString("yyyy-MM") : "",
                    EOP = x.EOP.HasValue ? x.EOP.Value.ToString("yyyy-MM") : "",
                    Program = x.Program,
                    MaxProduction = x.Max_Production.ToString()
                })
                .ToList();
            ViewBag.VehicleList = vehicles;

            var qualityList = db.SCDM_cat_grado_calidad
                .Where(q => q.activo)
                .Select(q => new
                {
                    id = q.grado_calidad, // o puedes usar q.clave si lo prefieres
                    text = q.grado_calidad
                })
                .ToList();

            ViewBag.QualityList = qualityList;

            // En tu acción GET, antes de retornar la vista
            var routes = db.CTZ_Route
                .Where(r => r.Active)  // si solo deseas rutas activas
                .Select(r => new SelectListItem
                {
                    Value = r.ID_Route.ToString(),
                    Text = r.Route_Name
                })
                .ToList();

            ViewBag.ID_RouteList = routes;

            // ID_Plant
            int plantId = project.ID_Plant;

            // Obtener los IDs de líneas de producción para esa planta
            var productionLineIds = db.CTZ_Production_Lines
                .Where(l => l.ID_Plant == plantId)
                .Select(l => l.ID_Line)
                .ToList();

            // Obtener los IDs de Material Type asociados a esas líneas
            var materialTypeIds = db.CTZ_Material_Type_Lines
                .Where(mt => productionLineIds.Contains(mt.ID_Line))
                .Select(mt => mt.ID_Material_Type)
                .Distinct();

            // Obtener la lista de tipos de material disponibles
            var availableMaterialTypes = db.CTZ_Material_Type
                .Where(mt => materialTypeIds.Contains(mt.ID_Material_Type) && mt.Active)
                .Select(mt => new
                {
                    Value = mt.ID_Material_Type,
                    Text = mt.Material_Name
                })
                .ToList();

            // Crear el SelectList y asignarlo al ViewBag
            ViewBag.MaterialTypeList = new SelectList(availableMaterialTypes, "Value", "Text");

            // Obtener la lista de formas (Shape) disponibles usando ConcatKey para el texto
            var shapeList = db.SCDM_cat_forma_material
                         .Where(s => new int[] { 19, 18, 3 }.Contains(s.id)) //19 = Recto, 18 = configurado, 3 = Trapecio 
                         .ToList()
                         .Select(s => new
                         {
                             Value = s.id,
                             Text = s.ConcatKey
                         })
                         .ToList();

            ViewBag.ShapeList = new SelectList(shapeList, "Value", "Text");

            // Obtener la lista de lineas de produccion según la planta
            var LinesList = db.CTZ_Production_Lines.Where(x => x.ID_Plant == project.ID_Plant).ToList()
                .Select(s => new
                {
                    Value = s.ID_Line,
                    Text = s.Description
                })
                .ToList();
            ViewBag.LinesList = new SelectList(LinesList, "Value", "Text");

            return View(project);
        }

        // GET: CTZ_Projects/Create
        public ActionResult ProjectStatus()
        {
            // Validar que el usuario tenga el rol ADMIN; si no, mostrar error de permisos.
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");


            // Enviar un modelo nuevo para evitar errores.
            return View(db.CTZ_Projects.OrderByDescending(x => x.ID_Project).ToList());
        }

        [HttpPost]
        public ActionResult SendQuoteEmail(int projectId)
        {
            try
            {
                // Retrieve the project if needed
                var project = db.CTZ_Projects.Find(projectId);
                if (project == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Project not found");
                }

                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>(); //correos TO

                envioCorreo.SendEmailAsync(correos, "New Quote: " + project.ConcatQuoteID, getBodyNewQuote(project));

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Optionally log the error
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public ActionResult DownloadFile(int fileId)
        {
            using (var db = new Portal_2_0Entities())
            {
                var file = db.CTZ_Files.FirstOrDefault(f => f.ID_File == fileId);

                if (file == null)
                {
                    return HttpNotFound("Archivo no encontrado.");
                }

                return File(file.Data, file.MineType, file.Name);
            }
        }


        [NonAction]
        public string getBodyNewQuote(CTZ_Projects quote)
        {
            // Generate the URL for the "View Request Details" button.
            string detailsUrl = Url.Action("EditProject", "CTZ_Projects", new { id = quote.ID_Project }, protocol: Request.Url.Scheme);

            string body = @"
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>New Quote Notification</title>
                </head>
                <body style='margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;'>
                    <table align='center' border='0' cellpadding='0' cellspacing='0' width='600' style='border-collapse: collapse; margin:20px auto; background-color:#ffffff; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                        <!-- Header -->
                        <tr>
                            <td align='center' bgcolor='#009ff7' style='padding: 20px 0;'>
                                <h1 style='color:#ffffff; margin:0; font-size: 26px;'>New Quote Notification</h1>
                                <p style='color:#ffffff; margin: 5px 0 0 0; font-size: 16px;'>thyssenkrupp Materials de México (tkMM)</p>
                            </td>
                        </tr>
                        <!-- Body Content -->
                        <tr>
                            <td style='padding: 30px;'>
                                <p style='font-size: 14px; color:#333333; margin:0 0 20px 0;'>
                                    Dear Approval Team,
                                </p>
                                <p style='font-size: 14px; color:#333333; margin:0 0 20px 0;'>
                                    A new quote has been generated for your review. Below are the details: 
                                </p>
                                <table border='0' cellpadding='5' cellspacing='0' width='100%' style='font-size: 14px; color:#333333; border: 1px solid #009ff7; border-collapse: collapse;'>
                                    <tr>
                                        <td style='background-color:#f2f9ff; border:1px solid #009ff7;'><strong>Quote ID:</strong></td>
                                        <td style='border:1px solid #009ff7;'>" + quote.ConcatQuoteID + @"</td>
                                    </tr>
                                    <tr>
                                        <td style='background-color:#f2f9ff; border:1px solid #009ff7;'><strong>Client:</strong></td>
                                        <td style='border:1px solid #009ff7;'>" + (quote.CTZ_Clients != null ? quote.CTZ_Clients.Client_Name : quote.Cliente_Otro) + @"</td>
                                    </tr>
                                    <tr>
                                        <td style='background-color:#f2f9ff; border:1px solid #009ff7;'><strong>Facility:</strong></td>
                                        <td style='border:1px solid #009ff7;'>" + quote.CTZ_plants.Description + @"</td>
                                    </tr>
                                    <tr>
                                        <td style='background-color:#f2f9ff; border:1px solid #009ff7;'><strong>Created Date:</strong></td>
                                        <td style='border:1px solid #009ff7;'>" + quote.Creted_Date.ToString("dd/MM/yyyy") + @"</td>
                                    </tr>
                                </table>
                                <p style='font-size: 14px; color:#333333; margin:20px 0 20px 0;'>
                                    For further details, please click the button below to view the full request.
                                </p>
                                <div style='text-align: center; margin: 30px 0;'>
                                    <a href='" + detailsUrl + @"' style='background-color:#009ff7; color:#ffffff; padding: 12px 25px; text-decoration:none; border-radius: 4px; font-size: 16px; display:inline-block;'>
                                        View Request Details
                                    </a>
                                </div>
                              
                            </td>
                        </tr>
                        <!-- Footer -->
                        <tr>
                            <td bgcolor='#009ff7' style='padding: 10px; text-align: center; font-size: 12px; color:#ffffff;'>
                                &copy; " + DateTime.Now.Year + @" thyssenkrupp Materials de México (tkMM). All rights reserved.
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";
            return body;
        }

        [HttpGet]
        public JsonResult GetTheoreticalStrokes(int productionLineId, float pitch, float rotation)
        {
            // Obtener la línea de producción para extraer el manufacturer.
            var prodLine = db.CTZ_Production_Lines.Find(productionLineId);
            if (prodLine == null)
                return Json(new { success = false, message = "Production line not found" }, JsonRequestBehavior.AllowGet);
            if (!prodLine.ID_Manufacturer.HasValue)
                return Json(new { success = false, message = "Manufacturer line not found" }, JsonRequestBehavior.AllowGet);

            int manufacturerId = prodLine.ID_Manufacturer.Value;

            // Obtener todas las configuraciones para este manufacturer.
            var settings = db.CTZ_Line_Stroke_Settings
                             .Where(s => s.ID_Machine_Manufacturer == manufacturerId)
                             .ToList();
            if (!settings.Any())
                return Json(new { success = false, message = "No stroke settings found" }, JsonRequestBehavior.AllowGet);

            // Agrupar los registros por valor de Rotation_degrees (giro)
            var groupsByRotation = settings.GroupBy(s => s.Rotation_degrees)
                                           .ToDictionary(g => g.Key, g => g.OrderBy(s => s.Advance_mm).ToList());

            // Ordenar los valores de giro
            var rotationKeys = groupsByRotation.Keys.OrderBy(r => r).ToList();

            // Determinar los dos valores de giro que encierran el rotation solicitado
            double rotLow, rotHigh;
            if (rotation <= rotationKeys.First())
            {
                rotLow = rotHigh = rotationKeys.First();
            }
            else if (rotation >= rotationKeys.Last())
            {
                rotLow = rotHigh = rotationKeys.Last();
            }
            else
            {
                rotLow = rotationKeys.First(r => r <= rotation);
                rotHigh = rotationKeys.First(r => r >= rotation);
            }

            // Función local para interpolar en la dimensión "advance" (pitch)
            double InterpolateAdvance(List<CTZ_Line_Stroke_Settings> list, double pitchValue)
            {
                if (pitchValue <= list.First().Advance_mm)
                    return list.First().Strokes;
                if (pitchValue >= list.Last().Advance_mm)
                    return list.Last().Strokes;
                // Buscar dos registros que encierren el pitchValue
                for (int i = 0; i < list.Count - 1; i++)
                {
                    var current = list[i];
                    var next = list[i + 1];
                    if (current.Advance_mm <= pitchValue && pitchValue <= next.Advance_mm)
                    {
                        double ratio = (pitchValue - current.Advance_mm) / (next.Advance_mm - current.Advance_mm);
                        return current.Strokes + ratio * (next.Strokes - current.Strokes);
                    }
                }
                return list.First().Strokes; // valor por defecto
            }

            // Obtener el valor de strokes para el grupo con giro inferior y superior.
            double strokesLow = InterpolateAdvance(groupsByRotation[rotLow], pitch);
            double strokesHigh = InterpolateAdvance(groupsByRotation[rotHigh], pitch);

            // Si el giro solicitado coincide exactamente con uno de los grupos, no se interpola en giro.
            double resultStrokes = (rotLow == rotHigh)
                ? strokesLow
                : strokesLow + ((rotation - rotLow) / (rotHigh - rotLow)) * (strokesHigh - strokesLow);

            // Redondear a entero antes de retornar
            int roundedStrokes = (int)Math.Round(resultStrokes);  

            return Json(new { success = true, theoreticalStrokes = roundedStrokes }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetEngineeringDimensions(int lineId)
        {
            // Buscar los registros para la línea solicitada

            var dimensions = db.CTZ_Engineering_Dimension
                               .Where(d => d.ID_Line == lineId && d.Active)
                               .Select(d => new
                               {
                                   d.ID_Criteria,
                                   d.Min_Value,
                                   d.Max_Value
                               })
                               .ToList();

            return Json(dimensions, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public JsonResult GetProductionLineData(int plantId)
        {
            try
            {
                // 1. Obtener las líneas de producción activas para la planta
                var productionLines = db.CTZ_Production_Lines
                    .Where(l => l.ID_Plant == plantId && l.Active)
                    .ToList();

                // 2. Obtener los años fiscales ordenados (por ejemplo, ascendente)
                var fiscalYears = db.CTZ_Fiscal_Years
                    .OrderBy(fy => fy.ID_Fiscal_Year)
                    .ToList();

                // 3. Obtener las horas totales disponibles por FY
                var totalTimeByFY = db.CTZ_Total_Time_Per_Fiscal_Year
                    .ToDictionary(t => t.ID_Fiscal_Year, t => t.Value);

                // Definir el orden deseado para los estatus
                // Por ejemplo: "POH" primero, luego "Casi Casi", seguido de "Carry Over" y finalmente "Quotes"
                var orderMapping = new Dictionary<string, int>
        {
            { "POH", 1 },
            { "Casi Casi", 2 },
            { "Carry Over", 3 },
            { "Quotes", 4 }
        };

                var resultData = new List<object>();

                foreach (var line in productionLines)
                {
                    // Creamos un objeto para cada línea con su id y descripción.
                    var lineData = new
                    {
                        LineId = line.ID_Line,
                        LineName = line.Description,
                        DataByFY = new List<object>()
                    };

                    foreach (var fy in fiscalYears)
                    {
                        double totalHours = totalTimeByFY.ContainsKey(fy.ID_Fiscal_Year) ? totalTimeByFY[fy.ID_Fiscal_Year] : 0;

                        // Obtener todos los registros de CTZ_Hours_By_Line para esta línea y FY.
                        var hoursEntries = db.CTZ_Hours_By_Line
                            .Where(h => h.ID_Line == line.ID_Line && h.ID_Fiscal_Year == fy.ID_Fiscal_Year)
                            .ToList();

                        double totalOccupied = hoursEntries.Sum(x => x.Hours);

                        // Agrupar por estatus (usando la descripción) y luego ordenar de acuerdo al diccionario orderMapping
                        var breakdown = hoursEntries
                            .GroupBy(x => x.CTZ_Project_Status.Description)
                            .Select(g => new
                            {
                                StatusId = g.Key,
                                OccupiedHours = g.Sum(x => x.Hours),
                                Percentage = totalHours > 0 ? Math.Round((g.Sum(x => x.Hours) / totalHours) * 100, 2) : 0
                            })
                            .OrderBy(b => orderMapping.ContainsKey(b.StatusId) ? orderMapping[b.StatusId] : 100)
                            .ToList();

                        lineData.DataByFY.Add(new
                        {
                            FiscalYear = fy.Fiscal_Year_Name,
                            TotalOccupied = totalOccupied,
                            TotalHours = totalHours,
                            Breakdown = breakdown
                        });
                    }
                    resultData.Add(lineData);
                }

                return Json(new { success = true, data = resultData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        #region What-if Capacidad por Material

        [HttpGet]
        public ActionResult GetMaterialCapacityScenarios(int projectId, int? materialId, int blkID, string vehicle, double? partsPerVehicle, double? idealCycleTimePerTool,
                                                         double? blanksPerStroke, double? oee, DateTime? realSOP, DateTime? realEOP, int? annualVol)
        {
            try
            {
                // 1. Cargar el proyecto con sus materiales
                var project = db.CTZ_Projects
                                .Include("CTZ_Project_Materials")
                                .FirstOrDefault(p => p.ID_Project == projectId);
                if (project == null)
                    return Json(new { success = false, message = "Proyecto no encontrado." }, JsonRequestBehavior.AllowGet);

                // 2. Obtener el material seleccionado o tratar como nuevo si es null
                CTZ_Project_Materials selectedMaterial = null;
                if (materialId.HasValue)
                {
                    selectedMaterial = project.CTZ_Project_Materials.FirstOrDefault(m => m.ID_Material == materialId.Value);
                }

                if (selectedMaterial != null)
                {
                    // Actualiza el valor de la línea de producción al blkID
                    selectedMaterial.ID_Real_Blanking_Line = blkID;
                    selectedMaterial.Vehicle = vehicle ?? "";
                    selectedMaterial.Parts_Per_Vehicle = partsPerVehicle;
                    selectedMaterial.Ideal_Cycle_Time_Per_Tool = idealCycleTimePerTool;
                    selectedMaterial.Blanks_Per_Stroke = blanksPerStroke;
                    selectedMaterial.OEE = oee;
                    selectedMaterial.Real_SOP = realSOP;
                    selectedMaterial.Real_EOP = realEOP;
                    selectedMaterial.Annual_Volume = annualVol;
                }
                else
                {
                    // Si no se encontró, se crea un nuevo material.
                    // Se deben inicializar los campos mínimos requeridos; 
                    selectedMaterial = new CTZ_Project_Materials
                    {
                        // Aquí asigna el id de proyecto y la línea nueva.
                        ID_Project = project.ID_Project,
                        ID_Real_Blanking_Line = blkID,
                        Vehicle = vehicle ?? "",
                        Parts_Per_Vehicle = partsPerVehicle,
                        Ideal_Cycle_Time_Per_Tool = idealCycleTimePerTool,
                        Blanks_Per_Stroke = blanksPerStroke,
                        OEE = oee,
                        Real_SOP = realSOP,
                        Real_EOP = realEOP,
                        Annual_Volume = annualVol
                    };

                    // Agregar el nuevo material a la colección del proyecto.
                    project.CTZ_Project_Materials.Add(selectedMaterial);
                }

                // 3. Obtiene la capacidad simulando el cambio de línea para el material seleccionado
                var summarizeData = project.GetCapacityScenarios(project.CTZ_Project_Materials);
                // (summarizeData es un Dictionary<int, Dictionary<int, double>>)

                // 4. Convertir el diccionario en un array de objetos para el hansontable
                var linesDict = db.CTZ_Production_Lines.ToDictionary(x => x.ID_Line, x => x.Description);
                var fyDict = db.CTZ_Fiscal_Years.ToDictionary(x => x.ID_Fiscal_Year, x => x.Fiscal_Year_Name);

                var allLineIds = summarizeData.Keys.OrderBy(x => x).ToList();
                var allFYIds = summarizeData.Values
                    .SelectMany(dict => dict.Keys)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                var responseData = new List<Dictionary<string, object>>();

                // Construir las filas (por línea)
                foreach (var lineId in allLineIds)
                {
                    var row = new Dictionary<string, object>();

                    string lineName = linesDict.ContainsKey(lineId)
                        ? linesDict[lineId]
                        : $"Line#{lineId}";
                    row["Line"] = lineName;
                    row["LineId"] = lineId;
                    foreach (var fyId in allFYIds)
                    {
                        string fyName = fyDict.ContainsKey(fyId)
                            ? fyDict[fyId]
                            : $"FY#{fyId}";
                        double val = 0.0;
                        if (summarizeData[lineId].ContainsKey(fyId))
                        {
                            val = summarizeData[lineId][fyId];
                        }
                        row[fyName] = Math.Round(val, 4);
                    }
                    responseData.Add(row);
                }

                // 5. Determinar el rango efectivo de producción
                // Primero, tomar Real_SOP y Real_EOP; si no existen, usar SOP_SP/EOP_SP
                DateTime effectiveSOP, effectiveEOP;
                if (selectedMaterial.Real_SOP.HasValue && selectedMaterial.Real_EOP.HasValue)
                {
                    effectiveSOP = selectedMaterial.Real_SOP.Value;
                    effectiveEOP = selectedMaterial.Real_EOP.Value;
                }
                else if (selectedMaterial.SOP_SP.HasValue && selectedMaterial.EOP_SP.HasValue)
                {
                    effectiveSOP = selectedMaterial.SOP_SP.Value;
                    effectiveEOP = selectedMaterial.EOP_SP.Value;
                }
                else
                {
                    // Si ninguno existe, no se puede calcular el rango.
                    effectiveSOP = DateTime.MinValue;
                    effectiveEOP = DateTime.MaxValue;
                }

                // 6. Con el rango efectivo, determinar los FY que entran en el rango.
                // Considerando que un FY va de octubre a septiembre, se consultan los FY de la BD que se traslapan.
                var fiscalYearsInRange = db.CTZ_Fiscal_Years
                    .Where(fy => !(fy.End_Date < effectiveSOP || fy.Start_Date > effectiveEOP))
                    .Select(fy => fy.Fiscal_Year_Name)
                    .ToList();

                // 7. Determinar el status del material en base a los valores dentro del rango
                // Se recorre cada FY dentro del rango (por ejemplo, del FY de la línea asignada, o se decide usar el primer registro de summarizeData)
                // Aquí se asume que se evalúa el status para la línea real asignada al material.
                double capacityOver98 = 0;
                double capacityOver95 = 0;
                // Por ejemplo, si el material tiene asignada una línea real, la usamos; si no, se recorre el primer grupo.
                int lineForStatus = blkID;
                if (summarizeData.ContainsKey(lineForStatus))
                {
                    foreach (var kvp in summarizeData[lineForStatus])
                    {
                        // Obtener el nombre fiscal para comparar si está en el rango efectivo.
                        string fyName = fyDict.ContainsKey(kvp.Key) ? fyDict[kvp.Key] : "";
                        if (fiscalYearsInRange.Contains(fyName))
                        {
                            double cap = kvp.Value; // valor en decimal (ej. 0.95)
                            if (cap >= 0.98) capacityOver98 = 1; // se marca que hay uno que supera 100%
                            if (cap >= 0.95) capacityOver95 = 1;  // se marca que hay uno >=98%
                        }
                    }
                }
                string status_dm;
                if (capacityOver98 > 0)
                    status_dm = "REJECTED";
                else if (capacityOver95 > 0)
                    status_dm = "ON REVIEWED";
                else
                    status_dm = "APPROVED";

                // 8. Devolver en el JSON además del array de datos, la metadata:
                //    - status_dm: para actualizar el input status_dm
                //    - highlightedFY: un array de encabezados (FY) que están en el rango de producción (para marcar de un color distinto)
                return Json(new { success = true, data = responseData, status_dm = status_dm, highlightedFY = fiscalYearsInRange }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetMaterialCapacityScenariosGraphs(int projectId, int? materialId, int? blkID,
                                                           string vehicle, double? partsPerVehicle,
                                                           double? idealCycleTimePerTool, double? blanksPerStroke,
                                                           double? oee, DateTime? realSOP, DateTime? realEOP, int? annualVol, bool OnlyBDMaterials = false)
        {
            try
            {
                // 1. Cargar el proyecto con sus materiales
                var project = db.CTZ_Projects
                                .Include("CTZ_Project_Materials")
                                .FirstOrDefault(p => p.ID_Project == projectId);
                if (project == null)
                    return Json(new { success = false, message = "Proyecto no encontrado." }, JsonRequestBehavior.AllowGet);

                // 2. Obtener el material seleccionado o tratarlo como nuevo si es null
                if (!OnlyBDMaterials) { //si onlyBDmaterials no esta activo
                    CTZ_Project_Materials selectedMaterial = null;
                    if (materialId.HasValue)
                    {
                        selectedMaterial = project.CTZ_Project_Materials.FirstOrDefault(m => m.ID_Material == materialId.Value);
                    }

                    if (selectedMaterial != null)
                    {
                        // Actualiza la línea de producción real
                        selectedMaterial.ID_Real_Blanking_Line = blkID;
                        selectedMaterial.Vehicle = vehicle ?? "";
                        selectedMaterial.Parts_Per_Vehicle = partsPerVehicle;
                        selectedMaterial.Ideal_Cycle_Time_Per_Tool = idealCycleTimePerTool;
                        selectedMaterial.Blanks_Per_Stroke = blanksPerStroke;
                        selectedMaterial.OEE = oee;
                        selectedMaterial.Real_SOP = realSOP;
                        selectedMaterial.Real_EOP = realEOP;
                        selectedMaterial.Annual_Volume = annualVol;
                    }
                    else
                    {
                        // Si no se encontró el material, crear uno nuevo con los valores mínimos requeridos.
                        selectedMaterial = new CTZ_Project_Materials
                        {
                            ID_Project = project.ID_Project,
                            ID_Real_Blanking_Line = blkID,
                            Vehicle = vehicle ?? "",
                            Parts_Per_Vehicle = partsPerVehicle,
                            Ideal_Cycle_Time_Per_Tool = idealCycleTimePerTool,
                            Blanks_Per_Stroke = blanksPerStroke,
                            OEE = oee,
                            Real_SOP = realSOP,
                            Real_EOP = realEOP,
                            Annual_Volume = annualVol
                        };
                        project.CTZ_Project_Materials.Add(selectedMaterial);
                    } 
                }

                // 3. Obtener el diccionario final de capacidad utilizando tu método existente.
                // finalPercentageDict es del tipo Dictionary<int, Dictionary<int, Dictionary<int, double>>>
                var finalPercentageDict = project.GetGraphCapacityScenarios(project.CTZ_Project_Materials);

                // 4. Calcula la lista de líneas de producción utilizadas en el proyecto.
                // para cada material, si tiene línea real se toma esa; si no, se toma la teórica.
                var validLineIds = project.CTZ_Project_Materials
                                          .Select(m => m.ID_Real_Blanking_Line.HasValue
                                                     ? m.ID_Real_Blanking_Line.Value
                                                     : (m.ID_Theoretical_Blanking_Line.HasValue
                                                            ? m.ID_Theoretical_Blanking_Line.Value
                                                            : 0))
                                          .Where(id => id != 0)
                                          .Distinct()
                                          .ToList();

                // 5. Cargar las líneas de producción activas (para obtener la descripción)
                var activeLines = db.CTZ_Production_Lines.Where(l => l.Active).ToList();
                var linesDict = activeLines.ToDictionary(l => l.ID_Line, l => l.Description);

                // 6. Cargar los años fiscales ordenados (para mapear el ID a nombre fiscal)
                var fiscalYears = db.CTZ_Fiscal_Years.OrderBy(fy => fy.ID_Fiscal_Year).ToList();
                var fyDict = fiscalYears.ToDictionary(fy => fy.ID_Fiscal_Year, fy => fy.Fiscal_Year_Name);

                // 7. Cargar los estatus de proyecto y mapear el ID al nombre
                var statuses = db.CTZ_Project_Status.ToList();
                var statusMapping = statuses.ToDictionary(s => s.ID_Status, s => s.Description);

                // 8. Transformar finalPercentageDict a la estructura deseada, pero solo incluir líneas que estén en validLineIds
                var resultData = new List<object>();
                foreach (var lineEntry in finalPercentageDict)
                {
                    int lineId = lineEntry.Key;
                    if (!validLineIds.Contains(lineId))
                        continue; // Omitir esta línea

                    string lineName = linesDict.ContainsKey(lineId) ? linesDict[lineId] : "Line " + lineId;

                    // Recopilar todos los FY que tengan datos para esta línea
                    var fyIdSet = new HashSet<int>();
                    foreach (var statusEntry in lineEntry.Value)
                    {
                        foreach (var fyId in statusEntry.Value.Keys)
                        {
                            fyIdSet.Add(fyId);
                        }
                    }
                    var sortedFyIds = fyIdSet.OrderBy(f => f).ToList();

                    // Construir el arreglo DataByFY
                    var dataByFYList = new List<object>();
                    foreach (var fyId in sortedFyIds)
                    {
                        string fiscalYearName = fyDict.ContainsKey(fyId) ? fyDict[fyId] : "FY " + fyId;
                        var breakdownList = new List<object>();

                        // Para cada estatus, si existe dato para este FY, se incluye en el desglose
                        foreach (var statusEntry in lineEntry.Value)
                        {
                            int statusId = statusEntry.Key;
                            if (statusEntry.Value.ContainsKey(fyId))
                            {
                                double percentage = statusEntry.Value[fyId];
                                // Convertir a porcentaje convencional
                                double percentageValue = Math.Round(percentage * 100, 2);
                                string statusName = statusMapping.ContainsKey(statusId) ? statusMapping[statusId] : "Status " + statusId;
                                breakdownList.Add(new
                                {
                                    StatusId = statusName,
                                    StatusName = statusName,
                                    Percentage = percentageValue
                                });
                            }
                        }
                        dataByFYList.Add(new
                        {
                            FiscalYear = fiscalYearName,
                            Breakdown = breakdownList
                        });
                    }

                    resultData.Add(new
                    {
                        LineId = lineId,
                        LineName = lineName,
                        DataByFY = dataByFYList
                    });
                }

                // 9. Retornar la nueva estructura en JSON
                return Json(new { success = true, data = resultData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        #endregion


        #region clases extras
        public class SelectListItemComparer : IEqualityComparer<SelectListItem>
        {
            public bool Equals(SelectListItem x, SelectListItem y)
            {
                return x.Value == y.Value;
            }

            public int GetHashCode(SelectListItem obj)
            {
                return obj.Value.GetHashCode();
            }
        }


        #endregion
    }
    public class VehicleItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public string SOP { get; set; }
        public string EOP { get; set; }
        public string Program { get; set; }
        public string MaxProduction { get; set; }

        // Nueva propiedad: producción por año en formato JSON o como estructura serializable
        public string ProductionDataJson { get; set; }
    }
}
