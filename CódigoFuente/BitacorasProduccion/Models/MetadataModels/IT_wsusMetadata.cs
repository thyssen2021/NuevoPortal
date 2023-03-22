using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_wsusMetadata
    {

        [Display(Name = "Id")]
        public int id { get; set; }
        [Display(Name = "Name")]
        public string name { get; set; }
        [Display(Name = "IP")]
        public string ip { get; set; }
        [Display(Name = "Operating System")]
        public string operating_system { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Subida")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha { get; set; }

    }

    [MetadataType(typeof(IT_wsusMetadata))]
    public partial class IT_wsus
    {

    }
}