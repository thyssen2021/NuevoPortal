using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class ReportePesadasController : BaseController
    {
        // GET: ReportePesadas/Silao
        private Portal_2_0Entities db = new Portal_2_0Entities();

        //public ActionResult Silao(string cliente, string fecha_inicial, string fecha_final)
        //{
        //    if (TieneRol(TipoRoles.REPORTES_PESADAS))
        //    {
        //        //mensaje en caso de crear, editar, etc
        //        if (TempData["Mensaje"] != null)
        //        {
        //            ViewBag.MensajeAlert = TempData["Mensaje"];
        //        }


        //        CultureInfo provider = CultureInfo.InvariantCulture;

        //        DateTime dateInicial = DateTime.Now;
        //        DateTime dateFinal = DateTime.Now;

        //        try
        //        {
        //            if (!String.IsNullOrEmpty(fecha_inicial))
        //                dateInicial = Convert.ToDateTime(fecha_inicial);
        //            if (!String.IsNullOrEmpty(fecha_final))
        //                dateFinal = Convert.ToDateTime(fecha_final);
        //        }
        //        catch (FormatException e)
        //        {
        //            Console.WriteLine("Error de Formato: " + e.Message);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error al convertir: " + ex.Message);
        //        }

        //        List<Bitacoras.DBUtil.ReportePesada> listado = Bitacoras.DBUtil.ReportesPesadasDBUtil.ObtieneReporteSilao(cliente, dateInicial, dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59));

        //        System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
        //        routeValues["cliente"] = cliente;
        //        routeValues["fecha_inicial"] = fecha_inicial;
        //        routeValues["fecha_final"] = fecha_final;

        //        ViewBag.Filtros = routeValues;
        //        ViewBag.Clientes = ComboSelect.obtieneClientesSilao();

        //        return View(listado);
        //    }
        //    else
        //    {
        //        return View("../Home/ErrorPermisos");
        //    }
        //}


        //// GET: ReportesPesadasPuebla/Puebla
        //public ActionResult Puebla(string cliente, string fecha_inicial, string fecha_final)
        //{
        //    if (TieneRol(TipoRoles.REPORTES_PESADAS))
        //    {
        //        //mensaje en caso de crear, editar, etc
        //        if (TempData["Mensaje"] != null)
        //        {
        //            ViewBag.MensajeAlert = TempData["Mensaje"];
        //        }


        //        CultureInfo provider = CultureInfo.InvariantCulture;

        //        DateTime dateInicial = DateTime.Now;
        //        DateTime dateFinal = DateTime.Now;

        //        try
        //        {
        //            if (!String.IsNullOrEmpty(fecha_inicial))
        //                dateInicial = Convert.ToDateTime(fecha_inicial);
        //            if (!String.IsNullOrEmpty(fecha_final))
        //                dateFinal = Convert.ToDateTime(fecha_final);
        //        }
        //        catch (FormatException e)
        //        {
        //            Console.WriteLine("Error de Formato: " + e.Message);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error al convertir: " + ex.Message);
        //        }

        //        List<Bitacoras.DBUtil.ReportePesada> listado = Bitacoras.DBUtil.ReportesPesadasDBUtil.ObtieneReportePuebla(cliente, dateInicial, dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59));

        //        System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
        //        routeValues["cliente"] = cliente;
        //        routeValues["fecha_inicial"] = fecha_inicial;
        //        routeValues["fecha_final"] = fecha_final;

        //        ViewBag.Filtros = routeValues;
        //        ViewBag.Clientes = ComboSelect.obtieneClientesPuebla();

        //        return View(listado);
        //    }
        //    else
        //    {
        //        return View("../Home/ErrorPermisos");
        //    }
        //}

        // GET: ReportesPesadas/Index
        public ActionResult Index(string cliente, string fecha_inicial, string fecha_final, int? id_planta, string material, int muestra = 5)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }


                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime dateInicial = DateTime.Now;
                DateTime dateFinal = DateTime.Now;

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

                 List<Bitacoras.DBUtil.ReportePesada> listado = Bitacoras.DBUtil.ReportesPesadasDBUtil.ObtieneReportePuebla(cliente, dateInicial, dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59), id_planta, material, muestra);

                //List<Bitacoras.DBUtil.ReportePesada> listado = ObtieneReportePesadas(cliente, dateInicial, dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59), id_planta, material, muestra);

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["cliente"] = cliente;
                routeValues["fecha_inicial"] = fecha_inicial;
                routeValues["fecha_final"] = fecha_final;

                List<string> clientes = db.view_datos_base_reporte_pesadas.Where(x => x.clave_planta == id_planta).Select(x => x.invoiced_to).Distinct().OrderBy(x => x).ToList();
                List<SelectListItem> newList = new List<SelectListItem>();

                foreach (var p in clientes)
                {
                    newList.Add(new SelectListItem()
                    {
                        Text = p,
                        Value = p
                    });
                }

                //envia el select list por viewbag
                ViewBag.Filtros = routeValues;
                ViewBag.cliente = AddFirstItem(new SelectList(newList, "Value", "Text"), textoPorDefecto: "-- Seleccione --");
                ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas, nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --");

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // Crea funcion para procecesar del lado del servidor los datos del reporte de pesadas

        [NonAction]
        private List<Bitacoras.DBUtil.ReportePesada> ObtieneReportePesadas(string cliente, DateTime fecha_inicio, DateTime fecha_final, int? planta, string material, int muestra)
        {
            List<Bitacoras.DBUtil.ReportePesada> resultado = new List<Bitacoras.DBUtil.ReportePesada> ();

            return resultado;
        }



    }
}
