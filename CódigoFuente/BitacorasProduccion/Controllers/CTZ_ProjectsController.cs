using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Newtonsoft.Json;
using Portal_2_0.Models;

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

            // Traer todos los registros de CTZ_Temp_IHS
            var tempIHSList = db.CTZ_Temp_IHS.ToList();

            // Traer todas las producciones y agruparlas por ID_IHS en un diccionario
            var productionLookup = db.CTZ_Temp_IHS_Production
                .Select(p => new { p.ID_IHS, p.Production_Year, p.Production_Sum })
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


            // Retornar la vista con el proyecto cargado y sus materiales.
            return View(project);
        }

        // POST: CTZ_Projects/EditClientPartInformation/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClientPartInformation(CTZ_Projects project, List<CTZ_Project_Materials> materials)
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

                        db.CTZ_Project_Materials.Add(material);
                    }
                }

                // Guardar cambios
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert("Client & Part Information updated successfully.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("EditProject", new { id = project.ID_Project });
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

            return View(project);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region extras
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
