using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class class_v3Metadata
    {
        [Display(Name = "Número de Parte")]
        public string Customer_part_number { get; set; }
    }

    [MetadataType(typeof(class_v3Metadata))]
    public partial class class_v3 : IEquatable<class_v3>
    {
        ///*
        public string Object { get; set; }
        public string Grade { get; set; }
        public string Customer { get; set; }
        public string Shape { get; set; }
        public string Customer_part_number { get; set; }
        public string Surface { get; set; }
        public string Gauge___Metric { get; set; }
        public string Mill { get; set; }
        public string Width___Metr { get; set; }
        public string Length_mm_ { get; set; }
        public bool activo { get; set; }
        public string commodity { get; set; }
        public string flatness_metric { get; set; }
        public string surface_treatment { get; set; }
        public string coating_weight { get; set; }
        public string customer_part_msa { get; set; }
        public string outer_diameter_coil { get; set; }
        public string inner_diameter_coil { get; set; }
        //*/

        //para realizar la comparacion    
        public bool Equals(class_v3 other)
        {
            if (other is null)
                return false;

            return
                this.Object == other.Object
                    && this.Grade == other.Grade
                    && this.Customer == other.Customer
                    && this.Shape == other.Shape
                    && this.Customer_part_number == other.Customer_part_number
                    && this.Surface == other.Surface
                    && this.Gauge___Metric == other.Gauge___Metric
                    && this.Mill == other.Mill
                    && this.Width___Metr == other.Width___Metr
                    && this.Length_mm_ == other.Length_mm_
                    && this.activo == other.activo
                    && this.commodity == other.commodity
                    && this.flatness_metric == other.flatness_metric
                    && this.surface_treatment == other.surface_treatment
                    && this.coating_weight == other.coating_weight
                    && this.customer_part_msa == other.customer_part_msa
                    && this.outer_diameter_coil == other.outer_diameter_coil
                    && this.inner_diameter_coil == other.inner_diameter_coil
                ;
        }

        public override bool Equals(object obj) => Equals(obj as class_v3);
        public override int GetHashCode() => (Object).GetHashCode();
    }
}