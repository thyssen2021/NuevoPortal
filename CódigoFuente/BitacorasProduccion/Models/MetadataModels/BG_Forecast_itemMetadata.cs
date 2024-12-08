using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class BG_Forecast_itemMetadata
    {
        [Display(Name = "Folio")]
        public int id { get; set; }

        [Display(Name = "Reporte")]
        public int id_bg_forecast_reporte { get; set; }

        [Display(Name = "Pos")]
        public int pos { get; set; }

        [MaxLength(30)]
        [Display(Name = "Business & plant")]
        public string business_and_plant { get; set; }

        [Display(Name = "Aplica en Cálculo")]
        public bool calculo_activo { get; set; }

        [MaxLength(25)]
        [Display(Name = "SAP - Invoice Code")]
        public string sap_invoice_code { get; set; }

        [MaxLength(100)]
        [Display(Name = "Invoiced to")]
        public string invoiced_to { get; set; }

        [MaxLength(8)]
        [Display(Name = "Number SAP Client")]
        public string number_sap_client { get; set; }

        [MaxLength(100)]
        [Display(Name = "Shipped to")]
        public string shipped_to { get; set; }

        [MaxLength(5)]
        [Display(Name = "OWN/CM")]
        public string own_cm { get; set; }

        [MaxLength(25)]
        [Display(Name = "Route")]
        public string route { get; set; }

        [MaxLength(4)]
        [Display(Name = "Plant")]
        public string plant { get; set; }

        [MaxLength(25)]
        [Display(Name = "External Processor")]
        public bool external_processor { get; set; }

        [MaxLength(100)]
        [Display(Name = "Mill")]
        public string mill { get; set; }

        [MaxLength(12)]
        [Display(Name = "SAP Master Coil")]
        public string sap_master_coil { get; set; }

        [MaxLength(100)]
        [Display(Name = "Part description")]
        public string part_description { get; set; }

        [MaxLength(100)]
        [Display(Name = "Part number")]
        public string part_number { get; set; }

        [MaxLength(15)]
        [Display(Name = "Production Line")]
        public string production_line { get; set; }

        [MaxLength(150)]
        [Display(Name = "Vehicle IHS")]
        public string vehicle { get; set; }

        [Display(Name = "Parts/Auto [-]")]
        public Nullable<double> parts_auto { get; set; }

        [Display(Name = "Strokes/Auto[-]")]
        public Nullable<double> strokes_auto { get; set; }

        [MaxLength(20)]
        [Display(Name = "Material Type")]
        public string material_type { get; set; }

        [MaxLength(5)]
        [Display(Name = "Shape")]
        public string shape { get; set; }

        [Display(Name = "Initial Weight/Part[kg]")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public Nullable<double> initial_weight_part { get; set; }

        [Display(Name = "Net Weight/Part[kg]")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public Nullable<double> net_weight_part { get; set; }

        [Display(Name = "Scrap Consolidation")]
        public bool scrap_consolidation { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "Ventas/Part[USD]")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<double> ventas_part { get; set; } //moneda

        [Range(0, Int32.MaxValue)]
        [Display(Name = "Material Cost/Part[USD]")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<double> material_cost_part { get; set; } //moneda

        [Range(0, Int32.MaxValue)]
        [Display(Name = "Cost of Outside Processor")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<double> cost_of_outside_processor { get; set; } //moneda

        [Range(0, Int32.MaxValue)]
        [Display(Name = "Additional Material Cost/Part[USD]")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<double> additional_material_cost_part { get; set; } //moneda

        [Range(0, Int32.MaxValue)]
        [Display(Name = "Outgoing freight/Part[USD]")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<double> outgoing_freight_part { get; set; } //moneda

        [MaxLength(100)]
        [Display(Name = "Freights Income")]
        public string freights_income { get; set; }

        [MaxLength(100)]
        [Display(Name = "Outgoing Freight")]
        public string outgoing_freight { get; set; }

        [MaxLength(20)]
        [Display(Name = "Cat 1")]
        public string cat_1 { get; set; }

        [MaxLength(20)]
        [Description("PO in hand")]
        [Display(Name = "PO in hand")]
        public string cat_2 { get; set; }

        [MaxLength(20)]
        [Display(Name = "Cat 3")]
        public string cat_3 { get; set; }

        [MaxLength(20)]
        [Display(Name = "Cat 4")]
        public string cat_4 { get; set; }

        [Display(Name = "Freights Income USD/PART")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<double> freights_income_usd_part { get; set; }

        [Display(Name = "Maniobras USD/PART")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<double> maniobras_usd_part { get; set; }

        [Display(Name = "Custom Expenses USD/PART")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<double> customs_expenses { get; set; }

    }

    [MetadataType(typeof(BG_Forecast_itemMetadata))]
    public partial class BG_Forecast_item : IEquatable<BG_Forecast_item>
    {
        //agregar aquellos campos que son calulados
        [NotMapped]
        [Display(Name = "VAS/PART")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public double? vas_part
        {
            get
            {
                return (this.ventas_part.HasValue ? this.ventas_part.Value : 0)
                - (this.material_cost_part.HasValue ? this.material_cost_part.Value : 0)
                - (this.cost_of_outside_processor.HasValue ? this.cost_of_outside_processor.Value : 0)
                ;

            }
        }

        [NotMapped]
        public string p_id_asociacion_ihs { get; set; }

        //agregar aquellos campos que son calulados
        [NotMapped]
        [Display(Name = "Clave asociacion IHS")]
        public string id_asociacion_ihs
        {
            get
            {
                string clave = string.Empty;

                switch (ElementoIHS.tipo)
                {
                    case TipoElementoIHS.IHS:
                        clave = "I_" + BG_IHS_item.id;
                        break;
                    case TipoElementoIHS.COMBINACION:
                        clave = "C_" + BG_IHS_combinacion.id;
                        break;
                    case TipoElementoIHS.DIVISION:
                        clave = "D_" + BG_IHS_rel_division.id;
                        break;
                }

                return clave;
            }
            set
            {
                this.p_id_asociacion_ihs = value;
            }

        }

        //agregar aquellos campos que son calulados
        [NotMapped]
        [Display(Name = "Elemento IHS")]
        public RelElementoIHS ElementoIHS
        {
            get
            {
                var RelElemento = new RelElementoIHS { descripcionTipo = "Pendiente" };

                //inicializa el objeto de IHS
                if (BG_IHS_combinacion != null)
                {
                    RelElemento.BG_IHS_combinacion = BG_IHS_combinacion;
                    RelElemento.tipo = TipoElementoIHS.COMBINACION;
                    RelElemento.descripcion = BG_IHS_combinacion.vehicle;
                    RelElemento.descripcionTipo = "Combinación";

                }
                else if (BG_IHS_item != null)
                {
                    RelElemento.BG_IHS_item = BG_IHS_item;
                    RelElemento.tipo = TipoElementoIHS.IHS;
                    RelElemento.descripcion = BG_IHS_item.ConcatCodigo;
                    RelElemento.descripcionTipo = "Elemento IHS";
                }
                else if (BG_IHS_rel_division != null)
                {
                    RelElemento.BG_IHS_rel_division = BG_IHS_rel_division;
                    RelElemento.tipo = TipoElementoIHS.DIVISION;
                    RelElemento.descripcion = BG_IHS_rel_division.vehicle;
                    RelElemento.descripcionTipo = "División";
                }

                return RelElemento;
            }
        }


        [NotMapped]
        [Display(Name = "Eng. Scrap/Part")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public double? eng_scrap_part
        {
            get
            {
                return (this.initial_weight_part.HasValue ? this.initial_weight_part.Value : 0)
                - (this.net_weight_part.HasValue ? this.net_weight_part.Value : 0)
                ;

            }
        }

        [NotMapped]
        [Display(Name = "VAS/to")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public double? vas_to
        {
            get
            {

                try
                {
                    //si alguna de las variables es nula retorna 0 en su lugar
                    double? result = ((this.vas_part.HasValue ? this.vas_part.Value : 0) / (this.initial_weight_part.HasValue ? this.initial_weight_part : 0)) * 1000;

                    if (result.HasValue && !Double.IsNaN(result.Value))
                        return result;
                    else
                        return (double?)null;
                }
                catch
                {
                    return (double?)null;
                }
            }
        }

        //para realizar la comparacion    
        public bool Equals(BG_Forecast_item other)
        {
            if (other is null)
                return false;

            return
                this.id == other.id
                && this.id_bg_forecast_reporte == other.id_bg_forecast_reporte
                && this.id_ihs_item == other.id_ihs_item
                && this.id_ihs_combinacion == other.id_ihs_combinacion
                && this.id_ihs_rel_division == other.id_ihs_rel_division
                && this.pos == other.pos
                && this.business_and_plant == other.business_and_plant
                && this.calculo_activo == other.calculo_activo
                && this.sap_invoice_code == other.sap_invoice_code
                && this.invoiced_to == other.invoiced_to
                && this.number_sap_client == other.number_sap_client
                && this.shipped_to == other.shipped_to
                && this.own_cm == other.own_cm
                && this.route == other.route
                && this.plant == other.plant
                && this.external_processor == other.external_processor
                && this.mill == other.mill
                && this.sap_master_coil == other.sap_master_coil
                && this.part_description == other.part_description
                && this.part_number == other.part_number
                && this.production_line == other.production_line
                && this.vehicle == other.vehicle
                && this.parts_auto == other.parts_auto
                && this.strokes_auto == other.strokes_auto
                && this.material_type == other.material_type
                && this.shape == other.shape
                && this.initial_weight_part == other.initial_weight_part
                && this.net_weight_part == other.net_weight_part
                && this.scrap_consolidation == other.scrap_consolidation
                && this.ventas_part == other.ventas_part
                && this.material_cost_part == other.material_cost_part
                && this.cost_of_outside_processor == other.cost_of_outside_processor
                && this.additional_material_cost_part == other.additional_material_cost_part
                && this.outgoing_freight_part == other.outgoing_freight_part
                && this.freights_income == other.freights_income
                && this.outgoing_freight == other.outgoing_freight
                && this.cat_1 == other.cat_1
                && this.cat_2 == other.cat_2
                && this.cat_3 == other.cat_3
                && this.cat_4 == other.cat_4
                ;
        }

        public override bool Equals(object obj) => Equals(obj as class_v3);
        public override int GetHashCode() => (id).GetHashCode();

    }

    public class RelElementoIHS
    {
        public BG_IHS_combinacion BG_IHS_combinacion { get; set; }
        public BG_IHS_item BG_IHS_item { get; set; }
        public BG_IHS_rel_division BG_IHS_rel_division { get; set; }

        public TipoElementoIHS? tipo = null;

        public string descripcion { get; set; }
        public string descripcionTipo { get; set; }
    }

    public enum TipoElementoIHS
    {
        IHS,
        COMBINACION,
        DIVISION,
    }

}