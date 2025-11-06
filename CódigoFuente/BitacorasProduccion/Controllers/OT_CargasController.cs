using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcelDataReader;
using Portal_2_0.Models;
using System.IO;

namespace Portal_2_0.Controllers
{
    public class OT_CargasController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();
        // GET: OT_Cargas
        //
        // GET: /OT_Cargas/
        // Muestra la vista maestra con el listado y el formulario.
        public ActionResult Index()
        {
            // --- MARCADOR: AÑADIDO (ViewBags para _Layout) ---
            ViewBag.Title = "Carga Órdenes de Transporte";
            ViewBag.PrimerNivel = "ordenes_transporte"; // Lo usaremos en _Layout
            ViewBag.SegundoNivel = "ot_cargas";
            // --- FIN MARCADOR ---

            var viewModel = new OT_CargasViewModel
            {
                CargasExistentes = db.OT_Cargas.OrderByDescending(c => c.FechaCarga).ToList()
            };

            // Para mostrar mensajes de éxito/error de la carga anterior
            if (TempData["Mensaje"] != null)
            {
                viewModel.AlertMessage = TempData["Mensaje"].ToString();
                viewModel.AlertType = "success";
            }

            return View(viewModel);
        }

        //
        // POST: /OT_Cargas/ProcesarCarga
        // Recibe el archivo, lo procesa y guarda en BD.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcesarCarga(OT_CargasViewModel model)
        {
            // --- 1. Validación del Archivo ---
            if (model.ArchivoExcel == null || model.ArchivoExcel.ContentLength == 0)
                ModelState.AddModelError("ArchivoExcel", "No se ha seleccionado ningún archivo.");
            else if (!Path.GetExtension(model.ArchivoExcel.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError("ArchivoExcel", "El archivo debe tener formato .xlsx.");

            if (!ModelState.IsValid)
            {
                // Si hay un error, recargamos la vista con el listado y el error.
                model.CargasExistentes = db.OT_Cargas.OrderByDescending(c => c.FechaCarga).ToList();
                model.AlertMessage = "Error de validación. Revise los campos.";
                model.AlertType = "danger";
                return View("Index", model);
            }

            // Usamos una transacción de base de datos.
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    // --- 2. Crear la Carga Padre (OT_Cargas) ---
                    OT_Cargas nuevaCarga = new OT_Cargas
                    {
                        FechaCarga = DateTime.Now,
                        UsuarioCarga = User.Identity.Name,
                        NombreArchivo = Path.GetFileName(model.ArchivoExcel.FileName),
                        Comentarios = model.Comentarios,
                        TotalRegistros = 0 // Se actualiza al final
                    };
                    db.OT_Cargas.Add(nuevaCarga);
                    db.SaveChanges(); // Guardamos para obtener el ID_CargaOT

                    // --- 3. Preparar el DataTable para SqlBulkCopy ---
                    System.Data.DataTable dt = new System.Data.DataTable("OT_Datos");
                    // Los nombres DEBEN coincidir con la BD
                    dt.Columns.Add("ID_CargaOT", typeof(int));
                    dt.Columns.Add("Posicion", typeof(int));
                    dt.Columns.Add("ClDocumentoCompras", typeof(string));
                    dt.Columns.Add("TipoDocCompras", typeof(string));
                    dt.Columns.Add("GrupoCompras", typeof(string));
                    dt.Columns.Add("FechaDocumento", typeof(DateTime));
                    dt.Columns.Add("Material", typeof(string));
                    dt.Columns.Add("TextoBreve", typeof(string));
                    dt.Columns.Add("IndicadorBorrado", typeof(string));
                    dt.Columns.Add("Centro", typeof(string));
                    dt.Columns.Add("Almacen", typeof(string));
                    dt.Columns.Add("CantidadPedido", typeof(decimal));
                    dt.Columns.Add("UnidadMedidaPedido", typeof(string));
                    dt.Columns.Add("CantidadUMA", typeof(decimal));
                    dt.Columns.Add("UnidadMedidaAlmacen", typeof(string));
                    dt.Columns.Add("PrecioNeto", typeof(decimal));
                    dt.Columns.Add("Moneda", typeof(string));
                    dt.Columns.Add("CantidadBase", typeof(decimal));
                    dt.Columns.Add("CtdPrevPendiente", typeof(decimal));
                    dt.Columns.Add("PorEntregarCantidad", typeof(decimal));
                    dt.Columns.Add("PorEntregarValor", typeof(decimal));
                    dt.Columns.Add("PorCalcularCantidad", typeof(decimal));
                    dt.Columns.Add("TipoImputacion", typeof(string));
                    dt.Columns.Add("PorCalcularValor", typeof(decimal));
                    dt.Columns.Add("IndLiberacion", typeof(string));
                    dt.Columns.Add("DocumentoCompras", typeof(string));
                    dt.Columns.Add("ProveedorCentroSuministrador", typeof(string));

                    // --- 4. Leer el Excel con ExcelDataReader ---
                    DataSet result;
                    using (var reader = ExcelReaderFactory.CreateReader(model.ArchivoExcel.InputStream))
                    {
                        result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            // Configuración para que la primera fila sea de encabezados (y sea ignorada si usamos índices)
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                        });
                    }

                    // Buscamos la hoja "General"
                    if (!result.Tables.Contains("General"))
                        throw new Exception("El archivo Excel no contiene una hoja llamada 'General'.");

                    System.Data.DataTable worksheet = result.Tables["General"];

                    if (worksheet.Rows.Count == 0)
                        throw new Exception("La hoja 'General' está vacía.");

                    // Iteramos las filas (asumiendo que la fila 0 es encabezado, la saltamos)
                    foreach (DataRow row in worksheet.Rows)
                    {
                        // Validación simple: si la primera celda está vacía, saltamos.
                        if (row[0] == null || row[0] == DBNull.Value || string.IsNullOrWhiteSpace(row[0].ToString()))
                        {
                            continue;
                        }

                        DataRow dr = dt.NewRow();
                        dr["ID_CargaOT"] = nuevaCarga.ID_CargaOT;

                        // Mapeo de columnas (usamos los helpers que definiremos abajo)
                        dr["Posicion"] = GetInt(row, 0);
                        dr["ClDocumentoCompras"] = GetString(row, 1);
                        dr["TipoDocCompras"] = GetString(row, 2);
                        dr["GrupoCompras"] = GetString(row, 3);
                        dr["FechaDocumento"] = GetDate(row, 4);
                        dr["Material"] = GetString(row, 5);
                        dr["TextoBreve"] = GetString(row, 6);
                        dr["IndicadorBorrado"] = GetString(row, 7);
                        dr["Centro"] = GetString(row, 8);
                        dr["Almacen"] = GetString(row, 9);
                        dr["CantidadPedido"] = GetDecimal(row, 10);
                        dr["UnidadMedidaPedido"] = GetString(row, 11);
                        dr["CantidadUMA"] = GetDecimal(row, 12);
                        dr["UnidadMedidaAlmacen"] = GetString(row, 13);
                        dr["PrecioNeto"] = GetDecimal(row, 14, 2); // Redondeo a 2 decimales
                        dr["Moneda"] = GetString(row, 15);
                        dr["CantidadBase"] = GetDecimal(row, 16);
                        dr["CtdPrevPendiente"] = GetDecimal(row, 17);
                        dr["PorEntregarCantidad"] = GetDecimal(row, 18);
                        dr["PorEntregarValor"] = GetDecimal(row, 19, 2); // Redondeo a 2 decimales
                        dr["PorCalcularCantidad"] = GetDecimal(row, 20);
                        dr["TipoImputacion"] = GetString(row, 21);
                        dr["PorCalcularValor"] = GetDecimal(row, 22, 2); // Redondeo a 2 decimales
                        dr["IndLiberacion"] = GetString(row, 23);
                        dr["DocumentoCompras"] = GetString(row, 24);
                        dr["ProveedorCentroSuministrador"] = GetString(row, 25);

                        dt.Rows.Add(dr);
                    }

                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("La hoja 'General' no contiene registros válidos (después de los encabezados).");
                    }

