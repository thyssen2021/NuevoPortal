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
    
    public partial class currency
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public currency()
        {
            this.poliza_manual = new HashSet<poliza_manual>();
            this.budget_valores = new HashSet<budget_valores>();
        }
    
        public string CurrencyISO { get; set; }
        public string CurrencyName { get; set; }
        public string Money { get; set; }
        public string Symbol { get; set; }
        public Nullable<decimal> LimitePoliza { get; set; }
        public bool activo { get; set; }

        //Concatena clave y nombre
        public string CocatCurrency
        {
            get
            {
                return "(" + CurrencyISO + ") " + CurrencyName;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_valores> budget_valores { get; set; }
    }
}
