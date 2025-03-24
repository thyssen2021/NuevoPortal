using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class view_ideas_mejoraMetadata
    {
        public int id { get; set; }
        [Display(Name = "Folio")]
        public string folio { get; set; }
        public Nullable<int> clave_planta { get; set; }
        [Display(Name = "Planta")]
        public string nombre_planta { get; set; }
        [Display(Name = "Fecha Captura")]
        public System.DateTime fecha_captura { get; set; }
        [Display(Name = "Título")]
        public string titulo { get; set; }
        public string situacionActual { get; set; }
        public string objetivo { get; set; }
        public string descripcion { get; set; }
        public Nullable<bool> comiteAceptada { get; set; }
        public Nullable<bool> ideaEnEquipo { get; set; }
        public Nullable<int> clasificacionClave { get; set; }
        public string Clasificacion_text { get; set; }
        public Nullable<int> nivelImpactoClave { get; set; }
        public string nivel_impacto_text { get; set; }
        public Nullable<bool> enProcesoImplementacion { get; set; }
        public Nullable<int> plantaImplementacionClave { get; set; }
        public string planta_implementacion_text { get; set; }
        public Nullable<int> areaImplementacionClave { get; set; }
        public string area_implementacion_text { get; set; }
        public Nullable<bool> ideaImplementada { get; set; }
        public Nullable<System.DateTime> implementacionFecha { get; set; }
        public Nullable<int> reconocimentoClave { get; set; }
        public string reconocimiento_text { get; set; }
        public Nullable<decimal> reconocimientoMonto { get; set; }
        public Nullable<int> estatus_id { get; set; }
        [Display(Name = "Estatus")]
        public string estatus_text { get; set; }

        [Display(Name = "Tipo de Idea")]
        public string tipo_idea { get; set; }
        public string proponentes { get; set; }
        public Nullable<int> proponente_1_id { get; set; }
        public string proponente_1_nombre { get; set; }
        public Nullable<int> proponente_2_id { get; set; }
        public string proponente_2_nombre { get; set; }
        public Nullable<int> proponente_3_id { get; set; }
        public string proponente_3_nombre { get; set; }
        public Nullable<int> proponente_4_id { get; set; }
        public string proponente_4_nombre { get; set; }
        public Nullable<int> proponente_5_id { get; set; }
        public string proponente_5_nombre { get; set; }
    }

    [MetadataType(typeof(view_ideas_mejoraMetadata))]
    public partial class view_ideas_mejora
    {

    }
}