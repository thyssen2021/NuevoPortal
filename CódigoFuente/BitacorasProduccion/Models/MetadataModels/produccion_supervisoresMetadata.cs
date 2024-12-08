using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class produccion_supervisoresMetadata
    {
        [Required(ErrorMessage = "El campo empleado es requerido")]
        [Display(Name = "Empleado")]
        public int id_empleado { get; set; }
        [Required(ErrorMessage = "El campo planta es requerido")]
        [Display(Name = "Planta")]
        public int clave_planta { get; set; }

        [Display(Name = "Estado")]
        public Nullable<bool> activo { get; set; }
    }

    [MetadataType(typeof(produccion_supervisoresMetadata))]
    public partial class produccion_supervisores
    {

    }
}