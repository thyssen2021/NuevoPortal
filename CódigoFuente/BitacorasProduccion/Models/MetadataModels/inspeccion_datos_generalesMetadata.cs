using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class inspeccion_datos_generalesMetadata
    {
        [Display(Name = "Inspector")]
        public int id_empleado_inspector { get; set; }


        [StringLength(255, MinimumLength = 2)]
        [Display(Name = "Comentarios")]
        public string comentarios { get; set; }
    }

    [MetadataType(typeof(inspeccion_datos_generalesMetadata))]
    public partial class inspeccion_datos_generales
    {

    }
}