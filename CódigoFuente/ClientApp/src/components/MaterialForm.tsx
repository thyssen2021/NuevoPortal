//src/componemts/MaterialForm.tsx
import { useState, useEffect, useMemo } from 'react';
import type { Material, WeldedPlate } from '../types';
// Aseguramos la importación.
import type { FieldConfig } from '../types/form-schema';
import { materialFields } from '../config/material-fields';
import ToleranceReference from './ToleranceReference';
import IncotermsReference from './IncotermsReference';
import DynamicField from './DynamicField';
import { toast, Slide } from 'react-toastify';
import { calculateTheoreticalLine } from '../utils/theoretical-calculator';
import { ROUTES } from '../constants'; // Asegúrate de tener tus constantes de rutas

// --- ESTILOS MODERNOS INCRUSTADOS ---
// Esto le da el look "Panel de Configuración" sin tocar tu site.css
const modernStyles = `
  /* Navegación Izquierda */
 .main-background {
      background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
      min-height: 100vh;
      padding: 30px;
  }

  /* Navegación Izquierda */
  .modern-nav .nav-link {
    background-color: #ffffff;      /* Fondo blanco para que no flote */
    color: #6c757d;                 /* Gris elegante */
    border: 1px solid #e9ecef;      /* Borde sutil para definir la caja */
    border-radius: 10px;            /* Bordes más redondeados */
    padding: 15px 20px;             /* Más espacio interno */
    margin-bottom: 12px;            /* Separación entre tarjetas */
    transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1); /* Animación suave */
    text-align: left;
    display: flex;
    align-items: center;
    font-weight: 600;
    box-shadow: 0 2px 5px rgba(0,0,0,0.02); /* Sombra mínima */
    position: relative;
    overflow: hidden;
  }
  
  .modern-nav .nav-link:hover {
    border-color: #b3e0ff;          /* Borde azul suave */
    background-color: #f8fbff;      /* Fondo azul muy pálido */
    color: #009ff5;                 /* Texto azul */
    transform: translateY(-2px);    /* Se levanta un poco */
    box-shadow: 0 5px 15px rgba(0, 159, 245, 0.1); /* Sombra azulada */
    /*padding-left: 25px;*/             /* Desplazamiento interno */
  }

 .modern-nav .nav-link.active {
    background-color: #fff;
    color: #009ff5;                 /* Azul Thyssen */
    border: 1px solid #009ff5;      /* Borde completo azul */
    border-left: 6px solid #009ff5; /* Indicador grueso a la izquierda */
    box-shadow: 0 8px 20px rgba(0, 159, 245, 0.15); /* Sombra "Glow" */
    transform: scale(1.02);         /* Crece levemente */
    z-index: 2;
  }

  .modern-nav .nav-link i {
    font-size: 1.1rem;
    width: 25px;
    text-align: center;
    transition: color 0.3s;
  }

  /* Tarjeta de Contenido */
  .form-card {
    background: #ffffff;
    border-radius: 12px;
    /* Sombra más pronunciada para separarlo del fondo */
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08); 
    border: none;
    padding: 35px; /* Un poco más de aire */
    min-height: 550px;
  }

  /* Títulos de Inputs (Mejora de Legibilidad) */
  .form-group label {
      font-size: 0.85rem;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      margin-bottom: 0.4rem;
  }

  .form-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    position: relative;
    margin-bottom: 25px;
    padding-bottom: 15px; /* Un poco más compacto */
  }

  /* Creamos la línea con un pseudo-elemento para tener control total del degradado */
  .form-header::after {
    content: '';
    position: absolute;
    bottom: 0;
    left: 0;
    width: 100%;
    height: 1px; /* 👈 Más delgada como solicitaste */
    background-color: #009ff5; /* 👈 Color sólido Thyssen */
    opacity: 0.6; /* Un toque de transparencia para que no sea tan agresiva */
  }

  .btn-modern-cancel {
    background-color: #fff;
    color: #dc3545; /* Rojo Danger de Bootstrap */
    border: 1px solid #dc3545;
    border-radius: 50px;
    padding: 6px 18px;
    font-size: 0.85rem;
    font-weight: 600;
    display: flex;
    align-items: center;
    gap: 8px;
    transition: all 0.3s ease-in-out;
    box-shadow: 0 2px 4px rgba(220, 53, 69, 0.05);
    cursor: pointer;
  }

  .btn-modern-cancel:hover {
    background-color: #dc3545; /* Fondo rojo sólido al hacer hover */
    color: #ffffff;            /* Texto blanco para contraste */
    border-color: #dc3545;
    box-shadow: 0 4px 8px rgba(220, 53, 69, 0.2);
    transform: translateY(-1px);
  }

  .btn-modern-cancel:active {
    transform: translateY(0);
    background-color: #a71d2a; /* Rojo más oscuro al hacer clic */
  }

  /* Estilo para el Globo de Notificación de Errores */
  .error-badge {
    background-color: #dc3545; /* Rojo */
    color: white;
    font-size: 0.75rem;
    font-weight: bold;
    padding: 2px 8px;
    border-radius: 12px;
    margin-left: auto; /* Empuja el globo a la derecha */
    box-shadow: 0 2px 4px rgba(220, 53, 69, 0.3);
    transition: all 0.2s ease;
  }
  
  /* Animación de pulso para llamar la atención */
  @keyframes pulse-red {
    0% { box-shadow: 0 0 0 0 rgba(220, 53, 69, 0.7); }
    70% { box-shadow: 0 0 0 6px rgba(220, 53, 69, 0); }
    100% { box-shadow: 0 0 0 0 rgba(220, 53, 69, 0); }
  }
  
  .error-badge.pulse {
      animation: pulse-red 2s infinite;
  }

  /* Contenedor del grupo de inputs para que no se estiren */
 /* Contenedor flexible para los 3 inputs */
  .matrix-group-container {
      display: flex !important;
      flex-direction: row !important;
      gap: 10px !important; /* Espacio fijo entre inputs */
      align-items: flex-start !important;
  }

  /* Celda rígida para cada input */
  .matrix-cell {
      flex: 0 0 150px !important; /* SIEMPRE 150px de ancho */
      width: 150px !important;
      min-width: 150px !important; /* Evita que se aplaste */
      max-width: 150px !important;
      padding: 0 !important; /* Quitamos padding de Bootstrap */
  }

  /* Forzamos al input a llenar la celda */
  .matrix-cell .form-group, 
  .matrix-cell .input-group,
  .matrix-cell input {
      width: 100% !important;
  }

  /* --- ESTILOS PRO CHECKBOX GROUP --- */
    .checkbox-group-frame {
        display: flex;
        flex-wrap: wrap;
        gap: 10px;
        margin-top: 5px;
        
        /* El borde y fondo que crean la "caja" */
        border: 1px solid #e9ecef;
        background-color: #f8f9fa; /* Fondo gris muy sutil para diferenciar del blanco general */
        border-radius: 8px;
        padding: 15px; /* Espacio interno para que respiren las opciones */
        
        /* Transición suave por si quieres efectos hover en el grupo completo */
        transition: border-color 0.2s ease;
    }
    
    .checkbox-group-frame:hover {
        border-color: #dbe0e6; /* Se oscurece un poco el borde al pasar el mouse */
    }
  .checkbox-group-wrapper {
      display: flex;
      flex-wrap: wrap;
      gap: 10px;
      margin-top: 5px;
  }
      
  /* El contenedor de cada opción individual (El "Chip") */
    .pro-checkbox-option {
        position: relative;
        cursor: pointer;
        display: flex;
        align-items: center;
        padding: 8px 14px; /* Un poco más compacto dentro del marco */
        background-color: #fff; /* Blanco puro para resaltar sobre el fondo gris del marco */
        border: 1px solid #dee2e6; 
        border-radius: 6px; 
        transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
        box-shadow: 0 1px 2px rgba(0,0,0,0.03); /* Sombra muy sutil */
        user-select: none;
        min-width: 130px; 
        flex-grow: 1; /* Para que llenen espacios uniformemente si quieres, o quítalo */
        max-width: 48%; /* Para forzar 2 columnas en espacios pequeños, opcional */
    }

 /* Estado Hover de la opción */
.pro-checkbox-option:hover:not(.disabled) {
    border-color: #b3e0ff;
    transform: translateY(-1px);
    box-shadow: 0 3px 6px rgba(0, 159, 245, 0.15);
    z-index: 1; /* Para que la sombra se vea por encima de otros */
}

/* Estado CHECKED */
.pro-checkbox-option.checked {
    background-color: #fff; /* Mantenemos blanco o usamos un azul muy suave */
    border-color: #009ff5;
    box-shadow: 0 0 0 1px #009ff5 inset; /* Borde interior extra para más peso visual */
}

/* Icono y Texto (Igual que antes, ajustado tamaños) */
.pro-checkbox-icon {
    width: 18px;
    height: 18px;
    border-radius: 4px;
    border: 2px solid #dee2e6;
    margin-right: 10px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: #f8f9fa;
    transition: all 0.2s ease;
}

.pro-checkbox-option.checked .pro-checkbox-icon {
    background-color: #009ff5;
    border-color: #009ff5;
}

.pro-checkbox-option.checked .pro-checkbox-icon i {
    color: #fff;
    font-size: 10px;
    transform: scale(1);
}

.pro-checkbox-icon i {
    transform: scale(0);
    transition: transform 0.2s;
}

.pro-checkbox-label {
    font-size: 0.85rem;
    color: #495057;
    font-weight: 500;
}

.pro-checkbox-option.checked .pro-checkbox-label {
    color: #009ff5;
    font-weight: 700;
}

.pro-checkbox-option.disabled {
    opacity: 0.5;
    cursor: not-allowed;
    background-color: #e9ecef;
    border-color: #dee2e6;
}

    /* ESTILO PARA LA PESTAÑA ESPECIAL (Technical Feasibility) */
 .modern-nav .nav-link.special-tab {
     border-left: 6px solid #28a745; /* Verde éxito */
     background-color: #f0fff4;
     color: #155724;
 }
 
 .modern-nav .nav-link.special-tab.active {
     background-color: #28a745;
     color: white;
     border-color: #28a745;
     box-shadow: 0 8px 20px rgba(40, 167, 69, 0.25);
 }
 
 .modern-nav .nav-link.special-tab:hover:not(.active) {
     background-color: #dcffe4;
     border-color: #28a745;
 }

 .modern-nav .nav-link.data-tab {
     border-left: 6px solid #6f42c1; /* Púrpura Bootstrap */
     background-color: #f3f0ff;
     color: #452c7a;
 }
 
 .modern-nav .nav-link.data-tab.active {
     background-color: #6f42c1;
     color: white;
     border-color: #6f42c1;
     box-shadow: 0 8px 20px rgba(111, 66, 193, 0.25);
 }
 
 .modern-nav .nav-link.data-tab:hover:not(.active) {
     background-color: #e5dbff;
     border-color: #6f42c1;
 }

`;

interface Props {
    selectedMaterial: Material | null;
    onCancel: () => void;
    lists: any;
    urls: any;
    interplantProcess: boolean;
    projectStatusId: number;
    onSaved: (material: Material) => void;
    projectPlantId: number;
}

