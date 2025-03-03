using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{

    [Authorize]
    public class budget_targetController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: budget_target
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.BG_CONTROLLING))
                return View("../Home/ErrorPermisos");

            var budget_target = db.budget_target.Include(b => b.budget_anio_fiscal).Include(b => b.budget_centro_costo).Include(b => b.budget_cuenta_sap);
            return View(budget_target.ToList());
        }


        // GET: budget_target/Edit/5
        public ActionResult Edit()
        {
            if (!TieneRol(TipoRoles.BG_CONTROLLING))
                return View("../Home/ErrorPermisos");

            // Cargamos las listas para los combos
            ViewBag.FiscalYears = db.budget_anio_fiscal.ToList();
            ViewBag.CentroCostos = db.budget_centro_costo.ToList();
            return View();
        }

        // GET: Budget_target/GetTargets?id_anio_fiscal=1&id_centro_costo=1
        public JsonResult GetTargets(int id_anio_fiscal, int id_centro_costo)
        {
            // Recuperamos la descripción del Año Fiscal y del Centro de Costos
            var fiscalYear = db.budget_anio_fiscal.Find(id_anio_fiscal);
            var centroCosto = db.budget_centro_costo.Find(id_centro_costo);

            // Obtenemos todas las cuentas disponibles
            var accounts = db.budget_cuenta_sap.ToList();

            // Obtenemos los registros existentes en budget_target para la combinación seleccionada
            var targets = db.budget_target
                .Where(t => t.id_anio_fiscal == id_anio_fiscal && t.id_centro_costo == id_centro_costo)
                .ToList();

            // Para cada cuenta, se arma un objeto con la data informativa y, de existir, el target y estatus
            var data = accounts.Select(a =>
            {
                var targetRecord = targets.FirstOrDefault(t => t.id_cuenta_sap == a.id);
                return new
                {
                    id = targetRecord != null ? targetRecord.id : 0,
                    id_anio_fiscal = id_anio_fiscal,
                    fiscalYearDesc = fiscalYear != null ? fiscalYear.descripcion : "",
                    id_centro_costo = id_centro_costo,
                    centroCostoDesc = centroCosto != null ? centroCosto.ConcatCentro : "",
                    id_cuenta_sap = a.id,
                    sap_account = a.sap_account,
                    accountName = a.name,
                    target = targetRecord != null ? targetRecord.target : (double?)null,
                    activado = targetRecord != null ? targetRecord.activado : false
                };
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // POST: Budget_target/SaveTargets
        [HttpPost]
        public JsonResult SaveTargets(int id_anio_fiscal, int id_centro_costo, List<BudgetTargetGridRow> rows)
        {
            foreach (var row in rows)
            {
                // Buscamos si ya existe un registro para esa cuenta, año fiscal y centro de costo
                var existing = db.budget_target.FirstOrDefault(t => t.id_cuenta_sap == row.id_cuenta_sap
                                                                && t.id_anio_fiscal == id_anio_fiscal
                                                                && t.id_centro_costo == id_centro_costo);
                if (existing != null)
                {
                    // Actualizamos el registro
                    existing.target = row.target.HasValue ? row.target.Value : 0;
                    existing.activado = row.activado;
                }
                else
                {
                    // Si no existe y se ingresó un valor, creamos el nuevo registro
                    if (row.target.HasValue)
                    {
                        var newRecord = new budget_target
                        {
                            id_cuenta_sap = row.id_cuenta_sap,
                            id_anio_fiscal = id_anio_fiscal,
                            id_centro_costo = id_centro_costo,
                            target = row.target.Value,
                            activado = row.activado
                        };
                        db.budget_target.Add(newRecord);
                    }
                }
            }
            db.SaveChanges();
            return Json(new { success = true });
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
    // Clase auxiliar para recibir la data del grid
    public class BudgetTargetGridRow
    {
        public int id { get; set; }
        public int id_cuenta_sap { get; set; }
        public double? target { get; set; }
        public bool activado { get; set; }
    }
}
