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
    
    public partial class budget_comentarios_rel_anio_cuenta
    {
        public int id { get; set; }
        public int id_anio_fiscal { get; set; }
        public int id_centro_costo { get; set; }
        public int id_cuenta_sap { get; set; }
        public string comentario { get; set; }
    
        public virtual budget_anio_fiscal budget_anio_fiscal { get; set; }
        public virtual budget_centro_costo budget_centro_costo { get; set; }
        public virtual budget_cuenta_sap budget_cuenta_sap { get; set; }
    }
}
