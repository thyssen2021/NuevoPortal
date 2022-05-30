using Clases.Util;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (!String.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Aqui realizar conexion a BD para consultar el username
            string username = Clases.DBUtil.UsuariosDBUtil.ObtieneUsername(model.Email);

            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(username, model.Password, false, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    //inserta en log de inicio de sesion
                    AspNetUsers usuarioASP = db.AspNetUsers.FirstOrDefault(x => x.UserName == username);

                    //guarda el inicio de sesión en el log
                    if (usuarioASP != null)
                    {
                        log_inicio_sesion logLogin = new log_inicio_sesion
                        {
                            id_usuario = usuarioASP.Id,
                            fecha = DateTime.Now
                        };
                        db.log_inicio_sesion.Add(logLogin);
                        db.SaveChanges();
                    }

                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    if (String.IsNullOrEmpty(username))
                        ModelState.AddModelError("", "No existe el usuario.");
                    else
                        ModelState.AddModelError("", "La contraseña ingresada no es válida.");
                    return View(model);
            }
        }


        ////
        //// GET: /Account/VerifyCode
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
        //    if (user != null)
        //    {
        //        ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(model.ReturnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid code.");
        //            return View(model);
        //    }
        //}

        ////
        //// GET: /Account/Register
        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {                
        //        //obtiene número aleatorio
        //        Random rd = new Random();
        //        int valor = rd.Next(0, 999);

        //        string apellidos = model.Apellidos.Replace(" ", "");

        //        if (apellidos.Length >= 8) {
        //            apellidos = apellidos.Substring(0, 7);
        //        }

        //        string username = model.Nombre[0] + apellidos + string.Format("{0:000}", valor);

        //        var user = new ApplicationUser { UserName = username.ToUpper(), Email = model.Email, Nombre = model.Nombre.ToUpper(), Apellidos = model.Apellidos.ToUpper(), FechaCreacion = DateTime.Now };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            //var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
        //            //ViewBag.Link = callbackUrl;
        //            return View("Register");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// GET: /Account/ConfirmEmail
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}


        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {

            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var aspNetUsers = db.AspNetUsers.FirstOrDefault(x => x.Email == model.Email);

            if (aspNetUsers == null)
                ModelState.AddModelError("", "No existe ningún usuario registrado con ese correo electrónico.");


            if (ModelState.IsValid)
            {

                var user = await UserManager.FindByIdAsync(aspNetUsers.Id);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPasswordAnonymous", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                //envia correo electronico
                EnvioCorreoElectronico envioCorreo = new EnvioCorreoElectronico();
                List<String> correos = new List<string>(); //correos TO

                if (!String.IsNullOrEmpty(model.Email))
                    correos.Add(model.Email); //agrega correo del usuario

                envioCorreo.SendEmailAsync(correos, "Solicitud de restablecimiento de contraseña.", envioCorreo.getBodyAccountResetPassword(callbackUrl));


                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        // GET: /Account/ResetPasswordAnonymous
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPasswordAnonymous(string userId, string code)
        {

            if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(code))
            {
                return RedirectToAction("NotFound", "Error");
            }
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            ResetPasswordViewModel modelo = new ResetPasswordViewModel();
            modelo.Email = user.Email;
            modelo.Code = code;
            return View("ResetPassword", modelo);

        }

        //
        // POST: /Account/ResetPasswordAnonymous
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPasswordAnonymous(ResetPasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("No se ha podido restablecer la contraseña, el email no existe.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index", "UsersAdmin");
            }

            //genera el token
            //var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }
            ModelState.AddModelError("", "No se ha podido restablecer la contraseña. Sí el token no es válido genere nuevamente el enlace de restablecimiento.");
            AddErrors(result);
            return View("ResetPassword", model);
        }

        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }



        // GET: /Account/ResetPassword
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> ResetPassword(string id)
        {
            if (UserManager.IsInRoleAsync(User.Identity.GetUserId(), TipoRoles.USERS).Result)
            {
                if (id == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }

                ResetPasswordViewModel modelo = new ResetPasswordViewModel();
                modelo.Email = user.Email;
                return View(modelo);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }


        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["Mensaje"] = new MensajesSweetAlert("No se ha podido restablecer la contraseña, el email no existe.", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index", "UsersAdmin");
            }

            //genera el token
            var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

            var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
            if (result.Succeeded)
            {
                //agrega el mensaje para sweetalert
                TempData["Mensaje"] = new MensajesSweetAlert("Se ha restablecido la contraseña para el email " + model.Email + ".", TipoMensajesSweetAlerts.SUCCESS);

                return RedirectToAction("Index", "UsersAdmin");
            }
            AddErrors(result);
            return View();
        }

        //


        //
        // POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        ////
        //// GET: /Account/SendCode
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // Generate the token and send it
        //    if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //    {
        //        return View("Error");
        //    }
        //    return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //}

        ////
        //// GET: /Account/ExternalLoginCallback
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
        //        case SignInStatus.Failure:
        //        default:
        //            // If the user does not have an account, then prompt the user to create an account
        //            ViewBag.ReturnUrl = returnUrl;
        //            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //    }
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //
        // POST: /Account/LogOff
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        // GET: /Account/CerrarSession
        public ActionResult CerrarSesion()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult SolicitarUsuario()
        {
            ViewBag.id_planta = AddFirstItem(new SelectList(db.plantas.Where(x => x.activo), "clave", "descripcion"));
            ViewBag.id_empleado = AddFirstItem(new SelectList(db.empleados.Where(x => x.activo.HasValue && x.activo.Value), "id", "ConcatNumEmpleadoNombre"));

            return View();

        }

        ////
        //// GET: /Account/ExternalLoginFailure
        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}