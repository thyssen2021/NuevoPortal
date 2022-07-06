using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_inventory_cellular_lineMetadata
    {
        public int id { get; set; }
        public int id_inventory_celullar_plan { get; set; }
        public int id_planta { get; set; }

        [Display(Name = "Número Celular")]
        public string numero_celular { get; set; }
        public Nullable<System.DateTime> fecha_corte { get; set; }
        public bool activo { get; set; }
    }

    [MetadataType(typeof(IT_inventory_cellular_lineMetadata))]
    public partial class IT_inventory_cellular_line
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatDetalles
        {
            get
            {
                String detalles = String.Empty;

                detalles = "(" + this.numero_celular + ") ";

                if (this.plantas != null)
                    detalles += this.plantas.descripcion+" - ";

                if (this.IT_inventory_cellular_plans != null)
                    detalles += this.IT_inventory_cellular_plans.nombre_plan;


                return detalles;
            }
        }

    }
}