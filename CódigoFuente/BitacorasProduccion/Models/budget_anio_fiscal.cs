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
    
    public partial class budget_anio_fiscal
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public budget_anio_fiscal()
        {
            this.budget_rel_fy_centro = new HashSet<budget_rel_fy_centro>();
        }
    
        public int id { get; set; }
        public string descripcion { get; set; }
        public int anio_inicio { get; set; }
        public int mes_inicio { get; set; }
        public int anio_fin { get; set; }
        public int mes_fin { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_rel_fy_centro> budget_rel_fy_centro { get; set; }
    }
}
