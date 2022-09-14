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
    public class GV_solicitudController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: GV_solicitud
        public ActionResult Solicitudes()
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD))            
                return View("../Home/ErrorPermisos");


            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["demo"] = "demo";

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = 1,
                TotalDeRegistros = 1,
                RegistrosPorPagina = 20,
                ValoresQueryString = routeValues
            };

            ViewBag.Paginacion = paginacion;


            var gV_solicitud = db.GV_solicitud;
            return View(gV_solicitud.ToList());

        }

        // GET: GV_solicitud/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }
            return View(gV_solicitud);
        }

        // GET: GV_solicitud/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD))
                return View("../Home/ErrorPermisos");

            //obtiene el usuario logeado
            empleados solicitante = obtieneEmpleadoLogeado();
          
            GV_solicitud solicitud = new GV_solicitud { 
                id_solicitante = solicitante.id,
                empleados5 = solicitante,
                fecha_salida = DateTime.Now,
                fecha_regreso = DateTime.Now,                
            };

            ViewBag.iso_moneda_extranjera = AddFirstItem(new SelectList(db.currency.Where(x => x.activo && (x.CurrencyISO == "USD" || x.CurrencyISO =="EUR")), "CurrencyISO", "CocatCurrency"), selected: "USD");
            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value && x.planta_clave == solicitante.planta_clave), "id", "ConcatNumEmpleadoNombre"), selected: solicitante.id.ToString());
            ViewBag.id_medio_transporte = AddFirstItem(new SelectList(db.GV_medios_transporte.Where(x => x.activo), "id", "descripcion"), selected: solicitante.id.ToString());
           
            return View(solicitud);
        }

        // POST: GV_solicitud/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GV_solicitud gV_solicitud)
        {
            if (ModelState.IsValid)
            {
                db.GV_solicitud.Add(gV_solicitud);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.iso_moneda_extranjera = new SelectList(db.currency, "CurrencyISO", "CurrencyName", gV_solicitud.iso_moneda_extranjera);
            ViewBag.id_contabilidad = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_contabilidad);
            ViewBag.id_controlling = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_controlling);
            ViewBag.id_empleado = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_empleado);
            ViewBag.id_jefe_directo = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_jefe_directo);
            ViewBag.id_nomina = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_nomina);
            ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_solicitante);
            ViewBag.id_medio_transporte = new SelectList(db.GV_medios_transporte, "id", "descripcion", gV_solicitud.id_medio_transporte);
            return View(gV_solicitud);
        }

        // GET: GV_solicitud/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }
            ViewBag.iso_moneda_extranjera = new SelectList(db.currency, "CurrencyISO", "CurrencyName", gV_solicitud.iso_moneda_extranjera);
            ViewBag.id_contabilidad = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_contabilidad);
            ViewBag.id_controlling = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_controlling);
            ViewBag.id_empleado = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_empleado);
            ViewBag.id_jefe_directo = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_jefe_directo);
            ViewBag.id_nomina = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_nomina);
            ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_solicitante);
            ViewBag.id_medio_transporte = new SelectList(db.GV_medios_transporte, "id", "descripcion", gV_solicitud.id_medio_transporte);
            return View(gV_solicitud);
        }

        // POST: GV_solicitud/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_solicitante,id_empleado,id_jefe_directo,id_controlling,id_contabilidad,id_nomina,fecha_solicitud,tipo_viaje,origen,destino,fecha_salida,fecha_regreso,id_medio_transporte,medio_transporte_otro,moneda_extranjera,moneda_extranjera_comentarios,moneda_extranjera_monto,iso_moneda_extranjera,boletos_avion,boletos_avion_comentarios,hospedaje,hospedaje_comentarios,fecha_aceptacion_jefe_area,fecha_aceptacion_controlling,fecha_aceptacion_contabilidad,fecha_aceptacion_nomina,comentario_rechazo,estatus")] GV_solicitud gV_solicitud)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gV_solicitud).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.iso_moneda_extranjera = new SelectList(db.currency, "CurrencyISO", "CurrencyName", gV_solicitud.iso_moneda_extranjera);
            ViewBag.id_contabilidad = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_contabilidad);
            ViewBag.id_controlling = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_controlling);
            ViewBag.id_empleado = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_empleado);
            ViewBag.id_jefe_directo = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_jefe_directo);
            ViewBag.id_nomina = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_nomina);
            ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", gV_solicitud.id_solicitante);
            ViewBag.id_medio_transporte = new SelectList(db.GV_medios_transporte, "id", "descripcion", gV_solicitud.id_medio_transporte);
            return View(gV_solicitud);
        }

        // GET: GV_solicitud/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }
            return View(gV_solicitud);
        }

        // POST: GV_solicitud/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            db.GV_solicitud.Remove(gV_solicitud);
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
