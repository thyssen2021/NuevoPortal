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
    
    public partial class poliza_manual
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public poliza_manual()
        {
            this.PM_conceptos = new HashSet<PM_conceptos>();
        }
    
        public int id { get; set; }
        public int id_PM_tipo_poliza { get; set; }
        public string currency_iso { get; set; }
        public int id_planta { get; set; }
        public int id_elaborador { get; set; }
        public Nullable<int> id_validador { get; set; }
        public Nullable<int> id_autorizador { get; set; }
        public Nullable<int> id_contabilidad { get; set; }
        public Nullable<int> id_direccion { get; set; }
        public Nullable<int> id_documento_soporte { get; set; }
        public Nullable<int> id_documento_registro { get; set; }
        public string numero_documento_sap { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public System.DateTime fecha_documento { get; set; }
        public Nullable<System.DateTime> fecha_validacion { get; set; }
        public Nullable<System.DateTime> fecha_autorizacion { get; set; }
        public Nullable<System.DateTime> fecha_direccion { get; set; }
        public Nullable<System.DateTime> fecha_registro { get; set; }
        public string comentario_rechazo { get; set; }
        public string descripcion_poliza { get; set; }
        public string estatus { get; set; }
    
        public virtual biblioteca_digital biblioteca_digital { get; set; }
        public virtual biblioteca_digital biblioteca_digital1 { get; set; }
        public virtual currency currency { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PM_conceptos> PM_conceptos { get; set; }
        public virtual PM_tipo_poliza PM_tipo_poliza { get; set; }
        public virtual empleados empleados { get; set; }
        public virtual empleados empleados1 { get; set; }
        public virtual empleados empleados2 { get; set; }
        public virtual empleados empleados3 { get; set; }
        public virtual empleados empleados4 { get; set; }
        public virtual plantas plantas { get; set; }
    }
}
