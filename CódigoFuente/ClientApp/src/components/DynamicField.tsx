// src/components/DynamicField.tsx
import type { FieldConfig } from '../types/form-schema';
import SearchableSelect from './SearchableSelect';
import MonthPicker from './MonthPicker';
// Importa el nuevo componente
import VehicleSelector from './VehicleSelector';
import FileUploader from './FileUploader'; // Importar el nuevo componente
import CreatableSelect from './CreatableSelect';

interface Props {
    config: FieldConfig;
    value: any;
    onChange: (name: string, value: any) => void;
    lists: any; // El objeto con todas tus listas (materialTypes, etc.)
    error?: string;
    warning?: string; //
    urls?: any; // Agregamos explícitamente esta prop
    isVisible?: boolean;
    onSideEffect?: (data: any) => void;
    fullData?: any;
    helperText?: string | React.ReactNode;
}

export default function DynamicField({ config, value, onChange, lists, error, warning,
    isVisible = true, fullData, onSideEffect, helperText }: Props) {

    // Función para manejar cambios de input
    const handleChange = (newValue: any) => {
        let val = newValue;

        if (config.type === 'number') {
            // Si está vacío, mandamos null para indicar "sin valor"
            // Si es un número válido, lo parseamos.
            // OJO: isNaN("") es false en JS puro (lo toma como 0), así que validamos string vacío explícitamente.
            if (val === '' || val === null || val === undefined) {
                val = null;
            } else {
                val = parseFloat(val);
            }
        }
        onChange(config.name, val);
    };

    // Renderizado según tipo
    const renderInput = () => {
        switch (config.type) {
            case 'select':
                // ------------------------------------------------------------------
                // 1. DETERINAR EL ORIGEN DE LA LISTA (MEJORADO)
                // ------------------------------------------------------------------
                let rawList: any[] = [];

                // A. Caso Nuevo: config.options es un string (ej: 'shapes') -> Buscar en lists
                if (typeof config.options === 'string') {
                    rawList = lists[config.options] || [];
                }
                // B. Caso Array Directo: config.options es ya un array -> Usarlo directo
                else if (Array.isArray(config.options)) {
                    rawList = config.options;
                }
                // C. Caso Legacy: config.optionsKey -> Buscar en lists
                else if (config.optionsKey) {
                    rawList = lists[config.optionsKey] || [];
                }

                const safeList = Array.isArray(rawList) ? rawList : [];

                // ------------------------------------------------------------------
                // 2. ADAPTACIÓN SEGURA (TU CÓDIGO EXISTENTE - INTACTO)
                // ------------------------------------------------------------------
                // Mapeamos los datos para que SearchableSelect los entienda siempre
                const formattedOptions = safeList.map((item: any) => ({
                    // Intenta leer 'Value' (C#), si no existe lee 'value' (JS), si no 'id'
                    value: item.Value !== undefined ? item.Value : (item.value !== undefined ? item.value : item.id),
                    // Intenta leer 'Text' (C#), si no existe lee 'label', si no 'text'
                    label: item.Text || item.label || item.text || ''
                }));

                return (
                    <SearchableSelect
                        value={value}
                        // SearchableSelect devuelve el valor limpio, lo pasamos directo
                        onChange={(val) => handleChange(val)}
                        options={formattedOptions}
                        disabled={config.disabled}
                        placeholder={config.placeholder}
                        error={error}
                    />
                );

            case 'number':
                // CÁLCULO DE STEP:
                // 1. Si trae 'step' explícito ("any"), úsalo.
                // 2. Si trae 'decimals', calcula 1 / 10^decimals.
                // 3. Si no trae nada, default a "1" (Enteros).
                let stepValue = "1";

                if (config.step) {
                    stepValue = config.step;
                } else if (config.decimals !== undefined) {
                    // Ej: decimals 2 -> 1/100 = 0.01
                    stepValue = (1 / Math.pow(10, config.decimals)).toFixed(config.decimals);
                }
                return (
                    <div className="input-group">
                        <input
                            type="number"
                            className={`form-control ${error ? 'is-invalid' : ''}`}
                            value={value ?? ''}
                            onChange={(e) => handleChange(e.target.value)}
                            placeholder={config.placeholder}
                            disabled={config.disabled}
                            step={stepValue}
                            style={{
                                colorScheme: 'light',
                                backgroundColor: config.disabled ? '#e9ecef' : '#fff', // Gris de Bootstrap si está deshabilitado
                                cursor: config.disabled ? 'not-allowed' : 'text',      // Cursor de bloqueo
                                fontWeight: config.disabled ? 'bold' : 'normal',      // Para que resalte el cálculo
                                color: config.disabled ? '#495057' : '#333'            // Texto un poco más tenue
                            }}
                        />
                        {config.unit && (
                            <div className="input-group-append">
                                <span className="input-group-text">{config.unit}</span>
                            </div>
                        )}
                    </div>
                );
            case 'vehicle-selector':
                return (
                    <VehicleSelector
                        value={value}
                        onChange={(val, fullData) => {
                            onChange(config.name, val);
                            if (fullData && onSideEffect) {
                                onSideEffect(fullData);
                            }
                        }}
                        countries={lists.ihsCountries || []}

                        // 👇 CAMBIO AQUÍ: Pasamos la data maestra en lugar de la URL
                        masterData={lists.vehicleMasterData || {}}

                        error={error}
                        countryValue={config.countryFieldName && fullData ? fullData[config.countryFieldName] : ''}
                        onCountryChange={(newCountry) => {
                            if (config.countryFieldName) {
                                onChange(config.countryFieldName, newCountry);
                            }
                        }}
                    />
                );
            case 'checkbox':
                return (
                    <div
                        className="form-group"
                        style={{ marginTop: config.label ? '0' : '1.9rem' }}
                    >
                        <div
                            // CONTENEDOR CLICABLE
                            onClick={() => {
                                if (!config.disabled) {
                                    handleChange(!value);
                                }
                            }}
                            style={{
                                display: 'flex',
                                alignItems: 'center',
                                padding: '0 12px',
                                border: value ? '1px solid #009ff5' : '1px solid #009ff566',
                                backgroundColor: value ? '#eef9ff' : '#fff',
                                borderRadius: '4px',
                                cursor: config.disabled ? 'not-allowed' : 'pointer',
                                transition: 'all 0.2s ease',
                                height: '38px',
                                opacity: config.disabled ? 0.6 : 1,
                                userSelect: 'none'
                            }}
                        >
                            <input
                                type="checkbox"
                                checked={!!value}
                                onChange={() => { }}
                                disabled={config.disabled}
                                style={{ display: 'none' }}
                            />

                            {/* CUADRITO DEL CHECK */}
                            <div style={{
                                width: '18px',
                                height: '18px',
                                minWidth: '18px',
                                borderRadius: '3px',
                                border: value ? 'none' : '2px solid #009ff5',
                                backgroundColor: value ? '#009ff5' : '#fff',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                marginRight: '10px',
                                transition: 'all 0.2s ease'
                            }}>
                                {value && (
                                    <i className="fa fa-check" style={{ color: '#fff', fontSize: '11px' }}></i>
                                )}
                            </div>

                            {/* 👇 AQUÍ ESTÁ EL CAMBIO: TEXTO DEL LABEL */}
                            <span style={{
                                fontSize: '13px',
                                color: value ? '#007bff' : '#495057',
                                fontWeight: value ? '600' : '500',
                                textTransform: 'uppercase',
                                letterSpacing: '0.5px'
                            }}>
                                {config.label}
                                {/* Mostramos el asterisco si es requerido */}
                                {config.validation?.required && <span className="text-danger ml-1">*</span>}
                            </span>
                        </div>
                    </div>
                );
            case 'month':
                return (
                    <MonthPicker
                        value={value} // El componente ya sabe convertir "2025-01"
                        onChange={(val) => handleChange(val)} // Devuelve string limpio
                        placeholder={config.placeholder}
                        disabled={config.disabled}
                        error={error}
                    />
                );
            case 'file':
                let displayFileName = undefined;

                // 1. Si el usuario acaba de seleccionar un archivo local (File object)
                if (value instanceof File) {
                    displayFileName = value.name;
                } 
                // 2. Si viene de la BD, leemos la data
                else if (fullData) {
                    // A. Prioridad 1: Leer el objeto anidado de Entity Framework (Ej: CTZ_Files7.Name)
                    if (config.fileEntityProp && fullData[config.fileEntityProp]) {
                        displayFileName = fullData[config.fileEntityProp].Name;
                    }
                    // B. Prioridad 2: Fallback a la propiedad plana (por si C# la manda)
                    else if (config.fileNameProp && fullData[config.fileNameProp]) {
                        displayFileName = fullData[config.fileNameProp];
                    }
                }

                const maxSize = config.validation?.maxSizeInMB || 10;
                const acceptText = config.validation?.accept
                    ? config.validation.accept.replace(/\./g, '').toUpperCase().split(',').join(', ')
                    : 'Any file type';

                return (
                    <div>
                        <FileUploader
                            fileId={value}
                            fileName={displayFileName} // 👈 Ahora sí recibirá el nombre correcto

                            onFileSelect={(file) => {
                                onChange(config.uploadFieldName || config.name, file);
                            }}
                            disabled={config.disabled}
                            accept={config.validation?.accept}
                            maxSizeInMB={maxSize}
                            downloadUrl="/CTZ_Projects/DownloadFile"
                        />

                        {!config.disabled && (
                            <small className="form-text text-muted mt-1" style={{ fontSize: '0.75rem' }}>
                                <i className="fa fa-info-circle mr-1"></i>
                                Supported formats: <strong>{acceptText}</strong>. Max size: <strong>{maxSize} MB</strong>.
                            </small>
                        )}
                    </div>
                );
            case 'textarea':
                return (
                    <div className="input-group">
                        <textarea
                            className={`form-control ${error ? 'is-invalid' : ''}`}
                            rows={3} // Altura por defecto (puedes cambiarla)
                            value={value || ''}
                            onChange={(e) => handleChange(e.target.value)}
                            placeholder={config.placeholder}
                            disabled={config.disabled}
                            style={{ resize: 'vertical', minHeight: '80px' }} // Permite al usuario ajustar alto
                        />
                        {/* Contador de caracteres opcional (visual) */}
                        {config.validation?.maxLength && (
                            <small className="text-muted w-100 text-right mt-1" style={{ fontSize: '0.75rem' }}>
                                {(value || '').length} / {config.validation.maxLength}
                            </small>
                        )}
                    </div>
                );
            case 'creatable-select':
                // si no existe, buscar en lists usando config.optionsKey.
                let optionsList = [];

                if (Array.isArray(config.options)) {
                    optionsList = config.options;
                } else if (config.optionsKey) {
                    optionsList = lists[config.optionsKey] || [];
                }

                return (
                    <CreatableSelect
                        value={value}
                        onChange={(val) => handleChange(val)}
                        options={optionsList}
                        placeholder={config.placeholder}
                        disabled={config.disabled}
                        error={error}
                    />
                );
            case 'checkbox-group':
                const groupOptions = config.optionsKey ? lists[config.optionsKey] : [];
                const selectedIds: number[] = Array.isArray(value) ? value : [];

                const handleCheck = (id: number, isChecked: boolean) => {
                    let newIds = [...selectedIds];
                    if (isChecked) {
                        newIds.push(id);
                    } else {
                        newIds = newIds.filter(x => x !== id);
                    }
                    onChange(config.name, newIds);
                };

                return (
                    // 👇 CAMBIO AQUÍ: Usamos la clase del marco
                    <div className="checkbox-group-frame">
                        {groupOptions && groupOptions.length > 0 ? (
                            groupOptions.map((opt: any) => {
                                const optId = typeof opt.Value === 'string' ? parseInt(opt.Value) : opt.Value;
                                const isChecked = selectedIds.includes(optId);
                                const isDisabled = config.disabled;

                                return (
                                    <label
                                        key={optId}
                                        className={`pro-checkbox-option ${isChecked ? 'checked' : ''} ${isDisabled ? 'disabled' : ''}`}
                                    >
                                        <input
                                            type="checkbox"
                                            checked={isChecked}
                                            disabled={isDisabled}
                                            onChange={(e) => handleCheck(optId, e.target.checked)}
                                            style={{ display: 'none' }}
                                        />

                                        <div className="pro-checkbox-icon">
                                            <i className="fa fa-check"></i>
                                        </div>

                                        <span className="pro-checkbox-label">
                                            {opt.Text}
                                        </span>
                                    </label>
                                );
                            })
                        ) : (
                            <span className="text-muted small w-100 text-center">No options available</span>
                        )}
                    </div>
                );
            case 'text':
            default:
                return (
                    <input
                        type="text"
                        className={`form-control ${error ? 'is-invalid' : ''}`}
                        value={value || ''}
                        onChange={(e) => handleChange(e.target.value)}
                        placeholder={config.placeholder}
                        disabled={config.disabled}
                    />
                );
        }

    };

    const containerClass = config.type === 'vehicle-selector' ? 'col-12' : (config.className || 'col-md-12');

    return (
        <div
            id={`field-container-${config.name}`}
            className={`form-group ${containerClass}`}
            style={{
                display: isVisible ? 'block' : 'none',
                marginBottom: isVisible ? undefined : 0 // Evita márgenes fantasma
            }}
        >
            {config.type !== 'vehicle-selector' && (
                <label className="font-weight-bold" style={{ color: '#009ff5' }}>
                    {config.label}
                    {config.validation?.required && <span className="text-danger ml-1">*</span>}
                </label>
            )}

            {renderInput()}

            {/* 👇 NUEVO BLOQUE: Texto de ayuda (Límites de ingeniería) */}
            {/* Se muestra si existe texto y NO hay un error bloqueante (para no saturar) */}
            {helperText && !error && config.type !== 'vehicle-selector' && (
                <small className="form-text text-muted mt-1 limit-display" style={{ fontSize: '0.85em' }}>
                    {helperText}
                </small>
            )}

            {error && config.type !== 'vehicle-selector' && (
                <small className="text-danger d-block">{error}</small>
            )}

            {/* Renderizado de Warning (Naranja) - Solo si NO hay error */}
            {!error && warning && config.type !== 'vehicle-selector' && (
                <small className="d-block mt-1" style={{ color: '#ec7525ff' }}>
                    <i className="fa fa-exclamation-triangle mr-1"></i>
                    {warning}
                </small>
            )}
        </div>
    );
}