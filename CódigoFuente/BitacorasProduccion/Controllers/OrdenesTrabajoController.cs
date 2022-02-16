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
    public class OrdenesTrabajoController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: ListadoUsuario
        public ActionResult ListadoUsuario(string estatus, int? id, int pagina = 1)
        {
            if (TieneRol(TipoRoles.OT_SOLICITUD))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro
               
                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.orden_trabajo
                    .Where(x => x.id_solicitante == empleado.id
                      && (id == null || x.id == id)
                      && (String.IsNullOrEmpty(estatus) || x.estatus.Contains(estatus))
                    )
                    .OrderByDescending(x => x.fecha_solicitud)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.orden_trabajo
                   .Where(x => x.id_solicitante == empleado.id
                      && (id == null || x.id == id)
                      && (String.IsNullOrEmpty(estatus) || x.estatus.Contains(estatus))
                    )
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["id"] = id;
                routeValues["estatus"] = estatus;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                List<string> estatusList = db.orden_trabajo.Select(x => x.estatus).Distinct().ToList();
                //crea un Select  list para el estatus
                List<SelectListItem> newList = new List<SelectListItem>();

                foreach (string statusItem in estatusList)
                {
                    newList.Add(new SelectListItem()
                    {
                        Text = statusItem,
                        Value = statusItem
                    });
                }

                SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);

                ViewBag.estatus = AddFirstItem(selectListItemsStatus,textoPorDefecto: "-- Todos --");
                ViewBag.Paginacion = paginacion;
                //Viewbags para los botones               
                ViewBag.Title = "Listado Solicitudes Creadas";
                ViewBag.SegundoNivel = "mis_solicitudes";
               // ViewBag.Create = true;
                                
                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        //// GET: OrdenesTrabajo
        //public ActionResult Index()
        //{
        //    var orden_trabajo = db.orden_trabajo.Include(o => o.Area).Include(o => o.biblioteca_digital).Include(o => o.biblioteca_digital1).Include(o => o.empleados).Include(o => o.empleados1).Include(o => o.empleados2).Include(o => o.produccion_lineas);
        //    return View(orden_trabajo.ToList());
        //}

        //// GET: OrdenesTrabajo/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    orden_trabajo orden_trabajo = db.orden_trabajo.Find(id);
        //    if (orden_trabajo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(orden_trabajo);
        //}

        //// GET: OrdenesTrabajo/Create
        public ActionResult Create()
        {

            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();
                ViewBag.Solicitante = empleado;

                ViewBag.id_area = new SelectList(db.Area, "clave", "descripcion");
                ViewBag.id_documento_cierre = new SelectList(db.biblioteca_digital, "Id", "Nombre");
                ViewBag.id_documento_solicitud = new SelectList(db.biblioteca_digital, "Id", "Nombre");
                ViewBag.id_asignacion = new SelectList(db.empleados, "id", "numeroEmpleado");
                ViewBag.id_responsable = new SelectList(db.empleados, "id", "numeroEmpleado");
                ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado");
                ViewBag.id_linea = new SelectList(db.produccion_lineas, "id", "linea");
                return View();

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

            
        }

        //// POST: OrdenesTrabajo/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id,id_solicitante,id_asignacion,id_responsable,id_area,id_linea,id_documento_solicitud,id_documento_cierre,fecha_solicitud,nivel_urgencia,titulo,descripcion,fecha_asignacion,fecha_en_proceso,fecha_cierre,estatus,comentario")] orden_trabajo orden_trabajo)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.orden_trabajo.Add(orden_trabajo);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.id_area = new SelectList(db.Area, "clave", "descripcion", orden_trabajo.id_area);
        //    ViewBag.id_documento_cierre = new SelectList(db.biblioteca_digital, "Id", "Nombre", orden_trabajo.id_documento_cierre);
        //    ViewBag.id_documento_solicitud = new SelectList(db.biblioteca_digital, "Id", "Nombre", orden_trabajo.id_documento_solicitud);
        //    ViewBag.id_asignacion = new SelectList(db.empleados, "id", "numeroEmpleado", orden_trabajo.id_asignacion);
        //    ViewBag.id_responsable = new SelectList(db.empleados, "id", "numeroEmpleado", orden_trabajo.id_responsable);
        //    ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", orden_trabajo.id_solicitante);
        //    ViewBag.id_linea = new SelectList(db.produccion_lineas, "id", "linea", orden_trabajo.id_linea);
        //    return View(orden_trabajo);
        //}

        //// GET: OrdenesTrabajo/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    orden_trabajo orden_trabajo = db.orden_trabajo.Find(id);
        //    if (orden_trabajo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.id_area = new SelectList(db.Area, "clave", "descripcion", orden_trabajo.id_area);
        //    ViewBag.id_documento_cierre = new SelectList(db.biblioteca_digital, "Id", "Nombre", orden_trabajo.id_documento_cierre);
        //    ViewBag.id_documento_solicitud = new SelectList(db.biblioteca_digital, "Id", "Nombre", orden_trabajo.id_documento_solicitud);
        //    ViewBag.id_asignacion = new SelectList(db.empleados, "id", "numeroEmpleado", orden_trabajo.id_asignacion);
        //    ViewBag.id_responsable = new SelectList(db.empleados, "id", "numeroEmpleado", orden_trabajo.id_responsable);
        //    ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", orden_trabajo.id_solicitante);
        //    ViewBag.id_linea = new SelectList(db.produccion_lineas, "id", "linea", orden_trabajo.id_linea);
        //    return View(orden_trabajo);
        //}

        //// POST: OrdenesTrabajo/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "id,id_solicitante,id_asignacion,id_responsable,id_area,id_linea,id_documento_solicitud,id_documento_cierre,fecha_solicitud,nivel_urgencia,titulo,descripcion,fecha_asignacion,fecha_en_proceso,fecha_cierre,estatus,comentario")] orden_trabajo orden_trabajo)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(orden_trabajo).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.id_area = new SelectList(db.Area, "clave", "descripcion", orden_trabajo.id_area);
        //    ViewBag.id_documento_cierre = new SelectList(db.biblioteca_digital, "Id", "Nombre", orden_trabajo.id_documento_cierre);
        //    ViewBag.id_documento_solicitud = new SelectList(db.biblioteca_digital, "Id", "Nombre", orden_trabajo.id_documento_solicitud);
        //    ViewBag.id_asignacion = new SelectList(db.empleados, "id", "numeroEmpleado", orden_trabajo.id_asignacion);
        //    ViewBag.id_responsable = new SelectList(db.empleados, "id", "numeroEmpleado", orden_trabajo.id_responsable);
        //    ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", orden_trabajo.id_solicitante);
        //    ViewBag.id_linea = new SelectList(db.produccion_lineas, "id", "linea", orden_trabajo.id_linea);
        //    return View(orden_trabajo);
        //}

        //// GET: OrdenesTrabajo/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    orden_trabajo orden_trabajo = db.orden_trabajo.Find(id);
        //    if (orden_trabajo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(orden_trabajo);
        //}

        //// POST: OrdenesTrabajo/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    orden_trabajo orden_trabajo = db.orden_trabajo.Find(id);
        //    db.orden_trabajo.Remove(orden_trabajo);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
