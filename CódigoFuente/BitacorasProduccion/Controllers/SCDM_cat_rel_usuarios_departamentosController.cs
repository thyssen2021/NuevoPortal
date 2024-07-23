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
    public class SCDM_cat_rel_usuarios_departamentosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: SCDM_cat_rel_usuarios_departamentos
        public ActionResult Index()
        {
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            return View(db.SCDM_cat_rel_usuarios_departamentos.ToList());
        }



        // GET: SCDM_cat_rel_usuarios_departamentos/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)));
            ViewBag.id_departamento = AddFirstItem(new SelectList(db.SCDM_cat_departamentos_asignacion.Where(x => x.activo == true), nameof(SCDM_cat_departamentos_asignacion.id), nameof(SCDM_cat_departamentos_asignacion.descripcion)));

            return View();
        }

        // POST: SCDM_cat_rel_usuarios_departamentos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SCDM_cat_rel_usuarios_departamentos SCDM_cat_rel_usuarios_departamentos)
        {
            
            //verifica que no exista otro registro con el mismo empleado y departamento
            if (db.SCDM_cat_rel_usuarios_departamentos.Any(x => x.id_empleado == SCDM_cat_rel_usuarios_departamentos.id_empleado && x.id_departamento == SCDM_cat_rel_usuarios_departamentos.id_departamento))
                ModelState.AddModelError("", "El usuario ya se encuentra asignado a este departamento.");

            if (ModelState.IsValid)
            {
                SCDM_cat_rel_usuarios_departamentos.activo = true;
                db.SCDM_cat_rel_usuarios_departamentos.Add(SCDM_cat_rel_usuarios_departamentos);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha agregado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)));
            ViewBag.id_departamento = AddFirstItem(new SelectList(db.SCDM_cat_departamentos_asignacion.Where(x => x.activo == true), nameof(SCDM_cat_departamentos_asignacion.id), nameof(SCDM_cat_departamentos_asignacion.descripcion)));

            return View(SCDM_cat_rel_usuarios_departamentos);
        }

        // GET: SCDM_cat_rel_usuarios_departamentos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_rel_usuarios_departamentos SCDM_cat_rel_usuarios_departamentos = db.SCDM_cat_rel_usuarios_departamentos.Find(id);
            if (SCDM_cat_rel_usuarios_departamentos == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), selected: SCDM_cat_rel_usuarios_departamentos.id_empleado.ToString());
            ViewBag.id_departamento = AddFirstItem(new SelectList(db.SCDM_cat_departamentos_asignacion.Where(x => x.activo == true), nameof(SCDM_cat_departamentos_asignacion.id), nameof(SCDM_cat_departamentos_asignacion.descripcion)), selected: SCDM_cat_rel_usuarios_departamentos.id_departamento.ToString());

            return View(SCDM_cat_rel_usuarios_departamentos);
        }

        // POST: SCDM_cat_rel_usuarios_departamentos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SCDM_cat_rel_usuarios_departamentos SCDM_cat_rel_usuarios_departamentos)
        {
         
            //verifica que no exista otro registro con el mimo empleado y departamento
            if (db.SCDM_cat_rel_usuarios_departamentos.Any(x => x.id_empleado == SCDM_cat_rel_usuarios_departamentos.id_empleado && x.id_departamento == SCDM_cat_rel_usuarios_departamentos.id_departamento && x.id != SCDM_cat_rel_usuarios_departamentos.id))
                ModelState.AddModelError("", "El usuario ya se encuentra asignado a este departamento.");

            if (ModelState.IsValid)
            {
                db.Entry(SCDM_cat_rel_usuarios_departamentos).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha editado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), selected: SCDM_cat_rel_usuarios_departamentos.id_empleado.ToString());
            ViewBag.id_departamento = AddFirstItem(new SelectList(db.SCDM_cat_departamentos_asignacion.Where(x => x.activo == true), nameof(SCDM_cat_departamentos_asignacion.id), nameof(SCDM_cat_departamentos_asignacion.descripcion)), selected: SCDM_cat_rel_usuarios_departamentos.id_departamento.ToString());

            return View(SCDM_cat_rel_usuarios_departamentos);
        }

        // GET: SCDM_cat_rel_usuarios_departamentos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_rel_usuarios_departamentos SCDM_cat_rel_usuarios_departamentos = db.SCDM_cat_rel_usuarios_departamentos.Find(id);
            if (SCDM_cat_rel_usuarios_departamentos == null)
            {
                return HttpNotFound();
            }
            return View(SCDM_cat_rel_usuarios_departamentos);
        }

        // POST: SCDM_cat_rel_usuarios_departamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SCDM_cat_rel_usuarios_departamentos SCDM_cat_rel_usuarios_departamentos = db.SCDM_cat_rel_usuarios_departamentos.Find(id);
            db.SCDM_cat_rel_usuarios_departamentos.Remove(SCDM_cat_rel_usuarios_departamentos);
            db.SaveChanges();
            TempData["Mensaje"] = new MensajesSweetAlert("Se ha borrado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

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
