using Bitacoras.Util;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using IdentitySample.Models;
using Portal_2_0.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Portal_2_0.Models
{
    public class EnvioCorreoElectronico
    {
        private delegate void DelegateEmail(List<string> emailsTo, string subject, string message, List<string> emailsCC = null); //delegate for the action
        private delegate void DelegateEmailAuditoria(List<string> emailsCC, string nombreRemitente, string correoRemitente, string subject, string message); //delegate for the action

        public string NOMBRE_FROM { get { return "thyssenkrupp Materials de México"; } }
        public string FILA_GENERICA
        {
            get
            {
                return " <tr style = \"border - collapse:collapse\">" +
                    "<td style =\"padding:5px 10px 5px 0;Margin:0\" width = \"50%\" align = \"left\" ><p style = \"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'open sans', 'helvetica neue', helvetica, arial, sans-serif;line-height:24px;color:#333333;font-size:16px\">#CONCEPTO</p ></td>" +
                    "<td style = \"padding:5px 0;Margin:0\" width = \"50%\" align = \"left\" ><p style = \"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'open sans', 'helvetica neue', helvetica, arial, sans-serif;line-height:24px;color:#333333;font-size:16px\">#VALOR</p></td>" +
                    "</tr>";
            }
        }

        public string FILA_GENERICA_SCDM
        {
            get
            {
                return @"<tr><td style = ""padding:0;Margin:0;font-size:14px"" > #CONCEPTO</ td >
                            <td style = ""padding:0;Margin:0;font-size:14px"" > #VALOR </ td >
                        </tr > ";
            }
        }

        public void SendEmailAsync(List<string> emailsTo, string subject, string message, List<string> emailsCC = null)
        {
            try
            {
                emailsCC = emailsCC ?? new List<string>();

                DelegateEmail sd = DelegateEmailMethod;

                IAsyncResult asyncResult = sd.BeginInvoke(emailsTo, subject, message, emailsCC, null, null);
            }

            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }

            //return Task.FromResult(0);
        }
        public void SendEmailAsyncAuditoria(List<string> emailsCC, string nombreRemitente, string correoRemitente, string subject, string message)
        {
            try
            {
                DelegateEmailAuditoria sd = DelegateEmailMethodAuditoria;

                IAsyncResult asyncResult = sd.BeginInvoke(emailsCC, nombreRemitente, correoRemitente, subject, message, null, null);
            }

            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }

            //return Task.FromResult(0);
        }

        public void DelegateEmailMethod(List<string> emailsTo, string subject, string message, List<string> emailsCC = null)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                string smtpServer = appSettings["smtpServer"];
                int portEmail = Int32.Parse(appSettings["portEmail"]);
                string emailSMTP = appSettings["emailSMTP"];
                string paswordEmail = appSettings["paswordEmail"];

                // Credentials
                var credentials = new NetworkCredential(emailSMTP, paswordEmail);
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(emailSMTP, this.NOMBRE_FROM),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                //********** Comentar para productivo ************//
                emailsTo = new List<string>();
                emailsTo.Add("alfredo.xochitemol@thyssenkrupp-materials.com");
                emailsCC = new List<string>();
                mail.Subject = "(🔔 Pruebas MM) " + subject;
                emailsCC.Add("alfredo.xochitemol@thyssenkrupp-materials.com");
                // ************************************//

                //agrega los destinatarios
                foreach (string email in emailsTo)
                {
                    if (!string.IsNullOrEmpty(email) && !email.Contains("lagermex.com.mx"))
                        mail.To.Add(new MailAddress(email));
                }

                //agrega los destinatarios CC
                foreach (string email in emailsCC)
                {
                    if (!string.IsNullOrEmpty(email) && !email.Contains("lagermex.com.mx"))
                        mail.CC.Add(new MailAddress(email));
                }

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = portEmail,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = smtpServer,
                    EnableSsl = false,
                    Credentials = credentials
                };

                // Send it...
                if (mail.To.Count > 0 || mail.CC.Count > 0)
                    client.Send(mail);
            }
            catch (System.Net.Mail.SmtpException emailExeption)
            {
                System.Diagnostics.Debug.Print("Error:" + emailExeption.StackTrace);
                System.Diagnostics.Debug.Print("Error:" + emailExeption.Message);
                //throw emailExeption;
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                System.Diagnostics.Debug.Print("Error:" + ex.StackTrace);
                // throw new InvalidOperationException(ex.Message);
            }

            //return Task.FromResult(0);
        }
        public void DelegateEmailMethodAuditoria(List<string> emailsCC, string nombreRemitente, string correoRemitente, string subject, string message)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                string smtpServer = appSettings["smtpServer"];
                int portEmail = Int32.Parse(appSettings["portEmail"]);
                string emailSMTP = correoRemitente;
                string paswordEmail = appSettings["paswordEmail"];

                // Credentials
                var credentials = new NetworkCredential(emailSMTP, paswordEmail);
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(emailSMTP, nombreRemitente),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };



                //agrega los destinatarios CC
                foreach (string email in emailsCC)
                {
                    if (!string.IsNullOrEmpty(email))
                        mail.Bcc.Add(new MailAddress(email));
                }

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = portEmail,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = smtpServer,
                    EnableSsl = true,
                    Credentials = credentials
                };

                // Send it...
                if (mail.Bcc.Count > 0)
                    client.Send(mail);
            }
            catch (System.Net.Mail.SmtpException emailExeption)
            {
                System.Diagnostics.Debug.Print("Error:" + emailExeption.StackTrace);
                System.Diagnostics.Debug.Print("Error:" + emailExeption.Message);
                //throw emailExeption;
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                System.Diagnostics.Debug.Print("Error:" + ex.StackTrace);
                // throw new InvalidOperationException(ex.Message);
            }

            //return Task.FromResult(0);
        }

        #region correos PFA
        //metodo para el body de uuna remiones enviada
        public string getBodyPFAEnviado(PFA pFA)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PFA_solicitud_recibida.html"));

            body = body.Replace("#USUARIO", pFA.empleados1.ConcatNombre);
            body = body.Replace("#NUM_PFA", pFA.id.ToString());
            body = body.Replace("#REASON_PFA", pFA.PFA_Reason.descripcion);
            body = body.Replace("#TYPE_SHIPMENT", pFA.PFA_Type_shipment.descripcion);
            body = body.Replace("#RESPONSIBLE_PFA", pFA.PFA_Responsible_cost.descripcion);
            body = body.Replace("#SAP_PART_NUMBER", pFA.sap_part_number);
            body = body.Replace("#CUSTOMER_PART_NUMBER", pFA.customer_part_number);
            body = body.Replace("#TOTAL_PF_COST", "$ " + pFA.total_pf_cost.ToString());
            body = body.Replace("#TOTAL_ORIGINAL_COST", "$ " + pFA.total_cost.ToString());
            body = body.Replace("#TOTAL_COST_TO_RECOVER", "$ " + pFA.TotalCostToRecover.ToString());
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            body = body.Replace("#ENLACE", domainName + "/PremiumFreightApproval/AutorizarRechazar/" + pFA.id);

            return body;
        }

        //metodo para el body de una rechazada
        public string getBodyPFARechazado(PFA pFA)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PFA_solicitud_rechazada.html"));

            body = body.Replace("#AUTORIZADOR", pFA.empleados.ConcatNombre);
            body = body.Replace("#USUARIO", pFA.empleados1.ConcatNombre);
            body = body.Replace("#NUM_PFA", pFA.id.ToString());
            body = body.Replace("#REASON_PFA", pFA.PFA_Reason.descripcion);
            body = body.Replace("#TYPE_SHIPMENT", pFA.PFA_Type_shipment.descripcion);
            body = body.Replace("#RESPONSIBLE_PFA", pFA.PFA_Responsible_cost.descripcion);
            body = body.Replace("#SAP_PART_NUMBER", pFA.sap_part_number);
            body = body.Replace("#CUSTOMER_PART_NUMBER", pFA.customer_part_number);
            body = body.Replace("#TOTAL_PF_COST", "$ " + pFA.total_pf_cost.ToString());
            body = body.Replace("#TOTAL_ORIGINAL_COST", "$ " + pFA.total_cost.ToString());
            body = body.Replace("#TOTAL_COST_TO_RECOVER", "$ " + pFA.TotalCostToRecover.ToString());
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            body = body.Replace("#ENLACE", domainName + "/PremiumFreightApproval/RechazadasCapturista/" + pFA.id);

            return body;
        }


        //metodo para el body de una rechazada
        public string getBodyPFARechazadoInfo(PFA pFA)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PFA_solicitud_rechazada_info.html"));

            body = body.Replace("#AUTORIZADOR", pFA.empleados.ConcatNombre);
            body = body.Replace("#USUARIO", pFA.empleados1.ConcatNombre);
            body = body.Replace("#NUM_PFA", pFA.id.ToString());
            body = body.Replace("#REASON_PFA", pFA.PFA_Reason.descripcion);
            body = body.Replace("#TYPE_SHIPMENT", pFA.PFA_Type_shipment.descripcion);
            body = body.Replace("#RESPONSIBLE_PFA", pFA.PFA_Responsible_cost.descripcion);
            body = body.Replace("#SAP_PART_NUMBER", pFA.sap_part_number);
            body = body.Replace("#CUSTOMER_PART_NUMBER", pFA.customer_part_number);
            body = body.Replace("#TOTAL_PF_COST", "$ " + pFA.total_pf_cost.ToString());
            body = body.Replace("#TOTAL_ORIGINAL_COST", "$ " + pFA.total_cost.ToString());
            body = body.Replace("#TOTAL_COST_TO_RECOVER", "$ " + pFA.TotalCostToRecover.ToString());
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            body = body.Replace("#ENLACE", domainName + "/PremiumFreightApproval/Details/" + pFA.id);

            return body;
        }

        //metodo para el body de una remiones PFA Authorizada
        public string getBodyPFAAutorizado(PFA pFA)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PFA_solicitud_autorizada.html"));

            body = body.Replace("#AUTORIZADOR", pFA.empleados.ConcatNombre);
            body = body.Replace("#USUARIO", pFA.empleados1.ConcatNombre);
            body = body.Replace("#NUM_PFA", pFA.id.ToString());
            body = body.Replace("#REASON_PFA", pFA.PFA_Reason.descripcion);
            body = body.Replace("#TYPE_SHIPMENT", pFA.PFA_Type_shipment.descripcion);
            body = body.Replace("#RESPONSIBLE_PFA", pFA.PFA_Responsible_cost.descripcion);
            body = body.Replace("#SAP_PART_NUMBER", pFA.sap_part_number);
            body = body.Replace("#CUSTOMER_PART_NUMBER", pFA.customer_part_number);
            body = body.Replace("#TOTAL_PF_COST", "$ " + pFA.total_pf_cost.ToString());
            body = body.Replace("#TOTAL_ORIGINAL_COST", "$ " + pFA.total_cost.ToString());
            body = body.Replace("#TOTAL_COST_TO_RECOVER", "$ " + pFA.TotalCostToRecover.ToString());
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            body = body.Replace("#ENLACE", domainName + "/PremiumFreightApproval/AutorizadasCapturista/" + pFA.id);

            return body;
        }

        //metodo para el body de una remiones PFA Authorizada
        public string getBodyPFAAutorizadoInfo(PFA pFA)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PFA_solicitud_autorizada_info.html"));

            body = body.Replace("#AUTORIZADOR", pFA.empleados.ConcatNombre);
            body = body.Replace("#USUARIO", pFA.empleados1.ConcatNombre);
            body = body.Replace("#NUM_PFA", pFA.id.ToString());
            body = body.Replace("#REASON_PFA", pFA.PFA_Reason.descripcion);
            body = body.Replace("#TYPE_SHIPMENT", pFA.PFA_Type_shipment.descripcion);
            body = body.Replace("#RESPONSIBLE_PFA", pFA.PFA_Responsible_cost.descripcion);
            body = body.Replace("#SAP_PART_NUMBER", pFA.sap_part_number);
            body = body.Replace("#CUSTOMER_PART_NUMBER", pFA.customer_part_number);
            body = body.Replace("#TOTAL_PF_COST", "$ " + pFA.total_pf_cost.ToString());
            body = body.Replace("#TOTAL_ORIGINAL_COST", "$ " + pFA.total_cost.ToString());
            body = body.Replace("#TOTAL_COST_TO_RECOVER", "$ " + pFA.TotalCostToRecover.ToString());
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            body = body.Replace("#ENLACE", domainName + "/PremiumFreightApproval/Details/" + pFA.id);

            return body;
        }

        #endregion

        #region correos Póliza Manual
        //metodo para obtener el body de Poliza Manual enviada a Validador
        public string getBodyPMSendValidador(poliza_manual poliza)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PM_envio_a_validador.html"));

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#USUARIO", poliza.empleados3.ConcatNombre); //elaborador
            body = body.Replace("#ELABORA", poliza.empleados3.ConcatNombre); //elaborador
            body = body.Replace("#NUM_PM", poliza.id.ToString());
            body = body.Replace("#TIPO_PM", poliza.PM_tipo_poliza.descripcion);
            body = body.Replace("#PLANTA", poliza.plantas.descripcion);
            body = body.Replace("#MONEDA", poliza.currency.CocatCurrency);
            body = body.Replace("#FECHA_DOCUMENTO", poliza.fecha_documento.ToString("dd/MM/yyyy"));
            body = body.Replace("#DESCRIPCION_PM", poliza.descripcion_poliza);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/PolizaManual/ValidarArea/" + poliza.id);

            return body;
        }

        /// <summary>
        /// Obtiene el body de envio a Autorizador
        /// </summary>
        /// <param name="poliza"></param>
        /// <returns></returns>
        public string getBodyPMSendAutorizador(poliza_manual poliza)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PM_envio_a_autorizador.html"));

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#USUARIO", poliza.empleados4.ConcatNombre); //validador
            body = body.Replace("#ELABORA", poliza.empleados3.ConcatNombre); //elaborador
            body = body.Replace("#NUM_PM", poliza.id.ToString());
            body = body.Replace("#TIPO_PM", poliza.PM_tipo_poliza.descripcion);
            body = body.Replace("#PLANTA", poliza.plantas.descripcion);
            body = body.Replace("#MONEDA", poliza.currency.CocatCurrency);
            body = body.Replace("#FECHA_DOCUMENTO", poliza.fecha_documento.ToString("dd/MM/yyyy"));
            body = body.Replace("#DESCRIPCION_PM", poliza.descripcion_poliza);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/PolizaManual/AutorizarControlling/" + poliza.id);

            return body;
        }
        /// <summary>
        /// Obtiene el body de envio a Direccion
        /// </summary>
        /// <param name="poliza"></param>
        /// <returns></returns>
        public string getBodyPMSendDireccion(poliza_manual poliza)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PM_envio_a_direccion.html"));

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#USUARIO", poliza.empleados.ConcatNombre); //autorizador
            body = body.Replace("#ELABORA", poliza.empleados3.ConcatNombre); //elaborador
            body = body.Replace("#NUM_PM", poliza.id.ToString());
            body = body.Replace("#TIPO_PM", poliza.PM_tipo_poliza.descripcion);
            body = body.Replace("#PLANTA", poliza.plantas.descripcion);
            body = body.Replace("#MONEDA", poliza.currency.CocatCurrency);
            body = body.Replace("#FECHA_DOCUMENTO", poliza.fecha_documento.ToString("dd/MM/yyyy"));
            body = body.Replace("#DESCRIPCION_PM", poliza.descripcion_poliza);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/PolizaManual/AutorizarDireccion/" + poliza.id);

            return body;
        }

        /// <summary>
        /// Obtiene el body de notificacion a contabilidad
        /// </summary>
        /// <param name="poliza"></param>
        /// <returns></returns>
        public string getBodyPMSendContabilidad(poliza_manual poliza)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PM_envio_a_contabilidad.html"));

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#USUARIO", poliza.empleados.ConcatNombre); //autorizador
            body = body.Replace("#ELABORA", poliza.empleados3.ConcatNombre); //elaborador
            body = body.Replace("#NUM_PM", poliza.id.ToString());
            body = body.Replace("#TIPO_PM", poliza.PM_tipo_poliza.descripcion);
            body = body.Replace("#PLANTA", poliza.plantas.descripcion);
            body = body.Replace("#MONEDA", poliza.currency.CocatCurrency);
            body = body.Replace("#FECHA_DOCUMENTO", poliza.fecha_documento.ToString("dd/MM/yyyy"));
            body = body.Replace("#DESCRIPCION_PM", poliza.descripcion_poliza);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/PolizaManual/FinalizarContabilidad/" + poliza.id);

            return body;
        }

        //metodo para obtener el body de Poliza Manual cuando se valida una poliza por el jefe de area
        public string getBodyPMValidadoPorArea(poliza_manual poliza)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PM_autorizacion_validador.html"));

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#VALIDADOR", poliza.empleados4.ConcatNombre);
            body = body.Replace("#ELABORA", poliza.empleados3.ConcatNombre); //elaborador
            body = body.Replace("#NUM_PM", poliza.id.ToString());
            body = body.Replace("#TIPO_PM", poliza.PM_tipo_poliza.descripcion);
            body = body.Replace("#PLANTA", poliza.plantas.descripcion);
            body = body.Replace("#MONEDA", poliza.currency.CocatCurrency);
            body = body.Replace("#FECHA_DOCUMENTO", poliza.fecha_documento.ToString("dd/MM/yyyy"));
            body = body.Replace("#DESCRIPCION_PM", poliza.descripcion_poliza);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/PolizaManual/Details/" + poliza.id);

            return body;
        }

        //metodo para obtener el body de Poliza Manual cuando una Póliza ha sido rechazada
        public string getBodyPMRechazada(poliza_manual poliza, string nombreRechazante)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PM_rechazo.html"));


            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#RECHAZANTE", nombreRechazante);
            body = body.Replace("#ELABORA", poliza.empleados3.ConcatNombre); //elaborador
            body = body.Replace("#NUM_PM", poliza.id.ToString());
            body = body.Replace("#TIPO_PM", poliza.PM_tipo_poliza.descripcion);
            body = body.Replace("#PLANTA", poliza.plantas.descripcion);
            body = body.Replace("#MONEDA", poliza.currency.CocatCurrency);
            body = body.Replace("#FECHA_DOCUMENTO", poliza.fecha_documento.ToString("dd/MM/yyyy"));
            body = body.Replace("#DESCRIPCION_PM", poliza.descripcion_poliza);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#RECHAZO", String.IsNullOrEmpty(poliza.comentario_rechazo) ? String.Empty : poliza.comentario_rechazo);
            body = body.Replace("#ENLACE", domainName + "/PolizaManual/Edit/" + poliza.id);

            return body;
        }

        //metodo para obtener el body de Poliza Manual cuando se valida una poliza por controlling
        public string getBodyPMNotificacionAutorizado(poliza_manual poliza)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PM_autorizacion_controlling.html"));

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#AUTORIZADOR", poliza.empleados.ConcatNombre);
            body = body.Replace("#ELABORA", poliza.empleados3.ConcatNombre); //elaborador
            body = body.Replace("#NUM_PM", poliza.id.ToString());
            body = body.Replace("#TIPO_PM", poliza.PM_tipo_poliza.descripcion);
            body = body.Replace("#PLANTA", poliza.plantas.descripcion);
            body = body.Replace("#MONEDA", poliza.currency.CocatCurrency);
            body = body.Replace("#FECHA_DOCUMENTO", poliza.fecha_documento.ToString("dd/MM/yyyy"));
            body = body.Replace("#DESCRIPCION_PM", poliza.descripcion_poliza);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/PolizaManual/Details/" + poliza.id);

            return body;
        }

        //metodo para obtener el body de Poliza Manual cuando ha sido registrada por contabilidad
        public string getBodyPMRegistradoPorContabilidad(poliza_manual poliza)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PM_confirmacion_registro.html"));

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#USUARIO_CONTABILIDAD", poliza.empleados1.ConcatNombre);
            body = body.Replace("#ELABORA", poliza.empleados3.ConcatNombre); //elaborador
            body = body.Replace("#NUM_PM", poliza.id.ToString());
            body = body.Replace("#DOCUMENTO_SAP", poliza.numero_documento_sap);
            body = body.Replace("#TIPO_PM", poliza.PM_tipo_poliza.descripcion);
            body = body.Replace("#PLANTA", poliza.plantas.descripcion);
            body = body.Replace("#MONEDA", poliza.currency.CocatCurrency);
            body = body.Replace("#FECHA_DOCUMENTO", poliza.fecha_documento.ToString("dd/MM/yyyy"));
            body = body.Replace("#DESCRIPCION_PM", poliza.descripcion_poliza);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/PolizaManual/Details/" + poliza.id);

            return body;
        }

        #endregion

        #region Ordenes Trabajo

        /// <summary>
        /// metodo para obtener el body de email al crear OT
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string getBodyCreacionOT(orden_trabajo item)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/OT_creacion.html"));

            string tpm = "NO";

            if (item.tpm)
                tpm = "SÍ";

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#SOLICITANTE", item.empleados2.ConcatNombre); //elaborador
            body = body.Replace("#NUM_OT", item.id.ToString()); //elaborador
            body = body.Replace("#NIVEL_URGENCIA", OT_nivel_urgencia.DescripcionStatus(item.nivel_urgencia));
            body = body.Replace("#TITULO", item.titulo);
            body = body.Replace("#DESCRIPCION", item.descripcion);
            body = body.Replace("#DEPARTAMENTO", item.empleados2.Area.descripcion);
            body = body.Replace("#TPM", tpm);
            body = body.Replace("#FECHA_SOLICITUD", item.fecha_solicitud.ToString("dd/MM/yyyy"));
            body = body.Replace("#ENLACE", domainName + "/OrdenesTrabajo/Asignar/" + item.id);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;
        }

        /// <summary>
        /// metodo para obtener el body de email al asignar OT
        /// </summary>
        /// <param name="orden"></param>
        /// <returns></returns>
        public string getBodyAsignacionOT(orden_trabajo orden)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/OT_asignacion.html"));

            string tpm = "NO";

            if (orden.tpm)
                tpm = "SÍ";

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#SOLICITANTE", orden.empleados2.ConcatNombre); //elaborador
            body = body.Replace("#RESPONSABLE", orden.empleados1.ConcatNombre); //elaborador
            body = body.Replace("#NUM_OT", orden.id.ToString()); //elaborador
            body = body.Replace("#NIVEL_URGENCIA", OT_nivel_urgencia.DescripcionStatus(orden.nivel_urgencia));
            body = body.Replace("#TITULO", orden.titulo);
            body = body.Replace("#DESCRIPCION", orden.descripcion);
            body = body.Replace("#DEPARTAMENTO", orden.empleados2.Area.descripcion);
            body = body.Replace("#TPM", tpm);
            body = body.Replace("#FECHA_SOLICITUD", orden.fecha_solicitud.ToString("dd/MM/yyyy"));
            body = body.Replace("#ENLACE", domainName + "/OrdenesTrabajo/CambiarEstatus/" + orden.id);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;
        }

        /// <summary>
        /// metodo para obtener el body de email al marcar como en proceso OT
        /// </summary>
        /// <param name="orden"></param>
        /// <returns></returns>
        public string getBodyEnProcesoOT(orden_trabajo orden)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/OT_en_proceso.html"));

            string tpm = "NO";

            if (orden.tpm)
                tpm = "SÍ";

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#SOLICITANTE", orden.empleados2.ConcatNombre); //elaborador
            body = body.Replace("#RESPONSABLE", orden.empleados1.ConcatNombre); //elaborador
            body = body.Replace("#NUM_OT", orden.id.ToString()); //elaborador
            body = body.Replace("#NIVEL_URGENCIA", OT_nivel_urgencia.DescripcionStatus(orden.nivel_urgencia));
            body = body.Replace("#TITULO", orden.titulo);
            body = body.Replace("#DESCRIPCION", orden.descripcion);
            body = body.Replace("#DEPARTAMENTO", orden.empleados2.Area.descripcion);
            body = body.Replace("#TPM", tpm);
            body = body.Replace("#FECHA_SOLICITUD", orden.fecha_solicitud.ToString("dd/MM/yyyy"));
            body = body.Replace("#ENLACE", domainName + "/OrdenesTrabajo/Details/" + orden.id);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;
        }

        /// <summary>
        /// metodo para obtener el body de email al cerrar una OT
        /// </summary>
        /// <param name="orden"></param>
        /// <returns></returns>
        public string getBodyCerrarOT(orden_trabajo orden)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/OT_finalizado.html"));

            string tpm = "NO";

            if (orden.tpm)
                tpm = "SÍ";

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#SOLICITANTE", orden.empleados2.ConcatNombre); //elaborador
            body = body.Replace("#RESPONSABLE", orden.empleados1.ConcatNombre); //elaborador
            body = body.Replace("#NUM_OT", orden.id.ToString()); //elaborador
            body = body.Replace("#NIVEL_URGENCIA", OT_nivel_urgencia.DescripcionStatus(orden.nivel_urgencia));
            body = body.Replace("#TITULO", orden.titulo);
            body = body.Replace("#DESCRIPCION", orden.descripcion);
            body = body.Replace("#DEPARTAMENTO", orden.empleados2.Area.descripcion);
            body = body.Replace("#TPM", tpm);
            body = body.Replace("#FECHA_SOLICITUD", orden.fecha_solicitud.ToString("dd/MM/yyyy"));
            body = body.Replace("#ENLACE", domainName + "/OrdenesTrabajo/Details/" + orden.id);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;
        }


        #endregion

        #region Matriz de Requerimientos


        /// <summary>
        /// metodo para obtener el body de email para el jefe directo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string getBody_IT_MR_Notificacion_Jefe_Directo(IT_matriz_requerimientos item)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/IT_MR_notificacion_jefe.html"));


            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#SOLICITANTE", item.empleados3.ConcatNombre); //elaborador
            body = body.Replace("#ID", item.id.ToString()); //elaborador
            body = body.Replace("#EMPLEADO", item.empleados.ConcatNombre);
            body = body.Replace("#PUESTO", item.empleados.puesto1.descripcion);
            body = body.Replace("#COMENTARIO", item.comentario);
            body = body.Replace("#FECHA_SOLICITUD", item.fecha_solicitud.ToString("dd/MM/yyyy"));
            body = body.Replace("#ENLACE", domainName + "/IT_matriz_requerimientos/Autorizar/" + item.id);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;

        }

        /// <summary>
        /// metodo para obtener el body de email de notificación de autorización para solicitante
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string getBody_IT_MR_Notificacion_Autorizado_Solicitante(IT_matriz_requerimientos item)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/IT_MR_notificacion_autorizacion_solicitante.html"));


            body = body.Replace("#JEFE", item.empleados1.ConcatNombre); //elaborador
            body = body.Replace("#ID", item.id.ToString()); //jefe
            body = body.Replace("#ESTADO", IT_MR_Status.DescripcionStatus(item.estatus));
            body = body.Replace("#EMPLEADO", item.empleados.ConcatNombre);
            body = body.Replace("#PLANTA", item.empleados.plantas != null ? item.empleados.plantas.descripcion : String.Empty);
            body = body.Replace("#DEPARTAMENTO", item.empleados.Area != null ? item.empleados.Area.descripcion : String.Empty);
            body = body.Replace("#PUESTO", item.empleados.puesto1 != null ? item.empleados.puesto1.descripcion : String.Empty);
            body = body.Replace("#COMENTARIO", item.comentario);
            body = body.Replace("#FECHA_SOLICITUD", item.fecha_solicitud.ToString("dd/MM/yyyy"));
            body = body.Replace("#ENLACE", domainName + "/IT_matriz_requerimientos/Details/" + item.id);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;

        }

        /// <summary>
        /// metodo para obtener el body de email de notificación de autorización para sistemas
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string getBody_IT_MR_Notificacion_Autorizado_Sistemas(IT_matriz_requerimientos item)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/IT_MR_notificacion_autorizacion_sistemas.html"));


            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#JEFE", item.empleados1.ConcatNombre); //jefe
            body = body.Replace("#ESTADO", IT_MR_Status.DescripcionStatus(item.estatus));
            body = body.Replace("#ID", item.id.ToString()); //elaborador
            body = body.Replace("#EMPLEADO", item.empleados.ConcatNombre);
            body = body.Replace("#PLANTA", item.empleados.plantas != null ? item.empleados.plantas.descripcion : String.Empty);
            body = body.Replace("#DEPARTAMENTO", item.empleados.Area != null ? item.empleados.Area.descripcion : String.Empty);
            body = body.Replace("#PUESTO", item.empleados.puesto1 != null ? item.empleados.puesto1.descripcion : String.Empty);
            body = body.Replace("#COMENTARIO", item.comentario);
            body = body.Replace("#FECHA_SOLICITUD", item.fecha_solicitud.ToString("dd/MM/yyyy"));
            body = body.Replace("#ENLACE", domainName + "/IT_matriz_requerimientos/Cerrar/" + item.id);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;

        }

        /// <summary>
        /// metodo para obtener el body de email de notificación de rechazo para solicitante
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string getBody_IT_MR_Notificacion_Rechazado_Solicitante(IT_matriz_requerimientos item)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/IT_MR_notificacion_rechazo_solicitante.html"));


            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#SISTEMAS", item.empleados2.ConcatNombre); //sistemas
            body = body.Replace("#ESTADO", IT_MR_Status.DescripcionStatus(item.estatus));
            body = body.Replace("#ID", item.id.ToString()); //elaborador
            body = body.Replace("#EMPLEADO", item.empleados.ConcatNombre);
            body = body.Replace("#PUESTO", item.empleados.puesto1.descripcion);
            body = body.Replace("#COMENTARIO", item.comentario);
            body = body.Replace("#RECHAZO", item.comentario_rechazo);
            body = body.Replace("#FECHA_SOLICITUD", item.fecha_solicitud.ToString("dd/MM/yyyy"));
            body = body.Replace("#ENLACE", domainName + "/IT_matriz_requerimientos/Autorizar/" + item.id);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;

        }

        /// <summary>
        /// metodo para obtener el body de email de notificación de cierre de remiones
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string getBody_IT_MR_Notificacion_Cierre(IT_matriz_requerimientos item)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/IT_MR_notificacion_cierre.html"));


            body = body.Replace("#SISTEMAS", item.empleados2.ConcatNombre); //sistemas
            body = body.Replace("#ESTADO", IT_MR_Status.DescripcionStatus(item.estatus));
            body = body.Replace("#ID", item.id.ToString()); //elaborador
            body = body.Replace("#EMPLEADO", item.empleados.ConcatNombre);
            body = body.Replace("#PUESTO", item.empleados.puesto1.descripcion);
            body = body.Replace("#COMENTARIO", item.comentario);
            body = body.Replace("#CIERRE", item.comentario_cierre);
            body = body.Replace("#8ID", item.empleados.C8ID);
            body = body.Replace("#CORREO", item.empleados.correo);
            body = body.Replace("#FECHA_SOLICITUD", item.fecha_solicitud.ToString("dd/MM/yyyy"));
            body = body.Replace("#ENLACE", domainName + "/IT_matriz_requerimientos/Details/" + item.id);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;

        }

        /// <summary>
        /// metodo para obtener el body de email de notificación en proceso de remiones
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string getBody_IT_MR_Notificacion_En_Proceso(IT_matriz_requerimientos item)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/IT_MR_notificacion_en_proceso.html"));


            body = body.Replace("#SISTEMAS", item.empleados2.ConcatNombre); //sistemas
            body = body.Replace("#ESTADO", IT_MR_Status.DescripcionStatus(item.estatus));
            body = body.Replace("#ID", item.id.ToString()); //elaborador
            body = body.Replace("#EMPLEADO", item.empleados.ConcatNombre);
            body = body.Replace("#PUESTO", item.empleados.puesto1.descripcion);
            body = body.Replace("#COMENTARIO", item.comentario);
            body = body.Replace("#8ID", item.empleados.C8ID);
            body = body.Replace("#CORREO", item.empleados.correo);
            body = body.Replace("#FECHA_SOLICITUD", item.fecha_solicitud.ToString("dd/MM/yyyy"));
            body = body.Replace("#ENLACE", domainName + "/IT_matriz_requerimientos/Details/" + item.id);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;

        }


        public string getBodyAsignacionITMatrizRequerimientos(IT_matriz_requerimientos matriz, string comentario_asignacion, string nombre_asigna)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Notificacion_solicitud_generica.html"));
            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>();

            //agrega los valores al diccionario
            tablaContentDictionary.Add("Folio", matriz.id.ToString());
            tablaContentDictionary.Add("Solicitante", matriz.empleados3.ConcatNombre);
            tablaContentDictionary.Add("Empleado", matriz.empleados.ConcatNombre);
            tablaContentDictionary.Add("Puesto", matriz.empleados.puesto1.descripcion);
            tablaContentDictionary.Add("Departamento", matriz.empleados.Area.descripcion);
            tablaContentDictionary.Add("Planta", matriz.empleados.plantas.descripcion);
            tablaContentDictionary.Add("Comentario", matriz.comentario);
            tablaContentDictionary.Add("Comentario Asignación", comentario_asignacion);

            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla
            body = body.Replace("#TITULO", "¡Se te ha asignado la matriz de requerimientos #" + matriz.id + "!");
            body = body.Replace("#SUBTITULO", "El usuario " + nombre_asigna + " te ha asignado la matriz de requerimientos #" + matriz.id + ". Para ver los detalles de la matriz de requerimientos, haga clic en el siguiente enlace."); //elaborador
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/IT_matriz_requerimientos/Cerrar/" + matriz.id);



            return body;
        }

        #endregion

        #region account

        /// <summary>
        /// metodo para obtener el body de email de notificación de restablecimiento de contraseña
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string getBodyAccountResetPassword(string enlace)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Account_reset_password.html"));

            body = body.Replace("#ENLACE", enlace);

            return body;

        }

        /// <summary>
        /// metodo para obtener el body de email de notificación para la creación de un usuario
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string getBodyAccountWelcome(RegisterViewModel model)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Account_welcome.html"));

            empleados emp = db.empleados.Find(model.IdEmpleado);
            string nombre = "NO DISPONIBLE";

            if (emp != null)
                nombre = emp.ConcatNombre;
            else
                nombre = model.Nombre;



            body = body.Replace("#NOMBRE", nombre); //usuario creado
            body = body.Replace("#USER", model.Email);
            body = body.Replace("#PASS", model.Password); //elaborador         
            body = body.Replace("#ENLACE", domainName + "/Home/");
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;
        }

        /// <summary>
        /// metodo para obtener el body de email de notificación para la creación de un usuario
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string getBodyAccountChangeEmail(empleados emp, string correoAnterior)
        {


            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Account_change_email.html"));


            string nombre = "NO DISPONIBLE";

            if (emp != null)
                nombre = emp.ConcatNombre;



            body = body.Replace("#NOMBRE", nombre); //usuario creado
            body = body.Replace("#CORREO", emp.correo);
            body = body.Replace("#CANTERIOR", correoAnterior);
            body = body.Replace("#ENLACE", domainName + "/Home/");
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;
        }

        /// <summary>
        /// metodo para obtener el body de notificacion de creacion de usuario
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string getBodySolicitudUsuarioPortal(IT_solicitud_usuarios item)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/IT_solicitud_usuario_portal.html"));

            string solicitante = item.empleados != null ? item.empleados.ConcatNombre : item.nombre + " " + item.apellido1 + " " + item.apellido2;
            string planta = item.empleados != null && item.empleados.plantas != null ? item.empleados.plantas.descripcion : item.plantas != null ? item.plantas.descripcion : string.Empty;
            string correo = item.empleados != null ? item.empleados.correo : item.correo;

            body = body.Replace("#SOLICITANTE", solicitante);
            body = body.Replace("#ID", item.id.ToString()); //elaborador
            body = body.Replace("#PLANTA", planta);
            body = body.Replace("#COMENTARIO", item.comentario);
            body = body.Replace("#CORREO", correo);
            body = body.Replace("#FECHA_SOLICITUD", item.fecha_solicitud.ToString("dd/MM/yyyy"));
            body = body.Replace("#ENLACE", domainName + "/IT_solicitud_usuarios");
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;

        }

        #endregion

        #region Remisiones Manuales
        public string getBodyRemisiones(RM_cabecera remiones, string asunto)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Notificacion_solicitud_generica.html"));
            string tablaContenido = String.Empty;

            //obtiene el ultimo cambio
            var cambio = remiones.RM_cambio_estatus.LastOrDefault();

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>();

            //agrega los valores al diccionario
            tablaContentDictionary.Add("Remisión Número", remiones.ConcatNumeroRemision);
            tablaContentDictionary.Add("Fecha Creación", remiones.FechaCreacion.ToString());
            tablaContentDictionary.Add("Cliente", !string.IsNullOrEmpty(remiones.clienteOtro) ? remiones.clienteOtro : "N/A");
            tablaContentDictionary.Add("Proveedor", !String.IsNullOrEmpty(remiones.proveedorOtro) ? remiones.proveedorOtro : "N/A");
            tablaContentDictionary.Add("Enviado A", remiones.enviadoAOtro);
            tablaContentDictionary.Add("Transporte", remiones.transporteOtro);
            tablaContentDictionary.Add("Usuario", cambio.empleados.ConcatNombre);
            tablaContentDictionary.Add("Movimiento", cambio.RM_estatus.descripcion);
            tablaContentDictionary.Add("Detallles", cambio.texto);

            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla
            body = body.Replace("#TITULO", "¡Hola! " + asunto + ":");
            body = body.Replace("#SUBTITULO", "El usuario " + cambio.empleados.ConcatNombre + ", ha creado o realizado un cambio en la remisión " + remiones.ConcatNumeroRemision + ". Haga clic en el enlace para más información."); //elaborador
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/RM_cabecera/Details/" + remiones.clave);

            return body;
        }
        #endregion

        #region Gastos de Viaje

        //metodo para obtener el body de Gastos de Viaje cuando ha sido enviada a jefeDirecto
        public string getBodyGVNotificacionEnvioJefe(GV_solicitud solicitud)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Notificacion_solicitud_generica.html"));
            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>();

            //agrega los valores al diccionario
            tablaContentDictionary.Add("Folio", solicitud.id.ToString());
            tablaContentDictionary.Add("Solicitante", solicitud.empleados5.ConcatNombre);
            tablaContentDictionary.Add("Empleado", solicitud.empleados2.ConcatNombre);
            tablaContentDictionary.Add("Origen", solicitud.origen);
            tablaContentDictionary.Add("Destino", solicitud.destino);
            tablaContentDictionary.Add("Motivo del viaje", solicitud.motivo_viaje);
            if (!String.IsNullOrEmpty(solicitud.comentario_adicional))
                tablaContentDictionary.Add("Comentarios Adicionales", solicitud.comentario_adicional);

            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla
            body = body.Replace("#TITULO", "¡Hola, ha recibido una remiones de Anticipo de Gastos de Viaje para su Autorización!");
            body = body.Replace("#SUBTITULO", "El usuario " + solicitud.empleados5.ConcatNombre + ", ha enviado una remiones de Anticipo de Gastos de Viaje. Haga clic en el enlace para más información."); //elaborador
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/GV_solicitud/AutorizarJefeDirecto/" + solicitud.id);

            return body;
        }
        public string getBodyGVNotificacionRechazo(GV_solicitud solicitud, string nombre)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Notificacion_solicitud_generica.html"));
            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>();

            //agrega los valores al diccionario
            tablaContentDictionary.Add("Folio", solicitud.id.ToString());
            tablaContentDictionary.Add("Estatus", Bitacoras.Util.GV_solicitud_estatus.DescripcionStatus(solicitud.estatus));
            tablaContentDictionary.Add("Solicitante", solicitud.empleados5.ConcatNombre);
            tablaContentDictionary.Add("Empleado", solicitud.empleados2.ConcatNombre);
            tablaContentDictionary.Add("Origen", solicitud.origen);
            tablaContentDictionary.Add("Destino", solicitud.destino);
            tablaContentDictionary.Add("Motivo del viaje", solicitud.motivo_viaje);

            if (!String.IsNullOrEmpty(solicitud.comentario_rechazo))
                tablaContentDictionary.Add("Comentarios Adicionales", solicitud.comentario_rechazo);

            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla
            body = body.Replace("#TITULO", "¡Hola, su remiones de Anticipo de Gastos de Viaje ha sido rechazada!");
            body = body.Replace("#SUBTITULO", "El usuario " + nombre + ", ha rechazado su remiones de Anticipo de Gastos de Viaje. Verifique los datos y envíela nuevamente."); //elaborador
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/GV_solicitud/Edit/" + solicitud.id);

            return body;
        }

        public string getBodyGVNotificacionRechazadoPorUsuario(GV_solicitud solicitud)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Notificacion_solicitud_generica.html"));
            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>();

            //agrega los valores al diccionario
            tablaContentDictionary.Add("Folio", solicitud.id.ToString());
            tablaContentDictionary.Add("Estatus", Bitacoras.Util.GV_solicitud_estatus.DescripcionStatus(solicitud.estatus));
            tablaContentDictionary.Add("Solicitante", solicitud.empleados5.ConcatNombre);
            tablaContentDictionary.Add("Empleado", solicitud.empleados2.ConcatNombre);
            tablaContentDictionary.Add("Origen", solicitud.origen);
            tablaContentDictionary.Add("Destino", solicitud.destino);
            tablaContentDictionary.Add("Motivo del viaje", solicitud.motivo_viaje);

            if (!String.IsNullOrEmpty(solicitud.comentario_rechazo))
                tablaContentDictionary.Add("Comentarios Adicionales", solicitud.comentario_rechazo);

            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla
            body = body.Replace("#TITULO", "¡Hola, el deposito de la remiones de Anticipo de Gastos de Viaje ha sido rechazada!");
            body = body.Replace("#SUBTITULO", "El usuario " + solicitud.empleados5.ConcatNombre + ", no ha confirmado el deposito de la remiones #" + solicitud.id + "."); //elaborador
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/GV_solicitud/AutorizarNomina/" + solicitud.id);

            return body;
        }
        public string getBodyGVNotificacionEnvioControlling(GV_solicitud solicitud)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Notificacion_solicitud_generica.html"));
            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>();

            //agrega los valores al diccionario
            tablaContentDictionary.Add("Folio", solicitud.id.ToString());
            tablaContentDictionary.Add("Estatus", Bitacoras.Util.GV_solicitud_estatus.DescripcionStatus(solicitud.estatus));
            tablaContentDictionary.Add("Solicitante", solicitud.empleados5.ConcatNombre);
            tablaContentDictionary.Add("Empleado", solicitud.empleados2.ConcatNombre);
            tablaContentDictionary.Add("Origen", solicitud.origen);
            tablaContentDictionary.Add("Destino", solicitud.destino);
            tablaContentDictionary.Add("Motivo del viaje", solicitud.motivo_viaje);

            if (!String.IsNullOrEmpty(solicitud.comentario_rechazo))
                tablaContentDictionary.Add("Comentarios Adicionales", solicitud.comentario_rechazo);

            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla
            body = body.Replace("#TITULO", "¡Hola, ha recibido una remiones de Anticipo de Gastos de viaje!");
            body = body.Replace("#SUBTITULO", "El usuario " + solicitud.empleados3.ConcatNombre + ", ha autorizado una remiones de Anticipo de Gastos de Viaje."); //elaborador
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/GV_solicitud/AutorizarControlling/" + solicitud.id);

            return body;
        }
        public string getBodyGVNotificacionEnvioNominas(GV_solicitud solicitud)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Notificacion_solicitud_generica.html"));
            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>();

            //agrega los valores al diccionario
            tablaContentDictionary.Add("Folio", solicitud.id.ToString());
            tablaContentDictionary.Add("Estatus", Bitacoras.Util.GV_solicitud_estatus.DescripcionStatus(solicitud.estatus));
            tablaContentDictionary.Add("Solicitante", solicitud.empleados5.ConcatNombre);
            tablaContentDictionary.Add("Empleado", solicitud.empleados2.ConcatNombre);
            tablaContentDictionary.Add("Origen", solicitud.origen);
            tablaContentDictionary.Add("Destino", solicitud.destino);
            tablaContentDictionary.Add("Motivo del viaje", solicitud.motivo_viaje);

            if (!String.IsNullOrEmpty(solicitud.comentario_rechazo))
                tablaContentDictionary.Add("Comentarios Adicionales", solicitud.comentario_rechazo);

            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla
            body = body.Replace("#TITULO", "¡Hola, ha recibido una remiones de Anticipo de Gastos de viaje!");
            body = body.Replace("#SUBTITULO", "El usuario " + solicitud.empleados1.ConcatNombre + ", ha autorizado una remiones de Anticipo de Gastos de Viaje."); //elaborador
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/GV_solicitud/AutorizarNomina/" + solicitud.id);

            return body;
        }

        public string getBodyGVNotificacionEnvioConfirmacionDeposito(GV_solicitud solicitud, string metodo)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Notificacion_solicitud_generica.html"));
            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>();

            //agrega los valores al diccionario
            tablaContentDictionary.Add("Folio", solicitud.id.ToString());
            tablaContentDictionary.Add("Estatus", Bitacoras.Util.GV_solicitud_estatus.DescripcionStatus(solicitud.estatus));
            tablaContentDictionary.Add("Solicitante", solicitud.empleados5.ConcatNombre);
            tablaContentDictionary.Add("Empleado", solicitud.empleados2.ConcatNombre);
            tablaContentDictionary.Add("Origen", solicitud.origen);
            tablaContentDictionary.Add("Destino", solicitud.destino);
            tablaContentDictionary.Add("Motivo del viaje", solicitud.motivo_viaje);

            if (!String.IsNullOrEmpty(solicitud.comentario_rechazo))
                tablaContentDictionary.Add("Comentarios Adicionales", solicitud.comentario_rechazo);

            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla
            body = body.Replace("#TITULO", "¡Se ha confirmado el deposito de la remiones de Anticipo de Gastos de Viaje #" + solicitud.id + "!");
            body = body.Replace("#SUBTITULO", "El usuario " + solicitud.empleados4.ConcatNombre + ", ha confirmado el depósito de la remiones de Anticipo de Gastos de Viaje #" + solicitud.id + "."); //elaborador
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/GV_solicitud/" + metodo + "/" + solicitud.id);

            return body;
        }
        public string getBodyGVNotificacionFinalizado(GV_solicitud solicitud)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Notificacion_solicitud_generica.html"));
            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>();

            //agrega los valores al diccionario
            tablaContentDictionary.Add("Folio", solicitud.id.ToString());
            tablaContentDictionary.Add("Estatus", Bitacoras.Util.GV_solicitud_estatus.DescripcionStatus(solicitud.estatus));
            tablaContentDictionary.Add("Solicitante", solicitud.empleados5.ConcatNombre);
            tablaContentDictionary.Add("Empleado", solicitud.empleados2.ConcatNombre);
            tablaContentDictionary.Add("Origen", solicitud.origen);
            tablaContentDictionary.Add("Destino", solicitud.destino);
            tablaContentDictionary.Add("Motivo del viaje", solicitud.motivo_viaje);

            if (!String.IsNullOrEmpty(solicitud.comentario_rechazo))
                tablaContentDictionary.Add("Comentarios Adicionales", solicitud.comentario_rechazo);

            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla
            body = body.Replace("#TITULO", "¡Se ha finalizado el proceso de registro de la remiones de Anticipo de Gastos de Viaje #" + solicitud.id + "!");
            body = body.Replace("#SUBTITULO", "Se ha completado el proceso de la remiones de Anticipo de Gastos de Viaje #" + solicitud.id + ". Haga clic en el siguiente enlace para ver los detalles."); //elaborador
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/GV_solicitud/details/" + solicitud.id);

            return body;
        }


        #endregion

        #region IT
        public string getBodyITBajaEmpleado(empleados empleado)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Notificacion_solicitud_generica.html"));
            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>();

            //agrega los valores al diccionario
            tablaContentDictionary.Add("Núm. empleado", empleado.numeroEmpleado);
            tablaContentDictionary.Add("Empleado", empleado.ConcatNombre);
            tablaContentDictionary.Add("Puesto", empleado.puesto1 != null ? empleado.puesto1.descripcion : String.Empty);
            tablaContentDictionary.Add("Departamento", empleado.Area != null ? empleado.Area.descripcion : String.Empty);
            tablaContentDictionary.Add("Planta", empleado.plantas != null ? empleado.plantas.descripcion : String.Empty);


            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla
            body = body.Replace("#TITULO", "¡Se ha dado de baja al empleado " + empleado.ConcatNombre + "!");
            body = body.Replace("#SUBTITULO", "Recurso Humanos ha dado de baja al empleado " + empleado.ConcatNombre + " y ha solicitado su baja en las distintas plataformas. Para ver los detalles de las asignaciones actuales del empleado, haga clic en el siguiente enlace."); //elaborador
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/it_asignacion_hardware/DetailsEmpleado/" + empleado.id);

            return body;
        }

        public string getBodyActividadPendienteMatrizRequerimientos(IT_matriz_requerimientos solicitud, string tipo, string tipo_nombre, string nombre, string mensaje)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/Notificacion_solicitud_generica.html"));
            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>
            {
                //agrega los valores al diccionario
                { "Folio", solicitud.id.ToString() },
                { "Empleado", solicitud.empleados.ConcatNombre },
                { "Puesto", solicitud.empleados.puesto1.descripcion },
                { tipo, tipo_nombre },
                { "Comentario", mensaje }
            };


            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla
            body = body.Replace("#TITULO", "¡Hola, tienes una actividad de Matriz de Requerimientos asignada!");
            body = body.Replace("#SUBTITULO", "El usuario " + nombre + " te ha asignado una actividad en la matriz de requerimientos."); //elaborador
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/IT_matriz_requerimientos/Cerrar/" + solicitud.id);

            return body;
        }


        #endregion

        #region SCDM
        public string getBodySCDMActividad(SCDM_tipo_correo_notificacionENUM tipo, empleados usuarioLogeado, SCDM_solicitud solicitud, SCDM_tipo_view_edicionENUM vista, String departamento = "", SCDM_tipo_correo_notificacionENUM tipoNotificacionUsuario = SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_INICIAL,
            string comentario = "", int id_departamento = 0, string comentarioRechazo = "", string motivoAsignacionIncorrecta = "", string comentarioAsignacionIncorrecta = "")
        {

            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string mensajeSaludo = string.Empty;
            string mensajeMain = string.Empty;
            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/SCDM_actividad.html"));

            //crea el enlace que se enviará al final del correo
            string enlace = domainName + "/SCDM_solicitud/EditarSolicitud/" + solicitud.id + "?viewUser=" + (int)vista; //edición por defecto

            List<DateTime> DateRechazos = solicitud.SCDM_solicitud_asignaciones.Where(x => x.fecha_rechazo != null).Select(x => x.fecha_rechazo.Value).ToList();

            //obtiene el mensaje inicial
            switch (tipo)
            {
                case SCDM_tipo_correo_notificacionENUM.ENVIA_SOLICITUD:
                    mensajeSaludo = string.Format("¡Hola! {0} ha enviado la solicitud #{1} para tu revisión.", usuarioLogeado.ConcatNombre, solicitud.id);
                    mensajeMain = "Se ha asignado la solictud para tu revisión.";
                    break;
                case SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_INICIAL:
                    mensajeSaludo = string.Format("¡Hola! {0} ha aprobado la solicitud #{1}", usuarioLogeado.ConcatNombre, solicitud.id);
                    mensajeMain = "Se ha aprobado la revisión inicial de la solicitud y ha sido enviada a SCDM para su procesamiento.";
                    break;
                case SCDM_tipo_correo_notificacionENUM.CREACION_MATERIALES_BUDGET:
                    mensajeSaludo = string.Format("¡Hola! {0} se ha creado y enviado la solicitud #{1} de Master Data", usuarioLogeado.ConcatNombre, solicitud.id);
                    mensajeMain = "Puedes revisar los detalles de la solicitud dando clic en el botón al final del correo.";
                    enlace = domainName + "/SCDM_solicitud/Details/" + solicitud.id;
                    break;
                case SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_DEPARTAMENTO_PENDIENTES:
                    mensajeSaludo = string.Format("¡Hola! {0} ha cerrado la actividad de la solicitud #{1}, correspondiente al departamento de {2}.", usuarioLogeado.ConcatNombre, solicitud.id, departamento);
                    mensajeMain = string.Format("Se ha finalizado la actividad por parte del departamento de {0}. Sin embargo, existen otros departamentos con actividades pendientes.", departamento);
                    break;
                case SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_DEPARTAMENTO_FINAL:
                    mensajeSaludo = string.Format("¡Hola! {0} ha cerrado la actividad de la solicitud #{1}, correspondiente al departamento de {2}.", usuarioLogeado.ConcatNombre, solicitud.id, departamento);

                    //valida si la solocitud se encuentra rechazada
                    if (solicitud.SCDM_solicitud_asignaciones.Any(x => (x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE && x.fecha_cierre == null && x.fecha_rechazo == null)
                                || (x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM && x.fecha_cierre == null && x.fecha_rechazo == null && DateRechazos.Contains(x.fecha_asignacion)))
                        )
                    {
                        mensajeMain = string.Format("Se ha finalizado la actividad por parte del departamento de {0}. Todos los departamentos han finalizado su actividad. Sin embargo, la solicitud a sido rechazada previamente. Haz clic en el botón al final del correo, para ver los detalles de las asignaciones.", departamento);
                        enlace = domainName + "/SCDM_solicitud/AsignarTareas/" + solicitud.id; //edición por defecto
                    }
                    else
                        mensajeMain = string.Format("Se ha finalizado la actividad por parte del departamento de {0}. Todos los departamentos han finalizado su actividad. Se asigna la solicitud a SCDM.", departamento);

                    break;

                case SCDM_tipo_correo_notificacionENUM.NOTIFICACION_A_USUARIO:
                    switch (tipoNotificacionUsuario)
                    {
                        //En caso de Aprobación inicial o de departamento.

                        case SCDM_tipo_correo_notificacionENUM.ASIGNACION_SOLICITUD_A_DEPARTAMENTO:
                            mensajeSaludo = string.Format("¡Hola! Tu solicitud #{0} ha sido asignada ", solicitud.id);
                            mensajeMain = "Tu solicitud ha sido asignada a los departamentos:<b> " + departamento + "</b>. Para ver el estatus de la solicitud, haz clic en el botón al final del correo.";
                            break;
                        case SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_INICIAL:
                            mensajeSaludo = string.Format("¡Hola! {0} ha aprobado la solicitud #{1}", usuarioLogeado.ConcatNombre, solicitud.id);
                            mensajeMain = "Se ha aprobado la revisión inicial de la solicitud y ha sido enviada a SCDM para su procesamiento.";
                            break;
                        case SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_DEPARTAMENTO_PENDIENTES:
                            mensajeSaludo = string.Format("¡Hola! {0} ha cerrado la actividad de tu solicitud #{1}, correspondiente al departamento de {2}.", usuarioLogeado.ConcatNombre, solicitud.id, departamento);
                            mensajeMain = string.Format("Se ha finalizado la actividad por parte del departamento de {0}. Sin embargo, existen otros departamentos con actividades pendientes.", departamento);
                            break;
                        case SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_DEPARTAMENTO_FINAL:
                            mensajeSaludo = string.Format("¡Hola! {0} ha cerrado la actividad de la solicitud #{1}, correspondiente al departamento de {2}.", usuarioLogeado.ConcatNombre, solicitud.id, departamento);

                            //valida si la solocitud se encuentra rechazada
                            if (solicitud.SCDM_solicitud_asignaciones.Any(x => (x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SOLICITANTE && x.fecha_cierre == null && x.fecha_rechazo == null)
                                || (x.descripcion == SCDM_solicitudes_asignaciones_tipos.ASIGNACION_SCDM && x.fecha_cierre == null && x.fecha_rechazo == null && DateRechazos.Contains(x.fecha_asignacion)))
                                )
                                mensajeMain = string.Format("Se ha finalizado la actividad por parte del departamento de {0}. Todos los departamentos han finalizado su actividad. Sin embargo, la solicitud a sido rechazada previamente. Haz clic en el botón al final del correo, para ver los detalles de las asignaciones.", departamento);
                            else
                                mensajeMain = string.Format("Se ha finalizado la actividad por parte del departamento de {0}. Todos los departamentos han finalizado su actividad, SCDM le dará continuidad a la solicitud.", departamento);
                            break;

                    }
                    enlace = domainName + "/SCDM_solicitud/Details/" + solicitud.id;
                    break;
                case SCDM_tipo_correo_notificacionENUM.ASIGNACION_SOLICITUD_A_DEPARTAMENTO:
                    mensajeSaludo = string.Format("¡Hola! Se ha asignado la solicitud #{0} al departamento de {1}.", solicitud.id, departamento);
                    mensajeMain = "Se ha asignado una actividad al departamento de "+departamento+ ", haz clic en el botón al final del correo para más detalles.";
                    break;
                case SCDM_tipo_correo_notificacionENUM.RECORDATORIO:
                    if (id_departamento == 99)
                    { //solicitante 
                        using (var db = new Portal_2_0Entities())
                        {
                            DetalleAsignacion detalleSolicitante = solicitud.GetTiempoUltimaAsignacion(99, db.SCDM_cat_dias_feriados.Select(x => x.fecha).ToList());        // 99 -> Solicitante                                           
                            string tiempo = detalleSolicitante.tiempoString;
                            mensajeSaludo = string.Format("<span style ='color:red;'>¡Hola! Tienes una actividad pendiente desde hace {0} (horas laborales).</span>", tiempo);
                            mensajeMain = string.Format("Tienes una actividad pendiente, para el departamento de {0}. Por favor, termina las tareas pendientes y confirma la actividad.", departamento);

                        }
                    }
                    else {
                        TimeSpan? tiempo = solicitud.GetTiempoAsignacion(id_departamento);
                        mensajeSaludo = string.Format("<span style ='color:red;'>¡Hola! Tienes una actividad pendiente desde hace {0} (horas laborales).</span>", tiempo.HasValue ? string.Format("{0}h {1}m", (int)tiempo.Value.TotalHours, tiempo.Value.Minutes) : "--");
                        mensajeMain = string.Format("Tienes una actividad pendiente, para el departamento de {0}. Por favor, termina las tareas pendientes y confirma la actividad.", departamento);
                    }
                  
                    break;
                case SCDM_tipo_correo_notificacionENUM.RECHAZA_SOLICITUD_INICIAL_A_SOLICITANTE:
                    mensajeSaludo = string.Format("¡Hola! {0} ha rechazado tu solicitud #{1}", usuarioLogeado.ConcatNombre, solicitud.id);
                    mensajeMain = "Ingresa al sistema, realiza las actividades correspondientes y envia nuevamente la solicitud.";
                    break;
                case SCDM_tipo_correo_notificacionENUM.RECHAZA_SOLICITUD_SCDM_A_SOLICITANTE:
                    mensajeSaludo = string.Format("¡Hola! SCDM ha rechazado tu solicitud #{0}", solicitud.id);
                    mensajeMain = "Ingresa al sistema, realiza las actividades correspondientes y envia nuevamente la solicitud.";
                    break;
                case SCDM_tipo_correo_notificacionENUM.RECHAZA_SOLICITUD_DEPARTAMENTO_A_SCDM:
                    mensajeSaludo = string.Format("¡Hola! {0} ha rechazado la solicitud #{1}, correspondiente al departamento de {2}.", usuarioLogeado.ConcatNombre, solicitud.id, departamento);
                    mensajeMain = "La solicitud ha sido rechazada por un departamento. Ingresa al sistema, realiza las actividades correspondientes y determina si la solicitud deber ser reenviada al departamento o al solicitante.";
                    break;
                case SCDM_tipo_correo_notificacionENUM.FINALIZA_SOLICITUD:
                    mensajeSaludo = "¡Hola! La solicitud ha sido cerrada por SCDM.";
                    mensajeMain = "La solicitud ha finalizado. Ingresa al sistema, para ver los detalles de la solicitud.";
                    enlace = domainName + "/SCDM_solicitud/Details/" + solicitud.id;
                    break;
                case SCDM_tipo_correo_notificacionENUM.ASIGNACION_INCORRECTA:
                    mensajeSaludo = string.Format("¡Hola! {0} ha informado de una asignación incorrecta de la solicitud #{1}, correspondiente al departamento de {2}.", usuarioLogeado.ConcatNombre, solicitud.id, departamento);
                    mensajeMain = "Ingresa al sistema y determina si la solicitud deber ser reenviada al departamento o si se considera la actividad como terminada.";
                    break;

            }

            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>
            {
                //agrega los valores al diccionario
                { "Folio", solicitud.id.ToString() },
                { "Estatus", solicitud.estatusTexto },
                { "Tipo Solicitud", solicitud.SCDM_cat_tipo_solicitud.descripcion },
            };

            //si es cambio, agrega el tipo de cambio
            if (solicitud.id_tipo_solicitud == (int)SCDMTipoSolicitudENUM.CAMBIOS)
                tablaContentDictionary.Add("Tipo Cambio", solicitud.SCDM_cat_tipo_cambio.descripcion);

            tablaContentDictionary.Add("Solicitante", solicitud.empleados.ConcatNombre);
            tablaContentDictionary.Add("Descripción", solicitud.descripcion);
            tablaContentDictionary.Add("Justificación", solicitud.justificacion);
            tablaContentDictionary.Add("Planta", solicitud.SCDM_rel_solicitud_plantas.FirstOrDefault().plantas.ConcatPlantaSap);
            tablaContentDictionary.Add("Prioridad", solicitud.SCDM_cat_prioridad.descripcion);

            //agrega comentario de rechazo
            if ((tipo == SCDM_tipo_correo_notificacionENUM.RECHAZA_SOLICITUD_INICIAL_A_SOLICITANTE
                || tipo == SCDM_tipo_correo_notificacionENUM.RECHAZA_SOLICITUD_DEPARTAMENTO_A_SCDM
                || tipo == SCDM_tipo_correo_notificacionENUM.RECHAZA_SOLICITUD_SCDM_A_SOLICITANTE)
                && !string.IsNullOrEmpty(comentarioRechazo)
                )
            {
                tablaContentDictionary.Add("<b style ='color:red;'>Comentario de Rechazo</b>", "<b style ='color:red;'>" + comentarioRechazo + "</b>");
            }

            //agrega comentario de asignación incorrecta
            if (tipo == SCDM_tipo_correo_notificacionENUM.ASIGNACION_INCORRECTA
                && !string.IsNullOrEmpty(motivoAsignacionIncorrecta)
                && !string.IsNullOrEmpty(comentarioAsignacionIncorrecta)
                )
            {
                tablaContentDictionary.Add("<b style ='color:red;'>Motivo Asignación Incorrecta</b>", "<b style ='color:red;'>" + motivoAsignacionIncorrecta + "</b>");
                tablaContentDictionary.Add("<b style ='color:red;'>Comentario Asignación Incorrecta</b>", "<b style ='color:red;'>" + comentarioAsignacionIncorrecta + "</b>");
            }

            //agrega comentario
            if (!string.IsNullOrEmpty(comentario))
                tablaContentDictionary.Add("<b>Comentario de Asignación</b>", "<b>" + comentario + "</b>");


            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA_SCDM.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla       
            body = body.Replace("#MENSAJE_SALUDO", mensajeSaludo);
            body = body.Replace("#MENSAJE_MAIN", mensajeMain);
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#ENLACE", enlace);

            return body;
        }


        /// <summary>
        /// Construye el cuerpo del correo para notificar sobre materiales próximos a vencer.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del destinatario.</param>
        /// <param name="materiales">Lista de materiales por vencer.</param>
        /// <returns>Cuerpo del correo en formato HTML.</returns>
        // Reemplaza tu método existente con esta versión

        public string getBodyVencimientoMateriales(string nombreUsuario, List<ReporteVencimientosViewModel> materiales) // <-- CAMBIO 1: Acepta el nuevo ViewModel
        {
            var bodyBuilder = new StringBuilder();
            bodyBuilder.Append($"<h3 style='color:#009ff5;'>Hola {nombreUsuario},</h3>");
            bodyBuilder.Append("<p>Este es un recordatorio automático para informarte que los siguientes materiales solicitados por ti están próximos a vencer:</p>");
            bodyBuilder.Append("<table border='1' cellpadding='5' style='border-collapse:collapse; width: 80%; font-family: sans-serif;'>");

            // Encabezado con la nueva columna
            bodyBuilder.Append("<tr style='background-color:#009ff5; color:white; text-align: left;'><th>Material</th><th>Plantas</th><th>Fecha de Vencimiento</th><th>Días para Vencer</th></tr>");

            foreach (var mat in materiales)
            {
                string estiloDias = mat.Dias_Para_Vencer <= 7 ? "style='color:red; font-weight:bold;'" : "";

                // CAMBIO 2: Se usa "mat.Plantas" (plural) que ya contiene la cadena concatenada
                bodyBuilder.Append($"<tr><td>{mat.Material}</td><td><b>{mat.Plantas}</b></td><td>{mat.Fecha_Vencimiento_Fin_De_Mes:dd/MM/yyyy}</td><td {estiloDias}>{mat.Dias_Para_Vencer}</td></tr>");
            }

            bodyBuilder.Append("</table>");
            bodyBuilder.Append("<p>Si deseas ampliar su vigencia, por favor ingresa al portal y crea una solicitud de <strong>\"Ampliación de Vigencia\"</strong> o la que corresponda según el proceso interno.</p>");
            bodyBuilder.Append("<p>Gracias,<br>Master Data</p>");

            return bodyBuilder.ToString();
        }

        #endregion

        #region Ideas de Mejora
        public string getBodyNuevaIdeaMejoraToProponentes(IM_Idea_mejora solicitud)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/IM_notificacion.html"));
            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>
            {
                //agrega los valores al diccionario
                { "Folio", solicitud.ConcatFolio },
                { "Planta", solicitud.plantas.descripcion },
                { "Fecha", solicitud.captura.ToString() },
                { "Título", solicitud.titulo },
                { "Estatus", "Creada" }
            };

            //Carga los proponentes
            String proponentes = string.Empty;

            foreach (var item in solicitud.IM_rel_proponente)
            {
                proponentes += $"<li style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;Margin-bottom:15px;margin-left:0;color:#333333;font-size:14px\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#333333;font-size:14px\">{item.empleados.ConcatNumEmpleadoNombre}</p></li>\r\n";
            }

            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla     
            body = body.Replace("#MENSAJE_SALUDO", "¡Hola! Tu idea de mejora ha sido registrada.");
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#PROPONENTES", proponentes);
            body = body.Replace("#ENLACE", domainName + "/MejoraContinua/Details/" + solicitud.id);
            return body;
        }

        public string getBodyNuevaIdeaMejoraCambioEstatus(IM_Idea_mejora solicitud, int proximoEstatus, string comentario)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string estatus = IM_EstatusConstantes.DescripcionStatus(proximoEstatus);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/IM_notificacion.html"));
            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>
            {
                //agrega los valores al diccionario
                { "Folio", solicitud.ConcatFolio },
                { "Planta", solicitud.plantas.descripcion },
                { "Fecha", solicitud.captura.ToString() },
                { "Título", solicitud.titulo },
                { "Estatus", estatus}
            };

            if (!String.IsNullOrEmpty(comentario))
                tablaContentDictionary.Add("Comentario", comentario);

            //Carga los proponentes
            String proponentes = string.Empty;

            foreach (var item in solicitud.IM_rel_proponente)
            {
                proponentes += $"<li style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;Margin-bottom:15px;margin-left:0;color:#333333;font-size:14px\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#333333;font-size:14px\">{item.empleados.ConcatNumEmpleadoNombre}</p></li>\r\n";
            }

            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla     
            body = body.Replace("#MENSAJE_SALUDO", "¡Hola! Ha habido un cambio en tu idea de mejora. Consulta los detalles en el portal.");
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#PROPONENTES", proponentes);
            body = body.Replace("#ENLACE", domainName + "/MejoraContinua/Details/" + solicitud.id);
            return body;
        }
        public string getBodyNuevaIdeaMejora(IM_Idea_mejora solicitud)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/IM_notificacion.html"));
            string tablaContenido = String.Empty;

            //crea un diccionario para los valores de la tabla
            Dictionary<string, string> tablaContentDictionary = new Dictionary<string, string>
            {
                //agrega los valores al diccionario
                { "Folio", solicitud.ConcatFolio },
                { "Planta", solicitud.plantas.descripcion },
                { "Fecha", solicitud.captura.ToString() },
                { "Título", solicitud.titulo },
                { "Estatus", "Creada" }
            };

            //Carga los proponentes
            String proponentes = string.Empty;

            foreach (var item in solicitud.IM_rel_proponente)
            {
                proponentes += $"<li style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;Margin-bottom:15px;margin-left:0;color:#333333;font-size:14px\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;line-height:21px;color:#333333;font-size:14px\">{item.empleados.ConcatNumEmpleadoNombre}</p></li>\r\n";
            }

            //agrega los valores del diccionario al contenido de la tabla
            foreach (KeyValuePair<string, string> kvp in tablaContentDictionary)
                tablaContenido += FILA_GENERICA.Replace("#CONCEPTO", kvp.Key).Replace("#VALOR", kvp.Value);

            //reemplaza los valores en la plantilla     
            body = body.Replace("#MENSAJE_SALUDO", "¡Hola! Se ha registrado una nueva Idea de Mejora.");
            body = body.Replace("#TABLA_CONTENIDO", tablaContenido);
            body = body.Replace("#PROPONENTES", proponentes);
            body = body.Replace("#ENLACE", domainName + "/MejoraContinua/Evaluar/" + solicitud.id);
            return body;
        }
        #endregion

        #region Concur
        public string getBodyConcurNuevaSolicitudIT(IT_Concur_Solicitudes solicitud, string nombreSolicitante)
        {
            // Obtener la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string enlace = domainName + "/Concur/DashboardIT";
            string anio = DateTime.Now.Year.ToString();
            string logoUrl = $"data:image/png;base64,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";
            string perfilTexto = solicitud.IsApprover == "Y" ? "Sí" : "No";

            // Construcción de la plantilla HTML On-The-Fly
            StringBuilder sb = new StringBuilder();

            sb.Append("<!DOCTYPE html><html lang=\"es\"><head><meta charset=\"utf-8\" /><title>Notificación Concur</title></head>");
            sb.Append("<body style=\"margin: 0; padding: 0; background-color: #f0f2f5; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;\">");

            // Contenedor principal
            sb.Append("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"background-color: #f0f2f5; padding: 40px 20px;\">");
            sb.Append("<tr><td align=\"center\">");

            // Tarjeta blanca central
            sb.Append("<table width=\"600\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"background-color: #ffffff; border-radius: 8px; border-top: 6px solid #009ff5; box-shadow: 0 5px 20px rgba(0,0,0,0.05);\">");

            // Header
            sb.Append("<tr><td style=\"padding: 30px 40px 10px 40px;\">");
            sb.Append("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr>");
            // Si la imagen del logo sale rota (x roja), asegúrate de que logoUrl apunte a una imagen pública o incrustada correctamente.
            sb.Append($"<td align=\"left\"><img src=\"{logoUrl}\" alt=\"thyssenkrupp\" style=\"max-height: 35px; display: block;\" /></td>");
            sb.Append($"<td align=\"right\" style=\"font-size: 13px; color: #888888; font-family: sans-serif;\">Folio <b style=\"color: #333;\">#{solicitud.IdSolicitud}</b></td>");
            sb.Append("</tr></table>");
            sb.Append("</td></tr>");

            // Título principal
            sb.Append("<tr><td style=\"padding: 10px 40px 25px 40px;\">");
            sb.Append("<h2 style=\"margin: 0 0 15px 0; color: #1a1a1a; font-size: 24px; font-family: sans-serif;\">Nueva Solicitud de Concur</h2>");
            sb.Append("<div style=\"background-color: #f0f9ff; border-left: 4px solid #009ff5; padding: 15px 20px;\">");
            sb.Append($"<p style=\"margin: 0; color: #004d7a; font-size: 14px; line-height: 1.6; font-family: sans-serif;\"><b>{nombreSolicitante}</b> de Recursos Humanos ha solicitado la creación de un nuevo perfil en Concur. Entre al portal para generar el layout layout TXT.</p>");
            sb.Append("</div>");
            sb.Append("</td></tr>");

            // Caja de detalles
            sb.Append("<tr><td style=\"padding: 0 40px 30px 40px;\">");
            sb.Append("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"background-color: #f8f9fa; border: 1px solid #e9ecef; border-radius: 6px;\">");
            sb.Append("<tr><td style=\"padding: 25px;\">");
            sb.Append("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");

            sb.Append("<tr>");
            sb.Append("<td width=\"50%\" style=\"padding-bottom: 20px; font-family: sans-serif;\">");
            sb.Append("<span style=\"font-size: 11px; color: #888888; text-transform: uppercase; letter-spacing: 1px;\">Colaborador</span><br/>");
            sb.Append($"<span style=\"font-size: 15px; color: #222222; font-weight: bold;\">{solicitud.FirstName} {solicitud.LastName}</span>");
            sb.Append("</td>");
            sb.Append("<td width=\"50%\" style=\"padding-bottom: 20px; font-family: sans-serif;\">");
            sb.Append("<span style=\"font-size: 11px; color: #888888; text-transform: uppercase; letter-spacing: 1px;\">8ID</span><br/>");
            sb.Append($"<span style=\"font-size: 15px; color: #009ff5; font-weight: bold;\">{solicitud.GlobalEmployeeID}</span>");
            sb.Append("</td>");
            sb.Append("</tr>");

            sb.Append("<tr>");
            sb.Append("<td width=\"50%\" style=\"padding-bottom: 5px; font-family: sans-serif;\">");
            sb.Append("<span style=\"font-size: 11px; color: #888888; text-transform: uppercase; letter-spacing: 1px;\">Centro de Costos</span><br/>");
            sb.Append($"<span style=\"font-size: 15px; color: #222222; font-weight: bold;\">{solicitud.CostCenterValue}</span>");
            sb.Append("</td>");
            sb.Append("<td width=\"50%\" style=\"padding-bottom: 5px; font-family: sans-serif;\">");
            sb.Append("<span style=\"font-size: 11px; color: #888888; text-transform: uppercase; letter-spacing: 1px;\">¿Es autorizador?</span><br/>");
            sb.Append($"<span style=\"font-size: 13px; color: #333333; background-color: #e2e8f0; padding: 4px 10px; border-radius: 12px; font-weight: bold;\">{perfilTexto}</span>");
            sb.Append("</td>");
            sb.Append("</tr>");

            sb.Append("</table>");
            sb.Append("</td></tr>");
            sb.Append("</table>");
            sb.Append("</td></tr>");

            // Botón CTA (Bulletproof para Outlook y otros clientes de correo)
            sb.Append("<tr><td align=\"center\" style=\"padding: 0 40px 45px 40px;\">");
            sb.Append("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\"><tr>");
            sb.Append("<td align=\"center\" style=\"border-radius: 6px;\" bgcolor=\"#009ff5\">");
            sb.Append($"<a href=\"{enlace}\" target=\"_blank\" style=\"font-size: 15px; font-family: sans-serif; color: #ffffff; text-decoration: none; border-radius: 6px; padding: 12px 30px; border: 1px solid #009ff5; display: inline-block; font-weight: bold;\">Ir al Dashboard de IT</a>");
            sb.Append("</td></tr></table>");
            sb.Append("</td></tr>");

            // Footer corporativo (Azul Marino Thyssenkrupp #009ff5)
            sb.Append("<tr><td style=\"background-color: #009ff5; padding: 25px; text-align: center; border-radius: 0 0 8px 8px;\">");
            sb.Append("<p style=\"margin: 0; color: #ffffff; font-size: 13px; font-weight: bold; font-family: sans-serif; letter-spacing: 0.5px;\">thyssenkrupp Materials de México</p>");
            // Texto secundario en un azul claro para hacer buen contraste
            sb.Append($"<p style=\"margin: 5px 0 0 0; color: #80c4ec; font-size: 11px; font-family: sans-serif;\">Notificación automática del Portal tkMM &copy; {anio}</p>");
            sb.Append("</td></tr>");

            sb.Append("</table>");
            sb.Append("</td></tr></table>");
            sb.Append("</body></html>");

            return sb.ToString();
        }

        public string getBodyConcurSolicitudCompletadaRH(IT_Concur_Solicitudes solicitud)
        {
            // Obtener la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string enlace = domainName + "/Concur/MisSolicitudes";
            string anio = DateTime.Now.Year.ToString();
            string logoUrl = $"data:image/png;base64,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";
            string perfilTexto = solicitud.IsApprover == "Y" ? "Sí" : "No";

            // Construcción de la plantilla HTML On-The-Fly (Versión Éxito/Completado)
            StringBuilder sb = new StringBuilder();

            sb.Append("<!DOCTYPE html><html lang=\"es\"><head><meta charset=\"utf-8\" /><title>Solicitud Concur Completada</title></head>");
            sb.Append("<body style=\"margin: 0; padding: 0; background-color: #f0f2f5; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;\">");

            // Contenedor principal
            sb.Append("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"background-color: #f0f2f5; padding: 40px 20px;\">");
            sb.Append("<tr><td align=\"center\">");

            // Tarjeta central con acento verde (Success)
            sb.Append("<table width=\"600\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"background-color: #ffffff; border-radius: 8px; border-top: 6px solid #16a34a; box-shadow: 0 5px 20px rgba(0,0,0,0.05);\">");

            // Header
            sb.Append("<tr><td style=\"padding: 30px 40px 10px 40px;\">");
            sb.Append("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr>");
            sb.Append($"<td align=\"left\"><img src=\"{logoUrl}\" alt=\"thyssenkrupp\" style=\"max-height: 35px; display: block;\" /></td>");
            sb.Append($"<td align=\"right\" style=\"font-size: 13px; color: #888888; font-family: sans-serif;\">Folio <b style=\"color: #333;\">#{solicitud.IdSolicitud}</b></td>");
            sb.Append("</tr></table>");
            sb.Append("</td></tr>");

            // Título principal
            sb.Append("<tr><td style=\"padding: 10px 40px 25px 40px;\">");
            sb.Append("<h2 style=\"margin: 0 0 15px 0; color: #1a1a1a; font-size: 24px; font-family: sans-serif;\">¡Alta Completada!</h2>");

            // Caja de alerta verde
            sb.Append("<div style=\"background-color: #dcfce7; border-left: 4px solid #16a34a; padding: 15px 20px;\">");
            sb.Append($"<p style=\"margin: 0; color: #155724; font-size: 14px; line-height: 1.6; font-family: sans-serif;\">El equipo de IT ha procesado exitosamente la solicitud de <b>{solicitud.FirstName} {solicitud.LastName}</b>. El perfil en Concur ya se encuentra configurado.</p>");
            sb.Append("</div>");
            sb.Append("</td></tr>");

            // Caja de detalles
            sb.Append("<tr><td style=\"padding: 0 40px 30px 40px;\">");
            sb.Append("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"background-color: #f8f9fa; border: 1px solid #e9ecef; border-radius: 6px;\">");
            sb.Append("<tr><td style=\"padding: 25px;\">");
            sb.Append("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");

            sb.Append("<tr>");
            sb.Append("<td width=\"50%\" style=\"padding-bottom: 20px; font-family: sans-serif;\">");
            sb.Append("<span style=\"font-size: 11px; color: #888888; text-transform: uppercase; letter-spacing: 1px;\">Colaborador</span><br/>");
            sb.Append($"<span style=\"font-size: 15px; color: #222222; font-weight: bold;\">{solicitud.FirstName} {solicitud.LastName}</span>");
            sb.Append("</td>");
            sb.Append("<td width=\"50%\" style=\"padding-bottom: 20px; font-family: sans-serif;\">");
            sb.Append("<span style=\"font-size: 11px; color: #888888; text-transform: uppercase; letter-spacing: 1px;\">8ID</span><br/>");
            sb.Append($"<span style=\"font-size: 15px; color: #16a34a; font-weight: bold;\">{solicitud.GlobalEmployeeID}</span>");
            sb.Append("</td>");
            sb.Append("</tr>");

            sb.Append("<tr>");
            sb.Append("<td width=\"50%\" style=\"padding-bottom: 5px; font-family: sans-serif;\">");
            sb.Append("<span style=\"font-size: 11px; color: #888888; text-transform: uppercase; letter-spacing: 1px;\">Centro de Costos</span><br/>");
            sb.Append($"<span style=\"font-size: 15px; color: #222222; font-weight: bold;\">{solicitud.CostCenterValue}</span>");
            sb.Append("</td>");
            sb.Append("<td width=\"50%\" style=\"padding-bottom: 5px; font-family: sans-serif;\">");
            sb.Append("<span style=\"font-size: 11px; color: #888888; text-transform: uppercase; letter-spacing: 1px;\">¿Es autorizador?</span><br/>");
            sb.Append($"<span style=\"font-size: 13px; color: #333333; background-color: #e2e8f0; padding: 4px 10px; border-radius: 12px; font-weight: bold;\">{perfilTexto}</span>");
            sb.Append("</td>");
            sb.Append("</tr>");

            sb.Append("</table>");
            sb.Append("</td></tr>");
            sb.Append("</table>");
            sb.Append("</td></tr>");

            // Botón CTA al Dashboard de RH
            sb.Append("<tr><td align=\"center\" style=\"padding: 0 40px 45px 40px;\">");
            sb.Append("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\"><tr>");
            sb.Append("<td align=\"center\" style=\"border-radius: 6px;\" bgcolor=\"#009ff5\">");
            sb.Append($"<a href=\"{enlace}\" target=\"_blank\" style=\"font-size: 15px; font-family: sans-serif; color: #ffffff; text-decoration: none; border-radius: 6px; padding: 12px 30px; border: 1px solid #009ff5; display: inline-block; font-weight: bold;\">Ir a Mis Solicitudes</a>");
            sb.Append("</td></tr></table>");
            sb.Append("</td></tr>");

            // Footer corporativo
            sb.Append("<tr><td style=\"background-color: #009ff5; padding: 25px; text-align: center; border-radius: 0 0 8px 8px;\">");
            sb.Append("<p style=\"margin: 0; color: #ffffff; font-size: 13px; font-weight: bold; font-family: sans-serif; letter-spacing: 0.5px;\">thyssenkrupp Materials de México</p>");
            sb.Append($"<p style=\"margin: 5px 0 0 0; color: #80c4ec; font-size: 11px; font-family: sans-serif;\">Notificación automática del Portal Interno &copy; {anio}</p>");
            sb.Append("</td></tr>");

            sb.Append("</table>");
            sb.Append("</td></tr></table>");
            sb.Append("</body></html>");

            return sb.ToString();
        }

        #endregion
    }
}