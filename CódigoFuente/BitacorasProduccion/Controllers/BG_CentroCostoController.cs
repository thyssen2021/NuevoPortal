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
    public class BG_CentroCostoController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_CentroCosto
        public ActionResult Index()
        {

            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                var budget_centro_costo = db.budget_centro_costo;
                return View(budget_centro_costo.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // GET: BG_CentroCosto/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                budget_centro_costo budget_centro_costo = db.budget_centro_costo.Find(id);
                if (budget_centro_costo == null)
                {
                    return View("../Error/NotFound");
                }
                return View(budget_centro_costo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: BG_CentroCosto/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                ViewBag.id_planta = AddFirstItem(new SelectList(db.budget_plantas.Where(p => p.activo == true), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                ViewBag.id_budget_departamento = AddFirstItem(new SelectList(db.budget_departamentos.Where(p => p.activo == true), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                return View(new budget_centro_costo());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: BG_CentroCosto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(budget_centro_costo item, FormCollection collection)
        {

            //obtiene el listado de piezas de descarte desde el formcollection
            List<budget_responsables> listaResponsables = new List<budget_responsables>();

            foreach (string key in collection.AllKeys.Where(x => x.ToUpper().Contains(".ID_RESPONSABLE") == true).ToList())
            {
                int index = -1;

                int id_responsable = 0;
                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    int.TryParse(m.Value, out index);
                    int.TryParse(collection["budget_responsables[" + index + "].id_responsable"], out id_responsable);

                    listaResponsables.Add(
                        new budget_responsables
                        {
                            id_budget_centro_costo = item.id,
                            id_responsable = id_responsable,
                            estatus = true
                        }
                    );
                }
            }

            if (listaResponsables.Count == 0)
                ModelState.AddModelError("", "No se han seleccionado responsables para el centro de costo.");

            //verifica si hay valores disponibles
            var anyDuplicate = listaResponsables.GroupBy(x => x.id_responsable).Any(g => g.Count() > 1);
            if (anyDuplicate)
                ModelState.AddModelError("", "Verifique que cada responsables este definido una sóla vez.");

            if (ModelState.IsValid)
            {
                //busca si tipo poliza con la misma descripcion
                budget_centro_costo item_busca = db.budget_centro_costo.Where(s => s.budget_departamentos.id == item.id_budget_departamento)
                                        .FirstOrDefault();

                //busca si tipo poliza con la misma descripcion
                budget_centro_costo item_busca_num = db.budget_centro_costo.Where(s => s.num_centro_costo == item.num_centro_costo)
                                        .FirstOrDefault();

                if (item_busca != null)
                    ModelState.AddModelError("", "Ya existe un centro de costo asociado a este departamento.");

                if (item_busca_num != null)
                    ModelState.AddModelError("", "Ya existe el número de centro de costo.");

                if (item_busca == null && item_busca_num == null)
                { //Si no existe

                    item.budget_responsables = listaResponsables;

                    db.budget_centro_costo.Add(item);
                    db.SaveChanges();

                    //asiga el id del centro de costo creado a la lista de responsables
                    //listaResponsables.ForEach(i => { i.id_budget_centro_costo = item.id;  });

                    ////agrega las pza descarte nuevas
                    //foreach (budget_responsables br_item in listaResponsables)
                    //{
                    //    db.budget_responsables.Add(br_item);
                    //    db.SaveChanges();
                    //}

                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
            }

            //obtiene los ids de rel_fy_centro
            var idRels = listaResponsables.Select(x => x.id_responsable).Distinct();
            List<budget_responsables> lisResp = new List<budget_responsables>();

            item.budget_departamentos = db.budget_departamentos.Find(item.id_budget_departamento);

            var emps = db.empleados
                           .Where(s => s.activo == true && idRels.Contains(s.id)) //obtiene unicamente los sectores activos
                           .ToList();

            //CREA UN RESPONSABLE POR CADA EMPLEADO ENVIADO
            foreach (empleados e in emps)
                lisResp.Add(
                     new budget_responsables()
                     {
                         id_budget_centro_costo = item.id,
                         id_responsable = e.id
                     }
                    );

            item.budget_responsables = lisResp;

            ViewBag.id_planta = AddFirstItem(new SelectList(db.budget_plantas.Where(p => p.activo == true), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            ViewBag.id_budget_departamento = AddFirstItem(new SelectList(db.budget_departamentos.Where(p => p.activo == true), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            ViewBag.ListadoEmpleados = db.empleados.Where(x => x.activo == true).ToList();

            return View(item);
        }

        // GET: BG_CentroCosto/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                budget_centro_costo item = db.budget_centro_costo.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }
                ViewBag.id_planta = AddFirstItem(new SelectList(db.budget_plantas.Where(p => p.activo == true), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                ViewBag.id_budget_departamento = AddFirstItem(new SelectList(db.budget_departamentos.Where(p => p.activo == true), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");
                ViewBag.ListadoEmpleados = db.empleados.Where(x => x.activo == true).ToList();
                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: BG_CentroCosto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(budget_centro_costo item, FormCollection collection)
        {
            //obtiene el listado de piezas de descarte desde el formcollection
            List<budget_responsables> listaResponsables = new List<budget_responsables>();

            foreach (string key in collection.AllKeys.Where(x => x.ToUpper().Contains(".ID_RESPONSABLE") == true).ToList())
            {
                int index = -1;

                int id_responsable = 0;
                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    int.TryParse(m.Value, out index);
                    int.TryParse(collection["budget_responsables[" + index + "].id_responsable"], out id_responsable);

                    listaResponsables.Add(
                        new budget_responsables
                        {
                            id_budget_centro_costo = item.id,
                            id_responsable = id_responsable,
                            estatus = true
                        }
                    );
                }
            }

            if (listaResponsables.Count == 0)
                ModelState.AddModelError("", "No se han seleccionado responsables para el centro de costo.");

            //verifica si hay valores disponibles
            var anyDuplicate = listaResponsables.GroupBy(x => x.id_responsable).Any(g => g.Count() > 1);
            if (anyDuplicate)
                ModelState.AddModelError("", "Verifique que cada responsables este definido una sóla vez.");

            if (ModelState.IsValid)
            {

                //busca si tipo poliza con la mismo departamento
                budget_centro_costo item_busca = db.budget_centro_costo.Where(s => s.budget_departamentos.id == item.id_budget_departamento && s.id != item.id)
                                        .FirstOrDefault();

                //busca si tipo poliza con la mismom centro costo
                budget_centro_costo item_busca_num = db.budget_centro_costo.Where(s => s.num_centro_costo == item.num_centro_costo && s.id != item.id)
                                        .FirstOrDefault();

                if (item_busca != null)
                    ModelState.AddModelError("", "Ya existe un centro de costo asociado a este departamento.");

                if (item_busca_num != null)
                    ModelState.AddModelError("", "Ya existe el número de centro de costo.");

                if (item_busca == null && item_busca_num == null)
                {

                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();

                    //borra los ids responsables anteriores
                    var listResponsablesBD = db.budget_responsables.Where(x => x.id_budget_centro_costo == item.id);
                    foreach (budget_responsables br in listResponsablesBD)
                        db.budget_responsables.Remove(br);

                    db.SaveChanges();

                    //agrega las pza descarte nuevas
                    foreach (budget_responsables br_item in listaResponsables)
                    {
                        db.budget_responsables.Add(br_item);
                        db.SaveChanges();
                    }

                    //en caso de exito
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                    return RedirectToAction("Index");
                }
            }

            //obtiene los ids de rel_fy_centro
            var idRels = listaResponsables.Select(x => x.id_responsable).Distinct();
            List<budget_responsables> lisResp = new List<budget_responsables>();

            item.budget_departamentos = db.budget_departamentos.Find(item.id_budget_departamento);

            var emps = db.empleados
                           .Where(s => s.activo == true && idRels.Contains(s.id)) //obtiene unicamente los sectores activos
                           .ToList();

            //CREA UN RESPONSABLE POR CADA EMPLEADO ENVIADO
            foreach (empleados e in emps)
                lisResp.Add(
                     new budget_responsables()
                     {
                         id_budget_centro_costo = item.id,
                         id_responsable = e.id
                     }
                    );

            item.budget_responsables = lisResp;

            ViewBag.id_planta = AddFirstItem(new SelectList(db.budget_plantas.Where(p => p.activo == true), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            ViewBag.id_budget_departamento = AddFirstItem(new SelectList(db.budget_departamentos.Where(p => p.activo == true), "id", "descripcion"), textoPorDefecto: "-- Seleccionar --");
            ViewBag.ListadoEmpleados = db.empleados.Where(x => x.activo == true).ToList();
            return View(item);
        }

        // GET: BG_CentroCosto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            budget_centro_costo budget_centro_costo = db.budget_centro_costo.Find(id);
            if (budget_centro_costo == null)
            {
                return HttpNotFound();
            }
            return View(budget_centro_costo);
        }

        // POST: BG_CentroCosto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            budget_centro_costo budget_centro_costo = db.budget_centro_costo.Find(id);
            db.budget_centro_costo.Remove(budget_centro_costo);
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
