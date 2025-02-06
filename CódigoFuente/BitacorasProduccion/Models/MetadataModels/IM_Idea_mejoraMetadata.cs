using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IM_Idea_mejoraMetadata
    {
        [Display(Name = "Folio")]
        public int id { get; set; }

        [Display(Name = "Folio")]
        public string folio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Captura")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime captura { get; set; }

        [StringLength(150)]
        [Display(Name = "Titulo de la idea")]
        public string titulo { get; set; }

        [StringLength(2000)]
        [Display(Name = "Situación Actual")]
        public string situacionActual { get; set; }

        [StringLength(2000)]
        [Display(Name = "Objetivo de la Idea")]
        public string objetivo { get; set; }

        [StringLength(2000)]
        [Display(Name = "Situación Propuesta")]
        public string descripcion { get; set; }

        [Display(Name = "Aceptada Comite")]
        public byte comiteAceptada { get; set; }


        [Display(Name = "¿Idea en Equipo?")]
        public byte ideaEnEquipo { get; set; }

        [Display(Name = "Clasificación de la Idea")]
        public Nullable<int> clasificacionClave { get; set; }

        [Display(Name = "Nivel de Impacto")]
        public Nullable<int> nivelImpactoClave { get; set; }

        [Display(Name = "¿En proceso de Implementación?")]
        public byte enProcesoImplementacion { get; set; }

        //la planta está asociada al departamento
        [Display(Name = "Area donde se implementa")]
        public Nullable<int> areaImplementacionClave { get; set; }

        [Display(Name = "¿Idea Implementada?")]
        public Nullable<byte> ideaImplementada { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Implementación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime implementacionFecha { get; set; }

        [Display(Name = "Tipo de Reconocimiento")]
        public Nullable<int> reconocimentoClave { get; set; }

        [Display(Name = "Monto del Reconocimiento")]
        public decimal reconocimientoMonto { get; set; }

        [Display(Name = "Planta")]
        public Nullable<int> clave_planta { get; set; }

        [Display(Name = "Tipo de Idea")]
        public string tipo_idea { get; set; }
    }

    [MetadataType(typeof(IM_Idea_mejoraMetadata))]
    public partial class IM_Idea_mejora
    {

        [NotMapped]
        public string ConcatFolio
        {
            get
            {
                string planta = string.Empty;

                switch (this.clave_planta)
                {
                    case 1:
                        planta = "PUE";
                        break;
                    case 2:
                        planta = "SIL";
                        break;
                    case 3:
                        planta = "SAL";
                        break;
                    case 4:
                        planta = "SLP";
                        break;
                    default:
                        planta = String.Empty;
                        break;
                }



                return string.Format("{0}-{1}", planta, this.id).ToUpper();
            }
        }


        [NotMapped]
        [Display(Name = "¿Idea en Equipo?")]
        public bool ideaEnEquipoBool { get; set; }

        [NotMapped]
        [Display(Name = "¿En Proceso de Implementacion?")]
        public bool enProcesoBool { get; set; }

        [NotMapped]
        [Display(Name = "Planta de implementación")]
        public int? id_planta_implementacion { get; set; }

        [NotMapped]
        [StringLength(600)]
        [Display(Name = "Comentario")]
        public string comentario { get; set; }

        [NotMapped]
        [Display(Name = "Archivo 1")]
        public HttpPostedFileBase PostedFile1 { get; set; }
        [NotMapped]
        [Display(Name = "Archivo 2")]
        public HttpPostedFileBase PostedFile2 { get; set; }
        [NotMapped]
        [Display(Name = "Archivo 3")]
        public HttpPostedFileBase PostedFile3 { get; set; }
        [NotMapped]
        [Display(Name = "Archivo 4")]
        public HttpPostedFileBase PostedFile4 { get; set; }

    }
}