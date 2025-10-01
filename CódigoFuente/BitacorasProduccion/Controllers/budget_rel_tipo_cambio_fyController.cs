using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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
    public class budget_rel_tipo_cambio_fyController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: budget_rel_tipo_cambio_fy
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.BG_CONTROLLING))
                return View("../Home/ErrorPermisos");

            // Recupera todos los registros para el FY seleccionado (todos) con la información relacionada
            var rates = db.budget_rel_tipo_cambio_fy
                .Include(t => t.budget_anio_fiscal)
                .ToList();

            // Agrupa por año fiscal y mes
            var grouped = rates.GroupBy(x => new { x.id_budget_anio_fiscal, x.mes })
                .Select(g =>
                {
                    // Tomamos el primer registro para obtener datos comunes
                    var first = g.First();
                    string fyDesc = first.budget_anio_fiscal.descripcion; // Por ejemplo, "2018/19"
                                                                          // Para determinar el año que se muestra junto al mes, asumimos:
                                                                          // - Si el mes es de octubre (10) a diciembre (12): pertenece al año inicial
                                                                          // - Si el mes es de enero (1) a septiembre (9): pertenece al año inicial + 1
                    int yearStart = 0;
                    if (!string.IsNullOrEmpty(fyDesc) && fyDesc.Length >= 4)
                        int.TryParse(fyDesc.Substring(0, 4), out yearStart);
                    int actualYear = g.Key.mes >= 10 ? yearStart : yearStart + 1;

                    string mesName = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetMonthName(g.Key.mes) + " " + actualYear;
                    return new FiscalRateViewModel
                    {
                        fy = "FY " + fyDesc,
                        mes = g.Key.mes,
                        mesName = mesName,
                        usd_mxn = (decimal)(g.FirstOrDefault(x => x.id_tipo_cambio == 1)?.cantidad ?? 0),
                        eur_usd = (decimal)(g.FirstOrDefault(x => x.id_tipo_cambio == 2)?.cantidad ?? 0)
                    };
                })
                .ToList();

            // Ordena:
            // Primero por el año fiscal (suponiendo que ordenamos alfabéticamente "FY 2018/19", etc.)
            // Luego por mes de forma fiscal: si el mes es >=10 se transforma en mes - 9, si es de 1 a 9 se transforma en mes + 3.
            grouped = grouped
                .OrderBy(x => x.fy)
                .ThenBy(x => x.mes >= 10 ? x.mes - 9 : x.mes + 3)
                .ToList();

            return View(grouped);
        }




        // GET: budget_rel_tipo_cambio_fy/Edit/5
        public ActionResult Edit()
        {
            if (!TieneRol(TipoRoles.BG_CONTROLLING))
                return View("../Home/ErrorPermisos");

            // Obtén todos los años fiscales, ordenados (por ejemplo, descendente por anio_inicio)
            var fiscalYears = db.budget_anio_fiscal.OrderBy(x => x.anio_inicio).ToList();
            // Se define el año fiscal actual, utilizando tu método de obtención de FY (aquí se asume que tienes uno)
            int currentFYId = GetCurrentFiscalYearId();
            ViewBag.FiscalYears = new SelectList(fiscalYears, "id", nameof(budget_anio_fiscal.ConcatAnio), currentFYId);
            return View();
        }

        // Método auxiliar para determinar el año fiscal actual (adaptar a tu lógica)
        private int GetCurrentFiscalYearId()
        {
            DateTime today = DateTime.Now;
            // Por ejemplo, usamos el método GetAnioFiscal (asegúrate de tenerlo disponible)
            var fy = ResponsableBudgetController.GetAnioFiscal(today);
            return fy != null ? fy.id : db.budget_anio_fiscal.OrderByDescending(x => x.anio_inicio).First().id;
        }

        // GET: TipoCambioFiscal/GetFiscalRates?id_fy=...
        public JsonResult GetFiscalRates(int id_fy)
        {
            var rates = db.budget_rel_tipo_cambio_fy
                .Include(r => r.budget_anio_fiscal)
                .Where(r => r.id_budget_anio_fiscal == id_fy)
                .ToList();

            var grouped = rates.GroupBy(r => r.mes).Select(g =>
            {
                var fiscalYear = g.FirstOrDefault()?.budget_anio_fiscal;
                var yearStart = int.Parse(fiscalYear.descripcion.Substring(0, 4)); // Obtiene el año inicial del FY
                var actualYear = g.Key >= 10 ? yearStart : yearStart + 1; // Ajuste de año basado en el mes

                return new
                {
                    mes = g.Key,
                    mesName = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetMonthName(g.Key) + " " + actualYear,
                    fy = "FY " + fiscalYear.descripcion,
                    usd_mxn = g.FirstOrDefault(x => x.id_tipo_cambio == 1)?.cantidad ?? 0,
                    eur_usd = g.FirstOrDefault(x => x.id_tipo_cambio == 2)?.cantidad ?? 0
                };
            })
            .OrderBy(x => x.mes >= 10 ? x.mes - 9 : x.mes + 3) // Orden fiscal (octubre es el primero)
            .ToList();

            return Json(grouped, JsonRequestBehavior.AllowGet);
        }



        // POST: TipoCambioFiscal/SaveFiscalRates
        [HttpPost]
        public JsonResult SaveFiscalRates(List<FiscalRateViewModel> rates, int id_fy)
        {
            if (rates == null || !rates.Any())
            {
                return Json(new { result = "ERROR", message = "No se recibieron datos." });
            }

            List<int> mesesModificados = new List<int>(); // Lista de meses modificados

            foreach (var rate in rates)
            {
                // Obtiene los valores actuales de la base de datos
                var recordUsdMxn = db.budget_rel_tipo_cambio_fy.FirstOrDefault(r =>
                    r.id_budget_anio_fiscal == id_fy && r.mes == rate.mes && r.id_tipo_cambio == 1);

                var recordEurUsd = db.budget_rel_tipo_cambio_fy.FirstOrDefault(r =>
                    r.id_budget_anio_fiscal == id_fy && r.mes == rate.mes && r.id_tipo_cambio == 2);

                // Primero, verificamos que ambos valores no sean nulos
                if (rate.usd_mxn != 0 && rate.eur_usd != 0)
                {
                    // Detecta cambios en el tipo de cambio USD/MXN
                    if (recordUsdMxn != null && recordUsdMxn.cantidad != (double)rate.usd_mxn)
                    {
                        recordUsdMxn.cantidad = (double)rate.usd_mxn;
                        if (!mesesModificados.Contains(rate.mes))
                            mesesModificados.Add(rate.mes);
                    }

                    // Detecta cambios en el tipo de cambio EUR/USD
                    if (recordEurUsd != null && recordEurUsd.cantidad != (double)rate.eur_usd)
                    {
                        recordEurUsd.cantidad = (double)rate.eur_usd;
                        if (!mesesModificados.Contains(rate.mes))
                            mesesModificados.Add(rate.mes);
                    }
                }
                else {
                    recordUsdMxn.cantidad = 0;
                    recordEurUsd.cantidad = 0;

                }

            }

            db.SaveChanges();


            // Muestra en consola los meses que han cambiado



            foreach (var mes in mesesModificados)
            {
                db.Database.ExecuteSqlCommand(
                     "EXEC usp_ActualizarBudgetCantidad @id_fy, @month",
                     new SqlParameter("@id_fy", id_fy),
                     new SqlParameter("@month", mes)
                 );
            }


            return Json(new { result = "OK", message = "Se actualizaron los datos correctamente." });
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
    // ViewModel para el envío/recepción de datos del Handsontable
    public class FiscalRateViewModel
    {
        public int mes { get; set; }
        public string mesName { get; set; }
        public string fy { get; set; }
        public decimal usd_mxn { get; set; }
        public decimal eur_usd { get; set; }
        // Opcionalmente, podrías incluir el id del año fiscal si es necesario para la actualización
        public int fyId { get; set; }
    }
}
