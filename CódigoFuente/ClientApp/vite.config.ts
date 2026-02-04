import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  base: '/Scripts/ReactDist/',
  build: {
    // ESTA ES LA RUTA EXACTA CALCULADA:
    // "Sal de ClientApp (../), entra a BitacorasProduccion, entra a Scripts, crea ReactDist"
    outDir: '../BitacorasProduccion/Scripts/ReactDist',
    chunkSizeWarningLimit: 5000,
    emptyOutDir: true, // Borra la carpeta ReactDist antes de volver a compilar
    rollupOptions: {
      input: 'src/main.tsx',
      output: {
        // Nombres fijos para no tener hashes raros
        entryFileNames: 'assets/main.js',
        chunkFileNames: 'assets/[name].js',
        assetFileNames: 'assets/[name].[ext]'
      },
      
    }
  }
})