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
    
    public partial class empleados
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public empleados()
        {
            this.budget_responsables = new HashSet<budget_responsables>();
            this.inspeccion_datos_generales = new HashSet<inspeccion_datos_generales>();
            this.PFA = new HashSet<PFA>();
            this.PFA_Autorizador = new HashSet<PFA_Autorizador>();
            this.PFA1 = new HashSet<PFA>();
            this.PM_departamentos = new HashSet<PM_departamentos>();
            this.PM_usuarios_capturistas = new HashSet<PM_usuarios_capturistas>();
            this.poliza_manual = new HashSet<poliza_manual>();
            this.poliza_manual1 = new HashSet<poliza_manual>();
            this.poliza_manual2 = new HashSet<poliza_manual>();
            this.poliza_manual3 = new HashSet<poliza_manual>();
            this.poliza_manual4 = new HashSet<poliza_manual>();
            this.produccion_operadores = new HashSet<produccion_operadores>();
            this.produccion_respaldo = new HashSet<produccion_respaldo>();
            this.produccion_supervisores = new HashSet<produccion_supervisores>();
            this.upgrade_usuarios = new HashSet<upgrade_usuarios>();
            this.notificaciones_correo = new HashSet<notificaciones_correo>();
            this.IT_matriz_requerimientos = new HashSet<IT_matriz_requerimientos>();
            this.IT_matriz_requerimientos1 = new HashSet<IT_matriz_requerimientos>();
            this.IT_matriz_requerimientos2 = new HashSet<IT_matriz_requerimientos>();
            this.IT_matriz_requerimientos3 = new HashSet<IT_matriz_requerimientos>();
            this.orden_trabajo = new HashSet<orden_trabajo>();
            this.orden_trabajo1 = new HashSet<orden_trabajo>();
            this.orden_trabajo2 = new HashSet<orden_trabajo>();
            this.OT_responsables = new HashSet<OT_responsables>();
            this.OT_responsables1 = new HashSet<OT_responsables>();
            this.IT_solicitud_usuarios = new HashSet<IT_solicitud_usuarios>();
            this.IT_asignacion_hardware = new HashSet<IT_asignacion_hardware>();
            this.IT_asignacion_hardware1 = new HashSet<IT_asignacion_hardware>();
            this.IT_asignacion_hardware2 = new HashSet<IT_asignacion_hardware>();
            this.IT_asignacion_software = new HashSet<IT_asignacion_software>();
            this.IT_asignacion_software1 = new HashSet<IT_asignacion_software>();
            this.GV_solicitud = new HashSet<GV_solicitud>();
            this.GV_solicitud1 = new HashSet<GV_solicitud>();
            this.GV_solicitud2 = new HashSet<GV_solicitud>();
            this.GV_solicitud3 = new HashSet<GV_solicitud>();
            this.GV_solicitud4 = new HashSet<GV_solicitud>();
            this.GV_solicitud5 = new HashSet<GV_solicitud>();
        }
    
        public int id { get; set; }
        public Nullable<System.DateTime> nueva_fecha_nacimiento { get; set; }
        public Nullable<int> planta_clave { get; set; }
        public Nullable<int> clave { get; set; }
        public Nullable<bool> activo { get; set; }
        public string numeroEmpleado { get; set; }
        public string nombre { get; set; }
        public string apellido1 { get; set; }
        public string apellido2 { get; set; }
        public string nacimientoFecha { get; set; }
        public string correo { get; set; }
        public string telefono { get; set; }
        public string extension { get; set; }
        public string celular { get; set; }
        public string nivel { get; set; }
        public Nullable<int> puesto { get; set; }
        public string compania { get; set; }
        public Nullable<System.DateTime> ingresoFecha { get; set; }
        public Nullable<System.DateTime> bajaFecha { get; set; }
        public string C8ID { get; set; }
        public Nullable<int> id_area { get; set; }
    
        public virtual Area Area { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<budget_responsables> budget_responsables { get; set; }
        public virtual plantas plantas { get; set; }
        public virtual puesto puesto1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<inspeccion_datos_generales> inspeccion_datos_generales { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PFA> PFA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PFA_Autorizador> PFA_Autorizador { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PFA> PFA1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PM_departamentos> PM_departamentos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PM_usuarios_capturistas> PM_usuarios_capturistas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<poliza_manual> poliza_manual4 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<produccion_operadores> produccion_operadores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<produccion_respaldo> produccion_respaldo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<produccion_supervisores> produccion_supervisores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<upgrade_usuarios> upgrade_usuarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<notificaciones_correo> notificaciones_correo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_matriz_requerimientos> IT_matriz_requerimientos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_matriz_requerimientos> IT_matriz_requerimientos1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_matriz_requerimientos> IT_matriz_requerimientos2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_matriz_requerimientos> IT_matriz_requerimientos3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orden_trabajo> orden_trabajo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orden_trabajo> orden_trabajo1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orden_trabajo> orden_trabajo2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OT_responsables> OT_responsables { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OT_responsables> OT_responsables1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_solicitud_usuarios> IT_solicitud_usuarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_asignacion_hardware> IT_asignacion_hardware { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_asignacion_hardware> IT_asignacion_hardware1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_asignacion_hardware> IT_asignacion_hardware2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_asignacion_software> IT_asignacion_software { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IT_asignacion_software> IT_asignacion_software1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GV_solicitud> GV_solicitud { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GV_solicitud> GV_solicitud1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GV_solicitud> GV_solicitud2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GV_solicitud> GV_solicitud3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GV_solicitud> GV_solicitud4 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GV_solicitud> GV_solicitud5 { get; set; }
    }
}
