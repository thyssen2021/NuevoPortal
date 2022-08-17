﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
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
using Portal_2_0.Models;
using Portal_2_0.Models.PDFHandlers;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_mantenimientosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_mantenimientos
        public ActionResult Index(int? id_empleado, int? planta_clave, bool? documento, string estatus_mantenimiento = "", int id_it_inventory_item = 0, int pagina = 1)
        {
            if (!TieneRol(TipoRoles.IT_MANTENIMIENTO_REGISTRO))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro


            var listado = db.IT_mantenimientos
                .Where(x => (x.IT_inventory_items.id_planta == planta_clave || planta_clave == null)
                    && (x.id_empleado_responsable == id_empleado
                        || x.IT_inventory_items.IT_asignacion_hardware_rel_items.Any(y => y.IT_asignacion_hardware.es_asignacion_actual == true
                            && id_empleado == y.IT_asignacion_hardware.id_empleado
                            /*&& id_empleado == y.IT_asignacion_hardware.id_responsable_principal*/)
                        || id_empleado == null
                        )
                        && (x.id_it_inventory_item == id_it_inventory_item || id_it_inventory_item == 0)
                        && (documento == null || x.id_biblioteca_digital.HasValue == documento)
                        && (
                        (estatus_mantenimiento == IT_matenimiento_Estatus.REALIZADO && x.fecha_realizacion.HasValue)
                        || (estatus_mantenimiento == IT_matenimiento_Estatus.VENCIDO && x.fecha_programada < DateTime.Now && x.fecha_realizacion == null)
                        || (estatus_mantenimiento == IT_matenimiento_Estatus.PROXIMO && x.fecha_programada > DateTime.Now && x.fecha_realizacion == null)
                        || String.IsNullOrEmpty(estatus_mantenimiento)

                        )
                )
                   .OrderByDescending(x => x.id)
                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.IT_mantenimientos
                  .Where(x => (x.IT_inventory_items.id_planta == planta_clave || planta_clave == null)
                    && (x.id_empleado_responsable == id_empleado
                        || x.IT_inventory_items.IT_asignacion_hardware_rel_items.Any(y => y.IT_asignacion_hardware.es_asignacion_actual == true
                            && id_empleado == y.IT_asignacion_hardware.id_empleado
                            /*&& id_empleado == y.IT_asignacion_hardware.id_responsable_principal*/)
                        || id_empleado == null
                        )
                        && (x.id_it_inventory_item == id_it_inventory_item || id_it_inventory_item == 0)
                        && (documento == null || x.id_biblioteca_digital.HasValue == documento)
                        && (
                        (estatus_mantenimiento == IT_matenimiento_Estatus.REALIZADO && x.fecha_realizacion.HasValue)
                        || (estatus_mantenimiento == IT_matenimiento_Estatus.VENCIDO && x.fecha_programada < DateTime.Now && x.fecha_realizacion == null)
                        || (estatus_mantenimiento == IT_matenimiento_Estatus.PROXIMO && x.fecha_programada > DateTime.Now && x.fecha_realizacion == null)
                        || String.IsNullOrEmpty(estatus_mantenimiento)

                        )
                )
                .Count();

            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["planta_clave"] = planta_clave;
            routeValues["id_empleado"] = id_empleado;
            routeValues["id_it_inventory_item"] = id_it_inventory_item;
            routeValues["estatus_mantenimiento"] = estatus_mantenimiento;
            routeValues["documento"] = documento;


            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            //select list para responsiva
            List<SelectListItem> newListStatusMantenimiento = new List<SelectListItem>();
            newListStatusMantenimiento.Add(new SelectListItem() { Text = IT_matenimiento_Estatus.DescripcionStatus(IT_matenimiento_Estatus.PROXIMO), Value = IT_matenimiento_Estatus.PROXIMO });
            newListStatusMantenimiento.Add(new SelectListItem() { Text = IT_matenimiento_Estatus.DescripcionStatus(IT_matenimiento_Estatus.REALIZADO), Value = IT_matenimiento_Estatus.REALIZADO });
            newListStatusMantenimiento.Add(new SelectListItem() { Text = IT_matenimiento_Estatus.DescripcionStatus(IT_matenimiento_Estatus.VENCIDO), Value = IT_matenimiento_Estatus.VENCIDO });
            SelectList selectListStatusResponsiva = new SelectList(newListStatusMantenimiento, "Value", "Text");

            //select list para responsiva
            List<SelectListItem> newListStatusDocumento = new List<SelectListItem>();
            newListStatusDocumento.Add(new SelectListItem() { Text = "Subido", Value = true.ToString() });
            newListStatusDocumento.Add(new SelectListItem() { Text = "Pendiente", Value = false.ToString() });
            SelectList selectListStatusDocumentoResponsiva = new SelectList(newListStatusDocumento, "Value", "Text");

            ViewBag.Paginacion = paginacion;
            ViewBag.estatus_mantenimiento = AddFirstItem(selectListStatusResponsiva, selected: estatus_mantenimiento.ToString(), textoPorDefecto: "-- Todos --");
            ViewBag.documento = AddFirstItem(selectListStatusDocumentoResponsiva, selected: documento.ToString(), textoPorDefecto: "-- Todos --");
            ViewBag.planta_clave = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true), "clave", "descripcion", planta_clave.ToString()), textoPorDefecto: "-- Todas --");
            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados, "id", "ConcatNumEmpleadoNombre"), selected: id_empleado.ToString(), textoPorDefecto: "-- Todos --");
            ViewBag.id_it_inventory_item = AddFirstItem(new SelectList(db.IT_inventory_items.Where(x =>
               x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.LAPTOP 
               || x.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.DESKTOP
            ).OrderBy(x => x.id_inventory_type), "id", "ConcatInfoSearch"), selected: id_it_inventory_item.ToString(), textoPorDefecto: "-- Todos --");

            return View(listado);
        }



        // GET: IT_mantenimientos/CerrarMantenimiento/5
        public ActionResult CerrarMantenimiento(int? id)
        {
            if (!TieneRol(TipoRoles.IT_MANTENIMIENTO_REGISTRO))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_mantenimientos iT_mantenimientos = db.IT_mantenimientos.Find(id);
            if (iT_mantenimientos == null)
            {
                return HttpNotFound();
            }

            if (iT_mantenimientos.estatus == Bitacoras.Util.IT_matenimiento_Estatus.REALIZADO)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede editar un registro que ha sido Finalizado!";
                ViewBag.Descripcion = "No se puede editar un registro de mantenimiento que ya ha sido marcado como finalizado.";

                return View("../Home/ErrorGenerico");
            }
            var sistemas = obtieneEmpleadoLogeado();

            //Asigna los valores del empleado de sistemas 
            iT_mantenimientos.id_empleado_sistemas = sistemas.id;
            iT_mantenimientos.empleados1 = sistemas;

            //id responsable por defecto
            int id_responsable_default = 0;

            if (iT_mantenimientos.id_empleado_responsable.HasValue)
                id_responsable_default = iT_mantenimientos.id_empleado_responsable.Value;
            else
            {
                //obtiene el valor del responsable principal
                var asignacion = db.IT_asignacion_hardware_rel_items.Where(x => x.id_it_inventory_item == iT_mantenimientos.id_it_inventory_item && x.IT_asignacion_hardware.es_asignacion_actual == true && x.IT_asignacion_hardware.id_empleado == x.IT_asignacion_hardware.id_responsable_principal).FirstOrDefault();
                if (asignacion != null)
                    id_responsable_default = asignacion.IT_asignacion_hardware.id_empleado;
            }


            //agrega elementos para los rel checklist            
            foreach (var itemCK in db.IT_mantenimientos_checklist_item.Where(x => x.activo))
            {
                //solo agrega si no existe previamente 
                if (!iT_mantenimientos.IT_mantenimientos_rel_checklist.Any(x => x.id_item_checklist_mantenimiento == itemCK.id))
                    iT_mantenimientos.IT_mantenimientos_rel_checklist.Add(new IT_mantenimientos_rel_checklist
                    {
                        id_item_checklist_mantenimiento = itemCK.id,
                        IT_mantenimientos_checklist_item = itemCK,
                        id_mantenimiento = iT_mantenimientos.id,
                        terminado = false
                    });
            }

            ViewBag.id_empleado_responsable = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true && x.planta_clave == iT_mantenimientos.IT_inventory_items.id_planta), "id", "ConcatNumEmpleadoNombre"), selected: id_responsable_default.ToString(), textoPorDefecto: "-- Seleccionar --");

            return View(iT_mantenimientos);
        }

        // POST: IT_mantenimientos/CerrarMantenimiento/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CerrarMantenimiento(IT_mantenimientos iT_mantenimientos)
        {
            //si es finalizar asocia docto iatf
            if (iT_mantenimientos.finalizar_mantenimiento)
            {
                //primero obtiene los documentos asociados al proceso y de la planta del usuario de sistemas
                empleados empleado_sistemas = db.empleados.Find(iT_mantenimientos.id_empleado_sistemas);
                var iatf_docto = db.IATF_documentos.Where(x => x.proceso == Bitacoras.Util.DocumentosProcesos.IT_FORMATO_HOJA_DE_VIDA
                                && x.id_planta == empleado_sistemas.planta_clave).FirstOrDefault();

                if (iatf_docto != null && iatf_docto.IATF_revisiones.Count == 0)
                    ModelState.AddModelError("", "No se encontraron revisiones asociadas al documento IATF.");

                //selecciona la revision más reciente
                if (iatf_docto != null)
                {
                    int id_version_iatf = iatf_docto.IATF_revisiones.OrderByDescending(x => x.fecha_revision).Take(1).Select(x => x.id).FirstOrDefault();
                    //asigna la versión de IATF
                    iT_mantenimientos.id_iatf_version = id_version_iatf;
                }
                //asigna la fecha de realización
                iT_mantenimientos.fecha_realizacion = DateTime.Now;
            }

            if (ModelState.IsValid)
            {

                //borra los conceptos anteriores
                var list = db.IT_mantenimientos_rel_checklist.Where(x => x.id_mantenimiento == iT_mantenimientos.id);
                foreach (var itemRemove in list)
                    db.IT_mantenimientos_rel_checklist.Remove(itemRemove);

                //agrega los nuevos items rel 
                foreach (var iteamAdd in iT_mantenimientos.IT_mantenimientos_rel_checklist)
                    db.IT_mantenimientos_rel_checklist.Add(iteamAdd);

                db.Entry(iT_mantenimientos).State = EntityState.Modified;

                //si es finalizar debe agregar una nueva entrada segun los meses de mantenimiento
                if (iT_mantenimientos.finalizar_mantenimiento)
                {
                    var inventory_item = db.IT_inventory_items.Find(iT_mantenimientos.id_it_inventory_item);

                    var nuevo_manto = new IT_mantenimientos
                    {
                        id_it_inventory_item = inventory_item.id,
                        fecha_programada = DateTime.Now.AddMonths(inventory_item.maintenance_period_months.HasValue
                        ? inventory_item.maintenance_period_months.Value : 6), //seis meses por defecto
                    };

                    db.IT_mantenimientos.Add(nuevo_manto);
                }

                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }

            //asigna propiedades previas
            iT_mantenimientos.IT_inventory_items = db.IT_inventory_items.Find(iT_mantenimientos.id_it_inventory_item);
            iT_mantenimientos.empleados = db.empleados.Find(iT_mantenimientos.id_empleado_responsable);
            iT_mantenimientos.empleados1 = db.empleados.Find(iT_mantenimientos.id_empleado_sistemas);

            //obtiene la propiedades para rels
            foreach (var rel in iT_mantenimientos.IT_mantenimientos_rel_checklist)
            {
                rel.IT_mantenimientos_checklist_item = db.IT_mantenimientos_checklist_item.Find(rel.id_item_checklist_mantenimiento);
            }

            ViewBag.id_empleado_responsable = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo == true && x.planta_clave == iT_mantenimientos.IT_inventory_items.id_planta), "id", "ConcatNumEmpleadoNombre"), selected: iT_mantenimientos.id_empleado_responsable.ToString(), textoPorDefecto: "-- Seleccionar --");

            return View(iT_mantenimientos);
        }

        //genera el PDF
        public ActionResult GenerarPDF(int? id, bool inline = true)
        {
            if (!TieneRol(TipoRoles.IT_MANTENIMIENTO_REGISTRO))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            IT_mantenimientos item = db.IT_mantenimientos.Find(id);
            if (item == null)
            {
                return View("../Error/NotFound");
            }
            if (item.estatus != Bitacoras.Util.IT_matenimiento_Estatus.REALIZADO)
            {
                ViewBag.Titulo = "¡Lo sentimos!¡No se puede generar el PDF de un mantenimiento que no ha sido finalizado!";
                ViewBag.Descripcion = "Para poder generar el documento PDF, es necesario primero finalizar el mantenimiento.";

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
                pdf.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandlerPDF_Mantenimiento(img, font, item));
                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandlerPDF_Mantenimiento(font, item));

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
                Paragraph pTitle = new Paragraph("").Add(new Tab());
                pTitle.AddTabStops(new TabStop(85, TabAlignment.RIGHT));
                pTitle.Add(" 1.- Datos del Usuario").AddStyle(encabezado);
                doc.Add(pTitle);

                fuenteThyssen.SetFontSize(10).SetTextAlignment(TextAlignment.LEFT);

                float[] cellWidth = { 15f, 35f, 15f, 35f };

                Table table = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();

                table.AddCell(new Cell().Add(new Paragraph("Nombre:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(item.empleados.ConcatNombre)).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("8ID:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.empleados.C8ID) ? item.empleados.C8ID : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Núm. Empleado:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.empleados.numeroEmpleado) ? item.empleados.numeroEmpleado : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Correo:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.empleados.correo) ? item.empleados.correo : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Planta:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(item.empleados.plantas != null ? item.empleados.plantas.descripcion : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Departamento:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(item.empleados.Area != null ? item.empleados.Area.descripcion : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Puesto:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(item.empleados.puesto1 != null ? item.empleados.puesto1.descripcion : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));

                doc.Add(table);

                //DATOS DEL EQUIPO
                pTitle = new Paragraph("").Add(new Tab());
                pTitle.AddTabStops(new TabStop(20, TabAlignment.RIGHT));
                pTitle.Add(" 2.- Datos del Equipo").AddStyle(encabezado);
                doc.Add(pTitle);

                table = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();

                if (item.IT_inventory_items.IT_inventory_hardware_type.descripcion != Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES)
                {
                    table.AddCell(new Cell().Add(new Paragraph("Tipo:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph(item.IT_inventory_items.IT_inventory_hardware_type.descripcion)).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                }

                if (item.IT_inventory_items.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.LAPTOP
                    || item.IT_inventory_items.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.DESKTOP)
                {
                    table.AddCell(new Cell().Add(new Paragraph("Hostname:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.IT_inventory_items.hostname) ? item.IT_inventory_items.hostname : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                }

                if (item.IT_inventory_items.IT_inventory_hardware_type.descripcion == Bitacoras.Util.IT_Tipos_Hardware.ACCESSORIES)
                {
                    table.AddCell(new Cell().Add(new Paragraph("Tipo Accesorio:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph(item.IT_inventory_items.IT_inventory_tipos_accesorios != null ? item.IT_inventory_items.IT_inventory_tipos_accesorios.descripcion : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                }


                table.AddCell(new Cell().Add(new Paragraph("Marca:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.IT_inventory_items.brand) ? item.IT_inventory_items.brand : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Modelo:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.IT_inventory_items.model) ? item.IT_inventory_items.model : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Núm. Serie:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.IT_inventory_items.serial_number) ? item.IT_inventory_items.serial_number : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph("Sistema Operativo:")).AddStyle(fuenteThyssenBold).SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(!String.IsNullOrEmpty(item.IT_inventory_items.operation_system) ? item.IT_inventory_items.operation_system : "--")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));

                doc.Add(table);

                //---------- REGISTRO DE CHECKLIST --------                
                //titulo
                pTitle = new Paragraph("").Add(new Tab());
                pTitle.AddTabStops(new TabStop(20, TabAlignment.RIGHT));
                pTitle.Add(" 3.- Checklist de Mantenimiento").AddStyle(encabezado);
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
                foreach (var actividad in item.IT_mantenimientos_rel_checklist)
                {

                    if (cantidadItems == 0)
                    {
                        cantidadItems = item.IT_mantenimientos_rel_checklist.Where(x => x.IT_mantenimientos_checklist_item.id_IT_mantenimientos_checklist_categorias == actividad.IT_mantenimientos_checklist_item.id_IT_mantenimientos_checklist_categorias).Count();
                        table.AddCell(new Cell(cantidadItems, 1).Add(new Paragraph(actividad.IT_mantenimientos_checklist_item.IT_mantenimientos_checklist_categorias.descripcion)).AddStyle(fuenteThyssen));
                    }

                    table.AddCell(new Cell().Add(new Paragraph(actividad.IT_mantenimientos_checklist_item.descripcion)).AddStyle(fuenteThyssen));
                    table.AddCell(new Cell().Add(new Paragraph(!actividad.terminado.HasValue ? "No Aplica" : actividad.terminado.Value ? "Terminado" : "Pendiente")).AddStyle(fuenteThyssen));
                    table.AddCell(new Cell().Add(new Paragraph(!string.IsNullOrEmpty(actividad.comentarios) ? actividad.comentarios : String.Empty)).AddStyle(fuenteThyssen));
                    cantidadItems--;
                }

                doc.Add(table);
                //---> Comentarios Adicionales <-------               
                pTitle = new Paragraph("").Add(new Tab());
                pTitle.AddTabStops(new TabStop(20, TabAlignment.RIGHT));

                doc.Add(newline);
                pTitle.Add(" 4.- Comentarios Adicionales").AddStyle(encabezado);
                doc.Add(pTitle);
                doc.Add(new Paragraph(!string.IsNullOrEmpty(item.comentarios) ? item.comentarios : "---").AddStyle(fuenteThyssen).SetTextAlignment(TextAlignment.JUSTIFIED));
                //---> Cierre Comentarios Adicionales <-------               

                //---> Tabla de firmas <-------               
                //agrega dos lineas en blanco

                pTitle = new Paragraph("").Add(new Tab());
                pTitle.AddTabStops(new TabStop(20, TabAlignment.RIGHT));
                pTitle.Add(" 5.- Firmas de Aceptación").AddStyle(encabezado);
                doc.Add(pTitle);

                //crea tabla para firmas
                float[] cellWidth2 = { 50f, 50f };
                table = new Table(UnitValue.CreatePercentArray(cellWidth2)).UseAllAvailableWidth();
                table.AddCell(new Cell().Add(new Paragraph("Realizó:")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1)));
                table.AddCell(new Cell().Add(new Paragraph("Aceptación de Usuario:")).AddStyle(fuenteThyssen).SetBorder(Border.NO_BORDER));

                string elaboro = String.Empty, recibe = String.Empty;

                if (item.empleados1 != null)
                    elaboro = item.empleados1.ConcatNombre.ToUpper();
                if (item.empleados1.puesto1 != null)
                    elaboro += "\n" + item.empleados1.puesto1.descripcion.ToUpper();
                if (item.empleados1.Area != null)
                    elaboro += "\n" + item.empleados1.Area.descripcion.ToUpper();

                if (item.empleados != null)
                    recibe = item.empleados.ConcatNombre.ToUpper();
                if (item.empleados.puesto1 != null)
                    recibe += "\n" + item.empleados.puesto1.descripcion.ToUpper();
                if (item.empleados.Area != null)
                    recibe += "\n" + item.empleados.Area.descripcion.ToUpper();

                table.AddCell(new Cell().Add(new Paragraph(elaboro)
                    .SetTextAlignment(TextAlignment.CENTER)).AddStyle(fuenteThyssen)
                    .SetHeight(120)
                    .SetBorder(Border.NO_BORDER)
                    .SetBorderRight(new SolidBorder(ColorConstants.BLACK, 1))
                    .SetVerticalAlignment(VerticalAlignment.BOTTOM)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                    );

                table.AddCell(new Cell().Add(new Paragraph(recibe).SetTextAlignment(TextAlignment.CENTER)).AddStyle(fuenteThyssen)
                    .SetHeight(120)
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.BOTTOM)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER));

                table.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                //evita que la tabla se divida
                table.SetKeepTogether(true);

                doc.Add(table);

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

                foreach (var revision in item.IATF_revisiones.IATF_documentos.IATF_revisiones.OrderBy(x => x.numero_revision))
                {
                    table.AddCell(new Cell().Add(new Paragraph(revision.fecha_revision.ToShortDateString()).AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                    table.AddCell(new Cell().Add(new Paragraph(revision.responsable).AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                    //  table.AddCell(new Cell().Add(new Paragraph(String.IsNullOrEmpty(revision.puesto_responsable) ? String.Empty : revision.puesto_responsable).AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                    table.AddCell(new Cell().Add(new Paragraph(revision.numero_revision.ToString()).SetTextAlignment(TextAlignment.CENTER).AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                    table.AddCell(new Cell().Add(new Paragraph(revision.descripcion).AddStyle(fuenteThyssen)).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1)));
                }

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

        // GET: IT_mantenimientos/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.IT_MANTENIMIENTO_REGISTRO))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_mantenimientos iT_mantenimientos = db.IT_mantenimientos.Find(id);
            if (iT_mantenimientos == null)
            {
                return HttpNotFound();
            }
            return View(iT_mantenimientos);
        }

        // GET: IT_mantenimientos/CargarDocumento/5
        public ActionResult CargarDocumento(int? id)
        {
            if (!TieneRol(TipoRoles.IT_MANTENIMIENTO_REGISTRO))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_mantenimientos iT_mantenimientos = db.IT_mantenimientos.Find(id);
            if (iT_mantenimientos == null)
            {
                return HttpNotFound();
            }
            return View(iT_mantenimientos);
        }

        // POST: IT_asignacion_hardware/CargarResponsiva
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CargarDocumento(IT_mantenimientos it_mantenimiento)
        {

            IT_mantenimientos item = db.IT_mantenimientos.Find(it_mantenimiento.id);

            if (it_mantenimiento.PostedFileDocumentoAceptacion != null && it_mantenimiento.PostedFileDocumentoAceptacion.InputStream.Length > 5242880)
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 5MB.");
            else if (it_mantenimiento.PostedFileDocumentoAceptacion != null)
            { //verifica la extensión del archivo
                string extension = System.IO.Path.GetExtension(it_mantenimiento.PostedFileDocumentoAceptacion.FileName);
                if (extension.ToUpper() != ".XLS"   //si no contiene una extensión válida
                               && extension.ToUpper() != ".XLSX"
                               && extension.ToUpper() != ".DOC"
                               && extension.ToUpper() != ".DOCX"
                               && extension.ToUpper() != ".PDF"
                               && extension.ToUpper() != ".PNG"
                               && extension.ToUpper() != ".JPG"
                               && extension.ToUpper() != ".JPEG"
                               )
                    ModelState.AddModelError("", "Sólo se permiten archivos con extensión .xls, .xlsx, .doc, .docx, .pdf, .png, .jpg, .jpeg.");
                else
                { //si la extension y el tamaño son válidos
                    String nombreArchivo = it_mantenimiento.PostedFileDocumentoAceptacion.FileName;

                    //recorta el nombre del archivo en caso de ser necesario
                    if (nombreArchivo.Length > 80)
                        nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                    //lee el archivo en un arreglo de bytes
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(it_mantenimiento.PostedFileDocumentoAceptacion.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(it_mantenimiento.PostedFileDocumentoAceptacion.ContentLength);
                    }

                    //genera el archivo de biblioce digital
                    biblioteca_digital archivo = new biblioteca_digital
                    {
                        Nombre = nombreArchivo,
                        MimeType = UsoStrings.RecortaString(it_mantenimiento.PostedFileDocumentoAceptacion.ContentType, 80),
                        Datos = fileData
                    };

                    //en caso de exister un archivo realiza un update
                    if (item.biblioteca_digital != null)
                    {
                        item.biblioteca_digital.Nombre = archivo.Nombre;
                        item.biblioteca_digital.MimeType = archivo.MimeType;
                        item.biblioteca_digital.Datos = archivo.Datos;

                        db.Entry(item.biblioteca_digital).State = EntityState.Modified;
                    }
                    else
                    {      //si no existe el archivo lo crea
                        item.biblioteca_digital = archivo;
                    }

                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert("Se ha subido el documento de aceptación correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                    TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                    return RedirectToAction("Index");

                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Index");
                }
            }
            return View(item);
        }


        //// GET: IT_mantenimientos/Create
        //public ActionResult Create()
        //{
        //    ViewBag.id_biblioteca_digital = new SelectList(db.biblioteca_digital, "Id", "Nombre");
        //    ViewBag.id_empleado_responsable = new SelectList(db.empleados, "id", "numeroEmpleado");
        //    ViewBag.id_empleado_sistemas = new SelectList(db.empleados, "id", "numeroEmpleado");
        //    ViewBag.id_iatf_version = new SelectList(db.IATF_revisiones, "id", "responsable");
        //    ViewBag.id_it_inventory_item = new SelectList(db.IT_inventory_items, "id", "comments");
        //    return View();
        //}

        //// POST: IT_mantenimientos/Create
        //// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        //// más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id,id_it_inventory_item,id_empleado_responsable,id_empleado_sistemas,id_iatf_version,id_biblioteca_digital,fecha_programada,fecha_realizacion,comentarios")] IT_mantenimientos iT_mantenimientos)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.IT_mantenimientos.Add(iT_mantenimientos);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.id_biblioteca_digital = new SelectList(db.biblioteca_digital, "Id", "Nombre", iT_mantenimientos.id_biblioteca_digital);
        //    ViewBag.id_empleado_responsable = new SelectList(db.empleados, "id", "numeroEmpleado", iT_mantenimientos.id_empleado_responsable);
        //    ViewBag.id_empleado_sistemas = new SelectList(db.empleados, "id", "numeroEmpleado", iT_mantenimientos.id_empleado_sistemas);
        //    ViewBag.id_iatf_version = new SelectList(db.IATF_revisiones, "id", "responsable", iT_mantenimientos.id_iatf_version);
        //    ViewBag.id_it_inventory_item = new SelectList(db.IT_inventory_items, "id", "comments", iT_mantenimientos.id_it_inventory_item);
        //    return View(iT_mantenimientos);
        //}

        //// GET: IT_mantenimientos/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    IT_mantenimientos iT_mantenimientos = db.IT_mantenimientos.Find(id);
        //    if (iT_mantenimientos == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.id_biblioteca_digital = new SelectList(db.biblioteca_digital, "Id", "Nombre", iT_mantenimientos.id_biblioteca_digital);
        //    ViewBag.id_empleado_responsable = new SelectList(db.empleados, "id", "numeroEmpleado", iT_mantenimientos.id_empleado_responsable);
        //    ViewBag.id_empleado_sistemas = new SelectList(db.empleados, "id", "numeroEmpleado", iT_mantenimientos.id_empleado_sistemas);
        //    ViewBag.id_iatf_version = new SelectList(db.IATF_revisiones, "id", "responsable", iT_mantenimientos.id_iatf_version);
        //    ViewBag.id_it_inventory_item = new SelectList(db.IT_inventory_items, "id", "comments", iT_mantenimientos.id_it_inventory_item);
        //    return View(iT_mantenimientos);
        //}

        //// POST: IT_mantenimientos/Edit/5
        //// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        //// más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "id,id_it_inventory_item,id_empleado_responsable,id_empleado_sistemas,id_iatf_version,id_biblioteca_digital,fecha_programada,fecha_realizacion,comentarios")] IT_mantenimientos iT_mantenimientos)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(iT_mantenimientos).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.id_biblioteca_digital = new SelectList(db.biblioteca_digital, "Id", "Nombre", iT_mantenimientos.id_biblioteca_digital);
        //    ViewBag.id_empleado_responsable = new SelectList(db.empleados, "id", "numeroEmpleado", iT_mantenimientos.id_empleado_responsable);
        //    ViewBag.id_empleado_sistemas = new SelectList(db.empleados, "id", "numeroEmpleado", iT_mantenimientos.id_empleado_sistemas);
        //    ViewBag.id_iatf_version = new SelectList(db.IATF_revisiones, "id", "responsable", iT_mantenimientos.id_iatf_version);
        //    ViewBag.id_it_inventory_item = new SelectList(db.IT_inventory_items, "id", "comments", iT_mantenimientos.id_it_inventory_item);
        //    return View(iT_mantenimientos);
        //}

        //// GET: IT_mantenimientos/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    IT_mantenimientos iT_mantenimientos = db.IT_mantenimientos.Find(id);
        //    if (iT_mantenimientos == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(iT_mantenimientos);
        //}

        //// POST: IT_mantenimientos/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    IT_mantenimientos iT_mantenimientos = db.IT_mantenimientos.Find(id);
        //    db.IT_mantenimientos.Remove(iT_mantenimientos);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
