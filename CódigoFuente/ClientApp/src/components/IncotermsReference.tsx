import React, { useState, useEffect } from 'react'; // 👈 Importar useEffect

interface Props {
    show: boolean;
    onClose: () => void;
}

// --- DATA: DEFINICIONES ---
const DEFINITIONS = {
    EN: [
        { code: "EXW (Ex Works)", app: "Any mode", desc: "Seller makes goods available at their premises; buyer bears all subsequent costs and risks." },
        { code: "FCA (Free Carrier)", app: "Any mode", desc: "Seller delivers goods to the carrier chosen by the buyer at an agreed location." },
        { code: "CPT (Carriage Paid To)", app: "Any mode", desc: "Seller pays transport to the destination; risk transfers when goods are handed to the carrier." },
        { code: "CIP (Carriage and Insurance Paid To)", app: "Any mode", desc: "Same as CPT, but seller must provide minimum cargo insurance." },
        { code: "DAP (Delivered at Place)", app: "Any mode", desc: "Seller delivers goods ready for unloading at destination; buyer handles import clearance." },
        { code: "DPU (Delivered at Place Unloaded)", app: "Any mode", desc: "Same as DAP, but seller also unloads goods at destination." },
        { code: "DDP (Delivered Duty Paid)", app: "Any mode", desc: "Seller bears all costs, including import duties and customs clearance." },
        { code: "FAS (Free Alongside Ship)", app: "Sea/Inland", desc: "Seller places goods alongside the vessel at the port of shipment." },
        { code: "FOB (Free On Board)", app: "Sea/Inland", desc: "Seller delivers goods once they are on board the vessel." },
        { code: "CFR (Cost and Freight)", app: "Sea/Inland", desc: "Seller pays freight to destination port; risk transfers when goods are on board." },
        { code: "CIF (Cost, Insurance & Freight)", app: "Sea/Inland", desc: "Same as CFR, but seller must provide minimum insurance." },
        { code: "DEL (Delivery to...)", app: "Generic", desc: "No official ICC Incoterms rules" }
    ],
    ES: [
        { code: "EXW (Ex Works)", app: "Cualquier modo", desc: "Vendedor pone mercancía a disposición en su local; comprador asume todo lo demás." },
        { code: "FCA (Free Carrier)", app: "Cualquier modo", desc: "Vendedor entrega al transportista designado por el comprador en un lugar convenido." },
        { code: "CPT (Carriage Paid To)", app: "Cualquier modo", desc: "Vendedor paga transporte hasta destino; riesgo se transfiere al comprador al entregar al transportista." },
        { code: "CIP (Carriage and Insurance Paid To)", app: "Cualquier modo", desc: "Igual que CPT, pero vendedor contrata seguro mínimo; mejor protección." },
        { code: "DAP (Delivered at Place)", app: "Cualquier modo", desc: "Vendedor entrega la mercancía en un lugar convenido en destino; comprador asume importación/aranceles." },
        { code: "DPU (Delivered at Place Unloaded)", app: "Cualquier modo", desc: "Igual que DAP, pero el vendedor además descarga la mercancía en destino." },
        { code: "DDP (Delivered Duty Paid)", app: "Cualquier modo", desc: "Vendedor asume todos los costos, transporte, descarga, aranceles — entrega en destino listo para comprador." },
        { code: "FAS (Free Alongside Ship)", app: "Solo marítimo", desc: "Vendedor entrega al costado del barco en puerto embarque; comprador asume desde ahí." },
        { code: "FOB (Free On Board)", app: "Solo marítimo", desc: "Vendedor entrega cuando mercancía supera la borda del buque; luego riesgo/pago lo asume comprador." },
        { code: "CFR (Cost and Freight)", app: "Solo marítimo", desc: "Vendedor paga transporte hasta puerto destino; riesgo se transfiere cuando la mercancía cruza la borda." },
        { code: "CIF (Cost, Insurance & Freight)", app: "Solo marítimo", desc: "Igual que CFR, pero el vendedor contrata seguro (nivel mínimo por defecto)." },
        { code: "DEL (Delivery to...)", app: "Genérico", desc: "No es oficial en las reglas de ICC Incoterms" }
    ]
};

