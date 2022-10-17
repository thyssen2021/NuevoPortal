using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public static class IT_site_checklist_estatus
    {
        public const string INICIADO = "INICIADO";
        public const string EN_PROCESO = "EN_PROCESO";
        public const string FINALIZADO = "FINALIZADO";
   
        public static string DescripcionStatus(String status)
        {

            switch (status)
            {
                case IT_site_checklist_estatus.INICIADO:
                    return "Iniciado";
                case IT_site_checklist_estatus.EN_PROCESO:
                    return "En proceso";
                case IT_site_checklist_estatus.FINALIZADO:
                    return "Finalizado";           
                default:
                    return "No Disponible";
            }
        }
    }

}
