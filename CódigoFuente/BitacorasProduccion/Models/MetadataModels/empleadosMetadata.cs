﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class empleadosMetadata
    {
        [Required]
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
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Sólo se permiten números.")]
        [Display(Name = "Número de Empleado")]
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
        [Display(Name = "Apellido materno")]
        public string apellido2 { get; set; }
        public string nacimientoFecha { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string correo { get; set; }

        [Display(Name = "Teléfono")]
        public string telefono { get; set; }

        [Display(Name = "Extensión")]
        public string extension { get; set; }
        [Display(Name = "Celular")]
        public string celular { get; set; }
        [Display(Name = "Nivel")]
        public string nivel { get; set; }

        [Display(Name = "Puesto")]
        [Required]
        public Nullable<int> puesto { get; set; }

        [Display(Name = "Compañia")]
        public string compania { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Ingreso")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ingresoFecha { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Baja")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> bajaFecha { get; set; }

        [Display(Name = "8ID")]
        [StringLength(8, MinimumLength = 8)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Sólo se permiten números.")]
        public string C8ID { get; set; }

        [Display(Name = "Área")]
        [Required]
        public Nullable<int> id_area { get; set; }
    }

    [MetadataType(typeof(empleadosMetadata))]
    public partial class empleados
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatNombre
        {
            get
            {
                return string.Format("{0} {1} {2}", nombre, apellido1, apellido2).ToUpper();
            }
        }

        //concatena el número de empleado con el nombre
        [NotMapped]
        public string ConcatNumEmpleadoNombre
        {
            get
            {
                return string.Format("({0}) {1} {2} {3}", numeroEmpleado, nombre, apellido1, apellido2).ToUpper();
            }
        }
    }
}