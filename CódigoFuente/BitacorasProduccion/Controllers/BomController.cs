using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Org.BouncyCastle.Asn1.Ocsp;
using Portal_2_0.Models;
using System.Data.SqlClient;


namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BomController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: Bom
        //public ActionResult Index(string material, int pagina = 1)
        //{
        //    if (TieneRol(TipoRoles.ADMIN) || TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
        //    {
        //        //mensaje en caso de crear, editar, etc
        //        if (TempData["Mensaje"] != null)
        //        {
        //            ViewBag.MensajeAlert = TempData["Mensaje"];
        //        }

        //        var cantidadRegistrosPorPagina = 20; // parámetro


        //        var listaBom = db.bom_en_sap.Where(x => String.IsNullOrEmpty(material) || x.Material.Contains(material)).OrderBy(x => x.Material)
        //            .Skip((pagina - 1) * cantidadRegistrosPorPagina)
        //            .Take(cantidadRegistrosPorPagina).ToList();

        //        var totalDeRegistros = db.bom_en_sap.Where(x => String.IsNullOrEmpty(material) || x.Material.Contains(material)).Count();

        //        System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
        //        routeValues["material"] = material;

        //        Paginacion paginacion = new Paginacion
        //        {
        //            PaginaActual = pagina,
        //            TotalDeRegistros = totalDeRegistros,
        //            RegistrosPorPagina = cantidadRegistrosPorPagina,
        //            ValoresQueryString = routeValues
        //        };

        //        ViewBag.Paginacion = paginacion;

        //        return View(listaBom);
        //    }
        //    else
        //    {
        //        return View("../Home/ErrorPermisos");
        //    }

        //}

        // GET: Bom/CargaBom/5
        public ActionResult CargaBom()
        {
            if (TieneRol(TipoRoles.ADMIN) || TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }



        // POST: Dante/CargaBom/5
        //[HttpPost]
        //public ActionResult CargaBom(ExcelViewModel excelViewModel, FormCollection collection)
        //{
        //    // 1. Validaciones del archivo (sin cambios)
        //    var file = Request.Files["PostedFile"];
        //    if (file == null || file.ContentLength == 0)
        //    {
        //        ModelState.AddModelError("", "Debe seleccionar un archivo.");
        //        return View(excelViewModel);
        //    }

        //    const int MAX_BYTES = 8 * 1024 * 1024; // 8 MB
        //    if (file.ContentLength > MAX_BYTES)
        //    {
        //        ModelState.AddModelError("", "Sólo se permiten archivos menores a 8 MB.");
        //        return View(excelViewModel);
        //    }

        //    var extension = Path.GetExtension(file.FileName)?.ToUpperInvariant();
        //    if (extension != ".XLS" && extension != ".XLSX")
        //    {
        //        ModelState.AddModelError("", "Sólo se permiten archivos Excel (.xls / .xlsx).");
        //        return View(excelViewModel);
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return View(excelViewModel);
        //    }

        //    // --- INICIA EL REEMPLAZO ---
        //    // Envolvemos TODAS las operaciones de BD en una única transacción
        //    using (var transaction = db.Database.BeginTransaction())
        //    {
        //        var sqlConnection = db.Database.Connection as SqlConnection;
        //        var sqlTransaction = transaction.UnderlyingTransaction as SqlTransaction;

        //        try
        //        {
        //            // 2. Leer datos de Excel
        //            bool estructuraValida = false;
        //            var lista = UtilExcel.LeeBom(file, ref estructuraValida);

        //            if (!estructuraValida)
        //            {
        //                ModelState.AddModelError("", "El archivo no cumple con la estructura esperada.");
        //                return View(excelViewModel);
        //            }

        //            lista = lista.Distinct().ToList();

        //            // --- OPTIMIZACIÓN PARTE 1: Carga de bom_en_sap ---

        //            // 3. Vaciar la tabla
        //            db.Database.ExecuteSqlCommand("TRUNCATE TABLE bom_en_sap");

        //            // 4. Insertar todos los nuevos registros con SqlBulkCopy
        //            System.Data.DataTable dataTableBom = UtilBulkCopy.ToDataTable(lista);
        //            using (var bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, sqlTransaction))
        //            {
        //                bulkCopy.DestinationTableName = "bom_en_sap";
        //                // Mapeo manual (aunque coinciden, es más seguro)
        //                bulkCopy.ColumnMappings.Add("Material", "Material");
        //                bulkCopy.ColumnMappings.Add("Plnt", "Plnt");
        //                bulkCopy.ColumnMappings.Add("BOM", "BOM");
        //                bulkCopy.ColumnMappings.Add("AltBOM", "AltBOM");
        //                bulkCopy.ColumnMappings.Add("Item", "Item");
        //                bulkCopy.ColumnMappings.Add("Component", "Component");
        //                bulkCopy.ColumnMappings.Add("Quantity", "Quantity");
        //                bulkCopy.ColumnMappings.Add("Un", "Un");
        //                bulkCopy.ColumnMappings.Add("Created", "Created");
        //                bulkCopy.ColumnMappings.Add("LastDateUsed", "LastDateUsed");

        //                bulkCopy.WriteToServer(dataTableBom);
        //            }


        //            // --- OPTIMIZACIÓN PARTE 2: Cálculo y actualización de Pesos ---

        //            // 5. Agrupar la lista (lógica de negocio sin cambios)
        //            var bomGroups = lista.GroupBy(b => new { b.Material, b.Plnt });
        //            var pesosDict = db.bom_pesos.ToDictionary(p => $"{p.plant}|{p.material}", p => p);
        //            var pesosParaSincronizar = new List<bom_pesos>();

        //            foreach (var group in bomGroups)
        //            {
        //                // (Toda tu lógica de cálculo de peso_neto y peso_bruto se mantiene igual)
        //                var listTemporalBOM = group.ToList();
        //                DateTime? fechaUso = listTemporalBOM.Max(b => b.LastDateUsed);
        //                DateTime? fechaCreacion = listTemporalBOM.Max(b => b.Created);
        //                double? peso_neto = null;
        //                double? peso_bruto = null;
        //                if (fechaUso.HasValue)
        //                {
        //                    peso_bruto = listTemporalBOM.Where(x => x.LastDateUsed == fechaUso).Max(x => x.Quantity);
        //                    bool duplicado = listTemporalBOM.Count(x => x.LastDateUsed == fechaUso && x.Quantity == peso_bruto) > 1;
        //                    if (duplicado)
        //                    {
        //                        peso_neto = listTemporalBOM.Where(x => x.LastDateUsed == fechaUso && x.Quantity != -0.001).Select(x => x.Quantity).Distinct().Sum();
        //                    }
        //                    else
        //                    {
        //                        peso_neto = peso_bruto + listTemporalBOM.Where(x => x.LastDateUsed == fechaUso && x.Quantity < -0.001).Sum(x => x.Quantity);
        //                        if (group.Key.Material == "HD10928")
        //                        {
        //                            peso_bruto = peso_neto;
        //                        }
        //                    }
        //                }
        //                else if (fechaCreacion.HasValue)
        //                {
        //                    peso_bruto = listTemporalBOM.Where(x => x.Created == fechaCreacion).Max(x => x.Quantity);
        //                    peso_neto = peso_bruto + listTemporalBOM.Where(x => x.Created == fechaCreacion && x.Quantity < -0.001).Sum(x => x.Quantity);
        //                }
        //                if (peso_neto.HasValue && peso_bruto.HasValue)
        //                {
        //                    if (pesosDict.TryGetValue($"{group.Key.Plnt}|{group.Key.Material}", out var pesoExistente))
        //                    {
        //                        if (pesoExistente.net_weight != peso_neto.Value || pesoExistente.gross_weight != peso_bruto.Value)
        //                        {
        //                            pesoExistente.net_weight = peso_neto.Value;
        //                            pesoExistente.gross_weight = peso_bruto.Value;
        //                            pesosParaSincronizar.Add(pesoExistente);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        pesosParaSincronizar.Add(new bom_pesos
        //                        {
        //                            plant = group.Key.Plnt,
        //                            material = group.Key.Material,
        //                            gross_weight = peso_bruto.Value,
        //                            net_weight = peso_neto.Value,
        //                        });
        //                    }
        //                }
        //            }

        //            // 6. Sincronizar (Insertar/Actualizar) pesos con SqlBulkCopy + MERGE
        //            if (pesosParaSincronizar.Any())
        //            {
        //                // 6a. Convertir la lista de pesos a DataTable
        //                System.Data.DataTable dataTablePesos = UtilBulkCopy.ToDataTable(pesosParaSincronizar);

        //                // 6b. Subir los datos a una tabla temporal (#TempPesos)
        //                //      Usamos la misma transacción
        //                using (var bulkCopyPesos = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, sqlTransaction))
        //                {
        //                    bulkCopyPesos.DestinationTableName = "#TempPesos";

        //                    // Mapeamos SÓLO las 4 columnas de datos, ignorando id, CreatedIn, SYSSTART, etc.
        //                    bulkCopyPesos.ColumnMappings.Add("plant", "plant");
        //                    bulkCopyPesos.ColumnMappings.Add("material", "material"); // <-- CORREGIDO
        //                    bulkCopyPesos.ColumnMappings.Add("gross_weight", "gross_weight"); // <-- CORREGIDO
        //                    bulkCopyPesos.ColumnMappings.Add("net_weight", "net_weight"); // <-- CORREGIDO


        //                    // Creamos la tabla temporal primero (SqlBulkCopy no puede crearla si no existe)
        //                    // Nota: Usamos varchar(10) para material basado en tu tabla.
        //                    db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, @"
        //                        CREATE TABLE #TempPesos (
        //                            [plant] [varchar](4) NOT NULL,
        //                            [material] [varchar](10) NOT NULL,
        //                            [gross_weight] [float] NOT NULL,
        //                            [net_weight] [float] NOT NULL
        //                        );");

        //                    bulkCopyPesos.WriteToServer(dataTablePesos);
        //                }

        //                // 6c. Ejecutar el comando MERGE
        //                string mergeSql = @"
        //                    MERGE INTO bom_pesos AS T
        //                    USING #TempPesos AS S
        //                    ON (T.plant = S.plant AND T.material = S.material)
        //                    WHEN MATCHED THEN
        //                        UPDATE SET
        //                            T.gross_weight = S.gross_weight,
        //                            T.net_weight = S.net_weight
        //                    WHEN NOT MATCHED BY TARGET THEN
        //                        INSERT (plant, material, gross_weight, net_weight)
        //                        VALUES (S.plant, S.material, S.gross_weight, S.net_weight);
        //                ";

        //                db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, mergeSql);
        //            }

        //            // 7. Si todo salió bien, confirma la transacción completa
        //            transaction.Commit();

        //            // 8. Preparar mensaje final
        //            TempData["Mensaje"] = new MensajesSweetAlert(
        //                $"Proceso completado. Se cargaron {lista.Count} registros de BOM y se actualizaron/crearon {pesosParaSincronizar.Count} registros de pesos.",
        //                TipoMensajesSweetAlerts.SUCCESS);

        //            return RedirectToAction("Index");
        //        }
        //        catch (Exception ex)
        //        {
        //            // Si algo falla (TRUNCATE, BulkCopy 1, o BulkCopy/MERGE 2), revierte todo
        //            transaction.Rollback();

        //            // Manejo de errores detallado
        //            var errorMessage = new StringBuilder();
        //            errorMessage.AppendLine("Ocurrió un error al procesar el archivo. Detalles:");
        //            errorMessage.AppendLine($"Error principal: {ex.Message}");
        //            Exception inner = ex.InnerException;
        //            int nivel = 1;
        //            while (inner != null)
        //            {
        //                errorMessage.AppendLine($"--- Error Interno Nivel {nivel} ---");
        //                errorMessage.AppendLine(inner.Message);
        //                inner = inner.InnerException;
        //                nivel++;
        //            }
        //            ModelState.AddModelError("", errorMessage.ToString());
        //            return View(excelViewModel);
        //        }
        //    }
        //    // --- FIN DEL REEMPLAZO ---
        //}
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
