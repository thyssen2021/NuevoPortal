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

namespace Portal_2_0.Models
{
    public class SapSyncService
    {
        // El lock previene que dos hilos pidan el mismo material al mismo tiempo
        private static readonly ConcurrentDictionary<string, object> _materialLocks = new ConcurrentDictionary<string, object>();

        // Usamos un Func<T> para instanciar el DbContext solo cuando sea necesario
        // Esto evita problemas de DbContext en hilos (común en 'static')
        private readonly Func<Portal_2_0_ServicesEntities> _contextFactory;

        public SapSyncService()
        {
            _contextFactory = () => new Portal_2_0_ServicesEntities();
        }

        /// <summary>
        /// Flujo principal "On-Demand": Busca un material; si no existe, lo trae de SAP y lo guarda.
        /// Maneja la concurrencia.
        /// </summary>
        public async Task<bool> SyncMaterialOnDemandAsync(string materialId, List<string> plants)
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

     
            rfcFunction.SetValue("IV_GET_BATCH_CHARS", ""); // Vacío para NO obtener lotes

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
                // ... (Este código se copia/pega de tu SapRfcRepository.cs) ...

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
                            db.BomItems.Add(new BomItems
                            {
                                Matnr = m.Matnr,
                                Werks = p.Werks,
                                Alt_Bom = b.Alt_Bom,
                                Item_No = b.Item_No,
                                Component = b.Component,
                                Comp_Desc = b.Comp_Desc,
                                Quantity = (float?)b.Quantity,
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