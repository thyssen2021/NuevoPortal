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
    
    public partial class BG_IHS_rel_cuartos
    {
        public int id { get; set; }
        public int id_ihs_item { get; set; }
        public int cuarto { get; set; }
        public int anio { get; set; }
        public Nullable<int> cantidad { get; set; }
        public Nullable<System.DateTime> fecha_carga { get; set; }
    
        public virtual BG_IHS_item BG_IHS_item { get; set; }
    }
}
