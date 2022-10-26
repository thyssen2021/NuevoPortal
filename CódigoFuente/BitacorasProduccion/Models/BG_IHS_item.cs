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
    
    public partial class BG_IHS_item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BG_IHS_item()
        {
            this.BG_IHS_rel_demanda = new HashSet<BG_IHS_rel_demanda>();
            this.BG_IHS_rel_cuartos = new HashSet<BG_IHS_rel_cuartos>();
            this.BG_IHS_division = new HashSet<BG_IHS_division>();
            this.BG_IHS_rel_combinacion = new HashSet<BG_IHS_rel_combinacion>();
        }
    
        public int id { get; set; }
        public string core_nameplate_region_mnemonic { get; set; }
        public string core_nameplate_plant_mnemonic { get; set; }
        public string mnemonic_vehicle { get; set; }
        public string mnemonic_vehicle_plant { get; set; }
        public string mnemonic_platform { get; set; }
        public string mnemonic_plant { get; set; }
        public string region { get; set; }
        public string market { get; set; }
        public string country_territory { get; set; }
        public string production_plant { get; set; }
        public string city { get; set; }
        public string plant_state_province { get; set; }
        public string source_plant { get; set; }
        public string source_plant_country_territory { get; set; }
        public string source_plant_region { get; set; }
        public string design_parent { get; set; }
        public string engineering_group { get; set; }
        public string manufacturer_group { get; set; }
        public string manufacturer { get; set; }
        public string sales_parent { get; set; }
        public string production_brand { get; set; }
        public string platform_design_owner { get; set; }
        public string architecture { get; set; }
        public string platform { get; set; }
        public string program { get; set; }
        public string production_nameplate { get; set; }
        public Nullable<System.DateTime> sop_start_of_production { get; set; }
        public Nullable<System.DateTime> eop_end_of_production { get; set; }
        public Nullable<int> lifecycle_time { get; set; }
        public string vehicle { get; set; }
        public string assembly_type { get; set; }
        public string strategic_group { get; set; }
        public string sales_group { get; set; }
        public string global_nameplate { get; set; }
        public string primary_design_center { get; set; }
        public string primary_design_country_territory { get; set; }
        public string primary_design_region { get; set; }
        public string secondary_design_center { get; set; }
        public string secondary_design_country_territory { get; set; }
        public string secondary_design_region { get; set; }
        public string gvw_rating { get; set; }
        public string gvw_class { get; set; }
        public string car_truck { get; set; }
        public string production_type { get; set; }
        public string global_production_segment { get; set; }
        public string regional_sales_segment { get; set; }
        public string global_production_price_class { get; set; }
        public string global_sales_segment { get; set; }
        public string global_sales_sub_segment { get; set; }
        public Nullable<int> global_sales_price_class { get; set; }
        public Nullable<int> short_term_risk_rating { get; set; }
        public Nullable<int> long_term_risk_rating { get; set; }
        public string origen { get; set; }
        public decimal porcentaje_scrap { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BG_IHS_rel_demanda> BG_IHS_rel_demanda { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BG_IHS_rel_cuartos> BG_IHS_rel_cuartos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BG_IHS_division> BG_IHS_division { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BG_IHS_rel_combinacion> BG_IHS_rel_combinacion { get; set; }
    }
}
