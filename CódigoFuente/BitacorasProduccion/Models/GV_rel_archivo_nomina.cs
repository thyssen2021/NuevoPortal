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
    
    public partial class GV_rel_archivo_nomina
    {
        public int id { get; set; }
        public int id_gv_solicitud { get; set; }
        public int id_biblioteca_digital { get; set; }
        public double cantidad { get; set; }
        public Nullable<int> id_soporte_sap { get; set; }
    
        public virtual biblioteca_digital biblioteca_digital { get; set; }
        public virtual biblioteca_digital biblioteca_digital1 { get; set; }
        public virtual GV_solicitud GV_solicitud { get; set; }
    }
}
