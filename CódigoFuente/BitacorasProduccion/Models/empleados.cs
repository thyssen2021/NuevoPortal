//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Portal_2_0.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class empleados
    {
        public int id { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> nueva_fecha_nacimiento { get; set; }

        [Required]
        [Display(Name = "Planta")]
        public Nullable<int> planta_clave { get; set; }


        public Nullable<int> clave { get; set; }

        [Display(Name = "Estatus")]
        public Nullable<bool> activo { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(6, MinimumLength = 1)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "S�lo se permiten n�meros.")]
        [Display(Name = "N�mero de Empleado")]
        public string numeroEmpleado { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Apellido paterno")]
        public string apellido1 { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Apellido materno")]
        public string apellido2 { get; set; }
        public string nacimientoFecha { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string correo { get; set; }

        [Display(Name = "Tel�fono")]
        public string telefono { get; set; }

        [Display(Name = "Extensi�n")]
        public string extension { get; set; }
        [Display(Name = "Celular")]
        public string celular { get; set; }
        [Display(Name = "Nivel")]
        public string nivel { get; set; }

        [Display(Name = "Puesto")]
        public Nullable<int> puesto { get; set; }

        [Display(Name = "Compa�ia")]
        public string compania { get; set; }

        [Display(Name = "8ID")]
        [StringLength(8, MinimumLength = 8)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "S�lo se permiten n�meros.")]
        public string C8ID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Ingreso")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ingresoFecha { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Baja")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> bajaFecha { get; set; }

        public virtual plantas plantas { get; set; }
        public virtual puesto puesto1 { get; set; }

       
    }
}
