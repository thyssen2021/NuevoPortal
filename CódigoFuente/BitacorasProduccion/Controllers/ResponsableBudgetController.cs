using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using DocumentFormat.OpenXml;
using Newtonsoft.Json;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class ResponsableBudgetController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: ResponsableBudget/Actual
        public ActionResult Centros()
        {
            if (TieneRol(TipoRoles.BG_RESPONSABLE))
            {
                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }


                var centros = db.budget_centro_costo.Where(x => x.budget_responsables.Any(y => y.id_responsable == empleado.id));
                return View(centros.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        

        // GET: ResponsableBudget/EditCentroPresente
        public ActionResult EditCentroPresenteHT(int? id, int? id_fiscal_year, bool proximo = false, bool info = false, bool controlling = false, bool import = false)
        {

            if (TieneRol(TipoRoles.BG_RESPONSABLE) || TieneRol(TipoRoles.BG_CONTROLLING))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }
                //obtiene centro de costo
                budget_centro_costo centroCosto = db.budget_centro_costo.Find(id);
                if (centroCosto == null)
                    return View("../Error/NotFound");

                //obtiene el usuario logeado
                empleados empleado = obtieneEmpleadoLogeado();
                //verifica que el usuario este registrado como capturista
                if (!db.budget_responsables.Any(x => x.id_responsable == empleado.id && centroCosto.id == x.id_budget_centro_costo) && !TieneRol(TipoRoles.BG_CONTROLLING))
                {
                    ViewBag.Titulo = "¡Lo sentimos!¡No se puede acceder a esta Sección!";
                    ViewBag.Descripcion = "Este usuario no se encuentra asociado a este centro de costo.";

                    return View("../Home/ErrorGenerico");
                }

                //obtiene el año fiscal anterior (actual)
                budget_anio_fiscal anio_Fiscal_anterior = GetAnioFiscal(DateTime.Now.AddYears(-1));
                if (anio_Fiscal_anterior == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal actual (forecast)
                budget_anio_fiscal anio_Fiscal_actual = GetAnioFiscal(DateTime.Now);
                if (anio_Fiscal_actual == null)
                    return View("../Error/NotFound");

                //obtiene el año fiscal proximo (budget)
                budget_anio_fiscal anio_Fiscal_proximo = GetAnioFiscal(DateTime.Now.AddYears(1));
                if (anio_Fiscal_proximo == null)
                    return View("../Error/NotFound");

                //cambia los años ficales si la vista es controlling
                if (controlling && id_fiscal_year.HasValue)
                {
                    //presente
                    anio_Fiscal_actual = db.budget_anio_fiscal.Find(id_fiscal_year);
                    //anterior
                    anio_Fiscal_anterior = db.budget_anio_fiscal.FirstOrDefault(x => x.anio_inicio == anio_Fiscal_actual.anio_inicio - 1);
                    //proximo
                    anio_Fiscal_proximo = db.budget_anio_fiscal.FirstOrDefault(x => x.anio_inicio == anio_Fiscal_actual.anio_inicio + 1);
                }


                //obtiene el id_rel_centro_costo anterior
                budget_rel_fy_centro rel_fy_centro_anterior = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_anterior.id).FirstOrDefault();
                if (rel_fy_centro_anterior == null)
                {
                    //si no existe crea el rel anio forecast
                    rel_fy_centro_anterior = new budget_rel_fy_centro
                    {
                        id_anio_fiscal = anio_Fiscal_anterior.id,
                        id_centro_costo = centroCosto.id,
                        estatus = true //activado por defecto
                    };

                    db.budget_rel_fy_centro.Add(rel_fy_centro_anterior);
                    //guarda en base de datos el centro creado
                    db.SaveChanges();
                }

                //obtiene el id_rel_centro_costo del forecast
                budget_rel_fy_centro rel_fy_centro_presente = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_actual.id).FirstOrDefault();
                if (rel_fy_centro_presente == null)
                {
                    //si no existe crea el rel anio forecast
                    rel_fy_centro_presente = new budget_rel_fy_centro
                    {
                        id_anio_fiscal = anio_Fiscal_actual.id,
                        id_centro_costo = centroCosto.id,
                        estatus = true //activado por defecto
                    };

                    db.budget_rel_fy_centro.Add(rel_fy_centro_presente);
                    //guarda en base de datos el centro creado
                    db.SaveChanges();
                }

                //obtiene el id_rel_centro_costo del proximo
                budget_rel_fy_centro rel_fy_centro_proximo = db.budget_rel_fy_centro.Where(x => x.id_centro_costo == centroCosto.id && x.id_anio_fiscal == anio_Fiscal_proximo.id).FirstOrDefault();
                if (rel_fy_centro_proximo == null)
                {
                    //si no existe crea el rel anio forecast
                    rel_fy_centro_proximo = new budget_rel_fy_centro
                    {
                        id_anio_fiscal = anio_Fiscal_proximo.id,
                        id_centro_costo = centroCosto.id,
                        estatus = true //activado por defecto
                    };

                    db.budget_rel_fy_centro.Add(rel_fy_centro_proximo);
                    //guarda en base de datos el centro creado
                    db.SaveChanges();
                }

                //agrega el objeto de fiscal year
                rel_fy_centro_proximo.budget_anio_fiscal = anio_Fiscal_proximo;
                rel_fy_centro_presente.budget_anio_fiscal = anio_Fiscal_actual;
                rel_fy_centro_anterior.budget_anio_fiscal = anio_Fiscal_anterior;


                #region cabeceras HT
                //cabeceras HT FORECAST
                List<string> headersForecast = new List<string> { "SAP Account", "Name", "Mapping" };
                var cabeceraObject = new object[13];
                var cabeceraObjectActual = new object[13];
                var cabeceraObjectBudget = new object[13];
                cabeceraObject[0] = new
                {
                    label = "SAP ACCOUNT",
                    colspan = 3
                };
                cabeceraObjectActual[0] = cabeceraObject[0];
                cabeceraObjectBudget[0] = cabeceraObject[0];
                DateTime fecha = new DateTime(rel_fy_centro_presente.budget_anio_fiscal.anio_inicio, rel_fy_centro_presente.budget_anio_fiscal.mes_inicio, 1);
                for (int i = 0; i < 12; i++)
                {
                    //agrega cabecera para tipo moneda
                    headersForecast.Add("MXN");
                    headersForecast.Add("USD");
                    headersForecast.Add("EUR");
                    headersForecast.Add("Local (USD)");
                    //agrega cabecera para meses
                    cabeceraObject[i + 1] = new
                    {
                        label = string.Format("{0} {1}", rel_fy_centro_presente.budget_anio_fiscal.isActual(fecha.Month), fecha.ToString("MMM yy").ToUpper()),
                        colspan = 4
                    };
                    cabeceraObjectActual[i + 1] = new
                    {
                        label = string.Format("ACT {0}", fecha.AddYears(-1).ToString("MMM yy").ToUpper()),
                        colspan = 4
                    };
                    cabeceraObjectBudget[i + 1] = new
                    {
                        label = string.Format("BG {0}", fecha.AddYears(1).ToString("MMM yy").ToUpper()),
                        colspan = 4
                    };
                    fecha = fecha.AddMonths(1);
                }
                headersForecast.Add("Total (MXN)");
                headersForecast.Add("Total (USD)");
                headersForecast.Add("Total (EUR)");
                headersForecast.Add("Total (Local - USD)");
                headersForecast.Add("Comentarios");
                headersForecast.Add("AplicaFormula");
                headersForecast.Add("aplicaMXN");
                headersForecast.Add("AplicaUSD");
                headersForecast.Add("AplicaEUR");

                ViewBag.HeadersForecast1_actual = JsonConvert.SerializeObject(cabeceraObjectActual);
                ViewBag.HeadersForecast1_budget = JsonConvert.SerializeObject(cabeceraObjectBudget);
                ViewBag.HeadersForecast1 = JsonConvert.SerializeObject(cabeceraObject);
                ViewBag.HeadersForecast2 = headersForecast.ToArray();

                #endregion

                ViewBag.centroCosto = centroCosto;
                ViewBag.rel_anterior = rel_fy_centro_anterior;
                ViewBag.rel_presente = rel_fy_centro_presente;
                ViewBag.rel_proximo = rel_fy_centro_proximo;
                ViewBag.numCuentasSAP = db.budget_cuenta_sap.Where(x => x.activo).Count();

                //en caso de que sea próximo cambia la vista
                if (proximo && rel_fy_centro_proximo.estatus) //proximo editable
                    return View("EditCentroProximoHT", centroCosto);
                else if (proximo && !rel_fy_centro_proximo.estatus) //proximo no editable
                {
                    ViewBag.proximo = proximo;
                    return View("DetailsCentroHT", centroCosto);
                }
                else if (info || import) //muestra la vista de Info //info
                {
                    ViewBag.proximo = proximo;
                    ViewBag.import = import;
                    return View("DetailsCentroHT", centroCosto);
                }
                else if (!proximo && !rel_fy_centro_presente.estatus)
                {   //presente no editable
                    ViewBag.proximo = proximo;
                    return View("DetailsCentroHT", centroCosto);
                }

                ViewBag.controlling = controlling;
                return View(centroCosto); //presente  editable

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }



        // POST: ResponsableBudget/EditCentro
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCentro(FormCollection form)
        {
            //crea objetos apartir del form collection

            int id_rel_fy = 0;
            int.TryParse(form["id_rel_fy_centro"], out id_rel_fy);

            bool continuar = false;
            Boolean.TryParse(form["continuar"], out continuar);


            //1.-Obtiene los keys de tipo cantidad
            List<string> keysCantidades = form.AllKeys.Where(x => x.ToUpper().Contains("CANTIDAD") == true).ToList();

            //2.- Recorre y crea un objeto de tipo budget_valores
            List<budget_cantidad> valores = new List<budget_cantidad>();

            foreach (var keyC in keysCantidades)
            {
                Match m = Regex.Match(keyC, "(\\d+)");
                string num = string.Empty;

                //extrae el numero de key
                if (m.Success)
                {
                    num = m.Value;

                    int id_cuenta_sap = 0;
                    int mes = 0;
                    decimal cantidad = 0;

                    //obtiene el valor de la cuenta sap
                    bool success_id_cuenta_sap = int.TryParse(form["budget_valores[" + num + "].id_cuenta_sap"], out id_cuenta_sap);
                    bool success_mes = int.TryParse(form["budget_valores[" + num + "].mes"], out mes);
                    bool succes_cantidad = decimal.TryParse(form["budget_valores[" + num + "].cantidad"], out cantidad);
                    string currency = "USD";  //currency por defecto

                    //verifica que no hubo errores al covertir los datos
                    if (success_id_cuenta_sap && success_mes)
                    {
                        //crea y agrega un objeto de tipo budget_valores con la información leida
                        valores.Add(new budget_cantidad
                        {
                            id_budget_rel_fy_centro = id_rel_fy,
                            id_cuenta_sap = id_cuenta_sap,
                            mes = mes,
                            currency_iso = currency,
                            cantidad = cantidad,
                            comentario = string.IsNullOrEmpty(form["budget_valores[" + num + "].comentario"]) ? null : form["budget_valores[" + num + "].comentario"]
                        });
                    }

                }
            }

            //lista para valores de meses segun la fecha actual
            List<int> mesesPendientes = new List<int>();

            //3.- identificar cuales son update, create, delete y sin cambios

            //obtiene los valores actuales en BD
            List<budget_cantidad> valoresEnBD = db.budget_cantidad.Where(x => x.id_budget_rel_fy_centro == id_rel_fy).ToList();

            //obtiene valores sin cambios
            List<budget_cantidad> valoresSinCambio = valores.Where(x => valoresEnBD.Any(y => y.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro && y.id_cuenta_sap == x.id_cuenta_sap && y.mes == x.mes && y.comentario == x.comentario && y.cantidad == x.cantidad)).ToList();

            //obtiene valores con cambios o nuevos
            List<budget_cantidad> valoresDiferentes = valores.Except(valoresSinCambio).ToList();

            //DE valores Diferentes identificar cuales son create, update y delete
            List<budget_cantidad> valoresCreate = valoresDiferentes.Where(a => !valoresEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes) && (a.cantidad != 0 || !string.IsNullOrEmpty(a.comentario))).ToList();

            //DE valores Diferentes identificar cuales son create, update y delete
            List<budget_cantidad> valoresUpdate = valoresDiferentes.Where(a => valoresEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes && (a.cantidad != 0 || !string.IsNullOrEmpty(a.comentario)))).ToList();
            //agrega el id correspondiente
            valoresUpdate = valoresUpdate.Select(x =>
            {
                x.id = (from v in valoresEnBD
                        where v.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro && v.id_cuenta_sap == x.id_cuenta_sap && v.mes == x.mes
                        select v.id).FirstOrDefault(); return x;
            }).ToList();

            //DE valores Diferentes identificar cuales son create, update y delete
            List<budget_cantidad> valoresDelete = valoresDiferentes.Where(a => valoresEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a1.mes == a.mes && a.cantidad == 0 && string.IsNullOrEmpty(a.comentario))).ToList();
            //agrega el id  correspondiente
            valoresDelete = valoresDelete.Select(x =>
            {
                x.id = (from v in valoresEnBD
                        where v.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro && v.id_cuenta_sap == x.id_cuenta_sap && v.mes == x.mes
                        select v.id).FirstOrDefault(); return x;
            }).ToList();

            //crea los nuevos registros
            try
            {
                db.budget_cantidad.AddRange(valoresCreate);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }

            //modifica los registros actuales
            try
            {
                foreach (budget_cantidad item in valoresUpdate)
                {
                    //obtiene el elemento de BD
                    budget_cantidad objeto = valoresEnBD.FirstOrDefault(x => x.id == item.id);

                    if (objeto != null)
                    {
                        db.Entry(objeto).CurrentValues.SetValues(item);

                    }

                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }

            //Elimina los elemntos necesarios 
            try
            {
                foreach (budget_cantidad item in valoresDelete)
                {
                    //obtiene el elemento de BD
                    budget_cantidad objeto = valoresEnBD.FirstOrDefault(x => x.id == item.id);

                    if (objeto != null)
                    {
                        db.budget_cantidad.Remove(objeto);
                    }

                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }

            #region comentarios
            //1.-Obtiene los keys de tipo comentario
            List<string> keysComentarios = form.AllKeys.Where(x => x.ToUpper().Contains(".COMENTARIO") && x.ToUpper().Contains("BUDGET_REL_COMENTARIOS")).ToList();

            //2.- Recorre y crea un objeto de tipo budget_comentarios
            List<budget_rel_comentarios> listComentarios = new List<budget_rel_comentarios>();

            foreach (var keyC in keysComentarios)
            {
                Match m = Regex.Match(keyC, "(\\d+)");
                string num = string.Empty;

                //extrae el numero de key
                if (m.Success)
                {
                    num = m.Value;

                    int id_cuenta_sap = 0;

                    //obtiene el valor de la cuenta sap
                    bool success_id_cuenta_sap = int.TryParse(form["budget_rel_comentarios[" + num + "].id_cuenta_sap"], out id_cuenta_sap);
                    string comen = form["budget_rel_comentarios[" + num + "].comentario"].ToString();


                    //verifica que no hubo errores al covertir los datos
                    if (success_id_cuenta_sap && !String.IsNullOrEmpty(comen))
                    {
                        //crea y agrega un objeto de tipo budget_valores con la información leida
                        listComentarios.Add(new budget_rel_comentarios
                        {
                            id_budget_rel_fy_centro = id_rel_fy,
                            id_cuenta_sap = id_cuenta_sap,
                            comentarios = comen
                        });
                    }

                }
            }

            //3.- identificar cuales son update, create, delete y sin cambios

            //obtiene los valores actuales en BD
            List<budget_rel_comentarios> comentariosEnBD = db.budget_rel_comentarios.Where(x => x.id_budget_rel_fy_centro == id_rel_fy).ToList();

            //valores create
            List<budget_rel_comentarios> valoresCreateComentarios = listComentarios.Where(a => !comentariosEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap) && !String.IsNullOrEmpty(a.comentarios)).ToList();

            //valores updated
            List<budget_rel_comentarios> valoresUpdateComentarios = listComentarios.Where(a => comentariosEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && a.comentarios != a1.comentarios && !String.IsNullOrEmpty(a.comentarios))).ToList();

            //agrega id a update
            valoresUpdateComentarios = valoresUpdateComentarios.Select(x =>
            {
                x.id = (from v in comentariosEnBD
                        where v.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro && v.id_cuenta_sap == x.id_cuenta_sap
                        select v.id).FirstOrDefault(); return x;
            }).ToList();

            //valores delete
            List<budget_rel_comentarios> valoresDeleteComentarios = comentariosEnBD.Where(a => !listComentarios.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap)).ToList();
            valoresDeleteComentarios.AddRange(listComentarios.Where(a => comentariosEnBD.Any(a1 => a1.id_budget_rel_fy_centro == a.id_budget_rel_fy_centro && a1.id_cuenta_sap == a.id_cuenta_sap && String.IsNullOrEmpty(a.comentarios))).ToList());

            //agrega el id a delete
            valoresDeleteComentarios = valoresDeleteComentarios.Select(x =>
            {
                x.id = (from v in comentariosEnBD
                        where v.id_budget_rel_fy_centro == x.id_budget_rel_fy_centro && v.id_cuenta_sap == x.id_cuenta_sap
                        select v.id).FirstOrDefault(); return x;
            }).ToList();

            //crea los nuevos registros
            try
            {
                db.budget_rel_comentarios.AddRange(valoresCreateComentarios);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }

            //modifica los registros actuales
            try
            {
                foreach (budget_rel_comentarios item in valoresUpdateComentarios)
                {
                    //obtiene el elemento de BD
                    budget_rel_comentarios objeto = comentariosEnBD.FirstOrDefault(x => x.id == item.id);

                    if (objeto != null)
                    {
                        db.Entry(objeto).CurrentValues.SetValues(item);

                    }

                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }

            //Elimina los elemntos necesarios 
            try
            {
                foreach (budget_rel_comentarios item in valoresDeleteComentarios)
                {
                    //obtiene el elemento de BD
                    budget_rel_comentarios objeto = comentariosEnBD.FirstOrDefault(x => x.id == item.id);

                    if (objeto != null)
                    {
                        db.budget_rel_comentarios.Remove(objeto);
                    }

                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }
            #endregion

            TempData["Mensaje"] = new MensajesSweetAlert("Se ha modificado correctamente. ", TipoMensajesSweetAlerts.SUCCESS);


            if (!continuar)
                return RedirectToAction("Centros");
            else
                return RedirectToAction(form["action"].ToString(), new { id = form["id_cc"].ToString() });
        }


        // POST: ResponsableBudget/EditCentro
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        public ActionResult EditCentroHT(int? id, List<object[]> dataListFromTable)
        {
            if (dataListFromTable == null)
                return Json(new { result = "WARNING", icon = "warning", message = "No se detectaron elementos que procesar." }, JsonRequestBehavior.AllowGet);

            //inicializa la lista de objetos
            var response = new object[1];

            List<budget_rel_comentarios> lista_comentarios_form = new List<budget_rel_comentarios>();

            //convierte el list de arrays en objetos budget_cantidad
            List<budget_cantidad> lista_cantidad_form = ConvierteArrayACantidades(dataListFromTable, id, lista_comentarios_form);

            //obtiene el listado actual de la BD
            List<budget_cantidad> lista_cantidad_BD = db.budget_cantidad.Where(x => x.id_budget_rel_fy_centro == id).ToList();

            //obtiene el listado actual de la BD
            List<budget_rel_comentarios> lista_comentarios_BD = db.budget_rel_comentarios.Where(x => x.id_budget_rel_fy_centro == id).ToList();

            //borra todos los comentarios, (se agregan en la segunda llamada ajax)
            lista_cantidad_BD = lista_cantidad_BD
                    .Select(x =>
                    {
                        x.comentario = null;
                        return x;
                    }).ToList();

            //crea, modifica o elimina cantidad
            try
            {
                response[0] = new { result = "OK", icon = "success", message = "Se guardaron los cambios correctamente." };

                //Recorre todas las cantidades de la tabla
                for (int i = 0; i < lista_cantidad_form.Count; i++)
                {
                    budget_cantidad itemBD = lista_cantidad_BD.FirstOrDefault(x =>
                                                x.id_cuenta_sap == lista_cantidad_form[i].id_cuenta_sap
                                                && x.mes == lista_cantidad_form[i].mes
                                                && x.currency_iso == lista_cantidad_form[i].currency_iso
                                                && x.moneda_local_usd == lista_cantidad_form[i].moneda_local_usd
                                            );
                    //EXISTE
                    if (itemBD != null)
                    {
                        //UPDATE
                        if (itemBD.cantidad != lista_cantidad_form[i].cantidad)
                            itemBD.cantidad = lista_cantidad_form[i].cantidad;
                    }
                    else  //CREATE
                    {
                        db.budget_cantidad.Add(lista_cantidad_form[i]);
                    }
                }

                //DELETE
                //elimina aquellos que no aparezcan en los enviados
                List<budget_cantidad> toDeleteList = lista_cantidad_BD.Where(x => !lista_cantidad_form.Any(y => y.id_cuenta_sap == x.id_cuenta_sap
                                && y.mes == x.mes
                                && y.currency_iso == x.currency_iso
                                && y.moneda_local_usd == x.moneda_local_usd
                                )).ToList();

                //borra los conceptos que inpiden el borrado
                foreach (var item in toDeleteList)
                    db.budget_rel_conceptos_cantidades.RemoveRange(item.budget_rel_conceptos_cantidades);

                db.budget_cantidad.RemoveRange(toDeleteList);



                //crea, modifica o elimina COMENTARIOS generales
                for (int i = 0; i < lista_comentarios_form.Count; i++)
                {
                    var itemBD = lista_comentarios_BD.FirstOrDefault(x => x.id_cuenta_sap == lista_comentarios_form[i].id_cuenta_sap);
                    //EXISTE
                    if (itemBD != null)
                    {
                        //UPDATE
                        if (itemBD.comentarios != lista_comentarios_form[i].comentarios)
                            itemBD.comentarios = lista_comentarios_form[i].comentarios;
                    }
                    else  //CREATE
                    {
                        db.budget_rel_comentarios.Add(lista_comentarios_form[i]);
                    }
                }

                //DELETE
                //elimina aquellos que no aparezcan en los enviados
                List<budget_rel_comentarios> toDeleteListComentarios = lista_comentarios_BD.Where(x => !lista_comentarios_form.Any(y => y.id_cuenta_sap == x.id_cuenta_sap)).ToList();
                db.budget_rel_comentarios.RemoveRange(toDeleteListComentarios);


                try
                {
                    db.SaveChanges();
                    response[0] = new { result = "OK", icon = "success", message = "Se guardaron los cambios correctamente" };
                }
                catch (Exception e)
                {
                    response[0] = new { result = "ERROR", icon = "error", message = e.Message };
                }


            }
            catch (Exception e)
            {
                response[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveComments(List<budget_cantidad> comments, int? id_rel)
        {
            if (comments == null)
                comments = new List<budget_cantidad>();

            var response = new object[1];
            response[0] = new { result = "OK", icon = "success", message = "Se guardaron los cambios correctamente." };

            //obtiene el listado actual de la BD
            List<budget_cantidad> lista_cantidad_BD = db.budget_cantidad.Where(x => x.id_budget_rel_fy_centro == id_rel).ToList();

            //obtiene el listado de cantidades
            List<budget_cuenta_sap> lista_cuenta_sap_BD = db.budget_cuenta_sap.ToList();

            //UPDATE comentarios
            foreach (var item in comments)
            {

                var itemBD = lista_cantidad_BD.FirstOrDefault(x =>
                                            x.budget_cuenta_sap.sap_account == item.numero_cuenta_sap
                                            && x.mes == item.mes
                                            && x.currency_iso == item.currency_iso
                                            && x.moneda_local_usd == item.moneda_local_usd
                                        );
                //EXISTE
                if (itemBD != null)
                {
                    //UPDATE
                    if (itemBD.comentario != item.comentario)
                        itemBD.comentario = Clases.Util.UsoStrings.RecortaString(item.comentario, 150);
                }
                else  //CREATE
                {
                    item.cantidad = 0;
                    item.comentario = Clases.Util.UsoStrings.RecortaString(item.comentario, 150);
                    item.id_cuenta_sap = lista_cuenta_sap_BD.FirstOrDefault(x => x.sap_account == item.numero_cuenta_sap).id;

                    db.budget_cantidad.Add(item);
                }
            }

            //DELETE comentarios
            //elimina aquellos que no aparezcan en los enviados
            //List<budget_cantidad> toDeleteList = lista_cantidad_BD.Where(x => !lista_cantidad_form.Any(y => y.id_cuenta_sap == x.id_cuenta_sap
            //                && y.mes == x.mes
            //                && y.currency_iso == x.currency_iso
            //                && y.moneda_local_usd == x.moneda_local_usd
            //                )).ToList();
            //db.budget_cantidad.RemoveRange(toDeleteList);

            try
            {
                db.SaveChanges();
                response[0] = new { result = "OK", icon = "success", message = "Se guardaron los cambios correctamente" };
            }
            catch (Exception e)
            {
                response[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }

            return Json(response, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SaveConceptosCapturados(List<budget_rel_conceptos_cantidades> conceptos_form, int? id_rel)
        {

            if (conceptos_form == null)
                conceptos_form = new List<budget_rel_conceptos_cantidades>();

            var response = new object[1];
            response[0] = new { result = "OK", icon = "success", message = "Se guardaron los cambios correctamente." };

            //obtiene el listado actual de la BD
            List<budget_rel_conceptos_cantidades> lista_conceptos_BD = db.budget_rel_conceptos_cantidades.Where(x => x.budget_cantidad.id_budget_rel_fy_centro == id_rel).ToList();
            //obtiene listado de cantidades
            List<budget_cantidad> lista_cantidades_bd = db.budget_cantidad.Where(x => x.id_budget_rel_fy_centro == id_rel).ToList();


            foreach (var item in conceptos_form)
            {

                var itemBD = lista_conceptos_BD.FirstOrDefault(x => x.id_budget_cantidad == item.id_budget_cantidad && x.id_rel_conceptos == item.id_rel_conceptos);
                //EXISTE
                if (itemBD != null)
                {
                    //UPDATE
                    if (itemBD.cantidad != item.cantidad)
                        itemBD.cantidad = item.cantidad;
                }
                else  //CREATE
                {
                    var cantidad = lista_cantidades_bd.FirstOrDefault(x =>
                                                x.budget_cuenta_sap.sap_account == item.cuenta_sap
                                                && x.mes == item.mes
                                                && x.currency_iso == item.currency
                                                && !x.moneda_local_usd
                                                );
                    if (cantidad != null)
                        db.budget_rel_conceptos_cantidades.Add(new budget_rel_conceptos_cantidades
                        {
                            id_budget_cantidad = cantidad.id,
                            id_rel_conceptos = item.id_rel_conceptos,
                            cantidad = item.cantidad,
                        });
                }
            }

            /* DELETE */

            try
            {
                db.SaveChanges();
                response[0] = new { result = "OK", icon = "success", message = "Se guardaron los cambios correctamente" };
            }
            catch (Exception e)
            {
                response[0] = new { result = "ERROR", icon = "error", message = e.Message };
            }

            return Json(response, JsonRequestBehavior.AllowGet);

        }



        /// <summary>
        /// Carga los datos iniciales del año indicado
        /// </summary>
        /// <param name="id_fy"></param>
        /// <returns></returns>
        public JsonResult CargaFYComentarios(int? id_fy, int? id_centro_costo, int? id_fy_cc)
        {
            //obtiene los valores 
            var valoresListAnioActual = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == id_fy && x.id_centro_costo == id_centro_costo).ToList();
            valoresListAnioActual = AgregaCuentasSAPFaltantes(valoresListAnioActual, id_fy.Value, id_centro_costo.Value).OrderBy(x => x.sap_account).ToList();

            //obtiene el rel fy cc
            var fy_cc = db.budget_rel_fy_centro.Find(id_fy_cc);

            var jsonData = new List<object>();

            //calcula si debe ser readonly
            bool isActualOctubre = fy_cc.budget_anio_fiscal.isActual(10) == "ACT";
            bool isActualNoviembre = fy_cc.budget_anio_fiscal.isActual(11) == "ACT";
            bool isActualDiciembre = fy_cc.budget_anio_fiscal.isActual(12) == "ACT";
            bool isActualEnero = fy_cc.budget_anio_fiscal.isActual(1) == "ACT";
            bool isActualFebrero = fy_cc.budget_anio_fiscal.isActual(2) == "ACT";
            bool isActualMarzo = fy_cc.budget_anio_fiscal.isActual(3) == "ACT";
            bool isActualAbril = fy_cc.budget_anio_fiscal.isActual(4) == "ACT";
            bool isActualMayo = fy_cc.budget_anio_fiscal.isActual(5) == "ACT";
            bool isActualJunio = fy_cc.budget_anio_fiscal.isActual(6) == "ACT";
            bool isActualJulio = fy_cc.budget_anio_fiscal.isActual(7) == "ACT";
            bool isActualAgosto = fy_cc.budget_anio_fiscal.isActual(8) == "ACT";
            bool isActualSeptiembre = fy_cc.budget_anio_fiscal.isActual(9) == "ACT";

            //recorre todos los comentarios
            foreach (var cantidad in fy_cc.budget_cantidad.Where(x => !string.IsNullOrEmpty(x.comentario)))
            {
                var col = 0;
                bool readOnly = false;

                //determina la columna inicial (default para pesos)
                switch (cantidad.mes)
                {
                    case 10:
                        col = 3;
                        if (isActualOctubre)
                            readOnly = true;
                        break;
                    case 11:
                        col = 7;
                        if (isActualNoviembre)
                            readOnly = true;
                        break;
                    case 12:
                        col = 11;
                        if (isActualDiciembre)
                            readOnly = true;
                        break;
                    case 1:
                        col = 15;
                        if (isActualEnero)
                            readOnly = true;
                        break;
                    case 2:
                        col = 19;
                        if (isActualFebrero)
                            readOnly = true;
                        break;
                    case 3:
                        col = 23;
                        if (isActualMarzo)
                            readOnly = true;
                        break;
                    case 4:
                        col = 27;
                        if (isActualAbril)
                            readOnly = true;
                        break;
                    case 5:
                        col = 31;
                        if (isActualMayo)
                            readOnly = true;
                        break;
                    case 6:
                        col = 35;
                        if (isActualJunio)
                            readOnly = true;
                        break;
                    case 7:
                        col = 39;
                        if (isActualJulio)
                            readOnly = true;
                        break;
                    case 8:
                        col = 43;
                        if (isActualAgosto)
                            readOnly = true;
                        break;
                    case 9:
                        col = 47;
                        if (isActualSeptiembre)
                            readOnly = true;
                        break;
                }

                //dolares USD
                if (cantidad.currency_iso == "USD" && !cantidad.moneda_local_usd)
                    col++;
                //euros
                else if (cantidad.currency_iso == "EUR" && !cantidad.moneda_local_usd)
                    col += 2;
                //dolares moneda local
                else if (cantidad.currency_iso == "USD" && cantidad.moneda_local_usd)
                    col += 3;

                jsonData.Add(new
                {
                    row = valoresListAnioActual.IndexOf(valoresListAnioActual.FirstOrDefault(x => x.sap_account == cantidad.budget_cuenta_sap.sap_account)),
                    col,
                    comment = new { value = cantidad.comentario, readOnly = readOnly }
                });
            }



            return Json(jsonData.ToArray(), JsonRequestBehavior.AllowGet);
        }


        private List<budget_cantidad> ConvierteArrayACantidades(List<object[]> data, int? id_rel_fy_cc, List<budget_rel_comentarios> comentarios_general)
        {
            var rel_fy_cc = db.budget_rel_fy_centro.Find(id_rel_fy_cc);

            var cuestasSAPBD = db.budget_cuenta_sap.ToList();

            List<budget_cantidad> resultado = new List<budget_cantidad> { };

            #region cabeceras HT
            //cabeceras HT FORECAST
            List<string> headersForecast = new List<string> { "SAP_ACCOUNT", "Name", "Mapping" };


            DateTime fecha = new DateTime(rel_fy_cc.budget_anio_fiscal.anio_inicio, rel_fy_cc.budget_anio_fiscal.mes_inicio, 1);
            for (int i = 0; i < 12; i++)
            {
                //agrega cabecera para tipo moneda
                headersForecast.Add("MXN");
                headersForecast.Add("USD");
                headersForecast.Add("EUR");
                headersForecast.Add("USD");

                fecha = fecha.AddMonths(1);
            }
            headersForecast.Add("Total (MXN)");
            headersForecast.Add("Total (USD)");
            headersForecast.Add("Total (EUR)");
            headersForecast.Add("Total (Local USD)"); headersForecast.Add("Totals");
            headersForecast.Add("Comments");
            headersForecast.Add("AplicaFormula");

            #endregion

            //listado de encabezados
            string[] encabezados = headersForecast.ToArray();

            //recorre todos los arrays recibidos
            foreach (var array in data)
            {

                //variables generales
                string sap_account = array[Array.IndexOf(encabezados, "SAP_ACCOUNT")].ToString();
                int id_sap_account = !string.IsNullOrEmpty(sap_account) ? cuestasSAPBD.FirstOrDefault(x => x.sap_account == sap_account).id : 0;
                DateTime fechaArray = new DateTime(rel_fy_cc.budget_anio_fiscal.anio_inicio, rel_fy_cc.budget_anio_fiscal.mes_inicio, 1);
                int col = 3;

                //recorre cada uno de los meses
                if (!string.IsNullOrEmpty(sap_account))
                    for (int i = 0; i < 12; i++)
                    {

                        decimal cantidadTemporal = 0;


                        for (int j = 0; j < 4; j++)
                        {
                            bool puedeConvertir = decimal.TryParse(array[col + j] != null ? array[col + j].ToString() : string.Empty, out cantidadTemporal);

                            if (array[col + j] != null && !string.IsNullOrEmpty(array[col + j].ToString()) && cantidadTemporal != 0)
                                resultado.Add(new budget_cantidad
                                {
                                    id_budget_rel_fy_centro = rel_fy_cc.id,
                                    id_cuenta_sap = id_sap_account,
                                    mes = fecha.Month,
                                    currency_iso = encabezados[col + j],
                                    cantidad = decimal.Round(cantidadTemporal, 2),
                                    //comentario
                                    moneda_local_usd = j == 3 ? true : false
                                });
                        }

                        //aumenta variables
                        col += 4;
                        fecha = fecha.AddMonths(1);
                    }

                int comentarioIndex = headersForecast.IndexOf("Comments") - 1;

                //agrega el comentario general en caso de existir
                if (array[comentarioIndex] != null && !string.IsNullOrEmpty(array[comentarioIndex].ToString()))
                    comentarios_general.Add(new budget_rel_comentarios
                    {
                        id_budget_rel_fy_centro = id_rel_fy_cc.Value,
                        id_cuenta_sap = id_sap_account,
                        comentarios = array[comentarioIndex].ToString()
                    });

            }


            return resultado;
        }

        /// <summary>
        /// Carga los datos iniciales del año indicado
        /// </summary>
        /// <param name="id_fy"></param>
        /// <returns></returns>
        public JsonResult CargaFY(int? id_fy, int? id_centro_costo)
        {

            var valoresListAnioActual = db.view_valores_fiscal_year.Where(x => x.id_anio_fiscal == id_fy && x.id_centro_costo == id_centro_costo).ToList();
            valoresListAnioActual = AgregaCuentasSAPFaltantes(valoresListAnioActual, id_fy.Value, id_centro_costo.Value).OrderBy(x => x.sap_account).ToList();
            var fy = db.budget_anio_fiscal.Find(id_fy);

            var jsonData = new List<object>();

            var sapAccountsList = db.budget_cuenta_sap.ToList();

            for (int i = 0; i < valoresListAnioActual.Count(); i++)
            {

                //obtine las tres monedas de la cantidad
                var tipo_cambio_usd_mxn = fy.budget_rel_tipo_cambio_fy.FirstOrDefault(x => x.id_tipo_cambio == 1).cantidad.ToString();
                var tipo_cambio_eur_usd = fy.budget_rel_tipo_cambio_fy.FirstOrDefault(x => x.id_tipo_cambio == 2).cantidad.ToString();

                jsonData.Add(new[] {
                    valoresListAnioActual[i].sap_account,
                    valoresListAnioActual[i].name,
                    valoresListAnioActual[i].mapping_bridge,
                    valoresListAnioActual[i].Octubre_MXN.ToString(),
                    valoresListAnioActual[i].Octubre.ToString(),
                    valoresListAnioActual[i].Octubre_EUR.ToString(),
                    string.Format("=(D{0}/{1}) + E{0} + (F{0}*{2})", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Noviembre_MXN.ToString(),
                    valoresListAnioActual[i].Noviembre.ToString(),
                    valoresListAnioActual[i].Noviembre_EUR.ToString(),
                    string.Format("=(H{0}/{1}) + I{0} + (J{0}*{2})", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Diciembre_MXN.ToString(),
                    valoresListAnioActual[i].Diciembre.ToString(),
                    valoresListAnioActual[i].Diciembre_EUR.ToString(),
                    string.Format("=(L{0}/{1}) + M{0} + (N{0}*{2})", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Enero_MXN.ToString(),
                    valoresListAnioActual[i].Enero.ToString(),
                    valoresListAnioActual[i].Enero_EUR.ToString(),
                    string.Format("=(P{0}/{1}) + Q{0} + (R{0}*{2})", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Febrero_MXN.ToString(),
                    valoresListAnioActual[i].Febrero.ToString(),
                    valoresListAnioActual[i].Febrero_EUR.ToString(),
                    string.Format("=(T{0}/{1}) + U{0} + (V{0}*{2})", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Marzo_MXN.ToString(),
                    valoresListAnioActual[i].Marzo.ToString(),
                    valoresListAnioActual[i].Marzo_EUR.ToString(),
                    string.Format("=(X{0}/{1}) + Y{0} + (Z{0}*{2})", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Abril_MXN.ToString(),
                    valoresListAnioActual[i].Abril.ToString(),
                    valoresListAnioActual[i].Abril_EUR.ToString(),
                    string.Format("=(AB{0}/{1}) + AC{0} + (AD{0}*{2})", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Mayo_MXN.ToString(),
                    valoresListAnioActual[i].Mayo.ToString(),
                    valoresListAnioActual[i].Mayo_EUR.ToString(),
                    string.Format("=(AF{0}/{1}) + AG{0} + (AH{0}*{2})", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Junio_MXN.ToString(),
                    valoresListAnioActual[i].Junio.ToString(),
                    valoresListAnioActual[i].Junio_EUR.ToString(),
                    string.Format("=(AJ{0}/{1}) + AK{0} + (AL{0}*{2})", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Julio_MXN.ToString(),
                    valoresListAnioActual[i].Julio.ToString(),
                    valoresListAnioActual[i].Julio_EUR.ToString(),
                    string.Format("=(AN{0}/{1}) + AO{0} + (AP{0}*{2})", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Agosto_MXN.ToString(),
                    valoresListAnioActual[i].Agosto.ToString(),
                    valoresListAnioActual[i].Agosto_EUR.ToString(),
                    string.Format("=(AR{0}/{1}) + AS{0} + (AT{0}*{2})", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Septiembre_MXN.ToString(),
                    valoresListAnioActual[i].Septiembre.ToString(),
                    valoresListAnioActual[i].Septiembre_EUR.ToString(),
                    string.Format("=(AV{0}/{1}) + AW{0} + (AX{0}*{2})", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    string.Format("= D{0} + H{0} + L{0} + P{0} + T{0} + X{0} + AB{0} + AF{0} + AJ{0} + AN{0} + AR{0} + AV{0}", i+1),
                    string.Format("= E{0} + I{0} + M{0} + Q{0} + U{0} + Y{0} + AC{0} + AG{0} + AK{0} + AO{0} + AS{0} + AW{0}", i+1),
                    string.Format("= F{0} + J{0} + N{0} + R{0} + V{0} + Z{0} + AD{0} + AH{0} + AL{0} + AP{0} + AT{0} + AX{0}", i+1),
                    string.Format("= G{0} + K{0} + O{0} + S{0} + W{0} + AA{0} + AE{0} + AI{0} + AM{0} + AQ{0} + AU{0} + AY{0}", i+1),
                    valoresListAnioActual[i].Comentario,
                    sapAccountsList.Any(x=> x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_formula == true).ToString(),
                    sapAccountsList.Any(x=> x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_mxn == true).ToString(),
                    sapAccountsList.Any(x=> x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_usd == true).ToString(),
                    sapAccountsList.Any(x=> x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_eur == true).ToString()
                    });



            }
            int filas = valoresListAnioActual.Count();
            //fila  para sumatorias
            jsonData.Add(new[] {
                string.Empty,
                string.Empty,
                string.Empty,
                string.Format("=SUM({0}{1}:{0}{2})","D", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","E", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","F", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","G", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","H", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","I", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","J", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","K", 1, filas),//  
                string.Format("=SUM({0}{1}:{0}{2})","L", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","M", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","N", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","O", 1, filas),// 
                string.Format("=SUM({0}{1}:{0}{2})","P", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","Q", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","R", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","S", 1, filas),//    
                string.Format("=SUM({0}{1}:{0}{2})","T", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","U", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","V", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","W", 1, filas),//    
                string.Format("=SUM({0}{1}:{0}{2})","X", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","Y", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","Z", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AA", 1, filas),//    
                string.Format("=SUM({0}{1}:{0}{2})","AB", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AC", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AD", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AE", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","AF", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AG", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AH", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AI", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","AJ", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AK", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AL", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AM", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","AN", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AO", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AP", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AQ", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","AR", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AS", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AT", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AU", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","AV", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AW", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AX", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","AY", 1, filas),//
                string.Format("=SUM({0}{1}:{0}{2})","AZ", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","BA", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","BB", 1, filas),
                string.Format("=SUM({0}{1}:{0}{2})","BC", 1, filas),

            });

            return Json(jsonData.ToArray(), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Carga los datos iniciales del año indicado
        /// </summary>
        /// <param name="id_fy"></param>
        /// <returns></returns>
        public JsonResult CargaFormFormula(int? row, int? column, string cuenta_sap, int? id_bd_fy_centro, int? mes, string currency,
            bool datosPrevios, bool readOnly, string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, string k, string m, string l)
        {
            //en caso de readonly, deshabilita los datos previos
            if (readOnly)
                datosPrevios = false;

            var formulario = new object[1];

            var sapAccount = db.budget_cuenta_sap.Where(x => x.sap_account == cuenta_sap).FirstOrDefault();
            var bgCantidad = db.budget_cantidad.Where(x => x.id_budget_rel_fy_centro == id_bd_fy_centro
                                    && x.budget_cuenta_sap.sap_account == cuenta_sap
                                    && x.mes == mes
                                    && x.currency_iso == currency
                                    && !x.moneda_local_usd
                                    ).FirstOrDefault();

            string id_cantidad = bgCantidad == null ? "0" : bgCantidad.id.ToString();

            string html = @" 
                <input type=""hidden"" name=""_readonly"" id=""_readonly"" value=""" + readOnly + @""">
                <input type=""hidden"" name=""_cuenta_sap"" id=""_cuenta_sap"" value=""" + cuenta_sap + @""">
                <input type=""hidden"" name=""_id_bd_fy_centro"" id=""_id_bd_fy_centro"" value=""" + id_bd_fy_centro + @""">
                <input type=""hidden"" name=""_mes"" id=""_mes"" value=""" + mes + @""">
                <input type=""hidden"" name=""_currency"" id=""_currency"" value=""" + currency + @""">
                <input type=""hidden"" name=""id_budget_cantidad"" id=""id_budget_cantidad"" value=""" + id_cantidad + @""">
                <input type=""hidden"" name=""formula"" id=""formula"" value=""" + sapAccount.formula + @""">
                <input type=""hidden"" name=""row_formula"" id=""row_formula"" value=""" + row + @""">
                <input type=""hidden"" name=""column_formula"" id=""column_formula"" value=""" + column + @""">";

            if (sapAccount.budget_rel_conceptos_formulas.Count == 0)
            {
                formulario[0] = new { estatus = "ERROR" };
                return Json(formulario, JsonRequestBehavior.AllowGet);
            }

            foreach (var concepto in sapAccount.budget_rel_conceptos_formulas)
            {
                //valor por defecto,  al cargar el formulario por primera vez
                string valor = currency == "MXN" ? concepto.valor_defecto_mxn.ToString() : currency == "USD" ? concepto.valor_defecto_usd.ToString() : currency == "EUR" ? concepto.valor_defecto_eur.ToString() : "0.0";

                //valor formulario
                if (datosPrevios && (!concepto.valor_fijo.HasValue || !concepto.valor_fijo.Value))
                    switch (concepto.clave)
                    {
                        case "a":
                            valor = a;
                            break;
                        case "b":
                            valor = b;
                            break;
                        case "c":
                            valor = c;
                            break;
                        case "d":
                            valor = d;
                            break;
                        case "e":
                            valor = e;
                            break;
                        case "f":
                            valor = f;
                            break;
                        case "g":
                            valor = g;
                            break;
                        case "h":
                            valor = h;
                            break;
                        case "i":
                            valor = i;
                            break;
                        case "j":
                            valor = j;
                            break;
                        case "k":
                            valor = k;
                            break;
                        case "l":
                            valor = l;
                            break;
                        case "m":
                            valor = m;
                            break;

                    }
                //si no toma el valor desde bd
                else if (bgCantidad != null && bgCantidad.budget_rel_conceptos_cantidades.Count > 0)
                {
                    valor = bgCantidad.budget_rel_conceptos_cantidades.FirstOrDefault(x => x.budget_rel_conceptos_formulas.clave == concepto.clave) != null ?
                        bgCantidad.budget_rel_conceptos_cantidades.FirstOrDefault(x => x.budget_rel_conceptos_formulas.clave == concepto.clave).cantidad.ToString() : "0";

                    //si no es read only toma el valor por defecto y no el de la BD
                    if (!readOnly && concepto.valor_fijo.HasValue && concepto.valor_fijo.Value)
                        valor = currency == "MXN" ? concepto.valor_defecto_mxn.ToString() : currency == "USD" ? concepto.valor_defecto_usd.ToString() : currency == "EUR" ? concepto.valor_defecto_eur.ToString() : "0.0";
                }

                html += String.Format(@"
                <input type=""hidden"" name=""id_rel_concepto_{0}"" id=""id_rel_concepto_{0}"" value=""" + concepto.id + @""">
                <input type=""hidden"" name=""concepto_clave_{0}"" id=""concepto_clave_{0}"" value=""" + concepto.clave + @""">
                <div class=""form-group row"">
                    <label class=""control-label col-md-3 col-sm-3"" for=""val_{0}"" style=""text-align:right"">{1}:</label>
                    <div class=""col-md-9"">
                        <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" {2} value=""{3}"" {4}/>
                        <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                    </div>
                </div>", concepto.clave, concepto.descripcion, concepto.valor_fijo.HasValue && concepto.valor_fijo.Value ? "readonly" : string.Empty
                , valor, readOnly ? "readonly" : string.Empty);
            }

            html += String.Format(@" <div class=""form-group row"">
                    <label class=""control-label col-md-3 col-sm-3"" for=""val_{0}"" style=""text-align:right"">{1}:</label>
                    <div class=""col-md-9"">
                        <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" readonly />
                        <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                    </div>
                </div>", "result", "Total");

            formulario[0] = new
            {
                estatus = "OK",
                html = html
            };

            return Json(formulario, JsonRequestBehavior.AllowGet);
        }


        [NonAction]
        public static budget_anio_fiscal GetAnioFiscal(DateTime fechaBusqueda)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            //obtiene la fecha y la coloca al inicio del mes          
            var fecha = new DateTime(fechaBusqueda.Year, fechaBusqueda.Month, 1, 0, 0, 0);
            //encuentra el año fiscal anterior

            var anio_fiscal_list = db.budget_anio_fiscal.Where(x => x.anio_inicio == fecha.Year || x.anio_fin == fecha.Year).ToList();

            foreach (budget_anio_fiscal anioFiscal in anio_fiscal_list)
            {
                DateTime inicio = new DateTime(anioFiscal.anio_inicio, anioFiscal.mes_inicio, 1, 0, 0, 0);
                DateTime fin = new DateTime(anioFiscal.anio_fin, anioFiscal.mes_fin, 1, 0, 0, 0);

                if (fecha >= inicio && fecha <= fin)
                    return anioFiscal;
            }

            return null;
        }



        [NonAction]
        public static List<view_valores_fiscal_year> AgregaCuentasSAPFaltantes(List<view_valores_fiscal_year> listValores, int id_anio_fiscal, int id_centro_costo, bool soloCuentasActivas = false)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            List<budget_cuenta_sap> listCuentas = new List<budget_cuenta_sap>();

            budget_centro_costo centro = db.budget_centro_costo.Find(id_centro_costo);

            if (soloCuentasActivas) //carga sólo las cuentas activas
                listCuentas = db.budget_cuenta_sap.Where(x => x.activo == true).ToList();
            else //carga todas las cuentas
                listCuentas = db.budget_cuenta_sap.ToList();

            //Agrega cuentas vacias

            List<view_valores_fiscal_year> listViewCuentas = new List<view_valores_fiscal_year>();

            foreach (budget_cuenta_sap cuenta in listCuentas)
            {
                //agragega un objeto de tipo view_valores_anio_fiscal por cada cuenta existente
                listViewCuentas.Add(new view_valores_fiscal_year
                {
                    id_anio_fiscal = id_anio_fiscal,
                    id_centro_costo = id_centro_costo,
                    id_cuenta_sap = cuenta.id,
                    sap_account = cuenta.sap_account,
                    name = cuenta.name,
                    mapping = cuenta.budget_mapping.descripcion,
                    mapping_bridge = cuenta.budget_mapping.budget_mapping_bridge.descripcion,
                    currency_iso = "USD",
                    class_1 = centro.class_1,
                    class_2 = centro.class_2,
                });
            }

            //obtiene las cuentas que no se encuentran en el listado original
            List<view_valores_fiscal_year> listDiferencias = listViewCuentas.Except(listValores).ToList();

            //suba la lista original con la lista de excepciones
            listValores.AddRange(listDiferencias);


            return listValores.OrderBy(x => x.id_cuenta_sap).ToList();
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