export default function MaterialForm({ selectedMaterial, onCancel, lists, urls,
    interplantProcess, projectStatusId, onSaved, projectPlantId }: Props) {
    // DIAGNÓSTICO
    //console.log('📋 Cargando Configuración de campos:', materialFields);
    //console.log("📦 Listas disponibles:", lists);

    const [formData, setFormData] = useState<Partial<Material>>({
        Max_Production_Factor: 100
    });
    const [errors, setErrors] = useState<Record<string, string>>({});
    const [isSaving, setIsSaving] = useState(false);
    const [warnings, setWarnings] = useState<Record<string, string>>({});
    const [showToleranceModal, setShowToleranceModal] = useState(false);
    const [showIncotermsModal, setShowIncotermsModal] = useState(false);

    // Estado Metadata
    const [vehicleMetadata, setVehicleMetadata] = useState<Record<string, {
        Program?: string,
        SOP?: string,
        EOP?: string,
        ProductionDataJson?: string
    }>>({});

    // 1. DEFINICIÓN DE RUTAS Y CAMPOS (Para mover los inputs de volumen)
    const COIL_ROUTES = [
        ROUTES.COIL_TO_COIL,
        ROUTES.WAREHOUSING,
        ROUTES.WAREHOUSING_RP
    ]; // IDs 6, 11, 12

    const SLITTER_ROUTES = [
        ROUTES.REWINDED,
        ROUTES.SLT,
        ROUTES.SLT_BLK,
        ROUTES.SLT_BLK_WLD,
        ROUTES.WEIGHT_DIVISION
    ]; // IDs 7, 8, 9, 10, 13

    // Lista exacta de los campos que acabamos de agregar
    const VOLUME_FIELDS = [
        'Annual_Volume',
        'Volume_Per_year',
        'WeightPerPart',
        'Initial_Weight',
        'AnnualTonnage',
        'ShippingTons',
        'ID_File_VolumeAdditional'
    ];

    // 2. CÁLCULO DINÁMICO DE CAMPOS (Sobrescribe safeFields)
    const safeFields = useMemo(() => {
        const rawFields = Array.isArray(materialFields) ? materialFields : [];
        const currentRoute = Number(formData.ID_Route || 0);

        // Determinamos el destino basado en la ruta
        let targetSection = 'HIDDEN'; // Por defecto oculto (Caso Blanking)

        if (COIL_ROUTES.includes(currentRoute)) {
            targetSection = 'Coil Data';
        } else if (SLITTER_ROUTES.includes(currentRoute)) {
            targetSection = 'Slitter Data';
        }

        // Mapeamos la configuración original
        return rawFields.map(field => {
            // Si es un campo de volumen, alteramos su sección
            if (VOLUME_FIELDS.includes(field.name)) {

                // Si la ruta es Blanking (HIDDEN), lo ocultamos con una regla imposible
                if (targetSection === 'HIDDEN') {
                    return {
                        ...field,
                        section: 'General', // Sección dummy
                        visibleWhen: { field: 'ID_Project', is: [-9999] } // Nunca será visible
                    };
                }

                // Si aplica (Coil o Slitter), lo movemos a esa sección
                return {
                    ...field,
                    section: targetSection
                };
            }
            return field;
        });

    }, [formData.ID_Route, materialFields]); // Se recalcula al cambiar Ruta

    const sections = useMemo(() => {
        if (safeFields.length === 0) return ['General'];
        const uniqueSections = Array.from(new Set(safeFields.map(f => f.section)));
        return uniqueSections;
    }, []);

    const [activeTab, setActiveTab] = useState(sections[0] || 'General');

    // Búsqueda Síncrona en Memoria
    const getVehicleDetailsLocal = (vehicleName: string, countryName: string) => {
        if (!vehicleName || !countryName || !lists.vehicleMasterData) return null;

        const vehiclesInCountry = lists.vehicleMasterData[countryName] || [];
        // Buscamos el vehículo en la lista de ese país
        return vehiclesInCountry.find((v: any) => v.Value === vehicleName || v.Text === vehicleName);
    };

    // --- EFECTOS (Lógica de negocio intacta) ---

    // 1. Cargar Datos
    useEffect(() => {
        if (selectedMaterial) {
            // Función auxiliar para limpiar la fecha al cargar
            const formatMonth = (val: any) => {
                if (typeof val === 'string' && val.length > 7) {
                    return val.substring(0, 7); // Convierte "2024-05-01T..." a "2024-05"
                }
                return val;
            };

            // leer las propiedades que vienen del servidor (CTZ_Files4) pero no están en la interfaz.
            const raw = selectedMaterial as any;

            // =======================================================
            // 👇 CÁLCULO INICIAL DE SURFACE (AL CARGAR)
            // =======================================================
            let computedSurface = selectedMaterial.Surface || ""; // Valor por defecto

            // Obtenemos el ID del material actual
            const currentMatTypeId = selectedMaterial.ID_Material_type;

            // Si hay un ID y tenemos la lista, recalculamos la regla
            if (currentMatTypeId && lists.materialTypes) {
                // Buscamos el objeto en la lista usando '==' para flexibilidad (string/number)
                const matOption = lists.materialTypes.find((item: any) => item.Value == currentMatTypeId);

                if (matOption && matOption.Text) {
                    // Aplicamos la misma regla: Si empieza con "E" -> Exposed
                    if (matOption.Text.trim().startsWith("E")) {
                        computedSurface = "Exposed";
                    } else {
                        computedSurface = "Unexposed";
                    }
                }
            }
            // =======================================================
            // Parsear Welded Plates AQUÍ MISMO para asegurar que estén listos
            let hydratedPlates = [];
            try {
                const jsonStr = (selectedMaterial as any).WeldedPlatesJson;
                if (jsonStr && jsonStr !== '[]') {
                    hydratedPlates = JSON.parse(jsonStr);
                }
            } catch (e) {
                console.error("Error parsing welded plates on load", e);
            }

            console.log("🔍 DIAGNÓSTICO COIL DATA FILE:");
            console.log("ID del archivo:", raw.ID_File_CoilDataAdditional);
            console.log("Objeto CTZ_Files7 completo:", raw.CTZ_Files7);
            console.log("Nombre plano enviado de C#:", raw.FileName_CoilDataAdditional);

            // A. Llenar el formulario con lo que hay en BD (Tu lógica original)
            setFormData({
                ...selectedMaterial,
                _weldedPlates: hydratedPlates,
                Max_Production_Factor: selectedMaterial.Max_Production_Factor ?? 100,
                Real_SOP: formatMonth(selectedMaterial.Real_SOP),
                Real_EOP: formatMonth(selectedMaterial.Real_EOP),
                Surface: computedSurface,

                //IMPORTANTE: DEBE COINCIDIR CON: fileNameProp

                // 0. CAD Drawing (CTZ_Files)
                CADFileName: raw.CTZ_Files?.Name || raw.CADFileName,
                // 1. Packaging (CTZ_Files1)
                FileName_Packaging: raw.CTZ_Files1?.Name || raw.FileName_Packaging,
                // 2. Interplant Packaging (CTZ_Files2)
                InterplantPackagingFileName: raw.CTZ_Files2?.Name || raw.InterplantPackagingFileName,
                // 3. Interplant Outbound Freight (CTZ_Files3)
                FileName_InterplantOutboundFreight: raw.CTZ_Files3?.Name || raw.FileName_InterplantOutboundFreight,
                // 4. Technical Sheet (CTZ_Files4)
                technicalSheetFileName: raw.CTZ_Files4?.Name || raw.technicalSheetFileName,
                // 5. Additional File - Blank Data (CTZ_Files5)
                AdditionalFileName: raw.CTZ_Files5?.Name || raw.AdditionalFileName,
                // 6. Arrival Additional (CTZ_Files6) - ¡ESTE ERA TU CASO!
                arrivalAdditionalFileName: raw.CTZ_Files6?.Name || raw.arrivalAdditionalFileName,
                // 7. Coil Data Additional (CTZ_Files7)
                coilDataAdditionalFileName: raw.CTZ_Files7?.Name || raw.coilDataAdditionalFileName,
                // 8. Slitter Data Additional (CTZ_Files8)
                FileName_SlitterDataAdditional: raw.CTZ_Files8?.Name || raw.FileName_SlitterDataAdditional,
                // 9. Volume Additional (CTZ_Files9)    
                FileName_VolumeAdditional: raw.CTZ_Files9?.Name || raw.FileName_VolumeAdditional,
                // 10. Outbound Freight Additional (CTZ_Files10)
                FileName_OutboundFreightAdditional: raw.CTZ_Files10?.Name || raw.FileName_OutboundFreightAdditional,
                // 11. Delivery Packaging Additional (CTZ_Files11)
                FileName_DeliveryPackagingAdditional: raw.CTZ_Files11?.Name || raw.FileName_DeliveryPackagingAdditional,

                ['_projectPlantId']: projectPlantId,
                ['_projectInterplantActive']: interplantProcess,
                ['_projectStatusId']: projectStatusId
            } as any);

            // B. HIDRATACIÓN DE METADATA (INSTANTÁNEA)
            ['Vehicle', 'Vehicle_2', 'Vehicle_3', 'Vehicle_4'].forEach(field => {
                const vehicleName = (selectedMaterial as any)[field];
                // Necesitamos saber el país para buscar en el diccionario maestro
                // Buscamos la configuración del campo para saber cuál es su 'countryFieldName'
                const fieldConfig = safeFields.find(f => f.name === field);
                const countryField = fieldConfig?.countryFieldName;
                const countryName = countryField ? (selectedMaterial as any)[countryField] : null;

                if (vehicleName && countryName) {
                    const vehicleData = getVehicleDetailsLocal(vehicleName, countryName);

                    if (vehicleData) {
                        setVehicleMetadata(prev => ({
                            ...prev,
                            [field]: {
                                Program: vehicleData.Program,
                                SOP: vehicleData.SOP,
                                EOP: vehicleData.EOP,
                                ProductionDataJson: vehicleData.ProductionDataJson
                            }
                        }));
                    }
                }
            });

        } else {
            // Lógica para nuevo material (reset)
            setFormData({
                Max_Production_Factor: 100,
                isRunningChange: false,
                isCarryOver: false,
                ['_projectPlantId']: projectPlantId,
                ['_projectInterplantActive']: interplantProcess,
                ['_projectStatusId']: projectStatusId
            } as any);
            setVehicleMetadata({}); // Limpiar metadata anterior
        }
    }, [selectedMaterial, interplantProcess]);

    // 2. Calcular Programas y Fechas Globales
    useEffect(() => {
        const vehicleFields = ['Vehicle', 'Vehicle_2', 'Vehicle_3'];
        const programsFound: string[] = [];
        const sopsFound: string[] = [];
        const eopsFound: string[] = [];

        vehicleFields.forEach(fieldName => {
            const hasValue = (formData as any)[fieldName];
            const meta = vehicleMetadata[fieldName];
            if (hasValue && meta) {
                if (meta.Program) programsFound.push(meta.Program);
                if (meta.SOP) sopsFound.push(meta.SOP);
                if (meta.EOP) eopsFound.push(meta.EOP);
            }
        });

        const uniquePrograms = Array.from(new Set(programsFound));
        const resultProgram = uniquePrograms.join(" / ");

        let resultSOP = "";
        if (sopsFound.length > 0) {
            sopsFound.sort();
            resultSOP = sopsFound[0];
        }

        let resultEOP = "";
        if (eopsFound.length > 0) {
            eopsFound.sort();
            resultEOP = eopsFound[eopsFound.length - 1];
        }

        setFormData(prev => {
            const current = prev as any;
            if (
                current.Program_SP !== resultProgram ||
                current.SOP_SP !== resultSOP ||
                current.EOP_SP !== resultEOP
            ) {
                return {
                    ...prev,
                    Program_SP: resultProgram,
                    SOP_SP: resultSOP,
                    EOP_SP: resultEOP
                };
            }
            return prev;
        });

    }, [formData, vehicleMetadata]);

    // 3. Calcular Producción
    useEffect(() => {
        const vehicleFields = ['Vehicle', 'Vehicle_2', 'Vehicle_3', 'Vehicle_4'];
        const productionByYear: Record<string, number> = {};

        vehicleFields.forEach(fieldName => {
            const hasValue = (formData as any)[fieldName];
            const meta = vehicleMetadata[fieldName];
            if (hasValue && meta && meta.ProductionDataJson) {
                try {
                    const prodData = JSON.parse(meta.ProductionDataJson);
                    if (Array.isArray(prodData)) {
                        prodData.forEach((item: any) => {
                            const year = item.Production_Year;
                            const amount = parseFloat(item.Production_Amount) || 0;
                            if (year) {
                                productionByYear[year] = (productionByYear[year] || 0) + amount;
                            }
                        });
                    }
                } catch (e) {
                    console.error(`Error parsing JSON for ${fieldName}`, e);
                }
            }
        });

        let maxProd = 0;
        const realSOP = (formData as any)['Real_SOP'];
        const realEOP = (formData as any)['Real_EOP'];
        const isCarryOver = (formData as any)['IsRunningChange'] === true;

        if (isCarryOver && realSOP && realEOP) {
            const startYear = parseInt(realSOP.substring(0, 4)) || 0;
            const endYear = parseInt(realEOP.substring(0, 4)) || 9999;
            Object.entries(productionByYear).forEach(([yearStr, amount]) => {
                const year = parseInt(yearStr);
                if (year >= startYear && year <= endYear) {
                    if (amount > maxProd) maxProd = amount;
                }
            });
        } else {
            Object.values(productionByYear).forEach(amount => {
                if (amount > maxProd) maxProd = amount;
            });
        }

        if ((formData as any)['Max_Production_SP'] !== maxProd) {
            setFormData(prev => ({ ...prev, Max_Production_SP: maxProd }));
        }

    }, [formData, vehicleMetadata]);

    // 4. Calcular Efectivo
    useEffect(() => {
        const maxSP = parseFloat((formData as any)['Max_Production_SP']) || 0;
        const factorRaw = (formData as any)['Max_Production_Factor'];
        const factor = (factorRaw === undefined || factorRaw === null || factorRaw === "") ? 100 : parseFloat(factorRaw);
        const effective = Math.round(maxSP * (factor / 100));

        if ((formData as any)['Max_Production_Effective'] !== effective) {
            setFormData(prev => ({ ...prev, Max_Production_Effective: effective }));
        }
    }, [
        (formData as any)['Max_Production_SP'],
        (formData as any)['Max_Production_Factor']
    ]);

    // -------------------------------------------------------------
    // EFECTO: CÁLCULO DE PESO TEÓRICO (updateTheoreticalGrossWeight)
    // -------------------------------------------------------------
    useEffect(() => {
        const thickness = parseFloat((formData as any)['Thickness']);
        const width = parseFloat((formData as any)['Width']);
        const pitch = parseFloat((formData as any)['Pitch']);
        const blanksPerStroke = parseFloat((formData as any)['Blanks_Per_Stroke']);
        const routeId = parseInt((formData as any)['ID_Route']);
        const matTypeId = (formData as any)['ID_Material_type'];

        // Define tus IDs de rutas de Blanking (según tu vista legacy)
        // Ejemplo: const blankingRouteIds = [1, 2, 4, 5, 9, 10]; // Ajusta según tu sistema real
        // O mejor aún, pásalos en 'lists' desde el backend si es dinámico.
        // Asumiremos hardcode por ahora para replicar el ejemplo:
        const blankingRouteIds = [1, 2, 4, 5, 9, 10]; // ⚠️ VERIFICA ESTOS IDs EN TU DB

        if (blankingRouteIds.includes(routeId)) {
            // Buscamos el texto del tipo de material
            const matOption = lists.materialTypes?.find((m: any) => m.Value == matTypeId);
            const matText = matOption ? (matOption.Text || "").toUpperCase() : "";

            if (!isNaN(thickness) && !isNaN(width) && !isNaN(pitch) && !isNaN(blanksPerStroke) && blanksPerStroke > 0 && matText && matText !== "SELECT MATERIAL TYPE") {

                let density = 0;
                if (matText.includes("STEEL")) density = 7.85;
                else if (matText.includes("ALU")) density = 2.7;

                if (density > 0) {
                    const weight = (thickness * width * pitch * density) / 1000000 / blanksPerStroke;
                    const finalVal = parseFloat(weight.toFixed(3));

                    // Solo actualizamos si cambió para evitar loop infinito
                    if ((formData as any)['Theoretical_Gross_Weight'] !== finalVal) {
                        setFormData(prev => ({ ...prev, Theoretical_Gross_Weight: finalVal }));
                    }
                }
            } else {
                // Si faltan datos, limpiamos (si no está ya limpio)
                if ((formData as any)['Theoretical_Gross_Weight'] !== undefined && (formData as any)['Theoretical_Gross_Weight'] !== null) {
                    setFormData(prev => ({ ...prev, Theoretical_Gross_Weight: undefined }));
                }
            }
        } else {
            // Si no es ruta de blanking, limpiamos
            if ((formData as any)['Theoretical_Gross_Weight']) {
                setFormData(prev => ({ ...prev, Theoretical_Gross_Weight: undefined }));
            }
        }
    }, [
        (formData as any)['Thickness'],
        (formData as any)['Width'],
        (formData as any)['Pitch'],
        (formData as any)['Blanks_Per_Stroke'],
        (formData as any)['ID_Route'],
        (formData as any)['ID_Material_type']
    ]);

    // -------------------------------------------------------------
    // EFECTO: HIDRATACIÓN DE WELDED PLATES (JSON -> Array)
    // -------------------------------------------------------------
    /*
    useEffect(() => {
        if (selectedMaterial && (selectedMaterial as any).WeldedPlatesJson) {
            try {
                const jsonStr = (selectedMaterial as any).WeldedPlatesJson;
                if (jsonStr && jsonStr !== '[]') {
                    const parsed = JSON.parse(jsonStr);
                    // Actualizamos el estado con el array hidratado
                    setFormData(prev => ({
                        ...prev,
                        _weldedPlates: parsed,
                        // Aseguramos que el checkbox esté marcado si hay datos
                        IsWeldedBlank: true
                    }));
                }
            } catch (e) {
                console.error("Error parsing WeldedPlatesJson", e);
            }
        }
    }, [selectedMaterial]);
    */

    // -------------------------------------------------------------
    // EFECTO: CÁLCULOS DE VOLÚMENES Y TONELAJES (updateCalculatedWeightFields)
    // -------------------------------------------------------------
    useEffect(() => {
        // 1. Obtener valores (Helpers internos para evitar NaN)
        const getVal = (key: string) => {
            const v = parseFloat((formData as any)[key]);
            return (isNaN(v) || v < 0) ? 0 : v;
        };

        // Inputs Generales (Asumiendo que existen en el form)
        const annualVolume = getVal('Annual_Volume');
        const volumePerYear = getVal('Volume_Per_year');

        // Inputs de Blanking / Pesos
        const theoreticalGrossWeight = getVal('Theoretical_Gross_Weight');
        const grossWeight = getVal('Gross_Weight');
        const clientNetWeight = getVal('ClientNetWeight');
        const partsPerVehicle = getVal('Parts_Per_Vehicle');
        const blankingAnnualVolume = getVal('Blanking_Annual_Volume');

        // Determinar Scrap %
        const matTypeId = (formData as any)['ID_Material_type'];
        let scrapPercent = 0.015; // 1.5% default

        if (lists.materialTypes) {
            // Buscamos el texto usando '==' para string/number match
            const matOption = lists.materialTypes.find((m: any) => m.Value == matTypeId);
            if (matOption && matOption.Text && matOption.Text.trim().toUpperCase().startsWith("E ")) {
                scrapPercent = 0.025; // 2.5% Exposed
            }
        }

        // --- 2. CÁLCULO VOLUMEN ESTÁNDAR ---
        let weightPerPart = 0;
        let initialWeight = 0;
        let annualTonnage = 0;
        let shippingTons = 0;

        if (annualVolume > 0 && volumePerYear > 0) {
            weightPerPart = (volumePerYear / annualVolume) * 1000;
        }
        if (weightPerPart > 0) {
            initialWeight = weightPerPart * (1 + scrapPercent);
        }
        if (initialWeight > 0 && annualVolume > 0) {
            annualTonnage = (initialWeight * annualVolume) / 1000;
        }
        if (weightPerPart > 0 && annualVolume > 0) {
            shippingTons = (weightPerPart * annualVolume) / 1000;
        }

        // --- 3. CÁLCULO VOLUMEN BLANKING ---
        let blankingInitialWeightPerPart = 0;
        let initialWeightPerPart = 0;
        let blankingProcessTons = 0;
        let blankingShippingTons = 0;

        if (theoreticalGrossWeight > 0) {
            blankingInitialWeightPerPart = theoreticalGrossWeight * (1 + scrapPercent);
        }
        if (theoreticalGrossWeight > 0 && blankingInitialWeightPerPart > 0 && grossWeight > 0) {
            initialWeightPerPart = (blankingInitialWeightPerPart / theoreticalGrossWeight) * grossWeight;
        }
        if (initialWeightPerPart > 0 && blankingAnnualVolume > 0 && partsPerVehicle > 0) {
            blankingProcessTons = (initialWeightPerPart * blankingAnnualVolume * partsPerVehicle) / 1000;
        }

        const netWeightToUse = clientNetWeight > 0 ? clientNetWeight : grossWeight;
        if (netWeightToUse > 0 && blankingAnnualVolume > 0 && partsPerVehicle > 0) {
            blankingShippingTons = (netWeightToUse * blankingAnnualVolume * partsPerVehicle) / 1000;
        }

        // --- 4. ACTUALIZAR ESTADO (Solo si cambiaron) ---
        // Usamos una actualización funcional para evitar dependencias circulares innecesarias
        setFormData(prev => {
            const newData = { ...prev } as any;
            let changed = false;

            const update = (key: string, val: number) => {
                const fixedVal = parseFloat(val.toFixed(3));
                if (newData[key] !== fixedVal) {
                    newData[key] = fixedVal;
                    changed = true;
                }
            };

            // Standard Values (Si tienes estos campos en el form)
            update('WeightPerPart', weightPerPart);
            update('Initial_Weight', initialWeight);
            update('AnnualTonnage', annualTonnage);
            update('ShippingTons', shippingTons);

            // Blanking Values
            update('Blanking_InitialWeightPerPart', blankingInitialWeightPerPart);
            update('InitialWeightPerPart', initialWeightPerPart);
            update('Blanking_ProcessTons', blankingProcessTons);
            update('Blanking_ShippingTons', blankingShippingTons);

            return changed ? newData : prev;
        });

    }, [
        (formData as any)['Annual_Volume'],
        (formData as any)['Volume_Per_year'],
        (formData as any)['Theoretical_Gross_Weight'],
        (formData as any)['Gross_Weight'],
        (formData as any)['ClientNetWeight'],
        (formData as any)['Parts_Per_Vehicle'],
        (formData as any)['Blanking_Annual_Volume'],
        (formData as any)['ID_Material_type'],
        lists.materialTypes // Importante para detectar cambio de Exposed/Unexposed
    ]);


    // -------------------------------------------------------------
    // EFECTO: CÁLCULO DE INTERPLANT PACKAGE WEIGHT
    // -------------------------------------------------------------
    useEffect(() => {
        // 1. Obtener valores
        const pieces = parseFloat((formData as any)['InterplantPiecesPerPackage']) || 0;
        const stacks = parseFloat((formData as any)['InterplantStacksPerPackage']) || 0;

        const clientNetWeight = parseFloat((formData as any)['ClientNetWeight']) || 0;
        const grossWeight = parseFloat((formData as any)['Gross_Weight']) || 0;

        // Lógica de fallback: Si no hay neto, usa bruto
        const weightToUse = (clientNetWeight > 0) ? clientNetWeight : grossWeight;

        let calculatedWeight = 0;

        // 2. Calcular solo si todo es positivo
        if (pieces > 0 && stacks > 0 && weightToUse > 0) {
            calculatedWeight = parseFloat((pieces * stacks * weightToUse).toFixed(3));
        }

        // 3. Actualizar estado si cambió
        if ((formData as any)['InterplantPackageWeight'] !== calculatedWeight) {
            // Si el resultado es 0, preferimos dejarlo vacío o undefined para que el input se limpie
            // en lugar de mostrar "0.000" si faltan datos.
            const finalVal = calculatedWeight > 0 ? calculatedWeight : undefined;

            setFormData(prev => ({
                ...prev,
                InterplantPackageWeight: finalVal
            }));
        }

    }, [
        (formData as any)['InterplantPiecesPerPackage'],
        (formData as any)['InterplantStacksPerPackage'],
        (formData as any)['ClientNetWeight'],
        (formData as any)['Gross_Weight']
    ]);

    // -------------------------------------------------------------
    // EFECTO: CÁLCULO DE FINAL PACKAGE WEIGHT
    // -------------------------------------------------------------
    useEffect(() => {
        // 1. Obtener valores
        const pieces = parseFloat((formData as any)['PiecesPerPackage']) || 0;
        const stacks = parseFloat((formData as any)['StacksPerPackage']) || 0;

        const clientNetWeight = parseFloat((formData as any)['ClientNetWeight']) || 0;
        const grossWeight = parseFloat((formData as any)['Gross_Weight']) || 0;

        // Lógica de fallback: Si no hay neto, usa bruto
        const weightToUse = (clientNetWeight > 0) ? clientNetWeight : grossWeight;

        let calculatedWeight = 0;

        // 2. Calcular solo si todo es positivo
        if (pieces > 0 && stacks > 0 && weightToUse > 0) {
            calculatedWeight = parseFloat((pieces * stacks * weightToUse).toFixed(3));
        }

        // 3. Actualizar estado si cambió
        if ((formData as any)['PackageWeight'] !== calculatedWeight) {
            const finalVal = calculatedWeight > 0 ? calculatedWeight : undefined;

            setFormData(prev => ({
                ...prev,
                PackageWeight: finalVal
            }));
        }

    }, [
        (formData as any)['PiecesPerPackage'],
        (formData as any)['StacksPerPackage'],
        (formData as any)['ClientNetWeight'],
        (formData as any)['Gross_Weight']
    ]);

    // -------------------------------------------------------------
    // EFECTO: CÁLCULO DE THEORETICAL STROKES (AJAX)
    // -------------------------------------------------------------
    useEffect(() => {
        // Datos necesarios para el cálculo
        const productionLineId = (formData as any)['ID_Theoretical_Blanking_Line'];
        const pitch = parseFloat((formData as any)['Pitch']);
        const rotation = 0; // Hardcodeado a 0 por ahora según tu legacy

        // Validación básica antes de llamar
        if (!productionLineId || isNaN(pitch)) {
            // Si faltan datos, limpiamos y salimos
            setFormData(prev => ({
                ...prev,
                Theoretical_Strokes: undefined,
                Theoretical_Effective_Strokes: undefined
            }));
            return;
        }

        // Definimos una función asíncrona interna para el fetch
        const fetchStrokes = async () => {
            try {
                // Construimos la URL con parámetros query string
                const queryParams = new URLSearchParams({
                    productionLineId: productionLineId.toString(),
                    pitch: pitch.toString(),
                    rotation: rotation.toString()
                });

                const response = await fetch(`${urls.getTheoreticalStrokes}?${queryParams}`);
                const data = await response.json();

                if (data.success) {
                    const strokes = parseFloat(data.theoreticalStrokes);
                    const finalStrokes = parseFloat(strokes.toFixed(2));

                    // Actualizamos el estado con los Strokes y disparamos el cálculo de efectivos
                    setFormData(prev => {
                        // Cálculo local de efectivos (OEE * Strokes)
                        // Asumimos que OEE viene en formData o lo tienes en una constante.
                        // Si OEE está en el objeto de la línea, deberías haberlo guardado al seleccionar la línea.
                        // Usaremos 0.85 (85%) como ejemplo si no existe campo OEE, ajusta según tu lógica.
                        const oeeVal = (prev as any)['OEE'] || 85;
                        const oeeFactor = oeeVal / 100;

                        let effective = 0;
                        if (finalStrokes > 0 && oeeFactor > 0) {
                            effective = Math.round(finalStrokes * oeeFactor);
                        }

                        // Evitamos update si no hay cambios
                        if ((prev as any)['Theoretical_Strokes'] === finalStrokes &&
                            (prev as any)['Theoretical_Effective_Strokes'] === effective) {
                            return prev;
                        }

                        return {
                            ...prev,
                            Theoretical_Strokes: finalStrokes,
                            Theoretical_Effective_Strokes: effective
                        };
                    });
                } else {
                    console.warn("Server warning:", data.message);
                    // toast.warning(data.message); // Opcional
                }
            } catch (error) {
                console.error("Error fetching theoretical strokes:", error);
            }
        };

        // Debounce simple: esperamos 500ms después de que deje de escribir Pitch para llamar
        const timeoutId = setTimeout(() => {
            fetchStrokes();
        }, 500);

        return () => clearTimeout(timeoutId);

    }, [
        (formData as any)['ID_Theoretical_Blanking_Line'],
        (formData as any)['Pitch'],
        (formData as any)['OEE'] // Dependencia del OEE si es dinámico
    ]);

    // -------------------------------------------------------------
    // EFECTO: CÁLCULO DE REAL STROKES (AJAX)
    // -------------------------------------------------------------
    useEffect(() => {
        // Datos necesarios
        const realLineId = (formData as any)['ID_Real_Blanking_Line'];
        const pitch = parseFloat((formData as any)['Pitch']);
        const rotation = 0; // Hardcodeado 0

        // Validación: Si no hay línea seleccionada o pitch, limpiar y salir
        if (!realLineId || isNaN(pitch) || realLineId === 0) {
            setFormData(prev => {
                if (!prev['Real_Strokes'] && !prev['Real_Effective_Strokes']) return prev;
                return {
                    ...prev,
                    Real_Strokes: undefined,
                    Real_Effective_Strokes: undefined
                };
            });
            return;
        }

        const fetchRealStrokes = async () => {
            try {
                const queryParams = new URLSearchParams({
                    productionLineId: realLineId.toString(),
                    pitch: pitch.toString(),
                    rotation: rotation.toString()
                });

                // Reutilizamos el MISMO endpoint que el teórico
                const response = await fetch(`${urls.getTheoreticalStrokes}?${queryParams}`);
                const data = await response.json();

                if (data.success) {
                    const strokes = parseFloat(data.theoreticalStrokes); // El endpoint devuelve "theoreticalStrokes" como nombre de propiedad
                    const finalStrokes = parseFloat(strokes.toFixed(2));

                    setFormData(prev => {
                        // Cálculo local de efectivos
                        const oeeVal = (prev as any)['OEE'] || 85;
                        const oeeFactor = oeeVal / 100;

                        let effective = 0;
                        if (finalStrokes > 0 && oeeFactor > 0) {
                            effective = Math.round(finalStrokes * oeeFactor);
                        }

                        // Evitar loop si no cambió
                        if ((prev as any)['Real_Strokes'] === finalStrokes &&
                            (prev as any)['Real_Effective_Strokes'] === effective) {
                            return prev;
                        }

                        return {
                            ...prev,
                            Real_Strokes: finalStrokes,
                            Real_Effective_Strokes: effective
                        };
                    });
                }
            } catch (error) {
                console.error("Error fetching real strokes:", error);
            }
        };

        // Debounce para no saturar si cambia el Pitch rápido
        const timeoutId = setTimeout(() => {
            fetchRealStrokes();
        }, 500);

        return () => clearTimeout(timeoutId);

    }, [
        (formData as any)['ID_Real_Blanking_Line'], // Trigger principal
        (formData as any)['Pitch'],                 // Trigger secundario
        (formData as any)['OEE']                    // Trigger recálculo
    ]);

    // 1. Efecto para Cálculo de Línea Teórica
    useEffect(() => {
        // ... (Tu lógica de guard para rutas blanking igual que antes) ...
        const blankingRoutes = [ROUTES.BLK, ROUTES.BLK_RP, ROUTES.BLK_RPLTZ, ROUTES.BLK_SH, ROUTES.BLK_WLD, ROUTES.SLT_BLK, ROUTES.SLT_BLK_WLD];
        const currentRouteId = formData.ID_Route ?? 0;

        if (!blankingRoutes.includes(currentRouteId)) return;

        // Ejecutar cálculo
        const result = calculateTheoreticalLine(formData, lists.theoreticalRules || []);
        const hasRealLine = !!formData.ID_Real_Blanking_Line;

        if (result) {
            // A. CASO: SE ENCONTRÓ UNA LÍNEA COMPATIBLE
            if (formData.ID_Theoretical_Blanking_Line !== result.lineId) {

                console.log("🔄 Updating Theoretical Line:", result.lineName);

                setFormData(prev => ({
                    ...prev,
                    ID_Theoretical_Blanking_Line: result.lineId
                }));

                // 👇 LÓGICA DE TOAST INTELIGENTE
                if (hasRealLine) {
                    // 1. Buscamos el nombre amigable en la lista de líneas
                    // Usamos '==' para que coincida aunque uno sea string y el otro number
                    const realLineObj = lists.linesList?.find((l: any) => l.Value == formData.ID_Real_Blanking_Line);
                    const realLineName = realLineObj ? realLineObj.Text : "Real Line";

                    toast.info(
                        <div>
                            Theoretical Line updated to <strong>{result.lineName}</strong>.<br />
                            <small>Note: Validation is using <strong>{realLineName}</strong>.</small>
                        </div>,
                        { position: "top-right", autoClose: 5000, hideProgressBar: true }
                    );
                } else {
                    toast.info(
                        <div>
                            Selected Theoretical Line: <strong>{result.lineName}</strong>
                        </div>,
                        { position: "top-right", autoClose: 5000, hideProgressBar: true, transition: Slide }
                    );
                }
            }
        } else {
            // B. CASO: NO HUBO MATCH
            if (formData.ID_Theoretical_Blanking_Line) {
                console.log("⚠️ No matching theoretical line found. Clearing selection.");

                setFormData(prev => ({
                    ...prev,
                    ID_Theoretical_Blanking_Line: undefined
                }));

                // Solo avisamos si NO hay línea real, para no molestar si ya está usando la real.
                if (!hasRealLine) {
                    toast.warn(
                        "Theoretical Line cleared (No match found).",
                        { position: "top-right", autoClose: 4000, hideProgressBar: true, transition: Slide }
                    );
                }
            }
        }

    }, [
        formData.ID_Route,
        formData.ID_Material_type,
        formData.Thickness,
        formData.Width,
        formData.Pitch,
        formData.Tensile_Strenght,
        formData.ID_Real_Blanking_Line, // 👈 IMPORTANTE: Agregar esta dependencia para que detecte cambios aquí
        lists.theoreticalRules
    ]);

    // CÁLCULO DE RANGOS DE INGENIERÍA ACTIVOS
    const currentEngineeringRanges = useMemo(() => {
        // 1. Validar que la lista maestra exista
        if (!lists.engineeringRanges) return [];

        // 2. Obtener y CONVERTIR inputs (Para evitar error String vs Number)
        const rawReal = (formData as any)['ID_Real_Blanking_Line'];
        const rawTheo = (formData as any)['ID_Theoretical_Blanking_Line'];
        const rawMat = (formData as any)['ID_Material_type'];

        const realLineId = Number(rawReal);       // Convierte "5" -> 5
        const theoreticalLineId = Number(rawTheo);// Convierte "5" -> 5
        const materialTypeId = Number(rawMat);    // Convierte "2" -> 2

        // 3. Lógica de Decisión: ¿Quién manda?
        // Si hay línea Real válida (mayor a 0), ella manda. Si no, la Teórica.
        let activeLineId = 0;
        let sourceUsed = "";

        if (realLineId && realLineId !== 0 && !isNaN(realLineId)) {
            activeLineId = realLineId;
            sourceUsed = "REAL LINE (Prioridad)";
        } else {
            activeLineId = theoreticalLineId;
            sourceUsed = "THEORETICAL LINE (Fallback)";
        }

        // 4. Filtrado Seguro
        // Usamos activeLineId que ya es NUMBER seguro
        const results = lists.engineeringRanges.filter((r: any) =>
            r.lineId === activeLineId &&
            r.materialTypeId === materialTypeId
        );

        // --- 🕵️‍♂️ DEBUG DETALLADO (Mirar Consola) ---
        console.groupCollapsed(`🔧 Debug Rangos: Usando ${sourceUsed}`);
        console.log(`1. Inputs Crudos: Real="${rawReal}", Theo="${rawTheo}", Mat="${rawMat}"`);
        console.log(`2. Inputs Number: Real=${realLineId}, Theo=${theoreticalLineId}, Mat=${materialTypeId}`);
        console.log(`3. ID Línea Activa: ${activeLineId}`);

        if (results.length === 0) {
            console.warn(`⚠️ ALERTA: No se encontraron rangos para Línea ID ${activeLineId} y Material ID ${materialTypeId}`);
            console.log("   ¿Existe esta combinación en la BD (tabla CTZ_Technical_Information_Line)?");
            // Intento de ayuda: Buscar si existe la línea en la lista, aunque sea con otro material
            const lineExists = lists.engineeringRanges.some((r: any) => r.lineId === activeLineId);
            console.log(`   ¿Existen datos para la línea ${activeLineId} con CUALQUIER material? ${lineExists ? 'SÍ' : 'NO'}`);
        } else {
            console.log(`✅ ÉXITO: Se encontraron ${results.length} reglas de validación.`);
        }
        console.groupEnd();
        // ---------------------------------------------

        return results;

    }, [
        (formData as any)['ID_Real_Blanking_Line'],
        (formData as any)['ID_Theoretical_Blanking_Line'],
        (formData as any)['ID_Material_type'],
        lists.engineeringRanges
    ]);

    // -------------------------------------------------------------
    // EFECTO: RE-VALIDAR INPUTS CUANDO CAMBIAN LOS RANGOS
    // Si cambio de línea, los valores actuales pueden volverse válidos o inválidos.
    // -------------------------------------------------------------
    useEffect(() => {
        // 1. Lista de campos que dependen de ingeniería
        const engineeringFields = [
            'Tensile_Strenght',
            'Tensile_Strength',
            'Thickness',
            'Width',
            'Pitch'
        ];

        // 2. Actualizamos el estado de errores
        setErrors(prevErrors => {
            const newErrors = { ...prevErrors };
            let hasChanges = false;

            engineeringFields.forEach(fieldName => {
                // Obtenemos el valor actual del formulario
                const currentValue = (formData as any)[fieldName];

                // Solo validamos si hay un valor escrito (para no llenar de rojos campos vacíos)
                if (currentValue !== undefined && currentValue !== null && currentValue !== "") {

                    // Ejecutamos la validación compleja con los NUEVOS rangos
                    const result = validateComplexRules(fieldName, currentValue, formData);

                    // A. Si ahora hay error
                    if (result.error) {
                        if (newErrors[fieldName] !== result.error) {
                            newErrors[fieldName] = result.error;
                            hasChanges = true;
                        }
                    }
                    // B. Si ya no hay error (se arregló al cambiar de máquina)
                    else {
                        if (newErrors[fieldName]) {
                            delete newErrors[fieldName];
                            hasChanges = true;
                        }
                    }
                }
            });

            // Solo actualizamos el estado si hubo cambios reales para evitar render loops
            return hasChanges ? newErrors : prevErrors;
        });

        // Opcional: Si manejas warnings también, replica la lógica para setWarnings aquí.

    }, [currentEngineeringRanges]); // 👈 SE DISPARA CUANDO CAMBIAN LOS RANGOS

    // 1. NUEVA FUNCIÓN MEJORADA: Soporta múltiples condiciones (AND)
    const isFieldActive = (field: FieldConfig, contextData: any) => {
        // Si no hay reglas de visibilidad, el campo está activo por defecto
        if (!field.visibleWhen) return true;

        // 1. Normalizamos: Si es un objeto único, lo convertimos en un array de 1 elemento.
        // Así tratamos todo como una lista de reglas.
        const rules = Array.isArray(field.visibleWhen) ? field.visibleWhen : [field.visibleWhen];

        // 2. Verificamos que se cumplan TODAS las reglas (every = AND)
        // Si quisieras lógica OR, usarías .some()
        return rules.every(rule => {
            const dependencyName = rule.field;
            const dependencyValue = (contextData as any)[dependencyName];

            // Regla A: "is" -> El valor debe estar en la lista permitida
            if (rule.is) {
                // Caso especial: Si el valor del formulario es un array (ej. Checkbox Group)
                if (Array.isArray(dependencyValue)) {
                    // Verificamos si hay intersección (si alguno de los valores seleccionados está permitido)
                    return rule.is.some((reqVal: any) => dependencyValue.includes(reqVal));
                }
                // Caso normal: Valor simple (Select, Radio, Text)
                else {
                    return rule.is.includes(dependencyValue);
                }
            }

            // Regla B: "hasValue" -> Solo verifica que no esté vacío
            if (rule.hasValue) {
                if (dependencyValue === null || dependencyValue === undefined || dependencyValue === "") return false;
                if (Array.isArray(dependencyValue) && dependencyValue.length === 0) return false;
            }

            return true;
        });
    };

    // 2. MODIFICAMOS LA FUNCIÓN EXISTENTE para usar la lógica base
    const isFieldVisible = (field: FieldConfig, contextData: any) => {
        // Primero: ¿Es la pestaña correcta?
        if (field.section !== activeTab) return false;
        // Segundo: ¿Cumple las reglas lógicas?
        return isFieldActive(field, contextData);
    };

    // Ahora recibe 'allData' para poder evaluar reglas que dependen de otros campos (como el estatus)
    const validateField = (name: string, value: any, allData: any) => {
        const fieldConfig = safeFields.find(f => f.name === name);

        // Si no existe configuración o está deshabilitado, no validamos
        if (!fieldConfig || fieldConfig.disabled) return "";

        const rules = fieldConfig.validation;
        if (!rules) return "";

        // 1. Determinar si el campo es obligatorio
        let isRequired = rules.required || false;

        // --- LÓGICA REQUIRED CONDICIONAL ---
        if (rules.requiredWhen) {
            const dependencyName = rules.requiredWhen.field; // ej: '_projectStatusId'
            const dependencyValue = allData[dependencyName];

            // Si el valor de la dependencia está en la lista 'is', activamos Required
            if (rules.requiredWhen.is && rules.requiredWhen.is.includes(dependencyValue)) {
                isRequired = true;
            }
        }

        // 2. Validar si está vacío
        if (isRequired) {
            // A. Leemos la configuración directa del campo
            // fieldConfig ya lo tienes definido al inicio de esta función: 
            // const fieldConfig = safeFields.find(f => f.name === name);

            const isZeroAllowed = fieldConfig?.allowZero || false;

            // B. Definición de "Vacío":
            // - null, undefined o "" siempre son vacío.
            // - 0 es vacío SOLO SI allowZero es false (o undefined).
            const isEmpty = value === null || value === undefined || value === "" || (!isZeroAllowed && value === 0);

            if (isEmpty) {
                return rules.customMessage || `${fieldConfig?.label} is required.`;
            }
        }

        // 3. Validaciones estándar (Longitud, Patrón)
        if (value) { // Solo validamos formato si hay valor
            if (rules.maxLength && value.toString().length > rules.maxLength) {
                return `${fieldConfig.label} cannot exceed ${rules.maxLength} characters.`;
            }
            if (rules.pattern && !rules.pattern.test(value.toString())) {
                return `Invalid format for ${fieldConfig.label}.`;
            }
            // Validaciones numéricas
            if (rules.min !== undefined && Number(value) < rules.min) {
                return `${fieldConfig.label} must be at least ${rules.min}.`;
            }
            if (rules.max !== undefined && Number(value) > rules.max) {
                return `${fieldConfig.label} cannot be greater than ${rules.max}.`;
            }
            // Verificamos si el campo tiene definida la propiedad 'decimals'
            if (fieldConfig.type === 'number' && fieldConfig.decimals !== undefined) {
                const valStr = value.toString();

                // Si tiene punto decimal, contamos cuántos números hay después
                if (valStr.includes('.')) {
                    const decimalPart = valStr.split('.')[1];
                    if (decimalPart.length > fieldConfig.decimals) {
                        return `Invalid value. Maximum ${fieldConfig.decimals} decimal places allowed.`;
                    }
                }
            }
        }

        // Lógica específica para Real Blanking Line
        if (name === 'ID_Real_Blanking_Line') {
            // Si el usuario es ingeniero, el campo es obligatorio
            // (Asegúrate de tener acceso a la variable canEditEngineering aquí)
            //if (canEditEngineering) { 
            //    if (!value || value === "0" || value === 0) {
            //        return "Real blanking line is required.";
            //    }
            //}
        }

        // Validación Condicional para Ingeniería
        if (['Ideal_Cycle_Time_Per_Tool', 'OEE'].includes(name)) {
            /* 
             if (canEditEngineering) {
                 if (value === null || value === undefined || value === "") {
                     return `${fieldConfig?.label} is required.`;
                 }
             }
             */
        }

        return "";
    };

    type ValidationResult = { error: string; warning: string };

    const validateComplexRules = (name: string, value: any, allData: any): ValidationResult => {
        let result = { error: "", warning: "" };
        const now = new Date();
        const currentYear = now.getFullYear();
        const currentMonth = now.getMonth() + 1;

        if (name === 'Real_SOP' || name === 'Real_EOP') {
            if (!value) {
                result.error = `${name.replace('_', ' ')} is required.`;
                return result;
            }
            const [yearStr, monthStr] = value.toString().split('-');
            const inputYear = parseInt(yearStr);
            const inputMonth = parseInt(monthStr);

            if (inputYear > 2099) {
                result.error = "Year cannot be greater than 2099.";
                return result;
            }

            if (name === 'Real_SOP') {
                const minAllowedYear = currentYear - 2;
                if (inputYear < minAllowedYear) {
                    result.error = `Date cannot be older than ${minAllowedYear}.`;
                }
                else if (inputYear < currentYear || (inputYear === currentYear && inputMonth < currentMonth)) {
                    result.warning = "Date is in the past.";
                }
            }

            if (name === 'Real_EOP') {
                const sopValue = allData['Real_SOP'];
                if (sopValue) {
                    if (value < sopValue) {
                        result.error = "Real EOP cannot be before Real SOP.";
                    }
                }
            }
        }

        // Relación Tolerancia vs Espesor
        if (name === 'ThicknessToleranceNegative' || name === 'ThicknessTolerancePositive') {
            const thickness = parseFloat(allData['Thickness']);

            // Solo validamos si hay un espesor definido y es un número válido
            if (!isNaN(thickness) && value !== null && value !== undefined && value !== "") {
                const tolVal = parseFloat(value);

                if (Math.abs(tolVal) > thickness) {
                    result.error = `Absolute value cannot exceed Thickness (${thickness} mm).`;
                }
            }
        }

        // Lo mismo para Width si lo necesitas después
        if (name === 'WidthToleranceNegative' || name === 'WidthTolerancePositive') {
            const width = parseFloat(allData['Width']);
            if (!isNaN(width) && value !== null && value !== undefined && value !== "") {
                const tolVal = parseFloat(value);
                if (Math.abs(tolVal) > width) {
                    result.error = `Absolute value cannot exceed Width (${width} mm).`;
                }
            }
        }

        // ---------------------------------------------------------
        // LÓGICA COIL DATA (Pesos y Diámetros)
        // ---------------------------------------------------------

        // Acceso a Engineering Ranges (Asumiendo que están en window como en legacy, 
        // o pásalos en 'lists' si ya migraste esa parte).
        const ranges = (window as any).engineeringRanges || [];
        // NOTA: Si 'ranges' viene en props.lists, usa: const ranges = lists.engineeringRanges || [];


        // C) Inner Coil Diameter (Criterio 15 + Regla 508/610)
        if (name === 'InnerCoilDiameterArrival' && value) {
            // 1. VALIDACIÓN DE FORMATO NUMÉRICO ESTRICTO
            if (isNaN(Number(value))) {
                result.error = "Value must be a valid number.";
            } else {
                // 2. Lógica de Negocio
                const val = parseFloat(value);
                const criterio = ranges.find((r: any) => r.ID_Criteria === 15);

                if (criterio) {
                    // Caso 1: Valor exacto definido en DB
                    if (criterio.NumericValue != null) {
                        if (val !== criterio.NumericValue) {
                            result.error = `Value must be exactly ${criterio.NumericValue}.`;
                        }
                    }
                    // Caso 2: Rango definido (Extremos solamente)
                    else if (criterio.MinValue != null && criterio.MaxValue != null) {
                        if (val !== criterio.MinValue && val !== criterio.MaxValue) {
                            result.error = `Value must be either ${criterio.MinValue} or ${criterio.MaxValue}.`;
                        }
                    }
                } else {
                    // Caso 3: NO hay criterio en DB -> Regla Hardcodeada Legacy
                    if (val !== 508 && val !== 610) {
                        result.error = "Value must be 508 or 610.";
                    }
                }
            }
        }

        // ---------------------------------------------------------
        // LÓGICA SLITTER DATA (Delivery)
        // ---------------------------------------------------------

        // A) Inner Coil Diameter Delivery (Misma lógica que Arrival - Criterio 15)
        if (name === 'InnerCoilDiameterDelivery' && value) {
            const val = parseFloat(value);
            const criterio = ranges.find((r: any) => r.ID_Criteria === 15); // ID 15

            if (criterio) {
                // Caso 1: Valor exacto
                if (criterio.NumericValue != null) {
                    if (val !== criterio.NumericValue) {
                        result.error = `Value must be exactly ${criterio.NumericValue}.`;
                    }
                }
                // Caso 2: Extremos válidos (Legacy logic: val !== min && val !== max)
                else if (criterio.MinValue != null && criterio.MaxValue != null) {
                    if (val !== criterio.MinValue && val !== criterio.MaxValue) {
                        result.error = `Value must be either ${criterio.MinValue} or ${criterio.MaxValue}.`;
                    }
                }
            } else {
                // Caso 3: Default Hardcodeado (508 o 610)
                if (val !== 508 && val !== 610) {
                    result.error = "Value must be 508 or 610.";
                }
            }
        }

        // B) Outer Coil Diameter Delivery (Misma lógica que Arrival - Criterio 16)
        if (name === 'OuterCoilDiameterDelivery' && value) {
            const val = parseFloat(value);
            const criterio = ranges.find((r: any) => r.ID_Criteria === 16); // ID 16

            if (criterio) {
                if (criterio.MinValue != null && criterio.MaxValue != null) {
                    // Validamos que esté DENTRO del rango (según interpretación estándar de min/max)
                    // Nota: Si tu legacy validaba "endpoints" para outer también, ajusta a '!=='
                    // pero usualmente Outer es un rango continuo.
                    if (val < criterio.MinValue || val > criterio.MaxValue) {
                        result.error = `Value must be between ${criterio.MinValue} and ${criterio.MaxValue}.`;
                    }
                }
            }
        }

        // D) Mults Width Tolerances (Legacy: validateWidth_Mults_Tol_Pos / Neg)
        if (name === 'Width_Mults_Tol_Neg' || name === 'Width_Mults_Tol_Pos') {
            const widthVal = parseFloat(allData['Width_Mults']);

            // Solo validamos si hay un ancho definido
            if (!isNaN(widthVal) && value !== null && value !== undefined && value !== "") {
                const tolVal = parseFloat(value);

                // La tolerancia (en valor absoluto) no puede ser mayor que el ancho mismo
                if (Math.abs(tolVal) > widthVal) {
                    result.error = `Value cannot exceed Width (${widthVal}).`;
                }
            }
        }

        // E) Weight of Final Mults - Validación Cruzada (Min <= Optimal <= Max)
        if (['WeightOfFinalMults', 'WeightOfFinalMults_Min', 'WeightOfFinalMults_Max'].includes(name)) {
            const min = parseFloat(allData['WeightOfFinalMults_Min']);
            const opt = parseFloat(allData['WeightOfFinalMults']);
            const max = parseFloat(allData['WeightOfFinalMults_Max']);

            // Validar Max
            if (name === 'WeightOfFinalMults_Max' && !isNaN(max)) {
                if (!isNaN(opt) && max < opt) result.error = "Max cannot be less than Optimal.";
                if (!isNaN(min) && max < min) result.error = "Max cannot be less than Min.";
            }

            // Validar Min
            if (name === 'WeightOfFinalMults_Min' && !isNaN(min)) {
                if (!isNaN(opt) && min > opt) result.error = "Min cannot be greater than Optimal.";
                if (!isNaN(max) && min > max) result.error = "Min cannot be greater than Max.";
            }

            // Validar Optimal
            if (name === 'WeightOfFinalMults' && !isNaN(opt)) {
                if (!isNaN(min) && opt < min) result.error = "Optimal cannot be less than Min.";
                if (!isNaN(max) && opt > max) result.error = "Optimal cannot be greater than Max.";
            }
        }

        // F) Validación de Combinación: Peso Total vs Master Coil
        // (Se activa al cambiar Max, Multipliers o MasterCoilWeight)
        // Legacy: validateWeightMultsCombination
        if (name === 'WeightOfFinalMults_Max' || name === 'Multipliers' || name === 'MasterCoilWeight') {
            // Solo mostramos el error en el campo Max (como en legacy) o en el actual si es Max

            // Obtenemos valores (usando el MAX para el cálculo de seguridad)
            const weightMax = parseFloat(name === 'WeightOfFinalMults_Max' ? value : allData['WeightOfFinalMults_Max']);
            const mults = parseFloat(name === 'Multipliers' ? value : allData['Multipliers']);
            const masterWeight = parseFloat(name === 'MasterCoilWeight' ? value : allData['MasterCoilWeight']);

            if (!isNaN(weightMax) && !isNaN(mults) && !isNaN(masterWeight) &&
                weightMax > 0 && mults > 0 && masterWeight > 0) {

                const totalCalculated = weightMax * mults;

                if (totalCalculated > masterWeight) {
                    // Si estamos editando el campo Max, mostramos el error ahí
                    if (name === 'WeightOfFinalMults_Max') {
                        result.error = `Total max weight (${totalCalculated.toFixed(2)} kg) cannot exceed Master Coil (${masterWeight.toFixed(2)} kg).`;
                    }
                    // Nota: Si cambias Multipliers, el error debería aparecer ahí o en Max. 
                    // En este diseño, DynamicField valida campo por campo. 
                    // Si quieres que el error aparezca en Max cuando cambias Multipliers, 
                    // necesitaríamos un efecto secundario (triggerValidation), pero por ahora esto protege el guardado.
                }
            }
        }

        // G) Plates Width Tolerances (Legacy: Value cannot exceed Width)
        if (name === 'Width_Plates_Tol_Neg' || name === 'Width_Plates_Tol_Pos') {
            const widthVal = parseFloat(allData['Width_Plates']);
            if (!isNaN(widthVal) && value) {
                const tolVal = parseFloat(value);
                if (Math.abs(tolVal) > widthVal) {
                    result.error = `Value cannot exceed Width (${widthVal}).`;
                }
            }
        }

        // H) Pitch (Criterio 9)
        if (name === 'Pitch' && value) {
            const val = parseFloat(value);
            const criterio = ranges.find((r: any) => r.ID_Criteria === 9);

            if (criterio) {
                // Lógica estándar de validación de rango (copiar helper si es necesario)
                if (criterio.MinValue != null && criterio.MaxValue != null) {
                    if (val < criterio.MinValue || val > criterio.MaxValue) {
                        result.error = `Value must be between ${criterio.MinValue} and ${criterio.MaxValue}.`;
                    }
                }
            }
        }

        // J) Shearing Width Tolerances (Legacy: Tolerance cannot be greater than width)
        if (name === 'Shearing_Width_Tol_Neg' || name === 'Shearing_Width_Tol_Pos') {
            const widthVal = parseFloat(allData['Shearing_Width']);

            // Solo validamos si hay un ancho definido y el valor actual no está vacío
            if (!isNaN(widthVal) && value !== null && value !== undefined && value !== "") {
                const tolVal = parseFloat(value);

                // La tolerancia absoluta no puede superar el ancho
                if (Math.abs(tolVal) > widthVal) {
                    result.error = `Tolerance cannot be greater than width (${widthVal}).`;
                }
            }
        }

        // K) Shearing Pitch Tolerances
        if (name === 'Shearing_Pitch_Tol_Neg' || name === 'Shearing_Pitch_Tol_Pos') {
            const mainVal = parseFloat(allData['Shearing_Pitch']);
            if (!isNaN(mainVal) && value) {
                const tolVal = parseFloat(value);
                if (Math.abs(tolVal) > mainVal) {
                    result.error = `Tolerance cannot be greater than Pitch (${mainVal}).`;
                }
            }
        }

        // L) Shearing Weight Tolerances
        if (name === 'Shearing_Weight_Tol_Neg' || name === 'Shearing_Weight_Tol_Pos') {
            const mainVal = parseFloat(allData['Shearing_Weight']);
            if (!isNaN(mainVal) && value) {
                const tolVal = parseFloat(value);
                if (Math.abs(tolVal) > mainVal) {
                    result.error = `Tolerance cannot be greater than Weight (${mainVal}).`;
                }
            }
        }

        // M) Interplant Scrap Percentages (Min <= Optimal <= Max)
        // Solo validamos si los campos existen y tienen valor
        if (['InterplantScrapReconciliationPercent', 'InterplantScrapReconciliationPercent_Min', 'InterplantScrapReconciliationPercent_Max'].includes(name)) {
            const min = parseFloat(allData['InterplantScrapReconciliationPercent_Min']);
            const opt = parseFloat(allData['InterplantScrapReconciliationPercent']);
            const max = parseFloat(allData['InterplantScrapReconciliationPercent_Max']);

            if (name === 'InterplantScrapReconciliationPercent_Min' && !isNaN(min)) {
                if (!isNaN(opt) && min > opt) result.error = "Min cannot be greater than Optimal.";
                if (!isNaN(max) && min > max) result.error = "Min cannot be greater than Max.";
            }
            if (name === 'InterplantScrapReconciliationPercent' && !isNaN(opt)) {
                if (!isNaN(min) && opt < min) result.error = "Optimal cannot be less than Min.";
                if (!isNaN(max) && opt > max) result.error = "Optimal cannot be greater than Max.";
            }
            if (name === 'InterplantScrapReconciliationPercent_Max' && !isNaN(max)) {
                if (!isNaN(opt) && max < opt) result.error = "Max cannot be less than Optimal.";
                if (!isNaN(min) && max < min) result.error = "Max cannot be less than Min.";
            }
        }

        // N) Interplant Head/Tail Percentages (Min <= Optimal <= Max)
        if (['InterplantHeadTailReconciliationPercent', 'InterplantHeadTailReconciliationPercent_Min', 'InterplantHeadTailReconciliationPercent_Max'].includes(name)) {
            const min = parseFloat(allData['InterplantHeadTailReconciliationPercent_Min']);
            const opt = parseFloat(allData['InterplantHeadTailReconciliationPercent']);
            const max = parseFloat(allData['InterplantHeadTailReconciliationPercent_Max']);

            if (name === 'InterplantHeadTailReconciliationPercent_Min' && !isNaN(min)) {
                if (!isNaN(opt) && min > opt) result.error = "Min cannot be greater than Optimal.";
                if (!isNaN(max) && min > max) result.error = "Min cannot be greater than Max.";
            }
            if (name === 'InterplantHeadTailReconciliationPercent' && !isNaN(opt)) {
                if (!isNaN(min) && opt < min) result.error = "Optimal cannot be less than Min.";
                if (!isNaN(max) && opt > max) result.error = "Optimal cannot be greater than Max.";
            }
            if (name === 'InterplantHeadTailReconciliationPercent_Max' && !isNaN(max)) {
                if (!isNaN(opt) && max < opt) result.error = "Max cannot be less than Optimal.";
                if (!isNaN(min) && max < min) result.error = "Max cannot be less than Min.";
            }
        }

        // O) Final Scrap Percentages (Min <= Optimal <= Max)
        if (['ScrapReconciliationPercent', 'ScrapReconciliationPercent_Min', 'ScrapReconciliationPercent_Max'].includes(name)) {
            const min = parseFloat(allData['ScrapReconciliationPercent_Min']);
            const opt = parseFloat(allData['ScrapReconciliationPercent']);
            const max = parseFloat(allData['ScrapReconciliationPercent_Max']);

            if (name === 'ScrapReconciliationPercent_Min' && !isNaN(min)) {
                if (!isNaN(opt) && min > opt) result.error = "Min cannot be greater than Optimal.";
                if (!isNaN(max) && min > max) result.error = "Min cannot be greater than Max.";
            }
            if (name === 'ScrapReconciliationPercent' && !isNaN(opt)) {
                if (!isNaN(min) && opt < min) result.error = "Optimal cannot be less than Min.";
                if (!isNaN(max) && opt > max) result.error = "Optimal cannot be greater than Max.";
            }
            if (name === 'ScrapReconciliationPercent_Max' && !isNaN(max)) {
                if (!isNaN(opt) && max < opt) result.error = "Max cannot be less than Optimal.";
                if (!isNaN(min) && max < min) result.error = "Max cannot be less than Min.";
            }
        }

        // P) Final Head/Tail Percentages (Min <= Optimal <= Max)
        if (['HeadTailReconciliationPercent', 'HeadTailReconciliationPercent_Min', 'HeadTailReconciliationPercent_Max'].includes(name)) {
            const min = parseFloat(allData['HeadTailReconciliationPercent_Min']);
            const opt = parseFloat(allData['HeadTailReconciliationPercent']);
            const max = parseFloat(allData['HeadTailReconciliationPercent_Max']);

            if (name === 'HeadTailReconciliationPercent_Min' && !isNaN(min)) {
                if (!isNaN(opt) && min > opt) result.error = "Min cannot be greater than Optimal.";
                if (!isNaN(max) && min > max) result.error = "Min cannot be greater than Max.";
            }
            if (name === 'HeadTailReconciliationPercent' && !isNaN(opt)) {
                if (!isNaN(min) && opt < min) result.error = "Optimal cannot be less than Min.";
                if (!isNaN(max) && opt > max) result.error = "Optimal cannot be greater than Max.";
            }
            if (name === 'HeadTailReconciliationPercent_Max' && !isNaN(max)) {
                if (!isNaN(opt) && max < opt) result.error = "Max cannot be less than Optimal.";
                if (!isNaN(min) && max < min) result.error = "Max cannot be less than Min.";
            }
        }

        // ---------------------------------------------------------
        // VALIDACIÓN DE RANGOS DE INGENIERÍA (Tensile, Thickness, etc.)
        // ---------------------------------------------------------        
        // Solo validamos si hay valor y si tenemos las listas cargadas
        if (value && currentEngineeringRanges.length > 0) {

            // 2. Helper interno para validar un criterio específico
            const checkCriteria = (val: number, criteriaId: number) => {
                const range = currentEngineeringRanges.find((r: any) => r.criteriaId === criteriaId); if (!range) return null;

                // Definimos la tolerancia (si es null, usamos 0)
                const tol = range.tolerance || 0;

                // Preparamos el sufijo de texto "(+/- 50)"
                const tolSuffix = tol > 0 ? ` (+/- ${tol})` : '';

                // A. Validación de Rango Completo (Min y Max)
                if (range.minValue !== null && range.maxValue !== null) {
                    const effectiveMin = range.minValue - tol;
                    const effectiveMax = range.maxValue + tol;

                    // Si está fuera de los límites efectivos (Base +/- Tolerancia)
                    if (val < effectiveMin || val > effectiveMax) {
                        return `Allowed range: ${range.minValue} - ${range.maxValue}${tolSuffix}`;
                    }
                }

                // B. Validación Solo Máximo
                else if (range.maxValue !== null) {
                    const effectiveMax = range.maxValue + tol;
                    if (val > effectiveMax) {
                        return `Max allowed: ${range.maxValue}${tolSuffix}`;
                    }
                }

                // C. Validación Solo Mínimo
                else if (range.minValue !== null) {
                    const effectiveMin = range.minValue - tol;
                    if (val < effectiveMin) {
                        return `Min allowed: ${range.minValue}${tolSuffix}`;
                    }
                }

                // D. Validación Valor Exacto
                else if (range.numericValue !== null) {
                    const minExact = range.numericValue - tol;
                    const maxExact = range.numericValue + tol;
                    if (val < minExact || val > maxExact) {
                        return `Required value: ${range.numericValue}${tolSuffix}`;
                    }
                }

                return null;
            };

            const numVal = parseFloat(value);

            if (!isNaN(numVal)) {
                // ID 14 = Tensile Strength
                if (name === 'Tensile_Strenght' || name === 'Tensile_Strength') {
                    const msg = checkCriteria(numVal, 14);
                    if (msg) result.error = msg;
                }

                // ID 7 = Thickness (Gauge - Metric)
                if (name === 'Thickness') {
                    const msg = checkCriteria(numVal, 7); // 👈 ID 7
                    if (msg) result.error = msg;
                }

                // ID 8 = Width (Longitudinal Width)
                if (name === 'Width') {
                    const msg = checkCriteria(numVal, 8); // 👈 ID 8
                    if (msg) result.error = msg;
                }

                // ID 9 = Pitch (Longitudinal Pitch)
                if (name === 'Pitch') {
                    const msg = checkCriteria(numVal, 9); // 👈 ID 9
                    if (msg) result.error = msg;
                }
                if (name === 'OuterCoilDiameterArrival') {
                    const msg = checkCriteria(numVal, 16); // ID 16 = Outer Diameter
                    if (msg) result.error = msg;
                }
                if (name === 'MasterCoilWeight') {
                    const msg = checkCriteria(numVal, 17); // ID 17 = MasterCoilWeight
                    if (msg) result.error = msg;
                }
            }
        }

        return result;
    };

    const handleFieldChange = (name: string, value: any) => {
        let nextData = { ...formData, [name]: value };

        // -------------------------------------------------------------
        // NUEVA LÓGICA: Sincronización País-Vehículo (Sin Race Condition)
        // -------------------------------------------------------------
        // Buscamos si el campo que cambió ('name') es el 'countryFieldName' de algún vehículo.
        // Ej: Si cambié 'IHS_Country', busco qué campo tiene countryFieldName = 'IHS_Country'.
        const dependentVehicleField = safeFields.find(f => f.countryFieldName === name);

        if (dependentVehicleField) {
            // Si encontramos uno (ej: 'Vehicle'), lo limpiamos AQUÍ MISMO.
            // Al hacerlo en el mismo objeto 'nextData', garantizamos que ambos cambios
            // (nuevo país + vehículo vacío) se guarden juntos.
            (nextData as any)[dependentVehicleField.name] = null;

            // Opcional: Limpiar metadata si quieres que se borre info de SOP/EOP visualmente
            // setVehicleMetadata(prev => ({ ...prev, [dependentVehicleField.name]: undefined }));
        }
        // -------------------------------------------------------------

        // REGLA: Si el almacén es Sur (ID 2), el checkbox "Passes Through South" se apaga.
        if (name === 'ID_Arrival_Warehouse') {
            // Convertimos a String para asegurar comparación segura ("2" vs 2)
            if (String(value) === '2') {
                (nextData as any)['PassesThroughSouthWarehouse'] = false;
            }
        }

        // =========================================================
        // 👇 NUEVA LÓGICA: CÁLCULO DE SURFACE
        // =========================================================
        if (name === 'ID_Material_type') {
            // 1. Si no hay selección, limpiamos Surface
            if (!value || value === "0") {
                (nextData as any)['Surface'] = "";
            }
            else {
                // 2. Buscamos el objeto completo en la lista para obtener el TEXTO
                // Usamos '==' para que coincida aunque uno sea string ("5") y otro number (5)
                const selectedOption = lists.materialTypes?.find((item: any) => item.Value == value);

                if (selectedOption && selectedOption.Text) {
                    const text = selectedOption.Text.trim();

                    // 3. Aplicamos la regla: Empieza con "E" -> Exposed, sino -> Unexposed
                    if (text.startsWith("E")) {
                        (nextData as any)['Surface'] = "Exposed";
                    } else {
                        (nextData as any)['Surface'] = "Unexposed";
                    }
                }
            }
        }

        // LÓGICA WELDED BLANKS: Ajustar array cuando cambia el número
        if (name === 'NumberOfPlates') {
            const num = parseInt(value);

            // Validamos rango 2-5 para generar
            if (!isNaN(num) && num >= 2 && num <= 5) {
                const currentPlates = (nextData as any)._weldedPlates || [];
                const newPlates: WeldedPlate[] = [];

                for (let i = 1; i <= num; i++) {
                    // Intentamos preservar el valor existente si ya estaba
                    const existing = currentPlates.find((p: any) => p.PlateNumber === i);
                    newPlates.push({
                        PlateNumber: i,
                        Thickness: existing ? existing.Thickness : 0
                    });
                }
                (nextData as any)._weldedPlates = newPlates;
            } else {
                // 👇 ESTO ES LO QUE FALTABA:
                // Si el número es inválido (1, vacío, o >5), limpiamos el array para que se oculten los inputs.
                (nextData as any)._weldedPlates = [];
            }
        }

        // Si desmarca IsWeldedBlank, limpiamos NumberOfPlates visualmente 
        if (name === 'IsWeldedBlank' && value === false) {
            (nextData as any).NumberOfPlates = null;
        }

        // -------------------------------------------------------------
        // LÓGICA INTERPLANT PACKAGING STANDARD
        // -------------------------------------------------------------
        if (name === 'InterplantPackagingStandard') {
            // Si el valor NO es 'OWN', limpiamos y desmarcamos el checkbox de Rack
            if (value !== 'OWN') {
                (nextData as any)['InterplantRequiresRackManufacturing'] = false;

                // Nota: Tu legacy también limpiaba 'DieManufacturing' aquí. 
                // Si agregas ese campo en el futuro, límpialo aquí también:
                (nextData as any)['InterplantRequiresDieManufacturing'] = false;
            }
        }

        // LIMPIEZA DE LABEL OTHER
        if (name === 'InterplantLabelTypeIds') {
            // value es un array de números, ej: [1, 5]
            const selectedIds = value as number[];

            // Si el ID 3 (Other) NO está en la selección, limpiamos la descripción
            if (!selectedIds.includes(3)) {
                (nextData as any)['InterplantLabelOtherDescription'] = null;
            }
        }

        // LIMPIEZA 1: Si cambio los Racks y ya no hay ninguno retornable
        if (name === 'InterplantRackTypeIds') {
            const selectedIds = value as number[];
            const returnableIds = [2, 3, 4]; // Mismos IDs de config

            // Si NO hay intersección (ningún rack retornable seleccionado)
            if (!selectedIds.some(id => returnableIds.includes(id))) {
                (nextData as any)['IsInterplantReturnableRack'] = false;
                (nextData as any)['InterplantReturnableUses'] = null;
            }
        }

        // LIMPIEZA 2: Si desmarco "Is Returnable", borro "Uses"
        if (name === 'IsInterplantReturnableRack' && value === false) {
            (nextData as any)['InterplantReturnableUses'] = null;
        }

        if (name === 'InterplantAdditionalIds') {
            const selectedIds = value as number[];

            // Si el ID 6 (Other) NO está, limpiamos descripción
            if (!selectedIds.includes(6)) {
                (nextData as any)['InterplantAdditionalsOtherDescription'] = null;
            }
        }

        // LIMPIEZA DE TRANSPORT OTHER (INTERPLANT)
        if (name === 'ID_InterplantDelivery_Transport_Type') {
            // Si el valor NO es 5 (Other), limpiamos el campo de texto
            // Convertimos a string para comparación segura ("5" vs 5)
            if (String(value) !== '5') {
                (nextData as any)['InterplantDelivery_Transport_Type_Other'] = null;
            }
        }

        // LIMPIEZA SCRAP RECONCILIATION
        if (name === 'InterplantScrapReconciliation' && value === false) {
            (nextData as any)['InterplantScrapReconciliationPercent_Min'] = null;
            (nextData as any)['InterplantScrapReconciliationPercent'] = null;
            (nextData as any)['InterplantScrapReconciliationPercent_Max'] = null;
            (nextData as any)['InterplantClientScrapReconciliationPercent'] = null;
        }

        // LIMPIEZA HEAD/TAIL RECONCILIATION
        if (name === 'InterplantHeadTailReconciliation' && value === false) {
            (nextData as any)['InterplantHeadTailReconciliationPercent_Min'] = null;
            (nextData as any)['InterplantHeadTailReconciliationPercent'] = null;
            (nextData as any)['InterplantHeadTailReconciliationPercent_Max'] = null;
            (nextData as any)['InterplantClientHeadTailReconciliationPercent'] = null;
        }

        // LIMPIEZA FINAL DELIVERY STANDARD
        if (name === 'PackagingStandard') {
            if (value !== 'OWN') {
                (nextData as any)['RequiresRackManufacturing'] = false;

                // Si agregas 'RequiresDieManufacturing' en el futuro, límpialo aquí también
                // (nextData as any)['RequiresDieManufacturing'] = false;
            }
        }

        // --- FINAL DELIVERY CLEANUP ---
        // 1. Labels: Clear "Other" description
        if (name === 'SelectedLabelIds') {
            const selectedIds = value as number[];
            if (!selectedIds.includes(3)) { // ID 3 = Other
                (nextData as any)['LabelOtherDescription'] = null;
            }
        }

        // 2. Racks: Clear Returnable logic if no returnable rack selected
        if (name === 'SelectedRackTypeIds') {
            const selectedIds = value as number[];
            const returnableIds = [2, 3, 4]; // Adjust IDs as per DB

            if (!selectedIds.some(id => returnableIds.includes(id))) {
                (nextData as any)['IsReturnableRack'] = false;
                (nextData as any)['ReturnableUses'] = null;
            }
        }

        // 3. Returnable Checkbox: Clear Uses if unchecked
        if (name === 'IsReturnableRack' && value === false) {
            (nextData as any)['ReturnableUses'] = null;
        }

        // 4. Additionals: Clear "Other" description
        if (name === 'SelectedAdditionalIds') {
            const selectedIds = value as number[];
            if (!selectedIds.includes(6)) { // ID 6 = Other
                (nextData as any)['AdditionalsOtherDescription'] = null;
            }
        }

        // LIMPIEZA FINAL TRANSPORT OTHER
        if (name === 'ID_Delivery_Transport_Type') {
            if (String(value) !== '5') {
                (nextData as any)['Delivery_Transport_Type_Other'] = null;
            }
        }

        // LIMPIEZA FINAL SCRAP RECONCILIATION
        if (name === 'ScrapReconciliation' && value === false) {
            (nextData as any)['ScrapReconciliationPercent_Min'] = null;
            (nextData as any)['ScrapReconciliationPercent'] = null;
            (nextData as any)['ScrapReconciliationPercent_Max'] = null;
            (nextData as any)['ClientScrapReconciliationPercent'] = null;
        }

        // LIMPIEZA FINAL HEAD/TAIL RECONCILIATION
        if (name === 'HeadTailReconciliation' && value === false) {
            (nextData as any)['HeadTailReconciliationPercent_Min'] = null;
            (nextData as any)['HeadTailReconciliationPercent'] = null;
            (nextData as any)['HeadTailReconciliationPercent_Max'] = null;
            (nextData as any)['ClientHeadTailReconciliationPercent'] = null;
        }

        // Si el valor es un objeto File (Binario), lo guardamos en su propiedad específica
        // pero NO tocamos el ID numérico (eso lo maneja el backend al guardar)
        // Si el valor es un objeto File (Binario) o null (al borrar archivo)
        if (value instanceof File || value === null) {
            // Forzamos a TypeScript a aceptar 'name' como una clave válida usando 'as keyof Material'
            // Ojo: 'name' aquí vendrá como 'arrivalAdditionalFile' (tu propiedad virtual)
            // Como esa propiedad NO está en la interfaz Material original, usamos 'as any' para evitar líos.
            (nextData as any)[name] = value;
        } else {
            // Para valores normales, también aseguramos el tipado
            (nextData as any)[name] = value;
        }

        const basicError = validateField(name, value, nextData);
        const complexResult = validateComplexRules(name, value, nextData);
        const finalError = basicError || complexResult.error;
        const finalWarning = complexResult.warning;

        let nextErrors = { ...errors, [name]: finalError };
        let nextWarnings = { ...warnings, [name]: finalWarning };
        let hasChanges = true;

        while (hasChanges) {
            hasChanges = false;
            safeFields.forEach(field => {
                const currentValue = (nextData as any)[field.name];
                const shouldBeActive = isFieldActive(field, nextData);

                // Si tiene valor pero LÓGICAMENTE no debería existir (ej: borraste Vehicle 1, Vehicle 2 debe morir)
                if (currentValue !== undefined && currentValue !== null && !shouldBeActive) {
                    (nextData as any)[field.name] = null; // Borramos el dato

                    // Limpiamos errores fantasmas
                    if (nextErrors[field.name]) delete nextErrors[field.name];
                    if (nextWarnings[field.name]) delete nextWarnings[field.name];

                    hasChanges = true; // Repetimos el ciclo por si esto afectó a otros campos
                }

                // Regla especial para valores por defecto (ej: Factor 100)
                //else if (field.name === 'Max_Production_Factor' && shouldBeActive && !(nextData as any)[field.name]) {
                //    (nextData as any)[field.name] = 100;
                //}

            });
        }



        setFormData(nextData);
        setErrors(nextErrors);
        setWarnings(nextWarnings);
    };

    const handleSave = async () => {
        // --- PARTE 1: VALIDACIÓN (INTACTA) ---
        const newErrors: Record<string, string> = {};
        const newWarnings: Record<string, string> = {};

        let firstInvalidFieldName = "";
        let firstInvalidSection = "";

        // Iteramos sobre TODOS los campos configurados
        safeFields.forEach(field => {
            const isActive = isFieldActive(field, formData);
            if (!isActive) return;

            const value = (formData as any)[field.name];

            // Usamos la nueva firma de validateField(name, value, allData)
            const basicError = validateField(field.name, value, formData);
            const complexResult = validateComplexRules(field.name, value, formData);
            const errorMsg = basicError || complexResult.error;

            if (errorMsg) {
                // 👇 AGREGA ESTE BLOQUE DE DEPURACIÓN
                console.group(`❌ Error de Validación detectado`);
                console.log(`Campo: %c${field.name}`, 'color: red; font-weight: bold;');
                console.log(`Etiqueta: ${field.label}`);
                console.log(`Sección: ${field.section}`);
                console.log(`Valor actual:`, value);
                console.log(`Mensaje de error: ${errorMsg}`);
                console.log(`¿Está activo/visible?: ${isActive}`);
                console.groupEnd();
                // 👆 FIN DEL BLOQUE

                newErrors[field.name] = errorMsg;
                if (!firstInvalidFieldName) {
                    firstInvalidFieldName = field.name;
                    firstInvalidSection = field.section;
                }
            }
            if (complexResult.warning) {
                newWarnings[field.name] = complexResult.warning;
            }
        });

        setErrors(newErrors);
        setWarnings(newWarnings);

        // --- PARTE 2: MANEJO DE NAVEGACIÓN SI HAY ERRORES (INTACTA) ---
        if (firstInvalidFieldName) {
            // Toast de Advertencia
            toast.warn("Please verify the highlighted fields before saving.", {
                position: "top-right",
                autoClose: 5000
            });

            if (firstInvalidSection && firstInvalidSection !== activeTab) {
                console.log(`🔎 Error en sección oculta: ${firstInvalidSection}. Cambiando tab...`);
                setActiveTab(firstInvalidSection);
                setTimeout(() => {
                    const element = document.getElementById(`field-container-${firstInvalidFieldName}`);
                    if (element) {
                        element.scrollIntoView({ behavior: 'smooth', block: 'center' });
                        const input = element.querySelector('input, select, textarea');
                        if (input instanceof HTMLElement) input.focus();
                    }
                }, 100);
            } else {
                const element = document.getElementById(`field-container-${firstInvalidFieldName}`);
                if (element) {
                    element.scrollIntoView({ behavior: 'smooth', block: 'center' });
                    const input = element.querySelector('input, select, textarea');
                    if (input instanceof HTMLElement) input.focus();
                }
            }
            return;
        }

        // PRE-PROCESAMIENTO: Serializar Welded Plates
        if ((formData as any)._weldedPlates) {
            (formData as any).WeldedPlatesJson = JSON.stringify((formData as any)._weldedPlates);
        }

        // --- PARTE 3: ENVÍO AL SERVIDOR (MODIFICADA PARA ARCHIVOS) ---
        setIsSaving(true);

        // Toast de carga (opcional, le da feedback inmediato)
        const toastId = toast.loading("Saving changes...");

        try {

            // PRE-PROCESAMIENTO: Serializar Welded Plates
            if ((formData as any)._weldedPlates) {
                (formData as any).WeldedPlatesJson = JSON.stringify((formData as any)._weldedPlates);
            }

            // 1. CREAMOS EL FORM DATA (Soporte para Archivos + Datos)
            const dataToSend = new FormData();

            // 2. CONVERTIMOS EL OBJETO 'formData' A FORM DATA
            Object.keys(formData).forEach(key => {
                const value = (formData as any)[key];

                // A. Ignoramos campos virtuales (empiezan con _)
                if (key.startsWith('_')) return;

                // B. CASO ARCHIVO: Si es un objeto File nativo, lo adjuntamos directo
                if (value instanceof File) {
                    // El 'key' aquí será el nombre definido en 'uploadFieldName' (ej: arrivalAdditionalFile)
                    dataToSend.append(key, value);
                }
                // C. CASO DATOS NORMALES (No nulos)
                else if (value !== null && value !== undefined) {

                    // C1. Fechas YYYY-MM -> Agregar día 01 para que C# no falle (Lógica Legacy conservada)
                    if ((key === 'Real_SOP' || key === 'Real_EOP') && typeof value === 'string' && value.length === 7) {
                        dataToSend.append(key, `${value}-01`);
                    }
                    // C2. Objetos complejos que no sean Arrays (ej. JSON strings si los hubiera)
                    else if (typeof value === 'object' && !Array.isArray(value)) {
                        dataToSend.append(key, JSON.stringify(value));
                    }
                    // C3. Todo lo demás (números, strings, booleanos, arrays simples)
                    else {
                        dataToSend.append(key, value.toString());
                    }
                }
            });

            // 3. ASEGURAR QUE EL ID DEL PROYECTO SE ENVÍE
            if (!dataToSend.has('ID_Project')) {
                const projectId = (formData as any).ID_Project || selectedMaterial?.ID_Project;
                if (projectId) dataToSend.append('ID_Project', projectId.toString());
            }

            if (!urls || !urls.saveUrl) throw new Error("Save URL is missing.");

            // 4. ENVIAR LA PETICIÓN
            // IMPORTANTE: NO agregamos 'Content-Type': 'multipart/form-data'.
            // Fetch lo detecta automáticamente al ver el objeto FormData y añade el 'boundary' correcto.
            const response = await fetch(urls.saveUrl, {
                method: 'POST',
                // headers: { ... }  <-- SE DEJA VACÍO PARA QUE EL BROWSER PONGA EL MULTIPART CORRECTO
                body: dataToSend
            });

            if (!response.ok) {
                // Intentamos leer el error del servidor si es texto plano
                const errorText = await response.text();
                throw new Error(`HTTP Error ${response.status}: ${errorText || response.statusText}`);
            }

            const result = await response.json();

            if (result.success) {
                // Actualizar el toast de carga a Éxito
                toast.update(toastId, {
                    render: "Data saved successfully!",
                    type: "success",
                    isLoading: false,
                    autoClose: 3000,
                    transition: Slide
                });

                // Pequeño delay para que el usuario vea el check verde antes de cerrar
                setTimeout(() => {
                    if (onSaved && result.data) {
                        onSaved(result.data); // Actualiza la tabla y cierra el form

                        // 👇 AGREGAR ESTO: Scroll suave hacia arriba
                        window.scrollTo({ top: 0, behavior: 'smooth' });

                    } else {
                        // Fallback por si el backend no devolvió 'data'
                        window.location.reload();
                    }
                }, 1000);

            } else {
                // Actualizar el toast de carga a Error
                toast.update(toastId, {
                    render: "Server Error: " + result.message,
                    type: "error",
                    isLoading: false,
                    autoClose: 5000
                });
            }

        } catch (error: any) {
            console.error("❌ Error saving data:", error);
            // Actualizar el toast de carga a Error Crítico
            toast.update(toastId, {
                render: "An unexpected error occurred. Please check console.",
                type: "error",
                isLoading: false,
                autoClose: 5000
            });
        } finally {
            setIsSaving(false);
        }
    };

    if (!selectedMaterial) return null;
    if (!DynamicField) {
        return <div className="alert alert-danger">Error: El componente DynamicField no se cargó correctamente.</div>;
    }
    // Calculamos qué secciones tienen al menos un campo activo bajo las condiciones actuales
    const visibleSections = useMemo(() => {
        return sections.filter(sectionName => {
            // Buscamos todos los campos de esta sección
            const fieldsInSection = safeFields.filter(f => f.section === sectionName);

            // Verificamos si AL MENOS UNO está activo (cumple visibleWhen)
            const hasVisibleFields = fieldsInSection.some(f => isFieldActive(f, formData));

            return hasVisibleFields;
        });
    }, [sections, safeFields, formData]); // Se recalcula cada vez que cambia la data

    // 2. AUTO-CAMBIO DE PESTAÑA
    // Si la pestaña actual se vuelve invisible (ej: cambiaste la Ruta y se ocultó Arrival),
    // saltamos automáticamente a la primera pestaña visible para no dejar al usuario en el limbo.
    useEffect(() => {
        if (visibleSections.length > 0 && !visibleSections.includes(activeTab)) {
            setActiveTab(visibleSections[0]);
        }
    }, [visibleSections, activeTab]);

    // --- NUEVO LAYOUT VISUAL (GRID + PESTAÑAS IZQUIERDAS) ---
    return (
        <div className="container-fluid main-background" style={{ padding: '20px', backgroundColor: '#f4f6f9', minHeight: '100vh' }}>
            <style>{modernStyles}</style>

            <div className="row">
                {/* COLUMNA IZQUIERDA: MENÚ DE NAVEGACIÓN */}
                <div className="col-lg-2 col-md-3 mb-4">
                    <div className="nav flex-column nav-pills modern-nav" role="tablist" aria-orientation="vertical">
                        {visibleSections.map(sectionName => {

                            // 1. CALCULAMOS LOS ERRORES DE ESTA PESTAÑA ESPECÍFICA
                            // Filtramos todos los campos que pertenecen a esta sección ('sectionName')
                            // y verificamos si existe una entrada para ellos en el objeto 'errors'.
                            const errorCount = safeFields
                                .filter(f => f.section === sectionName)
                                .filter(f => errors[f.name])
                                .length;

                            // 2. DETERMINAMOS EL ICONO
                            let iconClass = "fa-cube"; // Default (sirve para datos genéricos)
                            let linkClass = ""; // Clase adicional para el link

                            // --- LÓGICA DE ICONOS ---
                            if (sectionName.includes("Vehicle")) {
                                iconClass = "fa-car"; // Representa el vehículo
                            }
                            else if (sectionName.includes("Production")) {
                                iconClass = "fa-cogs"; // Representa maquinaria/proceso
                            }
                            else if (sectionName.includes("Arrival")) {
                                iconClass = "fa-truck"; // Representa la llegada de material
                            }
                            else if (sectionName.includes("Coil")) {
                                iconClass = "fa-toilet-paper"; // Parece una bobina/rollo con la punta
                            }
                            else if (sectionName.includes("Slitter")) {
                                iconClass = "fa-scissors"; // Representa corte longitudinal
                            }
                            else if (sectionName.includes("Blank")) {
                                iconClass = "fa-clone"; // Representa hojas apiladas (Blanks)
                            }
                            else if (sectionName.includes("Shearing")) {
                                iconClass = "fa-crop"; // Representa recorte/cizalla
                            }
                            else if (sectionName.includes("Interplant")) {
                                iconClass = "fa-exchange"; // Flechas de intercambio entre plantas
                            }
                            else if (sectionName.includes("Packaging")) {
                                iconClass = "fa-box-open"; // Caja abierta para empaque
                            }
                            else if (sectionName.includes("Final")) {
                                iconClass = "fa-flag-checkered"; // Bandera de meta (Fin del proceso)
                            } else if (sectionName === "Technical Feasibility") {
                                iconClass = "fa-clipboard-check"; // Icono de reporte/resultado
                                linkClass = "special-tab";        // 👈 Clase CSS nueva
                            } else if (sectionName === "Efficiency and Capacity") {
                                iconClass = "fa-chart-line"; // Icono de gráfica/análisis
                                linkClass = "data-tab";      // Clase CSS Púrpura
                            }

                            return (
                                <a
                                    key={sectionName}
                                    className={`nav-link ${activeTab === sectionName ? 'active' : ''} ${linkClass}`} onClick={(e) => { e.preventDefault(); setActiveTab(sectionName); }}
                                    href="#"
                                    role="tab"
                                    style={{ cursor: 'pointer' }}
                                >
                                    {/* ICONO */}
                                    <i className={`fa ${iconClass} mr-2`}></i>

                                    {/* NOMBRE DE LA SECCIÓN */}
                                    <span style={{ marginRight: '5px' }}>{sectionName}</span>

                                    {/* GLOBO DE NOTIFICACIÓN (Solo aparece si hay errores) */}
                                    {errorCount > 0 && (
                                        <span className="error-badge pulse" title={`${errorCount} errors in this section`}>
                                            {errorCount}
                                        </span>
                                    )}
                                </a>
                            );
                        })}
                    </div>
                </div>

                {/* COLUMNA DERECHA: TARJETA DE CONTENIDO */}
                <div className="col-lg-10 col-md-9">
                    <div className="form-card">

                        <div className="form-header" style={{
                            display: 'flex',
                            justifyContent: 'space-between',
                            alignItems: 'center', // Alineación vertical centrada
                            borderBottom: '1px solid #e9ecef',
                            paddingBottom: '20px',
                            marginBottom: '25px',
                            position: 'relative'
                        }}>
                            {/* Barra decorativa inferior */}
                            <div style={{
                                position: 'absolute',
                                bottom: '-1px',
                                left: 0,
                                width: '100%',
                                height: '3px',
                                background: 'linear-gradient(90deg, #009ff5 0%, #009ff5 15%, transparent 100%)',
                                borderRadius: '2px'
                            }}></div>

                            {/* PARTE IZQUIERDA: TÍTULO */}
                            <div className="header-text-group">
                                <h4 style={{
                                    margin: 0,
                                    color: '#009ff5',
                                    fontWeight: '700',
                                    letterSpacing: '-0.5px',
                                    display: 'flex',
                                    alignItems: 'center'
                                }}>
                                    <i className="fa fa-pen-to-square" style={{ marginRight: '10px', fontSize: '20px', color: '#009ff5' }}></i>
                                    {selectedMaterial.ID_Material === 0 ? 'Adding New Material' : 'Editing Material'}
                                </h4>
                                <p className="text-muted mb-0" style={{ fontSize: '0.9rem', marginTop: '5px' }}>
                                    Please review the information below. <span className="text-danger">*</span> indicates mandatory fields.
                                </p>
                            </div>

                            {/* PARTE DERECHA: BOTONES DE ACCIÓN Y AYUDA */}
                            <div style={{ display: 'flex', gap: '10px', alignItems: 'center' }}>

                                {/* BOTÓN TOLERANCIAS (Solo en Coil/Blank Data) */}
                                {(activeTab === 'Coil Data' || activeTab === 'Blank Data') && (
                                    <button
                                        type="button"
                                        className="btn btn-sm btn-outline-info shadow-sm"
                                        onClick={() => setShowToleranceModal(true)}
                                        style={{ borderRadius: '50px', fontWeight: 600, padding: '6px 15px' }}
                                    >
                                        <i className="fa fa-table mr-2"></i> Show Tolerance Tables
                                    </button>
                                )}

                                {/* BOTÓN INCOTERMS (Solo en Freight) */}
                                {(activeTab === 'Interplant Outbound Freight & Conditions' ||
                                    activeTab === 'Final Outbound Freight & Conditions') && (
                                        <button
                                            type="button"
                                            className="btn btn-sm btn-info shadow-sm"
                                            onClick={() => setShowIncotermsModal(true)}
                                            style={{ borderRadius: '50px', fontWeight: 600, color: 'white', padding: '6px 15px' }}
                                        >
                                            <i className="fa fa-info-circle mr-2"></i> Incoterms Help
                                        </button>
                                    )}

                                {/* BOTÓN CANCELAR */}
                                <button className="btn-modern-cancel" onClick={onCancel} title="Discard all changes">
                                    <i className="fa fa-times-circle" style={{ fontSize: '16px' }}></i>
                                    <span>Discard Changes</span>
                                </button>
                            </div>

                        </div>

                        {/* ÁREA DE FORMULARIO SCROLLABLE */}
                        <div className="form-body">
                            <div className="row">

                                {safeFields.map((fieldConfig, index) => {

                                    // =======================================================================
                                    // 1. DEFINICIÓN UNIFICADA DE getFieldProps
                                    //    Esta función ahora está disponible para TODOS los campos del ciclo.
                                    // =======================================================================
                                    const getFieldProps = (fc: any) => {
                                        // A. Visibilidad
                                        if (!fc || !isFieldVisible(fc, formData)) return null;

                                        // B. Required Dinámico
                                        let isReq = fc.validation?.required || false;
                                        if (fc.validation?.requiredWhen) {
                                            const { field, is } = fc.validation.requiredWhen;
                                            const depVal = (formData as any)[field];
                                            if (is && is.includes(depVal)) isReq = true;
                                        }

                                        // C. Valor y Configuración Base
                                        let displayValue = (formData as any)[fc.name];
                                        let currentConfig = { ...fc };

                                        // D. Interceptor: Línea Teórica (ID -> Nombre)
                                        if (fc.name === 'ID_Theoretical_Blanking_Line') {
                                            const foundLine = lists.linesList?.find((l: any) => l.Value == displayValue);
                                            if (foundLine) displayValue = foundLine.Text;
                                            currentConfig.type = 'text';
                                            currentConfig.disabled = true;
                                        }

                                        // E. Lógica de Ingeniería (HelperText - Límites)
                                        let helperText = undefined;
                                        let criteriaId = null;

                                        // Normalizamos nombre para evitar errores de espacios
                                        const fieldName = fc.name ? fc.name.trim() : "";

                                        // Mapa manual de IDs
                                        if (fieldName === 'Tensile_Strenght' || fieldName === 'Tensile_Strength') criteriaId = 14;
                                        else if (fieldName === 'Thickness') criteriaId = 7;  // Gauge - Metric
                                        else if (fieldName === 'Width') criteriaId = 8; //Longitudinal width
                                        else if (fieldName === 'Pitch') criteriaId = 9; //Longitudinal pitch
                                        else if (fieldName === 'OuterCoilDiameterArrival') criteriaId = 16;
                                        else if (fieldName === 'MasterCoilWeight') criteriaId = 17;


                                        // Si es un campo de ingeniería (tiene criteriaId)...
                                        if (criteriaId !== null) {

                                            // CASO 1: Hay reglas cargadas y encontramos la específica
                                            if (currentEngineeringRanges && currentEngineeringRanges.length > 0) {
                                                const range = currentEngineeringRanges.find((r: any) => r.criteriaId === criteriaId);

                                                if (range) {
                                                    const tolText = (range.tolerance !== null && range.tolerance !== 0) ? ` (±${range.tolerance})` : '';

                                                    if (range.minValue !== null && range.maxValue !== null) {
                                                        helperText = `Limit: ${range.minValue} - ${range.maxValue}${tolText}`;
                                                    } else if (range.maxValue !== null) {
                                                        helperText = `Max: ${range.maxValue}${tolText}`;
                                                    } else if (range.minValue !== null) {
                                                        helperText = `Min: ${range.minValue}${tolText}`;
                                                    } else if (range.numericValue !== null) {
                                                        helperText = `Exact: ${range.numericValue}${tolText}`;
                                                    }
                                                } else {
                                                    // Hay reglas para la línea (ej: ancho), pero NO para este campo (ej: tensile)
                                                    helperText = "No specific limit defined.";
                                                }
                                            }
                                            // CASO 2: No hay reglas para esta combinación (Línea + Material)
                                            else {
                                                // Solo mostramos el mensaje si el usuario ya seleccionó Línea y Material
                                                // (para que no salga el aviso apenas entra al formulario vacío)
                                                const hasLine = (formData as any)['ID_Real_Blanking_Line'] || (formData as any)['ID_Theoretical_Blanking_Line'];
                                                const hasMat = (formData as any)['ID_Material_type'];

                                                if (hasLine && hasMat) {
                                                    helperText = "No limits for this Line/Material.";
                                                }
                                            }
                                        }

                                        return {
                                            config: {
                                                ...currentConfig,
                                                validation: { ...currentConfig.validation, required: isReq }
                                            },
                                            value: displayValue,
                                            helperText: helperText
                                        };
                                    };

                                    // =======================================================================
                                    // 2. LÓGICA PARA EVITAR DUPLICADOS (Si ya se renderizó en un grupo anterior)
                                    // =======================================================================
                                    const prev1 = safeFields[index - 1];
                                    const prev2 = safeFields[index - 2];
                                    if (prev1?.rowTitle || prev2?.rowTitle) {
                                        return null;
                                    }

                                    // =======================================================================
                                    // 3. PREPARAR PROPS (Usando la función unificada)
                                    // =======================================================================
                                    const hasRowTitle = !!fieldConfig.rowTitle;

                                    // Calculamos las props del campo actual (p1)
                                    const p1 = getFieldProps(fieldConfig);

                                    // Si es un grupo, calculamos los siguientes 2
                                    const field2 = hasRowTitle ? safeFields[index + 1] : null;
                                    const field3 = hasRowTitle ? safeFields[index + 2] : null;

                                    const p2 = field2 ? getFieldProps(field2) : null;
                                    const p3 = field3 ? getFieldProps(field3) : null;

                                    // Si el campo principal no es visible, no renderizamos nada
                                    if (!p1) return null;

                                    // =======================================================================
                                    // 4. SOBRESCRITURA DE DATOS (Fix para ID -> Texto en DynamicField)
                                    //    Usamos 'any' para evitar error TS7053 (Index signature)
                                    // =======================================================================
                                    const overriddenData: any = { ...formData };

                                    if (p1) overriddenData[fieldConfig.name] = p1.value;

                                    // Validamos existencia antes de asignar para evitar error 'possibly null'
                                    if (p2 && field2) {
                                        overriddenData[field2.name] = p2.value;
                                    }
                                    if (p3 && field3) {
                                        overriddenData[field3.name] = p3.value;
                                    }


                                    // =======================================================================
                                    // 5. RENDERIZADO
                                    // =======================================================================
                                    if (hasRowTitle) {
                                        // === VISTA MATRIZ (CON TÍTULO A LA IZQUIERDA) ===
                                        return (
                                            <div className="col-12 mb-3 pb-2 border-bottom" key={fieldConfig.name} style={{ borderBottomColor: '#eee' }}>

                                                {/* 👇 NUEVO BLOQUE: SEPARADOR TRAPEZOIDE 👇 */}
                                                {fieldConfig.name === 'Angle_A' && (
                                                    <div className="col-12 mt-4 mb-2">
                                                        <h6 className="text-primary font-weight-bold" style={{
                                                            borderBottom: '1px solid #dee2e6',
                                                            paddingBottom: '10px',
                                                            textTransform: 'uppercase',
                                                            fontSize: '0.9rem',
                                                            letterSpacing: '0.5px'
                                                        }}>
                                                            <i className="fa fa-ruler-combined mr-2"></i> Trapezoid Dimensions
                                                        </h6>
                                                    </div>
                                                )}

                                                <div style={{ display: 'flex', flexDirection: 'row' }}>

                                                    {/* Título de la Fila */}
                                                    <div style={{ width: '140px', minWidth: '140px', textAlign: 'right', paddingRight: '20px' }}>
                                                        <label style={{ color: '#009ff5', fontWeight: 'bold', fontSize: '1.1em', marginTop: '38px', display: 'block' }}>
                                                            {fieldConfig.rowTitle}
                                                        </label>
                                                    </div>

                                                    {/* Inputs Agrupados */}
                                                    <div className="matrix-group-container">
                                                        <div className="matrix-cell">
                                                            <DynamicField
                                                                key={fieldConfig.name}
                                                                config={p1.config}
                                                                value={p1.value}
                                                                onChange={handleFieldChange}
                                                                lists={lists} urls={urls}
                                                                error={errors[fieldConfig.name]}
                                                                warning={warnings[fieldConfig.name]}
                                                                isVisible={true}
                                                                fullData={overriddenData}
                                                                helperText={p1.helperText}
                                                            />
                                                        </div>
                                                        {p2 && field2 && (
                                                            <div className="matrix-cell">
                                                                <DynamicField
                                                                    key={field2.name}
                                                                    config={p2.config}
                                                                    value={p2.value}
                                                                    onChange={handleFieldChange}
                                                                    lists={lists} urls={urls}
                                                                    error={errors[field2.name]}
                                                                    warning={warnings[field2.name]}
                                                                    isVisible={true}
                                                                    fullData={overriddenData}
                                                                    helperText={p2.helperText}
                                                                />
                                                            </div>
                                                        )}
                                                        {p3 && field3 && (
                                                            <div className="matrix-cell">
                                                                <DynamicField
                                                                    key={field3.name}
                                                                    config={p3.config}
                                                                    value={p3.value}
                                                                    onChange={handleFieldChange}
                                                                    lists={lists} urls={urls}
                                                                    error={errors[field3.name]}
                                                                    warning={warnings[field3.name]}
                                                                    isVisible={true}
                                                                    fullData={overriddenData}
                                                                    helperText={p3.helperText}
                                                                />
                                                            </div>
                                                        )}
                                                    </div>
                                                </div>
                                            </div>
                                        );
                                    } else {
                                        // === VISTA NORMAL (COLUMNAS ESTÁNDAR) ===
                                        return (
                                            <div key={fieldConfig.name + "_container"} style={{ display: 'contents' }}>
                                                {/* 👇 NUEVO BLOQUE: SEPARADOR VISUAL PARA VOLUMEN 👇 */}
                                                {fieldConfig.name === 'Annual_Volume' && (
                                                    <div className="col-12 mt-4 mb-2">
                                                        <h6 className="text-primary font-weight-bold" style={{
                                                            borderBottom: '1px solid #dee2e6',
                                                            paddingBottom: '10px',
                                                            textTransform: 'uppercase',
                                                            fontSize: '0.9rem',
                                                            letterSpacing: '0.5px'
                                                        }}>
                                                            <i className="fa fa-chart-bar mr-2"></i> Volume & Weights
                                                        </h6>
                                                    </div>
                                                )}
                                                <DynamicField
                                                    key={fieldConfig.name}
                                                    config={p1.config}

                                                    // Side Effects (Vehicles, etc)
                                                    onSideEffect={fieldConfig.type === 'vehicle-selector'
                                                        ? (fullData: any) => {
                                                            if (fullData) {
                                                                setVehicleMetadata(prev => ({
                                                                    ...prev,
                                                                    [fieldConfig.name]: {
                                                                        Program: fullData.Program,
                                                                        SOP: fullData.SOP,
                                                                        EOP: fullData.EOP,
                                                                        ProductionDataJson: fullData.ProductionDataJson
                                                                    }
                                                                }));
                                                            }
                                                        }
                                                        : fieldConfig.onSideEffect
                                                    }

                                                    value={p1.value}
                                                    onChange={handleFieldChange}
                                                    lists={lists} urls={urls}
                                                    error={errors[fieldConfig.name]}
                                                    warning={warnings[fieldConfig.name]}
                                                    isVisible={true}

                                                    fullData={overriddenData}
                                                    helperText={p1.helperText}
                                                />

                                                {/* Renderizado especial de Welded Plates */}
                                                {fieldConfig.name === 'NumberOfPlates'
                                                    && (formData as any).IsWeldedBlank
                                                    && (formData as any)._weldedPlates?.length > 0 && (
                                                        <div className="col-12 mt-2 mb-4 pl-0" key="welded-children">
                                                            <div className="card bg-light border-0">
                                                                <div className="card-body py-3 px-3">
                                                                    <h6 className="text-primary small font-weight-bold mb-3" style={{ borderBottom: '1px solid #dee2e6', paddingBottom: '5px' }}>
                                                                        Thickness per Blank [mm]
                                                                    </h6>
                                                                    <div className="row">
                                                                        {(formData as any)._weldedPlates.map((plate: any, pIdx: number) => {
                                                                            const count = (formData as any)._weldedPlates.length;
                                                                            const colWidth = Math.max(Math.floor(12 / count), 2);
                                                                            return (
                                                                                <div key={plate.PlateNumber} className={`col-md-${colWidth} form-group`}>
                                                                                    <label className="small font-weight-bold text-muted">Blank #{plate.PlateNumber} Thickness</label>
                                                                                    <div className="input-group input-group-sm">
                                                                                        <input
                                                                                            type="number"
                                                                                            className="form-control"
                                                                                            placeholder="0.00"
                                                                                            min="0"
                                                                                            step="0.01"
                                                                                            value={plate.Thickness || ''}
                                                                                            onChange={(e) => {
                                                                                                let val = parseFloat(e.target.value);
                                                                                                if (val < 0) val = 0;
                                                                                                const newArr = [...(formData as any)._weldedPlates];
                                                                                                newArr[pIdx].Thickness = isNaN(val) ? 0 : val;
                                                                                                setFormData({ ...formData, _weldedPlates: newArr });
                                                                                            }}
                                                                                            style={{ borderColor: (!plate.Thickness || plate.Thickness <= 0) ? '#ffc107' : '#ced4da' }}
                                                                                        />
                                                                                        <div className="input-group-append"><span className="input-group-text">mm</span></div>
                                                                                    </div>
                                                                                </div>
                                                                            );
                                                                        })}
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    )}
                                            </div>
                                        );
                                    }
                                })}

                                {/* Mensaje de "No hay campos" si nada es visible en esta sección */}
                                {!safeFields.some(f => isFieldVisible(f, formData)) && (
                                    <div className="col-12 text-center p-5 text-muted">
                                        <i className="fa fa-folder-open-o fa-3x mb-3"></i>
                                        <p>No visible fields in this section based on current selection.</p>
                                    </div>
                                )}
                            </div>
                        </div>

                        {/* PIE DE PÁGINA DE LA TARJETA (BOTONES) */}
                        <div className="mt-4 pt-3 border-top text-right">
                            <button
                                className="btn btn-primary btn-lg shadow-sm"
                                onClick={handleSave}
                                disabled={isSaving}
                                style={{
                                    minWidth: '200px',
                                    borderRadius: '50px',
                                    background: 'linear-gradient(135deg, #009ff5 0%, #007bff 100%)', // Degradado thyssen
                                    border: 'none'
                                }}
                            >
                                {isSaving ? (
                                    <><i className="fa fa-spinner fa-spin mr-2"></i> Saving...</>
                                ) : (
                                    <><i className="fa fa-check-circle mr-2"></i> Save Changes</>
                                )}
                            </button>
                        </div>

                    </div>
                </div>
            </div>
            <ToleranceReference
                show={showToleranceModal}
                onClose={() => setShowToleranceModal(false)}
            />
            <IncotermsReference
                show={showIncotermsModal}
                onClose={() => setShowIncotermsModal(false)}
            />
        </div>
    );
}