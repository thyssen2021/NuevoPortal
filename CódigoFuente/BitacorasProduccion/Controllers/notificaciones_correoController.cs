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
    public class notificaciones_correoController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: notificaciones_correo
        public ActionResult Index(string descripcion, string nombre, string num_empleado, int pagina = 1)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listado = db.notificaciones_correo
                       .Where(x => ((x.empleados != null && (x.empleados.numeroEmpleado).Contains(num_empleado)) || String.IsNullOrEmpty(num_empleado))
                                   && (x.descripcion.Contains(descripcion) || String.IsNullOrEmpty(descripcion))
                                   && ((x.empleados != null && (x.empleados.nombre + " " + x.empleados.apellido1 + " " + x.empleados.apellido2).Contains(nombre)) || String.IsNullOrEmpty(nombre))
                       )
                       .OrderBy(x => x.descripcion)
                       .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                      .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.notificaciones_correo
                          .Where(x => ((x.empleados != null && (x.empleados.numeroEmpleado).Contains(num_empleado)) || String.IsNullOrEmpty(num_empleado))
                                   && (x.descripcion.Contains(descripcion) || String.IsNullOrEmpty(descripcion))
                                   && ((x.empleados != null && (x.empleados.nombre + " " + x.empleados.apellido1 + " " + x.empleados.apellido2).Contains(nombre)) || String.IsNullOrEmpty(nombre))
                       )
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["descripcion"] = descripcion;
                routeValues["nombre"] = nombre;
                routeValues["num_empleado"] = num_empleado;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: notificaciones_correo/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                notificaciones_correo notificaciones_correo = db.notificaciones_correo.Find(id);
                if (notificaciones_correo == null)
                {
                    return HttpNotFound();
                }
                return View(notificaciones_correo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // GET: notificaciones_correo/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                ViewBag.clave_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"));
                ViewBag.id_empleado = AddFirstItem(new SelectList(items: db.empleados.Where(x => x.activo.HasValue && x.activo.Value),
                                                                  "id",
                                                                  "ConcatNumEmpleadoNombre"));
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: notificaciones_correo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(notificaciones_correo notificaciones_correo)
        {
            //busca si existe in objeto con la misma descripcion
            notificaciones_correo item_busca = db.notificaciones_correo.Where(s =>
                s.descripcion.ToUpper() == notificaciones_correo.descripcion.ToUpper()
                && s.id_empleado == notificaciones_correo.id_empleado
                && s.correo == notificaciones_correo.correo
                && s.clave_planta == notificaciones_correo.clave_planta
                )
                .FirstOrDefault();

            if (item_busca != null)
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores.");

            if (ModelState.IsValid)
            {
                notificaciones_correo.activo = true;
                notificaciones_correo.descripcion = notificaciones_correo.descripcion.ToUpper();
                db.notificaciones_correo.Add(notificaciones_correo);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");

            }

            ViewBag.clave_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"));
            ViewBag.id_empleado = AddFirstItem(new SelectList(items: db.empleados.Where(x => x.activo.HasValue && x.activo.Value),
                                                              "id",
                                                              "ConcatNumEmpleadoNombre"));
            return View(notificaciones_correo);
        }

        // GET: notificaciones_correo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                notificaciones_correo notificaciones_correo = db.notificaciones_correo.Find(id);
                if (notificaciones_correo == null)
                {
                    return View("../Error/NotFound");
                }
                ViewBag.clave_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion", selectedValue: notificaciones_correo.clave_planta.ToString()));
                ViewBag.id_empleado = AddFirstItem(new SelectList(items: db.empleados.Where(x => x.activo.HasValue && x.activo.Value),
                                                                  "id",
                                                                  "ConcatNumEmpleadoNombre",
                                                                  selectedValue: notificaciones_correo.id_empleado.ToString()
                                                                  ));
                //manda el correo en caso de que haya empleado

                if (notificaciones_correo.empleados != null)
                    notificaciones_correo.correo = notificaciones_correo.empleados.correo;

                return View(notificaciones_correo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: notificaciones_correo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_empleado,descripcion,activo,correo,clave_planta")] notificaciones_correo notificaciones_correo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notificaciones_correo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clave_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion", selectedValue: notificaciones_correo.clave_planta.ToString()));
            ViewBag.id_empleado = AddFirstItem(new SelectList(items: db.empleados.Where(x => x.activo.HasValue && x.activo.Value),
                                                              "id",
                                                              "ConcatNumEmpleadoNombre",
                                                              selectedValue: notificaciones_correo.id_empleado.ToString()
                                                              ));
            return View(notificaciones_correo);
        }

        // GET: notificaciones_correo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (TieneRol(TipoRoles.IT_CATALOGOS))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                notificaciones_correo notificaciones_correo = db.notificaciones_correo.Find(id);
                if (notificaciones_correo == null)
                {
                    return HttpNotFound();
                }
                return View(notificaciones_correo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // POST: notificaciones_correo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            notificaciones_correo notificaciones_correo = db.notificaciones_correo.Find(id);
            db.notificaciones_correo.Remove(notificaciones_correo);
            db.SaveChanges();
            TempData["Mensaje"] = new MensajesSweetAlert("Se ha eliminado correctamente el registro.", TipoMensajesSweetAlerts.SUCCESS);
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
