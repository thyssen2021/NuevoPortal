using Portal_2_0.Models;
using System.Web.Mvc;
using System.Linq;
using Clases.Util;
using Bitacoras.Util;

namespace IdentitySample.Controllers
{
    [Authorize]

    public class HomeController : BaseController
    {

        private Portal_2_0Entities db = new Portal_2_0Entities();

        [HttpGet]
        public ActionResult Index()
        {

            #region IT matriz de requerimientos

            //Envia los datos para los widget
            if (TieneRol(TipoRoles.IT_MATRIZ_REQUERIMIENTOS_CERRAR))
            {
                var totalPendientes = db.IT_matriz_requerimientos
                  .Where(x => (x.estatus == IT_MR_Status.EN_PROCESO || x.estatus == IT_MR_Status.ENVIADO_A_IT))
                  .Count();

                var empleado = obtieneEmpleadoLogeado();

                var totalPendientesPropias = db.IT_matriz_requerimientos
                  .Where(x => (x.estatus == IT_MR_Status.EN_PROCESO || x.estatus == IT_MR_Status.ENVIADO_A_IT)
                        && (x.IT_matriz_asignaciones.Any(y => y.activo && y.id_sistemas == empleado.id))
                  )
                  .Count();

                ViewBag.PendientesDepto = totalPendientes;
                ViewBag.totalPendientesPropias = totalPendientesPropias;
            }

            #endregion
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
