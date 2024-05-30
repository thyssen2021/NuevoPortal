﻿using Bitacoras.Util;
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

        public TimeSpan? GetTiempoAsignacion(int id_departamento)
        {
            TimeSpan? result = null;


            //obtiene las asignaciones del departamento asignado
            var asignaciones = this.SCDM_solicitud_asignaciones.Where(x => x.id_departamento_asignacion == id_departamento).ToList();

            if (asignaciones.Any(x => x.fecha_cierre == null && x.fecha_rechazo == null))
            {
                //obtiene la asinacion
                var asignacion = asignaciones.FirstOrDefault(x => x.fecha_cierre == null && x.fecha_rechazo == null);
                //obtiene los minutos transcurridos
                //TimeSpan timeSpan = DateTime.Now - asignacion.fecha_asignacion;

                return CalculaHoras(asignacion.fecha_asignacion, DateTime.Now);
            }

            return result;

        }

        private TimeSpan CalculaHoras(DateTime fechaInicio, DateTime fechaFin)
        {

            List<DateTime> diasFestivos = GetDiasFestivos();

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

        [Display(Name = "Motivo Rechazo")]
        public int id_motivo_rechazo { get; set; }

        [StringLength(250, MinimumLength = 1)]
        [Display(Name = "Comentario Rechazo")]
        public string comentario_rechazo_departamento { get; set; }

        [Display(Name = "Motivo Rechazo")]
        public int id_motivo_rechazo_departamento { get; set; }

    }
    public enum SCMD_solicitud_estatus_enum
    {
        CREADO = 1,     //creada sin enviar
        ASIGNADA = 2,   //asignada (revisión inicial)
        ASIGNACION_INICIAL = 3,   //asignada a cualquier otro depto
        SCDM = 4,       //asignada a scdm  
        RECHAZADA_SCDM = 5,  //rechazada por SCDM
        RECHAZADA_DEPARTAMENTO = 6,  //rechazada por DEPARTAMENTO
        RECHAZADA_ASIGNACION_INICIAL = 7,  //rechazada por DEPARTAMENTO (inicio)
        FINALIZADA = 8, //finalizada
        SIN_DEFINIR = 9
    }
}