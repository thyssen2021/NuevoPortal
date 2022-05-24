//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Portal_2_0.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class orden_trabajo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public orden_trabajo()
        {
            this.OT_refacciones = new HashSet<OT_refacciones>();
            this.OT_rel_archivos = new HashSet<OT_rel_archivos>();
        }
    
        public int id { get; set; }
        public int id_solicitante { get; set; }
        public Nullable<int> id_asignacion { get; set; }
        public Nullable<int> id_responsable { get; set; }
        public int id_area { get; set; }
        public Nullable<int> id_linea { get; set; }
        public Nullable<int> id_grupo_trabajo { get; set; }
        public System.DateTime fecha_solicitud { get; set; }
        public string nivel_urgencia { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public bool tpm { get; set; }
        public string numero_tarjeta { get; set; }
        public Nullable<System.DateTime> fecha_asignacion { get; set; }
        public Nullable<System.DateTime> fecha_en_proceso { get; set; }
        public Nullable<System.DateTime> fecha_cierre { get; set; }
        public string estatus { get; set; }
        public string comentario { get; set; }
    
        public virtual Area Area { get; set; }
        public virtual empleados empleados { get; set; }
        public virtual empleados empleados1 { get; set; }
        public virtual empleados empleados2 { get; set; }
        public virtual OT_grupo_trabajo OT_grupo_trabajo { get; set; }
        public virtual produccion_lineas produccion_lineas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OT_refacciones> OT_refacciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OT_rel_archivos> OT_rel_archivos { get; set; }
    }
}
