using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class CTZ_Coil_PositionMetadata
    {
        public int ID_Coil_Position { get; set; }
        public string PositionKey { get; set; }
        public string Description_ES { get; set; }
        public string Description_EN { get; set; }
        public bool Active { get; set; }
    }

    [MetadataType(typeof(CTZ_Coil_PositionMetadata))]
    public partial class CTZ_Coil_Position
    {
        [NotMapped]
        public string ConcatDescription
        {
            get
            {
                return string.Format("{0} - {1}", PositionKey, Description_EN).ToUpper();
            }
        }
    }
}