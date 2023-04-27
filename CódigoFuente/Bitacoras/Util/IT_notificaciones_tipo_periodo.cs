using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class IT_notificaciones_tipo_periodo
    {
        public const string DIAS = "DIAS";  
        public const string SEMANAS = "SEMANAS"; 
        public const string MESES = "MESES";      
        public const string ANIOS = "AÑOS";      


        public static string DescripcionStatus(String status)
        {
            switch (status)
            {
                case IT_notificaciones_tipo_periodo.DIAS:
                    return "Días";
                case IT_notificaciones_tipo_periodo.SEMANAS:
                    return "Semanas";
                case IT_notificaciones_tipo_periodo.MESES:
                    return "Meses";
                case IT_notificaciones_tipo_periodo.ANIOS:
                    return "Años";
                default:
                    return "No Disponible";
            }
        }
    }
}
