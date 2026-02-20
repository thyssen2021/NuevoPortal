import type { Material, AppLists, FiscalYearDef, IhsProductionItem, ChartDataPoints } from '../types';
import type { CapacityValidationResult, LineCapacityGroup } from '../types';
// 1. Paleta de Colores
const STATUS_COLORS: Record<string, string> = {
    "Spare Parts": "#94c149", // Gris (ID 9)
    "POH": "#0098f7",         // Verde (ID 4)
    "Casi Casi": "#ffd400",   // Amarillo (ID 3)
    "Carry Over": "#aaaaaa",  // Cyan (ID 2)
    "Quotes": "#01c401",      // Azul (ID 1)
    //"New": "#007bff",         // Azul (Fallback)
    //"Concept": "#6f42c1",     // Morado
    //"Existing Load": "#e9ecef" // Gris Claro (Fondo)
};

// 2. Mapa Estático de Nombres (Respaldo robusto para IDs)
const STATIC_STATUS_NAMES: Record<number, string> = {
    1: "Quotes",
    2: "Carry Over",
    3: "Casi Casi",
    4: "POH",
    9: "Spare Parts"
};

// IDs de Rutas que son de Blanking (Asegúrate de que coincidan con tus constantes ROUTES)
// Usualmente: 1=BLK, 2=BLK_RP, 4=BLK_RPLTZ, 5=BLK_SH, 9=SLT_BLK, 10=SLT_BLK_WLD
const BLANKING_ROUTE_IDS = [1, 2, 4, 5, 9, 10, 13];

// 3. Orden Lógico (De abajo hacia arriba en Gráfica / De Izq a Der en Tabla)
const STATUS_ORDER = ["Spare Parts", "POH", "Casi Casi", "Carry Over", "Quotes"];


/**
 * Valida lo básico (si faltan campos)
 */
export const validateMaterialCapacity = (
    mat: Partial<Material>
): CapacityValidationResult => {

    const missing: string[] = [];

    // 1. Validar Línea
    const realLine = Number(mat.ID_Real_Blanking_Line);
    const theoLine = Number(mat.ID_Theoretical_Blanking_Line);
    const lineId = realLine > 0 ? realLine : (theoLine > 0 ? theoLine : 0);
    if (lineId === 0) missing.push("Line");

    // 2. Validar Volumen
    const routeId = Number(mat.ID_Route || 0);
    // Ajusta estos IDs a tus constantes reales de Blanking
    const isBlanking = [1, 2, 4, 5, 9, 10, 13].includes(routeId);
    let vol = Number(mat.Annual_Volume || 0);
    if (isBlanking && Number(mat.Blanking_Annual_Volume) > 0) {
        vol = Number(mat.Blanking_Annual_Volume);
    }
    if (vol <= 0) missing.push("Volume");

    // 3. Validar Ciclo
    const ict = Number(mat.Ideal_Cycle_Time_Per_Tool || mat.Theoretical_Strokes || mat.Real_Strokes || 0);
    if (ict <= 0) missing.push("Cycle");

    // 4. Validar Fechas (Crucial para el desglose por año)
    if (!mat.Real_SOP) missing.push("SOP");
    if (!mat.Real_EOP) missing.push("EOP");

    return {
        isValid: missing.length === 0,
        missingFields: missing,
        // loadPercentage y loadMinutes eliminados, se calculan dinámicamente
    };
};

/**
 * REPLICA EXACTA DE: GetProductionByFiscalYearID (Legacy)
 * Obtiene la producción total por año fiscal para el material seleccionado.
 * (Versión limpia: Sin logs, solo cálculo)
 */
export const getProductionByFiscalYearID = (
    formData: Partial<Material>,
    lists: AppLists
): Record<number, number> => {
    const productionByFYId: Record<number, number> = {};

    // 1. Validar Vehicle
    if (!formData.Vehicle) {
        // console.warn("[ERROR] Vehicle es nulo o vacío."); // Opcional: comentar para menos ruido
        return productionByFYId;
    }

    // 2. Obtener la Metadata del Vehículo
    let vehicleData: any = null;

    if (lists.vehicleMasterData) {
        const allCountryLists = Object.values(lists.vehicleMasterData) as any[][];
        allCountryLists.some((vehicles: any[]) => {
            const found = vehicles.find(v => v.Value === formData.Vehicle);
            if (found) {
                vehicleData = found;
                return true;
            }
            return false;
        });
    }

    if (!vehicleData || !vehicleData.ProductionDataJson) {
        // console.warn(`[WARNING] No se encontró data IHS...`);
        return productionByFYId;
    }

    // Parsear la producción
    const productions: IhsProductionItem[] = JSON.parse(vehicleData.ProductionDataJson);

    // 3. Determinar Fechas de Búsqueda (SOP/EOP)
    const parseDate = (dateStr?: string, isEnd: boolean = false): Date | null => {
        if (!dateStr) return null;
        const [y, m] = dateStr.split('-').map(Number);
        if (isEnd) return new Date(y, m, 0);
        return new Date(y, m - 1, 1);
    };

    const formSOP = parseDate(formData.Real_SOP);
    const formEOP = parseDate(formData.Real_EOP, true);
    const ihsSOP = parseDate(vehicleData.SOP);
    const ihsEOP = parseDate(vehicleData.EOP, true);

    const startSearchDate = formSOP || ihsSOP || new Date(1900, 0, 1);
    const endSearchDate = formEOP || ihsEOP || new Date(2100, 11, 31);

    // 4. Obtener Años Fiscales Traslapados
    if (!lists.fiscalYears) return productionByFYId;

    const overlappingFiscalYears: FiscalYearDef[] = lists.fiscalYears.filter(fy => {
        const fyStart = new Date(fy.Start);
        const fyEnd = new Date(fy.End);
        return !(fyEnd < startSearchDate || fyStart > endSearchDate);
    });

    if (overlappingFiscalYears.length === 0) {
        return productionByFYId;
    }

    // 5. Filtrar y Sumarizar
    productions.forEach(prod => {
        const safeMonth = prod.Production_Month || 1;
        const prodDate = new Date(prod.Production_Year, safeMonth - 1, 1);

        const fiscalRow = overlappingFiscalYears.find(fy => {
            const fyStart = new Date(fy.Start);
            const fyEnd = new Date(fy.End);
            return prodDate >= fyStart && prodDate <= fyEnd;
        });

        if (fiscalRow) {
            const fyId = fiscalRow.ID;
            if (!productionByFYId[fyId]) {
                productionByFYId[fyId] = 0;
            }
            productionByFYId[fyId] += prod.Production_Amount;
        }
    });

    // ❌ SE ELIMINÓ EL BLOQUE DE CONSOLE.LOG AQUÍ PARA EVITAR DUPLICADOS

    return productionByFYId;
};

