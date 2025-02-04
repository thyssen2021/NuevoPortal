using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BG_IHS_divisionController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_IHS_division     
        public ActionResult Index(int? id_ihs_item, int? version, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var listadoBD = db.BG_IHS_division.Where(x => x.id_ihs_version == version);

            var listado = listadoBD
                .Where(x => x.id_ihs_item == id_ihs_item || id_ihs_item == null)
                .OrderByDescending(x => x.id)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = listadoBD
                .Where(x => x.id_ihs_item == id_ihs_item || id_ihs_item == null)
                .Count();

            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id_ihs_item"] = id_ihs_item;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.Paginacion = paginacion;
            ViewBag.id_ihs_item = AddFirstItem(new SelectList(db.BG_IHS_item.Where(x => x.id_ihs_version == version && x.BG_IHS_division.Any()), nameof(BG_IHS_item.id), nameof(BG_IHS_item.ConcatCodigo)), textoPorDefecto: "-- Todos --");

            ViewBag.VersionIHS = db.BG_IHS_versiones.Find(version);
            return View(listado);
        }

        // GET: BG_IHS_division/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_division bG_IHS_division = db.BG_IHS_division.Find(id);
            if (bG_IHS_division == null)
            {
                return HttpNotFound();
            }

            //convierte el porcentaje
            foreach (var rel in bG_IHS_division.BG_IHS_rel_division)
                rel.porcentaje = (int)(rel.porcentaje * 100);

            return View(bG_IHS_division);
        }

        // GET: BG_IHS_division/Create
        public ActionResult Create(int? version)
        {

            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            var model = new BG_IHS_division
            {
                porcentaje_scrap_100 = 3
            };

            model.id_ihs_version = version.HasValue ? version.Value : 0;
            ViewBag.id_ihs_item = AddFirstItem(new SelectList(db.BG_IHS_item.Where(x => x.id_ihs_version == version), nameof(BG_IHS_item.id), nameof(BG_IHS_item.ConcatCodigo), model.id_ihs_item), textoPorDefecto: "-- Seleccionar --");

            ViewBag.VersionIHS = db.BG_IHS_versiones.Find(version);
            return View(model);
        }

        // POST: BG_IHS_division/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BG_IHS_division bG_IHS_division)
        {
            //List<string> codigosIHS = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_division.id_ihs_version).ToList().Select(x => x.ConcatCodigo).ToList();

            if (bG_IHS_division.BG_IHS_rel_division.Count == 0)
                ModelState.AddModelError("", "No se ha agregado ningún elemento a la división.");

            //if (bG_IHS_division.BG_IHS_rel_division.Sum(x => x.porcentaje) != 100)
            //    ModelState.AddModelError("", "La suma de los porcentajes debe ser igual al 100%. Suma Actual: " + bG_IHS_division.BG_IHS_rel_division.Sum(x => x.porcentaje) + "%");

            foreach (var div in bG_IHS_division.BG_IHS_rel_division)
            {
                if (db.BG_IHS_rel_division.Any(x => x.BG_IHS_division.id_ihs_version == bG_IHS_division.id_ihs_version && x.vehicle == div.vehicle))
                    ModelState.AddModelError("", "Ya existe una división con la misma clave de vehículo: " + div.vehicle);

                if (db.BG_IHS_combinacion.Any(x => x.id_ihs_version == bG_IHS_division.id_ihs_version && x.vehicle == div.vehicle))
                    ModelState.AddModelError("", "Ya existe una combinación con la misma clave de vehículo: " + div.vehicle);

                //compara vehiculo con concatCodigo           
                //if (codigosIHS.Any(x => x == div.vehicle))
                //    ModelState.AddModelError("", "Ya existe un registro de IHS con la misma clave de vehículo.");
            }

            //verificar si no hay ids repetidos
            var query = bG_IHS_division.BG_IHS_rel_division.GroupBy(x => x.vehicle)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            if (query.Count > 0)
                ModelState.AddModelError("", "Existen códigos de vehículo repetidos en la división, favor de verificarlo.");


            if (ModelState.IsValid)
            {
                //convierte el porcentaje
                foreach (var rel in bG_IHS_division.BG_IHS_rel_division)
                {
                    rel.porcentaje /= 100;
                }

                bG_IHS_division.activo = true;

                db.BG_IHS_division.Add(bG_IHS_division);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se han guardado los cambios correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index", new { version = bG_IHS_division.id_ihs_version });
            }

            ViewBag.id_ihs_item = AddFirstItem(new SelectList(db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_division.id_ihs_version), nameof(BG_IHS_item.id), nameof(BG_IHS_item.ConcatCodigo), bG_IHS_division.id_ihs_item), textoPorDefecto: "-- Seleccionar --");
            ViewBag.VersionIHS = db.BG_IHS_versiones.Find(bG_IHS_division.id_ihs_version);

            return View(bG_IHS_division);
        }

        // GET: BG_IHS_division/Edit/5
        public ActionResult Edit(int? id)
        {

            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_division bG_IHS_division = db.BG_IHS_division.Find(id);
            if (bG_IHS_division == null)
            {
                return HttpNotFound();
            }

            //convierte el porcentaje
            foreach (var rel in bG_IHS_division.BG_IHS_rel_division)
                rel.porcentaje = (int)(rel.porcentaje * 100);

            ViewBag.id_ihs_item = AddFirstItem(new SelectList(db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_division.id_ihs_version), nameof(BG_IHS_item.id), nameof(BG_IHS_item.ConcatCodigo), bG_IHS_division.id_ihs_item), textoPorDefecto: "-- Seleccionar --");
            ViewBag.VersionIHS = db.BG_IHS_versiones.Find(bG_IHS_division.id_ihs_version);

            return View(bG_IHS_division);



        }

        // POST: BG_IHS_division/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BG_IHS_division bG_IHS_division)
        {
            List<string> codigosIHS = db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_division.id_ihs_version).ToList().Select(x => x.ConcatCodigo).ToList();

            if (bG_IHS_division.BG_IHS_rel_division.Count == 0)
                ModelState.AddModelError("", "No se ha agregado ningún elemento a la división.");

            //if (bG_IHS_division.BG_IHS_rel_division.Sum(x => x.porcentaje) != 100)
            //    ModelState.AddModelError("", "La suma de los porcentajes debe ser igual al 100%. Suma Actual: " + bG_IHS_division.BG_IHS_rel_division.Sum(x => x.porcentaje) + "%");

            foreach (var div in bG_IHS_division.BG_IHS_rel_division)
            {
                if (db.BG_IHS_rel_division.Any(x => x.BG_IHS_division.id_ihs_version == bG_IHS_division.id_ihs_version && x.vehicle == div.vehicle && bG_IHS_division.id != x.id_ihs_division))
                    ModelState.AddModelError("", "Ya existe una división con la misma clave de vehículo: " + div.vehicle);

                if (db.BG_IHS_combinacion.Any(x => x.id_ihs_version == bG_IHS_division.id_ihs_version && x.vehicle == div.vehicle))
                    ModelState.AddModelError("", "Ya existe una combinación con la misma clave de vehículo: " + div.vehicle);

                ////compara vehiculo con concatCodigo           
                //if (codigosIHS.Any(x => x == div.vehicle))
                //    ModelState.AddModelError("", "Ya existe un registro de IHS con la misma clave de vehículo.");
            }

            //verificar si no hay ids repetidos
            var query = bG_IHS_division.BG_IHS_rel_division.GroupBy(x => x.vehicle)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            if (query.Count > 0)
                ModelState.AddModelError("", "Existen códigos de vehículo repetidos en la división, favor de verificarlo.");

            if (ModelState.IsValid)
            {
                //convierte el porcentaje
                foreach (var rel in bG_IHS_division.BG_IHS_rel_division)
                    rel.porcentaje /= 100;

                //borra los conceptos anteriores
                var listBD = db.BG_IHS_rel_division.Where(x => x.id_ihs_division == bG_IHS_division.id);
                //foreach (BG_IHS_rel_division item in list)
                //    db.BG_IHS_rel_division.Remove(item);

                //los nuevos conceptos se agregarán automáticamente
                //si existe lo modifica
                BG_IHS_division division = db.BG_IHS_division.Find(bG_IHS_division.id);
                // Activity already exist in database and modify it
                db.Entry(division).CurrentValues.SetValues(bG_IHS_division);




                //agrega los conceptos nuevos
                division.BG_IHS_rel_division = bG_IHS_division.BG_IHS_rel_division.Where(x => x.id == 0).ToList();

                db.Entry(division).State = EntityState.Modified;
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                db.SaveChanges();

                //modifica los conceptos con id
                foreach (var item in bG_IHS_division.BG_IHS_rel_division.Where(x => x.id > 0).ToList())
                {
                    var relBD = db.BG_IHS_rel_division.Find(item.id);

                    //actualiza los valores del objeto de la BD
                    relBD.vehicle = item.vehicle;
                    relBD.production_nameplate = item.production_nameplate;
                    relBD.porcentaje = item.porcentaje;

                }
                db.SaveChanges();

                return RedirectToAction("Index", new { version = bG_IHS_division.id_ihs_version });
            }
            ViewBag.id_ihs_item = AddFirstItem(new SelectList(db.BG_IHS_item.Where(x => x.id_ihs_version == bG_IHS_division.id_ihs_version), nameof(BG_IHS_item.id), nameof(BG_IHS_item.ConcatCodigo), bG_IHS_division.id_ihs_item), textoPorDefecto: "-- Seleccionar --");
            ViewBag.VersionIHS = db.BG_IHS_versiones.Find(bG_IHS_division.id_ihs_version);
            return View(bG_IHS_division);
        }

        /// <summary>
        /// Método que retorna las filas de las tablas de combinacion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult GetRows(int? id_ihs, string demanda, float? porcentaje)
        {

            var cabeceraDemanda = Portal_2_0.Models.BG_IHS_UTIL.GetCabecera();
            var cabeceraCuartos = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraCuartos();
            var cabeceraAnios = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAnios();
            var cabeceraAniosFY = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAniosFY();

            if (porcentaje == null)
                porcentaje = 0;

            float porcentaje_scrap = 1 + (porcentaje.Value / 100.0f);

            string resultString = string.Empty;


            var ihs = db.BG_IHS_item.Find(id_ihs);

            var demandaMeses = ihs.GetDemanda(cabeceraDemanda, demanda);

            if (ihs != null)
                resultString += @"<tr>
                                        <td>" + 1 + @"</td>
                                        <td>" + ihs.origen + @"</td>
                                        <td>" + ihs.manufacturer_group + @"</td>
                                        <td>" + ihs.production_plant + @"</td>
                                        <td>" + ihs.production_brand + @"</td>
                                        <td>" + ihs.program + @"</td>                                     
                                        <td>" + ihs.vehicle + @"</td>
                                        <td>" + ihs.production_nameplate + @"</td>
                                        <td>" + (ihs.sop_start_of_production.HasValue ? ihs.sop_start_of_production.Value.ToShortDateString() : String.Empty) + @"</td>                                        
                                        <td>" + (ihs.eop_end_of_production.HasValue ? ihs.eop_end_of_production.Value.ToShortDateString() : String.Empty) + @"</td>
                                        <td>" + porcentaje + @"%</td>";

            string clase_demanda = "valores-" + demanda;

            //obtiene la demanda por meses 
            foreach (var item_demanda in demandaMeses)
            {

                resultString += @"<td class="" " + (
                                item_demanda != null ? item_demanda.origen_datos == Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER ? "fondo-customer"
                                : item_demanda.origen_datos == Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL ? "fondo-original"
                                : "fondo-default"
                                : "fondo-default"
                                ) + @" " + clase_demanda + @" "" >" +
                                  (item_demanda != null ? (item_demanda.cantidad * porcentaje_scrap).ToString() : String.Empty)
                                + "</td>";

            }
            //obtiene la demanda por cuartos 
            foreach (var item_cuarto in ihs.GetCuartos(demandaMeses, cabeceraCuartos, demanda))
            {

                resultString += @"<td class="" " + (
                                item_cuarto != null ? item_cuarto.origen_datos == Enum_BG_origen_cuartos.Calculado ? "fondo-cuarto-calculado"
                                : item_cuarto.origen_datos == Enum_BG_origen_cuartos.IHS ? "fondo-cuarto-ihs"
                                : "fondo-default"
                                : "fondo-default"
                                ) + @" " + clase_demanda + @" "" >" +
                                  (item_cuarto != null ? (item_cuarto.cantidad * porcentaje_scrap).ToString() : String.Empty)
                                + "</td>";

            }
            //obtiene la demanda por años Ene-Dic 
            foreach (var item_anio in ihs.GetAnios(demandaMeses, cabeceraAnios, demanda))
            {
                resultString += @"<td class="" " + (
                               item_anio != null ? item_anio.origen_datos == Enum_BG_origen_anios.Calculado ? "fondo-cuarto-calculado"
                                : item_anio.origen_datos == Enum_BG_origen_anios.IHS ? "fondo-cuarto-ihs"
                                : "fondo-default"
                                : "fondo-default"
                                ) + @" " + clase_demanda + @" "" >" +
                                  (item_anio != null ? (item_anio.cantidad * porcentaje_scrap).ToString() : String.Empty)
                                + "</td>";

            }
            //obtiene la demanda por años FY 
            foreach (var item_anio in ihs.GetAniosFY(demandaMeses, cabeceraAniosFY, demanda))
            {

                resultString += @"<td class="" " + (
                               item_anio != null ? item_anio.origen_datos == Enum_BG_origen_anios.Calculado ? "fondo-cuarto-calculado"
                                : item_anio.origen_datos == Enum_BG_origen_anios.IHS ? "fondo-cuarto-ihs"
                                : "fondo-default"
                                : "fondo-default"
                                ) + @" " + clase_demanda + @" "" >" +
                                   (item_anio != null ? (item_anio.cantidad * porcentaje_scrap).ToString() : String.Empty)
                                + "</td>";
            }

            resultString += "</tr>";

            //inicializa la lista de objetos
            var result = new object[1];

            result[0] = new { value = resultString };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: BG_IHS_division/Disable/5
        public ActionResult Disable(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_division bG_IHS_division = db.BG_IHS_division.Find(id);
            if (bG_IHS_division == null)
            {
                return HttpNotFound();
            }

            //convierte el porcentaje
            foreach (var rel in bG_IHS_division.BG_IHS_rel_division)
                rel.porcentaje = (int)(rel.porcentaje * 100);


            return View(bG_IHS_division);
        }

        // POST: BG_IHS_division/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            BG_IHS_division item = db.BG_IHS_division.Find(id);
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
                return RedirectToAction("Index", new { version = item.id_ihs_version });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Index", new { version = item.id_ihs_version });
            }
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.DISABLED, TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("Index", new { version = item.id_ihs_version });
        }

        // GET: BG_IHS_division/Enable/5
        public ActionResult Enable(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_division bG_IHS_division = db.BG_IHS_division.Find(id);
            if (bG_IHS_division == null)
            {
                return HttpNotFound();
            }

            //convierte el porcentaje
            foreach (var rel in bG_IHS_division.BG_IHS_rel_division)
                rel.porcentaje = (int)(rel.porcentaje * 100);


            return View(bG_IHS_division);
        }

        // POST: BG_IHS_division/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            BG_IHS_division item = db.BG_IHS_division.Find(id);
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
                return RedirectToAction("Index", new { version = item.id_ihs_version });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Index", new { version = item.id_ihs_version });
            }
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.ENABLED, TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("Index", new { version = item.id_ihs_version });
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
