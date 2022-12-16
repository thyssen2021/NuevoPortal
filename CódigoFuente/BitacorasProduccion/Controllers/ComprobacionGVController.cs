using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class ComprobacionGVController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();


        // GET: ComprobacionGV    
        public ActionResult Solicitudes(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            //en caso de solicitudes pendientes manda a otra vista
            if (estatus == "PENDIENTES")
                return RedirectToAction("Pendientes", new { estatus = estatus });


            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();

            var listado = db.GV_comprobacion
                .Where(x => (x.GV_solicitud.id_empleado == empleado.id || x.GV_solicitud.id_solicitante == empleado.id)
                    && (x.estatus == estatus || estatus == "ALL"
                                    || (estatus == "EN_PROCESO" && (x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                    ))
                                    || (estatus == "RECHAZADAS" && (x.estatus == GV_solicitud_estatus.RECHAZADO_JEFE
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_CONTROLLING
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_NOMINA || x.estatus == GV_solicitud_estatus.RECHAZADO_CONTABILIDAD))
                                                    )
                )
                .OrderByDescending(x => x.GV_solicitud.fecha_solicitud)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_comprobacion
                  .Where(x => (x.GV_solicitud.id_empleado == empleado.id || x.GV_solicitud.id_solicitante == empleado.id)
                    && (x.estatus == estatus || estatus == "ALL"
                                    || (estatus == "EN_PROCESO" && (x.estatus == GV_solicitud_estatus.ENVIADO_CONTROLLING
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.ENVIADO_NOMINA
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_USUARIO
                                                    || x.estatus == GV_solicitud_estatus.CONFIRMADO_CONTABILIDAD
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_USUARIO
                                                    ))
                                    || (estatus == "RECHAZADAS" && (x.estatus == GV_solicitud_estatus.RECHAZADO_JEFE
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_CONTROLLING
                                                    || x.estatus == GV_solicitud_estatus.RECHAZADO_NOMINA || x.estatus == GV_solicitud_estatus.RECHAZADO_CONTABILIDAD))
                                                    )
                )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual


            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = "Pendientes", Value = "PENDIENTES" });
            newList.Add(new SelectListItem() { Text = "En proceso", Value = "EN_PROCESO" });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = "RECHAZADAS" });
            newList.Add(new SelectListItem() { Text = "Finalizadas", Value = Bitacoras.Util.GV_comprobacion_estatus.FINALIZADO });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;
            ViewBag.SegundoNivel = "GV_solicitud";
            ViewBag.Title = "Mis Solicitudes";


            //ordenar por fecha de solicitud más reciente
            return View("solicitudes", listado);

        }

        public ActionResult Pendientes(string estatus = "", int pagina = 1)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            //en caso de solicitudes pendientes manda a otra vista
            if (estatus != "PENDIENTES")
                return RedirectToAction("Solicitudes", new { estatus = estatus });

            var cantidadRegistrosPorPagina = 20; // parámetro

            var empleado = obtieneEmpleadoLogeado();

            var listado = db.GV_solicitud
                .Where(x => (x.id_empleado == empleado.id || x.id_solicitante == empleado.id)
                    && x.estatus == GV_solicitud_estatus.FINALIZADO && x.GV_comprobacion == null
                )
                .OrderByDescending(x => x.fecha_solicitud)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.GV_solicitud
                   .Where(x => (x.id_empleado == empleado.id || x.id_solicitante == empleado.id)
                    && x.estatus == GV_solicitud_estatus.FINALIZADO && x.GV_comprobacion == null
                )
                .Count();

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["estatus"] = estatus;
            routeValues["action_result"] = System.Reflection.MethodBase.GetCurrentMethod().Name.ToUpper();  //obtiene el nombre del metodo actual


            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            //crea un Select  list para el estatus
            List<SelectListItem> newList = new List<SelectListItem>();

            //Agrega el estatus al selectListItem
            newList.Add(new SelectListItem() { Text = "Pendientes", Value = "PENDIENTES" });
            newList.Add(new SelectListItem() { Text = "En proceso", Value = "EN_PROCESO" });
            newList.Add(new SelectListItem() { Text = "Rechazadas", Value = "RECHAZADAS" });
            newList.Add(new SelectListItem() { Text = "Finalizadas", Value = Bitacoras.Util.GV_comprobacion_estatus.FINALIZADO });

            SelectList selectListItemsStatus = new SelectList(newList, "Value", "Text", estatus);
            ViewBag.estatus = AddFirstItem(selectListItemsStatus, textoPorDefecto: "-- Seleccionar --");
            ViewBag.Paginacion = paginacion;


            //ordenar por fecha de solicitud más reciente
            return View("pendientes", listado);

        }

        // GET: ComprobacionGV/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ComprobacionGV/Details/5
        public ActionResult ComprobacionGastos(int? id)
        {
            if (!TieneRol(TipoRoles.GV_SOLICITUD)) //Todos los usuarios van a tener permiso para crear solicitudes de GV, Por lo que solo se necesita validar este permiso
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GV_solicitud gV_solicitud = db.GV_solicitud.Find(id);
            if (gV_solicitud == null)
            {
                return HttpNotFound();
            }

            //determina si aplica otro o no
            gV_solicitud.medio_transporte_aplica_otro = !gV_solicitud.id_medio_transporte.HasValue;
            ViewBag.plantaClave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccione un valor --");
            ViewBag.id_centro_costo = AddFirstItem(new SelectList(db.GV_centros_costo.Where(p => p.activo == true), nameof(GV_centros_costo.id), nameof(GV_centros_costo.ConcatCentroDepto)), textoPorDefecto: "-- Seleccione un valor --");

            return View(gV_solicitud); ;
        }

        // POST: OrdenesTrabajo/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ComprobacionGastos(GV_solicitud solicitud)
        {

            // ModelState.AddModelError("", "Error prueba.");
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha creado el registro correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Solicitudes", new { estatus = "EN_PROCESO" });
            }

            //En caso de que el modelo no sea válido
            //guarda de forma temporal la comprobacion recibida en el formulario y lo asigna el modelo completo
            GV_comprobacion gV_ComprobacionRecibida = solicitud.GV_comprobacion;
            solicitud = db.GV_solicitud.Find(solicitud.id);
            solicitud.GV_comprobacion = gV_ComprobacionRecibida;

            //determina si aplica otro o no
            solicitud.medio_transporte_aplica_otro = !solicitud.id_medio_transporte.HasValue;

            ViewBag.plantaClave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccione un valor --", selected: gV_ComprobacionRecibida.clave_planta.ToString());
            ViewBag.id_centro_costo = AddFirstItem(new SelectList(db.GV_centros_costo.Where(p => p.activo == true && p.clave_planta == solicitud.GV_comprobacion.clave_planta), nameof(GV_centros_costo.id), nameof(GV_centros_costo.ConcatCentroDepto)), textoPorDefecto: "-- Seleccione un valor --", selected: gV_ComprobacionRecibida.id_centro_costo.ToString());

            return View(solicitud);
        }


        // GET: ComprobacionGV/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ComprobacionGV/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ComprobacionGV/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ComprobacionGV/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ComprobacionGV/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ComprobacionGV/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Método que lee el contenido de una factura xml
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult ObtieneConceptos()
        {
            var result = new object[1];

            HttpPostedFileBase file = Request.Files[0];

            MemoryStream newStream = new MemoryStream();
            file.InputStream.CopyTo(newStream);

            //Valida que se haya enviado un archivo
            if (file.FileName == "")
            {
                result[0] = new { status = "ERROR", value = "Seleccione un archivo." };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //valida que sea un archivo en excel
            string extension = Path.GetExtension(file.FileName);
            if (extension.ToUpper() != ".XML")
            {
                result[0] = new { status = "ERROR", value = "Sólo se permiten archivos con extensión .xml." };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


            #region LeeXML

            string estatus = string.Empty;
            string msj = string.Empty;

            Comprobante factura_3_3 = null;
            Bitacoras.CFDI_4_0.Comprobante factura_4_0 = null;
            //primero trata de leer la factura 3.3
            try
            {
                file.InputStream.Position = 0;
                factura_3_3 = XmlUtil.LeeXML_3_3(file.InputStream);
            }
            catch (Exception e)
            {
                //si falla al leer la 3.3, trata de leer la 4.0
                try
                {
                    newStream.Position = 0;
                    factura_4_0 = XmlUtil.LeeXML_4_0(newStream);
                }
                catch (Exception ex)
                {
                    estatus = "ERROR";
                    msj = "Ocurrió un error al leer la estructura del XML: " + ex.Message;
                }

            }

            #endregion            



            //si hubo algún error al leer el archivo
            if (estatus.ToUpper() == "ERROR")
            {
                result[0] = new { status = estatus, value = msj };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            { //si el archivo se lee correctamente

                string body = string.Empty;
                string div_ocultos = string.Empty;

                if (factura_3_3 != null)
                {
                    estatus = "OK";
                    body = "Factura Versión: " + factura_3_3.Version;
                }
                else if (factura_4_0 != null)
                {
                    estatus = "OK";
                    body = "Factura Versión: " + factura_4_0.Version;
                }

                /*
                 * enviar el contenido del body de la tabla
                 * Aumenta la longitud máxima por defecto del serializador
                 */

                result[0] = new { status = estatus, value = body, div_ocultos = div_ocultos };
                var json = Json(result, JsonRequestBehavior.AllowGet);
                return json;
            }
        }
        /// <summary>
        /// Método que busca y lee el contenido de una factura xml desde cofidi
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult BuscaCOFIDI(string uuid_field)
        {
            var result = new object[1];

            string estatus = string.Empty;
            string msj = string.Empty;
            Comprobante factura_3_3 = null;
            Bitacoras.CFDI_4_0.Comprobante factura_4_0 = null;

            CFDProveedor factDB = null;

            //valida uuid
            if (String.IsNullOrEmpty(uuid_field))
            {
                estatus = "ERROR";
                msj = "El campo UUID es requerido.";
            }
            else if (uuid_field.Contains("_"))
            {
                estatus = "ERROR";
                msj = "El campo UUID no es válido. Favor de verificarlo.";
            }
            else //la longitud es valida
            {
                //busca en cofidi
                string xmlText = String.Empty;
                using (var db = new ATEBCOFIDIEntities())
                {
                    factDB = db.CFDProveedor.Where(x => x.UUID == uuid_field).FirstOrDefault();

                    if (factDB == null)
                    {
                        estatus = "404";
                        msj = "No se encontró una factura asociada.";
                    }
                    else
                    { //si se encontro el uuid
                      //primero trata de leer la factura 3.3
                      // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(factDB.CFDOriginal);
                        //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
                        MemoryStream stream = new MemoryStream(byteArray);
                        MemoryStream newStream = new MemoryStream();
                        stream.CopyTo(newStream);

                        try
                        {
                            stream.Position = 0;
                            factura_3_3 = XmlUtil.LeeXML_3_3(stream);
                        }
                        catch (Exception e)
                        {
                            //si falla al leer la 3.3, trata de leer la 4.0
                            try
                            {
                                newStream.Position = 0;
                                factura_4_0 = XmlUtil.LeeXML_4_0(newStream);
                            }
                            catch (Exception ex)
                            {
                                estatus = "ERROR";
                                msj = "Ocurrió un error al leer la estructura del XML: " + ex.Message;
                            }

                        }

                    }
                }
            }

            //si hubo algún error al leer el archivo
            if (estatus.ToUpper() == "ERROR" || estatus.ToUpper() == "404")
            {
                result[0] = new { status = estatus, value = msj };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            { //si el archivo se lee correctamente

                string body = string.Empty;
                string div_ocultos = string.Empty;


                if (factura_3_3 != null)
                {
                    estatus = "OK";
                    body = "Factura Versión: " + factura_3_3.Version +" emisor: "+factura_3_3.Emisor.Rfc;
                }
                else if (factura_4_0 != null)
                {
                    estatus = "OK";
                    body = "Factura Versión: " + factura_4_0.Version + " emisor: " + factura_4_0.Emisor.Rfc;
                }

                result[0] = new { status = estatus, value = body, div_ocultos = div_ocultos };
                var json = Json(result, JsonRequestBehavior.AllowGet);
                return json;
            }

        }
    }
}
