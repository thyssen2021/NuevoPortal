using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Models
{
    public class ComboSelect
    {

        private Portal_2_0Entities db = new Portal_2_0Entities();
        ///<summary>
        ///Obtiene un List<SelectListItem> con las áreas activas
        ///</summary>
        ///<return>
        ///retorna un List<SelectListItem> con las áreas activas
        ///</return>
        //public static List<SelectListItem> obtieneAreasActivas()
        //{
        //    //consulta todos los productos de la BD
        //    List<Producto> listadoProductos = Presupuestos.UtilBD.ProductoDBUtil.Lista();

        //    var productos = new List<SelectListItem>();

        //    //usando linQ
        //    productos = listadoProductos
        //        .Where(s => s.Activo == true) //obtiene unicamente los sectores activos
        //        .Select(s => new SelectListItem()
        //        {
        //            Text = s.Nombre,
        //            Value = s.Id.ToString()
        //        }).ToList();

        //    //agrega valor vacio
        //    productos.Insert(0, new SelectListItem()
        //    {
        //        Text = "Seleccione un valor",
        //        Value = ""
        //    });

        //    return productos;
        //}
    }
}