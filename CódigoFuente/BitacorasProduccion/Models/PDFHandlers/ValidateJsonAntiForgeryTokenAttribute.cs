using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Portal_2_0.Filters // Asegúrate de que este namespace coincida con tu proyecto
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateJsonAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var httpContext = filterContext.HttpContext;
            var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
            var headerToken = httpContext.Request.Headers["RequestVerificationToken"];

            //AntiForgery.Validate(cookie != null ? cookie.Value : null, headerToken);
            try
            {
                AntiForgery.Validate(cookie?.Value, headerToken);
            }
            catch (HttpAntiForgeryException)
            {
                // Si la validación falla, se produce una excepción
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Anti-forgery token not found or invalid.");
            }
        }
    }
}