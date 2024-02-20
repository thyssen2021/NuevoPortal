using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
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

                //envia año fiscal actual y proximo

                //obtiene el año fiscal actual (forecast)
                budget_anio_fiscal anio_Fiscal_actual = GetAnioFiscal(DateTime.Now);

                //obtiene el año fiscal proximo (budget)
                budget_anio_fiscal anio_Fiscal_proximo = GetAnioFiscal(DateTime.Now.AddYears(1));

                ViewBag.IdFYactual = anio_Fiscal_actual != null ? anio_Fiscal_actual.id : 0;
                ViewBag.IdFYproximo = anio_Fiscal_proximo != null ? anio_Fiscal_proximo.id : 0;

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
                if (id_fiscal_year.HasValue)
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
                headersForecast.Add("Soporte");
                headersForecast.Add("AplicaGastos");

                ViewBag.HeadersForecast1_actual = JsonConvert.SerializeObject(cabeceraObjectActual);
                ViewBag.HeadersForecast1_budget = JsonConvert.SerializeObject(cabeceraObjectBudget);
                ViewBag.HeadersForecast1 = JsonConvert.SerializeObject(cabeceraObject);
                ViewBag.HeadersForecast2 = headersForecast.ToArray();

                //obtiene todos lo valores sugeridos
                List<string[]> listaSugeridos = new List<string[]>();
                List<int> listMeses = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                List<string> listMoneda = new List<string> { "MXN", "USD", "EUR" };

                foreach (var sapAccount in rel_fy_centro_presente.budget_cantidad.Where(x=>x.budget_cuenta_sap.aplica_gastos_mantenimiento).Select(x=>x.budget_cuenta_sap).Distinct())
                {
                    foreach (var mes in listMeses)
                    {
                        foreach (var moneda in listMoneda)
                        {
                            double sugerido = 0;

                            //obtiene los ultimos tres gastos
                            var listGastos = sapAccount.budget_conceptos_mantenimiento.Where(x => x.budget_rel_fy_centro.id_centro_costo == rel_fy_centro_presente.id_centro_costo && x.mes == mes
                            && x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio < rel_fy_centro_presente.budget_anio_fiscal.anio_inicio && x.gasto != 0
                            && x.moneda == moneda
                            ).OrderByDescending(x => x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio).Take(3).ToList();

                            sugerido = listGastos.Count == 0 ? 0 : listGastos.Sum(x => x.gasto - (x.one_time.HasValue ? x.one_time.Value : 0)) / listGastos.Count;

                            listaSugeridos.Add(new string[4] { sapAccount.sap_account, moneda, mes.ToString(), Math.Round(sugerido, 2).ToString() });
                        }
                    }                 
                }
                ViewBag.GastosSugeridos = listaSugeridos.ToArray();

                #endregion

                ViewBag.centroCosto = centroCosto;
                ViewBag.rel_anterior = rel_fy_centro_anterior;
                ViewBag.rel_presente = rel_fy_centro_presente;
                ViewBag.rel_proximo = rel_fy_centro_proximo;
                ViewBag.numCuentasSAP = db.budget_cuenta_sap.Where(x => x.activo).Count();

              
                ViewBag.controlling = controlling;
                ViewBag.info = info;
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
                    if (itemBD.cantidad != item.cantidad || itemBD.comentario != item.comentario)
                    {
                        itemBD.cantidad = item.cantidad;
                        itemBD.comentario = item.comentario;
                    }
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
                            comentario = item.comentario
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
        public JsonResult CargaFYComentarios(int? id_fy, int? id_centro_costo, int? id_fy_cc, bool controlling = false)
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
                        if (isActualOctubre && controlling == false)
                            readOnly = true;
                        break;
                    case 11:
                        col = 7;
                        if (isActualNoviembre && controlling == false)
                            readOnly = true;
                        break;
                    case 12:
                        col = 11;
                        if (isActualDiciembre && controlling == false)
                            readOnly = true;
                        break;
                    case 1:
                        col = 15;
                        if (isActualEnero && controlling == false)
                            readOnly = true;
                        break;
                    case 2:
                        col = 19;
                        if (isActualFebrero && controlling == false)
                            readOnly = true;
                        break;
                    case 3:
                        col = 23;
                        if (isActualMarzo && controlling == false)
                            readOnly = true;
                        break;
                    case 4:
                        col = 27;
                        if (isActualAbril && controlling == false)
                            readOnly = true;
                        break;
                    case 5:
                        col = 31;
                        if (isActualMayo && controlling == false)
                            readOnly = true;
                        break;
                    case 6:
                        col = 35;
                        if (isActualJunio && controlling == false)
                            readOnly = true;
                        break;
                    case 7:
                        col = 39;
                        if (isActualJulio && controlling == false)
                            readOnly = true;
                        break;
                    case 8:
                        col = 43;
                        if (isActualAgosto && controlling == false)
                            readOnly = true;
                        break;
                    case 9:
                        col = 47;
                        if (isActualSeptiembre && controlling == false)
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

            //obtiene el id fy cc
            var fy_cc = fy.budget_rel_fy_centro.FirstOrDefault(x => x.id_anio_fiscal == id_fy && x.id_centro_costo == id_centro_costo);

            //obtiene los documento
            var doctosList = fy_cc.budget_rel_documento;

            //obtiene el listado de los CeCo en los que aplica gastos de manto
            List<int> ccListManto = db.budget_conceptos_mantenimiento.Select(x => x.budget_rel_fy_centro.id_centro_costo).Distinct().ToList();
            List<string> cuentasListManto = db.budget_conceptos_mantenimiento.Select(x => x.budget_cuenta_sap.sap_account).Distinct().ToList();

            for (int i = 0; i < valoresListAnioActual.Count(); i++)
            {

                //obtine las tres monedas de la cantidad
                var tipo_cambio_usd_mxn = fy.budget_rel_tipo_cambio_fy.FirstOrDefault(x => x.id_tipo_cambio == 1).cantidad.ToString();
                var tipo_cambio_eur_usd = fy.budget_rel_tipo_cambio_fy.FirstOrDefault(x => x.id_tipo_cambio == 2).cantidad.ToString();

                //valor por defecto
                string btnDoc = "<button class='btn-documento-pendiente' onclick=\"muestraSoporte(" + fy_cc.id + ", " +
                    valoresListAnioActual[i].id_cuenta_sap
                    + ")\">" + (fy_cc.estatus ? "Agregar" : "Ver") + "</button>";

                //valida si ya existe un documento para esta cuenta sap y rel fy cc
                if (doctosList.Any(x => x.id_cuenta_sap == valoresListAnioActual[i].id_cuenta_sap))
                {
                    btnDoc = "<button class='btn-documento-agregado' onclick=\"muestraSoporte(" + fy_cc.id + ", " +
                        valoresListAnioActual[i].id_cuenta_sap
                        + ")\">" + (fy_cc.estatus ? "Modificar" : "Ver") + "</button>";
                }

                //determina si las cuentas de manto son editables o no
                bool editableCtaManto = cuentasListManto.Any(x => x == valoresListAnioActual[i].sap_account) && !ccListManto.Any(x => x == id_centro_costo) ? false: true;

                    jsonData.Add(new[] {
                    valoresListAnioActual[i].sap_account,
                    valoresListAnioActual[i].name,
                    valoresListAnioActual[i].mapping_bridge,
                    valoresListAnioActual[i].Octubre_MXN.ToString(),
                    valoresListAnioActual[i].Octubre.ToString(),
                    valoresListAnioActual[i].Octubre_EUR.ToString(),
                    string.Format("=ROUND((D{0}/{1}) + E{0} + (F{0}*{2}),2)", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Noviembre_MXN.ToString(),
                    valoresListAnioActual[i].Noviembre.ToString(),
                    valoresListAnioActual[i].Noviembre_EUR.ToString(),
                    string.Format("=ROUND((H{0}/{1}) + I{0} + (J{0}*{2}),2)", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Diciembre_MXN.ToString(),
                    valoresListAnioActual[i].Diciembre.ToString(),
                    valoresListAnioActual[i].Diciembre_EUR.ToString(),
                    string.Format("=ROUND((L{0}/{1}) + M{0} + (N{0}*{2}),2)", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Enero_MXN.ToString(),
                    valoresListAnioActual[i].Enero.ToString(),
                    valoresListAnioActual[i].Enero_EUR.ToString(),
                    string.Format("=ROUND((P{0}/{1}) + Q{0} + (R{0}*{2}),2)", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Febrero_MXN.ToString(),
                    valoresListAnioActual[i].Febrero.ToString(),
                    valoresListAnioActual[i].Febrero_EUR.ToString(),
                    string.Format("=ROUND((T{0}/{1}) + U{0} + (V{0}*{2}),2)", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Marzo_MXN.ToString(),
                    valoresListAnioActual[i].Marzo.ToString(),
                    valoresListAnioActual[i].Marzo_EUR.ToString(),
                    string.Format("=ROUND((X{0}/{1}) + Y{0} + (Z{0}*{2}),2)", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Abril_MXN.ToString(),
                    valoresListAnioActual[i].Abril.ToString(),
                    valoresListAnioActual[i].Abril_EUR.ToString(),
                    string.Format("=ROUND((AB{0}/{1}) + AC{0} + (AD{0}*{2}),2)", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Mayo_MXN.ToString(),
                    valoresListAnioActual[i].Mayo.ToString(),
                    valoresListAnioActual[i].Mayo_EUR.ToString(),
                    string.Format("=ROUND((AF{0}/{1}) + AG{0} + (AH{0}*{2}),2)", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Junio_MXN.ToString(),
                    valoresListAnioActual[i].Junio.ToString(),
                    valoresListAnioActual[i].Junio_EUR.ToString(),
                    string.Format("=ROUND((AJ{0}/{1}) + AK{0} + (AL{0}*{2}),2)", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Julio_MXN.ToString(),
                    valoresListAnioActual[i].Julio.ToString(),
                    valoresListAnioActual[i].Julio_EUR.ToString(),
                    string.Format("=ROUND((AN{0}/{1}) + AO{0} + (AP{0}*{2}),2)", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Agosto_MXN.ToString(),
                    valoresListAnioActual[i].Agosto.ToString(),
                    valoresListAnioActual[i].Agosto_EUR.ToString(),
                    string.Format("=ROUND((AR{0}/{1}) + AS{0} + (AT{0}*{2}),2)", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    valoresListAnioActual[i].Septiembre_MXN.ToString(),
                    valoresListAnioActual[i].Septiembre.ToString(),
                    valoresListAnioActual[i].Septiembre_EUR.ToString(),
                    string.Format("=ROUND((AV{0}/{1}) + AW{0} + (AX{0}*{2}),2)", i+1,tipo_cambio_usd_mxn, tipo_cambio_eur_usd),
                    string.Format("= D{0} + H{0} + L{0} + P{0} + T{0} + X{0} + AB{0} + AF{0} + AJ{0} + AN{0} + AR{0} + AV{0}", i+1),
                    string.Format("= E{0} + I{0} + M{0} + Q{0} + U{0} + Y{0} + AC{0} + AG{0} + AK{0} + AO{0} + AS{0} + AW{0}", i+1),
                    string.Format("= F{0} + J{0} + N{0} + R{0} + V{0} + Z{0} + AD{0} + AH{0} + AL{0} + AP{0} + AT{0} + AX{0}", i+1),
                    string.Format("= G{0} + K{0} + O{0} + S{0} + W{0} + AA{0} + AE{0} + AI{0} + AM{0} + AQ{0} + AU{0} + AY{0}", i+1),
                    valoresListAnioActual[i].Comentario,
                    sapAccountsList.Any(x=> x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_formula == true).ToString(),
                    //si se trata de una cuenta de mantenimiento valida si el centro de costo es valido
                    !editableCtaManto ? editableCtaManto.ToString(): sapAccountsList.Any(x=> x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_mxn == true).ToString(),
                    !editableCtaManto ? editableCtaManto.ToString(): sapAccountsList.Any(x=> x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_usd == true).ToString(),
                    !editableCtaManto ? editableCtaManto.ToString(): sapAccountsList.Any(x=> x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_eur == true).ToString(),
                    btnDoc,
                     sapAccountsList.Any(x=> x.sap_account == valoresListAnioActual[i].sap_account && x.aplica_gastos_mantenimiento == true).ToString(),
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
        /// Obtiene el monto sugerido
        /// </summary>
        /// <param name="id_fy"></param>
        /// <returns></returns>
        public JsonResult ObtieneMontoSugerido(int? id_fy_cc, int? mes, string moneda, string sap_account)
        {

            budget_cuenta_sap sapAccount = db.budget_cuenta_sap.FirstOrDefault(x=>x.sap_account == sap_account);

            //obtiene todos los posibles valores
            budget_rel_fy_centro rel_fy_cc = db.budget_rel_fy_centro.Find(id_fy_cc);

            //inicializa la lista de objetos
            var objeto = new object[1];

            double sugerido = 0;

            //obtiene el valor sugerido
            //obtiene los ultimos tres gastos
            var listGastos = sapAccount.budget_conceptos_mantenimiento.Where(x => x.budget_rel_fy_centro.id_centro_costo == rel_fy_cc.id_centro_costo && x.mes == mes
            && x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio < rel_fy_cc.budget_anio_fiscal.anio_inicio && x.gasto != 0
            && x.moneda == moneda
            ).OrderByDescending(x => x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio).Take(3).ToList();

            sugerido = listGastos.Count == 0 ? 0 : listGastos.Sum(x => x.gasto - (x.one_time.HasValue ? x.one_time.Value : 0)) / listGastos.Count;

            objeto[0] = new
            {
                sugerido = sugerido,           
            };

            return Json(objeto, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Carga los datos iniciales del año indicado
        /// </summary>
        /// <param name="id_fy"></param>
        /// <returns></returns>
        public JsonResult CargaFormFormula(int? row, int? column, string cuenta_sap, int? id_bd_fy_centro, int? mes, string currency,
            bool datosPrevios, bool readOnly, string valor_actual, string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, string k, string m, string l,
             string c_a, string c_b, string c_c, string c_d, string c_e, string c_f, string c_g, string c_h, string c_i, string c_j, string c_k, string c_m, string c_l
            )
        {
            //en caso de readonly, deshabilita los datos previos
            if (readOnly)
                datosPrevios = false;

            var formulario = new object[1];

            var sapAccountList = db.budget_cuenta_sap;
            var sapAccount = sapAccountList.Where(x => x.sap_account == cuenta_sap).FirstOrDefault();
            var rel_fy_cc = db.budget_rel_fy_centro.Find(id_bd_fy_centro);
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

            #region form gastos mantenimientos
            if (sapAccountList.Any(x => x.sap_account == cuenta_sap && x.aplica_gastos_mantenimiento))
            {
                double sugerido = 0;
                decimal? real = null;

                //obtiene el valor real
                if (decimal.TryParse(valor_actual, out decimal decimalResult))
                    real = decimalResult;

                //obtiene el valor sugerido
                //obtiene los ultimos tres gastos
                var listGastos = sapAccount.budget_conceptos_mantenimiento.Where(x => x.budget_rel_fy_centro.id_centro_costo == rel_fy_cc.id_centro_costo && x.mes == mes
                && x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio < rel_fy_cc.budget_anio_fiscal.anio_inicio && x.gasto != 0
                && x.moneda == currency
                ).OrderByDescending(x => x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio).Take(3).ToList();

                //realiaza la suma o toma el valor de 0
                sugerido = listGastos.Count == 0 ? 0 : listGastos.Sum(x => x.gasto - (x.one_time.HasValue ? x.one_time.Value : 0)) / listGastos.Count;

                foreach (var item in listGastos.OrderBy(x=>x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio)) {
                    html += @"
                        <div class=""form-group row"">
                            <label class=""control-label col-md-2 col-sm-2"" for=""anio_1"" style=""text-align:right"">FY "+(item.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio-2000)+"/"+ (item.budget_rel_fy_centro.budget_anio_fiscal.anio_fin - 2000) + @":</label>
                            <div class=""col-md-4"">
                                <input type=""text"" class=""form-control concepto-formula"" name=""anio_1"" id=""val_sugerido_{0}"" value="""+(item.gasto)+ @"""  readonly />
                            </div>
                            <label class=""control-label col-md-2 col-sm-2"" for=""anio_1"" style=""text-align:right"">One Time:</label>
                            <div class=""col-md-4"">
                                <input type=""text"" class=""form-control concepto-formula"" name=""anio_1"" id=""val_sugerido_{0}"" value="""+(item.one_time)+@"""  readonly />
                            </div>
                        </div>";
                }



                html += String.Format(@" 
                <hr/>
                <div class=""form-group row"">
                    <label class=""control-label col-md-8 col-sm-8"" for=""val_sugerido_{0}"" style=""text-align:right"">Promedio normalizado sugerido:</label>
                    <div class=""col-md-4"">
                        <input type=""text"" class=""form-control concepto-formula"" name=""val_sugerido_{0}"" id=""val_sugerido_{0}"" value=""{1}""  readonly />
                    </div>
                </div>
                <div class=""form-group row"">
                    <label class=""control-label col-md-8 col-sm-8"" for=""val_{0}"" style=""text-align:right"">Mantto. mayor:</label>
                    <div class=""col-md-4"">
                        <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" value=""{2}""  {3}/>
                        <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                    </div>
                </div>", "result", String.Format("{0:0.##}", Math.Round (sugerido,2)), String.Format("{0:0.##}", real), readOnly ? "readonly" : string.Empty);

                //envia form de gastos
                formulario[0] = new
                {
                    estatus = "OK",
                    html = html,
                    tieneComentarios = "false"
                };

                return Json(formulario, JsonRequestBehavior.AllowGet);
            }
            #endregion


            #region form html conceptos


            if (sapAccount.budget_rel_conceptos_formulas.Count == 0)
            {
                formulario[0] = new { estatus = "ERROR" };
                return Json(formulario, JsonRequestBehavior.AllowGet);
            }

            bool tieneComentarios = sapAccount.budget_rel_conceptos_formulas.Any(x => x.aplica_comentario);

            foreach (var concepto in sapAccount.budget_rel_conceptos_formulas)
            {
                //valor por defecto,  al cargar el formulario por primera vez
                string valor = currency == "MXN" ? concepto.valor_defecto_mxn.ToString() : currency == "USD" ? concepto.valor_defecto_usd.ToString() : currency == "EUR" ? concepto.valor_defecto_eur.ToString() : "0.0";

                string comentario = string.Empty;

                //valor formulario
                if (datosPrevios && (!concepto.valor_fijo.HasValue || !concepto.valor_fijo.Value))
                    switch (concepto.clave)
                    {
                        case "a":
                            valor = a;
                            comentario = c_a;
                            break;
                        case "b":
                            valor = b;
                            comentario = c_b;
                            break;
                        case "c":
                            valor = c;
                            comentario = c_c;
                            break;
                        case "d":
                            valor = d;
                            comentario = c_d;
                            break;
                        case "e":
                            valor = e;
                            comentario = c_e;
                            break;
                        case "f":
                            valor = f;
                            comentario = c_f;
                            break;
                        case "g":
                            valor = g;
                            comentario = c_g;
                            break;
                        case "h":
                            valor = h;
                            comentario = c_h;
                            break;
                        case "i":
                            valor = i;
                            comentario = c_i;
                            break;
                        case "j":
                            valor = j;
                            comentario = c_j;
                            break;
                        case "k":
                            valor = k;
                            comentario = c_k;
                            break;
                        case "l":
                            valor = l;
                            comentario = c_l;
                            break;
                        case "m":
                            valor = m;
                            comentario = c_m;
                            break;

                    }
                //si no toma el valor desde bd
                else if (bgCantidad != null && bgCantidad.budget_rel_conceptos_cantidades.Count > 0)
                {
                    valor = bgCantidad.budget_rel_conceptos_cantidades.FirstOrDefault(x => x.budget_rel_conceptos_formulas.clave == concepto.clave) != null ?
                        bgCantidad.budget_rel_conceptos_cantidades.FirstOrDefault(x => x.budget_rel_conceptos_formulas.clave == concepto.clave).cantidad.ToString() : "0";

                    comentario = bgCantidad.budget_rel_conceptos_cantidades.FirstOrDefault(x => x.budget_rel_conceptos_formulas.clave == concepto.clave) != null ?
                        bgCantidad.budget_rel_conceptos_cantidades.FirstOrDefault(x => x.budget_rel_conceptos_formulas.clave == concepto.clave).comentario : string.Empty;

                    //si no es read only toma el valor por defecto y no el de la BD
                    if (!readOnly && concepto.valor_fijo.HasValue && concepto.valor_fijo.Value)
                        valor = currency == "MXN" ? concepto.valor_defecto_mxn.ToString() : currency == "USD" ? concepto.valor_defecto_usd.ToString() : currency == "EUR" ? concepto.valor_defecto_eur.ToString() : "0.0";
                }

                //habilita o deshabilita conceptos segun cuenta sap
                bool readonlyConcepto = false;
                if ((cuenta_sap == "610030" || cuenta_sap == "610040") && (concepto.clave == "d" || concepto.clave == "e" || concepto.clave == "f") && currency == "MXN")
                    readonlyConcepto = true;
                if ((cuenta_sap == "610030" || cuenta_sap == "610040") && (concepto.clave == "a" || concepto.clave == "b" || concepto.clave == "c") && currency == "USD")
                    readonlyConcepto = true;



                //determina si aplica comentario o no
                if (tieneComentarios)
                {
                    if (concepto.aplica_comentario)
                    {
                        if (cuenta_sap == "610091") // Si es festejos
                        {
                            //determina la letra del evento
                            string eventoClave = string.Empty;
                            switch (concepto.clave)
                            {
                                case "a":
                                case "b":
                                    eventoClave = "A";
                                    break;
                                case "c":
                                case "d":
                                    eventoClave = "B";
                                    break;
                                case "e":
                                case "f":
                                    eventoClave = "C";
                                    break;
                                case "g":
                                case "h":
                                    eventoClave = "Otro";
                                    break;
                            }

                            //determina si mostra un select o campo abierto
                            string select = string.Empty;
                            if (concepto.clave != "g") // g = clave para otro
                            {
                                select = string.Format(@" <select class=""form-control select2bs4"" name=""comentario_{0}"" id=""comentario_{0}"" style =""width:100%"" {1}>
                                                           <option value="""">-- Seleccione --</option>
                                                           <option value=""Rosca de reyes"" " + (comentario == "Rosca de reyes" ? "selected" : string.Empty) + @">Rosca de reyes</option>
                                                           <option value=""Día del amor y la amistad"" " + (comentario == "Día del amor y la amistad" ? "selected" : string.Empty) + @">Día del amor y la amistad</option>
                                                           <option value=""Día de la mujer"" " + (comentario == "Día de la mujer" ? "selected" : string.Empty) + @">Día de la mujer</option>
                                                           <option value=""Día de la familia""  " + (comentario == "Día de la familia" ? "selected" : string.Empty) + @">Día de la familia</option>
                                                           <option value=""Día de las madres""  " + (comentario == "Día de las madres" ? "selected" : string.Empty) + @">Día de las madres</option>
                                                           <option value=""Día del padre"" " + (comentario == "Día del padre" ? "selected" : string.Empty) + @">Día del padre</option>
                                                           <option value=""Semana de la salud""  " + (comentario == "Semana de la salud" ? "selected" : string.Empty) + @">Semana de la salud</option>
                                                           <option value=""Torneo de fútbol"" " + (comentario == "Torneo de fútbol" ? "selected" : string.Empty) + @">Torneo de fútbol</option>
                                                           <option value=""Kermés (día de la independencia)""  " + (comentario == "Kermés (día de la independencia)" ? "selected" : string.Empty) + @">Kermés (día de la independencia)</option>
                                                           <option value=""Día de muertos"" " + (comentario == "Día de muertos" ? "selected" : string.Empty) + @">Día de muertos</option>
                                                           <option value=""Fiesta fin de año"" " + (comentario == "Fiesta fin de año" ? "selected" : string.Empty) + @">Fiesta fin de año</option>
                                                         </select>", concepto.clave, readOnly ? "disabled" : string.Empty, comentario);
                            }
                            else
                            {//es otro 
                                select = string.Format(@"<input type=""text"" class=""form-control"" name=""comentario_{0}"" id=""comentario_{0}""  value=""{2}"" maxlength=""80"" {1}/>"
                                                        , concepto.clave, readOnly ? "disabled" : string.Empty, comentario);
                            }

                            //agrega los input correspondientes
                            html += String.Format(@"
                                                <input type=""hidden"" name=""id_rel_concepto_{0}"" id=""id_rel_concepto_{0}"" value=""" + concepto.id + @""">
                                                <input type=""hidden"" name=""concepto_clave_{0}"" id=""concepto_clave_{0}"" value=""" + concepto.clave + @""">
                                                <div class=""form-group row"" style=""{5}"">
                                                    <label class=""control-label col-md-4 col-sm-4"" for=""val_{0}"" style=""text-align:right"">{1}:</label>
                                                    <div class=""col-md-2"">
                                                        <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" {2} value=""{3}"" {4}/>
                                                        <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                                                    </div>
                                                    <label class=""control-label col-md-2 col-sm-2"" for=""comentario_{0}"" style=""text-align:right"">Evento {6}:</label>
                                                    <div class=""col-md-4"">
                                                         {7}             
                                                        <span class=""field-validation-valid text-danger"" data-valmsg-for=""comentario_{0}"" data-valmsg-replace=""true""></span>
                                                    </div>
                                                </div>                                                
                                            ", concepto.clave, concepto.descripcion, concepto.valor_fijo.HasValue && concepto.valor_fijo.Value ? "readonly" : string.Empty
                                                            , valor, readOnly ? "readonly" : string.Empty, readonlyConcepto ? "display:none" : string.Empty,
                                                            eventoClave, select
                                                            );
                        }
                        else  //si aplica comentario, pero no es festojos
                        {
                            html += String.Format(@"
                        <input type=""hidden"" name=""id_rel_concepto_{0}"" id=""id_rel_concepto_{0}"" value=""" + concepto.id + @""">
                        <input type=""hidden"" name=""concepto_clave_{0}"" id=""concepto_clave_{0}"" value=""" + concepto.clave + @""">
                        <div class=""form-group row"" style=""{5}"">
                            <label class=""control-label col-md-4 col-sm-4"" for=""val_{0}"" style=""text-align:right"">{1}:</label>
                            <div class=""col-md-2"">
                                <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" {2} value=""{3}"" {4}/>
                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                            </div>
                            <label class=""control-label col-md-1 col-sm-1"" for=""comentario_{0}"" style=""text-align:right"">Descrip.:</label>
                            <div class=""col-md-5"">
                                <input type=""text"" class=""form-control"" name=""comentario_{0}"" id=""comentario_{0}"" {2} value=""{6}"" maxlength=""80"" {4}/>
                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""comentario_{0}"" data-valmsg-replace=""true""></span>
                            </div>
                        </div>
                        ", concepto.clave, concepto.descripcion, concepto.valor_fijo.HasValue && concepto.valor_fijo.Value ? "readonly" : string.Empty
                                    , valor, readOnly ? "readonly" : string.Empty, readonlyConcepto ? "display:none" : string.Empty, comentario);
                        }
                    }
                    else  //este campo no tiene comentarios, pero si hay comentarios en otros campos
                    {
                        html += String.Format(@"
                        <input type=""hidden"" name=""id_rel_concepto_{0}"" id=""id_rel_concepto_{0}"" value=""" + concepto.id + @""">
                        <input type=""hidden"" name=""concepto_clave_{0}"" id=""concepto_clave_{0}"" value=""" + concepto.clave + @""">
                        <div class=""form-group row"" style=""{5}"">
                            <label class=""control-label col-md-4 col-sm-4"" for=""val_{0}"" style=""text-align:right"">{1}:</label>
                            <div class=""col-md-2"">
                                <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" {2} value=""{3}"" {4}/>
                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                            </div>                            
                        </div>
                         {6}"
                        , concepto.clave, concepto.descripcion, concepto.valor_fijo.HasValue && concepto.valor_fijo.Value ? "readonly" : string.Empty
                                   , valor, readOnly ? "readonly" : string.Empty, readonlyConcepto ? "display:none" : string.Empty
                                   , cuenta_sap == "610091" ? "<hr />" : string.Empty //si es el último campo de cada evento de festejos agrega una division
                                   );
                    }
                }
                else //no hay comentarios en todo el form
                {
                    html += String.Format(@"
                        <input type=""hidden"" name=""id_rel_concepto_{0}"" id=""id_rel_concepto_{0}"" value=""" + concepto.id + @""">
                        <input type=""hidden"" name=""concepto_clave_{0}"" id=""concepto_clave_{0}"" value=""" + concepto.clave + @""">
                        <div class=""form-group row"" style=""{5}"">
                            <label class=""control-label col-md-6 col-sm-6"" for=""val_{0}"" style=""text-align:right"">{1}:</label>
                            <div class=""col-md-6"">
                                <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" {2} value=""{3}"" {4}/>
                                <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                            </div>
                        </div>
                       ", concepto.clave, concepto.descripcion, concepto.valor_fijo.HasValue && concepto.valor_fijo.Value ? "readonly" : string.Empty
                            , valor, readOnly ? "readonly" : string.Empty, readonlyConcepto ? "display:none" : string.Empty
                            );
                }
            }

            html += String.Format(@" <div class=""form-group row"">
                    <label class=""control-label col-md-{2} col-sm-{2}"" for=""val_{0}"" style=""text-align:right"">{1}:</label>
                    <div class=""col-md-{3}"">
                        <input type=""text"" class=""form-control concepto-formula"" name=""val_{0}"" id=""val_{0}"" readonly />
                        <span class=""field-validation-valid text-danger"" data-valmsg-for=""val_{0}"" data-valmsg-replace=""true""></span>
                    </div>
                </div>", "result", "Total", tieneComentarios ? 4 : 6, tieneComentarios ? 2 : 6);

            #endregion
            formulario[0] = new
            {
                estatus = "OK",
                html = html,
                tieneComentarios = tieneComentarios ? "true" : "false"
            };

            return Json(formulario, JsonRequestBehavior.AllowGet);
        }


        ///<summary>
        ///Obtiene el botonDeDocumento
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneBotonDocumento(int? id_rel_fy_cc, int? id_cuenta_sap)
        {

            //obtiene todos los posibles valores
            budget_rel_documento doc = db.budget_rel_documento.FirstOrDefault(x => x.id_budget_rel_fy_centro == id_rel_fy_cc && x.id_cuenta_sap == id_cuenta_sap);

            //inicializa la lista de objetos
            var response = new object[1];

            if (doc == null)
            { //no hay correo
                response[0] = new
                {
                    correcto = true,
                    html = "<button class='btn-documento-pendiente' onclick=\"muestraSoporte(" + id_rel_fy_cc + ", " +
                        id_cuenta_sap + ")\">Agregar</button>"
                };
            }
            else //hay correo
            {
                response[0] = new
                {
                    correcto = true,
                    html = "<button class='btn-documento-agregado' onclick=\"muestraSoporte(" + id_rel_fy_cc + ", " +
                        id_cuenta_sap + ")\">Modificar</button>"
                };
            }

            return Json(response, JsonRequestBehavior.AllowGet);
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

        public ActionResult ActualizaGastosManto()
        {

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            //crea el select list para plantas
            List<SelectListItem> newList = new List<SelectListItem>();

            DateTime fecha = new DateTime(2000, 1, 1);
            for (int i = 1; i <= 12; i++)
            {
                newList.Add(new SelectListItem()
                {
                    Text = i.ToString() + " - " + fecha.ToString("MMMM"),
                    Value = i.ToString(),
                });

                fecha = fecha.AddMonths(1);
            }

            //envia el select list por viewbag
            ViewBag.mes = AddFirstItem(new SelectList(newList, "Value", "Text"), textoPorDefecto: "-- Todos --");
            ViewBag.id_centro_costo = AddFirstItem(new SelectList(db.budget_centro_costo, "id", "ConcatCentro"), textoPorDefecto: "-- Todos --");
            ViewBag.sap_account = AddFirstItem(new SelectList(db.budget_cuenta_sap.Where(x => x.aplica_gastos_mantenimiento), nameof(budget_cuenta_sap.id), nameof(budget_cuenta_sap.sap_account)), textoPorDefecto: "-- Todos --");
            ViewBag.id_fiscal_year = AddFirstItem(new SelectList(db.budget_anio_fiscal.Where(x => x.id != 1), "id", "ConcatAnio"), textoPorDefecto: "-- Todos --");

            return View();
        }


        // POST: IT_site/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizaGastosManto(int? mes, int? id_centro_costo, int? id_fiscal_year, int? sap_account)
        {

            List<string> monedas = new List<string> { "MXN", "USD", "EUR" };
            List<int> meses = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            //obtiene el año fiscal
            var fiscal_year = db.budget_anio_fiscal.Find(id_fiscal_year);

            //obtiene las cuentas sap donde aplica mantenimiento
            var satAccountList = db.budget_cuenta_sap.Where(x => (sap_account == null || sap_account == x.id) && x.aplica_gastos_mantenimiento);
            //rel fy centro costo,
            var ccList = db.budget_centro_costo.Where(x => (id_centro_costo == null || id_centro_costo == x.id));
            //listas de cantidades sap
            var cantidadList = db.budget_cantidad.Where(x => satAccountList.Any(y => y.id == x.id_cuenta_sap) && meses.Any(y => y == x.mes)
            && x.budget_rel_fy_centro.budget_anio_fiscal.id == id_fiscal_year
            );

            //obtiene los ultimos tres gastos
            var listGastosTotal = db.budget_conceptos_mantenimiento.Where(x => satAccountList.Any(y => y.id == x.id_cuenta_sap) && meses.Any(y => y == x.mes)
            && x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio < fiscal_year.anio_inicio);

            //var listGastos = sapAccount.budget_conceptos_mantenimiento.Where(x => x.budget_rel_fy_centro.id_centro_costo == rel_fy_cc.id_centro_costo && x.mes == mes
            //&& x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio < rel_fy_cc.budget_anio_fiscal.anio_inicio && x.gasto != 0
            //&& x.moneda == currency
            //).OrderByDescending(x => x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio).Take(3).ToList();
            DateTime fecha_actual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            //recorre cada cc
            foreach (var cc in ccList/*.Where(x => listGastosTotal.Any(y => y.budget_rel_fy_centro.budget_centro_costo.id == x.id))*/)
            {
                var rel_fy_cc = cc.budget_rel_fy_centro.Where(x => x.id_anio_fiscal == id_fiscal_year).FirstOrDefault();

                //recorre cada cuenta sap
                foreach (var sapAccount in satAccountList/*.Where(x => listGastosTotal.Any(y => y.budget_cuenta_sap.id == x.id))*/)
                {

                    //recorre cada mes
                    foreach (var mesItem in meses.Where(x => (mes == null || mes == x) /*&& listGastosTotal.Any(y => y.mes == x)*/))
                    {
                        DateTime fechaComparacion = DateTime.Now;

                        if (mesItem >= 10)
                            fechaComparacion = new DateTime(rel_fy_cc.budget_anio_fiscal.anio_inicio, mesItem, 1);
                        else
                            fechaComparacion = new DateTime(rel_fy_cc.budget_anio_fiscal.anio_fin, mesItem, 1);

                        if (fechaComparacion < fecha_actual) //si es actual se salta la iteracion
                            continue;

                        //recorre cada moneda
                        foreach (var moneda in monedas/*.Where(x => listGastosTotal.Any(y => y.moneda == x))*/)
                        {
                            List<budget_conceptos_mantenimiento> listGastos = sapAccount.budget_conceptos_mantenimiento.Where(x =>
                                x.budget_rel_fy_centro.id_centro_costo == rel_fy_cc.id_centro_costo && x.mes == mesItem
                               && x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio < rel_fy_cc.budget_anio_fiscal.anio_inicio && x.gasto != 0
                                && x.moneda == moneda
                            ).OrderByDescending(x => x.budget_rel_fy_centro.budget_anio_fiscal.anio_inicio).Take(3).ToList();

                            var sugerido = listGastos.Count == 0 ? 0 : listGastos.Sum(x => x.gasto - (x.one_time.HasValue ? x.one_time.Value : 0)) / listGastos.Count;
                            sugerido = Math.Round(sugerido, 2);

                            //actualiza/crea budget cantidad con el valor sugerido
                            var cantidad = sapAccount.budget_cantidad.Where(x => x.mes == mesItem && x.currency_iso == moneda && x.moneda_local_usd != true && x.id_budget_rel_fy_centro == rel_fy_cc.id).FirstOrDefault();

                            if (cantidad != null) //existe, entonces lo modifica
                            {
                                cantidad.cantidad = (decimal)sugerido;
                            }
                            else if (sugerido > 0)
                            { //lo crea solo si es mayor a 0
                                budget_cantidad nueva_cantidad = new budget_cantidad
                                {
                                    id_budget_rel_fy_centro = rel_fy_cc.id,
                                    id_cuenta_sap = sapAccount.id,
                                    mes = mesItem,
                                    currency_iso = moneda,
                                    cantidad = (decimal)sugerido,
                                    moneda_local_usd = false
                                };
                                db.budget_cantidad.Add(nueva_cantidad);
                            }

                            //una vez modificada la cantidad tambien el valor de local usd
                            //actualiza/crea budget cantidad con el valor sugerido
                            var cantidad_local = sapAccount.budget_cantidad.Where(x => x.mes == mesItem && x.currency_iso == "USD" && x.moneda_local_usd == true && x.id_budget_rel_fy_centro == rel_fy_cc.id).FirstOrDefault();

                            //calcula moneda local 
                            decimal local = 0;

                            var mxn_usd = (decimal)rel_fy_cc.budget_anio_fiscal.budget_rel_tipo_cambio_fy.Where(x => x.id_tipo_cambio == 1).FirstOrDefault().cantidad;
                            var eur_usd = (decimal)rel_fy_cc.budget_anio_fiscal.budget_rel_tipo_cambio_fy.Where(x => x.id_tipo_cambio == 2).FirstOrDefault().cantidad;

                            //suma USD
                            decimal d_usd = sapAccount.budget_cantidad.Where(x => x.mes == mesItem && x.currency_iso == "USD" && x.moneda_local_usd == false && x.id_budget_rel_fy_centro == rel_fy_cc.id).Sum(x => x.cantidad);
                            //MXN
                            decimal d_mxn_usd = sapAccount.budget_cantidad.Where(x => x.mes == mesItem && x.currency_iso == "MXN" && x.moneda_local_usd == false && x.id_budget_rel_fy_centro == rel_fy_cc.id).Sum(x => x.cantidad)
                                / mxn_usd;
                            //EUR
                            decimal d_eur_usd = sapAccount.budget_cantidad.Where(x => x.mes == mesItem && x.currency_iso == "EUR" && x.moneda_local_usd == false && x.id_budget_rel_fy_centro == rel_fy_cc.id).Sum(x => x.cantidad)
                                * eur_usd;

                            local = (moneda == "USD" ? (decimal)sugerido : d_usd) + (moneda == "MXN" ? Math.Round((decimal)sugerido / mxn_usd,2) : d_mxn_usd) + (moneda == "EUR" ? Math.Round( (decimal)sugerido * eur_usd,2) : d_eur_usd);
                            local = Math.Round(local, 2);

                            if (cantidad_local != null) //existe, entonces lo modifica
                            {
                                cantidad_local.cantidad = local;
                            }
                            else if (local > 0) //solo lo agrega si el numero es mayor a cero
                            {
                                budget_cantidad nueva_cantidad = new budget_cantidad
                                {
                                    id_budget_rel_fy_centro = rel_fy_cc.id,
                                    id_cuenta_sap = sapAccount.id,
                                    mes = mesItem,
                                    currency_iso = "USD",
                                    cantidad = local,
                                    moneda_local_usd = true
                                };

                                db.budget_cantidad.Add(nueva_cantidad);
                            }
                        }
                    }


                }
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert(e.Message, TipoMensajesSweetAlerts.ERROR);
            }


            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("ActualizaGastosManto");
        }

        public ActionResult EditarSoporte(int? id_rel_fy_cc, int? id_cuenta_sap)
        {

            if (!TieneRol(TipoRoles.BG_RESPONSABLE) && !TieneRol(TipoRoles.BG_CONTROLLING))
                return View("../Home/ErrorPermisos");

            if (id_rel_fy_cc == null || id_cuenta_sap == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            budget_rel_documento documento = db.budget_rel_documento.FirstOrDefault(x => x.id_budget_rel_fy_centro == id_rel_fy_cc && x.id_cuenta_sap == id_cuenta_sap);

            //en caso de que no haya un documento en BD, genera uno vacio
            if (documento == null)
            {
                documento = new budget_rel_documento
                {
                    id_budget_rel_fy_centro = id_rel_fy_cc.Value,
                    id_cuenta_sap = id_cuenta_sap.Value,
                    budget_rel_fy_centro = db.budget_rel_fy_centro.Find(id_rel_fy_cc),
                    budget_cuenta_sap = db.budget_cuenta_sap.Find(id_cuenta_sap)
                };
            }
            return View(documento);
        }

        // POST: PolizaManual/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarSoporte(budget_rel_documento relDocumento, bool borrar)
        {
            //si es borrar
            if (borrar)
            {
                //borra el rel
                var relD = db.budget_rel_documento.Find(relDocumento.id);
                db.budget_rel_documento.Remove(relD);
                //borra el archivo de biblioteca digital
                db.biblioteca_digital.Remove(db.biblioteca_digital.Find(relDocumento.id_documento));

                db.SaveChanges();
                //TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return View("../RU_registros/Message");
            }

            biblioteca_digital archivo = new biblioteca_digital { };

            //verifica si el tamaño del archivo es válido
            if (relDocumento.ArchivoSoporte != null && relDocumento.ArchivoSoporte.InputStream.Length > 10485760)
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 10MB.");
            else if (relDocumento.ArchivoSoporte != null)
            { //verifica la extensión del archivo
                string extension = Path.GetExtension(relDocumento.ArchivoSoporte.FileName);
                if (extension.ToUpper() != ".XLS"   //si no contiene una extensión válida
                               && extension.ToUpper() != ".XLSX"
                               && extension.ToUpper() != ".DOC"
                               && extension.ToUpper() != ".DOCX"
                               && extension.ToUpper() != ".PDF"
                               && extension.ToUpper() != ".PNG"
                               && extension.ToUpper() != ".JPG"
                               && extension.ToUpper() != ".JPEG"
                               && extension.ToUpper() != ".RAR"
                               && extension.ToUpper() != ".ZIP"
                               && extension.ToUpper() != ".EML"
                               )
                    ModelState.AddModelError("", "Sólo se permiten archivos con extensión .xls, .xlsx, .doc, .docx, .pdf, .png, .jpg, .jpeg, .rar, .zip. y .eml");
                else
                { //si la extension y el tamaño son válidos
                    String nombreArchivo = relDocumento.ArchivoSoporte.FileName;

                    //recorta el nombre del archivo en caso de ser necesario
                    if (nombreArchivo.Length > 80)
                        nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                    //lee el archivo en un arreglo de bytes
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(relDocumento.ArchivoSoporte.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(relDocumento.ArchivoSoporte.ContentLength);
                    }

                    //si tiene archivo hace un update sino hace un create
                    if (relDocumento.id_documento.HasValue)//si tiene valor hace un update
                    {
                        archivo = db.biblioteca_digital.Find(relDocumento.id_documento.Value);
                        archivo.Nombre = nombreArchivo;
                        archivo.MimeType = UsoStrings.RecortaString(relDocumento.ArchivoSoporte.ContentType, 80);
                        archivo.Datos = fileData;

                        db.Entry(archivo).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    { //si no tiene hace un create 

                        //genera el archivo de biblioteca digital
                        archivo = new biblioteca_digital
                        {
                            Nombre = nombreArchivo,
                            MimeType = UsoStrings.RecortaString(relDocumento.ArchivoSoporte.ContentType, 80),
                            Datos = fileData
                        };

                        //update en BD
                        db.biblioteca_digital.Add(archivo);
                        db.SaveChanges();
                    }

                }
            }

            if (ModelState.IsValid)
            {

                //verifica si se creo un archivo
                if (archivo.Id > 0) //si existe algún archivo en biblioteca digital
                    relDocumento.id_documento = archivo.Id;


                //si existe lo modifica
                budget_rel_documento documento = db.budget_rel_documento.Find(relDocumento.id);

                if (documento != null)
                {
                    documento.id_documento = relDocumento.id_documento;
                    documento.comentario = relDocumento.comentario;
                }
                else
                { //no existe, lo crea
                    db.budget_rel_documento.Add(relDocumento);
                }

                db.SaveChanges();
                //TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return View("../RU_registros/Message");
            }

            if (relDocumento.id_documento.HasValue && relDocumento.id_documento.Value > 0)
                relDocumento.biblioteca_digital = db.biblioteca_digital.Find(relDocumento.id_documento);

            relDocumento.budget_rel_fy_centro = db.budget_rel_fy_centro.Find(relDocumento.id_budget_rel_fy_centro);
            relDocumento.budget_cuenta_sap = db.budget_cuenta_sap.Find(relDocumento.id_cuenta_sap);
            return View(relDocumento);
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
