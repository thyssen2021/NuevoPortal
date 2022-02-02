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
                foreach (PM_conceptos c in item.PM_conceptos) {

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
                if (item.produccion_datos_entrada != null) {
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
                if(pesoNetoUnitario.HasValue)
                    row["Peso Neto Unitario"] = pesoNetoUnitario;
                else
                    row["Peso Neto Unitario"] = DBNull.Value;

                row["Subtotal Piezas Daño Interno"] = item.NumPiezasDescarteDanoInterno();
                row["Total de Kg NG internos"] = item.produccion_datos_entrada.TotalKgNGInterno();

                if(totalPiezasOk.HasValue)
                    row["Peso Total Piezas OK"] = totalPiezasOk;
                else
                    row["Peso Total Piezas OK"] = DBNull.Value;

                if(pesoPuntas.HasValue)
                    row["Peso Puntas"] = pesoPuntas;
                else
                    row["Peso Puntas"] = DBNull.Value;

                if(pesoColas.HasValue)
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
              
                oSLDocument.MergeWorksheetCells(1, inicioColumna, 1,columna_falla-1);
                oSLDocument.SetCellStyle(1,inicioColumna,styleFondo);
             
            }

            //agrega el color de fondo a los ultimos resultados          
            SLStyle styleFondoGray = oSLDocument.CreateStyle();
            styleFondoGray.Font.Bold = true;
            styleFondoGray.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#bebebe"), System.Drawing.ColorTranslator.FromHtml("#bebebe"));
            oSLDocument.SetCellStyle(3, columna_falla, listadoRegistros.Count + 3, columna_falla+5, styleFondoGray);

            oSLDocument.ImportDataTable(3, 1, dt,true);

            //agrega el color de fondo para las filas
            for (int i = 4; i <= listadoRegistros.Count + 3; i++) {
                if (i % 2 == 0)
                    oSLDocument.SetCellStyle(i,1, i, columna_falla-1, styleFondoRenglon);
            }
                


            //da estilo a la hoja de excel
            //inmoviliza el encabezado
            oSLDocument.FreezePanes(3, 9);

            oSLDocument.Filter("A3", "BR3");

            styleHeaderFont.Alignment.TextRotation = 90;

            oSLDocument.SetCellStyle(3,1,1,11, styleHeader);
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
            oSLDocument.SetColumnStyle(1,9, styleVertical);

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
            double sumaPesoDespunte= listadoHistorico.Sum(item => item.Peso_despunte_kgs_.HasValue ? item.Peso_despunte_kgs_.Value : 0);

            //fila para sumatorias
            dt.Rows.Add(null, null, null, null, null, null, null, null, null, null,
                    null, null, null, null, null, null, null, null, null
                    , sumaRolloUsado, null, null, null, null, null, null, sumaNumGolpes, null,sumaPesoDespunte, sumaPesoCola,
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