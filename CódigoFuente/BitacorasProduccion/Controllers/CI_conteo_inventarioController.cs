using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clases.Util;
using Portal_2_0.Models;

namespace Portal_2_0.Controllers
{
    public class CI_conteo_inventarioController : Controller
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: CI_conteo_inventario
        public ActionResult Index()
        {
            List<CI_conteo_inventario> lista = new List<CI_conteo_inventario>();
            return View(lista);
        }

     
        // GET: CI_conteo_inventario/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CI_conteo_inventario cI_conteo_inventario = db.CI_conteo_inventario.Find(id);
            if (cI_conteo_inventario == null)
            {
                return HttpNotFound();
            }
            return View(cI_conteo_inventario);
        }

        // POST: CI_conteo_inventario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,centro,almacen,ubicacion,articulo,material,lote,no_bobina,sap_cantidad,libre_utilizacion,bloqueado,control_calidad,unidad_base_medida,altura,espesor,peso")] CI_conteo_inventario cI_conteo_inventario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cI_conteo_inventario).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = "Se editó correctamente el registro.";
                ViewBag.Tipo = TipoMensajesSweetAlerts.SUCCESS;
                TempData["Mensaje"] = new MensajesSweetAlert("Se editó correctamente el registro.", TipoMensajesSweetAlerts.SUCCESS);
                return View("Message");
            }
            return View(cI_conteo_inventario);
        }

        ///<summary>
        ///Obtiene las areas segun la planta recibida
        ///</summary>
        ///<return>
        ///retorna un JsonResult con las opciones disponibles
        public JsonResult CargaTabla()
        {


            DataTable dataTable = new DataTable();
            List<CI_conteo_inventario> stud = db.CI_conteo_inventario.ToList();
            
            foreach (var item in stud)
            {
                item.acciones = @" <button onclick=""editar("+item.id+@")"" class=""btn btn-success btn-sm btn-block"">
                                                            <i class=""fa-regular fa-pen-to-square""></i>
                                                        </button>";
            }

            //The magic happens here
            dataTable.data = stud;
            return Json(dataTable, JsonRequestBehavior.AllowGet);
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

    public class DataTable
    {
        public List<CI_conteo_inventario> data { get; set; }
    }
}
