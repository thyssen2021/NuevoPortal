using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{  

    public static class GV_tipo_departamento
    {
        public const string CUENTASPORPAGAR = "CUENTASPORPAGAR";
        public const string NOMINA = "NOMINA";
        public const string CONTROLLING = "CONTROLLING";
     
        public static string DescripcionStatus(String status)
        {
            switch (status)
            {
                case GV_tipo_departamento.CUENTASPORPAGAR:
                    return "Cuentas por Pagar";
                case GV_tipo_departamento.CONTROLLING:
                    return "Controlling";
                case GV_tipo_departamento.NOMINA:
                    return "Nómina";              

                default:
                    return "No Disponible"; 
            }
        }
    }
}
