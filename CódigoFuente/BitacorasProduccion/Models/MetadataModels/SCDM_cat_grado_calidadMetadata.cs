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