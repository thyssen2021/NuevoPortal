// src/components/ConfirmationModal.tsx
//import React from 'react';

type Variant = 'danger' | 'warning' | 'info' | 'success';

interface Props {
    isOpen: boolean;
    title: string;
    message: string;
    onConfirm: () => void;
    onCancel: () => void;
    isLoading?: boolean;
    
    // Nuevas props opcionales para personalización
    confirmText?: string;
    cancelText?: string;
    variant?: Variant; // 'danger' (rojo), 'warning' (naranja/amarillo), 'info' (azul), 'success' (verde)
    icon?: string; // Clase de FontAwesome, ej: 'fa-trash'
}

const modalStyles = `
  .modern-modal-overlay {
    position: fixed;
    top: 0; left: 0; right: 0; bottom: 0;
    background-color: rgba(0, 0, 0, 0.5);
    backdrop-filter: blur(4px);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 9999;
    animation: fadeIn 0.2s ease-out;
  }

  .modern-modal-card {
    background: white;
    border-radius: 12px;
    padding: 30px;
    width: 100%;
    max-width: 400px;
    box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
    transform: scale(1);
    animation: scaleIn 0.2s ease-out;
    text-align: center;
  }

  /* Variantes de Icono */
  .modal-icon-container {
    margin: 0 auto 20px;
    width: 60px;
    height: 60px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .variant-danger .modal-icon-container { background-color: #fee2e2; }
  .variant-danger .modal-icon { color: #dc2626; }
  
  .variant-warning .modal-icon-container { background-color: #fef3c7; }
  .variant-warning .modal-icon { color: #d97706; }

  .variant-info .modal-icon-container { background-color: #e0f2fe; }
  .variant-info .modal-icon { color: #0284c7; }

  .variant-success .modal-icon-container { background-color: #dcfce7; }
  .variant-success .modal-icon { color: #16a34a; }

  .modal-icon {
    font-size: 24px;
  }

  .modal-title {
    font-size: 1.25rem;
    font-weight: 600;
    color: #111827;
    margin-bottom: 8px;
  }

  .modal-message {
    color: #6b7280;
    font-size: 0.95rem;
    margin-bottom: 25px;
    line-height: 1.5;
  }

  .modal-actions {
    display: flex;
    gap: 12px;
    justify-content: center;
  }

  .btn-modal {
    padding: 10px 20px;
    border-radius: 6px;
    font-weight: 500;
    font-size: 0.9rem;
    cursor: pointer;
    transition: all 0.2s;
    border: none;
    flex: 1;
  }

  .btn-modal-cancel {
    background-color: #f3f4f6;
    color: #374151;
  }
  .btn-modal-cancel:hover { background-color: #e5e7eb; }

  /* Variantes de Botón Confirmar */
  .variant-danger .btn-modal-confirm {
    background-color: #dc2626;
    color: white;
    box-shadow: 0 4px 6px -1px rgba(220, 38, 38, 0.3);
  }
  .variant-danger .btn-modal-confirm:hover { background-color: #b91c1c; }

  .variant-warning .btn-modal-confirm {
    background-color: #d97706;
    color: white;
    box-shadow: 0 4px 6px -1px rgba(217, 119, 6, 0.3);
  }
  .variant-warning .btn-modal-confirm:hover { background-color: #b45309; }

  .variant-info .btn-modal-confirm {
    background-color: #0284c7;
    color: white;
    box-shadow: 0 4px 6px -1px rgba(2, 132, 199, 0.3);
  }
  .variant-info .btn-modal-confirm:hover { background-color: #0369a1; }
  
  .variant-success .btn-modal-confirm {
    background-color: #16a34a;
    color: white;
    box-shadow: 0 4px 6px -1px rgba(22, 163, 74, 0.3);
  }
  .variant-success .btn-modal-confirm:hover { background-color: #15803d; }

  @keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
  @keyframes scaleIn { from { transform: scale(0.95); opacity: 0; } to { transform: scale(1); opacity: 1; } }
`;

export default function ConfirmationModal({ 
    isOpen, 
    title, 
    message, 
    onConfirm, 
    onCancel, 
    isLoading,
    confirmText = "Confirm", // Valor por defecto
    cancelText = "Cancel",
    variant = "danger",      // Valor por defecto (Rojo)
    icon = "fa-exclamation-triangle" // Icono por defecto
}: Props) {
    
    if (!isOpen) return null;

    return (
        <div className="modern-modal-overlay">
            <style>{modalStyles}</style>
            {/* Agregamos la clase de la variante al contenedor principal para que aplique los estilos CSS */}
            <div className={`modern-modal-card variant-${variant}`}>
                
                <div className="modal-icon-container">
                    <i className={`fa ${icon} modal-icon`}></i>
                </div>

                <h3 className="modal-title">{title}</h3>
                <p className="modal-message">{message}</p>

                <div className="modal-actions">
                    <button 
                        className="btn-modal btn-modal-cancel" 
                        onClick={onCancel}
                        disabled={isLoading}
                    >
                        {cancelText}
                    </button>
                    <button 
                        className="btn-modal btn-modal-confirm" 
                        onClick={onConfirm}
                        disabled={isLoading}
                    >
                        {isLoading ? (
                            <span><i className="fa fa-spinner fa-spin mr-2"></i>Processing...</span>
                        ) : confirmText}
                    </button>
                </div>
            </div>
        </div>
    );
}