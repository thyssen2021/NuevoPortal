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
    
    public partial class GV_solicitud
    {
        public int id { get; set; }
        public int id_solicitante { get; set; }
        public int id_empleado { get; set; }
        public int id_jefe_directo { get; set; }
        public Nullable<int> id_controlling { get; set; }
        public Nullable<int> id_contabilidad { get; set; }
        public Nullable<int> id_nomina { get; set; }
        public System.DateTime fecha_solicitud { get; set; }
        public string tipo_viaje { get; set; }
        public string origen { get; set; }
        public string destino { get; set; }
        public System.DateTime fecha_salida { get; set; }
        public System.DateTime fecha_regreso { get; set; }
        public Nullable<int> id_medio_transporte { get; set; }
        public string medio_transporte_otro { get; set; }
        public string motivo_viaje { get; set; }
        public bool moneda_extranjera { get; set; }
        public string moneda_extranjera_comentarios { get; set; }
        public Nullable<decimal> moneda_extranjera_monto { get; set; }
        public string iso_moneda_extranjera { get; set; }
        public bool boletos_avion { get; set; }
        public bool hospedaje { get; set; }
        public string reservaciones_comentarios { get; set; }
        public Nullable<System.DateTime> fecha_aceptacion_jefe_area { get; set; }
        public Nullable<System.DateTime> fecha_aceptacion_controlling { get; set; }
        public Nullable<System.DateTime> fecha_aceptacion_contabilidad { get; set; }
        public Nullable<System.DateTime> fecha_aceptacion_nomina { get; set; }
        public string comentario_rechazo { get; set; }
        public string comentario_adicional { get; set; }
        public string estatus { get; set; }
    
        public virtual currency currency { get; set; }
        public virtual empleados empleados { get; set; }
        public virtual empleados empleados1 { get; set; }
        public virtual empleados empleados2 { get; set; }
        public virtual empleados empleados3 { get; set; }
        public virtual empleados empleados4 { get; set; }
        public virtual empleados empleados5 { get; set; }
        public virtual GV_medios_transporte GV_medios_transporte { get; set; }
    }
}
