using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class PesadaViewModel
    {
        public string Fecha { get; set; }
        public decimal PesoReal { get; set; }
        public decimal PesoSAP { get; set; }
        public decimal Diferencia { get; set; } // Nuevo campo para la diferencia
        public decimal PorcentajeVariacion { get; set; } // Nuevo campo -> ((peso neto real - peso neto sap)/peso neto sap)*100
        public bool EsAtipico { get; set; } 


    }

    public class AnalisisViewModel
    {
        public List<PesadaViewModel> DatosGrafica { get; set; }
        public string Planta { get; set; }
        public string NombrePlanta { get; set; }
        public string Cliente { get; set; }
        public string Material { get; set; }
        public string FechaInicial { get; set; }
        public string FechaFinal { get; set; }
        public double PesoNetoSAP { get; set; }
        public double Media { get; set; }
        public double Mediana { get; set; }
        public double Minimo { get; set; }
        public double Maximo { get; set; }
        public double DesviacionEstandar { get; set; }
        public int TotalRegistros { get; set;}
        
    }
}