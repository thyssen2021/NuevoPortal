using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_inventory_softwareMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        [Display(Name = "Software Name")]
        [MaxLength(50, ErrorMessage = "The max length for {0} is {1} characters.")]
        public string descripcion { get; set; }

        [Display(Name = "Active?")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(IT_inventory_softwareMetadata))]
    public partial class IT_inventory_software
    {
    }
}