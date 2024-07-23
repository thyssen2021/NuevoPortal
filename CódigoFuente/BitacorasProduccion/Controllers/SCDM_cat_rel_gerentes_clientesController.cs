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
    public class SCDM_cat_rel_gerentes_clientesController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: SCDM_cat_rel_gerentes_clientes
        public ActionResult Index()
        {
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            return View(db.SCDM_cat_rel_gerentes_clientes.ToList());
        }



        // GET: SCDM_cat_rel_gerentes_clientes/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true && x.SCDM_cat_rel_usuarios_departamentos.Any(y=> y.id_departamento == (int)Bitacoras.Util.SCDM_departamentos_AsignacionENUM.VENTAS)), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)));
            ViewBag.id_cliente = AddFirstItem(new SelectList(db.clientes.Where(x => x.activo == true), nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)));

            return View();
        }

        // POST: SCDM_cat_rel_gerentes_clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SCDM_cat_rel_gerentes_clientes SCDM_cat_rel_gerentes_clientes)
        {

            //verifica que no exista otro registro con el mismo empleado y departamento
            if (db.SCDM_cat_rel_gerentes_clientes.Any(x => x.id_empleado == SCDM_cat_rel_gerentes_clientes.id_empleado && x.id_cliente == SCDM_cat_rel_gerentes_clientes.id_cliente))
                ModelState.AddModelError("", "El gerente de cuenta ya se encuentra asignado a este cliente.");

            if (ModelState.IsValid)
            {
                SCDM_cat_rel_gerentes_clientes.activo = true;
                db.SCDM_cat_rel_gerentes_clientes.Add(SCDM_cat_rel_gerentes_clientes);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha agregado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true && x.SCDM_cat_rel_usuarios_departamentos.Any(y => y.id_departamento == (int)Bitacoras.Util.SCDM_departamentos_AsignacionENUM.VENTAS)), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)));
            ViewBag.id_cliente = AddFirstItem(new SelectList(db.clientes.Where(x => x.activo == true), nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)));

            return View(SCDM_cat_rel_gerentes_clientes);
        }

        // GET: SCDM_cat_rel_gerentes_clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_rel_gerentes_clientes SCDM_cat_rel_gerentes_clientes = db.SCDM_cat_rel_gerentes_clientes.Find(id);
            if (SCDM_cat_rel_gerentes_clientes == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true && x.SCDM_cat_rel_usuarios_departamentos.Any(y => y.id_departamento == (int)Bitacoras.Util.SCDM_departamentos_AsignacionENUM.VENTAS)), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), selected: SCDM_cat_rel_gerentes_clientes.id_empleado.ToString());
            ViewBag.id_cliente = AddFirstItem(new SelectList(db.clientes.Where(x => x.activo == true), nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)), selected: SCDM_cat_rel_gerentes_clientes.id_cliente.ToString());

            return View(SCDM_cat_rel_gerentes_clientes);
        }

        // POST: SCDM_cat_rel_gerentes_clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SCDM_cat_rel_gerentes_clientes SCDM_cat_rel_gerentes_clientes)
        {

            //verifica que no exista otro registro con el mimo empleado y departamento
            if (db.SCDM_cat_rel_gerentes_clientes.Any(x => x.id_empleado == SCDM_cat_rel_gerentes_clientes.id_empleado && x.id_cliente == SCDM_cat_rel_gerentes_clientes.id_cliente && x.id != SCDM_cat_rel_gerentes_clientes.id))
                ModelState.AddModelError("", "El gerente de cuenta ya se encuentra asignado a este departamento.");

            if (ModelState.IsValid)
            {
                db.Entry(SCDM_cat_rel_gerentes_clientes).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha editado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true && x.SCDM_cat_rel_usuarios_departamentos.Any(y => y.id_departamento == (int)Bitacoras.Util.SCDM_departamentos_AsignacionENUM.VENTAS)), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), selected: SCDM_cat_rel_gerentes_clientes.id_empleado.ToString());
            ViewBag.id_cliente = AddFirstItem(new SelectList(db.clientes.Where(x => x.activo == true), nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)), selected: SCDM_cat_rel_gerentes_clientes.id_cliente.ToString());

            return View(SCDM_cat_rel_gerentes_clientes);
        }

        // GET: SCDM_cat_rel_gerentes_clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_rel_gerentes_clientes SCDM_cat_rel_gerentes_clientes = db.SCDM_cat_rel_gerentes_clientes.Find(id);
            if (SCDM_cat_rel_gerentes_clientes == null)
            {
                return HttpNotFound();
            }
            return View(SCDM_cat_rel_gerentes_clientes);
        }

        // POST: SCDM_cat_rel_gerentes_clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SCDM_cat_rel_gerentes_clientes SCDM_cat_rel_gerentes_clientes = db.SCDM_cat_rel_gerentes_clientes.Find(id);
            db.SCDM_cat_rel_gerentes_clientes.Remove(SCDM_cat_rel_gerentes_clientes);
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
