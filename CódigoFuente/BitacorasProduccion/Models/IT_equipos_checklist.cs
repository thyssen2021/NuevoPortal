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
    
    public partial class IT_equipos_checklist
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IT_equipos_checklist()
        {
            this.IT_equipos_rel_checklist_actividades = new HashSet<IT_equipos_rel_checklist_actividades>();
        }
    
        public int id { get; set; }
        public int id_sistemas { get; set; }
        public string nombre { get; set; }
        public string numero_serie { get; set; }
        public string modelo { get; set; }
        public string sistema_operativo { get; set; }
        public string comentario_general { get; set; }
        public System.DateTime fecha { get; set; }
        public string estatus { get; set; }
    
        public virtual empleados empleados { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_equipos_rel_checklist_actividades> IT_equipos_rel_checklist_actividades { get; set; }
    }
}
