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
    
    public partial class budget_cuenta_sap
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public budget_cuenta_sap()
        {
            this.budget_cantidad = new HashSet<budget_cantidad>();
            this.budget_rel_comentarios = new HashSet<budget_rel_comentarios>();
            this.budget_rel_conceptos_formulas = new HashSet<budget_rel_conceptos_formulas>();
            this.budget_rel_documento = new HashSet<budget_rel_documento>();
        }
    
        public int id { get; set; }
        public int id_mapping { get; set; }
        public string sap_account { get; set; }
        public string name { get; set; }
        public bool activo { get; set; }
        public Nullable<bool> aplica_formula { get; set; }
        public string formula { get; set; }
        public bool aplica_mxn { get; set; }
        public bool aplica_usd { get; set; }
        public bool aplica_eur { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_cantidad> budget_cantidad { get; set; }
        public virtual budget_mapping budget_mapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_rel_comentarios> budget_rel_comentarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_rel_conceptos_formulas> budget_rel_conceptos_formulas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_rel_documento> budget_rel_documento { get; set; }
    }
}
