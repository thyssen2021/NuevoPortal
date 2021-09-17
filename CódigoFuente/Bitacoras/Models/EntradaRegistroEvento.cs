using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clases.Models
{
    public class EntradaRegistroEvento
    {
        #region Propiedades

        private int idEvento;
        private DateTime fechaEvento;
        private string usuario;
        private TipoEntradaRegistroEvento tipo = TipoEntradaRegistroEvento.Informacion;
        private string origen;
        private string descripcion;
        private int gravedad;
        private int numeroError;

        public EntradaRegistroEvento(int idEvento, DateTime fechaEvento, string usuario, TipoEntradaRegistroEvento tipo, string origen, string descripcion, int gravedad, int numeroError)
        {
            this.IdEvento = idEvento;
            this.FechaEvento = fechaEvento;
            this.Usuario = usuario;
            this.Tipo = tipo;
            this.Origen = origen;
            this.Descripcion = descripcion;
            this.Gravedad = gravedad;
            this.NumeroError = numeroError;
        }

        public int IdEvento { get => idEvento; set => idEvento = value; }
        public DateTime FechaEvento { get => fechaEvento; set => fechaEvento = value; }
        public string Usuario { get => usuario; set => usuario = value; }
        public TipoEntradaRegistroEvento Tipo { get => tipo; set => tipo = value; }
        public string Origen { get => origen; set => origen = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
        public int Gravedad { get => gravedad; set => gravedad = value; }
        public int NumeroError { get => numeroError; set => numeroError = value; }

        public char ObtenerTipoCodigo()
        {
            switch (this.Tipo)
            {
                case EntradaRegistroEvento.TipoEntradaRegistroEvento.Informacion:
                    return 'I';

                case EntradaRegistroEvento.TipoEntradaRegistroEvento.Advertencia:
                    return 'A';

                case EntradaRegistroEvento.TipoEntradaRegistroEvento.Error:
                    return 'E';
            }
            return 'I';
        }



        #endregion


        #region Enumeraciones

        /// <summary>
        /// Tipos de eventos
        /// </summary>
        public enum TipoEntradaRegistroEvento : int
        {
            Error = 2,
            Informacion = 0,
            Advertencia = 1
        }
        #endregion


    }
}
