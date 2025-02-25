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

        [StringLength(100)]
        [Display(Name = "Other Client")]
        public string Cliente_Otro { get; set; }

        [StringLength(100)]
        [Display(Name = "Other OEM")]
        public string OEM_Otro { get; set; }

        [StringLength(255)]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Display(Name = "Created Date")]
        public System.DateTime Creted_Date { get; set; }

        [Display(Name = "Updated Date")]
        public Nullable<System.DateTime> Update_Date { get; set; }

        [Display(Name = "Vehicle Type")]
        public Nullable<int> ID_VehicleType { get; set; }

        [Display(Name = "¿Import Required?")]
        public bool ImportRequired { get; set; }
    }

    [MetadataType(typeof(CTZ_ProjectsMetadata))]
    public partial class CTZ_Projects
    {

        //concatena el nombre
        [NotMapped]
        [Display(Name = "Quote ID")]
        public string ConcatQuoteID
        {
            get
            {
                // Obtiene el cliente, priorizando CTZ_Clients, de lo contrario Cliente_Otro, y recorta espacios
                string cliente = (CTZ_Clients?.Client_Name?.Trim() ?? Cliente_Otro?.Trim() ?? string.Empty);

                // Obtiene el OEM, priorizando CTZ_OEMClients, de lo contrario OEM_Otro, y recorta espacios
                string oem = (CTZ_OEMClients?.Client_Name?.Trim() ?? OEM_Otro?.Trim() ?? string.Empty);

                // Si el tipo de vehículo es diferente de Automotriz (ID_VehicleType distinto de 1),
                // se utiliza la descripción del vehículo en lugar del OEM.
                if (ID_VehicleType.HasValue && ID_VehicleType.Value != 1)
                {
                    oem = CTZ_Vehicle_Types?.VehicleType_Name?.Trim() ?? string.Empty;
                }

                // Obtiene la descripción de la planta y la clave del owner, recortadas
                string plant = CTZ_plants?.Description?.Trim() ?? string.Empty;
                string owner = CTZ_Material_Owner?.Owner_Key?.Trim() ?? string.Empty;

                // Concatenar los programas, ignorando nulos o vacíos y recortando espacios
                string concatPrograms = string.Empty;
                if (CTZ_Project_Materials != null && CTZ_Project_Materials.Any())
                {
                    concatPrograms = string.Join("_", CTZ_Project_Materials
                        .Select(x => x.Program_SP?.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Distinct());
                }

                // Construir el resultado sin guión final en caso de no haber programas
                string result = $"TKMM_{plant}_{owner}_{cliente}_{oem}";
                if (!string.IsNullOrEmpty(concatPrograms))
                {
                    result += "_" + concatPrograms;
                }

                result += "_V_" + LastedVersionNumber;

                return result.Replace(" ", "_").ToUpper();
            }
        }


        [NotMapped]
        [Display(Name = "Version")]
        public string LastedVersionNumber
        {
            get
            {
                string lastVersion = "0.1";

                if (this.CTZ_Projects_Versions.Any())
                {
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