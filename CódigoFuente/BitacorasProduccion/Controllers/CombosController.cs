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
        public ActionResult MuestraArchivo(string uuid, bool inline = true)
        {
            //si el uuid es null
            if (String.IsNullOrEmpty(uuid))
            {
                ViewBag.Titulo = "¡Lo sentimos!¡La estructura del UUID buscado no es correcta!";
                ViewBag.Descripcion = "El UUID enviado es vacío o nulo.";
                return View("../Home/ErrorGenerico");
            }

            using (var db = new ATEBCOFIDIEntities())
            {

                var factura = db.CFDProveedor.Where(x => x.UUID == uuid).FirstOrDefault();

                if (factura == null)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se encontró el UUID indicado en la base de datos de COFIDI!";
                    ViewBag.Descripcion = "La factura no se encuentra disponible en la base de datos de COFIDI.";
                    return View("../Home/ErrorGenerico");
                }

                var archivo = db.CFDProveedorArchivos.Where(x => x.ID == factura.ID).FirstOrDefault();
                if (archivo == null)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se encontró el PDF asociado a esta factura!";
                    ViewBag.Descripcion = "La factura se encuentra en la base de datos de COFIDI, pero no se encuentra un PDF asociado.";
                    return View("../Home/ErrorGenerico");
                }

                string cadena = archivo.Archivo;


                int mod4 = cadena.Length % 4;
                if (mod4 > 0)
                {
                    cadena += new string('=', 4 - mod4);
                }

                var buffer = Convert.FromBase64String(cadena);
                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = uuid + ".pdf",

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = inline,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(buffer, "application/pdf");
            }         
        } 
        /// <summary>
        /// Metodo de prueba para obtener el pdf de facturas
        /// </summary>
        /// <param name="inline"></param>
        /// <returns></returns>
        public ActionResult MuestraArchivoXML(string uuid, bool inline = true)
        {
            //si el uuid es null
            if (String.IsNullOrEmpty(uuid))
            {
                ViewBag.Titulo = "¡Lo sentimos!¡La estructura del UUID buscado no es correcta!";
                ViewBag.Descripcion = "El UUID enviado es vacío o nulo.";
                return View("../Home/ErrorGenerico");
            }

            using (var db = new ATEBCOFIDIEntities())
            {

                var factura = db.CFDProveedor.Where(x => x.UUID == uuid).FirstOrDefault();

                if (factura == null)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se encontró el UUID indicado en la base de datos de COFIDI!";
                    ViewBag.Descripcion = "La factura no se encuentra disponible en la base de datos de COFIDI.";
                    return View("../Home/ErrorGenerico");
                }

                if (String.IsNullOrEmpty(factura.CFDOriginal))
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se encontró el documento XML!";
                    ViewBag.Descripcion = "La factura se encuentra en COFIDI, pero no hay un XML asociado.";
                    return View("../Home/ErrorGenerico");
                }

                return Content(factura.CFDOriginal, "text/xml");
            }         
        }
    }
}