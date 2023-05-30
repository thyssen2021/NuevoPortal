using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class IT_matriz_requerimientosMetadata
    {
        [Display(Name = "Folio")]
        public int id { get; set; }
        [Display(Name = "Empleado")]
        public int id_empleado { get; set; }

        [Display(Name = "Solicitante")]
        public int id_solicitante { get; set; }
        [Required]
        [Display(Name = "Jefe Directo")]
        public int id_jefe_directo { get; set; }
        [Display(Name = "Sistemas")]
        public Nullable<int> id_sistemas { get; set; }
        [Required]
        [Display(Name = "Acceso a Internet")]
        public int id_internet_tipo { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Solicitud")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_solicitud { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Aprobación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_aprobacion_jefe { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Cierre")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_cierre { get; set; }
        [Display(Name = "Estado")]
        public string estatus { get; set; }
        [StringLength(350, MinimumLength = 5)]
        [Display(Name = "Comentarios Adicionales")]
        public string comentario { get; set; }
        [StringLength(350, MinimumLength = 5)]
        [Display(Name = "Razón Rechazo")]
        public string comentario_rechazo { get; set; }

        [StringLength(350, MinimumLength = 5)]
        [Display(Name = "Comentario Cierre")]
        public string comentario_cierre { get; set; }


    }

    [MetadataType(typeof(IT_matriz_requerimientosMetadata))]
    public partial class IT_matriz_requerimientos
    {
        [NotMapped]
        [Display(Name = "Asignado A")]
        public string ultima_asignacion
        {
            get
            {
                string result = "SIN ASIGNAR";

                var last = this.IT_matriz_asignaciones.OrderByDescending(x => x.fecha_asignacion).FirstOrDefault();

                if (last != null && last.activo)
                    result = last.empleados.ConcatNombre;


                return result;
            }
        }
        
        [NotMapped]
        [Display(Name = "Última asiganación ID")]
        public int ultima_asignacion_id
        {
            get
            {
                int result = 0;

                var last = this.IT_matriz_asignaciones.OrderByDescending(x => x.fecha_asignacion).FirstOrDefault();

                if (last != null && last.activo)
                    result = last.id_sistemas;

                return result;
            }
        }

        [NotMapped]
        [Display(Name = "Asignado A")]
        public int id_asignacion { get; set; }

        [StringLength(350, MinimumLength = 2)]
        [Display(Name = "Comentario asignación")]
        public string comentario_asignacion { get; set; }

    }

    //modelo para formulario de cerrar sistemas
    public class IT_matriz_requerimientosCerrarModel
    {
        [Display(Name = "Folio")]
        public int id { get; set; }

        //para cierre de solicitud

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string correo { get; set; }


        [Display(Name = "8ID")]
        [StringLength(8, MinimumLength = 8)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Sólo se permiten números.")]
        public string C8ID { get; set; }

        [StringLength(350, MinimumLength = 5)]
        [Display(Name = "Comentario Cierre")]
        public string comentario_cierre { get; set; }

        [StringLength(350, MinimumLength = 5)]
        [Display(Name = "Razón Rechazo")]
        public string comentario_rechazo { get; set; }

        public virtual IT_matriz_requerimientos matriz { get; set; }
    }
}