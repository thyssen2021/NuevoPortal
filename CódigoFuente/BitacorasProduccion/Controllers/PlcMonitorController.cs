using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [System.Web.Mvc.Authorize]
    public class PlcMonitorController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: PlcMonitor/Index/1
        public ActionResult Index(int? id)
        {

            if (!TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE) && !TieneRol(TipoRoles.BITACORAS_PRODUCCION_REPORTE_ALL_ACCESS))
                return View("../Home/ErrorPermisos");

            // ESCENARIO 1: No hay ID -> Mostrar lista de selección
            if (id == null)
            {
                // Consultamos solo los PLCs activos (Active == true o 1)
                var listaPlcs = db.PLC_Data.Where(x => x.Active == true).ToList();

                ViewBag.Title = "Seleccionar PLC";
                ViewBag.PrimerNivel = "bitacoras_produccion";
                ViewBag.SegundoNivel = "monitor_plc";

                // Retornamos una vista DIFERENTE llamada "SeleccionPlc"
                return View("SeleccionPlc", listaPlcs);
            }

            // ESCENARIO 2: Hay ID -> Mostrar Dashboard en Vivo
            // ESCENARIO 2: Hay ID -> Mostrar Dashboard en Vivo
            var plcSeleccionado = db.PLC_Data.Find(id);

            if (plcSeleccionado == null || plcSeleccionado.Active == false)
            {
                return RedirectToAction("Index");
            }

            // NUEVO: Cargar los tags activos de este PLC para generar las tarjetas
            var tagsDelPlc = db.PLC_Tags
                               .Where(t => t.ID_PLC == id && t.Active == true)
                               .OrderBy(t => t.FriendlyName) // Opcional: ordenarlos
                               .ToList();

            ViewBag.Title = $"Monitor en Vivo - {plcSeleccionado.FriendlyName}";
            ViewBag.PrimerNivel = "bitacoras_produccion";
            ViewBag.SegundoNivel = "monitor_plc";

            ViewBag.PlcId = id;

            // Pasamos la lista de tags como MODELO a la vista
            ViewBag.PlcNombre = plcSeleccionado.FriendlyName; // Nombre para mostrar en la vista

            // Pasamos la lista de tags como MODELO a la vista
            return View(tagsDelPlc);

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
