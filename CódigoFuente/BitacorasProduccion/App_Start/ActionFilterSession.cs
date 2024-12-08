using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Portal_2_0.App_Start
{
    public class ActionFilterSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;

            bool inicio = false;
            //obtiene la variable de sesion INICIO para saber si se trata de una nueva session.
            if (ctx.Session["Inicio"] != null)
            {
                inicio = (bool)ctx.Session["Inicio"];
            }

            // Validar la información que se encuentra en la sesión.
            if (inicio)
            {
                //una vez utilizada se cambia su valor a null
                ctx.Session["Inicio"] = null;

                //redirige al método de contolador
                //comentado para registro de bitácoras
                //filterContext.Result = new RedirectToRouteResult(
                //   new RouteValueDictionary {
                //   { "Controller", "Account" },
                //   { "Action", "CerrarSesion" } });

                System.Diagnostics.Debug.Print("Se ha cerrado Sessión!!!");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}