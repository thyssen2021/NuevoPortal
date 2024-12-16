using ExcelDataReader;
using Microsoft.AspNet.SignalR;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [System.Web.Mvc.Authorize]
    public class Inv_CargaArchivosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: CargarArchivos
        public ActionResult Index()
        {
            ViewBag.Materiales = db.Inv_Material.ToList();
            ViewBag.Lotes = db.Inv_Lote.ToList();
            ViewBag.Plantas = db.plantas.ToList(); // Pasar lista de plantas al ViewBag

            return View();
        }

        [HttpPost]
        public ActionResult UploadInventario(HttpPostedFileBase archivoInventario, int? plantaSeleccionada)
        {
            if (archivoInventario != null && archivoInventario.ContentLength > 0)
            {
                int processedRows = 0;

                int filasCreadas = 0;
                int filasActualizadas = 0;
                int materialesCreados = 0;
                int materialesEliminados = 0;
                int lotesEliminados = 0;
                int gruposEliminados = 0;
                List<int> filasErrores = new List<int>();
                var context = GlobalHost.ConnectionManager.GetHubContext<InventarioHub>();


                try
                {
                    var plantaDB = db.plantas.FirstOrDefault(p => p.clave == plantaSeleccionada.Value);
                    if (plantaDB == null)
                    {
                        TempData["ErrorMessage"] = "La planta seleccionada no es válida.";
                        return RedirectToAction("Index");
                    }

                    List<string> materialesEnArchivo = new List<string>();
                    List<string> lotesEnArchivo = new List<string>();

                    using (var stream = archivoInventario.InputStream)
                    {
                        var datasetConfig = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = tableReader => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true // Usar la primera fila como encabezado
                            }
                        };

                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var dataSet = reader.AsDataSet(datasetConfig);
                            var dataTable = dataSet.Tables[0]; // Primera hoja del archivo

                            for (int i = 0; i < dataTable.Rows.Count; i++)
                            {
                                try
                                {
                                    var row = dataTable.Rows[i];

                                    // indica el numero de filas procesadas
                                    processedRows++;
                                    // Enviar progreso al cliente

                                    if (string.IsNullOrWhiteSpace(row["Plant"].ToString()) &&
                                        string.IsNullOrWhiteSpace(row["Material"].ToString()) &&
                                        string.IsNullOrWhiteSpace(row["Batch"].ToString()))
                                    {
                                        break;
                                    }

                                    string material = row["Material"].ToString();
                                    string batch = row["Batch"].ToString();
                                    materialesEnArchivo.Add(material);
                                    string plant = row["Plant"].ToString();
                                    string storageLocation = row["Storage Location"].ToString();
                                    string storageBin = row["Storage Bin"].ToString();
                                    string materialDescription = row["Material Description"].ToString();
                                    int pieces = (int)Math.Floor(decimal.Parse(row["Pieces"].ToString()));
                                    int unrestricted = (int)Math.Floor(decimal.Parse(row["Unrestricted"].ToString()));
                                    int blocked = (int)Math.Floor(decimal.Parse(row["Blocked"].ToString()));
                                    int inQualityInsp = (int)Math.Floor(decimal.Parse(row["In Quality Insp."].ToString()));

                                    if (plant != plantaDB.codigoSap)
                                    {
                                        TempData["ErrorMessage"] = "La planta seleccionada y el archivo no es válido.";
                                        return RedirectToAction("Index");
                                    }


                                    //agrega la concatenacion del lote
                                    lotesEnArchivo.Add(storageLocation + storageBin + batch);


                                    var planta = db.plantas.FirstOrDefault(p => p.codigoSap == plant);
                                    if (planta == null)
                                    {
                                        filasErrores.Add(i + 2);
                                        continue;
                                    }

                                    int plantClave = planta.clave;
                                    var materialExistente = db.Inv_Material.FirstOrDefault(m => m.NumeroMaterial == material);
                                    if (materialExistente == null)
                                    {
                                        materialExistente = new Inv_Material
                                        {
                                            NumeroMaterial = material,
                                            Descripcion = materialDescription,
                                        };
                                        db.Inv_Material.Add(materialExistente);
                                        db.SaveChanges();
                                        materialesCreados++;
                                    }

                                    var loteExistente = db.Inv_Lote.FirstOrDefault(l => l.Batch == batch && l.MaterialID == materialExistente.MaterialID
                                            && l.StorageBin == storageBin && l.StorageLocation == storageLocation && l.PlantClave == plantaDB.clave);
                                    if (loteExistente != null)
                                    {
                                        loteExistente.Pieces = pieces;
                                        loteExistente.StorageBin = storageBin;
                                        loteExistente.PlantClave = plantClave;
                                        loteExistente.Unrestricted = unrestricted;
                                        loteExistente.StorageLocation = storageLocation;
                                        loteExistente.Blocked = blocked;
                                        loteExistente.QualityInspection = inQualityInsp;
                                        filasActualizadas++;
                                    }
                                    else
                                    {
                                        var nuevoLote = new Inv_Lote
                                        {
                                            MaterialID = materialExistente.MaterialID,
                                            Batch = batch,
                                            StorageBin = storageBin,
                                            StorageLocation = storageLocation,
                                            PlantClave = plantClave,
                                            Pieces = pieces,
                                            Unrestricted = unrestricted,
                                            Blocked = blocked,
                                            QualityInspection = inQualityInsp
                                        };

                                        db.Inv_Lote.Add(nuevoLote);
                                        db.SaveChanges();
                                        filasCreadas++;
                                    }

                                    int porcentaje = (int)(((i + 1) / (double)(dataTable.Rows.Count - 1)) * 100);
                                    context.Clients.All.recibirProgresoExcel(porcentaje, i + 1, dataTable.Rows.Count - 1);
                                }
                                catch (Exception)
                                {
                                    filasErrores.Add(i + 2);
                                }
                            }
                        }
                    }

                    // Construir una lista de identificadores únicos para lotes en el archivo
                    var lotesIdentificadoresEnArchivo = lotesEnArchivo
                        .Select(lote => lote)
                        .ToHashSet(); // Usar HashSet para búsquedas rápidas

                    // Obtener los lotes a eliminar (solo IDs)
                    var lotesAEliminarIds = db.Inv_Lote
                        .Where(l => l.PlantClave == plantaSeleccionada.Value &&
                                    !lotesIdentificadoresEnArchivo.Contains(l.StorageLocation + l.StorageBin + l.Batch))
                        .Select(l => l.LoteID)
                        .ToList();

                    // Eliminar referencias relacionadas con los lotes
                    db.Inv_HistorialCaptura.RemoveRange(db.Inv_HistorialCaptura.Where(h => lotesAEliminarIds.Contains(h.LoteID.Value)));
                    db.Inv_GrupoLoteDetalle.RemoveRange(db.Inv_GrupoLoteDetalle.Where(d => lotesAEliminarIds.Contains(d.LoteID)));

                    // Eliminar los lotes
                    db.Inv_Lote.RemoveRange(db.Inv_Lote.Where(l => lotesAEliminarIds.Contains(l.LoteID)));

                    // Actualizar el contador
                    lotesEliminados += lotesAEliminarIds.Count;

                    // Construir un HashSet para búsquedas rápidas de materiales presentes en el archivo
                    var materialesEnArchivoSet = new HashSet<string>(materialesEnArchivo);

                    // Obtener los IDs de los materiales a eliminar
                    var materialesAEliminarIds = db.Inv_Material
                        .Where(m => !materialesEnArchivoSet.Contains(m.NumeroMaterial))
                        .Select(m => m.MaterialID)
                        .ToList();

                    // Eliminar lotes asociados a los materiales y planta seleccionada
                    db.Inv_Lote.RemoveRange(db.Inv_Lote.Where(l => materialesAEliminarIds.Contains(l.MaterialID) && l.PlantClave == plantaDB.clave));


                    // Eliminar grupos sin lotes
                    var gruposSinLotes = db.Inv_LoteGrupo
                        .Where(g => !db.Inv_GrupoLoteDetalle.Any(d => d.GrupoID == g.GrupoID))
                        .ToList();

                    foreach (var grupo in gruposSinLotes)
                    {
                        db.Inv_LoteGrupo.Remove(grupo);
                        gruposEliminados++;
                    }

                    db.SaveChanges();

                    TempData["SuccessMessage"] = $"Inventario procesado: {filasCreadas} filas creadas, {filasActualizadas} actualizadas, {materialesCreados} materiales creados, {filasErrores.Count} errores, {lotesEliminados} lotes eliminados, {materialesEliminados} materiales eliminados, {gruposEliminados} grupos eliminados.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al procesar el archivo: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Por favor, cargue un archivo válido de inventario.";
            }

            return RedirectToAction("Index");
        }


        // Subir Archivo de Espesores
        [HttpPost]
        public ActionResult UploadEspesores(HttpPostedFileBase archivoEspesores)
        {
            if (archivoEspesores != null && archivoEspesores.ContentLength > 0)
            {
                int filasActualizadas = 0;
                int materialesNoEncontrados = 0;
                List<int> filasErrores = new List<int>();
                var context = GlobalHost.ConnectionManager.GetHubContext<InventarioHub>();

                try
                {
                    using (var stream = archivoEspesores.InputStream)
                    {
                        // Configuración para usar encabezados
                        var datasetConfig = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = tableReader => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true // Usar la primera fila como encabezado
                            }
                        };

                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var dataSet = reader.AsDataSet(datasetConfig);
                            var dataTable = dataSet.Tables[0]; // Primera hoja del archivo

                            for (int i = 0; i < dataTable.Rows.Count; i++) // Procesar todas las filas
                            {
                                try
                                {
                                    var row = dataTable.Rows[i];

                                    // Leer datos de la fila usando los nombres de las columnas
                                    string numeroMaterial = row["Matl."].ToString();
                                    string numeroAntiguo = row["NºMaterial antiguo"].ToString();
                                    string descripcion = row["Desc EN"].ToString();
                                    string gauge = row["Gauge"].ToString();
                                    string gaugeMin = row["Gauge MIN"].ToString();
                                    string gaugeMax = row["Gauge MAX"].ToString();

                                    // Validar si el material existe en la base de datos
                                    var materialExistente = db.Inv_Material.FirstOrDefault(m => m.NumeroMaterial == numeroMaterial);
                                    if (materialExistente != null)
                                    {
                                        // Actualizar los campos de espesores, tolerancias y número antiguo
                                        materialExistente.NumeroAntiguo = numeroAntiguo;
                                        materialExistente.Descripcion = descripcion;
                                        materialExistente.Espesor = decimal.TryParse(gauge, out var espesor) ? (decimal?)espesor : null;
                                        materialExistente.EspesorMin = decimal.TryParse(gaugeMin, out var espesorMin) ? (decimal?)espesorMin : null;
                                        materialExistente.EspesorMax = decimal.TryParse(gaugeMax, out var espesorMax) ? (decimal?)espesorMax : null;

                                        filasActualizadas++;
                                    }
                                    else
                                    {
                                        // Contar materiales no encontrados sin crearlos
                                        materialesNoEncontrados++;
                                    }
                                }
                                catch (Exception e)
                                {
                                    filasErrores.Add(i + 2); // Registrar fila con error
                                }

                                db.SaveChanges();

                                int porcentaje = (int)(((i + 1) / (double)(dataTable.Rows.Count - 1)) * 100);
                                context.Clients.All.recibirProgresoExcel(porcentaje, i, dataTable.Rows.Count - 1);
                            }
                        }

                    }


                    TempData["SuccessMessage"] = $"Archivo de espesores procesado: {filasActualizadas} filas actualizadas, {materialesNoEncontrados} materiales no encontrados, {filasErrores.Count} errores.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al procesar el archivo: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Por favor, cargue un archivo válido de espesores.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [System.Web.Mvc.Authorize]
        public ActionResult BorrarDatosCapturados(string planta)
        {
            try
            {
                int plantaClave = 0;

                if (string.IsNullOrEmpty(planta) || (planta != "all" && !int.TryParse(planta, out plantaClave)))
                {
                    return Json(new { success = false, message = "Debe seleccionar una planta válida." });
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    if (planta == "all")
                    {
                        // Actualizar y eliminar datos de todas las plantas
                        db.Database.ExecuteSqlCommand("UPDATE Inv_Lote SET AlturaCalculada = NULL, EspesorUsuario = NULL, UbicacionFisica = NULL");
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_HistorialCaptura");
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_GrupoLoteDetalle");
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_LoteGrupo");
                    }
                    else
                    {
                        // Actualizar y eliminar datos solo de la planta seleccionada
                        db.Database.ExecuteSqlCommand("UPDATE Inv_Lote SET AlturaCalculada = NULL, EspesorUsuario = NULL, UbicacionFisica = NULL WHERE PlantClave = @p0", plantaClave);
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_HistorialCaptura WHERE LoteID IN (SELECT LoteID FROM Inv_Lote WHERE PlantClave = @p0)", plantaClave);
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_GrupoLoteDetalle WHERE LoteID IN (SELECT LoteID FROM Inv_Lote WHERE PlantClave = @p0)", plantaClave);
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_LoteGrupo WHERE GrupoID IN (SELECT GrupoID FROM Inv_GrupoLoteDetalle WHERE LoteID IN (SELECT LoteID FROM Inv_Lote WHERE PlantClave = @p0))", plantaClave);
                    }

                    transaction.Commit();
                }

                return Json(new { success = true, message = "Datos borrados correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al borrar los datos: {ex.Message}" });
            }
        }


        public ActionResult BorrarTodo(string planta)
        {
            try
            {
                int plantaClave = 0;

                if (string.IsNullOrEmpty(planta) || (planta != "all" && !int.TryParse(planta, out plantaClave)))
                {
                    return Json(new { success = false, message = "Debe seleccionar una planta válida." }, JsonRequestBehavior.AllowGet);
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    if (planta == "all")
                    {
                        // Borrar datos de todas las plantas
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_HistorialCaptura");
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_GrupoLoteDetalle");
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_LoteGrupo");
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_Lote");
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_Material");
                    }
                    else
                    {
                        // Borrar datos solo de la planta seleccionada
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_HistorialCaptura WHERE LoteID IN (SELECT LoteID FROM Inv_Lote WHERE PlantClave = @p0)", plantaClave);
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_GrupoLoteDetalle WHERE LoteID IN (SELECT LoteID FROM Inv_Lote WHERE PlantClave = @p0)", plantaClave);
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_LoteGrupo WHERE GrupoID IN (SELECT GrupoID FROM Inv_GrupoLoteDetalle WHERE LoteID IN (SELECT LoteID FROM Inv_Lote WHERE PlantClave = @p0))", plantaClave);
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_Lote WHERE PlantClave = @p0", plantaClave);
                        db.Database.ExecuteSqlCommand("DELETE FROM Inv_Material WHERE MaterialID NOT IN (SELECT MaterialID FROM Inv_Lote)", plantaClave);
                    }

                    transaction.Commit();
                }

                return Json(new { success = true, message = "Datos borrados correctamente." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al borrar los datos: {ex.Message}" }, JsonRequestBehavior.AllowGet);
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