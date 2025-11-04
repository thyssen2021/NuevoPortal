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
                rel.porcentaje = (decimal?)((float)rel.porcentaje * 100);

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

                // --- LÓGICA DE BLACKOUT ---
                if (bG_IHS_division.BG_IHS_division_blackout != null)
                {
                    foreach (var blackoutForm in bG_IHS_division.BG_IHS_division_blackout)
                    {
                        // El input "month" se enlaza como el primer día ("2025-12-01").
                        // Ajustamos la fecha_fin para que sea el ÚLTIMO día de ese mes.
                        blackoutForm.fecha_fin = blackoutForm.fecha_fin.AddMonths(1).AddDays(-1);
                    }
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
                rel.porcentaje = (decimal?)((float)rel.porcentaje * 100);

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

                // --- INICIO LÓGICA DE BLACKOUT ---
                var blackoutsEnBD = db.BG_IHS_division_blackout
                                      .Where(b => b.id_ihs_division == division.id)
                                      .ToList();

                var blackoutsFormulario = bG_IHS_division.BG_IHS_division_blackout ?? new List<BG_IHS_division_blackout>();

                var blackoutsParaEliminar = blackoutsEnBD
                    .Where(b_bd => !blackoutsFormulario.Any(b_form => b_form.id == b_bd.id))
                    .ToList();

                foreach (var blackout in blackoutsParaEliminar)
                {
                    db.Entry(blackout).State = EntityState.Deleted;
                }

                foreach (var blackoutForm in blackoutsFormulario)
                {
                    // El input "month" ("2025-12") se enlaza como el primer día ("2025-12-01").
                    // Ajustamos la fecha_fin para que sea el ÚLTIMO día de ese mes.
                    blackoutForm.fecha_fin = blackoutForm.fecha_fin.AddMonths(1).AddDays(-1);

                    if (blackoutForm.id > 0) // Actualizar
                    {
                        var blackoutBD = blackoutsEnBD.FirstOrDefault(b => b.id == blackoutForm.id);
                        if (blackoutBD != null)
                        {
                            blackoutBD.fecha_inicio = blackoutForm.fecha_inicio;
                            blackoutBD.fecha_fin = blackoutForm.fecha_fin; // Ya está ajustada
                            blackoutBD.comentario = blackoutForm.comentario;
                            db.Entry(blackoutBD).State = EntityState.Modified;
                        }
                    }
                    else // Agregar
                    {
                        blackoutForm.id_ihs_division = division.id;
                        db.BG_IHS_division_blackout.Add(blackoutForm); // fecha_fin ya está ajustada
                    }
                }
                // --- FIN LÓGICA DE BLACKOUT ---

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
        /// Metodo que rotorna las filas de demanda en formato html
        /// </summary>
        /// <param name="id_ihs"></param>
        /// <param name="demanda"></param>
        /// <param name="porcentaje"></param>
        /// <param name="id_ihs_division"></param>
        /// <param name="blackouts"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRows(int? id_ihs, string demanda, float? porcentaje, int? id_ihs_division, List<BlackoutPeriodoVM> blackouts)
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

            // --- INICIO LÓGICA DE BLACKOUT ---

            // 1. Ajustar la lista de blackouts recibida
            // El JS nos envía "2025-12-01" como fecha_fin. La ajustamos al último día del mes.
            if (blackouts != null)
            {
                foreach (var b in blackouts)
                {
                    b.fecha_fin = b.fecha_fin.AddMonths(1).AddDays(-1);
                }
            }

            // 2. Aplicar blackout a DEMANDA MENSUAL
            var demandaMeses = ihs.GetDemanda(cabeceraDemanda, demanda);
            if (blackouts != null && blackouts.Any())
            {
                // Iteramos sobre la demanda mensual y aplicamos blackouts
                foreach (var item_demanda in demandaMeses)
                {
                    if (item_demanda != null) // Solo procesar si no es nulo
                    {
                        bool enBlackout = false;

                        // Usamos la lista del parámetro
                        foreach (var blackout in blackouts)
                        {
                            // Comprobar si la fecha del item de demanda cae dentro de CUALQUIER periodo de blackout
                            if (item_demanda.fecha.Date >= blackout.fecha_inicio.Date && item_demanda.fecha.Date <= blackout.fecha_fin.Date)
                            {
                                enBlackout = true;
                                break; // Salir del bucle de blackouts, ya encontramos una coincidencia
                            }
                        }

                        if (enBlackout)
                        {
                            item_demanda.cantidad = 0; // ¡Aplicamos el blackout poniendo la cantidad a 0!
                        }
                    }
                }
            }

            // 3. Aplicar blackout a DATOS DE CUARTOS (fallback)
            // Clonamos la lista original de cuartos para no modificar la data de EF.
            var cuartosModificados = ihs.BG_IHS_rel_cuartos.Select(c => new BG_IHS_rel_cuartos
            {
                id = c.id,
                id_ihs_item = c.id_ihs_item,
                cuarto = c.cuarto,
                anio = c.anio,
                cantidad = c.cantidad, // <-- Copiamos el valor original
                fecha_carga = c.fecha_carga
            }).ToList();


            if (blackouts != null && blackouts.Any())
            {
                // Llamamos al helper para que modifique la lista 'cuartosModificados'
                // (Este método debe existir en tu controlador)
                AplicarBlackoutACuartos_MesCompleto(cuartosModificados, blackouts);
            }
            // --- FIN LÓGICA DE BLACKOUT ---


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
            // Se pasa 'cuartosModificados' como parámetro
            foreach (var item_cuarto in ihs.GetCuartos(demandaMeses, cabeceraCuartos, demanda, cuartosModificados))
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
            // Se pasa 'cuartosModificados' como parámetro
            foreach (var item_anio in ihs.GetAnios(demandaMeses, cabeceraAnios, demanda, cuartosModificados))
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
            // Se pasa 'cuartosModificados' como parámetro
            foreach (var item_anio in ihs.GetAniosFY(demandaMeses, cabeceraAniosFY, demanda, cuartosModificados))
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

        /// <summary>
        /// Modifica una lista de cuartos (BG_IHS_rel_cuartos) aplicando la reducción basada en MESES COMPLETOS.
        /// </summary>
        private void AplicarBlackoutACuartos_MesCompleto(List<BG_IHS_rel_cuartos> cuartos, List<BlackoutPeriodoVM> blackouts)
        {
            foreach (var cuarto in cuartos)
            {
                if (!cuarto.cantidad.HasValue || cuarto.cantidad == 0) continue;

                // 1. Determinar los 3 meses que componen este cuarto
                int mesInicioCuarto = (cuarto.cuarto * 3) - 2; // Q1->M1, Q2->M4, Q3->M7, Q4->M10
                DateTime mes1 = new DateTime(cuarto.anio, mesInicioCuarto, 1);
                DateTime mes2 = mes1.AddMonths(1);
                DateTime mes3 = mes1.AddMonths(2);

                var mesesDelCuarto = new List<DateTime> { mes1, mes2, mes3 };
                int mesesEnBlackout = 0;

                // 2. Contar cuántos de esos 3 meses caen en un período de blackout
                foreach (var mes in mesesDelCuarto)
                {
                    bool enBlackout = false;
                    foreach (var blackout in blackouts)
                    {
                        // Comprueba si el primer día del mes está dentro del rango
                        if (mes.Date >= blackout.fecha_inicio.Date && mes.Date <= blackout.fecha_fin.Date)
                        {
                            enBlackout = true;
                            break;
                        }
                    }
                    if (enBlackout)
                    {
                        mesesEnBlackout++;
                    }
                }

                // 3. Aplicar la reducción (0/3, 1/3, 2/3, o 3/3)
                if (mesesEnBlackout > 0)
                {
                    // Usamos decimal para precisión
                    decimal reduccionPct = (decimal)mesesEnBlackout / 3.0m;

                    cuarto.cantidad = (int)Math.Round(cuarto.cantidad.Value * (1.0m - reduccionPct));
                }
            }
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
                rel.porcentaje = (decimal?)((float)rel.porcentaje * 100);


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
                rel.porcentaje = (decimal?)((float)rel.porcentaje * 100);


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
    public class BlackoutPeriodoVM
    {
        public System.DateTime fecha_inicio { get; set; }
        public System.DateTime fecha_fin { get; set; }
    }
}
