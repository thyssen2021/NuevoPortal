using Bitacoras.Util;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace Portal_2_0.Models
{
    public class ExcelUtil
    {

        public static byte[] GeneraReporteBitacorasExcel(List<view_historico_resultado> listado, bool tipo_turno = false)
        {


            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");
            Portal_2_0Entities db = new Portal_2_0Entities();

            System.Data.DataTable dt = new System.Data.DataTable();

            //para llevar el control de si es encabezado o no
            List<bool> filasEncabezados = new List<bool>();
            filasEncabezados.Add(false); //es el encabezado principal

            //columnas
            dt.Columns.Add("Planta", typeof(string));
            dt.Columns.Add("Linea", typeof(string));
            dt.Columns.Add("Operador", typeof(string));
            dt.Columns.Add("Supervisor", typeof(string));
            dt.Columns.Add("SAP Platina", typeof(string));
            dt.Columns.Add("Tipo de Material", typeof(string));
            dt.Columns.Add("Número Parte Cliente", typeof(string));
            dt.Columns.Add("SAP Rollo", typeof(string));
            dt.Columns.Add("Material", typeof(string));
            dt.Columns.Add("Fecha", typeof(DateTime));
            dt.Columns.Add("Turno", typeof(string));
            dt.Columns.Add("Hora", typeof(string));
            dt.Columns.Add("Orden SAP", typeof(string));
            dt.Columns.Add("Orden SAP 2", typeof(string));
            dt.Columns.Add("Pieza por Golpe", typeof(int));
            dt.Columns.Add("Número de Rollo", typeof(string));
            dt.Columns.Add("Lote de Rollo", typeof(string));
            dt.Columns.Add("Peso Etiqueta (kg)", typeof(string));
            dt.Columns.Add("Peso Regreso Rollo Real", typeof(string));
            dt.Columns.Add("Peso Rollo Usado", typeof(double));
            dt.Columns.Add("Peso Báscula", typeof(double));
            dt.Columns.Add("No. Lote Izquierdo", typeof(double));
            dt.Columns.Add("No. Lote Derecho", typeof(double));
            dt.Columns.Add("Piezas por Paquete", typeof(double));
            dt.Columns.Add("Total Piezas", typeof(double));
            dt.Columns.Add("Peso de Rollo Consumido", typeof(double));
            dt.Columns.Add("Número de Golpes", typeof(double));
            dt.Columns.Add("Kg Restante de Rollo", typeof(double));
            dt.Columns.Add("Peso Despunte Kgs", typeof(double));
            dt.Columns.Add("Peso Cola Kgs", typeof(double));
            dt.Columns.Add("% Punta y colas", typeof(double));
            dt.Columns.Add("Total Piezas Ajuste", typeof(double));
            dt.Columns.Add("Peso Bruto Kgs", typeof(double));
            dt.Columns.Add("Peso Real Pieza Bruto", typeof(double));
            dt.Columns.Add("Peso Real Pieza Neto", typeof(double));
            dt.Columns.Add("Scrap Natural", typeof(double));
            dt.Columns.Add("Peso Neto SAP", typeof(double));
            dt.Columns.Add("Peso Bruto SAP", typeof(double));
            dt.Columns.Add("Balance de Scrap", typeof(double));
            dt.Columns.Add("Órdenes por Pieza", typeof(double));
            dt.Columns.Add("Peso Rollo Usado Real", typeof(double));
            dt.Columns.Add("Peso Bruto Total Piezas Kgs", typeof(double));
            dt.Columns.Add("Peso Neto Total Piezas Kgs", typeof(double));
            dt.Columns.Add("Scrap de Ingeniería (buenas + ajuste) Total piezas", typeof(double));
            dt.Columns.Add("Peso Neto Total Piezas de Ajuste Kgs", typeof(double));
            dt.Columns.Add("Peso Punta y Colas Reales kgs", typeof(double));
            dt.Columns.Add("Balance de Scrap Real", typeof(double));

            //registros , rows
            foreach (view_historico_resultado item in listado)
            {
                dt.Rows.Add(item.Planta, item.Linea, item.Operador, item.Supervisor, item.SAP_Platina, item.Tipo_de_Material, item.Número_de_Parte__de_cliente, item.SAP_Rollo, item.Material, item.Fecha,
                    item.Turno, String.Format("{0:T}", item.Hora), item.Orden_SAP, item.Orden_en_SAP_2, item.Pieza_por_Golpe, item.N__de_Rollo, item.Lote_de_rollo, item.Peso_Etiqueta__Kg_, item.Peso_de_regreso_de_rollo_Real
                    , item.Peso_de_rollo_usado, item.Peso_Báscula_Kgs, null, null, null, item.Total_de_piezas, item.Peso_de_rollo_consumido, item.Numero_de_golpes, item.Kg_restante_de_rollo, item.Peso_despunte_kgs_,
                     item.Peso_cola_Kgs_, item.Porcentaje_de_puntas_y_colas, item.Total_de_piezas_de_Ajustes, item.Peso_Bruto_Kgs, item.Peso_Real_Pieza_Bruto, item.Peso_Real_Pieza_Neto, item.Scrap_Natural
                     , item.Peso_neto_SAP, item.Peso_Bruto_SAP, item.Balance_de_Scrap, item.Ordenes_por_pieza, item.Peso_de_rollo_usado_real__Kg, item.Peso_bruto_Total_piezas_Kg, item.Peso_NetoTotal_piezas_Kg
                     , item.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg, item.Peso_Neto_total_piezas_de_ajuste_Kgs, item.Peso_puntas_y_colas_reales_Kg, item.Balance_de_Scrap_Real);

                filasEncabezados.Add(true);

                produccion_registros p = null;
                //busca si tiene registro en el nuevo sistema
                if (item.IdRegistro.HasValue)
                    p = db.produccion_registros.Find(item.IdRegistro.Value);

                //obtiene la cantidad de fila actual
                int fila_inicial = filasEncabezados.Count + 1;

                //si tiene registro, agrega los lotes
                if (p != null)
                {
                    foreach (produccion_lotes lote in p.produccion_lotes)
                    {
                        dt.Rows.Add(null, null, null, null, null, null, null, null, null, null,
                      null, null, null, null, null, null, null, null, null
                      , null, null, lote.numero_lote_izquierdo, lote.numero_lote_derecho, lote.piezas_paquete.Value, null, null, null, null, null,
                       null, null, null, null, null, null, null
                       , null, null, null, null, null, null, null
                       , null, null, null, null);
                        filasEncabezados.Add(false);
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

            double sumaRolloUsado = listado.Sum(item => item.Peso_de_rollo_usado.HasValue ? item.Peso_de_rollo_usado.Value : 0);
            double sumaNumGolpes = listado.Sum(item => item.Numero_de_golpes.HasValue ? item.Numero_de_golpes.Value : 0);
            double promedioBalanceScrap = listado.Average(item => item.Balance_de_Scrap.HasValue ? item.Balance_de_Scrap.Value : 0);
            double promedioBalanceScrapReal = listado.Average(item => item.Balance_de_Scrap_Real.HasValue ? item.Balance_de_Scrap_Real.Value : 0);

            if (tipo_turno)
                //fila para sumatorias
                dt.Rows.Add(null, null, null, null, null, null, null, null, null, null,
                        null, null, null, null, null, null, null, null, null
                        , sumaRolloUsado, null, null, null, null, null, null, sumaNumGolpes, null, null,
                         null, null, null, null, null, null, null
                         , null, null, promedioBalanceScrap, null, null, null, null
                         , null, null, null, promedioBalanceScrapReal);


            //crea la hoja de FACTURAS y la selecciona
            oSLDocument.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Sábana Producción");
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
                oSLDocument.SetRowStyle(filasEncabezados.Count + 1, styleFooter);
            }

            //estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(10, styleShortDate);

            //estilo para porcentanjes
            SLStyle stylePercent = oSLDocument.CreateStyle();
            stylePercent.FormatCode = "0.00%";
            oSLDocument.SetColumnStyle(31, stylePercent);
            oSLDocument.SetColumnStyle(39, stylePercent);
            oSLDocument.SetColumnStyle(47, stylePercent);

            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //estilo para numeros
            SLStyle styleNumber = oSLDocument.CreateStyle();
            styleNumber.FormatCode = "#,##0.000";

            //da estilo a la hoja de excel

            //aplica formato a las filas de encabezado
            for (int i = 0; i < filasEncabezados.Count; i++)
            {
                if (filasEncabezados[i])
                {
                    oSLDocument.SetRowStyle(i + 1, styleHeaderRow);
                }
                else
                {
                    oSLDocument.SetCellStyle(i + 1, 22, i + 1, 24, styleLoteInfo);
                }
                //colapsa todas las filas
                oSLDocument.CollapseRows(i + 1);
            }

            //da estilo a los numero
            oSLDocument.SetColumnStyle(33, 38, styleNumber);
            oSLDocument.SetColumnStyle(42, 46, styleNumber);

            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter("A1", "AU1");
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);


            oSLDocument.CollapseRows(filasEncabezados.Count + 1);

            oSLDocument.SetRowHeight(1, filasEncabezados.Count + 1, 15.0);

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

                if (item.empleados4 != null && item.fecha_validacion.HasValue)
                {
                    row["Validó (área)"] = item.empleados4.ConcatNombre;
                    row["Fecha Validación"] = item.fecha_validacion;
                }
                else
                {
                    row["Validó (área)"] = DBNull.Value;
                    row["Fecha Validación"] = DBNull.Value;
                }

                if (item.empleados != null && item.fecha_autorizacion.HasValue)
                {
                    row["Autorizó (doble validación)"] = item.empleados.ConcatNombre;
                    row["Fecha Autorización"] = item.fecha_autorizacion;
                }
                else
                {
                    row["Autorizó (doble validación)"] = DBNull.Value;
                    row["Fecha Autorización"] = DBNull.Value;
                }


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

                if (item.empleados1 != null && item.fecha_registro.HasValue)
                {
                    row["Registró (contabilidad)"] = item.empleados1.ConcatNombre;
                    row["Fecha Registro"] = item.fecha_registro;
                }
                else
                {
                    row["Fecha Registro"] = DBNull.Value;
                    row["Registró (contabilidad)"] = DBNull.Value;
                }

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
        public static byte[] GeneraReporteBudgetPorCentroCosto(budget_centro_costo centro, List<view_valores_anio_fiscal> valoresListAnioAnterior, List<view_valores_anio_fiscal> valoresListAnioActual,
            List<view_valores_anio_fiscal> valoresListAnioProximo, budget_anio_fiscal anio_Fiscal_anterior, budget_anio_fiscal anio_Fiscal_actual, budget_anio_fiscal anio_Fiscal_proximo)
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
            oSLDocument.MergeWorksheetCells(1,2, 1, 4);

            oSLDocument.SetCellValue("B2", "Cost Center");
            oSLDocument.SetCellValue("B3", "Deparment");
            oSLDocument.SetCellValue("B4", "Responsable");
            oSLDocument.SetCellValue("C2", centro.num_centro_costo);
            oSLDocument.SetCellValue("C3", centro.Area.descripcion);
            oSLDocument.SetCellValue("C4", centro.empleados.ConcatNombre);

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
            string titulo_anterior_total = "Totals FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1)));

            dt.Columns.Add(titulo_anterior_octubre, typeof(decimal));
            dt.Columns.Add(titulo_anterior_noviembre, typeof(decimal));
            dt.Columns.Add(titulo_anterior_diciembre, typeof(decimal));
            dt.Columns.Add(titulo_anterior_enero, typeof(decimal));
            dt.Columns.Add(titulo_anterior_febrero, typeof(decimal));
            dt.Columns.Add(titulo_anterior_marzo, typeof(decimal));
            dt.Columns.Add(titulo_anterior_abril, typeof(decimal));
            dt.Columns.Add(titulo_anterior_mayo, typeof(decimal)); 
            dt.Columns.Add(titulo_anterior_junio, typeof(decimal));
            dt.Columns.Add(titulo_anterior_julio, typeof(decimal));
            dt.Columns.Add(titulo_anterior_agosto, typeof(decimal));
            dt.Columns.Add(titulo_anterior_septiembre, typeof(decimal));
            dt.Columns.Add(titulo_anterior_total, typeof(decimal));
            //dt.Columns.Add("Comentarios");

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
            string titulo_actual_total = "Totals FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual));

            dt.Columns.Add(titulo_actual_octubre, typeof(decimal));
            dt.Columns.Add(titulo_actual_noviembre, typeof(decimal));
            dt.Columns.Add(titulo_actual_diciembre, typeof(decimal));
            dt.Columns.Add(titulo_actual_enero, typeof(decimal));
            dt.Columns.Add(titulo_actual_febrero, typeof(decimal));
            dt.Columns.Add(titulo_actual_marzo, typeof(decimal));
            dt.Columns.Add(titulo_actual_abril, typeof(decimal));
            dt.Columns.Add(titulo_actual_mayo, typeof(decimal));
            dt.Columns.Add(titulo_actual_junio, typeof(decimal));
            dt.Columns.Add(titulo_actual_julio, typeof(decimal));
            dt.Columns.Add(titulo_actual_agosto, typeof(decimal));
            dt.Columns.Add(titulo_actual_septiembre, typeof(decimal));
            dt.Columns.Add(titulo_actual_total, typeof(decimal));
            //dt.Columns.Add("Comentarios");

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
            string titulo_proximo_total = "Totals FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1)));

            dt.Columns.Add(titulo_proximo_octubre, typeof(decimal));
            dt.Columns.Add(titulo_proximo_noviembre, typeof(decimal));
            dt.Columns.Add(titulo_proximo_diciembre, typeof(decimal));
            dt.Columns.Add(titulo_proximo_enero, typeof(decimal));
            dt.Columns.Add(titulo_proximo_febrero, typeof(decimal));
            dt.Columns.Add(titulo_proximo_marzo, typeof(decimal));
            dt.Columns.Add(titulo_proximo_abril, typeof(decimal));
            dt.Columns.Add(titulo_proximo_mayo, typeof(decimal));
            dt.Columns.Add(titulo_proximo_junio, typeof(decimal));
            dt.Columns.Add(titulo_proximo_julio, typeof(decimal));
            dt.Columns.Add(titulo_proximo_agosto, typeof(decimal));
            dt.Columns.Add(titulo_proximo_septiembre, typeof(decimal));
            dt.Columns.Add(titulo_proximo_total, typeof(decimal));
            //dt.Columns.Add("Comentarios");


            for (int i = 0; i < valoresListAnioAnterior.Count; i++)
            {
                System.Data.DataRow row = dt.NewRow();

                //Inserta los datos de la cienta

                row["Item"] = i+1; 
                row["Sap Account"] = valoresListAnioAnterior[i].sap_account;
                row["Name"] = valoresListAnioAnterior[i].name;
                row["Mapping Bridge"] = valoresListAnioAnterior[i].descripcion;

                //completa valores para el año anterior
                #region valores anio pasado
                if (valoresListAnioAnterior[i].Octubre.HasValue)
                    row[titulo_anterior_octubre] = valoresListAnioAnterior[i].Octubre;
                else
                    row[titulo_anterior_octubre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Noviembre.HasValue)
                    row[titulo_anterior_noviembre] = valoresListAnioAnterior[i].Noviembre;
                else
                    row[titulo_anterior_noviembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Diciembre.HasValue)
                    row[titulo_anterior_diciembre] = valoresListAnioAnterior[i].Diciembre;
                else
                    row[titulo_anterior_diciembre] = DBNull.Value;

                if (valoresListAnioAnterior[i].Enero.HasValue)
                    row[titulo_anterior_enero] = valoresListAnioAnterior[i].Enero;
                else
                    row[titulo_anterior_enero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Febrero.HasValue)
                    row[titulo_anterior_febrero] = valoresListAnioAnterior[i].Febrero;
                else
                    row[titulo_anterior_febrero] = DBNull.Value;

                if (valoresListAnioAnterior[i].Marzo.HasValue)
                    row[titulo_anterior_marzo] = valoresListAnioAnterior[i].Marzo;
                else
                    row[titulo_anterior_marzo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Abril.HasValue)
                    row[titulo_anterior_abril] = valoresListAnioAnterior[i].Abril;
                else
                    row[titulo_anterior_abril] = DBNull.Value;

                if (valoresListAnioAnterior[i].Mayo.HasValue)
                    row[titulo_anterior_mayo] = valoresListAnioAnterior[i].Mayo;
                else
                    row[titulo_anterior_mayo] = DBNull.Value;

                if (valoresListAnioAnterior[i].Junio.HasValue)
                    row[titulo_anterior_junio] = valoresListAnioAnterior[i].Junio;
                else
                    row[titulo_anterior_junio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Julio.HasValue)
                    row[titulo_anterior_julio] = valoresListAnioAnterior[i].Julio;
                else
                    row[titulo_anterior_julio] = DBNull.Value;

                if (valoresListAnioAnterior[i].Agosto.HasValue)
                    row[titulo_anterior_agosto] = valoresListAnioAnterior[i].Agosto;
                else
                    row[titulo_anterior_agosto] = DBNull.Value;

                if (valoresListAnioAnterior[i].Septiembre.HasValue)
                    row[titulo_anterior_septiembre] = valoresListAnioAnterior[i].Septiembre;
                else
                    row[titulo_anterior_septiembre] = DBNull.Value;
               
              row[titulo_anterior_total] = valoresListAnioAnterior[i].TotalMeses();
                #endregion

                //completa valores para el año actual
                #region valores anio actual
                if (valoresListAnioActual[i].Octubre.HasValue)
                    row[titulo_actual_octubre] = valoresListAnioActual[i].Octubre;
                else
                    row[titulo_actual_octubre] = DBNull.Value;

                if (valoresListAnioActual[i].Noviembre.HasValue)
                    row[titulo_actual_noviembre] = valoresListAnioActual[i].Noviembre;
                else
                    row[titulo_actual_noviembre] = DBNull.Value;

                if (valoresListAnioActual[i].Diciembre.HasValue)
                    row[titulo_actual_diciembre] = valoresListAnioActual[i].Diciembre;
                else
                    row[titulo_actual_diciembre] = DBNull.Value;

                if (valoresListAnioActual[i].Enero.HasValue)
                    row[titulo_actual_enero] = valoresListAnioActual[i].Enero;
                else
                    row[titulo_actual_enero] = DBNull.Value;

                if (valoresListAnioActual[i].Febrero.HasValue)
                    row[titulo_actual_febrero] = valoresListAnioActual[i].Febrero;
                else
                    row[titulo_actual_febrero] = DBNull.Value;

                if (valoresListAnioActual[i].Marzo.HasValue)
                    row[titulo_actual_marzo] = valoresListAnioActual[i].Marzo;
                else
                    row[titulo_actual_marzo] = DBNull.Value;

                if (valoresListAnioActual[i].Abril.HasValue)
                    row[titulo_actual_abril] = valoresListAnioActual[i].Abril;
                else
                    row[titulo_actual_abril] = DBNull.Value;

                if (valoresListAnioActual[i].Mayo.HasValue)
                    row[titulo_actual_mayo] = valoresListAnioActual[i].Mayo;
                else
                    row[titulo_actual_mayo] = DBNull.Value;

                if (valoresListAnioActual[i].Junio.HasValue)
                    row[titulo_actual_junio] = valoresListAnioActual[i].Junio;
                else
                    row[titulo_actual_junio] = DBNull.Value;

                if (valoresListAnioActual[i].Julio.HasValue)
                    row[titulo_actual_julio] = valoresListAnioActual[i].Julio;
                else
                    row[titulo_actual_julio] = DBNull.Value;

                if (valoresListAnioActual[i].Agosto.HasValue)
                    row[titulo_actual_agosto] = valoresListAnioActual[i].Agosto;
                else
                    row[titulo_actual_agosto] = DBNull.Value;

                if (valoresListAnioActual[i].Septiembre.HasValue)
                    row[titulo_actual_septiembre] = valoresListAnioActual[i].Septiembre;
                else
                    row[titulo_actual_septiembre] = DBNull.Value;

                row[titulo_actual_total] = valoresListAnioActual[i].TotalMeses();
                #endregion

                //completa valores para el año 
                #region valores anio poximo
                if (valoresListAnioProximo[i].Octubre.HasValue)
                    row[titulo_proximo_octubre] = valoresListAnioProximo[i].Octubre;
                else
                    row[titulo_proximo_octubre] = DBNull.Value;

                if (valoresListAnioProximo[i].Noviembre.HasValue)
                    row[titulo_proximo_noviembre] = valoresListAnioProximo[i].Noviembre;
                else
                    row[titulo_proximo_noviembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Diciembre.HasValue)
                    row[titulo_proximo_diciembre] = valoresListAnioProximo[i].Diciembre;
                else
                    row[titulo_proximo_diciembre] = DBNull.Value;

                if (valoresListAnioProximo[i].Enero.HasValue)
                    row[titulo_proximo_enero] = valoresListAnioProximo[i].Enero;
                else
                    row[titulo_proximo_enero] = DBNull.Value;

                if (valoresListAnioProximo[i].Febrero.HasValue)
                    row[titulo_proximo_febrero] = valoresListAnioProximo[i].Febrero;
                else
                    row[titulo_proximo_febrero] = DBNull.Value;

                if (valoresListAnioProximo[i].Marzo.HasValue)
                    row[titulo_proximo_marzo] = valoresListAnioProximo[i].Marzo;
                else
                    row[titulo_proximo_marzo] = DBNull.Value;

                if (valoresListAnioProximo[i].Abril.HasValue)
                    row[titulo_proximo_abril] = valoresListAnioProximo[i].Abril;
                else
                    row[titulo_proximo_abril] = DBNull.Value;

                if (valoresListAnioProximo[i].Mayo.HasValue)
                    row[titulo_proximo_mayo] = valoresListAnioProximo[i].Mayo;
                else
                    row[titulo_proximo_mayo] = DBNull.Value;

                if (valoresListAnioProximo[i].Junio.HasValue)
                    row[titulo_proximo_junio] = valoresListAnioProximo[i].Junio;
                else
                    row[titulo_proximo_junio] = DBNull.Value;

                if (valoresListAnioProximo[i].Julio.HasValue)
                    row[titulo_proximo_julio] = valoresListAnioProximo[i].Julio;
                else
                    row[titulo_proximo_julio] = DBNull.Value;

                if (valoresListAnioProximo[i].Agosto.HasValue)
                    row[titulo_proximo_agosto] = valoresListAnioProximo[i].Agosto;
                else
                    row[titulo_proximo_agosto] = DBNull.Value;

                if (valoresListAnioProximo[i].Septiembre.HasValue)
                    row[titulo_proximo_septiembre] = valoresListAnioProximo[i].Septiembre;
                else
                    row[titulo_proximo_septiembre] = DBNull.Value;

                row[titulo_proximo_total] = valoresListAnioProximo[i].TotalMeses();

                #endregion

                dt.Rows.Add(row);
            }

            //agregar los totales
            System.Data.DataRow rowTotales = dt.NewRow();
            rowTotales["Mapping Bridge"] = "Totales";
            rowTotales[titulo_anterior_octubre] = valoresListAnioAnterior.Sum(item => item.Octubre);
            rowTotales[titulo_anterior_noviembre] = valoresListAnioAnterior.Sum(item => item.Noviembre);
            rowTotales[titulo_anterior_diciembre] = valoresListAnioAnterior.Sum(item => item.Diciembre);
            rowTotales[titulo_anterior_enero] = valoresListAnioAnterior.Sum(item => item.Enero);
            rowTotales[titulo_anterior_febrero] = valoresListAnioAnterior.Sum(item => item.Febrero);
            rowTotales[titulo_anterior_marzo] = valoresListAnioAnterior.Sum(item => item.Marzo);
            rowTotales[titulo_anterior_abril] = valoresListAnioAnterior.Sum(item => item.Abril);
            rowTotales[titulo_anterior_mayo] = valoresListAnioAnterior.Sum(item => item.Mayo);
            rowTotales[titulo_anterior_junio] = valoresListAnioAnterior.Sum(item => item.Junio);
            rowTotales[titulo_anterior_julio] = valoresListAnioAnterior.Sum(item => item.Julio);
            rowTotales[titulo_anterior_agosto] = valoresListAnioAnterior.Sum(item => item.Agosto);
            rowTotales[titulo_anterior_septiembre] = valoresListAnioAnterior.Sum(item => item.Septiembre);
            rowTotales[titulo_anterior_total] = valoresListAnioAnterior.Sum(item => item.TotalMeses());

            rowTotales[titulo_proximo_octubre] = valoresListAnioProximo.Sum(item => item.Octubre);
            rowTotales[titulo_proximo_noviembre] = valoresListAnioProximo.Sum(item => item.Noviembre);
            rowTotales[titulo_proximo_diciembre] = valoresListAnioProximo.Sum(item => item.Diciembre);
            rowTotales[titulo_proximo_enero] = valoresListAnioProximo.Sum(item => item.Enero);
            rowTotales[titulo_proximo_febrero] = valoresListAnioProximo.Sum(item => item.Febrero);
            rowTotales[titulo_proximo_marzo] = valoresListAnioProximo.Sum(item => item.Marzo);
            rowTotales[titulo_proximo_abril] = valoresListAnioProximo.Sum(item => item.Abril);
            rowTotales[titulo_proximo_mayo] = valoresListAnioProximo.Sum(item => item.Mayo);
            rowTotales[titulo_proximo_junio] = valoresListAnioProximo.Sum(item => item.Junio);
            rowTotales[titulo_proximo_julio] = valoresListAnioProximo.Sum(item => item.Julio);
            rowTotales[titulo_proximo_agosto] = valoresListAnioProximo.Sum(item => item.Agosto);
            rowTotales[titulo_proximo_septiembre] = valoresListAnioProximo.Sum(item => item.Septiembre);
            rowTotales[titulo_proximo_total] = valoresListAnioProximo.Sum(item => item.TotalMeses());

            rowTotales[titulo_actual_octubre] = valoresListAnioActual.Sum(item => item.Octubre);
            rowTotales[titulo_actual_noviembre] = valoresListAnioActual.Sum(item => item.Noviembre);
            rowTotales[titulo_actual_diciembre] = valoresListAnioActual.Sum(item => item.Diciembre);
            rowTotales[titulo_actual_enero] = valoresListAnioActual.Sum(item => item.Enero);
            rowTotales[titulo_actual_febrero] = valoresListAnioActual.Sum(item => item.Febrero);
            rowTotales[titulo_actual_marzo] = valoresListAnioActual.Sum(item => item.Marzo);
            rowTotales[titulo_actual_abril] = valoresListAnioActual.Sum(item => item.Abril);
            rowTotales[titulo_actual_mayo] = valoresListAnioActual.Sum(item => item.Mayo);
            rowTotales[titulo_actual_junio] = valoresListAnioActual.Sum(item => item.Junio);
            rowTotales[titulo_actual_julio] = valoresListAnioActual.Sum(item => item.Julio);
            rowTotales[titulo_actual_agosto] = valoresListAnioActual.Sum(item => item.Agosto);
            rowTotales[titulo_actual_septiembre] = valoresListAnioActual.Sum(item => item.Septiembre);
            rowTotales[titulo_actual_total] = valoresListAnioActual.Sum(item => item.TotalMeses());

            dt.Rows.Add(rowTotales);


            //define y combina las celdas de los encabezados 
            oSLDocument.SetCellValue(4, 5, "FY " +(Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(-1))));
            oSLDocument.SetCellValue(5, 5, "ACTUAL");
            oSLDocument.MergeWorksheetCells(5, 5, 5, 16);
            oSLDocument.MergeWorksheetCells(4, 5, 4, 16);

            oSLDocument.SetCellValue(4, 18, "FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual)));
            oSLDocument.SetCellValue(5, 18, "ACTUAL/FORECAST");
            oSLDocument.MergeWorksheetCells(5, 18, 5, 29);
            oSLDocument.MergeWorksheetCells(4, 18, 4, 29);

            oSLDocument.SetCellValue(4, 31, "FY " + (Bitacoras.Util.BgPlantillaUtil.DescripcionAnio(fechaActual.AddYears(1))));
            oSLDocument.SetCellValue(5, 31, "BUDGET");
            oSLDocument.MergeWorksheetCells(5, 31, 5, 42);
            oSLDocument.MergeWorksheetCells(4, 31, 4, 42);

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
            SLStyle styleTotales= oSLDocument.CreateStyle();
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
            styleBoldTexto.Font.Bold=true;

            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;

            //estilo para numeros
            SLStyle styleNumber = oSLDocument.CreateStyle();
            styleNumber.FormatCode = "$  #,##0.00";

           
            //da estilo a los numero
            //camniar cuando se agregen los comentarios
            oSLDocument.SetColumnStyle(5, 43, styleNumber);

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
            oSLDocument.SetCellStyle(filaInicial, 17, styleTotalsColor);
            oSLDocument.SetCellStyle(filaInicial, 30, styleTotalsColor);
            oSLDocument.SetCellStyle(filaInicial, 42, styleTotalsColor);

            //aplica estilo a las cabeceras de tipo año
            oSLDocument.SetCellStyle(4, 5, styleHeader);
            oSLDocument.SetCellStyle(4, 5, styleHeaderFont);
            oSLDocument.SetCellStyle(4, 5, styleCentrarTexto);
            oSLDocument.SetCellStyle(5, 5, styleTotalsColor);
            oSLDocument.SetCellStyle(5, 5, styleCentrarTexto);

            oSLDocument.SetCellStyle(4, 18, styleHeader);
            oSLDocument.SetCellStyle(4, 18, styleHeaderFont);
            oSLDocument.SetCellStyle(4, 18, styleCentrarTexto);
            oSLDocument.SetCellStyle(5, 18, styleTotalsColor);
            oSLDocument.SetCellStyle(5, 18, styleCentrarTexto);

            oSLDocument.SetCellStyle(4, 31, styleHeader);
            oSLDocument.SetCellStyle(4, 31, styleHeaderFont);
            oSLDocument.SetCellStyle(4, 31, styleCentrarTexto);
            oSLDocument.SetCellStyle(5, 31, styleTotalsColor);
            oSLDocument.SetCellStyle(5, 31, styleCentrarTexto);

            //estilo para titulo thyssen
            oSLDocument.SetRowHeight(1, 40.0);
            oSLDocument.SetCellStyle("B1", styleThyssen);

            //estilo para totales
            oSLDocument.SetCellStyle(valoresListAnioAnterior.Count + filaInicial + 1, 4, valoresListAnioAnterior.Count + filaInicial + 1, 43, styleTotales);

            oSLDocument.SetRowHeight(2, valoresListAnioAnterior.Count + filaInicial+ 1, 15.0);

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

        /// <summary>
        /// Genera el reporte en Excel
        /// </summary>
        /// <param name="listadoPPM"></param>
        /// <returns></returns>
        public static byte[] GeneraReportePPMExcel(List<inspeccion_categoria_fallas> listadoFallas, List<PPM> listadoPPM, List<produccion_registros> listadoRegistros, List<view_historico_resultado> listadoHistorico)
        {
            Portal_2_0Entities db = new Portal_2_0Entities();

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"), "Sheet1");

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
                System.Data.DataRow row = dt.NewRow();
                row["fecha"] = item.fecha;
                row["turno"] = item.produccion_turnos.valor;
                row["supervisor"] = item.produccion_supervisores.empleados.ConcatNombre;
                row["operador"] = item.produccion_operadores.empleados.ConcatNombre;
                row["inspector"] = inspectorName;
                row["Número de parte"] = item.Class_asociado.Customer_part_number;
                row["Número SAP rollo"] = item.sap_rollo;
                row["Número SAP platina"] = item.sap_platina;
                row["Lote"] = loteRollo;
                row["Observaciones"] = comentariosInspeccion;
                row["Total Piezas"] = item.TotalPiezasProduccion();


                foreach (inspeccion_categoria_fallas falla_c in listadoFallas)
                    foreach (inspeccion_fallas falla in falla_c.inspeccion_fallas.OrderBy(x => x.id))
                    {
                        //verifica si tiene alguna pieza de descarte falla con ese id
                        inspeccion_pieza_descarte_produccion pza_descarte = item.inspeccion_pieza_descarte_produccion.FirstOrDefault(x => x.id_falla == falla.id);
                        if (pza_descarte != null)
                            row[falla.descripcion] = pza_descarte.cantidad;
                        else
                            row[falla.descripcion] = DBNull.Value;
                    }
                if (pesoNetoUnitario.HasValue)
                    row["Peso Neto Unitario"] = pesoNetoUnitario;
                else
                    row["Peso Neto Unitario"] = DBNull.Value;

                row["Subtotal Piezas Daño Interno"] = item.NumPiezasDescarteDanoInterno();
                row["Total de Kg NG internos"] = item.produccion_datos_entrada.TotalKgNGInterno();

                if (totalPiezasOk.HasValue)
                    row["Peso Total Piezas OK"] = totalPiezasOk;
                else
                    row["Peso Total Piezas OK"] = DBNull.Value;

                if (pesoPuntas.HasValue)
                    row["Peso Puntas"] = pesoPuntas;
                else
                    row["Peso Puntas"] = DBNull.Value;

                if (pesoColas.HasValue)
                    row["Peso Colas"] = pesoColas;
                else
                    row["Peso Colas"] = DBNull.Value;

                dt.Rows.Add(row);
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
            filasEncabezados.Add(false); //es el encabezado principal

            //columnas
            dt.Columns.Add("Planta", typeof(string));
            dt.Columns.Add("Linea", typeof(string));
            dt.Columns.Add("Operador", typeof(string));
            dt.Columns.Add("Supervisor", typeof(string));
            dt.Columns.Add("SAP Platina", typeof(string));
            dt.Columns.Add("Tipo de Material", typeof(string));
            dt.Columns.Add("Número Parte Cliente", typeof(string));
            dt.Columns.Add("SAP Rollo", typeof(string));
            dt.Columns.Add("Material", typeof(string));
            dt.Columns.Add("Fecha", typeof(DateTime));
            dt.Columns.Add("Turno", typeof(string));
            dt.Columns.Add("Hora", typeof(string));
            dt.Columns.Add("Orden SAP", typeof(string));
            dt.Columns.Add("Orden SAP 2", typeof(string));
            dt.Columns.Add("Pieza por Golpe", typeof(int));
            dt.Columns.Add("Número de Rollo", typeof(string));
            dt.Columns.Add("Lote de Rollo", typeof(string));
            dt.Columns.Add("Peso Etiqueta (kg)", typeof(string));
            dt.Columns.Add("Peso Regreso Rollo Real", typeof(string));
            dt.Columns.Add("Peso Rollo Usado", typeof(double));
            dt.Columns.Add("Peso Báscula", typeof(double));
            dt.Columns.Add("No. Lote Izquierdo", typeof(double));
            dt.Columns.Add("No. Lote Derecho", typeof(double));
            dt.Columns.Add("Piezas por Paquete", typeof(double));
            dt.Columns.Add("Total Piezas", typeof(double));
            dt.Columns.Add("Peso de Rollo Consumido", typeof(double));
            dt.Columns.Add("Número de Golpes", typeof(double));
            dt.Columns.Add("Kg Restante de Rollo", typeof(double));
            dt.Columns.Add("Peso Despunte Kgs", typeof(double));
            dt.Columns.Add("Peso Cola Kgs", typeof(double));
            dt.Columns.Add("% Punta y colas", typeof(double));
            dt.Columns.Add("Total Piezas Ajuste", typeof(double));
            dt.Columns.Add("Peso Bruto Kgs", typeof(double));
            dt.Columns.Add("Peso Real Pieza Bruto", typeof(double));
            dt.Columns.Add("Peso Real Pieza Neto", typeof(double));
            dt.Columns.Add("Scrap Natural", typeof(double));
            dt.Columns.Add("Peso Neto SAP", typeof(double));
            dt.Columns.Add("Peso Bruto SAP", typeof(double));
            dt.Columns.Add("Balance de Scrap", typeof(double));
            dt.Columns.Add("Órdenes por Pieza", typeof(double));
            dt.Columns.Add("Peso Rollo Usado Real", typeof(double));
            dt.Columns.Add("Peso Bruto Total Piezas Kgs", typeof(double));
            dt.Columns.Add("Peso Neto Total Piezas Kgs", typeof(double));
            dt.Columns.Add("Scrap de Ingeniería (buenas + ajuste) Total piezas", typeof(double));
            dt.Columns.Add("Peso Neto Total Piezas de Ajuste Kgs", typeof(double));
            dt.Columns.Add("Peso Punta y Colas Reales kgs", typeof(double));
            dt.Columns.Add("Balance de Scrap Real", typeof(double));



            //registros , rows
            foreach (view_historico_resultado item in listadoHistorico)
            {
                dt.Rows.Add(item.Planta, item.Linea, item.Operador, item.Supervisor, item.SAP_Platina, item.Tipo_de_Material, item.Número_de_Parte__de_cliente, item.SAP_Rollo, item.Material, item.Fecha,
                    item.Turno, String.Format("{0:T}", item.Hora), item.Orden_SAP, item.Orden_en_SAP_2, item.Pieza_por_Golpe, item.N__de_Rollo, item.Lote_de_rollo, item.Peso_Etiqueta__Kg_, item.Peso_de_regreso_de_rollo_Real
                    , item.Peso_de_rollo_usado, item.Peso_Báscula_Kgs, null, null, null, item.Total_de_piezas, item.Peso_de_rollo_consumido, item.Numero_de_golpes, item.Kg_restante_de_rollo, item.Peso_despunte_kgs_,
                     item.Peso_cola_Kgs_, item.Porcentaje_de_puntas_y_colas, item.Total_de_piezas_de_Ajustes, item.Peso_Bruto_Kgs, item.Peso_Real_Pieza_Bruto, item.Peso_Real_Pieza_Neto, item.Scrap_Natural
                     , item.Peso_neto_SAP, item.Peso_Bruto_SAP, item.Balance_de_Scrap, item.Ordenes_por_pieza, item.Peso_de_rollo_usado_real__Kg, item.Peso_bruto_Total_piezas_Kg, item.Peso_NetoTotal_piezas_Kg
                     , item.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg, item.Peso_Neto_total_piezas_de_ajuste_Kgs, item.Peso_puntas_y_colas_reales_Kg, item.Balance_de_Scrap_Real);

                filasEncabezados.Add(true);

                produccion_registros p = null;
                //busca si tiene registro en el nuevo sistema
                if (item.IdRegistro.HasValue)
                    p = db.produccion_registros.Find(item.IdRegistro.Value);

                //obtiene la cantidad de fila actual
                int fila_inicial = filasEncabezados.Count + 1;

                //si tiene registro, agrega los lotes
                if (p != null)
                {
                    foreach (produccion_lotes lote in p.produccion_lotes)
                    {
                        dt.Rows.Add(null, null, null, null, null, null, null, null, null, null,
                      null, null, null, null, null, null, null, null, null
                      , null, null, lote.numero_lote_izquierdo, lote.numero_lote_derecho, lote.piezas_paquete.Value, null, null, null, null, null,
                       null, null, null, null, null, null, null
                       , null, null, null, null, null, null, null
                       , null, null, null, null);
                        filasEncabezados.Add(false);
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
            double promedioBalanceScrap = listadoHistorico.Average(item => item.Balance_de_Scrap.HasValue ? item.Balance_de_Scrap.Value : 0);
            double promedioBalanceScrapReal = listadoHistorico.Average(item => item.Balance_de_Scrap_Real.HasValue ? item.Balance_de_Scrap_Real.Value : 0);
            double sumaPesoCola = listadoHistorico.Sum(item => item.Peso_cola_Kgs_.HasValue ? item.Peso_cola_Kgs_.Value : 0);
            double sumaPesoDespunte = listadoHistorico.Sum(item => item.Peso_despunte_kgs_.HasValue ? item.Peso_despunte_kgs_.Value : 0);

            //fila para sumatorias
            dt.Rows.Add(null, null, null, null, null, null, null, null, null, null,
                    null, null, null, null, null, null, null, null, null
                    , sumaRolloUsado, null, null, null, null, null, null, sumaNumGolpes, null, sumaPesoDespunte, sumaPesoCola,
                      null, null, null, null, null, null
                     , null, null, promedioBalanceScrap, null, null, null, null
                     , null, null, null, promedioBalanceScrapReal);

            oSLDocument.ImportDataTable(1, 1, dt, true);

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


            //estilo para sumatorias

            SLStyle styleFooter = oSLDocument.CreateStyle();
            styleFooter.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#c6efce"), System.Drawing.ColorTranslator.FromHtml("#c6efce"));
            styleFooter.Font.Bold = true;
            styleFooter.Font.FontColor = System.Drawing.ColorTranslator.FromHtml("#006100");
            oSLDocument.SetRowStyle(filasEncabezados.Count + 1, styleFooter);

            //estilo para fecha

            oSLDocument.SetColumnStyle(10, styleShortDate);

            //estilo para porcentanjes
            SLStyle stylePercent = oSLDocument.CreateStyle();
            stylePercent.FormatCode = "0.00%";
            oSLDocument.SetColumnStyle(31, stylePercent);
            oSLDocument.SetColumnStyle(39, stylePercent);
            oSLDocument.SetColumnStyle(47, stylePercent);

            //da estilo a la hoja de excel

            //aplica formato a las filas de encabezado
            for (int i = 0; i < filasEncabezados.Count; i++)
            {
                if (filasEncabezados[i])
                {
                    oSLDocument.SetRowStyle(i + 1, styleHeaderRow);
                }
                else
                {
                    oSLDocument.SetCellStyle(i + 1, 22, i + 1, 24, styleLoteInfo);
                }
                //colapsa todas las filas
                oSLDocument.CollapseRows(i + 1);
            }

            styleHeaderFont.Alignment.TextRotation = 0;

            //da estilo a los numero
            oSLDocument.SetColumnStyle(33, 38, styleNumber);
            oSLDocument.SetColumnStyle(42, 46, styleNumber);

            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter("A1", "AU1");
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);


            oSLDocument.CollapseRows(filasEncabezados.Count + 1);

            oSLDocument.SetRowHeight(1, filasEncabezados.Count + 1, 15.0);

            oSLDocument.SelectWorksheet(hojaReportePorDia);

            #endregion


            //genera el archivo xlsx
            System.IO.Stream stream = new System.IO.MemoryStream();

            oSLDocument.SaveAs(stream);

            byte[] array = Bitacoras.Util.StreamUtil.ToByteArray(stream);

            return (array);
        }
    }
}