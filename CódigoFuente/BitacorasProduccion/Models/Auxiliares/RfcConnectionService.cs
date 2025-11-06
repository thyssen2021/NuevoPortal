using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SAP.Middleware.Connector;

namespace Portal_2_0.Models
{
    public static class RfcConnectionService
    {
        // --- INICIO MODIFICACIÓN ---
        private static readonly RfcDestination _destination;
        private static readonly bool _isInitialized;
        private static readonly string _initializationError;

        /// <summary>
        /// Constructor estático: Se ejecuta una vez y es thread-safe.
        /// Aquí es donde DEBEN inicializarse los campos 'static readonly'.
        /// </summary>
        static RfcConnectionService()
        {
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                RfcConfigParameters connParams = new RfcConfigParameters
                {
                    { RfcConfigParameters.Name, appSettings["SapSystemName"] ?? "P60" },
                    { RfcConfigParameters.AppServerHost, appSettings["SapServer"] },
                    { RfcConfigParameters.SystemNumber, appSettings["SapSystemNumber"] },
                    { RfcConfigParameters.User, appSettings["SapUser"] },
                    { RfcConfigParameters.Password, appSettings["SapPassword"] },
                    { RfcConfigParameters.Client, appSettings["SapClient"] },
                    { RfcConfigParameters.Language, appSettings["SapLanguage"] ?? "EN" },
                    { RfcConfigParameters.SAPRouter, appSettings["SapRouter"] }
                };

                // Asignación directa al campo readonly
                _destination = RfcDestinationManager.GetDestination(connParams);
                _destination.Ping(); // Prueba la conexión al iniciar
                _isInitialized = true;
                _initializationError = null;

                // Log (si tienes un logger en tu web app)
                // LogWriter.LogInfo("[RfcConnectionService] Conexión SAP establecida.");
            }
            catch (Exception ex)
            {
                // Log (IMPORTANTE: Loguear el error de inicialización)
                // LogWriter.LogInfo($"<!!!> [RfcConnectionService] ERROR al inicializar conexión SAP: {ex.Message}");
                _isInitialized = false;
                _initializationError = ex.Message; // Almacenamos el error
            }
        }
        // --- FIN MODIFICACIÓN ---

        /// <summary>
        /// Obtiene la función RFC preconfigurada.
        /// </summary>
        public static IRfcFunction CreateFunction(string functionName)
        {
            // --- INICIO MODIFICACIÓN ---
            // Verifica si la inicialización falló y lanza un error claro.
            if (!_isInitialized || _destination == null)
            {
                throw new InvalidOperationException($"La conexión RFC de SAP no pudo ser inicializada. Error: {_initializationError}. Verifique el Web.config y la conectividad con SAP.");
            }
            // --- FIN MODIFICACIÓN ---

            return _destination.Repository.CreateFunction(functionName);
        }

        /// <summary>
        /// Ejecuta la función RFC de forma síncrona.
        /// (Se usa desde SapSyncService que ya está en un Task.Run)
        /// </summary>
        public static void InvokeFunction(IRfcFunction rfcFunction)
        {
            if (!_isInitialized || _destination == null)
            {
                throw new InvalidOperationException($"La conexión RFC de SAP no pudo ser inicializada. Error: {_initializationError}.");
            }

            rfcFunction.Invoke(_destination);
        }

        /// <summary>
        /// Ejecuta la función RFC de forma asíncrona.
        /// </summary>
        public static async Task InvokeFunctionAsync(IRfcFunction rfcFunction)
        {
            // --- INICIO MODIFICACIÓN ---
            // Asegura que la conexión esté inicializada antes de invocar
            if (!_isInitialized || _destination == null)
            {
                throw new InvalidOperationException($"La conexión RFC de SAP no pudo ser inicializada. Error: {_initializationError}. Verifique el Web.config y la conectividad con SAP.");
            }
            // --- FIN MODIFICACIÓN ---

            await Task.Run(() => rfcFunction.Invoke(_destination));
        }

        // --- INICIO MODIFICACIÓN ---
        // (El método InitializeDestination() y los campos _initLock/_isInitialized se eliminaron)
        // --- FIN MODIFICACIÓN ---
    }

}