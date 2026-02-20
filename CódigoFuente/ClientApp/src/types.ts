// src/types.ts
// 1. Definimos una interfaz auxiliar para los items de los dropdowns
export interface DropdownItem {
    value?: string | number; // Opción minúscula
    text?: string;
    Value?: string | number; // Opción Mayúscula (C#)
    Text?: string;
}

// 2. Definimos la estructura del objeto 'lists'
export interface AppLists {
    ihsCountries: DropdownItem[];
    materialTypes: DropdownItem[];
    interplantPlants: DropdownItem[];
    qualityList: DropdownItem[];
    millList: DropdownItem[];
    shapes: DropdownItem[];
    rackTypeList: DropdownItem[];
    labelList: DropdownItem[];
    additionalList: DropdownItem[]; 
    strapTypeList: DropdownItem[];
    freightTypeList: DropdownItem[];
    slittingRules: SlittingValidationRule[];
    capacityData: Record<string, CapacityInfo[]>; // Key: PlantID
    currentLoad: Record<string, CurrentLoadItem[]>; // Key: LineID
    oeeHistory: Record<string, number>; // Key: LineID -> AvgOEE
    fiscalYears: FiscalYearDef[];
    [key: string]: any; // Esto permite flexibilidad para otras listas futuras    
    projectStatuses?: Array<{ Value: number; Text: string }>; // 👈 Debe coincidir con el nombre en C#
}

export interface ChartDataPoints {
    labels: string[];
    datasets: any[];
    maxPercentage: number;
    lineName: string;
    fyStartLine?: string; 
    fyEndLine?: string;
}

export interface SlittingValidationRule {
    ID_Production_Line: number;
    LineName?: string; // Nombre amigable para mostrar en la tabla
    Thickness_Min: number;
    Thickness_Max: number;
    Tensile_Min: number;
    Tensile_Max: number;
    Width_Min?: number | null;
    Width_Max?: number | null;
    Mults_Max: number;
    Is_Active: boolean;
}

export interface TheoreticalRule {
    materialTypeId: number;
    thicknessMin: number | null;
    thicknessMax: number | null;
    widthMin: number | null;
    widthMax: number | null;
    pitchMin: number | null;
    pitchMax: number | null;
    tensileMin: number | null;
    tensileMax: number | null;
    priority: number;
    resultingLineId: number;
    resultingLineName: string;
}

export interface EngineeringRange {
    lineId: number;
    materialTypeId: number;
    criteriaId: number;
    minValue: number | null;
    maxValue: number | null;
    numericValue: number | null;
    tolerance: number | null; 
}

// Respuesta del endpoint GetEngineeringDimensions
export interface EngineeringResponse {
    success: boolean;
    message?: string;
    validationRanges: EngineeringRange[];
    maxMultsAllowed: number | null; // 👇 AQUÍ ESTÁ EL DATO CLAVE
}


export interface Material {
    // Identificadores
    ID_Material: number;
    ID_Project?: number;

    // Campos principales (Tal cual vienen en tu JSON)

    Vehicle?: string;
    Vehicle_2?: string;
    Vehicle_3?: string;
    IHS_Country?: string;
    IHS_Country_2?: string;
    IHS_Country_3?: string;

    // Fechas (Vienen como string ISO desde C#)
    SOP_SP?: string;
    EOP_SP?: string;
    Real_SOP?: string;
    Real_EOP?: string;

    Max_Production_SP?: number;       // El valor sumado de los JSON de producción
    Max_Production_Factor?: number;   // Tu Adj. Factor % (el que quieres con 100 por defecto)
    Max_Production_Effective?: number; // El resultado final de Max * Factor
    IsRunningChange?: boolean;
    IsCarryOver?: boolean;
    // Otros campos que vi en tu JSON
    Program_SP?: string;

    ID_Route?: number;
    Part_Number: string;
    Part_Name: string;
    Ship_To?: string;
    ID_Interplant_Plant?: number

