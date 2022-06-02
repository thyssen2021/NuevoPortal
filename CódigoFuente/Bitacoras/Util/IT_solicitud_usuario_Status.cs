using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class IT_solicitud_usuario_Status
    {
        public const string CREADO = "CREADO";
        public const string CERRADO = "CERRADO";
        public static string DescripcionStatus(String status)
        {

            switch (status)
            {
                case IT_solicitud_usuario_Status.CREADO:
                    return "Pendiente";
                case IT_solicitud_usuario_Status.CERRADO:
                    return "Cerrado";
                default:
                    return "No Disponible";
            }
        }


    }
}
