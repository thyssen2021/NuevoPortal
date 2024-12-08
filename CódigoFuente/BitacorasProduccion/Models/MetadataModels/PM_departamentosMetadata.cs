using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class PM_departamentosMetadata
    {
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Display(Name = "Jefe de Área")]
        public Nullable<int> id_empleado_jefe { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Departamento que valida")]
        public Nullable<int> id_departamento_validacion { get; set; }
        [Display(Name = "Estado")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(PM_departamentosMetadata))]
    public partial class PM_departamentos
    {

    }
}