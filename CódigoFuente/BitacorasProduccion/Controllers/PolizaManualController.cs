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
    public class PolizaManualController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: PolizaManual
        public ActionResult CapturistaCreadas(int pagina = 1)
        {

            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                var listado = db.poliza_manual.Include(p => p.biblioteca_digital).Include(p => p.biblioteca_digital1).Include(p => p.currency).Include(p => p.empleados).Include(p => p.plantas).Include(p => p.PM_autorizadores).Include(p => p.PM_tipo_poliza).Include(p => p.PM_validadores)
                   .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual.Include(p => p.biblioteca_digital).Include(p => p.biblioteca_digital1).Include(p => p.currency).Include(p => p.empleados).Include(p => p.plantas).Include(p => p.PM_autorizadores).Include(p => p.PM_tipo_poliza).Include(p => p.PM_validadores).Count();

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



                //Viewbags para los botones
                ViewBag.Title = "Listado Pólizas Creadas";
                ViewBag.SegundoNivel = "PM_registro";
                ViewBag.Create = true;

                return View("ListadoPolizas", listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
           
        }

        // GET: PolizaManual/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            poliza_manual poliza_manual = db.poliza_manual.Find(id);
            if (poliza_manual == null)
            {
                return HttpNotFound();
            }
            return View(poliza_manual);
        }

        // GET: PolizaManual/Create
        public ActionResult Create()
        {

            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();
                ViewBag.Solicitante = empleado;


                ViewBag.currency_iso = AddFirstItem(new SelectList(db.currency.Where(x => x.activo), "CurrencyISO", "CocatCurrency"), selected:"USD");
                ViewBag.id_PM_tipo_poliza = AddFirstItem(new SelectList(db.PM_tipo_poliza.Where(x => x.activo), "id", "descripcion"));
                ViewBag.id_validador = AddFirstItem(new SelectList(db.PM_validadores.Where(x => x.activo), "id", "ConcatNameValidador"));

                return View(new poliza_manual());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


            
        }

        // POST: PolizaManual/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_PM_tipo_poliza,currency_iso,id_planta,id_elaborador,id_validador,id_autorizador,id_documento_soporte,id_documento_registro,numero_documento_sap,fecha_creacion,fecha_documento,fecha_validacion,fecha_autorizacion,comentario_rechazo,descripcion_poliza,estatus")] poliza_manual poliza_manual)
        {
            if (ModelState.IsValid)
            {
                db.poliza_manual.Add(poliza_manual);
             //   db.SaveChanges();
                return RedirectToAction("Index");
            }

            //obtiene el usuario logeado
            empleados empleado = obtieneEmpleadoLogeado();
            ViewBag.Solicitante = empleado;
            
            ViewBag.currency_iso = AddFirstItem(new SelectList(db.currency.Where(x => x.activo), "CurrencyISO", "CocatCurrency"));
            ViewBag.id_PM_tipo_poliza = AddFirstItem(new SelectList(db.PM_tipo_poliza.Where(x => x.activo), "id", "descripcion"));
            ViewBag.id_validador = AddFirstItem(new SelectList(db.PM_validadores.Where(x => x.activo), "id", "ConcatNameValidador"));
            return View(poliza_manual);
        }

        // GET: PolizaManual/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            poliza_manual poliza_manual = db.poliza_manual.Find(id);
            if (poliza_manual == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_documento_registro = new SelectList(db.biblioteca_digital, "Id", "Nombre", poliza_manual.id_documento_registro);
            ViewBag.id_documento_soporte = new SelectList(db.biblioteca_digital, "Id", "Nombre", poliza_manual.id_documento_soporte);
            ViewBag.currency_iso = new SelectList(db.currency, "CurrencyISO", "CurrencyName", poliza_manual.currency_iso);
            ViewBag.id_elaborador = new SelectList(db.empleados, "id", "numeroEmpleado", poliza_manual.id_elaborador);
            ViewBag.id_planta = new SelectList(db.plantas, "clave", "descripcion", poliza_manual.id_planta);
            ViewBag.id_autorizador = new SelectList(db.PM_autorizadores, "id", "id", poliza_manual.id_autorizador);
            ViewBag.id_PM_tipo_poliza = new SelectList(db.PM_tipo_poliza, "id", "descripcion", poliza_manual.id_PM_tipo_poliza);
            ViewBag.id_validador = new SelectList(db.PM_validadores, "id", "id", poliza_manual.id_validador);
            return View(poliza_manual);
        }

        // POST: PolizaManual/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_PM_tipo_poliza,currency_iso,id_planta,id_elaborador,id_validador,id_autorizador,id_documento_soporte,id_documento_registro,numero_documento_sap,fecha_creacion,fecha_documento,fecha_validacion,fecha_autorizacion,comentario_rechazo,descripcion_poliza,estatus")] poliza_manual poliza_manual)
        {
            if (ModelState.IsValid)
            {
                db.Entry(poliza_manual).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_documento_registro = new SelectList(db.biblioteca_digital, "Id", "Nombre", poliza_manual.id_documento_registro);
            ViewBag.id_documento_soporte = new SelectList(db.biblioteca_digital, "Id", "Nombre", poliza_manual.id_documento_soporte);
            ViewBag.currency_iso = new SelectList(db.currency, "CurrencyISO", "CurrencyName", poliza_manual.currency_iso);
            ViewBag.id_elaborador = new SelectList(db.empleados, "id", "numeroEmpleado", poliza_manual.id_elaborador);
            ViewBag.id_planta = new SelectList(db.plantas, "clave", "descripcion", poliza_manual.id_planta);
            ViewBag.id_autorizador = new SelectList(db.PM_autorizadores, "id", "id", poliza_manual.id_autorizador);
            ViewBag.id_PM_tipo_poliza = new SelectList(db.PM_tipo_poliza, "id", "descripcion", poliza_manual.id_PM_tipo_poliza);
            ViewBag.id_validador = new SelectList(db.PM_validadores, "id", "id", poliza_manual.id_validador);
            return View(poliza_manual);
        }

        // GET: PolizaManual/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            poliza_manual poliza_manual = db.poliza_manual.Find(id);
            if (poliza_manual == null)
            {
                return HttpNotFound();
            }
            return View(poliza_manual);
        }

        // POST: PolizaManual/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            poliza_manual poliza_manual = db.poliza_manual.Find(id);
            db.poliza_manual.Remove(poliza_manual);
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
