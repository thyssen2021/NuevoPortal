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
    public class PM_poliza_manual_modeloController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: PM_poliza_manual_modelo
        public ActionResult Index(int pagina = 1)
        {
            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                var listado = db.PM_poliza_manual_modelo
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.poliza_manual
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;
               
                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

            
        }

        // GET: PM_poliza_manual_modelo/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PM_poliza_manual_modelo pM_poliza_manual_modelo = db.PM_poliza_manual_modelo.Find(id);
                if (pM_poliza_manual_modelo == null)
                {
                    return HttpNotFound();
                }
                return View(pM_poliza_manual_modelo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        // GET: PM_poliza_manual_modelo/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                return View(new PM_poliza_manual_modelo());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: PM_poliza_manual_modelo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PM_poliza_manual_modelo pM_poliza_manual_modelo)
        {
            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                //determina si hay conceptos
                if (pM_poliza_manual_modelo.PM_conceptos_modelo.Count == 0)
                    ModelState.AddModelError("", "No se ingresaron conceptos para la póliza modelo.");
                                
                if (ModelState.IsValid)
                {
                    pM_poliza_manual_modelo.fecha_creacion = DateTime.Now;
                    pM_poliza_manual_modelo.activo = true;
                    db.PM_poliza_manual_modelo.Add(pM_poliza_manual_modelo);
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(pM_poliza_manual_modelo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
           
        }

        // GET: PM_poliza_manual_modelo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                PM_poliza_manual_modelo pM_poliza_manual_modelo = db.PM_poliza_manual_modelo.Find(id);
                if (pM_poliza_manual_modelo == null)
                {
                    return HttpNotFound();
                }
                return View(pM_poliza_manual_modelo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: PM_poliza_manual_modelo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PM_poliza_manual_modelo pM_poliza_manual_modelo)
        {
            //determina si hay conceptos
            if (pM_poliza_manual_modelo.PM_conceptos_modelo.Count == 0)
                ModelState.AddModelError("", "No se ingresaron conceptos para la póliza modelo.");

            if (ModelState.IsValid)
            {
                //borra los conceptos anteriores
                var list = db.PM_conceptos_modelo.Where(x => x.id_poliza_modelo == pM_poliza_manual_modelo.id);
                foreach (PM_conceptos_modelo item in list)
                    db.PM_conceptos_modelo.Remove(item);
                db.SaveChanges();

                //los nuevos conceptos se agregan automáticamente

                //si existe lo modifica
                PM_poliza_manual_modelo poliza = db.PM_poliza_manual_modelo.Find(pM_poliza_manual_modelo.id);
                // Activity already exist in database and modify it
                db.Entry(poliza).CurrentValues.SetValues(pM_poliza_manual_modelo);
                //agrega los conceptos 
                poliza.PM_conceptos_modelo = pM_poliza_manual_modelo.PM_conceptos_modelo;
                
                db.Entry(poliza).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            return View(pM_poliza_manual_modelo);
        }

        // GET: PM_conceptos_modelo/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                PM_poliza_manual_modelo item = db.PM_poliza_manual_modelo.Find(id);
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

        // POST: PM_conceptos_modelo/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            PM_poliza_manual_modelo item = db.PM_poliza_manual_modelo.Find(id);
            item.activo = false;

            db.Entry(item).State = EntityState.Modified;
            try
            {
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
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.DISABLED, TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("Index");
        }

        // GET: PM_conceptos_modelo/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.PM_REGISTRO))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                PM_poliza_manual_modelo item = db.PM_poliza_manual_modelo.Find(id);
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

        // POST: PM_conceptos_modelo/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            PM_poliza_manual_modelo item = db.PM_poliza_manual_modelo.Find(id);
            item.activo = true;

            db.Entry(item).State = EntityState.Modified;
            try
            {
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
