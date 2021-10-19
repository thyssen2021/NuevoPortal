using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class ProduccionRegistrosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: ProduccionRegistros
        public ActionResult Index(string planta, string linea, int pagina = 1)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro
          
                var produccion_registros = db.produccion_registros.Include(p => p.plantas).Include(p => p.produccion_datos_entrada).Include(p => p.produccion_lineas).Include(p => p.produccion_lotes).Include(p => p.produccion_operadores).Include(p => p.produccion_supervisores).Include(p => p.produccion_turnos).Where(x =>
                        x.activo == true
                        &&   (!String.IsNullOrEmpty(linea) && x.id_linea.ToString().Contains(linea))
                        && (!String.IsNullOrEmpty(planta) && x.clave_planta.ToString().Contains(planta))
                        )
                    .OrderByDescending(x => x.fecha)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.produccion_registros.Include(p => p.plantas).Include(p => p.produccion_datos_entrada).Include(p => p.produccion_lineas).Include(p => p.produccion_lotes).Include(p => p.produccion_operadores).Include(p => p.produccion_supervisores).Include(p => p.produccion_turnos).Where(x => 
                        x.activo==true
                        && (!String.IsNullOrEmpty(linea) && x.id_linea.ToString().Contains(linea))
                        && (!String.IsNullOrEmpty(planta) && x.clave_planta.ToString().Contains(planta))
                       ).Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["planta"] = planta;
                routeValues["linea"] = linea;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.linea = new SelectList(db.produccion_lineas.Where(p => p.activo == true), "id", "linea");
                ViewBag.planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.Paginacion = paginacion;

                return View(produccion_registros);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }            
        }

        // GET: ProduccionRegistros/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            produccion_registros produccion_registros = db.produccion_registros.Find(id);
            if (produccion_registros == null)
            {
                return HttpNotFound();
            }
            return View(produccion_registros);
        }

        // GET: ProduccionRegistros/Create
        public ActionResult Create(int? planta, int? linea)
        {

            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                //si no hay parámetros retorna al inxdex
                if (planta==null || linea==null) {
                    TempData["Mensaje"] = new MensajesSweetAlert("Verifique los valores de planta y línea.", TipoMensajesSweetAlerts.WARNING);
                    return RedirectToAction("Index");
                }

                //verifica si hay planta
                plantas plantas = db.plantas.Find(planta);
                if (plantas == null) {
                    TempData["Mensaje"] = new MensajesSweetAlert("No existe la planta.", TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Index");
                }

                //verifica si hay linea
                produccion_lineas lineas = db.produccion_lineas.Find(linea);
                if (lineas == null)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("No existe la línea de producción.", TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Index");
                }

                //obtiene el listado de turnos de la planata
                var turnos = db.produccion_turnos.Where(x => x.clave_planta == planta);

                
                //recorre los turnos para identificar que turno es
                produccion_turnos turno=null;
                foreach (produccion_turnos t in turnos){
                    if (TimeSpanUtil.CalculateDalUren(DateTime.Now,t.hora_inicio, t.hora_fin)){
                        turno = t;
                    }                    
                }

                //verifica si hay turno o si no manda mensaje de advertencia
                if (turno != null)
                    ViewBag.Turno = turno;
                else {
                    TempData["Mensaje"] = new MensajesSweetAlert("No hay un horario asignado para la hora actual.", TipoMensajesSweetAlerts.WARNING);
                    return RedirectToAction("Index");
                }
               
                ViewBag.Planta = plantas;
                ViewBag.Linea = lineas;
                ViewBag.sap_platina = ComboSelect.obtieneMaterial_BOM();
                ViewBag.sap_rollo = ComboSelect.obtieneRollo_BOM();
                ViewBag.id_supervisor = ComboSelect.obtieneSupervisoresPlanta(planta.Value);
                ViewBag.id_operador = ComboSelect.obtieneOperadorPorLinea(linea.Value);
                
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: ProduccionRegistros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,clave_planta,id_linea,id_operador,id_supervisor,id_turno,sap_platina,sap_rollo,fecha,activo")] produccion_registros produccion_registros, FormCollection collection)
        {

            //valores enviados previamente
            String c_sap_platina = String.Empty;
            if (!String.IsNullOrEmpty(collection["sap_platina"]))
                c_sap_platina = collection["sap_platina"].ToString();

            //valores enviados previamente
            String c_sap_rollo = String.Empty;
            if (!String.IsNullOrEmpty(collection["sap_rollo"]))
                c_sap_rollo = collection["sap_rollo"].ToString();

            //valores enviados previamente
            int c_id_supervisor = 0;
            if (!String.IsNullOrEmpty(collection["id_supervisor"]))
                Int32.TryParse(collection["id_supervisor"].ToString(), out c_id_supervisor);

            int c_id_operador = 0;
            if (!String.IsNullOrEmpty(collection["id_operador"]))
                Int32.TryParse(collection["id_operador"].ToString(), out c_id_operador);

            //si el modelo es válido
            if (ModelState.IsValid)
            {
                produccion_registros.activo = true;
                produccion_registros.fecha = DateTime.Now;
                db.produccion_registros.Add(produccion_registros);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            //si no es válido
            //en este punto ya se válido que existen en la BD
            plantas plantas = db.plantas.Find(produccion_registros.clave_planta);
            produccion_lineas lineas = db.produccion_lineas.Find(produccion_registros.id_linea.Value);
            var turnos = db.produccion_turnos.Where(x => x.clave_planta == produccion_registros.clave_planta.Value);
            produccion_turnos turno = null;

            foreach (produccion_turnos t in turnos)
            {
                if (TimeSpanUtil.CalculateDalUren(DateTime.Now, t.hora_inicio, t.hora_fin))
                {
                    turno = t;
                }
            }

            //verifica si hay turno o si no manda mensaje de advertencia
            if (turno != null)
                ViewBag.Turno = turno;
            else
            {
                TempData["Mensaje"] = new MensajesSweetAlert("No hay un horario asignado para la hora actual.", TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("Index");
            }

            ViewBag.Planta = plantas;
            ViewBag.Linea = lineas;
            ViewBag.sap_platina = ComboSelect.obtieneMaterial_BOM();
            ViewBag.sap_rollo = ComboSelect.obtieneRollo_BOM();
            ViewBag.id_supervisor = ComboSelect.obtieneSupervisoresPlanta(produccion_registros.clave_planta.Value);
            ViewBag.id_operador = ComboSelect.obtieneOperadorPorLinea(produccion_registros.id_linea.Value);
            //para completar_valores previos
            ViewBag.c_sap_platina = c_sap_platina;
            ViewBag.c_sap_rollo = c_sap_rollo;
            ViewBag.c_id_supervisor = c_id_supervisor;
            ViewBag.c_id_operador = c_id_operador;

           
            return View(produccion_registros);
        }

        // GET: ProduccionRegistros/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            produccion_registros produccion_registros = db.produccion_registros.Find(id);
            if (produccion_registros == null)
            {
                return HttpNotFound();
            }
            ViewBag.clave_planta = new SelectList(db.plantas, "clave", "descripcion", produccion_registros.clave_planta);
            ViewBag.id = new SelectList(db.produccion_datos_entrada, "id_produccion_registro", "orden_sap", produccion_registros.id);
            ViewBag.id_linea = new SelectList(db.produccion_lineas, "id", "linea", produccion_registros.id_linea);
            ViewBag.id = new SelectList(db.produccion_lotes, "id_produccion_registro", "id_produccion_registro", produccion_registros.id);
            ViewBag.id_operador = new SelectList(db.produccion_operadores, "id", "id", produccion_registros.id_operador);
            ViewBag.id_supervisor = new SelectList(db.produccion_supervisores, "id", "id", produccion_registros.id_supervisor);
            ViewBag.id_turno = new SelectList(db.produccion_turnos, "id", "descripcion", produccion_registros.id_turno);
            return View(produccion_registros);
        }

        // POST: ProduccionRegistros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,clave_planta,id_linea,id_operador,id_supervisor,id_turno,sap_platina,sap_rollo,fecha,activo")] produccion_registros produccion_registros)
        {
            if (ModelState.IsValid)
            {
                db.Entry(produccion_registros).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clave_planta = new SelectList(db.plantas, "clave", "descripcion", produccion_registros.clave_planta);
            ViewBag.id = new SelectList(db.produccion_datos_entrada, "id_produccion_registro", "orden_sap", produccion_registros.id);
            ViewBag.id_linea = new SelectList(db.produccion_lineas, "id", "linea", produccion_registros.id_linea);
            ViewBag.id = new SelectList(db.produccion_lotes, "id_produccion_registro", "id_produccion_registro", produccion_registros.id);
            ViewBag.id_operador = new SelectList(db.produccion_operadores, "id", "id", produccion_registros.id_operador);
            ViewBag.id_supervisor = new SelectList(db.produccion_supervisores, "id", "id", produccion_registros.id_supervisor);
            ViewBag.id_turno = new SelectList(db.produccion_turnos, "id", "descripcion", produccion_registros.id_turno);
            return View(produccion_registros);
        }

        // GET: ProduccionRegistros/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            produccion_registros produccion_registros = db.produccion_registros.Find(id);
            if (produccion_registros == null)
            {
                return HttpNotFound();
            }
            return View(produccion_registros);
        }

        // POST: ProduccionRegistros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            produccion_registros produccion_registros = db.produccion_registros.Find(id);
            db.produccion_registros.Remove(produccion_registros);
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
    }
}
