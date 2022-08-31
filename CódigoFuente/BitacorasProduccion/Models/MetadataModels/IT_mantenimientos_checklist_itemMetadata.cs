using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_mantenimientos_checklist_itemMetadata
    {    
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Display(Name = "Categoría")]
        [Required]
        public int id_IT_mantenimientos_checklist_categorias { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Estatus")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(IT_mantenimientos_checklist_itemMetadata))]
    public partial class IT_mantenimientos_checklist_item
    {

    }
}