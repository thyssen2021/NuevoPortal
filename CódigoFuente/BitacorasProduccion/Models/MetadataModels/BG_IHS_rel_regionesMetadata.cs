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

        [Display(Name = "Versión IHS")]
        public int id_ihs_version { get; set; }

        [Display(Name = "Región")]
        public Nullable<int> id_bg_ihs_region { get; set; }

        [Display(Name = "Planta")]
        public int id_bg_ihs_plant { get; set; }
    }

    [MetadataType(typeof(BG_IHS_rel_regionesMetadata))]
    public partial class BG_IHS_rel_regiones
    {

    }
}