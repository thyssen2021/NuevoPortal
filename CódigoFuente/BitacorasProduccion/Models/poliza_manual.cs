//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Portal_2_0.Models
{
    using Foolproof;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    public partial class poliza_manual
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public poliza_manual()
        {
            this.PM_conceptos = new HashSet<PM_conceptos>();
            fecha_documento = DateTime.Now;
            currency_iso = "USD";
        }

        [Display(Name = "# P�liza")]
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
        [Required(ErrorMessage ="El usuario quien valida es obligatorio.", AllowEmptyStrings = false)]
        public Nullable<int> id_validador { get; set; }

        [Display(Name = "Autoriza")]
        public Nullable<int> id_autorizador { get; set; }

        [Display(Name = "Soporte de P�liza")]
        public Nullable<int> id_documento_soporte { get; set; }

        [Display(Name = "Soporte de Registro")]
        public Nullable<int> id_documento_registro { get; set; }

        [Display(Name = "# Documento SAP")]
        [StringLength(15)]
        public string numero_documento_sap { get; set; }

        [Display(Name = "Fecha de creaci�n")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_creacion { get; set; }

        [Display(Name = "Fecha de documento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public System.DateTime fecha_documento { get; set; }

        [Display(Name = "Fecha de validaci�n")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_validacion { get; set; }

        [Display(Name = "Fecha Autorizaci�n")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_autorizacion { get; set; }

        [Display(Name = "Comentario de rechazo")]
        [StringLength(355, MinimumLength = 5)]
        public string comentario_rechazo { get; set; }

        [StringLength(355, MinimumLength = 5)]
        [Display(Name = "Descripci�n de la p�liza")]
        [Required(AllowEmptyStrings = false)]
        public string descripcion_poliza { get; set; }

        [Display(Name = "Estado")]
        public string estatus { get; set; }

        //para el archivo de soporte de p�liza
        [Display(Name = "Soporte de P�liza")]
        [RequiredIf("is_create", true, ErrorMessage = "Soporte de poliza es requerido")]
        /* En caso de que se requiera que este soporte sea "no required": agregar un check que indique que se cuenta con soporte 
         y que al seleccionarlo aparezca el boton submit (utilizar foolproof con la anotaci�n [requiredif("campoCheck")])
        */
        public HttpPostedFileBase PostedFileSoporte { get; set; }

        //Para foolproof
        public bool is_create
        {
            get
            {
                return !(id > 0);
            }
        }

        //obtiene el total de debe
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

        public virtual biblioteca_digital biblioteca_digital { get; set; }
        public virtual biblioteca_digital biblioteca_digital1 { get; set; }
        public virtual currency currency { get; set; }
        public virtual empleados empleados { get; set; }
        public virtual plantas plantas { get; set; }
        public virtual PM_autorizadores PM_autorizadores { get; set; }
        public virtual PM_tipo_poliza PM_tipo_poliza { get; set; }
        public virtual PM_validadores PM_validadores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PM_conceptos> PM_conceptos { get; set; }
    }
}
