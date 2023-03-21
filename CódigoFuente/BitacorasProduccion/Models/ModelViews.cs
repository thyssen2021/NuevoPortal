﻿using System;
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

    /// <summary>
    /// Modelo para crear inspector de piezas de descarte
    /// </summary>
    public class AsignarSoftwareViewModel
    {

        public AsignarSoftwareViewModel()
        {
            this.IT_asignacion_software = new List<IT_asignacion_software>();
        }

        [Required]
        [Display(Name = "Empleado")]
        public int id_empleado { get; set; }

        [Required]
        [Display(Name = "Sistemas")]
        public int id_sistemas { get; set; }

        public List<IT_asignacion_software> IT_asignacion_software;


        public empleados EmpleadoUsuario;
        public empleados EmpleadoSistemas;

        
    }

    /// <summary>
    /// Modelo para el cambio de Jefe Directo
    /// </summary>
    public class CambioJefeViewModel
    {

        public CambioJefeViewModel()
        {
            
        }

        [Required]
        [Display(Name = "Jefe Actual")]
        public int id_jefe_actual { get; set; }

        [Required]
        [Display(Name = "Nuevo Jefe")]
        public int id_nuevo_jefe { get; set; }

        
        public empleados JefeActual;
        public empleados Nuevo;


    }
}