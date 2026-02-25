using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Portal_2_0.Models; // Asegúrate de que este namespace apunte a tu EDMX

namespace Portal_2_0.Controllers
{
    [AllowAnonymous] // Permite el acceso sin login, como pediste
    public class UtilsController : Controller
    {
        // Instanciamos tu contexto de Entity Framework
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // 1. El endpoint que renderiza la vista de Razor
        public ActionResult DashboardVisitasLagermex()
        {
            return View();
        }

        // 2. El endpoint de la API que alimenta a React
        [HttpGet]
        public JsonResult ObtenerRegistrosReales(string fechaInicio, string fechaFin) // <-- Aquí está la magia
        {
            // Evita errores de referencias circulares al serializar a JSON en EF6
            db.Configuration.ProxyCreationEnabled = false;

            try
            {
                // 1. Validamos y convertimos la fecha de inicio (por defecto 30 días atrás)
                DateTime inicio = string.IsNullOrEmpty(fechaInicio)
                    ? DateTime.Now.Date.AddDays(-30)
                    : DateTime.Parse(fechaInicio).Date;

                // 2. Validamos y convertimos la fecha fin (por defecto hoy a las 23:59:59)
                DateTime fin = string.IsNullOrEmpty(fechaFin)
                    ? DateTime.Now.Date.AddDays(1).AddTicks(-1)
                    : DateTime.Parse(fechaFin).Date.AddDays(1).AddTicks(-1);

                // 3. Aplicamos el filtro real a la base de datos
                var visitasRaw = db.VisitasLagermex
                                   .Where(v => v.FechaVisita >= inicio && v.FechaVisita <= fin)
                                   .OrderByDescending(v => v.FechaVisita)
                                   .ToList();

                // A. Formateamos el DETALLE para la tabla
                var detalleTabla = visitasRaw.Select(v => new
                {
                    Id = v.Id,
                    Fecha = v.FechaVisita.HasValue ? v.FechaVisita.Value.ToString("yyyy-MM-dd HH:mm") : "",
                    Ubicacion = string.IsNullOrEmpty(v.Ubicacion) ? "Desconocida" : v.Ubicacion,
                    Navegador = ExtraerNavegadorCorto(v.UserAgent),
                    Usuario = string.IsNullOrEmpty(v.VisitorId) ? "Anónimo" : "Identificado",
                    Estatus = "Completado"
                }).ToList();

                // B. AGRUPAMOS por día para la gráfica
                var agrupadoGrafica = visitasRaw
                    .Where(v => v.FechaVisita.HasValue)
                    .GroupBy(v => v.FechaVisita.Value.ToString("yyyy-MM-dd"))
                    .OrderBy(g => g.Key)
                    .Select(g => new
                    {
                        Fecha = g.Key,
                        Cantidad = g.Count()
                    }).ToList();

                return Json(new { success = true, tabla = detalleTabla, grafica = agrupadoGrafica }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Método auxiliar para limpiar el UserAgent y que se vea bien en tu tabla
        private string ExtraerNavegadorCorto(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return "Desconocido";
            string ua = userAgent.ToLower();

            if (ua.Contains("edg/")) return "Microsoft Edge";
            if (ua.Contains("chrome/")) return "Google Chrome";
            if (ua.Contains("firefox/")) return "Mozilla Firefox";
            if (ua.Contains("safari/") && !ua.Contains("chrome/")) return "Apple Safari";

            return "Otro";
        }

        // Liberamos la conexión
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