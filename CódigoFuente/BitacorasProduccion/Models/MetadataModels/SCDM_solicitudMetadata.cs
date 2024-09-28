using Bitacoras.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_solicitudMetadata
    {
        [Display(Name = "Folio")]
        public int id { get; set; }

        [Display(Name = "Tipo de Solicitud")]
        public int id_tipo_solicitud { get; set; }

        [Display(Name = "Tipo de Cambio")]
        public Nullable<int> id_tipo_cambio { get; set; }

        [Display(Name = "Prioridad")]
        public int id_prioridad { get; set; }

        [Display(Name = "Solicitante")]
        public int id_solicitante { get; set; }

        [StringLength(180, MinimumLength = 1)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [StringLength(250, MinimumLength = 1)]
        [Display(Name = "Justificación")]
        public string justificacion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Creación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_creacion { get; set; }

        [Display(Name = "¿On hold?")]
        public bool on_hold { get; set; }

        [Display(Name = "¿Activo?")]
        public bool activo { get; set; }

    }

    [MetadataType(typeof(SCDM_solicitudMetadata))]
    public partial class SCDM_solicitud
    {
        //get status solicitud
        [NotMapped]
        [Display(Name = "Estado")]
        public SCMD_solicitud_estatus_enum EstatusSolicitud
        {
            //determina el estatus de la solicitud en base al estado actual de las asignaciones de la solictud
            get
            {
                //si no hay ninguna asigacion
                if (this.SCDM_solicitud_asignaciones.Count == 0)
                {
                    return SCMD_solicitud_estatus_enum.CREADO;
                }

                if(!this.activo)
                    return SCMD_solicitud_estatus_enum.CANCELADA;

                //obtiene todas las solicitudes abiertas
                var asignacionesAbiertas = this.SCDM_solicitud_asignaciones.Where(x => x.id_cierre == null && x.id_rechazo == null);

                //si no hay asignaciones abiertas, ya fue finalizada
                if (asignacionesAbiertas.Count() == 0)
                    return SCMD_solicitud_estatus_enum.FINALIZADA;

                //si hay alguna asignacion abierta al departamento inicial
                if (asignacionesAbiertas.Count() == 1 && asignacionesAbiertas.Any(x => x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL))
                    return SCMD_solicitud_estatus_enum.EN_REVISION_INICIAL;

                //verifica si esta asignado a SCDM
                if (asignacionesAbiertas.Count() == 1 && asignacionesAbiertas.Any(x => x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM))
                    return SCMD_solicitud_estatus_enum.ASIGNADA_A_SCDM;

                //verifica si esta asignado a departamentos
                if (asignacionesAbiertas.Count() == asignacionesAbiertas.Where(x => x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO).Count())
                    return SCMD_solicitud_estatus_enum.ASIGNADA_A_DEPARTAMENTOS;

                //verifica si esta asignada al solicitante
                if (asignacionesAbiertas.Any(x => x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE))
                    return SCMD_solicitud_estatus_enum.RECHAZADA_ASIGNADA_A_SOLICITANTE;

                //verifica si existen rechazos
                var asignacionesRechazadas = this.SCDM_solicitud_asignaciones.Where(x => x.id_rechazo != null);
                var fechasRechazo = asignacionesRechazadas.Select(x => x.fecha_rechazo).ToList();

                //verifica si fecha de asignacion de la asignacion actual a SCDM concide con alguna fecha de rechazo
                if (asignacionesAbiertas.Any(x => x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM && fechasRechazo.Any(y => y == x.fecha_asignacion)))
                    return SCMD_solicitud_estatus_enum.RECHAZADA_ASIGNADA_A_SCDM;

                //verifica que existan solicitudes abiertas para SCDM y departamentos y que ninguna coincida con la fecha de rechazo
                if (asignacionesAbiertas.Any(x => x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM && !fechasRechazo.Any(y => y == x.fecha_asignacion))
                    && asignacionesAbiertas.Any(x => x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_DEPARTAMENTO && !fechasRechazo.Any(y => y == x.fecha_asignacion))
                    )
                    return SCMD_solicitud_estatus_enum.ASIGNADA_A_SCDM_Y_DEPARTAMENTOS;



                return SCMD_solicitud_estatus_enum.SIN_DEFINIR;
            }
            /*
            get
            {
                if (this.SCDM_solicitud_asignaciones.Count == 0)
                {
                    return SCMD_solicitud_estatus_enum.CREADO;
                }
                else if (!this.SCDM_solicitud_asignaciones.Any(x => x.fecha_cierre == null && x.fecha_rechazo == null)
                    && this.SCDM_solicitud_asignaciones.LastOrDefault().fecha_cierre != null
                    )
                {
                    return SCMD_solicitud_estatus_enum.FINALIZADA;
                }
                //determina si fue rechazada por scdm o revision inicial
                else if (this.SCDM_solicitud_asignaciones.Any(x => x.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE && x.fecha_cierre == null && x.fecha_rechazo == null
                     ))
                {
                    SCDM_solicitud_asignaciones rechazo = this.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_rechazo != null);
                    
                    //si no hay solicitud de rechazo previa, se considera rechazado por scdm
                    if (rechazo == null) {
                        return SCMD_solicitud_estatus_enum.RECHAZADA_SCDM;
                    }

                    return rechazo.descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL ? SCMD_solicitud_estatus_enum.RECHAZADA_ASIGNACION_INICIAL :  SCMD_solicitud_estatus_enum.RECHAZADA_SCDM;
                }
                
                //*CUANDO UN DEPARTAMENTO RECHAZE UNA SOLICITUD, RECHAZAR TODAS LAS DEMAS ASIGNACIONES
                else if (this.SCDM_solicitud_asignaciones.LastOrDefault().fecha_rechazo != null && this.SCDM_solicitud_asignaciones.LastOrDefault().id_departamento_asignacion != 9)
                {
                    return SCMD_solicitud_estatus_enum.RECHAZADA_DEPARTAMENTO;
                }
                else if (this.SCDM_solicitud_asignaciones.Any(y => y.id_departamento_asignacion == 9 && y.fecha_cierre == null && y.fecha_rechazo == null))
                {
                    return SCMD_solicitud_estatus_enum.SCDM;
                }
                else if (this.SCDM_solicitud_asignaciones.LastOrDefault().fecha_cierre == null && this.SCDM_solicitud_asignaciones.LastOrDefault().fecha_rechazo == null
                    && this.SCDM_solicitud_asignaciones.LastOrDefault().id_departamento_asignacion != 9
                    && this.SCDM_solicitud_asignaciones.LastOrDefault().descripcion == Bitacoras.Util.SCDM_solicitudes_asignaciones_tipos.ASIGNACION_INICIAL
                    )
                {
                    return SCMD_solicitud_estatus_enum.ASIGNACION_INICIAL;
                }
                else if (this.SCDM_solicitud_asignaciones.Any(y => y.id_departamento_asignacion != 9 && y.fecha_cierre == null && y.fecha_rechazo == null))
                {
                    return SCMD_solicitud_estatus_enum.ASIGNADA;
                }

                //valor por defecto
                return SCMD_solicitud_estatus_enum.CREADO;
            }*/
        }

        public string ClienteString
        {
            //determina el estatus de la solicitud en base al estado actual de las asignaciones de la solictud
            get { return GetClientes(); }
        }

        public TimeSpan? GetTiempoAsignacion(int id_departamento, List<DateTime> diasFestivos = null)
        {

            TimeSpan? result = null;

            //obtiene las ultima asignación del departamento asignado
            var ultimaAsignacion = this.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_departamento_asignacion == id_departamento && x.descripcion != SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE);

            if(id_departamento == 99) //si es el solicitante
                ultimaAsignacion = this.SCDM_solicitud_asignaciones.LastOrDefault(x=>x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE);
           

            //si no hay asignaciones retorna null
            if (ultimaAsignacion == null)
                return result;

            //si hay asignación valida si es una asignación abierta
            if (ultimaAsignacion.fecha_cierre == null && ultimaAsignacion.fecha_rechazo == null)
                return CalculaHoras(ultimaAsignacion.fecha_asignacion, DateTime.Now, diasFestivos);

            //en caso contrario retorna null 
            return result;

        }

        public TimeSpan? GetTiempoAsignacionAprobada(int id_departamento, List<DateTime> diasFestivos = null)
        {
            TimeSpan? result = null;

            //obtiene la última asignación del repartamento
            var ultimaAsignacion = this.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_departamento_asignacion == id_departamento && x.descripcion != SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE);

            //si no hay asignaciones retorna null
            if (ultimaAsignacion == null)
                return result;

            //si hay asignación, valida si la asignacion esta cerrada y no es asignacion incorrecta
            if (ultimaAsignacion.fecha_cierre != null && ultimaAsignacion.id_motivo_asignacion_incorrecta == null)
                return CalculaHoras(ultimaAsignacion.fecha_asignacion, ultimaAsignacion.fecha_cierre.Value, diasFestivos);

            //en caso contrario retorna null 
            return result;

        }
        public TimeSpan? GetTiempoAsignacionIncorrecta(int id_departamento, List<DateTime> diasFestivos = null)
        {
            TimeSpan? result = null;

            //obtiene la última asignación del repartamento
            var ultimaAsignacion = this.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_departamento_asignacion == id_departamento && x.descripcion != SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE);

            //si no hay asignaciones retorna null
            if (ultimaAsignacion == null)
                return result;

            //si hay asignación, valida si la asignacion esta cerrada y es asignación incorrecta
            if (ultimaAsignacion.fecha_cierre != null && ultimaAsignacion.id_motivo_asignacion_incorrecta != null)
                return CalculaHoras(ultimaAsignacion.fecha_asignacion, ultimaAsignacion.fecha_cierre.Value, diasFestivos);

            //en caso contrario retorna null 
            return result;

        }
        public TimeSpan? GetTiempoAsignacionRechazada(int id_departamento, List<DateTime> diasFestivos = null)
        {
            TimeSpan? result = null;

            //obtiene la última asignación del repartamento
            var ultimaAsignacion = this.SCDM_solicitud_asignaciones.LastOrDefault(x => x.id_departamento_asignacion == id_departamento && x.descripcion != SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE);

            //si no hay asignaciones retorna null
            if (ultimaAsignacion == null)
                return result;

            //si hay asignación, valida si la asignacion esta cerrada
            if (ultimaAsignacion.fecha_rechazo != null)
                return CalculaHoras(ultimaAsignacion.fecha_asignacion, ultimaAsignacion.fecha_rechazo.Value, diasFestivos);

            //en caso contrario retorna null 
            return result;
        }



        private TimeSpan CalculaHoras(DateTime fechaInicio, DateTime fechaFin, List<DateTime> diasFestivos = null)
        {
            //si no se define el parámetro por defecto inicializa el list de días festivos
            diasFestivos = diasFestivos ?? GetDiasFestivos();

            Calculation calculation = new Calculation(diasFestivos, new OpenHours("08:00;18:00"));

            Double result = calculation.getElapsedMinutes(fechaInicio, fechaFin);

            return TimeSpan.FromMinutes(result);
        }

        /// <summary>
        /// Obtiene todas las lineas activas para un empleado según sus asignaciones
        /// </summary>
        /// <returns></returns>
        public List<DateTime> GetDiasFestivos()
        {
            List<DateTime> listado = new List<DateTime>();

            using (var db = new Portal_2_0Entities())
            {
                listado = db.SCDM_cat_dias_feriados.Select(x => x.fecha).ToList();
            }
            return listado;
        }

        public string GetClientes()
        {
            string result = string.Empty;

            //obtiene los clientes desde los diferentes tipos de solicitud
            using (var db = new Portal_2_0Entities())
            {
                List<string> clientes = db.view_SCDM_clientes_por_solictud.Where(x => x.id_solicitud == this.id).Select(x => x.sap_cliente).ToList();
                if (clientes.Count > 0)
                {
                    result = String.Join(", ", clientes);
                }
                else
                {
                    result = "N/D";
                }
            }

            return result;
        }

        public bool ExisteClienteEnSolicitud(string numClienteSAP)
        {
            //obtiene los clientes desde los diferentes tipos de solicitud
            using (var db = new Portal_2_0Entities())
            {
                return db.view_SCDM_clientes_por_solictud.Any(x => x.id_solicitud == this.id && x.sap_cliente == numClienteSAP);
            }
        }


        [NotMapped]
        [Display(Name = "Estatus")]
        public string estatusTexto
        {
            get
            {
                switch (this.EstatusSolicitud)
                {
                    case SCMD_solicitud_estatus_enum.CREADO:
                        return "Creado";
                    case SCMD_solicitud_estatus_enum.EN_REVISION_INICIAL:
                        return "En revisión inicial";
                    case SCMD_solicitud_estatus_enum.ASIGNADA_A_SCDM:
                        return "Asignada a SCDM";
                    case SCMD_solicitud_estatus_enum.ASIGNADA_A_DEPARTAMENTOS:
                        return "Asignada a Departamentos";
                    case SCMD_solicitud_estatus_enum.ASIGNADA_A_SCDM_Y_DEPARTAMENTOS:
                        return "Asignada a SCDM/Departamentos";
                    case SCMD_solicitud_estatus_enum.RECHAZADA_ASIGNADA_A_SCDM:
                        return "Rechazada - Asignada a SCDM";
                    case SCMD_solicitud_estatus_enum.RECHAZADA_ASIGNADA_A_SOLICITANTE:
                        return "Rechazada - Asignada a Solicitante";
                    case SCMD_solicitud_estatus_enum.FINALIZADA:
                        return "Finalizada";
                    case SCMD_solicitud_estatus_enum.CANCELADA:
                        return "Cancelada";
                    default:
                        return "Sin Definir";
                }
            }
        }


        [NotMapped]
        [Display(Name = "Documento 1 (opcional)")]
        public HttpPostedFileBase PostedFileSolicitud_1 { get; set; }
        [NotMapped]
        [Display(Name = "Documento 2 (opcional)")]
        public HttpPostedFileBase PostedFileSolicitud_2 { get; set; }
        [NotMapped]
        [Display(Name = "Documento 2 (opcional)")]
        public HttpPostedFileBase PostedFileSolicitud_3 { get; set; }

        [StringLength(250, MinimumLength = 1)]
        [Display(Name = "Comentario Rechazo")]
        public string comentario_rechazo { get; set; }

        [StringLength(250, MinimumLength = 1)]
        [Display(Name = "Comentario Asignación Incorrecta")]
        public string comentario_asignacion_incorrecta { get; set; }


        [Display(Name = "Motivo Rechazo")]
        public int id_motivo_rechazo { get; set; }
        [Display(Name = "Motivo Asignación Incorrecta")]
        public int id_motivo_asignacion_incorrecta { get; set; }

        [StringLength(250, MinimumLength = 1)]
        [Display(Name = "Comentario Rechazo")]
        public string comentario_rechazo_departamento { get; set; }

        [Display(Name = "Motivo Rechazo")]
        public int id_motivo_rechazo_departamento { get; set; }

    }
    public enum SCMD_solicitud_estatus_enum
    {
        CREADO = 1,     //creada sin enviar
        EN_REVISION_INICIAL = 2,   //En revision Inicial
        ASIGNADA_A_SCDM = 3,   //Asignada a SCDM
        ASIGNADA_A_DEPARTAMENTOS = 4,       //asignada a departamentos  
        ASIGNADA_A_SCDM_Y_DEPARTAMENTOS = 5,       //asignada a departamentos  
        RECHAZADA_ASIGNADA_A_SCDM = 6,  //rechazada asignada a SCDM
        RECHAZADA_ASIGNADA_A_SOLICITANTE = 7,  //rechazada asignada a solicitante
        FINALIZADA = 8, //finalizada
        SIN_DEFINIR = 9,
        CANCELADA = 10,

    }


}