// --- DATA: CHART DE RESPONSABILIDADES ---
// S = Seller, B = Buyer, N = Negotiable
const CHART_ROWS = [
    { label: "Export Packaging", values: ["S", "S", "S", "S", "S", "S", "S", "S", "S", "S", "S"] },
    { label: "Loading Charges", values: ["B", "S", "S", "S", "S", "S", "S", "S", "S", "S", "S"] },
    { label: "Delivery to Port/Place", values: ["B", "S", "S", "S", "S", "S", "S", "S", "S", "S", "S"] },
    { label: "Export Duty/Taxes", values: ["B", "S", "S", "S", "S", "S", "S", "S", "S", "S", "S"] },
    { label: "Origin Terminal Charges", values: ["B", "B", "S", "S", "S", "S", "S", "S", "S", "S", "S"] },
    { label: "Loading on Carriage", values: ["B", "B", "B", "S", "S", "S", "S", "S", "S", "S", "S"] },
    { label: "Carriage Charges", values: ["B", "B", "B", "B", "S", "S", "S", "S", "S", "S", "S"] },
    { label: "Insurance", values: ["N", "N", "N", "N", "N", "S*", "N", "S**", "N", "N", "N"] },
    { label: "Dest. Terminal Charges", values: ["B", "B", "B", "B", "B", "B", "S", "S", "S", "S", "S"] },
    { label: "Delivery to Destination", values: ["B", "B", "B", "B", "B", "B", "B", "B", "S", "S", "S"] },
    { label: "Unloading at Dest.", values: ["B", "B", "B", "B", "B", "B", "B", "B", "B", "S", "S"] },
    { label: "Import Duty/Taxes", values: ["B", "B", "B", "B", "B", "B", "B", "B", "B", "B", "S"] },
];

const TERMS = ["EXW", "FCA", "FAS", "FOB", "CFR", "CIF", "CPT", "CIP", "DAP", "DPU", "DDP"];

