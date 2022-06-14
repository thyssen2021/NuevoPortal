using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_inventory_cellular_plansMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "Equipo Celular")]
        public Nullable<int> id_it_inventory_items { get; set; }

        [MaxLength(120)]
        [Display(Name = "Razón Social")]
        public string razon_social { get; set; }

        [MaxLength(8)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Sólo se permiten números.")]
        [Display(Name = "Cuenta Padre")]
        public string cuenta_padre { get; set; }

        [Display(Name = "Cuenta hija")]
        [MaxLength(8)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Sólo se permiten números.")]
        public string cuenta_hija { get; set; }

        [Required]
        [Display(Name = "Núm. Teléfono")]
        [MaxLength(10)]
        //[RegularExpression("(^[0-9]+$)", ErrorMessage = "Sólo se permiten números.")]
        public string num_telefono { get; set; }

        [Display(Name = "Centro de Costo")]
        [MaxLength(4)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Sólo se permiten números.")]
        public string centro_costo { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de corte")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_corte { get; set; }

        [MaxLength(15)]
        [Display(Name = "Núm. factura")]
        public string numero_factura { get; set; }

        [Required]
        [MaxLength(120)]
        [Display(Name = "Nombre del Plan")]
        public string nombre_plan { get; set; }

        [Display(Name = "Servicios de Telecomunicaciones")]
        // [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Formato inválido para {0}. Utilice sólo dos decimales.")]
        [Range(0, 99999.99)]
        public Nullable<decimal> costo_servicios_telecomunicaciones { get; set; }

        [Display(Name = "Servicios y suscripciones")]
        //[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Formato inválido para {0}. Utilice sólo dos decimales.")]
        [Range(0, 99999.99)]
        public Nullable<decimal> costo_servicios_y_suscripciones { get; set; }

        [Display(Name = "Servicios y suscripciones de Terceros")]
        //[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Formato inválido para {0}. Utilice sólo dos decimales.")]
        [Range(0, 99999.99)]
        public Nullable<decimal> costo_servicios_y_suscripciones_terceros { get; set; }

        [Display(Name = "Equipo celular")]
        //[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Formato inválido para {0}. Utilice sólo dos decimales.")]
        [Range(0, 99999.99)]
        public Nullable<decimal> costo_equipo_celular { get; set; }

        [Display(Name = "Servicios Cobrados por cuenta y orden de Terceros")]
        //[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Formato inválido para {0}. Utilice sólo dos decimales.")]
        [Range(0, 99999.99)]
        public Nullable<decimal> costo_servicios_cobrados_terceros { get; set; }

        [Required]
        [Display(Name = "% IVA")]
        //[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Formato inválido para {0}. Utilice sólo dos decimales.")]
        [Range(0, 99.99)]
        public Nullable<decimal> porcentaje_iva { get; set; }

        [Display(Name = "Estado")]
        public bool activo { get; set; }

        [Display(Name = "Compañia telefónica")]
        [MaxLength(80)]
        [Required]
        public string nombre_compania { get; set; }

        [Display(Name = "Comentarios")]
        [MaxLength(250)]
        public string comentarios { get; set; }

    }

    [MetadataType(typeof(IT_inventory_cellular_plansMetadata))]
    public partial class IT_inventory_cellular_plans
    {
        [NotMapped]
        [Display(Name = "Costo Total (sin equipo)")]
        public decimal? CostoTotal  //no incluye equipo
        {
            get
            {
                decimal? cantidad = 0M;
                try
                {
                    cantidad = (costo_servicios_telecomunicaciones
                        + costo_servicios_y_suscripciones
                        + costo_servicios_cobrados_terceros
                        + costo_servicios_y_suscripciones_terceros)
                        * (1 + (porcentaje_iva / 100));
                    return cantidad;
                }
                catch
                {
                    return cantidad;
                }

            }
        }

        [NotMapped]
        [Display(Name = "Total con Equipo")]
        public decimal? CostoTotalConEquipo
        {
            get
            {
                decimal? cantidad = 0M;
                try
                {
                    cantidad = (costo_servicios_telecomunicaciones
                        + costo_servicios_y_suscripciones
                        + costo_servicios_cobrados_terceros
                        + costo_servicios_y_suscripciones_terceros
                        + costo_equipo_celular
                        )
                        * (1 + (porcentaje_iva / 100));
                    return cantidad;
                }
                catch
                {
                    return cantidad;
                }
            }
        }

        [NotMapped]
        [Display(Name = "IVA")]
        public decimal? IVA
        {
            get
            {
                decimal? cantidad = 0M;
                try
                {
                    cantidad = (costo_servicios_telecomunicaciones
                        + costo_servicios_y_suscripciones
                        + costo_servicios_cobrados_terceros
                        + costo_servicios_y_suscripciones_terceros
                        + costo_equipo_celular
                        )
                        * (porcentaje_iva / 100);
                    return cantidad;
                }
                catch
                {
                    return cantidad;
                }
            }
        }       
    }
}