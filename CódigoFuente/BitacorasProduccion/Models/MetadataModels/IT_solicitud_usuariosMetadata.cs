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
        public Nullable<int> id_empleado { get; set; }

        [Display(Name = "Planta")]
        public Nullable<int> id_planta { get; set; }

        [Display(Name = "Fecha Solicitud")]
        public System.DateTime fecha_solicitud { get; set; }

        [Display(Name = "Fecha cierre")]
        public Nullable<System.DateTime> fecha_cierre { get; set; }

        [Display(Name = "Departamento")]
        public string area { get; set; }

        [Display(Name = "Puesto")]
        public string puesto { get; set; }

        [Display(Name = "8ID")]
        public string C8ID { get; set; }

        [Display(Name = "Correo")]
        public string correo { get; set; }

        [Display(Name = "Comentario")]
        public string comentario { get; set; }

        [Display(Name = "Estado")]
        public string estatus { get; set; }

        [RequiredIfTrue("no_encuentra_empleado", ErrorMessage = "Support of acceptance is Required")]
        [Display(Name = "Nombre")]
        public string nombre { get; set; }
        [Display(Name = "Apellido Paterno")]
        public string apellido1 { get; set; }
        [Display(Name = "Apellido Materno")]
        public string apellido2 { get; set; }
    }

    [MetadataType(typeof(IT_solicitud_usuariosMetadata))]
    public partial class IT_solicitud_usuarios
    {

        [Display(Name = "NO estoy en la lista")]
        public bool no_encuentra_empleado { get; set; }
    }
}