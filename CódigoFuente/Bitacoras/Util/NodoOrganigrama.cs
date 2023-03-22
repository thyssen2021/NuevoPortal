using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public class NodoOrganigrama
    {
        public NodoOrganigrama()
        {
            Childs = new List<NodoOrganigrama>();
        }

        public int ID { get; set; }
        public string ClassName { get; set; }
        public string NodeTitle { get; set; }
        public string NodeContent { get; set; }

        public List<NodoOrganigrama> Childs { get; set; }
        public NodoOrganigrama NodoPadre { get; set; }

    }
}
