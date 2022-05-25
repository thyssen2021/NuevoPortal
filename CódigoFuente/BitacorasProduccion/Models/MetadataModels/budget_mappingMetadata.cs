using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class budget_mappingMetadata
    {
        [Required]
        [Display(Name = "Mapping Bridge")]
        public int id_mapping_bridge { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(40, MinimumLength = 2)]
        [Display(Name = "Description")]
        public string descripcion { get; set; }
    }

    [MetadataType(typeof(budget_mappingMetadata))]
    public partial class budget_mapping
    {

    }
}