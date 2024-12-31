using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using DocumentFormat.OpenXml.Presentation;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BG_forecast_cat_historico_ventasController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: BG_forecast_cat_historico_ventas
        public ActionResult Index(string fy, int? id_seccion)
        {

            if (!TieneRol(TipoRoles.BUDGET_IHS))
                return View("../Home/ErrorPermisos");

            // -- tipo de listado ORIGEN
            List<SelectListItem> selectSeccion = new List<SelectListItem>();

            for (int i = 2010; i < 2040; i++)
            {
                selectSeccion.Add(new SelectListItem() { Text = "FY " + i + "-" + (i + 1), Value = i + "-" + (i + 1) });
            }

            if (string.IsNullOrEmpty(fy))
            {
                fy = (DateTime.Now.Year - 1).ToString() + "-" + (DateTime.Now.Year).ToString();
            }

            ViewBag.SeccionSelected = db.BG_Forecast_cat_secciones_calculo.Find(id_seccion.HasValue ? id_seccion : 1);
            ViewBag.fy = new SelectList(selectSeccion, "Value", "Text", selectedValue: fy);
            ViewBag.id_seccion = new SelectList(db.BG_Forecast_cat_secciones_calculo.Where(x => x.activo), nameof(BG_Forecast_cat_secciones_calculo.id), nameof(BG_Forecast_cat_secciones_calculo.descripcion));

            return View();
        }

        public JsonResult CargaSeccion(string fy, int? id_seccion)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var clientesList = db.BG_forecast_cat_clientes.ToList();

            var jsonData = new object[clientesList.Count()];

            //fecha inicial
            string[] years = fy.Split('-');
            DateTime.TryParseExact(years[0] + "-10-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechainicial);


            for (int i = 0; i < clientesList.Count(); i++)
            {
                //octubre
                var octubre = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var noviembre = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(1) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var diciembre = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(2) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var enero = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(3) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var febrero = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(4) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var marzo = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(5) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var abril = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(6) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var mayo = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(7) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var junio = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(8) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var julio = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(9) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var agosto = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(10) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);
                var septiembre = clientesList[i].BG_forecast_cat_historico_ventas.FirstOrDefault(x => x.fecha == fechainicial.AddMonths(11) && x.id_cliente == clientesList[i].id && x.id_seccion == id_seccion);


                jsonData[i] = new[] {
                    clientesList[i].id.ToString(),  //id cliente
                    fy,                             //clave de fy
                    id_seccion.ToString(),          //clave seccion    
                    clientesList[i].descripcion,     //nombre cliente
                    octubre!= null ? octubre.valor.ToString() : null, //octubre
                    noviembre!= null ? noviembre.valor.ToString() : null, //noviembre
                    diciembre!= null ? diciembre.valor.ToString() : null, //diciembre
                    enero!= null ? enero.valor.ToString() : null, //enero
                    febrero!= null ? febrero.valor.ToString() : null, //febrero
                    marzo!= null ? marzo.valor.ToString() : null, //marzo
                    abril!= null ? abril.valor.ToString() : null, //abril
                    mayo!= null ? mayo.valor.ToString() : null, //mayo
                    junio!= null ? junio.valor.ToString() : null, //junio
                    julio!= null ? julio.valor.ToString() : null, //julio
                    agosto!= null ? agosto.valor.ToString() : null, //agosto
                    septiembre!= null ? septiembre.valor.ToString() : null, //septiembre
                    };
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EnviaVentasForm(string fy_val, int? id_seccion, List<string[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var list = new object[1];

            //convierte el list de arrays en objetos SCDM_solicitud_rel_item_material
            List<BG_forecast_cat_historico_ventas> valoresList = ConvierteArrayAValoresVentas(dataListFromTable, fy_val);
            //if (valoresList.Count == 0)
            //    list[0] = new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." };

            //fecha inicial
            string[] years = fy_val.Split('-');
            DateTime.TryParseExact(years[0] + "-10-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechainicial);

            DateTime fechaFinal = fechainicial.AddMonths(12);

            //obtiene el listado Actual de la BD
            List<BG_forecast_cat_historico_ventas> valoresListBD = db.BG_forecast_cat_historico_ventas.Where(x => x.id_seccion == id_seccion
                && x.fecha >= fechainicial && x.fecha < fechaFinal).ToList();


            //crea, modifica o elimina 
            try
            {
                foreach (var valor in valoresList)
                {
                    // si edicion
                    var itemBD = valoresListBD.FirstOrDefault(x => x.id_cliente == valor.id_cliente && x.fecha == valor.fecha);
                    if (itemBD != null) //update
                    {
                        if (itemBD.valor != valor.valor)
                            itemBD.valor = valor.valor;
                    }
                    else //creacion
                    {
                        db.BG_forecast_cat_historico_ventas.Add(new BG_forecast_cat_historico_ventas
                        {
                            id_cliente = valor.id_cliente,
                            id_seccion = valor.id_seccion,
                            fecha = valor.fecha,
                            valor = valor.valor,
                        });
                    }

                }
                //elimina aquellos que no aparezcan en los enviados 
                var toDeleteList = valoresListBD.Where(x => !valoresList.Any(y => y.fecha == x.fecha && y.id_cliente == x.id_cliente)).ToList();
                db.BG_forecast_cat_historico_ventas.RemoveRange(toDeleteList);
                db.SaveChanges();
                list[0] = new { result = "OK", icon = "success", operacion = "CREATE", message = "Se guardaron los cambios correctamente" };

            }
            catch (Exception e)
            {
                list[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private List<BG_forecast_cat_historico_ventas> ConvierteArrayAValoresVentas(List<string[]> data, string fy_val)
        {
            List<BG_forecast_cat_historico_ventas> resultado = new List<BG_forecast_cat_historico_ventas> { };

            //variables globales para el metodo
            var BD_SCDM_planta = db.plantas.Where(x => x.activo == true).ToList();

            //listado de encabezados
            string[] encabezados = {
                "ClienteID","FY","SeccionID","Cliente"
            };

            string[] mesesFY = { "Octubre", "Noviembre", "Diciembre", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre" };

            encabezados = encabezados.Concat(mesesFY).ToArray();


            //recorre todos los arrays recibidos
            foreach (var array in data)
            {
                //verifica si la fila cumple con los requerimientos campos minimos
                var arrayList = array.ToList();
                bool isAllEqual = arrayList.Count > 0 && !arrayList.Any(x => x != arrayList.First()) && String.IsNullOrEmpty(arrayList.First());
                if (isAllEqual)
                    continue;

                //declaración de variables
                int id_cliente = 0;
                int id_seccion = 0;
                DateTime fecha = DateTime.Now;
                string codigoPlanta = string.Empty;

                //id_cliente
                if (int.TryParse(array[Array.IndexOf(encabezados, "ClienteID")], out int id_cliente_result))
                    id_cliente = id_cliente_result;

                //id_seccion
                if (int.TryParse(array[Array.IndexOf(encabezados, "SeccionID")], out int id_seccion_result))
                    id_seccion = id_seccion_result;

                //fecha inicial
                string[] years = fy_val.Split('-');
                DateTime.TryParseExact(years[0] + "-10-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechainicial);

                for (int i = 0; i < 12; i++)
                {
                    double valor = 0;
                    if (double.TryParse(array[Array.IndexOf(encabezados, mesesFY[i])], out double valor_result))
                        valor = valor_result;

                    //solo agrega si tiene valor
                    if (valor != 0)
                        resultado.Add(new BG_forecast_cat_historico_ventas
                        {
                            id_cliente = id_cliente,
                            id_seccion = id_seccion,
                            fecha = fechainicial.AddMonths(i),
                            valor = valor
                        });
                }


            }

            return resultado;
        }

        // GET: BG_forecast_cat_historico_ventas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_forecast_cat_historico_ventas bG_forecast_cat_historico_ventas = db.BG_forecast_cat_historico_ventas.Find(id);
            if (bG_forecast_cat_historico_ventas == null)
            {
                return HttpNotFound();
            }
            return View(bG_forecast_cat_historico_ventas);
        }

        // GET: BG_forecast_cat_historico_ventas/Create
        public ActionResult Create()
        {
            ViewBag.id_cliente = new SelectList(db.BG_forecast_cat_clientes, "id", "descripcion");
            return View();
        }

        // POST: BG_forecast_cat_historico_ventas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_cliente,fecha,valor")] BG_forecast_cat_historico_ventas bG_forecast_cat_historico_ventas)
        {
            if (ModelState.IsValid)
            {
                db.BG_forecast_cat_historico_ventas.Add(bG_forecast_cat_historico_ventas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_cliente = new SelectList(db.BG_forecast_cat_clientes, "id", "descripcion", bG_forecast_cat_historico_ventas.id_cliente);
            return View(bG_forecast_cat_historico_ventas);
        }

        // GET: BG_forecast_cat_historico_ventas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_forecast_cat_historico_ventas bG_forecast_cat_historico_ventas = db.BG_forecast_cat_historico_ventas.Find(id);
            if (bG_forecast_cat_historico_ventas == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cliente = new SelectList(db.BG_forecast_cat_clientes, "id", "descripcion", bG_forecast_cat_historico_ventas.id_cliente);
            return View(bG_forecast_cat_historico_ventas);
        }

        // POST: BG_forecast_cat_historico_ventas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_cliente,fecha,valor")] BG_forecast_cat_historico_ventas bG_forecast_cat_historico_ventas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bG_forecast_cat_historico_ventas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_cliente = new SelectList(db.BG_forecast_cat_clientes, "id", "descripcion", bG_forecast_cat_historico_ventas.id_cliente);
            return View(bG_forecast_cat_historico_ventas);
        }

        // GET: BG_forecast_cat_historico_ventas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BG_forecast_cat_historico_ventas bG_forecast_cat_historico_ventas = db.BG_forecast_cat_historico_ventas.Find(id);
            if (bG_forecast_cat_historico_ventas == null)
            {
                return HttpNotFound();
            }
            return View(bG_forecast_cat_historico_ventas);
        }

        // POST: BG_forecast_cat_historico_ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BG_forecast_cat_historico_ventas bG_forecast_cat_historico_ventas = db.BG_forecast_cat_historico_ventas.Find(id);
            db.BG_forecast_cat_historico_ventas.Remove(bG_forecast_cat_historico_ventas);
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
