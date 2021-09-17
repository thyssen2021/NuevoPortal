using Clases.Models;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Clases.DBUtil;

namespace Portal_2_0.Models
{
    public class BaseController:Controller
    {

        public const string MENSAJE_ERROR_PERMISOS = "No se encuentra asignado a este proyecto, solicite los permisos necesarios con el creador del proyecto.";
        public const string MENSAJE_ERROR_DANTE = "Para acceder a este recurso es necesario cargar los datos de Dante del proyecto";
        public ApplicationUserManager _userManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public ApplicationRoleManager _roleManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }

        }

        public bool TieneRol(String rol)
        {
            if (_userManager.IsInRoleAsync(User.Identity.GetUserId(), rol).Result)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Evento disparado cuando no se administra una excepción adecuadamente
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {


            //Obtenemos el origen del error
            string controllerName = (string)filterContext.RouteData.Values["controller"];
            string actionName = (string)filterContext.RouteData.Values["action"];
            string source = controllerName + "." + actionName;

            //Creamos y escribimos en el registro de eventos la excepción generada
            EntradaRegistroEvento evento = new EntradaRegistroEvento(0,
                DateTime.Now, getUsuario(),
                EntradaRegistroEvento.TipoEntradaRegistroEvento.Error,
                UsoStrings.RecortaString(source, 200),
                UsoStrings.RecortaString(filterContext.Exception.ToString(), 4000),
                0,
                0
                );

            //guarda en BD
            ExcepcionesBDUtil.RegistraExcepcion(evento);

            //Redireccionamos a la página de mensajes de error
            HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml",
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
            };
            filterContext.ExceptionHandled = true;
        }

        /// <summary>
        /// Metodo para  guardar excepcion en BD
        /// </summary>
        /// <param name="filterContext"></param>

        /// <summary>
        /// Escribe la excepción en base de datos
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="tipo"></param>
        protected void EscribeExcepcion(Exception exception, EntradaRegistroEvento.TipoEntradaRegistroEvento tipo)
        {

            ExceptionContext ec = new ExceptionContext(this.ControllerContext, exception);
            string controllerName = (string)ec.RouteData.Values["controller"];
            string actionName = (string)ec.RouteData.Values["action"];

            EntradaRegistroEvento evento = new EntradaRegistroEvento(0,
               DateTime.Now, getUsuario(),
               EntradaRegistroEvento.TipoEntradaRegistroEvento.Error,
               Clases.Util.UsoStrings.RecortaString(controllerName + "." + actionName, 200),
               Clases.Util.UsoStrings.RecortaString(exception.ToString(), 4000),
               0,
               0
               );

            //guarda en BD
            ExcepcionesBDUtil.RegistraExcepcion(evento);
        }

        private string getUsuario()
        {
            string userName = User.Identity.Name;

            if (String.IsNullOrEmpty(userName))
                userName = "System";

            return userName;
        }
    }
}