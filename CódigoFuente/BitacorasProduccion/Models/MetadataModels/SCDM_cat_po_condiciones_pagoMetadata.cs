
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class SCDM_cat_po_condiciones_pagoMetadata
    {

    }

    [MetadataType(typeof(SCDM_cat_po_condiciones_pagoMetadata))]
    public partial class SCDM_cat_po_condiciones_pago
    {
        public string ConcatCodigoDescripcion
        {
            get
            {
                return string.Format("{0}-{1}", this.codigo, descripcion);
            }
        }
    }
}