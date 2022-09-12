using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using IdentitySample.Models;
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
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REGISTRO) || TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                //obtiene el empleado que inicio sesión
                empleados emp = obtieneEmpleadoLogeado();

                //obtiene los id lineas a las que esta asignado
                List<int> idLineas = db.produccion_operadores.Where(x => x.id_empleado == emp.id).Select(x => x.id_linea).ToList();
                //obtiene el lsitado de ids de operadores
                //List<int> idOperador = db.produccion_operadores.Where(x => x.id_empleado == emp.id ).Select(x => x.id).ToList();

                //verifica si es supervisor
                //bool esOperador = db.produccion_supervisores.Where(x => x.id_empleado == emp.id).Select(x => x.id).ToList().Count > 0 ? true: false;

                var cantidadRegistrosPorPagina = 20; // parámetro

                //muestra unicamente lso registros que el usuario edite
                var produccion_registros = db.produccion_registros.Include(p => p.plantas).Include(p => p.produccion_lineas).Include(p => p.produccion_lotes).Include(p => p.produccion_operadores).Include(p => p.produccion_supervisores).Include(p => p.produccion_turnos).Where(x =>
                          //x.activo == true && 
                          (!String.IsNullOrEmpty(linea) && x.id_linea.ToString().Contains(linea))
                        && (!String.IsNullOrEmpty(planta) && x.clave_planta.ToString().Contains(planta))
                        //     && (idOperador.Contains(x.id_operador.Value) || esOperador)
                        )
                    .OrderByDescending(x => x.fecha)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.produccion_registros.Include(p => p.plantas).Include(p => p.produccion_lineas).Include(p => p.produccion_lotes).Include(p => p.produccion_operadores).Include(p => p.produccion_supervisores).Include(p => p.produccion_turnos).Where(x =>
                         // x.activo==true &&
                         (!String.IsNullOrEmpty(linea) && x.id_linea.ToString().Contains(linea))
                        && (!String.IsNullOrEmpty(planta) && x.clave_planta.ToString().Contains(planta))
                       //       && (idOperador.Contains(x.id_operador.Value) || esOperador)
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



                //si no tiene el rol de produccion muestra todas las lineas
                if (!TieneRol(TipoRoles.BITACORAS_PRODUCCION_REGISTRO))
                    ViewBag.linea = new SelectList(db.produccion_lineas.Where(p => p.activo == true && p.clave_planta == emp.planta_clave), "id", "linea");
                else
                    //obtiene unicamente las lineas a las que está asignado y a la planta correspondiente
                    ViewBag.linea = new SelectList(db.produccion_lineas.Where(p => p.activo == true && idLineas.Contains(p.id) && p.clave_planta == emp.planta_clave), "id", "linea");



                ViewBag.planta = new SelectList(db.plantas.Where(p => p.activo == true && emp.planta_clave == p.clave), "clave", "descripcion");
                ViewBag.Paginacion = paginacion;

                return View(produccion_registros);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }



        // GET: ProduccionRegistros/Create
        public ActionResult Create(int? planta, int? linea)
        {

            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REGISTRO))
            {
                //si no hay parámetros retorna al inxdex
                if (planta == null || linea == null)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Verifique los valores de planta y línea.", TipoMensajesSweetAlerts.WARNING);
                    return RedirectToAction("Index");
                }

                //verifica si hay planta
                plantas plantas = db.plantas.Find(planta);
                if (plantas == null)
                {
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

                //obtiene el empleado que inicio sesión
                empleados emp = obtieneEmpleadoLogeado();

                ViewBag.Planta = plantas;
                ViewBag.Linea = lineas;

                //crea un select li
                List<SelectListItem> listadoPlatinas = ComboSelect.obtieneMaterial_BOM();
                List<SelectListItem> listadoRollos = ComboSelect.obtieneRollo_BOM();

                ViewBag.sap_platina = AddFirstItem(new SelectList(listadoPlatinas, "Value", "Text"), textoPorDefecto: "-- Seleccionar --");
                ViewBag.sap_platina_2 = AddFirstItem(new SelectList(listadoPlatinas, "Value", "Text"), textoPorDefecto: "-- Seleccionar --");
                ViewBag.sap_rollo = new SelectList(ComboSelect.obtieneRollo_BOM(), "Value", "Text");
                ViewBag.id_supervisor = AddFirstItem(new SelectList(db.produccion_supervisores.Where(p => p.activo == true && p.clave_planta == planta.Value),
                    nameof(produccion_supervisores.id), "empleados." + nameof(produccion_supervisores.empleados.ConcatNombre)),
                    textoPorDefecto: "-- Seleccionar --");
                ViewBag.id_operador = ComboSelect.obtieneOperadorPorLinea(emp, linea.Value);

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
        public ActionResult Create(produccion_registros produccion_registros, FormCollection collection)
        {

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

            empleados emp = obtieneEmpleadoLogeado();

            ViewBag.Planta = plantas;
            ViewBag.Linea = lineas;

            //crea un select li
            List<SelectListItem> listadoPlatinas = ComboSelect.obtieneMaterial_BOM();
            List<SelectListItem> listadoRollos = ComboSelect.obtieneRollo_BOM(produccion_registros.sap_platina);

            ViewBag.sap_platina = AddFirstItem(new SelectList(listadoPlatinas, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: produccion_registros.sap_platina);
            ViewBag.sap_platina_2 = AddFirstItem(new SelectList(listadoPlatinas, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: produccion_registros.sap_platina_2);
            ViewBag.sap_rollo = new SelectList(ComboSelect.obtieneRollo_BOM(produccion_registros.sap_platina), "Value", "Text", selectedValue: produccion_registros.sap_rollo);
            ViewBag.id_supervisor = AddFirstItem(new SelectList(db.produccion_supervisores.Where(p => p.activo == true && p.clave_planta == produccion_registros.clave_planta),
                nameof(produccion_supervisores.id), "empleados." + nameof(produccion_supervisores.empleados.ConcatNombre)),
                textoPorDefecto: "-- Seleccionar --", selected: produccion_registros.id_supervisor.ToString());
            ViewBag.id_operador = ComboSelect.obtieneOperadorPorLinea(emp, produccion_registros.id_linea.Value);

            return View(produccion_registros);
        }

        // GET: ProduccionRegistros/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REGISTRO))
            {
                //si no hay parámetros retorna al inxdex
                if (id == null)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Verifique los valores de planta y línea.", TipoMensajesSweetAlerts.WARNING);
                    return RedirectToAction("Index");
                }

                produccion_registros produccion = db.produccion_registros.Find(id);
                if (produccion == null)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("No existe el registro de producción.", TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Index");
                }



                empleados emp = obtieneEmpleadoLogeado();

                //verifica si tiene segunda platina
                if (!string.IsNullOrEmpty(produccion.sap_platina_2))
                    produccion.segunda_platina = true;

                //crea un select li
                List<SelectListItem> listadoPlatinas = ComboSelect.obtieneMaterial_BOM();
                List<SelectListItem> listadoRollos = ComboSelect.obtieneRollo_BOM(produccion.sap_platina);

                ViewBag.sap_platina = AddFirstItem(new SelectList(listadoPlatinas, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: produccion.sap_platina);
                ViewBag.sap_platina_2 = AddFirstItem(new SelectList(listadoPlatinas, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: produccion.sap_platina_2);
                ViewBag.sap_rollo = new SelectList(ComboSelect.obtieneRollo_BOM(produccion.sap_platina), "Value", "Text", selectedValue: produccion.sap_rollo);
                ViewBag.id_supervisor = AddFirstItem(new SelectList(db.produccion_supervisores.Where(p => p.activo == true && p.clave_planta == produccion.clave_planta),
                    nameof(produccion_supervisores.id), "empleados." + nameof(produccion_supervisores.empleados.ConcatNombre)),
                    textoPorDefecto: "-- Seleccionar --", selected: produccion.id_supervisor.ToString());
                ViewBag.id_operador = ComboSelect.obtieneOperadorPorLinea(emp, produccion.id_linea.Value);


                return View(produccion);
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
        public ActionResult Edit(produccion_registros produccion_registros, FormCollection collection)
        {

            //si el modelo es válido
            if (ModelState.IsValid)
            {
                db.Entry(produccion_registros).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                //retorna la vista de datos de entrada
                return RedirectToAction("Index", new
                {
                    planta = produccion_registros.clave_planta,
                    linea = produccion_registros.id_linea
                });
            }

            empleados emp = obtieneEmpleadoLogeado();

            produccion_registros.plantas = db.plantas.Find(produccion_registros.clave_planta);
            produccion_registros.produccion_lineas = db.produccion_lineas.Find(produccion_registros.id_linea);
            produccion_registros.produccion_turnos = db.produccion_turnos.Find(produccion_registros.id_turno);

            //crea un select li
            List<SelectListItem> listadoPlatinas = ComboSelect.obtieneMaterial_BOM();
            List<SelectListItem> listadoRollos = ComboSelect.obtieneRollo_BOM(produccion_registros.sap_platina);

            ViewBag.sap_platina = AddFirstItem(new SelectList(listadoPlatinas, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: produccion_registros.sap_platina);
            ViewBag.sap_platina_2 = AddFirstItem(new SelectList(listadoPlatinas, "Value", "Text"), textoPorDefecto: "-- Seleccionar --", selected: produccion_registros.sap_platina_2);
            ViewBag.sap_rollo = new SelectList(ComboSelect.obtieneRollo_BOM(produccion_registros.sap_platina), "Value", "Text", selectedValue: produccion_registros.sap_rollo);
            ViewBag.id_supervisor = AddFirstItem(new SelectList(db.produccion_supervisores.Where(p => p.activo == true && p.clave_planta == produccion_registros.clave_planta),
                nameof(produccion_supervisores.id), "empleados." + nameof(produccion_supervisores.empleados.ConcatNombre)),
                textoPorDefecto: "-- Seleccionar --", selected: produccion_registros.id_supervisor.ToString());
            ViewBag.id_operador = ComboSelect.obtieneOperadorPorLinea(emp, produccion_registros.id_linea.Value);




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
                    return View("../Error/BadRequest");
                }

                //busca si existe el registro de produccion
                produccion_registros produccion = db.produccion_registros.Find(id);

                if (produccion == null)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("No existe el registro de producción.", TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Index");
                }

                //busca si hay datos de entrada para el registro de producción
                produccion_datos_entrada produccion_datos_entrada = db.produccion_datos_entrada.FirstOrDefault(x => x.id_produccion_registro == id.Value);

                if (produccion_datos_entrada == null)
                {
                    //si no hay registro de entrada de datos crea uno nuevo con el id de registro de produccion
                    produccion_datos_entrada = new produccion_datos_entrada
                    {
                        id_produccion_registro = id.Value,
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

                //ENVIAR CLASS V3, SEGÚN EL MATERIAL produccion.sap_platina 2
                mm_v3 mm_2 = db.mm_v3.FirstOrDefault(x => x.Material == produccion.sap_platina_2);
                if (mm_2 == null)
                    mm_2 = new mm_v3 { };

                //ENVIAR cLASS SEGUN EL MATERIAL
                class_v3 class_2 = db.class_v3.FirstOrDefault(x => x.Object == produccion.sap_platina_2);
                if (class_2 == null)
                    class_2 = new class_v3 { };

                ViewBag.MM = mm;
                ViewBag.Class = class_;
                ViewBag.MM_2 = mm_2;
                ViewBag.Class_2 = class_2;


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
            //comprueba si hay un margen del 3% de toleracnicia
            //try
            //{
            //    //sumatoria de ambas
            //    double peso_bascula = produccion_registros.produccion_datos_entrada.peso_bascula_kgs.HasValue ? produccion_registros.produccion_datos_entrada.peso_bascula_kgs.Value : 0;
            //    double peso_regreso_rollo = produccion_registros.produccion_datos_entrada.peso_regreso_rollo_real.HasValue ? produccion_registros.produccion_datos_entrada.peso_regreso_rollo_real.Value : 0;
            //    double peso_etiqueta = produccion_registros.produccion_datos_entrada.peso_etiqueta.HasValue ? produccion_registros.produccion_datos_entrada.peso_etiqueta.Value : 0;
            //    double peso_de_rollo_usado_real = peso_bascula - peso_regreso_rollo;
            //    double dif_abs = Math.Abs(peso_bascula - peso_de_rollo_usado_real);
            //    double porcentaje_dif = (dif_abs / peso_de_rollo_usado_real) * 100;

            //    if (porcentaje_dif > 3)
            //        ModelState.AddModelError("", "La diferencia entre el Peso de Rollo Usado Real y el Peso de Báscula es mayor al 3%. Favor de verificar los datos.");

            //}
            //catch (Exception e)
            //{
            //    //do nothing
            //    //ModelState.AddModelError("", "Ocurrió un error al calcular el porcentaje de diferencia: " + e.Message);
            //}


            bool error = false;
            bool errorDoble = false;

            foreach (produccion_lotes lote in produccion_registros.produccion_lotes)
            {
                if (lote.numero_lote_derecho == null && lote.numero_lote_izquierdo == null)
                    error = true;
                if (lote.numero_lote_derecho != null && lote.numero_lote_izquierdo != null)
                    errorDoble = true;
            }

            if (error)
                ModelState.AddModelError("", "Verifique que se haya especificado un lote izquierdo o derecho para cada lote.");
            if (errorDoble)
                ModelState.AddModelError("", "Verifique que todos los lotes tengan especificado únicamente un lote, ya sea lote izquierdo o lote derecho. No ambos.");

            if (ModelState.IsValid)
            {
                //verifica que si existe en un registro en datos entrada            

                if (db.produccion_datos_entrada.Find(produccion_registros.id) == null) //si no existe en BD crea la entrada
                {
                    db.produccion_datos_entrada.Add(produccion_registros.produccion_datos_entrada);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch
                    {
                        EscribeExcepcion(new Exception("Trato de registrarse datos duplicados: " + produccion_registros.id), Clases.Models.EntradaRegistroEvento.TipoEntradaRegistroEvento.Error);
                    }
                }
                else
                {
                    //si existe lo modifica
                    produccion_datos_entrada datos_Entrada = db.produccion_datos_entrada.Find(produccion_registros.id);
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

                //actualiza y turno  por la ultima modificacion              
                produccion_registros pd = db.produccion_registros.Find(produccion_registros.id);
                pd.fecha = DateTime.Now;

                //actualiza el turno por la ultima modificacion
                var turnos = db.produccion_turnos.Where(x => x.clave_planta == pd.clave_planta.Value); //listado de turnos
                foreach (produccion_turnos t in turnos)
                    if (TimeSpanUtil.CalculateDalUren(DateTime.Now, t.hora_inicio, t.hora_fin))
                        pd.id_turno = t.id;

                db.Entry(pd).State = EntityState.Modified;
                db.SaveChanges();


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

            mm_v3 mm = db.mm_v3.FirstOrDefault(x => x.Material == produccion_registros.sap_platina);
            if (mm == null)
                mm = new mm_v3 { };

            //ENVIAR cLASS SEGUN EL MATERIAL
            class_v3 class_ = db.class_v3.FirstOrDefault(x => x.Object == produccion_registros.sap_platina);
            if (class_ == null)
                class_ = new class_v3 { };

            //ENVIAR CLASS V3, SEGÚN EL MATERIAL produccion.sap_platina 2
            mm_v3 mm_2 = db.mm_v3.FirstOrDefault(x => x.Material == produccion_registros.sap_platina_2);
            if (mm_2 == null)
                mm_2 = new mm_v3 { };

            //ENVIAR cLASS SEGUN EL MATERIAL
            class_v3 class_2 = db.class_v3.FirstOrDefault(x => x.Object == produccion_registros.sap_platina_2);
            if (class_2 == null)
                class_2 = new class_v3 { };

            ViewBag.MM = mm;
            ViewBag.Class = class_;
            ViewBag.MM_2 = mm_2;
            ViewBag.Class_2 = class_2;

            //para agregar sap platina 2
            ViewBag.produccion_datos_entrada_sap_platina_2 = ComboSelect.obtieneMaterial_BOM();

            return View(produccion_registros);
        }

        // GET: ProduccionRegistros/DatosEntradaDetais/5
        public ActionResult DatosEntradasDetails(int? id)
        {
            //verifica los permisos del usuario
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REGISTRO) || TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE))
            {
                //verifica si se envio un id
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }

                //busca si existe el registro de produccion
                produccion_registros produccion = db.produccion_registros.Find(id);

                if (produccion == null)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("No existe el registro de producción.", TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Index");
                }

                //busca si hay datos de entrada para el registro de producción
                produccion_datos_entrada produccion_datos_entrada = db.produccion_datos_entrada.FirstOrDefault(x => x.id_produccion_registro == id.Value);

                if (produccion_datos_entrada == null)
                {
                    //si no hay registro de entrada de datos crea uno nuevo con el id de registro de produccion
                    produccion_datos_entrada = new produccion_datos_entrada
                    {
                        id_produccion_registro = id.Value,
                        produccion_registros = produccion
                    };
                }

                //agrega datos entrada a la produccion
                produccion.produccion_datos_entrada = produccion_datos_entrada;


                mm_v3 mm = db.mm_v3.FirstOrDefault(x => x.Material == produccion.sap_platina);
                if (mm == null)
                    mm = new mm_v3 { };

                //ENVIAR cLASS SEGUN EL MATERIAL
                class_v3 class_ = db.class_v3.FirstOrDefault(x => x.Object == produccion.sap_platina);
                if (class_ == null)
                    class_ = new class_v3 { };

                //ENVIAR CLASS V3, SEGÚN EL MATERIAL produccion.sap_platina 2
                mm_v3 mm_2 = db.mm_v3.FirstOrDefault(x => x.Material == produccion.sap_platina_2);
                if (mm_2 == null)
                    mm_2 = new mm_v3 { };

                //ENVIAR cLASS SEGUN EL MATERIAL
                class_v3 class_2 = db.class_v3.FirstOrDefault(x => x.Object == produccion.sap_platina_2);
                if (class_2 == null)
                    class_2 = new class_v3 { };

                ViewBag.MM = mm;
                ViewBag.Class = class_;
                ViewBag.MM_2 = mm_2;
                ViewBag.Class_2 = class_2;
                return View(produccion);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: Plantas/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REGISTRO))
            {
                //verifica si se envio un id
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }

                //busca si existe el registro de produccion
                produccion_registros produccion = db.produccion_registros.Find(id);

                if (produccion == null)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("No existe el registro de producción.", TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Index");
                }

                //busca si hay datos de entrada para el registro de producción
                produccion_datos_entrada produccion_datos_entrada = db.produccion_datos_entrada.FirstOrDefault(x => x.id_produccion_registro == id.Value);

                if (produccion_datos_entrada == null)
                {
                    //si no hay registro de entrada de datos crea uno nuevo con el id de registro de produccion
                    produccion_datos_entrada = new produccion_datos_entrada
                    {
                        id_produccion_registro = id.Value,
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

        // POST: Plantas/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            produccion_registros registros = db.produccion_registros.Find(id);
            registros.activo = false;

            db.Entry(registros).State = EntityState.Modified;
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
                //retorna la vista de datos de entrada
                return RedirectToAction("Index", new
                {
                    planta = registros.clave_planta,
                    linea = registros.id_linea
                });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                //retorna la vista de datos de entrada
                return RedirectToAction("Index", new
                {
                    planta = registros.clave_planta,
                    linea = registros.id_linea
                });
            }
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.DISABLED, TipoMensajesSweetAlerts.SUCCESS);
            //retorna la vista de datos de entrada
            return RedirectToAction("Index", new
            {
                planta = registros.clave_planta,
                linea = registros.id_linea
            });
        }

        // GET: Plantas/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REGISTRO))
            {
                //verifica si se envio un id
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }

                //busca si existe el registro de produccion
                produccion_registros produccion = db.produccion_registros.Find(id);

                if (produccion == null)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("No existe el registro de producción.", TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Index");
                }

                //busca si hay datos de entrada para el registro de producción
                produccion_datos_entrada produccion_datos_entrada = db.produccion_datos_entrada.FirstOrDefault(x => x.id_produccion_registro == id.Value);

                if (produccion_datos_entrada == null)
                {
                    //si no hay registro de entrada de datos crea uno nuevo con el id de registro de produccion
                    produccion_datos_entrada = new produccion_datos_entrada
                    {
                        id_produccion_registro = id.Value,
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

        // POST: Plantas/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            produccion_registros registros = db.produccion_registros.Find(id);
            registros.activo = true;

            db.Entry(registros).State = EntityState.Modified;
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
                //retorna la vista de datos de entrada
                return RedirectToAction("Index", new
                {
                    planta = registros.clave_planta,
                    linea = registros.id_linea
                });

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                //retorna la vista de datos de entrada
                return RedirectToAction("Index", new
                {
                    planta = registros.clave_planta,
                    linea = registros.id_linea
                });
            }
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.ENABLED, TipoMensajesSweetAlerts.SUCCESS);
            //retorna la vista de datos de entrada
            return RedirectToAction("Index", new
            {
                planta = registros.clave_planta,
                linea = registros.id_linea
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: ProduccionRegistros/Respaldo
        public ActionResult Respaldo(int pagina = 1)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REGISTRO))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                //obtiene el empleado que inicio sesión
                empleados emp = obtieneEmpleadoLogeado();


                var cantidadRegistrosPorPagina = 20; // parámetro

                //muestra unicamente lso registros que el usuario edite
                var produccion_registros = db.produccion_respaldo.Include(p => p.empleados).Where(
                         x => x.empleado_id == emp.id
                        )
                    .OrderByDescending(x => x.fecha)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.produccion_respaldo.Include(p => p.empleados).Where(
                         x => x.empleado_id == emp.id
                       ).Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;

                return View(produccion_registros);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: ProduccionRegistros/CargarRespaldo
        public ActionResult CargarRespaldo()
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_REGISTRO))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: ProduccionRegistros/CargarRespaldo
        [HttpPost]
        public ActionResult CargarRespaldo(ExcelViewModel excelViewModel, FormCollection collection)
        {
            if (ModelState.IsValid)
            {


                string msjError = "No se ha podido leer el archivo seleccionado.";

                //lee el archivo seleccionado
                try
                {
                    HttpPostedFileBase stream = Request.Files["PostedFile"];


                    if (stream.InputStream.Length > 8388608)
                    {
                        msjError = "Sólo se permiten archivos con peso menor a 8 MB.";
                        throw new Exception(msjError);
                    }
                    else
                    {
                        string extension = Path.GetExtension(excelViewModel.PostedFile.FileName);
                        if (extension.ToUpper() != ".XLSM")
                        {
                            msjError = "Seleccione un archivo Excel válido.";
                            throw new Exception(msjError);
                        }
                    }

                    bool estructuraValida = false;

                    ////el archivo es válido
                    List<produccion_respaldo> lista = UtilExcel.LeeRespaldoBitacora(excelViewModel.PostedFile, ref estructuraValida);


                    if (!estructuraValida)
                    {
                        msjError = "No cumple con la estructura válida.";
                        throw new Exception(msjError);
                    }
                    else
                    {
                        int actualizados = 0;
                        int creados = 0;
                        int error = 0;

                        //obtiene el empleado que inicio sesión
                        empleados emp = obtieneEmpleadoLogeado();

                        List<produccion_respaldo> listAnterior = db.produccion_respaldo.ToList();

                        //determina que elementos de la lista no se encuentran en la lista anterior
                        List<produccion_respaldo> listDiferencias = lista.Except(listAnterior).ToList();

                        //determina cuales ya existian
                        int exitentes = lista.Intersect(listAnterior).Count();

                        foreach (produccion_respaldo respaldo_item in listDiferencias)
                        {
                            try
                            {
                                //si hay valor en id empleado lo asigna
                                if (emp.id != 0)
                                    respaldo_item.empleado_id = emp.id;

                                //obtiene el elemento de BD
                                produccion_respaldo item = db.produccion_respaldo.FirstOrDefault(x => x.planta == respaldo_item.planta && x.linea == respaldo_item.linea && x.fecha == respaldo_item.fecha);

                                //si existe actualiza
                                if (item != null)
                                {
                                    respaldo_item.id = item.id;
                                    db.Entry(item).CurrentValues.SetValues(respaldo_item);
                                    db.SaveChanges();
                                    actualizados++;
                                }
                                else
                                {
                                    //crea un nuevo registro
                                    db.produccion_respaldo.Add(respaldo_item);
                                    db.SaveChanges();
                                    creados++;
                                }

                            }
                            catch (Exception e)
                            {
                                error++;
                            }

                        }

                        ////obtiene nuevamente la lista de BD
                        //listAnterior = db.bom_en_sap.ToList();
                        ////determina que elementos de la listAnterior no se encuentran en la lista Excel
                        //listDiferencias = listAnterior.Except(lista).ToList();

                        ////elima de BD aquellos que no se encuentren en el excel
                        //foreach (bom_en_sap bom in listDiferencias)
                        //{
                        //    try
                        //    {
                        //        //obtiene el elemento de BD
                        //        bom_en_sap item = db.bom_en_sap.FirstOrDefault(x => x.Material == bom.Material && x.Plnt == bom.Plnt && x.BOM == bom.BOM && x.AltBOM == bom.AltBOM && x.Item == bom.Item);

                        //        //si existe elimina
                        //        if (item != null)
                        //        {
                        //            db.Entry(item).State = EntityState.Deleted;
                        //            db.SaveChanges();
                        //            eliminados++;
                        //        }

                        //    }
                        //    catch (Exception e)
                        //    {
                        //        error++;
                        //    }
                        //}


                        TempData["Mensaje"] = new MensajesSweetAlert("Creados: " + creados + " -> Actualizados: " + actualizados + " -> Existentes: " + exitentes + "-> Errores: " + error, TipoMensajesSweetAlerts.INFO);
                        return RedirectToAction("Respaldo");
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", msjError);
                    return View(excelViewModel);
                }

            }
            return View(excelViewModel);
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
            list[0] = new { Message = "Error: Se recibe mensaje, pero no hubo macth" };
            string patron = @"(?:- *)?\d+(?:\.\d+)?";

            
            ServiceReferenceBasculas.WebServiceBasculasSoapClient cliente = new ServiceReferenceBasculas.WebServiceBasculasSoapClient();

            //primero trata de obtener el peso desde webservice
            try
            {
                string peso = String.Empty;
                peso = cliente.PesoBascula(ip);

                if (Double.TryParse(peso,out double result)) {

                    list[0] = new { Message = "OK", Peso = peso };
                    //si recibe peso, retorna la respuiesta
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e) {
                list[0] = new { Message = "Error: " + e.Message };
            }

            //conecta con la báscula (en caso de que no funcionará el webservice)
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
                list[0] = new { Message = "Error: " + e.Message };
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        ///<summary>
        ///Retorna si la contraseña ingresada es correcta
        ///</summary>
        ///<return>
        ///retorna un JsonResult con el resultado
        ///        
        public JsonResult VerificaPassword(int? idSupervisor, double? tiempo, string password = "")
        {
            var list = new object[1];

            if (idSupervisor == null)
            {
                list[0] = new { Status = "Error", Message = "No se envíó el id de Supervisor." };
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            ApplicationUser userSupervisor = _userManager.Users.FirstOrDefault(x => x.IdEmpleado == idSupervisor);

            if (userSupervisor == null)
            {
                list[0] = new { Status = "Error", Message = "No existe el supervisor." };
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            Task<bool> validado = _userManager.CheckPasswordAsync(userSupervisor, password);

            //si el password es correcto
            if (validado.Result)
            {
                list[0] = new { Status = "OK", Message = "Contraseña Correcta" };

                //guarda en variable de sesión el tiempo permitido
                if (tiempo != null && tiempo.HasValue)
                    Session["TiempoAutorizado"] = DateTime.Now.AddMinutes(tiempo.Value);

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                list[0] = new { Status = "FALSE", Message = "Contraseña incorrecta" };
                return Json(list, JsonRequestBehavior.AllowGet);
            }

        }

        ///<summary>
        ///Verifica si tiene permiso para modificar
        ///</summary>
        ///<return>
        ///retorna un JsonResult con el resultado
        ///        
        public JsonResult VerificaTiempoPermitido()
        {
            var list = new object[1];

            empleados e = obtieneEmpleadoLogeado();



            DateTime autorizacion = DateTime.Now.AddDays(-1);
            if (Session["TiempoAutorizado"] != null)
                autorizacion = Convert.ToDateTime(Session["TiempoAutorizado"]);

            int estado = DateTime.Compare(autorizacion, DateTime.Now);

            //si el tiempo de autorizacion es mayor al tiempo actual
            if (estado >= 1)
            {
                list[0] = new { Status = "OK", Message = "Está autorizado" };

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                list[0] = new { Status = "FALSE", Message = "No está autorizado" };
                return Json(list, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
