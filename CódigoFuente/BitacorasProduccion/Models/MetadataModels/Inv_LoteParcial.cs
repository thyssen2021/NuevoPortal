using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models.MetadataModels
{
    public class Inv_LoteMetadata
    {
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

    [MetadataType(typeof(Inv_LoteMetadata))]
    public partial class Inv_Lote
    {
        
    }
   
}