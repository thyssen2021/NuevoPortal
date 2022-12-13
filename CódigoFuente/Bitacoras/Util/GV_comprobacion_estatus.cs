using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class GV_comprobacion_estatus
    {
        public const string CREADO = "CREADO";
        public const string ENVIADO_A_JEFE = "ENVIADO_A_JEFE";
        public const string RECHAZADO_JEFE = "RECHAZADO_JEFE";
        public const string ENVIADO_CONTROLLING = "ENVIADO_CONTROLLING";
        public const string RECHAZADO_CONTROLLING = "RECHAZADO_CONTROLLING";
        public const string CONFIRMADO_NOMINA = "CONFIRMADO_NOMINA";
        public const string RECHAZADO_CONTABILIDAD = "RECHAZADO_CONTABILIDAD";
        public const string ENVIADO_NOMINA = "ENVIADO_NOMINA";
        public const string RECHAZADO_NOMINA = "RECHAZADO_NOMINA";
        public const string CONFIRMADO_CONTABILIDAD = "CONFIRMADO_CONTABILIDAD";
        public const string FINALIZADO = "FINALIZADO";


        public static string DescripcionStatus(String status)
        {

            switch (status)
            {
                case GV_comprobacion_estatus.CREADO:
                    return "Creado";
                case GV_comprobacion_estatus.ENVIADO_A_JEFE:
                    return "Enviado a Jefe";
                case GV_comprobacion_estatus.RECHAZADO_JEFE:
                    return "Rechazado por Jefe";
                case GV_comprobacion_estatus.ENVIADO_CONTROLLING:
                    return "Enviado a Controlling";
                case GV_comprobacion_estatus.RECHAZADO_CONTROLLING:
                    return "Rechazado Controlling";
                case GV_comprobacion_estatus.CONFIRMADO_NOMINA:
                    return "Confirmado por Nóminas";
                case GV_comprobacion_estatus.RECHAZADO_CONTABILIDAD:
                    return "Rechazado Cuentas por Pagar";
                case GV_comprobacion_estatus.ENVIADO_NOMINA:
                    return "Enviado a Nómina";
                case GV_comprobacion_estatus.RECHAZADO_NOMINA:
                    return "Rechazado Nómina";
                case GV_comprobacion_estatus.FINALIZADO:
                    return "Finalizado";
                case GV_comprobacion_estatus.CONFIRMADO_CONTABILIDAD:
                    return "Confirmado Cuentas por Pagar";           

                default:
                    return "No Disponible";
            }
        }
    }

}
