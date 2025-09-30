using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using Portal_2_0.Models;
using SpreadsheetLight;
using HorizontalAlignmentValues = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues;

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
            // 1) Permisos de acceso
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
                return View("../Home/ErrorPermisos");

            // 2) Mensaje tras Upsert, SendQuote, etc.
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            // 3) Listas para filtros
            var clientListFromClients = db.CTZ_Clients
                .AsEnumerable()
                .Select(c => new SelectListItem { Value = c.Client_Name, Text = c.ConcatSAPName })
                .ToList();

            var clientListFromProjects = db.CTZ_Projects
                .Where(p => !string.IsNullOrEmpty(p.Cliente_Otro))
                .Select(p => p.Cliente_Otro)
                .Distinct()
                .AsEnumerable()
                .Select(c => new SelectListItem { Value = c, Text = c.ToUpper() })
                .ToList();

            var clientList = clientListFromClients
                .Union(clientListFromProjects, new SelectListItemComparer())
                .ToList();
            clientList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            ViewBag.ClientList = clientList;

            var facilityList = db.CTZ_plants
                .Select(f => new SelectListItem { Value = f.Description, Text = f.Description })
                .ToList();
            facilityList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            ViewBag.FacilityList = facilityList;

            var statusList = db.CTZ_Project_Status
                .AsEnumerable()
                .Select(s => new SelectListItem { Value = s.Description, Text = s.ConcatStatus })
                .ToList();
            statusList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            ViewBag.StatusList = statusList;

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

            // 4) Guardar valores de filtros en ViewBag
            ViewBag.SearchProject = searchProject;
            ViewBag.SearchClient = searchClient;
            ViewBag.SearchFacility = searchFacility;
            ViewBag.SearchStatus = searchStatus;
            ViewBag.SearchCreatedBy = searchCreatedBy;
            ViewBag.SearchDateStart = searchDateStart.HasValue ? searchDateStart.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.SearchDateEnd = searchDateEnd.HasValue ? searchDateEnd.Value.ToString("yyyy-MM-dd") : "";

            // 5) Permiso para Upsert
            int me = obtieneEmpleadoLogeado().id;
            var auth = new AuthorizationService(db);
            ViewBag.CanUpsert = auth.CanPerform(me, ResourceKey.UpsertQuotes, ActionKey.Edit);

            // 6) Construir query base
            var query = db.CTZ_Projects
                .Include(p => p.CTZ_Clients)
                .Include(p => p.CTZ_plants)
                .Include(p => p.CTZ_Project_Status)
                .Include(p => p.empleados)
                .AsQueryable();

            // 7) Aplicar filtros
            if (!string.IsNullOrEmpty(searchProject))
            {
                string lower = searchProject.ToLower();
                query = query.AsEnumerable()
                             .Where(p => p.ConcatQuoteID.ToLower().Contains(lower))
                             .AsQueryable();
            }
            if (searchClient != "0") query = query.Where(p =>
                (p.CTZ_Clients != null && p.CTZ_Clients.Client_Name == searchClient) ||
                (p.Cliente_Otro != null && p.Cliente_Otro == searchClient));
            if (searchFacility != "0") query = query.Where(p => p.CTZ_plants != null && p.CTZ_plants.Description == searchFacility);
            if (searchStatus != "0") query = query.Where(p => p.CTZ_Project_Status != null && p.CTZ_Project_Status.Description == searchStatus);
            if (searchCreatedBy != "0") query = query.Where(p => p.empleados != null && p.empleados.nombre == searchCreatedBy);
            if (searchDateStart.HasValue) query = query.Where(p => p.Creted_Date >= searchDateStart.Value);
            if (searchDateEnd.HasValue) query = query.Where(p => p.Creted_Date <= searchDateEnd.Value);

            // 8) Ejecutar y obtener lista
            var projects = query
                .OrderByDescending(x => x.Creted_Date)
                .ToList();

            // 9) Calcular CanEdit para cada proyecto
            //   - creador sin asignaciones
            //   - o pertenece a depto+planta asignados
            var userDeptIds = db.CTZ_Employee_Departments
                .Where(ed => ed.ID_Employee == me)
                .Select(ed => ed.ID_Department)
                .ToList();
            var userPlantIds = db.CTZ_Employee_Plants
                .Where(ep => ep.ID_Employee == me)
                .Select(ep => ep.ID_Plant)
                .ToList();

            foreach (var p in projects)
            {
                // 1) Es el creador y NUNCA se ha asignado: puede editar
                bool isCreatorAndUnassigned =
                    p.ID_Created_By == me
                    && !db.CTZ_Project_Assignment.Any(a => a.ID_Project == p.ID_Project);

                // 2) Busca la ÚLTIMA asignación para este proyecto + depto/planta del usuario
                var lastAssignmentInUserDeptPlant = db.CTZ_Project_Assignment
                    .Where(a =>
                        a.ID_Project == p.ID_Project
                        && userDeptIds.Contains(a.ID_Department)
                        && userPlantIds.Contains(a.ID_Plant)
                    )
                    .OrderByDescending(a => a.Assignment_Date)  // o a.ID_Assignment si Assignment_Date puede empatar
                    .FirstOrDefault();

                // Estados terminales que NO permiten edición
                var approvedStatus = (int)AssignmentStatusEnum.APPROVED;
                var rejectedStatus = (int)AssignmentStatusEnum.REJECTED;

                // 3) Si existe esa última asignación y NO está en Approved/Rejeted → puede editar
                bool isInAssignedDeptAndPlant =
                    lastAssignmentInUserDeptPlant != null
                    && lastAssignmentInUserDeptPlant.ID_Assignment_Status != approvedStatus
                    && lastAssignmentInUserDeptPlant.ID_Assignment_Status != rejectedStatus;

                // 4) Combina ambas condiciones
                p.CanEdit = isCreatorAndUnassigned || isInAssignedDeptAndPlant;
            }


            // 10) Devolver vista con la lista completa
            return View(projects);
        }



        // GET: CTZ_Projects/Upsert
        public ActionResult Upsert(int? id)
        {
            //valida permisos
            int me = obtieneEmpleadoLogeado().id;
            var auth = new AuthorizationService(db);
            var context = new Dictionary<string, object> { ["ProjectId"] = id };
            bool canUpsert = auth.CanPerform(me, ResourceKey.UpsertQuotes, ActionKey.Edit, context);

            // Validar que el usuario tenga el rol ADMIN; si no, mostrar error de permisos.
            if (!TieneRol(TipoRoles.CTZ_ACCESO) || !canUpsert)
            {
                ViewBag.Titulo = "Quote In Process";
                ViewBag.Descripcion =
                    "This quote has already been sent and is currently being processed. " +
                    "Editing is not permitted at this time. Please wait until the process completes or contact your administrator for assistance.";

                return View("../Home/ErrorGenerico");
            }

            // Obtener el empleado logueado y asignarlo al ViewBag.
            empleados empleadoLogeado = obtieneEmpleadoLogeado();
            ViewBag.EmpleadoLogeado = empleadoLogeado;

            //ontiene el modelo o crea uno vacio
            CTZ_Projects model;
            if (id == null)
            {
                // Create
                model = new CTZ_Projects();
            }
            else
            {
                // Edit: cargar existente
                model = db.CTZ_Projects.Find(id.Value);
                if (model == null) return HttpNotFound();
            }

            // Construir las listas para los dropdowns.
            ViewBag.ID_Client = new SelectList(db.CTZ_Clients, "ID_Cliente", "ConcatSAPName");
            ViewBag.ID_Material_Owner = new SelectList(db.CTZ_Material_Owner, "ID_Owner", "Owner_Key");
            ViewBag.ID_OEM = new SelectList(db.CTZ_OEMClients, "ID_OEM", "ConcatSAPName");
            ViewBag.ID_Plant = new SelectList(db.CTZ_plants, "ID_Plant", "Description");
            ViewBag.ID_VehicleType = new SelectList(db.CTZ_Vehicle_Types, "ID_VehicleType", "VehicleType_Name");
            ViewBag.ID_Status = new SelectList(db.CTZ_Project_Status, nameof(CTZ_Project_Status.ID_Status), nameof(CTZ_Project_Status.ConcatStatus));
            ViewBag.ID_Import_Business_Model = new SelectList(db.CTZ_Import_Business_Model, nameof(CTZ_Import_Business_Model.ID_Model), nameof(CTZ_Import_Business_Model.Description));
            ViewBag.ExternalProcessorList = new SelectList(db.CTZ_ExternalProcessors.Where(p => p.IsActive), "ID_ExternalProcessor", "Name", model.ID_ExternalProcessor);

            ViewBag.CountriesWithWarning = db.CTZ_Countries
            .Where(c => c.Active)
            .Select(c => new CountryWithWarningVm
            {
                ID_Country = c.ID_Country,
                ConcatKey = c.ISO3 + " - " + c.Nicename,
                Warning = c.Warning
            })
            .ToList();

            ViewBag.ID_Incoterm = new SelectList(
                db.CTZ_Incoterms.Where(i => i.Active),
                nameof(CTZ_Incoterms.ID_Incoterm),
                nameof(CTZ_Incoterms.ConcatEnglish) // usa ConcatEnglish para el texto :contentReference[oaicite:2]{index=2}&#8203;:contentReference[oaicite:3]{index=3}
            );


            // En el GET, por defecto, los checkboxes estarán sin marcar.
            ViewBag.OtherClient = false;
            ViewBag.OtherOEM = false;


            // Enviar un modelo nuevo para evitar errores.
            return View(model);
        }

        // POST: CTZ_Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(CTZ_Projects cTZ_Projects)
        {
            // Validar que el usuario tenga el rol ADMIN.
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
                return View("../Home/ErrorPermisos");

            // Capturar el estado de los checkboxes.
            bool otherClient = !string.IsNullOrEmpty(Request["chkOtherClient"]);
            bool otherOEM = !string.IsNullOrEmpty(Request["chkOtherOEM"]);
            // Guardarlos en el ViewBag para que la vista los mantenga.
            ViewBag.OtherClient = otherClient;
            ViewBag.OtherOEM = otherOEM;

            #region validacion

            // Validación para Client:
            if (otherClient)
            {
                if (string.IsNullOrWhiteSpace(cTZ_Projects.Cliente_Otro))
                {
                    ModelState.AddModelError("Cliente_Otro", "Other Client name is required when 'Other Client' is checked.");
                }
                cTZ_Projects.ID_Client = null;
            }
            else
            {
                if (!cTZ_Projects.ID_Client.HasValue || cTZ_Projects.ID_Client.Value == 0)
                {
                    ModelState.AddModelError("ID_Client", "Client selection is required.");
                }
                // Limpiamos los campos "Other" si no se usa el checkbox
                //cTZ_Projects.Cliente_Otro = null;
                //cTZ_Projects.OtherClient_Address = null;
                //cTZ_Projects.OtherClient_Telephone = null;
            }

            // Verificar el tipo de vehículo:
            if (cTZ_Projects.ID_VehicleType != 1)
            {
                // Si el vehículo no es "Automotriz", se ignoran los campos OEM
                cTZ_Projects.ID_OEM = null;
                cTZ_Projects.OEM_Otro = null;
                cTZ_Projects.OtherOEM_Address = null;
                cTZ_Projects.OtherOEM_Telephone = null;
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
                    //cTZ_Projects.OEM_Otro = null;
                    //cTZ_Projects.OtherOEM_Address = null;
                    //cTZ_Projects.OtherOEM_Telephone = null;
                }
            }

            //validar campos import required
            if (cTZ_Projects.ImportRequired)
            {
                if (cTZ_Projects.ID_Import_Business_Model == null)
                {
                    ModelState.AddModelError("ID_Import_Business_Model", "Business Model is required.");
                }
                if (cTZ_Projects.ID_Incoterm == null)
                {
                    ModelState.AddModelError("ID_Incoterm", "Incoterm is required.");
                }
                if (cTZ_Projects.ID_Country_Origin == null)
                {
                    ModelState.AddModelError("ID_Country_Origin", "Material Origin is required.");
                }
            }
            else
            {
                //vacia campos
                cTZ_Projects.ID_Import_Business_Model = null;
                cTZ_Projects.Comments_Import = null;
            }

            //validar campos material owner
            if (cTZ_Projects.ID_Material_Owner == 1)  // 1 ->  OWN
            {

                if (cTZ_Projects.ID_Incoterm == null)
                {
                    ModelState.AddModelError("ID_Incoterm", "Incoterm is required.");
                }
                if (cTZ_Projects.ID_Country_Origin == null)
                {
                    ModelState.AddModelError("ID_Country_Origin", "Material Origin is required.");
                }
            }
            else
            {
                //vacia campos               
                cTZ_Projects.Mults = null;
                cTZ_Projects.Comments_Material_Owner = null;
            }

            //encaso de que import required == false y material owner != OWN, borra los campos comunes
            if (!cTZ_Projects.ImportRequired && cTZ_Projects.ID_Material_Owner != 1)
            {
                cTZ_Projects.ID_Incoterm = null;
                cTZ_Projects.ID_Country_Origin = null;
            }

            #endregion

            if (ModelState.IsValid)
            {
                if (cTZ_Projects.ID_Project == 0)
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
                else
                {
                    // UPDATE
                    var existing = db.CTZ_Projects.Find(cTZ_Projects.ID_Project);
                    if (existing == null) return HttpNotFound();

                    // copiar sólo los campos que vengan del form...
                    // por ejemplo:
                    existing.ID_Client = cTZ_Projects.ID_Client;
                    existing.Cliente_Otro = cTZ_Projects.Cliente_Otro;
                    existing.ID_OEM = cTZ_Projects.ID_OEM;
                    existing.OEM_Otro = cTZ_Projects.OEM_Otro;
                    existing.ID_Material_Owner = cTZ_Projects.ID_Material_Owner;
                    existing.ID_Plant = cTZ_Projects.ID_Plant;
                    existing.ID_Status = cTZ_Projects.ID_Status;
                    existing.ID_VehicleType = cTZ_Projects.ID_VehicleType;
                    existing.ImportRequired = cTZ_Projects.ImportRequired;
                    existing.ID_Import_Business_Model = cTZ_Projects.ID_Import_Business_Model;
                    existing.ID_Incoterm = cTZ_Projects.ID_Incoterm;
                    existing.ID_Country_Origin = cTZ_Projects.ID_Country_Origin;
                    existing.Comments_Import = cTZ_Projects.Comments_Import;
                    existing.Comments_Material_Owner = cTZ_Projects.Comments_Material_Owner;
                    existing.Comments = cTZ_Projects.Comments;
                    existing.Mults = cTZ_Projects.Mults;
                    existing.ID_Incoterm = cTZ_Projects.ID_Incoterm;
                    existing.ID_Country_Origin = cTZ_Projects.ID_Country_Origin;
                    existing.Update_Date = DateTime.Now;
                    existing.ID_Updated_By = obtieneEmpleadoLogeado().id;
                    existing.OtherClient_Address = cTZ_Projects.OtherClient_Address;
                    existing.OtherClient_Telephone = cTZ_Projects.OtherClient_Telephone;
                    existing.OtherOEM_Address = cTZ_Projects.OtherOEM_Address;
                    existing.OtherOEM_Telephone = cTZ_Projects.OtherOEM_Telephone;

                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert("Project updated successfully.", TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("EditProject", new { id = cTZ_Projects.ID_Project });

                }
            }

            // Si hay errores, repoblar los dropdowns y el empleado logueado.
            ViewBag.ID_Client = new SelectList(db.CTZ_Clients, "ID_Cliente", "ConcatSAPName", cTZ_Projects.ID_Client);
            ViewBag.ID_Material_Owner = new SelectList(db.CTZ_Material_Owner, "ID_Owner", "Owner_Key", cTZ_Projects.ID_Material_Owner);
            ViewBag.ID_OEM = new SelectList(db.CTZ_OEMClients, "ID_OEM", "Client_Name", cTZ_Projects.ID_OEM);
            ViewBag.ID_Plant = new SelectList(db.CTZ_plants, "ID_Plant", "Description", cTZ_Projects.ID_Plant);
            ViewBag.ID_Status = new SelectList(db.CTZ_Project_Status, nameof(CTZ_Project_Status.ID_Status), nameof(CTZ_Project_Status.ConcatStatus), cTZ_Projects.ID_Status);
            ViewBag.ID_VehicleType = new SelectList(db.CTZ_Vehicle_Types, "ID_VehicleType", "VehicleType_Name", cTZ_Projects.ID_VehicleType);
            ViewBag.ID_Import_Business_Model = new SelectList(db.CTZ_Import_Business_Model, nameof(CTZ_Import_Business_Model.ID_Model), nameof(CTZ_Import_Business_Model.Description));
            ViewBag.ExternalProcessorList = new SelectList(db.CTZ_ExternalProcessors.Where(p => p.IsActive), "ID_ExternalProcessor", "Name", cTZ_Projects.ID_ExternalProcessor);

            ViewBag.EmpleadoLogeado = obtieneEmpleadoLogeado();

            // Si hay errores, repoblar los dropdowns
            ViewBag.CountriesWithWarning = db.CTZ_Countries
            .Where(c => c.Active)
            .Select(c => new CountryWithWarningVm
            {
                ID_Country = c.ID_Country,
                ConcatKey = c.ISO3 + " - " + c.Nicename,
                Warning = c.Warning
            })
            .ToList();

            ViewBag.ID_Incoterm = new SelectList(
                db.CTZ_Incoterms.Where(i => i.Active),
                "ID_Incoterm", "ConcatEnglish",
                cTZ_Projects.ID_Incoterm
            );

            return View(cTZ_Projects);
        }

        // GET: CTZ_Projects/EditProject/{id}
        public ActionResult EditProject(int id, string expandedSection = "collapseOne", int? versionId = null)
        {
            // 1) Permisos base
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
                return View("../Home/ErrorPermisos");

            // 2) Mensaje flash
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            // 3) Cargo el proyecto con sus materiales
            var project = db.CTZ_Projects
                .Include(p => p.CTZ_Project_Materials)
                .FirstOrDefault(p => p.ID_Project == id);
            if (project == null)
                return HttpNotFound();

            // 3bis) Preparar el ViewModel
            var vm = new EditProjectViewModel
            {
                CTZ_Project = project,
                ID_Version = versionId,
                ID_Project = project.ID_Project,
                // los demás campos de versión los llenamos más abajo
                Materials = new List<MaterialViewModel>()
            };

            //Se llenan los datos
            if (versionId.HasValue)
            {
                // 1) Recupero la fila de CTZ_Projects_Versions
                var ver = db.CTZ_Projects_Versions
                            .FirstOrDefault(v => v.ID_Version == versionId.Value);

                if (ver != null)
                {
                    vm.ID_Version = ver.ID_Version;
                    vm.ID_Project = ver.ID_Project;
                    vm.ID_Created_by = ver.ID_Created_by;
                    vm.Version_Number = ver.Version_Number;
                    vm.Creation_Date = ver.Creation_Date;
                    vm.Is_Current = ver.Is_Current;
                    vm.Comments = ver.Comments;
                    vm.ID_Status_Project = ver.ID_Status_Project;
                    vm.CTZ_Project_Status = ver.CTZ_Project_Status;
                    vm.empleados = ver.empleados;
                }

                // 2) Cargo snapshot de materiales
                vm.Materials = db.CTZ_Project_Materials_History
                     //.Include(h => h.CTZ_Route)
                     .Where(h => h.ID_Version == versionId.Value)
                     .Select(h => new MaterialViewModel
                     {
                         ID_Material = h.ID_History,
                         ID_IHS_Item = h.ID_IHS_Item,
                         Max_Production_SP = h.Max_Production_SP,
                         Program_SP = h.Program_SP,
                         Vehicle_version = h.Vehicle_version,
                         SOP_SP = h.SOP_SP,
                         EOP_SP = h.EOP_SP,
                         Real_SOP = h.Real_SOP,
                         Real_EOP = h.Real_EOP,
                         Ship_To = h.Ship_To,
                         Part_Name = h.Part_Name,
                         Part_Number = h.Part_Number,
                         ID_Route = h.ID_Route,
                         Quality = h.Quality,
                         Tensile_Strenght = h.Tensile_Strenght,
                         ID_Material_type = h.ID_Material_type,
                         Thickness = h.Thickness,
                         Width = h.Width,
                         Pitch = h.Pitch,
                         Theoretical_Gross_Weight = h.Theoretical_Gross_Weight,
                         Gross_Weight = h.Gross_Weight,
                         Annual_Volume = h.Annual_Volume,
                         Volume_Per_year = h.Volume_Per_year,
                         ID_Shape = h.ID_Shape,
                         Angle_A = h.Angle_A,
                         Angle_B = h.Angle_B,
                         Blanks_Per_Stroke = h.Blanks_Per_Stroke,
                         Parts_Per_Vehicle = h.Parts_Per_Vehicle,
                         ID_Theoretical_Blanking_Line = h.ID_Theoretical_Blanking_Line,
                         ID_Real_Blanking_Line = h.ID_Real_Blanking_Line,
                         Theoretical_Strokes = h.Theoretical_Strokes,
                         Real_Strokes = h.Real_Strokes,
                         Ideal_Cycle_Time_Per_Tool = h.Ideal_Cycle_Time_Per_Tool,
                         OEE = h.OEE,
                         ID_Project = h.ID_Project,
                         Vehicle = h.Vehicle,
                         Vehicle_2 = h.Vehicle_2,
                         Vehicle_3 = h.Vehicle_3,
                         Vehicle_4 = h.Vehicle_4,
                         ThicknessToleranceNegative = h.ThicknessToleranceNegative,
                         ThicknessTolerancePositive = h.ThicknessTolerancePositive,
                         WidthToleranceNegative = h.WidthToleranceNegative,
                         WidthTolerancePositive = h.WidthTolerancePositive,
                         PitchToleranceNegative = h.PitchToleranceNegative,
                         PitchTolerancePositive = h.PitchTolerancePositive,
                         WeightOfFinalMults = h.WeightOfFinalMults,
                         Multipliers = h.Multipliers,
                         AngleAToleranceNegative = h.AngleAToleranceNegative,
                         AngleATolerancePositive = h.AngleATolerancePositive,
                         AngleBToleranceNegative = h.AngleBToleranceNegative,
                         AngleBTolerancePositive = h.AngleBTolerancePositive,
                         MajorBase = h.MajorBase,
                         MajorBaseToleranceNegative = h.MajorBaseToleranceNegative,
                         MajorBaseTolerancePositive = h.MajorBaseTolerancePositive,
                         MinorBase = h.MinorBase,
                         MinorBaseToleranceNegative = h.MinorBaseToleranceNegative,
                         MinorBaseTolerancePositive = h.MinorBaseTolerancePositive,
                         Flatness = h.Flatness,
                         FlatnessToleranceNegative = h.FlatnessToleranceNegative,
                         FlatnessTolerancePositive = h.FlatnessTolerancePositive,
                         MasterCoilWeight = h.MasterCoilWeight,
                         InnerCoilDiameterArrival = h.InnerCoilDiameterArrival,
                         OuterCoilDiameterArrival = h.OuterCoilDiameterArrival,
                         InnerCoilDiameterDelivery = h.InnerCoilDiameterDelivery,
                         OuterCoilDiameterDelivery = h.OuterCoilDiameterDelivery,
                         PackagingStandard = h.PackagingStandard,
                         SpecialRequirement = h.SpecialRequirement,
                         SpecialPackaging = h.SpecialPackaging,
                         ID_File_CAD_Drawing = h.ID_File_CAD_Drawing,
                         TurnOver = h.TurnOver,
                         DM_status = h.DM_status,
                         DM_status_comment = h.DM_status_comment,
                         CTZ_Files = h.CTZ_Files,
                         CTZ_Material_Type = h.CTZ_Material_Type,
                         CTZ_Production_Lines = h.CTZ_Production_Lines,
                         CTZ_Production_Lines1 = h.CTZ_Production_Lines1,
                         CTZ_Projects = h.CTZ_Projects,
                         CTZ_Route = h.CTZ_Route,
                         SCDM_cat_forma_material = h.SCDM_cat_forma_material,
                         Width_Mults = h.Width_Mults,
                         Width_Mults_Tol_Pos = h.Width_Mults_Tol_Pos,
                         Width_Mults_Tol_Neg = h.Width_Mults_Tol_Neg,
                         Width_Plates = h.Width_Plates,
                         Width_Plates_Tol_Pos = h.Width_Plates_Tol_Pos,
                         Width_Plates_Tol_Neg = h.Width_Plates_Tol_Neg,
                         Initial_Weight = (h.Annual_Volume.HasValue && h.Volume_Per_year.HasValue && h.Annual_Volume != 0)
                            ? (h.Volume_Per_year / h.Annual_Volume * 1000)
                            : (double?)null,
                     })
                     .ToList();
            }
            else
            {
                // Edición normal: materiales vivos
                vm.Materials = project.CTZ_Project_Materials
                    .Select(m => new MaterialViewModel
                    {
                        ID_Material = m.ID_Material,
                        ID_IHS_Item = m.ID_IHS_Item,
                        Max_Production_SP = m.Max_Production_SP,
                        Program_SP = m.Program_SP,
                        Vehicle_version = m.Vehicle_version,
                        SOP_SP = m.SOP_SP,
                        EOP_SP = m.EOP_SP,
                        Real_SOP = m.Real_SOP,
                        Real_EOP = m.Real_EOP,
                        Ship_To = m.Ship_To,
                        Part_Name = m.Part_Name,
                        Part_Number = m.Part_Number,
                        ID_Route = m.ID_Route,
                        Quality = m.Quality,
                        Tensile_Strenght = m.Tensile_Strenght,
                        ID_Material_type = m.ID_Material_type,
                        Thickness = m.Thickness,
                        Width = m.Width,
                        Pitch = m.Pitch,
                        Theoretical_Gross_Weight = m.Theoretical_Gross_Weight,
                        Gross_Weight = m.Gross_Weight,
                        Annual_Volume = m.Annual_Volume,
                        Volume_Per_year = m.Volume_Per_year,
                        ID_Shape = m.ID_Shape,
                        Angle_A = m.Angle_A,
                        Angle_B = m.Angle_B,
                        Blanks_Per_Stroke = m.Blanks_Per_Stroke,
                        Parts_Per_Vehicle = m.Parts_Per_Vehicle,
                        ID_Theoretical_Blanking_Line = m.ID_Theoretical_Blanking_Line,
                        ID_Real_Blanking_Line = m.ID_Real_Blanking_Line,
                        Theoretical_Strokes = m.Theoretical_Strokes,
                        Real_Strokes = m.Real_Strokes,
                        Ideal_Cycle_Time_Per_Tool = m.Ideal_Cycle_Time_Per_Tool,
                        OEE = m.OEE,
                        ID_Project = m.ID_Project,
                        Vehicle = m.Vehicle,
                        Vehicle_2 = m.Vehicle_2,
                        Vehicle_3 = m.Vehicle_3,
                        Vehicle_4 = m.Vehicle_4,
                        ThicknessToleranceNegative = m.ThicknessToleranceNegative,
                        ThicknessTolerancePositive = m.ThicknessTolerancePositive,
                        WidthToleranceNegative = m.WidthToleranceNegative,
                        WidthTolerancePositive = m.WidthTolerancePositive,
                        PitchToleranceNegative = m.PitchToleranceNegative,
                        PitchTolerancePositive = m.PitchTolerancePositive,
                        WeightOfFinalMults = m.WeightOfFinalMults,
                        Multipliers = m.Multipliers,
                        AngleAToleranceNegative = m.AngleAToleranceNegative,
                        AngleATolerancePositive = m.AngleATolerancePositive,
                        AngleBToleranceNegative = m.AngleBToleranceNegative,
                        AngleBTolerancePositive = m.AngleBTolerancePositive,
                        MajorBase = m.MajorBase,
                        MajorBaseToleranceNegative = m.MajorBaseToleranceNegative,
                        MajorBaseTolerancePositive = m.MajorBaseTolerancePositive,
                        MinorBase = m.MinorBase,
                        MinorBaseToleranceNegative = m.MinorBaseToleranceNegative,
                        MinorBaseTolerancePositive = m.MinorBaseTolerancePositive,
                        Flatness = m.Flatness,
                        FlatnessToleranceNegative = m.FlatnessToleranceNegative,
                        FlatnessTolerancePositive = m.FlatnessTolerancePositive,
                        MasterCoilWeight = m.MasterCoilWeight,
                        InnerCoilDiameterArrival = m.InnerCoilDiameterArrival,
                        OuterCoilDiameterArrival = m.OuterCoilDiameterArrival,
                        InnerCoilDiameterDelivery = m.InnerCoilDiameterDelivery,
                        OuterCoilDiameterDelivery = m.OuterCoilDiameterDelivery,
                        PackagingStandard = m.PackagingStandard,
                        SpecialRequirement = m.SpecialRequirement,
                        SpecialPackaging = m.SpecialPackaging,
                        ID_File_CAD_Drawing = m.ID_File_CAD_Drawing,
                        TurnOver = m.TurnOver,
                        DM_status = m.DM_status,
                        DM_status_comment = m.DM_status_comment,
                        CTZ_Files = m.CTZ_Files,
                        CTZ_Material_Type = m.CTZ_Material_Type,
                        CTZ_Production_Lines = m.CTZ_Production_Lines,
                        CTZ_Production_Lines1 = m.CTZ_Production_Lines1,
                        CTZ_Projects = m.CTZ_Projects,
                        CTZ_Route = m.CTZ_Route,
                        SCDM_cat_forma_material = m.SCDM_cat_forma_material,
                        Width_Mults = m.Width_Mults,
                        Width_Mults_Tol_Pos = m.Width_Mults_Tol_Pos,
                        Width_Mults_Tol_Neg = m.Width_Mults_Tol_Neg,
                        Width_Plates = m.Width_Plates,
                        Width_Plates_Tol_Pos = m.Width_Plates_Tol_Pos,
                        Width_Plates_Tol_Neg = m.Width_Plates_Tol_Neg,
                        Initial_Weight = (m.Annual_Volume.HasValue && m.Volume_Per_year.HasValue && m.Annual_Volume != 0)
                            ? (m.Volume_Per_year / m.Annual_Volume * 1000)
                            : (double?)null,
                    })
                    .ToList();
            }

            // 4) Identifico al usuario y sus departamentos
            empleados LoggedEmployee = obtieneEmpleadoLogeado();
            int me = LoggedEmployee.id;
            var userDeptIds = db.CTZ_Employee_Departments
                .Where(ed => ed.ID_Employee == me)
                .Select(ed => ed.ID_Department)
                .ToList();

            bool isSales = userDeptIds.Contains((int)DepartmentEnum.Sales);
            bool isEngineering = userDeptIds.Contains((int)DepartmentEnum.Engineering);
            bool isForeignTrade = userDeptIds.Contains((int)DepartmentEnum.ForeignTrade);
            bool isDisposition = userDeptIds.Contains((int)DepartmentEnum.Disposition);
            bool isDataMgmt = userDeptIds.Contains((int)DepartmentEnum.DataManagement);

            //valida permisos
            var context = new Dictionary<string, object> { ["ProjectId"] = id };
            var auth = new AuthorizationService(db);
            bool canUpsert = auth.CanPerform(me, ResourceKey.UpsertQuotes, ActionKey.Edit, context);
            bool cantEditClientPartInformationSalesSection = auth.CanPerform(me, ResourceKey.EditClientPartInformationSalesSection, ActionKey.Edit, context);
            bool canEditClientPartInformationEngineeringSection = auth.CanPerform(me, ResourceKey.EditClientPartInformationEngineeringSection, ActionKey.Edit, context);
            bool canEditClientPartInformationDataManagementSection = auth.CanPerform(me, ResourceKey.EditClientPartInformationDataManagementSection, ActionKey.Edit, context);

            // Envia si admin o no
            ViewBag.IsAdmin = LoggedEmployee.CTZ_Roles.Any(x => x.ID_Role == (int)CTZ_RolesEnum.ADMIN);

            ViewBag.CanUpsert = canUpsert;
            ViewBag.CantEditClientPartInformationSalesSection = cantEditClientPartInformationSalesSection;
            ViewBag.CanEditClientPartInformationEngineeringSection = canEditClientPartInformationEngineeringSection;
            ViewBag.CanEditClientPartInformationDataManagementSection = canEditClientPartInformationDataManagementSection;

            // 5) Obtengo asignaciones activas/completadas
            var activeDepts = db.CTZ_Project_Assignment
                .Where(a => a.ID_Project == id && a.Completition_Date == null)
                .Select(a => a.ID_Department)
                .ToList();
            var doneDepts = db.CTZ_Project_Assignment
                .Where(a => a.ID_Project == id && a.Completition_Date != null)
                .Select(a => a.ID_Department)
                .ToList();

            bool engDone = doneDepts.Contains((int)DepartmentEnum.Engineering);
            bool ftDone = !project.ImportRequired || doneDepts.Contains((int)DepartmentEnum.ForeignTrade);
            bool dpDone = project.ID_Material_Owner != 1 || doneDepts.Contains((int)DepartmentEnum.Disposition);

            // 6) Calcular NextDepartments para el mensaje dinámico
            var nextDepts = new List<string>();
            if (isSales)
            {
                nextDepts.Add("Engineering");
                if (project.ImportRequired) nextDepts.Add("Foreign Trade");
                if (project.ID_Material_Owner == 1) nextDepts.Add("Disposition");
            }
            else if (isEngineering && engDone && ftDone && dpDone)
            {
                nextDepts.Add("Data Management");
            }
            // si no encaja en ninguno, al menos devolvemos un array vacío
            ViewBag.NextDepartments = nextDepts;

            // 7) Flags para mostrar botones
            // Sales → no enviado aún a Engineering
            ViewBag.CanSendSales = isSales ||
                activeDepts.Contains((int)DepartmentEnum.Sales);

            // Foreign Trade → solo si ya envió Sales y aún no se completó FT
            ViewBag.CanSendForeignTrade = isForeignTrade &&
                activeDepts.Contains((int)DepartmentEnum.ForeignTrade);

            // Disposition → solo si ya envió Sales y aún no se completó Disposition
            ViewBag.CanSendDisposition = isDisposition &&
                activeDepts.Contains((int)DepartmentEnum.Disposition);

            // — ENGINEERING: si es Eng y hay asignación Eng activa
            ViewBag.CanSendEngineering = isEngineering
                && activeDepts.Contains((int)DepartmentEnum.Engineering);

            // DataManagement → cuando Sales, FT, Dispo e Ingeniería hayan completado
            ViewBag.CanSendDataManagement = isDataMgmt &&
                activeDepts.Contains((int)DepartmentEnum.DataManagement);

            // Finalizar → si DataMgmt tiene asignación pendiente
            ViewBag.CanFinalize = isDataMgmt && !activeDepts.Any();

            // 8) Sección expandida según permisos de edición
            ViewBag.CanEditSales = isSales;
            ViewBag.CanEditEng = isEngineering;
            ViewBag.CanEditDM = isDataMgmt;
            if (ViewBag.CanEditSales) expandedSection = "collapseOne";
            else if (ViewBag.CanEditEng) expandedSection = "collapseTwo";
            else if (ViewBag.CanEditDM) expandedSection = "collapseThree";
            ViewBag.ExpandedSection = expandedSection;


            #region carga actividades
            // por simplicidad, si el usuario pertenece a varios, tomamos el primero:

            int? currentDept = null;
            List<CTZ_Department_Activity> activities = new List<CTZ_Department_Activity>();
            List<int> completedIds = new List<int>();
            string currentStatus = "No department assigned";

            // 2) Si tiene al menos un depto, cargamos, si no, dejamos en blanco
            if (userDeptIds.Any())
            {
                currentDept = userDeptIds.First();

                // 3) Definiciones de actividades para ese depto
                activities = db.CTZ_Department_Activity
                               .Where(a => a.ID_Department == currentDept && a.Active)
                               .ToList();

                // 4) Cargar actividad completadas de la asignación activa
                var assignment = db.CTZ_Project_Assignment
                    .Where(a =>
                        a.ID_Project == id &&
                        a.ID_Department == currentDept &&
                        a.Completition_Date == null
                    )
                    .OrderByDescending(a => a.Assignment_Date)
                    .FirstOrDefault();

                currentStatus = assignment != null
                    ? assignment.CTZ_Assignment_Status.Status_Name
                    : "No assignment";

                if (assignment != null)
                {
                    completedIds = db.CTZ_Assignment_Activity
                        .Where(x => x.ID_Assignment == assignment.ID_Assignment && x.IsComplete)
                        .Select(x => x.ID_Activity)
                        .ToList();
                }
            }

            // 5) ¿Ya existe al menos una asignación para este proyecto?
            bool hasAnyAssignment = db.CTZ_Project_Assignment
                .Any(a => a.ID_Project == id);

            // 6) Pasamos todo al ViewBag
            ViewBag.HasAnyAssignment = hasAnyAssignment;
            ViewBag.CurrentDept = currentDept;
            ViewBag.Activities = activities;
            ViewBag.CompletedActs = completedIds;
            ViewBag.CurrentAssignmentStatus = currentStatus;
            #endregion

            //carga razones de rechazo por depto
            #region razones de rechazo

            // 1) Obtén todas las razones válidas para este depto
            var rawReasons = db.CTZ_RejectionReason_Department
                .Where(rd => rd.ID_Department == currentDept && rd.CTZ_RejectionReason.Active)
                .Select(rd => rd.CTZ_RejectionReason)
                .Distinct()
                .ToList();

            // 2) Mapea a RejectionReasonOption
            var options = rawReasons
                .Select(r => new RejectionReasonOption
                {
                    ID_Reason = r.ID_Reason,
                    Name = r.Name.Trim(),
                    ActionType = r.ActionType,
                    ReassignToDept = r.ReassignDepartmentId
                })
                .ToList();

            // 3) Agrega la opción “Other…”
            options.Add(new RejectionReasonOption
            {
                ID_Reason = 0,
                Name = "Other…",
                ActionType = (byte)ActionTypeEnum.KeepActive,
                ReassignToDept = null
            });


            // pásalo directamente a ViewBag para construir el <select> a mano
            ViewBag.RejectionReasons = options;


            #endregion

            //9. Obtener los rechazos
            #region Rechazos Comentarios
            var deptIds = new[]
            {
                (int)DepartmentEnum.Sales,
                (int)DepartmentEnum.Engineering,
                (int)DepartmentEnum.ForeignTrade,
                (int)DepartmentEnum.Disposition,
                (int)DepartmentEnum.DataManagement
            };

            // Para cada depto, traemos su última asignación (por Assignment_Date),
            // y sólo nos quedamos con aquellas cuya última asignación esté en estado REJECTED
            var activeRejections = deptIds
                .Select(deptId =>
                    db.CTZ_Project_Assignment
                      .Where(a => a.ID_Project == id && a.ID_Department == deptId)
                      // ordenamos por Assignment_Date descendente -> la más reciente primero
                      .OrderByDescending(a => a.Assignment_Date)
                      .FirstOrDefault()
                )
                // filtramos: existencia y que efectivamente esté REJECTED
                .Where(a => a != null && a.ID_Assignment_Status == (int)AssignmentStatusEnum.REJECTED)
                .Select(a => new ActiveRejection
                {
                    Dept = ((DepartmentEnum)a.ID_Department).ToString().Replace("_", " "),
                    Comment = a.Comments,
                    // la fecha en la que se completó el rechazo
                    DateRejection = a.Completition_Date.HasValue ? a.Completition_Date.Value : a.Last_Status_Change.HasValue ? a.Last_Status_Change.Value : DateTime.Now,

                })
                .ToList();

            ViewBag.ActiveRejections = activeRejections;
            #endregion

            //// Sólo para DataManagement
            if (currentDept == (int)DepartmentEnum.DataManagement)
            {
                var reassignOptions = new[]
                    {
                        DepartmentEnum.Sales,
                        DepartmentEnum.Engineering,
                        DepartmentEnum.ForeignTrade,
                        DepartmentEnum.Disposition
                    }
                    .Select(d => new DeptReassignOption
                    {
                        Id = (int)d,
                        Name = d.ToString().Replace("_", " ")
                    })
                    .ToList();

                // mejor aún: usa un SelectList directo
                ViewBag.ReassignOptions = new SelectList(reassignOptions, "Id", "Name");
            }

            return View(vm);
        }


        #region New Versions
        // GET: muestra el formulario
        public ActionResult NewVersion(int id)
        {
            // Dentro de tu GET NewVersion(int id)
            var project = db.CTZ_Projects.Find(id);
            if (project == null) return HttpNotFound();

            // calcula la versión actual y la siguiente
            var latestVerStr = db.CTZ_Projects_Versions
                .Where(v => v.ID_Project == id && v.Is_Current == true)
                .Select(v => v.Version_Number)
                .FirstOrDefault() ?? "0.0";

            decimal lastDec = 0m;
            Decimal.TryParse(latestVerStr, out lastDec);
            decimal nextDec = lastDec < 0.1m ? 0.1m
                            : (lastDec < 1.0m ? 1.0m
                            : lastDec + 0.1m);

            ViewBag.ProjectName = project.ConcatQuoteID;
            ViewBag.CurrentVersion = latestVerStr;
            ViewBag.NextVersion = nextDec.ToString("0.0");
            ViewBag.LoggedUserName = obtieneEmpleadoLogeado().ConcatNombre;
            ViewBag.CreationDate = DateTime.Now.ToString("g");

            var vm = new NewVersionViewModel { ID_Project = id };
            return View(vm);
        }

        // POST: procesa la creación
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewVersion(NewVersionViewModel vm)
        {
            // 0) Validación manual de Comments
            if (string.IsNullOrWhiteSpace(vm.Comments))
            {
                ModelState.AddModelError(nameof(vm.Comments), "Comments are required.");
            }
            else if (vm.Comments.Length > 255)
            {
                ModelState.AddModelError(nameof(vm.Comments), "Comments cannot exceed 255 characters.");
            }

            // Dentro de tu GET NewVersion(int id)
            var project = db.CTZ_Projects.Find(vm.ID_Project);
            if (project == null) return HttpNotFound();

            // calcula la versión actual y la siguiente
            var latestVerStr = db.CTZ_Projects_Versions
                .Where(v => v.ID_Project == vm.ID_Project && v.Is_Current == true)
                .Select(v => v.Version_Number)
                .FirstOrDefault() ?? "0.0";

            decimal lastDec = 0m;
            Decimal.TryParse(latestVerStr, out lastDec);
            decimal nextDec = lastDec < 0.1m ? 0.1m
                            : (lastDec < 1.0m ? 1.0m
                            : lastDec + 0.1m);

            ViewBag.ProjectName = project.ConcatQuoteID;
            ViewBag.CurrentVersion = latestVerStr;
            ViewBag.NextVersion = nextDec.ToString("0.0");
            ViewBag.LoggedUserName = obtieneEmpleadoLogeado().ConcatNombre;
            ViewBag.CreationDate = DateTime.Now.ToString("g");


            //valida que no haya errores en el modelo
            if (!ModelState.IsValid) return View(vm);

            var userId = obtieneEmpleadoLogeado().id;
            var statusId = db.CTZ_Projects
                             .Where(p => p.ID_Project == vm.ID_Project)
                             .Select(p => p.ID_Status)
                             .FirstOrDefault();

            // 1) Crear la nueva versión
            var newVer = VersionService.CreateNewVersion(
                vm.ID_Project, userId, statusId, vm.Comments, DateTime.Now);

            // 2) Copiar los materiales vivos al history de esa versión
            HistoryHelper.CopyMaterialsToHistory(vm.ID_Project, newVer.ID_Version);

            TempData["Mensaje"] = new MensajesSweetAlert("New version created successfully.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("EditProject", new { id = vm.ID_Project });
        }
        #endregion

        [HttpGet]
        public ActionResult ManageAssignments(int id)
        {
            var project = db.CTZ_Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            ViewBag.DepartmentList = new SelectList(db.CTZ_Departments, "ID_Department", "Name");
            ViewBag.UserList = new SelectList(db.empleados.ToList().Where(e => e.activo == true), nameof(empleados.id), nameof(empleados.ConcatNombre));

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignToDepartment(int projectId, int departmentId, string comments)
        {
            try
            {
                // TODO: Lógica para guardar la nueva asignación
                TempData["SuccessMessage"] = "Project successfully assigned.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error assigning project: " + ex.Message;
            }
            return RedirectToAction("ManageAssignments", new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CloseOnBehalfOf(int projectId, int userId, string comments)
        {
            try
            {
                // TODO: Lógica para cerrar la tarea en nombre de otro usuario
                TempData["SuccessMessage"] = "Task successfully closed on behalf of the user.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error closing task: " + ex.Message;
            }
            return RedirectToAction("ManageAssignments", new { id = projectId });
        }

        [HttpPost]
        public JsonResult FinalizeActivity(int projectId)
        {
            //var me = obtieneEmpleadoLogeado();
            //// 1) Marcar asignación DataManagement como completada
            //var assign = db.CTZ_Project_Assignment
            //    .FirstOrDefault(a => a.ID_Project == projectId
            //                      && a.ID_Department == (int)DepartmentEnum.DataManagement
            //                      && a.Completition_Date == null);
            //if (assign != null)
            //{
            //    assign.Completition_Date = DateTime.Now;
            //    db.SaveChanges();
            //}

            //// 2) Marcar la cotización cerrada (puede ser un campo en CTZ_Projects)
            //var proj = db.CTZ_Projects.Find(projectId);
            //proj.ID_Status = (int)ProjectStatus.Closed;
            //db.SaveChanges();

            //// 3) Enviar correo al solicitante (empleado que creó)
            //var creator = db.empleados.Find(proj.ID_Created_By);
            //if (!string.IsNullOrEmpty(creator.correo))
            //{
            //    var mail = new EnvioCorreoElectronico();
            //    mail.SendEmailAsync(
            //      new[] { creator.correo },
            //      "Your quote is complete",
            //      $"Your quote {proj.ConcatQuoteID} has been finalized on {DateTime.Now:dd/MM/yyyy}."
            //    );
            //}

            return Json(new { message = "Quote finalized and requester notified." });
        }

        // GET: CTZ_Projects/Edit/{id}
        public ActionResult Edit(int id)
        {
            // Validar permisos: si el usuario no tiene rol ADMIN, mostrar error.
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
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
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
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
        public ActionResult ViewClientPartInformationHistory(int id, int versionId)
        {
            // Validar permisos: si el usuario no tiene rol ADMIN, mostrar error.
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
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

            #region viewModel
            // Inicializa el ViewModel existente
            var vm = new EditProjectViewModel
            {
                CTZ_Project = project,
                CTZ_Project_Status = project.CTZ_Project_Status,
                empleados = project.empleados,
                ID_Version = versionId,
                ID_Project = project.ID_Project,
                ID_Status_Project = project.CTZ_Project_Status.ID_Status
            };

            // 1.1) Si es snapshot, cargo la versión

            var ver = db.CTZ_Projects_Versions.Find(versionId);
            vm.Version_Number = ver.Version_Number;
            vm.Creation_Date = ver.Creation_Date;
            vm.Is_Current = ver.Is_Current;
            vm.Comments = ver.Comments;

            // Materia-les desde history
            vm.Materials = db.CTZ_Project_Materials_History
                .Where(h => h.ID_Version == versionId)
                .Select(h => new MaterialViewModel
                {
                    ID_Material = h.ID_History,
                    ID_IHS_Item = h.ID_IHS_Item,
                    Max_Production_SP = h.Max_Production_SP,
                    Program_SP = h.Program_SP,
                    Vehicle_version = h.Vehicle_version,
                    SOP_SP = h.SOP_SP,
                    EOP_SP = h.EOP_SP,
                    Real_SOP = h.Real_SOP,
                    Real_EOP = h.Real_EOP,
                    Ship_To = h.Ship_To,
                    Part_Name = h.Part_Name,
                    Part_Number = h.Part_Number,
                    ID_Route = h.ID_Route,
                    Quality = h.Quality,
                    Tensile_Strenght = h.Tensile_Strenght,
                    ID_Material_type = h.ID_Material_type,
                    Thickness = h.Thickness,
                    Width = h.Width,
                    Pitch = h.Pitch,
                    Theoretical_Gross_Weight = h.Theoretical_Gross_Weight,
                    Gross_Weight = h.Gross_Weight,
                    Annual_Volume = h.Annual_Volume,
                    Volume_Per_year = h.Volume_Per_year,
                    ID_Shape = h.ID_Shape,
                    Angle_A = h.Angle_A,
                    Angle_B = h.Angle_B,
                    Blanks_Per_Stroke = h.Blanks_Per_Stroke,
                    Parts_Per_Vehicle = h.Parts_Per_Vehicle,
                    ID_Theoretical_Blanking_Line = h.ID_Theoretical_Blanking_Line,
                    ID_Real_Blanking_Line = h.ID_Real_Blanking_Line,
                    Theoretical_Strokes = h.Theoretical_Strokes,
                    Real_Strokes = h.Real_Strokes,
                    Ideal_Cycle_Time_Per_Tool = h.Ideal_Cycle_Time_Per_Tool,
                    OEE = h.OEE,
                    ID_Project = h.ID_Project,
                    Vehicle = h.Vehicle,
                    Vehicle_2 = h.Vehicle_2,
                    Vehicle_3 = h.Vehicle_3,
                    Vehicle_4 = h.Vehicle_4,
                    ThicknessToleranceNegative = h.ThicknessToleranceNegative,
                    ThicknessTolerancePositive = h.ThicknessTolerancePositive,
                    WidthToleranceNegative = h.WidthToleranceNegative,
                    WidthTolerancePositive = h.WidthTolerancePositive,
                    PitchToleranceNegative = h.PitchToleranceNegative,
                    PitchTolerancePositive = h.PitchTolerancePositive,
                    WeightOfFinalMults = h.WeightOfFinalMults,
                    Multipliers = h.Multipliers,
                    AngleAToleranceNegative = h.AngleAToleranceNegative,
                    AngleATolerancePositive = h.AngleATolerancePositive,
                    AngleBToleranceNegative = h.AngleBToleranceNegative,
                    AngleBTolerancePositive = h.AngleBTolerancePositive,
                    MajorBase = h.MajorBase,
                    MajorBaseToleranceNegative = h.MajorBaseToleranceNegative,
                    MajorBaseTolerancePositive = h.MajorBaseTolerancePositive,
                    MinorBase = h.MinorBase,
                    MinorBaseToleranceNegative = h.MinorBaseToleranceNegative,
                    MinorBaseTolerancePositive = h.MinorBaseTolerancePositive,
                    Flatness = h.Flatness,
                    FlatnessToleranceNegative = h.FlatnessToleranceNegative,
                    FlatnessTolerancePositive = h.FlatnessTolerancePositive,
                    MasterCoilWeight = h.MasterCoilWeight,
                    InnerCoilDiameterArrival = h.InnerCoilDiameterArrival,
                    OuterCoilDiameterArrival = h.OuterCoilDiameterArrival,
                    InnerCoilDiameterDelivery = h.InnerCoilDiameterDelivery,
                    OuterCoilDiameterDelivery = h.OuterCoilDiameterDelivery,
                    PackagingStandard = h.PackagingStandard,
                    SpecialRequirement = h.SpecialRequirement,
                    SpecialPackaging = h.SpecialPackaging,
                    ID_File_CAD_Drawing = h.ID_File_CAD_Drawing,
                    TurnOver = h.TurnOver,
                    DM_status = h.DM_status,
                    DM_status_comment = h.DM_status_comment,
                    CTZ_Production_Lines = h.CTZ_Production_Lines,
                    CTZ_Production_Lines1 = h.CTZ_Production_Lines1,
                    CTZ_Files = h.CTZ_Files,
                    SCDM_cat_forma_material = h.SCDM_cat_forma_material,
                    CTZ_Material_Type = h.CTZ_Material_Type,
                    CTZ_Projects = h.CTZ_Projects,
                    CTZ_Route = h.CTZ_Route,
                    TurnOverSide = h.TurnOverSide,
                })
                .ToList();


            #endregion

            #region permisos

            // **Permiso específico para cada sección, cambiar por enum**
            ViewBag.CanEditSales = false;
            ViewBag.CanEditDataManagement = false;
            ViewBag.CanEditEngineering = false;

            #endregion

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
            return View(vm);
        }

        // GET: CTZ_Projects/EditClientPartInformation/{id}
        public ActionResult EditClientPartInformation(int id, bool details = false)
        {
            // Validar permisos: si el usuario no tiene rol ADMIN, mostrar error.
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
                return View("../Home/ErrorPermisos");

            // Buscar el proyecto por id junto con sus materiales relacionados.
            var project = db.CTZ_Projects
                .Include(p => p.CTZ_Project_Materials.Select(m => m.CTZ_Route))
                .Include(p => p.CTZ_Project_Materials.Select(m => m.CTZ_Files)) // <--- AÑADE ESTA LÍNEA (para CAD)
                .Include(p => p.CTZ_Project_Materials.Select(m => m.CTZ_Files1)) // <--- AÑADE ESTA LÍNEA (para Packaging)
                .Include(p => p.CTZ_Project_Materials.Select(m => m.CTZ_Material_RackTypes))
                .Include(p => p.CTZ_Project_Materials.Select(m => m.CTZ_Material_Additionals))
                .Include(p => p.CTZ_Project_Materials.Select(m => m.CTZ_Material_Labels))
                .Include(p => p.CTZ_Project_Materials.Select(m => m.CTZ_Material_StrapTypes))
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

            // Ahora que los datos están cargados, llenamos la lista de IDs para cada material.
            if (project.CTZ_Project_Materials != null)
            {
                var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                foreach (var material in project.CTZ_Project_Materials)
                {
                    material.SelectedRackTypeIds = material.CTZ_Material_RackTypes.Select(rt => rt.ID_RackType).ToList();
                    material.SelectedAdditionalIds = material.CTZ_Material_Additionals.Select(a => a.ID_Additional).ToList();
                    material.SelectedLabelIds = material.CTZ_Material_Labels.Select(l => l.ID_LabelType).ToList();
                    material.SelectedStrapTypeIds = material.CTZ_Material_StrapTypes.Select(st => st.ID_StrapType).ToList();

                    if (material.IsWeldedBlank == true && material.CTZ_Material_WeldedPlates.Any())
                    {
                        var plates = material.CTZ_Material_WeldedPlates
                                             .Select(p => new { p.PlateNumber, p.Thickness })
                                             .OrderBy(p => p.PlateNumber)
                                             .ToList();
                        material.WeldedPlatesJson = jsonSerializer.Serialize(plates);
                    }
                }


            }

            #region permisos

            int me = obtieneEmpleadoLogeado().id;
            var auth = new AuthorizationService(db);

            // Contexto necesario para “AssignedToProject”
            var context = new Dictionary<string, object> { ["ProjectId"] = id };

            // Ver permiso global de ver la página
            if (!auth.CanPerform(me, ResourceKey.EditClientPartInformationView, ActionKey.View, context))
            {
                ViewBag.Titulo = "Insufficient Permissions";
                ViewBag.Descripcion =
                    "You do not have permission to view this section. " +
                    "Please contact your administrator to grant you this seccition. ResourceKey: " + ResourceKey.EditClientPartInformationView + ", ActionKey: " + ActionKey.View;

                return View("../Home/ErrorGenerico");
            }


            // **Permiso específico para cada sección, cambiar por enum**
            ViewBag.CanEditSales = auth.CanPerform(me, ResourceKey.EditClientPartInformationSalesSection, ActionKey.Edit, context);
            ViewBag.CanEditDataManagement = auth.CanPerform(me, ResourceKey.EditClientPartInformationDataManagementSection, ActionKey.Edit, context);
            ViewBag.CanEditEngineering = auth.CanPerform(me, ResourceKey.EditClientPartInformationEngineeringSection, ActionKey.Edit, context);


            #endregion

            #region Combo Lists
            // Cargar lista para "Coil Position"
            ViewBag.CoilPositionList = new SelectList(db.CTZ_Coil_Position.Where(cp => cp.Active), "ID_Coil_Position", nameof(CTZ_Coil_Position.ConcatDescription));

            // 1. Obtener la lista de tipos de transporte de la BD
            var transportTypes = db.CTZ_Transport_Types.Where(t => t.activo).ToList();
            // 2. Encontrar el elemento "Other" (asumiendo que su ID es 5, como en nuestro script anterior)
            var otherOption = transportTypes.FirstOrDefault(t => t.ID_Transport_Type == 5);
            // 3. Si se encuentra "Other", lo quitamos de su posición actual y lo agregamos al final
            if (otherOption != null)
            {
                transportTypes.Remove(otherOption);
                transportTypes.Add(otherOption);
            }
            // 4. Crear el SelectList con la lista ya ordenada
            ViewBag.ArrivalTransportTypeList = new SelectList(transportTypes, "ID_Transport_Type", "descripcion");

            // Cargar lista para "Rack Type"
            ViewBag.RackTypeList = new SelectList(db.CTZ_Packaging_RackType.Where(r => r.IsActive), "ID_RackType", "RackTypeName");
            // Cargar lista para "Strap Type"
            ViewBag.StrapTypeList = new SelectList(db.CTZ_Packaging_StrapType.Where(s => s.IsActive), "ID_StrapType", "StrapTypeName");
            ///-- lABEL OTHER AL FINAL ---
            // 1. Obtener todos los labels activos de la BD
            var allLabels = db.CTZ_Packaging_LabelType.Where(l => l.IsActive).ToList();
            // 2. Encontrar el label "Other" por su ID (3)
            var otherLabel = allLabels.FirstOrDefault(l => l.ID_LabelType == 3);
            // 3. Si existe, lo removemos y lo añadimos al final
            if (otherLabel != null)
            {
                allLabels.Remove(otherLabel);
                allLabels.Add(otherLabel);
            }
            // 4. Crear el SelectList para el ViewBag con la lista ya ordenada
            ViewBag.LabelList = new SelectList(allLabels, nameof(CTZ_Packaging_LabelType.ID_LabelType), nameof(CTZ_Packaging_LabelType.LabelTypeName));

            ///-- Aditional OTHER AL FINAL ---

            // 1. Obtener todos los additionals activos de la BD
            var allAdditionals = db.CTZ_Packaging_Additionals.Where(a => a.IsActive).ToList();
            // 2. Encontrar el additional "Other" por su ID (6)
            var otherAdditional = allAdditionals.FirstOrDefault(a => a.ID_Additional == 6);
            // 3. Si existe, lo removemos y lo añadimos al final
            if (otherAdditional != null)
            {
                allAdditionals.Remove(otherAdditional);
                allAdditionals.Add(otherAdditional);
            }
            // 4. Crear el SelectList para el ViewBag con la lista ya ordenada
            ViewBag.AdditionalList = new SelectList(allAdditionals, nameof(CTZ_Packaging_Additionals.ID_Additional), nameof(CTZ_Packaging_Additionals.AdditionalName));

            var allIhsCountries = db.CTZ_Temp_IHS
                          .Select(i => i.Country)
                          .Distinct()
                          .OrderBy(c => c)
                          .ToList();

            // Proyectamos la lista de strings a una lista de objetos anónimos
            var countrySelectListItems = allIhsCountries.Select(c => new { Value = c, Text = c }).ToList();

            // Usamos el constructor de SelectList que define explícitamente el campo para el valor y el texto.
            ViewBag.IHSCountries = new SelectList(countrySelectListItems, "Value", "Text", "MEX");

            string defaultCountry = "MEX";
            var defaultTempIHSList = db.CTZ_Temp_IHS.Where(i => i.Country == defaultCountry).ToList();

            // Paso 1: Extraer los IDs de la lista a una nueva variable de tipo simple (List<int>).
            var defaultIhsIds = defaultTempIHSList.Select(i => i.ID_IHS).ToList();

            // Paso 2: Usar esta nueva lista de IDs en la consulta.
            var defaultProductionLookup = db.CTZ_Temp_IHS_Production
                  .Where(p => defaultIhsIds.Contains(p.ID_IHS)) // <-- Consulta corregida
                  .ToList()
                  .GroupBy(p => p.ID_IHS)
                  .ToDictionary(g => g.Key, g => g.Select(p => new { p.Production_Year, p.Production_Month, p.Production_Amount }).ToList());

            // El resto de la lógica no cambia...
            var defaultVehicles = defaultTempIHSList.Select(x => new VehicleItem
            {
                Value = x.ConcatCodigo,
                Text = x.ConcatCodigo,
                SOP = x.SOP.HasValue ? x.SOP.Value.ToString("yyyy-MM") : "",
                EOP = x.EOP.HasValue ? x.EOP.Value.ToString("yyyy-MM") : "",
                Program = x.Program,
                MaxProduction = x.Max_Production.ToString(),
                ProductionDataJson = defaultProductionLookup.ContainsKey(x.ID_IHS)
                    ? JsonConvert.SerializeObject(defaultProductionLookup[x.ID_IHS])
                    : "[]"
            }).OrderBy(v => v.Value).ToList();

            ViewBag.VehicleList = defaultVehicles;


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
            // --- INICIO DE LA MODIFICACIÓN: LÓGICA DINÁMICA PARA MATERIAL TYPE LIST ---

            // ID_Plant
            int plantId = project.ID_Plant;

            // Declaramos la lista que vamos a llenar.
            List<object> availableMaterialTypes;

            // Verificamos si la planta es Saltillo (ID = 3)
            if (plantId == 3)
            {
                // LÓGICA PARA SALTILLO (ALMACÉN): Cargar TODOS los materiales activos.
                availableMaterialTypes = db.CTZ_Material_Type
                    .Where(mt => mt.Active)
                    .OrderBy(mt => mt.Material_Name)
                    .Select(mt => new
                    {
                        Value = mt.ID_Material_Type,
                        Text = mt.Material_Name
                    })
                    .ToList<object>(); // Convertimos a List<object> para consistencia
            }
            else
            {
                // LÓGICA PARA OTRAS PLANTAS: Mantenemos el filtro original por línea de producción.
                var productionLineIds = db.CTZ_Production_Lines
                    .Where(l => l.ID_Plant == plantId && l.Active)
                    .Select(l => l.ID_Line)
                    .ToList();

                var materialTypeIds = db.CTZ_Material_Type_Lines
                    .Where(mt => productionLineIds.Contains(mt.ID_Line))
                    .Select(mt => mt.ID_Material_Type)
                    .Distinct();

                availableMaterialTypes = db.CTZ_Material_Type
                    .Where(mt => materialTypeIds.Contains(mt.ID_Material_Type) && mt.Active)
                    .OrderBy(mt => mt.Material_Name)
                    .Select(mt => new
                    {
                        Value = mt.ID_Material_Type,
                        Text = mt.Material_Name
                    })
                    .ToList<object>(); // Convertimos a List<object> para consistencia
            }

            // Finalmente, creamos el SelectList con la lista que se haya generado.
            ViewBag.MaterialTypeList = new SelectList(availableMaterialTypes, "Value", "Text");

            // --- FIN DE LA MODIFICACIÓN ---
            var shapeList = db.SCDM_cat_forma_material
                        .Where(s => new int[] { 2, 18, 3 }.Contains(s.id)) //2 = Rectangular, 18 = configurado, 3 = Trapecio
                        .ToList()
                        .Select(s => new
                        {
                            Value = s.id,
                            Text = s.ConcatKey
                        })
                        .ToList();

            ViewBag.ShapeList = new SelectList(shapeList, "Value", "Text");

            // Obtener la lista de lineas de produccion según la planta
            var LinesList = db.CTZ_Production_Lines.Where(x => x.Active && x.ID_Plant == plantId).ToList()
                .Select(s => new
                {
                    Value = s.ID_Line,
                    Text = s.Description
                })
                .ToList();
            ViewBag.LinesList = new SelectList(LinesList, "Value", "Text");

            // Cargar las reglas de Slitting activas para mostrarlas en la advertencia
            ViewBag.SlittingRules = db.CTZ_Slitting_Validation_Rules
             .Include(r => r.CTZ_Production_Lines) // Incluir el nombre de la línea
             .Where(r => r.Is_Active && r.ID_Production_Line == 8) // Asegúrate de que slitterLineId esté disponible o ajusta el filtro
             .OrderBy(r => r.CTZ_Production_Lines.Line_Name)
             .ThenBy(r => r.Thickness_Min)
             .ThenBy(r => r.Tensile_Min)
             .Select(r => new SlittingRuleViewModel // Usamos un ViewModel simple para pasar solo los datos necesarios
             {
                 LineName = r.CTZ_Production_Lines.Description,
                 ThicknessRange = (r.Thickness_Min ?? 0) + " - " + (r.Thickness_Max ?? 0) + " mm",
                 TensileRange = (r.Tensile_Min ?? 0) + " - " + (r.Tensile_Max ?? 0) + " N/mm²",
                 WidthRange = (r.Width_Min ?? 0) + " - " + (r.Width_Max ?? 0) + " mm", // <-- AGREGA ESTA LÍNEA
                 MaxStrips = r.Mults_Max
             })
             .ToList();

            ViewBag.FreightTypeList = new SelectList(
               db.CTZ_FreightTypes.Where(f => f.Active),
               nameof(CTZ_FreightTypes.ID_FreightType),
               nameof(CTZ_FreightTypes.Description) // Usaremos la descripción completa
           );

            // Cargar lista para Warehouses (filtrada por la planta del proyecto)
            ViewBag.WarehouseList = new SelectList(
                db.CTZ_Arrival_Warehouses.Where(w => w.Active && w.ID_Plant == project.ID_Plant),
                nameof(CTZ_Arrival_Warehouses.ID_Warehouse),
                nameof(CTZ_Arrival_Warehouses.WarehouseName)
            );

            #endregion

            // Pasamos el modo "detalles" a la vista a través del ViewBag
            ViewBag.IsDetailsMode = details;
            ViewBag.ProjectPlantId = project.ID_Plant;

            // Retornar la vista con el proyecto cargado y sus materiales.
            return View(project);
        }

        // POST: CTZ_Projects/EditClientPartInformation/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClientPartInformation(CTZ_Projects project, List<CTZ_Project_Materials> materials, HttpPostedFileBase archivo, HttpPostedFileBase packaging_archivo,
            HttpPostedFileBase technicalSheetFile, HttpPostedFileBase AdditionalFile, HttpPostedFileBase arrivalAdditionalFile, HttpPostedFileBase coilDataAdditionalFile, HttpPostedFileBase slitterDataAdditionalFile, HttpPostedFileBase volumeAdditionalFile,
            HttpPostedFileBase outboundFreightAdditionalFile, HttpPostedFileBase deliveryPackagingAdditionalFile
            )
        {
            // Validar permisos
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
                return View("../Home/ErrorPermisos");

            // Valida el modelo antes de cualquier operación
            if (!ModelState.IsValid)
            {
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

                var allIhsCountries = db.CTZ_Temp_IHS
                                  .Select(i => i.Country)
                                  .Distinct()
                                  .OrderBy(c => c)
                                  .ToList();
                ViewBag.IHSCountries = new SelectList(allIhsCountries, "MEX");

                // --- INICIO DE LA MODIFICACIÓN ---
                // Cargar lista para "Rack Type"
                ViewBag.RackTypeList = new SelectList(db.CTZ_Packaging_RackType.Where(r => r.IsActive), "ID_RackType", "RackTypeName");
                // Cargar lista para "Additionals"
                ViewBag.AdditionalList = new SelectList(db.CTZ_Packaging_Additionals.Where(a => a.IsActive), "ID_Additional", "AdditionalName");
                // Cargar lista para "Strap Type"
                ViewBag.StrapTypeList = new SelectList(db.CTZ_Packaging_StrapType.Where(s => s.IsActive), "ID_StrapType", "StrapTypeName");
                ViewBag.LabelList = new SelectList(db.CTZ_Packaging_LabelType.Where(l => l.IsActive).ToList(), nameof(CTZ_Packaging_LabelType.ID_LabelType), nameof(CTZ_Packaging_LabelType.LabelTypeName));
                // Cargar lista para "Coil Position"
                ViewBag.CoilPositionList = new SelectList(db.CTZ_Coil_Position.Where(cp => cp.Active), "ID_Coil_Position", nameof(CTZ_Coil_Position.ConcatDescription));
                ViewBag.ArrivalTransportTypeList = new SelectList(db.CTZ_Transport_Types.Where(t => t.activo), "ID_Transport_Type", "descripcion");

                return View(project);
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {

                    // 1. CREAMOS UNA FUNCIÓN AUXILIAR REUTILIZABLE PARA GUARDAR ARCHIVOS.
                    //    Esto nos ahorrará mucho código repetido.
                    // Función auxiliar reutilizable para guardar un archivo en la BD
                    Func<HttpPostedFileBase, int?> saveFileToDb = (fileToSave) =>
                    {
                        if (fileToSave != null && fileToSave.ContentLength > 0)
                        {
                            byte[] fileData;
                            using (var binaryReader = new System.IO.BinaryReader(fileToSave.InputStream))
                            {
                                fileData = binaryReader.ReadBytes(fileToSave.ContentLength);
                            }
                            var newFile = new CTZ_Files
                            {
                                Name = System.IO.Path.GetFileName(fileToSave.FileName), // Se puede agregar lógica de saneamiento aquí
                                MineType = fileToSave.ContentType,
                                Data = fileData
                            };
                            db.CTZ_Files.Add(newFile);
                            db.SaveChanges();
                            System.Diagnostics.Debug.WriteLine($"Archivo '{newFile.Name}' guardado en BD con ID: {newFile.ID_File}");
                            return newFile.ID_File;
                        }
                        return null;
                    };


                    #region guarda y limpia archivos
                    // 2. PROCESAMOS TODOS LOS ARCHIVOS ANTES DEL BUCLE.
                    //    Guardamos los nuevos archivos (si se subieron) y obtenemos sus IDs.
                    int? newFileId_CAD = saveFileToDb(archivo);
                    int? newFileId_Packaging = saveFileToDb(packaging_archivo);
                    int? newFileId_TechnicalSheet = saveFileToDb(technicalSheetFile);
                    int? newFileId_Additional = saveFileToDb(AdditionalFile);
                    int? newFileId_ArrivalAdditional = saveFileToDb(arrivalAdditionalFile);
                    int? newFileId_CoilDataAdditional = saveFileToDb(coilDataAdditionalFile);
                    int? newFileId_SlitterDataAdditional = saveFileToDb(slitterDataAdditionalFile);
                    int? newFileId_VolumeAdditional = saveFileToDb(volumeAdditionalFile);
                    int? newFileId_OutboundFreightAdditional = saveFileToDb(outboundFreightAdditionalFile);
                    int? newFileId_DeliveryPackagingAdditional = saveFileToDb(deliveryPackagingAdditionalFile);

                    // Obtener el proyecto existente
                    var existingProject = db.CTZ_Projects
             .Include(p => p.CTZ_Project_Materials.Select(m => m.CTZ_Material_WeldedPlates)) // Incluir las platinas soldadas
             .FirstOrDefault(p => p.ID_Project == project.ID_Project);

                    if (existingProject == null)
                    {
                        return HttpNotFound();
                    }


                    var fileIdsToDelete = new List<int>();

                    // Lógica para CAD
                    // --- Lógica para ID_File_CAD_Drawing ---
                    var oldCadFileLinks = existingProject.CTZ_Project_Materials.Where(m => m.ID_File_CAD_Drawing.HasValue).Select(m => new { m.ID_Material, FileId = m.ID_File_CAD_Drawing.Value }).ToList();
                    foreach (var oldLink in oldCadFileLinks)
                    {
                        var newMaterial = materials?.FirstOrDefault(m => m.ID_Material == oldLink.ID_Material);
                        // Borrar si: 1) El material ya no existe, 2) Se subió un archivo nuevo (reemplazo), 3) Se desvinculó el archivo.
                        if (newMaterial == null || newMaterial.IsFile == true || !newMaterial.ID_File_CAD_Drawing.HasValue)
                        {
                            fileIdsToDelete.Add(oldLink.FileId);
                        }
                    }

                    // --- Lógica para ID_File_Packaging ---
                    var oldPackagingFileLinks = existingProject.CTZ_Project_Materials.Where(m => m.ID_File_Packaging.HasValue).Select(m => new { m.ID_Material, FileId = m.ID_File_Packaging.Value }).ToList();
                    foreach (var oldLink in oldPackagingFileLinks)
                    {
                        var newMaterial = materials?.FirstOrDefault(m => m.ID_Material == oldLink.ID_Material);
                        if (newMaterial == null || newMaterial.IsPackagingFile == true || !newMaterial.ID_File_Packaging.HasValue)
                        {
                            fileIdsToDelete.Add(oldLink.FileId);
                        }
                    }

                    // --- Lógica para ID_File_TechnicalSheet ---
                    var oldTechSheetFileLinks = existingProject.CTZ_Project_Materials.Where(m => m.ID_File_TechnicalSheet.HasValue).Select(m => new { m.ID_Material, FileId = m.ID_File_TechnicalSheet.Value }).ToList();
                    foreach (var oldLink in oldTechSheetFileLinks)
                    {
                        var newMaterial = materials?.FirstOrDefault(m => m.ID_Material == oldLink.ID_Material);
                        if (newMaterial == null || newMaterial.IsTechnicalSheetFile == true || !newMaterial.ID_File_TechnicalSheet.HasValue)
                        {
                            fileIdsToDelete.Add(oldLink.FileId);
                        }
                    }

                    // --- Lógica para ID_File_Additional ---
                    var oldAdditionalFileLinks = existingProject.CTZ_Project_Materials.Where(m => m.ID_File_Additional.HasValue).Select(m => new { m.ID_Material, FileId = m.ID_File_Additional.Value }).ToList();
                    foreach (var oldLink in oldAdditionalFileLinks)
                    {
                        var newMaterial = materials?.FirstOrDefault(m => m.ID_Material == oldLink.ID_Material);
                        if (newMaterial == null || newMaterial.IsAdditionalFile == true || !newMaterial.ID_File_Additional.HasValue)
                        {
                            fileIdsToDelete.Add(oldLink.FileId);
                        }
                    }

                    // --- Lógica para ID_File_ArrivalAdditional ---
                    var oldArrivalAdditionalFileLinks = existingProject.CTZ_Project_Materials.Where(m => m.ID_File_ArrivalAdditional.HasValue).Select(m => new { m.ID_Material, FileId = m.ID_File_ArrivalAdditional.Value }).ToList();
                    foreach (var oldLink in oldArrivalAdditionalFileLinks)
                    {
                        var newMaterial = materials?.FirstOrDefault(m => m.ID_Material == oldLink.ID_Material);
                        if (newMaterial == null || newMaterial.IsArrivalAdditionalFile == true || !newMaterial.ID_File_ArrivalAdditional.HasValue)
                        {
                            fileIdsToDelete.Add(oldLink.FileId);
                        }
                    }


                    // Lógica para Coil Data Additional
                    var oldCoilDataFileLinks = existingProject.CTZ_Project_Materials.Where(m => m.ID_File_CoilDataAdditional.HasValue).Select(m => new { m.ID_Material, FileId = m.ID_File_CoilDataAdditional.Value }).ToList();
                    foreach (var oldLink in oldCoilDataFileLinks)
                    {
                        var newMaterial = materials?.FirstOrDefault(m => m.ID_Material == oldLink.ID_Material);
                        if (newMaterial == null || newMaterial.IsCoilDataAdditionalFile == true || !newMaterial.ID_File_CoilDataAdditional.HasValue)
                        {
                            fileIdsToDelete.Add(oldLink.FileId);
                        }
                    }

                    // Lógica para Slitter Data Additional
                    var oldSlitterDataFileLinks = existingProject.CTZ_Project_Materials.Where(m => m.ID_File_SlitterDataAdditional.HasValue).Select(m => new { m.ID_Material, FileId = m.ID_File_SlitterDataAdditional.Value }).ToList();
                    foreach (var oldLink in oldSlitterDataFileLinks)
                    {
                        var newMaterial = materials?.FirstOrDefault(m => m.ID_Material == oldLink.ID_Material);
                        if (newMaterial == null || newMaterial.IsSlitterDataAdditionalFile == true || !newMaterial.ID_File_SlitterDataAdditional.HasValue)
                        {
                            fileIdsToDelete.Add(oldLink.FileId);
                        }
                    }

                    // Lógica para Volume Additional
                    var oldVolumeFileLinks = existingProject.CTZ_Project_Materials.Where(m => m.ID_File_VolumeAdditional.HasValue).Select(m => new { m.ID_Material, FileId = m.ID_File_VolumeAdditional.Value }).ToList();
                    foreach (var oldLink in oldVolumeFileLinks)
                    {
                        var newMaterial = materials?.FirstOrDefault(m => m.ID_Material == oldLink.ID_Material);
                        if (newMaterial == null || newMaterial.IsVolumeAdditionalFile == true || !newMaterial.ID_File_VolumeAdditional.HasValue)
                        {
                            fileIdsToDelete.Add(oldLink.FileId);
                        }
                    }

                    // Lógica para Outbound Freight Additional
                    var oldOutboundFreightFileLinks = existingProject.CTZ_Project_Materials.Where(m => m.ID_File_OutboundFreightAdditional.HasValue).Select(m => new { m.ID_Material, FileId = m.ID_File_OutboundFreightAdditional.Value }).ToList();
                    foreach (var oldLink in oldOutboundFreightFileLinks)
                    {
                        var newMaterial = materials?.FirstOrDefault(m => m.ID_Material == oldLink.ID_Material);
                        if (newMaterial == null || newMaterial.IsOutboundFreightAdditionalFile == true || !newMaterial.ID_File_OutboundFreightAdditional.HasValue)
                        {
                            fileIdsToDelete.Add(oldLink.FileId);
                        }
                    }

                    // Lógica para Delivery Packaging Additional
                    var oldDeliveryPackagingFileLinks = existingProject.CTZ_Project_Materials.Where(m => m.ID_File_DeliveryPackagingAdditional.HasValue).Select(m => new { m.ID_Material, FileId = m.ID_File_DeliveryPackagingAdditional.Value }).ToList();
                    foreach (var oldLink in oldDeliveryPackagingFileLinks)
                    {
                        var newMaterial = materials?.FirstOrDefault(m => m.ID_Material == oldLink.ID_Material);
                        if (newMaterial == null || newMaterial.IsDeliveryPackagingAdditionalFile == true || !newMaterial.ID_File_DeliveryPackagingAdditional.HasValue)
                        {
                            fileIdsToDelete.Add(oldLink.FileId);
                        }
                    }

                    // Eliminamos explícitamente los hijos (RackTypes) y luego los padres (Materials).
                    // Esto asegura que EF entienda el orden correcto de eliminación.
                    foreach (var mat in existingProject.CTZ_Project_Materials.ToList())
                    {
                        db.CTZ_Material_RackTypes.RemoveRange(mat.CTZ_Material_RackTypes);
                        db.CTZ_Material_Labels.RemoveRange(mat.CTZ_Material_Labels);
                        db.CTZ_Material_Additionals.RemoveRange(mat.CTZ_Material_Additionals);
                        db.CTZ_Material_StrapTypes.RemoveRange(mat.CTZ_Material_StrapTypes);
                    }
                    db.CTZ_Project_Materials.RemoveRange(existingProject.CTZ_Project_Materials);

                    #endregion

                    // Guardamos la eliminación. Ahora la BD está limpia de los registros viejos.
                    db.SaveChanges();

                    // Agregar los materiales recibidos en la lista "materials" 
                    if (materials != null)
                    {
                        foreach (var material in materials)
                        {
                            //si teorical blk line es cero converir a null
                            if (material.ID_Real_Blanking_Line == 0)
                                material.ID_Real_Blanking_Line = null;
                            if (material.ID_Theoretical_Blanking_Line == 0)
                                material.ID_Theoretical_Blanking_Line = null;

                            if (material.ID_Slitting_Line == 0) material.ID_Slitting_Line = null;


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

                            // 1. Archivo CAD (Lógica existente, pero con depuración)
                            // Si este material tiene la bandera Y se subió un archivo, se asigna el ID.
                            if (material.IsFile == true && newFileId_CAD.HasValue) material.ID_File_CAD_Drawing = newFileId_CAD;
                            if (material.IsPackagingFile == true && newFileId_Packaging.HasValue) material.ID_File_Packaging = newFileId_Packaging;
                            if (material.IsTechnicalSheetFile == true && newFileId_TechnicalSheet.HasValue) material.ID_File_TechnicalSheet = newFileId_TechnicalSheet;
                            if (material.IsAdditionalFile == true && newFileId_Additional.HasValue) material.ID_File_Additional = newFileId_Additional;
                            if (material.IsArrivalAdditionalFile == true && newFileId_ArrivalAdditional.HasValue) material.ID_File_ArrivalAdditional = newFileId_ArrivalAdditional;
                            if (material.IsCoilDataAdditionalFile == true && newFileId_CoilDataAdditional.HasValue) material.ID_File_CoilDataAdditional = newFileId_CoilDataAdditional;
                            if (material.IsSlitterDataAdditionalFile == true && newFileId_SlitterDataAdditional.HasValue) material.ID_File_SlitterDataAdditional = newFileId_SlitterDataAdditional;
                            if (material.IsVolumeAdditionalFile == true && newFileId_VolumeAdditional.HasValue) material.ID_File_VolumeAdditional = newFileId_VolumeAdditional;
                            if (material.IsOutboundFreightAdditionalFile == true && newFileId_OutboundFreightAdditional.HasValue) material.ID_File_OutboundFreightAdditional = newFileId_OutboundFreightAdditional;
                            if (material.IsDeliveryPackagingAdditionalFile == true && newFileId_DeliveryPackagingAdditional.HasValue) material.ID_File_DeliveryPackagingAdditional = newFileId_DeliveryPackagingAdditional;

                            db.CTZ_Project_Materials.Add(material);


                            // --- INICIO DE LA NUEVA LÓGICA PARA PLATINAS SOLDADAS ---
                            if (material.IsWeldedBlank == true && !string.IsNullOrEmpty(material.WeldedPlatesJson))
                            {
                                var plates = new System.Web.Script.Serialization.JavaScriptSerializer()
                                                .Deserialize<List<WeldedPlateInfo>>(material.WeldedPlatesJson);

                                if (plates != null)
                                {
                                    foreach (var plate in plates)
                                    {
                                        var newPlate = new CTZ_Material_WeldedPlates
                                        {
                                            ID_Material = material.ID_Material, // Asignamos el nuevo ID del material recién guardado
                                            PlateNumber = plate.PlateNumber,
                                            Thickness = (double)plate.Thickness
                                        };
                                        db.CTZ_Material_WeldedPlates.Add(newPlate);
                                    }
                                }
                            }
                            // --- FIN DE LA NUEVA LÓGICA ---

                            var rackTypesToAdd = new List<CTZ_Material_RackTypes>();
                            if (material.SelectedRackTypeIds != null && material.SelectedRackTypeIds.Any())
                            {
                                foreach (var rackTypeId in material.SelectedRackTypeIds)
                                {
                                    rackTypesToAdd.Add(new CTZ_Material_RackTypes { ID_RackType = rackTypeId });
                                }
                            }

                            var labelsToAdd = new List<CTZ_Material_Labels>();
                            if (material.SelectedLabelIds != null && material.SelectedLabelIds.Any())
                            {
                                foreach (var labelId in material.SelectedLabelIds)
                                {
                                    labelsToAdd.Add(new CTZ_Material_Labels { ID_LabelType = labelId });
                                }
                            }
                            var strapsToAdd = new List<CTZ_Material_StrapTypes>();
                            if (material.SelectedStrapTypeIds != null && material.SelectedStrapTypeIds.Any())
                            {
                                foreach (var strapId in material.SelectedStrapTypeIds)
                                {
                                    strapsToAdd.Add(new CTZ_Material_StrapTypes { ID_StrapType = strapId });
                                }
                            }
                            var additionalsToAdd = new List<CTZ_Material_Additionals>();
                            if (material.SelectedAdditionalIds != null && material.SelectedAdditionalIds.Any())
                            {
                                foreach (var additionalID in material.SelectedAdditionalIds)
                                {
                                    additionalsToAdd.Add(new CTZ_Material_Additionals { ID_Additional = additionalID });
                                }
                            }

                            // Asignamos la colección al objeto material. EF entenderá la relación.
                            material.CTZ_Material_RackTypes = rackTypesToAdd;
                            material.CTZ_Material_Labels = labelsToAdd;
                            material.CTZ_Material_StrapTypes = strapsToAdd;
                            material.CTZ_Material_Additionals = additionalsToAdd;


                        }
                    }

                    // --------------------------------------------------------------------------
                    // ELIMINAR LOS ARCHIVOS HUÉRFANOS DE LA TABLA CTZ_Files
                    // --------------------------------------------------------------------------
                    foreach (var fileId in fileIdsToDelete.Distinct())
                    {
                        bool isStillReferenced = db.CTZ_Project_Materials.Any(m =>
                            m.ID_File_CAD_Drawing == fileId ||
                            m.ID_File_Packaging == fileId ||
                            m.ID_File_TechnicalSheet == fileId ||
                            m.ID_File_Additional == fileId ||
                            m.ID_File_ArrivalAdditional == fileId ||
                            m.ID_File_CoilDataAdditional == fileId ||
                            m.ID_File_SlitterDataAdditional == fileId ||
                            m.ID_File_VolumeAdditional == fileId ||
                            m.ID_File_OutboundFreightAdditional == fileId ||
                            m.ID_File_DeliveryPackagingAdditional == fileId
                        );

                        if (!isStillReferenced)
                        {
                            var oldFile = db.CTZ_Files.Find(fileId);
                            if (oldFile != null)
                            {
                                db.CTZ_Files.Remove(oldFile);
                            }
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

                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    // Si algo falla, revertir todos los cambios
                    transaction.Rollback();
                    // Guardar o mostrar el error para depuración
                    TempData["ErrorMessage"] = "An error occurred while saving: " + ex.Message;
                    // Es importante redirigir o devolver una vista de error
                    return RedirectToAction("EditClientPartInformation", new { id = project.ID_Project });
                }
            }


            TempData["SuccessMessage"] = "Material saved successfully!";
            return RedirectToAction("EditClientPartInformation", new { id = project.ID_Project });


        }

        // En CTZ_ProjectsController.cs

        /// <summary>
        /// Devuelve la lista de vehículos (IHS) filtrada por un país.
        /// </summary>

        [HttpGet]
        public JsonResult GetIHSByCountry(string country)
        {
            var tempIHSList = db.CTZ_Temp_IHS.Where(i => i.Country == country).ToList();

            // --- INICIO DE LA CORRECCIÓN ---

            // Paso 1: Extraer los IDs de la lista en una nueva variable.
            // Esto crea una lista de tipo simple (List<int>) que Entity Framework sí entiende.
            var ihsIds = tempIHSList.Select(i => i.ID_IHS).ToList();

            // Paso 2: Usar esta nueva lista de IDs en la consulta.
            var productionLookup = db.CTZ_Temp_IHS_Production
                .Where(p => ihsIds.Contains(p.ID_IHS)) // Ahora la consulta es válida.
                .ToList()
                .GroupBy(p => p.ID_IHS)
                .ToDictionary(g => g.Key, g => g.Select(p => new { p.Production_Year, p.Production_Month, p.Production_Amount }).ToList());

            // --- FIN DE LA CORRECCIÓN ---

            var vehicles = tempIHSList.Select(x => new
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
            }).OrderBy(v => v.Value).ToList();

            return Json(vehicles, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Devuelve el país para un código IHS específico.
        /// </summary>
        // En /Controllers/CTZ_ProjectsController.cs

        [HttpGet]
        public JsonResult GetCountryForIHS(string ihsCode)
        {
            // 1. Verificamos que el código recibido no sea nulo o vacío.
            if (string.IsNullOrEmpty(ihsCode))
            {
                return Json(new { success = false, message = "ihsCode cannot be null." }, JsonRequestBehavior.AllowGet);
            }

            // 2. Extraemos el Mnemonic de la cadena completa 'ihsCode'.
            //    Asumimos que el formato es "MNEMONIC_DESCRIPCION..."
            string mnemonic = ihsCode; // Por defecto, usamos el código completo si no hay guion bajo.
            int underscoreIndex = ihsCode.IndexOf('_');
            if (underscoreIndex > 0)
            {
                // Si encontramos un '_', tomamos solo la parte anterior.
                mnemonic = ihsCode.Substring(0, underscoreIndex);
            }

            // 3. Usamos el mnemonic extraído para una búsqueda eficiente en la BD.
            //    (Mejora de rendimiento: se elimina .ToList() para que el filtro se ejecute en SQL Server).
            var country = db.CTZ_Temp_IHS
                            .Where(i => i.Mnemonic_Vehicle_plant == mnemonic)
                            .Select(i => i.Country)
                            .FirstOrDefault();

            if (country != null)
            {
                // Se encontró el país para el mnemonic.
                return Json(new { success = true, country = country }, JsonRequestBehavior.AllowGet);
            }

            // No se encontró un país que coincida con el mnemonic.
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetTheoreticalLine(int plantId, int? materialTypeId, float? thickness, float? width, float? tensile, float? pitch)
        {
            Debug.WriteLine("--- Starting GetTheoreticalLine ---");
            Debug.WriteLine($"Parameters: plantId={plantId}, materialTypeId={materialTypeId}, thickness={thickness}, width={width}, tensile={tensile}, pitch={pitch}");

            if (!materialTypeId.HasValue)
            {
                return Json(new { success = false, message = "Material Type is required." }, JsonRequestBehavior.AllowGet);
            }

            var allRules = await db.CTZ_Theoretical_Line_Criteria
                .Where(r => r.ID_Plant == plantId &&
                            r.ID_Material_Type == materialTypeId &&
                            r.Is_Active)
                .Include(r => r.CTZ_Production_Lines)
                .ToListAsync();

            Debug.WriteLine($"Found {allRules.Count} potential rules for plant/material.");

            var scoredRules = new List<Tuple<int, CTZ_Theoretical_Line_Criteria>>();

            foreach (var rule in allRules)
            {
                Debug.WriteLine($"\n--- Evaluating Rule ID: {rule.ID_Rule}, Priority: {rule.Priority} ---");
                int score = 0;

                // Evalúa Thickness
                if (thickness.HasValue && rule.Thickness_Min.HasValue && rule.Thickness_Max.HasValue)
                {
                    if (thickness.Value >= rule.Thickness_Min.Value && thickness.Value <= rule.Thickness_Max.Value)
                    {
                        score++;
                        Debug.WriteLine($"  [+] MATCH on Thickness. Score is now: {score}");
                    }
                    else
                    {
                        Debug.WriteLine($"  [-] NO MATCH on Thickness. User value {thickness.Value} is outside rule range [{rule.Thickness_Min.Value}-{rule.Thickness_Max.Value}].");
                    }
                }

                // Evalúa Width
                if (width.HasValue && rule.Width_Min.HasValue && rule.Width_Max.HasValue)
                {
                    if (width.Value >= rule.Width_Min.Value && width.Value <= rule.Width_Max.Value)
                    {
                        score++;
                        Debug.WriteLine($"  [+] MATCH on Width. Score is now: {score}");
                    }
                    else
                    {
                        Debug.WriteLine($"  [-] NO MATCH on Width. User value {width.Value} is outside rule range [{rule.Width_Min.Value}-{rule.Width_Max.Value}].");
                    }
                }

                // Evalúa Pitch
                if (pitch.HasValue && rule.Pitch_Min.HasValue && rule.Pitch_Max.HasValue)
                {
                    if (pitch.Value >= rule.Pitch_Min.Value && pitch.Value <= rule.Pitch_Max.Value)
                    {
                        score++;
                        Debug.WriteLine($"  [+] MATCH on Pitch. Score is now: {score}");
                    }
                    else
                    {
                        Debug.WriteLine($"  [-] NO MATCH on Pitch. User value {pitch.Value} is outside rule range [{rule.Pitch_Min.Value}-{rule.Pitch_Max.Value}].");
                    }
                }

                // Evalúa Tensile
                if (tensile.HasValue && rule.Tensile_Min.HasValue && rule.Tensile_Max.HasValue)
                {
                    if (tensile.Value >= rule.Tensile_Min.Value && tensile.Value <= rule.Tensile_Max.Value)
                    {
                        score++;
                        Debug.WriteLine($"  [+] MATCH on Tensile. Score is now: {score}");
                    }
                    else
                    {
                        Debug.WriteLine($"  [-] NO MATCH on Tensile. User value {tensile.Value} is outside rule range [{rule.Tensile_Min.Value}-{rule.Tensile_Max.Value}].");
                    }
                }

                Debug.WriteLine($"  ==> Final score for Rule {rule.ID_Rule}: {score}");
                scoredRules.Add(new Tuple<int, CTZ_Theoretical_Line_Criteria>(score, rule));
            }

            Debug.WriteLine("\n--- Sorting Rules... ---");
            var sortedRules = scoredRules
                .OrderByDescending(t => t.Item1)
                .ThenBy(t => t.Item2.Priority)
                .ToList();

            foreach (var scoredRule in sortedRules)
            {
                Debug.WriteLine($"  - Candidate: Rule ID {scoredRule.Item2.ID_Rule} (Score: {scoredRule.Item1}, Priority: {scoredRule.Item2.Priority})");
            }

            var bestRule = sortedRules.Select(t => t.Item2).FirstOrDefault();

            if (bestRule != null)
            {
                Debug.WriteLine($"\n==> BEST RULE SELECTED: ID {bestRule.ID_Rule} (Line: {bestRule.CTZ_Production_Lines.Line_Name})");
                return Json(new
                {
                    success = true,
                    lineId = bestRule.Resulting_Line_ID,
                    lineName = bestRule.CTZ_Production_Lines.Description
                }, JsonRequestBehavior.AllowGet);
            }

            Debug.WriteLine("\n==> No suitable rule was found.");
            return Json(new { success = false, message = "No matching rule found." }, JsonRequestBehavior.AllowGet);
        }


        // GET: CTZ_Projects/Create
        public ActionResult ProjectStatus(
       string projectStatusFilter = "All",   // ← NUEVO
     string assignmentStatusFilter = "All",
     string quoteID = "",
      DateTime? from = null,
      DateTime? to = null)
        {
            // 1) Filtros básicos
            var query = db.CTZ_Projects
                .Include(p => p.CTZ_Project_Status)
                .Include(p => p.CTZ_Project_Assignment)
                .OrderByDescending(p => p.ID_Project)
                .AsQueryable();

            if (projectStatusFilter != "All")
                query = query.Where(p => p.CTZ_Project_Status.Description == projectStatusFilter);

            if (from.HasValue)
                query = query.Where(p => p.Creted_Date >= from.Value);

            if (to.HasValue)
                query = query.Where(p => p.Creted_Date <= to.Value);

            //trae a memoria los registros antes de aplicar el filtro de la propiedad notmapped
            var inMemory = query.ToList();

            //trae a memoria cuando los filtros ya son notmapped
            if (assignmentStatusFilter != "All")
            {
                inMemory = inMemory
                    .Where(p => p.GeneralAssignmentStatusDisplay == assignmentStatusFilter)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(quoteID))
            {
                var q = quoteID.ToUpperInvariant();
                inMemory = inMemory
                    .Where(p =>
                        !string.IsNullOrEmpty(p.ConcatQuoteID) &&
                        p.ConcatQuoteID.ToUpperInvariant().Contains(q)
                    )
                    .ToList();
            }


            // Festivos activos
            var holidays = db.CTZ_Holidays
                .Where(h => h.Active)
                .Select(h => h.HolidayDate)
                .ToList();

            // 2) Proyección a nuestro ViewModel
            var list = inMemory.Select(p =>
            {
                // Función local para procesar una asignación de depto
                string calcRaw(int dept)
                {
                    var a = p.CTZ_Project_Assignment
                            .Where(x => x.ID_Department == dept)
                            .OrderByDescending(x => x.Assignment_Date)
                            .FirstOrDefault();
                    return a?.CTZ_Assignment_Status.Status_Name;
                }
                string calcElapsed(int dept)
                {
                    var a = p.CTZ_Project_Assignment
                            .Where(x => x.ID_Department == dept)
                            .OrderByDescending(x => x.Assignment_Date)
                            .FirstOrDefault();
                    if (a == null) return "--";


                    var raw = a.CTZ_Assignment_Status.Status_Name.ToLowerInvariant();
                    bool isFinalStatus = raw == "approved" || raw == "rejected" || raw == "on_hold";

                    var start = a.Assignment_Date;
                    var end = isFinalStatus
                              ? a.Last_Status_Change.GetValueOrDefault(start)
                              : DateTime.Now;
                    var span = BusinessTimeCalculator
                               .GetBusinessTime(start, end, holidays);
                    return $"{(int)span.TotalHours}h {span.Minutes}m";
                }

                return new ProjectStatusItem
                {
                    ID_Project = p.ID_Project,
                    QuoteID = p.ConcatQuoteID,
                    ProjectStatus = p.CTZ_Project_Status.ConcatStatus,
                    AssignmentStatus = p.GeneralAssignmentStatusDisplay,

                    SalesRaw = calcRaw((int)DepartmentEnum.Sales),
                    SalesElapsed = calcElapsed((int)DepartmentEnum.Sales),

                    EngineeringRaw = calcRaw((int)DepartmentEnum.Engineering),
                    EngineeringElapsed = calcElapsed((int)DepartmentEnum.Engineering),

                    ForeignTradeRaw = calcRaw((int)DepartmentEnum.ForeignTrade),
                    ForeignTradeElapsed = calcElapsed((int)DepartmentEnum.ForeignTrade),

                    DispositionRaw = calcRaw((int)DepartmentEnum.Disposition),
                    DispositionElapsed = calcElapsed((int)DepartmentEnum.Disposition),

                    DataManagementRaw = calcRaw((int)DepartmentEnum.DataManagement),
                    DataManagementElapsed = calcElapsed((int)DepartmentEnum.DataManagement)
                };
            }).ToList();


            // 3) Serializo a JSON para pasarlo a la vista
            ViewBag.StatusDataJson = JsonConvert.SerializeObject(list);

            // 4) Preparo listas para filtros (por ejemplo Status)
            var statusList = db.CTZ_Project_Status.ToList()
                .Select(s => s.ConcatStatus)
                .Distinct()
                .ToList();
            statusList.Insert(0, "All");
            ViewBag.ProjectStatusList = new SelectList(statusList);

            var assignmentStatusList = new[] {
                "All",
                "Created",
                "In Process",
                "On Hold",
                "On Review",
                "Rejected",
                "Finalized"
            };
            ViewBag.AssignmentStatusList = new SelectList(assignmentStatusList, assignmentStatusFilter);


            ViewBag.FromDate = from?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.ToDate = to?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.QuoteID = quoteID;
            ViewBag.SelectedStatus = projectStatusFilter;

            return View();
        }

        // POST: CTZ_Projects/ProcessProjectActivity
        [HttpPost]
        public ActionResult ProcessProjectActivity(
                int projectId,
                DepartmentEnum department,
                int newStatus,        // AssignmentStatusEnum como entero
                string comment,       // Comentario opcional
                int[] activities,     // Array de IDs de actividad seleccionadas
                int? reassignToDept,   // ← opcional
                byte? rejectionReasonId,
                string rejectionReasonOther,
                byte? actionType
        )
        {
            try
            {

                #region validaciones de formulario
                // ——— VALIDACIONES GENERALES ———

                // 1) Comentario no puede exceder 200 caracteres
                if (!string.IsNullOrEmpty(comment) && comment.Length > 200)
                    return Json(new { success = false, message = "Comment cannot exceed 200 characters." });

                // 2) Debe haber materiales definidos antes de cualquier acción
                if (!db.CTZ_Project_Materials.Any(m => m.ID_Project == projectId))
                    return Json(new { success = false, message = "Cannot proceed: no materials defined for this project." });

                // 3) Regla de negocio específica para Ingeniería
                if (department == DepartmentEnum.Engineering
                    && (newStatus == (int)AssignmentStatusEnum.APPROVED
                        || newStatus == (int)AssignmentStatusEnum.ON_REVIEWED))
                {
                    bool missingRealBL = db.CTZ_Project_Materials
                        .Any(m => m.ID_Project == projectId && m.ID_Real_Blanking_Line == null);
                    if (missingRealBL)
                        return Json(new
                        {
                            success = false,
                            message = "Cannot complete Engineering: every part needs a real blanking line."
                        });
                }

                // 3) Regla de negocio específica para Ingeniería → NO aprobar si hay materiales rechazados
                if (department == DepartmentEnum.Engineering
                    && (newStatus == (int)AssignmentStatusEnum.APPROVED
                        || newStatus == (int)AssignmentStatusEnum.ON_REVIEWED))
                {
                    // buscamos en la BD si algún material de este proyecto tiene DM_status = "Rejected"
                    bool anyRejectedMaterial = db.CTZ_Project_Materials
                        .Any(m => m.ID_Project == projectId && m.DM_status == "Rejected");
                    if (anyRejectedMaterial)
                    {
                        return Json(new
                        {
                            success = false,
                            // mensaje en inglés que explique claramente el porqué
                            message = "Cannot complete Engineering approval: some part numbers have exceeded the allowed capacity percentage and will be rejected by Data Management. Please adjust those part numbers before proceeding."
                        });
                    }
                }


                // ——— VALIDACIONES PARA RECHAZOS ———
                if (newStatus == (int)AssignmentStatusEnum.REJECTED)
                {
                    // Helper local para evitar repetir el check de “Other”
                    bool isOther = rejectionReasonId.GetValueOrDefault() == 0;

                    // a) Debe venir un motivo seleccionado
                    if (!rejectionReasonId.HasValue)
                        return Json(new { success = false, message = "Please select a rejection reason." });

                    // b) Si es “Other”, exige texto no vacío y ≤200 caracteres
                    if (isOther)
                    {
                        if (string.IsNullOrWhiteSpace(rejectionReasonOther))
                            return Json(new { success = false, message = "Please specify the rejection reason (max 200 chars)." });
                        if (rejectionReasonOther.Length > 200)
                            return Json(new { success = false, message = "Rejection reason cannot exceed 200 characters." });
                    }
                    else
                    {
                        // Si no es “Other”, limpiamos cualquier dato sobrante
                        rejectionReasonOther = null;
                        reassignToDept = null;
                        actionType = null;
                    }

                    // c) Validación adicional para DataManagement + “Other”
                    if (department == DepartmentEnum.DataManagement && isOther)
                    {
                        // c1) Siempre debe venir actionType
                        if (!actionType.HasValue)
                            return Json(new { success = false, message = "Please select a behavior for this ‘Other’ reason." });

                        var act = (ActionTypeEnum)actionType.Value;
                        // c2) Si no es FinalizeAll, exige también reassignToDept
                        if (act != ActionTypeEnum.FinalizeAll && !reassignToDept.HasValue)
                            return Json(new { success = false, message = "Please select a department to reassign for this ‘Other’ reason." });
                    }

                    // d) Comentario (campo “comment”) obligatorio y ≤200 caracteres
                    if (string.IsNullOrWhiteSpace(comment))
                        return Json(new { success = false, message = "A non‐empty comment is required when rejecting." });
                    // (El check de longitud ya lo hicimos en la sección “VALIDACIONES GENERALES”)
                }

                #endregion

                var me = obtieneEmpleadoLogeado();
                var project = db.CTZ_Projects.Find(projectId);
                if (project == null)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Project not found");

                //variables globales del metodo
                var now = DateTime.Now;
                var statusText = ((AssignmentStatusEnum)newStatus).ToString().Replace("_", " ");


                Action<DepartmentEnum, string, bool, bool, string, string> assignOrNotify = (
                      DepartmentEnum dept,
                      string deptName,
                      bool shouldAssign,
                      bool shouldNotify,
                      string subject,
                      string body
                  ) =>
                {
                    if (shouldAssign)
                    {
                        AssignmentService.AssignProjectToDepartment(projectId, dept, project.ID_Plant, now);
                    }

                    // si hay cuerpo, notificar
                    if (shouldNotify && !string.IsNullOrWhiteSpace(body))
                    {
                        var emails = (
                            from ed in db.CTZ_Employee_Departments
                            join ep in db.CTZ_Employee_Plants on ed.ID_Employee equals ep.ID_Employee
                            join emp in db.empleados on ed.ID_Employee equals emp.id
                            where ed.ID_Department == (int)dept
                               && ep.ID_Plant == project.ID_Plant
                               && !string.IsNullOrEmpty(emp.correo)
                            select emp.correo
                        ).Distinct().ToList();

                        if (emails.Any())
                        {
                            var mail = new EnvioCorreoElectronico();
                            mail.SendEmailAsync(emails, subject, body);
                        }
                    }
                };


                // 1) Intentar obtener asignación activa de este depto
                var assignment = db.CTZ_Project_Assignment
                    .FirstOrDefault(a =>
                        a.ID_Project == projectId &&
                        a.ID_Department == (int)department &&
                        a.Completition_Date == null);

                // 2) Si es Sales y no hay asignación → flujo inicial
                if (department == DepartmentEnum.Sales && assignment == null)
                {
                    // a) Versionado + histórico
                    bool has1_0 = db.CTZ_Projects_Versions
                        .Any(v => v.ID_Project == projectId && v.Version_Number == "1.0");
                    if (!has1_0)
                    {
                        var version = VersionService.CreateNewVersion(projectId, me.id, project.ID_Status, "Initial version", now);
                        HistoryHelper.CopyMaterialsToHistory(projectId, version.ID_Version);
                    }


                    // c) Asignar + mail a todos los deptos necesarios
                    var sentDepts = new List<string>();

                    void SendToDept(DepartmentEnum deptEnum, string deptName)
                    {
                        var body = getBodyNewQuote(project, deptName, now);
                        assignOrNotify(deptEnum, deptName, true, true, $"Quote Assigned to {deptName}", body);
                        sentDepts.Add(deptName);
                    }

                    SendToDept(DepartmentEnum.Engineering, "Engineering");

                    if (project.ImportRequired)
                        SendToDept(DepartmentEnum.ForeignTrade, "Foreign Trade");

                    if (project.ID_Material_Owner == 1)
                        SendToDept(DepartmentEnum.Disposition, "Disposition");

                    // d) Responder con mensaje específico
                    var listText = string.Join(", ", sentDepts);
                    return Json(new
                    {
                        success = true,
                        message = $"Quote sent from Sales to {listText}."
                    });
                }


                // 4) Actualizar datos de la asignacion
                if (assignment != null)
                {
                    assignment.ID_Assignment_Status = newStatus;
                    assignment.Comments = comment?.Trim();

                    // 1) Siempre actualizamos Last_Status_Change
                    assignment.Last_Status_Change = now;
                    assignment.ID_Employee = me.id;

                    // 2) Solo si es un estatus terminal rellenamos Completion_Date
                    if (newStatus == (int)AssignmentStatusEnum.APPROVED
                     || newStatus == (int)AssignmentStatusEnum.REJECTED)
                    {
                        assignment.Completition_Date = now;
                    }

                    db.SaveChanges();

                    // 6) Registrar actividades completadas / desmarcadas
                    //   - activities[] trae sólo las marcadas actualmente
                    var existing = db.CTZ_Assignment_Activity
                        .Where(x => x.ID_Assignment == assignment.ID_Assignment)
                        .ToList();

                    // a) Agregar nuevas
                    foreach (var actId in activities ?? Enumerable.Empty<int>())
                    {
                        if (!existing.Any(x => x.ID_Activity == actId))
                        {
                            db.CTZ_Assignment_Activity.Add(new CTZ_Assignment_Activity
                            {
                                ID_Assignment = assignment.ID_Assignment,
                                ID_Activity = actId,
                                IsComplete = true,
                                // CompletionDate = now    // si quieres marcar fecha
                            });
                        }
                    }

                    // b) Quitar (o marcar false) las que el usuario desmarcó
                    foreach (var ea in existing)
                    {
                        if (activities == null || !activities.Contains(ea.ID_Activity))
                        {
                            // Opción 1: eliminarlas
                            db.CTZ_Assignment_Activity.Remove(ea);
                            // Opción 2: sólo poner IsComplete = false
                            // ea.IsComplete = false;
                        }
                    }

                    db.SaveChanges();

                }

                //valida que si es rechazo no se envie el comentario de rechazo vacio
                if (newStatus == (int)AssignmentStatusEnum.REJECTED)
                {


                    // 2) Marca la asignación
                    // ——— MARCAR LA ASIGNACIÓN ———
                    assignment.WasRejected = true;
                    assignment.ID_RejectionReason = rejectionReasonId.Value == 0 ? (int?)null : rejectionReasonId;
                    assignment.RejectionReasonOther = rejectionReasonId.Value == 0
                                                       ? rejectionReasonOther.Trim()
                                                       : null;
                    db.SaveChanges();

                    // ——— NORMALIZAR actionType y reassignToDept ———
                    // 1) Carga del catálogo si existe
                    CTZ_RejectionReason catalogReason = null;
                    if (rejectionReasonId.Value != 0)
                        catalogReason = db.CTZ_RejectionReason.Find(rejectionReasonId.Value);

                    // 2) ActionType efectivo
                    var effectiveAction = actionType.HasValue
                        ? (ActionTypeEnum)actionType.Value
                        : catalogReason != null
                            ? (ActionTypeEnum)catalogReason.ActionType
                            : ActionTypeEnum.KeepActive;

                    // 3) ReassignToDept efectivo
                    var effectiveReassign = reassignToDept.HasValue
                        ? (DepartmentEnum)reassignToDept.Value
                        : catalogReason?.ReassignDepartmentId.HasValue == true
                            ? (DepartmentEnum)catalogReason.ReassignDepartmentId.Value
                            : DepartmentEnum.Sales;

                    switch (effectiveAction)
                    {
                        case ActionTypeEnum.HoldOthers:
                            // 1) Hold a todos los demás
                            var others = db.CTZ_Project_Assignment
                                .Where(a => a.ID_Project == projectId
                                         && a.Completition_Date == null
                                         && a.ID_Assignment != assignment.ID_Assignment)
                                .ToList();

                            foreach (var o in others)
                            {
                                o.ID_Assignment_Status = (int)AssignmentStatusEnum.ON_HOLD;
                                o.Last_Status_Change = now;
                            }
                            db.SaveChanges();

                            // 2) Notificar a cada depto en On Hold
                            var onHoldDeptIds = others
                                .Select(o => o.ID_Department)
                                .Distinct();

                            foreach (var deptId in onHoldDeptIds)
                            {
                                var deptEnum = (DepartmentEnum)deptId;
                                var deptName = deptEnum.ToString();
                                var subject = $"Quote {project.ConcatQuoteID} Placed On Hold";
                                var body = GetBodyOnHoldNotification(project, deptName, now);
                                assignOrNotify(deptEnum, deptName, false, true, subject, body);

                            }

                            // 3) Reasignar al destino normal
                            AssignmentService.AssignProjectToDepartment(
                                projectId,
                                effectiveReassign,
                                project.ID_Plant,
                                now
                            );
                            NotifyRejectedApproverOrTeam(project, department, effectiveReassign, now);

                            break;


                        case ActionTypeEnum.KeepActive:
                            // 1) No tocamos a los demás
                            // 2) Reasignar actual a reassignToDept o Sales
                            AssignmentService.AssignProjectToDepartment(
                                   projectId,
                                   effectiveReassign,
                                   project.ID_Plant,
                                   now
                               );
                            NotifyRejectedApproverOrTeam(project, department, effectiveReassign, now);


                            break;



                        case ActionTypeEnum.FinalizeAll:
                            // 1) Poner On Hold a todos los demás sin reasignar después
                            var allOthers = db.CTZ_Project_Assignment
                                .Where(a => a.ID_Project == projectId
                                         && a.Completition_Date == null
                                         && a.ID_Assignment != assignment.ID_Assignment);
                            foreach (var o in allOthers)
                            {
                                o.ID_Assignment_Status = (int)AssignmentStatusEnum.ON_HOLD;
                                o.Last_Status_Change = now;
                            }
                            db.SaveChanges();

                            // 2) (Opcional) marcar el proyecto como cerrado aquí…
                            // project.ID_Status = /* ID de "Closed" */;
                            // db.SaveChanges();

                            // 3) Notificar al solicitante que la solicitud ya no puede procesarse
                            var creator = db.empleados.Find(project.ID_Created_By);
                            if (!string.IsNullOrEmpty(creator?.correo))
                            {
                                var subject = $"Quote {project.ConcatQuoteID} Cannot Be Processed";
                                var body = GetBodyFinalizeAllNotification(project, now);
                                var mail = new EnvioCorreoElectronico();
                                mail.SendEmailAsync(new List<string> { creator.correo }, subject, body);
                            }
                            break;
                    }

                    // 4) Notifica y devuelve mensaje
                    return Json(new
                    {
                        success = true,
                        message = $"Assignment rejected and processed by {department.ToString()}."
                    });
                }

                // 8) Flujos posteriores según departamento
                if (newStatus != (int)AssignmentStatusEnum.REJECTED)
                    switch (department)
                    {
                        case DepartmentEnum.Engineering:
                        case DepartmentEnum.ForeignTrade:
                        case DepartmentEnum.Disposition:

                            //si tiene estos estatus, puede pasar a DM
                            var doneStatuses = new[]
                            {
                            (int)AssignmentStatusEnum.ON_REVIEWED,
                            (int)AssignmentStatusEnum.APPROVED
                        };

                            // Helper para saber si un depto está “done” usando solo su última asignación
                            bool IsDeptDone(int deptId)
                            {
                                var last = db.CTZ_Project_Assignment
                                    .Where(a => a.ID_Project == projectId && a.ID_Department == deptId)
                                    .OrderByDescending(a => a.Assignment_Date)    // o por ID_Assignment
                                    .FirstOrDefault();

                                return last != null && doneStatuses.Contains(last.ID_Assignment_Status);
                            }

                            // Entonces:
                            bool engDone = IsDeptDone((int)DepartmentEnum.Engineering);
                            bool ftDone = !project.ImportRequired || IsDeptDone((int)DepartmentEnum.ForeignTrade);
                            bool dpDone = project.ID_Material_Owner != 1 || IsDeptDone((int)DepartmentEnum.Disposition);


                            if (engDone && ftDone && dpDone)
                            {
                                //envia correo y asigna a DM
                                string deptName = "Data Management";
                                var body = getBodyNewQuote(project, deptName, now);
                                assignOrNotify(DepartmentEnum.DataManagement, deptName, true, true, $"Quote Assigned to {deptName}", body);

                                return Json(new
                                {
                                    success = true,
                                    message = $"Assignment changed to “{statusText}” for {department}. All prior departments completed; forwarded to Data Management."
                                });
                            }

                            return Json(new
                            {
                                success = true,
                                message = $"Assignment changed to “{statusText}” for {department}. Waiting on other departments."

                            });

                        case DepartmentEnum.DataManagement:


                            if (newStatus == (int)AssignmentStatusEnum.APPROVED)
                            {
                                // c) Notificar al solicitante
                                var creator = db.empleados.Find(project.ID_Created_By);
                                if (!string.IsNullOrEmpty(creator?.correo))
                                {
                                    var mail = new EnvioCorreoElectronico();
                                    var bodyHtml = GetBodyQuoteFinalized(project, now);
                                    mail.SendEmailAsync(
                                        new List<string> { creator.correo },
                                        $"Quote {project.ConcatQuoteID} Finalized",
                                        bodyHtml
                                    );
                                }
                                return Json(new { success = true, message = "Quote finalized and requester notified." });
                            }

                            return Json(new
                            {
                                success = true,
                                message = $"Assignment changed to “{statusText}” for {department}."
                            });

                        case DepartmentEnum.Sales:
                            if (assignment != null && newStatus == (int)AssignmentStatusEnum.APPROVED)
                            {
                                // Ventana de ±1s alrededor de la hora de asignación actual
                                var lower = assignment.Assignment_Date.AddSeconds(-1);
                                var upper = assignment.Assignment_Date.AddSeconds(1);

                                // 1) Encuentra el depto que rechazó justo antes
                                var prevReject = db.CTZ_Project_Assignment
                                    .FirstOrDefault(a =>
                                        a.ID_Project == projectId &&
                                        a.WasRejected &&
                                        a.Completition_Date >= lower &&
                                        a.Completition_Date <= upper
                                    );
                                if (prevReject == null)
                                    break; // nada que hacer

                                // 2) Cierra todas las asignaciones OnHold en esa ventana
                                var onHoldList = db.CTZ_Project_Assignment
                                    .Where(a =>
                                        a.ID_Project == projectId &&
                                        a.ID_Assignment_Status == (int)AssignmentStatusEnum.ON_HOLD &&
                                        a.Last_Status_Change >= lower &&
                                        a.Last_Status_Change <= upper
                                    )
                                    .ToList();
                                foreach (var o in onHoldList)
                                {
                                    o.Completition_Date = now;
                                    o.Last_Status_Change = now;
                                }
                                db.SaveChanges();

                                // 3) Prepara la lista de departamentos a re-asignar:
                                //    - el que rechazó (prevReject.ID_Department)
                                //    - todos los que estaban OnHold
                                var deptsToReassign = new HashSet<DepartmentEnum>();
                                deptsToReassign.Add((DepartmentEnum)prevReject.ID_Department);
                                foreach (var o in onHoldList)
                                    deptsToReassign.Add((DepartmentEnum)o.ID_Department);



                                // 5) Para cada depto en la lista, crear + notificar
                                foreach (var dept in deptsToReassign)
                                {
                                    var toDeptName = dept.ToString();
                                    var fromDeptName = department.ToString();
                                    var assignDate = now;

                                    // 1) Tema del correo
                                    var subject = $"Quote {project.ConcatQuoteID} Reassigned to {toDeptName}";

                                    // 2) Cuerpo generado por tu helper
                                    var body = GetBodyReassignment(
                                        quote: project,
                                        fromDepartment: fromDeptName,
                                        toDepartment: toDeptName,
                                        assignmentDate: assignDate
                                    );

                                    // 3) Crea la nueva asignación Y envía notificación
                                    //    El tercer parámetro 'true' indica que también quieres crear la asignación
                                    assignOrNotify(
                                         dept,           // DepartmentEnum
                                         toDeptName,     // nombre legible
                                         true, // crear AssignProjectToDepartment
                                         true, // enviar e-mail
                                         subject,
                                         body
                                    );

                                }

                                return Json(new
                                {
                                    success = true,
                                    message = $"Quote resubmitted to {string.Join(", ", deptsToReassign)}."
                                });

                            }
                            else if (newStatus == (int)AssignmentStatusEnum.IN_PROGRESS)
                            {
                                // solo cambio de estado sin reasignar
                                return Json(new
                                {
                                    success = true,
                                    message = $"Assignment changed to “IN PROGRESS” for Sales."
                                });
                            }
                            break;

                        default:
                            return Json(new { success = false, message = "Invalid department." });
                    }
                return Json(new
                {
                    success = true,
                    message = $"Done."

                });
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET: Show all versions for a given project
        public ActionResult Versions(int id)
        {
            // id = ID_Project
            // traemos todas las versiones de este proyecto, ordenadas por fecha
            var versions = db.CTZ_Projects_Versions
                             .Where(v => v.ID_Project == id)
                             .OrderByDescending(v => v.Creation_Date)
                             .ToList();

            // pasamos la lista de versiones a la vista
            return View(versions);
        }
        public ActionResult AssignmentHistory(int id)
        {
            // carga las asignaciones del proyecto y mapea al ViewModel
            var history = db.CTZ_Project_Assignment
                .Where(a => a.ID_Project == id)
                .Include(a => a.CTZ_Departments)
                .Include(a => a.CTZ_Assignment_Status)
                .Include(a => a.empleados)              // quien cierra
                .Include(a => a.CTZ_RejectionReason)    // razón de rechazo
                .Select(a => new AssignmentHistoryViewModel
                {
                    ID_Assignment = a.ID_Assignment,
                    DepartmentName = a.CTZ_Departments.Name,
                    AssignmentDate = a.Assignment_Date,
                    CompletionDate = a.Completition_Date,
                    StatusName = a.CTZ_Assignment_Status.Status_Name,
                    ClosedBy = a.empleados != null
                                        ? a.empleados.nombre + " " + a.empleados.apellido1
                                        : "—",
                    WasRejected = a.WasRejected,
                    RejectionReason = a.WasRejected
                                        ? (a.CTZ_RejectionReason.Name ?? a.RejectionReasonOther)
                                        : null,
                    Comments = a.Comments
                })
                .OrderBy(a => a.AssignmentDate)
                .ToList();

            return View(history);
        }

        /// <summary>
        /// Notifica tras un rechazo y reasignación:
        /// - Si se reasigna a Sales, notifica siempre al solicitante (creator).
        /// - En otros casos, notifica al último aprobador de ese depto, o al equipo si no se encuentra.
        /// </summary>
        private void NotifyRejectedApproverOrTeam(CTZ_Projects project, DepartmentEnum rejectingDept, DepartmentEnum targetDept, DateTime now)
        {
            List<string> recipients = new List<string>();

            // 1) Si reasignamos a Sales, notificar siempre al solicitante
            if (targetDept == DepartmentEnum.Sales)
            {
                var creator = db.empleados.Find(project.ID_Created_By);
                if (!string.IsNullOrEmpty(creator?.correo))
                    recipients.Add(creator.correo);
            }
            else
            {
                // 2) Buscar última asignación aprobada en este depto
                var lastApproved = db.CTZ_Project_Assignment
                    .Where(a =>
                        a.ID_Project == project.ID_Project &&
                        a.ID_Department == (int)targetDept &&
                        (a.ID_Assignment_Status == (int)AssignmentStatusEnum.APPROVED || a.ID_Assignment_Status == (int)AssignmentStatusEnum.ON_REVIEWED))
                    .OrderByDescending(a => a.Completition_Date ?? a.Last_Status_Change)
                    .FirstOrDefault();

                if (lastApproved?.ID_Employee != null)
                {
                    var approver = db.empleados.Find(lastApproved.ID_Employee.Value);
                    if (!string.IsNullOrEmpty(approver?.correo))
                        recipients.Add(approver.correo);
                }

                // 3) Si no hallamos un aprobador individual, notificar a todo el equipo de targetDept
                if (!recipients.Any())
                {
                    recipients = (
                        from ed in db.CTZ_Employee_Departments
                        join ep in db.CTZ_Employee_Plants on ed.ID_Employee equals ep.ID_Employee
                        join emp in db.empleados on ed.ID_Employee equals emp.id
                        where ed.ID_Department == (int)targetDept
                           && ep.ID_Plant == project.ID_Plant
                           && !string.IsNullOrEmpty(emp.correo)
                        select emp.correo
                    )
                    .Distinct()
                    .ToList();
                }
            }

            // 4) Enviar correo si hay destinatarios
            if (recipients.Any())
            {
                var subject = $"Quote {project.ConcatQuoteID} Reassigned to {targetDept}";
                // Obtenemos el texto de motivo desde la asignación rechazada
                var rejectedAssign = db.CTZ_Project_Assignment
                    .FirstOrDefault(a =>
                        a.ID_Project == project.ID_Project &&
                        a.ID_Department == (int)targetDept &&
                        a.WasRejected);
                var reasonOther = rejectedAssign?.RejectionReasonOther;

                var body = GetBodyQuoteRejected(project, rejectingDept.ToString(), targetDept.ToString(), now, reasonOther);
                var mail = new EnvioCorreoElectronico();
                mail.SendEmailAsync(recipients, subject, body);
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
        public string GetBodyOnHoldNotification(CTZ_Projects quote, string departmentName, DateTime holdDate)
        {
            // URL al detalle de la cotización
            string detailsUrl = Url.Action(
                "EditClientPartInformation",
                "CTZ_Projects",
                new { id = quote.ID_Project },
                protocol: Request.Url.Scheme
            );

            string holdDateStr = holdDate.ToString("MMMM dd, yyyy HH:mm");

            return $@"
<html>
<head><meta charset='utf-8'><title>Quote On Hold</title></head>
<body style='font-family:Arial,sans-serif; background:#f4f4f4; margin:0; padding:0;'>
  <table width='600' align='center' cellpadding='0' cellspacing='0' style='background:#fff;margin:20px auto;border-collapse:collapse;'>
    <tr>
      <td style='background:#009ff7;text-align:center;padding:20px;color:#fff;'>
        <h1 style='margin:0;font-size:24px;'>Quote On Hold</h1>
      </td>
    </tr>
    <tr>
      <td style='padding:30px;color:#333;font-size:14px;'>
        <p>Dear {departmentName} Team,</p>
        <p>The quote <strong>{quote.ConcatQuoteID}</strong> has been placed <strong>On Hold</strong> as of <strong>{holdDateStr}</strong>.</p>
        <p>Please await further updates or resolve any outstanding issues before proceeding.</p>
        <p style='text-align:center;margin:30px 0;'>
          <a href='{detailsUrl}' style='background:#009ff7;color:#fff;padding:12px 25px;text-decoration:none;border-radius:4px;font-size:16px;'>
            View Quote Details
          </a>
        </p>
        <p>Thank you for your attention.</p>
      </td>
    </tr>
    <tr>
      <td style='background:#009ff7;color:#fff;text-align:center;padding:10px;font-size:12px;'>
        &copy; {DateTime.Now:yyyy} Your Company Name. All rights reserved.
      </td>
    </tr>
  </table>
</body>
</html>";
        }

        [NonAction]
        public string GetBodyFinalizeAllNotification(CTZ_Projects quote, DateTime processedAt)
        {
            // URL al detalle de la cotización
            string detailsUrl = Url.Action(
                "EditClientPartInformation",
                "CTZ_Projects",
                new { id = quote.ID_Project },
                protocol: Request.Url.Scheme
            );

            string processedAtStr = processedAt.ToString("MMMM dd, yyyy HH:mm");

            return $@"
<html>
<head><meta charset='utf-8'><title>Order Cannot Be Processed</title></head>
<body style='font-family:Arial,sans-serif; background-color:#f4f4f4; margin:0; padding:0;'>
  <table width='600' align='center' cellpadding='0' cellspacing='0' style='background:#fff; margin:20px auto; padding:0; border-collapse:collapse;'>
    <tr>
      <td style='background:#009ff7; text-align:center; padding:20px; color:#fff;'>
        <h1 style='margin:0; font-size:24px;'>Quote Not Processable</h1>
      </td>
    </tr>
    <tr>
      <td style='padding:30px; color:#333; font-size:14px;'>
        <p>Dear Customer,</p>
        <p>We regret to inform you that <strong>Quote {quote.ConcatQuoteID}</strong> cannot be processed further as of <strong>{processedAtStr}</strong>.</p>
        <p>Please review the request details below and contact your sales representative if you need further assistance:</p>
        <table cellpadding='5' cellspacing='0' border='1' style='border-collapse:collapse; width:100%;'>
          <tr>
            <td style='background:#f2f9ff;'><strong>Quote ID</strong></td>
            <td>{quote.ConcatQuoteID}</td>
          </tr>
          <tr>
            <td style='background:#f2f9ff;'><strong>Created On</strong></td>
            <td>{quote.Creted_Date:MMMM dd, yyyy}</td>
          </tr>
          <tr>
            <td style='background:#f2f9ff;'><strong>Client</strong></td>
            <td>{(quote.CTZ_Clients?.Client_Name ?? quote.Cliente_Otro)}</td>
          </tr>
        </table>
        <p style='text-align:center; margin:30px 0;'>
          <a href='{detailsUrl}'
             style='background:#009ff7; color:#fff; padding:12px 25px; text-decoration:none; border-radius:4px;
                    display:inline-block; font-size:16px;'>
            View Quote Details
          </a>
        </p>
        <p>We apologize for any inconvenience. If you have questions, please reach out to your sales team.</p>
        <p>Thank you for your understanding.</p>
      </td>
    </tr>
    <tr>
      <td style='background:#009ff7; color:#fff; text-align:center; padding:10px; font-size:12px;'>
        &copy; {DateTime.Now:yyyy} Your Company Name. All rights reserved.
      </td>
    </tr>
  </table>
</body>
</html>";
        }


        [NonAction]
        public string GetBodyReassignment(
    CTZ_Projects quote,
    string fromDepartment,
    string toDepartment,
    DateTime assignmentDate)
        {
            // URL al detalle de la solicitud
            string detailsUrl = Url.Action(
                "EditClientPartInformation",
                "CTZ_Projects",
                new { id = quote.ID_Project },
                protocol: Request.Url.Scheme
            );

            // Formato de fecha
            string assignDateStr = assignmentDate.ToString("MM/dd/yyyy HH:mm");

            // Construcción del HTML
            var body = $@"
    <html>
      <head>
        <meta charset='utf-8' />
        <title>Quote Reassignment Notification</title>
      </head>
      <body style='margin:0; padding:0; background:#f4f4f4; font-family:Arial, sans-serif;'>
        <table align='center' width='600' cellpadding='0' cellspacing='0' 
               style='background:#fff; border-collapse:collapse; margin:20px auto; 
                      box-shadow:0 0 10px rgba(0,0,0,0.1);'>
          <!-- Header -->
          <tr>
            <td align='center' bgcolor='#009ff7' style='padding:20px;'>
              <h1 style='color:#fff; font-size:24px; margin:0;'>
                Quote Reassigned to {toDepartment}
              </h1>
            </td>
          </tr>

          <!-- Body -->
          <tr>
            <td style='padding:20px; color:#333; font-size:14px;'>
              <p>Dear {toDepartment} Team,</p>
              <p>
                The quote <strong>{quote.ConcatQuoteID}</strong> has been reassigned 
                from <strong>{fromDepartment}</strong> to your department on 
                <strong>{assignDateStr}</strong>. Please review the details below:
              </p>

              <table width='100%' cellpadding='5' cellspacing='0' 
                     style='border:1px solid #009ff7; border-collapse:collapse;'>
                <tr style='background:#f0f8ff;'>
                  <td style='border:1px solid #009ff7;'><strong>Quote ID:</strong></td>
                  <td style='border:1px solid #009ff7;'>{quote.ConcatQuoteID}</td>
                </tr>
                <tr>
                  <td style='background:#f0f8ff; border:1px solid #009ff7;'>
                    <strong>From Department:</strong>
                  </td>
                  <td style='border:1px solid #009ff7;'>{fromDepartment}</td>
                </tr>
                <tr style='background:#f0f8ff;'>
                  <td style='border:1px solid #009ff7;'><strong>To Department:</strong></td>
                  <td style='border:1px solid #009ff7;'>{toDepartment}</td>
                </tr>
                <tr>
                  <td style='background:#f0f8ff; border:1px solid #009ff7;'>
                    <strong>Assignment Date:</strong>
                  </td>
                  <td style='border:1px solid #009ff7;'>{assignDateStr}</td>
                </tr>
                <tr style='background:#f0f8ff;'>
                  <td style='border:1px solid #009ff7;'><strong>Client:</strong></td>
                  <td style='border:1px solid #009ff7;'>
                    {(quote.CTZ_Clients != null ? quote.CTZ_Clients.Client_Name : quote.Cliente_Otro)}
                  </td>
                </tr>
                <tr>
                  <td style='background:#f0f8ff; border:1px solid #009ff7;'><strong>Facility:</strong></td>
                  <td style='border:1px solid #009ff7;'>{quote.CTZ_plants.Description}</td>
                </tr>
              </table>

              <p style='margin:20px 0;'>
                Click the button below to view the full quote details:
              </p>

              <!-- Button -->
              <table role='presentation' cellpadding='0' cellspacing='0' align='center'>
                <tr>
                  <td align='center' bgcolor='#009ff7' 
                      style='border-radius:4px; padding:12px 24px;'>
                    <a href='{detailsUrl}' 
                       style='color:#fff; text-decoration:none; display:inline-block;
                              font-size:16px; font-family:Arial, sans-serif;'>
                      View Quote Details
                    </a>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td align='center' bgcolor='#009ff7' 
                style='padding:10px; color:#fff; font-size:12px;'>
              &copy; {DateTime.Now.Year} thyssenkrupp Materials de México. All rights reserved.
            </td>
          </tr>
        </table>
      </body>
    </html>";

            return body;
        }


        [NonAction]
        public string getBodyNewQuote(CTZ_Projects quote, string departmentName, DateTime assignmentDate)
        {
            // Generar la URL del botón
            string detailsUrl = Url.Action(
                "EditClientPartInformation",
                "CTZ_Projects",
                new { id = quote.ID_Project },
                protocol: Request.Url.Scheme
            );

            // Formatear fecha de asignación
            string asignDateStr = assignmentDate.ToString("dd/MM/yyyy HH:mm");

            string body = $@"
            <html>
            <head>
                <meta charset='utf-8'>
                <title>Quote Assignment Notification</title>
            </head>
            <body style='margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;'>
                <table align='center' border='0' cellpadding='0' cellspacing='0' width='600'
                       style='border-collapse: collapse; margin:20px auto; background-color:#ffffff;
                              box-shadow:0 0 10px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td align='center' bgcolor='#009ff7' style='padding:20px 0;'>
                            <h1 style='color:#ffffff; margin:0; font-size:26px;'>
                                Quote Assigned to {departmentName}
                            </h1>
                        </td>
                    </tr>
                  
                    <!-- Body Content -->
                    <tr>
                        <td style='padding:15px 30px;'>
                            <p style='font-size:14px; color:#333333; margin:0 0 20px 0;'>
                                Dear {departmentName} Team,
                            </p>
                            <p style='font-size:14px; color:#333333; margin:0 0 20px 0;'>
                                A quote has just been assigned to your department. Please review the details below:
                            </p>

                            <table border='0' cellpadding='5' cellspacing='0' width='100%'
                                   style='font-size:14px; color:#333333;
                                          border:1px solid #009ff7; border-collapse:collapse;'>
                                <tr>
                                    <td style='background-color:#f2f9ff; border:1px solid #009ff7;'>
                                        <strong>Quote ID:</strong>
                                    </td>
                                    <td style='border:1px solid #009ff7;'>
                                        {quote.ConcatQuoteID}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='background-color:#f2f9ff; border:1px solid #009ff7;'>
                                        <strong>Department:</strong>
                                    </td>
                                    <td style='border:1px solid #009ff7;'>
                                        {departmentName}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='background-color:#f2f9ff; border:1px solid #009ff7;'>
                                        <strong>Assignment Date:</strong>
                                    </td>
                                    <td style='border:1px solid #009ff7;'>
                                        {asignDateStr}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='background-color:#f2f9ff; border:1px solid #009ff7;'>
                                        <strong>Client:</strong>
                                    </td>
                                    <td style='border:1px solid #009ff7;'>
                                        {(quote.CTZ_Clients != null ? quote.CTZ_Clients.Client_Name : quote.Cliente_Otro)}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='background-color:#f2f9ff; border:1px solid #009ff7;'>
                                        <strong>Facility:</strong>
                                    </td>
                                    <td style='border:1px solid #009ff7;'>
                                        {quote.CTZ_plants.Description}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='background-color:#f2f9ff; border:1px solid #009ff7;'>
                                        <strong>Created Date:</strong>
                                    </td>
                                    <td style='border:1px solid #009ff7;'>
                                        {quote.Creted_Date.ToString("dd/MM/yyyy")}
                                    </td>
                                </tr>
                            </table>

                            <p style='font-size:14px; color:#333333; margin:20px 0;'>
                                Click the button below to view full request details.
                            </p>

                            <!-- Botón reforzado -->
                            <!-- Botón reforzado: Bulletproof button -->
                            <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"">
                              <tr>
                                <td align=""center"" bgcolor=""#009ff7"" 
                                    style=""border-radius:4px; background-color:#009ff7;"">
                                  <!--[if mso]>
                                  <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" 
                                               xmlns:w=""urn:schemas-microsoft-com:office:word"" 
                                               href=""{detailsUrl}"" style=""height:40px; v-text-anchor:middle; width:200px;"" 
                                               arcsize=""8%"" stroke=""false"" fillcolor=""#009ff7"">
                                    <w:anchorlock/>
                                    <center style=""color:#ffffff; font-family:Arial, sans-serif; font-size:16px;"">
                                      View Request Details
                                    </center>
                                  </v:roundrect>
                                  <![endif]-->
                                  <![if !mso]>
                                  <a href=""{detailsUrl}""
                                     style=""
                                       display:inline-block;
                                       padding:12px 30px;
                                       font-size:16px;
                                       color:#ffffff;
                                       text-decoration:none;
                                       border:2px solid #007acc;
                                       border-radius:4px;
                                       background-color:#009ff7;
                                       font-family:Arial, sans-serif;
                                     "">
                                    View Request Details
                                  </a>
                                  <![endif]>
                                </td>
                              </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td bgcolor='#009ff7'
                            style='padding:10px; text-align:center; font-size:12px; color:#ffffff;'>
                            &copy; {DateTime.Now.Year} thyssenkrupp Materials de México (tkMM). All rights reserved.
                        </td>
                    </tr>
                </table>
            </body>
            </html>";

            return body;
        }

        /// <summary>
        /// Genera el cuerpo HTML para el correo de finalización de la cotización.
        /// </summary>
        /// <summary>
        /// Genera el cuerpo HTML para el correo de finalización de la cotización
        /// usando la paleta azul de tkMM.
        /// </summary>
        /// <param name="quote">Proyecto/cotización a notificar</param>
        /// <param name="finalizedDate">Fecha y hora de finalización</param>
        /// <returns>HTML completo para el cuerpo del correo</returns>
        public string GetBodyQuoteFinalized(CTZ_Projects quote, DateTime finalizedDate)
        {
            // URL del botón "View Quote Details"
            string detailsUrl = Url.Action(
                "EditProject", "CTZ_Projects",
                new { id = quote.ID_Project },
                protocol: Request.Url.Scheme
            );

            // Formatear fecha y hora
            string dateStr = finalizedDate.ToString("dd/MM/yyyy HH:mm");

            // Construcción del HTML con la paleta #009ff7
            string body = $@"
    <html>
    <head>
        <meta charset='utf-8'>
        <title>Quote Finalized</title>
    </head>
    <body style='margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;'>
        <table align='center' border='0' cellpadding='0' cellspacing='0' width='600'
               style='border-collapse:collapse; margin:20px auto; background-color:#ffffff;
                      box-shadow:0 0 10px rgba(0,0,0,0.1);'>
            <!-- Header -->
            <tr>
                <td align='center' bgcolor='#009ff7' style='padding:20px 0;'>
                    <h1 style='color:#ffffff; margin:0; font-size:26px;'>
                        Quote Finalized
                    </h1>
                </td>
            </tr>

            <!-- Body Content -->
            <tr>
                <td style='padding:30px;'>
                    <p style='font-size:14px; color:#333333; margin-bottom:20px;'>
                        Hello {quote.empleados?.nombre ?? "User"},
                    </p>
                    <p style='font-size:14px; color:#333333; margin-bottom:20px;'>
                        Your quote <strong>{quote.ConcatQuoteID}</strong> has been finalized on <strong>{dateStr}</strong>.
                    </p>

                    <!-- Detalles del quote -->
                    <table border='0' cellpadding='5' cellspacing='0' width='100%'
                           style='font-size:14px; color:#333333; border:1px solid #009ff7;
                                  border-collapse:collapse; margin-bottom:30px;'>
                        <tr>
                            <td style='background-color:#e6f4ff; border:1px solid #009ff7;'><strong>Client:</strong></td>
                            <td style='border:1px solid #009ff7;'>
                                {(quote.CTZ_Clients != null ? quote.CTZ_Clients.Client_Name : quote.Cliente_Otro)}
                            </td>
                        </tr>
                        <tr>
                            <td style='background-color:#e6f4ff; border:1px solid #009ff7;'><strong>Facility:</strong></td>
                            <td style='border:1px solid #009ff7;'>{quote.CTZ_plants.Description}</td>
                        </tr>
                        <tr>
                            <td style='background-color:#e6f4ff; border:1px solid #009ff7;'><strong>Version:</strong></td>
                            <td style='border:1px solid #009ff7;'>{quote.LastedVersionNumber}</td>
                        </tr>
                    </table>

                    <!-- Bulletproof button -->
                    <table role='presentation' border='0' cellpadding='0' cellspacing='0' align='center'>
                      <tr>
                        <td align='center' bgcolor='#009ff7' style='border-radius:4px;'>
                          <!--[if mso]>
                          <v:roundrect xmlns:v='urn:schemas-microsoft-com:vml' href='{detailsUrl}'
                                       style='height:40px; v-text-anchor:middle; width:220px;'
                                       arcsize='8%' stroke='false' fillcolor='#009ff7'>
                            <w:anchorlock/>
                            <center style='color:#ffffff; font-family:Arial, sans-serif; font-size:16px;'>
                              View Quote Details
                            </center>
                          </v:roundrect>
                          <![endif]-->
                          <![if !mso]>
                          <a href='{detailsUrl}'
                             style='display:inline-block; padding:12px 30px; font-size:16px;
                                    color:#ffffff; text-decoration:none; border:2px solid #007acc;
                                    border-radius:4px; background-color:#009ff7;
                                    font-family:Arial, sans-serif;'>
                            View Quote Details
                          </a>
                          <![endif]>
                        </td>
                      </tr>
                    </table>
                </td>
            </tr>

            <!-- Footer -->
            <tr>
                <td align='center' bgcolor='#009ff7' style='padding:10px; font-size:12px; color:#ffffff;'>
                    &copy; {DateTime.Now.Year} thyssenkrupp Materials de México (tkMM). All rights reserved.
                </td>
            </tr>
        </table>
    </body>
    </html>";

            return body;
        }

        /// <summary>
        /// Genera el HTML para notificar al solicitante que su cotización fue rechazada.
        /// </summary>
        /// <param name="quote">El proyecto/cotización</param>
        /// <param name="departmentRejectingName">Nombre del depto que rechazó</param>
        /// <param name="rejectionDate">Fecha de rechazo</param>
        /// <param name="reason">Motivo de rechazo (comentarios)</param>
        public string GetBodyQuoteRejected(CTZ_Projects quote, string departmentRejectingName, string targetDeptName, DateTime rejectionDate, string reason)
        {
            string detailsUrl = Url.Action(
                "EditProject",
                "CTZ_Projects",
                new { id = quote.ID_Project },
                protocol: Request.Url.Scheme
            );

            string dateStr = rejectionDate.ToString("dd/MM/yyyy HH:mm");

            return $@"
<html>
<head>
    <meta charset='utf-8'>
    <title>Quote Rejected Notification</title>
</head>
<body style='margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;'>
    <table align='center' border='0' cellpadding='0' cellspacing='0' width='600'
           style='border-collapse: collapse; margin:20px auto; background-color:#ffffff;
                  box-shadow:0 0 10px rgba(0,0,0,0.1);'>
        <!-- Header -->
        <tr>
            <td align='center' bgcolor='#d9534f' style='padding:20px 0;'>
                <h1 style='color:#ffffff; margin:0; font-size:24px;'>Quote Rejected</h1>
            </td>
        </tr>
        <!-- Body -->
        <tr>
            <td style='padding:30px; color:#333333; font-size:14px;'>         
                <p>The quote <strong>{quote.ConcatQuoteID}</strong> was <span style='color:#d9534f;font-weight:bold;'>rejected</span> by <strong>{departmentRejectingName}</strong> and reassigned to <span style='color:#d9534f;font-weight:bold;'>{targetDeptName}</span> on <strong>{dateStr}</strong>.</p>
                <p><strong>Reason for rejection:</strong></p>
                <blockquote style='background:#DDDDDD;border-left:4px solid #d9534f;padding:10px;margin:10px 0;'>{HttpUtility.HtmlEncode(reason)}</blockquote>
                <p>You can review or update your request by clicking the button below:</p>
                <div style='text-align:center; margin:30px 0;'>
                    <a href='{detailsUrl}'
                       style='background-color:#d9534f; color:#ffffff; padding:12px 25px;
                              text-decoration:none; border-radius:4px; font-size:16px; display:inline-block;'>
                        View Quote Details
                    </a>
                </div>               
            </td>
        </tr>
        <!-- Footer -->
        <tr>
            <td bgcolor='#d9534f' style='padding:10px; text-align:center; font-size:12px; color:#DDDDDD;'>
                &copy; {DateTime.Now.Year} thyssenkrupp Materials de México (tkMM). All rights reserved.
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        [HttpGet]
        public JsonResult GetSlitterLines(int plantId)
        {
            var slitterLines = db.CTZ_Production_Lines
                .Where(l => l.ID_Plant == plantId && l.IsSlitter == true && l.Active)
                .Select(l => new { Value = l.ID_Line, Text = l.Description })
                .ToList();

            return Json(slitterLines, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetClientDetails(int id)
        {
            var client = db.CTZ_Clients.Find(id);
            if (client == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            // Creamos un objeto anónimo para devolver solo lo que necesitamos
            var result = new
            {
                Address = client.Direccion,
                Telephone = client.Telephone
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOemDetails(int id)
        {
            var oem = db.CTZ_OEMClients.Find(id);
            if (oem == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                Address = oem.Direccion,
                Telephone = oem.Telephone
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Obtiene los límites de validación (Min/Max) para una combinación específica de Línea y Tipo de Material.
        /// Los datos se extraen de la tabla de Información Técnica.
        /// </summary>
        [HttpGet]
        public JsonResult GetEngineeringDimensions(
            int materialTypeId,
            int? primaryLineId,
            int? slitterLineId = null,
            double? thickness = null,
            double? tensile = null
        )
        {
            try
            {
                // 1. Objeto de respuesta inicializado
                var result = new Dictionary<string, object>
        {
            { "success", true },
            { "validationRanges", new List<object>() },
            { "maxMultsAllowed", null } // Valor que vamos a calcular
        };

                // 2. Obtener los rangos de validación BASE desde la línea principal (Blanking o Slitter)
                // Esto no cambia, siempre necesitamos los límites físicos de la máquina.
                if (primaryLineId.HasValue)
                {
                    var baseRanges = db.CTZ_Technical_Information_Line
                        .Where(ti => ti.ID_Line == primaryLineId.Value && ti.ID_Material_type == materialTypeId && ti.IsActive)
                        .Select(ti => new
                        {
                            ID_Criteria = ti.ID_Criteria,
                            NumericValue = ti.NumericValue,
                            MinValue = ti.MinValue,
                            MaxValue = ti.MaxValue,
                            Tolerance = ti.AbsoluteTolerance
                        })
                        .ToList();

                    result["validationRanges"] = baseRanges;
                }

                // 3. --- NUEVA LÓGICA ---
                // Si tenemos los datos necesarios (slitter, espesor y resistencia), calculamos el máximo de mults.
                if (slitterLineId.HasValue && thickness.HasValue && tensile.HasValue)
                {
                    // Buscamos la regla en CTZ_Slitting_Validation_Rules que coincida con el espesor y la resistencia proporcionados.
                    var rule = db.CTZ_Slitting_Validation_Rules.FirstOrDefault(r =>
                        r.ID_Production_Line == slitterLineId.Value &&
                        thickness.Value >= r.Thickness_Min &&
                        thickness.Value <= r.Thickness_Max &&
                        tensile.Value >= r.Tensile_Min &&
                        tensile.Value <= r.Tensile_Max &&
                        r.Is_Active);

                    if (rule != null)
                    {
                        // Si encontramos una regla, asignamos el valor de Mults_Max a nuestra respuesta.
                        result["maxMultsAllowed"] = rule.Mults_Max;
                    }
                    else
                    {
                        // Si no se encuentra una regla, significa que la combinación no es válida.
                        // Devolvemos 0 para que la validación en el front-end falle si el usuario ingresa > 0.
                        result["maxMultsAllowed"] = 0;
                    }
                }

                // La lógica de intersección ya no es necesaria aquí, porque los rangos de validación
                // provienen únicamente de la línea primaria (baseRanges).

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult ValidateLineForMaterial(int lineId, int materialTypeId)
        {
            // Buscamos en la tabla de relación si existe una entrada que vincule
            // esta línea con este tipo de material.
            bool isValid = db.CTZ_Material_Type_Lines
                             .Any(mtl => mtl.ID_Line == lineId && mtl.ID_Material_Type == materialTypeId);

            // Devolvemos un JSON simple con el resultado.
            return Json(new { isValid = isValid }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // Agrega este nuevo método en cualquier parte dentro de tu CTZ_ProjectsController

        [HttpGet]
        public JsonResult GetAvailableMaterialTypesForSlitter(int plantId, int slitterLineId)
        {
            try
            {
                // 1. Obtener los IDs de los tipos de material asociados a la línea de slitter específica.
                var materialTypeIds = db.CTZ_Material_Type_Lines
                    .Where(mtl => mtl.ID_Line == slitterLineId)
                    .Select(mtl => mtl.ID_Material_Type)
                    .Distinct();

                // 2. Obtener la lista completa de tipos de material disponibles para esa planta (como fallback o base).
                var allMaterialTypesForPlant = db.CTZ_Production_Lines
                    .Where(l => l.ID_Plant == plantId && l.Active)
                    .SelectMany(l => l.CTZ_Material_Type_Lines)
                    .Select(mtl => mtl.CTZ_Material_Type)
                    .Where(mt => mt.Active)
                    .Distinct()
                    .ToList();

                // 3. Filtrar la lista completa para quedarnos solo con los que son compatibles con el slitter.
                var availableMaterialTypes = allMaterialTypesForPlant
                    .Where(mt => materialTypeIds.Contains(mt.ID_Material_Type))
                    .OrderBy(mt => mt.Material_Name)
                    .Select(mt => new
                    {
                        Value = mt.ID_Material_Type,
                        Text = mt.Material_Name
                    })
                    .ToList();

                return Json(new { success = true, data = availableMaterialTypes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetProductionLineData(int plantId, bool applyDateFilter = false)
        {
            try
            {
                // --- INICIO DEL CÓDIGO Filtrar por fecha ---

                // 1. Obtener los años fiscales ordenados.
                var fiscalYearsQuery = db.CTZ_Fiscal_Years.OrderBy(fy => fy.ID_Fiscal_Year);

                // Esta será la lista de años fiscales que procesaremos.
                List<CTZ_Fiscal_Years> fiscalYears;

                // 2. Si el filtro de fecha está activado, calculamos el rango.
                if (applyDateFilter)
                {
                    var today = DateTime.Now;

                    // Encuentra el año fiscal actual basándose en la fecha de hoy.
                    var currentFiscalYear = db.CTZ_Fiscal_Years
                        .FirstOrDefault(fy => today >= fy.Start_Date && today <= fy.End_Date);

                    if (currentFiscalYear != null)
                    {
                        // Si encontramos el año actual, tomamos ese y los 3 siguientes.
                        // Usamos .Where() sobre la consulta original para mantener el orden.
                        fiscalYears = fiscalYearsQuery
                            .Where(fy => fy.ID_Fiscal_Year >= currentFiscalYear.ID_Fiscal_Year)
                            .Take(4) // Tomamos el actual + 3 siguientes = 4 en total.
                            .ToList();
                    }
                    else
                    {
                        // Si por alguna razón no se encuentra un FY actual (poco probable), 
                        // cargamos todos como medida de seguridad.
                        fiscalYears = fiscalYearsQuery.ToList();
                    }
                }
                else
                {
                    // Si el filtro no está activado, cargamos todos los años fiscales.
                    fiscalYears = db.CTZ_Fiscal_Years
                    .OrderBy(fy => fy.ID_Fiscal_Year)
                    .ToList();
                }

                // --- FIN DEL CÓDIGO Filtrar por fecha ---

                // 1. Obtener las líneas de producción activas para la planta
                var productionLines = db.CTZ_Production_Lines
                    .Where(l => l.ID_Plant == plantId && l.Active)
                    .ToList();

                // 3. Obtener las horas totales disponibles por FY, **filtrando por la planta seleccionada**.
                var totalTimeByFY = db.CTZ_Total_Time_Per_Fiscal_Year
                         .Where(t => t.ID_Plant == plantId)
                         .ToDictionary(t => t.ID_Fiscal_Year, t => t); // Guardamos el objeto completo, no solo un valor.

                // Definir el orden deseado para los estatus
                // Por ejemplo: "POH" primero, luego "Casi Casi", seguido de "Carry Over" y finalmente "Quotes"
                var orderMapping = new Dictionary<string, int>
                {
                    { "POH", 1 }, { "Casi Casi", 2 }, { "Carry Over", 3 }, { "Quotes", 4 }
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
                        // ▼▼▼ INICIO DE LA MODIFICACIÓN #2 ▼▼▼
                        // Variable para el tiempo total disponible (puede ser horas o turnos)
                        double totalAvailableTime = 0;

                        // Verificamos si la línea es de tipo Slitter
                        if (line.IsSlitter)
                        {
                            // Si es SLITTER, usamos los turnos (Shifts_SLT)
                            if (totalTimeByFY.ContainsKey(fy.ID_Fiscal_Year))
                            {
                                totalAvailableTime = totalTimeByFY[fy.ID_Fiscal_Year].Shifts_SLT.GetValueOrDefault(0);
                            }
                        }
                        else
                        {
                            // Si NO es SLITTER (es Blanking), usamos las horas (Hours_BLK)
                            if (totalTimeByFY.ContainsKey(fy.ID_Fiscal_Year))
                            {
                                totalAvailableTime = totalTimeByFY[fy.ID_Fiscal_Year].Hours_BLK.GetValueOrDefault(0);
                            }
                        }
                        // ▲▲▲ FIN DE LA MODIFICACIÓN #2 ▲▲▲

                        var hoursEntries = db.CTZ_Hours_By_Line
                            .Where(h => h.ID_Line == line.ID_Line && h.ID_Fiscal_Year == fy.ID_Fiscal_Year)
                            .ToList();

                        // Para Slitter, la columna "Hours" en realidad representa "Shifts"
                        double occupiedTime = hoursEntries.Sum(x => x.Hours);

                        var breakdown = hoursEntries
                            .GroupBy(x => x.CTZ_Project_Status.Description)
                            .Select(g => new
                            {
                                StatusId = g.Key,
                                OccupiedHours = g.Sum(x => x.Hours),
                                // ▼▼▼ INICIO DE LA MODIFICACIÓN #3 ▼▼▼
                                // Usamos la nueva variable 'totalAvailableTime' para el cálculo
                                Percentage = totalAvailableTime > 0 ? Math.Round((g.Sum(x => x.Hours) / totalAvailableTime) * 100, 2) : 0
                                // ▲▲▲ FIN DE LA MODIFICACIÓN #3 ▲▲▲
                            })
                            .OrderBy(b => orderMapping.ContainsKey(b.StatusId) ? orderMapping[b.StatusId] : 100)
                            .ToList();

                        lineData.DataByFY.Add(new
                        {
                            FiscalYear = fy.Fiscal_Year_Name,
                            TotalOccupied = occupiedTime,
                            // ▼▼▼ INICIO DE LA MODIFICACIÓN #4 ▼▼▼
                            // Pasamos el valor correcto (horas o turnos) a la propiedad TotalHours del JSON
                            TotalHours = totalAvailableTime,
                            // ▲▲▲ FIN DE LA MODIFICACIÓN #4 ▲▲▲
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
        public ActionResult GetMaterialCapacityScenarios(int projectId, int? materialId, int? blkID, string vehicle, double? partsPerVehicle, double? idealCycleTimePerTool,
                                                         double? blanksPerStroke, double? oee, DateTime? realSOP, DateTime? realEOP, int? annualVol, bool OnlyBDMaterials = false)
        {
            try
            {
                // 1. Cargar el proyecto con sus materiales
                var project = db.CTZ_Projects
                                .Include("CTZ_Project_Materials")
                                .FirstOrDefault(p => p.ID_Project == projectId);
                if (project == null)
                    return Json(new { success = false, message = "Proyecto no encontrado." }, JsonRequestBehavior.AllowGet);

                /*** Part number concatenado ***/

                // Agrupamos los materiales por su línea (real o teórica) y concatenamos sus Part_Number
                var partNumbersByLine = project.CTZ_Project_Materials
                    .Where(m => (m.ID_Real_Blanking_Line ?? m.ID_Theoretical_Blanking_Line) != null)
                    .GroupBy(m => m.ID_Real_Blanking_Line ?? m.ID_Theoretical_Blanking_Line)
                    .ToDictionary(
                        g => g.Key.Value, // La llave es el ID de la línea
                                          // El valor es un string con los Part_Number unidos por ", "
                        g => string.Join(", ", g.Select(m => m.Part_Number).Where(pn => !string.IsNullOrEmpty(pn)))
                    );

                /**/

                // 2. Obtener el material seleccionado o tratar como nuevo si es null
                CTZ_Project_Materials selectedMaterial = null;
                if (!OnlyBDMaterials)
                { //si onlyBDmaterials no esta activo

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

                    // V V V --- LÍNEA A AGREGAR --- V V V
                    row["PartNumbers"] = partNumbersByLine.ContainsKey(lineId) ? partNumbersByLine[lineId] : "";
                    // ^ ^ ^ --- FIN DE LA LÍNEA A AGREGAR --- ^ ^ ^

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
                if (selectedMaterial != null && selectedMaterial.Real_SOP.HasValue && selectedMaterial.Real_EOP.HasValue)
                {
                    effectiveSOP = selectedMaterial.Real_SOP.Value;
                    effectiveEOP = selectedMaterial.Real_EOP.Value;
                }
                else if (selectedMaterial != null && selectedMaterial.SOP_SP.HasValue && selectedMaterial.EOP_SP.HasValue)
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
                int lineForStatus = blkID.HasValue ? blkID.Value : 0;
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
                                                           double? oee, DateTime? realSOP, DateTime? realEOP, int? annualVol, bool OnlyBDMaterials = false, bool isSnapShot = false, int? versionId = null)
        {
            try
            {
                // Declaramos las nuevas variables que vamos a retornar
                double? oeeToReturn = null;
                bool oeeIsCalculated = false;

                // 1. Cargar el proyecto con sus materiales
                var project = db.CTZ_Projects
                                .Include("CTZ_Project_Materials")
                                .FirstOrDefault(p => p.ID_Project == projectId);
                if (project == null)
                    return Json(new { success = false, message = "Proyecto no encontrado." }, JsonRequestBehavior.AllowGet);


                var fiscalYears = db.CTZ_Fiscal_Years.OrderBy(fy => fy.ID_Fiscal_Year).ToList();

                // 2. Obtener el material seleccionado o tratarlo como nuevo si es null
                if (!OnlyBDMaterials)
                { //si onlyBDmaterials no esta activo

                    var oeeResult = GetOeeToUse(oee, blkID, db);
                    oeeToReturn = oeeResult.oeeValue; // Guardamos el valor para el JSON de respuesta.
                    oeeIsCalculated = oeeResult.isCalculated; // Guardamos el indicador.


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

                // ↓↓↓ AQUÍ: Elegimos la lista que vamos a pasar a GetGraphCapacityScenarios ↓↓↓
                List<CTZ_Project_Materials> materialsToUse;

                if (isSnapShot && versionId.HasValue)
                {
                    // Cargo los snapshot desde el history y mapeo a CTZ_Project_Materials
                    materialsToUse = db.CTZ_Project_Materials_History
                        .Where(h => h.ID_Version == versionId.Value)
                        .ToList()
                        .Select(h => new CTZ_Project_Materials
                        {
                            // Mapeo SOLO de los campos que usa tu método:
                            ID_Material = h.ID_History,
                            ID_IHS_Item = h.ID_IHS_Item,
                            Max_Production_SP = h.Max_Production_SP,
                            Program_SP = h.Program_SP,
                            Vehicle_version = h.Vehicle_version,
                            SOP_SP = h.SOP_SP,
                            EOP_SP = h.EOP_SP,
                            Real_SOP = h.Real_SOP,
                            Real_EOP = h.Real_EOP,
                            Ship_To = h.Ship_To,
                            Part_Name = h.Part_Name,
                            Part_Number = h.Part_Number,
                            ID_Route = h.ID_Route,
                            Quality = h.Quality,
                            Tensile_Strenght = h.Tensile_Strenght,
                            ID_Material_type = h.ID_Material_type,
                            Thickness = h.Thickness,
                            Width = h.Width,
                            Pitch = h.Pitch,
                            Theoretical_Gross_Weight = h.Theoretical_Gross_Weight,
                            Gross_Weight = h.Gross_Weight,
                            Annual_Volume = h.Annual_Volume,
                            Volume_Per_year = h.Volume_Per_year,
                            ID_Shape = h.ID_Shape,
                            Angle_A = h.Angle_A,
                            Angle_B = h.Angle_B,
                            Blanks_Per_Stroke = h.Blanks_Per_Stroke,
                            Parts_Per_Vehicle = h.Parts_Per_Vehicle,
                            ID_Theoretical_Blanking_Line = h.ID_Theoretical_Blanking_Line,
                            ID_Real_Blanking_Line = h.ID_Real_Blanking_Line,
                            Theoretical_Strokes = h.Theoretical_Strokes,
                            Real_Strokes = h.Real_Strokes,
                            Ideal_Cycle_Time_Per_Tool = h.Ideal_Cycle_Time_Per_Tool,
                            OEE = h.OEE,
                            ID_Project = h.ID_Project,
                            Vehicle = h.Vehicle,
                            Vehicle_2 = h.Vehicle_2,
                            Vehicle_3 = h.Vehicle_3,
                            Vehicle_4 = h.Vehicle_4,
                            ThicknessToleranceNegative = h.ThicknessToleranceNegative,
                            ThicknessTolerancePositive = h.ThicknessTolerancePositive,
                            WidthToleranceNegative = h.WidthToleranceNegative,
                            WidthTolerancePositive = h.WidthTolerancePositive,
                            PitchToleranceNegative = h.PitchToleranceNegative,
                            PitchTolerancePositive = h.PitchTolerancePositive,
                            WeightOfFinalMults = h.WeightOfFinalMults,
                            Multipliers = h.Multipliers,
                            AngleAToleranceNegative = h.AngleAToleranceNegative,
                            AngleATolerancePositive = h.AngleATolerancePositive,
                            AngleBToleranceNegative = h.AngleBToleranceNegative,
                            AngleBTolerancePositive = h.AngleBTolerancePositive,
                            MajorBase = h.MajorBase,
                            MajorBaseToleranceNegative = h.MajorBaseToleranceNegative,
                            MajorBaseTolerancePositive = h.MajorBaseTolerancePositive,
                            MinorBase = h.MinorBase,
                            MinorBaseToleranceNegative = h.MinorBaseToleranceNegative,
                            MinorBaseTolerancePositive = h.MinorBaseTolerancePositive,
                            Flatness = h.Flatness,
                            FlatnessToleranceNegative = h.FlatnessToleranceNegative,
                            FlatnessTolerancePositive = h.FlatnessTolerancePositive,
                            MasterCoilWeight = h.MasterCoilWeight,
                            InnerCoilDiameterArrival = h.InnerCoilDiameterArrival,
                            OuterCoilDiameterArrival = h.OuterCoilDiameterArrival,
                            InnerCoilDiameterDelivery = h.InnerCoilDiameterDelivery,
                            OuterCoilDiameterDelivery = h.OuterCoilDiameterDelivery,
                            PackagingStandard = h.PackagingStandard,
                            SpecialRequirement = h.SpecialRequirement,
                            SpecialPackaging = h.SpecialPackaging,
                            ID_File_CAD_Drawing = h.ID_File_CAD_Drawing,
                            TurnOver = h.TurnOver,
                            CTZ_Files = h.CTZ_Files,
                            CTZ_Material_Type = h.CTZ_Material_Type,
                            CTZ_Production_Lines = h.CTZ_Production_Lines,
                            CTZ_Production_Lines1 = h.CTZ_Production_Lines1,
                            CTZ_Projects = h.CTZ_Projects,
                            CTZ_Route = h.CTZ_Route,
                            SCDM_cat_forma_material = h.SCDM_cat_forma_material,
                        })
                        .ToList();
                }
                else
                {
                    // Uso la lista “viva” original
                    materialsToUse = project.CTZ_Project_Materials.ToList();
                }

                // agrupamos por línea real o teórica y calculamos FY de inicio/fin
                var fyRanges = materialsToUse
                    .Where(m => m.Real_SOP.HasValue && m.Real_EOP.HasValue)
                    .GroupBy(m => m.ID_Real_Blanking_Line ?? m.ID_Theoretical_Blanking_Line)
                    .Where(g => g.Key.HasValue && g.Key.Value != 0)
                    .ToDictionary(
                      g => g.Key.Value.ToString(),
                      g =>
                      {
                          var minDate = g.Min(m => m.Real_SOP.Value);
                          var maxDate = g.Max(m => m.Real_EOP.Value);
                          // FY que contiene minDate
                          var startFY = fiscalYears
            .First(f => f.Start_Date <= minDate && f.End_Date >= minDate)
            .Fiscal_Year_Name;
                          // FY que contiene maxDate
                          var endFY = fiscalYears
            .First(f => f.Start_Date <= maxDate && f.End_Date >= maxDate)
            .Fiscal_Year_Name;
                          return new { MinFY = startFY, MaxFY = endFY };
                      }
                    );


                // --- INICIO: Bloque para filtrar la producción de materialsToUse ---

                // 1. Obtenemos una lista de todos los años fiscales para hacer la conversión de fecha a ID de FY.
                var allFiscalYearsForFiltering = db.CTZ_Fiscal_Years.ToList();



                // 3. Obtener el diccionario final de capacidad utilizando tu método existente.
                // finalPercentageDict es del tipo Dictionary<int, Dictionary<int, Dictionary<int, double>>>
                var finalPercentageDict = project.GetGraphCapacityScenarios(materialsToUse);


                // *** NUEVO BLOQUE: CALCULO Y GUARDO DM_Status ***
                var dmStatuses = new Dictionary<int, string>();
                var dmStatusComments = new Dictionary<int, string>();
                if (!OnlyBDMaterials)
                {
                    int projStatusId = project.ID_Status;
                    var fiscalYearsBD = db.CTZ_Fiscal_Years.ToList();

                    var debugLines = new List<string>();

                    // Diccionarios de salida


                    foreach (var mat in project.CTZ_Project_Materials)
                    {
                        debugLines.Add($"--- Material {mat.ID_Material} ---");

                        if (!mat.Real_SOP.HasValue || !mat.Real_EOP.HasValue)
                        {
                            debugLines.Add("   > Skipped: missing Real_SOP or Real_EOP");
                            continue;
                        }

                        var sop = mat.Real_SOP.Value.Date;
                        var eop = mat.Real_EOP.Value.Date;
                        debugLines.Add($"   SOP={sop:yyyy-MM-dd}, EOP={eop:yyyy-MM-dd}");

                        // 1) Encuentro todos los FY que se cruzan con [sop,eop]
                        var overlappingFY = fiscalYearsBD
                            .Where(f => f.Start_Date <= eop && f.End_Date >= sop)
                            .OrderBy(f => f.ID_Fiscal_Year)
                            .ToList();
                        if (!overlappingFY.Any())
                        {
                            debugLines.Add("   > No fiscal years overlap");
                            continue;
                        }
                        debugLines.Add($"   Overlaps FY IDs: {string.Join(", ", overlappingFY.Select(f => f.ID_Fiscal_Year))}");

                        // 2) Determino la línea a usar
                        int lineId = mat.ID_Real_Blanking_Line
                                     ?? mat.ID_Theoretical_Blanking_Line
                                     ?? 0;
                        if (lineId == 0)
                        {
                            debugLines.Add("   > Skipped: no blanking line");
                            continue;
                        }
                        if (!finalPercentageDict.TryGetValue(lineId, out var byStatus))
                        {
                            debugLines.Add($"   > No data in finalPercentageDict for line {lineId}");
                            continue;
                        }

                        // 3) Para cada FY sumo todos los rawPct y guardo el peor
                        double worstPct = 0;
                        string worstFY = "";

                        foreach (var fy in overlappingFY)
                        {
                            double sumRaw = 0;
                            foreach (var statusEntry in byStatus)
                            {
                                if (statusEntry.Value.TryGetValue(fy.ID_Fiscal_Year, out var rawPct))
                                    sumRaw += rawPct;
                            }
                            double pctYear = sumRaw * 100.0;
                            debugLines.Add($"      FY{fy.ID_Fiscal_Year}: sumRaw={sumRaw:F4}, pct={pctYear:F2}%");

                            if (pctYear > worstPct)
                            {
                                worstPct = pctYear;
                                worstFY = fy.Fiscal_Year_Name;
                            }
                        }

                        // 4) Mapéo el peor porcentaje a DM_Status
                        string dmStatus = worstPct >= 98.0
                                          ? "Rejected"
                                          : (worstPct >= 95.0
                                              ? "On Review"
                                              : "Approved");

                        // 5) Construyo el comentario en inglés
                        string dmComment = $"{dmStatus}: max value was {worstPct:F2}% in {worstFY}";

                        debugLines.Add($"   → worstPct={worstPct:F2}, FY={worstFY}");
                        debugLines.Add($"   → DM_Status='{dmStatus}', Comment='{dmComment}'");

                        dmStatuses[mat.ID_Material] = dmStatus;
                        dmStatusComments[mat.ID_Material] = dmComment;

                        // 6) Actualizo en BD ambos campos
                        //var toUpdate = db.CTZ_Project_Materials.Find(mat.ID_Material);
                        //if (toUpdate != null)
                        //{
                        //    toUpdate.DM_status = dmStatus;
                        //    toUpdate.DM_status_comment = dmComment;    // asegúrate de que exista esta columna
                        //    db.Entry(toUpdate).State = EntityState.Modified;
                        //}
                    }


                    // Grabo cambios y vuelco el debug
                    // db.SaveChanges();
                    System.Diagnostics.Debug.WriteLine(string.Join(Environment.NewLine, debugLines));
                }



                // 4. Calcula la lista de líneas de producción utilizadas en el proyecto.
                // para cada material, si tiene línea real se toma esa; si no, se toma la teórica.
                var validLineIds = materialsToUse
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

                var dmStringKeys = dmStatuses
                .ToDictionary(kvp => kvp.Key.ToString(),
                  kvp => kvp.Value);

                // 9. Retornar la nueva estructura en JSON
                return Json(new
                {
                    success = true,
                    data = resultData,
                    dateRanges = fyRanges,
                    dmStatuses = dmStatuses.ToDictionary(k => k.Key.ToString(), k => k.Value),
                    dmStatusComments = dmStatusComments.ToDictionary(k => k.Key.ToString(), k => k.Value),
                    // Nuevas propiedades añadidas a la respuesta:
                    oeeValue = oeeToReturn.HasValue ? Math.Round(oeeToReturn.Value * 100, 2) : (double?)null,
                    oeeIsCalculated = oeeIsCalculated
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private (double oeeValue, bool isCalculated) GetOeeToUse(double? oeeFromForm, int? lineId, Portal_2_0Entities db)
        {
            // Caso A: El OEE fue proporcionado desde el formulario.
            if (oeeFromForm.HasValue)
            {
                var rawOee = oeeFromForm.Value;
                // Normaliza si viene como 1-100 en lugar de 0-1
                double normalizedOee = (rawOee > 1.0) ? rawOee / 100.0 : rawOee;
                return (oeeValue: normalizedOee, isCalculated: false);
            }

            // Caso B: El OEE no fue proporcionado, se debe calcular.
            // Si no hay línea, no se puede calcular, devolvemos un valor por defecto.
            if (!lineId.HasValue || lineId.Value == 0)
            {
                // Devuelve 100% como valor por defecto y marca como calculado.
                return (oeeValue: 1.0, isCalculated: true);
            }

            // Fecha de corte: últimos 6 meses desde hoy.
            var cutoffDate = DateTime.Now.AddMonths(-5).Date;
            int cutoffYear = cutoffDate.Year;
            int cutoffMonth = cutoffDate.Month;

            // Búsqueda en la base de datos de los valores de OEE.
            var oeeValues = db.CTZ_OEE
                              .Where(x =>
                                  x.ID_Line == lineId.Value &&
                                  (x.Year > cutoffYear || (x.Year == cutoffYear && x.Month >= cutoffMonth)))
                              .OrderByDescending(x => x.Year)
                              .ThenByDescending(x => x.Month)
                              .Take(6)
                              .Where(x => x.OEE.HasValue)
                              .Select(x => x.OEE.Value)
                              .ToList();

            if (oeeValues.Any())
            {
                var avg = oeeValues.Average();
                double calculatedOee = (avg > 1.0) ? avg / 100.0 : avg; // Normalizar
                return (oeeValue: calculatedOee, isCalculated: true);
            }
            else
            {
                // Fallback si no se encontraron valores. Se usa 1.0 (100%).
                return (oeeValue: 1.0, isCalculated: true);
            }
        }

        #endregion

        #region Grafica Slitter
        [HttpGet]
        public JsonResult GetSlitterCapacityData(int projectId, int plantId, bool applyDateFilter = false, string whatIfDataJson = null)
        {
            // ========= INICIO DEL CAMBIO ===========
            Debug.WriteLine("\n--- [START] GetSlitterCapacityData ---");
            Debug.WriteLine($"Parámetros: projectId={projectId}, plantId={plantId}, applyDateFilter={applyDateFilter}");

            const int SLITTER_LINE_ID = 8;
            const double TONS_PER_SHIFT = 74.0;
            var slitterRouteIds = new List<int> { 8, 9, 10 };

            // Se cargan todos los años para asegurar que el cálculo de demanda sea completo.
            // El filtrado para la visualización se hará al final.
            var allFiscalYears = db.CTZ_Fiscal_Years.OrderBy(fy => fy.ID_Fiscal_Year).ToList();
            Debug.WriteLine($"Años Fiscales disponibles para cálculo: {allFiscalYears.Count}");


            var totalShiftsByFY = db.CTZ_Total_Time_Per_Fiscal_Year
                .Where(t => t.ID_Plant == plantId)
                .ToDictionary(t => t.ID_Fiscal_Year, t => t.Shifts_SLT);

            try
            {
                // Esta será la lista final de materiales que usaremos para los cálculos.
                var materialsForCalculation = new List<SlitterWhatIfMaterial>();

                if (!string.IsNullOrEmpty(whatIfDataJson))
                {
                    // Escenario "What-If": Los datos vienen del formulario.
                    Debug.WriteLine("\n--- Modo Simulación (What-If) ---");
                    Debug.WriteLine($"JSON recibido del cliente: {whatIfDataJson}");

                    // ========= INICIO DEL CAMBIO ===========
                    // 1. Deserializamos a una lista temporal.
                    var allWhatIfMaterials = JsonConvert.DeserializeObject<List<SlitterWhatIfMaterial>>(whatIfDataJson);
                    Debug.WriteLine($"Datos deserializados: {allWhatIfMaterials.Count} materiales recibidos en total.");


                    // 3. Filtramos la lista para quedarnos solo con los materiales que usan una ruta de Slitter.
                    materialsForCalculation = allWhatIfMaterials
                        .Where(m => m.ID_Route.HasValue && slitterRouteIds.Contains(m.ID_Route.Value))
                        .ToList();

                    Debug.WriteLine($"Materiales filtrados para cálculo de Slitter: {materialsForCalculation.Count}");
                    // ========= FIN DEL CAMBIO ============
                }
                else
                {
                    // ========= INICIO DEL CAMBIO #3: Usar projectId para filtrar los materiales ===========
                    // Escenario Normal: Cargar datos solo del proyecto actual desde la BD.
                    Debug.WriteLine("\n--- Modo Carga Inicial (Desde BD) ---");

                    // PASO A: Ejecutar la consulta en la BD filtrando por el ID del proyecto actual.
                    var materialsFromDb = db.CTZ_Project_Materials
                        .Where(m => m.ID_Project == projectId && // <-- FILTRO CLAVE AÑADIDO
                                    m.ID_Route.HasValue &&
                                    slitterRouteIds.Contains(m.ID_Route.Value))
                        .ToList(); // Materializa la consulta

                    // PASO B: Proyectar a la clase DTO en memoria.
                    materialsForCalculation = materialsFromDb
                       .Select(m => new SlitterWhatIfMaterial
                       {
                           ID_Material = m.ID_Material,
                           IsEdited = false,
                           Vehicle = m.Vehicle,
                           Vehicle_2 = m.Vehicle_2,
                           Vehicle_3 = m.Vehicle_3,
                           Vehicle_4 = m.Vehicle_4,
                           // Se calcula explícitamente el valor usando la fórmula y se valida la división por cero.
                           Initial_Weight = (m.Annual_Volume.HasValue && m.Volume_Per_year.HasValue && m.Annual_Volume != 0)
                               ? (m.Volume_Per_year / m.Annual_Volume * 1000)
                               : (double?)null,
                           ID_Route = m.ID_Route,
                           Real_SOP = m.Real_SOP,
                           Real_EOP = m.Real_EOP,
                           SOP_SP = m.SOP_SP,
                           EOP_SP = m.EOP_SP
                       }).ToList();

                    Debug.WriteLine($"Se encontraron {materialsForCalculation.Count} materiales de Slitter en la BD para el proyecto ID {projectId}.");
                    // ========= FIN DEL CAMBIO #3 =======================================================
                }


                // --- SECCIÓN DE DEPURACIÓN (común para ambos escenarios) ---
                // ========= INICIO DEL CAMBIO ===========
                Debug.WriteLine("----------------------------------------------------------------------------------------------------------------------");
                // 1. Se añaden las columnas "Eff. SOP" y "Eff. EOP" al encabezado.
                Debug.WriteLine(String.Format("{0,-15} | {1,-10} | {2,-15} | {3,-12} | {4,-12} | {5,-15} | {6,-15} | {7,-20}",
                    "Material ID", "Ruta ID", "Peso Inicial", "Eff. SOP", "Eff. EOP", "Vehículo 1", "Vehículo 2", "Escenario de Cálculo"));
                Debug.WriteLine("----------------------------------------------------------------------------------------------------------------------");

                foreach (var mat in materialsForCalculation)
                {
                    string scenario = "DESCONOCIDO";
                    if (!string.IsNullOrEmpty(whatIfDataJson))
                    {
                        if (mat.ID_Material == 0) scenario = "NUEVO (Desde Form)";
                        else if (mat.ID_Material > 0 && mat.IsEdited) scenario = "EDITADO (Desde Form)";
                        else if (mat.ID_Material > 0 && !mat.IsEdited) scenario = "YA GUARDADO (Desde Fila)";
                    }
                    else
                    {
                        scenario = "CARGADO DESDE BD";
                    }

                    // 2. Se determina el SOP y EOP efectivos para este material, priorizando las fechas "Real".
                    DateTime? effectiveSop = mat.Real_SOP ?? mat.SOP_SP;
                    DateTime? effectiveEop = mat.Real_EOP ?? mat.EOP_SP;

                    // 3. Se formatean las fechas para una visualización clara, manejando los casos nulos.
                    string sopString = effectiveSop?.ToString("yyyy-MM-dd") ?? "N/A";
                    string eopString = effectiveEop?.ToString("yyyy-MM-dd") ?? "N/A";

                    // 4. Se añaden las fechas formateadas a la línea de la tabla.
                    Debug.WriteLine(String.Format("{0,-15} | {1,-10} | {2,-15:F3} | {3,-12} | {4,-12} | {5,-15} | {6,-15} | {7,-20}",
                        mat.ID_Material,
                        mat.ID_Route?.ToString() ?? "N/A",
                        mat.Initial_Weight?.ToString() ?? "N/A",
                        sopString, // <-- AÑADIDO
                        eopString, // <-- AÑADIDO
                        mat.Vehicle ?? "N/A",
                        mat.Vehicle_2 ?? "N/A",
                        scenario));
                }
                Debug.WriteLine("----------------------------------------------------------------------------------------------------------------------");

                // ========= INICIO DEL CAMBIO: Lógica para calcular y depurar el rango de fechas ===========
                DateTime? minSop = null;
                DateTime? maxEop = null;
                // Diccionarios para almacenar los resultados del cálculo
                var whatIfShifts = new Dictionary<int, double>();

                // NUEVAS ESTRUCTURAS para almacenar unidades y toneladas juntas
                var monthlyData = new Dictionary<string, (double Units, double Tons, double Shifts)>();
                var fiscalYearData = new Dictionary<string, (double Units, double Tons, double Shifts)>();


                if (materialsForCalculation.Any())
                {
                    // 1. Calcular el rango combinado de fechas PRIMERO
                    minSop = materialsForCalculation.Select(m => m.Real_SOP ?? m.SOP_SP).Where(d => d.HasValue).DefaultIfEmpty(null).Min();
                    maxEop = materialsForCalculation.Select(m => m.Real_EOP ?? m.EOP_SP).Where(d => d.HasValue).DefaultIfEmpty(null).Max();

                    var vehicleCodes = materialsForCalculation.SelectMany(m => new[] { m.Vehicle, m.Vehicle_2, m.Vehicle_3, m.Vehicle_4 }).Where(v => !string.IsNullOrEmpty(v)).Select(v => v.Split('_')[0]).Distinct().ToList();
                    var ihsProductions = db.CTZ_Temp_IHS
                        .Include(i => i.CTZ_Temp_IHS_Production)
                        .AsEnumerable()
                        .Where(i => vehicleCodes.Contains(i.Mnemonic_Vehicle_plant))
                        .ToDictionary(i => i.Mnemonic_Vehicle_plant, i => i.CTZ_Temp_IHS_Production.ToList());

                    foreach (var material in materialsForCalculation)
                    {
                        DateTime? effectiveSop = material.Real_SOP ?? material.SOP_SP;
                        DateTime? effectiveEop = material.Real_EOP ?? material.EOP_SP;

                        if (!effectiveSop.HasValue || !effectiveEop.HasValue) continue;

                        var vehiclesForMaterial = new[] { material.Vehicle, material.Vehicle_2, material.Vehicle_3, material.Vehicle_4 }.Where(v => !string.IsNullOrEmpty(v));
                        foreach (var vehicleCode in vehiclesForMaterial)
                        {
                            string mnemonic = vehicleCode.Split('_')[0];
                            if (ihsProductions.TryGetValue(mnemonic, out var productions))
                            {
                                foreach (var prod in productions)
                                {
                                    var quarterStartDate = new DateTime(prod.Production_Year, prod.Production_Month, 1);
                                    var quarterEndDate = quarterStartDate.AddMonths(3).AddDays(-1);

                                    if (quarterEndDate >= effectiveSop && quarterStartDate <= effectiveEop)
                                    {
                                        var fiscalYear = allFiscalYears.FirstOrDefault(fy => quarterStartDate >= fy.Start_Date && quarterStartDate <= fy.End_Date);
                                        if (fiscalYear != null)
                                        {
                                            // 1. Calcular unidades, toneladas y turnos para el trimestre.
                                            double quarterlyUnits = prod.Production_Amount;
                                            double quarterlyTons = (quarterlyUnits * material.Initial_Weight ?? 0) / 1000;
                                            double quarterlyShifts = (TONS_PER_SHIFT > 0) ? quarterlyTons / TONS_PER_SHIFT : 0;

                                            // 2. Sumar al resumen ANUAL.
                                            if (!fiscalYearData.ContainsKey(fiscalYear.Fiscal_Year_Name))
                                                fiscalYearData[fiscalYear.Fiscal_Year_Name] = (Units: 0, Tons: 0, Shifts: 0);
                                            var fyData = fiscalYearData[fiscalYear.Fiscal_Year_Name];
                                            fiscalYearData[fiscalYear.Fiscal_Year_Name] = (fyData.Units + quarterlyUnits, fyData.Tons + quarterlyTons, fyData.Shifts + quarterlyShifts);

                                            // 3. Distribuir en los meses para la depuración.
                                            double monthlyUnits = quarterlyUnits / 3.0;
                                            double monthlyTons = quarterlyTons / 3.0;
                                            double monthlyShifts = quarterlyShifts / 3.0;
                                            for (int i = 0; i < 3; i++)
                                            {
                                                string monthKey = quarterStartDate.AddMonths(i).ToString("yyyy-MM");
                                                if (!monthlyData.ContainsKey(monthKey))
                                                    monthlyData[monthKey] = (Units: 0, Tons: 0, Shifts: 0);
                                                var monthData = monthlyData[monthKey];
                                                monthlyData[monthKey] = (monthData.Units + monthlyUnits, monthData.Tons + monthlyTons, monthData.Shifts + monthlyShifts);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                // --- SECCIÓN DE DEPURACIÓN ACTUALIZADA CON TABLAS CONSOLIDADAS ---
                Debug.WriteLine("\n--- Desglose de Demanda Mensual (Unidades, Toneladas y Turnos) ---");
                Debug.WriteLine("-----------------------------------------------------------------------------------------------------------");
                // 1. Se añade la columna "Tons / Turno" al encabezado mensual.
                Debug.WriteLine(String.Format("{0,-12} | {1,-15} | {2,-15} | {3,-20} | {4,-15} | {5,-15}",
                    "Mes", "Unidades", "Toneladas", "Peso Inicial Prom. (kg)", "Turnos Req.", "Tons / Turno"));
                Debug.WriteLine("-----------------------------------------------------------------------------------------------------------");
                foreach (var entry in monthlyData.OrderBy(kv => kv.Key))
                {
                    var data = entry.Value;
                    double avgWeight = (data.Units > 0) ? (data.Tons * 1000) / data.Units : 0;

                    // 2. Se añade la constante TONS_PER_SHIFT al final de la línea de datos mensual.
                    Debug.WriteLine(String.Format("{0,-12} | {1,-15:N0} | {2,-15:F2} | {3,-20:F3} | {4,-15:F2} | {5,-15:F2}",
                        entry.Key, data.Units, data.Tons, avgWeight, data.Shifts, TONS_PER_SHIFT));
                }
                Debug.WriteLine("-----------------------------------------------------------------------------------------------------------");

                Debug.WriteLine("\n--- Resumen de Demanda por Año Fiscal (Unidades, Toneladas y Turnos) ---");
                Debug.WriteLine("-----------------------------------------------------------------------------------------------------------------");
                // 3. Se añade la columna "Tons / Turno" al encabezado anual.
                Debug.WriteLine(String.Format("{0,-12} | {1,-18} | {2,-18} | {3,-22} | {4,-18} | {5,-15}",
                    "Año Fiscal", "Total Unidades", "Total Toneladas", "Peso Inicial Prom. (kg)", "Total Turnos Req.", "Tons / Turno"));
                Debug.WriteLine("-----------------------------------------------------------------------------------------------------------------");
                foreach (var entry in fiscalYearData.OrderBy(kv => kv.Key))
                {
                    var data = entry.Value;
                    double avgWeight = (data.Units > 0) ? (data.Tons * 1000) / data.Units : 0;

                    // 4. Se añade la constante TONS_PER_SHIFT al final de la línea de datos anual.
                    Debug.WriteLine(String.Format("{0,-12} | {1,-18:N0} | {2,-18:N2} | {3,-22:F3} | {4,-18:F2} | {5,-15:F2}",
                        entry.Key, data.Units, data.Tons, avgWeight, data.Shifts, TONS_PER_SHIFT));
                }
                Debug.WriteLine("-----------------------------------------------------------------------------------------------------------------");

                // ========= INICIO DEL CAMBIO #2: Filtrar los años fiscales SÓLO para la visualización ===========
                List<CTZ_Fiscal_Years> fiscalYearsForGraph;
                if (applyDateFilter)
                {
                    var today = DateTime.Now;
                    var currentFiscalYear = allFiscalYears.FirstOrDefault(fy => today >= fy.Start_Date && today <= fy.End_Date) ?? allFiscalYears.LastOrDefault();

                    if (currentFiscalYear != null)
                    {
                        int startFyId = currentFiscalYear.ID_Fiscal_Year - 1;
                        var endFiscalYear = maxEop.HasValue
                            ? allFiscalYears.FirstOrDefault(fy => maxEop.Value >= fy.Start_Date && maxEop.Value <= fy.End_Date)
                            : null;
                        int endFyId = Math.Max(currentFiscalYear.ID_Fiscal_Year, endFiscalYear?.ID_Fiscal_Year ?? 0);

                        fiscalYearsForGraph = allFiscalYears
                            .Where(fy => fy.ID_Fiscal_Year >= startFyId && fy.ID_Fiscal_Year <= endFyId)
                            .ToList();
                    }
                    else
                    {
                        fiscalYearsForGraph = new List<CTZ_Fiscal_Years>();
                    }
                }
                else
                {
                    fiscalYearsForGraph = allFiscalYears;
                }

                Debug.WriteLine($"\nAños Fiscales a mostrar en la gráfica: {string.Join(", ", fiscalYearsForGraph.Select(fy => fy.Fiscal_Year_Name))}");

                // ========= FIN DEL CAMBIO #2 ====================================================================

                // --- PASO FINAL: CONSTRUIR ESTRUCTURA DE DATOS PARA LA GRÁFICA ---
                var resultData = new List<object>();
                var lineData = new
                {
                    LineId = SLITTER_LINE_ID,
                    LineName = "Slitter Capacity",
                    DataByFY = new List<object>()
                };

                var currentProjectStatusId = db.CTZ_Projects.Where(p => p.ID_Project == projectId).Select(p => p.ID_Status).FirstOrDefault();
                var orderMapping = new Dictionary<string, int> { { "POH", 1 }, { "Casi Casi", 2 }, { "Carry Over", 3 }, { "Quotes", 4 } };

                // ========= INICIO DEL CAMBIO #3: Usar la lista filtrada 'fiscalYearsForGraph' en el bucle final ===========
                foreach (var fy in fiscalYearsForGraph)
                // ========= FIN DEL CAMBIO #3 ========================================================================
                {
                    var hoursEntries = db.CTZ_Hours_By_Line
                        .Where(h => h.ID_Line == SLITTER_LINE_ID && h.ID_Fiscal_Year == fy.ID_Fiscal_Year)
                        .ToList();

                    // Sumar los turnos calculados del "what-if" (que ahora se llaman 'Shifts' en la tupla)
                    if (fiscalYearData.TryGetValue(fy.Fiscal_Year_Name, out var calculatedData))
                    {
                        var existingEntry = hoursEntries.FirstOrDefault(e => e.ID_Status == currentProjectStatusId);
                        if (existingEntry != null)
                        {
                            existingEntry.Hours += calculatedData.Shifts;
                        }
                        else
                        {
                            hoursEntries.Add(new CTZ_Hours_By_Line
                            {
                                ID_Status = currentProjectStatusId,
                                Hours = calculatedData.Shifts,
                                CTZ_Project_Status = db.CTZ_Project_Status.Find(currentProjectStatusId)
                            });
                        }
                    }

                    double totalAvailableShifts = totalShiftsByFY.ContainsKey(fy.ID_Fiscal_Year) ? totalShiftsByFY[fy.ID_Fiscal_Year].GetValueOrDefault(0) : 0;
                    double totalOccupiedShifts = hoursEntries.Sum(x => x.Hours);

                    var breakdown = hoursEntries
                        .GroupBy(x => x.CTZ_Project_Status.Description)
                        .Select(g => new
                        {
                            StatusId = g.Key,
                            OccupiedHours = g.Sum(x => x.Hours),
                            Percentage = totalAvailableShifts > 0 ? Math.Round((g.Sum(x => x.Hours) / totalAvailableShifts) * 100, 2) : 0
                        })
                        .OrderBy(b => orderMapping.ContainsKey(b.StatusId) ? orderMapping[b.StatusId] : 100)
                        .ToList();

                    lineData.DataByFY.Add(new
                    {
                        FiscalYear = fy.Fiscal_Year_Name,
                        TotalOccupied = totalOccupiedShifts,
                        TotalHours = totalAvailableShifts,
                        Breakdown = breakdown
                    });
                }

                resultData.Add(lineData);

                Debug.WriteLine("\n--- [END] GetSlitterCapacityData ---");
                return Json(new { success = true, data = resultData }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"--- [ERROR] GetSlitterCapacityData: {ex.Message} ---");
                return Json(new { success = false, message = "Error en el Paso 1: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            // ========= FIN DEL CAMBIO ============
        }
        #endregion

        #region Exporta Excel
        [HttpGet]
        public ActionResult ExportProjectToExcel(int id)
        {
            try
            {
                // 1. Cargar el proyecto y todas sus relaciones necesarias.
                var project = db.CTZ_Projects
                    .Include(p => p.CTZ_Project_Status)
                    .Include(p => p.CTZ_Clients)
                    .Include(p => p.CTZ_OEMClients)
                    .Include(p => p.CTZ_plants)
                    .Include(p => p.empleados)
                    .Include(p => p.CTZ_Project_Materials.Select(m => m.CTZ_Material_Type))
                    .Include(p => p.CTZ_Project_Materials.Select(m => m.SCDM_cat_forma_material))
                    .FirstOrDefault(p => p.ID_Project == id);

                if (project == null)
                {
                    TempData["ErrorMessage"] = "Project not found.";
                    return RedirectToAction("Index");
                }

                // 2. Crear el documento Excel en memoria
                using (SLDocument sl = new SLDocument())
                {
                    // --- Definición de Estilos ---
                    #region Styles
                    SLStyle headerStyle = CreateHeaderStyle(sl);
                    SLStyle sectionStyle = CreateSectionStyle(sl);
                    SLStyle labelStyle = CreateLabelStyle(sl);
                    SLStyle valueStyle = CreateValueStyle(sl);
                    SLStyle tableHeaderStyle = CreateTableHeaderStyle(sl);
                    SLStyle verticalSectionStyle = CreateVerticalSectionStyle(sl);
                    SLStyle itemNumberStyle = CreateItemNumberStyle(sl);
                    SLStyle notesHeaderStyle = CreateNotesHeaderStyle(sl);
                    SLStyle notesTextStyle = CreateNotesTextStyle(sl);

                    #endregion

                    // --- Información General (Filas 1-18) ---
                    #region General_Information
                    sl.SetCellValue("B2", "GENERAL QUOTATION");
                    sl.SetCellStyle("B2", headerStyle);
                    //sl.MergeWorksheetCells("B2", "C2");

                    // Datos del proyecto
                    WriteLabelAndValue(sl, 4, 3, "Quote ID", project.ConcatQuoteID, labelStyle, valueStyle);
                    WriteLabelAndValue(sl, 5, 3, "Client", project.CTZ_Clients?.Client_Name ?? project.Cliente_Otro, labelStyle, valueStyle);
                    WriteLabelAndValue(sl, 6, 3, "OEM", project.CTZ_OEMClients?.Client_Name ?? project.OEM_Otro, labelStyle, valueStyle);
                    WriteLabelAndValue(sl, 7, 3, "Facility", project.CTZ_plants?.Description, labelStyle, valueStyle);
                    WriteLabelAndValue(sl, 8, 3, "Created by", project.empleados?.nombre, labelStyle, valueStyle);
                    WriteLabelAndValue(sl, 9, 3, "Status", project.CTZ_Project_Status?.Description, labelStyle, valueStyle);
                    WriteLabelAndValue(sl, 10, 3, "Creation Date", project.Creted_Date.ToString("yyyy-MM-dd"), labelStyle, valueStyle);

                    #endregion

                    // --- Tabla principal de conceptos (Desde Fila 12) ---
                    #region Main_Table
                    int currentRow = 12;
                    int itemCol = 3;      // Item en Col C
                    int conceptCol = 4;   // Concepto en Col D
                    int valueStartCol = 5;// Valores empiezan en Col E
                    int requiredMaterialColumns = 13; // Se define el número mínimo de columnas de material

                    // Encabezados de la tabla
                    sl.SetCellValue(currentRow, 3, "Item");
                    sl.SetCellStyle(currentRow, 3, tableHeaderStyle);
                    sl.SetCellValue(currentRow, 4, "Concept");
                    sl.SetCellStyle(currentRow, 4, tableHeaderStyle);


                    var materials = project.CTZ_Project_Materials.OrderBy(m => m.ID_Material).ToList();
                    //  Se asegura de que se escriban al menos 13 columnas de materiales
                    int totalColumnsToWrite = Math.Max(materials.Count, requiredMaterialColumns);
                    for (int i = 0; i < totalColumnsToWrite; i++)
                    {
                        string headerText = (i < materials.Count)
                            ? (materials[i].Part_Number ?? $"Part {i + 1}")
                            : ""; // Dejar encabezados vacíos para las columnas extra
                        sl.SetCellValue(currentRow, valueStartCol + i, headerText);
                        sl.SetCellStyle(currentRow, valueStartCol + i, tableHeaderStyle);
                    }
                    currentRow++;

                    // Mapeo de conceptos a sus valores
                    var conceptMappings = GetConceptMappings();
                    int conceptCounter = 1;


                    // --- LÓGICA MODIFICADA PARA CREAR SECCIONES VERTICALES ---
                    foreach (var section in conceptMappings)
                    {
                        int startRowForMerge = currentRow;

                        foreach (var concept in section.Value)
                        {
                            sl.SetCellValue(currentRow, itemCol, conceptCounter++);
                            sl.SetCellStyle(currentRow, itemCol, itemNumberStyle);
                            sl.SetCellValue(currentRow, conceptCol, concept.Key);
                            sl.SetCellStyle(currentRow, conceptCol, labelStyle);

                            for (int i = 0; i < totalColumnsToWrite; i++) // <<< CAMBIO: Bucle hasta el total de columnas requerido
                            {
                                string cellValue = "";
                                if (i < materials.Count)
                                {
                                    var material = materials[i];
                                    object value = concept.Value(material);
                                    cellValue = value?.ToString() ?? "";
                                }
                                sl.SetCellValue(currentRow, valueStartCol + i, cellValue);
                                sl.SetCellStyle(currentRow, valueStartCol + i, valueStyle); // Aplicar estilo con bordes
                            }
                            currentRow++;
                        }

                        int endRowForMerge = currentRow - 1;
                        if (startRowForMerge <= endRowForMerge)
                        {
                            sl.SetCellValue(startRowForMerge, 2, section.Key); // Col B para sección vertical
                            sl.MergeWorksheetCells(startRowForMerge, 2, endRowForMerge, 2);
                            // Aplica el estilo A TODO EL RANGO combinado para que el borde se dibuje correctamente
                            sl.SetCellStyle(startRowForMerge, 2, endRowForMerge, 2, verticalSectionStyle);
                        }
                    }
                    #endregion

                    #region Additional_Notes
                    currentRow++; // Dejar una fila en blanco

                    sl.SetCellValue(currentRow, 2, "Additional Notes:");
                    sl.SetCellStyle(currentRow, 2, notesHeaderStyle);
                    currentRow++;

                    var notes = GetAdditionalNotes();
                    foreach (var note in notes)
                    {
                        sl.SetCellValue(currentRow, 2, note.Key); // Número de nota
                        sl.SetCellStyle(currentRow, 2, labelStyle);

                        sl.SetCellValue(currentRow, 3, note.Value); // Texto de la nota
                        sl.MergeWorksheetCells(currentRow, 3, currentRow, valueStartCol + totalColumnsToWrite - 1);
                        sl.SetCellStyle(currentRow, 3, notesTextStyle);

                        currentRow++;
                    }
                    #endregion

                    // Ajustar anchos de columna
                    sl.SetColumnWidth("A", 1); // Sección vertical
                    sl.SetColumnWidth("B", 5); // Sección vertical
                    sl.SetColumnWidth("C", 13.7); // Item
                    //sl.SetColumnWidth("D", 40); // Concepto
                    sl.AutoFitColumn(4);

                    for (int i = 0; i < totalColumnsToWrite; i++)
                    {
                        sl.SetColumnWidth(valueStartCol + i, 18);
                    }


                    // 3. Guardar el libro en un MemoryStream
                    using (MemoryStream ms = new MemoryStream())
                    {
                        sl.SaveAs(ms);
                        ms.Position = 0;
                        string fileName = $"Project_{project.ConcatQuoteID.Replace(" ", "_").Replace("/", "-")}.xlsx";
                        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                TempData["ErrorMessage"] = "An error occurred while exporting the file.";
                return RedirectToAction("Index");
            }
        }

        // --- MÉTODOS DE AYUDA PARA ESTILOS Y ESTRUCTURA ---

        private void WriteLabelAndValue(SLDocument sl, int row, int colIndex, string label, string value, SLStyle labelStyle, SLStyle valueStyle)
        {
            sl.SetCellValue(row, colIndex, label);
            sl.SetCellStyle(row, colIndex, labelStyle);
            sl.SetCellValue(row, colIndex + 1, value);
            sl.SetCellStyle(row, colIndex + 1, valueStyle);
        }

        private Dictionary<string, Dictionary<string, Func<CTZ_Project_Materials, object>>> GetConceptMappings()
        {
            var mappings = new Dictionary<string, Dictionary<string, Func<CTZ_Project_Materials, object>>>
            {
                { "Customer", new Dictionary<string, Func<CTZ_Project_Materials, object>> {
                    { "Customer / End User", m => string.Empty },
                    { "Program", m => string.Empty },
                    { "Customer Part Name", m => string.Empty },
                    { "SOP", m => string.Empty},
                    // ... Agrega aquí el resto de conceptos de la sección "Sales"
                }},
                { "Technical information", new Dictionary<string, Func<CTZ_Project_Materials, object>> {
                    { "Material Specification", m => string.Empty },
                    { "Client Spec", m => string.Empty },
                    { "Coating Weight", m => string.Empty },
                    { "Type", m => string.Empty},
                    { "Mill", m => string.Empty},
                    { "Annual Volume Units", m => string.Empty},
                    { "Annual Volume MT/YR", m => string.Empty},
                    { "Est.Volume (Blanks)", m => string.Empty},
                    { "Thickness (mm)", m => string.Empty},
                    { "Thickness tolerance (mm)", m => string.Empty},
                    { "Width  (mm)", m => string.Empty},
                    { "Pitch  (mm)", m => string.Empty},
                    { "Shape (rectangular, trapeze, special shape)", m => string.Empty},
                    { "Type M", m => string.Empty},
                    { "Gross weight (kg)", m => string.Empty},
                    { "Net weight (kg)", m => string.Empty},
                    { "Offall (Kgs)", m => string.Empty},
                    { "Blanks per Mt", m => string.Empty},            
                    // ... Agrega aquí el resto de conceptos de la sección "Sales"
                }},
                { "Blanking cost", new Dictionary<string, Func<CTZ_Project_Materials, object>> {
                    { "Blanking cost per blank (acc rate: $566 USD/hr)", m => string.Empty},
                    { "Blanking cost per MT (acc rate: $566 USD/hr)", m => string.Empty},
                    { "Blanking cost per Hr ($USD/Hr)", m => string.Empty},
                    { "Blanking cost per MT (acc rate: Min. to Quote) $USD/Mt", m => string.Empty},
                    { "Blanking cost per Blank(acc rate: Min. to Quote) $USD/Blank", m => string.Empty},

                }},
                { "Sales", new Dictionary<string, Func<CTZ_Project_Materials, object>> {
                    { "Part Name", m => m.Part_Name },
                    { "Part Number", m => m.Part_Number },
                    { "Vehicle", m => m.Vehicle != null ? Regex.Replace(m.Vehicle, @"\s+", " ").Trim() : string.Empty },
                    // ... Agrega aquí el resto de conceptos de la sección "Sales"
                }},
                { "Data Management", new Dictionary<string, Func<CTZ_Project_Materials, object>> {
                    { "Thickness (mm)", m => m.Thickness },
                    { "Thickness (-) Tol. (mm)", m => m.ThicknessToleranceNegative },
                    { "Thickness (+) Tol. (mm)", m => m.ThicknessTolerancePositive },
                    { "Width (mm)", m => m.Width },
                    { "Width (-) Tol. (mm)", m => m.WidthToleranceNegative },
                    { "Width (+) Tol. (mm)", m => m.WidthTolerancePositive },
                    { "Pitch (mm)", m => m.Pitch },
                    { "Pitch (-) Tol. (mm)", m => m.PitchToleranceNegative },
                    { "Pitch (+) Tol. (mm)", m => m.PitchTolerancePositive },
                    { "Theoretical Gross Weight (KG)", m => m.Theoretical_Gross_Weight?.ToString("F3") },
                    { "Gross Weight (Client)", m => m.Gross_Weight },
                    { "Annual Volume (Client)", m => m.Annual_Volume },
                    { "Shape", m => m.SCDM_cat_forma_material?.ConcatKey },
                    { "Angle A", m => m.Angle_A },
                    { "Angle B", m => m.Angle_B },
                    // ... Agrega aquí todos los conceptos de la sección "Data Management"
                }},
                // ... Agrega aquí más secciones como "Engineering", "Quality", etc.
            };
            return mappings;
        }

        private List<KeyValuePair<string, string>> GetAdditionalNotes()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("1.", "Quoted price is expressed in #USD/Blank. For invoicing purposes it will be ADDED the Mexican VAT (VAT Tax of 16%). Quotation validity: three months after its submission's date."),
                new KeyValuePair<string, string>("2.", "Standard Payment Term: 30 Days (invoice date). Penalty for no payments received on time, due customer's responsibility will have an additional charge of 0.8% per month or fraction."),
                new KeyValuePair<string, string>("3.", "Delivery Terms:"),
                new KeyValuePair<string, string>("4.", "Indicated tkMM Added Value Price is including:"),
                new KeyValuePair<string, string>("", "- Unloading of master coils received (by truck or rails) in tkMM facility."),
                new KeyValuePair<string, string>("", "- 60 days of free warehousing of the master coils. If additional/subsequent storage is needed, it will have a cost of $3.50 USD/MT for steel, per month or fraction."),
                new KeyValuePair<string, string>("", "- Steel blank's storage package until 7 Days. Additional storage time $11.40 USD/MT for the next 30 days or fraction."),
                new KeyValuePair<string, string>("", "- Processing (blanking)"),
                new KeyValuePair<string, string>("", "- Regular and standard preventive maintenance to blank dies property of customer, in order to assure a Processing service with good quality."),
                new KeyValuePair<string, string>("", "- Standard packaging conditions for blanks, standard tkMM label and plastic strips. non-returnable wooden pallets to be provided by tkMM."),
                new KeyValuePair<string, string>("", "- JIT Delivery until customer facility (if applies), using trucks of single and double platform and considering a minimum load per package of 2,400Kg and per truck of 46 MT as blanks in a double platform and 22MT in a single platform, as average."),
                new KeyValuePair<string, string>("", "This is a very sensitive cost, that it could be increased if the amount per truck (weight) of shipped blanks is reduced significantly as a result of customer's requirement."),
                new KeyValuePair<string, string>("", "- EDI transactions having previous agreement with customer, could be developed for materials manage inventory and/or Goods Shipping, transactions/information provided on a regular bases by tkMM to customer."),
                new KeyValuePair<string, string>("5.", "tkMM Processing (blanking) conditions:"),
                new KeyValuePair<string, string>("", "- for Cut to Length (CTL) blanking services, an oscillating die property of tkMM will be used, for special shape/configurated blanks, a blanking die, property of customer will have to be used."),
                new KeyValuePair<string, string>("", "- tkMM is responsible to provide regular and standard “preventive maintenance” to blank dies property of customers, in order to assure a blanking process with good quality and according acceptance criterias previously established between customer and tkMM. If blank die requires special maintenance due poor quality of components, major design changes, engineering changes, etc., tkMM will prepare and provide a quotation for customer including cost and timing information, and it will have to be reviewed and agreed between tkMM and Customer."),
                new KeyValuePair<string, string>("", "- tkMM requests to have a minimum stock of master coils, representing a coverage of two weeks in comparison with customer's blanks requirements."),
                new KeyValuePair<string, string>("", "- tkMM considers one production/processing run per week, based on customer's weekly release/requirements, provided in advance to tkMM Production Control department."),
                new KeyValuePair<string, string>("", "- Blanking price is based on standard tkMM processing conditions, having a minimum processing speed as 40 strokes/minute. If due blanking die's design or performance characteristics, the processing speed using a blank die property of customer, is less than 40 strokes/minute, then a blanking price increase will have to be considered and agreed, between tkMM and customer. Minimum expected price increase adjustment as 50% of original quoted price."),
                new KeyValuePair<string, string>("", "- In steel blanking process at tkMM, typical values of percentages of Heads and Tails of 1.5% for interior/unexposed parts and 2.5% for exterior/exposed parts are considered for preliminary quotation purposes, however, these values depend on several characteristics related to the material and the blanking process as: blanking die performance, master coil's receipt conditions upon arrival at tkMM facility, master coil's weight (min. master coil weight reference: 15 MT for steel), eviction and stacking conditions, etc. Therefore, tkMM requires a 3 month period of time after starting the mass production and having a representative blanking process of the part in question, to record and measure during 3 months at least (preferably together with the client), the representative conditions of the blanking process, tools and material involved, which allow to determine real weight of the blanks produced and real/representative Heads and Tails percentages."),
                new KeyValuePair<string, string>("7.", "Quarter Scrap revisions/conciliations (if applies) between tkMM and Customer, tkMM will pay to Customer, an scrap/busheils/bale, for steel, based on AMM Monthly Average for Chicago No. 1 Heavy Melt indicator, less 20%, as tkMM scrap administration fee. tkMM will not pay Offal/Engineering scrap or Heads & Tails scrap at steel premium costs."),
                new KeyValuePair<string, string>("8.", "Steel delivered by Customer in tkMM facility. Conditions for steel master coils for processing (blanking):"),
                new KeyValuePair<string, string>("", "a) Min. Weight: 15 MT/Master Coil for steel"),
                new KeyValuePair<string, string>("", "b) Max. Weight: 25 MT/Master Coil for steel."),
                new KeyValuePair<string, string>("", "c) Internal Diameter (ID): 24 in /610 mm for steel"),
                new KeyValuePair<string, string>("", "d) Max. External Diameter (OD): 78.74 in /2,000 mm) for steel"),
            };
        }

        private SLStyle CreateNotesHeaderStyle(SLDocument sl)
        {
            SLStyle style = sl.CreateStyle();
            style.Font.Bold = true;
            style.Font.Underline = UnderlineValues.Single;
            return style;
        }

        private SLStyle CreateNotesTextStyle(SLDocument sl)
        {
            SLStyle style = sl.CreateStyle();
            style.Alignment.WrapText = true;
            style.Alignment.Vertical = VerticalAlignmentValues.Top;
            return style;
        }

        private SLStyle CreateHeaderStyle(SLDocument sl)
        {
            SLStyle style = sl.CreateStyle();
            style.Font.Bold = true;
            style.Font.FontSize = 14;
            style.Font.FontColor = System.Drawing.Color.FromArgb(0, 32, 96); // Azul oscuro
            return style;
        }

        private SLStyle CreateSectionStyle(SLDocument sl)
        {
            SLStyle style = sl.CreateStyle();
            style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.FromArgb(0, 112, 192), System.Drawing.Color.White);
            style.Font.Bold = true;
            style.Font.FontColor = System.Drawing.Color.White;
            return style;
        }

        private SLStyle CreateLabelStyle(SLDocument sl)
        {
            SLStyle style = sl.CreateStyle();
            style.Font.Bold = true;
            style.Alignment.Horizontal = HorizontalAlignmentValues.Left;
            style.Alignment.Indent = 1;
            style.Border.SetTopBorder(BorderStyleValues.Thin, System.Drawing.Color.DarkGray);
            style.Border.SetBottomBorder(BorderStyleValues.Thin, System.Drawing.Color.DarkGray);
            style.Border.SetLeftBorder(BorderStyleValues.Thin, System.Drawing.Color.DarkGray);
            style.Border.SetRightBorder(BorderStyleValues.Thin, System.Drawing.Color.DarkGray);
            return style;
        }

        private SLStyle CreateValueStyle(SLDocument sl)
        {
            SLStyle style = sl.CreateStyle();
            style.Alignment.Horizontal = HorizontalAlignmentValues.Left;
            style.Border.SetTopBorder(BorderStyleValues.Thin, System.Drawing.Color.DarkGray);
            style.Border.SetBottomBorder(BorderStyleValues.Thin, System.Drawing.Color.DarkGray);
            style.Border.SetLeftBorder(BorderStyleValues.Thin, System.Drawing.Color.DarkGray);
            style.Border.SetRightBorder(BorderStyleValues.Thin, System.Drawing.Color.DarkGray);
            return style;
        }

        private SLStyle CreateTableHeaderStyle(SLDocument sl)
        {
            SLStyle style = sl.CreateStyle();
            style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.FromArgb(79, 129, 189), System.Drawing.Color.White);
            style.Font.Bold = true;
            style.Font.FontColor = System.Drawing.Color.White;
            style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            style.Border.BottomBorder.Color = System.Drawing.Color.Black;
            style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            return style;
        }


        private SLStyle CreateVerticalSectionStyle(SLDocument sl)
        {
            SLStyle style = sl.CreateStyle();
            style.Font.Bold = true;
            style.Alignment.TextRotation = 90;
            style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            style.Alignment.Vertical = VerticalAlignmentValues.Center;
            style.Border.SetTopBorder(BorderStyleValues.Thin, System.Drawing.Color.Black);
            style.Border.SetBottomBorder(BorderStyleValues.Thin, System.Drawing.Color.Black);
            style.Border.SetLeftBorder(BorderStyleValues.Thin, System.Drawing.Color.Black);
            style.Border.SetRightBorder(BorderStyleValues.Thin, System.Drawing.Color.Black);
            return style;
        }

        private SLStyle CreateItemNumberStyle(SLDocument sl)
        {
            SLStyle style = sl.CreateStyle();
            style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            style.Border.SetTopBorder(BorderStyleValues.Thin, System.Drawing.Color.DarkGray);
            style.Border.SetBottomBorder(BorderStyleValues.Thin, System.Drawing.Color.DarkGray);
            style.Border.SetLeftBorder(BorderStyleValues.Thin, System.Drawing.Color.DarkGray);
            style.Border.SetRightBorder(BorderStyleValues.Thin, System.Drawing.Color.DarkGray);
            return style;
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

    public class SlittingRuleViewModel
    {
        public string LineName { get; set; }
        public string ThicknessRange { get; set; }
        public string TensileRange { get; set; }
        public string WidthRange { get; set; } // <-- AGREGA ESTA LÍNEA

        public int MaxStrips { get; set; }
    }
    public class CTZ_Slitting_Validation_Rules_DTO
    {
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
    }

    public class RejectionReasonOption
    {
        public int ID_Reason { get; set; }
        public string Name { get; set; }
        public byte? ActionType { get; set; }
        public int? ReassignToDept { get; set; }
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
    public class CountryWithWarningVm
    {
        public int ID_Country { get; set; }
        public string ConcatKey { get; set; }
        public bool Warning { get; set; }
    }
    public class ProjectStatusItem
    {
        public int ID_Project { get; set; }
        public string AssignmentStatus { get; set; }
        public string ProjectStatus { get; set; }
        public string QuoteID { get; set; }
        public string SalesRaw { get; set; }
        public string SalesElapsed { get; set; }
        public string EngineeringRaw { get; set; }
        public string EngineeringElapsed { get; set; }
        public string ForeignTradeRaw { get; set; }
        public string ForeignTradeElapsed { get; set; }
        public string DispositionRaw { get; set; }
        public string DispositionElapsed { get; set; }
        public string DataManagementRaw { get; set; }
        public string DataManagementElapsed { get; set; }
    }
    public class SlitterWhatIfMaterial
    {
        public string Vehicle { get; set; }
        public string Vehicle_2 { get; set; }
        public string Vehicle_3 { get; set; }
        public string Vehicle_4 { get; set; }
        public double? Initial_Weight { get; set; }
        public int? ID_Route { get; set; }
        public int ID_Material { get; set; } // Asegúrate de que este ID se envíe desde el cliente
        public bool IsEdited { get; set; } // Nueva bandera para identificar si el material está siendo editado

        // Propiedades de fecha añadidas
        public DateTime? Real_SOP { get; set; }
        public DateTime? Real_EOP { get; set; }
        public DateTime? SOP_SP { get; set; }
        public DateTime? EOP_SP { get; set; }

    }
    // Helper class para deserializar los datos de las platinas
    public class WeldedPlateInfo
    {
        public int PlateNumber { get; set; }
        public double Thickness { get; set; }
    }
}
