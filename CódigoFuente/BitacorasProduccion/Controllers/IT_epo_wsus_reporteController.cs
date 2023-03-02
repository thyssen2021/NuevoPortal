using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_epo_wsus_reporteController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();
        // GET: IT_epo_wsus_reporte
        public ActionResult EpoNoInventario()
        {
            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            List<IT_epo> listado = db.IT_epo.Where(x => !String.IsNullOrEmpty(x.system_name) && !db.IT_inventory_items.Any(y => y.hostname == x.system_name)).ToList();

            return View(listado);
        }
       
        public ActionResult InventarioNoEpo()
        {
            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            List<IT_inventory_items> listado = db.IT_inventory_items.Where(
                x => !String.IsNullOrEmpty(x.hostname)
                && (x.id_inventory_type == 1    //laptop
                    || x.id_inventory_type == 2  //desktop
                    || x.id_inventory_type == 7 //server
                    || x.id_inventory_type == 14 //virtual server
                    || x.id_inventory_type == 8 //tablet
                    )
                && x.baja ==false
                && !db.IT_epo.Any(y => y.system_name == x.hostname)).ToList();

            return View(listado);
        }
        public ActionResult WsusNoInventario()
        {
            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");
            //List<string> hostnames = db.IT_wsus.Select(x=>x.)

            List<IT_wsus> listado = db.IT_wsus.Where(x => !String.IsNullOrEmpty(x.name) && !db.IT_inventory_items.Any(y => x.name.Contains(y.hostname))).ToList();

            return View(listado);
        }
        public ActionResult InventarioNoWsus()
        {
            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            List<IT_inventory_items> listado = db.IT_inventory_items.Where(
                x => !String.IsNullOrEmpty(x.hostname)
                && (x.id_inventory_type == 1    //laptop
                    || x.id_inventory_type == 2  //desktop
                    || x.id_inventory_type == 7 //server
                    || x.id_inventory_type == 14 //virtual server
                    || x.id_inventory_type == 8 //tablet
                    )
                && x.baja ==false
                && !db.IT_wsus.Any(y => y.name.Contains(x.hostname))).ToList();

            return View(listado);
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