    ID_Arrival_Transport_Type?: number;
    Arrival_Transport_Type_Other?: string;
    ID_Arrival_Packaging_Type?: number;
    ID_Arrival_Protective_Material?: number;
    Arrival_Protective_Material_Other?: string;
    Is_By_Container?: boolean;
    Is_Stackable?: boolean; // Checkbox de apilable    
    Stackable_Levels?: number; // Cantidad de niveles (Numérico)
    ID_Arrival_Warehouse?: number;
    PassesThroughSouthWarehouse?: boolean;
    Arrival_Comments?: string;

    //archivo arrival
    ID_File_ArrivalAdditional?: number;
    FileName_ArrivalAdditional?: string; // Para mostrar el nombre si ya existe
    file_ArrivalAdditional?: File | null; // Propiedad temporal para guardar el binario (File) en memoria antes de subir

    Quality?: string;
    ID_Material_type?: number;
    Mill?: string;
    MaterialSpecification?: string;
    /*------------ */
    Surface?: string;
    Tensile_Strenght?: number;

    Thickness?: number;
    ThicknessToleranceNegative?: number;
    ThicknessTolerancePositive?: number;

    Width?: number;
    WidthToleranceNegative?: number;
    WidthTolerancePositive?: number;

    MasterCoilWeight?: number;
    InnerCoilDiameterArrival?: number;
    OuterCoilDiameterArrival?: number;

    // 👇 NUEVO ARCHIVO
    ID_File_CoilDataAdditional?: number;
    FileName_CoilDataAdditional?: string; // Para mostrar nombre al editar
    coilDataAdditionalFile?: File | null; // Para subir el binario

    /* -------------- */
    Multipliers?: number;
    Width_Mults?: number;
    Width_Mults_Tol_Neg?: number;
    Width_Mults_Tol_Pos?: number;
    // Weight of Final Mults
    WeightOfFinalMults?: number; // Este es el "Optimal" según tu Legacy
    WeightOfFinalMults_Min?: number;
    WeightOfFinalMults_Max?: number;
    // Slitter Data Additional File
    ID_File_SlitterDataAdditional?: number;      // El ID guardado en BD
    FileName_SlitterDataAdditional?: string;     // El nombre para mostrar en el enlace de descarga
    slitterDataAdditionalFile?: File | null;     // El archivo físico al subir

    ID_Shape?: number;
    Blanks_Per_Stroke?: number; // legacy: Blanks_Per_Stroke
    Parts_Per_Vehicle?: number; // legacy: Parts_Per_Vehicle
    // Blank Data - Plates Width (Width_Plates)
    Width_Plates?: number;
    Width_Plates_Tol_Neg?: number;
    Width_Plates_Tol_Pos?: number;
    // Blank Data - Pitch
    Pitch?: number;
    PitchToleranceNegative?: number;
    PitchTolerancePositive?: number;
    // Blank Data - Flatness
    Flatness?: number;
    FlatnessToleranceNegative?: number;
    FlatnessTolerancePositive?: number;
    // Blank Data - Angle A
    Angle_A?: number;
    AngleAToleranceNegative?: number;
    AngleATolerancePositive?: number;
    // Blank Data - Angle B
    Angle_B?: number;
    AngleBToleranceNegative?: number;
    AngleBTolerancePositive?: number;
    // Blank Data - Major Base
    MajorBase?: number;
    MajorBaseToleranceNegative?: number;
    MajorBaseTolerancePositive?: number;
    // Blank Data - Minor Base
    MinorBase?: number;
    MinorBaseToleranceNegative?: number;
    MinorBaseTolerancePositive?: number;
    Theoretical_Gross_Weight?: number;
    Gross_Weight?: number;
    ClientNetWeight?: number;
    // Campos Calculados (Resultados de updateCalculatedWeightFields) 
    TurnOver?: boolean; // Checkbox
    TurnOverSide?: string; // Select (Left/Right)

