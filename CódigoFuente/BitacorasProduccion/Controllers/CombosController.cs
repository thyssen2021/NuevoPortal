using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

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
            List<Area> listado = db.Area.Where(p => p.plantaClave.Value == clavePlanta && p.activo == true && !p.shared_services).ToList();

            //shared services
            if (clavePlanta == 99)
                listado = db.Area.Where(p => p.shared_services && p.activo == true).ToList();

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
                    list[i] = new { value = listado[i].clave, name = listado[i].ConcatDeptoCeCo };
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
        ///Obtiene los datos del id de empleado enviado
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult obtieneDatosEmpleado(int id_empleado = 0)
        {

            //obtiene todos los posibles valores
            empleados emp = db.empleados.Find(id_empleado);

            if (emp == null) //crea un empleado por defecto
                emp = new empleados
                {
                    id = id_empleado,
                    Area = new Area { descripcion = "" },
                    plantas = new plantas { descripcion = "" },
                    puesto1 = new puesto { descripcion = "" }
                };

            //obtiene el telefono asignado
            string phone_1 = string.Empty;
            if (emp.mostrar_telefono)
            {
                var lineas = emp.GetIT_Inventory_Cellular_LinesActivas();

                if (lineas.Count > 0)
                {
                    phone_1 = lineas.FirstOrDefault().numero_celular;
                }

            }

            //inicializa la lista de objetos
            var empleado = new object[1];

            empleado[0] = new
            {
                area = emp.Area == null ? string.Empty : emp.Area.descripcion,
                plantas = emp.plantas == null ? string.Empty : emp.plantas.descripcion,
                id_planta = emp.plantas == null ? string.Empty : emp.planta_clave.ToString(),
                puesto = emp.puesto1 == null ? String.Empty : emp.puesto1.descripcion,
                nombre = emp.nombre,
                correo = emp.correo,
                phone_1 = phone_1,
                apellidos = emp.apellido1 + (!string.IsNullOrEmpty(emp.apellido2) ? " " + emp.apellido2 : string.Empty),

            };

            return Json(empleado, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene los datos del id de planta enviado
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult obtieneDatosPlanta(int id_planta = 0)
        {

            //obtiene todos los posibles valores
            plantas planta = db.plantas.Find(id_planta);

            if (planta == null) //crea un empleado por defecto
                planta = new plantas
                {
                    clave = 0,
                };

            //inicializa la lista de objetos
            var plantaJSON = new object[1];

            plantaJSON[0] = new
            {
                calle = planta.tkorgstreet,
                ciudad = planta.tkorgpostaladdress,
                estado = planta.tkorgfedst,
                codigo_postal = planta.tkorgpostalcode,
            };

            return Json(plantaJSON, JsonRequestBehavior.AllowGet);
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
        ///Envia notificacion a Jefe Directo
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles que esten activas
        public JsonResult NotificacionRegistroJefeDirecto(int idJefeDirecto)
        {

            //inicializa la lista de objetos
            var list = new object[1];

            list[0] = new { result = "ERROR", msj = "No se ha iniciado la solicitud" };

            //envia la notificacion 
            try
            {
                //comprueba si ya existe una solicitud actual
                if (db.IT_solicitud_usuarios.Any(x => x.id_empleado == idJefeDirecto))
                {
                    list[0] = new { result = "DUPLICADO", msj = "Ya se existe una solicitud para el empleado actual." };
                    return Json(list, JsonRequestBehavior.AllowGet);
                }

                //crea un nuevo objeto de la solicitud
                IT_solicitud_usuarios solicitud = new IT_solicitud_usuarios
                {
                    id_empleado = idJefeDirecto,
                    comentario = "SOLICITUD DE USUARIO PARA JEFE DIRECTO EN AUTORIZACION DE MATRIZ DE REQUERIMIENTOS",
                    fecha_solicitud = DateTime.Now,
                    estatus = IT_solicitud_usuario_Status.CREADO,
                };

                //guarda en BD
                db.IT_solicitud_usuarios.Add(solicitud);
                db.SaveChanges();

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>(); //correos TO

                //-- INICIO POR TABLA NOTIFICACION
                List<notificaciones_correo> listadoNotificaciones = db.notificaciones_correo.Where(x => x.descripcion == NotificacionesCorreo.IT_SOLICITUD_PORTAL && x.activo).ToList();
                foreach (var n in listadoNotificaciones)
                {
                    //si el campo correo no está vacio
                    if (!String.IsNullOrEmpty(n.correo) && !n.id_empleado.HasValue)
                        correos.Add(n.correo);
                    //si tiene empleado asociado
                    else if (n.empleados != null && !String.IsNullOrEmpty(n.empleados.correo))
                        correos.Add(n.empleados.correo);
                }
                //-- FIN POR TABLA NOTIFICACION
                envioCorreo.SendEmailAsync(correos, "Se ha creado una solicitud de usuario para el Portal.", envioCorreo.getBodySolicitudUsuarioPortal(solicitud));
                list[0] = new { result = "OK", msj = "Se envió notificación a IT correctamente." };
            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", msj = "Ocurrió un error al enviar la solicitud" + e.Message };
            }

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

        ///<summary>
        ///Obtiene los almacenes por planta
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneAlmacenes(int id_planta = 0)
        {
            //obtiene todos los posibles valores
            List<RM_almacen> listado = db.RM_almacen.Where(p => p.plantaClave == id_planta && p.activo == true).ToList();

            //inserta el valor por default
            listado.Insert(0, new RM_almacen
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
        ///Obtiene los detalles del proveedor
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneClienteDetalles(int id_cliente = 0)
        {
            var detalles = new object[1];

            //obtiene todos los posibles valores
            clientes cliente = db.clientes.Find(id_cliente);

            if (cliente == null)
            {
                detalles[0] = new
                {
                    nombre = "",
                    direccion = ""
                };
                return Json(detalles, JsonRequestBehavior.AllowGet);
            }

            detalles[0] = new
            {
                nombre = Clases.Util.UsoStrings.RecortaString(cliente.descripcion, 50),
                direccion = Clases.Util.UsoStrings.RecortaString(cliente.ConcatDireccion, 100)
            };

            return Json(detalles, JsonRequestBehavior.AllowGet);
        }
        ///<summary>
        ///Obtiene los detalles del proveedor
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneProveedorDetalles(int id_proveedor = 0)
        {
            var detalles = new object[1];

            //obtiene todos los posibles valores
            proveedores proveedor = db.proveedores.Find(id_proveedor);

            if (proveedor == null)
            {
                detalles[0] = new
                {
                    nombre = "",
                    direccion = ""
                };
                return Json(detalles, JsonRequestBehavior.AllowGet);
            }

            detalles[0] = new
            {
                nombre = Clases.Util.UsoStrings.RecortaString(proveedor.descripcion, 50),
                direccion = Clases.Util.UsoStrings.RecortaString(proveedor.ConcatDireccion, 100)
            };

            return Json(detalles, JsonRequestBehavior.AllowGet);
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
                FileName = UsoStrings.ReemplazaCaracteres(archivo.Nombre),

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = inline,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(archivo.Datos, archivo.MimeType);

        }

        ///<summary>
        ///Obtiene las lineas de la plantas enviada
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult RH_MR_creacion_modificacion(int id_empleado = 0)
        {
            //obtiene todos los posibles valores
            var empleado = db.empleados.Find(id_empleado);

            //inicializa la lista de objetos
            var list = new object[1];

            if (empleado == null)
                list[0] = new { result = "ERROR", message = "No se pudo obtener el empleado.", tipo = Bitacoras.Util.IT_MR_tipo.CREACION };
            else if (empleado.IT_matriz_requerimientos.Count == 0)
                list[0] = new { result = "OK", message = "No hay solicitudes previas.", tipo = Bitacoras.Util.IT_MR_tipo.CREACION };
            else
                list[0] = new { result = "OK", message = "Hay solicitudes previas.", tipo = Bitacoras.Util.IT_MR_tipo.MODIFICACION };

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region budget forecast
        ///<summary>
        ///Obtiene los detalles de un reporte de Forecast
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult DetallesReporteBGForecast(int id_reporte = 0)
        {
            //obtiene todos los posibles valores
            var reporte = db.BG_Forecast_reporte.Find(id_reporte);

            //inicializa la lista de objetos
            var list = new object[1];

            if (reporte == null)
                list[0] = new { result = "ERROR", message = "No se pudo obtener el reporte." };
            else
                list[0] = new
                {
                    result = "OK",
                    message = "Datos de reporte obtenido",
                    r_descripcion = reporte.descripcion,
                    r_activo = reporte.activo,
                    r_id_ihs_version = reporte.id_ihs_version,
                    r_ihs_ConcatVersion = reporte.BG_IHS_versiones.ConcatVersion,
                };

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene los detalles del la planta de BH_IHS
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneBgIHSPlant(int? version, string mnemonic_plant = "")
        {
            var detalles = new object[1];

            //obtiene todos los posibles valores
            BG_IHS_plantas planta = db.BG_IHS_plantas.FirstOrDefault(x => x.id_ihs_version == version && x.mnemonic_plant == mnemonic_plant);

            if (planta == null)
            {
                detalles[0] = new
                {
                    mnemonic_plant = "",
                    descripcion = ""
                };
                return Json(detalles, JsonRequestBehavior.AllowGet);
            }

            detalles[0] = new
            {
                mnemonic_plant = Clases.Util.UsoStrings.RecortaString(planta.mnemonic_plant, 50),
                descripcion = Clases.Util.UsoStrings.RecortaString(planta.descripcion, 100)
            };

            return Json(detalles, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene las areas segun la planta recibida
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtienePlantasBGCombinaciones(string manufacturer, int id_ihs_version)
        {
            //obtiene todos los posibles valores
            List<string> listaManufacturer = db.BG_IHS_item.Where(x => x.id_ihs_version == id_ihs_version && x.manufacturer == manufacturer).Select(x => x.production_plant).Distinct().ToList();

            //inicializa la lista de objetos
            var list = new object[listaManufacturer.Count + 1];

            //completa la lista de objetos
            for (int i = 0; i < listaManufacturer.Count + 1; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = "-- Seleccione --" };
                else
                    list[i] = new { value = listaManufacturer[i - 1], name = listaManufacturer[i - 1] };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        ///<summary>
        ///Obtiene las areas segun la planta recibida
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneBrandBGCombinaciones(string manufacturer_group, string production_plant, int id_ihs_version)
        {
            //obtiene todos los posibles valores
            List<string> listaBrand = db.BG_IHS_item.Where(x => x.id_ihs_version == id_ihs_version && x.production_plant == production_plant && x.manufacturer == manufacturer_group).Select(x => x.production_brand).Distinct().ToList();

            //inicializa la lista de objetos
            var list = new object[listaBrand.Count + 1];

            //completa la lista de objetos
            for (int i = 0; i < listaBrand.Count + 1; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = "-- Seleccione --" };
                else
                    list[i] = new { value = listaBrand[i - 1], name = listaBrand[i - 1] };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult obtieneNameplateBGCombinaciones(string manufacturer_group, string production_plant, int id_ihs_version)
        {
            //obtiene todos los posibles valores
            List<string> listaNameplate = db.BG_IHS_item.Where(x => x.id_ihs_version == id_ihs_version && x.production_plant == production_plant && x.manufacturer == manufacturer_group).Select(x => x.production_nameplate).Distinct().ToList();

            //inicializa la lista de objetos
            var list = new object[listaNameplate.Count + 1];

            //completa la lista de objetos
            for (int i = 0; i < listaNameplate.Count + 1; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = "-- Seleccione --" };
                else
                    list[i] = new { value = listaNameplate[i - 1], name = listaNameplate[i - 1] };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Revista Digital

        /// <summary>
        /// Registra visita, like o dislike
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonpResult RegistraVisita(string tipo = "")
        {
            var resultado = new object[1];

            //string x = Request.UserHostName;

            try
            {

                string nombreEquipo = String.Empty;
                // string nombreUsuario =  String.Empty;

                // nombreEquipo = DetermineCompName(ip);


                //if (String.IsNullOrEmpty(nombreEquipo))
                //    nombreEquipo = ip;

                RD_hits hit = new RD_hits
                {
                    fecha = DateTime.Now,
                    // usuario = nombreEquipo,
                    tipo = tipo
                };


                db.RD_hits.Add(hit);

                db.SaveChanges();

                resultado[0] = new { result = "OK", message = "Nueva Entrada", contador = db.RD_hits.Where(x => x.tipo == tipo).Count() };
            }
            catch (Exception e)
            {
                resultado[0] = new { result = "ERROR", message = e.Message, contador = db.RD_hits.Where(x => x.tipo == tipo).Count() };
            }

            return new JsonpResult(resultado);
        }

        ///<summary>
        ///Obtiene los detalles del proveedor
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneEtiquetaDetalles(int id = 0)
        {
            var detalles = new object[1];

            //obtiene todos los posibles valores
            CI_conteo_inventario item = db.CI_conteo_inventario.Find(id);

            if (item == null)
            {
                detalles[0] = new
                {
                    id = 0,
                    pieces = 0,
                    unrestricted = 0,
                    blocked = 0,
                    in_quality = 0,
                    total_piezas_min = 0,
                    total_piezas_max = 0,

                };
                return Json(detalles, JsonRequestBehavior.AllowGet);
            }

            detalles[0] = new
            {
                id = item.id,
                pieces = item.pieces != null ? item.pieces : 0,
                unrestricted = item.unrestricted != null ? item.unrestricted : 0,
                blocked = item.blocked != null ? item.blocked : 0,
                in_quality = item.in_quality != null ? item.in_quality : 0,
                total_piezas_min = item.total_piezas_min != null ? item.total_piezas_min : 0,
                total_piezas_max = item.total_piezas_max != null ? item.total_piezas_max : 0,
            };

            return Json(detalles, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]

        public JsonpResult GetVisita(string tipo = "")
        {
            var resultado = new object[1];

            //string x = Request.UserHostName;

            try
            {
                resultado[0] = new { result = "OK", message = "Obtencion de cantidad", contador = db.RD_hits.Where(x => x.tipo == tipo).Count() };
            }
            catch (Exception e)
            {
                resultado[0] = new { result = "ERROR", message = e.Message, contador = db.RD_hits.Where(x => x.tipo == tipo).Count() };
            }

            return new JsonpResult(resultado);
        }


        [NonAction]
        public static string DetermineCompName(string IP)
        {
            try
            {
                System.Net.IPAddress myIP = System.Net.IPAddress.Parse(IP);
                System.Net.IPHostEntry GetIPHost = System.Net.Dns.GetHostEntry(myIP);
                List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
                return compName.First();
            }
            catch
            {
                return null;
            }

        }
        #endregion


        ///<summary>
        ///Obtiene las lineas de la plantas enviada
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneClientesReportePesadas(int clavePlanta = 0)
        {
            //obtiene todos los posibles valores

            List<string> clientes = db.view_datos_base_reporte_pesadas.Where(x => x.clave_planta == clavePlanta).Select(x => x.invoiced_to).Distinct().OrderBy(x => x).ToList();


            //inserta el valor por default
            clientes.Insert(0, "-- Seleccione --");

            //inicializa la lista de objetos
            var list = new object[clientes.Count];

            //completa la lista de objetos
            for (int i = 0; i < clientes.Count; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = clientes[i] };
                else
                    list[i] = new { value = clientes[i], name = clientes[i] };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region scdm
        ///<summary>
        ///Actualiza o crea los datos de la solictud de materiales
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult SCDM_updateMaterialesXSolicitud(int? idSolicitud, string material, string ejecucion_correcta, string mensaje_sap, string nuevo_numero_material)
        {
            //inicializa la lista de objetos
            var result = new object[1];
            var rel_item_material = db.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.id_solicitud == idSolicitud && x.numero_material == material);
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);

            if (rel_item_material == null || solicitud == null)
            {
                result[0] = new
                {
                    mensaje = "No hay rel item material en BD."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //recorta los strings 
            if (ejecucion_correcta.Length > 120)
                ejecucion_correcta = Clases.Util.UsoStrings.RecortaString(ejecucion_correcta, 120);

            if (mensaje_sap.Length > 120)
                mensaje_sap = Clases.Util.UsoStrings.RecortaString(mensaje_sap, 120);

            //Determina si es create o update
            if (rel_item_material.SCDM_solicitud_item_material_datos_sap == null)
            {
                rel_item_material.SCDM_solicitud_item_material_datos_sap = new SCDM_solicitud_item_material_datos_sap
                {
                    materiales_x_solicitud_ejecucion_correcta = ejecucion_correcta,
                    materiales_x_solicitud_mensaje_sap = mensaje_sap,
                    materiales_x_solicitud_nuevo_numero_material = nuevo_numero_material
                };
            }
            else
            { //update 
                rel_item_material.SCDM_solicitud_item_material_datos_sap.materiales_x_solicitud_mensaje_sap = mensaje_sap;
                rel_item_material.SCDM_solicitud_item_material_datos_sap.materiales_x_solicitud_ejecucion_correcta = ejecucion_correcta;
                rel_item_material.SCDM_solicitud_item_material_datos_sap.materiales_x_solicitud_nuevo_numero_material = nuevo_numero_material;
            }

            //Actualiza Formato de  Ordenes de Compra
            foreach (var item in solicitud.SCDM_solicitud_rel_orden_compra.Where(x => x.num_material == material))
                item.num_material = nuevo_numero_material;

            //actualiza el número de material
            rel_item_material.numero_material = nuevo_numero_material;

            //actualiza la lista tecnica
            foreach (var item in solicitud.SCDM_solicitud_rel_lista_tecnica)
            {
                if (item.componente == material)
                    item.componente = nuevo_numero_material;
                if (item.resultado == material)
                    item.resultado = nuevo_numero_material;
            }


            try
            {
                db.SaveChanges();
                result[0] = new
                {
                    mensaje = "Correcto."

                };
            }
            catch (Exception ex)
            {
                result[0] = new
                {
                    mensaje = "Error: " + ex.Message,
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Actualiza o crea los datos de la solictud de materiales
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult SCDM_updateClass001(int? idSolicitud, string material, string ejecucion_correcta, string mensaje_sap, string tratamiento)
        {
            //inicializa la lista de objetos
            var result = new object[1];
            var rel_item_material = db.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.id_solicitud == idSolicitud && x.numero_material == material);
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);

            if (rel_item_material == null || solicitud == null)
            {
                result[0] = new
                {
                    mensaje = "No hay rel item material en BD."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


            //recorta los strings 
            if (ejecucion_correcta.Length > 120)
                ejecucion_correcta = Clases.Util.UsoStrings.RecortaString(ejecucion_correcta, 120);

            if (mensaje_sap.Length > 120)
                mensaje_sap = Clases.Util.UsoStrings.RecortaString(mensaje_sap, 120);

            //Determina si es create o update
            if (rel_item_material.SCDM_solicitud_item_material_datos_sap == null)
            {
                rel_item_material.SCDM_solicitud_item_material_datos_sap = new SCDM_solicitud_item_material_datos_sap
                {
                    class_001_ejecucion_correcta = ejecucion_correcta,
                    class_001_mensaje_sap = mensaje_sap,
                };
            }
            else
            { //update 

                rel_item_material.SCDM_solicitud_item_material_datos_sap.class_001_mensaje_sap = mensaje_sap;
                rel_item_material.SCDM_solicitud_item_material_datos_sap.class_001_ejecucion_correcta = ejecucion_correcta;

            }

            try
            {
                //actualiza el tratamiento
                rel_item_material.tratamiento_superficial = tratamiento;

                db.SaveChanges();
                result[0] = new
                {
                    mensaje = "Correcto."

                };
            }
            catch (Exception ex)
            {
                result[0] = new
                {
                    mensaje = "Error: " + ex.Message,
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Actualiza o crea los datos de la solictud de materiales
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult SCDM_updateClass023(int? idSolicitud, string material, string ejecucion_correcta, string mensaje_sap)
        {
            //inicializa la lista de objetos
            var result = new object[1];
            var rel_item_material = db.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.id_solicitud == idSolicitud && x.numero_material == material);
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);

            if (rel_item_material == null || solicitud == null)
            {
                result[0] = new
                {
                    mensaje = "No hay rel item material en BD."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //recorta los strings 
            if (ejecucion_correcta.Length > 120)
                ejecucion_correcta = Clases.Util.UsoStrings.RecortaString(ejecucion_correcta, 120);

            if (mensaje_sap.Length > 120)
                mensaje_sap = Clases.Util.UsoStrings.RecortaString(mensaje_sap, 120);

            //Determina si es create o update
            if (rel_item_material.SCDM_solicitud_item_material_datos_sap == null)
            {
                rel_item_material.SCDM_solicitud_item_material_datos_sap = new SCDM_solicitud_item_material_datos_sap
                {
                    class_023_ejecucion_correcta = ejecucion_correcta,
                    class_023_mensaje_sap = mensaje_sap,

                };
            }
            else
            { //update 
                rel_item_material.SCDM_solicitud_item_material_datos_sap.class_023_mensaje_sap = mensaje_sap;
                rel_item_material.SCDM_solicitud_item_material_datos_sap.class_023_ejecucion_correcta = ejecucion_correcta;
            }

            try
            {
                db.SaveChanges();
                result[0] = new
                {
                    mensaje = "Correcto."

                };
            }
            catch (Exception ex)
            {
                result[0] = new
                {
                    mensaje = "Error: " + ex.Message,
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Actualiza o crea los datos de la solictud de materiales
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult SCDM_updateAUM(int? idSolicitud, string material, string ejecucion_correcta, string mensaje_sap)
        {
            //inicializa la lista de objetos
            var result = new object[1];
            var rel_item_material = db.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.id_solicitud == idSolicitud && x.numero_material == material);
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);

            if (rel_item_material == null || solicitud == null)
            {
                result[0] = new
                {
                    mensaje = "No hay rel item material en BD."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //recorta los strings 
            if (ejecucion_correcta.Length > 120)
                ejecucion_correcta = Clases.Util.UsoStrings.RecortaString(ejecucion_correcta, 120);

            if (mensaje_sap.Length > 120)
                mensaje_sap = Clases.Util.UsoStrings.RecortaString(mensaje_sap, 120);

            //Determina si es create o update
            if (rel_item_material.SCDM_solicitud_item_material_datos_sap == null)
            {
                rel_item_material.SCDM_solicitud_item_material_datos_sap = new SCDM_solicitud_item_material_datos_sap
                {
                    class_023_ejecucion_correcta = ejecucion_correcta,
                    class_023_mensaje_sap = mensaje_sap,
                };
            }
            else
            { //update 
                rel_item_material.SCDM_solicitud_item_material_datos_sap.aum_ejecucion_correcta = mensaje_sap;
                rel_item_material.SCDM_solicitud_item_material_datos_sap.aum_mensaje_sap = ejecucion_correcta;
            }

            try
            {
                db.SaveChanges();
                result[0] = new
                {
                    mensaje = "Correcto."

                };
            }
            catch (Exception ex)
            {
                result[0] = new
                {
                    mensaje = "Error: " + ex.Message,
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Actualiza o crea los datos de la solictud de materiales
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult SCDM_updateDeliveringPlant(int? idSolicitud, string material, string ejecucion_correcta, string mensaje_sap)
        {
            //inicializa la lista de objetos
            var result = new object[1];
            var rel_item_material = db.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.id_solicitud == idSolicitud && x.numero_material == material);
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);

            if (rel_item_material == null || solicitud == null)
            {
                result[0] = new
                {
                    mensaje = "No hay rel item material en BD."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //recorta los strings 
            if (ejecucion_correcta.Length > 120)
                ejecucion_correcta = Clases.Util.UsoStrings.RecortaString(ejecucion_correcta, 120);

            if (mensaje_sap.Length > 120)
                mensaje_sap = Clases.Util.UsoStrings.RecortaString(mensaje_sap, 120);

            //Determina si es create o update
            if (rel_item_material.SCDM_solicitud_item_material_datos_sap == null)
            {
                rel_item_material.SCDM_solicitud_item_material_datos_sap = new SCDM_solicitud_item_material_datos_sap
                {
                    dp_ejecucion_correcta = ejecucion_correcta,
                    dp_mensaje_sap = mensaje_sap,
                };
            }
            else
            { //update 
                rel_item_material.SCDM_solicitud_item_material_datos_sap.dp_ejecucion_correcta = ejecucion_correcta;
                rel_item_material.SCDM_solicitud_item_material_datos_sap.dp_mensaje_sap = mensaje_sap;
            }

            try
            {
                db.SaveChanges();
                result[0] = new
                {
                    mensaje = "Correcto."

                };
            }
            catch (Exception ex)
            {
                result[0] = new
                {
                    mensaje = "Error: " + ex.Message,
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        ///<summary>
        ///Actualiza o crea los datos de la solictud de materiales
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult SCDM_updateBudget(int? idSolicitud, string material, string ejecucion_correcta, string mensaje_sap)
        {
            //inicializa la lista de objetos
            var result = new object[1];
            var rel_item_material = db.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.id_solicitud == idSolicitud && x.numero_material == material);
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);
            var rel_cambios_budget = db.SCDM_solicitud_rel_cambio_budget.FirstOrDefault(x => x.id_solicitud == idSolicitud && x.material_existente == material);
            var rel_creacion_referencia = db.SCDM_solicitud_rel_creacion_referencia.FirstOrDefault(x => x.id_solicitud == idSolicitud && x.nuevo_material == material);
            var rel_cambio_ingenieria = db.SCDM_solicitud_rel_cambio_ingenieria.FirstOrDefault(x => x.id_solicitud == idSolicitud && x.material_existente == material);

            if (solicitud == null || (rel_item_material == null && rel_cambios_budget == null && rel_cambios_budget == null && rel_creacion_referencia == null && rel_cambio_ingenieria == null))
            {
                result[0] = new
                {
                    mensaje = "No se encontraron los elementos de la solicitud en BD."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //recorta los strings 
            if (ejecucion_correcta.Length > 120)
                ejecucion_correcta = Clases.Util.UsoStrings.RecortaString(ejecucion_correcta, 120);

            if (mensaje_sap.Length > 120)
                mensaje_sap = Clases.Util.UsoStrings.RecortaString(mensaje_sap, 120);

            //ACTUALIZA REL ITEM MATERIAL
            if (rel_item_material != null)
                //Determina si es create o update
                if (rel_item_material.SCDM_solicitud_item_material_datos_sap == null)
                {
                    rel_item_material.SCDM_solicitud_item_material_datos_sap = new SCDM_solicitud_item_material_datos_sap
                    {
                        budget_ejecucion_correcta = ejecucion_correcta,
                        budget_mensaje_sap = mensaje_sap,
                    };
                }
                else
                { //update 
                    rel_item_material.SCDM_solicitud_item_material_datos_sap.budget_mensaje_sap = mensaje_sap;
                    rel_item_material.SCDM_solicitud_item_material_datos_sap.budget_ejecucion_correcta = ejecucion_correcta;
                }

            //ACTUALIZA REL ITEM BUDGET
            if (rel_cambios_budget != null)
            {  //update, porque ya debe exitir
                rel_cambios_budget.resultado = mensaje_sap;
                rel_cambios_budget.ejecucion_correcta = ejecucion_correcta;
            }

            //ACTUALIZA REL ITEM CREACION ING
            if (rel_creacion_referencia != null)
            {  //update, porque ya debe exitir
                rel_creacion_referencia.resultado_budget = mensaje_sap;
                rel_creacion_referencia.ejecucion_correcta_budget = ejecucion_correcta;
            }

            //ACTUALIZA REL ITEM Cambio Ingenieria
            if (rel_cambio_ingenieria != null)
            {  //update, porque ya debe exitir
                rel_cambio_ingenieria.resultado_budget = mensaje_sap;
                rel_cambio_ingenieria.ejecucion_correcta_budget = ejecucion_correcta;
            }

            try
            {
                db.SaveChanges();
                result[0] = new
                {
                    mensaje = "Correcto."

                };
            }
            catch (Exception ex)
            {
                result[0] = new
                {
                    mensaje = "Error: " + ex.Message,
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        ///<summary>
        ///Actualiza o crea los datos de la solictud de materiales
        ///</summary>
        ///<return> 
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult SCDM_updateCreacionReferencia(int? idSolicitud, string material, string materialAnterior, string ejecucion_correcta, string mensaje_sap, string mensaje_sap_update)
        {
            //inicializa la lista de objetos
            var result = new object[1];
            var item_creacion_referencia = db.SCDM_solicitud_rel_creacion_referencia.FirstOrDefault(x => x.id_solicitud == idSolicitud && x.nuevo_material == materialAnterior);
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);

            if (item_creacion_referencia == null || solicitud == null)
            {
                result[0] = new
                {
                    mensaje = "No hay rel item material en BD."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //recorta los strings 
            if (ejecucion_correcta.Length > 120)
                ejecucion_correcta = Clases.Util.UsoStrings.RecortaString(ejecucion_correcta, 120);

            if (mensaje_sap.Length > 120)
                mensaje_sap = Clases.Util.UsoStrings.RecortaString(mensaje_sap, 120);

            if (mensaje_sap_update.Length > 120)
                mensaje_sap_update = Clases.Util.UsoStrings.RecortaString(mensaje_sap_update, 120);

            try
            {
                item_creacion_referencia.nuevo_material = material;
                item_creacion_referencia.ejecucion_correcta = ejecucion_correcta;
                item_creacion_referencia.resultado = mensaje_sap;
                item_creacion_referencia.resultado_update = mensaje_sap_update;

                //Actualiza Formato de  Ordenes de Compra
                foreach (var item in solicitud.SCDM_solicitud_rel_orden_compra.Where(x => x.num_material == materialAnterior))
                    item.num_material = material;


                //actualiza la lista tecnica
                foreach (var item in solicitud.SCDM_solicitud_rel_lista_tecnica)
                {
                    if (item.componente == materialAnterior)
                        item.componente = material;
                    if (item.resultado == materialAnterior)
                        item.resultado = material;
                }

                db.SaveChanges();
                result[0] = new
                {
                    mensaje = "Correcto."

                };
            }
            catch (Exception ex)
            {
                result[0] = new
                {
                    mensaje = "Error: " + ex.Message,
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Actualiza o crea los datos de la solictud de materiales
        ///</summary>
        ///<return> 
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult SCDM_updateCambioIngenieria(int? idSolicitud, string material, string ejecucion_correcta, string mensaje_sap)
        {
            //inicializa la lista de objetos
            var result = new object[1];
            var itemCambioIngenieria = db.SCDM_solicitud_rel_cambio_ingenieria.FirstOrDefault(x => x.id_solicitud == idSolicitud && x.material_existente == material);
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);

            if (itemCambioIngenieria == null || solicitud == null)
            {
                result[0] = new
                {
                    mensaje = "No hay rel item material en BD."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //recorta los strings 
            if (ejecucion_correcta.Length > 120)
                ejecucion_correcta = Clases.Util.UsoStrings.RecortaString(ejecucion_correcta, 120);

            if (mensaje_sap.Length > 120)
                mensaje_sap = Clases.Util.UsoStrings.RecortaString(mensaje_sap, 120);



            try
            {

                itemCambioIngenieria.ejecucion_correcta = ejecucion_correcta;
                itemCambioIngenieria.resultado = mensaje_sap;

                //Actualiza Formato de  Ordenes de Compra
                foreach (var item in solicitud.SCDM_solicitud_rel_orden_compra.Where(x => x.num_material == material))
                    item.num_material = material;


                //actualiza la lista tecnica
                foreach (var item in solicitud.SCDM_solicitud_rel_lista_tecnica)
                {
                    if (item.componente == material)
                        item.componente = material;
                    if (item.resultado == material)
                        item.resultado = material;
                }

                db.SaveChanges();
                result[0] = new
                {
                    mensaje = "Correcto."

                };
            }
            catch (Exception ex)
            {
                result[0] = new
                {
                    mensaje = "Error: " + ex.Message,
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Actualiza o crea los datos de la solictud de materiales
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult SCDM_updateExtension(int? idSolicitud, string material, string ejecucion_correcta, string mensaje_sap, string sloc)
        {
            //inicializa la lista de objetos
            var result = new object[1];
            SCDM_solicitud_rel_extension rel_item_extension = null;

            //busca por item material
            rel_item_extension = db.SCDM_solicitud_rel_extension.FirstOrDefault(x =>
                                    x.SCDM_solicitud_rel_item_material != null
                                    && x.SCDM_solicitud_rel_item_material.id_solicitud == idSolicitud
                                    && x.SCDM_solicitud_rel_item_material.numero_material == material
                                    && x.SCDM_cat_storage_location.clave == sloc
                                    );

            //busca por creacion con referencia
            if (rel_item_extension == null)
            {
                rel_item_extension = db.SCDM_solicitud_rel_extension.FirstOrDefault(x =>
                                  x.SCDM_solicitud_rel_creacion_referencia != null
                                  && x.SCDM_solicitud_rel_creacion_referencia.id_solicitud == idSolicitud
                                  && x.SCDM_solicitud_rel_creacion_referencia.nuevo_material == material
                                  && x.SCDM_cat_storage_location.clave == sloc
                                  );
            }

            //busca por extension usuario
            if (rel_item_extension == null)
            {
                rel_item_extension = db.SCDM_solicitud_rel_extension.FirstOrDefault(x =>
                                  x.SCDM_solicitud_rel_extension_usuario != null
                                  && x.SCDM_solicitud_rel_extension_usuario.id_solicitud == idSolicitud
                                  && x.SCDM_solicitud_rel_extension_usuario.material == material
                                  && x.SCDM_cat_storage_location.clave == sloc
                                  );
            }



            //obtiene la solicitud
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);

            var rel_item_material = solicitud.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.numero_material == material);
            var rel_item_creacion_referencia = solicitud.SCDM_solicitud_rel_creacion_referencia.FirstOrDefault(x => x.nuevo_material == material);
            var rel_item_extension_usuario = solicitud.SCDM_solicitud_rel_extension_usuario.FirstOrDefault(x => x.material == material);


            int? id_rel_item_material = null;
            int? id_rel_creacion_referencia = null;
            int? id_rel_extension_usuario = null;

            //asigna los valores correspondientes
            if (rel_item_material != null)
                id_rel_item_material = rel_item_material.id;
            if (rel_item_creacion_referencia != null)
                id_rel_creacion_referencia = rel_item_creacion_referencia.id;
            if (rel_item_extension_usuario != null)
                id_rel_extension_usuario = rel_item_extension_usuario.id;


            if (solicitud == null)
            {
                result[0] = new
                {
                    mensaje = "No hay rel item material en BD."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //recorta los strings 
            if (ejecucion_correcta.Length > 120)
                ejecucion_correcta = Clases.Util.UsoStrings.RecortaString(ejecucion_correcta, 120);

            if (mensaje_sap.Length > 120)
                mensaje_sap = Clases.Util.UsoStrings.RecortaString(mensaje_sap, 120);

            //Determina si es create o update
            if (rel_item_extension == null) //CREATE
            {
                rel_item_extension = new SCDM_solicitud_rel_extension
                {
                    id_solicitud_rel_item_material = id_rel_item_material,
                    id_solicitud_rel_creacion_referencia = id_rel_creacion_referencia,
                    id_rel_solicitud_extension_usuario = id_rel_extension_usuario,
                    id_cat_storage_location = db.SCDM_cat_storage_location.First(x => x.clave == sloc).id,
                    extension_ejecucion_correcta = ejecucion_correcta,
                    extension_mensaje_sap = mensaje_sap,
                };
                db.SCDM_solicitud_rel_extension.Add(rel_item_extension);
            }
            else
            { //update 
                rel_item_extension.extension_mensaje_sap = mensaje_sap;
                rel_item_extension.extension_ejecucion_correcta = ejecucion_correcta;
            }

            try
            {
                db.SaveChanges();
                result[0] = new
                {
                    mensaje = "Correcto."

                };
            }
            catch (Exception ex)
            {
                result[0] = new
                {
                    mensaje = "Error: " + ex.Message,
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        ///<summary>
        ///Actualiza o crea los datos de la solictud de materiales
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult SCDM_updateExtensionUsuario(int? idSolicitud, string material, string ejecucion_correcta, string mensaje_sap)
        {
            //inicializa la lista de objetos
            var result = new object[1];
            SCDM_solicitud_rel_extension_usuario rel_item_extension = null;

            rel_item_extension = db.SCDM_solicitud_rel_extension_usuario.FirstOrDefault(x =>
                                       x.id_solicitud == idSolicitud
                                    && x.material == material
                                    );

            //obtiene la solicitud
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);


            if (solicitud == null)
            {
                result[0] = new
                {
                    mensaje = "No hay rel item material en BD."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //recorta los strings 
            if (ejecucion_correcta.Length > 120)
                ejecucion_correcta = Clases.Util.UsoStrings.RecortaString(ejecucion_correcta, 120);

            if (mensaje_sap.Length > 120)
                mensaje_sap = Clases.Util.UsoStrings.RecortaString(mensaje_sap, 120);

            //Determina si es create o update
            if (rel_item_extension != null) //UPDATE
            {
                rel_item_extension.mensaje_sap = mensaje_sap;
                rel_item_extension.ejecucion_correcta = ejecucion_correcta;
            }

            try
            {
                db.SaveChanges();
                result[0] = new
                {
                    mensaje = "Correcto."

                };
            }
            catch (Exception ex)
            {
                result[0] = new
                {
                    mensaje = "Error: " + ex.Message,
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        ///<summary>
        ///Actualiza o crea los datos de la solictud de materiales
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult SCDM_updateExtensionAlmacenes(int? idSolicitud, string material, string ejecucion_correcta, string mensaje_sap, string almacen, string almacen_tipo, string ubicacion, string tipoSolicitud)
        {
            //inicializa la lista de objetos
            var result = new object[1];
            SCDM_solicitud_rel_extension_almacenes rel_item_extension_almacen = null;

            //busca por item material
            rel_item_extension_almacen = db.SCDM_solicitud_rel_extension_almacenes.FirstOrDefault(x =>
                                      (x.SCDM_solicitud_rel_item_material.id_solicitud == idSolicitud
                                          || x.SCDM_solicitud_rel_creacion_referencia.id_solicitud == idSolicitud
                                          || x.SCDM_solicitud_rel_extension_usuario.id_solicitud == idSolicitud
                                          )
                                    && (x.SCDM_solicitud_rel_item_material.numero_material == material
                                            || x.SCDM_solicitud_rel_creacion_referencia.nuevo_material == material
                                            || x.SCDM_solicitud_rel_extension_usuario.material == material
                                            )
                                    && x.SCDM_cat_almacenes.warehouse == almacen
                                    && x.SCDM_cat_almacenes.storage_type == almacen_tipo
                                    );



            //obtiene la solicitud
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);

            int? id_rel_item_material = null, id_solicitud_rel_item_creacion_referencia = null, id_rel_extension_usuario = null;

            //obtiene el id relacionado de creacion de materiales
            var rel_item_material = solicitud.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.numero_material == material);
            //asigna los valores correspondientes
            if (rel_item_material != null)
                id_rel_item_material = rel_item_material.id;

            //obtiene el idRelacionado de creación con referencia
            var rel_item_creacion_referencia = solicitud.SCDM_solicitud_rel_creacion_referencia.FirstOrDefault(x => x.nuevo_material == material);
            //asigna los valores correspondientes
            if (rel_item_creacion_referencia != null)
                id_solicitud_rel_item_creacion_referencia = rel_item_creacion_referencia.id;

            //obtiene el idRelacionado a Extension Usuario
            var rel_item_extension_usuario = solicitud.SCDM_solicitud_rel_extension_usuario.FirstOrDefault(x => x.material == material);
            //asigna los valores correspondientes
            if (rel_item_extension_usuario != null)
                id_rel_extension_usuario = rel_item_extension_usuario.id;


            if (solicitud == null)
            {
                result[0] = new
                {
                    mensaje = "No hay rel item material en BD."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //recorta los strings 
            if (ejecucion_correcta.Length > 120)
                ejecucion_correcta = Clases.Util.UsoStrings.RecortaString(ejecucion_correcta, 120);

            if (mensaje_sap.Length > 120)
                mensaje_sap = Clases.Util.UsoStrings.RecortaString(mensaje_sap, 120);

            if (ubicacion.Length > 50)
                ubicacion = Clases.Util.UsoStrings.RecortaString(ubicacion, 50);

            //Determina si es create o update
            if (rel_item_extension_almacen == null) //CREATE
            {

                rel_item_extension_almacen = new SCDM_solicitud_rel_extension_almacenes
                {
                    id_solicitud_rel_item_material = id_rel_item_material,
                    id_solicitud_rel_item_creacion_referencia = id_solicitud_rel_item_creacion_referencia,
                    id_rel_solicitud_extension_usuario = id_rel_extension_usuario,
                    id_cat_almacenes = db.SCDM_cat_almacenes.First(x => x.warehouse == almacen && x.storage_type == almacen_tipo).id,
                    ubicacion = ubicacion,
                    almacen_ejecucion_correcta = ejecucion_correcta,
                    almacen_mensaje_sap = mensaje_sap,
                };
                db.SCDM_solicitud_rel_extension_almacenes.Add(rel_item_extension_almacen);
            }
            else
            { //update 
                rel_item_extension_almacen.ubicacion = ubicacion;
                rel_item_extension_almacen.almacen_mensaje_sap = mensaje_sap;
                rel_item_extension_almacen.almacen_ejecucion_correcta = ejecucion_correcta;
            }

            try
            {
                db.SaveChanges();
                result[0] = new
                {
                    mensaje = "Correcto."

                };
            }
            catch (Exception ex)
            {
                result[0] = new
                {
                    mensaje = "Error: " + ex.Message,
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        ///<summary>
        ///Actualiza o crea los datos de la solictud de materiales
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult SCDM_updateFacturacion(int? idSolicitud, string material, string unidad_medida, string clave_producto, string cliente, string descripcion, string ejecucion_correcta, string mensaje_sap)
        {
            //inicializa la lista de objetos
            var result = new object[1];
            SCDM_solicitud_rel_facturacion rel_item_facturacion = null;

            //busca por item material
            rel_item_facturacion = db.SCDM_solicitud_rel_facturacion.FirstOrDefault(x =>
                                    (x.SCDM_solicitud_rel_item_material.id_solicitud == idSolicitud
                                     || x.SCDM_solicitud_rel_creacion_referencia.id_solicitud == idSolicitud)
                                    && (x.SCDM_solicitud_rel_item_material.numero_material == material
                                    || x.SCDM_solicitud_rel_creacion_referencia.nuevo_material == material)
                                    );



            //obtiene la solicitud
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);

            int? id_rel_item_material = null, id_solicitud_rel_item_creacion_referencia = null;

            //obtiene el id relacionado de creacion de materiales
            var rel_item_material = solicitud.SCDM_solicitud_rel_item_material.FirstOrDefault(x => x.numero_material == material);
            //asigna los valores correspondientes
            if (rel_item_material != null)
                id_rel_item_material = rel_item_material.id;

            //obtiene el idRelacionado de creación con referencia
            var rel_item_creacion_referencia = solicitud.SCDM_solicitud_rel_creacion_referencia.FirstOrDefault(x => x.nuevo_material == material);
            //asigna los valores correspondientes
            if (rel_item_creacion_referencia != null)
                id_solicitud_rel_item_creacion_referencia = rel_item_creacion_referencia.id;


            if (solicitud == null)
            {
                result[0] = new
                {
                    mensaje = "No hay rel item material en BD."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //recorta los strings 
            if (ejecucion_correcta.Length > 120)
                ejecucion_correcta = Clases.Util.UsoStrings.RecortaString(ejecucion_correcta, 120);

            if (mensaje_sap.Length > 120)
                mensaje_sap = Clases.Util.UsoStrings.RecortaString(mensaje_sap, 120);



            //Determina si es create o update
            if (rel_item_facturacion == null) //CREATE
            {

                rel_item_facturacion = new SCDM_solicitud_rel_facturacion
                {
                    id_solicitud_rel_item_material = id_rel_item_material,
                    id_solicitud_rel_item_creacion_referencia = id_solicitud_rel_item_creacion_referencia,
                    unidad_medida = unidad_medida,
                    clave_producto_servicio = clave_producto,
                    cliente = cliente,
                    descripcion_en = descripcion,
                    ejecucion_correcta = ejecucion_correcta,
                    mensaje_sap = mensaje_sap
                };
                db.SCDM_solicitud_rel_facturacion.Add(rel_item_facturacion);
            }
            else
            { //update 
                rel_item_facturacion.unidad_medida = unidad_medida;
                rel_item_facturacion.clave_producto_servicio = clave_producto;
                rel_item_facturacion.cliente = cliente;
                rel_item_facturacion.descripcion_en = descripcion;
                rel_item_facturacion.ejecucion_correcta = ejecucion_correcta;
                rel_item_facturacion.mensaje_sap = mensaje_sap;
            }

            try
            {
                db.SaveChanges();
                result[0] = new
                {
                    mensaje = "Correcto."

                };
            }
            catch (Exception ex)
            {
                result[0] = new
                {
                    mensaje = "Error: " + ex.Message,
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        ///<summary>
        ///Actualiza o crea los datos de la solictud de materiales
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult SCDM_updateEstatusMaterial(int? idSolicitud, string material, string ejecucion_correcta, string mensaje_sap,
            string planta, string sales_org, string estatus_planta, string estatus_dchain, string fecha)
        {
            //inicializa la lista de objetos
            var result = new object[1];
            SCDM_solicitud_rel_activaciones rel_activaciones = null;

            //obtiene la solicitud
            var solicitud = db.SCDM_solicitud.Find(idSolicitud);

            if (solicitud == null)
            {
                result[0] = new
                {
                    mensaje = "No hay rel item material en BD."
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


            //convierte la fecha a DateTime
            DateTime Newfecha = DateTime.Now;

            DateTime.TryParse(fecha, out Newfecha);

            //busca por item material
            rel_activaciones = solicitud.SCDM_solicitud_rel_activaciones.LastOrDefault(x =>
                                        x.material == material
                                        && x.planta == planta
                                        && x.estatus_planta == estatus_planta
                                        && x.estatus_dchain == estatus_dchain
                                        && x.sales_org == sales_org
                                    );



            //recorta los strings 
            if (ejecucion_correcta.Length > 120)
                ejecucion_correcta = Clases.Util.UsoStrings.RecortaString(ejecucion_correcta, 120);

            if (mensaje_sap.Length > 120)
                mensaje_sap = Clases.Util.UsoStrings.RecortaString(mensaje_sap, 120);



            //Determina si es create o update
            if (rel_activaciones == null) //CREATE
            {
                rel_activaciones = new SCDM_solicitud_rel_activaciones
                {
                    id_solicitud = solicitud.id,
                    material = material,
                    planta = planta,
                    estatus_planta = estatus_planta,
                    estatus_dchain = estatus_dchain,
                    sales_org = sales_org,
                    fecha = Newfecha,
                    ejecucion_correcta = ejecucion_correcta,
                    mensaje_sap = mensaje_sap,
                };
                db.SCDM_solicitud_rel_activaciones.Add(rel_activaciones);
            }
            else
            {
                //update 
                rel_activaciones.planta = planta;
                rel_activaciones.estatus_planta = estatus_planta;
                rel_activaciones.estatus_dchain = estatus_dchain;
                rel_activaciones.sales_org = sales_org;
                rel_activaciones.fecha = Newfecha;
                rel_activaciones.ejecucion_correcta = ejecucion_correcta;
                rel_activaciones.mensaje_sap = mensaje_sap;
            }

            try
            {
                db.SaveChanges();
                result[0] = new
                {
                    mensaje = "Correcto."

                };
            }
            catch (Exception ex)
            {
                result[0] = new
                {
                    mensaje = "Error: " + ex.Message,
                };
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Ideas de Mejora
        ///<summary>
        ///Obtiene las areas segun la planta recibida
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneAreasMC(int? clavePlanta)
        {
            //obtiene todos los posibles valores
            List<IM_cat_area> listado = db.IM_cat_area.Where(p => p.id_planta == clavePlanta && p.activo).ToList();

            //inserta el valor por default
            listado.Insert(0, new IM_cat_area
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
                    list[i] = new { value = listado[i].id, name = listado[i].descripcion.ToUpper() };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult DownloadFileIM(int? idDocumento, bool inline = false)
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

            //Valida que el archivo pertenezca a una IM
            if (!archivo.IM_rel_archivos.Any())
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
        #endregion
    }
    public class JsonpResult : JsonResult
    {
        object data = null;

        public JsonpResult()
        {
        }

        public JsonpResult(object data)
        {
            this.data = data;
        }

        public override void ExecuteResult(ControllerContext controllerContext)
        {
            if (controllerContext != null)
            {
                HttpResponseBase Response = controllerContext.HttpContext.Response;
                HttpRequestBase Request = controllerContext.HttpContext.Request;

                string callbackfunction = Request["callback"];

                if (string.IsNullOrEmpty(callbackfunction))
                {
                    throw new Exception("Callback function name must be provided in the request!");
                }
                Response.ContentType = "application/x-javascript";
                if (data != null)
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    Response.Write(string.Format("{0}({1});", callbackfunction, serializer.Serialize(data)));
                }
            }
        }
    }
}