using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class IT_MR_tipo
    {
        public const string CREACION = "CREACION";
        public const string MODIFICACION = "MODIFICACION";
        public const string BAJA = "BAJA";

        public static string DescripcionStatus(String status)
        {

            switch (status)
            {
                case IT_MR_tipo.CREACION:
                    return "Creación";
                case IT_MR_tipo.MODIFICACION:
                    return "Modificación";
                case IT_MR_tipo.BAJA:
                    return "Baja";
                default:
                    return "No Disponible";
            }
        }
    }
}
