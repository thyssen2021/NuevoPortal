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
    
    public partial class PM_conceptos
    {
        public int id { get; set; }
        public int id_poliza { get; set; }
        public string cuenta { get; set; }
        public string cc { get; set; }
        public string concepto { get; set; }
        public string poliza { get; set; }
        public Nullable<decimal> debe { get; set; }
        public Nullable<decimal> haber { get; set; }
    
        public virtual poliza_manual poliza_manual { get; set; }
    }
}
