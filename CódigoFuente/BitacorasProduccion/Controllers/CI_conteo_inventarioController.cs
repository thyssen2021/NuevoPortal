using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Clases.Util;
using Microsoft.Ajax.Utilities;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class CI_conteo_inventarioController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: CI_conteo_inventario
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.CI_CONTEO_INVENTARIO) && !!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            List<CI_conteo_inventario> lista = new List<CI_conteo_inventario>();
            return View(lista);
        }
        
        // GET: CI_conteo_inventario
        public ActionResult Inventario(string planta_sap)
        {
            if (!TieneRol(TipoRoles.CI_CONTEO_INVENTARIO) && !!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var planta = db.plantas.Where(x => x.codigoSap == planta_sap).FirstOrDefault();

            if (planta == null) {
                ViewBag.Titulo = "No se pudo cargar la información.";
                ViewBag.Descripcion = "No se envío la planta o el número no es válido. Intenta ingresando desde el menú principal.";

                return View("../Home/ErrorGenerico");
            }

            ViewBag.numero_registros = db.CI_conteo_inventario.Where(x=> x.plant == planta_sap).Count();

            return View(planta);
        }


        // GET: CI_conteo_inventario/Edit/5
        public ActionResult Multiple(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CI_conteo_inventario cI_conteo_inventario = db.CI_conteo_inventario.Find(id);
            if (cI_conteo_inventario == null)
            {
                return HttpNotFound();
            }
            List<CI_conteo_inventario> listadoEtiquetas = db.CI_conteo_inventario.Where(p => p.material == cI_conteo_inventario.material).ToList();

            ViewBag.ListadoEtiquetas = listadoEtiquetas;

            cI_conteo_inventario.etiquetas = db.CI_conteo_inventario.Where(x => cI_conteo_inventario.num_tarima != null && x.num_tarima == cI_conteo_inventario.num_tarima).ToList();

            return View(cI_conteo_inventario);
        }

        // POST: CI_conteo_inventario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Multiple(CI_conteo_inventario cI_conteo_inventario, FormCollection form)
        {
            cI_conteo_inventario.etiquetas = new List<CI_conteo_inventario>();
            foreach (var item in form.AllKeys.Where(x => x.Contains("etiquetas[")))
            {
                cI_conteo_inventario.etiquetas.Add(new CI_conteo_inventario
                {
                    id = Int32.Parse(form[item])
                });
            }

            if (cI_conteo_inventario.etiquetas == null || cI_conteo_inventario.etiquetas.Count == 0)
            {
                ModelState.AddModelError("", "No se agregaron eqtiquetas.");
            }

            if (cI_conteo_inventario.altura == null || cI_conteo_inventario.altura == 0)
                ModelState.AddModelError("altura", "Ingrese una altura válida.");

            bool repetido = false;
            if (cI_conteo_inventario.etiquetas != null)
                foreach (var item in cI_conteo_inventario.etiquetas)
                {
                    repetido = cI_conteo_inventario.etiquetas.Count(x => x.id == item.id) > 1;

                    if (repetido)
                    {
                        ModelState.AddModelError("", "Existen etiquetas repetidas.");
                        break;
                    }
                }
            var empleado = obtieneEmpleadoLogeado();
            if (ModelState.IsValid)
            {

                var tarimasFormIds = cI_conteo_inventario.etiquetas.Select(x => x.id).Distinct().ToList();

                var tarimas = db.CI_conteo_inventario.Where(x => tarimasFormIds.Contains(x.id)).Select(x => x.num_tarima).Distinct().ToList();
                //todas las tarimas que tenga asociado el numero de tarima
                var tarimasBD = db.CI_conteo_inventario.Where(x => tarimas.Contains(x.num_tarima));
                //quita los valores de la BD
                foreach (var item in tarimasBD)
                {
                    item.altura = null;
                    item.espesor = null;
                    item.num_tarima = null;
                    item.id_empleado = null;
                }

                var tarimasBDForm = db.CI_conteo_inventario.Where(x => tarimasFormIds.Contains(x.id)).ToList();
                for (int i = 0; i < tarimasBDForm.Count(); i++)
                {
                    tarimasBDForm[i].num_tarima = tarimasBDForm[0].id;
                    tarimasBDForm[i].id_empleado = empleado.id;

                    if (i == 0)
                    {
                        tarimasBDForm[i].altura = cI_conteo_inventario.altura;
                        tarimasBDForm[i].espesor = cI_conteo_inventario.espesor;
                    }
                    else
                    {
                        tarimasBDForm[i].altura = 0;
                        tarimasBDForm[i].espesor = 0;
                    }
                }

                //db.Entry(cI_conteo_inventario).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = "Se editó correctamente el registro.";
                ViewBag.Tipo = TipoMensajesSweetAlerts.SUCCESS;
                TempData["Mensaje"] = new MensajesSweetAlert("Se editó correctamente el registro.", TipoMensajesSweetAlerts.SUCCESS);
                return View("Message");
            }

            List<CI_conteo_inventario> listadoEtiquetas = db.CI_conteo_inventario.Where(p => p.material == cI_conteo_inventario.material).ToList();
            ViewBag.ListadoEtiquetas = listadoEtiquetas;
            return View(cI_conteo_inventario);
        }
        // GET: CI_conteo_inventario/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.CI_CONTEO_INVENTARIO) && !!TieneRol(TipoRoles.ADMIN))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CI_conteo_inventario cI_conteo_inventario = db.CI_conteo_inventario.Find(id);
            if (cI_conteo_inventario == null)
            {
                return HttpNotFound();
            }

            return View(cI_conteo_inventario);
        }

        // POST: CI_conteo_inventario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CI_conteo_inventario cI_conteo_inventario)
        {
            var empleado = obtieneEmpleadoLogeado();
            if (ModelState.IsValid)
            {
                var ciBD = db.CI_conteo_inventario.Find(cI_conteo_inventario.id);

                //borra las tarimas relacionadas
                var tarimasBD = db.CI_conteo_inventario.Where(x => ciBD.num_tarima != null && x.num_tarima == ciBD.num_tarima && x.id != ciBD.id);
                //quita los valores de la BD
                foreach (var item in tarimasBD)
                {
                    item.altura = null;
                    item.espesor = null;
                    item.num_tarima = null;
                    item.id_empleado = null;
                }


                ciBD.num_tarima = ciBD.id;
                ciBD.espesor = cI_conteo_inventario.espesor;
                ciBD.altura = cI_conteo_inventario.altura;
                ciBD.id_empleado = empleado.id;
                //db.Entry(cI_conteo_inventario).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = "Se editó correctamente el registro.";
                ViewBag.Tipo = TipoMensajesSweetAlerts.SUCCESS;
                TempData["Mensaje"] = new MensajesSweetAlert("Se editó correctamente el registro.", TipoMensajesSweetAlerts.SUCCESS);
                return View("Message");
            }
            return View(cI_conteo_inventario);
        }

        ///<summary>
        ///Obtiene las areas segun la planta recibida
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult CargaTabla()
        {

            DataTable dataTable = new DataTable();
            List<CI_conteo_inventario> stud = db.CI_conteo_inventario.OrderBy(x => x.id).ToList();
            List<CI_conteo_table> newList = new List<CI_conteo_table>();

            foreach (var item in stud)
            {
                //diferencia SAP
                double? dS = null;
                if (item.num_tarima != null && stud.Count(x => x.num_tarima == item.num_tarima) > 1)
                {
                    if (item.altura > 0)
                    {
                        var tarimas = stud.Where(x => x.num_tarima == item.num_tarima).ToList();
                        dS = tarimas.Sum(x => x.cantidad_teorica) - tarimas.Sum(x => x.pieces);
                    }
                    else
                    {
                        dS = 0;
                    }

                }
                else
                {
                    if (item.cantidad_teorica == null)
                        dS = null;
                    else
                        dS = item.cantidad_teorica - item.pieces;
                }
                //validacion
                string validacion = "Ajustar";
                //valida si es multiple

                if (item.num_tarima != null && stud.Count(x => x.num_tarima == item.num_tarima) > 1)
                {
                    var tarimas = stud.Where(x => x.num_tarima == item.num_tarima).ToList();
                    if (tarimas.Sum(x => x.cantidad_teorica) >= tarimas.Sum(x => x.total_piezas_min) && tarimas.Sum(x => x.cantidad_teorica) <= tarimas.Sum(x => x.total_piezas_max))
                        validacion = "(Múltiple) Dentro de tolerancias";
                    else
                        validacion = "(Múltiple) Ajustar";
                }
                else
                {
                    if (item.altura == null)
                        validacion = "Pendiente";
                    else if (item.cantidad_teorica != null && item.cantidad_teorica >= item.total_piezas_min && item.cantidad_teorica <= item.total_piezas_max)
                        validacion = "Dentro de tolerancias";
                    else if (item.cantidad_teorica == null || item.total_piezas_min == null || item.total_piezas_max == null)
                        validacion = "--";
                }

                newList.Add(new CI_conteo_table
                {
                    acciones = string.Format("<button onclick=editar({0})>Editar</button>", item.id),
                    plant = item.plant,
                    storage_location = item.storage_location,
                    storage_bin = item.storage_bin,
                    batch = item.batch,
                    material = item.material,
                    pieces = item.pieces,
                    altura = item.altura,
                    espesor = item.espesor,
                    cantidad_teorica = item.cantidad_teorica,
                    diferencia_sap = dS.HasValue ? Math.Round(dS.Value, 2) : dS,
                    validacion = validacion,
                    numTarimaString = item.num_tarima == null ? string.Empty : "T" + item.num_tarima.Value.ToString("D04")
                });

            }

            //The magic happens here
            dataTable.data = newList;

            //Se crear una referencia a JavaScriptSerializer
            var serializer = new JavaScriptSerializer();
            //Se cambia el Length directo a nuestra referencia
            serializer.MaxJsonLength = 500000000;

            var json = Json(dataTable, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;

            return json;
        }

        /// <summary>
        /// Carga los materiales del inventario, según la planta solicitada
        /// </summary>
        /// <param name="planta_sap"></param>
        /// <returns></returns>
        public JsonResult CargaInventario(string planta_sap)
        {

            //obtiene el listado de item tipo rollo de la solicitud
            var data = db.CI_conteo_inventario.Where(x => x.plant == planta_sap).ToList();

            var jsonData = new object[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {                       

                jsonData[i] = new[] {
                    data[i].id.ToString(),
                    data[i].plant,
                    data[i].storage_location,
                    data[i].storage_bin,
                    data[i].batch,
                    data[i].material,                 
                    data[i].num_tarima.ToString(),
                    data[i].pieces.ToString(),
                    data[i].altura.ToString(),
                    data[i].espesor.ToString(),
                    data[i].cantidad_teorica.ToString(),
                    data[i].diferencia_sap.ToString(),
                    data[i].validacion,
                    
                  
                };
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // GET: Bom/restablecer/5
        public ActionResult restablecer()
        {
            if (TieneRol(TipoRoles.CI_CONTEO_INVENTARIO))
            {
                ViewBag.codigo_sap = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo == true), nameof(plantas.codigoSap), nameof(plantas.ConcatPlantaSap)));
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Dante/restablcer/5
        [HttpPost]
        public ActionResult restablecer(int? id, ExcelViewInventarioSAPModel excelViewModel)
        {
            try
            {
                db.Database.ExecuteSqlCommand("UPDATE ci_conteo_inventario set altura=null, espesor=null, num_tarima=null, id_empleado=null where plant=" + excelViewModel.codigo_sap);
                TempData["Mensaje"] = new MensajesSweetAlert("Se restablecieron los datos del inventario.", TipoMensajesSweetAlerts.SUCCESS);
            }
            catch (Exception e)
            {
                if (string.IsNullOrEmpty(excelViewModel.codigo_sap))
                    TempData["Mensaje"] = new MensajesSweetAlert("No se indicó ninguna planta.", TipoMensajesSweetAlerts.WARNING);
                else
                    TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }
            return RedirectToAction("Index");
        }


        // GET: Bom/carga_inventario_sap/5
        public ActionResult carga_inventario_sap()
        {
            if (!TieneRol(TipoRoles.CI_CONTEO_INVENTARIO))
                return View("../Home/ErrorPermisos");


            ViewBag.codigo_sap = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo == true), nameof(plantas.codigoSap), nameof(plantas.ConcatPlantaSap)));
            return View();

        }

        // POST: Dante/CargaBom/5
        [HttpPost]
        public ActionResult carga_inventario_sap(ExcelViewInventarioSAPModel excelViewModel)
        {

            //Lee el archivo EXCEL

            HttpPostedFileBase stream = Request.Files["PostedFile"];

            if (stream.InputStream.Length > 8388608)
            {
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 8MB: " + excelViewModel.PostedFile.FileName);
            }
            else
            {
                string extension = Path.GetExtension(excelViewModel.PostedFile.FileName);
                if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                {
                    ModelState.AddModelError("", "Sólo se permiten archivos Excel.");
                }
            }

            bool estructuraValida = false;
            string mensaje = string.Empty;
            //el archivo es válido
            List<CI_conteo_inventario> lista = UtilExcel.LeeInventarioSAP(excelViewModel.PostedFile, ref estructuraValida, ref mensaje);
            //obtiene el listado actual de la BD
            List<CI_conteo_inventario> lista_cantidad_BD = db.CI_conteo_inventario.Where(x => x.plant == excelViewModel.codigo_sap).ToList();


            //quita los repetidos
            lista = lista.Distinct().ToList();


            if (!estructuraValida)
            {
                ModelState.AddModelError("", "El archivo seleccionado no cumple con la estructura válida.");
            }

            //verifica que el codigo sap sea el mismo
            if (lista.Any() && lista.FirstOrDefault().plant != excelViewModel.codigo_sap)
                ModelState.AddModelError("", "La planta seleccionada no coincide con la planta dentro del archivo.");


            if (ModelState.IsValid)
            {
                //lee el archivo seleccionado
                try
                {
                    //Recorre todas las cantidades de la tabla
                    for (int i = 0; i < lista.Count; i++)
                    {
                        CI_conteo_inventario itemBD = lista_cantidad_BD.FirstOrDefault(x =>
                                                    x.plant == lista[i].plant
                                                    && x.material == lista[i].material
                                                    && x.batch == lista[i].batch
                                                );
                        //EXISTE
                        if (itemBD != null)
                        {
                            //UPDATE
                            if (itemBD.pieces != lista[i].pieces
                                || itemBD.unrestricted != lista[i].unrestricted
                                || itemBD.blocked != lista[i].blocked
                                || itemBD.in_quality != lista[i].in_quality
                                || itemBD.value_stock != lista[i].value_stock
                                )
                            {
                                itemBD.pieces = lista[i].pieces;
                                itemBD.unrestricted = lista[i].unrestricted;
                                itemBD.blocked = lista[i].blocked;
                                itemBD.in_quality = lista[i].in_quality;
                                itemBD.value_stock = lista[i].value_stock;
                            }

                        }
                        else  //CREATE
                        {
                            db.CI_conteo_inventario.Add(lista[i]);
                        }
                    }

                    //DELETE
                    //elimina aquellos que no aparezcan en los enviados
                    List<CI_conteo_inventario> toDeleteList = lista_cantidad_BD.Where(x => !lista.Any(y => y.plant == x.plant
                                    && y.material == x.material
                                    && y.batch == x.batch
                                    )).ToList();

                    db.CI_conteo_inventario.RemoveRange(toDeleteList);

                    db.SaveChanges();


                    //llamada a metodo que calcula y actualiza los valores de neto y bruto sap                     

                    TempData["Mensaje"] = new MensajesSweetAlert("Los datos se han cargado correctamente.", TipoMensajesSweetAlerts.INFO);
                    return RedirectToAction("index");

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    ViewBag.codigo_sap = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo == true), nameof(plantas.codigoSap), nameof(plantas.ConcatPlantaSap)), selected: excelViewModel.codigo_sap.ToString());

                    return View(excelViewModel);
                }

            }
            ViewBag.codigo_sap = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo == true), nameof(plantas.codigoSap), nameof(plantas.ConcatPlantaSap)), selected: excelViewModel.codigo_sap.ToString());

            return View(excelViewModel);
        }

        // GET: Bom/carga_tolerancias/5
        public ActionResult carga_tolerancias()
        {
            if (TieneRol(TipoRoles.CI_CONTEO_INVENTARIO))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: Dante/CargaBom/5
        [HttpPost]
        public ActionResult carga_tolerancias(ExcelViewInventarioSAPModel excelViewModel)
        {

            HttpPostedFileBase stream = Request.Files["PostedFile"];

            if (stream.InputStream.Length > 8388608)
            {
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 8MB: " + excelViewModel.PostedFile.FileName);
            }
            else
            {
                string extension = Path.GetExtension(excelViewModel.PostedFile.FileName);
                if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                {
                    ModelState.AddModelError("", "Sólo se permiten archivos Excel.");
                }
            }

            bool estructuraValida = false;
            //el archivo es válido
            List<CI_Tolerancias> lista = UtilExcel.LeeToleranciasSAP(excelViewModel.PostedFile, ref estructuraValida);
            //obtiene el listado actual de la BD
            List<CI_conteo_inventario> lista_cantidad_BD = db.CI_conteo_inventario.ToList();


            //quita los repetidos
            lista = lista.Distinct().ToList();

            if (!estructuraValida)
            {
                ModelState.AddModelError("", "El archivo seleccionado no cumple con la estructura válida.");

            }

            if (ModelState.IsValid)
            {



                //lee el archivo seleccionado
                try
                {


                    //Recorre todas las cantidades de la tabla
                    for (int i = 0; i < lista.Count; i++)
                    {
                        List<CI_conteo_inventario> itemBDList = lista_cantidad_BD.Where(x =>
                                                    x.material == lista[i].material
                                                ).ToList();

                        //actualiza si es necesario
                        foreach (var item in itemBDList)
                        {
                            if (lista[i].gauge != item.gauge || lista[i].gauge_min != item.gauge_min || lista[i].gauge_max != item.gauge_max)
                            {
                                item.gauge = lista[i].gauge;
                                item.gauge_min = lista[i].gauge_min;
                                item.gauge_max = lista[i].gauge_max;
                            }
                        }
                    }

                    db.SaveChanges();


                    //llamada a metodo que calcula y actualiza los valores de neto y bruto sap                     

                    TempData["Mensaje"] = new MensajesSweetAlert("Los datos se han cargado correctamente.", TipoMensajesSweetAlerts.INFO);
                    return RedirectToAction("index");


                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    return View(excelViewModel);
                }
            }
            return View(excelViewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Exportar()
        {
            if (!TieneRol(TipoRoles.CI_CONTEO_INVENTARIO))
                return View("../Home/ErrorPermisos");

            var listado = db.CI_conteo_inventario
                    .OrderBy(x => x.id)
                  .ToList();

            byte[] stream = ExcelUtil.GeneraReporteConteoInventario(listado);


            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = "Reporte_conteo_inventario_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(stream, "application/vnd.ms-excel");


        }
    }


    public class DataTable
    {
        public List<CI_conteo_table> data { get; set; }
    }

    public class CI_conteo_table
    {
        public int id { get; set; }
        public string plant { get; set; }
        public string storage_location { get; set; }
        public string storage_bin { get; set; }
        public string batch { get; set; }
        //public string ship_to_number { get; set; }
        public string material { get; set; }
        //public string material_description { get; set; }
        //public string ihs_number { get; set; }
        public Nullable<int> pieces { get; set; }
        //public Nullable<int> unrestricted { get; set; }
        //public Nullable<int> blocked { get; set; }
        //public Nullable<int> in_quality { get; set; }
        //public Nullable<double> value_stock { get; set; }
        public string base_unit_measure { get; set; }
        //public Nullable<double> gauge { get; set; }
        //public Nullable<double> gauge_min { get; set; }
        //public Nullable<double> gauge_max { get; set; }
        public Nullable<double> altura { get; set; }
        public Nullable<double> espesor { get; set; }
        public Nullable<int> num_tarima { get; set; }
        public double? cantidad_teorica { get; set; }
        public double? diferencia_sap { get; set; }
        public string validacion { get; set; }
        public string numTarimaString { get; set; }
        public string acciones { get; set; }


    }
}
