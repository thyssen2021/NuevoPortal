import type { Material, AppLists, FiscalYearDef, IhsProductionItem, ChartDataPoints } from '../types';

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
const STATUS_ORDER = ["Spare Parts", "POH", "Casi Casi", "Carry Over", "Quotes"];/**
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

    // 1. Obtener Inputs
    const partNumber = formData.Part_Number || "N/A"; // 👈 Capturamos el número de parte
    const routeId = Number(formData.ID_Route || 0);    
    let annualVol = Number(formData.Annual_Volume || 0);
    // Si es ruta de Blanking y existe volumen de blanking, lo usamos con prioridad
    if (BLANKING_ROUTE_IDS.includes(routeId)) {
        const blkVol = Number(formData.Blanking_Annual_Volume);
        if (blkVol > 0) {
            annualVol = blkVol;
        }
    }

    const partsPerVeh = Number(formData.Parts_Per_Vehicle || 1);

    // Prioridad de Ciclo: ICT > Theo > Real > Default 1
    const ict = Number(formData.Ideal_Cycle_Time_Per_Tool || formData.Theoretical_Strokes || formData.Real_Strokes || 1);

    const blanksPerStroke = Number(formData.Blanks_Per_Stroke || 1);
    const lineId = Number(formData.ID_Real_Blanking_Line || formData.ID_Theoretical_Blanking_Line || 0);
    // Validación básica
    if (annualVol === 0 || lineId === 0) return {};

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
            "Part #": partNumber, // 👈 PRIMERA COLUMNA
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
            const s = new Date(`${formData.Real_SOP}-01`);
            const e = new Date(`${formData.Real_EOP}-28`);
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
        console.groupCollapsed(`📊 CÁLCULO DE CAPACIDAD: ${partNumber}`); // 👈 Título del grupo

        console.log("1. CONSTANTES GLOBALES:");
        console.log({
            "Line ID": lineId,
            "Parts Per Vehicle": partsPerVeh,
            "Cycle Time": ict,
            "Blanks Per Stroke": blanksPerStroke,
            "OEE": oee,
            "Denominator": denominator.toFixed(4)
        });

        console.log("2. TABLA DE CÁLCULO POR AÑO:");
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
            console.log(`Config: Factor ${factor}% | OEE ${(oee*100).toFixed(0)}% | ICT ${ict}`);

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