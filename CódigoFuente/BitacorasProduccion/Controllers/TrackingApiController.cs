using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [RoutePrefix("api/tracking")]
    public class TrackingApiController : ApiController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // La ruta para este método será: POST api/trackingapi/logclick
        [HttpPost]
        [Route("logclick")]
        public IHttpActionResult LogClick([FromBody] string token)
        {
            // Valida que el token sea del formato correcto
            if (!Guid.TryParse(token, out Guid trackingToken))
            {
                return BadRequest(); // Devuelve un error si el token no es válido
            }

            try
            {
                var logEntry = db.EmailTrackingLog.FirstOrDefault(l => l.TrackingToken == trackingToken);

                // Si encontramos el registro y es la primera vez que se hace clic
                if (logEntry != null && !logEntry.FechaApertura.HasValue)
                {
                    logEntry.FechaApertura = DateTime.Now;
                    logEntry.FechaClic = DateTime.Now;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                // Si hay un error de base de datos, no hacemos nada.
                // No queremos dar información sensible al exterior.
            }

            // Siempre devolvemos "OK" para no revelar si el token existía o no.
            return Ok();
        }

        [HttpGet]
        [Route("status")] // <-- La ruta será: api/tracking/status
        [AllowAnonymous] // Permite que se llame sin necesidad de autenticación
        public IHttpActionResult GetStatus()
        {
            return Ok("API de Tracking funcionando correctamente. Hora del servidor: " + DateTime.Now.ToString("g"));
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