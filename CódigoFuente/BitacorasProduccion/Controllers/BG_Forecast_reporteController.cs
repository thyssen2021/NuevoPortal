using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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

            var totalDeRegistros = db.BG_Forecast_reporte
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

            ViewBag.id = AddFirstItem(new SelectList(db.BG_Forecast_reporte.Where(p => p.activo == true), nameof(BG_Forecast_reporte.id), nameof(BG_Forecast_reporte.ConcatCodigo)), textoPorDefecto: "-- Todos --");

            return View(listado);
        }

        // GET: BG_Forecast_reporte/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

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

        /// <summary>
        /// Edita elmento de reporte
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: BG_Forecast_reporte/EditElemento/5
        public ActionResult EditElemento(int? id, string vehicle)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_item item = db.BG_Forecast_item.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            ViewBag.s_vehicle = vehicle;
            return View(item);
        }

        // POST: BG_Forecast_reporte/EditElemento/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditElemento(BG_Forecast_item model, string s_vehicle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("El reporte se actualizó correctamente", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Edit", new { id = model.id_bg_forecast_reporte, vehicle = s_vehicle });
            }

            ViewBag.s_vehicle = s_vehicle;
            return View(model);
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
                //busca si es posible relaciona con un elemento del ihs
                foreach (var item in bG_Forecast_reporte.BG_Forecast_item)
                {

                    // busca en ihs item
                    var IHSlist = db.BG_IHS_item.Where(x => x.vehicle == item.vehicle);
                    if (IHSlist.Count() == 1)
                    {
                        item.id_ihs_item = IHSlist.FirstOrDefault().id;
                        continue; // pasa a la siguiente iteracion
                    }

                    // busca en combinaciones
                    var Combinacioneslist = db.BG_IHS_combinacion.Where(x => x.vehicle == item.vehicle);
                    if (Combinacioneslist.Count() == 1)
                    {
                        item.id_ihs_combinacion = Combinacioneslist.FirstOrDefault().id;
                        continue; // pasa a la siguiente iteracion
                    }

                    // busca en divisiones
                    var Divisioneslist = db.BG_IHS_rel_division.Where(x => x.vehicle == item.vehicle);
                    if (Divisioneslist.Count() == 1)
                    {
                        item.id_ihs_rel_division = Divisioneslist.FirstOrDefault().id;
                        continue; // pasa a la siguiente iteracion
                    }

                }

                db.BG_Forecast_reporte.Add(bG_Forecast_reporte);
                TempData["Mensaje"] = new MensajesSweetAlert("El reporte se cargó correctamente", TipoMensajesSweetAlerts.SUCCESS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bG_Forecast_reporte);
        }

        // GET: BG_Forecast_reporte/ReemplazarReporte
        public ActionResult ReemplazarReporte(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte model = db.BG_Forecast_reporte.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            ViewBag.Reemplazar = true;

            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReemplazarReporte(BG_Forecast_reporte bG_Forecast_reporte)
        {

            if (bG_Forecast_reporte.BG_Forecast_item.Count == 0)
                ModelState.AddModelError("", "No se han agregado elementos al reporte.");

            if (ModelState.IsValid)
            {
                //borra los conceptos anteriores
                var list = db.BG_Forecast_item.Where(x => x.id_bg_forecast_reporte == bG_Forecast_reporte.id);
                foreach (BG_Forecast_item item in list)
                    db.BG_Forecast_item.Remove(item);

                //si existe lo modifica
                BG_Forecast_reporte reporte = db.BG_Forecast_reporte.Find(bG_Forecast_reporte.id);
                // Activity already exist in database and modify it
                db.Entry(reporte).CurrentValues.SetValues(bG_Forecast_reporte);

                //busca si es posible relaciona con un elemento del ihs
                foreach (var item in bG_Forecast_reporte.BG_Forecast_item)
                {

                    // busca en ihs item
                    var IHSlist = db.BG_IHS_item.Where(x => x.vehicle == item.vehicle);
                    if (IHSlist.Count() == 1)
                    {
                        item.id_ihs_item = IHSlist.FirstOrDefault().id;
                        continue; // pasa a la siguiente iteracion
                    }

                    // busca en combinaciones
                    var Combinacioneslist = db.BG_IHS_combinacion.Where(x => x.vehicle == item.vehicle);
                    if (Combinacioneslist.Count() == 1)
                    {
                        item.id_ihs_combinacion = Combinacioneslist.FirstOrDefault().id;
                        continue; // pasa a la siguiente iteracion
                    }

                    // busca en divisiones
                    var Divisioneslist = db.BG_IHS_rel_division.Where(x => x.vehicle == item.vehicle);
                    if (Divisioneslist.Count() == 1)
                    {
                        item.id_ihs_rel_division = Divisioneslist.FirstOrDefault().id;
                        continue; // pasa a la siguiente iteracion
                    }

                }

                //agrega los conceptos 
                reporte.BG_Forecast_item = bG_Forecast_reporte.BG_Forecast_item;

                db.Entry(reporte).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert("Se ha reemplazado el registro correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.Reemplazar = true;
            return View("Create", bG_Forecast_reporte);
        }



        // GET: BG_Forecast_reporte/Edit/5
        public ActionResult Edit(int? id, string vehicle, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte bG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (bG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro


            var listado = db.BG_Forecast_item
               .Where(x =>
                        (x.id_bg_forecast_reporte == id)
                        && (x.vehicle == vehicle || string.IsNullOrEmpty(vehicle))
                      )
               .OrderBy(x => x.id)
               .Skip((pagina - 1) * cantidadRegistrosPorPagina)
              .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.BG_Forecast_item
             .Where(x =>
                        (x.id_bg_forecast_reporte == id)
                        && (x.vehicle == vehicle || string.IsNullOrEmpty(vehicle))
                      )
             .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id"] = id;
            routeValues["vehicle"] = vehicle;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };
            ViewBag.Paginacion = paginacion;

            // -- vehicle --
            List<SelectListItem> selectListVehicle = new List<SelectListItem>();
            List<string> vehicleList = db.BG_Forecast_item
               .Where(x =>
                        (x.id_bg_forecast_reporte == id)
                      )
               .Select(x => x.vehicle).Distinct().ToList();

            foreach (var itemList in vehicleList)
                selectListVehicle.Add(new SelectListItem() { Text = itemList, Value = itemList });
            ViewBag.vehicle = AddFirstItem(new SelectList(selectListVehicle, "Value", "Text", vehicle), textoPorDefecto: "-- Todos --");

            return View(listado);
        }

        // GET: BG_Forecast_reporte/EditarReporte/5
        public ActionResult EditarReporte(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte BG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (BG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }

            var listado = BG_Forecast_reporte.BG_Forecast_item.ToList();

            return View(listado);
        }


        // GET: BG_Forecast_reporte/EditarReporte/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarReporte(List<BG_Forecast_item> listado)
        {
            var result = new object[1];

            try
            {

                int idReporte = 0;
                //valida que haya elementos
                if (listado.Count > 0)
                    idReporte = listado[0].id_bg_forecast_reporte;


                if (ModelState.IsValid)
                {
                    var listadoBD = db.BG_Forecast_item.Where(x => x.id_bg_forecast_reporte == idReporte).ToList();
                    List<BG_Forecast_item> diferencias = listado.Except(listadoBD).ToList();

                    foreach (var dif in diferencias)
                    {
                        var difBD = listadoBD.FirstOrDefault(x => x.id == dif.id);
                        db.Entry(difBD).CurrentValues.SetValues(dif);
                    }

                    db.SaveChanges();
                    //TempData["Mensaje"] = new MensajesSweetAlert("Se actualizó el reporte correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                    result[0] = new { status = "OK", value = "Se actualizó el reporte correctamente." };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                //TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message , TipoMensajesSweetAlerts.ERROR);
                result[0] = new { status = "ERROR", value = "Error: " + e.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            //TempData["Mensaje"] = new MensajesSweetAlert("Error: No se completó el proceso. Verifique los datos.", TipoMensajesSweetAlerts.ERROR);
            result[0] = new { status = "ERROR", value = "No se completó el proceso." };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: BG_Forecast_reporte/EditarAsociacionIHS/5
        public ActionResult EditarAsociacionIHS(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte BG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (BG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }

            //viewbag para listado de elemento de IHS
            List<BG_IHS_item> vehicleListIHS = db.BG_IHS_item.ToList();
            List<BG_IHS_combinacion> vehicleListCombinacion = db.BG_IHS_combinacion.Where(x => x.activo).ToList();
            List<BG_IHS_rel_division> vehicleListDivision = db.BG_IHS_rel_division.ToList();

            ViewBag.vehicleListIHS = vehicleListIHS;
            ViewBag.vehicleListCombinacion = vehicleListCombinacion;
            ViewBag.vehicleListDivision = vehicleListDivision;

            var listado = BG_Forecast_reporte.BG_Forecast_item.ToList();

            return View(listado);
        }


        // GET: BG_Forecast_reporte/EditarReporte/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAsociacionIHS(List<BG_Forecast_item> listado)
        {
            var result = new object[1];

            try
            {

                int idReporte = 0;
                //valida que haya elementos
                if (listado.Count > 0)
                    idReporte = listado[0].id_bg_forecast_reporte;


                var listadoBD = db.BG_Forecast_item.Where(x => x.id_bg_forecast_reporte == idReporte).ToList();

                //actualiza el id de la asociacion 
                foreach (var item in listado)
                {
                    Match m = Regex.Match(!String.IsNullOrEmpty(item.p_id_asociacion_ihs)? item.p_id_asociacion_ihs:String.Empty, @"\d+");

                    int num_id = 0;
                    if (m.Success)
                    {  //si tiene un numero
                        int.TryParse(m.Value, out num_id);

                        //obtiene el valor de la bd
                        var forecastItem = listadoBD.Where(x => x.id == item.id).FirstOrDefault();
                        //borra la asociacion
                        forecastItem.id_ihs_item = null;
                        forecastItem.id_ihs_combinacion = null;
                        forecastItem.id_ihs_rel_division = null;

                        //separa la clave del id
                        switch (item.p_id_asociacion_ihs[0])
                        {
                            case 'I':
                                forecastItem.id_ihs_item = num_id;
                                break;
                            case 'C':
                                forecastItem.id_ihs_combinacion = num_id;
                                break;
                            case 'D':
                                forecastItem.id_ihs_rel_division = num_id;
                                break;
                        }
                    }
                }

                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se actualizó el reporte correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                result[0] = new { status = "OK", value = "Se actualizó el reporte correctamente." };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                //TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message , TipoMensajesSweetAlerts.ERROR);
                result[0] = new { status = "ERROR", value = "Error: " + e.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //viewbag para listado de elemento de IHS
            List<BG_IHS_item> vehicleListIHS = db.BG_IHS_item.ToList();
            List<BG_IHS_combinacion> vehicleListCombinacion = db.BG_IHS_combinacion.Where(x => x.activo).ToList();
            List<BG_IHS_rel_division> vehicleListDivision = db.BG_IHS_rel_division.Where(x => x.activo).ToList();

            ViewBag.vehicleListIHS = vehicleListIHS;
            ViewBag.vehicleListCombinacion = vehicleListCombinacion;
            ViewBag.vehicleListDivision = vehicleListDivision;

            //TempData["Mensaje"] = new MensajesSweetAlert("Error: No se completó el proceso. Verifique los datos.", TipoMensajesSweetAlerts.ERROR);
            result[0] = new { status = "ERROR", value = "No se completó el proceso." };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: BG_Forecast_reporte/ActualizaReferencias/5
        public ActionResult ActualizaReferencias(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte bG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (bG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }

            //obtiene el listado original de bd
            //var listadoBD = db.BG_Forecast_item.Where(x => x.id_bg_forecast_reporte == id).ToList();
            int cambios = 0;

            //busca si es posible relacionar con un elemento del ihs
            foreach (var item in bG_Forecast_reporte.BG_Forecast_item.Where(x => x.id_ihs_item == null && x.id_ihs_rel_division == null && x.id_ihs_combinacion == null))
            {

                // busca en ihs item
                var IHSlist = db.BG_IHS_item.Where(x => x.vehicle == item.vehicle);
                if (IHSlist.Count() == 1)
                {
                    item.id_ihs_item = IHSlist.FirstOrDefault().id;
                    cambios++;
                    continue; // pasa a la siguiente iteracion
                }

                // busca en combinaciones
                var Combinacioneslist = db.BG_IHS_combinacion.Where(x => x.vehicle == item.vehicle);
                if (Combinacioneslist.Count() == 1)
                {
                    item.id_ihs_combinacion = Combinacioneslist.FirstOrDefault().id;
                    cambios++;
                    continue; // pasa a la siguiente iteracion
                }

                // busca en divisiones
                var Divisioneslist = db.BG_IHS_rel_division.Where(x => x.vehicle == item.vehicle);
                if (Divisioneslist.Count() == 1)
                {
                    item.id_ihs_rel_division = Divisioneslist.FirstOrDefault().id;
                    cambios++;
                    continue; // pasa a la siguiente iteracion
                }

            }

            //busca las diferencias y actualiza la BD
            //List<BG_Forecast_item> diferencias = bG_Forecast_reporte.BG_Forecast_item.Except(listadoBD).ToList();

            //foreach (var dif in diferencias)
            //{
            //    var difBD = listadoBD.FirstOrDefault(x => x.id == dif.id);
            //    db.Entry(difBD).CurrentValues.SetValues(dif);
            //}

            db.SaveChanges();
            TempData["Mensaje"] = new MensajesSweetAlert("Se actualizaron: " + cambios + " registros, para el reporte: " + bG_Forecast_reporte.descripcion + ".", TipoMensajesSweetAlerts.SUCCESS);

            return RedirectToAction("Index", new { id = id });
        }

        // GET: BG_Forecast_reporte/Disable/5
        public ActionResult Disable(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte BG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (BG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }
            return View(BG_Forecast_reporte);
        }

        // POST: BG_Forecast_reporte/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            BG_Forecast_reporte item = db.BG_Forecast_reporte.Find(id);
            item.activo = false;

            db.Entry(item).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Index");
            }
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.DISABLED, TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("Index");
        }

        // GET: BG_Forecast_reporte/Enable/5
        public ActionResult Enable(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_Forecast_reporte BG_Forecast_reporte = db.BG_Forecast_reporte.Find(id);
            if (BG_Forecast_reporte == null)
            {
                return HttpNotFound();
            }
            return View(BG_Forecast_reporte);
        }

        // POST: BG_Forecast_reporte/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            BG_Forecast_reporte item = db.BG_Forecast_reporte.Find(id);
            item.activo = true;

            db.Entry(item).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Index");
            }
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.ENABLED, TipoMensajesSweetAlerts.SUCCESS);
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
            if (file.FileName == "")
            {
                result[0] = new { status = "ERROR", value = "Seleccione un archivo." };
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
            else
            { //si el archivo se lee correctamente

                string body = string.Empty;
                string div_ocultos = string.Empty;

                foreach (var item in lista)
                {
                    int index = lista.IndexOf(item);

                    div_ocultos += @"
                            <input type=""hidden"" name=""BG_Forecast_item.Index"" id=""BG_Forecast_item.Index"" value=""" + index + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.id_bg_forecast_reporte) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.id_bg_forecast_reporte) + @""" value=""" + item.id_bg_forecast_reporte + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.pos) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.pos) + @""" value=""" + item.pos + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.business_and_plant) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.business_and_plant) + @""" value=""" + item.business_and_plant + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.calculo_activo) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.calculo_activo) + @""" value=""" + item.calculo_activo + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.sap_invoice_code) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.sap_invoice_code) + @""" value=""" + item.sap_invoice_code + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.invoiced_to) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.invoiced_to) + @""" value=""" + item.invoiced_to + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.number_sap_client) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.number_sap_client) + @""" value=""" + item.number_sap_client + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.shipped_to) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.shipped_to) + @""" value=""" + item.shipped_to + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.own_cm) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.own_cm) + @""" value=""" + item.own_cm + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.route) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.route) + @""" value=""" + item.route + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.plant) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.plant) + @""" value=""" + item.plant + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.external_processor) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.external_processor) + @""" value=""" + item.external_processor + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.mill) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.mill) + @""" value=""" + item.mill + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.sap_master_coil) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.sap_master_coil) + @""" value=""" + item.sap_master_coil + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.part_description) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.part_description) + @""" value=""" + item.part_description + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.part_number) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.part_number) + @""" value=""" + item.part_number + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.production_line) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.production_line) + @""" value=""" + item.production_line + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.vehicle) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.vehicle) + @""" value=""" + item.vehicle + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.parts_auto) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.parts_auto) + @""" value=""" + item.parts_auto + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.strokes_auto) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.strokes_auto) + @""" value=""" + item.strokes_auto + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.material_type) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.material_type) + @""" value=""" + item.material_type + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.shape) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.shape) + @""" value=""" + item.shape + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.initial_weight_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.initial_weight_part) + @""" value=""" + item.initial_weight_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.net_weight_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.net_weight_part) + @""" value=""" + item.net_weight_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.scrap_consolidation) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.scrap_consolidation) + @""" value=""" + item.scrap_consolidation + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.ventas_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.ventas_part) + @""" value=""" + item.ventas_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.material_cost_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.material_cost_part) + @""" value=""" + item.material_cost_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cost_of_outside_processor) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cost_of_outside_processor) + @""" value=""" + item.cost_of_outside_processor + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.additional_material_cost_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.additional_material_cost_part) + @""" value=""" + item.additional_material_cost_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.outgoing_freight_part) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.outgoing_freight_part) + @""" value=""" + item.outgoing_freight_part + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.freights_income) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.freights_income) + @""" value=""" + item.freights_income + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.outgoing_freight) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.outgoing_freight) + @""" value=""" + item.outgoing_freight + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_1) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_1) + @""" value=""" + item.cat_1 + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_2) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_2) + @""" value=""" + item.cat_2 + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_3) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_3) + @""" value=""" + item.cat_3 + @""" />
                            <input type=""hidden"" name=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_4) + @""" id=""BG_Forecast_item[" + index + @"]." + nameof(BG_Forecast_item.cat_4) + @""" value=""" + item.cat_4 + @""" />
";

                    body += @"                            
                             <tr> 
                                <td>" + (lista.IndexOf(item) + 1) + "</td>" +
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
                                "<td>" + item.external_processor + "</td>" +
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
                                "<td>" + item.eng_scrap_part + "</td>" +
                                "<td>" + item.scrap_consolidation + "</td>" +
                                "<td>" + "$ " + (item.ventas_part.HasValue ? item.ventas_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.material_cost_part.HasValue ? item.material_cost_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.cost_of_outside_processor.HasValue ? item.cost_of_outside_processor.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.vas_part.HasValue ? item.vas_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.additional_material_cost_part.HasValue ? item.additional_material_cost_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.outgoing_freight_part.HasValue ? item.outgoing_freight_part.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + "$ " + (item.vas_to.HasValue ? item.vas_to.Value.ToString("0.00") : String.Empty) + "</td>" +
                                "<td>" + item.freights_income + "</td>" +
                                "<td>" + item.outgoing_freight + "</td>" +
                            @"</tr>";


                }
                body += "</tr>";

                /*
                 * enviar el contenido del body de la tabla
                 * Aumenta la longitud máxima por defecto del serializador
                 */
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;

                result[0] = new { status = estatus, value = body, div_ocultos = div_ocultos };
                var json = Json(result, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = 500000000;
                return json;
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
            if (file.FileName == "")
            {
                result[0] = new { status = "ERROR", value = "Seleccione un archivo." };
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
            else
            { //si el archivo se lee correctamente

                string selects = "<option value=\"\" selected>Seleccione</option>";

                foreach (var h in lista)
                {
                    selects += "<option value=\"" + h + "\">" + h + "</option>";
                }

                result[0] = new { status = "OK", value = selects };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: BG_Forecast_reporte/GenerarReporte/
        public ActionResult GenerarReporte()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            var model = new ReporteBudgetForecastViewModel { };

            // -- tipo de listado ORIGEN
            List<SelectListItem> selectListDemanda = new List<SelectListItem>();
            selectListDemanda.Add(new SelectListItem() { Text = Bitacoras.Util.BG_IHS_tipo_demanda.DescripcionStatus(Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL), Value = Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL });
            selectListDemanda.Add(new SelectListItem() { Text = Bitacoras.Util.BG_IHS_tipo_demanda.DescripcionStatus(Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER), Value = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER });
            ViewBag.demanda = AddFirstItem(new SelectList(selectListDemanda, "Value", "Text"), textoPorDefecto: "-- Seleccionar --");
            ViewBag.id_reporte = AddFirstItem(new SelectList(db.BG_Forecast_reporte.Where(p => p.activo == true), nameof(BG_Forecast_reporte.id), nameof(BG_Forecast_reporte.ConcatCodigo)), textoPorDefecto: "-- Seleccionar --");

            return View(model);
        }

        // GET: BG_Forecast_reporte/GenerarReporte/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerarReporte(ReporteBudgetForecastViewModel model)
        {
            if (ModelState.IsValid)
            {
                byte[] stream = { };

                //Envia un contexto de EF que se cerrarrá después de llamar al método
                using (var dbContext = new Portal_2_0Entities())
                {
                    stream = ExcelUtil.GeneraReporteBudgetForecast(model, dbContext);
                }

                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Reporte_Forecast_" + model.demanda + "_reporte_" + ".xlsx",

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(stream, "application/vnd.ms-excel");
            }
            // -- tipo de listado ORIGEN
            List<SelectListItem> selectListDemanda = new List<SelectListItem>();
            selectListDemanda.Add(new SelectListItem() { Text = Bitacoras.Util.BG_IHS_tipo_demanda.DescripcionStatus(Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL), Value = Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL });
            selectListDemanda.Add(new SelectListItem() { Text = Bitacoras.Util.BG_IHS_tipo_demanda.DescripcionStatus(Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER), Value = Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER });
            ViewBag.demanda = AddFirstItem(new SelectList(selectListDemanda, "Value", "Text"), textoPorDefecto: "-- Seleccionar --"); 
            ViewBag.id_reporte = AddFirstItem(new SelectList(db.BG_Forecast_reporte.Where(p => p.activo == true), nameof(BG_Forecast_reporte.id), nameof(BG_Forecast_reporte.ConcatCodigo)), textoPorDefecto: "-- Seleccionar --");

            return View(model);
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
