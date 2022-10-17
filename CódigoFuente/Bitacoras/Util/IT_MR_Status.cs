using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
   
    public static class IT_MR_Status
    {
        public const string CREADO = "CREADO";
        public const string ENVIADO_A_JEFE = "ENVIADO_A_JEFE";
        public const string ENVIADO_A_IT = "ENVIADO_A_IT";
        public const string RECHAZADO = "RECHAZADO";
        public const string FINALIZADO = "FINALIZADO";
        public const string EN_PROCESO = "EN_PROCESO";

        public static string DescripcionStatus(String status)
        {

            switch (status)
            {
                case IT_MR_Status.CREADO:
                    return "Creado";
                case IT_MR_Status.ENVIADO_A_JEFE:
                    return "Enviado a Jefe";
                case IT_MR_Status.ENVIADO_A_IT:
                    return "Enviado a IT";
                case IT_MR_Status.RECHAZADO:
                    return "Rechazado";
                case IT_MR_Status.FINALIZADO:
                    return "Finalizado";
                case IT_MR_Status.EN_PROCESO:
                    return "En proceso";
                default:
                    return "No Disponible";
            }
        }
    }
}
