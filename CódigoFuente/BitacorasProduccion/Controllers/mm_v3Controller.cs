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
    public class mm_v3Controller : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: mm_v3
        public ActionResult Index(string material, string tipoMaterial, int pagina = 1)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var cantidadRegistrosPorPagina = 20; // parámetro

                //en caso de que material este vacio o contenga el parameto material              

                var listaBom = db.mm_v3.Where(
                        x => (String.IsNullOrEmpty(material) || x.Material.Contains(material))
                        && (String.IsNullOrEmpty(tipoMaterial) || x.Type_of_Material.Contains(tipoMaterial))
                        )
                    .OrderBy(x => x.Material)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                    .Take(cantidadRegistrosPorPagina).ToList();

                var totalDeRegistros = db.mm_v3.Where(x => (String.IsNullOrEmpty(material) || x.Material.Contains(material))
                       && (String.IsNullOrEmpty(tipoMaterial) || x.Type_of_Material.Contains(tipoMaterial))).Count();

                System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
                routeValues["material"] = material;
                routeValues["tipoMaterial"] = tipoMaterial;

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
