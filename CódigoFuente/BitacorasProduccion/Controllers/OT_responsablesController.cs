using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class OT_responsablesController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: OT_responsables
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                empleados emp = obtieneEmpleadoLogeado();

                return View(db.OT_responsables.Where(x => x.empleados.planta_clave == emp.planta_clave));
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }


        // GET: OT_responsables/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                empleados emp = obtieneEmpleadoLogeado();

                ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.planta_clave == emp.planta_clave && x.activo == true), "id", "ConcatNumEmpleadoNombre"));
                var supervisores = db.OT_responsables.Where(x => x.activo == true && x.empleados.planta_clave == emp.planta_clave && x.supervisor == true);
                List<empleados> supervisoresList = supervisores.Select(x => x.empleados).Distinct().ToList();
                ViewBag.id_supervisor = AddFirstItem(new SelectList(supervisoresList, "id", "ConcatNumEmpleadoNombre"));
                return View(new OT_responsables { });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: OT_responsables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OT_responsables oT_responsables)
        {
            bool existe = db.OT_responsables.Any(x => x.id_empleado == oT_responsables.id_empleado);
            if (existe)
                ModelState.AddModelError("", "Este empleado ya se ecuentra registrado como responsable.");

            var user = db.AspNetUsers.Where(x => x.IdEmpleado == oT_responsables.id_empleado).FirstOrDefault();


            if (user == null)
                ModelState.AddModelError("", "Este empleado no tiene usuario registrado. Contacte al Administrador del sitio para continuar.");


            if (ModelState.IsValid)
            {
                //trata de asignar el permiso
                var result = await _userManager.AddToRoleAsync(user.Id, TipoRoles.OT_RESPONSABLE);

                //busca el permiso de responsable
                db.OT_responsables.Add(oT_responsables);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            empleados emp = obtieneEmpleadoLogeado();

            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.planta_clave == emp.planta_clave && x.activo == true), "id", "ConcatNumEmpleadoNombre"));
            var supervisores = db.OT_responsables.Where(x => x.activo == true && x.empleados.planta_clave == emp.planta_clave && x.supervisor == true);
            List<empleados> supervisoresList = supervisores.Select(x => x.empleados).Distinct().ToList();
            ViewBag.id_supervisor = AddFirstItem(new SelectList(supervisoresList, "id", "ConcatNumEmpleadoNombre"));

            return View(oT_responsables);
        }

        // GET: OT_responsables/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OT_responsables oT_responsables = db.OT_responsables.Find(id);
            if (oT_responsables == null)
            {
                return HttpNotFound();
            }
            return View(oT_responsables);
        }

        // POST: OT_responsables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                //obtiene el empleado
                var responsable = db.OT_responsables.Find(id);
                var user = db.AspNetUsers.Where(x => x.IdEmpleado == responsable.id_empleado).FirstOrDefault();

                //trata de eliminar el permiso
                var result = await _userManager.RemoveFromRoleAsync(user.Id, TipoRoles.OT_RESPONSABLE);

                OT_responsables oT_responsables = db.OT_responsables.Find(id);
                db.OT_responsables.Remove(oT_responsables);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }            
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
