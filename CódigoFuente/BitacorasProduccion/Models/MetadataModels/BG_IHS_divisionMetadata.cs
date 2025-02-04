using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{

    public class BG_IHS_divisionMetadata
    {
        [Display(Name = "Id")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Elemento IHS")]
        public int id_ihs_item { get; set; }
        [Display(Name = "Comentario")]
        [MaxLength(150)]
        public string comentario { get; set; }

        [Display(Name = "Estado")]
        public bool activo { get; set; }

        [Display(Name = "Porcentaje scrap")]
        [DisplayFormat(DataFormatString = "{0:P0}", ApplyFormatInEditMode = false)]
        public decimal porcentaje_scrap { get; set; }

    }

    [MetadataType(typeof(BG_IHS_divisionMetadata))]
    public partial class BG_IHS_division
    {

        [NotMapped]
        [Display(Name = "Elementos en División")]
        public int ElementosEnDivision
        {
            get
            {
                return this.BG_IHS_rel_division.Count;
            }
        }

        [NotMapped]
        [Required]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)]
        [Range(0, 100)]
        [Display(Name = "Porcentaje scrap")]
        public float? porcentaje_scrap_100
        {
            get
            {
                float? val = null;

                if (this.porcentaje_scrap.HasValue)
                    val = (float)(this.porcentaje_scrap * 100);

                if (val.HasValue)
                    return val.Value;
                else
                    return null;
            }
            set
            {
                this.porcentaje_scrap = (decimal)value / 100;
            }
        }
    }

}