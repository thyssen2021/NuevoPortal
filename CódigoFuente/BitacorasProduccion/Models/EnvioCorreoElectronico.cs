using Bitacoras.Util;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                // emailsCC.Add("alfredo.xochitemol@thyssenkrupp-materials.com");
                // ************************************//

                //agrega los destinatarios
                foreach (string email in emailsTo)
                {
                    if (!string.IsNullOrEmpty(email) && !email.Contains("lagermex.com.mx"))
                        mail.To.Add(new MailAddress(email));
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
                if (mail.To.Count > 0)
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
                    mensajeSaludo = string.Format("¡Hola! El usuario {0} ha enviado la solicitud #{1} para tu revisión.", usuarioLogeado.ConcatNombre, solicitud.id);
                    mensajeMain = "Se ha asignado la solictud para tu revisión.";
                    break;
                case SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_INICIAL:
                    mensajeSaludo = string.Format("¡Hola! El usuario {0} ha aprobado la solicitud #{1}", usuarioLogeado.ConcatNombre, solicitud.id);
                    mensajeMain = "Se ha aprobado la revisión inicial de la solicitud y ha sido enviada a SCDM para su procesamiento.";
                    break;
                case SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_DEPARTAMENTO_PENDIENTES:
                    mensajeSaludo = string.Format("¡Hola! El usuario {0} ha cerrado la actividad de la solicitud #{1}, correspondiente al departamento de {2}.", usuarioLogeado.ConcatNombre, solicitud.id, departamento);
                    mensajeMain = string.Format("Se ha finalizado la actividad por parte del departamento de {0}. Sin embargo, existen otros departamentos con actividades pendientes.", departamento);
                    break;
                case SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_DEPARTAMENTO_FINAL:
                    mensajeSaludo = string.Format("¡Hola! El usuario {0} ha cerrado la actividad de la solicitud #{1}, correspondiente al departamento de {2}.", usuarioLogeado.ConcatNombre, solicitud.id, departamento);

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
                            mensajeSaludo = string.Format("¡Hola! El usuario {0} ha aprobado la solicitud #{1}", usuarioLogeado.ConcatNombre, solicitud.id);
                            mensajeMain = "Se ha aprobado la revisión inicial de la solicitud y ha sido enviada a SCDM para su procesamiento.";
                            break;
                        case SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_DEPARTAMENTO_PENDIENTES:
                            mensajeSaludo = string.Format("¡Hola! El usuario {0} ha cerrado la actividad de tu solicitud #{1}, correspondiente al departamento de {2}.", usuarioLogeado.ConcatNombre, solicitud.id, departamento);
                            mensajeMain = string.Format("Se ha finalizado la actividad por parte del departamento de {0}. Sin embargo, existen otros departamentos con actividades pendientes.", departamento);
                            break;
                        case SCDM_tipo_correo_notificacionENUM.APRUEBA_SOLICITUD_DEPARTAMENTO_FINAL:
                            mensajeSaludo = string.Format("¡Hola! El usuario {0} ha cerrado la actividad de la solicitud #{1}, correspondiente al departamento de {2}.", usuarioLogeado.ConcatNombre, solicitud.id, departamento);

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
                    mensajeMain = "Se ha asignado la solictud para tu revisión.";
                    break;
                case SCDM_tipo_correo_notificacionENUM.RECORDATORIO:

                    TimeSpan? tiempo = solicitud.GetTiempoAsignacion(id_departamento);

                    mensajeSaludo = string.Format("<span style ='color:red;'>¡Hola! Tienes una actividad pendiente desde hace {0} (horas laborales).</span>", tiempo.HasValue ? string.Format("{0}h {1}m", (int)tiempo.Value.TotalHours, tiempo.Value.Minutes) : "--");
                    mensajeMain = string.Format("Tienes una actividad pendiente, para el departamento de {0}. Por favor, termina las tareas pendientes y confirma la actividad.", departamento);
                    break;
                case SCDM_tipo_correo_notificacionENUM.RECHAZA_SOLICITUD_INICIAL_A_SOLICITANTE:
                    mensajeSaludo = string.Format("¡Hola! El usuario {0} ha rechazado tu solicitud #{1}", usuarioLogeado.ConcatNombre, solicitud.id);
                    mensajeMain = "Ingresa al sistema, realiza las actividades correspondientes y envia nuevamente la solicitud.";
                    break;
                case SCDM_tipo_correo_notificacionENUM.RECHAZA_SOLICITUD_SCDM_A_SOLICITANTE:
                    mensajeSaludo = string.Format("¡Hola! SCDM ha rechazado tu solicitud #{0}", solicitud.id);
                    mensajeMain = "Ingresa al sistema, realiza las actividades correspondientes y envia nuevamente la solicitud.";
                    break;
                case SCDM_tipo_correo_notificacionENUM.RECHAZA_SOLICITUD_DEPARTAMENTO_A_SCDM:
                    mensajeSaludo = string.Format("¡Hola! El usuario {0} ha rechazado la solicitud #{1}, correspondiente al departamento de {2}.", usuarioLogeado.ConcatNombre, solicitud.id, departamento);
                    mensajeMain = "La solicitud ha sido rechazada por un departamento. Ingresa al sistema, realiza las actividades correspondientes y determina si la solicitud deber ser reenviada al departamento o al solicitante.";
                    break;
                case SCDM_tipo_correo_notificacionENUM.FINALIZA_SOLICITUD:
                    mensajeSaludo = "¡Hola! La solicitud ha sido cerrada por SCDM.";
                    mensajeMain = "La solicitud ha finalizado. Ingresa al sistema, para ver los detalles de la solicitud.";
                    enlace = domainName + "/SCDM_solicitud/Details/" + solicitud.id;
                    break;
                case SCDM_tipo_correo_notificacionENUM.ASIGNACION_INCORRECTA:
                    mensajeSaludo = string.Format("¡Hola! El usuario {0} ha informado de una asignación incorrecta de la solicitud #{1}, correspondiente al departamento de {2}.", usuarioLogeado.ConcatNombre, solicitud.id, departamento);
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

        #endregion
    }
}