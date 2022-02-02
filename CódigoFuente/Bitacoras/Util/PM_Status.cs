using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class PM_Status
    {
        public const string CREADO = "CREADO";
        public const string ENVIADO_A_AREA = "ENVIADO_A_AREA";
        public const string RECHAZADO_VALIDADOR = "RECHAZADO_VALIDADOR";
        public const string RECHAZADO_AUTORIZADOR = "RECHAZADO_AUTORIZADOR";
        public const string VALIDADO_POR_AREA = "VALIDADO_POR_AREA";
        public const string ENVIADO_SEGUNDA_VALIDACION = "ENVIADO_SEGUNDA_VALIDACION";
        public const string AUTORIZADO_SEGUNDA_VALIDACION = "AUTORIZADO_SEGUNDA_VALIDACION";
        public const string ENVIADO_A_CONTABILIDAD = "ENVIADO_A_CONTABILIDAD";
        public const string FINALIZADO = "FINALIZADO";
        public const string ENVIADO_A_DIRECCION = "ENVIADO_A_DIRECCION";
        public const string RECHAZADO_DIRECCION = "RECHAZADO_DIRECCION";

        public static string DescripcionStatus(String status) {

            switch (status) {
                case PM_Status.CREADO:
                    return "Creado";
                case PM_Status.ENVIADO_A_AREA:
                    return "Enviado a Área";
                case PM_Status.RECHAZADO_VALIDADOR:
                    return "Rechazado por Área";
                case PM_Status.RECHAZADO_AUTORIZADOR:
                    return "Rechazado por Autorizador";
                case PM_Status.VALIDADO_POR_AREA:
                    return "Validado por Área";
                case PM_Status.ENVIADO_SEGUNDA_VALIDACION:
                    return "Enviado a Autorizador";
                case PM_Status.AUTORIZADO_SEGUNDA_VALIDACION:
                    return "Validado por Autorizador";
                case PM_Status.ENVIADO_A_CONTABILIDAD:
                    return "Enviado a Contabilidad";
                case PM_Status.FINALIZADO:
                    return "Registrado en SAP";
                case PM_Status.ENVIADO_A_DIRECCION:
                    return "Enviado a Dirección";
                case PM_Status.RECHAZADO_DIRECCION:
                    return "Rechazado por Dirección";
                default:
                    return "No Disponible";
            }
        }
    }        
    
}
