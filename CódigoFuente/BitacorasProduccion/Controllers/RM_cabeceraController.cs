using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
            if (!TieneRol(TipoRoles.RM_DETALLES) && !TieneRol(TipoRoles.RM_CREACION) && !TieneRol(TipoRoles.RM_APROBAR) && !TieneRol(TipoRoles.RM_APROBAR) && !TieneRol(TipoRoles.RM_REPORTES))
                return View("../Home/ErrorPermisos");

            if (id == null)
                return View("../Error/BadRequest");

            RM_cabecera rM_cabecera = db.RM_cabecera.Find(id);
            if (rM_cabecera == null)
                return View("../Error/NotFound");

            //establece los valores para aplicaEnviadoOtro, aplicaClienteOtro y aplicaTransporteOtro
            rM_cabecera.aplicaClienteOtro = rM_cabecera.clientes == null;
            rM_cabecera.aplicaEnviadoOtro = rM_cabecera.clientes1 == null;
            rM_cabecera.aplicaTransporteOtro = rM_cabecera.RM_transporte_proveedor == null;

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


            //se obtiene el listado de almacenes preferidos
            var almacenesPrefList = db.RM_cambio_estatus.Where(x => x.id_empleado == empleado.id && x.catalogoEstatusClave == (int)RM_estatus_enum.Creada)
                .GroupBy(x => x.RM_cabecera.RM_almacen);

            //crea almacen por defecto
            RM_almacen almacenPreferido = new RM_almacen { clave = 0, plantaClave = 0 };
            //si hay almacen lo asigna
            if (almacenesPrefList.Count() > 0)
                almacenPreferido = almacenesPrefList.OrderByDescending(x => x.Count()).First().Key;

            ViewBag.clienteClave = AddFirstItem(new SelectList(db.clientes, nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)));
            ViewBag.enviadoAClave = AddFirstItem(new SelectList(db.clientes, nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)));
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), nameof(plantas.clave), nameof(plantas.descripcion)), selected: almacenPreferido != null ? almacenPreferido.plantaClave.ToString() : String.Empty);
            ViewBag.almacenClave = AddFirstItem(new SelectList(db.RM_almacen.Where(x => x.activo && x.plantaClave == almacenPreferido.plantaClave), nameof(RM_almacen.clave), nameof(RM_almacen.descripcion)), selected: almacenPreferido != null ? almacenPreferido.clave.ToString() : String.Empty);
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

            //  ModelState.AddModelError("", "Error para depuración.");

            if (rM_cabecera.RM_elemento.Count == 0)
                ModelState.AddModelError("", "No se agregaron elementos a la remisión.");


            var empleado = obtieneEmpleadoLogeado();

            if (ModelState.IsValid)
            {
                DateTime fechaActual = DateTime.Now;

                //actualiza la fecha para todos lo elementos
                rM_cabecera.RM_elemento = rM_cabecera.RM_elemento.Select(x => { x.capturaFecha = fechaActual; x.activo = true; return x; }).ToList();

                //se agrega el cambio de estatus
                rM_cabecera.RM_cambio_estatus.Add(new RM_cambio_estatus
                {
                    capturaFecha = fechaActual,
                    id_empleado = empleado.id,
                    catalogoEstatusClave = (int)RM_estatus_enum.Creada,
                    texto = String.Format("<code>Obsevaciones </code>{0}<br />", rM_cabecera.observaciones)
                });
                //establece el valor del último estatus en la cabecera
                rM_cabecera.ultimoEstatus = (int)RM_estatus_enum.Creada;

                //guarda en BD
                db.RM_cabecera.Add(rM_cabecera);
                db.SaveChanges();

                //obtiene el valor del almacen, para poder utilizar ConcatNumeroRemision
                rM_cabecera.RM_almacen = db.RM_almacen.Find(rM_cabecera.almacenClave);

                TempData["Mensaje"] = new MensajesSweetAlert("Se ha creado la remisión: " + rM_cabecera.ConcatNumeroRemision, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

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
            if (!TieneRol(TipoRoles.RM_CREACION))
                return View("../Home/ErrorPermisos");

            if (id == null)
                return View("../Error/BadRequest");

            RM_cabecera rM_cabecera = db.RM_cabecera.Find(id);
            if (rM_cabecera == null)
                return View("../Error/NotFound");

            var empleado = obtieneEmpleadoLogeado();
            ViewBag.EmpleadoActual = empleado;

            //establece los valores para aplicaEnviadoOtro, aplicaClienteOtro y aplicaTransporteOtro
            rM_cabecera.aplicaClienteOtro = rM_cabecera.clientes == null;
            rM_cabecera.aplicaEnviadoOtro = rM_cabecera.clientes1 == null;
            rM_cabecera.aplicaTransporteOtro = rM_cabecera.RM_transporte_proveedor == null;

            ViewBag.clienteClave = AddFirstItem(new SelectList(db.clientes, nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)), selected: rM_cabecera.clienteClave.ToString());
            ViewBag.enviadoAClave = AddFirstItem(new SelectList(db.clientes, nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)), selected: rM_cabecera.enviadoAClave.ToString());
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), nameof(plantas.clave), nameof(plantas.descripcion)), selected: rM_cabecera.RM_almacen.plantas.clave.ToString());
            ViewBag.almacenClave = AddFirstItem(new SelectList(db.RM_almacen.Where(x => x.activo && x.plantaClave == rM_cabecera.RM_almacen.plantas.clave), nameof(RM_almacen.clave), nameof(RM_almacen.descripcion)), selected: rM_cabecera.RM_almacen.clave.ToString());
            ViewBag.motivoClave = AddFirstItem(new SelectList(db.RM_remision_motivo.Where(x => x.activo), nameof(RM_remision_motivo.clave), nameof(RM_remision_motivo.descripcion)), selected: rM_cabecera.motivoClave.ToString());
            ViewBag.transporteProveedorClave = AddFirstItem(new SelectList(db.RM_transporte_proveedor.Where(x => x.activo), nameof(RM_transporte_proveedor.clave), nameof(RM_transporte_proveedor.descripcion)), selected: rM_cabecera.transporteProveedorClave.ToString());
            return View(rM_cabecera);
        }

        // POST: RM_cabecera/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RM_cabecera rM_form)
        {
            // ModelState.AddModelError("", "Error para depuración.");

            var empleado = obtieneEmpleadoLogeado();

            if (ModelState.IsValid)
            {
                DateTime fechaActual = DateTime.Now;

                //obtiene el elemento desde BD
                var remisionBD = db.RM_cabecera.Find(rM_form.clave);

                //inicia texto de cambios             
                string texto = String.Format("<code>Obsevaciones </code>{0}<br/>", UsoStrings.RecortaString(rM_form.observaciones, 950));

                CompararPropiedades(remisionBD, rM_form, ref texto);

                //se agrega el cambio de estatus
                remisionBD.RM_cambio_estatus.Add(new RM_cambio_estatus
                {
                    capturaFecha = fechaActual,
                    id_empleado = empleado.id,
                    catalogoEstatusClave = (int)RM_estatus_enum.Editada,
                    texto = texto
                });

                //establece el valor del último estatus en la cabecera
                remisionBD.ultimoEstatus = (int)RM_estatus_enum.Editada;


                try
                {
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ha editado la remisión: " + remisionBD.ConcatNumeroRemision, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Ocurrió un error: " + e.Message);
                }
                finally
                {
                    db.Configuration.ValidateOnSaveEnabled = true;
                }

            }
            ViewBag.EmpleadoActual = empleado;

            //necesario para viewbag
            rM_form.RM_almacen = db.RM_almacen.Find(rM_form.almacenClave);

            ViewBag.clienteClave = AddFirstItem(new SelectList(db.clientes, nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)), selected: rM_form.clienteClave.ToString());
            ViewBag.enviadoAClave = AddFirstItem(new SelectList(db.clientes, nameof(clientes.clave), nameof(clientes.ConcatClienteSAP)), selected: rM_form.enviadoAClave.ToString());
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), nameof(plantas.clave), nameof(plantas.descripcion)), selected: (rM_form.RM_almacen != null ? rM_form.RM_almacen.plantas.clave.ToString() : String.Empty));
            ViewBag.almacenClave = AddFirstItem(new SelectList(db.RM_almacen.Where(x => x.activo && x.plantaClave == rM_form.RM_almacen.plantas.clave), nameof(RM_almacen.clave), nameof(RM_almacen.descripcion)), selected: rM_form.almacenClave.ToString());
            ViewBag.motivoClave = AddFirstItem(new SelectList(db.RM_remision_motivo.Where(x => x.activo), nameof(RM_remision_motivo.clave), nameof(RM_remision_motivo.descripcion)), selected: rM_form.motivoClave.ToString());
            ViewBag.transporteProveedorClave = AddFirstItem(new SelectList(db.RM_transporte_proveedor.Where(x => x.activo), nameof(RM_transporte_proveedor.clave), nameof(RM_transporte_proveedor.descripcion)), selected: rM_form.transporteProveedorClave.ToString());
            return View(rM_form);
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


        /// <summary>
        /// Método que lee el contenido de un archivo xlsx
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult LeeArchivoXLSX(int numConcepto = 0)
        {
            var result = new object[1];

            int indexKey = Request.Files.AllKeys.ToList().IndexOf("archivo_xlsx");
            HttpPostedFileBase fileXLSX = Request.Files[indexKey];

            MemoryStream newStream = new MemoryStream();
            fileXLSX.InputStream.CopyTo(newStream);

            //Valida que se haya enviado un archivo
            if (fileXLSX.FileName == "")
            {
                result[0] = new { status = "WARNING", message = "Seleccione un archivo .xlsx" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //valida que sea un archivo en excel
            string extensionXML = Path.GetExtension(fileXLSX.FileName);

            if (extensionXML.ToUpper() != ".XLSX")
            {
                result[0] = new { status = "WARNING", message = "Sólo se permiten archivos con extensión .xlsx." };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            #region LeeXLSX

            string estatus = string.Empty;
            string msj = string.Empty;

            List<RM_elemento> listElementos = new List<RM_elemento>();

            try
            {
                listElementos = UtilExcel.LeeFormatoRemisionesProvisionales(fileXLSX, ref estatus, ref msj);
            }
            catch (Exception ex)
            {
                estatus = "ERROR";
                msj = "Ocurrió un error: " + ex.Message;
            }

            #endregion

            //si hubo algún error al leer el archivo
            if (estatus.ToUpper() == "ERROR")
            {
                result[0] = new { status = estatus, message = msj };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string html = string.Empty;
                //si el archivo se lee correctamente
                foreach (var item in listElementos)
                {
                    html += String.Format(@"
                                    <tr  id=""div_concepto_{0}"">
                                        <input type=""hidden"" name=""RM_elemento.Index"" id=""RM_elemento.Index"" value=""{0}"" />
                                        <input type=""hidden"" name=""RM_elemento.[{0}].activo"" value=""true"" />
                                            <td class=""input-contador-conceptos""></td>
                                            <td>
                                                <input style="" font-size: 12px;"" type=""text"" name=""RM_elemento[{0}].numeroParteCliente"" id=""RM_elemento[{0}].numeroParteCliente"" class=""form-control col-md-12"" value=""{1}"" autocomplete=""off"" maxlength=""50"" required>
                                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""RM_elemento[{0}].numeroParteCliente"" data-valmsg-replace=""true""></span>
                                            </td>
                                            <td>
                                                <input style="" font-size: 12px;"" type=""text"" name=""RM_elemento[{0}].numeroMaterial"" id=""RM_elemento[{0}].numeroMaterial"" class=""form-control col-md-12"" value=""{2}"" autocomplete=""off"" maxlength=""50"" required>
                                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""RM_elemento[{0}].numeroMaterial"" data-valmsg-replace=""true""></span>
                                            </td>
                                             <td>
                                                <input style="" font-size: 12px;"" type=""text"" name=""RM_elemento[{0}].numeroLote"" id=""RM_elemento[{0}].numeroLote"" class=""form-control col-md-12"" value=""{3}"" autocomplete=""off"" maxlength=""50"" required>
                                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""RM_elemento[{0}].numeroLote"" data-valmsg-replace=""true""></span>
                                            </td>
                                           <td>
                                                <input style="" font-size: 12px;"" type=""text"" name=""RM_elemento[{0}].numeroRollo"" id=""RM_elemento[{0}].numeroRollo"" class=""form-control col-md-12"" value=""{4}"" autocomplete=""off"" maxlength=""50"" required>
                                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""RM_elemento[{0}].numeroRollo"" data-valmsg-replace=""true""></span>
                                            </td>
                                           <td>
                                                <input style="" font-size: 12px;"" type=""text"" name=""RM_elemento[{0}].cantidad"" id=""RM_elemento[{0}].cantidad"" class=""form-control col-md-12 entero"" value=""{5}"" autocomplete=""off"">
                                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""RM_elemento[{0}].cantidad"" data-valmsg-replace=""true""></span>
                                            </td>
                                            <td>
                                                <input style="" font-size: 12px;"" type=""text"" name=""RM_elemento[{0}].peso"" id=""RM_elemento[{0}].peso"" class=""form-control col-md-12 decimal"" value=""{6}"" autocomplete=""off"">
                                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""RM_elemento[{0}].peso"" data-valmsg-replace=""true""></span>
                                            </td>

                                           <td>
                                                 <input type=""button"" value=""Borrar"" class=""btn btn-danger"" onclick=""borrarConcepto({0}); return false;"">
                                            </td>
                                     </tr>
                    ", numConcepto++, item.numeroParteCliente, item.numeroMaterial, item.numeroLote, item.numeroRollo, item.cantidad, item.peso);
                }

                //llama al método que lee la factura
                result[0] = new { status = estatus, html = html, num_conceptos = listElementos.Count, message = msj };
                var json = Json(result, JsonRequestBehavior.AllowGet);
                return json;
            }
        }

        /// <summary>
        /// Muestra el manual de usuario
        /// </summary>
        /// <returns></returns>
        public ActionResult FormatoEjemplo()
        {

            String ruta = System.Web.HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/formato_carga_remisiones.xlsx");

            //byte[] array = System.IO.File.ReadAllBytes(ruta);

            FileInfo archivo = new FileInfo(ruta);

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = archivo.Name,
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(ruta, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");


        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void CompararPropiedades(RM_cabecera original, RM_cabecera modificado, ref string texto)
        {
            List<string> cambios = new List<string>();

            //cambio de material
            if (original.retornaMaterial != modificado.retornaMaterial)
            {
                cambios.Add(String.Format("Cambio en <b>{0}</b>; original: <i>{1}</i>, cambio: <i>{2}</i>", "Retorna Material", original.retornaMaterial ? "Sí" : "No", modificado.retornaMaterial ? "Sí" : "No"));
                original.retornaMaterial = modificado.retornaMaterial;
            }
            //cambio de almacen
            if (original.almacenClave != modificado.almacenClave)
            {
                cambios.Add(String.Format("Cambio en <b>{0}</b>; original: <i>{1}</i>, cambio: <i>{2}</i>", "Almacén", original.RM_almacen.descripcion, db.RM_almacen.Find(modificado.almacenClave).descripcion));
                original.almacenClave = modificado.almacenClave;
            }


            //agrega el registro de cambios
            if (cambios.Count > 0)
            {
                if (texto.Length < 1140)
                    texto += "<code>Cambios Registrados</code><br/><ul>";
                foreach (var item in cambios)
                {
                    //lo agrega solo si el resultado es menor 1000 carácteres
                    if (texto.Length + item.Length <1180)
                        texto += string.Format("<li class='last'>{0}</li>", item);
                }
                texto += "</ul>";
            }

        }

    }
}
