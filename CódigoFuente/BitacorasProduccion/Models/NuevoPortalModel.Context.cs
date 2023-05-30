﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Portal_2_0Entities : DbContext
    {
        public Portal_2_0Entities()
            : base("name=Portal_2_0Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<biblioteca_digital> biblioteca_digital { get; set; }
        public virtual DbSet<bom_en_sap> bom_en_sap { get; set; }
        public virtual DbSet<budget_anio_fiscal> budget_anio_fiscal { get; set; }
        public virtual DbSet<budget_centro_costo> budget_centro_costo { get; set; }
        public virtual DbSet<budget_cuenta_sap> budget_cuenta_sap { get; set; }
        public virtual DbSet<budget_departamentos> budget_departamentos { get; set; }
        public virtual DbSet<budget_mapping> budget_mapping { get; set; }
        public virtual DbSet<budget_mapping_bridge> budget_mapping_bridge { get; set; }
        public virtual DbSet<budget_plantas> budget_plantas { get; set; }
        public virtual DbSet<budget_rel_comentarios> budget_rel_comentarios { get; set; }
        public virtual DbSet<budget_rel_fy_centro> budget_rel_fy_centro { get; set; }
        public virtual DbSet<budget_responsables> budget_responsables { get; set; }
        public virtual DbSet<class_v3> class_v3 { get; set; }
        public virtual DbSet<currency> currency { get; set; }
        public virtual DbSet<GV_medios_transporte> GV_medios_transporte { get; set; }
        public virtual DbSet<GV_rel_gastos_solicitud> GV_rel_gastos_solicitud { get; set; }
        public virtual DbSet<GV_tipo_gastos_viaje> GV_tipo_gastos_viaje { get; set; }
        public virtual DbSet<IATF_documentos> IATF_documentos { get; set; }
        public virtual DbSet<IATF_revisiones> IATF_revisiones { get; set; }
        public virtual DbSet<inspeccion_categoria_fallas> inspeccion_categoria_fallas { get; set; }
        public virtual DbSet<inspeccion_datos_generales> inspeccion_datos_generales { get; set; }
        public virtual DbSet<inspeccion_fallas> inspeccion_fallas { get; set; }
        public virtual DbSet<inspeccion_pieza_descarte_produccion> inspeccion_pieza_descarte_produccion { get; set; }
        public virtual DbSet<IT_asignacion_hardware> IT_asignacion_hardware { get; set; }
        public virtual DbSet<IT_asignacion_hardware_rel_items> IT_asignacion_hardware_rel_items { get; set; }
        public virtual DbSet<IT_asignacion_software> IT_asignacion_software { get; set; }
        public virtual DbSet<IT_carpetas_red> IT_carpetas_red { get; set; }
        public virtual DbSet<IT_comunicaciones_tipo> IT_comunicaciones_tipo { get; set; }
        public virtual DbSet<IT_internet_tipo> IT_internet_tipo { get; set; }
        public virtual DbSet<IT_inventory_cellular_plans> IT_inventory_cellular_plans { get; set; }
        public virtual DbSet<IT_inventory_hard_drives> IT_inventory_hard_drives { get; set; }
        public virtual DbSet<IT_inventory_items_genericos> IT_inventory_items_genericos { get; set; }
        public virtual DbSet<IT_inventory_tipos_accesorios> IT_inventory_tipos_accesorios { get; set; }
        public virtual DbSet<IT_mantenimientos_checklist_categorias> IT_mantenimientos_checklist_categorias { get; set; }
        public virtual DbSet<IT_mantenimientos_checklist_item> IT_mantenimientos_checklist_item { get; set; }
        public virtual DbSet<IT_mantenimientos_rel_checklist> IT_mantenimientos_rel_checklist { get; set; }
        public virtual DbSet<IT_matriz_carpetas> IT_matriz_carpetas { get; set; }
        public virtual DbSet<IT_matriz_comunicaciones> IT_matriz_comunicaciones { get; set; }
        public virtual DbSet<IT_solicitud_usuarios> IT_solicitud_usuarios { get; set; }
        public virtual DbSet<log_acceso_email> log_acceso_email { get; set; }
        public virtual DbSet<log_inicio_sesion> log_inicio_sesion { get; set; }
        public virtual DbSet<mm_v3> mm_v3 { get; set; }
        public virtual DbSet<notificaciones_correo> notificaciones_correo { get; set; }
        public virtual DbSet<orden_trabajo> orden_trabajo { get; set; }
        public virtual DbSet<OT_grupo_trabajo> OT_grupo_trabajo { get; set; }
        public virtual DbSet<OT_refacciones> OT_refacciones { get; set; }
        public virtual DbSet<OT_rel_archivos> OT_rel_archivos { get; set; }
        public virtual DbSet<OT_rel_depto_aplica_linea> OT_rel_depto_aplica_linea { get; set; }
        public virtual DbSet<OT_responsables> OT_responsables { get; set; }
        public virtual DbSet<OT_zona_falla> OT_zona_falla { get; set; }
        public virtual DbSet<PFA> PFA { get; set; }
        public virtual DbSet<PFA_Autorizador> PFA_Autorizador { get; set; }
        public virtual DbSet<PFA_Border_port> PFA_Border_port { get; set; }
        public virtual DbSet<PFA_Department> PFA_Department { get; set; }
        public virtual DbSet<PFA_Destination_plant> PFA_Destination_plant { get; set; }
        public virtual DbSet<PFA_Reason> PFA_Reason { get; set; }
        public virtual DbSet<PFA_Recovered_cost> PFA_Recovered_cost { get; set; }
        public virtual DbSet<PFA_Responsible_cost> PFA_Responsible_cost { get; set; }
        public virtual DbSet<PFA_Type_shipment> PFA_Type_shipment { get; set; }
        public virtual DbSet<PFA_Volume> PFA_Volume { get; set; }
        public virtual DbSet<PM_conceptos> PM_conceptos { get; set; }
        public virtual DbSet<PM_conceptos_modelo> PM_conceptos_modelo { get; set; }
        public virtual DbSet<PM_departamentos> PM_departamentos { get; set; }
        public virtual DbSet<PM_poliza_manual_modelo> PM_poliza_manual_modelo { get; set; }
        public virtual DbSet<PM_tipo_poliza> PM_tipo_poliza { get; set; }
        public virtual DbSet<PM_usuarios_capturistas> PM_usuarios_capturistas { get; set; }
        public virtual DbSet<poliza_manual> poliza_manual { get; set; }
        public virtual DbSet<produccion_datos_entrada> produccion_datos_entrada { get; set; }
        public virtual DbSet<produccion_lineas> produccion_lineas { get; set; }
        public virtual DbSet<produccion_lotes> produccion_lotes { get; set; }
        public virtual DbSet<produccion_operadores> produccion_operadores { get; set; }
        public virtual DbSet<produccion_registros> produccion_registros { get; set; }
        public virtual DbSet<produccion_respaldo> produccion_respaldo { get; set; }
        public virtual DbSet<produccion_supervisores> produccion_supervisores { get; set; }
        public virtual DbSet<produccion_turnos> produccion_turnos { get; set; }
        public virtual DbSet<puesto> puesto { get; set; }
        public virtual DbSet<upgrade_check_item> upgrade_check_item { get; set; }
        public virtual DbSet<upgrade_departamentos> upgrade_departamentos { get; set; }
        public virtual DbSet<upgrade_plantas> upgrade_plantas { get; set; }
        public virtual DbSet<upgrade_revision> upgrade_revision { get; set; }
        public virtual DbSet<upgrade_usuarios> upgrade_usuarios { get; set; }
        public virtual DbSet<upgrade_values_checklist> upgrade_values_checklist { get; set; }
        public virtual DbSet<upgrade_values_transaccion> upgrade_values_transaccion { get; set; }
        public virtual DbSet<view_historico_resultado> view_historico_resultado { get; set; }
        public virtual DbSet<view_valores_fiscal_year> view_valores_fiscal_year { get; set; }
        public virtual DbSet<GV_usuarios> GV_usuarios { get; set; }
        public virtual DbSet<GV_solicitud> GV_solicitud { get; set; }
        public virtual DbSet<IT_site> IT_site { get; set; }
        public virtual DbSet<IT_site_actividades> IT_site_actividades { get; set; }
        public virtual DbSet<IT_site_checklist_rel_actividades> IT_site_checklist_rel_actividades { get; set; }
        public virtual DbSet<IT_site_checklist> IT_site_checklist { get; set; }
        public virtual DbSet<IT_equipos_checklist_actividades> IT_equipos_checklist_actividades { get; set; }
        public virtual DbSet<IT_equipos_checklist_categorias> IT_equipos_checklist_categorias { get; set; }
        public virtual DbSet<IT_equipos_rel_checklist_actividades> IT_equipos_rel_checklist_actividades { get; set; }
        public virtual DbSet<IT_inventory_cellular_line> IT_inventory_cellular_line { get; set; }
        public virtual DbSet<IT_inventory_software> IT_inventory_software { get; set; }
        public virtual DbSet<IT_inventory_hardware_type> IT_inventory_hardware_type { get; set; }
        public virtual DbSet<IT_matriz_hardware> IT_matriz_hardware { get; set; }
        public virtual DbSet<IT_matriz_software> IT_matriz_software { get; set; }
        public virtual DbSet<IT_matriz_requerimientos> IT_matriz_requerimientos { get; set; }
        public virtual DbSet<IT_equipos_checklist> IT_equipos_checklist { get; set; }
        public virtual DbSet<IT_inventory_items> IT_inventory_items { get; set; }
        public virtual DbSet<IT_mantenimientos> IT_mantenimientos { get; set; }
        public virtual DbSet<RH_menu_comedor_platillos> RH_menu_comedor_platillos { get; set; }
        public virtual DbSet<budget_cantidad> budget_cantidad { get; set; }
        public virtual DbSet<view_valores_fiscal_year_budget_historico> view_valores_fiscal_year_budget_historico { get; set; }
        public virtual DbSet<empleados> empleados { get; set; }
        public virtual DbSet<IT_epo> IT_epo { get; set; }
        public virtual DbSet<IT_wsus> IT_wsus { get; set; }
        public virtual DbSet<plantas> plantas { get; set; }
        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<IT_notificaciones_checklist> IT_notificaciones_checklist { get; set; }
        public virtual DbSet<IT_notificaciones_usuarios> IT_notificaciones_usuarios { get; set; }
        public virtual DbSet<IT_notificaciones_recordatorio> IT_notificaciones_recordatorio { get; set; }
        public virtual DbSet<log_envio_correo> log_envio_correo { get; set; }
        public virtual DbSet<IT_notificaciones_actividad> IT_notificaciones_actividad { get; set; }
        public virtual DbSet<clientes> clientes { get; set; }
        public virtual DbSet<RM_almacen> RM_almacen { get; set; }
        public virtual DbSet<RM_cabecera> RM_cabecera { get; set; }
        public virtual DbSet<RM_estatus> RM_estatus { get; set; }
        public virtual DbSet<RM_remision_motivo> RM_remision_motivo { get; set; }
        public virtual DbSet<RM_transporte_proveedor> RM_transporte_proveedor { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<RM_elemento> RM_elemento { get; set; }
        public virtual DbSet<RM_cambio_estatus> RM_cambio_estatus { get; set; }
        public virtual DbSet<RU_usuarios_vigilancia> RU_usuarios_vigilancia { get; set; }
        public virtual DbSet<RU_usuarios_embarques> RU_usuarios_embarques { get; set; }
        public virtual DbSet<budget_cantidad_budget_historico> budget_cantidad_budget_historico { get; set; }
        public virtual DbSet<menu_item> menu_item { get; set; }
        public virtual DbSet<menu_link> menu_link { get; set; }
        public virtual DbSet<IT_matriz_asignaciones> IT_matriz_asignaciones { get; set; }
        public virtual DbSet<RU_registros> RU_registros { get; set; }
        public virtual DbSet<IT_mantenimientos_aplazamientos> IT_mantenimientos_aplazamientos { get; set; }
    }
}