/**
 * Calcula la carga en MINUTOS para el material actual del formulario
 * (Incluye Lógica de Factor de Ajuste y Tabla de Depuración Completa)
 */
export const calculateMaterialLoad = (
    formData: Partial<Material>,
    lists: AppLists,
    fiscalYears: FiscalYearDef[]
): Record<number, number> => {

    // 🛑 1. GATEKEEPER PRINCIPAL: Validación Estricta
    const validation = validateMaterialCapacity(formData);
    if (!validation.isValid) {
        return {};
    }

    // 2. Obtener Inputs y SANITIZARLOS
    const partNumber = formData.Part_Number || "N/A";
    const routeId = Number(formData.ID_Route || 0);
    let annualVol = Number(formData.Annual_Volume || 0);

    // Lógica de prioridad de volumen (Blanking vs General)
    if (BLANKING_ROUTE_IDS.includes(routeId)) {
        const blkVol = Number(formData.Blanking_Annual_Volume || 0);
        if (blkVol > 0) {
            annualVol = blkVol;
        }
    }

    // 🛑 3. SEGUNDO GATEKEEPER (Redundancia de Seguridad)
    // Aunque el validador diga que es válido, si el volumen final que vamos a usar es 0, abortamos.
    // Esto previene el dibujo de líneas planas si la lógica de validación difiere sutilmente.
    if (annualVol <= 0) return {};

    const partsPerVeh = Number(formData.Parts_Per_Vehicle || 0); // Ojo: Si es 0, no calcula
    if (partsPerVeh <= 0) return {}; // Abortar si no hay piezas por vehículo

    // Prioridad de Ciclo: ICT > Theo > Real > 0
    // IMPORTANTE: Quitamos el "|| 1" del final para que sea estricto
    const ict = Number(formData.Ideal_Cycle_Time_Per_Tool || formData.Theoretical_Strokes || formData.Real_Strokes || 0);

    if (ict <= 0) return {}; // Abortar si no hay ciclo

    const blanksPerStroke = Number(formData.Blanks_Per_Stroke || 0);
    if (blanksPerStroke <= 0) return {}; // Abortar si no hay blanks/golpe

    const lineId = Number(formData.ID_Real_Blanking_Line || formData.ID_Theoretical_Blanking_Line || 0);
    if (lineId === 0) return {};

    // ---------------------------------------------------------
    // Lógica de Factor de Ajuste
    // ---------------------------------------------------------
    let factorPercent = Number(formData.Max_Production_Factor);
    if (!formData.Max_Production_Factor || factorPercent <= 0) {
        factorPercent = 100;
    }
    const multiplier = factorPercent / 100;

    // 2. Determinar OEE
    let oee = Number(formData.OEE);
    if (!oee || oee === 0) {
        const histOee = lists.oeeHistory ? lists.oeeHistory[lineId.toString()] : null;
        oee = histOee || 0.85;
    }
    if (oee > 1) oee = oee / 100;

    // 3. Calcular Producción
    const productionByFY = getProductionByFiscalYearID(formData, lists);

    const resultByFY: Record<number, number> = {};
    const denominator = ict * blanksPerStroke * oee;

    if (denominator === 0) return {};

    // ---------------------------------------------------------
    // PREPARACIÓN DE LOGS DE DEPURACIÓN
    // ---------------------------------------------------------
    const debugTable: any[] = [];

    // Función helper para llenar la fila del log
    const addRow = (fyName: string, originalVol: number, adjustedVol: number, minutes: number) => {
        debugTable.push({
            "Part #": partNumber,
            "FY": fyName,
            "Orig Vol": Math.round(originalVol),
            "Factor": `${factorPercent}%`,
            "Adj Vol": Math.round(adjustedVol),
            // Variables de la fórmula:
            "Parts": partsPerVeh,
            "Cycle": ict,
            "Blanks": blanksPerStroke,
            "OEE": oee.toFixed(2),
            // Resultado
            "Minutes": Number(minutes.toFixed(2))
        });
    };

    const hasDetailedProd = Object.keys(productionByFY).length > 0;

    if (hasDetailedProd) {
        // CASO A: S&P
        Object.entries(productionByFY).forEach(([fyId, amount]) => {
            const adjustedAmount = amount * multiplier;
            const totalMinutes = (adjustedAmount * partsPerVeh) / denominator;

            resultByFY[Number(fyId)] = totalMinutes;

            const fyName = fiscalYears.find(f => f.ID === Number(fyId))?.Name || `FY ${fyId}`;
            addRow(fyName, amount, adjustedAmount, totalMinutes);
        });
    } else {
        // CASO B: Fallback (Annual Volume)
        const relevantFYs = fiscalYears.filter(fy => {
            if (!formData.Real_SOP || !formData.Real_EOP) return false;
            // Manejo seguro de fechas cortas (YYYY-MM)
            const sStr = formData.Real_SOP.length === 7 ? `${formData.Real_SOP}-01` : formData.Real_SOP;
            const eStr = formData.Real_EOP.length === 7 ? `${formData.Real_EOP}-01` : formData.Real_EOP;

            const s = new Date(sStr);
            const e = new Date(eStr);
            // Ajuste a fin de mes para EOP
            e.setMonth(e.getMonth() + 1);
            e.setDate(0);

            const fs = new Date(fy.Start);
            const fe = new Date(fy.End);
            return !(fe < s || fs > e);
        });

        relevantFYs.forEach(fy => {
            const adjustedAnnualVol = annualVol * multiplier;
            const totalMinutes = (adjustedAnnualVol * partsPerVeh) / denominator;

            resultByFY[fy.ID] = totalMinutes;

            addRow(fy.Name, annualVol, adjustedAnnualVol, totalMinutes);
        });
    }

    // IMPRESIÓN EN CONSOLA (Detallada)
    if (debugTable.length > 0) {
        console.groupCollapsed(`📊 CÁLCULO DE CAPACIDAD: ${partNumber}`);
        console.log("1. CONSTANTES GLOBALES:");
        console.log({
            "Line ID": lineId,
            "Cycle Time": ict,
            "Denominator": denominator.toFixed(4)
        });
        console.table(debugTable);
        console.groupEnd();
    }

    return resultByFY;
};

/**
 * REPLICA EXACTA:
 * - Tabla 1: Desglose de Fórmulas (Excel Style) por Material.
 * - Tabla 2: Resumen final consolidado por Línea.
 */
