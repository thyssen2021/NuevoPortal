using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
       public static class GV_tipo_viaje
    {
        public const string NACIONAL = "NACIONAL";  //REAL
        public const string EXTRANJERO = "EXTRANJERO"; //PROYECCION


        public static string DescripcionStatus(String status)
        {

            switch (status)
            {
                case GV_tipo_viaje.NACIONAL:
                    return "Nacional";
                case GV_tipo_viaje.EXTRANJERO:
                    return "Extranjero";             
                default:
                    return "No Disponible";
            }
        }
    }
}
