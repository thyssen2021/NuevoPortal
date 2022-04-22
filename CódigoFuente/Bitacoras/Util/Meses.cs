using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public class Meses
    {
        public int Numero { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get {
                return this.Nombre.Substring(0,3);
            }  }
        public string Name { get; set; }

        public string Abreviation
        {
            get
            {
                return this.Name.Substring(0, 3);
            }
        }
    }

    public static class MesesUtil
    {
        
        public static Meses ENERO = new Meses { Numero = 1, Nombre = "Enero", Name = "January" };
        public static Meses FEBRERO = new Meses { Numero = 2, Nombre = "Febrero", Name = "February" };
        public static Meses MARZO = new Meses { Numero = 3, Nombre = "Marzo", Name = "March" };
        public static Meses ABRIL = new Meses { Numero = 4, Nombre = "Abril", Name = "April" };
        public static Meses MAYO = new Meses { Numero = 5, Nombre = "Mayo", Name = "May" };
        public static Meses JUNIO = new Meses { Numero = 6, Nombre = "Junio", Name = "June" };
        public static Meses JULIO = new Meses { Numero = 7, Nombre = "Julio", Name = "July" };
        public static Meses AGOSTO = new Meses { Numero = 8, Nombre = "Agosto", Name = "August" };
        public static Meses SEPTIEMBRE = new Meses { Numero = 9, Nombre = "Septiembre", Name = "September" };
        public static Meses OCTUBRE = new Meses { Numero = 10, Nombre = "Octubre", Name = "October" };
        public static Meses NOVIEMBRE = new Meses { Numero = 11, Nombre = "Noviembre", Name = "November" };
        public static Meses DICIEMBRE = new Meses { Numero = 12, Nombre = "Diciembre", Name = "December" };

        public static List<Meses> ListadoMeses = new List<Meses> {
            ENERO, FEBRERO,MARZO,ABRIL,MAYO,JUNIO, JULIO, AGOSTO,SEPTIEMBRE,OCTUBRE, NOVIEMBRE, DICIEMBRE
        };

        public static Meses getMes(int mes) {

            switch (mes)
            {
                case 1:
                    return ENERO;
                case 2:
                    return FEBRERO;
                case 3:
                    return MARZO;
                case 4:
                    return ABRIL;
                case 5:
                    return MAYO;
                case 6:
                    return JUNIO;
                case 7:
                    return JULIO;
                case 8:
                    return AGOSTO;
                case 9:
                    return SEPTIEMBRE;
                case 10:
                    return OCTUBRE;
                case 11:
                    return NOVIEMBRE;
                case 12:
                    return DICIEMBRE;
                default:
                    return null;
            }

        }
    }
}
