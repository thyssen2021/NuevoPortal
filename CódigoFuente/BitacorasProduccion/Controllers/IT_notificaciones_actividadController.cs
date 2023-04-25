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
        public ActionResult Index(string estatus)
        {
            if (!TieneRol(TipoRoles.IT_NOTIFICACIONES))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            List<SelectListItem> newList = new List<SelectListItem>();
            newList.Add(new SelectListItem() { Text = IT_notificaciones_recordatorio_estatus.DescripcionStatus(IT_notificaciones_recordatorio_estatus.PENDIENTE), Value = IT_notificaciones_recordatorio_estatus.PENDIENTE });
            newList.Add(new SelectListItem() { Text = IT_notificaciones_recordatorio_estatus.DescripcionStatus(IT_notificaciones_recordatorio_estatus.TERMINADO), Value = IT_notificaciones_recordatorio_estatus.TERMINADO });

            //envia el select list por viewbag
            ViewBag.estatus = AddFirstItem(new SelectList(newList, "Value", "Text", estatus), textoPorDefecto: "-- Todos --");

            return View();
        }

        // GET: IT_notificaciones_actividad/Details/5
        public ActionResult Details(int? id)
        {
            if (!TieneRol(TipoRoles.IT_NOTIFICACIONES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IT_notificaciones_recordatorio recordatorio = db.IT_notificaciones_recordatorio.Find(id);
            if (recordatorio == null)
            {
                return HttpNotFound();
            }
            return View(recordatorio);
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

                //inicializa checklist de actividades
                iT_notificaciones_actividad.IT_notificaciones_checklist_temp = iT_notificaciones_actividad.IT_notificaciones_checklist_temp ?? new List<IT_notificaciones_checklist> { };

                //calcula las notificaciones intermedias
                DateTime fechaTemp = iT_notificaciones_actividad.fecha_inicio.Value;
                do
                {
                    //crea una copia del checklist temp, para evitar error de multiplicidad
                    List<IT_notificaciones_checklist> ckList = new List<IT_notificaciones_checklist>();
                    foreach (var notificacion in iT_notificaciones_actividad.IT_notificaciones_checklist_temp)
                    {
                        ckList.Add(new IT_notificaciones_checklist
                        {
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
                Selected = selectedEmpleados.Contains("EMP_" + x.id),
                Text = string.Format("{0} <{1}>", x.ConcatNombre, x.correo),
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
            if (!TieneRol(TipoRoles.IT_NOTIFICACIONES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IT_notificaciones_recordatorio recordatorio = db.IT_notificaciones_recordatorio.Find(id);
            if (recordatorio == null)
            {
                return HttpNotFound();
            }

            #region listado de empleados 
            //obtiene el listado de empleados
            //recorre los usuarios con el permiso de IT_notificaciones
            AspNetRoles rol = db.AspNetRoles.Where(x => x.Name == TipoRoles.IT_NOTIFICACIONES).FirstOrDefault();
            List<AspNetUsers> usuariosInRole = new List<AspNetUsers>();
            if (rol != null)
                usuariosInRole = rol.AspNetUsers.ToList();

            List<int> idsRol = usuariosInRole.Select(x => x.IdEmpleado).Distinct().ToList();
            List<empleados> listEmpleados = db.empleados.Where(x => x.activo == true && idsRol.Contains(x.id) == true).ToList();

            //obtiene el listado de empleados seleccionados en BD
            var empleadosBD = recordatorio.IT_notificaciones_actividad.IT_notificaciones_usuarios.Where(x => x.id_empleado.HasValue).Select(x => x.id_empleado).ToList();
            string[] selectedEmpleados = new string[empleadosBD.Count];
            for (int i = 0; i < empleadosBD.Count; i++)
            {
                selectedEmpleados[i] = string.Format("EMP_{0}", empleadosBD[i].ToString());
            }

            //agrega los empleados seleccionables
            recordatorio.IT_notificaciones_actividad.EmpleadosList = listEmpleados.Select(x => new SelectListItem()
            {
                // Selected = userRoles.Contains(x.Name),
                Selected = selectedEmpleados.Contains("EMP_" + x.id),
                Text = string.Format("{0} <{1}>", x.ConcatNombre, x.correo),
                Value = "EMP_" + x.id
            });

            //obtiene el listado de emails seleccionados en BD
            var emailsBD = recordatorio.IT_notificaciones_actividad.IT_notificaciones_usuarios.Where(x => !x.id_empleado.HasValue).Select(x => x.correo).ToList();
            string[] selectedEmails = new string[emailsBD.Count];
            for (int i = 0; i < emailsBD.Count; i++)
            {
                selectedEmails[i] = string.Format("C_{0}", emailsBD[i]);
            }

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
            recordatorio.IT_notificaciones_actividad.CorreosList = newListCorreos;

            #endregion

            var sistemas = obtieneEmpleadoLogeado();
            recordatorio.id_sistemas = sistemas.id;
            recordatorio.aplica_recordatorio = recordatorio.dias_previos_notificacion.HasValue;
            ViewBag.NombreEmpleado = sistemas.ConcatNumEmpleadoNombre;

            return View(recordatorio);
        }

        // POST: IT_notificaciones_actividad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IT_notificaciones_recordatorio recordatorio, string tipo_save, string[] selectedEmails, params string[] selectedEmpleados)
        {
            selectedEmpleados = selectedEmpleados ?? new string[] { };
            selectedEmails = selectedEmails ?? new string[] { };

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
            recordatorio.IT_notificaciones_actividad.IT_notificaciones_usuarios = notificacionesUsuarios;

            if (ModelState.IsValid)
            {
                //obtiene el recordatorio a modificar
                var recordatorioBD = db.IT_notificaciones_recordatorio.Find(recordatorio.id);
                //modifica los valores del recordatorio
                recordatorioBD.fecha_programada = recordatorio.fecha_programada;
                //obtiene el valor de aplica recordatorio 
                if (recordatorio.aplica_recordatorio)
                    recordatorioBD.dias_previos_notificacion = recordatorio.dias_previos_notificacion;
                else
                    recordatorioBD.dias_previos_notificacion = null;

                recordatorioBD.comentario_cierre = recordatorio.comentario_cierre;
                recordatorioBD.id_sistemas = recordatorio.id_sistemas;

                //valores que aplican si no es recurrente
                if (!recordatorioBD.IT_notificaciones_actividad.es_recurrente)
                {
                    recordatorioBD.IT_notificaciones_actividad.titulo = recordatorio.IT_notificaciones_actividad.titulo;
                    recordatorioBD.IT_notificaciones_actividad.descripcion = recordatorio.IT_notificaciones_actividad.descripcion;
                    //cambio en notificacion 
                    db.IT_notificaciones_usuarios.RemoveRange(recordatorioBD.IT_notificaciones_actividad.IT_notificaciones_usuarios);
                    recordatorioBD.IT_notificaciones_actividad.IT_notificaciones_usuarios = notificacionesUsuarios;
                }

                //modifica los valores del checklist
                foreach (var ck in recordatorioBD.IT_notificaciones_checklist)
                    ck.estatus = recordatorio.IT_notificaciones_checklist.Where(x => x.id == ck.id).Select(x => x.estatus).FirstOrDefault();

                //determina si finaliza el recordatorio
                if (tipo_save == "finish")
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ha finalizado el evento correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                    recordatorioBD.estatus = Bitacoras.Util.IT_notificaciones_recordatorio_estatus.TERMINADO;
                }
                else
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Se guardaron los cambios correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                    recordatorioBD.estatus = Bitacoras.Util.IT_notificaciones_recordatorio_estatus.EN_PROCESO;
                }
                //db.Entry(recordatorio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var sistemas = obtieneEmpleadoLogeado();
            recordatorio.id_sistemas = sistemas.id;
            ViewBag.NombreEmpleado = sistemas.ConcatNumEmpleadoNombre;
            return View(recordatorio);
        }

        // GET: IT_notificaciones_actividad/EditRecurrentes/5
        public ActionResult EditRecurrentes(int? id)
        {
            if (!TieneRol(TipoRoles.IT_NOTIFICACIONES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IT_notificaciones_recordatorio recordatorio = db.IT_notificaciones_recordatorio.Find(id);
            if (recordatorio == null)
            {
                return HttpNotFound();
            }

            //inicializa fecha final de recurrentes
            recordatorio.IT_notificaciones_actividad.fecha_fin = recordatorio.IT_notificaciones_actividad.IT_notificaciones_recordatorio.OrderByDescending(x => x.fecha_programada).FirstOrDefault().fecha_programada;

            //inicializa actividades de checklist
            recordatorio.IT_notificaciones_actividad.IT_notificaciones_checklist_temp = recordatorio.IT_notificaciones_actividad.IT_notificaciones_recordatorio.OrderBy(x => x.fecha_programada).FirstOrDefault().IT_notificaciones_checklist;


            #region listado de empleados 
            //obtiene el listado de empleados
            //recorre los usuarios con el permiso de IT_notificaciones
            AspNetRoles rol = db.AspNetRoles.Where(x => x.Name == TipoRoles.IT_NOTIFICACIONES).FirstOrDefault();
            List<AspNetUsers> usuariosInRole = new List<AspNetUsers>();
            if (rol != null)
                usuariosInRole = rol.AspNetUsers.ToList();

            List<int> idsRol = usuariosInRole.Select(x => x.IdEmpleado).Distinct().ToList();
            List<empleados> listEmpleados = db.empleados.Where(x => x.activo == true && idsRol.Contains(x.id) == true).ToList();

            //obtiene el listado de empleados seleccionados en BD
            var empleadosBD = recordatorio.IT_notificaciones_actividad.IT_notificaciones_usuarios.Where(x => x.id_empleado.HasValue).Select(x => x.id_empleado).ToList();
            string[] selectedEmpleados = new string[empleadosBD.Count];
            for (int i = 0; i < empleadosBD.Count; i++)
            {
                selectedEmpleados[i] = string.Format("EMP_{0}", empleadosBD[i].ToString());
            }

            //agrega los empleados seleccionables
            recordatorio.IT_notificaciones_actividad.EmpleadosList = listEmpleados.Select(x => new SelectListItem()
            {
                // Selected = userRoles.Contains(x.Name),
                Selected = selectedEmpleados.Contains("EMP_" + x.id),
                Text = string.Format("{0} <{1}>", x.ConcatNombre, x.correo),
                Value = "EMP_" + x.id
            });

            //obtiene el listado de emails seleccionados en BD
            var emailsBD = recordatorio.IT_notificaciones_actividad.IT_notificaciones_usuarios.Where(x => !x.id_empleado.HasValue).Select(x => x.correo).ToList();
            string[] selectedEmails = new string[emailsBD.Count];
            for (int i = 0; i < emailsBD.Count; i++)
            {
                selectedEmails[i] = string.Format("C_{0}", emailsBD[i]);
            }

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
            recordatorio.IT_notificaciones_actividad.CorreosList = newListCorreos;

            #endregion

            var sistemas = obtieneEmpleadoLogeado();
            recordatorio.id_sistemas = sistemas.id;
            recordatorio.aplica_recordatorio = recordatorio.dias_previos_notificacion.HasValue;
            ViewBag.NombreEmpleado = sistemas.ConcatNumEmpleadoNombre;

            return View(recordatorio);
        }

        // POST: IT_notificaciones_actividad/EditRecurrentes/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRecurrentes(IT_notificaciones_recordatorio recordatorio, string tipo_save, string[] selectedEmails, params string[] selectedEmpleados)
        {
            selectedEmpleados = selectedEmpleados ?? new string[] { };
            selectedEmails = selectedEmails ?? new string[] { };

            if (selectedEmails.Length == 0 && selectedEmpleados.Length == 0)
                ModelState.AddModelError("", "No se seleccionó ningún usuario para mandar el recordatorio.");

            if (recordatorio.IT_notificaciones_actividad.fecha_fin < recordatorio.IT_notificaciones_actividad.fecha_inicio)
                ModelState.AddModelError("", "La fecha de fin no puede ser anterior a la fecha de inicio.");

            //ModelState.AddModelError("", "Error para depuración.");

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
            recordatorio.IT_notificaciones_actividad.IT_notificaciones_usuarios = notificacionesUsuarios;

            if (ModelState.IsValid)
            {
                //obtiene el recordatorio a modificar
                var recordatorioBD = db.IT_notificaciones_recordatorio.Find(recordatorio.id);

                #region agrega notificaciones intermedias

                //calcula las notificaciones intermedias
                DateTime fechaTemp = recordatorio.IT_notificaciones_actividad.fecha_inicio.Value;
                //lista para almacenar los recordatorios
                List<IT_notificaciones_recordatorio> iT_Notificaciones_RecordatoriosList = new List<IT_notificaciones_recordatorio>();
                //establece díasint?
                int? dias_p = null;
                if (recordatorio.aplica_recordatorio)
                    dias_p = recordatorio.dias_previos_notificacion;

                do
                {
                    //crea una copia del checklist temp, para evitar error de multiplicidad
                    List<IT_notificaciones_checklist> ckList = new List<IT_notificaciones_checklist>();
                    foreach (var notificacion in recordatorio.IT_notificaciones_actividad.IT_notificaciones_checklist_temp)
                    {
                        ckList.Add(new IT_notificaciones_checklist
                        {
                            descripcion = notificacion.descripcion,
                        });
                    }

                    //validar si no existe previamente
                    if (!recordatorioBD.IT_notificaciones_actividad.IT_notificaciones_recordatorio.Any(x => x.fecha_programada == fechaTemp))
                        iT_Notificaciones_RecordatoriosList.Add(new IT_notificaciones_recordatorio
                        {
                            fecha_programada = fechaTemp,
                            id_notificaciones_actividad = recordatorioBD.id_notificaciones_actividad,
                            IT_notificaciones_checklist = ckList,
                            dias_previos_notificacion = dias_p
                        });
                    //aumenta la fecha segun el periodo y el tipo

                    switch (recordatorioBD.IT_notificaciones_actividad.tipo_periodo)
                    {
                        case IT_notificaciones_tipo_periodo.DIAS:
                            fechaTemp = fechaTemp.AddDays(recordatorioBD.IT_notificaciones_actividad.periodo.Value);
                            break;
                        case IT_notificaciones_tipo_periodo.SEMANAS:
                            fechaTemp = fechaTemp.AddDays(recordatorioBD.IT_notificaciones_actividad.periodo.Value * 7);
                            break;
                        case IT_notificaciones_tipo_periodo.MESES:
                            fechaTemp = fechaTemp.AddMonths(recordatorioBD.IT_notificaciones_actividad.periodo.Value);
                            break;
                        case IT_notificaciones_tipo_periodo.ANIOS:
                            fechaTemp = fechaTemp.AddYears(recordatorioBD.IT_notificaciones_actividad.periodo.Value);
                            break;
                    }

                } while (fechaTemp <= recordatorio.IT_notificaciones_actividad.fecha_fin.Value);

                db.SaveChanges();
                #endregion

                //modifica los valores del recordatorio
                recordatorioBD.IT_notificaciones_actividad.titulo = recordatorio.IT_notificaciones_actividad.titulo;
                recordatorioBD.IT_notificaciones_actividad.descripcion = recordatorio.IT_notificaciones_actividad.descripcion;
                //cambio en notificacion 
                db.IT_notificaciones_usuarios.RemoveRange(recordatorioBD.IT_notificaciones_actividad.IT_notificaciones_usuarios);
                recordatorioBD.IT_notificaciones_actividad.IT_notificaciones_usuarios = notificacionesUsuarios;

                //Lista de actividades form
                List<string> actividadesFormulario = recordatorio.IT_notificaciones_actividad.IT_notificaciones_checklist_temp.Select(x => x.descripcion).ToList();

                //elimina los checklist de cada recordatorio
                foreach (var r in recordatorioBD.IT_notificaciones_actividad.IT_notificaciones_recordatorio)
                {
                    //establece días
                    if (recordatorio.aplica_recordatorio)
                        r.dias_previos_notificacion = recordatorio.dias_previos_notificacion;
                    else
                        r.dias_previos_notificacion = null;

                    //Lista de actividades bd
                    List<string> actividadesBD = r.IT_notificaciones_checklist.Select(x => x.descripcion).ToList();

                    //crea una copia del ck
                    List<IT_notificaciones_checklist> ckList = new List<IT_notificaciones_checklist>();
                    foreach (var notificacion in actividadesFormulario
                        .Where(x => !actividadesBD.Contains(x)))
                    {
                        ckList.Add(new IT_notificaciones_checklist
                        {
                            id_notificaciones_recordatorio = r.id,
                            descripcion = notificacion,
                        });
                    }

                    List<IT_notificaciones_checklist> borrarList = r.IT_notificaciones_checklist.Where(x => !actividadesFormulario.Contains(x.descripcion)).ToList();
                    //borra los check de cada recordatorio, excepto los que no hubo cambio
                    db.IT_notificaciones_checklist.RemoveRange(borrarList);
                    //agrega los nuevos
                    if (ckList.Count > 0)
                        db.IT_notificaciones_checklist.AddRange(ckList);

                }

                //agrega las notificaciones calculadas                
                db.IT_notificaciones_recordatorio.AddRange(iT_Notificaciones_RecordatoriosList);

                TempData["Mensaje"] = new MensajesSweetAlert("Se ha actualizado el evento correctamente." + (iT_Notificaciones_RecordatoriosList.Count > 0 ? " Se agregaron " + iT_Notificaciones_RecordatoriosList.Count + " eventos recurrentes." : String.Empty), TipoMensajesSweetAlerts.SUCCESS);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var sistemas = obtieneEmpleadoLogeado();
            recordatorio.id_sistemas = sistemas.id;
            ViewBag.NombreEmpleado = sistemas.ConcatNumEmpleadoNombre;
            return View(recordatorio);
        }

        // GET: IT_notificaciones_actividad/Cancelar/5
        public ActionResult Cancelar(int? id)
        {
            if (!TieneRol(TipoRoles.IT_NOTIFICACIONES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IT_notificaciones_recordatorio recordatorio = db.IT_notificaciones_recordatorio.Find(id);
            if (recordatorio == null)
            {
                return HttpNotFound();
            }

            try
            {
                db.IT_notificaciones_checklist.RemoveRange(recordatorio.IT_notificaciones_checklist);
                db.IT_notificaciones_recordatorio.Remove(recordatorio);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se canceló el evento correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }
            return RedirectToAction("Index");
        }
        // GET: IT_notificaciones_actividad/CancelarRecurrente/5
        public ActionResult CancelarRecurrente(int? id)
        {
            if (!TieneRol(TipoRoles.IT_NOTIFICACIONES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IT_notificaciones_recordatorio recordatorio = db.IT_notificaciones_recordatorio.Find(id);
            if (recordatorio == null)
            {
                return HttpNotFound();
            }

            try
            {
                DateTime fecha = DateTime.Now;

                var recordatorioList = db.IT_notificaciones_recordatorio.Where(x => x.id_notificaciones_actividad == recordatorio.id_notificaciones_actividad && x.fecha_programada >= fecha);
                foreach (var item in recordatorioList)
                {
                    db.IT_notificaciones_checklist.RemoveRange(item.IT_notificaciones_checklist);
                    db.IT_notificaciones_recordatorio.Remove(item);
                }

                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert("Se cancelaron los eventos recurrentes correctamente.", TipoMensajesSweetAlerts.SUCCESS);
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
            }
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
        ///Obtiene el total de los eventos
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult GetEventos(string estatus)
        {

            var recordatoriosList = db.IT_notificaciones_recordatorio
                .Where(x => string.IsNullOrEmpty(estatus)
                    || (estatus == IT_notificaciones_recordatorio_estatus.PENDIENTE
                        && (x.estatus == null || x.estatus == IT_notificaciones_recordatorio_estatus.EN_PROCESO || x.estatus == IT_notificaciones_recordatorio_estatus.PENDIENTE))
                    || (estatus == IT_notificaciones_recordatorio_estatus.TERMINADO
                        && x.estatus == IT_notificaciones_recordatorio_estatus.TERMINADO )
                )
                .ToList();

            //inicializa la lista de objetos
            var objeto = new object[recordatoriosList.Count];
            var fechaHoy = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            for (int i = 0; i < recordatoriosList.Count; i++)
            {
                string color = "#008DFF";
                if (recordatoriosList[i].fecha_programada < fechaHoy)
                    color = "#7BC7F7";

                var icon = recordatoriosList[i].estatus == Bitacoras.Util.IT_notificaciones_recordatorio_estatus.TERMINADO ?
                           "fa-regular fa-check-circle" : "fa-regular fa-circle-xmark";

                var icon_color = "white";
                //define el color del icono
                if (recordatoriosList[i].estatus == Bitacoras.Util.IT_notificaciones_recordatorio_estatus.TERMINADO)
                    if (recordatoriosList[i].fecha_programada < fechaHoy)
                        icon_color = "green";
                    else
                        icon_color = "#4fff4d";
                else //si no se termino
                     if (recordatoriosList[i].fecha_programada < fechaHoy)
                    icon_color = "#BF2D2D";
                else
                    icon_color = "#ff8080";


                objeto[i] = new
                {
                    id = recordatoriosList[i].id.ToString(),
                    icon = icon,
                    title = recordatoriosList[i].IT_notificaciones_actividad.titulo,
                    start = recordatoriosList[i].fecha_programada.ToString("yyyy-MM-dd"),
                    color = color,
                    textColor = "white", // an option!
                    icon_color = icon_color,
                    estatus = recordatoriosList[i].estatus,
                };
            }

            return Json(objeto, JsonRequestBehavior.AllowGet);
        }
        ///<summary>
        ///Obtiene el detalle de un evento en específico
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        [AllowAnonymous]
        public JsonResult GetDetallesEvento(int id)
        {
            string estatus = "ERROR";

            var recordatorio = db.IT_notificaciones_recordatorio.Find(id);

            //actualiza el estatus según si se encontro
            estatus = recordatorio != null ? "OK" : "ERROR";

            //actualiza el estado de las actividades
            string actividades = string.Empty;
            actividades = @"<ol>";
            foreach (var a in recordatorio.IT_notificaciones_checklist)
            {
                if (a.estatus == IT_notificaciones_checklist_estatus.TERMINADO)
                    actividades += @"<li><i class=""fa-sharp fa-solid fa-check fa-beat-fade"" style=""color: #189a3f;""></i> " + a.descripcion + @" </li>";
                else if (string.IsNullOrEmpty(a.estatus) || a.estatus == IT_notificaciones_checklist_estatus.PENDIENTE)
                    actividades += @"<li><i class=""fa-sharp fa-solid fa-xmark fa-beat-fade"" style=""color: #f50000;""></i> " + a.descripcion + @" </li>";
            }
            actividades += "</ol>";


            //inicializa la lista de objetos
            var objeto = new object[1];
            objeto[0] = new
            {
                mensaje = "Se obtuvo la información correctamente",
                titulo = recordatorio.IT_notificaciones_actividad.titulo,
                descripcion = recordatorio.IT_notificaciones_actividad.descripcion,
                fecha = recordatorio.fecha_programada.ToString("yyyy/MM/dd"),
                es_recurrente = recordatorio.IT_notificaciones_actividad.es_recurrente ? "Sí" : "No",
                estatus = Bitacoras.Util.IT_notificaciones_recordatorio_estatus.DescripcionStatus(recordatorio.estatus),
                actividades = actividades,
            };

            return Json(objeto, JsonRequestBehavior.AllowGet);
        }
    }
}
