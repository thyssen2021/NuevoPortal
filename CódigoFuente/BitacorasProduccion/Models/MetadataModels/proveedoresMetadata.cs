using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class proveedoresMetadata
    {

    }

    [MetadataType(typeof(proveedoresMetadata))]
    public partial class proveedores
    {
        //concatena el nombre
        [NotMapped]
        public string ConcatproveedoresAP
        {
            get
            {
                return string.Format("({0}) - {1}", !string.IsNullOrEmpty(claveSAP) ? claveSAP : "-", descripcion);
            }
        }
        public string ConcatDireccion
        {
            get
            {
                return string.Format("{0}, {1}, {2}, CP.:{3}, {4} "
                    , !string.IsNullOrEmpty(this.calle) ? calle : "-"
                    , !string.IsNullOrEmpty(this.ciudad) ? ciudad : "-"
                    , !string.IsNullOrEmpty(this.estado) ? estado : "-"
                    , !string.IsNullOrEmpty(this.codigo_postal) ? codigo_postal : "-"
                    , !string.IsNullOrEmpty(this.pais) ? pais : "-"
                    );
            }
        }
    }
}