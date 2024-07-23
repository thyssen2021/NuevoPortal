using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_cat_grado_calidadMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "Clave SAP")]
        [StringLength(4)]
        public string clave { get; set; }
       
        [Display(Name = "Grado/calidad")]
        [StringLength(30)]
        public string grado_calidad { get; set; }

        [Display(Name = "¿Activo?")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(SCDM_cat_grado_calidadMetadata))]
    public partial class SCDM_cat_grado_calidad
    {
        //concatena el nombre
        //[NotMapped]
        //public string ConcatGrado
        //{
        //    get
        //    {
        //        return string.Format("({0}) {1}", !string.IsNullOrEmpty(this.clave) ? clave : string.Empty, grado_calidad);
        //    }
        //}
    }
}