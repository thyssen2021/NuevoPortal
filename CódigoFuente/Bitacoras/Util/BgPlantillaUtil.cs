using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class BgPlantillaUtil
    {

        public static string DescripcionAnio(DateTime fecha)
        {
            DateTime fechaLimite = new DateTime(fecha.Year, 10, 1, 0, 0, 0);

            if (fecha >= fechaLimite)
                return fecha.Year + "/" + (fecha.Year + 1).ToString().Substring(2, 2);
            else
                return fecha.Year - 1 + "/" + (fecha.Year).ToString().Substring(2, 2);

        }
    }
}
