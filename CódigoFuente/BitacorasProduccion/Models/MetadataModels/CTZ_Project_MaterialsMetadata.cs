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
    public class CTZ_Project_MaterialsMetadata
    {
        [Display(Name = "ID")]
        public int ID_Material { get; set; }

        [Display(Name = "IHS")]
        public Nullable<int> ID_IHS_Item { get; set; }

        [Display(Name = "Max Production S&P [Vehicles]")]
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

        [Display(Name = "Tensile Strenght [Rm]")]
        public Nullable<double> Tensile_Strenght { get; set; }

        [Display(Name = "Material Type")]
        public Nullable<int> ID_Material_type { get; set; }

        [Display(Name = "Thickness [mm]")]
        public Nullable<double> Thickness { get; set; }
        [Display(Name = "Width [mm]")]
        public Nullable<double> Width { get; set; }
        [Display(Name = "Pitch [mm]")]
        public Nullable<double> Pitch { get; set; }

        [Display(Name = "Theoretical Gross Weight [kg]")]
        public Nullable<double> Theoretical_Gross_Weight { get; set; }

        [Display(Name = "Gross Weight [Prov. by client - kg]")]
        public Nullable<double> Gross_Weight { get; set; }

        [Display(Name = "Annual Volume [Prov. by client – Vehicles]")]
        public Nullable<int> Annual_Volume { get; set; }

        [Display(Name = "Annual Volume [Prov. by client – m tons/year]")]
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
        public Nullable<double> Theoretical_Strokes { get; set; }  ////////

        [Display(Name = "Real Strokes")]                        /////
        public Nullable<double> Real_Strokes { get; set; }

        [Display(Name = "Ideal Cycle Time Per Tool")]
        public Nullable<double> Ideal_Cycle_Time_Per_Tool { get; set; }
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

        [Display(Name = "Thickness (-) Tol. [mm]")]
        public Nullable<double> ThicknessToleranceNegative { get; set; }
        [Display(Name = "Thickness (+) Tol. [mm]")]
        public Nullable<double> ThicknessTolerancePositive { get; set; }
        [Display(Name = "Width (-) Tol. [mm]")]
        public Nullable<double> WidthToleranceNegative { get; set; }
        [Display(Name = "Width (+) Tol. [mm]")]
        public Nullable<double> WidthTolerancePositive { get; set; }
        [Display(Name = "Pitch (-) Tol. [mm]")]
        public Nullable<double> PitchToleranceNegative { get; set; }
        [Display(Name = "Pitch (+) Tol. [mm]")]
        public Nullable<double> PitchTolerancePositive { get; set; }
        [Display(Name = "Weight of Final Mults [kg] (Optimal)")] // CAMBIO: Texto actualizado
        public Nullable<double> WeightOfFinalMults { get; set; }
        [Display(Name = "Weight of Final Mults [kg] (Min)")]
        public Nullable<double> WeightOfFinalMults_Min { get; set; }

        [Display(Name = "Weight of Final Mults [kg] (Max)")]
        public Nullable<double> WeightOfFinalMults_Max { get; set; }
        [Display(Name = "Mults [Prov. by client - pcs]")]
        public Nullable<double> Multipliers { get; set; }
        [Display(Name = "Angle A (-) Tol.")]
        public Nullable<double> AngleAToleranceNegative { get; set; }
        [Display(Name = "Angle A (+) Tol.")]
        public Nullable<double> AngleATolerancePositive { get; set; }
        [Display(Name = "Angle B (-) Tol.")]
        public Nullable<double> AngleBToleranceNegative { get; set; }
        [Display(Name = "Angle B (+) Tol.")]
        public Nullable<double> AngleBTolerancePositive { get; set; }
        [Display(Name = "Major Base")]
        public Nullable<double> MajorBase { get; set; }
        [Display(Name = "Major Base (-) Tol.")]
        public Nullable<double> MajorBaseToleranceNegative { get; set; }
        [Display(Name = "Major Base (+) Tol.")]
        public Nullable<double> MajorBaseTolerancePositive { get; set; }
        [Display(Name = "Minor Base")]
        public Nullable<double> MinorBase { get; set; }
        [Display(Name = "Minor Base (-) Tol.")]
        public Nullable<double> MinorBaseToleranceNegative { get; set; }
        [Display(Name = "Minor Base (+) Tol.")]
        public Nullable<double> MinorBaseTolerancePositive { get; set; }
        [Display(Name = "Flatness [mm]")]
        public Nullable<double> Flatness { get; set; }
        [Display(Name = "Flatness (-) Tol. [mm]")]
        public Nullable<double> FlatnessToleranceNegative { get; set; }
        [Display(Name = "Flatness (+) Tol. [mm]")]
        public Nullable<double> FlatnessTolerancePositive { get; set; }
        [Display(Name = "Master Coil Weight [kg]")]
        public Nullable<double> MasterCoilWeight { get; set; }
        [Display(Name = "Inner Coil Diameter Arrival [mm]")]
        public Nullable<double> InnerCoilDiameterArrival { get; set; }
        [Display(Name = "Outer Coil Diameter Arrival [mm]")]
        public Nullable<double> OuterCoilDiameterArrival { get; set; }
        [Display(Name = "Inner Coil Diameter Delivery [mm]")]
        public Nullable<double> InnerCoilDiameterDelivery { get; set; }
        [Display(Name = "Outer Coil Diameter Delivery [mm]")]
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

        [Display(Name = "Packaging Drawing / Standard")]
        public Nullable<int> ID_File_Packaging { get; set; }

        [Display(Name = "Observations")]
        public string StrapTypeObservations { get; set; }

        [Display(Name = "Other")]
        public string AdditionalsOtherDescription { get; set; }

        [Display(Name = "Other")]
        public string LabelOtherDescription { get; set; }

        [Display(Name = "Tons Per Shift")]
        public Nullable<double> TonsPerShift { get; set; }
        [Display(Name = "Mults Width")]
        public Nullable<double> Width_Mults { get; set; }

        [Display(Name = "Mults Width (-) Tol. [mm]")]
        public Nullable<double> Width_Mults_Tol_Neg { get; set; }

        [Display(Name = "Mults Width (+) Tol. [mm]")]
        public Nullable<double> Width_Mults_Tol_Pos { get; set; }

        [Display(Name = "Blanks Width")]
        public Nullable<double> Width_Plates { get; set; }

        [Display(Name = "Blanks Width (-) Tol. [mm]")]
        public Nullable<double> Width_Plates_Tol_Neg { get; set; }

        [Display(Name = "Blanks Width (+) Tol. [mm]")]
        public Nullable<double> Width_Plates_Tol_Pos { get; set; }

        [Display(Name = "Annual Tonnage (Tons)")]
        public Nullable<double> AnnualTonnage { get; set; }

        [Display(Name = "Pieces per Package")]
        public Nullable<int> PiecesPerPackage { get; set; }

        [Display(Name = "Stacks per Package")]
        public Nullable<int> StacksPerPackage { get; set; }

        [Display(Name = "Package Weight (kg)")]
        public Nullable<double> PackageWeight { get; set; } // Cambiado a double

        [Display(Name = "Delivery Conditions")]
        public string DeliveryConditions { get; set; }

        [Display(Name = "Returnable Rack")]
        public Nullable<bool> IsReturnableRack { get; set; }

        [Display(Name = "Number of Uses")]
        public Nullable<int> ReturnableUses { get; set; }

        [Display(Name = "Scrap Reconciliation")] // CAMBIO: Texto corregido
        public Nullable<bool> ScrapReconciliation { get; set; }

        [Display(Name = "tkMM Scrap Rec. % (Optimal)")] // CAMBIO: Texto actualizado
        public Nullable<double> ScrapReconciliationPercent { get; set; }

        [Display(Name = "tkMM Scrap Rec. % (Min)")] // CAMBIO: Texto actualizado
        public Nullable<double> ScrapReconciliationPercent_Min { get; set; }

        [Display(Name = "tkMM Scrap Rec. % (Max)")] // CAMBIO: Texto actualizado
        public Nullable<double> ScrapReconciliationPercent_Max { get; set; }

        [Display(Name = "Head/Tail Reconciliation")] // CAMBIO: Texto corregido
        public Nullable<bool> HeadTailReconciliation { get; set; }

        [Display(Name = "tkMM Head/Tail Rec. % (Optimal)")] // CAMBIO: Texto actualizado
        public Nullable<double> HeadTailReconciliationPercent { get; set; }

        [Display(Name = "tkMM Head/Tail Rec. % (Min)")] // CAMBIO: Texto actualizado
        public Nullable<double> HeadTailReconciliationPercent_Min { get; set; }

        [Display(Name = "tkMM Head/Tail Rec. % (Max)")] // CAMBIO: Texto actualizado
        public Nullable<double> HeadTailReconciliationPercent_Max { get; set; }

        [Display(Name = "Client Scrap Rec. %")]
        public Nullable<double> ClientScrapReconciliationPercent { get; set; }

        [Display(Name = "Client Head/Tail Rec. %")]
        public Nullable<double> ClientHeadTailReconciliationPercent { get; set; }

        [Display(Name = "Delivery Coil Position")]
        public Nullable<int> ID_Delivery_Coil_Position { get; set; }

        [Display(Name = "Delivery Transport Type")]
        public Nullable<int> ID_Delivery_Transport_Type { get; set; }

        [Display(Name = "Specify Other Transport")]
        public string Delivery_Transport_Type_Other { get; set; }

        [Display(Name = "Client Net Weight (kg)")]
        public Nullable<double> ClientNetWeight { get; set; }

        [Display(Name = "Is Running Change?")]
        public Nullable<bool> IsRunningChange { get; set; }

        [Display(Name = "Technical Sheet")]
        public Nullable<int> ID_File_TechnicalSheet { get; set; }

        [Display(Name = "Additional Files")]
        public Nullable<int> ID_File_Additional { get; set; }

        [Display(Name = "Additional Arrival Files")]
        public Nullable<int> ID_File_ArrivalAdditional { get; set; }

        [Display(Name = "Coil Data Additional File")]
        public Nullable<int> ID_File_CoilDataAdditional { get; set; }

        [Display(Name = "Slitter Data Additional File")]
        public Nullable<int> ID_File_SlitterDataAdditional { get; set; }

        [Display(Name = "Volume Additional File")]
        public Nullable<int> ID_File_VolumeAdditional { get; set; }

        [Display(Name = "Outbound Freight Additional File")]
        public Nullable<int> ID_File_OutboundFreightAdditional { get; set; }

        [Display(Name = "Delivery Packaging Additional File")]
        public Nullable<int> ID_File_DeliveryPackagingAdditional { get; set; }

        [Display(Name = "Is Welded Blank?")]
        public Nullable<bool> IsWeldedBlank { get; set; }

        [Display(Name = "Number of Blanks")]
        public Nullable<int> NumberOfPlates { get; set; }

        [Display(Name = "Arrival Warehouse")] // CAMBIO: Texto actualizado para mayor claridad
        public Nullable<int> ID_Arrival_Warehouse { get; set; }

        [Display(Name = "Passes Through South WH?")]
        public Nullable<bool> PassesThroughSouthWarehouse { get; set; }

        [Display(Name = "Weight per Part [kg]")]
        public Nullable<double> WeightPerPart { get; set; }
        [Display(Name = "Is Carry Over?")]
        public Nullable<bool> IsCarryOver { get; set; }
        [Display(Name = "Requires Rack Manufacturing?")]
        public Nullable<bool> RequiresRackManufacturing { get; set; }

        [Display(Name = "Requires Die Manufacturing?")]
        public Nullable<bool> RequiresDieManufacturing { get; set; }

        [Display(Name = "Shearing Width [mm]")]
        public Nullable<double> Shearing_Width { get; set; }

        [Display(Name = "Shearing Width (+) Tol. [mm]")]
        public Nullable<double> Shearing_Width_Tol_Pos { get; set; }

        [Display(Name = "Shearing Width (-) Tol. [mm]")]
        public Nullable<double> Shearing_Width_Tol_Neg { get; set; }

        [Display(Name = "Shearing Pitch [mm]")]
        public Nullable<double> Shearing_Pitch { get; set; }

        [Display(Name = "Shearing Pitch (+) Tol. [mm]")]
        public Nullable<double> Shearing_Pitch_Tol_Pos { get; set; }

        [Display(Name = "Shearing Pitch (-) Tol. [mm]")]
        public Nullable<double> Shearing_Pitch_Tol_Neg { get; set; }

        [Display(Name = "Shearing Weight [kg]")]
        public Nullable<double> Shearing_Weight { get; set; }

        [Display(Name = "Shearing Weight (+) Tol. [kg]")]
        public Nullable<double> Shearing_Weight_Tol_Pos { get; set; }

        [Display(Name = "Shearing Weight (-) Tol. [kg]")]
        public Nullable<double> Shearing_Weight_Tol_Neg { get; set; }

        [Display(Name = "Shearing Pieces per Stroke")]
        public Nullable<double> Shearing_Pieces_Per_Stroke { get; set; }

        [Display(Name = "Shearing Pieces per Vehicle")]
        public Nullable<double> Shearing_Pieces_Per_Car { get; set; }

        [Display(Name = "Interplant Facility")]
        public Nullable<int> ID_Interplant_Plant { get; set; }

        [Display(Name = "Initial Weight per Part (Adjusted) [kg]")]
        public Nullable<double> InitialWeightPerPart { get; set; }

        [Display(Name = "Shipping Tons [Tons]")]
        public Nullable<double> ShippingTons { get; set; }

        [Display(Name = "Interplant Coil Position")]
        public Nullable<int> ID_InterplantDelivery_Coil_Position { get; set; }

        [Display(Name = "Interplant Transport Type")]
        public Nullable<int> ID_InterplantDelivery_Transport_Type { get; set; }

        [Display(Name = "Specify \"Other\" Interplant Transport")]
        [StringLength(50)]
        public string InterplantDelivery_Transport_Type_Other { get; set; }

        [Display(Name = "Interplant Packaging Standard")]
        [StringLength(20)]
        public string InterplantPackagingStandard { get; set; }

        [Display(Name = "Interplant Requires Rack Manufacturing")]
        public Nullable<bool> InterplantRequiresRackManufacturing { get; set; }

        [Display(Name = "Interplant Requires Die Manufacturing")]
        public Nullable<bool> InterplantRequiresDieManufacturing { get; set; }

        [Display(Name = "Interplant Pieces per Package")]
        public Nullable<int> InterplantPiecesPerPackage { get; set; } 

        [Display(Name = "Interplant Stacks per Package")]
        public Nullable<int> InterplantStacksPerPackage { get; set; }

        [Display(Name = "Interplant Package Weight (kg)")]
        public Nullable<double> InterplantPackageWeight { get; set; }

        
    }

    [MetadataType(typeof(CTZ_Project_MaterialsMetadata))]
    public partial class CTZ_Project_Materials
    {

        public CTZ_Project_Materials Clone()
        {
            // MemberwiseClone crea una copia superficial, que es suficiente si
            // no modificas propiedades complejas del objeto clonado.
            return (CTZ_Project_Materials)this.MemberwiseClone();
        }

        [NotMapped]
        [Display(Name = "Initial Weight [kg]")]
        public Nullable<double> Initial_Weight { get; set; }

        [NotMapped]
        [Display(Name = "Weight per Part [kg]")]
        public Nullable<double> WeightPerPart { get; set; }

        [NotMapped]
        public List<int> SelectedRackTypeIds { get; set; }

        [NotMapped]
        public List<int> SelectedAdditionalIds { get; set; }

        [NotMapped]
        public List<int> SelectedLabelIds { get; set; }

        [NotMapped]
        public List<int> SelectedStrapTypeIds { get; set; }

        [NotMapped]
        public List<int> SelectedInterplantRackTypeIds { get; set; } // Add this line
        
        //*** Variales para agregar archivos *** 
        [NotMapped]
        public bool? IsFile { get; set; }
        [NotMapped]
        public string CADFileName { get; set; }
        [NotMapped]
        public bool? IsPackagingFile { get; set; }
        [NotMapped]
        public bool? IsTechnicalSheetFile { get; set; }
        [NotMapped]
        public string TechnicalSheetFileName { get; set; }
        [NotMapped]
        public bool? IsAdditionalFile { get; set; }
        [NotMapped]
        public string AdditionalFileName { get; set; }
        [NotMapped]
        public bool? IsArrivalAdditionalFile { get; set; }
        [NotMapped]
        public string ArrivalAdditionalFileName { get; set; }
        [NotMapped]
        public bool? IsCoilDataAdditionalFile { get; set; }
        [NotMapped]
        public string CoilDataAdditionalFileName { get; set; }
        [NotMapped]
        public bool? IsSlitterDataAdditionalFile { get; set; }
        [NotMapped]
        public string SlitterDataAdditionalFileName { get; set; }
        [NotMapped]
        public bool? IsVolumeAdditionalFile { get; set; }
        [NotMapped]
        public string VolumeAdditionalFileName { get; set; }
        [NotMapped]
        public bool? IsOutboundFreightAdditionalFile { get; set; }
        [NotMapped]
        public string OutboundFreightAdditionalFileName { get; set; }
        [NotMapped]
        public bool? IsDeliveryPackagingAdditionalFile { get; set; }
        [NotMapped]
        public string DeliveryPackagingAdditionalFileName { get; set; }

        [NotMapped]
        public string WeldedPlatesJson { get; set; }

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

            // 1. Validar que la propiedad Vehicle no sea nula o vacía.
            if (string.IsNullOrEmpty(this.Vehicle))
            {
                return productionByFYId;
            }

            // 2. Extraer la clave de búsqueda (la parte antes del '_').
            //    Esta lógica maneja el caso donde el guion bajo no exista.
            string vehiclePlantKey;
            int underscoreIndex = this.Vehicle.IndexOf('_');

            if (underscoreIndex > -1)
            {
                // Si se encuentra un guion bajo, tomar la parte anterior.
                vehiclePlantKey = this.Vehicle.Substring(0, underscoreIndex).Trim();
            }
            else
            {
                // Si no hay guion bajo, se usa la cadena completa como clave.
                vehiclePlantKey = this.Vehicle.Trim();
            }

            // Si después de limpiar no queda nada, salimos.
            if (string.IsNullOrEmpty(vehiclePlantKey))
            {
                return productionByFYId;
            }


            using (var db = new Portal_2_0Entities())
            {            
                // 3. Buscar el registro CTZ_Temp_IHS usando la nueva clave y la columna correcta.
                //    Esta consulta ahora se traduce directamente a SQL, es más eficiente.
                var tempIHS = db.CTZ_Temp_IHS
                                .FirstOrDefault(t => t.Mnemonic_Vehicle_plant == vehiclePlantKey);


                if (tempIHS == null)
                {
                    // Si no existe, retornamos diccionario vacío
                    return productionByFYId;
                }

                //determinar el starsop y endeop
                DateTime startSearchDate = this.Real_SOP.HasValue? this.Real_SOP.Value : tempIHS.SOP.Value;
                DateTime endSearchDate = this.Real_EOP.HasValue? this.Real_EOP.Value : tempIHS.EOP.Value;

                // 4. Obtener la lista de AÑOS FISCALES que se traslapan con el rango de búsqueda.
                //    Un FY se traslapa si no termina antes de que el rango empiece, y no empieza después de que el rango termine.
                var overlappingFiscalYears = db.CTZ_Fiscal_Years
                    .Where(fy => !(fy.End_Date < startSearchDate || fy.Start_Date > endSearchDate))
                    .ToList();

                // Si no hay años fiscales en el rango, no hay nada que hacer.
                if (!overlappingFiscalYears.Any())
                {
                    return productionByFYId;
                }

                // 5. Traer las producciones para ese IHS, PERO filtrando por el rango de fechas.
                var productions = db.CTZ_Temp_IHS_Production
                    .Where(p => p.ID_IHS == tempIHS.ID_IHS)
                    .ToList() // Traemos los datos del IHS a memoria para poder construir la fecha
                    .Where(p =>
                    {
                        // Creamos una fecha a partir de los datos de producción.
                        // Se usa una salvaguarda por si el mes es 0.
                        var productionDate = new DateTime(p.Production_Year, p.Production_Month > 0 ? p.Production_Month : 1, 1);

                        // Verificamos si la fecha de producción cae dentro de CUALQUIERA de los años fiscales válidos.
                        return overlappingFiscalYears.Any(fy => productionDate >= fy.Start_Date && productionDate <= fy.End_Date);
                    })
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
            // --- Mensajes de depuración agregados ---
            //System.Diagnostics.Debug.WriteLine("--- Iniciando Cálculo de OEE (Paso 2) ---");

            // 1. Calcular OEE a usar
            double oeeToUse;
            if (this.OEE.HasValue)
            {
                // Caso A: El OEE fue proporcionado directamente en el formulario.
                var raw = this.OEE.Value;
                oeeToUse = (raw > 1.0) ? raw / 100.0 : raw; // Normaliza si el valor es > 1 (ej. 85 -> 0.85)

                //System.Diagnostics.Debug.WriteLine($"[OEE] Se proporcionó un valor de OEE explícito: {raw}");
                //System.Diagnostics.Debug.WriteLine($"[OEE] Valor normalizado a usar: {oeeToUse:P2}"); // P2 formatea como porcentaje
            }
            else
            {
                // Caso B: No se proporcionó OEE, se debe calcular el promedio.
                //System.Diagnostics.Debug.WriteLine("[OEE] No hay OEE proporcionado. Se procederá a calcular el promedio de los últimos 6 meses.");

                // Fecha de corte: últimos 6 meses.
                var cutoffDate = DateTime.Now.AddMonths(-5);
                int cutoffYear = cutoffDate.Year;
                int cutoffMonth = cutoffDate.Month;

                // Función local para obtener la lista de valores OEE
                List<double> FetchOee(int lineId)
                {
                    using (var db = new Portal_2_0Entities())
                    {
                        return db.CTZ_OEE
                                 .Where(x =>
                                     x.ID_Line == lineId &&
                                     (x.Year > cutoffYear || (x.Year == cutoffYear && x.Month >= cutoffMonth)))
                                 .OrderByDescending(x => x.Year)
                                 .ThenByDescending(x => x.Month)
                                 .Take(6)
                                 .Where(x => x.OEE.HasValue)
                                 .Select(x => x.OEE.Value)
                                 .ToList();
                    }
                }

                // 1.a. Determinar cuál línea usar: real si existe, si no, la teórica.
                int? lineToUse = this.ID_Real_Blanking_Line.HasValue && this.ID_Real_Blanking_Line != 0
                                     ? this.ID_Real_Blanking_Line
                                     : this.ID_Theoretical_Blanking_Line;

                List<double> oeeValues = new List<double>();
                if (lineToUse.HasValue)
                {
                    //System.Diagnostics.Debug.WriteLine($"[OEE] Buscando valores para la línea ID: {lineToUse.Value}");
                    oeeValues = FetchOee(lineToUse.Value);
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine("[OEE] ADVERTENCIA: No hay línea Real ni Teórica definida. No se puede buscar OEE.");
                }

                // 1.c. Promediar y normalizar, o usar el valor por defecto.
                if (oeeValues.Any())
                {
                    var avg = oeeValues.Average();
                    oeeToUse = (avg > 1.0) ? avg / 100.0 : avg; // Normalizar

                    // Mensaje detallado con los valores encontrados y el resultado.
                    string valoresEncontrados = string.Join(", ", oeeValues.Select(v => v.ToString("0.0")));
                    //System.Diagnostics.Debug.WriteLine($"[OEE] Valores encontrados: [{valoresEncontrados}]");
                    //System.Diagnostics.Debug.WriteLine($"[OEE] Promedio calculado: {avg:F2}. Valor normalizado a usar: {oeeToUse:P2}");
                }
                else
                {
                    // Fallback si no se encontraron valores de OEE.
                    oeeToUse = 1.0; // Se usa 1.0 (100%) para no afectar el cálculo si no hay datos.
                    //System.Diagnostics.Debug.WriteLine($"[OEE] No se encontraron valores de OEE para la línea ID: {lineToUse?.ToString() ?? "N/A"}. Usando valor por defecto: {oeeToUse:P2}");
                }
            }

            // El resto del método no necesita cambios...
            double partsPerVehicle = this.Parts_Per_Vehicle ?? 1.0;
            double idealCycleTimePerTool = this.Ideal_Cycle_Time_Per_Tool ?? 1.0;
            double blanksPerStroke = this.Blanks_Per_Stroke ?? 1.0;

            var transformedData = new Dictionary<int, double>();
            foreach (var kvp in fyData)
            {
                double production = kvp.Value;
                double result = (idealCycleTimePerTool > 0 && blanksPerStroke > 0 && oeeToUse > 0)
                                    ? (production * partsPerVehicle) / (idealCycleTimePerTool * blanksPerStroke * oeeToUse)
                                    : 0; // Evitar división por cero

                transformedData[kvp.Key] = result;
            }

            //System.Diagnostics.Debug.WriteLine("--- Finalizado Cálculo de OEE ---");
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
}