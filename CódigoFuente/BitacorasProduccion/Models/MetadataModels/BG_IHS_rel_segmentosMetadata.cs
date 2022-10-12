using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
   
    public class BG_IHS_rel_segmentosMetadata
    {

        [Display(Name = "Clave")]
        public int id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(30)]
        [Display(Name = "Global Product Segment")]
        public string global_production_segment { get; set; }

        [Display(Name = "Flat Rolled Steel Usage")]
        [Range(1, Int32.MaxValue, ErrorMessage ="Ingrese un número válido")]
        public Nullable<int> flat_rolled_steel_usage { get; set; }

        [Display(Name = "Blanks Usage Percent")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        [Range(0, 100)]
        public Nullable<decimal> blanks_usage_percent { get; set; }

       

    }

    [MetadataType(typeof(BG_IHS_rel_segmentosMetadata))]
    public partial class BG_IHS_rel_segmentos
    {
        //concatena el nombre
        [NotMapped]
        [Display(Name = "Blanks Usage")]
        public decimal BlanksUsage
        {
            get
            {
                try
                {
                    return this.flat_rolled_steel_usage.GetValueOrDefault() * this.blanks_usage_percent.GetValueOrDefault();
                }
                catch {
                    return 0.0M;
                }

                
            }
        }
    }
}