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
    
    public partial class IT_inventory_hardware_type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IT_inventory_hardware_type()
        {
            this.IT_inventory_items = new HashSet<IT_inventory_items>();
            this.IT_inventory_items_genericos = new HashSet<IT_inventory_items_genericos>();
            this.IT_matriz_hardware = new HashSet<IT_matriz_hardware>();
        }
    
        public int id { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
        public bool puede_asignarse { get; set; }
        public bool disponible_en_matriz_rh { get; set; }
        public bool aplica_descripcion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_inventory_items> IT_inventory_items { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_inventory_items_genericos> IT_inventory_items_genericos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_matriz_hardware> IT_matriz_hardware { get; set; }
    }
}
