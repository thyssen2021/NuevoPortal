using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class GV_comprobacion_origen
    {        
        public const string COFIDI_RESUMEN = "COFIDI_RESUMEN";   //incluye los totales de una factura (obtenida en COFIDI)
        public const string COFIDI_CONCEPTO = "COFIDI_CONCEPTO";    //datos de cada concepto en la factura
        public const string XML_RESUMEN = "XML_RESUMEN";        //incluye los totales de una factura (xml subido)
        public const string XML_CONCEPTO = "XML_CONCEPTO";
        public const string GASTO_EXTRANJERO = "GASTO_EXTRANJERO";
        public const string GASTO_SIN_COMPROBANTE = "GASTO_SIN_COMPROBANTE";
    }        
}
