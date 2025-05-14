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
    /// <summary>
    /// Define la estructura de los rechazos en la vista
    /// </summary>
    public class ActiveRejection
    {
        public string Dept { get; set; }
        public string Comment { get; set; }
        public DateTime DateRejection { get; set; }
    }

    /// <summary>
    /// Define la estructura de los deptos a los que se puede rechazar
    /// </summary>
    public class DeptReassignOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}