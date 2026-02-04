import React from 'react';
import DatePicker from 'react-datepicker';
import "react-datepicker/dist/react-datepicker.css";

// CSS Mejorado: Azul corporativo y input full width
const customStyles = `
  .react-datepicker-wrapper { width: 100%; }
  .react-datepicker__input-container { width: 100%; }
  
  /* Icono de cerrar */
  .react-datepicker__close-icon { padding: 0 10px; }
  .react-datepicker__close-icon::after { background-color: #009ff5; font-size: 1rem; }

  /* ENCABEZADO AZUL (Mejora Visual) */
  .react-datepicker__header {
      background-color: #009ff5;
      border-bottom: 1px solid #0088d1;
  }
  .react-datepicker__current-month, 
  .react-datepicker-time__header, 
  .react-datepicker-year-header {
      color: white !important; /* Texto blanco */
      font-weight: bold;
  }
  .react-datepicker__navigation-icon::before {
      border-color: white; /* Flechas blancas */
  }
  
  /* Mes seleccionado azul */
  .react-datepicker__month-text--keyboard-selected,
  .react-datepicker__month-text--selected {
      background-color: #007bff;
      color: white;
  }
  .react-datepicker__month-text:hover {
      background-color: #e2e6ea;
  }
`;

interface Props {
    value: string | null; // Recibe "2025-01"
    onChange: (val: string | null) => void;
    placeholder?: string;
    disabled?: boolean;
    error?: string;
}

export default function MonthPicker({ value, onChange, placeholder, disabled, error }: Props) {

    // 1. String -> Date
    const selectedDate = React.useMemo(() => {
        if (!value) return null;
        const [year, month] = value.split('-').map(Number);
        // Validar que sean números reales antes de crear fecha
        if (!year || !month) return null;
        return new Date(year, month - 1);
    }, [value]);

    // 2. Date -> String
    const handleDateChange = (date: Date | null) => {
        if (!date) {
            onChange(null);
            return;
        }
        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        onChange(`${year}-${month}`);
    };

    return (
        <>
            <style>{customStyles}</style>
            <DatePicker
                selected={selectedDate}
                onChange={handleDateChange}
                dateFormat="yyyy-MM"
                showMonthYearPicker
                className={`form-control ${error ? 'is-invalid' : ''}`}
                placeholderText={placeholder || "yyyy-MM"}
                disabled={disabled}
                isClearable={!disabled}
                strictParsing
                onChangeRaw={(e: any) => {
                    // Verificamos que el evento y el target existan (evita el error de 'possibly undefined')
                    if (!e || !e.target) return;

                    const val = e.target.value;
                    // Si el usuario borra manualmente el texto, notificamos al padre
                    if (val === "") {
                        onChange(null);
                    }
                }}
            />
        </>
    );
}