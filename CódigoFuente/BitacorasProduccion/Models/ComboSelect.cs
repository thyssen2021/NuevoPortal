using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Models
{
    public class ComboSelect
    {

        
        ///<summary>
        ///Obtiene las empleados
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public static List<SelectListItem> obtieneEmpleadosSelectList()
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            //obtiene todos los posibles valores
            List<empleados> listado = db.empleados.Where(p => p.activo == true).ToList();

            var items = new List<SelectListItem>();

            //usando linQ
            items = listado
                            .Where(s => s.activo == true) //obtiene unicamente los sectores activos
                            .Select(s => new SelectListItem()
                            {
                                Text = s.numeroEmpleado + " - " + s.nombre + " " + s.apellido1 + " " + s.apellido2,
                                Value = s.numeroEmpleado
                            }).ToList();

            //agrega valor vacio
            items.Insert(0, new SelectListItem()
            {
                Text = "Seleccione un valor",
                Value = ""
            });

            return items;
        }
    }
}