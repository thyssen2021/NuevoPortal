using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{

    public class IT_asignacion_hardwareMetadata
    {
        [Display(Name = "Folio")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Hardware")]
        public int id_it_inventory_item { get; set; }

        [Display(Name = "Versión IATF")]
        public int id_iatf_version { get; set; }

        [Required]
        [Display(Name = "Empleado")]
        public int id_empleado { get; set; }

        [Required]
        [Display(Name = "Sistemas")]
        public int id_sistemas { get; set; }

        [Display(Name = "Responsiva")]
        public Nullable<int> id_biblioteca_digital { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Asignación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_asignacion { get; set; }

        [Display(Name = "¿Asignación Actual?")]
        public bool es_asignacion_actual { get; set; }

        [Display(Name = "Responsable Principal")]
        public Nullable<int> id_responsable_principal { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Desvinculación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_desasignacion { get; set; }

    }

    [MetadataType(typeof(IT_asignacion_hardwareMetadata))]
    public partial class IT_asignacion_hardware
    {
        [NotMapped]
        [Display(Name = "Documento Responsiva")]
        public HttpPostedFileBase PostedFileResponsiva { get; set; }

        /// <summary>
        /// Determina si la asignación es una asignación actual sin responsable principal
        /// </summary>
        /// <returns></returns>
        public bool TieneAsignacionValida() {

            bool result = false;
            using (var db = new Portal_2_0Entities()) {
                result = db.IT_asignacion_hardware.Any(x => x.id_it_inventory_item == this.id_it_inventory_item && x.es_asignacion_actual == true && x.id_empleado == x.id_responsable_principal);
            }

            return result;

        }

            

    }
}