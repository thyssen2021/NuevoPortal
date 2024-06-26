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
    
    public partial class plantas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public plantas()
        {
            this.Area = new HashSet<Area>();
            this.empleados = new HashSet<empleados>();
            this.IATF_documentos = new HashSet<IATF_documentos>();
            this.IT_inventory_cellular_line = new HashSet<IT_inventory_cellular_line>();
            this.IT_inventory_items = new HashSet<IT_inventory_items>();
            this.IT_site = new HashSet<IT_site>();
            this.IT_solicitud_usuarios = new HashSet<IT_solicitud_usuarios>();
            this.notificaciones_correo = new HashSet<notificaciones_correo>();
            this.poliza_manual = new HashSet<poliza_manual>();
            this.produccion_lineas = new HashSet<produccion_lineas>();
            this.produccion_registros = new HashSet<produccion_registros>();
            this.produccion_supervisores = new HashSet<produccion_supervisores>();
            this.produccion_turnos = new HashSet<produccion_turnos>();
            this.RM_almacen = new HashSet<RM_almacen>();
            this.SCDM_cat_usuarios_revision_departamento = new HashSet<SCDM_cat_usuarios_revision_departamento>();
            this.SCDM_rel_solicitud_plantas = new HashSet<SCDM_rel_solicitud_plantas>();
            this.SCDM_solicitud_rel_creacion_referencia = new HashSet<SCDM_solicitud_rel_creacion_referencia>();
            this.SCDM_solicitud_rel_cambio_ingenieria = new HashSet<SCDM_solicitud_rel_cambio_ingenieria>();
            this.SCDM_solicitud = new HashSet<SCDM_solicitud>();
            this.SCDM_solicitud1 = new HashSet<SCDM_solicitud>();
        }
    
        public int clave { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
        public string codigoSap { get; set; }
        public string tkorgstreet { get; set; }
        public string tkorgpostalcode { get; set; }
        public string tkorgpostaladdress { get; set; }
        public string tkorgaddonaddr { get; set; }
        public string tkorgfedst { get; set; }
        public string tkorgcountry { get; set; }
        public string tkorgcountrykey { get; set; }
        public string tkapsite { get; set; }
        public Nullable<bool> aplica_solicitud_scdm { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Area> Area { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<empleados> empleados { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IATF_documentos> IATF_documentos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_inventory_cellular_line> IT_inventory_cellular_line { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_inventory_items> IT_inventory_items { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_site> IT_site { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_solicitud_usuarios> IT_solicitud_usuarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<notificaciones_correo> notificaciones_correo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<produccion_lineas> produccion_lineas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<produccion_registros> produccion_registros { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<produccion_supervisores> produccion_supervisores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<produccion_turnos> produccion_turnos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RM_almacen> RM_almacen { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCDM_cat_usuarios_revision_departamento> SCDM_cat_usuarios_revision_departamento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCDM_rel_solicitud_plantas> SCDM_rel_solicitud_plantas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCDM_solicitud_rel_creacion_referencia> SCDM_solicitud_rel_creacion_referencia { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCDM_solicitud_rel_cambio_ingenieria> SCDM_solicitud_rel_cambio_ingenieria { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCDM_solicitud> SCDM_solicitud { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCDM_solicitud> SCDM_solicitud1 { get; set; }
    }
}
