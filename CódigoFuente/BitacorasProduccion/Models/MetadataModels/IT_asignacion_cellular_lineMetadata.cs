using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_asignacion_cellular_lineMetadata
    {
     
        [Display(Name = "Id")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Línea Celular")]
        public int id_inventory_cellular_line { get; set; }

        [Required]
        [Display(Name = "Empleado")]
        public int id_empleado { get; set; }

        [Required]
        [Display(Name = "Sistemas")]
        public int id_sistemas { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Asignación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_asignacion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Desvinculación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_desasignacion { get; set; }

        [Display(Name = "¿Es asignación Actual?")]
        public bool es_asignacion_actual { get; set; }


    }

    [MetadataType(typeof(IT_asignacion_cellular_lineMetadata))]
    public partial class IT_asignacion_cellular_line
    {

    }


}