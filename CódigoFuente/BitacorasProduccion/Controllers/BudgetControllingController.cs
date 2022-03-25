using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
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
                        .Where(x => (x.Area.plantaClave == planta || planta == null)
                            && (x.id_responsable == responsable || responsable == null)
                            && (x.num_centro_costo.Contains(centro_costo) ||String.IsNullOrEmpty(centro_costo))
                        )
                        .OrderByDescending(x => x.id)
                        .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                        .Take(cantidadRegistrosPorPagina).ToList();


                var totalDeRegistros = db.budget_centro_costo
                        .Where(x => (x.Area.plantaClave == planta || planta == null)
                            && (x.id_responsable == responsable || responsable == null)
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

                var centros = db.budget_centro_costo.ToList();
                var empleados = db.empleados.ToList();

                //obtiene el listado de empleados que son responsables de algún área
                List<empleados> listadoResponsables = empleados.Where(x => centros.Any(y => y.id_responsable == x.id)).ToList();


                ViewBag.planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Todos --");
                ViewBag.responsable = AddFirstItem(new SelectList(listadoResponsables, "id", "ConcatNombre"), textoPorDefecto: "-- Todos --");
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
                    
                    //el archivo es válido
                    List<budget_cantidad> lista = UtilExcel.LeeActual(centro,  ref estructuraValida, ref msjError, ref noEncontrados);


                    if (!estructuraValida)
                    {
                        msjError = "No cumple con la estructura válida. "+msjError;
                        throw new Exception(msjError);
                    }
                    else if (lista.Count>0)//cumple con la estructura válida
                    {
                        //obtiene los ids de rel_fy_centro
                        var idRels = lista.Select(x => x.id_budget_rel_fy_centro).Distinct();

                        //1.- identificar cuales son update, create, delete y sin cambios
                        //obtiene los valores actuales en BD
                        List<budget_cantidad> valoresEnBD = db.budget_cantidad.Where(x => idRels.Contains( x.id_budget_rel_fy_centro)).ToList();

                        
                        //DE valores Diferentes identificar cuales son create, update y delete
                        List<budget_cantidad> valoresCreate = lista.Where(a => !valoresEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes) && a.cantidad != 0).ToList();

                        //DE valores Diferentes identificar cuales son create, update y delete
                        List<budget_cantidad> valoresUpdate = lista.Where(a => valoresEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes && a.cantidad != a1.cantidad && a.cantidad!=0)).ToList();
                        //agrega el id correspondiente
                        valoresUpdate = valoresUpdate.Select(x =>
                        {
                            x.id = (from v in valoresEnBD
                                    where v.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro && v.id_cuenta_sap == x.id_cuenta_sap && v.mes == x.mes
                                    select v.id).FirstOrDefault(); return x;
                        }).ToList();

                        //DE valores Diferentes identificar cuales son create, update y delete
                        List<budget_cantidad> valoresDelete = valoresEnBD.Where(a => !lista.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes )).ToList();
                        valoresDelete.AddRange(lista.Where(a => valoresEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes && a.cantidad == 0)).ToList());
                        
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
                            msjError = "Error: " + msjError;
                            throw new Exception(msjError);
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
                            msjError = "Error: " + msjError;
                            throw new Exception(msjError);
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
                            msjError = "Error: " + msjError;
                            throw new Exception(msjError);
                        }

                        if (noEncontrados==0)
                            TempData["Mensaje"] = new MensajesSweetAlert("Se ha actualizado correctamente." , TipoMensajesSweetAlerts.INFO);
                        else
                            TempData["Mensaje"] = new MensajesSweetAlert("Algunas cuentas SAP no se encontraron: "+noEncontrados, TipoMensajesSweetAlerts.WARNING);

                        return RedirectToAction("Centros");
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Error: "+e.Message);
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

        //// GET: BudgetControlling/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    budget_centro_costo budget_centro_costo = db.budget_centro_costo.Find(id);
        //    if (budget_centro_costo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(budget_centro_costo);
        //}

        //// GET: BudgetControlling/Create
        //public ActionResult Create()
        //{
        //    ViewBag.id_area = new SelectList(db.Area, "clave", "descripcion");
        //    ViewBag.id_responsable = new SelectList(db.empleados, "id", "numeroEmpleado");
        //    return View();
        //}

        //// POST: BudgetControlling/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id,id_area,id_responsable,num_centro_costo")] budget_centro_costo budget_centro_costo)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.budget_centro_costo.Add(budget_centro_costo);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.id_area = new SelectList(db.Area, "clave", "descripcion", budget_centro_costo.id_area);
        //    ViewBag.id_responsable = new SelectList(db.empleados, "id", "numeroEmpleado", budget_centro_costo.id_responsable);
        //    return View(budget_centro_costo);
        //}

        //// GET: BudgetControlling/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    budget_centro_costo budget_centro_costo = db.budget_centro_costo.Find(id);
        //    if (budget_centro_costo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.id_area = new SelectList(db.Area, "clave", "descripcion", budget_centro_costo.id_area);
        //    ViewBag.id_responsable = new SelectList(db.empleados, "id", "numeroEmpleado", budget_centro_costo.id_responsable);
        //    return View(budget_centro_costo);
        //}

        //// POST: BudgetControlling/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "id,id_area,id_responsable,num_centro_costo")] budget_centro_costo budget_centro_costo)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(budget_centro_costo).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.id_area = new SelectList(db.Area, "clave", "descripcion", budget_centro_costo.id_area);
        //    ViewBag.id_responsable = new SelectList(db.empleados, "id", "numeroEmpleado", budget_centro_costo.id_responsable);
        //    return View(budget_centro_costo);
        //}

        //// GET: BudgetControlling/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    budget_centro_costo budget_centro_costo = db.budget_centro_costo.Find(id);
        //    if (budget_centro_costo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(budget_centro_costo);
        //}

        //// POST: BudgetControlling/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    budget_centro_costo budget_centro_costo = db.budget_centro_costo.Find(id);
        //    db.budget_centro_costo.Remove(budget_centro_costo);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        public ActionResult Exportar(int? id_centro_costo)
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
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

                valoresListAnioAnterior = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioAnterior, anio_Fiscal_anterior.id, centroCosto.id, soloCuentasActivas:false).OrderBy(x => x.id_cuenta_sap).ToList();
                valoresListAnioActual = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioActual, anio_Fiscal_actual.id, centroCosto.id, soloCuentasActivas:false).OrderBy(x => x.id_cuenta_sap).ToList();
                valoresListAnioProximo = ResponsableBudgetController.AgregaCuentasSAPFaltantes(valoresListAnioProximo, anio_Fiscal_proximo.id, centroCosto.id, soloCuentasActivas: false).OrderBy(x => x.id_cuenta_sap).ToList();


                byte[] stream = ExcelUtil.GeneraReporteBudgetPorCentroCosto(centroCosto,valoresListAnioAnterior, valoresListAnioActual, valoresListAnioProximo, anio_Fiscal_anterior, anio_Fiscal_actual, anio_Fiscal_proximo);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Cost_Center_"+centroCosto.num_centro_costo+"_"+ DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",
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


        public JsonResult CambiaEstatusRelFYCentro(int? idRelFYCentro, bool activo = true )
        {
            var list = new object[1];
            list[0] = new { Message = "Error: Se recibe solicitud, pero hubo un error" };           

            //obtiene el id_rel_centro_costo del forecast p
            budget_rel_fy_centro rel_centro_forecast = db.budget_rel_fy_centro.Find(idRelFYCentro);

            if (rel_centro_forecast==null) {
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
            catch (Exception e) {
                list[0] = new { Status = "Error", Message = "Error en BD: "+e.Message };
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
    }
}
