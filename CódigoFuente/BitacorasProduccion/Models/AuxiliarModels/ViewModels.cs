using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class ProjectIndexViewModel
    {
        public int ID_Project { get; set; }
        public string ConcatQuoteID { get; set; }
        public int ID_Created_By { get; set; }
        public int ID_Plant { get; set; }

        // Será true si puede editar según tus reglas
        public bool CanEdit { get; set; }
    }
}