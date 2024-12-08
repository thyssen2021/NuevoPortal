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
    public class upgrade_revisionController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: upgrade_revision
        public ActionResult Index(int? id_usuario, int pagina = 1)
        {

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var listado = db.upgrade_revision
                   .Where(x => x.id_upgrade_usuario == id_usuario)
                   .OrderBy(x => x.upgrade_departamentos.upgrade_plantas.id)
                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.upgrade_revision
                .Where(x => x.id_upgrade_usuario == id_usuario)
                .Count();

            //para paginación

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id_usuario"] = id_usuario;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.id_usuario = AddFirstItem(new SelectList(db.upgrade_usuarios, "id", "empleados.ConcatNumEmpleadoNombre"));
            ViewBag.Paginacion = paginacion;

            var upgrade_revision = listado;
            return View(upgrade_revision.ToList());
        }

        public ActionResult Info(int? id_usuario, int pagina = 1)
        {

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var listado = db.upgrade_revision
                   .Where(x => x.id_upgrade_usuario == id_usuario || id_usuario == null)
                   .OrderBy(x => x.upgrade_departamentos.upgrade_plantas.id)
                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.upgrade_revision
                  .Where(x => x.id_upgrade_usuario == id_usuario || id_usuario == null)
                .Count();

            //para paginación

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id_usuario"] = id_usuario;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.id_usuario = AddFirstItem(new SelectList(db.upgrade_usuarios, "id", "empleados.ConcatNumEmpleadoNombre"));
            ViewBag.Paginacion = paginacion;

            var upgrade_revision = listado;
            return View(upgrade_revision.ToList());
        }

        // GET: upgrade_revision/Details/5
        public ActionResult Details(int? id)
        {
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            upgrade_revision upgrade_revision = db.upgrade_revision.Find(id);
            if (upgrade_revision == null)
            {
                return HttpNotFound();
            }
            var listCheckItems = db.upgrade_check_item.Where(x => x.activo == true);

            foreach (var item in listCheckItems)
            {
                var item_busqueda = db.upgrade_values_checklist.Where(x => x.id_checklist_item == item.id && x.id_revision == upgrade_revision.id).FirstOrDefault();

                //verifica si existe el valor
                if (item_busqueda != null)
                    upgrade_revision.upgrade_values_checklist.Add(item_busqueda);
                else //si no existe lo agrega
                    upgrade_revision.upgrade_values_checklist.Add(new upgrade_values_checklist
                    {
                        id_revision = upgrade_revision.id,
                        id_checklist_item = item.id,
                        estatus = "PENDIENTE",
                        upgrade_check_item = item,
                        activo = true
                    });
            }

            return View(upgrade_revision);
        }

        // GET: upgrade_revision/Edit/5
        public ActionResult Edit(int? id)
        {
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            upgrade_revision upgrade_revision = db.upgrade_revision.Find(id);
            if (upgrade_revision == null)
            {
                return HttpNotFound();
            }
            var listCheckItems = db.upgrade_check_item.Where(x => x.activo == true);

            foreach (var item in listCheckItems)
            {
                var item_busqueda = db.upgrade_values_checklist.Where(x => x.id_checklist_item == item.id && x.id_revision == upgrade_revision.id).FirstOrDefault();

                //verifica si existe el valor
                if (item_busqueda != null)
                    upgrade_revision.upgrade_values_checklist.Add(item_busqueda);
                else //si no existe lo agrega
                    upgrade_revision.upgrade_values_checklist.Add(new upgrade_values_checklist
                    {
                        id_revision = upgrade_revision.id,
                        id_checklist_item = item.id,
                        estatus = "PENDIENTE",
                        upgrade_check_item = item,
                        activo = true
                    });
            }

            return View(upgrade_revision);
        }

        // POST: upgrade_revision/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(upgrade_revision upgrade_revision, FormCollection collection)
        {

            //agrega los values checklist nuevos
            foreach (upgrade_values_checklist item in upgrade_revision.upgrade_values_checklist)
            {
                try
                {
                    //Values check list
                    if (item.id > 0)
                    {
                        //si existe lo modifica
                        upgrade_values_checklist item_exists = db.upgrade_values_checklist.Find(item.id);
                        // Activity already exist in database and modify it
                        db.Entry(item_exists).CurrentValues.SetValues(item);
                        db.Entry(item_exists).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {//si no existe lo crea 
                        db.upgrade_values_checklist.Add(item);
                        db.SaveChanges();
                    }

                    item.upgrade_check_item = db.upgrade_check_item.Find(item.id);
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

                    ModelState.AddModelError("", ex.Message);

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }

            }

            //agrega los values de transacciones
            List<upgrade_values_transaccion> listaTranssacciones = new List<upgrade_values_transaccion>();

            foreach (string key in collection.AllKeys.Where(x => x.Contains("upgrade_values_transaccion") && x.Contains("id_revision")))
            {

                int index = -1;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    int.TryParse(m.Value, out index);

                    listaTranssacciones.Add(
                        new upgrade_values_transaccion
                        {
                            id_revision = upgrade_revision.id,
                            transaccion = collection["upgrade_values_transaccion[" + index + "].transaccion"],
                            estatus = collection["upgrade_values_transaccion[" + index + "].estatus"],
                            nota = collection["upgrade_values_transaccion[" + index + "].nota"],
                            activo = true
                        }
                    );

                }
            }



            try
            {
                //borra los lotes anteriores
                var listTransaccionesBD = db.upgrade_values_transaccion.Where(x => x.id_revision == upgrade_revision.id);
                foreach (upgrade_values_transaccion it in listTransaccionesBD)
                    db.upgrade_values_transaccion.Remove(it);

                db.SaveChanges();

                //agrega los lotes nuevos
                foreach (upgrade_values_transaccion it in listaTranssacciones)
                {
                    db.upgrade_values_transaccion.Add(it);                    
                }
                db.SaveChanges();

                //item.upgrade_check_item = db.upgrade_check_item.Find(item.id);
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

                ModelState.AddModelError("", ex.Message);

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }



            if (ModelState.IsValid)
            {
                //db.Entry(upgrade_revision).State = EntityState.Modified;
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                //db.SaveChanges();
                return RedirectToAction("Edit", new { id = upgrade_revision.id });
            }


            return View(upgrade_revision);
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
