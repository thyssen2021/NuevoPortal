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
    
    public partial class budget_valores
    {
        public int id { get; set; }
        public int id_rel_anio_centro { get; set; }
        public int id_cuenta_sap { get; set; }
        public int mes { get; set; }
        public string currency_iso { get; set; }
        public decimal cantidad { get; set; }
    
        public virtual budget_cuenta_sap budget_cuenta_sap { get; set; }
        public virtual budget_rel_anio_fiscal_centro budget_rel_anio_fiscal_centro { get; set; }
        public virtual currency currency { get; set; }
    }
}
