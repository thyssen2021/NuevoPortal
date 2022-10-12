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
        //para realizar la comparacion    

        public bool Equals(bom_en_sap other)
        {
            if (other is null)
                return false;

            return this.Material == other.Material
                && this.Plnt == other.Plnt
                && this.BOM == other.BOM
                && this.AltBOM == other.AltBOM
                && this.Item == other.Item
                && this.Component == other.Component           
                && this.Quantity == other.Quantity
                && this.Un == other.Un
                && this.Created == other.Created
                && this.LastDateUsed == other.LastDateUsed
                ;
        }

        public override bool Equals(object obj) => Equals(obj as bom_en_sap);
        public override int GetHashCode() => (Material, Plnt, BOM, AltBOM, Item, Component, Quantity, Un, Created, LastDateUsed).GetHashCode();
    }
}