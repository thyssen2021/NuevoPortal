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
        private delegate void DelegateEmail(List<string> emailsTo, string subject, string message); //delegate for the action

        public string NOMBRE_FROM { get { return "thyssenkrupp Materials de México"; } }

        public void SendEmailAsync(List<string> emailsTo, string subject, string message)
        {
            try
            {
              
                DelegateEmail sd = DelegateEmailMethod;

                IAsyncResult asyncResult = sd.BeginInvoke(emailsTo,subject,message,null,null);
            }
          
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }

            //return Task.FromResult(0);
        }

        public void DelegateEmailMethod(List<string> emailsTo, string subject, string message)
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

                //agrega los destinatarios
                foreach (string email in emailsTo)
                {
                    if (!string.IsNullOrEmpty(email))
                        mail.To.Add(new MailAddress(email));
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
                //client.SendMailAsync(mail);
                if(mail.To.Count>0)
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
        //metodo para el body de uuna solicitud enviada
        public string getBodyPFAEnviado(PFA pFA)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PFA_solicitud_recibida.html"));

            body = body.Replace("#USUARIO",pFA.empleados1.ConcatNombre);
            body = body.Replace("#NUM_PFA", pFA.id.ToString());
            body = body.Replace("#REASON_PFA", pFA.PFA_Reason.descripcion);
            body = body.Replace("#TYPE_SHIPMENT", pFA.PFA_Type_shipment.descripcion);
            body = body.Replace("#RESPONSIBLE_PFA", pFA.PFA_Responsible_cost.descripcion);
            body = body.Replace("#SAP_PART_NUMBER", pFA.sap_part_number);
            body = body.Replace("#CUSTOMER_PART_NUMBER", pFA.customer_part_number);
            body = body.Replace("#TOTAL_PF_COST", "$ "+ pFA.total_pf_cost.ToString());
            body = body.Replace("#TOTAL_ORIGINAL_COST", "$ " + pFA.total_cost.ToString());
            body = body.Replace("#TOTAL_COST_TO_RECOVER", "$ " + pFA.TotalCostToRecover.ToString());
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            body = body.Replace("#ENLACE", domainName+ "/PremiumFreightApproval/AutorizarRechazar/"+pFA.id);

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
            body = body.Replace("#ANIO",  DateTime.Now.Year.ToString());

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

        //metodo para el body de una solicitud PFA Authorizada
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

        //metodo para el body de una solicitud PFA Authorizada
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
            body = body.Replace("#USUARIO", poliza.empleados2.ConcatNombre);
            body = body.Replace("#NUM_PM", poliza.id.ToString());
            body = body.Replace("#DOCUMENTO_SAP", poliza.numero_documento_sap);
            body = body.Replace("#TIPO_PM", poliza.PM_tipo_poliza.descripcion);
            body = body.Replace("#PLANTA", poliza.plantas.descripcion);
            body = body.Replace("#MONEDA", poliza.currency.CocatCurrency);
            body = body.Replace("#FECHA_DOCUMENTO", poliza.fecha_documento.ToString("dd/MM/yyyy"));
            body = body.Replace("#DESCRIPCION_PM", poliza.descripcion_poliza);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#ENLACE", domainName + "/PolizaManual/ValidarArea/" + poliza.id);

            return body;
        }

        //metodo para obtener el body de Poliza Manual cuando se valida una poliza por el jefe de area
        public string getBodyPMValidadoPorArea(poliza_manual poliza)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PM_autorizacion_validador.html"));

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
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

        //metodo para obtener el body de Poliza Manual cuando una Póliza ha sido rechazada
        public string getBodyPMRechazada(poliza_manual poliza, string nombreRechazante)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PM_rechazo.html"));


            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#RECHAZANTE", nombreRechazante);
            body = body.Replace("#NUM_PM", poliza.id.ToString());
            body = body.Replace("#DOCUMENTO_SAP", poliza.numero_documento_sap);
            body = body.Replace("#TIPO_PM", poliza.PM_tipo_poliza.descripcion);
            body = body.Replace("#PLANTA", poliza.plantas.descripcion);
            body = body.Replace("#MONEDA", poliza.currency.CocatCurrency);
            body = body.Replace("#FECHA_DOCUMENTO", poliza.fecha_documento.ToString("dd/MM/yyyy"));
            body = body.Replace("#DESCRIPCION_PM", poliza.descripcion_poliza);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());
            body = body.Replace("#RECHAZO", String.IsNullOrEmpty(poliza.comentario_rechazo)?String.Empty:poliza.comentario_rechazo);
            body = body.Replace("#ENLACE", domainName + "/PolizaManual/Edit/" + poliza.id);

            return body;
        }

        //metodo para obtener el body de Poliza Manual cuando se valida una poliza por controlling
        public string getBodyPMValidadoPorControlling(poliza_manual poliza)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PM_autorizacion_controlling.html"));

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#AUTORIZADOR", poliza.empleados.ConcatNombre);
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

        //metodo para obtener el body de Poliza Manual cuando ha sido registrada por contabilidad
        public string getBodyPMRegistradoPorContabilidad(poliza_manual poliza)
        {
            //obtiene la direccion del dominio
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/emails_plantillas/PM_confirmacion_registro.html"));

            //body = body.Replace("#VALIDADOR", poliza.PM_validadores.empleados.ConcatNombre);
            body = body.Replace("#USUARIO_CONTABILIDAD", poliza.empleados1.ConcatNombre);
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


    }
}