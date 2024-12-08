using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class BG_IHS_rel_cuartosMetadata
    {
    }

    [MetadataType(typeof(BG_IHS_rel_cuartosMetadata))]
    public partial class BG_IHS_rel_cuartos
    {
        //Origen de los datos (utilizado por el método GB_IHS_item.getDemanda())
        [NotMapped]
        public Enum_BG_origen_cuartos origen_datos { get; set; }

    }
}