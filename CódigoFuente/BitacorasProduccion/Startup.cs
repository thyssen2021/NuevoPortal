using Hangfire.SqlServer;
using Hangfire;
using System;
using Owin;
using Hangfire.Console;
using Portal_2_0.Models.Auxiliares;

namespace IdentitySample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();

            ConfigureAuth(app);

            // --- INICIO: CÓDIGO DE HANGFIRE AGREGADO ---

            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                // Reemplaza "Portal_2_0Entities" si el nombre de tu connection string es diferente
                .UseSqlServerStorage("HangfireConnection", new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                })
                .UseConsole(); // <-- AGREGA ESTA LÍNEA PARA ACTIVAR LA CONSOLA

            // Inicia el servidor de Hangfire para que procese los trabajos
            app.UseHangfireServer();

            // 1. Creamos un objeto con las opciones para el dashboard
            var options = new DashboardOptions
            {
                // 2. Le decimos que use nuestro filtro personalizado para la autorización
                Authorization = new[] { new HangfireAuthorizationFilter() }
            };

            // 3. Aplicamos esas opciones al dashboard
            app.UseHangfireDashboard("/hangfire", options);
            // --- FIN: CÓDIGO DE HANGFIRE AGREGADO ---

        }
    }
}
