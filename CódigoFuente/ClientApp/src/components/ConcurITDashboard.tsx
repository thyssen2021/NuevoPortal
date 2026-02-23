import { useEffect, useState, useMemo } from 'react';
import { MaterialReactTable, useMaterialReactTable, type MRT_ColumnDef } from 'material-react-table';
import { MRT_Localization_ES } from 'material-react-table/locales/es';
import ConfirmationModal from './ConfirmationModal';

interface Solicitud {
    IdSolicitud: number;
    GlobalEmployeeID: string;
    FirstName: string;
    LastName: string;
    FechaSolicitud: string;
    Estatus: string;
}

export default function ConcurITDashboard() {
    const [solicitudes, setSolicitudes] = useState<Solicitud[]>([]);
    const [loading, setLoading] = useState(true);
    
    // Estados para el Modal de Confirmación de Estatus
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedRequest, setSelectedRequest] = useState<number | null>(null);
    const [newStatus, setNewStatus] = useState<string>('');
    const [actionLoading, setActionLoading] = useState(false);

    const loadRequests = async () => {
        setLoading(true);
        try {
            const response = await fetch('/Concur/GetAllRequests');
            const result = await response.json();
            if (result.success) {
                setSolicitudes(result.data);
            }
        } catch (error) {
            console.error("Error cargando solicitudes", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadRequests();
    }, []);

    // Descargar TXT sin afectar estatus
    const handleDownload = async (id: number) => {
        try {
            const response = await fetch(`/Concur/DownloadConcurFile?requestId=${id}`, { method: 'POST' });
            if (response.ok) {
                const blob = await response.blob();
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                
                const contentDisposition = response.headers.get('Content-Disposition');
                let fileName = `Concur_Req_${id}.txt`;
                if (contentDisposition && contentDisposition.includes('filename=')) {
                    fileName = contentDisposition.split('filename=')[1].replace(/['"]/g, '');
                }

                a.download = fileName;
                document.body.appendChild(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            } else {
                alert("Error al descargar el archivo de Concur.");
            }
        } catch (error) {
            console.error("Error de descarga", error);
        }
    };

    // Prepara el cambio de estado abriendo el modal
    const handleStatusChangeClick = (id: number, status: string) => {
        setSelectedRequest(id);
        setNewStatus(status);
        setIsModalOpen(true);
    };

    // Ejecuta el cambio de estado en BD
    const confirmStatusChange = async () => {
        if (!selectedRequest || !newStatus) return;
        
        setActionLoading(true);
        try {
            const response = await fetch(`/Concur/UpdateStatus?id=${selectedRequest}&nuevoEstatus=${newStatus}`, { method: 'POST' });
            const result = await response.json();
            if (result.success) {
                loadRequests(); // Recargar tabla
            } else {
                alert(result.message);
            }
        } catch (error) {
            alert('Error de conexión al actualizar estatus.');
        } finally {
            setActionLoading(false);
            setIsModalOpen(false);
        }
    };

    // Configuración de Colores por Estatus
    const getStatusStyles = (status: string) => {
        switch (status) {
            case 'Completado': return { bg: '#dcfce7', color: '#155724', border: '#c3e6cb', icon: 'fa-check-circle' };
            case 'En Proceso': return { bg: '#cce5ff', color: '#004085', border: '#b8daff', icon: 'fa-cogs' };
            default: return { bg: '#fff3cd', color: '#856404', border: '#ffeeba', icon: 'fa-clock-o' }; // Pendiente
        }
    };

    const columns = useMemo<MRT_ColumnDef<Solicitud>[]>(
        () => [
            {
                accessorKey: 'IdSolicitud',
                header: 'Folio',
                Cell: ({ cell }) => <span style={{ fontWeight: 'bold' }}>{cell.getValue<number>()}</span>,
            },
            {
                accessorKey: 'GlobalEmployeeID',
                header: '8ID',
            },
            {
                accessorFn: (row) => `${row.FirstName} ${row.LastName}`,
                id: 'NombreCompleto',
                header: 'Colaborador',
            },
            {
                accessorKey: 'FechaSolicitud',
                header: 'Fecha Solicitud',
                Cell: ({ cell }) => {
                    const rawDate = cell.getValue<string>();
                    const parsedDate = new Date(parseInt(rawDate.replace(/\/Date\((.*?)\)\//gi, "$1")));
                    return parsedDate.toLocaleDateString('es-MX', { year: 'numeric', month: 'short', day: '2-digit', hour: '2-digit', minute: '2-digit' });
                },
            },
            {
                accessorKey: 'Estatus',
                header: 'Gestión de Estado',
                Cell: ({ cell, row }) => {
                    const currentStatus = cell.getValue<string>();
                    const styles = getStatusStyles(currentStatus);
                    
                    return (
                        <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                            <span style={{ 
                                backgroundColor: styles.bg, color: styles.color, border: `1px solid ${styles.border}`,
                                padding: '4px 8px', borderRadius: '12px', fontSize: '12px', fontWeight: 'bold', display: 'flex', alignItems: 'center', gap: '5px'
                            }}>
                                <i className={`fa ${styles.icon}`}></i> {currentStatus}
                            </span>
                            
                            {/* Selector para que IT cambie el estado */}
                            <select 
                                value="" 
                                onChange={(e) => handleStatusChangeClick(row.original.IdSolicitud, e.target.value)}
                                style={{ padding: '4px', borderRadius: '4px', border: '1px solid #ced4da', fontSize: '11px', cursor: 'pointer' }}
                            >
                                <option value="" disabled>Actualizar...</option>
                                {currentStatus !== 'Pendiente' && <option value="Pendiente">Mover a Pendiente</option>}
                                {currentStatus !== 'En Proceso' && <option value="En Proceso">Mover a En Proceso</option>}
                                {currentStatus !== 'Completado' && <option value="Completado">Marcar Completado</option>}
                            </select>
                        </div>
                    );
                },
            },
            {
                id: 'acciones',
                header: 'Archivo (Layout)',
                Cell: ({ row }) => (
                    <button 
                        onClick={() => handleDownload(row.original.IdSolicitud)}
                        style={{ backgroundColor: '#17a2b8', color: 'white', border: 'none', padding: '6px 12px', cursor: 'pointer', borderRadius: '4px', fontSize: '12px', fontWeight: 'bold', display: 'flex', alignItems: 'center', gap: '5px' }}
                    >
                        <i className="fa fa-download"></i> TXT
                    </button>
                ),
            },
        ],
        []
    );

    const table = useMaterialReactTable({
        columns,
        data: solicitudes,
        state: { isLoading: loading },
        localization: MRT_Localization_ES,
        enableColumnResizing: false,
        layoutMode: 'semantic',
        enableGlobalFilter: true,
        enablePagination: true,
        initialState: {
            showGlobalFilter: true,
            sorting: [{ id: 'IdSolicitud', desc: true }],
        },
        muiTablePaperProps: { elevation: 0, style: { borderRadius: '12px', border: '1px solid #e0e0e0', padding: '10px' } },
        muiTableHeadCellProps: { style: { backgroundColor: '#f4f6f9', color: '#333', fontWeight: 'bold', whiteSpace: 'normal', textTransform: 'uppercase', fontSize: '12px' } },
        muiTableBodyCellProps: { style: { whiteSpace: 'normal', wordWrap: 'break-word' } }
    });

    return (
        <div style={{ width: '100%', fontFamily: 'sans-serif' }}>
          <ConfirmationModal 
                isOpen={isModalOpen}
                title="Actualizar Estado de Solicitud"
                message={
                    `¿Confirmas cambiar el estado de la solicitud #${selectedRequest} a "${newStatus}"?` + 
                    (newStatus === 'Completado' ? ' Se enviará automáticamente un correo electrónico al usuario de Recursos Humanos para notificarle que el alta ha finalizado.' : '')
                }
                confirmText={`Sí, mover a ${newStatus}`}
                cancelText="Cancelar"
                variant={newStatus === 'Completado' ? 'success' : newStatus === 'En Proceso' ? 'info' : 'warning'}
                icon={newStatus === 'Completado' ? 'fa-check' : newStatus === 'En Proceso' ? 'fa-cogs' : 'fa-clock-o'}
                onConfirm={confirmStatusChange}
                onCancel={() => setIsModalOpen(false)}
                isLoading={actionLoading}
            />

            <div style={{ marginBottom: '20px' }}>
                <h3 style={{ color: '#333', margin: '0 0 5px 0', fontSize: '24px' }}>
                    <i className="fa fa-cogs mr-2" style={{ color: '#009ff5' }}></i> 
                    Administración IT - Cuentas Concur
                </h3>
            </div>

            <MaterialReactTable table={table} />
        </div>
    );
}