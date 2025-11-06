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
using System.Data.SqlClient;
using System.Reflection;
using System.Text; 


namespace Portal_2_0.Controllers
{
    public class mm_v3Controller : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: mm_v3
        //public ActionResult Index(string material, string tipoMaterial, int pagina = 1)
        //{
        //    if (TieneRol(TipoRoles.ADMIN) || TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
        //    {
        //        //mensaje en caso de crear, editar, etc
        //        if (TempData["Mensaje"] != null)
        //        {
        //            ViewBag.MensajeAlert = TempData["Mensaje"];
        //        }

        //        var cantidadRegistrosPorPagina = 20; // parámetro

        //        //en caso de que material este vacio o contenga el parameto material              

        //        var listaBom = db.mm_v3.Where(
        //                x => (String.IsNullOrEmpty(material) || x.Material.Contains(material))
        //                && (String.IsNullOrEmpty(tipoMaterial) || x.Type_of_Material.Contains(tipoMaterial))
        //                )
        //            .OrderBy(x => x.Material)
        //            .Skip((pagina - 1) * cantidadRegistrosPorPagina)
        //            .Take(cantidadRegistrosPorPagina).ToList();

        //        var totalDeRegistros = db.mm_v3.Where(x => (String.IsNullOrEmpty(material) || x.Material.Contains(material))
        //               && (String.IsNullOrEmpty(tipoMaterial) || x.Type_of_Material.Contains(tipoMaterial))).Count();

        //        System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
        //        routeValues["material"] = material;
        //        routeValues["tipoMaterial"] = tipoMaterial;

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
        //[HttpPost]
        //public ActionResult CargaMM(ExcelViewModel excelViewModel, FormCollection collection)
        //{
        //    if (!ModelState.IsValid)
        //        return View(excelViewModel);

        //    // 1. Validaciones de archivo (sin cambios)
        //    var file = Request.Files["PostedFile"];
        //    if (file == null || file.ContentLength == 0)
        //    {
        //        ModelState.AddModelError("", "Debe seleccionar un archivo.");
        //        return View(excelViewModel);
        //    }
        //    const int MAX_BYTES = 15 * 1024 * 1024;
        //    if (file.ContentLength > MAX_BYTES)
        //    {
        //        ModelState.AddModelError("", "Sólo se permiten archivos menores a 15 MB.");
        //        return View(excelViewModel);
        //    }
        //    var ext = Path.GetExtension(file.FileName)?.ToUpperInvariant();
        //    if (ext != ".XLS" && ext != ".XLSX")
        //    {
        //        ModelState.AddModelError("", "Sólo se permiten archivos Excel (.xls / .xlsx).");
        //        return View(excelViewModel);
        //    }

        //    // --- INICIA LÓGICA DE TRANSACCIÓN Y SqlBulkCopy ---
        //    using (var transaction = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            // 2. Leer datos de Excel (sin cambios)
        //            bool estructuraValida = false;
        //            var incomingList = UtilExcel.LeeMM(file, ref estructuraValida);
        //            if (!estructuraValida)
        //            {
        //                ModelState.AddModelError("", "El archivo no cumple con la estructura esperada.");
        //                return View(excelViewModel);
        //            }

        //            // 3. PASO 1: Vaciar la tabla (dentro de la transacción)
        //            db.Database.ExecuteSqlCommand("TRUNCATE TABLE mm_v3");

        //            // 4. PASO 2: Convertir la lista a DataTable
        //            System.Data.DataTable dataTable = UtilBulkCopy.ToDataTable(incomingList);

        //            // 5. PASO 3: Insertar todos los nuevos registros con SqlBulkCopy
        //            var sqlConnection = db.Database.Connection as SqlConnection;
        //            var sqlTransaction = transaction.UnderlyingTransaction as SqlTransaction;

        //            using (var bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, sqlTransaction))
        //            {
        //                bulkCopy.DestinationTableName = "mm_v3";

