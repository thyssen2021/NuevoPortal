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
    
    public partial class OT_responsables
    {
        public int id { get; set; }
        public int id_empleado { get; set; }
        public bool supervisor { get; set; }
        public Nullable<int> id_supervisor { get; set; }
        public bool activo { get; set; }
    
        public virtual empleados empleados { get; set; }
        public virtual empleados empleados1 { get; set; }
    }
}
