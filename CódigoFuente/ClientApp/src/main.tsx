import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'

// 1. Buscamos el ID definido en tu nueva vista
const rootElement = document.getElementById('client-part-information-root');

if (rootElement) {
  
  // 2. Leemos los datos inyectados desde Razor (C#)
  // @ts-ignore (Ignoramos el error de TS porque window.reactInitialData es dinámico)
  const initialData = window.reactInitialData;

  console.log("🚀 React iniciándose con datos del proyecto ID:", initialData?.project?.ID_Project);

  createRoot(rootElement).render(
    <StrictMode>
      {/* Pasamos los datos como props principales a la App */}
      <App context={initialData} />
    </StrictMode>,
  )
}