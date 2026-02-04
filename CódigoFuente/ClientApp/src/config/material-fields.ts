// config/material-fields
import type { FieldConfig } from '../types/form-schema';

// --- CONSTANTES DE RUTAS (IDs de BD) ---
export const ROUTES = {
  BLK: 1,
  BLK_RP: 2,
  BLK_RPLTZ: 3,
  BLK_SH: 4,
  BLK_WLD: 5,
  COIL_TO_COIL: 6,
  REWINDED: 7,
  SLT: 8,
  SLT_BLK: 9,
  SLT_BLK_WLD: 10,
  WAREHOUSING: 11,
  WAREHOUSING_RP: 12,
  WEIGHT_DIVISION: 13
};

export const PROJECT_STATUS = {
  QUOTES: 1,
  CARRY_OVER: 2,
  CASI_CASI: 3,
  POH: 4,
  SPARE_PARTS: 9,
};

export const materialFields: FieldConfig[] = [
  // --- SECCIÓN: Vehicle Information ---
  {
    name: 'Vehicle',
    label: 'Vehicle Selection',
    type: 'vehicle-selector',
    section: 'Vehicle Information',
    validation: { required: true },
    showInTable: true,
    countryFieldName: 'IHS_Country' // 👈 VINCULAMOS AL CAMPO DE BD
  },
  {
    name: 'Vehicle_2',
    label: 'Vehicle 2',
    type: 'vehicle-selector',
    section: 'Vehicle Information',
    visibleWhen: { field: 'Vehicle', hasValue: true },
    countryFieldName: 'IHS_Country_2' // 👈 VINCULAMOS AL CAMPO DE BD
  },
  {
    name: 'Vehicle_3',
    label: 'Vehicle 3',
    type: 'vehicle-selector',
    section: 'Vehicle Information',
    visibleWhen: { field: 'Vehicle_2', hasValue: true },
    countryFieldName: 'IHS_Country_3' // 👈 VINCULAMOS AL CAMPO DE BD
  },
  {
    name: 'Vehicle_version',
    label: 'Version',
    type: 'text',
    section: 'Vehicle Information',
    className: 'col-md-6',
    showInTable: true,
    validation: {
      required: true,
      maxLength: 50
    }
  },
  {
    name: 'Program_SP',
    label: 'Program S&P',
    type: 'text',
    section: 'Vehicle Information',
    className: 'col-md-6',
    showInTable: true,
    disabled: true, // <--- Esto lo hace readonly
    placeholder: 'Automatic...',
  },
  {
    name: 'IsRunningChange',
    label: 'Is Running Change?',
    type: 'checkbox',
    section: 'Vehicle Information',
    className: 'col-md-6'
  },
  {
    name: 'IsCarryOver',
    label: 'Is Carry Over?',
    type: 'checkbox',
    section: 'Vehicle Information',
    className: 'col-md-6'
  },
  {
    name: 'SOP_SP',
    label: 'SOP S&P', // Start Of Production
    type: 'text',     // O 'date' si prefieres el picker, pero text va bien para YYYY-MM
    section: 'Vehicle Information',
    className: 'col-md-3',
    showInTable: true,
    disabled: true,   // Readonly
    placeholder: 'Min Date',
  },
  {
    name: 'EOP_SP',
    label: 'EOP S&P', // End Of Production
    type: 'text',
    section: 'Vehicle Information',
    className: 'col-md-3',
    showInTable: true,
    disabled: true,   // Readonly
    placeholder: 'Max Date',
  },
  // Real SOP
  {
    name: 'Real_SOP',
    label: 'Real SOP',
    type: 'month', // O 'month' para input nativo de navegador
    section: 'Vehicle Information',
    className: 'col-md-3',
    showInTable: true,
    placeholder: 'yyyy-MM',
    validation: { required: true, pattern: /^\d{4}-\d{2}$/ }
  },
  // Real EOP
  {
    name: 'Real_EOP',
    label: 'Real EOP',
    type: 'month',
    section: 'Vehicle Information',
    className: 'col-md-3',
    placeholder: 'yyyy-MM',
    showInTable: true,
    validation: { required: true, pattern: /^\d{4}-\d{2}$/ }
  },

  // Max Production SP (Calculado Automático)
  {
    name: 'Max_Production_SP',
    label: 'Max Production SP',
    type: 'number',
    section: 'Vehicle Information',
    className: 'col-md-4',
    disabled: true, // Readonly
    showInTable: true,
    placeholder: 'Calculated...'
  },

  // Adjustment Factor (Editable 0-100)
  {
    name: 'Max_Production_Factor',
    label: 'Adj. Factor %',
    type: 'number',
    section: 'Vehicle Information',
    className: 'col-md-4',
    defaultValue: 100,
    showInTable: true,
    validation: { required: true, min: 0, max: 100 }
  },

  // Effective Max Production (Resultado Final)
  {
    name: 'Max_Production_Effective',
    label: 'Effective Max Prod.',
    type: 'number',
    section: 'Vehicle Information',
    className: 'col-md-4',
    showInTable: true,
    disabled: true, // Readonly
    placeholder: 'Result...'
  },
  {
    name: 'ID_Route',           // Debe coincidir con la interfaz del Paso 2
    label: 'Route',             // Etiqueta visual
    type: 'select',             // Tipo Dropdown
    section: 'Production & Route', // Esto creará la pestaña nueva automáticamente a la izquierda
    className: 'col-md-4',
    optionsKey: 'routeList',     // 👈 Debe coincidir con el nombre que pusimos en el Controlador (Paso 1)
    placeholder: '-- Select Route --',
    validation: {
      required: true          // Ajusta a false si no es obligatorio
    }
  },
  {
    name: 'Part_Number',        // Debe coincidir con la interfaz del Paso 1
    label: 'Part Number',
    type: 'text',               // Input de texto normal
    section: 'Production & Route',
    className: 'col-md-4',      // Ocupa 1/3 del ancho
    placeholder: 'Part Number',
    showInTable: true,          // Para que salga en la tabla resumen si la usas
    validation: {
      required: true,
      maxLength: 50
    }
  },
  {
    name: 'Part_Name',
    label: 'Part Name',
    type: 'text',
    section: 'Production & Route',
    className: 'col-md-4',
    placeholder: 'Part Name',
    validation: {
      required: true,
      maxLength: 50
    }
  },
  {
    name: 'Ship_To',
    label: 'Ship To',
    type: 'text',               // Texto libre
    section: 'Production & Route',
    className: 'col-md-4',
    placeholder: 'Ship To',
    validation: {
      required: false,        // Cámbialo a true si es obligatorio
      maxLength: 150
    }
  },
  {
    name: 'ID_Interplant_Plant',      // Debe coincidir con la propiedad en la interfaz Material
    label: 'Interplant Facility',
    type: 'select',
    section: 'Production & Route',    // Pestaña donde aparecerá
    className: 'col-md-4',
    optionsKey: 'interplantPlants',   // Debe coincidir con la key en AppLists (Paso 2)
    placeholder: '-- Select Facility --',
    validation: {
      required: false                 // Ajusta a true si es obligatorio por regla de negocio
    },
    // Opcional: Si quieres que salga en la tabla resumen superior
    showInTable: false,
    visibleWhen: {
      field: '_projectInterplantActive', // Escucha el campo virtual que inyectamos
      is: [true]                         // Solo muestra si es true
    }
  },
  {
    // Reemplaza 'ID_Delivery_Coil_Position' por el nombre real de tu columna en BD si es diferente
    name: 'ID_Delivery_Coil_Position',
    label: 'Coil Position',
    type: 'select',

    // ALERTA: Al poner un nombre nuevo de sección, se creará una pestaña nueva automáticamente
    section: 'Arrival Conditions',

    className: 'col-md-4',
    optionsKey: 'coilPositions', // Debe coincidir con la key en AppLists (Paso 2)
    placeholder: '-- Select Position --',
    validation: {
      required: false, // Por defecto no es requerido
      requiredWhen: {
        field: '_projectStatusId', // Campo virtual que inyectamos
        is: [
          PROJECT_STATUS.QUOTES,
          PROJECT_STATUS.CARRY_OVER,
          PROJECT_STATUS.CASI_CASI,
          PROJECT_STATUS.POH
        ]
      }
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK,
        ROUTES.BLK_RP,
        ROUTES.BLK_RPLTZ,
        ROUTES.BLK_SH,
        ROUTES.BLK_WLD,
        ROUTES.COIL_TO_COIL,
        // ROUTES.REWINDED,  <-- OMITIDO (ID 7) para que se oculte
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
    }
  },
  {
    // Verifica en tu BD si la columna se llama 'ID_Arrival_Transport_Type' o 'ID_Transport_Type'
    name: 'ID_Arrival_Transport_Type',
    label: 'Transportation Type',
    type: 'select',

    // Mantenemos la misma sección
    section: 'Arrival Conditions',

    className: 'col-md-4',

    // IMPORTANTE: Esto debe coincidir con el nombre de la lista que envías desde C#
    optionsKey: 'transportTypes',

    placeholder: '-- Select Transport Type --',

    validation: {
      required: false,
      // Replicamos la lógica: Obligatorio solo para ciertos estatus del proyecto
      requiredWhen: {
        field: '_projectStatusId',
        is: [
          PROJECT_STATUS.QUOTES,
          PROJECT_STATUS.CARRY_OVER,
          PROJECT_STATUS.CASI_CASI,
          PROJECT_STATUS.POH
        ]
      }
    },

    // Replicamos la lógica: Visible en todas las rutas EXCEPTO 'REWINDED'
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK,
        ROUTES.BLK_RP,
        ROUTES.BLK_RPLTZ,
        ROUTES.BLK_SH,
        ROUTES.BLK_WLD,
        ROUTES.COIL_TO_COIL,
        // ROUTES.REWINDED,  <-- Omitido igual que en tu ejemplo
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
    }
  },
  // ... configuración anterior de ID_Arrival_Transport_Type ...

  {
    name: 'Arrival_Transport_Type_Other', // Debe coincidir con types.ts
    label: 'Specify Other Transport',
    type: 'text',
    section: 'Arrival Conditions',
    className: 'col-md-4',
    placeholder: 'Specify details...',

    // VALIDACIÓN: Solo aplica si el campo es visible
    validation: {
      required: true, // Requerido si se muestra
      maxLength: 50   // Límite de caracteres del legacy
    },

    // VISIBILIDAD: Solo se muestra si el dropdown anterior vale 5
    visibleWhen: {
      field: 'ID_Arrival_Transport_Type',
      is: [5] // 5 es el ID de "Other" según tu código legacy
    }
  },

  {
    name: 'ID_Arrival_Packaging_Type',
    label: 'Packaging Type',
    type: 'select',
    section: 'Arrival Conditions',
    className: 'col-md-4',

    // Debe coincidir con el nombre en el C# (Paso 1)
    optionsKey: 'arrivalRackTypes',

    placeholder: '-- Select Packaging --',

    validation: {
      required: false,
      // Validación dinámica basada en tu legacy:
      // Requerido solo si el estatus es Quotes(1), CarryOver(2), CasiCasi(3) o POH(4)
      requiredWhen: {
        field: '_projectStatusId',
        is: [
          PROJECT_STATUS.QUOTES,
          PROJECT_STATUS.CARRY_OVER,
          PROJECT_STATUS.CASI_CASI,
          PROJECT_STATUS.POH
        ]
      }
    },

    // Visibilidad basada en la Ruta (Oculto en Rewinded, etc.)
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK,
        ROUTES.BLK_RP,
        ROUTES.BLK_RPLTZ,
        ROUTES.BLK_SH,
        ROUTES.BLK_WLD,
        ROUTES.COIL_TO_COIL,
        // ROUTES.REWINDED omitido intencionalmente
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
    }
  },
  {
    name: 'ID_Arrival_Protective_Material',
    label: 'Protective Material',
    type: 'select',
    section: 'Arrival Conditions',
    className: 'col-md-4',

    // Debe coincidir con el nombre en C# (Paso 1)
    optionsKey: 'arrivalProtectiveMaterials',

    placeholder: '-- Select Option --',

    validation: {
      required: true // Siempre requerido si es visible (según tu JS legacy)
    },

    // Misma visibilidad que los otros campos de Arrival
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK,
        ROUTES.BLK_RP,
        ROUTES.BLK_RPLTZ,
        ROUTES.BLK_SH,
        ROUTES.BLK_WLD,
        ROUTES.COIL_TO_COIL,
        // ROUTES.REWINDED omitido
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
    }
  },
  {
    name: 'Arrival_Protective_Material_Other',
    label: 'Specify "Other" Material',
    type: 'text',
    section: 'Arrival Conditions',
    className: 'col-md-4',
    placeholder: 'Specify material...',

    validation: {
      required: true, // Obligatorio siempre que sea visible
      maxLength: 120  // Límite de tu legacy
    },

    // Solo visible si el Dropdown anterior vale 6 (Other)
    visibleWhen: {
      field: 'ID_Arrival_Protective_Material',
      is: [6]
    }
  },
  {
    name: 'Is_By_Container',
    label: 'By Container?',
    type: 'checkbox',
    section: 'Arrival Conditions',
    className: 'col-md-4', // Mantenemos la consistencia de ancho

    // Al no poner 'visibleWhen', se muestra siempre en esta pestaña.
    // Al no poner 'validation', es opcional (true/false).
  },
  {
    name: 'Is_Stackable',
    label: 'Is it stackable?',
    type: 'checkbox',
    section: 'Arrival Conditions',
    className: 'col-md-4'
  },
  {
    name: 'Stackable_Levels',
    label: 'Stackable Levels',
    type: 'number',
    section: 'Arrival Conditions',
    className: 'col-md-4',
    placeholder: 'Levels',

    // Validación: Solo aplica si el campo es visible
    validation: {
      required: true, // Obligatorio si Is_Stackable está marcado
      min: 1          // Debe ser un número no negativo
    },

    // Visibilidad: Solo si Is_Stackable es true
    visibleWhen: {
      field: 'Is_Stackable',
      is: [true]
    }
  },
  {
    name: 'ID_Arrival_Warehouse',
    label: 'Arrival Warehouse',
    type: 'select',
    section: 'Arrival Conditions',
    className: 'col-md-4',
    optionsKey: 'warehouseList', // Coincide con el C#
    placeholder: '-- Select Warehouse --',

    validation: { required: false }, // En tu legacy la validación estaba comentada

    // VISIBILIDAD: Solo si la planta inyectada es 1 (Puebla)
    visibleWhen: {
      field: '_projectPlantId',
      is: [1]
    }
  },
  {
    name: 'PassesThroughSouthWarehouse',
    label: 'Passes Through South Warehouse?',
    type: 'checkbox',
    section: 'Arrival Conditions',
    className: 'col-md-4', // O el tamaño que prefieras

    // VISIBILIDAD: Misma regla, solo para planta 1
    visibleWhen: {
      field: '_projectPlantId',
      is: [1]
    }
  },
  {
    name: 'ID_File_ArrivalAdditional', // Campo que guarda el ID (viene de BD)
    label: 'Arrival Additional File',
    type: 'file', // Nuevo tipo
    section: 'Arrival Conditions',
    className: 'col-md-4',

    // Nombre de la propiedad donde guardaremos el binario temporalmente
    // Esta propiedad debe coincidir con el nombre del parámetro en el Controller C# (Legacy)
    // En tu legacy: arrivalAdditionalFile
    uploadFieldName: 'arrivalAdditionalFile',

    validation: {
      required: false,
      // 👇 AHORA PUEDES CONFIGURAR ESTO A GUSTO:
      accept: ".pdf, .jpg, .png, .zip, .rar",
      maxSizeInMB: 10 // Solo 5 MB para este campo
    },

    // Visibilidad (ejemplo, solo para Warehouse 1)
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK,
        ROUTES.BLK_RP,
        ROUTES.BLK_RPLTZ,
        ROUTES.BLK_SH,
        ROUTES.BLK_WLD,
        ROUTES.COIL_TO_COIL,
        // ROUTES.REWINDED omitido
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
    }
  },
  {
    name: 'Arrival_Comments', // Debe coincidir con la BD y types.ts
    label: 'Comments',
    type: 'textarea', // 👈 Usamos el nuevo tipo
    section: 'Arrival Conditions',
    className: 'col-md-12', // 👈 Ocupa todo el ancho
    placeholder: 'Comments...',

    validation: {
      required: false,
      maxLength: 250 // 👈 Validación automática (Legacy: limit = 250)
    },

    // Misma visibilidad que los otros campos de Arrival (basado en la Ruta)
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK,
        ROUTES.BLK_RP,
        ROUTES.BLK_RPLTZ,
        ROUTES.BLK_SH,
        ROUTES.BLK_WLD,
        ROUTES.COIL_TO_COIL,
        // ROUTES.REWINDED omitido
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
    }
  },
  {
    name: 'Quality', // Debe coincidir con tu modelo C# y types.ts
    label: 'Quality',
    type: 'creatable-select', // 👈 Usamos el nuevo tipo
    section: 'Coil Data',     // Esto lo pondrá en la pestaña Coil Data
    className: 'col-md-3',    // Ancho solicitado en tu legacy
    optionsKey: 'qualityList', // La lista que cargamos en el Paso 1
    placeholder: 'Select or type...',

    validation: {
      required: false, // Por defecto no es requerido
      maxLength: 50,   // Regla legacy: Max 50 chars

      // Regla legacy: Requerido solo para estos estatus
      requiredWhen: {
        field: '_projectStatusId',
        is: [
          PROJECT_STATUS.QUOTES,     // 1
          PROJECT_STATUS.CARRY_OVER, // 2
          PROJECT_STATUS.CASI_CASI,  // 3
          PROJECT_STATUS.POH         // 4
        ]
      }
    },
    // Misma visibilidad que los otros campos de Arrival (basado en la Ruta)
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK,
        ROUTES.BLK_RP,
        ROUTES.BLK_RPLTZ,
        ROUTES.BLK_SH,
        ROUTES.BLK_WLD,
        ROUTES.COIL_TO_COIL,
        ROUTES.REWINDED, //omitido
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
    }
  },
  {
    name: 'ID_Material_type', // Coincide con la BD y types.ts
    label: 'Material Type',
    type: 'select',           // Dropdown estándar
    section: 'Coil Data',     // Pestaña Coil Data
    className: 'col-md-3',    // Ajustado a 3 columnas para mejor diseño (Legacy era 2)

    // Nombre de la lista en tu objeto 'lists' (revisado en tu controlador C#)
    optionsKey: 'materialTypes',

    placeholder: 'Select Material Type',

    validation: {
      required: false, // Por defecto opcional

      // Regla Legacy: Obligatorio para Quotes, CarryOver, CasiCasi, POH
      requiredWhen: {
        field: '_projectStatusId',
        is: [
          PROJECT_STATUS.QUOTES,     // 1
          PROJECT_STATUS.CARRY_OVER, // 2
          PROJECT_STATUS.CASI_CASI,  // 3
          PROJECT_STATUS.POH         // 4
        ]
      }
    }
  },
  {
    name: 'Mill',
    label: 'Mill',
    type: 'creatable-select', // Permite seleccionar o escribir
    section: 'Coil Data',
    className: 'col-md-3',    // Mismo ancho que en Legacy

    optionsKey: 'millList',   // La lista que cargamos en el Paso 1
    placeholder: 'Select or type Mill...',

    validation: {
      required: false, // No era obligatorio en tu legacy
      maxLength: 80    // Regla legacy: "Cannot exceed 80 characters"
    }
  },
  {
    name: 'MaterialSpecification', // Debe coincidir con types.ts
    label: 'Material Specification',
    type: 'text',             // Input de texto normal
    section: 'Coil Data',     // Pestaña Coil Data
    className: 'col-md-3',    // Ancho solicitado (Legacy era col-md-3)
    placeholder: 'Specification',

    validation: {
      required: false, // Legacy: No validaba si estaba vacío, solo longitud
      maxLength: 80    // Regla Legacy: "Cannot exceed 80 characters"
    }
  },
  {
    name: 'Surface',
    label: 'Surface (Calculated)',
    type: 'text',          // Usamos text para que se vea como el input HTML que pasaste
    section: 'Coil Data',  // Ajusta la sección si es necesario
    className: 'col-md-2', // 👈 Ancho solicitado (col-md-2)
    placeholder: 'Auto-calculated',

    disabled: true,        // 👈 ESTO es clave: Lo hace ReadOnly y le pone fondo gris (#e9ecef)
    showInTable: true      // Opcional: Para que salga en la tabla resumen
  },
  // 1. Tensile Strength (Solo)
  {
    name: 'Tensile_Strenght',
    label: 'Tensile Strength',
    type: 'number',
    section: 'Coil Data',
    className: 'col-md-3',
    placeholder: 'Tensile Strength',
    decimals: 2, // Esto generará step="0.01" automáticamente

    validation: {
      required: false, // Default
      min: 0,          // Valida que sea positivo

      // Regla Legacy: Requerido para CarryOver(2), CasiCasi(3), POH(4)
      requiredWhen: {
        field: '_projectStatusId',
        is: [
          PROJECT_STATUS.CARRY_OVER,
          PROJECT_STATUS.CASI_CASI,
          PROJECT_STATUS.POH
        ]
      }
    }
  },
  // --- GRUPO THICKNESS ---
  // Campo 1: Thickness (Lleva el título de la fila)
  {
    name: 'Thickness',
    label: 'Value [mm]', // Etiqueta pequeña sobre el input
    rowTitle: 'Thickness', // 👈 TÍTULO LATERAL AZUL GRANDE
    type: 'number',
    section: 'Coil Data',
    className: 'col-md-12', // Ajustamos anchos
    placeholder: 'Value',
    decimals: 2, // Step 0.01
    validation: {
      required: false, // Default
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [
          PROJECT_STATUS.QUOTES,
          PROJECT_STATUS.CARRY_OVER,
          PROJECT_STATUS.CASI_CASI,
          PROJECT_STATUS.POH
        ]
      }
    }
  },
  // Campo 2: Tol (-)
  {
    name: 'ThicknessToleranceNegative',
    label: 'Tol (-)',
    type: 'number',
    section: 'Coil Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    validation: {
      required: false,
      max: 0, // Debe ser negativo
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  // Campo 3: Tol (+)
  {
    name: 'ThicknessTolerancePositive',
    label: 'Tol (+)',
    type: 'number',
    section: 'Coil Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },

  // --- FILA VISUAL: WIDTH ---
  {
    name: 'Width',
    label: 'Value [mm]',
    rowTitle: 'Width', // ESTO activa el diseño de fila matriz
    type: 'number',
    section: 'Coil Data',
    className: 'col-md-12', // Se ignora visualmente, pero se deja por consistencia
    placeholder: 'Value',
    decimals: 2, // Step 0.01
    validation: {
      required: false,
      min: 0,
      // Reglas de tus estatus Legacy (Quotes, CarryOver, CasiCasi, POH)
      requiredWhen: {
        field: '_projectStatusId',
        is: [
          PROJECT_STATUS.QUOTES,
          PROJECT_STATUS.CARRY_OVER,
          PROJECT_STATUS.CASI_CASI,
          PROJECT_STATUS.POH
        ]
      }
    }
  },

  // 2. Tolerancia Negativa
  {
    name: 'WidthToleranceNegative',
    label: 'Tol (-)',
    type: 'number',
    section: 'Coil Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    validation: {
      required: false,
      max: 0, // Debe ser negativo o 0
      // Requerido solo en CasiCasi y POH
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },

  // 3. Tolerancia Positiva
  {
    name: 'WidthTolerancePositive',
    label: 'Tol (+)',
    type: 'number',
    section: 'Coil Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  // 1. Master Coil Weight
  {
    name: 'MasterCoilWeight',
    label: 'Master Coil Weight',
    type: 'number',
    section: 'Coil Data',
    className: 'col-md-3', // Ajustado a 3 para mejor espacio (Legacy era 2)
    placeholder: 'Weight',
    decimals: 0, // Permite decimales
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CARRY_OVER, PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },

  // 2. Inner Coil Diameter Arrival
  {
    name: 'InnerCoilDiameterArrival',
    label: 'Inner Diameter',
    type: 'number',
    section: 'Coil Data',
    className: 'col-md-3',
    placeholder: 'Inner Dia.',
    decimals: 0,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },

  // 3. Outer Coil Diameter Arrival
  {
    name: 'OuterCoilDiameterArrival',
    label: 'Outer Diameter',
    type: 'number',
    section: 'Coil Data',
    className: 'col-md-3',
    placeholder: 'Outer Dia.',
    decimals: 0,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  {
    name: 'ID_File_CoilDataAdditional',
    label: 'Coil Data Additional File',
    type: 'file',
    section: 'Coil Data',
    className: 'col-md-4', // Ancho Legacy para archivo

    // Nombre del parámetro que espera tu controlador C#
    uploadFieldName: 'coilDataAdditionalFile',

    validation: {
      required: false,
      accept: ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
      maxSizeInMB: 10
    }
  },
  // --- NUEVA SECCIÓN: Slitter Data ---
  {
    name: 'Multipliers', // Debe coincidir con types.ts
    label: 'Mults [Prov. by client - pcs]', // Etiqueta descriptiva
    type: 'number',
    section: 'Slitter Data', // 👈 Esto crea la nueva pestaña lateral
    className: 'col-md-3',   // Ancho solicitado
    placeholder: 'Strips',   // Placeholder del legacy
    //step: 'any',             // Permite decimales si fuera necesario

    validation: {
      required: false, // Ajusta si es obligatorio
      min: 0           // Regla visual básica (no negativos)
    },

    // Opcional: Si este campo solo debe verse para ciertas rutas (ej. Slitter)
    // puedes descomentar y ajustar esto:
    /*
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.SLT, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
    }
    */
  },
  // 1. Inner Coil Diameter Delivery
  {
    name: 'InnerCoilDiameterDelivery',
    label: 'Inner Diameter Delivery',
    type: 'number',
    section: 'Slitter Data',
    className: 'col-md-3',
    placeholder: 'Inner Dia.',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      // Legacy: Requerido en CasiCasi y POH
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },

  // 2. Outer Coil Diameter Delivery
  {
    name: 'OuterCoilDiameterDelivery',
    label: 'Outer Diameter Delivery',
    type: 'number',
    section: 'Slitter Data',
    className: 'col-md-3',
    placeholder: 'Outer Dia.',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },

  // 3. Slitter Estimated Annual Volume
  {
    name: 'SlitterEstimatedAnnualVolume [Tons]',
    label: 'Est. Annual Volume',
    type: 'number',
    section: 'Slitter Data',
    className: 'col-md-3',
    placeholder: 'Volume',
    decimals: 2,
    validation: {
      required: false,
      min: 0
      // Legacy no muestra "required" explícito en el HTML para este campo, 
      // pero puedes agregarlo si es necesario.
    }
  },
  // 1. Campo Principal (Value)
  {
    name: 'Width_Mults',
    label: 'Value [mm]',
    rowTitle: 'Mults Width', // 👈 ESTO activa la fila azul/blanca
    type: 'number',
    section: 'Slitter Data',
    className: 'col-md-12', // Solicitado
    placeholder: 'Value',
    decimals: 2,
    validation: {
      required: false,
      min: 0
    }
  },

  // 2. Tolerancia Negativa
  {
    name: 'Width_Mults_Tol_Neg',
    label: 'Tol (-)',
    type: 'number',
    section: 'Slitter Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    validation: {
      required: false,
      max: 0 // Debe ser negativo o 0
    }
  },

  // 3. Tolerancia Positiva
  {
    name: 'Width_Mults_Tol_Pos',
    label: 'Tol (+)',
    type: 'number',
    section: 'Slitter Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    validation: {
      required: false,
      min: 0
    }
  },
  // 1. Campo Min (Lleva el título de la fila)
  {
    name: 'WeightOfFinalMults_Min',
    label: 'Min [kg]',
    rowTitle: 'Weight of Final Mults', // Título Azul Lateral
    type: 'number',
    section: 'Slitter Data',
    className: 'col-md-12',
    placeholder: 'Min',
    decimals: 2,
    validation: {
      required: false,
      min: 0
    }
  },

  // 2. Campo Optimal (Es el WeightOfFinalMults "a secas" en tu modelo)
  {
    name: 'WeightOfFinalMults',
    label: 'Optimal [kg]',
    type: 'number',
    section: 'Slitter Data',
    className: 'col-md-12',
    placeholder: 'Optimal',
    decimals: 2,
    validation: {
      required: false, // Se valida por estatus en la lógica compleja si es necesario
      min: 0,
      // Estatus donde el Optimal es requerido (Legacy)
      requiredWhen: {
        field: '_projectStatusId',
        is: [
          PROJECT_STATUS.QUOTES,
          PROJECT_STATUS.CARRY_OVER,
          PROJECT_STATUS.CASI_CASI,
          PROJECT_STATUS.POH
        ]
      }
    }
  },

  // 3. Campo Max
  {
    name: 'WeightOfFinalMults_Max',
    label: 'Max [kg]',
    type: 'number',
    section: 'Slitter Data',
    className: 'col-md-12',
    placeholder: 'Max',
    decimals: 2,
    validation: {
      required: false,
      min: 0
    }
  },
  {
    name: 'ID_File_SlitterDataAdditional',
    label: 'Slitter Data Additional File',
    type: 'file',
    section: 'Slitter Data',
    className: 'col-md-6', // Un poco más ancho que col-md-4 para ver mejor el nombre

    // Nombre del parámetro en el controlador C#
    uploadFieldName: 'slitterDataAdditionalFile',

    validation: {
      required: false,
      accept: ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
      maxSizeInMB: 20 // Ajusta el tamaño máximo si es necesario
    }
  },
  // --- SECCIÓN: Blank Data ---
  {
    name: 'ID_Shape',
    label: 'Shape',
    type: 'select',
    section: 'Blank Data', // 👈 Esto crea la nueva pestaña automáticamente
    className: 'col-md-3', // Ancho Legacy
    options: 'shapes',     // ⚠️ Asegúrate que 'lists.shapes' exista en tu App.tsx
    placeholder: 'Select Shape',
    validation: {
      required: false,
      // Validación Legacy: Requerido en Quotes, CarryOver, CasiCasi, POH
      requiredWhen: {
        field: '_projectStatusId',
        is: [
          PROJECT_STATUS.QUOTES,
          PROJECT_STATUS.CARRY_OVER,
          PROJECT_STATUS.CASI_CASI,
          PROJECT_STATUS.POH
        ]
      }
    }
  },
  // 1. Blanks Per Stroke
  {
    name: 'Blanks_Per_Stroke',
    label: 'Blanks Per Stroke',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-3',
    placeholder: 'Blanks/Stroke',
    decimals: 2, // Permite decimales si es necesario (step="any" en legacy)
    validation: {
      required: false,
      min: 0,
      // Requerido en CarryOver, CasiCasi, POH (Legacy)
      requiredWhen: {
        field: '_projectStatusId',
        is: [
          PROJECT_STATUS.CARRY_OVER,
          PROJECT_STATUS.CASI_CASI,
          PROJECT_STATUS.POH
        ]
      }
    }
    // Nota: Los cálculos (updates) se manejarán automáticamente en React
    // gracias a que el estado es reactivo. Si necesitas un cálculo específico,
    // lo agregaremos en un useEffect en MaterialForm.
  },

  // 2. Parts Per Vehicle
  {
    name: 'Parts_Per_Vehicle',
    label: 'Parts Per Vehicle',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-3',
    placeholder: 'Parts/Vehicle',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      // Requerido en CarryOver, CasiCasi, POH (Legacy)
      requiredWhen: {
        field: '_projectStatusId',
        is: [
          PROJECT_STATUS.CARRY_OVER,
          PROJECT_STATUS.CASI_CASI,
          PROJECT_STATUS.POH
        ]
      }
    }
  },
  // --- GRUPO 1: PLATES WIDTH ---
  {
    name: 'Width_Plates',
    label: 'Value [mm]',
    rowTitle: 'Plates Width', // 👈 Título Azul
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Value',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      // Validación Lógica Cruzada (Tol vs Width) se hará en MaterialForm
    }
  },
  {
    name: 'Width_Plates_Tol_Neg',
    label: 'Tol (-)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    validation: {
      required: false,
      max: 0 // Debe ser negativo o 0
    }
  },
  {
    name: 'Width_Plates_Tol_Pos',
    label: 'Tol (+)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    validation: {
      required: false,
      min: 0
    }
  },

  // --- GRUPO 2: PITCH ---
  {
    name: 'Pitch',
    label: 'Value [mm]',
    rowTitle: 'Pitch', // 👈 Título Azul
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Value',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.QUOTES, PROJECT_STATUS.CARRY_OVER, PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  {
    name: 'PitchToleranceNegative',
    label: 'Tol (-)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    validation: {
      required: false,
      max: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  {
    name: 'PitchTolerancePositive',
    label: 'Tol (+)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },

  // --- GRUPO 3: FLATNESS ---
  {
    name: 'Flatness',
    label: 'Value [mm]',
    rowTitle: 'Flatness', // 👈 Título Azul
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Value',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  {
    name: 'FlatnessToleranceNegative',
    label: 'Tol (-)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    validation: {
      required: false,
      max: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  {
    name: 'FlatnessTolerancePositive',
    label: 'Tol (+)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  // --- GRUPO 1: ANGLE A ---
  {
    name: 'Angle_A',
    label: 'Angle A',
    rowTitle: 'Angle A', // 👈 Título Azul
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Value',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.QUOTES, PROJECT_STATUS.CARRY_OVER, PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  {
    name: 'AngleAToleranceNegative',
    label: 'Angle A Tol (-)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    validation: {
      required: false,
      max: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  {
    name: 'AngleATolerancePositive',
    label: 'Angle A Tol (+)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },

  // --- GRUPO 2: ANGLE B ---
  {
    name: 'Angle_B',
    label: 'Angle B',
    rowTitle: 'Angle B', // 👈 Título Azul
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Value',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.QUOTES, PROJECT_STATUS.CARRY_OVER, PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  {
    name: 'AngleBToleranceNegative',
    label: 'Angle B Tol (-)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    validation: {
      required: false,
      max: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  {
    name: 'AngleBTolerancePositive',
    label: 'Angle B Tol (+)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },

  // --- GRUPO 3: MAJOR BASE ---
  {
    name: 'MajorBase',
    label: 'Major Base',
    rowTitle: 'Major Base', // 👈 Título Azul
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Value',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.QUOTES, PROJECT_STATUS.CARRY_OVER, PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  {
    name: 'MajorBaseToleranceNegative',
    label: 'Major Base Tol (-)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    validation: {
      required: false,
      max: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  {
    name: 'MajorBaseTolerancePositive',
    label: 'Major Base Tol (+)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },

  // --- GRUPO 4: MINOR BASE ---
  {
    name: 'MinorBase',
    label: 'Minor Base',
    rowTitle: 'Minor Base', // 👈 Título Azul
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Value',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.QUOTES, PROJECT_STATUS.CARRY_OVER, PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  {
    name: 'MinorBaseToleranceNegative',
    label: 'Minor Base Tol (-)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    validation: {
      required: false,
      max: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  {
    name: 'MinorBaseTolerancePositive',
    label: 'Minor Base Tol (+)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },
  // 1. Theoretical Gross Weight (Calculado)
  {
    name: 'Theoretical_Gross_Weight',
    label: 'Theoretical Gross Weight',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-4', // 1/3 de ancho
    placeholder: 'Auto-calculated',
    decimals: 3, // 3 decimales según tu legacy
    disabled: true, // 👈 El usuario no lo edita
    validation: {
      required: false,
      min: 0
    }
  },

  // 2. Gross Weight (Manual)
  {
    name: 'Gross_Weight',
    label: 'Gross Weight',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-4',
    placeholder: 'Gross Weight',
    decimals: 3,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.QUOTES, PROJECT_STATUS.CARRY_OVER, PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    }
  },

  // 3. Client Net Weight (Manual)
  {
    name: 'ClientNetWeight',
    label: 'Client Net Weight',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-4',
    placeholder: 'Client Net Weight',
    decimals: 3,
    validation: {
      required: false,
      min: 0
    }
  },
  // 1. TurnOver (Checkbox)
  {
    name: 'TurnOver',
    label: 'Turn Over?',
    type: 'checkbox',
    section: 'Blank Data',
    className: 'col-md-3', // Suficiente espacio para el label
    // Visible SOLO si Shape es 18 (Configured)
    visibleWhen: {
      field: 'ID_Shape',
      is: [18] // Valor numérico del ID 18
    },
    validation: {
      required: false
    }
  },

  // 2. TurnOverSide (Select)
  {
    name: 'TurnOverSide',
    label: 'Side',
    type: 'select',
    section: 'Blank Data',
    className: 'col-md-3',
    // Opciones hardcodeadas (Left/Right)
    options: [
      { value: 'Left', label: 'Left' },
      { value: 'Right', label: 'Right' }
    ],
    placeholder: 'Select an option',
    // Visible SOLO si Shape es 18 Y TurnOver está marcado
    // NOTA: La lógica 'visibleWhen' simple solo soporta una dependencia.
    // Manejaremos la visibilidad combinada (Shape + Checkbox) en MaterialForm.tsx 
    // o usaremos un truco de dependencia en cadena.
    // Por ahora, lo hacemos depender de TurnOver, y como TurnOver depende de Shape, funciona en cadena.
    visibleWhen: {
      field: 'TurnOver',
      is: [true]
    },
    validation: {
      // Requerido si es visible (es decir, si TurnOver es true)
      required: true,
      customMessage: "Side is required when 'Turn Over' is checked."
    }
  },
  // 1. Requires Die Manufacturing? (Checkbox independiente)
  {
    name: 'RequiresDieManufacturing',
    label: 'Requires Die Manufacturing?',
    type: 'checkbox',
    section: 'Blank Data',
    className: 'col-md-3',
    validation: { required: false }
  },
  // 2. Is Welded Blank? (Checkbox)
  {
    name: 'IsWeldedBlank',
    label: 'Is Welded Blank?',
    type: 'checkbox',
    section: 'Blank Data',
    className: 'col-md-3',
    validation: { required: false }
    // En MaterialForm.tsx manejaremos la lógica de limpiar/mostrar hijos
  },
  // 3. Number of Blanks (Input Numérico)
  {
    name: 'NumberOfPlates',
    label: 'Number of Blanks',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-3',
    placeholder: 'Enter number',
    visibleWhen: {
      field: 'IsWeldedBlank',
      is: [true]
    },
    validation: {
      required: true, // Si es visible, es requerido
      min: 2,
      max: 5,
      customMessage: "Must be a whole number between 2 and 5."
    }
  },
  // 1. CAD Drawings (Visible solo si Shape = 18 'Configured')
  {
    name: 'ID_File_CAD_Drawing',
    label: 'CAD Drawings (C)',
    type: 'file',
    section: 'Blank Data',
    className: 'col-md-4',
    uploadFieldName: 'archivo', // 👈 Nombre exacto del input en tu Legacy
    visibleWhen: {
      field: 'ID_Shape',
      is: [18] // Valor numérico para 'Configured'
    },
    validation: {
      required: false,
      accept: ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
      maxSizeInMB: 20
    }
  },

  // 2. Technical Sheet (Visible para rutas de Blanking)
  {
    name: 'ID_File_TechnicalSheet',
    label: 'Technical Sheet',
    type: 'file',
    section: 'Blank Data',
    className: 'col-md-4',
    uploadFieldName: 'technicalSheetFile', // 👈 Nombre exacto del input en tu Legacy
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK,
        ROUTES.BLK_RP,
        ROUTES.BLK_RPLTZ,
        ROUTES.BLK_SH,
        ROUTES.BLK_WLD,
        // ROUTES.COIL_TO_COIL,  <-- Generalmente NO aplica a Coil to Coil
        // ROUTES.REWINDED,      <-- Tampoco a Rewinded
        // ROUTES.SLT,           <-- Tampoco a Slitter puro
        ROUTES.SLT_BLK,       // <-- Sí aplica porque tiene BLK
        ROUTES.SLT_BLK_WLD    // <-- Sí aplica porque tiene BLK
        // Verifica si WAREHOUSING o WEIGHT_DIVISION aplican según tu negocio
      ]
    },
    validation: {
      required: false,
      accept: ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
      maxSizeInMB: 20
    }
  },

  // 3. Additional File (Misma lógica de visibilidad que Technical Sheet)
  {
    name: 'ID_File_Additional',
    label: 'Additional File',
    type: 'file',
    section: 'Blank Data',
    className: 'col-md-4',
    uploadFieldName: 'AdditionalFile', // 👈 Nombre exacto del input en tu Legacy
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK,
        ROUTES.BLK_RP,
        ROUTES.BLK_RPLTZ,
        ROUTES.BLK_SH,
        ROUTES.BLK_WLD,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD
      ]
    },
    validation: {
      required: false,
      accept: ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
      maxSizeInMB: 20
    }
  },
// 1. Blanking Annual Volume
  {
    name: 'Blanking_Annual_Volume',
    label: 'Blanking Annual Volume',
    type: 'number',
    section: 'Blanking Specific Volumes', // Nueva pestaña
    className: 'col-md-4', // 3 columnas por fila para mejor diseño
    placeholder: 'Volume',
    decimals: 0, // Entero
    validation: {
        required: false, // Legacy dice opcional (a menos que cambies lógica)
        min: 0,
        customMessage: "Must be a positive whole number."
    }
  },

  // 2. Blanking Volume Per Year
  {
    name: 'Blanking_Volume_Per_year',
    label: 'Blanking Volume / Year',
    type: 'number',
    section: 'Blanking Specific Volumes',
    className: 'col-md-4',
    placeholder: 'M Tons/Year',
    decimals: 3, // step="any"
    validation: {
        required: false,
        min: 0
    }
  },

  // 3. Blanking Initial Weight Per Part (Calculado)
  {
    name: 'Blanking_InitialWeightPerPart',
    label: 'Blanking Initial Weight/Part',
    type: 'number',
    section: 'Blanking Specific Volumes',
    className: 'col-md-4',
    placeholder: 'Calculated',
    decimals: 3,
    disabled: true // Readonly
  },

  // 4. Initial Weight Per Part (Calculado)
  {
    name: 'InitialWeightPerPart',
    label: 'Initial Weight/Part',
    type: 'number', // O text, según prefieras visualmente
    section: 'Blanking Specific Volumes',
    className: 'col-md-4',
    placeholder: 'Calculated',
    decimals: 3,
    disabled: true
  },

  // 5. Blanking Process Tons (Calculado)
  {
    name: 'Blanking_ProcessTons',
    label: 'Blanking Process Tons',
    type: 'number',
    section: 'Blanking Specific Volumes',
    className: 'col-md-4',
    placeholder: 'Calculated',
    decimals: 3,
    disabled: true
  },

  // 6. Blanking Shipping Tons (Calculado)
  {
    name: 'Blanking_ShippingTons',
    label: 'Blanking Shipping Tons',
    type: 'number',
    section: 'Blanking Specific Volumes',
    className: 'col-md-4',
    placeholder: 'Calculated',
    decimals: 3,
    disabled: true
  },
  // 1. Shearing Pieces Per Stroke
  {
    name: 'Shearing_Pieces_Per_Stroke',
    label: 'Pieces / Stroke',
    type: 'number',
    section: 'Shearing Data', // Nueva Pestaña
    className: 'col-md-3',
    placeholder: 'Pieces / Stroke',
    decimals: 2, // step="any"
    validation: {
        required: false,
        min: 0
    }
  },

  // 2. Shearing Pieces Per Car
  {
    name: 'Shearing_Pieces_Per_Car',
    label: 'Pieces / Car',
    type: 'number',
    section: 'Shearing Data',
    className: 'col-md-3',
    placeholder: 'Pieces / Car',
    decimals: 2,
    validation: {
        required: false,
        min: 0
    }
  },

  // --- GRUPO MATRIZ: Shearing Width ---
  
  // A) Campo Principal (Value)
  {
    name: 'Shearing_Width',
    label: 'Value [mm]',
    rowTitle: 'Shearing Width', // 👈 Título Azul Lateral
    type: 'number',
    section: 'Shearing Data',
    className: 'col-md-12', // Se ignora visualmente en modo matriz
    placeholder: 'Width',
    decimals: 2,
    validation: {
        required: false,
        min: 0
    }
  },

  // B) Tolerancia Negativa
  {
    name: 'Shearing_Width_Tol_Neg',
    label: 'Tol (-)',
    type: 'number',
    section: 'Shearing Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    validation: {
        required: false,
        max: 0 // Debe ser negativo o 0
    }
  },

  // C) Tolerancia Positiva
  {
    name: 'Shearing_Width_Tol_Pos',
    label: 'Tol (+)',
    type: 'number',
    section: 'Shearing Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    validation: {
        required: false,
        min: 0
    }
  },
  // A) Campo Principal (Value)
  {
    name: 'Shearing_Pitch',
    label: 'Value [mm]',
    rowTitle: 'Shearing Pitch', // 👈 Título Azul Lateral
    type: 'number',
    section: 'Shearing Data',
    className: 'col-md-12', // Se ignora visualmente en modo matriz
    placeholder: 'Pitch',
    decimals: 2,
    validation: {
        required: false,
        min: 0
    }
  },

  // B) Tolerancia Negativa
  {
    name: 'Shearing_Pitch_Tol_Neg',
    label: 'Tol (-)',
    type: 'number',
    section: 'Shearing Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    validation: {
        required: false,
        max: 0 // Debe ser negativo o 0
    }
  },

  // C) Tolerancia Positiva
  {
    name: 'Shearing_Pitch_Tol_Pos',
    label: 'Tol (+)',
    type: 'number',
    section: 'Shearing Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    validation: {
        required: false,
        min: 0
    }
  },

  // --- GRUPO MATRIZ: Shearing Weight ---
  
  // A) Campo Principal (Value)
  {
    name: 'Shearing_Weight',
    label: 'Value [kg]', // Nota: Unidad es kg
    rowTitle: 'Shearing Weight', // 👈 Título Azul Lateral
    type: 'number',
    section: 'Shearing Data',
    className: 'col-md-12', 
    placeholder: 'Weight',
    decimals: 2,
    validation: {
        required: false,
        min: 0
    }
  },

  // B) Tolerancia Negativa
  {
    name: 'Shearing_Weight_Tol_Neg',
    label: 'Tol (-)',
    type: 'number',
    section: 'Shearing Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    validation: {
        required: false,
        max: 0 
    }
  },

  // C) Tolerancia Positiva
  {
    name: 'Shearing_Weight_Tol_Pos',
    label: 'Tol (+)',
    type: 'number',
    section: 'Shearing Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    validation: {
        required: false,
        min: 0
    }
  },

];