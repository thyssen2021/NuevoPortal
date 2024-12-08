using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class PFA_DepartmentMetadata
    {
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Estatus")]
        public bool activo { get; set; }
    }
    [MetadataType(typeof(PFA_DepartmentMetadata))]
    public partial class PFA_Department
    {

    }
}