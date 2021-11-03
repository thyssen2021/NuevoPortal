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

        public static byte[] GeneraReporteBitacorasExcel(List<view_historico_resultado> listado) {

            SLDocument oSLDocument = new SLDocument();

          
            System.Data.DataTable dt = new System.Data.DataTable();

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
                    , item.Peso_de_rollo_usado, item.Peso_Báscula_Kgs, item.Piezas_por_paquete, item.Total_de_piezas, item.Peso_de_rollo_consumido, item.Numero_de_golpes, item.Kg_restante_de_rollo, item.Peso_despunte_kgs_,
                     item.Peso_cola_Kgs_, item.Porcentaje_de_puntas_y_colas, item.Total_de_piezas_de_Ajustes,item.Peso_Bruto_Kgs, item.Peso_Real_Pieza_Bruto, item.Peso_Real_Pieza_Neto, item.Scrap_Natural
                     , item.Peso_neto_SAP, item.Peso_Bruto_SAP, item.Balance_de_Scrap, item.Ordenes_por_pieza, item.Peso_de_rollo_usado_real__Kg, item.Peso_bruto_Total_piezas_Kg, item.Peso_NetoTotal_piezas_Kg
                     , item.Scrap_de_ingeniería__buenas___Ajuste__Total_Piezas_Kg,item.Peso_Neto_total_piezas_de_ajuste_Kgs, item.Peso_puntas_y_colas_reales_Kg, item.Balance_de_Scrap_Real);
            }

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

            //estilo para fecha
            SLStyle styleShortDate = oSLDocument.CreateStyle();
            styleShortDate.FormatCode = "yyyy/MM/dd";
            oSLDocument.SetColumnStyle(10, styleShortDate);

            //estilo para fecha
            SLStyle stylePercent = oSLDocument.CreateStyle();
            stylePercent.FormatCode = "0.00%";
            oSLDocument.SetColumnStyle(29, stylePercent);
            oSLDocument.SetColumnStyle(37, stylePercent);
            oSLDocument.SetColumnStyle(45, stylePercent);

            SLStyle styleHeaderFont = oSLDocument.CreateStyle();
            styleHeaderFont.Font.FontName = "Calibri";
            styleHeaderFont.Font.FontSize = 11;
            styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
            styleHeaderFont.Font.Bold = true;


            //da estilo a la hoja de excel

            //inmoviliza el encabezado
            oSLDocument.FreezePanes(1, 0);

            oSLDocument.Filter("A1", "AS1");
            oSLDocument.AutoFitColumn(1, 45);

            oSLDocument.SetColumnStyle(1, 45, styleWrap);
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