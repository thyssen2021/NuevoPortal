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
        
        public static byte[] GeneraReporteBitacorasExcel(List<view_historico_resultado> listado, bool tipo_turno=false) {
         

            SLDocument oSLDocument = new SLDocument(HttpContext.Current.Server.MapPath("~/Content/plantillas_excel/plantilla_reporte_produccion.xlsx"),"Sheet1");
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
                    , item.Peso_de_rollo_usado, item.Peso_Báscula_Kgs,null, null, null, item.Total_de_piezas, item.Peso_de_rollo_consumido, item.Numero_de_golpes, item.Kg_restante_de_rollo, item.Peso_despunte_kgs_,
                     item.Peso_cola_Kgs_, item.Porcentaje_de_puntas_y_colas, item.Total_de_piezas_de_Ajustes,item.Peso_Bruto_Kgs, item.Peso_Real_Pieza_Bruto, item.Peso_Real_Pieza_Neto, item.Scrap_Natural
                     , item.Peso_neto_SAP, item.Peso_Bruto_SAP, item.Balance_de_Scrap, item.Ordenes_por_pieza, item.Peso_de_rollo_usado_real__Kg, item.Peso_bruto_Total_piezas_Kg, item.Peso_NetoTotal_piezas_Kg
                     , item.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg,item.Peso_Neto_total_piezas_de_ajuste_Kgs, item.Peso_puntas_y_colas_reales_Kg, item.Balance_de_Scrap_Real);

                filasEncabezados.Add(true);

                produccion_registros p = null;
                //busca si tiene registro en el nuevo sistema
                if (item.IdRegistro.HasValue)
                    p = db.produccion_registros.Find(item.IdRegistro.Value);

                //obtiene la cantidad de fila actual
                int fila_inicial = filasEncabezados.Count + 1;

                //si tiene registro, agrega los lotes
                if (p != null) {
                    foreach (produccion_lotes lote in p.produccion_lotes) {
                        dt.Rows.Add(null, null, null, null, null, null, null, null, null, null,
                      null, null, null, null, null, null, null, null,null
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

            double sumaRolloUsado = listado.Sum(item => item.Peso_de_rollo_usado.HasValue ? item.Peso_de_rollo_usado.Value:0);
            double sumaNumGolpes = listado.Sum(item => item.Numero_de_golpes.HasValue ? item.Numero_de_golpes.Value : 0);
            double promedioBalanceScrap = listado.Average(item => item.Balance_de_Scrap.HasValue ? item.Balance_de_Scrap.Value : 0);
            double promedioBalanceScrapReal = listado.Average(item => item.Balance_de_Scrap_Real.HasValue ? item.Balance_de_Scrap_Real.Value : 0);

            if(tipo_turno)
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
            oSLDocument.SetColumnStyle(33,38, styleNumber);
            oSLDocument.SetColumnStyle(42,46, styleNumber);

            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter("A1", "AU1");
            oSLDocument.AutoFitColumn(1, dt.Columns.Count);

            oSLDocument.SetColumnStyle(1, dt.Columns.Count, styleWrap);
            oSLDocument.SetRowStyle(1, styleHeader);
            oSLDocument.SetRowStyle(1, styleHeaderFont);          

          
            oSLDocument.CollapseRows(filasEncabezados.Count+1);

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
                item.PFA_Reason.descripcion, item.PFA_Type_shipment.descripcion,  item.PFA_Responsible_cost.descripcion,
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
    }
}