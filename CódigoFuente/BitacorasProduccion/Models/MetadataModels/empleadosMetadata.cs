using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class empleadosMetadata
    {
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> nueva_fecha_nacimiento { get; set; }

        [Required]
        [Display(Name = "Planta")]
        public Nullable<int> planta_clave { get; set; }
        public Nullable<int> clave { get; set; }

        [Display(Name = "Estatus")]
        public Nullable<bool> activo { get; set; }

        //[Required(AllowEmptyStrings = false)]
        [StringLength(6, MinimumLength = 1)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Sólo se permiten números.")]
        [Display(Name = "Número de Empleado")]
        public string numeroEmpleado { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Apellido paterno")]
        public string apellido1 { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Apellido materno")]
        public string apellido2 { get; set; }
        public string nacimientoFecha { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string correo { get; set; }

        [Display(Name = "Teléfono")]
        public string telefono { get; set; }

        [Display(Name = "Extensión")]
        public string extension { get; set; }
        [Display(Name = "Celular")]
        public string celular { get; set; }
        [Display(Name = "Nivel")]
        public string nivel { get; set; }

        [Display(Name = "Puesto")]
        [Required]
        public Nullable<int> puesto { get; set; }

        [Display(Name = "Compañia")]
        public string compania { get; set; }

        //[Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Ingreso")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ingresoFecha { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Baja")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> bajaFecha { get; set; }

        [Display(Name = "8ID")]
        [StringLength(8, MinimumLength = 8)]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Sólo se permiten números.")]
        public string C8ID { get; set; }

        [Display(Name = "Área")]
        [Required]
        public Nullable<int> id_area { get; set; }

        [Required]
        [Display(Name = "Jefe Directo")]
        public Nullable<int> id_jefe_directo { get; set; }

        [Display(Name = "Fotografía")]
        public Nullable<int> id_fotografia { get; set; }

        [Display(Name = "Mostrar Teléfono")]
        public bool mostrar_telefono { get; set; }
    }

    [MetadataType(typeof(empleadosMetadata))]
    public partial class empleados
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatNombre
        {
            get
            {
                return string.Format("{0} {1} {2}", nombre, apellido1, apellido2).ToUpper();
            }
        }

        //concatena el número de empleado con el nombre
        [NotMapped]
        public string ConcatNumEmpleadoNombre
        {
            get
            {
                if (String.IsNullOrEmpty(this.numeroEmpleado))
                    return string.Format("{0} {1} {2}", nombre, apellido1, apellido2).ToUpper();
                else
                    return string.Format("({0}) {1} {2} {3}", numeroEmpleado, nombre, apellido1, apellido2).ToUpper();
            }
        }

        /// <summary>
        /// Obtiene todas las lineas activas para un empleado según sus asignaciones
        /// </summary>
        /// <returns></returns>
        public List<IT_inventory_cellular_line> GetIT_Inventory_Cellular_LinesActivas()
        {
            List<IT_inventory_cellular_line> lineas = new List<IT_inventory_cellular_line>();

            using (var db = new Portal_2_0Entities())
            {
                lineas = db.IT_asignacion_hardware.Where(x => x.es_asignacion_linea_actual == true
                && x.id_empleado == this.id
                && x.id_cellular_line.HasValue
                ).Select(x => x.IT_inventory_cellular_line).ToList();

                //elimina las lineas repetidas
                lineas = lineas.Distinct().ToList();

            }


            return lineas;
        }

        public string ImageToBase64()
        {
            string result = string.Empty;

            if (this.biblioteca_digital!=null) {
                string imageBase64 = Convert.ToBase64String(this.biblioteca_digital.Datos);
                result = string.Format("data:image/"+ (Path.GetExtension(biblioteca_digital.Nombre).Replace(".","")) + ";base64,{0}", imageBase64);
            }

            return result;
        }
        public string ImageToBase64(int newWidth, int newHeight)
        {
            string result = string.Empty;

            if (this.biblioteca_digital!=null) {
                //convirte los datos a un tipo Image
                Image imagen = Image.FromStream(new MemoryStream(this.biblioteca_digital.Datos));

                byte[] newData =Bitacoras.Util.ImageUtil.ResizeImage(imagen, newWidth, newHeight);
                
                string imageBase64 = Convert.ToBase64String(newData);


                result = string.Format("data:image/"+ (Path.GetExtension(biblioteca_digital.Nombre).Replace(".","")) + ";base64,{0}", imageBase64);
            }

            return result;
        }

        [NotMapped]
        public HttpPostedFileBase ArchivoImagen { get; set; }
    }
}