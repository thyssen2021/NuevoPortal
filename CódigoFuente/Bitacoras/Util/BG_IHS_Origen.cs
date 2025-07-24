using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class BG_IHS_Origen
    {
        public const string IHS = "IHS";
        public const string USER = "USER";
        public const string UNION = "UNION";


        public static string DescripcionStatus(String status)
        {
            switch (status)
            {
                case BG_IHS_Origen.IHS:
                    return "IHS";
                case BG_IHS_Origen.USER:
                    return "Usuario";
                default:
                    return "No Disponible";
            }
        }


    }

}
