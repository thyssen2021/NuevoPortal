using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Portal_2_0.Models
{
    public class CTZ_ProjectsMetadata
    {

        [Display(Name = "ID Project")]
        public int ID_Project { get; set; }

        [Display(Name = "Status")]
        public int ID_Status { get; set; }

        [Display(Name = "Client")]
        public Nullable<int> ID_Client { get; set; }

        [Display(Name = "OEM/Final Client")]
        public Nullable<int> ID_OEM { get; set; }

        [Display(Name = "Facility")]

        public int ID_Plant { get; set; }

        [Display(Name = "Created By")]

        public int ID_Created_By { get; set; }

        [Display(Name = "Updated By")]
        public Nullable<int> ID_Updated_By { get; set; }

        [Display(Name = "Material Owner")]
        public int ID_Material_Owner { get; set; }

        [StringLength(100)]
        [Display(Name = "Other Client")]
        public string Cliente_Otro { get; set; }

        [StringLength(100)]
        [Display(Name = "Other OEM")]
        public string OEM_Otro { get; set; }

        [StringLength(255)]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Display(Name = "Created Date")]
        public System.DateTime Creted_Date { get; set; }

        [Display(Name = "Updated Date")]
        public Nullable<System.DateTime> Update_Date { get; set; }

        [Display(Name = "Market Type")]
        public Nullable<int> ID_VehicleType { get; set; }

        [Display(Name = "Import Required?")]
        public bool ImportRequired { get; set; }

        [Display(Name = "Import Business Model")]
        public Nullable<int> ID_Import_Business_Model { get; set; }
        [Display(Name = "Incoterm")]
        public Nullable<int> ID_Incoterm { get; set; }
        [Display(Name = "Material Origin")]
        public Nullable<int> ID_Country_Origin { get; set; }
        [Display(Name = "Multipliers")]
        public Nullable<int> Mults { get; set; }
        [Display(Name = "Comments Material Owner")]
        [StringLength(350)]
        public string Comments_Material_Owner { get; set; }
        [Display(Name = "Comments Import")]
        [StringLength(350)]
        public string Comments_Import { get; set; }

        [Display(Name = "Address")]
        [StringLength(120, ErrorMessage = "Address cannot exceed 120 characters.")]
        public string OtherClient_Address { get; set; }
        
        [Display(Name = "Telephone")]
        [StringLength(25, ErrorMessage = "Telephone cannot exceed 25 characters.")]        
        public string OtherClient_Telephone { get; set; }

        [Display(Name = "Address")]
        [StringLength(120, ErrorMessage = "Address cannot exceed 120 characters.")]
        public string OtherOEM_Address { get; set; }

        [Display(Name = "Telephone")]
        [StringLength(25, ErrorMessage = "Telephone cannot exceed 25 characters.")]
        public string OtherOEM_Telephone { get; set; }

        [Display(Name = "Requires Interplant Process?")]
        public bool InterplantProcess { get; set; }

        [Display(Name = "Processor Name")]
        public int? ID_ExternalProcessorName { get; set; }

        [Display(Name = "Buyer Name")]
        [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters.")]
        public string BuyerName { get; set; }

        [Display(Name = "Job Position")]
        [StringLength(100, ErrorMessage = "Position cannot exceed 100 characters.")]
        public string BuyerJobPosition { get; set; }

        [Display(Name = "Telephone")]
        [StringLength(50, ErrorMessage = "Telephone cannot exceed 50 characters.")]
        public string BuyerTelephone { get; set; }

        [Display(Name = "Email")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address format.")]
        public string BuyerEmail { get; set; }
    }

    [MetadataType(typeof(CTZ_ProjectsMetadata))]
    public partial class CTZ_Projects
    {

        //concatena el nombre
        [NotMapped]
        [Display(Name = "Quote ID")]
        public string ConcatQuoteID
        {
            get
            {
                // Obtiene el cliente, priorizando CTZ_Clients, de lo contrario Cliente_Otro, y recorta espacios
                // Obtiene el cliente, priorizando el nombre corto.
                string cliente = !string.IsNullOrWhiteSpace(CTZ_Clients?.ShortName)
                    ? CTZ_Clients.ShortName.Trim()
                    : (CTZ_Clients?.Client_Name?.Trim() ?? Cliente_Otro?.Trim() ?? string.Empty);

                // Obtiene el OEM, priorizando CTZ_OEMClients, de lo contrario OEM_Otro, y recorta espacios
                string oem = !string.IsNullOrWhiteSpace(CTZ_OEMClients?.ShortName)
                   ? CTZ_OEMClients.ShortName.Trim()
                   : (CTZ_OEMClients?.Client_Name?.Trim() ?? OEM_Otro?.Trim() ?? string.Empty);

                /// Si el tipo de vehículo no es automotriz, la lógica del OEM se mantiene.
                if (ID_VehicleType.HasValue && ID_VehicleType.Value != 1)
                {
                    oem = CTZ_Vehicle_Types?.VehicleType_Name?.Trim() ?? string.Empty;
                }

                // Obtiene la descripción de la planta y la clave del owner, recortadas
                string plant = !string.IsNullOrWhiteSpace(CTZ_plants?.ShortName)
                           ? CTZ_plants.ShortName.Trim()
                           : (CTZ_plants?.Description?.Trim() ?? string.Empty);
                string owner = CTZ_Material_Owner?.Owner_Key?.Trim() ?? string.Empty;

                // Concatenar los programas, ignorando nulos o vacíos y recortando espacios
                string concatPrograms = string.Empty;
                if (CTZ_Project_Materials != null && CTZ_Project_Materials.Any())
                {
                    concatPrograms = string.Join("_", CTZ_Project_Materials
                        .Select(x => x.Program_SP?.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Distinct());
                }

                // Construir el resultado sin guión final en caso de no haber programas
                string result = $"TKMM_{plant}_{owner}_{cliente}_{oem}";
                if (!string.IsNullOrEmpty(concatPrograms))
                {
                    result += "_" + concatPrograms;
                }

                result += "_V_" + LastedVersionNumber;

                return result.Replace(" ", "_").ToUpper();
            }
        }

        [NotMapped]
        public bool CanEdit { get; set; }  // indica si el usuario puede ver “Edit”


        [NotMapped]
        public ProjectAssignmentStatus GeneralAssignmentStatus
        {
            get
            {
                // 1) Todas las asignaciones del proyecto (puede venir null)
                var allAssigns = this.CTZ_Project_Assignment ?? Enumerable.Empty<CTZ_Project_Assignment>();

                // 2) Si no hay ninguna → Created
                if (!allAssigns.Any())
                    return ProjectAssignmentStatus.Created;

                // 3) Para cada departamento, quedarnos solo con la última asignación
                var latestPerDept = allAssigns
                    .GroupBy(a => a.ID_Department)
                    .Select(g => g
                        .OrderByDescending(a => a.Assignment_Date)
                        .ThenByDescending(a => a.ID_Assignment)
                        .First())
                    .ToList();

                // 4) Definimos cuáles son los estados terminales
                var terminalStates = new[]
                {
                    (int)AssignmentStatusEnum.APPROVED,
                    (int)AssignmentStatusEnum.REJECTED,
                    (int)AssignmentStatusEnum.ON_HOLD    // ahora lo tratamos como “terminado”
                };

                bool allApproved = latestPerDept.All(a => a.ID_Assignment_Status == (int)AssignmentStatusEnum.APPROVED);
                bool anyApproved = latestPerDept.Any(a => a.ID_Assignment_Status == (int)AssignmentStatusEnum.APPROVED);
                bool allTerminal = latestPerDept.All(a => terminalStates.Contains(a.ID_Assignment_Status));

                // 5) Si todas las últimas están aprobadas → Finalized
                if (allApproved)
                    return ProjectAssignmentStatus.Finalized;

                // 6) Si todas están en un estado terminal pero ninguna aprobada → ClosedWithoutApproval
                if (allTerminal && !anyApproved)
                    return ProjectAssignmentStatus.ClosedWithoutApproval;

                // 7) Si alguna de las últimas está REJECTED → Rejected
                if (latestPerDept.Any(a => a.ID_Assignment_Status == (int)AssignmentStatusEnum.REJECTED))
                    return ProjectAssignmentStatus.Rejected;

                // 8) Si alguna de las últimas está ON_HOLD → OnHold
                if (latestPerDept.Any(a => a.ID_Assignment_Status == (int)AssignmentStatusEnum.ON_HOLD))
                    return ProjectAssignmentStatus.OnHold;

                // 9) Si alguna de las últimas está ON_REVIEWED → OnReview
                if (latestPerDept.Any(a => a.ID_Assignment_Status == (int)AssignmentStatusEnum.ON_REVIEWED))
                    return ProjectAssignmentStatus.OnReview;

                // 10) En cualquier otro caso (PENDING, IN_PROGRESS, etc.) → InProcess
                return ProjectAssignmentStatus.InProcess;
            }
        }



        [NotMapped]
        [Display(Name = "Assignment Status")]

        public string GeneralAssignmentStatusDisplay
        {
            get
            {
                // Convierte e.g. "InProcess" → "In Process"
                var raw = GeneralAssignmentStatus.ToString();
                return Regex.Replace(raw, "(\\B[A-Z])", " $1");
            }
        }

        [NotMapped]
        [Display(Name = "Version")]
        public string LastedVersionNumber
        {
            get
            {
                string lastVersion = "0.1";

                if (this.CTZ_Projects_Versions.Any())
                {
                    lastVersion = this.CTZ_Projects_Versions.OrderByDescending(v => v.ID_Version)
                       .FirstOrDefault().Version_Number;
                }

                return lastVersion;

            }
        }
        public static string GetNextVersionNumber(int projectId)
        {
            CTZ_Projects_Versions latestVersion = new CTZ_Projects_Versions();

            using (var db = new Portal_2_0Entities())
            {
                latestVersion = db.CTZ_Projects_Versions
                    .Where(v => v.ID_Project == projectId)
                    .OrderByDescending(v => v.ID_Version)
                    .FirstOrDefault();
            }


            if (latestVersion == null)
            {
                return "0.1";
            }
            else if (latestVersion.Version_Number == "0.1")
            {
                return "1";
            }
            else
            {
                int ver;
                if (int.TryParse(latestVersion.Version_Number, out ver))
                {
                    return (ver + 1).ToString();
                }
                else
                {
                    // Por si hay algún error en la conversión.
                    return "1";
                }
            }
        }

        /// <summary>
        /// CALCULA EL PORCENTAJE DE CAPACIDAD ÚNICAMENTE PARA LOS MATERIALES DEL ESCENARIO ACTUAL.
        /// Esta versión aplica la lógica de negocio de reemplazar el valor máximo.
        /// </summary>
        /// <param name="materials">La colección de materiales del escenario actual.</param>
        /// <returns>Diccionario [lineId -> [fyId -> % de capacidad del escenario]]</returns>
        public Dictionary<int, Dictionary<int, double>> GetCapacityScenarios(ICollection<CTZ_Project_Materials> materials)
        {
            // Asegurarse de que los materiales no sean nulos
            if (materials == null || !materials.Any())
            {
                return new Dictionary<int, Dictionary<int, double>>();
            }

            // Normalizar datos de entrada
            foreach (var item in materials)
            {
                item.Ideal_Cycle_Time_Per_Tool = item.Ideal_Cycle_Time_Per_Tool ?? item.Theoretical_Strokes ?? 1;
                // La línea efectiva es la Real, si no existe, la Teórica.
                item.ID_Real_Blanking_Line = item.ID_Real_Blanking_Line ?? item.ID_Theoretical_Blanking_Line;
            }

            // Paso 1: Obtener la suma de minutos para el proyecto/escenario actual.
            var projectMinutesByLine = SummarizeCapacityByLineAndFYScenario(materials);
      
            // Paso 2: Aplicar la regla de reemplazar el valor máximo por la suma de RealMin.
            // Esta función ahora también convierte de minutos a horas (dividiendo entre 60).
            var projectHoursByLine = ReplaceMaxValueWithSumOfRealMin(projectMinutesByLine, materials);

            // Paso 3: Cargar el total de horas disponibles por año fiscal.
            using (var db = new Portal_2_0Entities())
            {
                // 1. Crear un lookup para obtener el ID de planta y si es Slitter para cada línea de producción.
                // Esto nos permite saber a qué planta pertenece cada línea y qué tipo de capacidad usar (Horas o Turnos).
                var lineDetailsLookup = db.CTZ_Production_Lines
                    .ToDictionary(
                        l => l.ID_Line,
                        l => new { PlantId = l.ID_Plant, IsSlitter = l.IsSlitter }
                    );

                // 2. Cargar TODAS las horas/turnos disponibles y organizarlos por Planta y luego por Año Fiscal.
                // Usamos un diccionario anidado: Dictionary<ID_Planta, Dictionary<ID_AñoFiscal, Horas/Turnos>>
                var totalTimeByPlantAndFY = db.CTZ_Total_Time_Per_Fiscal_Year
                    .GroupBy(t => t.ID_Plant)
                    .ToDictionary(
                        plantGroup => plantGroup.Key,
                        plantGroup => plantGroup.ToDictionary(
                            fyTime => fyTime.ID_Fiscal_Year,
                            fyTime => new { BlkHours = fyTime.Hours_BLK, SltShifts = fyTime.Shifts_SLT }
                        )
                    );

                // Paso 4: Calcular el porcentaje que representa el proyecto actual.
                var result = new Dictionary<int, Dictionary<int, double>>();

                foreach (var lineKvp in projectHoursByLine)
                {
                    int lineId = lineKvp.Key;
                    result[lineId] = new Dictionary<int, double>();

                    // Obtener los detalles de la línea actual (su planta y si es slitter)
                    if (!lineDetailsLookup.TryGetValue(lineId, out var lineInfo))
                    {
                        // Si la línea no se encuentra en nuestro lookup, la saltamos para evitar errores.
                        continue;
                    }

                    int plantId = lineInfo.PlantId;

                    foreach (var fyKvp in lineKvp.Value)
                    {
                        int fyId = fyKvp.Key;
                        double projectHoursOrShifts = fyKvp.Value;
                        double totalAvailable = 1.0; // Valor por defecto si no se encuentra capacidad

                        // Buscar la capacidad para la planta y el año fiscal correctos
                        if (totalTimeByPlantAndFY.TryGetValue(plantId, out var fyTimes) && fyTimes.TryGetValue(fyId, out var timeInfo))
                        {
                            // Decidir si usar horas de BLK o turnos de SLT
                            if (lineInfo.IsSlitter)
                            {
                                totalAvailable = timeInfo.SltShifts.GetValueOrDefault(1.0);
                            }
                            else
                            {
                                totalAvailable = timeInfo.BlkHours.GetValueOrDefault(1.0);
                            }
                        }

                        if (totalAvailable > 0)
                        {
                            result[lineId][fyId] = projectHoursOrShifts / totalAvailable;
                        }
                        else
                        {
                            result[lineId][fyId] = 0;
                        }
                    }
                }
                return result;
            }
        }

        //obtiene los posibles escenarios para el material indicado
        // 1. Método Principal Orquestador
        public Dictionary<int, Dictionary<int, Dictionary<int, double>>> GetGraphCapacityScenarios(
            ICollection<CTZ_Project_Materials> materials,
            List<string> debugTrace = null) // <--- Parámetro agregado
        {
            if (debugTrace != null) debugTrace.Add("=== INICIO CÁLCULO: GetGraphCapacityScenarios ===");

            // Normalización de datos
            foreach (var item in materials)
            {
                item.Ideal_Cycle_Time_Per_Tool = item.Ideal_Cycle_Time_Per_Tool.HasValue ? item.Ideal_Cycle_Time_Per_Tool.Value : item.Theoretical_Strokes.HasValue ? item.Theoretical_Strokes.Value : 1;
                item.ID_Real_Blanking_Line = item.ID_Real_Blanking_Line.HasValue ? item.ID_Real_Blanking_Line.Value : item.ID_Theoretical_Blanking_Line.HasValue ? item.ID_Theoretical_Blanking_Line.Value : 0;
            }

            // Paso 1: Obtener la suma (Minutos por línea)
            if (debugTrace != null) debugTrace.Add(">> Paso 1: Sumarizar Minutos por Línea y Año Fiscal");
            var SummarizeData = SummarizeCapacityByLineAndFYScenario(materials, debugTrace); // Pasamos el trace

            // Paso 2: Sustituye el valor maximo por la produccion máxima
            var deepCopy = DeepCopySummary(SummarizeData);
            if (debugTrace != null) debugTrace.Add(">> Paso 2: Reemplazar Valor Máximo con Suma de RealMin (Lógica de Negocio)");
            var SummarizaDataWithReplace = ReplaceMaxValueWithSumOfRealMin(deepCopy, materials, debugTrace); // Pasamos el trace

            // Paso 3: Toma los minutos por linea sin sustituir y los divide entre 60 
            deepCopy = DeepCopySummary(SummarizeData); // Nota: Aquí usabas SummarizeData original, no el reemplazado, mantengo tu lógica original.

            // NOTA: ConvertMinutesToHours parece que solo convierte unidades, si quieres trazarlo puedes pasarlo, 
            // pero para no saturar solo lo haré si es crítico. Lo agregaré por consistencia.
            var minutosPorLineaSP = ConvertMinutesToHours(deepCopy, debugTrace);

            // Paso 4: Generar el diccionario de porcentajes por línea, status y FY
            // basado en la versión que sí sustituyó el valor máximo (SummarizaDataWithReplace).
            deepCopy = DeepCopySummary(SummarizaDataWithReplace);

            if (debugTrace != null) debugTrace.Add(">> Paso 4: Calcular Porcentajes Finales vs Capacidad Instalada");

            // Ahora construimos el diccionario final de % usando BuildLineStatusFYPercentage
            var finalPercentageDict = BuildLineStatusFYPercentage(deepCopy, true, debugTrace); // Pasamos el trace

            if (debugTrace != null) debugTrace.Add("=== FIN CÁLCULO: GetGraphCapacityScenarios ===");

            return finalPercentageDict;
        }

        //public Dictionary<int, Dictionary<int, double>> GetCapacityPlusQuote()
        //{
        //    // Paso 1: Obtener la suma
        //    var SummarizeData = SummarizeCapacityByLineAndFY();

        //    // Debug: imprimir la producción base
        //    Debug.WriteLine("=== Minutos por linea ===");
        //    DebugCapacityByLineAndFY(SummarizeData);

        //    //Paso 2: Sustituye el valor maximo por la produccion máxima
        //    // Crear una copia profunda para que el método no modifique SummarizeData
        //    var deepCopy = DeepCopySummary(SummarizeData);

        //    var SummarizaDataWithReplace = ReplaceMaxValueWithSumOfRealMin(deepCopy);
        //    Debug.WriteLine("=== S&P sustituyendo valor máximo / 60 ===");
        //    DebugCapacityByLineAndFY(SummarizaDataWithReplace);

        //    //Paso 3:Toma los minutos por linea sin sustituir y los divide entre 60 
        //    // Crear una copia profunda para que el método no modifique SummarizeData
        //    deepCopy = DeepCopySummary(SummarizeData);
        //    var minutosPorLineaSP = ConvertMinutesToHours(deepCopy);
        //    Debug.WriteLine("=== S&P por Linea Sin sustituir / 60 ===");
        //    DebugCapacityByLineAndFY(minutosPorLineaSP);

        //    // Paso 4: Generar el diccionario de porcentajes por línea, status y FY
        //    //         basado en la versión que sí sustituyó el valor máximo (SummarizaDataWithReplace).
        //    deepCopy = DeepCopySummary(SummarizaDataWithReplace);
        //    // Ahora construimos el diccionario final de % usando BuildLineStatusFYPercentage
        //    var finalPercentageDict = BuildLineStatusFYPercentage(deepCopy);

        //    Debug.WriteLine("=== (4) % de capacidad por línea, status y FY ===");
        //    Debug.WriteLine("=== Suma la capacidad de la cotización al Estatus actual del proyecto ===");
        //    DebugLineStatusFYPercentage(finalPercentageDict);

        //    //Paso 5. Obtiene el porcentaje final de capacidad segun el % del proyecto
        //    // Quotes = Quotes + Carry Over + Casi Casi + POH
        //    // Carry Over = Carry Over + Casi Casi + POH
        //    // Casi Casi = Casi Casi + POH
        //    // PHO = POH

        //    var aggregateCapacity = BuildAggregateByProjectStatus(finalPercentageDict);

        //    Debug.WriteLine("=== (5) Capacidad agregada según Status_Percent del proyecto ===");
        //    Debug.WriteLine("=== Quotes = Quotes + Carry Over + Casi Casi + POH ===");
        //    Debug.WriteLine("=== Carry Over = Carry Over + Casi Casi + POH ===");
        //    Debug.WriteLine("=== Casi Casi = Casi Casi + POH ===");
        //    Debug.WriteLine("=== PHO = POH ===");
        //    DebugCapacityByLineAndFY(aggregateCapacity);

        //    return aggregateCapacity;
        //}

        /// <summary>
        /// A partir del diccionario [lineId -> [statusId -> [fyId -> capacity]]],
        /// genera otro diccionario [lineId -> [fyId -> sum]] en el que solo se incluyen
        /// los estatus cuyo Status_Percent sea >= Status_Percent del proyecto actual.
        /// </summary>
        /// <param name="data">Diccionario de tres niveles con la capacidad por línea, estatus y FY.</param>
        /// <returns>Diccionario de dos niveles [lineId -> [fyId -> capacidadAcumulada]].</returns>
        public Dictionary<int, Dictionary<int, double>> BuildAggregateByProjectStatus(
            Dictionary<int, Dictionary<int, Dictionary<int, double>>> data)
        {
            var result = new Dictionary<int, Dictionary<int, double>>();

            using (var db = new Portal_2_0Entities())
            {
                // 1. Obtener el Status_Percent del proyecto actual
                //    (asumiendo que CTZ_Projects.ID_Status hace referencia a CTZ_Project_Status)
                int projectStatusId = this.ID_Status;
                var projectStatus = db.CTZ_Project_Status.Find(projectStatusId);
                if (projectStatus == null)
                {
                    // Si no hay un estatus válido, devolvemos un diccionario vacío
                    return result;
                }

                // El campo Status_Percent puede ser un string o un tipo numérico.
                // Ajusta según tu modelo de datos. Si es string, conviértelo a double.
                // Si ya es un int o decimal, ajusta la conversión.
                double projectStatusPercent = 0;
                double.TryParse(projectStatus.Status_Percent, out projectStatusPercent);

                // 2. Cargar todos los estatus en un diccionario para conocer su Status_Percent
                //    clave = ID_Status, valor = Status_Percent (en double)
                var allStatuses = db.CTZ_Project_Status
                    .ToDictionary(
                        s => s.ID_Status,
                        s =>
                        {
                            double val = 0;
                            double.TryParse(s.Status_Percent, out val);
                            return val;
                        }
                    );

                // 3. Recorrer cada línea del diccionario de entrada
                foreach (var lineKvp in data)
                {
                    int lineId = lineKvp.Key;

                    // Creamos el diccionario interno para esa línea en el resultado
                    if (!result.ContainsKey(lineId))
                        result[lineId] = new Dictionary<int, double>();

                    // lineKvp.Value => [statusId -> [fyId -> capacity]]
                    // Juntamos todos los fyId que aparecen en cualquiera de los estatus
                    var fyIds = lineKvp.Value.Values
                        .SelectMany(dict => dict.Keys)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();

                    // 4. Para cada año fiscal, sumamos la capacidad de los estatus
                    //    cuyo Status_Percent >= projectStatusPercent
                    foreach (var fyId in fyIds)
                    {
                        double sumCapacity = 0.0;

                        // Recorremos cada statusId
                        foreach (var stId in lineKvp.Value.Keys)
                        {
                            // Verificamos que exista en allStatuses
                            if (!allStatuses.ContainsKey(stId))
                                continue;

                            double stPercent = allStatuses[stId];

                            // Si el estatus cumple con stPercent >= projectStatusPercent
                            if (stPercent >= projectStatusPercent)
                            {
                                // Si existe la capacidad en data[lineId][stId][fyId], la sumamos
                                if (data[lineId][stId].ContainsKey(fyId))
                                {
                                    sumCapacity += data[lineId][stId][fyId];
                                }
                            }
                        }

                        result[lineId][fyId] = sumCapacity;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Genera un diccionario con el % de capacidad por línea de producción, estatus y año fiscal.
        /// Filtra CTZ_Hours_By_Line por las líneas del diccionario 'replacedData', y
        /// para el estatus del proyecto suma replacedData + CTZ_Hours_By_Line,
        /// para los demás estatus usa solo CTZ_Hours_By_Line.
        /// Finalmente divide entre CTZ_Total_Time_Per_Fiscal_Year.Value.
        /// </summary>
        /// <param name="replacedData">
        /// Diccionario [lineId -> [fyId -> horas]] ya modificado (max reemplazado, dividido /60).
        /// </param>
        /// <returns>
        /// Diccionario [lineId -> [statusId -> [fyId -> % capacidad]]].
        /// </returns>
        public Dictionary<int, Dictionary<int, Dictionary<int, double>>> BuildLineStatusFYPercentage(
    Dictionary<int, Dictionary<int, double>> replacedData,
    bool allLines = false,
    List<string> debugTrace = null)
        {
            var result = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>();

            using (var db = new Portal_2_0Entities())
            {
                // --- PREPARACIÓN DE DEBUG (Nombres Amigables) ---
                Dictionary<int, string> lineNamesMap = new Dictionary<int, string>();
                Dictionary<int, string> fyNamesMap = new Dictionary<int, string>();
                Dictionary<int, string> statusNamesMap = new Dictionary<int, string>();

                if (debugTrace != null)
                {
                    debugTrace.Add(">> Paso 4: Cálculo de Porcentaje de Ocupación Final");

                    // Carga ligera de nombres (Líneas, Años y Estatus)
                    lineNamesMap = db.CTZ_Production_Lines.ToDictionary(l => l.ID_Line, l => l.Description);
                    fyNamesMap = db.CTZ_Fiscal_Years.ToDictionary(f => f.ID_Fiscal_Year, f => f.Fiscal_Year_Name);
                    statusNamesMap = db.CTZ_Project_Status.ToDictionary(s => s.ID_Status, s => s.Description);
                }
                // ------------------------------------------------

                var hoursByLine = db.CTZ_Hours_By_Line
                    .GroupBy(x => new { x.ID_Line, x.ID_Status, x.ID_Fiscal_Year })
                    .Select(g => new
                    {
                        g.Key.ID_Line,
                        g.Key.ID_Status,
                        g.Key.ID_Fiscal_Year,
                        TotalHours = g.Sum(x => x.Hours)
                    })
                    .ToList();

                var lineDetailsLookup = db.CTZ_Production_Lines
                    .ToDictionary(
                        l => l.ID_Line,
                        l => new { PlantId = l.ID_Plant, IsSlitter = l.IsSlitter }
                    );

                var totalTimeByPlantAndFY = db.CTZ_Total_Time_Per_Fiscal_Year
                    .GroupBy(t => t.ID_Plant)
                    .ToDictionary(
                        plantGroup => plantGroup.Key,
                        plantGroup => plantGroup.ToDictionary(
                            fyTime => fyTime.ID_Fiscal_Year,
                            fyTime => new { BlkHours = fyTime.Hours_BLK, SltShifts = fyTime.Shifts_SLT }
                        )
                    );

                int projectStatusId = this.ID_Status;

                // Determinamos qué líneas procesar para el resultado final
                List<int> linesToProcess = allLines
                    ? db.CTZ_Production_Lines.Where(l => l.Active).Select(l => l.ID_Line).ToList()
                    : replacedData.Keys.ToList();

                foreach (var lineId in linesToProcess)
                {
                    if (!result.ContainsKey(lineId))
                        result[lineId] = new Dictionary<int, Dictionary<int, double>>();

                    if (!lineDetailsLookup.TryGetValue(lineId, out var lineInfo))
                        continue;

                    int plantId = lineInfo.PlantId;
                    var lineHours = hoursByLine.Where(h => h.ID_Line == lineId).ToList();

                    var distinctStatuses = lineHours.Select(h => h.ID_Status).Distinct().ToList();
                    if (!distinctStatuses.Contains(projectStatusId))
                        distinctStatuses.Add(projectStatusId);

                    foreach (int statusId in distinctStatuses)
                    {
                        if (!result[lineId].ContainsKey(statusId))
                            result[lineId][statusId] = new Dictionary<int, double>();

                        var relevantRows = lineHours.Where(h => h.ID_Status == statusId).ToList();

                        List<int> replacedFYIds = replacedData.ContainsKey(lineId)
                            ? replacedData[lineId].Keys.ToList()
                            : (replacedData.FirstOrDefault().Value?.Keys.ToList() ?? new List<int>());

                        var fyIds = new HashSet<int>(replacedFYIds.Union(relevantRows.Select(r => r.ID_Fiscal_Year)));

                        foreach (int fyId in fyIds)
                        {
                            double hoursLineStatus = relevantRows
                                .Where(r => r.ID_Fiscal_Year == fyId)
                                .Select(r => r.TotalHours)
                                .FirstOrDefault();

                            double totalAvailable = 1.0;

                            if (totalTimeByPlantAndFY.TryGetValue(plantId, out var fyTimes) && fyTimes.TryGetValue(fyId, out var timeInfo))
                            {
                                totalAvailable = lineInfo.IsSlitter
                                    ? timeInfo.SltShifts.GetValueOrDefault(1.0)
                                    : timeInfo.BlkHours.GetValueOrDefault(1.0);
                            }

                            double replacedValue = 0;
                            // Verificamos si esta línea tiene datos del proyecto actual
                            bool hasProjectData = replacedData.ContainsKey(lineId);

                            if (hasProjectData && replacedData[lineId].ContainsKey(fyId))
                                replacedValue = replacedData[lineId][fyId];

                            double finalValue = 0;
                            double numerator = 0;

                            if (totalAvailable > 0)
                            {
                                if (statusId == projectStatusId)
                                {
                                    numerator = (hoursLineStatus + replacedValue);
                                    finalValue = numerator / totalAvailable;

                                    // --- DEBUG TRACE MEJORADO Y FILTRADO ---
                                    // Solo mostramos log si:
                                    // 1. El debug está activo.
                                    // 2. La línea actual es parte de los materiales del proyecto (hasProjectData).
                                    if (debugTrace != null && hasProjectData)
                                    {
                                        string lineName = lineNamesMap.ContainsKey(lineId) ? lineNamesMap[lineId] : $"ID {lineId}";
                                        string fyName = fyNamesMap.ContainsKey(fyId) ? fyNamesMap[fyId] : $"ID {fyId}";
                                        string statusName = statusNamesMap.ContainsKey(statusId) ? statusNamesMap[statusId] : $"Status {statusId}";

                                        debugTrace.Add($"   [{lineName} | {statusName} | {fyName}]");
                                        debugTrace.Add($"     Base BD: {hoursLineStatus:N2}h + Proyecto: {replacedValue:N2}h = {numerator:N2}h");
                                        debugTrace.Add($"     Capacidad Total: {totalAvailable:N2}h");
                                        debugTrace.Add($"     -> Resultado: {numerator:N2} / {totalAvailable:N2} = {finalValue:P2}");
                                    }
                                    // ---------------------------------------
                                }
                                else
                                {
                                    numerator = hoursLineStatus;
                                    finalValue = numerator / totalAvailable;
                                }
                            }

                            result[lineId][statusId][fyId] = finalValue;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Depuración: imprime el diccionario [lineId -> [statusId -> [fyId -> %]]] 
        /// en la ventana de Salida de Visual Studio, en formato ASCII.
        /// </summary>
        public void DebugLineStatusFYPercentage(
            Dictionary<int, Dictionary<int, Dictionary<int, double>>> data)
        {
            Debug.WriteLine($"=== Diccionario % de capacidad para proyecto ID {this.ID_Project} ===");

            using (var db = new Portal_2_0Entities())
            {
                // Diccionarios auxiliares para mostrar nombres
                var lineNames = db.CTZ_Production_Lines
                    .ToDictionary(l => l.ID_Line, l => l.Line_Name);

                var statuses = db.CTZ_Project_Status
                    .ToDictionary(s => s.ID_Status, s => s.Description);

                var fiscalYears = db.CTZ_Fiscal_Years
                    .ToDictionary(f => f.ID_Fiscal_Year, f => f.Fiscal_Year_Name);

                // Ordenar las líneas
                var allLineIds = data.Keys.OrderBy(x => x).ToList();

                foreach (var lineId in allLineIds)
                {
                    string lineName = lineNames.ContainsKey(lineId) ? lineNames[lineId] : $"Line#{lineId}";
                    Debug.WriteLine($"=== Línea: {lineName} (ID {lineId}) ===");

                    // Obtener todos los statusId
                    var allStatusIds = data[lineId].Keys.OrderByDescending(x => x).ToList();

                    // Obtener todos los fyId
                    var allFYIds = data[lineId].Values
                        .SelectMany(dict => dict.Keys)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();

                    // Imprimir encabezado (col 0 = "Status", luego cada FY)
                    int colWidth = 12;
                    string topLine = CreateTableLine(allFYIds.Count + 1, colWidth);
                    Debug.WriteLine(topLine);

                    var headerRow = new List<string> { "Status" };
                    foreach (var fyId in allFYIds)
                    {
                        string fyName = fiscalYears.ContainsKey(fyId)
                            ? fiscalYears[fyId]
                            : $"FY#{fyId}";
                        headerRow.Add(fyName);
                    }
                    Debug.WriteLine(CreateTableRow(headerRow, colWidth));
                    Debug.WriteLine(topLine);

                    // Imprimir filas (una por status)
                    foreach (var stId in allStatusIds)
                    {
                        var rowData = new List<string>();
                        string stName = statuses.ContainsKey(stId)
                            ? statuses[stId]
                            : $"Status#{stId}";
                        rowData.Add(stName);

                        foreach (var fyId in allFYIds)
                        {
                            double val = 0.0;
                            if (data[lineId][stId].ContainsKey(fyId))
                                val = data[lineId][stId][fyId];

                            // Mostrarlo como porcentaje con dos decimales, por ejemplo "45.23%"
                            rowData.Add((val * 100).ToString("F2") + "%");
                        }

                        Debug.WriteLine(CreateTableRow(rowData, colWidth));
                    }

                    Debug.WriteLine(topLine);
                    Debug.WriteLine("");
                }
            }
        }


        /// <summary>
        /// Agrupa la capacidad por línea de producción (ID_Real_Blanking_Line) y año fiscal (ID_Fiscal_Year).
        /// Retorna un diccionario:
        ///   key   = ID de la línea real
        ///   value = otro diccionario [ID_Fiscal_Year -> valor transformado].
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Dictionary<int, double>> SummarizeCapacityByLineAndFY()
        {
            var result = new Dictionary<int, Dictionary<int, double>>();

            // Si no hay materiales, devolvemos el diccionario vacío
            if (this.CTZ_Project_Materials == null) return result;

            // Recorremos cada material del proyecto
            foreach (var material in this.CTZ_Project_Materials)
            {

                // Asegurarnos de que tenga una línea real asignada
                if (!material.ID_Real_Blanking_Line.HasValue)
                    continue;

                int lineId = material.ID_Real_Blanking_Line.Value;

                // Llamamos al método del material que retorna (ID_Fiscal_Year -> valor)
                var capacityByFY = material.GetRealMinutes();

                // Sumamos esos valores en el diccionario principal
                foreach (var kvp in capacityByFY)
                {
                    int fyId = kvp.Key;
                    double capacityValue = kvp.Value;

                    if (!result.ContainsKey(lineId))
                    {
                        result[lineId] = new Dictionary<int, double>();
                    }
                    if (!result[lineId].ContainsKey(fyId))
                    {
                        result[lineId][fyId] = 0;
                    }

                    // Sumamos el valor (por si hay varios materiales en la misma línea y FY)
                    result[lineId][fyId] += capacityValue;
                }
            }

            return result;
        }

        // 2. Método de Sumarización
        public Dictionary<int, Dictionary<int, double>> SummarizeCapacityByLineAndFYScenario(
    ICollection<CTZ_Project_Materials> materials,
    List<string> debugTrace = null)
        {
            // result: Acumulador [ID_Linea -> [ID_AñoFiscal -> Minutos Totales]]
            var result = new Dictionary<int, Dictionary<int, double>>();

            if (materials == null) return result;

            // --- PREPARACIÓN DE DEBUG (NOMBRES AMIGABLES) ---
            Dictionary<int, string> lineNamesMap = new Dictionary<int, string>();
            Dictionary<int, string> fyNamesMap = new Dictionary<int, string>();

            if (debugTrace != null)
            {
                debugTrace.Add("   [INFO] Iniciando sumarización de capacidad (result)...");

                // Carga ligera de nombres para mostrarlos en el log (solo IDs relevantes si es posible, o todos)
                using (var db = new Portal_2_0Entities())
                {
                    lineNamesMap = db.CTZ_Production_Lines.ToDictionary(l => l.ID_Line, l => l.Description);
                    fyNamesMap = db.CTZ_Fiscal_Years.ToDictionary(f => f.ID_Fiscal_Year, f => f.Fiscal_Year_Name);
                }
            }
            // ------------------------------------------------

            // 1. PROCESO DE ACUMULACIÓN (MATERIAL POR MATERIAL)
            foreach (var material in materials)
            {
                // Validación de línea
                if (!material.ID_Real_Blanking_Line.HasValue && !material.ID_Theoretical_Blanking_Line.HasValue)
                    continue;

                int lineId = material.ID_Real_Blanking_Line.HasValue ? material.ID_Real_Blanking_Line.Value : material.ID_Theoretical_Blanking_Line.Value;

                // Obtener minutos de este material (ya incluye el factor si lo aplicaste en el paso anterior)
                var capacityByFY = material.GetRealMinutes(debugTrace);

                foreach (var kvp in capacityByFY)
                {
                    int fyId = kvp.Key;
                    double capacityValue = kvp.Value;

                    // Inicializar diccionarios si no existen
                    if (!result.ContainsKey(lineId)) result[lineId] = new Dictionary<int, double>();
                    if (!result[lineId].ContainsKey(fyId)) result[lineId][fyId] = 0;

                    // ACUMULAR
                    result[lineId][fyId] += capacityValue;

                    // LOG DETALLADO (Paso a paso)
                    if (debugTrace != null)
                    {
                        string lineName = lineNamesMap.ContainsKey(lineId) ? lineNamesMap[lineId] : $"Line {lineId}";
                        string fyName = fyNamesMap.ContainsKey(fyId) ? fyNamesMap[fyId] : $"FY {fyId}";

                        // Muestra cuánto suma este material y cuánto lleva acumulado la línea en ese año
                        debugTrace.Add($"   + Mat {material.ID_Material} ({material.Part_Number}) | {lineName} | {fyName}");
                        debugTrace.Add($"     Sumando: {capacityValue:N2} min -> Nuevo Acumulado: {result[lineId][fyId]:N2} min");
                    }
                }
            }

            // 2. RESUMEN FINAL CONSOLIDADO (¡LO QUE FALTABA!)
            if (debugTrace != null)
            {
                debugTrace.Add("   ============================================================");
                debugTrace.Add("   [RESULT] RESUMEN FINAL DE MINUTOS REQUERIDOS (Variable 'result')");
                debugTrace.Add("   ============================================================");

                if (result.Count == 0)
                {
                    debugTrace.Add("   (Sin resultados acumulados)");
                }
                else
                {
                    foreach (var lineKvp in result)
                    {
                        int lId = lineKvp.Key;
                        string lName = lineNamesMap.ContainsKey(lId) ? lineNamesMap[lId] : $"Line {lId}";

                        debugTrace.Add($"   LÍNEA: {lName}");

                        foreach (var fyKvp in lineKvp.Value.OrderBy(x => x.Key))
                        {
                            int fId = fyKvp.Key;
                            string fName = fyNamesMap.ContainsKey(fId) ? fyNamesMap[fId] : $"FY {fId}";
                            double totalMin = fyKvp.Value;
                            double totalHoras = totalMin / 60.0;

                            // Muestra el total final en minutos y horas
                            debugTrace.Add($"     -> {fName}: {totalMin:N2} min ({totalHoras:N2} horas)");
                        }
                    }
                }
                debugTrace.Add("   ============================================================");
            }

            return result;
        }



        /// <summary>
        /// Para cada línea en 'data', calcula la suma de RealMin de los materiales que tienen ID_Real_Blanking_Line = línea
        /// y reemplaza el valor máximo de esa línea en el diccionario por la suma calculada.
        /// </summary>
        /// <param name="data">Diccionario [ID_Real_Blanking_Line -> [ID_Fiscal_Year -> valor]]</param>
        /// <returns>El mismo diccionario 'data', con los máximos reemplazados.</returns>
        // 3. Método de Reemplazo de Máximos (Lógica Crítica)
        public Dictionary<int, Dictionary<int, double>> ReplaceMaxValueWithSumOfRealMin(
     Dictionary<int, Dictionary<int, double>> data,
     ICollection<CTZ_Project_Materials> materials,
     List<string> debugTrace = null)
        {
            if (data == null) return null;

            // --- PREPARACIÓN DE DEBUG (Nombres Amigables) ---
            Dictionary<int, string> lineNamesMap = new Dictionary<int, string>();
            Dictionary<int, string> fyNamesMap = new Dictionary<int, string>();

            if (debugTrace != null)
            {
                debugTrace.Add(">> Paso 2.1: Reemplazo de Picos y Conversión a Horas");
                debugTrace.Add($"   [FÓRMULA] Strokes per Auto = Parts_per_Auto / BLANKS PER STROKE");
                debugTrace.Add($"   [FÓRMULA] BLANKS PER YEAR [1/1000] = Parts_per_Auto *  ANNUAL_VOLUME");
                debugTrace.Add($"   [FÓRMULA] Min. Max. Reales = BLANKS PER YEAR [1/1000] / IDEAL CYCLE TIME PER TOOL  / BLANKS PER STROKE");
                debugTrace.Add($"   [FÓRMULA] Min. Reales = Min. Max. Reales / OEE");
                debugTrace.Add($"   [FÓRMULA] Turno Reales = Min. Reales / 7.5 / 60");
                debugTrace.Add($"   [FÓRMULA] Strokes per Turno = BLANKS PER YEAR [1/1000] / BLANKS PER STROKE / Turno Reales");


                using (var db = new Portal_2_0Entities())
                {
                    // Cargar nombres de líneas y años fiscales para que el log sea legible
                    var allFyIds = data.Values.SelectMany(d => d.Keys).Distinct().ToList();
                    var allLineIds = data.Keys.ToList();

                    fyNamesMap = db.CTZ_Fiscal_Years
                        .Where(f => allFyIds.Contains(f.ID_Fiscal_Year))
                        .ToDictionary(f => f.ID_Fiscal_Year, f => f.Fiscal_Year_Name);

                    lineNamesMap = db.CTZ_Production_Lines
                        .Where(l => allLineIds.Contains(l.ID_Line))
                        .ToDictionary(l => l.ID_Line, l => l.Description);
                }
            }
            // ------------------------------------------------

            foreach (var lineKvp in data)
            {
                int lineId = lineKvp.Key;
                string lineName = (debugTrace != null && lineNamesMap.ContainsKey(lineId)) ? lineNamesMap[lineId] : $"ID {lineId}";

                // 1. Sumar RealMin de todos los materiales que tengan ID_Real_Blanking_Line = lineId
                double sumRealMin = 0;

                if (materials != null)
                {
                    // Filtramos la lista para usarla tanto en el cálculo como en el debug
                    var materialsOnLine = materials.Where(m => m.ID_Real_Blanking_Line == lineId).ToList();

                    sumRealMin = materialsOnLine.Sum(m => m.RealMin);

                    // --- DEBUG: Desglose de la suma ---
                    if (debugTrace != null)
                    {
                        debugTrace.Add($"   --------------------------------------------------");
                        debugTrace.Add($" ===  LÍNEA: {lineName}");

                        if (materialsOnLine.Any())
                        {
                         //   debugTrace.Add($"   [FÓRMULA] RealMin = (Vol * PartsPerVeh) / (ICT * BlanksPerStroke * OEE) / 7.5 / 60");
                            debugTrace.Add($"   [DETALLE] Suma de 'RealMin' por linea Produccion ({materialsOnLine.Count} materiales):");
                            foreach (var m in materialsOnLine)
                            {
                                debugTrace.Add($"     + Mat {m.ID_Material} ({m.Part_Number}): {m.RealMin:N2} min");
                            }
                            debugTrace.Add($"   = SUMA TOTAL REALMIN: {sumRealMin:N2} min");
                        }
                        else
                        {
                            debugTrace.Add($"   [DETALLE] No hay materiales asignados a esta línea (Suma = 0).");
                        }
                    }
                }

                // 2. Encontrar el valor máximo en 'data[lineId]'
                if (data[lineId].Count == 0)
                    continue;

                var maxKvp = data[lineId].OrderByDescending(k => k.Value).FirstOrDefault();
                int fyWithMaxValue = maxKvp.Key;
                double maxValue = maxKvp.Value;

                // --- DEBUG TRACE: Acción de Reemplazo ---
                if (debugTrace != null)
                {
                    string fyName = fyNamesMap.ContainsKey(fyWithMaxValue) ? fyNamesMap[fyWithMaxValue] : $"FY {fyWithMaxValue}";

                    debugTrace.Add($"   [PICO DETECTADO] Valor máximo actual: {maxValue:N2} min en {fyName}");

                    if (Math.Abs(maxValue - sumRealMin) > 0.01)
                    {
                        debugTrace.Add($"   > ACCIÓN: Reemplazando pico ({maxValue:N2}) por Suma RealMin ({sumRealMin:N2})");
                    }
                    else
                    {
                        debugTrace.Add($"   > ACCIÓN: El pico y la suma son iguales. Se mantiene {sumRealMin:N2}");
                    }
                }

                // 3. Reemplazar el valor máximo con sumRealMin
                data[lineId][fyWithMaxValue] = sumRealMin;
                
                if (debugTrace != null)
                    debugTrace.Add(">> Paso 2.2: Convierte a minutos a horas (Reemplazando Pico)");

                // 4. Dividir todos los valores del diccionario interno entre 60 (convertir minutos a horas)
                // Usamos ToList() para iterar sobre una copia de las claves y poder modificar el diccionario
                foreach (var fy in data[lineId].Keys.ToList())
                {
                    double originalValue = data[lineId][fy];
                    data[lineId][fy] = originalValue / 60.0;

                    // --- DEBUG TRACE: Conversión ---
                    if (debugTrace != null)
                    {
                        string fyName = fyNamesMap.ContainsKey(fy) ? fyNamesMap[fy] : $"FY {fy}";
                        debugTrace.Add($"     -> Conversión {fyName}: {originalValue:N2} min / 60 = {data[lineId][fy]:N2} hrs");
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Convierte los valores de un diccionario [lineId -> [fyId -> valor]] de minutos a horas.
        /// Crea y retorna un nuevo diccionario con los valores divididos entre 60.
        /// </summary>
        /// <param name="data">Diccionario con el resumen por línea y año fiscal.</param>
        /// <returns>Diccionario anidado con los valores transformados a horas.</returns>
        public Dictionary<int, Dictionary<int, double>> ConvertMinutesToHours(
     Dictionary<int, Dictionary<int, double>> data,
     List<string> debugTrace = null)
        {

            debugTrace.Add(">> Paso 3: Convierte de minutos a horas (sin reemplazar)");

            var result = new Dictionary<int, Dictionary<int, double>>();

            foreach (var kvLine in data)
            {
                int lineId = kvLine.Key;
                var fyDictionary = kvLine.Value;

                result[lineId] = new Dictionary<int, double>();

                foreach (var kvFy in fyDictionary)
                {
                    int fyId = kvFy.Key;
                    double minutesValue = kvFy.Value;
                    double hoursValue = minutesValue / 60.0;

                    result[lineId][fyId] = hoursValue;

                    // --- DEBUG TRACE ---
                    if (debugTrace != null) debugTrace.Add($"   [Conv] Línea {lineId} FY {fyId}: {minutesValue:F2} min -> {hoursValue:F2} hrs");
                }
            }

            return result;
        }


        /// <summary>
        /// Depura/imprime el resultado de SummarizeCapacityByLineAndFY() en forma de tabla ASCII,
        /// mostrando una fila por línea de producción y columnas por año fiscal.
        /// </summary>
        /// <param name="data">Diccionario [lineId -> [FY_Id -> valor]]</param>
        public void DebugCapacityByLineAndFY(Dictionary<int, Dictionary<int, double>> data)
        {
            Debug.WriteLine($"=== Resumen de capacidad para el proyecto ID: {this.ID_Project} ===");

            using (var db = new Portal_2_0Entities())
            {
                // 1. Obtener info de líneas: ID_Line -> Nombre de línea
                var linesDict = db.CTZ_Production_Lines
                    .ToDictionary(x => x.ID_Line, x => x.Description);

                // 2. Obtener info de años fiscales: ID_Fiscal_Year -> Nombre
                var fyDict = db.CTZ_Fiscal_Years
                    .ToDictionary(x => x.ID_Fiscal_Year, x => x.Fiscal_Year_Name);

                // 3. Obtener todas las líneas presentes en 'data'
                var allLineIds = data.Keys.OrderBy(x => x).ToList();
                // 4. Obtener todos los FY presentes en 'data'
                var allFYIds = data.Values
                    .SelectMany(dict => dict.Keys)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                // Construimos una tabla ASCII
                int colWidth = 15; // Ajusta el ancho de columna según necesites

                // 3.1. Línea superior
                string topLine = CreateTableLine(allFYIds.Count + 1, colWidth);
                Debug.WriteLine(topLine);

                // 3.2. Fila de encabezados (col 0 = "Line", luego cada FY)
                var headerRow = new List<string> { "Line" };
                foreach (var fyId in allFYIds)
                {
                    string fyName = fyDict.ContainsKey(fyId) ? fyDict[fyId] : $"FY#{fyId}";
                    headerRow.Add(fyName);
                }
                Debug.WriteLine(CreateTableRow(headerRow, colWidth));
                Debug.WriteLine(topLine);

                // 3.3. Filas de datos (una fila por línea)
                foreach (var lineId in allLineIds)
                {
                    var rowData = new List<string>();

                    // Primera columna: nombre de la línea
                    string lineName = linesDict.ContainsKey(lineId) ? linesDict[lineId] : $"Line#{lineId}";
                    rowData.Add(lineName);

                    // Columnas de cada FY
                    foreach (var fyId in allFYIds)
                    {
                        double val = 0;
                        if (data[lineId].ContainsKey(fyId))
                        {
                            val = data[lineId][fyId];
                        }
                        // Redondeamos a 2 decimales
                        rowData.Add(val.ToString("F2"));
                    }

                    Debug.WriteLine(CreateTableRow(rowData, colWidth));
                }

                // 3.4. Línea inferior
                Debug.WriteLine(topLine);
            }
        }

        // Métodos auxiliares para imprimir la tabla ASCII

        private string CreateTableLine(int columnCount, int colWidth)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < columnCount; i++)
            {
                sb.Append("+").Append(new string('-', colWidth));
            }
            sb.Append("+");
            return sb.ToString();
        }

        private string CreateTableRow(List<string> values, int colWidth)
        {
            var sb = new StringBuilder();
            foreach (var val in values)
            {
                sb.Append("|").Append(val.PadLeft(colWidth));
            }
            sb.Append("|");
            return sb.ToString();
        }

        /// <summary>
        /// Crea una copia profunda del diccionario de resumen.
        /// </summary>
        /// <param name="source">Diccionario original [lineId -> [FY_Id -> valor]]</param>
        /// <returns>Nuevo diccionario idéntico al original</returns>
        public Dictionary<int, Dictionary<int, double>> DeepCopySummary(Dictionary<int, Dictionary<int, double>> source)
        {
            var copy = new Dictionary<int, Dictionary<int, double>>();
            foreach (var kvp in source)
            {
                // Crea una nueva instancia para el diccionario interno
                copy[kvp.Key] = new Dictionary<int, double>(kvp.Value);
            }
            return copy;
        }

    }
}