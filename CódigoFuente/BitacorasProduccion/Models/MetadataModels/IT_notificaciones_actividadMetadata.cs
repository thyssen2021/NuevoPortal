using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Models
{
    public class IT_notificaciones_actividadMetadata
    {
        [Display(Name = "Id")]
        public int id { get; set; }

        [Display(Name = "Título")]
        [StringLength(50, MinimumLength = 1)]
        public string titulo { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(300, MinimumLength = 1)]
        public string descripcion { get; set; }

        [Display(Name = "Periodo")]
        [Range(1, Int16.MaxValue)]
        public Nullable<int> periodo { get; set; }

        [Display(Name = "Tipo de Periodo")]
        public string tipo_periodo { get; set; }

        [Display(Name = "¿Es recurrente?")]
        public bool es_recurrente { get; set; }
    }

    [MetadataType(typeof(IT_notificaciones_actividadMetadata))]
    public partial class IT_notificaciones_actividad
    {
        [NotMapped]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_inicio { get; set; }

        [NotMapped]
        [DataType(DataType.Date)]
        [Display(Name = "Hasta:")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_fin { get; set; }

        [NotMapped]
        [Display(Name = "¿Recordar antes del evento?")]
        public bool aplica_recordatorio { get; set; }
        
        [NotMapped]
        [Display(Name = "Días antes")]
        [Range(1, Int16.MaxValue)]
        public Nullable<int> dias_antes_recordatorio { get; set; }


        //temporal para checklist
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_notificaciones_checklist> IT_notificaciones_checklist_temp { get; set; }

        //empleados a notificar
        public IEnumerable<SelectListItem> EmpleadosList { get; set; }
        //correos a notificar
        public IEnumerable<SelectListItem> CorreosList { get; set; }
    }
}