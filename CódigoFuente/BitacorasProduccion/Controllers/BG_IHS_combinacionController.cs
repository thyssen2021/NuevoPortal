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
    public class BG_IHS_combinacionController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_IHS_combinacion
        public ActionResult Index(int pagina = 1)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro


            var listado = db.BG_IHS_combinacion
                .OrderByDescending(x => x.id)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.BG_IHS_combinacion
                .Count();

            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            //routeValues["material"] = material;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.Paginacion = paginacion;

            return View(db.BG_IHS_combinacion.ToList());
        }

        // GET: BG_IHS_combinacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_combinacion bG_IHS_combinacion = db.BG_IHS_combinacion.Find(id);
            if (bG_IHS_combinacion == null)
            {
                return HttpNotFound();
            }
            return View(bG_IHS_combinacion);
        }

        // GET: BG_IHS_combinacion/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            List<BG_IHS_item> listadoIHSItems = db.BG_IHS_item.ToList();
            ViewBag.listadoIHSItems = listadoIHSItems;

            var model = new BG_IHS_combinacion();

            return View(model);
        }

        // POST: BG_IHS_combinacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BG_IHS_combinacion bG_IHS_combinacion)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (bG_IHS_combinacion.BG_IHS_rel_combinacion.Count == 0)
                ModelState.AddModelError("", "Debe agregar al menos un elemento de IHS.");

            if (db.BG_IHS_combinacion.Any(x => x.vehicle == bG_IHS_combinacion.vehicle))
                ModelState.AddModelError("", "Ya existe una combinación con la misma clave de vehículo.");

            //compara vehiculo con concatCodigo
            List<string> codigosIHS = db.BG_IHS_item.ToList().Select(x => x.ConcatCodigo).ToList();
            if (codigosIHS.Any(x => x == bG_IHS_combinacion.vehicle))
                ModelState.AddModelError("", "Ya existe un registro de IHS con la misma clave de vehículo.");

            if (db.BG_IHS_rel_division.Any(x => x.vehicle == bG_IHS_combinacion.vehicle))
                ModelState.AddModelError("", "Ya existe una división con la misma clave de vehículo.");

            //verificar si no hay ids repetidos
            var query = bG_IHS_combinacion.BG_IHS_rel_combinacion.GroupBy(x => x.id_ihs_item)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            if (query.Count > 0)
                ModelState.AddModelError("", "Existen elementos repetidos en la combinación de IHS, favor de verificarlo.");

            if (ModelState.IsValid)
            {
                bG_IHS_combinacion.activo = true;
                db.BG_IHS_combinacion.Add(bG_IHS_combinacion);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se han guardado los cambios correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            List<BG_IHS_item> listadoIHSItems = db.BG_IHS_item.ToList();
            ViewBag.listadoIHSItems = listadoIHSItems;

            return View(bG_IHS_combinacion);
        }

        // GET: BG_IHS_combinacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_combinacion bG_IHS_combinacion = db.BG_IHS_combinacion.Find(id);
            if (bG_IHS_combinacion == null)
            {
                return HttpNotFound();
            }
            return View(bG_IHS_combinacion);
        }

        // POST: BG_IHS_combinacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,vehicle,production_brand,sop_start_of_production,eop_end_of_production,comentario,activo")] BG_IHS_combinacion bG_IHS_combinacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bG_IHS_combinacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bG_IHS_combinacion);
        }

        // GET: BG_IHS_combinacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_IHS_combinacion bG_IHS_combinacion = db.BG_IHS_combinacion.Find(id);
            if (bG_IHS_combinacion == null)
            {
                return HttpNotFound();
            }
            return View(bG_IHS_combinacion);
        }

        // POST: BG_IHS_combinacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BG_IHS_combinacion bG_IHS_combinacion = db.BG_IHS_combinacion.Find(id);
            db.BG_IHS_combinacion.Remove(bG_IHS_combinacion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Método que retorna las filas de las tablas de combinacion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult GetRows(int[] data)
        {
            var cabeceraDemanda = Portal_2_0.Models.BG_IHS_UTIL.GetCabecera();
            var cabeceraCuartos = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraCuartos();
            var cabeceraAnios = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAnios();
            var cabeceraAniosFY = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAniosFY();

          //  List<int> list = data.ToList().Distinct().ToList();
            List<int> list = data.ToList();

            string resultString = string.Empty;

            foreach (var item in list) {
                var ihs = db.BG_IHS_item.Find(item);

                if (ihs != null)
                    resultString += @"<tr>
                                        <td>"+ihs.id+@"</td>
                                        <td>"+ihs.origen+@"</td>
                                        <td>"+ihs.manufacturer_group+@"</td>
                                        <td>"+ihs.production_plant+@"</td>
                                        <td>"+ihs.production_brand+@"</td>
                                        <td>"+ihs.program+@"</td>
                                        <td>"+ihs.production_nameplate+@"</td>
                                        <td>"+ihs.vehicle+@"</td>
                                        <td>"+(ihs.sop_start_of_production.HasValue?ihs.sop_start_of_production.Value.ToShortDateString():String.Empty) +@"</td>                                        
                                        <td>"+(ihs.eop_end_of_production.HasValue?ihs.eop_end_of_production.Value.ToShortDateString():String.Empty) +@"</td>                                        

                                </tr>";
            }

            //inicializa la lista de objetos
            var result = new object[1];

            result[0] = new { value = resultString};

            return Json(result, JsonRequestBehavior.AllowGet);
        }
                
    }
}
