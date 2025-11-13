using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DocumentFormat.OpenXml.Spreadsheet;
using SAP.Middleware.Connector;
using System.Windows.Media.Media3D;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace Portal_2_0.Models
{
    public class SapSyncService
    {
        // El lock previene que dos hilos pidan el mismo material al mismo tiempo
        private static readonly ConcurrentDictionary<string, object> _materialLocks = new ConcurrentDictionary<string, object>();

        // Usamos un Func<T> para instanciar el DbContext solo cuando sea necesario
        // Esto evita problemas de DbContext en hilos (común en 'static')
        private readonly Func<Portal_2_0_ServicesEntities> _contextFactory;
        private readonly Func<Portal_2_0Entities> _dbFactory;
        public SapSyncService()
        {
            _contextFactory = () => new Portal_2_0_ServicesEntities();
            _dbFactory = () => new Portal_2_0Entities(); // Instancia el contexto principal
        }

        /// <summary>
        /// Flujo principal "On-Demand" PÚBLICO.
        /// Llama al método privado con 'syncComponents = true' para iniciar la sincronización.
        /// </summary>
        public async Task<bool> SyncMaterialOnDemandAsync(string materialId, List<string> plants)
        {
            // Llama al método principal con 'syncComponents' en 'true' por defecto.
            return await SyncMaterialOnDemandAsync(materialId, plants, true);
        }

        /// <summary>
        /// Flujo principal "On-Demand": Busca un material; si no existe, lo trae de SAP y lo guarda.
        /// Maneja la concurrencia.
        /// </summary>
        private async Task<bool> SyncMaterialOnDemandAsync(string materialId, List<string> plants, bool syncComponents)
        {
            materialId = materialId.ToUpper();

            // 1. Obtener el 'lock' específico para este material
            object materialLock = _materialLocks.GetOrAdd(materialId, (key) => new object());

            // 2. Ejecutar la lógica de sincronización dentro del lock
            // Usamos un Task.Run para mover la operación (que puede ser bloqueante)
            // fuera del hilo principal de la solicitud web, previniendo bloqueos.
            return await Task.Run(async () =>
            {
                lock (materialLock)
                {
                    try
                    {
                        // 3. DOBLE VERIFICACIÓN (DENTRO DEL LOCK)
                        // Quizás otro hilo lo insertó mientras esperábamos el lock.
                        using (var db = _contextFactory())
                        {
                            if (db.Materials.Any(m => m.Matnr == materialId))
                            {
                                return true; // El material ya fue sincronizado por otro hilo.
                            }
                        }

                        // 4. LLAMADA RFC (Real-time)
                        // Esta es la lógica adaptada de tu SapRfcRepository.cs
                        var materialsFromSap = CallRfcToGetMaterial(materialId, plants);

                        if (materialsFromSap == null || !materialsFromSap.Any())
                        {
                            // SAP no encontró nada o falló.
                            return false;
                        }

                        // 5. WRITE-BACK (Guardar en BD local)
                        // Esta es la lógica adaptada de tu SapDbRepository.cs
                        SaveMaterialsToDatabase(materialsFromSap, DateTime.Now);

                        // Solo ejecutar este bloque si syncComponents es 'true' (la primera llamada)
                        if (syncComponents)
                        {
                            try
                            {
                                var allComponentsToSync = new List<string>();
                                foreach (var mat in materialsFromSap)
                                {
                                    foreach (var plant in mat.Plants)
                                    {
                                        foreach (var bomItem in plant.BomItems)
                                        {
                                            if (!string.IsNullOrEmpty(bomItem.Component) && !bomItem.Component.StartsWith("sm"))
                                            {
                                                allComponentsToSync.Add(bomItem.Component.ToUpper());
                                            }
                                        }
                                    }
                                }

                                // Obtenemos los componentes únicos
                                var uniqueComponents = allComponentsToSync.Distinct().ToList();

                                if (uniqueComponents.Any())
                                {
                                    Task.Run(async () =>
                                    {
                                        var syncTasks = new List<Task>();
                                        foreach (var componentId in uniqueComponents)
                                        {
                                            // --- INICIO MODIFICACIÓN 4 ---
                                            // Llamada recursiva con 'syncComponents = false' para DETENER
                                            // la recursión en el siguiente nivel.
                                            syncTasks.Add(SyncMaterialOnDemandAsync(componentId, plants, false));
                                            // --- FIN MODIFICACIÓN 4 ---
                                        }
                                        await Task.WhenAll(syncTasks);
                                    }).Wait(); // .Wait() es crucial aquí
                                }
                            }
                            catch (Exception ex_recursive)
                            {
                                // Loguear error de la sincronización recursiva, pero no fallar la principal
                                // LogWriter.LogInfo($"<!!!> [SapSyncService] ERROR Sinc. Recursiva para '{materialId}': {ex_recursive.Message}");
                            }
                        } // <-- Fin del 'if (syncComponents)'

                        // --- INICIO: Calcular y Guardar Pesos BOM ---
                        try
                        {
                            // Obtenemos los materiales (platinas) que acabamos de sincronizar
                            var platinasSincronizadas = materialsFromSap
                                .Where(m => !m.Matnr.StartsWith("sm")) // Excluir 'sm'
                                .Select(m => m.Matnr)
                                .Distinct()
                                .ToList();

                            if (platinasSincronizadas.Any())
                            {
                                // Llamamos a la nueva lógica de cálculo (no es async, se ejecuta síncrono)
                                CalculateAndSaveBomWeights(platinasSincronizadas);
                            }
                        }
                        catch (Exception ex_pesos)
                        {
                            // Loguear error de cálculo de pesos, pero no fallar la sincronización principal
                            // LogWriter.LogInfo($"<!!!> [SapSyncService] ERROR Calculando Pesos BOM para '{materialId}': {ex_pesos.Message}");
                        }
                        // --- FIN Calulo BOM ---

                        return true;
                    }
                    catch (Exception ex)
                    {
                        // 6. FALLBACK
                        // Ocurrió un error (SAP caído, RFC falló, BD falló).
                        // Se loguea el error pero NO se lanza la excepción,
                        // para que el controlador pueda usar el fallback.
                        // LogWriter.LogInfo($"<!!!> [SapSyncService] ERROR On-Demand para '{materialId}': {ex.Message}");
                        return false;
                    }
                    finally
                    {
                        // 7. Liberar el lock
                        _materialLocks.TryRemove(materialId, out _);
                    }
                }
            });
        }

        /// <summary>
        /// (Adaptado de SapRfcRepository) Llama a Z_RFC_GET_MATERIAL_DETAILS
        /// </summary>
        private List<MaterialDetailsViewModel> CallRfcToGetMaterial(string materialPattern, List<string> plants)
        {
            var resultList = new List<MaterialDetailsViewModel>();
            IRfcFunction rfcFunction = RfcConnectionService.CreateFunction("Z_RFC_GET_MATERIAL_DETAILS");

            // --- Poblar Filtros (S_MATNR y S_WERKS) ---
            IRfcTable matnrTable = rfcFunction.GetTable("S_MATNR");
            matnrTable.Append();
            matnrTable.SetValue("SIGN", "I");
            matnrTable.SetValue("OPTION", materialPattern.Contains("*") ? "CP" : "EQ");
            matnrTable.SetValue("LOW", materialPattern.ToUpper());

            IRfcTable werksTable = rfcFunction.GetTable("S_WERKS");
            if (plants != null && plants.Any())
            {
                foreach (var plant in plants.Where(p => !string.IsNullOrWhiteSpace(p)))
                {
                    werksTable.Append();
                    werksTable.SetValue("SIGN", "I");
                    werksTable.SetValue("OPTION", "EQ");
                    werksTable.SetValue("LOW", plant.Trim().ToUpper());
                }
            }

            // Vacío para NO obtener lotes
            rfcFunction.SetValue("IV_GET_BATCH_CHARS", ""); 

            // --- Invocar la función RFC (Bloqueante, por eso está en Task.Run) ---
            RfcConnectionService.InvokeFunction(rfcFunction);

            // --- Procesar Resultados (Mapeo idéntico a tu SapRfcRepository) ---
            IRfcTable resultsTable = rfcFunction.GetTable("E_MATERIALS");

            foreach (IRfcStructure materialRow in resultsTable)
            {
                var materialVM = new MaterialDetailsViewModel
                {
                    // (Mapeo de todos los campos de ZSTR_RFC_MATERIAL_DETAILS...
                    // Este código se copia/pega de tu SapRfcRepository.cs)
                    Matnr = materialRow.GetString("MATNR"),
                    Mtart = materialRow.GetString("MTART"),
                    Bismt = materialRow.GetString("BISMT"),
                    Brgew = ParseSapDecimal(materialRow.GetString("BRGEW")) ?? 0M,
                    Ntgwe = ParseSapDecimal(materialRow.GetString("NTGEW")) ?? 0M,
                    Meins = materialRow.GetString("MEINS"),
                    Groes = materialRow.GetString("GROES"),
                    Zzreappl = materialRow.GetString("ZZREAPPL"),
                    Zzmtltyp = materialRow.GetString("ZZMTLTYP"),
                    Zzselltyp = materialRow.GetString("ZZSELLTYP"),
                    Zzbussmodl = materialRow.GetString("ZZBUSSMODL"),
                    Vmsta = materialRow.GetString("VMSTA"),
                    Gewei = materialRow.GetString("GEWEI"),
                    Zzmattype = materialRow.GetString("ZZMATTYP"),
                    Zzcustscrp = materialRow.GetString("ZZCUSTSCRP"),
                    Zzengscrp = materialRow.GetString("ZZENGSCRP"),
                    Zzihsnum1 = materialRow.GetString("ZZIHSNUM1"),
                    Zzihsnum2 = materialRow.GetString("ZZIHSNUM2"),
                    Zzihsnum3 = materialRow.GetString("ZZIHSNUM3"),
                    Zzihsnum4 = materialRow.GetString("ZZIHSNUM4"),
                    Zzihsnum5 = materialRow.GetString("ZZIHSNUM5"),
                    Zzpkgpcs = materialRow.GetString("ZZPKGPCS"),
                    Zzthcknss = ParseSapDecimal(materialRow.GetString("ZZTHCKNSS")),
                    Zzwidth = ParseSapDecimal(materialRow.GetString("ZZWIDTH")),
                    Zzadvance = ParseSapDecimal(materialRow.GetString("ZZADVANCE")),
                    Zzhtalscrp = ParseSapDecimal(materialRow.GetString("ZZHTALSCRP")),
                    Zzcarpcs = ParseSapDecimal(materialRow.GetString("ZZCARPCS")),
                    Zzinitwt = ParseSapDecimal(materialRow.GetString("ZZINITWT")),
                    Zzminwt = ParseSapDecimal(materialRow.GetString("ZZMINWT")),
                    Zzmaxwt = ParseSapDecimal(materialRow.GetString("ZZMAXWT")),
                    Zzstkpcs = materialRow.GetString("ZZSTKPCS"),
                    Zzanglea = ParseSapDecimal(materialRow.GetString("ZZANGLEA")),
                    Zzangleb = ParseSapDecimal(materialRow.GetString("ZZANGLEB")),
                    Zzrealntwt = ParseSapDecimal(materialRow.GetString("ZZREALNTWT")),
                    Zzrealgrwt = ParseSapDecimal(materialRow.GetString("ZZREALGRWT")),
                    Zzdouopcs = materialRow.GetString("ZZDOUPCS"),
                    Zzcoilsltpos = materialRow.GetString("ZZCOILSLTPOS"),
                    Zzmxwttolp = ParseSapDecimal(materialRow.GetString("ZZMXWTTOLP")),
                    Zzmxwttoln = ParseSapDecimal(materialRow.GetString("ZZMXWTTOLN")),
                    Zzmnwttolp = ParseSapDecimal(materialRow.GetString("ZZMNWTTOLP")),
                    Zzmnwttoln = ParseSapDecimal(materialRow.GetString("ZZMNWTTOLN")),
                    Zzwh = materialRow.GetString("ZZWH"),
                    Zztransp = materialRow.GetString("ZZTRANSP"),
                    Zztkmmsop = materialRow.GetString("ZZTKMMSOP"),
                    Zztkmmeop = materialRow.GetString("ZZTKMMEOP"),
                    Zzppackage = materialRow.GetString("ZZPPACKAGE"),
                    Zzspackage = materialRow.GetString("ZZSPACKAGE"),
                    Zzpallet = materialRow.GetString("ZZPALLET"),
                    Zzstamd = materialRow.GetString("ZZSTAMD"),
                    Zzidpnum = materialRow.GetString("ZZIDPNUM"),
                    Zzidtool = materialRow.GetString("ZZIDTOOL"),
                    Zzidobsol = materialRow.GetString("ZZIDOBSOL"),
                    Zztourd = materialRow.GetString("ZZTOURD"),
                    Zztolmaxwt = materialRow.GetString("ZZTOLMAXWT"),
                    Zztolminwt = materialRow.GetString("ZZTOLMINWT"),

                    Descriptions = new List<DescriptionDataViewModel>(),
                    Plants = new List<PlantDataViewModel>(),
                    Characteristics = new List<CharDataViewModel>(),
                    Batches = new List<BatchDataViewModel>()
                };

                // (Mapeo de tablas anidadas: DESC_DATA, PLANT_DATA, BOM_DATA, CHAR_DATA, BATCH_DATA, BATCH_CHARS)         

                // Leer tabla anidada de Descripciones (DESC_DATA)
                IRfcTable descTable = materialRow.GetTable("DESC_DATA");
                foreach (IRfcStructure descRow in descTable)
                {
                    materialVM.Descriptions.Add(new DescriptionDataViewModel
                    {
                        Spras = descRow.GetString("SPRAS"),
                        Maktx = descRow.GetString("MAKTX")
                    });
                }

                // Leer tabla anidada de Plantas (PLANT_DATA)
                IRfcTable plantTable = materialRow.GetTable("PLANT_DATA");
                foreach (IRfcStructure plantRow in plantTable)
                {
                    var plantVM = new PlantDataViewModel
                    {
                        Werks = plantRow.GetString("WERKS"),
                        Mmsta = plantRow.GetString("MMSTA"),
                        BomItems = new List<BomItemViewModel>()
                    };

                    // Leer tabla anidada de BOMs (BOM_DATA)
                    IRfcTable bomTable = plantRow.GetTable("BOM_DATA");
                    foreach (IRfcStructure bomRow in bomTable)
                    {
                        plantVM.BomItems.Add(new BomItemViewModel
                        {
                            Alt_Bom = bomRow.GetString("ALT_BOM"),
                            Item_No = bomRow.GetString("ITEM_NO"),
                            Component = bomRow.GetString("COMPONENT"),
                            Comp_Desc = bomRow.GetString("COMP_DESC"),
                            Quantity = ParseSapDecimal(bomRow.GetString("QUANTITY")) ?? 0M,
                            Uom = bomRow.GetString("UOM"),
                            Valid_From = ParseSapDate(bomRow.GetString("VALID_FROM")),
                            Created_On = ParseSapDate(bomRow.GetString("CREATED_ON"))
                        });
                    }
                    materialVM.Plants.Add(plantVM);
                }

                // Leer tabla anidada de Características (CHAR_DATA)
                IRfcTable charTable = materialRow.GetTable("CHAR_DATA");
                foreach (IRfcStructure charRow in charTable)
                {
                    materialVM.Characteristics.Add(new CharDataViewModel
                    {
                        Charact = charRow.GetString("CHARACT"),
                        Charact_Desc_En = charRow.GetString("CHARACT_DESC_EN"),
                        Charact_Desc_Es = charRow.GetString("CHARACT_DESC_ES"),
                        Value_Internal = charRow.GetString("VALUE_INTERNAL"),
                        Value_Desc_En = charRow.GetString("VALUE_DESC_EN"),
                        Value_Desc_Es = charRow.GetString("VALUE_DESC_ES"),
                        Unit = charRow.GetString("UNIT")
                    });
                }

                // Mapeo Condicional de Lotes
                IRfcTable batchTable = materialRow.GetTable("BATCH_DATA");
                foreach (IRfcStructure batchRow in batchTable)
                {
                    var batchVM = new BatchDataViewModel
                    {
                        Charg = batchRow.GetString("CHARG"),
                        Werks = batchRow.GetString("WERKS"),
                        Ersda = ParseSapDate(batchRow.GetString("ERSDA")),
                        Vfdat = ParseSapDate(batchRow.GetString("VFDAT")),
                        BatchChars = new List<CharDataViewModel>()
                    };


                    IRfcTable batchCharTable = batchRow.GetTable("BATCH_CHARS");
                    foreach (IRfcStructure batchCharRow in batchCharTable)
                    {
                        batchVM.BatchChars.Add(new CharDataViewModel
                        {
                            Charact = batchCharRow.GetString("CHARACT"),
                            Charact_Desc_En = batchCharRow.GetString("CHARACT_DESC_EN"),
                            Charact_Desc_Es = batchCharRow.GetString("CHARACT_DESC_ES"),
                            Value_Internal = batchCharRow.GetString("VALUE_INTERNAL"),
                            Value_Desc_En = batchCharRow.GetString("VALUE_DESC_EN"),
                            Value_Desc_Es = batchCharRow.GetString("VALUE_DESC_ES"),
                            Unit = batchCharRow.GetString("UNIT")
                        });
                    }
                    materialVM.Batches.Add(batchVM);
                }

                resultList.Add(materialVM);
            }

            return resultList;
        }

        /// <summary>
        /// Trunca un valor decimal a un número específico de decimales.
        /// </summary>
        private decimal TruncateDecimal(decimal value, int precision)
        {
            decimal step = (decimal)Math.Pow(10, precision);
            return Math.Truncate(value * step) / step;
        }

        /// <summary>
        /// (Adaptado de SapDbRepository) Guarda los ViewModels en la BD.
        /// NO usa Staging/Merge, inserta directamente.
        /// </summary>
        private void SaveMaterialsToDatabase(List<MaterialDetailsViewModel> materialsFragment, DateTime syncTimestamp)
        {
            using (var db = _contextFactory())
            {
                foreach (var m in materialsFragment)
                {
                    // 1. Mapear Material (Padre)
                    var materialEntity = new Portal_2_0.Models.Materials
                    {
                        Matnr = m.Matnr,
                        LastSyncTimestamp = syncTimestamp,
                        Mtart = m.Mtart,
                        Bismt = m.Bismt,
                        Brgew = (float?)m.Brgew,
                        Ntgwe = (float?)m.Ntgwe,
                        Meins = m.Meins,
                        Groes = m.Groes,
                        Vmsta = m.Vmsta,
                        Gewei = m.Gewei,
                        ZZMATTYP = m.Zzmattype,
                        ZZCUSTSCRP = ParseSapBit(m.Zzcustscrp),
                        ZZENGSCRP = ParseSapBit(m.Zzengscrp),
                        ZZIHSNUM1 = m.Zzihsnum1,
                        ZZIHSNUM2 = m.Zzihsnum2,
                        ZZIHSNUM3 = m.Zzihsnum3,
                        ZZIHSNUM4 = m.Zzihsnum4,
                        ZZIHSNUM5 = m.Zzihsnum5,
                        ZZPKGPCS = ParseSapFloat(m.Zzpkgpcs),
                        ZZTHCKNSS = (float?)m.Zzthcknss,
                        ZZWIDTH = (float?)m.Zzwidth,
                        ZZADVANCE = (float?)m.Zzadvance,
                        ZZHTALSCRP = (float?)m.Zzhtalscrp,
                        ZZCARPCS = (float?)m.Zzcarpcs,
                        ZZINITWT = (float?)m.Zzinitwt,
                        ZZMINWT = (float?)m.Zzminwt,
                        ZZMAXWT = (float?)m.Zzmaxwt,
                        ZZSTKPCS = ParseSapFloat(m.Zzstkpcs),
                        ZZANGLEA = (float?)m.Zzanglea,
                        ZZANGLEB = (float?)m.Zzangleb,
                        ZZREALNTWT = (float?)m.Zzrealntwt,
                        ZZREALGRWT = (float?)m.Zzrealgrwt,
                        ZZDOUPCS = ParseSapBit(m.Zzdouopcs),
                        ZZCOILSLTPOS = m.Zzcoilsltpos,
                        ZZMXWTTOLP = (float?)m.Zzmxwttolp,
                        ZZMXWTTOLN = (float?)m.Zzmxwttoln,
                        ZZMNWTTOLP = (float?)m.Zzmnwttolp,
                        ZZMNWTTOLN = (float?)m.Zzmnwttoln,
                        ZZWH = ParseSapBit(m.Zzwh),
                        ZZTRANSP = m.Zztransp,
                        ZZTKMMSOP = m.Zztkmmsop,
                        ZZTKMMEOP = m.Zztkmmeop,
                        ZZPPACKAGE = ParseSapFloat(m.Zzppackage),
                        ZZSPACKAGE = ParseSapFloat(m.Zzspackage),
                        ZZREAPPL = ParseSapBit(m.Zzreappl),
                        ZZSTAMD = m.Zzstamd,
                        ZZIDPNUM = m.Zzidpnum,
                        ZZIDTOOL = m.Zzidtool,
                        ZZIDOBSOL = m.Zzidobsol,
                        ZZTOURD = m.Zztourd,
                        ZZTOLMAXWT = m.Zztolmaxwt,
                        ZZTOLMINWT = m.Zztolminwt,
                        ZZPALLET = m.Zzpallet,
                        ZZMTLTYP = m.Zzmtltyp,
                        ZZSELLTYP = m.Zzselltyp,
                        ZZBUSSMODL = m.Zzbussmodl
                    };
                    db.Materials.Add(materialEntity);

                    // 2. Mapear Descripciones
                    foreach (var d in m.Descriptions)
                    {
                        db.MaterialDescriptions.Add(new Portal_2_0.Models.MaterialDescriptions
                        {
                            Matnr = m.Matnr,
                            Spras = d.Spras,
                            Maktx = d.Maktx,
                            LastSyncTimestamp = syncTimestamp
                        });
                    }

                    // 3. Mapear Plantas
                    foreach (var p in m.Plants)
                    {
                        db.MaterialPlants.Add(new MaterialPlants
                        {
                            Matnr = m.Matnr,
                            Werks = p.Werks,
                            Mmsta = p.Mmsta,
                            LastSyncTimestamp = syncTimestamp
                        });

                        // 4. Mapear BOM Items (Anidados en Plantas)
                        foreach (var b in p.BomItems)
                        {
                            decimal qty_truncada_decimal = TruncateDecimal(b.Quantity, 3);


                            db.BomItems.Add(new BomItems
                            {
                                Matnr = m.Matnr,
                                Werks = p.Werks,
                                Alt_Bom = b.Alt_Bom,
                                Item_No = b.Item_No,
                                Component = b.Component,
                                Comp_Desc = b.Comp_Desc,
                                Quantity = qty_truncada_decimal,
                                Uom = b.Uom,
                                Valid_From = b.Valid_From,
                                Created_On = b.Created_On,
                                LastSyncTimestamp = syncTimestamp
                            });
                        }
                    }

                    // 5. Mapear Características
                    foreach (var c in m.Characteristics.GroupBy(x => x.Charact).Select(g => g.First()))
                    {
                        db.MaterialCharacteristics.Add(new MaterialCharacteristics
                        {
                            Matnr = m.Matnr,
                            Charact = c.Charact,
                            Charact_Desc_En = c.Charact_Desc_En,
                            Charact_Desc_Es = c.Charact_Desc_Es,
                            Value_Internal = c.Value_Internal,
                            Value_Desc_En = c.Value_Desc_En,
                            Value_Desc_Es = c.Value_Desc_Es,
                            Unit = c.Unit,
                            LastSyncTimestamp = syncTimestamp
                        });
                    }

                    // 6. Mapear Lotes (Batches)
                    foreach (var b in m.Batches)
                    {
                        db.Batches.Add(new Batches
                        {
                            Matnr = m.Matnr,
                            Werks = b.Werks,
                            Charg = b.Charg,
                            Ersda = b.Ersda,
                            Vfdat = b.Vfdat,
                            LastSyncTimestamp = syncTimestamp
                        });

                        // 7. Mapear Características de Lote (Anidadas en Lotes)
                        foreach (var bc in b.BatchChars.GroupBy(x => x.Charact).Select(g => g.First()))
                        {
                            db.BatchCharacteristics.Add(new BatchCharacteristics
                            {
                                Matnr = m.Matnr,
                                Werks = b.Werks,
                                Charg = b.Charg,
                                Charact = bc.Charact,
                                Charact_Desc_En = bc.Charact_Desc_En,
                                Charact_Desc_Es = bc.Charact_Desc_Es,
                                Value_Internal = bc.Value_Internal,
                                Value_Desc_En = bc.Value_Desc_En,
                                Value_Desc_Es = bc.Value_Desc_Es,
                                Unit = bc.Unit,
                                LastSyncTimestamp = syncTimestamp
                            });
                        }
                    }
                } // Fin foreach material

                db.SaveChanges();
            }
        }

        /// <summary>
        /// (Adaptado de CargaBom) Calcula los pesos Bruto/Neto basados en SAP.BomItems 
        /// y los guarda en Portal_2_0.bom_pesos.
        /// </summary>
        private void CalculateAndSaveBomWeights(List<string> materialIds)
        {
            var pesosParaSincronizar = new List<bom_pesos>();

            // Usamos ambos contextos
            using (var db_sap = _contextFactory())
            using (var db = _dbFactory())
            {
                // 1. Obtener los BOMs de los materiales recién sincronizados
                var bomItems = db_sap.BomItems
                   .Where(b => materialIds.Contains(b.Matnr) &&
                                b.Quantity.HasValue && b.Quantity != 0 && 
                                !b.Component.StartsWith("SM"))
                    .ToList();

                // 2. Agrupar por Material y Planta
                var bomGroups = bomItems.GroupBy(b => new { b.Matnr, b.Werks });

                // 3. Cargar pesos existentes de la BD principal (db)
                var pesosDict = db.bom_pesos
                    .Where(p => materialIds.Contains(p.material))
                    .ToDictionary(p => $"{p.plant}|{p.material}", p => p);

                foreach (var group in bomGroups)
                {
                    var listTemporalBOM = group.ToList();

                    // 4. Lógica de cálculo (Adaptada a 'Valid_From' y regla -0.001)

                    // Encontramos la fecha MÁXIMA de inicio de validez (el BOM más reciente)
                    // (Se ignora el 'Created_On' que se usaba en CargaBom)
                    DateTime? fechaValida = listTemporalBOM.Max(b => b.Valid_From);

                    double? peso_neto_final = null;
                    double? peso_bruto_final = null;

                    if (fechaValida.HasValue)
                    {
                        var bomVersionActual = group.Where(x => x.Valid_From == fechaValida).ToList();


                        // Peso Bruto: El máximo componente positivo (Máximo de decimales)
                        decimal pesoBrutoDecimal = bomVersionActual
                                     .Where(x => x.Quantity > 0)
                                     .Select(x => x.Quantity.Value) // Aseguramos el valor decimal
                                     .DefaultIfEmpty(0m)
                                     .Max();

                        // 1. Sumamos solo los componentes negativos (excluyendo -0.001m)
                        decimal sumOfNegatives = bomVersionActual
                            .Where(x => x.Quantity < 0m && x.Quantity.Value != -0.001m)
                            .Sum(x => x.Quantity.Value);

                        decimal pesoNetoDecimal = pesoBrutoDecimal + sumOfNegatives;

                        // Asignamos a las variables que serán truncadas y guardadas
                        peso_bruto_final = (double)pesoBrutoDecimal;
                        peso_neto_final = (double)pesoNetoDecimal;

                    }

                    // 5. Preparar la lista para MERGE
                    if (peso_neto_final.HasValue && peso_bruto_final.HasValue)
                    {
                        double peso_neto_truncado = Math.Truncate(peso_neto_final.Value * 1000) / 1000.0;
                        double peso_bruto_truncado = Math.Truncate(peso_bruto_final.Value * 1000) / 1000.0;

                        // ... (Lógica de Debug.WriteLine y Merge) ...
                        string key = $"{group.Key.Werks}|{group.Key.Matnr}";

                        if (pesosDict.TryGetValue(key, out var pesoExistente))
                        {
                            if (pesoExistente.net_weight != peso_neto_truncado || pesoExistente.gross_weight != peso_bruto_truncado)
                            {
                                Debug.WriteLine($"[SapSyncService.BomWeights] ACTUALIZANDO (Pesos cambiaron): Material={group.Key.Matnr}, Planta={group.Key.Werks}. NETO: {pesoExistente.net_weight} -> {peso_neto_truncado}, BRUTO: {pesoExistente.gross_weight} -> {peso_bruto_truncado}");
                                pesoExistente.net_weight = peso_neto_truncado;
                                pesoExistente.gross_weight = peso_bruto_truncado;
                                pesosParaSincronizar.Add(pesoExistente);
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"[SapSyncService.BomWeights] INSERTANDO (Nuevo): Material={group.Key.Matnr}, Planta={group.Key.Werks}. NETO: {peso_neto_truncado}, BRUTO: {peso_bruto_truncado}");
                            pesosParaSincronizar.Add(new bom_pesos
                            {
                                plant = group.Key.Werks,
                                material = group.Key.Matnr,
                                gross_weight = peso_bruto_truncado,
                                net_weight = peso_neto_truncado,
                            });
                        }
                    }
                } // fin foreach group

                // 6. Sincronizar (Insertar/Actualizar) pesos usando MERGE
                if (pesosParaSincronizar.Any())
                {
                    MergeBomPesos(db, pesosParaSincronizar);
                }
            }
        }

        /// <summary>
        /// (Adaptado de CargaBom) Ejecuta un BulkCopy a #Temp y luego un MERGE en bom_pesos.
        /// </summary>
        private void MergeBomPesos(Portal_2_0Entities db, List<bom_pesos> pesosParaSincronizar)
        {
            // Obtenemos la conexión del contexto 'db' (Portal_2_0)
            using (var transaction = db.Database.BeginTransaction())
            {
                var sqlConnection = db.Database.Connection as SqlConnection;
                var sqlTransaction = transaction.UnderlyingTransaction as SqlTransaction;

                try
                {
                    // 1. Convertir a DataTable (Usando el helper)
                    System.Data.DataTable dataTablePesos = ConvertToDataTable(pesosParaSincronizar, new HashSet<string> { "plant", "material", "gross_weight", "net_weight" });

                    // 2. Subir datos a #TempPesos
                    using (var bulkCopyPesos = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, sqlTransaction))
                    {
                        bulkCopyPesos.DestinationTableName = "#TempPesos";
                        bulkCopyPesos.ColumnMappings.Add("plant", "plant");
                        bulkCopyPesos.ColumnMappings.Add("material", "material");
                        bulkCopyPesos.ColumnMappings.Add("gross_weight", "gross_weight");
                        bulkCopyPesos.ColumnMappings.Add("net_weight", "net_weight");

                        // 3. Crear la tabla temporal
                        db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, @"
                            CREATE TABLE #TempPesos (
                                [plant] [varchar](4) NOT NULL,
                                [material] [varchar](10) NOT NULL,
                                [gross_weight] [float] NOT NULL,
                                [net_weight] [float] NOT NULL
                            );");

                        bulkCopyPesos.WriteToServer(dataTablePesos);
                    }

                    // 4. Ejecutar el MERGE (Actualiza la tabla 'bom_pesos' con versión de sistema)
                    string mergeSql = @"
                        MERGE INTO bom_pesos AS T
                        USING #TempPesos AS S
                        ON (T.plant = S.plant AND T.material = S.material)
                        WHEN MATCHED THEN
                            UPDATE SET
                                T.gross_weight = S.gross_weight,
                                T.net_weight = S.net_weight
                        WHEN NOT MATCHED BY TARGET THEN
                            INSERT (plant, material, gross_weight, net_weight)
                            VALUES (S.plant, S.material, S.gross_weight, S.net_weight);
                    ";

                    db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, mergeSql);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw; // Lanza la excepción para que el log principal la capture
                }
            }
        }

        private DataTable ConvertToDataTable<T>(IEnumerable<T> data, HashSet<string> allowedProps = null)
        {
            DataTable table = new DataTable();
            if (!data.Any()) return table;

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Filtra las propiedades si se proporciona una lista
            if (allowedProps != null)
            {
                props = props.Where(p => allowedProps.Contains(p.Name)).ToArray();
            }

            foreach (var prop in props)
            {
                Type propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                table.Columns.Add(prop.Name, propType);
            }
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (var prop in props)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        #region Helpers SAP Data Parsing (Copiados de tus archivos)

        private DateTime? ParseSapDate(string sapDateStr)
        {
            if (string.IsNullOrWhiteSpace(sapDateStr) || sapDateStr == "0000-00-00" || sapDateStr == "00000000") return null;
            if (DateTime.TryParseExact(sapDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result)) return result;
            if (DateTime.TryParseExact(sapDateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result)) return result;
            return null;
        }

        private decimal? ParseSapDecimal(string sapDecimalStr)
        {
            if (string.IsNullOrWhiteSpace(sapDecimalStr)) return null;
            string invariantDecimalStr = sapDecimalStr.Trim().Replace(',', '.');
            if (decimal.TryParse(invariantDecimalStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result)) return result;
            return null;
        }

        private static bool? ParseSapBit(string sapFlag)
        {
            if (string.IsNullOrWhiteSpace(sapFlag)) return null;
            return true; // Asumimos que cualquier valor no vacío es true
        }

        private static float? ParseSapFloat(string sapFloatStr)
        {
            if (string.IsNullOrWhiteSpace(sapFloatStr)) return null;
            string invariantFloatStr = sapFloatStr.Trim().Replace(',', '.');
            if (float.TryParse(invariantFloatStr, NumberStyles.Any, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }
            return null;
        }
        #endregion
    }
}