using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class PFAMetadata
    {
        [Display(Name = "Number of PFA")]
        public int id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Planner/User")]
        public int id_solicitante { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Department")]
        public int id_PFA_Department { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Type of Volume")]
        public int id_PFA_volume { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Border/Port")]
        public int id_PFA_border_port { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "tkMM Destination Plant")]
        public int id_PFA_destination_plant { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Reason of PFA")]
        public int id_PFA_reason { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Type of Shipment")]
        public int id_PFA_type_shipment { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Responsible of the PF")]
        public int id_PFA_responsible_cost { get; set; }

        //[Required(AllowEmptyStrings = false)]
        [Display(Name = "How is Recovered Cost")]
        public int id_PFA_recovered_cost { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Authorizer")]
        public int id_PFA_autorizador { get; set; }

        [Display(Name = "Date of Request")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime date_request { get; set; }

        [StringLength(40, MinimumLength = 2)]
        [Display(Name = "SAP Part number")]
        [Required(AllowEmptyStrings = false)]
        public string sap_part_number { get; set; }

        [StringLength(40, MinimumLength = 2)]
        [Display(Name = "Customer Part Number")]
        // [Required(AllowEmptyStrings = false)]
        public string customer_part_number { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Ingrese un valor positivo")]
        [Display(Name = "Volume (mt/pcs) ")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Ingrese sólo dos decimales")]
        [Required]
        public decimal volume { get; set; }

        [StringLength(35, MinimumLength = 2)]
        [Display(Name = "Mill/Supplier of Steel")]
        [Required(AllowEmptyStrings = false)]
        public string mill { get; set; }

        [StringLength(40, MinimumLength = 2)]
        [Display(Name = "Customer")]
        [Required(AllowEmptyStrings = false)]
        public string customer { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Ingrese un valor positivo")]
        [Display(Name = "Total Original Cost usd/mt")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Ingrese sólo dos decimales")]
        [LessThan("total_pf_cost")]
        [Required]
        public decimal total_cost { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Ingrese un valor positivo")]
        [Required]
        [Display(Name = "Total PF Cost usd/mt")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Ingrese sólo dos decimales")]
        public decimal total_pf_cost { get; set; }

        //[Required]
        [Display(Name = "Promise Recovered Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime promise_recovering_date { get; set; }

        [StringLength(350, MinimumLength = 2)]
        [Display(Name = "Comentarios")]
        public string comentarios { get; set; }

        [StringLength(350)]
        [Display(Name = "Razon Rechazo")]
        public string razon_rechazo { get; set; }

        [StringLength(35)]
        [Display(Name = "Credit/debit note number")]
        public string credit_debit_note_number { get; set; }
        [Display(Name = "Status")]
        public string estatus { get; set; }

        [Display(Name = "Date of Approval")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_aprobacion { get; set; }

        [Display(Name = "Active")]
        public bool activo { get; set; }


        [RequiredIf("is_required_cost_is_accepted", true, ErrorMessage = "The field PFA cost is accepted/approved is Required")]
        [Display(Name = "PFA cost is accepted/approved")]
        public Nullable<bool> cost_is_accepted { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Attached Email")]
        public string email_support { get; set; }

        [Display(Name = "Support of acceptance")]
        public Nullable<int> id_document_support { get; set; }

        //[Required]
        [Display(Name = "Is recovered?")]
        public Nullable<bool> is_recovered { get; set; }
    }

    [MetadataType(typeof(PFAMetadata))]
    public partial class PFA
    {
        //Para el archivo de soporte
        [RequiredIfTrue("is_support_of_acceptance", ErrorMessage = "Support of acceptance is Required")]
        [Display(Name = "Support of acceptance")]
        [NotMapped]
        // [RegularExpression(@"^.*\.(xls|xlsx|XLS|XLSX|doc|DOC|docx|DOCX|pdf|PDF|png|PNG|jpg|JPG|jpeg|JPEG|rar|RAR|zip|ZIP|)$", ErrorMessage = "Seleccione una extensión válida.")]
        public HttpPostedFileBase PostedFile { get; set; }

        //CALCULA EL TotalCostToRecover
        [NotMapped]
        public decimal TotalCostToRecover
        {
            get
            {
                return total_pf_cost - total_cost;
            }
        }

        //Para foolproof
        [NotMapped]
        public bool is_required_cost_is_accepted
        {
            get
            {
                return id_PFA_responsible_cost > 1;
            }
        }
        [NotMapped]
        public bool is_support_of_acceptance
        {
            get
            {
                return cost_is_accepted != null && cost_is_accepted.Value && (id_document_support == null || id_document_support <= 0);
            }
        }
    }
}