export const calculateTotalProjectLoad = (
    materials: Partial<Material>[],
    lists: AppLists
): Record<number, Record<number, number>> => {

    const result: Record<number, Record<number, number>> = {};
    const sumRealMinByLine: Record<number, number> = {};
    const summaryLog: any[] = [];

    console.groupCollapsed(`🏭 PROCESO DE CÁLCULO TOTAL (${materials.length} Materiales)`);

    // ========================================================================
    // PASO 1: PROCESO DE ACUMULACIÓN Y CÁLCULO DE REAL MIN
    // ========================================================================
    materials.forEach((material, index) => {

        // 🟢 NUEVO: Si este material está incompleto, lo saltamos y no ensuciamos la gráfica
        if (!validateMaterialCapacity(material).isValid) {
            return; // Continue al siguiente material
        }

        // --- A. Selección de Línea ---
        const realLine = Number(material.ID_Real_Blanking_Line);
        const theoLine = Number(material.ID_Theoretical_Blanking_Line);
        let lineId = realLine > 0 ? realLine : (theoLine > 0 ? theoLine : 0);

        if (lineId === 0) return;

        // Nombre de línea
        const lineObj = lists.linesList?.find((l: any) => Number(l.Value) === lineId);
        const lineName = lineObj ? lineObj.Text : `Line ${lineId}`;
        const partNumber = material.Part_Number || `Mat #${index + 1}`;

        // Selección de Volumen según Ruta
        const routeId = Number(material.ID_Route || 0);
        let annualVol = Number(material.Annual_Volume || 0);

        if (BLANKING_ROUTE_IDS.includes(routeId)) {
            const blkVol = Number(material.Blanking_Annual_Volume);
            if (blkVol > 0) {
                annualVol = blkVol;
            }
        }

        const partsPerVeh = Number(material.Parts_Per_Vehicle || 1);
        const ict = Number(material.Ideal_Cycle_Time_Per_Tool || material.Theoretical_Strokes || material.Real_Strokes || 1); // Strokes per Minute
        const blanksPerStroke = Number(material.Blanks_Per_Stroke || 1);

        let oee = Number(material.OEE);
        if (!oee || oee === 0) {
            const histOee = lists.oeeHistory ? lists.oeeHistory[lineId.toString()] : null;
            oee = histOee || 0.85;
        }
        if (oee > 1) oee = oee / 100;

        // Factor de Ajuste
        let factor = Number(material.Max_Production_Factor);
        if (!factor || factor <= 0) factor = 100;
        const multiplier = factor / 100;

        // Volumen Ajustado (El que usa la fórmula)
        const adjustedVol = annualVol * multiplier;

        // --- C. CÁLCULOS INTERMEDIOS (Excel Logic) ---
        // Nota: Estas variables replican las filas de tu imagen

        // 1. Parts per Auto
        const val_PartsPerAuto = partsPerVeh;

        // 2. Strokes per Auto
        const val_StrokesPerAuto = val_PartsPerAuto / blanksPerStroke;

        // 3. BLANKS PER YEAR (Ajustado con Factor)
        const val_BlanksPerYear = val_PartsPerAuto * adjustedVol;

        // 4. Min. Max. Reales (Ideal Minutes / Sin OEE)
        // Fórmula: BlanksPerYear / ICT / BlanksPerStroke
        // (ICT está en Strokes/Min. Blanks/Min = ICT * BlanksPerStroke)
        const val_MinMaxReales = (ict * blanksPerStroke) > 0
            ? val_BlanksPerYear / (ict * blanksPerStroke)
            : 0;

        // 5. Min. Reales (Con OEE) -> ESTE ES EL 'RealMin'
        const val_MinReales = oee > 0 ? val_MinMaxReales / oee : 0;

        // 6. Turno Reales (Turnos de 7.5 hrs)
        const val_TurnoReales = val_MinReales / 7.5 / 60;

        // 7. Strokes per Turno
        // Fórmula: (BlanksPerYear / BlanksPerStroke) / TurnoReales
        const totalStrokesYear = val_BlanksPerYear / blanksPerStroke;
        const val_StrokesPerTurno = val_TurnoReales > 0 ? totalStrokesYear / val_TurnoReales : 0;


        // --- D. LOGGING: TABLA DE FÓRMULAS (Por Material) ---
        // Solo mostramos si hay volumen para no ensuciar
        if (adjustedVol > 0) {
            console.group(`📐 Fórmulas: ${partNumber} (${lineName})`);
            console.log(`Config: Factor ${factor}% | OEE ${(oee * 100).toFixed(0)}% | ICT ${ict}`);

            const excelTable = [
                {
                    "COLUMNA": "Parts per Auto",
                    "FORMULA": "=PARTS PER VEHICLE",
                    "VALOR": val_PartsPerAuto
                },
                {
                    "COLUMNA": "Strokes per Auto",
                    "FORMULA": "=Parts_per_Auto / BLANKS PER STROKE",
                    "VALOR": Number(val_StrokesPerAuto.toFixed(4))
                },
                {
                    "COLUMNA": "BLANKS PER YEAR [1/1000]",
                    "FORMULA": "=Parts_per_Auto * ANNUAL_VOLUME (Ajustado)",
                    "VALOR": Math.round(val_BlanksPerYear)
                },
                {
                    "COLUMNA": "Min. Max. Reales",
                    "FORMULA": "=BLANKS PER YEAR / ICT / BLANKS PER STROKE",
                    "VALOR": Number(val_MinMaxReales.toFixed(2))
                },
                {
                    "COLUMNA": "Min. Reales",
                    "FORMULA": "=Min. Max. Reales / OEE",
                    "VALOR": Number(val_MinReales.toFixed(2))
                },
                {
                    "COLUMNA": "Turno Reales",
                    "FORMULA": "=Min. Reales / 7.5 / 60",
                    "VALOR": Number(val_TurnoReales.toFixed(2))
                },
                {
                    "COLUMNA": "Strokes per Turno",
                    "FORMULA": "=Total Strokes / Turno Reales",
                    "VALOR": Math.round(val_StrokesPerTurno)
                }
            ];
            console.table(excelTable);
            console.groupEnd();
        }

        // --- E. ACUMULACIÓN PARA RESULTADOS FINALES ---

        // Acumular RealMin (Min. Reales) para la lógica de Picos
        if (!sumRealMinByLine[lineId]) sumRealMinByLine[lineId] = 0;
        sumRealMinByLine[lineId] += val_MinReales;

        // Distribución por FY (Esto sigue usando la función mensual detallada)
        const capacityByFY = calculateMaterialLoad(material, lists, lists.fiscalYears || []);

        Object.entries(capacityByFY).forEach(([fyIdStr, minutes]) => {
            const fyId = Number(fyIdStr);
            if (!result[lineId]) result[lineId] = {};
            if (!result[lineId][fyId]) result[lineId][fyId] = 0;
            result[lineId][fyId] += minutes;
        });
    });

    // 📸 SNAPSHOT ORIGINAL
    const originalResult = JSON.parse(JSON.stringify(result));

    // ========================================================================
    // PASO 2: REEMPLAZO DE PICOS
    // ========================================================================
    console.log(">> Paso 2: Análisis de Picos (Comparando Max S&P vs Suma Min. Reales)");

    Object.keys(result).forEach(lineIdStr => {
        const lineId = Number(lineIdStr);
        const fyData = result[lineId];
        const sumRealMin = sumRealMinByLine[lineId] || 0;

        let maxFyId = -1;
        let maxVal = -1;

        // Buscar pico
        Object.entries(fyData).forEach(([fy, val]) => {
            if (val > maxVal) {
                maxVal = val;
                maxFyId = Number(fy);
            }
        });

        if (maxFyId !== -1) {
            // Nombre para log
            const fyObj = lists.fiscalYears?.find(f => f.ID === maxFyId);
            const fyName = fyObj ? fyObj.Name : `FY ${maxFyId}`;
            const lineObj = lists.linesList?.find((l: any) => Number(l.Value) === lineId);
            const lineName = lineObj ? lineObj.Text : `Line ${lineId}`;

            if (Math.abs(maxVal - sumRealMin) > 1) {
                console.log(`   [${lineName}] Pico en ${fyName}. S&P: ${Math.round(maxVal)} vs Min.Reales: ${Math.round(sumRealMin)} -> REEMPLAZANDO.`);
                result[lineId][maxFyId] = sumRealMin;
            }
        }
    });

    // ========================================================================
    // PASO 3: TABLA FINAL Y CONVERSIÓN A HORAS
    // ========================================================================
    if (Object.keys(result).length > 0) {
        Object.entries(result).forEach(([lineIdStr, fyData]) => {
            const lineId = Number(lineIdStr);
            const lineObj = lists.linesList?.find((l: any) => Number(l.Value) === lineId);
            const lineName = lineObj ? lineObj.Text : `Line ${lineId}`;

            const originalFyData = originalResult[lineId] || {};
            const sortedFYs = Object.entries(fyData).sort((a, b) => Number(a[0]) - Number(b[0]));

            sortedFYs.forEach(([fyIdStr, finalMinutes]) => {
                const fyId = Number(fyIdStr);
                const fyObj = lists.fiscalYears?.find(f => f.ID === fyId);
                const fyName = fyObj ? fyObj.Name : `FY ${fyId}`;

                // Original S&P
                const originalMinutes = originalFyData[fyId] || 0;
                const originalHours = originalMinutes / 60.0;

                // Final Ajustado
                const finalHours = finalMinutes / 60.0;

                summaryLog.push({
                    "Línea": lineName,
                    "FY": fyName,
                    "Carga Original (S&P)": `${Math.round(originalMinutes)} min (${originalHours.toFixed(1)} hrs)`,
                    "Carga Final (Ajustada)": `${Math.round(finalMinutes)} min (${finalHours.toFixed(1)} hrs)`
                });
            });
        });

        console.log(">> Tabla 2: Resumen Final Consolidado");
        console.table(summaryLog);
    } else {
        console.log(" (Sin resultados acumulados)");
    }

    // CONVERSIÓN FINAL DEL OBJETO A HORAS (Para retorno)
    Object.keys(result).forEach(lineIdStr => {
        const lineId = Number(lineIdStr);
        Object.keys(result[lineId]).forEach(fyKey => {
            const fyId = Number(fyKey);
            result[lineId][fyId] = result[lineId][fyId] / 60;
        });
    });

    console.groupEnd();
    return result;
};

