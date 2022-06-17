using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_inventory_cellular_plansMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

     
        [Required]
        [MaxLength(120)]
        [Display(Name = "Nombre del Plan")]
        public string nombre_plan { get; set; }

        [Display(Name = "Compañia telefónica")]
        [MaxLength(80)]
        [Required]
        public string nombre_compania { get; set; }


        [Display(Name = "Precio")]
        [Required]
        //[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Formato inválido para {0}. Utilice sólo dos decimales.")]
        [Range(0, 99999.99)]
        public Nullable<decimal> precio { get; set; }


        [Display(Name = "Comentarios")]
        [MaxLength(250)]
        public string comentarios { get; set; }



        [Display(Name = "Estado")]
        public bool activo { get; set; }

      


    }

    [MetadataType(typeof(IT_inventory_cellular_plansMetadata))]
    public partial class IT_inventory_cellular_plans
    {
       
    }
}