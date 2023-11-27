using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class budget_cantidadMetadata
    {
     
    }

    [MetadataType(typeof(budget_cantidadMetadata))]
    public partial class budget_cantidad
    {        
        //para binding de ajax
        public string numero_cuenta_sap { get; set; }
    }
}