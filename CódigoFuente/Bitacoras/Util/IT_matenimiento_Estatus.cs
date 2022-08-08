using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
   
    public static class IT_matenimiento_Estatus
    {
        public const string PROXIMO = "PROXIMO";
        public const string VENCIDO = "VENCIDO";
        public const string REALIZADO = "REALIZADO";
        public static string DescripcionStatus(String status)
        {

            switch (status)
            {
                case IT_matenimiento_Estatus.PROXIMO:
                    return "Próximo";
                case IT_matenimiento_Estatus.VENCIDO:
                    return "Vencido";
                case IT_matenimiento_Estatus.REALIZADO:
                    return "Realizado";
                default:
                    return "No Disponible";
            }
        }


    }
}
