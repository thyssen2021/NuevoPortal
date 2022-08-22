using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
   
    public class GV_solicitudMetadata
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

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Salida")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_salida { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Regreso")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_regreso { get; set; }


        [Display(Name = "Medio de Transporte")]
        public Nullable<int> id_medio_transporte { get; set; }


        [Display(Name = "Medio Transporte (Otro)")]
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
        public string estatus { get; set; }
    }

    [MetadataType(typeof(GV_solicitudMetadata))]
    public partial class GV_solicitud
    {
        [NotMapped]
        [Display(Name = "Otro")]
        public bool medio_transporte_aplica_otro { get; set; }


    }
}