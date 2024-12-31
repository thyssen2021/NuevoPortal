using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{

    public class BG_Forecast_cat_inventory_ownMetadata
    {

        [Display(Name = "Clave")]
        public int id { get; set; }

        [Display(Name = "Reporte")]
        public int id_bg_forecast_reporte { get; set; }

        [Display(Name = "Orden")]
        public int orden { get; set; }

        [Display(Name = "Mes")]
        public int mes { get; set; }

        [Display(Name = "Cantidad")]
        public double cantidad { get; set; }



    }

    [MetadataType(typeof(BG_Forecast_cat_inventory_ownMetadata))]
    public partial class BG_Forecast_cat_inventory_own
    {

    }
}