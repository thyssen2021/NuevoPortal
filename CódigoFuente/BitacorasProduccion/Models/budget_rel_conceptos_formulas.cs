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
    
    public partial class budget_rel_conceptos_formulas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public budget_rel_conceptos_formulas()
        {
            this.budget_rel_conceptos_cantidades = new HashSet<budget_rel_conceptos_cantidades>();
        }
    
        public int id { get; set; }
        public int id_budget_cuenta_sap { get; set; }
        public string clave { get; set; }
        public string descripcion { get; set; }
        public Nullable<double> valor_defecto_mxn { get; set; }
        public Nullable<double> valor_defecto_usd { get; set; }
        public Nullable<double> valor_defecto_eur { get; set; }
        public Nullable<bool> valor_fijo { get; set; }
        public bool aplica_comentario { get; set; }
    
        public virtual budget_cuenta_sap budget_cuenta_sap { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_rel_conceptos_cantidades> budget_rel_conceptos_cantidades { get; set; }
    }
}
