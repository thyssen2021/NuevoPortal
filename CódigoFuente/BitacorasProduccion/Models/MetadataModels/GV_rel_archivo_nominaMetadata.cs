using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class GV_rel_archivo_nominaMetadata
    {
        }

    [MetadataType(typeof(GV_rel_archivo_nominaMetadata))]
    public partial class GV_rel_archivo_nomina
    {
        [NotMapped]
        public HttpPostedFileBase PostedFilePDF { get; set; }
        [NotMapped]
        public HttpPostedFileBase PostedFileSAP { get; set; }
        

    }
}