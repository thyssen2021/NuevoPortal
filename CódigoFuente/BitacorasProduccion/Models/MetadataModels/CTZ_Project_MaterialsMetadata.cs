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

        [Display(Name = "Tensile Strenght [N/mm²]")]
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

        [Display(Name = "Real Theoretical Strokes")]                        /////
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

        [Display(Name = "Outbound Incoterm")] // CAMBIO: Antes "Freight Type"
        public Nullable<int> ID_FreightType { get; set; }

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

        [Display(Name = "Interplant Other Label")]
        [StringLength(120)]
        public string InterplantLabelOtherDescription { get; set; }

        [Display(Name = "Interplant Other Additional")]
        [StringLength(120)]
        public string InterplantAdditionalsOtherDescription { get; set; }

        [Display(Name = "Interplant Strap Observations")]
        [StringLength(120)]
        public string InterplantStrapTypeObservations { get; set; }


        [Display(Name = "Interplant Special Requirement")]
        [StringLength(350)]
        public string InterplantSpecialRequirement { get; set; }

        [Display(Name = "Interplant Special Packaging")]
        [StringLength(350)]
        public string InterplantSpecialPackaging { get; set; }

        [Display(Name = "Interplant Packaging File")]
        public Nullable<int> ID_File_InterplantPackaging { get; set; }

        [Display(Name = "Interplant Returnable Rack")]
        public Nullable<bool> IsInterplantReturnableRack { get; set; }

        [Display(Name = "Interplant Returnable Uses")]
        public Nullable<int> InterplantReturnableUses { get; set; }

        [Display(Name = "Interplant Outbound Incoterm")] // CAMBIO: Antes "Interplant Freight Type"
        public Nullable<int> ID_Interplant_FreightType { get; set; }

        [Display(Name = "Interplant Delivery Conditions")]
        [StringLength(350)]
        public string InterplantDeliveryConditions { get; set; }

        [Display(Name = "Interplant Scrap Reconciliation")]
        public Nullable<bool> InterplantScrapReconciliation { get; set; }

        [Display(Name = "Interplant Scrap Min %")]
        public Nullable<double> InterplantScrapReconciliationPercent_Min { get; set; }

        [Display(Name = "Interplant Scrap Optimal %")]
        public Nullable<double> InterplantScrapReconciliationPercent { get; set; }

        [Display(Name = "Interplant Scrap Max %")]
        public Nullable<double> InterplantScrapReconciliationPercent_Max { get; set; }

        [Display(Name = "Interplant Client Scrap %")]
        public Nullable<double> InterplantClientScrapReconciliationPercent { get; set; }

        [Display(Name = "Interplant Head/Tail Reconciliation")]
        public Nullable<bool> InterplantHeadTailReconciliation { get; set; }

        [Display(Name = "Interplant H/T Min %")]
        public Nullable<double> InterplantHeadTailReconciliationPercent_Min { get; set; }

        [Display(Name = "Interplant H/T Optimal %")]
        public Nullable<double> InterplantHeadTailReconciliationPercent { get; set; }

        [Display(Name = "Interplant H/T Max %")]
        public Nullable<double> InterplantHeadTailReconciliationPercent_Max { get; set; }

        [Display(Name = "Interplant Client H/T %")]
        public Nullable<double> InterplantClientHeadTailReconciliationPercent { get; set; }

        [Display(Name = "Interplant Outbound Freight File")]
        public Nullable<int> ID_File_InterplantOutboundFreight { get; set; }

        // ... otros campos existentes ...

        [Display(Name = "Mill")]
        [StringLength(80, ErrorMessage = "Mill cannot exceed 80 characters.")]
        public string Mill { get; set; }

        [Display(Name = "Material Specification")]
        [StringLength(80, ErrorMessage = "Specification cannot exceed 80 characters.")]
        public string MaterialSpecification { get; set; }

        [Display(Name = "Estimated Annual Volume [Tons]")]
        public Nullable<double> SlitterEstimatedAnnualVolume { get; set; }

        [Display(Name = "Load Per Transport [Tons]")]
        public Nullable<double> LoadPerTransport { get; set; }

        [Display(Name = "Interplant Load Per Transport [Tons]")] // NUEVO CAMPO
        public Nullable<double> InterplantLoadPerTransport { get; set; }

        // --- NUEVOS CAMPOS PARA BLOQUE BLANKING ---

        [Display(Name = "Annual Volume [Vehicles] (Blanking)")]
        public Nullable<int> Blanking_Annual_Volume { get; set; }

        [Display(Name = "Annual Volume [m tons/year] (Blanking)")]
        public Nullable<double> Blanking_Volume_Per_year { get; set; }

        [Display(Name = "Initial Weight per Part [kg]")]
        public Nullable<double> Blanking_InitialWeightPerPart { get; set; }

        [Display(Name = "Process Tons")]
        public Nullable<double> Blanking_ProcessTons { get; set; }

        [Display(Name = "Shipping Tons")]
        public Nullable<double> Blanking_ShippingTons { get; set; }

         [NotMapped]
         [Display(Name = "Initial Weight [kg]")]
         public Nullable<double> Initial_Weight { get; set; }

        [Display(Name = "Adjustment Factor (%)")]
        public double Max_Production_Factor { get; set; }

        [Display(Name = "Effective Max Production")]
        public Nullable<int> Max_Production_Effective { get; set; }


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
        public List<int> SelectedRackTypeIds { get; set; }

        [NotMapped]
        public List<int> SelectedAdditionalIds { get; set; }

        [NotMapped]
        public List<int> SelectedLabelIds { get; set; }

        [NotMapped]
        public List<int> SelectedStrapTypeIds { get; set; }

        [NotMapped]
        public List<int> SelectedInterplantRackTypeIds { get; set; } // Add this line
        [NotMapped]
        public List<int> SelectedInterplantLabelIds { get; set; }

        [NotMapped]
        public List<int> SelectedInterplantAdditionalIds { get; set; }
        [NotMapped]
        public List<int> SelectedInterplantStrapTypeIds { get; set; }
 

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
        public bool IsInterplantPackagingFile { get; set; }
        [NotMapped]
        public string InterplantPackagingFileName { get; set; }
        [NotMapped]
        public bool IsInterplantOutboundFreightFile { get; set; }
        [NotMapped]
        public string InterplantOutboundFreightFileName { get; set; }



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
        /// <summary>
        /// Calcula el porcentaje de capacidad aplicando el factor de producción a las unidades brutas
        /// ANTES de convertir a minutos/capacidad.
        /// </summary>
        public Dictionary<int, double> GetRealMinutes(List<string> debugTrace = null)
        {
            // 1. Obtener la producción "cruda"
            if (debugTrace != null) debugTrace.Add($"   [GetRealMinutes] Iniciando cálculo para Material: {this.Part_Number}");

            var fiscalYearDataRaw = GetProductionByFiscalYearID(debugTrace);

            // --- NUEVO BLOQUE: LÓGICA DE FACTOR MEJORADA ---

            // Determinar el factor a usar
            double factorPercent;
            bool isDefault = false;

            if (this.Max_Production_Factor.HasValue && this.Max_Production_Factor.Value > 0)
            {
                factorPercent = this.Max_Production_Factor.Value;
            }
            else
            {
                factorPercent = 100.0;
                isDefault = true;
            }

            double multiplier = factorPercent / 100.0;

            // LOG: Reportar explícitamente qué está pasando
            if (debugTrace != null)
            {
                if (isDefault)
                {
                    // Mensaje específico cuando NO se define valor
                    debugTrace.Add($"   [FACTOR] Valor no definido o cero. Aplicando por defecto: 100% (x1.0)");
                }
                else
                {
                    // Mensaje cuando SÍ hay valor
                    debugTrace.Add($"   [FACTOR] Ajuste definido por usuario: {factorPercent}% (x{multiplier:F2})");
                }
            }

            var fiscalYearDataAdjusted = new Dictionary<int, double>();

            // Diccionario temporal para nombres de FY
            var fyNamesMap = new Dictionary<int, string>();

            if (debugTrace != null && fiscalYearDataRaw.Any())
            {
                using (var db = new Portal_2_0Entities())
                {
                    var fyIds = fiscalYearDataRaw.Keys.ToList();
                    fyNamesMap = db.CTZ_Fiscal_Years
                        .Where(f => fyIds.Contains(f.ID_Fiscal_Year))
                        .ToDictionary(f => f.ID_Fiscal_Year, f => f.Fiscal_Year_Name);
                }
            }

            foreach (var kvp in fiscalYearDataRaw)
            {
                int fyId = kvp.Key;
                double rawUnits = kvp.Value;
                double adjustedUnits = rawUnits * multiplier;

                fiscalYearDataAdjusted[fyId] = adjustedUnits;

                // Solo mostrar el detalle línea por línea si el factor NO es 100%, para no saturar el log
                if (debugTrace != null && Math.Abs(multiplier - 1.0) > 0.001)
                {
                    string fyName = fyNamesMap.ContainsKey(fyId) ? fyNamesMap[fyId] : $"ID {fyId}";
                    debugTrace.Add($"     -> {fyName}: {rawUnits:N0} uds * {multiplier:F2} = {adjustedUnits:N0} uds");
                }
            }
            // -------------------------------------------------------------

            // 2. Transformar con las fórmulas
            var transformedData = ApplyStep2Formulas(fiscalYearDataAdjusted, debugTrace);

            if (debugTrace != null)
            {
                debugTrace.Add($"   [GetRealMinutes] Resultados finales (Minutos requeridos):");
                foreach (var kvp in transformedData)
                {
                    string fyName = fyNamesMap.ContainsKey(kvp.Key) ? fyNamesMap[kvp.Key] : $"ID {kvp.Key}";
                    debugTrace.Add($"     -> {fyName}: {kvp.Value:F2} min");
                }
            }

            return transformedData;
        }

        /// <summary>
        /// Obtiene la producción total por año fiscal para este material.
        /// </summary>
        public Dictionary<int, double> GetProductionByFiscalYearID(List<string> debugTrace = null)
        {
            var productionByFYId = new Dictionary<int, double>();
            // Diccionario auxiliar para guardar los nombres de los FY solo para debug
            var fyNamesDebug = new Dictionary<int, string>();

            // 1. Validar que la propiedad Vehicle no sea nula o vacía.
            if (string.IsNullOrEmpty(this.Vehicle))
            {
                if (debugTrace != null) debugTrace.Add("     [ERROR] Vehicle es nulo o vacío. Retornando 0 producción.");
                return productionByFYId;
            }

            // 2. Extraer la clave de búsqueda
            string vehiclePlantKey;
            int underscoreIndex = this.Vehicle.IndexOf('_');

            if (underscoreIndex > -1)
                vehiclePlantKey = this.Vehicle.Substring(0, underscoreIndex).Trim();
            else
                vehiclePlantKey = this.Vehicle.Trim();

            if (string.IsNullOrEmpty(vehiclePlantKey))
            {
                if (debugTrace != null) debugTrace.Add("     [ERROR] Clave de vehículo vacía tras limpieza. Retornando 0.");
                return productionByFYId;
            }

            if (debugTrace != null) debugTrace.Add($"     Buscando IHS con Mnemonic_Vehicle_plant: '{vehiclePlantKey}'");

            using (var db = new Portal_2_0Entities())
            {
                // 3. Buscar el registro CTZ_Temp_IHS
                var tempIHS = db.CTZ_Temp_IHS
                                .FirstOrDefault(t => t.Mnemonic_Vehicle_plant == vehiclePlantKey);

                if (tempIHS == null)
                {
                    if (debugTrace != null) debugTrace.Add($"     [WARNING] No se encontró registro en CTZ_Temp_IHS para '{vehiclePlantKey}'.");
                    return productionByFYId;
                }

                DateTime startSearchDate = this.Real_SOP.HasValue ? this.Real_SOP.Value : (tempIHS.SOP ?? DateTime.MinValue);
                DateTime endSearchDate = this.Real_EOP.HasValue ? this.Real_EOP.Value : (tempIHS.EOP ?? DateTime.MaxValue);

                if (debugTrace != null) debugTrace.Add($"     Rango Fechas: {startSearchDate:yyyy-MM-dd} a {endSearchDate:yyyy-MM-dd}");

                // 4. Obtener la lista de AÑOS FISCALES traslapados
                var overlappingFiscalYears = db.CTZ_Fiscal_Years
                    .Where(fy => !(fy.End_Date < startSearchDate || fy.Start_Date > endSearchDate))
                    .ToList();

                if (!overlappingFiscalYears.Any())
                {
                    if (debugTrace != null) debugTrace.Add("     [INFO] No hay Años Fiscales en el rango de fechas.");
                    return productionByFYId;
                }

                // 5. Traer las producciones y filtrar
                var productions = db.CTZ_Temp_IHS_Production
                    .Where(p => p.ID_IHS == tempIHS.ID_IHS)
                    .ToList()
                    .Where(p =>
                    {
                        var productionDate = new DateTime(p.Production_Year, p.Production_Month > 0 ? p.Production_Month : 1, 1);
                        return overlappingFiscalYears.Any(fy => productionDate >= fy.Start_Date && productionDate <= fy.End_Date);
                    })
                    .ToList();

                if (debugTrace != null) debugTrace.Add($"     Registros de producción encontrados: {productions.Count}");

                // 6. Sumarizar por Año Fiscal
                foreach (var prod in productions)
                {
                    int fy = (prod.Production_Month >= 10) ? prod.Production_Year + 1 : prod.Production_Year;
                    DateTime startDate = new DateTime(fy - 1, 10, 1);
                    DateTime endDate = new DateTime(fy, 9, 30);

                    var fiscalRow = db.CTZ_Fiscal_Years
                        .FirstOrDefault(x => x.Start_Date == startDate && x.End_Date == endDate);

                    if (fiscalRow != null)
                    {
                        int fyId = fiscalRow.ID_Fiscal_Year;

                        if (!productionByFYId.ContainsKey(fyId))
                            productionByFYId[fyId] = 0;

                        productionByFYId[fyId] += prod.Production_Amount;

                        // CAMBIO AQUÍ: Guardamos el nombre para el debug
                        if (debugTrace != null && !fyNamesDebug.ContainsKey(fyId))
                        {
                            fyNamesDebug[fyId] = fiscalRow.Fiscal_Year_Name; // Guardamos "FY 25/26"
                        }
                    }
                }

                // IMPRESIÓN DEL LOG CON NOMBRE AMIGABLE
                if (debugTrace != null)
                {
                    // Ordenamos por ID para que salgan en orden cronológico
                    foreach (var kvp in productionByFYId.OrderBy(x => x.Key))
                    {
                        // Usamos el nombre si existe, si no, el ID como fallback
                        string fyName = fyNamesDebug.ContainsKey(kvp.Key) ? fyNamesDebug[kvp.Key] : $"ID {kvp.Key}";

                        debugTrace.Add($"     -> {fyName}: {kvp.Value:N0} unidades");
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
        private Dictionary<int, double> ApplyStep2Formulas(Dictionary<int, double> fyData, List<string> debugTrace = null) // <--- Parámetro agregado
        {
            if (debugTrace != null) debugTrace.Add(">> Inicio: Aplicación de Fórmulas (Paso 2)");

            // 1. Calcular OEE a usar
            double oeeToUse;

            // --- LÓGICA DE OEE ---
            if (this.OEE.HasValue)
            {
                // Caso A: El OEE fue proporcionado directamente en el formulario.
                var raw = this.OEE.Value;
                oeeToUse = (raw > 1.0) ? raw / 100.0 : raw; // Normaliza si el valor es > 1 (ej. 85 -> 0.85)

                if (debugTrace != null)
                {
                    debugTrace.Add($"   [OEE] Valor explícito en material: {raw}");
                    debugTrace.Add($"   [OEE] -> Normalizado: {oeeToUse:P2}");
                }
            }
            else
            {
                // Caso B: No se proporcionó OEE, se debe calcular el promedio.
                if (debugTrace != null) debugTrace.Add("   [OEE] No hay valor explícito. Calculando promedio histórico (6 meses)...");

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
                    if (debugTrace != null) debugTrace.Add($"   [OEE] Consultando BD para Línea ID: {lineToUse.Value}");
                    oeeValues = FetchOee(lineToUse.Value);
                }
                else
                {
                    if (debugTrace != null) debugTrace.Add("   [OEE] [WARNING] Sin línea Real ni Teórica. No se puede buscar OEE.");
                }

                // 1.c. Promediar y normalizar, o usar el valor por defecto.
                if (oeeValues.Any())
                {
                    var avg = oeeValues.Average();
                    oeeToUse = (avg > 1.0) ? avg / 100.0 : avg; // Normalizar

                    if (debugTrace != null)
                    {
                        string valoresEncontrados = string.Join(", ", oeeValues.Select(v => v.ToString("0.0")));
                        debugTrace.Add($"   [OEE] Valores hallados: [{valoresEncontrados}]");
                        debugTrace.Add($"   [OEE] -> Promedio: {avg:F2}. Normalizado: {oeeToUse:P2}");
                    }
                }
                else
                {
                    // Fallback si no se encontraron valores de OEE.
                    oeeToUse = 1.0;
                    if (debugTrace != null) debugTrace.Add($"   [OEE] Sin historial. Usando Default: {oeeToUse:P0}");
                }
            }

            // --- VARIABLES DE FÓRMULA ---
            double partsPerVehicle = this.Parts_Per_Vehicle ?? 1.0;
            double idealCycleTimePerTool = this.Ideal_Cycle_Time_Per_Tool ?? 1.0;
            double blanksPerStroke = this.Blanks_Per_Stroke ?? 1.0;

            if (debugTrace != null)
            {
                debugTrace.Add($"   [FÓRMULA] Parámetros usados:");
                debugTrace.Add($"     • Parts/Veh: {partsPerVehicle}");
                debugTrace.Add($"     • CycleTime (Strokes/min): {idealCycleTimePerTool}");
                debugTrace.Add($"     • Blanks/Stroke: {blanksPerStroke}");
                debugTrace.Add($"     • OEE Final: {oeeToUse:P2}");
                debugTrace.Add($"   [FÓRMULA] Ecuación: (Volumen(S&P) * PartsPerVeh) / (Strokes * BlanksPerStroke * OEE)");
            }

            var transformedData = new Dictionary<int, double>();

            foreach (var kvp in fyData)
            {
                double production = kvp.Value;

                // Evitar división por cero
                double denominator = (idealCycleTimePerTool * blanksPerStroke * oeeToUse);
                double result = 0;

                if (denominator > 0)
                {
                    result = (production * partsPerVehicle) / denominator;
                }
                else
                {
                    if (debugTrace != null) debugTrace.Add($"   [ERROR] Denominador cero en FY {kvp.Key}. Revise parámetros.");
                }

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
}