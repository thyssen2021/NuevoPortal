using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{   

    public class BG_IHS_combinacionMetadata
    {
        [Display(Name = "Id")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Vehicle")]
        [MaxLength(100)]
        public string vehicle { get; set; }

        [Required]
        [Display(Name = "Production Brand")]
        [MaxLength(100)]
        public string production_brand { get; set; }

        [Display(Name = "Production Plant")]
        [MaxLength(50)]
        public string production_plant { get; set; }

        [Display(Name = "Manufacturer")]
        [MaxLength(30)]
        public string manufacturer_group { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start of Production")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> sop_start_of_production { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End of Production")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [GreaterThan("sop_start_of_production",ErrorMessage ="La fecha de EOP debe ser posterior a SOP")]
        public Nullable<System.DateTime> eop_end_of_production { get; set; }

        [Display(Name = "Comentario")]
        [MaxLength(150)]
        public string comentario { get; set; }

        [Display(Name = "Estado")]
        public bool activo { get; set; }
    
    }

    [MetadataType(typeof(BG_IHS_combinacionMetadata))]
    public partial class BG_IHS_combinacion
    {

    }
}