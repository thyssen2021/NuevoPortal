//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Portal_2_0.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Area
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Area()
        {
            this.puesto = new HashSet<puesto>();
        }

        [Display(Name = "Clave")]
        public int clave { get; set; }

        [Display(Name = "Estatus")]
        public bool activo { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(80, MinimumLength = 2)]
        [Display(Name = "Descripci�n")]
        public string descripcion { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(100, MinimumLength = 6)]
        [EmailAddress]
        [Display(Name = "Correo de �rea")]
        public string listaCorreoElectronico { get; set; }

        [Required]
        [Display(Name = "Planta")]
        public Nullable<int> plantaClave { get; set; }


        public virtual plantas plantas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<puesto> puesto { get; set; }
    }
}
