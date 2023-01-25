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
        public const string COFIDI_TOTALES = "COFIDI_TOTALES";    //datos de cada concepto en la factura
        public const string XML_RESUMEN = "XML_RESUMEN";        //incluye los totales de una factura (xml subido)
        public const string XML_CONCEPTO = "XML_CONCEPTO";
        public const string XML_TOTALES = "XML_TOTALES";
        public const string GASTO_EXTRANJERO = "GASTO_EXTRANJERO";
        public const string GASTO_SIN_COMPROBANTE = "GASTO_SIN_COMPROBANTE";
        public const string COFIDI_CONCEPTO_CC = "COFIDI_CONCEPTO_CC";
        public const string XML_CONCEPTO_CC = "XML_CONCEPTO_CC";

        public static string DescripcionOrigen(String status)
        {

            switch (status)
            {
                case GV_comprobacion_origen.COFIDI_RESUMEN:
                    return "COFIDI";
                case GV_comprobacion_origen.XML_RESUMEN:
                    return "XML";
                case GV_comprobacion_origen.GASTO_EXTRANJERO:
                    return "GASTO EXTRANJERO";
                case GV_comprobacion_origen.GASTO_SIN_COMPROBANTE:
                    return "GASTO SIN COMPROBANTE";

                default:
                    return "No Disponible";
            }
        }
        public static int GetOrden(String status)
        {

            switch (status)
            {
                case GV_comprobacion_origen.COFIDI_RESUMEN:
                case GV_comprobacion_origen.XML_RESUMEN:
                    return 1;
                case GV_comprobacion_origen.COFIDI_CONCEPTO:
                case GV_comprobacion_origen.XML_CONCEPTO:
                    return 2;
                case GV_comprobacion_origen.XML_CONCEPTO_CC:
                case GV_comprobacion_origen.COFIDI_CONCEPTO_CC:
                    return 3;
                case GV_comprobacion_origen.COFIDI_TOTALES:
                case GV_comprobacion_origen.XML_TOTALES:
                    return 4;
                default:
                    return 0;

            }
        }
    }
}
