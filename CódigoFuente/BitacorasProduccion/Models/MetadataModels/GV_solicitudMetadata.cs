using Foolproof;
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
        [Display(Name = "Folio")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Solicitante")]
        public int id_solicitante { get; set; }

        [Required]
        [Display(Name = "Empleado")]
        public int id_empleado { get; set; }

        [Required]
        [Display(Name = "Jefe Directo")]
        public int id_jefe_directo { get; set; }

        [Display(Name = "Controlling")]
        public Nullable<int> id_controlling { get; set; }

        [Display(Name = "Cuentas por Pagar")]
        public Nullable<int> id_contabilidad { get; set; }

        [Display(Name = "Nóminas")]
        public Nullable<int> id_nomina { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Solicitud")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_solicitud { get; set; }

        [Required]
        [MaxLength(15)]
        [Display(Name = "Tipo de Viaje")]
        public string tipo_viaje { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Origen")]
        public string origen { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Destino")]
        public string destino { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Salida")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_salida { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Regreso")]
        [GreaterThanOrEqualTo("fecha_salida", ErrorMessage = "La fecha de regreso debe ser posterior a la fecha de salida.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_regreso { get; set; }

        //agregar requided if
        [Display(Name = "Medio de Transporte")]
        [RequiredIf("medio_transporte_aplica_otro", false, ErrorMessage = "El campo {0} es obligatorio.")]
        public Nullable<int> id_medio_transporte { get; set; }

        [MaxLength(80)]
        [Display(Name = "Medio Transporte (Otro)")]
        [RequiredIf("medio_transporte_aplica_otro", true, ErrorMessage = "El campo {0} es obligatorio.")]
        public string medio_transporte_otro { get; set; }

        [MaxLength(350)]
        [Required]
        [Display(Name = "Motivo de Viaje")]
        public string motivo_viaje { get; set; }

        [Display(Name = "Moneda Extranjera")]
        public bool moneda_extranjera { get; set; }

        [MaxLength(120)]
        [Display(Name = "Comentarios (Moneda Extranjera)")]
        public string moneda_extranjera_comentarios { get; set; }

        [Display(Name = "Monto")]
        [RequiredIf("moneda_extranjera", true, ErrorMessage = "El campo {0} es obligatorio.")]
        public Nullable<decimal> moneda_extranjera_monto { get; set; }

        [Display(Name = "Tipo Moneda")]
        [RequiredIf("moneda_extranjera", true, ErrorMessage = "El campo {0} es obligatorio.")]
        public string iso_moneda_extranjera { get; set; }

        [Display(Name = "¿Boletos Avión?")]
        public bool boletos_avion { get; set; }

        [Display(Name = "Hospedaje")]
        public bool hospedaje { get; set; }

        [MaxLength(120)]
        [Display(Name = "Comentarios (Reservaciones)")]
        public string reservaciones_comentarios { get; set; }

        [Display(Name = "Aceptación Jefe")]
        public Nullable<System.DateTime> fecha_aceptacion_jefe_area { get; set; }

        [Display(Name = "Aceptación Controlling")]
        public Nullable<System.DateTime> fecha_aceptacion_controlling { get; set; }


        [Display(Name = "Confirmación Cuentas por Pagar")]
        public Nullable<System.DateTime> fecha_aceptacion_contabilidad { get; set; }

        [Display(Name = "Confirmación Nóminas")]
        public Nullable<System.DateTime> fecha_aceptacion_nomina { get; set; }

        [Display(Name = "Confirmación Solicitante")]
        public Nullable<System.DateTime> fecha_confirmacion_usuario { get; set; }

        [MaxLength(355)]
        [Display(Name = "Comentario rechazo")]
        public string comentario_rechazo { get; set; }

        [MaxLength(355)]
        [Display(Name = "Comentario adicional")]
        public string comentario_adicional { get; set; }

        [Display(Name = "Estado")]
        public string estatus { get; set; }
    }

    [MetadataType(typeof(GV_solicitudMetadata))]
    public partial class GV_solicitud
    {       

        [NotMapped]
        [Display(Name = "Otro")]
        public bool medio_transporte_aplica_otro { get; set; }
       
        [NotMapped]
        [Display(Name = "Suma Gastos Nacional")]
        public decimal sumaGastosNacional {
            get {
                return this.GV_rel_gastos_solicitud.Sum(x => x.total);            
            }        
        }

        //para filtros de búsqueda
        [NotMapped]
        public string s_estatus { get; set; }


    }
}