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
        
        public ActionResult Silao(string cliente, string fecha_inicial, string fecha_final)
        {
            if (TieneRol(TipoRoles.ADMIN))
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



                List<Bitacoras.DBUtil.ReportePesada> listado = Bitacoras.DBUtil.ReportesPesadasDBUtil.ObtieneReporteSilao(cliente, dateInicial, dateFinal);

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["cliente"] = cliente;
                routeValues["fecha_inicial"] = fecha_inicial;
                routeValues["fecha_final"] = fecha_final;

                ViewBag.Filtros = routeValues;
                ViewBag.Clientes = ComboSelect.obtieneClientesSilao();

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }


        // GET: ReportesPesadasPuebla/Puebla
        public ActionResult Puebla(string cliente, string fecha_inicial, string fecha_final)
        {
            if (TieneRol(TipoRoles.ADMIN))
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



                List<Bitacoras.DBUtil.ReportePesada> listado = Bitacoras.DBUtil.ReportesPesadasDBUtil.ObtieneReportePuebla(cliente, dateInicial, dateFinal);

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["cliente"] = cliente;
                routeValues["fecha_inicial"] = fecha_inicial;
                routeValues["fecha_final"] = fecha_final;

                ViewBag.Filtros = routeValues;
                ViewBag.Clientes = ComboSelect.obtieneClientesPuebla();

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }
    }
}
