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
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2013.WebExtension;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Portal_2_0.Models;
using SpreadsheetLight;
using static QRCoder.PayloadGenerator;

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
            ViewBag.ID_OEM = new SelectList(db.CTZ_OEMClients, "ID_OEM", "Client_Name");
            ViewBag.ID_Plant = new SelectList(db.CTZ_plants, "ID_Plant", "Description");
            ViewBag.ID_VehicleType = new SelectList(db.CTZ_Vehicle_Types, "ID_VehicleType", "VehicleType_Name");
            ViewBag.ID_Status = new SelectList(db.CTZ_Project_Status, nameof(CTZ_Project_Status.ID_Status), nameof(CTZ_Project_Status.ConcatStatus));
            ViewBag.ID_Import_Business_Model = new SelectList(db.CTZ_Import_Business_Model, nameof(CTZ_Import_Business_Model.ID_Model), nameof(CTZ_Import_Business_Model.Description));

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
                    CTZ_Route = h.CTZ_Route

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
        public ActionResult EditClientPartInformation(int id)
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
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
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
                      g => {
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
                    dateRanges = fyRanges,               // ahora mapea lineId → { MinFY, MaxFY }
                    dmStatuses = dmStatuses.ToDictionary(k => k.Key.ToString(), k => k.Value),
                    dmStatusComments = dmStatusComments.ToDictionary(k => k.Key.ToString(), k => k.Value)
                }, JsonRequestBehavior.AllowGet);
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
}
