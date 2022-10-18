using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{ 
   
    public class IT_equipos_checklist_actividadesMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Categoria")]
        public int id_categoria_ck { get; set; }

        [Required]
        [MaxLength(120)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Estatus")]
        public bool activo { get; set; }             

    }

    [MetadataType(typeof(IT_equipos_checklist_actividadesMetadata))]
    public partial class IT_equipos_checklist_actividades
    {
       

    }
}