//src/componemts/MaterialForm.tsx
import { useState, useEffect, useMemo } from 'react';
import type { Material, WeldedPlate } from '../types';
// Aseguramos la importación.
import type { FieldConfig } from '../types/form-schema';
import { materialFields } from '../config/material-fields';
import ToleranceReference from './ToleranceReference';
import DynamicField from './DynamicField';
import { toast, Slide } from 'react-toastify';

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
    console.log('📋 Cargando Configuración de campos:', materialFields);
    console.log("📦 Listas disponibles:", lists);

    const [formData, setFormData] = useState<Partial<Material>>({
        Max_Production_Factor: 100
    });
    const [errors, setErrors] = useState<Record<string, string>>({});
    const [isSaving, setIsSaving] = useState(false);
    const [warnings, setWarnings] = useState<Record<string, string>>({});
    const [showToleranceModal, setShowToleranceModal] = useState(false); // 👈 2. NUEVO ESTADO

    // Estado Metadata
    const [vehicleMetadata, setVehicleMetadata] = useState<Record<string, {
        Program?: string,
        SOP?: string,
        EOP?: string,
        ProductionDataJson?: string
    }>>({});

    const safeFields = Array.isArray(materialFields) ? materialFields : [];

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

            // A. Llenar el formulario con lo que hay en BD (Tu lógica original)
            setFormData({
                ...selectedMaterial,
                Max_Production_Factor: selectedMaterial.Max_Production_Factor ?? 100,
                Real_SOP: formatMonth(selectedMaterial.Real_SOP),
                Real_EOP: formatMonth(selectedMaterial.Real_EOP),
                Surface: computedSurface,
                //AQUI van los files
                FileName_ArrivalAdditional: raw.CTZ_Files4?.Name,
                FileName_CoilDataAdditional: raw.CTZ_Files5?.Name,
                FileName_SlitterDataAdditional: raw.CTZ_Files6?.Name,
                CADFileName: raw.CTZ_Files?.Name,
                TechnicalSheetFileName: raw.CTZ_Files1?.Name,
                AdditionalFileName: raw.CTZ_Files2?.Name,
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


    // 1. NUEVA FUNCIÓN: Determina si el campo aplica lógicamente (ignorando la pestaña actual)
    const isFieldActive = (field: FieldConfig, contextData: any) => {
        // NO revisamos field.section aquí. Solo dependencias de datos.
        if (field.visibleWhen) {
            const dependencyName = field.visibleWhen.field;
            const dependencyValue = (contextData as any)[dependencyName];

            if (field.visibleWhen.is) {
                if (!field.visibleWhen.is.includes(dependencyValue)) return false;
            }
            if (field.visibleWhen.hasValue) {
                if (dependencyValue === null || dependencyValue === undefined || dependencyValue === "") return false;
            }
        }
        return true;
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
        // Consideramos vacío: null, undefined, string vacío, o 0 (común en selects de ID)
        if (isRequired) {
            const isEmpty = value === null || value === undefined || value === "" || value === 0;
            if (isEmpty) {
                return rules.customMessage || `${fieldConfig.label} is required.`;
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

        // A) Master Coil Weight (Criterio 17)
        if (name === 'MasterCoilWeight' && value) {
            const val = parseFloat(value);
            const criterio = ranges.find((r: any) => r.ID_Criteria === 17);

            // Función auxiliar para validar rango (copiada de tu lógica legacy/global)
            // Si no tienes esta función disponible aquí, la podemos implementar inline
            if (criterio) {
                if (criterio.NumericValue != null && val !== criterio.NumericValue) {
                    result.error = `Value must be exactly ${criterio.NumericValue}.`;
                }
                else if (criterio.MinValue != null && criterio.MaxValue != null) {
                    // Nota: Legacy valida "contra criterio", asumo que verifica estar DENTRO del rango
                    if (val < criterio.MinValue || val > criterio.MaxValue) {
                        result.error = `Value must be between ${criterio.MinValue} and ${criterio.MaxValue}.`;
                    }
                }
            }
        }

        // B) Outer Coil Diameter (Criterio 16)
        if (name === 'OuterCoilDiameterArrival' && value) {
            const val = parseFloat(value);
            const criterio = ranges.find((r: any) => r.ID_Criteria === 16);
            if (criterio) {
                if (criterio.MinValue != null && criterio.MaxValue != null) {
                    if (val < criterio.MinValue || val > criterio.MaxValue) {
                        result.error = `Value must be between ${criterio.MinValue} and ${criterio.MaxValue}.`;
                    }
                }
            }
        }

        // C) Inner Coil Diameter (Criterio 15 + Regla 508/610)
        if (name === 'InnerCoilDiameterArrival' && value) {
            const val = parseFloat(value);
            const criterio = ranges.find((r: any) => r.ID_Criteria === 15);

            if (criterio) {
                // Caso 1: Valor exacto definido en DB
                if (criterio.NumericValue != null) {
                    if (val !== criterio.NumericValue) {
                        result.error = `Value must be exactly ${criterio.NumericValue}.`;
                    }
                }
                // Caso 2: Rango definido (Legacy dice "endpoints only" en su lógica else-if, 
                // pero revisa si tu regla es "entre" o "solo extremos". 
                // El legacy dice: val !== min && val !== max. Mantenemos eso.)
                else if (criterio.MinValue != null && criterio.MaxValue != null) {
                    if (val !== criterio.MinValue && val !== criterio.MaxValue) {
                        result.error = `Value must be either ${criterio.MinValue} or ${criterio.MaxValue}.`;
                    }
                }
            } else {
                // Caso 3: NO hay criterio en DB -> Regla Hardcodeada Legacy
                // Solo permite 508 o 610
                if (val !== 508 && val !== 610) {
                    result.error = "Value must be 508 or 610.";
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
            // Opcional: (nextData as any).NumberOfPlates = null;
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
                else if (field.name === 'Max_Production_Factor' && shouldBeActive && !(nextData as any)[field.name]) {
                    (nextData as any)[field.name] = 100;
                }

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
                position: "top-center",
                autoClose: 4000
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
                    autoClose: 2000,
                    transition: Slide
                });

                // Pequeño delay para que el usuario vea el check verde antes de cerrar
                setTimeout(() => {
                    if (onSaved && result.data) {
                        onSaved(result.data); // Actualiza la tabla en App.tsx
                    } else {
                        // Fallback por si el backend no devolvió 'data'
                        window.location.reload();
                    }
                }, 1000); // 1 segundo es suficiente

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

    // --- NUEVO LAYOUT VISUAL (GRID + PESTAÑAS IZQUIERDAS) ---
    return (
        <div className="container-fluid main-background" style={{ padding: '20px', backgroundColor: '#f4f6f9', minHeight: '100vh' }}>
            <style>{modernStyles}</style>

            <div className="row">
                {/* COLUMNA IZQUIERDA: MENÚ DE NAVEGACIÓN */}
                <div className="col-lg-2 col-md-3 mb-4">
                    <div className="nav flex-column nav-pills modern-nav" role="tablist" aria-orientation="vertical">
                        {sections.map(sectionName => {

                            // 1. CALCULAMOS LOS ERRORES DE ESTA PESTAÑA ESPECÍFICA
                            // Filtramos todos los campos que pertenecen a esta sección ('sectionName')
                            // y verificamos si existe una entrada para ellos en el objeto 'errors'.
                            const errorCount = safeFields
                                .filter(f => f.section === sectionName)
                                .filter(f => errors[f.name])
                                .length;

                            // 2. DETERMINAMOS EL ICONO
                            let iconClass = "fa-cube"; // Default (sirve para datos genéricos)

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
                            }

                            return (
                                <a
                                    key={sectionName}
                                    className={`nav-link ${activeTab === sectionName ? 'active' : ''}`}
                                    onClick={(e) => { e.preventDefault(); setActiveTab(sectionName); }}
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

                        {/* ENCABEZADO DENTRO DE LA TARJETA */}
                        <div className="form-header" style={{
                            display: 'flex',
                            justifyContent: 'space-between',
                            alignItems: 'flex-start',
                            borderBottom: '1px solid #e9ecef', /* Línea gris suave de base */
                            paddingBottom: '20px',
                            marginBottom: '25px',
                            position: 'relative'
                        }}>
                            {/* Agregamos la barra de color thyssen abajo mediante un div absoluto para asegurar que se vea */}
                            <div style={{
                                position: 'absolute',
                                bottom: '-1px', // Para tapar el borde gris
                                left: 0,
                                width: '100%',
                                height: '3px',
                                background: 'linear-gradient(90deg, #009ff5 0%, #009ff5 15%, transparent 100%)',
                                borderRadius: '2px'
                            }}></div>

                            {/* PARTE IZQUIERDA: TÍTULO Y DESCRIPCIÓN */}
                            <div className="header-text-group">
                                {/* TÍTULO PRINCIPAL CON COLOR THYSSEN (#0093d0) */}
                                <h4 style={{
                                    margin: 0,
                                    color: '#009ff5', /* 👈 AQUÍ ESTÁ EL CAMBIO DE COLOR */
                                    fontWeight: '700',
                                    letterSpacing: '-0.5px',
                                    display: 'flex',
                                    alignItems: 'center'
                                }}>
                                    {/* ICONO SIMPLIFICADO Y ROBUSTO */}
                                    <i
                                        className="fa fa-pen-to-square" /* Regresamos al clásico que suele no fallar */
                                        style={{
                                            marginRight: '10px',
                                            fontSize: '20px', /* Tamaño grande para asegurar visibilidad */
                                            color: '#009ff5'
                                        }}
                                    ></i>

                                    {selectedMaterial.ID_Material === 0 ? 'Adding New Material' : 'Editing Material'}
                                </h4>

                                {/* Texto descriptivo debajo */}
                                <p className="text-muted mb-0" style={{ fontSize: '0.9rem', marginTop: '5px', marginLeft: '0px' }}>
                                    Please review the information below. <span className="text-danger">*</span> indicates mandatory fields.
                                </p>
                            </div>

                            {/* PARTE DERECHA DEL ENCABEZADO */}
                            <button className="btn-modern-cancel" onClick={onCancel} title="Discard all changes">
                                <i className="fa fa-times-circle" style={{ fontSize: '16px' }}></i>
                                <span>Discard Changes</span>
                            </button>

                        </div>

                        {/* ÁREA DE FORMULARIO SCROLLABLE */}
                        <div className="form-body">
                            <div className="row">
                                {(activeTab === 'Coil Data' || activeTab === 'Blank Data') && (
                                    <div className="col-12 mb-3 text-right">
                                        <button
                                            type="button"
                                            className="btn btn-sm btn-outline-info shadow-sm"
                                            onClick={() => setShowToleranceModal(true)}
                                            style={{ borderRadius: '20px', fontWeight: 600 }}
                                        >
                                            <i className="fa fa-table mr-2"></i> Show Tolerance Tables
                                        </button>
                                    </div>
                                )}
                                {safeFields.map((fieldConfig, index) => {

                                    // -----------------------------------------------------------------------
                                    // 1. LÓGICA PARA EVITAR DUPLICADOS (Si ya se renderizó en un grupo)
                                    // -----------------------------------------------------------------------
                                    const prev1 = safeFields[index - 1];
                                    const prev2 = safeFields[index - 2];
                                    if (prev1?.rowTitle || prev2?.rowTitle) {
                                        return null;
                                    }

                                    // -----------------------------------------------------------------------
                                    // 2. RENDERIZADO DE GRUPO (FILA MATRIZ - Solución Definitiva)
                                    // -----------------------------------------------------------------------
                                    if (fieldConfig.rowTitle) {
                                        const field2 = safeFields[index + 1];
                                        const field3 = safeFields[index + 2];

                                        // Helper para preparar props
                                        const getFieldProps = (fc: any) => {
                                            if (!fc || !isFieldVisible(fc, formData)) return null;

                                            let isReq = fc.validation?.required || false;
                                            if (fc.validation?.requiredWhen) {
                                                const { field, is } = fc.validation.requiredWhen;
                                                const depVal = (formData as any)[field];
                                                if (is && is.includes(depVal)) isReq = true;
                                            }

                                            return {
                                                config: { ...fc, validation: { ...fc.validation, required: isReq } },
                                                value: (formData as any)[fc.name]
                                            };
                                        };

                                        const p1 = getFieldProps(fieldConfig);
                                        const p2 = getFieldProps(field2);
                                        const p3 = getFieldProps(field3);

                                        if (!p1) return null;

                                        return (
                                            <div className="col-12 mb-3 pb-2 border-bottom" key={fieldConfig.name} style={{ borderBottomColor: '#eee' }}>
                                                {/* Usamos Flexbox directo para alinear Título e Inputs */}
                                                <div style={{ display: 'flex', flexDirection: 'row' }}>

                                                    {/* 1. COLUMNA TÍTULO AZUL */}
                                                    <div style={{ width: '140px', minWidth: '140px', textAlign: 'right', paddingRight: '20px' }}>
                                                        <label style={{
                                                            color: '#009ff5',
                                                            fontWeight: 'bold',
                                                            fontSize: '1.1em',
                                                            marginTop: '38px',
                                                            display: 'block'
                                                        }}>
                                                            {fieldConfig.rowTitle}
                                                        </label>
                                                    </div>

                                                    {/* 2. GRUPO DE INPUTS */}
                                                    <div className="matrix-group-container">
                                                        <div className="matrix-cell">
                                                            <DynamicField
                                                                config={p1.config} value={p1.value}
                                                                onChange={handleFieldChange} lists={lists} urls={urls}
                                                                error={errors[fieldConfig.name]} warning={warnings[fieldConfig.name]}
                                                                isVisible={true} fullData={formData}
                                                            />
                                                        </div>
                                                        {p2 && (
                                                            <div className="matrix-cell">
                                                                <DynamicField
                                                                    config={p2.config} value={p2.value}
                                                                    onChange={handleFieldChange} lists={lists} urls={urls}
                                                                    error={errors[field2.name]} warning={warnings[field2.name]}
                                                                    isVisible={true} fullData={formData}
                                                                />
                                                            </div>
                                                        )}
                                                        {p3 && (
                                                            <div className="matrix-cell">
                                                                <DynamicField
                                                                    config={p3.config} value={p3.value}
                                                                    onChange={handleFieldChange} lists={lists} urls={urls}
                                                                    error={errors[field3.name]} warning={warnings[field3.name]}
                                                                    isVisible={true} fullData={formData}
                                                                />
                                                            </div>
                                                        )}
                                                    </div>
                                                </div>
                                            </div>
                                        );
                                    }

                                    // -----------------------------------------------------------------------
                                    // 3. RENDERIZADO NORMAL (Para el resto de campos)
                                    // -----------------------------------------------------------------------
                                    const visible = isFieldVisible(fieldConfig, formData);

                                    let isCurrentlyRequired = fieldConfig.validation?.required || false;
                                    if (fieldConfig.validation?.requiredWhen) {
                                        const { field, is } = fieldConfig.validation.requiredWhen;
                                        const dependencyValue = (formData as any)[field];
                                        if (is && is.includes(dependencyValue)) {
                                            isCurrentlyRequired = true;
                                        }
                                    }

                                    let isCurrentlyDisabled = fieldConfig.disabled;
                                    if (fieldConfig.name === 'PassesThroughSouthWarehouse') {
                                        const warehouseId = (formData as any)['ID_Arrival_Warehouse'];
                                        if (String(warehouseId) === '2') {
                                            isCurrentlyDisabled = true;
                                        }
                                    }

                                    const dynamicConfig = {
                                        ...fieldConfig,
                                        disabled: isCurrentlyDisabled,
                                        validation: {
                                            ...fieldConfig.validation,
                                            required: isCurrentlyRequired
                                        }
                                    };

                                    // Componente Base (El Input Normal)
                                    const dynamicComponent = (
                                        <DynamicField
                                            key={fieldConfig.name} // Key explícito para estabilidad
                                            config={dynamicConfig}
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
                                            value={(formData as any)[fieldConfig.name]}
                                            onChange={handleFieldChange}
                                            lists={lists} urls={urls}
                                            error={errors[fieldConfig.name]}
                                            warning={warnings[fieldConfig.name]}
                                            isVisible={visible}
                                            fullData={formData}
                                        />
                                    );

                                    // Verificamos si este es el campo "Number of Blanks" y si debe mostrar hijos
                                    const isWeldedParent = fieldConfig.name === 'NumberOfPlates';
                                    // 👇 CORRECCIÓN: Agregamos "&& visible" al final
                                    // Esto asegura que solo se muestren si la pestaña actual es la correcta.
                                    const showWeldedChildren = isWeldedParent 
                                        && (formData as any).IsWeldedBlank 
                                        && (formData as any)._weldedPlates?.length > 0
                                        && visible;
                                    // 👇 AQUÍ ESTÁ EL ARREGLO: Usamos un Fragment (<>) o un div contenedor estable
                                    // en lugar de devolver cosas distintas en un if/else.
                                    // Esto mantiene el input "Number of Blanks" montado siempre.
                                    return (
                                        <div key={fieldConfig.name + "_container"} style={{ display: 'contents' }}>

                                            {/* 1. Siempre renderizamos el componente principal igual */}
                                            {dynamicComponent}

                                            {/* 2. Renderizamos los hijos condicionalmente DEBAJO, sin reemplazar al padre */}
                                            {showWeldedChildren && (
                                                <div className="col-12 mt-2 mb-4 pl-0" key="welded-children">
                                                    <div className="card bg-light border-0">
                                                        <div className="card-body py-3 px-3">
                                                            <h6 className="text-primary small font-weight-bold mb-3" style={{ borderBottom: '1px solid #dee2e6', paddingBottom: '5px' }}>
                                                                Thickness per Blank [mm]
                                                            </h6>

                                                            <div className="row">
                                                                {(formData as any)._weldedPlates.map((plate: any, pIdx: number) => {
                                                                    // Calculamos colWidth aquí dentro para tener acceso al length actualizado
                                                                    const count = (formData as any)._weldedPlates.length;
                                                                    const colWidth = Math.max(Math.floor(12 / count), 2);

                                                                    return (
                                                                        <div key={plate.PlateNumber} className={`col-md-${colWidth} form-group`}>
                                                                            <label className="small font-weight-bold text-muted">
                                                                                Blank #{plate.PlateNumber} Thickness
                                                                            </label>
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
                                                                                <div className="input-group-append">
                                                                                    <span className="input-group-text">mm</span>
                                                                                </div>
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
        </div>
    );
}