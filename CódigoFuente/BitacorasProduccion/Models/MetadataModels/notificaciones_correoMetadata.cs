using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class notificaciones_correoMetadata
    {
        [Display(Name = "Clave")]
        public int id { get; set; }

        [Display(Name = "Empleado")]
        public Nullable<int> id_empleado { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Perfil")]
        public string descripcion { get; set; }

        [Display(Name = "Estado")]
        public bool activo { get; set; }

        [RequiredIf("is_required_email", true, ErrorMessage = "El campo correo es obligatorio si no se elige un empleado.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [MaxLength(70)]
        [Display(Name = "Correo")]
        public string correo { get; set; }

        [Display(Name = "Planta")]
        public Nullable<int> clave_planta { get; set; }

      
    }

    [MetadataType(typeof(notificaciones_correoMetadata))]
    public partial class notificaciones_correo
    {
        //Para foolproof
        [NotMapped]
        public bool is_required_email
        {
            get
            {
                return !(id_empleado >= 1);
            }
        }
    }
}