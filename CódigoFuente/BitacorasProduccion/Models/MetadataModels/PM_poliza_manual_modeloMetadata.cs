using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class PM_poliza_manual_modeloMetadata
    {
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(80, MinimumLength = 5)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Creación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_creacion { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(250, MinimumLength = 5)]
        [Display(Name = "Comentario")]
        public string comentario { get; set; }

        [Display(Name = "Estado")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(PM_poliza_manual_modeloMetadata))]
    public partial class PM_poliza_manual_modelo
    {

    }
}