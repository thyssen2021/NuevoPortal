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

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class BomController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: Bom
        public ActionResult Index(string material, int pagina = 1)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

            
                var listaBom = db.bom_en_sap.Where(x => String.IsNullOrEmpty(material) || x.Material.Contains(material)).OrderBy(x => x.Material)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.bom_en_sap.Where(x => String.IsNullOrEmpty(material) || x.Material.Contains(material)).Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["material"] = material;

                Paginacion paginacion = new Paginacion
                {
                    PaginaActual = pagina,
                    TotalDeRegistros = totalDeRegistros,
                    RegistrosPorPagina = cantidadRegistrosPorPagina,
                    ValoresQueryString = routeValues
                };

                ViewBag.Paginacion = paginacion;

                return View(listaBom);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
            
        }

        //// GET: Bom/Details/5
        //public ActionResult Details(string id)
        //{
        //    if(TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        bom_en_sap bom_en_sap = db.bom_en_sap.Find(id);
        //        if (bom_en_sap == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(bom_en_sap);
        //    }
        //    else
        //    {
        //        return View("../Home/ErrorPermisos");
        //    }

            
        //}

        // GET: Bom/Create
        //diseñar método para cargar valores desde archivo en excel
        //public ActionResult Create()
        //{
        //    return View();
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
