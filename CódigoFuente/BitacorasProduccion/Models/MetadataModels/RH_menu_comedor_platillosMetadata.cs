using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class RH_menu_comedor_platillosMetadata
    {
        public int id { get; set; }
        public int orden_display { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "Tipo de Platillo")]
        public string tipo_platillo { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(60, MinimumLength = 2)]
        [Display(Name = "Nombre de Platillo")]
        public string nombre_platillo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha { get; set; }

        [Display(Name = "Kcal")]
        [Required]
        [Range(0, 9999)]
        public Nullable<int> kcal { get; set; }


    }

    [MetadataType(typeof(RH_menu_comedor_platillosMetadata))]
    public partial class RH_menu_comedor_platillos
    {

    }
}