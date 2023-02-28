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
    public class IT_epoController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: Bom
        public ActionResult Index(int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listado = db.IT_epo
                    //.Where(x => String.IsNullOrEmpty(material) || x.Material.Contains(material))
                    .OrderBy(x => x.id)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_epo
                    //.Where(x => String.IsNullOrEmpty(material) || x.Material.Contains(material))
                    .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                //routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: Bom/LoadFile/5
        public ActionResult LoadFile()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: Dante/LoadFile/5
        [HttpPost]
        public ActionResult LoadFile(ExcelViewModel excelViewModel, FormCollection collection)
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

                    string msj = string.Empty;
                    //el archivo es válido
                    List<IT_epo> lista = UtilExcel.LeeEpo(excelViewModel.PostedFile, ref msj);

                    //quita los repetidos
                    lista = lista.Distinct().ToList();

                    if (!string.IsNullOrEmpty(msj))
                    {
                        msjError = msj;
                        throw new Exception(msjError);
                    }
                    else
                    {
                        //truncate a tabla
                        db.Database.ExecuteSqlCommand("TRUNCATE TABLE [IT_epo]");
                        //inserta nuevos registros
                        db.IT_epo.AddRange(lista);                

                        db.SaveChanges();

                        TempData["Mensaje"] = new MensajesSweetAlert("Se importó el archivo correctamente: "+lista.Count+" registros insertados.", TipoMensajesSweetAlerts.SUCCESS);
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
