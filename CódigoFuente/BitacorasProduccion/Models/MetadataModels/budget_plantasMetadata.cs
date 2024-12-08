using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class budget_plantasMetadata
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(40, MinimumLength = 2)]
        [Display(Name = "Description")]
        public string descripcion { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(6, MinimumLength = 2)]
        [Display(Name = "SAP code")]
        public string codigo_sap { get; set; }

        [Display(Name = "Status")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(budget_plantasMetadata))]
    public partial class budget_plantas
    {
       
    }
}