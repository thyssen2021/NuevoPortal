using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_inventory_softwareController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_inventory_software
        public ActionResult Index(string description, int pagina = 1)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                var listado = db.IT_inventory_software
                    .Where(x =>
                     (x.descripcion.Contains(description) || String.IsNullOrEmpty(description))
                    )
                  .OrderBy(x => x.descripcion)
                  .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                 .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.IT_inventory_software
                       .Where(x =>
                     (x.descripcion.Contains(description) || String.IsNullOrEmpty(description))
                    )
                         .Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["description"] = description;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;

                return View(listado);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: IT_inventory_software/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {       
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_software iT_inventory_software = db.IT_inventory_software.Find(id);
                if (iT_inventory_software == null)
                {
                    return View("../Error/NotFound");
                }
                return View(iT_inventory_software);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

          
        }

        // GET: IT_inventory_software/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                return View(new IT_inventory_software { activo = true });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_inventory_software/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IT_inventory_software iT_inventory_software, FormCollection collection)
        {


            if (ModelState.IsValid)
            {
                db.IT_inventory_software.Add(iT_inventory_software);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            return View(iT_inventory_software);
        }

        // GET: IT_inventory_software/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                IT_inventory_software iT_inventory_software = db.IT_inventory_software.Find(id);
                if (iT_inventory_software == null)
                {
                    return View("../Error/NotFound");
                }
                return View(iT_inventory_software);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: IT_inventory_software/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IT_inventory_software iT_inventory_software, FormCollection collection)
        {
            
            if (ModelState.IsValid)
            {              

               
                //guarda en BD
                db.Entry(db.IT_inventory_software.Find(iT_inventory_software.id)).CurrentValues.SetValues(iT_inventory_software);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            return View(iT_inventory_software);
        }

        public ActionResult Export(string description)
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {

                var listado = db.IT_inventory_software
                    .Where(x =>
                     (x.descripcion.Contains(description) || String.IsNullOrEmpty(description))
                    )
                  .OrderBy(x => x.id)
                  .ToList();

                //** DE MOMENTO ES EL MISMO QUE DESKTOP ***//
                byte[] stream = ExcelUtil.GeneraReporteITSoftwareExcel(listado);


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = "Inventory_Software_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(stream, "application/vnd.ms-excel");
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

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
