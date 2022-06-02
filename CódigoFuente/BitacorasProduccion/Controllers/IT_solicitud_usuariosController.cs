using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_solicitud_usuariosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_solicitud_usuarios
        public ActionResult Index(string estado_solicitud, int pagina = 1)
        {

            if (TieneRol(TipoRoles.IT_SOLICITUD_USUARIOS))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                var listado = db.IT_solicitud_usuarios
                       .Where(x => (x.estatus.Contains(estado_solicitud) || String.IsNullOrEmpty(estado_solicitud)))
                       .OrderByDescending(x => x.fecha_solicitud)
                       .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                      .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_solicitud_usuarios
                      .Where(x => (x.estatus.Contains(estado_solicitud) || String.IsNullOrEmpty(estado_solicitud)))
                    .Count();

                //para paginación

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["estado_solicitud"] = estado_solicitud;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                List<string> estatusList = db.IT_solicitud_usuarios.Select(x => x.estatus).Distinct().ToList();
                //crea un Select  list para el estatus
                List<SelectListItem> newList = new List<SelectListItem>();

                foreach (string statusItem in estatusList)
                {
                    newList.Add(new SelectListItem()
                    {
                        Text = IT_solicitud_usuario_Status.DescripcionStatus(statusItem),
                        Value = statusItem
                    });
                }

                SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estado_solicitud);
                ViewBag.estado_solicitud = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Todos --");

                ViewBag.Paginacion = paginacion;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

           
        }

        // GET: IT_solicitud_usuarios/Details/5
        public ActionResult Details(int? id)
        {

            if (TieneRol(TipoRoles.IT_SOLICITUD_USUARIOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_solicitud_usuarios iT_solicitud_usuarios = db.IT_solicitud_usuarios.Find(id);
                if (iT_solicitud_usuarios == null)
                {
                    return View("../Error/NotFound");
                }
                return View(iT_solicitud_usuarios);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
           
        }

        // GET: IT_solicitud_usuarios/Close/5
        public ActionResult Close(int? id)
        {

            if (TieneRol(TipoRoles.IT_SOLICITUD_USUARIOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_solicitud_usuarios iT_solicitud_usuarios = db.IT_solicitud_usuarios.Find(id);
                if (iT_solicitud_usuarios == null)
                {
                    return View("../Error/NotFound");
                }
                return View(iT_solicitud_usuarios);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: Plantas/Close/5
        [HttpPost, ActionName("Close")]
        [ValidateAntiForgeryToken]
        public ActionResult CloseConfirmed(int id)
        {
            IT_solicitud_usuarios item = db.IT_solicitud_usuarios.Find(id);

            if (!item.id_empleado.HasValue) {

                item.no_encuentra_empleado = true;
            }

            item.estatus = IT_solicitud_usuario_Status.CERRADO;
            item.fecha_cierre = DateTime.Now;

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
