using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class PFA_VolumeMetadata
    {
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Estatus")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(PFA_VolumeMetadata))]
    public partial class PFA_Volume
    {

    }
}