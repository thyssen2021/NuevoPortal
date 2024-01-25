using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class budget_anio_fiscalMetadata
    {
    }

    [MetadataType(typeof(budget_anio_fiscalMetadata))]
    public partial class budget_anio_fiscal
    {

        //concatena el Anio Fiscal
        [NotMapped]
        public string ConcatAnio
        {
            get
            {
                return string.Format("FY {0}/{1}", anio_inicio, anio_fin);
            }
        }

        /// <summary>
        /// Indica si un mes ya es actual(true) o es Forecast(false)
        /// </summary>
        /// <param name="mesUtil"></param>
        public string isActual(int mes)
        {
            DateTime fechaActual = DateTime.Now;
            fechaActual = new DateTime(fechaActual.Year, fechaActual.Month, 1, 0, 0, 0);

            int anio = 0;

            if (mes >= 10 && mes <= 12)
                anio = anio_inicio;
            else if (mes >= 1 && mes < 10)
                anio = anio_fin;
            else
                return String.Empty;

            DateTime fechaComparacion = new DateTime(anio, mes, 1, 0, 0, 0);
            DateTime fechaFinAnoPresente = new DateTime(fechaActual.Year, 10, 1, 0, 0, 0);

            if (fechaComparacion >= fechaFinAnoPresente)
                return "BG";
            if (fechaActual <= fechaComparacion)
                return "FC";
            else
                return "ACT";

        }

        public static budget_anio_fiscal Get_Anio_Fiscal(DateTime fecha)
        {

            Portal_2_0Entities db = new Portal_2_0Entities();

            //obtiene el listado de año fiscale
            List<budget_anio_fiscal> listAnios = db.budget_anio_fiscal.ToList();

            //recorre todos los años buscando la coincidencia
            foreach (budget_anio_fiscal item in listAnios)
            {

                DateTime timeInicial = new DateTime(item.anio_inicio, item.mes_inicio, 1, 0, 0, 0);
                DateTime timeFinal = new DateTime(item.anio_fin, item.mes_fin, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                if ((fecha >= timeInicial) && (fecha <= timeFinal))
                {

                    return item;
                }

            }

            return null;
        }
    }
}