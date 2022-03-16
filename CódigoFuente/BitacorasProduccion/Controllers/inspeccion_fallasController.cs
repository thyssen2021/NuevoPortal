using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class inspeccion_fallasController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: inspeccion_fallas
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.INSPECCION_CATALOGOS))
            {
                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                    ViewBag.MensajeAlert = TempData["Mensaje"];

                var inspeccion_fallas = db.inspeccion_fallas;
                return View(inspeccion_fallas.ToList());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // GET: inspeccion_fallas/Details/5
        public ActionResult Details(int? id)
        {
            if (TieneRol(TipoRoles.INSPECCION_CATALOGOS))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                inspeccion_fallas inspeccion_fallas = db.inspeccion_fallas.Find(id);
                if (inspeccion_fallas == null)
                {
                    return HttpNotFound();
                }
                return View(inspeccion_fallas);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

            
        }

        // GET: inspeccion_fallas/Create
        public ActionResult Create()
        {
            if (TieneRol(TipoRoles.INSPECCION_CATALOGOS))
            {
                ViewBag.id_categoria_falla = AddFirstItem(new SelectList(db.inspeccion_categoria_fallas.Where(x=> x.activo==true), "id", "descripcion"));
              
                return View(new inspeccion_fallas());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: inspeccion_fallas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(inspeccion_fallas inspeccion_fallas, FormCollection collection)
        {
            string tipoDano = String.Empty;
            
            if (collection.AllKeys.Contains("tipo_dano"))
                tipoDano = collection["tipo_dano"];


            if (tipoDano == "externo")
            {
                inspeccion_fallas.dano_externo = true;
                inspeccion_fallas.dano_interno = false;
            }

            if (tipoDano == "interno")
            {
                inspeccion_fallas.dano_interno = true;
                inspeccion_fallas.dano_externo = false;
            }

            inspeccion_fallas.activo = true;

            if (ModelState.IsValid)
            {
                //busca si existe con los mismos valores
                inspeccion_fallas item_busca = db.inspeccion_fallas.Where(s => s.descripcion.ToUpper() == inspeccion_fallas.descripcion.ToUpper() && !String.IsNullOrEmpty(inspeccion_fallas.descripcion)
                                                                        && s.id_categoria_falla == inspeccion_fallas.id_categoria_falla)
                                        .FirstOrDefault();

                if (item_busca == null)
                { //Si no existe
                    db.inspeccion_fallas.Add(inspeccion_fallas);
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.id_categoria_falla = AddFirstItem(new SelectList(db.inspeccion_fallas.Where(x => x.activo == true), "id", "descripcion"));
                    ModelState.AddModelError("", "Ya existe un registro con la misma descripción en la misma categoria.");
                    return View(inspeccion_fallas);
                }

               
            }

            ViewBag.id_categoria_falla = AddFirstItem(new SelectList(db.inspeccion_fallas.Where(x => x.activo == true), "id", "descripcion"));
            return View(inspeccion_fallas);
        }

        // GET: inspeccion_fallas/Edit/5
        public ActionResult Edit(int? id)
        {

            if (TieneRol(TipoRoles.INSPECCION_CATALOGOS))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                inspeccion_fallas inspeccion_fallas = db.inspeccion_fallas.Find(id);
                if (inspeccion_fallas == null)
                {
                    return HttpNotFound();
                }
                ViewBag.id_categoria_falla = AddFirstItem(new SelectList(db.inspeccion_categoria_fallas.Where(x => x.activo == true), "id", "descripcion"),selected: inspeccion_fallas.id_categoria_falla.ToString());
                return View(inspeccion_fallas);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

           
        }

        // POST: inspeccion_fallas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(inspeccion_fallas inspeccion_fallas, FormCollection collection)
        {
            string tipoDano = String.Empty;

            if (collection.AllKeys.Contains("tipo_dano"))
                tipoDano = collection["tipo_dano"];


            if (tipoDano == "externo")
            {
                inspeccion_fallas.dano_externo = true;
                inspeccion_fallas.dano_interno = false;
            }

            if (tipoDano == "interno")
            {
                inspeccion_fallas.dano_interno = true;
                inspeccion_fallas.dano_externo = false;
            }

            if (ModelState.IsValid)
            {
                //busca si existe con los mismos valores
                inspeccion_fallas item_busca = db.inspeccion_fallas.Where(s => s.descripcion.ToUpper() == inspeccion_fallas.descripcion.ToUpper() && !String.IsNullOrEmpty(inspeccion_fallas.descripcion)
                                                                        && s.id_categoria_falla == inspeccion_fallas.id_categoria_falla && s.id!=inspeccion_fallas.id)
                                        .FirstOrDefault();

                if (item_busca == null)
                { //Si no existe
                    db.Entry(inspeccion_fallas).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.id_categoria_falla = AddFirstItem(new SelectList(db.inspeccion_fallas.Where(x => x.activo == true), "id", "descripcion"), selected: inspeccion_fallas.id_categoria_falla.ToString());
                    ModelState.AddModelError("", "Ya existe un registro con la misma descripción en la misma categoria.");
                    return View(inspeccion_fallas);
                }

               
            }
            ViewBag.id_categoria_falla = AddFirstItem(new SelectList(db.inspeccion_fallas.Where(x => x.activo == true), "id", "descripcion"), selected: inspeccion_fallas.id_categoria_falla.ToString());
            return View(inspeccion_fallas);
        }

        // GET: inspeccion_fallas/Disable/5
        public ActionResult Disable(int? id)
        {
            if (TieneRol(TipoRoles.INSPECCION_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                inspeccion_fallas item = db.inspeccion_fallas.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }
                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: inspeccion_fallas/Disable/5
        [HttpPost, ActionName("Disable")]
        [ValidateAntiForgeryToken]
        public ActionResult DisableConfirmed(int id)
        {
            inspeccion_fallas item = db.inspeccion_fallas.Find(id);
            item.activo = false;

            db.Entry(item).State = EntityState.Modified;
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

        // GET: inspeccion_fallas/Enable/5
        public ActionResult Enable(int? id)
        {
            if (TieneRol(TipoRoles.INSPECCION_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }
                inspeccion_fallas item = db.inspeccion_fallas.Find(id);
                if (item == null)
                {
                    return View("../Error/NotFound");
                }
                return View(item);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: inspeccion_fallas/Enable/5
        [HttpPost, ActionName("Enable")]
        [ValidateAntiForgeryToken]
        public ActionResult EnableConfirmed(int id)
        {
            inspeccion_fallas item = db.inspeccion_fallas.Find(id);
            item.activo = true;

            db.Entry(item).State = EntityState.Modified;
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

        /// <summary>
        /// Muestra Listado de usuarios con el permiso de inspecciocion_registros
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Inspectores()
        {
            if (TieneRol(TipoRoles.INSPECCION_CATALOGOS))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                List<IdentitySample.Models.ApplicationUser> usuarios = await _userManager.Users.ToListAsync();
            
                usuarios=usuarios.Where(x=> _userManager.IsInRoleAsync(x.Id, TipoRoles.INSPECCION_REGISTRO).Result==true).ToList();

                return View(usuarios);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }


        public async Task<ActionResult> CreateInspector()
        {
            if (TieneRol(TipoRoles.INSPECCION_CATALOGOS))
            {

                List<IdentitySample.Models.ApplicationUser> usuarios = await _userManager.Users.ToListAsync();

                usuarios = usuarios.Where(x => _userManager.IsInRoleAsync(x.Id, TipoRoles.INSPECCION_REGISTRO).Result == false).ToList();

                ViewBag.id_usuario = ComboSelect.obtieneUsuarios(usuarios);

                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        // POST: inspeccion_fallas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateInspector(UserInspectorViewModel usuario)
        {

            if (ModelState.IsValid)
            {
                var result = await _userManager.AddToRoleAsync(usuario.id_usuario, TipoRoles.INSPECCION_REGISTRO);

                if (result.Succeeded)
                {
                    TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);
                    return RedirectToAction("Inspectores");
                }
                else {
                    TempData["Mensaje"] = new MensajesSweetAlert("Hubo un error al crear al inspector.", TipoMensajesSweetAlerts.ERROR);
                    return RedirectToAction("Inspectores");
                }
            }
            List<IdentitySample.Models.ApplicationUser> usuarios =  _userManager.Users.ToList();

            usuarios = usuarios.Where(x => _userManager.IsInRoleAsync(x.Id, TipoRoles.INSPECCION_REGISTRO).Result == false).ToList();

            ViewBag.id_usuario = ComboSelect.obtieneUsuarios(usuarios);

            return View(usuario);
        }

        // GET: inspeccion_fallas/DisableInspector/5
        public async Task<ActionResult> DisableInspector(string id)
        {
            if (TieneRol(TipoRoles.INSPECCION_CATALOGOS))
            {
                if (id == null)
                {
                    return View("../Error/BadRequest");
                }

                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return RedirectToAction("NotFound", "Error");


                return View(user);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        // POST: inspeccion_fallas/DisableInspector/5
        [HttpPost, ActionName("DisableInspector")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableInspectorConfirmedAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return RedirectToAction("NotFound", "Error");

            var result = await _userManager.RemoveFromRoleAsync(user.Id, TipoRoles.INSPECCION_REGISTRO);

            if (result.Succeeded)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Se deshabilitó correctamente el inspector.", TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Inspectores");
            }
            else
            {
                TempData["Mensaje"] = new MensajesSweetAlert("Hubo un error al deshabilitar al inspector.", TipoMensajesSweetAlerts.ERROR);
                return RedirectToAction("Inspectores");
            }

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
