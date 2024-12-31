using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [GreaterThan("sop_start_of_production", ErrorMessage = "La fecha de EOP debe ser posterior a SOP")]
        public Nullable<System.DateTime> eop_end_of_production { get; set; }

        [Display(Name = "Comentario")]
        [MaxLength(150)]
        public string comentario { get; set; }

        [Display(Name = "Porcentaje scrap")]
        [DisplayFormat(DataFormatString = "{0:P0}", ApplyFormatInEditMode = false)]
        public decimal porcentaje_scrap { get; set; }

        [Display(Name = "Estado")]
        public bool activo { get; set; }

        [Display(Name = "Production Nameplate")]
        [MaxLength(30)]
        public string production_nameplate { get; set; }


    }

    [MetadataType(typeof(BG_IHS_combinacionMetadata))]
    public partial class BG_IHS_combinacion
    {
        [NotMapped]
        [Required]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)]
        [Range(0, 100)]
        [Display(Name = "Porcentaje scrap")]
        public int? porcentaje_scrap_100
        {
            get
            {
                int? val = null;

                if (this.porcentaje_scrap.HasValue)
                    val = (int)(this.porcentaje_scrap * 100);

                if (val.HasValue)
                    return val.Value;
                else
                    return null;
            }
            set
            {
                this.porcentaje_scrap = value / (decimal)100;
            }
        }
    }
}