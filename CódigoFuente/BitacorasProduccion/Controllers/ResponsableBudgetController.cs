using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class ResponsableBudgetController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        //// GET: ResponsableBudget/Actual
        //public ActionResult Actual()
        //{
        //    if (TieneRol(TipoRoles.BG_RESPONSABLE))
        //    {
        //        //obtiene el usuario logeado
        //        empleados empleado = obtieneEmpleadoLogeado();

        //        var centros = db.budget_centro_costo.Where(x => x.id_responsable == empleado.id);
        //        return View(centros.ToList());
        //    }
        //    else
        //    {
        //        return View("../Home/ErrorPermisos");
        //    }

        //}
        //// GET: ResponsableBudget/DetailsActual
        //public ActionResult DetailsActual(int? id)
        //{

        //    if (TieneRol(TipoRoles.BG_RESPONSABLE))
        //    {
        //        if (id == null)
        //        {
        //            return View("../Error/BadRequest");
        //        }

        //        //obtiene centro de costo
        //        budget_centro_costo centroCosto = db.budget_centro_costo.Find(id);
        //        if (centroCosto == null)
        //            return View("../Error/NotFound");

        //        //obtiene el año fiscal anterior
        //        budget_anio_fiscal anio_Fiscal_anterior = GetAnioFiscal(DateTime.Now.AddYears(-1));
        //        if (anio_Fiscal_anterior == null)
        //            return View("../Error/NotFound");

        //        //mezcla la lista de actual con forecast y el listado de cuentas
        //        var valoresListAnioAnterior = db.view_valores_anio_fiscal.Where(x => x.id_anio_fiscal == anio_Fiscal_anterior.id && x.id_centro_costo == centroCosto.id).ToList();

        //        valoresListAnioAnterior = AgregaCuentasSAP(valoresListAnioAnterior, anio_Fiscal_anterior.id, centroCosto.id);

        //        ViewBag.centroCosto = centroCosto;
        //        ViewBag.anio_Fiscal_anterior = anio_Fiscal_anterior;
        //        return View(valoresListAnioAnterior);

        //    }
        //    else
        //    {
        //        return View("../Home/ErrorPermisos");
        //    }

        //}

        //// GET: ResponsableBudget/Forecast
        //public ActionResult Forecast()
        //{
        //    if (TieneRol(TipoRoles.BG_RESPONSABLE))
        //    {
        //        //mensaje en caso de crear, editar, etc
        //        if (TempData["Mensaje"] != null)
        //        {
        //            ViewBag.MensajeAlert = TempData["Mensaje"];
        //        }

        //        //obtiene el usuario logeado
        //        empleados empleado = obtieneEmpleadoLogeado();

        //        var centros = db.budget_centro_costo.Where(x => x.id_responsable == empleado.id);
        //        return View(centros.ToList());
        //    }
        //    else
        //    {
        //        return View("../Home/ErrorPermisos");
        //    }

        //}

        //// GET: ResponsableBudget/EditForecast
        //public ActionResult EditForecast(int? id)
        //{

        //    if (TieneRol(TipoRoles.BG_RESPONSABLE))
        //    {
        //        if (id == null)
        //        {
        //            return View("../Error/BadRequest");
        //        }

        //        //obtiene centro de costo
        //        budget_centro_costo centroCosto = db.budget_centro_costo.Find(id);
        //        if (centroCosto == null)
        //            return View("../Error/NotFound");

        //        //obtiene el año fiscal anterior (actual)
        //        budget_anio_fiscal anio_Fiscal_anterior = GetAnioFiscal(DateTime.Now.AddYears(-1));
        //        if (anio_Fiscal_anterior == null)
        //            return View("../Error/NotFound");

        //        //obtiene el año fiscal actual (forecast)
        //        budget_anio_fiscal anio_Fiscal_actual = GetAnioFiscal(DateTime.Now);
        //        if (anio_Fiscal_actual == null)
        //            return View("../Error/NotFound");

        //        //obtiene el año fiscal proximo (budget)
        //        budget_anio_fiscal anio_Fiscal_proximo = GetAnioFiscal(DateTime.Now.AddYears(1));
        //        if (anio_Fiscal_proximo == null)
        //            return View("../Error/NotFound");

        //        //obtiene el id_rel_centro_costo del forecast
        //        budget_rel_anio_fiscal_centro rel_centro_forecast = db.budget_rel_anio_fiscal_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_actual.id && x.tipo == BG_Status.FORECAST).FirstOrDefault();
        //        if (rel_centro_forecast == null)
        //        {
        //            //si no existe crea el rel anio forecast
        //            rel_centro_forecast = new budget_rel_anio_fiscal_centro
        //            {
        //                id_anio_fiscal = anio_Fiscal_actual.id,
        //                id_centro_costo = centroCosto.id,
        //                tipo = BG_Status.FORECAST,
        //                estatus = true //activado por defecto
        //            };

        //            db.budget_rel_anio_fiscal_centro.Add(rel_centro_forecast);
        //            //guarda en base de datos el centro creado
        //            db.SaveChanges();
        //        }


        //        //obtiene los valores para cada cuenta sap
        //        var valoresListAnioAnterior = db.view_valores_anio_fiscal.Where(x => x.id_anio_fiscal == anio_Fiscal_anterior.id && x.id_centro_costo == centroCosto.id).ToList();
        //        var valoresListAnioActual = db.view_valores_anio_fiscal.Where(x => x.id_anio_fiscal == anio_Fiscal_actual.id && x.id_centro_costo == centroCosto.id).ToList();
        //        var valoresListAnioProximo = db.view_valores_anio_fiscal.Where(x => x.id_anio_fiscal == anio_Fiscal_proximo.id && x.id_centro_costo == centroCosto.id).ToList();

        //        valoresListAnioAnterior = AgregaCuentasSAP(valoresListAnioAnterior, anio_Fiscal_anterior.id, centroCosto.id);
        //        valoresListAnioActual = AgregaCuentasSAP(valoresListAnioActual, anio_Fiscal_actual.id, centroCosto.id).OrderBy(x => x.id_cuenta_sap).ToList();
        //        valoresListAnioProximo = AgregaCuentasSAP(valoresListAnioProximo, anio_Fiscal_proximo.id, centroCosto.id);


        //        ViewBag.centroCosto = centroCosto;
        //        ViewBag.anio_Fiscal_anterior = anio_Fiscal_anterior;
        //        ViewBag.anio_Fiscal_actual = anio_Fiscal_actual;
        //        ViewBag.anio_Fiscal_proximo = anio_Fiscal_proximo;
        //        ViewBag.ValoresActual = valoresListAnioAnterior;
        //        ViewBag.ValoresActualForecast = valoresListAnioActual;
        //        ViewBag.ValoresBudget = valoresListAnioProximo;

        //        return View(rel_centro_forecast);

        //    }
        //    else
        //    {
        //        return View("../Home/ErrorPermisos");
        //    }

        //}

        //// POST: ResponsableBudget/EditForecast
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditForecast(budget_rel_anio_fiscal_centro model, FormCollection form)
        //{
        //    //crea objetos apartir del form collection

        //    //1.-Obtiene los keys de tipo cantidad
        //    List<string> keysCantidades = form.AllKeys.Where(x => x.ToUpper().Contains("CANTIDAD") == true).ToList();

        //    //2.- Recorre y crea un objeto de tipo budget_valores
        //    List<budget_valores> valores = new List<budget_valores>();

        //    foreach (var keyC in keysCantidades)
        //    {
        //        Match m = Regex.Match(keyC, "(\\d+)");
        //        string num = string.Empty;

        //        //extrae el numero de key
        //        if (m.Success)
        //        {
        //            num = m.Value;


        //            int id_cuenta_sap = 0;
        //            int mes = 0;
        //            decimal cantidad = 0;

        //            //obtiene el valor de la cuenta sap
        //            bool success_id_cuenta_sap = int.TryParse(form["budget_valores[" + num + "].id_cuenta_sap"], out id_cuenta_sap);
        //            bool success_mes = int.TryParse(form["budget_valores[" + num + "].mes"], out mes);
        //            bool succes_cantidad = decimal.TryParse(form["budget_valores[" + num + "].cantidad"], out cantidad);
        //            string currency = "USD";  //currency por defecto

        //            //verifica que no hubo errores al covertir los datos
        //            if (success_id_cuenta_sap && success_mes)
        //            {
        //                valores.Add(new budget_valores
        //                {
        //                    id_rel_anio_centro = model.id,
        //                    id_cuenta_sap = id_cuenta_sap,
        //                    mes = mes,
        //                    currency_iso = currency,
        //                    cantidad = cantidad,
        //                });
        //            }
        //            //else {
        //            //    int i = 0;
        //            //}

        //        }
        //    }

        //    //lista para valores de meses segun la fecha actual
        //    List<int> mesesPendientes = new List<int>();
        //    DateTime fecha = DateTime.Now;

        //    for (int i = fecha.Month; i >= 10 && i <= 12; i++)
        //        mesesPendientes.Add(i);

        //    if (fecha.Month >= 10)
        //    {
        //        mesesPendientes.AddRange(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        //    }

        //    for (int i = fecha.Month; i <= 9; i++)
        //        mesesPendientes.Add(i);

        //    //3.- identificar cuales son update, create, delete y sin cambios

        //    //obtiene los valores actuales en BD
        //    List<budget_valores> valoresEnBD = db.budget_valores.Where(x => x.id_rel_anio_centro == model.id && mesesPendientes.Contains(x.mes)).ToList();

        //    //obtiene valores sin cambios
        //    List<budget_valores> valoresSinCambio = valores.Where(x => valoresEnBD.Any(y => y.id_rel_anio_centro == x.id_rel_anio_centro && y.id_cuenta_sap == x.id_cuenta_sap && y.mes == x.mes && y.cantidad == x.cantidad)).ToList();

        //    //obtiene valores con cambios o nuevos
        //    List<budget_valores> valoresDiferentes = valores.Except(valoresSinCambio).ToList();

        //    //DE valores Diferentes identificar cuales son create, update y delete
        //    List<budget_valores> valoresCreate = valoresDiferentes.Where(a => !valoresEnBD.Any(a1 => a1.id_rel_anio_centro == a.id_rel_anio_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes) && a.cantidad != 0).ToList();

        //    //DE valores Diferentes identificar cuales son create, update y delete
        //    List<budget_valores> valoresUpdate = valoresDiferentes.Where(a => valoresEnBD.Any(a1 => a1.id_rel_anio_centro == a.id_rel_anio_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes && a.cantidad != 0)).ToList();
        //    //agrega el id correspondiente
        //    valoresUpdate = valoresUpdate.Select(x =>
        //    {
        //        x.id = (from v in valoresEnBD
        //                where v.id_rel_anio_centro == x.id_rel_anio_centro && v.id_cuenta_sap == x.id_cuenta_sap && v.mes == x.mes
        //                select v.id).FirstOrDefault(); return x;
        //    }).ToList();

        //    //DE valores Diferentes identificar cuales son create, update y delete
        //    List<budget_valores> valoresDelete = valoresDiferentes.Where(a => valoresEnBD.Any(a1 => a1.id_rel_anio_centro == a.id_rel_anio_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes && a.cantidad == 0)).ToList();
        //    //agrega el id  correspondiente
        //    valoresDelete = valoresDelete.Select(x =>
        //    {
        //        x.id = (from v in valoresEnBD
        //                where v.id_rel_anio_centro == x.id_rel_anio_centro && v.id_cuenta_sap == x.id_cuenta_sap && v.mes == x.mes
        //                select v.id).FirstOrDefault(); return x;
        //    }).ToList();

        //    //crea los nuevos registros
        //    try
        //    {
        //        db.budget_valores.AddRange(valoresCreate);
        //        db.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
        //    }

        //    //modifica los registros actuales
        //    try
        //    {
        //        foreach (budget_valores item in valoresUpdate)
        //        {
        //            //obtiene el elemento de BD
        //            budget_valores objeto = valoresEnBD.FirstOrDefault(x => x.id == item.id);

        //            if (objeto != null)
        //            {
        //                db.Entry(objeto).CurrentValues.SetValues(item);

        //            }

        //        }
        //        db.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
        //    }

        //    //Elimina los elemntos necesarios 
        //    try
        //    {
        //        foreach (budget_valores item in valoresDelete)
        //        {
        //            //obtiene el elemento de BD
        //            budget_valores objeto = valoresEnBD.FirstOrDefault(x => x.id == item.id);

        //            if (objeto != null)
        //            {
        //                db.budget_valores.Remove(objeto);
        //            }

        //        }
        //        db.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
        //    }

        //    TempData["Mensaje"] = new MensajesSweetAlert("Se ha modificado correctamente", TipoMensajesSweetAlerts.SUCCESS);

        //    return RedirectToAction("Forecast");
        //}

        //// GET: ResponsableBudget/Budget
        //public ActionResult Budget()
        //{
        //    if (TieneRol(TipoRoles.BG_RESPONSABLE))
        //    {
        //        //mensaje en caso de crear, editar, etc
        //        if (TempData["Mensaje"] != null)
        //        {
        //            ViewBag.MensajeAlert = TempData["Mensaje"];
        //        }

        //        //obtiene el usuario logeado
        //        empleados empleado = obtieneEmpleadoLogeado();

        //        var centros = db.budget_centro_costo.Where(x => x.id_responsable == empleado.id);
        //        return View(centros.ToList());
        //    }
        //    else
        //    {
        //        return View("../Home/ErrorPermisos");
        //    }

        //}

        //// GET: ResponsableBudget/EditBudget
        //public ActionResult EditBudget(int? id)
        //{

        //    if (TieneRol(TipoRoles.BG_RESPONSABLE))
        //    {
        //        if (id == null)
        //        {
        //            return View("../Error/BadRequest");
        //        }

        //        //obtiene centro de costo
        //        budget_centro_costo centroCosto = db.budget_centro_costo.Find(id);
        //        if (centroCosto == null)
        //            return View("../Error/NotFound");

        //        //obtiene el año fiscal anterior (actual)
        //        budget_anio_fiscal anio_Fiscal_anterior = GetAnioFiscal(DateTime.Now.AddYears(-1));
        //        if (anio_Fiscal_anterior == null)
        //            return View("../Error/NotFound");

        //        //obtiene el año fiscal actual (forecast)
        //        budget_anio_fiscal anio_Fiscal_actual = GetAnioFiscal(DateTime.Now);
        //        if (anio_Fiscal_actual == null)
        //            return View("../Error/NotFound");

        //        //obtiene el año fiscal proximo (budget)
        //        budget_anio_fiscal anio_Fiscal_proximo = GetAnioFiscal(DateTime.Now.AddYears(1));
        //        if (anio_Fiscal_proximo == null)
        //            return View("../Error/NotFound");

        //        //obtiene el id_rel_centro_costo del budget
        //        budget_rel_anio_fiscal_centro rel_centro_budget = db.budget_rel_anio_fiscal_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_proximo.id && x.tipo == BG_Status.BUDGET).FirstOrDefault();
        //        if (rel_centro_budget == null)
        //        {
        //            //si no existe crea el rel anio forecast
        //            rel_centro_budget = new budget_rel_anio_fiscal_centro
        //            {
        //                id_anio_fiscal = anio_Fiscal_proximo.id,
        //                id_centro_costo = centroCosto.id,
        //                tipo = BG_Status.BUDGET,
        //                estatus = true //activado por defecto
        //            };

        //            db.budget_rel_anio_fiscal_centro.Add(rel_centro_budget);
        //            //guarda en base de datos el centro creado
        //            db.SaveChanges();
        //        }


        //        //obtiene los valores para cada cuenta sap
        //        var valoresListAnioAnterior = db.view_valores_anio_fiscal.Where(x => x.id_anio_fiscal == anio_Fiscal_anterior.id && x.id_centro_costo == centroCosto.id).ToList();
        //        var valoresListAnioActual = db.view_valores_anio_fiscal.Where(x => x.id_anio_fiscal == anio_Fiscal_actual.id && x.id_centro_costo == centroCosto.id).ToList();
        //        var valoresListAnioProximo = db.view_valores_anio_fiscal.Where(x => x.id_anio_fiscal == anio_Fiscal_proximo.id && x.id_centro_costo == centroCosto.id).ToList();

        //        valoresListAnioAnterior = AgregaCuentasSAP(valoresListAnioAnterior, anio_Fiscal_anterior.id, centroCosto.id);
        //        valoresListAnioActual = AgregaCuentasSAP(valoresListAnioActual, anio_Fiscal_actual.id, centroCosto.id).OrderBy(x => x.id_cuenta_sap).ToList();
        //        valoresListAnioProximo = AgregaCuentasSAP(valoresListAnioProximo, anio_Fiscal_proximo.id, centroCosto.id);

        //        ViewBag.centroCosto = centroCosto;
        //        ViewBag.anio_Fiscal_anterior = anio_Fiscal_anterior;
        //        ViewBag.anio_Fiscal_actual = anio_Fiscal_actual;
        //        ViewBag.anio_Fiscal_proximo = anio_Fiscal_proximo;
        //        ViewBag.ValoresActual = valoresListAnioAnterior;
        //        ViewBag.ValoresActualForecast = valoresListAnioActual;
        //        ViewBag.ValoresBudget = valoresListAnioProximo;

        //        return View(rel_centro_budget);

        //    }
        //    else
        //    {
        //        return View("../Home/ErrorPermisos");
        //    }

        //}


        //// POST: ResponsableBudget/EditBudget
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditBudget(budget_rel_anio_fiscal_centro model, FormCollection form)
        //{
        //    //crea objetos apartir del form collection

        //    //1.-Obtiene los keys de tipo cantidad
        //    List<string> keysCantidades = form.AllKeys.Where(x => x.ToUpper().Contains("CANTIDAD") == true).ToList();

        //    //2.- Recorre y crea un objeto de tipo budget_valores
        //    List<budget_valores> valores = new List<budget_valores>();

        //    foreach (var keyC in keysCantidades)
        //    {
        //        Match m = Regex.Match(keyC, "(\\d+)");
        //        string num = string.Empty;

        //        //extrae el numero de key
        //        if (m.Success)
        //        {
        //            num = m.Value;

        //            int id_cuenta_sap = 0;
        //            int mes = 0;
        //            decimal cantidad = 0;

        //            //obtiene el valor de la cuenta sap
        //            bool success_id_cuenta_sap = int.TryParse(form["budget_valores[" + num + "].id_cuenta_sap"], out id_cuenta_sap);
        //            bool success_mes = int.TryParse(form["budget_valores[" + num + "].mes"], out mes);
        //            bool succes_cantidad = decimal.TryParse(form["budget_valores[" + num + "].cantidad"], out cantidad);
        //            string currency = "USD";  //currency por defecto

        //            //verifica que no hubo errores al covertir los datos
        //            if (success_id_cuenta_sap && success_mes)
        //            {
        //                //crea y agrega un objeto de tipo budget_valores con la información leida
        //                valores.Add(new budget_valores
        //                {
        //                    id_rel_anio_centro = model.id,
        //                    id_cuenta_sap = id_cuenta_sap,
        //                    mes = mes,
        //                    currency_iso = currency,
        //                    cantidad = cantidad,
        //                });
        //            }
                    
        //        }
        //    }

        //    //lista para valores de meses segun la fecha actual
        //    List<int> mesesPendientes = new List<int>();                     

        //    //3.- identificar cuales son update, create, delete y sin cambios

        //    //obtiene los valores actuales en BD
        //    List<budget_valores> valoresEnBD = db.budget_valores.Where(x => x.id_rel_anio_centro == model.id ).ToList();

        //    //obtiene valores sin cambios
        //    List<budget_valores> valoresSinCambio = valores.Where(x => valoresEnBD.Any(y => y.id_rel_anio_centro == x.id_rel_anio_centro && y.id_cuenta_sap == x.id_cuenta_sap && y.mes == x.mes && y.cantidad == x.cantidad)).ToList();

        //    //obtiene valores con cambios o nuevos
        //    List<budget_valores> valoresDiferentes = valores.Except(valoresSinCambio).ToList();

        //    //DE valores Diferentes identificar cuales son create, update y delete
        //    List<budget_valores> valoresCreate = valoresDiferentes.Where(a => !valoresEnBD.Any(a1 => a1.id_rel_anio_centro == a.id_rel_anio_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes) && a.cantidad != 0).ToList();

        //    //DE valores Diferentes identificar cuales son create, update y delete
        //    List<budget_valores> valoresUpdate = valoresDiferentes.Where(a => valoresEnBD.Any(a1 => a1.id_rel_anio_centro == a.id_rel_anio_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes && a.cantidad != 0)).ToList();
        //    //agrega el id correspondiente
        //    valoresUpdate = valoresUpdate.Select(x =>
        //    {
        //        x.id = (from v in valoresEnBD
        //                where v.id_rel_anio_centro == x.id_rel_anio_centro && v.id_cuenta_sap == x.id_cuenta_sap && v.mes == x.mes
        //                select v.id).FirstOrDefault(); return x;
        //    }).ToList();

        //    //DE valores Diferentes identificar cuales son create, update y delete
        //    List<budget_valores> valoresDelete = valoresDiferentes.Where(a => valoresEnBD.Any(a1 => a1.id_rel_anio_centro == a.id_rel_anio_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes && a.cantidad == 0)).ToList();
        //    //agrega el id  correspondiente
        //    valoresDelete = valoresDelete.Select(x =>
        //    {
        //        x.id = (from v in valoresEnBD
        //                where v.id_rel_anio_centro == x.id_rel_anio_centro && v.id_cuenta_sap == x.id_cuenta_sap && v.mes == x.mes
        //                select v.id).FirstOrDefault(); return x;
        //    }).ToList();

        //    //crea los nuevos registros
        //    try
        //    {
        //        db.budget_valores.AddRange(valoresCreate);
        //        db.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
        //    }

        //    //modifica los registros actuales
        //    try
        //    {
        //        foreach (budget_valores item in valoresUpdate)
        //        {
        //            //obtiene el elemento de BD
        //            budget_valores objeto = valoresEnBD.FirstOrDefault(x => x.id == item.id);

        //            if (objeto != null)
        //            {
        //                db.Entry(objeto).CurrentValues.SetValues(item);

        //            }

        //        }
        //        db.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
        //    }

        //    //Elimina los elemntos necesarios 
        //    try
        //    {
        //        foreach (budget_valores item in valoresDelete)
        //        {
        //            //obtiene el elemento de BD
        //            budget_valores objeto = valoresEnBD.FirstOrDefault(x => x.id == item.id);

        //            if (objeto != null)
        //            {
        //                db.budget_valores.Remove(objeto);
        //            }

        //        }
        //        db.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
        //    }

        //    TempData["Mensaje"] = new MensajesSweetAlert("Se ha modificado correctamente. ", TipoMensajesSweetAlerts.SUCCESS);

        //    return RedirectToAction("Budget");
        //}

        // GET: ResponsableBudget/Actual
        public ActionResult Centros()
        {
            if (TieneRol(TipoRoles.BG_RESPONSABLE))
            {
                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

               
                var centros = db.budget_centro_costo.Where(x => x.budget_responsables.Any(y => y.id_responsable == empleado.id));
                return View(centros.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: ResponsableBudget/DetailsCentro
        public ActionResult DetailsCentro(int? id)
        {

            if (TieneRol(TipoRoles.BG_RESPONSABLE))
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
                budget_anio_fiscal anio_Fiscal_anterior = GetAnioFiscal(DateTime.Now.AddYears(-1));
                if (anio_Fiscal_anterior == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal actual (forecast)
                budget_anio_fiscal anio_Fiscal_actual = GetAnioFiscal(DateTime.Now);
                if (anio_Fiscal_actual == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal proximo (budget)
                budget_anio_fiscal anio_Fiscal_proximo = GetAnioFiscal(DateTime.Now.AddYears(1));
                if (anio_Fiscal_proximo == null)
                    return View("../Error/NotFound");

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


                //obtiene los valores para cada cuenta sap
                var valoresListAnioAnterior = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_anterior.id && x.id_centro_costo == centroCosto.id).ToList();
                var valoresListAnioActual = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_actual.id && x.id_centro_costo == centroCosto.id).ToList();
                var valoresListAnioProximo = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == anio_Fiscal_proximo.id && x.id_centro_costo == centroCosto.id).ToList();

                valoresListAnioAnterior = AgregaCuentasSAPFaltantes(valoresListAnioAnterior, anio_Fiscal_anterior.id, centroCosto.id);
                valoresListAnioActual = AgregaCuentasSAPFaltantes(valoresListAnioActual, anio_Fiscal_actual.id, centroCosto.id).OrderBy(x => x.id_cuenta_sap).ToList();
                valoresListAnioProximo = AgregaCuentasSAPFaltantes(valoresListAnioProximo, anio_Fiscal_proximo.id, centroCosto.id);


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

        // GET: ResponsableBudget/EditCentroPresente
        public ActionResult EditCentroPresente(int? id)
        {

            if (TieneRol(TipoRoles.BG_RESPONSABLE))
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
                budget_anio_fiscal anio_Fiscal_anterior = GetAnioFiscal(DateTime.Now.AddYears(-1));
                if (anio_Fiscal_anterior == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal actual (forecast)
                budget_anio_fiscal anio_Fiscal_actual = GetAnioFiscal(DateTime.Now);
                if (anio_Fiscal_actual == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal proximo (budget)
                budget_anio_fiscal anio_Fiscal_proximo = GetAnioFiscal(DateTime.Now.AddYears(1));
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

                valoresListAnioAnterior = AgregaCuentasSAPFaltantes(valoresListAnioAnterior, anio_Fiscal_anterior.id, centroCosto.id);
                valoresListAnioActual = AgregaCuentasSAPFaltantes(valoresListAnioActual, anio_Fiscal_actual.id, centroCosto.id).OrderBy(x => x.id_cuenta_sap).ToList();
                valoresListAnioProximo = AgregaCuentasSAPFaltantes(valoresListAnioProximo, anio_Fiscal_proximo.id, centroCosto.id);


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

        // GET: ResponsableBudget/EditCentroProximo
        public ActionResult EditCentroProximo(int? id)
        {

            if (TieneRol(TipoRoles.BG_RESPONSABLE))
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
                budget_anio_fiscal anio_Fiscal_anterior = GetAnioFiscal(DateTime.Now.AddYears(-1));
                if (anio_Fiscal_anterior == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal actual (forecast)
                budget_anio_fiscal anio_Fiscal_actual = GetAnioFiscal(DateTime.Now);
                if (anio_Fiscal_actual == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal proximo (budget)
                budget_anio_fiscal anio_Fiscal_proximo = GetAnioFiscal(DateTime.Now.AddYears(1));
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

                valoresListAnioAnterior = AgregaCuentasSAPFaltantes(valoresListAnioAnterior, anio_Fiscal_anterior.id, centroCosto.id);
                valoresListAnioActual = AgregaCuentasSAPFaltantes(valoresListAnioActual, anio_Fiscal_actual.id, centroCosto.id).OrderBy(x => x.id_cuenta_sap).ToList();
                valoresListAnioProximo = AgregaCuentasSAPFaltantes(valoresListAnioProximo, anio_Fiscal_proximo.id, centroCosto.id);


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


        // POST: ResponsableBudget/EditCentro
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCentro(FormCollection form)
        {
            //crea objetos apartir del form collection

            int id_rel_fy = 0;
            int.TryParse(form["id_rel_fy_centro"], out id_rel_fy);


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
                        });
                    }

                }
            }

            //lista para valores de meses segun la fecha actual
            List<int> mesesPendientes = new List<int>();

            //3.- identificar cuales son update, create, delete y sin cambios

            //obtiene los valores actuales en BD
            List<budget_cantidad> valoresEnBD = db.budget_cantidad.Where(x => x.id_budget_rel_fy_centro == id_rel_fy).ToList();

            //obtiene valores sin cambios
            List<budget_cantidad> valoresSinCambio = valores.Where(x => valoresEnBD.Any(y => y.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro && y.id_cuenta_sap == x.id_cuenta_sap && y.mes == x.mes && y.cantidad == x.cantidad)).ToList();

            //obtiene valores con cambios o nuevos
            List<budget_cantidad> valoresDiferentes = valores.Except(valoresSinCambio).ToList();

            //DE valores Diferentes identificar cuales son create, update y delete
            List<budget_cantidad> valoresCreate = valoresDiferentes.Where(a => !valoresEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes) && a.cantidad != 0).ToList();

            //DE valores Diferentes identificar cuales son create, update y delete
            List<budget_cantidad> valoresUpdate = valoresDiferentes.Where(a => valoresEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes && a.cantidad != 0)).ToList();
            //agrega el id correspondiente
            valoresUpdate = valoresUpdate.Select(x =>
            {
                x.id = (from v in valoresEnBD
                        where v.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro && v.id_cuenta_sap == x.id_cuenta_sap && v.mes == x.mes
                        select v.id).FirstOrDefault(); return x;
            }).ToList();

            //DE valores Diferentes identificar cuales son create, update y delete
            List<budget_cantidad> valoresDelete = valoresDiferentes.Where(a => valoresEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes && a.cantidad == 0)).ToList();
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

            TempData["Mensaje"] = new MensajesSweetAlert("Se ha modificado correctamente. ", TipoMensajesSweetAlerts.SUCCESS);

            return RedirectToAction("Centros");
        }

        [NonAction]
        public static budget_anio_fiscal GetAnioFiscal(DateTime fechaBusqueda)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            //obtiene la fecha y la coloca al inicio del mes          
            var fecha = new DateTime(fechaBusqueda.Year, fechaBusqueda.Month, 1, 0, 0, 0);
            //encuentra el año fiscal anterior

            var anio_fiscal_list = db.budget_anio_fiscal.Where(x => x.anio_inicio == fecha.Year || x.anio_fin == fecha.Year).ToList();

            foreach (budget_anio_fiscal anioFiscal in anio_fiscal_list)
            {
                DateTime inicio = new DateTime(anioFiscal.anio_inicio, anioFiscal.mes_inicio, 1, 0, 0, 0);
                DateTime fin = new DateTime(anioFiscal.anio_fin, anioFiscal.mes_fin, 1, 0, 0, 0);

                if (fecha >= inicio && fecha <= fin)
                    return anioFiscal;
            }

            return null;
        }

        //eliminar erte método

        [NonAction]
        public static List<view_valores_anio_fiscal> AgregaCuentasSAP(List<view_valores_anio_fiscal> listValores, int id_anio_fiscal, int id_centro_costo, bool soloCuentasActivas = false)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            List<budget_cuenta_sap> listCuentas = new List<budget_cuenta_sap>();

            if (soloCuentasActivas) //carga sólo las cuentas activas
                listCuentas = db.budget_cuenta_sap.Where(x => x.activo == true).ToList();
            else //carga todas las cuentas
                listCuentas = db.budget_cuenta_sap.ToList();

            //Agrega cuentas vacias

            List<view_valores_anio_fiscal> listViewCuentas = new List<view_valores_anio_fiscal>();

            foreach (budget_cuenta_sap cuenta in listCuentas)
            {
                //agragega un objeto de tipo view_valores_anio_fiscal por cada cuenta existente
                listViewCuentas.Add(new view_valores_anio_fiscal
                {
                    id_anio_fiscal = id_anio_fiscal,
                    id_centro_costo = id_centro_costo,
                    id_cuenta_sap = cuenta.id,
                    sap_account = cuenta.sap_account,
                    name = cuenta.name,
                    descripcion = cuenta.budget_mapping.budget_mapping_bridge.descripcion,
                    currency_iso = "USD",
                });
            }

            //obtiene las cuentas que no se encuentran en el listado original
            List<view_valores_anio_fiscal> listDiferencias = listViewCuentas.Except(listValores).ToList();

            //suba la lista original con la lista de excepciones
            listValores.AddRange(listDiferencias);

            //ordena la lista
            listValores.OrderBy(x => x.id_cuenta_sap);

            return listValores;
        }

        [NonAction]
        public static List<view_valores_fiscal_year> AgregaCuentasSAPFaltantes(List<view_valores_fiscal_year> listValores, int id_anio_fiscal, int id_centro_costo, bool soloCuentasActivas = false)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            List<budget_cuenta_sap> listCuentas = new List<budget_cuenta_sap>();

            if (soloCuentasActivas) //carga sólo las cuentas activas
                listCuentas = db.budget_cuenta_sap.Where(x => x.activo == true).ToList();
            else //carga todas las cuentas
                listCuentas = db.budget_cuenta_sap.ToList();

            //Agrega cuentas vacias

            List<view_valores_fiscal_year> listViewCuentas = new List<view_valores_fiscal_year>();

            foreach (budget_cuenta_sap cuenta in listCuentas)
            {
                //agragega un objeto de tipo view_valores_anio_fiscal por cada cuenta existente
                listViewCuentas.Add(new view_valores_fiscal_year
                {
                    id_anio_fiscal = id_anio_fiscal,
                    id_centro_costo = id_centro_costo,
                    id_cuenta_sap = cuenta.id,
                    sap_account = cuenta.sap_account,
                    name = cuenta.name,
                    mapping = cuenta.budget_mapping.descripcion,
                    mapping_bridge = cuenta.budget_mapping.budget_mapping_bridge.descripcion,
                    currency_iso = "USD",
                });
            }

            //obtiene las cuentas que no se encuentran en el listado original
            List<view_valores_fiscal_year> listDiferencias = listViewCuentas.Except(listValores).ToList();

            //suba la lista original con la lista de excepciones
            listValores.AddRange(listDiferencias);

           
            return listValores.OrderBy(x => x.id_cuenta_sap).ToList();
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
