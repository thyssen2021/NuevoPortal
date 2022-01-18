using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class InspeccionRegistrosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: InspeccionRegistros
        public ActionResult BusquedaRegistro(int? clave_planta, int? id_linea, string fecha_inicial, string fecha_final, int pagina = 1)
        {

            if (TieneRol(TipoRoles.INSPECCION_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                empleados emp = obtieneEmpleadoLogeado();

                //muestra error en caso de querer solicitar una planta distinta
                if (clave_planta != null && clave_planta != emp.planta_clave)
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


                List<produccion_registros> listado = db.produccion_registros.Where(
                        x =>
                        x.plantas.descripcion.ToUpper().Contains(planta.descripcion.ToUpper())
                        && (x.produccion_lineas.linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                        && x.fecha >= dateInicial && x.fecha <= dateFinal
                        && x.activo == true
                        && x.produccion_datos_entrada != null
                        //&& !x.sap_platina.ToUpper().Contains("TEMPORAL")
                        //&& !x.sap_rollo.ToUpper().Contains("TEMPORAL")
                        )
                        .OrderBy(x => x.id)
                        .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                        .Take(cantidadRegistrosPorPagina).ToList();

                int totalDeRegistros = db.produccion_registros.Where(
                        x =>
                        x.plantas.descripcion.ToUpper().Contains(planta.descripcion.ToUpper())
                        && (x.produccion_lineas.linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                        && x.fecha >= dateInicial && x.fecha <= dateFinal
                         && x.activo == true
                         && x.produccion_datos_entrada != null
                        //&& !x.sap_platina.ToUpper().Contains("TEMPORAL")
                        //&& !x.sap_rollo.ToUpper().Contains("TEMPORAL")
                        ).Count();


                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["clave_planta"] = clave_planta;
                routeValues["id_linea"] = id_linea;
                routeValues["fecha_inicial"] = fecha_inicial;
                routeValues["fecha_final"] = fecha_final;
                routeValues["pagina"] = pagina;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.id_linea = new SelectList(db.produccion_lineas.Where(p => p.activo == true), "id", "linea");
                ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true && p.clave == emp.planta_clave), "clave", "descripcion");
                ViewBag.Paginacion = paginacion;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: Reportes
        public ActionResult Reportes(int? clave_planta, int? id_linea, string fecha_inicial, string fecha_final, int pagina = 1)
        {

            if (TieneRol(TipoRoles.INSPECCION_REPORTES))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                empleados emp = obtieneEmpleadoLogeado();

                //muestra error en caso de querer solicitar una planta distinta
                if (clave_planta != null && clave_planta != emp.planta_clave)
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede acceder a la información solicitada!";
                    ViewBag.Descripcion = "No puede consultar la información de la planta solicitada.";

                    return View("../Home/ErrorGenerico");
                }


                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
                DateTime dateFinal = DateTime.Now;          //fecha final por defecto

                try
                {
                    if (!String.IsNullOrEmpty(fecha_inicial))
                        dateInicial = Convert.ToDateTime(fecha_inicial);
                    if (!String.IsNullOrEmpty(fecha_final))
                    {
                        dateFinal = Convert.ToDateTime(fecha_final);
                        dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
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

                List<PPM> listaPPMs = UtilPPM.ObtieneReportePorDia(db.produccion_registros.Where(
                        x =>
                        x.plantas.descripcion.ToUpper().Contains(planta.descripcion.ToUpper())
                        && (x.produccion_lineas.linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                        && x.fecha >= dateInicial && x.fecha <= dateFinal
                         && x.activo == true
                         && x.produccion_datos_entrada != null
                        )
                    .ToList(), fecha_inicial, fecha_final);


                List<PPM> listado = listaPPMs
                        .OrderBy(x => x.fecha.Value)
                        .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                        .Take(cantidadRegistrosPorPagina).ToList();

                int totalDeRegistros = listaPPMs.Count();


                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["clave_planta"] = clave_planta;
                routeValues["id_linea"] = id_linea;
                routeValues["fecha_inicial"] = fecha_inicial;
                routeValues["fecha_final"] = fecha_final;
                routeValues["pagina"] = pagina;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.id_linea = new SelectList(db.produccion_lineas.Where(p => p.activo == true), "id", "linea");
                ViewBag.clave_planta = new SelectList(db.plantas.Where(p => p.activo == true && p.clave == emp.planta_clave), "clave", "descripcion");
                ViewBag.Paginacion = paginacion;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: InspeccionRegistros/RegistroPiezasDescarte/5
        public ActionResult RegistroPiezasDescarte(int? id, int? clave_planta, int? id_linea, string fecha_inicial, string fecha_final, int pagina = 1)
        {
            //verifica los permisos del usuario
            if (TieneRol(TipoRoles.INSPECCION_REGISTRO))
            {
                //verifica si se envio un id
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }

                //busca si existe el registro de produccion
                produccion_registros produccion = db.produccion_registros.Find(id);

                if (produccion == null)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("No existe el registro de producción.", TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("BusquedaRegistro");
                }

                //busca si hay datos generales de registro de piezas de descarte de producción
                inspeccion_datos_generales datos_generales = db.inspeccion_datos_generales.FirstOrDefault(x => x.id_produccion_registro == id.Value);

                if (datos_generales == null)
                {
                    //si no hay registro de datos generales crea uno nuevo con el id de registro de produccion
                    datos_generales = new inspeccion_datos_generales
                    {
                        id_produccion_registro = id.Value,
                        produccion_registros = produccion
                    };
                }

                //agrega datos entrada a la produccion
                produccion.inspeccion_datos_generales = datos_generales;

                //pasa los parametros de la busqueda
                ViewBag.clave_planta = clave_planta;
                ViewBag.id_linea = id_linea;
                ViewBag.fecha_inicial = fecha_inicial;
                ViewBag.fecha_final = fecha_final;
                ViewBag.pagina = pagina;

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                List<inspeccion_categoria_fallas> listadoFallas = db.inspeccion_categoria_fallas.Where(p => p.activo == true).ToList();

                ViewBag.Empleado = empleado;
                ViewBag.ListadoFallas = listadoFallas;

                return View(produccion);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistroPiezasDescarte(produccion_registros produccion_registros, FormCollection collection, int? clave_planta, int? id_linea)
        {
            //obtiene el listado de piezas de descarte desde el formcollection
            List<inspeccion_pieza_descarte_produccion> listaPzasDescarte = new List<inspeccion_pieza_descarte_produccion>();

            foreach (string key in collection.AllKeys)
            {
                int indexPzaDescarte = -1;
                int id_falla = 0;
                int cantidad = 0;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    int.TryParse(m.Value, out indexPzaDescarte);
                    int.TryParse(collection["inspeccion_pieza_descarte_produccion[" + indexPzaDescarte + "].id_falla"], out id_falla);
                    int.TryParse(collection["inspeccion_pieza_descarte_produccion[" + indexPzaDescarte + "].cantidad"], out cantidad);

                    inspeccion_pieza_descarte_produccion item = listaPzasDescarte.FirstOrDefault(x => x.id == indexPzaDescarte);
                    if (item == null && id_falla != 0) //si no existe lo agrega
                    {
                        listaPzasDescarte.Add(
                            new inspeccion_pieza_descarte_produccion
                            {
                                id = indexPzaDescarte,
                                id_falla = id_falla,
                                cantidad = cantidad,
                                id_produccion_registro = produccion_registros.id
                            }
                        );
                    }
                }
            }

            //Verifica que no haya mas de dos veces el mimo id de falal
            bool duplicados = false;
            foreach (inspeccion_pieza_descarte_produccion item in listaPzasDescarte)
            {
                if (listaPzasDescarte.Where(x => x.id_falla == item.id_falla).ToList().Count > 1)
                    duplicados = true;
            }
            if (duplicados)
                ModelState.AddModelError("", "Verifique que cada falla sólo este definida una sóla vez.");

            if (ModelState.IsValid)
            {
                //verifica si existe datos generales
                if (db.inspeccion_datos_generales.Find(produccion_registros.id) == null) //si no existe en BD crea la entrada
                {
                    db.inspeccion_datos_generales.Add(produccion_registros.inspeccion_datos_generales);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch
                    {
                        EscribeExcepcion(new Exception("Trato de registrarse datos duplicados: " + produccion_registros.id), Clases.Models.EntradaRegistroEvento.TipoEntradaRegistroEvento.Error);
                    }
                }
                else  //si existe lo modifica
                {

                    inspeccion_datos_generales datos_generales = db.inspeccion_datos_generales.Find(produccion_registros.id);
                    // Activity already exist in database and modify it
                    db.Entry(datos_generales).CurrentValues.SetValues(produccion_registros.inspeccion_datos_generales);
                    db.Entry(datos_generales).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //borra las pzas descarte anteriores
                var listPzasDescarte = db.inspeccion_pieza_descarte_produccion.Where(x => x.id_produccion_registro == produccion_registros.id);
                foreach (inspeccion_pieza_descarte_produccion item in listPzasDescarte)
                    db.inspeccion_pieza_descarte_produccion.Remove(item);

                db.SaveChanges();

                //agrega las pza descarte nuevas
                foreach (inspeccion_pieza_descarte_produccion item in listaPzasDescarte)
                {
                    item.id = 0;
                    db.inspeccion_pieza_descarte_produccion.Add(item);
                    db.SaveChanges();
                }

                //en caso de exito
                TempData["Mensaje"] = new MensajesSweetAlert("Se han registrado correctamente las piezas de descarte.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("BusquedaRegistro",
                    new
                    {
                        clave_planta = clave_planta,
                        id_linea = id_linea,
                        fecha_inicial = Convert.ToString(collection["fecha_inicial"]),
                        fecha_final = Convert.ToString(collection["fecha_final"]),
                        pagina = Convert.ToString(collection["pagina"])
                    }
                    );
            }
            else
            {

                //asigna las piezas de desgaste
                produccion_registros.inspeccion_pieza_descarte_produccion = listaPzasDescarte;

                produccion_registros produccion = db.produccion_registros.Find(produccion_registros.id);
                produccion_registros.produccion_supervisores = produccion.produccion_supervisores;
                produccion_registros.produccion_operadores = produccion.produccion_operadores;
                produccion_registros.plantas = produccion.plantas;
                produccion_registros.produccion_lineas = produccion.produccion_lineas;
                produccion_registros.produccion_turnos = produccion.produccion_turnos;


                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                List<inspeccion_categoria_fallas> listadoFallas = db.inspeccion_categoria_fallas.Where(p => p.activo == true).ToList();

                ViewBag.Empleado = empleado;
                ViewBag.ListadoFallas = listadoFallas;

                return View(produccion_registros);
            }

        }

        // GET: InspeccionRegistros/RegistroPiezasDescarte/5
        public ActionResult Details(int? id)
        {
            //verifica los permisos del usuario
            if (TieneRol(TipoRoles.INSPECCION_REGISTRO) || TieneRol(TipoRoles.INSPECCION_REPORTES) || TieneRol(TipoRoles.INSPECCION_CATALOGOS))
            {
                //verifica si se envio un id
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }

                //busca si existe el registro de produccion
                produccion_registros produccion = db.produccion_registros.Find(id);

                if (produccion == null)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("No existe el registro de producción.", TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("BusquedaRegistro");
                }

                //busca si hay datos generales de registro de piezas de descarte de producción
                inspeccion_datos_generales datos_generales = db.inspeccion_datos_generales.FirstOrDefault(x => x.id_produccion_registro == id.Value);

                if (datos_generales == null)
                {
                    //si no hay registro de datos generales crea uno nuevo con el id de registro de produccion
                    datos_generales = new inspeccion_datos_generales
                    {
                        id_produccion_registro = id.Value,
                        produccion_registros = produccion
                    };
                }

                //agrega datos entrada a la produccion
                produccion.inspeccion_datos_generales = datos_generales;

                return View(produccion);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        public ActionResult Exportar(int? clave_planta, int? id_linea, string fecha_inicial, string fecha_final)
        {
            if (TieneRol(TipoRoles.INSPECCION_REPORTES))
            {
                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
                DateTime dateFinal = DateTime.Now;          //fecha final por defecto

                try
                {
                    if (!String.IsNullOrEmpty(fecha_inicial))
                        dateInicial = Convert.ToDateTime(fecha_inicial);
                    if (!String.IsNullOrEmpty(fecha_final))
                    {
                        dateFinal = Convert.ToDateTime(fecha_final);
                        dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
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


                //genera la lista de listado de fallas
                List<inspeccion_categoria_fallas> listadoFallas = db.inspeccion_categoria_fallas.OrderBy(x=>x.id).ToList();

                //lisado de registros
                List<produccion_registros> listaProduccionRegistros = db.produccion_registros.Where(
                                x =>
                                x.plantas.descripcion.ToUpper().Contains(planta.descripcion.ToUpper())
                                && (x.produccion_lineas.linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                                && x.fecha >= dateInicial && x.fecha <= dateFinal
                                 && x.activo == true
                                 && x.produccion_datos_entrada != null
                                )
                            .ToList();

                //listado de PPM
                List<PPM> listaPPMs = UtilPPM.ObtieneReportePorDia(listaProduccionRegistros, fecha_inicial, fecha_final);

                //obtiene el reporte de bitácora de producción
                List<view_historico_resultado> listadoHistorico = new List<view_historico_resultado>();
                listadoHistorico = db.view_historico_resultado.Where(
                        x =>
                        x.Planta.ToUpper().Contains(planta.descripcion.ToUpper())
                        && (x.Linea.ToUpper().Contains(produccion_Lineas.linea.ToUpper()) || linea == 0)
                        && x.Fecha >= dateInicial && x.Fecha <= dateFinal
                        && x.Column40.HasValue
                        //&& !x.SAP_Platina.ToUpper().Contains("TEMPORAL")
                        //&& !x.SAP_Rollo.ToUpper().Contains("TEMPORAL")
                        )
                        .OrderBy(x => x.id)
                       .ToList();


                byte[] stream = ExcelUtil.GeneraReportePPMExcel(listadoFallas, listaPPMs, listaProduccionRegistros, listadoHistorico);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = planta.descripcion + "_PPMs_" + fecha_inicial + "_" + dateFinal.ToString("yyyy-MM-dd") + ".xlsx",

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
    }
}