using Bitacoras.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{

    public class IT_mantenimientosMetadata
    {
        public int id { get; set; }

        [Display(Name = "Equipo")]
        public int id_it_inventory_item { get; set; }
        
        [Display(Name = "Usuario (Firma aceptación)")]
        public Nullable<int> id_empleado_responsable { get; set; }

        [Display(Name = "Sistemas")]
        public Nullable<int> id_empleado_sistemas { get; set; }
        public Nullable<int> id_iatf_version { get; set; }

        [Display(Name = "Documento de Aceptación")]
        public Nullable<int> id_biblioteca_digital { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Programada")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha_programada { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Realización")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha_realizacion { get; set; }

        [StringLength(300)]
        [Display(Name = "Comentarios Generales")]
        public string comentarios { get; set; }

    }

    [MetadataType(typeof(IT_mantenimientosMetadata))]
    public partial class IT_mantenimientos
    {
        // obtiene el estatus actual
        [NotMapped]
        [Display(Name = "Estatus")]
        public string estatus
        {
            get
            {
                if (this.fecha_realizacion.HasValue)
                    return IT_matenimiento_Estatus.REALIZADO;

                if (this.fecha_programada < DateTime.Now && this.fecha_realizacion == null)
                    return IT_matenimiento_Estatus.VENCIDO;

                if (this.fecha_programada > DateTime.Now && this.fecha_realizacion == null)
                    return IT_matenimiento_Estatus.PROXIMO;

                //valor por defecto
                return string.Empty;
            }

        }

        [NotMapped]
        [Display(Name = "¿Finalizar Mantenimiento?")]
        public bool finalizar_mantenimiento{get; set;}

        // obtiene el responsable principal
        [NotMapped]
        [Display(Name = "Responsable Principal")]
        public string responsable_principal
        {
            get
            {
                using (var db = new Portal_2_0Entities())
                {
                    var asignacion_principal = db.IT_asignacion_hardware_rel_items.Where(x => x.id_it_inventory_item == this.id_it_inventory_item && x.IT_asignacion_hardware.es_asignacion_actual == true && x.IT_asignacion_hardware.id_empleado == x.IT_asignacion_hardware.id_responsable_principal).FirstOrDefault();

                    if (asignacion_principal != null && asignacion_principal.IT_asignacion_hardware.empleados!=null)
                        return asignacion_principal.IT_asignacion_hardware.empleados.ConcatNombre;
                }

                //valor por defecto
                return "No asignado";
            }

        }



        // obtiene si es el prim mantenimiento
        [NotMapped]
        [Display(Name = "Primer mantenimiento")]
        public bool EsPrimerMantenimieento
        {
            get
            {
                using (var db = new Portal_2_0Entities())
                {
                    var asignacion_principal = db.IT_mantenimientos.Any(x => x.fecha_programada.Year == 2000 && x.id == this.id);
                     
                   return asignacion_principal;
                }
            }

        }

        // obtiene si es el prim mantenimiento
        [NotMapped]
        [Display(Name = "Fecha Programada Mes")]
        public string fecha_programada_texto
        {
            get
            {
                return this.fecha_programada.ToString("MMMM yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("es-MX"));
            }

        }

        [NotMapped]
        [Display(Name = "Documento de Aceptación")]
        public HttpPostedFileBase PostedFileDocumentoAceptacion { get; set; }
    }
}