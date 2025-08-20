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

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BomController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: Bom
        public ActionResult Index(string material, int pagina = 1)
        {
            if (TieneRol(TipoRoles.ADMIN) || TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listaBom = db.bom_en_sap.Where(x => String.IsNullOrEmpty(material) || x.Material.Contains(material)).OrderBy(x => x.Material)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.bom_en_sap.Where(x => String.IsNullOrEmpty(material) || x.Material.Contains(material)).Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;

                return View(listaBom);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

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
        [HttpPost]
        public ActionResult CargaBom(ExcelViewModel excelViewModel, FormCollection collection)
        {
            // 1. Validaciones del archivo (buenas prácticas de MVC)
            var file = Request.Files["PostedFile"];
            if (file == null || file.ContentLength == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un archivo.");
                return View(excelViewModel);
            }

            const int MAX_BYTES = 8 * 1024 * 1024; // 8 MB
            if (file.ContentLength > MAX_BYTES)
            {
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 8 MB.");
                return View(excelViewModel);
            }

            var extension = Path.GetExtension(file.FileName)?.ToUpperInvariant();
            if (extension != ".XLS" && extension != ".XLSX")
            {
                ModelState.AddModelError("", "Sólo se permiten archivos Excel (.xls / .xlsx).");
                return View(excelViewModel);
            }

            if (!ModelState.IsValid)
            {
                return View(excelViewModel);
            }

            try
            {
                // 2. Leer datos de Excel
                bool estructuraValida = false;
                var lista = UtilExcel.LeeBom(file, ref estructuraValida);

                if (!estructuraValida)
                {
                    ModelState.AddModelError("", "El archivo no cumple con la estructura esperada.");
                    return View(excelViewModel);
                }

                // Quita los repetidos que puedan venir en el archivo.
                lista = lista.Distinct().ToList();

                // --- OPTIMIZACIÓN PARTE 1: Carga de bom_en_sap ---

                // 3. Vaciar la tabla de forma masiva
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE bom_en_sap");

                // 4. Insertar todos los nuevos registros de forma masiva
                db.BulkInsert(lista);

                // --- OPTIMIZACIÓN PARTE 2: Cálculo y actualización de Pesos ---

                // 5. Agrupar la lista recién insertada por material para procesar eficientemente
                var bomGroups = lista.GroupBy(b => new { b.Material, b.Plnt });

                // Traemos los pesos existentes a un diccionario para búsquedas ultra-rápidas en memoria
                var pesosDict = db.bom_pesos.ToDictionary(p => $"{p.plant}|{p.material}", p => p);

                var pesosParaSincronizar = new List<bom_pesos>();

                foreach (var group in bomGroups)
                {
                    var listTemporalBOM = group.ToList();

                    // La lógica de cálculo de pesos se mantiene, pero ahora opera sobre grupos pequeños.
                    DateTime? fechaUso = listTemporalBOM.Max(b => b.LastDateUsed);
                    DateTime? fechaCreacion = listTemporalBOM.Max(b => b.Created);

                    double? peso_neto = null;
                    double? peso_bruto = null;

                    if (fechaUso.HasValue)
                    {
                        peso_bruto = listTemporalBOM.Where(x => x.LastDateUsed == fechaUso).Max(x => x.Quantity);
                        bool duplicado = listTemporalBOM.Count(x => x.LastDateUsed == fechaUso && x.Quantity == peso_bruto) > 1;

                        if (duplicado)
                        {
                            peso_neto = listTemporalBOM.Where(x => x.LastDateUsed == fechaUso && x.Quantity != -0.001).Select(x => x.Quantity).Distinct().Sum();
                        }
                        else
                        {
                            peso_neto = peso_bruto + listTemporalBOM.Where(x => x.LastDateUsed == fechaUso && x.Quantity < -0.001).Sum(x => x.Quantity);
                            if (group.Key.Material == "HD10928")
                            {
                                peso_bruto = peso_neto;
                            }
                        }
                    }
                    else if (fechaCreacion.HasValue)
                    {
                        peso_bruto = listTemporalBOM.Where(x => x.Created == fechaCreacion).Max(x => x.Quantity);
                        peso_neto = peso_bruto + listTemporalBOM.Where(x => x.Created == fechaCreacion && x.Quantity < -0.001).Sum(x => x.Quantity);
                    }

                    if (peso_neto.HasValue && peso_bruto.HasValue)
                    {
                        // Buscamos en el diccionario (muy rápido) si el peso ya existe
                        if (pesosDict.TryGetValue($"{group.Key.Plnt}|{group.Key.Material}", out var pesoExistente))
                        {
                            // Si existe y cambió, lo actualizamos
                            if (pesoExistente.net_weight != peso_neto.Value || pesoExistente.gross_weight != peso_bruto.Value)
                            {
                                pesoExistente.net_weight = peso_neto.Value;
                                pesoExistente.gross_weight = peso_bruto.Value;
                                pesosParaSincronizar.Add(pesoExistente);
                            }
                        }
                        else
                        {
                            // Si no existe, creamos uno nuevo para insertar
                            pesosParaSincronizar.Add(new bom_pesos
                            {
                                plant = group.Key.Plnt,
                                material = group.Key.Material,
                                gross_weight = peso_bruto.Value,
                                net_weight = peso_neto.Value,
                            });
                        }
                    }
                }

                // 6. Sincronizar (Insertar/Actualizar) todos los pesos calculados en una sola operación masiva.
                if (pesosParaSincronizar.Any())
                {
                    db.BulkMerge(pesosParaSincronizar, options =>
                    {
                        // Define la llave primaria compuesta para que sepa cómo encontrar duplicados.
                        options.ColumnPrimaryKeyExpression = p => new { p.plant, p.material };

                        // SOLUCIÓN: Especificamos EXACTAMENTE qué columnas de nuestro objeto
                        // deben ser enviadas a la base de datos. Todas las demás propiedades
                        // (id, CreatedIn, SYSSTART, SYSEND) serán completamente ignoradas por la librería.
                        // Esto resuelve el error tanto para INSERT como para UPDATE.
                        options.ColumnInputExpression = p => new
                        {
                            p.plant,
                            p.material,
                            p.gross_weight,
                            p.net_weight
                        };
                    });
                }

                // 7. Preparar mensaje final
                TempData["Mensaje"] = new MensajesSweetAlert(
                    $"Proceso completado. Se cargaron {lista.Count} registros de BOM y se actualizaron/crearon {pesosParaSincronizar.Count} registros de pesos.",
                    TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Manejo de errores detallado
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine("Ocurrió un error al procesar el archivo. Detalles:");
                errorMessage.AppendLine($"Error principal: {ex.Message}");
                Exception inner = ex.InnerException;
                int nivel = 1;
                while (inner != null)
                {
                    errorMessage.AppendLine($"--- Error Interno Nivel {nivel} ---");
                    errorMessage.AppendLine(inner.Message);
                    inner = inner.InnerException;
                    nivel++;
                }
                ModelState.AddModelError("", errorMessage.ToString());
                return View(excelViewModel);
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
