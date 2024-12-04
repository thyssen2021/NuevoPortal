using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    [Authorize]
    public class Inv_CapturaController : BaseController
    {
        private Portal_2_0Entities db = new Portal_2_0Entities();

        // GET: Inv_Captura
        public ActionResult Index(string plantaFiltro = null, string batchFiltro = null, string storageBinFiltro = null, string materialFiltro = null)
        {
            var lotesQuery = db.Inv_Lote.Include("Inv_Material").Include("plantas").AsQueryable();

            // Aplicar filtros según los valores ingresados por el usuario
            if (!string.IsNullOrEmpty(plantaFiltro))
                lotesQuery = lotesQuery.Where(l => l.plantas.codigoSap == plantaFiltro);

            if (!string.IsNullOrEmpty(batchFiltro))
                lotesQuery = lotesQuery.Where(l => l.Batch == batchFiltro);

            if (!string.IsNullOrEmpty(storageBinFiltro))
                lotesQuery = lotesQuery.Where(l => l.StorageBin.Contains(storageBinFiltro));

            if (!string.IsNullOrEmpty(materialFiltro))
                lotesQuery = lotesQuery.Where(l => l.Inv_Material.NumeroMaterial.Contains(materialFiltro));

            // Convertir la consulta en una lista de CapturaViewModel para pasar a la vista
            var capturas = lotesQuery.Select(l => new CapturaViewModel
            {
                LoteId = l.LoteID,
                NumeroMaterial = l.Inv_Material.NumeroMaterial,
                Batch = l.Batch,
                StorageBin = l.StorageBin,
                StorageLocation = l.StorageLocation,
                Planta = l.plantas.descripcion,
                Espesor = l.Inv_Material.Espesor ?? 0,
                EspesorMin = l.Inv_Material.EspesorMin ?? 0,
                EspesorMax = l.Inv_Material.EspesorMax ?? 0,
                AlturaMedida = l.AlturaCalculada, // Inicialmente en cero, ya que el usuario lo ingresará
                CantidadEstimada = 0, // Inicialmente en cero, ya que se calcula en la vista
                PiezasSAP = l.Pieces ?? 0,   
                UbicacionFisica =  l.UbicacionFisica ?? l.StorageBin,
                Comentarios = string.Empty,
                EspesorUsuario = l.EspesorUsuario,
            }).ToList();

            return View(capturas);
        }


        [HttpPost]
        public JsonResult GuardarCaptura(List<CapturaViewModel> capturas)
        {
            if (capturas == null || capturas.Count == 0)
            {
                return Json(new { success = false, message = "No se realizaron cambios para guardar." });
            }

            try
            {
                foreach (var captura in capturas)
                {
                    var lote = db.Inv_Lote.FirstOrDefault(l => l.LoteID == captura.LoteId);
                    if (lote != null)
                    {
                        // Actualizar los valores capturados por el usuario
                        lote.AlturaCalculada = captura.AlturaMedida;
                        lote.EspesorUsuario = captura.EspesorUsuario;
                        lote.UbicacionFisica = captura.UbicacionFisica;

                        // Establecer la versión original para manejo de concurrencia
                        db.Entry(lote).OriginalValues["RowVersion"] = captura.RowVersion;
                    }
                }

                db.SaveChanges();
                //TempData["SuccessMessage"] = "Datos guardados exitosamente.";
                return Json(new { success = true, message = "Datos guardados exitosamente" });

            }
            catch (DbUpdateConcurrencyException ex)
            {
                //TempData["ErrorMessage"] = "Hubo un conflicto de concurrencia. Algunos datos pueden haber sido modificados por otro usuario.";
                return Json(new { success = false, message = $"Error al guardar los datos: {ex.Message}" });
            }
            catch (Exception ex)
            {
                //TempData["ErrorMessage"] = "Ocurrió un error al guardar los datos.";
                return Json(new { success = false, message = $"Error al guardar los datos: {ex.Message}" });
            }
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

    //viewModel
    public class CapturaViewModel
    {
        public int LoteId { get; set; }  // ID del lote a capturar
        public string NumeroMaterial { get; set; }  // Número de material asociado al lote
        public string Batch { get; set; }  // Identificación del lote (Batch)
        public string StorageBin { get; set; }  // Ubicación en el almacén (Storage Bin)
        public string StorageLocation { get; set; }  // storage location
        public string Planta { get; set; }  // Planta donde se encuentra el material
        public decimal Espesor { get; set; }  // Espesor 
        public decimal EspesorMin { get; set; }  // Espesor mínimo del material, necesario para calcular las piezas
        public decimal EspesorMax { get; set; }  // Espesor máximo del material (para verificar el rango permitido)
        public int CantidadEstimada { get; set; }  // Cantidad estimada de piezas calculadas a partir de la altura medida
        public string Advertencia { get; set; }  // Advertencia si la cantidad estimada está fuera del rango
        public string Comentarios { get; set; }  // Comentarios adicionales del usuario sobre la captura
        public string UsuarioCaptura { get; set; }  // Nombre o ID del usuario que realizó la captura
        public int PiezasSAP { get; set; }  // piezasSAP

        // Campos capturados por el usuario
        public decimal? AlturaMedida { get; set; } // Campo capturable: Altura medida por el usuario
        public decimal? EspesorUsuario { get; set; } // Campo capturable: Espesor ingresado por el usuario
        public string UbicacionFisica { get; set; } // Campo capturable: Ubicación física

        // Campo para control de concurrencia
        [Timestamp]
        public byte[] RowVersion { get; set; }

        // Valores calculados con base en PiezasSAP, EspesorMin y EspesorMax
        public decimal? PiezasMin
        {
            get
            {
                return EspesorMin * PiezasSAP;
            }
        }

        public decimal? PiezasMax
        {
            get
            {
                return EspesorMax * PiezasSAP;
            }
        }
    }
}