using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_inventory_items_genericosMetadata
    {
        [Display(Name = "Id")]
        public int id { get; set; }

        [Required]
        [Display(Name = "Tipo de Hardware")]
        public int id_inventory_type { get; set; }

        //agregar anotaciones foolproof para cuando id_inventory_type == ACCESORIO, hacerlo obligatorio
        [Display(Name = "Tipo de Accesorio")]
        public Nullable<int> id_tipo_accesorio { get; set; }

        [Required]
        [MaxLength(80, ErrorMessage = "La longitud másima para {0} es de {1} carácteres.")]
        [Display(Name = "Marca")]
        public string brand { get; set; }

        [Required]
        [MaxLength(80, ErrorMessage = "La longitud másima para {0} es de {1} carácteres.")]
        [Display(Name = "Model")]
        public string model { get; set; }

        [MaxLength(250, ErrorMessage = "La longitud másima para {0} es de {1} carácteres.")]
        [Display(Name = "Comentarios")]
        public string comments { get; set; }

        [Display(Name = "¿Activo?")]
        public bool active { get; set; }

    }   

    [MetadataType(typeof(IT_inventory_items_genericosMetadata))]
    public partial class IT_inventory_items_genericos
    {
        [NotMapped]
        public string ConcatInfoGeneral
        {
            get
            {
                string info = String.Empty;
                try
                {

                    if (this.IT_inventory_hardware_type != null)
                        info += " (" + this.IT_inventory_hardware_type.descripcion + ") ";
                    if (!string.IsNullOrEmpty(this.brand))
                        info += " - " + brand + " ";
                    if (!string.IsNullOrEmpty(this.model))
                        info += " - " + model + " ";
                    if (!string.IsNullOrEmpty(this.comments))
                        info += " - [" +Clases.Util.UsoStrings.RecortaString(this.comments,100)+ "] ";

                    return info;
                }
                catch
                {
                    return info;
                }

            }
        }

        [NotMapped]
        public string ConcatAccesoriesInfo
        {
            get
            {
                //string info = "(" + id + ") ";
                string info = String.Empty;
                try
                {

                    if (this.IT_inventory_tipos_accesorios != null)
                        info += " (" + this.IT_inventory_tipos_accesorios.descripcion + ") ";
                  
                    if (!string.IsNullOrEmpty(this.brand))
                        info += " - " + brand + " ";
                    if (!string.IsNullOrEmpty(this.model))
                        info += " - " + model + " ";
                    if (!string.IsNullOrEmpty(this.comments))
                        info += " - [" + Clases.Util.UsoStrings.RecortaString(this.comments, 100) + "] ";

                    return info;
                }
                catch
                {
                    return info;
                }

            }
        }
    }
}