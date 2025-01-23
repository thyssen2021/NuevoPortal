using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;
using SpreadsheetLight;

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
                            //Debug.WriteLine($"Vehicle ID: {vehicle.Id}, Month: {month}, Cantidad: {cantidad}");
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

        #region Carga Masiva Hechizos
        public ActionResult DownloadDemandTemplate()
        {
            using (var sl = new SLDocument())
            {
                var culture = new CultureInfo("es-MX");

                int startYear = 2019;
                int endYear = DateTime.Now.Year + 10;
                var yearRange = Enumerable.Range(startYear, endYear - startYear + 1).ToList();

                var vehicles = db.BG_ihs_vehicle_custom.ToList();
                var demands = db.BG_IHS_custom_rel_demanda.ToList();

                // Cambiar el nombre de la hoja predeterminada a un nombre técnico
                sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Demand_Data");

                // Crear encabezados
                sl.SetCellValue(1, 1, "ID_Vehicle");
                sl.HideColumn(1); // Ocultar ID
                sl.SetCellValue(1, 2, "Vehicle");

                var vehicleProperties = new[] {
            "MnemonicVehiclePlant", "ProductionPlant", "ManufacturerGroup",
            "SOP_StartOfProduction", "EOP_EndOfProduction", "AssemblyType"
        };

                int currentColumn = 3;
                foreach (var prop in vehicleProperties)
                {
                    sl.SetCellValue(1, currentColumn++, prop);
                }

                foreach (var year in yearRange)
                {
                    for (int month = 1; month <= 12; month++)
                    {
                        sl.SetCellValue(1, currentColumn++, new DateTime(year, month, 1).ToString("MMM-yyyy", culture));
                    }
                }

                // Estilos para encabezados
                var monthHeaderStyle = sl.CreateStyle();
                monthHeaderStyle.Font.Bold = true;
                monthHeaderStyle.Font.FontColor = System.Drawing.Color.White;
                monthHeaderStyle.Fill.SetPattern(
                    DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid,
                    System.Drawing.ColorTranslator.FromHtml("#009ff5"),
                    System.Drawing.Color.White
                );
                monthHeaderStyle.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                monthHeaderStyle.Alignment.Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center;

                var nonMonthHeaderStyle = sl.CreateStyle();
                nonMonthHeaderStyle.Font.Bold = true;
                nonMonthHeaderStyle.Font.FontColor = System.Drawing.Color.Black;
                nonMonthHeaderStyle.Fill.SetPattern(
                    DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid,
                    System.Drawing.Color.LightGray,
                    System.Drawing.Color.White
                );
                nonMonthHeaderStyle.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                nonMonthHeaderStyle.Alignment.Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center;

                sl.SetCellStyle(1, 1, 1, 2 + vehicleProperties.Length, nonMonthHeaderStyle);
                sl.SetCellStyle(1, 3 + vehicleProperties.Length, 1, currentColumn - 1, monthHeaderStyle);

                sl.FreezePanes(1, 2);

                var vehicleStyle = sl.CreateStyle();
                vehicleStyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.LightYellow, System.Drawing.Color.Black);

                var textStyle = sl.CreateStyle();
                textStyle.FormatCode = "@"; // Formato explícito como texto

                int row = 2;
                foreach (var vehicle in vehicles)
                {
                    sl.SetCellValue(row, 1, vehicle.Id);
                    sl.SetCellValue(row, 2, vehicle.Vehicle);
                    sl.SetCellStyle(row, 2, vehicleStyle);

                    sl.SetCellValue(row, 3, vehicle.MnemonicVehiclePlant);
                    sl.SetCellValue(row, 4, vehicle.ProductionPlant);
                    sl.SetCellValue(row, 5, vehicle.ManufacturerGroup);

                    // Configurar SOP y EOP como texto
                    sl.SetCellStyle(row, 6, textStyle);
                    sl.SetCellValue(row, 6, vehicle.sop_start_of_production.ToString("yyyy-MM"));

                    sl.SetCellStyle(row, 7, textStyle);
                    sl.SetCellValue(row, 7, vehicle.eop_end_of_production.ToString("yyyy-MM"));

                    sl.SetCellValue(row, 8, vehicle.AssemblyType);

                    var vehicleDemands = demands.Where(d => d.id_vehicle_custom == vehicle.Id).ToList();

                    int monthStartColumn = 9;
                    foreach (var year in yearRange)
                    {
                        for (int month = 1; month <= 12; month++)
                        {
                            var date = new DateTime(year, month, 1);
                            var demand = vehicleDemands.FirstOrDefault(d => d.fecha == date);
                            if (demand != null)
                            {
                                sl.SetCellValue(row, monthStartColumn++, demand.cantidad ?? 0);
                            }
                            else
                            {
                                monthStartColumn++;
                            }
                        }
                    }

                    int startCol = 3;
                    int endCol = currentColumn - 1;
                    sl.SetCellStyle(row, startCol, row, endCol, CreateNumericStyle(sl));

                    row++;
                }

                // Fila separadora
                sl.SetCellValue(row, 2, "Agrega nuevos elementos aquí");
                var separatorStyle = sl.CreateStyle();
                separatorStyle.Font.Italic = true;
                separatorStyle.Font.FontColor = System.Drawing.Color.Red;
                sl.SetCellStyle(row, 2, separatorStyle);

                sl.AutoFitColumn(2);
                sl.AutoFitColumn(3, currentColumn);

                var stream = new System.IO.MemoryStream();
                sl.SaveAs(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Demand_Template.xlsx");
            }
        }


        [HttpPost]
        public ActionResult UploadDemandTemplate(HttpPostedFileBase uploadedFile)
        {
            if (uploadedFile == null || uploadedFile.ContentLength == 0)
            {
                TempData["ErrorMessage"] = "No se seleccionó ningún archivo.";
                return RedirectToAction("UpdateDemand");
            }

            try
            {
                using (var sl = new SLDocument(uploadedFile.InputStream))
                {
                    // Cargar datos actuales en diccionarios para búsquedas rápidas
                    var existingVehicles = db.BG_ihs_vehicle_custom.ToDictionary(v => v.Id);
                    var vehicleNames = db.BG_ihs_vehicle_custom.Select(v => v.Vehicle).ToHashSet(StringComparer.OrdinalIgnoreCase);
                    var dbDemands = db.BG_IHS_custom_rel_demanda.ToDictionary(
                        r => $"{r.id_vehicle_custom}-{r.fecha:yyyy-MM-dd}");

                    var newVehicles = new List<BG_ihs_vehicle_custom>();
                    var newDemands = new List<BG_IHS_custom_rel_demanda>();
                    var updatedDemands = new List<BG_IHS_custom_rel_demanda>();
                    var deletedDemands = new List<BG_IHS_custom_rel_demanda>();
                    var pendingDemands = new List<(BG_ihs_vehicle_custom Vehicle, DateTime Fecha, int Cantidad)>();

                    int creadosVehiculos = 0, creadosDemandas = 0, actualizadosDemandas = 0, borradosDemandas = 0;

                    sl.SelectWorksheet("Demand_Data"); // Nombre técnico de la hoja

                    // Validar encabezados
                    if (sl.GetCellValueAsString(1, 1) != "ID_Vehicle" || sl.GetCellValueAsString(1, 2) != "Vehicle")
                    {
                        TempData["ErrorMessage"] = "La plantilla tiene encabezados inválidos.";
                        return RedirectToAction("UpdateDemand");
                    }

                    // Leer datos de la hoja
                    int row = 2;
                    while (!string.IsNullOrWhiteSpace(sl.GetCellValueAsString(row, 2)))
                    {
                        // Detectar y omitir la fila de leyenda "Agrega nuevos elementos aquí"
                        if (sl.GetCellValueAsString(row, 2).Trim().Equals("Agrega nuevos elementos aquí", StringComparison.OrdinalIgnoreCase))
                        {
                            row++;
                            continue;
                        }

                        // Leer ID o identificar si es un nuevo vehículo
                        var idVehicleCell = sl.GetCellValueAsString(row, 1);
                        int? idVehicle = int.TryParse(idVehicleCell, out int parsedId) ? parsedId : (int?)null;

                        var vehicleName = sl.GetCellValueAsString(row, 2).Trim();

                        BG_ihs_vehicle_custom vehicle = null;
                        if (idVehicle.HasValue && existingVehicles.ContainsKey(idVehicle.Value))
                        {
                            // Vehículo existente
                            vehicle = existingVehicles[idVehicle.Value];
                        }
                        else
                        {
                            // Validar si el nombre del vehículo ya existe en la base de datos
                            if (vehicleNames.Contains(vehicleName))
                            {
                                TempData["ErrorMessage"] = $"El vehículo '{vehicleName}' ya existe en la base de datos. No se puede duplicar.";
                                return RedirectToAction("UpdateDemand");
                            }

                            // Parsear SOP y EOP como texto (yyyy-MM)
                            DateTime? sop = null, eop = null;
                            if (!DateTime.TryParseExact(sl.GetCellValueAsString(row, 6).Trim(), "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedSop))
                            {
                                TempData["ErrorMessage"] = $"Fecha SOP inválida en fila {row}.";
                                return RedirectToAction("UpdateDemand");
                            }
                            sop = parsedSop;

                            if (!DateTime.TryParseExact(sl.GetCellValueAsString(row, 7).Trim(), "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedEop))
                            {
                                TempData["ErrorMessage"] = $"Fecha EOP inválida en fila {row}.";
                                return RedirectToAction("UpdateDemand");
                            }
                            eop = parsedEop;

                            // Crear nuevo vehículo si no existe    
                            vehicle = new BG_ihs_vehicle_custom
                            {
                                MnemonicVehiclePlant = sl.GetCellValueAsString(row, 3).Trim(),
                                ProductionPlant = sl.GetCellValueAsString(row, 4).Trim(),
                                ManufacturerGroup = sl.GetCellValueAsString(row, 5).Trim(),
                                sop_start_of_production = sop.Value,
                                eop_end_of_production = eop.Value,
                                Vehicle = vehicleName,
                                AssemblyType = sl.GetCellValueAsString(row, 8).Trim(),
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };

                            newVehicles.Add(vehicle);
                            vehicleNames.Add(vehicleName); // Agregar el nuevo nombre a la lista de vehículos existentes
                            creadosVehiculos++;
                        }

                        // Procesar demandas mensuales para el vehículo
                        for (int col = 9; !string.IsNullOrWhiteSpace(sl.GetCellValueAsString(1, col)); col++)
                        {
                            string header = sl.GetCellValueAsString(1, col);
                            DateTime fecha = DateTime.ParseExact(header, "MMM-yyyy", new CultureInfo("es-MX"));

                            var valorCell = sl.GetCellValueAsString(row, col);
                            int? cantidad = int.TryParse(valorCell, out int parsedValue) ? parsedValue : (int?)null;

                            if (idVehicle.HasValue && dbDemands.TryGetValue($"{idVehicle.Value}-{fecha:yyyy-MM-dd}", out var existingDemand))
                            {
                                if (cantidad.HasValue)
                                {
                                    // Actualizar si el valor cambió
                                    if (existingDemand.cantidad != cantidad.Value)
                                    {
                                        existingDemand.cantidad = cantidad.Value;
                                        db.Entry(existingDemand).State = EntityState.Modified;
                                        actualizadosDemandas++;
                                    }
                                }
                                else
                                {
                                    deletedDemands.Add(existingDemand);
                                    borradosDemandas++;
                                }
                            }
                            else if (cantidad.HasValue)
                            {
                                if (!idVehicle.HasValue)
                                {
                                    pendingDemands.Add((vehicle, fecha, cantidad.Value));
                                }
                                else
                                {
                                    newDemands.Add(new BG_IHS_custom_rel_demanda
                                    {
                                        id_vehicle_custom = idVehicle.Value,
                                        fecha = fecha,
                                        cantidad = cantidad.Value
                                    });
                                    creadosDemandas++;
                                }
                            }
                        }

                        row++;
                    }

                    if (newVehicles.Any())
                    {
                        db.BG_ihs_vehicle_custom.AddRange(newVehicles);
                        db.SaveChanges();

                        foreach (var newVehicle in newVehicles)
                        {
                            existingVehicles[newVehicle.Id] = newVehicle;
                        }
                    }

                    foreach (var pending in pendingDemands)
                    {
                        var vehicleId = existingVehicles.First(v => v.Value.Vehicle == pending.Vehicle.Vehicle).Key;
                        newDemands.Add(new BG_IHS_custom_rel_demanda
                        {
                            id_vehicle_custom = vehicleId,
                            fecha = pending.Fecha,
                            cantidad = pending.Cantidad
                        });
                        creadosDemandas++;
                    }

                    if (newDemands.Any())
                    {
                        db.BG_IHS_custom_rel_demanda.AddRange(newDemands);
                    }

                    if (deletedDemands.Any())
                    {
                        db.BG_IHS_custom_rel_demanda.RemoveRange(deletedDemands);
                    }

                    db.SaveChanges();

                    TempData["SuccessMessage"] = $"Carga completada. Vehículos creados: {creadosVehiculos}, Demandas creadas: {creadosDemandas}, Actualizadas: {actualizadosDemandas}, Eliminadas: {borradosDemandas}.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error procesando archivo: {ex.Message}";
            }

            return RedirectToAction("UpdateDemand");
        }



        private SLStyle CreateNumericStyle(SLDocument sl)
        {
            var style = sl.CreateStyle();
            style.FormatCode = "#,##0";
            return style;
        }




        #endregion
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
