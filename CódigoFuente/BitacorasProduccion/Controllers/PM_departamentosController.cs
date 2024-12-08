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
    public class PM_departamentosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: PM_departamentos
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var pM_departamentos = db.PM_departamentos;
                return View(pM_departamentos.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

          
        }

        // GET: PM_departamentos/Details/5
        public ActionResult Details(int? id)
        {

            if (TieneRol(TipoRoles.ADMIN))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PM_departamentos pM_departamentos = db.PM_departamentos.Find(id);
                if (pM_departamentos == null)
                {
                    return HttpNotFound();
                }
                return View(pM_departamentos);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // GET: PM_departamentos/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                ViewBag.id_empleado_jefe = AddFirstItem(new SelectList(db.empleados.Where(x=>x.activo==true), "id", "ConcatNumEmpleadoNombre"));
                ViewBag.id_departamento_validacion = AddFirstItem(new SelectList(db.PM_departamentos.Where(x => x.activo == true), "id", "descripcion"));
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
           
        }

        // POST: PM_departamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PM_departamentos pM_departamentos)
        {
            
            //busca si ya existe un departamento con esa descreipcion
            PM_departamentos item = db.PM_departamentos.Where(s => s.descripcion.ToUpper() == pM_departamentos.descripcion.ToUpper()).FirstOrDefault();

            if (item != null)
                ModelState.AddModelError("", "Ya se encuentra un registro con la misma descripción.");

            if (ModelState.IsValid)
            {
                pM_departamentos.activo = true;
                db.PM_departamentos.Add(pM_departamentos);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            ViewBag.id_empleado_jefe = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), "id", "ConcatNumEmpleadoNombre"));
            ViewBag.id_departamento_validacion = AddFirstItem(new SelectList(db.PM_departamentos.Where(x => x.activo == true), "id", "descripcion"));
            return View(pM_departamentos);
        }

        // GET: PM_departamentos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PM_departamentos pM_departamentos = db.PM_departamentos.Find(id);
                if (pM_departamentos == null)
                {
                    return HttpNotFound();
                }
                ViewBag.id_empleado_jefe = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), "id", "ConcatNumEmpleadoNombre"),selected: pM_departamentos.id_empleado_jefe.ToString());
                ViewBag.id_departamento_validacion = AddFirstItem(new SelectList(db.PM_departamentos.Where(x => x.activo == true), "id", "descripcion"),selected: pM_departamentos.id_departamento_validacion.ToString());
                return View(pM_departamentos);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

           
        }

        // POST: PM_departamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_empleado_jefe,descripcion,id_departamento_validacion,activo")] PM_departamentos pM_departamentos)
        {
            //busca si ya existe un departamento con esa descreipcion
            PM_departamentos item = db.PM_departamentos.Where(s => s.descripcion.ToUpper() == pM_departamentos.descripcion.ToUpper() && s.id!=pM_departamentos.id).FirstOrDefault();

            if (item != null)
                ModelState.AddModelError("", "Ya se existe un registro con la misma descripción.");

            if (ModelState.IsValid)
            {
                db.Entry(pM_departamentos).State = EntityState.Modified;
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_empleado_jefe = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), "id", "ConcatNumEmpleadoNombre"), selected: pM_departamentos.id_empleado_jefe.ToString());
            ViewBag.id_departamento_validacion = AddFirstItem(new SelectList(db.PM_departamentos.Where(x => x.activo == true), "id", "descripcion"), selected: pM_departamentos.id_departamento_validacion.ToString());
            return View(pM_departamentos);
        }

        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PM_departamentos pM_departamentos = db.PM_departamentos.Find(id);
                if (pM_departamentos == null)
                {
                    return HttpNotFound();
                }
                return View(pM_departamentos);
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
            PM_departamentos item = db.PM_departamentos.Find(id);
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
                PM_departamentos pM_departamentos = db.PM_departamentos.Find(id);
                if (pM_departamentos == null)
                {
                    return HttpNotFound();
                }
                return View(pM_departamentos);

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
            PM_departamentos item = db.PM_departamentos.Find(id);
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
