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
    
    public partial class IT_mantenimientos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IT_mantenimientos()
        {
            this.IT_mantenimientos_rel_checklist = new HashSet<IT_mantenimientos_rel_checklist>();
            this.IT_mantenimientos_aplazamientos = new HashSet<IT_mantenimientos_aplazamientos>();
        }
    
        public int id { get; set; }
        public int id_it_inventory_item { get; set; }
        public Nullable<int> id_empleado_responsable { get; set; }
        public Nullable<int> id_empleado_sistemas { get; set; }
        public Nullable<int> id_iatf_version { get; set; }
        public Nullable<int> id_biblioteca_digital { get; set; }
        public System.DateTime fecha_programada { get; set; }
        public Nullable<System.DateTime> fecha_realizacion { get; set; }
        public string comentarios { get; set; }
    
        public virtual biblioteca_digital biblioteca_digital { get; set; }
        public virtual IATF_revisiones IATF_revisiones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_mantenimientos_rel_checklist> IT_mantenimientos_rel_checklist { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_mantenimientos_aplazamientos> IT_mantenimientos_aplazamientos { get; set; }
        public virtual empleados empleados { get; set; }
        public virtual empleados empleados1 { get; set; }
        public virtual IT_inventory_items IT_inventory_items { get; set; }
    }
}
