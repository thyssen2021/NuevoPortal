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
                ;
        }

        public override bool Equals(object obj) => Equals(obj as class_v3);
        public override int GetHashCode() => (Object).GetHashCode();
    }
}