using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Presentation;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BudgetControllingController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BudgetControlling
        public ActionResult Centros(int? planta, int? responsable, string centro_costo, int pagina = 1)
        {

            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listado = db.budget_centro_costo
                        .Where(x => (x.budget_departamentos.budget_plantas.id == planta || planta == null)
                            && (x.budget_responsables.Any(x2 => x2.id_responsable == responsable) || responsable == null)
                            && (x.num_centro_costo.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                        )
                        .OrderByDescending(x => x.id)
                        .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                        .Take(cantidadRegistrosPorPagina).ToList();


                var totalDeRegistros = db.budget_centro_costo
                         .Where(x => (x.budget_departamentos.budget_plantas.id == planta || planta == null)
                            && (x.budget_responsables.Any(x2 => x2.id_responsable == responsable) || responsable == null)
                            && (x.num_centro_costo.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                        )
                        .Count();

                //para paginación
                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["planta"] = planta;
                routeValues["responsable"] = responsable;
                routeValues["centro_costo"] = centro_costo;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                var responsables = db.budget_responsables.ToList();
                var empleados = db.empleados.ToList();

                //obtiene el listado de empleados que son responsables de algún área
                List<empleados> listadoResponsables = empleados.Where(x => responsables.Any(y => y.id_responsable == x.id)).ToList();


                ViewBag.planta = AddFirstItem(new SelectList(db.budget_plantas.Where(x => x.activo == true), "id", "descripcion"), textoPorDefecto: "-- Todos --");
                ViewBag.responsable = AddFirstItem(new SelectList(listadoResponsables, "id", "ConcatNombre"), textoPorDefecto: "-- Todos --");
                ViewBag.id_centro_costo = AddFirstItem(new SelectList(db.budget_centro_costo, "id", "ConcatCentro"), textoPorDefecto: "-- Seleccionar --");
                ViewBag.id_fiscal_year = AddFirstItem(new SelectList(db.budget_anio_fiscal.Where(x => x.id != 1), "id", "ConcatAnio"), textoPorDefecto: "-- Seleccionar --");
                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones               
                ViewBag.Title = "Listado Centros de Costo";
                ViewBag.SegundoNivel = "centro_costos";
                // ViewBag.Create = true;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: ResponsableBudget/DetailsCentro
        public ActionResult DetailsCentro(int? id)
        {

            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                //obtiene centro de costo
                budget_centro_costo centroCosto = db.budget_centro_costo.Find(id);
                if (centroCosto == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal anterior (actual)
                budget_anio_fiscal anio_Fiscal_anterior = ResponsableBudgetController.GetAnioFiscal(DateTime.Now.AddYears(-1));
                if (anio_Fiscal_anterior == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal actual (forecast)
                budget_anio_fiscal anio_Fiscal_actual = ResponsableBudgetController.GetAnioFiscal(DateTime.Now);
                if (anio_Fiscal_actual == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal proximo (budget)
                budget_anio_fiscal anio_Fiscal_proximo = ResponsableBudgetController.GetAnioFiscal(DateTime.Now.AddYears(1));
                if (anio_Fiscal_proximo == null)
                    return View("../Error/NotFound");

                //obtiene el id_rel_centro_costo anterior
                budget_rel_fy_centro rel_fy_centro_anterior = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_anterior.id).FirstOrDefault();
                if (rel_fy_centro_anterior == null)
                {
                    //si no existe crea el rel anio forecast
                    rel_fy_centro_anterior = new budget_rel_fy_centro
                    {
                        id_anio_fiscal = anio_Fiscal_anterior.id,
                        id_centro_costo = centroCosto.id,
                        estatus = true //activado por defecto
                    };

                    db.budget_rel_fy_centro.Add(rel_fy_centro_anterior);
                    //guarda en base de datos el centro creado
                    db.SaveChanges();


                }

                //obtiene el id_rel_centro_costo del forecast
                budget_rel_fy_centro rel_fy_centro_presente = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_actual.id).FirstOrDefault();
                if (rel_fy_centro_presente == null)
                {
                    //si no existe crea el rel anio forecast
                    rel_fy_centro_presente = new budget_rel_fy_centro
                    {
                        id_anio_fiscal = anio_Fiscal_actual.id,
                        id_centro_costo = centroCosto.id,
                        estatus = true //activado por defecto
                    };

                    db.budget_rel_fy_centro.Add(rel_fy_centro_presente);
                    //guarda en base de datos el centro creado
                    db.SaveChanges();


                }

                //obtiene el id_rel_centro_costo del proximo
                budget_rel_fy_centro rel_fy_centro_proximo = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_proximo.id).FirstOrDefault();
                if (rel_fy_centro_proximo == null)
                {
                    //si no existe crea el rel anio forecast
                    rel_fy_centro_proximo = new budget_rel_fy_centro
                    {
                        id_anio_fiscal = anio_Fiscal_proximo.id,
                        id_centro_costo = centroCosto.id,
                        estatus = true //activado por defecto
                    };

                    db.budget_rel_fy_centro.Add(rel_fy_centro_proximo);
                    //guarda en base de datos el centro creado
                    db.SaveChanges();


                }

                //agrega el objeto de fiscal year
                rel_fy_centro_proximo.budget_anio_fiscal = anio_Fiscal_proximo;
                rel_fy_centro_presente.budget_anio_fiscal = anio_Fiscal_actual;
                rel_fy_centro_anterior.budget_anio_fiscal = anio_Fiscal_anterior;

                //obtiene los valores para cada cuenta sap
                var valoresListAnioAnterior = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_anterior.id && x.id_centro_costo == centroCosto.id).ToList();
                var valoresListAnioActual = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_actual.id && x.id_centro_costo == centroCosto.id).ToList();
                var valoresListAnioProximo = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_proximo.id && x.id_centro_costo == centroCosto.id).ToList();

                valoresListAnioAnterior = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioAnterior, anio_Fiscal_anterior.id, centroCosto.id).OrderBy(x => x.id_cuenta_sap).ToList();
                valoresListAnioActual = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioActual, anio_Fiscal_actual.id, centroCosto.id).OrderBy(x => x.id_cuenta_sap).ToList();
                valoresListAnioProximo = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioProximo, anio_Fiscal_proximo.id, centroCosto.id).OrderBy(x => x.id_cuenta_sap).ToList();


                ViewBag.centroCosto = centroCosto;
                ViewBag.rel_anterior = rel_fy_centro_anterior;
                ViewBag.rel_presente = rel_fy_centro_presente;
                ViewBag.rel_proximo = rel_fy_centro_proximo;
                ViewBag.ValoresActual = valoresListAnioAnterior;
                ViewBag.ValoresActualForecast = valoresListAnioActual;
                ViewBag.ValoresBudget = valoresListAnioProximo;

                return View(centroCosto);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: BudgetControlling/Edit
        public ActionResult Edit(int? id_centro_costo, int? id_fiscal_year)
        {

            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id_centro_costo == null || id_fiscal_year == null)
                {
                    return View("../Error/BadRequest");
                }

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                //obtiene centro de costo
                budget_centro_costo centroCosto = db.budget_centro_costo.Find(id_centro_costo);
                if (centroCosto == null)
                    return View("../Error/NotFound");


                //obtiene el año fiscal actual (forecast)
                budget_anio_fiscal anio_Fiscal_actual = db.budget_anio_fiscal.Find(id_fiscal_year);
                if (anio_Fiscal_actual == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal anterior (actual)
                budget_anio_fiscal anio_Fiscal_anterior = db.budget_anio_fiscal.Find(anio_Fiscal_actual.id - 1);
                if (anio_Fiscal_anterior == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal proximo (budget)
                budget_anio_fiscal anio_Fiscal_proximo = db.budget_anio_fiscal.Find(anio_Fiscal_actual.id + 1);
                if (anio_Fiscal_proximo == null)
                    return View("../Error/NotFound");

                //obtiene el id_rel_centro_costo anterior
                budget_rel_fy_centro rel_fy_centro_anterior = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_anterior.id).FirstOrDefault();
                if (rel_fy_centro_anterior == null)
                {
                    //si no existe crea el rel anio forecast
                    rel_fy_centro_anterior = new budget_rel_fy_centro
                    {
                        id_anio_fiscal = anio_Fiscal_anterior.id,
                        id_centro_costo = centroCosto.id,
                        estatus = true //activado por defecto
                    };

                    db.budget_rel_fy_centro.Add(rel_fy_centro_anterior);
                    //guarda en base de datos el centro creado
                    db.SaveChanges();
                }

                //obtiene el id_rel_centro_costo del forecast
                budget_rel_fy_centro rel_fy_centro_presente = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_actual.id).FirstOrDefault();
                if (rel_fy_centro_presente == null)
                {
                    //si no existe crea el rel anio forecast
                    rel_fy_centro_presente = new budget_rel_fy_centro
                    {
                        id_anio_fiscal = anio_Fiscal_actual.id,
                        id_centro_costo = centroCosto.id,
                        estatus = true //activado por defecto
                    };

                    db.budget_rel_fy_centro.Add(rel_fy_centro_presente);
                    //guarda en base de datos el centro creado
                    db.SaveChanges();
                }

                //obtiene el id_rel_centro_costo del proximo
                budget_rel_fy_centro rel_fy_centro_proximo = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_proximo.id).FirstOrDefault();
                if (rel_fy_centro_proximo == null)
                {
                    //si no existe crea el rel anio forecast
                    rel_fy_centro_proximo = new budget_rel_fy_centro
                    {
                        id_anio_fiscal = anio_Fiscal_proximo.id,
                        id_centro_costo = centroCosto.id,
                        estatus = true //activado por defecto
                    };

                    db.budget_rel_fy_centro.Add(rel_fy_centro_proximo);
                    //guarda en base de datos el centro creado
                    db.SaveChanges();
                }

                ////agrega el objeto de fiscal year
                //rel_fy_centro_proximo.budget_anio_fiscal = anio_Fiscal_proximo;
                //rel_fy_centro_presente.budget_anio_fiscal = anio_Fiscal_actual;
                //rel_fy_centro_anterior.budget_anio_fiscal = anio_Fiscal_anterior;

                ////obtiene los valores para cada cuenta sap
                //var valoresListAnioAnterior = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_anterior.id && x.id_centro_costo == centroCosto.id).ToList();
                //var valoresListAnioActual = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_actual.id && x.id_centro_costo == centroCosto.id).ToList();
                //var valoresListAnioProximo = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_proximo.id && x.id_centro_costo == centroCosto.id).ToList();

                //valoresListAnioAnterior = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioAnterior, anio_Fiscal_anterior.id, centroCosto.id).OrderBy(x => x.sap_account).ToList();
                //valoresListAnioActual = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioActual, anio_Fiscal_actual.id, centroCosto.id).OrderBy(x => x.sap_account).ToList();
                //valoresListAnioProximo = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioProximo, anio_Fiscal_proximo.id, centroCosto.id).OrderBy(x => x.sap_account).ToList();

                //ViewBag.centroCosto = centroCosto;
                //ViewBag.rel_anterior = rel_fy_centro_anterior;
                //ViewBag.rel_presente = rel_fy_centro_presente;
                //ViewBag.rel_proximo = rel_fy_centro_proximo;
                //ViewBag.ValoresActual = valoresListAnioAnterior;
                //ViewBag.ValoresActualForecast = valoresListAnioActual;
                //ViewBag.ValoresBudget = valoresListAnioProximo;

                //return View(centroCosto);

                //redirige a metodo de budget Responsable
                return RedirectToAction("EditCentroPresenteHT", "ResponsableBudget", new { id = id_centro_costo, proximo = false, info = false, controlling = true, id_fiscal_year = id_fiscal_year });

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: ResponsableBudget/EditCentro
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection form)
        {
            //crea objetos apartir del form collection

            int id_rel_fy = 0;
            int.TryParse(form["id_rel_fy_centro"], out id_rel_fy);

            //obtiene el id rel centro fy
            budget_rel_fy_centro rel_Fy_Centro = db.budget_rel_fy_centro.Find(id_rel_fy);

            if (rel_Fy_Centro == null)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + "No existe la relación de centro de costo con año fiscal", TipoMensajesSweetAlerts.ERROR);
            }

            #region cantidades
            //1.-Obtiene los keys de tipo cantidad
            List<string> keysCantidades = form.AllKeys.Where(x => x.ToUpper().Contains("CANTIDAD") == true).ToList();

            //2.- Recorre y crea un objeto de tipo budget_valores
            List<budget_cantidad> valores = new List<budget_cantidad>();

            foreach (var keyC in keysCantidades)
            {
                Match m = Regex.Match(keyC, "(\\d+)");
                string num = string.Empty;

                //extrae el numero de key
                if (m.Success)
                {
                    num = m.Value;

                    int id_cuenta_sap = 0;
                    int mes = 0;
                    decimal cantidad = 0;

                    //obtiene el valor de la cuenta sap
                    bool success_id_cuenta_sap = int.TryParse(form["budget_valores[" + num + "].id_cuenta_sap"], out id_cuenta_sap);
                    bool success_mes = int.TryParse(form["budget_valores[" + num + "].mes"], out mes);
                    bool succes_cantidad = decimal.TryParse(form["budget_valores[" + num + "].cantidad"], out cantidad);
                    string currency = "USD";  //currency por defecto

                    //verifica que no hubo errores al covertir los datos
                    if (success_id_cuenta_sap && success_mes)
                    {
                        //crea y agrega un objeto de tipo budget_valores con la información leida
                        valores.Add(new budget_cantidad
                        {
                            id_budget_rel_fy_centro = id_rel_fy,
                            id_cuenta_sap = id_cuenta_sap,
                            mes = mes,
                            currency_iso = currency,
                            cantidad = cantidad,
                            comentario = string.IsNullOrEmpty(form["budget_valores[" + num + "].comentario"]) ? null : form["budget_valores[" + num + "].comentario"]
                        });
                    }

                }
            }



            //3.- identificar cuales son update, create, delete y sin cambios

            //obtiene los valores actuales en BD
            List<budget_cantidad> valoresEnBD = db.budget_cantidad.Where(x => x.id_budget_rel_fy_centro == id_rel_fy).ToList();

            //obtiene valores sin cambios
            List<budget_cantidad> valoresSinCambio = valores.Where(x => valoresEnBD.Any(y => y.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro && y.id_cuenta_sap == x.id_cuenta_sap && y.mes == x.mes && y.comentario == x.comentario && y.cantidad == x.cantidad)).ToList();

            //obtiene valores con cambios o nuevos
            List<budget_cantidad> valoresDiferentes = valores.Except(valoresSinCambio).ToList();

            //DE valores Diferentes identificar cuales son create, update y delete
            List<budget_cantidad> valoresCreate = valoresDiferentes.Where(a => !valoresEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes) && (a.cantidad != 0 || !string.IsNullOrEmpty(a.comentario))).ToList();

            //DE valores Diferentes identificar cuales son create, update y delete
            List<budget_cantidad> valoresUpdate = valoresDiferentes.Where(a => valoresEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes && (a.cantidad != 0 || !string.IsNullOrEmpty(a.comentario)))).ToList();
            //agrega el id correspondiente
            valoresUpdate = valoresUpdate.Select(x =>
            {
                x.id = (from v in valoresEnBD
                        where v.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro && v.id_cuenta_sap == x.id_cuenta_sap && v.mes == x.mes
                        select v.id).FirstOrDefault(); return x;
            }).ToList();

            //DE valores Diferentes identificar cuales son create, update y delete
            List<budget_cantidad> valoresDelete = valoresDiferentes.Where(a => valoresEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes && a.cantidad == 0 && string.IsNullOrEmpty(a.comentario))).ToList();
            //agrega el id  correspondiente
            valoresDelete = valoresDelete.Select(x =>
            {
                x.id = (from v in valoresEnBD
                        where v.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro && v.id_cuenta_sap == x.id_cuenta_sap && v.mes == x.mes
                        select v.id).FirstOrDefault(); return x;
            }).ToList();

            //crea los nuevos registros
            try
            {
                db.budget_cantidad.AddRange(valoresCreate);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }

            //modifica los registros actuales
            try
            {
                foreach (budget_cantidad item in valoresUpdate)
                {
                    //obtiene el elemento de BD
                    budget_cantidad objeto = valoresEnBD.FirstOrDefault(x => x.id == item.id);

                    if (objeto != null)
                    {
                        db.Entry(objeto).CurrentValues.SetValues(item);

                    }

                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }

            //Elimina los elemntos necesarios 
            try
            {
                foreach (budget_cantidad item in valoresDelete)
                {
                    //obtiene el elemento de BD
                    budget_cantidad objeto = valoresEnBD.FirstOrDefault(x => x.id == item.id);

                    if (objeto != null)
                    {
                        db.budget_cantidad.Remove(objeto);
                    }

                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }

            #endregion

            #region comentarios
            //1.-Obtiene los keys de tipo comentario
            List<string> keysComentarios = form.AllKeys.Where(x => x.ToUpper().Contains(".COMENTARIO") && x.ToUpper().Contains("BUDGET_REL_COMENTARIOS")).ToList();

            //2.- Recorre y crea un objeto de tipo budget_comentarios
            List<budget_rel_comentarios> listComentarios = new List<budget_rel_comentarios>();

            foreach (var keyC in keysComentarios)
            {
                Match m = Regex.Match(keyC, "(\\d+)");
                string num = string.Empty;

                //extrae el numero de key
                if (m.Success)
                {
                    num = m.Value;

                    int id_cuenta_sap = 0;

                    //obtiene el valor de la cuenta sap
                    bool success_id_cuenta_sap = int.TryParse(form["budget_rel_comentarios[" + num + "].id_cuenta_sap"], out id_cuenta_sap);
                    string comen = form["budget_rel_comentarios[" + num + "].comentario"].ToString();


                    //verifica que no hubo errores al covertir los datos
                    if (success_id_cuenta_sap && !String.IsNullOrEmpty(comen))
                    {
                        //crea y agrega un objeto de tipo budget_valores con la información leida
                        listComentarios.Add(new budget_rel_comentarios
                        {
                            id_budget_rel_fy_centro = id_rel_fy,
                            id_cuenta_sap = id_cuenta_sap,
                            comentarios = comen
                        });
                    }

                }
            }

            //3.- identificar cuales son update, create, delete y sin cambios

            //obtiene los valores actuales en BD
            List<budget_rel_comentarios> comentariosEnBD = db.budget_rel_comentarios.Where(x => x.id_budget_rel_fy_centro == id_rel_fy).ToList();

            //valores create
            List<budget_rel_comentarios> valoresCreateComentarios = listComentarios.Where(a => !comentariosEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap) && !String.IsNullOrEmpty(a.comentarios)).ToList();

            //valores updated
            List<budget_rel_comentarios> valoresUpdateComentarios = listComentarios.Where(a => comentariosEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a.comentarios != a1.comentarios && !String.IsNullOrEmpty(a.comentarios))).ToList();

            //agrega id a update
            valoresUpdateComentarios = valoresUpdateComentarios.Select(x =>
            {
                x.id = (from v in comentariosEnBD
                        where v.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro && v.id_cuenta_sap == x.id_cuenta_sap
                        select v.id).FirstOrDefault(); return x;
            }).ToList();

            //valores delete
            List<budget_rel_comentarios> valoresDeleteComentarios = comentariosEnBD.Where(a => !listComentarios.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap)).ToList();
            valoresDeleteComentarios.AddRange(listComentarios.Where(a => comentariosEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && String.IsNullOrEmpty(a.comentarios))).ToList());

            //agrega el id a delete
            valoresDeleteComentarios = valoresDeleteComentarios.Select(x =>
            {
                x.id = (from v in comentariosEnBD
                        where v.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro && v.id_cuenta_sap == x.id_cuenta_sap
                        select v.id).FirstOrDefault(); return x;
            }).ToList();

            //crea los nuevos registros
            try
            {
                db.budget_rel_comentarios.AddRange(valoresCreateComentarios);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }

            //modifica los registros actuales
            try
            {
                foreach (budget_rel_comentarios item in valoresUpdateComentarios)
                {
                    //obtiene el elemento de BD
                    budget_rel_comentarios objeto = comentariosEnBD.FirstOrDefault(x => x.id == item.id);

                    if (objeto != null)
                    {
                        db.Entry(objeto).CurrentValues.SetValues(item);

                    }

                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }

            //Elimina los elemntos necesarios 
            try
            {
                foreach (budget_rel_comentarios item in valoresDeleteComentarios)
                {
                    //obtiene el elemento de BD
                    budget_rel_comentarios objeto = comentariosEnBD.FirstOrDefault(x => x.id == item.id);

                    if (objeto != null)
                    {
                        db.budget_rel_comentarios.Remove(objeto);
                    }

                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }

            #endregion

            TempData["Mensaje"] = new MensajesSweetAlert("Se ha modificado correctamente. ", TipoMensajesSweetAlerts.SUCCESS);

            return RedirectToAction("Edit", new { id_centro_costo = rel_Fy_Centro.id_centro_costo, id_fiscal_year = rel_Fy_Centro.id_anio_fiscal });
        }

        // GET: ResponsableBudget/ImportActual
        public ActionResult ImportActual(int? id)
        {

            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }

                //obtiene centro de costo
                budget_centro_costo centroCosto = db.budget_centro_costo.Find(id);
                if (centroCosto == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal anterior (actual)
                budget_anio_fiscal anio_Fiscal_anterior = ResponsableBudgetController.GetAnioFiscal(DateTime.Now.AddYears(-1));
                if (anio_Fiscal_anterior == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal actual (forecast)
                budget_anio_fiscal anio_Fiscal_actual = ResponsableBudgetController.GetAnioFiscal(DateTime.Now);
                if (anio_Fiscal_actual == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal proximo (budget)
                budget_anio_fiscal anio_Fiscal_proximo = ResponsableBudgetController.GetAnioFiscal(DateTime.Now.AddYears(1));
                if (anio_Fiscal_proximo == null)
                    return View("../Error/NotFound");

                //obtiene el id_rel_centro_costo anterior
                budget_rel_fy_centro rel_fy_centro_anterior = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_anterior.id).FirstOrDefault();
                if (rel_fy_centro_anterior == null)
                {
                    //si no existe crea el rel anio forecast
                    rel_fy_centro_anterior = new budget_rel_fy_centro
                    {
                        id_anio_fiscal = anio_Fiscal_anterior.id,
                        id_centro_costo = centroCosto.id,
                        estatus = true //activado por defecto
                    };

                    db.budget_rel_fy_centro.Add(rel_fy_centro_anterior);
                    //guarda en base de datos el centro creado
                    db.SaveChanges();
                }

                //obtiene el id_rel_centro_costo del forecast
                budget_rel_fy_centro rel_fy_centro_presente = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_actual.id).FirstOrDefault();
                if (rel_fy_centro_presente == null)
                {
                    //si no existe crea el rel anio forecast
                    rel_fy_centro_presente = new budget_rel_fy_centro
                    {
                        id_anio_fiscal = anio_Fiscal_actual.id,
                        id_centro_costo = centroCosto.id,
                        estatus = true //activado por defecto
                    };

                    db.budget_rel_fy_centro.Add(rel_fy_centro_presente);
                    //guarda en base de datos el centro creado
                    db.SaveChanges();
                }

                //obtiene el id_rel_centro_costo del proximo
                budget_rel_fy_centro rel_fy_centro_proximo = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_proximo.id).FirstOrDefault();
                if (rel_fy_centro_proximo == null)
                {
                    //si no existe crea el rel anio forecast
                    rel_fy_centro_proximo = new budget_rel_fy_centro
                    {
                        id_anio_fiscal = anio_Fiscal_proximo.id,
                        id_centro_costo = centroCosto.id,
                        estatus = true //activado por defecto
                    };

                    db.budget_rel_fy_centro.Add(rel_fy_centro_proximo);
                    //guarda en base de datos el centro creado
                    db.SaveChanges();
                }

                //agrega el objeto de fiscal year
                rel_fy_centro_proximo.budget_anio_fiscal = anio_Fiscal_proximo;
                rel_fy_centro_presente.budget_anio_fiscal = anio_Fiscal_actual;
                rel_fy_centro_anterior.budget_anio_fiscal = anio_Fiscal_anterior;

                //obtiene los valores para cada cuenta sap
                var valoresListAnioAnterior = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_anterior.id && x.id_centro_costo == centroCosto.id).ToList();
                var valoresListAnioActual = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_actual.id && x.id_centro_costo == centroCosto.id).ToList();
                var valoresListAnioProximo = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_proximo.id && x.id_centro_costo == centroCosto.id).ToList();

                valoresListAnioAnterior = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioAnterior, anio_Fiscal_anterior.id, centroCosto.id).OrderBy(x => x.id_cuenta_sap).ToList();
                valoresListAnioActual = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioActual, anio_Fiscal_actual.id, centroCosto.id).OrderBy(x => x.id_cuenta_sap).ToList();
                valoresListAnioProximo = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioProximo, anio_Fiscal_proximo.id, centroCosto.id).OrderBy(x => x.id_cuenta_sap).ToList();


                ViewBag.centroCosto = centroCosto;
                ViewBag.anio_Fiscal_anterior = anio_Fiscal_anterior;
                ViewBag.anio_Fiscal_actual = anio_Fiscal_actual;
                ViewBag.anio_Fiscal_proximo = anio_Fiscal_proximo;
                ViewBag.ValoresActual = valoresListAnioAnterior;
                ViewBag.ValoresActualForecast = valoresListAnioActual;
                ViewBag.ValoresBudget = valoresListAnioProximo;

                return View(centroCosto);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: ResponsableBudget/ImportActual
        [HttpPost]
        public ActionResult ImportActual(budget_centro_costo centro, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                string msjError = "No se ha podido leer el archivo seleccionado.";

                //lee el archivo seleccionado
                try
                {
                    HttpPostedFileBase stream = Request.Files["PostedFile"];


                    if (stream.InputStream.Length > 8388608)
                    {
                        msjError = "Sólo se permiten archivos con peso menor a 8 MB.";
                        throw new Exception(msjError);
                    }
                    else
                    {
                        string extension = Path.GetExtension(centro.PostedFile.FileName);
                        if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                        {
                            msjError = "Sólo se permiten archivos Excel.";
                            throw new Exception(msjError);
                        }
                    }

                    bool estructuraValida = false;
                    int noEncontrados = 0;

                    List<budget_rel_comentarios> lista_comentarios_form = new List<budget_rel_comentarios>();
                    List<int> idRels = new List<int>();

                    //el archivo es válido
                    List<budget_cantidad> lista_cantidad_form = UtilExcel.LeeActual(stream, 10, ref estructuraValida, ref msjError, ref noEncontrados, ref lista_comentarios_form, ref idRels);
                    

                    //obtiene el listado actual de la BD
                    List<budget_cantidad> lista_cantidad_BD = db.budget_cantidad.Where(x => idRels.Contains(x.id_budget_rel_fy_centro)).ToList();

                    //obtiene el listado actual de la BD
                    List<budget_rel_comentarios> lista_comentarios_BD = db.budget_rel_comentarios.Where(x => idRels.Contains(x.id_budget_rel_fy_centro)).ToList();

                    /*crea una copia de cada cantidad en moneda local
                        !!! Eliminar cuando la plantilla incluya los tres tipos de moneda
                     */
                    ////////////////////////////////////////////////////////////////
                    List<budget_cantidad> tempLocalUSD = new List<budget_cantidad>();
                    foreach (var item in lista_cantidad_form)
                    {
                        tempLocalUSD.Add(new budget_cantidad
                        {
                            id_budget_rel_fy_centro = item.id_budget_rel_fy_centro,
                            id_cuenta_sap = item.id_cuenta_sap,
                            mes = item.mes,
                            currency_iso = "USD",
                            cantidad = item.cantidad,
                            comentario = item.comentario,
                            moneda_local_usd = true
                        });
                    }
                    lista_cantidad_form.AddRange(tempLocalUSD);
                    /////////////////////////////////////////////////////////////////

                    if (!estructuraValida)
                    {
                        msjError = "No cumple con la estructura válida. " + msjError;
                        throw new Exception(msjError);
                    }
                    else if (lista_cantidad_form.Count > 0)//cumple con la estructura válida
                    {

                        //Recorre todas las cantidades de la tabla
                        for (int i = 0; i < lista_cantidad_form.Count; i++)
                        {
                            budget_cantidad itemBD = lista_cantidad_BD.FirstOrDefault(x =>
                                                        x.id_cuenta_sap == lista_cantidad_form[i].id_cuenta_sap
                                                        && x.id_budget_rel_fy_centro == lista_cantidad_form[i].id_budget_rel_fy_centro
                                                        && x.mes == lista_cantidad_form[i].mes
                                                        && x.currency_iso == lista_cantidad_form[i].currency_iso
                                                        && x.moneda_local_usd == lista_cantidad_form[i].moneda_local_usd
                                                    );
                            //EXISTE
                            if (itemBD != null)
                            {
                                //UPDATE
                                if (itemBD.cantidad != lista_cantidad_form[i].cantidad)
                                    itemBD.cantidad = lista_cantidad_form[i].cantidad;
                            }
                            else  //CREATE
                            {
                                db.budget_cantidad.Add(lista_cantidad_form[i]);
                            }
                        }

                        //DELETE
                        //elimina aquellos que no aparezcan en los enviados
                        List<budget_cantidad> toDeleteList = lista_cantidad_BD.Where(x => !lista_cantidad_form.Any(y => y.id_cuenta_sap == x.id_cuenta_sap
                                        && y.mes == x.mes
                                        && y.currency_iso == x.currency_iso
                                        && y.moneda_local_usd == x.moneda_local_usd
                                        && y.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro
                                        )).ToList();

                        //borra los conceptos que inpiden el borrado
                        foreach (var item in toDeleteList.Where(x=>x.budget_rel_conceptos_cantidades.Count>0))
                            db.budget_rel_conceptos_cantidades.RemoveRange(item.budget_rel_conceptos_cantidades);

                        db.budget_cantidad.RemoveRange(toDeleteList);

                        //crea, modifica o elimina COMENTARIOS generales
                        for (int i = 0; i < lista_comentarios_form.Count; i++)
                        {
                            var itemBD = lista_comentarios_BD.FirstOrDefault(x => x.id_cuenta_sap == lista_comentarios_form[i].id_cuenta_sap && x.id_budget_rel_fy_centro == lista_comentarios_form[i].id_budget_rel_fy_centro);
                            //EXISTE
                            if (itemBD != null)
                            {
                                //UPDATE
                                if (itemBD.comentarios != lista_comentarios_form[i].comentarios)
                                    itemBD.comentarios = lista_comentarios_form[i].comentarios;
                            }
                            else  //CREATE
                            {
                                db.budget_rel_comentarios.Add(lista_comentarios_form[i]);
                            }
                        }

                        //DELETE
                        //elimina aquellos que no aparezcan en los enviados
                        List<budget_rel_comentarios> toDeleteListComentarios = lista_comentarios_BD.Where(x => !lista_comentarios_form.Any(y => y.id_cuenta_sap == x.id_cuenta_sap)).ToList();
                        db.budget_rel_comentarios.RemoveRange(toDeleteListComentarios);

                        try
                        {
                            db.SaveChanges();
                            if (noEncontrados == 0)
                                TempData["Mensaje"] = new MensajesSweetAlert("Se ha actualizado correctamente.", TipoMensajesSweetAlerts.INFO);
                            else
                                TempData["Mensaje"] = new MensajesSweetAlert("Algunas cuentas SAP no se encontraron: " + noEncontrados, TipoMensajesSweetAlerts.WARNING);
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("", "Error: " + e.Message);
                        }

                        return RedirectToAction("EditCentroPresenteHT", "ResponsableBudget", new { id = centro.id, import = true });
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Error: " + e.Message);
                }

            }


            //obtiene centro de costo
            budget_centro_costo centroCosto = db.budget_centro_costo.Find(centro.id);
            if (centroCosto == null)
                return View("../Error/NotFound");

            //obtiene el año fiscal anterior (actual)
            budget_anio_fiscal anio_Fiscal_anterior = ResponsableBudgetController.GetAnioFiscal(DateTime.Now.AddYears(-1));
            if (anio_Fiscal_anterior == null)
                return View("../Error/NotFound");

            //obtiene el año fiscal actual (forecast)
            budget_anio_fiscal anio_Fiscal_actual = ResponsableBudgetController.GetAnioFiscal(DateTime.Now);
            if (anio_Fiscal_actual == null)
                return View("../Error/NotFound");

            //obtiene el año fiscal proximo (budget)
            budget_anio_fiscal anio_Fiscal_proximo = ResponsableBudgetController.GetAnioFiscal(DateTime.Now.AddYears(1));
            if (anio_Fiscal_proximo == null)
                return View("../Error/NotFound");

            //obtiene el id_rel_centro_costo del budget
            budget_rel_fy_centro rel_fy_centro_anterior = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_anterior.id).FirstOrDefault();
            budget_rel_fy_centro rel_fy_centro_presente = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_actual.id).FirstOrDefault();
            budget_rel_fy_centro rel_fy_centro_proximo = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_proximo.id).FirstOrDefault();



            //obtiene los valores para cada cuenta sap
            var valoresListAnioAnterior = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_anterior.id && x.id_centro_costo == centroCosto.id).ToList();
            var valoresListAnioActual = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_actual.id && x.id_centro_costo == centroCosto.id).ToList();
            var valoresListAnioProximo = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_proximo.id && x.id_centro_costo == centroCosto.id).ToList();

            valoresListAnioAnterior = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioAnterior, anio_Fiscal_anterior.id, centroCosto.id);
            valoresListAnioActual = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioActual, anio_Fiscal_actual.id, centroCosto.id).OrderBy(x => x.id_cuenta_sap).ToList();
            valoresListAnioProximo = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioProximo, anio_Fiscal_proximo.id, centroCosto.id);


            ViewBag.centroCosto = centroCosto;
            ViewBag.anio_Fiscal_anterior = anio_Fiscal_anterior;
            ViewBag.anio_Fiscal_actual = anio_Fiscal_actual;
            ViewBag.anio_Fiscal_proximo = anio_Fiscal_proximo;
            ViewBag.ValoresActual = valoresListAnioAnterior;
            ViewBag.ValoresActualForecast = valoresListAnioActual;
            ViewBag.ValoresBudget = valoresListAnioProximo;

            return View(centroCosto);
        }


        // GET: Bom/CargaBom/5
        public ActionResult CargaConcentrado()
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {

                ViewBag.planta = AddFirstItem(new SelectList(db.budget_plantas.Where(x => x.activo == true), "id", "descripcion"), textoPorDefecto: "-- Todos --");
                ViewBag.centro_costo = AddFirstItem(new SelectList(db.budget_centro_costo.Where(x => x.activo == true), "id", nameof(budget_centro_costo.ConcatCentro)), textoPorDefecto: "-- Todos --");
                ViewBag.id_fiscal_year = AddFirstItem(new SelectList(db.budget_anio_fiscal.Where(x => x.id != 1), "id", "ConcatAnio"), textoPorDefecto: "-- Seleccionar --");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Dante/CargaConcentrado/5
        // POST: BudgetControlling/CargaConcentrado
        [HttpPost]
        public ActionResult CargaConcentrado(ExcelViewModel excelViewModel, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                string msjError = "No se ha podido leer el archivo seleccionado.";

                try
                {
                    HttpPostedFileBase stream = Request.Files["PostedFile"];

                    // Validación de tamaño y extensión
                    if (stream.InputStream.Length > 8388608)
                    {
                        msjError = "Sólo se permiten archivos con peso menor a 8 MB.";
                        throw new Exception(msjError);
                    }
                    else
                    {
                        string extension = Path.GetExtension(excelViewModel.PostedFile.FileName);
                        if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                        {
                            msjError = "Sólo se permiten archivos Excel.";
                            throw new Exception(msjError);
                        }
                    }

                    bool estructuraValida = false;
                    int noEncontrados = 0;
                    List<budget_rel_comentarios> lista_comentarios_form = new List<budget_rel_comentarios>();
                    List<int> idRels = new List<int>();

                    // Lee el archivo Excel y obtiene la lista de cantidades
                    List<budget_cantidad> lista_cantidad_form = UtilExcel.BudgetLeeConcentrado(
                        stream, 5, ref estructuraValida, ref msjError, ref noEncontrados,
                        ref lista_comentarios_form, ref idRels);

                    if (!estructuraValida)
                    {
                        msjError = "No cumple con la estructura válida. " + msjError;
                        throw new Exception(msjError);
                    }

                    if (lista_cantidad_form.Count > 0)
                    {
                        // 1. Obtener cadena de conexión (Patrón solicitado)
                        string efConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Portal_2_0Entities"].ConnectionString;
                        var builder = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(efConnectionString);
                        string connectionString = builder.ProviderConnectionString;

                        // 2. Iniciar conexión y transacción SQL nativa
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            using (SqlTransaction transaction = conn.BeginTransaction())
                            {
                                try
                                {
                                    // --- BLOQUE 1: PROCESAMIENTO DE CANTIDADES ---

                                    // A. Crear tabla temporal para Cantidades
                                    string createTempCantidades = @"
                                        CREATE TABLE #TempBudgetCantidad (
                                            id_budget_rel_fy_centro int,
                                            id_cuenta_sap int,
                                            mes int,
                                            currency_iso varchar(3),
                                            cantidad decimal(14,2),
                                            comentario varchar(150),
                                            moneda_local_usd bit
                                        )";
                                    using (SqlCommand cmd = new SqlCommand(createTempCantidades, conn, transaction)) { cmd.ExecuteNonQuery(); }

                                    // B. Preparar DataTable
                                    System.Data.DataTable dtCantidades = new System.Data.DataTable();
                                    dtCantidades.Columns.Add("id_budget_rel_fy_centro", typeof(int));
                                    dtCantidades.Columns.Add("id_cuenta_sap", typeof(int));
                                    dtCantidades.Columns.Add("mes", typeof(int));
                                    dtCantidades.Columns.Add("currency_iso", typeof(string));
                                    dtCantidades.Columns.Add("cantidad", typeof(decimal));
                                    dtCantidades.Columns.Add("comentario", typeof(string));
                                    dtCantidades.Columns.Add("moneda_local_usd", typeof(bool));

                                    foreach (var item in lista_cantidad_form)
                                    {
                                        dtCantidades.Rows.Add(item.id_budget_rel_fy_centro, item.id_cuenta_sap, item.mes,
                                                            item.currency_iso, item.cantidad, item.comentario, item.moneda_local_usd);
                                    }

                                    // C. BulkCopy a tabla temporal
                                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                                    {
                                        bulkCopy.DestinationTableName = "#TempBudgetCantidad";
                                        bulkCopy.WriteToServer(dtCantidades);
                                    }

                                    // D. MERGE (Delete, Update, Insert)
                                    // Nota: Se elimina primero la dependencia en budget_rel_conceptos_cantidades
                                    string mergeSqlCantidades = @"
                                        -- 1. Eliminar dependencias de registros que van a ser borrados
                                        DELETE FROM budget_rel_conceptos_cantidades 
                                        WHERE id_budget_cantidad IN (
                                            SELECT T.id 
                                            FROM budget_cantidad T
                                            WHERE T.id_budget_rel_fy_centro IN (SELECT DISTINCT id_budget_rel_fy_centro FROM #TempBudgetCantidad)
                                            AND NOT EXISTS (
                                                SELECT 1 FROM #TempBudgetCantidad S
                                                WHERE T.id_budget_rel_fy_centro = S.id_budget_rel_fy_centro 
                                                  AND T.id_cuenta_sap = S.id_cuenta_sap 
                                                  AND T.mes = S.mes 
                                                  AND T.currency_iso = S.currency_iso 
                                                  AND T.moneda_local_usd = S.moneda_local_usd
                                            )
                                        );

                                        -- 2. Ejecutar MERGE en budget_cantidad
                                        MERGE budget_cantidad AS T
                                        USING #TempBudgetCantidad AS S
                                        ON T.id_budget_rel_fy_centro = S.id_budget_rel_fy_centro 
                                           AND T.id_cuenta_sap = S.id_cuenta_sap 
                                           AND T.mes = S.mes 
                                           AND T.currency_iso = S.currency_iso 
                                           AND T.moneda_local_usd = S.moneda_local_usd
                                        WHEN MATCHED AND (T.cantidad <> S.cantidad OR ISNULL(T.comentario,'') <> ISNULL(S.comentario,'')) THEN
                                            UPDATE SET T.cantidad = S.cantidad, T.comentario = S.comentario
                                        WHEN NOT MATCHED BY TARGET THEN
                                            INSERT (id_budget_rel_fy_centro, id_cuenta_sap, mes, currency_iso, cantidad, comentario, moneda_local_usd)
                                            VALUES (S.id_budget_rel_fy_centro, S.id_cuenta_sap, S.mes, S.currency_iso, S.cantidad, S.comentario, S.moneda_local_usd)
                                        WHEN NOT MATCHED BY SOURCE AND T.id_budget_rel_fy_centro IN (SELECT DISTINCT id_budget_rel_fy_centro FROM #TempBudgetCantidad) THEN
                                            DELETE;
                                    ";

                                    using (SqlCommand cmdMerge = new SqlCommand(mergeSqlCantidades, conn, transaction))
                                    {
                                        cmdMerge.ExecuteNonQuery();
                                    }


                                    // --- BLOQUE 2: PROCESAMIENTO DE COMENTARIOS ---
                                    if (lista_comentarios_form.Count > 0)
                                    {
                                        // A. Crear tabla temporal para Comentarios
                                        string createTempComments = @"
                                            CREATE TABLE #TempComments (
                                                id_budget_rel_fy_centro int,
                                                id_cuenta_sap int,
                                                comentarios varchar(200)
                                            )";
                                        using (SqlCommand cmd = new SqlCommand(createTempComments, conn, transaction)) { cmd.ExecuteNonQuery(); }

                                        // B. Preparar DataTable
                                        System.Data.DataTable dtComments = new System.Data.DataTable();
                                        dtComments.Columns.Add("id_budget_rel_fy_centro", typeof(int));
                                        dtComments.Columns.Add("id_cuenta_sap", typeof(int));
                                        dtComments.Columns.Add("comentarios", typeof(string));

                                        foreach (var c in lista_comentarios_form)
                                        {
                                            dtComments.Rows.Add(c.id_budget_rel_fy_centro, c.id_cuenta_sap, c.comentarios);
                                        }

                                        // C. BulkCopy a tabla temporal
                                        using (SqlBulkCopy bulkComments = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                                        {
                                            bulkComments.DestinationTableName = "#TempComments";
                                            bulkComments.WriteToServer(dtComments);
                                        }

                                        // D. MERGE para Comentarios
                                        string mergeComments = @"
                                            MERGE budget_rel_comentarios AS T
                                            USING #TempComments AS S
                                            ON T.id_budget_rel_fy_centro = S.id_budget_rel_fy_centro 
                                               AND T.id_cuenta_sap = S.id_cuenta_sap
                                            WHEN MATCHED AND ISNULL(T.comentarios,'') <> S.comentarios THEN
                                                UPDATE SET T.comentarios = S.comentarios
                                            WHEN NOT MATCHED BY TARGET THEN
                                                INSERT (id_budget_rel_fy_centro, id_cuenta_sap, comentarios)
                                                VALUES (S.id_budget_rel_fy_centro, S.id_cuenta_sap, S.comentarios)
                                            WHEN NOT MATCHED BY SOURCE AND T.id_budget_rel_fy_centro IN (SELECT DISTINCT id_budget_rel_fy_centro FROM #TempComments) THEN
                                                DELETE;
                                        ";
                                        using (SqlCommand cmdC = new SqlCommand(mergeComments, conn, transaction)) { cmdC.ExecuteNonQuery(); }
                                    }
                                    else
                                    {
                                        // Si el excel viene sin comentarios pero existen en BD para esos centros, hay que borrarlos (lógica de limpieza)
                                        // Usamos los IDs de budget_cantidad procesados para saber qué ambito limpiar
                                        string cleanComments = @"
                                            DELETE FROM budget_rel_comentarios 
                                            WHERE id_budget_rel_fy_centro IN (SELECT DISTINCT id_budget_rel_fy_centro FROM #TempBudgetCantidad)";
                                        using (SqlCommand cmdClean = new SqlCommand(cleanComments, conn, transaction)) { cmdClean.ExecuteNonQuery(); }
                                    }

                                    transaction.Commit();
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    throw;
                                }
                            }
                        }

                        // --- BLOQUE 3: CÁLCULOS POSTERIORES (STORED PROCEDURES) ---
                        // Esto se mantiene fuera de la transacción de carga masiva para evitar bloqueos largos, 
                        // o puede incluirse si es crítico que sea atómico. Aquí se mantiene como en el original.

                        var fiscalYears = db.budget_rel_fy_centro
                            .Where(x => idRels.Contains(x.id))
                            .Select(x => x.id_anio_fiscal)
                            .Distinct()
                            .ToList();

                        foreach (var id_fy in fiscalYears)
                        {
                            for (int mes = 1; mes <= 12; mes++)
                            {
                                // Verificar si existen tipos de cambio antes de ejecutar SP
                                var tc = db.budget_rel_tipo_cambio_fy
                                    .Where(x => x.id_budget_anio_fiscal == id_fy && x.mes == mes && (x.id_tipo_cambio == 1 || x.id_tipo_cambio == 2))
                                    .ToList();

                                if (tc.Count == 2 && tc.All(x => x.cantidad != 0))
                                {
                                    db.Database.ExecuteSqlCommand(
                                        "EXEC usp_ActualizarBudgetCantidad @id_fy, @month",
                                        new SqlParameter("@id_fy", id_fy),
                                        new SqlParameter("@month", mes)
                                    );
                                }
                            }
                        }

                        string mensaje = $"Proceso completado. Registros procesados: {lista_cantidad_form.Count}.";
                        if (noEncontrados > 0)
                            mensaje += $" Cuentas SAP no encontradas: {noEncontrados}.";

                        TempData["Mensaje"] = new MensajesSweetAlert(mensaje, TipoMensajesSweetAlerts.SUCCESS);
                        return RedirectToAction("centros");
                    }
                }
                catch (Exception e)
                {
                    ViewBag.planta = AddFirstItem(new SelectList(db.budget_plantas.Where(x => x.activo == true), "id", "descripcion"), textoPorDefecto: "-- Todos --");
                    ViewBag.centro_costo = AddFirstItem(new SelectList(db.budget_centro_costo.Where(x => x.activo == true), "id", nameof(budget_centro_costo.ConcatCentro)), textoPorDefecto: "-- Todos --");
                    ViewBag.id_fiscal_year = AddFirstItem(new SelectList(db.budget_anio_fiscal.Where(x => x.id != 1), "id", "ConcatAnio"), textoPorDefecto: "-- Seleccionar --");

                    ModelState.AddModelError("", msjError + ": " + e.Message);
                    return View(excelViewModel);
                }
            }
            ViewBag.planta = AddFirstItem(new SelectList(db.budget_plantas.Where(x => x.activo == true), "id", "descripcion"), textoPorDefecto: "-- Todos --");
            ViewBag.centro_costo = AddFirstItem(new SelectList(db.budget_centro_costo.Where(x => x.activo == true), "id", nameof(budget_centro_costo.ConcatCentro)), textoPorDefecto: "-- Todos --");
            ViewBag.id_fiscal_year = AddFirstItem(new SelectList(db.budget_anio_fiscal.Where(x => x.id != 1), "id", "ConcatAnio"), textoPorDefecto: "-- Seleccionar --");

            return View(excelViewModel);
        }



        public ActionResult Reportes(int? planta, string responsable, string centro_costo, int pagina = 1)
        {

            if (TieneRol(TipoRoles.BG_REPORTES))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listado = db.view_valores_fiscal_year
                        .Where(x => (x.id_budget_plant == planta || planta == null)
                            && (x.responsable.Contains(responsable) || String.IsNullOrEmpty(responsable))
                            && (x.cost_center.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                        )
                        .OrderByDescending(x => x.id)
                        .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                        .Take(cantidadRegistrosPorPagina).ToList();


                var totalDeRegistros = db.view_valores_fiscal_year
                        .Where(x => (x.id_budget_plant == planta || planta == null)
                            && (x.responsable.Contains(responsable) || String.IsNullOrEmpty(responsable))
                            && (x.cost_center.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                        )
                        .Count();

                //para paginación
                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["planta"] = planta;
                routeValues["responsable"] = responsable;
                routeValues["centro_costo"] = centro_costo;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                var responsables = db.budget_responsables.ToList();
                var empleados = db.empleados.ToList();

                //obtiene el listado de empleados que son responsables de algún área
                List<empleados> listadoResponsables = empleados.Where(x => responsables.Any(y => y.id_responsable == x.id)).ToList();

                ViewBag.planta = AddFirstItem(new SelectList(db.budget_plantas.Where(x => x.activo == true), "id", "descripcion"), textoPorDefecto: "-- Todos --");
                ViewBag.responsable = AddFirstItem(new SelectList(listadoResponsables, "ConcatNombre", "ConcatNombre"), textoPorDefecto: "-- Todos --");
                ViewBag.id_fiscal_year = AddFirstItem(new SelectList(db.budget_anio_fiscal.Where(x => x.id != 1), "id", "ConcatAnio"), textoPorDefecto: "-- Seleccionar --");
                ViewBag.Paginacion = paginacion;


                // ViewBag.Create = true;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        public ActionResult ExportarConcentrado(int? planta, int? id_fiscal_year, string responsable, string centro_costo, string tipo_reporte)
        {

            if (TieneRol(TipoRoles.BG_REPORTES))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                //

                //obtiene el año fiscal actual (forecast)
                budget_anio_fiscal anio_Fiscal_actual = db.budget_anio_fiscal.Find(id_fiscal_year);
                if (anio_Fiscal_actual == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal anterior (actual)
                budget_anio_fiscal anio_Fiscal_anterior = db.budget_anio_fiscal.FirstOrDefault(x=> x.anio_inicio == anio_Fiscal_actual.anio_inicio-1);
                if (anio_Fiscal_anterior == null)
                    return View("../Error/NotFound");

               

                //obtiene el año fiscal proximo (budget)
                budget_anio_fiscal anio_Fiscal_proximo = db.budget_anio_fiscal.FirstOrDefault(x => x.anio_inicio == anio_Fiscal_actual.anio_inicio + 1);
                if (anio_Fiscal_proximo == null)
                    return View("../Error/NotFound");

                List<view_valores_fiscal_year> listadoAnterior = new List<view_valores_fiscal_year>();
                List<view_valores_fiscal_year> listadoPresente = new List<view_valores_fiscal_year>();
                List<view_valores_fiscal_year> listadoProximo = new List<view_valores_fiscal_year>();

                if (tipo_reporte == "forecast")
                {
                    listadoAnterior = db.view_valores_fiscal_year
                        .Where(x =>
                                (x.id_budget_plant == planta || planta == null)
                            && (x.responsable.Contains(responsable) || String.IsNullOrEmpty(responsable))
                            && (x.cost_center.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                            && (x.id_anio_fiscal == anio_Fiscal_anterior.id)
                        )
                        .ToList();



                    listadoPresente = db.view_valores_fiscal_year
                         .Where(x =>
                                    (x.id_budget_plant == planta || planta == null)
                                && (x.responsable.Contains(responsable) || String.IsNullOrEmpty(responsable))
                                && (x.cost_center.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                                && (x.id_anio_fiscal == anio_Fiscal_actual.id)
                            )
                         .ToList();

                    listadoProximo = db.view_valores_fiscal_year
                        .Where(x =>
                                    (x.id_budget_plant == planta || planta == null)
                                && (x.responsable.Contains(responsable) || String.IsNullOrEmpty(responsable))
                                && (x.cost_center.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                                && (x.id_anio_fiscal == anio_Fiscal_proximo.id)
                            )
                         .ToList();
                }
                else if (tipo_reporte == "budget")
                {
                    var listAnterior = db.view_valores_fiscal_year_budget_historico
                         .Where(x =>
                                 (x.id_budget_plant == planta || planta == null)
                             && (x.responsable.Contains(responsable) || String.IsNullOrEmpty(responsable))
                             && (x.cost_center.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                             && (x.id_anio_fiscal == anio_Fiscal_anterior.id)
                         )
                         .ToList();

                    var listPresente = db.view_valores_fiscal_year_budget_historico
                         .Where(x =>
                                    (x.id_budget_plant == planta || planta == null)
                                && (x.responsable.Contains(responsable) || String.IsNullOrEmpty(responsable))
                                && (x.cost_center.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                                && (x.id_anio_fiscal == anio_Fiscal_actual.id)
                            )
                         .ToList();

                    var listProximo = db.view_valores_fiscal_year_budget_historico
                        .Where(x =>
                                    (x.id_budget_plant == planta || planta == null)
                                && (x.responsable.Contains(responsable) || String.IsNullOrEmpty(responsable))
                                && (x.cost_center.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                                && (x.id_anio_fiscal == anio_Fiscal_proximo.id)
                            )
                         .ToList();

                    //transforma de view_valores_fiscal_year_budget_historico a view_valores_fiscal_year
                    listadoAnterior = ConvertToViewValoresFY(listAnterior);
                    listadoPresente = ConvertToViewValoresFY(listPresente);
                    listadoProximo = ConvertToViewValoresFY(listProximo);
                } else if (tipo_reporte == "forecast_controlling") {
                    var listAnterior = db.view_valores_forecast
                            .Where(x =>
                                    (x.id_budget_plant == planta || planta == null)
                                && (x.responsable.Contains(responsable) || String.IsNullOrEmpty(responsable))
                                && (x.cost_center.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                                && (x.id_anio_fiscal == anio_Fiscal_anterior.id)
                            )
                            .ToList();

                    var listPresente = db.view_valores_forecast
                         .Where(x =>
                                    (x.id_budget_plant == planta || planta == null)
                                && (x.responsable.Contains(responsable) || String.IsNullOrEmpty(responsable))
                                && (x.cost_center.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                                && (x.id_anio_fiscal == anio_Fiscal_actual.id)
                            )
                         .ToList();

                    var listProximo = db.view_valores_forecast
                        .Where(x =>
                                    (x.id_budget_plant == planta || planta == null)
                                && (x.responsable.Contains(responsable) || String.IsNullOrEmpty(responsable))
                                && (x.cost_center.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                                && (x.id_anio_fiscal == anio_Fiscal_proximo.id)
                            )
                         .ToList();

                    //transforma de view_valores_forecast a view_valores_fiscal_year
                    listadoAnterior = ConvertToViewValoresFY(listAnterior);
                    listadoPresente = ConvertToViewValoresFY(listPresente);
                    listadoProximo = ConvertToViewValoresFY(listProximo);
                }



                listadoAnterior = AgregaCuentasSAPConcentrado(listadoAnterior, anio_Fiscal_anterior.id, centro_costo, responsable, planta, soloCuentasActivas: false);
                listadoPresente = AgregaCuentasSAPConcentrado(listadoPresente, anio_Fiscal_actual.id, centro_costo, responsable, planta, soloCuentasActivas: false);
                listadoProximo = AgregaCuentasSAPConcentrado(listadoProximo, anio_Fiscal_proximo.id, centro_costo, responsable, planta, soloCuentasActivas: false);


                byte[] stream = ExcelUtil.GeneraReporteBudgetPorPlanta(listadoAnterior, listadoPresente, listadoProximo, anio_Fiscal_anterior, anio_Fiscal_actual, anio_Fiscal_proximo, tipo_reporte);

                //encuentra la planta
                string fileName = "Concentrado";
                var budget_planta = db.budget_plantas.Find(planta);
                if (budget_planta != null)
                    fileName += "_" + budget_planta.descripcion;
                if (!string.IsNullOrEmpty(responsable))
                    fileName += "_" + responsable;

                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = fileName + "_" + tipo_reporte + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",
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

         public ActionResult GenerarPlantilla(int? planta, int? id_fiscal_year, int? centro_costo)
        {

            if (TieneRol(TipoRoles.BG_REPORTES))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                //

                //obtiene el año fiscal actual (forecast)
                budget_anio_fiscal anio_Fiscal_actual = db.budget_anio_fiscal.Find(id_fiscal_year);
                if (anio_Fiscal_actual == null)
                    return View("../Error/NotFound");
              

                List<view_valores_fiscal_year> listadoPresente = new List<view_valores_fiscal_year>();


                listadoPresente = db.view_valores_fiscal_year
                     .Where(x =>
                                (x.id_budget_plant == planta || planta == null)
                            && (x.id_centro_costo == centro_costo|| centro_costo == null)
                            && (x.id_anio_fiscal == anio_Fiscal_actual.id)
                        )
                     .ToList();

                listadoPresente = AgregaCuentasSAPConcentrado(listadoPresente, anio_Fiscal_actual.id, centro_costo, planta, soloCuentasActivas: false).OrderBy(x=>x.cost_center).ThenBy(x=>x.sap_account).ToList();              

                byte[] stream = ExcelUtil.BudgetPlantillaCargaMasiva( listadoPresente,  anio_Fiscal_actual);

                //encuentra la planta
                string fileName = "Plantilla";
                var budget_planta = db.budget_plantas.Find(planta);
                if (budget_planta != null)
                    fileName += "_" + budget_planta.descripcion;
               if (anio_Fiscal_actual != null)
                    fileName += "_FY - " + anio_Fiscal_actual.descripcion;
              

                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = fileName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",
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


        public ActionResult Exportar(int? id_centro_costo)
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING) || TieneRol(TipoRoles.BG_RESPONSABLE))
            {
                if (id_centro_costo == null)
                {
                    return View("../Error/BadRequest");
                }

                //obtiene centro de costo
                budget_centro_costo centroCosto = db.budget_centro_costo.Find(id_centro_costo);
                if (centroCosto == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal anterior (actual)
                budget_anio_fiscal anio_Fiscal_anterior = ResponsableBudgetController.GetAnioFiscal(DateTime.Now.AddYears(-1));
                if (anio_Fiscal_anterior == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal actual (forecast)
                budget_anio_fiscal anio_Fiscal_actual = ResponsableBudgetController.GetAnioFiscal(DateTime.Now);
                if (anio_Fiscal_actual == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal proximo (budget)
                budget_anio_fiscal anio_Fiscal_proximo = ResponsableBudgetController.GetAnioFiscal(DateTime.Now.AddYears(1));
                if (anio_Fiscal_proximo == null)
                    return View("../Error/NotFound");

                //obtiene los valores para cada cuenta sap
                var valoresListAnioAnterior = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_anterior.id && x.id_centro_costo == centroCosto.id).ToList();
                var valoresListAnioActual = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_actual.id && x.id_centro_costo == centroCosto.id).ToList();
                var valoresListAnioProximo = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_proximo.id && x.id_centro_costo == centroCosto.id).ToList();

                valoresListAnioAnterior = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioAnterior, anio_Fiscal_anterior.id, centroCosto.id, soloCuentasActivas: false).OrderBy(x => x.id_cuenta_sap).ToList();
                valoresListAnioActual = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioActual, anio_Fiscal_actual.id, centroCosto.id, soloCuentasActivas: false).OrderBy(x => x.id_cuenta_sap).ToList();
                valoresListAnioProximo = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioProximo, anio_Fiscal_proximo.id, centroCosto.id, soloCuentasActivas: false).OrderBy(x => x.id_cuenta_sap).ToList();


                byte[] stream = ExcelUtil.GeneraReporteBudgetPorCentroCosto(centroCosto, valoresListAnioAnterior, valoresListAnioActual, valoresListAnioProximo, anio_Fiscal_anterior, anio_Fiscal_actual, anio_Fiscal_proximo);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Cost_Center_" + centroCosto.num_centro_costo + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",
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


        [NonAction]
        public static List<view_valores_fiscal_year> AgregaCuentasSAPConcentrado(List<view_valores_fiscal_year> listValores, int id_anio_fiscal, string centro_costo, string responsable, int? planta, bool soloCuentasActivas = false)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            List<budget_cuenta_sap> listCuentas = new List<budget_cuenta_sap>();

            if (soloCuentasActivas) //carga sólo las cuentas activas
                listCuentas = db.budget_cuenta_sap.Where(x => x.activo == true).ToList();
            else //carga todas las cuentas
                listCuentas = db.budget_cuenta_sap.ToList();

            string responsableTrim = responsable.Replace(" ", String.Empty);

            //obtiene todos los centros de costo
            var centros = db.budget_centro_costo.Where(x => (x.budget_departamentos.id_budget_planta == planta || planta == null)
                            && (x.num_centro_costo.Contains(centro_costo) || String.IsNullOrEmpty(centro_costo))
                            && x.budget_responsables.Any(y => ((y.empleados.nombre) + (y.empleados.apellido1) + (y.empleados.apellido2)).Replace(" ", String.Empty).Contains(responsableTrim))
                            );

            // centros = centros.Where 

            //Agrega cuentas vacias
            List<view_valores_fiscal_year> listViewCuentas = new List<view_valores_fiscal_year>();

            foreach (budget_cuenta_sap cuenta in listCuentas)
            {

                foreach (budget_centro_costo c in centros)
                {
                    string resp = (string.Join("/", c.budget_responsables.Select(x => x.empleados.ConcatNombre).ToArray()));

                    //agragega un objeto de tipo view_valores_anio_fiscal por cada cuenta existente
                    listViewCuentas.Add(new view_valores_fiscal_year
                    {
                        id_anio_fiscal = id_anio_fiscal,
                        id_centro_costo = c.id,
                        cost_center = c.num_centro_costo,
                        id_cuenta_sap = cuenta.id,
                        sap_account = cuenta.sap_account,
                        name = cuenta.name,
                        mapping = cuenta.budget_mapping.descripcion,
                        mapping_bridge = cuenta.budget_mapping.budget_mapping_bridge.descripcion,
                        currency_iso = "USD",
                        responsable = resp,
                        codigo_sap = c.budget_departamentos.budget_plantas.codigo_sap,
                        department = c.descripcion,
                        class_1 = c.class_1,
                        class_2 = c.class_2,
                    });

                }
            }
            //obtiene las cuentas que no se encuentran en el listado original
            List<view_valores_fiscal_year> listDiferencias = listViewCuentas.Except(listValores).ToList();

            //suba la lista original con la lista de excepciones
            listValores.AddRange(listDiferencias);

            return listValores.OrderBy(x => x.cost_center).ThenBy(x => x.id_cuenta_sap).ThenBy(x => x.id_budget_plant).ToList();
        }
        
        [NonAction]
        public static List<view_valores_fiscal_year> AgregaCuentasSAPConcentrado(List<view_valores_fiscal_year> listValores, int id_anio_fiscal, int? centro_costo, int? planta, bool soloCuentasActivas = false)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            List<budget_cuenta_sap> listCuentas = new List<budget_cuenta_sap>();

            if (soloCuentasActivas) //carga sólo las cuentas activas
                listCuentas = db.budget_cuenta_sap.Where(x => x.activo == true).ToList();
            else //carga todas las cuentas
                listCuentas = db.budget_cuenta_sap.ToList();
           

            //obtiene todos los centros de costo
            var centros = db.budget_centro_costo.Where(x => (x.budget_departamentos.id_budget_planta == planta || planta == null)
                             && (x.id == centro_costo || centro_costo == null)
                            );

            // centros = centros.Where 

            //Agrega cuentas vacias
            List<view_valores_fiscal_year> listViewCuentas = new List<view_valores_fiscal_year>();

            foreach (budget_cuenta_sap cuenta in listCuentas)
            {

                foreach (budget_centro_costo c in centros)
                {
                    //string resp =c.budget_responsables.Any() ? c.budget_responsables.FirstOrDefault().empleados.ConcatNombre: string.Empty;

                    string resp = (string.Join("/", c.budget_responsables.Select(x => x.empleados.ConcatNombre).ToArray()));

                    //agragega un objeto de tipo view_valores_anio_fiscal por cada cuenta existente
                    listViewCuentas.Add(new view_valores_fiscal_year
                    {
                        id_anio_fiscal = id_anio_fiscal,
                        id_centro_costo = c.id,
                        cost_center = c.num_centro_costo,
                        id_cuenta_sap = cuenta.id,
                        sap_account = cuenta.sap_account,
                        name = cuenta.name,
                        mapping = cuenta.budget_mapping.descripcion,
                        mapping_bridge = cuenta.budget_mapping.budget_mapping_bridge.descripcion,
                        currency_iso = "USD",
                        responsable = resp,
                        codigo_sap = c.budget_departamentos.budget_plantas.codigo_sap,
                        department = c.descripcion,
                        class_1 = c.class_1,
                        class_2 = c.class_2,
                    });

                }
            }
            //obtiene las cuentas que no se encuentran en el listado original
            List<view_valores_fiscal_year> listDiferencias = listViewCuentas.Except(listValores).ToList();

            //suba la lista original con la lista de excepciones
            listValores.AddRange(listDiferencias);

            return listValores.OrderBy(x => x.cost_center).ThenBy(x => x.id_cuenta_sap).ThenBy(x => x.id_budget_plant).ToList();
        }

        public JsonResult CambiaEstatusRelFYCentro(int? idRelFYCentro, bool activo = true)
        {
            var list = new object[1];
            list[0] = new { Message = "Error: Se recibe solicitud, pero hubo un error" };

            //obtiene el id_rel_centro_costo del forecast p
            budget_rel_fy_centro rel_centro_forecast = db.budget_rel_fy_centro.Find(idRelFYCentro);

            if (rel_centro_forecast == null)
            {
                list[0] = new { Status = "Error", Message = "No existe Rel entre año fiscal actual y centro de costo de tipo FORECAST" };
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            try
            {
                rel_centro_forecast.estatus = activo;
                db.Entry(rel_centro_forecast).State = EntityState.Modified;
                db.SaveChanges();
                list[0] = new { Status = "OK", Message = "Se modificó correctamente el registro." };
            }
            catch (Exception e)
            {
                list[0] = new { Status = "Error", Message = "Error en BD: " + e.Message };
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        protected List<view_valores_fiscal_year> ConvertToViewValoresFY(List<view_valores_fiscal_year_budget_historico> listado)
        {
            List<view_valores_fiscal_year> result = new List<view_valores_fiscal_year>();

            foreach (view_valores_fiscal_year_budget_historico item in listado)
            {
                result.Add(new view_valores_fiscal_year
                {
                    id = item.id,
                    id_budget_rel_fy_centro = item.id_budget_rel_fy_centro,
                    id_anio_fiscal = item.id_anio_fiscal,
                    id_centro_costo = item.id_centro_costo,
                    id_cuenta_sap = item.id_cuenta_sap,
                    sap_account = item.sap_account,
                    name = item.name,
                    cost_center = item.cost_center,
                    department = item.department,
                    class_1 = item.class_1,
                    class_2 = item.class_2,
                    responsable = item.responsable,
                    codigo_sap = item.codigo_sap,
                    id_budget_plant = item.id_budget_plant,
                    mapping = item.mapping,
                    mapping_bridge = item.mapping_bridge,
                    currency_iso = item.currency_iso,
                    //USD
                    Enero = item.Enero,
                    Febrero = item.Febrero,
                    Marzo = item.Marzo,
                    Abril = item.Abril,
                    Mayo = item.Mayo,
                    Junio = item.Junio,
                    Julio = item.Julio,
                    Agosto = item.Agosto,
                    Septiembre = item.Septiembre,
                    Octubre = item.Octubre,
                    Noviembre = item.Noviembre,
                    Diciembre = item.Diciembre,   
                    //MXN
                    Enero_MXN = item.Enero_MXN,
                    Febrero_MXN = item.Febrero_MXN,
                    Marzo_MXN = item.Marzo_MXN,
                    Abril_MXN = item.Abril_MXN,
                    Mayo_MXN = item.Mayo_MXN,
                    Junio_MXN = item.Junio_MXN,
                    Julio_MXN = item.Julio_MXN,
                    Agosto_MXN = item.Agosto_MXN,
                    Septiembre_MXN = item.Septiembre_MXN,
                    Octubre_MXN = item.Octubre_MXN,
                    Noviembre_MXN = item.Noviembre_MXN,
                    Diciembre_MXN = item.Diciembre_MXN,
                    //EUR
                    Enero_EUR = item.Enero_EUR,
                    Febrero_EUR = item.Febrero_EUR,
                    Marzo_EUR = item.Marzo_EUR,
                    Abril_EUR = item.Abril_EUR,
                    Mayo_EUR = item.Mayo_EUR,
                    Junio_EUR = item.Junio_EUR,
                    Julio_EUR = item.Julio_EUR,
                    Agosto_EUR = item.Agosto_EUR,
                    Septiembre_EUR = item.Septiembre_EUR,
                    Octubre_EUR = item.Octubre_EUR,
                    Noviembre_EUR = item.Noviembre_EUR,
                    Diciembre_EUR = item.Diciembre_EUR,
                    //LOCAL
                    Enero_USD_LOCAL = item.Enero_USD_LOCAL,
                    Febrero_USD_LOCAL = item.Febrero_USD_LOCAL,
                    Marzo_USD_LOCAL = item.Marzo_USD_LOCAL,
                    Abril_USD_LOCAL = item.Abril_USD_LOCAL,
                    Mayo_USD_LOCAL = item.Mayo_USD_LOCAL,
                    Junio_USD_LOCAL = item.Junio_USD_LOCAL,
                    Julio_USD_LOCAL = item.Julio_USD_LOCAL,
                    Agosto_USD_LOCAL = item.Agosto_USD_LOCAL,
                    Septiembre_USD_LOCAL = item.Septiembre_USD_LOCAL,
                    Octubre_USD_LOCAL = item.Octubre_USD_LOCAL,
                    Noviembre_USD_LOCAL = item.Noviembre_USD_LOCAL,
                    Diciembre_USD_LOCAL = item.Diciembre_USD_LOCAL,

                });
            }

            return result;
        }
         protected List<view_valores_fiscal_year> ConvertToViewValoresFY(List<view_valores_forecast> listado)
        {
            List<view_valores_fiscal_year> result = new List<view_valores_fiscal_year>();

            foreach (view_valores_forecast item in listado)
            {
                result.Add(new view_valores_fiscal_year
                {
                    id = (long)item.id,
                    id_budget_rel_fy_centro = item.id_budget_rel_fy_centro,
                    id_anio_fiscal = item.id_anio_fiscal,
                    id_centro_costo = item.id_centro_costo,
                    id_cuenta_sap = item.id_cuenta_sap,
                    sap_account = item.sap_account,
                    name = item.name,
                    cost_center = item.cost_center,
                    department = item.department,
                    class_1 = item.class_1,
                    class_2 = item.class_2,
                    responsable = item.responsable,
                    codigo_sap = item.codigo_sap,
                    id_budget_plant = item.id_budget_plant,
                    mapping = item.mapping,
                    mapping_bridge = item.mapping_bridge,
                    currency_iso = item.currency_iso,
                    //USD
                    Enero = item.Enero,
                    Febrero = item.Febrero,
                    Marzo = item.Marzo,
                    Abril = item.Abril,
                    Mayo = item.Mayo,
                    Junio = item.Junio,
                    Julio = item.Julio,
                    Agosto = item.Agosto,
                    Septiembre = item.Septiembre,
                    Octubre = item.Octubre,
                    Noviembre = item.Noviembre,
                    Diciembre = item.Diciembre,   
                    //MXN
                    Enero_MXN = item.Enero_MXN,
                    Febrero_MXN = item.Febrero_MXN,
                    Marzo_MXN = item.Marzo_MXN,
                    Abril_MXN = item.Abril_MXN,
                    Mayo_MXN = item.Mayo_MXN,
                    Junio_MXN = item.Junio_MXN,
                    Julio_MXN = item.Julio_MXN,
                    Agosto_MXN = item.Agosto_MXN,
                    Septiembre_MXN = item.Septiembre_MXN,
                    Octubre_MXN = item.Octubre_MXN,
                    Noviembre_MXN = item.Noviembre_MXN,
                    Diciembre_MXN = item.Diciembre_MXN,
                    //EUR
                    Enero_EUR = item.Enero_EUR,
                    Febrero_EUR = item.Febrero_EUR,
                    Marzo_EUR = item.Marzo_EUR,
                    Abril_EUR = item.Abril_EUR,
                    Mayo_EUR = item.Mayo_EUR,
                    Junio_EUR = item.Junio_EUR,
                    Julio_EUR = item.Julio_EUR,
                    Agosto_EUR = item.Agosto_EUR,
                    Septiembre_EUR = item.Septiembre_EUR,
                    Octubre_EUR = item.Octubre_EUR,
                    Noviembre_EUR = item.Noviembre_EUR,
                    Diciembre_EUR = item.Diciembre_EUR,
                    //LOCAL
                    Enero_USD_LOCAL = item.Enero_USD_LOCAL,
                    Febrero_USD_LOCAL = item.Febrero_USD_LOCAL,
                    Marzo_USD_LOCAL = item.Marzo_USD_LOCAL,
                    Abril_USD_LOCAL = item.Abril_USD_LOCAL,
                    Mayo_USD_LOCAL = item.Mayo_USD_LOCAL,
                    Junio_USD_LOCAL = item.Junio_USD_LOCAL,
                    Julio_USD_LOCAL = item.Julio_USD_LOCAL,
                    Agosto_USD_LOCAL = item.Agosto_USD_LOCAL,
                    Septiembre_USD_LOCAL = item.Septiembre_USD_LOCAL,
                    Octubre_USD_LOCAL = item.Octubre_USD_LOCAL,
                    Noviembre_USD_LOCAL = item.Noviembre_USD_LOCAL,
                    Diciembre_USD_LOCAL = item.Diciembre_USD_LOCAL,

                });
            }

            return result;
        }

        public JsonResult GetCentrosCostoByPlanta(int? plantaId)
        {
            // Incluir las relaciones necesarias para poder acceder a budget_departamentos y budget_plantas
            var query = db.budget_centro_costo
                          .Include("budget_departamentos.budget_plantas")
                          .AsQueryable();

            // Si se proporcionó un id de planta válido, filtrar los centros de costo correspondientes
            if (plantaId.HasValue && plantaId.Value > 0)
            {
                query = query.Where(c => c.budget_departamentos.id_budget_planta == plantaId.Value);
            }

            // Se ejecuta la consulta y se utiliza la propiedad ConcatCentro para formar el texto
            var centros = query
                .ToList() // Se trae a memoria para poder evaluar la propiedad NotMapped
                .Select(c => new
                {
                    Value = c.id, // Ajusta según el nombre de la propiedad identificadora
                    Text = c.ConcatCentro
                })
                .OrderBy(x => x.Text)
                .ToList();

            return Json(centros, JsonRequestBehavior.AllowGet);
        }
    }
}
