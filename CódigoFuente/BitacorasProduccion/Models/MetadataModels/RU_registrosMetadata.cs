using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class RU_registrosMetadata
    {

        [Display(Name = "Folio")]
        public int id { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Ingreso (Vigilancia)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_ingreso_vigilancia { get; set; }

        [Display(Name = "Línea de Transporte")]
        [StringLength(50, MinimumLength = 1)]
        public string linea_transporte { get; set; }

        [Display(Name = "Placas Tractor")]
        [StringLength(20, MinimumLength = 1)]
        public string placas_tractor { get; set; }

        [Display(Name = "Nombre Operador")]
        [StringLength(100, MinimumLength = 1)]
        public string nombre_operador { get; set; }

        [Display(Name = "Vigilancia (Ingreso)")]
        public int id_vigilancia_ingreso { get; set; }

        [Display(Name = "Comentarios Vigilancia (Ingreso)")]
        [StringLength(150, MinimumLength = 1)]
        public string comentarios_vigilancia_ingreso { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Hora recepción (Embarques)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> hora_embarques_recepcion { get; set; }

        [Display(Name = "Embarques (Recepción)")]
        public Nullable<int> id_embarques_recepcion { get; set; }

        [Display(Name = "Comentarios Embarques (Recepción)")]
        [StringLength(150, MinimumLength = 1)]
        public string comentarios_embarques_recepcion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Hora Liberación (Embarques)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> hora_embarques_liberacion { get; set; }

        [Display(Name = "Embarques (Liberación)")]
        public Nullable<int> id_embarques_liberacion { get; set; }

        [Display(Name = "Comentarios Embarques (Liberación)")]
        [StringLength(150, MinimumLength = 1)]
        public string comentarios_embarques_liberacion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Hora Liberación (Vigilancia)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> hora_vigilancia_liberacion { get; set; }

        [Display(Name = "Comentarios Vigilancia (Liberación)")]
        [StringLength(150, MinimumLength = 1)]
        public string comentarios_vigilancia_liberacion { get; set; }

        [Display(Name = "Vigilancia (Liberación)")]
        public Nullable<int> id_vigilancia_liberacion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Hora Salida (Vigilancia)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> hora_vigilancia_salida { get; set; }

        [Display(Name = "Comentarios Vigilancia (Salida)")]
        [StringLength(150, MinimumLength = 1)]
        public string comentarios_vigilancia_salida { get; set; }

        [Display(Name = "Vigilancia (Salida)")]
        public Nullable<int> id_vigilancia_salida { get; set; }

        [Display(Name = "¿Carga?")]
        public bool carga { get; set; }

        [Display(Name = "¿Descarga?")]
        public bool descarga { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Hora Cancelación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> hora_cancelacion { get; set; }

        [Display(Name = "Cancela")]
        [StringLength(100, MinimumLength = 1)]
        public string nombre_cancelacion { get; set; }

        [Display(Name = "Comentarios Cancelación")]
        [StringLength(150, MinimumLength = 1)]
        public string comentario_cancelacion { get; set; }

        [Display(Name = "¿Activo?")]
        public bool activo { get; set; }

        [Display(Name = "Placas Plataforma 1")]
        [StringLength(20, MinimumLength = 1)]
        public string placa_plataforma_uno { get; set; }

        [Display(Name = "Placas Plataforma 2")]
        [StringLength(20, MinimumLength = 1)]
        public string placa_plataforma_dos { get; set; }

        [Display(Name = "Folio")]
        public string folio { get; set; }
        [Display(Name = "Entrada")]
        public int id_acceso { get; set; }
        [Display(Name = "Salida")]
        public int id_salida { get; set; }

    }

    [MetadataType(typeof(RU_registrosMetadata))]
    public partial class RU_registros
    {
        [NotMapped]
        [Display(Name = "Operación")]
        public string TipoOperacion
        {
            get
            {
                if (this.carga && !this.descarga)
                    return "Carga";
                else if (!this.carga && this.descarga)
                    return "Descarga";
                else if (this.carga && this.descarga)
                    return "Carga / Descarga";
                else
                    return string.Empty;
            }
        }

        [NotMapped]
        [Display(Name = "Estado")]
        public string EstadoString
        {
            get
            {
                if(hora_cancelacion.HasValue && !hora_vigilancia_salida.HasValue)
                    return "Cancelado (dentro de planta)";
                else if (hora_cancelacion.HasValue && hora_vigilancia_salida.HasValue)
                    return "Cancelado (fuera de planta)";
                else if (!hora_embarques_recepcion.HasValue)
                    return "Registrado por Vigilancia";
                else if (hora_embarques_recepcion.HasValue && !hora_embarques_liberacion.HasValue)
                    return "Recibido por Embarques";
                else if (hora_embarques_liberacion.HasValue && !hora_vigilancia_liberacion.HasValue)
                    return "Liberado por Embarques";
                else if (hora_vigilancia_liberacion.HasValue && !hora_vigilancia_salida.HasValue)
                    return "Liberado por Vigilancia";
                else if (hora_vigilancia_salida.HasValue)
                    return "Salida de Planta";
                else
                    return string.Empty;
            }
        }
        [NotMapped]
        [Display(Name = "EstadoEnum")]
        public EstatusRURegistrosEnum EstadoEnum
        {
            get
            {
                if (hora_cancelacion.HasValue && !hora_vigilancia_salida.HasValue)
                    return EstatusRURegistrosEnum.CanceladoEnPlanta;
                else if (hora_cancelacion.HasValue && hora_vigilancia_salida.HasValue)
                    return EstatusRURegistrosEnum.Cancelada;
                else if (!hora_embarques_recepcion.HasValue)
                    return EstatusRURegistrosEnum.RegistradoVigilancia;
                else if (hora_embarques_recepcion.HasValue && !hora_embarques_liberacion.HasValue)
                    return EstatusRURegistrosEnum.RecibidoEmbarques;
                else if (hora_embarques_liberacion.HasValue && !hora_vigilancia_liberacion.HasValue)
                    return EstatusRURegistrosEnum.LiberadoEmbarques;
                else if (hora_vigilancia_liberacion.HasValue && !hora_vigilancia_salida.HasValue)
                    return EstatusRURegistrosEnum.LiberadoVigilancia;
                else //if (hora_vigilancia_salida.HasValue)
                    return EstatusRURegistrosEnum.SalidaPlanta;
                
            }
        }
    }

    public enum EstatusRURegistrosEnum
    {
        RegistradoVigilancia,
        RecibidoEmbarques,
        LiberadoEmbarques,
        LiberadoVigilancia,
        SalidaPlanta,
        CanceladoEnPlanta,
        Cancelada
    }
}