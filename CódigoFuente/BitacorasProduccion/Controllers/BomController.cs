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
    public class BomController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: Bom
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.BITACORAS_PRODUCCION_CATALOGOS))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                return View(db.bom_en_sap.ToList());
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
