using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class SCDM_versiones_herramienta_excelController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: SCDM_versiones_herramienta_excel
        public ActionResult Index()
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            var sCDM_versiones_herramienta_excel = db.SCDM_versiones_herramienta_excel.OrderByDescending(x=> x.id);
            return View(sCDM_versiones_herramienta_excel.ToList());
        }

        // GET: SCDM_versiones_herramienta_excel/Create
        public ActionResult Create()
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            var empleado = obtieneEmpleadoLogeado();

            ViewBag.id_archivo = new SelectList(db.biblioteca_digital, "Id", "Nombre");
            ViewBag.id_responsable = new SelectList(db.empleados.Where(x => x.id == empleado.id), nameof(empleado.id), nameof(empleado.ConcatNombre));

            SCDM_versiones_herramienta_excel modelo = new SCDM_versiones_herramienta_excel
            {
                id_responsable = empleado.id,
                fecha_liberacion = DateTime.Now,
            };

            return View(modelo);
        }

        // POST: SCDM_versiones_herramienta_excel/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SCDM_versiones_herramienta_excel sCDM_versiones_herramienta_excel, FormCollection collection)
        {
            if (sCDM_versiones_herramienta_excel.PostedFileBase == null)
                ModelState.AddModelError("", "No se agregó archivo.");

            var empleado = obtieneEmpleadoLogeado();

            //verifica si el tamaño del archivo es OT 1
            if (sCDM_versiones_herramienta_excel.PostedFileBase != null && sCDM_versiones_herramienta_excel.PostedFileBase.InputStream.Length > 10485760)
                ModelState.AddModelError("", "Sólo se permiten archivos menores a 10MB: " + sCDM_versiones_herramienta_excel.PostedFileBase.FileName + ".");
            else if (sCDM_versiones_herramienta_excel.PostedFileBase != null)
            { //verifica la extensión del archivo
                string extension = Path.GetExtension(sCDM_versiones_herramienta_excel.PostedFileBase.FileName);
                if (extension.ToUpper() != ".XLSM"   //si contiene una extensión inválida
                               )
                    ModelState.AddModelError("", "No se permiten archivos con extensión " + extension + ": " + sCDM_versiones_herramienta_excel.PostedFileBase.FileName + ".");
                else
                { //si la extension y el tamaño son válidos
                    String nombreArchivo = sCDM_versiones_herramienta_excel.PostedFileBase.FileName;

                    //recorta el nombre del archivo en caso de ser necesario
                    if (nombreArchivo.Length > 80)
                        nombreArchivo = nombreArchivo.Substring(0, 78 - extension.Length) + extension;

                    //lee el archivo en un arreglo de bytes
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(sCDM_versiones_herramienta_excel.PostedFileBase.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(sCDM_versiones_herramienta_excel.PostedFileBase.ContentLength);
                    }

                    //genera el archivo de biblioce digital
                    biblioteca_digital archivo = new biblioteca_digital
                    {
                        Nombre = nombreArchivo,
                        MimeType = UsoStrings.RecortaString(sCDM_versiones_herramienta_excel.PostedFileBase.ContentType, 80),
                        Datos = fileData
                    };
                    sCDM_versiones_herramienta_excel.biblioteca_digital = archivo;
                }
            }


            if (ModelState.IsValid)
            {

                db.SCDM_versiones_herramienta_excel.Add(sCDM_versiones_herramienta_excel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_archivo = new SelectList(db.biblioteca_digital, "Id", "Nombre", sCDM_versiones_herramienta_excel.id_archivo);
            ViewBag.id_responsable = new SelectList(db.empleados.Where(x => x.id == empleado.id), nameof(empleado.id), nameof(empleado.ConcatNombre), sCDM_versiones_herramienta_excel.id_responsable);
            return View(sCDM_versiones_herramienta_excel);
        }

        public ActionResult Descargar(int? id_version)
        {
            if (!TieneRol(TipoRoles.SCDM_MM_ADMINISTRADOR))
                return View("../Home/ErrorPermisos");

            SCDM_versiones_herramienta_excel version = db.SCDM_versiones_herramienta_excel.Find(id_version);

            byte[] stream = version.biblioteca_digital.Datos;

            var cd = new System.Net.Mime.ContentDisposition
            {

                // for example foo.bak
                //FileName = planta.descripcion + "_" + produccion_Lineas.linea + "_" + fecha_inicial + "_" + dateFinal.ToString("yyyy-MM-dd") + ".xlsx",
                FileName = Server.UrlEncode("AdministraciónSolicitudes_v_" +  version.version + ".xlsm"),

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(stream, "application/vnd.ms-excel");

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
