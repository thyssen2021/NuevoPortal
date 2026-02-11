using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal_2_0.Models;
using SAP.Middleware.Connector;

namespace Portal_2_0.Controllers
{
    public class SapPocController : Controller
    {
        // GET: SapPoc
        // Esta acción muestra la página por primera vez (el formulario vacío)
        public ActionResult Index()
        {
            var model = new SapMaterialViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SapMaterialViewModel model)
        {
            model.QueryExecuted = true;

            if (string.IsNullOrWhiteSpace(model.MaterialNumber))
            {
                model.ErrorMessage = "Por favor, ingresa un número de material.";
                return View(model);
            }

            try
            {
                RfcConfigParameters connParams = new RfcConfigParameters();
                connParams.Add(RfcConfigParameters.Name, ConfigurationManager.AppSettings["SapSystemName"]);
                connParams.Add(RfcConfigParameters.AppServerHost, ConfigurationManager.AppSettings["SapServer"]);
                connParams.Add(RfcConfigParameters.SystemNumber, ConfigurationManager.AppSettings["SapSystemNumber"]);
                connParams.Add(RfcConfigParameters.User, ConfigurationManager.AppSettings["SapUser"]);
                connParams.Add(RfcConfigParameters.Password, ConfigurationManager.AppSettings["SapPassword"]);
                connParams.Add(RfcConfigParameters.Client, ConfigurationManager.AppSettings["SapClient"]);
                connParams.Add(RfcConfigParameters.Language, ConfigurationManager.AppSettings["SapLanguage"]);
                connParams.Add(RfcConfigParameters.SAPRouter, ConfigurationManager.AppSettings["SapRouter"]);

                RfcDestination destination = RfcDestinationManager.GetDestination(connParams);
                IRfcFunction rfcFunction = destination.Repository.CreateFunction("Z_GET_MATERIAL_BASIC_DATA");
                rfcFunction.SetValue("I_MATNR", model.MaterialNumber);
                rfcFunction.Invoke(destination);

                // --- 5. RECUPERAR LOS RESULTADOS (VERSIÓN ACTUALIZADA) ---
                string descriptionEN = rfcFunction.GetString("E_MAKTX");

                if (string.IsNullOrWhiteSpace(descriptionEN))
                {
                    model.ErrorMessage = $"El material '{model.MaterialNumber}' no fue encontrado en SAP.";
                }
                else
                {
                    // Asignamos todos los valores al modelo
                    model.DescriptionEN = descriptionEN;
                    model.DescriptionES = rfcFunction.GetString("E_MAKTX_ES");
                    model.OldMaterialNumber = rfcFunction.GetString("E_BISMT");
                    model.GrossWeight = rfcFunction.GetString("E_BRGEW");
                    model.NetWeight = rfcFunction.GetString("E_NTGEW");
                    model.Dimensions = rfcFunction.GetString("E_GROES");
                    model.MaterialType = rfcFunction.GetString("E_MTART");
                    model.UnitOfMeasure = rfcFunction.GetString("E_MEINS");
                }
            }
            catch (RfcCommunicationException ex)
            {
                model.ErrorMessage = $"ERROR DE COMUNICACIÓN CON SAP: {ex.Message}.";
            }
            catch (RfcLogonException ex)
            {
                model.ErrorMessage = $"ERROR DE LOGON EN SAP: {ex.Message}.";
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"ERROR INESPERADO: {ex.Message}";
            }

            return View(model);
        }

