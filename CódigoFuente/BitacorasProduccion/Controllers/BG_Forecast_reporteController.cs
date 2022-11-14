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
    [Authorize]
    public class BG_Forecast_reporteController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_Forecast_reporte
        public ActionResult Index(int? id, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro

            //verifica que el elemento este relacionado con el elmento anterior

            var listado = db.BG_Forecast_reporte
               .Where(x =>
                        (x.id == id || id == null)
                      )
               .OrderBy(x => x.id)
               .Skip((pagina - 1) * cantidadRegistrosPorPagina)
              .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.BG_IHS_item
               .Where(x =>
                        (x.id == id || id == null)
                      )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id"] = id;          

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };
            ViewBag.Paginacion = paginacion;

            ViewBag.id = AddFirstItem(new SelectList(db.BG_Forecast_reporte.Where(p => p.activo == true), nameof(BG_Forecast_reporte.id) , nameof(BG_Forecast_reporte.descripcion)), textoPorDefecto: "-- Todos --");

            return View(listado);
        }

        // GET: BG_Forecast_reporte/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte bG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (bG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }
            return View(bG_Forecast_reporte);
        }

        // GET: BG_Forecast_reporte/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            BG_Forecast_reporte model = new BG_Forecast_reporte { fecha = DateTime.Now };    

            return View(model);
        }

        // POST: BG_Forecast_reporte/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BG_Forecast_reporte bG_Forecast_reporte)
        {
            if (bG_Forecast_reporte.BG_Forecast_item.Count == 0)
                ModelState.AddModelError("", "No se han agregado elementos al reporte.");


            if (ModelState.IsValid)
            {
                db.BG_Forecast_reporte.Add(bG_Forecast_reporte);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bG_Forecast_reporte);
        }

        // GET: BG_Forecast_reporte/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte bG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (bG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }
            return View(bG_Forecast_reporte);
        }

        // POST: BG_Forecast_reporte/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,fecha,descripcion,estatus")] BG_Forecast_reporte bG_Forecast_reporte)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bG_Forecast_reporte).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bG_Forecast_reporte);
        }

        // GET: BG_Forecast_reporte/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte bG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (bG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }
            return View(bG_Forecast_reporte);
        }

        // POST: BG_Forecast_reporte/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BG_Forecast_reporte bG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            db.BG_Forecast_reporte.Remove(bG_Forecast_reporte);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Método que retorna las filas de las tablas de combinacion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult GetReportRows(string select_hoja) 
        {
            var result = new object[1];

            HttpPostedFileBase file = Request.Files[0];

            //Valida que se haya enviado un archivo
            if (file.FileName=="" ) {
                result[0] = new { status ="ERROR", value = "Seleccione un archivo." };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //valida que sea un archivo en excel
            string extension = Path.GetExtension(file.FileName);
            if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
            {
                result[0] = new { status = "ERROR", value = "Sólo se permiten archivos en Excel." };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            string estatus = string.Empty;   
            string msj = string.Empty;   

            //el archivo es válido
            List<BG_Forecast_item> lista = UtilExcel.LeeReporteBudget(file, select_hoja, ref estatus, ref msj);

            //si hubo algún error al leer el archivo
            if (estatus.ToUpper() == "ERROR")
            {
                result[0] = new { status = estatus, value = msj };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else { //si el archivo se lee correctamente

                string body = string.Empty;

                foreach (var item in lista) {
                    body += @"<tr> 
                                <td>" + (lista.IndexOf(item) +1) + "</td>" +
                                "<td>" + item.business_and_plant + "</td>" +
                                "<td>" + item.cat_2 + "</td>" +
                                "<td>" + item.calculo_activo + "</td>" +
                                "<td>" + item.sap_invoice_code + "</td>" +
                                "<td>" + item.invoiced_to + "</td>" +
                                "<td>" + item.number_sap_client + "</td>" +
                                "<td>" + item.shipped_to + "</td>" +
                                "<td>" + item.own_cm + "</td>" +
                                "<td>" + item.route + "</td>" +
                                "<td>" + item.plant + "</td>" +
                                "<td>" + item.external_process + "</td>" +
                                "<td>" + item.mill + "</td>" +
                                "<td>" + item.sap_master_coil + "</td>" +
                                "<td>" + item.part_description + "</td>" +
                                "<td>" + item.part_number + "</td>" +
                                "<td>" + item.production_line + "</td>" +
                                "<td>" + item.vehicle + "</td>" +
                                "<td>" + item.parts_auto + "</td>" +
                                "<td>" + item.strokes_auto + "</td>" +
                                "<td>" + item.material_type + "</td>" +
                                "<td>" + item.shape + "</td>" +
                                "<td>" + item.initial_weight_part + "</td>" +
                                "<td>" + item.net_weight_part + "</td>" +
                                "<td>" + item.scrap_consolidation + "</td>" +
                                "<td>" + (item.ventas_part.HasValue ? item.ventas_part.Value.ToString("0.00"): String.Empty) + "</td>" +
                                "<td>" + (item.material_cost_part.HasValue ? item.material_cost_part.Value.ToString("0.00"): String.Empty) + "</td>" +
                                "<td>" + (item.cost_of_outside_processor.HasValue ? item.cost_of_outside_processor.Value.ToString("0.00"): String.Empty) + "</td>" +
                                "<td>" + (item.vas_part.HasValue ? item.vas_part.Value.ToString("0.00"): String.Empty) + "</td>" +
                                "<td>" + (item.additional_material_cost_part.HasValue ? item.additional_material_cost_part.Value.ToString("0.00"): String.Empty) + "</td>" +
                                "<td>" + (item.outgoing_freight_part.HasValue ? item.outgoing_freight_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + item.freights_income+ "</td>" +
                                "<td>" + item.outgoing_freight + "</td>" +
                            @"</tr>";
                        

                }
                body += "</tr>";

                /*enviar el contenido del body de la tabla*/

                result[0] = new { status = estatus, value = body };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

           
        }
         /// <summary>
        /// Método que retorna el nombre de las hojas del archivo
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult GetReportSheetNames() 
        {
            var result = new object[1];

            HttpPostedFileBase file = Request.Files[0];

            //Valida que se haya enviado un archivo
            if (file.FileName=="" ) {
                result[0] = new { status ="ERROR", value = "Seleccione un archivo." };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //valida que sea un archivo en excel
            string extension = Path.GetExtension(file.FileName);
            if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
            {
                result[0] = new { status = "ERROR", value = "Sólo se permiten archivos en Excel." };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            string estatus = string.Empty;   
            string msj = string.Empty;   

            //el archivo es válido
            List<string> lista = UtilExcel.LeeHojasArchivo(file, ref estatus, ref msj);

            //si hubo algún error al leer el archivo
            if (estatus == "ERROR")
            {
                result[0] = new { status = estatus, value = "<option value=\"\" selected>Seleccione</option>" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else { //si el archivo se lee correctamente

                string selects = "<option value=\"\" selected>Seleccione</option>";

                foreach(var h in lista){
                    selects += "<option value=\""+h+"\">"+h+"</option>";
                }

                result[0] = new { status = "OK" , value = selects };
                return Json(result, JsonRequestBehavior.AllowGet);
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
