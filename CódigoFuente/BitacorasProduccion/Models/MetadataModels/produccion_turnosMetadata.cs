using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class produccion_turnosMetadata
    {
        [Required]
        [Display(Name = "Planta")]
        public int clave_planta { get; set; }

        [Display(Name = "Valor")]
        [Range(1, 3, ErrorMessage = "Positivo menor o igual a 3")]
        public int valor { get; set; }

        [Display(Name = "Turno")]
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string descripcion { get; set; }

        [Display(Name = "Hora inicio")]
        [RegularExpression(@"^(?:[01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$", ErrorMessage = "Ingrese la hora en el fomato hh:mm:ss.")]
        public System.TimeSpan hora_inicio { get; set; }

        [Display(Name = "Hora fin")]
        [RegularExpression(@"^(?:[01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$", ErrorMessage = "Ingrese la hora en el fomato hh:mm:ss.")]
        public System.TimeSpan hora_fin { get; set; }

        [Display(Name = "Estado")]
        public Nullable<bool> activo { get; set; }
    }

    [MetadataType(typeof(produccion_turnosMetadata))]
    public partial class produccion_turnos
    {

    }
}