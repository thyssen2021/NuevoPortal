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
    
    public partial class currency
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public currency()
        {
            this.budget_cantidad = new HashSet<budget_cantidad>();
            this.GV_rel_gastos_solicitud = new HashSet<GV_rel_gastos_solicitud>();
            this.poliza_manual = new HashSet<poliza_manual>();
            this.GV_comprobacion_rel_gastos = new HashSet<GV_comprobacion_rel_gastos>();
            this.GV_solicitud = new HashSet<GV_solicitud>();
        }
    
        public string CurrencyISO { get; set; }
        public string CurrencyName { get; set; }
        public string Money { get; set; }
        public string Symbol { get; set; }
        public Nullable<decimal> LimitePoliza { get; set; }
        public bool activo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_cantidad> budget_cantidad { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GV_rel_gastos_solicitud> GV_rel_gastos_solicitud { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GV_comprobacion_rel_gastos> GV_comprobacion_rel_gastos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GV_solicitud> GV_solicitud { get; set; }
    }
}
