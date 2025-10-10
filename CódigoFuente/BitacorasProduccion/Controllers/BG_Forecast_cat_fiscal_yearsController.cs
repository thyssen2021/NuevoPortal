using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BG_Forecast_cat_fiscal_yearsController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_Forecast_cat_fiscal_years
        // Esta acción carga la página y pasa la lista de años fiscales a la vista.
        public ActionResult Index()
        {
            if (!TieneRol(Clases.Util.TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            // CAMBIO: Proyectamos los resultados para formatear las fechas a string "dd/MM/yyyy"
            var aniosFiscales = db.BG_Forecast_cat_fiscal_years
                .OrderBy(fy => fy.fecha_inicio)
                .ToList() // Traemos los datos a memoria para poder usar .ToString()
                .Select(fy => new {
                    fy.id,
                    fy.descripcion,
                    // Convertimos la fecha al formato final de visualización
                    fecha_inicio = fy.fecha_inicio.ToString("dd/MM/yyyy"),
                    fecha_fin = fy.fecha_fin.ToString("dd/MM/yyyy"),
                    fy.porcentaje_flete
                });

            return View(aniosFiscales);
        }

        // POST: BG_Forecast_cat_fiscal_years/GuardarCambios
        // Esta acción será llamada por AJAX desde Handsontable para guardar los datos.
        [HttpPost]
        public ActionResult GuardarCambios(List<BG_Forecast_cat_fiscal_years> aniosFiscales)
        {
            if (!TieneRol(Clases.Util.TipoRoles.BUDGET_IHS))
            {
                return Json(new { success = false, message = "No tienes permisos para realizar esta acción." });
            }

            if (aniosFiscales == null || !aniosFiscales.Any())
            {
                return Json(new { success = false, message = "No se recibieron datos para guardar." });
            }

            try
            {
                foreach (var anio in aniosFiscales)
                {
                    // Buscamos el registro en la BD por su ID
                    var anioEnDB = db.BG_Forecast_cat_fiscal_years.Find(anio.id);
                    if (anioEnDB != null)
                    {
                        // Actualizamos únicamente el campo de porcentaje
                        anioEnDB.porcentaje_flete = anio.porcentaje_flete;
                        db.Entry(anioEnDB).State = EntityState.Modified;
                    }
                }
                db.SaveChanges(); // Guardamos todos los cambios en una sola transacción

                return Json(new { success = true, message = "Los cambios se guardaron correctamente." });
            }
            catch (System.Exception ex)
            {
                // Manejo de errores
                return Json(new { success = false, message = "Ocurrió un error al guardar: " + ex.Message });
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
