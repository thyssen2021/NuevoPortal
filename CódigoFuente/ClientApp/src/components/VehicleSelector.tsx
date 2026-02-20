import { useMemo, memo } from 'react';
import SearchableSelect from './SearchableSelect';

interface VehicleData {
    Value: string;
    Text: string;
    SOP?: string;
    EOP?: string;
    MaxProduction?: string;
    Program?: string;
    ProductionDataJson?: string;
    Country?: string;
}

interface Props {
    value: string;
    onChange: (val: string, fullObject?: VehicleData) => void;
    countryValue: string; 
    onCountryChange: (val: string) => void;
    countries: any[];
    
    // 👇 YA NO USAMOS dataUrl. AHORA RECIBIMOS LA DATA MAESTRA.
    masterData: Record<string, VehicleData[]>; 
    error?: string;
}

const VehicleSelector = memo(function VehicleSelector({ 
    value, onChange, 
    countryValue, onCountryChange, 
    countries, masterData, error 
}: Props) {
    
    // 1. FILTRADO INSTANTÁNEO EN MEMORIA
    // Usamos useMemo para que solo recalcule si cambia el país o la data maestra.
    const vehicleOptions = useMemo(() => {
        if (!countryValue || !masterData) return [];
        
        // Buscamos directamente en el diccionario por la clave del país (ej: "Mexico")
        // Si no existe, devolvemos array vacío.
        return masterData[countryValue] || [];
    }, [countryValue, masterData]);

    // 2. Mapeo para el Select (igual que antes)
    const dropdownOptions = useMemo(() => vehicleOptions.map(v => ({
        value: v.Value,
        label: v.Text,
        ...v
    })), [vehicleOptions]);

    return (
        <div className="row">
            <div className="col-md-4">
                <label className="font-weight-bold" style={{ color: '#009ff5' }}>S&P Country</label>
                <SearchableSelect
                    value={countryValue} 
                    onChange={(val) => onCountryChange(val)}
                    options={countries}
                    placeholder="-- Country --"
                />
            </div>

            <div className="col-md-8">
                <label className="font-weight-bold" style={{ color: '#009ff5' }}>Vehicle</label>
                <SearchableSelect
                    value={value || null}
                    onChange={(val) => {
                        // Búsqueda instantánea en array local
                        const fullObj = vehicleOptions.find(v => v.Value == val);
                        onChange(val, fullObj);
                    }}
                    options={dropdownOptions}
                    disabled={!countryValue} // Solo deshabilitado si no hay país
                    isLoading={false} // ¡Ya no hay carga!
                    placeholder={countryValue ? "-- Search Vehicle --" : "-- Select Country First --"}
                    error={error}
                />
                
                {value && vehicleOptions.find(v => v.Value == value)?.SOP && (
                    <small className="text-muted d-block mt-1">
                        SOP: {vehicleOptions.find(v => v.Value === value)?.SOP} |
                        EOP: {vehicleOptions.find(v => v.Value === value)?.EOP} |
                        MaxProd: {vehicleOptions.find(v => v.Value === value)?.MaxProduction}
                    </small>
                )}
            </div>
        </div>
    );
});

export default VehicleSelector;