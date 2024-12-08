using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class inspeccion_fallasMetadata
    {
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Categoria")]
        public int id_categoria_falla { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(80, MinimumLength = 2)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Aplica en Cálculo")]
        public bool aplica_en_calculo { get; set; }
        public bool dano_interno { get; set; }
        public bool dano_externo { get; set; }
        [Display(Name = "Estado")]
        public bool activo { get; set; }
    }


    [MetadataType(typeof(inspeccion_fallasMetadata))]
    public partial class inspeccion_fallas
    {

    }
}