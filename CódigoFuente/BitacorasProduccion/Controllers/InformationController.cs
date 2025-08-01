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
using System.Threading;
using Hangfire;
using iText.Kernel.Pdf.Canvas.Wmf;
using Hangfire.Server;
using Hangfire.Console;

namespace Portal_2_0.Controllers
{

    public class InformationController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // [ActionName("Vacaciones")]
        // GET: Muestra la vista para enviar comunicados
        public ActionResult EnvioCorreo()
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
                Correo = e.correo,
                PlantaId = e.planta_clave // Add the Plant ID
            }).ToList();

            // Mensaje prellenado por defecto
            // Mensaje prellenado con el nuevo formato y los placeholders
            viewModel.CuerpoMensaje = @"
<body style='background-color: #f4f4f4; margin: 0; padding: 0;'>
    <table border='0' cellpadding='0' cellspacing='0' width='100%'>
        <tr>
            <td align='center' style='padding: 20px 0;'>
                <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);'>
                    <tr>
                        <td align='center' style='padding: 30px 20px 20px 20px;'>
                            <img src='cid:logo_empresa' alt='Logo Grupo Tress' style='max-width: 200px;' />
                        </td>
                    </tr>
                    <tr>
                        <td align='left' style='padding: 20px 30px; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; color: #333333;'>
                            <p><strong>Hola, {NombreEmpleado}:</strong></p>
                            <p><strong style='color: #f05e16;'>Urge que actualices tus datos en TRES.</strong></p>
                            <p>Sin no actualisas tus datos, serán eliminados de las siguientes plataformas:</p>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='padding: 10px 30px;'>
                            <table border='0' cellpadding='0' cellspacing='0' width='100%'>
                                <tr>
                                    <td align='center' valign='top' width='50%' style='font-family: Arial, sans-serif; font-size: 14px; color: #555;'>
                                        <img src='cid:logo_workflow' alt='Logo WorkFlow' style='max-width: 120px;' />
                                        <p style='margin-top: 5px;'>Work-Flow</p>
                                    </td>
                                    <td align='center' valign='top' width='50%' style='font-family: Arial, sans-serif; font-size: 14px; color: #555;'>
                                        <img src='cid:logo_orden' alt='Logo ORDEN' style='max-width: 120px;' />
                                        <p style='margin-top: 5px;'>MASS-ORDHEN</p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align='left' style='padding: 20px 30px; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; color: #333333;'>
                            <p>Tienes <strong>8 horas</strong> a partir de hoy para entrar al siguiente enlace:</p>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='padding: 10px 30px 30px 30px;'>
                            <a href='{EnlaceEncuesta}' target='_blank' style='background-color: #f05e16; color: #ffffff; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-size: 18px; font-weight: bold; display: inline-block;'>Actualizar Mis Datos</a>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='padding: 20px 30px; font-size: 12px; color: #888888; font-family: Arial, sans-serif;'>
                            <p>Este es un correo automatizado. Por favor, no respondas a este mensaje.</p>
                        </td>
                    </tr>
                </table>
                </td>
        </tr>
    </table>
