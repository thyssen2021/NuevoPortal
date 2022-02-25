using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class BG_Status
    {
        public const string ACTUAL = "ACTUAL";  //REAL
        public const string FORECAST = "FORECAST"; //PROYECCION
        public const string BUDGET = "BUDGET";      //PRESUPUESTO
    

        public static string DescripcionStatus(String status)
        {

            switch (status)
            {
                case BG_Status.ACTUAL:
                    return "Actual";
                case BG_Status.FORECAST:
                    return "Forecast";
                case BG_Status.BUDGET:
                    return "Budget";               
                default:
                    return "No Disponible";
            }
        }
    }
}
