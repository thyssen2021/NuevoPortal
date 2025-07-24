using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{

    public class BG_Forecast_cat_defaultsMetadata
    {
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Valor de Scrap de Acero (Puebla)")]
        public double scrap_acero_valor_puebla { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Ganancia de Scrap de Acero (Puebla)")]
        public double scrap_acero_ganancia_puebla { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Valor de Scrap de Aluminio (Puebla)")]
        public double scrap_aluminio_valor_puebla { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Ganancia de Scrap de Alumino (Puebla)")]
        public double scrap_aluminio_ganancia_puebla { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Valor de Scrap de Acero (Silao)")]
        public double scrap_acero_valor_silao { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Ganancia de Scrap de Acero (Silao)")]
        public double scrap_acero_ganancia_silao { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Valor de Scrap de Aluminio (Silao)")]
        public double scrap_aluminio_valor_silao { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Ganancia de Scrap de Aluminio (Silao)")]
        public double scrap_aluminio_ganancia_silao { get; set; }

    }

    [MetadataType(typeof(BG_Forecast_cat_defaultsMetadata))]
    public partial class BG_Forecast_cat_defaults
    {

    }
}