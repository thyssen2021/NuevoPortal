using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    
    public class OT_zona_fallaMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Línea de Producción")]
        public int id_linea { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Zona de Falla")]
        public string zona_falla { get; set; }

        [Display(Name = "Status")]
        public bool activo { get; set; }
        
    }

    [MetadataType(typeof(OT_zona_fallaMetadata))]
    public partial class OT_zona_falla
    {

    }
}