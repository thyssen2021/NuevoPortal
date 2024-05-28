﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class ProduccionResultadoController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();


        // GET: ProduccionReportes
        public ActionResult Index(int? clave_planta, int? id_linea, string platina, string numero_parte, string fecha_inicial, string fecha_final, string tipo_reporte, string fecha_turno, int? turno, int pagina = 1)
        {

            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                empleados emp = obtieneEmpleadoLogeado();

                ////muestra error en caso de querer solicitar una planta distinta
                if (!TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE_ALL_ACCESS) && clave_planta != null && clave_planta != emp.planta_clave)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede acceder a la información solicitada!";
                    ViewBag.Descripcion = "No puede consultar la información de la planta solicitada.";

                    return View("../Home/ErrorGenerico");
                }


                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
                DateTime dateFinal = DateTime.Now;          //fecha final por defecto
                DateTime dateTurno = DateTime.Now;          //fecha turno por defecto

                try
                {
                    if (!String.IsNullOrEmpty(fecha_inicial))
                        dateInicial = Convert.ToDateTime(fecha_inicial);
                    if (!String.IsNullOrEmpty(fecha_final))
                    {
                        dateFinal = Convert.ToDateTime(fecha_final);
                        dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
                    if (!String.IsNullOrEmpty(fecha_turno))
                        dateTurno = Convert.ToDateTime(fecha_turno);
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Error de Formato: " + e.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al convertir: " + ex.Message);
                }

                //valor por defecto
                int clave = 0;

                if (clave_planta != null)
                {
                    clave = clave_planta.Value;
                }
                plantas planta = db.plantas.Find(clave);
                if (planta == null)
                {
                    planta = new plantas
                    {
                        clave = 0
                    };
                }


                //valor por defecto linea
                int linea = 0;

                if (id_linea != null)
                {
                    linea = id_linea.Value;
                }

                produccion_lineas produccion_Lineas = db.produccion_lineas.Find(linea);
                if (produccion_Lineas == null)
                {
                    produccion_Lineas = new produccion_lineas
                    {
                        id = 0,
                    };
                }

                //valor por defecto para turno
                int id_turno = 0;

                if (turno != null)
                    id_turno = turno.Value;

                produccion_turnos turno1 = db.produccion_turnos.Find(id_turno);

                //si no hay turno lo inicializa
                if (turno1 == null)
                    turno1 = new produccion_turnos { id = 0 };


                var cantidadRegistrosPorPagina = 20; // parámetro

                //REALIZA LA CONSULTA DEPENDIENDO DEL TIPO
                String tipoR = "sabana";  //valor por defecto 
                if (!String.IsNullOrEmpty(tipo_reporte))
                {
                    tipoR = tipo_reporte;
                }

                List<view_historico_resultado> listado = new List<view_historico_resultado>();
                int totalDeRegistros = 0;

                if (clave_planta == null)
                {
                    listado = new List<view_historico_resultado> { };
                    totalDeRegistros = 0;
                }
                else
                {
                    //aumenta el tiempo de espera
                    db.Database.CommandTimeout = 240;
                    if (tipoR.Contains("sabana"))
                    {             //BUSCA POR SÁBANA
                        var listadoBD = db.view_historico_resultado.Where(
                            x =>
                            x.Planta.ToUpper().Contains(planta.descripcion.ToUpper())
                            && (x.Linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                            && x.Fecha >= dateInicial && x.Fecha <= dateFinal
                            && (x.Número_de_Parte__de_cliente == numero_parte || x.Número_de_Parte_de_Cliente_platina2 == numero_parte || String.IsNullOrEmpty(numero_parte))
                            && (x.SAP_Platina == platina || x.SAP_Platina_2 == platina || String.IsNullOrEmpty(platina))
                            // && !x.SAP_Platina.ToUpper().Contains("TEMPORAL")
                            // && !x.SAP_Rollo.ToUpper().Contains("TEMPORAL")
                            );

                        listado = listadoBD
                            .OrderBy(x => x.id)
                            .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                            .Take(cantidadRegistrosPorPagina).ToList();

                        totalDeRegistros = listadoBD.Count();
                    }
                    else if (tipoR.Contains("turno"))
                    { //BUSCA POR TURNO
                      //determina la hora inicial y final del turno                   
                        DateTime fecha_fin_turno = dateTurno.Add(turno1.hora_fin);
                        //hora inicial
                        dateTurno = dateTurno.Add(turno1.hora_inicio);

                        //si la hora fin es menor a la hora inicio es otro dia
                        if (TimeSpan.Compare(turno1.hora_inicio, turno1.hora_fin) == 1)
                            fecha_fin_turno = fecha_fin_turno.AddDays(1);

                        var listadoBD = db.view_historico_resultado.Where(
                            x =>
                             x.Planta.ToUpper().Contains(planta.descripcion.ToUpper())
                           && (x.Linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                           && (x.Turno.ToUpper().Contains(turno1.descripcion.ToUpper()) || x.Turno.ToUpper().Contains(turno1.valor.ToString()) || id_turno == 0)
                           && x.Fecha >= dateTurno && x.Fecha <= fecha_fin_turno
                             && (x.Número_de_Parte__de_cliente == numero_parte || x.Número_de_Parte_de_Cliente_platina2 == numero_parte || String.IsNullOrEmpty(numero_parte))
                            && (x.SAP_Platina == platina || x.SAP_Platina_2 == platina || String.IsNullOrEmpty(platina))

                            // && !x.SAP_Platina.ToUpper().Contains("TEMPORAL")
                            // && !x.SAP_Rollo.ToUpper().Contains("TEMPORAL")
                            );


                        listado = listadoBD
                           .OrderBy(x => x.id)
                           .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                           .Take(cantidadRegistrosPorPagina).ToList();

                        totalDeRegistros = listadoBD.Count();

                    }
                }

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["clave_planta"] = clave_planta;
                routeValues["id_linea"] = id_linea;
                routeValues["platina"] = platina;
                routeValues["numero_parte"] = numero_parte;
                routeValues["fecha_inicial"] = fecha_inicial;
                routeValues["fecha_final"] = fecha_final;
                routeValues["tipo_reporte"] = tipo_reporte;
                routeValues["fecha_turno"] = fecha_turno;
                routeValues["turno"] = turno;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                //crea el select List para todos los numeros de parte
                List<string> listaNumeroParte = new List<string>();
                List<string> listaNumeroPartePlatina2 = new List<string>();
                //listas para num platina
                List<string> listaPlatina1 = new List<string>();
                List<string> listaPlatina2 = new List<string>();


                if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE_ALL_ACCESS))
                {
                    listaNumeroParte = db.view_historico_resultado.Select(x => x.Número_de_Parte__de_cliente).Distinct().ToList();
                    listaPlatina1 = db.view_historico_resultado.Select(x => x.SAP_Platina).Distinct().ToList();
                }
                else
                {
                    listaNumeroParte = db.view_historico_resultado.Where(x => x.Planta.ToUpper().Contains(emp.plantas.descripcion.ToUpper())).Select(x => x.Número_de_Parte__de_cliente).Distinct().ToList();
                    listaPlatina1 = db.view_historico_resultado.Where(x => x.Planta.ToUpper().Contains(emp.plantas.descripcion.ToUpper())).Select(x => x.SAP_Platina).Distinct().ToList();
                }
                if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE_ALL_ACCESS))
                {
                    listaNumeroPartePlatina2 = db.view_historico_resultado.Select(x => x.Número_de_Parte_de_Cliente_platina2).Distinct().ToList();
                    listaPlatina2 = db.view_historico_resultado.Select(x => x.SAP_Platina_2).Distinct().ToList();
                }
                else
                {
                    listaNumeroPartePlatina2 = db.view_historico_resultado.Where(x => x.Planta.ToUpper().Contains(emp.plantas.descripcion.ToUpper())).Select(x => x.Número_de_Parte_de_Cliente_platina2).Distinct().ToList();
                    listaPlatina2 = db.view_historico_resultado.Where(x => x.Planta.ToUpper().Contains(emp.plantas.descripcion.ToUpper())).Select(x => x.SAP_Platina_2).Distinct().ToList();
                }
                //une con numero de parte de platina 2

                //Une ambas listas
                listaNumeroParte.AddRange(listaNumeroPartePlatina2);
                //quita elementos vacios
                listaNumeroParte.RemoveAll(x => string.IsNullOrEmpty(x));
                //elimina duplicados
                listaNumeroParte = listaNumeroParte.Distinct().ToList();

                //une ambas listas y elimina duplicados
                listaPlatina1.AddRange(listaPlatina2);
                //quita elementos vacios
                listaPlatina1.RemoveAll(x => string.IsNullOrEmpty(x));
                //elimina duplicados
                listaPlatina1 = listaPlatina1.Distinct().ToList();



                //creamos una lista tipo SelectListItem
                List<SelectListItem> lst = new List<SelectListItem>();

                foreach (var item in listaNumeroParte)
                    lst.Add(new SelectListItem() { Text = item, Value = item });

                //Agregamos la lista a nuestro SelectList
                SelectList listNumParte = new SelectList(lst, "Value", "Text");

               

                List<SelectListItem> lstP = new List<SelectListItem>();
                foreach (var item in listaPlatina1)
                    lstP.Add(new SelectListItem() { Text = item, Value = item });

                SelectList listPlatina = new SelectList(lstP, "Value", "Text");


                ViewBag.id_linea = new SelectList(db.produccion_lineas.Where(p => p.activo == true), "id", "linea");
                ViewBag.turno = new SelectList(db.produccion_turnos.Where(p => p.activo.HasValue && p.activo.Value && p.clave_planta == emp.planta_clave), "id", "descripcion");
                ViewBag.numero_parte = listNumParte;
                ViewBag.platina = listPlatina;

                if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE_ALL_ACCESS))
                    ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                else
                    ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true && p.clave == emp.planta_clave), "clave", "descripcion");
                ViewBag.Paginacion = paginacion;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }


        public ActionResult Exportar(int? clave_planta, int? id_linea, string platina, string numero_parte, string fecha_inicial, string fecha_final, string tipo_reporte, string fecha_turno, int? turno, int pagina = 1)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE))
            {
                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
                DateTime dateFinal = DateTime.Now;          //fecha final por defecto
                DateTime dateTurno = DateTime.Now;          //fecha turno por defecto

                try
                {
                    if (!String.IsNullOrEmpty(fecha_inicial))
                        dateInicial = Convert.ToDateTime(fecha_inicial);
                    if (!String.IsNullOrEmpty(fecha_final))
                    {
                        dateFinal = Convert.ToDateTime(fecha_final);
                        dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
                    if (!String.IsNullOrEmpty(fecha_turno))
                        dateTurno = Convert.ToDateTime(fecha_turno);
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Error de Formato: " + e.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al convertir: " + ex.Message);
                }

                //valor por defecto
                int clave = 0;

                if (clave_planta != null)
                {
                    clave = clave_planta.Value;
                }
                plantas planta = db.plantas.Find(clave);
                if (planta == null)
                {
                    planta = new plantas
                    {
                        clave = 0
                    };
                }


                //valor por defecto linea
                int linea = 0;

                if (id_linea != null)
                {
                    linea = id_linea.Value;
                }
                produccion_lineas produccion_Lineas = db.produccion_lineas.Find(linea);
                if (produccion_Lineas == null)
                {
                    produccion_Lineas = new produccion_lineas
                    {
                        id = 0,
                    };
                }

                //valor por defecto para turno
                int id_turno = 0;

                if (turno != null)
                    id_turno = turno.Value;

                produccion_turnos turno1 = db.produccion_turnos.Find(id_turno);

                //si no hay turno lo inicializa
                if (turno1 == null)
                    turno1 = new produccion_turnos { id = 0 };


                //REALIZA LA CONSULTA DEPENDIENDO DEL TIPO
                String tipoR = "sabana";  //valor por defecto 
                if (!String.IsNullOrEmpty(tipo_reporte))
                {
                    tipoR = tipo_reporte;
                }

                List<view_historico_resultado> listado = new List<view_historico_resultado>();

                bool porturno = false;

                if (tipoR.Contains("sabana"))
                {             //BUSCA POR SÁBANA
                    listado = db.view_historico_resultado.Where(
                        x =>
                        x.Planta.ToUpper().Contains(planta.descripcion.ToUpper())
                        && (x.Linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                        && x.Fecha >= dateInicial && x.Fecha <= dateFinal
                       && (x.Número_de_Parte__de_cliente == numero_parte || x.Número_de_Parte_de_Cliente_platina2 == numero_parte || String.IsNullOrEmpty(numero_parte))
                       && (x.SAP_Platina == platina || x.SAP_Platina_2 == platina || String.IsNullOrEmpty(platina))

                        //&& !x.SAP_Rollo.ToUpper().Contains("TEMPORAL")
                        )
                        .OrderBy(x => x.id)
                       .ToList();


                }
                else if (tipoR.Contains("turno"))
                { //BUSCA POR TURNO
                    porturno = true;
                    //determina la hora inicial y final del turno                   
                    DateTime fecha_fin_turno = dateTurno.Add(turno1.hora_fin);
                    //hora inicial
                    dateTurno = dateTurno.Add(turno1.hora_inicio);

                    //si la hora fin es menor a la hora inicio es otro dia
                    if (TimeSpan.Compare(turno1.hora_inicio, turno1.hora_fin) == 1)
                        fecha_fin_turno = fecha_fin_turno.AddDays(1);


                    listado = db.view_historico_resultado.Where(
                       x =>
                       //ver comparar por el campo hora=?
                       x.Planta.ToUpper().Contains(planta.descripcion.ToUpper())
                       && (x.Linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                       && (x.Turno.ToUpper().Contains(turno1.descripcion.ToUpper()) || x.Turno.ToUpper().Contains(turno1.valor.ToString()) || id_turno == 0)
                       && x.Fecha >= dateTurno && x.Fecha <= fecha_fin_turno
                       && (x.Número_de_Parte__de_cliente == numero_parte || x.Número_de_Parte_de_Cliente_platina2 == numero_parte || String.IsNullOrEmpty(numero_parte))
                       && (x.SAP_Platina == platina || x.SAP_Platina_2 == platina || String.IsNullOrEmpty(platina))

                       //  && !x.SAP_Platina.ToUpper().Contains("TEMPORAL")
                       //  && !x.SAP_Rollo.ToUpper().Contains("TEMPORAL")
                       )
                       .OrderBy(x => x.id)
                       .ToList();
                }

                byte[] stream = ExcelUtil.GeneraReporteBitacorasExcel(listado, planta, porturno);

                //obtiene la clave según planta
                string claveDoc = String.Empty;

                switch (planta.clave)
                {
                    case 1:
                    default:
                        //para puebla y default
                        claveDoc = "PRF014-04";
                        break;
                    case 2:
                        //para silao
                        claveDoc = "PRF005-04";
                        break;

                }


                var cd = new System.Net.Mime.ContentDisposition
                {

                    // for example foo.bak
                    //FileName = planta.descripcion + "_" + produccion_Lineas.linea + "_" + fecha_inicial + "_" + dateFinal.ToString("yyyy-MM-dd") + ".xlsx",
                    FileName = Server.UrlEncode(claveDoc + "_Bitácora_de_Producción_" + planta.descripcion + ".xlsx"),

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(stream, "application/vnd.ms-excel");
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
