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
    
    public partial class IT_matriz_carpetas
    {
        public int id { get; set; }
        public int id_matriz_requerimientos { get; set; }
        public int id_it_carpeta_red { get; set; }
        public string descripcion { get; set; }
    
        public virtual IT_carpetas_red IT_carpetas_red { get; set; }
        public virtual IT_matriz_requerimientos IT_matriz_requerimientos { get; set; }
    }
}
