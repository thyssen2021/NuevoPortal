using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
   
    public static class OT_tipo_documento
    {
        public const string SOLICITUD = "SOLICITUD";
        public const string CIERRE = "CIERRE";  

        public static string DescripcionStatus(String status)
        {

            switch (status)
            {
                case OT_tipo_documento.SOLICITUD:
                    return "Solicitud";
                case OT_tipo_documento.CIERRE:
                    return "Cierre";            
                default:
                    return "No Disponible";
            }
        }


    }
}
