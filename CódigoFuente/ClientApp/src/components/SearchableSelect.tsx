// src/components/SearchableSelect.tsx
import Select, { type StylesConfig } from 'react-select';

interface OptionType {
    value: string | number;
    label: string;
    // Permitimos pasar datos extra (como SOP, EOP, etc.)
    [key: string]: any;
}

interface Props {
    value: string | number | null;
    onChange: (val: any) => void; // Devuelve el valor simple o el objeto completo según necesites
    options: any[]; // Tus listas { Value, Text }
    placeholder?: string;
    disabled?: boolean;
    error?: string;
    isLoading?: boolean;
}

export default function SearchableSelect({ value, onChange, options, placeholder, disabled, error, isLoading }: Props) {

    // 1. Transformar tus opciones {Value, Text} al formato de react-select {value, label}
    const formattedOptions: OptionType[] = options.map(opt => ({
        value: opt.Value,
        label: opt.Text,
        ...opt // Mantenemos propiedades extra (SOP, EOP) si existen
    }));

    // 2. Encontrar el objeto seleccionado actual basado en el value simple
    const selectedOption = formattedOptions.find(opt => opt.value === value) || null;

    // 3. Estilos para imitar Bootstrap 4 (height 38px, colores, focus)
    const customStyles: StylesConfig<OptionType, false> = {
        control: (provided, state) => ({
            ...provided,
            minHeight: '38px',
            height: '38px',
            backgroundColor: state.isDisabled ? '#e9ecef' : '#ffffff',
            borderColor: error ? '#dc3545' : (state.isFocused ? '#80bdff' : '#ced4da'),
            cursor: state.isDisabled ? 'not-allowed' : 'default',
            boxShadow: state.isFocused
                ? (error ? '0 0 0 0.2rem rgba(220, 53, 69, 0.25)' : '0 0 0 0.2rem rgba(0, 123, 255, 0.25)')
                : 'none',
            '&:hover': {
                borderColor: error ? '#dc3545' : '#b3b3b3'
            }
        }),
        singleValue: (provided, state) => ({
            ...provided,
            color: state.isDisabled ? '#6c757d' : '#333',
        }),
        menuList: (provided) => ({
            ...provided,
            // Estilizado de la barra para Webkit (Chrome, Edge, Safari)
            "::-webkit-scrollbar": {
                width: "9px",
                height: "9px",
            },
            "::-webkit-scrollbar-track": {
                background: "#f1f1f1"
            },
            "::-webkit-scrollbar-thumb": {
                background: "#c1c1c1",
                borderRadius: "5px"
            },
            "::-webkit-scrollbar-thumb:hover": {
                background: "#a8a8a8"
            },
            // Firefox standard (opcional)
            scrollbarWidth: "thin",
            scrollbarColor: "#c1c1c1 #f1f1f1",
        }),
        valueContainer: (provided) => ({
            ...provided,
            height: '38px',
            padding: '0 8px'
        }),
        input: (provided) => ({
            ...provided,
            margin: '0px'
        }),
        indicatorSeparator: () => ({
            display: 'none'
        }),
        indicatorsContainer: (provided) => ({
            ...provided,
            height: '38px'
        }),
        menu: (provided) => ({
            ...provided,
            zIndex: 9999 // Para que flote sobre todo
        })
    };

    return (
        <Select
            value={selectedOption}
            onChange={(selected) => {
                // Al cambiar, devolvemos solo el valor (value) para mantener consistencia con tu form
                // O null si se limpia
                onChange(selected ? (selected as OptionType).value : null);
            }}
            options={formattedOptions}
            isDisabled={disabled}
            isLoading={isLoading}
            placeholder={placeholder || "-- Select --"}
            isClearable={true} // Permite borrar con una 'x' como select2
            styles={customStyles}
            classNamePrefix="react-select"
        />
    );
}