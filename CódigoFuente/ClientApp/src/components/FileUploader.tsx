import { useState, useRef } from 'react';
import { toast } from 'react-toastify';

// Estilos CSS reales para controlar pseudo-clases (hover, focus) correctamente
const cssStyles = `
  .file-uploader-container {
    display: flex;
    align-items: center;
    width: 100%;
    height: 38px;
    border: 1px solid #ced4da;
    border-radius: 4px;
    background-color: #fff;
    overflow: hidden;
    transition: all 0.2s ease-in-out;
    position: relative;
  }
  
  .file-uploader-container:hover {
    border-color: #80bdff;
    box-shadow: 0 0 0 0.2rem rgba(0,123,255,.15);
  }

  /* El truco para quitar el borde negro */
  .clean-btn {
    border: none;
    background: transparent;
    padding: 0 12px;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #6c757d;
    height: 100%;
    font-size: 14px;
    text-decoration: none !important;
    transition: all 0.2s;
    outline: none !important; /* Mata el borde negro */
  }

  .clean-btn:focus, .clean-btn:active {
    outline: none !important;
    box-shadow: none !important;
    background-color: #e9ecef; /* Feedback sutil en lugar de borde negro */
  }

  .clean-btn:hover {
    background-color: #e2e6ea;
    color: #495057;
  }

  /* Colores específicos por acción */
  .btn-view:hover { color: #17a2b8; background-color: #e3faff; }
  .btn-down:hover { color: #28a745; background-color: #e3ffeb; }
  .btn-edit:hover { color: #007bff; background-color: #dbeafe; }
  .btn-cancel:hover { color: #dc3545; background-color: #ffe3e3; }

  .text-section {
    flex: 1;
    padding: 0 12px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    display: flex;
    align-items: center;
    font-size: 0.9rem;
    color: #495057;
    height: 100%;
    border-right: 1px solid #ced4da;
    user-select: none;
  }
  
  .text-section.readonly {
    background-color: #e9ecef; /* Fondo gris para archivos guardados */
    color: #495057;
  }
  
  .text-section.editable {
    background-color: #fff;
    cursor: pointer;
  }
`;

interface Props {
    fileId?: number;
    fileName?: string;
    onFileSelect: (file: File | null) => void;
    downloadUrl?: string;
    accept?: string;
    maxSizeInMB?: number;
    disabled?: boolean;
}

