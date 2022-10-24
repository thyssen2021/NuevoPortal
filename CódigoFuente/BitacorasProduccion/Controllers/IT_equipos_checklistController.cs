using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;
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
using System.IO;
using Portal_2_0.Models.PDFHandlers;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_equipos_checklistController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_equipos_checklist
        public ActionResult Index(int? id_inventory_item, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.IT_CHECKLIST_EQUIPOS))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            var listado = db.IT_equipos_checklist
                .Where(x => x.id_inventory_item == id_inventory_item || id_inventory_item == null)
               .OrderByDescending(x => x.fecha)
               .ThenByDescending(x => x.id)
               .Skip((pagina - 1) * cantidadRegistrosPorPagina)
              .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.IT_equipos_checklist
                .Where(x => x.id_inventory_item == id_inventory_item || id_inventory_item == null)
               .Count();

            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id_inventory_item"] = id_inventory_item;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };
            ViewBag.Paginacion = paginacion;


            ViewBag.id_inventory_item = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x => x.active == true
                && (x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.LAPTOP
                || x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.DESKTOP
                || x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.TABLET)
                ), nameof(IT_inventory_items.id), nameof(IT_inventory_items.ConcatInfoGeneral)), selected: id_inventory_item.ToString(), textoPorDefecto: "-- Todos --");


            return View(listado);
        }

        // GET: IT_equipos_checklist/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.IT_CHECKLIST_EQUIPOS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_equipos_checklist IT_equipos_checklist = db.IT_equipos_checklist.Find(id);
            if (IT_equipos_checklist == null)
            {
                return HttpNotFound();
            }
            return View(IT_equipos_checklist);
        }

        // GET: IT_equipos_checklist/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.IT_CHECKLIST_EQUIPOS))
                return View("../Home/ErrorPermisos");

            var sistemas = obtieneEmpleadoLogeado();

            IT_equipos_checklist model = new IT_equipos_checklist();
            model.id_sistemas = sistemas.id;
            model.empleados = sistemas;
            model.estatus = Bitacoras.Util.IT_equipos_checklist_estatus.EN_PROCESO;
            model.fecha = DateTime.Now;

            //agrega las actividades al modelo 
            List<IT_equipos_checklist_actividades> listActividades = db.IT_equipos_checklist_actividades.Where(x => x.activo == true && x.IT_equipos_checklist_categorias.activo == true).ToList();

            foreach (var item in listActividades)
            {
                model.IT_equipos_rel_checklist_actividades.Add(new IT_equipos_rel_checklist_actividades
                {
                    estatus = Bitacoras.Util.IT_equipos_actividad_estatus.OK,
                    id_it_equipo_actividad = item.id,
                    IT_equipos_checklist_actividades = item
                });
            }

            ViewBag.id_inventory_item = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x => x.active == true
                       && (x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.LAPTOP
                       || x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.DESKTOP
                       || x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.TABLET)
                       ), nameof(IT_inventory_items.id), nameof(IT_inventory_items.ConcatInfoGeneral)), selected: model.id_inventory_item.ToString(), textoPorDefecto: "-- Seleccionar --");

            return View(model);
        }

        // POST: IT_equipos_checklist/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IT_equipos_checklist IT_equipos_checklist)
        {
            if (db.IT_equipos_checklist.Any(x => x.id_inventory_item == IT_equipos_checklist.id_inventory_item))
                ModelState.AddModelError("", "Ya existe un checklist para el equipo seleccionado");

            if (ModelState.IsValid)
            {
                db.IT_equipos_checklist.Add(IT_equipos_checklist);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            //asigna los objetos de los valores por defecto
            var sistemas = obtieneEmpleadoLogeado();
            IT_equipos_checklist.empleados = sistemas;

            //obtiene y asigna los objetos de las actividades
            foreach (var item in IT_equipos_checklist.IT_equipos_rel_checklist_actividades)
            {
                item.IT_equipos_checklist_actividades = db.IT_equipos_checklist_actividades.Find(item.id_it_equipo_actividad);
            }

            ViewBag.id_inventory_item = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x => x.active == true
                       && (x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.LAPTOP
                       || x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.DESKTOP
                       || x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.TABLET)
                       ), nameof(IT_inventory_items.id), nameof(IT_inventory_items.ConcatInfoGeneral)), selected: IT_equipos_checklist.id_inventory_item.ToString(), textoPorDefecto: "-- Seleccionar --");


            return View(IT_equipos_checklist);
        }

        // GET: IT_equipos_checklist/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TieneRol(TipoRoles.IT_CHECKLIST_EQUIPOS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_equipos_checklist IT_equipos_checklist = db.IT_equipos_checklist.Find(id);
            if (IT_equipos_checklist == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_inventory_item = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x => x.active == true
                       && (x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.LAPTOP
                       || x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.DESKTOP
                       || x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.TABLET)
                       ), nameof(IT_inventory_items.id), nameof(IT_inventory_items.ConcatInfoGeneral)), selected: IT_equipos_checklist.id_inventory_item.ToString(), textoPorDefecto: "-- Seleccionar --");

            return View(IT_equipos_checklist);
        }

        // POST: IT_equipos_checklist/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IT_equipos_checklist IT_equipos_checklist)
        {


            if (ModelState.IsValid)
            {
                //borra los conceptos anteriores
                var list = db.IT_equipos_rel_checklist_actividades.Where(x => x.id_equipo_checklist == IT_equipos_checklist.id);
                foreach (var itemRemove in list)
                    db.IT_equipos_rel_checklist_actividades.Remove(itemRemove);

                //agrega los nuevos items rel 
                foreach (var iteamAdd in IT_equipos_checklist.IT_equipos_rel_checklist_actividades)
                    db.IT_equipos_rel_checklist_actividades.Add(iteamAdd);

                db.Entry(IT_equipos_checklist).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            //asigna los objetos de los valores por defecto
            var sistemas = obtieneEmpleadoLogeado();
            IT_equipos_checklist.empleados = sistemas;

            //obtiene y asigna los objetos de las actividades
            foreach (var item in IT_equipos_checklist.IT_equipos_rel_checklist_actividades)
            {
                item.IT_equipos_checklist_actividades = db.IT_equipos_checklist_actividades.Find(item.id_it_equipo_actividad);
            }

            ViewBag.id_inventory_item = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x => x.active == true
                       && (x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.LAPTOP
                       || x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.DESKTOP
                       || x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.TABLET)
                       ), nameof(IT_inventory_items.id), nameof(IT_inventory_items.ConcatInfoGeneral)), selected: IT_equipos_checklist.id_inventory_item.ToString(), textoPorDefecto: "-- Seleccionar --");


            return View(IT_equipos_checklist);
        }

        //genera el PDF
        public ActionResult ExportPDF(int? id, bool inline = true)
        {
            if (!TieneRol(TipoRoles.IT_CHECKLIST_EQUIPOS))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_equipos_checklist item = db.IT_equipos_checklist.Find(id);
            if (item == null)
            {
                return View("../Error/NotFound");
            }
            if (item.estatus != Bitacoras.Util.IT_equipos_checklist_estatus.FINALIZADO)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede generar el PDF de un checklist que no ha sido finalizado!";
                ViewBag.Descripcion = "Para poder generar el documento PDF, es necesario primero finalizar el checklist de registro del equipo.";

                return View("../Home/ErrorGenerico");
            }

            byte[] pdfBytes;
            using (var stream = new MemoryStream())
            using (var wri = new PdfWriter(stream))
            using (var pdf = new PdfDocument(wri))
            using (var doc = new Document(pdf, PageSize.LETTER))
            {
                //fuente principal
                iText.Kernel.Font.PdfFont font = PdfFontFactory.CreateFont(Server.MapPath("/fonts/tkmm/TKTypeMedium.ttf"));
                var thyssenColor = new DeviceRgb(0, 159, 245);

                //márgenes del documento
                doc.SetMargins(75, 35, 75, 35);

                //lineaEn blanco
                Paragraph newline = new Paragraph(new Text("\n"));

                //imagen para encabezado
                Image img = new Image(ImageDataFactory.Create(Server.MapPath("/Content/images/logo_1.png")));
                //maneja los eventos de encabezado y pie de página
                pdf.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandlerPDF_IT_checklist_equipos(img, font, item));
                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandlerPDF_IT_checklist_equipos(font, item));

                //Empieza contenido personalizado

                //estilo fuente
                Style styleFuenteThyssen = new Style().SetFont(font);

                //estilo para encabezados
                Style fuenteThyssen = new Style().SetFont(font).SetFontSize(14).SetTextAlignment(TextAlignment.CENTER);
                Style fuenteThyssenBold = new Style().SetFont(font).SetFontSize(10).SetTextAlignment(TextAlignment.LEFT).SetBold();

                //estilo para encabezados
                Style encabezado = new Style().SetFont(font).SetFontSize(10).SetFontColor(ColorConstants.WHITE).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBackgroundColor(thyssenColor).SetBold();
                //encabezado tabla
                Style encabezadoTabla = new Style().SetFontSize(10).SetFontColor(ColorConstants.BLACK).SetBorder(new SolidBorder(ColorConstants.GRAY, 1)).SetBackgroundColor(ColorConstants.LIGHT_GRAY);

                //fecha del documento
                fuenteThyssen.SetFontSize(10).SetTextAlignment(TextAlignment.RIGHT);
                doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToShortDateString()).AddStyle(fuenteThyssen));


                //Crea el parráfo que funciona como título
                Paragraph pTitle = new Paragraph("");
                pTitle.Add(" Datos del Equipo").AddStyle(encabezado);
                doc.Add(pTitle);


                fuenteThyssen.SetFontSize(10).SetTextAlignment(TextAlignment.LEFT);

                float[] cellWidth = { 15f, 35f, 15f, 35f };

                Table table = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();

                table.AddCell(new Cell().Add(new Paragraph("Hostname:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.IT_inventory_items.hostname) ? item.IT_inventory_items.hostname : String.Empty)).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Tipo:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.IT_inventory_items.IT_inventory_hardware_type.descripcion) ? item.IT_inventory_items.IT_inventory_hardware_type.descripcion : String.Empty)).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Número de Serie:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.IT_inventory_items.serial_number) ? item.IT_inventory_items.serial_number : String.Empty)).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Modelo:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.IT_inventory_items.model) ? item.IT_inventory_items.model : String.Empty)).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Sistema Operativo:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.IT_inventory_items.operation_system) ? item.IT_inventory_items.operation_system : String.Empty)).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));

                doc.Add(table);

                //---------- REGISTRO DE CHECKLIST --------     
                pTitle = new Paragraph("");
                pTitle.Add("Checklist").AddStyle(encabezado);
                doc.Add(pTitle);

                //tabla de tareas
                table = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();

                //cabecera
                table.AddCell(new Cell().Add(new Paragraph("Categoría")).AddStyle(encabezadoTabla));
                table.AddCell(new Cell().Add(new Paragraph("Actividad")).AddStyle(encabezadoTabla));
                table.AddCell(new Cell().Add(new Paragraph("Estado")).AddStyle(encabezadoTabla));
                table.AddCell(new Cell().Add(new Paragraph("Comentarios")).AddStyle(encabezadoTabla));

                //contenido de cada tabla
                int cantidadItems = 0;
                foreach (var actividad in item.IT_equipos_rel_checklist_actividades)
                {

                    if (cantidadItems == 0)
                    {
                        cantidadItems = item.IT_equipos_rel_checklist_actividades.Where(x => x.IT_equipos_checklist_actividades.id_categoria_ck == actividad.IT_equipos_checklist_actividades.id_categoria_ck).Count();
                        table.AddCell(new Cell(cantidadItems, 1).Add(new Paragraph(actividad.IT_equipos_checklist_actividades.IT_equipos_checklist_categorias.descripcion)).AddStyle(fuenteThyssen));
                    }

                    table.AddCell(new Cell().Add(new Paragraph(actividad.IT_equipos_checklist_actividades.descripcion)).AddStyle(fuenteThyssen));
                    table.AddCell(new Cell().Add(new Paragraph(actividad.estatus)).AddStyle(fuenteThyssen));
                    table.AddCell(new Cell().Add(new Paragraph(!string.IsNullOrEmpty(actividad.observacion) ? actividad.observacion : String.Empty)).AddStyle(fuenteThyssen));
                    cantidadItems--;
                }

                doc.Add(table);


                //---> Comentarios Adicionales <-------               
                pTitle = new Paragraph("");

                doc.Add(newline);
                pTitle.Add("Comentarios Adicionales").AddStyle(encabezado);
                doc.Add(pTitle);
                doc.Add(new Paragraph(!string.IsNullOrEmpty(item.comentario_general) ? item.comentario_general : String.Empty).AddStyle(fuenteThyssen).SetTextAlignment(TextAlignment.JUSTIFIED));
                //---> Cierre Comentarios Adicionales <-------            




                //---> Tabla de firmas <-------               
                //agrega dos lineas en blanco

                //  pTitle = new Paragraph("").Add(new Tab());
                //  pTitle.AddTabStops(new TabStop(20, TabAlignment.RIGHT));
                //  pTitle.Add("Firma").AddStyle(encabezado);
                //  doc.Add(pTitle);

                //  //crea tabla para firmas
                //  float[] cellWidth2 = { 50f };
                //  table = new Table(UnitValue.CreatePercentArray(cellWidth2)).UseAllAvailableWidth();
                //  table.AddCell(new Cell().Add(new Paragraph("Realizó:")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1)));
                ////  table.AddCell(new Cell().Add(new Paragraph("Aceptación de Usuario:")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));

                //  string elaboro = String.Empty, recibe = String.Empty;

                //  if (item.empleados != null)
                //      elaboro = item.empleados.ConcatNombre.ToUpper();
                //  if (item.empleados.puesto1 != null)
                //      elaboro += "\n" + item.empleados.puesto1.descripcion.ToUpper();
                //  if (item.empleados.Area != null)
                //      elaboro += "\n" + item.empleados.Area.descripcion.ToUpper();


                //  table.AddCell(new Cell().Add(new Paragraph(elaboro)
                //      .SetTextAlignment(TextAlignment.CENTER)).AddStyle(fuenteThyssen)
                //      .SetHeight(120)
                //      .SetBorder(Border.NO_BORDER)
                //      .SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1))
                //      .SetVerticalAlignment(VerticalAlignment.BOTTOM)
                //      .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                //      );


                //  table.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                //  //evita que la tabla se divida
                //  table.SetKeepTogether(true);
                //  //tabla al 50%
                //  table.SetWidth(UnitValue.CreatePercentValue(50));
                //  //centra la tabla
                //  table.SetHorizontalAlignment(HorizontalAlignment.CENTER);

                //  doc.Add(table);

                /// <---- fin tabla de firmas ------->


                //salto de página
                doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                fuenteThyssen.SetFontSize(10).SetTextAlignment(TextAlignment.JUSTIFIED);

                //tabla de movimientos         
                doc.Add(new Paragraph("Control de cambios").AddStyle(encabezado));

                float[] cellWidth3 = { 10f, 26.6f, 10f, 53.4f };

                table = new Table(UnitValue.CreatePercentArray(cellWidth3)).UseAllAvailableWidth();
                table.AddCell(new Cell().Add(new Paragraph("Fecha")).AddStyle(encabezadoTabla));
                table.AddCell(new Cell().Add(new Paragraph("Autor")).AddStyle(encabezadoTabla));
                //table.AddCell(new Cell().Add(new Paragraph("Puesto")).AddStyle(encabezadoTabla));
                table.AddCell(new Cell().Add(new Paragraph("Revisión")).AddStyle(encabezadoTabla));
                table.AddCell(new Cell().Add(new Paragraph("Descripción")).AddStyle(encabezadoTabla));


                table.AddCell(new Cell().Add(new Paragraph(new DateTime(2022, 10, 04).ToShortDateString()).AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                table.AddCell(new Cell().Add(new Paragraph("IT").AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                //  table.AddCell(new Cell().Add(new Paragraph(String.IsNullOrEmpty(revision.puesto_responsable) ? String.Empty : revision.puesto_responsable).AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                table.AddCell(new Cell().Add(new Paragraph("1").SetTextAlignment(TextAlignment.CENTER).AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                table.AddCell(new Cell().Add(new Paragraph("Alta de Formato").AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));


                doc.Add(table);
                doc.Close();
                doc.Flush();
                pdfBytes = stream.ToArray();
            }
            // return new FileContentResult(pdfBytes, "application/pdf");

            string filename = IT_matriz_requerimientosController.itfNumber + "_Responsiva de Equipo_" + item.empleados.ConcatNombre.Trim() + ".pdf";

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = filename,
                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = inline,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(pdfBytes, "application/pdf");

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
