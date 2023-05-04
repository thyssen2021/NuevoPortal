using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class clientesMetadata
    {

    }

    [MetadataType(typeof(clientesMetadata))]
    public partial class clientes
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatClienteSAP
        {
            get
            {
                return string.Format("({0}) - {1}", !string.IsNullOrEmpty(claveSAP) ? claveSAP : "-", descripcion);
            }
        }
    }
}