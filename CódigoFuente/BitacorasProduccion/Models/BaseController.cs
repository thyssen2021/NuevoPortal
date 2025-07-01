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
        private Portal_2_0Entities db = new Portal_2_0Entities();

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
            if (User.Identity == null) { //cierra sesión 
                return false;
            }

            if (_userManager.IsInRoleAsync(User.Identity.GetUserId(), rol).Result)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Agrega un elemento a un select list
        /// </summary>
        public static SelectList AddFirstItem(SelectList origList, string textoPorDefecto = "", string selected = "" )
        {
            string defaultText_ = "-- Seleccione un valor --";

            if (!string.IsNullOrEmpty(textoPorDefecto))
                defaultText_ = textoPorDefecto;

            SelectListItem firstItem = new SelectListItem()
            {
                Text = defaultText_,
                Value =""
            };

            List<SelectListItem> newList = origList.ToList();
            newList.Insert(0, firstItem);

            var selectedItem = newList.FirstOrDefault(item => item.Selected);
            var selectedItemValue = selected;
            if (selectedItem != null)
            {
                selectedItemValue = selectedItem.Value;
            }

            return new SelectList(newList, "Value", "Text", selectedItemValue);
        }



        /// <summary>
        /// Obtiene el empleado logeado  
        public empleados obtieneEmpleadoLogeado()
        {
           
            string userId = String.Empty;
            if (User.Identity!=null) {
                userId = User.Identity.GetUserId();
            }

           int idEmpleado = Clases.DBUtil.UsuariosDBUtil.ObtieneIdEmpleadoById(userId);

            empleados empleado = db.empleados.Find(idEmpleado);

            if (empleado == null)
                empleado = new empleados
                {
                    id = 0,
                    nombre = "NO DISPONIBLE",
                    planta_clave =0,
                };

            return empleado;
        }


        /// <summary>
        /// Evento disparado cuando no se administra una excepción adecuadamente
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            // No hacer nada si la excepción ya fue manejada
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            // 1. Obtener toda la información de contexto posible
            var exception = filterContext.Exception;
            string controllerName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string source = $"{controllerName}.{actionName}";
            string user = getUsuario(); // Tu método para obtener el usuario

            // 2. CONSTRUIR UN MENSAJE DE ERROR DETALLADO (LA CLAVE ESTÁ AQUÍ)
            // Esta función recursiva extrae los mensajes de TODAS las excepciones internas.
            var fullExceptionDetails = new System.Text.StringBuilder();
            Exception currentException = exception;
            int level = 0;
            while (currentException != null)
            {
                fullExceptionDetails.AppendLine($"--- Exception Level {level} ---");
                fullExceptionDetails.AppendLine($"Type: {currentException.GetType().FullName}");
                fullExceptionDetails.AppendLine($"Message: {currentException.Message}");
                fullExceptionDetails.AppendLine($"StackTrace: {currentException.StackTrace}");
                fullExceptionDetails.AppendLine();
                currentException = currentException.InnerException;
                level++;
            }

            // 3. Crear el evento para guardarlo en la base de datos
            EntradaRegistroEvento evento = new EntradaRegistroEvento(
                0,
                DateTime.Now,
                user,
                EntradaRegistroEvento.TipoEntradaRegistroEvento.Error,
                UsoStrings.RecortaString(source, 200),
                // Guardamos el mensaje de error completo y detallado
                UsoStrings.RecortaString(fullExceptionDetails.ToString(), 4000),
                0,
                0
            );

            // 4. Guardar en la base de datos
            try
            {
                ExcepcionesBDUtil.RegistraExcepcion(evento);
            }
            catch (Exception dbEx)
            {
                // Si falla el guardado en la BD, al menos lo escribimos en el Log de eventos de Windows para no perderlo.
                System.Diagnostics.EventLog.WriteEntry("Application", "Error al registrar excepción en BaseController: " + dbEx.Message, System.Diagnostics.EventLogEntryType.Error);
                System.Diagnostics.EventLog.WriteEntry("Application", "Excepción original: " + fullExceptionDetails.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }

            // 5. Redireccionar a la página de error (sin cambios)
            HandleErrorInfo model = new HandleErrorInfo(exception, controllerName, actionName);
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
        //obtiene el usuario actual
        private string getUsuario()
        {
            if (User.Identity == null)
            {  
                return string.Empty;
            }

            string userName = User.Identity.Name;

            if (String.IsNullOrEmpty(userName))
                userName = "System";

            return userName;
        }

        
    }
}