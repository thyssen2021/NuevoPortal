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
    
    public partial class IATF_revisiones
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IATF_revisiones()
        {
            this.IT_asignacion_hardware = new HashSet<IT_asignacion_hardware>();
            this.IT_mantenimientos = new HashSet<IT_mantenimientos>();
        }
    
        public int id { get; set; }
        public int id_iatf_documento { get; set; }
        public int numero_revision { get; set; }
        public string responsable { get; set; }
        public string puesto_responsable { get; set; }
        public System.DateTime fecha_revision { get; set; }
        public string descripcion { get; set; }
    
        public virtual IATF_documentos IATF_documentos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_asignacion_hardware> IT_asignacion_hardware { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_mantenimientos> IT_mantenimientos { get; set; }
    }
}
