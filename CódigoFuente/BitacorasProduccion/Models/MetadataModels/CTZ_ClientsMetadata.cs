using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class CTZ_ClientsMetadata
    {

        [Display(Name = "Client ID")]
        public int ID_Cliente { get; set; }

        [StringLength(250, ErrorMessage = "Client Name cannot exceed 250 characters.")]
        [Display(Name = "Client Name")]
        public string Client_Name { get; set; }

        [StringLength(10, ErrorMessage = "SAP Key cannot exceed 10 characters.")]
        [Display(Name = "SAP Key")]
        public string Clave_SAP { get; set; }

        [Display(Name = "Country")]
        public string Pais { get; set; }

        [Display(Name = "Address")]
        public string Direccion { get; set; }

        [StringLength(120, ErrorMessage = "City cannot exceed 120 characters.")]
        [Display(Name = "City")]
        public string Ciudad { get; set; }

        [Display(Name = "Postal Code")]
        [StringLength(10, ErrorMessage = "Postal Code cannot exceed 10 characters.")]
        public string Codigo_Postal { get; set; }

        [StringLength(120, ErrorMessage = "Street cannot exceed 120 characters.")]
        [Display(Name = "Street")]
        public string Calle { get; set; }

        [StringLength(55, ErrorMessage = "State cannot exceed 55 characters.")]
        [Display(Name = "State")]
        public string Estado { get; set; }

        [Display(Name = "Automotive Industry")]
        public bool Automotriz { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

    }

    [MetadataType(typeof(CTZ_ClientsMetadata))]
    public partial class CTZ_Clients
    {
        [NotMapped]
        public string ConcatSAPName
        {
            get
            {
                if (string.IsNullOrEmpty(Clave_SAP))
                    return Client_Name;
                else
                    return string.Format("({0}) {1}", Clave_SAP, Client_Name).ToUpper();
            }
        }
    }
}