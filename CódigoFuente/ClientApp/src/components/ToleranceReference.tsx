import React, { useState, useEffect } from 'react'; // 👈 Importar useEffect

interface Props {
    show: boolean;
    onClose: () => void;
}

// Datos convertidos del C# Legacy
const STEEL_DATA = {
    low: { // Yield < 260
        title: "Yield Strength < 260 MPa",
        rows: [
            ["0.20", "0.40", "± 0.04", "± 0.05", "± 0.06"],
            ["0.40", "0.60", "± 0.04", "± 0.05", "± 0.06"],
            ["0.60", "0.80", "± 0.05", "± 0.06", "± 0.07"],
            ["0.80", "1.00", "± 0.06", "± 0.07", "± 0.08"],
            ["1.00", "1.20", "± 0.07", "± 0.08", "± 0.09"],
            ["1.20", "1.60", "± 0.10", "± 0.11", "± 0.12"],
            ["1.60", "2.00", "± 0.12", "± 0.13", "± 0.14"],
            ["2.00", "2.50", "± 0.14", "± 0.15", "± 0.16"],
            ["2.50", "3.00", "± 0.17", "± 0.17", "± 0.18"],
            ["3.00", "5.00", "± 0.20", "± 0.20", "± 0.21"],
            ["5.00", "6.50", "± 0.22", "± 0.22", "± 0.23"],
        ]
    },
    med: { // 260 - 360
        title: "260 ≤ Yield < 360 MPa",
        rows: [
            ["0.20", "0.40", "± 0.05", "± 0.06", "± 0.07"],
            ["0.40", "0.60", "± 0.05", "± 0.06", "± 0.07"],
            ["0.60", "0.80", "± 0.06", "± 0.07", "± 0.08"],
            ["0.80", "1.00", "± 0.07", "± 0.08", "± 0.09"],
            ["1.00", "1.20", "± 0.08", "± 0.09", "± 0.11"],
            ["1.20", "1.60", "± 0.11", "± 0.13", "± 0.14"],
            ["1.60", "2.00", "± 0.14", "± 0.15", "± 0.16"],
            ["2.00", "2.50", "± 0.16", "± 0.17", "± 0.18"],
            ["2.50", "3.00", "± 0.19", "± 0.20", "± 0.20"],
            ["3.00", "5.00", "± 0.22", "± 0.24", "± 0.25"],
            ["5.00", "6.50", "± 0.24", "± 0.25", "± 0.26"],
        ]
    },
    high: { // 360 - 420
        title: "360 ≤ Yield ≤ 420 MPa",
        rows: [
            ["0.35", "0.40", "± 0.05", "± 0.06", "± 0.07"],
            ["0.40", "0.60", "± 0.06", "± 0.07", "± 0.08"],
            ["0.60", "0.80", "± 0.07", "± 0.08", "± 0.09"],
            ["0.80", "1.00", "± 0.08", "± 0.09", "± 0.11"],
            ["1.00", "1.20", "± 0.10", "± 0.11", "± 0.12"],
            ["1.20", "1.60", "± 0.13", "± 0.14", "± 0.16"],
            ["1.60", "2.00", "± 0.16", "± 0.17", "± 0.19"],
            ["2.00", "2.50", "± 0.18", "± 0.20", "± 0.21"],
            ["2.50", "3.00", "± 0.22", "± 0.22", "± 0.23"],
            ["3.00", "5.00", "± 0.22", "± 0.24", "± 0.25"],
            ["5.00", "6.50", "± 0.24", "± 0.25", "± 0.26"],
        ]
    },
    ultra: { // 420 - 900
        title: "420 < Yield ≤ 900 MPa",
        rows: [
            ["0.35", "0.40", "± 0.06", "± 0.07", "± 0.08"],
            ["0.40", "0.60", "± 0.06", "± 0.08", "± 0.09"],
            ["0.60", "0.80", "± 0.07", "± 0.09", "± 0.11"],
            ["0.80", "1.00", "± 0.09", "± 0.11", "± 0.12"],
            ["1.00", "1.20", "± 0.11", "± 0.13", "± 0.14"],
            ["1.20", "1.60", "± 0.15", "± 0.16", "± 0.18"],
            ["1.60", "2.00", "± 0.18", "± 0.19", "± 0.21"],
            ["2.00", "2.50", "± 0.21", "± 0.22", "± 0.24"],
            ["2.50", "3.00", "± 0.24", "± 0.25", "± 0.26"],
            ["3.00", "5.00", "± 0.26", "± 0.27", "± 0.28"],
            ["5.00", "6.50", "± 0.28", "± 0.29", "± 0.30"],
        ]
    }
};