export const generateChartData = (
    newLoadHours: Record<number, Record<number, number>>,
    formData: Partial<Material>,
    lists: AppLists,
    projectPlantId: number,
    projectStatusName: string,
    currentProjectStatusId: number
): ChartDataPoints | null => {

    console.groupCollapsed("📊 PASO 4: Generación de Datos para Gráfica y Auditoría");

    // -------------------------------------------------------------------------
    // 1. DETERMINAR LÍNEA ACTIVA Y CONTEXTO
    // -------------------------------------------------------------------------
    const realLine = Number(formData.ID_Real_Blanking_Line);
    const theoLine = Number(formData.ID_Theoretical_Blanking_Line);
    // Prioridad: Real > Teórica
    let activeLineId = realLine > 0 ? realLine : (theoLine > 0 ? theoLine : 0);

    if (activeLineId === 0) {
        console.warn("⚠️ No se ha seleccionado una línea válida.");
        console.groupEnd();
        return null;
    }

    const lineObj = lists.linesList?.find((l: any) => Number(l.Value) === activeLineId);
    const activeLineName = lineObj ? lineObj.Text : `Line ${activeLineId}`;

    const slitterLineId = Number(formData.ID_Slitting_Line);
    // Si la línea activa coincide con el Slitter seleccionado, usamos turnos
    const isSlitterContext = slitterLineId > 0 && activeLineId === slitterLineId;

    // -------------------------------------------------------------------------
    // 2. OBTENER DATOS DE REFERENCIA
    // -------------------------------------------------------------------------
    const plantCapacity = lists.capacityData ? lists.capacityData[projectPlantId.toString()] : [];
    const currentLoadRaw = lists.currentLoad ? lists.currentLoad[activeLineId.toString()] : [];

    // Helper para obtener nombres (Estático > Dinámico > Fallback)
    const getStatusName = (id: number): string => {
        if (STATIC_STATUS_NAMES[id]) return STATIC_STATUS_NAMES[id];

        if (lists.projectStatuses) {
            const s = lists.projectStatuses.find((x: any) => x.Value == id);
            if (s) return s.Text;
        }
        return `Status ${id}`;
    };

    // Estructuras de datos
    const finalMap: Record<number, Record<string, number>> = {};
    const auditMap: Record<number, any> = {};
    const allFyIds = new Set<number>();

    // -------------------------------------------------------------------------
    // A. PROCESAR CARGA EXISTENTE (Base de Datos)
    // -------------------------------------------------------------------------
    if (currentLoadRaw && currentLoadRaw.length > 0) {
        currentLoadRaw.forEach((item: any) => {
            const fyId = item.FY;

            // Detección robusta de propiedad ID (incluyendo 'St')
            const rawStatusId = item.St
                || item.ID_Status
                || item.StatusId
                || 0;

            const capInfo = plantCapacity.find(c => c.FY === fyId);
            let available = (capInfo ? (isSlitterContext ? capInfo.Slt : capInfo.Blk) : 0) || 1;
            if (available <= 0) available = 1;

            const percentage = (item.Hrs / available) * 100;

            // Determinar nombre
            let statusName = "Unclassified";
            if (rawStatusId > 0) {
                statusName = getStatusName(rawStatusId);
            }

            // Acumular en Mapa Final (Para Chart.js)
            if (!finalMap[fyId]) finalMap[fyId] = {};
            finalMap[fyId][statusName] = (finalMap[fyId][statusName] || 0) + percentage;

            // Acumular en Mapa Auditoría
            if (!auditMap[fyId]) auditMap[fyId] = {};
            auditMap[fyId][statusName] = (auditMap[fyId][statusName] || 0) + percentage;

            allFyIds.add(fyId);
        });
    }

    // -------------------------------------------------------------------------
    // B. PROCESAR CARGA NUEVA (Proyecto Actual)
    // -------------------------------------------------------------------------
    const myLineHours = newLoadHours[activeLineId] || {};

    // Determinar a qué estatus va lo nuevo (Usamos el ID pasado desde el padre)
    const targetStatus = projectStatusName || STATIC_STATUS_NAMES[currentProjectStatusId] || "Quotes";

    Object.entries(myLineHours).forEach(([fyIdStr, hours]) => {
        const fyId = Number(fyIdStr);
        const capInfo = plantCapacity.find(c => c.FY === fyId);
        let available = (capInfo ? (isSlitterContext ? capInfo.Slt : capInfo.Blk) : 0) || 1;
        if (available <= 0) available = 1;

        // Convertir Horas a Turnos si es Slitter (7.5h por turno)
        let numerator = isSlitterContext ? (hours / 7.5) : hours;

        const addedPercentage = (numerator / available) * 100;

        // 1. Sumar al Mapa Final (Acumulado para la gráfica)
        if (!finalMap[fyId]) finalMap[fyId] = {};
        finalMap[fyId][targetStatus] = (finalMap[fyId][targetStatus] || 0) + addedPercentage;

        // 2. Guardar en Auditoría (Separado para visualizar el incremento)
        if (!auditMap[fyId]) auditMap[fyId] = {};
        auditMap[fyId]["_ADDED_VAL_"] = addedPercentage;

        allFyIds.add(fyId);
    });


    // -------------------------------------------------------------------------
    // C. PADDING VISUAL & FILTRADO: (SOP - 1 año) hasta (EOP + 1 año)
    // -------------------------------------------------------------------------

    // Definimos las variables de rango fuera del try/catch para usarlas en el filtro
    let viewStart: Date | null = null;
    let viewEnd: Date | null = null;

    if (formData.Real_SOP && formData.Real_EOP && lists.fiscalYears) {
        try {
            // Helper simple para fechas YYYY-MM
            const parseDate = (dStr: string) => new Date(dStr.length === 7 ? `${dStr}-01` : dStr);

            const sopDate = parseDate(formData.Real_SOP);
            const eopDate = parseDate(formData.Real_EOP);

            // 1. Calcular rango extendido (-1 año / +1 año)
            viewStart = new Date(sopDate);
            viewStart.setFullYear(viewStart.getFullYear() - 1);

            viewEnd = new Date(eopDate);
            viewEnd.setFullYear(viewEnd.getFullYear() + 1);

            // 2. Buscar qué Años Fiscales caen en ese rango y asegurar que estén en allFyIds
            lists.fiscalYears.forEach(fy => {
                const fyStart = new Date(fy.Start);
                const fyEnd = new Date(fy.End);

                // Si el FY se traslapa con nuestro rango de visualización...
                if (fyStart <= viewEnd! && fyEnd >= viewStart!) {
                    allFyIds.add(fy.ID); // ...¡Lo forzamos a aparecer!

                    // Inicializamos sus objetos si no existen para evitar errores
                    if (!finalMap[fy.ID]) finalMap[fy.ID] = {};
                    if (!auditMap[fy.ID]) auditMap[fy.ID] = {};
                }
            });
        } catch (e) {
            console.warn("Error calculando rango de fechas extendido:", e);
        }
    }

    // -------------------------------------------------------------------------
    // D. GENERAR TABLA DE AUDITORÍA REORDENADA Y FILTRADA
    // -------------------------------------------------------------------------

    // 🔥 FILTRO CRÍTICO: Aquí eliminamos los años que vienen de BD pero están fuera del rango
    const sortedFYs = Array.from(allFyIds)
        .filter(fyId => {
            // Si no se pudo calcular el rango (ej. faltan fechas), mostramos todo por defecto
            if (!viewStart || !viewEnd) return true;

            const fyDef = lists.fiscalYears?.find(f => f.ID === fyId);
            if (!fyDef) return false;

            const fyStart = new Date(fyDef.Start);
            const fyEnd = new Date(fyDef.End);

            // Verificar intersección con el rango de visualización
            return fyEnd >= viewStart && fyStart <= viewEnd;
        })
        .sort((a, b) => a - b);

    const auditTable: any[] = [];

    sortedFYs.forEach(fyId => {
        const fyObj = lists.fiscalYears?.find(f => f.ID === fyId);
        const capInfo = plantCapacity.find(c => c.FY === fyId);
        const available = (capInfo ? (isSlitterContext ? capInfo.Slt : capInfo.Blk) : 0) || 1;
        const rowData = auditMap[fyId] || {};
        const addedVal = rowData["_ADDED_VAL_"] || 0;

        // Construcción de Objeto: El orden de asignación define el orden de columnas en console.table
        const orderedRow: any = {};

        // 1. Identificadores
        orderedRow["Línea"] = activeLineName;
        orderedRow["Fiscal Year"] = fyObj ? fyObj.Name : `FY ${fyId}`;
        orderedRow["Capacidad"] = isSlitterContext ? `${available.toFixed(1)} T` : `${Math.round(available)} H`;

        // 2. Estatus Dinámicos (En orden de STATUS_ORDER)
        STATUS_ORDER.forEach(status => {
            let baseVal = rowData[status] || 0;

            // Si este es el estatus del proyecto actual, mostramos desglose
            if (status === targetStatus && addedVal > 0) {
                orderedRow[status] = `${baseVal.toFixed(2)}% (+${addedVal.toFixed(2)}%)`;
            } else {
                // Solo mostrar si tiene valor o es > 0
                orderedRow[status] = baseVal > 0 ? `${baseVal.toFixed(2)}%` : "";
            }
        });

        // 3. Columna de Referencia de lo Agregado
        // Usamos targetStatus dentro de los corchetes [] para que el nombre de la columna sea dinámico
        orderedRow[`➕ AGREGADO (${targetStatus})`] = addedVal > 0 ? `+${addedVal.toFixed(2)}%` : "0%";
        // 4. Total Final (Calculado desde finalMap para exactitud)
        let finalTotal = 0;
        if (finalMap[fyId]) {
            Object.values(finalMap[fyId]).forEach(v => finalTotal += v);
        }
        orderedRow["TOTAL OCUPACIÓN"] = `${finalTotal.toFixed(2)}%`;

        auditTable.push(orderedRow);
    });

    console.table(auditTable);
    console.groupEnd();

    // -------------------------------------------------------------------------
    // E. GENERAR DATASETS PARA CHART.JS
    // -------------------------------------------------------------------------

    // Eje X: Etiquetas de Años Fiscales
    const labels = sortedFYs.map(id => {
        const fy = lists.fiscalYears?.find(f => f.ID === id);
        return fy ? fy.Name : `FY ${id}`;
    });

    // Identificar estatus presentes
    const distinctStatuses = new Set<string>();
    Object.values(finalMap).forEach(dict => Object.keys(dict).forEach(s => distinctStatuses.add(s)));

    // Construir Series de Datos
    const datasets = Array.from(distinctStatuses).map(status => {
        // Mapeamos usando sortedFYs para asegurar que los datos coincidan con el eje X filtrado
        const data = sortedFYs.map(fyId => {
            return finalMap[fyId] && finalMap[fyId][status]
                ? Number(finalMap[fyId][status].toFixed(2))
                : 0;
        });

        return {
            label: status,
            data: data,
            backgroundColor: STATUS_COLORS[status] || "#999", // Fallback gris
            fill: true,
            stack: 'Stack 0' // Apilamiento habilitado
        };
    })
        // Ordenar visualmente (Base abajo -> Nuevo arriba)
        .sort((a, b) => {
            let idxA = STATUS_ORDER.indexOf(a.label);
            let idxB = STATUS_ORDER.indexOf(b.label);

            // Si no está en la lista (ej: Unclassified), va al final o principio según preferencia
            if (idxA === -1) idxA = 99;
            if (idxB === -1) idxB = 99;

            return idxA - idxB;
        });

    // Calcular máximo eje Y (para evitar cortes visuales)
    let maxTotal = 0;
    sortedFYs.forEach((_, idx) => {
        const sum = datasets.reduce((acc, ds) => acc + (ds.data[idx] || 0), 0);
        if (sum > maxTotal) maxTotal = sum;
    });

    const maxPercentage = Math.ceil(Math.max(100, maxTotal) * 1.1);

    return {
        labels,
        datasets,
        maxPercentage,
        lineName: activeLineName
    };
};

