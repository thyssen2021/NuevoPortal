using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
 
    public class IT_site_actividadesMetadata
    {
      
        [Display(Name = "ID")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        [MaxLength(120)]
        public string descripcion { get; set; }

        [Required]
        [Display(Name = "Referencia")]
        [MaxLength(120)]
        public string referencia { get; set; }

        [Display(Name = "Estatus")]
        public bool activo { get; set; }

    }

    [MetadataType(typeof(IT_site_actividadesMetadata))]
    public partial class IT_site_actividades
    {

    }
}