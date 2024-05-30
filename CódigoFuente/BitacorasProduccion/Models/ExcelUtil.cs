using Bitacoras.Util;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using SpreadsheetLight.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            dt.Columns.Add(nameof(view_historico_resultado.Orden_en_SAP_2), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.SAP_Platina_2), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Tipo_de_Material_platina2), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Número_de_Parte_de_Cliente_platina2), typeof(string));
            dt.Columns.Add(nameof(view_historico_resultado.Material_platina2), typeof(string));
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

            var datosProduccionRegistrosDBLista = db.produccion_registros.Where(x=> listaIds.Contains(x.id)).ToList();


            //registros , rows
           // int index = 1;

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

                //encuentra el valor de produccion registro

                produccion_registros p = null;
                //busca si tiene registro en el nuevo sistema

                p = datosProduccionRegistrosDBLista.FirstOrDefault(x=>x.id == item.Column40);

                string posteado = p != null && p.produccion_datos_entrada != null && p.produccion_datos_entrada.posteado ? "SÍ" : "NO";
              
                dt.Rows.Add(item.Planta, item.Linea, item.Operador, item.Supervisor, item.Fecha, String.Format("{0:T}", item.Hora), item.Turno, item.Orden_SAP, item.SAP_Platina,
                    item.Tipo_de_Material, item.Número_de_Parte__de_cliente, item.Material, item.Orden_en_SAP_2, item.SAP_Platina_2, item.Tipo_de_Material_platina2, item.Número_de_Parte_de_Cliente_platina2,
                    item.Material_platina2, item.ConcatCliente, item.SAP_Rollo, item.N__de_Rollo, item.Lote_de_rollo, item.Peso_Etiqueta__Kg_, item.Peso_de_regreso_de_rollo_Real,
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

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");


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
            oSLDocument.FreezePanes(filaInicial-2, 4);

            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);

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

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

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

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_piezas_descarte.xlsx"), "Sheet1");

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

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");
            System.Data.DataTable dt = new System.Data.DataTable();

            List<int> disabledItems = new List<int>();

            //columnas          
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

            ////registros , rows
            foreach (var item in listado)
            {
                System.Data.DataRow row = dt.NewRow();

                row["8ID"] = item.C8ID;
                row["Numero Empleado"] = item.numeroEmpleado;

                row["Nombre"] = item.ConcatNombre;
                if (item.nueva_fecha_nacimiento.HasValue)
                    row["Fecha Nacimiento"] = item.nueva_fecha_nacimiento.Value;
                else
                    row["Fecha Nacimiento"] = DBNull.Value;
                row["Sexo"] = item.sexo != null ? (item.sexo.ToString() == "F" ? "M" : "H") : String.Empty;
                if (item.ingresoFecha.HasValue)
                    row["Fecha Ingreso"] = item.ingresoFecha.Value;
                else
                    row["Fecha Ingreso"] = DBNull.Value;
                row["Correo"] = item.correo != null ? item.correo : String.Empty;
                row["Planta"] = item.plantas != null ? item.plantas.descripcion : String.Empty;
                row["Departamento"] = item.Area != null ? item.Area.descripcion : String.Empty;
                row["Puesto"] = item.puesto1 != null ? item.puesto1.descripcion : String.Empty;
                row["Jefe Directo"] = item.empleados2 != null ? item.empleados2.ConcatNombre : String.Empty;

                row["Baja"] = item.activo.HasValue && item.activo.Value ? "NO" : "SI";


                dt.Rows.Add(row);

                if (item.activo.HasValue && !item.activo.Value)
                    disabledItems.Add(dt.Rows.Count + 1);
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
            oSLDocument.SetColumnStyle(tkbirthColumn, styleShortDate);
            oSLDocument.SetColumnStyle(fechaIngresoColumn, styleShortDate);


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
            dt.Columns.Add("Cantidad Teórica", typeof(double));             //15
            dt.Columns.Add("Total pzas MIN", typeof(double));             //15
            dt.Columns.Add("Total pzas MAX", typeof(double));             //15
            dt.Columns.Add("Diferencia SAP", typeof(double));             //15
            dt.Columns.Add("Validación", typeof(string));             //15
            dt.Columns.Add("Capturista", typeof(string));

            ////registros , rows
            foreach (CI_conteo_inventario item in listado)
            {
                bool multiple = listado.Count(x => item.num_tarima != null && x.num_tarima == item.num_tarima) > 1;
                double? total_pzas_min = null,  total_pzas_max = null;

                if (multiple )
                {
                    if (item.cantidad_teorica.HasValue && item.cantidad_teorica > 0)
                    {
                        total_pzas_min = listado.Where(x => x.num_tarima == item.num_tarima).Sum(x => x.total_piezas_min);
                        total_pzas_max = listado.Where(x => x.num_tarima == item.num_tarima).Sum(x => x.total_piezas_max);
                    }
                }
                else {
                    total_pzas_min = item.total_piezas_min;
                    total_pzas_max = item.total_piezas_max;
                }


                dt.Rows.Add(item.plant, item.storage_location, item.storage_bin, item.batch, item.material, item.base_unit_measure, item.ship_to_number, item.material_description, item.num_tarima == null ? string.Empty : "T" + item.num_tarima.Value.ToString("D04"),
                    multiple ? "Sí" : "No", item.pieces,
                    item.unrestricted, item.blocked, item.in_quality, item.gauge, item.gauge_min, item.gauge_max, item.altura, item.espesor, item.cantidad_teorica,
                    total_pzas_min, total_pzas_max,
                    item.diferencia_sap, item.validacion,
                    item.empleados!=null? item.empleados.ConcatNombre: string.Empty 
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

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");
            System.Data.DataTable dt = new System.Data.DataTable();

            List<int> disabledItems = new List<int>();

            //columnas          
            dt.Columns.Add("userName", typeof(string));
            dt.Columns.Add("tksir", typeof(string));
            dt.Columns.Add("tktitle", typeof(string));
            dt.Columns.Add("tknameprefix", typeof(string));
            dt.Columns.Add("lastName", typeof(string));
            dt.Columns.Add("firstName", typeof(string));
            dt.Columns.Add("tkbirth", typeof(DateTime));
            int tkbirthColumn = dt.Columns.Count;
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
            dt.Columns.Add("tkedatetrust", typeof(string));
            dt.Columns.Add("tkldateorg", typeof(string));
            dt.Columns.Add("tklreason", typeof(string));
            dt.Columns.Add("shares", typeof(string));
            dt.Columns.Add("supervisoryboardelection", typeof(string));
            dt.Columns.Add("tkbkz", typeof(string));
            dt.Columns.Add("tkinside", typeof(string));
            dt.Columns.Add("Baja", typeof(string));

            ////registros , rows
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
                row["tkfkz6"] = "801495";       //unico
                row["tkfkzext"] = string.Empty;     //unico
                row["tkuniqueid"] = "801495-01";    //unico
                row["tkpstatus"] = "40";    //unico
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
                //obtiene las lineas
                if (item.GetIT_Inventory_Cellular_LinesActivas().Count > 0)
                    row["tkmobile"] = item.GetIT_Inventory_Cellular_LinesActivas().First().GetPhoneNumberFormat;
                else
                    row["tkmobile"] = String.Empty;

                row["tkgodfather"] = item.empleados2 != null ? item.empleados2.C8ID : String.Empty;
                row["tkprefdelmethod"] = "O";
                row["tkedatetrust"] = string.Empty;
                row["tklreason"] = string.Empty;
                row["shares"] = "N";
                row["supervisoryboardelection"] = "N";
                row["tkbkz"] = string.Empty;
                row["tkinside"] = "N";
                row["Baja"] = item.activo.HasValue && item.activo.Value ? "NO" : "YES";

                dt.Rows.Add(row);

                if (item.activo.HasValue && !item.activo.Value)
                    disabledItems.Add(dt.Rows.Count + 1);
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
            styleShortDate.FormatCode = "dd.MM.yyyy";
            oSLDocument.SetColumnStyle(tkbirthColumn, styleShortDate);


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
            dt.Columns.Add("Comentarios Cancelación", typeof(string));

            ////registros , rows
            foreach (var item in listado)
            {
                System.Data.DataRow row = dt.NewRow();

                row["Folio"] = item.id;
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
    }
}