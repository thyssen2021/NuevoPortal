//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class PFA
    {
        public int id { get; set; }
        public int id_solicitante { get; set; }
        public int id_PFA_Department { get; set; }
        public int id_PFA_volume { get; set; }
        public int id_PFA_border_port { get; set; }
        public int id_PFA_destination_plant { get; set; }
        public int id_PFA_reason { get; set; }
        public int id_PFA_type_shipment { get; set; }
        public int id_PFA_responsible_cost { get; set; }
        public int id_PFA_recovered_cost { get; set; }
        public int id_PFA_autorizador { get; set; }
        public System.DateTime date_request { get; set; }
        public string sap_part_number { get; set; }
        public string customer_part_number { get; set; }
        public int volume { get; set; }
        public string mill { get; set; }
        public string customer { get; set; }
        public decimal total_cost { get; set; }
        public decimal total_pf_cost { get; set; }
        public System.DateTime promise_recovering_date { get; set; }
        public string comentarios { get; set; }
        public string razon_rechazo { get; set; }
        public string estatus { get; set; }
        public Nullable<System.DateTime> fecha_aprobacion { get; set; }
        public bool activo { get; set; }
    
        public virtual empleados empleados { get; set; }
        public virtual empleados empleados1 { get; set; }
        public virtual PFA_Recovered_cost PFA_Recovered_cost { get; set; }
        public virtual PFA_Border_port PFA_Border_port { get; set; }
        public virtual PFA_Department PFA_Department { get; set; }
        public virtual PFA_Destination_plant PFA_Destination_plant { get; set; }
        public virtual PFA_Reason PFA_Reason { get; set; }
        public virtual PFA_Responsible_cost PFA_Responsible_cost { get; set; }
        public virtual PFA_Type_shipment PFA_Type_shipment { get; set; }
        public virtual PFA_Volume PFA_Volume { get; set; }
    }
}
