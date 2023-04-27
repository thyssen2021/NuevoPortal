using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class RM_almacenMetadata
    {

    }

    [MetadataType(typeof(RM_almacenMetadata))]
    public partial class RM_almacen
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatAlmacenPlanta
        {
            get
            {
                return string.Format("({0}){1}", this.plantas.descripcion, this.descripcion).ToUpper();
            }
        }
    }
}