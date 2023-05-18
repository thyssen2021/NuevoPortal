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
using System.Collections.Generic;
using Bitacoras.Util;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IdentitySample.Controllers
{
    [Authorize]
    public class UsersAdminController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();
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

                empleados empleados = db.empleados.FirstOrDefault(e => e.id == user.IdEmpleado);

                if (empleados != null)
                {
                    ViewBag.Empleado = empleados;
                }

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
        public async Task<ActionResult> Create(int? idEmpleado)
        {

            RegisterViewModel model = new RegisterViewModel();

            //Si tiene id_empleado, busca el empleado y lo manda a la vista
            if (idEmpleado != null)
            {
                var empleado = db.empleados.Find(idEmpleado);

                if (empleado != null)
                {
                    ViewBag.numEmpleado = empleado.id.ToString();
                    ViewBag.TipoU = "empleado";
                }
            }

            if (TieneRol(TipoRoles.USERS))
            {

                var s = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
                //obtiene el listado de roles de BD
                ViewBag.RolesBD = db.AspNetRoles.ToList();

                //Get the list of Roles
                ViewBag.RoleId = new SelectList(await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync(), "Name", "Name");
                //empleados
                ViewBag.EmpleadosList = ComboSelect.obtieneEmpleadosSelectList();

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
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, FormCollection collection, params string[] selectedRoles)
        {
            String tipoU = collection["tipoUsuario"];
            bool notificacion = Boolean.Parse(collection["notificacion"]);
            string username = String.Empty;
            string nombre = String.Empty;
            string apellidos = string.Empty;
            if (ModelState.IsValid)
            {
                //obtiene el empleado de la solicitud
                empleados empleados = db.empleados.FirstOrDefault(e => e.id == userViewModel.IdEmpleado);

                Random rd = new Random();
                int valor = rd.Next(0, 999);

                if (tipoU == "empleado")
                {

                    apellidos = empleados.apellido1;
                    if (apellidos.Length >= 8)
                    {
                        apellidos = apellidos.Substring(0, 7);
                    }
                    username = empleados.nombre[0] + apellidos + string.Format("{0:000}", valor);
                    //crea el usuario

                }
                else
                { //es otro..

                    nombre = userViewModel.Nombre;
                    if (nombre.Length >= 12)
                    {
                        nombre = nombre.Substring(0, 12);
                    }
                    username = nombre + string.Format("{0:000}", valor);
                }


                username = Clases.Util.UsoStrings.ReemplazaCaracteres(username);
                username = username.Replace(" ", String.Empty); //quita los espacios en blanco

                var user = new ApplicationUser { UserName = username.ToUpper(), Email = userViewModel.Email, Nombre = nombre, IdEmpleado = userViewModel.IdEmpleado, FechaCreacion = DateTime.Now };

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
                            ViewBag.RoleId = new SelectList(await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync(), "Name", "Name");
                            //obtiene el listado de roles de BD
                            ViewBag.RolesBD = db.AspNetRoles.ToList();
                            ViewBag.EmpleadosList = ComboSelect.obtieneEmpleadosSelectList();
                            ViewBag.TipoU = tipoU;
                            ViewBag.numEmpleado = userViewModel.IdEmpleado.ToString();
                            ViewBag.NombreE = nombre;
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    //obtiene el listado de roles de BD
                    ViewBag.RolesBD = db.AspNetRoles.ToList();
                    ViewBag.RoleId = new SelectList(await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync(), "Name", "Name");
                    ViewBag.EmpleadosList = ComboSelect.obtieneEmpleadosSelectList();
                    ViewBag.TipoU = tipoU;
                    ViewBag.numEmpleado = userViewModel.IdEmpleado.ToString();
                    ViewBag.NombreE = nombre;
                    return View();

                }
                string mensaje = TextoMensajesSweetAlerts.CREATE;



                if (notificacion)
                {
                    //envia correo electrónico
                    EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                    List<String> correos = new List<string>(); //correos TO

                    if (!String.IsNullOrEmpty(userViewModel.Email))
                        correos.Add(userViewModel.Email); //agrega correo del usuario

                    envioCorreo.SendEmailAsync(correos, "Bienvenido al portal thyssenkrupp.", envioCorreo.getBodyAccountWelcome(userViewModel));

                    mensaje += " Se envió email de notificación.";
                }

                try
                {
                    //busca si existe una solicitud de usuario para este empleado
                    var solicitud = db.IT_solicitud_usuarios.FirstOrDefault(x => x.id_empleado == userViewModel.IdEmpleado && x.estatus == IT_solicitud_usuario_Status.CREADO);

                    if (solicitud != null)
                    {
                        solicitud.fecha_cierre = DateTime.Now;
                        solicitud.estatus = IT_solicitud_usuario_Status.CERRADO;

                        db.Entry(solicitud).State = EntityState.Modified;
                        db.SaveChanges();

                        mensaje += " Se cerró una solicitud usuario.";
                    }
                }
                catch (Exception e)
                {

                    System.Diagnostics.Debug.WriteLine(e.Message);
                }

                //agrega el mensaje para sweetalert
                TempData["Mensaje"] = new MensajesSweetAlert(mensaje, TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(_roleManager.Roles.OrderBy(x => x.Name), "Name", "Name");
            //empleados
            //obtiene el listado de roles de BD
            ViewBag.RolesBD = db.AspNetRoles.ToList();
            ViewBag.EmpleadosList = ComboSelect.obtieneEmpleadosSelectList();
            ViewBag.TipoU = tipoU;
            ViewBag.numEmpleado = userViewModel.IdEmpleado.ToString();
            ViewBag.NombreE = nombre;
            ViewBag.TipoU = tipoU;
            return View();
        }

        //
        // GET: /Users/Edit/1
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            String tipoU = String.Empty;
            string username = String.Empty;
            string nombre = String.Empty;
            string apellidos = string.Empty;

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

                //obtien el tipo de usuario
                if (user.IdEmpleado > 0)
                {
                    empleados emp = null;
                    try
                    {
                        emp = db.empleados.Find(user.IdEmpleado);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.Print(e.Message);
                    }

                    if (emp.activo.HasValue && !emp.activo.Value)
                    {
                        ViewBag.Titulo = "¡Lo sentimos!¡No se puede editar el usuario de un empleado que ha sido dado de baja!";
                        ViewBag.Descripcion = "No se puede editar un usuario perteneciente a un usuario que ha sido dado de baja.";

                        return View("../Home/ErrorGenerico");
                    }
                    tipoU = "empleado";
                }
                else
                    tipoU = "otro";

                ViewBag.EmpleadosList = ComboSelect.obtieneEmpleadosSelectList();
                ViewBag.TipoU = tipoU;
                ViewBag.numEmpleado = user.IdEmpleado;
                ViewBag.NombreE = user.Nombre;
                //obtiene el listado de roles de BD
                ViewBag.RolesBD = db.AspNetRoles.ToList();
                return View(new EditUserViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Nombre = user.Nombre,
                    //Apellidos = user.Apellidos,
                    FechaCreacion = user.FechaCreacion,
                    RolesList = _roleManager.Roles.OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
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
            String tipoU = String.Empty;
            string username = String.Empty;
            string nombre = String.Empty;
            string apellidos = string.Empty;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }

                //user.UserName = editUser.Email;
                user.Email = editUser.Email;

                if (!String.IsNullOrEmpty(editUser.Nombre))
                    user.Nombre = editUser.Nombre.ToUpper();
                //  user.Apellidos = editUser.Apellidos.ToUpper(); 

                var userRoles = await _userManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await _userManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    ViewBag.EmpleadosList = ComboSelect.obtieneEmpleadosSelectList();
                    //obtiene el listado de roles de BD
                    ViewBag.RolesBD = db.AspNetRoles.ToList();
                    return View(new EditUserViewModel()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Nombre = user.Nombre,
                        //Apellidos = user.Apellidos,
                        FechaCreacion = user.FechaCreacion,
                        RolesList = _roleManager.Roles.OrderBy(x=>x.Name).ToList().Select(x => new SelectListItem()
                        {
                            Selected = userRoles.Contains(x.Name),
                            Text = x.Name,
                            Value = x.Name
                        })
                    });
                }
                result = await _userManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ViewBag.EmpleadosList = ComboSelect.obtieneEmpleadosSelectList();
                    ModelState.AddModelError("", result.Errors.First());
                    //obtiene el listado de roles de BD
                    ViewBag.RolesBD = db.AspNetRoles.ToList();
                    return View(new EditUserViewModel()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Nombre = user.Nombre,
                        //Apellidos = user.Apellidos,
                        FechaCreacion = user.FechaCreacion,
                        RolesList = _roleManager.Roles.OrderBy(x => x.Name).ToList().Select(x => new SelectListItem()
                        {
                            Selected = userRoles.Contains(x.Name),
                            Text = x.Name,
                            Value = x.Name
                        })
                    });
                }
                //agrega el mensaje para sweetalert
                TempData["Mensaje"] = new MensajesSweetAlert(TextoMensajesSweetAlerts.UPDATE, TipoMensajesSweetAlerts.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.EmpleadosList = ComboSelect.obtieneEmpleadosSelectList();

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
                empleados empleados = db.empleados.FirstOrDefault(e => e.id == user.IdEmpleado);

                if (empleados != null)
                {
                    ViewBag.Empleado = empleados;
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
                empleados empleados = db.empleados.FirstOrDefault(e => e.id == user.IdEmpleado);

                if (empleados != null)
                {
                    ViewBag.Empleado = empleados;
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
