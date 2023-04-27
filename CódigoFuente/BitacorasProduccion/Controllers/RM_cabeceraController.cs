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
    public class RM_cabeceraController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: RM_cabecera
        public ActionResult Index(int? id_planta, int? almacenClave, int? clave, int? motivoClave, int? clienteClave, string clienteOtro, string estatus, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.RM_CREACION) && !TieneRol(TipoRoles.RM_DETALLES) && !TieneRol(TipoRoles.RM_REPORTES))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro

            //valida almacen
            if (almacenClave != null && !db.RM_almacen.Any(x => x.clave == almacenClave && x.plantaClave == id_planta))
                almacenClave = null;

            //valida id_remision
            if (clave != null && !db.RM_cabecera.Any(x => (id_planta == null || x.RM_almacen.plantaClave == id_planta)
                                                        && (almacenClave == null || almacenClave == x.almacenClave)
                                                        && x.clave == clave
                                                        && (motivoClave == null || x.motivoClave == motivoClave)
                                                        && (clienteClave == null || x.clienteClave == clienteClave)
                                                        && (string.IsNullOrEmpty(clienteOtro) || x.clienteOtro.Contains(clienteOtro))
                                                        && ((string.IsNullOrEmpty(estatus) && x.ultimoEstatus.HasValue)
                                                                        || (estatus == "PENDIENTES" && (x.ultimoEstatus == 1 || x.ultimoEstatus == 2))
                                                                        || (estatus == "APROBADAS" && (x.ultimoEstatus == 3))
                                                                        || (estatus == "REGULARIZADAS" && (x.ultimoEstatus == 4))
                                                                        || (estatus == "CANCELADAS" && (x.ultimoEstatus == 5))
                                                           )
                                                        && x.activo
                                                        )
        )
                clave = null;

            //valida cliente
            if (clienteClave != null && !db.RM_cabecera.Any(x => (id_planta == null || x.RM_almacen.plantaClave == id_planta)
                                                        && (almacenClave == null || almacenClave == x.almacenClave)
                                                        && (clave == null || x.clave == clave)
                                                        && (motivoClave == null || x.motivoClave == motivoClave)
                                                        && (x.clienteClave == clienteClave)
                                                        && (string.IsNullOrEmpty(clienteOtro) || x.clienteOtro.Contains(clienteOtro))
                                                         && ((string.IsNullOrEmpty(estatus) && x.ultimoEstatus.HasValue)
                                                                        || (estatus == "PENDIENTES" && (x.ultimoEstatus == 1 || x.ultimoEstatus == 2))
                                                                        || (estatus == "APROBADAS" && (x.ultimoEstatus == 3))
                                                                        || (estatus == "REGULARIZADAS" && (x.ultimoEstatus == 4))
                                                                        || (estatus == "CANCELADAS" && (x.ultimoEstatus == 5))
                                                           )
                                                        && x.activo
                                                        )
                                                        )
                clienteClave = null;

            //obtiene el total de registros, según los filtros 
            var listado = db.RM_cabecera
               .Where(x =>
                       (id_planta == null || x.RM_almacen.plantaClave == id_planta)
                       && (almacenClave == null || almacenClave == x.almacenClave)
                       && (clave == null || x.clave == clave)
                       && (motivoClave == null || x.motivoClave == motivoClave)
                       && (clienteClave == null || x.clienteClave == clienteClave)
                       && (string.IsNullOrEmpty(clienteOtro) || x.clienteOtro.Contains(clienteOtro))
                      && ((string.IsNullOrEmpty(estatus) && x.ultimoEstatus.HasValue)
                                                                        || (estatus == "PENDIENTES" && (x.ultimoEstatus == 1 || x.ultimoEstatus == 2))
                                                                        || (estatus == "APROBADAS" && (x.ultimoEstatus == 3))
                                                                        || (estatus == "REGULARIZADAS" && (x.ultimoEstatus == 4))
                                                                        || (estatus == "CANCELADAS" && (x.ultimoEstatus == 5))
                                                           )
                                                        && x.activo
                 )
                .OrderByDescending(x => x.clave)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();


            //obtiene el listado de renisiones
            var remisionesList = db.RM_cabecera
                  .Where(x =>
                        (id_planta == null || x.RM_almacen.plantaClave == id_planta)
                        && (almacenClave == null || almacenClave == x.almacenClave)
                        //&& (clave == null || x.clave == clave)
                        && (motivoClave == null || x.motivoClave == motivoClave)
                        && (clienteClave == null || x.clienteClave == clienteClave)
                        && (string.IsNullOrEmpty(clienteOtro) || x.clienteOtro.Contains(clienteOtro))
                        && ((string.IsNullOrEmpty(estatus) && x.ultimoEstatus.HasValue)
                                                                        || (estatus == "PENDIENTES" && (x.ultimoEstatus == 1 || x.ultimoEstatus == 2))
                                                                        || (estatus == "APROBADAS" && (x.ultimoEstatus == 3))
                                                                        || (estatus == "REGULARIZADAS" && (x.ultimoEstatus == 4))
                                                                        || (estatus == "CANCELADAS" && (x.ultimoEstatus == 5))
                                                           )
                        && x.activo
                     )
                    .OrderBy(x => x.almacenClave);


            //obtiene la cantidad de registros
            var totalDeRegistros = remisionesList.Count();

            //variables para la páginacion
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id_planta"] = id_planta;
            routeValues["almacenClave"] = almacenClave;
            routeValues["clave"] = clave;
            routeValues["motivoClave"] = motivoClave;
            routeValues["clienteClave"] = clienteClave;
            routeValues["clienteOtro"] = clienteOtro;
            routeValues["estatus"] = estatus;
            routeValues["pagina"] = pagina;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.Paginacion = paginacion;

            var remisionesListClientes = db.RM_cabecera
                 .Where(x =>
                       (id_planta == null || x.RM_almacen.plantaClave == id_planta)
                       && (almacenClave == null || almacenClave == x.almacenClave)
                       && (clave == null || x.clave == clave)
                       && (motivoClave == null || x.motivoClave == motivoClave)
                       //& (clienteClave == null || x.clienteClave == clienteClave)
                       && (string.IsNullOrEmpty(clienteOtro) || x.clienteOtro.Contains(clienteOtro))
                       && ((string.IsNullOrEmpty(estatus) && x.ultimoEstatus.HasValue)
                                                                        || (estatus == "PENDIENTES" && (x.ultimoEstatus == 1 || x.ultimoEstatus == 2))
                                                                        || (estatus == "APROBADAS" && (x.ultimoEstatus == 3))
                                                                        || (estatus == "REGULARIZADAS" && (x.ultimoEstatus == 4))
                                                                        || (estatus == "CANCELADAS" && (x.ultimoEstatus == 5))
                                                           )
                       && x.activo
                    )
                    .Select(x => x.clientes)
                    .Distinct();


            //map para estatus
            Dictionary<string, string> estatusMap = new Dictionary<string, string>();
            estatusMap.Add("", "Todos");
            estatusMap.Add("PENDIENTES", "Pendientes de Aprobar");
            estatusMap.Add("APROBADAS", "Aprobadas y Pendientes de regularizar en SAP");
            estatusMap.Add("REGULARIZADAS", "Regularizadas en SAP/Cerradas");
            estatusMap.Add("CANCELADAS", "Canceladas");
            ViewBag.estatusMap = estatusMap;

            //linq para obtener el total de registros con su estatus
            var totalDeRegistrosEstatus = db.RM_cabecera
                .Where(x =>
                      (id_planta == null || x.RM_almacen.plantaClave == id_planta)
                      && (almacenClave == null || almacenClave == x.almacenClave)
                      && (clave == null || x.clave == clave)
                      && (motivoClave == null || x.motivoClave == motivoClave)
                      && (clienteClave == null || x.clienteClave == clienteClave)
                      && (string.IsNullOrEmpty(clienteOtro) || x.clienteOtro.Contains(clienteOtro))
                      && x.ultimoEstatus.HasValue
                     && x.activo
              )
                .Select(x => x.ultimoEstatus);


            //map para cantidad de status
            Dictionary<string, int> estatusAmount = new Dictionary<string, int>();
            estatusAmount.Add("", totalDeRegistrosEstatus.Count());
            estatusAmount.Add("PENDIENTES", totalDeRegistrosEstatus.Count(x => x == 1 || x == 2));
            estatusAmount.Add("APROBADAS", totalDeRegistrosEstatus.Count(x => x == 3));
            estatusAmount.Add("REGULARIZADAS", totalDeRegistrosEstatus.Count(x => x == 4));
            estatusAmount.Add("CANCELADAS", totalDeRegistrosEstatus.Count(x => x == 5));
            ViewBag.estatusAmount = estatusAmount;


            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Todas --", selected: id_planta.ToString());
            ViewBag.almacenClave = AddFirstItem(new SelectList(db.RM_almacen.Where(x => x.activo && x.plantaClave == id_planta), nameof(RM_almacen.clave), nameof(RM_almacen.descripcion)), textoPorDefecto: "-- Todos --", selected: almacenClave.ToString());
            ViewBag.clave = AddFirstItem(new SelectList(remisionesList, nameof(RM_cabecera.clave), nameof(RM_cabecera.ConcatNumeroRemision)), textoPorDefecto: "-- Todos --", selected: clave.ToString());
            ViewBag.motivoClave = AddFirstItem(new SelectList(db.RM_remision_motivo.Where(x => x.activo), nameof(RM_remision_motivo.clave), nameof(RM_remision_motivo.descripcion)), textoPorDefecto: "-- Cualquiera --", selected: motivoClave.ToString());
            ViewBag.clienteClave = AddFirstItem(new SelectList(remisionesListClientes.Where(x => x != null), nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)), textoPorDefecto: "-- Cualquiera --", selected: clienteClave.ToString());

            return View(listado);
        }

        // GET: RM_cabecera/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.RM_CREACION))
                return View("../Home/ErrorPermisos");

            if (id == null)
                return View("../Error/BadRequest");

            RM_cabecera rM_cabecera = db.RM_cabecera.Find(id);
            if (rM_cabecera == null)
                return View("../Error/NotFound");

            return View(rM_cabecera);
        }

        // GET: RM_cabecera/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.RM_CREACION))
                return View("../Home/ErrorPermisos");

            RM_cabecera model = new RM_cabecera
            {
                observaciones = "Se crea remisión."
            };

            var empleado = obtieneEmpleadoLogeado();
            ViewBag.EmpleadoActual = empleado;

            ViewBag.clienteClave = AddFirstItem(new SelectList(db.clientes, nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)));
            ViewBag.enviadoAClave = AddFirstItem(new SelectList(db.clientes, nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)));
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), nameof(plantas.clave), nameof(plantas.descripcion)));
            ViewBag.almacenClave = AddFirstItem(new SelectList(db.RM_almacen.Where(x => x.activo && x.plantaClave == model.id_planta), nameof(RM_almacen.clave), nameof(RM_almacen.descripcion)));
            ViewBag.motivoClave = AddFirstItem(new SelectList(db.RM_remision_motivo.Where(x => x.activo), nameof(RM_remision_motivo.clave), nameof(RM_remision_motivo.descripcion)));
            ViewBag.transporteProveedorClave = AddFirstItem(new SelectList(db.RM_transporte_proveedor.Where(x => x.activo), nameof(RM_transporte_proveedor.clave), nameof(RM_transporte_proveedor.descripcion)));
            return View(model);
        }

        // POST: RM_cabecera/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RM_cabecera rM_cabecera)
        {
            if (ModelState.IsValid)
            {
                db.RM_cabecera.Add(rM_cabecera);
                //     db.SaveChanges();
                return RedirectToAction("Index");
            }

            var empleado = obtieneEmpleadoLogeado();
            ViewBag.EmpleadoActual = empleado;

            ViewBag.clienteClave = AddFirstItem(new SelectList(db.clientes, nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)));
            ViewBag.enviadoAClave = AddFirstItem(new SelectList(db.clientes, nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)));
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), nameof(plantas.clave), nameof(plantas.descripcion)));
            ViewBag.almacenClave = AddFirstItem(new SelectList(db.RM_almacen.Where(x => x.activo && x.plantaClave == rM_cabecera.id_planta), nameof(RM_almacen.clave), nameof(RM_almacen.descripcion)));
            ViewBag.motivoClave = AddFirstItem(new SelectList(db.RM_remision_motivo.Where(x => x.activo), nameof(RM_remision_motivo.clave), nameof(RM_remision_motivo.descripcion)));
            ViewBag.transporteProveedorClave = AddFirstItem(new SelectList(db.RM_transporte_proveedor.Where(x => x.activo), nameof(RM_transporte_proveedor.clave), nameof(RM_transporte_proveedor.descripcion)));
            return View(rM_cabecera);
        }

        // GET: RM_cabecera/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RM_cabecera rM_cabecera = db.RM_cabecera.Find(id);
            if (rM_cabecera == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteClave = new SelectList(db.clientes, "clave", "claveSAP", rM_cabecera.clienteClave);
            ViewBag.enviadoAClave = new SelectList(db.clientes, "clave", "claveSAP", rM_cabecera.enviadoAClave);
            ViewBag.almacenClave = new SelectList(db.RM_almacen, "clave", "descripcion", rM_cabecera.almacenClave);
            ViewBag.motivoClave = new SelectList(db.RM_remision_motivo, "clave", "descripcion", rM_cabecera.motivoClave);
            ViewBag.transporteProveedorClave = new SelectList(db.RM_transporte_proveedor, "clave", "descripcion", rM_cabecera.transporteProveedorClave);
            return View(rM_cabecera);
        }

        // POST: RM_cabecera/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "clave,activo,remisionNumero,almacenClave,transporteOtro,transporteProveedorClave,nombreChofer,clienteClave,clienteOtro,placaTractor,placaRemolque,horarioDescarga,enviadoAClave,enviadoAOtro,clienteOtroDireccion,enviadoAOtroDireccion,motivoClave,motivoTexto,retornaMaterial")] RM_cabecera rM_cabecera)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rM_cabecera).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteClave = new SelectList(db.clientes, "clave", "claveSAP", rM_cabecera.clienteClave);
            ViewBag.enviadoAClave = new SelectList(db.clientes, "clave", "claveSAP", rM_cabecera.enviadoAClave);
            ViewBag.almacenClave = new SelectList(db.RM_almacen, "clave", "descripcion", rM_cabecera.almacenClave);
            ViewBag.motivoClave = new SelectList(db.RM_remision_motivo, "clave", "descripcion", rM_cabecera.motivoClave);
            ViewBag.transporteProveedorClave = new SelectList(db.RM_transporte_proveedor, "clave", "descripcion", rM_cabecera.transporteProveedorClave);
            return View(rM_cabecera);
        }

        // GET: RM_cabecera/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RM_cabecera rM_cabecera = db.RM_cabecera.Find(id);
            if (rM_cabecera == null)
            {
                return HttpNotFound();
            }
            return View(rM_cabecera);
        }

        // POST: RM_cabecera/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RM_cabecera rM_cabecera = db.RM_cabecera.Find(id);
            db.RM_cabecera.Remove(rM_cabecera);
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
