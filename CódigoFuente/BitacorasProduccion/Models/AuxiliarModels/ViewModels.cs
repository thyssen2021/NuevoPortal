using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Models
{
    public class ProjectIndexViewModel
    {
        public int ID_Project { get; set; }
        public string ConcatQuoteID { get; set; }
        public int ID_Created_By { get; set; }
        public int ID_Plant { get; set; }

        // Será true si puede editar según tus reglas
        public bool CanEdit { get; set; }
    }
    /// <summary>
    /// Define la estructura de los rechazos en la vista
    /// </summary>
    public class ActiveRejection
    {
        public string Dept { get; set; }
        public string Comment { get; set; }
        public DateTime DateRejection { get; set; }
    }

    /// <summary>
    /// Define la estructura de los deptos a los que se puede rechazar
    /// </summary>
    public class DeptReassignOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AssignmentHistoryViewModel
    {
        public int ID_Assignment { get; set; }
        public string DepartmentName { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string StatusName { get; set; }
        public string ClosedBy { get; set; }
        public bool WasRejected { get; set; }
        public string RejectionReason { get; set; }
        public string Comments { get; set; }
    }

    public class GanttEntryViewModel
    {
        public string Department { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Completed { get; set; }
    }
    public class EditProjectViewModel
    {
        /* Referencia al projecto */       
        public CTZ_Projects CTZ_Project { get; set; }
        public CTZ_Project_Status CTZ_Project_Status { get; set; }
        public empleados empleados { get; set; }

        //valores de CTZ_Proyects_Versions
        public int? ID_Version { get; set; }
        public Nullable<int> ID_Project { get; set; }
        public Nullable<int> ID_Created_by { get; set; }
        public string Version_Number { get; set; }
        public Nullable<System.DateTime> Creation_Date { get; set; }
        public Nullable<bool> Is_Current { get; set; }
        public string Comments { get; set; }
        public Nullable<int> ID_Status_Project { get; set; }
            

        // Materiales (vivos o snapshot):
        public List<MaterialViewModel> Materials { get; set; }

        // Para controlar la UI:
        public bool IsSnapshot => ID_Version.HasValue ;
    }

    public partial class MaterialViewModel
    {
        [Display(Name = "ID")]
        public int ID_Material { get; set; }
        [Display(Name = "IHS")]
        public Nullable<int> ID_IHS_Item { get; set; }
        [Display(Name = "Max Production S&P")]
        public Nullable<int> Max_Production_SP { get; set; }
        [Display(Name = "Program S&P")]
        public string Program_SP { get; set; }
        [Display(Name = "Vehicle Version")]
        public string Vehicle_version { get; set; }
        [Display(Name = "SOP (S&P)")]
        public Nullable<System.DateTime> SOP_SP { get; set; }
        [Display(Name = "EOP (S&P)")]
        public Nullable<System.DateTime> EOP_SP { get; set; }
        [Display(Name = "Real SOP")]
        public Nullable<System.DateTime> Real_SOP { get; set; }
        [Display(Name = "Real EOP")]
        public Nullable<System.DateTime> Real_EOP { get; set; }
        [Display(Name = "Ship To")]
        public string Ship_To { get; set; }
        [Display(Name = "Part Name")]
        public string Part_Name { get; set; }
        [Display(Name = "Part Number")]
        public string Part_Number { get; set; }
        [Display(Name = "Route")]
        public Nullable<int> ID_Route { get; set; }
        [Display(Name = "Quality")]
        public string Quality { get; set; }
        [Display(Name = "Tensile Strenght")]
        public Nullable<double> Tensile_Strenght { get; set; }
        [Display(Name = "Material Type")]
        public Nullable<int> ID_Material_type { get; set; }
        [Display(Name = "Thickness")]
        public Nullable<double> Thickness { get; set; }
        public Nullable<double> Width { get; set; }
        public Nullable<double> Pitch { get; set; }
        [Display(Name = "Theoretical Gross Weight")]
        public Nullable<double> Theoretical_Gross_Weight { get; set; }
        [Display(Name = "Gross Weight")]
        public Nullable<double> Gross_Weight { get; set; }
        [Display(Name = "Annual Volume")]
        public Nullable<int> Annual_Volume { get; set; }
        [Display(Name = "Volume (TN)/year")]
        public Nullable<double> Volume_Per_year { get; set; }
        [Display(Name = "Shape")]
        public Nullable<int> ID_Shape { get; set; }
        [Display(Name = "Angle A")]
        public Nullable<double> Angle_A { get; set; }
        [Display(Name = "Angle B")]
        public Nullable<double> Angle_B { get; set; }
        [Display(Name = "Blanks Per Stroke")]
        public Nullable<double> Blanks_Per_Stroke { get; set; }
        [Display(Name = "Parts Per Vehicle")]
        public Nullable<double> Parts_Per_Vehicle { get; set; }
        [Display(Name = "Theoretical Blanking Line")]
        public Nullable<int> ID_Theoretical_Blanking_Line { get; set; }
        [Display(Name = "Real Blanking Line")]
        public Nullable<int> ID_Real_Blanking_Line { get; set; }
        [Display(Name = "Theoretical Strokes")]
        public Nullable<double> Theoretical_Strokes { get; set; }
        [Display(Name = "Real Strokes")]
        public Nullable<double> Real_Strokes { get; set; }
        [Display(Name = "Ideal Cycle Time Per Tool")]
        public Nullable<double> Ideal_Cycle_Time_Per_Tool { get; set; }
        [Display(Name = "% OEE")]
        public Nullable<double> OEE { get; set; }
        [Display(Name = "ID Project")]
        public int ID_Project { get; set; }
        [Display(Name = "Vehicle 1")]
        public string Vehicle { get; set; }
        [Display(Name = "Vehicle 2")]
        public string Vehicle_2 { get; set; }
        [Display(Name = "Vehicle 3")]
        public string Vehicle_3 { get; set; }
        [Display(Name = "Vehicle 4")]
        public string Vehicle_4 { get; set; }
        [Display(Name = "Thickness Tolerance Negative")]
        public Nullable<double> ThicknessToleranceNegative { get; set; }
        [Display(Name = "Thickness Tolerance Positive")]
        public Nullable<double> ThicknessTolerancePositive { get; set; }
        [Display(Name = "Width Tolerance Negative")]
        public Nullable<double> WidthToleranceNegative { get; set; }
        [Display(Name = "Width Tolerance Positive")]
        public Nullable<double> WidthTolerancePositive { get; set; }
        [Display(Name = "Pitch Tolerance Negative")]
        public Nullable<double> PitchToleranceNegative { get; set; }
        [Display(Name = "Pitch Tolerance Positive")]
        public Nullable<double> PitchTolerancePositive { get; set; }
        [Display(Name = "Weight of Final Mults")]
        public Nullable<double> WeightOfFinalMults { get; set; }
        [Display(Name = "Multipliers")]
        public Nullable<double> Multipliers { get; set; }
        [Display(Name = "Angle A Tolerance Negative")]
        public Nullable<double> AngleAToleranceNegative { get; set; }
        [Display(Name = "Angle A Tolerance Positive")]
        public Nullable<double> AngleATolerancePositive { get; set; }
        [Display(Name = "Angle B Tolerance Negative")]
        public Nullable<double> AngleBToleranceNegative { get; set; }
        [Display(Name = "Angle B Tolerance Positive")]
        public Nullable<double> AngleBTolerancePositive { get; set; }
        [Display(Name = "Major Base")]
        public Nullable<double> MajorBase { get; set; }
        [Display(Name = "Major Base Tolerance Negative")]
        public Nullable<double> MajorBaseToleranceNegative { get; set; }
        [Display(Name = "Major Base Tolerance Positive")]
        public Nullable<double> MajorBaseTolerancePositive { get; set; }
        [Display(Name = "Minor Base")]
        public Nullable<double> MinorBase { get; set; }
        [Display(Name = "Minor Base Tolerance Negative")]
        public Nullable<double> MinorBaseToleranceNegative { get; set; }
        [Display(Name = "Minor Base Tolerance Positive")]
        public Nullable<double> MinorBaseTolerancePositive { get; set; }
        [Display(Name = "Flatness")]
        public Nullable<double> Flatness { get; set; }
        [Display(Name = "Flatness Tolerance Negative")]
        public Nullable<double> FlatnessToleranceNegative { get; set; }
        [Display(Name = "Flatness Tolerance Positive")]
        public Nullable<double> FlatnessTolerancePositive { get; set; }
        [Display(Name = "Master Coil Weight")]
        public Nullable<double> MasterCoilWeight { get; set; }
        [Display(Name = "Inner Coil Diameter Arrival")]
        public Nullable<double> InnerCoilDiameterArrival { get; set; }
        [Display(Name = "Outer Coil Diameter Arrival")]
        public Nullable<double> OuterCoilDiameterArrival { get; set; }
        [Display(Name = "Inner Coil Diameter Delivery")]
        public Nullable<double> InnerCoilDiameterDelivery { get; set; }
        [Display(Name = "Outer Coil Diameter Delivery")]
        public Nullable<double> OuterCoilDiameterDelivery { get; set; }
        [Display(Name = "Packaging Standard")]
        public string PackagingStandard { get; set; }
        [Display(Name = "Special Requirement")]
        public string SpecialRequirement { get; set; }
        [Display(Name = "Special Packaging")]
        public string SpecialPackaging { get; set; }
        [Display(Name = "CAD Drawing File")]
        public Nullable<int> ID_File_CAD_Drawing { get; set; }
        [Display(Name = "¿Turn Over?")]
        public Nullable<bool> TurnOver { get; set; }

        [Display(Name = "Side")]
        public string TurnOverSide { get; set; }

        [Display(Name = "DM Status")]
        public string DM_status { get; set; }

        [Display(Name = "DM Comment")]
        public string DM_status_comment { get; set; }

        [Display(Name = "Mults Width")]
        public Nullable<double> Width_Mults { get; set; }

        [Display(Name = "Mults Width Tol. (-)")]
        public Nullable<double> Width_Mults_Tol_Neg { get; set; }

        [Display(Name = "Mults Width Tol. (+)")]
        public Nullable<double> Width_Mults_Tol_Pos { get; set; }

        [Display(Name = "Plates Width")]
        public Nullable<double> Width_Plates { get; set; }

        [Display(Name = "Plates Width Tol. (-)")]
        public Nullable<double> Width_Plates_Tol_Neg { get; set; }

        [Display(Name = "Plates Width Tol. (+)")]
        public Nullable<double> Width_Plates_Tol_Pos { get; set; }

        [Display(Name = "Initial Weight [kg]")]
        public Nullable<double> Initial_Weight { get; set; }

        public virtual CTZ_Files CTZ_Files { get; set; }
        public virtual CTZ_Material_Type CTZ_Material_Type { get; set; }
        public virtual CTZ_Production_Lines CTZ_Production_Lines { get; set; }
        public virtual CTZ_Production_Lines CTZ_Production_Lines1 { get; set; }
        public virtual CTZ_Projects CTZ_Projects { get; set; }
        public virtual CTZ_Route CTZ_Route { get; set; }
        public virtual SCDM_cat_forma_material SCDM_cat_forma_material { get; set; }
    }

    public partial class MaterialViewModel
    {
        [NotMapped]
        public bool? IsFile { get; set; }

        [NotMapped]
        public string CADFileName { get; set; }

        /// <summary>
        /// Equivale a "Parts per Auto".
        /// Fórmula: se toma directamente la propiedad Parts_Per_Vehicle (si es null, 0).
        /// </summary>
        [NotMapped]
        public double PartsPerAuto
        {
            get
            {
                return Parts_Per_Vehicle ?? 0.0;
            }
        }

        /// <summary>
        /// Equivale a "Strokes per Auto".
        /// Fórmula: PartsPerAuto / Blanks_Per_Stroke.
        /// </summary>
        [NotMapped]
        public double StrokesPerAuto
        {
            get
            {
                double blanks = Blanks_Per_Stroke ?? 1.0;
                return PartsPerAuto / blanks;
            }
        }

        /// <summary>
        /// Equivale a "BLANKS PER YEAR [1/1000]".
        /// Fórmula: PartsPerAuto * Annual_Volume .
        /// </summary>
        [NotMapped]
        public double BlanksPerYearThousands
        {
            get
            {
                double annualVol = Annual_Volume ?? 0.0;
                return PartsPerAuto * annualVol;
            }
        }

        /// <summary>
        /// Equivale a "Min. Max. Reales".
        /// Fórmula: BLANKS PER YEAR [1/1000] / Ideal_Cycle_Time_Per_Tool / Blanks_Per_Stroke.
        /// </summary>
        [NotMapped]
        public double RealMinMax
        {
            get
            {
                double ict = Ideal_Cycle_Time_Per_Tool ?? 1.0;
                double blanks = Blanks_Per_Stroke ?? 1.0;
                return BlanksPerYearThousands / (ict * blanks);
            }
        }

        /// <summary>
        /// Equivale a "Min. Reales".
        /// Fórmula: RealMinMax / OEE.
        /// Si OEE es mayor a 1, se asume que está en %, por lo que se divide entre 100.
        /// </summary>
        [NotMapped]
        public double RealMin
        {
            get
            {
                double rawOee = OEE ?? 1.0;
                // Si OEE > 1, se asume porcentaje y se divide entre 100
                double localOee = (rawOee > 1.0) ? (rawOee / 100.0) : rawOee;
                return RealMinMax / localOee;
            }
        }

        /// <summary>
        /// Equivale a "Turno Reales".
        /// Fórmula: Min. Reales / 7.5 / 60.
        /// </summary>
        [NotMapped]
        public double RealShift
        {
            get
            {
                // 7.5 horas * 60 = 450 min
                return RealMin / (7.5 * 60.0);
            }
        }

        /// <summary>
        /// Equivale a "Strokes per Turno".
        /// Fórmula: BLANKS PER YEAR [1/1000] / Blanks_Per_Stroke / Turno Reales.
        /// </summary>
        [NotMapped]
        public double StrokesPerShift
        {
            get
            {
                double blanks = Blanks_Per_Stroke ?? 1.0;
                double shift = RealShift == 0 ? 1.0 : RealShift; // evitar división por cero
                return BlanksPerYearThousands / (blanks * shift);
            }
        }

        /// <summary>
        /// Calcula el porcentaje (o valor) promedio de capacidad para este material,
        /// tomando solo los años fiscales que se traslapan con [Real_SOP, Real_EOP].
        /// Usa el diccionario devuelto por GetCapacityPlusQuote() del proyecto.
        /// </summary>
        /// <param name="capacityPlusQuote">
        /// Diccionario: [lineId -> [fyId -> valor]],
        /// donde 'valor' representa la capacidad (por ejemplo, en porcentaje o en decimal).
        /// </param>
        /// <returns>Promedio de capacidad en los FY traslapados, o 0 si no aplica.</returns>
        public double GetAverageCapacityFromQuote(Dictionary<int, Dictionary<int, double>> capacityPlusQuote)
        {
            Debug.WriteLine("========== Cálculo de capacidad promedio para Material ID: " + this.ID_Material + " ==========");

            // 1. Verificar que el material tenga línea real asignada
            if (!this.ID_Real_Blanking_Line.HasValue)
            {
                Debug.WriteLine("No existe línea real (ID_Real_Blanking_Line) para este material. Se retorna 0.");
                return 0.0;
            }

            int lineId = this.ID_Real_Blanking_Line.Value;

            // 2. Verificar si la línea está en el diccionario
            if (!capacityPlusQuote.ContainsKey(lineId))
            {
                Debug.WriteLine($"La línea real {lineId} no se encuentra en el diccionario de capacidad. Se retorna 0.");
                return 0.0;
            }

            // 3. Obtener el diccionario interno [fyId -> valor] para esa línea
            var lineCapacity = capacityPlusQuote[lineId];

            // 4. Determinar el rango de fechas [Real_SOP, Real_EOP]
            //    Si no hay valores, usar MinValue / MaxValue como fallback
            DateTime sop = this.Real_SOP ?? DateTime.MinValue;
            DateTime eop = this.Real_EOP ?? DateTime.MaxValue;

            // 5. Cargar todos los FY en memoria para checar Start_Date y End_Date
            using (var db = new Portal_2_0Entities())
            {
                var allFiscalYears = db.CTZ_Fiscal_Years
                    .ToDictionary(fy => fy.ID_Fiscal_Year, fy => fy);

                // 6. Recorrer los fyId presentes en lineCapacity y ver cuáles se traslapan
                var relevantValues = new List<double>();

                foreach (var kvp in lineCapacity)
                {
                    int fyId = kvp.Key;
                    double capacityVal = kvp.Value;

                    // Verificar si tenemos info de este FY en la tabla CTZ_Fiscal_Years
                    if (!allFiscalYears.ContainsKey(fyId))
                    {
                        Debug.WriteLine($"FY con ID {fyId} no existe en CTZ_Fiscal_Years. Se omite.");
                        continue;
                    }

                    var fyRow = allFiscalYears[fyId];
                    DateTime fyStart = fyRow.Start_Date;
                    DateTime fyEnd = fyRow.End_Date;

                    // 7. Comprobar traslape: si [fyStart, fyEnd] se traslapa con [sop, eop]
                    //    Traslape ocurre si el fin del FY no es antes del inicio SOP,
                    //    y el inicio del FY no es después del fin EOP.
                    bool overlap = !(fyEnd < sop || fyStart > eop);

                    if (overlap)
                    {
                        Debug.WriteLine($"{fyRow.Fiscal_Year_Name} traslapa con SOP/EOP. Valor={capacityVal}");
                        relevantValues.Add(capacityVal);
                    }
                    else
                    {
                        Debug.WriteLine($"{fyRow.Fiscal_Year_Name} NO traslapa con SOP/EOP. Se omite.");
                    }
                }

                // 8. Si no hay FY traslapados, retornar 0
                if (relevantValues.Count == 0)
                {
                    Debug.WriteLine("No se encontraron FY traslapados. Se retorna 0.");
                    return 0.0;
                }

                // 9. Calcular el promedio
                double average = relevantValues.Average();
                Debug.WriteLine($"Promedio de capacidad (material {this.ID_Material}): {average}");

                return average;
            }
        }


        /// <summary>
        /// Calcula el porcentaje de capacidad basado en la producción, las fórmulas y agrupaciones definidas.
        /// Por ahora retorna 0 por defecto.
        /// </summary>
        /// <returns>Porcentaje de capacidad (double)</returns>
        public Dictionary<int, double> GetRealMinutes()
        {
            // Paso 1: Obtener la producción por año fiscal (ID_Fiscal_Year -> producción)
            var fiscalYearData = GetProductionByFiscalYearID();

            //// Debug: imprimir la producción base
            //Debug.WriteLine("=== Producción base ===");
            //DebugProductionDictionary(fiscalYearData);

            // Paso 2: Transformar con las fórmulas
            var transformedData = ApplyStep2Formulas(fiscalYearData);

            //// Debug: imprimir la producción tras aplicar el paso 2
            //Debug.WriteLine("=== Producción tras aplicar Paso 2 ===");
            //DebugProductionDictionary(transformedData);

            // Por el momento, la implementación retorna 0.
            return transformedData;
        }

        /// <summary>
        /// Obtiene la producción total por año fiscal para este material.
        /// El año fiscal se define de octubre a septiembre.
        /// Se utiliza el campo Vehicle del material, que es una concatenación que debe coincidir con CTZ_Temp_IHS.ConcatCodigo.
        /// </summary>
        /// <returns>Diccionario donde la clave es el año fiscal y el valor es la producción total (Production_Amount) para ese año.</returns>        
        public Dictionary<int, double> GetProductionByFiscalYearID()
        {
            var productionByFYId = new Dictionary<int, double>();

            using (var db = new Portal_2_0Entities())
            {
                // 1) Obtener la clave del Vehicle en mayúsculas
                string vehicleKey = (this.Vehicle ?? "").ToUpper().Trim();

                // 2) Buscar el registro CTZ_Temp_IHS correspondiente
                var tempIHS = db.CTZ_Temp_IHS
                    .AsEnumerable() // Para poder usar la propiedad NotMapped ConcatCodigo en memoria
                    .FirstOrDefault(t => t.ConcatCodigo == vehicleKey);

                if (tempIHS == null)
                {
                    // Si no existe, retornamos diccionario vacío
                    return productionByFYId;
                }

                // 3) Traer las producciones para ese IHS
                var productions = db.CTZ_Temp_IHS_Production
                    .Where(p => p.ID_IHS == tempIHS.ID_IHS)
                    .ToList();

                // 4) Para cada producción, calcular el año fiscal y buscar en CTZ_Fiscal_Years
                foreach (var prod in productions)
                {
                    // Calcular el año fiscal base
                    int fy = (prod.Production_Month >= 10)
                                ? prod.Production_Year + 1
                                : prod.Production_Year;

                    // Generar las fechas de inicio y fin de ese FY: 1-Oct-(fy-1) a 30-Sep-fy
                    DateTime startDate = new DateTime(fy - 1, 10, 1); // 1-oct del año anterior
                    DateTime endDate = new DateTime(fy, 9, 30);       // 30-sep del año calculado

                    // Buscar en la tabla CTZ_Fiscal_Years la fila que coincida con ese rango
                    var fiscalRow = db.CTZ_Fiscal_Years
                        .FirstOrDefault(x => x.Start_Date == startDate && x.End_Date == endDate);

                    if (fiscalRow != null)
                    {
                        // Tomamos el ID_Fiscal_Year
                        int fyId = fiscalRow.ID_Fiscal_Year;

                        // Sumamos la producción
                        if (!productionByFYId.ContainsKey(fyId))
                            productionByFYId[fyId] = 0;

                        productionByFYId[fyId] += prod.Production_Amount;
                    }
                }
            }

            return productionByFYId;
        }

        /// <summary>
        /// Aplica las fórmulas del Paso 2 a los datos de producción por año fiscal.
        ///  1) value *= Parts_Per_Vehicle
        ///  2) value /= Ideal_Cycle_Time_Per_Tool
        ///     value /= Blanks_Per_Stroke
        ///  3) value /= OEE
        /// </summary>
        /// <param name="fyData">Diccionario con la producción por ID_Fiscal_Year</param>
        /// <returns>Nuevo diccionario con los valores transformados</returns>
        private Dictionary<int, double> ApplyStep2Formulas(Dictionary<int, double> fyData)
        {
            // 1. Calcular OEE a usar
            double oeeToUse;
            if (this.OEE.HasValue)
            {
                var raw = this.OEE.Value; //si hay valor, lo normalizamos 85 -> 0.85
                oeeToUse = (raw > 1.0) ? raw / 100.0 : raw;
            }
            else
            {
                // Fecha de corte: últimos 6 meses (incluyendo mes actual)
                var cutoffDate = DateTime.Now.AddMonths(-5);
                int cutoffYear = cutoffDate.Year;
                int cutoffMonth = cutoffDate.Month;

                // Función local para obtener lista de valores OEE
                List<double> FetchOee(int lineId)
                {
                    using (var db = new Portal_2_0Entities())
                    {
                        return db.CTZ_OEE
                            .Where(x =>
                                x.ID_Line == lineId &&
                                // comparar Year/Month >= cutoffYear/cutoffMonth
                                (x.Year > cutoffYear ||
                                 (x.Year == cutoffYear && x.Month >= cutoffMonth)))
                            .OrderByDescending(x => x.Year)
                            .ThenByDescending(x => x.Month)
                            .Take(6)                              // máximo 6 meses
                            .Where(x => x.OEE.HasValue)
                            .Select(x => x.OEE.Value)
                            .ToList();
                    }
                }

                // 1.a. Determinar cuál línea usar: real si existe, si no la teórica
                int? lineToUse = this.ID_Real_Blanking_Line.HasValue
                    ? this.ID_Real_Blanking_Line
                    : this.ID_Theoretical_Blanking_Line;

                // 1.b. Si no hay ninguna línea, fallback directo a OEE = 1.0
                List<double> oeeValues = new List<double>();
                if (lineToUse.HasValue)
                {
                    oeeValues = FetchOee(lineToUse.Value);
                }

                // 1.c. Promediar y normalizar, o fallback a 1.0
                if (oeeValues.Any())
                {
                    var avg = oeeValues.Average();
                    oeeToUse = (avg > 1.0) ? avg / 100.0 : avg;
                }
                else
                {
                    oeeToUse = 1.0;
                }
            }

            // 2. Preparar demás parámetros (evitar null)
            double partsPerVehicle = this.Parts_Per_Vehicle ?? 1.0;
            double idealCycleTimePerTool = this.Ideal_Cycle_Time_Per_Tool ?? 1.0;
            double blanksPerStroke = this.Blanks_Per_Stroke ?? 1.0;

            // 3. Aplicar fórmula a cada entrada
            var transformedData = new Dictionary<int, double>();
            foreach (var kvp in fyData)
            {
                double production = kvp.Value;
                double result = production
                                * partsPerVehicle
                                / idealCycleTimePerTool
                                / blanksPerStroke
                                / oeeToUse;

                transformedData[kvp.Key] = result;
            }

            return transformedData;

        }

        /// <summary>
        /// Método para depurar/imprimir la producción por año fiscal.
        /// Aquí puedes adaptar el formato de salida a tus necesidades.
        /// </summary>
        /// <param name="fyData">Diccionario con la producción [AñoFiscal, Producción]</param>
        private void DebugProductionDictionary(Dictionary<int, double> fyData)
        {
            Debug.WriteLine($"Material ID: {this.ID_Material}");
            Debug.WriteLine($"Vehicle Key: {this.Vehicle}");
            Debug.WriteLine("Volumen S&P");

            using (var db = new Portal_2_0Entities())
            {
                // Obtenemos los IDs de FY que tenemos en el diccionario, ordenados.
                var fiscalYearIds = fyData.Keys.OrderBy(x => x).ToList();

                // Mapeamos cada ID_Fiscal_Year a su Fiscal_Year_Name (ej. "FY24/25", "FY25/26", etc.).
                var fiscalYears = db.CTZ_Fiscal_Years
                    .Where(f => fiscalYearIds.Contains(f.ID_Fiscal_Year))
                    .ToDictionary(f => f.ID_Fiscal_Year, f => f.Fiscal_Year_Name);

                // Construimos dos listas paralelas:
                //  1) Lista con los nombres de los años fiscales (encabezado).
                //  2) Lista con los valores de producción.
                var headerValues = new List<string>();
                var dataValues = new List<string>();

                foreach (var fyId in fiscalYearIds)
                {
                    string fyName = fiscalYears.ContainsKey(fyId) ? fiscalYears[fyId] : "N/A";
                    headerValues.Add(fyName);

                    double prodValue = fyData[fyId];
                    dataValues.Add(prodValue.ToString("F2"));
                }

                // Definimos el ancho de cada columna (ajusta según tu preferencia).
                int colWidth = 12;

                // Imprimir la tabla con líneas ASCII
                string topLine = CreateTableLine(headerValues.Count, colWidth);
                Debug.WriteLine(topLine);
                Debug.WriteLine(CreateTableRow(headerValues, colWidth));
                Debug.WriteLine(topLine);
                Debug.WriteLine(CreateTableRow(dataValues, colWidth));
                Debug.WriteLine(topLine);
            }
        }

        /// <summary>
        /// Crea una línea de tabla ASCII. Ejemplo: para 3 columnas de ancho 10:
        /// +----------+----------+----------+
        /// </summary>
        private string CreateTableLine(int columnCount, int colWidth)
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < columnCount; i++)
            {
                sb.Append("+").Append(new string('-', colWidth));
            }
            sb.Append("+");
            return sb.ToString();
        }

        /// <summary>
        /// Crea una fila de tabla ASCII a partir de una lista de valores. Ejemplo:
        /// |    FY24/25 |    FY25/26 |    FY26/27 |
        /// Ajusta PadLeft o PadRight según tu preferencia de alineación.
        /// </summary>
        private string CreateTableRow(List<string> values, int colWidth)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var val in values)
            {
                // Usamos PadLeft para alinear a la derecha, o PadRight si quieres alinear a la izquierda.
                sb.Append("|").Append(val.PadLeft(colWidth));
            }
            sb.Append("|");
            return sb.ToString();
        }


    }

  

    public class NewVersionViewModel
    {
        public int ID_Project { get; set; }
        public string Comments { get; set; }
    }
}