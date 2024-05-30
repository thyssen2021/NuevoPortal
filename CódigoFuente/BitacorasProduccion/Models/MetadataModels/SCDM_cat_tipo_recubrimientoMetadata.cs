using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_cat_tipo_recubrimientoMetadata
    {

    }

    [MetadataType(typeof(SCDM_cat_tipo_recubrimientoMetadata))]
    public partial class SCDM_cat_tipo_recubrimiento
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatRecubrimiento
        {
            get
            {
                return string.Format("{0} --> {1}", !string.IsNullOrEmpty(this.codigo) ? codigo : "-", descripcion);
            }
        }
    }
}