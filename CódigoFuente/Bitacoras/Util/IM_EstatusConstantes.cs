using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public class IM_EstatusConstantes
    {
        public const int RECIBIDA = 1;
        public const int FALTA_INFORMACION = 2;
        public const int NO_ACEPTADA = 3;
        public const int RECHAZADA = 4;
        public const int EN_PROCESO_IMPLEMENTACION = 6;
        public const int FINALIZADA_SIN_IMPLEMENTAR = 7;
        public const int IMPLEMENTADA = 8;
        public const int CREADA = 9;
        public const int ACEPTADA_POR_COMITE = 10;



        public static string DescripcionStatus(int status)
        {

            switch (status)
            {
                case IM_EstatusConstantes.RECIBIDA:
                    return "Recibida";
                case IM_EstatusConstantes.FALTA_INFORMACION:
                    return "Falta informacion / hay dudas";
                case IM_EstatusConstantes.NO_ACEPTADA:
                    return "No aceptada / no cumple con los lineamientos";
                case IM_EstatusConstantes.RECHAZADA:
                    return "Rechazada por comite";
                case IM_EstatusConstantes.EN_PROCESO_IMPLEMENTACION:
                    return "En proceso de implementación";
                case IM_EstatusConstantes.FINALIZADA_SIN_IMPLEMENTAR:
                    return "Finalizada sin implementación";
                case IM_EstatusConstantes.IMPLEMENTADA:
                    return "Implementada";
                case IM_EstatusConstantes.CREADA:
                    return "Creada";
                case IM_EstatusConstantes.ACEPTADA_POR_COMITE:
                    return "Aceptada por comite";
                default:
                    return "No Disponible";
            }
        }
    }
}
