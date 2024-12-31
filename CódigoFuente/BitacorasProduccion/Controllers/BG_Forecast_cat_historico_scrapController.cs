using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BG_Forecast_cat_historico_scrapController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_Forecast_cat_historico_scrap
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            return View(db.BG_Forecast_cat_historico_scrap.OrderBy(x => x.fecha).ToList());
        }

        // GET: BG_Forecast_cat_historico_scrap/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_cat_historico_scrap bG_Forecast_cat_historico_scrap = db.BG_Forecast_cat_historico_scrap.Find(id);
            if (bG_Forecast_cat_historico_scrap == null)
            {
                return HttpNotFound();
            }
            return View(bG_Forecast_cat_historico_scrap);
        }

        // GET: BG_Forecast_cat_historico_scrap/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //list para estatus
            List<SelectListItem> newList = new List<SelectListItem>{
                new SelectListItem(){Text = "STEEL",Value = "STEEL"},
                new SelectListItem(){Text = "ALUMINIO",Value = "ALU"}};
            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text");
            ViewBag.tipo_metal = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Todos --");
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true
                                && (p.clave == 1 || p.clave == 2 || p.clave == 5)), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --");

            return View();
        }

        // POST: BG_Forecast_cat_historico_scrap/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BG_Forecast_cat_historico_scrap bG_Forecast_cat_historico_scrap)
        {
            //valida si existe ya el mismo mes

            if (db.BG_Forecast_cat_historico_scrap.Any(x => x.fecha == bG_Forecast_cat_historico_scrap.fecha
            && x.tipo_metal == bG_Forecast_cat_historico_scrap.tipo_metal
            && x.id_planta == bG_Forecast_cat_historico_scrap.id_planta
            ))
                ModelState.AddModelError("", "Ya existe un valor para esta planta; " + bG_Forecast_cat_historico_scrap.tipo_metal + " " + bG_Forecast_cat_historico_scrap.fecha.ToString("yyyy-MM"));


            if (ModelState.IsValid)
            {
                db.BG_Forecast_cat_historico_scrap.Add(bG_Forecast_cat_historico_scrap);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            //list para estatus
            List<SelectListItem> newList = new List<SelectListItem>{
                new SelectListItem(){Text = "STEEL",Value = "STEEL"},
                new SelectListItem(){Text = "ALUMINIO",Value = "ALU"}};
            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", bG_Forecast_cat_historico_scrap.tipo_metal);
            ViewBag.tipo_metal = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Todos --");
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true
                                && (p.clave == 1 || p.clave == 2 || p.clave == 5)), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --");

            return View(bG_Forecast_cat_historico_scrap);
        }

        // GET: BG_Forecast_cat_historico_scrap/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_cat_historico_scrap bG_Forecast_cat_historico_scrap = db.BG_Forecast_cat_historico_scrap.Find(id);
            if (bG_Forecast_cat_historico_scrap == null)
            {
                return HttpNotFound();
            }
            //list para estatus
            List<SelectListItem> newList = new List<SelectListItem>{
                new SelectListItem(){Text = "STEEL",Value = "STEEL"},
                new SelectListItem(){Text = "ALUMINIO",Value = "ALU"}};
            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", bG_Forecast_cat_historico_scrap.tipo_metal);
            ViewBag.tipo_metal = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Todos --");
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true
                                && (p.clave == 1 || p.clave == 2 || p.clave == 5)), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --", selected: bG_Forecast_cat_historico_scrap.id_planta.ToString());


            return View(bG_Forecast_cat_historico_scrap);
        }

        // POST: BG_Forecast_cat_historico_scrap/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BG_Forecast_cat_historico_scrap bG_Forecast_cat_historico_scrap)
        {

            if (db.BG_Forecast_cat_historico_scrap.Any(x => x.id != bG_Forecast_cat_historico_scrap.id
                        && x.fecha == bG_Forecast_cat_historico_scrap.fecha
                        && x.id_planta == bG_Forecast_cat_historico_scrap.id_planta
                        && x.tipo_metal == bG_Forecast_cat_historico_scrap.tipo_metal))
                ModelState.AddModelError("", "Ya existe un valor para esta planta; " + bG_Forecast_cat_historico_scrap.tipo_metal + " " + bG_Forecast_cat_historico_scrap.fecha.ToString("yyyy-MM"));

            if (ModelState.IsValid)
            {
                db.Entry(bG_Forecast_cat_historico_scrap).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            List<SelectListItem> newList = new List<SelectListItem>{
                new SelectListItem(){Text = "STEEL",Value = "STEEL"},
                new SelectListItem(){Text = "ALUMINIO",Value = "ALU"}};
            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", bG_Forecast_cat_historico_scrap.tipo_metal);
            ViewBag.tipo_metal = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Todos --");
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true
                               && (p.clave == 1 || p.clave == 2 || p.clave == 5)), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Seleccionar --", selected: bG_Forecast_cat_historico_scrap.id_planta.ToString());

            return View(bG_Forecast_cat_historico_scrap);
        }

        // GET: BG_Forecast_cat_historico_scrap/Delete/5
        //list para estatus

        public ActionResult Delete(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_cat_historico_scrap bG_Forecast_cat_historico_scrap = db.BG_Forecast_cat_historico_scrap.Find(id);
            if (bG_Forecast_cat_historico_scrap == null)
            {
                return HttpNotFound();
            }
            return View(bG_Forecast_cat_historico_scrap);
        }

        // POST: BG_Forecast_cat_historico_scrap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BG_Forecast_cat_historico_scrap bG_Forecast_cat_historico_scrap = db.BG_Forecast_cat_historico_scrap.Find(id);
            db.BG_Forecast_cat_historico_scrap.Remove(bG_Forecast_cat_historico_scrap);
            TempData["Mensaje"] = new MensajesSweetAlert("Se ha borrado el registro corretamente.", TipoMensajesSweetAlerts.SUCCESS);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CargaHistoricoScrap()
        {
            if (TieneRol(TipoRoles.BUDGET_IHS))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: Controller/CargaClass/5
        [HttpPost]
        public ActionResult CargaHistoricoScrap(ExcelViewModel excelViewModel, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                int nuevos = 0, modificados = 0;
                string msjError = "No se ha podido leer el archivo seleccionado.";

                //lee el archivo seleccionado
                try
                {
                    HttpPostedFileBase stream = Request.Files["PostedFile"];


                    if (stream.InputStream.Length > 8388608)
                    {
                        msjError = "Sólo se permiten archivos con peso menor a 8 MB.";
                        throw new Exception(msjError);
                    }
                    else
                    {
                        string extension = Path.GetExtension(excelViewModel.PostedFile.FileName);
                        if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                        {
                            msjError = "Sólo se permiten archivos Excel";
                            throw new Exception(msjError);
                        }
                    }

                    bool estructuraValida = false;
                    string msj = string.Empty;
                    //el archivo es válido
                    List<BG_Forecast_cat_historico_scrap> lista = UtilExcel.LeeHistoricoScrap(excelViewModel.PostedFile, ref estructuraValida, ref msj);



                    if (!estructuraValida)
                    {
                        msjError = msj;
                        throw new Exception(msjError);
                    }
                    else
                    {
                        var historicoBD = db.BG_Forecast_cat_historico_scrap.ToList();

                        //Recorre el listado, actualiza los existentes y agrega los faltanteas
                        foreach (var item in lista)
                        {
                            var elemento = historicoBD.FirstOrDefault(x => x.id_planta == item.id_planta && x.fecha == item.fecha && x.tipo_metal == x.tipo_metal);
                            if (elemento != null) //exite
                            {
                                // hay cambios
                                if (elemento.scrap != item.scrap || elemento.scrap_ganancia != item.scrap_ganancia)
                                {
                                    elemento.scrap = item.scrap;
                                    elemento.scrap_ganancia = item.scrap_ganancia;
                                    modificados++;
                                }
                            }
                            else
                            { // se debe agregar
                                historicoBD.Add(item); //agrega al listado de la BD, para futuras comparaciones
                                db.BG_Forecast_cat_historico_scrap.Add(item);
                                nuevos++;
                            }
                        }

                        db.SaveChanges();
                        TempData["Mensaje"] = new MensajesSweetAlert("Se cargó el archivo. Nuevos:" + nuevos + ", Modificados: " + modificados, TipoMensajesSweetAlerts.SUCCESS);
                        return RedirectToAction("index");
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", msjError);
                    return View(excelViewModel);
                }

            }
            return View(excelViewModel);
        }

        /// <summary>
        /// Descarga la plantilla de ejemplo
        /// </summary>
        /// <returns></returns>
        public ActionResult FormatoPlantilla()
        {
            String ruta = System.Web.HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/PlantillaHistoricoScrap.xlsx");

            //byte[] array = System.IO.File.ReadAllBytes(ruta);

            FileInfo archivo = new FileInfo(ruta);

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = archivo.Name,
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(ruta, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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
