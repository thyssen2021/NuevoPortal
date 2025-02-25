using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class CTZ_Temp_IHSMetadata
    {
        public int ID_IHS { get; set; }
        public string Vehicle { get; set; }
        public string Program { get; set; }
        public Nullable<System.DateTime> SOP { get; set; }
        public Nullable<System.DateTime> EOP { get; set; }
        public Nullable<double> Max_Production { get; set; }
        public string Mnemonic_Vehicle_plant { get; set; }
        public string Production_Plant { get; set; }
    }

    [MetadataType(typeof(CTZ_Temp_IHSMetadata))]
    public partial class CTZ_Temp_IHS
    {
        [NotMapped]
        public string ConcatCodigo
        {
            get
            {
                return string.Format("{0}_{1}{2}{3}", Mnemonic_Vehicle_plant, Vehicle, "{" + Production_Plant + "}", SOP.HasValue ? EOP.Value.ToString("yyyy-MM") : String.Empty).ToUpper();
            }
        }
    }
}