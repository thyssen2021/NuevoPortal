using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        ///Obtiene las areas segun la planta recibida para budget
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneBudgetAreas(int id_planta = 0)
        {
            //obtiene todos los posibles valores
            List<budget_departamentos> listado = db.budget_departamentos.Where(p => p.id_budget_planta == id_planta && p.activo == true).ToList();


            //inserta el valor por default
            listado.Insert(0, new budget_departamentos
            {
                id = 0,
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
                    list[i] = new { value = listado[i].id, name = listado[i].descripcion };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        ///<summary>
        ///Obtiene las areas segun la planta recibida para budget
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneGVCentros(int clavePlanta = 0)
        {
            //obtiene todos los posibles valores
            List<GV_centros_costo> listado = db.GV_centros_costo.Where(p => p.clave_planta == clavePlanta && p.activo == true).ToList();


            //inserta el valor por default
            listado.Insert(0, new GV_centros_costo
            {
                id = 0,
                centro_costo = "-- Seleccione un valor --"
            });

            //inicializa la lista de objetos
            var list = new object[listado.Count];

            //completa la lista de objetos
            for (int i = 0; i < listado.Count; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = listado[i].centro_costo };
                else
                    list[i] = new { value = listado[i].id, name = listado[i].ConcatCentroDepto };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        ///<summary>
        ///Obtiene las mapping según la mapping bridge recibida
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneMapping(int id_mapping_bridge = 0)
        {
            //obtiene todos los posibles valores
            List<budget_mapping> listado = db.budget_mapping.Where(p => p.id_mapping_bridge == id_mapping_bridge && p.activo == true).ToList();


            //inserta el valor por default
            listado.Insert(0, new budget_mapping
            {
                id = 0,
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
                    list[i] = new { value = listado[i].id, name = listado[i].descripcion };
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
                    list[i] = new { value = listado[i].id, name = ("(" + listado[i].numeroEmpleado + ") " + listado[i].nombre + " " + listado[i].apellido1 + " " + listado[i].apellido2) };
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
                linea = "-- Todas --"
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
        ///Obtiene las fallas según la línea de produccion recibida
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult ObtieneZonaFalla(int id_linea = 0)
        {
            //obtiene todos los posibles valores
            List<OT_zona_falla> listado = db.OT_zona_falla.Where(p => p.id_linea == id_linea && p.activo == true).ToList();

            //inserta el valor por default
            listado.Insert(0, new OT_zona_falla
            {
                id = 0,
                zona_falla = "-- N/A --"
            });

            //inicializa la lista de objetos
            var list = new object[listado.Count];

            //completa la lista de objetos
            for (int i = 0; i < listado.Count; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = listado[i].zona_falla };
                else
                    list[i] = new { value = listado[i].id, name = listado[i].zona_falla };
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
        public JsonResult obtieneEmail(int numEmpleado = 0)
        {

            //obtiene todos los posibles valores
            empleados emp = db.empleados.Find(numEmpleado);

            //inicializa la lista de objetos
            var empleado = new object[1];

            if (emp == null || String.IsNullOrEmpty(emp.correo))
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
        ///Obtiene el puesto de un empleado
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult obtienePuestoEmpleado(int id_empleado = 0)
        {

            //obtiene todos los posibles valores
            empleados emp = db.empleados.Find(id_empleado);

            if (emp == null) //crea un empleado por defecto
                emp = new empleados
                {
                    id = id_empleado,
                    Area = new Area { descripcion = "NO DISPONIBLE" },
                    plantas = new plantas { descripcion = "NO DISPONIBLE" },
                    puesto1 = new puesto { descripcion = "NO DISPONIBLE" }
                };

            //inicializa la lista de objetos
            var empleado = new object[1];

            bool existe = db.AspNetUsers.Any(x => x.IdEmpleado == emp.id);


            empleado[0] = new
            {
                area = emp.Area == null ? "NO DISPONIBLE" : emp.Area.descripcion,
                plantas = emp.plantas == null ? "NO DISPONIBLE" : emp.plantas.descripcion,
                puesto = emp.puesto1 == null ? "NO DISPONIBLE" : emp.puesto1.descripcion,
                existe = existe

            };

            return Json(empleado, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene datos InventoryIntem
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult InventoryItemDetails(int id = 0)
        {

            //obtiene todos los posibles valores
            IT_inventory_items item = db.IT_inventory_items.Find(id);

            //inicializa la lista de objetos
            var objeto = new object[1];

            bool existe = db.IT_asignacion_hardware.Any(x => x.IT_asignacion_hardware_rel_items.Any(y => y.id_it_inventory_item == id) && x.es_asignacion_actual == true);

            int id_responsable = 0;

            string nombre = String.Empty;

            var asignacion = db.IT_asignacion_hardware_rel_items.Where(x => x.id_it_inventory_item == id && x.IT_asignacion_hardware.es_asignacion_actual == true && x.IT_asignacion_hardware.id_empleado == x.IT_asignacion_hardware.id_responsable_principal).FirstOrDefault();

            if (asignacion != null)
            {
                nombre = asignacion.IT_asignacion_hardware.empleados.ConcatNombre;
                id_responsable = asignacion.IT_asignacion_hardware.id_empleado;
            }

            objeto[0] = new
            {
                id_responsable = id_responsable,
                nombre_responsable = nombre,
                existe = existe

            };

            return Json(objeto, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene del empleado, según el id recibido
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult GetDatosEmpleados(int id = 0)
        {

            //obtiene todos los posibles valores
            empleados item = db.empleados.Find(id);

            //inicializa la lista de objetos
            var objeto = new object[1];

            //inicializa objeto principal
            if (item == null)
            {
                item = new empleados();
            }

            objeto[0] = new
            {
                nombre = !string.IsNullOrEmpty(item.ConcatNombre) && !string.IsNullOrEmpty(item.nombre) ? item.ConcatNombre : "--",
                num_empleado = !string.IsNullOrEmpty(item.numeroEmpleado) ? item.numeroEmpleado : "--",
                correo = !string.IsNullOrEmpty(item.correo) ? item.correo : "--",
                c8id = !string.IsNullOrEmpty(item.C8ID) ? item.C8ID : "--",
                planta = item.plantas != null ? item.plantas.descripcion : "--",
                area = item.Area != null ? item.Area.descripcion : "--",
                puesto = item.puesto1 != null ? item.puesto1.descripcion : "--",
                activo = item.activo == true ? "Activo" : "Inactivo",
                id_jefe_directo = item.empleados2 != null ? item.empleados2.id.ToString() : "--",
                nombre_jefe_directo = item.empleados2 != null ? item.empleados2.ConcatNombre : "--",

            };

            return Json(objeto, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene datos de la Línea seleccionada
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult InventoryLineDetails(int id = 0, int id_empleado = 0)
        {

            //obtiene todos los posibles valores
            var itemList = db.IT_asignacion_hardware.Where(x => x.es_asignacion_linea_actual && x.id_cellular_line == id && x.id_empleado != id_empleado).ToList();

            var objeto = new object[itemList.Count];

            int i = 0;
            //inicializa la lista de objetos
            foreach (var item in itemList)
            {
                objeto[i++] = new
                {
                    id_responsable = item.id_empleado,
                    nombre_responsable = item.empleados.ConcatNombre
                };
            }

            return Json(objeto, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene si un usuario está registrado segun el email
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult getUserByEmail(string correo)
        {

            //obtiene todos los posibles valores
            bool existe = db.AspNetUsers.Any(x => x.Email == correo);



            //inicializa la lista de objetos
            var item = new object[1];


            item[0] = new
            {
                existe = existe
            };

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene el rollo de bom según el material enviado
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneRollosBom(string material = "")
        {
            //obtiene todos los posibles valores
            List<bom_en_sap> listado = db.bom_en_sap.Where(p => p.Quantity > 0 && !p.Material.StartsWith("sm") && p.Material == material).ToList();

            //realiza un distict de los materiales
            List<string> distinctList = listado.Where(m => m.Material == material).Select(m => m.Component).Distinct().ToList();

            //inserta el valor por default
            distinctList.Insert(0, "-- Seleccione un valor --");

            //En caso de "Temporal" agrega temporal como opcion
            if (material.ToUpper().Contains("TEMPORAL"))
            {
                distinctList.Add("TEMPORAL");
            }

            //inicializa la lista de objetos
            var list = new object[distinctList.Count];

            //completa la lista de objetos
            for (int i = 0; i < distinctList.Count; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = distinctList[i] };
                else
                    list[i] = new { value = distinctList[i], name = distinctList[i] };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene los items activos del tipo de hardware recibido 
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneDescripcionInventoryItem(int? id_tipo_hardware = 0)
        {
            //obtiene todos los posibles valores
            List<IT_inventory_items> listado = db.IT_inventory_items.Where(p => p.active == true && p.id_inventory_type == id_tipo_hardware).ToList();

            //inicializa la lista de objetos
            var list = new object[listado.Count + 1];

            //inserta valor por defecto
            list[0] = new { value = "", name = "-- Selectione un valor --" };

            //obtiene el tipo de hardware
            var item_type = db.IT_inventory_hardware_type.Find(id_tipo_hardware);
            string tipo = String.Empty;

            if (item_type != null)
                tipo = item_type.descripcion;

            switch (tipo)
            {
                case Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES:
                    for (int i = 1; i < listado.Count + 1; i++)
                        list[i] = new { value = listado[i - 1].id, name = listado[i - 1].ConcatAccesoriesInfo };
                    break;
                //case Bitacoras.Util.IT_Tipos_Hardware.LAPTOP:
                //case Bitacoras.Util.IT_Tipos_Hardware.DESKTOP:
                //case Bitacoras.Util.IT_Tipos_Hardware.SERVER:
                default:
                    for (int i = 1; i < listado.Count + 1; i++)
                        list[i] = new { value = listado[i - 1].id, name = listado[i - 1].ConcatInfoGeneral };
                    break;
            }

            //completa la lista de objetos
            for (int i = 1; i < listado.Count + 1; i++)
            {
                list[i] = new { value = listado[i - 1].id, name = listado[i - 1].ConcatInfoGeneral };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene todas las fallas
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles que esten activas
        public JsonResult obtieneFallas()
        {
            //obtiene todos los posibles valores
            List<inspeccion_categoria_fallas> listado = db.inspeccion_categoria_fallas.Where(p => p.activo == true).ToList();

            String textoSelects = "<option value=''>-- Seleccione un valor --</option>";

            foreach (inspeccion_categoria_fallas item in listado)
            {

                textoSelects += "<optgroup label = '" + item.descripcion + "'>";

                foreach (inspeccion_fallas falla in item.inspeccion_fallas)
                {
                    if (falla.activo)
                        textoSelects += "<option value='" + falla.id + "'>" + falla.descripcion + "</option>";
                }

                textoSelects += "</optgroup>";
            }

            //inicializa la lista de objetos
            var list = new object[1];

            list[0] = new { text = textoSelects };

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene todas los software activos
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles que esten activas
        public JsonResult obtieneSoftware()
        {
            //obtiene todos los posibles valores
            List<IT_inventory_software> listado = db.IT_inventory_software.Where(p => p.activo == true).ToList();

            String textoSelects = "<option value=''>-- Seleccione un valor --</option>";

            foreach (IT_inventory_software item in listado)
            {
                textoSelects += "<option value='" + item.id + "'>" + item.descripcion + "</option>";
            }

            //inicializa la lista de objetos
            var list = new object[1];

            list[0] = new { text = textoSelects };

            return Json(list, JsonRequestBehavior.AllowGet);
        }



        ///<summary>
        ///Obtiene todas los empleados
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles que esten activas
        public JsonResult obtieneEmpleadosSelect()
        {
            //obtiene todos los posibles valores
            List<empleados> listado = db.empleados.Where(p => p.activo == true).ToList();

            String textoSelects = "<option value=''>-- Seleccione un valor --</option>";


            foreach (empleados item in listado)
            {
                textoSelects += "<option value='" + item.id + "'>" + item.ConcatNumEmpleadoNombre + "</option>";
            }

            //inicializa la lista de objetos
            var list = new object[1];

            list[0] = new { text = textoSelects };

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadFile(int? idDocumento, bool inline = false)
        {
            if (idDocumento == null)
            {
                return View("../Error/BadRequest");
            }

            biblioteca_digital archivo = db.biblioteca_digital.Find(idDocumento);

            if (archivo == null)
            {
                return View("../Error/NotFound");
            }


            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = archivo.Nombre,

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = inline,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(archivo.Datos, archivo.MimeType);

        }

        /// <summary>
        /// Metodo de prueba para obtener el pdf de facturas
        /// </summary>
        /// <param name="inline"></param>
        /// <returns></returns>
        public ActionResult DownloadFileDemo(bool inline = false)
        {

            string cadena = "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RzTPM4JQBmLxkYxFH9ikpisB03CkekctwxX92Y0EWfIdZ0q2Q6VWKUUU0nYju7rZUaVfqCyfRVoKKtYCZ3NADMuXlCNBpflkfyIiqjFiSTGyXE3kLhEMw2YtQk+6B7RlrpESKOZgjfA/JHu5vqW5zEMW9dlxY2+G9FJNr4R/ucp6gflkiojUqLIj/uw4qdLZ0Rb8r+UsVB65S8y1teA4VEMCQOdr8c2hVdP5bB7nCPQTwrgmkiPiP3AMqusKZW5kc3RyZWFtCmVuZG9iago4IDAgb2JqCjw8L1R5cGUvUGFnZS9NZWRpYUJveFswIDAgNTk1IDg0Ml0vUmVzb3VyY2VzPDwvUHJvY1NldCBbL1BERiAvVGV4dCAvSW1hZ2VCIC9JbWFnZUMgL0ltYWdlSV0vRm9udDw8L0YxIDEgMCBSL0YyIDQgMCBSPj4vWE9iamVjdDw8L2ltZzAgMiAwIFIvaW1nMSAzIDAgUi9pbWcyIDUgMCBSPj4+Pi9Db250ZW50cyA2IDAgUi9QYXJlbnQgNyAwIFI+PgplbmRvYmoKMSAwIG9iago8PC9UeXBlL0ZvbnQvU3VidHlwZS9UeXBlMS9CYXNlRm9udC9Db3VyaWVyLUJvbGQvRW5jb2RpbmcvV2luQW5zaUVuY29kaW5nPj4KZW5kb2JqCjQgMCBvYmoKPDwvVHlwZS9Gb250L1N1YnR5cGUvVHlwZTEvQmFzZUZvbnQvSGVsdmV0aWNhL0VuY29kaW5nL1dpbkFuc2lFbmNvZGluZz4+CmVuZG9iago3IDAgb2JqCjw8L1R5cGUvUGFnZXMvQ291bnQgMS9LaWRzWzggMCBSXS9JVFhUKDUuMi4xKT4+CmVuZG9iago5IDAgb2JqCjw8L1R5cGUvQ2F0YWxvZy9QYWdlcyA3IDAgUj4+CmVuZG9iagoxMCAwIG9iago8PC9Qcm9kdWNlcihpVGV4dFNoYXJwIDUuMi4xIFwoY1wpIDFUM1hUIEJWQkEpL0NyZWF0aW9uRGF0ZShEOjIwMjIxMjE0MTA1NDMxLTA2JzAwJykvTW9kRGF0ZShEOjIwMjIxMjE0MTA1NDMxLTA2JzAwJyk+PgplbmRvYmoKeHJlZgowIDExCjAwMDAwMDAwMDAgNjU1MzUgZiAKMDAwMDAxMDMxNyAwMDAwMCBuIAowMDAwMDAwMDE1IDAwMDAwIG4gCjAwMDAwMDAyNDQgMDAwMDAgbiAKMDAwMDAxMDQwOCAwMDAwMCBuIAowMDAwMDAwNTg2IDAwMDAwIG4gCjAwMDAwMDc5MDUgMDAwMDAgbiAKMDAwMDAxMDQ5NiAwMDAwMCBuIAowMDAwMDEwMTA2IDAwMDAwIG4gCjAwMDAwMTA1NTkgMDAwMDAgbiAKMDAwMDAxMDYwNCAwMDAwMCBuIAp0cmFpbGVyCjw8L1NpemUgMTEvUm9vdCA5IDAgUi9JbmZvIDEwIDAgUi9JRCBbPDY0MDQyNzcyMmExNGVlMDQ0ZWRhNWVhZDJlMzA3ODliPjxiMzhhMGIwMTQwMThiY2Y1NGUyZWRjNWM2NTBkMzU3YT5dPj4Kc3RhcnR4cmVmCjEwNzQwCiUlRU9GCg==";

            int mod4 = cadena.Length % 4;
            if (mod4 > 0)
            {
                cadena += new string('=', 4 - mod4);
            }

            var buffer = Convert.FromBase64String(cadena);
            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = "foo.pdf",

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(buffer, "application/pdf");

        }


    }
}