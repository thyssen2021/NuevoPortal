using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BG_ihs_vehicle_customController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_ihs_vehicle_custom
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            return View(db.BG_ihs_vehicle_custom.ToList());
        }

        // GET: BG_ihs_vehicle_custom/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_ihs_vehicle_custom bG_ihs_vehicle_custom = db.BG_ihs_vehicle_custom.Find(id);
            if (bG_ihs_vehicle_custom == null)
            {
                return HttpNotFound();
            }
            return View(bG_ihs_vehicle_custom);
        }

        // GET: UpdateDemand
        public ActionResult UpdateDemand()
        {
            ViewBag.SelectedYear = DateTime.Now.Year;
            ViewBag.Years = Enumerable.Range(2000, 41).ToList(); // Años de 2000 a 2040

            // Devuelve una lista vacía o inicial para que la vista funcione sin errores
            var emptyVehicles = new List<BG_IHS_VehicleCustomWithDemandDTO>();

            return View(emptyVehicles); // Carga la vista sin datos iniciales
        }

        // GET: LoadDemandData
        public JsonResult LoadDemandData(int year)
        {
            // Obtener los vehículos de la base de datos
            // Cargar los datos de los vehículos desde la base de datos
            var vehicles = db.BG_ihs_vehicle_custom
                .Select(v => new
                {
                    v.Id,
                    v.Vehicle,
                    v.MnemonicVehiclePlant,
                    v.ProductionPlant,
                    v.ManufacturerGroup,
                    v.sop_start_of_production,
                    v.eop_end_of_production,
                    v.AssemblyType
                })
                .AsEnumerable() // Cambiar a LINQ to Objects para permitir formateo
                .Select(v => new BG_IHS_VehicleCustomWithDemandDTO
                {
                    Id = v.Id,
                    Vehicle = v.Vehicle,
                    MnemonicVehiclePlant = v.MnemonicVehiclePlant,
                    ProductionPlant = v.ProductionPlant,
                    ManufacturerGroup = v.ManufacturerGroup,
                    sop_start_of_production = v.sop_start_of_production.ToString("yyyy-MM"), // Formatear aquí
                    eop_end_of_production = v.eop_end_of_production.ToString("yyyy-MM"), // Formatear aquí
                    AssemblyType = v.AssemblyType
                })
                .ToList();

            // Obtener las demandas del año seleccionado
            var demands = db.BG_IHS_custom_rel_demanda
                .Where(d => d.fecha.Year == year)
                .ToList();

            // Asignar demandas por mes a cada vehículo
            foreach (var vehicle in vehicles)
            {
                vehicle.DemandByMonth = Enumerable.Range(1, 12)
                    .ToDictionary(
                        month => month.ToString(), // Convierte la clave a string
                        month =>
                        {
                            var cantidad = demands
                                .Where(d => d.id_vehicle_custom == vehicle.Id && d.fecha.Month == month)
                                .Sum(d => d.cantidad ?? 0);
                            Debug.WriteLine($"Vehicle ID: {vehicle.Id}, Month: {month}, Cantidad: {cantidad}");
                            return cantidad;
                        }
                    );
            }


            // Devolver los datos en formato JSON
            return Json(vehicles, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveDemand(int year, List<DemandaModel> demandas)
        {
            try
            {
                if (demandas == null || demandas.Count == 0)
                {
                    return Json(new { success = false, message = "No hay datos para guardar." });
                }

                // Obtener los registros existentes de la base de datos en una sola consulta
                var vehicleIds = demandas.Select(d => d.VehicleId).Distinct().ToList();
                var registrosExistentes = db.BG_IHS_custom_rel_demanda
                    .Where(d => vehicleIds.Contains(d.id_vehicle_custom) && d.fecha.Year == year)
                    .ToList();

                // Listas para nuevos registros y registros a eliminar
                var nuevosRegistros = new List<BG_IHS_custom_rel_demanda>();
                var registrosAEliminar = new List<BG_IHS_custom_rel_demanda>();

                foreach (var item in demandas)
                {
                    // Buscar si el registro ya existe
                    var demandaExistente = registrosExistentes
                        .FirstOrDefault(d => d.id_vehicle_custom == item.VehicleId && d.fecha.Month == item.Month);

                    if (item.Cantidad == 0)
                    {
                        // Si la cantidad es 0 y el registro existe, marcar para eliminar
                        if (demandaExistente != null)
                        {
                            registrosAEliminar.Add(demandaExistente);
                        }
                    }
                    else
                    {
                        if (demandaExistente != null)
                        {
                            // Actualizar la cantidad si ya existe
                            demandaExistente.cantidad = item.Cantidad;
                        }
                        else
                        {
                            // Agregar a la lista de nuevos registros si no existe
                            nuevosRegistros.Add(new BG_IHS_custom_rel_demanda
                            {
                                id_vehicle_custom = item.VehicleId,
                                cantidad = item.Cantidad,
                                fecha = new DateTime(year, item.Month, 1)
                            });
                        }
                    }
                }

                // Eliminar registros marcados
                if (registrosAEliminar.Any())
                {
                    db.BG_IHS_custom_rel_demanda.RemoveRange(registrosAEliminar);
                }

                // Insertar nuevos registros
                if (nuevosRegistros.Any())
                {
                    db.BG_IHS_custom_rel_demanda.AddRange(nuevosRegistros);
                }

                // Guardar cambios en la base de datos
                db.SaveChanges();

                return Json(new { success = true, message = "Demanda guardada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al guardar los datos: " + ex.Message });
            }
        }



        // GET: BG_ihs_vehicle_custom/Create

        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            return View();
        }

        // POST: BG_ihs_vehicle_custom/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MnemonicVehiclePlant,ProductionPlant,ManufacturerGroup,sop_start_of_production,eop_end_of_production,Vehicle,AssemblyType")] BG_ihs_vehicle_custom bG_ihs_vehicle_custom)
        {
            // Verifica si existe un vehículo duplicado
            var existeVehiculo = db.BG_ihs_vehicle_custom
                .Any(v => v.Vehicle == bG_ihs_vehicle_custom.Vehicle && v.Id != bG_ihs_vehicle_custom.Id);

            if (existeVehiculo)
            {
                ModelState.AddModelError("Vehicle", "Ya existe un registro con este vehículo.");
            }

            // Valida que la fecha de fin de producción sea igual o mayor a la fecha de inicio
            if (bG_ihs_vehicle_custom.eop_end_of_production < bG_ihs_vehicle_custom.sop_start_of_production)
            {
                ModelState.AddModelError("eop_end_of_production", "La fecha de fin de producción debe ser igual o mayor a la fecha de inicio de producción.");
            }

            // Si el modelo no es válido después de las validaciones
            if (!ModelState.IsValid)
            {
                return View(bG_ihs_vehicle_custom);
            }

            // Si las validaciones son correctas
            bG_ihs_vehicle_custom.CreatedAt = DateTime.Now;
            bG_ihs_vehicle_custom.UpdatedAt = bG_ihs_vehicle_custom.CreatedAt;

            db.BG_ihs_vehicle_custom.Add(bG_ihs_vehicle_custom);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: BG_ihs_vehicle_custom/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_ihs_vehicle_custom bG_ihs_vehicle_custom = db.BG_ihs_vehicle_custom.Find(id);
            if (bG_ihs_vehicle_custom == null)
            {
                return HttpNotFound();
            }
            return View(bG_ihs_vehicle_custom);
        }

        // POST: BG_ihs_vehicle_custom/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MnemonicVehiclePlant,ProductionPlant,ManufacturerGroup,sop_start_of_production,eop_end_of_production,Vehicle,AssemblyType,CreatedAt")] BG_ihs_vehicle_custom bG_ihs_vehicle_custom)
        {
            // Verifica si existe un vehículo duplicado (excluyendo el registro actual)
            var existeVehiculo = db.BG_ihs_vehicle_custom
                .Any(v => v.Vehicle == bG_ihs_vehicle_custom.Vehicle && v.Id != bG_ihs_vehicle_custom.Id);

            if (existeVehiculo)
            {
                ModelState.AddModelError("Vehicle", "Ya existe un registro con este vehículo.");
            }

            // Valida que la fecha de fin de producción sea igual o mayor a la fecha de inicio
            if (bG_ihs_vehicle_custom.eop_end_of_production < bG_ihs_vehicle_custom.sop_start_of_production)
            {
                ModelState.AddModelError("eop_end_of_production", "La fecha de fin de producción debe ser igual o mayor a la fecha de inicio de producción.");
            }

            // Si el modelo no es válido después de las validaciones
            if (!ModelState.IsValid)
            {
                return View(bG_ihs_vehicle_custom);
            }

            // Actualiza el registro si las validaciones son correctas
            bG_ihs_vehicle_custom.UpdatedAt = DateTime.Now;
            db.Entry(bG_ihs_vehicle_custom).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }



        // POST: BG_ihs_vehicle_custom/Delete/5
        [HttpPost]
        public JsonResult DeleteConfirmed(int id)
        {
            try
            {
                // Buscar el registro en BG_ihs_vehicle_custom
                var entity = db.BG_ihs_vehicle_custom.Find(id);
                if (entity != null)
                {
                    // Verificar si hay registros en BG_IHS_custom_rel_demanda relacionados con este id
                    var relatedDemands = db.BG_IHS_custom_rel_demanda.Where(d => d.id_vehicle_custom == id).ToList();
                    if (relatedDemands.Any())
                    {
                        // Eliminar los registros relacionados en BG_IHS_custom_rel_demanda
                        db.BG_IHS_custom_rel_demanda.RemoveRange(relatedDemands);
                    }

                    // Verificar si hay registros en BG_Forecast_item con id_ihs_custom igual al id actual
                    var relatedForecasts = db.BG_Forecast_item.Where(f => f.id_ihs_custom == id).ToList();
                    if (relatedForecasts.Any())
                    {
                        // Establecer id_ihs_custom como NULL para los registros relacionados
                        foreach (var forecast in relatedForecasts)
                        {
                            forecast.id_ihs_custom = null;
                        }
                    }

                    // Eliminar el registro de BG_ihs_vehicle_custom
                    db.BG_ihs_vehicle_custom.Remove(entity);

                    // Guardar los cambios en la base de datos
                    db.SaveChanges();

                    return Json(new { success = true, message = "Registro eliminado exitosamente." });
                }

                return Json(new { success = false, message = "Registro no encontrado." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar el registro: " + ex.Message });
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

    public class DemandaModel
    {
        public int VehicleId { get; set; }
        public int Month { get; set; }
        public int Cantidad { get; set; }
    }
    public class BG_IHS_VehicleCustomWithDemandDTO
    {
        public int Id { get; set; } // ID del vehículo

        public string Vehicle { get; set; } // Nombre del vehículo

        public string MnemonicVehiclePlant { get; set; } // Mnemonic / Planta

        public string ProductionPlant { get; set; } // Planta de producción

        public string ManufacturerGroup { get; set; } // Grupo fabricante

        public string sop_start_of_production { get; set; } // Cambiado a string
        public string eop_end_of_production { get; set; } // Cambiado a string

        public string AssemblyType { get; set; } // Tipo de ensamble

        // Diccionario para las demandas por mes (Claves como cadenas)
        public Dictionary<string, int> DemandByMonth { get; set; } = new Dictionary<string, int>();
    }
}
