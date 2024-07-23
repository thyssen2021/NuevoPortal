using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_cat_molinoMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "Nombre del Molino")]
        [StringLength(30)]
        public string descripcion { get; set; }
        
        [Display(Name = "Clave SAP")]
        [StringLength(3)]
        public string clave { get; set; }

        [Display(Name = "¿Activo?")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(SCDM_cat_molinoMetadata))]
    public partial class SCDM_cat_molino
    {
        //public string ConcatMolinoSAP
        //{
        //    get
        //    {
        //        return string.Format("({0}) - {1}", !string.IsNullOrEmpty(this.) ? claveSAP : "-", descripcion);
        //    }
        //}
    }
}