using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class GV_usuariosMetadata
    {

        [Display(Name = "Clave")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Empleado")]
        public int id_empleado { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "Departamento")]
        public string departamento { get; set; }
    }

    [MetadataType(typeof(GV_usuariosMetadata))]
    public partial class GV_usuarios
    {

    }
}