using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
    public class IT_asignacion_softwareController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();


        // GET: IT_asignacion_software/Details/5
        public ActionResult Asignar(int? id_empleado)
        {
            if (!TieneRol(TipoRoles.IT_ASIGNACION_HARDWARE))
                return View("../Home/ErrorPermisos");

            if (id_empleado == null)
            {
                return View("../Error/BadRequest");
            }
            empleados empleado = db.empleados.Find(id_empleado);
            if (empleado == null)
            {
                return View("../Error/NotFound");
            }

            empleados sistemas = obtieneEmpleadoLogeado();

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            //crea la base del modelo
            AsignarSoftwareViewModel model = new AsignarSoftwareViewModel
            {
                id_empleado = id_empleado.Value,
                id_sistemas = sistemas.id,
                EmpleadoSistemas = sistemas,
                EmpleadoUsuario = empleado
            };

            //si ya tiene asignaciones previas las agrega
            if (empleado.IT_asignacion_software.Count > 0)
                model.IT_asignacion_software = empleado.IT_asignacion_software.ToList();

            //envia el listado de software activos
            List<IT_inventory_software> listadoSoftware = db.IT_inventory_software.Where(p => p.activo == true).ToList();

            ViewBag.ListadoSoftware = listadoSoftware;

            return View(model);
        }

        // POST: IT_asignacion_software/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Asignar(AsignarSoftwareViewModel model, FormCollection collection)
        {

            //leer los valores
            List<IT_asignacion_software> listaSoftware = new List<IT_asignacion_software>();

            foreach (string key in collection.AllKeys.Where(x => x.StartsWith("software_")))
            {
                int index = -1;
                int id_sofware = 0;
                int id_version = 0;
                string usuario = null;
                int? version = null;

                Match m = Regex.Match(key, @"\d+");

                if (m.Success) //si tiene un numero
                {
                    int.TryParse(m.Value, out index);
                    int.TryParse(collection["software_" + index], out id_sofware);
                    int.TryParse(collection["version_" + index], out id_version);
                    usuario = collection["usuario_" + index];


                    if (id_version > 0)
                        version = id_version;
                    else
                        version = null;

                    listaSoftware.Add(
                        new IT_asignacion_software
                        {
                            id_inventory_software = id_sofware,
                            usuario = usuario,
                            id_empleado = model.id_empleado,
                            id_sistemas = model.id_sistemas,
                        }
                    );
                }
            }

            //asocia los obtenido con el modelo
            model.IT_asignacion_software = listaSoftware;

            //validar que no se repida la misma conbinacion entre software y versión
            if (listaSoftware.Count != listaSoftware.Distinct().Count())
                ModelState.AddModelError("", "Verifique que el mismo software no esté asignado dos veces.");

            if (ModelState.IsValid)
            {
                //elimina los items previos del empleado
                var listBD = db.IT_asignacion_software.Where(x => x.id_empleado == model.id_empleado);

                //pone en null todas las asignaciones de la matriz
                foreach (var asig in listBD)
                {
                    var matriz = db.IT_matriz_software.Where(x => x.id_it_asignacion_software == asig.id).FirstOrDefault();
                    if (matriz != null)
                        matriz.id_it_asignacion_software = null;
                }

                db.IT_asignacion_software.RemoveRange(listBD);

                db.IT_asignacion_software.AddRange(model.IT_asignacion_software);

                //vuelve a colocar el id de asignacion a la matriz en caso de existir
                foreach (var asig in model.IT_asignacion_software)
                {
                    var matriz = db.IT_matriz_software.Where(x => x.id_it_asignacion_software == asig.id).FirstOrDefault();
                    if (matriz != null)
                        matriz.id_it_asignacion_software = asig.id;
                }

                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("ListadoAsignaciones", "IT_asignacion_hardware", new { id = model.id_empleado });
            }
            model.EmpleadoSistemas = db.empleados.Find(model.id_sistemas);
            model.EmpleadoUsuario = db.empleados.Find(model.id_empleado);

            //envia el listado de software activos
            List<IT_inventory_software> listadoSoftware = db.IT_inventory_software.Where(p => p.activo == true).ToList();

            ViewBag.ListadoSoftware = listadoSoftware;

            return View(model);
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
