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
    
    public partial class Area
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Area()
        {
            this.orden_trabajo = new HashSet<orden_trabajo>();
            this.OT_rel_depto_aplica_linea = new HashSet<OT_rel_depto_aplica_linea>();
            this.puesto = new HashSet<puesto>();
            this.empleados = new HashSet<empleados>();
        }
    
        public int clave { get; set; }
        public bool activo { get; set; }
        public string descripcion { get; set; }
        public string listaCorreoElectronico { get; set; }
        public Nullable<int> plantaClave { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orden_trabajo> orden_trabajo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OT_rel_depto_aplica_linea> OT_rel_depto_aplica_linea { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<puesto> puesto { get; set; }
        public virtual plantas plantas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<empleados> empleados { get; set; }
    }
}
