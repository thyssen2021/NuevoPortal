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
    
    public partial class SCDM_solicitud_historial
    {
        public int id { get; set; }
        public int id_solicitud { get; set; }
        public int id_empleado { get; set; }
        public System.DateTime fecha { get; set; }
        public string descripcion { get; set; }
    
        public virtual empleados empleados { get; set; }
        public virtual SCDM_solicitud SCDM_solicitud { get; set; }
    }
}
