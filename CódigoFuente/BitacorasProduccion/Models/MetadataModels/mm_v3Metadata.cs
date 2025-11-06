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
        ///*
        //propiedades del modelo de BD
        public string Material { get; set; }
        public string Plnt { get; set; }
        public string MS { get; set; }
        public string Material_Description { get; set; }
        public string Type_of_Material { get; set; }
        public string Type_of_Metal { get; set; }
        public string Old_material_no_ { get; set; }
        public string Head_and_Tails_Scrap_Conciliation { get; set; }
        public string Engineering_Scrap_conciliation { get; set; }
        public string Business_Model { get; set; }
        public string Re_application { get; set; }
        public string IHS_number_1 { get; set; }
        public string IHS_number_2 { get; set; }
        public string IHS_number_3 { get; set; }
        public string IHS_number_4 { get; set; }
        public string IHS_number_5 { get; set; }
        public string Type_of_Selling { get; set; }
        public string Package_Pieces { get; set; }
        public Nullable<double> Gross_weight { get; set; }
        public string Un_ { get; set; }
        public Nullable<double> Net_weight { get; set; }
        public string Un_1 { get; set; }
        public Nullable<double> Thickness { get; set; }
        public Nullable<double> Width { get; set; }
        public Nullable<double> Advance { get; set; }
        public Nullable<double> Head_and_Tail_allowed_scrap { get; set; }
        public Nullable<double> Pieces_per_car { get; set; }
        public Nullable<double> Initial_Weight { get; set; }
        public Nullable<double> Min_Weight { get; set; }
        public Nullable<double> Maximum_Weight { get; set; }
        public bool activo { get; set; }
        public Nullable<int> num_piezas_golpe { get; set; }
        public string unidad_medida { get; set; }
        public string size_dimensions { get; set; }
        public string material_descripcion_es { get; set; }
        public Nullable<double> angle_a { get; set; }
        public Nullable<double> angle_b { get; set; }
        public Nullable<double> real_net_weight { get; set; }
        public Nullable<double> real_gross_weight { get; set; }
        public string double_pieces { get; set; }
        public string coil_position { get; set; }
        public Nullable<double> maximum_weight_tol_positive { get; set; }
        public Nullable<double> maximum_weight_tol_negative { get; set; }
        public Nullable<double> minimum_weight_tol_positive { get; set; }
        public Nullable<double> minimum_weight_tol_negative { get; set; }
        public Nullable<bool> Almacen_Norte { get; set; }
        public string Tipo_de_Transporte { get; set; }
        public Nullable<System.DateTime> Tkmm_SOP { get; set; }
        public Nullable<System.DateTime> Tkmm_EOP { get; set; }
        public Nullable<double> Pieces_Pac { get; set; }
        public Nullable<double> Stacks_Pac { get; set; }
        public string Type_of_Pallet { get; set; }
        //*/

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
                && this.material_descripcion_es == other.material_descripcion_es
               && this.angle_a == other.angle_a
               && this.angle_b == other.angle_b
               && this.real_net_weight == other.real_net_weight
               && this.real_gross_weight == other.real_gross_weight
              && this.double_pieces == other.double_pieces
              && this.coil_position == other.coil_position
               && this.maximum_weight_tol_positive == other.maximum_weight_tol_positive
              && this.maximum_weight_tol_negative == other.maximum_weight_tol_negative
               && this.minimum_weight_tol_positive == other.minimum_weight_tol_positive
               && this.minimum_weight_tol_negative == other.minimum_weight_tol_negative
               && this.Almacen_Norte == other.Almacen_Norte
               && this.Tipo_de_Transporte == other.Tipo_de_Transporte
               && this.Tkmm_SOP == other.Tkmm_SOP
               && this.Tkmm_EOP == other.Tkmm_EOP
               && this.Pieces_Pac == other.Pieces_Pac
               && this.Stacks_Pac == other.Stacks_Pac
               && this.Type_of_Pallet == other.Type_of_Pallet
                ;
        }

        public override bool Equals(object obj) => Equals(obj as mm_v3);
        public override int GetHashCode() => (Material, Plnt).GetHashCode();
    }
}