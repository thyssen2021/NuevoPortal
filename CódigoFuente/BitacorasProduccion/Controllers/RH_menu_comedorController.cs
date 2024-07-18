using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using DocumentFormat.OpenXml.EMMA;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class RH_menu_comedorController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: RH_menu_comedor
        public ActionResult Index(int? id_planta, string fecha = "")
        {
            if (!TieneRol(TipoRoles.RH_MENU_COMEDOR_PUEBLA))
                return View("../Home/ErrorPermisos");

            //obtiene la planta
            var planta = db.plantas.Find(id_planta);

            //verifica que la planta exista
            if (planta == null) {
                ViewBag.Titulo = "¡Lo sentimos!¡No se pudo cargar el menú del comedor!";
                ViewBag.Descripcion = "Motivo: No se pudo encontrar la planta de consulta.";

                return View("../Home/ErrorGenerico");
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //fecha por defecto
            DateTime fechaBusqueda = DateTime.Now;
            //elimina las horas, minutos y segundos de la fecha
            fechaBusqueda = new DateTime(fechaBusqueda.Year, fechaBusqueda.Month, fechaBusqueda.Day);

            if (!string.IsNullOrEmpty(fecha))
                fechaBusqueda = DateTime.Parse(fecha);

            //obtiene el numero de semana
            int semana = GetWeekNumber(fechaBusqueda);

            DateTime inicioSemana = GetInicioSemana(fechaBusqueda);
            DateTime finSemana = GetFinSemana(fechaBusqueda);

            //Manda la fecha en Texto
            ViewBag.FechaTexto = String.IsNullOrEmpty(fecha) ? fechaBusqueda.ToString("yyyy-MM-dd") : fecha;
            ViewBag.SemanaTexto = "Semana " + semana + ": del " + inicioSemana.ToString("D", new System.Globalization.CultureInfo("es-MX")) + " al " + finSemana.ToString("D", new System.Globalization.CultureInfo("es-MX"));
            ViewBag.Planta = planta;

            var listado = db.RH_menu_comedor_platillos.Where(x => x.fecha >= inicioSemana && x.fecha <= finSemana && x.id_planta == planta.clave).ToList();

            return View(listado);
        }
        public ActionResult CargaMenu(int? id_planta)
        {
            if (!TieneRol(TipoRoles.RH_MENU_COMEDOR_PUEBLA))
                return View("../Home/ErrorPermisos");

            //obtiene la planta
            var planta = db.plantas.Find(id_planta);

            //verifica que la planta exista
            if (planta == null)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se pudo cargar el menú del comedor!";
                ViewBag.Descripcion = "Motivo: No se pudo encontrar la planta de consulta.";

                return View("../Home/ErrorGenerico");
            }

            ViewBag.Planta = planta;


            return View();
        }

        public ActionResult ListaDistribucion()
        {
            if (!TieneRol(TipoRoles.RH_MENU_COMEDOR_PUEBLA))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            ViewBag.empleados = db.empleados.Where(x => x.activo.HasValue && x.activo.Value).OrderBy(x => x.planta_clave).ThenBy(x=>x.numeroEmpleado).ToList();
            ViewBag.activos = db.notificaciones_correo.Where(x => x.descripcion == "MENU_COMEDOR_DISTRIBUCION_PUEBLA" && x.id_empleado.HasValue).Select(x => x.id_empleado.Value).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListaDistribucion(int[] empleados)
        {

            if (empleados == null || empleados.Length == 0)
                ModelState.AddModelError("", "No se seleccionaron empleados.");

            //listado de notificaciones de correo
            var notificacionesList = db.notificaciones_correo.Where(x=> x.descripcion == "MENU_COMEDOR_DISTRIBUCION_PUEBLA" && x.activo).ToList();

            //realiza cambios en la BD        
            foreach (var id_empleado in empleados)
            {
                //crea los roles en caso de no existir
                if (!notificacionesList.Any(x => x.id_empleado == id_empleado)) {
                    var newNotificacion = new notificaciones_correo
                    {
                        id_empleado = id_empleado,
                        descripcion = "MENU_COMEDOR_DISTRIBUCION_PUEBLA",
                        activo =  true,
                    };

                    db.notificaciones_correo.Add(newNotificacion);
                }
            }


            //elimina los roles que no se enviaron
            db.notificaciones_correo.RemoveRange(notificacionesList.Where(x=> !empleados.Any(y=> y == x.id_empleado)));

            db.SaveChanges();
            TempData["Mensaje"] = new MensajesSweetAlert("Se actualizó la lista de distribución correctamente", TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("ListaDistribucion");


        }

        [AllowAnonymous]
        public ActionResult DetailsMenu(int? id_planta, string fecha = "")
        {
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            //obtiene la planta
            var planta = db.plantas.Find(id_planta);

            //verifica que la planta exista
            if (planta == null)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se pudo cargar el menú del comedor!";
                ViewBag.Descripcion = "Motivo: No se pudo encontrar la planta de consulta.";

                return View("../Home/ErrorGenerico");
            }

            //fecha por defecto
            DateTime fechaBusqueda = DateTime.Now;
            //elimina las horas, minutos y segundos de la fecha
            fechaBusqueda = new DateTime(fechaBusqueda.Year, fechaBusqueda.Month, fechaBusqueda.Day);

            if (!string.IsNullOrEmpty(fecha))
                fechaBusqueda = DateTime.Parse(fecha);

            //obtiene el numero de semana
            int semana = GetWeekNumber(fechaBusqueda);

            DateTime inicioSemana = GetInicioSemana(fechaBusqueda);
            DateTime finSemana = GetFinSemana(fechaBusqueda);

            //Manda la fecha en Texto
            ViewBag.FechaTexto = String.IsNullOrEmpty(fecha) ? fechaBusqueda.ToString("yyyy-MM-dd") : fecha;
            ViewBag.FechaDateTime = fechaBusqueda;
            ViewBag.SemanaTexto = "Semana " + semana + ": del " + inicioSemana.ToString("D", new System.Globalization.CultureInfo("es-MX")) + " al " + finSemana.ToString("D", new System.Globalization.CultureInfo("es-MX"));
            ViewBag.Planta = planta;


            var listado = db.RH_menu_comedor_platillos.Where(x => x.fecha >= inicioSemana && x.fecha <= finSemana && x.id_planta == planta.clave).ToList();

            return View(listado);
        }

        // POST: Dante/CargaBom/5
        [HttpPost]
        public ActionResult CargaMenu(ExcelViewModel excelViewModel, FormCollection collection)
        {
            //determina si se encuentra la planta
            int? plantaClave = null;
            if (int.TryParse(collection["id_planta"], out int resultPlanta))
                plantaClave = resultPlanta;

            var planta = db.plantas.Find(plantaClave);

            if (plantaClave == null)
                ModelState.AddModelError("", "No se encontró una planta en la solicitud. Intente ingresar nuevamente desde el menú principal.");
            else
                ViewBag.Planta = planta;

            if (ModelState.IsValid)
            {
                string msjError = "No se ha podido leer el archivo seleccionado.";

                //lee el archivo seleccionado

                HttpPostedFileBase stream = Request.Files["PostedFile"];

                //valida tamaño y extiensión de archivo
                if (stream.InputStream.Length > 8388608)
                {
                    msjError = "Sólo se permiten archivos con peso menor a 8 MB.";
                    throw new Exception(msjError);
                }
                else
                {
                    string extension = Path.GetExtension(excelViewModel.PostedFile.FileName);
                    if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                    {
                        msjError = "Sólo se permiten archivos Excel";
                        throw new Exception(msjError);
                    }
                }

                //lee archivo

                List<string> errores = new List<string>();

                List<RH_menu_comedor_platillos> menuItems = UtilExcel.LeeMenuComedor(excelViewModel.PostedFile, ref errores, plantaClave.Value);

                //Valida el resultado de la lectura
                foreach (var error in errores)
                    ModelState.AddModelError("", error);

                if (ModelState.IsValid)
                {
                    int creados = 0, actualizados = 0, sinCambio = 0;

                    //recorre todos los registros recibidos
                    foreach (var item in menuItems)
                    {
                        //busca un registro en la bd;
                        var itemBD = db.RH_menu_comedor_platillos.FirstOrDefault(x => x.fecha == item.fecha && x.tipo_platillo == item.tipo_platillo && x.id_planta == item.id_planta);
                        //si no existe en BD lo agrega
                        if (itemBD == null)
                        {
                            db.RH_menu_comedor_platillos.Add(item);
                            creados++;
                        }//si existe y tiene datos diferentes ...edita
                        else if ((itemBD.tipo_platillo != item.tipo_platillo)
                            || (itemBD.nombre_platillo != item.nombre_platillo)
                            || (itemBD.kcal != item.kcal)
                            )
                        {
                            //edita
                            itemBD.kcal = item.kcal;
                            itemBD.nombre_platillo = item.nombre_platillo;
                            actualizados++;
                        }
                        else
                        { //existe, pero tiene los mismos valores 
                            sinCambio++;
                        }

                        //En caso de no existir ... crea un Nuevo Registro
                    }
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error al guardar en BD", TipoMensajesSweetAlerts.ERROR);
                        return RedirectToAction("index");
                    }

                    TempData["Mensaje"] = new MensajesSweetAlert("Se ha cargado el menú correctamente. Creados: " + creados + ", Actualizados: " + actualizados + ", Sin Cambio: " + sinCambio, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("index", new { id_planta = plantaClave });
                }
                else
                {
                    return View(excelViewModel);
                }

            }
            return View(excelViewModel);
        }

        // GET: empleados/Edit/5
        public ActionResult Edit(int? id, int? id_planta, string s_fecha = "")
        {

            if (TieneRol(TipoRoles.RH_MENU_COMEDOR_PUEBLA))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                RH_menu_comedor_platillos item = db.RH_menu_comedor_platillos.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }

                ViewBag.s_fecha = s_fecha;
                ViewBag.id_planta = id_planta.ToString();

                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: empleados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RH_menu_comedor_platillos item, FormCollection collection)
        {
            //valores enviados previamente
            string fecha = collection["s_fecha"].ToString();
            string plantaString = collection["id_planta"].ToString();


            if (ModelState.IsValid)
            {
                //realiza la modificacion del modelo
                var itemBD = db.RH_menu_comedor_platillos.Find(item.id);

                if (itemBD != null)
                {
                    itemBD.nombre_platillo = item.nombre_platillo;
                    itemBD.kcal = item.kcal;
                    db.SaveChanges();

                }
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index", new { fecha = fecha, id_planta = plantaString });
            }

            ViewBag.s_fecha = fecha;
            ViewBag.id_planta = plantaString;
            return View(item);
        }


        [NonAction]
        protected int GetWeekNumber(DateTime fechaBusqueda)
        {
            //obtiene el numero de la semana
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(fechaBusqueda);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                fechaBusqueda = fechaBusqueda.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(fechaBusqueda, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        [NonAction]
        protected DateTime GetInicioSemana(DateTime fechaBusqueda)
        {

            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(fechaBusqueda);
            int resta = 0;

            if (day == DayOfWeek.Sunday)
                resta = 6;
            else
                resta = (int)day - 1;

            // Return the week of our adjusted day
            return fechaBusqueda.AddDays(-resta);
        }
        [NonAction]
        protected DateTime GetFinSemana(DateTime fechaBusqueda)
        {

            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(fechaBusqueda);
            int suma = 0;

            switch (day)
            {
                case DayOfWeek.Sunday:
                    //nothing
                    break;
                case DayOfWeek.Monday:
                    suma = 6;
                    break;
                case DayOfWeek.Tuesday:
                    suma = 5;
                    break;
                case DayOfWeek.Wednesday:
                    suma = 4;
                    break;
                case DayOfWeek.Thursday:
                    suma = 3;
                    break;
                case DayOfWeek.Friday:
                    suma = 2;
                    break;
                case DayOfWeek.Saturday:
                    suma = 1;
                    break;
            }

            // Return the week of our adjusted day
            return fechaBusqueda.AddDays(suma).AddHours(23).AddMinutes(59).AddSeconds(59);
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
