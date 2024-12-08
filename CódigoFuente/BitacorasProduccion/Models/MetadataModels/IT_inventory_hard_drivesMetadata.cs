using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_inventory_hard_drivesMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "Inventory Item")]
        public int id_inventory_item { get; set; }

        [Display(Name = "Disk Name")]
        public string disk_name { get; set; }

        [Display(Name = "Total Drive Space (MB)")]
        public Nullable<int> total_drive_space_mb { get; set; }
        public string type_drive { get; set; }
    }

    [MetadataType(typeof(IT_inventory_hard_drivesMetadata))]
    public partial class IT_inventory_hard_drives
    {
        // para covertir megas a gb
        [Display(Name = "Total Drive Space (GB)")]
        public Nullable<decimal> total_drive_space_gb
        {
            get
            {
                decimal? val=null;

                if (this.total_drive_space_mb.HasValue)
                    val = this.total_drive_space_mb / (decimal)1024;
                
                if (val.HasValue)
                    return Decimal.Round(val.Value, 2);
                else
                    return null;
           }
            set
            {
                decimal? val = value *1024;

                if (val.HasValue)
                {
                    this.total_drive_space_mb = Decimal.ToInt32(val.Value);
                }
                else {
                    this.total_drive_space_mb = null;
                }               

            }
        }
    }
}