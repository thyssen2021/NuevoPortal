using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_cat_motivo_creacionMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "Motivo")]
        [StringLength(30)]
        public string descripcion { get; set; }

     
        [Display(Name = "¿Activo?")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(SCDM_cat_motivo_creacionMetadata))]
    public partial class SCDM_cat_motivo_creacion
    {
     
    }
}