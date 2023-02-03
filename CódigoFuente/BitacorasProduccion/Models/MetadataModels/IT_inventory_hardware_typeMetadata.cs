using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_inventory_hardware_typeMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        [Display(Name = "Hardware Name")]
        [MaxLength(40, ErrorMessage = "The max length for {0} is {1} characters.")]
        public string descripcion { get; set; }

        [Display(Name = "Active?")]
        public bool activo { get; set; }

        [Display(Name = "¿Puede Asignarse?")]
        public bool puede_asignarse { get; set; }

        [Display(Name = "¿Disponible en Matriz RH?")]
        public bool disponible_en_matriz_rh { get; set; }

        [Display(Name = "¿Aplica descripción (Matriz RH)?")]
        public bool aplica_descripcion { get; set; }
    }

    [MetadataType(typeof(IT_inventory_hardware_typeMetadata))]
    public partial class IT_inventory_hardware_type
    {
       

     
    }
}