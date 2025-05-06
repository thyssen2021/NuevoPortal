using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class CTZ_Countries_Metadata
    {
        public int ID_Country { get; set; }
        public string ISO { get; set; }
        public string Name { get; set; }
        public string Nicename { get; set; }
        public string ISO3 { get; set; }
        public bool Active { get; set; }
        public bool Warning { get; set; }
    }

    [MetadataType(typeof(CTZ_Countries_Metadata))]
    public partial class CTZ_Countries
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatKey
        {
            get
            {
                return string.Format("{0} - {1} ", ISO3, Nicename);
            }
        }
       
    }
}