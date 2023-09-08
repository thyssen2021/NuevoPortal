using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class Empleados_tipo
    {
        public const string EMPLEADO = "EMPLEADO";
        public const string PRACTICANTE = "PRACTICANTE";
        public const string PROVEEDOR = "PROVEEDOR";
        public static string Descripcion(String status)
        {

            switch (status)
            {
                case Empleados_tipo.EMPLEADO:
                    return "Empleado";
                case Empleados_tipo.PRACTICANTE:
                    return "Practicante";
                case Empleados_tipo.PROVEEDOR:
                    return "Proveedor Externo";
                default:
                    return "No Disponible";
            }
        }
    }
}
