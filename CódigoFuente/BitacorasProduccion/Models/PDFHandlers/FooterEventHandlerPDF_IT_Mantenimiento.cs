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
using Portal_2_0.Controllers;

namespace Portal_2_0.Models.PDFHandlers
{
    public class FooterEventHandlerPDF_Mantenimiento : IEventHandler
    {
        PdfFont fontThyssen;
        IT_mantenimientos mantenimiento;
        string claveDocumento;
        public FooterEventHandlerPDF_Mantenimiento(PdfFont font, IT_mantenimientos mantenimiento)
        {
            this.fontThyssen = font;
            this.mantenimiento = mantenimiento;
            this.claveDocumento = mantenimiento.IATF_revisiones.IATF_documentos.clave + "-" + mantenimiento.IATF_revisiones.numero_revision.ToString("D2"); 
        }

        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent doctEvent = (PdfDocumentEvent)@event;
            PdfDocument pdfDoc = doctEvent.GetDocument();
            PdfPage page = doctEvent.GetPage();

            Rectangle rootArea = new Rectangle(35, 20, page.GetPageSize().GetWidth() - 70, 50);

            Canvas canvas = new Canvas(doctEvent.GetPage(), rootArea);

            canvas.Add(GetTable(doctEvent))

                .Close();

        }

        public Table GetTable(PdfDocumentEvent docEvent)
        {
            //porcentaje de ancho de columna
            float[] cellWidth = { 15f, 70f, 15f };
            Table tableEvent = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();

            int pageNum = docEvent.GetDocument().GetPageNumber(docEvent.GetPage());
            var thyssenColor = new DeviceRgb(0, 159, 245);
            Style styleCell = new Style().SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
            Style styleText = new Style().SetFontSize(12f).SetFontColor(thyssenColor);

            //crea la primera celda
            Cell cell = new Cell()
                .Add(new Paragraph("Pág. " + pageNum)).SetFont(fontThyssen)
                .AddStyle(styleText).SetTextAlignment(TextAlignment.LEFT)
               .SetHorizontalAlignment(HorizontalAlignment.CENTER)
               .AddStyle(styleCell).SetFontColor(new DeviceRgb(70, 70, 70));

            tableEvent.AddCell(cell);

            cell = new Cell()
               .Add(new Paragraph("engineering.tomorrow.together")).SetFont(fontThyssen)
               .AddStyle(styleText).SetTextAlignment(TextAlignment.CENTER)
               .SetHorizontalAlignment(HorizontalAlignment.CENTER)
               .AddStyle(styleCell).SetFontSize(20f);
            tableEvent.AddCell(cell);

            cell = new Cell()
            .Add(new Paragraph(claveDocumento)).SetFont(fontThyssen)
            .AddStyle(styleText).SetTextAlignment(TextAlignment.RIGHT)
            .SetHorizontalAlignment(HorizontalAlignment.RIGHT)
            .AddStyle(styleCell).SetFontColor(new DeviceRgb(70, 70, 70));
            tableEvent.AddCell(cell);

            return tableEvent;

        }
    }
}