        // GET: SapPoc/MaterialDetails
        // Esta acción muestra la página por primera vez (el formulario vacío)
        public ActionResult MaterialDetails()
        {
            var model = new SapDetailsQueryViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MaterialDetails(SapDetailsQueryViewModel model)
        {
            model.QueryExecuted = true;

            if (string.IsNullOrWhiteSpace(model.MaterialNumber))
            {
                model.ErrorMessage = "Por favor, ingresa un número de material.";
                ViewBag.NotificationMessage = "Por favor, ingresa un número de material.";
                ViewBag.NotificationType = "warning"; // Usamos 'warning' para validaciones
                return View(model);
            }

            try
            {
                // Helper function to safely parse SAP dates
                Func<string, DateTime?> parseSapDate = (sapDateStr) => {
                    if (string.IsNullOrWhiteSpace(sapDateStr) || sapDateStr == "00000000")
                    {
                        return null;
                    }
                    if (DateTime.TryParseExact(sapDateStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        return result;
                    }
                    return null; // Return null if parsing fails
                };

                // 1. Conexión a SAP
                RfcConfigParameters connParams = new RfcConfigParameters();
                connParams.Add(RfcConfigParameters.Name, ConfigurationManager.AppSettings["SapSystemName"]);
                connParams.Add(RfcConfigParameters.AppServerHost, ConfigurationManager.AppSettings["SapServer"]);
                connParams.Add(RfcConfigParameters.SystemNumber, ConfigurationManager.AppSettings["SapSystemNumber"]);
                connParams.Add(RfcConfigParameters.User, ConfigurationManager.AppSettings["SapUser"]);
                connParams.Add(RfcConfigParameters.Password, ConfigurationManager.AppSettings["SapPassword"]);
                connParams.Add(RfcConfigParameters.Client, ConfigurationManager.AppSettings["SapClient"]);
                connParams.Add(RfcConfigParameters.Language, ConfigurationManager.AppSettings["SapLanguage"]);
                connParams.Add(RfcConfigParameters.SAPRouter, ConfigurationManager.AppSettings["SapRouter"]);

                RfcDestination destination = RfcDestinationManager.GetDestination(connParams);

                // 2. Obtener la nueva RFC
                IRfcFunction rfcFunction = destination.Repository.CreateFunction("Z_RFC_GET_MATERIAL_DETAILS");

                // 3. Preparar los filtros
                // Para el filtro de material S_MATNR
                IRfcTable matnrTable = rfcFunction.GetTable("S_MATNR");
                matnrTable.Append(); // Añadimos una nueva fila al filtro
                matnrTable.SetValue("SIGN", "I");

                // Lógica inteligente para usar comodines: si el texto contiene un '*',
                // usamos la opción 'CP' (Contains Pattern). Si no, usamos 'EQ' (Equal).
                if (model.MaterialNumber.Contains("*"))
                {
                    matnrTable.SetValue("OPTION", "CP");
                }
                else
                {
                    matnrTable.SetValue("OPTION", "EQ");
                }
                matnrTable.SetValue("LOW", model.MaterialNumber.ToUpper());

                // Bloque de código NUEVO y MEJORADO
                if (!string.IsNullOrWhiteSpace(model.Plant))
                {
                    // 1. Dividimos el string de entrada por las comas para obtener una lista de plantas.
                    //    Usamos Trim() para eliminar espacios en blanco accidentales (ej. "5190, 5390").
                    string[] plants = model.Plant.Split(',').Select(p => p.Trim()).ToArray();

                    // 2. Obtenemos la tabla de filtro de la RFC.
                    IRfcTable werksTable = rfcFunction.GetTable("S_WERKS");

                    // 3. Iteramos sobre cada planta que encontramos y añadimos una fila de filtro para cada una.
                    foreach (var singlePlant in plants)
                    {
                        if (!string.IsNullOrWhiteSpace(singlePlant))
                        {
                            werksTable.Append();
                            werksTable.SetValue("SIGN", "I");
                            werksTable.SetValue("OPTION", "EQ");
                            werksTable.SetValue("LOW", singlePlant.ToUpper());
                        }
                    }
                }

                // Si el checkbox 'GetBatchChars' está marcado, pasamos 'X', si no, ' '
                rfcFunction.SetValue("IV_GET_BATCH_CHARS", model.GetBatchChars ? "X" : " ");

                // 4. Invocar la función
                rfcFunction.Invoke(destination);

                // 5. LEER LA ESTRUCTURA DE RESULTADOS ANIDADA
                IRfcTable resultsTable = rfcFunction.GetTable("E_MATERIALS");

                if (resultsTable.RowCount == 0) // Check if the RFC returned *any* results
                {
                    if (!string.IsNullOrWhiteSpace(model.Plant)) // Check if the user entered plant filters
                    {
                        // Construct the message mentioning the specific plants
                        model.ErrorMessage = $"El material(es) '{model.MaterialNumber}' no fue encontrado en la(s) planta(s) especificadas: {model.Plant}.";
                        ViewBag.NotificationMessage = "Material(es) no encontrado en las plantas especificadas."; // Mensaje corto para Toastr
                        ViewBag.NotificationType = "warning";
                    }
                    else
                    {
                        // Use the original, general message if no plant filter was used
                        model.ErrorMessage = $"El material(es) '{model.MaterialNumber}' no fue encontrado en SAP.";
                        ViewBag.NotificationMessage = "Material(es) no encontrado.";
                        ViewBag.NotificationType = "warning";
                    }
                }
                else
                {
                    // Bucle principal: Recorre cada material
                    foreach (IRfcStructure materialRow in resultsTable)
                    {
                        var materialVM = new MaterialDetailsViewModel();

                        materialVM.Matnr = materialRow.GetString("MATNR");
                        materialVM.Mtart = materialRow.GetString("MTART");
                        materialVM.Bismt = materialRow.GetString("BISMT");
                        materialVM.Brgew = materialRow.GetDecimal("BRGEW");
                        materialVM.Ntgwe = materialRow.GetDecimal("NTGEW");
                        materialVM.Meins = materialRow.GetString("MEINS");
                        materialVM.Groes = materialRow.GetString("GROES");
                        materialVM.Zzreappl = materialRow.GetString("ZZREAPPL");
                        materialVM.Zzmtltyp = materialRow.GetString("ZZMTLTYP");
                        materialVM.Zzselltyp = materialRow.GetString("ZZSELLTYP");
                        materialVM.Zzbussmodl = materialRow.GetString("ZZBUSSMODL");
                        materialVM.Vmsta = materialRow.GetString("VMSTA");
                        materialVM.Gewei = materialRow.GetString("GEWEI");
                        materialVM.Zzmattype = materialRow.GetString("ZZMATTYP");
                        materialVM.Zzcustscrp = materialRow.GetString("ZZCUSTSCRP");
                        materialVM.Zzengscrp = materialRow.GetString("ZZENGSCRP");
                        materialVM.Zzihsnum1 = materialRow.GetString("ZZIHSNUM1");
                        materialVM.Zzihsnum2 = materialRow.GetString("ZZIHSNUM2");
                        materialVM.Zzihsnum3 = materialRow.GetString("ZZIHSNUM3");
                        materialVM.Zzihsnum4 = materialRow.GetString("ZZIHSNUM4");
                        materialVM.Zzihsnum5 = materialRow.GetString("ZZIHSNUM5");
                        materialVM.Zzpkgpcs = materialRow.GetString("ZZPKGPCS"); // NUMC
                        materialVM.Zzthcknss = materialRow.GetDecimal("ZZTHCKNSS"); // DEC
                        materialVM.Zzwidth = materialRow.GetDecimal("ZZWIDTH");     // DEC
                        materialVM.Zzadvance = materialRow.GetDecimal("ZZADVANCE"); // DEC
                        materialVM.Zzhtalscrp = materialRow.GetDecimal("ZZHTALSCRP");// DEC
                        materialVM.Zzcarpcs = materialRow.GetDecimal("ZZCARPCS");   // DEC
                        materialVM.Zzinitwt = materialRow.GetDecimal("ZZINITWT");   // DEC
                        materialVM.Zzminwt = materialRow.GetDecimal("ZZMINWT");     // DEC
                        materialVM.Zzmaxwt = materialRow.GetDecimal("ZZMAXWT");     // DEC
                        materialVM.Zzstkpcs = materialRow.GetString("ZZSTKPCS");    // CHAR
                        materialVM.Zzanglea = materialRow.GetDecimal("ZZANGLEA");   // DEC
                        materialVM.Zzangleb = materialRow.GetDecimal("ZZANGLEB");   // DEC
                        materialVM.Zzrealntwt = materialRow.GetDecimal("ZZREALNTWT");     // DEC
                        materialVM.Zzrealgrwt = materialRow.GetDecimal("ZZREALGRWT");     // DEC
                        materialVM.Zzdouopcs = materialRow.GetString("ZZDOUPCS");         // CHAR
                        materialVM.Zzcoilsltpos = materialRow.GetString("ZZCOILSLTPOS"); // CHAR
                        materialVM.Zzmxwttolp = materialRow.GetDecimal("ZZMXWTTOLP");     // DEC
                        materialVM.Zzmxwttoln = materialRow.GetDecimal("ZZMXWTTOLN");     // DEC
                        materialVM.Zzmnwttolp = materialRow.GetDecimal("ZZMNWTTOLP");     // DEC
                        materialVM.Zzmnwttoln = materialRow.GetDecimal("ZZMNWTTOLN");     // DEC
                        materialVM.Zzwh = materialRow.GetString("ZZWH");                 // CHAR
                        materialVM.Zztransp = materialRow.GetString("ZZTRANSP");         // CHAR
                        materialVM.Zztkmmsop = materialRow.GetString("ZZTKMMSOP");       // CHAR
                        materialVM.Zztkmmeop = materialRow.GetString("ZZTKMMEOP");       // CHAR
                        materialVM.Zzppackage = materialRow.GetString("ZZPPACKAGE");     // CHAR
                        materialVM.Zzspackage = materialRow.GetString("ZZSPACKAGE");     // CHAR
                        materialVM.Zzpallet = materialRow.GetString("ZZPALLET");         // CHAR
                        materialVM.Zzstamd = materialRow.GetString("ZZSTAMD");           // CHAR
                        materialVM.Zzidpnum = materialRow.GetString("ZZIDPNUM");         // CHAR
                        materialVM.Zzidtool = materialRow.GetString("ZZIDTOOL");         // CHAR
                        materialVM.Zzidobsol = materialRow.GetString("ZZIDOBSOL");       // CHAR
                        materialVM.Zztourd = materialRow.GetString("ZZTOURD");           // CHAR
                        materialVM.Zztolmaxwt = materialRow.GetString("ZZTOLMAXWT");     // DEC
                        materialVM.Zztolminwt = materialRow.GetString("ZZTOLMINWT");     // DEC

                        // Leer tabla anidada de Descripciones
                        IRfcTable descTable = materialRow.GetTable("DESC_DATA");
                        foreach (IRfcStructure descRow in descTable)
                        {
                            materialVM.Descriptions.Add(new DescriptionDataViewModel
                            {
                                Spras = descRow.GetString("SPRAS"),
                                Maktx = descRow.GetString("MAKTX")
                            });
                        }

                        // Leer tabla anidada de Plantas (y sus BOMs)
                        IRfcTable plantTable = materialRow.GetTable("PLANT_DATA");
                        foreach (IRfcStructure plantRow in plantTable)
                        {
                            var plantVM = new PlantDataViewModel
                            {
                                Werks = plantRow.GetString("WERKS"),
                                Mmsta = plantRow.GetString("MMSTA")
                            };

                            // --- Leer tabla anida de BOMs (dentro de la planta) ---
                            IRfcTable bomTable = plantRow.GetTable("BOM_DATA");
                            foreach (IRfcStructure bomRow in bomTable)
                            {
                               

                                plantVM.BomItems.Add(new BomItemViewModel
                                {
                                    Alt_Bom = bomRow.GetString("ALT_BOM"),
                                    Item_No = bomRow.GetString("ITEM_NO"),
                                    Component = bomRow.GetString("COMPONENT"),
                                    Comp_Desc = bomRow.GetString("COMP_DESC"),
                                    Quantity = bomRow.GetDecimal("QUANTITY"),
                                    Uom = bomRow.GetString("UOM"),
                                    // ++ LEER FECHAS COMO STRING Y PARSEAR ++
                                    Valid_From = parseSapDate(bomRow.GetString("VALID_FROM")),
                                    Created_On = parseSapDate(bomRow.GetString("CREATED_ON")),
                                });
                            }
                            materialVM.Plants.Add(plantVM);
                        }

                        // Leer tabla anidada de Características
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

                        // ++ NUEVO BLOQUE: Leer tabla anidada de Lotes (BATCH_DATA) ++
                        IRfcTable batchTable = materialRow.GetTable("BATCH_DATA");
                        foreach (IRfcStructure batchRow in batchTable)
                        {
                            var batchVM = new BatchDataViewModel
                            {
                                Charg = batchRow.GetString("CHARG"),
                                Werks = batchRow.GetString("WERKS"),
                                // Usamos el mismo helper de parseo de fechas que en el BOM
                                Ersda = parseSapDate(batchRow.GetString("ERSDA")),
                                Vfdat = parseSapDate(batchRow.GetString("VFDAT"))
                            };

                            // --- Leer tabla anidada de Características del Lote (BATCH_CHARS) ---
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
                            } // Fin del bucle de características de lote

                            materialVM.Batches.Add(batchVM);
                        } // Fin del bucle de Lotes

                        // --- INICIO: CALCULAR PESOS DESDE BOM (POR PLANTA - EXCLUYENDO -0.001) ---
                        foreach (var plantVM in materialVM.Plants)
                        {
                            // Seleccionar la alternativa prioritaria (ej: '01' o la menor)
                            string relevantAlternative = plantVM.BomItems
                                .OrderBy(b => b.Alt_Bom == "01" ? 0 : int.Parse(b.Alt_Bom)) // Prioriza '01', luego ordena numéricamente
                                .Select(b => b.Alt_Bom)
                                .FirstOrDefault(); // Toma la primera (la prioritaria)

                            if (relevantAlternative != null)
                            {
                                // Filtrar componentes de la alternativa seleccionada
                                var relevantComponents = plantVM.BomItems
                                    .Where(b => b.Alt_Bom == relevantAlternative)
                                    .ToList();

                                // Calcular Peso Bruto (Suma de cantidades positivas)
                                plantVM.CalculatedGrossWeight = relevantComponents
                                    .Where(c => c.Quantity > 0)
                                    .Sum(c => c.Quantity);

                                // Calcular Peso Neto (Suma de todas las cantidades EXCEPTO -0.001)
                                plantVM.CalculatedNetWeight = relevantComponents
                                    .Where(c => c.Quantity != -0.001M) // <-- ¡AQUÍ ESTÁ EL CAMBIO!
                                    .Sum(c => c.Quantity);
                            }
                            else
                            {
                                // Si no hay BOMs, los pesos calculados son 0
                                plantVM.CalculatedGrossWeight = 0;
                                plantVM.CalculatedNetWeight = 0;
                            }
                        }

                        model.Materials.Add(materialVM);
                    }

                    int materialCount = model.Materials.Count;
                    ViewBag.NotificationMessage = $"Datos de {materialCount} material(es) cargados correctamente.";
                    ViewBag.NotificationType = "success";
                }
            }
            catch (RfcCommunicationException ex)
            {
                model.ErrorMessage = $"ERROR DE COMUNICACIÓN CON SAP: {ex.Message}.";
                ViewBag.NotificationMessage = "Error de Comunicación con SAP.";
                ViewBag.NotificationType = "error";
            }
            catch (RfcLogonException ex)
            {
                model.ErrorMessage = $"ERROR DE LOGON EN SAP: {ex.Message}.";
                ViewBag.NotificationMessage = "Error de Logon en SAP.";
                ViewBag.NotificationType = "error";
            }
            catch (RfcAbapRuntimeException ex) // Catch ABAP errors
            {
                model.ErrorMessage = $"ERROR EN EJECUCIÓN ABAP: {ex.Message} (Key: {ex.Key})";
                ViewBag.NotificationMessage = "Error en Ejecución ABAP.";
                ViewBag.NotificationType = "error";
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"ERROR INESPERADO: {ex.GetType().Name} - {ex.Message}";
                ViewBag.NotificationMessage = "Error Inesperado.";
                ViewBag.NotificationType = "error";
            }

            return View(model);
        }
    }

    public class SapMaterialViewModel
    {
        [Display(Name = "Número de Material")]
        public string MaterialNumber { get; set; }

        // --- Propiedades para los resultados (ACTUALIZADAS) ---
        [Display(Name = "Descripción (EN)")]
        public string DescriptionEN { get; set; }

        [Display(Name = "Descripción (ES)")]
        public string DescriptionES { get; set; } // <-- NUEVO

        [Display(Name = "Número de material antiguo")]
        public string OldMaterialNumber { get; set; } // <-- NUEVO

        [Display(Name = "Peso Bruto")]
        public string GrossWeight { get; set; } // <-- NUEVO

        [Display(Name = "Peso Neto")]
        public string NetWeight { get; set; } // <-- NUEVO

        [Display(Name = "Dimensiones")]
        public string Dimensions { get; set; } // <-- NUEVO

        [Display(Name = "Tipo de Material")]
        public string MaterialType { get; set; }

        [Display(Name = "Unidad de Medida")]
        public string UnitOfMeasure { get; set; }

        // --- Propiedades de control ---
        public bool QueryExecuted { get; set; } = false;
        public string ErrorMessage { get; set; }
    }
}