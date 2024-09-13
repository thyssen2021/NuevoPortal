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

        [Display(Name = "Plant")]
        public string plant { get; set; }

        [Display(Name = "Storage Loc.")]
        public string storage_location { get; set; }

        [Display(Name = "Storage Bin")]
        public string storage_bin { get; set; }

        [Display(Name = "Batch")]
        public string batch { get; set; }

        [Display(Name = "Ship to")]
        public string ship_to_number { get; set; }

        [Display(Name = "Material")]
        public string material { get; set; }

        [Display(Name = "Mat. description")]
        public string material_description { get; set; }

        [Display(Name = "IHS number")]
        public string ihs_number { get; set; }

        [Display(Name = "Pieces")]
        public Nullable<int> pieces { get; set; }

        [Display(Name = "Unrestricted")]
        public Nullable<int> unrestricted { get; set; }

        [Display(Name = "Blocked")]
        public Nullable<int> blocked { get; set; }

        [Display(Name = "In Quality")]
        public Nullable<int> in_quality { get; set; }

        [Display(Name = "Value Stock")]
        public Nullable<double> value_stock { get; set; }

        [Display(Name = "Base Unit")]
        public string base_unit_measure { get; set; }

        [Display(Name = "Gauge")]
        public Nullable<double> gauge { get; set; }

        [Display(Name = "Gauge Min")]
        public Nullable<double> gauge_min { get; set; }

        [Display(Name = "Gauge Max")]
        public Nullable<double> gauge_max { get; set; }

        [Display(Name = "Altura")]
        public Nullable<double> altura { get; set; }

        [Display(Name = "Espesor")]
        public Nullable<double> espesor { get; set; }

        [Display(Name = "Núm. Tarima")]
        public Nullable<int> num_tarima { get; set; }

      
    }

    [MetadataType(typeof(CI_conteo_inventarioMetadata))]
    public partial class CI_conteo_inventario
    {
        public CI_conteo_inventario()
        {
            this.etiquetas = new List<CI_conteo_inventario>();
        }

        [NotMapped]
        [Display(Name = "Acciones")]
        public string acciones { get; set; }

        [NotMapped]
        [Display(Name = "Cantidad Teórica")]
        public double? cantidad_teorica
        {
            get
            {
                if (altura == null || gauge == null || gauge == 0)
                    return null;

                return Math.Round( altura.Value / gauge.Value, 2);
            }
        }
        [NotMapped]
        [Display(Name = "Diferencia SAP")]
        public double? diferencia_sap
        {
            get
            {
                using (var db = new Portal_2_0Entities())
                {
                    if (this.num_tarima != null && db.CI_conteo_inventario.Count(x => x.num_tarima == this.num_tarima) > 1)
                    {
                        if (altura > 0) {
                            var tarimas = db.CI_conteo_inventario.Where(x => x.num_tarima == num_tarima).ToList();
                            return tarimas.Sum(x => x.cantidad_teorica) - tarimas.Sum(x => x.pieces);
                        }
                            return 0;
                                              
                      
                    }
                    else
                    {
                        if (cantidad_teorica == null)
                            return null;

                        return cantidad_teorica - pieces;
                    }

                }

              
            }
        }
        [NotMapped]
        [Display(Name = "Total pzas MIN")]
        public double? total_piezas_min
        {
            get
            {
                if (pieces == null || gauge_max == null || gauge == null || gauge_max == 0)
                    return null;


                return  Math.Round ((pieces.Value * gauge.Value) / gauge_max.Value, 2);
            }
        }

        [NotMapped]
        [Display(Name = "Total pzas MAX")]
        public double? total_piezas_max
        {
            get
            {
                if (pieces == null || gauge_min == null || gauge == null || gauge_min == 0)
                    return null;

                return Math.Round((pieces.Value * gauge.Value) / gauge_min.Value,2);
            }
        }

        [NotMapped]
        [Display(Name = "Validación")]
        public string validacion
        {
            get
            {
                string result = "Ajustar";
                //valida si es multiple
                using (var db = new Portal_2_0Entities())
                {
                    if (this.num_tarima!=null && db.CI_conteo_inventario.Count(x => x.num_tarima == this.num_tarima) > 1)
                    {
                        var tarimas = db.CI_conteo_inventario.Where(x => x.num_tarima == num_tarima).ToList();
                        if (tarimas.Sum(x=>x.cantidad_teorica) >= tarimas.Sum(x => x.total_piezas_min) && tarimas.Sum(x => x.cantidad_teorica) <= tarimas.Sum(x => x.total_piezas_max))
                            return "(Múltiple) Dentro de tolerancias";
                        else
                            return "(Múltiple) Ajustar";
                    }
                    else {
                        if (altura == null)
                            return "Pendiente";
                        if (cantidad_teorica != null && cantidad_teorica >= total_piezas_min && cantidad_teorica <= total_piezas_max)
                            return "Dentro de tolerancias";
                        if (cantidad_teorica == null || total_piezas_min == null || total_piezas_max == null)
                            return "--";
                    }

                }

              


                return result;

            }
        }

        [NotMapped]
        public string ConcatEtiqueta
        {
            get
            {
                return string.Format("{0} * Storage Location: {1} * Storage Bin: {2} * {3} * Lote: {4} * Cantidad: {5}", plant, storage_location, storage_bin, material, batch, pieces);
            }
        }

        public List<CI_conteo_inventario> etiquetas { get; set; }

    }
}