export default function FileUploader({ 
    fileId, 
    fileName, 
    onFileSelect, 
    downloadUrl, 
    accept = ".dwg,.dxf,.dwt,.pdf,.rar,.zip",
    maxSizeInMB = 10,
    disabled 
}: Props) {
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [isEditing, setIsEditing] = useState(false);
    const [selectedFileObj, setSelectedFileObj] = useState<File | null>(null);

    // Lógica de estado
    const hasDbFile = !!fileId && !isEditing;
    const hasNewFile = !!selectedFileObj;

    // Nombre a mostrar
    const displayFileName = hasNewFile 
        ? selectedFileObj?.name 
        : (hasDbFile ? (fileName || "File Uploaded") : "");

    // ¿Es visualizable?
    const isViewable = displayFileName 
        ? /\.(pdf|jpg|jpeg|png|gif)$/i.test(displayFileName) 
        : false;

    // Helper iconos
    const getFileIcon = () => {
        if (hasNewFile) return "fa-file"; 
        if (displayFileName.endsWith('.pdf')) return "fa-file-pdf-o";
        if (/\.(jpg|png|jpeg)$/i.test(displayFileName)) return "fa-file-image-o";
        return "fa-file-text-o";
    };

    const validateFile = (file: File): boolean => {
        const maxSizeBytes = maxSizeInMB * 1024 * 1024;
        if (file.size > maxSizeBytes) {
            toast.error(`File size must be ${maxSizeInMB}MB or less.`);
            return false;
        }
        const allowedExtensions = accept.split(',').map(ext => ext.trim().toLowerCase());
        const fileExt = "." + file.name.split('.').pop()?.toLowerCase();
        
        if (!allowedExtensions.includes(fileExt)) {
            toast.error(`Invalid format. Allowed: ${accept}`);
            return false;
        }
        return true;
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            if (validateFile(file)) {
                setSelectedFileObj(file);
                onFileSelect(file);
            } else {
                if (fileInputRef.current) fileInputRef.current.value = "";
            }
        }
    };

    const handleBrowseClick = () => {
        if (!disabled) fileInputRef.current?.click();
    };

    const handleCancel = (e: React.MouseEvent) => {
        e.stopPropagation(); 
        setIsEditing(false);
        setSelectedFileObj(null);
        if (fileInputRef.current) fileInputRef.current.value = "";
        onFileSelect(null); 
    };

    return (
        <>
            <style>{cssStyles}</style>
            <div className="file-uploader-container">
                <input 
                    type="file" 
                    ref={fileInputRef}
                    onChange={handleFileChange}
                    accept={accept}
                    style={{ display: 'none' }}
                    disabled={disabled}
                />

                {/* ZONA DE TEXTO (Con fondo gris si hay archivo) */}
                <div 
                    className={`text-section ${hasDbFile ? 'readonly' : 'editable'}`}
                    onClick={(!hasDbFile && !hasNewFile) ? handleBrowseClick : undefined}
                    title={displayFileName || "Click to upload"}
                >
                    <i className={`fa ${getFileIcon()} mr-2 ${hasNewFile ? 'text-success' : 'text-primary'}`} 
                       style={{ opacity: (hasDbFile || hasNewFile) ? 1 : 0.5 }}></i>

                    {displayFileName ? (
                        <span style={{fontWeight: 500}}>{displayFileName}</span>
                    ) : (
                        <span style={{fontStyle: 'italic', color: '#6c757d'}}>Choose file...</span>
                    )}
                </div>

                {/* ZONA DE BOTONES */}
                <div style={{ display: 'flex', height: '100%', backgroundColor: '#f8f9fa' }}>
                    
                    {hasDbFile && (
                        <>
                            {isViewable && downloadUrl && (
                                <a 
                                    href={`${downloadUrl}?fileId=${fileId}&inline=true`} 
                                    target="_blank" 
                                    rel="noopener noreferrer"
                                    className="clean-btn btn-view"
                                    title="View"
                                >
                                    <i className="fa fa-eye"></i>
                                </a>
                            )}

                            {downloadUrl && (
                                <a 
                                    href={`${downloadUrl}?fileId=${fileId}&inline=false`}
                                    target="_blank" 
                                    rel="noopener noreferrer" 
                                    className="clean-btn btn-down"
                                    title="Download"
                                >
                                    <i className="fa fa-download"></i>
                                </a>
                            )}

                            <div style={{width: '1px', backgroundColor: '#dee2e6', margin: '4px 0'}}></div>

                            <button 
                                type="button"
                                onClick={() => setIsEditing(true)}
                                disabled={disabled}
                                className="clean-btn btn-edit"
                                title="Change File"
                            >
                                <i className="fa fa-pencil"></i>
                            </button>
                        </>
                    )}

                    {hasNewFile && (
                        <button 
                            type="button"
                            onClick={handleCancel}
                            className="clean-btn btn-cancel"
                            title="Remove selection"
                        >
                            <i className="fa fa-times text-danger"></i>
                        </button>
                    )}

                    {(!hasDbFile && !hasNewFile) && (
                        <button 
                            type="button"
                            onClick={handleBrowseClick}
                            disabled={disabled}
                            className="clean-btn"
                            style={{ fontWeight: 500, padding: '0 15px' }}
                        >
                            Browse
                        </button>
                    )}

                    {isEditing && !hasNewFile && (
                        <button 
                            type="button"
                            onClick={() => setIsEditing(false)}
                            className="clean-btn text-danger"
                            title="Cancel edit"
                        >
                            <i className="fa fa-undo"></i>
                        </button>
                    )}
                </div>
            </div>
        </>
    );
}