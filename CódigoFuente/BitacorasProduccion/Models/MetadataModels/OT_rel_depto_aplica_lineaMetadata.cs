using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class OT_rel_depto_aplica_lineaMetadata
    {       
        [Required]
        public int id_area { get; set; }
    }
    [MetadataType(typeof(OT_rel_depto_aplica_lineaMetadata))]
    public partial class OT_rel_depto_aplica_linea {
        
    }
}