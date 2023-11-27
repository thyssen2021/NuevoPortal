using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class budget_rel_conceptos_cantidadesMetadata
    {

    }

    [MetadataType(typeof(budget_rel_conceptos_cantidadesMetadata))]
    public partial class budget_rel_conceptos_cantidades
    {
        //para binding de ajax
        public string clave { get; set; }
        public string cuenta_sap { get; set; }
        public int id_rel_fy_centro { get; set; }
        public int mes { get; set; }
        public string currency { get; set; }
    }
}