import { useState, useEffect } from 'react';
import SearchableSelect from './SearchableSelect';
import ConfirmationModal from './ConfirmationModal';

export default function ConcurRequestForm() {
    const [loading, setLoading] = useState(false);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [message, setMessage] = useState({ text: '', type: '' });
    const [employeeOptions, setEmployeeOptions] = useState<any[]>([]);
    const [isLoadingOptions, setIsLoadingOptions] = useState(true);
    const [errors, setErrors] = useState<Record<string, string>>({});
    
    const [formData, setFormData] = useState({
        GlobalEmployeeID: '',
        FirstName: '',
        LastName: '',
        LoginId: '',
        EmailAddress: '',
        CostCenterValue: '',
        ApproverEmployeeID: '',
        IsApprover: 'N'
    });

    const companyColor = '#009ff5';

    useEffect(() => {
        const fetchEmployees = async () => {
            try {
                const response = await fetch('/Concur/GetEmployeesList');
                const result = await response.json();
                if (result.success) setEmployeeOptions(result.data);
            } catch (error) {
                setMessage({ text: 'Error al cargar empleados.', type: 'error' });
            } finally {
                setIsLoadingOptions(false);
            }
        };
        fetchEmployees();
    }, []);

    const validateForm = () => {
        const newErrors: Record<string, string> = {};
        if (!formData.GlobalEmployeeID) newErrors.GlobalEmployeeID = 'err';
        if (!formData.FirstName.trim()) newErrors.FirstName = 'err';
        if (!formData.LastName.trim()) newErrors.LastName = 'err';
        if (!formData.EmailAddress.trim()) newErrors.EmailAddress = 'err';
        if (!formData.CostCenterValue.trim()) newErrors.CostCenterValue = 'err';
        if (!formData.ApproverEmployeeID) newErrors.ApproverEmployeeID = 'err';

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleEmployeeSelect = (selectedValue: string | null) => {
        // Limpiar errores visuales al seleccionar nuevo empleado
        setErrors({});
        setMessage({ text: '', type: '' });

        if (!selectedValue) {
            setFormData({ ...formData, GlobalEmployeeID: '', FirstName: '', LastName: '', LoginId: '', EmailAddress: '', ApproverEmployeeID: '' });
            return;
        }

        const emp = employeeOptions.find(e => e.Value === selectedValue);
        if (emp) {
            setFormData({
                ...formData,
                GlobalEmployeeID: emp.GlobalEmployeeID,
                FirstName: emp.FirstName,
                LastName: emp.LastName,
                LoginId: emp.LoginId,
                EmailAddress: emp.EmailAddress,
                ApproverEmployeeID: emp.ApproverEmployeeID || ''
            });
        }
    };

    const handlePreSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (validateForm()) {
            setIsModalOpen(true);
        } else {
            setMessage({ text: 'Por favor, complete los campos resaltados.', type: 'error' });
        }
    };

    const handleConfirmSubmit = async () => {
        setIsModalOpen(false);
        setLoading(true);
        try {
            const response = await fetch('/Concur/CreateRequest', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(formData)
            });
            const result = await response.json();
            if (result.success) {
                setMessage({ text: '¡Solicitud enviada con éxito! Redirigiendo a sus solicitudes...', type: 'success' });
                // Limpiar formulario y redirigir después de 1.5 segundos
                setFormData({ GlobalEmployeeID: '', FirstName: '', LastName: '', LoginId: '', EmailAddress: '', CostCenterValue: '', ApproverEmployeeID: '', IsApprover: 'N' });
                setErrors({});
                setTimeout(() => {
                    window.location.href = '/Concur/MisSolicitudes';
                }, 1500);
            } else {
                setMessage({ text: result.message, type: 'error' });
            }
        } catch (error) {
            setMessage({ text: 'Error en la comunicación con el servidor.', type: 'error' });
        } finally {
            setLoading(false);
        }
    };

    const labelStyle = (fieldName: string) => ({
        display: 'block',
        fontSize: '12px',
        fontWeight: '700',
        marginBottom: '5px',
        color: errors[fieldName] ? '#dc3545' : companyColor
    });

    const inputStyle = (fieldName: string, isReadOnly = false) => ({
        width: '100%',
        padding: '10px',
        border: `1.5px solid ${errors[fieldName] ? '#dc3545' : '#ced4da'}`,
        borderRadius: '6px',
        outline: 'none',
        backgroundColor: isReadOnly ? '#f8f9fa' : '#fff',
        cursor: isReadOnly ? 'not-allowed' : 'text'
    });

    return (
        <div style={{ maxWidth: '1000px', margin: '0 auto', fontFamily: 'sans-serif' }}>
            <ConfirmationModal 
                isOpen={isModalOpen}
                title="Confirmación de Solicitud de Alta"
                message={`¿Estás seguro de solicitar la creación de la cuenta Concur para el colaborador ${formData.FirstName} ${formData.LastName}? Este proceso notificará de forma automática al equipo de IT para su configuración en el sistema.`}
                confirmText="Sí, Enviar Solicitud"
                cancelText="Cancelar y Revisar"
                variant="info"
                icon="fa-user-plus"
                onConfirm={handleConfirmSubmit}
                onCancel={() => setIsModalOpen(false)}
                isLoading={loading}
            />

            <div style={{ backgroundColor: 'white', padding: '30px', borderRadius: '12px', boxShadow: '0 4px 20px rgba(0,0,0,0.08)' }}>
                
                {/* Selector de Empleado */}
                <div style={{ marginBottom: '25px', padding: '15px', border: `1px solid ${errors.GlobalEmployeeID ? '#dc3545' : companyColor}`, borderRadius: '8px', backgroundColor: '#f0f9ff' }}>
                    <label style={{ ...labelStyle('GlobalEmployeeID'), color: errors.GlobalEmployeeID ? '#dc3545' : '#003366' }}>
                        SELECCIONAR COLABORADOR *
                    </label>
                    <SearchableSelect 
                        value={formData.GlobalEmployeeID || null}
                        onChange={handleEmployeeSelect}
                        options={employeeOptions}
                        isLoading={isLoadingOptions}
                        error={errors.GlobalEmployeeID}
                        placeholder="Escribe 8ID, Nombre o Número de empleado..."
                    />
                </div>

                {message.text && (
                    <div style={{ padding: '10px', marginBottom: '20px', borderRadius: '6px', backgroundColor: message.type === 'success' ? '#dcfce7' : '#fee2e2', color: message.type === 'success' ? '#15803d' : '#991b1b', border: `1px solid ${message.type === 'success' ? '#16a34a' : '#dc2626'}` }}>
                        {message.text}
                    </div>
                )}

                <form onSubmit={handlePreSubmit}>
                    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, 1fr)', gap: '15px' }}>
                        
                        <div style={{ gridColumn: 'span 12' }}><h5 style={{ color: companyColor, margin: '5px 0' }}>Información Identificadora</h5></div>

                        {/* Campos Readonly movidos de la sección técnica */}
                        <div style={{ gridColumn: 'span 6' }}>
                            <label style={labelStyle('GlobalEmployeeID')}>Global Employee ID (8ID)</label>
                            <input type="text" value={formData.GlobalEmployeeID} readOnly style={inputStyle('GlobalEmployeeID', true)} />
                        </div>
                        <div style={{ gridColumn: 'span 6' }}>
                            <label style={labelStyle('LoginId')}>Login ID</label>
                            <input type="text" value={formData.LoginId} readOnly style={inputStyle('LoginId', true)} />
                        </div>

                        <div style={{ gridColumn: 'span 6' }}>
                            <label style={labelStyle('FirstName')}>Nombres *</label>
                            <input type="text" value={formData.FirstName} onChange={e => { setFormData({...formData, FirstName: e.target.value}); setErrors({...errors, FirstName: ''}); }} style={inputStyle('FirstName')} />
                        </div>
                        <div style={{ gridColumn: 'span 6' }}>
                            <label style={labelStyle('LastName')}>Apellidos *</label>
                            <input type="text" value={formData.LastName} onChange={e => { setFormData({...formData, LastName: e.target.value}); setErrors({...errors, LastName: ''}); }} style={inputStyle('LastName')} />
                        </div>
                        <div style={{ gridColumn: 'span 12' }}>
                            <label style={labelStyle('EmailAddress')}>Email Corporativo *</label>
                            <input type="email" value={formData.EmailAddress} onChange={e => { setFormData({...formData, EmailAddress: e.target.value}); setErrors({...errors, EmailAddress: ''}); }} style={inputStyle('EmailAddress')} />
                        </div>

                        <div style={{ gridColumn: 'span 12' }}><h5 style={{ color: companyColor, margin: '15px 0 5px 0' }}>Configuración de Aprobaciones</h5></div>

                        {/* Centro de Costos (3/12) y Aprobador (9/12) */}
                        <div style={{ gridColumn: 'span 3' }}>
                            <label style={labelStyle('CostCenterValue')}>C. Costos *</label>
                            <input type="text" placeholder="Ej: 3100" value={formData.CostCenterValue} onChange={e => { setFormData({...formData, CostCenterValue: e.target.value}); setErrors({...errors, CostCenterValue: ''}); }} style={inputStyle('CostCenterValue')} />
                        </div>
                        <div style={{ gridColumn: 'span 9' }}>
                            <label style={labelStyle('ApproverEmployeeID')}>Jefe Directo (Autorizador de Gastos) *</label>
                            <SearchableSelect 
                                value={formData.ApproverEmployeeID || null}
                                onChange={(val) => { setFormData({...formData, ApproverEmployeeID: val || ''}); setErrors({...errors, ApproverEmployeeID: ''}); }}
                                options={employeeOptions}
                                error={errors.ApproverEmployeeID}
                                placeholder="Seleccione quién autoriza los gastos..."
                            />
                        </div>

                        <div style={{ gridColumn: 'span 12' }}>
                            <label style={labelStyle('IsApprover')}>¿Es Autorizador? *</label>
                            <select value={formData.IsApprover} onChange={e => setFormData({...formData, IsApprover: e.target.value})} style={{ ...inputStyle('IsApprover'), height: '42px' }}>
                                <option value="N">No</option>
                                <option value="Y">Sí</option>
                            </select>
                        </div>

                        {/* Parámetros Técnicos Restantes */}
                        <div style={{ gridColumn: 'span 12', marginTop: '20px', padding: '15px', backgroundColor: '#f8f9fa', borderRadius: '8px', border: '1px solid #dee2e6' }}>
                            <p style={{ fontSize: '11px', fontWeight: 'bold', color: companyColor, marginBottom: '10px' }}>LOG DE DATOS FIJOS DEL SISTEMA</p>
                            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '8px', fontSize: '10px', color: '#6b7280' }}>
                                <span><strong>Trans. Type:</strong> 305</span>
                                <span><strong>Locale:</strong> es_MX</span>
                                <span><strong>Country:</strong> MEX</span>
                                <span><strong>Currency:</strong> MXN</span>
                                <span><strong>Ledger:</strong> TK_Symbolic</span>
                                <span><strong>Active:</strong> Y</span>
                                <span><strong>OU1-Country:</strong> MEX</span>
                                <span><strong>OU2-Business:</strong> MXN</span>
                                <span><strong>OU3-Global Co:</strong> 0000801495</span>
                                <span><strong>OU4-ERP Co:</strong> 5090</span>
                                <span><strong>OU5-Cost Type:</strong> CC</span>
                                <span><strong>Group:</strong> MEX0000801495</span>
                                <span><strong>Status:</strong> Pendiente</span>
                                <span><strong>End File:</strong> EOL</span>
                            </div>
                        </div>

                        <div style={{ gridColumn: 'span 12', marginTop: '15px' }}>
                            <button type="submit" disabled={loading} style={{ width: '100%', padding: '14px', backgroundColor: companyColor, color: 'white', border: 'none', fontWeight: 'bold', borderRadius: '8px', fontSize: '16px', cursor: 'pointer', transition: 'background-color 0.2s' }}>
                                {loading ? 'Enviando solicitud...' : 'CREAR SOLICITUD DE ALTA'}
                            </button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    );
}