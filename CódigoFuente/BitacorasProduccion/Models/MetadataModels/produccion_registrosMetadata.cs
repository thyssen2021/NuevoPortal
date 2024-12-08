using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class produccion_registrosMetadata
    {
        public int id { get; set; }
        [Display(Name = "Planta")]
        public Nullable<int> clave_planta { get; set; }
        [Display(Name = "Línea")]
        public Nullable<int> id_linea { get; set; }

        [Required]
        [Display(Name = "Operador")]
        public Nullable<int> id_operador { get; set; }

        [Required]
        [Display(Name = "Supervisor")]
        public Nullable<int> id_supervisor { get; set; }
        [Display(Name = "Turno")]
        public Nullable<int> id_turno { get; set; }

        [Required]
        [Display(Name = "SAP Platina")]
        public string sap_platina { get; set; }

        [RequiredIf("segunda_platina", true, ErrorMessage = "El campo SAP platina 2 es requerido")]
        [Display(Name = "SAP Platina 2")]
        public string sap_platina_2 { get; set; }


        [Required]
        [Display(Name = "SAP Rollo")]
        public string sap_rollo { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha { get; set; }
        [Display(Name = "Estado")]
        public Nullable<bool> activo { get; set; }
    }

    [MetadataType(typeof(produccion_registrosMetadata))]
    public partial class produccion_registros
    {
        [NotMapped]
        private Portal_2_0Entities db = new Portal_2_0Entities();


        [NotMapped]
        [Display(Name = "¿Segunda Platina?")]
        public bool segunda_platina { get; set; }
        

        //retorna el objeto mm y el cobjeto class asociado
        [NotMapped]
        public mm_v3 MM_asociado
        {
            get
            {
                mm_v3 mm = db.mm_v3.FirstOrDefault(x => x.Material == this.sap_platina);
                if (mm == null)
                    mm = new mm_v3 { };

                return mm;
            }
        }

        [NotMapped]
        public class_v3 Class_asociado
        {
            get
            {
                class_v3 class_ = db.class_v3.FirstOrDefault(x => x.Object == this.sap_platina);
                if (class_ == null)
                    class_ = new class_v3 { };

                return class_;
            }
        }

        //obtiene el numero de piezas donde aplica calculo
        public int TotalPiezasProduccion()
        {
            int cantidad = 0;

            var list = this.inspeccion_pieza_descarte_produccion.Where(x => x.inspeccion_fallas.aplica_en_calculo == true).ToList();
            cantidad = list.Sum(x => x.cantidad);

            return cantidad;

        }

        //obtiene el numero de pezas de descarte con daño interno
        public int NumPiezasDescarteDanoInterno()
        {
            int cantidad = 0;

            var list = this.inspeccion_pieza_descarte_produccion.Where(x => x.inspeccion_fallas.dano_interno == true && x.inspeccion_fallas.aplica_en_calculo == true).ToList();

            cantidad = list.Sum(x => x.cantidad);

            return cantidad;
        }

        //obtiene el numero de pezas de descarte con daño interno
        public int NumPiezasDescarteDanoExterno()
        {
            int cantidad = 0;

            var list = this.inspeccion_pieza_descarte_produccion.Where(x => x.inspeccion_fallas.dano_externo == true && x.inspeccion_fallas.aplica_en_calculo == true).ToList();

            cantidad = list.Sum(x => x.cantidad);

            return cantidad;

        }
    }

}