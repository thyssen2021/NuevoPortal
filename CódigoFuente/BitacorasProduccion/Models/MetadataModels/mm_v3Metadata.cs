using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class mm_v3Metadata
    {
    }

    [MetadataType(typeof(mm_v3Metadata))]
    public partial class mm_v3 : IEquatable<mm_v3>
    {
        //para realizar la comparacion    

        public bool Equals(mm_v3 other)
        {
            if (other is null)
                return false;

            return
                this.Material == other.Material
                && this.Plnt == other.Plnt
               && this.MS == other.MS
               && this.Material_Description == other.Material_Description
                && this.Type_of_Material == other.Type_of_Material
                && this.Type_of_Metal == other.Type_of_Metal
                && this.Old_material_no_ == other.Old_material_no_
                && this.Head_and_Tails_Scrap_Conciliation == other.Head_and_Tails_Scrap_Conciliation
                && this.Engineering_Scrap_conciliation == other.Engineering_Scrap_conciliation
                && this.Business_Model == other.Business_Model
                && this.Re_application == other.Re_application
                && this.IHS_number_1 == other.IHS_number_1
                && this.IHS_number_2 == other.IHS_number_2
                && this.IHS_number_3 == other.IHS_number_3
                && this.IHS_number_4 == other.IHS_number_4
                && this.IHS_number_5 == other.IHS_number_5
                && this.Type_of_Selling == other.Type_of_Selling
                && this.Package_Pieces == other.Package_Pieces
                && this.Gross_weight == other.Gross_weight
                && this.Un_ == other.Un_
                && this.Net_weight == other.Net_weight
                && this.Un_1 == other.Un_1
                && this.Thickness == other.Thickness
                && this.Width == other.Width
                && this.Advance == other.Advance
                && this.Head_and_Tail_allowed_scrap == other.Head_and_Tail_allowed_scrap
                && this.Pieces_per_car == other.Pieces_per_car
                 && this.Initial_Weight == other.Initial_Weight
                && this.Min_Weight == other.Min_Weight
                && this.Maximum_Weight == other.Maximum_Weight
                && this.activo == other.activo
                && this.num_piezas_golpe == other.num_piezas_golpe
                && this.unidad_medida == other.unidad_medida
                && this.size_dimensions == other.size_dimensions
                ;
        }

        public override bool Equals(object obj) => Equals(obj as mm_v3);
        public override int GetHashCode() => (Material, Plnt).GetHashCode();
    }
}