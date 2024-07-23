using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class clientesMetadata
    {

        [Display(Name = "ID")]
        public int clave { get; set; }

        [Display(Name = "¿Activo?")]
        public Nullable<bool> activo { get; set; }

        [Display(Name = "Clave SAP")]
        [StringLength(7)]
        public string claveSAP { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(100)]
        public string descripcion { get; set; }

        [Display(Name = "Código País")]
        [StringLength(2)]
        public string pais { get; set; }

        [Display(Name = "Dirección")]
        [StringLength(100)]
        public string direccion { get; set; }

        [Display(Name = "Ciudad")]
        [StringLength(120)]
        public string ciudad { get; set; }

        [Display(Name = "Código Postal")]
        [StringLength(5, MinimumLength = 5)]
        public string codigo_postal { get; set; }

        [Display(Name = "Calle")]
        [StringLength(120)]
        public string calle { get; set; }

        [Display(Name = "Código Estado")]
        [StringLength(6)]
        public string estado { get; set; }

    }

    [MetadataType(typeof(clientesMetadata))]
    public partial class clientes
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatClienteSAP
        {
            get
            {
                return string.Format("({0}) - {1}", !string.IsNullOrEmpty(claveSAP) ? claveSAP : "-", descripcion);
            }
        }
        public string ConcatDireccion
        {
            get
            {
                return string.Format("{0}, {1}, {2}, CP.:{3}, {4} "
                    , !string.IsNullOrEmpty(this.calle) ? calle : "-"
                    , !string.IsNullOrEmpty(this.ciudad) ? ciudad : "-"
                    , !string.IsNullOrEmpty(this.estado) ? estado : "-"
                    , !string.IsNullOrEmpty(this.codigo_postal) ? codigo_postal : "-"
                    , !string.IsNullOrEmpty(this.pais) ? pais : "-"
                    );
            }
        }
    }
}