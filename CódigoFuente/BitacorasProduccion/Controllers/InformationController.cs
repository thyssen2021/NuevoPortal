using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Management;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Portal_2_0.Filters;

namespace Portal_2_0.Controllers
{

    public class InformationController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // [ActionName("Vacaciones")]
        // GET: Muestra la vista para enviar comunicados
        public ActionResult Comunicado()
        {
            var viewModel = new ComunicadoViewModel();

            // Obtenemos los empleados activos con correo
            var empleadosConCorreo = db.empleados
                .Where(e => e.activo == true && !string.IsNullOrEmpty(e.correo))
                .ToList();

            // Proyectamos los datos al modelo de selección
            viewModel.Empleados = empleadosConCorreo.Select(e => new EmpleadoSeleccionable
            {
                Id = e.id,
                Seleccionado = false, // Todos seleccionados por defecto
                NombreCompleto = e.ConcatNumEmpleadoNombrePlanta,
                Correo = e.correo
            }).ToList();

            // Mensaje prellenado por defecto
            // Mensaje prellenado con el nuevo formato y los placeholders
            viewModel.CuerpoMensaje = @"
<div style='font-family: Arial, sans-serif; text-align: center;'>
    <img src='cid:logo_empresa' alt='Logo Grupo Tress' style='max-width: 200px; margin-bottom: 20px;' />
    <div style='text-align: left; padding: 0 15px;'>
        <p><strong>Querido {NombreEmpleado}:</strong></p>
        <p><strong>Urge que actualices tus datos en TRESS.</strong></p>
        <p>Si no actualizas tus datos, serán eliminados de las siguientes plataformas:</p>
        <table border='0' cellpadding='0' cellspacing='0' width='100%' style='margin-top: 20px; margin-bottom: 20px; text-align: center;'>
            <tr>
                <td align='center' valign='top'>
                    <img src='cid:logo_workflow' alt='Logo WorkFlow' style='max-width: 150px;' />                
                </td>
                <td align='center' valign='top'>
                    <img src='cid:logo_orden' alt='Logo ORDEN' style='max-width: 150px;' />                    
                </td>
            </tr>
        </table>
        <p>Tienes <strong>8 horas</strong> a partir de hoy para entrar al siguiente enlace:</p>
        <p style='text-align: center; margin: 20px 0;'>
            <a href='{EnlaceEncuesta}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; font-size: 16px;'>Actualizar Mis Datos</a>
        </p>
    </div>
</div>";

            ViewBag.Title = "Envío de Comunicado Masivo";
            viewModel.Asunto = "Ejercicio Phishing";
            return View(viewModel);
        }