                    // --- 5. Obtener ConnectionString (Tu método) ---
                    string efConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Portal_2_0Entities"].ConnectionString;
                    var builder = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(efConnectionString);
                    string connectionString = builder.ProviderConnectionString;

                    // --- 6. Ejecutar SqlBulkCopy ---
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
                    {
                        bulkCopy.DestinationTableName = "dbo.OT_Datos";
                        bulkCopy.BulkCopyTimeout = 300; // 5 minutos

                        // Mapeo explícito
                        foreach (DataColumn col in dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.WriteToServer(dt);
                    }

                    // --- 7. Actualizar el conteo y Commitear ---
                    nuevaCarga.TotalRegistros = dt.Rows.Count;
                    db.Entry(nuevaCarga).State = EntityState.Modified;
                    db.SaveChanges();

                    dbContextTransaction.Commit(); // ¡Todo salió bien!

                    TempData["Mensaje"] = $"¡Carga exitosa! Se guardaron {nuevaCarga.TotalRegistros} registros.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback(); // Revertir todo

                    // Preparamos el modelo para recargar la vista con el error
                    model.CargasExistentes = db.OT_Cargas.OrderByDescending(c => c.FechaCarga).ToList();
                    model.AlertMessage = $"Error al procesar el archivo: {ex.Message}";
                    model.AlertType = "danger";
                    return View("Index", model);
                }
            }
        }

        //
        // POST: /OT_Cargas/Delete/5
        // Elimina una carga y sus datos (gracias a ON DELETE CASCADE)
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                OT_Cargas carga = db.OT_Cargas.Find(id);
                if (carga == null)
                {
                    return Json(new { success = false, message = "No se encontró la carga a eliminar." });
                }

                db.OT_Cargas.Remove(carga);
                db.SaveChanges(); // <-- ON DELETE CASCADE hace la magia aquí

                return Json(new { success = true, message = "Carga eliminada correctamente." });
            }
            catch (Exception ex)
            {
                // Manejar error de borrado (ej. si la FK estuviera mal configurada)
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }

        #region (Helpers de Lectura - Basados en tu ejemplo)

        // Estos métodos ayudan a parsear de forma segura los datos del Excel

        private string GetString(DataRow row, int colIndex)
        {
            if (row.IsNull(colIndex)) return null;
            return row[colIndex].ToString().Trim();
        }

        private int? GetInt(DataRow row, int colIndex)
        {
            if (row.IsNull(colIndex)) return null;
            if (int.TryParse(row[colIndex].ToString(), out int result))
                return result;
            return null;
        }

        private decimal? GetDecimal(DataRow row, int colIndex, int? decimals = null)
        {
            if (row.IsNull(colIndex)) return null;
            if (decimal.TryParse(row[colIndex].ToString(), out decimal result))
            {
                if (decimals.HasValue)
                    return Math.Round(result, decimals.Value);
                return result;
            }
            return null;
        }

        private DateTime? GetDate(DataRow row, int colIndex)
        {
            if (row.IsNull(colIndex)) return null;

            // ExcelDataReader a veces lee fechas como OADate (double)
            if (row[colIndex] is double d)
            {
                return DateTime.FromOADate(d);
            }
            // Otras veces como string
            if (DateTime.TryParse(row[colIndex].ToString(), out DateTime result))
            {
                return result;
            }
            return null;
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
}