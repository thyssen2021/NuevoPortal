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
    public class mm_v3Controller : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: mm_v3
        public ActionResult Index(string material, string tipoMaterial, int pagina = 1)
        {
            if (TieneRol(TipoRoles.ADMIN) || TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //en caso de que material este vacio o contenga el parameto material              

                var listaBom = db.mm_v3.Where(
                        x => (String.IsNullOrEmpty(material) || x.Material.Contains(material))
                        && (String.IsNullOrEmpty(tipoMaterial) || x.Type_of_Material.Contains(tipoMaterial))
                        )
                    .OrderBy(x => x.Material)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.mm_v3.Where(x => (String.IsNullOrEmpty(material) || x.Material.Contains(material))
                       && (String.IsNullOrEmpty(tipoMaterial) || x.Type_of_Material.Contains(tipoMaterial))).Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["material"] = material;
                routeValues["tipoMaterial"] = tipoMaterial;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;

                return View(listaBom);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: Bom/CargaMM/5
        public ActionResult CargaMM()
        {
            if (TieneRol(TipoRoles.ADMIN) || TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: Dante/CargaMM/5
        [HttpPost]
        public ActionResult CargaMM(ExcelViewModel excelViewModel, FormCollection collection)
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
                    List<mm_v3> lista = UtilExcel.LeeMM(excelViewModel.PostedFile, ref estructuraValida);


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


                        List<mm_v3> listAnterior = db.mm_v3.ToList();

                        //determina que elementos de la lista no se encuentran en la lista anterior
                        List<mm_v3> listDiferencias = lista.Except(listAnterior).ToList();

                        foreach (mm_v3 mm in listDiferencias)
                        {
                            try
                            {
                                //obtiene el elemento de BD
                                mm_v3 item = listAnterior.FirstOrDefault(x => x.Material == mm.Material && x.Plnt == mm.Plnt );

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
                                    db.mm_v3.Add(mm);
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
                        listAnterior = db.mm_v3.ToList();
                        //determina que elementos de la listAnterior no se encuentran en la lista Excel
                        listDiferencias = listAnterior.Except(lista).ToList();

                        //elima de BD aquellos que no se encuentren en el excel
                        foreach (mm_v3 mm in listDiferencias)
                        {
                            try
                            {
                                //obtiene el elemento de BD
                                mm_v3 item = db.mm_v3.FirstOrDefault(x => x.Material == mm.Material && x.Plnt == mm.Plnt );

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
