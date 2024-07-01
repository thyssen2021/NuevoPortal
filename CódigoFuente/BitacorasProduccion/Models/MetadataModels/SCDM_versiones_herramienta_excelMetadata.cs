using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_versiones_herramienta_excelMetadata
    {
        public int id { get; set; }

        [Display(Name = "Archivo")]
        public int id_archivo { get; set; }

        [Required]
        [Display(Name = "Responsable")]
        public int id_responsable { get; set; }
        
        [Required]
        [Display(Name = "Versión")]
        public string version { get; set; }

        [Required]
        [Display(Name = "Cambio")]
        [StringLength(150, MinimumLength = 1)]
        public string cambio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha del cambio")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_liberacion { get; set; }

    
    }

    [MetadataType(typeof(SCDM_versiones_herramienta_excelMetadata))]
    public partial class SCDM_versiones_herramienta_excel
    {
        [NotMapped]
        public HttpPostedFileBase PostedFileBase { get; set; }
    }
}