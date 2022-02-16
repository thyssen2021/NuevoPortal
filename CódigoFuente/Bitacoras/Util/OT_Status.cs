using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class OT_Status
    {
        public const string ABIERTO = "ABIERTO";
        public const string ASIGNADO = "ASIGNADO";
        public const string EN_PROCESO = "EN_PROCESO";
        public const string CERRADO = "CERRADO";
      

        public static string DescripcionStatus(String status)
        {

            switch (status)
            {
                case OT_Status.ABIERTO:
                    return "Abierto";
                case OT_Status.ASIGNADO:
                    return "Asignado";
                case OT_Status.EN_PROCESO:
                    return "En Proceso";
                case OT_Status.CERRADO:
                    return "Cerrado";
                default:
                    return "No Disponible";
            }
        }
    }
}
