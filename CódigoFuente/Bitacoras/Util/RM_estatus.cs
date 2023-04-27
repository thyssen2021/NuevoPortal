using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{

    //public static class RM_estatus
    //{


    //    public static string DescripcionStatus(String status)
    //    {

    //        switch (status)
    //        {
    //            case RM_estatus.CREADO:
    //                return "Creado";
    //            case RM_estatus.ENVIADO_A_JEFE:
    //                return "Enviado a Jefe";
    //            case RM_estatus.ENVIADO_A_IT:
    //                return "Enviado a IT";
    //            case RM_estatus.RECHAZADO:
    //                return "Rechazado";
    //            case RM_estatus.FINALIZADO:
    //                return "Finalizado";
    //            case RM_estatus.EN_PROCESO:
    //                return "En proceso";
    //            default:
    //                return "No Disponible";
    //        }
    //    }
    //}
    public enum RM_estatus_enum
    {
        Creada = 1,
        Editada = 2,
        Aprobada = 3,
        Regulariza = 4,
        Cancelada = 5,
        Impresa = 6,
    }
}