    IsWeldedBlank?: boolean;
    RequiresDieManufacturing?: boolean;
    NumberOfPlates?: number;    
    // JSON serializado que viene de la BD (Legacy)
    WeldedPlatesJson?: string;     
    // Array helper para el frontend (no es columna directa de BD, se mapea)
    _weldedPlates?: WeldedPlate[];

    // 1. CAD Drawings (Shape 18)
    ID_File_CAD_Drawing?: number;
    CADFileName?: string;         // Para mostrar "archivo.dwg"
    archivo?: File | null;        // ⚠️ Nombre exacto del input legacy

    // 2. Technical Sheet
    ID_File_TechnicalSheet?: number;
    TechnicalSheetFileName?: string;
    technicalSheetFile?: File | null; // ⚠️ Nombre exacto del input legacy

    // 3. Additional File
    ID_File_Additional?: number;
    AdditionalFileName?: string;
    AdditionalFile?: File | null;     // ⚠️ Nombre exacto del input legacy

    // Volúmenes Generales (posiblemente ya existan)
    Annual_Volume?: number;
    Volume_Per_year?: number; // M Tons/Year

    // Blanking Specific Volumes (NUEVOS)
    Blanking_Annual_Volume?: number;
    Blanking_Volume_Per_year?: number; // M Tons/Year
    Blanking_InitialWeightPerPart?: number; // Calculado
    InitialWeightPerPart?: number;          // Calculado
    Blanking_ProcessTons?: number;          // Calculado
    Blanking_ShippingTons?: number;         // Calculado

    Shearing_Pieces_Per_Stroke?: number;
    Shearing_Pieces_Per_Car?: number;

    Shearing_Width?: number;
    Shearing_Width_Tol_Neg?: number;
    Shearing_Width_Tol_Pos?: number;
    // Shearing Data
    Shearing_Pitch?: number;
    Shearing_Pitch_Tol_Neg?: number;
    Shearing_Pitch_Tol_Pos?: number;

    Shearing_Weight?: number;
    Shearing_Weight_Tol_Neg?: number;
    Shearing_Weight_Tol_Pos?: number;

    ID_InterplantDelivery_Coil_Position?: number;
    InterplantPackagingStandard?: string; // 'OWN' | 'CM'
    InterplantRequiresRackManufacturing?: boolean;
    InterplantPiecesPerPackage?: number;
    InterplantStacksPerPackage?: number;
    // Interplant Delivery Packaging
    InterplantPackageWeight?: number; // Calculado    
    // Archivo
    ID_File_InterplantPackaging?: number;
    InterplantPackagingFileName?: string; // Para mostrar nombre al descargar
    interplant_packaging_archivo?: File | null; // Binario
    InterplantRackTypeIds?: number[]; // Guardará un array de números: [1, 3, 5]
    IsInterplantReturnableRack?: boolean;
    InterplantReturnableUses?: number;
    // Checkbox Group (Array de IDs)
    InterplantLabelTypeIds?: number[];     
    // Campo de texto condicional
    InterplantLabelOtherDescription?: string;

    // Interplant Additionals (Checkbox Group + Other)
    InterplantAdditionalIds?: number[]; 
    InterplantAdditionalsOtherDescription?: string;

    // Interplant Standard Packaging / Straps (Checkbox Group + Observations)
    InterplantStrapTypeIds?: number[];
    InterplantStrapTypeObservations?: string;
    InterplantSpecialRequirement?: string;
    InterplantSpecialPackaging?: string;


    ID_Interplant_FreightType?: number;
    // Interplant Outbound Freight
    ID_InterplantDelivery_Transport_Type?: number;
    InterplantDelivery_Transport_Type_Other?: string;
    InterplantLoadPerTransport?: number;
    InterplantDeliveryConditions?: string;

    // Interplant Scrap Reconciliation
    InterplantScrapReconciliation?: boolean;
    InterplantScrapReconciliationPercent_Min?: number;
    InterplantScrapReconciliationPercent?: number; // Optimal
    InterplantScrapReconciliationPercent_Max?: number;
    InterplantClientScrapReconciliationPercent?: number;

