using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{ 
    public class IT_siteMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Planta")]
        public int id_planta { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Estatus")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(IT_siteMetadata))]
    public partial class IT_site
    {
        [NotMapped]
        public string ConcatSite
        {
            get
            {
                return string.Format("({0}) {1}", plantas.descripcion, descripcion).ToUpper();
            }
        }

    }
}