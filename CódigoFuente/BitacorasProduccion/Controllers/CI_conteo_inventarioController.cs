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

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            List<CI_conteo_inventario> lista = new List<CI_conteo_inventario>();
            return View(lista);
        }

     
        // GET: CI_conteo_inventario/Edit/5
        public ActionResult Edit(int? id)
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
            return View(cI_conteo_inventario);
        }

        // POST: CI_conteo_inventario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CI_conteo_inventario cI_conteo_inventario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cI_conteo_inventario).State = EntityState.Modified;
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
            List<CI_conteo_inventario> stud = db.CI_conteo_inventario.ToList();
            
            foreach (var item in stud)
            {
                item.acciones = @" <button onclick=""editar("+item.id+@")"" class=""btn btn-success btn-sm btn-block"">
                                                            <i class=""fa-regular fa-pen-to-square""></i>
                                                        </button>";
            }

            //The magic happens here
            dataTable.data = stud;
            return Json(dataTable, JsonRequestBehavior.AllowGet);
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
                                        ) {
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

                        }
                        catch (Exception e)
                        {
                            TempData["Mensaje"] = new MensajesSweetAlert("Error: "+e.Message, TipoMensajesSweetAlerts.ERROR);
                            return RedirectToAction("index");
                        }



                        //llamada a metodo que calcula y actualiza los valores de neto y bruto sap                     

                        TempData["Mensaje"] = new MensajesSweetAlert("Los cambios han sido guardados correctamente.", TipoMensajesSweetAlerts.INFO);
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
    }

    public class DataTable
    {
        public List<CI_conteo_inventario> data { get; set; }
    }
}