    // Interplant Head/Tail Reconciliation
    InterplantHeadTailReconciliation?: boolean;
    InterplantHeadTailReconciliationPercent_Min?: number;
    InterplantHeadTailReconciliationPercent?: number; // Optimal
    InterplantHeadTailReconciliationPercent_Max?: number;
    InterplantClientHeadTailReconciliationPercent?: number;

    // Interplant Outbound Freight File
    ID_File_InterplantOutboundFreight?: number;      // ID en BD
    FileName_InterplantOutboundFreight?: string;     // Nombre para mostrar (hidratado desde CTZ_Files3)
    interplantOutboundFreightAdditionalFile?: File | null; // Binario para subida (nombre exacto del legacy)

    // Final Delivery Packaging
    ID_Delivery_Coil_Position?: number;
    PackagingStandard?: string; // 'OWN' | 'CM'
    RequiresRackManufacturing?: boolean;
    PiecesPerPackage?: number;
    StacksPerPackage?: number;
    PackageWeight?: number; // Calculado
    // 1. Packaging Drawing (CTZ_Files1)
    ID_File_Packaging?: number;
    FileName_Packaging?: string;     // Hidratar esto en MaterialForm
    packaging_archivo?: File | null; // Nombre exacto del input legacy
    // 2. Delivery Packaging Additional (CTZ_Files11)
    ID_File_DeliveryPackagingAdditional?: number;
    FileName_DeliveryPackagingAdditional?: string; // Hidratar esto en MaterialForm
    deliveryPackagingAdditionalFile?: File | null; // Nombre exacto del input legacy
    // Final Delivery - Rack Types
    SelectedRackTypeIds?: number[]; // Stores [1, 5, 2]
    // Final Delivery - Labels
    SelectedLabelIds?: number[];    // Stores [1, 3]
    LabelOtherDescription?: string; // Text for "Other" label (ID 3)
    // Final Delivery - Returnable Rack Logic
    IsReturnableRack?: boolean;
    ReturnableUses?: number;
    // Final Delivery - Additionals
    SelectedAdditionalIds?: number[];
    AdditionalsOtherDescription?: string; // Text for "Other" additional (ID 6)
    // Final Delivery - Standard Packaging (Straps)
    SelectedStrapTypeIds?: number[];
    StrapTypeObservations?: string;       // Always visible observation    
    // Final Delivery - Special Requirements
    SpecialRequirement?: string;
    SpecialPackaging?: string;

    // Final Outbound Freight & Conditions
    ID_FreightType?: number;
    ID_Delivery_Transport_Type?: number;
    Delivery_Transport_Type_Other?: string;
    LoadPerTransport?: number;
    DeliveryConditions?: string;

    // Scrap Reconciliation (Final)
    ScrapReconciliation?: boolean;
    ScrapReconciliationPercent_Min?: number;
    ScrapReconciliationPercent?: number; // Optimal
    ScrapReconciliationPercent_Max?: number;
    ClientScrapReconciliationPercent?: number;

    // Head/Tail Reconciliation (Final)
    HeadTailReconciliation?: boolean;
    HeadTailReconciliationPercent_Min?: number;
    HeadTailReconciliationPercent?: number; // Optimal
    HeadTailReconciliationPercent_Max?: number;
    ClientHeadTailReconciliationPercent?: number;

    // Archivo
    ID_File_OutboundFreightAdditional?: number;
    FileName_OutboundFreightAdditional?: string;
    outboundFreightAdditionalFile?: File | null;

    // Technical Feasibility
    ID_Theoretical_Blanking_Line?: number;
    Theoretical_Blanking_Line_Name?: string; // Para mostrar el nombre en el input de solo lectura
    Theoretical_Strokes?: number;
    Theoretical_Effective_Strokes?: number;
    
    // Real Blanking Data
    ID_Real_Blanking_Line?: number;
    Real_Strokes?: number;
    Real_Effective_Strokes?: number;

