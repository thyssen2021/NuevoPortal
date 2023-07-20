using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using static Portal_2_0.Models.UtilExcel;

namespace Portal_2_0.Controllers
{

    public class InformationController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();
        // GET: Information

        [ActionName("Vacaciones")]
        public ActionResult Index()
        {
            try
            {

                string nombreEquipo = String.Empty;
                // string nombreUsuario =  String.Empty;

                nombreEquipo = DetermineCompName(Request.UserHostName);

                //if (!String.IsNullOrEmpty(nombreEquipo))
                //    ViewBag.NombreEquipo = nombreEquipo;

                //if (!String.IsNullOrEmpty(nombreUsuario))
                //    ViewBag.NombreUsuario = nombreUsuario;

                //crear objeto de base de datos enviar a guardar en el log

                if (String.IsNullOrEmpty(nombreEquipo))
                    nombreEquipo = Request.UserHostName;

                log_acceso_email log = new log_acceso_email
                {
                    fecha = DateTime.Now,
                    nombre_equipo = nombreEquipo,
                    //  usuario = nombreUsuario
                };


                db.log_acceso_email.Add(log);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                ////do nothing
                ViewBag.Error = e.Message;
                //System.Diagnostics.Debug.Print(e.Message);
            }
            return View();
        }

        public ActionResult Log(string usuario, string nombre_equipo)
        {

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }              
          
            return View(db.log_acceso_email.ToList());
        }

        public string EnvioMailMWP()
        {
            System.Diagnostics.Debug.WriteLine("Envio de correos iniciado");

            //envia correo electronico
            EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

            List<CorreoMWP> listado = UtilExcel.LeeFormatoCorreosMWP();

            int c = 1;
            int total = listado.Count;

            foreach(var empleado in listado)
            {
                Thread.Sleep(12000);

                List<string> emailsTo = new List<string>();
                emailsTo.Add(empleado.correo);

                string body = @"<p>Hola,</p>
                        <p>Próximamente tu equipo será migrado a Modern Workplace, la cual será nuestra nueva forma de trabajo más segura. Este proceso nos tomará aproximadamente dos horas, por lo que te solicitamos que nos prestes tu equipo durante este periodo de tiempo. 
                        Tu horario asignado es el siguiente:</p>                        
                        <b>Usuario: </b>&nbsp; "+empleado.usuario+ @"</br>
                        <b>Equipo: </b>&nbsp; "+empleado.hostname+ @"</br>
                        <b>Fecha: </b>&nbsp; " + empleado.fecha.Value.ToLongDateString() + @"</br>
                        <b>Hora: </b>&nbsp; " + empleado.horario_asignado + @"</br>
                        </br>    
                        <b style=""color:red;"">Importante:</b> Recuerda que debes respaldar todos tus archivos en OneDrive antes de la migración, ya que sólo los archivos que hayas respaldado estarán disponibles después de la migración.
                        </br>
                        </br>        
                        Si tienes alguna duda respecto al proceso o tienes conflictos con el horario asigando, por favor háznoslo saber a través del correo: <a href=""mailto:IT.tkMM@lagermex.com.mx"">IT.tkMM@lagermex.com.mx</a> .
                        </br>
                        </br>   
                        <p>Saludos.</p>                            
                        ";

                envioCorreo.SendEmailAsync(emailsTo, "Horario de Migración a Modern Workplace", body);
                System.Diagnostics.Debug.WriteLine((c++)+"/"+total+" Enviando correo a:"+empleado.usuario );
            }

            System.Diagnostics.Debug.WriteLine("Envio de correos Finalizado");

            return "Email enviados";
        }


        public ActionResult Truncate()
        {
            db.Database.ExecuteSqlCommand("Truncate table [log_acceso_email]");

            TempData["Mensaje"] = new MensajesSweetAlert("Se restableció el Log", TipoMensajesSweetAlerts.SUCCESS);

            return RedirectToAction("Log");
        }
        public ActionResult EnvioEmail()
        {
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            List<string> correosCC = new List<string>();

            for (int i = 0; i < 8; i++)
            {
                correosCC.Add(string.Empty);
            }

            var model = new EnvioCorreoViewModel
            {
                nombreRemitente = "ThysenKrrupp",
                correoRemitente = "notificaciones.tkNA@lagerrmex.com.mx",
                asunto = "Actualización vacaciones",
                ccList = correosCC,
                mensaje = getBodyEmailAuditoria()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EnvioEmail(EnvioCorreoViewModel model)
        {
            //envío de correo
            List<string> correosCC = model.ccList.Where(x => !string.IsNullOrEmpty(x)).ToList();

            if (correosCC.Count == 0)
            {
                ModelState.AddModelError("", "La lista de destinatarios se encuentra vacía.");
            }



            if (ModelState.IsValid)
            {
                

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                

                envioCorreo.SendEmailAsyncAuditoria(correosCC, model.nombreRemitente, model.correoRemitente, model.asunto, model.mensaje);


                TempData["Mensaje"] = new MensajesSweetAlert("Se envío el correo electrónico.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("EnvioEmail");
            }

            return View(model);
        }

        public static string DetermineCompName(string IP)
        {
            try
            {
                System.Net.IPAddress myIP = System.Net.IPAddress.Parse(IP);
                System.Net.IPHostEntry GetIPHost = System.Net.Dns.GetHostEntry(myIP);
                List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
                return compName.First();
            }
            catch
            {
                return null;
            }

        }

        [NonAction]
        public string getBodyEmailAuditoria()
        {
            string domainName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = @"<style>                          
                            h2,h3{color:#00a0f4;}                           
                            .output {
                                font: 1rem 'Fira Sans', sans-serif;
                            }
                            dt {
								color:darkblue;
                                font-weight: bold;
                            }
                            dl,
                            dd {
                                font-size: .9rem;
                            }
                            dd {
                                margin-bottom: 1em;
                            }		
                            p{
                                color:gray;
                            }
                            </style>
                            <h2>Hola, se ha modificado o actualizado el estatus de tus vacaciones.</h2></br>

                            <dl>
                                <dt>Título<dt>
                                <dd>" + "Vacacioness" + @"</dd>
                                <dt>Situación Actual</dt>
                                <dd>" + "Aun tienes diaz por tomar" + @"</dd>
                                <dt>Objetivo</dt>
                                <dd>" + "Regularizar diaz." + @"</dd>
                                <dt>Descripción</dt>
                                <dd>" + "Para poder validar los días a disfrutar favor de llenar y confirmar en el siguiente link" + @".</dd>
                            </dl>

                            <h3>Para ver los diaz pendientes haga clic en el siguente enlace:</h3>                           
                            <a href='" + domainName + @"/Information/vacaciones'>Vacaciones</a>
                            <hr />                          
                            ";

            return body;
        }


    }
}
