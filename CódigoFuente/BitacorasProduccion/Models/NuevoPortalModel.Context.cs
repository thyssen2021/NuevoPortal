﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
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
    
        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<bom_en_sap> bom_en_sap { get; set; }
        public virtual DbSet<class_v3> class_v3 { get; set; }
        public virtual DbSet<mm_v3> mm_v3 { get; set; }
        public virtual DbSet<plantas> plantas { get; set; }
        public virtual DbSet<produccion_datos_entrada> produccion_datos_entrada { get; set; }
        public virtual DbSet<produccion_lineas> produccion_lineas { get; set; }
        public virtual DbSet<produccion_lotes> produccion_lotes { get; set; }
        public virtual DbSet<produccion_operadores> produccion_operadores { get; set; }
        public virtual DbSet<produccion_registros> produccion_registros { get; set; }
        public virtual DbSet<produccion_supervisores> produccion_supervisores { get; set; }
        public virtual DbSet<produccion_turnos> produccion_turnos { get; set; }
        public virtual DbSet<puesto> puesto { get; set; }
        public virtual DbSet<notificaciones_correo> notificaciones_correo { get; set; }
        public virtual DbSet<PFA_Border_port> PFA_Border_port { get; set; }
        public virtual DbSet<PFA_Department> PFA_Department { get; set; }
        public virtual DbSet<PFA_Destination_plant> PFA_Destination_plant { get; set; }
        public virtual DbSet<PFA_Reason> PFA_Reason { get; set; }
        public virtual DbSet<PFA_Recovered_cost> PFA_Recovered_cost { get; set; }
        public virtual DbSet<PFA_Responsible_cost> PFA_Responsible_cost { get; set; }
        public virtual DbSet<PFA_Type_shipment> PFA_Type_shipment { get; set; }
        public virtual DbSet<PFA_Volume> PFA_Volume { get; set; }
        public virtual DbSet<PFA_Autorizador> PFA_Autorizador { get; set; }
        public virtual DbSet<view_historico_resultado> view_historico_resultado { get; set; }
        public virtual DbSet<produccion_respaldo> produccion_respaldo { get; set; }
        public virtual DbSet<biblioteca_digital> biblioteca_digital { get; set; }
        public virtual DbSet<PFA> PFA { get; set; }
        public virtual DbSet<currency> currency { get; set; }
        public virtual DbSet<PM_tipo_poliza> PM_tipo_poliza { get; set; }
        public virtual DbSet<PM_validadores> PM_validadores { get; set; }
        public virtual DbSet<PM_conceptos> PM_conceptos { get; set; }
        public virtual DbSet<empleados> empleados { get; set; }
        public virtual DbSet<PM_autorizadores> PM_autorizadores { get; set; }
        public virtual DbSet<inspeccion_categoria_fallas> inspeccion_categoria_fallas { get; set; }
        public virtual DbSet<inspeccion_datos_generales> inspeccion_datos_generales { get; set; }
        public virtual DbSet<inspeccion_fallas> inspeccion_fallas { get; set; }
        public virtual DbSet<inspeccion_pieza_descarte_produccion> inspeccion_pieza_descarte_produccion { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<log_inicio_sesion> log_inicio_sesion { get; set; }
        public virtual DbSet<PM_conceptos_modelo> PM_conceptos_modelo { get; set; }
        public virtual DbSet<PM_departamentos> PM_departamentos { get; set; }
        public virtual DbSet<PM_poliza_manual_modelo> PM_poliza_manual_modelo { get; set; }
        public virtual DbSet<PM_usuarios_capturistas> PM_usuarios_capturistas { get; set; }
        public virtual DbSet<poliza_manual> poliza_manual { get; set; }
    }
}
