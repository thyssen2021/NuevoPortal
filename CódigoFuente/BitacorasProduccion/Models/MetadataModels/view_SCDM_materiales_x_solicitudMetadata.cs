using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class view_SCDM_materiales_x_solicitudMetadata
    {
        [Display(Name = "ID")]
        public long id { get; set; }

        [Display(Name = "Número Solicitud")]
        public Nullable<int> numero_solicitud { get; set; }

        [Display(Name = "Tipo de Solicitud")]
        public string tipo_solicitud { get; set; }

        [Display(Name = "Tipo de Cambio")]
        public string tipo_cambio { get; set; }

        [Display(Name = "Tipo de Material")]
        public string tipo_material { get; set; }

        [Display(Name = "Número Material")]
        public string numero_material { get; set; }

        [Display(Name = "Planta Solicitud")]
        public string planta { get; set; }

        [Display(Name = "Solicitante")]
        public string solicitante { get; set; }

        [Display(Name = "Prioridad")]
        public string prioridad { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Solicitud")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_solicitud { get; set; }
    }

    [MetadataType(typeof(view_SCDM_materiales_x_solicitudMetadata))]
    public partial class view_SCDM_materiales_x_solicitud
    {

        [Display(Name = "Estatus")]
        public string Estatus
        {
            get
            {
                using (var db = new Portal_2_0Entities())
                {
                    return db.SCDM_solicitud.Find(this.numero_solicitud).estatusTexto;
                }
            }
        }


    }
}