// ====================================================================================
// 🔥 NUEVA FUNCIÓN MULTI-LÍNEA (REEMPLAZA A LA ANTIGUA)
// ====================================================================================
export const generateMultiLineChartData = (
    newLoadHours: Record<number, Record<number, number>>, // Carga total del proyecto agrupada por línea
    allProjectMaterials: Partial<Material>[], // Materiales del proyecto para cálculo de rangos
    formData: Partial<Material>, // Material actual (para fallback de fechas)
    lists: AppLists,
    projectPlantId: number,
    projectStatusName: string,
    currentProjectStatusId: number
): ChartDataPoints[] => {

    const charts: ChartDataPoints[] = [];
    console.groupCollapsed("📊 PASO 4: Generación Multi-Línea de Gráficas y Auditoría");

    // 1. Identificar qué líneas tienen actividad
    // Incluimos la línea actual del formulario + las que vienen en el cálculo total
    const linesToProcess = new Set<number>();

   // --- CORRECCIÓN DEFINITIVA AQUÍ ---
    // Antes: Agregábamos la línea siempre (if currentLine > 0...)
    // Ahora: Solo agregamos la línea del formulario si el material es VÁLIDO.
    // Si falta volumen o ciclo, no la forzamos. Si hay otros materiales (hermanos) en esa línea,
    // se agregarán por el bucle de newLoadHours de abajo.
    const currentLine = Number(formData.ID_Real_Blanking_Line || formData.ID_Theoretical_Blanking_Line || 0);
    const isCurrentMaterialValid = validateMaterialCapacity(formData).isValid;

    if (currentLine > 0 && isCurrentMaterialValid) {
        linesToProcess.add(currentLine);
    }
    // ----------------------------------

    Object.keys(newLoadHours).forEach(k => linesToProcess.add(Number(k)));

    if (linesToProcess.size === 0) {
        console.warn("⚠️ No hay líneas activas para graficar.");
        console.groupEnd();
        return [];
    }

    // 2. Iterar sobre cada línea y aplicar LA MISMA LÓGICA que tenías para una sola
    linesToProcess.forEach(activeLineId => {
        if (activeLineId === 0) return;

        const lineObj = lists.linesList?.find((l: any) => Number(l.Value) === activeLineId);
        const activeLineName = lineObj ? lineObj.Text : `Line ${activeLineId}`;

        console.group(`📈 Análisis para: ${activeLineName}`);

        // --- LÓGICA DE RANGO DINÁMICO POR LÍNEA ---
        let minDate: Date | null = null;
        let maxDate: Date | null = null;

        // A. Filtramos los materiales que "viven" en esta línea
        const materialsOnThisLine = allProjectMaterials.filter(m => {
            const rLine = Number(m.ID_Real_Blanking_Line);
            const tLine = Number(m.ID_Theoretical_Blanking_Line);
            const effLine = rLine > 0 ? rLine : (tLine > 0 ? tLine : 0);
            return effLine === activeLineId;
        });

        // B. Buscamos el SOP más antiguo y el EOP más futuro
        materialsOnThisLine.forEach(m => {
            if (m.Real_SOP) {
                // Convertir "YYYY-MM" a fecha
                const s = new Date(m.Real_SOP.length === 7 ? `${m.Real_SOP}-01` : m.Real_SOP);
                if (!minDate || s < minDate) minDate = s;
            }
            if (m.Real_EOP) {
                // Convertir "YYYY-MM" a fecha
                const e = new Date(m.Real_EOP.length === 7 ? `${m.Real_EOP}-01` : m.Real_EOP);
                if (!maxDate || e > maxDate) maxDate = e;
            }
        });

        // C. Aplicamos Padding (-1 año al inicio, +1 año al final)
        let viewStart: Date | null = null;
        let viewEnd: Date | null = null;

        if (minDate && maxDate) {
            viewStart = new Date(minDate);
            viewStart.setFullYear(viewStart.getFullYear() - 1);

            viewEnd = new Date(maxDate);
            viewEnd.setFullYear(viewEnd.getFullYear() + 1);
        } else {
            // Fallback: Si la línea tiene carga pero no tiene materiales con fechas (raro, pero posible si viene de BD puro)
            // Usamos las fechas del form actual por defecto
            if (formData.Real_SOP && formData.Real_EOP) {
                const s = new Date(formData.Real_SOP.length === 7 ? `${formData.Real_SOP}-01` : formData.Real_SOP);
                const e = new Date(formData.Real_EOP.length === 7 ? `${formData.Real_EOP}-01` : formData.Real_EOP);
                viewStart = new Date(s); viewStart.setFullYear(s.getFullYear() - 1);
                viewEnd = new Date(e); viewEnd.setFullYear(e.getFullYear() + 1);
            }
        }
        // ---------------------------------------------

        const slitterLineId = Number(formData.ID_Slitting_Line);
        const isSlitterContext = slitterLineId > 0 && activeLineId === slitterLineId;

        // Datos base de BD
        const plantCapacity = lists.capacityData ? lists.capacityData[projectPlantId.toString()] : [];
        const currentLoadRaw = lists.currentLoad ? lists.currentLoad[activeLineId.toString()] : [];

        // Helper Status
        const getStatusName = (id: number): string => {
            if (STATIC_STATUS_NAMES[id]) return STATIC_STATUS_NAMES[id];
            const s = lists.projectStatuses?.find((x: any) => x.Value == id);
            return s ? s.Text : `Status ${id}`;
        };

        const finalMap: Record<number, Record<string, number>> = {};
        const auditMap: Record<number, any> = {};
        const allFyIds = new Set<number>();

        // A. PROCESAR CARGA BASE (BD)
        if (currentLoadRaw && currentLoadRaw.length > 0) {
            currentLoadRaw.forEach((item: any) => {
                const fyId = item.FY;
                const rawStatusId = item.St || item.ID_Status || item.StatusId || 0;

                // Buscar capacidad total de la línea para ese año
                const loadItem = currentLoadRaw.find((x: any) => x.FY === fyId);
                // Usamos 'as any' para acceder a TotalHours si TS se queja
                const totalCapacityHours = (loadItem as any)?.TotalHours || 0;

                // Si no hay TotalHours en el objeto, intentamos usar el capacityData genérico (fallback)
                const capInfo = plantCapacity.find(c => c.FY === fyId);
                let available = 1;

                // Preferencia: Si el endpoint devolvió TotalHours específico, usalo. Si no, usa el genérico.
                if (totalCapacityHours > 0) {
                    available = totalCapacityHours;
                } else {
                    available = (capInfo ? (isSlitterContext ? capInfo.Slt : capInfo.Blk) : 0) || 1;
                }

                if (available <= 0) available = 1;

                const percentage = (item.Hrs / available) * 100;
                const statusName = rawStatusId > 0 ? getStatusName(rawStatusId) : "Unclassified";

                if (!finalMap[fyId]) finalMap[fyId] = {};
                finalMap[fyId][statusName] = (finalMap[fyId][statusName] || 0) + percentage;

                if (!auditMap[fyId]) auditMap[fyId] = {};
                auditMap[fyId][statusName] = (auditMap[fyId][statusName] || 0) + percentage;

                allFyIds.add(fyId);
            });
        }

        // B. PROCESAR CARGA NUEVA (PROYECTO ACTUAL)
        const myLineHours = newLoadHours[activeLineId] || {};
        const targetStatus = projectStatusName || STATIC_STATUS_NAMES[currentProjectStatusId] || "Quotes";

        Object.entries(myLineHours).forEach(([fyIdStr, hours]) => {
            const fyId = Number(fyIdStr);

            // Misma lógica de capacidad disponible para ser consistente
            let available = 1;
            // Intenta buscar en BD load primero
            const dbItem = currentLoadRaw?.find((x: any) => x.FY === fyId);
            if ((dbItem as any)?.TotalHours) {
                available = (dbItem as any).TotalHours;
            } else {
                const capInfo = plantCapacity.find(c => c.FY === fyId);
                available = (capInfo ? (isSlitterContext ? capInfo.Slt : capInfo.Blk) : 0) || 1;
            }
            if (available <= 0) available = 1;

            // Ajuste turno slitter
            const numerator = isSlitterContext ? (hours / 7.5) : hours;
            const addedPercentage = (numerator / available) * 100;

            if (!finalMap[fyId]) finalMap[fyId] = {};
            finalMap[fyId][targetStatus] = (finalMap[fyId][targetStatus] || 0) + addedPercentage;

            if (!auditMap[fyId]) auditMap[fyId] = {};
            auditMap[fyId]["_ADDED_VAL_"] = addedPercentage;

            allFyIds.add(fyId);
        });

        // D. TABLA DE AUDITORÍA Y SORTING
        const sortedFYs = Array.from(allFyIds)
            .filter(fyId => {
                if (!viewStart || !viewEnd) return true;
                const fyDef = lists.fiscalYears?.find(f => f.ID === fyId);
                if (!fyDef) return false;
                const fyStart = new Date(fyDef.Start);
                const fyEnd = new Date(fyDef.End);
                return fyEnd >= viewStart && fyStart <= viewEnd;
            })
            .sort((a, b) => a - b);

        const auditTable: any[] = [];
        sortedFYs.forEach(fyId => {
            const fyObj = lists.fiscalYears?.find(f => f.ID === fyId);
            // Capacidad para mostrar en tabla
            let capDisplay = 0;
            const dbItem = currentLoadRaw?.find((x: any) => x.FY === fyId);
            if ((dbItem as any)?.TotalHours) capDisplay = (dbItem as any).TotalHours;
            else {
                const ci = plantCapacity.find(c => c.FY === fyId);
                capDisplay = (ci ? (isSlitterContext ? ci.Slt : ci.Blk) : 0) || 0;
            }

            const rowData = auditMap[fyId] || {};
            const addedVal = rowData["_ADDED_VAL_"] || 0;
            const orderedRow: any = {};

            orderedRow["Línea"] = activeLineName;
            orderedRow["Fiscal Year"] = fyObj ? fyObj.Name : `FY ${fyId}`;
            orderedRow["Capacidad"] = isSlitterContext ? `${capDisplay.toFixed(1)} T` : `${Math.round(capDisplay)} H`;

            STATUS_ORDER.forEach(st => {
                let base = rowData[st] || 0;
                if (st === targetStatus && addedVal > 0) orderedRow[st] = `${base.toFixed(2)}% (+${addedVal.toFixed(2)}%)`;
                else orderedRow[st] = base > 0 ? `${base.toFixed(2)}%` : "";
            });

            let total = 0;
            if (finalMap[fyId]) Object.values(finalMap[fyId]).forEach(v => total += v);
            orderedRow["TOTAL"] = `${total.toFixed(2)}%`;
            auditTable.push(orderedRow);
        });

        console.table(auditTable);
        console.groupEnd(); // Fin grupo línea

        // ... (Generación de Datasets igual que antes, usando sortedFYs) ...
        const labels = sortedFYs.map(id => lists.fiscalYears?.find(f => f.ID === id)?.Name || `FY ${id}`);
        const distinctStatuses = new Set<string>();
        Object.values(finalMap).forEach(d => Object.keys(d).forEach(s => distinctStatuses.add(s)));

        const datasets = Array.from(distinctStatuses).map(st => {
            const data = sortedFYs.map(fyId => (finalMap[fyId] && finalMap[fyId][st]) ? Number(finalMap[fyId][st].toFixed(2)) : 0);
            return {
                label: st,
                data: data,
                backgroundColor: STATUS_COLORS[st] || "#999",
                fill: true,
                stack: 'Stack 0'
            };
        }).sort((a, b) => {
            let iA = STATUS_ORDER.indexOf(a.label), iB = STATUS_ORDER.indexOf(b.label);
            if (iA === -1) iA = 99; if (iB === -1) iB = 99;
            return iA - iB;
        });

        let maxTotal = 0;
        sortedFYs.forEach((_, i) => {
            const sum = datasets.reduce((acc, d) => acc + (d.data[i] || 0), 0);
            if (sum > maxTotal) maxTotal = sum;
        });

        // NUEVO: Helper local para convertir las fechas MIN/MAX calculadas en Nombres de FY
        const getFyName = (d: Date | null) => {
            if (!d || !lists.fiscalYears) return undefined;
            // Buscamos en qué FY cae esta fecha
            const match = lists.fiscalYears.find(f => {
                const s = new Date(f.Start);
                const e = new Date(f.End);
                return d >= s && d <= e;
            });
            return match ? match.Name : undefined;
        };

        // Calculamos los marcadores específicos para ESTA línea
        // minDate y maxDate ya fueron calculados al inicio del bucle (sección B)
        const specificFyStart = getFyName(minDate);
        const specificFyEnd = getFyName(maxDate);

        charts.push({
            labels,
            datasets,
            maxPercentage: Math.ceil(Math.max(100, maxTotal) * 1.1),
            lineName: activeLineName,
            // 👇 ASIGNAMOS LOS VALORES ESPECÍFICOS
            fyStartLine: specificFyStart,
            fyEndLine: specificFyEnd
        });
    });

    console.groupEnd(); // Fin del grupo paso 4
    return charts;
};

