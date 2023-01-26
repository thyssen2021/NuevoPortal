using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class GV_comprobacion_rel_gastosMetadata
    {
        //dataAnnotations     
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Factura")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_factura { get; set; }
    }

    [MetadataType(typeof(GV_comprobacion_rel_gastosMetadata))]
    public partial class GV_comprobacion_rel_gastos
    {
        [NotMapped]
        public int orden
        {
            get
            {
                return Bitacoras.Util.GV_comprobacion_origen.GetOrden(this.concepto_tipo);
            }
        }
        [NotMapped]
        public decimal? TotalOtroGasto
        {
            get
            {
                return importe * (decimal?)tipo_cambio;
            }
        }

        [NotMapped]
        public HttpPostedFileBase PostedFileXML { get; set; }
        [NotMapped]
        public HttpPostedFileBase PostedFilePDF { get; set; }
        [NotMapped]
        public HttpPostedFileBase PostedFileExtranjero { get; set; }

        [NotMapped]
        public bool? ExisteEnCOFIDI { get; set; }


    }
}