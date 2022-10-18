using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Portal_2_0.Models.PDFHandlers
{
    public class HeaderEventHandlerPDF_IT_checklist_equipos : IEventHandler
    {
        Image img;
        PdfFont fontThyssen;
        IT_equipos_checklist check;
        string tituloDocumento;

        public HeaderEventHandlerPDF_IT_checklist_equipos(Image img, PdfFont font, IT_equipos_checklist check)
        {
            this.img = img;
            this.fontThyssen = font;
            this.check = check;
            this.tituloDocumento = "Checklist Nuevos Equipos de Computo";
        }

        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent doctEvent = (PdfDocumentEvent)@event;
            PdfDocument pdfDoc = doctEvent.GetDocument();
            PdfPage page = doctEvent.GetPage();

            Rectangle rootArea = new Rectangle(35, page.GetPageSize().GetTop() - 70, page.GetPageSize().GetRight() - 70, 50);

            Canvas canvas = new Canvas(doctEvent.GetPage(), rootArea);

            canvas.Add(GetTable(doctEvent))
                //.ShowTextAligned("Esto es el encabezado de página", 10, 0, TextAlignment.CENTER)
                //.ShowTextAligned("Esto es el pie de página", 10, 0, TextAlignment.CENTER)
                //.ShowTextAligned("Texto agregado", 612, 0, TextAlignment.RIGHT)
                .Close();

        }

        public Table GetTable(PdfDocumentEvent docEvent)
        {
            //porcentaje de ancho de columna
            float[] cellWidth = { 25f, 50f, 25f };
            Table tableEvent = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();

            var thyssenColor = new DeviceRgb(0, 159, 245);
            Style styleCell = new Style().SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
            Style styleText = new Style().SetFontSize(12f).SetFontColor(thyssenColor);


            //crea la primera celda
            Cell cell = new Cell()
                .Add(new Paragraph("thyssenkrupp Materials de México S.A. de C.V.")).SetFont(fontThyssen)
                .AddStyle(styleText).SetTextAlignment(TextAlignment.LEFT)
               .SetHorizontalAlignment(HorizontalAlignment.CENTER)
               .AddStyle(styleCell);

            tableEvent.AddCell(cell);

            cell = new Cell()
              // .Add(new Paragraph(check.IATF_revisiones.IATF_documentos.nombre_documento)).SetFont(fontThyssen)
               .Add(new Paragraph(tituloDocumento)).SetFont(fontThyssen)  //Aquí va el nombre del documento
               .AddStyle(styleText).SetTextAlignment(TextAlignment.CENTER)
               .SetHorizontalAlignment(HorizontalAlignment.CENTER)
               .AddStyle(styleCell);

            tableEvent.AddCell(cell);

            //crea la celda para la imagen
            cell = new Cell()
                .Add(img.SetAutoScale(true).SetHorizontalAlignment(HorizontalAlignment.RIGHT))
                .AddStyle(styleCell)
                ;

            //agrega la celda a la tabla
            tableEvent.AddCell(cell);


            return tableEvent;

        }
    }

}