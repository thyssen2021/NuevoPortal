using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    //Para formulario de carga del archivo excel
    public class ExcelViewModel
    {
        [Required(ErrorMessage = "Por favor seleccione un archivo.")]
     //   [RegularExpression(@"^.*\.(xls|xlsx|XLS|XLSX)$", ErrorMessage = "Sólo se permite seleccionar archivos Excel.")]
        [Display(Name = "Archivo Excel")]
        public HttpPostedFileBase PostedFile { get; set; }
    }

    /// <summary>
    /// Modelo para crear inspector de piezas de descarte
    /// </summary>
    public class UserInspectorViewModel
    {
        [Required]       
        [Display(Name = "Usuario")]
        public string id_usuario { get; set; }
    }
}