using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{

    [Authorize]
    public class PuestosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: Puestos
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.RH))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                var puesto = db.puesto.Include(p => p.Area);
                return View(puesto.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // GET: Puestos/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.RH))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                puesto puesto = db.puesto.Find(id);
                if (puesto == null)
                {
                    return View("../Error/NotFound");
                }

                puesto.shared_services = puesto.Area != null && puesto.Area.shared_services;

                return View(puesto);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // GET: Puestos/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.RH))
            {
                //busca todas las plantas que tengan un area 
                List<plantas> plantasList = db.plantas.ToList();
                //crea el select list para plantas
                List<SelectListItem> newList = new List<SelectListItem>();

                foreach (var p in plantasList)
                {
                    newList.Add(new SelectListItem()
                    {
                        Text = p.descripcion,
                        Value = p.clave.ToString()
                    });
                }

                //envia el select list por viewbag
                ViewBag.plantaClave = AddFirstItem(new SelectList(newList, "Value", "Text"), textoPorDefecto: "-- Seleccione --");

                ViewBag.areaClave = new SelectList(db.Area.Where(p => p.activo == true), "clave", "descripcion");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: Puestos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(puesto puesto)
        {

            if (db.puesto.Any(x => x.descripcion == puesto.descripcion && x.areaClave == puesto.areaClave))
                ModelState.AddModelError("", "Ya existe un registro con los mismos valores.");

            if (ModelState.IsValid)
            {
                puesto.activo = true;
                db.puesto.Add(puesto);
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }

            //busca todas las plantas que tengan un area 
            List<plantas> plantasList = db.plantas.ToList();
            //crea el select list para plantas
            List<SelectListItem> newList = new List<SelectListItem>();


            foreach (var p in plantasList)
            {
                newList.Add(new SelectListItem()
                {
                    Text = p.descripcion,
                    Value = p.clave.ToString()
                });
            }

            var area = db.Area.Find(puesto.areaClave);

            var listArea = db.Area.Where(p => p.activo == true && p.plantaClave == area.plantaClave && !p.shared_services);
            if (puesto.shared_services)
                listArea = db.Area.Where(p => p.activo == true && p.shared_services);

            List<Area> xlist = listArea.ToList();

            //envia el select list por viewbag
            ViewBag.plantaClave = AddFirstItem(new SelectList(newList, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: area.plantaClave.ToString());
            ViewBag.areaClave = new SelectList(listArea, "clave", "descripcion");
            return View(puesto);
        }

        // GET: Puestos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TieneRol(TipoRoles.RH))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                puesto puesto = db.puesto.Find(id);
                if (puesto == null)
                {
                    return View("../Error/NotFound");
                }
                //busca todas las plantas que tengan un area 
                List<plantas> plantasList = db.plantas.ToList();
                //crea el select list para plantas
                List<SelectListItem> newList = new List<SelectListItem>();

                foreach (var p in plantasList)
                {
                    newList.Add(new SelectListItem()
                    {
                        Text = p.descripcion,
                        Value = p.clave.ToString()
                    });
                }

                if (puesto.Area.shared_services)
                    puesto.shared_services = true;

                var area = db.Area.Find(puesto.areaClave);

                var listArea = db.Area.Where(p => p.activo == true && p.plantaClave == area.plantaClave && !p.shared_services);
                if (puesto.shared_services)
                    listArea = db.Area.Where(p => p.activo == true && p.shared_services);

                //envia el select list por viewbag
                ViewBag.plantaClave = AddFirstItem(new SelectList(newList, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: area.plantaClave.ToString());
                ViewBag.areaClave = new SelectList(listArea, "clave", "descripcion", selectedValue: area.clave.ToString());
                return View(puesto);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        // POST: Puestos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(puesto puesto, FormCollection collection)
        {
            //valores enviados previamente
            int c_planta = 0;
            if (!String.IsNullOrEmpty(collection["plantaClave"]))
                Int32.TryParse(collection["plantaClave"].ToString(), out c_planta);

            if (ModelState.IsValid)
            {
                db.Entry(puesto).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            //busca todas las plantas que tengan un area 
            List<plantas> plantasList = db.plantas.ToList();
            //crea el select list para plantas
            List<SelectListItem> newList = new List<SelectListItem>();
            foreach (var p in plantasList)
            {
                newList.Add(new SelectListItem()
                {
                    Text = p.descripcion,
                    Value = p.clave.ToString()
                });
            }

            if (puesto.Area.shared_services)
                puesto.shared_services = true;

            var area = db.Area.Find(puesto.areaClave);

            var listArea = db.Area.Where(p => p.activo == true && p.plantaClave == area.plantaClave && !p.shared_services);
            if (puesto.shared_services)
                listArea = db.Area.Where(p => p.activo == true && p.shared_services);

            //envia el select list por viewbag
            ViewBag.plantaClave = AddFirstItem(new SelectList(newList, "Value", "Text"), textoPorDefecto: "-- Seleccione --", selected: area.plantaClave.ToString());
            ViewBag.areaClave = new SelectList(listArea, "clave", "descripcion", selectedValue: area.clave.ToString());

            return View(puesto);
        }

        // GET: Puestos/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.RH))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                puesto puesto = db.puesto.Find(id);
                if (puesto == null)
                {
                    return View("../Error/NotFound");
                }

                puesto.shared_services = puesto.Area != null && puesto.Area.shared_services;

                return View(puesto);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Puestos/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            puesto puestos = db.puesto.Find(id);
            puestos.activo = false;

            db.Entry(puestos).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Index");
            }
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.DISABLED, TipoMensajesSweetAlerts.SUCCESS);
            return RedirectToAction("Index");
        }

        // GET: puestos/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.ADMIN))
            {

                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                puesto puesto = db.puesto.Find(id);
                if (puesto == null)
                {
                    return View("../Error/NotFound");
                }

                puesto.shared_services = puesto.Area != null && puesto.Area.shared_services;

                return View(puesto);

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: Plantas/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            puesto puestos = db.puesto.Find(id);
            puestos.activo = true;

            db.Entry(puestos).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat("Para continuar verifique: ", fullErrorMessage);

                TempData["Mensaje"] = new MensajesSweetAlert(exceptionMessage, TipoMensajesSweetAlerts.WARNING);
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Ha ocurrido un error: " + e.Message, TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Index");
            }
            TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.ENABLED, TipoMensajesSweetAlerts.SUCCESS);
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

        #region Helpers
        ///<summary>
        ///Obtiene las areas segun la planta recibida
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneAreas(int clavePlanta = 0)
        {
            //obtiene todos los posibles valores
            var areas = db.Area.Include(a => a.plantas);
            List<Area> listado = areas.Where(p => p.plantaClave.Value == clavePlanta && p.activo == true && !p.shared_services).ToList();

            if (clavePlanta == 99)
                listado = areas.Where(p => p.shared_services && p.activo == true).ToList();

            //inserta el valor por default
            listado.Insert(0, new Area
            {
                clave = 0,
                descripcion = "-- Seleccione un valor --"
            });

            //inicializa la lista de objetos
            var list = new object[listado.Count];

            //completa la lista de objetos
            for (int i = 0; i < listado.Count; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = listado[i].descripcion };
                else
                    list[i] = new { value = listado[i].clave, name = listado[i].ConcatDeptoCeCo };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        ///Obtiene las areas segun la planta recibida (activas y no activas)
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult obtieneTodasAreas(int clavePlanta = 0)
        {
            //obtiene todos los posibles valores
            var areas = db.Area.Include(a => a.plantas);
            List<Area> listado = areas.Where(p => p.plantaClave.Value == clavePlanta).ToList();


            //inserta el valor por default
            listado.Insert(0, new Area
            {
                clave = 0,
                descripcion = "-- Seleccione un valor --"
            });

            //inicializa la lista de objetos
            var list = new object[listado.Count];

            //completa la lista de objetos
            for (int i = 0; i < listado.Count; i++)
            {
                if (i == 0)//en caso de item por defecto
                    list[i] = new { value = "", name = listado[i].descripcion };
                else
                    list[i] = new { value = listado[i].clave, name = listado[i].descripcion };
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        #endregion

    }
}
