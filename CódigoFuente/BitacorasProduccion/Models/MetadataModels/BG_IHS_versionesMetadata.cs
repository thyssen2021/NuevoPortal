using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{

    public class BG_IHS_versionesMetadataMetadata
    {

        [Display(Name = "Clave")]
        public int id { get; set; }

        [Display(Name = "Periodo")]
        public System.DateTime periodo { get; set; }

        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [Display(Name = "¿Activo?")]
        public bool activo { get; set; }

    }

    [MetadataType(typeof(BG_IHS_versionesMetadataMetadata))]
    public partial class BG_IHS_versiones
    {
        //concatena el número de empleado con el nombre
        [NotMapped]
        public string ConcatVersion
        {
            get
            {
                return string.Format("{0} ({1})", this.nombre, this.periodo.ToString("MMMM yyyy")).ToUpper();
            }
        }
        //concatena el número de empleado con el nombre
        [NotMapped]
        public string _Periodo
        {
            get
            {
                return string.Format("{0} {1}", this.periodo.ToString("MMMM"), this.periodo.ToString("yyyy")).ToUpper();
            }
        }

        [NotMapped]
        [Display(Name = "Archivo Excel")]
        public HttpPostedFileBase PostedFile { get; set; }

    }

}