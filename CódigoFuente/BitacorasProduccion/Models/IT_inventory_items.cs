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
    
    public partial class IT_inventory_items
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IT_inventory_items()
        {
            this.IT_asignacion_hardware_rel_items = new HashSet<IT_asignacion_hardware_rel_items>();
            this.IT_inventory_hard_drives = new HashSet<IT_inventory_hard_drives>();
            this.IT_mantenimientos = new HashSet<IT_mantenimientos>();
            this.IT_inventory_items1 = new HashSet<IT_inventory_items>();
            this.IT_equipos_checklist = new HashSet<IT_equipos_checklist>();
        }
    
        public int id { get; set; }
        public int id_planta { get; set; }
        public int id_inventory_type { get; set; }
        public bool active { get; set; }
        public Nullable<System.DateTime> purchase_date { get; set; }
        public Nullable<System.DateTime> inactive_date { get; set; }
        public string comments { get; set; }
        public string hostname { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string serial_number { get; set; }
        public Nullable<System.DateTime> end_warranty { get; set; }
        public string mac_lan { get; set; }
        public string mac_wlan { get; set; }
        public string processor { get; set; }
        public Nullable<decimal> total_physical_memory_mb { get; set; }
        public Nullable<int> maintenance_period_months { get; set; }
        public Nullable<int> cpu_speed_mhz { get; set; }
        public string operation_system { get; set; }
        public Nullable<int> bits_operation_system { get; set; }
        public Nullable<int> number_of_cpus { get; set; }
        public string printer_ubication { get; set; }
        public string ip_adress { get; set; }
        public string cost_center { get; set; }
        public Nullable<int> movil_device_storage_mb { get; set; }
        public Nullable<decimal> inches { get; set; }
        public string imei_1 { get; set; }
        public string imei_2 { get; set; }
        public string code { get; set; }
        public string accessories { get; set; }
        public Nullable<int> physical_server { get; set; }
        public Nullable<int> id_tipo_accesorio { get; set; }
        public bool baja { get; set; }
        public Nullable<System.DateTime> fecha_baja { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_asignacion_hardware_rel_items> IT_asignacion_hardware_rel_items { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_inventory_hard_drives> IT_inventory_hard_drives { get; set; }
        public virtual plantas plantas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_mantenimientos> IT_mantenimientos { get; set; }
        public virtual IT_inventory_tipos_accesorios IT_inventory_tipos_accesorios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_inventory_items> IT_inventory_items1 { get; set; }
        public virtual IT_inventory_items IT_inventory_items2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_equipos_checklist> IT_equipos_checklist { get; set; }
        public virtual IT_inventory_hardware_type IT_inventory_hardware_type { get; set; }
    }
}
