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
        [StringLength(16, MinimumLength =16, ErrorMessage ="La longitud debe ser de 16 dígitos.")]
        [Display(Name = "Business Card")]
        public string business_card { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Solicitud")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_comprobacion { get; set; }
        
        [Display(Name = "Aceptación Jefe")]
        public Nullable<System.DateTime> fecha_aceptacion_jefe_area { get; set; }

        [Display(Name = "Aceptación Controlling")]
        public Nullable<System.DateTime> fecha_aceptacion_controlling { get; set; }

        [Display(Name = "Confirmación Cuentas por Pagar")]
        public Nullable<System.DateTime> fecha_aceptacion_contabilidad { get; set; }

        [Display(Name = "Confirmación Nóminas")]
        public Nullable<System.DateTime> fecha_aceptacion_nomina { get; set; }


        [StringLength(355)]
        [Display(Name = "Motivo de Rechazo")]
        public string comentario_rechazo { get; set; }

        [StringLength(355)]
        [Display(Name = "Comentario Adicional")]
        public string comentario_adicional { get; set; }

        [Display(Name = "Estado")]
        public string estatus { get; set; }

        [Display(Name = "Jefe Directo")]
        public int id_jefe_directo { get; set; }

        [Display(Name = "Controlling")]
        public Nullable<int> id_controlling { get; set; }

        [Display(Name = "Cuentas por Pagar")]
        public Nullable<int> id_contabilidad { get; set; }

        [Display(Name = "Nóminas")]
        public Nullable<int> id_nomina { get; set; }

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