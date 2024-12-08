using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
   
    public class BG_IHS_rel_regionesMetadata
    {
                
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Display(Name = "Región")]
        public Nullable<int> id_bg_ihs_region { get; set; }


        [MaxLength(50)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Planta de Producción")]
        public string production_plant { get; set; }

    }

    [MetadataType(typeof(BG_IHS_rel_regionesMetadata))]
    public partial class BG_IHS_rel_regiones
    {

    }
}