/**
 * Genera la estructura de MATRIZ para la Tabla de Resumen (Por Año Fiscal)
 */
export const generateCapacityBreakdown = (
    allMaterials: Partial<Material>[],
    currentEditingId: number,
    lists: AppLists,
    projectPlantId: number,
    projectStatusName: string
): LineCapacityGroup[] => {

    const groups: Record<number, LineCapacityGroup> = {};

    // 1. Obtener datos de referencia
    const plantCapacity = lists.capacityData ? lists.capacityData[projectPlantId.toString()] : [];
    const fiscalYears = lists.fiscalYears || [];

    allMaterials.forEach(mat => {
        // A. Determinar Línea
        const rLine = Number(mat.ID_Real_Blanking_Line);
        const tLine = Number(mat.ID_Theoretical_Blanking_Line);
        const lineId = rLine > 0 ? rLine : (tLine > 0 ? tLine : 0);

        const safeLineId = lineId;
        const lineName = safeLineId === 0
            ? "⚠️ No Line Assigned"
            : (lists.linesList?.find((l: any) => Number(l.Value) === safeLineId)?.Text || `Line ${safeLineId}`);

        // B. Inicializar Grupo si no existe
        if (!groups[safeLineId]) {
            groups[safeLineId] = {
                lineId: safeLineId,
                lineName: lineName,
                materials: [],
                lineTotals: {},
                activeFYs: []
            };
        }

        // C. Validación Básica
        // Pasamos solo 'mat' porque quitamos dependencias innecesarias de la validación simple
        const validation = validateMaterialCapacity(mat);
        const fyBreakdown: Record<string, number> = {};
        const isCurrent = mat.ID_Material === currentEditingId || (currentEditingId === 0 && !mat.ID_Material);

        // D. CÁLCULO DETALLADO POR AÑO (Solo si es válido)
        if (validation.isValid) {

            // 1. Calculamos MINUTOS por FY ID usando tu función existente
            const minutesMap = calculateMaterialLoad(mat, lists, fiscalYears);

            const slitterLineId = Number(mat.ID_Slitting_Line);
            const isSlitter = slitterLineId > 0 && lineId === slitterLineId;

            // 2. Convertir Minutos a Porcentaje por FY
            Object.entries(minutesMap).forEach(([fyIdStr, minutes]) => {
                const fyId = Number(fyIdStr);
                const fyDef = fiscalYears.find(f => f.ID === fyId);
                if (!fyDef || minutes <= 0) return;

                const fyName = fyDef.Name;

                // Buscar capacidad instalada para ese año
                // Nota: Para la tabla resumen usamos 'plantCapacity' (genérico) o tratamos de ser precisos
                // Si tienes 'currentLoad' cargado en lists, podrías buscar ahí el 'TotalHours' específico, 
                // pero 'plantCapacity' es más seguro como fallback rápido.
                const capInfo = plantCapacity.find(c => c.FY === fyId);

                // Definir denominador (Horas disponibles anuales)
                let availableHours = 1;
                if (capInfo) {
                    // Si es Slitter y la BD lo maneja en turnos, ajusta aquí.
                    // Asumiremos que capacityData trae Horas o Turnos estándar.
                    // Tu lógica gráfica usa: isSlitter ? capInfo.Slt : capInfo.Blk
                    availableHours = isSlitter ? capInfo.Slt : capInfo.Blk;

                    // Si es Slitter, a veces availableHours viene en Turnos, y minutes en Minutos.
                    // Ajuste típico de tu lógica gráfica:
                    // if (isSlitter) numerator = minutes / 60 / 7.5; ...
                    // Para simplificar la tabla, convertiremos todo a % directo de ocupación.
                    // Si 'availableHours' son Horas anuales:
                }

                // Fallback seguro
                if (availableHours <= 0) availableHours = 5000; // ~3 turnos default

                // Cálculo %
                // Si es slitter y available son Turnos: minutes/60/7.5 / available * 100
                // Si available son Horas: minutes/60 / available * 100
                // Usaremos lógica estándar de Horas por ahora:
                const hours = minutes / 60;

                // Ajuste especial Slitter si tu 'available' viene en turnos (según tu código previo)
                let finalPercent = 0;
                if (isSlitter) {
                    // Asumiendo que availableHours es capacidad en TURNOS (como en tu gráfica)
                    // y hours son horas reales.
                    const shiftsRequired = hours / 7.5;
                    finalPercent = (shiftsRequired / availableHours) * 100;
                } else {
                    finalPercent = (hours / availableHours) * 100;
                }

                // Guardar en el breakdown
                fyBreakdown[fyName] = finalPercent;

                // Sumar al total de la línea
                groups[safeLineId].lineTotals[fyName] = (groups[safeLineId].lineTotals[fyName] || 0) + finalPercent;

                // Registrar que este año tiene datos
                if (!groups[safeLineId].activeFYs.includes(fyName)) {
                    groups[safeLineId].activeFYs.push(fyName);
                }
            });
        }

        groups[safeLineId].materials.push({
            materialId: mat.ID_Material || 0,
            partNumber: mat.Part_Number || "New Material",
            lineId: safeLineId,
            lineName: lineName,
            isCurrentEditing: isCurrent,
            validation: validation,
            fyBreakdown: fyBreakdown,
            projectStatus: projectStatusName
        });
    });

    // Ordenar FYs cronológicamente para cada grupo
    Object.values(groups).forEach(g => {
        g.activeFYs.sort(); // String sort "FY24", "FY25" funciona bien.
    });

    return Object.values(groups).sort((a, b) => a.lineId - b.lineId);
};