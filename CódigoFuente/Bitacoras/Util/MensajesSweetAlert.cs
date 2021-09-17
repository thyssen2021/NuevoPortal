using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clases.Util
{
    public class MensajesSweetAlert
    {
        string mensaje;
        TipoMensajesSweetAlerts tipo = TipoMensajesSweetAlerts.INFO;

        public MensajesSweetAlert(string mensaje, TipoMensajesSweetAlerts tipo)
        {
            this.mensaje = mensaje;
            this.tipo = tipo;
        }

        public string Mensaje { get => mensaje; set => mensaje = value; }
        public TipoMensajesSweetAlerts Tipo { get => tipo; set => tipo = value; }

        public string getTipoMensaje()
        {

            switch (this.Tipo)
            {
                case TipoMensajesSweetAlerts.WARNING:
                    return "warning";
                case TipoMensajesSweetAlerts.ERROR:
                    return "error";
                case TipoMensajesSweetAlerts.INFO:
                    return "info";
                case TipoMensajesSweetAlerts.SUCCESS:
                    return "success";
                default:
                    return "info";

            }
        }
    }

    public enum TipoMensajesSweetAlerts
    {
        WARNING,
        ERROR,
        SUCCESS,
        INFO
    }

    public static class TextoMensajesSweetAlerts
    {
        public const string CREATE = "Se ha creado el registro correctamente.";
        public const string UPDATE = "Se ha modificado el registro correctamente.";
        public const string ENABLED = "Se ha habilitado el registro correctamente.";
        public const string DISABLED = "Se ha deshabilitado el registro correctamente.";

    }

}
