using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Microsoft.AspNet.Identity;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class ConcurController : BaseController // Hereda de BaseController para tener acceso a _userManager y TieneRol
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // Vistas Razor (contenedores de React Islands)
        public ActionResult SolicitudRH()
        {
            if (!TieneRol(TipoRoles.CONCUR_RH_SOLICITUD))
                return View("../Home/ErrorPermisos");

            ViewBag.Title = "Solicitud de Usuario Concur";
            return View();
        }

        public ActionResult DashboardIT()
        {
            if (!TieneRol(TipoRoles.CONCUR_IT_ADMINISTRACION))
                return View("../Home/ErrorPermisos");

            ViewBag.Title = "Dashboard IT - Solicitudes Concur";
            return View();
        }

        // Endpoint para que React busque al empleado por su C8ID
        [HttpGet]
        public JsonResult GetEmployeeData(string employeeId)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var empleado = db.empleados
                .Where(e => e.C8ID == employeeId || e.numeroEmpleado == employeeId)
                .Select(e => new
                {
                    GlobalEmployeeID = e.C8ID,
                    FirstName = e.nombre,
                    LastName = e.apellido1 + " " + e.apellido2,
                    EmailAddress = e.correo,
                    LoginId = e.C8ID + "@thyssenkrupp"
                })
                .FirstOrDefault();

            if (empleado == null)
            {
                return Json(new { success = false, message = "Empleado no encontrado." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, data = empleado }, JsonRequestBehavior.AllowGet);
        }

        // Endpoint para que RH guarde la solicitud
        [HttpPost]
        public JsonResult CreateRequest(ConcurRequestViewModel model)
        {
            if (!TieneRol(TipoRoles.CONCUR_RH_SOLICITUD))
                return Json(new { success = false, message = "No tienes permisos." });

            // Validar obligatoriedad técnica en servidor
            if (string.IsNullOrEmpty(model.GlobalEmployeeID) ||
                string.IsNullOrEmpty(model.FirstName) ||
                string.IsNullOrEmpty(model.LastName) ||
                string.IsNullOrEmpty(model.CostCenterValue) ||
                string.IsNullOrEmpty(model.ApproverEmployeeID))
            {
                return Json(new { success = false, message = "Todos los campos marcados con * son obligatorios." });
            }

            try
            {
                var nuevaSolicitud = new IT_Concur_Solicitudes
                {
                    GlobalEmployeeID = model.GlobalEmployeeID,
                    FirstName = model.FirstName.ToUpper(), // Normalizar a mayúsculas
                    LastName = model.LastName.ToUpper(),
                    LoginId = model.LoginId,
                    EmailAddress = model.EmailAddress.ToLower(),
                    CostCenterValue = model.CostCenterValue,
                    ApproverEmployeeID = model.ApproverEmployeeID,
                    IsApprover = model.IsApprover ?? "N",
                    Estatus = "Pendiente",
                    UsuarioSolicita = User.Identity.GetUserId(),
                    FechaSolicitud = DateTime.Now
                };

                db.IT_Concur_Solicitudes.Add(nuevaSolicitud);
                db.SaveChanges();

                // Disparar Notificación por Correo a IT
                try
                {
                    // Consultar correos activos para este proceso
                    var correosIT = db.notificaciones_correo
                                      .Where(n => n.descripcion == "CONCUR_IT_ADMIN" && n.activo)
                                      .Select(n => n.correo)
                                      .ToList();

                    if (correosIT.Count > 0)
                    {
                        EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();

                        // Extraer el nombre de quien ejecuta la acción, por defecto Name (Username)
                        string nombreRH = User.Identity.Name;

                        var empleado = obtieneEmpleadoLogeado();

                        string nombreSolicitante = empleado.nombre + " " + empleado.apellido1; 

                        string bodyMail = envioCorreo.getBodyConcurNuevaSolicitudIT(nuevaSolicitud, nombreSolicitante);
                        string asuntoMail = "Nueva Solicitud de Alta Concur #" + nuevaSolicitud.IdSolicitud;

                        // Ejecutar envío en hilo asíncrono
                        envioCorreo.SendEmailAsync(correosIT, asuntoMail, bodyMail);
                    }
                }
                catch (Exception exMail)
                {
                    // Registrar error en debug sin detener la ejecución de la petición JSON
                    System.Diagnostics.Debug.Print("Error enviando notificación Concur: " + exMail.Message);
                }

                return Json(new { success = true, message = "Solicitud enviada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // 1. Vista para que RH vea su historial
        public ActionResult MisSolicitudes()
        {
            if (!TieneRol(TipoRoles.CONCUR_RH_SOLICITUD))
                return View("../Home/ErrorPermisos");

            ViewBag.Title = "Mis Solicitudes Concur";
            return View();
        }

        // 2. Endpoint que devuelve el listado filtrado por el usuario actual
        [HttpGet]
        public JsonResult GetMyRequests()
        {
            if (!TieneRol(TipoRoles.CONCUR_RH_SOLICITUD))
                return Json(new { success = false, message = "Sin permisos." }, JsonRequestBehavior.AllowGet);

            db.Configuration.ProxyCreationEnabled = false;

            string currentUserId = User.Identity.GetUserId();

            var solicitudes = db.IT_Concur_Solicitudes
                .Where(s => s.UsuarioSolicita == currentUserId)
                .Select(s => new
                {
                    s.IdSolicitud,
                    s.GlobalEmployeeID,
                    s.FirstName,
                    s.LastName,
                    s.FechaSolicitud,
                    s.Estatus,
                    // NUEVOS DATOS AÑADIDOS AL REPORTE
                    s.CostCenterValue,
                    s.ApproverEmployeeID,
                    s.IsApprover
                })
                .OrderByDescending(s => s.FechaSolicitud)
                .ToList();

            return Json(new { success = true, data = solicitudes }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllRequests()
        {
            if (!TieneRol(TipoRoles.CONCUR_IT_ADMINISTRACION))
                return Json(new { success = false, message = "Sin permisos." }, JsonRequestBehavior.AllowGet);

            db.Configuration.ProxyCreationEnabled = false;

            var solicitudes = db.IT_Concur_Solicitudes
                .Select(s => new
                {
                    s.IdSolicitud,
                    s.GlobalEmployeeID,
                    s.FirstName,
                    s.LastName,
                    s.FechaSolicitud,
                    s.Estatus,
                    s.CostCenterValue,
                    s.ApproverEmployeeID,
                    s.IsApprover
                })
                .OrderByDescending(s => s.FechaSolicitud) // Las más recientes primero
                .ToList();

            return Json(new { success = true, data = solicitudes }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateStatus(int id, string nuevoEstatus)
        {
            if (!TieneRol(TipoRoles.CONCUR_IT_ADMINISTRACION))
                return Json(new { success = false, message = "Sin permisos." });

            try
            {
                var solicitud = db.IT_Concur_Solicitudes.Find(id);
                if (solicitud == null)
                    return Json(new { success = false, message = "Solicitud no encontrada." });

                solicitud.Estatus = nuevoEstatus;

                if (nuevoEstatus == "Completado")
                {
                    solicitud.UsuarioProcesa = User.Identity.GetUserId();
                    solicitud.FechaProcesado = DateTime.Now;
                }

                db.SaveChanges();

                // -----------------------------------------------------------------
                // NOTIFICACIÓN A RH SI EL ESTATUS PASA A "COMPLETADO"
                // -----------------------------------------------------------------
                if (nuevoEstatus == "Completado" && !string.IsNullOrEmpty(solicitud.UsuarioSolicita))
                {
                    try
                    {
                        // Buscar el correo del solicitante evaluando IdEmpleado
                        // Prioriza empleados.correo, si no existe o IdEmpleado es 0, toma AspNetUsers.Email
                        string queryCorreo = @"
                            SELECT COALESCE(e.correo, u.Email)
                            FROM AspNetUsers u
                            LEFT JOIN empleados e ON u.IdEmpleado = e.id AND u.IdEmpleado > 0
                            WHERE u.Id = @p0";

                        string correoSolicitante = db.Database.SqlQuery<string>(
                            queryCorreo,
                            solicitud.UsuarioSolicita
                        ).FirstOrDefault();

                        if (!string.IsNullOrEmpty(correoSolicitante))
                        {
                            EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                            string bodyMail = envioCorreo.getBodyConcurSolicitudCompletadaRH(solicitud);
                            string asuntoMail = "✅ Solicitud Concur Completada #" + solicitud.IdSolicitud;

                            // Crear una lista con el único destinatario
                            List<string> destinatarios = new List<string> { correoSolicitante };

                            envioCorreo.SendEmailAsync(destinatarios, asuntoMail, bodyMail);
                        }
                    }
                    catch (Exception exMail)
                    {
                        System.Diagnostics.Debug.Print("Error enviando notificación RH (Completado): " + exMail.Message);
                    }
                }

                return Json(new { success = true, message = "Estatus actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al actualizar: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetEmployeesList()
        {
            if (!TieneRol(TipoRoles.CONCUR_RH_SOLICITUD))
                return Json(new { success = false, message = "Sin permisos." }, JsonRequestBehavior.AllowGet);

            db.Configuration.ProxyCreationEnabled = false;

            var empleados = db.empleados
                .Where(e => e.activo == true && !string.IsNullOrEmpty(e.C8ID))
                .Select(e => new
                {
                    Value = e.C8ID,
                    Text = "8ID: " + e.C8ID + " | Núm. Emp: " + e.numeroEmpleado + " | " + e.nombre + " " + e.apellido1 + " " + e.apellido2,

                    GlobalEmployeeID = e.C8ID,
                    FirstName = e.nombre,
                    LastName = e.apellido1 + " " + e.apellido2,
                    EmailAddress = e.correo,
                    LoginId = e.C8ID + "@thyssenkrupp",

                    // NUEVO: Buscar el 8ID del jefe directo mediante subconsulta a la misma tabla
                    ApproverEmployeeID = db.empleados.Where(j => j.id == e.id_jefe_directo).Select(j => j.C8ID).FirstOrDefault()
                })
                .ToList();

            var jsonResult = Json(new { success = true, data = empleados }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult DownloadConcurFile(int requestId)
        {
            if (!TieneRol(TipoRoles.CONCUR_IT_ADMINISTRACION))
                return new HttpStatusCodeResult(403, "Sin permisos");

            var solicitud = db.IT_Concur_Solicitudes.Find(requestId);
            if (solicitud == null)
                return HttpNotFound("Solicitud no encontrada.");

            // Inicializar arreglo de 137 elementos en vacío
            string[] cols = new string[137];
            for (int i = 0; i < 137; i++) { cols[i] = ""; }

            // Asignar valores por índice exacto (Índice = Columna - 1)
            cols[0] = "305";
            cols[1] = solicitud.FirstName;
            cols[3] = solicitud.LastName;
            cols[4] = solicitud.GlobalEmployeeID;
            cols[5] = solicitud.LoginId;
            cols[7] = solicitud.EmailAddress;
            cols[8] = "es_MX";
            cols[9] = "MEX";
            cols[11] = "TK_Symbolic";
            cols[12] = "MXN";
            cols[14] = "Y";
            cols[15] = "MEX";
            cols[16] = "MX"; // Ajustado a MX según requerimiento                                      
            cols[17] = "0000801495";
            cols[18] = "5090";
            cols[19] = "CC";
            cols[20] = solicitud.CostCenterValue;
            cols[24] = solicitud.GlobalEmployeeID;
            cols[37] = solicitud.GlobalEmployeeID;
            cols[41] = "MEX0000801495";
            cols[42] = "N";
            cols[43] = "N";
            cols[46] = "N";
            cols[47] = "N";
            cols[48] = "N";
            cols[49] = "N";
            cols[50] = "N";
            cols[51] = "N";
            cols[52] = "N";
            cols[58] = solicitud.ApproverEmployeeID;
            cols[63] = solicitud.IsApprover;
            cols[136] = "EOL";

            // Unir el arreglo con el delimitador PIPE (|)
            string dataLine = string.Join("|", cols);

            // Definir el encabezado fijo obligatorio
            string headerLine = "100|0|SSO|UPDATE|EN|N|N";

            // Ensamblar el contenido final (Encabezado + Salto de línea + Datos)
            string finalFileContent = headerLine + "\r\n" + dataLine;

            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(finalFileContent);

            string fileName = $"Concur_Req_{solicitud.GlobalEmployeeID}_{DateTime.Now:yyyyMMdd}.txt";
            return File(fileBytes, "text/plain", fileName);
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
