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
    [Authorize]
    public class class_v3Controller : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: class_v3
        public ActionResult Index(string material, int pagina = 1)
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //en caso de que material este vacio o contenga el parameto material

                var lista = db.class_v3.Where(x => String.IsNullOrEmpty(material) || x.Object.Contains(material)).OrderBy(x => x.Object)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();
                var totalDeRegistros = db.class_v3.Where(x => String.IsNullOrEmpty(material) || x.Object.Contains(material)).Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;

                return View(lista);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }
        // GET: Bom/CargaClass/5
        public ActionResult CargaClass()
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: Dante/CargaClass/5
        [HttpPost]
        public ActionResult CargaClass(ExcelViewModel excelViewModel, FormCollection collection)
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
                    List<class_v3> lista = UtilExcel.LeeClass(excelViewModel.PostedFile, ref estructuraValida);


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
                        int eliminados = 0;


                        List<class_v3> listAnterior = db.class_v3.ToList();

                        //determina que elementos de la lista no se encuentran en la lista anterior
                        List<class_v3> listDiferencias = lista.Except(listAnterior).ToList();

                        foreach (class_v3 mm in listDiferencias)
                        {
                            try
                            {
                                //obtiene el elemento de BD
                                class_v3 item = db.class_v3.FirstOrDefault(x => x.Object == mm.Object );

                                //si existe actualiza
                                if (item != null)
                                {
                                    db.Entry(item).CurrentValues.SetValues(mm);
                                    db.SaveChanges();
                                    actualizados++;
                                }
                                else
                                {
                                    //crea un nuevo registro
                                    db.class_v3.Add(mm);
                                    db.SaveChanges();
                                    creados++;
                                }

                            }
                            catch (Exception e)
                            {
                                error++;
                            }

                        }
                        //obtiene nuevamente la lista de BD
                        listAnterior = db.class_v3.ToList();
                        //determina que elementos de la listAnterior no se encuentran en la lista Excel
                        listDiferencias = listAnterior.Except(lista).ToList();

                        //elima de BD aquellos que no se encuentren en el excel
                        foreach (class_v3 mm in listDiferencias)
                        {
                            try
                            {
                                //obtiene el elemento de BD
                                class_v3 item = db.class_v3.FirstOrDefault(x => x.Object == mm.Object);

                                //si existe elimina
                                if (item != null)
                                {
                                    db.Entry(item).State = EntityState.Deleted;
                                    db.SaveChanges();
                                    eliminados++;
                                }

                            }
                            catch (Exception e)
                            {
                                error++;
                            }

                        }


                        TempData["Mensaje"] = new MensajesSweetAlert("Actualizados: " + actualizados + " -> Creados: " + creados + " -> Errores: " + error + " -> Eliminados: " + eliminados, TipoMensajesSweetAlerts.INFO);
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
}
