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
    public class SCDM_cat_usuarios_revision_departamentoController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: SCDM_cat_usuarios_revision_departamento
        public ActionResult Index()
        {
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            return View(db.SCDM_cat_usuarios_revision_departamento.ToList());
        }

        // GET: SCDM_cat_usuarios_revision_departamento/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            //creamos una lista tipo SelectListItem
            List<SelectListItem> listTipo = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Primario", Value = "PRIMARIO" },
                new SelectListItem() { Text = "Secundario", Value = "SECUNDARIO" }
            };
            
            ViewBag.tipo = AddFirstItem(new SelectList(listTipo, "Value", "Text"));
            ViewBag.id_rel_usuarios_departamentos = AddFirstItem(new SelectList(db.SCDM_cat_rel_usuarios_departamentos.Where(x => x.activo == true), nameof(SCDM_cat_rel_usuarios_departamentos.id), nameof(SCDM_cat_rel_usuarios_departamentos.ConcatDepartamentoEmpleado)));
            ViewBag.id_planta_solicitud = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo == true), nameof(plantas.clave), nameof(plantas.descripcion)));

            return View();
        }

        // POST: SCDM_cat_usuarios_revision_departamento/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SCDM_cat_usuarios_revision_departamento SCDM_cat_usuarios_revision_departamento)
        {

            //verifica que no exista otro registro con el mismo empleado y departamento
            if (db.SCDM_cat_usuarios_revision_departamento.Any(x => x.id_rel_usuarios_departamentos == SCDM_cat_usuarios_revision_departamento.id_rel_usuarios_departamentos && x.id_planta_solicitud == SCDM_cat_usuarios_revision_departamento.id_planta_solicitud))
                ModelState.AddModelError("", "El usuario ya se envuentra asignado a esta planta.");

            if (ModelState.IsValid)
            {
                SCDM_cat_usuarios_revision_departamento.activo = true;
                SCDM_cat_usuarios_revision_departamento.envia_correo = true;
                db.SCDM_cat_usuarios_revision_departamento.Add(SCDM_cat_usuarios_revision_departamento);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha agregado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            //creamos una lista tipo SelectListItem
            List<SelectListItem> listTipo = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Primario", Value = "PRIMARIO" },
                new SelectListItem() { Text = "Secundario", Value = "SECUNDARIO" }
            };
            SelectList selectListTipo = new SelectList(listTipo, "Value", "Text");

            ViewBag.tipo = AddFirstItem(selectListTipo, selected: SCDM_cat_usuarios_revision_departamento.tipo);
            ViewBag.id_rel_usuarios_departamentos = AddFirstItem(new SelectList(db.SCDM_cat_rel_usuarios_departamentos.Where(x => x.activo == true), nameof(SCDM_cat_rel_usuarios_departamentos.id), nameof(SCDM_cat_rel_usuarios_departamentos.ConcatDepartamentoEmpleado)));
            ViewBag.id_planta_solicitud = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo == true), nameof(plantas.clave), nameof(plantas.descripcion)));


            return View(SCDM_cat_usuarios_revision_departamento);
        }

        // GET: SCDM_cat_usuarios_revision_departamento/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_usuarios_revision_departamento SCDM_cat_usuarios_revision_departamento = db.SCDM_cat_usuarios_revision_departamento.Find(id);
            if (SCDM_cat_usuarios_revision_departamento == null)
            {
                return HttpNotFound();
            }
            //creamos una lista tipo SelectListItem
            List<SelectListItem> listTipo = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Primario", Value = "PRIMARIO" },
                new SelectListItem() { Text = "Secundario", Value = "SECUNDARIO" }
            };
            SelectList selectListTipo = new SelectList(listTipo, "Value", "Text");

            ViewBag.tipo = AddFirstItem(selectListTipo, selected: SCDM_cat_usuarios_revision_departamento.tipo);
            ViewBag.id_rel_usuarios_departamentos = AddFirstItem(new SelectList(db.SCDM_cat_rel_usuarios_departamentos.Where(x => x.activo == true), nameof(SCDM_cat_rel_usuarios_departamentos.id), nameof(SCDM_cat_rel_usuarios_departamentos.ConcatDepartamentoEmpleado)), selected: SCDM_cat_usuarios_revision_departamento.id_rel_usuarios_departamentos.ToString());
            ViewBag.id_planta_solicitud = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo == true), nameof(plantas.clave), nameof(plantas.descripcion)), selected: SCDM_cat_usuarios_revision_departamento.id_planta_solicitud.ToString());

            return View(SCDM_cat_usuarios_revision_departamento);
        }

        // POST: SCDM_cat_usuarios_revision_departamento/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SCDM_cat_usuarios_revision_departamento SCDM_cat_usuarios_revision_departamento)
        {

            //verifica que no exista otro registro con el mimo empleado y departamento
            if (db.SCDM_cat_usuarios_revision_departamento.Any(x => x.id_rel_usuarios_departamentos == SCDM_cat_usuarios_revision_departamento.id_rel_usuarios_departamentos && x.id_planta_solicitud == SCDM_cat_usuarios_revision_departamento.id_planta_solicitud && x.id != SCDM_cat_usuarios_revision_departamento.id))
                ModelState.AddModelError("", "El usuario ya se envuentra asignado a esta planta.");

            if (ModelState.IsValid)
            {
                db.Entry(SCDM_cat_usuarios_revision_departamento).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha editado correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            //creamos una lista tipo SelectListItem
            List<SelectListItem> listTipo = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Primario", Value = "PRIMARIO" },
                new SelectListItem() { Text = "Secundario", Value = "SECUNDARIO" }
            };
            SelectList selectListTipo = new SelectList(listTipo, "Value", "Text");

            ViewBag.tipo = AddFirstItem(selectListTipo, selected: SCDM_cat_usuarios_revision_departamento.tipo);
            ViewBag.id_rel_usuarios_departamentos = AddFirstItem(new SelectList(db.SCDM_cat_rel_usuarios_departamentos.Where(x => x.activo == true), nameof(SCDM_cat_rel_usuarios_departamentos.id), nameof(SCDM_cat_rel_usuarios_departamentos.ConcatDepartamentoEmpleado)), selected: SCDM_cat_usuarios_revision_departamento.id_rel_usuarios_departamentos.ToString());
            ViewBag.id_planta_solicitud = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo == true), nameof(plantas.clave), nameof(plantas.descripcion)), selected: SCDM_cat_usuarios_revision_departamento.id_planta_solicitud.ToString());

            return View(SCDM_cat_usuarios_revision_departamento);
        }

        // GET: SCDM_cat_usuarios_revision_departamento/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_cat_usuarios_revision_departamento SCDM_cat_usuarios_revision_departamento = db.SCDM_cat_usuarios_revision_departamento.Find(id);
            if (SCDM_cat_usuarios_revision_departamento == null)
            {
                return HttpNotFound();
            }
            return View(SCDM_cat_usuarios_revision_departamento);
        }

        // POST: SCDM_cat_usuarios_revision_departamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SCDM_cat_usuarios_revision_departamento SCDM_cat_usuarios_revision_departamento = db.SCDM_cat_usuarios_revision_departamento.Find(id);
            db.SCDM_cat_usuarios_revision_departamento.Remove(SCDM_cat_usuarios_revision_departamento);
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
