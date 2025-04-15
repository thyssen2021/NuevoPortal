using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        [Display(Name = "Vehicle Type")]
        public Nullable<int> ID_VehicleType { get; set; }

        [Display(Name = "¿Import Required?")]
        public bool ImportRequired { get; set; }
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
                string cliente = (CTZ_Clients?.Client_Name?.Trim() ?? Cliente_Otro?.Trim() ?? string.Empty);

                // Obtiene el OEM, priorizando CTZ_OEMClients, de lo contrario OEM_Otro, y recorta espacios
                string oem = (CTZ_OEMClients?.Client_Name?.Trim() ?? OEM_Otro?.Trim() ?? string.Empty);

                // Si el tipo de vehículo es diferente de Automotriz (ID_VehicleType distinto de 1),
                // se utiliza la descripción del vehículo en lugar del OEM.
                if (ID_VehicleType.HasValue && ID_VehicleType.Value != 1)
                {
                    oem = CTZ_Vehicle_Types?.VehicleType_Name?.Trim() ?? string.Empty;
                }

                // Obtiene la descripción de la planta y la clave del owner, recortadas
                string plant = CTZ_plants?.Description?.Trim() ?? string.Empty;
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

        //obtiene los posibles escenarios para el material indicado
        public Dictionary<int, Dictionary<int, double>> GetCapacityScenarios(ICollection<CTZ_Project_Materials> materials)
        {
            // Paso 1: Obtener la suma (cambiando la linea de producion indicada)
            var SummarizeData = SummarizeCapacityByLineAndFYScenario(materials);
                    

            //Paso 2: Sustituye el valor maximo por la produccion máxima
            // Crear una copia profunda para que el método no modifique SummarizeData
            var deepCopy = DeepCopySummary(SummarizeData);

            var SummarizaDataWithReplace = ReplaceMaxValueWithSumOfRealMin(deepCopy, materials);

          
            //Paso 3:Toma los minutos por linea sin sustituir y los divide entre 60 
            // Crear una copia profunda para que el método no modifique SummarizeData
            deepCopy = DeepCopySummary(SummarizeData);
            var minutosPorLineaSP = ConvertMinutesToHours(deepCopy);      

            // Paso 4: Generar el diccionario de porcentajes por línea, status y FY
            //         basado en la versión que sí sustituyó el valor máximo (SummarizaDataWithReplace).
            deepCopy = DeepCopySummary(SummarizaDataWithReplace);

            //muestra la capacidad agregada de cada linea de produccion
            Debug.WriteLine("===== Capacidad agregada =====");
            DebugCapacityByLineAndFY(minutosPorLineaSP);

            // Ahora construimos el diccionario final de % usando BuildLineStatusFYPercentage
            var finalPercentageDict = BuildLineStatusFYPercentage(deepCopy, allLines: true);
            
            Debug.WriteLine("=== % de capacidad ===");
            Debug.WriteLine("=== (4) % de capacidad por línea, status y FY ===");
            Debug.WriteLine("=== Suma la capacidad de la cotización al Estatus actual del proyecto ===");
            DebugLineStatusFYPercentage(finalPercentageDict);


            //suma segun los estatus del proyecto
            var aggregateCapacity = BuildAggregateByProjectStatus(finalPercentageDict);

            //Debug.WriteLine("=== Escenario para blk: " + blkID + " ===");
            //DebugCapacityByLineAndFY(aggregateCapacity);

            return aggregateCapacity;
        }
        //obtiene los posibles escenarios para el material indicado
        public Dictionary<int, Dictionary<int, Dictionary<int, double>>> GetGraphCapacityScenarios(ICollection<CTZ_Project_Materials> materials)
        {
            // Paso 1: Obtener la suma (cambiando la linea de producion indicada)
            var SummarizeData = SummarizeCapacityByLineAndFYScenario(materials);
                    

            //Paso 2: Sustituye el valor maximo por la produccion máxima
            // Crear una copia profunda para que el método no modifique SummarizeData
            var deepCopy = DeepCopySummary(SummarizeData);

            var SummarizaDataWithReplace = ReplaceMaxValueWithSumOfRealMin(deepCopy, materials);

          
            //Paso 3:Toma los minutos por linea sin sustituir y los divide entre 60 
            // Crear una copia profunda para que el método no modifique SummarizeData
            deepCopy = DeepCopySummary(SummarizeData);
            var minutosPorLineaSP = ConvertMinutesToHours(deepCopy);      

            // Paso 4: Generar el diccionario de porcentajes por línea, status y FY
            //         basado en la versión que sí sustituyó el valor máximo (SummarizaDataWithReplace).
            deepCopy = DeepCopySummary(SummarizaDataWithReplace);

            //muestra la capacidad agregada de cada linea de produccion
            Debug.WriteLine("===== Capacidad agregada =====");
            DebugCapacityByLineAndFY(minutosPorLineaSP);

            // Ahora construimos el diccionario final de % usando BuildLineStatusFYPercentage
            var finalPercentageDict = BuildLineStatusFYPercentage(deepCopy, allLines: true);
            
            Debug.WriteLine("=== % de capacidad ===");
            Debug.WriteLine("=== (4) % de capacidad por línea, status y FY ===");
            Debug.WriteLine("=== Suma la capacidad de la cotización al Estatus actual del proyecto ===");
            DebugLineStatusFYPercentage(finalPercentageDict);

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
        public Dictionary<int, Dictionary<int, Dictionary<int, double>>>
            BuildLineStatusFYPercentage(Dictionary<int, Dictionary<int, double>> replacedData, bool allLines = false)
        {
            // Diccionario resultado:
            //   lineId -> (statusId -> (fyId -> porcentaje))
            var result = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>();

            using (var db = new Portal_2_0Entities())
            {
                // 1. Cargar en memoria las horas por línea, estatus y año fiscal
                //    y agrupar por (ID_Line, ID_Status, ID_Fiscal_Year).
                var hoursByLine = db.CTZ_Hours_By_Line
                    .GroupBy(x => new { x.ID_Line, x.ID_Status, x.ID_Fiscal_Year })
                    .Select(g => new
                    {
                        g.Key.ID_Line,
                        g.Key.ID_Status,
                        g.Key.ID_Fiscal_Year,
                        TotalHours = g.Sum(x => x.Hours)  // si hubiese varias filas, se suman
                    })
                    .ToList();

                // 2. Cargar el total de horas disponibles por año fiscal en un diccionario
                var totalTimeByFY = db.CTZ_Total_Time_Per_Fiscal_Year
                    .ToDictionary(x => x.ID_Fiscal_Year, x => x.Value);

                // 3. Identificar el ID_Status del proyecto actual 
                int projectStatusId = this.ID_Status;

                // 4. Determinar las líneas a procesar.
                //    Si allLines es true, se obtienen todas las líneas activas; de lo contrario, se utilizan las claves de replacedData.
                List<int> linesToProcess = new List<int>();
                if (allLines)
                {
                    linesToProcess = db.CTZ_Production_Lines
                        .Where(l => l.Active)
                        .Select(l => l.ID_Line)
                        .ToList();
                }
                else
                {
                    linesToProcess = replacedData.Keys.ToList();
                }

                // 4.1 Iterar sobre cada línea presente en 'linesToProcess'
                // 5. Iterar sobre cada línea a procesar.
                foreach (var lineId in linesToProcess)
                {
                    // Filtrar los registros de CTZ_Hours_By_Line para la línea actual.
                    var lineHours = hoursByLine.Where(h => h.ID_Line == lineId).ToList();

                    // Si no se encontró información de horas para esta línea, se agrega una entrada vacía y se continúa.
                    if (!lineHours.Any())
                    {
                        result[lineId] = new Dictionary<int, Dictionary<int, double>>();
                        continue;
                    }

                    // Crear la entrada para la línea en el diccionario resultado.
                    if (!result.ContainsKey(lineId))
                        result[lineId] = new Dictionary<int, Dictionary<int, double>>();

                    // Obtener las claves de FY del replacedData para esta línea (si existen).
                    List<int> replacedFYIds = new List<int>();
                    if (replacedData.ContainsKey(lineId))
                        replacedFYIds = replacedData[lineId].Keys.ToList();
                    else {
                        // Si no existe para la línea actual, usamos los FY del primer registro de replacedData.
                        var firstRecord = replacedData.First().Value;
                        replacedFYIds = firstRecord?.Keys?.ToList() ?? new List<int>();
                    }


                    // 6. Obtener la lista de estatus distintos que se encuentran en los registros de horas para esta línea.
                    var distinctStatuses = lineHours.Select(h => h.ID_Status).Distinct().ToList();

                    // 7. Para cada estatus, calcular el porcentaje de capacidad para cada año fiscal.
                    foreach (int statusId in distinctStatuses)
                    {
                        // Crear la entrada interna para el estatus si aún no existe.
                        if (!result[lineId].ContainsKey(statusId))
                            result[lineId][statusId] = new Dictionary<int, double>();

                        // Filtrar los registros relevantes para (lineId, statusId).
                        var relevantRows = lineHours.Where(h => h.ID_Status == statusId).ToList();

                        // 7.1. Determinar los FYs a procesar para esta línea.
                        //       Se unen los FYs que provienen de replacedData (si existen) y los que aparecen en los registros filtrados.
                        var fyIds = new HashSet<int>(replacedFYIds.Union(relevantRows.Select(r => r.ID_Fiscal_Year)));

                        // 7.2. Para cada año fiscal, calcular el porcentaje de capacidad.
                        foreach (int fyId in fyIds)
                        {
                            // Obtener el total de horas para la combinación (lineId, statusId, fyId) de los registros de CTZ_Hours_By_Line.
                            double hoursLineStatus = relevantRows
                                .Where(r => r.ID_Fiscal_Year == fyId)
                                .Select(r => r.TotalHours)
                                .FirstOrDefault();

                            // Obtener el total de horas disponibles para el FY.
                            double totalFY = totalTimeByFY.ContainsKey(fyId) ? totalTimeByFY[fyId] : 1.0;

                            // Obtener el valor de replacedData para el FY, si existe; de lo contrario, se asume 0.
                            double replacedValue = 0;
                            if (replacedData.ContainsKey(lineId) && replacedData[lineId].ContainsKey(fyId))
                                replacedValue = replacedData[lineId][fyId];

                            double finalValue = 0;

                            // Si el estatus es el del proyecto actual, se suman las horas de la línea y el replacedValue.
                            if (statusId == projectStatusId)
                            {
                                double sum = hoursLineStatus + replacedValue;
                                finalValue = sum / totalFY;
                            }
                            else
                            {
                                // Para otros estatus, solo se toma el valor de la línea.
                                finalValue = hoursLineStatus / totalFY;
                            }

                            // Guardar el resultado en el diccionario.
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

        public Dictionary<int, Dictionary<int, double>> SummarizeCapacityByLineAndFYScenario(ICollection<CTZ_Project_Materials> materials)
        {
            var result = new Dictionary<int, Dictionary<int, double>>();

            // Si no hay materiales, devolvemos el diccionario vacío
            if (materials == null) return result;

            // Recorremos cada material del proyecto
            foreach (var material in materials)
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



        /// <summary>
        /// Para cada línea en 'data', calcula la suma de RealMin de los materiales que tienen ID_Real_Blanking_Line = línea
        /// y reemplaza el valor máximo de esa línea en el diccionario por la suma calculada.
        /// </summary>
        /// <param name="data">Diccionario [ID_Real_Blanking_Line -> [ID_Fiscal_Year -> valor]]</param>
        /// <returns>El mismo diccionario 'data', con los máximos reemplazados.</returns>
        public Dictionary<int, Dictionary<int, double>> ReplaceMaxValueWithSumOfRealMin(
            Dictionary<int, Dictionary<int, double>> data, ICollection<CTZ_Project_Materials> materials)
        {
            if (data == null) return null;

            // Recorremos cada línea de producción que exista en el diccionario
            foreach (var lineKvp in data)
            {
                int lineId = lineKvp.Key;

                // 1. Sumar RealMin de todos los materiales que tengan ID_Real_Blanking_Line = lineId
                //    (si no hay materiales o no tienen esa línea, la suma será 0)
                double sumRealMin = 0;
                if (materials != null)
                {
                    sumRealMin = materials
                        .Where(m => m.ID_Real_Blanking_Line == lineId)
                        .Sum(m => m.RealMin);
                }

                // 2. Encontrar el valor máximo en 'data[lineId]'
                //    Si el diccionario interno está vacío, continuamos a la siguiente línea
                if (data[lineId].Count == 0)
                    continue;

                // Tomamos el par (FY, Valor) con mayor 'Valor'
                var maxKvp = data[lineId].OrderByDescending(k => k.Value).FirstOrDefault();
                int fyWithMaxValue = maxKvp.Key;
                double maxValue = maxKvp.Value;

                // 3. Reemplazar el valor máximo con sumRealMin
                //    Si sumRealMin es 0, simplemente se sobreescribirá con 0, 
                //    o con el valor calculado si es > 0.
                data[lineId][fyWithMaxValue] = sumRealMin;

                //Debug.WriteLine($"Line {lineId}: Max value {maxValue} (FY-ID {fyWithMaxValue}) reemplazado por {sumRealMin}.");

                // 3. Dividir todos los valores del diccionario interno entre 60 (convertir minutos a horas)
                foreach (var fy in data[lineId].Keys.ToList())
                {
                    double originalValue = data[lineId][fy];
                    data[lineId][fy] = originalValue / 60.0;
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
            Dictionary<int, Dictionary<int, double>> data)
        {
            var result = new Dictionary<int, Dictionary<int, double>>();

            // Recorremos cada línea
            foreach (var kvLine in data)
            {
                int lineId = kvLine.Key;
                var fyDictionary = kvLine.Value;

                // Creamos un diccionario anidado para la línea actual
                result[lineId] = new Dictionary<int, double>();

                // Recorremos cada año fiscal y convertimos su valor
                foreach (var kvFy in fyDictionary)
                {
                    int fyId = kvFy.Key;
                    double minutesValue = kvFy.Value;
                    double hoursValue = minutesValue / 60.0; // Conversión a horas

                    // Asignamos al nuevo diccionario
                    result[lineId][fyId] = hoursValue;
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