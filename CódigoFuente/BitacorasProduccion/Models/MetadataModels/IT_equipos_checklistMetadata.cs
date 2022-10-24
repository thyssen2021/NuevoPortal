using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
  

    public class IT_equipos_checklistMetadata
    {
        [Display(Name = "Folio")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Elaboró")]
        public int id_sistemas { get; set; }

        [Required]
        [Display(Name = "Equipo")]
        public int id_inventory_item { get; set; }

        //[Required]
        //[Display(Name = "Nombre")]
        //[MaxLength(50)]
        //public string nombre { get; set; }

        //[Required]
        //[Display(Name = "Número de Serie")]
        //[MaxLength(50)]
        //public string numero_serie { get; set; }

        //[Required]
        //[Display(Name = "Modelo")]
        //[MaxLength(120)]
        //public string modelo { get; set; }

        //[Display(Name = "Sistema Operativo")]
        //[MaxLength(50)]
        //public string sistema_operativo { get; set; }

        [Display(Name = "Comentario General")]
        [MaxLength(200)]
        public string comentario_general { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha { get; set; }

        [Display(Name = "Estatus")]
        [MaxLength(20)]
        public string estatus { get; set; }
               

    }

    [MetadataType(typeof(IT_equipos_checklistMetadata))]
    public partial class IT_equipos_checklist
    {

    }
}