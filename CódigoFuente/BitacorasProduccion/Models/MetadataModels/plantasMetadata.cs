using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class plantasMetadata
    {
        [Display(Name = "Clave")]
        public int clave { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }
        [Display(Name = "Estatus")]
        public bool activo { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Código SAP")]
        [StringLength(4, MinimumLength = 2)]
        public string codigoSap { get; set; }


        //para formato tkmmnet
        [StringLength(80)]
        [Display(Name = "Calle (tkorgstreet)")]
        public string tkorgstreet { get; set; }

        [StringLength(5, MinimumLength = 5)]
        [Display(Name = "Código Postal (tkorgpostalcode)")]
        public string tkorgpostalcode { get; set; }
       
        [StringLength(80)]
        [Display(Name = "Municipio (tkorgpostaladdress)")]
        public string tkorgpostaladdress { get; set; }

        [StringLength(80)]
        [Display(Name = "Dirección (tkorgaddonaddr)")]
        public string tkorgaddonaddr { get; set; }
        
        [StringLength(30)]
        [Display(Name = "Estado (tkorgfedst)")]
        public string tkorgfedst { get; set; }

        [StringLength(30)]
        [Display(Name = "País (tkorgcountry)")]
        public string tkorgcountry { get; set; }

        [StringLength(3)]
        [Display(Name = "Código País (tkorgcountrykey)")]
        public string tkorgcountrykey { get; set; }

        [StringLength(80)]
        [Display(Name = "Ciudad (tkapsite)")]
        public string tkapsite { get; set; }
    }

    [MetadataType(typeof(plantasMetadata))]
    public partial class plantas
    {

    }
}