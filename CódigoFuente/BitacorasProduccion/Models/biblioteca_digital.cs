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
    
    public partial class biblioteca_digital
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public biblioteca_digital()
        {
            this.IT_asignacion_hardware = new HashSet<IT_asignacion_hardware>();
            this.IT_mantenimientos = new HashSet<IT_mantenimientos>();
            this.OT_rel_archivos = new HashSet<OT_rel_archivos>();
            this.PFA = new HashSet<PFA>();
            this.poliza_manual = new HashSet<poliza_manual>();
            this.poliza_manual1 = new HashSet<poliza_manual>();
            this.GV_comprobacion = new HashSet<GV_comprobacion>();
            this.GV_comprobacion_rel_gastos = new HashSet<GV_comprobacion_rel_gastos>();
            this.GV_comprobacion_rel_gastos1 = new HashSet<GV_comprobacion_rel_gastos>();
            this.GV_comprobacion_rel_gastos2 = new HashSet<GV_comprobacion_rel_gastos>();
        }
    
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string MimeType { get; set; }
        public byte[] Datos { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_asignacion_hardware> IT_asignacion_hardware { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_mantenimientos> IT_mantenimientos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OT_rel_archivos> OT_rel_archivos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PFA> PFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GV_comprobacion> GV_comprobacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GV_comprobacion_rel_gastos> GV_comprobacion_rel_gastos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GV_comprobacion_rel_gastos> GV_comprobacion_rel_gastos1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GV_comprobacion_rel_gastos> GV_comprobacion_rel_gastos2 { get; set; }
    }
}
