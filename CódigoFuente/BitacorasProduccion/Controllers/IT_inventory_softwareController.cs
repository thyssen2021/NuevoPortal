using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_inventory_softwareController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_inventory_software
        public ActionResult Index(string description, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                var listado = db.IT_inventory_software
                    .Where(x =>
                     (x.descripcion.Contains(description) || String.IsNullOrEmpty(description))
                    )
                  .OrderBy(x => x.descripcion)
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_software
                       .Where(x =>
                     (x.descripcion.Contains(description) || String.IsNullOrEmpty(description))
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["description"] = description;

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

        // GET: IT_inventory_software/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {       
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_software iT_inventory_software = db.IT_inventory_software.Find(id);
                if (iT_inventory_software == null)
                {
                    return View("../Error/NotFound");
                }
                return View(iT_inventory_software);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

          
        }

        // GET: IT_inventory_software/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                return View(new IT_inventory_software { activo = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_software/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IT_inventory_software iT_inventory_software, FormCollection collection)
        {

            List<IT_inventory_software_versions> versions = new List<IT_inventory_software_versions>();
            iT_inventory_software.IT_inventory_software_versions = versions; //vacia la lista que viene del form o la inicializa si viene vacia (solo detecta los agregados con js)

            #region obtiene_versiones_del_form

            //obtiene las versiones del form collection
            foreach (string key in collection.AllKeys.Where(x => x.StartsWith("IT_inventory_software_versions") && x.EndsWith(".version")))
            {
                int index = -1;
                bool active = false;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    //obtiene el index
                    int.TryParse(m.Value, out index);

                    //obtiene los valores del form para el index asociado
                    string version = collection["IT_inventory_software_versions[" + index + "].version"].ToUpper();
                    Boolean.TryParse(collection["IT_inventory_software_versions[" + index + "].activo"], out active);

                    //agrega el drive
                    versions.Add(
                        new IT_inventory_software_versions
                        {
                            version = version,
                            activo = active,
                        }
                    );

                }
            }
            #endregion

            if (ModelState.IsValid)
            {
                db.IT_inventory_software.Add(iT_inventory_software);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            return View(iT_inventory_software);
        }

        // GET: IT_inventory_software/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_software iT_inventory_software = db.IT_inventory_software.Find(id);
                if (iT_inventory_software == null)
                {
                    return View("../Error/NotFound");
                }
                return View(iT_inventory_software);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_software/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IT_inventory_software iT_inventory_software, FormCollection collection)
        {
            List<IT_inventory_software_versions> versions = new List<IT_inventory_software_versions>();
            iT_inventory_software.IT_inventory_software_versions = versions; //vacia la lista que viene del form o la inicializa si viene vacia (solo detecta los agregados con js)

            #region obtiene_versiones_del_form

            //obtiene las versiones del form collection
            foreach (string key in collection.AllKeys.Where(x => x.StartsWith("IT_inventory_software_versions") && x.EndsWith(".version")))
            {
                int index = -1;
                bool active = false;
                int id_version = -1;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    //obtiene el index
                    int.TryParse(m.Value, out index);

                    //obtiene los valores del form para el index asociado
                    string version = collection["IT_inventory_software_versions[" + index + "].version"].ToUpper();
                    Boolean.TryParse(collection["IT_inventory_software_versions[" + index + "].activo"], out active);
                    int.TryParse(collection["IT_inventory_software_versions[" + index + "].id"], out id_version);

                    //agrega el drive
                    versions.Add(
                        new IT_inventory_software_versions
                        {
                            id = id_version,
                            id_inventory_software = iT_inventory_software.id,
                            version = version,
                            activo = active,
                        }
                    );

                }
            }
            #endregion
            if (ModelState.IsValid)
            {
                //obtiene el listado de versiones asociadas a este software
                var versionsPrevious = db.IT_inventory_software_versions.Where(x => x.id_inventory_software == iT_inventory_software.id);

                //recorre el listado de versiones para todas las versiones recibidas en el formulario
                foreach (var v in versions)
                {
                    //si existe es un update
                    if (versionsPrevious.Any(x => x.id == v.id))
                    {
                        db.Entry(versionsPrevious.FirstOrDefault(x => x.id == v.id)).CurrentValues.SetValues(v);
                    }
                    else //no existe es un create 
                    {
                        db.IT_inventory_software_versions.Add(v);
                    }
                    /* NOTA: no se pueden eliminar sólo desactivar*/

                }
                //guarda en BD
                db.Entry(db.IT_inventory_software.Find(iT_inventory_software.id)).CurrentValues.SetValues(iT_inventory_software);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            return View(iT_inventory_software);
        }

        // GET: IT_inventory_software/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_inventory_software iT_inventory_software = db.IT_inventory_software.Find(id);
            if (iT_inventory_software == null)
            {
                return HttpNotFound();
            }
            return View(iT_inventory_software);
        }

        // POST: IT_inventory_software/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IT_inventory_software iT_inventory_software = db.IT_inventory_software.Find(id);
            db.IT_inventory_software.Remove(iT_inventory_software);
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
