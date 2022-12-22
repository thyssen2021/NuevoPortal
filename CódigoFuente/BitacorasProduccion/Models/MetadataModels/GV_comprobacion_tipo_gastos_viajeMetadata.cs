using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class GV_comprobacion_tipo_gastos_viajeMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Required]
        [StringLength(6)]
        [Display(Name = "Cuenta")]
        public string cuenta { get; set; }

        [Display(Name = "Estatus")]
        public bool activo { get; set; }

      

    }

    [MetadataType(typeof(GV_comprobacion_tipo_gastos_viajeMetadata))]
    public partial class GV_comprobacion_tipo_gastos_viaje
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatCuenta
        {
            get
            {
                return string.Format("{0} ({1})", descripcion, cuenta).ToUpper();
            }
        }
    }
}