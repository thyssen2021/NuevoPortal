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
    using System.Web;

    public partial class orden_trabajo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public orden_trabajo()
        {
            this.OT_refacciones = new HashSet<OT_refacciones>();
        }

        [Display(Name = "Folio")]
        public int id { get; set; }

        [Display(Name = "Solicitante")]
        public int id_solicitante { get; set; }

        [Display(Name = "Asignaci�n")]
        public Nullable<int> id_asignacion { get; set; }

        [Display(Name = "Responsable")]
        public Nullable<int> id_responsable { get; set; }

        [Display(Name = "�rea")]
        public int id_area { get; set; }

        [Display(Name = "L�nea")]
        public Nullable<int> id_linea { get; set; }

        [Display(Name = "Documento Solicitud")]
        public Nullable<int> id_documento_solicitud { get; set; }

        [Display(Name = "Documento Cierre")]
        public Nullable<int> id_documento_cierre { get; set; }

        [Display(Name = "Fecha solicitud")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_solicitud { get; set; }

        [Display(Name = "Urgencia")]
        [StringLength(15)]
        [Required(AllowEmptyStrings = false)]
        public string nivel_urgencia { get; set; }

        [Display(Name = "T�tulo")]
        [StringLength(80, MinimumLength = 5)]
        [Required(AllowEmptyStrings = false)]
        public string titulo { get; set; }

        [Display(Name = "Descripci�n")]
        [StringLength(300, MinimumLength = 5)]
        [Required(AllowEmptyStrings = false)]
        public string descripcion { get; set; }

        [Display(Name = "Fecha Asignaci�n")]
        public Nullable<System.DateTime> fecha_asignacion { get; set; }

        [Display(Name = "Fecha en Proceso")]
        public Nullable<System.DateTime> fecha_en_proceso { get; set; }

        [Display(Name = "Fecha Cierre")]
        public Nullable<System.DateTime> fecha_cierre { get; set; }

        [Display(Name = "Estatus")]
        [Required(AllowEmptyStrings = false)]
        public string estatus { get; set; }

        [Display(Name = "Comentario Cierre")]
        [StringLength(300, MinimumLength = 3)]
        public string comentario { get; set; }

        public HttpPostedFileBase PostedFileSolicitud { get; set; }

        public virtual Area Area { get; set; }
        public virtual biblioteca_digital biblioteca_digital { get; set; }
        public virtual biblioteca_digital biblioteca_digital1 { get; set; }
        public virtual empleados empleados { get; set; }
        public virtual empleados empleados1 { get; set; }
        public virtual empleados empleados2 { get; set; }
        public virtual produccion_lineas produccion_lineas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OT_refacciones> OT_refacciones { get; set; }
    }
}
