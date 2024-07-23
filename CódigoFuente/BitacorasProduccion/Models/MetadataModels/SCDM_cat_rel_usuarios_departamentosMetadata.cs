using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_cat_rel_usuarios_departamentosMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }
        [Display(Name = "Departamento")]
        public int id_departamento { get; set; }
        [Display(Name = "Usuario")]
        public int id_empleado { get; set; }
        
        [Display(Name = "¿Activo?")]
        public bool activo { get; set; }
    }

    [MetadataType(typeof(SCDM_cat_rel_usuarios_departamentosMetadata))]
    public partial class SCDM_cat_rel_usuarios_departamentos
    {
        public string ConcatDepartamentoEmpleado
        {
            get
            {
                return string.Format("({0}) - {1}", this.SCDM_cat_departamentos_asignacion.descripcion, this.empleados.ConcatNombre);
            }
        }
    }
}