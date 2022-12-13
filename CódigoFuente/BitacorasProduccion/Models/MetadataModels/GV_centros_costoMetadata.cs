using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class GV_centros_costoMetadata
    {

        [Display(Name = "ID")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Planta")]
        public int clave_planta { get; set; }

        [Required]
        [Display(Name = "Centro de Costo")]
        [MaxLength(4)]
        public string centro_costo { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        [Display(Name = "Departamento")]
        public string departamento { get; set; }

        [Display(Name = "Estatus")]
        public bool activo { get; set; }

    }

    [MetadataType(typeof(GV_centros_costoMetadata))]
    public partial class GV_centros_costo
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatCentroDepto
        {
            get
            {
                return string.Format("{0} ({1})", centro_costo, departamento).ToUpper();
            }
        }
    }
}