    // Technical Feasibility Additional Fields
    Ideal_Cycle_Time_Per_Tool?: number;
    OEE?: number;
    TonsPerShift?: number;
    ID_Slitting_Line?: number;

    // Efficiency and Capacity
    Parts_Auto?: number;
    Strokes_Auto?: number;
    Blanks_Per_Year?: number;
    Min_Max_Reales?: number;
    Min_Max_Reales_OEE?: number;
    Actual_Shifts?: number;
    Strokes_Shift?: number;
    
    // Campos del modelo de datos
    DM_status?: string;
    DM_status_comment?: string;
    // ... puedes agregar más según necesites
}

export interface ProjectData {
    ID_Project: number;
    ID_Plant: number;
    ConcatQuoteID: string;
    InterplantProcess: boolean;
    ID_Status: number;

    Creted_Date?: string;
    OEM_Otro?: string;
    Cliente_Otro?: string;

    // --- OBJETOS ANIDADOS (Aquí están los Friendly Names) ---
    CTZ_Project_Status?: {
        ID_Status: number;
        Description: string;
        ConcatStatus: string;
    };

    CTZ_Clients?: {
        ID_Cliente: number;
        Client_Name: string;
        ConcatSAPName: string; // Ej: "(989) GESTAMP PUEBLA"
    };

    CTZ_Material_Owner?: {
        ID_Owner: number;
        Description: string; // Ej: "Propiedad tkMM"
    };

    empleados?: { // Para "Created By"
        id: number;
        ConcatNombre: string; // Ej: "ALFREDO XOCHITEMOL CRUZ"
    };

    // ¡IMPORTANTE! Este nombre debe ser exacto al JSON:
    CTZ_Project_Materials: Material[];

}

export interface WeldedPlate {
    PlateNumber: number;
    Thickness: number;
}

export interface AppContext {
    project: ProjectData;
    user: { name: string; id: number };
    permissions: {
        canEditSales: boolean;
        canEditEngineering: boolean;
        canEditDataManagement: boolean;
        isDetailsMode: boolean;
    };
    urls: {
        saveUrl: string;
        backUrl: string;
    };
    lists: AppLists;
}

// Definición de Año Fiscal
export interface FiscalYearDef {
    ID: number;
    Name: string;
    Start: string; // Vienen como ISO string desde C#
    End: string;
}

// Capacidad Instalada { [FY_ID]: { Blk: horas, Slt: turnos } }
export interface CapacityInfo {
    FY: number;
    Blk: number;
    Slt: number;
}

// Carga Actual que viene del backend
export interface CurrentLoadItem {
    FY: number;   // ID del Año Fiscal
    St: number;   // ID del Estatus (1=Quotes, 4=POH, etc.) - Antes era ID_Status
    Hrs: number;  // Horas cargadas
}

// Estructura de un item de producción IHS (lo que viene en el JSON)
export interface IhsProductionItem {
    Production_Year: number;
    Production_Month: number; // 👈 Nuevo
    Production_Amount: number;
}

export interface CapacityValidationResult {
    isValid: boolean;
    missingFields: string[];
    // loadPercentage: number; // 👈 YA NO USAMOS ESTE GLOBAL
    // loadMinutes: number;    // 👈 NI ESTE
}

export interface MaterialCapacitySummary {
    materialId: number;
    partNumber: string;
    lineId: number;
    lineName: string;
    isCurrentEditing: boolean;
    validation: CapacityValidationResult;
    // 👇 NUEVO: Diccionario clave (Nombre FY) -> valor (%)
    fyBreakdown: Record<string, number>; 
    projectStatus: string; 
}

export interface LineCapacityGroup {
    lineId: number;
    lineName: string;
    // totalPercentage: number; // 👈 YA NO SIRVE EL TOTAL GLOBAL
    materials: MaterialCapacitySummary[];
    // 👇 NUEVO: Totales por año para el pie de tabla de esa línea
    lineTotals: Record<string, number>; 
    // 👇 NUEVO: Lista de años que tienen datos en esta línea (para pintar columnas)
    activeFYs: string[];
}