using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{

    public static class BG_IHS_tipo_demanda
    {
        public const string ORIGINAL = "ORIGINAL";
        public const string CUSTOMER = "CUSTOMER";


        public static string DescripcionStatus(String status)
        {
            switch (status)
            {
                case BG_IHS_tipo_demanda.ORIGINAL:
                    return "Original (IHS)";
                case BG_IHS_tipo_demanda.CUSTOMER:
                    return "Customer";
                default:
                    return "No Disponible";
            }
        }


    }
}
