using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{ 
    public class BG_Forecast_reporteMetadata
    {
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Reporte")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha { get; set; }

        [MaxLength(80)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Activo")]
        public bool activo { get; set; }
       
      
    }

    [MetadataType(typeof(BG_Forecast_reporteMetadata))]
    public partial class BG_Forecast_reporte
    {

    }
}