using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class PPM
    {
        public PPM() { 
        
        }

        public DateTime? fecha { get; set; }
        public double PiezasDescarteKG_internos { get; set; }
        public double PiezasDescarteKG_externos { get; set; }
        public double PiezasCortadasTon { get; set; }     

       
        public double? CalculoPPM_interno
        {
            get
            {
                double result = 0;
                try
                {
                    result = PiezasDescarteKG_internos / PiezasCortadasTon;
                }
                catch {                    
                    return 0;
                }

                if (double.IsNaN(result))
                    return 0;

                return result * 1000000;
            }
        }
        public double CalculoPPM_externo
        {
            get
            {
                double result = 0;
                try
                {
                    result = PiezasDescarteKG_externos / PiezasCortadasTon;
                }
                catch
                {
                    return 0;
                }

                if (double.IsNaN(result))
                    return 0;

                return result * 1000000;
            }
        }

    }

    /// <summary>
    /// obtiene el reporte de PPM's segun una lista de registros
    /// </summary>
    public static class UtilPPM {

        public static List<PPM> ObtieneReportePorDia(List<produccion_registros> registros , string fecha_inicial, string fecha_final) {

            #region completa_fechas

            List<PPM> listado = new List<PPM>();

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = DateTime.Now;          //fecha final por defecto          

            bool errorDate = false;
            try
            {
                if (!String.IsNullOrEmpty(fecha_inicial))
                    dateInicial = Convert.ToDateTime(fecha_inicial);
                if (!String.IsNullOrEmpty(fecha_final))
                {
                    dateFinal = Convert.ToDateTime(fecha_final);
                    dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }
            catch (FormatException e)
            {
                errorDate = true;
                Console.WriteLine("Error de Formato: " + e.Message);
            }
            catch (Exception ex)
            {
                errorDate = true;
                Console.WriteLine("Error al convertir: " + ex.Message);
            }

            //si hay registros, las fechas no estan vacias y no hubo error al convertir completa el list de PMM
            if (registros.Count > 0 && !String.IsNullOrEmpty(fecha_inicial) && !String.IsNullOrEmpty(fecha_final) && !errorDate) {
                while (dateInicial <= dateFinal) {
                    listado.Add(new PPM
                    {
                        fecha = dateInicial
                    });
                    dateInicial += new TimeSpan(1, 0, 0, 0);
                }

            }

            #endregion

            //Obtiene la sumatoria de cantidad piezas de descarte para cada fecha
            foreach (produccion_registros prod in registros) {

                foreach (var ppm in listado.Where(x => prod.fecha >= x.fecha &&  prod.fecha< x.fecha.Value.AddHours(23).AddMinutes(59).AddSeconds(59)))
                {
                    if (prod.produccion_datos_entrada != null && prod.produccion_datos_entrada.peso_real_pieza_neto.HasValue)
                    {
                        ppm.PiezasDescarteKG_internos += prod.produccion_datos_entrada.peso_real_pieza_neto.Value * prod.NumPiezasDescarteDanoInterno();
                        ppm.PiezasDescarteKG_externos += prod.produccion_datos_entrada.peso_real_pieza_neto.Value * prod.NumPiezasDescarteDanoExterno();
                        ppm.PiezasCortadasTon += prod.produccion_datos_entrada.PesoRegresoRolloUsado;                    
                    }
                }

            } 
            
            return listado ;
        }
    
    }

}