using ExcelDataReader;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class Inv_CargaArchivosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: CargarArchivos
        public ActionResult Index()
        {
            ViewBag.Materiales = db.Inv_Material.ToList();

            ViewBag.Lotes = db.Inv_Lote.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult UploadInventario(HttpPostedFileBase archivoInventario)
        {
            if (archivoInventario != null && archivoInventario.ContentLength > 0)
            {
                int filasCreadas = 0;
                int filasActualizadas = 0;
                int materialesCreados = 0;
                List<int> filasErrores = new List<int>();

                try
                {
                    using (var stream = archivoInventario.InputStream)
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

                                    // Verificar si la fila está vacía
                                    if (string.IsNullOrWhiteSpace(row["Plant"].ToString()) &&
                                        string.IsNullOrWhiteSpace(row["Material"].ToString()) &&
                                        string.IsNullOrWhiteSpace(row["Batch"].ToString()))
                                    {
                                        // Si la fila tiene los campos principales vacíos, se considera el final del archivo y se detiene el procesamiento
                                        break;
                                    }

                                    // Leer datos de la fila usando los nombres de las columnas
                                    string plant = row["Plant"].ToString();
                                    string storageLocation = row["Storage Location"].ToString();
                                    string storageBin = row["Storage Bin"].ToString();
                                    string material = row["Material"].ToString();
                                    string batch = row["Batch"].ToString();
                                    string materialDescription = row["Material Description"].ToString();

                                    // Convertir valores numéricos a enteros
                                    int pieces = (int)Math.Floor(decimal.Parse(row["Pieces"].ToString()));
                                    int unrestricted = (int)Math.Floor(decimal.Parse(row["Unrestricted"].ToString()));
                                    int blocked = (int)Math.Floor(decimal.Parse(row["Blocked"].ToString()));
                                    int inQualityInsp = (int)Math.Floor(decimal.Parse(row["In Quality Insp."].ToString()));

                                    // Buscar la planta correspondiente al código SAP
                                    var planta = db.plantas.FirstOrDefault(p => p.codigoSap == plant);
                                    if (planta == null)
                                    {
                                        // Si no se encuentra la planta, registrar el error y continuar
                                        filasErrores.Add(i + 2); // Registrar fila con error
                                        continue;
                                    }

                                    int plantClave = planta.clave;

                                    // Validar si el material existe en la base de datos
                                    var materialExistente = db.Inv_Material.FirstOrDefault(m => m.NumeroMaterial == material);
                                    if (materialExistente == null)
                                    {
                                        // Crear material si no existe
                                        materialExistente = new Inv_Material
                                        {
                                            NumeroMaterial = material,
                                            Descripcion = materialDescription,
                                        };
                                        db.Inv_Material.Add(materialExistente);
                                        db.SaveChanges(); // Guardar inmediatamente para evitar conflictos
                                        materialesCreados++;
                                    }

                                    // Buscar lote existente
                                    var loteExistente = db.Inv_Lote.FirstOrDefault(l => l.Batch == batch && l.MaterialID == materialExistente.MaterialID);
                                    if (loteExistente != null)
                                    {
                                        // Actualizar lote existente
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
                                        // Crear nuevo lote
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
                                        db.SaveChanges(); // Guardar inmediatamente para evitar conflictos
                                        filasCreadas++;
                                    }
                                }
                                catch (Exception)
                                {
                                    filasErrores.Add(i + 2); // Registrar fila con error
                                }
                            }
                        }
                    }

                    db.SaveChanges();
                    TempData["SuccessMessage"] = $"Inventario procesado: {filasCreadas} filas creadas, {filasActualizadas} actualizadas, {materialesCreados} materiales creados, {filasErrores.Count} errores.";
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
                                catch (Exception)
                                {
                                    filasErrores.Add(i + 2); // Registrar fila con error
                                }
                            }
                        }
                    }

                    db.SaveChanges();
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