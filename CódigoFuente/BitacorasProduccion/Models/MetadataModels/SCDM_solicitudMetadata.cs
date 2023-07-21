using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_solicitudMetadata
    {
        [Display(Name = "Folio")]
        public int id { get; set; }

        [Display(Name = "Tipo de Solicitud")]
        public int id_tipo_solicitud { get; set; }

        [Display(Name = "Tipo de Cambio")]
        public Nullable<int> id_tipo_cambio { get; set; }

        [Display(Name = "Prioridad")]
        public int id_prioridad { get; set; }

        [Display(Name = "Solicitante")]
        public int id_solicitante { get; set; }

        [StringLength(180, MinimumLength = 1)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [StringLength(250, MinimumLength = 1)]
        [Display(Name = "Justificación")]
        public string justificacion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Creación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_creacion { get; set; }

        [Display(Name = "¿On hold?")]
        public bool on_hold { get; set; }

        [Display(Name = "¿Activo?")]
        public bool activo { get; set; }

    }

    [MetadataType(typeof(SCDM_solicitudMetadata))]
    public partial class SCDM_solicitud
    {
        [NotMapped]
        public HttpPostedFileBase PostedFileSolicitud_1 { get; set; }
        [NotMapped]
        public HttpPostedFileBase PostedFileSolicitud_2 { get; set; }
        [NotMapped]
        public HttpPostedFileBase PostedFileSolicitud_3 { get; set; }
    }
}