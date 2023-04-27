//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Portal_2_0.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class IT_notificaciones_recordatorio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IT_notificaciones_recordatorio()
        {
            this.IT_notificaciones_checklist = new HashSet<IT_notificaciones_checklist>();
            this.log_envio_correo = new HashSet<log_envio_correo>();
        }
    
        public int id { get; set; }
        public int id_notificaciones_actividad { get; set; }
        public System.DateTime fecha_programada { get; set; }
        public Nullable<int> id_sistemas { get; set; }
        public Nullable<int> dias_previos_notificacion { get; set; }
        public bool notificacion_previa_enviada { get; set; }
        public bool notificacion_dia_evento_enviada { get; set; }
        public string comentario_cierre { get; set; }
        public string estatus { get; set; }
    
        public virtual empleados empleados { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_notificaciones_checklist> IT_notificaciones_checklist { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<log_envio_correo> log_envio_correo { get; set; }
        public virtual IT_notificaciones_actividad IT_notificaciones_actividad { get; set; }
    }
}
