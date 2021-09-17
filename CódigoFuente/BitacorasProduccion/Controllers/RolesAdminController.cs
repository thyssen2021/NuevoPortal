using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using Portal_2_0.Models;
using Clases.Util;

namespace IdentitySample.Controllers
{
    [Authorize]
    public class RolesAdminController : BaseController
    {
        //
        // GET: /Roles/
        [HttpGet]
        public ActionResult Index()
        {
            if (TieneRol(TipoRoles.USERS))
            {
                return View(_roleManager.Roles);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        //
        // GET: /Roles/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (TieneRol(TipoRoles.USERS))
            {
                if (id == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }
                var role = await _roleManager.FindByIdAsync(id);

                if (role == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }

                // Get the list of Users in this Role
                var users = new List<ApplicationUser>();

                // Get the list of Users in this Role
                foreach (var user in _userManager.Users.ToList())
                {
                    if (await _userManager.IsInRoleAsync(user.Id, role.Name))
                    {
                        users.Add(user);
                    }
                }

                ViewBag.Users = users;
                ViewBag.UserCount = users.Count();
                return View(role);
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

    }
}