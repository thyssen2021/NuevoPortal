using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    // 1. AÑADE ESTA CLASE PARCIAL para enlazar los metadatos con tu entidad de EF
    [MetadataType(typeof(Portal_2_0.Models.BG_IHS_division_blackoutMetadata))]
    public partial class BG_IHS_division_blackout
    {
        // Esta clase parcial debe estar en el namespace de tus entidades de EF
    }
}

namespace Portal_2_0.Models
{
    // 2. ESTA ES TU CLASE DE METADATOS (reemplaza la clase vacía)
    public class BG_IHS_division_blackoutMetadata
    {
        [Display(Name = "Fecha Inicio")]
        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_inicio { get; set; }

        [Display(Name = "Fecha Fin")]
        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_fin { get; set; }

        [Display(Name = "Comentario")]
        [StringLength(255, ErrorMessage = "El comentario no debe exceder los 255 caracteres.")]
        public string comentario { get; set; }
    }
}