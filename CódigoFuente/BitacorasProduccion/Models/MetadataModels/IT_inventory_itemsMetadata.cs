using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_inventory_itemsMetadata
    {
        [Display(Name = "Id")]
        public int id { get; set; }

        [Required(ErrorMessage ="The field {0} is required.")]
        [Display(Name = "Plant")]
        public int id_planta { get; set; }

        [Display(Name = "Type")]
        public int id_inventory_type { get; set; }

        [Display(Name = "Active?")]
        public bool active { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Purchase Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> purchase_date { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Inactive Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [RequiredIf("active", false, ErrorMessage = "The field {0} is Required.")]
        public Nullable<System.DateTime> inactive_date { get; set; }

        [MaxLength(250, ErrorMessage = "The max length for {0} is {1} characters.")]    
        [Display(Name = "Comments")]
        public string comments { get; set; }

        [MaxLength(50, ErrorMessage = "The max length for {0} is {1} characters.")]
        [Display(Name = "Hostname")]
        public string hostname { get; set; }

        [MaxLength(80, ErrorMessage = "The max length for {0} is {1} characters.")]
        [Display(Name = "Brand")]
        public string brand { get; set; }

        [MaxLength(80, ErrorMessage = "The max length for {0} is {1} characters.")]
        [Display(Name = "Model")]
        public string model { get; set; }

        [MaxLength(30, ErrorMessage = "The max length for {0} is {1} characters.")]
        [Display(Name = "Serial Number")]
        public string serial_number { get; set; }

        [Display(Name = "Warranty?")]
        public Nullable<bool> warranty { get; set; }

        [Display(Name = "Start Warranty")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [RequiredIf("warranty", true, ErrorMessage = "The field {0} is Required.")]
        public Nullable<System.DateTime> start_warranty { get; set; }

        [Display(Name = "End Warranty")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [RequiredIf("warranty", true, ErrorMessage = "The field {0} is Required.")]
        public Nullable<System.DateTime> end_warranty { get; set; }

        [Display(Name = "MAC LAN")]
        [RegularExpression("^(?:[0-9A-Fa-f]{2}[:-]){5}(?:[0-9A-Fa-f]{2})$", ErrorMessage = "Use the format 00:00:00:00:00:00")]
        public string mac_lan { get; set; }

        [Display(Name = "MAC WLAN")]
        [RegularExpression("^(?:[0-9A-Fa-f]{2}[:-]){5}(?:[0-9A-Fa-f]{2})$", ErrorMessage = "Use the format 00:00:00:00:00:00")]
        public string mac_wlan { get; set; }

        [Display(Name = "Processor")]
        [MaxLength(50, ErrorMessage = "The max length for {0} is {1} characters.")]
        public string processor { get; set; }

        [Display(Name = "Total Physical Memory (MB)")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public Nullable<int> total_physical_memory_mb { get; set; }

        [Display(Name = "Maintenance Period (months)")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public Nullable<int> maintenance_period_months { get; set; }

        [Display(Name = "Last Maintenance")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> last_maintenance { get; set; }

        [Display(Name = "Physical Status")]
        [MaxLength(150, ErrorMessage = "The max length for {0} is {1} characters.")]
        public string physical_status { get; set; }

        [Display(Name = "Is in operation?")]
        public Nullable<bool> is_in_operation { get; set; }

        [Display(Name = "CPU speed (MHz)")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public Nullable<int> cpu_speed_mhz { get; set; }

        [Display(Name = "Operation System")]
        [MaxLength(50, ErrorMessage = "The max length for {0} is {1} characters.")]
        public string operation_system { get; set; }

        [Display(Name = "OS bits")]
        [Range(32,64, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public Nullable<int> bits_operation_system { get; set; }

        [Display(Name = "Number of CPUs")]
        [Range(0, Int16.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public Nullable<int> number_of_cpus { get; set; }

        [Display(Name = "Printer Ubication")]
        [MaxLength(50, ErrorMessage = "The max length for {0} is {1} characters")]
        public string printer_ubication { get; set; }

        [Display(Name = "IP Address")]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "This is not a valid IP.")]
        public string ip_adress { get; set; }

        [Display(Name = "Cost Center")]
        [MaxLength(8, ErrorMessage = "The max length for {0} is {1} characters")]
        public string cost_center { get; set; }

        [Display(Name = "Storage (MB)")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public Nullable<int> movil_device_storage_mb { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$",ErrorMessage ="Invalid Format. Use only two decimals.")]
        [Range(0, 99.99)]
        [Display(Name = "Inches")]
        public Nullable<decimal> inches { get; set; }
    }

    [MetadataType(typeof(IT_inventory_itemsMetadata))]
    public partial class IT_inventory_items
    {
        
        [NotMapped]
        [Display(Name = "Number of hard drives")]
        public int NumberOfHardDrives
        {
            get
            {
                if (this.IT_inventory_hard_drives == null)
                    return 0;
                return this.IT_inventory_hard_drives.Count;
            }
        }

        [NotMapped]
        [Display(Name = "Total Disk Space (MB)")]
        public int? TotalDiskSpace
        {
            get
            {
                if (this.IT_inventory_hard_drives == null)
                    return 0;
                return this.IT_inventory_hard_drives.Sum(x=>x.total_drive_space_mb);
            }
        }

        [NotMapped]
        [Display(Name = "Total Free Disk Space (MB)")]
        public int? TotalFreeDiskSpace
        {
            get
            {
                if (this.IT_inventory_hard_drives == null)
                    return 0;
                return this.IT_inventory_hard_drives.Sum(x => x.free_drive_space_mb);
            }
        }
    }
}




