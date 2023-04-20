using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class IT_notificaciones_recordatorio_estatus
    {
        public const string PENDIENTE = "PENDIENTE";
        public const string EN_PROCESO = "EN PROCESO";
        public const string TERMINADO = "TERMINADO";


        public static string DescripcionStatus(String status)
        {
            switch (status)
            {
                case IT_notificaciones_recordatorio_estatus.PENDIENTE:
                    return "Pendiente";
                case IT_notificaciones_recordatorio_estatus.EN_PROCESO:
                    return "En proceso";
                case IT_notificaciones_recordatorio_estatus.TERMINADO:
                    return "Terminado";
                default:
                    return "Pendiente";
            }
        }
    }
}
