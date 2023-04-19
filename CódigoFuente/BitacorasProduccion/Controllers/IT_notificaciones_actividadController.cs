using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_notificaciones_actividadController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        //crea el select list para tipos de periodo
        List<SelectListItem> newListTipoPeriodo = new List<SelectListItem> {
                            new SelectListItem() {
                                    Text = IT_notificaciones_tipo_periodo.DescripcionStatus(IT_notificaciones_tipo_periodo.DIAS),
                                    Value = IT_notificaciones_tipo_periodo.DIAS
                                },
                            new SelectListItem() {
                                    Text = IT_notificaciones_tipo_periodo.DescripcionStatus(IT_notificaciones_tipo_periodo.SEMANAS),
                                    Value = IT_notificaciones_tipo_periodo.SEMANAS
                                },
                            new SelectListItem() {
                                    Text = IT_notificaciones_tipo_periodo.DescripcionStatus(IT_notificaciones_tipo_periodo.MESES),
                                    Value = IT_notificaciones_tipo_periodo.MESES
                                },
                            new SelectListItem() {
                                    Text = IT_notificaciones_tipo_periodo.DescripcionStatus(IT_notificaciones_tipo_periodo.ANIOS),
                                    Value = IT_notificaciones_tipo_periodo.ANIOS
                                },
            };

        Dictionary<string, string> dictionaryCorreos = new Dictionary<string, string>
        {
            ["C_IT.tkMM@lagermex.com.mx"] = "IT.tkMM <IT.tkMM@lagermex.com.mx>",
        };

        // GET: IT_notificaciones_actividad
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.IT_NOTIFICACIONES))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            return View(db.IT_notificaciones_actividad.ToList());
        }

        // GET: IT_notificaciones_actividad/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_notificaciones_actividad iT_notificaciones_actividad = db.IT_notificaciones_actividad.Find(id);
            if (iT_notificaciones_actividad == null)
            {
                return HttpNotFound();
            }
            return View(iT_notificaciones_actividad);
        }

        // GET: IT_notificaciones_actividad/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.IT_NOTIFICACIONES))
                return View("../Home/ErrorPermisos");

            var model = new IT_notificaciones_actividad();

            //obtiene el listado de empleados
            //recorre los usuarios con el permiso de IT_notificaciones
            AspNetRoles rol = db.AspNetRoles.Where(x => x.Name == TipoRoles.IT_NOTIFICACIONES).FirstOrDefault();
            List<AspNetUsers> usuariosInRole = new List<AspNetUsers>();
            if (rol != null)
                usuariosInRole = rol.AspNetUsers.ToList();

            List<int> idsRol = usuariosInRole.Select(x => x.IdEmpleado).Distinct().ToList();
            List<empleados> listEmpleados = db.empleados.Where(x => x.activo == true && idsRol.Contains(x.id) == true).ToList();

            //agrega los empleados seleccionables
            model.EmpleadosList = listEmpleados.Select(x => new SelectListItem()
            {
                // Selected = userRoles.Contains(x.Name),
                Text = string.Format("{0} <{1}>", x.ConcatNombre, x.correo),
                Value = "EMP_" + x.id
            });

            //agrega los correos seleccionables
            List<SelectListItem> newListCorreos = new List<SelectListItem>();
            foreach (KeyValuePair<string, string> kvp in dictionaryCorreos)
            {
                newListCorreos.Add(new SelectListItem()
                {
                    Text = kvp.Value,
                    Value = kvp.Key
                });
            }
            model.CorreosList = newListCorreos;

            //envia el select list por viewbag
            ViewBag.tipo_periodo = AddFirstItem(new SelectList(newListTipoPeriodo, "Value", "Text", model.tipo_periodo), textoPorDefecto: "-- Seleccione --");


            return View(model);
        }

        // POST: IT_notificaciones_actividad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IT_notificaciones_actividad iT_notificaciones_actividad, string[] selectedEmails, params string[] selectedEmpleados)
        {

            selectedEmpleados = selectedEmpleados ?? new string[] { };
            selectedEmails = selectedEmails ?? new string[] { };

            if (selectedEmails.Length == 0 && selectedEmpleados.Length == 0)
                ModelState.AddModelError("", "No se seleccionó ningún usuario para mandar el recordatorio.");

            if (iT_notificaciones_actividad.es_recurrente && (iT_notificaciones_actividad.fecha_fin < iT_notificaciones_actividad.fecha_inicio))
                ModelState.AddModelError("", "La fecha de fin no puede ser anterior a la fecha de inicio.");

            //ModelState.AddModelError("", "Error para depuración.");

            if (ModelState.IsValid)
            {
                #region asignacion de variables

                //lista para almacenar los recordatorios
                List<IT_notificaciones_recordatorio> iT_Notificaciones_RecordatoriosList = new List<IT_notificaciones_recordatorio>();

                //establece los dias de notificacion previa
                int? dias_p = null;
                if (iT_notificaciones_actividad.aplica_recordatorio && iT_notificaciones_actividad.dias_antes_recordatorio.HasValue)
                    dias_p = iT_notificaciones_actividad.dias_antes_recordatorio;

                //calcula las notificaciones intermedias
                DateTime fechaTemp = iT_notificaciones_actividad.fecha_inicio.Value;
                do
                {
                    //crea una copia del checlist temp, para evitar error de multiplicidad
                    List<IT_notificaciones_checklist> ckList = new List<IT_notificaciones_checklist>();
                    foreach (var notificacion in iT_notificaciones_actividad.IT_notificaciones_checklist_temp)
                    {
                        ckList.Add(new IT_notificaciones_checklist { 
                            descripcion = notificacion.descripcion,
                        });
                    }

                    iT_Notificaciones_RecordatoriosList.Add(new IT_notificaciones_recordatorio
                    {
                        fecha_programada = fechaTemp,
                        IT_notificaciones_checklist = ckList,
                        dias_previos_notificacion = dias_p
                    });
                    //aumenta la fecha segun el periodo y el tipo
                    if (iT_notificaciones_actividad.es_recurrente)
                        switch (iT_notificaciones_actividad.tipo_periodo)
                        {
                            case IT_notificaciones_tipo_periodo.DIAS:
                                fechaTemp = fechaTemp.AddDays(iT_notificaciones_actividad.periodo.Value);
                                break;
                            case IT_notificaciones_tipo_periodo.SEMANAS:
                                fechaTemp = fechaTemp.AddDays(iT_notificaciones_actividad.periodo.Value * 7);
                                break;
                            case IT_notificaciones_tipo_periodo.MESES:
                                fechaTemp = fechaTemp.AddMonths(iT_notificaciones_actividad.periodo.Value);
                                break;
                            case IT_notificaciones_tipo_periodo.ANIOS:
                                fechaTemp = fechaTemp.AddYears(iT_notificaciones_actividad.periodo.Value);
                                break;
                        }

                } while (iT_notificaciones_actividad.es_recurrente && fechaTemp <= iT_notificaciones_actividad.fecha_fin.Value);


                //agrega las notificaciones calculadas 
                iT_notificaciones_actividad.IT_notificaciones_recordatorio = iT_Notificaciones_RecordatoriosList;

                //agrega las notificaciones
                List<IT_notificaciones_usuarios> notificacionesUsuarios = new List<IT_notificaciones_usuarios>();

                //obtiene los usuarios a notificar
                foreach (var emp in selectedEmpleados)
                {
                    notificacionesUsuarios.Add(new IT_notificaciones_usuarios
                    {
                        id_empleado = Int32.Parse(emp.Split('_')[1])
                    });
                }

                //obtiene los correos a notificar
                foreach (var em in selectedEmails)
                {
                    notificacionesUsuarios.Add(new IT_notificaciones_usuarios
                    {
                        correo = em.Split('_')[1]
                    });
                }

                //agrega los usuarios seleccionados
                iT_notificaciones_actividad.IT_notificaciones_usuarios = notificacionesUsuarios;
                #endregion

                try
                {
                    db.IT_notificaciones_actividad.Add(iT_notificaciones_actividad);
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ha creado el evento correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                }

                return RedirectToAction("Index");
            }

            //envia el select list por viewbag
            ViewBag.tipo_periodo = AddFirstItem(new SelectList(newListTipoPeriodo, "Value", "Text", iT_notificaciones_actividad.tipo_periodo), textoPorDefecto: "-- Seleccione --");

            //obtiene el listado de empleados
            //recorre los usuarios con el permiso de IT_notificaciones
            AspNetRoles rol = db.AspNetRoles.Where(x => x.Name == TipoRoles.IT_NOTIFICACIONES).FirstOrDefault();
            List<AspNetUsers> usuariosInRole = new List<AspNetUsers>();
            if (rol != null)
                usuariosInRole = rol.AspNetUsers.ToList();

            List<int> idsRol = usuariosInRole.Select(x => x.IdEmpleado).Distinct().ToList();
            List<empleados> listEmpleados = db.empleados.Where(x => x.activo == true && idsRol.Contains(x.id) == true).ToList();

            //agrega los empleados seleccionables
            iT_notificaciones_actividad.EmpleadosList = listEmpleados.Select(x => new SelectListItem()
            {
                Selected = selectedEmpleados.Contains("EMP_" + x.correo),
                Text = x.ConcatNumEmpleadoNombre,
                Value = "EMP_" + x.id
            });

            //agrega los correos seleccionables
            List<SelectListItem> newListCorreos = new List<SelectListItem>();
            foreach (KeyValuePair<string, string> kvp in dictionaryCorreos)
            {
                newListCorreos.Add(new SelectListItem()
                {
                    Selected = selectedEmails.Contains(kvp.Key),
                    Text = kvp.Value,
                    Value = kvp.Key
                });
            }
            iT_notificaciones_actividad.CorreosList = newListCorreos;

            return View(iT_notificaciones_actividad);
        }

        // GET: IT_notificaciones_actividad/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_notificaciones_actividad iT_notificaciones_actividad = db.IT_notificaciones_actividad.Find(id);
            if (iT_notificaciones_actividad == null)
            {
                return HttpNotFound();
            }
            return View(iT_notificaciones_actividad);
        }

        // POST: IT_notificaciones_actividad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion,periodo,tipo_periodo,es_recurrente,mensaje,asunto")] IT_notificaciones_actividad iT_notificaciones_actividad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iT_notificaciones_actividad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(iT_notificaciones_actividad);
        }

        // GET: IT_notificaciones_actividad/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IT_notificaciones_actividad iT_notificaciones_actividad = db.IT_notificaciones_actividad.Find(id);
            if (iT_notificaciones_actividad == null)
            {
                return HttpNotFound();
            }
            return View(iT_notificaciones_actividad);
        }

        // POST: IT_notificaciones_actividad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IT_notificaciones_actividad iT_notificaciones_actividad = db.IT_notificaciones_actividad.Find(id);
            db.IT_notificaciones_actividad.Remove(iT_notificaciones_actividad);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        ///<summary>
        ///Obtiene del empleado, según el id recibido
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult GetEventos() {

            var recordatoriosList = db.IT_notificaciones_recordatorio.ToList();

            //inicializa la lista de objetos
            var objeto = new object[recordatoriosList.Count];
            var fechaHoy = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            for (int i = 0; i < recordatoriosList.Count; i++) {
                
                string color = "#008DFF";
                if (recordatoriosList[i].fecha_programada < fechaHoy)
                    color = "#7BC7F7";

                objeto[i] = new
                {
                    id = recordatoriosList[i].id.ToString(),
                    icon = "fa-check-circle",
                    title = recordatoriosList[i].IT_notificaciones_actividad.titulo,
                    start = recordatoriosList[i].fecha_programada.ToString("yyyy-MM-dd"),
                    color = color,
                    textColor = "white" // an option!

                };
            }

            return Json(objeto, JsonRequestBehavior.AllowGet);
        }
    }
}
