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
    
    public partial class SCDM_rel_solicitud_plantas
    {
        public int id { get; set; }
        public int id_solicitud { get; set; }
        public int id_planta { get; set; }
    
        public virtual SCDM_solicitud SCDM_solicitud { get; set; }
        public virtual plantas plantas { get; set; }
    }
}
