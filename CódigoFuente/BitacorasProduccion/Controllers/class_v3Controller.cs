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
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class class_v3Controller : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: class_v3
        public ActionResult Index(string material, int pagina = 1)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS) || TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //en caso de que material este vacio o contenga el parameto material

                var lista = db.class_v3.Where(x => String.IsNullOrEmpty(material) || x.Object.Contains(material)).OrderBy(x => x.Object)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();
                var totalDeRegistros = db.class_v3.Where(x => String.IsNullOrEmpty(material) || x.Object.Contains(material)).Count();

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

                return View(lista);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }
        // GET: Bom/CargaClass/5
        public ActionResult CargaClass()
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

        // POST: Dante/CargaClass/5
        [HttpPost]
        public ActionResult CargaClass(ExcelViewModel excelViewModel, FormCollection collection)
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

            // Si el modelo no es válido por alguna otra razón (ej. Data Annotations)
            if (!ModelState.IsValid)
            {
                return View(excelViewModel);
            }

            try
            {
                // 2. Leer datos de Excel
                bool estructuraValida = false;
                var lista = UtilExcel.LeeClass(file, ref estructuraValida);

                if (!estructuraValida)
                {
                    ModelState.AddModelError("", "El archivo no cumple con la estructura esperada.");
                    return View(excelViewModel);
                }

                // --- LÓGICA DE TRUNCATE E INSERT ---

                // 3. Vaciar la tabla de forma masiva y ultra-rápida.
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE class_v3");

                // 4. Insertar todos los nuevos registros de forma masiva.
                db.BulkInsert(lista);

                // 5. Preparar el mensaje de éxito.
                int creados = lista.Count;
                TempData["Mensaje"] = new MensajesSweetAlert(
                    $"Tabla limpiada exitosamente. Se crearon {creados} nuevos registros.",
                    TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Manejo de errores detallado, incluyendo excepciones internas.
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
