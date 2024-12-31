using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class BG_ihs_vehicle_customMetadata
    {

        [Display(Name = "Clave")]
        public int Id { get; set; }

        [Display(Name = "Mnemonic-Vehicle/Plant")]
        public string MnemonicVehiclePlant { get; set; }

        [Display(Name = "Production Plant")]
        public string ProductionPlant { get; set; }

        [Display(Name = "Manufacturer Group")]
        public string ManufacturerGroup { get; set; }

        [Display(Name = "SOP (Start of Production)")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public System.DateTime sop_start_of_production { get; set; }

        [Display(Name = "EOP (End of Production)")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public System.DateTime eop_end_of_production { get; set; }

        [Display(Name = "Vehicle")]
        public string Vehicle { get; set; }

        [Display(Name = "Assembly Type")]
        public string AssemblyType { get; set; }

        [Display(Name = "Fecha Creación")]
        public Nullable<System.DateTime> CreatedAt { get; set; }

        [Display(Name = "Fecha Actualización")]
        public Nullable<System.DateTime> UpdatedAt { get; set; }


    }

    [MetadataType(typeof(BG_ihs_vehicle_customMetadata))]
    public partial class BG_ihs_vehicle_custom
    {
    }
}