export default function ToleranceReference({ show, onClose }: Props) {
    const [activeTab, setActiveTab] = useState('width');

    // -------------------------------------------------------------------
    // 1. BLOQUEO DE SCROLL DEL BODY 👈 NUEVO
    // -------------------------------------------------------------------
    useEffect(() => {
        if (show) {
            // Al abrir, quitamos el scroll de la página de fondo
            document.body.style.overflow = 'hidden';
        } else {
            // Al cerrar (o si no se muestra), restauramos el scroll
            document.body.style.overflow = 'unset';
        }

        // Cleanup: Si el componente se desmonta, aseguramos restaurar el scroll
        return () => {
            document.body.style.overflow = 'unset';
        };
    }, [show]);

    if (!show) return null;

    // Estilos inline para el modal (para no depender de jQuery/Bootstrap JS)
    const modalStyle: React.CSSProperties = {
        display: 'block',
        backgroundColor: 'rgba(0,0,0,0.5)',
        position: 'fixed',
        top: 0, left: 0, right: 0, bottom: 0,
        zIndex: 1050,
        overflowX: 'hidden',
        overflowY: 'auto'
    };

    const headerStyle = { backgroundColor: '#009ff5', color: 'white' };
    const subHeaderStyle = { backgroundColor: '#008cd9', color: 'white', borderRight: '1px solid white' };
    const lightCellStyle = { backgroundColor: '#e6f7ff', fontWeight: 500 };
    const borderRightStyle = { borderRight: '2px solid #009ff5' };

    const renderSteelTable = (key: keyof typeof STEEL_DATA) => {
        const data = STEEL_DATA[key];
        return (
            <div className="table-responsive">
                <h6 className="text-primary font-weight-bold mb-3">{data.title}</h6>
                <table className="table table-bordered table-sm text-center table-hover" style={{ border: '1px solid #009ff5' }}>
                    <thead style={headerStyle}>
                        <tr>
                            <th colSpan={3} style={{ borderRight: '1px solid white' }}>Nominal Thickness (t) [mm]</th>
                            <th colSpan={3}>Normal Tolerances [mm]</th>
                        </tr>
                        <tr style={{ fontSize: '0.9em' }}>
                            <th style={subHeaderStyle}>Min &lt;</th>
                            <th style={subHeaderStyle}>t</th>
                            <th style={{ ...subHeaderStyle, ...borderRightStyle }}>&le; Max</th>
                            <th style={subHeaderStyle}>w &le; 1,200</th>
                            <th style={subHeaderStyle}>1,200 &lt; w &le; 1,500</th>
                            <th style={{ backgroundColor: '#008cd9', color: 'white' }}>w &gt; 1,500</th>
                        </tr>
                    </thead>
                    <tbody>
                        {data.rows.map((row, idx) => (
                            <tr key={idx}>
                                <td style={lightCellStyle}>{row[0]}</td>
                                <td style={{ backgroundColor: '#e6f7ff' }}>&lt; t &le;</td>
                                <td style={{ ...lightCellStyle, ...borderRightStyle }}>{row[1]}</td>
                                <td>{row[2]}</td>
                                <td>{row[3]}</td>
                                <td>{row[4]}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        );
    };

    return (
        <div className="modal fade show" style={modalStyle} tabIndex={-1} role="dialog" onClick={onClose}>
            <div className="modal-dialog modal-lg" role="document" style={{ maxWidth: '900px' }} onClick={(e) => e.stopPropagation()}>
                <div className="modal-content shadow-lg" style={{ border: 'none', borderRadius: '12px' }}>
                    
                    {/* Header Azul Moderno */}
                    <div className="modal-header" style={{ ...headerStyle, borderTopLeftRadius: '12px', borderTopRightRadius: '12px' }}>
                        <h5 className="modal-title font-weight-bold">
                            <i className="fa fa-ruler-combined mr-2"></i> Standard Tolerances Reference
                        </h5>
                        <button type="button" className="close" onClick={onClose} style={{ color: 'white', opacity: 1 }}>
                            <span>&times;</span>
                        </button>
                    </div>

                    <div className="modal-body p-4">
                        {/* Pestañas de Navegación */}
                        <ul className="nav nav-tabs mb-4">
                            <li className="nav-item">
                                <a className={`nav-link ${activeTab === 'width' ? 'active font-weight-bold' : ''}`} 
                                   onClick={() => setActiveTab('width')} style={{ cursor: 'pointer', color: activeTab === 'width' ? '#009ff5' : '#6c757d' }}>
                                   Width
                                </a>
                            </li>
                            <li className="nav-item">
                                <a className={`nav-link ${activeTab === 'low' ? 'active font-weight-bold' : ''}`} 
                                   onClick={() => setActiveTab('low')} style={{ cursor: 'pointer', color: activeTab === 'low' ? '#009ff5' : '#6c757d' }}>
                                   Yield &lt; 260
                                </a>
                            </li>
                            <li className="nav-item">
                                <a className={`nav-link ${activeTab === 'med' ? 'active font-weight-bold' : ''}`} 
                                   onClick={() => setActiveTab('med')} style={{ cursor: 'pointer', color: activeTab === 'med' ? '#009ff5' : '#6c757d' }}>
                                   260 &le; R &lt; 360
                                </a>
                            </li>
                            <li className="nav-item">
                                <a className={`nav-link ${activeTab === 'high' ? 'active font-weight-bold' : ''}`} 
                                   onClick={() => setActiveTab('high')} style={{ cursor: 'pointer', color: activeTab === 'high' ? '#009ff5' : '#6c757d' }}>
                                   360 &le; R &le; 420
                                </a>
                            </li>
                            <li className="nav-item">
                                <a className={`nav-link ${activeTab === 'ultra' ? 'active font-weight-bold' : ''}`} 
                                   onClick={() => setActiveTab('ultra')} style={{ cursor: 'pointer', color: activeTab === 'ultra' ? '#009ff5' : '#6c757d' }}>
                                   420 &lt; R &le; 900
                                </a>
                            </li>
                        </ul>

                        {/* Contenido Dinámico */}
                        <div className="tab-content">
                            {activeTab === 'width' && (
                                <div className="fade show active">
                                    <h6 className="text-primary font-weight-bold mb-3">Width Tolerances</h6>
                                    <table className="table table-bordered table-sm text-center table-hover" style={{ border: '1px solid #009ff5' }}>
                                        <thead style={headerStyle}>
                                            <tr>
                                                <th colSpan={3} style={{ borderRight: '1px solid white' }}>Nominal Width (w) [mm]</th>
                                                <th>Normal Tolerances [mm]</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {/* Datos Hardcodeados del Legacy Width */}
                                            <tr>
                                                <td style={lightCellStyle}>600</td>
                                                <td style={{ backgroundColor: '#e6f7ff' }}>&le; w &le;</td>
                                                <td style={{ ...lightCellStyle, ...borderRightStyle }}>1,200</td>
                                                <td className="font-weight-bold">+ 5</td>
                                            </tr>
                                            <tr>
                                                <td style={lightCellStyle}>1,200</td>
                                                <td style={{ backgroundColor: '#e6f7ff' }}>&lt; w &le;</td>
                                                <td style={{ ...lightCellStyle, ...borderRightStyle }}>1,500</td>
                                                <td className="font-weight-bold">+ 6</td>
                                            </tr>
                                            <tr>
                                                <td style={lightCellStyle}>1,500</td>
                                                <td style={{ backgroundColor: '#e6f7ff' }}>&lt; w &le;</td>
                                                <td style={{ ...lightCellStyle, ...borderRightStyle }}>1,800</td>
                                                <td className="font-weight-bold">+ 7</td>
                                            </tr>
                                            <tr>
                                                <td style={{ backgroundColor: '#e6f7ff' }}></td>
                                                <td style={{ backgroundColor: '#e6f7ff' }}>w &gt;</td>
                                                <td style={{ ...lightCellStyle, ...borderRightStyle }}>1,800</td>
                                                <td className="font-weight-bold">+ 8</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            )}

                            {activeTab !== 'width' && renderSteelTable(activeTab as any)}
                        </div>
                    </div>

                    <div className="modal-footer bg-light" style={{ borderBottomLeftRadius: '12px', borderBottomRightRadius: '12px' }}>
                        <button type="button" className="btn btn-secondary" onClick={onClose}>Close Reference</button>
                    </div>
                </div>
            </div>
        </div>
    );
}