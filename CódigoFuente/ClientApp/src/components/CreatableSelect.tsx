import { useState, useRef, useEffect } from 'react';

// --- ESTILOS CSS INYECTADOS PARA SCROLLBAR ---
const scrollbarStyles = `
  /* Definimos el ancho del scrollbar */
  .custom-scrollbar::-webkit-scrollbar {
    width: 8px;
  }

  /* Fondo del canal (track) */
  .custom-scrollbar::-webkit-scrollbar-track {
    background: #f8f9fa; 
    border-radius: 0 0 4px 0;
  }

  /* La barra en sí (thumb) */
  .custom-scrollbar::-webkit-scrollbar-thumb {
    background-color: #cbd5e0; /* Gris suave */
    border-radius: 4px;
    border: 2px solid #f8f9fa; /* Borde blanco para efecto flotante */
  }

  /* Hover sobre la barra */
  .custom-scrollbar::-webkit-scrollbar-thumb:hover {
    background-color: #a0aec0; /* Gris un poco más oscuro */
  }
`;

interface Option {
    value: string;
    label: string;
}

interface Props {
    value: string;
    onChange: (val: string) => void;
    options: any[]; 
    placeholder?: string;
    disabled?: boolean;
    error?: string;
}

export default function CreatableSelect({ value, onChange, options, placeholder, disabled, error }: Props) {
    const [isOpen, setIsOpen] = useState(false);
    const containerRef = useRef<HTMLDivElement>(null);

    // 1. Normalizar opciones
    const safeOptions: Option[] = (options || []).map(o => ({
        value: String(o.Value || o.value || o.id || o.text || ''),
        label: String(o.Text || o.label || o.text || o.Value || '')
    }));

    // 2. Filtrar opciones
    const filteredOptions = safeOptions.filter(o => {
        // Aseguramos que ambos lados de la comparación sean Strings válidos
        const optionLabel = String(o.label || "").toLowerCase();
        const searchTerm = String(value || "").toLowerCase();
        
        return optionLabel.includes(searchTerm);
    });

    // 3. Cerrar al hacer clic fuera
    useEffect(() => {
        const handleClickOutside = (event: any) => {
            if (containerRef.current && !containerRef.current.contains(event.target)) {
                setIsOpen(false);
            }
        };
        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    const handleSelect = (val: string) => {
        onChange(val);
        setIsOpen(false);
    };

    return (
        <div className="position-relative" ref={containerRef}>
            {/* Inyectamos los estilos aquí mismo */}
            <style>{scrollbarStyles}</style>

            {/* INPUT DE TEXTO */}
            <input
                type="text"
                className={`form-control ${error ? 'is-invalid' : ''}`}
                value={value || ''}
                onChange={(e) => {
                    onChange(e.target.value);
                    setIsOpen(true);
                }}
                onFocus={() => !disabled && setIsOpen(true)}
                onClick={() => !disabled && setIsOpen(true)}
                placeholder={placeholder}
                disabled={disabled}
                autoComplete="off"
                style={{ paddingRight: '30px' }} 
            />

            {/* ICONO */}
            <i 
                className={`fa fa-chevron-down ${disabled ? 'text-muted' : 'text-primary'}`}
                style={{
                    position: 'absolute',
                    right: '10px',
                    top: '50%',
                    transform: 'translateY(-50%)',
                    pointerEvents: 'none',
                    fontSize: '0.8rem',
                    opacity: 0.6
                }}
            ></i>

            {/* LISTA DESPLEGABLE */}
            {isOpen && filteredOptions.length > 0 && !disabled && (
                <div 
                    className="shadow-sm border rounded custom-scrollbar" // 👈 Agregamos la clase aquí
                    style={{
                        position: 'absolute',
                        top: '100%',
                        left: 0,
                        width: '100%',
                        zIndex: 1050,
                        backgroundColor: '#fff',
                        maxHeight: '200px',
                        overflowY: 'auto', // Esto activa el scrollbar
                        marginTop: '2px'
                    }}
                >
                    <ul className="list-group list-group-flush mb-0">
                        {filteredOptions.map((opt, index) => (
                            <li
                                key={index}
                                className="list-group-item list-group-item-action"
                                style={{ 
                                    cursor: 'pointer', 
                                    padding: '8px 12px',
                                    fontSize: '0.9rem',
                                    borderBottom: '1px solid #f1f1f1'
                                }}
                                onMouseDown={(e) => {
                                    e.preventDefault(); 
                                    handleSelect(opt.value);
                                }}
                            >
                                {opt.label}
                            </li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
}