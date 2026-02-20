// config/material-fields
import type { FieldConfig } from '../types/form-schema';
import { ROUTES, PROJECT_STATUS } from '../constants'; // Ajusta la ruta si constants.ts está en otra carpeta

const coilDiameterOptions = [
  { value: 508, label: '508' },
  { value: 610, label: '610' }
];

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
  /********** ARRIVAL CONDITION ************* */
  {
    name: 'ID_Coil_Position',
    label: 'Coil Position',
    type: 'select',
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
  {
    name: 'Arrival_Transport_Type_Other', // Debe coincidir con types.ts
    label: 'Specify Other Transport',
    type: 'text',
    section: 'Arrival Conditions',
    className: 'col-md-4',
    placeholder: 'Specify details...',
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
      maxLength: 120  // Límite 
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
    // Al no poner 'validation', es opcional (true/false).
  },
  {
    name: 'Is_Stackable',
    label: 'Is it stackable?',
    type: 'checkbox',
    section: 'Arrival Conditions',
    className: 'col-md-4',
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
    visibleWhen: [
      // Condición 1: Solo planta Puebla (ID 1)
      {
        field: '_projectPlantId',
        is: [1]
      },
      // Condición 2: Solo ciertas rutas
      {
        field: 'ID_Route',
        is: [
          ROUTES.BLK,
          ROUTES.BLK_RP,
          ROUTES.BLK_RPLTZ,
          ROUTES.BLK_SH,
          ROUTES.BLK_WLD,
          ROUTES.COIL_TO_COIL,
          ROUTES.SLT,
          ROUTES.SLT_BLK,
          ROUTES.SLT_BLK_WLD,
          ROUTES.WAREHOUSING,
          ROUTES.WAREHOUSING_RP,
          ROUTES.WEIGHT_DIVISION
        ]
      }
    ]
  },
  {
    name: 'PassesThroughSouthWarehouse',
    label: 'Passes Through South Warehouse?',
    type: 'checkbox',
    section: 'Arrival Conditions',
    className: 'col-md-4', // O el tamaño que prefieras
    // VISIBILIDAD: Misma regla, solo para planta 1
    visibleWhen: [
      // Condición 1: Solo planta Puebla (ID 1)
      {
        field: '_projectPlantId',
        is: [1]
      },
      // Condición 2: Solo ciertas rutas
      {
        field: 'ID_Route',
        is: [
          ROUTES.BLK,
          ROUTES.BLK_RP,
          ROUTES.BLK_RPLTZ,
          ROUTES.BLK_SH,
          ROUTES.BLK_WLD,
          ROUTES.COIL_TO_COIL,
          ROUTES.SLT,
          ROUTES.SLT_BLK,
          ROUTES.SLT_BLK_WLD,
          ROUTES.WAREHOUSING,
          ROUTES.WAREHOUSING_RP,
          ROUTES.WEIGHT_DIVISION
        ]
      }
    ]
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
    fileNameProp: 'arrivalAdditionalFileName',
    fileEntityProp: 'CTZ_Files6',
    validation: {
      required: false,
      //AHORA PUEDES CONFIGURAR ESTO A GUSTO:
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
    type: 'textarea', //  Usamos el nuevo tipo
    section: 'Arrival Conditions',
    className: 'col-md-12', //  Ocupa todo el ancho
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
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK,
        ROUTES.BLK_RP,
        ROUTES.BLK_RPLTZ,
        ROUTES.BLK_SH,
        ROUTES.BLK_WLD,
        ROUTES.COIL_TO_COIL,
        ROUTES.REWINDED,
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
        ROUTES.REWINDED,
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
        ROUTES.REWINDED,
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
    name: 'MaterialSpecification', // Debe coincidir con types.ts
    label: 'Material Specification',
    type: 'text',             // Input de texto normal
    section: 'Coil Data',     // Pestaña Coil Data
    className: 'col-md-3',    // Ancho solicitado (Legacy era col-md-3)
    placeholder: 'Specification',
    validation: {
      required: false, // Legacy: No validaba si estaba vacío, solo longitud
      maxLength: 80    // Regla Legacy: "Cannot exceed 80 characters"
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
        ROUTES.REWINDED,
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
    name: 'Surface',
    label: 'Surface (Calculated)',
    type: 'text',          // Usamos text para que se vea como el input HTML que pasaste
    section: 'Coil Data',  // Ajusta la sección si es necesario
    className: 'col-md-2', // 👈 Ancho solicitado (col-md-2)
    placeholder: 'Auto-calculated',
    disabled: true,        // 👈 ESTO es clave: Lo hace ReadOnly y le pone fondo gris (#e9ecef)
    showInTable: true,      // Opcional: Para que salga en la tabla resumen
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK,
        ROUTES.BLK_RP,
        ROUTES.BLK_RPLTZ,
        ROUTES.BLK_SH,
        ROUTES.BLK_WLD,
        ROUTES.COIL_TO_COIL,
        ROUTES.REWINDED,
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
    }
  },
  // 1. Tensile Strength (Solo)
  {
    name: 'Tensile_Strenght',
    label: 'Tensile Strength [N/mm²]',
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
        ROUTES.REWINDED,
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
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
        ROUTES.REWINDED,
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
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
    allowZero: true,
    validation: {
      required: false,
      max: 0, // Debe ser negativo
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
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
        ROUTES.REWINDED,
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
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
    allowZero: true,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
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
        ROUTES.REWINDED,
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
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
        ROUTES.REWINDED,
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
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
    allowZero: true,
    validation: {
      required: false,
      max: 0, // Debe ser negativo o 0
      // Requerido solo en CasiCasi y POH
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
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
        ROUTES.REWINDED,
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
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
    allowZero: true,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
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
        ROUTES.REWINDED,
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
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
        ROUTES.REWINDED,
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
    }
  },
  // 2. Inner Coil Diameter Arrival
  {
    name: 'InnerCoilDiameterArrival',
    label: 'Inner Diameter',
    type: 'creatable-select',
    section: 'Coil Data',
    className: 'col-md-3',
    placeholder: 'Inner Dia.',
    options: coilDiameterOptions,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
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
        ROUTES.REWINDED,
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
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
        ROUTES.REWINDED,
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
    name: 'ID_File_CoilDataAdditional',
    label: 'Coil Data Additional File',
    type: 'file',
    section: 'Coil Data',
    className: 'col-md-4', // Ancho Legacy para archivo
    // Nombre del parámetro que espera tu controlador C# Metodo POST
    uploadFieldName: 'coilDataAdditionalFile',
    fileNameProp: 'coilDataAdditionalFileName',
    fileEntityProp: 'CTZ_Files7',
    validation: {
      required: false,
      accept: ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
      maxSizeInMB: 10
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
        ROUTES.REWINDED,
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP,
        ROUTES.WEIGHT_DIVISION
      ]
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
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.REWINDED, ROUTES.SLT, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD, ROUTES.WEIGHT_DIVISION]
    }
  },
  // 1. Inner Coil Diameter Delivery
  {
    name: 'InnerCoilDiameterDelivery',
    label: 'Inner Diameter Delivery',
    type: 'creatable-select',
    options: coilDiameterOptions,
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.REWINDED, ROUTES.SLT, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD, ROUTES.WEIGHT_DIVISION]
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
    decimals: 0,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.REWINDED, ROUTES.SLT, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD, ROUTES.WEIGHT_DIVISION]
    }
  },
  // 3. Slitter Estimated Annual Volume
  {
    name: 'SlitterEstimatedAnnualVolume',
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.REWINDED, ROUTES.SLT, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD, ROUTES.WEIGHT_DIVISION]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.REWINDED, ROUTES.SLT, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD, ROUTES.WEIGHT_DIVISION]
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
    allowZero: true,
    validation: {
      required: false,
      max: 0 // Debe ser negativo o 0
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.REWINDED, ROUTES.SLT, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD, ROUTES.WEIGHT_DIVISION]
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
    allowZero: true,
    validation: {
      required: false,
      min: 0
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.REWINDED, ROUTES.SLT, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD, ROUTES.WEIGHT_DIVISION]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.REWINDED, ROUTES.SLT, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD, ROUTES.WEIGHT_DIVISION]
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
      required: false,
      min: 0
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.REWINDED, ROUTES.SLT, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD, ROUTES.WEIGHT_DIVISION]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.REWINDED, ROUTES.SLT, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD, ROUTES.WEIGHT_DIVISION]
    }
  },
  {
    name: 'ID_File_SlitterDataAdditional',
    label: 'Slitter Data Additional File',
    type: 'file',
    section: 'Slitter Data',
    className: 'col-md-6', // Un poco más ancho que col-md-4 para ver mejor el nombre
    // Nombre del parámetro en el controlador C#
    uploadFieldName: 'SlitterDataAdditionalFile',
    fileNameProp: 'FileName_SlitterDataAdditional',
    fileEntityProp: 'CTZ_Files8',
    validation: {
      required: false,
      accept: ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
      maxSizeInMB: 20 // Ajusta el tamaño máximo si es necesario
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.REWINDED, ROUTES.SLT, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD, ROUTES.WEIGHT_DIVISION]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
    }
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
    }
  },
  // --- GRUPO 1: PLATES WIDTH ---
  {
    name: 'Width_Plates',
    label: 'Value [mm]',
    rowTitle: 'Blank Width', // 👈 Título Azul
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Value',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      // Validación Lógica Cruzada (Tol vs Width) se hará en MaterialForm
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
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
    allowZero: true,
    validation: {
      required: false,
      max: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
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
    allowZero: true,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
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
    allowZero: true,
    validation: {
      required: false,
      max: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
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
    allowZero: true,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
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
    },
    visibleWhen: [
      // Regla 1: Debe ser una de estas Rutas
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      // Regla 2: ADEMÁS, el Shape debe ser Trapezoide (ID 3)
      {
        field: 'ID_Shape',
        is: [3]
      }
    ]
  },
  {
    name: 'AngleAToleranceNegative',
    label: 'Angle A Tol (-)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    allowZero: true,
    validation: {
      required: false,
      max: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    },
    visibleWhen: [
      // Regla 1: Debe ser una de estas Rutas
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      // Regla 2: ADEMÁS, el Shape debe ser Trapezoide (ID 3)
      {
        field: 'ID_Shape',
        is: [3]
      }
    ]
  },
  {
    name: 'AngleATolerancePositive',
    label: 'Angle A Tol (+)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    allowZero: true,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    },
    visibleWhen: [
      // Regla 1: Debe ser una de estas Rutas
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      // Regla 2: ADEMÁS, el Shape debe ser Trapezoide (ID 3)
      {
        field: 'ID_Shape',
        is: [3]
      }
    ]
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
    },
    visibleWhen: [
      // Regla 1: Debe ser una de estas Rutas
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      // Regla 2: ADEMÁS, el Shape debe ser Trapezoide (ID 3)
      {
        field: 'ID_Shape',
        is: [3]
      }
    ]
  },
  {
    name: 'AngleBToleranceNegative',
    label: 'Angle B Tol (-)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    allowZero: true,
    validation: {
      required: false,
      max: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    },
    visibleWhen: [
      // Regla 1: Debe ser una de estas Rutas
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      // Regla 2: ADEMÁS, el Shape debe ser Trapezoide (ID 3)
      {
        field: 'ID_Shape',
        is: [3]
      }
    ]
  },
  {
    name: 'AngleBTolerancePositive',
    label: 'Angle B Tol (+)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    allowZero: true,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    },
    visibleWhen: [
      // Regla 1: Debe ser una de estas Rutas
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      // Regla 2: ADEMÁS, el Shape debe ser Trapezoide (ID 3)
      {
        field: 'ID_Shape',
        is: [3]
      }
    ]
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
    },
    visibleWhen: [
      // Regla 1: Debe ser una de estas Rutas
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      // Regla 2: ADEMÁS, el Shape debe ser Trapezoide (ID 3)
      {
        field: 'ID_Shape',
        is: [3]
      }
    ]
  },
  {
    name: 'MajorBaseToleranceNegative',
    label: 'Major Base Tol (-)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    allowZero: true,
    validation: {
      required: false,
      max: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    },
    visibleWhen: [
      // Regla 1: Debe ser una de estas Rutas
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      // Regla 2: ADEMÁS, el Shape debe ser Trapezoide (ID 3)
      {
        field: 'ID_Shape',
        is: [3]
      }
    ]
  },
  {
    name: 'MajorBaseTolerancePositive',
    label: 'Major Base Tol (+)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    allowZero: true,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    },
    visibleWhen: [
      // Regla 1: Debe ser una de estas Rutas
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      // Regla 2: ADEMÁS, el Shape debe ser Trapezoide (ID 3)
      {
        field: 'ID_Shape',
        is: [3]
      }
    ]
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
    },
    visibleWhen: [
      // Regla 1: Debe ser una de estas Rutas
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      // Regla 2: ADEMÁS, el Shape debe ser Trapezoide (ID 3)
      {
        field: 'ID_Shape',
        is: [3]
      }
    ]
  },
  {
    name: 'MinorBaseToleranceNegative',
    label: 'Minor Base Tol (-)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (-)',
    decimals: 2,
    allowZero: true,
    validation: {
      required: false,
      max: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    },
    visibleWhen: [
      // Regla 1: Debe ser una de estas Rutas
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      // Regla 2: ADEMÁS, el Shape debe ser Trapezoide (ID 3)
      {
        field: 'ID_Shape',
        is: [3]
      }
    ]
  },
  {
    name: 'MinorBaseTolerancePositive',
    label: 'Minor Base Tol (+)',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-12',
    placeholder: 'Tol (+)',
    decimals: 2,
    allowZero: true,
    validation: {
      required: false,
      min: 0,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      }
    },
    visibleWhen: [
      // Regla 1: Debe ser una de estas Rutas
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      // Regla 2: ADEMÁS, el Shape debe ser Trapezoide (ID 3)
      {
        field: 'ID_Shape',
        is: [3]
      }
    ]
  },
  //---------
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
    }
  },
  // 1. TurnOver (Checkbox)
  {
    name: 'TurnOver',
    label: 'Turn Over?',
    type: 'checkbox',
    section: 'Blank Data',
    className: 'col-md-3', // Suficiente espacio para el label
    visibleWhen: [
      {
        field: 'ID_Shape',
        is: [18] // Valor numérico del ID 18
      },
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      }
    ]
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
    visibleWhen: [
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      {
        field: 'TurnOver',
        is: [true]
      }
    ],
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
    validation: { required: false },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
    }
  },
  // 2. Is Welded Blank? (Checkbox)
  {
    name: 'IsWeldedBlank',
    label: 'Is Welded Blank?',
    type: 'checkbox',
    section: 'Blank Data',
    className: 'col-md-3',
    validation: { required: false },
    visibleWhen: {
      field: 'ID_Route',
      is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
    }
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
    visibleWhen: [
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      {
        field: 'IsWeldedBlank',
        is: [true]
      },
    ],
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
    fileNameProp: 'CADFileName',
    fileEntityProp: 'CTZ_Files',
    visibleWhen: [
      {
        field: 'ID_Route',
        is: [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD]
      },
      {
        field: 'ID_Shape',
        is: [18] // Valor numérico para 'Configured'
      }
    ],
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
    fileNameProp: 'technicalSheetFileName',
    fileEntityProp: 'CTZ_Files4',
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK,
        ROUTES.BLK_RP,
        ROUTES.BLK_RPLTZ,
        ROUTES.BLK_SH,
        ROUTES.BLK_WLD,
        ROUTES.SLT_BLK,       // <-- Sí aplica porque tiene BLK
        ROUTES.SLT_BLK_WLD    // <-- Sí aplica porque tiene BLK

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
    fileNameProp: 'AdditionalFileName',
    fileEntityProp: 'CTZ_Files5',
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
  // ---- Seccion volumnes de blanking
  // 1. Blanking Annual Volume
  {
    name: 'Blanking_Annual_Volume',
    label: 'Annual Volume [Vehicles]',
    type: 'number',
    section: 'Blank Data', // Nueva pestaña
    className: 'col-md-4', // 3 columnas por fila para mejor diseño
    placeholder: 'Volume',
    decimals: 0, // Entero
    validation: {
      required: false, // Legacy dice opcional (a menos que cambies lógica)
      min: 0,
      customMessage: "Must be a positive whole number."
    },
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
  },

  // 2. Blanking Volume Per Year
  {
    name: 'Blanking_Volume_Per_year',
    label: 'Annual Volume [m tons/year]',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-4',
    placeholder: 'M Tons/Year',
    decimals: 3, // step="any"
    validation: {
      required: false,
      min: 0
    },
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
  },

  // 3. Blanking Initial Weight Per Part (Calculado)
  {
    name: 'Blanking_InitialWeightPerPart',
    label: 'Initial Weight per Part [kg]',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-4',
    placeholder: 'Calculated',
    decimals: 3,
    disabled: true, // Readonly
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
  },

  // 4. Initial Weight Per Part (Calculado)
  {
    name: 'InitialWeightPerPart',
    label: 'Initial Weight per Part (Adjusted) [kg]',
    type: 'number', // O text, según prefieras visualmente
    section: 'Blank Data',
    className: 'col-md-4',
    placeholder: 'Calculated',
    decimals: 3,
    disabled: true,
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
  },

  // 5. Blanking Process Tons (Calculado)
  {
    name: 'Blanking_ProcessTons',
    label: 'Blanking Process Tons',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-4',
    placeholder: 'Calculated',
    decimals: 3,
    disabled: true,
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
  },

  // 6. Blanking Shipping Tons (Calculado)
  {
    name: 'Blanking_ShippingTons',
    label: 'Blanking Shipping Tons',
    type: 'number',
    section: 'Blank Data',
    className: 'col-md-4',
    placeholder: 'Calculated',
    decimals: 3,
    disabled: true,
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
  },
  /*****  SHEARING DATA ******/

  // 1. Shearing Pieces Per Stroke
  {
    name: 'Shearing_Pieces_Per_Stroke',
    label: 'Shearing Pieces / Stroke',
    type: 'number',
    section: 'Shearing Data', // Nueva Pestaña
    className: 'col-md-3',
    placeholder: 'Pieces / Stroke',
    decimals: 2, // step="any"
    validation: {
      required: false,
      min: 0
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK_SH,
      ]
    }
  },

  // 2. Shearing Pieces Per Car
  {
    name: 'Shearing_Pieces_Per_Car',
    label: 'Shearing Pieces / Car',
    type: 'number',
    section: 'Shearing Data',
    className: 'col-md-3',
    placeholder: 'Pieces / Car',
    decimals: 2,
    validation: {
      required: false,
      min: 0
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK_SH,
      ]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK_SH,
      ]
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
    allowZero: true,
    validation: {
      required: false,
      max: 0 // Debe ser negativo o 0
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK_SH,
      ]
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
    allowZero: true,
    validation: {
      required: false,
      min: 0
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK_SH,
      ]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK_SH,
      ]
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
    allowZero: true,
    validation: {
      required: false,
      max: 0 // Debe ser negativo o 0
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK_SH,
      ]
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
    allowZero: true,
    validation: {
      required: false,
      min: 0
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK_SH,
      ]
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
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK_SH,
      ]
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
    allowZero: true,
    validation: {
      required: false,
      max: 0
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK_SH,
      ]
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
    allowZero: true,
    validation: {
      required: false,
      min: 0
    },
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.BLK_SH,
      ]
    }
  },
  // 1. Interplant Coil Position (Reusa la lista existente)
  {
    name: 'ID_InterplantDelivery_Coil_Position',
    label: 'Interplant Coil Position',
    type: 'select',
    section: 'Interplant Delivery Packaging', // Nueva pestaña
    className: 'col-md-3',
    optionsKey: 'coilPositions', // 👈 REUTILIZADO
    placeholder: 'Select an option',
    validation: {
      required: false
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
  // 2. Packaging Standard (Dropdown estático)
  {
    name: 'InterplantPackagingStandard',
    label: 'Packaging Standard',
    type: 'select',
    section: 'Interplant Delivery Packaging',
    className: 'col-md-4',
    // Opciones hardcodeadas según tu HTML Legacy
    options: [
      { value: 'OWN', label: 'Developed by tkMM' },
      { value: 'CM', label: 'Provided by Client' }
    ],
    placeholder: 'Select an option',
    validation: {
      required: false
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
  // 3. Requires Rack Manufacturing (Checkbox Condicional)
  {
    name: 'InterplantRequiresRackManufacturing',
    label: 'Requires Rack Manufacturing?',
    type: 'checkbox',
    section: 'Interplant Delivery Packaging',
    className: 'col-md-3',
    visibleWhen: [
      {
        field: 'InterplantPackagingStandard',
        is: ['OWN']
      },
      {
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
    ],
    validation: {
      required: false
    },
  },
  // 4. Pieces Per Package
  {
    name: 'InterplantPiecesPerPackage',
    label: 'Pieces Per Package',
    type: 'number',
    section: 'Interplant Delivery Packaging',
    className: 'col-md-3',
    placeholder: 'Qty',
    decimals: 0, // Entero
    validation: {
      required: false,
      min: 0,
      customMessage: "Must be a non-negative whole number."
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
  // 5. Stacks Per Package
  {
    name: 'InterplantStacksPerPackage',
    label: 'Stacks Per Package',
    type: 'number',
    section: 'Interplant Delivery Packaging',
    className: 'col-md-3',
    placeholder: 'Qty',
    decimals: 0, // Entero
    validation: {
      required: false,
      min: 0,
      customMessage: "Must be a non-negative whole number."
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
  // 6. Package Weight (Calculado)
  {
    name: 'InterplantPackageWeight',
    label: 'Package Weight [kg]',
    type: 'number',
    section: 'Interplant Delivery Packaging',
    className: 'col-md-3',
    placeholder: 'Calculated',
    decimals: 3,
    disabled: true, // ReadOnly
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
  // 7. Interplant Packaging File
  {
    name: 'ID_File_InterplantPackaging',
    label: 'Packaging File',
    type: 'file',
    section: 'Interplant Delivery Packaging',
    className: 'col-md-4',
    // Nombre del parámetro que espera tu controlador C#
    uploadFieldName: 'interplant_packaging_archivo',
    fileNameProp: 'InterplantPackagingFileName',
    fileEntityProp: 'CTZ_Files2',
    validation: {
      required: false,
      accept: ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
      maxSizeInMB: 20
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
  // 4. Interplant Rack Types (Grupo de Checkboxes)
  {
    name: 'InterplantRackTypeIds', // Array de IDs en el modelo
    label: 'Interplant Rack Types',
    type: 'checkbox-group',        // 👈 El nuevo tipo
    section: 'Interplant Delivery Packaging',
    className: 'col-md-12',         // Ancho según tu legacy
    // Nombre de la lista en 'lists' (debe coincidir con ViewBag.RackTypeList)
    optionsKey: 'rackTypeList',

    validation: {
      required: false
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
  // 8. Is Returnable Rack? (Depende de los Racks seleccionados)
  {
    name: 'IsInterplantReturnableRack',
    label: 'Is Returnable Rack?',
    type: 'checkbox',
    section: 'Interplant Delivery Packaging',
    className: 'col-md-3',
    visibleWhen: [
      {
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
      },
      {
        field: 'InterplantRackTypeIds',
        is: [2, 3, 4]
      }
    ],
    validation: {
      required: false
    }
  },

  // 9. Returnable Uses (Depende del checkbox anterior)
  {
    name: 'InterplantReturnableUses',
    label: 'Returnable Uses',
    type: 'number',
    section: 'Interplant Delivery Packaging',
    className: 'col-md-3',
    placeholder: 'e.g., 5',
    decimals: 0, // Entero
    visibleWhen: [
      {
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
      },
      {
        field: 'IsInterplantReturnableRack',
        is: [true]
      }
    ],
    validation: {
      required: true, // Si es visible, es requerido (legacy logic)
      min: 1,
      customMessage: "Must be a positive whole number."
    }
  },
  // 1. Interplant Labels (Grupo)
  {
    name: 'InterplantLabelTypeIds',
    label: 'Interplant Labels',
    type: 'checkbox-group', // Usará el estilo Pro que creamos
    section: 'Interplant Delivery Packaging',
    className: 'col-md-12',
    optionsKey: 'labelList',
    validation: {
      required: false
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
  // 2. Specify Other Label (Visible solo si se selecciona ID 3)
  {
    name: 'InterplantLabelOtherDescription',
    label: 'Specify "Other" Label',
    type: 'text', // O 'textarea' si prefieres
    section: 'Interplant Delivery Packaging',
    className: 'col-md-6',
    placeholder: 'Description...',
    visibleWhen: [
      {
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
      },
      {
        field: 'InterplantLabelTypeIds',
        is: [3] // 3 es el ID de "Other" en tu BD
      }
    ],
    validation: {
      required: true, // Si es visible, es requerido
      maxLength: 120
    }
  },
  // 1. Checkbox Group
  {
    name: 'InterplantAdditionalIds',
    label: 'Interplant Additionals',
    type: 'checkbox-group',
    section: 'Interplant Delivery Packaging',
    className: 'col-md-12',
    optionsKey: 'additionalList',
    validation: { required: false },
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
  // 2. Specify Other (Solo si ID 6 está seleccionado)
  {
    name: 'InterplantAdditionalsOtherDescription',
    label: 'Specify "Other" Additional',
    type: 'text', // Legacy usa textarea row=1
    section: 'Interplant Delivery Packaging',
    className: 'col-md-6',
    placeholder: 'Description...',
    visibleWhen: [
      {
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
      },
      {
        field: 'InterplantAdditionalIds',
        is: [6] // 6 es el ID de "Other" en tu BD
      }
    ],
    validation: {
      required: true, // Si es visible, es requerido
      maxLength: 120
    }
  },
  // --- GRUPO: Interplant Standard Packaging (Straps) ---
  // 3. Checkbox Group
  {
    name: 'InterplantStrapTypeIds',
    label: 'Interplant Standard Packaging',
    type: 'checkbox-group',
    section: 'Interplant Delivery Packaging',
    className: 'col-md-12',
    optionsKey: 'strapTypeList',
    validation: { required: false },
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
  // 4. Observations (SIEMPRE VISIBLE)
  {
    name: 'InterplantStrapTypeObservations',
    label: 'Strap Type Observations', // Etiqueta visual
    type: 'textarea',
    section: 'Interplant Delivery Packaging',
    className: 'col-md-12',
    placeholder: 'Strap type observations...',
    // Sin 'visibleWhen' -> Se muestra siempre en esta pestaña
    validation: {
      required: false, // Opcional
      maxLength: 120
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
  // 1. Special Requirement
  {
    name: 'InterplantSpecialRequirement',
    label: 'Interplant Special Requirement',
    type: 'textarea', // 👈 Usamos el tipo textarea
    section: 'Interplant Delivery Packaging',
    className: 'col-md-6', // 👈 Mitad del ancho
    placeholder: 'Enter requirements...',
    validation: {
      required: false,
      maxLength: 350 // Regla del legacy
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
  // 2. Special Packaging
  {
    name: 'InterplantSpecialPackaging',
    label: 'Interplant Special Packaging',
    type: 'textarea',
    section: 'Interplant Delivery Packaging',
    className: 'col-md-6', // 👈 La otra mitad
    placeholder: 'Enter packaging details...',
    validation: {
      required: false,
      maxLength: 350 // Regla del legacy
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
  /****** Interplant Freight ********/
  {
    name: 'ID_Interplant_FreightType',
    label: 'Incoterm Type',
    type: 'select',
    section: 'Interplant Outbound Freight & Conditions', // Nueva Pestaña
    className: 'col-md-3',
    optionsKey: 'freightTypeList', // Debe coincidir con el controller
    placeholder: 'Select an option',
    validation: {
      required: false
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
  // 2. Transport Type (Reutiliza la lista de transporte general)
  {
    name: 'ID_InterplantDelivery_Transport_Type',
    label: 'Transport Type',
    type: 'select',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-3',
    optionsKey: 'transportTypes', // 👈 REUTILIZADO
    placeholder: 'Select transport...',
    validation: {
      required: false
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
  // 3. Specify Other Transport (Condicional)
  {
    name: 'InterplantDelivery_Transport_Type_Other',
    label: 'Specify Other Transport',
    type: 'text',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Specify transport...',
    visibleWhen: [
      {
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
      },
      {
        field: 'ID_InterplantDelivery_Transport_Type',
        is: [5] // 5 es el ID de "Other" en tu BD
      }
    ],
    validation: {
      required: true, // Requerido si es visible
      maxLength: 50
    }
  },
  // 4. Load Per Transport
  {
    name: 'InterplantLoadPerTransport',
    label: 'Load Per Transport',
    type: 'number',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Load',
    decimals: 2, // step="any"
    validation: {
      required: false,
      min: 0,
      customMessage: "Must be a positive number."
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
  // 5. Delivery Conditions (Textarea grande)
  {
    name: 'InterplantDeliveryConditions',
    label: 'Delivery Conditions',
    type: 'textarea', // 👈 Tipo Textarea
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-12', // 👈 Ajustado a 12 para que use toda la fila (o 9 si prefieres mantener legacy layout exacto)
    placeholder: 'Describe interplant delivery conditions...',
    validation: {
      required: false,
      maxLength: 350
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
  // 1. Checkbox Padre
  {
    name: 'InterplantScrapReconciliation',
    label: 'Interplant Scrap Reconciliation',
    type: 'checkbox',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-12', // Ocupa toda la fila para separar visualmente
    validation: { required: false },
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

  // 2. Fila de Porcentajes (Matriz de 4 columnas)

  {
    name: 'InterplantScrapReconciliationPercent_Min',
    label: 'Scrap Min %',
    type: 'number',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Min %',
    decimals: 2,
    validation: { required: false, min: 0, max: 100 },
    visibleWhen: [
      {
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
      },
      { field: 'InterplantScrapReconciliation', is: [true] }
    ],
  },
  {
    name: 'InterplantScrapReconciliationPercent', // Optimal
    label: 'Scrap Optimal %',
    type: 'number',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Optimal %',
    decimals: 2,
    validation: {
      required: true, // Requerido si el padre está activo (legacy logic)
      min: 0,
      max: 100
    },
    visibleWhen: [
      {
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
      },
      { field: 'InterplantScrapReconciliation', is: [true] }
    ],
  },
  {
    name: 'InterplantScrapReconciliationPercent_Max',
    label: 'Scrap Max %',
    type: 'number',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Max %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'InterplantScrapReconciliation', is: [true] }
    ],
    validation: { required: false, min: 0, max: 100 }
  },
  {
    name: 'InterplantClientScrapReconciliationPercent',
    label: 'Client Scrap %',
    type: 'number',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Client %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'InterplantScrapReconciliation', is: [true] }
    ],
    validation: { required: false, min: 0, max: 100 }
  },
  // --- GRUPO 2: Interplant Head/Tail Reconciliation ---
  // 1. Checkbox Padre
  {
    name: 'InterplantHeadTailReconciliation',
    label: 'Interplant Head/Tail Reconciliation',
    type: 'checkbox',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-12',
    validation: { required: false },
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
  // 2. Fila de Porcentajes (4 inputs)
  {
    name: 'InterplantHeadTailReconciliationPercent_Min',
    label: 'H/T Min %',
    type: 'number',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Min %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'InterplantHeadTailReconciliation', is: [true] }
    ],
    validation: { required: false, min: 0, max: 100 }
  },
  {
    name: 'InterplantHeadTailReconciliationPercent', // Optimal
    label: 'H/T Optimal %',
    type: 'number',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Optimal %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'InterplantHeadTailReconciliation', is: [true] }
    ],
    validation: {
      required: true, // Requerido si el padre está activo
      min: 0,
      max: 100
    }
  },
  {
    name: 'InterplantHeadTailReconciliationPercent_Max',
    label: 'H/T Max %',
    type: 'number',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Max %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'InterplantHeadTailReconciliation', is: [true] }
    ],
    validation: { required: false, min: 0, max: 100 }
  },
  {
    name: 'InterplantClientHeadTailReconciliationPercent',
    label: 'Client H/T %',
    type: 'number',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Client %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'InterplantHeadTailReconciliation', is: [true] }
    ],
    validation: { required: false, min: 0, max: 100 }
  },
  // 6. Interplant Outbound Freight File
  {
    name: 'ID_File_InterplantOutboundFreight',
    label: 'Outbound Freight Additional File',
    type: 'file',
    section: 'Interplant Outbound Freight & Conditions',
    className: 'col-md-4',
    uploadFieldName: 'interplantOutboundFreightAdditionalFile',
    fileNameProp: 'FileName_InterplantOutboundFreight',
    fileEntityProp: 'CTZ_Files3',
    validation: {
      required: false,
      accept: ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
      maxSizeInMB: 20
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
  // 1. Delivery Coil Position (Reusa la lista existente)
  {
    name: 'ID_Delivery_Coil_Position',
    label: 'Delivery Coil Position',
    type: 'select',
    section: 'Final Delivery Packaging', // Nueva pestaña
    className: 'col-md-3',
    optionsKey: 'coilPositions', // 👈 REUTILIZADO
    placeholder: 'Select an option',
    validation: {
      required: false
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
  // 2. Packaging Standard
  {
    name: 'PackagingStandard',
    label: 'Packaging Standard',
    type: 'select',
    section: 'Final Delivery Packaging',
    className: 'col-md-4',
    // Opciones hardcodeadas según legacy
    options: [
      { value: 'OWN', label: 'Developed by tkMM' },
      { value: 'CM', label: 'Provided by Client' }
    ],
    placeholder: 'Select an option',
    validation: {
      required: false // Puedes cambiar a true si quieres replicar validatePackagingStandard
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
  // 3. Requires Rack Manufacturing (Condicional)
  {
    name: 'RequiresRackManufacturing',
    label: 'Requires Rack Manufacturing?',
    type: 'checkbox',
    section: 'Final Delivery Packaging',
    className: 'col-md-3',
    // VISIBILIDAD: Solo si Standard es 'OWN'
    visibleWhen: [
      {
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
      },
      {
        field: 'PackagingStandard',
        is: ['OWN']
      }
    ],
    validation: { required: false }
  },
  // 4. Pieces Per Package
  {
    name: 'PiecesPerPackage',
    label: 'Pieces Per Package',
    type: 'number',
    section: 'Final Delivery Packaging',
    className: 'col-md-3',
    placeholder: 'Qty',
    decimals: 0,
    validation: {
      required: false,
      min: 0,
      customMessage: "Must be a non-negative whole number."
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
  // 5. Stacks Per Package
  {
    name: 'StacksPerPackage',
    label: 'Stacks Per Package',
    type: 'number',
    section: 'Final Delivery Packaging',
    className: 'col-md-3',
    placeholder: 'Qty',
    decimals: 0,
    validation: {
      required: false,
      min: 0,
      customMessage: "Must be a non-negative whole number."
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
  // 6. Package Weight (Calculado)
  {
    name: 'PackageWeight',
    label: 'Package Weight [kg]',
    type: 'number',
    section: 'Final Delivery Packaging',
    className: 'col-md-3',
    placeholder: 'Calculated',
    decimals: 3,
    disabled: true, // ReadOnly
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
  // 7. Packaging Drawing / Standard
  {
    name: 'ID_File_Packaging', // ID en BD
    label: 'Packaging Drawing / Standard',
    type: 'file',
    section: 'Final Delivery Packaging',
    className: 'col-md-6', // Legacy usa col-md-8
    // Nombre del parámetro que espera el Controller C#
    uploadFieldName: 'packaging_archivo',
    fileNameProp: 'FileName_Packaging',
    fileEntityProp: 'CTZ_Files1',
    validation: {
      required: false,
      accept: ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
      maxSizeInMB: 10
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
  // 8. Delivery Packaging Additional File
  {
    name: 'ID_File_DeliveryPackagingAdditional', // ID en BD
    label: 'Packaging Additional File',
    type: 'file',
    section: 'Final Delivery Packaging',
    className: 'col-md-6', // Legacy usa col-md-4
    // Nombre del parámetro que espera el Controller C#
    uploadFieldName: 'deliveryPackagingAdditionalFile',
    fileNameProp: 'FileName_DeliveryPackagingAdditional',
    fileEntityProp: 'CTZ_Files11',
    validation: {
      required: false,
      accept: ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
      maxSizeInMB: 10 // Asumiendo 10MB igual que el otro
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
  // 9. Rack Types (Checkbox Group)
  {
    name: 'SelectedRackTypeIds',
    label: 'Rack Types',
    type: 'checkbox-group',
    section: 'Final Delivery Packaging',
    className: 'col-md-12',
    optionsKey: 'rackTypeList', // Reuses the list from Interplant/Arrival
    validation: { required: false },
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
  // --- RETURNABLE RACK LOGIC (Depends on Rack Types) ---  
  // 12. Is Returnable Rack?
  {
    name: 'IsReturnableRack',
    label: 'Is Returnable Rack?',
    type: 'checkbox',
    section: 'Final Delivery Packaging',
    className: 'col-md-3', // Small width    
    // VISIBILITY: Only if special racks [2, 3, 4] are selected
    visibleWhen: [
      {
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
      },
      {
        field: 'SelectedRackTypeIds',
        is: [2, 3, 4] // Adjust IDs based on your DB (Wood, etc.)
      }
    ],
    validation: { required: false }
  },
  // 13. Returnable Uses
  {
    name: 'ReturnableUses',
    label: 'Returnable Uses',
    type: 'number',
    section: 'Final Delivery Packaging',
    className: 'col-md-3',
    placeholder: 'e.g., 5',
    decimals: 0,
    // VISIBILITY: Only if IsReturnableRack is checked
    visibleWhen: [
      {
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
      },
      {
        field: 'IsReturnableRack',
        is: [true]
      }
    ],
    validation: {
      required: true,
      min: 1,
      customMessage: "Must be a positive whole number."
    }
  },
  // 10. Labels (Checkbox Group)
  {
    name: 'SelectedLabelIds',
    label: 'Labels',
    type: 'checkbox-group',
    section: 'Final Delivery Packaging',
    className: 'col-md-12',
    optionsKey: 'labelList', // Reuses label list
    validation: { required: false },
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
  // 11. Specify "Other" Label (Only if ID 3 is selected in Labels)
  {
    name: 'LabelOtherDescription',
    label: 'Specify "Other" Label',
    type: 'text', // Legacy uses textarea row=1
    section: 'Final Delivery Packaging',
    className: 'col-md-12',
    placeholder: 'Description...',
    // VISIBILITY: Depends on SelectedLabelIds containing 3
    visibleWhen: [
      {
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
      },
      {
        field: 'SelectedLabelIds',
        is: [3]
      }
    ],
    validation: {
      required: true,
      maxLength: 120
    }
  },
  // --- GRUPO: Additionals ---
  // 14. Additionals (Checkbox Group)
  {
    name: 'SelectedAdditionalIds',
    label: 'Additionals',
    type: 'checkbox-group',
    section: 'Final Delivery Packaging',
    className: 'col-md-12',
    optionsKey: 'additionalList',
    validation: { required: false },
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
  // 15. Specify "Other" Additional (Only if ID 6 is selected)
  {
    name: 'AdditionalsOtherDescription',
    label: 'Specify "Other" Additional',
    type: 'text',
    section: 'Final Delivery Packaging',
    className: 'col-md-12',
    placeholder: 'Description...',
    visibleWhen: [
      {
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
      },
      {
        field: 'SelectedAdditionalIds',
        is: [6] // ID 6 is "Other"
      }
    ],
    validation: {
      required: true,
      maxLength: 120
    }
  },
  // --- GRUPO: Standard Packaging (Straps) ---
  // 16. Standard Packaging / Straps (Checkbox Group)
  {
    name: 'SelectedStrapTypeIds',
    label: 'Standard Packaging',
    type: 'checkbox-group',
    section: 'Final Delivery Packaging',
    className: 'col-md-12',
    optionsKey: 'strapTypeList',
    validation: { required: false },
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
  // 17. Strap Type Observations (Always Visible)
  {
    name: 'StrapTypeObservations',
    label: 'Standard Packaging Observations',
    type: 'textarea',
    section: 'Final Delivery Packaging',
    className: 'col-md-12',
    placeholder: 'Strap type observations...',
    validation: {
      required: false,
      maxLength: 120
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
  // --- SPECIAL TEXTAREAS ---
  // 18. Special Requirement
  {
    name: 'SpecialRequirement',
    label: 'Special Requirement',
    type: 'textarea',
    section: 'Final Delivery Packaging',
    className: 'col-md-6',
    placeholder: 'Enter requirements...',
    validation: {
      required: false,
      maxLength: 350
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
  // 19. Special Packaging
  {
    name: 'SpecialPackaging',
    label: 'Special Packaging',
    type: 'textarea',
    section: 'Final Delivery Packaging',
    className: 'col-md-6',
    placeholder: 'Enter packaging details...',
    validation: {
      required: false,
      maxLength: 350
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
  // --- SECCIÓN: Final Outbound Freight & Conditions ---
  // 1. Freight Type
  {
    name: 'ID_FreightType',
    label: 'Freight Type',
    type: 'select',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-3',
    optionsKey: 'freightTypeList',
    validation: { required: false },
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
  // 2. Transport Type
  {
    name: 'ID_Delivery_Transport_Type',
    label: 'Transport Type',
    type: 'select',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-3',
    optionsKey: 'transportTypes', // Reutilizado
    placeholder: 'Select transport...',
    validation: { required: false },
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
  // 3. Specify Other Transport (Condicional)
  {
    name: 'Delivery_Transport_Type_Other',
    label: 'Specify Other Transport',
    type: 'text',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Specify transport...',
    // VISIBILIDAD: Solo si TransportType es 5 (Other)
    visibleWhen: [
      {
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
      },
      {
        field: 'ID_Delivery_Transport_Type',
        is: [5]
      }
    ],
    validation: {
      required: true,
      maxLength: 50
    }
  },
  // 4. Load Per Transport
  {
    name: 'LoadPerTransport',
    label: 'Load Per Transport',
    type: 'number',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Load',
    decimals: 2,
    validation: {
      required: false,
      min: 0,
      customMessage: "Must be a positive number."
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
  // 5. Delivery Conditions (Textarea)
  {
    name: 'DeliveryConditions',
    label: 'Delivery Conditions',
    type: 'textarea',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-12',
    placeholder: 'Describe delivery conditions...',
    validation: {
      required: false,
      maxLength: 350
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
  // --- GRUPO: Scrap Reconciliation ---
  // 6. Checkbox Padre
  {
    name: 'ScrapReconciliation',
    label: 'Scrap Reconciliation',
    type: 'checkbox',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-12',
    validation: { required: false },
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
  // 7. Porcentajes Scrap (4 inputs)
  {
    name: 'ScrapReconciliationPercent_Min',
    label: 'Scrap Min %',
    type: 'number',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Min %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'ScrapReconciliation', is: [true] }
    ],
    validation: { required: false, min: 0, max: 100 }
  },
  {
    name: 'ScrapReconciliationPercent', // Optimal
    label: 'Scrap Optimal %',
    type: 'number',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Optimal %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'ScrapReconciliation', is: [true] }
    ],
    validation: { required: true, min: 0, max: 100 }
  },
  {
    name: 'ScrapReconciliationPercent_Max',
    label: 'Scrap Max %',
    type: 'number',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Max %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'ScrapReconciliation', is: [true] }
    ],
    validation: { required: false, min: 0, max: 100 }
  },
  {
    name: 'ClientScrapReconciliationPercent',
    label: 'Client Scrap %',
    type: 'number',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Client %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'ScrapReconciliation', is: [true] }
    ],
    validation: { required: false, min: 0, max: 100 }
  },
  // --- GRUPO: Head/Tail Reconciliation ---
  // 8. Checkbox Padre
  {
    name: 'HeadTailReconciliation',
    label: 'Head/Tail Reconciliation',
    type: 'checkbox',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-12',
    validation: { required: false },
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
  // 9. Porcentajes H/T (4 inputs)
  {
    name: 'HeadTailReconciliationPercent_Min',
    label: 'H/T Min %',
    type: 'number',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Min %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'HeadTailReconciliation', is: [true] }
    ],
    validation: { required: false, min: 0, max: 100 }
  },
  {
    name: 'HeadTailReconciliationPercent', // Optimal
    label: 'H/T Optimal %',
    type: 'number',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Optimal %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'HeadTailReconciliation', is: [true] }
    ],
    validation: { required: true, min: 0, max: 100 }
  },
  {
    name: 'HeadTailReconciliationPercent_Max',
    label: 'H/T Max %',
    type: 'number',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Max %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'HeadTailReconciliation', is: [true] }
    ],
    validation: { required: false, min: 0, max: 100 }
  },
  {
    name: 'ClientHeadTailReconciliationPercent',
    label: 'Client H/T %',
    type: 'number',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-3',
    placeholder: 'Client %',
    decimals: 2,
    visibleWhen: [
      {
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
      },
      { field: 'HeadTailReconciliation', is: [true] }
    ],
    validation: { required: false, min: 0, max: 100 }
  },
  // 10. Outbound Freight Additional File
  {
    name: 'ID_File_OutboundFreightAdditional',
    label: 'Outbound Freight Additional File',
    type: 'file',
    section: 'Final Outbound Freight & Conditions',
    className: 'col-md-4',
    // Nombre del parámetro en el Controller C#
    uploadFieldName: 'outboundFreightAdditionalFile',
    fileNameProp: 'FileName_OutboundFreightAdditional',
    fileEntityProp: 'CTZ_Files10',
    validation: {
      required: false,
      accept: ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
      maxSizeInMB: 20
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
  // 1. Theoretical Blanking Line (Readonly)
  {
    name: 'ID_Theoretical_Blanking_Line',
    label: 'Theoretical Blanking Line',
    type: 'select', // CAMBIO: Ahora es un Select
    optionsKey: 'linesList', // CAMBIO: Se conecta a la lista que viene del backend
    section: 'Technical Feasibility',
    className: 'col-md-3',
    disabled: true, // Se mantiene deshabilitado para que sea solo lectura
    placeholder: 'Theoretical Blanking Line',
    // Usaremos un campo virtual en el form para mostrar el nombre si viene separado del ID
    // O si el backend manda el nombre en este campo, perfecto.
  },
  // 2. Theoretical Strokes (Calculado AJAX)
  {
    name: 'Theoretical_Strokes',
    label: 'Theoretical Strokes',
    type: 'number',
    section: 'Technical Feasibility',
    className: 'col-md-3',
    disabled: true, // Readonly
    placeholder: 'Theoretical Strokes',
    decimals: 2
  },
  // 3. Effective Strokes (Calculado Local)
  {
    name: 'Theoretical_Effective_Strokes',
    label: 'Theoretical Effective Strokes',
    type: 'number',
    section: 'Technical Feasibility',
    className: 'col-md-3',
    disabled: true, // Readonly
    placeholder: 'Strokes * OEE',
    decimals: 0 // Entero
  },
  // 4. Real Blanking Line (Dropdown editable por Ingeniería)
  {
    name: 'ID_Real_Blanking_Line',
    label: 'Real Blanking Line',
    type: 'select',
    section: 'Technical Feasibility',
    className: 'col-md-3',
    optionsKey: 'linesList', // Nombre en listsPayload
    placeholder: 'Select Line',
    validation: {
      required: false // Se valida dinámicamente según permisos
    }
  },
  // 5. Real Strokes (Calculado AJAX)
  {
    name: 'Real_Strokes',
    label: 'Real Strokes',
    type: 'number',
    section: 'Technical Feasibility',
    className: 'col-md-3',
    disabled: true, // Readonly
    placeholder: 'Real Strokes',
    decimals: 2
  },
  // 6. Real Effective Strokes (Calculado Local)
  {
    name: 'Real_Effective_Strokes',
    label: 'Real Effective Strokes',
    type: 'number',
    section: 'Technical Feasibility',
    className: 'col-md-3',
    disabled: true, // Readonly
    placeholder: 'Strokes * OEE',
    decimals: 0 // Entero
  },
  // 7. Ideal Cycle Time Per Tool
  {
    name: 'Ideal_Cycle_Time_Per_Tool',
    label: 'Ideal Cycle Time Per Tool',
    type: 'number',
    section: 'Technical Feasibility',
    className: 'col-md-3',
    placeholder: 'Ideal Cycle Time',
    decimals: 2,
    validation: {
      required: false // Se valida dinámicamente si es Ingeniero
    }
  },
  // 8. OEE (%)
  {
    name: 'OEE',
    label: 'OEE (%)',
    type: 'number', // O 'percentage' si creaste un tipo especial
    section: 'Technical Feasibility',
    className: 'col-md-3',
    placeholder: 'OEE',
    decimals: 2,
    validation: {
      required: false, // Dinámico
      min: 0,
      max: 100
    }
  },
  // 9. Tons Per Shift
  {
    name: 'TonsPerShift',
    label: 'Tons per Shift',
    type: 'number',
    section: 'Technical Feasibility',
    className: 'col-md-3',
    placeholder: 'Tons/Shift',
    decimals: 2,
    validation: {
      required: false,
      min: 0
    }
  },
  // 10. Slitting Line
  {
    name: 'ID_Slitting_Line',
    label: 'Slitting Line',
    type: 'select',
    section: 'Technical Feasibility',
    className: 'col-md-3',
    optionsKey: 'linesList',
    placeholder: 'Select a slitting line',
    validation: {
      required: false, // Opcional: Podrías poner requiredWhen aquí si quieres que sea obligatorio en estas rutas
    },
    // 👇 AGREGAR ESTE BLOQUE DE VISIBILIDAD
    visibleWhen: {
      field: 'ID_Route',
      is: [
        ROUTES.REWINDED,        // ID 7
        ROUTES.SLT,             // ID 8
        ROUTES.SLT_BLK,         // ID 9
        ROUTES.SLT_BLK_WLD,     // ID 10
        ROUTES.WEIGHT_DIVISION  // ID 13
      ]
    }
  },
  // 1. Parts / Auto
  {
    name: 'Parts_Auto',
    label: 'Parts/Auto',
    type: 'number',
    section: 'Efficiency and Capacity',
    className: 'col-md-3',
    disabled: true,
    placeholder: 'Parts/Auto',
    decimals: 2
  },
  // 2. Stroke / Auto
  {
    name: 'Strokes_Auto',
    label: 'Stroke/Auto',
    type: 'number',
    section: 'Efficiency and Capacity',
    className: 'col-md-3',
    disabled: true,
    placeholder: 'Stroke/Auto',
    decimals: 2
  },
  // 3. Blanks / Year
  {
    name: 'Blanks_Per_Year',
    label: 'Blanks/Year',
    type: 'number',
    section: 'Efficiency and Capacity',
    className: 'col-md-3',
    disabled: true,
    placeholder: 'Blanks/Year',
    decimals: 0
  },
  // 4. Min Max Reales
  {
    name: 'Min_Max_Reales',
    label: 'Min Max Reales',
    type: 'number',
    section: 'Efficiency and Capacity',
    className: 'col-md-3',
    disabled: true,
    placeholder: 'Min Max Reales',
    decimals: 2
  },
  // 5. Min Max Reales / OEE
  {
    name: 'Min_Max_Reales_OEE',
    label: 'Min Max Reales / OEE',
    type: 'number',
    section: 'Efficiency and Capacity',
    className: 'col-md-3',
    disabled: true,
    placeholder: 'Min Max / OEE',
    decimals: 2
  },
  // 6. Actual Shifts
  {
    name: 'Actual_Shifts',
    label: 'Actual Shifts',
    type: 'number',
    section: 'Efficiency and Capacity',
    className: 'col-md-3',
    disabled: true,
    placeholder: 'Actual Shifts',
    decimals: 2
  },
  // 7. Stroke / Shift
  {
    name: 'Strokes_Shift',
    label: 'Stroke / shift',
    type: 'number',
    section: 'Efficiency and Capacity',
    className: 'col-md-3',
    disabled: true,
    placeholder: 'Stroke / shift',
    decimals: 2
  },
  // 8. Status DM
  {
    name: 'DM_status',
    label: 'Status DM',
    type: 'text',
    section: 'Efficiency and Capacity',
    className: 'col-md-3',
    disabled: true,
    placeholder: 'Status DM'
  },
  // 9. Status Comment (Usamos textarea para emular el bloque grande del legacy)
  {
    name: 'DM_status_comment',
    label: 'Status Comment',
    type: 'textarea', // O 'text' si prefieres una sola línea
    section: 'Efficiency and Capacity',
    className: 'col-md-6', // Más ancho como en tu legacy
    disabled: true,
    placeholder: 'Comments...',
    // Simulamos el estilo grisáceo del legacy con disabled
  },
  // =====================================================================
  // SECCIÓN: VOLUME & WEIGHTS (DINÁMICA)
  // Estos campos se moverán a 'Coil Data' o 'Slitter Data' según la ruta
  // =====================================================================
  {
    name: 'Annual_Volume',
    label: 'Annual Volume',
    type: 'number',
    section: 'General', // Se sobrescribe dinámicamente
    className: 'col-md-2',
    placeholder: 'Annual Volume',
    decimals: 0,
    validation: {
      required: false, // Legacy: Requerido según estatus (Quotes, CarryOver, etc.)
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.QUOTES, PROJECT_STATUS.CARRY_OVER, PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      },
      min: 0
    }
  },
  {
    name: 'Volume_Per_year',
    label: 'Volume Per Year',
    type: 'number',
    section: 'General',
    className: 'col-md-2',
    placeholder: 'Volume / Year',
    decimals: 2, // step="any"
    validation: {
      required: false,
      requiredWhen: {
        field: '_projectStatusId',
        is: [PROJECT_STATUS.QUOTES, PROJECT_STATUS.CARRY_OVER, PROJECT_STATUS.CASI_CASI, PROJECT_STATUS.POH]
      },
      min: 0
    }
  },
  {
    name: 'WeightPerPart',
    label: 'Weight Per Part',
    type: 'number',
    section: 'General',
    className: 'col-md-2',
    placeholder: 'Weight/Part',
    decimals: 3,
    disabled: true // Readonly (Calculado)
  },
  {
    name: 'Initial_Weight',
    label: 'Initial Weight',
    type: 'number',
    section: 'General',
    className: 'col-md-2',
    placeholder: 'Initial Weight',
    decimals: 3,
    disabled: true // Readonly (Calculado)
  },
  {
    name: 'AnnualTonnage',
    label: 'Annual Tonnage',
    type: 'number',
    section: 'General',
    className: 'col-md-2',
    placeholder: 'Annual Tonnage',
    decimals: 3,
    disabled: true // Readonly (Calculado)
  },
  {
    name: 'ShippingTons',
    label: 'Shipping Tons',
    type: 'number',
    section: 'General',
    className: 'col-md-2', // Legacy lo tenía col-md-3, ajusta a gusto
    placeholder: 'Shipping Tons',
    decimals: 3,
    disabled: true // Readonly (Calculado)
  },
  {
    name: 'ID_File_VolumeAdditional',
    label: 'Volume Additional File',
    type: 'file',
    section: 'General',
    className: 'col-md-6',
    uploadFieldName: 'volumeAdditionalFile', // Nombre para el Controller
    fileNameProp: 'FileName_VolumeAdditional',
    fileEntityProp: 'CTZ_Files9',
    validation: {
      required: false,
      accept: ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
      maxSizeInMB: 20
    }
  }
];