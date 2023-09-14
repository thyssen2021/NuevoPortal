using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace IdentitySample.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Nombre de Rol")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {

        //Aquí se agregan propiedades a la clase para gestionar usuarios
        [StringLength(40)]
        public string Nombre { get; set; }

        //[Required]
        //[StringLength(45)]
        //public string Apellidos { get; set; }

        [Display(Name = "Empleado")]
        public int IdEmpleado { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }

        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }

    public class EditMailUserViewModel
    {

        [Display(Name = "ID usuario")]
        public string IdUsuario { get; set; }

        [Display(Name = "Empleado")]
        public int IdEmpleado { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display (Name = "Nuevo correo")]
        public string nuevoCorreo { get; set; }

        [Display(Name = "¿Envia notificación?")]
        public bool enviaNotificacion { get; set; }

        public empleados empleado { get; set; }
    }
}