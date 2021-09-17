using System.Web.Mvc;

namespace IdentitySample.Controllers
{ 
    [Authorize]

    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ErrorPermisos()
        {
            return View();
        }
    }
}
