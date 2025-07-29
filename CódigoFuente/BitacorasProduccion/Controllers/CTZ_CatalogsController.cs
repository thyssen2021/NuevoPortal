using Clases.Util;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using Newtonsoft.Json;
using Portal_2_0.Models;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class CTZ_CatalogsController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: CTZ_Catalogs
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
                return View("../Home/ErrorPermisos");

            int me = obtieneEmpleadoLogeado().id;
            var auth = new AuthorizationService(db);

            // Contexto necesario para “AssignedToProject”
            //var context = new Dictionary<string, object> { ["ProjectId"] = id };
            ViewBag.CanViewManagePermissions = auth.CanPerform(me, ResourceKey.ManagePermissions, ActionKey.View /*, context*/);
            ViewBag.CanEditManagePermissions = auth.CanPerform(me, ResourceKey.ManagePermissions, ActionKey.Edit /*, context*/);
            ViewBag.CanViewForeignTrade = auth.CanPerform(me, ResourceKey.CatalogsForeignTrade, ActionKey.View /*, context*/);
            ViewBag.CanEditForeignTrade = auth.CanPerform(me, ResourceKey.CatalogsForeignTrade, ActionKey.Edit /*, context*/);
            ViewBag.CanViewDataManagement = auth.CanPerform(me, ResourceKey.CatalogsDataManagement, ActionKey.View /*, context*/);
            ViewBag.CanEditDataManagement = auth.CanPerform(me, ResourceKey.CatalogsDataManagement, ActionKey.Edit /*, context*/);
            ViewBag.CanViewCatalogsEngineering = auth.CanPerform(me, ResourceKey.CatalogsEngineering, ActionKey.View /*, context*/);
            ViewBag.CanEditCatalogsEngineering = auth.CanPerform(me, ResourceKey.CatalogsEngineering, ActionKey.Edit /*, context*/);

            return View();
        }

        [Authorize]
        public ActionResult strokes_per_minute()
        {
            // Verificar rol
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
                return View("../Home/ErrorPermisos");

            // Cargar fabricantes activos
            var manufacturers = db.CTZ_Line_Manufacturer
                                  .Where(m => m.Active)
                                  .OrderBy(m => m.Manufacter_Name)
                                  .ToList();

            // Enviar la lista al ViewBag (o ViewModel) para poblar el select
            ViewBag.Manufacturers = new SelectList(manufacturers, "ID_Manufacturer", "Manufacter_Name");

            return View();
        }

        [HttpGet]
        public JsonResult LoadStrokesData(int manufacturerId)
        {
            try
            {
                // 1. Obtener todos los registros
                var settings = db.CTZ_Line_Stroke_Settings
                                 .Where(s => s.ID_Machine_Manufacturer == manufacturerId)
                                 .ToList();

                // 2. Obtener lista de giros y avances (ordenados)
                var allRotations = settings.Select(s => s.Rotation_degrees).Distinct().OrderBy(x => x).ToList();
                var allAdvances = settings.Select(s => s.Advance_mm).Distinct().OrderBy(x => x).ToList();

                // 3. Crear la matriz
                int rows = allRotations.Count + 1;  // +1 para la fila de encabezados
                int cols = allAdvances.Count + 1;   // +1 para la columna de giros
                var matrix = new object[rows][];

                for (int r = 0; r < rows; r++)
                {
                    matrix[r] = new object[cols];
                }

                // 4. Llenar la primera fila (encabezados de avance)
                //    matrix[0][0] = "Giro" (texto para la esquina)
                matrix[0][0] = "Giro";
                for (int c = 1; c < cols; c++)
                {
                    matrix[0][c] = allAdvances[c - 1];  // Avances en la primera fila
                }

                // 5. Llenar la primera columna (giros)
                for (int r = 1; r < rows; r++)
                {
                    matrix[r][0] = allRotations[r - 1]; // Giros en la primera columna
                }

                // 6. Llenar las celdas de Strokes
                foreach (var rot in allRotations)
                {
                    int rowIndex = allRotations.IndexOf(rot) + 1;
                    foreach (var adv in allAdvances)
                    {
                        int colIndex = allAdvances.IndexOf(adv) + 1;
                        var setting = settings.FirstOrDefault(s => s.Rotation_degrees == rot && s.Advance_mm == adv);
                        if (setting != null)
                        {
                            // Ponemos el valor de Strokes
                            matrix[rowIndex][colIndex] = setting.Strokes;
                        }
                        else
                        {
                            // Si no hay registro, null o 0
                            matrix[rowIndex][colIndex] = null;
                        }
                    }
                }

                // 7. Retornar la matriz en JSON
                return Json(new { success = true, data = matrix }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult engineering_dimension()
        {
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
                return View("../Home/ErrorPermisos");

            // Cargamos las plantas activas
            var plants = db.CTZ_plants
                           .Where(p => p.Active)
                           .OrderBy(p => p.Description)
                           .ToList();

            ViewBag.Plants = new SelectList(plants, "ID_Plant", "Description");


            // --- INICIO DEL CÓDIGO NUEVO QUE DEBES AGREGAR ---

            // Cargar listas para los dropdowns de la tabla de Reglas Teóricas
            // Usamos SelectListItem para tener Value (ID) y Text (Nombre)

            // Lista de Plantas para la tabla Handsontable
            ViewBag.PlantsForGrid = db.CTZ_plants
                .Where(p => p.Active)
                .Select(p => new { Value = p.ID_Plant, Text = p.Description })
                .ToList();

            // Lista de Tipos de Material para la tabla Handsontable
            ViewBag.MaterialTypesForGrid = db.CTZ_Material_Type
                .Where(mt => mt.Active)
                .Select(mt => new { Value = mt.ID_Material_Type, Text = mt.Material_Name })
                .ToList();

            // Lista de TODAS las Líneas de Producción para la tabla Handsontable
            ViewBag.AllLinesForGrid = db.CTZ_Production_Lines
               .Where(l => l.Active)
               .Select(l => new {
                   Value = l.ID_Line,
                   Text = l.Description,
                   PlantId = l.ID_Plant // <-- AÑADE ESTA LÍNEA
               })
       .ToList();

            // Creamos un mapa de las relaciones Línea -> Tipo de Material
            ViewBag.LineMaterialTypeMap = db.CTZ_Material_Type_Lines
                .Select(mtl => new { mtl.ID_Line, mtl.ID_Material_Type })
                .ToList();

            // --- FIN DEL CÓDIGO NUEVO ---

            return View();
        }

        /// <summary>
        /// Retorna las líneas activas de la planta indicada en formato JSON.
        /// </summary>
        [HttpGet]
        public JsonResult GetLinesByPlant(int plantId)
        {
            var lines = db.CTZ_Production_Lines
                          .Where(l => l.Active && l.ID_Plant == plantId)
                          .OrderBy(l => l.Line_Name)
                          .Select(l => new
                          {
                              ID_Line = l.ID_Line,
                              Line_Name = l.Line_Name
                          })
                          .ToList();

            return Json(lines, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Carga los registros de CTZ_Engineering_Dimension (unidos a Criteria y Lines),
        /// filtrados por la planta y opcionalmente la línea. Retorna JSON.
        /// Incluye ID_Engineering_Dimension para poder actualizar luego.
        /// </summary>
        [HttpGet]
        public JsonResult LoadEngineeringDimension(int plantId, int? lineId)
        {
            var query = from d in db.CTZ_Engineering_Dimension
                        join c in db.CTZ_Engineering_Criteria on d.ID_Criteria equals c.ID_Criteria
                        join l in db.CTZ_Production_Lines on d.ID_Line equals l.ID_Line
                        join p in db.CTZ_plants on l.ID_Plant equals p.ID_Plant
                        where p.ID_Plant == plantId
                              && p.Active
                              && l.Active
                              && d.Active
                              && c.Active
                        select new
                        {
                            d.ID_Engineering_Dimension,
                            ID_Line = l.ID_Line,
                            Plant = p.Description,
                            Line = l.Line_Name,
                            Criteria = c.CriteriaName,
                            MinValue = d.Min_Value,
                            MaxValue = d.Max_Value
                        };

            if (lineId.HasValue && lineId.Value != 0)
            {
                query = query.Where(x => x.ID_Line == lineId.Value);
            }

            var data = query.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Recibe los datos editados desde Handsontable y actualiza la tabla
        /// CTZ_Engineering_Dimension en la BD.
        /// </summary>
        [HttpPost]
        public JsonResult SaveEngineeringDimension(List<EngineeringDimensionDTO> data)
        {
            // Validación básica
            if (data == null || data.Count == 0)
            {
                return Json(new { success = false, message = "No data received." });
            }

            try
            {
                foreach (var item in data)
                {
                    // Buscamos el registro original
                    var dimension = db.CTZ_Engineering_Dimension
                                      .FirstOrDefault(d => d.ID_Engineering_Dimension == item.ID_Engineering_Dimension);

                    if (dimension != null)
                    {
                        // Actualizamos sólo Min y Max (o lo que necesites)
                        dimension.Min_Value = item.MinValue;
                        dimension.Max_Value = item.MaxValue;

                    }
                }

                db.SaveChanges();

                return Json(new { success = true, message = "Data saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===================================
        // MÉTODO PARA MOSTRAR LA VISTA
        // ===================================
        [Authorize]
        public ActionResult hours_by_line()
        {
            int me = obtieneEmpleadoLogeado().id;
            var auth = new AuthorizationService(db);
            bool canEditDataManagement = auth.CanPerform(me, ResourceKey.CatalogsDataManagement, ActionKey.Edit /*, context*/);

            // Verificar rol
            if (!TieneRol(TipoRoles.CTZ_ACCESO) || !canEditDataManagement)
                return View("../Home/ErrorPermisos");

            // 1. Generar la lista de periodos de 10 años fiscales (ejemplo: [ "FY 10/11 - FY 19/20", "FY 20/21 - FY 29/30", ... ])
            //    Necesitarás tener ya cargados tus años fiscales en la tabla CTZ_Fiscal_Years.
            //    Para simplicidad, asumimos que ya existen los registros en la BD.
            var fiscalYears = db.CTZ_Fiscal_Years
                                .OrderBy(f => f.ID_Fiscal_Year)
                                .ToList();

            // Agrupa de 10 en 10 (puedes ajustar la lógica según necesites).
            // Suponiendo que tienes un total de 40 o 50 años fiscales, agrupa de 10 en 10.
            // Un modo es iterar de 0 en 10 en 10.
            var periodGroups = new List<SelectListItem>();
            int groupSize = 10;
            for (int startIndex = 0; startIndex < fiscalYears.Count; startIndex += groupSize)
            {
                // Asegurarse de no salir de rango
                var chunk = fiscalYears.Skip(startIndex).Take(groupSize).ToList();
                if (chunk.Count < groupSize)
                    break; // O decide si agregas un último grupo incompleto

                // Tomamos el primer y último
                var firstFY = chunk.First();
                var lastFY = chunk.Last();
                string text = $"{firstFY.Fiscal_Year_Name} - {lastFY.Fiscal_Year_Name}";

                // Guardamos en la lista
                // El "Value" lo podemos almacenar como: "ID_Inicial,ID_Final" para luego filtrar en la consulta
                periodGroups.Add(new SelectListItem
                {
                    Text = text,
                    Value = firstFY.ID_Fiscal_Year + "," + lastFY.ID_Fiscal_Year
                });
            }

            ViewBag.FiscalYearGroups = new SelectList(periodGroups, "Value", "Text");

            // 2. Lista de plantas
            var plants = db.CTZ_plants
                           .Where(p => p.Active)
                           .OrderBy(p => p.Description)
                           .ToList();

            ViewBag.Plants = new SelectList(plants, "ID_Plant", "Description");

            return View();
        }

        // ===================================
        // MÉTODO PARA OBTENER DATOS (JSON)
        // ===================================
        [HttpGet]
        public JsonResult GetHoursData(int plantId, int startFyId, int endFyId)
        {
            try
            {
                // 1. Obtener la lista de años fiscales dentro del rango [startFyId, endFyId]
                var fiscalYears = db.CTZ_Fiscal_Years
                                    .Where(fy => fy.ID_Fiscal_Year >= startFyId && fy.ID_Fiscal_Year <= endFyId)
                                    .OrderBy(fy => fy.ID_Fiscal_Year)
                                    .ToList();

                // Para la configuración fija, usamos las claves de los FY que retornamos
                var fixedFYs = fiscalYears.Select(f => f.ID_Fiscal_Year.ToString()).ToList();

                // 2. Obtener la lista de IDs de líneas de la planta seleccionada
                var lineIds = db.CTZ_Production_Lines
                                .Where(l => l.ID_Plant == plantId && l.Active)
                                .Select(l => l.ID_Line)
                                .ToList();

                // 3. Obtener las líneas de la planta (solo lo necesario)
                var lines = db.CTZ_Production_Lines
                              .Where(l => lineIds.Contains(l.ID_Line))
                              .OrderBy(l => l.Line_Name)
                              .ToList();

                // 4. Obtener la lista de estatus de proyectos
                var statusList = db.CTZ_Project_Status
                                   .OrderByDescending(s => s.ID_Status)
                                   .ToList();

                // 5. Obtener los registros de CTZ_Hours_By_Line para las líneas y años fiscales
                var fyIds = fiscalYears.Select(f => f.ID_Fiscal_Year).ToList();
                var hoursData = db.CTZ_Hours_By_Line
                                  .Where(h => lineIds.Contains(h.ID_Line) && fyIds.Contains(h.ID_Fiscal_Year))
                                  .ToList();

                // 6. Obtener la descripción de la planta para evitar navegación (string)
                var plantDesc = db.CTZ_plants
                                  .Where(p => p.ID_Plant == plantId)
                                  .Select(p => p.Description)
                                  .FirstOrDefault();

                // 7. Construir la lista de DTO para Handsontable
                var resultRows = new List<HoursByLineRowDTO>();

                foreach (var line in lines)
                {
                    foreach (var status in statusList)
                    {
                        // Inicializar HoursByFY como un diccionario plano: clave = ID fiscal (string), valor = Hours (double?)
                        var hoursDict = fiscalYears.ToDictionary(
                            fy => fy.ID_Fiscal_Year.ToString(),
                            fy => (double?)null);

                        // Llenar el diccionario con los valores existentes
                        foreach (var fy in fiscalYears)
                        {
                            var key = fy.ID_Fiscal_Year.ToString();
                            var found = hoursData.FirstOrDefault(h =>
                                h.ID_Line == line.ID_Line &&
                                h.ID_Status == status.ID_Status &&
                                h.ID_Fiscal_Year == fy.ID_Fiscal_Year);
                            if (found != null)
                            {
                                hoursDict[key] = found.Hours;
                            }
                        }

                        resultRows.Add(new HoursByLineRowDTO
                        {
                            ID_Line = line.ID_Line,
                            LineName = line.Line_Name,
                            ID_Status = status.ID_Status,
                            StatusDescription = status.Description,
                            PlantName = plantDesc,
                            HoursByFY = hoursDict
                        });
                    }
                }

                // 8. Definir la configuración de columnas de forma fija en el controlador
                // Columnas fijas: Plant, Line y Status
                var colHeaders = new List<string> { "Plant", "Line", "Status" };
                var columnsConfig = new List<object> {
            new { data = "PlantName", readOnly = true },
            new { data = "LineName", readOnly = true },
            new { data = "StatusDescription", readOnly = true }
        };

                // Agregar columnas para cada FY
                foreach (var fy in fixedFYs)
                {
                    // Buscar el nombre del año fiscal
                    var fyObj = fiscalYears.FirstOrDefault(f => f.ID_Fiscal_Year.ToString() == fy);
                    var header = fyObj != null ? fyObj.Fiscal_Year_Name : "FY " + fy;
                    colHeaders.Add(header);
                    columnsConfig.Add(new
                    {
                        data = "HoursByFY." + fy, // acceso directo al diccionario
                        type = "numeric",
                        numericFormat = new { pattern = "0,0.00", culture = "en-US" }
                    });
                }

                return Json(new
                {
                    success = true,
                    data = resultRows,
                    columns = new { headers = colHeaders, config = columnsConfig }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult DownloadHoursTemplate()
        {
            // 1. Obtener todos los años fiscales (ordenados)
            var fiscalYears = db.CTZ_Fiscal_Years
                                .OrderBy(fy => fy.ID_Fiscal_Year)
                                .ToList();

            // 2. Obtener todas las líneas de producción activas, ordenadas por ID_Plant y luego por ID_Line
            var productionLines = db.CTZ_Production_Lines
                                    .Where(l => l.Active)
                                    .OrderBy(l => l.ID_Plant)
                                    .ThenBy(l => l.ID_Line)
                                    .ToList();

            // 3. Obtener todos los estatus de proyecto (en el orden que prefieras)
            var statusList = db.CTZ_Project_Status
                               .OrderByDescending(s => s.ID_Status)
                               .ToList();

            // 4. Obtener todas las plantas activas y construir un diccionario (ID_Plant => Description)
            var plants = db.CTZ_plants.Where(p => p.Active).ToList();
            var plantDict = plants.ToDictionary(p => p.ID_Plant, p => p.Description);

            // 5. Optimizar la construcción de filas:
            // a. Obtener listas de IDs
            var lineIds = productionLines.Select(l => l.ID_Line).ToList();
            var statusIds = statusList.Select(s => s.ID_Status).ToList();
            var fyIds = fiscalYears.Select(f => f.ID_Fiscal_Year).ToList();

            // b. Obtener todos los registros de CTZ_Hours_By_Line en una sola consulta
            var allRecords = db.CTZ_Hours_By_Line
                                .Where(h => lineIds.Contains(h.ID_Line) &&
                                            statusIds.Contains(h.ID_Status) &&
                                            fyIds.Contains(h.ID_Fiscal_Year))
                                .ToList();

            // c. Crear un diccionario para búsquedas rápidas: clave "lineId_statusId_fyId"
            var recordDictionary = allRecords.ToDictionary(
                h => $"{h.ID_Line}_{h.ID_Status}_{h.ID_Fiscal_Year}",
                h => h.Hours
            );

            // d. Construir la lista de filas: cada fila es la combinación de una línea de producción y un estatus.
            List<HoursByLineRowDTO> resultRows = new List<HoursByLineRowDTO>();
            foreach (var line in productionLines)
            {
                foreach (var status in statusList)
                {
                    var hoursDict = new Dictionary<string, double?>();
                    foreach (var fy in fiscalYears)
                    {
                        var key = $"{line.ID_Line}_{status.ID_Status}_{fy.ID_Fiscal_Year}";
                        double? hours = 0;
                        if (recordDictionary.TryGetValue(key, out double value))
                            hours = value;
                        hoursDict[fy.Fiscal_Year_Name] = hours;
                    }
                    resultRows.Add(new HoursByLineRowDTO
                    {
                        PlantName = plantDict.ContainsKey(line.ID_Plant) ? plantDict[line.ID_Plant] : string.Empty,
                        ID_Line = line.ID_Line,
                        LineName = line.Line_Name,
                        ID_Status = status.ID_Status,
                        StatusDescription = status.Description,
                        HoursByFY = hoursDict
                    });
                }
            }

            // 6. Crear el documento Excel usando SpreadsheetLight (SLDocument)
            SLDocument slDoc = new SLDocument();
            int currentRow = 1;
            int currentCol = 1;

            // Estilo para la cabecera: fondo #009ff5, texto blanco y negrita.
            SLStyle headerStyle = slDoc.CreateStyle();
            headerStyle.Fill.SetPattern(PatternValues.Solid,
                System.Drawing.ColorTranslator.FromHtml("#009ff5"),
                System.Drawing.ColorTranslator.FromHtml("#009ff5"));
            headerStyle.Font.Bold = true;
            headerStyle.Font.FontColor = System.Drawing.Color.White;

            // Estilo para las tres primeras columnas (fondo gris claro)
            SLStyle fixedColumnStyle = slDoc.CreateStyle();
            fixedColumnStyle.Fill.SetPattern(PatternValues.Solid,
                System.Drawing.ColorTranslator.FromHtml("#e0e0e0"),
                System.Drawing.ColorTranslator.FromHtml("#e0e0e0"));

            // Escribir encabezados fijos
            slDoc.SetCellValue(currentRow, currentCol++, "Plant");
            slDoc.SetCellValue(currentRow, currentCol++, "Line");
            slDoc.SetCellValue(currentRow, currentCol++, "Status");

            // Escribir encabezados para cada Fiscal Year
            foreach (var fy in fiscalYears)
            {
                slDoc.SetCellValue(currentRow, currentCol++, fy.Fiscal_Year_Name);
            }
            // Aplicar estilo de cabecera a la primera fila
            slDoc.SetRowStyle(1, headerStyle);
            currentRow++;

            // Escribir cada fila de datos
            foreach (var rec in resultRows)
            {
                currentCol = 1;
                slDoc.SetCellValue(currentRow, currentCol++, rec.PlantName);
                slDoc.SetCellValue(currentRow, currentCol++, rec.LineName);
                slDoc.SetCellValue(currentRow, currentCol++, rec.StatusDescription);
                foreach (var fy in fiscalYears)
                {
                    string header = fy.Fiscal_Year_Name;
                    double? hours = rec.HoursByFY.ContainsKey(header) ? rec.HoursByFY[header] : 0;
                    slDoc.SetCellValue(currentRow, currentCol++, hours.HasValue ? hours.Value : 0);
                }
                currentRow++;
            }

            // Aplicar estilo a las columnas fijas (Plant, Line, Status)
            slDoc.SetCellStyle(2, 1, 1 + resultRows.Count, 3, fixedColumnStyle);

            slDoc.FreezePanes(1, 3);

            // Ajustar automáticamente el ancho de todas las columnas utilizadas
            slDoc.AutoFitColumn(1, currentCol - 1);

            // Guardar el documento Excel en un MemoryStream y retornarlo
            using (var ms = new System.IO.MemoryStream())
            {
                slDoc.SaveAs(ms);
                ms.Position = 0;
                return File(ms.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "HoursByLineTemplate.xlsx");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadHoursTemplate(HttpPostedFileBase templateFile)
        {
            if (templateFile == null || templateFile.ContentLength == 0)
            {
                TempData["ErrorMessage"] = "Please upload a valid Excel file.";
                return RedirectToAction("hours_by_line");
            }

            try
            {
                // Cargar mapeos desde la base de datos.
                // Mapeo de plantas: ID_Plant -> Description (trimmed)
                var plants = db.CTZ_plants.Where(p => p.Active).ToList();
                var plantDict = plants.ToDictionary(p => p.ID_Plant, p => p.Description.Trim());

                // Opción 2: Usar clave compuesta para las líneas (PlantName_LineName)
                var lineMapping = db.CTZ_Production_Lines
                                    .Where(l => l.Active)
                                    .ToList()
                                    .ToDictionary(
                                        l => $"{plantDict[l.ID_Plant]}_{l.Line_Name.Trim()}",
                                        l => l.ID_Line
                                    );

                var statusMapping = db.CTZ_Project_Status
                                      .ToDictionary(s => s.Description.Trim(), s => s.ID_Status);

                var fiscalYearMapping = db.CTZ_Fiscal_Years
                                          .ToDictionary(fy => fy.Fiscal_Year_Name.Trim(), fy => fy.ID_Fiscal_Year);

                // Abrir el archivo Excel con SpreadsheetLight
                using (var ms = new System.IO.MemoryStream())
                {
                    templateFile.InputStream.CopyTo(ms);
                    ms.Position = 0;
                    SLDocument slDoc = new SLDocument(ms);

                    int headerRow = 1;
                    int dataStartRow = 2;

                    // Leer encabezados de FY (a partir de la columna 4)
                    List<string> fyHeaders = new List<string>();
                    int col = 4;
                    while (true)
                    {
                        string header = slDoc.GetCellValueAsString(headerRow, col);
                        if (string.IsNullOrEmpty(header))
                            break;
                        fyHeaders.Add(header.Trim());
                        col++;
                    }

                    // Leer los datos del Excel
                    List<HoursByLineRowDTO> excelRows = new List<HoursByLineRowDTO>();
                    int currentRow = dataStartRow;
                    while (true)
                    {
                        string plantName = slDoc.GetCellValueAsString(currentRow, 1).Trim();
                        if (string.IsNullOrEmpty(plantName))
                            break; // Fin de los datos

                        string lineName = slDoc.GetCellValueAsString(currentRow, 2).Trim();
                        string statusDesc = slDoc.GetCellValueAsString(currentRow, 3).Trim();

                        var hoursDict = new Dictionary<string, double?>();
                        for (int i = 0; i < fyHeaders.Count; i++)
                        {
                            double hours = slDoc.GetCellValueAsDouble(currentRow, 4 + i);
                            hoursDict[fyHeaders[i]] = hours;
                        }

                        excelRows.Add(new HoursByLineRowDTO
                        {
                            PlantName = plantName,
                            LineName = lineName,
                            StatusDescription = statusDesc,
                            HoursByFY = hoursDict
                        });
                        currentRow++;
                    }

                    // Acumular claves únicas del Excel para consulta en bloque.
                    var excelKeys = new HashSet<string>();
                    foreach (var rowItem in excelRows)
                    {
                        // Clave para la línea (PlantName_LineName)
                        string lineKey = $"{rowItem.PlantName}_{rowItem.LineName}";
                        if (lineMapping.ContainsKey(lineKey) && statusMapping.ContainsKey(rowItem.StatusDescription))
                        {
                            int lineId = lineMapping[lineKey];
                            int statusId = statusMapping[rowItem.StatusDescription];
                            foreach (var fyName in rowItem.HoursByFY.Keys)
                            {
                                if (fiscalYearMapping.ContainsKey(fyName))
                                {
                                    int fyId = fiscalYearMapping[fyName];
                                    string compositeKey = $"{lineId}_{statusId}_{fyId}";
                                    excelKeys.Add(compositeKey);
                                }
                            }
                        }
                    }

                    // Obtener en una sola consulta los registros existentes de CTZ_Hours_By_Line
                    var existingRecords = db.CTZ_Hours_By_Line
                        .Where(h => excelKeys.Contains(h.ID_Line + "_" + h.ID_Status + "_" + h.ID_Fiscal_Year))
                        .ToList();
                    var dbRecordsDict = existingRecords.ToDictionary(
                        h => $"{h.ID_Line}_{h.ID_Status}_{h.ID_Fiscal_Year}",
                        h => h
                    );

                    // Inicializar contadores
                    int countInserted = 0;
                    int countUpdated = 0;
                    int countDeleted = 0;

                    // Procesar cada fila del Excel
                    foreach (var rowItem in excelRows)
                    {
                        // Construir la clave compuesta para la línea
                        string lineKey = $"{rowItem.PlantName}_{rowItem.LineName}";
                        if (!lineMapping.ContainsKey(lineKey) || !statusMapping.ContainsKey(rowItem.StatusDescription))
                            continue;  // Saltar si falta algún mapeo

                        int lineId = lineMapping[lineKey];
                        int statusId = statusMapping[rowItem.StatusDescription];

                        foreach (var kvp in rowItem.HoursByFY)
                        {
                            string fyName = kvp.Key;
                            double hours = kvp.Value.HasValue ? kvp.Value.Value : 0;
                            if (!fiscalYearMapping.ContainsKey(fyName))
                                continue;
                            int fyId = fiscalYearMapping[fyName];
                            string compositeKey = $"{lineId}_{statusId}_{fyId}";

                            if (dbRecordsDict.ContainsKey(compositeKey))
                            {
                                var record = dbRecordsDict[compositeKey];
                                if (hours == 0)
                                {
                                    db.CTZ_Hours_By_Line.Remove(record);
                                    countDeleted++;
                                }
                                else
                                {
                                    // Actualiza si el valor es diferente
                                    if (record.Hours != hours)
                                    {
                                        record.Hours = hours;
                                        countUpdated++;
                                    }
                                }
                            }
                            else
                            {
                                if (hours != 0)
                                {
                                    CTZ_Hours_By_Line newRecord = new CTZ_Hours_By_Line
                                    {
                                        ID_Line = lineId,
                                        ID_Status = statusId,
                                        ID_Fiscal_Year = fyId,
                                        Hours = hours
                                    };
                                    db.CTZ_Hours_By_Line.Add(newRecord);
                                    countInserted++;
                                }
                            }
                        }
                    }
                    db.SaveChanges();

                    // Crear mensaje de éxito con contadores.
                    TempData["SuccessMessage"] =
                        $"Data updated successfully from Excel template. Inserted: {countInserted}, Updated: {countUpdated}, Deleted: {countDeleted}.";

                    return RedirectToAction("hours_by_line");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error processing the Excel file: " + ex.Message;
                return RedirectToAction("hours_by_line");
            }
        }





        // ===================================
        // MÉTODO PARA GUARDAR DATOS (POST)
        // ===================================
        [HttpPost]
        public JsonResult SaveHoursData(SaveHoursDTO model)
        {
            if (model == null || model.Rows == null || model.Rows.Count == 0)
            {
                return Json(new { success = false, message = "No data received." });
            }

            try
            {
                // 1. Obtener los FY involucrados (IDs)
                var fyIds = model.FiscalYearIDs;

                // 2. Procesar cada fila
                foreach (var row in model.Rows)
                {
                    // row.HoursByFY es un diccionario <fyId, double?>
                    foreach (var kvp in row.HoursByFY)
                    {
                        int fyId = int.Parse(kvp.Key);
                        double? hours = kvp.Value; // puede ser null

                        // Buscamos si ya existe un registro
                        var existing = db.CTZ_Hours_By_Line.FirstOrDefault(h =>
                            h.ID_Line == row.ID_Line &&
                            h.ID_Status == row.ID_Status &&
                            h.ID_Fiscal_Year == fyId
                        );

                        if (hours == null || hours == 0)
                        {
                            // Si es null o 0, podrías decidir si borrar el registro o poner 0. 
                            // Suponiendo que quieras borrar el registro si existe y está en 0:
                            if (existing != null)
                            {
                                db.CTZ_Hours_By_Line.Remove(existing);
                            }
                        }
                        else
                        {
                            // Insertar o actualizar
                            if (existing == null)
                            {
                                // Insertar
                                db.CTZ_Hours_By_Line.Add(new CTZ_Hours_By_Line
                                {
                                    ID_Line = row.ID_Line,
                                    ID_Status = row.ID_Status,
                                    ID_Fiscal_Year = fyId,
                                    Hours = hours.Value
                                });
                            }
                            else
                            {
                                // Actualizar
                                existing.Hours = hours.Value;
                            }
                        }
                    }
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Data saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        //OEE
        public ActionResult CTZ_OEE()
        {
            ViewBag.Plants = new SelectList(
                db.CTZ_plants.Where(p => p.Active),
                "ID_Plant", "Description"
            );
            var current = DateTime.Now.Year;
            // Creamos un objeto anónimo con Value/Text y lo metemos en un SelectList
            var yearsSrc = Enumerable
                .Range(current - 2, 10)
                .Select(y => new { Value = y.ToString(), Text = y.ToString() });

            ViewBag.Years = new SelectList(yearsSrc, "Value", "Text");
            return View();
        }

        [HttpGet]
        public JsonResult LoadOeeData(int plantId, int year)
        {
            var lines = db.CTZ_Production_Lines
                          .Where(l => l.ID_Plant == plantId && l.Active)
                          .Select(l => new { l.ID_Line, l.Description })
                          .ToList();

            var existing = db.CTZ_OEE
                             .Where(o => o.CTZ_Production_Lines.ID_Plant == plantId && o.Year == year)
                             .ToList();

            var data = lines.Select(l => new OeeRowDto
            {
                ID_Line = l.ID_Line,
                LineName = l.Description,
                ValuesByMonth = Enumerable.Range(1, 12)
                    .ToDictionary(
                        m => m.ToString(),      // <-- clave como string
                        m => {
                            var rec = existing
                                .FirstOrDefault(o => o.ID_Line == l.ID_Line && o.Month == m);
                            return rec == null ? (float?)null : rec.OEE;
                        })
            }).ToList();

            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SaveOeeData(SaveOeeDto dto)
        {
            foreach (var row in dto.Rows)
            {
                foreach (var kv in row.ValuesByMonth)
                {
                    // 1) Parsear el mes
                    if (!byte.TryParse(kv.Key, out byte monthNum))
                        continue; // si la clave no es un número válido, la saltamos

                    double? val = kv.Value;

                    // 2) Buscar registro existente
                    var exist = db.CTZ_OEE
                                  .FirstOrDefault(o =>
                                      o.ID_Line == row.ID_Line &&
                                      o.Year == dto.Year &&
                                      o.Month == monthNum    // comparar con byte
                                  );

                    // 3) Insertar / actualizar / borrar
                    if (val.HasValue && val.Value > 0)
                    {
                        if (exist == null)
                        {
                            db.CTZ_OEE.Add(new CTZ_OEE
                            {
                                ID_Line = row.ID_Line,
                                Year = dto.Year,
                                Month = monthNum,               // asignar byte
                                OEE = (float)val.Value       // o el tipo que sea tu columna
                            });
                        }
                        else
                        {
                            exist.OEE = (float)val.Value;
                        }
                    }
                    else if (exist != null)
                    {
                        db.CTZ_OEE.Remove(exist);
                    }
                }
            }

            db.SaveChanges();
            return Json(new { success = true, message = "OEE guardado correctamente." });
        }

        // GET: Admin/ManagePermissions
        public ActionResult ManagePermissions(int? selectedRoleId)
        {
            // 1) Determinar el rol activo (si no viene, tomamos el primero)
            int roleId = selectedRoleId ?? db.CTZ_Roles.First().ID_Role;

            // 2) Construir el ViewModel
            var vm = new PermissionManagementViewModel
            {
                Roles = db.CTZ_Roles.ToList(),
                Resources = db.CTZ_Resources.ToList(),
                Actions = db.CTZ_Actions.ToList(),
                Conditions = db.CTZ_Conditions.ToList(),
                // Traemos sólo los permisos de ese rol
                RolePermissions = db.CTZ_Role_Permissions
                                             .Where(rp => rp.ID_Role == roleId)
                                             .ToList(),
                RolePermissionConditions = db.CTZ_Role_Permission_Conditions
                                             .Where(rpc => rpc.ID_Role == roleId)
                                             .ToList(),
                Employees = db.empleados.Include("CTZ_Roles").ToList(),
                SelectedRoleId = roleId
            };

            return View(vm);
        }

        // POST: Admin/SaveUserRoles
        [HttpPost]
        public JsonResult SaveUserRoles(FormCollection form)
        {
            try
            {
                // 1) Traer todos los roles una sola vez
                var allRoles = db.CTZ_Roles.ToList();

                // 2) Para cada empleado, ajustar su colección e.CTZ_Roles
                var employees = db.empleados.Include("CTZ_Roles").ToList();
                foreach (var emp in employees)
                {
                    foreach (var role in allRoles)
                    {
                        string key = $"userRole_{emp.id}_{role.ID_Role}";
                        bool shouldHave = form[key] != null;
                        bool hasRole = emp.CTZ_Roles.Any(r => r.ID_Role == role.ID_Role);

                        if (shouldHave && !hasRole)
                            emp.CTZ_Roles.Add(role);
                        else if (!shouldHave && hasRole)
                            emp.CTZ_Roles.Remove(emp.CTZ_Roles.First(r => r.ID_Role == role.ID_Role));
                    }
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Admin/SavePermissions  (igual que antes)
        [HttpPost]
        public JsonResult SavePermissions(int selectedRoleId, FormCollection form)
        {
            try
            {
                // 1) Carga catálogos
                var allResources = db.CTZ_Resources.ToList();
                var allActions = db.CTZ_Actions.ToList();

                // 2) Trae sólo los permisos actuales de este rol
                var existingPerms = db.CTZ_Role_Permissions
                                      .Where(rp => rp.ID_Role == selectedRoleId)
                                      .ToList();

                // 3) Recorre cada posible combo (recurso × acción)
                foreach (var res in allResources)
                {
                    foreach (var act in allActions)
                    {
                        // nombre del checkbox en el form
                        string key = $"perm_{selectedRoleId}_{res.ID_Resource}_{act.ID_Action}";
                        bool shouldHave = form[key] != null;

                        // buscamos si ya existe la fila en la BD
                        var rp = existingPerms
                                    .FirstOrDefault(x => x.ID_Resource == res.ID_Resource
                                                      && x.ID_Action == act.ID_Action);

                        if (rp != null)
                        {
                            // existe → actualizamos el CanDo
                            rp.CanDo = shouldHave;
                        }
                        else if (shouldHave)
                        {
                            // no existía y marcaste el checkbox → la insertamos nueva
                            db.CTZ_Role_Permissions.Add(new CTZ_Role_Permissions
                            {
                                ID_Role = selectedRoleId,
                                ID_Resource = res.ID_Resource,
                                ID_Action = act.ID_Action,
                                CanDo = true
                            });
                        }
                        // si no existía y no marcaste, no hacemos nada
                    }
                }

                // 4) Condiciones: borramos todas las previas de este rol
                var toDelete = db.CTZ_Role_Permission_Conditions
                                 .Where(c => c.ID_Role == selectedRoleId);
                db.CTZ_Role_Permission_Conditions.RemoveRange(toDelete);

                // 5) Y volvemos a insertar sólo las conds marcadas
                foreach (var res in allResources)
                    foreach (var act in allActions)
                    {
                        // sólo nos interesa si el rol ahora tiene permiso en ese combo
                        string pkey = $"perm_{selectedRoleId}_{res.ID_Resource}_{act.ID_Action}";
                        if (form[pkey] == null)
                            continue;

                        // para cada condición posible...
                        foreach (var cond in db.CTZ_Conditions)
                        {
                            string ckey = $"cond_{selectedRoleId}_{res.ID_Resource}_{act.ID_Action}_{cond.ID_Condition}";
                            if (form[ckey] != null)
                            {
                                db.CTZ_Role_Permission_Conditions.Add(new CTZ_Role_Permission_Conditions
                                {
                                    ID_Role = selectedRoleId,
                                    ID_Resource = res.ID_Resource,
                                    ID_Action = act.ID_Action,
                                    ID_Condition = cond.ID_Condition
                                });
                            }
                        }
                    }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        //Empleados a Plantas y departamentos

        // GET: Manage both forms
        [HttpGet]
        public ActionResult ManageEmployees()
        {
            var vm = new EmployeeAssignmentsViewModel();
            PopulateLists(vm);
            return View(vm);
        }

        // AJAX: devuelve los empleados ya asignados a una planta
        [HttpGet]
        public JsonResult GetPlantEmployees(int plantId)
        {
            var ids = db.CTZ_Employee_Plants
                        .Where(x => x.ID_Plant == plantId)
                        .Select(x => x.ID_Employee)
                        .ToList();
            return Json(ids, JsonRequestBehavior.AllowGet);
        }

        // AJAX: devuelve los empleados ya asignados a un depto.
        [HttpGet]
        public JsonResult GetDepartmentEmployees(int deptId)
        {
            var ids = db.CTZ_Employee_Departments
                        .Where(x => x.ID_Department == deptId)
                        .Select(x => x.ID_Employee)
                        .ToList();
            return Json(ids, JsonRequestBehavior.AllowGet);
        }

        // POST: Guardar asignaciones Plant → Employees
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePlantAssignments(EmployeeAssignmentsViewModel vm)
        {
            PopulateLists(vm);
            if (!vm.SelectedPlantId.HasValue)
            {
                ModelState.AddModelError(nameof(vm.SelectedPlantId), "Plant is required.");
                return View("ManageEmployees", vm);
            }

            // borrar viejas
            var old = db.CTZ_Employee_Plants
                        .Where(x => x.ID_Plant == vm.SelectedPlantId.Value);
            db.CTZ_Employee_Plants.RemoveRange(old);

            // añadir nuevas
            foreach (var eid in vm.EmployeePlants ?? Enumerable.Empty<int>())
            {
                db.CTZ_Employee_Plants.Add(new CTZ_Employee_Plants
                {
                    ID_Plant = vm.SelectedPlantId.Value,
                    ID_Employee = eid
                });
            }
            db.SaveChanges();
            TempData["PlantSuccess"] = "Plant assignments saved.";
            return RedirectToAction("ManageEmployees");
        }

        // POST: Guardar asignaciones Dept → Employees
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDepartmentAssignments(EmployeeAssignmentsViewModel vm)
        {
            PopulateLists(vm);
            if (!vm.SelectedDepartmentId.HasValue)
            {
                ModelState.AddModelError(nameof(vm.SelectedDepartmentId), "Department is required.");
                return View("ManageEmployees", vm);
            }

            var old = db.CTZ_Employee_Departments
                        .Where(x => x.ID_Department == vm.SelectedDepartmentId.Value);
            db.CTZ_Employee_Departments.RemoveRange(old);

            foreach (var eid in vm.EmployeeDepartments ?? Enumerable.Empty<int>())
            {
                db.CTZ_Employee_Departments.Add(new CTZ_Employee_Departments
                {
                    ID_Department = vm.SelectedDepartmentId.Value,
                    ID_Employee = eid
                });
            }
            try
            {
                db.SaveChanges();
                TempData["DeptSuccess"] = "Department assignments saved.";
            }
            catch (Exception e) {
                TempData["DeptError"] = "Error: "+e.Message;
            }
            return RedirectToAction("ManageEmployees");
        }
        // GET: CTZ_Catalogs/ManageShiftsByPlant
        public ActionResult ManageShiftsByPlant()
        {
            // Por ahora, solo necesitamos verificar los permisos de acceso básicos.
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
                return View("../Home/ErrorPermisos");

            // Pasamos un booleano a la vista para saber si el usuario puede editar.
            // Reutilizamos el permiso de "Data Management" como ejemplo.
            int me = obtieneEmpleadoLogeado().id;
            var auth = new AuthorizationService(db);
            ViewBag.CanEdit = auth.CanPerform(me, ResourceKey.CatalogsDataManagement, ActionKey.Edit);

            return View();
        }

        private void PopulateLists(EmployeeAssignmentsViewModel vm)
        {
            // Empleados activos
            vm.Employees = db.empleados.ToList()
                             .Where(e => e.activo.HasValue && e.activo.Value)
                             .OrderBy(e => e.nombre)
                             .ThenBy(e => e.apellido1)
                             .Select(e => new SelectListItem
                             {
                                 Value = e.id.ToString(),
                                 Text = e.ConcatNumEmpleadoNombre
                             })
                             .ToList();

            // Plantas
            vm.Plants = db.CTZ_plants
                          .Where(p => p.Active)
                          .OrderBy(p => p.Description)
                          .Select(p => new SelectListItem
                          {
                              Value = p.ID_Plant.ToString(),
                              Text = p.Description
                          })
                          .ToList();

            // Departamentos
            vm.Departments = db.CTZ_Departments
                               .OrderBy(d => d.Name)
                               .Select(d => new SelectListItem
                               {
                                   Value = d.ID_Department.ToString(),
                                   Text = d.Name
                               })
                               .ToList();
        }

        //COUNTRIES
        public ActionResult CTZ_countries()
        {
            var vm = db.CTZ_Countries
                       .Select(c => new CountryToggleViewModel
                       {
                           ID_Country = c.ID_Country,
                           ISO3 = c.ISO3,
                           Nicename = c.Nicename,
                           Active = c.Active,
                           Warning = c.Warning
                       })
                       .ToList();
            return View(vm);
        }

        // POST: CTZ_Catalogs/CTZ_countries
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CTZ_countries(string countriesJson)
        {
            // 1. Deserializa tu JSON y arma un diccionario para lookup O(1)
            var updates = JsonConvert
                .DeserializeObject<List<CountryToggleViewModel>>(countriesJson)
                .ToDictionary(x => x.ID_Country);

            // 2. Trae de una sola vez sólo los países que vas a actualizar
            var ids = updates.Keys.ToList();
            var countries = db.CTZ_Countries
                              .Where(c => ids.Contains(c.ID_Country))
                              .ToList();

            // 3. Deshabilita la detección automática para el loop
            db.Configuration.AutoDetectChangesEnabled = false;

            // 4. Recorre y actualiza sólo los que cambian
            foreach (var country in countries)
            {
                var vm = updates[country.ID_Country];
                if (country.Active != vm.Active || country.Warning != vm.Warning)
                {
                    country.Active = vm.Active;
                    country.Warning = vm.Warning;
                    // Opcional: marcar sólo si cambió
                    db.Entry(country).State = EntityState.Modified;
                }
            }

            // 5. Reactiva la detección y guarda TODO de una vez
            db.Configuration.AutoDetectChangesEnabled = true;
            db.SaveChanges();

            TempData["Success"] = "Countries updated successfully.";
            return RedirectToAction("CTZ_countries");
        }


        // CTZ_Holiday
        // GET: CTZ_Holidays
        public ActionResult CTZ_Holidays()
        {
            int me = obtieneEmpleadoLogeado().id;
            var auth = new AuthorizationService(db);
            bool canEditDataManagement = auth.CanPerform(me, ResourceKey.CatalogsDataManagement, ActionKey.Edit /*, context*/);

            // Verificar rol
            if (!TieneRol(TipoRoles.CTZ_ACCESO) || !canEditDataManagement)
                return View("../Home/ErrorPermisos");

            var vm = db.CTZ_Holidays
                       .OrderBy(h => h.HolidayDate)
                       .ToList();


            ViewBag.CanViewDataManagement = auth.CanPerform(me, ResourceKey.CatalogsDataManagement, ActionKey.View /*, context*/);
            ViewBag.CanEditDataManagement = auth.CanPerform(me, ResourceKey.CatalogsDataManagement, ActionKey.Edit /*, context*/);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHoliday(DateTime HolidayDate, string Description, bool Active)
        {
            var dto = new CTZ_Holidays
            {
                HolidayDate = HolidayDate,
                Description = Description,
                Active = Active
            };
            db.CTZ_Holidays.Add(dto);
            db.SaveChanges();
            return Json(new
            {
                ID_Holiday = dto.ID_Holiday,
                HolidayDate = dto.HolidayDate.ToString("yyyy-MM-dd"),
                Description = dto.Description,
                Active = dto.Active
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateHoliday(int ID_Holiday, DateTime HolidayDate, string Description, bool Active)
        {
            var existing = db.CTZ_Holidays.Find(ID_Holiday);
            if (existing == null) return HttpNotFound();

            existing.HolidayDate = HolidayDate;
            existing.Description = Description;
            existing.Active = Active;
            db.SaveChanges();

            return Json(new
            {
                ID_Holiday,
                HolidayDate = existing.HolidayDate.ToString("yyyy-MM-dd"),
                Description = existing.Description,
                Active = existing.Active
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteHoliday(int id)
        {
            var h = db.CTZ_Holidays.Find(id);
            if (h == null)
                return HttpNotFound();

            db.CTZ_Holidays.Remove(h);
            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleHolidayActive(int id, bool active)
        {
            var h = db.CTZ_Holidays.Find(id);
            if (h == null)
                return HttpNotFound();

            h.Active = active;
            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        //----- para razones de RECHAZO -----------

        public ActionResult RejectionReasons(int? editId)
        {
            int me = obtieneEmpleadoLogeado().id;
            var auth = new AuthorizationService(db);
            bool canEditDataManagement = auth.CanPerform(me, ResourceKey.CatalogsDataManagement, ActionKey.Edit /*, context*/);

            // Verificar rol
            if (!TieneRol(TipoRoles.CTZ_ACCESO) || !canEditDataManagement)
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            // load lists for dropdowns
            ViewBag.Departments = new SelectList(
                db.CTZ_Departments.OrderBy(d => d.Name),
                "ID_Department", "Name");

            ViewBag.ActionTypes = new SelectList(
                Enum.GetValues(typeof(ActionTypeEnum))
                    .Cast<ActionTypeEnum>()
                    .Select(a => new { Value = (byte)a, Text = a.ToString() }),
                "Value", "Text");

            var vm = new RejectionReasonsPageViewModel();

            // fetch all reasons
            var all = db.CTZ_RejectionReason
                .Include(r => r.CTZ_Departments)
                .Include(r => r.CTZ_RejectionReason_Department.Select(rd => rd.CTZ_Departments))

                .ToList();

            vm.Reasons = all.Select(r => new RejectionReasonViewModel
            {
                ID_Reason = r.ID_Reason,
                Name = r.Name.Trim(),
                ActionType = (ActionTypeEnum)r.ActionType,
                ReassignDepartmentId = r.ReassignDepartmentId,
                ReassignDepartmentName = r.CTZ_Departments?.Name,
                Departments = r.CTZ_RejectionReason_Department
                               .Select(rd => rd.CTZ_Departments.Name).ToList(),
                Active = r.Active
            }).ToList();

            // if editing, populate vm.Current
            if (editId.HasValue)
            {
                var e = all.SingleOrDefault(r => r.ID_Reason == editId);
                if (e != null)
                {
                    vm.Current.ID_Reason = e.ID_Reason;
                    vm.Current.Name = e.Name.Trim();
                    vm.Current.ActionType = (ActionTypeEnum)e.ActionType;
                    vm.Current.ReassignDepartmentId = e.ReassignDepartmentId;
                    vm.Current.DepartmentIds = e.CTZ_RejectionReason_Department
                                                   .Select(rd => rd.ID_Department.Value).ToList();
                    vm.Current.Active = e.Active;
                }
            }

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveRejectionReason(RejectionReasonsPageViewModel vm)
        {
            // 1) Validar Nombre
            if (string.IsNullOrWhiteSpace(vm.Current.Name))
            {
                ModelState.AddModelError(nameof(vm.Current.Name), "A Reason Name is required.");
            }
            else if (vm.Current.Name.Length > 200)
            {
                ModelState.AddModelError(
                    nameof(vm.Current.Name),
                    "Reason Name cannot exceed 200 characters."
                );
            }

            // 2) Siempre requerir al menos un departamento
            if (vm.Current.DepartmentIds == null || !vm.Current.DepartmentIds.Any())
            {
                ModelState.AddModelError(
                    nameof(vm.Current.DepartmentIds),
                    "At least one department must be selected."
                );
            }

            // 3) ReassignDepartmentId es obligatorio salvo FinalizeAll
            if (vm.Current.ActionType != ActionTypeEnum.FinalizeAll
                && !vm.Current.ReassignDepartmentId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(vm.Current.ReassignDepartmentId),
                    "You must select a department to reassign unless the action is 'Finalize All'."
                );
            }


            // Si hay errores, volvemos a la vista de edición
            if (!ModelState.IsValid) 
                return RejectionReasons(vm.Current.ID_Reason);

            CTZ_RejectionReason entity;
            if (vm.Current.ID_Reason == 0)
            {
                entity = new CTZ_RejectionReason();
                db.CTZ_RejectionReason.Add(entity);
            }
            else
            {
                entity = db.CTZ_RejectionReason
                           .Include(r => r.CTZ_RejectionReason_Department)
                           .Single(r => r.ID_Reason == vm.Current.ID_Reason);
                // clear existing links
                db.CTZ_RejectionReason_Department.RemoveRange(
                    entity.CTZ_RejectionReason_Department);
            }

            // common fields
            entity.Name = vm.Current.Name;
            entity.ActionType = (byte)vm.Current.ActionType;
            entity.ReassignDepartmentId = vm.Current.ReassignDepartmentId;
            entity.Active = vm.Current.Active;

            // re-link departments
            foreach (var depId in vm.Current.DepartmentIds)
            {
                entity.CTZ_RejectionReason_Department.Add(new CTZ_RejectionReason_Department
                {
                    ID_Department = depId
                });
            }

            db.SaveChanges();
            return RedirectToAction("RejectionReasons");
        }
        public ActionResult DeleteRejectionReason(int id)
        {
            // 1) Traer la razón e incluir el nav-prop
            var reason = db.CTZ_RejectionReason
                           .Include(r => r.CTZ_RejectionReason_Department)
                           .SingleOrDefault(r => r.ID_Reason == id);

            if (reason == null)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("That rejection reason does not exist.", TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("RejectionReasons");
            }

            // 2) Verificar si se está usando en alguna asignación
            bool inUse = db.CTZ_Project_Assignment
                           .Any(a => a.ID_RejectionReason == id);
            if (inUse)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("This rejection reason is in use and cannot be deleted.", TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("RejectionReasons");
            }

            // 3) Primero eliminar sus mappings en CTZ_RejectionReason_Department
            if (reason.CTZ_RejectionReason_Department.Any())
            {
                db.CTZ_RejectionReason_Department.RemoveRange(
                    reason.CTZ_RejectionReason_Department
                );
                db.SaveChanges();
            }

            // 4) Ahora sí borrar la razón
            db.CTZ_RejectionReason.Remove(reason);
            db.SaveChanges();

            TempData["Mensaje"] = new MensajesSweetAlert("Rejection reason deleted successfully", TipoMensajesSweetAlerts.SUCCESS);
          
            return RedirectToAction("RejectionReasons");
        }


        #region ActividadesPorDepartamento
        // GET: CTZ_Catalogs/DepartmentActivities
        [HttpGet]
        public ActionResult DepartmentActivities()
        {
            int me = obtieneEmpleadoLogeado().id;
            var auth = new AuthorizationService(db);
            bool canEditDataManagement = auth.CanPerform(me, ResourceKey.CatalogsDataManagement, ActionKey.Edit /*, context*/);

            // Verificar rol
            if (!TieneRol(TipoRoles.CTZ_ACCESO) || !canEditDataManagement)
                return View("../Home/ErrorPermisos");

            // Para el dropdown de Departamentos
            ViewBag.Departments = new SelectList(
                db.CTZ_Departments.OrderBy(d => d.Name),
                "ID_Department", "Name");
            return View();
        }

        // GET JSON: lista de actividades
        [HttpGet]
        public JsonResult LoadDepartmentActivities()
        {
            var list = db.CTZ_Department_Activity
                         .Include(a => a.CTZ_Departments)
                         .Select(a => new {
                             a.ID_Activity,
                             a.ID_Department,                  // ← lo necesitamos en el cliente
                             DepartmentName = a.CTZ_Departments.Name,
                             a.Description,
                             a.Active
                         })
                         .ToList();
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }


        // POST: crear
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CreateDepartmentActivity(int deptId, string desc, bool active)
        {
            if (string.IsNullOrWhiteSpace(desc) || desc.Length > 200)
                return Json(new { success = false, message = "Description required (max 200 chars)." });

            // 1) Crear la entidad con el valor real de 'active'
            var ent = new CTZ_Department_Activity
            {
                ID_Department = deptId,
                Description = desc.Trim(),
                Active = active
            };
            db.CTZ_Department_Activity.Add(ent);
            db.SaveChanges();

            // 2) Recuperar el nombre del depto recién guardado
            var deptName = db.CTZ_Departments
                             .Where(d => d.ID_Department == ent.ID_Department)
                             .Select(d => d.Name)
                             .FirstOrDefault();

            // 3) Devolver JSON con todos los campos que usa tu tabla
            return Json(new
            {
                success = true,
                data = new
                {
                    ent.ID_Activity,
                    ent.ID_Department,
                    DepartmentName = deptName,
                    ent.Description,
                    ent.Active
                }
            });
        }


        // POST: actualizar
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult UpdateDepartmentActivity(int id, int deptId, string desc, bool active)
        {
            var ent = db.CTZ_Department_Activity.Find(id);
            if (ent == null)
                return Json(new { success = false, message = "Activity not found." });

            if (string.IsNullOrWhiteSpace(desc) || desc.Length > 200)
                return Json(new { success = false, message = "Description required (max 200 chars)." });

            ent.ID_Department = deptId;
            ent.Description = desc.Trim();
            ent.Active = active;
            db.SaveChanges();
            return Json(new { success = true });
        }


        // POST: eliminar
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DeleteDepartmentActivity(int id)
        {
            // verificar dependencias
            bool inUse = db.CTZ_Assignment_Activity.Any(x => x.ID_Activity == id);
            if (inUse)
                return Json(new
                {
                    success = false,
                    message = "Cannot delete: activity is used in assignments."
                });
            var ent = db.CTZ_Department_Activity.Find(id);
            if (ent == null) return Json(new { success = false, message = "Not found." });
            db.CTZ_Department_Activity.Remove(ent);
            db.SaveChanges();
            return Json(new { success = true });
        }

        #endregion

        #region CTZ_Total_Time_Per_Fiscal_Year
        [HttpGet]
        public JsonResult GetTotalTimeData(int idFiscalYearStart, int idFiscalYearEnd)
        {
            // traemos todos los años fiscales en el rango
            var list = db.CTZ_Fiscal_Years
                .Where(f => f.ID_Fiscal_Year >= idFiscalYearStart
                         && f.ID_Fiscal_Year <= idFiscalYearEnd)
                .OrderBy(f => f.Start_Date)
                .Select(f => new TotalTimeDto
                {
                    ID_Fiscal_Year = f.ID_Fiscal_Year,
                    Fiscal_Year_Name = f.Fiscal_Year_Name,
                    Value = db.CTZ_Total_Time_Per_Fiscal_Year
                                .Where(t => t.ID_Fiscal_Year == f.ID_Fiscal_Year)
                                .Select(t => (double?)t.Value)
                                .FirstOrDefault()
                })
                .ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveTotalTimeData(List<TotalTimeDto> data)
        {
            foreach (var item in data)
            {
                var existing = db.CTZ_Total_Time_Per_Fiscal_Year
                                 .FirstOrDefault(t => t.ID_Fiscal_Year == item.ID_Fiscal_Year);
                if (existing == null && item.Value.HasValue)
                {
                    // insertar nuevo
                    db.CTZ_Total_Time_Per_Fiscal_Year.Add(new CTZ_Total_Time_Per_Fiscal_Year
                    {
                        ID_Fiscal_Year = item.ID_Fiscal_Year,
                        Value = item.Value.Value
                    });
                }
                else if (existing != null)
                {
                    if (item.Value.HasValue)
                    {
                        // actualizar
                        existing.Value = item.Value.Value;
                        db.Entry(existing).State = EntityState.Modified;
                    }
                    else
                    {
                        // borrar
                        db.CTZ_Total_Time_Per_Fiscal_Year.Remove(existing);
                    }
                }
            }
            db.SaveChanges();
            return Json(new { success = true });
        }
        #endregion

        #region Theoretical Line Criteria Management

        [HttpGet]
        public ActionResult GetTheoreticalRules()
        {
            try
            {
                var rules = db.CTZ_Theoretical_Line_Criteria
                    .Select(r => new
                    {
                        r.ID_Rule,
                        r.ID_Plant,
                        r.ID_Material_Type,
                        r.Thickness_Min,
                        r.Thickness_Max,
                        r.Width_Min,
                        r.Width_Max,
                        r.Pitch_Min,
                        r.Pitch_Max,
                        r.Tensile_Min,
                        r.Tensile_Max,                            
                        r.Special_Rule_Key,
                        r.Resulting_Line_ID,
                        r.Priority,
                        r.Is_Active,
                        r.Description
                    })
                    .OrderBy(r => r.ID_Plant).ThenBy(r=> r.Resulting_Line_ID)
                    .ToList();

                return Json(rules, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Error getting theoretical rules: " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult SaveTheoreticalRules(List<CTZ_Theoretical_Line_Criteria> rules)
        {
            if (rules == null)
            {
                rules = new List<CTZ_Theoretical_Line_Criteria>();
            }

            try
            {
                // --- INICIO DE LA MODIFICACIÓN ---

                // 1. Obtener todas las reglas existentes en la base de datos de una sola vez.
                var existingRules = db.CTZ_Theoretical_Line_Criteria.ToList();

                // 2. Identificar las reglas a eliminar.
                // Son aquellas que están en la BD pero no en la lista que nos llega del cliente.
                var rulesToDelete = existingRules
                    .Where(r_db => !rules.Any(r_client => r_client.ID_Rule == r_db.ID_Rule))
                    .ToList();

                foreach (var rule in rulesToDelete)
                {
                    db.CTZ_Theoretical_Line_Criteria.Remove(rule);
                }

                // 3. Procesar las reglas que llegan del cliente para añadir o actualizar.
                foreach (var ruleFromClient in rules)
                {
                    if (ruleFromClient.ID_Rule > 0)
                    {
                        // Es una regla existente -> ACTUALIZAR
                        var ruleInDb = existingRules.FirstOrDefault(r => r.ID_Rule == ruleFromClient.ID_Rule);
                        if (ruleInDb != null)
                        {
                            // Actualizamos las propiedades de la entidad que ya está en el contexto.
                            // NO usamos Attach. Usamos Entry para copiar los valores.
                            db.Entry(ruleInDb).CurrentValues.SetValues(ruleFromClient);
                        }
                    }
                    else
                    {
                        // Es una regla nueva (ID = 0) -> AÑADIR
                        db.CTZ_Theoretical_Line_Criteria.Add(ruleFromClient);
                    }
                }

                // 4. Guardar todos los cambios (eliminaciones, modificaciones y adiciones) en una sola transacción.
                db.SaveChanges();

                // --- FIN DE LA MODIFICACIÓN ---

                return Json(new { success = true, message = "Theoretical line selection rules saved successfully." });
            }
            catch (Exception ex)
            {
                // Considerar registrar el error ex para depuración.
                return Json(new { success = false, message = "An error occurred while saving the rules. Details: " + ex.Message });
            }
        }

        #endregion

        #region TechnicalInformation
        // En CTZ_CatalogsController.cs

        [HttpGet]
        public ActionResult TechnicalInformation()
        {
            // Verificar permisos de edición para la vista
            int me = obtieneEmpleadoLogeado().id;
            var auth = new AuthorizationService(db);
            ViewBag.CanEdit = auth.CanPerform(me, ResourceKey.CatalogsEngineering, ActionKey.Edit);

            // Cargar la lista de plantas para el primer dropdown
            ViewBag.Plants = new SelectList(
                db.CTZ_plants.Where(p => p.Active).OrderBy(p => p.Description),
                "ID_Plant", "Description"
            );

            return View();
        }

        // GET JSON: Devuelve los tipos de material para una línea específica.
        [HttpGet]
        public JsonResult GetMaterialTypesByLine(int lineId)
        {
            // Esta consulta une las tablas para encontrar los tipos de material
            // asociados a una línea a través de la tabla intermedia.
            var materialTypes = db.CTZ_Material_Type_Lines
                .Where(mtl => mtl.ID_Line == lineId && mtl.CTZ_Material_Type.Active)
                .Select(mtl => new {
                    Value = mtl.ID_Material_Type,
                    Text = mtl.CTZ_Material_Type.Material_Name
                })
                .Distinct()
                .OrderBy(mt => mt.Text)
                .ToList();

            return Json(materialTypes, JsonRequestBehavior.AllowGet);
        }

        // En CTZ_CatalogsController.cs

        [HttpGet]
        public JsonResult LoadTechnicalInformationForLine(int lineId)
        {
            // 1. Obtenemos la línea y el nombre de su planta asociada en una sola consulta.
            var lineInfo = db.CTZ_Production_Lines
                .Where(l => l.ID_Line == lineId)
                .Select(l => new { l.Line_Name, PlantName = l.CTZ_plants.Description })
                .FirstOrDefault();

            if (lineInfo == null)
            {
                // Si la línea no existe, devolvemos una lista vacía.
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }

            // 2. Obtenemos todos los tipos de material asociados a esta línea.
            var materialTypesForLine = db.CTZ_Material_Type_Lines
                .Where(mtl => mtl.ID_Line == lineId && mtl.CTZ_Material_Type.Active)
                .Select(mtl => mtl.CTZ_Material_Type)
                .ToList();

            var materialTypeIds = materialTypesForLine.Select(mt => mt.ID_Material_Type).ToList();

            // 3. Obtenemos todos los criterios técnicos activos.
            var allCriteria = db.CTZ_Technical_Criteria.Where(c => c.IsActive).ToList();

            // 4. Obtenemos TODA la información técnica ya guardada para esta línea y sus tipos de material.
            var savedInfo = db.CTZ_Technical_Information_Line
                .Where(ti => ti.ID_Line == lineId && materialTypeIds.Contains(ti.ID_Material_type))
                .ToList();

            // 5. Construimos la lista completa de datos para la tabla.
            var dataForGrid = new List<TechnicalInformationDTO>();
            foreach (var materialType in materialTypesForLine)
            {
                foreach (var criterion in allCriteria)
                {
                    var info = savedInfo.FirstOrDefault(si =>
                        si.ID_Material_type == materialType.ID_Material_Type &&
                        si.ID_Criteria == criterion.ID_criteria);

                    dataForGrid.Add(new TechnicalInformationDTO
                    {
                        PlantName = lineInfo.PlantName,
                        LineName = lineInfo.Line_Name,
                        ID_Line = lineId,
                        ID_Material_Type = materialType.ID_Material_Type,
                        Material_Name = materialType.Material_Name,
                        ID_Technical_Information = info?.ID_Technical_Information ?? 0,
                        ID_Criteria = criterion.ID_criteria,
                        Description = criterion.CriteriaName,
                        DataType = criterion.DataType,
                        TextValue = info?.TextValue,
                        NumericValue = info?.NumericValue, 
                        MinValue = info?.MinValue,
                        MaxValue = info?.MaxValue,
                        Tolerance = info?.AbsoluteTolerance, // Añadimos la nueva propiedad
                        HasTolerance = criterion.HasTolerance,
                        IsActive = info?.IsActive ?? true
                    });
                }
            }

            // 6. Ordenamos el resultado final como solicitaste: por Línea, luego por ID de Criterio.
            var orderedData = dataForGrid
                .OrderBy(d => d.Material_Name)
                .ThenBy(d => d.ID_Criteria)
                .ToList();

            return Json(orderedData, JsonRequestBehavior.AllowGet);
        }


        // POST JSON: Guarda los cambios (nuevos, editados y borrados).
        [HttpPost]
        public JsonResult SaveTechnicalInformation(List<TechnicalInformationDTO> data)
        {
            if (data == null)
            {
                return Json(new { success = false, message = "No data received." });
            }

            try
            {
                // 1. Obtenemos el ID de la línea, que es común para todos los registros.
                var lineId = data.Select(d => d.ID_Line).FirstOrDefault();
                if (lineId == 0)
                {
                    return Json(new { success = false, message = "Line ID is missing. Cannot save." });
                }

                // 2. Agrupamos los datos que llegan desde el cliente por cada tipo de material.
                var groupedByMaterialType = data.GroupBy(d => d.ID_Material_Type);

                // 3. Obtenemos TODOS los registros existentes en la BD para la línea seleccionada de una sola vez.
                var allExistingRecordsForLine = db.CTZ_Technical_Information_Line
                    .Where(ti => ti.ID_Line == lineId)
                    .ToList();

                // 4. Iteramos sobre cada grupo (un grupo por cada tipo de material).
                foreach (var group in groupedByMaterialType)
                {
                    var materialTypeId = group.Key;
                    var dtosForThisMaterial = group.ToList();

                    // Filtramos los registros de la BD que corresponden a ESTE tipo de material.
                    var existingRecordsForMaterial = allExistingRecordsForLine
                        .Where(r => r.ID_Material_type == materialTypeId)
                        .ToList();

                    // 4a. Lógica de ELIMINACIÓN (para este grupo)
                    var recordsToDelete = existingRecordsForMaterial
                        .Where(dbRecord => !dtosForThisMaterial.Any(dto => dto.ID_Technical_Information == dbRecord.ID_Technical_Information))
                        .ToList();

                    if (recordsToDelete.Any())
                    {
                        db.CTZ_Technical_Information_Line.RemoveRange(recordsToDelete);
                    }

                    // 4b. Lógica de ACTUALIZACIÓN e INSERCIÓN (para este grupo)
                    foreach (var dto in dtosForThisMaterial)
                    {
                        // Solo procesamos filas que tienen una descripción válida para evitar guardar filas vacías.
                        if (string.IsNullOrWhiteSpace(dto.Description)) continue;

                        if (dto.ID_Technical_Information > 0) // Es un registro existente -> ACTUALIZAR
                        {
                            var recordInDb = existingRecordsForMaterial.FirstOrDefault(r => r.ID_Technical_Information == dto.ID_Technical_Information);
                            if (recordInDb != null)
                            {
                                // Asignación manual para evitar errores con propiedades extra del DTO.
                             //   recordInDb.Description = dto.Description;
                                recordInDb.TextValue = dto.TextValue;
                                recordInDb.NumericValue = dto.NumericValue;
                                recordInDb.MinValue = dto.MinValue;
                                recordInDb.MaxValue = dto.MaxValue;
                                recordInDb.IsActive = dto.IsActive;
                                recordInDb.AbsoluteTolerance = dto.Tolerance;

                            }
                        }
                        else // Es un registro nuevo -> INSERTAR
                        {
                            db.CTZ_Technical_Information_Line.Add(new CTZ_Technical_Information_Line
                            {
                                ID_Line = lineId,
                                ID_Material_type = materialTypeId,
                                ID_Criteria = dto.ID_Criteria,
                                //Description = dto.Description,
                                TextValue = dto.TextValue,
                                NumericValue = dto.NumericValue,
                                MinValue = dto.MinValue,
                                MaxValue = dto.MaxValue,
                                IsActive = dto.IsActive,
                                AbsoluteTolerance = dto.Tolerance
                            });
                        }
                    }
                }

                // 5. Guardar TODOS los cambios (de todos los grupos) en una sola transacción.
                db.SaveChanges();

                return Json(new { success = true, message = "Technical information saved successfully for all material types." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        #endregion

        #region Clientes
        // GET: CTZ_Catalogs/Clients
        public ActionResult Clients()
        {
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
                return View("../Home/ErrorPermisos");

            var vm = new ClientsViewModel();

            // 1. Obtenemos los clientes de la BD como antes
            var allClients = db.CTZ_Clients.Include(c => c.CTZ_Countries).OrderBy(c => c.Client_Name).ToList();

            // 2. ===== ¡LA MAGIA OCURRE AQUÍ! =====
            //    Transformamos la lista compleja en una lista simple usando LINQ.
            //    Seleccionamos explícitamente solo los campos que necesitamos.
            var simpleClientList = allClients.Select(c => new {
                c.ID_Cliente,
                c.Client_Name,
                c.Clave_SAP,
                c.Telephone,
                c.Direccion,
                c.Calle,
                c.Ciudad,
                c.Estado,
                c.Codigo_Postal,
                c.Automotriz,
                c.Active,
                c.ID_Country,
                // Aplanamos la relación: en lugar de un objeto Country completo, solo pasamos el nombre.
                CountryName = c.CTZ_Countries != null ? c.CTZ_Countries.Nicename : ""
            }).ToList();

            // 3. Pasamos esta nueva lista simple a la vista a través del ViewBag o un nuevo campo en el ViewModel.
            //    Vamos a usar ViewBag para simplicidad.
            ViewBag.ClientListDataForJs = JsonConvert.SerializeObject(simpleClientList);

            // El resto de tu ViewModel sigue igual
            vm.ClientList = allClients; // Para la tabla si la usas desde el modelo
            vm.CountryList = new SelectList(db.CTZ_Countries.Where(c => c.Active).OrderBy(c => c.Nicename), "ID_Country", "Nicename");

            // Pasamos los mensajes de éxito/error si existen
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(vm);
        }

        // POST: CTZ_Catalogs/SaveClient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveClient(CTZ_Clients client)
        {
            // --- PASO 1: Lógica de Validación de Nombre de Cliente Duplicado ---
            // Solo validamos si el usuario proporcionó un nombre de cliente.
            if (!string.IsNullOrEmpty(client.Client_Name))
            {
                // La consulta busca si existe OTRO cliente (ID_Cliente diferente) con el mismo nombre.
                bool isDuplicate = db.CTZ_Clients.Any(c => c.Clave_SAP == client.Clave_SAP && c.ID_Cliente != client.ID_Cliente);

                if (isDuplicate)
                {
                    // Si encontramos un duplicado, añadimos un error específico al campo 'Client_Name'.
                    ModelState.AddModelError("Clave_SAP", "This SAP Key is already in use by another client.");
                }
            }

            // --- PASO 2: Verificar si el Modelo es Válido en General ---
            if (ModelState.IsValid)
            {
                try
                {
                    // --- PASO 3: Lógica de Guardado (Crear o Actualizar) ---
                    if (client.ID_Cliente == 0) // Es un nuevo cliente
                    {
                        client.Active = true;
                        db.CTZ_Clients.Add(client);
                        TempData["SuccessMessage"] = "Client created successfully.";
                    }
                    else // Es una actualización
                    {
                        // Este enfoque es seguro y actualiza solo los campos que vienen del formulario.
                        db.Entry(client).State = EntityState.Modified;
                        TempData["SuccessMessage"] = "Client updated successfully.";
                    }

                    db.SaveChanges();
                    // Si todo sale bien, redirigimos al listado principal.
                    return RedirectToAction("Clients");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while saving: " + ex.Message;
                    return RedirectToAction("Clients");
                }
            }

            // --- PASO 4: Manejo de Errores de Validación ---
            // Si llegamos aquí, es porque ModelState.IsValid fue 'false'.
            // Volvemos a mostrar la vista del formulario para que el usuario vea los errores.

            TempData["ErrorMessage"] = "The provided information is invalid. Please review the fields.";

            // Para que la vista se renderice correctamente, debemos volver a cargar los datos que necesita,
            // como las listas de los DropDowns y la tabla de clientes.
            var vm = new ClientsViewModel
            {
                Client = client, // Devolvemos el objeto con los datos que el usuario ya había llenado.
                CountryList = new SelectList(db.CTZ_Countries.Where(c => c.Active).OrderBy(c => c.Nicename), "ID_Country", "Nicename", client.ID_Country)
            };

            // También necesitamos volver a cargar los datos para la tabla de Handsontable
            var allClients = db.CTZ_Clients.Include(c => c.CTZ_Countries).OrderBy(c => c.Client_Name).ToList();
            var simpleClientList = allClients.Select(c => new {
                c.ID_Cliente,
                c.Client_Name,
                c.Clave_SAP,
                c.Telephone,
                c.Direccion,
                c.Calle,
                c.Ciudad,
                c.Estado,
                c.Codigo_Postal,
                c.Automotriz,
                c.Active,
                c.ID_Country,
                CountryName = c.CTZ_Countries != null ? c.CTZ_Countries.Nicename : ""
            }).ToList();
            ViewBag.ClientListDataForJs = JsonConvert.SerializeObject(simpleClientList);

            return View("Clients", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadClients(HttpPostedFileBase excelFile)
        {
            if (excelFile == null || excelFile.ContentLength == 0)
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return RedirectToAction("Clients");
            }

            try
            {
                using (var stream = excelFile.InputStream)
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // 1. LEER EL EXCEL Y CONVERTIRLO A UNA LISTA
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                        });

                        System.Data.DataTable dataTable = result.Tables[0];
                        var excelClients = new List<Dictionary<string, string>>();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            var dict = new Dictionary<string, string>();
                            foreach (DataColumn col in dataTable.Columns)
                            {
                                dict[col.ColumnName] = row[col].ToString();
                            }
                            excelClients.Add(dict);
                        }

                        // 2. PREPARAR DATOS DE LA BD PARA BÚSQUEDAS RÁPIDAS
                        // Diccionario de clientes locales por Clave SAP
                        var localClientsDict = db.CTZ_Clients
                                        .Where(c => !string.IsNullOrEmpty(c.Clave_SAP))
                                        .GroupBy(c => c.Clave_SAP)
                                        .ToDictionary(g => g.Key, g => g.First());
                        // Diccionario de países por código ISO
                        var countriesDict = db.CTZ_Countries.Where(c => c.ISO != null).ToDictionary(c => c.ISO.ToUpper(), c => c.ID_Country);
                        // Conjunto de IDs de clientes que están siendo usados en Proyectos
                        var referencedClientIds = db.CTZ_Projects.Where(p => p.ID_Client.HasValue).Select(p => p.ID_Client.Value).ToHashSet();

                        var excelSapKeys = new HashSet<string>();
                        int createdCount = 0, updatedCount = 0;

                        // 3. PROCESAR FILAS DEL EXCEL (CREAR Y ACTUALIZAR)
                        // ===== INICIO DEL BLOQUE CORREGIDO =====
                        foreach (var excelRow in excelClients)
                        {
                            // Usamos los encabezados de tu Excel, asegurando que no haya espacios
                            string sapKey = excelRow["Customer"].Trim();
                            if (string.IsNullOrEmpty(sapKey)) continue;

                            excelSapKeys.Add(sapKey);

                            // Enlace automático del país usando la columna "Cty"
                            int? countryId = null;
                            string countryIso = excelRow["Cty"].Trim().ToUpper();
                            if (countriesDict.ContainsKey(countryIso))
                            {
                                countryId = countriesDict[countryIso];
                            }

                            // --- LÓGICA DE CONCATENACIÓN DE DIRECCIÓN ---
                            var addressParts = new[]
                            {
                                excelRow["Street"],
                                excelRow["City"],
                                excelRow["Rg"], // 'Rg' (Región) en Excel mapea a 'Estado' en tu BD
                                excelRow["PostalCode"]
                            };
                            string fullAddress = string.Join(", ", addressParts.Where(s => !string.IsNullOrEmpty(s.Trim())));
                            // ---------------------------------------------

                            if (localClientsDict.ContainsKey(sapKey)) // El cliente existe -> ACTUALIZAR
                            {
                                var clientInDb = localClientsDict[sapKey];
                                clientInDb.Client_Name = excelRow["Name 1"];
                                clientInDb.Pais = excelRow["Name"]; // El nombre del país viene de la columna 'Name'
                                clientInDb.Calle = excelRow["Street"]; // El campo 'Calle' es solo la calle
                                clientInDb.Direccion = fullAddress;    // El campo 'Direccion' es la concatenación
                                clientInDb.Ciudad = excelRow["City"];
                                clientInDb.Codigo_Postal = excelRow["PostalCode"];
                                clientInDb.Estado = excelRow["Rg"];
                                clientInDb.Telephone = excelRow["Telephone 1"];
                                clientInDb.ID_Country = countryId;
                                // La lógica para 'Automotriz' y 'Active' se mantiene como estaba o se puede añadir si viene en el Excel

                                db.Entry(clientInDb).State = EntityState.Modified;
                                updatedCount++;
                            }
                            else // El cliente no existe -> CREAR
                            {
                                var newClient = new CTZ_Clients
                                {
                                    Clave_SAP = sapKey,
                                    Client_Name = excelRow["Name 1"],
                                    Pais = excelRow["Name"],
                                    Calle = excelRow["Street"],
                                    Direccion = fullAddress,
                                    Ciudad = excelRow["City"],
                                    Codigo_Postal = excelRow["PostalCode"],
                                    Estado = excelRow["Rg"],
                                    Telephone = excelRow["Telephone 1"],
                                    ID_Country = countryId,
                                    Active = true,
                                    Automotriz = false // Asumimos 'false' por defecto para nuevos clientes
                                };
                                db.CTZ_Clients.Add(newClient);
                                createdCount++;
                            }
                        }
                        // ===== FIN DEL BLOQUE CORREGIDO =====

                        int deactivatedCount = 0;
                        int deletedCount = 0;

                        // Obtenemos los clientes que están en nuestra BD pero no en el nuevo Excel
                        var clientsToRemoveOrDeactivate = db.CTZ_Clients
                           .Where(c => !string.IsNullOrEmpty(c.Clave_SAP) && !excelSapKeys.Contains(c.Clave_SAP))
                           .ToList();

                        foreach (var client in clientsToRemoveOrDeactivate)
                        {
                            // Verificamos si este cliente está siendo usado en la tabla de Proyectos
                            if (referencedClientIds.Contains(client.ID_Cliente))
                            {
                                // Si está referenciado, NO PODEMOS BORRARLO.
                                // Solo lo desactivamos si es que estaba activo.
                                if (client.Active)
                                {
                                    client.Active = false;
                                    db.Entry(client).State = EntityState.Modified;
                                    deactivatedCount++;
                                }
                            }
                            else
                            {
                                // Si NO está referenciado en ninguna parte, lo borramos físicamente
                                db.CTZ_Clients.Remove(client);
                                deletedCount++;
                            }
                        }
                        // ===== FIN DE LA LÓGICA CORREGIDA =====


                        db.SaveChanges();
                        TempData["SuccessMessage"] = $"Process complete. Created: {createdCount}, Updated: {updatedCount}, Deactivated (in use): {deactivatedCount}, Deleted: {deletedCount}.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error processing file: " + ex.Message;
            }

            return RedirectToAction("Clients");
        }

        // POST: CTZ_Catalogs/DeactivateClient/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeactivateClient(int id)
        {
            try
            {
                var client = db.CTZ_Clients.Find(id);
                if (client == null)
                {
                    return Json(new { success = false, message = "Client not found." });
                }
                client.Active = false;
                db.SaveChanges();
                return Json(new { success = true, message = "Client deactivated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        // POST: CTZ_Catalogs/ReactivateClient/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ReactivateClient(int id)
        {
            try
            {
                var client = db.CTZ_Clients.Find(id);
                if (client == null)
                {
                    return Json(new { success = false, message = "Client not found." });
                }
                client.Active = true; // <-- La única diferencia: lo ponemos en true
                db.SaveChanges();
                return Json(new { success = true, message = "Client reactivated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        #endregion

        #region OEMs
        // GET: CTZ_Catalogs/OEMs
        public ActionResult OEMs()
        {
            if (!TieneRol(TipoRoles.CTZ_ACCESO))
                return View("../Home/ErrorPermisos");

         
            var vm = new OEMsViewModel();

            // 1. Obtenemos los clientes de la BD como antes
            var allOEMs = db.CTZ_OEMClients.Include(c => c.CTZ_Countries).OrderBy(c => c.Client_Name).ToList();

            // 2. ===== ¡LA MAGIA OCURRE AQUÍ! =====
            //    Transformamos la lista compleja en una lista simple usando LINQ.
            //    Seleccionamos explícitamente solo los campos que necesitamos.
            var simpleClientList = allOEMs.Select(c => new {
                c.ID_OEM,
                c.Client_Name,
                c.Clave_SAP,
                c.Telephone,
                c.Direccion,
                c.Calle,
                c.Ciudad,
                c.Estado,
                c.Codigo_Postal,
                c.Automotriz,
                c.Active,
                c.ID_Country,
                // Aplanamos la relación: en lugar de un objeto Country completo, solo pasamos el nombre.
                CountryName = c.CTZ_Countries != null ? c.CTZ_Countries.Nicename : ""
            }).ToList();

            // 3. Pasamos esta nueva lista simple a la vista a través del ViewBag o un nuevo campo en el ViewModel.
            //    Vamos a usar ViewBag para simplicidad.
            ViewBag.OEMListDataForJs = JsonConvert.SerializeObject(simpleClientList);

            // El resto de tu ViewModel sigue igual
            vm.OEMList = allOEMs; // Para la tabla si la usas desde el modelo
            vm.CountryList = new SelectList(db.CTZ_Countries.Where(c => c.Active).OrderBy(c => c.Nicename), "ID_Country", "Nicename");

            // Pasamos los mensajes de éxito/error si existen
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(vm);
        }

        // POST: CTZ_Catalogs/SaveClient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOEM(CTZ_OEMClients oem) // El modelo que viene del formulario
        {
            // --- PASO 1: Lógica de Validación de Clave SAP Duplicada ---
            // Solo validamos si el usuario proporcionó una Clave SAP.
            if (!string.IsNullOrEmpty(oem.Clave_SAP))
            {
                // La consulta busca si existe OTRO cliente (ID_OEM diferente) con la misma Clave_SAP.
                // Esto es crucial para que no se marque a sí mismo como duplicado al editar.
                bool isDuplicate = db.CTZ_OEMClients.Any(c => c.Clave_SAP == oem.Clave_SAP && c.ID_OEM != oem.ID_OEM);

                if (isDuplicate)
                {
                    // Si encontramos un duplicado, añadimos un error específico al campo 'Clave_SAP'.
                    // La vista usará esto para mostrar el mensaje de error junto al campo correcto.
                    ModelState.AddModelError("Clave_SAP", "This SAP Key is already in use by another client.");
                }
            }

            // --- PASO 2: Verificar si el Modelo es Válido en General ---
            // ModelState.IsValid ahora incluye nuestros errores personalizados y los de DataAnnotations (ej. [Required]).
            if (ModelState.IsValid)
            {
                try
                {
                    // --- PASO 3: Lógica de Guardado (Crear o Actualizar) ---
                    if (oem.ID_OEM == 0) // Es un nuevo cliente porque el ID es 0
                    {
                        oem.Active = true; // Por defecto, los nuevos clientes están activos
                        db.CTZ_OEMClients.Add(oem);
                        TempData["SuccessMessage"] = "OEM created successfully.";
                    }
                    else // Es una actualización de un cliente existente
                    {
                        // Usar db.Entry es la forma recomendada por Entity Framework para actualizaciones.
                        db.Entry(oem).State = EntityState.Modified;
                        TempData["SuccessMessage"] = "OEM updated successfully.";
                    }

                    db.SaveChanges();
                    // Si todo sale bien, redirigimos al listado principal.
                    return RedirectToAction("OEMs");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while saving: " + ex.Message;
                    return RedirectToAction("OEMs");
                }
            }

            // --- PASO 4: Manejo de Errores de Validación ---
            // Si llegamos aquí, es porque ModelState.IsValid fue 'false'.
            // NO redirigimos. En su lugar, volvemos a mostrar la vista del formulario
            // para que el usuario pueda ver los mensajes de error y corregir los datos.

            TempData["ErrorMessage"] = "The provided information is invalid. Please review the fields.";

            // Para que la vista se renderice correctamente, debemos volver a cargar los datos que necesita,
            // como las listas de los DropDowns y la tabla de clientes.
            var vm = new OEMsViewModel
            {
                OEM = oem, // Devolvemos el objeto con los datos que el usuario ya había llenado.
                CountryList = new SelectList(db.CTZ_Countries.Where(c => c.Active).OrderBy(c => c.Nicename), "ID_Country", "Nicename", oem.ID_Country)
            };

            // También necesitamos volver a cargar los datos para la tabla de Handsontable
            var allOEMs = db.CTZ_OEMClients.Include(c => c.CTZ_Countries).OrderBy(c => c.Client_Name).ToList();
            var simpleClientList = allOEMs.Select(c => new {
                c.ID_OEM,
                c.Client_Name,
                c.Clave_SAP,
                c.Telephone,
                c.Direccion,
                c.Calle,
                c.Ciudad,
                c.Estado,
                c.Codigo_Postal,
                c.Automotriz,
                c.Active,
                c.ID_Country,
                CountryName = c.CTZ_Countries != null ? c.CTZ_Countries.Nicename : ""
            }).ToList();
            ViewBag.OEMListDataForJs = JsonConvert.SerializeObject(simpleClientList);


            return View("OEMs", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadOEMs(HttpPostedFileBase excelFile)
        {
            if (excelFile == null || excelFile.ContentLength == 0)
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return RedirectToAction("OEMs");
            }

            try
            {
                using (var stream = excelFile.InputStream)
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // 1. LEER EL EXCEL Y CONVERTIRLO A UNA LISTA
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                        });

                        System.Data.DataTable dataTable = result.Tables[0];
                        var excelOEMs = new List<Dictionary<string, string>>();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            var dict = new Dictionary<string, string>();
                            foreach (DataColumn col in dataTable.Columns)
                            {
                                dict[col.ColumnName] = row[col].ToString();
                            }
                            excelOEMs.Add(dict);
                        }

                        // 2. PREPARAR DATOS DE LA BD PARA BÚSQUEDAS RÁPIDAS
                        // Diccionario de clientes locales por Clave SAP
                        var oemsList1 = db.CTZ_OEMClients
                                  .Where(c => !string.IsNullOrEmpty(c.Clave_SAP)).ToList();

                        var localOemsDict = db.CTZ_OEMClients
                          .Where(c => !string.IsNullOrEmpty(c.Clave_SAP))
                          .GroupBy(c => c.Clave_SAP) // 1. Agrupa por la clave SAP
                          .ToDictionary(g => g.Key, g => g.First()); // 2. De cada grupo, toma el primero

                        // Diccionario de países por código ISO
                        var countriesDict = db.CTZ_Countries.Where(c => c.ISO != null).ToDictionary(c => c.ISO.ToUpper(), c => c.ID_Country);
                        // Conjunto de IDs de clientes que están siendo usados en Proyectos
                        var referencedOEMsIds = db.CTZ_Projects.Where(p => p.ID_OEM.HasValue).Select(p => p.ID_OEM.Value).ToHashSet();

                        var excelSapKeys = new HashSet<string>();
                        int createdCount = 0, updatedCount = 0;

                        // 3. PROCESAR FILAS DEL EXCEL (CREAR Y ACTUALIZAR)
                        // ===== INICIO DEL BLOQUE CORREGIDO =====
                        foreach (var excelRow in excelOEMs)
                        {
                            // Usamos los encabezados de tu Excel, asegurando que no haya espacios
                            string sapKey = excelRow["Customer"].Trim();
                            if (string.IsNullOrEmpty(sapKey)) continue;

                            excelSapKeys.Add(sapKey);

                            // Enlace automático del país usando la columna "Cty"
                            int? countryId = null;
                            string countryIso = excelRow["Cty"].Trim().ToUpper();
                            if (countriesDict.ContainsKey(countryIso))
                            {
                                countryId = countriesDict[countryIso];
                            }

                            // --- LÓGICA DE CONCATENACIÓN DE DIRECCIÓN ---
                            var addressParts = new[]
                            {
                                excelRow["Street"],
                                excelRow["City"],
                                excelRow["Rg"], // 'Rg' (Región) en Excel mapea a 'Estado' en tu BD
                                excelRow["PostalCode"]
                            };
                            string fullAddress = string.Join(", ", addressParts.Where(s => !string.IsNullOrEmpty(s.Trim())));
                            // ---------------------------------------------

                            if (localOemsDict.ContainsKey(sapKey)) // El cliente existe -> ACTUALIZAR
                            {
                                var OemInDb = localOemsDict[sapKey];
                                OemInDb.Client_Name = excelRow["Name 1"];
                                OemInDb.Pais = excelRow["Name"]; // El nombre del país viene de la columna 'Name'
                                OemInDb.Calle = excelRow["Street"]; // El campo 'Calle' es solo la calle
                                OemInDb.Direccion = fullAddress;    // El campo 'Direccion' es la concatenación
                                OemInDb.Ciudad = excelRow["City"];
                                OemInDb.Codigo_Postal = excelRow["PostalCode"];
                                OemInDb.Estado = excelRow["Rg"];
                                OemInDb.Telephone = excelRow["Telephone 1"];
                                OemInDb.ID_Country = countryId;
                                // La lógica para 'Automotriz' y 'Active' se mantiene como estaba o se puede añadir si viene en el Excel

                                db.Entry(OemInDb).State = EntityState.Modified;
                                updatedCount++;
                            }
                            else // El cliente no existe -> CREAR
                            {
                                var newOEM = new CTZ_OEMClients
                                {
                                    Clave_SAP = sapKey,
                                    Client_Name = excelRow["Name 1"],
                                    Pais = excelRow["Name"],
                                    Calle = excelRow["Street"],
                                    Direccion = fullAddress,
                                    Ciudad = excelRow["City"],
                                    Codigo_Postal = excelRow["PostalCode"],
                                    Estado = excelRow["Rg"],
                                    Telephone = excelRow["Telephone 1"],
                                    ID_Country = countryId,
                                    Active = true,
                                    Automotriz = false // Asumimos 'false' por defecto para nuevos clientes
                                };
                                db.CTZ_OEMClients.Add(newOEM);
                                createdCount++;
                            }
                        }
                        // ===== FIN DEL BLOQUE CORREGIDO =====

                        int deactivatedCount = 0;
                        int deletedCount = 0;

                        // Obtenemos los clientes que están en nuestra BD pero no en el nuevo Excel
                        var oemsToRemoveOrDeactivate = db.CTZ_OEMClients
                            .Where(c => !string.IsNullOrEmpty(c.Clave_SAP) && !excelSapKeys.Contains(c.Clave_SAP))
                            .ToList();

                        foreach (var oem in oemsToRemoveOrDeactivate)
                        {
                            // Verificamos si este cliente está siendo usado en la tabla de Proyectos
                            if (referencedOEMsIds.Contains(oem.ID_OEM))
                            {
                                // Si está referenciado, NO PODEMOS BORRARLO.
                                // Solo lo desactivamos si es que estaba activo.
                                if (oem.Active)
                                {
                                    oem.Active = false;
                                    db.Entry(oem).State = EntityState.Modified;
                                    deactivatedCount++;
                                }
                            }
                            else
                            {
                                // Si NO está referenciado en ninguna parte, lo borramos físicamente
                                db.CTZ_OEMClients.Remove(oem);
                                deletedCount++;
                            }
                        }
                        // ===== FIN DE LA LÓGICA CORREGIDA =====


                        db.SaveChanges();
                        TempData["SuccessMessage"] = $"Process complete. Created: {createdCount}, Updated: {updatedCount}, Deactivated (in use): {deactivatedCount}, Deleted: {deletedCount}.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error processing file: " + ex.Message;
            }

            return RedirectToAction("OEMs");
        }

        // POST: CTZ_Catalogs/DeactivateOem/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeactivateOem(int id)
        {
            try
            {
                var oem = db.CTZ_OEMClients.Find(id);
                if (oem == null)
                {
                    return Json(new { success = false, message = "OEM not found." });
                }
                oem.Active = false;
                db.SaveChanges();
                return Json(new { success = true, message = "OEM deactivated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        // POST: CTZ_Catalogs/ReactivateOEM/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ReactivateOEM(int id)
        {
            try
            {
                var oem = db.CTZ_OEMClients.Find(id);
                if (oem == null)
                {
                    return Json(new { success = false, message = "OEM not found." });
                }
                oem.Active = true; // <-- La única diferencia: lo ponemos en true
                db.SaveChanges();
                return Json(new { success = true, message = "OEM reactivated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        #endregion

        #region PackagingCatalogs

        // GET: CTZ_Catalogs/ManagePackagingCatalogs
        public ActionResult ManagePackagingCatalogs()
        {
            // Lógica de permisos
            int me = obtieneEmpleadoLogeado().id;
            var auth = new AuthorizationService(db);
            bool canView = auth.CanPerform(me, ResourceKey.CatalogsEngineering, ActionKey.View);
            if (!canView)
            {
                return View("../Home/ErrorPermisos");
            }
            ViewBag.CanEdit = auth.CanPerform(me, ResourceKey.CatalogsEngineering, ActionKey.Edit);

            var vm = new PackagingCatalogsViewModel();

            // ===== INICIO DE LA MODIFICACIÓN: USAR EL DTO =====

            // Proyectamos cada lista a nuestro DTO para estandarizar los nombres de las propiedades
            var additionalsList = db.CTZ_Packaging_Additionals
                .OrderBy(a => a.AdditionalName)
                .Select(a => new CatalogItemDto { ID = a.ID_Additional, Name = a.AdditionalName, IsActive = a.IsActive })
                .ToList();

            var labelTypesList = db.CTZ_Packaging_LabelType
                .OrderBy(l => l.LabelTypeName)
                .Select(l => new CatalogItemDto { ID = l.ID_LabelType, Name = l.LabelTypeName, IsActive = l.IsActive })
                .ToList();

            var rackTypesList = db.CTZ_Packaging_RackType
                .OrderBy(r => r.RackTypeName)
                .Select(r => new CatalogItemDto { ID = r.ID_RackType, Name = r.RackTypeName, IsActive = r.IsActive })
                .ToList();

            var strapTypesList = db.CTZ_Packaging_StrapType
                .OrderBy(s => s.StrapTypeName)
                .Select(s => new CatalogItemDto { ID = s.ID_StrapType, Name = s.StrapTypeName, IsActive = s.IsActive })
                .ToList();

            // Serializamos las listas de DTOs. Ya no hay riesgo de referencias circulares.
            ViewBag.AdditionalsJson = JsonConvert.SerializeObject(additionalsList);
            ViewBag.LabelTypesJson = JsonConvert.SerializeObject(labelTypesList);
            ViewBag.RacksJson = JsonConvert.SerializeObject(rackTypesList);
            ViewBag.StrapsJson = JsonConvert.SerializeObject(strapTypesList);

            // ===== FIN DE LA MODIFICACIÓN =====

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View(vm);
        }

        // POST: Guarda o actualiza un item de CUALQUIER catálogo de packaging
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SavePackagingItem(string catalogType, int id, string name, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
            {
                return Json(new { success = false, message = "Name is required and cannot exceed 50 characters." });
            }

            try
            {
                switch (catalogType)
                {
                    case "Additional":
                        if (id == 0) // Crear nuevo
                        {
                            db.CTZ_Packaging_Additionals.Add(new CTZ_Packaging_Additionals { AdditionalName = name, IsActive = isActive });
                        }
                        else // Editar existente
                        {
                            var item = db.CTZ_Packaging_Additionals.Find(id);
                            if (item == null) return Json(new { success = false, message = "Item not found." });
                            item.AdditionalName = name;
                            item.IsActive = isActive;
                        }
                        break;

                    case "LabelType":
                        if (id == 0) // Crear nuevo
                        {
                            db.CTZ_Packaging_LabelType.Add(new CTZ_Packaging_LabelType { LabelTypeName = name, IsActive = isActive });
                        }
                        else // Editar existente
                        {
                            var item = db.CTZ_Packaging_LabelType.Find(id);
                            if (item == null) return Json(new { success = false, message = "Item not found." });
                            item.LabelTypeName = name;
                            item.IsActive = isActive;
                        }
                        break;

                    case "RackType":
                        if (id == 0) // Crear nuevo
                        {
                            db.CTZ_Packaging_RackType.Add(new CTZ_Packaging_RackType { RackTypeName = name, IsActive = isActive });
                        }
                        else // Editar existente
                        {
                            var item = db.CTZ_Packaging_RackType.Find(id);
                            if (item == null) return Json(new { success = false, message = "Item not found." });
                            item.RackTypeName = name;
                            item.IsActive = isActive;
                        }
                        break;

                    case "StrapType":
                        if (id == 0) // Crear nuevo
                        {
                            db.CTZ_Packaging_StrapType.Add(new CTZ_Packaging_StrapType { StrapTypeName = name, IsActive = isActive });
                        }
                        else // Editar existente
                        {
                            var item = db.CTZ_Packaging_StrapType.Find(id);
                            if (item == null) return Json(new { success = false, message = "Item not found." });
                            item.StrapTypeName = name;
                            item.IsActive = isActive;
                        }
                        break;

                    default:
                        return Json(new { success = false, message = "Invalid catalog type provided." });
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Item saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        // POST: Elimina o desactiva un item de CUALQUIER catálogo de packaging
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePackagingItem(string catalogType, int id)
        {
            try
            {
                bool isReferenced = false;
                object itemToDelete = null;

                switch (catalogType)
                {
                    case "Additional":
                        // Verifica si el ID existe en la tabla actual O en la de historial
                        isReferenced = db.CTZ_Material_Additionals.Any(m => m.ID_Additional == id) ||
                                       db.CTZ_Material_Additionals_History.Any(h => h.ID_Additional == id);
                        itemToDelete = db.CTZ_Packaging_Additionals.Find(id);
                        break;

                    case "LabelType":
                        // Verifica si el ID existe en la tabla actual O en la de historial
                        isReferenced = db.CTZ_Material_Labels.Any(m => m.ID_LabelType == id) ||
                                       db.CTZ_Material_Labels_History.Any(h => h.ID_LabelType == id);
                        itemToDelete = db.CTZ_Packaging_LabelType.Find(id);
                        break;

                    case "RackType":
                        // Verifica si el ID existe en la tabla actual O en la de historial
                        isReferenced = db.CTZ_Material_RackTypes.Any(m => m.ID_RackType == id) ||
                                       db.CTZ_Material_RackTypes_History.Any(h => h.ID_RackType == id);
                        itemToDelete = db.CTZ_Packaging_RackType.Find(id);
                        break;

                    case "StrapType":
                        // Verifica si el ID existe en la tabla actual O en la de historial
                        isReferenced = db.CTZ_Material_StrapTypes.Any(m => m.ID_StrapType == id) ||
                                       db.CTZ_Material_StrapTypes_History.Any(h => h.ID_StrapType == id);
                        itemToDelete = db.CTZ_Packaging_StrapType.Find(id);
                        break;

                    default:
                        return Json(new { success = false, message = "Invalid catalog type." });
                }

                if (itemToDelete == null)
                {
                    return Json(new { success = false, message = "Item not found." });
                }

                if (isReferenced)
                {
                    // Si está referenciado, solo lo desactivamos
                    // Usamos reflexión para establecer la propiedad 'IsActive' genéricamente
                    itemToDelete.GetType().GetProperty("IsActive").SetValue(itemToDelete, false);
                    db.Entry(itemToDelete).State = EntityState.Modified;
                }
                else
                {
                    // Si no está referenciado, lo borramos
                    db.Entry(itemToDelete).State = EntityState.Deleted;
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Operation completed successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ToggleItemActive(string catalogType, int id)
        {
            try
            {
                object item = null;
                bool newState = false;

                switch (catalogType)
                {
                    case "Additional":
                        var additional = db.CTZ_Packaging_Additionals.Find(id);
                        if (additional != null) { additional.IsActive = !additional.IsActive; newState = additional.IsActive; item = additional; }
                        break;
                    case "LabelType":
                        var label = db.CTZ_Packaging_LabelType.Find(id);
                        if (label != null) { label.IsActive = !label.IsActive; newState = label.IsActive; item = label; }
                        break;
                    case "RackType":
                        var rack = db.CTZ_Packaging_RackType.Find(id);
                        if (rack != null) { rack.IsActive = !rack.IsActive; newState = rack.IsActive; item = rack; }
                        break;
                    case "StrapType":
                        var strap = db.CTZ_Packaging_StrapType.Find(id);
                        if (strap != null) { strap.IsActive = !strap.IsActive; newState = strap.IsActive; item = strap; }
                        break;
                }

                if (item == null)
                {
                    return Json(new { success = false, message = "Item not found." });
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Status updated successfully.", newActiveState = newState });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }


    public class PackagingCatalogsViewModel
    {
        public List<CTZ_Packaging_Additionals> Additionals { get; set; }
        public List<CTZ_Packaging_LabelType> LabelTypes { get; set; }
        public List<CTZ_Packaging_RackType> RackTypes { get; set; }
        public List<CTZ_Packaging_StrapType> StrapTypes { get; set; }

        public PackagingCatalogsViewModel()
        {
            Additionals = new List<CTZ_Packaging_Additionals>();
            LabelTypes = new List<CTZ_Packaging_LabelType>();
            RackTypes = new List<CTZ_Packaging_RackType>();
            StrapTypes = new List<CTZ_Packaging_StrapType>();
        }
    }
    // Un objeto simple para transportar los datos de cualquier catálogo
    public class CatalogItemDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class TechnicalInformationDTO
    {
        public string PlantName { get; set; }
        public string LineName { get; set; }
        public string ID_L { get; set; }

        public int ID_Material_Type { get; set; }
        public string Material_Name { get; set; }
        public int ID_Technical_Information { get; set; }
        public int ID_Line { get; set; } // Lo mantenemos para el guardado
        public int ID_Criteria { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public string TextValue { get; set; }
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public double? NumericValue { get; set; }
        public double? Tolerance { get; set; }
        public bool IsActive { get; set; }
        public bool HasTolerance { get; set; }

    }

    /// <summary>
    /// DTO para recibir los datos editados desde Handsontable.
    /// </summary>
    public class EngineeringDimensionDTO
    {
        public int ID_Engineering_Dimension { get; set; }
        public int ID_Line { get; set; }
        public string Criteria { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
    }

    public class StrokeMatrixDTO
    {
        public int ManufacturerId { get; set; }
        public double Rotation { get; set; }
        public double Advance { get; set; }
        public double? Strokes { get; set; }
    }

    // ===================================
    // DTOs
    // ===================================
    // Fila que se muestra en Handsontable
    public class HoursByLineRowDTO
    {
        public string PlantName { get; set; }
        public int ID_Line { get; set; }
        public string LineName { get; set; }
        public int ID_Status { get; set; }
        public string StatusDescription { get; set; }
        // Mapa de <ID_Fiscal_Year, Hours>
        public Dictionary<string, double?> HoursByFY { get; set; }
    }

    public class OeeRowDto
    {
        public int ID_Line { get; set; }
        public string LineName { get; set; }
        public Dictionary<string, double?> ValuesByMonth { get; set; }
    }

    public class SaveOeeDto
    {
        public int Year { get; set; }
        public List<OeeRowDto> Rows { get; set; }
    }

    // Estructura para guardar
    public class SaveHoursDTO
    {
        public List<int> FiscalYearIDs { get; set; }
        public List<HoursByLineRowDTO> Rows { get; set; }
    }

    public class EmployeeAssignmentsViewModel
    {
        // ——— Planta ———
        [Display(Name = "Plant")]
        public int? SelectedPlantId { get; set; }
        public List<int> EmployeePlants { get; set; }
        public List<SelectListItem> Plants { get; set; }

        // ——— Departamento ———
        [Display(Name = "Department")]
        public int? SelectedDepartmentId { get; set; }
        public List<int> EmployeeDepartments { get; set; }
        public List<SelectListItem> Departments { get; set; }

        // ——— Empleados (para ambos multiselect) ———
        public IEnumerable<SelectListItem> Employees { get; set; }

        public EmployeeAssignmentsViewModel()
        {
            EmployeePlants = new List<int>();
            EmployeeDepartments = new List<int>();
            Plants = new List<SelectListItem>();
            Departments = new List<SelectListItem>();
        }
    }

    public class CountryToggleViewModel
    {
        public int ID_Country { get; set; }
        public string ISO3 { get; set; }
        public string Nicename { get; set; }
        public bool Active { get; set; }
        public bool Warning { get; set; }
    }
    public class PermissionManagementViewModel
    {
        public IEnumerable<CTZ_Roles> Roles { get; set; }
        public IEnumerable<CTZ_Resources> Resources { get; set; }
        public IEnumerable<CTZ_Actions> Actions { get; set; }
        public IEnumerable<CTZ_Conditions> Conditions { get; set; }
        public IEnumerable<CTZ_Role_Permissions> RolePermissions { get; set; }
        public IEnumerable<CTZ_Role_Permission_Conditions> RolePermissionConditions { get; set; }

        public IEnumerable<empleados> Employees { get; set; }
        public int SelectedRoleId { get; set; }
    }

    public class RejectionReasonsPageViewModel
    {
        public RejectionReasonViewModel Current { get; set; }
            = new RejectionReasonViewModel();
        public List<RejectionReasonViewModel> Reasons { get; set; }
            = new List<RejectionReasonViewModel>();
    }

    public class RejectionReasonViewModel
    {
        public int ID_Reason { get; set; }
        public string Name { get; set; }
        public ActionTypeEnum ActionType { get; set; }
        public int? ReassignDepartmentId { get; set; }
        public List<int> DepartmentIds { get; set; } = new List<int>();
        // for display only
        public string ReassignDepartmentName { get; set; }
        public List<string> Departments { get; set; } = new List<string>();
        public bool Active { get; set; }
    }

    public class ClientsViewModel
    {
        // Para el formulario de edición/creación
        public CTZ_Clients Client { get; set; }

        // Para la lista que mostrará Handsontable
        public List<CTZ_Clients> ClientList { get; set; }

        // Para el dropdown de países
        public SelectList CountryList { get; set; }

        public ClientsViewModel()
        {
            Client = new CTZ_Clients();
            ClientList = new List<CTZ_Clients>();
            CountryList = new SelectList(new List<object>());
        }
    }

    public class OEMsViewModel
    {
        // Para el formulario de edición/creación
        public CTZ_OEMClients OEM { get; set; }

        // Para la lista que mostrará Handsontable
        public List<CTZ_OEMClients> OEMList { get; set; }

        // Para el dropdown de países
        public SelectList CountryList { get; set; }

        public OEMsViewModel()
        {
            OEM = new CTZ_OEMClients();
            OEMList = new List<CTZ_OEMClients>();
            CountryList = new SelectList(new List<object>());
        }
    }

    // DTO auxiliar para recibir y enviar datos
    public class TotalTimeDto
    {
        public int ID_Fiscal_Year { get; set; }
        public string Fiscal_Year_Name { get; set; }
        public double? Value { get; set; }
    }
}