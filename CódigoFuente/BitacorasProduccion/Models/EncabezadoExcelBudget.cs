using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class EncabezadoExcelBudget
    {
        public string texto  { get; set;}
        public DateTime fecha { get; set; }

        public budget_anio_fiscal anio_Fiscal { get; set; }

        public budget_rel_fy_centro rel_fy { get; set; }

    }
}