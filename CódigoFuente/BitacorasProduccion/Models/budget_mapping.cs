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
    
    public partial class budget_mapping
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public budget_mapping()
        {
            this.budget_cuenta_sap = new HashSet<budget_cuenta_sap>();
        }
    
        public int id { get; set; }
        public int id_mapping_bridge { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
    
        public virtual budget_mapping_bridge budget_mapping_bridge { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_cuenta_sap> budget_cuenta_sap { get; set; }
    }
}
