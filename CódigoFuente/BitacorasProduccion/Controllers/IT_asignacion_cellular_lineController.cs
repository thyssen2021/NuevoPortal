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
    public class IT_asignacion_cellular_lineController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();
        // GET: IT_asignacion_cellular_line/Asignar
        public ActionResult Asignar(int? id_empleado)
        {
            if (!TieneRol(TipoRoles.IT_ASIGNACION_HARDWARE))
                return View("../Home/ErrorPermisos");

            if (id_empleado == null)
            {
                return View("../Error/BadRequest");
            }
            empleados empleado = db.empleados.Find(id_empleado);
            if (empleado == null)
            {
                return View("../Error/NotFound");
            }

            empleados sistemas = obtieneEmpleadoLogeado();

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var model = new IT_asignacion_cellular_line()
            {
                id_empleado = empleado.id,
                id_sistemas = sistemas.id,
                empleados = empleado,
                empleados1 = sistemas,
                fecha_asignacion = DateTime.Now,
                es_asignacion_actual = true
            };

            ViewBag.id_inventory_cellular_line = AddFirstItem(new SelectList(db.IT_inventory_cellular_line.Where(x => x.id_planta == empleado.planta_clave), "id", "ConcatDetalles"), textoPorDefecto: "-- Seleccionar --", selected: model.id_inventory_cellular_line.ToString());

            return View(model);
        }

        // POST: IT_asignacion_cellular_line/Asignar
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Asignar(IT_asignacion_cellular_line iT_asignacion_cellular_line)
        {
            //validar que no se repida la misma conbinacion entre linea y empleado
            if (db.IT_asignacion_cellular_line.Any(x=>x.id_empleado == iT_asignacion_cellular_line.id_empleado && x.es_asignacion_actual && x.id_inventory_cellular_line == iT_asignacion_cellular_line.id_inventory_cellular_line))
                ModelState.AddModelError("", "El empleado ya se encuentra asignado a este número.");

            if (ModelState.IsValid)
            {
                db.IT_asignacion_cellular_line.Add(iT_asignacion_cellular_line);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha generado la asignación correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("ListadoAsignaciones", "IT_asignacion_hardware", new { id = iT_asignacion_cellular_line.id_empleado });
            }

            empleados empleado = db.empleados.Find(iT_asignacion_cellular_line.id_empleado);
            empleados sistemas = db.empleados.Find(iT_asignacion_cellular_line.id_sistemas);

            iT_asignacion_cellular_line.empleados = empleado;
            iT_asignacion_cellular_line.empleados1 = sistemas;

            ViewBag.id_inventory_cellular_line = AddFirstItem(new SelectList(db.IT_inventory_cellular_line.Where(x => x.id_planta == empleado.planta_clave), "id", "ConcatDetalles"), textoPorDefecto: "-- Seleccionar --", selected: iT_asignacion_cellular_line.id_inventory_cellular_line.ToString());

            return View(iT_asignacion_cellular_line);
        }


        // GET: IT_asignacion_cellular_line/DesasociarNumero/5
        public ActionResult DesasociarNumero(int? id)
        {
            if (!TieneRol(TipoRoles.IT_ASIGNACION_HARDWARE))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_asignacion_cellular_line asignacion = db.IT_asignacion_cellular_line.Find(id);
            if (asignacion == null)
            {
                return View("../Error/NotFound");
            }

            if (asignacion.fecha_desasignacion == null)
                asignacion.fecha_desasignacion = DateTime.Now;

            return View(asignacion);
        }

        // POST: IT_asignacion_hardware/DesasociarNumero/5
        [HttpPost, ActionName("DesasociarNumero")]
        [ValidateAntiForgeryToken]
        public ActionResult DesasociarNumeroConfirmed(IT_asignacion_cellular_line asignacion)
        {

            IT_asignacion_cellular_line asignacion_old = db.IT_asignacion_cellular_line.Find(asignacion.id);
            asignacion_old.fecha_desasignacion = asignacion.fecha_desasignacion;
            asignacion_old.es_asignacion_actual = false;

            db.Entry(asignacion_old).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Mensaje"] = new MensajesSweetAlert("Se ha desvinculado el número celular correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("ListadoAsignaciones", "IT_asignacion_hardware", new { id = asignacion.id_empleado });
        }
       

        // GET: IT_asignacion_cellular_line/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.IT_ASIGNACION_HARDWARE))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_asignacion_cellular_line asignacion = db.IT_asignacion_cellular_line.Find(id);
            if (asignacion == null)
            {
                return View("../Error/NotFound");
            }

            if (asignacion.fecha_desasignacion == null)
                asignacion.fecha_desasignacion = DateTime.Now;

            return View(asignacion);
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
