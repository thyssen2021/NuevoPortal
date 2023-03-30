using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class puestoMetadata
    {
        [Display(Name = "Clave")]
        public int clave { get; set; }

        [Display(Name = "Estatus")]
        public bool activo { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(80, MinimumLength = 2)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "El campo área es requerido")]
        [Display(Name = "Área")]
        public Nullable<int> areaClave { get; set; }
    }

    [MetadataType(typeof(puestoMetadata))]
    public partial class puesto
    {
        [NotMapped]
        [Display(Name = "Shared Services")]
        public bool shared_services { get; set; }
    }
}