using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    public class ErrorController : BaseController
    {
        public ViewResult NotFound()
        {
            Response.StatusCode = 404;  //aqui podrías trabajarlo dinámicamente
            return View("NotFound");
        }

        public ViewResult BadRequest()
        {
            Response.StatusCode = 400;  //aqui podrías trabajarlo dinámicamente
            return View("BadRequest");
        }

    }
}