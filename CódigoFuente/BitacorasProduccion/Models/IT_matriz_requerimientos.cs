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
    
    public partial class IT_matriz_requerimientos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IT_matriz_requerimientos()
        {
            this.IT_matriz_asignaciones = new HashSet<IT_matriz_asignaciones>();
            this.IT_matriz_carpetas = new HashSet<IT_matriz_carpetas>();
            this.IT_matriz_comunicaciones = new HashSet<IT_matriz_comunicaciones>();
            this.IT_matriz_hardware = new HashSet<IT_matriz_hardware>();
            this.IT_matriz_software = new HashSet<IT_matriz_software>();
        }
    
        public int id { get; set; }
        public int id_empleado { get; set; }
        public int id_solicitante { get; set; }
        public int id_jefe_directo { get; set; }
        public Nullable<int> id_sistemas { get; set; }
        public int id_internet_tipo { get; set; }
        public System.DateTime fecha_solicitud { get; set; }
        public Nullable<System.DateTime> fecha_aprobacion_jefe { get; set; }
        public Nullable<System.DateTime> fecha_cierre { get; set; }
        public string estatus { get; set; }
        public string comentario { get; set; }
        public string comentario_rechazo { get; set; }
        public string comentario_cierre { get; set; }
        public string tipo { get; set; }
    
        public virtual empleados empleados { get; set; }
        public virtual empleados empleados1 { get; set; }
        public virtual empleados empleados2 { get; set; }
        public virtual empleados empleados3 { get; set; }
        public virtual IT_internet_tipo IT_internet_tipo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_matriz_asignaciones> IT_matriz_asignaciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_matriz_carpetas> IT_matriz_carpetas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_matriz_comunicaciones> IT_matriz_comunicaciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_matriz_hardware> IT_matriz_hardware { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_matriz_software> IT_matriz_software { get; set; }
    }
}
