using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_internet_tipoMetadata
    {
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(40, MinimumLength = 2)]
        [Display(Name = "Nombre")]
        public string descripcion { get; set; }       

        [Display(Name = "Estatus")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(IT_internet_tipoMetadata))]
    public partial class IT_internet_tipo
    {

    }
}