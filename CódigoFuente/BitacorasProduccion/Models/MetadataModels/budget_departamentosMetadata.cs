using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class budget_departamentosMetadata
    {
        [Required]
        [Display(Name = "Plant")]
        public int id_budget_planta { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(40, MinimumLength = 2)]
        [Display(Name = "Description")]
        public string descripcion { get; set; }

        [Display(Name = "Status")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(budget_departamentosMetadata))]
    public partial class budget_departamentos
    {

    }
}