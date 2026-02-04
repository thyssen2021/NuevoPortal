// src/components/ProjectHeader.tsx
import type { ProjectData } from '../types';

interface Props {
    data: ProjectData;
}

export default function ProjectHeader({ data }: Props) {
    
    // Función auxiliar con ancho dinámico (widthClass)
    const renderField = (
        label: string, 
        value: string | number | undefined | boolean, 
        widthClass: string = "col-md-3 col-sm-6 col-xs-12"
    ) => (
        <div className={`${widthClass} mb-3`}>
            <small className="text-uppercase text-muted font-weight-bold" style={{ fontSize: '11px', letterSpacing: '0.5px' }}>
                {label}
            </small>
            <div style={{ fontSize: '15px', color: '#2a3f54', fontWeight: '500', marginTop: '2px', wordBreak: 'break-word' }}>
                {value || <span className="text-muted font-italic">--</span>}
            </div>
        </div>
    );

    // --- LOGICA DE FORMATEO DE FECHA ---
    const formatDate = (dateString?: string) => {
        if (!dateString) return "";
        // Intenta cortar la parte de la hora "T"
        return dateString.split('T')[0];
    };

    return (
        <div className="x_panel shadow-sm" style={{ borderTop: '3px solid #009ff5' }}>
            <div className="x_title">
                <h2 style={{ color: '#009ff5', fontWeight: 'bold' }}>
                    <i className="fa fa-briefcase mr-2"></i>
                    Project Information
                </h2>
                <div className="clearfix"></div>
            </div>
            
            <div className="x_content">
                <div className="row">
                    {/* FILA 1 */}
                    
                    {/* Quote ID: Ancho para que quepa todo el texto */}
                    {renderField("Quote ID", data.ConcatQuoteID, "col-md-5 col-sm-12")}
                    
                    {/* Status: Leemos la descripción del objeto anidado */}
                    {renderField("Status", data.CTZ_Project_Status?.ConcatStatus || data.CTZ_Project_Status?.Description, "col-md-3 col-sm-6")}
                    
                    {/* Client: Preferimos el nombre de SAP, si no el "Otro" */}
                    {renderField("Client", data.CTZ_Clients?.ConcatSAPName || data.Cliente_Otro, "col-md-4 col-sm-6")}
                </div>
                
                <div className="row mt-2">
                    {/* FILA 2 */}

                    {/* OEM: Usamos el campo de texto directo */}
                    {renderField("OEM", data.OEM_Otro)}

                    {/* Material Owner: Descripción del objeto anidado */}
                    {renderField("Material Owner", data.CTZ_Material_Owner?.Description)}
                    
                    {/* Created By: Nombre concatenado del empleado */}
                    {renderField("Created By", data.empleados?.ConcatNombre)}
                    
                    {/* Creation Date: Usamos la propiedad "Creted_Date" (con el error de dedo original) */}
                    {renderField("Creation Date", formatDate(data.Creted_Date))}
                    
                    {/* Interplant */}
                    <div className="col-md-3 col-sm-6 col-xs-12 mb-3">
                        <small className="text-uppercase text-muted font-weight-bold" style={{ fontSize: '11px', letterSpacing: '0.5px' }}>
                            Interplant Process
                        </small>
                        <div style={{ marginTop: '5px' }}>
                            {data.InterplantProcess ? (
                                <span className="badge badge-success p-2">
                                    <i className="fa fa-check mr-1"></i> Enabled
                                </span>
                            ) : (
                                <span className="badge badge-secondary p-2" style={{ opacity: 0.7 }}>
                                    Disabled
                                </span>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}