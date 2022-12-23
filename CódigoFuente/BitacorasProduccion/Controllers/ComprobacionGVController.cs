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
            //ViewBag.plantaClave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccione un valor --");
            //ViewBag.id_centro_costo = AddFirstItem(new SelectList(db.GV_centros_costo.Where(p => p.activo == true), nameof(GV_centros_costo.id), nameof(GV_centros_costo.ConcatCentroDepto)), textoPorDefecto: "-- Seleccione un valor --");

            return View(gV_solicitud); ;
        }

        // POST: OrdenesTrabajo/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ComprobacionGastos(GV_solicitud solicitud, FormCollection form)
        {

            if (solicitud.GV_comprobacion_rel_gastos.Count == 0)
                ModelState.AddModelError("", "No se agregaron conceptos a la comprobación de gastos.");

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

            //ViewBag.plantaClave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion"), textoPorDefecto: "-- Seleccione un valor --", selected: gV_ComprobacionRecibida.clave_planta.ToString());
            //ViewBag.id_centro_costo = AddFirstItem(new SelectList(db.GV_centros_costo.Where(p => p.activo == true && p.clave_planta == solicitud.GV_comprobacion.clave_planta), nameof(GV_centros_costo.id), nameof(GV_centros_costo.ConcatCentroDepto)), textoPorDefecto: "-- Seleccione un valor --", selected: gV_ComprobacionRecibida.id_centro_costo.ToString());

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
        public JsonResult BuscaCOFIDI(string uuid_field, int numConcepto = 0)
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
                int cantidadConceptos = 0;

                if (factura_3_3 != null)
                {
                    List<GV_comprobacion_tipo_gastos_viaje> listaCuentas = db.GV_comprobacion_tipo_gastos_viaje.Where(x => x.activo).ToList();
                    List<GV_comprobacion_tipo_pago> listaTipoPago = db.GV_comprobacion_tipo_pago.Where(x => x.activo).ToList();
                    List<GV_centros_costo> listaCentroCosto = db.GV_centros_costo.Where(x => x.activo).OrderBy(x => x.plantas.clave).ToList();

                    estatus = "OK";

                    //obtiene el valor de los impuestos locales trasladados de la factura
                    decimal impuestosLocales = 0;
                    if (factura_3_3.ImpuestosLocales != null)
                        impuestosLocales = factura_3_3.ImpuestosLocales.TotaldeTraslados;

                    //lee CABECERA de la factura
                    body = String.Format(@"<tr style=""background-color:#FFEB9C"" class=""div_" + factura_3_3.TimbreFiscalDigital.UUID + @""">
                                                <input type=""hidden"" name=""GV_comprobacion_rel_gastos.Index"" id=""GV_comprobacion_rel_gastos.Index"" value=""" + numConcepto + @""" />
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" value=""" + Bitacoras.Util.GV_comprobacion_origen.COFIDI_RESUMEN + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" value=""" + factura_3_3.SubTotal + @""">                                                
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" value=""" + factura_3_3.GetTotalIVAImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" value=""" + factura_3_3.GetTotalISRImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" value=""" + factura_3_3.GetTotalIEPSImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" value=""" + factura_3_3.Impuestos.TotalImpuestosRetenidos + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].impuestos_locales"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].impuestos_locales"" value=""" + impuestosLocales + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" value=""" + factura_3_3.Total + @""">
                                                <td class=""input-contador-conceptos""></td> 
                                                <td><b>Fecha:</b> {0}</td> 
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].fecha_factura"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].fecha_factura"" value=""" + factura_3_3.Fecha + @""">
                                                <td colspan=""2"" nowrap><b>UUID:</b> <custom-div class=""class-uuid"">{1}</custom-div></td> 
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" value=""" + factura_3_3.TimbreFiscalDigital.UUID + @""">
                                                <td colspan=""1""> <b>Tipo de Cambio:</b> {2}</td> 
                                                <td colspan=""2""> <b>Moneda:</b> {3}</td>                                                
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].currency_iso"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].currency_iso"" value=""" + factura_3_3.Moneda + @""">
                                                <td colspan=""8""></td>                                               
                                                <td>{4}</td>
                                                <td>{5}</td>
                                                <td></td>
                                                <td rowspan=""" + (factura_3_3.Conceptos.Count() + 1) + @""">
                                                    <textarea style=""font-size:12px;"" type=""text"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].comentario"" id=""PM_conceptos[@contador].concepto"" class=""form-control col-md-12"" autocomplete=""off"" maxlength=""150"" rows=""" + (int)(3 + (factura_3_3.Conceptos.Count() * 2.2)) + @"""></textarea>
                                                    <span class=""field-validation-valid text-danger"" data-valmsg-for=""GV_comprobacion_rel_gastos[" + numConcepto + @"].comentario"" data-valmsg-replace=""true""></span>
                                                </td>
                                            </tr>
                    ", factura_3_3.Fecha.ToShortDateString(), factura_3_3.TimbreFiscalDigital.UUID
                     , factura_3_3.TipoCambio == 0 ? 1 : factura_3_3.TipoCambio, factura_3_3.Moneda
                    , @"<a href=""/Combos/MuestraArchivo/?uuid=" + factura_3_3.TimbreFiscalDigital.UUID + @""" class=""btn btn-info"" title=""Ver PDF"" target=""_blank""><i class=""fa-solid fa-file-pdf""></i></a>"
                    , @"<input type=""button"" value=""Borrar"" class=""btn btn-danger"" onclick=""borrarConcepto('" + factura_3_3.TimbreFiscalDigital.UUID + @"'); return false; "">"
                     );

                    //aumenta en 1 el número de concepto (debido a la cabecera)
                    numConcepto++;

                    #region selects
                    //crea el select para el tipo de cuenta
                    string selectCuenta = @"<select name = 'GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_gastos_viaje' id = 'GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_gastos_viaje' class=""form-control select2bs4"" style=""width:100%"" required>
                                                <option value = '' > --Seleccione un valor --</option>";

                    foreach (var cuenta in listaCuentas)
                        selectCuenta += @"<option value = """ + cuenta.id + @""">" + cuenta.ConcatCuenta + @"</option>";

                    selectCuenta += @"</select><span class=""field-validation-valid text-danger"" data-valmsg-for=GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_gastos_viaje data-valmsg-replace=""true""></span> ";

                    //crea el select para el tipo de pago
                    string selectTipoPago = @"<select name = 'GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_pago' id = 'GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_pago' class=""form-control select2bs4"" style=""width:100%"" required>
                                                <option value = '' > --Seleccione un valor --</option>";

                    foreach (var tipo in listaTipoPago)
                        selectTipoPago += @"<option value = """ + tipo.id + @""">" + tipo.descripcion + @"</option>";

                    selectTipoPago += @"</select><span class=""field-validation-valid text-danger"" data-valmsg-for=GV_comprobacion_rel_gastos[#ID].id_comprobacion_tipo_pago data-valmsg-replace=""true""></span> ";

                    //crea el select para el centro d costo
                    string selectCentroCosto = @"<select name = 'GV_comprobacion_rel_gastos[#ID].id_centro_costo' id = 'GV_comprobacion_rel_gastos[#ID].id_centro_costo' class=""form-control select2bs4"" style=""width:100%"" required>
                                                <option value = '' > --Seleccione un valor --</option>";

                    foreach (var planta in listaCentroCosto.Select(x => x.plantas).Distinct())
                    {
                        selectCentroCosto += @"<optgroup label= """ + planta.descripcion + @""">";
                        foreach (var cc in listaCentroCosto.Where(x => x.clave_planta == planta.clave))
                            selectCentroCosto += @"<option value = """ + cc.id + @""">" + cc.ConcatCentroDeptoPlanta + @"</option>";
                        selectCentroCosto += "</optgroup>";
                    }
                    selectCentroCosto += @"</select><span class=""field-validation-valid text-danger"" data-valmsg-for=GV_comprobacion_rel_gastos[#ID].id_centro_costo data-valmsg-replace=""true""></span> ";

                    #endregion

                    //agrega info para cada uno de los conceptos
                    foreach (var concepto in factura_3_3.Conceptos)
                    {
                        body += String.Format(@"<tr style=""background-color:#C9DCC1"" class=""div_" + factura_3_3.TimbreFiscalDigital.UUID + @""">   
                                                <input type=""hidden"" name=""GV_comprobacion_rel_gastos.Index"" id=""GV_comprobacion_rel_gastos.Index"" value=""" + numConcepto + @""" />
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].concepto_tipo"" value=""" + GV_comprobacion_origen.COFIDI_CONCEPTO + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].uuid"" value=""" + factura_3_3.TimbreFiscalDigital.UUID + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].descripcion"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].descripcion"" value=""" + UsoStrings.RecortaString(concepto.Descripcion, 350) + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].cantidad"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].cantidad"" value=""" + concepto.Cantidad + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].precio_unitario"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].precio_unitario"" value=""" + concepto.ValorUnitario + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].importe"" value=""" + concepto.Importe + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_porcentaje"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_porcentaje"" value=""" + concepto.GetIVATasa() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].iva_total"" value=""" + concepto.GetIVAImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_porcentaje"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_porcentaje"" value=""" + concepto.GetISRTasa() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].isr_total"" value=""" + concepto.GetISRImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_porcentaje"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_porcentaje"" value=""" + concepto.GetIEPSTasa() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].ieps_total"" value=""" + concepto.GetIEPSImporte() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_retenciones"" value=""" + concepto.GetTotalRetenciones() + @""">
                                                <input type=""hidden"" id=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" name=""GV_comprobacion_rel_gastos[" + numConcepto + @"].total_mxn"" value=""" + concepto.GetTotalImporteConTransladosyRetenciones() + @""">
                                                <td colspan=""1""></td>
                                                <td nowrap>{12}</td>
                                                <td nowrap>{13}</td>
                                                <td>{0}</td> 
                                                <td>{14}</td> 
                                                <td>{1}</td> 
                                                <td nowrap>{2}</td> 
                                                <td nowrap>{3}</td> 
                                                <td nowrap>{4}</td> 
                                                <td nowrap>{5}</td> 
                                                <td nowrap>{6}</td> 
                                                <td nowrap>{7}</td> 
                                                <td nowrap>{8}</td> 
                                                <td nowrap>{9}</td>   
                                                <td nowrap style=""color:#C10000"">{10}</td> 
                                                <td></td>
                                                <td nowrap>{11}</td> 
                                                <td colspan=""1""></td>
                                            </tr>
                            ", UsoStrings.RecortaString(concepto.Descripcion, 350), concepto.Cantidad.ToString("0.00"), concepto.ValorUnitario.ToString("$ 0.00"), concepto.Importe.ToString("$ 0.00")
                            , concepto.GetIVATasa() == 0 ? "--" : (concepto.GetIVATasa()).ToString("0.00 %"), concepto.GetIVAImporte() == 0 ? "--" : concepto.GetIVAImporte().ToString("$ 0.00")
                            , concepto.GetISRTasa() == 0 ? "--" : (concepto.GetISRTasa()).ToString("0.00 %"), concepto.GetISRImporte() == 0 ? "--" : concepto.GetISRImporte().ToString("$ 0.00")
                            , concepto.GetIEPSTasa() == 0 ? "--" : (concepto.GetIEPSTasa()).ToString("0.00 %"), concepto.GetIEPSImporte() == 0 ? "--" : concepto.GetIEPSImporte().ToString("$ 0.00")
                            //, concepto.GetTotalImpuestos() == 0 ? "--" : concepto.GetTotalImpuestos().ToString("$ 0.00")
                            , concepto.GetTotalRetenciones() == 0 ? "--" : concepto.GetTotalRetenciones().ToString("$ 0.00")
                            , concepto.GetTotalImporteConTransladosyRetenciones().ToString("$ 0.00")
                            , selectCuenta.Replace("#ID", numConcepto.ToString())
                            , selectCentroCosto.Replace("#ID", numConcepto.ToString())
                            , selectTipoPago.Replace("#ID", numConcepto.ToString())
                            );

                        //aumenta en 1 el número de concepto (por cada concepto dentro de la factura)
                        numConcepto++;
                    }

                    //agrega los TOTALES de la factura
                    body += String.Format(@"<tr style=""background-color:#AAE0FF"" class=""div_" + factura_3_3.TimbreFiscalDigital.UUID + @""">
                                                <td></td> 
                                                <td colspan=""6""><b>Totales Factura:</b></td> 
                                                <td nowrap>{0}</td> 
                                                <td></td> 
                                                <td nowrap>{1}</td> 
                                                <td></td> 
                                                <td nowrap>{2}</td> 
                                                <td></td> 
                                                <td nowrap>{3}</td>      
                                                <td nowrap style=""color:#C10000"">{4}</td> 
                                                <td nowrap>{5}</td>
                                                <td nowrap >{6}</td> 
                                                <td nowrap>{7}</td>
                                                <td colspan=""1""></td>
                                            </tr>
                    "
                    , "$ " + factura_3_3.SubTotal
                    , factura_3_3.GetTotalIVAImporte() == 0 ? "--" : factura_3_3.GetTotalIVAImporte().ToString("$ 0.00")
                    , factura_3_3.GetTotalISRImporte() == 0 ? "--" : factura_3_3.GetTotalISRImporte().ToString("$ 0.00")
                    , factura_3_3.GetTotalIEPSImporte() == 0 ? "--" : factura_3_3.GetTotalIEPSImporte().ToString("$ 0.00")
                    //, factura_3_3.Impuestos.TotalImpuestosTrasladados == 0 ? "--" : factura_3_3.Impuestos.TotalImpuestosTrasladados.ToString("$ 0.00")
                    , factura_3_3.Impuestos.TotalImpuestosRetenidos == 0 ? "--" : factura_3_3.Impuestos.TotalImpuestosRetenidos.ToString("$ 0.00")
                    , factura_3_3.ImpuestosLocales == null ? "--" : factura_3_3.ImpuestosLocales.TotaldeTraslados.ToString("$ 0.00")
                    , factura_3_3.Total.ToString("$ 0.00")
                    , factura_3_3.Total.ToString("$ 0.00")
                    );

                    //indica el número de conceptos de la factura + la cabecera
                    cantidadConceptos = factura_3_3.Conceptos.Count() + 1;
                }
                else if (factura_4_0 != null)
                {

                    estatus = "OK";
                    body = "Factura Versión: " + factura_4_0.Version + " emisor: " + factura_4_0.Emisor.Rfc;
                }

                result[0] = new { status = estatus, value = body, num_conceptos = cantidadConceptos, div_ocultos = div_ocultos };
                var json = Json(result, JsonRequestBehavior.AllowGet);
                return json;
            }

        }
    }
}
