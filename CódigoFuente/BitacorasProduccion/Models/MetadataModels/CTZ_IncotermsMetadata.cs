using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class CTZ_Incoterms_Metadata
    {
        public int ID_Incoterm { get; set; }
        public string key { get; set; }
        public string Description_ES { get; set; }
        public string Description_EN { get; set; }
        public bool Active { get; set; }
    }

    [MetadataType(typeof(CTZ_Incoterms_Metadata))]
    public partial class CTZ_Incoterms
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatEnglish
        {
            get
            {
                return string.Format("{0} - {1} ", key, Description_EN);
            }
        } 
        [NotMapped]
        public string ConcatSpanish
        {
            get
            {
                return string.Format("{0} - {1} ", key, Description_ES);
            }
        }
    }
}