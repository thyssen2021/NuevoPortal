using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class CTZ_ProjectsMetadata
    {
      
        [Display(Name = "ID Project")]
        public int ID_Project { get; set; }

        [Display(Name = "Status")]
        public int ID_Status { get; set; }

        [Display(Name = "Client")]
        public Nullable<int> ID_Client { get; set; }

        [Display(Name = "OEM/Final Client")]
        public Nullable<int> ID_OEM { get; set; }

        [Display(Name = "Facility")]

        public int ID_Plant { get; set; }

        [Display(Name = "Created By")]

        public int ID_Created_By { get; set; }

        [Display(Name = "Updated By")]
        public Nullable<int> ID_Updated_By { get; set; }

        [Display(Name = "Material Owner")]
        public int ID_Material_Owner { get; set; }

        [Display(Name = "Other Client")]
        public string Cliente_Otro { get; set; }

        [Display(Name = "Other OEM")]
        public string OEM_Otro { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Display(Name = "Created Date")]
        public System.DateTime Creted_Date { get; set; }

        [Display(Name = "Updated Date")]
        public Nullable<System.DateTime> Update_Date { get; set; }
    }

    [MetadataType(typeof(CTZ_ProjectsMetadata))]
    public partial class CTZ_Projects
    {

        [NotMapped]
        [Display(Name = "Version")]
        public string LastedVersionNumber
        {
            get
            {
               string lastVersion = "0.1";

                if (this.CTZ_Projects_Versions.Any()) {
                    lastVersion = this.CTZ_Projects_Versions.OrderByDescending(v => v.ID_Version)
                       .FirstOrDefault().Version_Number;                
                }             

            return lastVersion;

            }
        }
        public static string GetNextVersionNumber(int projectId)
        {
            CTZ_Projects_Versions latestVersion = new CTZ_Projects_Versions();

            using (var db = new Portal_2_0Entities())
            {
                 latestVersion = db.CTZ_Projects_Versions
                     .Where(v => v.ID_Project == projectId)
                     .OrderByDescending(v => v.ID_Version)
                     .FirstOrDefault();
            }
                

            if (latestVersion == null)
            {
                return "0.1";
            }
            else if (latestVersion.Version_Number == "0.1")
            {
                return "1";
            }
            else
            {
                int ver;
                if (int.TryParse(latestVersion.Version_Number, out ver))
                {
                    return (ver + 1).ToString();
                }
                else
                {
                    // Por si hay algún error en la conversión.
                    return "1";
                }
            }
        }
    }
}