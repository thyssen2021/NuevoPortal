using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class bom_en_sapMetadata
    {
    }

    [MetadataType(typeof(bom_en_sapMetadata))]
    public partial class bom_en_sap : IEquatable<bom_en_sap>
    {
        public string Material { get; set; }
        public string Plnt { get; set; }
        public string BOM { get; set; }
        public string AltBOM { get; set; }
        public string Item { get; set; }
        public string Component { get; set; }
        public Nullable<double> Quantity { get; set; }
        public string Un { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> LastDateUsed { get; set; }

        //para realizar la comparacion    

        public bool Equals(bom_en_sap other)
        {
            if (other is null)
                return false;

            return this.Material == other.Material &&
                   this.Plnt == other.Plnt &&
                   this.BOM == other.BOM &&
                   this.AltBOM == other.AltBOM &&
                   this.Item == other.Item &&
                   this.Component == other.Component;
        }

        public override bool Equals(object obj) => Equals(obj as bom_en_sap);
        public override int GetHashCode()
        {
            // Se combinan los HashCodes de todas las propiedades de la clave primaria.
            return (Material, Plnt, BOM, AltBOM, Item, Component).GetHashCode();
        }
    }
}