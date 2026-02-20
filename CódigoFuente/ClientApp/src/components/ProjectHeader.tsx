// src/components/ProjectHeader.tsx
import { useState } from 'react';
import type { ProjectData } from '../types';

interface Props {
    data: ProjectData;
}

export default function ProjectHeader({ data }: Props) {
    // Estado para controlar si está abierto o cerrado (Por defecto abierto)
    const [isOpen, setIsOpen] = useState(true);

    // Helper para formatear fecha
    const formatDate = (dateString?: string) => {
        if (!dateString) return "--";
        return dateString.split('T')[0];
    };

    const toggleOpen = () => setIsOpen(!isOpen);

    // Estilos "Pro"
    const styles = {
        card: {
            backgroundColor: '#ffffff',
            borderRadius: '8px',
            boxShadow: '0 2px 10px rgba(0,0,0,0.05)',
            border: '1px solid #e0e0e0',
            marginBottom: '20px',
            overflow: 'hidden',
            transition: 'all 0.3s ease', // Animación suave del contenedor
        },
        topBorder: {
            height: '4px',
            background: 'linear-gradient(90deg, #009ff5 0%, #009ff5 100%)',
        },
        headerRow: {
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            padding: '15px 25px',
            cursor: 'pointer', // Indica que es clicable
            backgroundColor: isOpen ? '#fff' : '#f8f9fa', // Cambio sutil de color al cerrar
            borderBottom: isOpen ? '1px solid #f0f0f0' : 'none',
        },
        titleGroup: {
            display: 'flex',
            alignItems: 'center',
            gap: '10px',
        },
        title: {
            fontSize: '18px',
            fontWeight: 700,
            color: '#2a3f54',
            margin: 0,
        },
        toggleIcon: {
            color: '#adb5bd',
            fontSize: '14px',
            transition: 'transform 0.3s ease',
            transform: isOpen ? 'rotate(180deg)' : 'rotate(0deg)', // Animación de rotación
        },
        content: {
            padding: '20px 25px',
            display: isOpen ? 'block' : 'none', // Ocultar/Mostrar contenido
        },
        // Estilos para la vista minimizada (Solo Quote ID)
        minimizedInfo: {
            display: 'flex',
            flexDirection: 'column' as const,
            marginLeft: '30px', // Separación del título
            borderLeft: '2px solid #e9ecef',
            paddingLeft: '15px',
        },
        minimizedLabel: {
            fontSize: '10px',
            color: '#8898aa',
            fontWeight: 600,
            textTransform: 'uppercase' as const,
        },
        minimizedValue: {
            fontSize: '14px',
            color: '#009ff5',
            fontWeight: 700,
        },
        // Estilos del Grid interno
        gridContainer: {
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fill, minmax(200px, 1fr))', 
            gap: '20px 30px', 
        },
        fieldGroup: {
            display: 'flex',
            flexDirection: 'column' as const,
        },
        label: {
            fontSize: '11px',
            textTransform: 'uppercase' as const,
            color: '#8898aa',
            fontWeight: 600,
            letterSpacing: '0.5px',
            marginBottom: '4px',
        },
        value: {
            fontSize: '14px',
            color: '#2a3f54',
            fontWeight: 500,
            wordBreak: 'break-word' as const,
            lineHeight: '1.4',
        },
        // Badges
        badgeSuccess: {
            backgroundColor: '#d4edda',
            color: '#155724',
            padding: '2px 8px',
            borderRadius: '4px',
            fontSize: '12px',
            fontWeight: 600,
            display: 'inline-flex',
            alignItems: 'center',
            gap: '5px',
            width: 'fit-content'
        },
        badgeInactive: {
            backgroundColor: '#e9ecef',
            color: '#6c757d',
            padding: '2px 8px',
            borderRadius: '4px',
            fontSize: '12px',
            fontWeight: 600,
            display: 'inline-flex',
            alignItems: 'center',
            width: 'fit-content',
            border: '1px solid #dee2e6'
        }
    };

    return (
        <div style={styles.card}>
            <div style={styles.topBorder}></div>

            {/* HEADER CLICABLE */}
            <div style={styles.headerRow} onClick={toggleOpen} title={isOpen ? "Click to collapse" : "Click to expand"}>
                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <div style={styles.titleGroup}>
                        <i className="fa fa-briefcase" style={{ color: '#009ff5', fontSize: '18px' }}></i>
                        <h2 style={styles.title}>Project Information</h2>
                    </div>

                    {/* VISTA MINIMIZADA: Solo mostramos esto si está cerrado */}
                    {!isOpen && (
                        <div style={styles.minimizedInfo} className="fade-in">
                            <span style={styles.minimizedLabel}>Quote ID</span>
                            <span style={styles.minimizedValue}>{data.ConcatQuoteID || '--'}</span>
                        </div>
                    )}
                </div>

                {/* Icono de Flecha */}
                <i className="fa fa-chevron-down" style={styles.toggleIcon}></i>
            </div>

            {/* CONTENIDO EXPANDIDO */}
            <div style={styles.content}>
                <div style={styles.gridContainer}>
                    
                    {/* Quote ID */}
                    <div style={{ ...styles.fieldGroup, gridColumn: 'span 2' }}>
                        <span style={styles.label}>Quote ID</span>
                        <span style={{ ...styles.value, fontSize: '15px', fontWeight: 700, color: '#009ff5' }}>
                            {data.ConcatQuoteID || '--'}
                        </span>
                    </div>

                    {/* Client */}
                    <div style={styles.fieldGroup}>
                        <span style={styles.label}>Client</span>
                        <span style={styles.value}>
                            {data.CTZ_Clients?.ConcatSAPName || data.Cliente_Otro || '--'}
                        </span>
                    </div>

                    {/* Status */}
                    <div style={styles.fieldGroup}>
                        <span style={styles.label}>Status</span>
                        <span style={styles.value}>
                            {data.CTZ_Project_Status?.ConcatStatus || data.CTZ_Project_Status?.Description || '--'}
                        </span>
                    </div>

                    {/* Material Owner */}
                    <div style={styles.fieldGroup}>
                        <span style={styles.label}>Material Owner</span>
                        <span style={styles.value}>
                            {data.CTZ_Material_Owner?.Description || '--'}
                        </span>
                    </div>

                    {/* OEM */}
                    <div style={styles.fieldGroup}>
                        <span style={styles.label}>OEM</span>
                        <span style={styles.value}>
                            {data.OEM_Otro || '--'}
                        </span>
                    </div>

                    {/* Created By */}
                    <div style={styles.fieldGroup}>
                        <span style={styles.label}>Created By</span>
                        <span style={styles.value}>
                            <i className="fa fa-user-circle-o mr-1" style={{ color: '#ccc' }}></i>
                            {data.empleados?.ConcatNombre || '--'}
                        </span>
                    </div>

                    {/* Creation Date */}
                    <div style={styles.fieldGroup}>
                        <span style={styles.label}>Creation Date</span>
                        <span style={styles.value}>
                            {formatDate(data.Creted_Date)}
                        </span>
                    </div>

                    {/* Interplant Process (Ahora como un campo más) */}
                    <div style={styles.fieldGroup}>
                        <span style={styles.label}>Interplant Process</span>
                        <div>
                            {data.InterplantProcess ? (
                                <span style={styles.badgeSuccess}>
                                    <i className="fa fa-check"></i> Enabled
                                </span>
                            ) : (
                                <span style={styles.badgeInactive}>
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