using Portal_2_0.Models;
using System.Web.Mvc;
using System.Linq;
using Clases.Util;

namespace IdentitySample.Controllers
{
    [Authorize]

    public class HomeController : BaseController
    {

        private Portal_2_0Entities db = new Portal_2_0Entities();

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

        [AllowAnonymous]
        public ActionResult Menu(string nombre)
        {
            if (!string.IsNullOrEmpty(nombre))
                nombre = UsoStrings.ReemplazaCaracteres(nombre).ToUpper();
            
            var listado = db.menu_item.ToList()
                .Where(p => (string.IsNullOrEmpty(nombre) || UsoStrings.ContainsIgnoreCase(UsoStrings.ReemplazaCaracteres(p.titulo), nombre)|| UsoStrings.ContainsIgnoreCase(UsoStrings.ReemplazaCaracteres(p.descripcion), nombre))
                && p.activo)
                .OrderBy(x => x.prioridad);
            return View(listado.ToList());

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
