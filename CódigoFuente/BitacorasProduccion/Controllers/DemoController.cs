using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class DemoController : Controller
    {
        // GET: Demo
        public ActionResult ListaModeloPoliza()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
    }
}