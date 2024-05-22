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
    public class BomController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: Bom
        public ActionResult Index(string material, int pagina = 1)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro


                var listaBom = db.bom_en_sap.Where(x => String.IsNullOrEmpty(material) || x.Material.Contains(material)).OrderBy(x => x.Material)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.bom_en_sap.Where(x => String.IsNullOrEmpty(material) || x.Material.Contains(material)).Count();

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

                return View(listaBom);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: Bom/CargaBom/5
        public ActionResult CargaBom()
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

        // POST: Dante/CargaBom/5
        [HttpPost]
        public ActionResult CargaBom(ExcelViewModel excelViewModel, FormCollection collection)
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
                    List<bom_en_sap> lista = UtilExcel.LeeBom(excelViewModel.PostedFile, ref estructuraValida);

                    //quita los repetidos
                    lista = lista.Distinct().ToList();

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


                        List<bom_en_sap> listAnterior = db.bom_en_sap.ToList();

                        //determina que elementos de la lista no se encuentran en la lista anterior
                        List<bom_en_sap> listDiferencias = lista.Except(listAnterior).ToList();

                        foreach (bom_en_sap bom in listDiferencias)
                        {
                            try
                            {
                                //obtiene el elemento de BD
                                bom_en_sap item = listAnterior.FirstOrDefault(x => x.Material == bom.Material && x.Plnt == bom.Plnt && x.BOM == bom.BOM && x.AltBOM == bom.AltBOM && x.Item == bom.Item
                                 && x.Component == bom.Component
                                );

                                //si existe actualiza
                                if (item != null)
                                {
                                    db.Entry(item).CurrentValues.SetValues(bom);

                                    actualizados++;
                                }
                                else
                                {
                                    //crea un nuevo registro
                                    db.bom_en_sap.Add(bom);

                                    creados++;
                                }
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                error++;
                            }

                        }
                        //obtiene nuevamente la lista de BD
                        listAnterior = db.bom_en_sap.ToList();
                        //determina que elementos de la listAnterior no se encuentran en la lista Excel
                        listDiferencias = listAnterior.Except(lista).ToList();

                        //elima de BD aquellos que no se encuentren en el excel
                        foreach (bom_en_sap bom in listDiferencias)
                        {
                            try
                            {
                                //obtiene el elemento de BD
                                bom_en_sap item = listAnterior.FirstOrDefault(x => x.Material == bom.Material && x.Plnt == bom.Plnt && x.BOM == bom.BOM && x.AltBOM == bom.AltBOM && x.Item == bom.Item
                                && x.Component == bom.Component
                                );

                                //si existe elimina
                                if (item != null)
                                {
                                    db.Entry(item).State = EntityState.Deleted;
                                    eliminados++;
                                }
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                error++;
                            }
                        }
                        

                        //actualiza los pesos del bom

                        #region PesosDeBOM

                        List<bom_en_sap> listadoBOM = db.bom_en_sap.ToList();
                        List<bom_pesos> listadosPesosHistorico = db.bom_pesos.ToList();
                        List<bom_pesos> nuevoListadoPesosHistorico = new List<bom_pesos>();

                        foreach (var item in listadoBOM)
                        {
                            List<bom_en_sap> listTemporalBOM = listadoBOM.Where(x => x.Plnt == item.Plnt && x.Material == item.Material).ToList();

                            DateTime? fechaCreacion = null, fechaUso = null;
                            double? peso_neto;
                            double? peso_bruto;

                            if (listTemporalBOM.Count > 0)
                            {
                                fechaCreacion = listTemporalBOM.OrderByDescending(x => x.Created).FirstOrDefault().Created;
                                fechaUso = listTemporalBOM.OrderByDescending(x => x.LastDateUsed).FirstOrDefault().LastDateUsed;
                            }

                            if (fechaUso.HasValue)
                            {
                                //obtiene el peso bruto (el de mayor peso)
                                peso_bruto = listTemporalBOM.Where(x => x.LastDateUsed == fechaUso).Max(x => x.Quantity);

                                //si el peso bruto aparece dos veces, lo marca como valores duplicados
                                bool duplicado = listTemporalBOM.Where(x => x.LastDateUsed == fechaUso && x.Quantity == peso_bruto).Count() > 1;

                                if (duplicado)
                                {
                                    //obtiene el peso quitando los duplicados
                                    peso_neto = listTemporalBOM.Where(x => x.LastDateUsed == fechaUso && x.Quantity != (-0.001)).Select(x => x.Quantity).Distinct().Sum();
                                }
                                else
                                {
                                    //peso bruto + los negativos
                                    peso_neto = peso_bruto + listTemporalBOM.Where(x => x.LastDateUsed == fechaUso && x.Quantity < (-0.001)).Sum(x => x.Quantity);

                                    //se agregan excepciones conocidas
                                    switch (item.Material)
                                    {
                                        case "HD10928": //no tiene scrap
                                            peso_bruto = peso_neto;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                peso_bruto = listTemporalBOM.Where(x => x.Created == fechaCreacion).Max(x => x.Quantity);
                                //peso bruto + los negativos
                                peso_neto = peso_bruto + listTemporalBOM.Where(x => x.Created == fechaCreacion && x.Quantity < (-0.001)).Sum(x => x.Quantity);
                            }

                            //si existe el peso, lo modifica
                            bom_pesos peso = listadosPesosHistorico.FirstOrDefault(x => x.plant == item.Plnt && x.material == item.Material);

                            if (peso != null && peso_neto.HasValue && peso_bruto.HasValue)
                            {
                                //valida si hubo un cambio de peso 
                                if (peso.net_weight != peso_neto.Value || peso.gross_weight != peso_bruto.Value)
                                {
                                    peso.net_weight = peso_neto.Value;
                                    peso.gross_weight = peso_bruto.Value;
                                }
                            }
                            else if(peso_neto.HasValue && peso_bruto.HasValue)
                            {
                                //si no existe, verifica si ya existe en el listado new o si no lo agrega
                                if (!nuevoListadoPesosHistorico.Any(x => x.plant == item.Plnt && x.material == item.Material))
                                {
                                    nuevoListadoPesosHistorico.Add(new bom_pesos
                                    {
                                        plant = item.Plnt,
                                        material = item.Material,
                                        gross_weight = peso_bruto.Value,
                                        net_weight = peso_neto.Value,
                                    });

                                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[bom_pesos]([plant],[material],[gross_weight],[net_weight]) VALUES('"+item.Plnt+"','"
                                        +item.Material+"',"+peso_bruto.Value+","+peso_neto.Value+")");
                                    //ejecuta sentancia sql
                                }
                            }


                        }


                        //if (nuevoListadoPesosHistorico.Count > 0)
                        //{
                        //    db.bom_pesos.AddRange(nuevoListadoPesosHistorico);
                        //}
                        db.SaveChanges();


                        #endregion



                        //llamada a metodo que calcula y actualiza los valores de neto y bruto sap                     

                        //TempData["Mensaje"] = new MensajesSweetAlert("Actualizados: " + actualizados + " -> Creados: " + creados + " -> Errores: " + error + " -> Eliminados: " + eliminados, TipoMensajesSweetAlerts.INFO);
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
