using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class CombosController : BaseController
    {

        private Portal_2_0Entities db = new Portal_2_0Entities();

        ///<summary>
        ///Obtiene las areas segun la planta recibida
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneAreas(int clavePlanta = 0)
        {
            //obtiene todos los posibles valores
            List<Area> listado = db.Area.Where(p => p.plantaClave.Value == clavePlanta && p.activo == true).ToList();


            //inserta el valor por default
            listado.Insert(0, new Area
            {
                clave = 0,
                descripcion = "-- Seleccione un valor --"
            });

            //inicializa la lista de objetos
            var list = new object[listado.Count];

            //completa la lista de objetos
            for (int i = 0; i < listado.Count; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = listado[i].descripcion };
                else
                    list[i] = new { value = listado[i].clave, name = listado[i].descripcion };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene los empleado segun la planta recibida
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneEmpleados(int clavePlanta = 0)
        {
            //obtiene todos los posibles valores
            List<empleados> listado = db.empleados.Where(p => p.planta_clave.Value == clavePlanta && p.activo == true).ToList();


            //inserta el valor por default
            listado.Insert(0, new empleados
            {
                clave = 0,
                nombre = "-- Seleccione un valor --"
            });

            //inicializa la lista de objetos
            var list = new object[listado.Count];

            //completa la lista de objetos
            for (int i = 0; i < listado.Count; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = listado[i].nombre };
                else
                    list[i] = new { value = listado[i].id, name = ("("+listado[i].numeroEmpleado+") "+ listado[i].nombre +" "+ listado[i].apellido1+" "+ listado[i].apellido2) };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene las lineas de la plantas enviada
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneLineasPlantas(int clavePlanta = 0)
        {
            //obtiene todos los posibles valores
            List<produccion_lineas> listado = db.produccion_lineas.Where(p => p.clave_planta == clavePlanta && p.activo == true).ToList();


            //inserta el valor por default
            listado.Insert(0, new produccion_lineas
            {
                id = 0,
                linea = "-- Seleccione un valor --"
            });

            //inicializa la lista de objetos
            var list = new object[listado.Count];

            //completa la lista de objetos
            for (int i = 0; i < listado.Count; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = listado[i].linea };
                else
                    list[i] = new { value = listado[i].id, name = listado[i].linea };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene los puestos segun el área recibida
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtienePuestos(int claveArea = 0)
        {
            //obtiene todos los posibles valores
            List<puesto> listado = db.puesto.Where(p => p.areaClave == claveArea && p.activo == true).ToList();


            //inserta el valor por default
            listado.Insert(0, new puesto
            {
                clave = 0,
                descripcion = "-- Seleccione un valor --"
            });

            //inicializa la lista de objetos
            var list = new object[listado.Count];

            //completa la lista de objetos
            for (int i = 0; i < listado.Count; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = listado[i].descripcion };
                else
                    list[i] = new { value = listado[i].clave, name = listado[i].descripcion };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene el email de un empleado
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneEmail(string numEmpleado = "")
        {
            //obtiene todos los posibles valores
            empleados emp = db.empleados.Where(p => p.numeroEmpleado == numEmpleado).FirstOrDefault();

            //inicializa la lista de objetos
            var empleado = new object[1];

            if (emp ==null || String.IsNullOrEmpty(emp.correo))
            { //no hay correo
                empleado[0] = new { email = "NO DISPONIBLE" };
            }
            else //hay correo
            {
                empleado[0] = new { email = emp.correo };
            }

            return Json(empleado, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene el rollo de bom según el material enviado
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneRollosBom(string material = "")
        {
            //obtiene todos los posibles valores
            List<bom_en_sap> listado = db.bom_en_sap.Where(p => p.activo == true && p.Quantity > 0 && !p.Material.StartsWith("sm") && p.Material==material).ToList();

            //realiza un distict de los materiales
            List<string> distinctList = listado.Where(m => m.Material == material).Select(m => m.Component).Distinct().ToList();

            //inserta el valor por default
            distinctList.Insert(0,  "-- Seleccione un valor --");

            //inicializa la lista de objetos
            var list = new object[distinctList.Count];

            //completa la lista de objetos
            for (int i = 0; i < distinctList.Count; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = distinctList[i]};
                else
                    list[i] = new { value = distinctList[i], name = distinctList[i] };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}