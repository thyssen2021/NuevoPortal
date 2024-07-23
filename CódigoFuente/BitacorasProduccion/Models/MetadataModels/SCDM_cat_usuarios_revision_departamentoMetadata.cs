using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_cat_usuarios_revision_departamentoMetadata
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "Usuario - Departamento")]
        public int id_rel_usuarios_departamentos { get; set; }

        [Display(Name = "Planta")]
        public int id_planta_solicitud { get; set; }

        [Display(Name = "Tipo")]
        public string tipo { get; set; }
        [Display(Name = "¿Activo?")]
        public bool activo { get; set; }
        [Display(Name = "¿Envía correo?")]
        public bool envia_correo { get; set; }



    }

    [MetadataType(typeof(SCDM_cat_usuarios_revision_departamentoMetadata))]
    public partial class SCDM_cat_usuarios_revision_departamento
    {
     
    }
}