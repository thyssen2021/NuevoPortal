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
    using System.ComponentModel.DataAnnotations;

    public partial class PM_usuarios_capturistas
    {
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Empleado")]
        public int id_empleado { get; set; }

        [Required]
        [Display(Name = "Departamento")]
        public int id_departamento { get; set; }

        [Display(Name = "Estado")]
        public bool activo { get; set; }
    
        public virtual empleados empleados { get; set; }
        public virtual PM_departamentos PM_departamentos { get; set; }
    }
}