</body>";

            // Load the list of plants for the filter
            viewModel.PlantasDisponibles = db.plantas.OrderBy(p => p.descripcion)
                .Select(p => new SelectListItem { Value = p.clave.ToString(), Text = p.descripcion })
                .ToList();

            ViewBag.Title = "Envío de Comunicado Masivo";
            viewModel.Asunto = "Acción Requerida: Actualización Urgente de tus Datos";
            viewModel.OmitirExistentes = true;

            return View(viewModel);
        }

        // POST: Envía los correos
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public ActionResult EnviarComunicado(ComunicadoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Datos inválidos.");
            }

            try
            {
                var idsDestinatarios = model.Empleados
                    .Where(e => e.Seleccionado)
                    .Select(e => e.Id)
                    .ToList();

                if (!idsDestinatarios.Any())
                {
                    return Json(new { success = false, message = "No se ha seleccionado ningún destinatario." });
                }

                string urlPlantilla = Url.Action("ActualizaDatos", "Information", new { token = "TOKEN_PLACEHOLDER" }, Request.Url.Scheme);

                // Se llama al método en la clase EmailJobs, pasando 'null' para el PerformContext
                BackgroundJob.Enqueue<EmailJobs>(jobs => jobs.ProcesarEnvioMasivo(idsDestinatarios, model.Asunto, model.CuerpoMensaje, model.CC, model.CCO, urlPlantilla, model.OmitirExistentes, null));

                return Json(new { success = true, message = $"Solicitud recibida. Se enviarán {idsDestinatarios.Count} comunicados en segundo plano." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error al crear la tarea de envío. Detalles: " + ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ActualizaDatos(Guid token)
        {
            // 1. Validar que el token no venga vacío
            if (token == Guid.Empty)
            {
                // Si el token es inválido, puedes mostrar un error o simplemente la página por defecto
                return View("ActualizaDatos");
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
            return View("ActualizaDatos");
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


        [HttpPost]
        [ValidateJsonAntiForgeryToken] // Usamos el atributo que creamos para peticiones AJAX
        [Authorize(Roles = TipoRoles.IT_CATALOGOS)] // Solo usuarios autorizados pueden borrar
        public ActionResult LimpiarLog()
        {
            try
            {
                // La forma más eficiente de borrar todos los registros de una tabla
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE EmailTrackingLog");

                return Json(new { success = true, message = "El log de seguimiento ha sido borrado exitosamente." });
            }
            catch (Exception ex)
            {
                // Registrar el error (opcional pero recomendado)
                return Json(new { success = false, message = "Ocurrió un error al intentar borrar el log. " + ex.Message });
            }
        }



        //// Agrega este método para iniciar la prueba desde tu navegador
        //[HttpGet]
        //public ActionResult IniciarPruebaDeEnvio()
        //{
        //    string correoDePrueba = "alfredo.xochitemol@thyssenkrupp-materials.com";

        //    // AHORA LA LLAMADA APUNTA A LA NUEVA CLASE EmailJobs
        //    BackgroundJob.Enqueue<EmailJobs>(jobs => jobs.ProcesarEnvioDePrueba(correoDePrueba, null));

        //    return Content("¡Prueba iniciada! Revisa el dashboard de Hangfire en /hangfire para ver el progreso.");
        //}

        // Este es el método que Hangfire ejecutará en segundo plano


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
        public int? PlantaId { get; set; } // ID for filtering

    }

    public class ComunicadoViewModel
    {
        // Lista para Handsontable
        public List<EmpleadoSeleccionable> Empleados { get; set; }
        public IEnumerable<SelectListItem> PlantasDisponibles { get; set; } // List for the dropdown


        // Campos del formulario
        [Required(ErrorMessage = "El asunto es requerido.")]
        [Display(Name = "Asunto")]
        public string Asunto { get; set; }

        [Display(Name = "Omitir empleados que ya recibieron el correo")]
        public bool OmitirExistentes { get; set; }

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

    public class EmailJobs
    {
        // ESTE MÉTODO ES PARA HANGFIRE, NO ES UNA ACCIÓN ACCESIBLE DESDE LA WEB
        // Debe ser público para que Hangfire pueda verlo.
        public void ProcesarEnvioMasivo(List<int> idsDestinatarios, string asunto, string cuerpoMensaje, string cc, string cco, string urlPlantilla, bool omitirExistentes, PerformContext context)
        {
            using (var db = new Portal_2_0Entities())
            {
                // 1. La configuración del cliente SMTP se hace aquí, dentro del trabajo
                var smtpServer = ConfigurationManager.AppSettings["smtpServer"];
                var portEmail = int.Parse(ConfigurationManager.AppSettings["portEmail"]);
                var emailSMTP = ConfigurationManager.AppSettings["emailSMTP"];
                var paswordEmail = ConfigurationManager.AppSettings["paswordEmail"];
                var smtpClient = new SmtpClient(smtpServer, portEmail) { EnableSsl = false };

                // Obtenemos el total de empleados que se seleccionaron en la pantalla
                int totalSeleccionados = idsDestinatarios.Count;
                context.WriteLine($"Se recibieron {totalSeleccionados} empleados seleccionados desde la interfaz.");

                // 2. Si la opción de omitir está activa, filtramos los IDs
                if (omitirExistentes)
                {
                    context.WriteLine("Opción 'Omitir Existentes' activada. Verificando historial...");
                    // Obtenemos los IDs de los empleados que ya están en el log
                    var idsYaEnviados = db.EmailTrackingLog.Select(log => log.IdEmpleado).ToList();

                    // Excluimos esos IDs de la lista a procesar
                    var idsOriginalesCount = idsDestinatarios.Count;
                    idsDestinatarios = idsDestinatarios.Except(idsYaEnviados).ToList();
                    var omitidosCount = idsOriginalesCount - idsDestinatarios.Count;

                    if (omitidosCount > 0)
                    {
                        context.WriteLine($"Se omitieron {omitidosCount} empleados porque ya existen en el log.", ConsoleTextColor.Yellow);
                    }
                }

                // 2. Usamos los IDs para obtener los destinatarios, igual que antes
                var destinatariosCompletos = db.empleados
                 .Where(e =>
                     idsDestinatarios.Contains(e.id) &&
                     !string.IsNullOrEmpty(e.correo) &&
                     e.correo.ToLower().Contains("thyssenkrupp-materials")
                 )
                 .ToList();


                int totalAEnviar = destinatariosCompletos.Count;
                int totalExcluidos = totalSeleccionados - totalAEnviar;

                // Escribimos un log con el resumen del filtrado
                if (totalExcluidos > 0)
                {
                    context.WriteLine($"Excluyendo {totalExcluidos} destinatarios (sin correo, ya enviado o con dominio no permitido).", ConsoleTextColor.Yellow);
                }

                context.WriteLine($"Procesando {totalAEnviar} correos válidos..."); 
                var progressBar = context.WriteProgressBar("Progreso de Envío", 0);
                int contador = 0;

                // 3. Se recorre cada destinatario para enviar el correo
                foreach (var destinatario in destinatariosCompletos)
                {
                    contador++;

                    var trackingToken = Guid.NewGuid();

                    // Preparación del correo (esta lógica no cambia)
                    string rutaLogoTress = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/images/logo_tress.png");
                    string rutaLogoWorkflow = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/images/login1.png"); // Reemplaza con el nombre real de tu archivo
                    string rutaLogoOrden = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/images/login2.png");       // Reemplaza con el nombre real de tu archivo

                 
                    string urlDeSeguimiento = urlPlantilla.Replace("TOKEN_PLACEHOLDER", trackingToken.ToString());

                    string cuerpoPersonalizado = cuerpoMensaje
                                                      .Replace("{NombreEmpleado}", destinatario.ConcatNombre)
                                                      .Replace("{EnlaceEncuesta}", urlDeSeguimiento);

                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(cuerpoPersonalizado, null, "text/html");

                    LinkedResource logoTress = new LinkedResource(rutaLogoTress) { ContentId = "logo_empresa" };
                    LinkedResource logoWorkflow = new LinkedResource(rutaLogoWorkflow) { ContentId = "logo_workflow" };
                    LinkedResource logoOrden = new LinkedResource(rutaLogoOrden) { ContentId = "logo_orden" };

                    htmlView.LinkedResources.Add(logoTress);
                    htmlView.LinkedResources.Add(logoWorkflow);
                    htmlView.LinkedResources.Add(logoOrden);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(emailSMTP, "Actualización de Datos TRESS"),
                        Subject = asunto
                    };
                    mailMessage.To.Add(new MailAddress(destinatario.correo));
                    mailMessage.AlternateViews.Add(htmlView);
                    if (!string.IsNullOrEmpty(cc)) mailMessage.CC.Add(cc);
                    if (!string.IsNullOrEmpty(cco)) mailMessage.Bcc.Add(cco);

                    try
                    {
                        smtpClient.Send(mailMessage);

                        var logEntry = new EmailTrackingLog
                        {
                            TrackingToken = trackingToken,
                            IdEmpleado = destinatario.id,
                            FechaEnvio = DateTime.Now,
                            FechaApertura = null
                        };
                        db.EmailTrackingLog.Add(logEntry);
                        db.SaveChanges();

                        // Mensaje de éxito en la consola
                        context.WriteLine($"{DateTime.Now:HH:mm:ss} - [{contador}/{totalAEnviar}] Correo enviado a: {destinatario.ConcatNombre}", ConsoleTextColor.Green);
                        progressBar.SetValue((contador * 100) / totalAEnviar);
                    }
                    catch (SmtpException smtpEx)
                    {
                        // Mensaje de error en la consola
                        context.WriteLine($"{DateTime.Now:HH:mm:ss} - [{contador}/{totalAEnviar}] ERROR al enviar a {destinatario.ConcatNombre}: {smtpEx.Message}", ConsoleTextColor.Red);
                    }

                    // 6. ¡LA PAUSA CLAVE! Espera 2 segundos antes de enviar el siguiente.
                    Thread.Sleep(5000);
                }
            }
        }

        // El método de prueba ahora vive en esta clase
        public void ProcesarEnvioDePrueba(string correoDestino, PerformContext context)
        {
            // Creamos una nueva instancia del contexto de la base de datos
            using (var db = new Portal_2_0Entities())
            {
                var smtpServer = ConfigurationManager.AppSettings["smtpServer"];
                var portEmail = int.Parse(ConfigurationManager.AppSettings["portEmail"]);
                var emailSMTP = ConfigurationManager.AppSettings["emailSMTP"];
                var smtpClient = new SmtpClient(smtpServer, portEmail) { EnableSsl = false };

                context.WriteLine("Iniciando envío de 5 correos de prueba...");
                var progressBar = context.WriteProgressBar("Progreso de Envío", 0);

                for (int i = 1; i <= 5; i++)
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(emailSMTP, "Prueba Hangfire"),
                        Subject = $"Correo de prueba #{i} - {DateTime.Now:HH:mm:ss}",
                        Body = $"<p>Este es el correo de prueba número {i} enviado por Hangfire.</p>",
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(new MailAddress(correoDestino));

                    try
                    {
                        smtpClient.Send(mailMessage);
                        context.WriteLine($"{DateTime.Now:HH:mm:ss} - Correo #{i} enviado a {correoDestino}", ConsoleTextColor.Green);
                        progressBar.SetValue(i * 20);
                    }
                    catch (SmtpException ex)
                    {
                        context.WriteLine($"Error en envío #{i}: {ex.Message}", ConsoleTextColor.Red);
                    }

                    if (i < 5)
                    {
                        Thread.Sleep(60000);
                    }
                }
                context.WriteLine("¡Prueba finalizada!");
            }
        }
    }
}
