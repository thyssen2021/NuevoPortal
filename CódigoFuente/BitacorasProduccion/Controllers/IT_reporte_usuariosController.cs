using Bitacoras.Util;
using Clases.Util;
using DocumentFormat.OpenXml.Bibliography;
using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static iText.Kernel.Pdf.Colorspace.PdfSpecialCs;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class IT_reporte_usuariosController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();
        //string para heads de tables
        private string headExport4Import = @"<thead>
                                          <tr>
                                               <th>userName (8ID)</th>
                                                <th>tksir</th>
                                                <th>tktitle</th>
                                                <th>tknameprefix</th>
                                                <th>lastName</th>
                                                <th>firstName</th>
                                                <th>tkbirth</th>
                                                <th>tksex</th>
                                                <th>tkstreet</th>
                                                <th>tkpostalcode</th>
                                                <th>tkpostaladdress</th>
                                                <th>tkaddaddon</th>
                                                <th>tkfedst</th>
                                                <th>tkcountry</th>
                                                <th>tkcountrykey</th>
                                                <th>tknationality</th>
                                                <th>tkpreflang</th>
                                                <th>tkempno</th>
                                                <th>tkfkz6</th>
                                                <th>tkfkzext</th>
                                                <th>tkuniqueid</th>
                                                <th>tkpstatus</th>
                                                <th>tkcostcenter</th>
                                                <th>tkdepartment</th>
                                                <th>tkfunction</th>
                                                <th>tkorgstreet</th>
                                                <th>tkorgpostalcode</th>
                                                <th>tkorgpostaladdress</th>
                                                <th>tkorgaddonaddr</th>
                                                <th>tkorgfedst</th>
                                                <th>tkorgcountry</th>
                                                <th>tkorgcountrykey</th>
                                                <th>tkapsite</th>
                                                <th>tkorgkey</th>
                                                <th>tkbuilding</th>
                                                <th>email</th>
                                                <th>tkareacode</th>
                                                <th>tkphoneext</th>
                                                <th>tkorgfax</th>
                                                <th>tkmobile</th>
                                                <th>tkgodfather</th>
                                                <th>tkprefdelmethod</th>
                                                <th>tkedateorg</th>
                                                <th>tkedatetrust</th>
                                                <th>tkldateorg</th>
                                                <th>tklreason</th>
                                                <th>shares</th>
                                                <th>supervisoryboardelection</th>
                                                <th>tkbkz</th>
                                                <th>tkinside</th>
                                          </tr>
                                      </thead>";
        private string headExportUsers = @"<thead>
                                          <tr>
                                             <th>8ID</th>
                                             <th>userPrincipalName</th>
                                            <th>displayName</th>
                                            <th>surname</th>
                                            <th>mail</th>
                                            <th>givenName</th>
                                            <th>id</th>
                                            <th>userType</th>
                                            <th>jobTitle</th>
                                            <th>department</th>
                                            <th>accountEnabled</th>
                                            <th>usageLocation</th>
                                            <th>streetAddress</th>
                                            <th>state</th>
                                            <th>country</th>
                                            <th>officeLocation</th>
                                            <th>city</th>
                                            <th>postalCode</th>
                                            <th>telephoneNumber</th>
                                            <th>mobilePhone</th>
                                            <th>alternateEmailAddress</th>
                                            <th>ageGroup</th>
                                            <th>consentProvidedForMinor</th>
                                            <th>legalAgeGroupClassification</th>
                                            <th>companyName</th>
                                            <th>creationType</th>
                                            <th>directorySynced</th>
                                            <th>invitationState</th>
                                            <th>identityIssuer</th>
                                            <th>createdDateTime</th>
                                          </tr>
                                      </thead>";
        private string headUsuariosActivos = @"<thead>
                                          <tr>
                                            <th>8ID</th>
                                           <th>Número</th>
                                            <th>Apellido Paterno</th>
                                            <th>Apellido Materno</th>
                                            <th>Nombre(s)</th>
                                            <th>Antigüedad</th>
                                            <th>Puesto</th>
                                            <th>Departamento</th>
                                            <th>Planta</th>
                                            <th>Área</th>
                                            <th>Departamento</th>
                                            <th>Género</th>                                           
                                          </tr>
                                      </thead>";

        // GET: IT_reporte_usuarios
        public ActionResult Index(string reporte1, string reporte2)
        {
            if (!TieneRol(TipoRoles.IT_INVENTORY))
                return View("../Home/ErrorPermisos");

            //mensaje en caso de crear, editar, etc
            if (TempData["Mensaje"] != null)
            {
                ViewBag.MensajeAlert = TempData["Mensaje"];
            }



            string table1 = string.Empty, table2 = string.Empty, table1no2 = string.Empty, table2no1 = string.Empty;
            string tituloTable1 = string.Empty;
            string tituloTable2 = string.Empty;

            var fechaExpor4Import = db.IT_Export4Import.FirstOrDefault();
            var fechaTextExpor4Import = string.Empty;

            var fechaExportUsers = db.IT_ExportUsers.FirstOrDefault();
            var fechaTextExportUsers = string.Empty;

            var fechaIT_UsuariosActivos = db.IT_UsuariosActivos.FirstOrDefault();
            var fechaTextIT_UsuariosActivost = string.Empty;

            if (fechaExpor4Import != null)
                fechaTextExpor4Import = fechaExpor4Import.updateTime.ToShortDateString();
            if (fechaExportUsers != null)
                fechaTextExportUsers = fechaExportUsers.updateTime.ToShortDateString();
            if (fechaIT_UsuariosActivos != null)
                fechaTextIT_UsuariosActivost = fechaIT_UsuariosActivos.updateTime.ToShortDateString();

            //crea select list para tipo de solicitud
            List<SelectListItem> listReportes = new List<SelectListItem> {
                    new SelectListItem()
                    {
                        Text = "Export4Import "+"("+fechaTextExpor4Import+")",
                        Value = "EXPORT4IMPORT"
                    },
                     new SelectListItem()
                    {
                        Text = "Export Users "+"("+fechaTextExportUsers+")",
                        Value = "EXPORTUSERS"
                    },
                        new SelectListItem()
                    {
                        Text = "Usuarios Activos "+"("+fechaTextIT_UsuariosActivost+")",
                        Value = "USUARIOSACTIVOS"
                    },
                };

            //tabla1
            switch (reporte1)
            {
                case "EXPORT4IMPORT":
                    table1 = GetHTMLTable(reporte1, export4ImportsList: db.IT_Export4Import.ToList());
                    tituloTable1 = "Export4Import";
                    break;
                case "EXPORTUSERS":
                    table1 = GetHTMLTable(reporte1, exportUsersList: db.IT_ExportUsers.ToList());
                    tituloTable1 = "ExportUser";
                    break;
                case "USUARIOSACTIVOS":
                    table1 = GetHTMLTable(reporte1, usuariosActivosList: db.IT_UsuariosActivos.ToList());
                    tituloTable1 = "Usuarios Activos";
                    break;
            }

            //tabla2
            switch (reporte2)
            {
                case "EXPORT4IMPORT":
                    table2 = GetHTMLTable(reporte2, export4ImportsList: db.IT_Export4Import.ToList());
                    tituloTable2 = "Export4Import";
                    break;
                case "EXPORTUSERS":
                    table2 = GetHTMLTable(reporte2, exportUsersList: db.IT_ExportUsers.ToList());
                    tituloTable2 = "ExportUser";
                    break;
                case "USUARIOSACTIVOS":
                    table2 = GetHTMLTable(reporte2, usuariosActivosList: db.IT_UsuariosActivos.ToList());
                    tituloTable2 = "Usuarios Activos";
                    break;
            }

            //Comparativa en tabla 1, pero no en tabla 2            

            if ((reporte1 == "EXPORT4IMPORT" && reporte2 == "EXPORTUSERS") || (reporte2 == "EXPORT4IMPORT" && reporte1 == "EXPORTUSERS"))
            {
                var datos = db.IT_Export4Import.Where(x => !db.IT_ExportUsers.Any(y => y.C8ID == x.C8ID)).ToList();
                var datos2 = db.IT_ExportUsers.Where(x => !db.IT_Export4Import.Any(y => y.C8ID == x.C8ID)).ToList();

                if (reporte1 == "EXPORT4IMPORT" && reporte2 == "EXPORTUSERS")
                {
                    table1no2 = GetHTMLTable(reporte1, export4ImportsList: datos);
                    table2no1 = GetHTMLTable(reporte2, exportUsersList: datos2);
                }
                else {
                    table1no2 = GetHTMLTable(reporte1, exportUsersList: datos2);
                    table2no1 = GetHTMLTable(reporte2, export4ImportsList: datos);
                }
            }

            if ((reporte1 == "EXPORT4IMPORT" && reporte2 == "USUARIOSACTIVOS") || (reporte2 == "EXPORT4IMPORT" && reporte1 == "USUARIOSACTIVOS"))
            {
                var datos = db.IT_Export4Import.Where(x => !db.IT_UsuariosActivos.Any(y => y.C8ID == x.C8ID || x.tkempno == y.Numero)).ToList();
                var datos2 = db.IT_UsuariosActivos.Where(x => !db.IT_Export4Import.Any(y => y.C8ID == x.C8ID || y.tkempno == x.Numero)).ToList();

                if (reporte1 == "EXPORT4IMPORT" && reporte2 == "USUARIOSACTIVOS")
                {
                    table1no2 = GetHTMLTable(reporte1, export4ImportsList: datos);
                    table2no1 = GetHTMLTable(reporte2, usuariosActivosList: datos2);
                }
                else {
                    table1no2 = GetHTMLTable(reporte1, usuariosActivosList: datos2);
                    table2no1 = GetHTMLTable(reporte2, export4ImportsList: datos);
                }
            }

            if ((reporte1 == "EXPORTUSERS" && reporte2 == "USUARIOSACTIVOS") || (reporte2 == "EXPORTUSERS" && reporte1 == "USUARIOSACTIVOS"))
            {
                var datos = db.IT_ExportUsers.Where(x => !db.IT_UsuariosActivos.Any(y => y.C8ID == x.C8ID)).ToList();
                var datos2 = db.IT_UsuariosActivos.Where(x => !db.IT_Export4Import.Any(y => y.C8ID == x.C8ID)).ToList();

                if (reporte1 == "EXPORTUSERS" && reporte2 == "USUARIOSACTIVOS")
                {
                    table1no2 = GetHTMLTable(reporte1, exportUsersList: datos);
                    table2no1 = GetHTMLTable(reporte2, usuariosActivosList: datos2);
                }
                else
                {
                    table1no2 = GetHTMLTable(reporte1, usuariosActivosList: datos2);
                    table2no1 = GetHTMLTable(reporte2, exportUsersList: datos);
                }
            }


            //list para tipo
            ViewBag.Reporte1 = AddFirstItem(new SelectList(listReportes, "Value", "Text", reporte1), textoPorDefecto: "-- Seleccionar --");
            ViewBag.Reporte2 = AddFirstItem(new SelectList(listReportes, "Value", "Text", reporte2), textoPorDefecto: "-- Seleccionar --");

            ViewBag.Table1HTML = table1;
            ViewBag.TituloTable1 = tituloTable1;
            ViewBag.Table2HTML = table2;
            ViewBag.TituloTable2 = tituloTable2;
            ViewBag.Table3HTML = table1no2;
            ViewBag.Table4HTML = table2no1;

            return View();
        }

        [NonAction]
        public String GetHTMLTable(string tipoTable, List<IT_Export4Import> export4ImportsList = null, List<IT_ExportUsers> exportUsersList = null, List<IT_UsuariosActivos> usuariosActivosList = null)
        {
            string table = string.Empty;

            //genera codigo html para cada table
            if (tipoTable == "EXPORT4IMPORT" && export4ImportsList != null)
            {
                table += headExport4Import;
                table += "<tbody>";

                foreach (var item in export4ImportsList)
                {
                    table += "<tr>";
                    table += "<td>" + item.C8ID + "</td>";
                    table += "<td>" + item.tksir + "</td>";
                    table += "<td>" + item.tktitle + "</td>";
                    table += "<td>" + item.tknameprefix + "</td>";
                    table += "<td>" + item.lastName + "</td>";
                    table += "<td>" + item.firstName + "</td>";
                    table += "<td>" + item.tkbirth + "</td>";
                    table += "<td>" + item.tksex + "</td>";
                    table += "<td>" + item.tkstreet + "</td>";
                    table += "<td>" + item.tkpostalcode + "</td>";
                    table += "<td>" + item.tkpostaladdress + "</td>";
                    table += "<td>" + item.tkaddaddon + "</td>";
                    table += "<td>" + item.tkfedst + "</td>";
                    table += "<td>" + item.tkcountry + "</td>";
                    table += "<td>" + item.tkcountrykey + "</td>";
                    table += "<td>" + item.tknationality + "</td>";
                    table += "<td>" + item.tkpreflang + "</td>";
                    table += "<td>" + item.tkempno + "</td>";
                    table += "<td>" + item.tkfkz6 + "</td>";
                    table += "<td>" + item.tkfkzext + "</td>";
                    table += "<td>" + item.tkuniqueid + "</td>";
                    table += "<td>" + item.tkpstatus + "</td>";
                    table += "<td>" + item.tkcostcenter + "</td>";
                    table += "<td>" + item.tkdepartment + "</td>";
                    table += "<td>" + item.tkfunction + "</td>";
                    table += "<td>" + item.tkorgstreet + "</td>";
                    table += "<td>" + item.tkorgpostalcode + "</td>";
                    table += "<td>" + item.tkorgpostaladdress + "</td>";
                    table += "<td>" + item.tkorgaddonaddr + "</td>";
                    table += "<td>" + item.tkorgfedst + "</td>";
                    table += "<td>" + item.tkorgcountry + "</td>";
                    table += "<td>" + item.tkorgcountrykey + "</td>";
                    table += "<td>" + item.tkapsite + "</td>";
                    table += "<td>" + item.tkorgkey + "</td>";
                    table += "<td>" + item.tkbuilding + "</td>";
                    table += "<td>" + item.email + "</td>";
                    table += "<td>" + item.tkareacode + "</td>";
                    table += "<td>" + item.tkphoneext + "</td>";
                    table += "<td>" + item.tkorgfax + "</td>";
                    table += "<td>" + item.tkmobile + "</td>";
                    table += "<td>" + item.tkgodfather + "</td>";
                    table += "<td>" + item.tkprefdelmethod + "</td>";
                    table += "<td>" + item.tkedateorg + "</td>";
                    table += "<td>" + item.tkedatetrust + "</td>";
                    table += "<td>" + item.tkldateorg + "</td>";
                    table += "<td>" + item.tklreason + "</td>";
                    table += "<td>" + item.shares + "</td>";
                    table += "<td>" + item.supervisoryboardelection + "</td>";
                    table += "<td>" + item.tkbkz + "</td>";
                    table += "<td>" + item.tkinside + "</td>";
                    table += "</tr>";
                }

                table += "</tbody>";
            }

            //genera codigo html para cada table
            if (tipoTable == "USUARIOSACTIVOS" && usuariosActivosList != null)
            {
                table += headUsuariosActivos;
                table += "<tbody>";

                foreach (var item in usuariosActivosList)
                {
                    table += "<tr>";
                    table += "<td>" + item.C8ID + "</td>";
                    table += "<td>" + item.Numero + "</td>";
                    table += "<td>" + item.ApellidoPaterno + "</td>";
                    table += "<td>" + item.ApellidoMaterno + "</td>";
                    table += "<td>" + item.Nombre + "</td>";
                    table += "<td>" + item.Antiguedad + "</td>";
                    table += "<td>" + item.Puesto + "</td>";
                    table += "<td>" + item.Departamento + "</td>";
                    table += "<td>" + item.Planta + "</td>";
                    table += "<td>" + item.Area + "</td>";
                    table += "<td>" + item.Departamento + "</td>";
                    table += "<td>" + item.Genero + "</td>";


                    table += "</tr>";
                }

                table += "</tbody>";
            }

            //genera codigo html para cada table
            if (tipoTable == "EXPORTUSERS" && exportUsersList != null)
            {
                table += headExportUsers;
                table += "<tbody>";

                foreach (var item in exportUsersList)
                {
                    table += "<tr>";
                    table += "<td>" + item.C8ID + "</td>";
                    table += "<td>" + item.userPrincipalName + "</td>";
                    table += "<td>" + item.displayName + "</td>";
                    table += "<td>" + item.surname + "</td>";
                    table += "<td>" + item.mail + "</td>";
                    table += "<td>" + item.givenName + "</td>";
                    table += "<td>" + item.C_id + "</td>";
                    table += "<td>" + item.userType + "</td>";
                    table += "<td>" + item.jobTitle + "</td>";
                    table += "<td>" + item.department + "</td>";
                    table += "<td>" + item.accountEnabled + "</td>";
                    table += "<td>" + item.usageLocation + "</td>";
                    table += "<td>" + item.streetAddress + "</td>";
                    table += "<td>" + item.state + "</td>";
                    table += "<td>" + item.country + "</td>";
                    table += "<td>" + item.officeLocation + "</td>";
                    table += "<td>" + item.city + "</td>";
                    table += "<td>" + item.postalCode + "</td>";
                    table += "<td>" + item.telephoneNumber + "</td>";
                    table += "<td>" + item.mobilePhone + "</td>";
                    table += "<td>" + item.alternateEmailAddress + "</td>";
                    table += "<td>" + item.ageGroup + "</td>";
                    table += "<td>" + item.consentProvidedForMinor + "</td>";
                    table += "<td>" + item.legalAgeGroupClassification + "</td>";
                    table += "<td>" + item.companyName + "</td>";
                    table += "<td>" + item.creationType + "</td>";
                    table += "<td>" + item.directorySynced + "</td>";
                    table += "<td>" + item.invitationState + "</td>";
                    table += "<td>" + item.identityIssuer + "</td>";
                    table += "<td>" + item.createdDateTime + "</td>";
                    table += "</tr>";
                }

                table += "</tbody>";
            }


            return table;
        }
        public ActionResult Export4Import()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        [HttpPost]
        public ActionResult Export4Import(ExcelViewModel excelViewModel, FormCollection collection)
        {
            if (ModelState.IsValid)
            {


                string msjError = "No se ha podido leer el archivo seleccionado.";

                //lee el archivo seleccionado
                try
                {
                    HttpPostedFileBase stream = Request.Files["PostedFile"];


                    if (stream.InputStream.Length > 8388608)
                    {
                        msjError = "Sólo se permiten archivos con peso menor a 8 MB.";
                        throw new Exception(msjError);
                    }
                    else
                    {
                        string extension = Path.GetExtension(excelViewModel.PostedFile.FileName);
                        if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                        {
                            msjError = "Sólo se permiten archivos Excel";
                            throw new Exception(msjError);
                        }
                    }

                    string msj = string.Empty;
                    //el archivo es válido
                    List<IT_Export4Import> lista = UtilExcel.LeeExport4Import(excelViewModel.PostedFile, ref msj);

                    //quita los repetidos
                    lista = lista.Distinct().ToList();

                    if (!string.IsNullOrEmpty(msj))
                    {
                        msjError = msj;
                        throw new Exception(msjError);
                    }
                    else
                    {
                        //truncate a tabla
                        db.Database.ExecuteSqlCommand("TRUNCATE TABLE [IT_Export4Import]");
                        //inserta nuevos registros
                        db.IT_Export4Import.AddRange(lista);

                        db.SaveChanges();

                        TempData["Mensaje"] = new MensajesSweetAlert("Se importó el archivo correctamente: " + lista.Count + " registros insertados.", TipoMensajesSweetAlerts.SUCCESS);
                        return RedirectToAction("index");
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", msjError);
                    return View(excelViewModel);
                }

            }
            return View(excelViewModel);
        }
        public ActionResult UsuariosActivos()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        [HttpPost]
        public ActionResult UsuariosActivos(ExcelViewModel excelViewModel, FormCollection collection)
        {
            if (ModelState.IsValid)
            {


                string msjError = "No se ha podido leer el archivo seleccionado.";

                //lee el archivo seleccionado
                try
                {
                    HttpPostedFileBase stream = Request.Files["PostedFile"];


                    if (stream.InputStream.Length > 8388608)
                    {
                        msjError = "Sólo se permiten archivos con peso menor a 8 MB.";
                        throw new Exception(msjError);
                    }
                    else
                    {
                        string extension = Path.GetExtension(excelViewModel.PostedFile.FileName);
                        if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX")
                        {
                            msjError = "Sólo se permiten archivos Excel";
                            throw new Exception(msjError);
                        }
                    }

                    string msj = string.Empty;
                    //el archivo es válido
                    List<IT_UsuariosActivos> lista = UtilExcel.LeeUsuariosActivos(excelViewModel.PostedFile, ref msj);

                    //quita los repetidos
                    lista = lista.Distinct().ToList();

                    if (!string.IsNullOrEmpty(msj))
                    {
                        msjError = msj;
                        throw new Exception(msjError);
                    }
                    else
                    {
                        //truncate a tabla
                        db.Database.ExecuteSqlCommand("TRUNCATE TABLE [IT_UsuariosActivos]");
                        //inserta nuevos registros
                        db.IT_UsuariosActivos.AddRange(lista);

                        db.SaveChanges();

                        TempData["Mensaje"] = new MensajesSweetAlert("Se importó el archivo correctamente: " + lista.Count + " registros insertados.", TipoMensajesSweetAlerts.SUCCESS);
                        return RedirectToAction("index");
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", msjError);
                    return View(excelViewModel);
                }

            }
            return View(excelViewModel);
        }

        public ActionResult ExportUsers()
        {
            if (TieneRol(TipoRoles.IT_INVENTORY))
            {
                return View();
            }
            else
            {
                return View("../Home/ErrorPermisos");
            }
        }

        [HttpPost]
        public ActionResult ExportUsers(ExcelViewModel excelViewModel, FormCollection collection)
        {
            if (ModelState.IsValid)
            {


                string msjError = "No se ha podido leer el archivo seleccionado.";

                //lee el archivo seleccionado
                try
                {
                    HttpPostedFileBase stream = Request.Files["PostedFile"];


                    if (stream.InputStream.Length > 8388608)
                    {
                        msjError = "Sólo se permiten archivos con peso menor a 8 MB.";
                        throw new Exception(msjError);
                    }
                    else
                    {
                        string extension = Path.GetExtension(excelViewModel.PostedFile.FileName);
                        if (extension.ToUpper() != ".XLS" && extension.ToUpper() != ".XLSX" && extension.ToUpper() != ".CSV")
                        {
                            msjError = "Sólo se permiten archivos Excel";
                            throw new Exception(msjError);
                        }
                    }

                    string msj = string.Empty;
                    //el archivo es válido
                    List<IT_ExportUsers> lista = UtilExcel.LeeExportUsers(excelViewModel.PostedFile, ref msj);

                    //quita los repetidos
                    lista = lista.Distinct().ToList();

                    if (!string.IsNullOrEmpty(msj))
                    {
                        msjError = msj;
                        throw new Exception(msjError);
                    }
                    else
                    {
                        //truncate a tabla
                        db.Database.ExecuteSqlCommand("TRUNCATE TABLE [IT_ExportUsers]");
                        //inserta nuevos registros
                        db.IT_ExportUsers.AddRange(lista);

                        db.SaveChanges();

                        TempData["Mensaje"] = new MensajesSweetAlert("Se importó el archivo correctamente: " + lista.Count + " registros insertados.", TipoMensajesSweetAlerts.SUCCESS);
                        return RedirectToAction("index");
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", msjError);
                    ModelState.AddModelError("Detalle:", e.Message);
                    return View(excelViewModel);
                }

            }
            return View(excelViewModel);
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
