using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_solicitud_rel_item_materialMetadata
    {

    }

    [MetadataType(typeof(SCDM_solicitud_rel_item_materialMetadata))]
    public partial class SCDM_solicitud_rel_item_material
    {
        [NotMapped]
        public int num_fila { get; set; }
    }
}