﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class ProduccionRegistrosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: ProduccionRegistros
        public ActionResult Index(string planta, string linea, int pagina = 1)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro
          
                var produccion_registros = db.produccion_registros.Include(p => p.plantas).Include(p => p.produccion_lineas).Include(p => p.produccion_lotes).Include(p => p.produccion_operadores).Include(p => p.produccion_supervisores).Include(p => p.produccion_turnos).Where(x =>
                        x.activo == true
                        &&   (!String.IsNullOrEmpty(linea) && x.id_linea.ToString().Contains(linea))
                        && (!String.IsNullOrEmpty(planta) && x.clave_planta.ToString().Contains(planta))
                        )
                    .OrderByDescending(x => x.fecha)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.produccion_registros.Include(p => p.plantas).Include(p => p.produccion_lineas).Include(p => p.produccion_lotes).Include(p => p.produccion_operadores).Include(p => p.produccion_supervisores).Include(p => p.produccion_turnos).Where(x => 
                        x.activo==true
                        && (!String.IsNullOrEmpty(linea) && x.id_linea.ToString().Contains(linea))
                        && (!String.IsNullOrEmpty(planta) && x.clave_planta.ToString().Contains(planta))
                       ).Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["planta"] = planta;
                routeValues["linea"] = linea;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.linea = new SelectList(db.produccion_lineas.Where(p => p.activo == true), "id", "linea");
                ViewBag.planta = new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion");
                ViewBag.Paginacion = paginacion;

                return View(produccion_registros);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }            
        }

        // GET: ProduccionRegistros/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            produccion_registros produccion_registros = db.produccion_registros.Find(id);
            if (produccion_registros == null)
            {
                return HttpNotFound();
            }
            return View(produccion_registros);
        }

        // GET: ProduccionRegistros/Create
        public ActionResult Create(int? planta, int? linea)
        {

            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                //si no hay parámetros retorna al inxdex
                if (planta==null || linea==null) {
                    TempData["Mensaje"] = new MensajesSweetAlert("Verifique los valores de planta y línea.", TipoMensajesSweetAlerts.WARNING);
                    return RedirectToAction("Index");
                }

                //verifica si hay planta
                plantas plantas = db.plantas.Find(planta);
                if (plantas == null) {
                    TempData["Mensaje"] = new MensajesSweetAlert("No existe la planta.", TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Index");
                }

                //verifica si hay linea
                produccion_lineas lineas = db.produccion_lineas.Find(linea);
                if (lineas == null)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("No existe la línea de producción.", TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Index");
                }

                //obtiene el listado de turnos de la planata
                var turnos = db.produccion_turnos.Where(x => x.clave_planta == planta);

                
                //recorre los turnos para identificar que turno es
                produccion_turnos turno=null;
                foreach (produccion_turnos t in turnos){
                    if (TimeSpanUtil.CalculateDalUren(DateTime.Now,t.hora_inicio, t.hora_fin)){
                        turno = t;
                    }                    
                }

                //verifica si hay turno o si no manda mensaje de advertencia
                if (turno != null)
                    ViewBag.Turno = turno;
                else {
                    TempData["Mensaje"] = new MensajesSweetAlert("No hay un horario asignado para la hora actual.", TipoMensajesSweetAlerts.WARNING);
                    return RedirectToAction("Index");
                }
               
                ViewBag.Planta = plantas;
                ViewBag.Linea = lineas;
                ViewBag.sap_platina = ComboSelect.obtieneMaterial_BOM();
                ViewBag.sap_rollo = ComboSelect.obtieneRollo_BOM();
                ViewBag.id_supervisor = ComboSelect.obtieneSupervisoresPlanta(planta.Value);
                ViewBag.id_operador = ComboSelect.obtieneOperadorPorLinea(linea.Value);
                
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: ProduccionRegistros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,clave_planta,id_linea,id_operador,id_supervisor,id_turno,sap_platina,sap_rollo,fecha,activo")] produccion_registros produccion_registros, FormCollection collection)
        {

            //valores enviados previamente
            String c_sap_platina = String.Empty;
            if (!String.IsNullOrEmpty(collection["sap_platina"]))
                c_sap_platina = collection["sap_platina"].ToString();

            //valores enviados previamente
            String c_sap_rollo = String.Empty;
            if (!String.IsNullOrEmpty(collection["sap_rollo"]))
                c_sap_rollo = collection["sap_rollo"].ToString();

            //valores enviados previamente
            int c_id_supervisor = 0;
            if (!String.IsNullOrEmpty(collection["id_supervisor"]))
                Int32.TryParse(collection["id_supervisor"].ToString(), out c_id_supervisor);

            int c_id_operador = 0;
            if (!String.IsNullOrEmpty(collection["id_operador"]))
                Int32.TryParse(collection["id_operador"].ToString(), out c_id_operador);

            //si el modelo es válido
            if (ModelState.IsValid)
            {
                produccion_registros.activo = true;
                produccion_registros.fecha = DateTime.Now;
                db.produccion_registros.Add(produccion_registros);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                
                //retorna la vista de datos de entrada
                return RedirectToAction("DatosEntradas", new
                {
                   id = produccion_registros.id
                });
            }

            //si no es válido
            //en este punto ya se válido que existen en la BD
            plantas plantas = db.plantas.Find(produccion_registros.clave_planta);
            produccion_lineas lineas = db.produccion_lineas.Find(produccion_registros.id_linea.Value);
            var turnos = db.produccion_turnos.Where(x => x.clave_planta == produccion_registros.clave_planta.Value);
            produccion_turnos turno = null;

            foreach (produccion_turnos t in turnos)
            {
                if (TimeSpanUtil.CalculateDalUren(DateTime.Now, t.hora_inicio, t.hora_fin))
                {
                    turno = t;
                }
            }

            //verifica si hay turno o si no manda mensaje de advertencia
            if (turno != null)
                ViewBag.Turno = turno;
            else
            {
                TempData["Mensaje"] = new MensajesSweetAlert("No hay un horario asignado para la hora actual.", TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("Index");
            }

            ViewBag.Planta = plantas;
            ViewBag.Linea = lineas;
            ViewBag.sap_platina = ComboSelect.obtieneMaterial_BOM();
            ViewBag.sap_rollo = ComboSelect.obtieneRollo_BOM();
            ViewBag.id_supervisor = ComboSelect.obtieneSupervisoresPlanta(produccion_registros.clave_planta.Value);
            ViewBag.id_operador = ComboSelect.obtieneOperadorPorLinea(produccion_registros.id_linea.Value);
            //para completar_valores previos
            ViewBag.c_sap_platina = c_sap_platina;
            ViewBag.c_sap_rollo = c_sap_rollo;
            ViewBag.c_id_supervisor = c_id_supervisor;
            ViewBag.c_id_operador = c_id_operador;

           
            return View(produccion_registros);
        }

        // GET: ProduccionRegistros/DatosEntrada/5
        public ActionResult DatosEntradas(int? id)
        {
            //verifica los permisos del usuario
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REGISTRO))
            {
                //verifica si se envio un id
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                //busca si existe el registro de produccion
                produccion_registros produccion = db.produccion_registros.Find(id);
                
                if(produccion==null){
                    TempData["Mensaje"] = new MensajesSweetAlert("No existe el registro de producción.", TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Index");
                }

                //busca si hay datos de entrada para el registro de producción
                produccion_datos_entrada produccion_datos_entrada = db.produccion_datos_entrada.FirstOrDefault(x=> x.id_produccion_registro == id.Value);
                
                if (produccion_datos_entrada == null)
                {
                    //si no hay registro de entrada de datos crea uno nuevo con el id de registro de produccion
                    produccion_datos_entrada = new produccion_datos_entrada
                    {
                        id_produccion_registro =  id.Value,
                        produccion_registros = produccion
                    };    
                }

                //agrega datos entrada a la produccion
                produccion.produccion_datos_entrada = produccion_datos_entrada;


                //ENVIAR CLASS V3, SEGÚN EL MATERIAL produccion.sap_platina
                mm_v3 mm = db.mm_v3.FirstOrDefault(x => x.Material == produccion.sap_platina);
                if (mm == null)            
                    mm = new mm_v3 { };

                //ENVIAR cLASS SEGUN EL MATERIAL
              class_v3 class_ = db.class_v3.FirstOrDefault(x => x.Object == produccion.sap_platina);
                if (class_ == null)
                    class_ = new class_v3 { };

                
                ViewBag.MM = mm;
                ViewBag.Class = class_;
                return View(produccion);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
           
        }

       
        // POST: ProduccionRegistros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatosEntradas(produccion_registros produccion_registros)
        {
            

            bool error=false;

            foreach (produccion_lotes lote in produccion_registros.produccion_lotes)
            {
                if (lote.numero_lote_derecho == null && lote.numero_lote_izquierdo == null)
                    error = true;
            }

            if (error) {
                ModelState.AddModelError("", "Verifique que se haya especificado al menos un lote izquierdo o derecho para cada lote.");
            }else if (ModelState.IsValid)
            {
                //verifica que si existe en un registro en datos entrada
                produccion_datos_entrada datos_Entrada = db.produccion_datos_entrada.FirstOrDefault(x => x.id_produccion_registro == produccion_registros.id);

                if (datos_Entrada == null) //si no existe en BD crea la entrada
                {
                    db.produccion_datos_entrada.Add(produccion_registros.produccion_datos_entrada);
                    db.SaveChanges();
                }
                else {
                    //si existe lo modifica
                    // Activity already exist in database and modify it
                    db.Entry(datos_Entrada).CurrentValues.SetValues(produccion_registros.produccion_datos_entrada);
                    db.Entry(datos_Entrada).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //borra los lotes anteriores
                var listLotesAnteriores = db.produccion_lotes.Where(x => x.id_produccion_registro == produccion_registros.id);
                foreach (produccion_lotes lote in listLotesAnteriores)
                    db.produccion_lotes.Remove(lote);
                
                db.SaveChanges();
               

                //agrega los lotes nuevos
                foreach (produccion_lotes lote in produccion_registros.produccion_lotes)
                {                   
                    lote.id_produccion_registro = produccion_registros.id;
                    db.produccion_lotes.Add(lote);
                    db.SaveChanges();
                }


                TempData["Mensaje"] = new MensajesSweetAlert("Se ha actualizado el registro correctamente", TipoMensajesSweetAlerts.SUCCESS);

                //retorna la vista de datos de entrada
                return RedirectToAction("Index", new
                {
                    planta = produccion_registros.clave_planta,
                    linea = produccion_registros.id_linea
                });
            }

            //obtiene el resto de los elemento del objeto

            produccion_registros.produccion_supervisores = db.produccion_supervisores.Find(produccion_registros.id_supervisor);
            produccion_registros.produccion_operadores = db.produccion_operadores.Find(produccion_registros.id_operador);
            produccion_registros.produccion_lineas = db.produccion_lineas.Find(produccion_registros.id_linea);
            produccion_registros.produccion_turnos = db.produccion_turnos.Find(produccion_registros.id_turno);

            //ENVIAR CLASS V3, SEGÚN EL MATERIAL produccion.sap_platina
            mm_v3 mm = db.mm_v3.FirstOrDefault(x => x.Material == produccion_registros.sap_platina);
            if (mm == null)
                mm = new mm_v3 { };

            //ENVIAR cLASS SEGUN EL MATERIAL
            class_v3 class_ = db.class_v3.FirstOrDefault(x => x.Object == produccion_registros.sap_platina);
            if (class_ == null)
                class_ = new class_v3 { };

            ViewBag.MM = mm;
            ViewBag.Class = class_;
            return View(produccion_registros);
        }

        // GET: ProduccionRegistros/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            produccion_registros produccion_registros = db.produccion_registros.Find(id);
            if (produccion_registros == null)
            {
                return HttpNotFound();
            }
            return View(produccion_registros);
        }

        // POST: ProduccionRegistros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            produccion_registros produccion_registros = db.produccion_registros.Find(id);
            db.produccion_registros.Remove(produccion_registros);
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

        ///<summary>
        ///Retorna el peso de la báscula
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        ///
        
        public JsonResult obtienePesoBascula(string ip = "")
        {
            byte[] msg = Encoding.UTF8.GetBytes("Portal");
            byte[] bytes = new byte[256];
            var list = new object[1];
            string patron = @"(?:- *)?\d+(?:\.\d+)?";

            //conecta con la báscula
            try
            {
                Socket miPrimerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);         
                IPEndPoint direccion = new IPEndPoint(IPAddress.Parse(ip), 1702);

                try
                {
                    miPrimerSocket.Connect(direccion);
                    miPrimerSocket.ReceiveTimeout = 16000;

                    // Blocks until send returns.
                    int byteCount = miPrimerSocket.Send(msg, 0, msg.Length, SocketFlags.None);
                   
                    byteCount = miPrimerSocket.Receive(bytes, 0, 20,
                                               SocketFlags.None);


                    if (byteCount > 0)
                    {
                        Regex regex = new Regex(patron);
                        string respuesta = Encoding.UTF8.GetString(bytes);

                        foreach (Match m in regex.Matches(respuesta))
                        {
                             list[0] = new { Message = "OK", Peso = m.Value };
                        }
                       
                    }              

                    miPrimerSocket.Close();

                }
                catch (ArgumentNullException ane)
                {
                    list[0] = new { Message = "Error: " + ane.Message };
                }
                catch (SocketException se)
                {
                    list[0] = new { Message = "Error: " + se.Message };
                }
                catch (Exception e)
                {
                    list[0] = new { Message = "Error: " + e.Message };
                }

            }
            catch (Exception e)
            {
                list[0] = new { Message = "Error: "+e.Message };
            }



           
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        
    }
}
