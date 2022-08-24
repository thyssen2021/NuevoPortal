using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class orden_trabajoMetadata
    {

        [Display(Name = "Folio")]
        public int id { get; set; }

        [Display(Name = "Solicitante")]
        public int id_solicitante { get; set; }

        [Display(Name = "Asignación")]
        public Nullable<int> id_asignacion { get; set; }

        [Display(Name = "Responsable")]
        public Nullable<int> id_responsable { get; set; }

        [Display(Name = "Área")]
        public int id_area { get; set; }

        [Display(Name = "Línea")]
        public Nullable<int> id_linea { get; set; }

        [Display(Name = "Fecha solicitud")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_solicitud { get; set; }

        [Display(Name = "Urgencia")]
        [StringLength(15)]
        [Required(AllowEmptyStrings = false)]
        public string nivel_urgencia { get; set; }

        [Display(Name = "Título")]
        [StringLength(80, MinimumLength = 5)]
        [Required(AllowEmptyStrings = false)]
        public string titulo { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(300, MinimumLength = 5)]
        [Required(AllowEmptyStrings = false)]
        public string descripcion { get; set; }
        [Display(Name = "Fecha Asignación")]
        public Nullable<System.DateTime> fecha_asignacion { get; set; }

        [Display(Name = "Fecha en Proceso")]
        public Nullable<System.DateTime> fecha_en_proceso { get; set; }

        [Display(Name = "Fecha Cierre")]
        public Nullable<System.DateTime> fecha_cierre { get; set; }

        [Display(Name = "Estatus")]
        [Required(AllowEmptyStrings = false)]
        public string estatus { get; set; }

        [Display(Name = "Comentario Cierre")]
        [StringLength(300, MinimumLength = 3)]
        public string comentario { get; set; }

        [Display(Name = "Grupo de Trabajo")]
        [RequiredIf("tpm", true, ErrorMessage = "El grupo de trabajo es requerido")]
        public Nullable<int> id_grupo_trabajo { get; set; }

        [Display(Name = "TPM")]
        [Required]
        public bool tpm { get; set; }

        [Display(Name = "Número de tarjeta")]
        [StringLength(25, MinimumLength = 1)]
        [RequiredIf("tpm", true, ErrorMessage = "El número de tarjeta es requerido")]
        public string numero_tarjeta { get; set; }

        [Display(Name = "Zona de Falla")]
        public Nullable<int> id_zona_falla { get; set; }

    }


    [MetadataType(typeof(orden_trabajoMetadata))]
    public partial class orden_trabajo
    {
        [NotMapped]
        public HttpPostedFileBase PostedFileSolicitud_1 { get; set; }
        [NotMapped]
        public HttpPostedFileBase PostedFileSolicitud_2 { get; set; }
        [NotMapped]
        public HttpPostedFileBase PostedFileSolicitud_3 { get; set; }
        [NotMapped]
        public HttpPostedFileBase PostedFileSolicitud_4 { get; set; }
        [NotMapped]
        public HttpPostedFileBase PostedFileCierre_1 { get; set; }
        [NotMapped]
        public HttpPostedFileBase PostedFileCierre_2 { get; set; }
        [NotMapped]
        public HttpPostedFileBase PostedFileCierre_3 { get; set; }
        [NotMapped]
        public HttpPostedFileBase PostedFileCierre_4 { get; set; }
    }
}