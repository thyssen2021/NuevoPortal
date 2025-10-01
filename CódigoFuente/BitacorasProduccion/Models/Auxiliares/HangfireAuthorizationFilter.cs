using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Clases.Util;
using Hangfire.Dashboard;
using Hangfire;


namespace Portal_2_0.Models.Auxiliares
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // Obtenemos el entorno OWIN del contexto de Hangfire.
            var owinEnvironment = context.GetOwinEnvironment();

            // Verificamos si el contexto de la petición web existe en el entorno.
            if (owinEnvironment.ContainsKey("System.Web.HttpContextBase"))
            {
                // Obtenemos el contexto como HttpContextBase, que es su tipo correcto.
                var httpContext = owinEnvironment["System.Web.HttpContextBase"] as HttpContextBase;

                // Si el contexto y el usuario existen, verificamos la autenticación y el rol.
                if (httpContext?.User != null && httpContext.User.Identity.IsAuthenticated)
                {
                    return httpContext.User.IsInRole(TipoRoles.IT_CATALOGOS);
                }
            }

            // Si algo falla o el usuario no cumple las condiciones, se deniega el acceso.
            return false;
        }
    }
}