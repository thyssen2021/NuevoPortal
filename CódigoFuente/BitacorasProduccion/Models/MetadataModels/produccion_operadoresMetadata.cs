using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class produccion_operadoresMetadata
    {
        [Required(ErrorMessage = "El campo empleado es requerido")]
        [Range(0, Int32.MaxValue, ErrorMessage = "El campo empleado es requerido")]
        [Display(Name = "Empleado")]
        public int id_empleado { get; set; }

        [Required(ErrorMessage = "El campo línea es requerido")]
        [Display(Name = "Línea")]
        [Range(0, Int32.MaxValue, ErrorMessage = "El campo línea es requerido")]
        public int id_linea { get; set; }

        [Display(Name = "Estado")]
        public Nullable<bool> activo { get; set; }
    }

    [MetadataType(typeof(produccion_operadoresMetadata))]
    public partial class produccion_operadores
    {

    }
}