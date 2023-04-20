using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class IT_notificaciones_checklist_estatus
    {
        public const string PENDIENTE = "PENDIENTE";
        public const string EN_PROCESO = "EN PROCESO";
        public const string TERMINADO = "TERMINADO";
        public const string N_A = "NO APLICA";


        public static string DescripcionStatus(String status)
        {
            switch (status)
            {
                case IT_notificaciones_checklist_estatus.PENDIENTE:
                    return "Pendiente";
                case IT_notificaciones_checklist_estatus.EN_PROCESO:
                    return "En proceso";
                case IT_notificaciones_checklist_estatus.TERMINADO:
                    return "Terminado";
                case IT_notificaciones_checklist_estatus.N_A:
                    return "No aplica";
                default:
                    return "Pendiente";
            }
        }
    }
}
