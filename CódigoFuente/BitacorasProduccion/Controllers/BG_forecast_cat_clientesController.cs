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
    public class BG_forecast_cat_clientesController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_forecast_cat_clientes
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            return View(db.BG_forecast_cat_clientes.ToList());
        }

      

        // GET: BG_forecast_cat_clientes/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            return View();
        }

        // POST: BG_forecast_cat_clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion,activo")] BG_forecast_cat_clientes bG_forecast_cat_clientes)
        {
            if (ModelState.IsValid)
            {
                // Validar si ya existe un cliente con el mismo nombre
                bool clienteExistente = db.BG_forecast_cat_clientes
                                          .Any(c => c.descripcion == bG_forecast_cat_clientes.descripcion);

                if (clienteExistente)
                {
                    // Agregar un mensaje de error al modelo
                    ModelState.AddModelError("descripcion", "Ya existe un cliente con este nombre.");
                    return View(bG_forecast_cat_clientes);
                }

                bG_forecast_cat_clientes.activo = true;
                db.BG_forecast_cat_clientes.Add(bG_forecast_cat_clientes);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            return View(bG_forecast_cat_clientes);
        }

        // GET: BG_forecast_cat_clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_forecast_cat_clientes bG_forecast_cat_clientes = db.BG_forecast_cat_clientes.Find(id);
            if (bG_forecast_cat_clientes == null)
            {
                return HttpNotFound();
            }
            return View(bG_forecast_cat_clientes);
        }

        // POST: BG_forecast_cat_clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,activo")] BG_forecast_cat_clientes bG_forecast_cat_clientes)
        {
            if (ModelState.IsValid)
            {
                // Validar si otro cliente ya tiene el mismo nombre (excluyendo el actual)
                bool clienteExistente = db.BG_forecast_cat_clientes
                                          .Any(c => c.descripcion == bG_forecast_cat_clientes.descripcion && c.id != bG_forecast_cat_clientes.id);

                if (clienteExistente)
                {
                    // Agregar un mensaje de error al modelo
                    ModelState.AddModelError("descripcion", "Ya existe otro cliente con este nombre.");
                    return View(bG_forecast_cat_clientes);
                }

                db.Entry(bG_forecast_cat_clientes).State = EntityState.Modified;
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bG_forecast_cat_clientes);
        }

        // GET: BG_forecast_cat_clientes/Delete/5
        public ActionResult Enable(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_forecast_cat_clientes bG_forecast_cat_clientes = db.BG_forecast_cat_clientes.Find(id);
            if (bG_forecast_cat_clientes == null)
            {
                return HttpNotFound();
            }
            return View(bG_forecast_cat_clientes);
        }

        // POST: BG_forecast_cat_clientes/Delete/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            BG_forecast_cat_clientes bG_forecast_cat_clientes = db.BG_forecast_cat_clientes.Find(id);
            bG_forecast_cat_clientes.activo = true;
            db.SaveChanges();
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.ENABLED, TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("Index");
        }
        public ActionResult Disable(int? id)
        {
            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_forecast_cat_clientes bG_forecast_cat_clientes = db.BG_forecast_cat_clientes.Find(id);
            if (bG_forecast_cat_clientes == null)
            {
                return HttpNotFound();
            }
            return View(bG_forecast_cat_clientes);
        }

        // POST: BG_forecast_cat_clientes/Delete/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            BG_forecast_cat_clientes bG_forecast_cat_clientes = db.BG_forecast_cat_clientes.Find(id);
            bG_forecast_cat_clientes.activo = false;
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.DISABLED, TipoMensajesSweetAlerts.SUCCESS);
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
