using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class BG_IHS_plantasMetadata
    {

    }

    [MetadataType(typeof(BG_IHS_plantasMetadata))]
    public partial class BG_IHS_plantas
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatPlanta
        {
            get
            {
                return string.Format("({0}) {1}", this.mnemonic_plant, this.descripcion);
            }
        }
    }
}