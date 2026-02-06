// src/types/form-schema.ts

export type FieldType = 'text' | 'number' | 'select' | 'boolean' | 'date' | 'textarea' | 'file' | 'vehicle-selector' | 'checkbox'
  | 'month' | 'file' | 'creatable-select' | 'checkbox-group';

export interface ValidationRules {
  required?: boolean;
  min?: number;
  max?: number;
  minLength?: number;
  maxLength?: number;
  pattern?: RegExp;
  customMessage?: string; // Mensaje de error específico    
  requiredWhen?: {
    field: string; // El campo dependencia (ej: _projectStatusId)
    is: any[];     // Lista de valores que activan el required
  };
  accept?: string;
  maxSizeInMB?: number;
}

export interface VisibilityRule {
  field: string; // El nombre del campo del que dependo (ej: "ID_Route")
  is?: any[];     // Mostrar si el valor de 'field' está en este array (ej: [1, 5])
  hasValue?: boolean; // Opción B: Mostrar si el campo "tiene valor" (no está vacío)
}

export interface FieldConfig {
  name: string;      // Nombre exacto en la BD (key del objeto)
  label: string;     // Texto para el humano
  type: FieldType;
  section: string;   // Para agrupar visualmente (Tabs/Fieldsets)
  options?: string | any[];
  defaultValue?: any;
  placeholder?: string;
  disabled?: boolean; // O una función que retorne boolean
  showInTable?: boolean; // <--- Si es true, aparece en el resumen superior
  tableWidth?: string;   // <--- Opcional: para controlar el ancho de la columna (ej: '150px')
  step?: string;      // Para casos manuales como "any"
  decimals?: number;  // Para calcular el step automáticamente (0, 1, 2...)
  // Catálogos
  optionsKey?: string; // Clave para buscar en el contexto 'lists' (ej: "materialTypes")
  countryFieldName?: string;
  rowTitle?: string;
  // Reglas de Negocio
  validation?: ValidationRules;
  visibleWhen?: VisibilityRule;

  // UI Helpers
  className?: string; // 'col-md-3', 'col-md-6', etc.
  unit?: string;      // 'mm', 'kg', '%', etc.
  // Permite ejecutar lógica extra al cambiar un valor (ej: autocompletar fechas)
  onSideEffect?: (data: any) => void;
  uploadFieldName?: string;
}