using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class CTZ_Project_MaterialsMetadata
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
        public Nullable<int> Theoretical_Strokes { get; set; }

        [Display(Name = "Real Strokes")]
        public Nullable<int> Real_Strokes { get; set; }

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

    }

    [MetadataType(typeof(CTZ_Project_MaterialsMetadata))]
    public partial class CTZ_Project_Materials
    {

    }
}