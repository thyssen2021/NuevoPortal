using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class CTZ_CatalogsController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: CTZ_Catalogs
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            return View();
        }

        public ActionResult engineering_dimension()
        {
            if (!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            // Cargamos las plantas activas
            var plants = db.CTZ_plants
                           .Where(p => p.Active)
                           .OrderBy(p => p.Description)
                           .ToList();

            ViewBag.Plants = new SelectList(plants, "ID_Plant", "Description");

            return View();
        }

        /// <summary>
        /// Retorna las líneas activas de la planta indicada en formato JSON.
        /// </summary>
        [HttpGet]
        public JsonResult GetLinesByPlant(int plantId)
        {
            var lines = db.CTZ_Production_Lines
                          .Where(l => l.Active && l.ID_Plant == plantId)
                          .OrderBy(l => l.Line_Name)
                          .Select(l => new
                          {
                              ID_Line = l.ID_Line,
                              Line_Name = l.Line_Name
                          })
                          .ToList();

            return Json(lines, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Carga los registros de CTZ_Engineering_Dimension (unidos a Criteria y Lines),
        /// filtrados por la planta y opcionalmente la línea. Retorna JSON.
        /// Incluye ID_Engineering_Dimension para poder actualizar luego.
        /// </summary>
        [HttpGet]
        public JsonResult LoadEngineeringDimension(int plantId, int? lineId)
        {
            var query = from d in db.CTZ_Engineering_Dimension
                        join c in db.CTZ_Engineering_Criteria on d.ID_Criteria equals c.ID_Criteria
                        join l in db.CTZ_Production_Lines on d.ID_Line equals l.ID_Line
                        join p in db.CTZ_plants on l.ID_Plant equals p.ID_Plant
                        where p.ID_Plant == plantId
                              && p.Active
                              && l.Active
                              && d.Active
                              && c.Active
                        select new
                        {
                            d.ID_Engineering_Dimension,
                            ID_Line = l.ID_Line,
                            Plant = p.Description,
                            Line = l.Line_Name,
                            Criteria = c.CriteriaName,
                            MinValue = d.Min_Value,
                            MaxValue = d.Max_Value
                        };

            if (lineId.HasValue && lineId.Value != 0)
            {
                query = query.Where(x => x.ID_Line == lineId.Value);
            }

            var data = query.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Recibe los datos editados desde Handsontable y actualiza la tabla
        /// CTZ_Engineering_Dimension en la BD.
        /// </summary>
        [HttpPost]
        public JsonResult SaveEngineeringDimension(List<EngineeringDimensionDTO> data)
        {
            // Validación básica
            if (data == null || data.Count == 0)
            {
                return Json(new { success = false, message = "No data received." });
            }

            try
            {
                foreach (var item in data)
                {
                    // Buscamos el registro original
                    var dimension = db.CTZ_Engineering_Dimension
                                      .FirstOrDefault(d => d.ID_Engineering_Dimension == item.ID_Engineering_Dimension);

                    if (dimension != null)
                    {
                        // Actualizamos sólo Min y Max (o lo que necesites)
                        dimension.Min_Value = item.MinValue;
                        dimension.Max_Value = item.MaxValue;
                        
                    }
                }

                db.SaveChanges();

                return Json(new { success = true, message = "Data saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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
    /// <summary>
    /// DTO para recibir los datos editados desde Handsontable.
    /// </summary>
    public class EngineeringDimensionDTO
    {
        public int ID_Engineering_Dimension { get; set; }
        public int ID_Line { get; set; }
        public string Criteria { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
    }
}