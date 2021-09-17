using Clases.DBUtil;
using Clases.Models;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.App_Start
{
    public class HandleError : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {

            Exception ex = filterContext.Exception;

            string claseError = "No disponible";

            if (ex != null && ex.TargetSite != null && ex.TargetSite.DeclaringType != null)
                claseError = ex.TargetSite.DeclaringType.FullName ?? "No disponible";


            //método
            string metodo = ex.TargetSite.Name ?? "No disponible";
            string source = "Clase: " + claseError + " Metodo = " + metodo;

            //Creamos y escribimos en el registro de eventos la excepción generada
            EntradaRegistroEvento evento = new EntradaRegistroEvento(0,
                DateTime.Now, getUsuario(),
                EntradaRegistroEvento.TipoEntradaRegistroEvento.Error,
                Clases.Util.UsoStrings.RecortaString(source, 200),
                 Clases.Util.UsoStrings.RecortaString(ex.ToString(), 4000),
                0,
                0
                );

            bool isController = false;
            //guarda en BD
            if (ex.StackTrace.Contains("ControllerActionInvoker"))
                isController = true;

            //Solo registra el error caso de que el error no provenga de un BaseControler
            if (!isController)
                ExcepcionesBDUtil.RegistraExcepcion(evento);

            base.OnException(filterContext);
        }

        private string getUsuario()
        {

            string userName = HttpContext.Current.User.Identity.Name;

            if (String.IsNullOrEmpty(userName))
                userName = "System";

            return userName;
        }
    }
}