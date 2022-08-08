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
    public class IT_mantenimientosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_mantenimientos
        public ActionResult Index(int? id_empleado, int? planta_clave, string estatus_mantenimiento = "", int id_it_inventory_item = 0, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.IT_MANTENIMIENTO_REGISTRO))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro


            var listado = db.IT_mantenimientos
                .Where(x => (x.IT_inventory_items.id_planta == planta_clave || planta_clave == null)
                    && (x.id_empleado_responsable == id_empleado
                        || x.IT_inventory_items.IT_asignacion_hardware_rel_items.Any(y => y.IT_asignacion_hardware.es_asignacion_actual == true
                            && id_empleado == y.IT_asignacion_hardware.id_empleado
                            /*&& id_empleado == y.IT_asignacion_hardware.id_responsable_principal*/)
                        || id_empleado == null
                        )
                        && (x.id_it_inventory_item == id_it_inventory_item || id_it_inventory_item == 0)
                        && (
                        (estatus_mantenimiento == IT_matenimiento_Estatus.REALIZADO && x.fecha_realizacion.HasValue)
                        || (estatus_mantenimiento == IT_matenimiento_Estatus.VENCIDO && x.fecha_programada < DateTime.Now && x.fecha_realizacion == null)
                        || (estatus_mantenimiento == IT_matenimiento_Estatus.PROXIMO && x.fecha_programada > DateTime.Now && x.fecha_realizacion == null)
                        || String.IsNullOrEmpty(estatus_mantenimiento)

                        )
                )
                   .OrderByDescending(x => x.id)
                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.IT_mantenimientos
                  .Where(x => (x.IT_inventory_items.id_planta == planta_clave || planta_clave == null)
                    && (x.id_empleado_responsable == id_empleado
                        || x.IT_inventory_items.IT_asignacion_hardware_rel_items.Any(y => y.IT_asignacion_hardware.es_asignacion_actual == true
                            && id_empleado == y.IT_asignacion_hardware.id_empleado
                            /*&& id_empleado == y.IT_asignacion_hardware.id_responsable_principal*/)
                        || id_empleado == null
                        )
                        && (x.id_it_inventory_item == id_it_inventory_item || id_it_inventory_item == 0)
                        && (
                        (estatus_mantenimiento == IT_matenimiento_Estatus.REALIZADO && x.fecha_realizacion.HasValue)
                        || (estatus_mantenimiento == IT_matenimiento_Estatus.VENCIDO && x.fecha_programada < DateTime.Now && x.fecha_realizacion == null)
                        || (estatus_mantenimiento == IT_matenimiento_Estatus.PROXIMO && x.fecha_programada > DateTime.Now && x.fecha_realizacion == null)
                        || String.IsNullOrEmpty(estatus_mantenimiento)

                        )
                )
                .Count();

            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["planta_clave"] = planta_clave;
            routeValues["id_empleado"] = id_empleado;
            routeValues["id_it_inventory_item"] = id_it_inventory_item;
            routeValues["estatus_mantenimiento"] = estatus_mantenimiento;


            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            //select list para responsiva
            List<SelectListItem> newListStatusMantenimiento = new List<SelectListItem>();
            newListStatusMantenimiento.Add(new SelectListItem() { Text = IT_matenimiento_Estatus.DescripcionStatus(IT_matenimiento_Estatus.PROXIMO), Value = IT_matenimiento_Estatus.PROXIMO });
            newListStatusMantenimiento.Add(new SelectListItem() { Text = IT_matenimiento_Estatus.DescripcionStatus(IT_matenimiento_Estatus.REALIZADO), Value = IT_matenimiento_Estatus.REALIZADO });
            newListStatusMantenimiento.Add(new SelectListItem() { Text = IT_matenimiento_Estatus.DescripcionStatus(IT_matenimiento_Estatus.VENCIDO), Value = IT_matenimiento_Estatus.VENCIDO });
            SelectList selectListStatusResponsiva = new SelectList(newListStatusMantenimiento, "Value", "Text");

            ViewBag.Paginacion = paginacion;
            ViewBag.estatus_mantenimiento = AddFirstItem(selectListStatusResponsiva, selected: estatus_mantenimiento.ToString(), textoPorDefecto: "-- Todos --");
            ViewBag.planta_clave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", planta_clave.ToString()), textoPorDefecto: "-- Todas --");
            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados, "id", "ConcatNumEmpleadoNombre"), selected: id_empleado.ToString(), textoPorDefecto: "-- Todos --");
            ViewBag.id_it_inventory_item = AddFirstItem(new SelectList(db.IT_inventory_items.OrderBy(x => x.id_inventory_type), "id", "ConcatInfoSearch"), selected: id_it_inventory_item.ToString(), textoPorDefecto: "-- Todos --");

            return View(listado);
        }



        // GET: IT_mantenimientos/CerrarMantenimiento/5
        public ActionResult CerrarMantenimiento(int? id)
        {
            if (!TieneRol(TipoRoles.IT_MANTENIMIENTO_REGISTRO))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_mantenimientos iT_mantenimientos = db.IT_mantenimientos.Find(id);
            if (iT_mantenimientos == null)
            {
                return HttpNotFound();
            }
            
            if (iT_mantenimientos.estatus == Bitacoras.Util.IT_matenimiento_Estatus.REALIZADO)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede editar un registro que ha sido Finalizado!";
                ViewBag.Descripcion = "No se puede editar un registro de mantenimiento que ya ha sido marcado como finalizado.";

                return View("../Home/ErrorGenerico");
            }

            

            var sistemas = obtieneEmpleadoLogeado();

            //Asigna los valores del empleado de sistemas 
            iT_mantenimientos.id_empleado_sistemas = sistemas.id;
            iT_mantenimientos.empleados1 = sistemas;

            //id responsable por defecto
            int id_responsable_default = 0;

            if (iT_mantenimientos.id_empleado_responsable.HasValue)
                id_responsable_default = iT_mantenimientos.id_empleado_responsable.Value;
            else
            {
                //obtiene el valor del responsable principal
                var asignacion = db.IT_asignacion_hardware_rel_items.Where(x => x.id_it_inventory_item == iT_mantenimientos.id_it_inventory_item && x.IT_asignacion_hardware.es_asignacion_actual == true && x.IT_asignacion_hardware.id_empleado == x.IT_asignacion_hardware.id_responsable_principal).FirstOrDefault();
                if (asignacion != null)
                    id_responsable_default = asignacion.IT_asignacion_hardware.id_empleado;
            }


            //agrega elementos para los rel checklist            
            foreach (var itemCK in db.IT_mantenimientos_checklist_item.Where(x => x.activo))
            {
                //solo agrega si no existe previamente 
                if (!iT_mantenimientos.IT_mantenimientos_rel_checklist.Any(x => x.id_item_checklist_mantenimiento == itemCK.id))
                    iT_mantenimientos.IT_mantenimientos_rel_checklist.Add(new IT_mantenimientos_rel_checklist
                    {
                        id_item_checklist_mantenimiento = itemCK.id,
                        IT_mantenimientos_checklist_item = itemCK,
                        id_mantenimiento = iT_mantenimientos.id,
                        terminado = false
                    });
            }

            ViewBag.id_empleado_responsable = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true && x.planta_clave == iT_mantenimientos.IT_inventory_items.id_planta), "id", "ConcatNumEmpleadoNombre"), selected: id_responsable_default.ToString(), textoPorDefecto: "-- Seleccionar --");

            return View(iT_mantenimientos);
        }

        // POST: IT_mantenimientos/CerrarMantenimiento/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CerrarMantenimiento(IT_mantenimientos iT_mantenimientos)
        {
            //si es finalizar asocia docto iatf
            if (iT_mantenimientos.finalizar_mantenimiento)
            {
                //primero obtiene los documentos asociados al proceso y de la planta del usuario de sistemas
                empleados empleado_sistemas = db.empleados.Find(iT_mantenimientos.id_empleado_sistemas);
                var iatf_docto = db.IATF_documentos.Where(x => x.proceso == Bitacoras.Util.DocumentosProcesos.IT_FORMATO_HOJA_DE_VIDA 
                                && x.id_planta == empleado_sistemas.planta_clave).FirstOrDefault();

                if (iatf_docto != null && iatf_docto.IATF_revisiones.Count == 0)
                    ModelState.AddModelError("", "No se encontraron revisiones asociadas al documento IATF.");

                //selecciona la revision más reciente
                if (iatf_docto != null)
                {
                    int id_version_iatf = iatf_docto.IATF_revisiones.OrderByDescending(x => x.fecha_revision).Take(1).Select(x => x.id).FirstOrDefault();
                   //asigna la versión de IATF
                    iT_mantenimientos.id_iatf_version = id_version_iatf;
                }
                //asigna la fecha de realización
                iT_mantenimientos.fecha_realizacion = DateTime.Now;
            }

            if (ModelState.IsValid)
            {                

                //borra los conceptos anteriores
                var list = db.IT_mantenimientos_rel_checklist.Where(x => x.id_mantenimiento == iT_mantenimientos.id);
                foreach (var itemRemove in list)
                    db.IT_mantenimientos_rel_checklist.Remove(itemRemove);

                //agrega los nuevos items rel 
                foreach (var iteamAdd in iT_mantenimientos.IT_mantenimientos_rel_checklist)
                    db.IT_mantenimientos_rel_checklist.Add(iteamAdd);

                db.Entry(iT_mantenimientos).State = EntityState.Modified;

                //si es finalizar debe agregar una nueva entrada segun los meses de mantenimiento
                if (iT_mantenimientos.finalizar_mantenimiento) {
                    var inventory_item = db.IT_inventory_items.Find(iT_mantenimientos.id_it_inventory_item);

                    var nuevo_manto = new IT_mantenimientos { 
                        id_it_inventory_item = inventory_item.id,
                        fecha_programada = DateTime.Now.AddMonths(inventory_item.maintenance_period_months.HasValue 
                        ? inventory_item.maintenance_period_months.Value : 6), //seis meses por defecto
                    };

                    db.IT_mantenimientos.Add(nuevo_manto);
                }

                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            //asigna propiedades previas
            iT_mantenimientos.IT_inventory_items = db.IT_inventory_items.Find(iT_mantenimientos.id_it_inventory_item);
            iT_mantenimientos.empleados = db.empleados.Find(iT_mantenimientos.id_empleado_responsable);
            iT_mantenimientos.empleados1 = db.empleados.Find(iT_mantenimientos.id_empleado_sistemas);

            //obtiene la propiedades para rels
            foreach (var rel in iT_mantenimientos.IT_mantenimientos_rel_checklist) {
                rel.IT_mantenimientos_checklist_item = db.IT_mantenimientos_checklist_item.Find(rel.id_item_checklist_mantenimiento);
            }

            ViewBag.id_empleado_responsable = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true && x.planta_clave == iT_mantenimientos.IT_inventory_items.id_planta), "id", "ConcatNumEmpleadoNombre"), selected: iT_mantenimientos.id_empleado_responsable.ToString(), textoPorDefecto: "-- Seleccionar --");

            return View(iT_mantenimientos);
        }

        //// GET: IT_mantenimientos/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    IT_mantenimientos iT_mantenimientos = db.IT_mantenimientos.Find(id);
        //    if (iT_mantenimientos == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(iT_mantenimientos);
        //}

        //// GET: IT_mantenimientos/Create
        //public ActionResult Create()
        //{
        //    ViewBag.id_biblioteca_digital = new SelectList(db.biblioteca_digital, "Id", "Nombre");
        //    ViewBag.id_empleado_responsable = new SelectList(db.empleados, "id", "numeroEmpleado");
        //    ViewBag.id_empleado_sistemas = new SelectList(db.empleados, "id", "numeroEmpleado");
        //    ViewBag.id_iatf_version = new SelectList(db.IATF_revisiones, "id", "responsable");
        //    ViewBag.id_it_inventory_item = new SelectList(db.IT_inventory_items, "id", "comments");
        //    return View();
        //}

        //// POST: IT_mantenimientos/Create
        //// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        //// más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id,id_it_inventory_item,id_empleado_responsable,id_empleado_sistemas,id_iatf_version,id_biblioteca_digital,fecha_programada,fecha_realizacion,comentarios")] IT_mantenimientos iT_mantenimientos)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.IT_mantenimientos.Add(iT_mantenimientos);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.id_biblioteca_digital = new SelectList(db.biblioteca_digital, "Id", "Nombre", iT_mantenimientos.id_biblioteca_digital);
        //    ViewBag.id_empleado_responsable = new SelectList(db.empleados, "id", "numeroEmpleado", iT_mantenimientos.id_empleado_responsable);
        //    ViewBag.id_empleado_sistemas = new SelectList(db.empleados, "id", "numeroEmpleado", iT_mantenimientos.id_empleado_sistemas);
        //    ViewBag.id_iatf_version = new SelectList(db.IATF_revisiones, "id", "responsable", iT_mantenimientos.id_iatf_version);
        //    ViewBag.id_it_inventory_item = new SelectList(db.IT_inventory_items, "id", "comments", iT_mantenimientos.id_it_inventory_item);
        //    return View(iT_mantenimientos);
        //}

        //// GET: IT_mantenimientos/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    IT_mantenimientos iT_mantenimientos = db.IT_mantenimientos.Find(id);
        //    if (iT_mantenimientos == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.id_biblioteca_digital = new SelectList(db.biblioteca_digital, "Id", "Nombre", iT_mantenimientos.id_biblioteca_digital);
        //    ViewBag.id_empleado_responsable = new SelectList(db.empleados, "id", "numeroEmpleado", iT_mantenimientos.id_empleado_responsable);
        //    ViewBag.id_empleado_sistemas = new SelectList(db.empleados, "id", "numeroEmpleado", iT_mantenimientos.id_empleado_sistemas);
        //    ViewBag.id_iatf_version = new SelectList(db.IATF_revisiones, "id", "responsable", iT_mantenimientos.id_iatf_version);
        //    ViewBag.id_it_inventory_item = new SelectList(db.IT_inventory_items, "id", "comments", iT_mantenimientos.id_it_inventory_item);
        //    return View(iT_mantenimientos);
        //}

        //// POST: IT_mantenimientos/Edit/5
        //// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        //// más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "id,id_it_inventory_item,id_empleado_responsable,id_empleado_sistemas,id_iatf_version,id_biblioteca_digital,fecha_programada,fecha_realizacion,comentarios")] IT_mantenimientos iT_mantenimientos)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(iT_mantenimientos).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.id_biblioteca_digital = new SelectList(db.biblioteca_digital, "Id", "Nombre", iT_mantenimientos.id_biblioteca_digital);
        //    ViewBag.id_empleado_responsable = new SelectList(db.empleados, "id", "numeroEmpleado", iT_mantenimientos.id_empleado_responsable);
        //    ViewBag.id_empleado_sistemas = new SelectList(db.empleados, "id", "numeroEmpleado", iT_mantenimientos.id_empleado_sistemas);
        //    ViewBag.id_iatf_version = new SelectList(db.IATF_revisiones, "id", "responsable", iT_mantenimientos.id_iatf_version);
        //    ViewBag.id_it_inventory_item = new SelectList(db.IT_inventory_items, "id", "comments", iT_mantenimientos.id_it_inventory_item);
        //    return View(iT_mantenimientos);
        //}

        //// GET: IT_mantenimientos/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    IT_mantenimientos iT_mantenimientos = db.IT_mantenimientos.Find(id);
        //    if (iT_mantenimientos == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(iT_mantenimientos);
        //}

        //// POST: IT_mantenimientos/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    IT_mantenimientos iT_mantenimientos = db.IT_mantenimientos.Find(id);
        //    db.IT_mantenimientos.Remove(iT_mantenimientos);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
