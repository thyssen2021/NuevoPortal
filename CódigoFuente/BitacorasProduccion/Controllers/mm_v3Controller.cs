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

            // 1. Validar archivo
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
                // 2. Leer datos de Excel
                bool estructuraValida = false;
                var incomingList = UtilExcel.LeeMM(file, ref estructuraValida);
                if (!estructuraValida)
                {
                    ModelState.AddModelError("", "El archivo no cumple con la estructura esperada.");
                    return View(excelViewModel);
                }

                // 3. Preparar diccionarios de clave única "Material|Plnt"
                var dbSet = db.mm_v3;
                var existingList = dbSet.ToList();
                var existingDict = existingList.ToDictionary(
                    x => $"{x.Material}|{x.Plnt}",
                    x => x);
                var incomingDict = incomingList.ToDictionary(
                    x => $"{x.Material}|{x.Plnt}",
                    x => x);

                int creados = 0;
                int actualizados = 0;
                int eliminados = 0;

                // 4. Upsert: actualizaciones y altas
                foreach (var kv in incomingDict)
                {
                    var key = kv.Key;
                    var newItem = kv.Value;

                    if (existingDict.TryGetValue(key, out var dbItem))
                    {
                        // Sólo actualiza si algo cambió (Equals compara propiedades)
                        if (!dbItem.Equals(newItem))
                        {
                            db.Entry(dbItem).CurrentValues.SetValues(newItem);
                            actualizados++;
                            try {
                                db.SaveChanges();
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        dbSet.Add(newItem);
                        creados++;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch { }
                    }
                }

                // 5. Borrado de los que ya no aparecen en Excel
                var keysToDelete = existingDict.Keys.Except(incomingDict.Keys);
                foreach (var key in keysToDelete)
                {
                    var dbItem = existingDict[key];
                    dbSet.Remove(dbItem);
                    eliminados++;
                }

                // 6. Guardar todo con un sólo SaveChanges
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(
                    $"Creado: {creados} • Actualizado: {actualizados} • Eliminado: {eliminados}",
                    TipoMensajesSweetAlerts.INFO);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error al procesar el archivo: {ex.Message}");
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
