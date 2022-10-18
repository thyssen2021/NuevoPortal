using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    
    public class IT_equipos_checklist_categoriasMetadata
    {

        [Display(Name = "ID")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        [MaxLength(50)]
        public string descripcion { get; set; }

        [Display(Name = "Estatus")]
        public bool activo { get; set; }

    }

    [MetadataType(typeof(IT_equipos_checklist_categoriasMetadata))]
    public partial class IT_equipos_checklist_categorias
    {

    }
}