        //                // --- INICIA EL MAPEO MANUAL ---
        //                // (Izquierda: C# Property Name)
        //                // (Derecha: SQL Column Name)
        //                bulkCopy.ColumnMappings.Add("Material", "Material");
        //                bulkCopy.ColumnMappings.Add("Plnt", "Plnt");
        //                bulkCopy.ColumnMappings.Add("MS", "MS");
        //                bulkCopy.ColumnMappings.Add("Material_Description", "Material Description");
        //                bulkCopy.ColumnMappings.Add("Type_of_Material", "Type of Material");
        //                bulkCopy.ColumnMappings.Add("Type_of_Metal", "Type of Metal");
        //                bulkCopy.ColumnMappings.Add("Old_material_no_", "Old material no#");
        //                bulkCopy.ColumnMappings.Add("Head_and_Tails_Scrap_Conciliation", "Head and Tails Scrap Conciliation");
        //                bulkCopy.ColumnMappings.Add("Engineering_Scrap_conciliation", "Engineering Scrap conciliation");
        //                bulkCopy.ColumnMappings.Add("Business_Model", "Business Model");
        //                bulkCopy.ColumnMappings.Add("Re_application", "Re-application");
        //                bulkCopy.ColumnMappings.Add("IHS_number_1", "IHS number 1");
        //                bulkCopy.ColumnMappings.Add("IHS_number_2", "IHS number 2");
        //                bulkCopy.ColumnMappings.Add("IHS_number_3", "IHS number 3");
        //                bulkCopy.ColumnMappings.Add("IHS_number_4", "IHS number 4");
        //                bulkCopy.ColumnMappings.Add("IHS_number_5", "IHS number 5");
        //                bulkCopy.ColumnMappings.Add("Type_of_Selling", "Type of Selling");
        //                bulkCopy.ColumnMappings.Add("Package_Pieces", "Package Pieces");
        //                bulkCopy.ColumnMappings.Add("Gross_weight", "Gross weight");
        //                bulkCopy.ColumnMappings.Add("Un_", "Un#");
        //                bulkCopy.ColumnMappings.Add("Net_weight", "Net weight");
        //                bulkCopy.ColumnMappings.Add("Un_1", "Un#1");
        //                bulkCopy.ColumnMappings.Add("Thickness", "Thickness");
        //                bulkCopy.ColumnMappings.Add("Width", "Width");
        //                bulkCopy.ColumnMappings.Add("Advance", "Advance");
        //                bulkCopy.ColumnMappings.Add("Head_and_Tail_allowed_scrap", "Head and Tail allowed scrap");
        //                bulkCopy.ColumnMappings.Add("Pieces_per_car", "Pieces per car");
        //                bulkCopy.ColumnMappings.Add("Initial_Weight", "Initial Weight");
        //                bulkCopy.ColumnMappings.Add("Min_Weight", "Min Weight");
        //                bulkCopy.ColumnMappings.Add("Maximum_Weight", "Maximum Weight");
        //                bulkCopy.ColumnMappings.Add("activo", "activo");
        //                bulkCopy.ColumnMappings.Add("num_piezas_golpe", "num_piezas_golpe");
        //                bulkCopy.ColumnMappings.Add("unidad_medida", "unidad_medida");
        //                bulkCopy.ColumnMappings.Add("size_dimensions", "size_dimensions");
        //                bulkCopy.ColumnMappings.Add("material_descripcion_es", "material_descripcion_es");
        //                bulkCopy.ColumnMappings.Add("angle_a", "angle_a");
        //                bulkCopy.ColumnMappings.Add("angle_b", "angle_b");
        //                bulkCopy.ColumnMappings.Add("real_net_weight", "real_net_weight");
        //                bulkCopy.ColumnMappings.Add("real_gross_weight", "real_gross_weight");
        //                bulkCopy.ColumnMappings.Add("double_pieces", "double_pieces");
        //                bulkCopy.ColumnMappings.Add("coil_position", "coil_position");
        //                bulkCopy.ColumnMappings.Add("maximum_weight_tol_positive", "maximum_weight_tol_positive");
        //                bulkCopy.ColumnMappings.Add("maximum_weight_tol_negative", "maximum_weight_tol_negative");
        //                bulkCopy.ColumnMappings.Add("minimum_weight_tol_positive", "minimum_weight_tol_positive");
        //                bulkCopy.ColumnMappings.Add("minimum_weight_tol_negative", "minimum_weight_tol_negative");
        //                bulkCopy.ColumnMappings.Add("Almacen_Norte", "Almacen_Norte");
        //                bulkCopy.ColumnMappings.Add("Tipo_de_Transporte", "Tipo_de_Transporte");
        //                bulkCopy.ColumnMappings.Add("Tkmm_SOP", "Tkmm_SOP");
        //                bulkCopy.ColumnMappings.Add("Tkmm_EOP", "Tkmm_EOP");
        //                bulkCopy.ColumnMappings.Add("Pieces_Pac", "Pieces_Pac");
        //                bulkCopy.ColumnMappings.Add("Stacks_Pac", "Stacks_Pac");
        //                bulkCopy.ColumnMappings.Add("Type_of_Pallet", "Type_of_Pallet");
        //                // --- FIN DEL MAPEO MANUAL ---

        //                // Escribir los datos en el servidor
        //                bulkCopy.WriteToServer(dataTable);
        //            }

        //            // 6. Si todo salió bien, confirma la transacción
        //            transaction.Commit();

        //            // 7. PASO 3: Realizar el conteo.
        //            int creados = incomingList.Count;

        //            TempData["Mensaje"] = new MensajesSweetAlert(
        //                $"Tabla limpiada. Se crearon {creados} nuevos registros.",
        //                TipoMensajesSweetAlerts.SUCCESS);

        //            return RedirectToAction("Index");
        //        }
        //        catch (Exception ex)
        //        {
        //            // Si algo falla, revierte la transacción
        //            transaction.Rollback();

        //            // El bloque para mostrar errores internos
        //            var errorMessage = new System.Text.StringBuilder();
        //            errorMessage.AppendLine($"Ocurrió un error al procesar el archivo. Detalles:");
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
