using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class PM_usuarios_capturistasMetadata
    {
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Empleado")]
        public int id_empleado { get; set; }

        [Required]
        [Display(Name = "Departamento")]
        public int id_departamento { get; set; }

        [Display(Name = "Estado")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(PM_usuarios_capturistasMetadata))]
    public partial class PM_usuarios_capturistas
    {

    }
}