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
    
    public partial class PM_usuarios_capturistas
    {
        public int id { get; set; }
        public int id_empleado { get; set; }
        public int id_departamento { get; set; }
        public bool activo { get; set; }
    
        public virtual PM_departamentos PM_departamentos { get; set; }
        public virtual empleados empleados { get; set; }
    }
}
