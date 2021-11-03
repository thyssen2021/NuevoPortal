using System;
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
        public ActionResult Index(int? clave_planta, int? id_linea, string fecha_inicial, string fecha_final, int pagina = 1)
        {

            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }
                          

                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime dateInicial = new DateTime(2000,1,1);  //fecha inicial por defecto
                DateTime dateFinal = DateTime.Now;          //fecha final por defecto

                try
                {
                    if (!String.IsNullOrEmpty(fecha_inicial))
                        dateInicial = Convert.ToDateTime(fecha_inicial);
                    if (!String.IsNullOrEmpty(fecha_final))
                        dateFinal = Convert.ToDateTime(fecha_final);
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

                var cantidadRegistrosPorPagina = 20; // parámetro

                var listado = db.view_historico_resultado.Where(
                    x=> 
                    x.Planta.ToUpper().Contains(planta.descripcion.ToUpper()) 
                    && (x.Linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                    && x.Fecha >= dateInicial && x.Fecha <= dateFinal
                    && !x.SAP_Platina.ToUpper().Contains("TEMPORAL")
                    && !x.SAP_Rollo.ToUpper().Contains("TEMPORAL")
                    )
                    .OrderBy(x=> x.id)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.view_historico_resultado.Where(
                    x =>
                    x.Planta.ToUpper().Contains(planta.descripcion.ToUpper())
                    && (x.Linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                    && x.Fecha >= dateInicial && x.Fecha <= dateFinal
                    && !x.SAP_Platina.ToUpper().Contains("TEMPORAL")
                    && !x.SAP_Rollo.ToUpper().Contains("TEMPORAL")
                    ).Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["clave_planta"] = clave_planta;
                routeValues["id_linea"] = id_linea;
                routeValues["fecha_inicial"] = fecha_inicial;
                routeValues["fecha_final"] = fecha_final;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.id_linea = new SelectList(db.produccion_lineas.Where(p => p.activo == true), "id", "linea");
                ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.Paginacion = paginacion;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }
               
     
        public ActionResult Exportar(int? clave_planta, int? id_linea, string fecha_inicial, string fecha_final)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE))
            {
                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
                DateTime dateFinal = DateTime.Now;          //fecha final por defecto

                try
                {
                    if (!String.IsNullOrEmpty(fecha_inicial))
                        dateInicial = Convert.ToDateTime(fecha_inicial);
                    if (!String.IsNullOrEmpty(fecha_final))
                        dateFinal = Convert.ToDateTime(fecha_final);
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

                var listado = db.view_historico_resultado.Where(
                    x =>
                    x.Planta.ToUpper().Contains(planta.descripcion.ToUpper())
                    && (x.Linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                    && x.Fecha >= dateInicial && x.Fecha <= dateFinal
                    && !x.SAP_Platina.ToUpper().Contains("TEMPORAL")
                    && !x.SAP_Rollo.ToUpper().Contains("TEMPORAL")
                    )
                    .OrderBy(x => x.id).ToList();

                byte[] stream = ExcelUtil.GeneraReporteBitacorasExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = planta.descripcion + "_" + produccion_Lineas.linea + "_" + fecha_inicial + "_" + dateFinal.ToString("yyyy-MM-dd") + ".xlsx",

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
