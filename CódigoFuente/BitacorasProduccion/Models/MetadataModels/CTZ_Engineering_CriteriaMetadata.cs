using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Bitacoras.Util;


namespace Portal_2_0.Models
{
    public class CTZ_Engineering_CriteriaMetadata
    {

    }

    [MetadataType(typeof(CTZ_Engineering_CriteriaMetadata))]
    public partial class CTZ_Engineering_Criteria
    {
        [NotMapped]
        public EngineeringCriteria CriteriaEnum
        {
            get { return (EngineeringCriteria)this.ID_Criteria; }
            set { this.ID_Criteria = (int)value; }
            // var criterio = db.CTZ_Engineering_Criteria.Find(1);
            // if (criterio.CriteriaEnum == EngineeringCriteria.GaugeMetric)

        }


    }
}