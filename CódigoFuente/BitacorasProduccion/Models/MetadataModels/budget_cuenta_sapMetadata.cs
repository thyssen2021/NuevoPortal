using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class budget_cuenta_sapMetadata
    {
        [Display(Name = "Mapping")]
        public int id_mapping { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(8, MinimumLength = 2)]
        [Display(Name = "SAP Account")]
        public string sap_account { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(40, MinimumLength = 2)]
        [Display(Name = "Name")]
        public string name { get; set; }

        [Display(Name = "Status")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(budget_cuenta_sapMetadata))]
    public partial class budget_cuenta_sap
    {
        
    }
}