using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Security.Twitter.Messages;
using Portal_2_0.Models;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace Portal_2_0.Controllers
{
    [System.Web.Mvc.Authorize]
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

            // Obtener la lista de lotes seleccionados
            var lotes = lotesQuery.ToList();
            var loteIds = lotes.Select(l => l.LoteID).ToList();

            // Obtener los movimientos más recientes por lote
            var movimientosLotes = db.Inv_HistorialCaptura
                .Where(h => h.LoteID.HasValue && loteIds.Contains(h.LoteID.Value))
                .GroupBy(h => h.LoteID)
                .Select(g => g.OrderByDescending(h => h.FechaCaptura).FirstOrDefault())
                .ToDictionary(h => h.LoteID.Value, h => h);

            // Obtener los grupos y sus lotes
            var grupos = db.Inv_LoteGrupo.ToList();
            var grupoIds = grupos.Select(g => g.GrupoID).ToList();

            var movimientosGrupos = db.Inv_HistorialCaptura
                .Where(h => h.GrupoID.HasValue && grupoIds.Contains(h.GrupoID.Value))
                .GroupBy(h => h.GrupoID)
                .SelectMany(g => g.OrderByDescending(h => h.FechaCaptura))
                .ToList();

            var lotesGrupos = db.Inv_GrupoLoteDetalle
                .Where(d => grupoIds.Contains(d.GrupoID))
                .ToList();


            // Cargar AspNetUsers e IdEmpleado
            var usuarios = db.AspNetUsers
                .Where(u => u.IdEmpleado > 0) // Filtrar usuarios con IdEmpleado no nulo
                .Select(u => new { u.Id, u.IdEmpleado })
                .ToList();

            // Cargar empleados en memoria
            var empleados = db.empleados.ToList()
                .Select(e => new { e.id, e.ConcatNombre })
                .ToList();

            var usuariosConEmpleados = usuarios
            .Join(empleados, // Unir usuarios con empleados
                  u => u.IdEmpleado, // Clave de usuario
                  e => e.id,         // Clave de empleado
                  (u, e) => new { u.Id, e.ConcatNombre }) // Seleccionar los campos necesarios
            .ToDictionary(u => u.Id, u => u.ConcatNombre);

            // Clasificar lotes y grupos
            var lotesProcesados = new HashSet<int>();
            var capturas = new List<CapturaViewModel>();
            var capturasGrupos = new List<CapturaGrupoViewModel>();

            foreach (var lote in lotes)
            {
                var movimientoLote = movimientosLotes.ContainsKey(lote.LoteID) ? movimientosLotes[lote.LoteID] : null;
                var movimientoGrupo = movimientosGrupos
                    .Where(h => lotesGrupos.Any(d => d.LoteID == lote.LoteID && d.GrupoID == h.GrupoID))
                    .OrderByDescending(h => h.FechaCaptura)
                    .FirstOrDefault();

                if (movimientoGrupo != null && (movimientoLote == null || movimientoGrupo.FechaCaptura > movimientoLote.FechaCaptura))
                {
                    // El movimiento más reciente es de grupo
                    var grupo = grupos.FirstOrDefault(g => g.GrupoID == movimientoGrupo.GrupoID);
                    if (grupo != null && !lotesProcesados.Contains(lote.LoteID))
                    {
                        var lotesRelacionados = lotesGrupos
                            .Where(d => d.GrupoID == grupo.GrupoID)
                            .Select(d => d.Inv_Lote)
                            .ToList();

                        // Agregar grupo con lotes relacionados
                        capturasGrupos.AddRange(GenerarCapturaGrupoViewModel(grupo, lotesRelacionados, movimientoGrupo, usuariosConEmpleados));
                        lotesRelacionados.ForEach(l => lotesProcesados.Add(l.LoteID));
                    }
                }
                else
                {
                    // El movimiento más reciente es individual o no hay movimiento en grupo
                    if (!lotesProcesados.Contains(lote.LoteID))
                    {
                        capturas.Add(MapToCapturaViewModel(lote, movimientoLote, usuariosConEmpleados));
                        lotesProcesados.Add(lote.LoteID);
                    }
                }
            }

            var model = new CapturaGuardarViewModel
            {
                Lotes = capturas,
                Grupos = capturasGrupos
            };

            var serializer = new JavaScriptSerializer
            {
                MaxJsonLength = int.MaxValue
            };

            // Serializa el modelo con el límite extendido
            ViewBag.LotesSerialized = serializer.Serialize(model.Lotes);

            return View(model);
        }



        private List<CapturaGrupoViewModel> GenerarCapturaGrupoViewModel(
     Inv_LoteGrupo grupo,
     List<Inv_Lote> lotesRelacionados,
     Inv_HistorialCaptura historialGrupo,
     Dictionary<string, string> usuariosConEmpleados
     )
        {
            // Validar entrada
            if (lotesRelacionados == null || !lotesRelacionados.Any())
            {
                throw new ArgumentException($"El grupo {grupo.codigo} no tiene lotes relacionados.");
            }

            // Obtener el primer lote relacionado como referencia
            var primerLote = lotesRelacionados.FirstOrDefault();
            decimal espesor = primerLote?.Inv_Material?.Espesor ?? 0;
            decimal espesorMin = primerLote?.Inv_Material?.EspesorMin ?? 0;
            decimal espesorMax = primerLote?.Inv_Material?.EspesorMax ?? 0;

            // Calcular la suma de Pieces de todos los lotes relacionados
            var piezasSAP = lotesRelacionados.Sum(l => l.Pieces ?? 0);
            var Unrestricted = lotesRelacionados.Sum(l => l.Unrestricted ?? 0);
            var Blocked = lotesRelacionados.Sum(l => l.Blocked ?? 0);
            var QualityInspection = lotesRelacionados.Sum(l => l.QualityInspection ?? 0);

            string empleadoConcatName = null;
            if (historialGrupo != null && historialGrupo.AspNetUsers != null)
            {              
                usuariosConEmpleados.TryGetValue(historialGrupo.AspNetUsers.Id, out empleadoConcatName);
            }

            // Crear fila de resumen del grupo
            var resumen = new CapturaGrupoViewModel
            {
                GrupoId = grupo.codigo,
                LoteId = grupo.codigo, // Usar el código del grupo como identificador de la fila resumen
                Planta = primerLote?.plantas?.descripcion ?? "--",
                StorageLocation = "--",
                StorageBin = "--",
                Batch = "--",
                NumeroMaterial = primerLote?.Inv_Material?.NumeroMaterial ?? "--",
                Espesor = espesor,
                EspesorMin = espesorMin,
                EspesorMax = espesorMax,
                AlturaMedida = historialGrupo?.AlturaUsuario,
                EspesorUsuario = historialGrupo?.EspesorUsuario,
                UbicacionFisica = historialGrupo?.UbicacionFisica,
                CantidadTeorica = (historialGrupo != null && historialGrupo.AlturaUsuario.HasValue && espesor > 0)
                    ? Math.Round((double)historialGrupo.AlturaUsuario.Value / (double)espesor, 3)
                    : (double?)null,
                PiezasSAP = piezasSAP,
                Unrestricted = Unrestricted,
                Blocked = Blocked, 
                QualityInspection = QualityInspection,
                PiezasMin = espesorMax != 0 ? Math.Round((piezasSAP * espesor) / espesorMax, 3) : 0,
                PiezasMax = espesorMin != 0 ? Math.Round((piezasSAP * espesor) / espesorMin, 3) : 0,
                DiferenciaPiezas = historialGrupo?.DiferenciaPiezas ?? 0,
                Advertencia = historialGrupo?.Advertencia,
                Comentarios = historialGrupo?.Comentarios,
                LotesIds = lotesRelacionados.Select(l => l.LoteID).ToList(),
                EsResumen = true,
                NumeroAntiguo =  "--",
                Descripcion = "--",
                UsuarioCaptura = empleadoConcatName,
                CapturaFecha = historialGrupo != null ? historialGrupo.FechaCaptura.Value : (DateTime?)null,
            };

            // Crear filas individuales para los lotes relacionados
            var detallesLotes = lotesRelacionados.Select(lote =>
            {
                var material = lote.Inv_Material ?? new Inv_Material();
                return new CapturaGrupoViewModel
                {
                    GrupoId = grupo.codigo,
                    LoteId = lote.LoteID.ToString(),
                    Planta = lote.plantas?.descripcion ?? "--",
                    StorageLocation = lote.StorageLocation,
                    StorageBin = lote.StorageBin,
                    Batch = lote.Batch,
                    NumeroMaterial = material.NumeroMaterial ?? "--",
                    NumeroAntiguo = material.NumeroAntiguo ?? "--",
                    Descripcion = material.Descripcion ?? "--",
                    Espesor = material.Espesor ?? 0,
                    EspesorMin = material.EspesorMin ?? 0,
                    EspesorMax = material.EspesorMax ?? 0,
                    AlturaMedida = null, // Solo se aplica al resumen
                    EspesorUsuario = null, // Solo se aplica al resumen
                    UbicacionFisica = null, // Solo se aplica al resumen
                    CantidadTeorica = null, // Solo se aplica al resumen
                    PiezasSAP = lote.Pieces ?? 0,
                    Unrestricted = lote.Unrestricted ?? 0,
                    Blocked = lote.Blocked ?? 0,
                    QualityInspection = lote.QualityInspection ?? 0,
                    PiezasMin = lote.PiezasMin,
                    PiezasMax = lote.PiezasMax,
                    DiferenciaPiezas = null, // Solo se aplica al resumen
                    Advertencia = null, // Solo se aplica al resumen
                    Comentarios = null, // Solo se aplica al resumen
                    EsResumen = false,
                };
            }).ToList();

            // Retornar el resumen junto con los detalles de los lotes
            return new List<CapturaGrupoViewModel> { resumen }.Concat(detallesLotes).ToList();
        }


        private CapturaViewModel MapToCapturaViewModel(Inv_Lote lote, Inv_HistorialCaptura historial, Dictionary<string, string> usuariosConEmpleados)
        {
            string empleadoConcatName = null;
            if (historial != null && historial.AspNetUsers != null)
            {
                usuariosConEmpleados.TryGetValue(historial.AspNetUsers.Id, out empleadoConcatName);
            }

            return new CapturaViewModel
            {
                LoteId = lote.LoteID,
                NumeroMaterial = lote.Inv_Material.NumeroMaterial,
                Batch = lote.Batch,
                StorageBin = lote.StorageBin,
                StorageLocation = lote.StorageLocation,
                Planta = lote.plantas.descripcion,
                Espesor = lote.Inv_Material.Espesor ?? 0,
                EspesorMin = lote.Inv_Material.EspesorMin ?? 0,
                EspesorMax = lote.Inv_Material.EspesorMax ?? 0,
                AlturaMedida = historial?.AlturaUsuario,
                EspesorUsuario = historial?.EspesorUsuario,
                UbicacionFisica = historial?.UbicacionFisica ?? lote.StorageBin,
                CantidadTeorica = (historial != null && historial.AlturaUsuario.HasValue && lote.Inv_Material.Espesor.HasValue && lote.Inv_Material.Espesor > 0)
                                  ? Math.Round((double)historial.AlturaUsuario.Value / (double)lote.Inv_Material.Espesor.Value, 3)
                                  : (double?)null,
                Advertencia = historial?.Advertencia ?? "Pendiente",
                PiezasSAP = lote.Pieces ?? 0,
                Unrestricted = lote.Unrestricted ?? 0,
                Blocked = lote.Blocked ?? 0,
                QualityInspection = lote.QualityInspection ?? 0,
                Comentarios = historial?.Comentarios,
                RowVersion = lote.RowVersion,
                NumeroAntiguo = lote.Inv_Material.NumeroAntiguo,
                Descripcion = lote.Inv_Material.Descripcion,
                UsuarioCaptura = empleadoConcatName,
                CapturaFecha = historial != null ? historial.FechaCaptura.Value : (DateTime?)null,
                Selected = false,
            };
        }



        [HttpPost]
        public JsonResult GuardarCaptura(CapturaGuardarViewModel capturaGuardarViewModel)
        {
            if (capturaGuardarViewModel == null ||
            (capturaGuardarViewModel.Lotes == null || capturaGuardarViewModel.Lotes.Count == 0) &&
            (capturaGuardarViewModel.Grupos == null || capturaGuardarViewModel.Grupos.Count == 0))
            {
                return Json(new { success = false, message = "No se realizaron cambios para guardar." });
            }

            var erroresLotes = new List<string>();
            var erroresGrupos = new List<string>();

            //variables en caso de nulos
            int? intNull = null;

            try
            {
                //Procesa los lotes Individuales
                #region Lotes individuales
                if (capturaGuardarViewModel.Lotes != null)
                {
                    foreach (var captura in capturaGuardarViewModel.Lotes)
                    {
                        try
                        {
                            // Código original para procesar lotes
                            var lote = db.Inv_Lote.FirstOrDefault(l => l.LoteID == captura.LoteId);
                            if (lote != null)
                            {
                                // Actualizar los valores capturados por el usuario
                                lote.AlturaCalculada = captura.AlturaMedida;
                                lote.EspesorUsuario = captura.EspesorUsuario;
                                lote.UbicacionFisica = captura.UbicacionFisica ?? string.Empty;
                                // Actualizar la fecha de modificación
                                lote.DateModified = DateTime.Now;

                                // Establecer la versión original para manejo de concurrencia
                                db.Entry(lote).OriginalValues["RowVersion"] = captura.RowVersion;
                            }

                            // Lógica para calcular el valor de Advertencia
                            string advertencia = "Pendiente"; // Valor por defecto
                            if (captura.AlturaMedida.HasValue && captura.AlturaMedida.Value > 0)
                            {
                                if (captura.CantidadTeorica == null || captura.PiezasMin == null || captura.PiezasMax == null)
                                {
                                    advertencia = "Sin Espesores";
                                }
                                else if ((decimal)captura.CantidadTeorica >= captura.PiezasMin && (decimal)captura.CantidadTeorica <= captura.PiezasMax)
                                {
                                    advertencia = "Dentro de Tolerancias";
                                }
                                else
                                {
                                    advertencia = "Ajustar";
                                }
                            }

                            //guarda la historia de captura
                            var historialCaptura = new Inv_HistorialCaptura
                            {
                                LoteID = captura.LoteId,
                                AlturaUsuario = captura.AlturaMedida,
                                EspesorUsuario = captura.EspesorUsuario,
                                UbicacionFisica = captura.UbicacionFisica ?? string.Empty,
                                UsuarioCaptura = User.Identity.GetUserId(), // Asigna el usuario actual
                                FechaCaptura = DateTime.Now,
                                PiezasCalculadas = captura.CantidadTeorica != null ? (int)captura.CantidadTeorica : intNull,
                                DiferenciaPiezas = captura.AlturaMedida == null ? intNull : (int)((captura.CantidadTeorica ?? 0) - (lote?.Pieces ?? 0)),
                                Advertencia = advertencia,
                                Comentarios = captura.Comentarios,
                            };

                            db.Inv_HistorialCaptura.Add(historialCaptura);

                            // Guardar los cambios del lote y su historial

                            db.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                            // Captura errores específicos de este lote
                            erroresLotes.Add($"Error en el Material-Lote {captura.NumeroMaterial}-{captura.Batch}: {ex.Message}");
                        }
                     
                    }
                }
                #endregion

                //Procesa los grupos
                #region Grupos
                if (capturaGuardarViewModel.Grupos != null && capturaGuardarViewModel.Grupos.Count > 0)
                {
                    foreach (var grupo in capturaGuardarViewModel.Grupos)
                    {

                        try
                        {
                            // Código original para procesar grupos
                            var grupoExistente = db.Inv_LoteGrupo.FirstOrDefault(g => g.codigo == grupo.GrupoId);

                            if (grupoExistente != null)
                            {
                                // Actualizar valores del grupo existente
                                grupoExistente.codigo = grupo.GrupoId;

                                // Eliminar los lotes actuales del grupo para reemplazarlos
                                var lotesExistentes = db.Inv_GrupoLoteDetalle.Where(d => d.GrupoID == grupoExistente.GrupoID).ToList();
                                db.Inv_GrupoLoteDetalle.RemoveRange(lotesExistentes);

                                // Añadir los nuevos lotes al grupo existente
                                foreach (var loteId in grupo.LotesIds)
                                {
                                    var detalleGrupo = new Inv_GrupoLoteDetalle
                                    {
                                        GrupoID = grupoExistente.GrupoID,
                                        LoteID = loteId
                                    };
                                    db.Inv_GrupoLoteDetalle.Add(detalleGrupo);
                                }

                                // Guardar la historia de captura del grupo actualizado
                                var historialGrupoExistente = new Inv_HistorialCaptura
                                {
                                    GrupoID = grupoExistente.GrupoID,
                                    AlturaUsuario = grupo.AlturaMedida,
                                    EspesorUsuario = grupo.EspesorUsuario,
                                    UbicacionFisica = grupo.UbicacionFisica ?? string.Empty,
                                    UsuarioCaptura = User.Identity.GetUserId(), // Asigna el usuario actual
                                    FechaCaptura = DateTime.Now,
                                    Comentarios = grupo.Comentarios,
                                    PiezasCalculadas = (int)grupo.CantidadTeorica,
                                    DiferenciaPiezas = grupo.AlturaMedida != null ?
                                       (grupo.CantidadTeorica.HasValue ? (int)(grupo.CantidadTeorica.Value - (grupo.PiezasSAP ?? 0)) : (int?)null)
                                       : (int?)null,
                                    Advertencia = grupo.Advertencia,
                                };

                                db.Inv_HistorialCaptura.Add(historialGrupoExistente);
                            }
                            else
                            {
                                // Crear un nuevo grupo en la tabla Inv_LoteGrupo
                                var nuevoGrupo = new Inv_LoteGrupo
                                {
                                    codigo = grupo.GrupoId
                                };

                                db.Inv_LoteGrupo.Add(nuevoGrupo);
                                db.SaveChanges(); // Guardar primero para obtener el GrupoID

                                // Agregar detalles de los lotes que pertenecen al nuevo grupo
                                foreach (var loteId in grupo.LotesIds)
                                {
                                    var detalleGrupo = new Inv_GrupoLoteDetalle
                                    {
                                        GrupoID = nuevoGrupo.GrupoID,
                                        LoteID = loteId
                                    };
                                    db.Inv_GrupoLoteDetalle.Add(detalleGrupo);
                                }

                                // Guardar la historia de captura del nuevo grupo
                                var historialGrupoNuevo = new Inv_HistorialCaptura
                                {
                                    GrupoID = nuevoGrupo.GrupoID,
                                    AlturaUsuario = grupo.AlturaMedida,
                                    EspesorUsuario = grupo.EspesorUsuario,
                                    UbicacionFisica = grupo.UbicacionFisica ?? string.Empty,
                                    UsuarioCaptura = User.Identity.GetUserId(), // Asigna el usuario actual
                                    FechaCaptura = DateTime.Now,
                                    Comentarios = grupo.Comentarios,
                                    PiezasCalculadas = grupo.CantidadTeorica.HasValue ? (int?)grupo.CantidadTeorica.Value : null,
                                    DiferenciaPiezas = grupo.AlturaMedida != null && grupo.CantidadTeorica.HasValue
                                    ? (int?)(grupo.CantidadTeorica.Value - (grupo.PiezasSAP ?? 0))
                                    : null,
                                    Advertencia = grupo.Advertencia,
                                };

                                db.Inv_HistorialCaptura.Add(historialGrupoNuevo);                            
                            }
                            db.SaveChanges(); // Guardar cambios del grupo actual
                        }
                        catch (Exception ex)
                        {
                            // Captura errores específicos de este grupo
                            erroresGrupos.Add($"Error en el GrupoID {grupo.GrupoId}: {ex.Message}");
                        }
                        // Buscar si ya existe un grupo con el mismo código
                       
                    }
                }


                #endregion

                db.SaveChanges();

                // Notificar a los clientes que los datos se han actualizado
                var context = GlobalHost.ConnectionManager.GetHubContext<InventarioHub>();
                context.Clients.All.recibirActualizacion();

                //TempData["SuccessMessage"] = "Datos guardados exitosamente.";
                if (!erroresLotes.Any() && !erroresGrupos.Any())
                {
                    return Json(new { success = true, message = "Datos guardados exitosamente." });
                }
                else {
                    return Json(new
                    {
                        success = true,
                        message = "Datos guardados con algunos errores.",
                        erroresLotes,
                        erroresGrupos
                    });
                }

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



        public JsonResult ObtenerDatosActualizados(DateTime fechaUltimaActualizacion)
        {
            try
            {
                // Obtener los registros del historial que han sido actualizados después de la fecha proporcionada
                var historialCapturas = db.Inv_HistorialCaptura
                    .Where(h => h.FechaCaptura > fechaUltimaActualizacion)
                    .GroupBy(h => h.LoteID)
                    .Select(g => g.OrderByDescending(h => h.FechaCaptura).FirstOrDefault())
                    .ToList();

                // Crear la lista de datos actualizados en el formato requerido
                var datosActualizados = historialCapturas.Select(h => new
                {
                    LoteId = h.LoteID,
                    RowVersion = h.Inv_Lote.RowVersion,
                    AlturaMedida = h.AlturaUsuario,
                    EspesorUsuario = h.EspesorUsuario,
                    UbicacionFisica = h.UbicacionFisica,
                    CantidadTeorica = h.PiezasCalculadas,
                    Comentarios = h.Comentarios
                }).ToList();

                return Json(datosActualizados, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ExportarExcel(string plantaFiltro = null)
        {
            try
            {
                // Obtén la información para los lotes y grupos utilizando la lógica existente del método Index
                var lotesQuery = db.Inv_Lote.Include("Inv_Material").Include("plantas").AsQueryable();

                // Aplicar filtros según los valores ingresados por el usuario
                if (!string.IsNullOrEmpty(plantaFiltro))
                    lotesQuery = lotesQuery.Where(l => l.plantas.codigoSap == plantaFiltro);

                var lotes = lotesQuery.ToList();
                var loteIds = lotes.Select(l => l.LoteID).ToList();

                // Procesa lotes y grupos
                var movimientosLotes = db.Inv_HistorialCaptura
                    .Where(h => h.LoteID.HasValue && loteIds.Contains(h.LoteID.Value))
                    .GroupBy(h => h.LoteID)
                    .Select(g => g.OrderByDescending(h => h.FechaCaptura).FirstOrDefault())
                    .ToDictionary(h => h.LoteID.Value, h => h);

                var grupos = db.Inv_LoteGrupo.ToList();
                var grupoIds = grupos.Select(g => g.GrupoID).ToList();

                var movimientosGrupos = db.Inv_HistorialCaptura
                    .Where(h => h.GrupoID.HasValue && grupoIds.Contains(h.GrupoID.Value))
                    .GroupBy(h => h.GrupoID)
                    .SelectMany(g => g.OrderByDescending(h => h.FechaCaptura))
                    .ToList();

                var lotesGrupos = db.Inv_GrupoLoteDetalle
                    .Where(d => grupoIds.Contains(d.GrupoID))
                    .ToList();

                var lotesProcesados = new HashSet<int>();
                var capturas = new List<CapturaViewModel>();
                var capturasGrupos = new List<CapturaGrupoViewModel>();

                // Cargar AspNetUsers e IdEmpleado
                var usuarios = db.AspNetUsers
                    .Where(u => u.IdEmpleado > 0) // Filtrar usuarios con IdEmpleado no nulo
                    .Select(u => new { u.Id, u.IdEmpleado })
                    .ToList();

                // Cargar empleados en memoria
                var empleados = db.empleados.ToList()
                    .Select(e => new { e.id, e.ConcatNombre })
                    .ToList();

                var usuariosConEmpleados = usuarios
                .Join(empleados, // Unir usuarios con empleados
                      u => u.IdEmpleado, // Clave de usuario
                      e => e.id,         // Clave de empleado
                      (u, e) => new { u.Id, e.ConcatNombre }) // Seleccionar los campos necesarios
                .ToDictionary(u => u.Id, u => u.ConcatNombre);

                foreach (var lote in lotes)
                {
                    var movimientoLote = movimientosLotes.ContainsKey(lote.LoteID) ? movimientosLotes[lote.LoteID] : null;
                    var movimientoGrupo = movimientosGrupos
                        .Where(h => lotesGrupos.Any(d => d.LoteID == lote.LoteID && d.GrupoID == h.GrupoID))
                        .OrderByDescending(h => h.FechaCaptura)
                        .FirstOrDefault();

                    if (movimientoGrupo != null && (movimientoLote == null || movimientoGrupo.FechaCaptura > movimientoLote.FechaCaptura))
                    {
                        var grupo = grupos.FirstOrDefault(g => g.GrupoID == movimientoGrupo.GrupoID);
                        if (grupo != null && !lotesProcesados.Contains(lote.LoteID))
                        {
                            var lotesRelacionados = lotesGrupos
                                .Where(d => d.GrupoID == grupo.GrupoID)
                                .Select(d => d.Inv_Lote)
                                .ToList();

                            capturasGrupos.AddRange(GenerarCapturaGrupoViewModel(grupo, lotesRelacionados, movimientoGrupo, usuariosConEmpleados));
                            lotesRelacionados.ForEach(l => lotesProcesados.Add(l.LoteID));
                        }
                    }
                    else
                    {
                        if (!lotesProcesados.Contains(lote.LoteID))
                        {
                            capturas.Add(MapToCapturaViewModel(lote, movimientoLote, usuariosConEmpleados));
                            lotesProcesados.Add(lote.LoteID);
                        }
                    }
                }

                // Generar el archivo Excel
                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    // Estilos para diferentes tipos de filas
                    SLStyle styleLote = sl.CreateStyle();
                    styleLote.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#E8F5E9"), System.Drawing.ColorTranslator.FromHtml("#E8F5E9")); // Verde sutil
                    styleLote.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    styleLote.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    styleLote.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    styleLote.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    styleLote.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
                    styleLote.Border.TopBorder.Color = System.Drawing.Color.LightGray;
                    styleLote.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
                    styleLote.Border.RightBorder.Color = System.Drawing.Color.LightGray;

                    SLStyle styleResumenGrupo = sl.CreateStyle();
                    styleResumenGrupo.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightBlue, System.Drawing.Color.LightBlue);
                    styleResumenGrupo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    styleResumenGrupo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    styleResumenGrupo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    styleResumenGrupo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
                    styleResumenGrupo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
                    styleResumenGrupo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
                    styleResumenGrupo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

                    SLStyle styleLoteGrupo = sl.CreateStyle();
                    styleLoteGrupo.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightYellow, System.Drawing.Color.LightYellow);
                    styleLoteGrupo.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    styleLoteGrupo.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    styleLoteGrupo.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    styleLoteGrupo.Border.BottomBorder.Color = System.Drawing.Color.LightGray;
                    styleLoteGrupo.Border.TopBorder.Color = System.Drawing.Color.LightGray;
                    styleLoteGrupo.Border.LeftBorder.Color = System.Drawing.Color.LightGray;
                    styleLoteGrupo.Border.RightBorder.Color = System.Drawing.Color.LightGray;

                    SLStyle styleAjustar = sl.CreateStyle();
                    styleAjustar.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#FFCDD2"), System.Drawing.ColorTranslator.FromHtml("#FFCDD2")); // Rojo claro
                    styleAjustar.Font.FontColor = System.Drawing.Color.DarkRed;

                    SLStyle stylePendiente = sl.CreateStyle();
                    stylePendiente.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#FFF9C4"), System.Drawing.ColorTranslator.FromHtml("#FFF9C4")); // Amarillo claro
                    stylePendiente.Font.FontColor = System.Drawing.ColorTranslator.FromHtml("#D22500");

                    SLStyle styleDentroTolerancia = sl.CreateStyle();
                    styleDentroTolerancia.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#C8E6C9"), System.Drawing.ColorTranslator.FromHtml("#C8E6C9")); // Verde claro
                    styleDentroTolerancia.Font.FontColor = System.Drawing.Color.Green;

                    SLStyle styleNeutral = sl.CreateStyle();
                    styleNeutral.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.White, System.Drawing.Color.White); // Blanco
                    styleNeutral.Font.FontColor = System.Drawing.Color.Black;

                    //crea los estilos
                    //estilo para ajustar al texto
                    SLStyle styleWrap = sl.CreateStyle();
                    styleWrap.SetWrapText(true);

                    //estilo para el encabezado
                    SLStyle styleHeader = sl.CreateStyle();
                    styleHeader.Font.Bold = true;
                    styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.ColorTranslator.FromHtml("#0094ff"), System.Drawing.ColorTranslator.FromHtml("#0094ff"));

                    SLStyle styleHeaderFont = sl.CreateStyle();
                    styleHeaderFont.Font.FontName = "Calibri";
                    styleHeaderFont.Font.FontSize = 11;
                    styleHeaderFont.Font.FontColor = System.Drawing.Color.White;
                    styleHeaderFont.Font.Bold = true;

                    //estilo para fecha
                    SLStyle styleLongDate = sl.CreateStyle();
                    styleLongDate.FormatCode = "dd/MM/yyyy hh:mm AM/PM";


                    System.Data.DataTable dt = new System.Data.DataTable();

                    // Agregar datos de lotes
                    dt.Columns.Add("ID", typeof(string));
                    dt.Columns.Add("Tipo", typeof(string));
                    dt.Columns.Add("Planta", typeof(string));
                    dt.Columns.Add("Storage Loc.", typeof(string));
                    dt.Columns.Add("Storage Bin", typeof(string));
                    dt.Columns.Add("Batch", typeof(string));
                    dt.Columns.Add("Material", typeof(string));
                    dt.Columns.Add("Número Antiguo", typeof(string));
                    dt.Columns.Add("Descripción", typeof(string));
                    dt.Columns.Add("Pieces", typeof(decimal));
                    dt.Columns.Add("Unrestricted", typeof(decimal));
                    dt.Columns.Add("Blocked", typeof(decimal));
                    dt.Columns.Add("Quality I.", typeof(decimal));
                    dt.Columns.Add("Gauge", typeof(decimal));
                    dt.Columns.Add("Gauge MIN", typeof(decimal));
                    dt.Columns.Add("Gauge MAX", typeof(decimal));
                    dt.Columns.Add("Altura (captura)", typeof(decimal));
                    dt.Columns.Add("Espesor (captura)", typeof(decimal));
                    dt.Columns.Add("Ubicación Fisica", typeof(string));
                    dt.Columns.Add("Piezas MIN", typeof(decimal));
                    dt.Columns.Add("Piezas MAX", typeof(decimal));
                    dt.Columns.Add("Piezas SAP", typeof(decimal));
                    dt.Columns.Add("Cantidad Teorica", typeof(decimal));
                    dt.Columns.Add("Diferencia Piezas", typeof(decimal));
                    dt.Columns.Add("Validación", typeof(string));
                    dt.Columns.Add("Comentarios", typeof(string));
                    dt.Columns.Add("Capturista", typeof(string));
                    dt.Columns.Add("Fecha", typeof(DateTime));

                    var nulo = DBNull.Value;

                    //agrega los lotes
                    foreach (var captura in capturas)
                    {
                        System.Data.DataRow row = dt.NewRow();
                        row["ID"] = captura.LoteId;
                        row["Tipo"] = "Individual";
                        row["Planta"] = captura.Planta;
                        row["Storage Loc."] = captura.StorageLocation;
                        row["Storage Bin"] = captura.StorageBin;
                        row["Batch"] = captura.Batch;
                        row["Material"] = captura.NumeroMaterial;
                        row["Número Antiguo"] = captura.NumeroAntiguo;
                        row["Descripción"] = captura.Descripcion;
                        row["Gauge"] = captura.Espesor;
                        row["Gauge MIN"] = captura.EspesorMin;
                        row["Gauge MAX"] = captura.EspesorMax;
                        row["Altura (captura)"] = captura.AlturaMedida.HasValue ? (object)captura.AlturaMedida.Value : DBNull.Value;
                        row["Espesor (captura)"] = captura.EspesorUsuario.HasValue ? (object)captura.EspesorUsuario.Value : DBNull.Value;
                        row["Ubicación Fisica"] = captura.UbicacionFisica != null ? captura.UbicacionFisica : captura.StorageBin;
                        row["Piezas MIN"] = captura.PiezasMin.HasValue ? (object)captura.PiezasMin.Value : DBNull.Value;
                        row["Piezas MAX"] = captura.PiezasMax.HasValue ? (object)captura.PiezasMax.Value : DBNull.Value;
                        row["Piezas SAP"] = captura.PiezasSAP;
                        row["Pieces"] = captura.PiezasSAP;
                        row["Unrestricted"] = captura.Unrestricted;
                        row["Blocked"] = captura.Blocked;
                        row["Quality I."] = captura.QualityInspection;

                        // Diferencia Piezas: Calcula solo si Altura Medida tiene valor
                        row["Diferencia Piezas"] = captura.AlturaMedida.HasValue
                            ? (captura.CantidadTeorica.HasValue
                                ? (object)(captura.CantidadTeorica.Value - captura.PiezasSAP)
                                : DBNull.Value)
                            : DBNull.Value;

                        row["Cantidad Teorica"] = captura.CantidadTeorica.HasValue ? (object)captura.CantidadTeorica.Value : DBNull.Value;
                        row["Validación"] = captura.Advertencia;
                        row["Comentarios"] = captura.Comentarios;
                        row["Capturista"] = captura.UsuarioCaptura;
                        row["Fecha"] = captura.CapturaFecha.HasValue ? (object)captura.CapturaFecha.Value : DBNull.Value;

                        dt.Rows.Add(row);

                        // Aplicar estilo de lote individual
                        int currentRow = dt.Rows.Count;
                        sl.SetRowStyle(currentRow + 1, styleLote);


                        // Aplicar estilo basado en "Validación"
                        SLStyle styleToApply;
                        switch (captura.Advertencia)
                        {
                            case "Ajustar":
                                styleToApply = styleAjustar;
                                break;
                            case "Pendiente":
                                styleToApply = stylePendiente;
                                break;
                            case "Dentro de Tolerancias":
                                styleToApply = styleDentroTolerancia;
                                break;
                            default:
                                styleToApply = styleNeutral;
                                break;
                        }

                        sl.SetCellStyle(currentRow + 1, dt.Columns["Validación"].Ordinal + 1, styleToApply);
                    }

                    //agrega los grupos
                    foreach (var grupo in capturasGrupos) {
                        System.Data.DataRow row = dt.NewRow();
                        
                        row["ID"] = grupo.EsResumen ?  grupo.GrupoId : string.Empty;
                        row["Tipo"] = grupo.EsResumen ? "Grupo" : "Grupo-Detalle";
                        row["Planta"] = grupo.Planta;
                        row["Storage Loc."] = grupo.StorageLocation;
                        row["Storage Bin"] = grupo.StorageBin;
                        row["Batch"] = grupo.Batch;
                        row["Material"] = grupo.NumeroMaterial;
                        row["Número Antiguo"] = grupo.NumeroAntiguo;
                        row["Descripción"] = grupo.Descripcion;
                        row["Gauge"] = grupo.Espesor;
                        row["Gauge MIN"] = grupo.EspesorMin;
                        row["Gauge MAX"] = grupo.EspesorMax;
                        row["Altura (captura)"] = grupo.AlturaMedida.HasValue ? (object)grupo.AlturaMedida.Value : DBNull.Value;
                        row["Espesor (captura)"] = grupo.EspesorUsuario.HasValue ? (object)grupo.EspesorUsuario.Value : DBNull.Value;
                        row["Ubicación Fisica"] = grupo.UbicacionFisica != null ? (object)grupo.UbicacionFisica : DBNull.Value;
                        row["Piezas MIN"] = grupo.PiezasMin.HasValue ? (object)grupo.PiezasMin.Value : DBNull.Value;
                        row["Piezas MAX"] = grupo.PiezasMax.HasValue ? (object)grupo.PiezasMax.Value : DBNull.Value;
                        row["Piezas SAP"] = grupo.PiezasSAP;
                        row["Pieces"] = grupo.PiezasSAP;
                        row["Unrestricted"] = grupo.Unrestricted;
                        row["Blocked"] = grupo.Blocked;
                        row["Quality I."] = grupo.QualityInspection;

                        //// Diferencia Piezas: Calcula solo si Altura Medida tiene valor
                        row["Diferencia Piezas"] = grupo.AlturaMedida.HasValue
                            ? (grupo.CantidadTeorica.HasValue
                                ? (object)(grupo.CantidadTeorica.Value - grupo.PiezasSAP)
                                : DBNull.Value)
                            : DBNull.Value;

                        row["Cantidad Teorica"] = grupo.CantidadTeorica.HasValue ? (object)grupo.CantidadTeorica.Value : DBNull.Value;
                        row["Validación"] = grupo.Advertencia;
                        row["Comentarios"] = grupo.Comentarios;
                        row["Capturista"] = grupo.UsuarioCaptura;
                        row["Fecha"] = grupo.CapturaFecha.HasValue ? (object)grupo.CapturaFecha.Value : DBNull.Value;

                        dt.Rows.Add(row);

                        // Aplicar estilo basado en si es resumen o lote dentro del grupo
                        int currentRow = dt.Rows.Count;
                        if (grupo.EsResumen)
                        {
                            sl.SetRowStyle(currentRow + 1, styleResumenGrupo);
                        }
                        else
                        {
                            sl.SetRowStyle(currentRow + 1, styleLoteGrupo);
                        }

                        SLStyle styleToApply;

                        switch (grupo.Advertencia)
                        {
                            case "Ajustar":
                                styleToApply = styleAjustar;
                                break;
                            case "Pendiente":
                                styleToApply = stylePendiente;
                                break;
                            case "Dentro de Tolerancias":
                                styleToApply = styleDentroTolerancia;
                                break;
                            default:
                                styleToApply = styleNeutral;
                                break;
                        }

                        sl.SetCellStyle(currentRow + 1, dt.Columns["Validación"].Ordinal + 1, styleToApply);
                    }

                    //agrega la tabla al documento
                    sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Reporte Inventario");
                    sl.ImportDataTable(1, 1, dt, true);

                    
                    sl.SetColumnStyle(dt.Columns["Fecha"].Ordinal + 1, styleLongDate);

                    //inmoviliza el encabezado
                    sl.FreezePanes(1, 0);

                    sl.Filter(1, 1, 1, dt.Columns.Count);
                    sl.AutoFitColumn(1, dt.Columns.Count);

                    sl.SetColumnStyle(1, dt.Columns.Count, styleWrap);
                    sl.SetRowStyle(1, styleHeader);
                    sl.SetRowStyle(1, styleHeaderFont);
                    sl.SetRowHeight(1, dt.Rows.Count + 1, 15.0);
                    // Exportar el archivo Excel
                    var stream = new System.IO.MemoryStream();
                    sl.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteCapturas.xlsx");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, $"Error al generar el reporte: {ex.Message}");
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
    public class CapturaGuardarViewModel
    {
        public List<CapturaViewModel> Lotes { get; set; }
        public List<CapturaGrupoViewModel> Grupos { get; set; }
    }

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
        public double? CantidadTeorica { get; set; }  // Cantidad estimada de piezas calculadas a partir de la altura medida
        public string Advertencia { get; set; }  // Advertencia si la cantidad estimada está fuera del rango
        public string Comentarios { get; set; } // Nueva propiedad para los comentarios
        public float? DiferenciaPiezas { get; set; } // Nueva propiedad para la diferencia entre PiezasSAP y CantidadTeorica
        public string UsuarioCaptura { get; set; }  // Nombre o ID del usuario que realizó la captura
        public int PiezasSAP { get; set; }  // piezasSAP
        public int Unrestricted { get; set; }  // Unrestricted
        public int Blocked { get; set; }  // Blocked
        public int QualityInspection { get; set; }  // QualityInspection

        // Campos capturados por el usuario
        public decimal? AlturaMedida { get; set; } // Campo capturable: Altura medida por el usuario
        public decimal? EspesorUsuario { get; set; } // Campo capturable: Espesor ingresado por el usuario
        public string UbicacionFisica { get; set; } // Campo capturable: Ubicación física
        public bool Selected { get; set; } = false; // Nueva propiedad para selección de lotes

        //campos para reporte
        public string NumeroAntiguo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? CapturaFecha { get; set; }

        // Campo para control de concurrencia
        [Timestamp]
        public byte[] RowVersion { get; set; }

        // Valores calculados con base en PiezasSAP, EspesorMin y EspesorMax
        public decimal? PiezasMin
        {
            get
            {
                return EspesorMax != 0 ? Math.Round((PiezasSAP * Espesor) / EspesorMax, 3) : (decimal?)null;
            }
        }

        public decimal? PiezasMax
        {
            get
            {
                return EspesorMin != 0 ? Math.Round((PiezasSAP * Espesor) / EspesorMin, 3) : (decimal?)null;
            }
        }
    }

    public class CapturaGrupoViewModel
    {
        public string GrupoId { get; set; }  // Código o identificador del grupo (puede ser generado automáticamente)
        public string LoteId { get; set; }  // Código o identificador del grupo (puede ser generado automáticamente)
        public string NumeroMaterial { get; set; }  // Número del material para todos los lotes del grupo (debe ser el mismo)
        public decimal? AlturaMedida { get; set; }  // Altura medida para el grupo
        public decimal? EspesorUsuario { get; set; }  // Espesor ingresado por el usuario para el grupo
        public string UbicacionFisica { get; set; }  // Ubicación física para el grupo
        public double? CantidadTeorica { get; set; }  // Cantidad teórica calculada para el grupo
        public int? PiezasSAP { get; set; }  // Número total de piezas en SAP para el grupo
        public int Unrestricted { get; set; }  // Unrestricted
        public int Blocked { get; set; }  // Blocked
        public int QualityInspection { get; set; }  // QualityInspection
        public decimal? PiezasMin { get; set; }  // Piezas mínimas permitidas para el grupo
        public decimal? PiezasMax { get; set; }  // Piezas máximas permitidas para el grupo
        public int? DiferenciaPiezas { get; set; }  // Diferencia entre piezas calculadas y piezas en SAP para el grupo
        public string Advertencia { get; set; }  // Estado de advertencia para el grupo
        public string Comentarios { get; set; }  // Comentarios para el grupo
        public List<int> LotesIds { get; set; }  // Lista de IDs de los lotes que pertenecen al grupo
        public bool EsResumen { get; set; }  // Indica si es un resumen del grupo (para identificar visualmente)

        public string Planta { get; set; }
        public string StorageLocation { get; set; }
        public string StorageBin { get; set; }
        public string Batch { get; set; }
        public decimal Espesor { get; set; }  // Espesor 
        public decimal EspesorMin { get; set; }  // Espesor mínimo del material, necesario para calcular las piezas
        public decimal EspesorMax { get; set; }  // Espesor máximo del material (para verificar el rango permitido)

        //campos para reporte
        public string NumeroAntiguo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? CapturaFecha { get; set; }
        public string UsuarioCaptura { get; set; }  // Nombre o ID del usuario que realizó la captura


    }


}