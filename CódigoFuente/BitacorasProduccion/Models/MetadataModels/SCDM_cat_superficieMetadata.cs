﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_cat_superficieMetadata
    {

    }

    [MetadataType(typeof(SCDM_cat_superficieMetadata))]
    public partial class SCDM_cat_superficie
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatClaveDescripcion
        {
            get
            {
                return string.Format("{0} - {1}", !string.IsNullOrEmpty(this.clave) ? clave : "-", descripcion);
            }
        }
    }
}