﻿using Bitacoras.Util;
using Clases.Util;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_matriz_requerimientosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: IT_matriz_requerimientos/CrearMatriz
        public ActionResult CrearMatriz(int? id)
        {
            if (TieneRol(TipoRoles.RH))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                empleados empleados = db.empleados.Find(id);
                if (empleados == null)
                {
                    return View("../Error/NotFound");

                }

                IT_matriz_requerimientos matriz = db.IT_matriz_requerimientos.Where(x => x.id_empleado == id).FirstOrDefault();
                if (matriz == null)
                {
                    //crea una matriz nueva con el empleado asociado
                    matriz = new IT_matriz_requerimientos
                    {
                        id_empleado = empleados.id,
                        empleados = empleados
                    };

                }

                //obtiene la lista de hardware
                ViewBag.listHardware = db.IT_hardware_tipo.Where(x => x.activo == true).ToList();
                ViewBag.listSoftware = db.IT_software_tipo.Where(x => x.activo == true).ToList();
                ViewBag.id_internet_tipo = AddFirstItem(new SelectList(db.IT_internet_tipo.Where(p => p.activo == true), "id", "descripcion"));
                ViewBag.listComunicaciones = db.IT_comunicaciones_tipo.Where(x => x.activo == true).ToList();
                ViewBag.listCarpetas = db.IT_carpetas_red.Where(x => x.activo == true).ToList();
                ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(p => p.activo == true), "id", "ConcatNumEmpleadoNombre"));
                return View(matriz);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: IT_matriz_requerimientos/CrearMatriz
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearMatriz(IT_matriz_requerimientos matriz, FormCollection collection, string[] SelectedHardware, string[] SelectedSoftware, string[] SelectedComunicaciones, string[] SelectedCarpetas)
        {

            //lista de key del collection
            List<string> keysCollection = collection.AllKeys.ToList();

            #region Asignación de Objetos

            //crea los objetos para hardware
            if (SelectedHardware != null)
                foreach (string id_hardware_string in SelectedHardware)
                {
                    //obtiene el id
                    Match m = Regex.Match(id_hardware_string, @"\d+");
                    int id_hardware = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_hardware);

                    string keyDescription = "hardware_" + id_hardware + "_descripcion";
                    String descripcionHardware = null;

                    //busca si existe una descripcion
                    if (keysCollection.Contains(keyDescription))
                        descripcionHardware = collection[keyDescription];

                    matriz.IT_matriz_hardware.Add(new IT_matriz_hardware { id_it_hardware = id_hardware, descripcion = descripcionHardware });
                }

            //crea los objetos para software
            if (SelectedSoftware != null)
                foreach (string id_software_string in SelectedSoftware)
                {
                    //obtiene el id
                    Match m = Regex.Match(id_software_string, @"\d+");
                    int id_software = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_software);

                    string keyDescription = "software_" + id_software + "_descripcion";
                    String descripcionSoftware = null;

                    if (keysCollection.Contains(keyDescription))
                        descripcionSoftware = collection[keyDescription];

                    matriz.IT_matriz_software.Add(new IT_matriz_software { id_it_software = id_software, descripcion = descripcionSoftware });
                }

            //crea los objetos para comunicaciones
            if (SelectedComunicaciones != null)
                foreach (string id_comunicaciones_string in SelectedComunicaciones)
                {
                    //obtiene el id
                    Match m = Regex.Match(id_comunicaciones_string, @"\d+");
                    int id_comunicaciones = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_comunicaciones);

                    string keyDescription = "comunicaciones_" + id_comunicaciones + "_descripcion";
                    String descripcionComunicaciones = null;

                    if (keysCollection.Contains(keyDescription))
                        descripcionComunicaciones = collection[keyDescription];

                    matriz.IT_matriz_comunicaciones.Add(new IT_matriz_comunicaciones { id_it_comunicaciones = id_comunicaciones, descripcion = descripcionComunicaciones });
                }

            //crea los objetos para las carpetas
            if (SelectedCarpetas != null)
                foreach (string id_carpetas_string in SelectedCarpetas)
                {
                    //obtiene el id
                    Match m = Regex.Match(id_carpetas_string, @"\d+");
                    int id_carpetas = 0;

                    if (m.Success)//si tiene un numero                
                        int.TryParse(m.Value, out id_carpetas);

                    string keyDescription = "carpetas_" + id_carpetas + "_descripcion";
                    String descripcionCarpetas = null;

                    if (keysCollection.Contains(keyDescription))
                        descripcionCarpetas = collection[keyDescription];

                    matriz.IT_matriz_carpetas.Add(new IT_matriz_carpetas { id_it_carpeta_red = id_carpetas, descripcion = descripcionCarpetas });
                }
            #endregion

            if (ModelState.IsValid)
            {
                //campos obligatorios 
                matriz.fecha_solicitud = DateTime.Now;
                matriz.estatus = IT_MR_Status.CREADO;

                db.IT_matriz_requerimientos.Add(matriz);
                db.SaveChanges();

                TempData["Mensaje"] = new MensajesSweetAlert("Se ha enviado la solicitud correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index", "empleados");
            }

            empleados empleados = db.empleados.Find(matriz.id_empleado);

            matriz.empleados = empleados;


            ViewBag.listHardware = db.IT_hardware_tipo.Where(x => x.activo == true).ToList();
            ViewBag.listSoftware = db.IT_software_tipo.Where(x => x.activo == true).ToList();
            ViewBag.id_internet_tipo = AddFirstItem(new SelectList(db.IT_internet_tipo.Where(p => p.activo == true), "id", "descripcion"));
            ViewBag.listComunicaciones = db.IT_comunicaciones_tipo.Where(x => x.activo == true).ToList();
            ViewBag.listCarpetas = db.IT_carpetas_red.Where(x => x.activo == true).ToList();
            ViewBag.id_jefe_directo = AddFirstItem(new SelectList(db.empleados.Where(p => p.activo == true), "id", "ConcatNumEmpleadoNombre"));
            return View(matriz);

        }
    }
}