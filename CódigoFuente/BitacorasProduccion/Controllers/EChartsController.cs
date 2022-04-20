using Bitacoras.Util;
using Portal_2_0.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class EChartsController : BaseController
    {
        // GET: ECharts
        private Portal_2_0Entities db = new Portal_2_0Entities();

        public JsonResult GetChartDemo()
        {
            ArrayList titulos = new ArrayList();
            ArrayList valores = new ArrayList();
            System.Data.DataTable dt = new System.Data.DataTable();

            // var list = db.orden_trabajo.ToList();

            //dimension
            titulos.Add("Nombre");
            titulos.Add("Alta");
            titulos.Add("Media");
            titulos.Add("Baja");

            //AGREGAR FILTRO PARA FECHAS y ESTATUS
            var listaOT = db.orden_trabajo.ToList();

            //obtiene todos los responsables de las OT
            List<empleados> listEmpleados = db.orden_trabajo.Where(x=>x.id_responsable>0).Select(x=> x.empleados1).Distinct().ToList();
            
            //inicializa la lista de objetos
            var list = new object[listEmpleados.Count];

            for (int i = 0; i < listEmpleados.Count; i++)
            {
                int altas = listaOT.Where(x => x.id_responsable == listEmpleados[i].id && x.nivel_urgencia == OT_nivel_urgencia.ALTA).Count();
                int bajas = listaOT.Where(x => x.id_responsable == listEmpleados[i].id && x.nivel_urgencia == OT_nivel_urgencia.BAJA).Count();
                int medias = listaOT.Where(x => x.id_responsable == listEmpleados[i].id && x.nivel_urgencia == OT_nivel_urgencia.MEDIA).Count();

                list[i] = new { Nombre = listEmpleados[i].nombre + " " + listEmpleados[i].apellido1, Alta = altas, Media= medias, Baja = bajas };
            }          


            //valores.Add(new ArrayList { Nombre = "AXC", 90,23,23});
            //valores.Add(new ArrayList { "OLV", 5, 26, 63 });


            var result = new { name = titulos, num = list };//Design JSON format
            return Json(result);//Return JSON data
        }
    }
}