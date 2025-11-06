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
        public static List<SelectListItem> obtieneSupervisoresPlanta(int planta = 0)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            //obtiene todos los posibles valores
            List<produccion_supervisores> listado = db.produccion_supervisores.Where(p => p.activo == true && p.clave_planta == planta).ToList();

            var items = new List<SelectListItem>();

            //usando linQ
            items = listado
                            .Select(s => new SelectListItem()
                            {
                                Text = s.empleados.nombre + " " + s.empleados.apellido1 + " " + s.empleados.apellido2,
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
            List<produccion_operadores> listado = db.produccion_operadores.Where(p => p.activo == true && p.id_linea == linea && p.id_empleado == emp.id).ToList();

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
            Portal_2_0_ServicesEntities db_sap = new Portal_2_0_ServicesEntities();

            List<string> distinctList = db_sap.BomItems
             .Where(p => p.Quantity.HasValue && p.Quantity > 0 && !p.Matnr.StartsWith("sm"))
             .Select(m => m.Matnr) 
             .Distinct()
             .ToList();

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
            items.Add(new SelectListItem()
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
        public static List<SelectListItem> obtieneRollo_BOM(string material = "")
        {
            Portal_2_0_ServicesEntities db_sap = new Portal_2_0_ServicesEntities();
     
            //obtiene todos los posibles valores
            List<string> distinctList = db_sap.BomItems
                .Where(p => p.Quantity.HasValue && p.Quantity > 0 && // Maneja float?
                            !p.Matnr.StartsWith("sm") &&
                            p.Matnr == material)
                .Select(m => m.Component) // Obtiene el Componente (rollo)
                .Distinct()
                .ToList();
     

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

            //obtiene unicamente los usuarios que tienen un empleado asociado
            usuarios = usuarios.Where(x => x.IdEmpleado > 0).ToList();

            //usando linQ
            foreach (var u in usuarios)
            {
                var emp = u.obtieneEmpleado();

                if(emp!=null)
                items.Add(new SelectListItem()
                {
                    Text = u.obtieneEmpleado().ConcatNombre.ToUpper() + " (" + u.Email + ")",
                    Value = u.Id
                });
            }

          

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