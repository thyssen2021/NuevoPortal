using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Portal_2_0.Models.Auxiliares;

namespace Portal_2_0.Models
{
    public static class UtilMapeoMateriales
    {
        /// <summary>
        /// Intenta parsear fechas en formatos comunes de SAP (YYYY-MM o YYYYMMDD).
        /// </summary>
        private static DateTime? ParseSAPDate(string sapDate)
        {
            if (string.IsNullOrWhiteSpace(sapDate)) return null;

            // Formato 1: YYYY-MM (Usado en tu código anterior)
            if (DateTime.TryParseExact(sapDate, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            // Formato 2: YYYYMMDD (Común en SAP)
            if (DateTime.TryParseExact(sapDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            // Formato 3: Fallback genérico
            if (DateTime.TryParse(sapDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Obtiene los datos completos de un material desde SAP (Materials, Descriptions, Plants) 
        /// y los mapea a un objeto 'mm_v3' para compatibilidad con las vistas.
        /// </summary>
        /// <param name="materialId">El ID del material (Matnr)</param>
        /// <param name="plantId">El ID de la planta (Werks). Opcional, pero necesario para Plnt y MS.</param>
        /// <returns>Un objeto mm_v3 poblado con datos de SAP.</returns>
        public static mm_v3 GetSAPMaterialData(string materialId, string plantId = null)
        {
            if (string.IsNullOrEmpty(materialId))
            {
                // Retorna un objeto vacío (pero no nulo) si no hay ID
                return new mm_v3 { activo = false };
            }

            // Usamos un nuevo contexto de SAP encapsulado
            using (var db_sap = new Portal_2_0_ServicesEntities())
            {
                var mm_sap = db_sap.Materials.FirstOrDefault(x => x.Matnr == materialId);

                if (mm_sap == null)
                {
                    // Si el material no existe en SAP, retorna un objeto vacío con el ID
                    return new mm_v3 { Material = materialId, activo = false };
                }

                // 1. Obtener Datos Relacionados (Descripciones y Planta)
                var mm_desc_es = db_sap.MaterialDescriptions.FirstOrDefault(x => x.Matnr == materialId && x.Spras == "S");
                var mm_desc_en = db_sap.MaterialDescriptions.FirstOrDefault(x => x.Matnr == materialId && x.Spras == "E");

                MaterialPlants mm_plant_sap = null;
                if (!string.IsNullOrEmpty(plantId))
                {
                    mm_plant_sap = db_sap.MaterialPlants.FirstOrDefault(x => x.Matnr == materialId && x.Werks == plantId);
                }
                // Fallback si la planta específica no se encontró o no se proveyó
                mm_plant_sap = mm_plant_sap ?? db_sap.MaterialPlants.FirstOrDefault(x => x.Matnr == materialId);

                // 2. Mapear SAP.Materials -> mm_v3
                var mm_v3_obj = new mm_v3
                {
                    // Mapeos Directos
                    Material = mm_sap.Matnr,
                    Plnt = mm_plant_sap?.Werks,
                    MS = mm_plant_sap?.Mmsta,
                    Material_Description = mm_desc_en?.Maktx,
                    material_descripcion_es = mm_desc_es?.Maktx,
                    Type_of_Material = mm_sap.ZZMATTYP?.Trim() ?? mm_sap.Mtart?.Trim(), // Prioriza el tipo custom
                    Type_of_Metal = mm_sap.ZZMTLTYP,
                    Old_material_no_ = mm_sap.Bismt,
                    Business_Model = mm_sap.ZZBUSSMODL,
                    IHS_number_1 = mm_sap.ZZIHSNUM1,
                    IHS_number_2 = mm_sap.ZZIHSNUM2,
                    IHS_number_3 = mm_sap.ZZIHSNUM3,
                    IHS_number_4 = mm_sap.ZZIHSNUM4,
                    IHS_number_5 = mm_sap.ZZIHSNUM5,
                    Type_of_Selling = mm_sap.ZZSELLTYP,

                    // Pesos
                    Gross_weight = mm_sap.Brgew,
                    Un_ = mm_sap.Gewei, // Unidad de Peso Bruto/Neto
                    Net_weight = mm_sap.Ntgwe,
                    Un_1 = mm_sap.Meins, // Unidad Base

                    // Dimensiones (Campos ZZ)
                    Thickness = mm_sap.ZZTHCKNSS,
                    Width = mm_sap.ZZWIDTH,
                    Advance = mm_sap.ZZADVANCE,
                    size_dimensions = mm_sap.Groes, // Dimensiones (ej. 1.5X1500X3000)

                    // Pesos (Campos ZZ)
                    Head_and_Tail_allowed_scrap = mm_sap.ZZHTALSCRP,
                    Pieces_per_car = mm_sap.ZZCARPCS,
                    Initial_Weight = mm_sap.ZZINITWT,
                    Min_Weight = mm_sap.ZZMINWT,
                    Maximum_Weight = mm_sap.ZZMAXWT,

                    activo = true, // Asumimos activo si existe en SAP

                    // Cantidades
                    Package_Pieces = mm_sap.ZZPKGPCS?.ToString(), // string en mm_v3
                    Pieces_Pac = mm_sap.ZZPPACKAGE, // float? en mm_v3
                    Stacks_Pac = mm_sap.ZZSPACKAGE, // float? en mm_v3
                    num_piezas_golpe = (int?)mm_sap.ZZSTKPCS, // int? en mm_v3

                    unidad_medida = mm_sap.Meins,

                    // Ángulos y Pesos Reales
                    angle_a = mm_sap.ZZANGLEA,
                    angle_b = mm_sap.ZZANGLEB,
                    real_net_weight = mm_sap.ZZREALNTWT,
                    real_gross_weight = mm_sap.ZZREALGRWT,

                    coil_position = mm_sap.ZZCOILSLTPOS,

                    // Tolerancias de Peso
                    maximum_weight_tol_positive = mm_sap.ZZMXWTTOLP,
                    maximum_weight_tol_negative = mm_sap.ZZMXWTTOLN,
                    minimum_weight_tol_positive = mm_sap.ZZMNWTTOLP,
                    minimum_weight_tol_negative = mm_sap.ZZMNWTTOLN,

                    Tipo_de_Transporte = mm_sap.ZZTRANSP,
                    Type_of_Pallet = mm_sap.ZZPALLET,

                    // --- Mapeos con Conversión ---

                    // Fechas (String a DateTime?)
                    Tkmm_SOP = ParseSAPDate(mm_sap.ZZTKMMSOP),
                    Tkmm_EOP = ParseSAPDate(mm_sap.ZZTKMMEOP),

                    // Banderas (bool? a string "true"/"false")
                    Head_and_Tails_Scrap_Conciliation = (mm_sap.ZZCUSTSCRP.HasValue && mm_sap.ZZCUSTSCRP.Value) ? "true" : "false",
                    Engineering_Scrap_conciliation = (mm_sap.ZZENGSCRP.HasValue && mm_sap.ZZENGSCRP.Value) ? "true" : "false",
                    Re_application = (mm_sap.ZZREAPPL.HasValue && mm_sap.ZZREAPPL.Value) ? "true" : "false",

                    // Banderas (bool? a string "SÍ"/"NO")
                    double_pieces = (mm_sap.ZZDOUPCS.HasValue && mm_sap.ZZDOUPCS.Value) ? "SÍ" : "NO",

                    // Banderas (bool? a bool?)
                    Almacen_Norte = mm_sap.ZZWH
                };

                return mm_v3_obj;
            }
        }

        /// <summary>
        /// Obtiene las características de SAP y las mapea a un objeto 'class_v3'.
        /// </summary>
        /// <param name="materialId">El ID del material (Matnr)</param>
        /// <returns>Un objeto class_v3 poblado con características de SAP.</returns>
        public static class_v3 GetSAPClassData(string materialId)
        {
            // 1. Inicializa el objeto (compatible con la vista)
            var class_v3_obj = new class_v3
            {
                Object = materialId,
                activo = true // Siempre dejar en true según requerimiento
            };

            if (string.IsNullOrEmpty(materialId))
            {
                return class_v3_obj;
            }

            // 2. Obtiene los datos de SAP
            using (var db_sap = new Portal_2_0_ServicesEntities())
            {
                var characteristics = db_sap.MaterialCharacteristics
                    .Where(x => x.Matnr == materialId)
                    .ToList()
                    .ToDictionary(x => x.Charact, x => x, StringComparer.OrdinalIgnoreCase);

                // --- Funciones auxiliares locales (Getters) ---
                Func<string, string> GetCharInternal = (charactKey) =>
                {
                    if (characteristics.TryGetValue(charactKey, out var entry))
                    {
                        // Devuelve el valor interno (el código o valor numérico)
                        return entry.Value_Internal?.Trim() ?? string.Empty;
                    }
                    return string.Empty;
                };

                Func<string, string> GetCharDescES = (charactKey) =>
                {
                    if (characteristics.TryGetValue(charactKey, out var entry))
                    {
                        // Devuelve la descripción en español
                        return entry.Value_Desc_Es?.Trim() ?? string.Empty;
                    }
                    return string.Empty;
                };

                // --- 3. Mapeo de Características a class_v3 ---

                // Usamos Descripciones (Value_Desc_Es) para campos descriptivos
                class_v3_obj.Grade = GetCharDescES(SAPCharacteristics.GRADE.ToString());
                class_v3_obj.Shape = GetCharDescES(SAPCharacteristics.SHAPE.ToString());
                class_v3_obj.Surface = GetCharDescES(SAPCharacteristics.SURFACE.ToString());
                class_v3_obj.Mill = GetCharDescES(SAPCharacteristics.MILL.ToString());
                class_v3_obj.surface_treatment = GetCharDescES(SAPCharacteristics.SURFACE_TREATMENT.ToString());
                class_v3_obj.coating_weight = GetCharDescES(SAPCharacteristics.COATING_WEIGHT.ToString());

                // Usamos Valores Internos (Value_Internal) para IDs, Códigos y Rangos Numéricos
                class_v3_obj.Customer = GetCharInternal(SAPCharacteristics.CUSTOMER_NUMBER.ToString());
                class_v3_obj.Customer_part_number = GetCharInternal(SAPCharacteristics.CUSTOMER_PART_NUMBER.ToString());
                class_v3_obj.customer_part_msa = GetCharInternal(SAPCharacteristics.CUSTOMER_PART2.ToString());
                class_v3_obj.commodity = GetCharInternal(SAPCharacteristics.COMMODITY.ToString()); // El código (ej. "GN")

                // Rangos/Valores
                class_v3_obj.Gauge___Metric = GetCharInternal(SAPCharacteristics.GAUGE_M.ToString()); // ej. "0.6400 - 0.7600"
                class_v3_obj.Width___Metr = GetCharInternal(SAPCharacteristics.WIDTH_M.ToString());
                class_v3_obj.Length_mm_ = GetCharInternal(SAPCharacteristics.LENGTH_M.ToString());
                class_v3_obj.flatness_metric = GetCharInternal(SAPCharacteristics.FLATNESS_M.ToString());
                class_v3_obj.outer_diameter_coil = GetCharInternal(SAPCharacteristics.OD_COIL_M.ToString());
                class_v3_obj.inner_diameter_coil = GetCharInternal(SAPCharacteristics.ID_COIL_M.ToString());

                return class_v3_obj;
            }
        }
    }
}