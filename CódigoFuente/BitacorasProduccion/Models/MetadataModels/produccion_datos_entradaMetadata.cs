using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class produccion_datos_entradaMetadata
    {
        public int id_produccion_registro { get; set; }
        [Required(ErrorMessage = "El Peso Real Neto es requerido", AllowEmptyStrings = false)]
        [Display(Name = "Peso Real Pieza Neto")]   //viene de la báscula
        public Nullable<double> peso_real_pieza_neto { get; set; }

        [Required(ErrorMessage = "El campo Orden SAP es requerido", AllowEmptyStrings = false)]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El campo {0}, debe tener una longitud de {1} carácteres")]
        [Display(Name = "Orden SAP")]
        public string orden_sap { get; set; }

        [RequiredIf("tiene_segunda_platina", true, ErrorMessage = "El campo {0} es requerido")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El campo {0}, debe tener una longitud de {1} carácteres")]
        [Display(Name = "Orden SAP 2")]
        public string orden_sap_2 { get; set; }
        [Display(Name = "Piezas por Golpe")]
        [Required(ErrorMessage = "El campo Piezas por Golpe es requerido")]
        [Range(1, 10)]
        public Nullable<int> piezas_por_golpe { get; set; }
        [StringLength(30, MinimumLength = 1)]
        [Display(Name = "Núm. Rollo")]
        public string numero_rollo { get; set; }

        [StringLength(10, MinimumLength = 1)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Sólo se permiten números.")]
        [Required(ErrorMessage = "El campo Lote de Rollo es requerido")]
        [Display(Name = "Lote Rollo")]
        public string lote_rollo { get; set; }
        [Display(Name = "Peso Etiqueta")]
        [Required(ErrorMessage = "El campo Peso Etiqueta es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "Ingrese un valor positivo")]
        public Nullable<double> peso_etiqueta { get; set; }

        //[Required(ErrorMessage = "El campo Peso Regreso Rollo es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "Ingrese un valor positivo")]
        [Display(Name = "Peso Regreso Rollo Real")]
        public Nullable<double> peso_regreso_rollo_real { get; set; }

        [Display(Name = "Peso Báscula Kgs")]
        //[Required(ErrorMessage = "El campo Peso Báscula Kgs es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "Ingrese un valor positivo")]
        public Nullable<double> peso_bascula_kgs { get; set; }

        [Display(Name = "Peso Despunte kgs")]
        //[Required(ErrorMessage = "El campo Peso Despunte es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "Ingrese un valor positivo")]
        public Nullable<double> peso_despunte_kgs { get; set; }

        //[Required(ErrorMessage = "El campo Peso Cola Kgs es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "Ingrese un valor positivo")]
        [Display(Name = "Peso Cola Kgs")]
        public Nullable<double> peso_cola_kgs { get; set; }

        [Display(Name = "Total Pieza Ajuste")]
        //[Required(ErrorMessage = "El campo Total Piezas Ajuste es requerido")]
        [Range(0, 1000, ErrorMessage = "Ingrese un valor positivo entre 1 y 1000")]
        public Nullable<int> total_piezas_ajuste { get; set; }

        [Display(Name = "Órdenes por Pieza")]
        [Required(ErrorMessage = "El campo Órdenes por Pieza es requerido")]
        [Range(1, 100, ErrorMessage = "Ingrese un valor positivo entre 1 y 100")]
        public Nullable<int> ordenes_por_pieza { get; set; }

        [Display(Name = "Comentarios")]
        [StringLength(600)]
        public string comentarios { get; set; }

        [RequiredIf("tiene_segunda_platina", true, ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Peso Real Pieza Neto (platina 2)")]   //viene de la báscula
        public Nullable<double> peso_real_pieza_neto_platina_2 { get; set; }


    }

    [MetadataType(typeof(produccion_datos_entradaMetadata))]
    public partial class produccion_datos_entrada
    {
        [NotMapped]
        private Portal_2_0Entities db = new Portal_2_0Entities();

        //calcula el peso de rollo usado
        [NotMapped]
        public double PesoRegresoRolloUsado
        {
            get
            {
                double pesoEtiqueta = 0;
                double pesoRegresoRolloReal = 0;

                if (this.peso_etiqueta.HasValue)
                    pesoEtiqueta = this.peso_etiqueta.Value;

                if (this.peso_regreso_rollo_real.HasValue)
                    pesoRegresoRolloReal = this.peso_regreso_rollo_real.Value;

                return pesoEtiqueta - pesoRegresoRolloReal;
            }
        }

        //Para foolproof
        [NotMapped]
        public bool tiene_segunda_platina
        {
            get
            {
                var p = db.produccion_registros.Find(this.id_produccion_registro);

                if (p == null)
                {
                    return false;
                }
                else {
                    return !String.IsNullOrEmpty(p.sap_platina_2);
                }
                
            }
        }

        //obtiene el numero de pezas de descarte con daño interno * Peso Real Pieza NEto

        public double TotalKgNGInterno()
        {

            produccion_registros produccion = db.produccion_registros.FirstOrDefault(x => x.id == this.id_produccion_registro);

            if (produccion != null && this.peso_real_pieza_neto.HasValue)
            {
                return this.peso_real_pieza_neto.Value * produccion.NumPiezasDescarteDanoInterno();
            }
            else
            {
                return 0;
            }

        }

        public double TotalKgNGExterno()
        {

            produccion_registros produccion = db.produccion_registros.FirstOrDefault(x => x.id == this.id_produccion_registro);

            if (produccion != null && this.peso_real_pieza_neto.HasValue)
            {
                return this.peso_real_pieza_neto.Value * produccion.NumPiezasDescarteDanoExterno();
            }
            else
            {
                return 0;
            }

        }

    }
}