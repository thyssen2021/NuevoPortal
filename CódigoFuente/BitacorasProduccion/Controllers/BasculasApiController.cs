using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    public class BasculasApiController : Controller
    {
        // GET: BasculasApi/ObtenerPeso?idBascula=PUE-BLK1
        [HttpGet]
        [AllowAnonymous] // IMPORTANTE: Permite que SAP entre sin login de usuario
        public JsonResult ObtenerPeso(string idBascula)
        {
            string ipObjetivo = "";
            int puerto = 1702;
            string nombrePlanta = "Desconocida";
            string nombreLinea = "";
            string codigoPlanta = ""; // Ej: PUE

            // 1. ANÁLISIS DEL ID (PARSING)
            // Si el ID viene como "PUE-BLK1", lo partimos por el guion '-'
            if (!string.IsNullOrEmpty(idBascula))
            {
                var partes = idBascula.Split('-');
                if (partes.Length >= 2)
                {
                    codigoPlanta = partes[0].ToUpper(); // "PUE"
                    nombreLinea = partes[1].ToUpper();  // "BLK1"

                    // Asignamos el nombre completo de la planta según el prefijo
                    switch (codigoPlanta)
                    {
                        case "PUE": nombrePlanta = "Puebla"; break;
                        case "SIL": nombrePlanta = "Silao"; break;
                        case "SLP": nombrePlanta = "San Luis Potosí"; break;
                        default: nombrePlanta = "Planta Externa"; break;
                    }
                }
            }

            // 2. DICCIONARIO DE IPs (Tu lógica actualizada)
            switch (idBascula.ToUpper())
            {
                case "PUE-BLK1": ipObjetivo = "10.122.162.190"; break;
                case "PUE-BLK2": ipObjetivo = "10.122.162.191"; break;
                case "PUE-BLK3": ipObjetivo = "10.122.162.192"; break;
                case "SIL-BLK1": ipObjetivo = "10.121.24.190"; break;
                case "SIL-BLK2": ipObjetivo = "10.121.24.182"; break;
                case "SIL-BLK3": ipObjetivo = "10.121.24.192"; break;
                case "SLP-BLK1": ipObjetivo = "10.121.34.186"; break;
                default:
                    return Json(new { Message = "Error: ID de báscula no encontrado o no configurado." }, JsonRequestBehavior.AllowGet);
            }

            // 3. OBTENER PESO (Llamada al socket)
            // Usamos 'dynamic' para poder acceder a las propiedades (Message, Peso) fácilmente
            dynamic resultadoSocket = LeerPesoDesdeSocket(ipObjetivo, puerto);

            // 4. CONSTRUIR RESPUESTA FINAL COMPLETA
            // Aquí combinamos lo que nos dio el socket con los datos nuevos (Fecha, Planta, Linea)
            var respuestaFinal = new
            {
                Message = resultadoSocket.Message,
                Peso = resultadoSocket.Peso,      // Puede venir null si hubo error
                Unidad = "KG",                    // Asumimos KG, o puedes sacarlo del regex si viene
                Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Fecha formato ISO legible
                Planta = nombrePlanta,            // "Puebla"
                PlantaCodigo = codigoPlanta,      // "PUE"
                Linea = nombreLinea,              // "BLK1"
                BasculaID = idBascula.ToUpper(),  // "PUE-BLK1"
                BasculaIP = ipObjetivo            // IP real usada
            };

            return Json(respuestaFinal, JsonRequestBehavior.AllowGet);
        }

        private object LeerPesoDesdeSocket(string ip, int port)
        {
            byte[] msg = Encoding.UTF8.GetBytes("Portal"); // Handshake
            byte[] bytes = new byte[256];
            string patron = @"(?:- *)?\d+(?:\.\d+)?"; // Regex para encontrar números

            try
            {
                // Crear el socket
                using (Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);

                    sender.ReceiveTimeout = 5000; // 5 segundos de espera máximo (importante para no congelar a SAP)
                    sender.SendTimeout = 5000;

                    try
                    {
                        sender.Connect(remoteEP);

                        // Enviar señal
                        int bytesSent = sender.Send(msg);

                        // Recibir respuesta
                        int bytesRec = sender.Receive(bytes);

                        if (bytesRec > 0)
                        {
                            string respuestaRaw = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                            Regex regex = new Regex(patron);

                            foreach (Match m in regex.Matches(respuestaRaw))
                            {
                                // ÉXITO: Retornamos el peso limpio
                                return new { Message = "OK", Peso = m.Value, BasculaIP = ip };
                            }
                        }

                        sender.Shutdown(SocketShutdown.Both);
                        sender.Close();
                    }
                    catch (ArgumentNullException ane)
                    {
                        return new { Message = "Error Argumento: " + ane.Message };
                    }
                    catch (SocketException se)
                    {
                        return new { Message = "Error de Conexión (Socket): " + se.Message };
                    }
                    catch (Exception e)
                    {
                        return new { Message = "Error General: " + e.Message };
                    }
                }
            }
            catch (Exception e)
            {
                return new { Message = "Error Crítico: " + e.Message };
            }

            return new { Message = "Error: No se recibió respuesta válida de la báscula." };
        }
    }
}