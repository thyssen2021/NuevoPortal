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
        public int id { get; set; }
        public int id_bg_forecast_reporte { get; set; }
        public string business_and_plant { get; set; }
        public bool calculo_activo { get; set; }
        public string sap_invoice_code { get; set; }
        public string invoiced_to { get; set; }
        public string number_sap_client { get; set; }
        public string shipped_to { get; set; }
        public string own_cm { get; set; }
        public string route { get; set; }
        public string plant { get; set; }
        public bool external_process { get; set; }
        public string mill { get; set; }
        public string sap_master_coil { get; set; }
        public string part_description { get; set; }
        public string part_number { get; set; }
        public string production_line { get; set; }
        public string vehicle { get; set; }
        public Nullable<decimal> parts_auto { get; set; }
        public Nullable<decimal> strokes_auto { get; set; }
        public string material_type { get; set; }
        public string shape { get; set; }
        public Nullable<decimal> initial_weight_part { get; set; }
        public Nullable<decimal> net_weight_part { get; set; }
        public bool scrap_consolidation { get; set; }
        public Nullable<decimal> ventas_part { get; set; } //moneda
        public Nullable<decimal> material_cost_part { get; set; } //moneda
        public Nullable<decimal> cost_of_outside_processor { get; set; } //moneda
        public Nullable<decimal> additional_material_cost_part { get; set; } //moneda
        public Nullable<decimal> outgoing_freight_part { get; set; } //moneda
        public string freights_income { get; set; }
        public string outgoing_freight { get; set; }

        [Display(Name = "Cat 1")]
        public string cat_1 { get; set; }

        [Description("PO in hand")]
        [Display(Name = "PO in hand")]
        public string cat_2 { get; set; }

        [Display(Name = "Cat 3")]
        public string cat_3 { get; set; }

        [Display(Name = "Cat 4")]
        public string cat_4 { get; set; }

    }

    [MetadataType(typeof(BG_Forecast_itemMetadata))]
    public partial class BG_Forecast_item
    {
        //agregar aquellos campos que son calulados
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:2}", ApplyFormatInEditMode = false)]
        [Display(Name = "VAS/PART")]
        public decimal? vas_part
        {
            get
            {
                return (this.ventas_part.HasValue ? this.ventas_part.Value : 0)
                - (this.material_cost_part.HasValue ? this.material_cost_part.Value : 0)
                - (this.cost_of_outside_processor.HasValue ? this.cost_of_outside_processor.Value : 0)
                ;

            }
        }

    }

}