using Bitacoras.Util;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNet.SignalR;
using SpreadsheetLight;
using SpreadsheetLight.Charts;
using SpreadsheetLight.Drawing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;


namespace Portal_2_0.Models
{
    public class ExcelUtil
    {

        public static byte[] GeneraReporteBitacorasExcel(List<view_historico_resultado> listado, plantas planta, bool tipo_turno = false)
        {

            string plantilla = String.Empty;
            string codigoDoc = String.Empty;
            switch (planta.clave)
            {
                case 1:
                default:
                    //para puebla y default
                    plantilla = "~/Content/plantillas_excel/plantilla_reporte_produccion_historial_cambios_puebla.xlsx";
                    codigoDoc = "PRF014-04";
                    break;
                case 2:
                    //para silao
                    plantilla = "~/Content/plantillas_excel/plantilla_reporte_produccion_historial_cambios_silao.xlsx";
                    codigoDoc = "PRF005-04";
                    break;

            }


            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath(plantilla), "Sheet1");
            Portal_2_0Entities db = new Portal_2_0Entities();

            System.Data.DataTable dt = new System.Data.DataTable();

            //para llevar el control de si es encabezado o no
            List<bool> filasEncabezados = new List<bool>();
            List<bool> filasTemporales = new List<bool>();
            //agrega tres para el encabezadoprincipal
            filasEncabezados.Add(false); //es el encabezado principal
            filasTemporales.Add(false);
            filasEncabezados.Add(false); //es el encabezado principal
            filasTemporales.Add(false);
            filasEncabezados.Add(false); //es el encabezado principal
            filasTemporales.Add(false);

            //columnas
            dt.Columns.Add(nameof(view_historico_resultado.Planta), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Linea), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Operador), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Supervisor), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Fecha), typeof(DateTime));
            dt.Columns.Add(nameof(view_historico_resultado.Hora), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Turno), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Orden_SAP), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.SAP_Platina), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Tipo_de_Material), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Número_de_Parte__de_cliente), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Material), typeof(string));
            dt.Columns.Add("Tipo_de_Metal", typeof(string));
            dt.Columns.Add("Mill", typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Orden_en_SAP_2), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.SAP_Platina_2), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Tipo_de_Material_platina2), typeof(string));            
            dt.Columns.Add(nameof(view_historico_resultado.Número_de_Parte_de_Cliente_platina2), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Material_platina2), typeof(string));
            dt.Columns.Add("Tipo_de_Metal_platina2", typeof(string));
            dt.Columns.Add("Mill_platina2", typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.ConcatCliente), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.SAP_Rollo), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.N__de_Rollo), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Lote_de_rollo), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Etiqueta__Kg_), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_de_regreso_de_rollo_Real), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_de_rollo_usado), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Báscula_Kgs), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Pieza_por_Golpe), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Ordenes_por_pieza), typeof(double));
            dt.Columns.Add("Lote_Material", typeof(string));
            dt.Columns.Add("No_lote_izq", typeof(int));
            dt.Columns.Add("No_lote_der", typeof(int));
            dt.Columns.Add("Lote_piezas_por_paquete", typeof(int));
            dt.Columns.Add(nameof(view_historico_resultado.Total_de_piezas_platina1), typeof(int));
            dt.Columns.Add(nameof(view_historico_resultado.Total_de_piezas_platina2), typeof(int));
            dt.Columns.Add(nameof(view_historico_resultado.Total_de_piezas), typeof(int));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_de_rollo_consumido), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Numero_de_golpes), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Kg_restante_de_rollo), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_despunte_kgs_), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_cola_Kgs_), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Porcentaje_de_puntas_y_colas), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Total_de_piezas_de_Ajustes_platina1), typeof(int));
            dt.Columns.Add(nameof(view_historico_resultado.Total_de_piezas_de_Ajustes_platina2), typeof(int));
            dt.Columns.Add(nameof(view_historico_resultado.Total_de_piezas_de_Ajustes), typeof(double));

            dt.Columns.Add(nameof(view_historico_resultado.Peso_Bruto_Kgs), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Real_Pieza_Bruto), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Real_Pieza_Neto), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Scrap_Natural), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_neto_SAP), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Bruto_SAP), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Balance_de_Scrap), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_de_rollo_usado_real__Kg), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_bruto_Total_piezas_Kg), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_NetoTotal_piezas_Kg), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Neto_total_piezas_de_ajuste_Kgs), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_puntas_y_colas_reales_Kg), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Balance_de_Scrap_Real), typeof(double));

            dt.Columns.Add(nameof(view_historico_resultado.Peso_Bruto_Kgs_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Real_Pieza_Bruto_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Real_Pieza_Neto_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Scrap_Natural_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_neto_SAP_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Bruto_SAP_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Balance_de_Scrap_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_de_rollo_usado_real__Kg_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_bruto_Total_piezas_Kg_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_NetoTotal_piezas_Kg_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Neto_total_piezas_de_ajuste_Kgs_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_puntas_y_colas_reales_Kg_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Balance_de_Scrap_Real_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Bruto_Kgs_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Real_Pieza_Bruto_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Real_Pieza_Neto_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Scrap_Natural_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_neto_SAP_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Bruto_SAP_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Balance_de_Scrap_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_de_rollo_usado_real__Kg_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_bruto_Total_piezas_Kg_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_NetoTotal_piezas_Kg_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Neto_total_piezas_de_ajuste_Kgs_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_puntas_y_colas_reales_Kg_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Balance_de_Scrap_Real_general), typeof(double));
            dt.Columns.Add("¿Posteado?", typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.comentario), typeof(string));

            var listaIds = listado.Where(x => x.IdRegistro.HasValue).Select(x => x.IdRegistro).Distinct().ToList();

            var datosProduccionRegistrosDBLista = db.produccion_registros.Where(x => listaIds.Contains(x.id)).ToList();


            // --- INICIO: CÓDIGO CORREGIDO PARA OBTENER DATOS ADICIONALES ---
            // 1. Recolectar todos los códigos SAP_Platina necesarios de ambas columnas.
            var sapPlatinas = listado.Select(i => i.SAP_Platina)
                                     .Union(listado.Select(i => i.SAP_Platina_2))
                                     .Where(s => !string.IsNullOrEmpty(s))
                                     .Distinct()
                                     .ToList();

            // 2. Consultar la BD UNA SOLA VEZ por cada tabla.
            var mmData = db.mm_v3
                .Where(m => sapPlatinas.Contains(m.Material))
                .GroupBy(m => m.Material) // <-- CAMBIO CLAVE: Agrupamos por Material para manejar duplicados
                .ToDictionary(g => g.Key, g => g.First()); // y tomamos solo el primer elemento de cada grupo.

            var classData = db.class_v3
                .Where(c => sapPlatinas.Contains(c.Object))
                .ToDictionary(c => c.Object, c => c);
            // --- FIN: CÓDIGO CORREGIDO ---

            foreach (view_historico_resultado item in listado)
            {
                // System.Diagnostics.Debug.WriteLine(index + "/" + listado.Count);

                if (String.IsNullOrEmpty(item.SAP_Platina_2))
                {
                    item.Peso_Bruto_Kgs_platina2 = null;
                    item.Peso_Real_Pieza_Bruto_platina2 = null;
                    item.Peso_Real_Pieza_Neto_platina2 = null;
                    item.Scrap_Natural_platina2 = null;
                    item.Peso_neto_SAP_platina2 = null;
                    item.Peso_Bruto_SAP_platina2 = null;
                    item.Balance_de_Scrap_platina2 = null;
                    item.Peso_de_rollo_usado_real__Kg_platina2 = null;
                    item.Peso_bruto_Total_piezas_Kg_platina2 = null;
                    item.Peso_NetoTotal_piezas_Kg_platina2 = null;
                    item.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg_platina2 = null;
                    item.Peso_Neto_total_piezas_de_ajuste_Kgs_platina2 = null;
                    item.Peso_puntas_y_colas_reales_Kg_platina2 = null;
                    item.Balance_de_Scrap_Real_platina2 = null;
                }

                // --- INICIO: BÚSQUEDA DE NUEVOS CAMPOS EN LOS DICCIONARIOS ---
                string mill1 = null;
                // <-- CORRECCIÓN: Se usa SAP_Platina para buscar en classData
                if (!string.IsNullOrEmpty(item.SAP_Platina) && classData.ContainsKey(item.SAP_Platina))
                {
                    mill1 = classData[item.SAP_Platina].Mill;
                }

                string tipoMetal1 = null;
                if (!string.IsNullOrEmpty(item.SAP_Platina) && mmData.ContainsKey(item.SAP_Platina))
                {
                    tipoMetal1 = mmData[item.SAP_Platina].Type_of_Metal;
                }

                string mill2 = null;
                // <-- CORRECCIÓN: Se usa SAP_Platina_2 para buscar en classData
                if (!string.IsNullOrEmpty(item.SAP_Platina_2) && classData.ContainsKey(item.SAP_Platina_2))
                {
                    mill2 = classData[item.SAP_Platina_2].Mill;
                }

                string tipoMetal2 = null;
                if (!string.IsNullOrEmpty(item.SAP_Platina_2) && mmData.ContainsKey(item.SAP_Platina_2))
                {
                    tipoMetal2 = mmData[item.SAP_Platina_2].Type_of_Metal;
                }
                // --- FIN: BÚSQUEDA CORREGIDA ---

            

                //encuentra el valor de produccion registro
                produccion_registros p = null;
                //busca si tiene registro en el nuevo sistema

                p = datosProduccionRegistrosDBLista.FirstOrDefault(x => x.id == item.Column40);

                string posteado = p != null && p.produccion_datos_entrada != null && p.produccion_datos_entrada.posteado ? "SÍ" : "NO";

                dt.Rows.Add(item.Planta, item.Linea, item.Operador, item.Supervisor, item.Fecha, String.Format("{0:T}", item.Hora), item.Turno, item.Orden_SAP, item.SAP_Platina,
                    item.Tipo_de_Material, item.Número_de_Parte__de_cliente, item.Material,
                    mill1, // <-- NUEVO
                    tipoMetal1, // <-- NUEVO
                    item.Orden_en_SAP_2, item.SAP_Platina_2, item.Tipo_de_Material_platina2, item.Número_de_Parte_de_Cliente_platina2, item.Material_platina2,
                    mill2, // <-- NUEVO
                    tipoMetal2, // <-- NUEVO
                    item.ConcatCliente, item.SAP_Rollo, item.N__de_Rollo, item.Lote_de_rollo, item.Peso_Etiqueta__Kg_, item.Peso_de_regreso_de_rollo_Real,
                    item.Peso_de_rollo_usado, item.Peso_Báscula_Kgs, item.Pieza_por_Golpe, item.Ordenes_por_pieza, null, null, null, null, item.Total_de_piezas_platina1, item.Total_de_piezas_platina2, item.Total_de_piezas,
                    item.Peso_de_rollo_consumido, item.Numero_de_golpes, item.Kg_restante_de_rollo, item.Peso_despunte_kgs_, item.Peso_cola_Kgs_, item.Porcentaje_de_puntas_y_colas,
                    item.Total_de_piezas_de_Ajustes_platina1, item.Total_de_piezas_de_Ajustes_platina2, item.Total_de_piezas_de_Ajustes,
                    item.Peso_Bruto_Kgs, item.Peso_Real_Pieza_Bruto, item.Peso_Real_Pieza_Neto, item.Scrap_Natural, item.Peso_neto_SAP, item.Peso_Bruto_SAP, item.Balance_de_Scrap,
                    item.Peso_de_rollo_usado_real__Kg, item.Peso_bruto_Total_piezas_Kg, item.Peso_NetoTotal_piezas_Kg, item.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg,
                    item.Peso_Neto_total_piezas_de_ajuste_Kgs, item.Peso_puntas_y_colas_reales_Kg, item.Balance_de_Scrap_Real,
                    item.Peso_Bruto_Kgs_platina2, item.Peso_Real_Pieza_Bruto_platina2, item.Peso_Real_Pieza_Neto_platina2, item.Scrap_Natural_platina2, item.Peso_neto_SAP_platina2, item.Peso_Bruto_SAP_platina2, item.Balance_de_Scrap_platina2,
                    item.Peso_de_rollo_usado_real__Kg_platina2, item.Peso_bruto_Total_piezas_Kg_platina2, item.Peso_NetoTotal_piezas_Kg_platina2, item.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg_platina2,
                    item.Peso_Neto_total_piezas_de_ajuste_Kgs_platina2, item.Peso_puntas_y_colas_reales_Kg_platina2, item.Balance_de_Scrap_Real_platina2,
                    item.Peso_Bruto_Kgs_general, item.Peso_Real_Pieza_Bruto_general, item.Peso_Real_Pieza_Neto_general, item.Scrap_Natural_general, item.Peso_neto_SAP_general, item.Peso_Bruto_SAP_general, item.Balance_de_Scrap_general,
                    item.Peso_de_rollo_usado_real__Kg_general, item.Peso_bruto_Total_piezas_Kg_general, item.Peso_NetoTotal_piezas_Kg_general, item.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg_general,
                    item.Peso_Neto_total_piezas_de_ajuste_Kgs_general, item.Peso_puntas_y_colas_reales_Kg_general, item.Balance_de_Scrap_Real_general,
                    posteado,
                    item.comentario
                 );

                filasEncabezados.Add(true);

                //verifica es una fila temporal
                if (item.SAP_Platina.ToUpper().Contains("TEMPORAL") || item.SAP_Rollo.ToUpper().Contains("TEMPORAL"))
                    filasTemporales.Add(true);
                else
                    filasTemporales.Add(false);


                //obtiene la cantidad de fila actual
                int fila_inicial = filasEncabezados.Count + 1;

                //si tiene registro, agrega los lotes
                if (p != null)
                {
                    foreach (produccion_lotes lote in p.produccion_lotes.Where(x => (x.sap_platina == item.SAP_Platina || x.sap_platina == item.SAP_Platina_2 || string.IsNullOrEmpty(x.sap_platina))))
                    {


                        System.Data.DataRow row = dt.NewRow();

                        if (!String.IsNullOrEmpty(lote.sap_platina))
                            row["Lote_Material"] = lote.sap_platina;
                        else
                            row["Lote_Material"] = DBNull.Value;

                        if (lote.numero_lote_izquierdo.HasValue)
                            row["No_lote_izq"] = lote.numero_lote_izquierdo.Value;
                        else
                            row["No_lote_izq"] = DBNull.Value;

                        if (lote.numero_lote_derecho.HasValue)
                            row["No_lote_der"] = lote.numero_lote_derecho.Value;
                        else
                            row["No_lote_der"] = DBNull.Value;

                        if (lote.piezas_paquete.HasValue)
                            row["Lote_piezas_por_paquete"] = lote.piezas_paquete.Value;
                        else
                            row["Lote_piezas_por_paquete"] = DBNull.Value;


                        dt.Rows.Add(row);

                        filasEncabezados.Add(false);
                        filasTemporales.Add(false);
                    }
                }
                //obtiene la fila final
                int fila_final = filasEncabezados.Count + 1;

                //verifica si hubo cambios
                if (fila_inicial != fila_final)
                {
                    oSLDocument.GroupRows(fila_inicial, fila_final - 1);
                }
                //index++;
            }

            double sumaRolloUsado = listado.Sum(item => item.Peso_de_rollo_usado.HasValue ? item.Peso_de_rollo_usado.Value : 0);
            double sumaNumGolpes = listado.Sum(item => item.Numero_de_golpes.HasValue ? item.Numero_de_golpes.Value : 0);
            //double promedioBalanceScrap = listado.Average(item => item.Balance_de_Scrap.HasValue ? item.Balance_de_Scrap.Value : 0);
            //double promedioBalanceScrapReal = listado.Average(item => item.Balance_de_Scrap_Real.HasValue ? item.Balance_de_Scrap_Real.Value : 0);
            //double promedioBalanceScrapPlatina2 = listado.Average(item => item.Balance_de_Scrap_platina2.HasValue ? item.Balance_de_Scrap_platina2.Value : 0);
            //double promedioBalanceScrapRealPlatina2 = listado.Average(item => item.Balance_de_Scrap_Real_platina2.HasValue ? item.Balance_de_Scrap_Real_platina2.Value : 0);
            double promedioBalanceScrapGeneral = listado.Average(item => item.Balance_de_Scrap_general.HasValue ? item.Balance_de_Scrap_general.Value : 0);
            double promedioBalanceScrapRealGeneral = listado.Average(item => item.Balance_de_Scrap_Real_general.HasValue ? item.Balance_de_Scrap_Real_general.Value : 0);

            if (tipo_turno)
            {
                //fila para sumatorias
                System.Data.DataRow row = dt.NewRow();
                row[nameof(view_historico_resultado.Peso_de_rollo_usado)] = sumaRolloUsado;
                row[nameof(view_historico_resultado.Numero_de_golpes)] = sumaNumGolpes;
                //row[nameof(view_historico_resultado.Balance_de_Scrap)] = promedioBalanceScrap;
                //row[nameof(view_historico_resultado.Balance_de_Scrap_Real)] = promedioBalanceScrapReal;
                //row[nameof(view_historico_resultado.Balance_de_Scrap_platina2)] = promedioBalanceScrapPlatina2;
                //row[nameof(view_historico_resultado.Balance_de_Scrap_Real_platina2)] = promedioBalanceScrapRealPlatina2;
                row[nameof(view_historico_resultado.Balance_de_Scrap_general)] = promedioBalanceScrapGeneral;
                row[nameof(view_historico_resultado.Balance_de_Scrap_Real_general)] = promedioBalanceScrapRealGeneral;
                dt.Rows.Add(row);

            }


            //crea la hoja de FACTURAS y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Bitácora Producción");
            oSLDocument.ImportDataTable(4, 1, dt, false); //fase omite el encabezado

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;


            //estilo para el encabezado de cada fila
            SLStyle styleHeaderRow = oSLDocument.CreateStyle();
            styleHeaderRow.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#daeef3"), System.Drawing.ColorTranslator.FromHtml("#daeef3"));

            //estilo para el encabezado de cada fila
            SLStyle styleHeaderRowTemporal = oSLDocument.CreateStyle();
            styleHeaderRowTemporal.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffa0a2"), System.Drawing.ColorTranslator.FromHtml("#ffa0a2"));


            

            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;


            //estilo para sumatorias
            if (tipo_turno)
            {
                SLStyle styleFooter = oSLDocument.CreateStyle();
                styleFooter.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#c6efce"), System.Drawing.ColorTranslator.FromHtml("#c6efce"));
                styleFooter.Font.Bold = true;
                styleFooter.Font.FontColor = System.Drawing.ColorTranslator.FromHtml("#006100");
                oSLDocument.SetCellStyle(filasEncabezados.Count + 1, 1, filasEncabezados.Count, dt.Columns.Count, styleFooter);
            }

            //aplica formato a las filas de encabezado
            for (int i = 0; i < filasEncabezados.Count; i++)
            {
                if (filasEncabezados[i])
                {
                    oSLDocument.SetCellStyle(i + 1, 1, i + 1, dt.Columns.Count, styleHeaderRow);
                }
                else if (i >= 3)
                {
                    oSLDocument.SetCellStyle(i + 1, 28, i + 1, 31, styleLoteInfo);
                }
                //colapsa todas las filas
                oSLDocument.CollapseRows(i + 3);
            }

            //Aplica formato a los temporales
            for (int i = 0; i < filasTemporales.Count; i++)
            {
                if (filasTemporales[i])
                {
                    oSLDocument.SetCellStyle(i + 1, 1, i + 1, dt.Columns.Count, styleHeaderRowTemporal);
                }

            }

            ////inmoviliza el encabezado
            oSLDocument.FreezePanes(3, 0);


            oSLDocument.SetRowHeight(4, filasEncabezados.Count + 1, 15.0);

            //inserta una celda al inicio                    
            oSLDocument.AutoFitColumn(1, 17);

            //combina las celdas
            oSLDocument.SetCellValue("J1", codigoDoc);
            oSLDocument.SetCellValue("A1", DateTime.Now.ToShortDateString());

            //Inserta pie de página
            oSLDocument.MergeWorksheetCells(dt.Rows.Count + 6, 3, dt.Rows.Count + 6, 4);
            oSLDocument.MergeWorksheetCells(dt.Rows.Count + 6, 5, dt.Rows.Count + 6, 7);
            oSLDocument.SetRowHeight(dt.Rows.Count + 6, 50.0);

            //copia de la hoja Aux
            oSLDocument.CopyCellFromWorksheet("Aux", 1, 1, 1, 7, dt.Rows.Count + 6, 2);

            //establece la clave del documento
            oSLDocument.SetCellValue(dt.Rows.Count + 6, 8, codigoDoc);

            SLPicture pic = new SLPicture(HttpContext.Current.Server.MapPath("~/Content/images/logo_1.png"));
            // set the top of the picture to be halfway in row 3
            // and the left of the picture to be halfway in column 1
            pic.ResizeInPercentage(30, 30);
            pic.SetPosition(dt.Rows.Count + 5.1, 1);
            oSLDocument.InsertPicture(pic);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        ///// <summary>
        ///// Genera reporte de IT_Mantenimientos
        ///// </summary>
        ///// <param name="listado"></param>
        ///// <returns></returns>
        //public static byte[] GeneraReporteITMantenimientos(List<IT_mantenimientos> listado)
        //{

        //    SLDocument oSLDocument = new SLDocument();


        //    System.Data.DataTable dt = new System.Data.DataTable();

        //    //columnas          
        //    dt.Columns.Add("Folio", typeof(string));
        //    dt.Columns.Add("Equipo", typeof(string));
        //    dt.Columns.Add("Tipo", typeof(string));
        //    dt.Columns.Add("Planta", typeof(string));
        //    dt.Columns.Add("Marca", typeof(string));
        //    dt.Columns.Add("Modelo", typeof(string));
        //    dt.Columns.Add("Serie", typeof(string));
        //    dt.Columns.Add("Fecha Programada", typeof(DateTime));   //8
        //    dt.Columns.Add("Fecha Realización", typeof(DateTime));  //9
        //    dt.Columns.Add("Estatus", typeof(string));
        //    dt.Columns.Add("Documento", typeof(string));
        //    dt.Columns.Add("Firma de Aceptación", typeof(string));
        //    dt.Columns.Add("Responsable Principal (Actual)", typeof(string));



        //    ////registros , rows
        //    foreach (var item in listado)
        //    {
        //        dt.Rows.Add(item.id, item.IT_inventory_items.hostname, item.IT_inventory_items.IT_inventory_hardware_type.descripcion, item.IT_inventory_items.plantas.descripcion, item.IT_inventory_items.brand
        //            , item.IT_inventory_items.model, item.IT_inventory_items.serial_number, item.fecha_programada, item.fecha_realizacion, Bitacoras.Util.IT_matenimiento_Estatus.DescripcionStatus(item.estatus),
        //            item.id_biblioteca_digital.HasValue ? "Subido" : "Pendiente", item.empleados != null ? item.empleados.ConcatNombre : String.Empty,
        //            item.responsable_principal
        //            );
        //    }

        //    //crea la hoja de FACTURAS y la selecciona
        //    oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Mantenimientos IT");
        //    oSLDocument.ImportDataTable(1, 1, dt, true);

        //    //estilo para ajustar al texto
        //    SLStyle styleWrap = oSLDocument.CreateStyle();
        //    styleWrap.SetWrapText(true);

        //    //estilo para el encabezado
        //    SLStyle styleHeader = oSLDocument.CreateStyle();
        //    styleHeader.Font.Bold = true;
        //    styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

        //    ////estilo para fecha
        //    SLStyle styleLongDate = oSLDocument.CreateStyle();
        //    styleLongDate.FormatCode = "mmmm/yyyy";
        //    oSLDocument.SetColumnStyle(8, styleLongDate);
        //    oSLDocument.SetColumnStyle(9, styleLongDate);

        //    SLStyle styleHeaderFont = oSLDocument.CreateStyle();
        //    styleHeaderFont.Font.FontName = "Calibri";
        //    styleHeaderFont.Font.FontSize = 11;
        //    styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
        //    styleHeaderFont.Font.Bold = true;

        //    //da estilo a la hoja de excel
        //    //inmoviliza el encabezado
        //    oSLDocument.FreezePanes(1, 0);

        //    oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
        //    oSLDocument.AutoFitColumn(1, dt.Columns.Count);

        //    oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
        //    oSLDocument.SetRowStyle(1, styleHeader);
        //    oSLDocument.SetRowStyle(1, styleHeaderFont);

        //    oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

        //    System.IO.Stream stream = new System.IO.MemoryStream();

        //    oSLDocument.SaveAs(stream);

        //    byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

        //    return (array);
        //}
        /// <summary>
        /// Genera reporte de IT_Mantenimientos (plan de Mantenimiento)
        /// </summary>
        /// <param name="listado"></param>
        /// <returns></returns>
        public static byte[] GeneraReporteITPlanMantenimientos(List<IT_mantenimientos> listado)
        {

            SLDocument oSLDocument = new SLDocument();

            #region hoja1

            System.Data.DataTable dt = new System.Data.DataTable();

            //columnas          
            dt.Columns.Add("Folio", typeof(string));
            dt.Columns.Add("Equipo", typeof(string));
            dt.Columns.Add("Tipo", typeof(string));
            dt.Columns.Add("Planta", typeof(string));
            dt.Columns.Add("Marca", typeof(string));
            dt.Columns.Add("Modelo", typeof(string));
            dt.Columns.Add("Serie", typeof(string));
            dt.Columns.Add("Fecha Programada", typeof(DateTime));   //8
            dt.Columns.Add("Fecha Realización", typeof(DateTime));  //9
            dt.Columns.Add("Estatus", typeof(string));
            dt.Columns.Add("Documento", typeof(string));
            dt.Columns.Add("Firma de Aceptación", typeof(string));
            dt.Columns.Add("Responsable Principal (Actual)", typeof(string));



            ////registros , rows
            foreach (var item in listado)
            {
                dt.Rows.Add(item.id, item.IT_inventory_items.hostname, item.IT_inventory_items.IT_inventory_hardware_type.descripcion, item.IT_inventory_items.plantas.descripcion, item.IT_inventory_items.brand
                    , item.IT_inventory_items.model, item.IT_inventory_items.serial_number, item.fecha_programada, item.fecha_realizacion, Bitacoras.Util.IT_matenimiento_Estatus.DescripcionStatus(item.estatus),
                    item.id_biblioteca_digital.HasValue ? "Subido" : "Pendiente", item.empleados != null ? item.empleados.ConcatNombre : String.Empty,
                    item.responsable_principal
                    );
            }

            //crea la hoja de FACTURAS y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Mantenimientos IT");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "mmmm/yyyy";
            oSLDocument.SetColumnStyle(8, styleShortDate);
            oSLDocument.SetColumnStyle(9, styleShortDate);

            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            #endregion

            #region hoja2

            //comienza la segunda hoja
            string hojaControlCambios = "Control de Cambios";
            //crear la hoja y la selecciona
            oSLDocument.AddWorksheet(hojaControlCambios);
            oSLDocument.SelectWorksheet(hojaControlCambios);

            //reinicia la tabla
            dt = new System.Data.DataTable();

            dt.Columns.Add("No.", typeof(int));
            dt.Columns.Add("Responsable del cambio", typeof(string));
            dt.Columns.Add("Fecha", typeof(DateTime));
            dt.Columns.Add("Descripción del Cambio", typeof(string));

            dt.Rows.Add(1, "Gerente de Sistemas", new DateTime(2018, 1, 22), "Se da de alta el documento en el sistema.");
            dt.Rows.Add(2, "Network administrator", new DateTime(2021, 1, 04), "Se sustituye el plan de mantenimiento por el calendario anual, se realiza por procesos.");
            dt.Rows.Add(3, "Alfredo Xochitemol Cruz", new DateTime(2022, 9, 1), "Se genera plan de mantenimientos a través del Portal tkMM.");

            oSLDocument.MergeWorksheetCells("A1", "D1");
            oSLDocument.SetCellValue("A1", "Control de Cambios");


            oSLDocument.ImportDataTable(2, 1, dt, true);

            SLStyle styleLongDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy-mm-dd";
            oSLDocument.SetColumnStyle(3, styleShortDate);

            oSLDocument.FreezePanes(2, 0);
            //oSLDocument.Filter(2, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(2, dt.Columns.Count);


            styleHeaderFont.Alignment.Horizontal = HorizontalAlignmentValues.Center;

            oSLDocument.SetColumnStyle(2, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, 2, styleHeader);
            oSLDocument.SetRowStyle(1, 2, styleHeaderFont);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            #endregion

            oSLDocument.SelectWorksheet("Mantenimientos IT");

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReportePMExcel(List<poliza_manual> listado)
        {

            SLDocument oSLDocument = new SLDocument();

            System.Data.DataTable dt = new System.Data.DataTable();

            //para llevar el control de si es encabezado o no
            List<bool> filasEncabezados = new List<bool>();
            filasEncabezados.Add(false); //es el encabezado principal

            //columnas          
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Tipo Póliza", typeof(string));
            dt.Columns.Add("Moneda", typeof(string));
            dt.Columns.Add("Planta", typeof(string));
            dt.Columns.Add("Núm. docto. SAP", typeof(string));
            dt.Columns.Add("Descripción", typeof(string));
            dt.Columns.Add("Fecha documento", typeof(DateTime)); //7
            dt.Columns.Add("Cuenta", typeof(string));//8
            dt.Columns.Add("CC", typeof(string));
            dt.Columns.Add("Concepto", typeof(string));
            dt.Columns.Add("Poliza", typeof(string));
            dt.Columns.Add("Debe", typeof(double));
            dt.Columns.Add("Haber", typeof(double));
            dt.Columns.Add("Total Debe", typeof(decimal));
            dt.Columns.Add("Total Haber", typeof(decimal));
            dt.Columns.Add("Elaboró", typeof(string));
            dt.Columns.Add("Fecha Creación", typeof(DateTime));//17
            dt.Columns.Add("Validó (área)", typeof(string));
            dt.Columns.Add("Fecha Validación", typeof(DateTime));
            dt.Columns.Add("Autorizó (doble validación)", typeof(string));
            dt.Columns.Add("Fecha Autorización", typeof(DateTime));
            dt.Columns.Add("Dirección", typeof(string));
            dt.Columns.Add("Fecha Dirección", typeof(DateTime)); //23
            dt.Columns.Add("Registró (contabilidad)", typeof(string));
            dt.Columns.Add("Fecha Registro", typeof(DateTime)); //25
            dt.Columns.Add("Estado", typeof(string)); //25

            ////registros , rows
            foreach (poliza_manual item in listado)
            {
                System.Data.DataRow row = dt.NewRow();
                row["ID"] = item.id;
                row["Tipo Póliza"] = item.PM_tipo_poliza.descripcion;
                row["Moneda"] = item.currency_iso;
                row["Planta"] = item.plantas.descripcion;
                row["Núm. docto. SAP"] = item.numero_documento_sap;
                row["Descripción"] = item.descripcion_poliza;
                row["Fecha documento"] = item.fecha_documento;

                if (item.empleados3 != null)
                    row["Elaboró"] = item.empleados3.ConcatNombre;
                else
                    row["Elaboró"] = DBNull.Value;

                row["Fecha Creación"] = item.fecha_creacion;

                //validación area
                if (item.empleados4 != null)
                    row["Validó (área)"] = item.empleados4.ConcatNombre;
                else
                    row["Validó (área)"] = DBNull.Value;

                if (item.fecha_validacion.HasValue)
                    row["Fecha Validación"] = item.fecha_validacion;
                else
                    row["Fecha Validación"] = DBNull.Value;

                //autorización
                if (item.empleados != null)
                    row["Autorizó (doble validación)"] = item.empleados.ConcatNombre;
                else
                    row["Autorizó (doble validación)"] = DBNull.Value;


                if (item.fecha_autorizacion.HasValue)
                    row["Fecha Autorización"] = item.fecha_autorizacion;
                else
                    row["Fecha Autorización"] = DBNull.Value;


                if (item.empleados2 != null && item.fecha_direccion.HasValue)
                {
                    row["Dirección"] = item.empleados2.ConcatNombre;
                    row["Fecha Dirección"] = item.fecha_direccion;
                }
                else
                {
                    row["Dirección"] = DBNull.Value;
                    row["Fecha Dirección"] = DBNull.Value;
                }

                if (item.empleados1 != null)
                    row["Registró (contabilidad)"] = item.empleados1.ConcatNombre;
                else
                    row["Registró (contabilidad)"] = DBNull.Value;


                if (item.fecha_registro.HasValue)
                    row["Fecha Registro"] = item.fecha_registro;
                else
                    row["Fecha Registro"] = DBNull.Value;

                row["Total Debe"] = item.totalDebe;
                row["Total Haber"] = item.totalHaber;

                row["Estado"] = Bitacoras.Util.PM_Status.DescripcionStatus(item.estatus);

                dt.Rows.Add(row);
                filasEncabezados.Add(true);
                //obtiene la cantidad de fila actual
                int fila_inicial = filasEncabezados.Count + 1;

                //detalle
                foreach (PM_conceptos c in item.PM_conceptos)
                {
                    row = dt.NewRow();

                    row["Cuenta"] = c.cuenta;
                    row["CC"] = c.cc;
                    row["Concepto"] = c.concepto;
                    row["Poliza"] = c.poliza;

                    if (c.debe.HasValue)
                        row["Debe"] = c.debe.Value;
                    else
                        row["Debe"] = DBNull.Value;

                    if (c.haber.HasValue)
                        row["Haber"] = c.haber.Value;
                    else
                        row["Haber"] = DBNull.Value;

                    dt.Rows.Add(row);
                    filasEncabezados.Add(false);

                }

                //obtiene la fila final
                int fila_final = filasEncabezados.Count + 1;

                //verifica si hubo cambios
                if (fila_inicial != fila_final)
                    oSLDocument.GroupRows(fila_inicial, fila_final - 1);
            }


            //crea la hoja de FACTURAS y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Reporte Pólizas Manuales");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            //estilo para el encabezado de cada fila
            SLStyle styleHeaderRow = oSLDocument.CreateStyle();
            styleHeaderRow.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#daeef3"), System.Drawing.ColorTranslator.FromHtml("#daeef3"));

            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(7, styleShortDate);

            SLStyle styleLongDate = oSLDocument.CreateStyle();
            styleLongDate.FormatCode = "yyyy/MM/dd h:mm AM/PM";

            oSLDocument.SetColumnStyle(7, styleShortDate);
            oSLDocument.SetColumnStyle(17, styleLongDate);
            oSLDocument.SetColumnStyle(19, styleLongDate);
            oSLDocument.SetColumnStyle(21, styleLongDate);
            oSLDocument.SetColumnStyle(23, styleLongDate);
            oSLDocument.SetColumnStyle(25, styleLongDate);

            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //estilo para numeros
            SLStyle styleNumber = oSLDocument.CreateStyle();
            styleNumber.FormatCode = "#,##0.00";

            //da estilo a los numero
            oSLDocument.SetColumnStyle(12, 15, styleNumber);

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            //aplica formato a las filas de encabezado
            for (int i = 0; i < filasEncabezados.Count; i++)
            {
                if (filasEncabezados[i])
                {
                    oSLDocument.SetRowStyle(i + 1, styleHeaderRow);
                }
                else
                {
                    oSLDocument.SetCellStyle(i + 1, 8, i + 1, 13, styleLoteInfo);
                }
                //colapsa todas las filas
                oSLDocument.CollapseRows(i + 2);
            }

            oSLDocument.Filter("A1", "Z1");
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);

            oSLDocument.SetRowHeight(1, filasEncabezados.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        /// <summary>
        /// Crea un archivo excel para los datos por centro de costo
        /// </summary>
        /// <param name="valoresListAnioAnterior"></param>
        /// <param name="valoresListAnioActual"></param>
        /// <param name="valoresListAnioProximo"></param>
        /// <returns></returns>
        public static byte[] GeneraReporteBudgetPorCentroCosto(budget_centro_costo centro, List<view_valores_fiscal_year> valoresListAnioAnterior, List<view_valores_fiscal_year> valoresListAnioActual,
            List<view_valores_fiscal_year> valoresListAnioProximo, budget_anio_fiscal anio_Fiscal_anterior, budget_anio_fiscal anio_Fiscal_actual, budget_anio_fiscal anio_Fiscal_proximo)
        {
            DateTime fechaActual = DateTime.Now;

            bool isActualOctubre = anio_Fiscal_actual.isActual(10) == "ACT";
            bool isActualNoviembre = anio_Fiscal_actual.isActual(11) == "ACT";
            bool isActualDiciembre = anio_Fiscal_actual.isActual(12) == "ACT";
            bool isActualEnero = anio_Fiscal_actual.isActual(1) == "ACT";
            bool isActualFebrero = anio_Fiscal_actual.isActual(2) == "ACT";
            bool isActualMarzo = anio_Fiscal_actual.isActual(3) == "ACT";
            bool isActualAbril = anio_Fiscal_actual.isActual(4) == "ACT";
            bool isActualMayo = anio_Fiscal_actual.isActual(5) == "ACT";
            bool isActualJunio = anio_Fiscal_actual.isActual(6) == "ACT";
            bool isActualJulio = anio_Fiscal_actual.isActual(7) == "ACT";
            bool isActualAgosto = anio_Fiscal_actual.isActual(8) == "ACT";
            bool isActualSeptiembre = anio_Fiscal_actual.isActual(9) == "ACT";

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");


            //crea los datos principales del centro de costo

            oSLDocument.SetCellValue("B1", "thyssenkrupp Materials de México");
            oSLDocument.MergeWorksheetCells(1, 2, 1, 4);

            oSLDocument.SetCellValue("B2", "Cost Center");
            oSLDocument.SetCellValue("B3", "Deparment");
            oSLDocument.SetCellValue("B4", "Responsable");
            oSLDocument.SetCellValue("C2", centro.num_centro_costo);
            oSLDocument.SetCellValue("C3", centro.budget_departamentos.descripcion);

            String responsables = (string.Join("/", centro.budget_responsables.Select(x => x.empleados.ConcatNombre).ToArray()));

            oSLDocument.SetCellValue("C4", responsables);

            System.Data.DataTable dt = new System.Data.DataTable();

            //columnas          
            dt.Columns.Add("Item", typeof(string));
            dt.Columns.Add("Sap Account", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Mapping Bridge", typeof(string));

            //Meses para Actual
            string titulo_anterior_octubre = "ACT " + MesesUtil.OCTUBRE.Abreviation + "-" + anio_Fiscal_anterior.anio_inicio.ToString().Substring(2, 2);
            string titulo_anterior_noviembre = "ACT " + MesesUtil.NOVIEMBRE.Abreviation + "-" + anio_Fiscal_anterior.anio_inicio.ToString().Substring(2, 2);
            string titulo_anterior_diciembre = "ACT " + MesesUtil.DICIEMBRE.Abreviation + "-" + anio_Fiscal_anterior.anio_inicio.ToString().Substring(2, 2);
            string titulo_anterior_enero = "ACT " + MesesUtil.ENERO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_febrero = "ACT " + MesesUtil.FEBRERO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_marzo = "ACT " + MesesUtil.MARZO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_abril = "ACT " + MesesUtil.ABRIL.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_mayo = "ACT " + MesesUtil.MAYO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_junio = "ACT " + MesesUtil.JUNIO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_julio = "ACT " + MesesUtil.JULIO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_agosto = "ACT " + MesesUtil.AGOSTO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_septiembre = "ACT " + MesesUtil.SEPTIEMBRE.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_total_mxn = "Total MXN FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1)));
            string titulo_anterior_total_usd = "Total USD FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1)));
            string titulo_anterior_total_eur = "Total EUR FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1)));
            string titulo_anterior_total_local = "Total USD Local FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1)));
            string comentarios_anterior = "Comentarios " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1)));

            //list con los los meses del año anterior
            List<string> meses_anterior = new List<string> {
                    titulo_anterior_octubre,
                    titulo_anterior_noviembre,
                    titulo_anterior_diciembre,
                    titulo_anterior_enero,
                    titulo_anterior_febrero,
                    titulo_anterior_marzo,
                    titulo_anterior_abril,
                    titulo_anterior_mayo,
                    titulo_anterior_junio,
                    titulo_anterior_julio,
                    titulo_anterior_agosto,
                    titulo_anterior_septiembre
            };

            //agrega las columnas a las tablas
            foreach (var item in meses_anterior)
            {
                dt.Columns.Add("MXN_" + item, typeof(decimal));
                dt.Columns.Add("USD_" + item, typeof(decimal));
                dt.Columns.Add("EUR_" + item, typeof(decimal));
                dt.Columns.Add("USD Local_" + item, typeof(decimal));
            }

            dt.Columns.Add(titulo_anterior_total_mxn, typeof(decimal));
            dt.Columns.Add(titulo_anterior_total_usd, typeof(decimal));
            dt.Columns.Add(titulo_anterior_total_eur, typeof(decimal));
            dt.Columns.Add(titulo_anterior_total_local, typeof(decimal));
            dt.Columns.Add(comentarios_anterior, typeof(string));

            //meses para actual/forecast
            string titulo_actual_octubre = (isActualOctubre ? "ACT" : "FC") + " " + MesesUtil.OCTUBRE.Abreviation + "-" + anio_Fiscal_actual.anio_inicio.ToString().Substring(2, 2);
            string titulo_actual_noviembre = (isActualNoviembre ? "ACT" : "FC") + " " + MesesUtil.NOVIEMBRE.Abreviation + "-" + anio_Fiscal_actual.anio_inicio.ToString().Substring(2, 2);
            string titulo_actual_diciembre = (isActualDiciembre ? "ACT" : "FC") + " " + MesesUtil.DICIEMBRE.Abreviation + "-" + anio_Fiscal_actual.anio_inicio.ToString().Substring(2, 2);
            string titulo_actual_enero = (isActualEnero ? "ACT" : "FC") + " " + MesesUtil.ENERO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_febrero = (isActualFebrero ? "ACT" : "FC") + " " + MesesUtil.FEBRERO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_marzo = (isActualMarzo ? "ACT" : "FC") + " " + MesesUtil.MARZO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_abril = (isActualAbril ? "ACT" : "FC") + " " + MesesUtil.ABRIL.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_mayo = (isActualMayo ? "ACT" : "FC") + " " + MesesUtil.MAYO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_junio = (isActualJunio ? "ACT" : "FC") + " " + MesesUtil.JUNIO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_julio = (isActualJulio ? "ACT" : "FC") + " " + MesesUtil.JULIO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_agosto = (isActualAgosto ? "ACT" : "FC") + " " + MesesUtil.AGOSTO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_septiembre = (isActualSeptiembre ? "ACT" : "FC") + " " + MesesUtil.SEPTIEMBRE.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_total_mxn = "Total MXN FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual));
            string titulo_actual_total_usd = "Total USD FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual));
            string titulo_actual_total_eur = "Total EUR FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual));
            string titulo_actual_total_local = "Total USD Local FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual));
            string comentarios_presente = "Comentarios " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual));

            //list con los los meses del año anterior
            List<string> meses_presente = new List<string> {
                    titulo_actual_octubre,
                    titulo_actual_noviembre,
                    titulo_actual_diciembre,
                    titulo_actual_enero,
                    titulo_actual_febrero,
                    titulo_actual_marzo,
                    titulo_actual_abril,
                    titulo_actual_mayo,
                    titulo_actual_junio,
                    titulo_actual_julio,
                    titulo_actual_agosto,
                    titulo_actual_septiembre
            };


            //agrega las columnas a las tablas
            foreach (var item in meses_presente)
            {
                dt.Columns.Add("MXN_" + item, typeof(decimal));
                dt.Columns.Add("USD_" + item, typeof(decimal));
                dt.Columns.Add("EUR_" + item, typeof(decimal));
                dt.Columns.Add("USD Local_" + item, typeof(decimal));
            }

            dt.Columns.Add(titulo_actual_total_mxn, typeof(decimal));
            dt.Columns.Add(titulo_actual_total_usd, typeof(decimal));
            dt.Columns.Add(titulo_actual_total_eur, typeof(decimal));
            dt.Columns.Add(titulo_actual_total_local, typeof(decimal));
            dt.Columns.Add(comentarios_presente, typeof(string));

            //meses para budget
            string titulo_proximo_octubre = "BG " + MesesUtil.OCTUBRE.Abreviation + "-" + anio_Fiscal_proximo.anio_inicio.ToString().Substring(2, 2);
            string titulo_proximo_noviembre = "BG " + MesesUtil.NOVIEMBRE.Abreviation + "-" + anio_Fiscal_proximo.anio_inicio.ToString().Substring(2, 2);
            string titulo_proximo_diciembre = "BG " + MesesUtil.DICIEMBRE.Abreviation + "-" + anio_Fiscal_proximo.anio_inicio.ToString().Substring(2, 2);
            string titulo_proximo_enero = "BG " + MesesUtil.ENERO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_febrero = "BG " + MesesUtil.FEBRERO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_marzo = "BG " + MesesUtil.MARZO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_abril = "BG " + MesesUtil.ABRIL.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_mayo = "BG " + MesesUtil.MAYO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_junio = "BG " + MesesUtil.JUNIO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_julio = "BG " + MesesUtil.JULIO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_agosto = "BG " + MesesUtil.AGOSTO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_septiembre = "BG " + MesesUtil.SEPTIEMBRE.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_total_mxn = "Total MXN FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1)));
            string titulo_proximo_total_usd = "Total USD FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1)));
            string titulo_proximo_total_eur = "Total EUR FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1)));
            string titulo_proximo_total_local = "Total USD Local FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1)));
            string comentarios_proximo = "Comentarios " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1)));

            //list con los los meses del año anterior
            List<string> meses_proximo = new List<string> {
                    titulo_proximo_octubre,
                    titulo_proximo_noviembre,
                    titulo_proximo_diciembre,
                    titulo_proximo_enero,
                    titulo_proximo_febrero,
                    titulo_proximo_marzo,
                    titulo_proximo_abril,
                    titulo_proximo_mayo,
                    titulo_proximo_junio,
                    titulo_proximo_julio,
                    titulo_proximo_agosto,
                    titulo_proximo_septiembre
            };


            //agrega las columnas a las tablas
            foreach (var item in meses_proximo)
            {
                dt.Columns.Add("MXN_" + item, typeof(decimal));
                dt.Columns.Add("USD_" + item, typeof(decimal));
                dt.Columns.Add("EUR_" + item, typeof(decimal));
                dt.Columns.Add("USD Local_" + item, typeof(decimal));
            }

            dt.Columns.Add(titulo_proximo_total_mxn, typeof(decimal));
            dt.Columns.Add(titulo_proximo_total_usd, typeof(decimal));
            dt.Columns.Add(titulo_proximo_total_eur, typeof(decimal));
            dt.Columns.Add(titulo_proximo_total_local, typeof(decimal));
            dt.Columns.Add(comentarios_proximo, typeof(string));

            for (int i = 0; i < valoresListAnioAnterior.Count; i++)
            {
                System.Data.DataRow row = dt.NewRow();

                //Inserta los datos de la cienta

                row["Item"] = i + 1;
                row["Sap Account"] = valoresListAnioAnterior[i].sap_account;
                row["Name"] = valoresListAnioAnterior[i].name;
                row["Mapping Bridge"] = valoresListAnioAnterior[i].mapping_bridge;

                //completa valores para el año anterior
                #region valores anio pasado
                //agrega las columnas a las tablas

                //octubre
                if (valoresListAnioAnterior[i].Octubre_MXN.HasValue)
                    row["MXN_" + titulo_anterior_octubre] = valoresListAnioAnterior[i].Octubre_MXN;
                else
                    row["MXN_" + titulo_anterior_octubre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Octubre.HasValue)
                    row["USD_" + titulo_anterior_octubre] = valoresListAnioAnterior[i].Octubre;
                else
                    row["USD_" + titulo_anterior_octubre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Octubre_EUR.HasValue)
                    row["EUR_" + titulo_anterior_octubre] = valoresListAnioAnterior[i].Octubre_EUR;
                else
                    row["EUR_" + titulo_anterior_octubre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Octubre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_octubre] = valoresListAnioAnterior[i].Octubre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_octubre] = DBNull.Value;
                //noviembre
                if (valoresListAnioAnterior[i].Noviembre_MXN.HasValue)
                    row["MXN_" + titulo_anterior_noviembre] = valoresListAnioAnterior[i].Noviembre_MXN;
                else
                    row["MXN_" + titulo_anterior_noviembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Noviembre.HasValue)
                    row["USD_" + titulo_anterior_noviembre] = valoresListAnioAnterior[i].Noviembre;
                else
                    row["USD_" + titulo_anterior_noviembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Noviembre_EUR.HasValue)
                    row["EUR_" + titulo_anterior_noviembre] = valoresListAnioAnterior[i].Noviembre_EUR;
                else
                    row["EUR_" + titulo_anterior_noviembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Noviembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_noviembre] = valoresListAnioAnterior[i].Noviembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_noviembre] = DBNull.Value;
                //diciembre
                if (valoresListAnioAnterior[i].Diciembre_MXN.HasValue)
                    row["MXN_" + titulo_anterior_diciembre] = valoresListAnioAnterior[i].Diciembre_MXN;
                else
                    row["MXN_" + titulo_anterior_diciembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Diciembre.HasValue)
                    row["USD_" + titulo_anterior_diciembre] = valoresListAnioAnterior[i].Diciembre;
                else
                    row["USD_" + titulo_anterior_diciembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Diciembre_EUR.HasValue)
                    row["EUR_" + titulo_anterior_diciembre] = valoresListAnioAnterior[i].Diciembre_EUR;
                else
                    row["EUR_" + titulo_anterior_diciembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Diciembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_diciembre] = valoresListAnioAnterior[i].Diciembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_diciembre] = DBNull.Value;
                //enero
                if (valoresListAnioAnterior[i].Enero_MXN.HasValue)
                    row["MXN_" + titulo_anterior_enero] = valoresListAnioAnterior[i].Enero_MXN;
                else
                    row["MXN_" + titulo_anterior_enero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Enero.HasValue)
                    row["USD_" + titulo_anterior_enero] = valoresListAnioAnterior[i].Enero;
                else
                    row["USD_" + titulo_anterior_enero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Enero_EUR.HasValue)
                    row["EUR_" + titulo_anterior_enero] = valoresListAnioAnterior[i].Enero_EUR;
                else
                    row["EUR_" + titulo_anterior_enero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Enero_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_enero] = valoresListAnioAnterior[i].Enero_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_enero] = DBNull.Value;
                //febrero
                if (valoresListAnioAnterior[i].Febrero_MXN.HasValue)
                    row["MXN_" + titulo_anterior_febrero] = valoresListAnioAnterior[i].Febrero_MXN;
                else
                    row["MXN_" + titulo_anterior_febrero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Febrero.HasValue)
                    row["USD_" + titulo_anterior_febrero] = valoresListAnioAnterior[i].Febrero;
                else
                    row["USD_" + titulo_anterior_febrero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Febrero_EUR.HasValue)
                    row["EUR_" + titulo_anterior_febrero] = valoresListAnioAnterior[i].Febrero_EUR;
                else
                    row["EUR_" + titulo_anterior_febrero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Febrero_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_febrero] = valoresListAnioAnterior[i].Febrero_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_febrero] = DBNull.Value;
                //marzo
                if (valoresListAnioAnterior[i].Marzo_MXN.HasValue)
                    row["MXN_" + titulo_anterior_marzo] = valoresListAnioAnterior[i].Marzo_MXN;
                else
                    row["MXN_" + titulo_anterior_marzo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Marzo.HasValue)
                    row["USD_" + titulo_anterior_marzo] = valoresListAnioAnterior[i].Marzo;
                else
                    row["USD_" + titulo_anterior_marzo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Marzo_EUR.HasValue)
                    row["EUR_" + titulo_anterior_marzo] = valoresListAnioAnterior[i].Marzo_EUR;
                else
                    row["EUR_" + titulo_anterior_marzo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Marzo_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_marzo] = valoresListAnioAnterior[i].Marzo_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_marzo] = DBNull.Value;
                //abril
                if (valoresListAnioAnterior[i].Abril_MXN.HasValue)
                    row["MXN_" + titulo_anterior_abril] = valoresListAnioAnterior[i].Abril_MXN;
                else
                    row["MXN_" + titulo_anterior_abril] = DBNull.Value;

                if (valoresListAnioAnterior[i].Abril.HasValue)
                    row["USD_" + titulo_anterior_abril] = valoresListAnioAnterior[i].Abril;
                else
                    row["USD_" + titulo_anterior_abril] = DBNull.Value;

                if (valoresListAnioAnterior[i].Abril_EUR.HasValue)
                    row["EUR_" + titulo_anterior_abril] = valoresListAnioAnterior[i].Abril_EUR;
                else
                    row["EUR_" + titulo_anterior_abril] = DBNull.Value;

                if (valoresListAnioAnterior[i].Abril_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_abril] = valoresListAnioAnterior[i].Abril_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_abril] = DBNull.Value;
                //mayo
                if (valoresListAnioAnterior[i].Mayo_MXN.HasValue)
                    row["MXN_" + titulo_anterior_mayo] = valoresListAnioAnterior[i].Mayo_MXN;
                else
                    row["MXN_" + titulo_anterior_mayo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Mayo.HasValue)
                    row["USD_" + titulo_anterior_mayo] = valoresListAnioAnterior[i].Mayo;
                else
                    row["USD_" + titulo_anterior_mayo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Mayo_EUR.HasValue)
                    row["EUR_" + titulo_anterior_mayo] = valoresListAnioAnterior[i].Mayo_EUR;
                else
                    row["EUR_" + titulo_anterior_mayo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Mayo_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_mayo] = valoresListAnioAnterior[i].Mayo_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_mayo] = DBNull.Value;
                //junio
                if (valoresListAnioAnterior[i].Junio_MXN.HasValue)
                    row["MXN_" + titulo_anterior_junio] = valoresListAnioAnterior[i].Junio_MXN;
                else
                    row["MXN_" + titulo_anterior_junio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Junio.HasValue)
                    row["USD_" + titulo_anterior_junio] = valoresListAnioAnterior[i].Junio;
                else
                    row["USD_" + titulo_anterior_junio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Junio_EUR.HasValue)
                    row["EUR_" + titulo_anterior_junio] = valoresListAnioAnterior[i].Junio_EUR;
                else
                    row["EUR_" + titulo_anterior_junio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Junio_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_junio] = valoresListAnioAnterior[i].Junio_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_junio] = DBNull.Value;
                //julio
                if (valoresListAnioAnterior[i].Julio_MXN.HasValue)
                    row["MXN_" + titulo_anterior_julio] = valoresListAnioAnterior[i].Julio_MXN;
                else
                    row["MXN_" + titulo_anterior_julio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Julio.HasValue)
                    row["USD_" + titulo_anterior_julio] = valoresListAnioAnterior[i].Julio;
                else
                    row["USD_" + titulo_anterior_julio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Julio_EUR.HasValue)
                    row["EUR_" + titulo_anterior_julio] = valoresListAnioAnterior[i].Julio_EUR;
                else
                    row["EUR_" + titulo_anterior_julio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Julio_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_julio] = valoresListAnioAnterior[i].Julio_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_julio] = DBNull.Value;
                //agosto
                if (valoresListAnioAnterior[i].Agosto_MXN.HasValue)
                    row["MXN_" + titulo_anterior_agosto] = valoresListAnioAnterior[i].Agosto_MXN;
                else
                    row["MXN_" + titulo_anterior_agosto] = DBNull.Value;

                if (valoresListAnioAnterior[i].Agosto.HasValue)
                    row["USD_" + titulo_anterior_agosto] = valoresListAnioAnterior[i].Agosto;
                else
                    row["USD_" + titulo_anterior_agosto] = DBNull.Value;

                if (valoresListAnioAnterior[i].Agosto_EUR.HasValue)
                    row["EUR_" + titulo_anterior_agosto] = valoresListAnioAnterior[i].Agosto_EUR;
                else
                    row["EUR_" + titulo_anterior_agosto] = DBNull.Value;

                if (valoresListAnioAnterior[i].Agosto_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_agosto] = valoresListAnioAnterior[i].Agosto_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_agosto] = DBNull.Value;

                //septiembre
                if (valoresListAnioAnterior[i].Septiembre_MXN.HasValue)
                    row["MXN_" + titulo_anterior_septiembre] = valoresListAnioAnterior[i].Septiembre_MXN;
                else
                    row["MXN_" + titulo_anterior_septiembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Septiembre.HasValue)
                    row["USD_" + titulo_anterior_septiembre] = valoresListAnioAnterior[i].Septiembre;
                else
                    row["USD_" + titulo_anterior_septiembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Septiembre_EUR.HasValue)
                    row["EUR_" + titulo_anterior_septiembre] = valoresListAnioAnterior[i].Septiembre_EUR;
                else
                    row["EUR_" + titulo_anterior_septiembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Septiembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_septiembre] = valoresListAnioAnterior[i].Septiembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_septiembre] = DBNull.Value;

                row[titulo_anterior_total_mxn] = valoresListAnioAnterior[i].TotalMesesMXN();
                row[titulo_anterior_total_usd] = valoresListAnioAnterior[i].TotalMesesUSD();
                row[titulo_anterior_total_eur] = valoresListAnioAnterior[i].TotalMesesEUR();
                row[titulo_anterior_total_local] = valoresListAnioAnterior[i].TotalMesesUSD_Local();

                row[comentarios_anterior] = valoresListAnioAnterior[i].Comentario;
                #endregion

                //completa valores para el año actual
                #region valores anio actual

                //octubre
                if (valoresListAnioActual[i].Octubre_MXN.HasValue)
                    row["MXN_" + titulo_actual_octubre] = valoresListAnioActual[i].Octubre_MXN;
                else
                    row["MXN_" + titulo_actual_octubre] = DBNull.Value;

                if (valoresListAnioActual[i].Octubre.HasValue)
                    row["USD_" + titulo_actual_octubre] = valoresListAnioActual[i].Octubre;
                else
                    row["USD_" + titulo_actual_octubre] = DBNull.Value;

                if (valoresListAnioActual[i].Octubre_EUR.HasValue)
                    row["EUR_" + titulo_actual_octubre] = valoresListAnioActual[i].Octubre_EUR;
                else
                    row["EUR_" + titulo_actual_octubre] = DBNull.Value;

                if (valoresListAnioActual[i].Octubre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_octubre] = valoresListAnioActual[i].Octubre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_octubre] = DBNull.Value;
                //noviembre
                if (valoresListAnioActual[i].Noviembre_MXN.HasValue)
                    row["MXN_" + titulo_actual_noviembre] = valoresListAnioActual[i].Noviembre_MXN;
                else
                    row["MXN_" + titulo_actual_noviembre] = DBNull.Value;

                if (valoresListAnioActual[i].Noviembre.HasValue)
                    row["USD_" + titulo_actual_noviembre] = valoresListAnioActual[i].Noviembre;
                else
                    row["USD_" + titulo_actual_noviembre] = DBNull.Value;

                if (valoresListAnioActual[i].Noviembre_EUR.HasValue)
                    row["EUR_" + titulo_actual_noviembre] = valoresListAnioActual[i].Noviembre_EUR;
                else
                    row["EUR_" + titulo_actual_noviembre] = DBNull.Value;

                if (valoresListAnioActual[i].Noviembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_noviembre] = valoresListAnioActual[i].Noviembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_noviembre] = DBNull.Value;
                //diciembre
                if (valoresListAnioActual[i].Diciembre_MXN.HasValue)
                    row["MXN_" + titulo_actual_diciembre] = valoresListAnioActual[i].Diciembre_MXN;
                else
                    row["MXN_" + titulo_actual_diciembre] = DBNull.Value;

                if (valoresListAnioActual[i].Diciembre.HasValue)
                    row["USD_" + titulo_actual_diciembre] = valoresListAnioActual[i].Diciembre;
                else
                    row["USD_" + titulo_actual_diciembre] = DBNull.Value;

                if (valoresListAnioActual[i].Diciembre_EUR.HasValue)
                    row["EUR_" + titulo_actual_diciembre] = valoresListAnioActual[i].Diciembre_EUR;
                else
                    row["EUR_" + titulo_actual_diciembre] = DBNull.Value;

                if (valoresListAnioActual[i].Diciembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_diciembre] = valoresListAnioActual[i].Diciembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_diciembre] = DBNull.Value;
                //enero
                if (valoresListAnioActual[i].Enero_MXN.HasValue)
                    row["MXN_" + titulo_actual_enero] = valoresListAnioActual[i].Enero_MXN;
                else
                    row["MXN_" + titulo_actual_enero] = DBNull.Value;

                if (valoresListAnioActual[i].Enero.HasValue)
                    row["USD_" + titulo_actual_enero] = valoresListAnioActual[i].Enero;
                else
                    row["USD_" + titulo_actual_enero] = DBNull.Value;

                if (valoresListAnioActual[i].Enero_EUR.HasValue)
                    row["EUR_" + titulo_actual_enero] = valoresListAnioActual[i].Enero_EUR;
                else
                    row["EUR_" + titulo_actual_enero] = DBNull.Value;

                if (valoresListAnioActual[i].Enero_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_enero] = valoresListAnioActual[i].Enero_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_enero] = DBNull.Value;
                //febrero
                if (valoresListAnioActual[i].Febrero_MXN.HasValue)
                    row["MXN_" + titulo_actual_febrero] = valoresListAnioActual[i].Febrero_MXN;
                else
                    row["MXN_" + titulo_actual_febrero] = DBNull.Value;

                if (valoresListAnioActual[i].Febrero.HasValue)
                    row["USD_" + titulo_actual_febrero] = valoresListAnioActual[i].Febrero;
                else
                    row["USD_" + titulo_actual_febrero] = DBNull.Value;

                if (valoresListAnioActual[i].Febrero_EUR.HasValue)
                    row["EUR_" + titulo_actual_febrero] = valoresListAnioActual[i].Febrero_EUR;
                else
                    row["EUR_" + titulo_actual_febrero] = DBNull.Value;

                if (valoresListAnioActual[i].Febrero_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_febrero] = valoresListAnioActual[i].Febrero_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_febrero] = DBNull.Value;
                //marzo
                if (valoresListAnioActual[i].Marzo_MXN.HasValue)
                    row["MXN_" + titulo_actual_marzo] = valoresListAnioActual[i].Marzo_MXN;
                else
                    row["MXN_" + titulo_actual_marzo] = DBNull.Value;

                if (valoresListAnioActual[i].Marzo.HasValue)
                    row["USD_" + titulo_actual_marzo] = valoresListAnioActual[i].Marzo;
                else
                    row["USD_" + titulo_actual_marzo] = DBNull.Value;

                if (valoresListAnioActual[i].Marzo_EUR.HasValue)
                    row["EUR_" + titulo_actual_marzo] = valoresListAnioActual[i].Marzo_EUR;
                else
                    row["EUR_" + titulo_actual_marzo] = DBNull.Value;

                if (valoresListAnioActual[i].Marzo_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_marzo] = valoresListAnioActual[i].Marzo_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_marzo] = DBNull.Value;
                //abril
                if (valoresListAnioActual[i].Abril_MXN.HasValue)
                    row["MXN_" + titulo_actual_abril] = valoresListAnioActual[i].Abril_MXN;
                else
                    row["MXN_" + titulo_actual_abril] = DBNull.Value;

                if (valoresListAnioActual[i].Abril.HasValue)
                    row["USD_" + titulo_actual_abril] = valoresListAnioActual[i].Abril;
                else
                    row["USD_" + titulo_actual_abril] = DBNull.Value;

                if (valoresListAnioActual[i].Abril_EUR.HasValue)
                    row["EUR_" + titulo_actual_abril] = valoresListAnioActual[i].Abril_EUR;
                else
                    row["EUR_" + titulo_actual_abril] = DBNull.Value;

                if (valoresListAnioActual[i].Abril_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_abril] = valoresListAnioActual[i].Abril_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_abril] = DBNull.Value;
                //mayo
                if (valoresListAnioActual[i].Mayo_MXN.HasValue)
                    row["MXN_" + titulo_actual_mayo] = valoresListAnioActual[i].Mayo_MXN;
                else
                    row["MXN_" + titulo_actual_mayo] = DBNull.Value;

                if (valoresListAnioActual[i].Mayo.HasValue)
                    row["USD_" + titulo_actual_mayo] = valoresListAnioActual[i].Mayo;
                else
                    row["USD_" + titulo_actual_mayo] = DBNull.Value;

                if (valoresListAnioActual[i].Mayo_EUR.HasValue)
                    row["EUR_" + titulo_actual_mayo] = valoresListAnioActual[i].Mayo_EUR;
                else
                    row["EUR_" + titulo_actual_mayo] = DBNull.Value;

                if (valoresListAnioActual[i].Mayo_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_mayo] = valoresListAnioActual[i].Mayo_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_mayo] = DBNull.Value;
                //junio
                if (valoresListAnioActual[i].Junio_MXN.HasValue)
                    row["MXN_" + titulo_actual_junio] = valoresListAnioActual[i].Junio_MXN;
                else
                    row["MXN_" + titulo_actual_junio] = DBNull.Value;

                if (valoresListAnioActual[i].Junio.HasValue)
                    row["USD_" + titulo_actual_junio] = valoresListAnioActual[i].Junio;
                else
                    row["USD_" + titulo_actual_junio] = DBNull.Value;

                if (valoresListAnioActual[i].Junio_EUR.HasValue)
                    row["EUR_" + titulo_actual_junio] = valoresListAnioActual[i].Junio_EUR;
                else
                    row["EUR_" + titulo_actual_junio] = DBNull.Value;

                if (valoresListAnioActual[i].Junio_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_junio] = valoresListAnioActual[i].Junio_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_junio] = DBNull.Value;
                //julio
                if (valoresListAnioActual[i].Julio_MXN.HasValue)
                    row["MXN_" + titulo_actual_julio] = valoresListAnioActual[i].Julio_MXN;
                else
                    row["MXN_" + titulo_actual_julio] = DBNull.Value;

                if (valoresListAnioActual[i].Julio.HasValue)
                    row["USD_" + titulo_actual_julio] = valoresListAnioActual[i].Julio;
                else
                    row["USD_" + titulo_actual_julio] = DBNull.Value;

                if (valoresListAnioActual[i].Julio_EUR.HasValue)
                    row["EUR_" + titulo_actual_julio] = valoresListAnioActual[i].Julio_EUR;
                else
                    row["EUR_" + titulo_actual_julio] = DBNull.Value;

                if (valoresListAnioActual[i].Julio_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_julio] = valoresListAnioActual[i].Julio_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_julio] = DBNull.Value;
                //agosto
                if (valoresListAnioActual[i].Agosto_MXN.HasValue)
                    row["MXN_" + titulo_actual_agosto] = valoresListAnioActual[i].Agosto_MXN;
                else
                    row["MXN_" + titulo_actual_agosto] = DBNull.Value;

                if (valoresListAnioActual[i].Agosto.HasValue)
                    row["USD_" + titulo_actual_agosto] = valoresListAnioActual[i].Agosto;
                else
                    row["USD_" + titulo_actual_agosto] = DBNull.Value;

                if (valoresListAnioActual[i].Agosto_EUR.HasValue)
                    row["EUR_" + titulo_actual_agosto] = valoresListAnioActual[i].Agosto_EUR;
                else
                    row["EUR_" + titulo_actual_agosto] = DBNull.Value;

                if (valoresListAnioActual[i].Agosto_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_agosto] = valoresListAnioActual[i].Agosto_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_agosto] = DBNull.Value;

                //septiembre
                if (valoresListAnioActual[i].Septiembre_MXN.HasValue)
                    row["MXN_" + titulo_actual_septiembre] = valoresListAnioActual[i].Septiembre_MXN;
                else
                    row["MXN_" + titulo_actual_septiembre] = DBNull.Value;

                if (valoresListAnioActual[i].Septiembre.HasValue)
                    row["USD_" + titulo_actual_septiembre] = valoresListAnioActual[i].Septiembre;
                else
                    row["USD_" + titulo_actual_septiembre] = DBNull.Value;

                if (valoresListAnioActual[i].Septiembre_EUR.HasValue)
                    row["EUR_" + titulo_actual_septiembre] = valoresListAnioActual[i].Septiembre_EUR;
                else
                    row["EUR_" + titulo_actual_septiembre] = DBNull.Value;

                if (valoresListAnioActual[i].Septiembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_septiembre] = valoresListAnioActual[i].Septiembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_septiembre] = DBNull.Value;

                row[titulo_actual_total_mxn] = valoresListAnioActual[i].TotalMesesMXN();
                row[titulo_actual_total_usd] = valoresListAnioActual[i].TotalMesesUSD();
                row[titulo_actual_total_eur] = valoresListAnioActual[i].TotalMesesEUR();
                row[titulo_actual_total_local] = valoresListAnioActual[i].TotalMesesUSD_Local();

                row[comentarios_presente] = valoresListAnioActual[i].Comentario;


                #endregion

                //completa valores para el año 
                #region valores anio poximo
                //octubre
                if (valoresListAnioProximo[i].Octubre_MXN.HasValue)
                    row["MXN_" + titulo_proximo_octubre] = valoresListAnioProximo[i].Octubre_MXN;
                else
                    row["MXN_" + titulo_proximo_octubre] = DBNull.Value;

                if (valoresListAnioProximo[i].Octubre.HasValue)
                    row["USD_" + titulo_proximo_octubre] = valoresListAnioProximo[i].Octubre;
                else
                    row["USD_" + titulo_proximo_octubre] = DBNull.Value;

                if (valoresListAnioProximo[i].Octubre_EUR.HasValue)
                    row["EUR_" + titulo_proximo_octubre] = valoresListAnioProximo[i].Octubre_EUR;
                else
                    row["EUR_" + titulo_proximo_octubre] = DBNull.Value;

                if (valoresListAnioProximo[i].Octubre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_octubre] = valoresListAnioProximo[i].Octubre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_octubre] = DBNull.Value;
                //noviembre
                if (valoresListAnioProximo[i].Noviembre_MXN.HasValue)
                    row["MXN_" + titulo_proximo_noviembre] = valoresListAnioProximo[i].Noviembre_MXN;
                else
                    row["MXN_" + titulo_proximo_noviembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Noviembre.HasValue)
                    row["USD_" + titulo_proximo_noviembre] = valoresListAnioProximo[i].Noviembre;
                else
                    row["USD_" + titulo_proximo_noviembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Noviembre_EUR.HasValue)
                    row["EUR_" + titulo_proximo_noviembre] = valoresListAnioProximo[i].Noviembre_EUR;
                else
                    row["EUR_" + titulo_proximo_noviembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Noviembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_noviembre] = valoresListAnioProximo[i].Noviembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_noviembre] = DBNull.Value;
                //diciembre
                if (valoresListAnioProximo[i].Diciembre_MXN.HasValue)
                    row["MXN_" + titulo_proximo_diciembre] = valoresListAnioProximo[i].Diciembre_MXN;
                else
                    row["MXN_" + titulo_proximo_diciembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Diciembre.HasValue)
                    row["USD_" + titulo_proximo_diciembre] = valoresListAnioProximo[i].Diciembre;
                else
                    row["USD_" + titulo_proximo_diciembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Diciembre_EUR.HasValue)
                    row["EUR_" + titulo_proximo_diciembre] = valoresListAnioProximo[i].Diciembre_EUR;
                else
                    row["EUR_" + titulo_proximo_diciembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Diciembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_diciembre] = valoresListAnioProximo[i].Diciembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_diciembre] = DBNull.Value;
                //enero
                if (valoresListAnioProximo[i].Enero_MXN.HasValue)
                    row["MXN_" + titulo_proximo_enero] = valoresListAnioProximo[i].Enero_MXN;
                else
                    row["MXN_" + titulo_proximo_enero] = DBNull.Value;

                if (valoresListAnioProximo[i].Enero.HasValue)
                    row["USD_" + titulo_proximo_enero] = valoresListAnioProximo[i].Enero;
                else
                    row["USD_" + titulo_proximo_enero] = DBNull.Value;

                if (valoresListAnioProximo[i].Enero_EUR.HasValue)
                    row["EUR_" + titulo_proximo_enero] = valoresListAnioProximo[i].Enero_EUR;
                else
                    row["EUR_" + titulo_proximo_enero] = DBNull.Value;

                if (valoresListAnioProximo[i].Enero_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_enero] = valoresListAnioProximo[i].Enero_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_enero] = DBNull.Value;
                //febrero
                if (valoresListAnioProximo[i].Febrero_MXN.HasValue)
                    row["MXN_" + titulo_proximo_febrero] = valoresListAnioProximo[i].Febrero_MXN;
                else
                    row["MXN_" + titulo_proximo_febrero] = DBNull.Value;

                if (valoresListAnioProximo[i].Febrero.HasValue)
                    row["USD_" + titulo_proximo_febrero] = valoresListAnioProximo[i].Febrero;
                else
                    row["USD_" + titulo_proximo_febrero] = DBNull.Value;

                if (valoresListAnioProximo[i].Febrero_EUR.HasValue)
                    row["EUR_" + titulo_proximo_febrero] = valoresListAnioProximo[i].Febrero_EUR;
                else
                    row["EUR_" + titulo_proximo_febrero] = DBNull.Value;

                if (valoresListAnioProximo[i].Febrero_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_febrero] = valoresListAnioProximo[i].Febrero_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_febrero] = DBNull.Value;
                //marzo
                if (valoresListAnioProximo[i].Marzo_MXN.HasValue)
                    row["MXN_" + titulo_proximo_marzo] = valoresListAnioProximo[i].Marzo_MXN;
                else
                    row["MXN_" + titulo_proximo_marzo] = DBNull.Value;

                if (valoresListAnioProximo[i].Marzo.HasValue)
                    row["USD_" + titulo_proximo_marzo] = valoresListAnioProximo[i].Marzo;
                else
                    row["USD_" + titulo_proximo_marzo] = DBNull.Value;

                if (valoresListAnioProximo[i].Marzo_EUR.HasValue)
                    row["EUR_" + titulo_proximo_marzo] = valoresListAnioProximo[i].Marzo_EUR;
                else
                    row["EUR_" + titulo_proximo_marzo] = DBNull.Value;

                if (valoresListAnioProximo[i].Marzo_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_marzo] = valoresListAnioProximo[i].Marzo_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_marzo] = DBNull.Value;
                //abril
                if (valoresListAnioProximo[i].Abril_MXN.HasValue)
                    row["MXN_" + titulo_proximo_abril] = valoresListAnioProximo[i].Abril_MXN;
                else
                    row["MXN_" + titulo_proximo_abril] = DBNull.Value;

                if (valoresListAnioProximo[i].Abril.HasValue)
                    row["USD_" + titulo_proximo_abril] = valoresListAnioProximo[i].Abril;
                else
                    row["USD_" + titulo_proximo_abril] = DBNull.Value;

                if (valoresListAnioProximo[i].Abril_EUR.HasValue)
                    row["EUR_" + titulo_proximo_abril] = valoresListAnioProximo[i].Abril_EUR;
                else
                    row["EUR_" + titulo_proximo_abril] = DBNull.Value;

                if (valoresListAnioProximo[i].Abril_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_abril] = valoresListAnioProximo[i].Abril_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_abril] = DBNull.Value;
                //mayo
                if (valoresListAnioProximo[i].Mayo_MXN.HasValue)
                    row["MXN_" + titulo_proximo_mayo] = valoresListAnioProximo[i].Mayo_MXN;
                else
                    row["MXN_" + titulo_proximo_mayo] = DBNull.Value;

                if (valoresListAnioProximo[i].Mayo.HasValue)
                    row["USD_" + titulo_proximo_mayo] = valoresListAnioProximo[i].Mayo;
                else
                    row["USD_" + titulo_proximo_mayo] = DBNull.Value;

                if (valoresListAnioProximo[i].Mayo_EUR.HasValue)
                    row["EUR_" + titulo_proximo_mayo] = valoresListAnioProximo[i].Mayo_EUR;
                else
                    row["EUR_" + titulo_proximo_mayo] = DBNull.Value;

                if (valoresListAnioProximo[i].Mayo_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_mayo] = valoresListAnioProximo[i].Mayo_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_mayo] = DBNull.Value;
                //junio
                if (valoresListAnioProximo[i].Junio_MXN.HasValue)
                    row["MXN_" + titulo_proximo_junio] = valoresListAnioProximo[i].Junio_MXN;
                else
                    row["MXN_" + titulo_proximo_junio] = DBNull.Value;

                if (valoresListAnioProximo[i].Junio.HasValue)
                    row["USD_" + titulo_proximo_junio] = valoresListAnioProximo[i].Junio;
                else
                    row["USD_" + titulo_proximo_junio] = DBNull.Value;

                if (valoresListAnioProximo[i].Junio_EUR.HasValue)
                    row["EUR_" + titulo_proximo_junio] = valoresListAnioProximo[i].Junio_EUR;
                else
                    row["EUR_" + titulo_proximo_junio] = DBNull.Value;

                if (valoresListAnioProximo[i].Junio_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_junio] = valoresListAnioProximo[i].Junio_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_junio] = DBNull.Value;
                //julio
                if (valoresListAnioProximo[i].Julio_MXN.HasValue)
                    row["MXN_" + titulo_proximo_julio] = valoresListAnioProximo[i].Julio_MXN;
                else
                    row["MXN_" + titulo_proximo_julio] = DBNull.Value;

                if (valoresListAnioProximo[i].Julio.HasValue)
                    row["USD_" + titulo_proximo_julio] = valoresListAnioProximo[i].Julio;
                else
                    row["USD_" + titulo_proximo_julio] = DBNull.Value;

                if (valoresListAnioProximo[i].Julio_EUR.HasValue)
                    row["EUR_" + titulo_proximo_julio] = valoresListAnioProximo[i].Julio_EUR;
                else
                    row["EUR_" + titulo_proximo_julio] = DBNull.Value;

                if (valoresListAnioProximo[i].Julio_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_julio] = valoresListAnioProximo[i].Julio_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_julio] = DBNull.Value;
                //agosto
                if (valoresListAnioProximo[i].Agosto_MXN.HasValue)
                    row["MXN_" + titulo_proximo_agosto] = valoresListAnioProximo[i].Agosto_MXN;
                else
                    row["MXN_" + titulo_proximo_agosto] = DBNull.Value;

                if (valoresListAnioProximo[i].Agosto.HasValue)
                    row["USD_" + titulo_proximo_agosto] = valoresListAnioProximo[i].Agosto;
                else
                    row["USD_" + titulo_proximo_agosto] = DBNull.Value;

                if (valoresListAnioProximo[i].Agosto_EUR.HasValue)
                    row["EUR_" + titulo_proximo_agosto] = valoresListAnioProximo[i].Agosto_EUR;
                else
                    row["EUR_" + titulo_proximo_agosto] = DBNull.Value;

                if (valoresListAnioProximo[i].Agosto_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_agosto] = valoresListAnioProximo[i].Agosto_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_agosto] = DBNull.Value;

                //septiembre
                if (valoresListAnioProximo[i].Septiembre_MXN.HasValue)
                    row["MXN_" + titulo_proximo_septiembre] = valoresListAnioProximo[i].Septiembre_MXN;
                else
                    row["MXN_" + titulo_proximo_septiembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Septiembre.HasValue)
                    row["USD_" + titulo_proximo_septiembre] = valoresListAnioProximo[i].Septiembre;
                else
                    row["USD_" + titulo_proximo_septiembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Septiembre_EUR.HasValue)
                    row["EUR_" + titulo_proximo_septiembre] = valoresListAnioProximo[i].Septiembre_EUR;
                else
                    row["EUR_" + titulo_proximo_septiembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Septiembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_septiembre] = valoresListAnioProximo[i].Septiembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_septiembre] = DBNull.Value;

                row[titulo_proximo_total_mxn] = valoresListAnioProximo[i].TotalMesesMXN();
                row[titulo_proximo_total_usd] = valoresListAnioProximo[i].TotalMesesUSD();
                row[titulo_proximo_total_eur] = valoresListAnioProximo[i].TotalMesesEUR();
                row[titulo_proximo_total_local] = valoresListAnioProximo[i].TotalMesesUSD_Local();

                row[comentarios_proximo] = valoresListAnioProximo[i].Comentario;

                #endregion

                dt.Rows.Add(row);
            }

            #region Sumatorias
            //agregar los totales
            System.Data.DataRow rowTotales = dt.NewRow();

            rowTotales["Mapping Bridge"] = "Totales";
            rowTotales["MXN_" + titulo_anterior_octubre] = valoresListAnioAnterior.Sum(item => item.Octubre_MXN);
            rowTotales["USD_" + titulo_anterior_octubre] = valoresListAnioAnterior.Sum(item => item.Octubre);
            rowTotales["EUR_" + titulo_anterior_octubre] = valoresListAnioAnterior.Sum(item => item.Octubre_EUR);
            rowTotales["USD Local_" + titulo_anterior_octubre] = valoresListAnioAnterior.Sum(item => item.Octubre_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_noviembre] = valoresListAnioAnterior.Sum(item => item.Noviembre_MXN);
            rowTotales["USD_" + titulo_anterior_noviembre] = valoresListAnioAnterior.Sum(item => item.Noviembre);
            rowTotales["EUR_" + titulo_anterior_noviembre] = valoresListAnioAnterior.Sum(item => item.Noviembre_EUR);
            rowTotales["USD Local_" + titulo_anterior_noviembre] = valoresListAnioAnterior.Sum(item => item.Noviembre_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_diciembre] = valoresListAnioAnterior.Sum(item => item.Diciembre_MXN);
            rowTotales["USD_" + titulo_anterior_diciembre] = valoresListAnioAnterior.Sum(item => item.Diciembre);
            rowTotales["EUR_" + titulo_anterior_diciembre] = valoresListAnioAnterior.Sum(item => item.Diciembre_EUR);
            rowTotales["USD Local_" + titulo_anterior_diciembre] = valoresListAnioAnterior.Sum(item => item.Diciembre_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_enero] = valoresListAnioAnterior.Sum(item => item.Enero_MXN);
            rowTotales["USD_" + titulo_anterior_enero] = valoresListAnioAnterior.Sum(item => item.Enero);
            rowTotales["EUR_" + titulo_anterior_enero] = valoresListAnioAnterior.Sum(item => item.Enero_EUR);
            rowTotales["USD Local_" + titulo_anterior_enero] = valoresListAnioAnterior.Sum(item => item.Enero_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_febrero] = valoresListAnioAnterior.Sum(item => item.Febrero_MXN);
            rowTotales["USD_" + titulo_anterior_febrero] = valoresListAnioAnterior.Sum(item => item.Febrero);
            rowTotales["EUR_" + titulo_anterior_febrero] = valoresListAnioAnterior.Sum(item => item.Febrero_EUR);
            rowTotales["USD Local_" + titulo_anterior_febrero] = valoresListAnioAnterior.Sum(item => item.Febrero_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_marzo] = valoresListAnioAnterior.Sum(item => item.Marzo_MXN);
            rowTotales["USD_" + titulo_anterior_marzo] = valoresListAnioAnterior.Sum(item => item.Marzo);
            rowTotales["EUR_" + titulo_anterior_marzo] = valoresListAnioAnterior.Sum(item => item.Marzo_EUR);
            rowTotales["USD Local_" + titulo_anterior_marzo] = valoresListAnioAnterior.Sum(item => item.Marzo_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_abril] = valoresListAnioAnterior.Sum(item => item.Abril_MXN);
            rowTotales["USD_" + titulo_anterior_abril] = valoresListAnioAnterior.Sum(item => item.Abril);
            rowTotales["EUR_" + titulo_anterior_abril] = valoresListAnioAnterior.Sum(item => item.Abril_EUR);
            rowTotales["USD Local_" + titulo_anterior_abril] = valoresListAnioAnterior.Sum(item => item.Abril_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_mayo] = valoresListAnioAnterior.Sum(item => item.Mayo_MXN);
            rowTotales["USD_" + titulo_anterior_mayo] = valoresListAnioAnterior.Sum(item => item.Mayo);
            rowTotales["EUR_" + titulo_anterior_mayo] = valoresListAnioAnterior.Sum(item => item.Mayo_EUR);
            rowTotales["USD Local_" + titulo_anterior_mayo] = valoresListAnioAnterior.Sum(item => item.Mayo_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_junio] = valoresListAnioAnterior.Sum(item => item.Junio_MXN);
            rowTotales["USD_" + titulo_anterior_junio] = valoresListAnioAnterior.Sum(item => item.Junio);
            rowTotales["EUR_" + titulo_anterior_junio] = valoresListAnioAnterior.Sum(item => item.Junio_EUR);
            rowTotales["USD Local_" + titulo_anterior_junio] = valoresListAnioAnterior.Sum(item => item.Junio_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_julio] = valoresListAnioAnterior.Sum(item => item.Julio_MXN);
            rowTotales["USD_" + titulo_anterior_julio] = valoresListAnioAnterior.Sum(item => item.Julio);
            rowTotales["EUR_" + titulo_anterior_julio] = valoresListAnioAnterior.Sum(item => item.Julio_EUR);
            rowTotales["USD Local_" + titulo_anterior_julio] = valoresListAnioAnterior.Sum(item => item.Julio_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_agosto] = valoresListAnioAnterior.Sum(item => item.Agosto_MXN);
            rowTotales["USD_" + titulo_anterior_agosto] = valoresListAnioAnterior.Sum(item => item.Agosto);
            rowTotales["EUR_" + titulo_anterior_agosto] = valoresListAnioAnterior.Sum(item => item.Agosto_EUR);
            rowTotales["USD Local_" + titulo_anterior_agosto] = valoresListAnioAnterior.Sum(item => item.Agosto_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_septiembre] = valoresListAnioAnterior.Sum(item => item.Septiembre_MXN);
            rowTotales["USD_" + titulo_anterior_septiembre] = valoresListAnioAnterior.Sum(item => item.Septiembre);
            rowTotales["EUR_" + titulo_anterior_septiembre] = valoresListAnioAnterior.Sum(item => item.Septiembre_EUR);
            rowTotales["USD Local_" + titulo_anterior_septiembre] = valoresListAnioAnterior.Sum(item => item.Septiembre_USD_LOCAL);

            rowTotales[titulo_anterior_total_mxn] = valoresListAnioAnterior.Sum(item => item.TotalMesesMXN());
            rowTotales[titulo_anterior_total_usd] = valoresListAnioAnterior.Sum(item => item.TotalMesesUSD());
            rowTotales[titulo_anterior_total_eur] = valoresListAnioAnterior.Sum(item => item.TotalMesesEUR());
            rowTotales[titulo_anterior_total_local] = valoresListAnioAnterior.Sum(item => item.TotalMesesUSD_Local());
            //ACTUAL/FORECAST
            rowTotales["MXN_" + titulo_actual_octubre] = valoresListAnioActual.Sum(item => item.Octubre_MXN);
            rowTotales["USD_" + titulo_actual_octubre] = valoresListAnioActual.Sum(item => item.Octubre);
            rowTotales["EUR_" + titulo_actual_octubre] = valoresListAnioActual.Sum(item => item.Octubre_EUR);
            rowTotales["USD Local_" + titulo_actual_octubre] = valoresListAnioActual.Sum(item => item.Octubre_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_noviembre] = valoresListAnioActual.Sum(item => item.Noviembre_MXN);
            rowTotales["USD_" + titulo_actual_noviembre] = valoresListAnioActual.Sum(item => item.Noviembre);
            rowTotales["EUR_" + titulo_actual_noviembre] = valoresListAnioActual.Sum(item => item.Noviembre_EUR);
            rowTotales["USD Local_" + titulo_actual_noviembre] = valoresListAnioActual.Sum(item => item.Noviembre_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_diciembre] = valoresListAnioActual.Sum(item => item.Diciembre_MXN);
            rowTotales["USD_" + titulo_actual_diciembre] = valoresListAnioActual.Sum(item => item.Diciembre);
            rowTotales["EUR_" + titulo_actual_diciembre] = valoresListAnioActual.Sum(item => item.Diciembre_EUR);
            rowTotales["USD Local_" + titulo_actual_diciembre] = valoresListAnioActual.Sum(item => item.Diciembre_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_enero] = valoresListAnioActual.Sum(item => item.Enero_MXN);
            rowTotales["USD_" + titulo_actual_enero] = valoresListAnioActual.Sum(item => item.Enero);
            rowTotales["EUR_" + titulo_actual_enero] = valoresListAnioActual.Sum(item => item.Enero_EUR);
            rowTotales["USD Local_" + titulo_actual_enero] = valoresListAnioActual.Sum(item => item.Enero_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_febrero] = valoresListAnioActual.Sum(item => item.Febrero_MXN);
            rowTotales["USD_" + titulo_actual_febrero] = valoresListAnioActual.Sum(item => item.Febrero);
            rowTotales["EUR_" + titulo_actual_febrero] = valoresListAnioActual.Sum(item => item.Febrero_EUR);
            rowTotales["USD Local_" + titulo_actual_febrero] = valoresListAnioActual.Sum(item => item.Febrero_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_marzo] = valoresListAnioActual.Sum(item => item.Marzo_MXN);
            rowTotales["USD_" + titulo_actual_marzo] = valoresListAnioActual.Sum(item => item.Marzo);
            rowTotales["EUR_" + titulo_actual_marzo] = valoresListAnioActual.Sum(item => item.Marzo_EUR);
            rowTotales["USD Local_" + titulo_actual_marzo] = valoresListAnioActual.Sum(item => item.Marzo_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_abril] = valoresListAnioActual.Sum(item => item.Abril_MXN);
            rowTotales["USD_" + titulo_actual_abril] = valoresListAnioActual.Sum(item => item.Abril);
            rowTotales["EUR_" + titulo_actual_abril] = valoresListAnioActual.Sum(item => item.Abril_EUR);
            rowTotales["USD Local_" + titulo_actual_abril] = valoresListAnioActual.Sum(item => item.Abril_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_mayo] = valoresListAnioActual.Sum(item => item.Mayo_MXN);
            rowTotales["USD_" + titulo_actual_mayo] = valoresListAnioActual.Sum(item => item.Mayo);
            rowTotales["EUR_" + titulo_actual_mayo] = valoresListAnioActual.Sum(item => item.Mayo_EUR);
            rowTotales["USD Local_" + titulo_actual_mayo] = valoresListAnioActual.Sum(item => item.Mayo_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_junio] = valoresListAnioActual.Sum(item => item.Junio_MXN);
            rowTotales["USD_" + titulo_actual_junio] = valoresListAnioActual.Sum(item => item.Junio);
            rowTotales["EUR_" + titulo_actual_junio] = valoresListAnioActual.Sum(item => item.Junio_EUR);
            rowTotales["USD Local_" + titulo_actual_junio] = valoresListAnioActual.Sum(item => item.Junio_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_julio] = valoresListAnioActual.Sum(item => item.Julio_MXN);
            rowTotales["USD_" + titulo_actual_julio] = valoresListAnioActual.Sum(item => item.Julio);
            rowTotales["EUR_" + titulo_actual_julio] = valoresListAnioActual.Sum(item => item.Julio_EUR);
            rowTotales["USD Local_" + titulo_actual_julio] = valoresListAnioActual.Sum(item => item.Julio_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_agosto] = valoresListAnioActual.Sum(item => item.Agosto_MXN);
            rowTotales["USD_" + titulo_actual_agosto] = valoresListAnioActual.Sum(item => item.Agosto);
            rowTotales["EUR_" + titulo_actual_agosto] = valoresListAnioActual.Sum(item => item.Agosto_EUR);
            rowTotales["USD Local_" + titulo_actual_agosto] = valoresListAnioActual.Sum(item => item.Agosto_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_septiembre] = valoresListAnioActual.Sum(item => item.Septiembre_MXN);
            rowTotales["USD_" + titulo_actual_septiembre] = valoresListAnioActual.Sum(item => item.Septiembre);
            rowTotales["EUR_" + titulo_actual_septiembre] = valoresListAnioActual.Sum(item => item.Septiembre_EUR);
            rowTotales["USD Local_" + titulo_actual_septiembre] = valoresListAnioActual.Sum(item => item.Septiembre_USD_LOCAL);

            rowTotales[titulo_actual_total_mxn] = valoresListAnioActual.Sum(item => item.TotalMesesMXN());
            rowTotales[titulo_actual_total_usd] = valoresListAnioActual.Sum(item => item.TotalMesesUSD());
            rowTotales[titulo_actual_total_eur] = valoresListAnioActual.Sum(item => item.TotalMesesEUR());
            rowTotales[titulo_actual_total_local] = valoresListAnioActual.Sum(item => item.TotalMesesUSD_Local());

            //PROXIMO
            rowTotales["MXN_" + titulo_proximo_octubre] = valoresListAnioProximo.Sum(item => item.Octubre_MXN);
            rowTotales["USD_" + titulo_proximo_octubre] = valoresListAnioProximo.Sum(item => item.Octubre);
            rowTotales["EUR_" + titulo_proximo_octubre] = valoresListAnioProximo.Sum(item => item.Octubre_EUR);
            rowTotales["USD Local_" + titulo_proximo_octubre] = valoresListAnioProximo.Sum(item => item.Octubre_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_noviembre] = valoresListAnioProximo.Sum(item => item.Noviembre_MXN);
            rowTotales["USD_" + titulo_proximo_noviembre] = valoresListAnioProximo.Sum(item => item.Noviembre);
            rowTotales["EUR_" + titulo_proximo_noviembre] = valoresListAnioProximo.Sum(item => item.Noviembre_EUR);
            rowTotales["USD Local_" + titulo_proximo_noviembre] = valoresListAnioProximo.Sum(item => item.Noviembre_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_diciembre] = valoresListAnioProximo.Sum(item => item.Diciembre_MXN);
            rowTotales["USD_" + titulo_proximo_diciembre] = valoresListAnioProximo.Sum(item => item.Diciembre);
            rowTotales["EUR_" + titulo_proximo_diciembre] = valoresListAnioProximo.Sum(item => item.Diciembre_EUR);
            rowTotales["USD Local_" + titulo_proximo_diciembre] = valoresListAnioProximo.Sum(item => item.Diciembre_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_enero] = valoresListAnioProximo.Sum(item => item.Enero_MXN);
            rowTotales["USD_" + titulo_proximo_enero] = valoresListAnioProximo.Sum(item => item.Enero);
            rowTotales["EUR_" + titulo_proximo_enero] = valoresListAnioProximo.Sum(item => item.Enero_EUR);
            rowTotales["USD Local_" + titulo_proximo_enero] = valoresListAnioProximo.Sum(item => item.Enero_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_febrero] = valoresListAnioProximo.Sum(item => item.Febrero_MXN);
            rowTotales["USD_" + titulo_proximo_febrero] = valoresListAnioProximo.Sum(item => item.Febrero);
            rowTotales["EUR_" + titulo_proximo_febrero] = valoresListAnioProximo.Sum(item => item.Febrero_EUR);
            rowTotales["USD Local_" + titulo_proximo_febrero] = valoresListAnioProximo.Sum(item => item.Febrero_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_marzo] = valoresListAnioProximo.Sum(item => item.Marzo_MXN);
            rowTotales["USD_" + titulo_proximo_marzo] = valoresListAnioProximo.Sum(item => item.Marzo);
            rowTotales["EUR_" + titulo_proximo_marzo] = valoresListAnioProximo.Sum(item => item.Marzo_EUR);
            rowTotales["USD Local_" + titulo_proximo_marzo] = valoresListAnioProximo.Sum(item => item.Marzo_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_abril] = valoresListAnioProximo.Sum(item => item.Abril_MXN);
            rowTotales["USD_" + titulo_proximo_abril] = valoresListAnioProximo.Sum(item => item.Abril);
            rowTotales["EUR_" + titulo_proximo_abril] = valoresListAnioProximo.Sum(item => item.Abril_EUR);
            rowTotales["USD Local_" + titulo_proximo_abril] = valoresListAnioProximo.Sum(item => item.Abril_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_mayo] = valoresListAnioProximo.Sum(item => item.Mayo_MXN);
            rowTotales["USD_" + titulo_proximo_mayo] = valoresListAnioProximo.Sum(item => item.Mayo);
            rowTotales["EUR_" + titulo_proximo_mayo] = valoresListAnioProximo.Sum(item => item.Mayo_EUR);
            rowTotales["USD Local_" + titulo_proximo_mayo] = valoresListAnioProximo.Sum(item => item.Mayo_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_junio] = valoresListAnioProximo.Sum(item => item.Junio_MXN);
            rowTotales["USD_" + titulo_proximo_junio] = valoresListAnioProximo.Sum(item => item.Junio);
            rowTotales["EUR_" + titulo_proximo_junio] = valoresListAnioProximo.Sum(item => item.Junio_EUR);
            rowTotales["USD Local_" + titulo_proximo_junio] = valoresListAnioProximo.Sum(item => item.Junio_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_julio] = valoresListAnioProximo.Sum(item => item.Julio_MXN);
            rowTotales["USD_" + titulo_proximo_julio] = valoresListAnioProximo.Sum(item => item.Julio);
            rowTotales["EUR_" + titulo_proximo_julio] = valoresListAnioProximo.Sum(item => item.Julio_EUR);
            rowTotales["USD Local_" + titulo_proximo_julio] = valoresListAnioProximo.Sum(item => item.Julio_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_agosto] = valoresListAnioProximo.Sum(item => item.Agosto_MXN);
            rowTotales["USD_" + titulo_proximo_agosto] = valoresListAnioProximo.Sum(item => item.Agosto);
            rowTotales["EUR_" + titulo_proximo_agosto] = valoresListAnioProximo.Sum(item => item.Agosto_EUR);
            rowTotales["USD Local_" + titulo_proximo_agosto] = valoresListAnioProximo.Sum(item => item.Agosto_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_septiembre] = valoresListAnioProximo.Sum(item => item.Septiembre_MXN);
            rowTotales["USD_" + titulo_proximo_septiembre] = valoresListAnioProximo.Sum(item => item.Septiembre);
            rowTotales["EUR_" + titulo_proximo_septiembre] = valoresListAnioProximo.Sum(item => item.Septiembre_EUR);
            rowTotales["USD Local_" + titulo_proximo_septiembre] = valoresListAnioProximo.Sum(item => item.Septiembre_USD_LOCAL);

            rowTotales[titulo_proximo_total_mxn] = valoresListAnioProximo.Sum(item => item.TotalMesesMXN());
            rowTotales[titulo_proximo_total_usd] = valoresListAnioProximo.Sum(item => item.TotalMesesUSD());
            rowTotales[titulo_proximo_total_eur] = valoresListAnioProximo.Sum(item => item.TotalMesesEUR());
            rowTotales[titulo_proximo_total_local] = valoresListAnioProximo.Sum(item => item.TotalMesesUSD_Local());



            dt.Rows.Add(rowTotales);
            #endregion

            //define y combina las celdas de los encabezados 
            oSLDocument.SetCellValue(3, 5, "FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1))));
            oSLDocument.SetCellValue(4, 5, "ACTUAL");
            oSLDocument.MergeWorksheetCells(4, 5, 4, 5 + (12 * 4) - 1);
            oSLDocument.MergeWorksheetCells(3, 5, 3, 5 + (12 * 4) - 1);

            oSLDocument.SetCellValue(3, 58, "FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual)));
            oSLDocument.SetCellValue(4, 58, "ACTUAL/FORECAST");
            oSLDocument.MergeWorksheetCells(4, 58, 4, 58 + (12 * 4) - 1);
            oSLDocument.MergeWorksheetCells(3, 58, 3, 58 + (12 * 4) - 1);

            oSLDocument.SetCellValue(3, 111, "FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1))));
            oSLDocument.SetCellValue(4, 111, "BUDGET");
            oSLDocument.MergeWorksheetCells(4, 111, 4, 111 + (12 * 4) - 1);
            oSLDocument.MergeWorksheetCells(3, 111, 3, 111 + (12 * 4) - 1);

            int filaInicial = 6;

            //crea la hoja de Plantilla
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Template");
            oSLDocument.ImportDataTable(filaInicial, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);

            //estilo thyssenkrupp
            SLStyle styleThyssen = oSLDocument.CreateStyle();
            styleThyssen.Font.Bold = true;
            styleThyssen.Font.FontColor = System.Drawing.ColorTranslator.FromHtml("#0094ff");
            styleThyssen.SetVerticalAlignment(VerticalAlignmentValues.Center);
            styleThyssen.Font.FontSize = 20;


            SLStyle styleBorder = oSLDocument.CreateStyle();
            styleBorder.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleBorder.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.BottomBorder.Color = System.Drawing.Color.Black;
            styleBorder.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.TopBorder.Color = System.Drawing.Color.Black;
            styleBorder.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.LeftBorder.Color = System.Drawing.Color.Black;
            styleBorder.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.RightBorder.Color = System.Drawing.Color.Black;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            //estilo para el encabezado
            SLStyle styleTotales = oSLDocument.CreateStyle();
            styleTotales.Font.Bold = true;
            styleTotales.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#c6efce"), System.Drawing.ColorTranslator.FromHtml("#c6efce"));
            styleTotales.Font.FontColor = System.Drawing.ColorTranslator.FromHtml("#005000");

            SLStyle styleTotalsColor = oSLDocument.CreateStyle();
            styleTotalsColor.Font.Bold = true;
            styleTotalsColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#002060"), System.Drawing.ColorTranslator.FromHtml("#002060"));
            styleTotalsColor.Font.FontName = "Calibri";
            styleTotalsColor.Font.FontSize = 11;
            styleTotalsColor.Font.FontColor = System.Drawing.Color.White;
            styleTotalsColor.Font.Bold = true;

            SLStyle styleCentrarTexto = oSLDocument.CreateStyle();
            styleCentrarTexto.SetHorizontalAlignment(HorizontalAlignmentValues.Center);

            SLStyle styleBoldTexto = oSLDocument.CreateStyle();
            styleBoldTexto.Font.Bold = true;

            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;
            styleHeaderFont.SetHorizontalAlignment(HorizontalAlignmentValues.Center);

            //estilo para meses 
            SLStyle styleMeses = oSLDocument.CreateStyle();
            styleMeses.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#FFCC99"), System.Drawing.ColorTranslator.FromHtml("#FFCC99"));
            styleMeses.Font.FontName = "Calibri";
            styleMeses.Font.FontSize = 11;
            styleMeses.Font.FontColor = System.Drawing.Color.DarkBlue;
            styleMeses.Font.Bold = true;
            styleMeses.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleMeses.Border.BottomBorder.Color = System.Drawing.Color.DarkBlue;
            styleMeses.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleMeses.Border.TopBorder.Color = System.Drawing.Color.DarkBlue;
            styleMeses.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleMeses.Border.LeftBorder.Color = System.Drawing.Color.DarkBlue;
            styleMeses.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleMeses.Border.RightBorder.Color = System.Drawing.Color.DarkBlue;

            SLStyle styleTotalColumna = oSLDocument.CreateStyle();
            styleTotalColumna.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#F2F2F2"), System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));
            styleTotalColumna.Font.FontName = "Calibri";
            styleTotalColumna.Font.FontSize = 11;
            styleTotalColumna.Font.FontColor = System.Drawing.Color.DarkBlue;
            styleTotalColumna.Font.Bold = true;
            styleTotalColumna.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleTotalColumna.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleTotalColumna.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleTotalColumna.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleTotalColumna.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleTotalColumna.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleTotalColumna.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleTotalColumna.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            //estilo para numeros
            SLStyle styleNumber = oSLDocument.CreateStyle();
            styleNumber.FormatCode = "$  #,##0.00";


            //da estilo a los numero
            //camniar cuando se agregen los comentarios
            oSLDocument.SetColumnStyle(5, 162, styleNumber);

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(filaInicial, 4);

            oSLDocument.Filter("A" + filaInicial, "D1" + filaInicial);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            //aplica estilo a los datos generales
            oSLDocument.SetCellStyle(2, 2, 4, 3, styleBorder);
            oSLDocument.SetCellStyle(2, 2, 4, 2, styleBoldTexto);


            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(filaInicial, styleHeader);
            oSLDocument.SetRowStyle(filaInicial, styleHeaderFont);

            //aplica estilo a los encabazado de totales
            oSLDocument.SetCellStyle(filaInicial, 53, filaInicial, 56, styleTotalsColor);
            oSLDocument.SetCellStyle(filaInicial, 106, filaInicial, 109, styleTotalsColor);
            oSLDocument.SetCellStyle(filaInicial, 159, filaInicial, 162, styleTotalsColor);

            //aplica estilo a las cabeceras de tipo año
            oSLDocument.SetCellStyle(3, 5, styleHeader);
            oSLDocument.SetCellStyle(3, 5, styleHeaderFont);
            oSLDocument.SetCellStyle(3, 5, styleCentrarTexto);
            oSLDocument.SetCellStyle(4, 5, styleTotalsColor);
            oSLDocument.SetCellStyle(4, 5, styleCentrarTexto);

            oSLDocument.SetCellStyle(3, 58, styleHeader);
            oSLDocument.SetCellStyle(3, 58, styleHeaderFont);
            oSLDocument.SetCellStyle(3, 58, styleCentrarTexto);
            oSLDocument.SetCellStyle(4, 58, styleTotalsColor);
            oSLDocument.SetCellStyle(4, 58, styleCentrarTexto);

            oSLDocument.SetCellStyle(3, 111, styleHeader);
            oSLDocument.SetCellStyle(3, 111, styleHeaderFont);
            oSLDocument.SetCellStyle(3, 111, styleCentrarTexto);
            oSLDocument.SetCellStyle(4, 111, styleTotalsColor);
            oSLDocument.SetCellStyle(4, 111, styleCentrarTexto);

            //estilo para titulo thyssen
            oSLDocument.SetRowHeight(1, 40.0);
            oSLDocument.SetCellStyle("B1", styleThyssen);

            //meses para Actual
            for (int i = 0; i < meses_anterior.Count; i++)
            {
                //pasado
                oSLDocument.SetCellValue(5, 5 + (i * 4), meses_anterior[i]);
                oSLDocument.SetCellStyle(5, 5 + (i * 4), styleCentrarTexto);
                oSLDocument.MergeWorksheetCells(5, 5 + (i * 4), 5, 5 + (i * 4) + 3);
                oSLDocument.SetCellStyle(5, 5 + (i * 4), 5, 5 + (i * 4) + 3, styleMeses);
                //presente
                oSLDocument.SetCellValue(5, 58 + (i * 4), meses_presente[i]);
                oSLDocument.SetCellStyle(5, 58 + (i * 4), styleCentrarTexto);
                oSLDocument.MergeWorksheetCells(5, 58 + (i * 4), 5, 58 + (i * 4) + 3);
                oSLDocument.SetCellStyle(5, 58 + (i * 4), 5, 58 + (i * 4) + 3, styleMeses);
                ////proximo
                oSLDocument.SetCellValue(5, 111 + (i * 4), meses_proximo[i]);
                oSLDocument.SetCellStyle(5, 111 + (i * 4), styleCentrarTexto);
                oSLDocument.MergeWorksheetCells(5, 111 + (i * 4), 5, 111 + (i * 4) + 3);
                oSLDocument.SetCellStyle(5, 111 + (i * 4), 5, 111 + (i * 4) + 3, styleMeses);
                //estilo para los totales de las columnas
                oSLDocument.SetCellStyle(filaInicial + 1, 8 + (i * 4), valoresListAnioAnterior.Count + filaInicial, 8 + (i * 4), styleTotalColumna);
                oSLDocument.SetCellStyle(filaInicial + 1, 61 + (i * 4), valoresListAnioAnterior.Count + filaInicial, 61 + (i * 4), styleTotalColumna);
                oSLDocument.SetCellStyle(filaInicial + 1, 114 + (i * 4), valoresListAnioAnterior.Count + filaInicial, 114 + (i * 4), styleTotalColumna);
            }

            //cambia los nombres de las columnas
            for (int i = 0; i < 12; i++)
            {
                //pasado
                oSLDocument.SetCellValue(6, 5 + (i * 4), "MXN");
                oSLDocument.SetCellValue(6, 6 + (i * 4), "USD");
                oSLDocument.SetCellValue(6, 7 + (i * 4), "EUR");
                oSLDocument.SetCellValue(6, 8 + (i * 4), "Local USD");
                //presente
                oSLDocument.SetCellValue(6, 58 + (i * 4), "MXN");
                oSLDocument.SetCellValue(6, 59 + (i * 4), "USD");
                oSLDocument.SetCellValue(6, 60 + (i * 4), "EUR");
                oSLDocument.SetCellValue(6, 61 + (i * 4), "Local USD");
                //proximo
                oSLDocument.SetCellValue(6, 111 + (i * 4), "MXN");
                oSLDocument.SetCellValue(6, 112 + (i * 4), "USD");
                oSLDocument.SetCellValue(6, 113 + (i * 4), "EUR");
                oSLDocument.SetCellValue(6, 114 + (i * 4), "Local USD");

            }
            //cambio en columnas
            oSLDocument.SetColumnWidth(5, 5 + (12 * 4) - 1, 13);
            oSLDocument.SetColumnWidth(58, 58 + (12 * 4) - 1, 13);
            oSLDocument.SetColumnWidth(111, 111 + (12 * 4) - 1, 13);


            //estilo para totales
            oSLDocument.SetCellStyle(valoresListAnioAnterior.Count + filaInicial + 1, 4, valoresListAnioAnterior.Count + filaInicial + 1, 162, styleTotales);

            oSLDocument.SetRowHeight(2, valoresListAnioAnterior.Count + filaInicial + 1, 15.0);
            oSLDocument.SetColumnWidth(3, 40);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        /// <summary>
        /// Crea archivo en excel para el concentrado por planta
        /// </summary>
        /// <param name="valoresListAnioAnterior"></param>
        /// <param name="valoresListAnioActual"></param>
        /// <param name="valoresListAnioProximo"></param>
        /// <param name="anio_Fiscal_anterior"></param>
        /// <param name="anio_Fiscal_actual"></param>
        /// <param name="anio_Fiscal_proximo"></param>
        /// <returns></returns>
        public static byte[] GeneraReporteBudgetPorPlanta(List<view_valores_fiscal_year> valoresListAnioAnterior, List<view_valores_fiscal_year> valoresListAnioActual,
            List<view_valores_fiscal_year> valoresListAnioProximo, budget_anio_fiscal anio_Fiscal_anterior, budget_anio_fiscal anio_Fiscal_actual, budget_anio_fiscal anio_Fiscal_proximo, string tipo_reporte)
        {


            DateTime fechaActual = new DateTime(anio_Fiscal_actual.anio_inicio, 11, 1);

            bool isActualOctubre = anio_Fiscal_actual.isActual(10) == "ACT";
            bool isActualNoviembre = anio_Fiscal_actual.isActual(11) == "ACT";
            bool isActualDiciembre = anio_Fiscal_actual.isActual(12) == "ACT";
            bool isActualEnero = anio_Fiscal_actual.isActual(1) == "ACT";
            bool isActualFebrero = anio_Fiscal_actual.isActual(2) == "ACT";
            bool isActualMarzo = anio_Fiscal_actual.isActual(3) == "ACT";
            bool isActualAbril = anio_Fiscal_actual.isActual(4) == "ACT";
            bool isActualMayo = anio_Fiscal_actual.isActual(5) == "ACT";
            bool isActualJunio = anio_Fiscal_actual.isActual(6) == "ACT";
            bool isActualJulio = anio_Fiscal_actual.isActual(7) == "ACT";
            bool isActualAgosto = anio_Fiscal_actual.isActual(8) == "ACT";
            bool isActualSeptiembre = anio_Fiscal_actual.isActual(9) == "ACT";

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");


            //crea los datos principales del centro de costo

            //oSLDocument.SetCellValue("B1", "thyssenkrupp Materials de México");
            //oSLDocument.MergeWorksheetCells(1, 2, 1, 4);

            //oSLDocument.SetCellValue("B2", "Cost Center");
            //oSLDocument.SetCellValue("B3", "Deparment");
            oSLDocument.SetCellValue("B4", "Tipo Reporte");
            oSLDocument.SetCellValue("C4", tipo_reporte.ToUpper() + " " + anio_Fiscal_actual.ConcatAnio);

            System.Data.DataTable dt = new System.Data.DataTable();

            //columnas          
            dt.Columns.Add("Item", typeof(string));
            dt.Columns.Add("Sap Account", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Mapping Bridge", typeof(string));
            dt.Columns.Add("Cost Center", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Responsable", typeof(string));
            dt.Columns.Add("Plant", typeof(string));

            //Meses para Actual
            string titulo_anterior_octubre = "ACT " + MesesUtil.OCTUBRE.Abreviation + "-" + anio_Fiscal_anterior.anio_inicio.ToString().Substring(2, 2);
            string titulo_anterior_noviembre = "ACT " + MesesUtil.NOVIEMBRE.Abreviation + "-" + anio_Fiscal_anterior.anio_inicio.ToString().Substring(2, 2);
            string titulo_anterior_diciembre = "ACT " + MesesUtil.DICIEMBRE.Abreviation + "-" + anio_Fiscal_anterior.anio_inicio.ToString().Substring(2, 2);
            string titulo_anterior_enero = "ACT " + MesesUtil.ENERO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_febrero = "ACT " + MesesUtil.FEBRERO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_marzo = "ACT " + MesesUtil.MARZO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_abril = "ACT " + MesesUtil.ABRIL.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_mayo = "ACT " + MesesUtil.MAYO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_junio = "ACT " + MesesUtil.JUNIO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_julio = "ACT " + MesesUtil.JULIO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_agosto = "ACT " + MesesUtil.AGOSTO.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_septiembre = "ACT " + MesesUtil.SEPTIEMBRE.Abreviation + "-" + anio_Fiscal_anterior.anio_fin.ToString().Substring(2, 2);
            string titulo_anterior_total_mxn = "Total MXN FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1)));
            string titulo_anterior_total_usd = "Total USD FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1)));
            string titulo_anterior_total_eur = "Total EUR FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1)));
            string titulo_anterior_total_local = "Total USD Local FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1)));
            string comentarios_anterior = "Comentarios " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1)));

            //list con los los meses del año anterior
            List<string> meses_anterior = new List<string> {
                    titulo_anterior_octubre,
                    titulo_anterior_noviembre,
                    titulo_anterior_diciembre,
                    titulo_anterior_enero,
                    titulo_anterior_febrero,
                    titulo_anterior_marzo,
                    titulo_anterior_abril,
                    titulo_anterior_mayo,
                    titulo_anterior_junio,
                    titulo_anterior_julio,
                    titulo_anterior_agosto,
                    titulo_anterior_septiembre
            };

            //agrega las columnas a las tablas
            foreach (var item in meses_anterior)
            {
                dt.Columns.Add("MXN_" + item, typeof(decimal));
                dt.Columns.Add("USD_" + item, typeof(decimal));
                dt.Columns.Add("EUR_" + item, typeof(decimal));
                dt.Columns.Add("USD Local_" + item, typeof(decimal));
            }

            dt.Columns.Add(titulo_anterior_total_mxn, typeof(decimal));
            dt.Columns.Add(titulo_anterior_total_usd, typeof(decimal));
            dt.Columns.Add(titulo_anterior_total_eur, typeof(decimal));
            dt.Columns.Add(titulo_anterior_total_local, typeof(decimal));
            dt.Columns.Add(comentarios_anterior, typeof(string));

            //meses para actual/forecast
            string titulo_actual_octubre = (isActualOctubre ? "ACT" : "FC") + " " + MesesUtil.OCTUBRE.Abreviation + "-" + anio_Fiscal_actual.anio_inicio.ToString().Substring(2, 2);
            string titulo_actual_noviembre = (isActualNoviembre ? "ACT" : "FC") + " " + MesesUtil.NOVIEMBRE.Abreviation + "-" + anio_Fiscal_actual.anio_inicio.ToString().Substring(2, 2);
            string titulo_actual_diciembre = (isActualDiciembre ? "ACT" : "FC") + " " + MesesUtil.DICIEMBRE.Abreviation + "-" + anio_Fiscal_actual.anio_inicio.ToString().Substring(2, 2);
            string titulo_actual_enero = (isActualEnero ? "ACT" : "FC") + " " + MesesUtil.ENERO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_febrero = (isActualFebrero ? "ACT" : "FC") + " " + MesesUtil.FEBRERO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_marzo = (isActualMarzo ? "ACT" : "FC") + " " + MesesUtil.MARZO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_abril = (isActualAbril ? "ACT" : "FC") + " " + MesesUtil.ABRIL.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_mayo = (isActualMayo ? "ACT" : "FC") + " " + MesesUtil.MAYO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_junio = (isActualJunio ? "ACT" : "FC") + " " + MesesUtil.JUNIO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_julio = (isActualJulio ? "ACT" : "FC") + " " + MesesUtil.JULIO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_agosto = (isActualAgosto ? "ACT" : "FC") + " " + MesesUtil.AGOSTO.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_septiembre = (isActualSeptiembre ? "ACT" : "FC") + " " + MesesUtil.SEPTIEMBRE.Abreviation + "-" + anio_Fiscal_actual.anio_fin.ToString().Substring(2, 2);
            string titulo_actual_total_mxn = "Total MXN FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual));
            string titulo_actual_total_usd = "Total USD FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual));
            string titulo_actual_total_eur = "Total EUR FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual));
            string titulo_actual_total_local = "Total USD Local FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual));
            string comentarios_presente = "Comentarios " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual));

            //list con los los meses del año anterior
            List<string> meses_presente = new List<string> {
                    titulo_actual_octubre,
                    titulo_actual_noviembre,
                    titulo_actual_diciembre,
                    titulo_actual_enero,
                    titulo_actual_febrero,
                    titulo_actual_marzo,
                    titulo_actual_abril,
                    titulo_actual_mayo,
                    titulo_actual_junio,
                    titulo_actual_julio,
                    titulo_actual_agosto,
                    titulo_actual_septiembre
            };


            //agrega las columnas a las tablas
            foreach (var item in meses_presente)
            {
                dt.Columns.Add("MXN_" + item, typeof(decimal));
                dt.Columns.Add("USD_" + item, typeof(decimal));
                dt.Columns.Add("EUR_" + item, typeof(decimal));
                dt.Columns.Add("USD Local_" + item, typeof(decimal));
            }

            dt.Columns.Add(titulo_actual_total_mxn, typeof(decimal));
            dt.Columns.Add(titulo_actual_total_usd, typeof(decimal));
            dt.Columns.Add(titulo_actual_total_eur, typeof(decimal));
            dt.Columns.Add(titulo_actual_total_local, typeof(decimal));
            dt.Columns.Add(comentarios_presente, typeof(string));

            //meses para budget
            string titulo_proximo_octubre = "BG " + MesesUtil.OCTUBRE.Abreviation + "-" + anio_Fiscal_proximo.anio_inicio.ToString().Substring(2, 2);
            string titulo_proximo_noviembre = "BG " + MesesUtil.NOVIEMBRE.Abreviation + "-" + anio_Fiscal_proximo.anio_inicio.ToString().Substring(2, 2);
            string titulo_proximo_diciembre = "BG " + MesesUtil.DICIEMBRE.Abreviation + "-" + anio_Fiscal_proximo.anio_inicio.ToString().Substring(2, 2);
            string titulo_proximo_enero = "BG " + MesesUtil.ENERO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_febrero = "BG " + MesesUtil.FEBRERO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_marzo = "BG " + MesesUtil.MARZO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_abril = "BG " + MesesUtil.ABRIL.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_mayo = "BG " + MesesUtil.MAYO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_junio = "BG " + MesesUtil.JUNIO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_julio = "BG " + MesesUtil.JULIO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_agosto = "BG " + MesesUtil.AGOSTO.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_septiembre = "BG " + MesesUtil.SEPTIEMBRE.Abreviation + "-" + anio_Fiscal_proximo.anio_fin.ToString().Substring(2, 2);
            string titulo_proximo_total_mxn = "Total MXN FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1)));
            string titulo_proximo_total_usd = "Total USD FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1)));
            string titulo_proximo_total_eur = "Total EUR FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1)));
            string titulo_proximo_total_local = "Total USD Local FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1)));
            string comentarios_proximo = "Comentarios " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1)));

            //list con los los meses del año anterior
            List<string> meses_proximo = new List<string> {
                    titulo_proximo_octubre,
                    titulo_proximo_noviembre,
                    titulo_proximo_diciembre,
                    titulo_proximo_enero,
                    titulo_proximo_febrero,
                    titulo_proximo_marzo,
                    titulo_proximo_abril,
                    titulo_proximo_mayo,
                    titulo_proximo_junio,
                    titulo_proximo_julio,
                    titulo_proximo_agosto,
                    titulo_proximo_septiembre
            };


            //agrega las columnas a las tablas
            foreach (var item in meses_proximo)
            {
                dt.Columns.Add("MXN_" + item, typeof(decimal));
                dt.Columns.Add("USD_" + item, typeof(decimal));
                dt.Columns.Add("EUR_" + item, typeof(decimal));
                dt.Columns.Add("USD Local_" + item, typeof(decimal));
            }

            dt.Columns.Add(titulo_proximo_total_mxn, typeof(decimal));
            dt.Columns.Add(titulo_proximo_total_usd, typeof(decimal));
            dt.Columns.Add(titulo_proximo_total_eur, typeof(decimal));
            dt.Columns.Add(titulo_proximo_total_local, typeof(decimal));
            dt.Columns.Add(comentarios_proximo, typeof(string));

            for (int i = 0; i < valoresListAnioAnterior.Count; i++)
            {
                System.Data.DataRow row = dt.NewRow();

                //Inserta los datos de la cienta
                //dt.Columns.Add("Cost Center", typeof(string));
                //dt.Columns.Add("Department", typeof(string));
                //dt.Columns.Add("Responsable", typeof(string));
                //dt.Columns.Add("Plant", typeof(string));

                row["Item"] = i + 1;
                row["Sap Account"] = valoresListAnioAnterior[i].sap_account;
                row["Name"] = valoresListAnioAnterior[i].name;
                row["Cost Center"] = valoresListAnioAnterior[i].cost_center;
                row["Department"] = valoresListAnioAnterior[i].department;
                row["Responsable"] = valoresListAnioAnterior[i].responsable;
                row["Plant"] = valoresListAnioAnterior[i].codigo_sap;
                row["Mapping Bridge"] = valoresListAnioAnterior[i].mapping_bridge;

                //completa valores para el año anterior
                #region valores anio pasado
                //agrega las columnas a las tablas

                //octubre
                if (valoresListAnioAnterior[i].Octubre_MXN.HasValue)
                    row["MXN_" + titulo_anterior_octubre] = valoresListAnioAnterior[i].Octubre_MXN;
                else
                    row["MXN_" + titulo_anterior_octubre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Octubre.HasValue)
                    row["USD_" + titulo_anterior_octubre] = valoresListAnioAnterior[i].Octubre;
                else
                    row["USD_" + titulo_anterior_octubre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Octubre_EUR.HasValue)
                    row["EUR_" + titulo_anterior_octubre] = valoresListAnioAnterior[i].Octubre_EUR;
                else
                    row["EUR_" + titulo_anterior_octubre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Octubre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_octubre] = valoresListAnioAnterior[i].Octubre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_octubre] = DBNull.Value;
                //noviembre
                if (valoresListAnioAnterior[i].Noviembre_MXN.HasValue)
                    row["MXN_" + titulo_anterior_noviembre] = valoresListAnioAnterior[i].Noviembre_MXN;
                else
                    row["MXN_" + titulo_anterior_noviembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Noviembre.HasValue)
                    row["USD_" + titulo_anterior_noviembre] = valoresListAnioAnterior[i].Noviembre;
                else
                    row["USD_" + titulo_anterior_noviembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Noviembre_EUR.HasValue)
                    row["EUR_" + titulo_anterior_noviembre] = valoresListAnioAnterior[i].Noviembre_EUR;
                else
                    row["EUR_" + titulo_anterior_noviembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Noviembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_noviembre] = valoresListAnioAnterior[i].Noviembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_noviembre] = DBNull.Value;
                //diciembre
                if (valoresListAnioAnterior[i].Diciembre_MXN.HasValue)
                    row["MXN_" + titulo_anterior_diciembre] = valoresListAnioAnterior[i].Diciembre_MXN;
                else
                    row["MXN_" + titulo_anterior_diciembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Diciembre.HasValue)
                    row["USD_" + titulo_anterior_diciembre] = valoresListAnioAnterior[i].Diciembre;
                else
                    row["USD_" + titulo_anterior_diciembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Diciembre_EUR.HasValue)
                    row["EUR_" + titulo_anterior_diciembre] = valoresListAnioAnterior[i].Diciembre_EUR;
                else
                    row["EUR_" + titulo_anterior_diciembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Diciembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_diciembre] = valoresListAnioAnterior[i].Diciembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_diciembre] = DBNull.Value;
                //enero
                if (valoresListAnioAnterior[i].Enero_MXN.HasValue)
                    row["MXN_" + titulo_anterior_enero] = valoresListAnioAnterior[i].Enero_MXN;
                else
                    row["MXN_" + titulo_anterior_enero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Enero.HasValue)
                    row["USD_" + titulo_anterior_enero] = valoresListAnioAnterior[i].Enero;
                else
                    row["USD_" + titulo_anterior_enero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Enero_EUR.HasValue)
                    row["EUR_" + titulo_anterior_enero] = valoresListAnioAnterior[i].Enero_EUR;
                else
                    row["EUR_" + titulo_anterior_enero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Enero_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_enero] = valoresListAnioAnterior[i].Enero_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_enero] = DBNull.Value;
                //febrero
                if (valoresListAnioAnterior[i].Febrero_MXN.HasValue)
                    row["MXN_" + titulo_anterior_febrero] = valoresListAnioAnterior[i].Febrero_MXN;
                else
                    row["MXN_" + titulo_anterior_febrero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Febrero.HasValue)
                    row["USD_" + titulo_anterior_febrero] = valoresListAnioAnterior[i].Febrero;
                else
                    row["USD_" + titulo_anterior_febrero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Febrero_EUR.HasValue)
                    row["EUR_" + titulo_anterior_febrero] = valoresListAnioAnterior[i].Febrero_EUR;
                else
                    row["EUR_" + titulo_anterior_febrero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Febrero_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_febrero] = valoresListAnioAnterior[i].Febrero_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_febrero] = DBNull.Value;
                //marzo
                if (valoresListAnioAnterior[i].Marzo_MXN.HasValue)
                    row["MXN_" + titulo_anterior_marzo] = valoresListAnioAnterior[i].Marzo_MXN;
                else
                    row["MXN_" + titulo_anterior_marzo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Marzo.HasValue)
                    row["USD_" + titulo_anterior_marzo] = valoresListAnioAnterior[i].Marzo;
                else
                    row["USD_" + titulo_anterior_marzo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Marzo_EUR.HasValue)
                    row["EUR_" + titulo_anterior_marzo] = valoresListAnioAnterior[i].Marzo_EUR;
                else
                    row["EUR_" + titulo_anterior_marzo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Marzo_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_marzo] = valoresListAnioAnterior[i].Marzo_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_marzo] = DBNull.Value;
                //abril
                if (valoresListAnioAnterior[i].Abril_MXN.HasValue)
                    row["MXN_" + titulo_anterior_abril] = valoresListAnioAnterior[i].Abril_MXN;
                else
                    row["MXN_" + titulo_anterior_abril] = DBNull.Value;

                if (valoresListAnioAnterior[i].Abril.HasValue)
                    row["USD_" + titulo_anterior_abril] = valoresListAnioAnterior[i].Abril;
                else
                    row["USD_" + titulo_anterior_abril] = DBNull.Value;

                if (valoresListAnioAnterior[i].Abril_EUR.HasValue)
                    row["EUR_" + titulo_anterior_abril] = valoresListAnioAnterior[i].Abril_EUR;
                else
                    row["EUR_" + titulo_anterior_abril] = DBNull.Value;

                if (valoresListAnioAnterior[i].Abril_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_abril] = valoresListAnioAnterior[i].Abril_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_abril] = DBNull.Value;
                //mayo
                if (valoresListAnioAnterior[i].Mayo_MXN.HasValue)
                    row["MXN_" + titulo_anterior_mayo] = valoresListAnioAnterior[i].Mayo_MXN;
                else
                    row["MXN_" + titulo_anterior_mayo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Mayo.HasValue)
                    row["USD_" + titulo_anterior_mayo] = valoresListAnioAnterior[i].Mayo;
                else
                    row["USD_" + titulo_anterior_mayo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Mayo_EUR.HasValue)
                    row["EUR_" + titulo_anterior_mayo] = valoresListAnioAnterior[i].Mayo_EUR;
                else
                    row["EUR_" + titulo_anterior_mayo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Mayo_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_mayo] = valoresListAnioAnterior[i].Mayo_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_mayo] = DBNull.Value;
                //junio
                if (valoresListAnioAnterior[i].Junio_MXN.HasValue)
                    row["MXN_" + titulo_anterior_junio] = valoresListAnioAnterior[i].Junio_MXN;
                else
                    row["MXN_" + titulo_anterior_junio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Junio.HasValue)
                    row["USD_" + titulo_anterior_junio] = valoresListAnioAnterior[i].Junio;
                else
                    row["USD_" + titulo_anterior_junio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Junio_EUR.HasValue)
                    row["EUR_" + titulo_anterior_junio] = valoresListAnioAnterior[i].Junio_EUR;
                else
                    row["EUR_" + titulo_anterior_junio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Junio_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_junio] = valoresListAnioAnterior[i].Junio_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_junio] = DBNull.Value;
                //julio
                if (valoresListAnioAnterior[i].Julio_MXN.HasValue)
                    row["MXN_" + titulo_anterior_julio] = valoresListAnioAnterior[i].Julio_MXN;
                else
                    row["MXN_" + titulo_anterior_julio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Julio.HasValue)
                    row["USD_" + titulo_anterior_julio] = valoresListAnioAnterior[i].Julio;
                else
                    row["USD_" + titulo_anterior_julio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Julio_EUR.HasValue)
                    row["EUR_" + titulo_anterior_julio] = valoresListAnioAnterior[i].Julio_EUR;
                else
                    row["EUR_" + titulo_anterior_julio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Julio_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_julio] = valoresListAnioAnterior[i].Julio_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_julio] = DBNull.Value;
                //agosto
                if (valoresListAnioAnterior[i].Agosto_MXN.HasValue)
                    row["MXN_" + titulo_anterior_agosto] = valoresListAnioAnterior[i].Agosto_MXN;
                else
                    row["MXN_" + titulo_anterior_agosto] = DBNull.Value;

                if (valoresListAnioAnterior[i].Agosto.HasValue)
                    row["USD_" + titulo_anterior_agosto] = valoresListAnioAnterior[i].Agosto;
                else
                    row["USD_" + titulo_anterior_agosto] = DBNull.Value;

                if (valoresListAnioAnterior[i].Agosto_EUR.HasValue)
                    row["EUR_" + titulo_anterior_agosto] = valoresListAnioAnterior[i].Agosto_EUR;
                else
                    row["EUR_" + titulo_anterior_agosto] = DBNull.Value;

                if (valoresListAnioAnterior[i].Agosto_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_agosto] = valoresListAnioAnterior[i].Agosto_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_agosto] = DBNull.Value;

                //septiembre
                if (valoresListAnioAnterior[i].Septiembre_MXN.HasValue)
                    row["MXN_" + titulo_anterior_septiembre] = valoresListAnioAnterior[i].Septiembre_MXN;
                else
                    row["MXN_" + titulo_anterior_septiembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Septiembre.HasValue)
                    row["USD_" + titulo_anterior_septiembre] = valoresListAnioAnterior[i].Septiembre;
                else
                    row["USD_" + titulo_anterior_septiembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Septiembre_EUR.HasValue)
                    row["EUR_" + titulo_anterior_septiembre] = valoresListAnioAnterior[i].Septiembre_EUR;
                else
                    row["EUR_" + titulo_anterior_septiembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Septiembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_anterior_septiembre] = valoresListAnioAnterior[i].Septiembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_anterior_septiembre] = DBNull.Value;

                row[titulo_anterior_total_mxn] = valoresListAnioAnterior[i].TotalMesesMXN();
                row[titulo_anterior_total_usd] = valoresListAnioAnterior[i].TotalMesesUSD();
                row[titulo_anterior_total_eur] = valoresListAnioAnterior[i].TotalMesesEUR();
                row[titulo_anterior_total_local] = valoresListAnioAnterior[i].TotalMesesUSD_Local();

                row[comentarios_anterior] = valoresListAnioAnterior[i].Comentario;
                #endregion

                //completa valores para el año actual
                #region valores anio actual

                //octubre
                if (valoresListAnioActual[i].Octubre_MXN.HasValue)
                    row["MXN_" + titulo_actual_octubre] = valoresListAnioActual[i].Octubre_MXN;
                else
                    row["MXN_" + titulo_actual_octubre] = DBNull.Value;

                if (valoresListAnioActual[i].Octubre.HasValue)
                    row["USD_" + titulo_actual_octubre] = valoresListAnioActual[i].Octubre;
                else
                    row["USD_" + titulo_actual_octubre] = DBNull.Value;

                if (valoresListAnioActual[i].Octubre_EUR.HasValue)
                    row["EUR_" + titulo_actual_octubre] = valoresListAnioActual[i].Octubre_EUR;
                else
                    row["EUR_" + titulo_actual_octubre] = DBNull.Value;

                if (valoresListAnioActual[i].Octubre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_octubre] = valoresListAnioActual[i].Octubre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_octubre] = DBNull.Value;
                //noviembre
                if (valoresListAnioActual[i].Noviembre_MXN.HasValue)
                    row["MXN_" + titulo_actual_noviembre] = valoresListAnioActual[i].Noviembre_MXN;
                else
                    row["MXN_" + titulo_actual_noviembre] = DBNull.Value;

                if (valoresListAnioActual[i].Noviembre.HasValue)
                    row["USD_" + titulo_actual_noviembre] = valoresListAnioActual[i].Noviembre;
                else
                    row["USD_" + titulo_actual_noviembre] = DBNull.Value;

                if (valoresListAnioActual[i].Noviembre_EUR.HasValue)
                    row["EUR_" + titulo_actual_noviembre] = valoresListAnioActual[i].Noviembre_EUR;
                else
                    row["EUR_" + titulo_actual_noviembre] = DBNull.Value;

                if (valoresListAnioActual[i].Noviembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_noviembre] = valoresListAnioActual[i].Noviembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_noviembre] = DBNull.Value;
                //diciembre
                if (valoresListAnioActual[i].Diciembre_MXN.HasValue)
                    row["MXN_" + titulo_actual_diciembre] = valoresListAnioActual[i].Diciembre_MXN;
                else
                    row["MXN_" + titulo_actual_diciembre] = DBNull.Value;

                if (valoresListAnioActual[i].Diciembre.HasValue)
                    row["USD_" + titulo_actual_diciembre] = valoresListAnioActual[i].Diciembre;
                else
                    row["USD_" + titulo_actual_diciembre] = DBNull.Value;

                if (valoresListAnioActual[i].Diciembre_EUR.HasValue)
                    row["EUR_" + titulo_actual_diciembre] = valoresListAnioActual[i].Diciembre_EUR;
                else
                    row["EUR_" + titulo_actual_diciembre] = DBNull.Value;

                if (valoresListAnioActual[i].Diciembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_diciembre] = valoresListAnioActual[i].Diciembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_diciembre] = DBNull.Value;
                //enero
                if (valoresListAnioActual[i].Enero_MXN.HasValue)
                    row["MXN_" + titulo_actual_enero] = valoresListAnioActual[i].Enero_MXN;
                else
                    row["MXN_" + titulo_actual_enero] = DBNull.Value;

                if (valoresListAnioActual[i].Enero.HasValue)
                    row["USD_" + titulo_actual_enero] = valoresListAnioActual[i].Enero;
                else
                    row["USD_" + titulo_actual_enero] = DBNull.Value;

                if (valoresListAnioActual[i].Enero_EUR.HasValue)
                    row["EUR_" + titulo_actual_enero] = valoresListAnioActual[i].Enero_EUR;
                else
                    row["EUR_" + titulo_actual_enero] = DBNull.Value;

                if (valoresListAnioActual[i].Enero_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_enero] = valoresListAnioActual[i].Enero_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_enero] = DBNull.Value;
                //febrero
                if (valoresListAnioActual[i].Febrero_MXN.HasValue)
                    row["MXN_" + titulo_actual_febrero] = valoresListAnioActual[i].Febrero_MXN;
                else
                    row["MXN_" + titulo_actual_febrero] = DBNull.Value;

                if (valoresListAnioActual[i].Febrero.HasValue)
                    row["USD_" + titulo_actual_febrero] = valoresListAnioActual[i].Febrero;
                else
                    row["USD_" + titulo_actual_febrero] = DBNull.Value;

                if (valoresListAnioActual[i].Febrero_EUR.HasValue)
                    row["EUR_" + titulo_actual_febrero] = valoresListAnioActual[i].Febrero_EUR;
                else
                    row["EUR_" + titulo_actual_febrero] = DBNull.Value;

                if (valoresListAnioActual[i].Febrero_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_febrero] = valoresListAnioActual[i].Febrero_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_febrero] = DBNull.Value;
                //marzo
                if (valoresListAnioActual[i].Marzo_MXN.HasValue)
                    row["MXN_" + titulo_actual_marzo] = valoresListAnioActual[i].Marzo_MXN;
                else
                    row["MXN_" + titulo_actual_marzo] = DBNull.Value;

                if (valoresListAnioActual[i].Marzo.HasValue)
                    row["USD_" + titulo_actual_marzo] = valoresListAnioActual[i].Marzo;
                else
                    row["USD_" + titulo_actual_marzo] = DBNull.Value;

                if (valoresListAnioActual[i].Marzo_EUR.HasValue)
                    row["EUR_" + titulo_actual_marzo] = valoresListAnioActual[i].Marzo_EUR;
                else
                    row["EUR_" + titulo_actual_marzo] = DBNull.Value;

                if (valoresListAnioActual[i].Marzo_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_marzo] = valoresListAnioActual[i].Marzo_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_marzo] = DBNull.Value;
                //abril
                if (valoresListAnioActual[i].Abril_MXN.HasValue)
                    row["MXN_" + titulo_actual_abril] = valoresListAnioActual[i].Abril_MXN;
                else
                    row["MXN_" + titulo_actual_abril] = DBNull.Value;

                if (valoresListAnioActual[i].Abril.HasValue)
                    row["USD_" + titulo_actual_abril] = valoresListAnioActual[i].Abril;
                else
                    row["USD_" + titulo_actual_abril] = DBNull.Value;

                if (valoresListAnioActual[i].Abril_EUR.HasValue)
                    row["EUR_" + titulo_actual_abril] = valoresListAnioActual[i].Abril_EUR;
                else
                    row["EUR_" + titulo_actual_abril] = DBNull.Value;

                if (valoresListAnioActual[i].Abril_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_abril] = valoresListAnioActual[i].Abril_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_abril] = DBNull.Value;
                //mayo
                if (valoresListAnioActual[i].Mayo_MXN.HasValue)
                    row["MXN_" + titulo_actual_mayo] = valoresListAnioActual[i].Mayo_MXN;
                else
                    row["MXN_" + titulo_actual_mayo] = DBNull.Value;

                if (valoresListAnioActual[i].Mayo.HasValue)
                    row["USD_" + titulo_actual_mayo] = valoresListAnioActual[i].Mayo;
                else
                    row["USD_" + titulo_actual_mayo] = DBNull.Value;

                if (valoresListAnioActual[i].Mayo_EUR.HasValue)
                    row["EUR_" + titulo_actual_mayo] = valoresListAnioActual[i].Mayo_EUR;
                else
                    row["EUR_" + titulo_actual_mayo] = DBNull.Value;

                if (valoresListAnioActual[i].Mayo_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_mayo] = valoresListAnioActual[i].Mayo_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_mayo] = DBNull.Value;
                //junio
                if (valoresListAnioActual[i].Junio_MXN.HasValue)
                    row["MXN_" + titulo_actual_junio] = valoresListAnioActual[i].Junio_MXN;
                else
                    row["MXN_" + titulo_actual_junio] = DBNull.Value;

                if (valoresListAnioActual[i].Junio.HasValue)
                    row["USD_" + titulo_actual_junio] = valoresListAnioActual[i].Junio;
                else
                    row["USD_" + titulo_actual_junio] = DBNull.Value;

                if (valoresListAnioActual[i].Junio_EUR.HasValue)
                    row["EUR_" + titulo_actual_junio] = valoresListAnioActual[i].Junio_EUR;
                else
                    row["EUR_" + titulo_actual_junio] = DBNull.Value;

                if (valoresListAnioActual[i].Junio_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_junio] = valoresListAnioActual[i].Junio_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_junio] = DBNull.Value;
                //julio
                if (valoresListAnioActual[i].Julio_MXN.HasValue)
                    row["MXN_" + titulo_actual_julio] = valoresListAnioActual[i].Julio_MXN;
                else
                    row["MXN_" + titulo_actual_julio] = DBNull.Value;

                if (valoresListAnioActual[i].Julio.HasValue)
                    row["USD_" + titulo_actual_julio] = valoresListAnioActual[i].Julio;
                else
                    row["USD_" + titulo_actual_julio] = DBNull.Value;

                if (valoresListAnioActual[i].Julio_EUR.HasValue)
                    row["EUR_" + titulo_actual_julio] = valoresListAnioActual[i].Julio_EUR;
                else
                    row["EUR_" + titulo_actual_julio] = DBNull.Value;

                if (valoresListAnioActual[i].Julio_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_julio] = valoresListAnioActual[i].Julio_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_julio] = DBNull.Value;
                //agosto
                if (valoresListAnioActual[i].Agosto_MXN.HasValue)
                    row["MXN_" + titulo_actual_agosto] = valoresListAnioActual[i].Agosto_MXN;
                else
                    row["MXN_" + titulo_actual_agosto] = DBNull.Value;

                if (valoresListAnioActual[i].Agosto.HasValue)
                    row["USD_" + titulo_actual_agosto] = valoresListAnioActual[i].Agosto;
                else
                    row["USD_" + titulo_actual_agosto] = DBNull.Value;

                if (valoresListAnioActual[i].Agosto_EUR.HasValue)
                    row["EUR_" + titulo_actual_agosto] = valoresListAnioActual[i].Agosto_EUR;
                else
                    row["EUR_" + titulo_actual_agosto] = DBNull.Value;

                if (valoresListAnioActual[i].Agosto_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_agosto] = valoresListAnioActual[i].Agosto_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_agosto] = DBNull.Value;

                //septiembre
                if (valoresListAnioActual[i].Septiembre_MXN.HasValue)
                    row["MXN_" + titulo_actual_septiembre] = valoresListAnioActual[i].Septiembre_MXN;
                else
                    row["MXN_" + titulo_actual_septiembre] = DBNull.Value;

                if (valoresListAnioActual[i].Septiembre.HasValue)
                    row["USD_" + titulo_actual_septiembre] = valoresListAnioActual[i].Septiembre;
                else
                    row["USD_" + titulo_actual_septiembre] = DBNull.Value;

                if (valoresListAnioActual[i].Septiembre_EUR.HasValue)
                    row["EUR_" + titulo_actual_septiembre] = valoresListAnioActual[i].Septiembre_EUR;
                else
                    row["EUR_" + titulo_actual_septiembre] = DBNull.Value;

                if (valoresListAnioActual[i].Septiembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_actual_septiembre] = valoresListAnioActual[i].Septiembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_actual_septiembre] = DBNull.Value;

                row[titulo_actual_total_mxn] = valoresListAnioActual[i].TotalMesesMXN();
                row[titulo_actual_total_usd] = valoresListAnioActual[i].TotalMesesUSD();
                row[titulo_actual_total_eur] = valoresListAnioActual[i].TotalMesesEUR();
                row[titulo_actual_total_local] = valoresListAnioActual[i].TotalMesesUSD_Local();

                row[comentarios_presente] = valoresListAnioActual[i].Comentario;


                #endregion

                //completa valores para el año 
                #region valores anio poximo
                //octubre
                if (valoresListAnioProximo[i].Octubre_MXN.HasValue)
                    row["MXN_" + titulo_proximo_octubre] = valoresListAnioProximo[i].Octubre_MXN;
                else
                    row["MXN_" + titulo_proximo_octubre] = DBNull.Value;

                if (valoresListAnioProximo[i].Octubre.HasValue)
                    row["USD_" + titulo_proximo_octubre] = valoresListAnioProximo[i].Octubre;
                else
                    row["USD_" + titulo_proximo_octubre] = DBNull.Value;

                if (valoresListAnioProximo[i].Octubre_EUR.HasValue)
                    row["EUR_" + titulo_proximo_octubre] = valoresListAnioProximo[i].Octubre_EUR;
                else
                    row["EUR_" + titulo_proximo_octubre] = DBNull.Value;

                if (valoresListAnioProximo[i].Octubre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_octubre] = valoresListAnioProximo[i].Octubre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_octubre] = DBNull.Value;
                //noviembre
                if (valoresListAnioProximo[i].Noviembre_MXN.HasValue)
                    row["MXN_" + titulo_proximo_noviembre] = valoresListAnioProximo[i].Noviembre_MXN;
                else
                    row["MXN_" + titulo_proximo_noviembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Noviembre.HasValue)
                    row["USD_" + titulo_proximo_noviembre] = valoresListAnioProximo[i].Noviembre;
                else
                    row["USD_" + titulo_proximo_noviembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Noviembre_EUR.HasValue)
                    row["EUR_" + titulo_proximo_noviembre] = valoresListAnioProximo[i].Noviembre_EUR;
                else
                    row["EUR_" + titulo_proximo_noviembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Noviembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_noviembre] = valoresListAnioProximo[i].Noviembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_noviembre] = DBNull.Value;
                //diciembre
                if (valoresListAnioProximo[i].Diciembre_MXN.HasValue)
                    row["MXN_" + titulo_proximo_diciembre] = valoresListAnioProximo[i].Diciembre_MXN;
                else
                    row["MXN_" + titulo_proximo_diciembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Diciembre.HasValue)
                    row["USD_" + titulo_proximo_diciembre] = valoresListAnioProximo[i].Diciembre;
                else
                    row["USD_" + titulo_proximo_diciembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Diciembre_EUR.HasValue)
                    row["EUR_" + titulo_proximo_diciembre] = valoresListAnioProximo[i].Diciembre_EUR;
                else
                    row["EUR_" + titulo_proximo_diciembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Diciembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_diciembre] = valoresListAnioProximo[i].Diciembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_diciembre] = DBNull.Value;
                //enero
                if (valoresListAnioProximo[i].Enero_MXN.HasValue)
                    row["MXN_" + titulo_proximo_enero] = valoresListAnioProximo[i].Enero_MXN;
                else
                    row["MXN_" + titulo_proximo_enero] = DBNull.Value;

                if (valoresListAnioProximo[i].Enero.HasValue)
                    row["USD_" + titulo_proximo_enero] = valoresListAnioProximo[i].Enero;
                else
                    row["USD_" + titulo_proximo_enero] = DBNull.Value;

                if (valoresListAnioProximo[i].Enero_EUR.HasValue)
                    row["EUR_" + titulo_proximo_enero] = valoresListAnioProximo[i].Enero_EUR;
                else
                    row["EUR_" + titulo_proximo_enero] = DBNull.Value;

                if (valoresListAnioProximo[i].Enero_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_enero] = valoresListAnioProximo[i].Enero_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_enero] = DBNull.Value;
                //febrero
                if (valoresListAnioProximo[i].Febrero_MXN.HasValue)
                    row["MXN_" + titulo_proximo_febrero] = valoresListAnioProximo[i].Febrero_MXN;
                else
                    row["MXN_" + titulo_proximo_febrero] = DBNull.Value;

                if (valoresListAnioProximo[i].Febrero.HasValue)
                    row["USD_" + titulo_proximo_febrero] = valoresListAnioProximo[i].Febrero;
                else
                    row["USD_" + titulo_proximo_febrero] = DBNull.Value;

                if (valoresListAnioProximo[i].Febrero_EUR.HasValue)
                    row["EUR_" + titulo_proximo_febrero] = valoresListAnioProximo[i].Febrero_EUR;
                else
                    row["EUR_" + titulo_proximo_febrero] = DBNull.Value;

                if (valoresListAnioProximo[i].Febrero_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_febrero] = valoresListAnioProximo[i].Febrero_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_febrero] = DBNull.Value;
                //marzo
                if (valoresListAnioProximo[i].Marzo_MXN.HasValue)
                    row["MXN_" + titulo_proximo_marzo] = valoresListAnioProximo[i].Marzo_MXN;
                else
                    row["MXN_" + titulo_proximo_marzo] = DBNull.Value;

                if (valoresListAnioProximo[i].Marzo.HasValue)
                    row["USD_" + titulo_proximo_marzo] = valoresListAnioProximo[i].Marzo;
                else
                    row["USD_" + titulo_proximo_marzo] = DBNull.Value;

                if (valoresListAnioProximo[i].Marzo_EUR.HasValue)
                    row["EUR_" + titulo_proximo_marzo] = valoresListAnioProximo[i].Marzo_EUR;
                else
                    row["EUR_" + titulo_proximo_marzo] = DBNull.Value;

                if (valoresListAnioProximo[i].Marzo_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_marzo] = valoresListAnioProximo[i].Marzo_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_marzo] = DBNull.Value;
                //abril
                if (valoresListAnioProximo[i].Abril_MXN.HasValue)
                    row["MXN_" + titulo_proximo_abril] = valoresListAnioProximo[i].Abril_MXN;
                else
                    row["MXN_" + titulo_proximo_abril] = DBNull.Value;

                if (valoresListAnioProximo[i].Abril.HasValue)
                    row["USD_" + titulo_proximo_abril] = valoresListAnioProximo[i].Abril;
                else
                    row["USD_" + titulo_proximo_abril] = DBNull.Value;

                if (valoresListAnioProximo[i].Abril_EUR.HasValue)
                    row["EUR_" + titulo_proximo_abril] = valoresListAnioProximo[i].Abril_EUR;
                else
                    row["EUR_" + titulo_proximo_abril] = DBNull.Value;

                if (valoresListAnioProximo[i].Abril_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_abril] = valoresListAnioProximo[i].Abril_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_abril] = DBNull.Value;
                //mayo
                if (valoresListAnioProximo[i].Mayo_MXN.HasValue)
                    row["MXN_" + titulo_proximo_mayo] = valoresListAnioProximo[i].Mayo_MXN;
                else
                    row["MXN_" + titulo_proximo_mayo] = DBNull.Value;

                if (valoresListAnioProximo[i].Mayo.HasValue)
                    row["USD_" + titulo_proximo_mayo] = valoresListAnioProximo[i].Mayo;
                else
                    row["USD_" + titulo_proximo_mayo] = DBNull.Value;

                if (valoresListAnioProximo[i].Mayo_EUR.HasValue)
                    row["EUR_" + titulo_proximo_mayo] = valoresListAnioProximo[i].Mayo_EUR;
                else
                    row["EUR_" + titulo_proximo_mayo] = DBNull.Value;

                if (valoresListAnioProximo[i].Mayo_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_mayo] = valoresListAnioProximo[i].Mayo_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_mayo] = DBNull.Value;
                //junio
                if (valoresListAnioProximo[i].Junio_MXN.HasValue)
                    row["MXN_" + titulo_proximo_junio] = valoresListAnioProximo[i].Junio_MXN;
                else
                    row["MXN_" + titulo_proximo_junio] = DBNull.Value;

                if (valoresListAnioProximo[i].Junio.HasValue)
                    row["USD_" + titulo_proximo_junio] = valoresListAnioProximo[i].Junio;
                else
                    row["USD_" + titulo_proximo_junio] = DBNull.Value;

                if (valoresListAnioProximo[i].Junio_EUR.HasValue)
                    row["EUR_" + titulo_proximo_junio] = valoresListAnioProximo[i].Junio_EUR;
                else
                    row["EUR_" + titulo_proximo_junio] = DBNull.Value;

                if (valoresListAnioProximo[i].Junio_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_junio] = valoresListAnioProximo[i].Junio_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_junio] = DBNull.Value;
                //julio
                if (valoresListAnioProximo[i].Julio_MXN.HasValue)
                    row["MXN_" + titulo_proximo_julio] = valoresListAnioProximo[i].Julio_MXN;
                else
                    row["MXN_" + titulo_proximo_julio] = DBNull.Value;

                if (valoresListAnioProximo[i].Julio.HasValue)
                    row["USD_" + titulo_proximo_julio] = valoresListAnioProximo[i].Julio;
                else
                    row["USD_" + titulo_proximo_julio] = DBNull.Value;

                if (valoresListAnioProximo[i].Julio_EUR.HasValue)
                    row["EUR_" + titulo_proximo_julio] = valoresListAnioProximo[i].Julio_EUR;
                else
                    row["EUR_" + titulo_proximo_julio] = DBNull.Value;

                if (valoresListAnioProximo[i].Julio_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_julio] = valoresListAnioProximo[i].Julio_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_julio] = DBNull.Value;
                //agosto
                if (valoresListAnioProximo[i].Agosto_MXN.HasValue)
                    row["MXN_" + titulo_proximo_agosto] = valoresListAnioProximo[i].Agosto_MXN;
                else
                    row["MXN_" + titulo_proximo_agosto] = DBNull.Value;

                if (valoresListAnioProximo[i].Agosto.HasValue)
                    row["USD_" + titulo_proximo_agosto] = valoresListAnioProximo[i].Agosto;
                else
                    row["USD_" + titulo_proximo_agosto] = DBNull.Value;

                if (valoresListAnioProximo[i].Agosto_EUR.HasValue)
                    row["EUR_" + titulo_proximo_agosto] = valoresListAnioProximo[i].Agosto_EUR;
                else
                    row["EUR_" + titulo_proximo_agosto] = DBNull.Value;

                if (valoresListAnioProximo[i].Agosto_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_agosto] = valoresListAnioProximo[i].Agosto_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_agosto] = DBNull.Value;

                //septiembre
                if (valoresListAnioProximo[i].Septiembre_MXN.HasValue)
                    row["MXN_" + titulo_proximo_septiembre] = valoresListAnioProximo[i].Septiembre_MXN;
                else
                    row["MXN_" + titulo_proximo_septiembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Septiembre.HasValue)
                    row["USD_" + titulo_proximo_septiembre] = valoresListAnioProximo[i].Septiembre;
                else
                    row["USD_" + titulo_proximo_septiembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Septiembre_EUR.HasValue)
                    row["EUR_" + titulo_proximo_septiembre] = valoresListAnioProximo[i].Septiembre_EUR;
                else
                    row["EUR_" + titulo_proximo_septiembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Septiembre_USD_LOCAL.HasValue)
                    row["USD Local_" + titulo_proximo_septiembre] = valoresListAnioProximo[i].Septiembre_USD_LOCAL;
                else
                    row["USD Local_" + titulo_proximo_septiembre] = DBNull.Value;

                row[titulo_proximo_total_mxn] = valoresListAnioProximo[i].TotalMesesMXN();
                row[titulo_proximo_total_usd] = valoresListAnioProximo[i].TotalMesesUSD();
                row[titulo_proximo_total_eur] = valoresListAnioProximo[i].TotalMesesEUR();
                row[titulo_proximo_total_local] = valoresListAnioProximo[i].TotalMesesUSD_Local();

                row[comentarios_proximo] = valoresListAnioProximo[i].Comentario;

                #endregion

                dt.Rows.Add(row);
            }

            #region Sumatorias
            //agregar los totales
            System.Data.DataRow rowTotales = dt.NewRow();

            rowTotales["Mapping Bridge"] = "Totales";
            rowTotales["MXN_" + titulo_anterior_octubre] = valoresListAnioAnterior.Sum(item => item.Octubre_MXN);
            rowTotales["USD_" + titulo_anterior_octubre] = valoresListAnioAnterior.Sum(item => item.Octubre);
            rowTotales["EUR_" + titulo_anterior_octubre] = valoresListAnioAnterior.Sum(item => item.Octubre_EUR);
            rowTotales["USD Local_" + titulo_anterior_octubre] = valoresListAnioAnterior.Sum(item => item.Octubre_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_noviembre] = valoresListAnioAnterior.Sum(item => item.Noviembre_MXN);
            rowTotales["USD_" + titulo_anterior_noviembre] = valoresListAnioAnterior.Sum(item => item.Noviembre);
            rowTotales["EUR_" + titulo_anterior_noviembre] = valoresListAnioAnterior.Sum(item => item.Noviembre_EUR);
            rowTotales["USD Local_" + titulo_anterior_noviembre] = valoresListAnioAnterior.Sum(item => item.Noviembre_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_diciembre] = valoresListAnioAnterior.Sum(item => item.Diciembre_MXN);
            rowTotales["USD_" + titulo_anterior_diciembre] = valoresListAnioAnterior.Sum(item => item.Diciembre);
            rowTotales["EUR_" + titulo_anterior_diciembre] = valoresListAnioAnterior.Sum(item => item.Diciembre_EUR);
            rowTotales["USD Local_" + titulo_anterior_diciembre] = valoresListAnioAnterior.Sum(item => item.Diciembre_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_enero] = valoresListAnioAnterior.Sum(item => item.Enero_MXN);
            rowTotales["USD_" + titulo_anterior_enero] = valoresListAnioAnterior.Sum(item => item.Enero);
            rowTotales["EUR_" + titulo_anterior_enero] = valoresListAnioAnterior.Sum(item => item.Enero_EUR);
            rowTotales["USD Local_" + titulo_anterior_enero] = valoresListAnioAnterior.Sum(item => item.Enero_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_febrero] = valoresListAnioAnterior.Sum(item => item.Febrero_MXN);
            rowTotales["USD_" + titulo_anterior_febrero] = valoresListAnioAnterior.Sum(item => item.Febrero);
            rowTotales["EUR_" + titulo_anterior_febrero] = valoresListAnioAnterior.Sum(item => item.Febrero_EUR);
            rowTotales["USD Local_" + titulo_anterior_febrero] = valoresListAnioAnterior.Sum(item => item.Febrero_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_marzo] = valoresListAnioAnterior.Sum(item => item.Marzo_MXN);
            rowTotales["USD_" + titulo_anterior_marzo] = valoresListAnioAnterior.Sum(item => item.Marzo);
            rowTotales["EUR_" + titulo_anterior_marzo] = valoresListAnioAnterior.Sum(item => item.Marzo_EUR);
            rowTotales["USD Local_" + titulo_anterior_marzo] = valoresListAnioAnterior.Sum(item => item.Marzo_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_abril] = valoresListAnioAnterior.Sum(item => item.Abril_MXN);
            rowTotales["USD_" + titulo_anterior_abril] = valoresListAnioAnterior.Sum(item => item.Abril);
            rowTotales["EUR_" + titulo_anterior_abril] = valoresListAnioAnterior.Sum(item => item.Abril_EUR);
            rowTotales["USD Local_" + titulo_anterior_abril] = valoresListAnioAnterior.Sum(item => item.Abril_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_mayo] = valoresListAnioAnterior.Sum(item => item.Mayo_MXN);
            rowTotales["USD_" + titulo_anterior_mayo] = valoresListAnioAnterior.Sum(item => item.Mayo);
            rowTotales["EUR_" + titulo_anterior_mayo] = valoresListAnioAnterior.Sum(item => item.Mayo_EUR);
            rowTotales["USD Local_" + titulo_anterior_mayo] = valoresListAnioAnterior.Sum(item => item.Mayo_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_junio] = valoresListAnioAnterior.Sum(item => item.Junio_MXN);
            rowTotales["USD_" + titulo_anterior_junio] = valoresListAnioAnterior.Sum(item => item.Junio);
            rowTotales["EUR_" + titulo_anterior_junio] = valoresListAnioAnterior.Sum(item => item.Junio_EUR);
            rowTotales["USD Local_" + titulo_anterior_junio] = valoresListAnioAnterior.Sum(item => item.Junio_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_julio] = valoresListAnioAnterior.Sum(item => item.Julio_MXN);
            rowTotales["USD_" + titulo_anterior_julio] = valoresListAnioAnterior.Sum(item => item.Julio);
            rowTotales["EUR_" + titulo_anterior_julio] = valoresListAnioAnterior.Sum(item => item.Julio_EUR);
            rowTotales["USD Local_" + titulo_anterior_julio] = valoresListAnioAnterior.Sum(item => item.Julio_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_agosto] = valoresListAnioAnterior.Sum(item => item.Agosto_MXN);
            rowTotales["USD_" + titulo_anterior_agosto] = valoresListAnioAnterior.Sum(item => item.Agosto);
            rowTotales["EUR_" + titulo_anterior_agosto] = valoresListAnioAnterior.Sum(item => item.Agosto_EUR);
            rowTotales["USD Local_" + titulo_anterior_agosto] = valoresListAnioAnterior.Sum(item => item.Agosto_USD_LOCAL);
            rowTotales["MXN_" + titulo_anterior_septiembre] = valoresListAnioAnterior.Sum(item => item.Septiembre_MXN);
            rowTotales["USD_" + titulo_anterior_septiembre] = valoresListAnioAnterior.Sum(item => item.Septiembre);
            rowTotales["EUR_" + titulo_anterior_septiembre] = valoresListAnioAnterior.Sum(item => item.Septiembre_EUR);
            rowTotales["USD Local_" + titulo_anterior_septiembre] = valoresListAnioAnterior.Sum(item => item.Septiembre_USD_LOCAL);

            rowTotales[titulo_anterior_total_mxn] = valoresListAnioAnterior.Sum(item => item.TotalMesesMXN());
            rowTotales[titulo_anterior_total_usd] = valoresListAnioAnterior.Sum(item => item.TotalMesesUSD());
            rowTotales[titulo_anterior_total_eur] = valoresListAnioAnterior.Sum(item => item.TotalMesesEUR());
            rowTotales[titulo_anterior_total_local] = valoresListAnioAnterior.Sum(item => item.TotalMesesUSD_Local());
            //ACTUAL/FORECAST
            rowTotales["MXN_" + titulo_actual_octubre] = valoresListAnioActual.Sum(item => item.Octubre_MXN);
            rowTotales["USD_" + titulo_actual_octubre] = valoresListAnioActual.Sum(item => item.Octubre);
            rowTotales["EUR_" + titulo_actual_octubre] = valoresListAnioActual.Sum(item => item.Octubre_EUR);
            rowTotales["USD Local_" + titulo_actual_octubre] = valoresListAnioActual.Sum(item => item.Octubre_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_noviembre] = valoresListAnioActual.Sum(item => item.Noviembre_MXN);
            rowTotales["USD_" + titulo_actual_noviembre] = valoresListAnioActual.Sum(item => item.Noviembre);
            rowTotales["EUR_" + titulo_actual_noviembre] = valoresListAnioActual.Sum(item => item.Noviembre_EUR);
            rowTotales["USD Local_" + titulo_actual_noviembre] = valoresListAnioActual.Sum(item => item.Noviembre_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_diciembre] = valoresListAnioActual.Sum(item => item.Diciembre_MXN);
            rowTotales["USD_" + titulo_actual_diciembre] = valoresListAnioActual.Sum(item => item.Diciembre);
            rowTotales["EUR_" + titulo_actual_diciembre] = valoresListAnioActual.Sum(item => item.Diciembre_EUR);
            rowTotales["USD Local_" + titulo_actual_diciembre] = valoresListAnioActual.Sum(item => item.Diciembre_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_enero] = valoresListAnioActual.Sum(item => item.Enero_MXN);
            rowTotales["USD_" + titulo_actual_enero] = valoresListAnioActual.Sum(item => item.Enero);
            rowTotales["EUR_" + titulo_actual_enero] = valoresListAnioActual.Sum(item => item.Enero_EUR);
            rowTotales["USD Local_" + titulo_actual_enero] = valoresListAnioActual.Sum(item => item.Enero_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_febrero] = valoresListAnioActual.Sum(item => item.Febrero_MXN);
            rowTotales["USD_" + titulo_actual_febrero] = valoresListAnioActual.Sum(item => item.Febrero);
            rowTotales["EUR_" + titulo_actual_febrero] = valoresListAnioActual.Sum(item => item.Febrero_EUR);
            rowTotales["USD Local_" + titulo_actual_febrero] = valoresListAnioActual.Sum(item => item.Febrero_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_marzo] = valoresListAnioActual.Sum(item => item.Marzo_MXN);
            rowTotales["USD_" + titulo_actual_marzo] = valoresListAnioActual.Sum(item => item.Marzo);
            rowTotales["EUR_" + titulo_actual_marzo] = valoresListAnioActual.Sum(item => item.Marzo_EUR);
            rowTotales["USD Local_" + titulo_actual_marzo] = valoresListAnioActual.Sum(item => item.Marzo_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_abril] = valoresListAnioActual.Sum(item => item.Abril_MXN);
            rowTotales["USD_" + titulo_actual_abril] = valoresListAnioActual.Sum(item => item.Abril);
            rowTotales["EUR_" + titulo_actual_abril] = valoresListAnioActual.Sum(item => item.Abril_EUR);
            rowTotales["USD Local_" + titulo_actual_abril] = valoresListAnioActual.Sum(item => item.Abril_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_mayo] = valoresListAnioActual.Sum(item => item.Mayo_MXN);
            rowTotales["USD_" + titulo_actual_mayo] = valoresListAnioActual.Sum(item => item.Mayo);
            rowTotales["EUR_" + titulo_actual_mayo] = valoresListAnioActual.Sum(item => item.Mayo_EUR);
            rowTotales["USD Local_" + titulo_actual_mayo] = valoresListAnioActual.Sum(item => item.Mayo_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_junio] = valoresListAnioActual.Sum(item => item.Junio_MXN);
            rowTotales["USD_" + titulo_actual_junio] = valoresListAnioActual.Sum(item => item.Junio);
            rowTotales["EUR_" + titulo_actual_junio] = valoresListAnioActual.Sum(item => item.Junio_EUR);
            rowTotales["USD Local_" + titulo_actual_junio] = valoresListAnioActual.Sum(item => item.Junio_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_julio] = valoresListAnioActual.Sum(item => item.Julio_MXN);
            rowTotales["USD_" + titulo_actual_julio] = valoresListAnioActual.Sum(item => item.Julio);
            rowTotales["EUR_" + titulo_actual_julio] = valoresListAnioActual.Sum(item => item.Julio_EUR);
            rowTotales["USD Local_" + titulo_actual_julio] = valoresListAnioActual.Sum(item => item.Julio_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_agosto] = valoresListAnioActual.Sum(item => item.Agosto_MXN);
            rowTotales["USD_" + titulo_actual_agosto] = valoresListAnioActual.Sum(item => item.Agosto);
            rowTotales["EUR_" + titulo_actual_agosto] = valoresListAnioActual.Sum(item => item.Agosto_EUR);
            rowTotales["USD Local_" + titulo_actual_agosto] = valoresListAnioActual.Sum(item => item.Agosto_USD_LOCAL);
            rowTotales["MXN_" + titulo_actual_septiembre] = valoresListAnioActual.Sum(item => item.Septiembre_MXN);
            rowTotales["USD_" + titulo_actual_septiembre] = valoresListAnioActual.Sum(item => item.Septiembre);
            rowTotales["EUR_" + titulo_actual_septiembre] = valoresListAnioActual.Sum(item => item.Septiembre_EUR);
            rowTotales["USD Local_" + titulo_actual_septiembre] = valoresListAnioActual.Sum(item => item.Septiembre_USD_LOCAL);

            rowTotales[titulo_actual_total_mxn] = valoresListAnioActual.Sum(item => item.TotalMesesMXN());
            rowTotales[titulo_actual_total_usd] = valoresListAnioActual.Sum(item => item.TotalMesesUSD());
            rowTotales[titulo_actual_total_eur] = valoresListAnioActual.Sum(item => item.TotalMesesEUR());
            rowTotales[titulo_actual_total_local] = valoresListAnioActual.Sum(item => item.TotalMesesUSD_Local());

            //PROXIMO
            rowTotales["MXN_" + titulo_proximo_octubre] = valoresListAnioProximo.Sum(item => item.Octubre_MXN);
            rowTotales["USD_" + titulo_proximo_octubre] = valoresListAnioProximo.Sum(item => item.Octubre);
            rowTotales["EUR_" + titulo_proximo_octubre] = valoresListAnioProximo.Sum(item => item.Octubre_EUR);
            rowTotales["USD Local_" + titulo_proximo_octubre] = valoresListAnioProximo.Sum(item => item.Octubre_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_noviembre] = valoresListAnioProximo.Sum(item => item.Noviembre_MXN);
            rowTotales["USD_" + titulo_proximo_noviembre] = valoresListAnioProximo.Sum(item => item.Noviembre);
            rowTotales["EUR_" + titulo_proximo_noviembre] = valoresListAnioProximo.Sum(item => item.Noviembre_EUR);
            rowTotales["USD Local_" + titulo_proximo_noviembre] = valoresListAnioProximo.Sum(item => item.Noviembre_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_diciembre] = valoresListAnioProximo.Sum(item => item.Diciembre_MXN);
            rowTotales["USD_" + titulo_proximo_diciembre] = valoresListAnioProximo.Sum(item => item.Diciembre);
            rowTotales["EUR_" + titulo_proximo_diciembre] = valoresListAnioProximo.Sum(item => item.Diciembre_EUR);
            rowTotales["USD Local_" + titulo_proximo_diciembre] = valoresListAnioProximo.Sum(item => item.Diciembre_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_enero] = valoresListAnioProximo.Sum(item => item.Enero_MXN);
            rowTotales["USD_" + titulo_proximo_enero] = valoresListAnioProximo.Sum(item => item.Enero);
            rowTotales["EUR_" + titulo_proximo_enero] = valoresListAnioProximo.Sum(item => item.Enero_EUR);
            rowTotales["USD Local_" + titulo_proximo_enero] = valoresListAnioProximo.Sum(item => item.Enero_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_febrero] = valoresListAnioProximo.Sum(item => item.Febrero_MXN);
            rowTotales["USD_" + titulo_proximo_febrero] = valoresListAnioProximo.Sum(item => item.Febrero);
            rowTotales["EUR_" + titulo_proximo_febrero] = valoresListAnioProximo.Sum(item => item.Febrero_EUR);
            rowTotales["USD Local_" + titulo_proximo_febrero] = valoresListAnioProximo.Sum(item => item.Febrero_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_marzo] = valoresListAnioProximo.Sum(item => item.Marzo_MXN);
            rowTotales["USD_" + titulo_proximo_marzo] = valoresListAnioProximo.Sum(item => item.Marzo);
            rowTotales["EUR_" + titulo_proximo_marzo] = valoresListAnioProximo.Sum(item => item.Marzo_EUR);
            rowTotales["USD Local_" + titulo_proximo_marzo] = valoresListAnioProximo.Sum(item => item.Marzo_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_abril] = valoresListAnioProximo.Sum(item => item.Abril_MXN);
            rowTotales["USD_" + titulo_proximo_abril] = valoresListAnioProximo.Sum(item => item.Abril);
            rowTotales["EUR_" + titulo_proximo_abril] = valoresListAnioProximo.Sum(item => item.Abril_EUR);
            rowTotales["USD Local_" + titulo_proximo_abril] = valoresListAnioProximo.Sum(item => item.Abril_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_mayo] = valoresListAnioProximo.Sum(item => item.Mayo_MXN);
            rowTotales["USD_" + titulo_proximo_mayo] = valoresListAnioProximo.Sum(item => item.Mayo);
            rowTotales["EUR_" + titulo_proximo_mayo] = valoresListAnioProximo.Sum(item => item.Mayo_EUR);
            rowTotales["USD Local_" + titulo_proximo_mayo] = valoresListAnioProximo.Sum(item => item.Mayo_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_junio] = valoresListAnioProximo.Sum(item => item.Junio_MXN);
            rowTotales["USD_" + titulo_proximo_junio] = valoresListAnioProximo.Sum(item => item.Junio);
            rowTotales["EUR_" + titulo_proximo_junio] = valoresListAnioProximo.Sum(item => item.Junio_EUR);
            rowTotales["USD Local_" + titulo_proximo_junio] = valoresListAnioProximo.Sum(item => item.Junio_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_julio] = valoresListAnioProximo.Sum(item => item.Julio_MXN);
            rowTotales["USD_" + titulo_proximo_julio] = valoresListAnioProximo.Sum(item => item.Julio);
            rowTotales["EUR_" + titulo_proximo_julio] = valoresListAnioProximo.Sum(item => item.Julio_EUR);
            rowTotales["USD Local_" + titulo_proximo_julio] = valoresListAnioProximo.Sum(item => item.Julio_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_agosto] = valoresListAnioProximo.Sum(item => item.Agosto_MXN);
            rowTotales["USD_" + titulo_proximo_agosto] = valoresListAnioProximo.Sum(item => item.Agosto);
            rowTotales["EUR_" + titulo_proximo_agosto] = valoresListAnioProximo.Sum(item => item.Agosto_EUR);
            rowTotales["USD Local_" + titulo_proximo_agosto] = valoresListAnioProximo.Sum(item => item.Agosto_USD_LOCAL);
            rowTotales["MXN_" + titulo_proximo_septiembre] = valoresListAnioProximo.Sum(item => item.Septiembre_MXN);
            rowTotales["USD_" + titulo_proximo_septiembre] = valoresListAnioProximo.Sum(item => item.Septiembre);
            rowTotales["EUR_" + titulo_proximo_septiembre] = valoresListAnioProximo.Sum(item => item.Septiembre_EUR);
            rowTotales["USD Local_" + titulo_proximo_septiembre] = valoresListAnioProximo.Sum(item => item.Septiembre_USD_LOCAL);

            rowTotales[titulo_proximo_total_mxn] = valoresListAnioProximo.Sum(item => item.TotalMesesMXN());
            rowTotales[titulo_proximo_total_usd] = valoresListAnioProximo.Sum(item => item.TotalMesesUSD());
            rowTotales[titulo_proximo_total_eur] = valoresListAnioProximo.Sum(item => item.TotalMesesEUR());
            rowTotales[titulo_proximo_total_local] = valoresListAnioProximo.Sum(item => item.TotalMesesUSD_Local());



            dt.Rows.Add(rowTotales);
            #endregion

            //define y combina las celdas de los encabezados 
            oSLDocument.SetCellValue(3, 9, "FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1))));
            oSLDocument.SetCellValue(4, 9, "ACTUAL");
            oSLDocument.MergeWorksheetCells(4, 9, 4, 9 + (12 * 4) - 1);
            oSLDocument.MergeWorksheetCells(3, 9, 3, 9 + (12 * 4) - 1);

            oSLDocument.SetCellValue(3, 62, "FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual)));
            oSLDocument.SetCellValue(4, 62, "ACTUAL/FORECAST");
            oSLDocument.MergeWorksheetCells(4, 62, 4, 62 + (12 * 4) - 1);
            oSLDocument.MergeWorksheetCells(3, 62, 3, 62 + (12 * 4) - 1);

            oSLDocument.SetCellValue(3, 115, "FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1))));
            oSLDocument.SetCellValue(4, 115, "BUDGET");
            oSLDocument.MergeWorksheetCells(4, 115, 4, 115 + (12 * 4) - 1);
            oSLDocument.MergeWorksheetCells(3, 115, 3, 115 + (12 * 4) - 1);

            int filaInicial = 6;

            //crea la hoja de Plantilla
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Template");
            oSLDocument.ImportDataTable(filaInicial, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);

            //estilo thyssenkrupp
            SLStyle styleThyssen = oSLDocument.CreateStyle();
            styleThyssen.Font.Bold = true;
            styleThyssen.Font.FontColor = System.Drawing.ColorTranslator.FromHtml("#0094ff");
            styleThyssen.SetVerticalAlignment(VerticalAlignmentValues.Center);
            styleThyssen.Font.FontSize = 20;


            SLStyle styleBorder = oSLDocument.CreateStyle();
            styleBorder.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleBorder.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.BottomBorder.Color = System.Drawing.Color.Black;
            styleBorder.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.TopBorder.Color = System.Drawing.Color.Black;
            styleBorder.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.LeftBorder.Color = System.Drawing.Color.Black;
            styleBorder.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.RightBorder.Color = System.Drawing.Color.Black;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            //estilo para el encabezado
            SLStyle styleTotales = oSLDocument.CreateStyle();
            styleTotales.Font.Bold = true;
            styleTotales.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#c6efce"), System.Drawing.ColorTranslator.FromHtml("#c6efce"));
            styleTotales.Font.FontColor = System.Drawing.ColorTranslator.FromHtml("#005000");

            SLStyle styleTotalsColor = oSLDocument.CreateStyle();
            styleTotalsColor.Font.Bold = true;
            styleTotalsColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#002060"), System.Drawing.ColorTranslator.FromHtml("#002060"));
            styleTotalsColor.Font.FontName = "Calibri";
            styleTotalsColor.Font.FontSize = 11;
            styleTotalsColor.Font.FontColor = System.Drawing.Color.White;
            styleTotalsColor.Font.Bold = true;

            SLStyle styleCentrarTexto = oSLDocument.CreateStyle();
            styleCentrarTexto.SetHorizontalAlignment(HorizontalAlignmentValues.Center);

            SLStyle styleBoldTexto = oSLDocument.CreateStyle();
            styleBoldTexto.Font.Bold = true;

            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;
            styleHeaderFont.SetHorizontalAlignment(HorizontalAlignmentValues.Center);

            //estilo para meses 
            SLStyle styleMeses = oSLDocument.CreateStyle();
            styleMeses.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#FFCC99"), System.Drawing.ColorTranslator.FromHtml("#FFCC99"));
            styleMeses.Font.FontName = "Calibri";
            styleMeses.Font.FontSize = 11;
            styleMeses.Font.FontColor = System.Drawing.Color.DarkBlue;
            styleMeses.Font.Bold = true;
            styleMeses.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleMeses.Border.BottomBorder.Color = System.Drawing.Color.DarkBlue;
            styleMeses.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleMeses.Border.TopBorder.Color = System.Drawing.Color.DarkBlue;
            styleMeses.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleMeses.Border.LeftBorder.Color = System.Drawing.Color.DarkBlue;
            styleMeses.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleMeses.Border.RightBorder.Color = System.Drawing.Color.DarkBlue;

            SLStyle styleTotalColumna = oSLDocument.CreateStyle();
            styleTotalColumna.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#F2F2F2"), System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));
            styleTotalColumna.Font.FontName = "Calibri";
            styleTotalColumna.Font.FontSize = 11;
            styleTotalColumna.Font.FontColor = System.Drawing.Color.DarkBlue;
            styleTotalColumna.Font.Bold = true;
            styleTotalColumna.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleTotalColumna.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleTotalColumna.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleTotalColumna.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleTotalColumna.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleTotalColumna.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleTotalColumna.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleTotalColumna.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            //estilo para numeros
            SLStyle styleNumber = oSLDocument.CreateStyle();
            styleNumber.FormatCode = "$  #,##0.00";


            //da estilo a los numero
            //camniar cuando se agregen los comentarios
            oSLDocument.SetColumnStyle(9, 166, styleNumber);

            //da estilo a la hoja de excel


            oSLDocument.Filter("A" + filaInicial, "D1" + filaInicial);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            //aplica estilo a los datos generales
            oSLDocument.SetCellStyle(4, 2, 4, 3, styleBorder);
            oSLDocument.SetCellStyle(4, 2, 4, 2, styleBoldTexto);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(filaInicial, styleHeader);
            oSLDocument.SetRowStyle(filaInicial, styleHeaderFont);

            //aplica estilo a los encabazado de totales
            oSLDocument.SetCellStyle(filaInicial, 57, filaInicial, 60, styleTotalsColor);
            oSLDocument.SetCellStyle(filaInicial, 110, filaInicial, 113, styleTotalsColor);
            oSLDocument.SetCellStyle(filaInicial, 163, filaInicial, 166, styleTotalsColor);

            //aplica estilo a las cabeceras de tipo año
            oSLDocument.SetCellStyle(3, 9, styleHeader);
            oSLDocument.SetCellStyle(3, 9, styleHeaderFont);
            oSLDocument.SetCellStyle(3, 9, styleCentrarTexto);
            oSLDocument.SetCellStyle(4, 9, styleTotalsColor);
            oSLDocument.SetCellStyle(4, 9, styleCentrarTexto);

            oSLDocument.SetCellStyle(3, 62, styleHeader);
            oSLDocument.SetCellStyle(3, 62, styleHeaderFont);
            oSLDocument.SetCellStyle(3, 62, styleCentrarTexto);
            oSLDocument.SetCellStyle(4, 62, styleTotalsColor);
            oSLDocument.SetCellStyle(4, 62, styleCentrarTexto);

            oSLDocument.SetCellStyle(3, 115, styleHeader);
            oSLDocument.SetCellStyle(3, 115, styleHeaderFont);
            oSLDocument.SetCellStyle(3, 115, styleCentrarTexto);
            oSLDocument.SetCellStyle(4, 115, styleTotalsColor);
            oSLDocument.SetCellStyle(4, 115, styleCentrarTexto);

            //estilo para titulo thyssen
            oSLDocument.SetRowHeight(1, 40.0);
            oSLDocument.SetCellStyle("B1", styleThyssen);

            //meses para Actual
            for (int i = 0; i < meses_anterior.Count; i++)
            {
                //pasado
                oSLDocument.SetCellValue(5, 9 + (i * 4), meses_anterior[i]);
                oSLDocument.SetCellStyle(5, 9 + (i * 4), styleCentrarTexto);
                oSLDocument.MergeWorksheetCells(5, 9 + (i * 4), 5, 9 + (i * 4) + 3);
                oSLDocument.SetCellStyle(5, 9 + (i * 4), 5, 9 + (i * 4) + 3, styleMeses);
                //presente
                oSLDocument.SetCellValue(5, 62 + (i * 4), meses_presente[i]);
                oSLDocument.SetCellStyle(5, 62 + (i * 4), styleCentrarTexto);
                oSLDocument.MergeWorksheetCells(5, 62 + (i * 4), 5, 62 + (i * 4) + 3);
                oSLDocument.SetCellStyle(5, 62 + (i * 4), 5, 62 + (i * 4) + 3, styleMeses);
                ////proximo
                oSLDocument.SetCellValue(5, 115 + (i * 4), meses_proximo[i]);
                oSLDocument.SetCellStyle(5, 115 + (i * 4), styleCentrarTexto);
                oSLDocument.MergeWorksheetCells(5, 115 + (i * 4), 5, 115 + (i * 4) + 3);
                oSLDocument.SetCellStyle(5, 115 + (i * 4), 5, 115 + (i * 4) + 3, styleMeses);
                //estilo para los totales de las columnas
                oSLDocument.SetCellStyle(filaInicial + 1, 12 + (i * 4), valoresListAnioAnterior.Count + filaInicial, 12 + (i * 4), styleTotalColumna);
                oSLDocument.SetCellStyle(filaInicial + 1, 65 + (i * 4), valoresListAnioAnterior.Count + filaInicial, 65 + (i * 4), styleTotalColumna);
                oSLDocument.SetCellStyle(filaInicial + 1, 118 + (i * 4), valoresListAnioAnterior.Count + filaInicial, 118 + (i * 4), styleTotalColumna);
            }

            //cambia los nombres de las columnas
            for (int i = 0; i < 12; i++)
            {
                //pasado
                oSLDocument.SetCellValue(6, 9 + (i * 4), "MXN");
                oSLDocument.SetCellValue(6, 10 + (i * 4), "USD");
                oSLDocument.SetCellValue(6, 11 + (i * 4), "EUR");
                oSLDocument.SetCellValue(6, 12 + (i * 4), "Local USD");
                //presente
                oSLDocument.SetCellValue(6, 62 + (i * 4), "MXN");
                oSLDocument.SetCellValue(6, 63 + (i * 4), "USD");
                oSLDocument.SetCellValue(6, 64 + (i * 4), "EUR");
                oSLDocument.SetCellValue(6, 65 + (i * 4), "Local USD");
                //proximo
                oSLDocument.SetCellValue(6, 115 + (i * 4), "MXN");
                oSLDocument.SetCellValue(6, 116 + (i * 4), "USD");
                oSLDocument.SetCellValue(6, 117 + (i * 4), "EUR");
                oSLDocument.SetCellValue(6, 118 + (i * 4), "Local USD");

            }
            //cambio en columnas
            oSLDocument.SetColumnWidth(9, 9 + (12 * 4) - 1, 13);
            oSLDocument.SetColumnWidth(62, 62 + (12 * 4) - 1, 13);
            oSLDocument.SetColumnWidth(115, 115 + (12 * 4) - 1, 13);


            //estilo para totales
            oSLDocument.SetCellStyle(valoresListAnioAnterior.Count + filaInicial + 1, 8, valoresListAnioAnterior.Count + filaInicial + 1, 166, styleTotales);

            oSLDocument.SetRowHeight(2, valoresListAnioAnterior.Count + filaInicial + 1, 15.0);
            oSLDocument.SetColumnWidth(3, 40);

            //borra las filas sobrantes
            oSLDocument.DeleteRow(1, 2);
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(filaInicial - 2, 4);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);

        }
        public static byte[] BudgetPlantillaCargaMasiva(
            List<view_valores_fiscal_year> valoresListAnioActual,
            budget_anio_fiscal anio_Fiscal_actual)
        {

            // Crea el documento a partir de la plantilla
            string plantillaPath = HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx");
            SLDocument oSLDocument = new SLDocument(plantillaPath, "Sheet1");

            // Crear DataTable con las columnas fijas
            DataTable dt = new DataTable();
            dt.Columns.Add("Item", typeof(string));
            dt.Columns.Add("Sap Account", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Mapping Bridge", typeof(string));
            dt.Columns.Add("Cost Center", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Responsable", typeof(string));
            dt.Columns.Add("Plant", typeof(string));

            // Definir meses en orden: Octubre, Noviembre, Diciembre, Enero ... Septiembre
            int[] monthNumbers = { 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            string[] monthNames =
            {
        "Octubre", "Noviembre", "Diciembre",
        "Enero", "Febrero", "Marzo",
        "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre"
    };

            // Construir títulos para cada mes (usa anio_inicio para meses 10-12 y anio_fin para meses 1-9)
            List<string> mesesPresente = new List<string>();
            for (int i = 0; i < monthNumbers.Length; i++)
            {
                int anio = monthNumbers[i] >= 10 ? anio_Fiscal_actual.anio_inicio : anio_Fiscal_actual.anio_fin;
                string titulo = $"{monthNames[i].Substring(0, 3)}-{anio.ToString().Substring(2, 2)}";
                mesesPresente.Add(titulo);
            }

            // Agrega columnas para cada mes y cada moneda
            string[] monedas = { "MXN", "USD", "EUR" };
            foreach (string titulo in mesesPresente)
            {
                foreach (string moneda in monedas)
                {
                    dt.Columns.Add($"{moneda} {titulo}", typeof(decimal));
                }
            }

            // Para reducir código repetitivo, usamos un arreglo con los nombres de propiedades
            // Se asume que en view_valores_fiscal_year las propiedades se llaman:
            //   "{Mes}" para USD, "{Mes}_MXN" para MXN y "{Mes}_EUR" para EUR.
            // Por ejemplo: "Octubre", "Octubre_MXN", "Octubre_EUR"
            for (int i = 0; i < valoresListAnioActual.Count; i++)
            {
                DataRow row = dt.NewRow();
                var valor = valoresListAnioActual[i];

                row["Item"] = (i + 1).ToString();
                row["Sap Account"] = valor.sap_account;
                row["Name"] = valor.name;
                row["Cost Center"] = valor.cost_center;
                row["Department"] = valor.department;
                row["Responsable"] = valor.responsable;
                row["Plant"] = valor.codigo_sap;
                row["Mapping Bridge"] = valor.mapping_bridge;

                // Para cada mes asigna los valores de cada moneda
                for (int j = 0; j < monthNames.Length; j++)
                {
                    foreach (string moneda in monedas)
                    {
                        // Para USD la propiedad es el nombre del mes, para las otras se agrega el sufijo
                        string propName = moneda == "USD" ? monthNames[j] : $"{monthNames[j]}_{moneda}";
                        var prop = valor.GetType().GetProperty(propName);
                        object propValue = prop?.GetValue(valor, null);
                        row[$"{moneda} {mesesPresente[j]}"] = propValue ?? DBNull.Value;
                    }
                }

                dt.Rows.Add(row);
            }

            // Número de fila inicial para la importación de datos
            int filaInicial = 3;

            // Renombra la hoja y carga la DataTable en el documento
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Template");
            oSLDocument.ImportDataTable(filaInicial, 1, dt, true);

            // Estilos y formatos
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);

            SLStyle styleThyssen = oSLDocument.CreateStyle();
            styleThyssen.Font.Bold = true;
            styleThyssen.Font.FontColor = System.Drawing.ColorTranslator.FromHtml("#0094ff");
            styleThyssen.SetVerticalAlignment(VerticalAlignmentValues.Center);
            styleThyssen.Font.FontSize = 20;

            SLStyle styleNumber = oSLDocument.CreateStyle();
            styleNumber.FormatCode = "$  #,##0.00";

            // Aplica formato numérico a columnas (por ejemplo, de la 9 a la 44)
            oSLDocument.SetColumnStyle(9, 44, styleNumber);
            int ultimaFilaEditable = valoresListAnioActual.Count;
            for (int fila = filaInicial + 1; fila <= ultimaFilaEditable; fila++)
            {
                // Para cada columna de datos (por ejemplo, columnas 9 a 44)
                for (int col = 9; col <= 44; col++)
                {
                    // Asigna un valor vacío si la celda está vacía
                    if (string.IsNullOrEmpty(oSLDocument.GetCellValueAsString(fila, col)))
                    {
                        oSLDocument.SetCellValue(fila, col, "");
                        // Aplica el estilo de moneda a la celda, por si no lo tuviera:
                        oSLDocument.SetCellStyle(fila, col, styleNumber);
                    }
                }
            }

            oSLDocument.Filter("A1", "H1");
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);
            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));
            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;
            styleHeaderFont.SetHorizontalAlignment(HorizontalAlignmentValues.Center);

            oSLDocument.SetRowStyle(filaInicial, styleHeader);
            oSLDocument.SetRowStyle(filaInicial, styleHeaderFont);
            oSLDocument.SetRowHeight(1, 40.0);
            oSLDocument.SetCellStyle("B1", styleThyssen);
            oSLDocument.DeleteRow(1, 2);
            oSLDocument.FreezePanes(filaInicial - 2, 5);


            // Número de columnas fijas antes de las columnas de meses
            int fixedColumns = 8;
            // Se alternan dos colores para distinguir visualmente cada grupo (puedes ajustar los colores)
            System.Drawing.Color color1 = System.Drawing.Color.DarkBlue;
            System.Drawing.Color color2 = System.Drawing.ColorTranslator.FromHtml("#FF0000AA");

            for (int m = 0; m < mesesPresente.Count; m++)
            {
                // Cada mes tiene 3 columnas (MXN, USD, EUR)
                int startCol = fixedColumns + (m * 3) + 1;
                int endCol = startCol + 2;

                // Crea un estilo basado en el estilo de encabezado existente
                SLStyle monthHeaderStyle = styleHeader; // copiar el formato del encabezado base

                // Alterna el color de fondo para cada grupo
                System.Drawing.Color bgColor = (m % 2 == 0) ? color1 : color2;
                monthHeaderStyle.Fill.SetPattern(PatternValues.Solid, bgColor, bgColor);

                // Aplica el estilo al rango de celdas del encabezado correspondiente a este mes
                oSLDocument.SetCellStyle(1, startCol, 1, endCol, monthHeaderStyle);
            }

            // Guardar documento en MemoryStream y devolver el arreglo de bytes
            using (var ms = new System.IO.MemoryStream())
            {
                oSLDocument.SaveAs(ms);
                return ms.ToArray();
            }
        }

        public static byte[] GeneraReporteOrdenesTrabajo(List<orden_trabajo> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");


            System.Data.DataTable dt = new System.Data.DataTable();

            //para llevar el control de si es encabezado o no
            List<bool> filasEncabezados = new List<bool>();
            filasEncabezados.Add(false); //es el encabezado principal

            //columnas          
            dt.Columns.Add("Folio", typeof(string));
            dt.Columns.Add("Solicitante", typeof(string));
            dt.Columns.Add("Fecha", typeof(DateTime));
            dt.Columns.Add("Estatus", typeof(string));
            dt.Columns.Add("Departamento", typeof(string));
            dt.Columns.Add("Nivel de Urgencia", typeof(string));
            dt.Columns.Add("Línea Producción", typeof(string)); //7
            dt.Columns.Add("Zona de Falla", typeof(string));
            dt.Columns.Add("TPM", typeof(string));
            dt.Columns.Add("No. Tarjeta", typeof(string));
            dt.Columns.Add("Grupo de Trabajo", typeof(string));
            dt.Columns.Add("Título", typeof(string));
            dt.Columns.Add("Descripción", typeof(string));
            dt.Columns.Add("Responsable", typeof(string));
            dt.Columns.Add("Fecha Asignación", typeof(DateTime));
            dt.Columns.Add("Fecha En Proceso", typeof(DateTime));
            dt.Columns.Add("Fecha Cierre", typeof(DateTime));
            dt.Columns.Add("R.N. Cantidad", typeof(decimal));
            dt.Columns.Add("R.N. Descripción", typeof(string));
            dt.Columns.Add("R.F. Cantidad", typeof(decimal));
            dt.Columns.Add("R.F. Descripción", typeof(string));
            dt.Columns.Add("Comentarios", typeof(string));

            ////registros , rows
            foreach (orden_trabajo item in listado)
            {

                string tPM = "NO";
                string noTarjeta = string.Empty;
                string grupoTrabajo = string.Empty;
                string linea = string.Empty;
                string responsable = string.Empty;
                object fecha_asignacion, fecha_en_proceso, fecha_cierre;

                if (item.tpm)
                {
                    tPM = "SÍ";
                    noTarjeta = item.numero_tarjeta;
                    grupoTrabajo = item.OT_grupo_trabajo.descripcion;
                }

                if (item.produccion_lineas != null)
                    linea = item.produccion_lineas.linea;

                if (item.empleados1 != null)
                    responsable = item.empleados1.ConcatNombre;

                if (item.fecha_asignacion.HasValue)
                    fecha_asignacion = item.fecha_asignacion.Value;
                else
                    fecha_asignacion = DBNull.Value;

                if (item.fecha_en_proceso.HasValue)
                    fecha_en_proceso = item.fecha_en_proceso.Value;
                else
                    fecha_en_proceso = DBNull.Value;

                if (item.fecha_cierre.HasValue)
                    fecha_cierre = item.fecha_cierre.Value;
                else
                    fecha_cierre = DBNull.Value;

                dt.Rows.Add(item.id, item.empleados2.ConcatNombre, item.fecha_solicitud, OT_Status.DescripcionStatus(item.estatus), item.Area.descripcion, OT_nivel_urgencia.DescripcionStatus(item.nivel_urgencia),
                    linea, item.id_zona_falla.HasValue ? item.OT_zona_falla.zona_falla : String.Empty, tPM, noTarjeta, grupoTrabajo, item.titulo, item.descripcion, responsable, fecha_asignacion, fecha_en_proceso, fecha_cierre, null, null, null, null, item.comentario
                    );

                filasEncabezados.Add(true);
                //obtiene la cantidad de fila actual
                int fila_inicial = filasEncabezados.Count + 1;

                List<OT_refacciones> RNList = item.OT_refacciones.Where(x => x.tipo == OT_tipo_refaccion.NECESARIA).ToList();
                List<OT_refacciones> RFList = item.OT_refacciones.Where(x => x.tipo == OT_tipo_refaccion.FALTANTE).ToList();

                int mayor = RNList.Count;
                if (RFList.Count > mayor)
                    mayor = RFList.Count;

                for (int i = 0; i < mayor; i++)
                {
                    System.Data.DataRow row = dt.NewRow();

                    if (RNList.Count > i)
                    {
                        row["R.N. Cantidad"] = RNList[i].cantidad;
                        row["R.N. Descripción"] = RNList[i].descripcion;
                    }
                    if (RFList.Count > i)
                    {
                        row["R.F. Cantidad"] = RFList[i].cantidad;
                        row["R.F. Descripción"] = RFList[i].descripcion;
                    }

                    dt.Rows.Add(row);
                    filasEncabezados.Add(false);
                }

                //obtiene la fila final
                int fila_final = filasEncabezados.Count + 1;

                //verifica si hubo cambios
                if (fila_inicial != fila_final)
                    oSLDocument.GroupRows(fila_inicial, fila_final - 1);

            }

            //crea la hoja de FACTURAS y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Reporte Órdenes de Trabajo");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            //estilo para el encabezado de cada fila
            SLStyle styleHeaderRow = oSLDocument.CreateStyle();
            styleHeaderRow.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#daeef3"), System.Drawing.ColorTranslator.FromHtml("#daeef3"));
            styleHeaderRow.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleHeaderRow.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleHeaderRow.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleHeaderRow.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleHeaderRow.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleHeaderRow.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleHeaderRow.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleHeaderRow.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            //estilo para numeros
            SLStyle styleNumber = oSLDocument.CreateStyle();
            styleNumber.FormatCode = "#,##0.00";

            //da estilo a los numero
            oSLDocument.SetColumnStyle(18, styleNumber);
            oSLDocument.SetColumnStyle(20, styleNumber);

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(3, styleShortDate);
            oSLDocument.SetColumnStyle(15, styleShortDate);
            oSLDocument.SetColumnStyle(16, styleShortDate);
            oSLDocument.SetColumnStyle(17, styleShortDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            //aplica formato a las filas de encabezado
            for (int i = 0; i < filasEncabezados.Count; i++)
            {
                if (filasEncabezados[i])
                {
                    oSLDocument.SetCellStyle(i + 1, 1, i + 1, dt.Columns.Count, styleHeaderRow);
                }
                else
                {
                    oSLDocument.SetCellStyle(i + 1, 18, i + 1, 21, styleLoteInfo);
                }
                //colapsa todas las filas
                oSLDocument.CollapseRows(i + 2);
            }

            oSLDocument.Filter("A1", "V1");
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);

            oSLDocument.SetRowHeight(1, filasEncabezados.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }
        /// <summary>
        /// Genera reporte de IT_inventory_lines
        /// </summary>
        /// <param name="listado"></param>
        /// <returns></returns>
        public static byte[] GeneraReporteBitacorasExcel(List<IT_inventory_cellular_line> listado)
        {

            SLDocument oSLDocument = new SLDocument();


            System.Data.DataTable dt = new System.Data.DataTable();

            //columnas          
            dt.Columns.Add("Planta", typeof(string));
            dt.Columns.Add("Número Celular", typeof(string));
            dt.Columns.Add("Compañia", typeof(string));
            dt.Columns.Add("Plan Celular", typeof(string));
            dt.Columns.Add("Fecha de Corte", typeof(DateTime));
            dt.Columns.Add("Fecha de Renovación(inicio)", typeof(DateTime));
            dt.Columns.Add("Fecha de Renovación(fin)", typeof(DateTime));
            dt.Columns.Add("Asignación (Responsable)", typeof(string));
            dt.Columns.Add("Departamento", typeof(string));
            dt.Columns.Add("Jefe Inmediato", typeof(string));
            dt.Columns.Add("Estado", typeof(string));

            ////registros , rows
            foreach (IT_inventory_cellular_line item in listado)
            {
                string nombreAsignacion = String.Empty, departamentoAsignacion = String.Empty, jefeInmediato = string.Empty;

                var asignacion = item.IT_asignacion_hardware.Where(x => x.es_asignacion_linea_actual && x.id_cellular_line == item.id && x.id_responsable_principal == x.id_empleado).FirstOrDefault();

                if (asignacion != null && asignacion.empleados != null)
                {
                    nombreAsignacion = asignacion.empleados.ConcatNombre;
                    if (asignacion.empleados.empleados2 != null)
                        jefeInmediato = asignacion.empleados.empleados2.ConcatNombre;

                }
                if (asignacion != null && asignacion.empleados != null && asignacion.empleados.Area != null)
                    departamentoAsignacion = asignacion.empleados.Area.descripcion;


                dt.Rows.Add(item.plantas.descripcion, item.numero_celular, item.IT_inventory_cellular_plans.nombre_compania, item.IT_inventory_cellular_plans.nombre_plan,
                    item.fecha_corte, item.fecha_renovacion_inicio, item.fecha_renovacion, nombreAsignacion, departamentoAsignacion, jefeInmediato, item.activo ? "Activo" : "Inactivo"
                    );
            }

            //crea la hoja de FACTURAS y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Líneas de Celular");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(5, styleShortDate);
            oSLDocument.SetColumnStyle(6, styleShortDate);
            oSLDocument.SetColumnStyle(7, styleShortDate);

            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }
        public static byte[] GeneraReportePFAExcel(List<PFA> listado)
        {

            SLDocument oSLDocument = new SLDocument();


            System.Data.DataTable dt = new System.Data.DataTable();

            //columnas          
            dt.Columns.Add("Number of PFA", typeof(string));
            dt.Columns.Add("Date of Request", typeof(DateTime));
            dt.Columns.Add("Planner/User", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Mill/Supplier of Steel", typeof(string));
            dt.Columns.Add("Customer", typeof(string));
            dt.Columns.Add("SAP Part number", typeof(string));
            dt.Columns.Add("Customer Part Number", typeof(string));
            dt.Columns.Add("Border/Port", typeof(string));
            dt.Columns.Add("tkMM Destination Plant", typeof(string));
            dt.Columns.Add("Reason of the PFA", typeof(string));
            dt.Columns.Add("Type of shipment (container, truck)", typeof(string));
            dt.Columns.Add("Responsible of the premium freight", typeof(string));
            dt.Columns.Add("Total PF Cost usd/mt", typeof(double));
            dt.Columns.Add("Total Original Cost usd/mt", typeof(double));
            dt.Columns.Add("Total Cost To Recover", typeof(double));
            dt.Columns.Add("How is Recovered Cost", typeof(string));
            dt.Columns.Add("Promise Recovered Date", typeof(DateTime));
            dt.Columns.Add("Credit/debit note number", typeof(string));
            dt.Columns.Add("Is Recovered?", typeof(bool));
            dt.Columns.Add("Status", typeof(string));

            ////registros , rows
            foreach (PFA item in listado)
            {
                dt.Rows.Add(item.id, item.date_request, item.empleados1.ConcatNombre, item.PFA_Department.descripcion, item.mill, item.customer,
                item.sap_part_number, item.customer_part_number, item.PFA_Border_port.descripcion, item.PFA_Destination_plant.descripcion,
                item.PFA_Reason.descripcion, item.PFA_Type_shipment.descripcion, item.PFA_Responsible_cost.descripcion,
                item.total_pf_cost, item.total_cost, item.TotalCostToRecover, item.PFA_Recovered_cost.descripcion, item.promise_recovering_date, item.credit_debit_note_number, item.is_recovered, item.estatus);
            }

            //crea la hoja de FACTURAS y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "PFA Report");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(2, styleShortDate);
            oSLDocument.SetColumnStyle(18, styleShortDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter("A1", "U1");
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteITDesktopExcel(List<IT_inventory_items> listado, String inventoryType)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();
            //para llevar el control de si es encabezado o no
            List<bool> filasEncabezados = new List<bool>();
            filasEncabezados.Add(false); //es el encabezado principal

            List<int> filasServidoresVirtuales = new List<int>();

            int physical = 0;
            if (inventoryType == Bitacoras.Util.IT_Tipos_Hardware.VIRTUAL_SERVER)
                physical++;

            bool esServer = (inventoryType == Bitacoras.Util.IT_Tipos_Hardware.VIRTUAL_SERVER || inventoryType == Bitacoras.Util.IT_Tipos_Hardware.SERVER);


            //columnas          
            dt.Columns.Add("Id", typeof(string));   //1
            dt.Columns.Add("Type", typeof(string)); //2
            dt.Columns.Add("Plant", typeof(string));  //3
            dt.Columns.Add("Hostname", typeof(string)); //4
            if (esServer)
                dt.Columns.Add("Physical Server", typeof(string));    //4.5
            dt.Columns.Add("Brand", typeof(string));    //5
            dt.Columns.Add("Model", typeof(string));    //6
            dt.Columns.Add("Serial Number", typeof(string));    //7
            dt.Columns.Add("Operation System", typeof(string)); //8
            dt.Columns.Add("OS bits", typeof(int)); //9
            dt.Columns.Add("CPU speed (MHz)", typeof(int)); //10
            dt.Columns.Add("Numbers of CPUs", typeof(int)); //11
            dt.Columns.Add("Processor", typeof(string));    //12
            dt.Columns.Add("MAC LAN", typeof(string));      //13
            dt.Columns.Add("MAC WLAN", typeof(string));     //14
            dt.Columns.Add("Total Physical Memory (GB)", typeof(decimal)); //15
            dt.Columns.Add("Drive Letter", typeof(string));     //16
            dt.Columns.Add("Total Drive Space (GB)", typeof(decimal));  //17
            dt.Columns.Add("Drive Type", typeof(string));       //18
            dt.Columns.Add("Number of hard drives", typeof(int));   //19
            dt.Columns.Add("Total Disk Space (GB)", typeof(decimal));   //20
            dt.Columns.Add("Maintenance Period (months)", typeof(int)); //21
            dt.Columns.Add("Purchase Date", typeof(DateTime));  //22
            dt.Columns.Add("End Warranty", typeof(DateTime));   //23
            dt.Columns.Add("Is active?", typeof(bool));         //24         
            dt.Columns.Add("Inactive Date", typeof(DateTime)); //25
            dt.Columns.Add("¿Baja?", typeof(bool));         //26         
            dt.Columns.Add("Fecha baja", typeof(DateTime)); //27

            dt.Columns.Add("Last Check-in", typeof(DateTime)); //28
            dt.Columns.Add("OS version", typeof(string));
            dt.Columns.Add("Primary User UPN", typeof(string));
            dt.Columns.Add("Primary User email", typeof(string));
            dt.Columns.Add("Primary User display name", typeof(string));
            dt.Columns.Add("Compliance", typeof(bool));
            dt.Columns.Add("Manage By", typeof(string));
            dt.Columns.Add("Encrypted", typeof(bool));
            dt.Columns.Add("Join Type", typeof(string));
            dt.Columns.Add("Management Certificate Expiration Date", typeof(DateTime)); //37

            dt.Columns.Add("Comments", typeof(string));         //28
            dt.Columns.Add("Asignación Actual", typeof(string));         //27
            dt.Columns.Add("Planta", typeof(string));         //27
            dt.Columns.Add("Departamento", typeof(string));         //27
            dt.Columns.Add("Puesto", typeof(string));         //27
            dt.Columns.Add("Jefe Inmediato", typeof(string));         //27
                                                                      //modern workplace


            ////registros , rows
            foreach (IT_inventory_items item in listado)
            {
                //  var Asignaciones = db.IT_asignacion_hardware.Where(x => x.IT_asignacion_hardware_rel_items.Any(y => y.id_it_inventory_item == id)).ToList();
                string asignacionActual = string.Empty;
                string planta = string.Empty;
                string departamento = string.Empty;
                string puesto = string.Empty;
                string jefeInmediato = string.Empty;


                var asignacion = item.IT_asignacion_hardware_rel_items.Where(x => x.IT_asignacion_hardware.es_asignacion_actual
                                    && x.IT_asignacion_hardware.id_empleado == x.IT_asignacion_hardware.id_responsable_principal).Select(x => x.IT_asignacion_hardware).FirstOrDefault();

                if (asignacion != null)
                {
                    asignacionActual = asignacion.empleados1.ConcatNombre;
                    planta = asignacion.empleados1.plantas != null ? asignacion.empleados1.plantas.descripcion : String.Empty;
                    departamento = asignacion.empleados1.Area != null ? asignacion.empleados1.Area.descripcion : String.Empty;
                    puesto = asignacion.empleados1.puesto1 != null ? asignacion.empleados1.puesto1.descripcion : String.Empty;
                    jefeInmediato = asignacion.empleados1.empleados2 != null ? asignacion.empleados1.empleados2.ConcatNombre : String.Empty;

                }
                if (!esServer)
                    dt.Rows.Add(item.id, item.IT_inventory_hardware_type.descripcion, item.plantas.descripcion, item.hostname, item.brand, item.model, item.serial_number,
                    item.operation_system, item.bits_operation_system, item.cpu_speed_mhz, item.number_of_cpus, item.processor, item.mac_lan, item.mac_wlan,
                    item.total_physical_memory_gb, null, null, null, item.NumberOfHardDrives, item.TotalDiskSpace, item.maintenance_period_months,
                     item.purchase_date, item.end_warranty,
                     item.active, item.inactive_date, item.baja, item.fecha_baja,
                     item.last_check_int, item.os_version, item.primary_user, item.primary_user_email, item.primary_user_display, item.compliance, item.managed_by, item.encrypted, item.joinType, item.management_certificate_expiration_date,
                     item.comments, asignacionActual, planta, departamento, puesto, jefeInmediato
                    );
                else
                { //se agrega phyical server{
                    string virtualhost = null;
                    if (item.IT_inventory_items2 != null)
                        virtualhost = item.IT_inventory_items2.hostname;

                    dt.Rows.Add(item.id, item.IT_inventory_hardware_type.descripcion, item.plantas.descripcion, item.hostname, virtualhost, item.brand, item.model, item.serial_number,
                      item.operation_system, item.bits_operation_system, item.cpu_speed_mhz, item.number_of_cpus, item.processor, item.mac_lan, item.mac_wlan,
                      item.total_physical_memory_gb, null, null, null, item.NumberOfHardDrives, item.TotalDiskSpace, item.maintenance_period_months,
                       item.purchase_date, item.end_warranty,
                       item.active, item.inactive_date, item.baja, item.fecha_baja,
                        item.last_check_int, item.os_version, item.primary_user, item.primary_user_email, item.primary_user_display, item.compliance, item.managed_by, item.encrypted, item.joinType, item.management_certificate_expiration_date,
                       item.comments, string.Empty
                      );
                }

                filasEncabezados.Add(true);
                //obtiene la cantidad de fila actual
                int fila_inicial = filasEncabezados.Count + 1;

                foreach (IT_inventory_hard_drives hd in item.IT_inventory_hard_drives)
                {
                    System.Data.DataRow row = dt.NewRow();

                    row["Drive Letter"] = hd.disk_name;
                    row["Total Drive Space (GB)"] = hd.total_drive_space_gb;
                    row["Drive Type"] = hd.type_drive;

                    dt.Rows.Add(row);
                    filasEncabezados.Add(false);
                }

                //obtiene la fila final
                int fila_final = filasEncabezados.Count + 1;

                //verifica si hubo cambios
                if (fila_inicial != fila_final)
                    oSLDocument.GroupRows(fila_inicial, fila_final - 1);

                //agrega una fila por cada servidor virtual
                foreach (var vs in item.IT_inventory_items1)
                {
                    dt.Rows.Add(vs.id, vs.IT_inventory_hardware_type.descripcion, vs.plantas.descripcion, vs.hostname, vs.IT_inventory_items2.hostname, vs.brand, vs.model
                     , vs.serial_number, vs.operation_system, vs.bits_operation_system, vs.cpu_speed_mhz, vs.number_of_cpus, vs.processor, vs.mac_lan, vs.mac_wlan,
                    vs.total_physical_memory_gb, null, null, null, vs.NumberOfHardDrives, vs.TotalDiskSpace, vs.maintenance_period_months,
                    vs.purchase_date, vs.end_warranty, vs.active, vs.inactive_date, vs.baja,
                    vs.fecha_baja,
                      item.last_check_int, item.os_version, item.primary_user, item.primary_user_email, item.primary_user_display, item.compliance, item.managed_by, item.encrypted, item.joinType, item.management_certificate_expiration_date,
                    vs.comments);

                    filasEncabezados.Add(false);
                    filasServidoresVirtuales.Add(filasEncabezados.Count);


                    //obtiene la cantidad de fila actual
                    fila_inicial = filasEncabezados.Count + 1;

                    foreach (IT_inventory_hard_drives hd in vs.IT_inventory_hard_drives)
                    {
                        System.Data.DataRow row = dt.NewRow();

                        row["Drive Letter"] = hd.disk_name;
                        row["Total Drive Space (GB)"] = hd.total_drive_space_gb;
                        row["Drive Type"] = hd.type_drive;

                        dt.Rows.Add(row);
                        filasEncabezados.Add(false);
                    }

                    //obtiene la fila final
                    fila_final = filasEncabezados.Count + 1;

                    //verifica si hubo cambios
                    if (fila_inicial != fila_final)
                        oSLDocument.GroupRows(fila_inicial, fila_final - 1);
                }
            }

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Inventory " + inventoryType);
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            //estilo para el encabezado de cada fila
            SLStyle styleHeaderRow = oSLDocument.CreateStyle();
            styleHeaderRow.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#daeef3"), System.Drawing.ColorTranslator.FromHtml("#daeef3"));

            //estilo para el encabezado de cada fila (Virtual server)
            SLStyle styleHeaderRowVS = oSLDocument.CreateStyle();
            styleHeaderRowVS.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#fae6d7"), System.Drawing.ColorTranslator.FromHtml("#fae6d7"));

            //estilo para el encabezado de cada fila
            SLStyle styleHeaderRowDrive = oSLDocument.CreateStyle();
            styleHeaderRowDrive.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#778082"), System.Drawing.ColorTranslator.FromHtml("#778082"));

            //estilo para numeros
            SLStyle styleNumberInt = oSLDocument.CreateStyle();
            styleNumberInt.FormatCode = "#,##0";

            //estilo para numeros
            SLStyle styleNumberDecimal = oSLDocument.CreateStyle();
            styleNumberDecimal.FormatCode = "#,##0.00";


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(22 + physical + (esServer ? 1 : 0), styleShortDate);
            oSLDocument.SetColumnStyle(23 + physical + (esServer ? 1 : 0), styleShortDate);
            oSLDocument.SetColumnStyle(25 + physical + (esServer ? 1 : 0), styleShortDate);
            oSLDocument.SetColumnStyle(27 + physical + (esServer ? 1 : 0), styleShortDate); //fecha de baja
            oSLDocument.SetColumnStyle(28 + physical + (esServer ? 1 : 0), styleShortDate); //fecha Last check-in
            oSLDocument.SetColumnStyle(37 + physical + (esServer ? 1 : 0), styleShortDate); //Management Certificate


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);
            //aplica formato a las filas de encabezado
            for (int i = 0; i < filasEncabezados.Count; i++)
            {
                if (filasEncabezados[i])
                {
                    oSLDocument.SetCellStyle(i + 1, 1, i + 1, dt.Columns.Count, styleHeaderRow);
                }
                else
                {
                    oSLDocument.SetCellStyle(i + 1, 17 + physical, i + 1, 19 + physical, styleLoteInfo);
                }
                //colapsa todas las filas
                oSLDocument.CollapseRows(i + 2);
            }

            //aplica formato a los encabezados de VS
            foreach (var i in filasServidoresVirtuales)
                oSLDocument.SetCellStyle(i, 1, i, dt.Columns.Count, styleHeaderRowVS);

            //da estilo a los numero
            oSLDocument.SetColumnStyle(10 + physical + (esServer ? 1 : 0), styleNumberInt);
            oSLDocument.SetColumnStyle(17 + physical + (esServer ? 1 : 0), styleNumberDecimal);
            oSLDocument.SetColumnStyle(20 + physical + (esServer ? 1 : 0), styleNumberDecimal);
            oSLDocument.SetColumnStyle(15 + physical + (esServer ? 1 : 0), styleNumberDecimal); //decimal


            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnWidth(26 + physical + (esServer ? 1 : 0), 12); //decimal

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);

            //da color gris a cabeceras expandibles
            oSLDocument.SetCellStyle(1, 17 + physical, 1, 19 + physical + (esServer ? 1 : 0), styleHeaderRowDrive);

            oSLDocument.SetRowHeight(1, dt.Rows.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteITMonitorExcel(List<IT_inventory_items> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();


            //columnas          
            dt.Columns.Add("Id", typeof(string));       //1
            dt.Columns.Add("Type", typeof(string));     //2
            dt.Columns.Add("Plant", typeof(string));    //3
            dt.Columns.Add("Brand", typeof(string));    //4
            dt.Columns.Add("Model", typeof(string));    //5
            dt.Columns.Add("Serial Number", typeof(string));    //6
            dt.Columns.Add("Inches", typeof(string));           //7
            dt.Columns.Add("Purchase Date", typeof(DateTime));  //8       
            dt.Columns.Add("End Warranty", typeof(DateTime));   //9
            dt.Columns.Add("Is active?", typeof(bool));         //10
            dt.Columns.Add("Inactive Date?", typeof(DateTime)); //11
            dt.Columns.Add("Comments", typeof(string));         //12

            ////registros , rows
            foreach (IT_inventory_items item in listado)
            {
                dt.Rows.Add(item.id, item.IT_inventory_hardware_type.descripcion, item.plantas.descripcion, item.brand, item.model, item.serial_number, item.inches,
                    item.purchase_date, item.end_warranty, item.active, item.inactive_date, item.comments
                    );
            }

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Inventory Monitor");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));


            //estilo para numeros
            SLStyle styleNumberInt = oSLDocument.CreateStyle();
            styleNumberInt.FormatCode = "#,##0";


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(8, 9, styleShortDate);
            oSLDocument.SetColumnStyle(11, styleShortDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);


            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);


            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteITAccessoryExcel(List<IT_inventory_items> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();


            //columnas          
            dt.Columns.Add("Id", typeof(string));                //1
            dt.Columns.Add("Type", typeof(string));             //2
            dt.Columns.Add("Plant", typeof(string));             //3
            dt.Columns.Add("Accessory Type", typeof(string));    //4
            dt.Columns.Add("Brand", typeof(string));            //5
            dt.Columns.Add("Model", typeof(string));            //6
            dt.Columns.Add("Serial Number", typeof(string));    //7
            dt.Columns.Add("Purchase Date", typeof(DateTime));  //8       
            dt.Columns.Add("End Warranty", typeof(DateTime));   //9
            dt.Columns.Add("Is active?", typeof(bool));         //10
            dt.Columns.Add("Inactive Date?", typeof(DateTime)); //11
            dt.Columns.Add("¿Baja?", typeof(bool));         //12        
            dt.Columns.Add("Fecha baja", typeof(DateTime)); //13
            dt.Columns.Add("Comments", typeof(string));         //14

            ////registros , rows
            foreach (IT_inventory_items item in listado)
            {
                dt.Rows.Add(item.id, item.IT_inventory_hardware_type.descripcion, item.plantas.descripcion, item.IT_inventory_tipos_accesorios.descripcion, item.brand, item.model, item.serial_number,
                    item.purchase_date, item.end_warranty, item.active, item.inactive_date, item.baja, item.fecha_baja, item.comments
                    );
            }

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Inventory Accessories");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));


            //estilo para numeros
            SLStyle styleNumberInt = oSLDocument.CreateStyle();
            styleNumberInt.FormatCode = "#,##0";


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(8, 9, styleShortDate);
            oSLDocument.SetColumnStyle(11, styleShortDate);
            oSLDocument.SetColumnStyle(13, styleShortDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);


            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetColumnWidth(12, 12);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteITPrinterExcel(List<IT_inventory_items> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();


            //columnas          
            dt.Columns.Add("Id", typeof(string));           //1
            dt.Columns.Add("Type", typeof(string));         //2
            dt.Columns.Add("Plant", typeof(string));        //3
            dt.Columns.Add("Brand", typeof(string));        //4
            dt.Columns.Add("Model", typeof(string));        //5
            dt.Columns.Add("Serial Number", typeof(string));//6
            dt.Columns.Add("Printer Ubication", typeof(string));    //7
            dt.Columns.Add("Ip Address", typeof(string));           //8
            dt.Columns.Add("Purchase Date", typeof(DateTime));      //9    
            dt.Columns.Add("End Warranty", typeof(DateTime));       //10
            dt.Columns.Add("Is active?", typeof(bool));             //11
            dt.Columns.Add("Inactive Date?", typeof(DateTime));     //12
            dt.Columns.Add("¿Baja?", typeof(bool));                 //13         
            dt.Columns.Add("Fecha baja", typeof(DateTime));         //14
            dt.Columns.Add("Comments", typeof(string));             //15

            ////registros , rows
            foreach (IT_inventory_items item in listado)
            {
                dt.Rows.Add(item.id, item.IT_inventory_hardware_type.descripcion, item.plantas.descripcion, item.brand, item.model, item.serial_number, item.printer_ubication,
                    item.ip_adress, item.purchase_date, item.end_warranty,
                     item.active, item.inactive_date, item.baja, item.fecha_baja, item.comments
                    );
            }

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Inventory Printers");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));


            //estilo para numeros
            SLStyle styleNumberInt = oSLDocument.CreateStyle();
            styleNumberInt.FormatCode = "#,##0";


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(9, 10, styleShortDate);
            oSLDocument.SetColumnStyle(12, styleShortDate);
            oSLDocument.SetColumnStyle(14, styleShortDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);


            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetColumnWidth(13, 12); //decimal

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteITLabelPrinterExcel(List<IT_inventory_items> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();


            //columnas          
            dt.Columns.Add("Id", typeof(string));               //1
            dt.Columns.Add("Type", typeof(string));             //2
            dt.Columns.Add("Plant", typeof(string));            //3
            dt.Columns.Add("Brand", typeof(string));            //4
            dt.Columns.Add("Model", typeof(string));            //5
            dt.Columns.Add("Serial Number", typeof(string));    //6
            dt.Columns.Add("Printer Ubication", typeof(string));//7
            dt.Columns.Add("Ip Address", typeof(string));       //8
            dt.Columns.Add("Cost Center", typeof(string));      //9
            dt.Columns.Add("Purchase Date", typeof(DateTime));  //10          
            dt.Columns.Add("End Warranty", typeof(DateTime));   //11
            dt.Columns.Add("Is active?", typeof(bool));         //12
            dt.Columns.Add("Inactive Date?", typeof(DateTime)); //13
            dt.Columns.Add("¿Baja?", typeof(bool));             //14         
            dt.Columns.Add("Fecha baja", typeof(DateTime));     //15
            dt.Columns.Add("Comments", typeof(string));         //16

            ////registros , rows
            foreach (IT_inventory_items item in listado)
            {
                dt.Rows.Add(item.id, item.IT_inventory_hardware_type.descripcion, item.plantas.descripcion, item.brand, item.model, item.serial_number, item.printer_ubication,
                    item.ip_adress, item.cost_center, item.purchase_date, item.end_warranty,
                     item.active, item.inactive_date, item.baja, item.fecha_baja, item.comments
                    );
            }

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Inventory Label Printers");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));


            //estilo para numeros
            SLStyle styleNumberInt = oSLDocument.CreateStyle();
            styleNumberInt.FormatCode = "#,##0";


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(10, 11, styleShortDate);
            oSLDocument.SetColumnStyle(13, styleShortDate);
            oSLDocument.SetColumnStyle(15, styleShortDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);


            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetColumnWidth(14, 12); //decimal

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }
        public static byte[] GeneraReporteIM(List<view_ideas_mejora> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");
            System.Data.DataTable dt = new System.Data.DataTable();

            //columnas          
            dt.Columns.Add("Folio", typeof(string));
            dt.Columns.Add("Planta", typeof(string));
            dt.Columns.Add("Fecha Captura", typeof(DateTime));
            dt.Columns.Add("Estatus", typeof(string));
            dt.Columns.Add("Título", typeof(string));
            dt.Columns.Add("Situación Actual", typeof(string));
            dt.Columns.Add("Situación Propuesta", typeof(string));
            dt.Columns.Add("Aceptada por comite", typeof(bool));
            dt.Columns.Add("Idea en Equipo", typeof(bool));
            //dt.Columns.Add("Clasificación", typeof(string));
            dt.Columns.Add("Nivel Impacto", typeof(string));
            dt.Columns.Add("En Proceso Implementación", typeof(bool));
            dt.Columns.Add("Planta Implementación", typeof(string));
            dt.Columns.Add("Area Implementación", typeof(string));
            dt.Columns.Add("Idea Implementada", typeof(bool));
            dt.Columns.Add("Fecha Implementación", typeof(DateTime));
            dt.Columns.Add("Reconocimiento", typeof(string));
            dt.Columns.Add("Reconocimiento Monto", typeof(decimal));
            //dt.Columns.Add("Tipo Idea", typeof(string));
            dt.Columns.Add("Proponente 1", typeof(string));
            dt.Columns.Add("Proponente 2", typeof(string));
            dt.Columns.Add("Proponente 3", typeof(string));
            dt.Columns.Add("Proponente 4", typeof(string));
            dt.Columns.Add("Proponente 5", typeof(string));


            ////registros , rows
            foreach (var item in listado)
            {
                System.Data.DataRow row = dt.NewRow();

                dt.Rows.Add(item.folio, item.nombre_planta, item.fecha_captura, item.estatus_text, item.titulo, item.situacionActual, item.descripcion, item.comiteAceptada, item.ideaEnEquipo
                    , /*item.Clasificacion_text, */ item.nivel_impacto_text, item.enProcesoImplementacion, item.planta_implementacion_text, item.area_implementacion_text
                    , item.ideaImplementada, item.implementacionFecha, item.reconocimiento_text, item.reconocimientoMonto/*, item.tipo_idea*/
                    , item.proponente_1_nombre, item.proponente_2_nombre, item.proponente_3_nombre, item.proponente_4_nombre, item.proponente_5_nombre
                   );

            }

            //crea la hoja de  y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Reporte Ideas de Mejora");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));


            //estilo para numeros
            SLStyle styleNumber = oSLDocument.CreateStyle();
            styleNumber.FormatCode = "#,##0.00";

            //da estilo a los numero
            //oSLDocument.SetColumnStyle(18, styleNumber);

            ////estilo para fecha
            SLStyle styleLongDate = oSLDocument.CreateStyle();
            styleLongDate.FormatCode = "dd/MM/yyyy hh:mm AM/PM";
            oSLDocument.SetColumnStyle(dt.Columns["Fecha Captura"].Ordinal + 1, styleLongDate);
            oSLDocument.SetColumnStyle(dt.Columns["Fecha Implementación"].Ordinal + 1, styleLongDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;


            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetRowHeight(1, dt.Rows.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }


        public static byte[] GeneraReporteITPDAExcel(List<IT_inventory_items> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();


            //columnas          
            dt.Columns.Add("Id", typeof(string));                   //1
            dt.Columns.Add("Type", typeof(string));                 //2
            dt.Columns.Add("Plant", typeof(string));                //3
            dt.Columns.Add("Brand", typeof(string));                //4
            dt.Columns.Add("Model", typeof(string));                //5
            dt.Columns.Add("Serial Number", typeof(string));        //6
            dt.Columns.Add("MAC WLAN", typeof(string));             //7
            dt.Columns.Add("Purchase Date", typeof(DateTime));      //8           
            dt.Columns.Add("End Warranty", typeof(DateTime));       //9
            dt.Columns.Add("Is active?", typeof(bool));             //10
            dt.Columns.Add("Inactive Date?", typeof(DateTime));     //11
            dt.Columns.Add("¿Baja?", typeof(bool));                 //12         
            dt.Columns.Add("Fecha baja", typeof(DateTime));         //13
            dt.Columns.Add("Comments", typeof(string));             //14

            ////registros , rows
            foreach (IT_inventory_items item in listado)
            {
                dt.Rows.Add(item.id, item.IT_inventory_hardware_type.descripcion, item.plantas.descripcion, item.brand, item.model, item.serial_number, item.mac_wlan,
                    item.purchase_date, item.end_warranty, item.active, item.inactive_date, item.baja, item.fecha_baja, item.comments
                    );
            }

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Inventory PDAs");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));


            //estilo para numeros
            SLStyle styleNumberInt = oSLDocument.CreateStyle();
            styleNumberInt.FormatCode = "#,##0";


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(8, 9, styleShortDate);
            oSLDocument.SetColumnStyle(11, styleShortDate);
            oSLDocument.SetColumnStyle(13, styleShortDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);


            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetColumnWidth(12, 12); //decimal

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteITTabletExcel(List<IT_inventory_items> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();


            //columnas          
            dt.Columns.Add("Id", typeof(string));                       //1
            dt.Columns.Add("Type", typeof(string));                     //2
            dt.Columns.Add("Plant", typeof(string));                    //3
            dt.Columns.Add("Hostname", typeof(string));                    //4
            dt.Columns.Add("Brand", typeof(string));                    //5
            dt.Columns.Add("Model", typeof(string));                    //6
            dt.Columns.Add("Serial Number", typeof(string));            //7
            dt.Columns.Add("Inches", typeof(string));                   //8
            dt.Columns.Add("Processor", typeof(string));                //9
            dt.Columns.Add("Total Physical Memory (GB)", typeof(double));  //10
            dt.Columns.Add("Storage (GB)", typeof(double));                //11
            dt.Columns.Add("Operation System", typeof(string));         //12
            dt.Columns.Add("MAC WLAN", typeof(string));                 //13
            dt.Columns.Add("Purchase Date", typeof(DateTime));          //14             
            dt.Columns.Add("End Warranty", typeof(DateTime));           //15
            dt.Columns.Add("Is active?", typeof(bool));                 //16
            dt.Columns.Add("Inactive Date?", typeof(DateTime));         //17
            dt.Columns.Add("¿Baja?", typeof(bool));                     //18         
            dt.Columns.Add("Fecha baja", typeof(DateTime));             //19
            dt.Columns.Add("Comments", typeof(string));                 //20

            ////registros , rows
            foreach (IT_inventory_items item in listado)
            {
                dt.Rows.Add(item.id, item.IT_inventory_hardware_type.descripcion, item.plantas.descripcion, item.hostname, item.brand, item.model, item.serial_number, item.inches,
                    item.processor, item.total_physical_memory_gb, item.movil_device_storage_gb, item.operation_system, item.mac_wlan,
                    item.purchase_date, item.end_warranty,
                   item.active, item.inactive_date, item.baja, item.fecha_baja, item.comments
                    );
            }

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Inventory Tablet");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));


            //estilo para numeros
            SLStyle styleNumberDecimal = oSLDocument.CreateStyle();
            styleNumberDecimal.FormatCode = "#,##0.00";


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(14, 15, styleShortDate);
            oSLDocument.SetColumnStyle(17, styleShortDate);
            oSLDocument.SetColumnStyle(19, styleShortDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a los numeros
            oSLDocument.SetColumnStyle(10, 11, styleNumberDecimal);

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);


            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetColumnWidth(18, 12);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteITRadioExcel(List<IT_inventory_items> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();


            //columnas          
            dt.Columns.Add("Id", typeof(string));                   //1
            dt.Columns.Add("Type", typeof(string));                 //2
            dt.Columns.Add("Plant", typeof(string));                //3
            dt.Columns.Add("Brand", typeof(string));                //4
            dt.Columns.Add("Model", typeof(string));                //5
            dt.Columns.Add("Serial Number", typeof(string));        //6
            dt.Columns.Add("Purchase Date", typeof(DateTime));      //7     
            dt.Columns.Add("End Warranty", typeof(DateTime));       //8
            dt.Columns.Add("Is active?", typeof(bool));             //9
            dt.Columns.Add("Inactive Date?", typeof(DateTime));     //10
            dt.Columns.Add("¿Baja?", typeof(bool));                 //11         
            dt.Columns.Add("Fecha baja", typeof(DateTime));         //12
            dt.Columns.Add("Comments", typeof(string));             //13

            ////registros , rows
            foreach (IT_inventory_items item in listado)
            {
                dt.Rows.Add(item.id, item.IT_inventory_hardware_type.descripcion, item.plantas.descripcion, item.brand, item.model, item.serial_number,
                    item.purchase_date, item.end_warranty,
                    item.active, item.inactive_date, item.baja, item.fecha_baja, item.comments
                    );
            }

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Inventory Radio");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));


            //estilo para numeros
            SLStyle styleNumberInt = oSLDocument.CreateStyle();
            styleNumberInt.FormatCode = "#,##0";


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(7, 8, styleShortDate);
            oSLDocument.SetColumnStyle(10, styleShortDate);
            oSLDocument.SetColumnStyle(12, styleShortDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetColumnWidth(11, 12);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteITAPExcel(List<IT_inventory_items> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();


            //columnas          
            dt.Columns.Add("Id", typeof(string));                   //1
            dt.Columns.Add("Type", typeof(string));                 //2
            dt.Columns.Add("Plant", typeof(string));                //3
            dt.Columns.Add("Name", typeof(string));                 //4
            dt.Columns.Add("Brand", typeof(string));                //5
            dt.Columns.Add("Model", typeof(string));                //6
            dt.Columns.Add("Serial Number", typeof(string));        //7
            dt.Columns.Add("MAC LAN", typeof(string));              //8
            dt.Columns.Add("MAC WLAN", typeof(string));             //9
            dt.Columns.Add("IP Address", typeof(string));           //10
            dt.Columns.Add("Purchase Date", typeof(DateTime));      //11              
            dt.Columns.Add("End Warranty", typeof(DateTime));       //12
            dt.Columns.Add("Is active?", typeof(bool));             //13
            dt.Columns.Add("Inactive Date?", typeof(DateTime));     //14
            dt.Columns.Add("¿Baja?", typeof(bool));                 //15         
            dt.Columns.Add("Fecha baja", typeof(DateTime));         //16
            dt.Columns.Add("Comments", typeof(string));             //17

            ////registros , rows
            foreach (IT_inventory_items item in listado)
            {
                dt.Rows.Add(item.id, item.IT_inventory_hardware_type.descripcion, item.plantas.descripcion, item.hostname, item.brand, item.model, item.serial_number,
                    item.mac_lan, item.mac_wlan, item.ip_adress,
                    item.purchase_date, item.end_warranty,
                    item.active, item.inactive_date, item.baja, item.fecha_baja, item.comments
                    );
            }

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Inventory Radio");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));


            //estilo para numeros
            SLStyle styleNumberInt = oSLDocument.CreateStyle();
            styleNumberInt.FormatCode = "#,##0";


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(11, 12, styleShortDate);
            oSLDocument.SetColumnStyle(14, styleShortDate);
            oSLDocument.SetColumnStyle(16, styleShortDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);


            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetColumnWidth(15, 12);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteITSmartphoneExcel(List<IT_inventory_items> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();


            //columnas          
            dt.Columns.Add("Id", typeof(string));                           //1
            dt.Columns.Add("Type", typeof(string));                         //2
            dt.Columns.Add("Plant", typeof(string));                        //3
            dt.Columns.Add("Brand", typeof(string));                        //4
            dt.Columns.Add("Model", typeof(string));                        //5
            dt.Columns.Add("Serial Number", typeof(string));                //6
            dt.Columns.Add("Processor", typeof(string));                    //7
            dt.Columns.Add("Total Physical Memory (GB)", typeof(int));      //8
            dt.Columns.Add("Storage (GB)", typeof(int));                    //9
            dt.Columns.Add("Operation System", typeof(string));             //10
            dt.Columns.Add("MAC WLAN", typeof(string));                     //11
            dt.Columns.Add("IMEI 1", typeof(string));                       //12
            dt.Columns.Add("IMEI 2", typeof(string));                       //13
            dt.Columns.Add("Purchase Date", typeof(DateTime));              //14
            dt.Columns.Add("End Warranty", typeof(DateTime));               //15
            dt.Columns.Add("Is active?", typeof(bool));                     //16
            dt.Columns.Add("Inactive Date?", typeof(DateTime));             //17
            dt.Columns.Add("¿Baja?", typeof(bool));                         //18         
            dt.Columns.Add("Fecha baja", typeof(DateTime));                 //19
            dt.Columns.Add("Comments", typeof(string));                     //20

            ////registros , rows
            foreach (IT_inventory_items item in listado)
            {
                dt.Rows.Add(item.id, item.IT_inventory_hardware_type.descripcion, item.plantas.descripcion, item.brand, item.model, item.serial_number,
                    item.processor, item.total_physical_memory_gb, item.movil_device_storage_gb, item.operation_system, item.mac_wlan, item.imei_1, item.imei_2,
                    item.purchase_date, item.end_warranty,
                    item.active, item.inactive_date, item.baja, item.fecha_baja, item.comments
                    );
            }

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Inventory Smartphone");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));


            //estilo para numeros
            SLStyle styleNumberDecimal = oSLDocument.CreateStyle();
            styleNumberDecimal.FormatCode = "#,##0.00";


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(14, 15, styleShortDate);
            oSLDocument.SetColumnStyle(17, styleShortDate);
            oSLDocument.SetColumnStyle(19, styleShortDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a los numeros
            oSLDocument.SetColumnStyle(8, 9, styleNumberDecimal);

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);


            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetColumnWidth(18, 12);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteITSoftwareExcel(List<IT_inventory_software> listado)
        {

            SLDocument oSLDocument = new SLDocument();
            oSLDocument.AddWorksheet("Sheet1");
            oSLDocument.SelectWorksheet("Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();
            //para llevar el control de si es encabezado o no
            List<bool> filasEncabezados = new List<bool>();
            filasEncabezados.Add(false); //es el encabezado principal

            //columnas          
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Matriz RH?", typeof(bool));
            dt.Columns.Add("Active?", typeof(bool));
            //dt.Columns.Add("Version", typeof(string));
            //dt.Columns.Add("Version Active?", typeof(bool));


            ////registros , rows
            foreach (IT_inventory_software item in listado)
            {
                dt.Rows.Add(item.id, item.descripcion, item.disponible_en_matriz_rh, item.activo
                    );

                filasEncabezados.Add(true);
                //obtiene la cantidad de fila actual
                int fila_inicial = filasEncabezados.Count + 1;

                //foreach (IT_inventory_software_versions sv in item.IT_inventory_software_versions)
                //{
                //    System.Data.DataRow row = dt.NewRow();

                //    row["Version"] = sv.version;
                //    row["Version Active?"] = sv.activo;

                //    dt.Rows.Add(row);
                //    filasEncabezados.Add(false);
                //}

                //obtiene la fila final
                int fila_final = filasEncabezados.Count + 1;

                //verifica si hubo cambios
                if (fila_inicial != fila_final)
                    oSLDocument.GroupRows(fila_inicial, fila_final - 1);
            }

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Inventory Software");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            //estilo para el encabezado de cada fila
            SLStyle styleHeaderRow = oSLDocument.CreateStyle();
            styleHeaderRow.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#daeef3"), System.Drawing.ColorTranslator.FromHtml("#daeef3"));

            //estilo para el encabezado de cada fila
            SLStyle styleHeaderRowDrive = oSLDocument.CreateStyle();
            styleHeaderRowDrive.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#778082"), System.Drawing.ColorTranslator.FromHtml("#778082"));

            //estilo para numeros
            SLStyle styleNumberInt = oSLDocument.CreateStyle();
            styleNumberInt.FormatCode = "#,##0";


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            ////estilo para fecha
            //SLStyle styleLongDate = oSLDocument.CreateStyle();
            //styleLongDate.FormatCode = "yyyy/MM/dd";
            //oSLDocument.SetColumnStyle(10, styleLongDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);
            //aplica formato a las filas de encabezado
            oSLDocument.SetCellStyle(1, 1, 1, dt.Columns.Count, styleHeaderRow);



            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);


            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);

            //da color gris a cabeceras expandibles
            //oSLDocument.SetCellStyle(1, 4, 1, 5, styleHeaderRowDrive);
            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);
            oSLDocument.SetColumnWidth(3, 4, 12.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteITPlanesTelefoniaExcel(List<IT_inventory_cellular_plans> listado)
        {


            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();


            //columnas          
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Plan Nombre", typeof(string));
            dt.Columns.Add("Compañia", typeof(string));
            dt.Columns.Add("Precio", typeof(decimal));
            dt.Columns.Add("Comentarios", typeof(string));
            dt.Columns.Add("Activo?", typeof(bool));

            ////registros , rows
            foreach (IT_inventory_cellular_plans item in listado)
            {
                dt.Rows.Add(item.id, item.nombre_plan, item.nombre_compania, item.precio, item.comentarios, item.activo);
            }



            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Planes Telefonía");
            oSLDocument.ImportDataTable(1, 1, dt, true);


            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));


            //estilo para numeros
            SLStyle styleCurrency = oSLDocument.CreateStyle();
            styleCurrency.FormatCode = "$ #,##0.00";
            oSLDocument.SetColumnStyle(4, styleCurrency);


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            //////estilo para fecha
            //SLStyle styleLongDate = oSLDocument.CreateStyle();
            //styleLongDate.FormatCode = "yyyy/MM/dd";
            //oSLDocument.SetColumnStyle(7, styleLongDate);

            //oSLDocument.SetColumnStyle(10, 17, styleNumberInt);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);


            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);

            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetRowHeight(1, 45.0);
            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            oSLDocument.AutoFitColumn(1, dt.Columns.Count);


            oSLDocument.SetColumnWidth(6, 12);


            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteITscannerExcel(List<IT_inventory_items> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();


            //columnas          
            dt.Columns.Add("Code", typeof(string));                   //1
            dt.Columns.Add("Type", typeof(string));                 //2
            dt.Columns.Add("Plant", typeof(string));                //3
            dt.Columns.Add("Brand", typeof(string));                //4
            dt.Columns.Add("Model", typeof(string));                //5
            dt.Columns.Add("Serial Number", typeof(string));        //6
            dt.Columns.Add("MAC WLAN", typeof(string));             //7
            dt.Columns.Add("Purchase Date", typeof(DateTime));      //8           
            dt.Columns.Add("End Warranty", typeof(DateTime));       //9
            dt.Columns.Add("Is active?", typeof(bool));             //10
            dt.Columns.Add("Inactive Date?", typeof(DateTime));     //11         
            dt.Columns.Add("¿Baja?", typeof(bool));                 //12         
            dt.Columns.Add("Fecha baja", typeof(DateTime));         //13
            dt.Columns.Add("Accessories", typeof(string));          //14
            dt.Columns.Add("Comments", typeof(string));             //15

            ////registros , rows
            foreach (IT_inventory_items item in listado)
            {
                dt.Rows.Add(item.code, item.IT_inventory_hardware_type.descripcion, item.plantas.descripcion, item.brand, item.model, item.serial_number, item.mac_wlan,
                    item.purchase_date, item.end_warranty, item.active, item.inactive_date, item.baja, item.fecha_baja, item.accessories, item.comments
                    );
            }

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Inventory Scanners");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));


            //estilo para numeros
            SLStyle styleNumberInt = oSLDocument.CreateStyle();
            styleNumberInt.FormatCode = "#,##0";


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(8, 9, styleShortDate);
            oSLDocument.SetColumnStyle(11, styleShortDate);
            oSLDocument.SetColumnStyle(13, styleShortDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);


            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetColumnWidth(12, 12);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        /// <summary>
        /// Genera el reporte en Excel
        /// </summary>
        /// <param name="listadoPPM"></param>
        /// <returns></returns>
        public static byte[] GeneraReportePPMExcel(List<inspeccion_categoria_fallas> listadoFallas, List<PPM> listadoPPM, List<produccion_registros> listadoRegistros, List<view_historico_resultado> listadoHistorico)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            // SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_piezas_descarte.xlsx"), "Sheet1");
            // Esto elimina la dependencia del archivo físico y previene problemas de corrupción de plantillas.
            SLDocument oSLDocument = new SLDocument();

            System.Data.DataTable dt = new System.Data.DataTable();

            //crea la hoja de resumen y la selecciona
            string hojaReportePorDia = "Reporte Por Día";
            //crear la hoja y la selecciona
            oSLDocument.AddWorksheet(hojaReportePorDia);
            oSLDocument.SelectWorksheet(hojaReportePorDia);


            #region styles
            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            styleWrap.SetVerticalAlignment(VerticalAlignmentValues.Center);

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            //estilo para renglones
            SLStyle styleFondoRenglon = oSLDocument.CreateStyle();
            styleFondoRenglon.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#dce6f1"), System.Drawing.ColorTranslator.FromHtml("#dce6f1"));

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";

            SLStyle styleLongDate = oSLDocument.CreateStyle();
            styleLongDate.FormatCode = "yyyy/MM/dd h:mm AM/PM";

            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            SLStyle styleNumber = oSLDocument.CreateStyle();
            styleNumber.FormatCode = "#,##0.00";

            //style para vertical
            SLStyle styleVertical = oSLDocument.CreateStyle();
            styleVertical.Alignment.Horizontal = HorizontalAlignmentValues.Left;
            // Alternatively, use the shortcut function:
            // style.SetHorizontalAlignment(HorizontalAlignmentValues.Left);

            // each indent is 3 spaces, so this is 15 spaces total           
            styleVertical.Alignment.ReadingOrder = SLAlignmentReadingOrderValues.RightToLeft;
            styleVertical.Alignment.ShrinkToFit = true;
            styleVertical.Alignment.TextRotation = 90;
            styleVertical.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            styleVertical.SetVerticalAlignment(VerticalAlignmentValues.Center);

            styleVertical.SetWrapText(true);

            //style para centrar
            SLStyle styleCenter = oSLDocument.CreateStyle();
            styleCenter.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            styleCenter.SetVerticalAlignment(VerticalAlignmentValues.Center);

            //style para border
            SLStyle styleBorder = oSLDocument.CreateStyle();
            styleBorder.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.TopBorder.Color = System.Drawing.Color.DarkGray;
            styleBorder.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.LeftBorder.Color = System.Drawing.Color.DarkGray;
            styleBorder.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.RightBorder.Color = System.Drawing.Color.DarkGray;
            styleBorder.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorder.Border.BottomBorder.Color = System.Drawing.Color.DarkGray;

            #endregion

            #region resumen_ppm
            //columnas          
            dt.Columns.Add("Fecha", typeof(DateTime));
            dt.Columns.Add("PIEZAS DESCARTE kilogramos internos", typeof(double));
            dt.Columns.Add("PIEZAS CORTADAS Toneladas", typeof(double));
            dt.Columns.Add("PPM's internos", typeof(double));

            ////registros , rows
            foreach (PPM item in listadoPPM)
            {
                dt.Rows.Add(item.fecha, item.PiezasDescarteKG_internos, item.PiezasCortadasTon, item.CalculoPPM_interno);
            }


            oSLDocument.ImportDataTable(1, 1, dt, true);

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter("A1", "D1");


            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);

            oSLDocument.SetRowHeight(1, listadoPPM.Count + 1, 15.0);

            //da estilo a los numero
            oSLDocument.SetColumnStyle(4, styleNumber);
            //da estilo a la fecha
            oSLDocument.SetColumnStyle(1, styleShortDate);
            //ajusta al texto
            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            #endregion

            #region resumen_registros

            string hojaCalidad = "Calidad";

            //crear la hoja y la selecciona
            oSLDocument.AddWorksheet(hojaCalidad);
            oSLDocument.SelectWorksheet(hojaCalidad);

            //reinicia la tabla
            dt = new System.Data.DataTable();

            //agrega encabezados
            dt.Columns.Add("Fecha", typeof(DateTime));
            dt.Columns.Add("Turno", typeof(string));
            dt.Columns.Add("Supervisor", typeof(string));
            dt.Columns.Add("Operador", typeof(string));
            dt.Columns.Add("Inspector", typeof(string));
            dt.Columns.Add("Número de parte", typeof(string));
            dt.Columns.Add("Número SAP rollo", typeof(string));
            dt.Columns.Add("Número SAP platina", typeof(string));
            dt.Columns.Add("Lote", typeof(string));
            dt.Columns.Add("Observaciones", typeof(string));
            dt.Columns.Add("Total Piezas", typeof(string));

            foreach (inspeccion_categoria_fallas falla_c in listadoFallas)
                foreach (inspeccion_fallas falla in falla_c.inspeccion_fallas.OrderBy(x => x.id))
                    dt.Columns.Add(falla.descripcion, typeof(int));

            dt.Columns.Add("Peso Neto Unitario", typeof(double));
            dt.Columns.Add("Subtotal Piezas Daño Interno", typeof(double));
            dt.Columns.Add("Total de Kg NG internos", typeof(double));
            dt.Columns.Add("Peso Total Piezas OK", typeof(double));

            dt.Columns.Add("Peso Puntas", typeof(double));
            dt.Columns.Add("Peso Colas", typeof(double));

            foreach (produccion_registros item in listadoRegistros)
            {
                string inspectorName = String.Empty;
                string loteRollo = String.Empty;
                string comentariosInspeccion = string.Empty;
                double? pesoNetoUnitario = null;
                double? totalPiezasOk = null;
                double? pesoPuntas = null;
                double? pesoColas = null;

                //obtiene datos de inspeccion            
                if (item.inspeccion_datos_generales != null && item.inspeccion_datos_generales.empleados != null)
                {
                    inspectorName = item.inspeccion_datos_generales.empleados.ConcatNombre;
                    comentariosInspeccion = item.inspeccion_datos_generales.comentarios;
                }

                //obtiene valores de datos entrada
                if (item.produccion_datos_entrada != null)
                {
                    loteRollo = item.produccion_datos_entrada.lote_rollo;
                    pesoNetoUnitario = item.produccion_datos_entrada.peso_real_pieza_neto;
                    totalPiezasOk = item.produccion_datos_entrada.PesoRegresoRolloUsado;
                    pesoPuntas = item.produccion_datos_entrada.peso_despunte_kgs;
                    pesoColas = item.produccion_datos_entrada.peso_cola_kgs;
                }

                //crea una fila
                System.Data.DataRow row1 = dt.NewRow();
                row1["fecha"] = item.fecha;
                row1["turno"] = item.produccion_turnos.valor;
                row1["supervisor"] = item.produccion_supervisores.empleados.ConcatNombre;
                row1["operador"] = item.produccion_operadores.empleados.ConcatNombre;
                row1["inspector"] = inspectorName;
                row1["Número de parte"] = item.Class_asociado.Customer_part_number;
                row1["Número SAP rollo"] = item.sap_rollo;
                row1["Número SAP platina"] = item.sap_platina;
                row1["Lote"] = loteRollo;
                row1["Observaciones"] = comentariosInspeccion;
                row1["Total Piezas"] = item.TotalPiezasProduccion();


                foreach (inspeccion_categoria_fallas falla_c in listadoFallas)
                    foreach (inspeccion_fallas falla in falla_c.inspeccion_fallas.OrderBy(x => x.id))
                    {
                        //verifica si tiene alguna pieza de descarte falla con ese id
                        inspeccion_pieza_descarte_produccion pza_descarte = item.inspeccion_pieza_descarte_produccion.FirstOrDefault(x => x.id_falla == falla.id);
                        if (pza_descarte != null)
                            row1[falla.descripcion] = pza_descarte.cantidad;
                        else
                            row1[falla.descripcion] = DBNull.Value;
                    }
                if (pesoNetoUnitario.HasValue)
                    row1["Peso Neto Unitario"] = pesoNetoUnitario;
                else
                    row1["Peso Neto Unitario"] = DBNull.Value;

                row1["Subtotal Piezas Daño Interno"] = item.NumPiezasDescarteDanoInterno();
                row1["Total de Kg NG internos"] = item.produccion_datos_entrada.TotalKgNGInterno();

                if (totalPiezasOk.HasValue)
                    row1["Peso Total Piezas OK"] = totalPiezasOk;
                else
                    row1["Peso Total Piezas OK"] = DBNull.Value;

                if (pesoPuntas.HasValue)
                    row1["Peso Puntas"] = pesoPuntas;
                else
                    row1["Peso Puntas"] = DBNull.Value;

                if (pesoColas.HasValue)
                    row1["Peso Colas"] = pesoColas;
                else
                    row1["Peso Colas"] = DBNull.Value;

                dt.Rows.Add(row1);
            }



            //encabezados
            int columna_falla = 12; //inicio
            //recorre cada tipo de falla
            foreach (inspeccion_categoria_fallas falla_c in listadoFallas)
            {
                oSLDocument.SetCellValue(1, columna_falla, falla_c.descripcion);
                int inicioColumna = columna_falla;

                //cambia el color de la celda
                SLStyle styleFondo = oSLDocument.CreateStyle();
                styleFondo.Font.Bold = true;
                //determina el color del fondo
                switch (falla_c.id)
                {
                    case 1:
                        styleFondo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#c6e0b4"), System.Drawing.ColorTranslator.FromHtml("#c6e0b4"));
                        break;
                    case 2:
                    case 3:
                        styleFondo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#bdd7ee"), System.Drawing.ColorTranslator.FromHtml("#bdd7ee"));
                        break;
                    case 4:
                    case 5:
                        styleFondo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#fff2cc"), System.Drawing.ColorTranslator.FromHtml("#fff2cc"));
                        break;
                    default:
                        styleFondo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#d6dce4"), System.Drawing.ColorTranslator.FromHtml("#d6dce4"));
                        break;
                }
                //recorre cada tipo de falla
                foreach (inspeccion_fallas falla in falla_c.inspeccion_fallas.OrderBy(x => x.id))
                {
                    oSLDocument.SetCellValue(2, columna_falla, falla.id);
                    oSLDocument.SetCellStyle(2, columna_falla, styleFondo);
                    oSLDocument.SetCellStyle(3, columna_falla, styleFondo);
                    columna_falla++;
                }

                oSLDocument.MergeWorksheetCells(1, inicioColumna, 1, columna_falla - 1);
                oSLDocument.SetCellStyle(1, inicioColumna, styleFondo);

            }

            //agrega el color de fondo a los ultimos resultados          
            SLStyle styleFondoGray = oSLDocument.CreateStyle();
            styleFondoGray.Font.Bold = true;
            styleFondoGray.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#bebebe"), System.Drawing.ColorTranslator.FromHtml("#bebebe"));
            oSLDocument.SetCellStyle(3, columna_falla, listadoRegistros.Count + 3, columna_falla + 5, styleFondoGray);

            oSLDocument.ImportDataTable(3, 1, dt, true);

            //agrega el color de fondo para las filas
            for (int i = 4; i <= listadoRegistros.Count + 3; i++)
            {
                if (i % 2 == 0)
                    oSLDocument.SetCellStyle(i, 1, i, columna_falla - 1, styleFondoRenglon);
            }



            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(3, 9);

            oSLDocument.Filter("A3", "BR3");

            styleHeaderFont.Alignment.TextRotation = 90;

            oSLDocument.SetCellStyle(3, 1, 1, 11, styleHeader);
            oSLDocument.SetCellStyle(3, 1, 1, 11, styleHeaderFont);

            //oSLDocument.SetRowHeight(3, listadoRegistros.Count + 3, 15.0);
            oSLDocument.SetRowHeight(1, 30);
            oSLDocument.SetRowHeight(3, listadoRegistros.Count + 3, 120);

            //da estilo a los numero
            //oSLDocument.SetColumnStyle(4, styleNumber);
            //da formato a la fecha
            oSLDocument.SetColumnStyle(1, styleLongDate);
            //ajusta al texto
            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);

            //encabezado
            oSLDocument.SetRowStyle(3, styleVertical);
            //primeros datos
            oSLDocument.SetColumnStyle(1, 9, styleVertical);

            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            //coloca borde
            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleBorder);

            //cambia tamaño de la columna 3
            oSLDocument.SetRowHeight(3, 120);

            #endregion

            #region sabana_produccion

            //crear la hoja y la selecciona
            string nombreHojaProduccion = "Producción";

            oSLDocument.SelectWorksheet("Sheet1");
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, nombreHojaProduccion);
            oSLDocument.MoveWorksheet(nombreHojaProduccion, 3);

            //reinicia la tabla
            dt = new System.Data.DataTable();

            //para llevar el control de si es encabezado o no
            List<bool> filasEncabezados = new List<bool>();
            List<bool> filasTemporales = new List<bool>();
            //agrega tres para el encabezadoprincipal
            filasEncabezados.Add(false); //es el encabezado principal
            filasTemporales.Add(false);
            filasEncabezados.Add(false); //es el encabezado principal
            filasTemporales.Add(false);
            filasEncabezados.Add(false); //es el encabezado principal
            filasTemporales.Add(false);

            //columnas
            dt.Columns.Add(nameof(view_historico_resultado.Planta), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Linea), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Operador), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Supervisor), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Fecha), typeof(DateTime));
            dt.Columns.Add(nameof(view_historico_resultado.Hora), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Turno), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Orden_SAP), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.SAP_Platina), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Tipo_de_Material), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Número_de_Parte__de_cliente), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Material), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Orden_en_SAP_2), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.SAP_Platina_2), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Tipo_de_Material_platina2), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Número_de_Parte_de_Cliente_platina2), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Material_platina2), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.SAP_Rollo), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.N__de_Rollo), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Lote_de_rollo), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Etiqueta__Kg_), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_de_regreso_de_rollo_Real), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_de_rollo_usado), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Báscula_Kgs), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Pieza_por_Golpe), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Ordenes_por_pieza), typeof(double));
            dt.Columns.Add("Lote_Material", typeof(string));
            dt.Columns.Add("No_lote_izq", typeof(int));
            dt.Columns.Add("No_lote_der", typeof(int));
            dt.Columns.Add("Lote_piezas_por_paquete", typeof(int));
            dt.Columns.Add(nameof(view_historico_resultado.Total_de_piezas_platina1), typeof(int));
            dt.Columns.Add(nameof(view_historico_resultado.Total_de_piezas_platina2), typeof(int));
            dt.Columns.Add(nameof(view_historico_resultado.Total_de_piezas), typeof(int));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_de_rollo_consumido), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Numero_de_golpes), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Kg_restante_de_rollo), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_despunte_kgs_), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_cola_Kgs_), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Porcentaje_de_puntas_y_colas), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Total_de_piezas_de_Ajustes_platina1), typeof(int));
            dt.Columns.Add(nameof(view_historico_resultado.Total_de_piezas_de_Ajustes_platina2), typeof(int));
            dt.Columns.Add(nameof(view_historico_resultado.Total_de_piezas_de_Ajustes), typeof(double));

            dt.Columns.Add(nameof(view_historico_resultado.Peso_Bruto_Kgs), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Real_Pieza_Bruto), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Real_Pieza_Neto), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Scrap_Natural), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_neto_SAP), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Bruto_SAP), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Balance_de_Scrap), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_de_rollo_usado_real__Kg), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_bruto_Total_piezas_Kg), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_NetoTotal_piezas_Kg), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Neto_total_piezas_de_ajuste_Kgs), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_puntas_y_colas_reales_Kg), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Balance_de_Scrap_Real), typeof(double));

            dt.Columns.Add(nameof(view_historico_resultado.Peso_Bruto_Kgs_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Real_Pieza_Bruto_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Real_Pieza_Neto_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Scrap_Natural_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_neto_SAP_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Bruto_SAP_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Balance_de_Scrap_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_de_rollo_usado_real__Kg_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_bruto_Total_piezas_Kg_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_NetoTotal_piezas_Kg_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Neto_total_piezas_de_ajuste_Kgs_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_puntas_y_colas_reales_Kg_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Balance_de_Scrap_Real_platina2), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Bruto_Kgs_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Real_Pieza_Bruto_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Real_Pieza_Neto_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Scrap_Natural_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_neto_SAP_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Bruto_SAP_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Balance_de_Scrap_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_de_rollo_usado_real__Kg_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_bruto_Total_piezas_Kg_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_NetoTotal_piezas_Kg_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_Neto_total_piezas_de_ajuste_Kgs_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Peso_puntas_y_colas_reales_Kg_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.Balance_de_Scrap_Real_general), typeof(double));
            dt.Columns.Add(nameof(view_historico_resultado.comentario), typeof(string));


            // ====================================================================================================
            // ===== INICIO DE LA NUEVA REGIÓN PARA LA CABECERA PERSONALIZADA =====================================
            // ====================================================================================================
            #region Creación de Encabezado Personalizado (Hoja Producción)

            // 1. Definir los estilos para la cabecera (colores, fuentes, alineación)
            SLStyle styleHeaderTop = oSLDocument.CreateStyle();
            styleHeaderTop.Fill.SetPattern(PatternValues.Solid, ColorTranslator.FromHtml("#009ff5"), ColorTranslator.FromHtml("#009ff5"));
            styleHeaderTop.Font.FontColor = System.Drawing.Color.White;
            styleHeaderTop.Font.Bold = true;
            styleHeaderTop.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            styleHeaderTop.SetVerticalAlignment(VerticalAlignmentValues.Center);

            SLStyle styleTitle = oSLDocument.CreateStyle();
            styleTitle.Font.Bold = true;
            styleTitle.Font.FontSize = 12;
            styleTitle.SetVerticalAlignment(VerticalAlignmentValues.Center);

            SLStyle styleGroupOperador = oSLDocument.CreateStyle();
            styleGroupOperador.Fill.SetPattern(PatternValues.Solid, ColorTranslator.FromHtml("#009ff5"), ColorTranslator.FromHtml("#009ff5"));
            styleGroupOperador.Font.FontColor = System.Drawing.Color.White;
            styleGroupOperador.Font.Bold = true;
            styleGroupOperador.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            styleGroupOperador.SetVerticalAlignment(VerticalAlignmentValues.Center);

            SLStyle styleGroupPlatina2 = oSLDocument.CreateStyle();
            styleGroupPlatina2.Fill.SetPattern(PatternValues.Solid, ColorTranslator.FromHtml("#1F4E78"), ColorTranslator.FromHtml("#1F4E78"));
            styleGroupPlatina2.Font.FontColor = System.Drawing.Color.White;
            styleGroupPlatina2.Font.Bold = true;
            styleGroupPlatina2.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            styleGroupPlatina2.SetVerticalAlignment(VerticalAlignmentValues.Center);

            SLStyle styleGroupPlatina1 = oSLDocument.CreateStyle();
            styleGroupPlatina1.Fill.SetPattern(PatternValues.Solid, ColorTranslator.FromHtml("#843C0C"), ColorTranslator.FromHtml("#843C0C"));
            styleGroupPlatina1.Font.FontColor = System.Drawing.Color.White;
            styleGroupPlatina1.Font.Bold = true;
            styleGroupPlatina1.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            styleGroupPlatina1.SetVerticalAlignment(VerticalAlignmentValues.Center);

            // Crear una copia de los estilos de grupo para el texto rotado de la Fila 3
            SLStyle styleGroupOperadorRotated = styleGroupOperador.Clone();
            // --- AÑADIR BORDES BLANCOS ---
            styleGroupOperadorRotated.Border.SetTopBorder(BorderStyleValues.Thin, System.Drawing.Color.White);
            styleGroupOperadorRotated.Border.SetBottomBorder(BorderStyleValues.Thin, System.Drawing.Color.White);
            styleGroupOperadorRotated.Border.SetLeftBorder(BorderStyleValues.Thin, System.Drawing.Color.White);
            styleGroupOperadorRotated.Border.SetRightBorder(BorderStyleValues.Thin, System.Drawing.Color.White);
            styleGroupOperadorRotated.SetWrapText(true); // <--- AÑADIR AQUÍ TAMBIÉN


            SLStyle styleGroupPlatina2Rotated = styleGroupPlatina2.Clone();
            styleGroupPlatina2Rotated.Border.SetTopBorder(BorderStyleValues.Thin, System.Drawing.Color.White);
            styleGroupPlatina2Rotated.Border.SetBottomBorder(BorderStyleValues.Thin, System.Drawing.Color.White);
            styleGroupPlatina2Rotated.Border.SetLeftBorder(BorderStyleValues.Thin, System.Drawing.Color.White);
            styleGroupPlatina2Rotated.Border.SetRightBorder(BorderStyleValues.Thin, System.Drawing.Color.White);
            styleGroupPlatina2Rotated.SetWrapText(true); // <--- AÑADIR AQUÍ TAMBIÉN


            SLStyle styleGroupPlatina1Rotated = styleGroupPlatina1.Clone();
            styleGroupPlatina1Rotated.Border.SetTopBorder(BorderStyleValues.Thin, System.Drawing.Color.White);
            styleGroupPlatina1Rotated.Border.SetBottomBorder(BorderStyleValues.Thin, System.Drawing.Color.White);
            styleGroupPlatina1Rotated.Border.SetLeftBorder(BorderStyleValues.Thin, System.Drawing.Color.White);
            styleGroupPlatina1Rotated.Border.SetRightBorder(BorderStyleValues.Thin, System.Drawing.Color.White);
            styleGroupPlatina2Rotated.SetWrapText(true); // <--- AÑADIR AQUÍ TAMBIÉN

            // 2. Construir la Fila 1
            oSLDocument.SetRowHeight(1, 22);
            oSLDocument.SetCellValue("A1", DateTime.Now.ToString("dd/MM/yyyy"));
            oSLDocument.SetCellStyle("A1", styleGroupOperador);
            oSLDocument.SetCellStyle("A1","CG2" , styleGroupOperador);

            oSLDocument.MergeWorksheetCells("B1", "I1");
            oSLDocument.SetCellValue("B1", "thyssenkrupp Materials de México S.A de C.V.");
            oSLDocument.SetCellStyle("B1", "I1", styleGroupOperador);

            oSLDocument.MergeWorksheetCells("J1", "O1");
            oSLDocument.SetCellValue("J1", "PRF014-04");
            SLStyle styleDocCode = styleGroupOperador.Clone();
            styleDocCode.SetHorizontalAlignment(HorizontalAlignmentValues.Right);
            oSLDocument.SetCellStyle("J1", "O1", styleDocCode);

            // 3. Construir la Fila 2 (Cabeceras de Grupo)
            oSLDocument.SetRowHeight(2, 20);

            // Grupo 'Operador'
            oSLDocument.MergeWorksheetCells("A2", "D2");
            oSLDocument.SetCellValue("A2", "Operador");
            oSLDocument.SetCellStyle("A2", "G2", styleGroupOperadorRotated);

            // Grupo 'Platina 2'
            oSLDocument.MergeWorksheetCells("H2", "L2");
            oSLDocument.SetCellValue("H2", "Platina 2");
            oSLDocument.SetCellStyle("H2", "L2", styleGroupPlatina2);

            // Grupo 'Platina 1'
            oSLDocument.MergeWorksheetCells("M2", "Q2");
            oSLDocument.SetCellValue("M2", "Platina 1");
            oSLDocument.SetCellStyle("M2", "Q2", styleGroupPlatina1);

            // (Añadir aquí más grupos si es necesario, siguiendo el mismo patrón)

            // 4. Construir la Fila 3 (Cabeceras de Columna, usando los nombres del DataTable)
            oSLDocument.SetRowHeight(3, 90);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                // Escribir el nombre de la columna (reemplazando guiones bajos por espacios)
                oSLDocument.SetCellValue(3, i + 1, dt.Columns[i].ColumnName.Replace("_", " "));

                // Aplicar el estilo rotado correspondiente al grupo de la columna
                int colIndex = i + 1;
                if (colIndex >= 1 && colIndex <= 7) oSLDocument.SetCellStyle(3, colIndex, styleGroupOperadorRotated);
                else if (colIndex >= 8 && colIndex <= 12) oSLDocument.SetCellStyle(3, colIndex, styleGroupPlatina2Rotated);
                else if (colIndex >= 13 && colIndex <= 17) oSLDocument.SetCellStyle(3, colIndex, styleGroupPlatina1Rotated);
                else oSLDocument.SetCellStyle(3, colIndex, styleGroupOperadorRotated); // Estilo por defecto para el resto
            }

            #endregion
            // ====================================================================================================
            // ===== FIN DE LA NUEVA REGIÓN =======================================================================
            // ====================================================================================================



            //registros , rows
            foreach (view_historico_resultado item in listadoHistorico)
            {
                if (String.IsNullOrEmpty(item.SAP_Platina_2))
                {
                    item.Peso_Bruto_Kgs_platina2 = null;
                    item.Peso_Real_Pieza_Bruto_platina2 = null;
                    item.Peso_Real_Pieza_Neto_platina2 = null;
                    item.Scrap_Natural_platina2 = null;
                    item.Peso_neto_SAP_platina2 = null;
                    item.Peso_Bruto_SAP_platina2 = null;
                    item.Balance_de_Scrap_platina2 = null;
                    item.Peso_de_rollo_usado_real__Kg_platina2 = null;
                    item.Peso_bruto_Total_piezas_Kg_platina2 = null;
                    item.Peso_NetoTotal_piezas_Kg_platina2 = null;
                    item.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg_platina2 = null;
                    item.Peso_Neto_total_piezas_de_ajuste_Kgs_platina2 = null;
                    item.Peso_puntas_y_colas_reales_Kg_platina2 = null;
                    item.Balance_de_Scrap_Real_platina2 = null;
                }

                dt.Rows.Add(item.Planta, item.Linea, item.Operador, item.Supervisor, item.Fecha, String.Format("{0:T}", item.Hora), item.Turno, item.Orden_SAP, item.SAP_Platina,
                    item.Tipo_de_Material, item.Número_de_Parte__de_cliente, item.Material, item.Orden_en_SAP_2, item.SAP_Platina_2, item.Tipo_de_Material_platina2, item.Número_de_Parte_de_Cliente_platina2,
                    item.Material_platina2, item.SAP_Rollo, item.N__de_Rollo, item.Lote_de_rollo, item.Peso_Etiqueta__Kg_, item.Peso_de_regreso_de_rollo_Real,
                    item.Peso_de_rollo_usado, item.Peso_Báscula_Kgs, item.Pieza_por_Golpe, item.Ordenes_por_pieza, null, null, null, null, item.Total_de_piezas_platina1, item.Total_de_piezas_platina2, item.Total_de_piezas,
                    item.Peso_de_rollo_consumido, item.Numero_de_golpes, item.Kg_restante_de_rollo, item.Peso_despunte_kgs_, item.Peso_cola_Kgs_, item.Porcentaje_de_puntas_y_colas,
                    item.Total_de_piezas_de_Ajustes_platina1, item.Total_de_piezas_de_Ajustes_platina2, item.Total_de_piezas_de_Ajustes,
                    item.Peso_Bruto_Kgs, item.Peso_Real_Pieza_Bruto, item.Peso_Real_Pieza_Neto, item.Scrap_Natural, item.Peso_neto_SAP, item.Peso_Bruto_SAP, item.Balance_de_Scrap,
                    item.Peso_de_rollo_usado_real__Kg, item.Peso_bruto_Total_piezas_Kg, item.Peso_NetoTotal_piezas_Kg, item.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg,
                    item.Peso_Neto_total_piezas_de_ajuste_Kgs, item.Peso_puntas_y_colas_reales_Kg, item.Balance_de_Scrap_Real,
                    item.Peso_Bruto_Kgs_platina2, item.Peso_Real_Pieza_Bruto_platina2, item.Peso_Real_Pieza_Neto_platina2, item.Scrap_Natural_platina2, item.Peso_neto_SAP_platina2, item.Peso_Bruto_SAP_platina2, item.Balance_de_Scrap_platina2,
                    item.Peso_de_rollo_usado_real__Kg_platina2, item.Peso_bruto_Total_piezas_Kg_platina2, item.Peso_NetoTotal_piezas_Kg_platina2, item.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg_platina2,
                    item.Peso_Neto_total_piezas_de_ajuste_Kgs_platina2, item.Peso_puntas_y_colas_reales_Kg_platina2, item.Balance_de_Scrap_Real_platina2,
                    item.Peso_Bruto_Kgs_general, item.Peso_Real_Pieza_Bruto_general, item.Peso_Real_Pieza_Neto_general, item.Scrap_Natural_general, item.Peso_neto_SAP_general, item.Peso_Bruto_SAP_general, item.Balance_de_Scrap_general,
                    item.Peso_de_rollo_usado_real__Kg_general, item.Peso_bruto_Total_piezas_Kg_general, item.Peso_NetoTotal_piezas_Kg_general, item.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg_general,
                    item.Peso_Neto_total_piezas_de_ajuste_Kgs_general, item.Peso_puntas_y_colas_reales_Kg_general, item.Balance_de_Scrap_Real_general,
                    item.comentario
                 );

                filasEncabezados.Add(true);

                //verifica es una fila temporal
                if (item.SAP_Platina.ToUpper().Contains("TEMPORAL") || item.SAP_Rollo.ToUpper().Contains("TEMPORAL"))
                    filasTemporales.Add(true);
                else
                    filasTemporales.Add(false);

                produccion_registros p = null;
                //busca si tiene registro en el nuevo sistema
                if (item.IdRegistro.HasValue)
                    p = db.produccion_registros.Find(item.IdRegistro.Value);

                //obtiene la cantidad de fila actual
                int fila_inicial = filasEncabezados.Count + 1;

                //si tiene registro, agrega los lotes
                if (p != null)
                {
                    foreach (produccion_lotes lote in p.produccion_lotes.Where(x => (x.sap_platina == item.SAP_Platina || x.sap_platina == item.SAP_Platina_2 || string.IsNullOrEmpty(x.sap_platina))))
                    {


                        System.Data.DataRow row1 = dt.NewRow();

                        if (!String.IsNullOrEmpty(lote.sap_platina))
                            row1["Lote_Material"] = lote.sap_platina;
                        else
                            row1["Lote_Material"] = DBNull.Value;

                        if (lote.numero_lote_izquierdo.HasValue)
                            row1["No_lote_izq"] = lote.numero_lote_izquierdo.Value;
                        else
                            row1["No_lote_izq"] = DBNull.Value;

                        if (lote.numero_lote_derecho.HasValue)
                            row1["No_lote_der"] = lote.numero_lote_derecho.Value;
                        else
                            row1["No_lote_der"] = DBNull.Value;

                        if (lote.piezas_paquete.HasValue)
                            row1["Lote_piezas_por_paquete"] = lote.piezas_paquete.Value;
                        else
                            row1["Lote_piezas_por_paquete"] = DBNull.Value;


                        dt.Rows.Add(row1);

                        filasEncabezados.Add(false);
                        filasTemporales.Add(false);
                    }
                }
                //obtiene la fila final
                int fila_final = filasEncabezados.Count + 1;

                //verifica si hubo cambios
                if (fila_inicial != fila_final)
                {
                    oSLDocument.GroupRows(fila_inicial, fila_final - 1);
                }
            }

            double sumaRolloUsado = listadoHistorico.Sum(item => item.Peso_de_rollo_usado.HasValue ? item.Peso_de_rollo_usado.Value : 0);
            double sumaNumGolpes = listadoHistorico.Sum(item => item.Numero_de_golpes.HasValue ? item.Numero_de_golpes.Value : 0);
            //double promedioBalanceScrap = listado.Average(item => item.Balance_de_Scrap.HasValue ? item.Balance_de_Scrap.Value : 0);
            //double promedioBalanceScrapReal = listado.Average(item => item.Balance_de_Scrap_Real.HasValue ? item.Balance_de_Scrap_Real.Value : 0);
            //double promedioBalanceScrapPlatina2 = listado.Average(item => item.Balance_de_Scrap_platina2.HasValue ? item.Balance_de_Scrap_platina2.Value : 0);
            //double promedioBalanceScrapRealPlatina2 = listado.Average(item => item.Balance_de_Scrap_Real_platina2.HasValue ? item.Balance_de_Scrap_Real_platina2.Value : 0);
            double promedioBalanceScrapGeneral = listadoHistorico.Average(item => item.Balance_de_Scrap_general.HasValue ? item.Balance_de_Scrap_general.Value : 0);
            double promedioBalanceScrapRealGeneral = listadoHistorico.Average(item => item.Balance_de_Scrap_Real_general.HasValue ? item.Balance_de_Scrap_Real_general.Value : 0);

            //fila para sumatorias
            System.Data.DataRow row = dt.NewRow();
            row[nameof(view_historico_resultado.Peso_de_rollo_usado)] = sumaRolloUsado;
            row[nameof(view_historico_resultado.Numero_de_golpes)] = sumaNumGolpes;
            //row[nameof(view_historico_resultado.Balance_de_Scrap)] = promedioBalanceScrap;
            //row[nameof(view_historico_resultado.Balance_de_Scrap_Real)] = promedioBalanceScrapReal;
            //row[nameof(view_historico_resultado.Balance_de_Scrap_platina2)] = promedioBalanceScrapPlatina2;
            //row[nameof(view_historico_resultado.Balance_de_Scrap_Real_platina2)] = promedioBalanceScrapRealPlatina2;
            row[nameof(view_historico_resultado.Balance_de_Scrap_general)] = promedioBalanceScrapGeneral;
            row[nameof(view_historico_resultado.Balance_de_Scrap_Real_general)] = promedioBalanceScrapRealGeneral;
            dt.Rows.Add(row);

            oSLDocument.ImportDataTable(4, 1, dt, false); //fase omite el encabezado

            //estilo para el encabezado de cada fila
            SLStyle styleHeaderRow = oSLDocument.CreateStyle();
            styleHeaderRow.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#daeef3"), System.Drawing.ColorTranslator.FromHtml("#daeef3"));

            //estilo para el encabezado de cada fila
            SLStyle styleHeaderRowTemporal = oSLDocument.CreateStyle();
            styleHeaderRowTemporal.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffa0a2"), System.Drawing.ColorTranslator.FromHtml("#ffa0a2"));


            //estilo para cada lote
            SLStyle styleLoteInfo = oSLDocument.CreateStyle();
            styleLoteInfo.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffffcc"), System.Drawing.ColorTranslator.FromHtml("#ffffcc"));
            styleLoteInfo.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
            styleLoteInfo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleLoteInfo.Border.RightBorder.Color = System.Drawing.Color.LightGray;



            SLStyle styleFooter = oSLDocument.CreateStyle();
            styleFooter.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#c6efce"), System.Drawing.ColorTranslator.FromHtml("#c6efce"));
            styleFooter.Font.Bold = true;
            styleFooter.Font.FontColor = System.Drawing.ColorTranslator.FromHtml("#006100");
            oSLDocument.SetCellStyle(filasEncabezados.Count + 1, 1, filasEncabezados.Count, dt.Columns.Count, styleFooter);

            //aplica formato a las filas de encabezado
            for (int i = 0; i < filasEncabezados.Count; i++)
            {
                if (filasEncabezados[i])
                {
                    oSLDocument.SetCellStyle(i + 1, 1, i + 1, dt.Columns.Count, styleHeaderRow);
                }
                else if (i >= 3)
                {
                    oSLDocument.SetCellStyle(i + 1, 27, i + 1, 30, styleLoteInfo);
                }
                //colapsa todas las filas
                oSLDocument.CollapseRows(i + 3);
            }

            //Aplica formato a los temporales
            for (int i = 0; i < filasTemporales.Count; i++)
            {
                if (filasTemporales[i])
                {
                    oSLDocument.SetCellStyle(i + 1, 1, i + 1, dt.Columns.Count, styleHeaderRowTemporal);
                }

            }

            ////inmoviliza el encabezado
            oSLDocument.FreezePanes(3, 0);
            oSLDocument.SetRowHeight(4, filasEncabezados.Count + 1, 15.0);

            //inserta una celda al inicio                    
            oSLDocument.AutoFitColumn(1, 17);
            oSLDocument.SetColumnWidth(85, 50);

            oSLDocument.SelectWorksheet(hojaReportePorDia);

            #endregion


            //genera el archivo xlsx
            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }


        public static byte[] GeneraReporteEmpleados(List<empleados> listado)
        {
            // Usamos 'using' para crear el documento desde cero y asegurar la liberación de recursos.
            using (SLDocument oSLDocument = new SLDocument())
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                List<int> disabledItems = new List<int>();

                // --- Definición de columnas (sin cambios) ---
                dt.Columns.Add("8ID", typeof(string));
                dt.Columns.Add("Numero Empleado", typeof(string));
                dt.Columns.Add("Nombre", typeof(string));
                dt.Columns.Add("Fecha Nacimiento", typeof(DateTime));
                int tkbirthColumn = dt.Columns.Count;
                dt.Columns.Add("Sexo", typeof(string));
                dt.Columns.Add("Fecha Ingreso", typeof(DateTime));
                int fechaIngresoColumn = dt.Columns.Count;
                dt.Columns.Add("Correo", typeof(string));
                dt.Columns.Add("Planta", typeof(string));
                dt.Columns.Add("Departamento", typeof(string));
                dt.Columns.Add("Puesto", typeof(string));
                dt.Columns.Add("Jefe Directo", typeof(string));
                dt.Columns.Add("Baja", typeof(string));

                // --- Llenado de registros , rows ---
                foreach (var item in listado)
                {
                    System.Data.DataRow row = dt.NewRow();

                    // Los datos se asignan directamente, sin pasar por la función de sanitización.
                    row["8ID"] = item.C8ID;
                    row["Numero Empleado"] = item.numeroEmpleado;
                    row["Nombre"] = item.ConcatNombre;

                    if (item.nueva_fecha_nacimiento.HasValue)
                        row["Fecha Nacimiento"] = item.nueva_fecha_nacimiento.Value;
                    else
                        row["Fecha Nacimiento"] = DBNull.Value;

                    row["Sexo"] = item.sexo != null ? (item.sexo.ToString() == "F" ? "M" : "H") : string.Empty;

                    if (item.ingresoFecha.HasValue)
                        row["Fecha Ingreso"] = item.ingresoFecha.Value;
                    else
                        row["Fecha Ingreso"] = DBNull.Value;

                    row["Correo"] = item.correo != null ? item.correo : string.Empty;
                    row["Planta"] = item.plantas != null ? item.plantas.descripcion : string.Empty;
                    row["Departamento"] = item.Area != null ? item.Area.descripcion : string.Empty;
                    row["Puesto"] = item.puesto1 != null ? item.puesto1.descripcion : string.Empty;
                    row["Jefe Directo"] = item.empleados2 != null ? item.empleados2.ConcatNombre : string.Empty;
                    row["Baja"] = item.activo.HasValue && item.activo.Value ? "NO" : "SI";

                    dt.Rows.Add(row);

                    if (item.activo.HasValue && !item.activo.Value)
                        disabledItems.Add(dt.Rows.Count + 1);
                }

                // --- Operaciones con la hoja de Excel ---
                oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Reporte Empleados");
                oSLDocument.ImportDataTable(1, 1, dt, true);

                // --- Creación y aplicación de estilos ---
                SLStyle styleWrap = oSLDocument.CreateStyle();
                styleWrap.SetWrapText(true);

                SLStyle styleHeader = oSLDocument.CreateStyle();
                styleHeader.Font.Bold = true;
                styleHeader.Font.FontName = "Calibri";
                styleHeader.Font.FontSize = 11;
                styleHeader.Font.FontColor = System.Drawing.Color.White;
                styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.Color.White);

                SLStyle styleHeaderRowBaja = oSLDocument.CreateStyle();
                styleHeaderRowBaja.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffa0a2"), System.Drawing.Color.White);

                SLStyle styleShortDate = oSLDocument.CreateStyle();
                styleShortDate.FormatCode = "yyyy/MM/dd";

                oSLDocument.SetColumnStyle(tkbirthColumn, styleShortDate);
                oSLDocument.SetColumnStyle(fechaIngresoColumn, styleShortDate);
                oSLDocument.SetRowStyle(1, styleHeader);
                oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);

                foreach (var bajaRowIndex in disabledItems)
                {
                    oSLDocument.SetRowStyle(bajaRowIndex, styleHeaderRowBaja);
                }

                // Corrección de FreezePanes y Filter.
                oSLDocument.FreezePanes(2, 1);
                oSLDocument.Filter(1, 1, dt.Rows.Count + 1, dt.Columns.Count);

                oSLDocument.AutoFitColumn(1, dt.Columns.Count);

                // --- Guardado y retorno del archivo ---
                using (MemoryStream stream = new MemoryStream())
                {
                    oSLDocument.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public static byte[] GeneraReporteConteoInventario(List<CI_conteo_inventario> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");
            System.Data.DataTable dt = new System.Data.DataTable();


            //columnas          
            //columnas          
            dt.Columns.Add("Plant", typeof(string));                   //1
            dt.Columns.Add("Storage Loc.", typeof(string));                 //2
            dt.Columns.Add("Storage Bin.", typeof(string));                //3
            dt.Columns.Add("Batch", typeof(string));                //4
            dt.Columns.Add("Material", typeof(string));                //5
            dt.Columns.Add("Base Unit", typeof(string));        //6
            dt.Columns.Add("Ship to", typeof(string));             //7
            dt.Columns.Add("Material description", typeof(string));      //8           
            dt.Columns.Add("Tarima", typeof(string));      //8           
            dt.Columns.Add("Multiple", typeof(string));      //8           
            dt.Columns.Add("pieces", typeof(int));       //9
            dt.Columns.Add("Unrestricted", typeof(int));             //10
            dt.Columns.Add("Blocked", typeof(int));     //11         
            dt.Columns.Add("In Quality", typeof(int));                 //12         
            dt.Columns.Add("Gauge", typeof(double));         //13
            dt.Columns.Add("Gauge MIN", typeof(double));          //14
            dt.Columns.Add("Gauge MAX", typeof(double));             //15
            dt.Columns.Add("Altura", typeof(double));             //15
            dt.Columns.Add("Espesor", typeof(double));             //15
            dt.Columns.Add("Ubicación Física", typeof(string));             //15
            dt.Columns.Add("Cantidad Teórica", typeof(double));             //15
            dt.Columns.Add("Total pzas MIN", typeof(double));             //15
            dt.Columns.Add("Total pzas MAX", typeof(double));             //15
            dt.Columns.Add("Diferencia SAP", typeof(double));             //15
            dt.Columns.Add("Validación", typeof(string));             //15
            dt.Columns.Add("Capturista", typeof(string));
            dt.Columns.Add("Comentario", typeof(string));

            ////registros , rows
            foreach (CI_conteo_inventario item in listado)
            {
                bool multiple = listado.Count(x => item.num_tarima != null && x.num_tarima == item.num_tarima) > 1;
                double? total_pzas_min = null, total_pzas_max = null;

                if (multiple)
                {
                    if (item.cantidad_teorica.HasValue && item.cantidad_teorica > 0)
                    {
                        total_pzas_min = listado.Where(x => x.num_tarima == item.num_tarima).Sum(x => x.total_piezas_min);
                        total_pzas_max = listado.Where(x => x.num_tarima == item.num_tarima).Sum(x => x.total_piezas_max);
                    }
                }
                else
                {
                    total_pzas_min = item.total_piezas_min;
                    total_pzas_max = item.total_piezas_max;
                }


                dt.Rows.Add(item.plant, item.storage_location, item.storage_bin, item.batch, item.material, item.base_unit_measure, item.ship_to_number, item.material_description, item.num_tarima == null ? string.Empty : "T" + item.num_tarima.Value.ToString("D04"),
                    multiple ? "Sí" : "No", item.pieces,
                    item.unrestricted, item.blocked, item.in_quality, item.gauge, item.gauge_min, item.gauge_max, item.altura, item.espesor, item.ubicacion_fisica, item.cantidad_teorica,
                    total_pzas_min, total_pzas_max,
                    item.diferencia_sap, item.validacion,
                    item.empleados != null ? item.empleados.ConcatNombre : string.Empty,
                    item.comentario
                    );
            }


            //crea la hoja de FACTURAS y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Reporte Empleados");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            //estilo para bajas
            SLStyle styleHeaderRowBaja = oSLDocument.CreateStyle();
            styleHeaderRowBaja.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffa0a2"), System.Drawing.ColorTranslator.FromHtml("#ffa0a2"));

            //estilo para numeros
            SLStyle styleNumber = oSLDocument.CreateStyle();
            styleNumber.FormatCode = "#,##0.00";

            //da estilo a los numero
            //oSLDocument.SetColumnStyle(18, styleNumber);

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";



            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetRowHeight(1, dt.Rows.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteEmpleadostkmmnet(List<empleados> listado)
        {
            // Usamos 'using' para asegurar que los recursos se liberen correctamente.
            using (SLDocument oSLDocument = new SLDocument())
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                List<int> disabledItems = new List<int>();

                // --- Definición de columnas (tu código original, sin cambios) ---
                dt.Columns.Add("userName", typeof(string));
                dt.Columns.Add("tksir", typeof(string));
                dt.Columns.Add("tktitle", typeof(string));
                dt.Columns.Add("tknameprefix", typeof(string));
                dt.Columns.Add("lastName", typeof(string));
                dt.Columns.Add("firstName", typeof(string));
                dt.Columns.Add("tkbirth", typeof(DateTime));
                int tkbirthColumn = dt.Columns.Count; // Se mantiene para el formato de fecha
                dt.Columns.Add("tksex", typeof(string));
                dt.Columns.Add("tkstreet", typeof(string));
                dt.Columns.Add("tkpostalcode", typeof(string));
                dt.Columns.Add("tkpostaladdress", typeof(string));
                dt.Columns.Add("tkaddaddon", typeof(string));
                dt.Columns.Add("tkfedst", typeof(string));
                dt.Columns.Add("tkcountry", typeof(string));
                dt.Columns.Add("tkcountrykey", typeof(string));
                dt.Columns.Add("tknationality", typeof(string));
                dt.Columns.Add("tkpreflang", typeof(string));
                dt.Columns.Add("tkempno", typeof(string));
                dt.Columns.Add("tkfkz6", typeof(string));
                dt.Columns.Add("tkfkzext", typeof(string));
                dt.Columns.Add("tkuniqueid", typeof(string));
                dt.Columns.Add("tkpstatus", typeof(string));
                dt.Columns.Add("tkcostcenter", typeof(string));
                dt.Columns.Add("tkdepartment", typeof(string));
                dt.Columns.Add("tkfunction", typeof(string));
                dt.Columns.Add("tkorgstreet", typeof(string));
                dt.Columns.Add("tkorgpostalcode", typeof(string));
                dt.Columns.Add("tkorgpostaladdress", typeof(string));
                dt.Columns.Add("tkorgaddonaddr", typeof(string));
                dt.Columns.Add("tkorgfedst", typeof(string));
                dt.Columns.Add("tkorgcountry", typeof(string));
                dt.Columns.Add("tkorgcountrykey", typeof(string));
                dt.Columns.Add("tkapsite", typeof(string));
                dt.Columns.Add("tkorgkey", typeof(string));
                dt.Columns.Add("tkbuilding", typeof(string));
                dt.Columns.Add("email", typeof(string));
                dt.Columns.Add("tkareacode", typeof(string));
                dt.Columns.Add("tkphoneext", typeof(string));
                dt.Columns.Add("tkorgfax", typeof(string));
                dt.Columns.Add("tkmobile", typeof(string));
                dt.Columns.Add("tkgodfather", typeof(string));
                dt.Columns.Add("tkprefdelmethod", typeof(string));
                dt.Columns.Add("tkedateorg", typeof(string));
                dt.Columns.Add("tkedatetrust", typeof(DateTime));
                int tkdateTrustColumn = dt.Columns.Count; // Se mantiene para el formato de fecha
                dt.Columns.Add("tkldateorg", typeof(string));
                dt.Columns.Add("tklreason", typeof(string));
                dt.Columns.Add("shares", typeof(string));
                dt.Columns.Add("supervisoryboardelection", typeof(string));
                dt.Columns.Add("tkbkz", typeof(string));
                dt.Columns.Add("tkinside", typeof(string));
                dt.Columns.Add("Baja", typeof(string));

                // --- Llenado de registros, rows (tu código original, sin cambios) ---
                foreach (var item in listado)
                {
                    System.Data.DataRow row = dt.NewRow();

                    row["userName"] = item.C8ID;
                    row["tksir"] = string.Empty;
                    row["tktitle"] = string.Empty;
                    row["tknameprefix"] = string.Empty;
                    row["lastName"] = string.Format("{0} {1}", item.apellido1, item.apellido2);
                    row["firstName"] = item.nombre;
                    if (item.nueva_fecha_nacimiento.HasValue)
                        row["tkbirth"] = item.nueva_fecha_nacimiento.Value;
                    else
                        row["tkbirth"] = DBNull.Value;
                    row["tksex"] = item.sexo != null ? item.sexo.ToString() : String.Empty;
                    row["tkstreet"] = string.Empty;
                    row["tkpostalcode"] = string.Empty;
                    row["tkpostaladdress"] = string.Empty;
                    row["tkaddaddon"] = string.Empty;
                    row["tkfedst"] = string.Empty;
                    row["tkcountry"] = string.Empty;
                    row["tkcountrykey"] = string.Empty;
                    row["tknationality"] = "MEX";
                    row["tkpreflang"] = "es";
                    row["tkempno"] = item.numeroEmpleado != null ? item.numeroEmpleado : String.Empty;
                    row["tkfkz6"] = "801495";
                    row["tkfkzext"] = string.Empty;
                    row["tkuniqueid"] = "801495-01";
                    row["tkpstatus"] = "40";
                    row["tkcostcenter"] = item.Area != null ? item.Area.numero_centro_costo : String.Empty;
                    row["tkdepartment"] = item.Area != null ? item.Area.descripcion : String.Empty;
                    row["tkfunction"] = item.puesto1 != null ? item.puesto1.descripcion : String.Empty;
                    row["tkorgstreet"] = item.plantas != null ? item.plantas.tkorgstreet : String.Empty;
                    row["tkorgpostalcode"] = item.plantas != null ? item.plantas.tkorgpostalcode : String.Empty;
                    row["tkorgpostaladdress"] = item.plantas != null ? item.plantas.tkorgpostaladdress : String.Empty;
                    row["tkorgaddonaddr"] = item.plantas != null ? item.plantas.tkorgaddonaddr : String.Empty;
                    row["tkorgfedst"] = item.plantas != null ? item.plantas.tkorgfedst : String.Empty;
                    row["tkorgcountry"] = item.plantas != null ? item.plantas.tkorgcountry : String.Empty;
                    row["tkorgcountrykey"] = item.plantas != null ? item.plantas.tkorgcountrykey : String.Empty;
                    row["tkapsite"] = item.plantas != null ? item.plantas.tkapsite : String.Empty;
                    row["tkorgkey"] = String.Empty;
                    row["tkbuilding"] = String.Empty;
                    row["email"] = item.correo != null ? item.correo : String.Empty;
                    row["tkareacode"] = String.Empty;
                    row["tkphoneext"] = String.Empty;
                    row["tkorgfax"] = String.Empty;
                    if (item.GetIT_Inventory_Cellular_LinesActivas().Count > 0)
                        row["tkmobile"] = item.GetIT_Inventory_Cellular_LinesActivas().First().GetPhoneNumberFormat;
                    else
                        row["tkmobile"] = String.Empty;
                    row["tkgodfather"] = item.empleados2 != null ? item.empleados2.C8ID : String.Empty;
                    row["tkprefdelmethod"] = "O";
                    if (item.ingresoFecha.HasValue)
                        row["tkedatetrust"] = item.ingresoFecha.Value;
                    else
                        row["tkedatetrust"] = DBNull.Value;
                    row["tklreason"] = string.Empty;
                    row["shares"] = "N";
                    row["supervisoryboardelection"] = "N";
                    row["tkbkz"] = string.Empty;
                    row["tkinside"] = "N";
                    row["Baja"] = item.activo.HasValue && item.activo.Value ? "NO" : "YES";

                    dt.Rows.Add(row);

                    // Se suma 2 porque: 1 por el encabezado y 1 porque dt.Rows.Count es base 0
                    if (item.activo.HasValue && !item.activo.Value)
                        disabledItems.Add(dt.Rows.Count + 1);
                }

                // Renombra la hoja de cálculo por defecto
                oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Reporte Empleados");

                // Importa los datos desde el DataTable a la hoja, empezando en la celda A1 (1,1)
                // El 'true' final indica que se deben incluir los nombres de las columnas como encabezado.
                oSLDocument.ImportDataTable(1, 1, dt, true);

                // --- Creación y aplicación de estilos (tu código original, casi sin cambios) ---
                SLStyle styleWrap = oSLDocument.CreateStyle();
                styleWrap.SetWrapText(true);

                SLStyle styleHeader = oSLDocument.CreateStyle();
                styleHeader.Font.Bold = true;
                styleHeader.Font.FontColor = System.Drawing.Color.White;
                styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.Color.White);

                SLStyle styleHeaderRowBaja = oSLDocument.CreateStyle();
                styleHeaderRowBaja.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffa0a2"), System.Drawing.Color.White);

                SLStyle styleShortDate = oSLDocument.CreateStyle();
                styleShortDate.FormatCode = "dd.MM.yyyy";

                // Aplica los estilos
                oSLDocument.SetColumnStyle(tkbirthColumn, styleShortDate);
                oSLDocument.SetColumnStyle(tkdateTrustColumn, styleShortDate);

                // Aplica el estilo de fila de baja
                foreach (var bajaRowIndex in disabledItems)
                {
                    oSLDocument.SetRowStyle(bajaRowIndex, styleHeaderRowBaja);
                }

                // Inmoviliza el encabezado (la fila 1)
                oSLDocument.FreezePanes(2, 1);

                // Aplica un filtro automático a la tabla
                oSLDocument.Filter(1, 1, dt.Rows.Count + 1, dt.Columns.Count);

                // Autoajusta el ancho de todas las columnas utilizadas
                oSLDocument.AutoFitColumn(1, dt.Columns.Count);

                // Aplica el estilo de ajuste de texto a todas las columnas
                oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);

                // Aplica el estilo al encabezado (fila 1)
                oSLDocument.SetRowStyle(1, styleHeader);


                // Guarda el documento en un stream de memoria
                using (MemoryStream stream = new MemoryStream())
                {
                    oSLDocument.SaveAs(stream);
                    // Convierte el stream a un arreglo de bytes y lo retorna
                    return stream.ToArray();
                }
            }
        }
        public static byte[] GeneraFormatoRM(RM_cabecera remision, empleados usuario)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");
            System.Data.DataTable dt = new System.Data.DataTable();

            List<int> disabledItems = new List<int>();

            //columnas          
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("NumeroRemision", typeof(string));
            dt.Columns.Add("ItemClave", typeof(string));
            dt.Columns.Add("NumeroMaterial", typeof(string));
            dt.Columns.Add("NumeroLote", typeof(string));
            dt.Columns.Add("Cantidad", typeof(double));
            dt.Columns.Add("NumeroRollo", typeof(string));
            dt.Columns.Add("Cliente", typeof(string));
            dt.Columns.Add("EnviadoA", typeof(string));
            dt.Columns.Add("Almacen", typeof(string));
            dt.Columns.Add("PlacaTractor", typeof(string));
            dt.Columns.Add("PlacaRemolque", typeof(string));
            dt.Columns.Add("HorarioDescarga", typeof(string));
            dt.Columns.Add("NombreChofer", typeof(string));
            dt.Columns.Add("Transporte", typeof(string));
            ////registros , rows
            foreach (var item in remision.RM_elemento)
            {
                System.Data.DataRow row = dt.NewRow();

                row["Usuario"] = usuario.ConcatNombre;
                row["NumeroRemision"] = remision.ConcatNumeroRemision;
                row["ItemClave"] = item.clave;
                row["NumeroMaterial"] = item.numeroMaterial;
                row["NumeroLote"] = item.numeroLote;
                row["Cantidad"] = item.cantidad;
                row["NumeroRollo"] = item.numeroRollo;
                row["Cliente"] = remision.clienteOtro;
                row["EnviadoA"] = remision.enviadoAOtro;
                row["Almacen"] = remision.RM_almacen.descripcion;
                row["PlacaTractor"] = remision.placaTractor;
                row["PlacaRemolque"] = remision.placaRemolque;
                row["HorarioDescarga"] = remision.horarioDescarga;
                row["NombreChofer"] = remision.nombreChofer;
                row["Transporte"] = remision.transporteOtro;

                dt.Rows.Add(row);

            }

            //crea la hoja de FACTURAS y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Cierre Remisión");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            //estilo para bajas
            SLStyle styleHeaderRowBaja = oSLDocument.CreateStyle();
            styleHeaderRowBaja.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffa0a2"), System.Drawing.ColorTranslator.FromHtml("#ffa0a2"));

            //estilo para numeros
            SLStyle styleNumber = oSLDocument.CreateStyle();
            styleNumber.FormatCode = "#,##0.00";

            //da estilo a los numero
            //oSLDocument.SetColumnStyle(18, styleNumber);

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "dd.MM.yyyy";
            //oSLDocument.SetColumnStyle(tkbirthColumn, styleLongDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            foreach (var baja in disabledItems)
                oSLDocument.SetCellStyle(baja, 1, baja, dt.Columns.Count, styleHeaderRowBaja);

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetRowHeight(1, dt.Rows.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteRU(List<RU_registros> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");
            System.Data.DataTable dt = new System.Data.DataTable();

            List<int> disabledItems = new List<int>();

            //columnas          
            dt.Columns.Add("Folio", typeof(string));
            dt.Columns.Add("Planta", typeof(string));
            dt.Columns.Add("Estado", typeof(string));
            dt.Columns.Add("Línea de transporte", typeof(string));
            dt.Columns.Add("Nombre Operador", typeof(string));
            dt.Columns.Add("Placas Tractor", typeof(string));
            dt.Columns.Add("Placas Plataforma 1", typeof(string));
            dt.Columns.Add("Placas Plataforma 2", typeof(string));
            dt.Columns.Add("Carga", typeof(string));
            dt.Columns.Add("Descarga", typeof(string));
            dt.Columns.Add("Hora Ingreso", typeof(DateTime));
            dt.Columns.Add("Vigilancia (Ingreso)", typeof(string));
            dt.Columns.Add("Comentarios (Ingreso)", typeof(string));
            dt.Columns.Add("Hora Recepción", typeof(DateTime));
            dt.Columns.Add("Oficina (Recepción)", typeof(string));
            dt.Columns.Add("Comentarios (Recepción)", typeof(string));
            dt.Columns.Add("Hora Liberación (Embarques)", typeof(DateTime));
            dt.Columns.Add("Embarques (Liberación)", typeof(string));
            dt.Columns.Add("Comentarios Liberación (Embarques)", typeof(string));
            dt.Columns.Add("Hora Liberación (Vigilancia)", typeof(DateTime));
            dt.Columns.Add("Vigilancia (Liberación)", typeof(string));
            dt.Columns.Add("Comentarios Liberación (Vigilancia)", typeof(string));
            dt.Columns.Add("Hora Salida (Vigilancia)", typeof(DateTime));
            dt.Columns.Add("Vigilancia (Salida)", typeof(string));
            dt.Columns.Add("Comentarios Salida (Vigilancia)", typeof(string));
            dt.Columns.Add("Hora Cancelación", typeof(DateTime));
            dt.Columns.Add("Canceló", typeof(string));
            dt.Columns.Add("Entrada", typeof(string));
            dt.Columns.Add("Salida", typeof(string));
            dt.Columns.Add("Comentarios Cancelación", typeof(string));

            ////registros , rows
            foreach (var item in listado)
            {
                System.Data.DataRow row = dt.NewRow();

                row["Folio"] = item.id;
                row["Planta"] = item.plantas.descripcion;
                row["Estado"] = item.EstadoString;
                row["Línea de transporte"] = item.linea_transporte;
                row["Nombre Operador"] = item.nombre_operador;
                row["Placas Tractor"] = item.placas_tractor;
                row["Placas Plataforma 1"] = item.placa_plataforma_uno;
                row["Placas Plataforma 2"] = item.placa_plataforma_dos;
                row["Carga"] = item.carga ? "Sí" : "No";
                row["Descarga"] = item.descarga ? "Sí" : "No";
                row["Hora Ingreso"] = item.fecha_ingreso_vigilancia;
                row["Vigilancia (Ingreso)"] = item.RU_usuarios_vigilancia.nombre;
                row["Comentarios (Ingreso)"] = item.comentarios_vigilancia_ingreso;
                if (item.hora_embarques_recepcion.HasValue)
                    row["Hora Recepción"] = item.hora_embarques_recepcion;
                else
                    row["Hora Recepción"] = DBNull.Value;
                row["Oficina (Recepción)"] = item.empleados1 != null ? item.empleados1.ConcatNombre : string.Empty;
                row["Comentarios (Recepción)"] = item.comentarios_embarques_recepcion;
                if (item.hora_embarques_liberacion.HasValue)
                    row["Hora Liberación (Embarques)"] = item.hora_embarques_liberacion;
                else
                    row["Hora Liberación (Embarques)"] = DBNull.Value;
                row["Embarques (Liberación)"] = item.empleados != null ? item.empleados.ConcatNombre : string.Empty;
                row["Comentarios Liberación (Embarques)"] = item.comentarios_embarques_liberacion;
                if (item.hora_vigilancia_liberacion.HasValue)
                    row["Hora Liberación (Vigilancia)"] = item.hora_vigilancia_liberacion;
                else
                    row["Hora Liberación (Vigilancia)"] = DBNull.Value;
                row["Vigilancia (Liberación)"] = item.RU_usuarios_vigilancia1 != null ? item.RU_usuarios_vigilancia1.nombre : string.Empty;
                row["Comentarios Liberación (Vigilancia)"] = item.comentarios_vigilancia_liberacion;
                if (item.hora_vigilancia_salida.HasValue)
                    row["Hora Salida (Vigilancia)"] = item.hora_vigilancia_salida;
                else
                    row["Hora Salida (Vigilancia)"] = DBNull.Value;
                row["Vigilancia (Salida)"] = item.RU_usuarios_vigilancia2 != null ? item.RU_usuarios_vigilancia2.nombre : string.Empty;
                row["Comentarios Salida (Vigilancia)"] = item.comentarios_vigilancia_salida;
                if (item.hora_cancelacion.HasValue)
                    row["Hora Cancelación"] = item.hora_cancelacion;
                else
                    row["Hora Cancelación"] = DBNull.Value;
                row["Canceló"] = item.nombre_cancelacion;
                row["Entrada"] = item.RU_accesos.descripcion;
                row["Salida"] = item.RU_accesos1 != null ? item.RU_accesos1.descripcion : string.Empty;
                row["Comentarios Cancelación"] = item.comentario_cancelacion;


                dt.Rows.Add(row);

            }

            //crea la hoja de FACTURAS y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Registro de Unidades");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            //estilo para bajas
            SLStyle styleHeaderRowBaja = oSLDocument.CreateStyle();
            styleHeaderRowBaja.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffa0a2"), System.Drawing.ColorTranslator.FromHtml("#ffa0a2"));

            //estilo para numeros
            SLStyle styleNumber = oSLDocument.CreateStyle();
            styleNumber.FormatCode = "#,##0.00";

            //da estilo a los numero
            //oSLDocument.SetColumnStyle(18, styleNumber);

            ////estilo para fecha
            SLStyle styleLongDate = oSLDocument.CreateStyle();
            styleLongDate.FormatCode = "dd/MM/yyyy hh:mm AM/PM";
            oSLDocument.SetColumnStyle(dt.Columns["Hora Ingreso"].Ordinal + 1, styleLongDate);
            oSLDocument.SetColumnStyle(dt.Columns["Hora Recepción"].Ordinal + 1, styleLongDate);
            oSLDocument.SetColumnStyle(dt.Columns["Hora Liberación (Embarques)"].Ordinal + 1, styleLongDate);
            oSLDocument.SetColumnStyle(dt.Columns["Hora Liberación (Vigilancia)"].Ordinal + 1, styleLongDate);
            oSLDocument.SetColumnStyle(dt.Columns["Hora Salida (Vigilancia)"].Ordinal + 1, styleLongDate);
            oSLDocument.SetColumnStyle(dt.Columns["Hora Cancelación"].Ordinal + 1, styleLongDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            foreach (var baja in disabledItems)
                oSLDocument.SetCellStyle(baja, 1, baja, dt.Columns.Count, styleHeaderRowBaja);

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetRowHeight(1, dt.Rows.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        public static byte[] GeneraReporteRemisionesManuales(List<RM_cabecera> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");
            System.Data.DataTable dt = new System.Data.DataTable();

            List<int> disabledItems = new List<int>();

            //columnas          
            dt.Columns.Add("Núm. Remisión", typeof(string));
            dt.Columns.Add("Estatus Actual", typeof(string));
            dt.Columns.Add("Fecha Creación", typeof(DateTime));
            dt.Columns.Add("Creada Por", typeof(string));
            dt.Columns.Add("Planta", typeof(string));
            dt.Columns.Add("Almacén", typeof(string));
            dt.Columns.Add("Motivo", typeof(string));
            dt.Columns.Add("Texto Breve Motivo", typeof(string));
            dt.Columns.Add("Nombre Cliente", typeof(string));
            dt.Columns.Add("Dirección Cliente", typeof(string));
            dt.Columns.Add("Nombre Proveedor", typeof(string));
            dt.Columns.Add("Dirección Proveedor", typeof(string));
            dt.Columns.Add("Enviado A", typeof(string));
            dt.Columns.Add("Enviado A (Dirección)", typeof(string));
            dt.Columns.Add("Transporte", typeof(string));
            dt.Columns.Add("Placa Tractor", typeof(string));
            dt.Columns.Add("Placa Remolque", typeof(string));
            dt.Columns.Add("Nombre Chofer", typeof(string));
            dt.Columns.Add("Horario Descarga", typeof(string));
            dt.Columns.Add("Total Cantidad", typeof(double));
            dt.Columns.Add("Total Peso", typeof(double));


            ////registros , rows
            foreach (var item in listado)
            {
                System.Data.DataRow row = dt.NewRow();

                row["Núm. Remisión"] = item.ConcatNumeroRemision;
                row["Estatus Actual"] = item.RM_estatus != null ? item.RM_estatus.descripcion : String.Empty;
                if (item.FechaCreacion.HasValue)
                    row["Fecha Creación"] = item.FechaCreacion;
                else
                    row["Fecha Creación"] = DBNull.Value;
                row["Creada Por"] = item.CreadaPor;
                row["Planta"] = item.RM_almacen != null ? item.RM_almacen.plantas.descripcion : String.Empty;
                row["Almacén"] = item.RM_almacen != null ? item.RM_almacen.descripcion : String.Empty;
                row["Nombre Cliente"] = item.NombreCliente;
                row["Dirección Cliente"] = item.clienteOtroDireccion;
                row["Nombre Proveedor"] = item.NombreProveedor;
                row["Dirección Proveedor"] = item.proveedorOtroDireccion;
                row["Enviado A"] = item.EnviadoA;
                row["Enviado A (Dirección)"] = item.enviadoAOtroDireccion;
                row["Transporte"] = item.transporteOtro;
                row["Placa Tractor"] = item.placaTractor;
                row["Placa Remolque"] = item.placaRemolque;
                row["Nombre Chofer"] = item.nombreChofer;
                row["Horario Descarga"] = item.horarioDescarga;
                row["Total Cantidad"] = item.TotalCantidadRemision;
                row["Total Peso"] = item.TotalPesoRemision;
                row["Motivo"] = item.RM_remision_motivo != null ? item.RM_remision_motivo.descripcion.Trim() : String.Empty;
                row["Texto Breve Motivo"] = item.motivoTexto;

                dt.Rows.Add(row);

            }

            //crea la hoja de FACTURAS y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Registro de Unidades");
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            //estilo para bajas
            SLStyle styleHeaderRowBaja = oSLDocument.CreateStyle();
            styleHeaderRowBaja.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffa0a2"), System.Drawing.ColorTranslator.FromHtml("#ffa0a2"));

            //estilo para numeros
            SLStyle styleNumber = oSLDocument.CreateStyle();
            styleNumber.FormatCode = "#,##0.000";

            //da estilo a los numero
            oSLDocument.SetColumnStyle(dt.Columns["Total Peso"].Ordinal + 1, styleNumber);
            oSLDocument.SetColumnStyle(dt.Columns["Total Cantidad"].Ordinal + 1, styleNumber);

            ////estilo para fecha
            SLStyle styleLongDate = oSLDocument.CreateStyle();
            styleLongDate.FormatCode = "dd/MM/yyyy hh:mm AM/PM";
            oSLDocument.SetColumnStyle(dt.Columns["Fecha Creación"].Ordinal + 1, styleLongDate);
            //oSLDocument.SetColumnStyle(dt.Columns["Hora Recepción"].Ordinal + 1, styleLongDate);
            //oSLDocument.SetColumnStyle(dt.Columns["Hora Liberación (Embarques)"].Ordinal + 1, styleLongDate);
            //oSLDocument.SetColumnStyle(dt.Columns["Hora Liberación (Vigilancia)"].Ordinal + 1, styleLongDate);
            //oSLDocument.SetColumnStyle(dt.Columns["Hora Salida (Vigilancia)"].Ordinal + 1, styleLongDate);
            //oSLDocument.SetColumnStyle(dt.Columns["Hora Cancelación"].Ordinal + 1, styleLongDate);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            foreach (var baja in disabledItems)
                oSLDocument.SetCellStyle(baja, 1, baja, dt.Columns.Count, styleHeaderRowBaja);

            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);
            oSLDocument.SetRowHeight(1, dt.Rows.Count + 1, 15.0);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        /// <summary>
        /// Genera reporte de IT_inventory_lines
        /// </summary>
        /// <param name="listado"></param>
        /// <returns></returns>
        public static byte[] SCDM_GeneraArchivoFacturacion(int? id_solicitud, List<SCDM_solicitud_rel_facturacion> listado)
        {

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/SCDM_Facturacion.xlsm"), "Facturacion");
            System.Data.DataTable dt = new System.Data.DataTable();

            //establece el número de solicitud
            oSLDocument.SetCellValue(1, 5, id_solicitud.ToString());

            int inicio_fila = 4;
            foreach (var item in listado)
            {
                //numero de material
                oSLDocument.SetCellValue(inicio_fila, 1, item.SCDM_solicitud_rel_item_material != null ? item.SCDM_solicitud_rel_item_material.numero_material :
                        item.SCDM_solicitud_rel_creacion_referencia != null ? item.SCDM_solicitud_rel_creacion_referencia.nuevo_material : string.Empty);
                //planta
                oSLDocument.SetCellValue(inicio_fila, 2, item.SCDM_solicitud_rel_item_material != null ? item.SCDM_solicitud_rel_item_material.SCDM_solicitud.SCDM_rel_solicitud_plantas.FirstOrDefault().plantas.codigoSap :
                        item.SCDM_solicitud_rel_creacion_referencia != null ? item.SCDM_solicitud_rel_creacion_referencia.plantas.codigoSap : string.Empty);

                //unidad medida
                oSLDocument.SetCellValue(inicio_fila, 3, item.unidad_medida);
                //clave producto
                oSLDocument.SetCellValue(inicio_fila, 4, item.clave_producto_servicio);
                //cliente
                oSLDocument.SetCellValue(inicio_fila, 5, item.cliente);
                //descripcion
                oSLDocument.SetCellValue(inicio_fila, 6, item.descripcion_en);
                //cfdi 01
                oSLDocument.SetCellValue(inicio_fila, 7, item.uso_CFDI_01.HasValue && item.uso_CFDI_01.Value ? "X" : string.Empty);
                //cfdi 02
                oSLDocument.SetCellValue(inicio_fila, 8, item.uso_CFDI_02.HasValue && item.uso_CFDI_02.Value ? "X" : string.Empty);
                //cfdi 03
                oSLDocument.SetCellValue(inicio_fila, 9, item.uso_CFDI_03.HasValue && item.uso_CFDI_03.Value ? "X" : string.Empty);
                //cfdi 04
                oSLDocument.SetCellValue(inicio_fila, 10, item.uso_CFDI_04.HasValue && item.uso_CFDI_04.Value ? "X" : string.Empty);
                //cfdi 05
                oSLDocument.SetCellValue(inicio_fila, 11, item.uso_CFDI_05.HasValue && item.uso_CFDI_05.Value ? "X" : string.Empty);
                //cfdi 06
                oSLDocument.SetCellValue(inicio_fila, 12, item.uso_CFDI_06.HasValue && item.uso_CFDI_06.Value ? "X" : string.Empty);
                //cfdi 07
                oSLDocument.SetCellValue(inicio_fila, 13, item.uso_CFDI_07.HasValue && item.uso_CFDI_07.Value ? "X" : string.Empty);
                //cfdi 08
                oSLDocument.SetCellValue(inicio_fila, 14, item.uso_CFDI_08.HasValue && item.uso_CFDI_08.Value ? "X" : string.Empty);
                //cfdi 09
                oSLDocument.SetCellValue(inicio_fila, 15, item.uso_CFDI_09.HasValue && item.uso_CFDI_09.Value ? "X" : string.Empty);
                //cfdi 10
                oSLDocument.SetCellValue(inicio_fila, 16, item.uso_CFDI_10.HasValue && item.uso_CFDI_10.Value ? "X" : string.Empty);
                //ejecucion correcta
                oSLDocument.SetCellValue(inicio_fila, 17, item.ejecucion_correcta);
                //mensaje sap
                oSLDocument.SetCellValue(inicio_fila, 18, item.mensaje_sap);

                inicio_fila++;
            }


            //genera el archivo excel
            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }

        /// <summary>
        /// Genera el reporte del IHS
        /// </summary>
        /// <param name="listado"></param>
        /// <returns></returns>
        /// <summary>
        /// Genera el reporte del IHS
        /// </summary>
        /// <param name="listado"></param>
        /// <returns></returns>
        public static byte[] GeneraReporteBudgetIHS(
             List<BG_IHS_item> listado,
             List<BG_IHS_combinacion> combinaciones,
             List<BG_IHS_division> divisiones,
             string demanda,
             List<BG_IHS_rel_demanda> todaLaDemanda,
            List<BG_IHS_rel_cuartos> todosLosCuartos,
            List<BG_IHS_rel_regiones> todasLasRelacionesDeRegion
         )
        {
            // <<-- TEMPORIZADOR PRINCIPAL -->>
            var stopwatch = new Stopwatch();
            Debug.WriteLine("*************************************************");
            Debug.WriteLine($"****** INICIANDO GENERACIÓN DE REPORTE IHS ( {DateTime.Now} ) ******");
            Debug.WriteLine($"****** Registros a procesar: {listado.Count} ******");
            Debug.WriteLine("*************************************************");

            // <<-- TEMPORIZADOR INICIA: Bloque 1 -->>
            Debug.WriteLine("--- INICIO: Bloque 1: Inicialización, estilos y preparación de cabeceras ---");
            stopwatch.Start();

            var cabeceraMeses = Portal_2_0.Models.BG_IHS_UTIL.GetCabecera();
            var cabeceraCuartos = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraCuartos();
            var cabeceraAnios = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAnios();
            var cabeceraAniosFY = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAniosFY();

            string hoja1 = "Autos normal";
            string hoja2 = "Regiones";
            string hoja3 = "Autos Modificados";

            int FYReference = 0;

            //para regiones
            List<BG_IHS_item_anios> listDatosRegionesFY = new List<BG_IHS_item_anios>();

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/Reporte_IHS.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();

            //estilos para celdas
            SLStyle styleIHS = oSLDocument.CreateStyle();
            styleIHS.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#faebd7"), System.Drawing.ColorTranslator.FromHtml("#faebd7"));

            SLStyle styleUser = oSLDocument.CreateStyle();
            styleUser.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#d6c6ea"), System.Drawing.ColorTranslator.FromHtml("#d6c6ea"));

            SLStyle styleDemandaOriginal = oSLDocument.CreateStyle();
            styleDemandaOriginal.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#faff6b"), System.Drawing.ColorTranslator.FromHtml("#faff6b"));

            SLStyle styleDemandaCustomer = oSLDocument.CreateStyle();
            styleDemandaCustomer.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#6afcf3"), System.Drawing.ColorTranslator.FromHtml("#6afcf3"));

            SLStyle styleValorCalculado = oSLDocument.CreateStyle();
            styleValorCalculado.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#bbf3c1"), System.Drawing.ColorTranslator.FromHtml("#bbf3c1"));

            SLStyle styleValorIHS = oSLDocument.CreateStyle();
            styleValorIHS.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffb6c1"), System.Drawing.ColorTranslator.FromHtml("#ffb6c1"));


            SLStyle styleTituloCombinacion = oSLDocument.CreateStyle();
            styleTituloCombinacion.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#FFCC99"), System.Drawing.ColorTranslator.FromHtml("#FFCC99"));
            styleTituloCombinacion.Font.FontColor = System.Drawing.Color.DarkBlue;
            styleTituloCombinacion.Font.Bold = true;

            SLStyle styleBoldBlue = oSLDocument.CreateStyle();
            styleBoldBlue.Font.FontColor = System.Drawing.Color.DarkBlue;
            styleBoldBlue.Font.Bold = true;

            SLStyle styleDivisionesData = oSLDocument.CreateStyle();
            styleDivisionesData.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#D5E8DB"), System.Drawing.ColorTranslator.FromHtml("#D5E8DB"));


            //columnas          
            #region cabecera general
            dt.Columns.Add("Id", typeof(string));                       //1
            dt.Columns.Add("Origen", typeof(string));                   //1
            dt.Columns.Add("Vehicle (IHS)", typeof(string));                   //1
            dt.Columns.Add("Vehicle (Compuesto)", typeof(string));                   //1
            dt.Columns.Add("Core Nameplate Region Mnemonic", typeof(string));                   //1
            dt.Columns.Add("Core Nameplate Plant Mnemonic", typeof(string));                   //1
            dt.Columns.Add("Mnemonic-Vehicle", typeof(string));                   //1
            dt.Columns.Add("Mnemonic-Vehicle/Plant", typeof(string));                   //1
            dt.Columns.Add("Mnemonic-Platform", typeof(string));                   //1
            dt.Columns.Add("Mnemonic-Plant", typeof(string));                   //1
            dt.Columns.Add("Region", typeof(string));                   //1
            dt.Columns.Add("Market", typeof(string));                   //1
            dt.Columns.Add("Country/Territory", typeof(string));                   //1
            dt.Columns.Add("Production Plant", typeof(string));                   //1
            dt.Columns.Add("Region(Plant)", typeof(string));                   //1
            dt.Columns.Add("City", typeof(string));                   //1
            dt.Columns.Add("Plant State/Province", typeof(string));                   //1
            dt.Columns.Add("Source Plant", typeof(string));                   //1
            dt.Columns.Add("Source Plant Country/Territory", typeof(string));                   //1
            dt.Columns.Add("Source Plant Region", typeof(string));                   //1
            dt.Columns.Add("Design Parent", typeof(string));                   //1
            dt.Columns.Add("Engineering Group", typeof(string));                   //1
            dt.Columns.Add("Manufacturer Group", typeof(string));                   //1
            dt.Columns.Add("Manufacturer", typeof(string));                   //1
            dt.Columns.Add("Sales Parent", typeof(string));                   //1
            dt.Columns.Add("Production Brand", typeof(string));                   //1
            dt.Columns.Add("Platform Design Owner", typeof(string));                   //1
            dt.Columns.Add("Architecture", typeof(string));                   //1
            dt.Columns.Add("Platform", typeof(string));                   //1
            dt.Columns.Add("Program", typeof(string));                   //1
            dt.Columns.Add("Production Nameplate", typeof(string));                   //1
            dt.Columns.Add("SOP (Start of Production)", typeof(DateTime));                   //1
            dt.Columns.Add("EOP (End of Production)", typeof(DateTime));                   //1
            dt.Columns.Add("Lifecycle (Time)", typeof(string));                   //1
            dt.Columns.Add("Assembly Type", typeof(string));                   //1
            dt.Columns.Add("Strategic Group", typeof(string));                   //1
            dt.Columns.Add("Sales Group", typeof(string));                   //1
            dt.Columns.Add("Global Nameplate", typeof(string));                   //1
            dt.Columns.Add("Primary Design Center", typeof(string));                   //1
            dt.Columns.Add("Primary Design Country/Territory", typeof(string));                   //1
            dt.Columns.Add("Primary Design Region", typeof(string));                   //1
            dt.Columns.Add("Secondary Design Center", typeof(string));                   //1
            dt.Columns.Add("Secondary Design Country/Territory	", typeof(string));                   //1
            dt.Columns.Add("Secondary Design Region", typeof(string));                   //1
            dt.Columns.Add("GVW Rating", typeof(string));                   //1
            dt.Columns.Add("GVW Class", typeof(string));                   //1
            dt.Columns.Add("Car/Truck", typeof(string));                   //1
            dt.Columns.Add("Production Type", typeof(string));                   //1
            dt.Columns.Add("Global Production Segment", typeof(string));                   //1
            dt.Columns.Add("Flat Rolled Steel Usage", typeof(string));                   //1
            dt.Columns.Add("Regional Sales Segment", typeof(string));                   //1
            dt.Columns.Add("Global Production Price Class", typeof(string));                   //1
            dt.Columns.Add("Global Sales Segment", typeof(string));                   //1
            dt.Columns.Add("Global Sales Sub-Segment", typeof(string));                   //1
            dt.Columns.Add("Global Sales Price Class", typeof(string));                   //1
            dt.Columns.Add("Short-Term Risk Rating", typeof(string));                   //1
            dt.Columns.Add("Long-Term Risk Rating", typeof(string));                   //1
            dt.Columns.Add("Porcentaje scrap", typeof(decimal));                  //1

            int camposPrevios = dt.Columns.Count;
            #endregion

            //agrega cabecera meses
            foreach (var c in cabeceraMeses)
                dt.Columns.Add(c.text, typeof(int));
            //agrega cabecera cuartos
            foreach (var c in cabeceraCuartos)
                dt.Columns.Add(c.text, typeof(int));
            //agrega cabecera años ene-dic
            foreach (var c in cabeceraAnios)
                dt.Columns.Add(c.text, typeof(int));
            //agrega cabecera fy
            foreach (var c in cabeceraAniosFY)
                dt.Columns.Add(c.text, typeof(int));

            // declara array multidimencional para guardar los estilos de cada celda
            int columnasStyles = camposPrevios + cabeceraMeses.Count + cabeceraCuartos.Count + cabeceraAnios.Count + cabeceraAniosFY.Count;

            SLStyle[,] styleCells = new SLStyle[listado.Count, columnasStyles];

            //copia el estilo de una celda 
            SLStyle styleSN = oSLDocument.GetCellStyle("A2");

            stopwatch.Stop();
            Debug.WriteLine($"--- FIN: Bloque 1 --- Tiempo: {stopwatch.Elapsed.TotalSeconds:F2} segundos.");
            Debug.WriteLine("-------------------------------------------------");
            // <<-- TEMPORIZADOR FIN: Bloque 1 -->>

            // <<-- TEMPORIZADOR INICIA: Bloque 2 -->>
            Debug.WriteLine($"--- INICIO: Bloque 2: Población del DataTable principal (Hoja '{hoja1}') ---");
            stopwatch.Restart();

            // OPTIMIZACIÓN: Crear Lookups y Diccionarios para búsquedas rápidas O(1).
            // Esto se hace UNA SOLA VEZ antes del bucle.
            var relacionesPorPlanta = todasLasRelacionesDeRegion
                .Where(r => r.BG_IHS_plantas != null && r.BG_IHS_regiones != null)
                .ToDictionary(r => r.BG_IHS_plantas.mnemonic_plant, r => r.BG_IHS_regiones);

            var demandaLookup = todaLaDemanda.ToLookup(d => d.id_ihs_item);
            var cuartosLookup = todosLosCuartos.ToLookup(c => c.id_ihs_item);


            ////registros , rows
            for (int i = 0; i < listado.Count; i++)
            {
                var item = listado[i];
                int indexColumna = 0;

                //crea row
                System.Data.DataRow row = dt.NewRow();

                #region datos general
                row["Id"] = item.id;
                row["Origen"] = item.origen;
                row["Vehicle (IHS)"] = item.vehicle;
                row["Vehicle (Compuesto)"] = item.ConcatCodigo;
                row["Core Nameplate Region Mnemonic"] = item.core_nameplate_region_mnemonic;
                row["Core Nameplate Plant Mnemonic"] = item.core_nameplate_plant_mnemonic;
                row["Mnemonic-Vehicle"] = item.mnemonic_vehicle;
                row["Mnemonic-Vehicle/Plant"] = item.mnemonic_vehicle_plant;
                row["Mnemonic-Platform"] = item.mnemonic_platform;
                row["Mnemonic-Plant"] = item.mnemonic_plant;
                row["Region"] = item.region;
                row["Market"] = item.market;
                row["Country/Territory"] = item.country_territory;
                row["Production Plant"] = item.production_plant;
                if (relacionesPorPlanta.TryGetValue(item.mnemonic_plant, out var region))
                {
                    item.SetCachedRegion(region);
                    row["Region(Plant)"] = region.descripcion;
                }
                else
                {
                    row["Region(Plant)"] = "SIN DEFINIR";
                }
                row["City"] = item.city;
                row["Plant State/Province"] = item.plant_state_province;
                row["Source Plant"] = item.source_plant;
                row["Source Plant Country/Territory"] = item.source_plant_country_territory;
                row["Source Plant Region"] = item.source_plant_region;
                row["Design Parent"] = item.design_parent;
                row["Engineering Group"] = item.engineering_group;
                row["Manufacturer Group"] = item.manufacturer_group;
                row["Manufacturer"] = item.manufacturer;
                row["Sales Parent"] = item.sales_parent;
                row["Production Brand"] = item.production_brand;
                row["Platform Design Owner"] = item.platform_design_owner;
                row["Architecture"] = item.architecture;
                row["Platform"] = item.platform;
                row["Program"] = item.program;
                row["Production Nameplate"] = item.production_nameplate;
                row["SOP (Start of Production)"] = item.sop_start_of_production;
                row["EOP (End of Production)"] = item.eop_end_of_production;
                row["Lifecycle (Time)"] = item.lifecycle_time;
                row["Assembly Type"] = item.assembly_type;
                row["Strategic Group"] = item.strategic_group;
                row["Sales Group"] = item.sales_group;
                row["Global Nameplate"] = item.global_nameplate;
                row["Primary Design Center"] = item.primary_design_center;
                row["Primary Design Country/Territory"] = item.primary_design_country_territory;
                row["Primary Design Region"] = item.primary_design_region;
                row["Secondary Design Center"] = item.secondary_design_center;
                row["Secondary Design Country/Territory	"] = item.secondary_design_country_territory;
                row["Secondary Design Region"] = item.secondary_design_region;
                row["GVW Rating"] = item.gvw_rating;
                row["GVW Class"] = item.gvw_class;
                row["Car/Truck"] = item.car_truck;
                row["Production Type"] = item.production_type;
                row["Global Production Segment"] = item.global_production_segment;
                row["Flat Rolled Steel Usage"] = item.RelSegmento != null ? item.RelSegmento.flat_rolled_steel_usage : null;
                row["Regional Sales Segment"] = item.regional_sales_segment;
                row["Global Production Price Class"] = item.global_production_price_class;
                row["Global Sales Segment"] = item.global_sales_segment;
                row["Global Sales Sub-Segment"] = item.global_sales_sub_segment;
                row["Global Sales Price Class"] = item.global_sales_price_class;
                row["Short-Term Risk Rating"] = item.short_term_risk_rating;
                row["Long-Term Risk Rating"] = item.long_term_risk_rating;
                row["Porcentaje scrap"] = item.porcentaje_scrap.HasValue ? item.porcentaje_scrap : (decimal)0.03;

                //agrega el tipo de index
                for (int j = 0; j < camposPrevios; j++)
                {

                    switch (item.origen)
                    {
                        case Bitacoras.Util.BG_IHS_Origen.IHS:
                            styleCells[i, j] = styleIHS;
                            break;
                        case Bitacoras.Util.BG_IHS_Origen.USER:
                            styleCells[i, j] = styleUser;
                            break;
                        default:
                            styleCells[i, j] = oSLDocument.CreateStyle();
                            break;

                    }
                    indexColumna++;
                }

                #endregion

                #region meses
                int indexCabecera = 0;

                //    Esta operación es súper rápida porque ya no va a la base de datos.
                var demandaParaEsteItem = demandaLookup[item.id];

                List<BG_IHS_rel_demanda> demandaMeses = item.GetDemanda(cabeceraMeses, demanda, demandaParaEsteItem);

                foreach (var item_demanda in demandaMeses)
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null)
                    {
                        row[cabeceraMeses[indexCabecera].text] = item_demanda.cantidad;

                        //agrega el estilo a la cabecera
                        switch (item_demanda.origen_datos)
                        {
                            case Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER:
                                styleCells[i, indexColumna] = styleDemandaCustomer;
                                break;
                            case Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL:
                                styleCells[i, indexColumna] = styleDemandaOriginal;
                                break;
                            default:
                                styleCells[i, indexColumna] = oSLDocument.CreateStyle();
                                break;
                        }
                    }
                    else
                    {
                        row[cabeceraMeses[indexCabecera].text] = DBNull.Value;
                        styleCells[i, indexColumna] = oSLDocument.CreateStyle();
                    }


                    indexColumna++;
                    indexCabecera++;
                }

                #endregion

                #region cuartos
                indexCabecera = 0;
                var cuartosParaEsteItem = cuartosLookup[item.id];

                foreach (var item_demanda in item.GetCuartos(demandaMeses, cabeceraCuartos, demanda, cuartosParaEsteItem))
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null && item_demanda.cantidad != null)
                    {
                        row[cabeceraCuartos[indexCabecera].text] = item_demanda.cantidad;
                        //agrega el estilo a la cabecera
                        switch (item_demanda.origen_datos)
                        {
                            case Portal_2_0.Models.Enum_BG_origen_cuartos.Calculado:
                                styleCells[i, indexColumna] = styleValorCalculado;
                                break;
                            case Portal_2_0.Models.Enum_BG_origen_cuartos.IHS:
                                styleCells[i, indexColumna] = styleValorIHS;
                                break;
                            default:
                                styleCells[i, indexColumna] = oSLDocument.CreateStyle();
                                break;
                        }

                    }
                    else
                    {
                        row[cabeceraCuartos[indexCabecera].text] = DBNull.Value;
                        styleCells[i, indexColumna] = oSLDocument.CreateStyle();
                    }


                    indexColumna++;
                    indexCabecera++;
                }

                #endregion

                #region años
                indexCabecera = 0;
                // 1. Filtras la lista grande de cuartos para obtener solo los de este item.
                //var cuartosParaEsteItem = todosLosCuartos.Where(c => c.id_ihs_item == item.id);

                // 2. Pasas esa lista filtrada a la nueva versión del método.
                foreach (var item_demanda in item.GetAnios(demandaMeses, cabeceraAnios, demanda, cuartosParaEsteItem))
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null && item_demanda.cantidad != null)
                    {
                        row[cabeceraAnios[indexCabecera].text] = item_demanda.cantidad;
                        //agrega el estilo a la cabecera
                        switch (item_demanda.origen_datos)
                        {
                            case Portal_2_0.Models.Enum_BG_origen_anios.Calculado:
                                styleCells[i, indexColumna] = styleValorCalculado;
                                break;
                            case Portal_2_0.Models.Enum_BG_origen_anios.IHS:
                                styleCells[i, indexColumna] = styleValorIHS;
                                break;
                            default:
                                styleCells[i, indexColumna] = oSLDocument.CreateStyle();
                                break;
                        }
                    }
                    else
                    {
                        row[cabeceraAnios[indexCabecera].text] = DBNull.Value;
                        styleCells[i, indexColumna] = oSLDocument.CreateStyle();
                    }

                    indexColumna++;
                    indexCabecera++;
                }

                #endregion
                #region años FY
                indexCabecera = 0;
                FYReference = indexColumna + 2;

                var datosAniosFY = item.GetAniosFY(demandaMeses, cabeceraAniosFY, demanda, cuartosParaEsteItem);
                listDatosRegionesFY.AddRange(datosAniosFY);

                foreach (var item_demanda in datosAniosFY)
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null && item_demanda.cantidad != null)
                    {
                        row[cabeceraAniosFY[indexCabecera].text] = item_demanda.cantidad;
                        //agrega el estilo a la cabecera
                        switch (item_demanda.origen_datos)
                        {
                            case Portal_2_0.Models.Enum_BG_origen_anios.Calculado:
                                styleCells[i, indexColumna] = styleValorCalculado;
                                break;
                            case Portal_2_0.Models.Enum_BG_origen_anios.IHS:
                                styleCells[i, indexColumna] = styleValorIHS;
                                break;
                            default:
                                styleCells[i, indexColumna] = oSLDocument.CreateStyle();
                                break;
                        }
                    }
                    else
                    {
                        row[cabeceraAniosFY[indexCabecera].text] = DBNull.Value;
                        styleCells[i, indexColumna] = oSLDocument.CreateStyle();
                    }

                    indexColumna++;
                    indexCabecera++;
                }

                #endregion

                //agrega la filas
                dt.Rows.Add(row);
            }

            stopwatch.Stop();
            Debug.WriteLine($"--- FIN: Bloque 2 --- Tiempo: {stopwatch.Elapsed.TotalSeconds:F2} segundos.");
            Debug.WriteLine("-------------------------------------------------");
            // <<-- TEMPORIZADOR FIN: Bloque 2 -->>

            #region Hoja Autos Normal
            // <<-- TEMPORIZADOR INICIA: Bloque 3 -->>
            Debug.WriteLine($"--- INICIO: Bloque 3: Importación del DataTable a Excel (Hoja '{hoja1}') ---");
            stopwatch.Restart();

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, hoja1);
            oSLDocument.SelectWorksheet(hoja1);
            oSLDocument.ImportDataTable(1, 2, dt, true);

            stopwatch.Stop();
            Debug.WriteLine($"--- FIN: Bloque 3 --- Tiempo: {stopwatch.Elapsed.TotalSeconds:F2} segundos.");
            Debug.WriteLine("-------------------------------------------------");
            // <<-- TEMPORIZADOR FIN: Bloque 3 -->>

            // <<-- TEMPORIZADOR INICIA: Bloque 4 -->>
            Debug.WriteLine($"--- INICIO: Bloque 4: Aplicación de estilos y formato (Hoja '{hoja1}') ---");
            stopwatch.Restart();

            foreach (BG_IHS_item item in listado)
            {
                int fila = listado.IndexOf(item) + 2;
                oSLDocument.SetCellValue(fila, 1, "SI");
            }

            //aplica el color de las celdas
            for (int a = 0; a < listado.Count; a++)
            {
                for (int b = 0; b < columnasStyles; b++)
                {
                    oSLDocument.SetCellStyle(a + 2, b + 2, styleCells[a, b]);
                }
            }

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            //estilo para numeros
            SLStyle styleNumberDecimal = oSLDocument.CreateStyle();
            styleNumberDecimal.FormatCode = "#,##0.000";

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy-mm";
            oSLDocument.SetColumnStyle(33, 34, styleShortDate);

            //crea Style para porcentaje
            SLStyle stylePercent = oSLDocument.CreateStyle();
            stylePercent.FormatCode = "0.00%";
            oSLDocument.SetColumnStyle(59, stylePercent);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;


            SLStyle styleAdvertencia = oSLDocument.CreateStyle();
            styleAdvertencia.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#FF8B00"), System.Drawing.ColorTranslator.FromHtml("#FF8B00"));


            //da estilo a los numeros
            //oSLDocument.SetColumnStyle(9, 10, styleNumberDecimal);

            //da estilo a la hoja de excel
            ////inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 4);

            oSLDocument.Filter(1, 1, 1, dt.Columns.Count + 1);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count + 1);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count + 1, styleWrap);
            oSLDocument.SetCellStyle(1, 1, 1, dt.Columns.Count + 1, styleHeader);
            oSLDocument.SetCellStyle(1, 1, 1, dt.Columns.Count + 1, styleHeaderFont);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);
            //oculta la columna de porcentaje
            oSLDocument.HideColumn(59);

            stopwatch.Stop();
            Debug.WriteLine($"--- FIN: Bloque 4 --- Tiempo: {stopwatch.Elapsed.TotalSeconds:F2} segundos.");
            Debug.WriteLine("-------------------------------------------------");
            // <<-- TEMPORIZADOR FIN: Bloque 4 -->>
            #endregion

            #region resumen_regiones
            // <<-- TEMPORIZADOR INICIA: Bloque 5 -->>
            Debug.WriteLine($"--- INICIO: Bloque 5: Generación de Hoja '{hoja2}' (fórmulas y gráficos) ---");
            stopwatch.Restart();

            //crear la hoja y la selecciona
            oSLDocument.AddWorksheet(hoja2);
            oSLDocument.SelectWorksheet(hoja2);

            dt = new System.Data.DataTable();

            //obtiene los FY
            dt.Columns.Add("Región", typeof(string));
            foreach (var fy in cabeceraAniosFY)
                dt.Columns.Add(fy.text, typeof(double));

            //inserta tabla
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //obtiene la lista de regiones
            // 1. Obtenemos los mnemonics de planta únicos de nuestra lista principal.
            var mnemonicsEnListado = listado.Select(item => item.mnemonic_plant).ToHashSet();

            // 2. Filtramos la lista de relaciones (que ya está en memoria) usando esos mnemonics,
            //    seleccionamos la descripción, quitamos nulos y obtenemos los valores únicos.
            var listRegiones = todasLasRelacionesDeRegion
                .Where(r => r.BG_IHS_plantas != null && mnemonicsEnListado.Contains(r.BG_IHS_plantas.mnemonic_plant))
                .Select(r => r.BG_IHS_regiones?.descripcion)
                .Where(desc => desc != null)
                .Distinct()
                .ToList();

            listRegiones.Add("SIN DEFINIR");

            int filaInicialFY = 2;
            int filaFinalFY = listado.Count + 1;

            //Recorre las regiones
            foreach (var region in listRegiones)
            {
                int fila = listRegiones.IndexOf(region) + 2;
                oSLDocument.SetCellValue(fila, 1, region);

                string SN = "SI";
                if (region != "CENTER" && region != "SOUTH" && region != "NORTH-WEST" && region != "NORTH")
                    SN = "NO";
                //Agrega el SI o NO
                oSLDocument.SetCellValue(fila, cabeceraAniosFY.Count + 2, SN);

                SLDataValidation dv;
                dv = oSLDocument.CreateDataValidation(fila, cabeceraAniosFY.Count + 2);
                dv.AllowList("aplica", true, true);
                oSLDocument.AddDataValidation(dv);

                //recorre los FY
                for (int i = 0; i < cabeceraAniosFY.Count; i++)
                {
                    string referenciaFY = GetCellReference(FYReference + i);
                    int columna = i + 2;

                    //=SUMAR.SI.CONJUNTO('Autos normal'!FI2:FI11,'Autos normal'!P2:P11,A2,'Autos normal'!A2:A11,"SI")/1000000
                    oSLDocument.SetCellValue(fila, columna, "=SUMIFS('" + hoja1 + "'!" + referenciaFY + filaInicialFY.ToString() + ":" + referenciaFY + filaFinalFY.ToString() +
                        ",'" + hoja1 + "'!P" + filaInicialFY + ":P" + filaFinalFY + ",A" + fila + ",'Autos normal'!A" + filaInicialFY + ":A" + filaFinalFY + ",\"SI\")/1000000");
                }
            }

            //formato condicional
            SLConditionalFormatting cf;
            cf = new SLConditionalFormatting(2, cabeceraAniosFY.Count + 2, listRegiones.Count + 2, cabeceraAniosFY.Count + 2);
            cf.HighlightCellsContainingText(true, "SI", SLHighlightCellsStyleValues.GreenFillWithDarkGreenText);
            oSLDocument.AddConditionalFormatting(cf);
            cf.HighlightCellsContainingText(true, "NO", SLHighlightCellsStyleValues.LightRedFillWithDarkRedText);
            oSLDocument.AddConditionalFormatting(cf);


            //agrega la tabla para SOP's
            int SOPInicial = cabeceraAniosFY.Count + 5;
            string referenciaSN = GetCellReference(cabeceraAniosFY.Count + 2);
            //agrega el titulo de la fila
            oSLDocument.SetCellValue(2, SOPInicial - 1, "Plant SOP's");

            for (int i = 0; i < cabeceraAniosFY.Count; i++)
            {
                string referenciaFY = GetCellReference(2 + i); //empieza desde la columna B

                oSLDocument.SetCellValue(1, SOPInicial + i, cabeceraAniosFY[i].text);

                oSLDocument.SetCellValue(2, SOPInicial + i, "=SUMIFS(" + referenciaFY + filaInicialFY + ": " + referenciaFY + (filaFinalFY - 1) + ", " + referenciaSN + filaInicialFY + ":" + referenciaSN + (filaFinalFY - 1) + ", \"SI\")");
            }

            //agrega la gráfica de regiones
            SLChart chart;
            string reference = GetCellReference(cabeceraAniosFY.Count + 1) + (listRegiones.Count + 1);
            chart = oSLDocument.CreateChart("A1", reference);
            chart.SetChartType(SLColumnChartType.ClusteredColumn);
            chart.SetChartPosition(listRegiones.Count + 4, 0.5f, listRegiones.Count + 26, 10);

            // use SLGroupDataLabelOptions for an entire data series
            SLGroupDataLabelOptions gdloptions;
            SLDataSeriesOptions dso;

            gdloptions = chart.CreateGroupDataLabelOptions();
            gdloptions.ShowValue = true;
            gdloptions.FormatCode = "0.00";

            // agrega el titulo da las series
            for (int i = 1; i <= listRegiones.Count; i++)
            {
                chart.SetGroupDataLabelOptions(i, gdloptions);
            }

            //titulo del la tabla
            chart.Title.SetTitle("Regiones");
            chart.ShowChartTitle(true);

            oSLDocument.InsertChart(chart);

            float tamanoGrafica = 21;
            float inicioGraficas = listRegiones.Count + 28;
            //crea una copia de la grafica
            foreach (var r in listRegiones)
            {
                int index = listRegiones.IndexOf(r);

                var chartX = chart;
                chartX.SetChartPosition(inicioGraficas + 2 + (tamanoGrafica * index), 0.5f, inicioGraficas + (tamanoGrafica * index) + tamanoGrafica, 10);
                chartX.Title.SetTitle(r);
                chartX.ShowChartTitle(true);

                // agrega el titulo da las series
                for (int i = 1; i <= listRegiones.Count; i++)
                {
                    // get the options from the 2nd data series
                    dso = chart.GetDataSeriesOptions(i);
                    // 10% transparency
                    dso.Fill.SetSolidFill(System.Drawing.ColorTranslator.FromHtml("#00B0F0"), 0);
                    // Or not, depending on what you want to achieve...
                    chart.SetDataSeriesOptions(i, dso);
                }
                oSLDocument.InsertChart(chartX);

                int iG = (int)inicioGraficas + 4 + ((int)tamanoGrafica * index);

                //inserta la tabla de tendencia
                for (int i = 0; i < cabeceraAniosFY.Count; i++)
                {
                    oSLDocument.SetCellValue(iG, SOPInicial + i, cabeceraAniosFY[i].text);
                    oSLDocument.SetCellValue(iG + i + 1, SOPInicial - 1, cabeceraAniosFY[i].text);
                    //da formato a la celda
                    oSLDocument.SetCellStyle(iG, SOPInicial + i, styleHeader);
                    oSLDocument.SetCellStyle(iG + i + 1, SOPInicial - 1, styleHeader);
                    oSLDocument.SetCellStyle(iG, SOPInicial + i, styleHeaderFont);
                    oSLDocument.SetCellStyle(iG + i + 1, SOPInicial - 1, styleHeaderFont);

                    for (int j = 0; j < cabeceraAniosFY.Count; j++)
                    {
                        string referenceMenor = GetCellReference(i + 2);
                        string referenceMayor = GetCellReference(j + 2);
                        int referenceFila = index + 2;

                        if (j != i && j > i)
                            oSLDocument.SetCellValue(iG + i + 1, SOPInicial + j, "=IFERROR((" + referenceMayor + referenceFila + "/" + referenceMenor + referenceFila + ")^(1/5)-1,\"-\")");
                        else if (j == i)
                            oSLDocument.SetCellValue(iG + i + 1, SOPInicial + j, "x");

                        //agrega el estilo de porcentaje
                        oSLDocument.SetCellStyle(iG + i + 1, SOPInicial + j, stylePercent);

                    }

                }

            }

            //inserta la grafica para SOP
            var chartSOP = oSLDocument.CreateChart(GetCellReference(SOPInicial - 1) + "1", GetCellReference(SOPInicial + cabeceraAniosFY.Count - 1) + "2");
            chartSOP.SetChartType(SLColumnChartType.ClusteredColumn);
            chartSOP.SetChartPosition(3, SOPInicial - 1, 17, SOPInicial + 8);
            chartSOP.Title.SetTitle("Plant SOP's");
            chartSOP.ShowChartTitle(true);
            //aplica el formato al texto de la serie
            chartSOP.SetGroupDataLabelOptions(1, gdloptions);
            //cambia el color de la serie
            dso = chartSOP.GetDataSeriesOptions(1);
            dso.Fill.SetSolidFill(System.Drawing.ColorTranslator.FromHtml("#00B0F0"), 0);
            chartSOP.SetDataSeriesOptions(1, dso);

            oSLDocument.InsertChart(chartSOP);


            //set sheet styles
            oSLDocument.Filter(1, 1, 1, dt.Columns.Count);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);
            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);

            //decimales
            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleNumberDecimal);
            oSLDocument.SetRowStyle(2, styleNumberDecimal);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            stopwatch.Stop();
            Debug.WriteLine($"--- FIN: Bloque 5 --- Tiempo: {stopwatch.Elapsed.TotalSeconds:F2} segundos.");
            Debug.WriteLine("-------------------------------------------------");
            // <<-- TEMPORIZADOR FIN: Bloque 5 -->>
            #endregion

            #region Autos Modificados
            // <<-- TEMPORIZADOR INICIA: Bloque 6 -->>
            Debug.WriteLine($"--- INICIO: Bloque 6: Generación de Hoja '{hoja3}' (copia, fórmulas, combinaciones y divisiones) ---");
            stopwatch.Restart();

            //copia la hoja1 a la hoja 3
            oSLDocument.CopyWorksheet(hoja1, hoja3);

            oSLDocument.SelectWorksheet(hoja3);

            //muestra la columna de porcentaje
            oSLDocument.UnhideColumn(59);
            //oculta la primera columna
            oSLDocument.HideColumn(1, 2);
            oSLDocument.HideColumn(6, 13);
            oSLDocument.HideColumn(21, 23);
            oSLDocument.HideColumn(28, 30);
            oSLDocument.HideColumn(35, 58);

            //obtiene la fila con la del porcentaje
            int colPorcentaje = camposPrevios + 1;  //+1 por el campo de si y no
            int numValores = cabeceraMeses.Count + cabeceraCuartos.Count + cabeceraAnios.Count + cabeceraAniosFY.Count;
            string colRef = GetCellReference(colPorcentaje);

            int fila_actual = 0;

            for (int i = 2; true; i++) //es infinito hasta se rompe
            {
                //lee si tiene un valor en la fila
                if (!String.IsNullOrEmpty(oSLDocument.GetCellValueAsString(i, 1)))
                {
                    for (int j = 1; j <= numValores; j++)
                    {
                        int col = j + colPorcentaje;
                        oSLDocument.SetCellValue(i, col, "='" + hoja1 + "'!" + GetCellReference(col) + i + "*(1+" + colRef + i + ")");

                    }
                }
                else
                { //termina el for
                    fila_actual = i + 1;
                    break;
                }

            }

            //-- COMBINACION

            #region combinaciones
            oSLDocument.SetCellValue(fila_actual, 2, "COMBINACIONES");
            oSLDocument.MergeWorksheetCells(fila_actual, 2, fila_actual, 4);
            oSLDocument.SetCellStyle(fila_actual, 2, styleHeader);
            oSLDocument.SetCellStyle(fila_actual, 2, styleHeaderFont);

            //aumenta la fila actual
            fila_actual++;
            string porcentajeReferencia = GetCellReference(camposPrevios + 1);

            //agrega una combinación
            foreach (var combinacion in combinaciones)
            {
                int inicio_fila = fila_actual;


                oSLDocument.SetCellValue(fila_actual, 3, "Combinación");
                oSLDocument.SetCellValue(fila_actual, 4, combinacion.vehicle);
                oSLDocument.SetCellValue(fila_actual, 5, combinacion.vehicle);
                oSLDocument.SetCellValue(fila_actual, 15, combinacion.production_plant);
                oSLDocument.SetCellValue(fila_actual, 24, combinacion.manufacturer_group);
                oSLDocument.SetCellValue(fila_actual, 25, combinacion.manufacturer_group);
                oSLDocument.SetCellValue(fila_actual, 27, combinacion.production_brand);
                oSLDocument.SetCellValue(fila_actual, 33, combinacion.sop_start_of_production.Value); //
                oSLDocument.SetCellValue(fila_actual, 34, combinacion.eop_end_of_production.Value); //
                oSLDocument.SetCellValue(fila_actual, 59, combinacion.porcentaje_scrap.Value); //

                //agrega el estilo a la cabecera
                oSLDocument.SetCellStyle(fila_actual, 1, fila_actual, columnasStyles + 1, styleTituloCombinacion);

                //crea las sumatoria
                for (int i = camposPrevios + 2; i < camposPrevios + 2 + (cabeceraMeses.Count + cabeceraCuartos.Count + cabeceraAnios.Count + cabeceraAniosFY.Count); i++)
                {
                    string celdaRef = GetCellReference(i);
                    oSLDocument.SetCellValue(fila_actual, i, "=SUM(" + celdaRef + (fila_actual + 1) + ":" + celdaRef + (fila_actual + combinacion.BG_IHS_rel_combinacion.Count) + ")");
                }

                fila_actual++;
                //sumar totales (con formula)
                dt = new System.Data.DataTable();

                //columnas          
                #region cabecera
                dt.Columns.Add("Id", typeof(string));                       //1
                dt.Columns.Add("Origen", typeof(string));                   //1
                dt.Columns.Add("Vehicle (IHS)", typeof(string));                   //1
                dt.Columns.Add("Vehicle (Compuesto)", typeof(string));                   //1
                dt.Columns.Add("Core Nameplate Region Mnemonic", typeof(string));                   //1
                dt.Columns.Add("Core Nameplate Plant Mnemonic", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Vehicle", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Vehicle/Plant", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Platform", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Plant", typeof(string));                   //1
                dt.Columns.Add("Region", typeof(string));                   //1
                dt.Columns.Add("Market", typeof(string));                   //1
                dt.Columns.Add("Country/Territory", typeof(string));                   //1
                dt.Columns.Add("Production Plant", typeof(string));                   //1
                dt.Columns.Add("Region(Plant)", typeof(string));                   //1
                dt.Columns.Add("City", typeof(string));                   //1
                dt.Columns.Add("Plant State/Province", typeof(string));                   //1
                dt.Columns.Add("Source Plant", typeof(string));                   //1
                dt.Columns.Add("Source Plant Country/Territory", typeof(string));                   //1
                dt.Columns.Add("Source Plant Region", typeof(string));                   //1
                dt.Columns.Add("Design Parent", typeof(string));                   //1
                dt.Columns.Add("Engineering Group", typeof(string));                   //1
                dt.Columns.Add("Manufacturer Group", typeof(string));                   //1
                dt.Columns.Add("Manufacturer", typeof(string));                   //1
                dt.Columns.Add("Sales Parent", typeof(string));                   //1
                dt.Columns.Add("Production Brand", typeof(string));                   //1
                dt.Columns.Add("Platform Design Owner", typeof(string));                   //1
                dt.Columns.Add("Architecture", typeof(string));                   //1
                dt.Columns.Add("Platform", typeof(string));                   //1
                dt.Columns.Add("Program", typeof(string));                   //1
                dt.Columns.Add("Production Nameplate", typeof(string));                   //1
                dt.Columns.Add("SOP (Start of Production)", typeof(DateTime));                   //1
                dt.Columns.Add("EOP (End of Production)", typeof(DateTime));                   //1
                dt.Columns.Add("Lifecycle (Time)", typeof(string));                   //1
                dt.Columns.Add("Assembly Type", typeof(string));                   //1
                dt.Columns.Add("Strategic Group", typeof(string));                   //1
                dt.Columns.Add("Sales Group", typeof(string));                   //1
                dt.Columns.Add("Global Nameplate", typeof(string));                   //1
                dt.Columns.Add("Primary Design Center", typeof(string));                   //1
                dt.Columns.Add("Primary Design Country/Territory", typeof(string));                   //1
                dt.Columns.Add("Primary Design Region", typeof(string));                   //1
                dt.Columns.Add("Secondary Design Center", typeof(string));                   //1
                dt.Columns.Add("Secondary Design Country/Territory	", typeof(string));                   //1
                dt.Columns.Add("Secondary Design Region", typeof(string));                   //1
                dt.Columns.Add("GVW Rating", typeof(string));                   //1
                dt.Columns.Add("GVW Class", typeof(string));                   //1
                dt.Columns.Add("Car/Truck", typeof(string));                   //1
                dt.Columns.Add("Production Type", typeof(string));                   //1
                dt.Columns.Add("Global Production Segment", typeof(string));                   //1
                dt.Columns.Add("Flat Rolled Steel Usage", typeof(string));                   //1
                dt.Columns.Add("Regional Sales Segment", typeof(string));                   //1
                dt.Columns.Add("Global Production Price Class", typeof(string));                   //1
                dt.Columns.Add("Global Sales Segment", typeof(string));                   //1
                dt.Columns.Add("Global Sales Sub-Segment", typeof(string));                   //1
                dt.Columns.Add("Global Sales Price Class", typeof(string));                   //1
                dt.Columns.Add("Short-Term Risk Rating", typeof(string));                   //1
                dt.Columns.Add("Long-Term Risk Rating", typeof(string));                   //1
                dt.Columns.Add("Porcentaje scrap", typeof(decimal));                  //1   
                                                                                      //agrega cabecera meses
                foreach (var c in cabeceraMeses)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                                                                             //agrega cabecera cuartos
                foreach (var c in cabeceraCuartos)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                                                                             //agrega cabecera años ene-dic
                foreach (var c in cabeceraAnios)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                                                                             //agrega cabecera fy
                foreach (var c in cabeceraAniosFY)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                #endregion

                foreach (var c_item in combinacion.BG_IHS_rel_combinacion)
                {
                    //crea row
                    System.Data.DataRow row = dt.NewRow();

                    // 1. Obtiene el porcentaje de demanda específico para este ítem.
                    decimal porcentajeItem = c_item.porcentaje_aplicado;
                    // 2. Lo convierte en un factor de cálculo para usar en la fórmula.
                    float factorAjusteItem = (float)porcentajeItem / 100.0f;

                    #region valores
                    row["Id"] = c_item.BG_IHS_item.id;
                    row["Origen"] = c_item.BG_IHS_item.origen;
                    row["Vehicle (IHS)"] = c_item.BG_IHS_item.vehicle;
                    row["Vehicle (Compuesto)"] = c_item.BG_IHS_item.ConcatCodigo;
                    row["Core Nameplate Region Mnemonic"] = c_item.BG_IHS_item.core_nameplate_region_mnemonic;
                    row["Core Nameplate Plant Mnemonic"] = c_item.BG_IHS_item.core_nameplate_plant_mnemonic;
                    row["Mnemonic-Vehicle"] = c_item.BG_IHS_item.mnemonic_vehicle;
                    row["Mnemonic-Vehicle/Plant"] = c_item.BG_IHS_item.mnemonic_vehicle_plant;
                    row["Mnemonic-Platform"] = c_item.BG_IHS_item.mnemonic_platform;
                    row["Mnemonic-Plant"] = c_item.BG_IHS_item.mnemonic_plant;
                    row["Region"] = c_item.BG_IHS_item.region;
                    row["Market"] = c_item.BG_IHS_item.market;
                    row["Country/Territory"] = c_item.BG_IHS_item.country_territory;
                    row["Production Plant"] = c_item.BG_IHS_item.production_plant;
                    // 1. Busca la relación en la lista grande que ya está en memoria.
                    var relacionRegion = todasLasRelacionesDeRegion
                        .FirstOrDefault(r => r.BG_IHS_plantas?.mnemonic_plant == c_item.BG_IHS_item.mnemonic_plant);

                    // 2. Asigna la descripción. Si no se encuentra nada, el resultado será null.
                    row["Region(Plant)"] = relacionRegion?.BG_IHS_regiones?.descripcion;
                    row["City"] = c_item.BG_IHS_item.city;
                    row["Plant State/Province"] = c_item.BG_IHS_item.plant_state_province;
                    row["Source Plant"] = c_item.BG_IHS_item.source_plant;
                    row["Source Plant Country/Territory"] = c_item.BG_IHS_item.source_plant_country_territory;
                    row["Source Plant Region"] = c_item.BG_IHS_item.source_plant_region;
                    row["Design Parent"] = c_item.BG_IHS_item.design_parent;
                    row["Engineering Group"] = c_item.BG_IHS_item.engineering_group;
                    row["Manufacturer Group"] = c_item.BG_IHS_item.manufacturer_group;
                    row["Manufacturer"] = c_item.BG_IHS_item.manufacturer;
                    row["Sales Parent"] = c_item.BG_IHS_item.sales_parent;
                    row["Production Brand"] = c_item.BG_IHS_item.production_brand;
                    row["Platform Design Owner"] = c_item.BG_IHS_item.platform_design_owner;
                    row["Architecture"] = c_item.BG_IHS_item.architecture;
                    row["Platform"] = c_item.BG_IHS_item.platform;
                    row["Program"] = c_item.BG_IHS_item.program;
                    row["Production Nameplate"] = c_item.BG_IHS_item.production_nameplate;
                    row["SOP (Start of Production)"] = c_item.BG_IHS_item.sop_start_of_production;
                    row["EOP (End of Production)"] = c_item.BG_IHS_item.eop_end_of_production;
                    row["Lifecycle (Time)"] = c_item.BG_IHS_item.lifecycle_time;
                    row["Assembly Type"] = c_item.BG_IHS_item.assembly_type;
                    row["Strategic Group"] = c_item.BG_IHS_item.strategic_group;
                    row["Sales Group"] = c_item.BG_IHS_item.sales_group;
                    row["Global Nameplate"] = c_item.BG_IHS_item.global_nameplate;
                    row["Primary Design Center"] = c_item.BG_IHS_item.primary_design_center;
                    row["Primary Design Country/Territory"] = c_item.BG_IHS_item.primary_design_country_territory;
                    row["Primary Design Region"] = c_item.BG_IHS_item.primary_design_region;
                    row["Secondary Design Center"] = c_item.BG_IHS_item.secondary_design_center;
                    row["Secondary Design Country/Territory	"] = c_item.BG_IHS_item.secondary_design_country_territory;
                    row["Secondary Design Region"] = c_item.BG_IHS_item.secondary_design_region;
                    row["GVW Rating"] = c_item.BG_IHS_item.gvw_rating;
                    row["GVW Class"] = c_item.BG_IHS_item.gvw_class;
                    row["Car/Truck"] = c_item.BG_IHS_item.car_truck;
                    row["Production Type"] = c_item.BG_IHS_item.production_type;
                    row["Global Production Segment"] = c_item.BG_IHS_item.global_production_segment;
                    row["Flat Rolled Steel Usage"] = c_item.BG_IHS_item.RelSegmento != null ? c_item.BG_IHS_item.RelSegmento.flat_rolled_steel_usage : null;
                    row["Regional Sales Segment"] = c_item.BG_IHS_item.regional_sales_segment;
                    row["Global Production Price Class"] = c_item.BG_IHS_item.global_production_price_class;
                    row["Global Sales Segment"] = c_item.BG_IHS_item.global_sales_segment;
                    row["Global Sales Sub-Segment"] = c_item.BG_IHS_item.global_sales_sub_segment;
                    row["Global Sales Price Class"] = c_item.BG_IHS_item.global_sales_price_class;
                    row["Short-Term Risk Rating"] = c_item.BG_IHS_item.short_term_risk_rating;
                    row["Long-Term Risk Rating"] = c_item.BG_IHS_item.long_term_risk_rating;
                    row["Porcentaje scrap"] = DBNull.Value;
                    #region meses
                    int indexCabecera = 0;
                    int indexColumna = camposPrevios + 2;


                    // 1. Filtra la lista grande en memoria para este item específico.
                    var demandaParaEsteItem = todaLaDemanda.Where(d => d.id_ihs_item == c_item.BG_IHS_item.id);

                    // 2. Llama al método sobrecargado con los datos precargados.
                    List<BG_IHS_rel_demanda> demandaMeses = c_item.BG_IHS_item.GetDemanda(cabeceraMeses, demanda, demandaParaEsteItem);

                    foreach (var item_demanda in demandaMeses)
                    {
                        //si no es nul agrega la cantidad
                        if (item_demanda != null)
                        {
                            //row[cabeceraMeses[indexCabecera].text] = item_demanda.cantidad;
                            //= 250 * (1 + BG11)
                            float cantidadOriginal = item_demanda?.cantidad ?? 0;
                            // Se añade la multiplicación por el factorAjusteItem a la fórmula de Excel
                            row[cabeceraMeses[indexCabecera].text] = $"={cantidadOriginal} * {factorAjusteItem} * (1+{porcentajeReferencia}{inicio_fila})";

                            //agrega el estilo a la cabecera
                            switch (item_demanda.origen_datos)
                            {
                                case Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleDemandaCustomer);
                                    break;
                                case Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleDemandaOriginal);
                                    break;

                            }
                        }
                        else
                        {
                            row[cabeceraMeses[indexCabecera].text] = DBNull.Value;
                        }
                        indexCabecera++;
                        indexColumna++;
                    }

                    #endregion

                    #region cuartos
                    indexCabecera = 0;

                    var cuartosParaEsteItem = todosLosCuartos.Where(c => c.id_ihs_item == c_item.BG_IHS_item.id);

                    foreach (var item_demanda in c_item.BG_IHS_item.GetCuartos(demandaMeses, cabeceraCuartos, demanda, cuartosParaEsteItem))
                    {
                        //si no es nul agrega la cantidad
                        if (item_demanda != null && item_demanda.cantidad != null)
                        {
                            float cantidadOriginal = item_demanda?.cantidad ?? 0;
                            row[cabeceraMeses[indexCabecera].text] = $"={cantidadOriginal} * {factorAjusteItem} * (1+{porcentajeReferencia}{inicio_fila})";

                            //agrega el estilo a la cabecera
                            switch (item_demanda.origen_datos)
                            {
                                case Portal_2_0.Models.Enum_BG_origen_cuartos.Calculado:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorCalculado);
                                    break;
                                case Portal_2_0.Models.Enum_BG_origen_cuartos.IHS:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorIHS);
                                    break;
                            }

                        }
                        else
                        {
                            row[cabeceraCuartos[indexCabecera].text] = DBNull.Value;
                        }
                        indexColumna++;
                        indexCabecera++;
                    }

                    #endregion

                    #region años
                    indexCabecera = 0;
                    foreach (var item_demanda in c_item.BG_IHS_item.GetAnios(demandaMeses, cabeceraAnios, demanda, cuartosParaEsteItem))
                    {
                        //si no es nul agrega la cantidad
                        if (item_demanda != null && item_demanda.cantidad != null)
                        {
                            float cantidadOriginal = item_demanda?.cantidad ?? 0;
                            row[cabeceraMeses[indexCabecera].text] = $"={cantidadOriginal} * {factorAjusteItem} * (1+{porcentajeReferencia}{inicio_fila})";
                            //agrega el estilo a la cabecera
                            switch (item_demanda.origen_datos)
                            {
                                case Portal_2_0.Models.Enum_BG_origen_anios.Calculado:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorCalculado);
                                    break;
                                case Portal_2_0.Models.Enum_BG_origen_anios.IHS:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorIHS);
                                    break;
                            }
                        }
                        else
                        {
                            row[cabeceraAnios[indexCabecera].text] = DBNull.Value;
                        }

                        indexColumna++;
                        indexCabecera++;
                    }

                    #endregion
                    #region años FY
                    indexCabecera = 0;
                    //FYReference = indexColumna + 2;

                    var datosAniosFY = c_item.BG_IHS_item.GetAniosFY(demandaMeses, cabeceraAniosFY, demanda, cuartosParaEsteItem);
                    listDatosRegionesFY.AddRange(datosAniosFY);

                    foreach (var item_demanda in datosAniosFY)
                    {
                        //si no es nul agrega la cantidad
                        if (item_demanda != null && item_demanda.cantidad != null)
                        {
                            float cantidadOriginal = item_demanda?.cantidad ?? 0;
                            row[cabeceraMeses[indexCabecera].text] = $"={cantidadOriginal} * {factorAjusteItem} * (1+{porcentajeReferencia}{inicio_fila})";
                            //agrega el estilo a la cabecera
                            switch (item_demanda.origen_datos)
                            {
                                case Portal_2_0.Models.Enum_BG_origen_anios.Calculado:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorCalculado);
                                    break;
                                case Portal_2_0.Models.Enum_BG_origen_anios.IHS:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorIHS);
                                    break;

                            }
                        }
                        else
                        {
                            row[cabeceraAniosFY[indexCabecera].text] = DBNull.Value;
                        }

                        indexColumna++;
                        indexCabecera++;
                    }

                    #endregion

                    #endregion


                    //aplica el estilo a los primeros campos
                    switch (c_item.BG_IHS_item.origen)
                    {
                        case Bitacoras.Util.BG_IHS_Origen.IHS:
                            oSLDocument.SetCellStyle(fila_actual, 2, fila_actual, camposPrevios + 1, styleIHS);
                            break;
                        case Bitacoras.Util.BG_IHS_Origen.USER:
                            oSLDocument.SetCellStyle(fila_actual, 2, fila_actual, camposPrevios + 1, styleUser);
                            break;

                    }
                    //agrega la filas
                    dt.Rows.Add(row);
                    fila_actual++;
                }

                oSLDocument.ImportDataTable(fila_actual - combinacion.BG_IHS_rel_combinacion.Count, 2, dt, false);

            }

            #endregion

            // -- DIVISIONES
            #region divisiones

            fila_actual++;

            oSLDocument.SetCellValue(fila_actual, 2, "DIVISIONES");
            oSLDocument.MergeWorksheetCells(fila_actual, 2, fila_actual, 4);
            oSLDocument.SetCellStyle(fila_actual, 2, styleHeader);
            oSLDocument.SetCellStyle(fila_actual, 2, styleHeaderFont);

            fila_actual++;

            foreach (var division in divisiones)
            {
                int inicio_fila = fila_actual;

                dt = new System.Data.DataTable();
                //columnas          
                #region cabecera
                dt.Columns.Add("Id", typeof(string));                       //1
                dt.Columns.Add("Origen", typeof(string));                   //1
                dt.Columns.Add("Vehicle (IHS)", typeof(string));                   //1
                dt.Columns.Add("Vehicle (Compuesto)", typeof(string));                   //1
                dt.Columns.Add("Core Nameplate Region Mnemonic", typeof(string));                   //1
                dt.Columns.Add("Core Nameplate Plant Mnemonic", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Vehicle", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Vehicle/Plant", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Platform", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Plant", typeof(string));                   //1
                dt.Columns.Add("Region", typeof(string));                   //1
                dt.Columns.Add("Market", typeof(string));                   //1
                dt.Columns.Add("Country/Territory", typeof(string));                   //1
                dt.Columns.Add("Production Plant", typeof(string));                   //1
                dt.Columns.Add("Region(Plant)", typeof(string));                   //1
                dt.Columns.Add("City", typeof(string));                   //1
                dt.Columns.Add("Plant State/Province", typeof(string));                   //1
                dt.Columns.Add("Source Plant", typeof(string));                   //1
                dt.Columns.Add("Source Plant Country/Territory", typeof(string));                   //1
                dt.Columns.Add("Source Plant Region", typeof(string));                   //1
                dt.Columns.Add("Design Parent", typeof(string));                   //1
                dt.Columns.Add("Engineering Group", typeof(string));                   //1
                dt.Columns.Add("Manufacturer Group", typeof(string));                   //1
                dt.Columns.Add("Manufacturer", typeof(string));                   //1
                dt.Columns.Add("Sales Parent", typeof(string));                   //1
                dt.Columns.Add("Production Brand", typeof(string));                   //1
                dt.Columns.Add("Platform Design Owner", typeof(string));                   //1
                dt.Columns.Add("Architecture", typeof(string));                   //1
                dt.Columns.Add("Platform", typeof(string));                   //1
                dt.Columns.Add("Program", typeof(string));                   //1
                dt.Columns.Add("Production Nameplate", typeof(string));                   //1
                dt.Columns.Add("SOP (Start of Production)", typeof(DateTime));                   //1
                dt.Columns.Add("EOP (End of Production)", typeof(DateTime));                   //1
                dt.Columns.Add("Lifecycle (Time)", typeof(string));                   //1
                dt.Columns.Add("Assembly Type", typeof(string));                   //1
                dt.Columns.Add("Strategic Group", typeof(string));                   //1
                dt.Columns.Add("Sales Group", typeof(string));                   //1
                dt.Columns.Add("Global Nameplate", typeof(string));                   //1
                dt.Columns.Add("Primary Design Center", typeof(string));                   //1
                dt.Columns.Add("Primary Design Country/Territory", typeof(string));                   //1
                dt.Columns.Add("Primary Design Region", typeof(string));                   //1
                dt.Columns.Add("Secondary Design Center", typeof(string));                   //1
                dt.Columns.Add("Secondary Design Country/Territory	", typeof(string));                   //1
                dt.Columns.Add("Secondary Design Region", typeof(string));                   //1
                dt.Columns.Add("GVW Rating", typeof(string));                   //1
                dt.Columns.Add("GVW Class", typeof(string));                   //1
                dt.Columns.Add("Car/Truck", typeof(string));                   //1
                dt.Columns.Add("Production Type", typeof(string));                   //1
                dt.Columns.Add("Global Production Segment", typeof(string));                   //1
                dt.Columns.Add("Flat Rolled Steel Usage", typeof(string));                   //1
                dt.Columns.Add("Regional Sales Segment", typeof(string));                   //1
                dt.Columns.Add("Global Production Price Class", typeof(string));                   //1
                dt.Columns.Add("Global Sales Segment", typeof(string));                   //1
                dt.Columns.Add("Global Sales Sub-Segment", typeof(string));                   //1
                dt.Columns.Add("Global Sales Price Class", typeof(string));                   //1
                dt.Columns.Add("Short-Term Risk Rating", typeof(string));                   //1
                dt.Columns.Add("Long-Term Risk Rating", typeof(string));                   //1
                dt.Columns.Add("Porcentaje scrap", typeof(decimal));                  //1   
                                                                                      //agrega cabecera meses
                foreach (var c in cabeceraMeses)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                                                                             //agrega cabecera cuartos
                foreach (var c in cabeceraCuartos)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                                                                             //agrega cabecera años ene-dic
                foreach (var c in cabeceraAnios)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                                                                             //agrega cabecera fy
                foreach (var c in cabeceraAniosFY)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                #endregion


                //crea row
                System.Data.DataRow row = dt.NewRow();

                #region valores
                row["Id"] = division.BG_IHS_item.id;
                row["Origen"] = division.BG_IHS_item.origen;
                row["Vehicle (IHS)"] = division.BG_IHS_item.vehicle;
                row["Vehicle (Compuesto)"] = division.BG_IHS_item.ConcatCodigo;
                row["Core Nameplate Region Mnemonic"] = division.BG_IHS_item.core_nameplate_region_mnemonic;
                row["Core Nameplate Plant Mnemonic"] = division.BG_IHS_item.core_nameplate_plant_mnemonic;
                row["Mnemonic-Vehicle"] = division.BG_IHS_item.mnemonic_vehicle;
                row["Mnemonic-Vehicle/Plant"] = division.BG_IHS_item.mnemonic_vehicle_plant;
                row["Mnemonic-Platform"] = division.BG_IHS_item.mnemonic_platform;
                row["Mnemonic-Plant"] = division.BG_IHS_item.mnemonic_plant;
                row["Region"] = division.BG_IHS_item.region;
                row["Market"] = division.BG_IHS_item.market;
                row["Country/Territory"] = division.BG_IHS_item.country_territory;
                row["Production Plant"] = division.BG_IHS_item.production_plant;
                var relacionRegion = todasLasRelacionesDeRegion
                    .FirstOrDefault(r => r.BG_IHS_plantas?.mnemonic_plant == division.BG_IHS_item.mnemonic_plant);

                row["Region(Plant)"] = relacionRegion?.BG_IHS_regiones?.descripcion;
                row["City"] = division.BG_IHS_item.city;
                row["Plant State/Province"] = division.BG_IHS_item.plant_state_province;
                row["Source Plant"] = division.BG_IHS_item.source_plant;
                row["Source Plant Country/Territory"] = division.BG_IHS_item.source_plant_country_territory;
                row["Source Plant Region"] = division.BG_IHS_item.source_plant_region;
                row["Design Parent"] = division.BG_IHS_item.design_parent;
                row["Engineering Group"] = division.BG_IHS_item.engineering_group;
                row["Manufacturer Group"] = division.BG_IHS_item.manufacturer_group;
                row["Manufacturer"] = division.BG_IHS_item.manufacturer;
                row["Sales Parent"] = division.BG_IHS_item.sales_parent;
                row["Production Brand"] = division.BG_IHS_item.production_brand;
                row["Platform Design Owner"] = division.BG_IHS_item.platform_design_owner;
                row["Architecture"] = division.BG_IHS_item.architecture;
                row["Platform"] = division.BG_IHS_item.platform;
                row["Program"] = division.BG_IHS_item.program;
                row["Production Nameplate"] = division.BG_IHS_item.production_nameplate;
                row["SOP (Start of Production)"] = division.BG_IHS_item.sop_start_of_production;
                row["EOP (End of Production)"] = division.BG_IHS_item.eop_end_of_production;
                row["Lifecycle (Time)"] = division.BG_IHS_item.lifecycle_time;
                row["Assembly Type"] = division.BG_IHS_item.assembly_type;
                row["Strategic Group"] = division.BG_IHS_item.strategic_group;
                row["Sales Group"] = division.BG_IHS_item.sales_group;
                row["Global Nameplate"] = division.BG_IHS_item.global_nameplate;
                row["Primary Design Center"] = division.BG_IHS_item.primary_design_center;
                row["Primary Design Country/Territory"] = division.BG_IHS_item.primary_design_country_territory;
                row["Primary Design Region"] = division.BG_IHS_item.primary_design_region;
                row["Secondary Design Center"] = division.BG_IHS_item.secondary_design_center;
                row["Secondary Design Country/Territory	"] = division.BG_IHS_item.secondary_design_country_territory;
                row["Secondary Design Region"] = division.BG_IHS_item.secondary_design_region;
                row["GVW Rating"] = division.BG_IHS_item.gvw_rating;
                row["GVW Class"] = division.BG_IHS_item.gvw_class;
                row["Car/Truck"] = division.BG_IHS_item.car_truck;
                row["Production Type"] = division.BG_IHS_item.production_type;
                row["Global Production Segment"] = division.BG_IHS_item.global_production_segment;
                row["Flat Rolled Steel Usage"] = division.BG_IHS_item.RelSegmento != null ? division.BG_IHS_item.RelSegmento.flat_rolled_steel_usage : null;
                row["Regional Sales Segment"] = division.BG_IHS_item.regional_sales_segment;
                row["Global Production Price Class"] = division.BG_IHS_item.global_production_price_class;
                row["Global Sales Segment"] = division.BG_IHS_item.global_sales_segment;
                row["Global Sales Sub-Segment"] = division.BG_IHS_item.global_sales_sub_segment;
                row["Global Sales Price Class"] = division.BG_IHS_item.global_sales_price_class;
                row["Short-Term Risk Rating"] = division.BG_IHS_item.short_term_risk_rating;
                row["Long-Term Risk Rating"] = division.BG_IHS_item.long_term_risk_rating;
                row["Porcentaje scrap"] = division.porcentaje_scrap;
                #region meses
                int indexCabecera = 0;
                int indexColumna = camposPrevios + 2;


                // 1. Filtra la lista grande en memoria.
                var demandaParaEsteItem = todaLaDemanda.Where(d => d.id_ihs_item == division.BG_IHS_item.id);

                // 2. Llama al método sobrecargado con los datos precargados.
                List<BG_IHS_rel_demanda> demandaMeses = division.BG_IHS_item.GetDemanda(cabeceraMeses, demanda, demandaParaEsteItem);

                foreach (var item_demanda in demandaMeses)
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null)
                    {
                        //row[cabeceraMeses[indexCabecera].text] = item_demanda.cantidad;
                        //= 250 * (1 + BG11)
                        row[cabeceraMeses[indexCabecera].text] = "=" + (item_demanda.cantidad != null ? item_demanda.cantidad.Value : 0) + "*(1+" + porcentajeReferencia + inicio_fila + ")";

                        //agrega el estilo a la cabecera
                        // coloca el texto en negritas
                        oSLDocument.SetCellStyle(fila_actual, indexColumna, styleBoldBlue);
                        switch (item_demanda.origen_datos)
                        {
                            case Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleDemandaCustomer);
                                break;
                            case Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleDemandaOriginal);
                                break;

                        }

                    }
                    else
                    {
                        row[cabeceraMeses[indexCabecera].text] = DBNull.Value;
                    }
                    indexCabecera++;
                    indexColumna++;
                }

                #endregion

                #region cuartos
                indexCabecera = 0;
                var cuartosParaEsteItem = todosLosCuartos.Where(c => c.id_ihs_item == division.BG_IHS_item.id);

                foreach (var item_demanda in division.BG_IHS_item.GetCuartos(demandaMeses, cabeceraCuartos, demanda, cuartosParaEsteItem))
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null && item_demanda.cantidad != null)
                    {
                        row[cabeceraCuartos[indexCabecera].text] = "=" + (item_demanda.cantidad != null ? item_demanda.cantidad.Value : 0) + "*(1+" + porcentajeReferencia + inicio_fila + ")";

                        //agrega el estilo a la cabecera
                        // coloca el texto en negritas
                        oSLDocument.SetCellStyle(fila_actual, indexColumna, styleBoldBlue);
                        switch (item_demanda.origen_datos)
                        {
                            case Portal_2_0.Models.Enum_BG_origen_cuartos.Calculado:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorCalculado);
                                break;
                            case Portal_2_0.Models.Enum_BG_origen_cuartos.IHS:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorIHS);
                                break;
                        }
                    }
                    else
                    {
                        row[cabeceraCuartos[indexCabecera].text] = DBNull.Value;
                    }
                    indexColumna++;
                    indexCabecera++;
                }

                #endregion

                #region años
                indexCabecera = 0;
                foreach (var item_demanda in division.BG_IHS_item.GetAnios(demandaMeses, cabeceraAnios, demanda, cuartosParaEsteItem))
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null && item_demanda.cantidad != null)
                    {
                        row[cabeceraAnios[indexCabecera].text] = "=" + (item_demanda.cantidad != null ? item_demanda.cantidad.Value : 0) + "*(1+" + porcentajeReferencia + inicio_fila + ")";

                        //agrega el estilo a la cabecera
                        // coloca el texto en negritas
                        oSLDocument.SetCellStyle(fila_actual, indexColumna, styleBoldBlue);
                        switch (item_demanda.origen_datos)
                        {
                            case Portal_2_0.Models.Enum_BG_origen_anios.Calculado:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorCalculado);
                                break;
                            case Portal_2_0.Models.Enum_BG_origen_anios.IHS:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorIHS);
                                break;
                        }

                    }
                    else
                    {
                        row[cabeceraAnios[indexCabecera].text] = DBNull.Value;
                    }

                    indexColumna++;
                    indexCabecera++;
                }

                #endregion
                #region años FY
                indexCabecera = 0;
                //FYReference = indexColumna + 2;

                var datosAniosFY = division.BG_IHS_item.GetAniosFY(demandaMeses, cabeceraAniosFY, demanda, cuartosParaEsteItem);
                listDatosRegionesFY.AddRange(datosAniosFY);

                foreach (var item_demanda in datosAniosFY)
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null && item_demanda.cantidad != null)
                    {
                        row[cabeceraAniosFY[indexCabecera].text] = "=" + (item_demanda.cantidad != null ? item_demanda.cantidad.Value : 0) + "*(1+" + porcentajeReferencia + inicio_fila + ")";

                        //agrega el estilo a la cabecera
                        // coloca el texto en negritas
                        oSLDocument.SetCellStyle(fila_actual, indexColumna, styleBoldBlue);
                        switch (item_demanda.origen_datos)
                        {
                            case Portal_2_0.Models.Enum_BG_origen_anios.Calculado:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorCalculado);
                                break;
                            case Portal_2_0.Models.Enum_BG_origen_anios.IHS:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorIHS);
                                break;

                        }

                    }
                    else
                    {
                        row[cabeceraAniosFY[indexCabecera].text] = DBNull.Value;
                    }

                    indexColumna++;
                    indexCabecera++;
                }

                #endregion

                #endregion

                // coloca el texto en negritas
                oSLDocument.SetCellStyle(fila_actual, 2, fila_actual, camposPrevios + 1, styleBoldBlue);
                //aplica el estilo a los primeros campos
                switch (division.BG_IHS_item.origen)
                {
                    case Bitacoras.Util.BG_IHS_Origen.IHS:
                        oSLDocument.SetCellStyle(fila_actual, 2, fila_actual, camposPrevios + 1, styleIHS);
                        break;
                    case Bitacoras.Util.BG_IHS_Origen.USER:
                        oSLDocument.SetCellStyle(fila_actual, 2, fila_actual, camposPrevios + 1, styleUser);
                        break;

                }


                //agrega la filas
                dt.Rows.Add(row);
                //agrega la tabla
                oSLDocument.ImportDataTable(fila_actual, 2, dt, false);

                fila_actual++;

                int fila_porcentaje_division = 31;
                string porcentajeDivisionReferencia = GetCellReference(fila_porcentaje_division);

                foreach (var rel in division.BG_IHS_rel_division)
                {
                    //aplica

                    oSLDocument.SetCellValue(fila_actual, 4, rel.vehicle);
                    oSLDocument.SetCellValue(fila_actual, 5, rel.vehicle);
                    oSLDocument.SetCellValue(fila_actual, 31, rel.porcentaje.Value);
                    oSLDocument.SetCellStyle(fila_actual, fila_porcentaje_division, stylePercent);
                    oSLDocument.SetCellValue(fila_actual, 32, rel.production_nameplate);

                    //agrega la formula
                    for (int i = camposPrevios + 2; i < camposPrevios + 2 + (cabeceraMeses.Count + cabeceraCuartos.Count + cabeceraAnios.Count + cabeceraAniosFY.Count); i++)
                    {
                        string celdaRef = GetCellReference(i);
                        oSLDocument.SetCellValue(fila_actual, i, "=" + celdaRef + inicio_fila + "*" + porcentajeDivisionReferencia + fila_actual);
                    }
                    fila_actual++;
                }
            }


            #endregion


            //estilos
            oSLDocument.SetColumnStyle(33, 34, styleShortDate);
            oSLDocument.SetColumnStyle(59, stylePercent);

            //establece alto de las filas
            oSLDocument.SetRowHeight(1, fila_actual, 15.0);
            //set autofit
            oSLDocument.AutoFitColumn(1, columnasStyles);

            stopwatch.Stop();
            Debug.WriteLine($"--- FIN: Bloque 6 --- Tiempo: {stopwatch.Elapsed.TotalSeconds:F2} segundos.");
            Debug.WriteLine("-------------------------------------------------");
            // <<-- TEMPORIZADOR FIN: Bloque 6 -->>
            #endregion


            //vuelve a selecciona la hoja 1
            oSLDocument.SelectWorksheet(hoja1);

            // <<-- TEMPORIZADOR INICIA: Bloque 7 -->>
            Debug.WriteLine("--- INICIO: Bloque 7: Guardado final del documento en Stream y conversión a byte[] ---");
            stopwatch.Restart();

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            stopwatch.Stop();
            Debug.WriteLine($"--- FIN: Bloque 7 --- Tiempo: {stopwatch.Elapsed.TotalSeconds:F2} segundos.");
            Debug.WriteLine("*************************************************");
            Debug.WriteLine($"****** FINALIZADA GENERACIÓN DE REPORTE IHS ( {DateTime.Now} ) *****");
            Debug.WriteLine("*************************************************");
            // <<-- TEMPORIZADOR FIN: Bloque 7 -->>

            return (array);
        }

        /// <summary>
        /// Genera reporte de Forecast (Unión IHS-Reporte)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static byte[] GeneraReporteBudgetForecast(ReporteBudgetForecastViewModel model, Portal_2_0Entities db, IHubContext hubContext)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();

            hubContext.Clients.All.recibirProgresoExcel(1, 1, 100, "Inicia Generación del reporte.");

            var cabeceraMeses = Portal_2_0.Models.BG_IHS_UTIL.GetCabecera();
            var cabeceraCuartos = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraCuartos();
            var cabeceraAnios = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAnios();
            var cabeceraAniosFY = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraAniosFY();

            string hoja1 = "Autos normal";
            string hoja2 = "Autos Modificados";

            System.Diagnostics.Debug.WriteLine($"[TIMER] Inicialización y Variables: {timer.Elapsed.TotalSeconds:F2} segundos");
            timer.Restart();
            // 1. Obtener los items del reporte
            var forecastItems = db.BG_Forecast_item
                                  .Where(x => x.BG_Forecast_reporte.id == model.id_reporte)
                                  .Include(x => x.BG_IHS_item)
                                  .Include(x => x.BG_IHS_combinacion.BG_IHS_rel_combinacion.Select(rel => rel.BG_IHS_item))
                                  .Include(x => x.BG_IHS_rel_division.BG_IHS_division.BG_IHS_item)
                                  .ToList();

            // 2. Consolidar una lista ÚNICA de todos los BG_IHS_item que se van a procesar
            var allItemsIHS = new Dictionary<int, BG_IHS_item>();
            foreach (var forecastItem in forecastItems)
            {
                if (forecastItem.BG_IHS_item != null && !allItemsIHS.ContainsKey(forecastItem.BG_IHS_item.id))
                    allItemsIHS.Add(forecastItem.BG_IHS_item.id, forecastItem.BG_IHS_item);

                if (forecastItem.BG_IHS_combinacion != null)
                    foreach (var rel in forecastItem.BG_IHS_combinacion.BG_IHS_rel_combinacion)
                        if (rel.BG_IHS_item != null && !allItemsIHS.ContainsKey(rel.BG_IHS_item.id))
                            allItemsIHS.Add(rel.BG_IHS_item.id, rel.BG_IHS_item);

                if (forecastItem.BG_IHS_rel_division?.BG_IHS_division?.BG_IHS_item != null)
                {
                    var divisionItem = forecastItem.BG_IHS_rel_division.BG_IHS_division.BG_IHS_item;
                    if (!allItemsIHS.ContainsKey(divisionItem.id))
                        allItemsIHS.Add(divisionItem.id, divisionItem);
                }
            }

            // 3. Obtener la lista de IDs de los items IHS únicos
            var uniqueIhsItemIds = allItemsIHS.Keys.ToList();

            // 4. Precargar TODA la data relacionada en consultas únicas
            // 4a. Precargar Demanda
            var demandasAgrupadas = db.BG_IHS_rel_demanda
                                      .Where(d => uniqueIhsItemIds.Contains(d.id_ihs_item))
                                      .ToLookup(d => d.id_ihs_item);

            // 4b. Precargar Regiones
            var plantMnemonics = allItemsIHS.Values.Where(i => !string.IsNullOrEmpty(i.mnemonic_plant)).Select(i => i.mnemonic_plant).Distinct().ToList();
            var regionesPorPlanta = db.BG_IHS_rel_regiones
                                      .Where(rel => plantMnemonics.Contains(rel.BG_IHS_plantas.mnemonic_plant))
                                      .Include(rel => rel.BG_IHS_regiones)
                                      .Include(rel => rel.BG_IHS_plantas)
                                      .GroupBy(rel => rel.BG_IHS_plantas.mnemonic_plant)
                                      .ToDictionary(g => g.Key, g => g.First().BG_IHS_regiones);

            // 4c. Precargar Cuartos (¡NUEVO!)
            var cuartosAgrupados = db.BG_IHS_rel_cuartos
                                     .Where(c => uniqueIhsItemIds.Contains(c.id_ihs_item))
                                     .ToLookup(c => c.id_ihs_item);


            // 5. Inyectar las regiones cacheadas en los objetos
            foreach (var item in allItemsIHS.Values)
            {
                if (regionesPorPlanta.TryGetValue(item.mnemonic_plant, out var region))
                {
                    item.SetCachedRegion(region);
                }
            }


            System.Diagnostics.Debug.WriteLine($"[TIMER] Obtención de forecastItems desde BD: {timer.Elapsed.TotalSeconds:F2} segundos");
            timer.Restart();

            List<BG_IHS_item> listado = new List<BG_IHS_item>();
            List<BG_IHS_combinacion> combinaciones = new List<BG_IHS_combinacion>();
            List<BG_IHS_division> divisiones = new List<BG_IHS_division>();
            List<BG_ihs_vehicle_custom> hechizos = new List<BG_ihs_vehicle_custom>();

            List<ReferenciaColumna> referenciaColumnas = new List<ReferenciaColumna>();
            List<ReferenciaColumna> referenciaColumnasFY = new List<ReferenciaColumna>();
            List<ReferenciaColumna> referenciaTablas = new List<ReferenciaColumna>();

            int inicioInventoryOwnMeses = 0;

            //crea un array con la referencia inicial a las tablas
            List<ReferenciaColumna> tablasReferenciasIniciales = new List<ReferenciaColumna>() {
                new ReferenciaColumna {celdaDescripcion ="Autos/Month" },
                new ReferenciaColumna {celdaDescripcion ="Total Sales [TUSD]" },
                new ReferenciaColumna {celdaDescripcion ="Material Cost [TUSD]", },
                new ReferenciaColumna {celdaDescripcion ="COST OF OUTSIDE PROCESSOR (e.g. LASER WELD)/PART  [TUSD]" },
                new ReferenciaColumna {celdaDescripcion ="Value Added Sales [TUSD]" },      //-ok
                new ReferenciaColumna {celdaDescripcion ="Processed Tons [to]" },
                new ReferenciaColumna {celdaDescripcion ="Engineered Scrap [to]", extra="Concatenado" },
                new ReferenciaColumna {celdaDescripcion ="Scrap Consolidation [to]" },
                new ReferenciaColumna {celdaDescripcion ="Strokes [ - / 1000 ]" },
                new ReferenciaColumna {celdaDescripcion ="Blanks [ - / 1000 ]" },
                new ReferenciaColumna {celdaDescripcion ="Additional material cost total [USD]" },
                new ReferenciaColumna {celdaDescripcion ="Outgoing freight total [USD]" },      //ok
                new ReferenciaColumna {celdaDescripcion ="Inventory OWN (monthly average) [tons]" },
                new ReferenciaColumna {celdaDescripcion ="Inventory (End of month) [USD]" },
                new ReferenciaColumna {celdaDescripcion ="Freights Income USD/PART", extra= "Freights Income USD/PART"},
                new ReferenciaColumna {celdaDescripcion ="Maniobras USD/PART", extra= "Maniobras USD/PART"},
                new ReferenciaColumna {celdaDescripcion ="Customs Expenses USD/PART", extra= "Customs Expenses USD/PART"},
                new ReferenciaColumna {celdaDescripcion ="Shipment Tons [to]" },
                new ReferenciaColumna {celdaDescripcion ="SALES Inc. SCRAP [USD]" },
                new ReferenciaColumna {celdaDescripcion ="VAS Inc. SCRAP [USD]" },
                new ReferenciaColumna {celdaDescripcion ="Processing  Inc. SCRAP" },
                new ReferenciaColumna {celdaDescripcion ="Wooden pallets", extra= "USD/PART" },
                new ReferenciaColumna {celdaDescripcion ="Standard packaging", extra= "USD/PART" },
                new ReferenciaColumna {celdaDescripcion ="PLASTIC STRIPS", extra= "USD/PART" },

            };

            //declara constantes para la cabecera
            const string _POS = "POS";
            const string _VEHICLE_IHS = "Vehicle - IHS";
            const string _SAP_INVOICE_CODE = "SAP Invoice Code";
            const string _PREVIOUS_SAP_INVOICE_CODE = "Previous SAP Invoice Code";
            const string _A_D = "A/D";
            const string _INICIO_DEMANDA = "Inicio Demanda";
            const string _FIN_DEMANDA = "Fin Demanda";
            const string _MNEMONIC_VEHICLE_PLANT = "Mnemonic-Vehicle/Plant";
            const string _BUSINESS_AND_PLANT = "Business & plant";
            const string _BUSINNESS = "Business";
            const string _INVOICED_TO = "Invoiced to";
            const string _NUMBER_SAP_CLIENT = "Number SAP Cliente";
            const string _SHIPPED_TO = "Shipped to";
            const string _OWN_CM = "OWN/CM";
            const string _ROUTE = "Route";
            const string _PLANT = "Plant";
            const string _EXTERNAL_PROCESSOR = "External Processor";
            const string _MILL = "Mill";
            const string _SAP_MASTER_COIL = "SAP Master Coil";
            const string _PART_DESCRIPTION = "Part Description";
            const string _PART_NUMBER = "Part number";
            const string _PRODUCTION_LINE = "Production Line";
            const string _PRODUCTION_NAMEPLATE = "Production Nameplate";
            const string _PROPULSION_SYSTEM_TYPE = "Propulsion System Type";
            const string _OEM = "OEM";
            const string _PARTS_AUTO = "Parts/Auto";
            const string _STROKES_AUTO = "Strokes/Auto";
            const string _MATERIAL_TYPE = "Material Type";
            const string _MATERIAL_TYPE_SHORT = "Material";
            const string _SHAPE = "Shape";
            const string _INITIAL_WEIGHT_PART = "Initial Weight/Part [KG]";
            const string _NET_WEIGHT_PART = "Net Weight/Part [KG]";
            const string _ENG_SCRAP_PART = "Eng. Scrap/Part [KG]";
            const string _SCRAP_CONSOLIDATION = "Scrap Consolidation";
            const string _VENTAS_PART = "Ventas/Part [USD]";
            const string _MATERIAL_COST_PART = "Material Cost/Part [USD]";
            const string _COST_OF_OUTSIDE_PROCESSOR = "Cost of Outside Proccessor [USD]";
            const string _VAS_PART = "VAS/Part [USD]";
            const string _ADDITIONAL_MATERIAL_COST_PART = "Additional Material Cost/Part [USD]";
            const string _OUTGOING_FREIGHT_PART = "Outgoing Freight/PART [USD]";
            const string _TRANS_SILAO_SLP = "Trans Silao  - SLP";
            const string _VAS_TO = "Vas/to";
            const string _GROSS_PROFIT_OUTGOING_FREIGHT_PART = "Gross profit-outgoing freight/Part [USD]";
            const string _GROSS_PROFIT_OUTGOING_FREIGHT_TO = "Gross profit-outgoing freight/to [USD]";
            const string _FREIGHTS_INCOME = "Freights Income";
            const string _OUTGOING_FREIGHT = "Outgoing freight";
            const string _COILS_AND_SLITTER = "Coils & slitter";
            const string _ADDITIONAL_PROCESSES = "Additional Processes";
            const string _PRODUCTION_PROCESSES = "Production Processes";
            const string _IHS_ASOCIADO = "IHS Asociado";

            //crea un list con los titulos de la tablas de by Month
            int cDic = 1;

            Dictionary<string, string> dictionaryTitulosByMonth = new Dictionary<string, string>
            {
                { GetCellReference(cDic++), _POS },
                { GetCellReference(cDic++), _VEHICLE_IHS },
                { GetCellReference(cDic++), _SAP_INVOICE_CODE },
                { GetCellReference(cDic++), _A_D },
                { GetCellReference(cDic++), _INICIO_DEMANDA },
                { GetCellReference(cDic++), _FIN_DEMANDA },
                { GetCellReference(cDic++), _BUSINESS_AND_PLANT },
                { GetCellReference(cDic++), _BUSINNESS },
                { GetCellReference(cDic++), _PREVIOUS_SAP_INVOICE_CODE },
                { GetCellReference(cDic++), _MNEMONIC_VEHICLE_PLANT},
                { GetCellReference(cDic++), _INVOICED_TO },
                { GetCellReference(cDic++), _NUMBER_SAP_CLIENT },
                { GetCellReference(cDic++), _SHIPPED_TO },
                { GetCellReference(cDic++), _OWN_CM },
                { GetCellReference(cDic++), _ROUTE },
                { GetCellReference(cDic++), _PLANT },
                { GetCellReference(cDic++), _EXTERNAL_PROCESSOR },
                { GetCellReference(cDic++), _MILL },
                { GetCellReference(cDic++), _SAP_MASTER_COIL },
                { GetCellReference(cDic++), _PART_DESCRIPTION },
                { GetCellReference(cDic++), _PART_NUMBER },
                { GetCellReference(cDic++), _PRODUCTION_LINE },
                { GetCellReference(cDic++), _PRODUCTION_NAMEPLATE },
                { GetCellReference(cDic++), _PROPULSION_SYSTEM_TYPE },
                { GetCellReference(cDic++), _OEM },
                { GetCellReference(cDic++), _PARTS_AUTO },
                { GetCellReference(cDic++), _STROKES_AUTO },
                { GetCellReference(cDic++), _MATERIAL_TYPE },
                { GetCellReference(cDic++), _MATERIAL_TYPE_SHORT },
                { GetCellReference(cDic++), _SHAPE },
                { GetCellReference(cDic++), _INITIAL_WEIGHT_PART },
                { GetCellReference(cDic++), _NET_WEIGHT_PART },
                { GetCellReference(cDic++), _ENG_SCRAP_PART },
                { GetCellReference(cDic++), _SCRAP_CONSOLIDATION },
                { GetCellReference(cDic++), _VENTAS_PART },
                { GetCellReference(cDic++), _MATERIAL_COST_PART },
                { GetCellReference(cDic++), _COST_OF_OUTSIDE_PROCESSOR },
                { GetCellReference(cDic++), _VAS_PART },
                { GetCellReference(cDic++), _ADDITIONAL_MATERIAL_COST_PART },
                { GetCellReference(cDic++), _OUTGOING_FREIGHT_PART },
                { GetCellReference(cDic++), _TRANS_SILAO_SLP },
                { GetCellReference(cDic++), _VAS_TO },
                { GetCellReference(cDic++), _GROSS_PROFIT_OUTGOING_FREIGHT_PART },
                { GetCellReference(cDic++), _GROSS_PROFIT_OUTGOING_FREIGHT_TO },
                { GetCellReference(cDic++), _FREIGHTS_INCOME },
                { GetCellReference(cDic++), _OUTGOING_FREIGHT },
                { GetCellReference(cDic++), _COILS_AND_SLITTER }, //cat 1
                { GetCellReference(cDic++), _ADDITIONAL_PROCESSES }, //cat 3
                { GetCellReference(cDic++), _PRODUCTION_PROCESSES }, //cat 4
                { GetCellReference(cDic++), _IHS_ASOCIADO },
            };

            //listado para tabla de InventoryOWN 
            List<BG_Forecast_cat_inventory_own> listOWNBD = db.BG_Forecast_cat_inventory_own.Where(x => x.id_bg_forecast_reporte == model.id_reporte).ToList();
            List<double> averageOwnList = new List<double>();
            foreach (var itemOWN in listOWNBD.OrderBy(x => x.orden))
                averageOwnList.Add(itemOWN.cantidad);

            System.Diagnostics.Debug.WriteLine($"[TIMER] Obtención de listOWNBD: {timer.Elapsed.TotalSeconds:F2} segundos");
            timer.Restart();

            //obtiene los elementos de IHS asociados al reporte
            listado = forecastItems
                      .Where(x => x.id_ihs_item.HasValue)
                      .Select(x => x.BG_IHS_item)
                      .Distinct()
                      .ToList();

            System.Diagnostics.Debug.WriteLine($"[TIMER] Obtención de listado: {timer.Elapsed.TotalSeconds:F2} segundos");
            timer.Restart();

            //obtiene las combinaciones de IHS asociados al reporte
            combinaciones = forecastItems
                            .Where(x => x.id_ihs_combinacion.HasValue)
                            .Select(x => x.BG_IHS_combinacion)
                            .Distinct()
                            .ToList();

            System.Diagnostics.Debug.WriteLine($"[TIMER] Obtención de combinaciones: {timer.Elapsed.TotalSeconds:F2} segundos");
            timer.Restart();
            //obtiene las divisiones de IHS asociados al reporte
            divisiones = forecastItems
                         .Where(x => x.id_ihs_rel_division.HasValue)
                         .Select(x => x.BG_IHS_rel_division.BG_IHS_division)
                         .Distinct()
                         .ToList();
            System.Diagnostics.Debug.WriteLine($"[TIMER] Obtención de divisiones: {timer.Elapsed.TotalSeconds:F2} segundos");
            timer.Restart();

            //obtiene los hechizos asociados al reporte
            hechizos = forecastItems
                         .Where(x => x.id_ihs_custom.HasValue)
                         .Select(x => x.BG_ihs_vehicle_custom)
                         .Distinct()
                         .ToList();

            System.Diagnostics.Debug.WriteLine($"[TIMER] Obtención de hechizos: {timer.Elapsed.TotalSeconds:F2} segundos");
            timer.Restart();

            //obtiene el reporte
            var reporte = db.BG_Forecast_reporte.Find(model.id_reporte);

            int filaTablaScrapPorPlanta = reporte.BG_Forecast_item.Count + 8;

            List<string> listaCombinacionesScrap = new List<string> { "CM5190STEEL","CM5390STEEL","CM5890STEEL","CM5190ALU","CM5390ALU","CM5890ALU"
                        ,"OWN5190STEEL","OWN5390STEEL","OWN5890STEEL","OWN5190ALU","OWN5390ALU","OWN5890ALU" };


            //fila para clientes
            int filaCliente = reporte.BG_Forecast_item.Count + 22;

            //fila steel 
            int filaScrapSteel = filaCliente + db.BG_forecast_cat_clientes.Where(x => x.activo).Count() + 4;

            //fila alu
            int filaScrapAlu = filaScrapSteel + listaCombinacionesScrap.Where(x => x.Contains("STEEL")).Count() + 2;


            int FYReference = 0;

            hubContext.Clients.All.recibirProgresoExcel(8, 8, 100, "Creando estilos para el excel.");


            //para regiones
            List<BG_IHS_item_anios> listDatosRegionesFY = new List<BG_IHS_item_anios>();

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/Reporte_IHS.xlsx"), "Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();

            //estilos para celdas
            SLStyle styleIHS = oSLDocument.CreateStyle();
            styleIHS.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#faebd7"), System.Drawing.ColorTranslator.FromHtml("#faebd7"));

            SLStyle styleUser = oSLDocument.CreateStyle();
            styleUser.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#d6c6ea"), System.Drawing.ColorTranslator.FromHtml("#d6c6ea"));

            SLStyle styleDemandaOriginal = oSLDocument.CreateStyle();
            styleDemandaOriginal.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#faff6b"), System.Drawing.ColorTranslator.FromHtml("#faff6b"));

            SLStyle styleDemandaCustomer = oSLDocument.CreateStyle();
            styleDemandaCustomer.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#6afcf3"), System.Drawing.ColorTranslator.FromHtml("#6afcf3"));

            SLStyle styleValorCalculado = oSLDocument.CreateStyle();
            styleValorCalculado.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#bbf3c1"), System.Drawing.ColorTranslator.FromHtml("#bbf3c1"));

            SLStyle styleValorIHS = oSLDocument.CreateStyle();
            styleValorIHS.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffb6c1"), System.Drawing.ColorTranslator.FromHtml("#ffb6c1"));

            SLStyle styleSinAsociacion = oSLDocument.CreateStyle();
            styleSinAsociacion.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffAAAA"), System.Drawing.ColorTranslator.FromHtml("#ffAAAA"));

            SLStyle styleTotales = oSLDocument.CreateStyle();
            styleTotales.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#E5E5E5"), System.Drawing.ColorTranslator.FromHtml("#E5E5E5"));

            SLStyle styleTituloCombinacion = oSLDocument.CreateStyle();
            styleTituloCombinacion.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#FFCC99"), System.Drawing.ColorTranslator.FromHtml("#FFCC99"));
            styleTituloCombinacion.Font.FontColor = System.Drawing.Color.DarkBlue;
            styleTituloCombinacion.Font.Bold = true;

            SLStyle styleBoldBlue = oSLDocument.CreateStyle();
            styleBoldBlue.Font.FontColor = System.Drawing.Color.DarkBlue;
            styleBoldBlue.Font.Bold = true;

            SLStyle styleDivisionesData = oSLDocument.CreateStyle();
            styleDivisionesData.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#D5E8DB"), System.Drawing.ColorTranslator.FromHtml("#D5E8DB"));

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para ajustar al texto
            SLStyle styleCenterTop = oSLDocument.CreateStyle();
            styleCenterTop.SetWrapText(true);
            styleCenterTop.Alignment.Vertical = VerticalAlignmentValues.Top;
            styleCenterTop.Alignment.Horizontal = HorizontalAlignmentValues.Center;

            //estilo para ajustar al texto
            SLStyle styleCenterCenter = oSLDocument.CreateStyle();
            styleCenterCenter.Alignment.Vertical = VerticalAlignmentValues.Center;
            styleCenterCenter.Alignment.Horizontal = HorizontalAlignmentValues.Center;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            // 1. Crea un nuevo objeto de estilo vacío
            SLStyle styleBorders = oSLDocument.CreateStyle();

            // 2. Define las propiedades de los cuatro bordes
            styleBorders.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorders.Border.TopBorder.Color = System.Drawing.Color.Black;
            styleBorders.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorders.Border.BottomBorder.Color = System.Drawing.Color.Black;
            styleBorders.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorders.Border.LeftBorder.Color = System.Drawing.Color.Black;
            styleBorders.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            styleBorders.Border.RightBorder.Color = System.Drawing.Color.Black;

            //estilo para el remarcar
            SLStyle styleHighlight = oSLDocument.CreateStyle();
            styleHighlight.Font.Bold = true;
            styleHighlight.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#FEF714"), System.Drawing.ColorTranslator.FromHtml("#FEF714"));

            //estilo para el remarcar
            SLStyle styleHighlightGray = oSLDocument.CreateStyle();
            styleHighlightGray.Font.Bold = true;
            styleHighlightGray.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ECECEC"), System.Drawing.ColorTranslator.FromHtml("#ECECEC"));


            //estilo para numeros
            SLStyle styleNumberDecimal_0 = oSLDocument.CreateStyle();
            styleNumberDecimal_0.FormatCode = "#,##0";

            //estilo para numeros
            SLStyle styleNumberDecimal_1 = oSLDocument.CreateStyle();
            styleNumberDecimal_1.FormatCode = "#,##0.0;[Red]- #,##0.0";

            //estilo para numeros
            SLStyle styleNumberDecimal_2 = oSLDocument.CreateStyle();
            styleNumberDecimal_2.FormatCode = "#,##0.00;[Red]- #,##0.00";

            //estilo para numeros
            SLStyle styleNumberDecimal_3 = oSLDocument.CreateStyle();
            styleNumberDecimal_3.FormatCode = "#,##0.000";

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy-mm";

            //crea Style para porcentaje
            SLStyle stylePercent = oSLDocument.CreateStyle();
            stylePercent.FormatCode = "0.00%";
            //crea Style para moneda

            SLStyle styleCurrency = oSLDocument.CreateStyle();
            styleCurrency.FormatCode = "$ #,##0.00;[Red]-$ #,##0.00";

            SLStyle styleCurrencyLinea = oSLDocument.CreateStyle();
            styleCurrencyLinea.FormatCode = "_-$* #,##0.0_-;-$* #,##0.0_-;_-$* \"-\"??_-;_-@_-";

            SLStyle styleNumericLine = oSLDocument.CreateStyle();
            styleNumericLine.FormatCode = "_-* #,##0_-;-* #,##0_-;_-* \"-\"??_-;_-@_-";


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            SLStyle styleMissedIHS = oSLDocument.CreateStyle();
            styleMissedIHS.Font.FontColor = System.Drawing.ColorTranslator.FromHtml("#680600");
            styleMissedIHS.Font.Bold = true;
            styleMissedIHS.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#FDE9D9"), System.Drawing.ColorTranslator.FromHtml("#FDE9D9"));


            System.Diagnostics.Debug.WriteLine($"[TIMER] estilos creados: {timer.Elapsed.TotalSeconds:F2} segundos");
            timer.Restart();

            #region Hoja Autos Normal
            hubContext.Clients.All.recibirProgresoExcel(12, 12, 100, "Inicia Hoja Autos Normal.");


            //columnas          
            #region cabecera general
            dt.Columns.Add("Id", typeof(string));                       //1
            dt.Columns.Add("Origen", typeof(string));                   //1
            dt.Columns.Add("Vehicle (IHS)", typeof(string));                   //1
            dt.Columns.Add("Vehicle (Compuesto)", typeof(string));                   //1
            dt.Columns.Add("Core Nameplate Region Mnemonic", typeof(string));                   //1
            dt.Columns.Add("Core Nameplate Plant Mnemonic", typeof(string));                   //1
            dt.Columns.Add("Mnemonic-Vehicle", typeof(string));                   //1
            dt.Columns.Add("Mnemonic-Vehicle/Plant", typeof(string));                   //1
            dt.Columns.Add("Mnemonic-Platform", typeof(string));                   //1
            dt.Columns.Add("Mnemonic-Plant", typeof(string));                   //1
            dt.Columns.Add("Region", typeof(string));                   //1
            dt.Columns.Add("Market", typeof(string));                   //1
            dt.Columns.Add("Country/Territory", typeof(string));                   //1
            dt.Columns.Add("Production Plant", typeof(string));                   //1
            dt.Columns.Add("Region(Plant)", typeof(string));                   //1
            dt.Columns.Add("City", typeof(string));                   //1
            dt.Columns.Add("Plant State/Province", typeof(string));                   //1
            dt.Columns.Add("Source Plant", typeof(string));                   //1
            dt.Columns.Add("Source Plant Country/Territory", typeof(string));                   //1
            dt.Columns.Add("Source Plant Region", typeof(string));                   //1
            dt.Columns.Add("Design Parent", typeof(string));                   //1
            dt.Columns.Add("Engineering Group", typeof(string));                   //1
            dt.Columns.Add("Manufacturer Group", typeof(string));                   //1
            dt.Columns.Add("Manufacturer", typeof(string));                   //1
            dt.Columns.Add("Sales Parent", typeof(string));                   //1
            dt.Columns.Add("Production Brand", typeof(string));                   //1
            dt.Columns.Add("Platform Design Owner", typeof(string));                   //1
            dt.Columns.Add("Architecture", typeof(string));                   //1
            dt.Columns.Add("Platform", typeof(string));                   //1
            dt.Columns.Add("Program", typeof(string));                   //1
            dt.Columns.Add("Production Nameplate", typeof(string));                   //1
            dt.Columns.Add("SOP (Start of Production)", typeof(DateTime));                   //1
            dt.Columns.Add("EOP (End of Production)", typeof(DateTime));                   //1
            dt.Columns.Add("Lifecycle (Time)", typeof(string));                   //1
            dt.Columns.Add("Assembly Type", typeof(string));                   //1
            dt.Columns.Add("Strategic Group", typeof(string));                   //1
            dt.Columns.Add("Sales Group", typeof(string));                   //1
            dt.Columns.Add("Global Nameplate", typeof(string));                   //1
            dt.Columns.Add("Primary Design Center", typeof(string));                   //1
            dt.Columns.Add("Primary Design Country/Territory", typeof(string));                   //1
            dt.Columns.Add("Primary Design Region", typeof(string));                   //1
            dt.Columns.Add("Secondary Design Center", typeof(string));                   //1
            dt.Columns.Add("Secondary Design Country/Territory	", typeof(string));                   //1
            dt.Columns.Add("Secondary Design Region", typeof(string));                   //1
            dt.Columns.Add("GVW Rating", typeof(string));                   //1
            dt.Columns.Add("GVW Class", typeof(string));                   //1
            dt.Columns.Add("Car/Truck", typeof(string));                   //1
            dt.Columns.Add("Production Type", typeof(string));                   //1
            dt.Columns.Add("Global Production Segment", typeof(string));                   //1
            dt.Columns.Add("Flat Rolled Steel Usage", typeof(string));                   //1
            dt.Columns.Add("Regional Sales Segment", typeof(string));                   //1
            dt.Columns.Add("Global Production Price Class", typeof(string));                   //1
            dt.Columns.Add("Global Sales Segment", typeof(string));                   //1
            dt.Columns.Add("Global Sales Sub-Segment", typeof(string));                   //1
            dt.Columns.Add("Global Sales Price Class", typeof(string));                   //1
            dt.Columns.Add("Short-Term Risk Rating", typeof(string));                   //1
            dt.Columns.Add("Long-Term Risk Rating", typeof(string));                   //1
            dt.Columns.Add("Porcentaje scrap", typeof(decimal));                  //1

            int camposPrevios = dt.Columns.Count;
            #endregion

            //agrega cabecera meses
            hubContext.Clients.All.recibirProgresoExcel(15, 15, 100, "Obteniendo las cabeceras del reporte.");

            foreach (var c in cabeceraMeses)
                dt.Columns.Add(c.text, typeof(int));                  //1
                                                                      //agrega cabecera cuartos
            foreach (var c in cabeceraCuartos)
                dt.Columns.Add(c.text, typeof(int));                  //1
                                                                      //agrega cabecera años ene-dic
            foreach (var c in cabeceraAnios)
                dt.Columns.Add(c.text, typeof(int));                  //1
                                                                      //agrega cabecera fy
            foreach (var c in cabeceraAniosFY)
                dt.Columns.Add(c.text, typeof(int));                  //1

            // declara array multidimencional para guardar los estilos de cada celda
            int columnasStyles = camposPrevios + cabeceraMeses.Count + cabeceraCuartos.Count + cabeceraAnios.Count + cabeceraAniosFY.Count;

            SLStyle[,] styleCells = new SLStyle[listado.Count, columnasStyles];

            //copia el estilo de una celda 
            SLStyle styleSN = oSLDocument.GetCellStyle("A2");

            ////registros , rows
            int indexElemento = 0;
            foreach (BG_IHS_item item in listado)
            {

                hubContext.Clients.All.recibirProgresoExcel(19, 19, 100, $"Agregando registros al reporte Hoja Autos normal ({indexElemento}/{listado.Count})");

                int indexColumna = 0;

                //crea row
                System.Data.DataRow row = dt.NewRow();

                #region datos general
                row["Id"] = "I_" + item.id;
                row["Origen"] = item.origen;
                row["Vehicle (IHS)"] = item.vehicle;
                row["Vehicle (Compuesto)"] = item.ConcatCodigo;
                row["Core Nameplate Region Mnemonic"] = item.core_nameplate_region_mnemonic;
                row["Core Nameplate Plant Mnemonic"] = item.core_nameplate_plant_mnemonic;
                row["Mnemonic-Vehicle"] = item.mnemonic_vehicle;
                row["Mnemonic-Vehicle/Plant"] = item.mnemonic_vehicle_plant;
                row["Mnemonic-Platform"] = item.mnemonic_platform;
                row["Mnemonic-Plant"] = item.mnemonic_plant;
                row["Region"] = item.region;
                row["Market"] = item.market;
                row["Country/Territory"] = item.country_territory;
                row["Production Plant"] = item.production_plant;
                row["Region(Plant)"] = item._Region != null ? item._Region.descripcion : null;
                row["City"] = item.city;
                row["Plant State/Province"] = item.plant_state_province;
                row["Source Plant"] = item.source_plant;
                row["Source Plant Country/Territory"] = item.source_plant_country_territory;
                row["Source Plant Region"] = item.source_plant_region;
                row["Design Parent"] = item.design_parent;
                row["Engineering Group"] = item.engineering_group;
                row["Manufacturer Group"] = item.manufacturer_group;
                row["Manufacturer"] = item.manufacturer;
                row["Sales Parent"] = item.sales_parent;
                row["Production Brand"] = item.production_brand;
                row["Platform Design Owner"] = item.platform_design_owner;
                row["Architecture"] = item.architecture;
                row["Platform"] = item.platform;
                row["Program"] = item.program;
                row["Production Nameplate"] = item.production_nameplate;
                row["SOP (Start of Production)"] = item.sop_start_of_production;
                row["EOP (End of Production)"] = item.eop_end_of_production;
                row["Lifecycle (Time)"] = item.lifecycle_time;
                row["Assembly Type"] = item.assembly_type;
                row["Strategic Group"] = item.strategic_group;
                row["Sales Group"] = item.sales_group;
                row["Global Nameplate"] = item.global_nameplate;
                row["Primary Design Center"] = item.primary_design_center;
                row["Primary Design Country/Territory"] = item.primary_design_country_territory;
                row["Primary Design Region"] = item.primary_design_region;
                row["Secondary Design Center"] = item.secondary_design_center;
                row["Secondary Design Country/Territory	"] = item.secondary_design_country_territory;
                row["Secondary Design Region"] = item.secondary_design_region;
                row["GVW Rating"] = item.gvw_rating;
                row["GVW Class"] = item.gvw_class;
                row["Car/Truck"] = item.car_truck;
                row["Production Type"] = item.production_type;
                row["Global Production Segment"] = item.global_production_segment;
                row["Flat Rolled Steel Usage"] = item.RelSegmento != null ? item.RelSegmento.flat_rolled_steel_usage : null;
                row["Regional Sales Segment"] = item.regional_sales_segment;
                row["Global Production Price Class"] = item.global_production_price_class;
                row["Global Sales Segment"] = item.global_sales_segment;
                row["Global Sales Sub-Segment"] = item.global_sales_sub_segment;
                row["Global Sales Price Class"] = item.global_sales_price_class;
                row["Short-Term Risk Rating"] = item.short_term_risk_rating;
                row["Long-Term Risk Rating"] = item.long_term_risk_rating;
                row["Porcentaje scrap"] = item.porcentaje_scrap.HasValue ? item.porcentaje_scrap : (decimal)0.03;

                //agrega el tipo de index
                for (int i = 0; i < camposPrevios; i++)
                {

                    switch (item.origen)
                    {
                        case Bitacoras.Util.BG_IHS_Origen.IHS:
                            styleCells[indexElemento, i] = styleIHS;
                            break;
                        case Bitacoras.Util.BG_IHS_Origen.USER:
                            styleCells[indexElemento, i] = styleUser;
                            break;
                        default:
                            styleCells[indexElemento, i] = oSLDocument.CreateStyle();
                            break;

                    }
                    indexColumna++;
                }

                #endregion

                #region meses
                int indexCabecera = 0;
                // 1. Obtiene la demanda para ESTE item específico desde el lookup en memoria (esto es instantáneo).
                var demandasParaEsteItem = demandasAgrupadas[item.id];

                // 2. Pasa esa lista precargada al método. Ahora no hará ninguna consulta a la base de datos.
                List<BG_IHS_rel_demanda> demandaMeses = item.GetDemanda(cabeceraMeses, model.demanda, demandaPrecargada: demandasParaEsteItem);

                foreach (var item_demanda in demandaMeses)
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null)
                    {
                        row[cabeceraMeses[indexCabecera].text] = item_demanda.cantidad;

                        //agrega el estilo a la cabecera
                        switch (item_demanda.origen_datos)
                        {
                            case Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER:
                                styleCells[listado.IndexOf(item), indexColumna] = styleDemandaCustomer;
                                break;
                            case Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL:
                                styleCells[listado.IndexOf(item), indexColumna] = styleDemandaOriginal;
                                break;
                            default:
                                styleCells[listado.IndexOf(item), indexColumna] = oSLDocument.CreateStyle();
                                break;
                        }
                    }
                    else
                    {
                        row[cabeceraMeses[indexCabecera].text] = DBNull.Value;
                        styleCells[listado.IndexOf(item), indexColumna] = oSLDocument.CreateStyle();
                    }


                    //guarda una referencia a la columna que contiene el año y mes
                    if (indexElemento == 0) //solo agrega en caso del primer item
                        referenciaColumnas.Add(new ReferenciaColumna
                        {
                            celdaReferencia = GetCellReference(indexColumna + 2), // +2 para coincidir con la celda correcta
                            fecha = cabeceraMeses[indexCabecera].fecha.Value
                        });

                    indexColumna++;
                    indexCabecera++;
                }

                #endregion

                #region cuartos
                indexCabecera = 0;
                // 1. Obtiene los "cuartos" para ESTE item desde el lookup que creaste al inicio (rápido).
                var cuartosParaEsteItem = cuartosAgrupados[item.id];

                // 2. Llama a GetCuartos UNA SOLA VEZ, pasándole los datos precargados.
                var listaDeCuartos = item.GetCuartos(demandaMeses, cabeceraCuartos, model.demanda, cuartosPrecargados: cuartosParaEsteItem);

                foreach (var item_demanda in listaDeCuartos)
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null && item_demanda.cantidad != null)
                    {
                        row[cabeceraCuartos[indexCabecera].text] = item_demanda.cantidad;
                        //agrega el estilo a la cabecera
                        switch (item_demanda.origen_datos)
                        {
                            case Portal_2_0.Models.Enum_BG_origen_cuartos.Calculado:
                                styleCells[listado.IndexOf(item), indexColumna] = styleValorCalculado;
                                break;
                            case Portal_2_0.Models.Enum_BG_origen_cuartos.IHS:
                                styleCells[listado.IndexOf(item), indexColumna] = styleValorIHS;
                                break;
                            default:
                                styleCells[listado.IndexOf(item), indexColumna] = oSLDocument.CreateStyle();
                                break;
                        }

                    }
                    else
                    {
                        row[cabeceraCuartos[indexCabecera].text] = DBNull.Value;
                        styleCells[listado.IndexOf(item), indexColumna] = oSLDocument.CreateStyle();
                    }


                    indexColumna++;
                    indexCabecera++;
                }

                #endregion

                #region años
                indexCabecera = 0;
                foreach (var item_demanda in item.GetAnios(demandaMeses, cabeceraAnios, model.demanda, cuartosParaEsteItem))
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null && item_demanda.cantidad != null)
                    {
                        row[cabeceraAnios[indexCabecera].text] = item_demanda.cantidad;
                        //agrega el estilo a la cabecera
                        switch (item_demanda.origen_datos)
                        {
                            case Portal_2_0.Models.Enum_BG_origen_anios.Calculado:
                                styleCells[listado.IndexOf(item), indexColumna] = styleValorCalculado;
                                break;
                            case Portal_2_0.Models.Enum_BG_origen_anios.IHS:
                                styleCells[listado.IndexOf(item), indexColumna] = styleValorIHS;
                                break;
                            default:
                                styleCells[listado.IndexOf(item), indexColumna] = oSLDocument.CreateStyle();
                                break;
                        }
                    }
                    else
                    {
                        row[cabeceraAnios[indexCabecera].text] = DBNull.Value;
                        styleCells[listado.IndexOf(item), indexColumna] = oSLDocument.CreateStyle();
                    }

                    indexColumna++;
                    indexCabecera++;
                }

                #endregion
                #region años FY
                indexCabecera = 0;
                FYReference = indexColumna + 2;

                var datosAniosFY = item.GetAniosFY(demandaMeses, cabeceraAniosFY, model.demanda, cuartosParaEsteItem);
                listDatosRegionesFY.AddRange(datosAniosFY);

                foreach (var item_demanda in datosAniosFY)
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null && item_demanda.cantidad != null)
                    {
                        row[cabeceraAniosFY[indexCabecera].text] = item_demanda.cantidad;
                        //agrega el estilo a la cabecera
                        switch (item_demanda.origen_datos)
                        {
                            case Portal_2_0.Models.Enum_BG_origen_anios.Calculado:
                                styleCells[listado.IndexOf(item), indexColumna] = styleValorCalculado;
                                break;
                            case Portal_2_0.Models.Enum_BG_origen_anios.IHS:
                                styleCells[listado.IndexOf(item), indexColumna] = styleValorIHS;
                                break;
                            default:
                                styleCells[listado.IndexOf(item), indexColumna] = oSLDocument.CreateStyle();
                                break;
                        }
                    }
                    else
                    {
                        row[cabeceraAniosFY[indexCabecera].text] = DBNull.Value;
                        styleCells[listado.IndexOf(item), indexColumna] = oSLDocument.CreateStyle();
                    }

                    //guarda una referencia a la columna que contiene el año y mes
                    if (indexElemento == 0) //solo agrega en caso del primer item
                        referenciaColumnasFY.Add(new ReferenciaColumna
                        {
                            celdaReferencia = GetCellReference(indexColumna + 2), // +2 para coincidir con la celda correcta
                            fecha = new DateTime(item_demanda.anio, 1, 1)
                        });

                    indexColumna++;
                    indexCabecera++;
                }

                #endregion

                //agrega la filas
                dt.Rows.Add(row);

                indexElemento++; //aumenta el index de la fila
            }

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, hoja1);
            oSLDocument.SelectWorksheet(hoja1);
            oSLDocument.ImportDataTable(1, 2, dt, true);

            hubContext.Clients.All.recibirProgresoExcel(23, 23, 100, $"Aplicando estilos.");
            timer.Restart();

            foreach (BG_IHS_item item in listado)
            {
                int fila = listado.IndexOf(item) + 2;
                oSLDocument.SetCellValue(fila, 1, "SI");
            }

            //aplica el color de las celdas
            for (int a = 0; a < listado.Count; a++)
            {
                for (int b = 0; b < columnasStyles; b++)
                {
                    oSLDocument.SetCellStyle(a + 2, b + 2, styleCells[a, b]);
                }
            }

            oSLDocument.SetColumnStyle(33, 34, styleShortDate);
            oSLDocument.SetColumnStyle(59, stylePercent);


            //da estilo a la hoja de excel
            ////inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 4);

            oSLDocument.Filter(1, 1, 1, dt.Columns.Count + 1);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count + 1);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count + 1, styleWrap);

            oSLDocument.SetCellStyle(1, 1, 1, dt.Columns.Count + 1, styleHeader);
            oSLDocument.SetCellStyle(1, 1, 1, dt.Columns.Count + 1, styleHeaderFont);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);
            //oculta la columna de porcentaje
            oSLDocument.HideColumn(59);
            //oculta la primera columna (aplica)
            oSLDocument.HideColumn(1);
            oSLDocument.HideColumn(2);

            System.Diagnostics.Debug.WriteLine($"[TIMER] estilos aplicados: {timer.Elapsed.TotalSeconds:F2} segundos");
            timer.Restart();

            #endregion


            timer.Restart();

            #region Autos Modificados

            hubContext.Clients.All.recibirProgresoExcel(27, 27, 100, $"Iniciando Hoja Autos Modificados.");

            //copia la hoja1 a la hoja 2
            oSLDocument.SelectWorksheet("Aux"); //selecciona la hoja actual para no tener detalles a la hora de copiar la hoja actual
            oSLDocument.CopyWorksheet(hoja1, hoja2);

            oSLDocument.SelectWorksheet(hoja2);

            //muestra la columna de porcentaje
            oSLDocument.UnhideColumn(59);
            //oculta la primera columna
            oSLDocument.HideColumn(1, 2);
            oSLDocument.HideColumn(6, 13);
            oSLDocument.HideColumn(21, 23);
            oSLDocument.HideColumn(28, 30);
            oSLDocument.HideColumn(35, 58);

            //obtiene la fila con la del porcentaje
            int colPorcentaje = camposPrevios + 1;  //+1 por el campo de si y no
            int numValores = cabeceraMeses.Count + cabeceraCuartos.Count + cabeceraAnios.Count + cabeceraAniosFY.Count;
            string colRef = GetCellReference(colPorcentaje);

            int fila_actual = 0;

            for (int i = 2; true; i++) //es infinito hasta se rompe
            {
                //lee si tiene un valor en la fila
                if (!String.IsNullOrEmpty(oSLDocument.GetCellValueAsString(i, 1)))
                {
                    for (int j = 1; j <= numValores; j++)
                    {
                        int col = j + colPorcentaje;
                        oSLDocument.SetCellValue(i, col, "='" + hoja1 + "'!" + GetCellReference(col) + i + "*(1+" + colRef + i + ")");

                    }
                }
                else
                { //termina el for
                    fila_actual = i + 1;
                    break;
                }

            }

            //-- COMBINACION

            #region combinaciones
            hubContext.Clients.All.recibirProgresoExcel(31, 31, 100, $"Agregando Combinaciones.");

            oSLDocument.SetCellValue(fila_actual, 2, "COMBINACIONES");
            oSLDocument.MergeWorksheetCells(fila_actual, 2, fila_actual, 4);
            oSLDocument.SetCellStyle(fila_actual, 2, styleHeader);
            oSLDocument.SetCellStyle(fila_actual, 2, styleHeaderFont);

            //aumenta la fila actual
            fila_actual++;
            string porcentajeReferencia = GetCellReference(camposPrevios + 1);

            //agrega una combinación
            foreach (var combinacion in combinaciones)
            {
                hubContext.Clients.All.recibirProgresoExcel(35, 35, 100, $"Agregando Combinaciones ({combinaciones.IndexOf(combinacion) + 1}/{combinaciones.Count}).");

                int inicio_fila = fila_actual;


                oSLDocument.SetCellValue(fila_actual, 2, "C_" + combinacion.id);
                oSLDocument.SetCellValue(fila_actual, 3, "Combinación");
                oSLDocument.SetCellValue(fila_actual, 4, combinacion.vehicle);
                oSLDocument.SetCellValue(fila_actual, 5, combinacion.vehicle);
                oSLDocument.SetCellValue(fila_actual, 15, combinacion.production_plant);
                oSLDocument.SetCellValue(fila_actual, 24, combinacion.manufacturer_group);
                oSLDocument.SetCellValue(fila_actual, 25, combinacion.manufacturer_group);
                oSLDocument.SetCellValue(fila_actual, 27, combinacion.production_brand);
                oSLDocument.SetCellValue(fila_actual, 33, combinacion.sop_start_of_production.Value); //
                oSLDocument.SetCellValue(fila_actual, 34, combinacion.eop_end_of_production.Value); //
                oSLDocument.SetCellValue(fila_actual, 59, combinacion.porcentaje_scrap.Value); //

                //agrega el estilo a la cabecera
                oSLDocument.SetCellStyle(fila_actual, 1, fila_actual, columnasStyles + 1, styleTituloCombinacion);

                //crea las sumatoria
                for (int i = camposPrevios + 2; i < camposPrevios + 2 + (cabeceraMeses.Count + cabeceraCuartos.Count + cabeceraAnios.Count + cabeceraAniosFY.Count); i++)
                {
                    string celdaRef = GetCellReference(i);
                    oSLDocument.SetCellValue(fila_actual, i, "=SUM(" + celdaRef + (fila_actual + 1) + ":" + celdaRef + (fila_actual + combinacion.BG_IHS_rel_combinacion.Count) + ")");
                }

                fila_actual++;
                //sumar totales (con formula)
                dt = new System.Data.DataTable();

                //columnas          
                #region cabecera
                dt.Columns.Add("Id", typeof(string));                       //1
                dt.Columns.Add("Origen", typeof(string));                   //1
                dt.Columns.Add("Vehicle (IHS)", typeof(string));                   //1
                dt.Columns.Add("Vehicle (Compuesto)", typeof(string));                   //1
                dt.Columns.Add("Core Nameplate Region Mnemonic", typeof(string));                   //1
                dt.Columns.Add("Core Nameplate Plant Mnemonic", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Vehicle", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Vehicle/Plant", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Platform", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Plant", typeof(string));                   //1
                dt.Columns.Add("Region", typeof(string));                   //1
                dt.Columns.Add("Market", typeof(string));                   //1
                dt.Columns.Add("Country/Territory", typeof(string));                   //1
                dt.Columns.Add("Production Plant", typeof(string));                   //1
                dt.Columns.Add("Region(Plant)", typeof(string));                   //1
                dt.Columns.Add("City", typeof(string));                   //1
                dt.Columns.Add("Plant State/Province", typeof(string));                   //1
                dt.Columns.Add("Source Plant", typeof(string));                   //1
                dt.Columns.Add("Source Plant Country/Territory", typeof(string));                   //1
                dt.Columns.Add("Source Plant Region", typeof(string));                   //1
                dt.Columns.Add("Design Parent", typeof(string));                   //1
                dt.Columns.Add("Engineering Group", typeof(string));                   //1
                dt.Columns.Add("Manufacturer Group", typeof(string));                   //1
                dt.Columns.Add("Manufacturer", typeof(string));                   //1
                dt.Columns.Add("Sales Parent", typeof(string));                   //1
                dt.Columns.Add("Production Brand", typeof(string));                   //1
                dt.Columns.Add("Platform Design Owner", typeof(string));                   //1
                dt.Columns.Add("Architecture", typeof(string));                   //1
                dt.Columns.Add("Platform", typeof(string));                   //1
                dt.Columns.Add("Program", typeof(string));                   //1
                dt.Columns.Add("Production Nameplate", typeof(string));                   //1
                dt.Columns.Add("SOP (Start of Production)", typeof(DateTime));                   //1
                dt.Columns.Add("EOP (End of Production)", typeof(DateTime));                   //1
                dt.Columns.Add("Lifecycle (Time)", typeof(string));                   //1
                dt.Columns.Add("Assembly Type", typeof(string));                   //1
                dt.Columns.Add("Strategic Group", typeof(string));                   //1
                dt.Columns.Add("Sales Group", typeof(string));                   //1
                dt.Columns.Add("Global Nameplate", typeof(string));                   //1
                dt.Columns.Add("Primary Design Center", typeof(string));                   //1
                dt.Columns.Add("Primary Design Country/Territory", typeof(string));                   //1
                dt.Columns.Add("Primary Design Region", typeof(string));                   //1
                dt.Columns.Add("Secondary Design Center", typeof(string));                   //1
                dt.Columns.Add("Secondary Design Country/Territory	", typeof(string));                   //1
                dt.Columns.Add("Secondary Design Region", typeof(string));                   //1
                dt.Columns.Add("GVW Rating", typeof(string));                   //1
                dt.Columns.Add("GVW Class", typeof(string));                   //1
                dt.Columns.Add("Car/Truck", typeof(string));                   //1
                dt.Columns.Add("Production Type", typeof(string));                   //1
                dt.Columns.Add("Global Production Segment", typeof(string));                   //1
                dt.Columns.Add("Flat Rolled Steel Usage", typeof(string));                   //1
                dt.Columns.Add("Regional Sales Segment", typeof(string));                   //1
                dt.Columns.Add("Global Production Price Class", typeof(string));                   //1
                dt.Columns.Add("Global Sales Segment", typeof(string));                   //1
                dt.Columns.Add("Global Sales Sub-Segment", typeof(string));                   //1
                dt.Columns.Add("Global Sales Price Class", typeof(string));                   //1
                dt.Columns.Add("Short-Term Risk Rating", typeof(string));                   //1
                dt.Columns.Add("Long-Term Risk Rating", typeof(string));                   //1
                dt.Columns.Add("Porcentaje scrap", typeof(decimal));                  //1   
                                                                                      //agrega cabecera meses
                foreach (var c in cabeceraMeses)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                                                                             //agrega cabecera cuartos
                foreach (var c in cabeceraCuartos)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                                                                             //agrega cabecera años ene-dic
                foreach (var c in cabeceraAnios)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                                                                             //agrega cabecera fy
                foreach (var c in cabeceraAniosFY)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                #endregion

                foreach (var c_item in combinacion.BG_IHS_rel_combinacion)
                {
                    //crea row
                    System.Data.DataRow row = dt.NewRow();

                    // 1. Obtiene el porcentaje de demanda específico para este ítem.
                    decimal porcentajeItem = c_item.porcentaje_aplicado;
                    // 2. Lo convierte en un factor de cálculo para usar en la fórmula (ej. 80% -> 0.80).
                    float factorAjusteItem = (float)porcentajeItem / 100.0f;

                    #region valores
                    row["Id"] = c_item.BG_IHS_item.id;
                    row["Origen"] = c_item.BG_IHS_item.origen;
                    row["Vehicle (IHS)"] = c_item.BG_IHS_item.vehicle;
                    row["Vehicle (Compuesto)"] = c_item.BG_IHS_item.ConcatCodigo;
                    row["Core Nameplate Region Mnemonic"] = c_item.BG_IHS_item.core_nameplate_region_mnemonic;
                    row["Core Nameplate Plant Mnemonic"] = c_item.BG_IHS_item.core_nameplate_plant_mnemonic;
                    row["Mnemonic-Vehicle"] = c_item.BG_IHS_item.mnemonic_vehicle;
                    row["Mnemonic-Vehicle/Plant"] = c_item.BG_IHS_item.mnemonic_vehicle_plant;
                    row["Mnemonic-Platform"] = c_item.BG_IHS_item.mnemonic_platform;
                    row["Mnemonic-Plant"] = c_item.BG_IHS_item.mnemonic_plant;
                    row["Region"] = c_item.BG_IHS_item.region;
                    row["Market"] = c_item.BG_IHS_item.market;
                    row["Country/Territory"] = c_item.BG_IHS_item.country_territory;
                    row["Production Plant"] = c_item.BG_IHS_item.production_plant;
                    row["Region(Plant)"] = c_item.BG_IHS_item._Region != null ? c_item.BG_IHS_item._Region.descripcion : null;
                    row["City"] = c_item.BG_IHS_item.city;
                    row["Plant State/Province"] = c_item.BG_IHS_item.plant_state_province;
                    row["Source Plant"] = c_item.BG_IHS_item.source_plant;
                    row["Source Plant Country/Territory"] = c_item.BG_IHS_item.source_plant_country_territory;
                    row["Source Plant Region"] = c_item.BG_IHS_item.source_plant_region;
                    row["Design Parent"] = c_item.BG_IHS_item.design_parent;
                    row["Engineering Group"] = c_item.BG_IHS_item.engineering_group;
                    row["Manufacturer Group"] = c_item.BG_IHS_item.manufacturer_group;
                    row["Manufacturer"] = c_item.BG_IHS_item.manufacturer;
                    row["Sales Parent"] = c_item.BG_IHS_item.sales_parent;
                    row["Production Brand"] = c_item.BG_IHS_item.production_brand;
                    row["Platform Design Owner"] = c_item.BG_IHS_item.platform_design_owner;
                    row["Architecture"] = c_item.BG_IHS_item.architecture;
                    row["Platform"] = c_item.BG_IHS_item.platform;
                    row["Program"] = c_item.BG_IHS_item.program;
                    row["Production Nameplate"] = c_item.BG_IHS_item.production_nameplate;
                    row["SOP (Start of Production)"] = c_item.BG_IHS_item.sop_start_of_production;
                    row["EOP (End of Production)"] = c_item.BG_IHS_item.eop_end_of_production;
                    row["Lifecycle (Time)"] = c_item.BG_IHS_item.lifecycle_time;
                    row["Assembly Type"] = c_item.BG_IHS_item.assembly_type;
                    row["Strategic Group"] = c_item.BG_IHS_item.strategic_group;
                    row["Sales Group"] = c_item.BG_IHS_item.sales_group;
                    row["Global Nameplate"] = c_item.BG_IHS_item.global_nameplate;
                    row["Primary Design Center"] = c_item.BG_IHS_item.primary_design_center;
                    row["Primary Design Country/Territory"] = c_item.BG_IHS_item.primary_design_country_territory;
                    row["Primary Design Region"] = c_item.BG_IHS_item.primary_design_region;
                    row["Secondary Design Center"] = c_item.BG_IHS_item.secondary_design_center;
                    row["Secondary Design Country/Territory	"] = c_item.BG_IHS_item.secondary_design_country_territory;
                    row["Secondary Design Region"] = c_item.BG_IHS_item.secondary_design_region;
                    row["GVW Rating"] = c_item.BG_IHS_item.gvw_rating;
                    row["GVW Class"] = c_item.BG_IHS_item.gvw_class;
                    row["Car/Truck"] = c_item.BG_IHS_item.car_truck;
                    row["Production Type"] = c_item.BG_IHS_item.production_type;
                    row["Global Production Segment"] = c_item.BG_IHS_item.global_production_segment;
                    row["Flat Rolled Steel Usage"] = c_item.BG_IHS_item.RelSegmento != null ? c_item.BG_IHS_item.RelSegmento.flat_rolled_steel_usage : null;
                    row["Regional Sales Segment"] = c_item.BG_IHS_item.regional_sales_segment;
                    row["Global Production Price Class"] = c_item.BG_IHS_item.global_production_price_class;
                    row["Global Sales Segment"] = c_item.BG_IHS_item.global_sales_segment;
                    row["Global Sales Sub-Segment"] = c_item.BG_IHS_item.global_sales_sub_segment;
                    row["Global Sales Price Class"] = c_item.BG_IHS_item.global_sales_price_class;
                    row["Short-Term Risk Rating"] = c_item.BG_IHS_item.short_term_risk_rating;
                    row["Long-Term Risk Rating"] = c_item.BG_IHS_item.long_term_risk_rating;
                    row["Porcentaje scrap"] = DBNull.Value;
                    #region meses
                    int indexCabecera = 0;
                    int indexColumna = camposPrevios + 2;


                    List<BG_IHS_rel_demanda> demandaMeses = new List<BG_IHS_rel_demanda>(); // Inicializa como vacía

                    // Verificamos que el item exista antes de intentar obtener su demanda
                    if (c_item.BG_IHS_item != null)
                    {
                        // 1. Buscamos la demanda para el ID del item en nuestro lookup (rápido)
                        var demandasParaEsteItem = demandasAgrupadas[c_item.BG_IHS_item.id];

                        // 2. Pasamos los datos precargados a GetDemanda (sin acceso a BD)
                        demandaMeses = c_item.BG_IHS_item.GetDemanda(cabeceraMeses, model.demanda, demandaPrecargada: demandasParaEsteItem);
                    }

                    foreach (var item_demanda in demandaMeses)
                    {
                        //si no es nul agrega la cantidad
                        if (item_demanda != null)
                        {
                            float cantidadOriginal = item_demanda?.cantidad ?? 0;
                            // Se añade la multiplicación por el factorAjusteItem a la fórmula de Excel.
                            row[cabeceraMeses[indexCabecera].text] = $"={cantidadOriginal} * {factorAjusteItem} * (1+{porcentajeReferencia}{inicio_fila})";


                            //agrega el estilo a la cabecera
                            switch (item_demanda.origen_datos)
                            {
                                case Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleDemandaCustomer);
                                    break;
                                case Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleDemandaOriginal);
                                    break;

                            }
                        }
                        else
                        {
                            row[cabeceraMeses[indexCabecera].text] = DBNull.Value;
                        }
                        indexCabecera++;
                        indexColumna++;
                    }

                    #endregion

                    #region cuartos
                    indexCabecera = 0;

                    // Inicializamos una lista vacía para manejar el caso de que el item sea nulo.
                    var listaDeCuartos = new List<BG_IHS_rel_cuartos>();

                    // Buena práctica: Verificamos que el item relacionado no sea nulo.
                    if (c_item.BG_IHS_item != null)
                    {
                        // 1. Obtenemos los "cuartos" para este item desde el lookup en memoria.
                        var cuartosParaEsteItem = cuartosAgrupados[c_item.BG_IHS_item.id];

                        // 2. Llama a GetCuartos UNA VEZ, pasándole los datos precargados.
                        listaDeCuartos = c_item.BG_IHS_item.GetCuartos(demandaMeses, cabeceraCuartos, model.demanda, cuartosPrecargados: cuartosParaEsteItem);
                    }

                    foreach (var item_demanda in listaDeCuartos)
                    {
                        //si no es nul agrega la cantidad
                        if (item_demanda != null && item_demanda.cantidad != null)
                        {
                            float cantidadOriginal = item_demanda?.cantidad ?? 0;
                            // Se añade la multiplicación por el factorAjusteItem a la fórmula de Excel.
                            row[cabeceraMeses[indexCabecera].text] = $"={cantidadOriginal} * {factorAjusteItem} * (1+{porcentajeReferencia}{inicio_fila})";

                            //agrega el estilo a la cabecera
                            switch (item_demanda.origen_datos)
                            {
                                case Portal_2_0.Models.Enum_BG_origen_cuartos.Calculado:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorCalculado);
                                    break;
                                case Portal_2_0.Models.Enum_BG_origen_cuartos.IHS:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorIHS);
                                    break;
                            }

                        }
                        else
                        {
                            row[cabeceraCuartos[indexCabecera].text] = DBNull.Value;
                        }
                        indexColumna++;
                        indexCabecera++;
                    }

                    #endregion

                    #region años
                    indexCabecera = 0;

                    // Inicializamos una lista vacía para el resultado.
                    var listaDeAnios = new List<BG_IHS_item_anios>();

                    // Verificamos que el item relacionado no sea nulo.
                    if (c_item.BG_IHS_item != null)
                    {
                        // 1. Obtenemos los "cuartos" para este item desde el lookup en memoria.
                        var cuartosParaEsteItem = cuartosAgrupados[c_item.BG_IHS_item.id];

                        // 2. Llama a GetAnios UNA VEZ, pasándole los datos precargados.
                        listaDeAnios = c_item.BG_IHS_item.GetAnios(demandaMeses, cabeceraAnios, model.demanda, cuartosPrecargados: cuartosParaEsteItem);
                    }

                    foreach (var item_demanda in listaDeAnios)
                    {
                        //si no es nul agrega la cantidad
                        if (item_demanda != null && item_demanda.cantidad != null)
                        {
                            float cantidadOriginal = item_demanda?.cantidad ?? 0;
                            // Se añade la multiplicación por el factorAjusteItem a la fórmula de Excel.
                            row[cabeceraMeses[indexCabecera].text] = $"={cantidadOriginal} * {factorAjusteItem} * (1+{porcentajeReferencia}{inicio_fila})";
                            //agrega el estilo a la cabecera
                            switch (item_demanda.origen_datos)
                            {
                                case Portal_2_0.Models.Enum_BG_origen_anios.Calculado:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorCalculado);
                                    break;
                                case Portal_2_0.Models.Enum_BG_origen_anios.IHS:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorIHS);
                                    break;
                            }
                        }
                        else
                        {
                            row[cabeceraAnios[indexCabecera].text] = DBNull.Value;
                        }

                        indexColumna++;
                        indexCabecera++;
                    }

                    #endregion
                    #region años FY
                    indexCabecera = 0;
                    //FYReference = indexColumna + 2;


                    var datosAniosFY = new List<BG_IHS_item_anios>(); // Inicializamos como lista vacía

                    // Verificamos que el item relacionado no sea nulo.
                    if (c_item.BG_IHS_item != null)
                    {
                        // 1. Obtenemos los "cuartos" para este item desde el lookup en memoria.
                        var cuartosParaEsteItem = cuartosAgrupados[c_item.BG_IHS_item.id];

                        // 2. Llama a GetAniosFY UNA VEZ, pasándole los datos precargados.
                        datosAniosFY = c_item.BG_IHS_item.GetAniosFY(demandaMeses, cabeceraAniosFY, model.demanda, cuartosPrecargados: cuartosParaEsteItem);
                    }

                    listDatosRegionesFY.AddRange(datosAniosFY);

                    foreach (var item_demanda in datosAniosFY)
                    {
                        //si no es nul agrega la cantidad
                        if (item_demanda != null && item_demanda.cantidad != null)
                        {
                            float cantidadOriginal = item_demanda?.cantidad ?? 0;
                            // Se añade la multiplicación por el factorAjusteItem a la fórmula de Excel.
                            row[cabeceraMeses[indexCabecera].text] = $"={cantidadOriginal} * {factorAjusteItem} * (1+{porcentajeReferencia}{inicio_fila})";
                            //agrega el estilo a la cabecera
                            switch (item_demanda.origen_datos)
                            {
                                case Portal_2_0.Models.Enum_BG_origen_anios.Calculado:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorCalculado);
                                    break;
                                case Portal_2_0.Models.Enum_BG_origen_anios.IHS:
                                    oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorIHS);
                                    break;

                            }
                        }
                        else
                        {
                            row[cabeceraAniosFY[indexCabecera].text] = DBNull.Value;
                        }

                        indexColumna++;
                        indexCabecera++;
                    }

                    #endregion

                    #endregion

                    //aplica el estilo a los primeros campos
                    switch (c_item.BG_IHS_item.origen)
                    {
                        case Bitacoras.Util.BG_IHS_Origen.IHS:
                            oSLDocument.SetCellStyle(fila_actual, 2, fila_actual, camposPrevios + 1, styleIHS);
                            break;
                        case Bitacoras.Util.BG_IHS_Origen.USER:
                            oSLDocument.SetCellStyle(fila_actual, 2, fila_actual, camposPrevios + 1, styleUser);
                            break;

                    }
                    //agrega la filas
                    dt.Rows.Add(row);
                    fila_actual++;
                }

                oSLDocument.ImportDataTable(fila_actual - combinacion.BG_IHS_rel_combinacion.Count, 2, dt, false);

            }

            #endregion

            // -- DIVISIONES
            #region divisiones

            fila_actual++;

            oSLDocument.SetCellValue(fila_actual, 2, "DIVISIONES");
            oSLDocument.MergeWorksheetCells(fila_actual, 2, fila_actual, 4);
            oSLDocument.SetCellStyle(fila_actual, 2, styleHeader);
            oSLDocument.SetCellStyle(fila_actual, 2, styleHeaderFont);

            fila_actual++;

            hubContext.Clients.All.recibirProgresoExcel(40, 40, 100, $"Agregando Divisiones.");

            foreach (var division in divisiones)
            {
                hubContext.Clients.All.recibirProgresoExcel(43, 43, 100, $"Agregando Divisiones ({divisiones.IndexOf(division) + 1}/{divisiones.Count}).");

                int inicio_fila = fila_actual;

                dt = new System.Data.DataTable();
                //columnas          
                #region cabecera
                dt.Columns.Add("Id", typeof(string));                       //1
                dt.Columns.Add("Origen", typeof(string));                   //1
                dt.Columns.Add("Vehicle (IHS)", typeof(string));                   //1
                dt.Columns.Add("Vehicle (Compuesto)", typeof(string));                   //1
                dt.Columns.Add("Core Nameplate Region Mnemonic", typeof(string));                   //1
                dt.Columns.Add("Core Nameplate Plant Mnemonic", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Vehicle", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Vehicle/Plant", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Platform", typeof(string));                   //1
                dt.Columns.Add("Mnemonic-Plant", typeof(string));                   //1
                dt.Columns.Add("Region", typeof(string));                   //1
                dt.Columns.Add("Market", typeof(string));                   //1
                dt.Columns.Add("Country/Territory", typeof(string));                   //1
                dt.Columns.Add("Production Plant", typeof(string));                   //1
                dt.Columns.Add("Region(Plant)", typeof(string));                   //1
                dt.Columns.Add("City", typeof(string));                   //1
                dt.Columns.Add("Plant State/Province", typeof(string));                   //1
                dt.Columns.Add("Source Plant", typeof(string));                   //1
                dt.Columns.Add("Source Plant Country/Territory", typeof(string));                   //1
                dt.Columns.Add("Source Plant Region", typeof(string));                   //1
                dt.Columns.Add("Design Parent", typeof(string));                   //1
                dt.Columns.Add("Engineering Group", typeof(string));                   //1
                dt.Columns.Add("Manufacturer Group", typeof(string));                   //1
                dt.Columns.Add("Manufacturer", typeof(string));                   //1
                dt.Columns.Add("Sales Parent", typeof(string));                   //1
                dt.Columns.Add("Production Brand", typeof(string));                   //1
                dt.Columns.Add("Platform Design Owner", typeof(string));                   //1
                dt.Columns.Add("Architecture", typeof(string));                   //1
                dt.Columns.Add("Platform", typeof(string));                   //1
                dt.Columns.Add("Program", typeof(string));                   //1
                dt.Columns.Add("Production Nameplate", typeof(string));                   //1
                dt.Columns.Add("SOP (Start of Production)", typeof(DateTime));                   //1
                dt.Columns.Add("EOP (End of Production)", typeof(DateTime));                   //1
                dt.Columns.Add("Lifecycle (Time)", typeof(string));                   //1
                dt.Columns.Add("Assembly Type", typeof(string));                   //1
                dt.Columns.Add("Strategic Group", typeof(string));                   //1
                dt.Columns.Add("Sales Group", typeof(string));                   //1
                dt.Columns.Add("Global Nameplate", typeof(string));                   //1
                dt.Columns.Add("Primary Design Center", typeof(string));                   //1
                dt.Columns.Add("Primary Design Country/Territory", typeof(string));                   //1
                dt.Columns.Add("Primary Design Region", typeof(string));                   //1
                dt.Columns.Add("Secondary Design Center", typeof(string));                   //1
                dt.Columns.Add("Secondary Design Country/Territory	", typeof(string));                   //1
                dt.Columns.Add("Secondary Design Region", typeof(string));                   //1
                dt.Columns.Add("GVW Rating", typeof(string));                   //1
                dt.Columns.Add("GVW Class", typeof(string));                   //1
                dt.Columns.Add("Car/Truck", typeof(string));                   //1
                dt.Columns.Add("Production Type", typeof(string));                   //1
                dt.Columns.Add("Global Production Segment", typeof(string));                   //1
                dt.Columns.Add("Flat Rolled Steel Usage", typeof(string));                   //1
                dt.Columns.Add("Regional Sales Segment", typeof(string));                   //1
                dt.Columns.Add("Global Production Price Class", typeof(string));                   //1
                dt.Columns.Add("Global Sales Segment", typeof(string));                   //1
                dt.Columns.Add("Global Sales Sub-Segment", typeof(string));                   //1
                dt.Columns.Add("Global Sales Price Class", typeof(string));                   //1
                dt.Columns.Add("Short-Term Risk Rating", typeof(string));                   //1
                dt.Columns.Add("Long-Term Risk Rating", typeof(string));                   //1
                dt.Columns.Add("Porcentaje scrap", typeof(decimal));                  //1   
                                                                                      //agrega cabecera meses
                foreach (var c in cabeceraMeses)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                                                                             //agrega cabecera cuartos
                foreach (var c in cabeceraCuartos)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                                                                             //agrega cabecera años ene-dic
                foreach (var c in cabeceraAnios)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                                                                             //agrega cabecera fy
                foreach (var c in cabeceraAniosFY)
                    dt.Columns.Add(c.text, typeof(string));                  //1
                #endregion


                //crea row
                System.Data.DataRow row = dt.NewRow();

                #region valores
                row["Id"] = division.BG_IHS_item.id;
                row["Origen"] = division.BG_IHS_item.origen;
                row["Vehicle (IHS)"] = division.BG_IHS_item.vehicle;
                row["Vehicle (Compuesto)"] = division.BG_IHS_item.ConcatCodigo;
                row["Core Nameplate Region Mnemonic"] = division.BG_IHS_item.core_nameplate_region_mnemonic;
                row["Core Nameplate Plant Mnemonic"] = division.BG_IHS_item.core_nameplate_plant_mnemonic;
                row["Mnemonic-Vehicle"] = division.BG_IHS_item.mnemonic_vehicle;
                row["Mnemonic-Vehicle/Plant"] = division.BG_IHS_item.mnemonic_vehicle_plant;
                row["Mnemonic-Platform"] = division.BG_IHS_item.mnemonic_platform;
                row["Mnemonic-Plant"] = division.BG_IHS_item.mnemonic_plant;
                row["Region"] = division.BG_IHS_item.region;
                row["Market"] = division.BG_IHS_item.market;
                row["Country/Territory"] = division.BG_IHS_item.country_territory;
                row["Production Plant"] = division.BG_IHS_item.production_plant;
                row["Region(Plant)"] = division.BG_IHS_item._Region != null ? division.BG_IHS_item._Region.descripcion : null;
                row["City"] = division.BG_IHS_item.city;
                row["Plant State/Province"] = division.BG_IHS_item.plant_state_province;
                row["Source Plant"] = division.BG_IHS_item.source_plant;
                row["Source Plant Country/Territory"] = division.BG_IHS_item.source_plant_country_territory;
                row["Source Plant Region"] = division.BG_IHS_item.source_plant_region;
                row["Design Parent"] = division.BG_IHS_item.design_parent;
                row["Engineering Group"] = division.BG_IHS_item.engineering_group;
                row["Manufacturer Group"] = division.BG_IHS_item.manufacturer_group;
                row["Manufacturer"] = division.BG_IHS_item.manufacturer;
                row["Sales Parent"] = division.BG_IHS_item.sales_parent;
                row["Production Brand"] = division.BG_IHS_item.production_brand;
                row["Platform Design Owner"] = division.BG_IHS_item.platform_design_owner;
                row["Architecture"] = division.BG_IHS_item.architecture;
                row["Platform"] = division.BG_IHS_item.platform;
                row["Program"] = division.BG_IHS_item.program;
                row["Production Nameplate"] = division.BG_IHS_item.production_nameplate;
                row["SOP (Start of Production)"] = division.BG_IHS_item.sop_start_of_production;
                row["EOP (End of Production)"] = division.BG_IHS_item.eop_end_of_production;
                row["Lifecycle (Time)"] = division.BG_IHS_item.lifecycle_time;
                row["Assembly Type"] = division.BG_IHS_item.assembly_type;
                row["Strategic Group"] = division.BG_IHS_item.strategic_group;
                row["Sales Group"] = division.BG_IHS_item.sales_group;
                row["Global Nameplate"] = division.BG_IHS_item.global_nameplate;
                row["Primary Design Center"] = division.BG_IHS_item.primary_design_center;
                row["Primary Design Country/Territory"] = division.BG_IHS_item.primary_design_country_territory;
                row["Primary Design Region"] = division.BG_IHS_item.primary_design_region;
                row["Secondary Design Center"] = division.BG_IHS_item.secondary_design_center;
                row["Secondary Design Country/Territory	"] = division.BG_IHS_item.secondary_design_country_territory;
                row["Secondary Design Region"] = division.BG_IHS_item.secondary_design_region;
                row["GVW Rating"] = division.BG_IHS_item.gvw_rating;
                row["GVW Class"] = division.BG_IHS_item.gvw_class;
                row["Car/Truck"] = division.BG_IHS_item.car_truck;
                row["Production Type"] = division.BG_IHS_item.production_type;
                row["Global Production Segment"] = division.BG_IHS_item.global_production_segment;
                row["Flat Rolled Steel Usage"] = division.BG_IHS_item.RelSegmento != null ? division.BG_IHS_item.RelSegmento.flat_rolled_steel_usage : null;
                row["Regional Sales Segment"] = division.BG_IHS_item.regional_sales_segment;
                row["Global Production Price Class"] = division.BG_IHS_item.global_production_price_class;
                row["Global Sales Segment"] = division.BG_IHS_item.global_sales_segment;
                row["Global Sales Sub-Segment"] = division.BG_IHS_item.global_sales_sub_segment;
                row["Global Sales Price Class"] = division.BG_IHS_item.global_sales_price_class;
                row["Short-Term Risk Rating"] = division.BG_IHS_item.short_term_risk_rating;
                row["Long-Term Risk Rating"] = division.BG_IHS_item.long_term_risk_rating;
                row["Porcentaje scrap"] = division.porcentaje_scrap;
                #region meses
                int indexCabecera = 0;
                int indexColumna = camposPrevios + 2;

                List<BG_IHS_rel_demanda> demandaMeses = new List<BG_IHS_rel_demanda>(); // Inicializa como lista vacía

                // Buena práctica: asegúrate de que el item anidado no sea nulo
                if (division.BG_IHS_item != null)
                {
                    // 1. Busca la demanda para el ID de este item en el lookup (acceso en memoria)
                    var demandasParaEsteItem = demandasAgrupadas[division.BG_IHS_item.id];

                    // 2. Llama a GetDemanda pasándole los datos precargados. ¡No hay consulta a BD!
                    demandaMeses = division.BG_IHS_item.GetDemanda(cabeceraMeses, model.demanda, demandaPrecargada: demandasParaEsteItem);
                }

                foreach (var item_demanda in demandaMeses)
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null)
                    {
                        //row[cabeceraMeses[indexCabecera].text] = item_demanda.cantidad;
                        //= 250 * (1 + BG11)
                        row[cabeceraMeses[indexCabecera].text] = "=" + (item_demanda.cantidad != null ? item_demanda.cantidad.Value : 0) + "*(1+" + porcentajeReferencia + inicio_fila + ")";

                        //agrega el estilo a la cabecera
                        // coloca el texto en negritas
                        oSLDocument.SetCellStyle(fila_actual, indexColumna, styleBoldBlue);
                        switch (item_demanda.origen_datos)
                        {
                            case Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleDemandaCustomer);
                                break;
                            case Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleDemandaOriginal);
                                break;

                        }

                    }
                    else
                    {
                        row[cabeceraMeses[indexCabecera].text] = DBNull.Value;
                    }
                    indexCabecera++;
                    indexColumna++;
                }

                #endregion

                #region cuartos
                indexCabecera = 0;

                // Inicializamos una lista vacía para manejar el caso de que el item sea nulo.
                var listaDeCuartos = new List<BG_IHS_rel_cuartos>();

                // Verificamos que el item relacionado no sea nulo.
                if (division.BG_IHS_item != null)
                {
                    // 1. Obtenemos los "cuartos" para este item desde el lookup en memoria.
                    var cuartosParaEsteItem = cuartosAgrupados[division.BG_IHS_item.id];

                    // 2. Llama a GetCuartos UNA VEZ, pasándole los datos precargados.
                    listaDeCuartos = division.BG_IHS_item.GetCuartos(demandaMeses, cabeceraCuartos, model.demanda, cuartosPrecargados: cuartosParaEsteItem);
                }

                // 3. Ahora itera sobre la lista que ya tienes en memoria.
                foreach (var item_demanda in listaDeCuartos)
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null && item_demanda.cantidad != null)
                    {
                        row[cabeceraCuartos[indexCabecera].text] = "=" + (item_demanda.cantidad != null ? item_demanda.cantidad.Value : 0) + "*(1+" + porcentajeReferencia + inicio_fila + ")";

                        //agrega el estilo a la cabecera
                        // coloca el texto en negritas
                        oSLDocument.SetCellStyle(fila_actual, indexColumna, styleBoldBlue);
                        switch (item_demanda.origen_datos)
                        {
                            case Portal_2_0.Models.Enum_BG_origen_cuartos.Calculado:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorCalculado);
                                break;
                            case Portal_2_0.Models.Enum_BG_origen_cuartos.IHS:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorIHS);
                                break;
                        }
                    }
                    else
                    {
                        row[cabeceraCuartos[indexCabecera].text] = DBNull.Value;
                    }
                    indexColumna++;
                    indexCabecera++;
                }

                #endregion

                #region años
                indexCabecera = 0;
                foreach (var item_demanda in division.BG_IHS_item.GetAnios(demandaMeses, cabeceraAnios, model.demanda))
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null && item_demanda.cantidad != null)
                    {
                        row[cabeceraAnios[indexCabecera].text] = "=" + (item_demanda.cantidad != null ? item_demanda.cantidad.Value : 0) + "*(1+" + porcentajeReferencia + inicio_fila + ")";

                        //agrega el estilo a la cabecera
                        // coloca el texto en negritas
                        oSLDocument.SetCellStyle(fila_actual, indexColumna, styleBoldBlue);
                        switch (item_demanda.origen_datos)
                        {
                            case Portal_2_0.Models.Enum_BG_origen_anios.Calculado:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorCalculado);
                                break;
                            case Portal_2_0.Models.Enum_BG_origen_anios.IHS:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorIHS);
                                break;
                        }

                    }
                    else
                    {
                        row[cabeceraAnios[indexCabecera].text] = DBNull.Value;
                    }

                    indexColumna++;
                    indexCabecera++;
                }

                #endregion
                #region años FY
                indexCabecera = 0;
                //FYReference = indexColumna + 2;

                var datosAniosFY = new List<BG_IHS_item_anios>(); // Inicializamos como lista vacía

                // Verificamos que el item relacionado no sea nulo.
                if (division.BG_IHS_item != null)
                {
                    // 1. Obtenemos los "cuartos" para este item desde el lookup en memoria.
                    var cuartosParaEsteItem = cuartosAgrupados[division.BG_IHS_item.id];

                    // 2. Llama a GetAniosFY UNA VEZ, pasándole los datos precargados.
                    datosAniosFY = division.BG_IHS_item.GetAniosFY(demandaMeses, cabeceraAniosFY, model.demanda, cuartosPrecargados: cuartosParaEsteItem);
                }

                listDatosRegionesFY.AddRange(datosAniosFY);

                foreach (var item_demanda in datosAniosFY)
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null && item_demanda.cantidad != null)
                    {
                        row[cabeceraAniosFY[indexCabecera].text] = "=" + (item_demanda.cantidad != null ? item_demanda.cantidad.Value : 0) + "*(1+" + porcentajeReferencia + inicio_fila + ")";

                        //agrega el estilo a la cabecera
                        // coloca el texto en negritas
                        oSLDocument.SetCellStyle(fila_actual, indexColumna, styleBoldBlue);
                        switch (item_demanda.origen_datos)
                        {
                            case Portal_2_0.Models.Enum_BG_origen_anios.Calculado:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorCalculado);
                                break;
                            case Portal_2_0.Models.Enum_BG_origen_anios.IHS:
                                oSLDocument.SetCellStyle(fila_actual, indexColumna, styleValorIHS);
                                break;

                        }

                    }
                    else
                    {
                        row[cabeceraAniosFY[indexCabecera].text] = DBNull.Value;
                    }

                    indexColumna++;
                    indexCabecera++;
                }

                #endregion

                #endregion

                // coloca el texto en negritas
                oSLDocument.SetCellStyle(fila_actual, 2, fila_actual, camposPrevios + 1, styleBoldBlue);
                //aplica el estilo a los primeros campos
                switch (division.BG_IHS_item.origen)
                {
                    case Bitacoras.Util.BG_IHS_Origen.IHS:
                        oSLDocument.SetCellStyle(fila_actual, 2, fila_actual, camposPrevios + 1, styleIHS);
                        break;
                    case Bitacoras.Util.BG_IHS_Origen.USER:
                        oSLDocument.SetCellStyle(fila_actual, 2, fila_actual, camposPrevios + 1, styleUser);
                        break;

                }


                //agrega la filas
                dt.Rows.Add(row);
                //agrega la tabla
                oSLDocument.ImportDataTable(fila_actual, 2, dt, false);

                fila_actual++;

                int fila_porcentaje_division = 31;
                string porcentajeDivisionReferencia = GetCellReference(fila_porcentaje_division);

                foreach (var rel in division.BG_IHS_rel_division)
                {
                    //aplica
                    oSLDocument.SetCellValue(fila_actual, 2, "D_" + rel.id);
                    oSLDocument.SetCellValue(fila_actual, 4, rel.vehicle);
                    oSLDocument.SetCellValue(fila_actual, 5, rel.vehicle);
                    oSLDocument.SetCellValue(fila_actual, 31, rel.porcentaje.Value);
                    oSLDocument.SetCellStyle(fila_actual, fila_porcentaje_division, stylePercent);
                    oSLDocument.SetCellValue(fila_actual, 32, rel.production_nameplate);

                    //agrega la formula
                    for (int i = camposPrevios + 2; i < camposPrevios + 2 + (cabeceraMeses.Count + cabeceraCuartos.Count + cabeceraAnios.Count + cabeceraAniosFY.Count); i++)
                    {
                        string celdaRef = GetCellReference(i);
                        oSLDocument.SetCellValue(fila_actual, i, "=" + celdaRef + inicio_fila + "*" + porcentajeDivisionReferencia + fila_actual);
                    }
                    fila_actual++;



                }

            }


            #endregion

            //-- HECHIZOS

            #region hechizos

            hubContext.Clients.All.recibirProgresoExcel(47, 47, 100, $"Agregando Hechizos.");

            fila_actual++; //aumenta la fila, para dejar separacion con divisones
            oSLDocument.SetCellValue(fila_actual, 2, "HECHIZOS");
            oSLDocument.MergeWorksheetCells(fila_actual, 2, fila_actual, 4);
            oSLDocument.SetCellStyle(fila_actual, 2, styleHeader);
            oSLDocument.SetCellStyle(fila_actual, 2, styleHeaderFont);

            //aumenta la fila actual
            fila_actual++;
            porcentajeReferencia = GetCellReference(camposPrevios + 1);

            //Agrega las cabeceras al d
            dt = new System.Data.DataTable();


            foreach (var c in cabeceraMeses)
                dt.Columns.Add(c.text, typeof(string));                  //1
                                                                         //agrega cabecera cuartos
            foreach (var c in cabeceraCuartos)
                dt.Columns.Add(c.text, typeof(string));                  //1
                                                                         //agrega cabecera años ene-dic
            foreach (var c in cabeceraAnios)
                dt.Columns.Add(c.text, typeof(string));                  //1
                                                                         //agrega cabecera fy
            foreach (var c in cabeceraAniosFY)
                dt.Columns.Add(c.text, typeof(string));

            //agrega un hechizo
            foreach (var hechizo in hechizos)
            {
                hubContext.Clients.All.recibirProgresoExcel(52, 52, 100, $"Agregando Hechizos ({hechizos.IndexOf(hechizo) + 1}/{hechizos.Count}).");

                int inicio_fila = fila_actual;

                System.Data.DataRow row = dt.NewRow();

                //valores sin tabla
                oSLDocument.SetCellValue(fila_actual, 2, "H_" + hechizo.Id);
                oSLDocument.SetCellValue(fila_actual, 3, "Hechizo");
                oSLDocument.SetCellValue(fila_actual, 4, hechizo.Vehicle);
                oSLDocument.SetCellValue(fila_actual, 5, hechizo.Vehicle);
                oSLDocument.SetCellValue(fila_actual, 15, hechizo.ProductionPlant);
                oSLDocument.SetCellValue(fila_actual, 24, hechizo.ManufacturerGroup);
                oSLDocument.SetCellValue(fila_actual, 25, hechizo.ManufacturerGroup);
                //oSLDocument.SetCellValue(fila_actual, 27, hechizo.production_brand);
                oSLDocument.SetCellValue(fila_actual, 33, hechizo.sop_start_of_production); //
                oSLDocument.SetCellValue(fila_actual, 34, hechizo.eop_end_of_production); //
                oSLDocument.SetCellValue(fila_actual, 59, .0); //0 % por defecto

                //agrega el estilo a la cabecera
                oSLDocument.SetCellStyle(fila_actual, 1, fila_actual, columnasStyles + 1, styleTituloCombinacion);


                #region Meses

                List<BG_IHS_custom_rel_demanda> demandaMeses = hechizo.BG_IHS_custom_rel_demanda.ToList();

                foreach (var item_demanda in demandaMeses)
                {
                    //encuentra la cabecera
                    var cabecera = cabeceraMeses.FirstOrDefault(x => x.fecha == item_demanda.fecha);

                    if (cabecera != null)
                        row[cabecera.text] = "=" + (item_demanda.cantidad != null ? item_demanda.cantidad.Value : 0) + "*(1+" + porcentajeReferencia + inicio_fila + ")";
                }

                #endregion

                #region Cuartos
                foreach (var cuarto in cabeceraCuartos)
                {
                    // Filtra los meses correspondientes al cuarto actual y al año correspondiente
                    var mesesDelCuarto = cabeceraMeses
                        .Where(x => x.fecha.HasValue &&
                                    x.fecha.Value.Year == cuarto.anio && // Verifica que el año coincida
                                    ((x.fecha.Value.Month - 1) / 3) + 1 == cuarto.quarter) // Calcula el trimestre del mes
                        .Select(x =>
                        {
                            // Obtén la columna usando el método GetCellReference y combina con la fila
                            string colReference = GetCellReference(camposPrevios + 2 + cabeceraMeses.IndexOf(x));
                            return $"{colReference}{fila_actual}"; // Combina columna y fila
                        })
                        .ToList();

                    // Si se encontraron los tres meses correspondientes al cuarto, genera la fórmula
                    if (mesesDelCuarto.Count == 3)
                    {
                        row[cuarto.text] = $"=SUM({string.Join(",", mesesDelCuarto)})"; // Crea la fórmula de suma para el cuarto
                    }
                    else
                    {
                        row[cuarto.text] = ""; // Deja la celda vacía si faltan datos para el trimestre
                    }
                }

                #endregion

                #region Años
                foreach (var anio in cabeceraAnios)
                {
                    // Filtra los meses correspondientes al año actual
                    var mesesDelAnio = cabeceraMeses
                        .Where(x => x.fecha.HasValue && x.fecha.Value.Year == anio.anio) // Verifica que el año coincida
                        .Select(x =>
                        {
                            // Obtén la columna usando el método GetCellReference y combina con la fila
                            string colReference = GetCellReference(camposPrevios + 2 + cabeceraMeses.IndexOf(x));
                            return $"{colReference}{fila_actual}"; // Combina columna y fila
                        })
                        .ToList();

                    // Si hay meses disponibles para el año, genera la fórmula
                    if (mesesDelAnio.Any())
                    {
                        row[anio.text] = $"=SUM({string.Join(",", mesesDelAnio)})"; // Crea la fórmula de suma para el año
                    }
                    else
                    {
                        row[anio.text] = ""; // Deja la celda vacía si no hay datos para el año
                    }
                }
                #endregion

                #region Años Fiscales (FY)
                foreach (var anio in cabeceraAniosFY)
                {
                    // Filtra los meses correspondientes al año fiscal (octubre a septiembre)
                    var mesesDelFY = cabeceraMeses
                        .Where(x => x.fecha.HasValue &&
                                    ((x.fecha.Value.Year == anio.anio - 1 && x.fecha.Value.Month >= 10) || // Meses de octubre a diciembre del año anterior
                                     (x.fecha.Value.Year == anio.anio && x.fecha.Value.Month <= 9))) // Meses de enero a septiembre del año actual
                        .Select(x =>
                        {
                            // Obtén la columna usando el método GetCellReference y combina con la fila
                            string colReference = GetCellReference(camposPrevios + 2 + cabeceraMeses.IndexOf(x));
                            return $"{colReference}{fila_actual}"; // Combina columna y fila
                        })
                        .ToList();

                    // Si hay meses disponibles para el año fiscal, genera la fórmula
                    if (mesesDelFY.Any())
                    {
                        row[anio.text] = $"=SUM({string.Join(",", mesesDelFY)})"; // Crea la fórmula de suma para el año fiscal
                    }
                    else
                    {
                        row[anio.text] = ""; // Deja la celda vacía si no hay datos para el año fiscal
                    }
                }
                #endregion



                dt.Rows.Add(row);
                fila_actual++;
            }
            //inserta la tabla en el momento en la columna donde empiezan los meses
            oSLDocument.ImportDataTable(fila_actual - hechizos.Count, camposPrevios + 2, dt, false);


            #endregion


            //estilos
            oSLDocument.SetColumnStyle(33, 34, styleShortDate);
            oSLDocument.SetColumnStyle(59, stylePercent);

            //establece alto de las filas
            oSLDocument.SetRowHeight(1, fila_actual, 15.0);
            //set autofit
            oSLDocument.AutoFitColumn(1, columnasStyles);

            #endregion

            System.Diagnostics.Debug.WriteLine($"[TIMER] finaliza Autos Modificados {timer.Elapsed.TotalSeconds:F2} segundos");
            timer.Restart();

            #region MonthByMonth                    

            hubContext.Clients.All.recibirProgresoExcel(56, 56, 100, $"Procesando Hoja Month By Moth.");

            //si cabecera anios tiene elementos de tipo meses
            var cabeceraAniosFY_conMeses = cabeceraAniosFY.Where(x => cabeceraMeses.Any(y => y.fecha.Value.Year == x.anio || y.fecha.Value.Year == (x.anio + 1))).ToList();

            //elimina los años no deseados
            int anioActual = DateTime.Now.Year;
            cabeceraAniosFY_conMeses = cabeceraAniosFY_conMeses.Where(x => x.anio >= (anioActual - 2) && x.anio <= (anioActual + 4)).ToList();

            //crea la plantilla para el primer elemento
            if (cabeceraAniosFY_conMeses.Count > 0)
            {
                oSLDocument.AddWorksheet(cabeceraAniosFY_conMeses[0].text + " by Month");
                oSLDocument.SelectWorksheet(cabeceraAniosFY_conMeses[0].text + " by Month");

                
                int columnIndex = 0;

                dt = new System.Data.DataTable();
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));  //pos                
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));  //vehicle
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //sap invoice code
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //A/D 

                ReferenciaColumna refA_D = new ReferenciaColumna
                {
                    celdaDescripcion = "A_D",
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(DateTime));   //Inicio Demanda 
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(DateTime));   //Fin Demanda
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //Business & plant
                ReferenciaColumna refBusinnessAndPlant = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //Business;           //
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //previous sap invoice code
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //mnemonic
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //Invoiced to          //6
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //Number SAP client
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   // Shipped to
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //OWN/CM               
                ReferenciaColumna refOwnCM = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //route
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //plant
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //External processor
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //mill
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //SAP master coil
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //part description
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //part number
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //production line
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //production nameplate
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //propulsion System Type
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //oem
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //parts auto
                ReferenciaColumna refPartsAuto = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //strokes auto
                ReferenciaColumna refStrokesAuto = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //Material type
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //Material type short
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //shape
                ReferenciaColumna refShape = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //initial weight part
                ReferenciaColumna refInitialWeightPart = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //net weight part
                ReferenciaColumna refNetWeightPart = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //Eng Scrap
                ReferenciaColumna refEngineeredScrap = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   //scrap consolidation  
                ReferenciaColumna refScrapConsolidation = new ReferenciaColumna
                {
                    celdaDescripcion = "Scrap Consolidation",
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //ventas part
                ReferenciaColumna refVentasPart = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //material cost part
                ReferenciaColumna refMaterialCostPart = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //Cost of outside processor
                ReferenciaColumna refCostOfOutsideProcessor = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //vas part
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //Additional material cost
                ReferenciaColumna refAdditionalMaterialCostPart = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //outgoing freight part
                ReferenciaColumna refOutgoingFreightPart = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(DateTime));   // trans silao-slp
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //vas to
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //gross profit /part
                ReferenciaColumna refGrossPart = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(double));   //gross profit /to
                ReferenciaColumna refGrossUSD = new ReferenciaColumna
                {
                    celdaReferencia = GetCellReference(dt.Columns.Count),
                    columnaNum = dt.Columns.Count
                };
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   // Freights Income
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   // Outgoing freight
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   // Cat 1
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   // Cat 3
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   // Cat 4
                dt.Columns.Add(dictionaryTitulosByMonth.ElementAt(columnIndex++).Value, typeof(string));   // IHS Asociado

                //obtiene el reporte
                int pos = 1;
                DateTime? transSilaoSLPDate = null;
                foreach (var forecast_Item in reporte.BG_Forecast_item)
                {

                    if (DateTime.TryParse(forecast_Item.trans_silao_slp, out DateTime transSilaoSLPDateResult))
                    {
                        transSilaoSLPDate = transSilaoSLPDateResult;
                    }

                    dt.Rows.Add(pos++, forecast_Item.vehicle, forecast_Item.sap_invoice_code, forecast_Item.calculo_activo ? "A" : "D",

                        forecast_Item.inicio_demanda, forecast_Item.fin_demanda, forecast_Item.business_and_plant
                        , forecast_Item.cat_2, forecast_Item.sap_invoice_code, forecast_Item.mnemonic_vehicle_plant
                        , forecast_Item.invoiced_to, forecast_Item.number_sap_client, forecast_Item.shipped_to, forecast_Item.own_cm, forecast_Item.route, forecast_Item.plant
                        , forecast_Item.external_processor, forecast_Item.mill, forecast_Item.sap_master_coil, forecast_Item.part_description, forecast_Item.part_number
                        , forecast_Item.production_line, forecast_Item.production_nameplate, forecast_Item.propulsion_system_type, forecast_Item.oem
                        , forecast_Item.parts_auto, forecast_Item.strokes_auto, forecast_Item.material_type, forecast_Item.material_short
                        , forecast_Item.shape, forecast_Item.initial_weight_part, forecast_Item.net_weight_part, forecast_Item.eng_scrap_part, forecast_Item.scrap_consolidation ? "YES" : "NO"
                        , forecast_Item.ventas_part, forecast_Item.material_cost_part, forecast_Item.cost_of_outside_processor, forecast_Item.vas_part
                        , forecast_Item.additional_material_cost_part, forecast_Item.outgoing_freight_part, transSilaoSLPDate
                        , forecast_Item.vas_to, DBNull.Value, DBNull.Value, forecast_Item.freights_income, forecast_Item.outgoing_freight, forecast_Item.cat_1
                        , forecast_Item.cat_3, forecast_Item.cat_4, forecast_Item.id_asociacion_ihs
                        );
                }


                oSLDocument.ImportDataTable(1, 1, dt, true);

                SLDataValidation dv;

                //validación para A/D
                dv = oSLDocument.CreateDataValidation(5, refA_D.columnaNum, dt.Rows.Count + 4, refA_D.columnaNum);
                dv.AllowList("A_D", true, true);
                oSLDocument.AddDataValidation(dv);

                //validacion para scrap consolidation
                dv = oSLDocument.CreateDataValidation(5, refScrapConsolidation.columnaNum, dt.Rows.Count + 4, refScrapConsolidation.columnaNum);
                dv.AllowList("YES_NO", true, true);
                oSLDocument.AddDataValidation(dv);

                int columnasAnterior = dt.Columns.Count;

                //da estilo a la hoja de excel
                //inmoviliza el encabezado
                oSLDocument.FreezePanes(1, 0);
                oSLDocument.Filter(1, 1, 1, columnasAnterior /*+ 1 + dt.Columns.Count*/);

                oSLDocument.AutoFitColumn(1, columnasAnterior /*+ 1 + dt.Columns.Count*/);

                //da estilo al encabezado
                oSLDocument.SetColumnStyle(1, columnasAnterior/* + 1 + dt.Columns.Count*/, styleCenterTop);
                oSLDocument.SetRowStyle(1, styleHeader);
                oSLDocument.SetRowStyle(1, styleHeaderFont);

                //estilos para números
                oSLDocument.SetColumnStyle(ColumnToIndex(dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _PARTS_AUTO).Key),
                                ColumnToIndex(dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _STROKES_AUTO).Key), styleNumberDecimal_3);
                oSLDocument.SetColumnStyle(ColumnToIndex(dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _INITIAL_WEIGHT_PART).Key),
                    ColumnToIndex(dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _ENG_SCRAP_PART).Key), styleNumberDecimal_3);
                oSLDocument.SetColumnStyle(ColumnToIndex(dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _VENTAS_PART).Key),
                    ColumnToIndex(dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _OUTGOING_FREIGHT_PART).Key), styleCurrency);
                oSLDocument.SetColumnStyle(ColumnToIndex(dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _OUTGOING_FREIGHT_PART).Key),
                    ColumnToIndex(dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _GROSS_PROFIT_OUTGOING_FREIGHT_TO).Key), styleCurrency);
                //estilo para fecha
                oSLDocument.SetColumnStyle(ColumnToIndex(dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _TRANS_SILAO_SLP).Key), styleShortDate);
                oSLDocument.SetColumnStyle(ColumnToIndex(dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _INICIO_DEMANDA).Key), styleShortDate);
                oSLDocument.SetColumnStyle(ColumnToIndex(dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _FIN_DEMANDA).Key), styleShortDate);

                SLStyle styleAdvertencia = oSLDocument.CreateStyle();
                styleAdvertencia.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffd100"), System.Drawing.ColorTranslator.FromHtml("#ffd100"));


                //da estilo a los vehiculos que no están asociado a ninguna combinacion, división o elemento de IHS
                foreach (var forecast_Item in reporte.BG_Forecast_item)
                {
                    if (forecast_Item.BG_IHS_item == null && forecast_Item.BG_IHS_combinacion == null && forecast_Item.BG_IHS_rel_division == null && forecast_Item.BG_ihs_vehicle_custom == null)
                        oSLDocument.SetCellStyle(reporte.BG_Forecast_item.ToList().IndexOf(forecast_Item) + 2, 2, styleSinAsociacion);
                    if (forecast_Item.mostrar_advertencia)
                        oSLDocument.SetCellStyle(reporte.BG_Forecast_item.ToList().IndexOf(forecast_Item) + 2, 2, styleAdvertencia);

                }

                //ajusta el tamaño de las filas
                oSLDocument.SetRowHeight(1, dt.Rows.Count + 1, 45.0);
                oSLDocument.SetRowHeight(2, dt.Rows.Count + 1, 15.0);

                //ajusta el ancho de las columnas
                //oSLDocument.SetColumnWidth(4, 13.0); //A/D
                //oSLDocument.SetColumnWidth(7, 13.0);   //Number SAP Client
                oSLDocument.SetColumnWidth(dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _INITIAL_WEIGHT_PART).Key,
                    dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _GROSS_PROFIT_OUTGOING_FREIGHT_TO).Key, 13.0);   //Number SAP Client

                //oculta columnas
                oSLDocument.HideColumn(dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _IHS_ASOCIADO).Key);

                //obtiene refencia a la columna con la clave de búsqueda (última de la tabla)
                string claveRef = GetCellReference(dt.Columns.Count);

                int numInicioColumnaDatosBase = columnasAnterior + 2;
                int numInicioColumnaTotalSales = 0;
                int numInicioColumnaMaterialCost = 0;
                int numInicioColumnaCostOfOutsiteP = 0;
                int numInicioColumnaEngScrap = 0;
                int numInicioColumnaScrapConsolidation = 0;
                int numInicioColumnaStrokes = 0;
                int numInicioColumnaBlanks = 0;
                int numInicioColumnaProccessTon = 0;
                int numInicioColumnaInventoryOWNAverage = 0;
                int numInicioColumnaInventoryEndMonth = 0;
                int numInicioColumnaFreightsIncomeUSD = 0;
                int numInicioColumnaOutgoingFreightTotal = 0;
                int numInicioColumnaAditionalMaterialCost = 0;
                int numInicioColumnaManiobrasPart = 0;
                int numInicioColumnaCustomExpenses = 0;
                int numInicioColumnaShipmentTons = 0;
                int numInicioColumnaSalesIncScrap = 0;
                int numInicioColumnaProcessingIncScrap = 0;
                int numInicioColumnaWoodenPallets = 0;
                int numInicioColumnaStandardPackaging = 0;
                int numInicioColumnaPlasticStrips = 0;
                int numInicioColumnaValueAddSales = 0;
                int numInicioColumnaVasIncScrap = 0;

                //agrega la tabla de referencias para cada fY
                for (int i = 0; i < cabeceraAniosFY_conMeses.Count; i++)
                {
                    int columnaActual = columnasAnterior + 2;
                    numInicioColumnaDatosBase = columnaActual;

                    //selecciona la hoja
                    string newSheetName = cabeceraAniosFY_conMeses[i].text + " by Month";
                    oSLDocument.SelectWorksheet(newSheetName);

                    //inserta una nueva columna y combina los encabezados
                    oSLDocument.InsertRow(1, 3);

                    //se agrega tabla para los meses
                    #region tabla meses                    

                    //columns Mapping
                    var columnMappings = new Dictionary<string, int>();
                    int tempColumnaActual = columnaActual; // Usamos una variable temporal para el cálculo

                    foreach (var t in tablasReferenciasIniciales)
                    {
                        int indexTabla = tablasReferenciasIniciales.IndexOf(t);
                        int extra = !string.IsNullOrEmpty(t.extra) ? 1 : 0;

                        // Asignamos los valores a tus variables importantes. Esto se hace una vez por hoja.
                        switch (indexTabla)
                        {
                            case 0: numInicioColumnaDatosBase = tempColumnaActual; break;
                            case 1: numInicioColumnaTotalSales = tempColumnaActual; break;
                            case 2: numInicioColumnaMaterialCost = tempColumnaActual; break;
                            case 3: numInicioColumnaCostOfOutsiteP = tempColumnaActual; break;
                            case 4: numInicioColumnaValueAddSales = tempColumnaActual; break;
                            case 5: numInicioColumnaProccessTon = tempColumnaActual; break;
                            case 6: numInicioColumnaEngScrap = tempColumnaActual; break;
                            case 7: numInicioColumnaScrapConsolidation = tempColumnaActual; break;
                            case 8: numInicioColumnaStrokes = tempColumnaActual; break;
                            case 9: numInicioColumnaBlanks = tempColumnaActual; break;
                            case 10: numInicioColumnaAditionalMaterialCost = tempColumnaActual; break;
                            case 11: numInicioColumnaOutgoingFreightTotal = tempColumnaActual; break;
                            case 12: numInicioColumnaInventoryOWNAverage = tempColumnaActual; inicioInventoryOwnMeses = tempColumnaActual; break;
                            case 13: numInicioColumnaInventoryEndMonth = tempColumnaActual; break;
                            case 14: numInicioColumnaFreightsIncomeUSD = tempColumnaActual; break;
                            case 15: numInicioColumnaManiobrasPart = tempColumnaActual; break;
                            case 16: numInicioColumnaCustomExpenses = tempColumnaActual; break;
                            case 17: numInicioColumnaShipmentTons = tempColumnaActual; break;
                            case 18: numInicioColumnaSalesIncScrap = tempColumnaActual; break;
                            case 19: numInicioColumnaVasIncScrap = tempColumnaActual; break;
                            case 20: numInicioColumnaProcessingIncScrap = tempColumnaActual; break;
                            case 21: numInicioColumnaWoodenPallets = tempColumnaActual; break;
                            case 22: numInicioColumnaStandardPackaging = tempColumnaActual; break;
                            case 23: numInicioColumnaPlasticStrips = tempColumnaActual; break;
                        }

                        // Guardamos la posición de cada columna futura en nuestro mapa.
                        if (extra == 1)
                        {
                            columnMappings.Add($"{t.celdaDescripcion}_extra", tempColumnaActual);
                        }
                        for (int j = 0; j < 12; j++)
                        {
                            DateTime mesFY = new DateTime(cabeceraAniosFY_conMeses[i].anio, 10, 1).AddMonths(j);
                            string colKey = $"{t.celdaDescripcion}_{mesFY:MMM yyyy}".ToUpper().Replace(".", "");
                            columnMappings.Add(colKey, tempColumnaActual + extra + j);
                        }
                        tempColumnaActual += 13 + extra;
                    }


                    hubContext.Clients.All.recibirProgresoExcel(62, 62, 100, $"Iniciando Tablas de Datos");

                    foreach (var t in tablasReferenciasIniciales)
                    {
                        //reinicia la tabla
                        dt = new System.Data.DataTable();
                        int indexTabla = tablasReferenciasIniciales.IndexOf(t);

                        hubContext.Clients.All.recibirProgresoExcel(68, 68, 100, $"Agregando tabla {t.celdaDescripcion} ({indexTabla + 1}/{tablasReferenciasIniciales.Count})");


                        //asigna el valor de la columna actual al valor de la referencia inicial de latabla
                        t.columnaNum = columnaActual;

                        //agrega la columna extra si es necesario
                        if (!String.IsNullOrEmpty(t.extra))
                        {
                            t.columnaNum = columnaActual + 1;
                            dt.Columns.Add(t.extra, typeof(double));
                        }

                        //agrega los promedios de ser necesarios
                        if (t.celdaDescripcion == "Inventory OWN (monthly average) [tons]")
                        {
                            for (int k = 0; k < averageOwnList.Count; k++)
                            {
                                oSLDocument.SetCellValue(1, columnaActual + k, averageOwnList[k]);
                                oSLDocument.SetCellValue(2, columnaActual + k, "=12/" + GetCellReference(columnaActual + k) + "1");
                                oSLDocument.SetCellStyle(1, columnaActual + k, styleNumberDecimal_1);
                                oSLDocument.SetCellStyle(2, columnaActual + k, styleNumberDecimal_1);
                            }
                        }



                        //crea las columnas para los doce meses del año fiscal
                        DateTime mesFY = new DateTime(cabeceraAniosFY_conMeses[i].anio, 10, 01); //octubre del FY
                        for (int j = 0; j < 12; j++)
                        {
                            dt.Columns.Add(mesFY.ToString("MMM yyyy").ToUpper().Replace(".", String.Empty)/*, typeof(double)*/);
                            //aumenta un mes
                            mesFY = mesFY.AddMonths(1);
                        }

                        int extra = !String.IsNullOrEmpty(t.extra) ? 1 : 0;

                        //Genera las referencias al reporte para cada elemento del reporte forecast

                        int excelRowNum = 5;
                        string colMaterialRef = dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _MATERIAL_TYPE_SHORT).Key;
                        System.Data.DataRow row = dt.NewRow();

                        //recorre todos los meses del año
                        mesFY = new DateTime(cabeceraAniosFY_conMeses[i].anio, 10, 01); //octubre del FY
                        foreach (var forecast_Item in reporte.BG_Forecast_item)
                        {
                            row = dt.NewRow();

                            for (int j = 0; j < 12; j++) //solo realiza una vez
                            {
                                mesFY = new DateTime(cabeceraAniosFY_conMeses[i].anio, 10, 1).AddMonths(j);
                                string colKey = $"{mesFY:MMM yyyy}".ToUpper().Replace(".", "");
                                string formula = "\"--\"";

                                string autosMonthColRef = GetCellReference(columnMappings[$"Autos/Month_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);

                                switch (indexTabla)
                                {
                                    //autos/month
                                    case 0:
                                        // OPTIMIZACIÓN: Se mantiene el comportamiento original.
                                        // Como este caso estaba comentado, no se asigna ninguna fórmula específica.
                                        // La variable 'formula' mantendrá su valor por defecto de "--".
                                        break;

                                    //total Sales [TUSD]
                                    case 1:
                                        // Se usa la referencia pre-calculada 'autosMonthColRef' que apunta a la columna de "Autos/Month"
                                        formula = $"=IFERROR(IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refVentasPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${refVentasPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum}),\"--\")";
                                        break;
                                    //Material Cost
                                    case 2:
                                        formula = $"=IFERROR(IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refMaterialCostPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${refMaterialCostPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum}),\"--\")";
                                        break;

                                    //cost of out side procesor
                                    case 3:
                                        formula = $"=IFERROR(IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refCostOfOutsideProcessor.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${refCostOfOutsideProcessor.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum}),\"--\")";
                                        break;

                                    //Value Added Sales[TUSD]
                                    case 4:
                                        // Buscamos en nuestro mapa las referencias a las columnas que necesitamos para la fórmula.
                                        string totalSalesCol = GetCellReference(columnMappings[$"Total Sales [TUSD]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);
                                        string materialCostCol = GetCellReference(columnMappings[$"Material Cost [TUSD]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);
                                        string costOutsideCol = GetCellReference(columnMappings[$"COST OF OUTSIDE PROCESSOR (e.g. LASER WELD)/PART  [TUSD]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);

                                        formula = $"=IFERROR(({totalSalesCol}{excelRowNum}-{materialCostCol}{excelRowNum}-{costOutsideCol}{excelRowNum}),\"--\")";
                                        break;
                                    //Processed Tons [to]
                                    case 5:
                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum}=\"WLD\",0,IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refInitialWeightPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${refInitialWeightPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum})),\"--\")";
                                        break;

                                    //Engineered Scrap [to]
                                    case 6:
                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum}=\"WLD\",0, ${refEngineeredScrap.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum}),\"--\")";
                                        break;

                                    //scrapConsolidation
                                    case 7:
                                        // OPTIMIZACIÓN: Buscamos en el mapa la referencia a la columna "Engineered Scrap" para este mes.
                                        string engScrapCol = GetCellReference(columnMappings[$"Engineered Scrap [to]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);
                                        formula = $"=IFERROR(IF(${refScrapConsolidation.celdaReferencia}{excelRowNum}=\"YES\", -{engScrapCol}{excelRowNum}, 0),\"--\")";
                                        break;

                                    //Strokes [ - / 1000 ]
                                    case 8:
                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum}=\"WLD\",0, ${refStrokesAuto.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000),\"--\")";
                                        break;

                                    //Blanks [ - / 1000 ]
                                    case 9:
                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum}=\"WLD\",0, ${refPartsAuto.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000),\"--\")";
                                        break;
                                    //Additional material Cost
                                    case 10:
                                        formula = $"=IFERROR(IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refAdditionalMaterialCostPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${refAdditionalMaterialCostPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum}),\"--\")";
                                        break;

                                    //Outgoing freight total [USD]
                                    case 11:
                                        formula = $"=IFERROR(IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refOutgoingFreightPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${refOutgoingFreightPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum}),\"--\")";
                                        break;

                                    //Inventory OWN (monthly average) [tons]
                                    case 12:
                                        // Buscamos las referencias de columna que necesitamos en nuestro mapa
                                        string procTonsCol = GetCellReference(columnMappings[$"Processed Tons [to]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);
                                        string invAvgCol = GetCellReference(columnMappings[$"Inventory OWN (monthly average) [tons]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);

                                        formula = $"=IFERROR(IF(LEFT(${refOwnCM.celdaReferencia}{excelRowNum},2)=\"CM\",0,{procTonsCol}{excelRowNum}*{invAvgCol}$2),\"--\")";
                                        break;

                                    //Inventory OWN (end of month) [USD]
                                    case 13:
                                        // Buscamos la referencia a la columna del promedio mensual de inventario
                                        string invOwnAvgCol = GetCellReference(columnMappings[$"Inventory OWN (monthly average) [tons]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);

                                        formula = $"=IFERROR({invOwnAvgCol}{excelRowNum}*(${refMaterialCostPart.celdaReferencia}{excelRowNum}+${refAdditionalMaterialCostPart.celdaReferencia}{excelRowNum})/${refInitialWeightPart.celdaReferencia}{excelRowNum},\"--\")";
                                        break;
                                    //Freights Income USD/PART
                                    case 14:
                                        // La fórmula original usaba GetCellReference(columnaActual), que se refería a la columna "extra"
                                        // que contenía el costo unitario. La buscamos en nuestro mapa.
                                        string freightsIncomeExtraCol = GetCellReference(columnMappings[$"{t.celdaDescripcion}_extra"]);
                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum} = \"WLD\", 0, IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${freightsIncomeExtraCol}{excelRowNum} * {autosMonthColRef}{excelRowNum} / 1000, ${freightsIncomeExtraCol}{excelRowNum} * {autosMonthColRef}{excelRowNum} / 1000 * ${refPartsAuto.celdaReferencia}{excelRowNum})), \"--\")";
                                        break;

                                    //Freights Maniobras USD/PART
                                    case 15:
                                        string maniobrasExtraCol = GetCellReference(columnMappings[$"{t.celdaDescripcion}_extra"]);
                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum} = \"WLD\", 0, IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${maniobrasExtraCol}{excelRowNum} * {autosMonthColRef}{excelRowNum} / 1000, ${maniobrasExtraCol}{excelRowNum} * {autosMonthColRef}{excelRowNum} / 1000 * ${refPartsAuto.celdaReferencia}{excelRowNum})), \"--\")";
                                        break;

                                    //Customs Expenses USD/PART
                                    case 16:
                                        string customsExtraCol = GetCellReference(columnMappings[$"{t.celdaDescripcion}_extra"]);
                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum} = \"WLD\", 0, IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${customsExtraCol}{excelRowNum} * {autosMonthColRef}{excelRowNum} / 1000, ${customsExtraCol}{excelRowNum} * {autosMonthColRef}{excelRowNum} / 1000 * ${refPartsAuto.celdaReferencia}{excelRowNum})), \"--\")";
                                        break;

                                    //Shipment Tons[to]
                                    case 17:
                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum} = \"WLD\", 0, IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refInitialWeightPart.celdaReferencia}{excelRowNum} * {autosMonthColRef}{excelRowNum} / 1000, ${refNetWeightPart.celdaReferencia}{excelRowNum} * {autosMonthColRef}{excelRowNum} / 1000 * ${refPartsAuto.celdaReferencia}{excelRowNum})), \"--\")";
                                        break;

                                    //SALES Inc. SCRAP[USD]
                                    case 18:
                                        // Buscamos las referencias de columna que necesitamos en nuestro mapa.
                                        string totalSalesCol18 = GetCellReference(columnMappings[$"Total Sales [TUSD]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);
                                        string engScrapCol18 = GetCellReference(columnMappings[$"Engineered Scrap [to]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);

                                        // La variable colMaterialRef fue pre-calculada antes del bucle de filas.
                                        formula = $"=IFERROR(IF(${colMaterialRef}{excelRowNum} = \"STEEL\", {totalSalesCol18}{excelRowNum}+{engScrapCol18}{excelRowNum}*{totalSalesCol18}${filaScrapSteel}/1000, {totalSalesCol18}{excelRowNum}+{engScrapCol18}{excelRowNum}*{totalSalesCol18}${filaScrapAlu}/1000), \"--\")";
                                        break;
                                    //VAS Inc. SCRAP [USD]
                                    case 19:
                                        // Buscamos todas las referencias de columna que necesitamos en nuestro mapa.
                                        string valAddSales = GetCellReference(columnMappings[$"Value Added Sales [TUSD]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);
                                        string engScrap = GetCellReference(columnMappings[$"Engineered Scrap [to]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);
                                        string scrapCons = GetCellReference(columnMappings[$"Scrap Consolidation [to]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);
                                        string totalSales = GetCellReference(columnMappings[$"Total Sales [TUSD]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);
                                        string materialCost = GetCellReference(columnMappings[$"Material Cost [TUSD]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);

                                        // La variable colMaterialRef fue pre-calculada antes del bucle de filas.
                                        formula = $"=IFERROR(IF(${colMaterialRef}{excelRowNum} = \"STEEL\", {valAddSales}{excelRowNum}+{engScrap}{excelRowNum}*{totalSales}${filaScrapSteel}/1000+{scrapCons}{excelRowNum}*{materialCost}${filaScrapSteel}/1000, {valAddSales}{excelRowNum}+{engScrap}{excelRowNum}*{totalSales}${filaScrapAlu}/1000+{scrapCons}{excelRowNum}*{materialCost}${filaScrapAlu}/1000), \"--\")";
                                        break;

                                    //Processing Inc. SCRAP
                                    case 20:
                                        // Buscamos las referencias de las columnas necesarias.
                                        string vasIncScrap = GetCellReference(columnMappings[$"VAS Inc. SCRAP [USD]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);
                                        string addMatCost = GetCellReference(columnMappings[$"Additional material cost total [USD]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);
                                        string outFreight = GetCellReference(columnMappings[$"Outgoing freight total [USD]_{mesFY:MMM yyyy}".ToUpper().Replace(".", "")]);

                                        formula = $"=IFERROR({vasIncScrap}{excelRowNum} - {addMatCost}{excelRowNum} - {outFreight}{excelRowNum}, \"--\")";
                                        break;
                                    //Wooden pallets
                                    case 21:
                                        // La fórmula original usaba GetCellReference(columnaActual), que se refería a la columna "extra"
                                        // que contenía el costo unitario. La buscamos en nuestro mapa.
                                        string woodenPalletsExtraCol = GetCellReference(columnMappings[$"{t.celdaDescripcion}_extra"]);
                                        formula = $"=IFERROR(${woodenPalletsExtraCol}{excelRowNum}*{autosMonthColRef}{excelRowNum}*${refPartsAuto.celdaReferencia}{excelRowNum}/1000, \"--\")";
                                        break;

                                    //Standard packaging
                                    case 22:
                                        string standardPackagingExtraCol = GetCellReference(columnMappings[$"{t.celdaDescripcion}_extra"]);
                                        formula = $"=IFERROR(${standardPackagingExtraCol}{excelRowNum}*{autosMonthColRef}{excelRowNum}*${refPartsAuto.celdaReferencia}{excelRowNum}/1000, \"--\")";
                                        break;

                                    //PLASTIC STRIPS
                                    case 23:
                                        string plasticStripsExtraCol = GetCellReference(columnMappings[$"{t.celdaDescripcion}_extra"]);
                                        formula = $"=IFERROR(${plasticStripsExtraCol}{excelRowNum}*{autosMonthColRef}{excelRowNum}*${refPartsAuto.celdaReferencia}{excelRowNum}/1000, \"--\")";
                                        break;
                                }
                                row[colKey] = formula;

                            }

                            dt.Rows.Add(row);
                            excelRowNum++;
                        }

                        //switch para stilos
                        switch (indexTabla)
                        {
                            //autos/month
                            case 0:
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                            case 12:
                            case 13:
                                oSLDocument.SetCellStyle(5, columnaActual, excelRowNum, (columnaActual + 11 + extra), styleNumberDecimal_2);
                                break;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 10:
                            case 11:
                            case 14:
                            case 15:
                            case 16:
                            case 18:
                            case 19:
                            case 20:
                            case 21:
                            case 22:
                            case 23:
                                oSLDocument.SetCellStyle(5, columnaActual, excelRowNum, (columnaActual + 11 + extra), styleCurrency);
                                break;
                            case 5:
                            case 17:
                                oSLDocument.SetCellStyle(5, columnaActual, excelRowNum, (columnaActual + 11 + extra), styleNumberDecimal_0);
                                break;


                        }


                        int rowActual = 5;

                        //agrega los campos de gross
                        var sumTotalSales = "SUM(" + GetCellReference(numInicioColumnaTotalSales) + rowActual + ":" + GetCellReference(numInicioColumnaTotalSales + 11) + rowActual + ")";
                        var sumEngScrap = "SUM(" + GetCellReference(numInicioColumnaEngScrap + 1) + rowActual + ":" + GetCellReference(numInicioColumnaEngScrap + 1 + 11) + rowActual + ")";
                        var sumScrapConsolidation = "SUM(" + GetCellReference(numInicioColumnaScrapConsolidation) + rowActual + ":" + GetCellReference(numInicioColumnaScrapConsolidation + 11) + rowActual + ")";
                        var sumAditionalMaterialCost = "SUM(" + GetCellReference(numInicioColumnaAditionalMaterialCost) + rowActual + ":" + GetCellReference(numInicioColumnaAditionalMaterialCost + 11) + rowActual + ")";
                        var sumOutgoingFreightTotal = "SUM(" + GetCellReference(numInicioColumnaOutgoingFreightTotal) + rowActual + ":" + GetCellReference(numInicioColumnaOutgoingFreightTotal + 11) + rowActual + ")";
                        var sumDatosBase = "SUM(" + GetCellReference(numInicioColumnaDatosBase) + rowActual + ":" + GetCellReference(numInicioColumnaDatosBase + 11) + rowActual + ")";

                        var formulaGrossPart = "=IFERROR((" + sumTotalSales + "*1000 + " + sumEngScrap + "*0 + " + sumScrapConsolidation + "*0 - (" + sumAditionalMaterialCost + " + " + sumOutgoingFreightTotal + ")*1000) / (" + refStrokesAuto.celdaReferencia + rowActual + "*" + sumDatosBase + "),\"--\")";
                        oSLDocument.SetCellValue(rowActual, refGrossPart.columnaNum, formulaGrossPart);
                        oSLDocument.SetColumnStyle(refGrossPart.columnaNum, styleCurrency);

                        var formulaGrossUSD = "=IFERROR(" + refGrossPart.celdaReferencia + rowActual + "*1000/" + refInitialWeightPart.celdaReferencia + rowActual + ",\"--\")";
                        oSLDocument.SetCellValue(rowActual, refGrossUSD.columnaNum, formulaGrossUSD);
                        oSLDocument.SetColumnStyle(refGrossUSD.columnaNum, styleCurrency);

                        //AGREGA LA SUMATORIA
                        //recorre todos los AÑOS FISCALES POR CADA CATEGORIA
                        int lastRow = reporte.BG_Forecast_item.Count + 5;
                        int startRow = 5;
                        int endRow = reporte.BG_Forecast_item.Count + 4;

                        for (int j = 0; j < 12; j++)
                        {
                            int currentColumn = columnaActual + j + extra;
                            string cellReference = GetCellReference(currentColumn);
                            oSLDocument.SetCellValue(lastRow, currentColumn, "=SUM(" + cellReference + startRow + ":" + cellReference + endRow + ")");
                        }
                        oSLDocument.SetCellStyle(lastRow, columnaActual + extra, lastRow, columnaActual + extra + 11, styleTotales);

                        //importa la tabla con los datos
                        oSLDocument.ImportDataTable(4, columnaActual, dt, true);


                        //aplica formato condicional cuando el elemento está desactivado
                        SLConditionalFormatting cf;
                        cf = new SLConditionalFormatting(5, columnaActual, 5 + reporte.BG_Forecast_item.Count, columnaActual + 11 + extra);
                        cf.HighlightCellsWithFormula("=$D5=\"D\"", SLHighlightCellsStyleValues.LightRedFill);
                        oSLDocument.AddConditionalFormatting(cf);

                        //inmoviliza el encabezado
                        oSLDocument.FreezePanes(4, 4);

                        int c = 0;
                        foreach (var forecast_Item in reporte.BG_Forecast_item)
                        {
                            //agrega el estilo en caso de que no haya coincidencia con elemento de IHS
                            if (String.IsNullOrEmpty(forecast_Item.id_asociacion_ihs))
                                oSLDocument.SetCellStyle((rowActual + c), columnaActual, (rowActual + c), columnaActual + 11 + extra, styleMissedIHS);

                            c++;
                        }

                        //aplica el estilo a cada hoja de FY
                        oSLDocument.SetCellStyle(4, columnaActual, 4, columnaActual + 11 + extra, styleCenterCenter);
                        oSLDocument.SetCellStyle(4, columnaActual, 4, columnaActual + 11 + extra, styleHeader);
                        oSLDocument.SetCellStyle(4, columnaActual, 4, columnaActual + 11 + extra, styleHeaderFont);
                        oSLDocument.Filter(4, 1, 4, columnaActual + 11 + extra);
                        oSLDocument.AutoFitColumn(columnaActual, columnaActual + 11 + extra);

                        //realiza el merge para los titulos de cada tabla
                        oSLDocument.MergeWorksheetCells(3, columnaActual, 3, columnaActual + 11 + extra);
                        oSLDocument.SetCellValue(3, columnaActual, t.celdaDescripcion);
                        oSLDocument.SetCellStyle(3, columnaActual, styleHeaderFont);
                        oSLDocument.SetCellStyle(3, columnaActual, styleCenterTop);
                        oSLDocument.SetCellStyle(3, columnaActual, styleHeader);

                        //aumenta el valor de columnaActual para la siguiente iteración
                        columnaActual += 13 + extra;

                    }

                    //agrega el valor de los extras
                    int c2 = 0;
                    int filaInicial = 5;

                    foreach (var forecast_Item in reporte.BG_Forecast_item)
                    {
                        hubContext.Clients.All.recibirProgresoExcel(72, 72, 100, $"Agregando valores extra ({c2 + 1}/{reporte.BG_Forecast_item.Count})");

                        //Engennered Scrap
                        if (numInicioColumnaEngScrap > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaEngScrap, forecast_Item.own_cm + forecast_Item.plant + forecast_Item.material_short);

                        //Freights Income USD/PART
                        if (numInicioColumnaFreightsIncomeUSD > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaFreightsIncomeUSD, forecast_Item.freights_income_usd_part.HasValue ? forecast_Item.freights_income_usd_part.Value : 0.0);
                        //Freights Maniobras USD/PART
                        if (numInicioColumnaManiobrasPart > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaManiobrasPart, forecast_Item.maniobras_usd_part.HasValue ? forecast_Item.maniobras_usd_part.Value : 0.0);
                        //Customs Expenses USD/PART
                        if (numInicioColumnaCustomExpenses > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaCustomExpenses, forecast_Item.customs_expenses.HasValue ? forecast_Item.customs_expenses.Value : 0.0);
                        //Wooden pallets												
                        if (numInicioColumnaWoodenPallets > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaWoodenPallets, forecast_Item.wooden_pallet_usd_part.HasValue ? forecast_Item.wooden_pallet_usd_part.Value : 0.0);
                        //Standar Packaging
                        if (numInicioColumnaStandardPackaging > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaStandardPackaging, forecast_Item.packaging_price_usd_part.HasValue ? forecast_Item.packaging_price_usd_part.Value : 0.0);
                        //Plastic Strips
                        if (numInicioColumnaPlasticStrips > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaPlasticStrips, forecast_Item.neopreno_usd_part.HasValue ? forecast_Item.neopreno_usd_part.Value : 0.0);

                        c2++;
                    }

                    for (int k = 1; k < reporte.BG_Forecast_item.Count; k++)
                    {
                        //copia las sumatorias de gross
                        oSLDocument.CopyCell(5, refGrossPart.columnaNum, 5 + k, refGrossPart.columnaNum);
                        oSLDocument.CopyCell(5, refGrossUSD.columnaNum, 5 + k, refGrossUSD.columnaNum);

                    }

                    //ajusta el tamaño de las filas
                    oSLDocument.SetRowHeight(1, dt.Rows.Count + 4, 15.0);
                    oSLDocument.SetRowHeight(4, 45.0);

                    //if (numInicioColumnaFreightsIncomeUSD != 0)
                    //    oSLDocument.SetColumnStyle(numInicioColumnaFreightsIncomeUSD, styleCurrency);

                    #endregion

                    //solo realiza el calculo para una hoja(depuracion)
                    break;
                }


                //hace una copia del inicio de las columnas para la hoja de resumen por FY
                int numInicioColumnaTotalSalesMonth = numInicioColumnaTotalSales;
                int numInicioColumnaMaterialCostMonth = numInicioColumnaMaterialCost;
                int numInicioColumnaCostOfOutsitePMonth = numInicioColumnaCostOfOutsiteP;
                int numInicioColumnaEngScrapMonth = numInicioColumnaEngScrap;
                int numInicioColumnaScrapConsolidationMonth = numInicioColumnaScrapConsolidation;
                int numInicioColumnaStrokesMonth = numInicioColumnaStrokes;
                int numInicioColumnaBlanksMonth = numInicioColumnaBlanks;
                int numInicioColumnaProccessTonMonth = numInicioColumnaProccessTon;
                int numInicioColumnaInventoryOWNAverageMonth = numInicioColumnaInventoryOWNAverage;
                int numInicioColumnaInventoryEndMonthMonth = numInicioColumnaInventoryEndMonth;
                int numInicioColumnaFreightsIncomeUSDMonth = numInicioColumnaFreightsIncomeUSD;
                int numInicioColumnaOutgoingFreightTotalMonth = numInicioColumnaOutgoingFreightTotal;
                int numInicioColumnaAditionalMaterialCostMonth = numInicioColumnaAditionalMaterialCost;
                int numInicioColumnaManiobrasPartMonth = numInicioColumnaManiobrasPart;
                int numInicioColumnaCustomExpensesMonth = numInicioColumnaCustomExpenses;
                int numInicioColumnaShipmentTonsMonth = numInicioColumnaShipmentTons;
                int numInicioColumnaSalesIncScrapMonth = numInicioColumnaSalesIncScrap;
                int numInicioColumnaProcessingIncScrapMonth = numInicioColumnaProcessingIncScrap;
                int numInicioColumnaWoodenPalletsMonth = numInicioColumnaWoodenPallets;
                int numInicioColumnaStandardPackagingMonth = numInicioColumnaStandardPackaging;
                int numInicioColumnaPlasticStripsMonth = numInicioColumnaPlasticStrips;
                int numInicioColumnaValueAddSalesMonth = numInicioColumnaValueAddSales;
                int numInicioColumnaVasIncScrapMonth = numInicioColumnaVasIncScrap;

                hubContext.Clients.All.recibirProgresoExcel(75, 75, 100, $"Creando nuevas hojas para FYs");


                //agrega el nobre del reporte
                SLStyle styleHeaderWithBorders = styleHeader.Clone();
                styleHeaderWithBorders.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                styleHeaderWithBorders.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                styleHeaderWithBorders.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                styleHeaderWithBorders.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                styleHeaderWithBorders.Font.FontColor = System.Drawing.Color.White;


                oSLDocument.SetCellValue("B1", "Reporte");
                oSLDocument.SetCellValue("B2", model.nombreReporte);
                oSLDocument.SetCellStyle("B1", styleHeaderWithBorders);
                oSLDocument.SetCellStyle("B2", styleBorders);

                //listas de la BD
                //FASE 1: inicializa diccionarios
                var historicoVentas = db.BG_forecast_cat_historico_ventas.ToList();
                var listClientes = db.BG_forecast_cat_clientes.Where(x => x.activo).ToList();
                var historicoVentasDict = historicoVentas.ToLookup(x => x.id_cliente);
                var historicoScrap = db.BG_Forecast_cat_historico_scrap.ToList();
                var defaultScrap = db.BG_Forecast_cat_defaults.First();
                var referenciaColumnasDict = referenciaColumnas.ToDictionary(x => x.fecha);
                string FirstSheetName = cabeceraAniosFY_conMeses[0].text + " by Month";
                string baseSheetName = FirstSheetName; // El nombre de la hoja base que se copiará


                // --- FASE 2: BUCLE PRINCIPAL POR HOJA (AÑO FISCAL) ---
                for (int i = 0; i < cabeceraAniosFY_conMeses.Count; i++)
                {
                    var anioFiscal = cabeceraAniosFY_conMeses[i];
                    string newSheetName = anioFiscal.text + " by Month";

                    // Reinicia el cronómetro para medir esta hoja específica
                    timer.Restart();

                    // --- LÓGICA DE COPIA (INTEGRADA) ---
                    // Si no es la primera hoja (i > 0), la copiamos.
                    // La primera hoja (i = 0) ya existe, solo la seleccionamos.
                    if (i > 0)
                    {
                        hubContext.Clients.All.recibirProgresoExcel(76, 76, 100, $"Copiando plantilla a {cabeceraAniosFY_conMeses[i].text} ({i + 1}/{cabeceraAniosFY_conMeses.Count})");
                        oSLDocument.SelectWorksheet("Aux"); // Hoja temporal para evitar errores al copiar
                        oSLDocument.CopyWorksheet(baseSheetName, newSheetName);
                    }

                    // Filtro para saltar años no deseados
                    if (anioFiscal.anio < (DateTime.Now.Year - 2) || anioFiscal.anio > (DateTime.Now.Year + 4))
                        continue;

                    // --- SELECCIÓN DE HOJA (LA OPERACIÓN LENTA) ---
                    // Esto ahora se hace solo una vez por hoja para copia y llenado.
                    hubContext.Clients.All.recibirProgresoExcel(76, 76, 100, $"Llenando datos a {cabeceraAniosFY_conMeses[i].text} ({i + 1}/{cabeceraAniosFY_conMeses.Count})");
                    oSLDocument.SelectWorksheet(newSheetName);
                    System.Diagnostics.Debug.WriteLine($"[TIMER] Copia y Selección de hoja {newSheetName}: {timer.Elapsed.TotalSeconds:F2} segundos");

                    timer.Restart();
                    // --- BLOQUE A: Fórmulas de referencia a la primera hoja (usando DataTable) ---
                    if (i != 0) // Esta lógica solo se ejecuta para las hojas secundarias
                    {
                        var dtReferencias = new System.Data.DataTable();
                        int numColumnasEstaticas = numInicioColumnaDatosBase - 3; // k=1 hasta k < num...-2

                        // 1. Definir columnas del DataTable
                        for (int c = 0; c < numColumnasEstaticas; c++) { dtReferencias.Columns.Add(); }

                        int itemIndex = 0; // Contador de filas para esta operación
                        foreach (var forecast_Item in reporte.BG_Forecast_item)
                        {
                            var nuevaFilaRef = dtReferencias.NewRow();
                            int excelRowNum = 5 + itemIndex; // Fila real en Excel para la fórmula

                            // 2. Llenar la fila del DataTable con fórmulas
                            for (int k = 1; k < numInicioColumnaDatosBase - 2; k++)
                            {
                                string formula = $"='{FirstSheetName}'!{GetCellReference(k)}{excelRowNum}";
                                nuevaFilaRef[k - 1] = formula; // k-1 para índice base 0
                            }
                            dtReferencias.Rows.Add(nuevaFilaRef);
                            itemIndex++;
                        }

                        // 3. Escribir todo el DataTable en la hoja de una sola vez
                        oSLDocument.ImportDataTable(5, 1, dtReferencias, false);
                    }

                    System.Diagnostics.Debug.WriteLine($"[TIMER] {newSheetName} - Bloque A (Referencias Estáticas): {timer.Elapsed.TotalSeconds:F2} segundos");
                    timer.Restart();

                    // --- BLOQUE B: Fórmulas de demanda mensual (usando DataTable) ---
                    var dtDemanda = new System.Data.DataTable();
                    for (int j = 0; j < 12; j++) { dtDemanda.Columns.Add(); } // Añadir 12 columnas para los meses

                    int itemIndexDemanda = 0; // Contador de filas para esta operación
                    foreach (var forecast_Item in reporte.BG_Forecast_item)
                    {
                        var nuevaFilaDemanda = dtDemanda.NewRow();
                        int excelRowNum = 5 + itemIndexDemanda; // Fila real en Excel

                        DateTime mesFYItem = new DateTime(cabeceraAniosFY_conMeses[i].anio, 10, 1);
                        for (int j = 0; j < 12; j++)
                        {
                            referenciaColumnasDict.TryGetValue(mesFYItem, out var columnRef);

                            DateTime? inicioDemanda = forecast_Item.inicio_demanda;
                            DateTime? finDemanda = forecast_Item.fin_demanda;
                            bool fueraDeRango = (inicioDemanda.HasValue && mesFYItem < inicioDemanda.Value) ||
                                                (finDemanda.HasValue && mesFYItem > finDemanda.Value);

                            if (fueraDeRango)
                            {
                                nuevaFilaDemanda[j] = "=\"--\"";
                            }
                            else if (columnRef != null && !string.IsNullOrEmpty(columnRef.celdaReferencia))
                            {

                                nuevaFilaDemanda[j] = $"=IF({refA_D.celdaReferencia}{excelRowNum} = \"A\", IFERROR(INDEX('{hoja2}'!{columnRef.celdaReferencia}:{columnRef.celdaReferencia}, MATCH({claveRef}{excelRowNum}, '{hoja2}'!B:B, 0)), \"N/D\"), \"--\")";
                                //nuevaFilaDemanda[j] = $"=IF({refA_D.celdaReferencia}{excelRowNum} = \"A\", IFERROR(INDEX('{hoja2}'!{columnRef.celdaReferencia}:{columnRef.celdaReferencia}, MATCH({claveRef}{excelRowNum}, '{hoja2}'!B:B, 0)), \"N/D\"), \"--\")";
                            }
                            else
                            {
                                nuevaFilaDemanda[j] = "=\"--\"";
                            }

                            mesFYItem = mesFYItem.AddMonths(1);
                        }
                        dtDemanda.Rows.Add(nuevaFilaDemanda);
                        itemIndexDemanda++;
                    }
                    // Escribe todo el bloque de demanda mensual de una vez
                    oSLDocument.ImportDataTable(5, numInicioColumnaDatosBase, dtDemanda, false);

                    System.Diagnostics.Debug.WriteLine($"[TIMER] {newSheetName} - Bloque B (Demanda Mensual): {timer.Elapsed.TotalSeconds:F2} segundos");
                    timer.Restart();

                    #region valores clientes


                    // Preprocesar el historial de ventas en un diccionario para acceso rápido
                    for (int indexCliente = 0; indexCliente < listClientes.Count; indexCliente++)
                    {
                        var cliente = listClientes[indexCliente];
                        int filaActual = filaCliente + indexCliente;

                        // Escribir cliente en la celda
                        oSLDocument.SetCellValue(filaActual, numInicioColumnaTotalSales - 1, cliente.descripcion);
                        oSLDocument.SetCellStyle(filaActual, numInicioColumnaTotalSales - 1, styleHighlightGray);

                        // Obtener histórico de ventas del cliente
                        var historicoCliente = historicoVentasDict[cliente.id];

                        // Inicializar mesFYC en octubre del año fiscal
                        DateTime mesFYC = new DateTime(cabeceraAniosFY_conMeses[i].anio, 10, 1);

                        for (int j = 0; j < 12; j++)
                        {
                            // Filtra los valores del mes actual
                            var historicoMes = historicoCliente.Where(x => x.fecha == mesFYC).ToList();

                            // Método inline para obtener valores con `FirstOrDefault`
                            double ObtenerValor(int seccion, double valorPorDefecto = 0) =>
                                historicoMes.FirstOrDefault(x => x.id_seccion == seccion)?.valor ?? valorPorDefecto;

                            // Diccionario de columnas y estilos para reducir llamadas repetitivas
                            var columnasValores = new Dictionary<int, System.Tuple<int, SLStyle>>
                                {
                                    { numInicioColumnaTotalSales + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.TOTAL_SALES_TUSD, styleCurrencyLinea) },
                                    { numInicioColumnaMaterialCost + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.MATERIAL_COST_TUSD, styleCurrencyLinea) },
                                    { numInicioColumnaCostOfOutsiteP + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.COST_OF_OUTSIDE_PROCESSOR_PART_TUSD, styleCurrencyLinea) },
                                    { numInicioColumnaValueAddSales + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.VALUE_ADDED_SALES_TUSD, styleCurrencyLinea) },
                                    { numInicioColumnaProccessTon + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.PROCESSED_TONS_TO, styleNumericLine) },
                                    { numInicioColumnaEngScrap + 1 + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.ENGINEERED_SCRAP_TO, styleNumericLine) },
                                    { numInicioColumnaScrapConsolidation + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.SCRAP_CONSOLIDATION_TO, styleNumericLine) },
                                    { numInicioColumnaStrokes + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.STROKES, styleNumericLine) },
                                    { numInicioColumnaBlanks + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.BLANKS, styleNumericLine) },
                                    { numInicioColumnaAditionalMaterialCost + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.ADDITIONAL_MATERIAL_COST_TOTAL_USD, styleCurrencyLinea) },
                                    { numInicioColumnaOutgoingFreightTotal + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.OUTGOING_FREIGHT_TOTAL_USD, styleCurrencyLinea) },
                                    { numInicioColumnaInventoryOWNAverage + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.INVENTORY_OWN_MONTHLY_AVERAGE_TONS, styleNumericLine) },
                                    { numInicioColumnaInventoryEndMonth + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.INVENTORY_END_OF_MONTH_USD, styleCurrencyLinea) },
                                    { numInicioColumnaFreightsIncomeUSD + 1 + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.FREIGHTS_INCOME_USD_PART, styleCurrencyLinea) },
                                    { numInicioColumnaManiobrasPart + 1 + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.MANIOBRAS_USD_PART, styleCurrencyLinea) },
                                    { numInicioColumnaCustomExpenses + 1 + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.CUSTOMS_EXPENSES_USD_PART, styleCurrencyLinea) },
                                    { numInicioColumnaShipmentTons + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.SHIPMENT_TONS_TO, styleNumericLine) },
                                    { numInicioColumnaSalesIncScrap + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.SALES_INC_SCRAP_TUSD, styleCurrencyLinea) },
                                    { numInicioColumnaVasIncScrap + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.VAS_INC_SCRAP_TUSD, styleCurrencyLinea) },
                                    { numInicioColumnaProcessingIncScrap + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.PROCESSING_INC_SCRAP, styleCurrencyLinea) },
                                    { numInicioColumnaWoodenPallets + 1 + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.WOODEN_PALLETS, styleCurrencyLinea) },
                                    { numInicioColumnaStandardPackaging + 1 + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.STANDARD_PACKAGING, styleCurrencyLinea) },
                                    { numInicioColumnaPlasticStrips + 1 + j, System.Tuple.Create((int)BG_forecast_seccion_calculoEnum.PLASTIC_STRIPS, styleCurrencyLinea) }
                                };

                            // Iterar sobre las columnas predefinidas y aplicar valores
                            foreach (var columna in columnasValores)
                            {
                                int columnaExcel = columna.Key;
                                int seccionEnum = columna.Value.Item1;
                                SLStyle estilo = columna.Value.Item2;

                                oSLDocument.SetCellValue(filaActual, columnaExcel, ObtenerValor(seccionEnum));
                                oSLDocument.SetCellStyle(filaActual, columnaExcel, estilo);
                            }

                            // Avanza el mes
                            mesFYC = mesFYC.AddMonths(1);
                        }
                    }


                    #endregion

                    System.Diagnostics.Debug.WriteLine($"[TIMER] {newSheetName} - Bloque C (Valores Clientes): {timer.Elapsed.TotalSeconds:F2} segundos");
                    timer.Restart();

                    #region ValoresVentaScrap por planta

                    //filaTablaScrapPorPlanta

                    for (int ic = 0; ic < listaCombinacionesScrap.Count; ic++)
                    {
                        //titulo
                        oSLDocument.SetCellValue(filaTablaScrapPorPlanta + ic, numInicioColumnaEngScrap, listaCombinacionesScrap[ic]);

                        //formula por cada mes
                        for (int im = 1; im <= 12; im++)
                        {
                            int filaInicio = 5;
                            string colf = GetCellReference(numInicioColumnaEngScrap);
                            string coln = GetCellReference(numInicioColumnaEngScrap + im);
                            //oSLDocument.SetCellValue(filaTablaScrapPorPlanta + ic, numInicioColumnaEngScrap + im, "=SUMIF($DX$5:$DX$69,$DX72, DY$5:DY$69)");
                            oSLDocument.SetCellValue(filaTablaScrapPorPlanta + ic, numInicioColumnaEngScrap + im,
                               "=SUMIF($" + colf + "$" + filaInicio + ":$" + colf + "$" + (reporte.BG_Forecast_item.Count + filaInicio) + ",$" + colf + (filaTablaScrapPorPlanta + ic).ToString()
                               + ", " + coln + "$" + 5 + ":" + coln + "$" + (reporte.BG_Forecast_item.Count + filaInicio) + ")");
                        }
                    }

                    #endregion
                    System.Diagnostics.Debug.WriteLine($"[TIMER] {newSheetName} - Bloque D (Venta Scrap): {timer.Elapsed.TotalSeconds:F2} segundos");
                    timer.Restart();

                    #region valores scrap
                    //// AGREGA LOS VALORES DE SCRAP////////


                    //agrega los valores de scrap
                    DateTime mesFY = new DateTime(cabeceraAniosFY_conMeses[i].anio, 10, 01); //octubre del FY

                    oSLDocument.SetCellValue(filaScrapSteel, numInicioColumnaTotalSales - 1, "STEEL SCRAP (Valor venta)");
                    oSLDocument.SetCellValue(filaScrapAlu, numInicioColumnaTotalSales - 1, "ALU SCRAP (Valor venta)");
                    oSLDocument.SetCellStyle(filaScrapSteel, numInicioColumnaTotalSales - 1, styleHighlight);
                    oSLDocument.SetCellStyle(filaScrapAlu, numInicioColumnaTotalSales - 1, styleHighlight);
                    oSLDocument.AutoFitColumn(numInicioColumnaTotalSales - 1);

                    //lista las combinaciones 
                    int indexC = 0;
                    foreach (var combinacion in listaCombinacionesScrap.Where(x => x.Contains("STEEL")))
                    {
                        indexC++;
                        oSLDocument.SetCellValue(filaScrapSteel + indexC, numInicioColumnaTotalSales - 1, combinacion);
                    }
                    indexC = 0;
                    foreach (var combinacion in listaCombinacionesScrap.Where(x => x.Contains("ALU")))
                    {
                        indexC++;
                        oSLDocument.SetCellValue(filaScrapAlu + indexC, numInicioColumnaTotalSales - 1, combinacion);
                    }


                    for (int j = 0; j < 12; j++) //solo realiza una vez
                    {
                        //OBTENER EL VALOR DESDE BASE DE DATOS DEL SCRAP
                        var scrapSteel = historicoScrap.FirstOrDefault(x => x.id_planta == 1 && x.tipo_metal == "STEEL" && x.fecha.Year == mesFY.Year && x.fecha.Month == mesFY.Month);
                        var scrapAlu = historicoScrap.FirstOrDefault(x => x.id_planta == 2 && x.tipo_metal == "ALU" && x.fecha.Year == mesFY.Year && x.fecha.Month == mesFY.Month);

                        double valorSteel = 0;
                        double valorAlu = 0;
                        double valorGananciaSteel = 0;
                        double valorGananciaAlu = 0;

                        //obtiene el  valor del acero
                        if (scrapSteel != null)
                        {
                            valorSteel = scrapSteel.scrap.Value;
                            valorGananciaSteel = scrapSteel.scrap_ganancia;
                        }
                        else //valor por defecto
                        {
                            valorSteel = defaultScrap.scrap_acero_valor_puebla;
                            valorGananciaSteel = defaultScrap.scrap_acero_ganancia_puebla;
                        }
                        //

                        //obtiene el  valor del alumnio
                        if (scrapAlu != null)
                        {
                            valorAlu = scrapAlu.scrap.Value;
                            valorGananciaAlu = scrapAlu.scrap_ganancia;
                        }
                        else //valor por defecto
                        {
                            valorAlu = defaultScrap.scrap_aluminio_valor_silao;
                            valorGananciaAlu = defaultScrap.scrap_aluminio_ganancia_silao;
                        }
                        //valor de acero
                        oSLDocument.SetCellValue(filaScrapSteel, numInicioColumnaTotalSales + j, valorSteel);
                        oSLDocument.SetCellStyle(filaScrapSteel, numInicioColumnaTotalSales + j, styleCurrency);
                        oSLDocument.SetCellStyle(filaScrapSteel, numInicioColumnaTotalSales + j, styleHighlight);
                        oSLDocument.SetCellValue(filaScrapSteel, numInicioColumnaMaterialCost + j, "=" + GetCellReference(numInicioColumnaTotalSales + j) + filaScrapSteel + "-" + valorGananciaSteel);
                        oSLDocument.SetCellStyle(filaScrapSteel, numInicioColumnaMaterialCost + j, styleHighlight);

                        //formulas para acero
                        indexC = 0;
                        foreach (var combinacion in listaCombinacionesScrap.Where(x => x.Contains("STEEL")))
                        {
                            indexC++;
                            //=BK$109*BUSCARV($BJ110,$DX$73:$EJ$87,2,FALSO)/1000
                            string vlookup = "=" + GetCellReference(numInicioColumnaTotalSales + j) + "$" + filaScrapSteel + "* VLOOKUP($" + GetCellReference(numInicioColumnaTotalSales - 1) + (filaScrapSteel + indexC) +
                                ",$" + GetCellReference(numInicioColumnaEngScrap) + "$" + filaTablaScrapPorPlanta + ":$" + GetCellReference(numInicioColumnaEngScrap + 12) + "$" + (filaTablaScrapPorPlanta + listaCombinacionesScrap.Count() - 1) + "," + (1 + indexC) + ",FALSE)/1000";
                            oSLDocument.SetCellValue(filaScrapSteel + indexC, numInicioColumnaTotalSales + j, vlookup);
                            string vlookupGanancia = "=" + GetCellReference(numInicioColumnaMaterialCost + j) + "$" + filaScrapSteel + "* VLOOKUP($" + GetCellReference(numInicioColumnaTotalSales - 1) + (filaScrapSteel + indexC) +
                              ",$" + GetCellReference(numInicioColumnaEngScrap) + "$" + filaTablaScrapPorPlanta + ":$" + GetCellReference(numInicioColumnaEngScrap + 12) + "$" + (filaTablaScrapPorPlanta + listaCombinacionesScrap.Count() - 1) + "," + (1 + indexC) + ",FALSE)/1000";
                            oSLDocument.SetCellValue(filaScrapSteel + indexC, numInicioColumnaMaterialCost + j, vlookupGanancia);
                        }

                        //valor de alu
                        oSLDocument.SetCellValue(filaScrapAlu, numInicioColumnaTotalSales + j, valorAlu);
                        oSLDocument.SetCellStyle(filaScrapAlu, numInicioColumnaTotalSales + j, styleCurrency);
                        oSLDocument.SetCellStyle(filaScrapAlu, numInicioColumnaTotalSales + j, styleHighlight);
                        oSLDocument.SetCellValue(filaScrapAlu, numInicioColumnaMaterialCost + j, "=" + GetCellReference(numInicioColumnaTotalSales + j) + filaScrapAlu + "-" + valorGananciaAlu);
                        oSLDocument.SetCellStyle(filaScrapAlu, numInicioColumnaMaterialCost + j, styleHighlight);


                        //formulas para alu
                        indexC = 0;
                        foreach (var combinacion in listaCombinacionesScrap.Where(x => x.Contains("ALU")))
                        {
                            indexC++;
                            //=BK$109*BUSCARV($BJ110,$DX$73:$EJ$87,2,FALSO)/1000
                            string vlookup = "=" + GetCellReference(numInicioColumnaTotalSales + j) + "$" + filaScrapAlu + "* VLOOKUP($" + GetCellReference(numInicioColumnaTotalSales - 1) + (filaScrapAlu + indexC) +
                                ",$" + GetCellReference(numInicioColumnaEngScrap) + "$" + filaTablaScrapPorPlanta + ":$" + GetCellReference(numInicioColumnaEngScrap + 12) + "$" + (filaTablaScrapPorPlanta + listaCombinacionesScrap.Count() - 1) + "," + (1 + indexC) + ",FALSE)/1000";
                            oSLDocument.SetCellValue(filaScrapAlu + indexC, numInicioColumnaTotalSales + j, vlookup);
                            string vlookupGanancia = "=" + GetCellReference(numInicioColumnaMaterialCost + j) + "$" + filaScrapAlu + "* VLOOKUP($" + GetCellReference(numInicioColumnaTotalSales - 1) + (filaScrapAlu + indexC) +
                           ",$" + GetCellReference(numInicioColumnaEngScrap) + "$" + filaTablaScrapPorPlanta + ":$" + GetCellReference(numInicioColumnaEngScrap + 12) + "$" + (filaTablaScrapPorPlanta + listaCombinacionesScrap.Count() - 1) + "," + (1 + indexC) + ",FALSE)/1000";
                            oSLDocument.SetCellValue(filaScrapAlu + indexC, numInicioColumnaMaterialCost + j, vlookupGanancia);
                        }


                        mesFY = mesFY.AddMonths(1);
                    }

                    /////////////
                    #endregion

                    System.Diagnostics.Debug.WriteLine($"[TIMER] {newSheetName} - Bloque E (Histórico Scrap): {timer.Elapsed.TotalSeconds:F2} segundos");
                    timer.Restart();
                    int columnaActual = numInicioColumnaDatosBase;


                    //actualiza los titulos
                    for (int k = 0; k < tablasReferenciasIniciales.Count; k++)
                    {
                        int extra = !String.IsNullOrEmpty(tablasReferenciasIniciales[k].extra) ? 1 : 0;

                        //crea las columnas para los doce meses del año fiscal
                        DateTime mesFY2 = new DateTime(cabeceraAniosFY_conMeses[i].anio, 10, 01); //octubre del FY

                        for (int j = 0; j < 12; j++)
                        {
                            oSLDocument.SetCellValue(4, columnaActual + extra + +j, mesFY2.ToString("MMM yyyy").ToUpper().Replace(".", String.Empty));
                            //aumenta un mes
                            mesFY2 = mesFY2.AddMonths(1);
                        }

                        columnaActual += (13 + extra);
                    }
                    System.Diagnostics.Debug.WriteLine($"[TIMER] {newSheetName} - Bloque F (Actualizar Títulos): {timer.Elapsed.TotalSeconds:F2} segundos");

                }


                //CREA LOS DATOS DEL RESUMEN DE fy
                #region resumen FY
                {
                    hubContext.Clients.All.recibirProgresoExcel(77, 77, 100, $"Iniciando Hoja de Resumen");


                    // crea la hoja de resumen
                    string hojaResumen = cabeceraAniosFY_conMeses[0].text + " until " + cabeceraAniosFY_conMeses[cabeceraAniosFY_conMeses.Count - 1].text;
                    oSLDocument.AddWorksheet(hojaResumen);

                    oSLDocument.SelectWorksheet("Aux"); //selecciona la hoja aux para no tener detalles a la hora de copiar la hoja actual
                    oSLDocument.CopyWorksheet(cabeceraAniosFY_conMeses[0].text + " by Month", hojaResumen);


                    //borra los datos despues de copiar
                    oSLDocument.SelectWorksheet(hojaResumen);
                    oSLDocument.DeleteColumn(numInicioColumnaDatosBase, 400);
                    oSLDocument.ClearConditionalFormatting();

                    //crea un ambito para las variables
                    Debug.WriteLine(oSLDocument.GetCurrentWorksheetName());
                    int columnaActual = numInicioColumnaDatosBase;

                    numInicioColumnaTotalSales = 0;
                    numInicioColumnaMaterialCost = 0;
                    numInicioColumnaCostOfOutsiteP = 0;
                    numInicioColumnaEngScrap = 0;
                    numInicioColumnaScrapConsolidation = 0;
                    numInicioColumnaProccessTon = 0;
                    numInicioColumnaInventoryOWNAverage = 0;
                    numInicioColumnaFreightsIncomeUSD = 0;
                    numInicioColumnaOutgoingFreightTotal = 0;
                    numInicioColumnaAditionalMaterialCost = 0;
                    numInicioColumnaManiobrasPart = 0;
                    int numInicioCustomExpenses = 0;

                    // Un diccionario para mapear las descripciones de las tablas a su columna de inicio.
                    // Se llena una vez antes de que comience el bucle principal.
                    var columnMappingSummary = new Dictionary<string, int>();
                    int tempColumnaActual = columnaActual; // Inicia con la columna actual global

                    foreach (var t in tablasReferenciasIniciales)
                    {
                        int extra = !string.IsNullOrEmpty(t.extra) ? 1 : 0;
                        columnMappingSummary.Add(t.celdaDescripcion, tempColumnaActual);

                        // Guardamos la posición de inicio para las variables importantes.
                        int indexTabla = tablasReferenciasIniciales.IndexOf(t);
                        switch (indexTabla)
                        {
                            case 0: numInicioColumnaDatosBase = tempColumnaActual; break;
                            case 1: numInicioColumnaTotalSales = tempColumnaActual; break;
                            case 2: numInicioColumnaMaterialCost = tempColumnaActual; break;
                            case 3: numInicioColumnaCostOfOutsiteP = tempColumnaActual; break;
                            case 4: numInicioColumnaValueAddSales = tempColumnaActual; break;
                            case 5: numInicioColumnaProccessTon = tempColumnaActual; break;
                            case 6: numInicioColumnaEngScrap = tempColumnaActual; break;
                            case 7: numInicioColumnaScrapConsolidation = tempColumnaActual; break;
                            case 8: numInicioColumnaStrokes = tempColumnaActual; break;
                            case 9: numInicioColumnaBlanks = tempColumnaActual; break;
                            case 10: numInicioColumnaAditionalMaterialCost = tempColumnaActual; break;
                            case 11: numInicioColumnaOutgoingFreightTotal = tempColumnaActual; break;
                            case 12: numInicioColumnaInventoryOWNAverage = tempColumnaActual; break;
                            case 13: numInicioColumnaInventoryEndMonth = tempColumnaActual; break;
                            case 14: numInicioColumnaFreightsIncomeUSD = tempColumnaActual; break;
                            case 15: numInicioColumnaManiobrasPart = tempColumnaActual; break;
                            case 16: numInicioColumnaCustomExpenses = tempColumnaActual; break;
                            case 17: numInicioColumnaShipmentTons = tempColumnaActual; break;
                            case 18: numInicioColumnaSalesIncScrap = tempColumnaActual; break;
                            case 19: numInicioColumnaVasIncScrap = tempColumnaActual; break;
                            case 20: numInicioColumnaProcessingIncScrap = tempColumnaActual; break;
                            case 21: numInicioColumnaWoodenPallets = tempColumnaActual; break;
                            case 22: numInicioColumnaStandardPackaging = tempColumnaActual; break;
                            case 23: numInicioColumnaPlasticStrips = tempColumnaActual; break;
                        }

                        tempColumnaActual += cabeceraAniosFY_conMeses.Count + extra;
                    }

                    foreach (var t in tablasReferenciasIniciales)
                    {

                        hubContext.Clients.All.recibirProgresoExcel(80, 80, 100, $"Resumen FY: {t.celdaDescripcion} ({tablasReferenciasIniciales.IndexOf(t) + 1}/{tablasReferenciasIniciales.Count})");

                        dt = new System.Data.DataTable();
                        int indexTabla = tablasReferenciasIniciales.IndexOf(t);

                        //asigna el valor de la columna actual al valor de la referencia inicial de latabla
                        t.columnaNum = columnaActual;

                        int extra = !String.IsNullOrEmpty(t.extra) ? 1 : 0;

                        //agrega la columna extra si es necesario
                        if (extra == 1)
                        {
                            t.columnaNum = columnaActual + 1;
                            dt.Columns.Add(t.extra, typeof(double));
                        }

                        foreach (var fy in cabeceraAniosFY_conMeses)
                        {
                            dt.Columns.Add(fy.text);
                        }
                        // Variable para la referencia de la columna de tipo de material.
                        string colMaterialRef = dictionaryTitulosByMonth.FirstOrDefault(x => x.Value == _MATERIAL_TYPE_SHORT).Key;

                        int excelRowNum = 5;

                        foreach (var forecast_Item in reporte.BG_Forecast_item)
                        {
                            System.Data.DataRow row = dt.NewRow();

                            for (int j = 0; j < cabeceraAniosFY_conMeses.Count; j++)
                            {
                                var fy = cabeceraAniosFY_conMeses[j];
                                string nombreCelda = fy.text;
                                string formula = "=\"--\""; // Fórmula por defecto

                                // Obtenemos la referencia a la columna de 'Autos/Month' para el FY actual.
                                string autosMonthColRef = GetCellReference(numInicioColumnaDatosBase + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[0].extra) ? 0 : 1));

                                switch (indexTabla)
                                {
                                    case 0: // autos/month
                                        var columnRef = referenciaColumnasFY.FirstOrDefault(x => x.fecha.Year + 1 == fy.anio);
                                        if (columnRef != null)
                                        {
                                            formula = $"=IF(${refA_D.celdaReferencia}{excelRowNum}=\"A\", IFERROR(INDEX('{hoja2}'!{columnRef.celdaReferencia}:{columnRef.celdaReferencia}, MATCH(${claveRef}{excelRowNum}, '{hoja2}'!$B:$B, 0)), \"N/D\"), \"--\")";
                                        }
                                        break;
                                    case 1: // Total Sales [TUSD]
                                        formula = $"=IFERROR(IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refVentasPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${refVentasPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum}),\"--\")";
                                        break;
                                    case 2: // Material Cost
                                        formula = $"=IFERROR(IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refMaterialCostPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${refMaterialCostPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum}),\"--\")";
                                        break;
                                    case 3: // cost of out side procesor
                                        formula = $"=IFERROR(IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refCostOfOutsideProcessor.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${refCostOfOutsideProcessor.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum}),\"--\")";
                                        break;
                                    case 4: // Value Added Sales[TUSD]
                                        string totalSalesCol = GetCellReference(numInicioColumnaTotalSales + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[1].extra) ? 0 : 1));
                                        string materialCostCol = GetCellReference(numInicioColumnaMaterialCost + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[2].extra) ? 0 : 1));
                                        string costOutsideCol = GetCellReference(numInicioColumnaCostOfOutsiteP + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[3].extra) ? 0 : 1));
                                        formula = $"=IFERROR(({totalSalesCol}{excelRowNum}-{materialCostCol}{excelRowNum}-{costOutsideCol}{excelRowNum}),\"--\")";
                                        break;
                                    case 5: // Processed Tons [to]
                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum}=\"WLD\",0,IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refInitialWeightPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${refInitialWeightPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum})),\"--\")";
                                        break;
                                    case 6: // Engineered Scrap [to]
                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum}=\"WLD\",0, ${refEngineeredScrap.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum}),\"--\")";
                                        break;
                                    case 7: // scrapConsolidation
                                            // Obtenemos la referencia a la columna de 'Engineered Scrap' para el FY actual,
                                            // considerando si esa tabla (índice 6) tiene una columna "extra".
                                        string engScrapCol = GetCellReference(numInicioColumnaEngScrap + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[6].extra) ? 0 : 1));

                                        formula = $"=IFERROR(IF(${refScrapConsolidation.celdaReferencia}{excelRowNum}=\"YES\", -{engScrapCol}{excelRowNum}, 0),\"--\")";
                                        break;
                                    case 8: // Strokes [ - / 1000 ]
                                            // Fórmula original: =SI($AA88="WLD",0,$Y88*AW88/1000)
                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum}=\"WLD\",0, ${refStrokesAuto.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000),\"--\")";
                                        break;

                                    case 9: // Blanks [ - / 1000 ]
                                            // Fórmula original: =SI($AA28="WLD",0,$X28*AW28/1000)
                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum}=\"WLD\",0, ${refPartsAuto.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000),\"--\")";
                                        break;

                                    case 10: // Additional material Cost [TUSD]
                                             // Fórmula original: =SI($D28="C&SLT",$AL28*AW28/1000,$AL28*AW28/1000*$X28)
                                        formula = $"=IFERROR(IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refAdditionalMaterialCostPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${refAdditionalMaterialCostPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum}),\"--\")";
                                        break;
                                    case 11: // Outgoing freight total [USD]
                                             // Fórmula original: =SI($D28="C&SLT",$AM28*AW28/1000,$AM28*AW28/1000*$X28)
                                        formula = $"=IFERROR(IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refOutgoingFreightPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${refOutgoingFreightPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum}),\"--\")";
                                        break;

                                    case 12: // Inventory OWN (monthly average) [tons]
                                             // Esta es una fórmula especial que no calcula, sino que hace referencia a un valor de otra hoja.
                                        if (oSLDocument.GetSheetNames().Any(x => x.StartsWith(fy.text)))
                                        {
                                            // Referencia al valor de septiembre (último mes del FY) de la hoja mensual correspondiente.
                                            formula = $"='{fy.text} by Month'!{GetCellReference(inicioInventoryOwnMeses + 11)}{excelRowNum}";
                                        }
                                        // Si la hoja "[FY] by Month" no existe, la fórmula se queda como "--" (valor por defecto).
                                        break;

                                    case 13: // Inventory OWN (end of month) [USD]
                                             // Fórmula original: =GS7*($AF7+$AI7)/$Z7
                                             // Depende del resultado del caso 12.

                                        // Obtenemos la referencia a la columna de 'Inventory OWN (monthly average)' para el FY actual.
                                        string invOwnAvgCol = GetCellReference(numInicioColumnaInventoryOWNAverage + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[12].extra) ? 0 : 1));

                                        formula = $"=IFERROR({invOwnAvgCol}{excelRowNum}*(${refMaterialCostPart.celdaReferencia}{excelRowNum}+${refAdditionalMaterialCostPart.celdaReferencia}{excelRowNum})/${refInitialWeightPart.celdaReferencia}{excelRowNum},\"--\")";
                                        break;

                                    case 14: // Freights Income USD/PART
                                             // Fórmula original: =SI.ERROR(SI($AA28="WLD",0,SI($D28="C&SLT",$HW28*AW28/1000,$HW28*AW28/1000*$X28)),"--")
                                             // Esta fórmula usa el valor de la columna "extra" de esta misma tabla.

                                        // Obtenemos la referencia a la columna "extra" de esta tabla.
                                        string freightsIncomeExtraCol = GetCellReference(numInicioColumnaFreightsIncomeUSD);

                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum} = \"WLD\", 0, IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${freightsIncomeExtraCol}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${freightsIncomeExtraCol}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum})), \"--\")";
                                        break;

                                    case 15: // Freights Maniobras USD/PART
                                             // Fórmula original: =SI.ERROR(SI($AA28="WLD",0,SI($D28="C&SLT",$IK28*AW28/1000,$IK28*AW28/1000*$X28)),"--")
                                             // Similar al caso 14, usa su propia columna "extra".

                                        // Obtenemos la referencia a la columna "extra" de esta tabla.
                                        string maniobrasExtraCol = GetCellReference(numInicioColumnaManiobrasPart);

                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum} = \"WLD\", 0, IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${maniobrasExtraCol}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${maniobrasExtraCol}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum})), \"--\")";
                                        break;
                                    case 16: // Customs Expenses USD/PART
                                             // Fórmula original: =SI.ERROR(SI($AA28="WLD",0,SI($D28="C&SLT",$IY28*AW28/1000,$IY28*AW28/1000*$X28)),"--")
                                             // Usa su propia columna "extra" para el costo.
                                        string customsExtraCol = GetCellReference(numInicioColumnaCustomExpenses);

                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum} = \"WLD\", 0, IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${customsExtraCol}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${customsExtraCol}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum})), \"--\")";
                                        break;

                                    case 17: // Shipment Tons[to]
                                             // Fórmula original: =SI.ERROR(SI($AA28="WLD",0,SI($D28="C&SLT",$AC28*AW28/1000,$AD28*AW28/1000*$X28)),"--")
                                        formula = $"=IFERROR(IF(${refShape.celdaReferencia}{excelRowNum} = \"WLD\", 0, IF(${refBusinnessAndPlant.celdaReferencia}{excelRowNum} = \"C&SLT\", ${refInitialWeightPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000, ${refNetWeightPart.celdaReferencia}{excelRowNum}*{autosMonthColRef}{excelRowNum}/1000*${refPartsAuto.celdaReferencia}{excelRowNum})), \"--\")";
                                        break;

                                    case 18: // SALES Inc. SCRAP[USD]
                                             // Fórmula original: =SI($AF7="STEEL",BN7+EA7*BN$1384/1000,BN7+EA7*BN$1389/1000)
                                             // Depende de 'Total Sales' (idx 1) y 'Engineered Scrap' (idx 6)
                                        string totalSalesCol18 = GetCellReference(numInicioColumnaTotalSales + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[1].extra) ? 0 : 1));
                                        string engScrapCol18 = GetCellReference(numInicioColumnaEngScrap + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[6].extra) ? 0 : 1));

                                        formula = $"=IFERROR(IF(${colMaterialRef}{excelRowNum}=\"STEEL\", {totalSalesCol18}{excelRowNum}+{engScrapCol18}{excelRowNum}*{totalSalesCol18}${filaScrapSteel}/1000, {totalSalesCol18}{excelRowNum}+{engScrapCol18}{excelRowNum}*{totalSalesCol18}${filaScrapAlu}/1000), \"--\")";
                                        break;

                                    case 19: // VAS Inc. SCRAP [USD]
                                             // Fórmula original: =SI($AF7="STEEL",DA7+EA7*BN$1384/1000+EN7*CA$1384/1000,DA7+EA7*BN$1389/1000+EN7*CA$1389/1000)
                                             // Depende de múltiples tablas.
                                        string valAddSales = GetCellReference(numInicioColumnaValueAddSales + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[4].extra) ? 0 : 1));
                                        string engScrap19 = GetCellReference(numInicioColumnaEngScrap + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[6].extra) ? 0 : 1));
                                        string scrapCons = GetCellReference(numInicioColumnaScrapConsolidation + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[7].extra) ? 0 : 1));
                                        string totalSales19 = GetCellReference(numInicioColumnaTotalSales + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[1].extra) ? 0 : 1));
                                        string materialCost19 = GetCellReference(numInicioColumnaMaterialCost + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[2].extra) ? 0 : 1));

                                        formula = $"=IFERROR(IF(${colMaterialRef}{excelRowNum}=\"STEEL\", {valAddSales}{excelRowNum}+{engScrap19}{excelRowNum}*{totalSales19}${filaScrapSteel}/1000+{scrapCons}{excelRowNum}*{materialCost19}${filaScrapSteel}/1000, {valAddSales}{excelRowNum}+{engScrap19}{excelRowNum}*{totalSales19}${filaScrapAlu}/1000+{scrapCons}{excelRowNum}*{materialCost19}${filaScrapAlu}/1000), \"--\")";
                                        break;

                                    case 20: // Processing Inc. SCRAP
                                             // Fórmula original: =+KS7-GA7-GN7
                                             // Depende de 'VAS Inc. SCRAP' (idx 19), 'Additional material cost' (idx 10) y 'Outgoing freight' (idx 11).
                                        string vasIncScrap = GetCellReference(numInicioColumnaVasIncScrap + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[19].extra) ? 0 : 1));
                                        string addMatCost = GetCellReference(numInicioColumnaAditionalMaterialCost + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[10].extra) ? 0 : 1));
                                        string outFreight = GetCellReference(numInicioColumnaOutgoingFreightTotal + j + (string.IsNullOrEmpty(tablasReferenciasIniciales[11].extra) ? 0 : 1));

                                        formula = $"=IFERROR({vasIncScrap}{excelRowNum}-{addMatCost}{excelRowNum}-{outFreight}{excelRowNum}, \"--\")";
                                        break;

                                    case 21: // Wooden pallets
                                             // Fórmula original: =+$LU7*BA7*$AB7/1000
                                             // Usa su propia columna "extra" para el costo.
                                        string woodenPalletsExtraCol = GetCellReference(numInicioColumnaWoodenPallets);

                                        formula = $"=IFERROR(${woodenPalletsExtraCol}{excelRowNum}*{autosMonthColRef}{excelRowNum}*${refPartsAuto.celdaReferencia}{excelRowNum}/1000, \"--\")";
                                        break;

                                    case 22: // Standard packaging
                                             // Fórmula original: =+$MI7*BA7*$AB7/1000
                                             // Usa su propia columna "extra" para el costo.
                                        string standardPackagingExtraCol = GetCellReference(numInicioColumnaStandardPackaging);

                                        formula = $"=IFERROR(${standardPackagingExtraCol}{excelRowNum}*{autosMonthColRef}{excelRowNum}*${refPartsAuto.celdaReferencia}{excelRowNum}/1000, \"--\")";
                                        break;

                                    case 23: // PLASTIC STRIPS
                                             // Fórmula original: =+$MW7*BA7*$AB7/1000
                                             // Usa su propia columna "extra" para el costo.
                                        string plasticStripsExtraCol = GetCellReference(numInicioColumnaPlasticStrips);

                                        formula = $"=IFERROR(${plasticStripsExtraCol}{excelRowNum}*{autosMonthColRef}{excelRowNum}*${refPartsAuto.celdaReferencia}{excelRowNum}/1000, \"--\")";
                                        break;
                                }
                                row[nombreCelda] = formula;
                            }
                            dt.Rows.Add(row);
                            excelRowNum++;
                        }

                        int startDataCol = columnaActual + extra;
                        int endDataCol = startDataCol + cabeceraAniosFY_conMeses.Count - 1;
                        int endRow = 5 + reporte.BG_Forecast_item.Count;

                        switch (indexTabla)
                        {
                            case 0:
                            case 5:
                            case 17:
                                oSLDocument.SetCellStyle(5, columnaActual, endRow, endDataCol, styleNumberDecimal_0);
                                break;
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                            case 12:
                            case 13:
                                oSLDocument.SetCellStyle(5, startDataCol, endRow, endDataCol, styleNumberDecimal_2);
                                break;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 10:
                            case 11:
                            case 14:
                            case 15:
                            case 16:
                            case 18:
                            case 19:
                            case 20:
                            case 21:
                            case 22:
                            case 23:
                            default: // Todos los demás son de tipo Moneda
                                oSLDocument.SetCellStyle(5, startDataCol, endRow, endDataCol, styleCurrency);
                                break;
                        }

                        oSLDocument.SetColumnWidth(startDataCol, endDataCol, 13);


                        // Define la fila de la sumatoria
                        int filaSumatoria = reporte.BG_Forecast_item.Count + 5;
                        int filaInicio = 5;
                        int filaFin = reporte.BG_Forecast_item.Count + 4;

                        // Recorre todos los años fiscales por cada categoría
                        for (int j = 0; j < cabeceraAniosFY_conMeses.Count; j++)
                        {
                            // Calcula la referencia de la columna actual
                            string celdaInicio = GetCellReference(columnaActual + j + extra) + filaInicio;
                            string celdaFin = GetCellReference(columnaActual + j + extra) + filaFin;
                            string formulaSumatoria = $"=SUM({celdaInicio}:{celdaFin})";

                            // Asigna la fórmula de sumatoria a la celda
                            oSLDocument.SetCellValue(filaSumatoria, columnaActual + j + extra, formulaSumatoria);
                        }

                        // Aplica el estilo a todo el rango de la sumatoria
                        oSLDocument.SetCellStyle(
                            filaSumatoria,
                            columnaActual + extra,
                            filaSumatoria,
                            columnaActual + extra + cabeceraAniosFY_conMeses.Count - 1,
                            styleTotales
                        );


                        ///....
                        ///Aplica estilo a la tabla
                        oSLDocument.ImportDataTable(4, columnaActual, dt, true);




                        //copia la formula al resto de las celdas
                        //copia Horizontalmente
                        //for (int j = 1; j < cabeceraAniosFY_conMeses.Count; j++)
                        //{
                        //    oSLDocument.CopyCell(5, columnaActual + extra, 5, columnaActual + extra + j);
                        //}

                        // Copia incrementalmente la fila completa verticalmente
                        //int totalFilas = reporte.BG_Forecast_item.Count; // Número total de filas a copiar
                        //int filaActual = 6; // Primera fila ancla
                        //int filasACopiar = 1; // Comienza copiando 1 fila

                        //while (filaActual < totalFilas + 5)
                        //{
                        //    // Obtiene las filas a copiar en esta iteración
                        //    int filaInicioCopiaFY = filaActual - filasACopiar; // Empieza desde donde hay datos
                        //    int filaFinCopiaFY = filaInicioCopiaFY + filasACopiar - 1; // Copia todas las filas previas

                        //    // Asegura que no copie más de lo necesario en la última iteración
                        //    if (filaActual + filasACopiar > totalFilas + 5)
                        //        filaFinCopiaFY = filaInicioCopiaFY + totalFilas + 5 - filaActual - 1;

                        //    // Copia todas las filas generadas hasta el momento
                        //    oSLDocument.CopyCell(filaInicioCopiaFY, columnaActual + extra, filaFinCopiaFY, columnaActual + extra + cabeceraAniosFY_conMeses.Count - 1, filaActual, columnaActual + extra);

                        //    // Incrementa la fila actual y ajusta filas a copiar para la siguiente iteración
                        //    filaActual += filasACopiar;
                        //    filasACopiar *= 2; // Duplica las filas a copiar en cada iteración
                        //}

                        //ajusta el tamaño de las filas
                        oSLDocument.SetRowHeight(1, dt.Rows.Count + 4, 15.0);
                        oSLDocument.SetRowHeight(4, 45.0);

                        //aplica formato condicional cuando el elemento está desactivado
                        SLConditionalFormatting cf;
                        cf = new SLConditionalFormatting(5, columnaActual, 5 + reporte.BG_Forecast_item.Count, columnaActual + cabeceraAniosFY_conMeses.Count - 1 + extra);
                        cf.HighlightCellsWithFormula("=$D5=\"D\"", SLHighlightCellsStyleValues.LightRedFill);
                        oSLDocument.AddConditionalFormatting(cf);

                        //aplica el estilo a cada hoja de FY
                        oSLDocument.SetCellStyle(4, columnaActual, 4, columnaActual + cabeceraAniosFY_conMeses.Count - 1 + extra, styleCenterCenter);
                        oSLDocument.SetCellStyle(4, columnaActual, 4, columnaActual + cabeceraAniosFY_conMeses.Count - 1 + extra, styleHeader);
                        oSLDocument.SetCellStyle(4, columnaActual, 4, columnaActual + cabeceraAniosFY_conMeses.Count - 1 + extra, styleHeaderFont);
                        oSLDocument.Filter(4, 1, 4, columnaActual + cabeceraAniosFY_conMeses.Count - 1 + extra);
                        oSLDocument.SetColumnWidth(columnaActual, columnaActual + cabeceraAniosFY_conMeses.Count - 1 + extra, 14);

                        //realiza el merge para los titulos de cada tabla
                        oSLDocument.MergeWorksheetCells(3, columnaActual, 3, columnaActual + cabeceraAniosFY_conMeses.Count - 1 + extra);
                        oSLDocument.SetCellValue(3, columnaActual, t.celdaDescripcion);
                        oSLDocument.SetCellStyle(3, columnaActual, styleHeaderFont);
                        oSLDocument.SetCellStyle(3, columnaActual, styleCenterTop);
                        oSLDocument.SetCellStyle(3, columnaActual, styleHeader);


                        // aumenta el valor de columnaActual para la siguiente iteración
                        columnaActual += cabeceraAniosFY_conMeses.Count + 1 + extra;

                    }

                    //actualiza valores generales de la hoja de resumen
                    int c2 = 0;
                    int filaInicial = 5;

                    hubContext.Clients.All.recibirProgresoExcel(83, 83, 100, $"Actualizando los valores de Hoja Resumen");


                    foreach (var forecast_Item in reporte.BG_Forecast_item)
                    {
                        //Engennered Scrap
                        if (numInicioColumnaEngScrap > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaEngScrap, forecast_Item.own_cm + forecast_Item.plant + forecast_Item.material_short);

                        //Freights Income USD/PART
                        if (numInicioColumnaFreightsIncomeUSD > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaFreightsIncomeUSD, forecast_Item.freights_income_usd_part.HasValue ? forecast_Item.freights_income_usd_part.Value : 0.0);
                        //Freights Maniobras USD/PART
                        if (numInicioColumnaManiobrasPart > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaManiobrasPart, forecast_Item.maniobras_usd_part.HasValue ? forecast_Item.maniobras_usd_part.Value : 0.0);
                        //Customs Expenses USD/PART
                        if (numInicioColumnaCustomExpenses > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaCustomExpenses, forecast_Item.customs_expenses.HasValue ? forecast_Item.customs_expenses.Value : 0.0);
                        //Wooden pallets												
                        if (numInicioColumnaWoodenPallets > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaWoodenPallets, forecast_Item.wooden_pallet_usd_part.HasValue ? forecast_Item.wooden_pallet_usd_part.Value : 0.0);
                        //Standar Packaging
                        if (numInicioColumnaStandardPackaging > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaStandardPackaging, forecast_Item.packaging_price_usd_part.HasValue ? forecast_Item.packaging_price_usd_part.Value : 0.0);
                        //Plastic Strips
                        if (numInicioColumnaPlasticStrips > 0)
                            oSLDocument.SetCellValue(filaInicial + c2, numInicioColumnaPlasticStrips, forecast_Item.neopreno_usd_part.HasValue ? forecast_Item.neopreno_usd_part.Value : 0.0);


                        c2++;
                    }

                    //agrega la formula para gross part
                    string grossPartFormula = string.Format("=IFERROR((SUM({0}{1}:{2}{1})*1000 + SUM({3}{1}:{4}{1})*0 + SUM({5}{1}:{6}{1})*0 - (SUM({7}{1}:{8}{1}) + SUM({9}{1}:{10}{1}))*1000) / ({11}{1} * SUM({12}{1}:{13}{1})), \"--\")",
                          GetCellReference(numInicioColumnaTotalSales), filaInicial,
                          GetCellReference(numInicioColumnaTotalSales + cabeceraAniosFY_conMeses.Count - 1),
                          GetCellReference(numInicioColumnaEngScrap + 1), GetCellReference(numInicioColumnaEngScrap + cabeceraAniosFY_conMeses.Count),
                          GetCellReference(numInicioColumnaScrapConsolidation), GetCellReference(numInicioColumnaScrapConsolidation + cabeceraAniosFY_conMeses.Count - 1),
                          GetCellReference(numInicioColumnaAditionalMaterialCost), GetCellReference(numInicioColumnaAditionalMaterialCost + cabeceraAniosFY_conMeses.Count - 1),
                          GetCellReference(numInicioColumnaOutgoingFreightTotal), GetCellReference(numInicioColumnaOutgoingFreightTotal + cabeceraAniosFY_conMeses.Count - 1),
                          refStrokesAuto.celdaReferencia, GetCellReference(numInicioColumnaDatosBase), GetCellReference(numInicioColumnaDatosBase + cabeceraAniosFY_conMeses.Count - 1));

                    oSLDocument.SetCellValue(filaInicial, refGrossPart.columnaNum, grossPartFormula);

                    for (int k = 1; k < reporte.BG_Forecast_item.Count; k++)
                    {
                        //copia las sumatorias de gross
                        oSLDocument.CopyCell(5, refGrossPart.columnaNum, 5 + k, refGrossPart.columnaNum);
                    }

                    #region valores clientes

                    hubContext.Clients.All.recibirProgresoExcel(86, 86, 100, $"Actualizando valores de clientes");



                    for (int indexCliente = 0; indexCliente < listClientes.Count; indexCliente++)
                    {

                        hubContext.Clients.All.recibirProgresoExcel(90, 90, 100, $"Actualizando valores de cliente: {listClientes[indexCliente].descripcion}");

                        var cliente = listClientes[indexCliente];
                        int currentRow = filaCliente + indexCliente;
                        oSLDocument.SetCellValue(currentRow, numInicioColumnaTotalSales - 1, cliente.descripcion);
                        oSLDocument.SetCellStyle(currentRow, numInicioColumnaTotalSales - 1, styleHighlightGray);

                        foreach (var (anioFY, j) in cabeceraAniosFY_conMeses.Select((anio, idx) => (anio, idx)))
                        {
                            string hojaRef = $"{anioFY.text} by Month";
                            var columnas = new[]
                            {
                                (numInicioColumnaTotalSales, numInicioColumnaTotalSalesMonth, styleCurrencyLinea),
                                (numInicioColumnaMaterialCost, numInicioColumnaMaterialCostMonth, styleCurrencyLinea),
                                (numInicioColumnaCostOfOutsiteP, numInicioColumnaCostOfOutsitePMonth, styleCurrencyLinea),
                                (numInicioColumnaValueAddSales, numInicioColumnaValueAddSalesMonth, styleCurrencyLinea),
                                (numInicioColumnaProccessTon, numInicioColumnaProccessTonMonth, styleNumericLine),
                                (numInicioColumnaEngScrap + 1, numInicioColumnaEngScrapMonth, styleNumericLine),
                                (numInicioColumnaScrapConsolidation, numInicioColumnaScrapConsolidationMonth, styleNumericLine),
                                (numInicioColumnaStrokes, numInicioColumnaStrokesMonth, styleNumericLine),
                                (numInicioColumnaBlanks, numInicioColumnaBlanksMonth, styleNumericLine),
                                (numInicioColumnaAditionalMaterialCost, numInicioColumnaAditionalMaterialCostMonth, styleCurrencyLinea),
                                (numInicioColumnaOutgoingFreightTotal, numInicioColumnaOutgoingFreightTotalMonth, styleCurrencyLinea),
                                (numInicioColumnaInventoryOWNAverage, numInicioColumnaInventoryOWNAverageMonth, styleNumericLine),
                                (numInicioColumnaInventoryEndMonth, numInicioColumnaInventoryEndMonthMonth, styleCurrencyLinea),
                                (numInicioColumnaFreightsIncomeUSD + 1, numInicioColumnaFreightsIncomeUSDMonth, styleCurrencyLinea),
                                (numInicioColumnaManiobrasPart + 1, numInicioColumnaManiobrasPartMonth, styleCurrencyLinea),
                                (numInicioColumnaCustomExpenses + 1, numInicioColumnaCustomExpensesMonth, styleCurrencyLinea),
                                (numInicioColumnaShipmentTons, numInicioColumnaShipmentTonsMonth, styleNumericLine),
                                (numInicioColumnaSalesIncScrap, numInicioColumnaSalesIncScrapMonth, styleCurrencyLinea),
                                (numInicioColumnaVasIncScrap, numInicioColumnaVasIncScrapMonth, styleCurrencyLinea),
                                (numInicioColumnaProcessingIncScrap, numInicioColumnaProcessingIncScrapMonth, styleCurrencyLinea),
                                (numInicioColumnaWoodenPallets + 1, numInicioColumnaWoodenPalletsMonth, styleCurrencyLinea),
                                (numInicioColumnaStandardPackaging + 1, numInicioColumnaStandardPackagingMonth, styleCurrencyLinea),
                                (numInicioColumnaPlasticStrips + 1, numInicioColumnaPlasticStripsMonth, styleCurrencyLinea)
                            };

                            foreach (var (columnaInicio, columnaMesInicio, estilo) in columnas)
                            {
                                oSLDocument.SetCellValue(currentRow, columnaInicio + j, $"=SUM('{hojaRef}'!{GetCellReference(columnaMesInicio)}{currentRow}:{GetCellReference(columnaMesInicio + 11)}{currentRow})");
                                oSLDocument.SetCellStyle(currentRow, columnaInicio + j, estilo);
                            }
                        }
                    }

                    #endregion

                    #region Scrap por planta

                    hubContext.Clients.All.recibirProgresoExcel(94, 94, 100, $"Agregando Scrap a Hoja Resumen");


                    void SetScrapValuesAndFormulas(int fila, string scrapType)
                    {
                        oSLDocument.SetCellValue(fila, numInicioColumnaTotalSales - 1, $"{scrapType} SCRAP (Valor venta)");
                        oSLDocument.SetCellStyle(fila, numInicioColumnaTotalSales - 1, styleHighlight);

                        for (int j = 0; j < cabeceraAniosFY_conMeses.Count; j++)
                        {
                            var currentMonthText = cabeceraAniosFY_conMeses[j].text;

                            // Total Sales
                            string totalSalesRange = $"{GetCellReference(numInicioColumnaTotalSalesMonth)}{fila}:{GetCellReference(numInicioColumnaTotalSalesMonth + 11)}{fila}";
                            oSLDocument.SetCellStyle(fila, numInicioColumnaTotalSales + j, styleHighlight);
                            oSLDocument.SetCellValue(fila, numInicioColumnaTotalSales + j,
                                $"=SUM('{currentMonthText} by Month'!{totalSalesRange})");

                            // Material Cost
                            string materialCostRange = $"{GetCellReference(numInicioColumnaMaterialCostMonth)}{fila}:{GetCellReference(numInicioColumnaMaterialCostMonth + 11)}{fila}";
                            oSLDocument.SetCellStyle(fila, numInicioColumnaMaterialCost + j, styleHighlight);
                            oSLDocument.SetCellValue(fila, numInicioColumnaMaterialCost + j,
                                $"=SUM('{currentMonthText} by Month'!{materialCostRange})");
                        }
                    }

                    // Optimized code calling the function for both Steel and Aluminum
                    SetScrapValuesAndFormulas(filaScrapSteel, "STEEL");
                    SetScrapValuesAndFormulas(filaScrapAlu, "ALU");
                    oSLDocument.AutoFitColumn(numInicioColumnaTotalSales - 1);

                    //lista las combinaciones 
                    int indexC = 0;
                    foreach (var combinacion in listaCombinacionesScrap.Where(x => x.Contains("STEEL")))
                    {
                        //formulas para acero                     

                        indexC++;
                        int currentRow = filaScrapSteel + indexC;
                        oSLDocument.SetCellValue(currentRow, numInicioColumnaTotalSales - 1, combinacion);

                        //agrega las formulas para cada combinacion
                        for (int j = 0; j < cabeceraAniosFY_conMeses.Count; j++)
                        {
                            var currentMonthText = cabeceraAniosFY_conMeses[j].text;
                            string totalSalesRange = $"{GetCellReference(numInicioColumnaTotalSalesMonth)}{currentRow}:{GetCellReference(numInicioColumnaTotalSalesMonth + 11)}{currentRow}";
                            string materialCostRange = $"{GetCellReference(numInicioColumnaMaterialCostMonth)}{currentRow}:{GetCellReference(numInicioColumnaMaterialCostMonth + 11)}{currentRow}";

                            // Total Sales
                            oSLDocument.SetCellValue(currentRow, numInicioColumnaTotalSales + j,
                                $"=SUM('{currentMonthText} by Month'!{totalSalesRange})");

                            // Material Cost
                            oSLDocument.SetCellValue(currentRow, numInicioColumnaMaterialCost + j,
                                $"=SUM('{currentMonthText} by Month'!{materialCostRange})");
                        }
                    }
                    //formulas para aluminio

                    indexC = 0;
                    foreach (var combinacion in listaCombinacionesScrap.Where(x => x.Contains("ALU")))
                    {
                        int currentRow = filaScrapAlu + (++indexC);
                        oSLDocument.SetCellValue(currentRow, numInicioColumnaTotalSales - 1, combinacion);

                        foreach (var (currentMonthText, j) in cabeceraAniosFY_conMeses.Select((value, index) => (value.text, index)))
                        {
                            string totalSalesRange = $"{GetCellReference(numInicioColumnaTotalSalesMonth)}{currentRow}:{GetCellReference(numInicioColumnaTotalSalesMonth + 11)}{currentRow}";
                            string materialCostRange = $"{GetCellReference(numInicioColumnaMaterialCostMonth)}{currentRow}:{GetCellReference(numInicioColumnaMaterialCostMonth + 11)}{currentRow}";

                            // Total Sales
                            oSLDocument.SetCellValue(currentRow, numInicioColumnaTotalSales + j,
                                $"=SUM('{currentMonthText} by Month'!{totalSalesRange})"
                            );

                            // Material Cost
                            oSLDocument.SetCellValue(currentRow, numInicioColumnaMaterialCost + j,
                                $"=SUM('{currentMonthText} by Month'!{materialCostRange})"
                            );
                        }
                    }



                    #endregion
                }

                #endregion               

            }

            #endregion

            System.Diagnostics.Debug.WriteLine($"[TIMER] MonthbyMonth: {timer.Elapsed.TotalSeconds:F2} segundos");
            timer.Restart();

            hubContext.Clients.All.recibirProgresoExcel(96, 96, 100, $"Resumen Excel procesado, creando archivo para descarga.");


            ///
            //vuelve a selecciona la hoja 1
            oSLDocument.SelectWorksheet(hoja1);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            //timeMeasure.Stop();

            return (array);
        }

        /// <summary>
        /// Genera el reporte del IHS
        /// </summary>
        /// <param name="listado"></param>
        /// <returns></returns>
        public static byte[] GeneraPlantillaDemandaIHS(List<BG_IHS_item> listado, List<BG_IHS_combinacion> combinaciones, List<BG_IHS_division> divisiones, string demanda, List<BG_IHS_rel_demanda> demandaGlobal)
        {

            //var cabeceraMeses = Portal_2_0.Models.BG_IHS_UTIL.GetCabeceraPlantillaDemanda(listado);
            var cabeceraMeses = Portal_2_0.Models.BG_IHS_UTIL.GetCabecera();

            // Agrupa la demanda que recibiste como parámetro (operación en memoria)
            var demandaGlobalLookup = demandaGlobal.ToLookup(d => d.id_ihs_item);


            string hoja1 = "Demanda Cliente";

            //para regiones
            List<BG_IHS_item_anios> listDatosRegionesFY = new List<BG_IHS_item_anios>();

            SLDocument oSLDocument = new SLDocument();
            oSLDocument.AddWorksheet("Sheet1");
            oSLDocument.SelectWorksheet("Sheet1");

            System.Data.DataTable dt = new System.Data.DataTable();

            //estilos para celdas
            SLStyle styleIHS = oSLDocument.CreateStyle();
            styleIHS.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#faebd7"), System.Drawing.ColorTranslator.FromHtml("#faebd7"));

            SLStyle styleUser = oSLDocument.CreateStyle();
            styleUser.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#d6c6ea"), System.Drawing.ColorTranslator.FromHtml("#d6c6ea"));

            SLStyle styleDemandaOriginal = oSLDocument.CreateStyle();
            styleDemandaOriginal.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#faff6b"), System.Drawing.ColorTranslator.FromHtml("#faff6b"));

            SLStyle styleDemandaCustomer = oSLDocument.CreateStyle();
            styleDemandaCustomer.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#6afcf3"), System.Drawing.ColorTranslator.FromHtml("#6afcf3"));

            SLStyle styleValorCalculado = oSLDocument.CreateStyle();
            styleValorCalculado.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#bbf3c1"), System.Drawing.ColorTranslator.FromHtml("#bbf3c1"));

            SLStyle styleValorIHS = oSLDocument.CreateStyle();
            styleValorIHS.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#ffb6c1"), System.Drawing.ColorTranslator.FromHtml("#ffb6c1"));

            SLStyle styleTituloCombinacion = oSLDocument.CreateStyle();
            styleTituloCombinacion.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#FFCC99"), System.Drawing.ColorTranslator.FromHtml("#FFCC99"));
            styleTituloCombinacion.Font.FontColor = System.Drawing.Color.DarkBlue;
            styleTituloCombinacion.Font.Bold = true;

            SLStyle styleBoldBlue = oSLDocument.CreateStyle();
            styleBoldBlue.Font.FontColor = System.Drawing.Color.DarkBlue;
            styleBoldBlue.Font.Bold = true;

            SLStyle styleDivisionesData = oSLDocument.CreateStyle();
            styleDivisionesData.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#D5E8DB"), System.Drawing.ColorTranslator.FromHtml("#D5E8DB"));



            //columnas          
            #region cabecera general
            dt.Columns.Add("Id", typeof(string));                      //1
            dt.Columns.Add("Origen", typeof(string));                   //1
            dt.Columns.Add("Vehicle (IHS)", typeof(string));                   //1
            dt.Columns.Add("Vehicle (Compuesto)", typeof(string));                   //1
            dt.Columns.Add("Core Nameplate Region Mnemonic", typeof(string));                   //1
            dt.Columns.Add("Core Nameplate Plant Mnemonic", typeof(string));                   //1
            dt.Columns.Add("Mnemonic-Vehicle", typeof(string));                   //1
            dt.Columns.Add("Mnemonic-Vehicle/Plant", typeof(string));                   //1
            dt.Columns.Add("Mnemonic-Platform", typeof(string));                   //1
            dt.Columns.Add("Mnemonic-Plant", typeof(string));                   //1
                                                                                //dt.Columns.Add("Region", typeof(string));                   //1
                                                                                //dt.Columns.Add("Market", typeof(string));                   //1
                                                                                //dt.Columns.Add("Country/Territory", typeof(string));                   //1
                                                                                //dt.Columns.Add("Production Plant", typeof(string));                   //1
                                                                                //dt.Columns.Add("Region(Plant)", typeof(string));                   //1
                                                                                //dt.Columns.Add("City", typeof(string));                   //1
                                                                                //dt.Columns.Add("Plant State/Province", typeof(string));                   //1
                                                                                //dt.Columns.Add("Source Plant", typeof(string));                   //1
                                                                                //dt.Columns.Add("Source Plant Country/Territory", typeof(string));                   //1
                                                                                //dt.Columns.Add("Source Plant Region", typeof(string));                   //1
                                                                                //dt.Columns.Add("Design Parent", typeof(string));                   //1
                                                                                //dt.Columns.Add("Engineering Group", typeof(string));                   //1
                                                                                //dt.Columns.Add("Manufacturer Group", typeof(string));                   //1
                                                                                //dt.Columns.Add("Manufacturer", typeof(string));                   //1
                                                                                //dt.Columns.Add("Sales Parent", typeof(string));                   //1
                                                                                //dt.Columns.Add("Production Brand", typeof(string));                   //1
                                                                                //dt.Columns.Add("Platform Design Owner", typeof(string));                   //1
                                                                                //dt.Columns.Add("Architecture", typeof(string));                   //1
                                                                                //dt.Columns.Add("Platform", typeof(string));                   //1
                                                                                //dt.Columns.Add("Program", typeof(string));                   //1
                                                                                //dt.Columns.Add("Production Nameplate", typeof(string));                   //1
            dt.Columns.Add("SOP (Start of Production)", typeof(DateTime));                   //1
            dt.Columns.Add("EOP (End of Production)", typeof(DateTime));                   //1
                                                                                           //dt.Columns.Add("Lifecycle (Time)", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Assembly Type", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Strategic Group", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Sales Group", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Global Nameplate", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Primary Design Center", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Primary Design Country/Territory", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Primary Design Region", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Secondary Design Center", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Secondary Design Country/Territory	", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Secondary Design Region", typeof(string));                   //1
                                                                                           //dt.Columns.Add("GVW Rating", typeof(string));                   //1
                                                                                           //dt.Columns.Add("GVW Class", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Car/Truck", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Production Type", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Global Production Segment", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Flat Rolled Steel Usage", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Regional Sales Segment", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Global Production Price Class", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Global Sales Segment", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Global Sales Sub-Segment", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Global Sales Price Class", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Short-Term Risk Rating", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Long-Term Risk Rating", typeof(string));                   //1
                                                                                           //dt.Columns.Add("Porcentaje scrap", typeof(decimal));                  //1

            int camposPrevios = dt.Columns.Count;
            #endregion

            //agrega cabecera meses
            foreach (var c in cabeceraMeses)
                dt.Columns.Add(c.text, typeof(int));                  //1


            // declara array multidimencional para guardar los estilos de cada celda
            int columnasStyles = camposPrevios + cabeceraMeses.Count;

            SLStyle[,] styleCells = new SLStyle[listado.Count, columnasStyles];

            //copia el estilo de una celda 
            SLStyle styleSN = oSLDocument.GetCellStyle("A2");




            ////registros , rows
            foreach (BG_IHS_item item in listado)
            {
                int indexColumna = 0;

                //crea row
                System.Data.DataRow row = dt.NewRow();

                #region datos general
                row["Id"] = item.id;
                row["Origen"] = item.origen;
                row["Vehicle (IHS)"] = item.vehicle;
                row["Vehicle (Compuesto)"] = item.ConcatCodigo;
                row["Core Nameplate Region Mnemonic"] = item.core_nameplate_region_mnemonic;
                row["Core Nameplate Plant Mnemonic"] = item.core_nameplate_plant_mnemonic;
                row["Mnemonic-Vehicle"] = item.mnemonic_vehicle;
                row["Mnemonic-Vehicle/Plant"] = item.mnemonic_vehicle_plant;
                row["Mnemonic-Platform"] = item.mnemonic_platform;
                row["Mnemonic-Plant"] = item.mnemonic_plant;
                //row["Region"] = item.region;
                //row["Market"] = item.market;
                //row["Country/Territory"] = item.country_territory;
                //row["Production Plant"] = item.production_plant;
                //row["Region(Plant)"] = item._Region != null ? item._Region.descripcion : "SIN DEFINIR";
                //row["City"] = item.city;
                //row["Plant State/Province"] = item.plant_state_province;
                //row["Source Plant"] = item.source_plant;
                //row["Source Plant Country/Territory"] = item.source_plant_country_territory;
                //row["Source Plant Region"] = item.source_plant_region;
                //row["Design Parent"] = item.design_parent;
                //row["Engineering Group"] = item.engineering_group;
                //row["Manufacturer Group"] = item.manufacturer_group;
                //row["Manufacturer"] = item.manufacturer;
                //row["Sales Parent"] = item.sales_parent;
                //row["Production Brand"] = item.production_brand;
                //row["Platform Design Owner"] = item.platform_design_owner;
                //row["Architecture"] = item.architecture;
                //row["Platform"] = item.platform;
                //row["Program"] = item.program;
                //row["Production Nameplate"] = item.production_nameplate;
                row["SOP (Start of Production)"] = item.sop_start_of_production;
                row["EOP (End of Production)"] = item.eop_end_of_production;
                //row["Lifecycle (Time)"] = item.lifecycle_time;
                //row["Assembly Type"] = item.assembly_type;
                //row["Strategic Group"] = item.strategic_group;
                //row["Sales Group"] = item.sales_group;
                //row["Global Nameplate"] = item.global_nameplate;
                //row["Primary Design Center"] = item.primary_design_center;
                //row["Primary Design Country/Territory"] = item.primary_design_country_territory;
                //row["Primary Design Region"] = item.primary_design_region;
                //row["Secondary Design Center"] = item.secondary_design_center;
                //row["Secondary Design Country/Territory	"] = item.secondary_design_country_territory;
                //row["Secondary Design Region"] = item.secondary_design_region;
                //row["GVW Rating"] = item.gvw_rating;
                //row["GVW Class"] = item.gvw_class;
                //row["Car/Truck"] = item.car_truck;
                //row["Production Type"] = item.production_type;
                //row["Global Production Segment"] = item.global_production_segment;
                //row["Flat Rolled Steel Usage"] = item.RelSegmento != null ? item.RelSegmento.flat_rolled_steel_usage : null;
                //row["Regional Sales Segment"] = item.regional_sales_segment;
                //row["Global Production Price Class"] = item.global_production_price_class;
                //row["Global Sales Segment"] = item.global_sales_segment;
                //row["Global Sales Sub-Segment"] = item.global_sales_sub_segment;
                //row["Global Sales Price Class"] = item.global_sales_price_class;
                //row["Short-Term Risk Rating"] = item.short_term_risk_rating;
                //row["Long-Term Risk Rating"] = item.long_term_risk_rating;
                //row["Porcentaje scrap"] = item.porcentaje_scrap;

                //agrega el tipo de index
                for (int i = 0; i < camposPrevios; i++)
                {

                    switch (item.origen)
                    {
                        case Bitacoras.Util.BG_IHS_Origen.IHS:
                            styleCells[listado.IndexOf(item), i] = styleIHS;
                            break;
                        case Bitacoras.Util.BG_IHS_Origen.USER:
                            styleCells[listado.IndexOf(item), i] = styleUser;
                            break;
                        default:
                            styleCells[listado.IndexOf(item), i] = oSLDocument.CreateStyle();
                            break;

                    }
                    indexColumna++;
                }

                #endregion

                #region meses
                int indexCabecera = 0;
                var demandaDelItem = demandaGlobalLookup[item.id];
                List<BG_IHS_rel_demanda> demandaMeses = item.GetDemanda(cabeceraMeses, demanda, demandaDelItem);

                foreach (var item_demanda in demandaMeses)
                {
                    //si no es nul agrega la cantidad
                    if (item_demanda != null)
                    {
                        row[cabeceraMeses[indexCabecera].text] = item_demanda.cantidad;

                        //agrega el estilo a la cabecera
                        switch (item_demanda.origen_datos)
                        {
                            case Bitacoras.Util.BG_IHS_tipo_demanda.CUSTOMER:
                                styleCells[listado.IndexOf(item), indexColumna] = styleDemandaCustomer;
                                break;
                            case Bitacoras.Util.BG_IHS_tipo_demanda.ORIGINAL:
                                styleCells[listado.IndexOf(item), indexColumna] = styleDemandaOriginal;
                                break;
                            default:
                                styleCells[listado.IndexOf(item), indexColumna] = oSLDocument.CreateStyle();
                                break;
                        }
                    }
                    else
                    {
                        row[cabeceraMeses[indexCabecera].text] = DBNull.Value;
                        styleCells[listado.IndexOf(item), indexColumna] = oSLDocument.CreateStyle();
                    }


                    indexColumna++;
                    indexCabecera++;
                }

                #endregion


                //agrega la filas
                dt.Rows.Add(row);

            }

            #region Hoja Autos Normal

            //crea la hoja de Inventory y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, hoja1);
            oSLDocument.SelectWorksheet(hoja1);
            oSLDocument.ImportDataTable(1, 1, dt, true);

            //aplica el color de las celdas
            for (int a = 0; a < listado.Count; a++)
            {
                for (int b = 0; b < columnasStyles; b++)
                {
                    oSLDocument.SetCellStyle(a + 2, b + 1, styleCells[a, b]);
                }
            }

            //estilo para ajustar al texto
            SLStyle styleWrap = oSLDocument.CreateStyle();
            styleWrap.SetWrapText(true);
            styleWrap.Alignment.Vertical = VerticalAlignmentValues.Top;

            //estilo para el encabezado
            SLStyle styleHeader = oSLDocument.CreateStyle();
            styleHeader.Font.Bold = true;
            styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

            //estilo para numeros
            SLStyle styleNumberDecimal = oSLDocument.CreateStyle();
            styleNumberDecimal.FormatCode = "#,##0.000";

            ////estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy-mm";
            oSLDocument.SetColumnStyle(11, 12, styleShortDate);

            //crea Style para porcentaje
            SLStyle stylePercent = oSLDocument.CreateStyle();
            stylePercent.FormatCode = "0.00%";
            //oSLDocument.SetColumnStyle(59, stylePercent);


            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //da estilo a los numeros
            //oSLDocument.SetColumnStyle(9, 10, styleNumberDecimal);

            //da estilo a la hoja de excel
            ////inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 3);

            oSLDocument.Filter(1, 1, 1, dt.Columns.Count + 1);
            oSLDocument.AutoFitColumn(1, dt.Columns.Count + 1);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count + 1, styleWrap);
            oSLDocument.SetCellStyle(1, 1, 1, dt.Columns.Count + 1, styleHeader);
            oSLDocument.SetCellStyle(1, 1, 1, dt.Columns.Count + 1, styleHeaderFont);

            oSLDocument.SetRowHeight(1, listado.Count + 1, 15.0);

            oSLDocument.HideColumn(1);

            #endregion

            ///
            //vuelve a selecciona la hoja 1
            oSLDocument.SelectWorksheet(hoja1);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }


        public static string GetCellReference(int col)
        {
            StringBuilder sb = new StringBuilder();

            do
            {
                col--;
                sb.Insert(0, (char)('A' + (col % 26)));
                col /= 26;
            } while (col > 0);
            return sb.ToString();
        }

        public static int ColumnToIndex(string col)
        {
            col = col.ToUpper();
            List<string> alphabet = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            int result = 0;

            if (col.Length == 1)
                result = alphabet.IndexOf(col) + 1;
            if (col.Length == 2)
                result = ((alphabet.IndexOf(col[0].ToString()) + 1) * alphabet.Count) + alphabet.IndexOf(col[1].ToString()) + 1;

            return result;

        }
    }

    public class ReferenciaColumna
    {
        public string celdaReferencia { get; set; }
        public int columnaNum { get; set; }
        public string celdaDescripcion { get; set; }
        public string extra { get; set; }
        public DateTime fecha { get; set; }
    }
}