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
    public class PM_usuarios_capturistasController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: PM_usuarios_capturistas
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var PM_usuarios_capturistas = db.PM_usuarios_capturistas;
                return View(PM_usuarios_capturistas.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // GET: PM_usuarios_capturistas/Details/5
        public ActionResult Details(int? id)
        {

            if (TieneRol(TipoRoles.ADMIN))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PM_usuarios_capturistas PM_usuarios_capturistas = db.PM_usuarios_capturistas.Find(id);
                if (PM_usuarios_capturistas == null)
                {
                    return HttpNotFound();
                }
                return View(PM_usuarios_capturistas);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: PM_usuarios_capturistas/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
               
                ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), "id", "ConcatNumEmpleadoNombre"));
                ViewBag.id_departamento = AddFirstItem(new SelectList(db.PM_departamentos.Where(x => x.activo == true), "id", "descripcion"));
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: PM_usuarios_capturistas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PM_usuarios_capturistas PM_usuarios_capturistas)
        {

            //busca si ya existe un usuario con el id de empleado 
            PM_usuarios_capturistas item = db.PM_usuarios_capturistas.Where(s => s.id_empleado == PM_usuarios_capturistas.id_empleado).FirstOrDefault();

            if (item != null)
                ModelState.AddModelError("", "Este empleado ya se encuentra asignado a un departamento.");

            if (ModelState.IsValid)
            {
                PM_usuarios_capturistas.activo = true;
                db.PM_usuarios_capturistas.Add(PM_usuarios_capturistas);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), "id", "ConcatNumEmpleadoNombre"));
            ViewBag.id_departamento = AddFirstItem(new SelectList(db.PM_departamentos.Where(x => x.activo == true), "id", "descripcion"));
            return View(PM_usuarios_capturistas);
        }

        // GET: PM_usuarios_capturistas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PM_usuarios_capturistas PM_usuarios_capturistas = db.PM_usuarios_capturistas.Find(id);
                if (PM_usuarios_capturistas == null)
                {
                    return HttpNotFound();
                }
                ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), "id", "ConcatNumEmpleadoNombre"), selected: PM_usuarios_capturistas.id_empleado.ToString());
                ViewBag.id_departamento = AddFirstItem(new SelectList(db.PM_departamentos.Where(x => x.activo == true), "id", "descripcion"), selected: PM_usuarios_capturistas.id_departamento.ToString());
                return View(PM_usuarios_capturistas);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: PM_usuarios_capturistas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( PM_usuarios_capturistas PM_usuarios_capturistas)
        {
            PM_usuarios_capturistas item = db.PM_usuarios_capturistas.Where(s => s.id_empleado == PM_usuarios_capturistas.id_empleado && s.id!=PM_usuarios_capturistas.id).FirstOrDefault();

            if (item != null)
                ModelState.AddModelError("", "Este empleado ya se encuentra asignado a un departamento.");


            if (ModelState.IsValid)
            {
                db.Entry(PM_usuarios_capturistas).State = EntityState.Modified;
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), "id", "ConcatNumEmpleadoNombre"), selected: PM_usuarios_capturistas.id_empleado.ToString());
            ViewBag.id_departamento = AddFirstItem(new SelectList(db.PM_departamentos.Where(x => x.activo == true), "id", "descripcion"), selected: PM_usuarios_capturistas.id_departamento.ToString());
            return View(PM_usuarios_capturistas);
        }

        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PM_usuarios_capturistas PM_usuarios_capturistas = db.PM_usuarios_capturistas.Find(id);
                if (PM_usuarios_capturistas == null)
                {
                    return HttpNotFound();
                }
                return View(PM_usuarios_capturistas);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: PM_validadores/Delete/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            PM_usuarios_capturistas item = db.PM_usuarios_capturistas.Find(id);
            item.activo = false;

            db.Entry(item).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);

                return RedirectToAction("Index");
            }


            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.DISABLED, TipoMensajesSweetAlerts.SUCCESS);

            return RedirectToAction("Index");
        }
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PM_usuarios_capturistas PM_usuarios_capturistas = db.PM_usuarios_capturistas.Find(id);
                if (PM_usuarios_capturistas == null)
                {
                    return HttpNotFound();
                }
                return View(PM_usuarios_capturistas);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: PM_validadores/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            PM_usuarios_capturistas item = db.PM_usuarios_capturistas.Find(id);
            item.activo = true;

            db.Entry(item).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);

                return RedirectToAction("Index");
            }


            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.ENABLED, TipoMensajesSweetAlerts.SUCCESS);

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
