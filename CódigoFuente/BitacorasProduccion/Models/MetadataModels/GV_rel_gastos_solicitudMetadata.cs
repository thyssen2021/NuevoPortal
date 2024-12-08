using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{

    public class GV_rel_gastos_solicitudMetadata
    {
        public int id { get; set; }
        public int id_gv_solicitud { get; set; }
        public int id_tipo_gastos_viaje { get; set; }
        public decimal importe { get; set; }
        public string currency_iso { get; set; }
        public decimal cantidad { get; set; }
        public string comentarios { get; set; }
    }

    [MetadataType(typeof(GV_rel_gastos_solicitudMetadata))]
    public partial class GV_rel_gastos_solicitud
    {
        [NotMapped]
        [Display(Name = "Total")]
        public decimal total { 
            get {
                try
                {
                    decimal valor = importe * cantidad;
                    return valor;
                }
                catch (Exception) {
                    return 0;
                }
            }
        }
    }
}