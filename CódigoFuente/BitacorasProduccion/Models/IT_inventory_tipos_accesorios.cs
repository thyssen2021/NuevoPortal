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
    
    public partial class IT_inventory_tipos_accesorios
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IT_inventory_tipos_accesorios()
        {
            this.IT_inventory_items_genericos = new HashSet<IT_inventory_items_genericos>();
            this.IT_inventory_items = new HashSet<IT_inventory_items>();
        }
    
        public int id { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
        public bool es_accesorio { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_inventory_items_genericos> IT_inventory_items_genericos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_inventory_items> IT_inventory_items { get; set; }
    }
}
