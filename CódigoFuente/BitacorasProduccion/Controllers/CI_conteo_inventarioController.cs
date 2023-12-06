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
                            //trunca la tabla
                            string cmd = $"TRUNCATE TABLE CI_conteo_inventario";
                            db.Database.ExecuteSqlCommand(cmd);

                            //agrega los nuevos registos
                            db.CI_conteo_inventario.AddRange(lista);
                            //obtiene el elemento de BD                         
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            TempData["Mensaje"] = new MensajesSweetAlert("Error: "+e.Message, TipoMensajesSweetAlerts.ERROR);
                            return RedirectToAction("index");
                        }

                        //List<bom_en_sap> listAnterior = db.bom_en_sap.ToList();

                        ////determina que elementos de la lista no se encuentran en la lista anterior
                        //List<bom_en_sap> listDiferencias = lista.Except(listAnterior).ToList();

                        //foreach (bom_en_sap bom in listDiferencias)
                        //{
                        //    try
                        //    {
                        //        //obtiene el elemento de BD
                        //        bom_en_sap item = listAnterior.FirstOrDefault(x => x.Material == bom.Material && x.Plnt == bom.Plnt && x.BOM == bom.BOM && x.AltBOM == bom.AltBOM && x.Item == bom.Item
                        //         && x.Component == bom.Component
                        //        );

                        //        //si existe actualiza
                        //        if (item != null)
                        //        {
                        //            db.Entry(item).CurrentValues.SetValues(bom);

                        //            actualizados++;
                        //        }
                        //        else
                        //        {
                        //            //crea un nuevo registro
                        //            db.bom_en_sap.Add(bom);

                        //            creados++;
                        //        }
                        //        db.SaveChanges();
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        error++;
                        //    }

                        //}
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
                        //        bom_en_sap item = listAnterior.FirstOrDefault(x => x.Material == bom.Material && x.Plnt == bom.Plnt && x.BOM == bom.BOM && x.AltBOM == bom.AltBOM && x.Item == bom.Item
                        //        && x.Component == bom.Component
                        //        );

                        //        //si existe elimina
                        //        if (item != null)
                        //        {
                        //            db.Entry(item).State = EntityState.Deleted;
                        //            eliminados++;
                        //        }
                        //        db.SaveChanges();
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        error++;
                        //    }

                        //}


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
