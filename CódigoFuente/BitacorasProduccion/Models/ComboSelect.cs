using Bitacoras.DBUtil;
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
        ///retorna un List<SelectListItem> con las opciones disponibles
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
                                Value = s.id.ToString()
                            }).ToList();

            //agrega valor vacio
            items.Insert(0, new SelectListItem()
            {
                Text = "Seleccione un valor",
                Value = ""
            });

            return items;
        }

        ///<summary>
        ///Obtiene las plantas
        ///</summary>
        ///<return>
        ///retorna un List<SelectListItem> con las opciones disponibles
        public static List<SelectListItem> obtienePlantasSelectList()
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            //obtiene todos los posibles valores
            List<plantas> listado = db.plantas.Where(p => p.activo == true).ToList();

            var items = new List<SelectListItem>();

            //usando linQ
            items = listado
                            .Where(s => s.activo == true) //obtiene unicamente los sectores activos
                            .Select(s => new SelectListItem()
                            {
                                Text = s.descripcion,
                                Value = s.clave.ToString()
                            }).ToList();

            //agrega valor vacio
            items.Insert(0, new SelectListItem()
            {
                Text = "-- Seleccione un valor --",
                Value = ""
            });

            return items;
        }

        ///<summary>
        ///Obtiene listado de clientes de Silao
        ///</summary>
        ///<return>
        ///retorna un List<SelectListItem> con las opciones disponibles
        public static List<SelectListItem> obtieneClientesSilao()
        {

            //obtiene todos los posibles valores
            List<String> listado = ReportesPesadasDBUtil.ObtieneClientesSilao();

            var items = new List<SelectListItem>();

            //usando linQ
            items = listado
                            
                            .Select(s => new SelectListItem()
                            {
                                Text = s,
                                Value = s
                            }).ToList();

            //agrega valor vacio
            items.Insert(0, new SelectListItem()
            {
                Text = "-- Seleccione un valor --",
                Value = ""
            });

            return items;
        }

        ///<summary>
        ///Obtiene listado de clientes de Silao
        ///</summary>
        ///<return>
        ///retorna un List<SelectListItem> con las opciones disponibles
        public static List<SelectListItem> obtieneClientesPuebla()
        {            

            //obtiene todos los posibles valores
            List<String> listado = ReportesPesadasDBUtil.ObtieneClientesPuebla();

            var items = new List<SelectListItem>();

            //usando linQ
            items = listado

                            .Select(s => new SelectListItem()
                            {
                                Text = s,
                                Value = s
                            }).ToList();

            //agrega valor vacio
            items.Insert(0, new SelectListItem()
            {
                Text = "-- Seleccione un valor --",
                Value = ""
            });

            return items;
        }

        ///<summary>
        ///Obtiene el listado de supervisores por planta
        ///</summary>
        ///<return>
        ///retorna un List<SelectListItem> con las opciones disponibles
        public static List<SelectListItem> obtieneSupervisoresPlanta(int planta=0)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            //obtiene todos los posibles valores
            List<produccion_supervisores> listado = db.produccion_supervisores.Where(p => p.activo == true && p.clave_planta==planta).ToList();

            var items = new List<SelectListItem>();

            //usando linQ
            items = listado
                            .Select(s => new SelectListItem()
                            {
                                Text = s.empleados.nombre +" "+ s.empleados.apellido1 +" "+s.empleados.apellido2,
                                Value = s.id.ToString()
                            }).ToList();

            //agrega valor vacio
            items.Insert(0, new SelectListItem()
            {
                Text = "-- Seleccione un valor --",
                Value = ""
            });

            return items;
        }

        ///<summary>
        ///Obtiene operadores por linea de producción 
        ///</summary>
        ///<return>
        ///retorna un List<SelectListItem> con las opciones disponibles
        public static List<SelectListItem> obtieneOperadorPorLinea(empleados emp, int linea = 0)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            //obtiene todos los posibles valores
            List<produccion_operadores> listado = db.produccion_operadores.Where(p => p.activo == true && p.id_linea == linea && p.id_empleado==emp.id).ToList();

            var items = new List<SelectListItem>();

            //usando linQ
            items = listado
                            .Select(s => new SelectListItem()
                            {
                                Text = s.empleados.nombre + " " + s.empleados.apellido1 + " " + s.empleados.apellido2,
                                Value = s.id.ToString()
                            }).ToList();

            //agrega valor vacio
            //items.Insert(0, new SelectListItem()
            //{
            //    Text = "-- Seleccione un valor --",
            //    Value = ""
            //});

            return items;
        }

        ///<summary>
        ///Obtiene material de boom 
        ///</summary>
        ///<return>
        ///retorna un List<SelectListItem> con las opciones disponibles
        public static List<SelectListItem> obtieneMaterial_BOM()
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            //obtiene todos los posibles valores
            List<bom_en_sap> listado = db.bom_en_sap.Where(p => p.activo == true && p.Quantity>0 && !p.Material.StartsWith("sm")).ToList();
 
            //realiza un distict de los materiales
            List<string> distinctList = listado.Select(m => m.Material).Distinct().ToList();
            
            var items = new List<SelectListItem>();

            //usando linQ
            items = distinctList
                            .Select(s => new SelectListItem()
                            {
                                Text = s,
                                Value = s
                            }).ToList();

            ////agrega valor vacio
            //items.Insert(0, new SelectListItem()
            //{
            //    Text = "-- Seleccione un valor --",
            //    Value = ""
            //});

            //agrega valores al final 
            items.Add( new SelectListItem()
            {
                Text = "TEMPORAL",
                Value = "TEMPORAL"
            });

            return items;
        }

        ///<summary>
        ///Obtiene el componente (rollo) de boom 
        ///</summary>
        ///<return>
        ///retorna un List<SelectListItem> con las opciones disponibles
        public static List<SelectListItem> obtieneRollo_BOM(string material="")
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            //obtiene todos los posibles valores
            List<bom_en_sap> listado = db.bom_en_sap.Where(p => p.activo == true && p.Quantity > 0 && !p.Material.StartsWith("sm") && p.Material == material).ToList();

            //realiza un distict de los materiales
            List<string> distinctList = listado.Select(m => m.Component).Distinct().ToList();

            var items = new List<SelectListItem>();

            //usando linQ
            items = distinctList
                            .Select(s => new SelectListItem()
                            {
                                Text = s,
                                Value = s
                            }).ToList();

            //agrega valor vacio
            items.Insert(0, new SelectListItem()
            {
                Text = "-- Seleccione un valor --",
                Value = ""
            });

            return items;
        }

        ///<summary>
        ///Obtiene combo de usuarios
        ///</summary>
        ///<return>
        ///retorna un List<SelectListItem> con las opciones disponibles
        public static List<SelectListItem> obtieneUsuarios(List<IdentitySample.Models.ApplicationUser> usuarios)
        {
            
            var items = new List<SelectListItem>();

            //usando linQ
            items = usuarios.Select(s => new SelectListItem()
                            {
                                Text = s.obtieneEmpleado().ConcatNombre.ToUpper() + " ("+s.Email+")",
                                Value = s.Id
                            }).ToList();

            //agrega valor vacio
            items.Insert(0, new SelectListItem()
            {
                Text = "-- Seleccione un valor --",
                Value = ""
            });

            return items;
        }
    }
}