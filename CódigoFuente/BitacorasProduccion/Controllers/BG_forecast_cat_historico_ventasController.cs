using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using DocumentFormat.OpenXml.Presentation;
using Portal_2_0.Models;
using SpreadsheetLight;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BG_forecast_cat_historico_ventasController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_forecast_cat_historico_ventas
        public ActionResult Index(string fy, int? id_seccion)
        {

            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            // -- tipo de listado ORIGEN
            List<SelectListItem> selectSeccion = new List<SelectListItem>();

            for (int i = 2010; i < 2040; i++)
            {
                selectSeccion.Add(new SelectListItem() { Text = "FY " + i + "-" + (i + 1), Value = i + "-" + (i + 1) });
            }

            if (string.IsNullOrEmpty(fy))
            {
                fy = (DateTime.Now.Year - 1).ToString() + "-" + (DateTime.Now.Year).ToString();
            }

            ViewBag.SeccionSelected = db.BG_Forecast_cat_secciones_calculo.Find(id_seccion.HasValue ? id_seccion : 1);
            ViewBag.fy = new SelectList(selectSeccion, "Value", "Text", selectedValue: fy);
            ViewBag.id_seccion = new SelectList(db.BG_Forecast_cat_secciones_calculo.Where(x => x.activo), nameof(BG_Forecast_cat_secciones_calculo.id), nameof(BG_Forecast_cat_secciones_calculo.descripcion));

            return View();
        }

        public JsonResult CargaSeccion(string fy, int? id_seccion)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var clientesList = db.BG_forecast_cat_clientes.ToList();

            var jsonData = new object[clientesList.Count()];

            //fecha inicial
            string[] years = fy.Split('-');
            DateTime.TryParseExact(years[0] + "-10-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechainicial);


            for (int i = 0; i < clientesList.Count(); i++)
            {
                //octubre
                var octubre = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var noviembre = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(1) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var diciembre = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(2) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var enero = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(3) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var febrero = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(4) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var marzo = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(5) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var abril = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(6) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var mayo = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(7) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var junio = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(8) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var julio = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(9) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var agosto = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(10) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var septiembre = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(11) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);


                jsonData[i] = new[] {
                    clientesList[i].id.ToString(),  //id cliente
                    fy,                             //clave de fy
                    id_seccion.ToString(),          //clave seccion    
                    clientesList[i].activo ? "1":"0", //cliente activo 
                    clientesList[i].descripcion,     //nombre cliente
                    octubre!= null ? octubre.valor.ToString() : null, //octubre
                    noviembre!= null ? noviembre.valor.ToString() : null, //noviembre
                    diciembre!= null ? diciembre.valor.ToString() : null, //diciembre
                    enero!= null ? enero.valor.ToString() : null, //enero
                    febrero!= null ? febrero.valor.ToString() : null, //febrero
                    marzo!= null ? marzo.valor.ToString() : null, //marzo
                    abril!= null ? abril.valor.ToString() : null, //abril
                    mayo!= null ? mayo.valor.ToString() : null, //mayo
                    junio!= null ? junio.valor.ToString() : null, //junio
                    julio!= null ? julio.valor.ToString() : null, //julio
                    agosto!= null ? agosto.valor.ToString() : null, //agosto
                    septiembre!= null ? septiembre.valor.ToString() : null, //septiembre
                    };
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EnviaVentasForm(string fy_val, int? id_seccion, List<string[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[1];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            List<BG_forecast_cat_historico_ventas> valoresList = ConvierteArrayAValoresVentas(dataListFromTable, fy_val);
            //if (valoresList.Count == 0)
            //    list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //fecha inicial
            string[] years = fy_val.Split('-');
            DateTime.TryParseExact(years[0] + "-10-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechainicial);

            DateTime fechaFinal = fechainicial.AddMonths(12);

            //obtiene el listado Actual de la BD
            List<BG_forecast_cat_historico_ventas> valoresListBD = db.BG_forecast_cat_historico_ventas.Where(x => x.id_seccion == id_seccion
                && x.fecha >= fechainicial && x.fecha < fechaFinal).ToList();


            //crea, modifica o elimina 
            try
            {
                foreach (var valor in valoresList)
                {
                    // si edicion
                    var itemBD = valoresListBD.FirstOrDefault(x => x.id_cliente == valor.id_cliente && x.fecha == valor.fecha);
                    if (itemBD != null) //update
                    {
                        if (itemBD.valor != valor.valor)
                            itemBD.valor = valor.valor;
                    }
                    else //creacion
                    {
                        db.BG_forecast_cat_historico_ventas.Add(new BG_forecast_cat_historico_ventas
                        {
                            id_cliente = valor.id_cliente,
                            id_seccion = valor.id_seccion,
                            fecha = valor.fecha,
                            valor = valor.valor,
                        });
                    }

                }
                //elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = valoresListBD.Where(x => !valoresList.Any(y => y.fecha == x.fecha && y.id_cliente == x.id_cliente)).ToList();
                db.BG_forecast_cat_historico_ventas.RemoveRange(toDeleteList);
                db.SaveChanges();
                list[0] = new { result = "OK", icon = "success", operacion = "CREATE", message = "Se guardaron los cambios correctamente" };

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private List<BG_forecast_cat_historico_ventas> ConvierteArrayAValoresVentas(List<string[]> data, string fy_val)
        {
            List<BG_forecast_cat_historico_ventas> resultado = new List<BG_forecast_cat_historico_ventas> { };

            //variables globales para el metodo
            var BD_SCDM_planta = db.plantas.Where(x => x.activo == true).ToList();

            //listado de encabezados
            string[] encabezados = {
                "ClienteID","FY","SeccionID","Cliente"
            };

            string[] mesesFY = { "Octubre", "Noviembre", "Diciembre", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre" };

            encabezados = encabezados.Concat(mesesFY).ToArray();


            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                //declaración de variables
                int id_cliente = 0;
                int id_seccion = 0;
                DateTime fecha = DateTime.Now;
                string codigoPlanta = string.Empty;

                //id_cliente
                if (int.TryParse(array[Array.IndexOf(encabezados, "ClienteID")], out int id_cliente_result))
                    id_cliente = id_cliente_result;

                //id_seccion
                if (int.TryParse(array[Array.IndexOf(encabezados, "SeccionID")], out int id_seccion_result))
                    id_seccion = id_seccion_result;

                //fecha inicial
                string[] years = fy_val.Split('-');
                DateTime.TryParseExact(years[0] + "-10-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechainicial);

                for (int i = 0; i < 12; i++)
                {
                    double valor = 0;
                    if (double.TryParse(array[Array.IndexOf(encabezados, mesesFY[i])], out double valor_result))
                        valor = valor_result;

                    //solo agrega si tiene valor
                    if (valor != 0)
                        resultado.Add(new BG_forecast_cat_historico_ventas
                        {
                            id_cliente = id_cliente,
                            id_seccion = id_seccion,
                            fecha = fechainicial.AddMonths(i),
                            valor = valor
                        });
                }


            }

            return resultado;
        }

        // GET: BG_forecast_cat_historico_ventas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_forecast_cat_historico_ventas bG_forecast_cat_historico_ventas = db.BG_forecast_cat_historico_ventas.Find(id);
            if (bG_forecast_cat_historico_ventas == null)
            {
                return HttpNotFound();
            }
            return View(bG_forecast_cat_historico_ventas);
        }

        // GET: BG_forecast_cat_historico_ventas/Create
        public ActionResult Create()
        {
            ViewBag.id_cliente = new SelectList(db.BG_forecast_cat_clientes, "id", "descripcion");
            return View();
        }

        // POST: BG_forecast_cat_historico_ventas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_cliente,fecha,valor")] BG_forecast_cat_historico_ventas bG_forecast_cat_historico_ventas)
        {
            if (ModelState.IsValid)
            {
                db.BG_forecast_cat_historico_ventas.Add(bG_forecast_cat_historico_ventas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_cliente = new SelectList(db.BG_forecast_cat_clientes, "id", "descripcion", bG_forecast_cat_historico_ventas.id_cliente);
            return View(bG_forecast_cat_historico_ventas);
        }

        // GET: BG_forecast_cat_historico_ventas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_forecast_cat_historico_ventas bG_forecast_cat_historico_ventas = db.BG_forecast_cat_historico_ventas.Find(id);
            if (bG_forecast_cat_historico_ventas == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cliente = new SelectList(db.BG_forecast_cat_clientes, "id", "descripcion", bG_forecast_cat_historico_ventas.id_cliente);
            return View(bG_forecast_cat_historico_ventas);
        }

        // POST: BG_forecast_cat_historico_ventas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_cliente,fecha,valor")] BG_forecast_cat_historico_ventas bG_forecast_cat_historico_ventas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bG_forecast_cat_historico_ventas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_cliente = new SelectList(db.BG_forecast_cat_clientes, "id", "descripcion", bG_forecast_cat_historico_ventas.id_cliente);
            return View(bG_forecast_cat_historico_ventas);
        }

        // GET: BG_forecast_cat_historico_ventas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_forecast_cat_historico_ventas bG_forecast_cat_historico_ventas = db.BG_forecast_cat_historico_ventas.Find(id);
            if (bG_forecast_cat_historico_ventas == null)
            {
                return HttpNotFound();
            }
            return View(bG_forecast_cat_historico_ventas);
        }

        // POST: BG_forecast_cat_historico_ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BG_forecast_cat_historico_ventas bG_forecast_cat_historico_ventas = db.BG_forecast_cat_historico_ventas.Find(id);
            db.BG_forecast_cat_historico_ventas.Remove(bG_forecast_cat_historico_ventas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region Plantilla Carga Masiva

        [HttpGet]
        public ActionResult DownloadTemplate()
        {
            using (var sl = new SLDocument())
            {
                // Configurar la cultura explícita
                var culture = new CultureInfo("es-MX");

                // Obtener años fiscales (20/21 al actual)
                int startYear = 2020;
                int currentYear = DateTime.Now.Year;
                int endYear = (DateTime.Now.Month >= 10) ? currentYear + 1 : currentYear;
                var fiscalYears = Enumerable.Range(startYear, endYear - startYear + 1)
                                            .Select(y => new { Start = y, End = y + 1 })
                                            .ToList();

                // Obtener todas las secciones activas
                var sections = db.BG_Forecast_cat_secciones_calculo.Where(s => s.activo && !s.aplica_formula).ToList();

                // Mapear clientes
                var clientes = db.BG_forecast_cat_clientes.ToList();

                // Conjunto para asegurar nombres únicos
                var existingSheetNames = new HashSet<string>();
                bool isFirstSheetUsed = false; // Indicador para reutilizar la primera hoja

                foreach (var section in sections)
                {
                    // Crear una hoja por sección
                    string sheetName = SanitizeSheetName(section.descripcion, existingSheetNames);

                    if (!isFirstSheetUsed)
                    {
                        // Renombrar y reutilizar la hoja predeterminada
                        sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, sheetName);
                        isFirstSheetUsed = true;
                    }
                    else
                    {
                        // Agregar nuevas hojas para las siguientes secciones
                        sl.AddWorksheet(sheetName);
                        sl.SelectWorksheet(sheetName);
                    }

                    // Agregar encabezados
                    sl.SetCellValue(1, 1, "ID_Cliente"); // ID del cliente (oculto)
                    sl.HideColumn(1); // Ocultar columna
                    sl.SetCellValue(1, 2, "Cliente"); // Nombre del cliente
                    sl.SetCellValue(1, 3, "ID_Seccion"); // ID de la sección (oculto)
                    sl.HideColumn(3); // Ocultar columna

                    int col = 4; // Columnas para datos
                    foreach (var fy in fiscalYears)
                    {
                  
                        for (int month = 10; month <= 12; month++) // Octubre-Diciembre
                            sl.SetCellValue(1, col++, new DateTime(fy.Start, month, 1).ToString("MMM-yyyy", culture));
                        for (int month = 1; month <= 9; month++) // Enero-Septiembre
                            sl.SetCellValue(1, col++, new DateTime(fy.End, month, 1).ToString("MMM-yyyy", culture));

                        int endColFiscalYear = col - 1; // Columna final del año fiscal

                        // Aplicar bordes alrededor del año fiscal
                        var borderStyle = sl.CreateStyle();
                        borderStyle.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium;
                                            
                        sl.SetCellStyle(2, endColFiscalYear, clientes.Count + 1, endColFiscalYear, borderStyle); // Celdas de datos
                    }

                    // Formatear encabezados
                    var headerStyle = sl.CreateStyle();
                    headerStyle.Font.Bold = true;
                    headerStyle.Font.FontColor = System.Drawing.Color.White;
                    headerStyle.Fill.SetPattern(
                        DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid,
                        System.Drawing.ColorTranslator.FromHtml("#009ff5"),
                        System.Drawing.Color.White
                    );
                    headerStyle.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    headerStyle.Alignment.Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center;
                    sl.SetRowStyle(1, headerStyle);

                    // Mantener fija la columna de clientes al desplazarse horizontalmente
                    sl.FreezePanes(1, 3); // Fijar hasta la fila 1 y columna 3 (clientes)

                    // Obtener los datos históricos para la sección
                    var data = db.BG_forecast_cat_historico_ventas
                                 .Where(h => h.id_seccion == section.id)
                                 .ToList();

                    // Estilo para la primera columna (Clientes)
                    var clientColumnStyle = sl.CreateStyle();
                    clientColumnStyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.LightYellow, System.Drawing.Color.Black);
                    clientColumnStyle.Alignment.WrapText = true;

                    // Determinar el formato del tipo de dato
                    var dataStyle = CreateDataStyle(sl, section.tipo_dato);

                    // Agregar todos los clientes (incluso sin datos históricos)
                    int row = 2;
                    foreach (var cliente in clientes)
                    {
                        sl.SetCellValue(row, 1, cliente.id); // ID del cliente
                        sl.SetCellValue(row, 2, cliente.descripcion); // Nombre del cliente
                        sl.SetCellValue(row, 3, section.id); // ID de la sección
                        sl.SetCellStyle(row, 2, clientColumnStyle); // Aplicar estilo a la columna de cliente


                        // Aplicar formato a todas las columnas de datos para la fila actual
                        int startCol = 4;
                        int endCol = 4 + fiscalYears.Count * 12 - 1; // Última columna de datos para el rango fiscal

                        sl.SetCellStyle(row, startCol, row, endCol, dataStyle);

                        // Si el cliente tiene datos históricos, agregarlos
                        var clienteData = data.Where(d => d.id_cliente == cliente.id).ToList();
                        foreach (var record in clienteData)
                        {
                            int monthOffset = GetMonthOffset(fiscalYears, record.fecha);
                            if (monthOffset > 0)
                            {
                                sl.SetCellValue(row, 4 + monthOffset - 1, record.valor);                  
                            }
                        }

                        row++;
                    }

                    // Ajustar automáticamente las celdas al contenido
                    sl.AutoFitColumn(2); // Ajustar columna de cliente
                    sl.AutoFitColumn(4, col); // Ajustar columnas de datos
                }

                // Seleccionar la primera hoja como activa
                sl.SelectWorksheet(existingSheetNames.First());

                // Descargar el archivo Excel
                var stream = new System.IO.MemoryStream();
                sl.SaveAs(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Plantilla_Historico_Ventas.xlsx");
            }
        }

        private string GetMonthName(int month)
        {
            return new DateTime(1, month, 1).ToString("MMM", System.Globalization.CultureInfo.GetCultureInfo("es-MX"));
        }

        private int GetMonthOffset(IEnumerable<dynamic> fiscalYears, DateTime date)
        {
            int offset = 0;
            foreach (var fy in fiscalYears)
            {
                if (date.Year == fy.Start && date.Month >= 10) // Octubre-Diciembre
                {
                    return offset + date.Month - 10 + 1;
                }
                else if (date.Year == fy.End && date.Month <= 9) // Enero-Septiembre
                {
                    return offset + date.Month + 3;
                }

                // Incrementar el offset por cada año fiscal completo
                offset += 12;
            }

            return 0; // No se encontró el año fiscal
        }

        private string SanitizeSheetName(string name, HashSet<string> existingNames)
        {
            // Reemplazar caracteres no válidos
            var invalidChars = new[] { '\\', '/', '?', '*', '[', ']', ':' };
            foreach (var ch in invalidChars)
            {
                name = name.Replace(ch, '-');
            }

            // Limitar a 31 caracteres
            if (name.Length > 31)
            {
                name = name.Substring(0, 31);
            }

            // Asegurar nombres únicos
            string originalName = name;
            int counter = 1;
            while (existingNames.Contains(name))
            {
                name = originalName.Substring(0, Math.Min(originalName.Length, 31 - counter.ToString().Length - 1)) + $"_{counter}";
                counter++;
            }

            // Registrar el nombre en los nombres existentes
            existingNames.Add(name);

            return name;
        }

        private SLStyle CreateDataStyle(SLDocument sl, string tipoDato)
        {
            var style = sl.CreateStyle();

            if (tipoDato == "CURRENCY")
            {
                style.FormatCode = "\"$\"#,##0.00"; // Formato de moneda
            }
            else if (tipoDato == "NUMERIC")
            {
                style.FormatCode = "0"; // Formato numérico sin símbolo
            }

            return style;
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase uploadedFile)
        {
            if (uploadedFile == null || uploadedFile.ContentLength == 0)
            {
                TempData["ErrorMessage"] = "No se seleccionó ningún archivo.";
                return RedirectToAction("Index");
            }

            try
            {
                using (var sl = new SLDocument(uploadedFile.InputStream))
                {
                    // Cargar datos actuales en un diccionario para búsquedas rápidas
                    var dbData = db.BG_forecast_cat_historico_ventas
                        .Where(x => !x.BG_Forecast_cat_secciones_calculo.aplica_formula)
                        .ToDictionary(r => $"{r.id_cliente}-{r.id_seccion}-{r.fecha:yyyy-MM-dd}");

                    var newRecords = new List<BG_forecast_cat_historico_ventas>();
                    var updatedRecords = new List<BG_forecast_cat_historico_ventas>();
                    var deletedRecords = new List<BG_forecast_cat_historico_ventas>();

                    int creados = 0, actualizados = 0, borrados = 0;

                    foreach (var sheetName in sl.GetWorksheetNames())
                    {
                        sl.SelectWorksheet(sheetName);

                        // Validar encabezados
                        if (sl.GetCellValueAsString(1, 1) != "ID_Cliente" || sl.GetCellValueAsString(1, 3) != "ID_Seccion")
                        {
                            TempData["ErrorMessage"] = $"La hoja '{sheetName}' tiene encabezados inválidos.";
                            return RedirectToAction("Index");
                        }

                        // Leer datos de la hoja
                        int row = 2;
                        while (!string.IsNullOrWhiteSpace(sl.GetCellValueAsString(row, 1)))
                        {
                            int idCliente = sl.GetCellValueAsInt32(row, 1);
                            int idSeccion = sl.GetCellValueAsInt32(row, 3);

                            for (int col = 4; !string.IsNullOrWhiteSpace(sl.GetCellValueAsString(1, col)); col++)
                            {
                                string header = sl.GetCellValueAsString(1, col);
                                DateTime fecha = DateTime.ParseExact(header, "MMM-yyyy", new CultureInfo("es-MX"));
                                string key = $"{idCliente}-{idSeccion}-{fecha:yyyy-MM-dd}";

                                var valorCell = sl.GetCellValueAsString(row, col);
                                double? valor = double.TryParse(valorCell, out double parsedValue) ? parsedValue : (double?)null;

                                if (dbData.TryGetValue(key, out var existingRecord))
                                {
                                    if (valor.HasValue)
                                    {
                                        if (existingRecord.valor != valor.Value)
                                        {
                                            existingRecord.valor = valor.Value;
                                            db.Entry(existingRecord).State = EntityState.Modified; // Marcar como modificado
                                            actualizados++;
                                        }
                                        if (valor.Value == 0)
                                        {
                                            deletedRecords.Add(existingRecord);
                                            borrados++;
                                        }
                                    }
                                    else
                                    {
                                        deletedRecords.Add(existingRecord);
                                        borrados++;
                                    }
                                }
                                else if (valor.HasValue)
                                {
                                    var newRecord = new BG_forecast_cat_historico_ventas
                                    {
                                        id_cliente = idCliente,
                                        id_seccion = idSeccion,
                                        fecha = fecha,
                                        valor = valor.Value
                                    };
                                    newRecords.Add(newRecord);
                                    creados++;
                                }
                            }
                            row++;
                        }
                    }

                    // Operaciones masivas
                    if (newRecords.Any())
                    {
                        db.BG_forecast_cat_historico_ventas.AddRange(newRecords);
                    }

                    if (deletedRecords.Any())
                    {
                        db.BG_forecast_cat_historico_ventas.RemoveRange(deletedRecords);
                    }

                    // Guardar cambios
                    db.SaveChanges();
                    TempData["SuccessMessage"] = $"Sincronización completa. Creados: {creados}, Actualizados: {actualizados}, Eliminados: {borrados}.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error procesando archivo: {ex.Message}";
            }

            return RedirectToAction("Index");
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
