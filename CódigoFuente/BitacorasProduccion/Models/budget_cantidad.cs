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
    
    public partial class budget_cantidad
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public budget_cantidad()
        {
            this.budget_rel_conceptos_cantidades = new HashSet<budget_rel_conceptos_cantidades>();
        }
    
        public int id { get; set; }
        public int id_budget_rel_fy_centro { get; set; }
        public int id_cuenta_sap { get; set; }
        public int mes { get; set; }
        public string currency_iso { get; set; }
        public decimal cantidad { get; set; }
        public string comentario { get; set; }
        public bool moneda_local_usd { get; set; }
    
        public virtual currency currency { get; set; }
        public virtual budget_cuenta_sap budget_cuenta_sap { get; set; }
        public virtual budget_rel_fy_centro budget_rel_fy_centro { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_rel_conceptos_cantidades> budget_rel_conceptos_cantidades { get; set; }
    }
}
