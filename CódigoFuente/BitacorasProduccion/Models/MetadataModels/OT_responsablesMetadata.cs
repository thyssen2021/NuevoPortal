using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class OT_responsablesMetadata
    {

        public int id { get; set; }

        [Required]
        [Display(Name = "Empleado")]
        public int id_empleado { get; set; }

        [Required]
        [Display(Name = "¿Es supervisor?")]
        public bool supervisor { get; set; }

        [Display(Name = "Supervisor")]
        [RequiredIf("supervisor", false, ErrorMessage = "El campo supervisor es obligatorio.")]
        public Nullable<int> id_supervisor { get; set; }

        [Display(Name = "Activo")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(OT_responsablesMetadata))]
    public partial class OT_responsables
    {
    }
}