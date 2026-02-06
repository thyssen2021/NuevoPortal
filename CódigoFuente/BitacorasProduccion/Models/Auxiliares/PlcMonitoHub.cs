using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNet.SignalR;

namespace Portal_2_0.Models.Auxiliares
{
    //CLASE PARA COMUNICACIÓN EN TIEMPO REAL ENTRE EL SERVIDOR Y LOS NAVEGADORES.
    public class PlcMonitorHub : Hub
    {
        // 1. MÉTODO PARA EL NAVEGADOR (UNIRSE a un grupo de plc)
        public async Task JoinPlcGroup(string plcId)
        {
            await Groups.Add(Context.ConnectionId, plcId);
        }

        // 2. MÉTODO PARA EL NAVEGADOR (SALIR de un grupo de plc)
        public async Task LeavePlcGroup(string plcId)
        {
            await Groups.Remove(Context.ConnectionId, plcId);
        }

        // 3. MÉTODO PARA EL SERVIDOR (ENVIAR datos a los clientes conectados a un plc específico)
        public void SendPlcData(string plcId, object data)
        {
            // "Clients.Group(plcId)" asegura que SOLO los usuarios viendo este PLC reciban el dato.
            // "receivePlcUpdate" es la función JS que crearemos en la vista más adelante.
            Clients.Group(plcId).receivePlcUpdate(data);
        }
    }
}