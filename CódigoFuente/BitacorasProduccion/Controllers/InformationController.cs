using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    public class InformationController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();
        // GET: Information

        [ActionName("BonoTPU")]
        public ActionResult Index()
        {
            try
            {

                string nombreEquipo = String.Empty;
                // string nombreUsuario =  String.Empty;

                nombreEquipo = DetermineCompName(Request.UserHostName);

                //if (!String.IsNullOrEmpty(nombreEquipo))
                //    ViewBag.NombreEquipo = nombreEquipo;

                //if (!String.IsNullOrEmpty(nombreUsuario))
                //    ViewBag.NombreUsuario = nombreUsuario;

                //crear objeto de base de datos enviar a guardar en el log

                if (String.IsNullOrEmpty(nombreEquipo))
                    nombreEquipo = Request.UserHostName;

                log_acceso_email log = new log_acceso_email
                {
                    fecha = DateTime.Now,
                    nombre_equipo = nombreEquipo,
                    //  usuario = nombreUsuario
                };


                db.log_acceso_email.Add(log);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                ////do nothing
                ViewBag.Error = e.Message;
                //System.Diagnostics.Debug.Print(e.Message);
            }
            return View();
        }

        public ActionResult Log(string usuario, string nombre_equipo, int registros_por_pagina = 20, int pagina = 1)
        {

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }

            var cantidadRegistrosPorPagina = registros_por_pagina; // parámetro

            var listado = db.log_acceso_email
                   .Where(x => (x.usuario == usuario || String.IsNullOrEmpty(usuario)) && (x.nombre_equipo == nombre_equipo || String.IsNullOrEmpty(nombre_equipo)))
                   .OrderBy(x => x.id)
                   .Skip((pagina - 1) * cantidadRegistrosPorPagina)
                  .Take(cantidadRegistrosPorPagina).ToList();

            var totalDeRegistros = db.log_acceso_email
                  .Where(x => (x.usuario == usuario || String.IsNullOrEmpty(usuario)) && (x.nombre_equipo == nombre_equipo || String.IsNullOrEmpty(nombre_equipo)))
                .Count();

            //para paginación

            System.Web.Routing.RouteValueDictionary routeValues = new System.Web.Routing.RouteValueDictionary();
            routeValues["usuario"] = usuario;
            routeValues["nombre_equipo"] = nombre_equipo;
            routeValues["registros_por_pagina"] = registros_por_pagina;

            Paginacion paginacion = new Paginacion
            {
                PaginaActual = pagina,
                TotalDeRegistros = totalDeRegistros,
                RegistrosPorPagina = cantidadRegistrosPorPagina,
                ValoresQueryString = routeValues
            };


            List<string> usuariosList = db.log_acceso_email.Select(x => x.usuario).Distinct().ToList();
            //crea un Select  list para el estatus
            List<SelectListItem> newListUsuarios = new List<SelectListItem>();

            foreach (string item in usuariosList)
            {
                newListUsuarios.Add(new SelectListItem()
                {
                    Text = item,
                    Value = item
                });
            }

            List<string> equiposList = db.log_acceso_email.Select(x => x.nombre_equipo).Distinct().ToList();
            //crea un Select  list para el estatus
            List<SelectListItem> newListequipos = new List<SelectListItem>();

            foreach (string item in equiposList)
            {
                newListequipos.Add(new SelectListItem()
                {
                    Text = item,
                    Value = item
                });
            }

            SelectList selectListItems_Usuarios = new SelectList(newListUsuarios, "Value", "Text", usuario);
            SelectList selectListItems_Equipos = new SelectList(newListequipos, "Value", "Text", nombre_equipo);

            ViewBag.usuario = AddFirstItem(selectListItems_Usuarios, textoPorDefecto: "-- Todos --");
            ViewBag.nombre_equipo = AddFirstItem(selectListItems_Equipos, textoPorDefecto: "-- Todos --");

            ViewBag.Paginacion = paginacion;

            var upgrade_revision = listado;
            return View(upgrade_revision.ToList());
        }

        public static string DetermineCompName(string IP)
        {
            try
            {
                System.Net.IPAddress myIP = System.Net.IPAddress.Parse(IP);
                System.Net.IPHostEntry GetIPHost = System.Net.Dns.GetHostEntry(myIP);
                List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
                return compName.First();
            }
            catch
            {
                return null;
            }

        }


    }
}
