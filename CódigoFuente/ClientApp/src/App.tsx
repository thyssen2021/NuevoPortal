// src/App.tsx
import { useState } from 'react';
import type { AppContext, Material } from './types';
import MaterialTable from './components/MaterialTable';
import MaterialForm from './components/MaterialForm';
import ProjectHeader from './components/ProjectHeader';
import ConfirmationModal from './components/ConfirmationModal';
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

  // --- ESTADOS DE MODALES ---
  const [materialToCopy, setMaterialToCopy] = useState<Material | null>(null);
  const [showCopyConfirm, setShowCopyConfirm] = useState(false);

  // NUEVO: Estado para editar
  const [materialToEdit, setMaterialToEdit] = useState<Material | null>(null);
  const [showEditConfirm, setShowEditConfirm] = useState(false);

  // --- HANDLERS ---

  // 1. AÑADIR NUEVO
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
    setSelectedMaterial(newMaterial);
  };

  // 2. EDITAR (Nuevo con Advertencia)
  const handleEditRequest = (material: Material) => {
    // Si ya hay un formulario abierto y es diferente al que queremos abrir
    if (selectedMaterial && selectedMaterial.ID_Material !== material.ID_Material) {
        setMaterialToEdit(material);
        setShowEditConfirm(true);
    } else {
        // Si no hay nada abierto, abrimos directo
        setSelectedMaterial(material);
    }
  };

  const confirmEdit = () => {
    if (materialToEdit) {
      setSelectedMaterial(materialToEdit);
      setShowEditConfirm(false);
      setMaterialToEdit(null);
      
      // Scroll suave hacia el formulario
      setTimeout(() => {
        const formElement = document.querySelector('.x_panel:last-child');
        if(formElement) formElement.scrollIntoView({ behavior: 'smooth' });
      }, 100);
    }
  };

  // 3. COPIAR
  const handleCopyRequest = (material: Material) => {
    setMaterialToCopy(material);
    setShowCopyConfirm(true);
  };

  const confirmCopy = () => {
    if (materialToCopy) {
      // Crear copia profunda y limpiar IDs para que sea un NUEVO registro
      const newMaterial = {
        ...materialToCopy,
        ID_Material: 0, // CRÍTICO: ID 0 indica "Nuevo" al backend
        // Opcional: Limpiar archivos si no deben clonarse físicamente
      };

      setSelectedMaterial(newMaterial);
      setShowCopyConfirm(false);
      setMaterialToCopy(null);

      // Scroll suave
      setTimeout(() => {
        const formElement = document.querySelector('.x_panel:last-child');
        if (formElement) formElement.scrollIntoView({ behavior: 'smooth' });
      }, 100);
    }
  };

  // 4. ELIMINAR Y GUARDAR
  const handleMaterialDeleted = (id: number) => {
    setMaterials(prevMaterials => prevMaterials.filter(m => m.ID_Material !== id));
  };

  const handleMaterialSaved = (savedMaterial: Material) => {
    const exists = materials.some(m => m.ID_Material === savedMaterial.ID_Material);
    if (exists) {
      setMaterials(materials.map(m => m.ID_Material === savedMaterial.ID_Material ? savedMaterial : m));
    } else {
      setMaterials([...materials, savedMaterial]);
    }
    setSelectedMaterial(null);
  };

  const handleCloseForm = () => {
    setSelectedMaterial(null);
  };

  // 5. BLINDAJE DE RENDERIZADO
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
            onEdit={handleEditRequest} // Usamos el request con advertencia
            onDeleted={handleMaterialDeleted}
            onCopy={handleCopyRequest}
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
              projectMaterials={materials}
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
        style={{ zIndex: 9999 }}
      />

      {/* MODAL 1: COPIAR (Info) */}
      <ConfirmationModal
        isOpen={showCopyConfirm}
        title="Copy Material"
        message="This will create a new material entry with data copied from the selected item. Unsaved changes in the current form (if open) might be lost. Do you want to continue?"
        onConfirm={confirmCopy}
        onCancel={() => setShowCopyConfirm(false)}
        isLoading={false}
        confirmText="Yes, Copy Data"
        variant="info"
        icon="fa-copy"
      />

      {/* MODAL 2: EDITAR (Warning) */}
      <ConfirmationModal
        isOpen={showEditConfirm}
        title="Edit Material"
        message="You are about to load a different material for editing. If you have unsaved changes in the current form, they will be lost. Do you want to continue?"
        onConfirm={confirmEdit}
        onCancel={() => setShowEditConfirm(false)}
        isLoading={false}
        confirmText="Yes, Edit"
        variant="warning"
        icon="fa-pen-to-square"
      />

    </div>
  );
}

export default App;