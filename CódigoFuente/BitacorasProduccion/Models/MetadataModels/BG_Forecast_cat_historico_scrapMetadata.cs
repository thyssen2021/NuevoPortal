using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{

    public class BG_Forecast_cat_historico_scrapMetadata
    {
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Planta")]
        public int id_planta { get; set; }


        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Tipo Metal")]
        public string tipo_metal { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Valor Scrap")]
        public Nullable<double> scrap { get; set; }

        [Required]
        [Display(Name = "Valor ganancia")]
        [DataType(DataType.Currency)]
        public double scrap_ganancia { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Mes")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha { get; set; }

    }

    [MetadataType(typeof(BG_Forecast_cat_historico_scrapMetadata))]
    public partial class BG_Forecast_cat_historico_scrap
    {

    }
}