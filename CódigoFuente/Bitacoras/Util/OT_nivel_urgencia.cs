using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class OT_nivel_urgencia
    {
        public const string ALTA = "ALTA";
        public const string MEDIA = "MEDIA";
        public const string BAJA = "BAJA";


        public static string DescripcionStatus(String status)
        {

            switch (status)
            {
                case OT_nivel_urgencia.ALTA:
                    return "Alta";
                case OT_nivel_urgencia.MEDIA:
                    return "Media";
                case OT_nivel_urgencia.BAJA:
                    return "Baja";
                
                default:
                    return "No Disponible";
            }
        }


    }
}
