import React from 'react';
import type { LineCapacityGroup } from '../types';

interface Props {
    data: LineCapacityGroup[];
}

export const CapacityBreakdownTable: React.FC<Props> = ({ data }) => {
    if (!data || data.length === 0) return null;

    // --- ESTILOS MODERNOS Y COMPACTOS INLINE ---
    const s = {
        cardContainer: {
            background: '#ffffff',
            borderRadius: '10px',
            boxShadow: '0 2px 10px rgba(0,0,0,0.04)',
            border: '1px solid #e2e5e8',
            overflow: 'hidden'
        },
        groupWrapper: {
            background: '#ffffff',
            borderRadius: '8px',
            margin: '0 10px 10px 10px',
            border: '1px solid #e9ecef',
            boxShadow: '0 1px 4px rgba(0,0,0,0.02)'
        },
        groupHeader: {
            background: '#f8f9fa',
            padding: '6px 16px', // 👈 Más delgado
            borderBottom: '1px solid #e9ecef',
            borderTopLeftRadius: '8px',
            borderTopRightRadius: '8px',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between'
        },
        yearHeaderPill: {
            background: '#009ff5',
            color: 'white',
            padding: '2px 10px', // 👈 Píldora más ajustada
            borderRadius: '12px',
            fontSize: '0.7rem',
            fontWeight: 'bold',
            letterSpacing: '0.5px'
        },
        tableRow: {
            borderBottom: '1px solid #f4f6f9',
            transition: 'background-color 0.2s ease'
        },
        editingRow: {
            background: 'linear-gradient(90deg, rgba(0,159,245,0.05) 0%, transparent 100%)',
            borderLeft: '3px solid #009ff5'
        },
        totalFooter: {
            background: '#fafbfd',
            borderTop: '2px dashed #dee2e6',
            borderBottomLeftRadius: '8px',
            borderBottomRightRadius: '8px'
        },
        statusBadgeWrapper: {
            display: 'inline-flex',
            alignItems: 'center',
            padding: '2px 8px', // 👈 Badge más pequeño
            borderRadius: '4px',
            background: '#f1f3f5',
            fontSize: '0.7rem',
            color: '#6c757d',
            fontWeight: 600,
            border: '1px solid #e9ecef'
        }
    };

    return (
        <div style={s.cardContainer} className="mb-3">
            {/* ENCABEZADO GLOBAL (Más fino) */}
            <div className="bg-white p-2 px-3 border-bottom d-flex justify-content-between align-items-center">
                <div className="d-flex align-items-center">
                    <i className="fa fa-layer-group mr-2" style={{ color: '#009ff5', fontSize: '1.1rem' }}></i>
                    <h6 className="mb-0" style={{ color: '#2c3e50', fontWeight: 700 }}>
                        Capacity Simulation
                    </h6>
                </div>                
            </div>

            {/* CUERPO - LISTA DE GRUPOS (LÍNEAS) */}
            <div className="p-2 bg-light">
                {data.map(group => {
                    const fyColumns = group.activeFYs;
                    const hasData = fyColumns.length > 0;

                    return (
                        <div key={group.lineId} style={s.groupWrapper}>
                            
                            {/* --- 1. CABECERA DEL GRUPO (Nombre de Línea) --- */}
                            <div style={s.groupHeader}>
                                <div className="d-flex align-items-center">
                                    <i className="fa fa-cogs text-secondary mr-2"></i>
                                    <strong style={{ color: '#344563', fontSize: '0.9rem' }}>
                                        {group.lineName}
                                    </strong>
                                </div>
                                <div className="text-muted" style={{ fontSize: '0.7rem', fontWeight: 600 }}>
                                    {hasData ? `${group.materials.length} ITEM(S)` : 'AWAITING DATA'}
                                </div>
                            </div>

                            {/* --- 2. TABLA DE MATERIALES --- */}
                            <div className="table-responsive" style={{ margin: 0 }}>
                                <table className="table table-borderless table-sm mb-0 align-middle" style={{ minWidth: '700px' }}>
                                    
                                    {/* Encabezados de Columnas */}
                                    <thead style={{ borderBottom: '1px solid #f0f2f5' }}>
                                        <tr>
                                            <th className="text-muted font-weight-bold text-uppercase pl-3 py-1" style={{ width: '35%', fontSize: '0.7rem' }}>Part Number</th>
                                            <th className="text-muted font-weight-bold text-uppercase text-center py-1" style={{ width: '15%', fontSize: '0.7rem' }}>State</th>
                                            <th className="text-muted font-weight-bold text-uppercase text-center py-1" style={{ width: '15%', fontSize: '0.7rem' }}>Status</th>
                                            
                                            {/* Columnas Dinámicas FY */}
                                            {hasData ? (
                                                fyColumns.map(fy => (
                                                    <th key={fy} className="text-center py-1" style={{ width: 'auto' }}>
                                                        <span style={s.yearHeaderPill}>{fy}</span>
                                                    </th>
                                                ))
                                            ) : (
                                                <th className="text-muted font-italic text-center py-1" style={{fontSize: '0.7rem'}}>Timeline</th>
                                            )}
                                        </tr>
                                    </thead>

                                    {/* Filas de Materiales */}
                                    <tbody>
                                        {group.materials.map((mat, idx) => {
                                            const isValid = mat.validation.isValid;
                                            const isEditing = mat.isCurrentEditing;
                                            const rowStyle = isEditing ? s.editingRow : s.tableRow;

                                            return (
                                                <tr key={`${group.lineId}-${idx}`} style={rowStyle} className={isEditing ? 'bg-white' : ''}>
                                                    
                                                    {/* Nombre del Material */}
                                                    <td className="pl-3 py-1">
                                                        <div className="d-flex flex-column justify-content-center" style={{ minHeight: '32px' }}>
                                                            <div className="d-flex align-items-center">
                                                                {isEditing && (
                                                                    <span className="badge badge-primary mr-2" style={{ backgroundColor: '#009ff5', fontSize: '0.6rem', padding: '2px 5px' }}>EDITING</span>
                                                                )}
                                                                <strong style={{ color: isEditing ? '#009ff5' : '#495057', fontSize: '0.85rem' }}>
                                                                    {mat.partNumber}
                                                                </strong>
                                                            </div>
                                                            {/* Errores (Apretados para no ocupar espacio extra) */}
                                                            {!isValid && (
                                                                <div className="mt-0" style={{ fontSize: '0.65rem', color: '#e74c3c', lineHeight: '1.1' }}>
                                                                    <strong>Missing:</strong> {mat.validation.missingFields.join(', ')}
                                                                </div>
                                                            )}
                                                        </div>
                                                    </td>

                                                    {/* State (Válido / Inválido) */}
                                                    <td className="text-center py-1">
                                                        {isValid ? (
                                                            <span className="text-success" style={{ fontSize: '0.85rem' }}>
                                                                <i className="fa fa-check-circle"></i>
                                                            </span>
                                                        ) : (
                                                            <span className="text-warning" style={{ fontSize: '0.85rem' }} title="Incomplete">
                                                                <i className="fa fa-exclamation-triangle"></i>
                                                            </span>
                                                        )}
                                                    </td>

                                                    {/* Project Status */}
                                                    <td className="text-center py-1">
                                                        <div style={s.statusBadgeWrapper}>
                                                            {mat.projectStatus}
                                                        </div>
                                                    </td>

                                                    {/* Celdas de Porcentaje */}
                                                    {hasData ? (
                                                        fyColumns.map(fy => {
                                                            const val = mat.fyBreakdown[fy];
                                                            return (
                                                                <td key={fy} className="text-center py-1">
                                                                    {val ? (
                                                                        <span style={{ color: '#34495e', fontSize: '0.85rem', fontWeight: 600 }}>
                                                                            {val.toFixed(1)}%
                                                                        </span>
                                                                    ) : (
                                                                        <span style={{ color: '#ced4da', fontSize: '0.85rem' }}>—</span>
                                                                    )}
                                                                </td>
                                                            );
                                                        })
                                                    ) : (
                                                        <td className="text-center text-muted font-italic py-1" style={{fontSize: '0.75rem'}}>
                                                            ...
                                                        </td>
                                                    )}
                                                </tr>
                                            );
                                        })}
                                    </tbody>

                                    {/* --- 3. FOOTER DEL GRUPO (Totales) --- */}
                                    {hasData && (
                                        <tfoot style={s.totalFooter}>
                                            <tr>
                                                <td colSpan={3} className="text-right pr-3 py-2 align-middle">
                                                    <span className="text-muted font-weight-bold text-uppercase" style={{ fontSize: '0.7rem' }}>
                                                        Total Line Load
                                                    </span>
                                                </td>
                                                
                                                {fyColumns.map(fy => {
                                                    const total = group.lineTotals[fy] || 0;
                                                    
                                                    // Semáforo
                                                    let colorHex = '#28a745'; // Success
                                                    let bgHex = '#e6f4ea';
                                                    if (total > 100) {
                                                        colorHex = '#dc3545'; // Danger
                                                        bgHex = '#fdf3f4'; // Fallback a gris/rojo tenue
                                                    } else if (total > 85) {
                                                        colorHex = '#ffc107'; // Warning
                                                        bgHex = '#fffcf2';
                                                    }

                                                    return (
                                                        <td key={fy} className="text-center py-2">
                                                            <div style={{
                                                                display: 'inline-block',
                                                                padding: '2px 8px',
                                                                borderRadius: '6px',
                                                                backgroundColor: bgHex,
                                                                color: colorHex === '#ffc107' ? '#d39e00' : colorHex, // Ajuste para legibilidad del amarillo
                                                                fontWeight: 700,
                                                                fontSize: '0.85rem',
                                                                border: `1px solid ${colorHex}40`
                                                            }}>
                                                                {total.toFixed(1)}%
                                                            </div>
                                                        </td>
                                                    );
                                                })}
                                            </tr>
                                        </tfoot>
                                    )}
                                </table>
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
};