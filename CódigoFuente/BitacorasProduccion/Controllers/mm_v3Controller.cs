using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;
using Z.BulkOperations;
using Z.EntityFramework.Extensions;


namespace Portal_2_0.Controllers
{
    public class mm_v3Controller : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: mm_v3
        public ActionResult Index(string material, string tipoMaterial, int pagina = 1)
        {
            if (TieneRol(TipoRoles.ADMIN) || TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //en caso de que material este vacio o contenga el parameto material              

                var listaBom = db.mm_v3.Where(
                        x => (String.IsNullOrEmpty(material) || x.Material.Contains(material))
                        && (String.IsNullOrEmpty(tipoMaterial) || x.Type_of_Material.Contains(tipoMaterial))
                        )
                    .OrderBy(x => x.Material)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.mm_v3.Where(x => (String.IsNullOrEmpty(material) || x.Material.Contains(material))
                       && (String.IsNullOrEmpty(tipoMaterial) || x.Type_of_Material.Contains(tipoMaterial))).Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["material"] = material;
                routeValues["tipoMaterial"] = tipoMaterial;

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

        // GET: Bom/CargaMM/5
        public ActionResult CargaMM()
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

        // POST: Dante/CargaMM/5
        [HttpPost]
        public ActionResult CargaMM(ExcelViewModel excelViewModel, FormCollection collection)
        {
            if (!ModelState.IsValid)
                return View(excelViewModel);

            // 1. Validaciones de archivo (sin cambios)
            var file = Request.Files["PostedFile"];
            if (file == null || file.ContentLength == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un archivo.");
                return View(excelViewModel);
            }
            const int MAX_BYTES = 15 * 1024 * 1024;
            if (file.ContentLength > MAX_BYTES)
            {
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 15 MB.");
                return View(excelViewModel);
            }
            var ext = Path.GetExtension(file.FileName)?.ToUpperInvariant();
            if (ext != ".XLS" && ext != ".XLSX")
            {
                ModelState.AddModelError("", "Sólo se permiten archivos Excel (.xls / .xlsx).");
                return View(excelViewModel);
            }

            try
            {
                // 2. Leer datos de Excel (sin cambios)
                bool estructuraValida = false;
                var incomingList = UtilExcel.LeeMM(file, ref estructuraValida);
                if (!estructuraValida)
                {
                    ModelState.AddModelError("", "El archivo no cumple con la estructura esperada.");
                    return View(excelViewModel);
                }

                // --- CAMBIO INICIA (ESTRATEGIA CON TRUNCATE) ---

                // 3. PASO 1: Vaciar la tabla de forma masiva y ultra-rápida.
                // Se ejecuta como un comando SQL directo sobre la base de datos.
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE mm_v3");

                // 4. PASO 2: Insertar todos los nuevos registros.
                // Usamos BulkInsert porque es más directo y eficiente que BulkMerge en una tabla vacía.
                db.BulkInsert(incomingList);

                // 5. PASO 3: Realizar el conteo.
                // Ahora es más simple: todos los registros del Excel fueron creados.
                int creados = incomingList.Count;

                TempData["Mensaje"] = new MensajesSweetAlert(
                    $"Tabla limpiada. Se crearon {creados} nuevos registros.",
                    TipoMensajesSweetAlerts.SUCCESS);

                // --- CAMBIO TERMINA ---

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // El bloque para mostrar errores internos que hicimos antes sigue siendo válido y útil.
                var errorMessage = new System.Text.StringBuilder();
                errorMessage.AppendLine($"Ocurrió un error al procesar el archivo. Detalles:");
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
