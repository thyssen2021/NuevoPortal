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
    public class PM_autorizadoresController : BaseController
    {

        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: PM_autorizadores
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var PM_autorizadores = db.PM_autorizadores.Include(p => p.empleados);
                return View(PM_autorizadores.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }


        // GET: PM_autorizadores/Create
        public ActionResult Create()
        {

            if (TieneRol(TipoRoles.ADMIN))
            {
                ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), "id", "ConcatNumEmpleadoNombre"));
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: PM_autorizadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_empleado,activo")] PM_autorizadores PM_autorizadores)
        {
            //busca si ya existe un validador con ese id empleado
            PM_autorizadores autorizador = db.PM_autorizadores.Where(s => s.id_empleado == PM_autorizadores.id_empleado).FirstOrDefault();


            if (autorizador != null)
                ModelState.AddModelError("", "Este empleado ya se encuentra como parte de Controlling.");

            if (ModelState.IsValid)
            {
                PM_autorizadores.activo = true;
                db.PM_autorizadores.Add(PM_autorizadores);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), "id", "ConcatNumEmpleadoNombre"));
            return View(PM_autorizadores);
        }

        //// GET: PM_autorizadores/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    PM_autorizadores PM_autorizadores = db.PM_autorizadores.Find(id);
        //    if (PM_autorizadores == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.id_empleado = new SelectList(db.empleados, "id", "numeroEmpleado", PM_autorizadores.id_empleado);
        //    return View(PM_autorizadores);
        //}

        //// POST: PM_autorizadores/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "id,id_empleado,activo")] PM_autorizadores PM_autorizadores)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(PM_autorizadores).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.id_empleado = new SelectList(db.empleados, "id", "numeroEmpleado", PM_autorizadores.id_empleado);
        //    return View(PM_autorizadores);
        //}

        // GET: PM_autorizadores/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                PM_autorizadores PM_autorizadores = db.PM_autorizadores.Find(id);
                if (PM_autorizadores == null)
                {
                    return View("../Error/NotFound");
                }

                return View(PM_autorizadores);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: PM_autorizadores/Delete/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            PM_autorizadores item = db.PM_autorizadores.Find(id);
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
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                PM_autorizadores PM_autorizadores = db.PM_autorizadores.Find(id);
                if (PM_autorizadores == null)
                {
                    return View("../Error/NotFound");
                }

                return View(PM_autorizadores);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: PM_autorizadores/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            PM_autorizadores item = db.PM_autorizadores.Find(id);
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
