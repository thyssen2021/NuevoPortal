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
    public class GV_usuariosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: GV_usuarios
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.GV_CATALOGOS))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                return View(db.GV_usuarios.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: GV_usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.GV_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_usuarios item = db.GV_usuarios.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }
                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // GET: GV_usuarios/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.GV_CATALOGOS))
            {

                //crea un Select  list para el estatus
                List<SelectListItem> newList = new List<SelectListItem>();

                //Agrega el estatus al selectListItem
                newList.Add(new SelectListItem() { Text = Bitacoras.Util.GV_tipo_departamento.DescripcionStatus(Bitacoras.Util.GV_tipo_departamento.NOMINA), Value = Bitacoras.Util.GV_tipo_departamento.NOMINA });
                newList.Add(new SelectListItem() { Text = Bitacoras.Util.GV_tipo_departamento.DescripcionStatus(Bitacoras.Util.GV_tipo_departamento.CONTROLLING), Value = Bitacoras.Util.GV_tipo_departamento.CONTROLLING });
                newList.Add(new SelectListItem() { Text = Bitacoras.Util.GV_tipo_departamento.DescripcionStatus(Bitacoras.Util.GV_tipo_departamento.CUENTASPORPAGAR), Value = Bitacoras.Util.GV_tipo_departamento.CUENTASPORPAGAR });

                SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text");
                ViewBag.departamento = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
                ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), "id", nameof(empleados.ConcatNumEmpleadoNombre)));

                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: GV_usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GV_usuarios item)
        {

            //busca si tipo poliza con la misma departamento
            GV_usuarios item_busca = db.GV_usuarios.Where(s => s.departamento.ToUpper() == item.departamento.ToUpper() && s.id_empleado == item.id_empleado)
                                    .FirstOrDefault();

            if (item_busca != null)
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores");


            if (ModelState.IsValid)
            {
                db.GV_usuarios.Add(item);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = Bitacoras.Util.GV_tipo_departamento.DescripcionStatus(Bitacoras.Util.GV_tipo_departamento.NOMINA), Value = Bitacoras.Util.GV_tipo_departamento.NOMINA });
            newList.Add(new SelectListItem() { Text = Bitacoras.Util.GV_tipo_departamento.DescripcionStatus(Bitacoras.Util.GV_tipo_departamento.CONTROLLING), Value = Bitacoras.Util.GV_tipo_departamento.CONTROLLING });
            newList.Add(new SelectListItem() { Text = Bitacoras.Util.GV_tipo_departamento.DescripcionStatus(Bitacoras.Util.GV_tipo_departamento.CUENTASPORPAGAR), Value = Bitacoras.Util.GV_tipo_departamento.CUENTASPORPAGAR });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text");
            ViewBag.departamento = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --", selected: item.departamento);
            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), "id", nameof(empleados.ConcatNumEmpleadoNombre)), selected: item.id_empleado.ToString());

            return View(item);

        }

        // GET: GV_usuarios/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.GV_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_usuarios item = db.GV_usuarios.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }

                List<SelectListItem> newList = new List<SelectListItem>();

                //Agrega el estatus al selectListItem
                newList.Add(new SelectListItem() { Text = Bitacoras.Util.GV_tipo_departamento.DescripcionStatus(Bitacoras.Util.GV_tipo_departamento.NOMINA), Value = Bitacoras.Util.GV_tipo_departamento.NOMINA });
                newList.Add(new SelectListItem() { Text = Bitacoras.Util.GV_tipo_departamento.DescripcionStatus(Bitacoras.Util.GV_tipo_departamento.CONTROLLING), Value = Bitacoras.Util.GV_tipo_departamento.CONTROLLING });
                newList.Add(new SelectListItem() { Text = Bitacoras.Util.GV_tipo_departamento.DescripcionStatus(Bitacoras.Util.GV_tipo_departamento.CUENTASPORPAGAR), Value = Bitacoras.Util.GV_tipo_departamento.CUENTASPORPAGAR });

                SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text");
                ViewBag.departamento = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --", selected: item.departamento);
                ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), "id", nameof(empleados.ConcatNumEmpleadoNombre)), selected: item.id_empleado.ToString());

                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: GV_usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GV_usuarios item)
        {

            //busca si existe in departamento con la misma departamento
            GV_usuarios item_busca = db.GV_usuarios.Where(s => s.departamento.ToUpper() == item.departamento.ToUpper() && s.id_empleado == item.id_empleado && s.id != item.id)
                                     .FirstOrDefault();

            if (item_busca != null)
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores");

            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = Bitacoras.Util.GV_tipo_departamento.DescripcionStatus(Bitacoras.Util.GV_tipo_departamento.NOMINA), Value = Bitacoras.Util.GV_tipo_departamento.NOMINA });
            newList.Add(new SelectListItem() { Text = Bitacoras.Util.GV_tipo_departamento.DescripcionStatus(Bitacoras.Util.GV_tipo_departamento.CONTROLLING), Value = Bitacoras.Util.GV_tipo_departamento.CONTROLLING });
            newList.Add(new SelectListItem() { Text = Bitacoras.Util.GV_tipo_departamento.DescripcionStatus(Bitacoras.Util.GV_tipo_departamento.CUENTASPORPAGAR), Value = Bitacoras.Util.GV_tipo_departamento.CUENTASPORPAGAR });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text");
            ViewBag.departamento = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --", selected: item.departamento);
            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true), "id", nameof(empleados.ConcatNumEmpleadoNombre)), selected: item.id_empleado.ToString());

            return View(item);
        }

        // GET: GV_usuarios/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.GV_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                GV_usuarios item = db.GV_usuarios.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }
                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: GV_usuarios/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            GV_usuarios item = db.GV_usuarios.Find(id);
            // item.activo = false;

            try
            {
                db.GV_usuarios.Remove(item);
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Index");
            }
            TempData["Mensaje"] = new MensajesSweetAlert("Se ha eliminado el registro correctamente", TipoMensajesSweetAlerts.SUCCESS);
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
