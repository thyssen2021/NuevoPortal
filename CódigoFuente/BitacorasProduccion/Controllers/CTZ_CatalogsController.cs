using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            return View();
        }

        [Authorize]
        public ActionResult strokes_per_minute()
        {
            // Verificar rol
            if (!TieneRol(TipoRoles.ADMIN))
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
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            // Cargamos las plantas activas
            var plants = db.CTZ_plants
                           .Where(p => p.Active)
                           .OrderBy(p => p.Description)
                           .ToList();

            ViewBag.Plants = new SelectList(plants, "ID_Plant", "Description");

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
            // Verificar rol
            if (!TieneRol(TipoRoles.ADMIN))
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


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
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

    // Estructura para guardar
    public class SaveHoursDTO
    {
        public List<int> FiscalYearIDs { get; set; }
        public List<HoursByLineRowDTO> Rows { get; set; }
    }
}