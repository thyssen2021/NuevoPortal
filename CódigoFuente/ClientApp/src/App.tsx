// src/App.tsx
import { useState } from 'react';
import type { AppContext, Material } from './types';
import MaterialTable from './components/MaterialTable';
import MaterialForm from './components/MaterialForm';
import ProjectHeader from './components/ProjectHeader';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

interface AppProps {
  context: AppContext;
}

function App({ context }: AppProps) {
  // 1. BLINDAJE: Verificar si el contexto existe antes de usarlo
  console.log('📦 Contexto recibido en React:', context);

  // Si context o project son nulos, usamos un array vacío para no romper la app
  const initialMaterials = context?.project?.CTZ_Project_Materials || [];

  const [materials, setMaterials] = useState<Material[]>(initialMaterials);
  const [selectedMaterial, setSelectedMaterial] = useState<Material | null>(null);

  const handleEdit = (material: Material) => {
    console.log('✏️ Editando:', material);
    setSelectedMaterial(material);
  };

  const handleAdd = () => {
    const newMaterial: Material = {
      ID_Material: 0, 
      ID_Project: context.project.ID_Project,
      Part_Number: '',
      Part_Name: '',
      Vehicle: '',
      Ship_To: '',
      Annual_Volume: 0
    };
    
    //setMaterials([...materials, newMaterial]);
    setSelectedMaterial(newMaterial);
  };
  
  // 1. NUEVA FUNCIÓN: Elimina el material de la lista en memoria inmediatamente
  const handleMaterialDeleted = (id: number) => {
    setMaterials(prevMaterials => prevMaterials.filter(m => m.ID_Material !== id));
  };

  const handleMaterialSaved = (savedMaterial: Material) => {
      // Verificamos si es una edición (ya existe) o uno nuevo
      const exists = materials.some(m => m.ID_Material === savedMaterial.ID_Material);

      if (exists) {
          // Si existe, reemplazamos solo ese elemento
          setMaterials(materials.map(m => m.ID_Material === savedMaterial.ID_Material ? savedMaterial : m));
      } else {
          // Si es nuevo, lo agregamos al final de la lista
          setMaterials([...materials, savedMaterial]);
      }
      
      // Cerramos el formulario
      setSelectedMaterial(null);
  };

  const handleCloseForm = () => {
    setSelectedMaterial(null);
  }

  // 2. BLINDAJE DE RENDERIZADO
  // Si algo crítico falta, mostramos un error amigable en lugar de pantalla blanca
  if (!context || !context.project) {
    return (
      <div className="alert alert-danger m-3">
        <h4>Error de Carga</h4>
        <p>No se recibieron datos del servidor. Verifique la consola (F12) para más detalles.</p>
        <pre>{JSON.stringify(context, null, 2)}</pre>
      </div>
    );
  }

  return (
    <div className="container-fluid" style={{ padding: '0' }}>
      <div className="row">
          <div className="col-md-12">
              <ProjectHeader data={context.project} />
          </div>
      </div>
      {/* HEADER & LIST */}
      <div className="x_panel">
        <div className="x_title">
          <h2>Parts Summary <small>Total: {materials.length}</small></h2>
          <ul className="nav navbar-right panel_toolbox">
            <li>
              <button className="btn btn-success btn-sm" onClick={handleAdd}>
                <i className="fa fa-plus"></i> Add New Material
              </button>
            </li>
          </ul>
          <div className="clearfix"></div>
        </div>
        <div className="x_content">
          <MaterialTable 
            materials={materials} 
            onEdit={handleEdit} 
            // 2. PASAR LA PROPIEDAD AQUÍ
            onDeleted={handleMaterialDeleted} 
        />
        </div>
      </div>

      {/* FORMULARIO DE EDICIÓN */}
      {selectedMaterial && (
        <div className="x_panel">
           <div className="x_title">
              <h2>Part Details</h2>
              <div className="clearfix"></div>
          </div>
          <div className="x_content">
              <MaterialForm 
                  selectedMaterial={selectedMaterial} 
                  onCancel={handleCloseForm} 
                  lists={context.lists}
                  urls={context.urls}
                  interplantProcess={context.project.InterplantProcess}
                  projectStatusId={context.project.ID_Status}
                  onSaved={handleMaterialSaved}
                  projectPlantId={context.project.ID_Plant}
              />
          </div>
        </div>
      )}

      {/* 2. AGREGAR CONTENEDOR CENTRALIZADO AQUÍ */}
      <ToastContainer 
          position="top-right"
          autoClose={3000}
          hideProgressBar={false}
          newestOnTop={false}
          closeOnClick
          rtl={false}
          pauseOnFocusLoss
          draggable
          pauseOnHover
          theme="colored"
          // Opcional: zIndex alto para asegurar que se vea sobre modales bootstrap si los hubiera
          style={{ zIndex: 9999 }} 
      />

    </div>
  );
}

export default App;