export default function IncotermsReference({ show, onClose }: Props) {
    const [activeTab, setActiveTab] = useState<'EN' | 'ES' | 'CHART'>('EN');

    useEffect(() => {
        if (show) {
            document.body.style.overflow = 'hidden';
        } else {
            document.body.style.overflow = 'unset';
        }
        return () => { document.body.style.overflow = 'unset'; };
    }, [show]);

    if (!show) return null;

    // Estilos Inline (Reutilizando paleta de colores del legacy)
    const modalStyle: React.CSSProperties = {
        display: 'block',
        backgroundColor: 'rgba(0,0,0,0.5)',
        position: 'fixed',
        top: 0, left: 0, right: 0, bottom: 0,
        zIndex: 1060,
        overflowX: 'hidden',
        overflowY: 'auto'
    };

    const headerStyle = { backgroundColor: '#009ff5', color: 'white' };
    const chartHeaderStyle = { backgroundColor: '#154360', color: 'white', fontSize: '12px' };
    const chartGroupStyle = { backgroundColor: '#d4edda', fontSize: '11px', textAlign: 'center' as const, border: '1px solid #ccc' };
    const cellStyle = { border: '1px solid #ccc', padding: '4px', textAlign: 'center' as const, fontSize: '11px' };

    // Helpers para el Chart
    const getCellColor = (val: string) => {
        if (val.startsWith("S")) return "#f2f3f4"; // Gris (Seller)
        if (val.startsWith("B")) return "#a9cce3"; // Azul (Buyer)
        return "#ffffff"; // Blanco (Negociable)
    };

    const getCellText = (val: string) => {
        let txt = "";
        if (val.startsWith("S")) txt = "Seller";
        else if (val.startsWith("B")) txt = "Buyer";
        else if (val.startsWith("N")) txt = "Negotiable";
        
        if (val.includes("*")) txt += val.substring(1); // Agrega asteriscos
        return txt;
    };

    return (
        <div className="modal fade show" style={modalStyle} tabIndex={-1} role="dialog" onClick={onClose}>
            <div className="modal-dialog modal-lg" role="document" style={{ maxWidth: '950px' }} onClick={(e) => e.stopPropagation()}>
                <div className="modal-content shadow-lg" style={{ border: 'none', borderRadius: '12px' }}>
                    
                    {/* Header */}
                    <div className="modal-header" style={{ ...headerStyle, borderTopLeftRadius: '12px', borderTopRightRadius: '12px' }}>
                        <h5 className="modal-title font-weight-bold">
                            <i className="fa fa-truck-loading mr-2"></i> Incoterms Rules
                        </h5>
                        <button type="button" className="close" onClick={onClose} style={{ color: 'white', opacity: 1 }}>
                            <span>&times;</span>
                        </button>
                    </div>

                    <div className="modal-body p-4">
                        {/* Tabs */}
                        <ul className="nav nav-tabs mb-3">
                            <li className="nav-item">
                                <a className={`nav-link ${activeTab === 'EN' ? 'active font-weight-bold' : ''}`} 
                                   onClick={() => setActiveTab('EN')} style={{ cursor: 'pointer', color: activeTab === 'EN' ? '#009ff5' : '#6c757d' }}>
                                   English
                                </a>
                            </li>
                            <li className="nav-item">
                                <a className={`nav-link ${activeTab === 'ES' ? 'active font-weight-bold' : ''}`} 
                                   onClick={() => setActiveTab('ES')} style={{ cursor: 'pointer', color: activeTab === 'ES' ? '#009ff5' : '#6c757d' }}>
                                   Español
                                </a>
                            </li>
                            <li className="nav-item">
                                <a className={`nav-link ${activeTab === 'CHART' ? 'active font-weight-bold' : ''}`} 
                                   onClick={() => setActiveTab('CHART')} style={{ cursor: 'pointer', color: activeTab === 'CHART' ? '#009ff5' : '#6c757d' }}>
                                   Responsibility Chart
                                </a>
                            </li>
                        </ul>

                        {/* Content */}
                        <div className="tab-content">
                            
                            {/* TABLA DE DEFINICIONES (EN/ES) */}
                            {(activeTab === 'EN' || activeTab === 'ES') && (
                                <div className="table-responsive fade show active">
                                    <table className="table table-sm table-bordered table-hover" style={{ border: '1px solid #009ff5', fontSize: '0.85rem' }}>
                                        <thead style={{ backgroundColor: '#009ff5', color: 'white', textAlign: 'center' }}>
                                            <tr>
                                                <th style={{ width: '15%', borderRight: '1px solid white' }}>Incoterm</th>
                                                <th style={{ width: '25%', borderRight: '1px solid white' }}>Applicable to</th>
                                                <th style={{ width: '60%' }}>Description</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {DEFINITIONS[activeTab].map((row, idx) => (
                                                <tr key={idx}>
                                                    <td style={{ backgroundColor: '#e6f7ff', fontWeight: 'bold' }}>{row.code}</td>
                                                    <td style={{ verticalAlign: 'middle' }}>{row.app}</td>
                                                    <td style={{ verticalAlign: 'middle' }}>{row.desc}</td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                </div>
                            )}

                            {/* CHART DE RESPONSABILIDADES */}
                            {activeTab === 'CHART' && (
                                <div className="table-responsive fade show active">
                                    <table style={{ width: '100%', borderCollapse: 'collapse', fontFamily: 'Arial, sans-serif' }}>
                                        <thead>
                                            <tr>
                                                <th style={{ ...cellStyle, ...chartGroupStyle, width: '15%' }}>Groups</th>
                                                <th colSpan={2} style={{ ...cellStyle, ...chartGroupStyle }}>Any Mode</th>
                                                <th colSpan={4} style={{ ...cellStyle, ...chartGroupStyle }}>Sea / Inland</th>
                                                <th colSpan={5} style={{ ...cellStyle, ...chartGroupStyle }}>Any Mode</th>
                                            </tr>
                                            <tr>
                                                <th style={{ ...cellStyle, ...chartHeaderStyle, fontSize: '14px' }}>Incoterm</th>
                                                {TERMS.map(t => <th key={t} style={{ ...cellStyle, ...chartHeaderStyle }}>{t}</th>)}
                                            </tr>
                                            <tr>
                                                <th style={{ ...cellStyle, backgroundColor: '#e74c3c', color: 'white', fontWeight: 'bold' }}>Transfer Risk</th>
                                                <td style={{...cellStyle, backgroundColor: '#e74c3c', color: 'white'}}>At Buyer</td>
                                                <td style={{...cellStyle, backgroundColor: '#e74c3c', color: 'white'}}>Buyer Trans.</td>
                                                <td style={{...cellStyle, backgroundColor: '#e74c3c', color: 'white'}}>Alongside</td>
                                                <td style={{...cellStyle, backgroundColor: '#e74c3c', color: 'white'}}>On Board</td>
                                                <td style={{...cellStyle, backgroundColor: '#e74c3c', color: 'white'}}>On Board</td>
                                                <td style={{...cellStyle, backgroundColor: '#e74c3c', color: 'white'}}>On Board</td>
                                                <td style={{...cellStyle, backgroundColor: '#e74c3c', color: 'white'}}>At Carrier</td>
                                                <td style={{...cellStyle, backgroundColor: '#e74c3c', color: 'white'}}>At Carrier</td>
                                                <td style={{...cellStyle, backgroundColor: '#e74c3c', color: 'white'}}>Named Place</td>
                                                <td style={{...cellStyle, backgroundColor: '#e74c3c', color: 'white'}}>Unloaded</td>
                                                <td style={{...cellStyle, backgroundColor: '#e74c3c', color: 'white'}}>Named Place</td>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {CHART_ROWS.map((row, idx) => (
                                                <tr key={idx}>
                                                    <td style={{ ...cellStyle, ...chartHeaderStyle, textAlign: 'left', paddingLeft: '10px' }}>
                                                        {row.label}
                                                    </td>
                                                    {row.values.map((val, vIdx) => (
                                                        <td key={vIdx} style={{ ...cellStyle, backgroundColor: getCellColor(val) }}>
                                                            {getCellText(val)}
                                                        </td>
                                                    ))}
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                    <div style={{ marginTop: '10px', fontSize: '11px', color: '#666' }}>
                                        <strong>* CIF:</strong> Requires minimum insurance coverage (Clause C). <br/>
                                        <strong>** CIP:</strong> Requires "All Risk" insurance coverage (Clause A).
                                    </div>
                                </div>
                            )}

                        </div>
                    </div>

                    <div className="modal-footer bg-light" style={{ borderBottomLeftRadius: '12px', borderBottomRightRadius: '12px' }}>
                        <button type="button" className="btn btn-secondary" onClick={onClose}>Close</button>
                    </div>
                </div>
            </div>
        </div>
    );
}