        // POST: Envía los correos
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public ActionResult EnviarComunicado(ComunicadoViewModel model)
        {
            // Valida que los datos del formulario (Asunto, CuerpoMensaje) sean correctos
            if (!ModelState.IsValid)
            {
                // Si hay errores de validación, se los puedes notificar al usuario.
                // Este es un manejo básico, se puede mejorar para devolver los errores específicos.
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Datos inválidos. Por favor, revisa el formulario.");
            }

            try
            {
                // 1. Obtener la configuración del servidor de correo desde Web.config
                var smtpServer = ConfigurationManager.AppSettings["smtpServer"];
                var portEmail = int.Parse(ConfigurationManager.AppSettings["portEmail"]);
                var emailSMTP = ConfigurationManager.AppSettings["emailSMTP"];
                var paswordEmail = ConfigurationManager.AppSettings["paswordEmail"]; // Contraseña (si la hubiera)

                // 2. Configurar el cliente SMTP
                var smtpClient = new SmtpClient(smtpServer, portEmail)
                {
                    // Descomenta la siguiente línea si tu servidor requiere autenticación
                    // Credentials = new NetworkCredential(emailSMTP, paswordEmail),
                    EnableSsl = false // Cambia a 'true' si tu servidor usa SSL
                };

                // 1. Obtener la LISTA DE IDs de los empleados seleccionados
                var idsDestinatarios = model.Empleados
                                            .Where(e => e.Seleccionado)
                                            .Select(e => e.Id)
                                            .ToList();

                if (!idsDestinatarios.Any())
                {
                    return Json(new { success = false, message = "No se ha seleccionado ningún destinatario." });
                }

                // 2. Usar esos IDs para obtener los objetos COMPLETOS desde la base de datos
                var destinatariosCompletos = db.empleados
                  .Where(e =>
                      idsDestinatarios.Contains(e.id) &&          // Debe estar en la lista de seleccionados Y
                      !string.IsNullOrEmpty(e.correo) &&          // Debe tener un correo (no nulo ni vacío) Y
                      !e.correo.ToLower().Contains("lagermex")    // El correo NO debe contener "lagermex"
                  )
                  .ToList();

                // 4. Recorrer cada destinatario para enviarle un correo personalizado
                foreach (var destinatario in destinatariosCompletos)
                {
                    // Generar un token único para el seguimiento de este envío específico
                    var trackingToken = Guid.NewGuid();

                    // Crear el registro de log para la base de datos
                    var logEntry = new EmailTrackingLog
                    {
                        TrackingToken = trackingToken,
                        IdEmpleado = destinatario.id,
                        FechaEnvio = DateTime.Now,
                        FechaClic = null // Se llenará cuando el usuario haga clic
                    };
                    db.EmailTrackingLog.Add(logEntry);

                    string rutaLogoTress = Server.MapPath("~/Content/images/logo_tress.png");
                    string rutaLogoWorkflow = Server.MapPath("~/Content/images/login1.png"); // Reemplaza con el nombre real de tu archivo
                    string rutaLogoOrden = Server.MapPath("~/Content/images/login2.png");       // Reemplaza con el nombre real de tu archivo



                    // Crear la URL de seguimiento que apunta a nuestra acción placeholder
                    // Asegúrate de que el nombre de la acción ("MostrarLugarEncuesta") sea correcto
                    string urlDeSeguimiento = Url.Action("MostrarLugarEncuesta", "Information", new { token = trackingToken }, Request.Url.Scheme);

                    // Crear el enlace HTML completo
                    string enlaceHtml = $"<a href='{urlDeSeguimiento}'>Clic aquí para continuar</a>";

                    // Reemplazar el marcador de posición en el cuerpo del correo con el enlace generado
                    string cuerpoPersonalizado = model.CuerpoMensaje
                                      .Replace("{NombreEmpleado}", destinatario.ConcatNombre)
                                      .Replace("{EnlaceEncuesta}", urlDeSeguimiento);
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(cuerpoPersonalizado, null, "text/html");

                    // Recurso para el logo principal
                    LinkedResource logoTress = new LinkedResource(rutaLogoTress);
                    logoTress.ContentId = "logo_empresa";

                    // Recurso para la imagen de Workflow
                    LinkedResource logoWorkflow = new LinkedResource(rutaLogoWorkflow);
                    logoWorkflow.ContentId = "logo_workflow"; // Debe coincidir con el cid:logo_workflow

                    // Recurso para la imagen de Orden
                    LinkedResource logoOrden = new LinkedResource(rutaLogoOrden);
                    logoOrden.ContentId = "logo_orden";     // Debe coincidir con el cid:logo_orden

                    // Incrustar la imagen en la vista HTML
                    // Incrustar las 3 imágenes en la vista HTML
                    htmlView.LinkedResources.Add(logoTress);
                    htmlView.LinkedResources.Add(logoWorkflow);
                    htmlView.LinkedResources.Add(logoOrden);

                    // 5. Preparar el mensaje de correo
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(emailSMTP, "Comunicados Corporativos"), // Remitente
                        Subject = model.Asunto,                                        // Asunto
                        IsBodyHtml = true,                                             // Indicar que el cuerpo es HTML
                    };

                    // Agregar el destinatario principal
                    mailMessage.To.Add(new MailAddress(destinatario.correo));
                    mailMessage.AlternateViews.Add(htmlView); // Se agrega la vista HTML con la imagen incrustada

                    // Agregar copias (CC y CCO) si se especificaron en el formulario
                    if (!string.IsNullOrEmpty(model.CC))
                        mailMessage.CC.Add(model.CC);
                    if (!string.IsNullOrEmpty(model.CCO))
                        mailMessage.Bcc.Add(model.CCO);

                    // 6. Enviar el correo
                    smtpClient.Send(mailMessage);
                }

                // 7. Guardar todos los registros de log en la base de datos de una sola vez
                db.SaveChanges();

                return Json(new { success = true, message = $"Comunicado enviado exitosamente a {destinatariosCompletos.Count} destinatarios." });
            }
            catch (Exception ex)
            {
                // En caso de un error, devolver un mensaje para que el frontend lo muestre
                // Es buena práctica registrar el error completo (ex.ToString()) en un sistema de logs
                return Json(new { success = false, message = "Ocurrió un error al enviar los correos. Detalles: " + ex.Message });
            }
        }

        [HttpGet]
        public ActionResult MostrarLugarEncuesta(Guid token)
        {
            // 1. Validar que el token no venga vacío
            if (token == Guid.Empty)
            {
                // Si el token es inválido, puedes mostrar un error o simplemente la página por defecto
                return View("MostrarLugarEncuesta");
            }

            try
            {
                // 2. Buscar el registro de envío que corresponde a este token
                var logEntry = db.EmailTrackingLog.FirstOrDefault(l => l.TrackingToken == token);

                // 3. Si se encuentra el registro y es la PRIMERA VEZ que se hace clic:
                if (logEntry != null && !logEntry.FechaClic.HasValue)
                {
                    // Se registra la fecha y hora actuales como el momento del clic
                    logEntry.FechaClic = DateTime.Now;

                    // Se guardan los cambios en la base de datos
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Es buena práctica registrar cualquier error de base de datos
                // pero aún así mostrarle la página al usuario para no interrumpir su experiencia.
                // Log.Error("Error al registrar clic de encuesta: " + ex.Message);
            }

            // 4. Siempre se muestra la vista del marcador de posición al usuario
            return View("MostrarLugarEncuesta");
        }

        public ActionResult Seguimiento()
        {
            // Obtenemos todos los registros del log, incluyendo los datos del empleado relacionado
            // y los ordenamos por la fecha de envío más reciente.
            var logs = db.EmailTrackingLog
                         .Include(log => log.empleados) // Carga los datos del empleado (hace el JOIN)
                         .OrderByDescending(log => log.FechaEnvio)
                         .ToList();

            ViewBag.Title = "Seguimiento de Comunicados";
            return View(logs);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }

    // Pequeña clase para manejar la selección en la tabla
    public class EmpleadoSeleccionable
    {
        public int Id { get; set; }
        public bool Seleccionado { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
    }

    public class ComunicadoViewModel
    {
        // Lista para Handsontable
        public List<EmpleadoSeleccionable> Empleados { get; set; }

        // Campos del formulario
        [Required(ErrorMessage = "El asunto es requerido.")]
        [Display(Name = "Asunto")]
        public string Asunto { get; set; }

        [Display(Name = "CC (separar correos con coma)")]
        public string CC { get; set; }

        [Display(Name = "CCO (separar correos con coma)")]
        public string CCO { get; set; }

        [Required(ErrorMessage = "El cuerpo del mensaje es requerido.")]
        [Display(Name = "Cuerpo del Mensaje")]
        [AllowHtml]
        public string CuerpoMensaje { get; set; }

        public ComunicadoViewModel()
        {
            Empleados = new List<EmpleadoSeleccionable>();
        }
    }
}
