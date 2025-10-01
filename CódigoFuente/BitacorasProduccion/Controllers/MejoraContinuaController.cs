using Bitacoras.Util;
using Clases.Util;
using iText.IO.Exceptions;
using Org.BouncyCastle.Crypto;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static System.Collections.Specialized.NameObjectCollectionBase;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class MejoraContinuaController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: MejoraContinua
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Create()
        {

            IM_Idea_mejora idea = new IM_Idea_mejora
            {
                captura = DateTime.Now,
            };


            //crea un Select  list para el estatus
            List<SelectListItem> newListTipoIdea = new List<SelectListItem>
            {
                //Agrega el estatus al selectListItem
                new SelectListItem() { Text = "Idea de Mejora", Value = "Idea de Mejora" },
                new SelectListItem() { Text = "Idea de Innovación", Value = "Idea de Innovación" }
            };


            ViewBag.ClasificacionImpactoList = db.IM_cat_impacto.Where(x => x.activo).ToList();
            ViewBag.DesperdicioList = db.IM_cat_desperdicio.Where(x => x.activo).ToList();

            List<empleados> listadoEmpleados = db.empleados.Where(p => p.activo == true).ToList();
            ViewBag.ListadoEmpleados = listadoEmpleados;

            SelectList selectListItemTipoIdea = new SelectList(newListTipoIdea, "Value", "Text");
            ViewBag.tipo_idea = AddFirstItem(selectListItemTipoIdea, textoPorDefecto: "-- Seleccionar --");

            ViewBag.clave_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo == true), nameof(plantas.clave), nameof(plantas.descripcion)),
                    textoPorDefecto: "-- Seleccione un valor --");


            return View(idea);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IM_Idea_mejora iM_Idea_mejora, FormCollection collection, string[] SelectedImpacto, string[] SelectedDesperdicio)
        {

            empleados empleadoLogeado = obtieneEmpleadoLogeado();

            #region validaformulario

            //crea los objetos para Impacto
            if (SelectedImpacto != null)
                foreach (string impacto in SelectedImpacto)
                {
                    //obtiene el id
                    Match m = Regex.Match(impacto, @"\d+");
                    int id_impacto = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_impacto);

                    iM_Idea_mejora.IM_rel_impacto.Add(new IM_rel_impacto { catalogoIdeaMejoraImpactoClave = id_impacto });
                }
            //crea los objetos para desperdicio
            if (SelectedDesperdicio != null)
                foreach (string despedicio in SelectedDesperdicio)
                {
                    //obtiene el id
                    Match m = Regex.Match(despedicio, @"\d+");
                    int id_desperdicio = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_desperdicio);

                    iM_Idea_mejora.IM_rel_reduccion_desperdicio.Add(new IM_rel_reduccion_desperdicio { catalogoIdeaMejoraDesperdicioClave = id_desperdicio });
                }

            //valida si la lista de proponentes está vacia
            if (iM_Idea_mejora.IM_rel_proponente == null || iM_Idea_mejora.IM_rel_proponente.Count == 0)
                ModelState.AddModelError("", "No se indicaron los proponentes de la idea de mejora.");

            if (iM_Idea_mejora.IM_rel_proponente == null || iM_Idea_mejora.IM_rel_proponente.Count > 5)
                ModelState.AddModelError("", "La idea de mejora, no puede tener más de cinco proponentes.");

            //valida si hay proponentes repetidos
            if (iM_Idea_mejora.IM_rel_proponente != null)
            {
                List<int> listProponentes = iM_Idea_mejora.IM_rel_proponente.Select(x => x.id_empleado).ToList();

                if (listProponentes.Count != listProponentes.Distinct().Count())
                {
                    ModelState.AddModelError("", "Existen nombres repetidos en la lista de proponentes.");
                }
            }


            #region validacion de archivos
            List<HttpPostedFileBase> archivosForm = new List<HttpPostedFileBase>();
            //agrega archivos enviados
            if (iM_Idea_mejora.PostedFile1 != null)
                archivosForm.Add(iM_Idea_mejora.PostedFile1);
            if (iM_Idea_mejora.PostedFile2 != null)
                archivosForm.Add(iM_Idea_mejora.PostedFile2);
            if (iM_Idea_mejora.PostedFile3 != null)
                archivosForm.Add(iM_Idea_mejora.PostedFile3);
            if (iM_Idea_mejora.PostedFile4 != null)
                archivosForm.Add(iM_Idea_mejora.PostedFile4);

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
                            Nombre = Regex.Replace(nombreArchivo, @"[^\w\s.]+", ""),
                            MimeType = UsoStrings.RecortaString(httpPostedFileBase.ContentType, 80),
                            Datos = fileData
                        };
                        iM_Idea_mejora.IM_rel_archivos.Add(new IM_rel_archivos { biblioteca_digital = archivo });
                    }
                }
            }

           


            #endregion

            #endregion
            #endregion

            if (ModelState.IsValid)
            {

                try
                {
                    //agrega un valor de numEmpleados para proponentes
                    foreach (var item in iM_Idea_mejora.IM_rel_proponente)
                        item.numeroEmpleado = string.Empty;

                    //cambia la fecha
                    iM_Idea_mejora.captura = DateTime.Now;
                    //agrega el estatus creada
                    iM_Idea_mejora.IM_rel_estatus.Add(
                        new IM_rel_estatus
                        {
                            catalogoIdeaMejoraEstatusClave = Bitacoras.Util.IM_EstatusConstantes.CREADA,
                            captura = DateTime.Now,
                            id_empleado = empleadoLogeado.id != 0 ? empleadoLogeado.id : (int?)null
                        }
                       );


                    db.IM_Idea_mejora.Add(iM_Idea_mejora);
                    db.SaveChanges();

                    TempData["Mensaje"] = new MensajesSweetAlert($"Tu idea de mejora ha sido registrada con el Folio: {iM_Idea_mejora.ConcatFolio}.", TipoMensajesSweetAlerts.SUCCESS);

                    // AGREGAR CORREO ELECTRONICO
                    var ideaBD = db.IM_Idea_mejora.Find(iM_Idea_mejora.id);

                    var planta = db.plantas.Find(iM_Idea_mejora.clave_planta);
                    string plantaText = planta != null ? planta.descripcion : string.Empty;

                    //envia correo electronico
                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                    List<String> correos = new List<string>(); //correos TO

                    //-- INICIO POR TABLA NOTIFICACION PROPONENTES
                    foreach (var proponente in iM_Idea_mejora.IM_rel_proponente)
                    {
                        //encuentra el empleado
                        var empleado = db.empleados.Find(proponente.id_empleado);
                        //si el campo correo no está vacio
                        if (empleado != null && !String.IsNullOrEmpty(empleado.correo))
                            correos.Add(empleado.correo);
                    }
                    envioCorreo.SendEmailAsync(correos, $"Tu idea de mejora ha sido registrada --> Folio: {iM_Idea_mejora.ConcatFolio}", envioCorreo.getBodyNuevaIdeaMejoraToProponentes(ideaBD));
                    //-- FIN POR TABLA NOTIFICACION PROPNENTES

                    correos = new List<string>();

                    //-- INICIO POR TABLA NOTIFICACION EVALUADORES
                    foreach (var evaluador in db.IM_administradores.Where(x => x.id_planta == iM_Idea_mejora.clave_planta && x.recibe_correo))
                    {
                        //encuentra el empleado
                        var empleado = db.empleados.Find(evaluador.id_empleado);
                        //si el campo correo no está vacio
                        if (empleado != null && !String.IsNullOrEmpty(empleado.correo))
                            correos.Add(empleado.correo);

                    }

                    envioCorreo.SendEmailAsync(correos, $"🔔 Mejora Continua. Nueva idea de mejora --> Folio: {iM_Idea_mejora.ConcatFolio}", envioCorreo.getBodyNuevaIdeaMejora(ideaBD));
                    //-- FIN POR TABLA NOTIFICACION EVALUADORES



                    return RedirectToAction("ListadoIdeas");
                }
                catch (Exception ex)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert(ex.Message, TipoMensajesSweetAlerts.WARNING);
                }


            }

            //crea un Select  list para el estatus
            List<SelectListItem> newListTipoIdea = new List<SelectListItem>
            {
                //Agrega el estatus al selectListItem
                new SelectListItem() { Text = "Idea de Mejora", Value = "Idea de Mejora" },
                new SelectListItem() { Text = "Idea de Innovación", Value = "Idea de Innovación" }
            };

            ViewBag.ArchivosPrevios = archivosForm.Any();
            ViewBag.ClasificacionImpactoList = db.IM_cat_impacto.Where(x => x.activo).ToList();
            ViewBag.DesperdicioList = db.IM_cat_desperdicio.Where(x => x.activo).ToList();

            List<empleados> listadoEmpleados = db.empleados.Where(p => p.activo == true).ToList();
            ViewBag.ListadoEmpleados = listadoEmpleados;

            SelectList selectListItemTipoIdea = new SelectList(newListTipoIdea, "Value", "Text");
            ViewBag.tipo_idea = AddFirstItem(selectListItemTipoIdea, textoPorDefecto: "-- Seleccionar --", selected: iM_Idea_mejora.tipo_idea);

            ViewBag.clave_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo == true), nameof(plantas.clave), nameof(plantas.descripcion)),
                   textoPorDefecto: "-- Seleccione un valor --", selected: iM_Idea_mejora.clave_planta.ToString());

            return View(iM_Idea_mejora);

        }

        public ActionResult Evaluar(int? id)
        {

            if (!TieneRol(TipoRoles.IM_ADMIN))
                return View("../Home/ErrorPermisos");

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            var idea = db.IM_Idea_mejora.Find(id);
            if (idea == null)
            {
                return View("../Error/NotFound");
            }


            //crea un Select  list para el estatus
            List<SelectListItem> newListTipoIdea = new List<SelectListItem>
            {
                //Agrega el estatus al selectListItem
                new SelectListItem() { Text = "Idea de Mejora", Value = "Idea de Mejora" },
                new SelectListItem() { Text = "Idea de Innovación", Value = "Idea de Innovación" }
            };


            ViewBag.ClasificacionImpactoList = db.IM_cat_impacto.Where(x => x.activo).ToList();
            ViewBag.DesperdicioList = db.IM_cat_desperdicio.Where(x => x.activo).ToList();

            List<empleados> listadoEmpleados = db.empleados.Where(p => p.activo == true).ToList();
            ViewBag.ListadoEmpleados = listadoEmpleados;

            SelectList selectListItemTipoIdea = new SelectList(newListTipoIdea, "Value", "Text");
            ViewBag.tipo_idea = AddFirstItem(selectListItemTipoIdea, textoPorDefecto: "-- Seleccionar --");

            ViewBag.id_planta_implementacion = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo == true), nameof(plantas.clave), nameof(plantas.descripcion)),
                    textoPorDefecto: "-- Seleccione un valor --", selected: idea.IM_cat_area != null ? idea.IM_cat_area.id_planta.ToString() : string.Empty);

            //si existe area
            if (idea.IM_cat_area != null)
                ViewBag.areaImplementacionClave = AddFirstItem(new SelectList(db.IM_cat_area.Where(x => x.activo == true && idea.areaImplementacionClave.HasValue && x.id_planta == idea.IM_cat_area.id_planta), nameof(IM_cat_area.id), nameof(IM_cat_area.descripcion)),
                        textoPorDefecto: "-- Seleccione un valor --", selected: idea.areaImplementacionClave.ToString());
            else //si no existe area
                ViewBag.areaImplementacionClave = AddFirstItem(new SelectList(db.IM_cat_area.Where(x => x.id_planta == 999), nameof(IM_cat_area.id), nameof(IM_cat_area.descripcion)),
                        textoPorDefecto: "-- Seleccione un valor --", selected: idea.areaImplementacionClave.ToString());

            ViewBag.nivelImpactoClave = AddFirstItem(new SelectList(db.IM_cat_nivel_impacto.Where(x => x.activo), nameof(IM_cat_nivel_impacto.id), nameof(IM_cat_nivel_impacto.descripcion)), selected: idea.nivelImpactoClave.ToString());
            ViewBag.reconocimentoClave = AddFirstItem(new SelectList(db.IM_cat_reconocimiento.Where(x => x.activo), nameof(IM_cat_reconocimiento.id), nameof(IM_cat_reconocimiento.descripcion)), selected: idea.reconocimentoClave.ToString());

            //obtiene el último estatus del la idea
            var estatus = idea.IM_rel_estatus.LastOrDefault();
            int id_estatus = 0;

            if (estatus != null)
                id_estatus = estatus.catalogoIdeaMejoraEstatusClave;
            ViewBag.evaluacion_inicial = id_estatus;

            //determina si la fecha de implementacion es valida
            if (idea.implementacionFecha.HasValue && idea.implementacionFecha.Value.Year < 2000)
                idea.implementacionFecha = null;

            //determina si esta en proceso de implementacion
            if (idea.enProcesoImplementacion.HasValue && idea.enProcesoImplementacion.Value == 1)
                idea.enProcesoBool = true;

            //determina si la idea en equipo
            if (idea.IM_rel_proponente.Count() > 1)
                idea.ideaEnEquipoBool = true;


            return View(idea);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Evaluar(IM_Idea_mejora iM_Idea_mejora, int? evaluacion_inicial, FormCollection collection)
        {
            if (iM_Idea_mejora == null || iM_Idea_mejora.id == 0)
            {
                return View("../Error/BadRequest");
            }
            var ideaBD = db.IM_Idea_mejora.Find(iM_Idea_mejora.id);
            if (ideaBD == null)
            {
                return View("../Error/NotFound");
            }

            //obtiene el usuario logeado
            empleados empleadoLogeado = obtieneEmpleadoLogeado();

            //valida si hay implementadores repetidos
            if (iM_Idea_mejora.IM_rel_implementador.Count != iM_Idea_mejora.IM_rel_implementador.Select(x => x.id_empleado).Distinct().Count())
                ModelState.AddModelError("", "Existen Responsables repetidos.");

            // ModelState.AddModelError("", "Error de prueba");

            if (ModelState.IsValid)
            {
                DateTime fechaActual = DateTime.Now;
                //Variables para saber si una sección está habilitada
                bool EvaluacionComiteEnabled = evaluacion_inicial == 1 ? true : false;
                bool ImpactoClasificacionEnabled = iM_Idea_mejora.comiteAceptada == 1 && EvaluacionComiteEnabled ? true : false;
                bool ImplementacionEnabled = iM_Idea_mejora.enProcesoBool && ImpactoClasificacionEnabled;
                bool FechaImplementacionEnabled = iM_Idea_mejora.ideaImplementada == 1 && ImplementacionEnabled ? true : false;

                #region Agrega proximo estatus
                //Obtiene el estatus anterior
                int estatusAnterior = ideaBD.IM_rel_estatus.Any() ? ideaBD.IM_rel_estatus.LastOrDefault().catalogoIdeaMejoraEstatusClave : 0;
                //variable para el próximo estatus
                int proximoEstatus = estatusAnterior;
                //1. determinar el nuevo estatus
                switch (evaluacion_inicial)
                {
                    case 1: //recibida
                        proximoEstatus = IM_EstatusConstantes.RECIBIDA;
                        //determina si es el estatus aceptada
                        proximoEstatus = iM_Idea_mejora.comiteAceptada == 1 && EvaluacionComiteEnabled ? IM_EstatusConstantes.ACEPTADA_POR_COMITE : proximoEstatus;
                        proximoEstatus = iM_Idea_mejora.comiteAceptada == 0 && EvaluacionComiteEnabled ? IM_EstatusConstantes.RECHAZADA : proximoEstatus;
                        //determina si la idea esta en proceso
                        proximoEstatus = iM_Idea_mejora.enProcesoBool && ImpactoClasificacionEnabled ? IM_EstatusConstantes.EN_PROCESO_IMPLEMENTACION : proximoEstatus;
                        //finalizada/implementada
                        proximoEstatus = iM_Idea_mejora.ideaImplementada == 1 && ImplementacionEnabled ? IM_EstatusConstantes.IMPLEMENTADA : proximoEstatus;
                        proximoEstatus = iM_Idea_mejora.ideaImplementada == 0 && ImplementacionEnabled ? IM_EstatusConstantes.FINALIZADA_SIN_IMPLEMENTAR : proximoEstatus;

                        break;
                    case 2: //falta info
                        proximoEstatus = IM_EstatusConstantes.FALTA_INFORMACION;
                        break;
                    case 3: //No aceptada
                        proximoEstatus = IM_EstatusConstantes.NO_ACEPTADA;
                        break;
                }
                //guarda en Base de Datos en Caso de que el estatus sea diferente
                if (estatusAnterior != proximoEstatus)
                {
                    db.IM_rel_estatus.Add(
                       new IM_rel_estatus
                       {
                           catalogoIdeaMejoraEstatusClave = proximoEstatus,
                           captura = fechaActual,
                           ideaMejoraClave = iM_Idea_mejora.id,
                           id_empleado = empleadoLogeado.id
                       }
                      );

                    //*** ENVIAR CORREO A INVOLUCRADOS ****///
                    //envia correo electronico
                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                    List<String> correos = new List<string>(); //correos TO
                    List<String> correosTO = new List<string>(); //correos TO

                    //-- INICIO POR TABLA NOTIFICACION PROPONENTES
                    foreach (var proponente in ideaBD.IM_rel_proponente)
                    {
                        //encuentra el empleado
                        var empleado = db.empleados.Find(proponente.id_empleado);
                        //si el campo correo no está vacio
                        if (empleado != null && !String.IsNullOrEmpty(empleado.correo))
                            correos.Add(empleado.correo);
                    }

                    //-- INICIO POR TABLA NOTIFICACION EVALUADORES
                    //foreach (var evaluador in db.IM_administradores.Where(x => x.id_planta == ideaBD.clave_planta && x.recibe_correo))
                    //{
                    //    //encuentra el empleado
                    //    var empleado = db.empleados.Find(evaluador.id_empleado);
                    //    //si el campo correo no está vacio
                    //    if (empleado != null && !String.IsNullOrEmpty(empleado.correo))
                    //        correosTO.Add(empleado.correo);

                    //}

                    envioCorreo.SendEmailAsync(correos, $"🔔 Mejora Continua. Cambio de Estatus en tu idea de mejora --> Folio: {iM_Idea_mejora.ConcatFolio}", envioCorreo.getBodyNuevaIdeaMejoraCambioEstatus(ideaBD, proximoEstatus, iM_Idea_mejora.comentario));

                }
                #endregion

                #region Agrega Comentario
                if (!string.IsNullOrEmpty(iM_Idea_mejora.comentario))
                {
                    db.IM_rel_comentario.Add(
                        new IM_rel_comentario
                        {
                            ideaMejoraClave = iM_Idea_mejora.id,
                            captura = fechaActual,
                            numeroEmpleado = empleadoLogeado.numeroEmpleado,
                            id_empleado = empleadoLogeado.id,
                            texto = iM_Idea_mejora.comentario
                        }
                   );
                }
                #endregion

                #region Actualiza Datos
                //actualiza los datos de la Idea según las areas habilitadas
                if (EvaluacionComiteEnabled)
                {
                    ideaBD.comiteAceptada = iM_Idea_mejora.comiteAceptada == null ? 2 : iM_Idea_mejora.comiteAceptada; //0 = rechazada; 1 = Aceptada; 2 = vacio;
                }
                if (ImpactoClasificacionEnabled)
                {
                    ideaBD.clasificacionClave = iM_Idea_mejora.clasificacionClave; //2 = sencilla; 3 = mayor; null = vacio;
                    ideaBD.nivelImpactoClave = iM_Idea_mejora.nivelImpactoClave;
                    ideaBD.ideaEnEquipo = iM_Idea_mejora.ideaEnEquipoBool ? (byte)1 : (byte)0;
                    ideaBD.enProcesoImplementacion = iM_Idea_mejora.enProcesoBool ? (byte)1 : (byte)0;
                }
                if (ImplementacionEnabled)
                {
                    ideaBD.areaImplementacionClave = iM_Idea_mejora.areaImplementacionClave; //2 = sencilla; 3 = mayor; null = vacio;

                    //borra y agrega los nuevos implementadores
                    var implementadoresBD = ideaBD.IM_rel_implementador;
                    //borra los implementadores anteriores
                    db.IM_rel_implementador.RemoveRange(implementadoresBD);
                    foreach (var implementador in iM_Idea_mejora.IM_rel_implementador)
                    {
                        //busca al empledo
                        var implementadorEmpleadoBD = db.empleados.Find(implementador.id_empleado);
                        //agrega los nuevos
                        db.IM_rel_implementador.Add(
                            new IM_rel_implementador
                            {
                                ideaMejoraClave = iM_Idea_mejora.id,
                                numeroEmpleado = implementadorEmpleadoBD != null ? implementadorEmpleadoBD.numeroEmpleado : "00000",
                                id_empleado = implementador.id_empleado,
                                actividad = implementador.actividad
                            }
                            );
                    }
                    //idea implementada
                    ideaBD.ideaImplementada = iM_Idea_mejora.ideaImplementada == null ? 2 : iM_Idea_mejora.ideaImplementada; //0 = sin implementar; 1 = Implementada; 2 = vacio;

                    //Guarda los valores asociados a la implementación
                    if (FechaImplementacionEnabled)
                    {
                        ideaBD.implementacionFecha = iM_Idea_mejora.implementacionFecha;
                        ideaBD.reconocimentoClave = iM_Idea_mejora.reconocimentoClave;
                        ideaBD.reconocimientoMonto = iM_Idea_mejora.reconocimientoMonto == null ? 0 : iM_Idea_mejora.reconocimientoMonto;
                    }
                }
                #endregion

                try
                {
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ha modificado la idea de mejora correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                    return RedirectToAction("listadoIdeas");
                }
                catch (Exception e)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert("Error al Guardar en BD: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                    ModelState.AddModelError("", $"Ocurrió un error al Guardar en BD({e.Message}): {e.StackTrace}");

                }

            }

            //envia todos los viewbag
            //crea un Select  list para el estatus
            List<SelectListItem> newListTipoIdea = new List<SelectListItem>
            {
                //Agrega el estatus al selectListItem
                new SelectListItem() { Text = "Idea de Mejora", Value = "Idea de Mejora" },
                new SelectListItem() { Text = "Idea de Innovación", Value = "Idea de Innovación" }
            };


            ViewBag.ClasificacionImpactoList = db.IM_cat_impacto.Where(x => x.activo).ToList();
            ViewBag.DesperdicioList = db.IM_cat_desperdicio.Where(x => x.activo).ToList();

            List<empleados> listadoEmpleados = db.empleados.Where(p => p.activo == true).ToList();
            ViewBag.ListadoEmpleados = listadoEmpleados;

            SelectList selectListItemTipoIdea = new SelectList(newListTipoIdea, "Value", "Text");
            ViewBag.tipo_idea = AddFirstItem(selectListItemTipoIdea, textoPorDefecto: "-- Seleccionar --");

            ViewBag.id_planta_implementacion = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo == true), nameof(plantas.clave), nameof(plantas.descripcion)),
                    textoPorDefecto: "-- Seleccione un valor --", selected: iM_Idea_mejora.id_planta_implementacion.ToString());
            ViewBag.areaImplementacionClave = AddFirstItem(new SelectList(db.IM_cat_area.Where(x => x.activo == true && iM_Idea_mejora.id_planta_implementacion.HasValue && x.id_planta == iM_Idea_mejora.id_planta_implementacion.Value), nameof(IM_cat_area.id), nameof(IM_cat_area.descripcion)),
                    textoPorDefecto: "-- Seleccione un valor --", selected: iM_Idea_mejora.areaImplementacionClave.ToString());

            ViewBag.nivelImpactoClave = AddFirstItem(new SelectList(db.IM_cat_nivel_impacto.Where(x => x.activo), nameof(IM_cat_nivel_impacto.id), nameof(IM_cat_nivel_impacto.descripcion)), selected: iM_Idea_mejora.nivelImpactoClave.ToString());
            ViewBag.reconocimentoClave = AddFirstItem(new SelectList(db.IM_cat_reconocimiento.Where(x => x.activo), nameof(IM_cat_reconocimiento.id), nameof(IM_cat_reconocimiento.descripcion)), selected: iM_Idea_mejora.reconocimentoClave.ToString());

            //obtiene el último estatus del la idea
            var estatus = ideaBD.IM_rel_estatus.LastOrDefault();
            int id_estatus = 0;
            if (estatus != null)
                id_estatus = estatus.catalogoIdeaMejoraEstatusClave;

            //****** COLOCAR EL PRÓXIMO ESTATUS **********
            ViewBag.evaluacion_inicial = evaluacion_inicial;

            //manda indicador, para mostrar la pestaña de evaluacion
            ViewBag.Reload = true;

            //asigna los objetos que deben ser cargados desde BD
            iM_Idea_mejora.plantas = ideaBD.plantas;
            iM_Idea_mejora.IM_rel_proponente = ideaBD.IM_rel_proponente;
            iM_Idea_mejora.IM_rel_archivos = ideaBD.IM_rel_archivos;
            iM_Idea_mejora.IM_rel_impacto = ideaBD.IM_rel_impacto;
            iM_Idea_mejora.IM_rel_reduccion_desperdicio = ideaBD.IM_rel_reduccion_desperdicio;
            iM_Idea_mejora.IM_rel_comentario = ideaBD.IM_rel_comentario;
            iM_Idea_mejora.IM_rel_estatus = ideaBD.IM_rel_estatus;


            foreach (var item in iM_Idea_mejora.IM_rel_implementador)
            {
                item.empleados = db.empleados.Find(item.id_empleado);
            }

            //determina si esta en proceso de implementacion
            if (iM_Idea_mejora.enProcesoImplementacion.HasValue && iM_Idea_mejora.enProcesoImplementacion.Value == 1)
                iM_Idea_mejora.enProcesoBool = true;

            return View(iM_Idea_mejora);
        }

        // GET: MejoraContinua
        [AllowAnonymous]
        public ActionResult ListadoIdeas(int? id_planta, int? id_estatus, string id_proponente, string fecha_inicial, string fecha_final, string folio, string titulo, int pagina = 1)
        {
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = 20; // parámetro

            //tranforma los valores de las fechas
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime hoy = DateTime.Now;

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = new DateTime(hoy.Year, hoy.Month, hoy.Day, 23, 59, 59);          //fecha final por defecto

            //CONVIERRTE EL FOLIO A MAYUSCULAS
            if(!String.IsNullOrEmpty(folio))
                folio = folio.ToUpper();

            try
            {
                if (!String.IsNullOrEmpty(fecha_inicial))
                    dateInicial = Convert.ToDateTime(fecha_inicial);
                if (!String.IsNullOrEmpty(fecha_final))
                {
                    dateFinal = Convert.ToDateTime(fecha_final);
                    dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine("Error de Formato: " + e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al convertir: " + ex.Message);
            }

            //variable para almacenar la totalidad de registros
            var totalidadRegistrosBD = db.view_ideas_mejora.Where(x =>
                 (id_planta == null || x.clave_planta == id_planta)
                && (id_estatus == null || x.estatus_id == id_estatus)
                && x.fecha_captura >= dateInicial && x.fecha_captura <= dateFinal
                && (string.IsNullOrEmpty(titulo) || x.titulo.Contains(titulo))
                && (string.IsNullOrEmpty(folio) || x.folio.Contains(folio))
                && (string.IsNullOrEmpty(id_proponente) || x.proponentes.Contains("|" + id_proponente + "|"))
                ).ToList();

            //obtiene el total de registros, según los filtros 
            var listado = totalidadRegistrosBD
                //.Where(x => )
                .OrderByDescending(x => x.id)
                .Skip((pagina - 1) * cantidadRegistrosPorPagina)
               .Take(cantidadRegistrosPorPagina).ToList();


            //obtiene la cantidad de registros
            var totalDeRegistros = totalidadRegistrosBD.Count();

            //variables para la páginacion
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id_planta"] = id_planta;
            routeValues["id_estatus"] = id_estatus;
            routeValues["titulo"] = titulo;
            routeValues["folio"] = folio;
            routeValues["pagina"] = pagina;


            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };

            ViewBag.Paginacion = paginacion;

            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true && p.clave != 4), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Todas --");
            ViewBag.id_estatus = AddFirstItem(new SelectList(db.IM_cat_estatus.Where(p => p.activo == true), nameof(IM_cat_estatus.id), nameof(IM_cat_estatus.descripcion)), textoPorDefecto: "-- Todos --");
            ViewBag.id_proponente = AddFirstItem(new SelectList(db.empleados.Where(p => p.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), textoPorDefecto: "-- Todos --");

            return View(listado);
        }

        // GET: MejoraContinua
        [AllowAnonymous]
        public ActionResult Metricas(int? id_planta, int? id_estatus, int? id_proponente, string fecha_inicial, string fecha_final, string titulo)
        {
            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            //tranforma los valores de las fechas
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = DateTime.Now;          //fecha final por defecto

            try
            {
                if (!String.IsNullOrEmpty(fecha_inicial))
                    dateInicial = Convert.ToDateTime(fecha_inicial);
                if (!String.IsNullOrEmpty(fecha_final))
                {
                    dateFinal = Convert.ToDateTime(fecha_final);
                    dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine("Error de Formato: " + e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al convertir: " + ex.Message);
            }


            //variables de filtro
            int xPlantaClave = id_planta ?? 0; // Si no se proporciona un valor, se usará 0 como ejemplo.

            // Obtener las ideas filtradas por planta, fechas, proponente, y estatus si se proporcionan los valores
            var ideasFiltradas = db.IM_Idea_mejora
                .Where(idea =>
                    (!id_planta.HasValue || idea.clave_planta == id_planta) &&
                    (idea.captura >= dateInicial) &&
                    (idea.captura <= dateFinal) &&
                    (!id_proponente.HasValue || db.IM_rel_proponente.Any(p => p.ideaMejoraClave == idea.id && p.id_empleado == id_proponente)) &&
                    (string.IsNullOrEmpty(titulo) || idea.titulo.Contains(titulo))
                )
                .Join(db.IM_rel_estatus,
                      idea => idea.id,
                      rel => rel.ideaMejoraClave,
                      (idea, rel) => new { idea, rel })
                .GroupBy(x => x.rel.ideaMejoraClave)
                .Select(g => g.OrderByDescending(e => e.rel.captura).FirstOrDefault())
                .Where(x => !id_estatus.HasValue || x.rel.catalogoIdeaMejoraEstatusClave == id_estatus)
                .Select(x => x.idea)
                .ToList();

            //crea el nuevo model
            EstadisticasIMViewModel modelo = new EstadisticasIMViewModel();


            //variable para almacenar la totalidad de registros
            var totalidadRegistrosBD = db.view_ideas_mejora.Where(x =>
                 (id_planta == null || x.clave_planta == id_planta)
                && (id_estatus == null || x.estatus_id == id_estatus)
                && x.fecha_captura >= dateInicial && x.fecha_captura <= dateFinal
                && (string.IsNullOrEmpty(titulo) || x.titulo.Contains(titulo))
                && (!id_proponente.HasValue || x.proponentes.Contains("|" + id_proponente + "|"))
                ).ToList();


            //obtiene el total de registros, según los filtros 
            var listado = totalidadRegistrosBD
                //.Where(x => )
                .OrderByDescending(x => x.id)
               .ToList();


            //obtiene la cantidad de registros
            var totalDeRegistros = totalidadRegistrosBD.Count();

            //variables para la páginacion
            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["id_planta"] = id_planta;
            routeValues["id_estatus"] = id_estatus;
            routeValues["titulo"] = titulo;

            //envia las estadisticas por planta
            // 1.- Consulta para obtener el número de ideas por planta
            modelo.IdeasPorPlanta = ideasFiltradas
                .Join(db.plantas,
                      idea => idea.clave_planta,
                      planta => planta.clave,
                      (idea, planta) => new { idea, planta })
                .GroupBy(x => x.planta.descripcion)
                .Select(g => new IdeasPorPlantaViewModel
                {
                    NombrePlanta = g.Key,
                    TotalIdeas = g.Count()
                }).ToList();

            // Obtener el estatus más reciente para cada solicitud
            var estatusPorSolicitud = ideasFiltradas
                 .Join(db.IM_rel_estatus,
                       idea => idea.id,
                       rel => rel.ideaMejoraClave,
                       (idea, rel) => new { rel, idea })
                 .GroupBy(x => x.rel.ideaMejoraClave)
                 .Select(g => g.OrderByDescending(e => e.rel.captura).FirstOrDefault())
                 .Join(db.IM_cat_estatus,
                       rel => rel.rel.catalogoIdeaMejoraEstatusClave,
                       cat => cat.id,
                       (rel, cat) => new { EstatusDescripcion = cat.descripcion })
                 .GroupBy(e => e.EstatusDescripcion)
                 .Select(g => new
                 {
                     Estatus = g.Key,
                     Total = g.Count()
                 })
                 .ToList();

            // Añadir los datos al ViewModel
            modelo.SolicitudesPorEstatus = estatusPorSolicitud.Select(e => new SolicitudesPorEstatusViewModel
            {
                Estatus = e.Estatus,
                Total = e.Total
            }).ToList();

            // 2.- Obtener la clasificación de ideas en equipo o individuales
            var ideasPorProponente = ideasFiltradas
                .GroupJoin(db.IM_rel_proponente,
                           idea => idea.id,
                           prop => prop.ideaMejoraClave,
                           (idea, proponentes) => new
                           {
                               Idea = idea,
                               ProponentesCount = proponentes.Count()
                           })
                .GroupBy(x => x.ProponentesCount > 1 ? "En Equipo" : "Individual")
                .Select(g => new
                {
                    Tipo = g.Key,
                    Total = g.Count()
                })
                .ToList();

            // Añadir los datos al ViewModel
            modelo.IdeasPorProponente = ideasPorProponente.Select(e => new IdeasPorProponenteViewModel
            {
                Tipo = e.Tipo,
                Total = e.Total
            }).ToList();

            // 3.-Obtener la clasificación de ideas por eliminación de desperdicio
            var desperdicioPorIdea = ideasFiltradas
                .Join(db.IM_rel_reduccion_desperdicio,
                      idea => idea.id,
                      rel => rel.ideaMejoraClave,
                      (idea, rel) => new { rel })
                .Join(db.IM_cat_desperdicio,
                      rel => rel.rel.catalogoIdeaMejoraDesperdicioClave,
                      cat => cat.id,
                      (rel, cat) => new { DesperdicioDescripcion = cat.descripcion })
                .GroupBy(e => e.DesperdicioDescripcion)
                .Select(g => new
                {
                    Desperdicio = g.Key,
                    Total = g.Count()
                })
                .ToList();

            // Añadir los datos al ViewModel
            modelo.IdeasPorDesperdicio = desperdicioPorIdea.Select(e => new IdeasPorDesperdicioViewModel
            {
                Desperdicio = e.Desperdicio,
                Total = e.Total
            }).ToList();


            // 4. Obtener la clasificación de ideas por nivel de impacto
            var impactoPorIdea = ideasFiltradas
                .Join(db.IM_rel_impacto,
                      idea => idea.id,
                      rel => rel.ideaMejoraClave,
                      (idea, rel) => new { rel })
                .Join(db.IM_cat_impacto,
                      rel => rel.rel.catalogoIdeaMejoraImpactoClave,
                      cat => cat.id,
                      (rel, cat) => new { ImpactoDescripcion = cat.descripcion })
                .GroupBy(e => e.ImpactoDescripcion)
                .Select(g => new
                {
                    Impacto = g.Key,
                    Total = g.Count()
                })
                .ToList();

            // Añadir los datos al ViewModel
            modelo.IdeasPorImpacto = impactoPorIdea.Select(e => new IdeasPorImpactoViewModel
            {
                Impacto = e.Impacto,
                Total = e.Total
            }).ToList();


            // 5. Ideas de mejora por mes de captura y planta (excluyendo las plantas que son null)
            var ideasPorMesPlanta = ideasFiltradas
                .Where(i => i.clave_planta != null)
                .GroupBy(i => new { Mes = i.captura.Month, Año = i.captura.Year, Planta = i.clave_planta })
                .Select(g => new
                {
                    MesPlanta = $"{g.Key.Año}-{g.Key.Mes.ToString("D2")}",
                    Total = g.Count(),
                    PlantaNombre = db.plantas.FirstOrDefault(p => p.clave == g.Key.Planta)?.descripcion
                }).ToList();

            // Añadir los datos al ViewModel
            modelo.IdeasPorMesPlanta = ideasPorMesPlanta.Select(e => new IdeasPorMesPlantaViewModel
            {
                MesPlanta = e.MesPlanta,
                Total = e.Total,
                PlantaNombre = e.PlantaNombre
            }).ToList();

            // Cantidad de solicitudes por área y por planta
            var solicitudesPorAreaYPlanta = ideasFiltradas
                .Where(i => i.areaImplementacionClave != null && i.clave_planta != null)
                .GroupBy(i => new { Area = i.areaImplementacionClave, Planta = i.clave_planta })
                .Select(g => new
                {
                    AreaNombre = db.IM_cat_area.FirstOrDefault(a => a.id == g.Key.Area)?.descripcion,
                    PlantaNombre = db.plantas.FirstOrDefault(p => p.clave == g.Key.Planta)?.descripcion,
                    Total = g.Count()
                }).ToList();

            // Añadir los datos al ViewModel
            modelo.SolicitudesPorAreaYPlanta = solicitudesPorAreaYPlanta.Select(e => new SolicitudesPorAreaYPlantaViewModel
            {
                AreaNombre = e.AreaNombre,
                PlantaNombre = e.PlantaNombre,
                Total = e.Total
            }).ToList();

            //valores para combos
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(p => p.activo == true && p.clave != 4), nameof(plantas.clave), nameof(plantas.descripcion)), textoPorDefecto: "-- Todas --");
            ViewBag.id_estatus = AddFirstItem(new SelectList(db.IM_cat_estatus.Where(p => p.activo == true), nameof(IM_cat_estatus.id), nameof(IM_cat_estatus.descripcion)), textoPorDefecto: "-- Todos --");
            ViewBag.id_proponente = AddFirstItem(new SelectList(db.empleados.Where(p => p.activo == true), nameof(empleados.id), nameof(empleados.ConcatNumEmpleadoNombre)), textoPorDefecto: "-- Todos --");


            modelo.listadoIdeasView = listado;

            return View(modelo);
        }

        public ActionResult Exportar(int? id_planta, int? id_estatus, string id_proponente, string fecha_inicial, string fecha_final, string titulo)
        {
            if (!TieneRol(TipoRoles.IM_ADMIN))
                return View("../Home/ErrorPermisos");

            //convierte las fechas recibidas
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateInicial = new DateTime(2000, 1, 1);  //fecha inicial por defecto
            DateTime dateFinal = DateTime.Now;          //fecha final por defecto

            try
            {
                if (!String.IsNullOrEmpty(fecha_inicial))
                    dateInicial = Convert.ToDateTime(fecha_inicial);
                if (!String.IsNullOrEmpty(fecha_final))
                {
                    dateFinal = Convert.ToDateTime(fecha_final);
                    dateFinal = dateFinal.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine("Error de Formato: " + e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al convertir: " + ex.Message);
            }

            var listado = db.view_ideas_mejora.Where(x =>
                 (id_planta == null || x.clave_planta == id_planta)
                && (id_estatus == null || x.estatus_id == id_estatus)
                && x.fecha_captura >= dateInicial && x.fecha_captura <= dateFinal
                && (string.IsNullOrEmpty(titulo) || x.titulo.Contains(titulo))
                && (string.IsNullOrEmpty(id_proponente) || x.proponentes.Contains("|" + id_proponente + "|"))
                ).ToList();

            byte[] stream = ExcelUtil.GeneraReporteIM(listado);


            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = "Reporte_Ideas_Mejora_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx",

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(stream, "application/vnd.ms-excel");

        }

        // GET: MejoraContinua
        public ActionResult Ingresar()
        {
            if (!TieneRol(TipoRoles.IM_ADMIN))
                return View("../MejoraContinua/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return View("../Error/BadRequest");
            }
            var idea = db.IM_Idea_mejora.Find(id);
            if (idea == null)
            {
                return View("../Error/NotFound");
            }

            //obtiene el último estatus del la idea
            var estatus = idea.IM_rel_estatus.LastOrDefault();
            int id_estatus = 0;

            if (estatus != null)
                id_estatus = estatus.catalogoIdeaMejoraEstatusClave;
            ViewBag.evaluacion_inicial = id_estatus;

            //determina si la fecha de implementacion es valida
            if (idea.implementacionFecha.HasValue && idea.implementacionFecha.Value.Year < 2000)
                idea.implementacionFecha = null;

            //determina si esta en proceso de implementacion
            if (idea.enProcesoImplementacion.HasValue && idea.enProcesoImplementacion.Value == 1)
                idea.enProcesoBool = true;

            //determina si la idea en equipo
            if (idea.IM_rel_proponente.Count() > 1)
                idea.ideaEnEquipoBool = true;

            ViewBag.ClasificacionImpactoList = db.IM_cat_impacto.Where(x => x.activo).ToList();
            ViewBag.DesperdicioList = db.IM_cat_desperdicio.Where(x => x.activo).ToList();

            return View(idea);
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