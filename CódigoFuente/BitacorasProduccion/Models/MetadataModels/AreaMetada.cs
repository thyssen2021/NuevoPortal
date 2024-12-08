using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class AreaMetadata
    {
        [Display(Name = "Clave")]
        public int clave { get; set; }

        [Display(Name = "Estatus")]
        public bool activo { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(80, MinimumLength = 2)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(100, MinimumLength = 6)]
        [EmailAddress]
        [Display(Name = "Correo de área")]
        public string listaCorreoElectronico { get; set; }

        [Required]
        [Display(Name = "Planta")]
        public Nullable<int> plantaClave { get; set; }
    }

    [MetadataType(typeof(AreaMetadata))]
    public partial class Area
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatDeptoPlanta
        {
            get
            {
                string pt = String.Empty;

                if (this.plantas != null)
                    pt = plantas.descripcion;


                return string.Format("({0}) {1}", pt, descripcion).ToUpper();
            }
        }
    }
}


