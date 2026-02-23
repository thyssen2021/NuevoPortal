import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './index.css';

// 1. Importar todos los componentes raíz (islas) disponibles
import App from './App.tsx';
// Importar los nuevos componentes
import ConcurRequestForm from './components/ConcurRequestForm.tsx';
import ConcurITDashboard from './components/ConcurITDashboard.tsx';
import ConcurHRDashboard from './components/ConcurHRDashboard.tsx';

// 2. Registrar los componentes mapeándolos al ID del contenedor definido en Razor (.cshtml)
const componentsRegistry: Record<string, React.ElementType> = {
    'client-part-information-root': App,
    'concur-hr-form-root': ConcurRequestForm,
    'concur-it-dashboard-root': ConcurITDashboard,
    'concur-hr-dashboard-root': ConcurHRDashboard, // <- Añadir esta línea
};

// 3. Escanear el DOM y montar dinámicamente el componente correspondiente
Object.keys(componentsRegistry).forEach((elementId) => {
    const rootElement = document.getElementById(elementId);
    
    if (rootElement) {
        const Component = componentsRegistry[elementId];
        
        // Extraer datos inyectados. 
        // Priorizar el uso de 'data-payload' (aislado) con respaldo a 'window.reactInitialData' (global)
        const rawPayload = rootElement.getAttribute('data-payload');
        
        // @ts-ignore: Omitir validación estricta para objeto global inyectado desde backend
        const initialData = rawPayload ? JSON.parse(rawPayload) : (window.reactInitialData || {});

        console.log(`🚀 React montando componente en [#${elementId}]. ID Proyecto:`, initialData?.project?.ID_Project);

        createRoot(rootElement).render(
            <StrictMode>
                <Component context={initialData} />
            </StrictMode>
        );
    }
});