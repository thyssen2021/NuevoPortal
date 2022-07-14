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
        [Display(Name = "ID")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Plan Celular")]
        public int id_inventory_celullar_plan { get; set; }

        [Required]
        [Display(Name = "Planta")]
        public int id_planta { get; set; }

        [Required]
        [Display(Name = "Número Celular")]
        [MaxLength(15, ErrorMessage = "The max length for {0} is {1} characters.")]
        public string numero_celular { get; set; }

        [Display(Name = "Fecha de Corte")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_corte { get; set; }

        [Display(Name = "Estado")]
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
                    detalles += this.plantas.descripcion + " - ";

                if (this.IT_inventory_cellular_plans != null)
                    detalles += this.IT_inventory_cellular_plans.nombre_plan;


                return detalles;
            }
        }

        [NotMapped]
        public string GetPhoneNumberFormat
        {
            get
            {
                if (string.IsNullOrEmpty(this.numero_celular)) return string.Empty;

                if (!Int64.TryParse(this.numero_celular, out Int64 result))
                    return this.numero_celular;

                this.numero_celular = new System.Text.RegularExpressions.Regex(@"\D")
                    .Replace(this.numero_celular, string.Empty);

                // this.numero_celular = this.numero_celular.TrimStart('1');
                if (this.numero_celular.Length == 7)
                    return result.ToString("###-####");
                if (this.numero_celular.Length == 10)
                    return result.ToString("###-###-####");
                if (this.numero_celular.Length > 10)
                    return result
                        .ToString("(###)-###-#### " + new String('#', (this.numero_celular.Length - 10)));
                return this.numero_celular;
            }
        }

        /// <summary>
        /// Obtiene todas las lineas activas para un empleado según sus asignaciones
        /// </summary>
        /// <returns></returns>
        public IT_inventory_cellular_plans Get_Cellular_Plans(int id_plan)
        {
            IT_inventory_cellular_plans plan = new IT_inventory_cellular_plans();

            using (var db = new Portal_2_0Entities())
            {
                plan = db.IT_inventory_cellular_plans.Find(id_plan);
            }


            return plan;
        }


    }
}