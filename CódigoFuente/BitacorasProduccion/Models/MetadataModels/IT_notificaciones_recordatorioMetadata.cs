using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_notificaciones_recordatorioMetadata
    {

        [Display(Name = "ID")]
        public int id { get; set; }
        [Display(Name = "Actividad")]
        public int id_notificaciones_actividad { get; set; }
                
        [DataType(DataType.Date)]
        [Display(Name = "Fecha programada")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_programada { get; set; }

        [Display(Name = "Elaboró")]
        public Nullable<int> id_sistemas { get; set; }

        [Display(Name = "Días previos de Notificación")]
        public Nullable<int> dias_previos_notificacion { get; set; }
        public bool notificacion_previa_enviada { get; set; }
        public bool notificacion_dia_evento_enviada { get; set; }

        [Display(Name = "Comentario de Cierre")]
        public string comentario_cierre { get; set; }

        [Display(Name = "Estatus")]
        public string estatus { get; set; }
      
    }

    [MetadataType(typeof(IT_notificaciones_recordatorioMetadata))]
    public partial class IT_notificaciones_recordatorio
    {
        [NotMapped]
        [Display(Name = "¿Recordar antes del evento?")]
        public bool aplica_recordatorio { get; set; }     

    }
}