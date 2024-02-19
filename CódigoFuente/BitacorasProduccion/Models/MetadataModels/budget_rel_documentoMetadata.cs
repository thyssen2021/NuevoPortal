using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class budget_rel_documentoMetadata
    {

        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "FY CC")]
        public int id_budget_rel_fy_centro { get; set; }

        [Display(Name = "Cuenta SAP")]
        public int id_cuenta_sap { get; set; }

        [Display(Name = "ID documento")]
        public Nullable<int> id_documento { get; set; }

        [StringLength(180)]
        [Display(Name = "Comentario")]
        public string comentario { get; set; }
    }

    [MetadataType(typeof(budget_rel_documentoMetadata))]
    public partial class budget_rel_documento
    {
        [NotMapped]
        [Display(Name = "Archivo Soporte")]
        public HttpPostedFileBase ArchivoSoporte { get; set; }

    }
}