using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class OT_tipo_refaccion
    {
        public const string FALTANTE = "FALTANTE";
        public const string NECESARIA = "NECESARIA";

        public static string DescripcionTipo(String tipo)
        {

            switch (tipo)
            {
                case OT_tipo_refaccion.FALTANTE:
                    return "Faltante";
                case OT_tipo_refaccion.NECESARIA:
                    return "Necesaria";
               
                default:
                    return "No Disponible";
            }
        }
    }

   
}
