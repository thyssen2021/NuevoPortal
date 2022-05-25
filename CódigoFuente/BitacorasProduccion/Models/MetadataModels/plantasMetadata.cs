using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class plantasMetadata
    {
        [Display(Name = "Clave")]
        public int clave { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }
        [Display(Name = "Estatus")]
        public bool activo { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Código SAP")]
        [StringLength(4, MinimumLength = 2)]
        public string codigoSap { get; set; }
    }

    [MetadataType(typeof(plantasMetadata))]
    public partial class plantas
    {

    }
}