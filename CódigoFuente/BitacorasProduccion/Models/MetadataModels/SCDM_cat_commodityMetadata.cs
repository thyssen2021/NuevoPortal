using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_cat_commodityMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "Commodity")]
        [StringLength(30)]
        public string descripcion { get; set; }

        [Display(Name = "Clave SAP")]
        [StringLength(2)]
        public string clave { get; set; }

        [Display(Name = "¿Activo?")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(SCDM_cat_commodityMetadata))]
    public partial class SCDM_cat_commodity
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatCommodity
        {
            get
            {
                return string.Format("{0} - {1}", !string.IsNullOrEmpty(this.clave) ? clave : "-", descripcion);
            }
        }
    }
}