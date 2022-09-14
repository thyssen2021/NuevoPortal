using Bitacoras.Util;
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
        private delegate void DelegateEmail(List<string> emailsTo, string subject, string message); //delegate for the action

        public string NOMBRE_FROM { get { return "thyssenkrupp Materials de México"; } }

        public void SendEmailAsync(List<string> emailsTo, string subject, string message)
        {
            try
            {

                DelegateEmail sd = DelegateEmailMethod;

                IAsyncResult asyncResult = sd.BeginInvoke(emailsTo, subject, message, null, null);
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

                //********** Comentar para productivo ************//
                emailsTo = new List<string>();
                emailsTo.Add("alfredo.xochitemol@lagermex.com.mx");
                // ************************************//

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

        #region correos PFA
        //metodo para el body de uuna solicitud enviada
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
            body = body.Replace("#PUESTO", item.empleados.puesto1.descripcion);
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
            body = body.Replace("#PUESTO", item.empleados.puesto1.descripcion);
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
            body = body.Replace("#JEFE", item.empleados1.ConcatNombre); //jefe
            body = body.Replace("#ESTADO", IT_MR_Status.DescripcionStatus(item.estatus));
            body = body.Replace("#ID", item.id.ToString()); //elaborador
            body = body.Replace("#EMPLEADO", item.empleados.ConcatNombre);
            body = body.Replace("#PUESTO", item.empleados.puesto1.descripcion);
            body = body.Replace("#COMENTARIO", item.comentario);
            body = body.Replace("#RECHAZO", item.comentario_rechazo);
            body = body.Replace("#FECHA_SOLICITUD", item.fecha_solicitud.ToString("dd/MM/yyyy"));
            body = body.Replace("#ENLACE", domainName + "/IT_matriz_requerimientos/CrearMatriz/" + item.id_empleado);
            body = body.Replace("#ANIO", DateTime.Now.Year.ToString());

            return body;

        }

        /// <summary>
        /// metodo para obtener el body de email de notificación de cierre de solicitud
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
        /// metodo para obtener el body de email de notificación en proceso de solicitud
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



            body = body.Replace("#NOMBRE", nombre); //usuario creado
            body = body.Replace("#USER", model.Email);
            body = body.Replace("#PASS", model.Password); //elaborador         
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
            string planta = item.empleados != null && item.empleados.plantas != null ? item.empleados.plantas.descripcion : item.plantas != null ? item.plantas.descripcion: string.Empty ;
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
    }
}