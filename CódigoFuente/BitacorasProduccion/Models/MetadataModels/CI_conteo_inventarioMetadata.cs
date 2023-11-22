using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class CI_conteo_inventarioMetadata
    {
        
        [Display(Name = "ID")]
        public int id { get; set; }
        [Display(Name = "Centro")]
        public string centro { get; set; }
        [Display(Name = "Almacén")]
        public string almacen { get; set; }
        [Display(Name = "Ubicación")]
        public string ubicacion { get; set; }
        [Display(Name = "Artículo")]
        public string articulo { get; set; }
        [Display(Name = "Material")]
        public string material { get; set; }
        [Display(Name = "Lote")]
        public string lote { get; set; }
        [Display(Name = "No. Bobina")]
        public string no_bobina { get; set; }
        [Display(Name = "SAP cantidad")]
        public Nullable<double> sap_cantidad { get; set; }
        [Display(Name = "Libre utilización")]
        public Nullable<double> libre_utilizacion { get; set; }
        [Display(Name = "Bloqueado")]
        public Nullable<double> bloqueado { get; set; }
        [Display(Name = "En control calidad")]
        public Nullable<double> control_calidad { get; set; }
        [Display(Name = "Unidad Base Medida")]
        public string unidad_base_medida { get; set; }
        [Display(Name = "Altura")]
        public Nullable<double> altura { get; set; }
        [Display(Name = "Espesor")]
        public Nullable<double> espesor { get; set; }
        [Display(Name = "Peso")]
        public Nullable<double> peso { get; set; }
    }

    [MetadataType(typeof(CI_conteo_inventarioMetadata))]
    public partial class CI_conteo_inventario
    {
        [NotMapped]
        [Display(Name = "Acciones")]
        public string acciones { get; set; }
    }
}