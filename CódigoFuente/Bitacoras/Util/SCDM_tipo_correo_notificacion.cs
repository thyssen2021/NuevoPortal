using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public enum SCDM_tipo_correo_notificacionENUM
    {
        ENVIA_SOLICITUD = 1,                            //Recibe: SCDM o ventas                             OK
        APRUEBA_SOLICITUD_INICIAL = 2,                  //Recibe: Scdm                  N: Usuario          OK
        APRUEBA_SOLICITUD_DEPARTAMENTO_PENDIENTES = 3,  //Recibe: Scdm                  N: Usuario          OK
        APRUEBA_SOLICITUD_DEPARTAMENTO_FINAL = 4,       //Recibe: Scdm                  N: Usuario          OK
        RECHAZA_SOLICITUD_DEPARTAMENTO_A_SCDM = 5,      //Recibe: Scdm                                      OK
        RECHAZA_SOLICITUD_SCDM_A_SOLICITANTE = 6,       //Recibe: Solictante                                OK
        ASIGNACION_SOLICITUD_A_DEPARTAMENTO = 7,        //Recibe: Depto                 N: Usuario          OK
        RECORDATORIO = 8,                               //Recibe: Depto                                     OK
        FINALIZA_SOLICITUD = 9,                         //Recibe: Usuario  CC: SCDM
        NOTIFICACION_A_USUARIO = 10,                     //Recibe: Usuario                                  OK
        RECHAZA_SOLICITUD_INICIAL_A_SOLICITANTE = 11,   //Recibe: Solictante                                OK
        ASIGNACION_INCORRECTA = 12,   //Recibe: SCDM                                OK

    }
}
