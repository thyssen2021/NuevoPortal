using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class produccion_lineasMetadata
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(25, MinimumLength = 3)]
        [Display(Name = "Valor")]
        public string linea { get; set; }

        [Display(Name = "Planta")]
        [Required]
        public int clave_planta { get; set; }
        [Display(Name = "Estado")]
        public Nullable<bool> activo { get; set; }

        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Ingrese una dirección ip válida.")]
        public string ip { get; set; }
    }

    [MetadataType(typeof(produccion_lineasMetadata))]
    public partial class produccion_lineas
    {

    }
}