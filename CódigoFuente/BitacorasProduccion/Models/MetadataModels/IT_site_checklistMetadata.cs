using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
        public class IT_site_checklistMetadata
    {

        [Display(Name = "Folio")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Site")]
        public int id_site { get; set; }

        [Required]
        [Display(Name = "Elaboró")]
        public int id_sistemas { get; set; }

        [Display(Name = "Observación Particular")]
        [MaxLength(250)]
        public string observacion_particular { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha { get; set; }

        [Display(Name = "Estatus")]
        [MaxLength(20)]
        public string estatus { get; set; }

      
    }

    [MetadataType(typeof(IT_site_checklistMetadata))]
    public partial class IT_site_checklist
    {

    }

}