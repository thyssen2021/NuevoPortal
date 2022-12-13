using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class GV_comprobacionMetadata
    {

        [Display(Name = "Folio Solicitud")]
        public int id_gv_solicitud { get; set; }

        [Required]
        [Display(Name = "Centro de Costo")]
        public int id_centro_costo { get; set; }

        [Required]
        [StringLength(16, MinimumLength =16, ErrorMessage ="La longitud debe ser de 16 dígitos.")]
        [Display(Name = "Business Card")]
        public string business_card { get; set; }
        public Nullable<System.DateTime> fecha_aceptacion_jefe_area { get; set; }
        public Nullable<System.DateTime> fecha_aceptacion_controlling { get; set; }
        public Nullable<System.DateTime> fecha_aceptacion_contabilidad { get; set; }
        public Nullable<System.DateTime> fecha_aceptacion_nomina { get; set; }

        [StringLength(355)]
        [Display(Name = "Motivo de Rechazo")]
        public string comentario_rechazo { get; set; }

        [StringLength(355)]
        [Display(Name = "Comentario Adicional")]
        public string comentario_adicional { get; set; }

        [Display(Name = "Estado")]
        public string estatus { get; set; }

    }

    [MetadataType(typeof(GV_comprobacionMetadata))]
    public partial class GV_comprobacion
    {
        //concatena el nombre
        [NotMapped]
        [Display(Name = "Planta")]
        public int clave_planta { get; set; }

    }
}