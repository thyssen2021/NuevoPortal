using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
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

            cI_conteo_inventario.etiquetas = db.CI_conteo_inventario.Where(x=> cI_conteo_inventario.num_tarima!=null && x.num_tarima == cI_conteo_inventario.num_tarima).ToList();

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

            if(cI_conteo_inventario.altura == null || cI_conteo_inventario.altura == 0)
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
                
                var tarimas = db.CI_conteo_inventario.Where(x=> tarimasFormIds.Contains(x.id)).Select(x => x.num_tarima).Distinct().ToList();
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
                    else {
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
                var tarimasBD = db.CI_conteo_inventario.Where(x => ciBD.num_tarima!=null && x.num_tarima == ciBD.num_tarima && x.id!=ciBD.id);
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
                    diferencia_sap = item.diferencia_sap,
                    validacion = item.validacion,
                    numTarimaString = item.num_tarima == null ? string.Empty : "T" + item.num_tarima.Value.ToString("D04")
                });


            }

            //The magic happens here
            dataTable.data = newList;
            return Json(dataTable, JsonRequestBehavior.AllowGet);
        }



        // GET: Bom/restablecer/5
        public ActionResult restablecer()
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

        // POST: Dante/restablcer/5
        [HttpPost]
        public ActionResult restablecer(int? id)
        {
            try {
                db.Database.ExecuteSqlCommand("UPDATE ci_conteo_inventario set altura=null, espesor=null, num_tarima=null, id_empleado=null");
                TempData["Mensaje"] = new MensajesSweetAlert("Se restablecieron los datos del inventario.", TipoMensajesSweetAlerts.SUCCESS);
            }
            catch(Exception e) {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: "+e.Message, TipoMensajesSweetAlerts.ERROR);
            }
            return RedirectToAction("Index");
        }


        // GET: Bom/carga_inventario_sap/5
        public ActionResult carga_inventario_sap()
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
        public ActionResult carga_inventario_sap(ExcelViewModel excelViewModel, FormCollection collection)
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
                        if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                        {
                            msjError = "Sólo se permiten archivos Excel";
                            throw new Exception(msjError);
                        }
                    }

                    bool estructuraValida = false;
                    //el archivo es válido
                    List<CI_conteo_inventario> lista = UtilExcel.LeeInventarioSAP(excelViewModel.PostedFile, ref estructuraValida);
                    //obtiene el listado actual de la BD
                    List<CI_conteo_inventario> lista_cantidad_BD = db.CI_conteo_inventario.ToList();


                    //quita los repetidos
                    lista = lista.Distinct().ToList();

                    if (!estructuraValida)
                    {
                        msjError = "No cumple con la estructura válida.";
                        throw new Exception(msjError);
                    }
                    else
                    {


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

                        }
                        catch (Exception e)
                        {
                            TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                            return RedirectToAction("index");
                        }



                        //llamada a metodo que calcula y actualiza los valores de neto y bruto sap                     

                        TempData["Mensaje"] = new MensajesSweetAlert("Los datos se han cargado correctamente.", TipoMensajesSweetAlerts.INFO);
                        return RedirectToAction("index");
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
        public ActionResult carga_tolerancias(ExcelViewModel excelViewModel, FormCollection collection)
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
                        if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                        {
                            msjError = "Sólo se permiten archivos Excel";
                            throw new Exception(msjError);
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
                        msjError = "No cumple con la estructura válida.";
                        throw new Exception(msjError);
                    }
                    else
                    {


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

                        }
                        catch (Exception e)
                        {
                            TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                            return RedirectToAction("index");
                        }



                        //llamada a metodo que calcula y actualiza los valores de neto y bruto sap                     

                        TempData["Mensaje"] = new MensajesSweetAlert("Los datos se han cargado correctamente.", TipoMensajesSweetAlerts.INFO);
                        return RedirectToAction("index");
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
            if (!TieneRol(TipoRoles.CI_CONTEO_INVENTARIO) )
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
