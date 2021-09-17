using Clases.Util;
using Portal_2_0.Models;
using IdentitySample.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    [Authorize]
    public class UsersAdminController : BaseController
    {

        //
        // GET: /Users/
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            if (TieneRol(TipoRoles.USERS))
            {

                //mensaje en caso de crear, editar, etc
                if (TempData["Mensaje"] != null)
                {
                    ViewBag.MensajeAlert = TempData["Mensaje"];
                }

                return View(await _userManager.Users.ToListAsync());
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        //
        // GET: /Users/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (TieneRol(TipoRoles.USERS))
            {
                if (id == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return RedirectToAction("NotFound", "Error");

                ViewBag.RoleNames = await _userManager.GetRolesAsync(user.Id);

                return View(user);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        //
        // GET: /Users/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {

            if (TieneRol(TipoRoles.USERS))
            {
                //Get the list of Roles
                ViewBag.RoleId = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }

        //
        // POST: /Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                //obtiene número aleatorio
                Random rd = new Random();
                int valor = rd.Next(0, 999);

                string apellidos = userViewModel.Apellidos.Replace(" ", "");

                if (apellidos.Length >= 8)
                {
                    apellidos = apellidos.Substring(0, 7);
                }

                string username = userViewModel.Nombre[0] + apellidos + string.Format("{0:000}", valor);

                username = Clases.Util.UsoStrings.ReemplazaCaracteres(username);

                var user = new ApplicationUser { UserName = username.ToUpper(), Email = userViewModel.Email, Nombre = userViewModel.Nombre.ToUpper(), Apellidos = userViewModel.Apellidos.ToUpper(), FechaCreacion = DateTime.Now };

                var adminresult = await _userManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await _userManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(_roleManager.Roles, "Name", "Name");
                    return View();

                }

                //agrega el mensaje para sweetalert
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.CREATE, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(_roleManager.Roles, "Name", "Name");
            return View();
        }

        //
        // GET: /Users/Edit/1
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (TieneRol(TipoRoles.USERS))
            {
                if (id == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }

                var userRoles = await _userManager.GetRolesAsync(user.Id);

                return View(new EditUserViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Nombre = user.Nombre,
                    Apellidos = user.Apellidos,
                    FechaCreacion = user.FechaCreacion,
                    RolesList = _roleManager.Roles.ToList().Select(x => new SelectListItem()
                    {
                        Selected = userRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    })
                });
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id,Nombre,Apellidos")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }

                //user.UserName = editUser.Email;
                user.Email = editUser.Email;
                user.Nombre = editUser.Nombre.ToUpper();
                user.Apellidos = editUser.Apellidos.ToUpper(); ;


                var userRoles = await _userManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await _userManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await _userManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                //agrega el mensaje para sweetalert
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Algo falló.");
            return View();
        }

        //
        // GET: /Users/Block/5
        [HttpGet]
        public async Task<ActionResult> Block(string id)
        {
            if (TieneRol(TipoRoles.USERS))
            {
                if (id == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }
                return View(user);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        //
        // POST: /Users/Block/5
        [HttpPost]
        [ActionName("Block")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (TieneRol(TipoRoles.USERS))
            {

                if (ModelState.IsValid)
                {
                    if (id == null)
                    {
                        return RedirectToAction("NotFound", "Error");
                    }

                    var user = await _userManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        return RedirectToAction("NotFound", "Error");
                    }

                    //Bloqueo del usuario
                    var result = await _userManager.SetLockoutEndDateAsync(id, DateTimeOffset.Now.AddYears(10));
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                    //agrega el mensaje para sweetalert
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ha bloqueado el usuario " + user.UserName + " correctamente.", TipoMensajesSweetAlerts.SUCCESS);

                    return RedirectToAction("Index");
                }
                return View();

            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        //
        // GET: /Users/UnBlock/5
        [HttpGet]
        public async Task<ActionResult> UnBlock(string id)
        {
            if (TieneRol(TipoRoles.USERS))
            {
                if (id == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }
                return View(user);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }

        }


        //
        // POST: /Users/UnBlock/5
        [HttpPost]
        [ActionName("UnBlock")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UnBlockConfirmed(string id)
        {
            if (TieneRol(TipoRoles.USERS))
            {
                if (ModelState.IsValid)
                {
                    if (id == null)
                    {
                        return RedirectToAction("NotFound", "Error");
                    }

                    var user = await _userManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        return RedirectToAction("NotFound", "Error");
                    }

                    //Desbloqueo del usuario con una fecha de desbloqueo anterior
                    var result = await _userManager.SetLockoutEndDateAsync(id, DateTimeOffset.Now.AddDays(-2));
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                    //agrega el mensaje para sweetalert
                    TempData["Mensaje"] = new MensajesSweetAlert("Se ha vuelto a habilitar el usuario " + user.UserName + " correctamente.", TipoMensajesSweetAlerts.SUCCESS);                   
                    return RedirectToAction("Index");
                }
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }
    }
}
