using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_epoMetadata
    {

        [Display(Name = "Id")]
        public int id { get; set; }

        [Display(Name = "System Name")]
        public string system_name { get; set; }

        [Display(Name = "Username")]
        public string username { get; set; }
        [Display(Name = "Operating System")]
        public string operating_system { get; set; }
        [Display(Name = "Is laptop?")]
        public Nullable<bool> is_laptop { get; set; }
        [Display(Name = "Group Name")]
        public string group_name { get; set; }

        [Display(Name = "OS Type")]
        public string os_type { get; set; }
        [Display(Name = "Mac Address")]
        public string mac_address { get; set; }
        [Display(Name = "Numbers od CPUs")]
        public Nullable<int> numbers_of_cpus { get; set; }
        [Display(Name = "CPU Speed")]
        public Nullable<int> cpu_speed { get; set; }
        [Display(Name = "System manufacturer")]
        public string system_manufacturer { get; set; }
        [Display(Name = "System Model")]
        public string system_model { get; set; }
        [Display(Name = "System Serial Number")]
        public string system_serial_number { get; set; }
        [Display(Name = "Total Disk Space MB")]
        public Nullable<int> total_disk_space_mb { get; set; }
        [Display(Name = "Total C Drive Space MB")]
        public Nullable<int> total_c_drive_space_mb { get; set; }
        [Display(Name = "Last Communication")]
        public string last_communication { get; set; }
        [Display(Name = "Total Physical Memory MB")]
        public Nullable<double> total_physical_memory_mb { get; set; }
        [Display(Name = "Assignment Path")]
        public string assigment_path { get; set; }
    }

    [MetadataType(typeof(IT_epoMetadata))]
    public partial class IT_epo
    {

    }
}