using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class CTZ_Project_StatusMetadata
    {

    }

    [MetadataType(typeof(CTZ_Project_StatusMetadata))]
    public partial class CTZ_Project_Status
    {
        public string ConcatStatus
        {
            get
            {
                return string.Format("{0}% ({1})", Status_Percent, Description).ToUpper();
            }
        }
    }
}