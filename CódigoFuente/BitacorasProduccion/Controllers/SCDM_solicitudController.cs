using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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
    public class SCDM_solicitudController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: SCDM_solicitud
        public ActionResult Index(int pagina = 1)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) && !TieneRol(TipoRoles.SCDM_MM_APROBACION_SOLICITUDES) &&
                !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES) && !TieneRol(TipoRoles.SCDM_MM_REPORTES))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            var cantidadRegistrosPorPagina = 20; // parámetro

            var listado = db.SCDM_solicitud
                    // .Where(x => x.id_elaborador == solicitante.id && x.estatus == PM_Status.CREADO)
                    .OrderByDescending(x => x.fecha_creacion)
                    .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                   .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.SCDM_solicitud
                //.Where(x => x.id_elaborador == solicitante.id && x.estatus == PM_Status.CREADO)
                .Count();

            //para paginación
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            //routeValues["material"] = material;

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

        // GET: SCDM_solicitud/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }
            return View(sCDM_solicitud);
        }

        // GET: SCDM_solicitud/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
                !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");


            var solicitante = obtieneEmpleadoLogeado();

            SCDM_solicitud solicitud = new SCDM_solicitud
            {
                id_solicitante = solicitante.id,
                empleados = solicitante,
                fecha_creacion = DateTime.Now,
                activo = true,
                on_hold = false,
            };

            ViewBag.listPlantas = db.plantas.Where(x => x.aplica_solicitud_scdm == true).ToList();
            ViewBag.listTipoMateriales = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList();
            ViewBag.id_prioridad = AddFirstItem(new SelectList(db.SCDM_cat_prioridad.Where(x => x.activo == true), nameof(SCDM_cat_prioridad.id), nameof(SCDM_cat_prioridad.descripcion)));
            ViewBag.id_tipo_cambio = AddFirstItem(new SelectList(db.SCDM_cat_tipo_cambio.Where(x => x.activo == true), nameof(SCDM_cat_tipo_cambio.id), nameof(SCDM_cat_tipo_cambio.descripcion)));
            ViewBag.id_tipo_solicitud = AddFirstItem(new SelectList(db.SCDM_cat_tipo_solicitud.Where(x => x.activo == true), nameof(SCDM_cat_tipo_solicitud.id), nameof(SCDM_cat_tipo_solicitud.descripcion)));
            return View(solicitud);
        }

        // POST: SCDM_solicitud/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SCDM_solicitud sCDM_solicitud, FormCollection collection, string[] SelectedMateriales, string[] SelectedPlantas)
        {
            var solicitante = obtieneEmpleadoLogeado();

            #region asignacion_listas

            //lista de key del collection
            List<string> keysCollection = collection.AllKeys.ToList();

            //crea los objetos para Materiales
            if (SelectedMateriales != null && (sCDM_solicitud.id_tipo_solicitud == 1 || sCDM_solicitud.id_tipo_solicitud == 2 || sCDM_solicitud.id_tipo_solicitud == 5))
                foreach (string material_string in SelectedMateriales)
                {
                    //obtiene el id
                    Match m = Regex.Match(material_string, @"\d+");
                    int id_material_int = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_material_int);

                    sCDM_solicitud.SCDM_rel_solicitud_materiales_solicitados.Add(new SCDM_rel_solicitud_materiales_solicitados { id_tipo_material = id_material_int });
                }

            if (SelectedPlantas != null)
                foreach (string planta_string in SelectedPlantas)
                {
                    //obtiene el id
                    Match m = Regex.Match(planta_string, @"\d+");
                    int id_planta_int = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_planta_int);

                    sCDM_solicitud.SCDM_rel_solicitud_plantas.Add(new SCDM_rel_solicitud_plantas { id_planta = id_planta_int });
                }

            #endregion

            #region validacion de archivos
            List<HttpPostedFileBase> archivosForm = new List<HttpPostedFileBase>();
            //agrega archivos enviados
            if (sCDM_solicitud.PostedFileSolicitud_1 != null)
                archivosForm.Add(sCDM_solicitud.PostedFileSolicitud_1);
            if (sCDM_solicitud.PostedFileSolicitud_2 != null)
                archivosForm.Add(sCDM_solicitud.PostedFileSolicitud_2);
            if (sCDM_solicitud.PostedFileSolicitud_3 != null)
                archivosForm.Add(sCDM_solicitud.PostedFileSolicitud_3);

            #region lectura de archivos

            foreach (HttpPostedFileBase httpPostedFileBase in archivosForm)
            {
                //verifica si el tamaño del archivo es OT 1
                if (httpPostedFileBase != null && httpPostedFileBase.InputStream.Length > 10485760)
                    ModelState.AddModelError("", "Sólo se permiten archivos menores a 10MB: " + httpPostedFileBase.FileName + ".");
                else if (httpPostedFileBase != null)
                { //verifica la extensión del archivo
                    string extension = Path.GetExtension(httpPostedFileBase.FileName);
                    if (extension.ToUpper() == ".EXE"   //si contiene una extensión inválida
                                   )
                        ModelState.AddModelError("", "No se permiten archivos con extensión " + extension + ": " + httpPostedFileBase.FileName + ".");
                    else
                    { //si la extension y el tamaño son válidos
                        String nombreArchivo = httpPostedFileBase.FileName;

                        //recorta el nombre del archivo en caso de ser necesario
                        if (nombreArchivo.Length > 80)
                            nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                        //lee el archivo en un arreglo de bytes
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(httpPostedFileBase.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(httpPostedFileBase.ContentLength);
                        }

                        //genera el archivo de biblioce digital
                        biblioteca_digital archivo = new biblioteca_digital
                        {
                            Nombre = nombreArchivo,
                            MimeType = UsoStrings.RecortaString(httpPostedFileBase.ContentType, 80),
                            Datos = fileData
                        };
                        sCDM_solicitud.SCDM_rel_solicitud_archivos.Add(new SCDM_rel_solicitud_archivos { biblioteca_digital = archivo });
                    }
                }
            }


            #endregion


            #endregion
            //validaciones de las listas

            if (SelectedMateriales == null && (sCDM_solicitud.id_tipo_solicitud == 1 || sCDM_solicitud.id_tipo_solicitud == 2 || sCDM_solicitud.id_tipo_solicitud == 5))
                ModelState.AddModelError("", "Seleccione los materiales deseados para la solicitud.");

            if (SelectedPlantas == null)
                ModelState.AddModelError("", "Seleccione las plantas para las cuales aplica la solicitud.");

            //si no es cambios
            if (sCDM_solicitud.id_tipo_solicitud != 3)
                sCDM_solicitud.id_tipo_cambio = null;

            //ModelState.AddModelError("", "Error para depuración.");

            if (ModelState.IsValid)
            {
                sCDM_solicitud.fecha_creacion = DateTime.Now;

                db.SCDM_solicitud.Add(sCDM_solicitud);
                try
                {
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ha comenzado la solicitud correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("EditarSolicitud", new { id = sCDM_solicitud.id });
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);
                   
                }
                return RedirectToAction("Index");
            }

            sCDM_solicitud.fecha_creacion = DateTime.Now;
            sCDM_solicitud.empleados = solicitante;

            ViewBag.listPlantas = db.plantas.Where(x => x.aplica_solicitud_scdm == true).ToList();
            ViewBag.listTipoMateriales = db.SCDM_cat_tipo_materiales_solicitud.Where(x => x.activo == true).ToList();

            ViewBag.id_prioridad = AddFirstItem(new SelectList(db.SCDM_cat_prioridad.Where(x => x.activo == true), nameof(SCDM_cat_prioridad.id), nameof(SCDM_cat_prioridad.descripcion)));
            ViewBag.id_tipo_cambio = AddFirstItem(new SelectList(db.SCDM_cat_tipo_cambio.Where(x => x.activo == true), nameof(SCDM_cat_tipo_cambio.id), nameof(SCDM_cat_tipo_cambio.descripcion)));
            ViewBag.id_tipo_solicitud = AddFirstItem(new SelectList(db.SCDM_cat_tipo_solicitud.Where(x => x.activo == true), nameof(SCDM_cat_tipo_solicitud.id), nameof(SCDM_cat_tipo_solicitud.descripcion)));
            return View(sCDM_solicitud);
        }

        // GET: SCDM_solicitud/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", sCDM_solicitud.id_solicitante);
            ViewBag.id_prioridad = new SelectList(db.SCDM_cat_prioridad, "id", "descripcion", sCDM_solicitud.id_prioridad);
            ViewBag.id_tipo_cambio = new SelectList(db.SCDM_cat_tipo_cambio, "id", "descripcion", sCDM_solicitud.id_tipo_cambio);
            ViewBag.id_tipo_solicitud = new SelectList(db.SCDM_cat_tipo_solicitud, "id", "descripcion", sCDM_solicitud.id_tipo_solicitud);
            return View(sCDM_solicitud);
        }

        // POST: SCDM_solicitud/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SCDM_solicitud sCDM_solicitud)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sCDM_solicitud).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_solicitante = new SelectList(db.empleados, "id", "numeroEmpleado", sCDM_solicitud.id_solicitante);
            ViewBag.id_prioridad = new SelectList(db.SCDM_cat_prioridad, "id", "descripcion", sCDM_solicitud.id_prioridad);
            ViewBag.id_tipo_cambio = new SelectList(db.SCDM_cat_tipo_cambio, "id", "descripcion", sCDM_solicitud.id_tipo_cambio);
            ViewBag.id_tipo_solicitud = new SelectList(db.SCDM_cat_tipo_solicitud, "id", "descripcion", sCDM_solicitud.id_tipo_solicitud);
            return View(sCDM_solicitud);
        }

        // GET: SCDM_solicitud/Edit/5
        public ActionResult EditarSolicitud(int? id)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR) &&
                !TieneRol(TipoRoles.SCDM_MM_CREACION_SOLICITUDES))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
                ViewBag.MensajeAlert = TempData["Mensaje"];

            ViewBag.secciones = AddFirstItem(new SelectList(db.SCDM_cat_secciones.Where(x => x.activo == true && x.aplica_solicitud == true), nameof(SCDM_cat_secciones.id), nameof(SCDM_cat_secciones.descripcion)));

            return View(sCDM_solicitud);
        }

        // POST: SCDM_solicitud/AddSeccion/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSeccion(int? id, int? secciones)
        {
            if (ModelState.IsValid)
            {
                var rel = new SCDM_rel_solicitud_secciones_activas()
                {
                    id_solicitud = id.Value,
                    id_seccion = secciones.Value,
                };


                try
                {

                    if (db.SCDM_rel_solicitud_secciones_activas.Any(x => x.id_solicitud == id && x.id_seccion == secciones))
                    {
                        TempData["Mensaje"] = new MensajesSweetAlert("La sección ya se encuentra agregada a la solicitud.", TipoMensajesSweetAlerts.WARNING);
                    }
                    else {
                        db.SCDM_rel_solicitud_secciones_activas.Add(rel);
                        db.SaveChanges();
                        TempData["Mensaje"] = new MensajesSweetAlert("Se ha agregado la sección correctamente.", TipoMensajesSweetAlerts.SUCCESS);
                    }     
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ocurrido un error al guardar en Base de Datos.", TipoMensajesSweetAlerts.ERROR);

                }
            }
         
            return RedirectToAction("EditarSolicitud", new { id = id});
        }

        // GET: SCDM_solicitud/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            if (sCDM_solicitud == null)
            {
                return HttpNotFound();
            }
            return View(sCDM_solicitud);
        }

        // POST: SCDM_solicitud/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SCDM_solicitud sCDM_solicitud = db.SCDM_solicitud.Find(id);
            db.SCDM_solicitud.Remove(sCDM_solicitud);
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
    }
}
