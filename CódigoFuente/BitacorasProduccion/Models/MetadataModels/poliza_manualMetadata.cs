using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class poliza_manualMetadata
    {
        [Display(Name = "# Póliza")]
        public int id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Tipo")]
        public int id_PM_tipo_poliza { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Moneda")]
        public string currency_iso { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Planta")]
        public int id_planta { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Elabora")]
        public int id_elaborador { get; set; }

        [Display(Name = "Valida")]
        [Required(ErrorMessage = "El usuario quien valida es obligatorio.", AllowEmptyStrings = false)]
        public Nullable<int> id_validador { get; set; }

        [Display(Name = "Autoriza")]
        public Nullable<int> id_autorizador { get; set; }

        [Display(Name = "Contabilidad")]
        [Required(ErrorMessage = "El usuario quien registrará en SAP es obligatorio.", AllowEmptyStrings = false)]
        public Nullable<int> id_contabilidad { get; set; }

        [Display(Name = "Dirección")]
        public Nullable<int> id_direccion { get; set; }

        [Display(Name = "Soporte de Póliza")]
        public Nullable<int> id_documento_soporte { get; set; }

        [Display(Name = "Soporte de Registro")]
        public Nullable<int> id_documento_registro { get; set; }

        [Display(Name = "# Documento SAP")]
        [StringLength(15)]
        public string numero_documento_sap { get; set; }

        [Display(Name = "Fecha de creación")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_creacion { get; set; }

        [Display(Name = "Fecha de documento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public System.DateTime fecha_documento { get; set; }

        [Display(Name = "Fecha de validación")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_validacion { get; set; }

        [Display(Name = "Fecha Autorización")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_autorizacion { get; set; }

        [Display(Name = "Fecha Dirección")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_direccion { get; set; }

        [Display(Name = "Fecha Registro en SAP")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_registro { get; set; }

        [Display(Name = "Comentario de rechazo")]
        [StringLength(355, MinimumLength = 5)]
        public string comentario_rechazo { get; set; }

        [StringLength(355, MinimumLength = 5)]
        [Display(Name = "Descripción de la póliza")]
        [Required(AllowEmptyStrings = false)]
        public string descripcion_poliza { get; set; }

        [Display(Name = "Estado")]
        public string estatus { get; set; }
    }

    [MetadataType(typeof(poliza_manualMetadata))]
    public partial class poliza_manual
    {
        //para el archivo de soporte de póliza
        [Display(Name = "Soporte de Póliza")]
        [RequiredIf("is_create", true, ErrorMessage = "Soporte de poliza es requerido")]
        [NotMapped]
        /* En caso de que se requiera que este soporte sea "no required": agregar un check que indique que se cuenta con soporte 
         y que al seleccionarlo aparezca el boton submit (utilizar foolproof con la anotación [requiredif("campoCheck")])
        */
        public HttpPostedFileBase PostedFileSoporte { get; set; }

        //Para foolproof
        [NotMapped]
        public bool is_create
        {
            get
            {
                return !(id > 0);
            }
        }
        [NotMapped]
        public HttpPostedFileBase PostedFileRegistro { get; set; }


        //obtiene el total de debe
        [NotMapped]
        public decimal totalDebe
        {
            get
            {
                decimal total = 0;

                foreach (PM_conceptos item in PM_conceptos)
                {
                    if (item.debe.HasValue)
                        total += item.debe.Value;
                }
                return total;
            }
        }

        //obtiene el total de haber
        [NotMapped]
        public decimal totalHaber
        {
            get
            {
                decimal total = 0;

                foreach (PM_conceptos item in PM_conceptos)
                {
                    if (item.haber.HasValue)
                        total += item.haber.Value;
                }
                return total;
            }
        }
    }
}