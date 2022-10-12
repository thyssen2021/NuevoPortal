using Bitacoras.Util;
using Portal_2_0.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

        public JsonResult GetChartEstatusResponsable(string fecha_inicial, string fecha_final)
        {

            //convierte las fechas recibidas
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = DateTime.Now;          //fecha final por defecto          

            if (!String.IsNullOrEmpty(fecha_inicial))
                dateInicial = Convert.ToDateTime(fecha_inicial);
            if (!String.IsNullOrEmpty(fecha_final))
            {
                dateFinal = Convert.ToDateTime(fecha_final);
                dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            ArrayList titulos = new ArrayList();
            ArrayList valores = new ArrayList();

            //dimension
            titulos.Add("Empleado");
            titulos.Add("Abierto");
            titulos.Add("Asignado");
            titulos.Add("Proceso");
            titulos.Add("Cerradas");

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //AGREGAR FILTRO PARA FECHAS y ESTATUS y Planta
            var listaOT = db.orden_trabajo.Where(x => x.empleados2.planta_clave == empleado.planta_clave
            && x.fecha_solicitud >= dateInicial && x.fecha_solicitud <= dateFinal
            ).ToList();

            //obtiene todos los responsables de las OT
            List<empleados> listEmpleados = listaOT.Where(x => x.id_responsable > 0).Select(x => x.empleados1).Distinct().ToList();

            //inicializa la lista de objetos
            var list = new object[listEmpleados.Count + 1];

            //muetra las ot sin asignar
            int abiertas = listaOT.Where(x => x.estatus == OT_Status.ABIERTO).Count();
            list[0] = new { Empleado = "SIN ASIGNAR", Abierto = abiertas, Asignado = 0, Proceso = 0, Cerradas = 0 };

            for (int i = 0; i < listEmpleados.Count; i++)
            {

                int asignadas = listaOT.Where(x => x.id_responsable == listEmpleados[i].id && x.estatus == OT_Status.ASIGNADO).Count();
                int proceso = listaOT.Where(x => x.id_responsable == listEmpleados[i].id && x.estatus == OT_Status.EN_PROCESO).Count();
                int cerradas = listaOT.Where(x => x.id_responsable == listEmpleados[i].id && x.estatus == OT_Status.CERRADO).Count();

                list[i + 1] = new { Empleado = listEmpleados[i].nombre + " " + listEmpleados[i].apellido1, Abierto = 0, Asignado = asignadas, Proceso = proceso, Cerradas = cerradas };
            }


            var result = new { name = titulos, num = list };//Design JSON format
            return Json(result);//Return JSON data
        }

        public JsonResult GetChartUrgenciaEstatus(string fecha_inicial, string fecha_final)
        {
            ArrayList titulos = new ArrayList();
            ArrayList valores = new ArrayList();

            //convierte las fechas recibidas
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = DateTime.Now;          //fecha final por defecto          

            if (!String.IsNullOrEmpty(fecha_inicial))
                dateInicial = Convert.ToDateTime(fecha_inicial);
            if (!String.IsNullOrEmpty(fecha_final))
            {
                dateFinal = Convert.ToDateTime(fecha_final);
                dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            //dimension
            titulos.Add("Estatus");
            titulos.Add("Abierto");
            titulos.Add("Asignado");
            titulos.Add("Proceso");
            titulos.Add("Cerradas");

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //AGREGAR FILTRO PARA FECHAS y ESTATUS y Planta
            var listaOT = db.orden_trabajo.Where(x => x.empleados2.planta_clave == empleado.planta_clave
            && x.fecha_solicitud >= dateInicial && x.fecha_solicitud <= dateFinal
            ).ToList();

            //obtiene todos los responsables de las OT
            List<string> listUrgencia = listaOT.Select(x => x.nivel_urgencia).Distinct().ToList();

            //inicializa la lista de objetos
            var list = new object[listUrgencia.Count];

            for (int i = 0; i < listUrgencia.Count; i++)
            {
                int abiertas = listaOT.Where(x => x.nivel_urgencia == listUrgencia[i] && x.estatus == OT_Status.ABIERTO).Count();
                int asignadas = listaOT.Where(x => x.nivel_urgencia == listUrgencia[i] && x.estatus == OT_Status.ASIGNADO).Count();
                int proceso = listaOT.Where(x => x.nivel_urgencia == listUrgencia[i] && x.estatus == OT_Status.EN_PROCESO).Count();
                int cerradas = listaOT.Where(x => x.nivel_urgencia == listUrgencia[i] && x.estatus == OT_Status.CERRADO).Count();

                list[i] = new { Estatus = listUrgencia[i], Abierto = abiertas, Asignado = asignadas, Proceso = proceso, Cerradas = cerradas };
            }


            var result = new { name = titulos, num = list };//Design JSON format
            return Json(result);//Return JSON data
        }

        public JsonResult GetChartCantidadPorLinea(string fecha_inicial, string fecha_final)
        {
            ArrayList titulos = new ArrayList();
            ArrayList valores = new ArrayList();

            //convierte las fechas recibidas
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = DateTime.Now;          //fecha final por defecto          

            if (!String.IsNullOrEmpty(fecha_inicial))
                dateInicial = Convert.ToDateTime(fecha_inicial);
            if (!String.IsNullOrEmpty(fecha_final))
            {
                dateFinal = Convert.ToDateTime(fecha_final);
                dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            //dimension
            titulos.Add("Linea");
            titulos.Add("Cantidad");

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //AGREGAR FILTRO PARA FECHAS y ESTATUS y Planta
            var listaOT = db.orden_trabajo.Where(x => x.empleados2.planta_clave == empleado.planta_clave
            && x.fecha_solicitud >= dateInicial && x.fecha_solicitud <= dateFinal
            ).ToList();

            //obtiene todos los responsables de las OT
            List<produccion_lineas> listLineas = listaOT.Where(x => x.id_linea.HasValue == true).Select(x => x.produccion_lineas).Distinct().ToList();

            //inicializa la lista de objetos
            var list = new object[listLineas.Count];

            for (int i = 0; i < listLineas.Count; i++)
            {
                int cantidad = listaOT.Where(x => x.id_linea == listLineas[i].id).Count();

                list[i] = new { Linea = listLineas[i].linea, Cantidad = cantidad };
            }

            var result = new { name = titulos, num = list };//Design JSON format
            return Json(result);//Return JSON data
        }

        public JsonResult GetChartCantidadPorDepartamento(string fecha_inicial, string fecha_final)
        {
            ArrayList titulos = new ArrayList();
            ArrayList valores = new ArrayList();

            //convierte las fechas recibidas
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = DateTime.Now;          //fecha final por defecto          

            if (!String.IsNullOrEmpty(fecha_inicial))
                dateInicial = Convert.ToDateTime(fecha_inicial);
            if (!String.IsNullOrEmpty(fecha_final))
            {
                dateFinal = Convert.ToDateTime(fecha_final);
                dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            //dimension
            titulos.Add("Departamento");
            titulos.Add("Abierto");
            titulos.Add("Asignado");
            titulos.Add("Proceso");
            titulos.Add("Cerradas");

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //AGREGAR FILTRO PARA FECHAS y ESTATUS y Planta
            var listaOT = db.orden_trabajo.Where(x => x.empleados2.planta_clave == empleado.planta_clave
            && x.fecha_solicitud >= dateInicial && x.fecha_solicitud <= dateFinal
            ).ToList();

            //obtiene todos los departamentos
            List<Area> listAreas = listaOT.Where(x => x.id_area > 0).Select(x => x.Area).Distinct().ToList();

            //inicializa la lista de objetos
            var list = new object[listAreas.Count];

            for (int i = 0; i < listAreas.Count; i++)
            {

                int abiertas = listaOT.Where(x => x.id_area == listAreas[i].clave && x.estatus == OT_Status.ABIERTO).Count();
                int asignadas = listaOT.Where(x => x.id_area == listAreas[i].clave && x.estatus == OT_Status.ASIGNADO).Count();
                int proceso = listaOT.Where(x => x.id_area == listAreas[i].clave && x.estatus == OT_Status.EN_PROCESO).Count();
                int cerradas = listaOT.Where(x => x.id_area == listAreas[i].clave && x.estatus == OT_Status.CERRADO).Count();

                int cantidad = listaOT.Where(x => x.id_area == listAreas[i].clave).Count();

                list[i] = new { Departamento = listAreas[i].descripcion, Abierto = abiertas, Asignado = asignadas, Proceso = proceso, Cerradas = cerradas };
            }

            var result = new { name = titulos, num = list };//Design JSON format
            return Json(result);//Return JSON data
        }

        public JsonResult GetChartCantidadPorGrupo(string fecha_inicial, string fecha_final)
        {
            ArrayList titulos = new ArrayList();
            ArrayList valores = new ArrayList();

            //convierte las fechas recibidas
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = DateTime.Now;          //fecha final por defecto          

            if (!String.IsNullOrEmpty(fecha_inicial))
                dateInicial = Convert.ToDateTime(fecha_inicial);
            if (!String.IsNullOrEmpty(fecha_final))
            {
                dateFinal = Convert.ToDateTime(fecha_final);
                dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            //dimension
            titulos.Add("Grupo");
            titulos.Add("Abierto");
            titulos.Add("Asignado");
            titulos.Add("Proceso");
            titulos.Add("Cerradas");

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //AGREGAR FILTRO PARA FECHAS y ESTATUS y Planta
            var listaOT = db.orden_trabajo.Where(x => x.empleados2.planta_clave == empleado.planta_clave
            && x.fecha_solicitud >= dateInicial && x.fecha_solicitud <= dateFinal
            ).ToList();

            //obtiene todos los grupos
            List<OT_grupo_trabajo> listGrupoTrabajo = listaOT.Where(x => x.id_grupo_trabajo > 0).Select(x => x.OT_grupo_trabajo).Distinct().ToList();

            //inicializa la lista de objetos
            var list = new object[listGrupoTrabajo.Count];

            for (int i = 0; i < listGrupoTrabajo.Count; i++)
            {
                int abiertas = listaOT.Where(x => x.id_grupo_trabajo == listGrupoTrabajo[i].id && x.estatus == OT_Status.ABIERTO).Count();
                int asignadas = listaOT.Where(x => x.id_grupo_trabajo == listGrupoTrabajo[i].id && x.estatus == OT_Status.ASIGNADO).Count();
                int proceso = listaOT.Where(x => x.id_grupo_trabajo == listGrupoTrabajo[i].id && x.estatus == OT_Status.EN_PROCESO).Count();
                int cerradas = listaOT.Where(x => x.id_grupo_trabajo == listGrupoTrabajo[i].id && x.estatus == OT_Status.CERRADO).Count();

                int cantidad = listaOT.Where(x => x.id_grupo_trabajo == listGrupoTrabajo[i].id).Count();

                list[i] = new { Grupo = listGrupoTrabajo[i].descripcion, Abierto = abiertas, Asignado = asignadas, Proceso = proceso, Cerradas = cerradas };
            }

            var result = new { name = titulos, num = list };//Design JSON format
            return Json(result);//Return JSON data
        }

        public JsonResult GetChartDefectoPorDepartamento(string fecha_inicial, string fecha_final)
        {
            ArrayList titulos = new ArrayList();
            ArrayList valores = new ArrayList();

            //convierte las fechas recibidas
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = DateTime.Now;          //fecha final por defecto          

            if (!String.IsNullOrEmpty(fecha_inicial))
                dateInicial = Convert.ToDateTime(fecha_inicial);
            if (!String.IsNullOrEmpty(fecha_final))
            {
                dateFinal = Convert.ToDateTime(fecha_final);
                dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //AGREGAR FILTRO PARA FECHAS y ESTATUS y Planta
            var listaOT = db.orden_trabajo.Where(x => x.empleados2.planta_clave == empleado.planta_clave
            && x.fecha_solicitud >= dateInicial && x.fecha_solicitud <= dateFinal
            ).ToList();

            //obtiene todos los departamentos
            List<Area> listAreas = listaOT.Where(x => x.id_area > 0).Select(x => x.Area).Distinct().ToList();

            //inicializa la lista de objetos
            var list = new object[listAreas.Count];

            for (int i = 0; i < listAreas.Count; i++)
            {
                int cantidad = listaOT.Where(x => x.id_area == listAreas[i].clave && x.tpm == true).Count();

                list[i] = new { Departamento = listAreas[i].descripcion, Cantidad = cantidad };
            }

            var result = new { num = list };//Design JSON format
            return Json(result);//Return JSON data
        }

        public JsonResult GetChartDetectados(string fecha_inicial, string fecha_final)
        {
            ArrayList titulos = new ArrayList();
            ArrayList valores = new ArrayList();

            //convierte las fechas recibidas
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = DateTime.Now;          //fecha final por defecto          

            if (!String.IsNullOrEmpty(fecha_inicial))
                dateInicial = Convert.ToDateTime(fecha_inicial);
            if (!String.IsNullOrEmpty(fecha_final))
            {
                dateFinal = Convert.ToDateTime(fecha_final);
                dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            //dimension
            titulos.Add("Empleado");
            titulos.Add("Cantidad");

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //AGREGAR FILTRO PARA FECHAS y ESTATUS y Planta
            var listaOT = db.orden_trabajo.Where(x => x.empleados2.planta_clave == empleado.planta_clave
            && x.fecha_solicitud >= dateInicial && x.fecha_solicitud <= dateFinal
            ).ToList();

            //obtiene todos los solicitante de las OT
            List<empleados> listSolicitantes = listaOT.Where(x => x.id_solicitante > 0).Select(x => x.empleados2).Distinct().ToList();

            //inicializa la lista de objetos
            var list = new object[listSolicitantes.Count];

            for (int i = 0; i < listSolicitantes.Count; i++)
            {
                //obtine la suma de solicitudes creadas donde hay tpm
                int cantidad = listaOT.Where(x => x.id_solicitante == listSolicitantes[i].id && x.tpm == true).Count();

                list[i] = new { Empleado = listSolicitantes[i].nombre + " " + listSolicitantes[i].apellido1, Cantidad = cantidad };
            }

            var result = new { name = titulos, num = list };//Design JSON format
            return Json(result);//Return JSON data
        }
        public JsonResult GetChartCorregidos(string fecha_inicial, string fecha_final)
        {
            ArrayList titulos = new ArrayList();
            ArrayList valores = new ArrayList();

            //convierte las fechas recibidas
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = DateTime.Now;          //fecha final por defecto          

            if (!String.IsNullOrEmpty(fecha_inicial))
                dateInicial = Convert.ToDateTime(fecha_inicial);
            if (!String.IsNullOrEmpty(fecha_final))
            {
                dateFinal = Convert.ToDateTime(fecha_final);
                dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            //dimension
            titulos.Add("Empleado");
            titulos.Add("Cantidad");

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //AGREGAR FILTRO PARA FECHAS y ESTATUS y Planta
            var listaOT = db.orden_trabajo.Where(x => x.empleados2.planta_clave == empleado.planta_clave
            && x.fecha_solicitud >= dateInicial && x.fecha_solicitud <= dateFinal
            ).ToList();

            //obtiene todos los responsables de las OT
            List<empleados> listResponsables = listaOT.Where(x => x.id_responsable > 0).Select(x => x.empleados1).Distinct().ToList();

            //inicializa la lista de objetos
            var list = new object[listResponsables.Count];

            for (int i = 0; i < listResponsables.Count; i++)
            {
                //obtine la suma de solicitudes creadas donde hay tpm
                int cantidad = listaOT.Where(x => x.id_responsable == listResponsables[i].id && x.tpm == true && x.estatus == OT_Status.CERRADO).Count();

                list[i] = new { Empleado = listResponsables[i].nombre + " " + listResponsables[i].apellido1, Cantidad = cantidad };
            }

            var result = new { name = titulos, num = list };//Design JSON format
            return Json(result);//Return JSON data
        }

        public JsonResult GetChartDetectadoVsCorregido(string fecha_inicial, string fecha_final)
        {
            ArrayList titulos = new ArrayList();
            ArrayList valores = new ArrayList();

            //convierte las fechas recibidas
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = DateTime.Now;          //fecha final por defecto          

            if (!String.IsNullOrEmpty(fecha_inicial))
                dateInicial = Convert.ToDateTime(fecha_inicial);
            if (!String.IsNullOrEmpty(fecha_final))
            {
                dateFinal = Convert.ToDateTime(fecha_final);
                dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            //dimension
            titulos.Add("Departamento");
            titulos.Add("Detectados");
            titulos.Add("Corregidos");

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //AGREGAR FILTRO PARA FECHAS y ESTATUS y Planta
            var listaOT = db.orden_trabajo.Where(x => x.empleados2.planta_clave == empleado.planta_clave
            && x.fecha_solicitud >= dateInicial && x.fecha_solicitud <= dateFinal
            ).ToList();

            //obtiene todos los departamentos
            List<Area> listAreas = listaOT.Where(x => x.id_area > 0).Select(x => x.Area).Distinct().ToList();

            //inicializa la lista de objetos
            var list = new object[listAreas.Count];

            for (int i = 0; i < listAreas.Count; i++)
            {
                int detectadas = listaOT.Where(x => x.id_area == listAreas[i].clave && x.tpm == true).Count();
                int corregidas = listaOT.Where(x => x.id_area == listAreas[i].clave && x.tpm == true && x.estatus == OT_Status.CERRADO).Count();

                list[i] = new { Departamento = listAreas[i].descripcion, Detectados = detectadas, Corregidos = corregidas };
            }

            var result = new { name = titulos, num = list };//Design JSON format
            return Json(result);//Return JSON data
        }

        public JsonResult GetChartScoreMensual(string fecha_inicial, string fecha_final)
        {
            ArrayList titulos = new ArrayList();
            ArrayList valores = new ArrayList();

            //convierte las fechas recibidas
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = DateTime.Now;          //fecha final por defecto          

            if (!String.IsNullOrEmpty(fecha_inicial))
                dateInicial = Convert.ToDateTime(fecha_inicial);
            if (!String.IsNullOrEmpty(fecha_final))
            {
                dateFinal = Convert.ToDateTime(fecha_final);
                //obtiene el último día del mes de la fecha final
                dateFinal = new DateTime(dateFinal.Year, dateFinal.Month, 1).AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            //dimension
            titulos.Add("Mes");
            titulos.Add("Cantidad");

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();

            //AGREGAR FILTRO PARA FECHAS y ESTATUS y Planta
            var listaOT = db.orden_trabajo.Where(x => x.empleados2.planta_clave == empleado.planta_clave
            && x.fecha_solicitud >= dateInicial && x.fecha_solicitud <= dateFinal
            ).ToList();

            //obtiene todos los responsables de las OT
            List<DateTime> listMeses = new List<DateTime>();


            DateTime mesInicial = new DateTime(dateInicial.Year, dateInicial.Month, 1);
            DateTime mesFinal = new DateTime(dateFinal.Year, dateFinal.Month, 1);

            listMeses.Add(mesInicial);

            while (mesInicial < mesFinal)
            {
                mesInicial = mesInicial.AddMonths(1);
                listMeses.Add(mesInicial);
            }

            //inicializa la lista de objetos
            var list = new object[listMeses.Count];

            for (int i = 0; i < listMeses.Count; i++)
            {
                DateTime finMes = listMeses[i].AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                //obtine la suma de solicitudes creadas donde hay tpm
                int cantidad = listaOT.Where(x => x.tpm == true && x.estatus == OT_Status.CERRADO && x.fecha_solicitud >= listMeses[i] && x.fecha_solicitud <= finMes).Count();

                list[i] = new { Mes = MesesUtil.getMes(listMeses[i].Month).Abreviatura + " " + listMeses[i].Year, Cantidad = cantidad };
            }

            var result = new { name = titulos, num = list };//Design JSON format
            return Json(result);//Return JSON data
        }


        /// <summary>
        /// Obtiene el total por región
        /// </summary>
        public ContentResult GetChartRegiones()
        {

            //obtiene todos los datos relacionados
            var datos = db.BG_IHS_item;

            //obtiene la lista de regiones
            List<String> listRegiones = db.BG_IHS_regiones.Select(x => x.descripcion).Distinct().ToList();

            //obtiene los años 
            var cabeceraAnios = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAnios();

            //obtiene los datos
            List<BG_IHS_rel_demanda> listDatos = new List<BG_IHS_rel_demanda>();
            var cabeceraDemanda = Portal_2_0.Models.BG_IHS_UTIL.GetCabecera();

            foreach (var ihs in db.BG_IHS_item)
            {
                listDatos.AddRange(ihs.GetDemanda(cabeceraDemanda, "CUSTOMER"));
            }
            string jsonString = @"{""name"":[""Region""";
            foreach (var r in listRegiones)
            {
                jsonString += ",\"" + r + "\"";
            }

            jsonString += @"],
                 ""num"":[
            ";

            for (int i = 0; i < cabeceraAnios.Count; i++)
            {
                DateTime fechaInicial = new DateTime(cabeceraAnios[i].anio, 1, 1);
                DateTime fechaFinal = fechaInicial.AddYears(1).AddDays(-1);

                if (i != 0)
                    jsonString += ", ";

                jsonString += "{ \"Region\":" + "\"FY " + cabeceraAnios[i].anio + "\",";

                for (int j = 0; j < listRegiones.Count; j++) //region
                {
                    int? cantidad = listDatos.Where(x => x != null && x.fecha >= fechaInicial && x.fecha <= fechaFinal
                                                 && x.BG_IHS_item.Region != null && x.BG_IHS_item.Region.descripcion == listRegiones[j]
                                                 ).Sum(x => x.cantidad);

                    if (j != 0)
                        jsonString += ", ";
                    //Se obtiene el valor de c
                    jsonString += "\"" + listRegiones[j] + "\":";
                    jsonString += cantidad != null ? cantidad.Value : 0;
                }

                jsonString += "}";

            }

            jsonString += @"]
                }";

            System.Diagnostics.Debug.Print(jsonString);

            return new ContentResult { Content = jsonString, ContentType = "application/json" };
        }
        /// <summary>
        /// Obtiene el total por región
        /// </summary>
        public ContentResult GetChartRegion(String region)
        {

            //obtiene todos los datos relacionados
            var datos = db.BG_IHS_item;

            //obtiene los años 
            var cabeceraAnios = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAnios();

            //obtiene los datos
            List<BG_IHS_rel_demanda> listDatos = new List<BG_IHS_rel_demanda>();
            var cabeceraDemanda = Portal_2_0.Models.BG_IHS_UTIL.GetCabecera();

            foreach (var ihs in db.BG_IHS_item)
            {
                listDatos.AddRange(ihs.GetDemanda(cabeceraDemanda, "CUSTOMER"));
            }
            string jsonString = @"{""name"":[""Region""";

            jsonString += ",\"" + region + "\"";

            jsonString += @"],
                 ""num"":[
            ";

            for (int i = 0; i < cabeceraAnios.Count; i++)
            {
                DateTime fechaInicial = new DateTime(cabeceraAnios[i].anio, 1, 1);
                DateTime fechaFinal = fechaInicial.AddYears(1).AddDays(-1);

                if (i != 0)
                    jsonString += ", ";

                jsonString += "{ \"Region\":" + "\"FY " + cabeceraAnios[i].anio + "\",";

              
                    int? cantidad = listDatos.Where(x => x != null && x.fecha >= fechaInicial && x.fecha <= fechaFinal
                                                 && x.BG_IHS_item.Region != null && x.BG_IHS_item.Region.descripcion == region
                                                 ).Sum(x => x.cantidad);
                    
                    //Se obtiene el valor de c
                    jsonString += "\"" + region + "\":";
                    jsonString += cantidad != null ? cantidad.Value : 0;
               
                jsonString += "}";

            }

            jsonString += @"]
                }";

            System.Diagnostics.Debug.Print(jsonString);

            return new ContentResult { Content = jsonString, ContentType = "application/json" };
        }
    }

}