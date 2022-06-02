using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_solicitud_usuariosMetadata
    {

        public int id { get; set; }

        [Display(Name = "Empleado")]
        [RequiredIf("no_encuentra_empleado", false, ErrorMessage ="El campo empleado es obligatorio. Si su nombre no se encuentra en la lista marque la casilla  \"NO estoy en la lista\" ")]
        public Nullable<int> id_empleado { get; set; }

        [RequiredIf("no_encuentra_empleado", true, ErrorMessage ="El campo planta es obligatorio")]
        [Display(Name = "Planta")]
        public Nullable<int> id_planta { get; set; }

        [Display(Name = "Fecha Solicitud")]
        public System.DateTime fecha_solicitud { get; set; }

        [Display(Name = "Fecha cierre")]
        public Nullable<System.DateTime> fecha_cierre { get; set; }

        [Display(Name = "Departamento")]
        [RequiredIf("no_encuentra_empleado", true, ErrorMessage ="El campo Departamento es obligatorio")]
        [MaxLength(50)]
        public string area { get; set; }

        [Display(Name = "Puesto")]
        [RequiredIf("no_encuentra_empleado", true, ErrorMessage = "El campo Puesto es obligatorio")]
        [MaxLength(50)]
        public string puesto { get; set; }

        [Display(Name = "8ID")]
        [StringLength(8, MinimumLength = 8)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Sólo se permiten números.")]
        public string C8ID { get; set; }

        [Display(Name = "Número empleado")]
        [MaxLength(100)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Sólo se permiten números.")]
        public string numero_empleado { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [MaxLength(100)]
        [Display(Name = "Correo")]
        public string correo { get; set; }

        [MaxLength(250)]
        [Display(Name = "Comentario")]
        public string comentario { get; set; }

        [Display(Name = "Estado")]
        public string estatus { get; set; }


        [RequiredIf("no_encuentra_empleado", true, ErrorMessage = "El campo Nombre es obligatorio")]
        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [RequiredIf("no_encuentra_empleado", true, ErrorMessage = "El campo Apellido Paterno es Obligatorio es obligatorio")]
        [Display(Name = "Apellido Paterno")]
        public string apellido1 { get; set; }
        
        [Display(Name = "Apellido Materno")]
        [RequiredIf("no_encuentra_empleado", true, ErrorMessage = "El campo Apellido Materno es Obligatorio es obligatorio")]
        public string apellido2 { get; set; }
    }

    [MetadataType(typeof(IT_solicitud_usuariosMetadata))]
    public partial class IT_solicitud_usuarios
    {

        [Display(Name = "NO estoy en la lista")]
        public bool no_encuentra_empleado { get; set; }
    }
}