using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_cat_tratamiento_superficialMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "Tratamiento Superficial")]
        [StringLength(30)]
        public string descripcion { get; set; }

        [Display(Name = "Clave SAP")]
        [StringLength(3)]
        public string clave { get; set; }

        [Display(Name = "¿Activo?")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(SCDM_cat_tratamiento_superficialMetadata))]
    public partial class SCDM_cat_tratamiento_superficial
    {
       
    }
}