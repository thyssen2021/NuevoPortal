using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_cat_posicion_rollo_embarquesMetadata
    {

    }

    [MetadataType(typeof(SCDM_cat_posicion_rollo_embarquesMetadata))]
    public partial class SCDM_cat_posicion_rollo_embarques
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatPosicion
        {
            get
            {
                return string.Format("{0} - {1}", !string.IsNullOrEmpty(this.codigo) ? codigo : string.Empty, descripcion);
            }
        }
    }
}