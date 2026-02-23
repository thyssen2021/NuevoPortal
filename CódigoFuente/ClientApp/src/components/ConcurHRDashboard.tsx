import { useEffect, useState, useMemo } from 'react';
import { MaterialReactTable, useMaterialReactTable, type MRT_ColumnDef } from 'material-react-table';
import { MRT_Localization_ES } from 'material-react-table/locales/es';

interface Solicitud {
    IdSolicitud: number;
    GlobalEmployeeID: string;
    FirstName: string;
    LastName: string;
    FechaSolicitud: string;
    Estatus: string;
    CostCenterValue: string;
    ApproverEmployeeID: string;
    IsApprover: string;
}

export default function ConcurHRDashboard() {
    const [solicitudes, setSolicitudes] = useState<Solicitud[]>([]);
    const [loading, setLoading] = useState(true);

    const companyColor = '#009ff5';

    useEffect(() => {
        const loadRequests = async () => {
            try {
                const response = await fetch('/Concur/GetMyRequests');
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

        loadRequests();
    }, []);

    const columns = useMemo<MRT_ColumnDef<Solicitud>[]>(
        () => [
            {
                accessorKey: 'IdSolicitud',
                header: '# Folio',
                Cell: ({ cell }) => (
                    <span style={{ fontWeight: 'bold', color: '#555' }}>
                        {cell.getValue<number>()}
                    </span>
                ),
            },
            {
                accessorKey: 'GlobalEmployeeID',
                header: '8ID Empleado',
            },
            {
                accessorFn: (row) => `${row.FirstName} ${row.LastName}`,
                id: 'NombreCompleto',
                header: 'Nombre del Colaborador',
            },
            {
                accessorKey: 'CostCenterValue',
                header: 'C. Costos',
                Cell: ({ cell }) => (
                    <span style={{ backgroundColor: '#f8f9fa', padding: '4px 8px', borderRadius: '4px', border: '1px solid #dee2e6', fontSize: '13px' }}>
                        {cell.getValue<string>()}
                    </span>
                ),
            },
            {
                accessorKey: 'ApproverEmployeeID',
                header: 'Jefe de Autorización (8ID)',
                Cell: ({ cell }) => (
                    <span><i className="fa fa-user-circle-o mr-1" style={{ color: '#6c757d' }}></i> {cell.getValue<string>()}</span>
                ),
            },
            {
                accessorKey: 'IsApprover',
                header: '¿Es aurorizador?',
                Cell: ({ cell }) => {
                    const isApprover = cell.getValue<string>() === 'Y';
                    return (
                        <span style={{ color: isApprover ? companyColor : '#6c757d', fontWeight: isApprover ? 'bold' : 'normal' }}>
                            {isApprover ? 'Sí' : 'No'}
                        </span>
                    );
                },
            },
            {
                accessorKey: 'FechaSolicitud',
                header: 'Fecha de Solicitud',
                Cell: ({ cell }) => {
                    const rawDate = cell.getValue<string>();
                    const parsedDate = new Date(parseInt(rawDate.replace(/\/Date\((.*?)\)\//gi, "$1")));
                    return parsedDate.toLocaleDateString('es-MX', { 
                        year: 'numeric', month: 'short', day: '2-digit', 
                        hour: '2-digit', minute: '2-digit' 
                    });
                },
            },
            {
                accessorKey: 'Estatus',
                header: 'Estado',
                Cell: ({ cell }) => {
                    const status = cell.getValue<string>();
                    
                    // Definir variables de estilo por defecto (Pendiente)
                    let bg = '#fff3cd';
                    let color = '#856404';
                    let border = '#ffeeba';
                    let icon = 'fa-clock-o';

                    // Asignar colores según el nuevo flujo
                    if (status === 'Completado') {
                        bg = '#dcfce7';
                        color = '#155724';
                        border = '#c3e6cb';
                        icon = 'fa-check-circle';
                    } else if (status === 'En Proceso') {
                        bg = '#cce5ff';
                        color = '#004085';
                        border = '#b8daff';
                        icon = 'fa-cogs';
                    }

                    return (
                        <span style={{ 
                            backgroundColor: bg, 
                            color: color, 
                            padding: '6px 12px', 
                            borderRadius: '20px', 
                            fontSize: '12px', 
                            fontWeight: 'bold',
                            border: `1px solid ${border}`,
                            display: 'inline-flex',
                            alignItems: 'center',
                            gap: '6px',
                            whiteSpace: 'nowrap'
                        }}>
                            <i className={`fa ${icon}`}></i>
                            {status}
                        </span>
                    );
                },
            },
        ],
        []
    );

    const table = useMaterialReactTable({
        columns,
        data: solicitudes,
        state: { isLoading: loading },
        localization: MRT_Localization_ES,
        
        // 1. Desactivar el resize forzado y usar el layout semántico (HTML Table clásico)
        enableColumnResizing: false,
        layoutMode: 'semantic', 
        
        enableGlobalFilter: true,
        enablePagination: true,
        paginationDisplayMode: 'pages',
        initialState: {
            showGlobalFilter: true,
            sorting: [{ id: 'IdSolicitud', desc: true }],
        },
        muiTablePaperProps: {
            elevation: 0,
            style: {
                borderRadius: '12px',
                border: '1px solid #e0e0e0',
                boxShadow: '0 4px 20px rgba(0,0,0,0.05)',
                padding: '10px'
            }
        },
        muiTableHeadCellProps: {
            style: {
                backgroundColor: '#f0f9ff',
                color: '#003366',
                fontWeight: 'bold',
                fontSize: '13px',
                textTransform: 'uppercase',
                // 2. Permitir que el texto baje a la siguiente línea en lugar de cortarse
                whiteSpace: 'normal',
                wordWrap: 'break-word',
                verticalAlign: 'bottom' // Alinea el texto hacia abajo si tienen diferentes alturas
            }
        },
        // Opcional: Aplicar el mismo salto de línea a las celdas de datos
        muiTableBodyCellProps: {
            style: {
                whiteSpace: 'normal',
                wordWrap: 'break-word'
            }
        },
        renderTopToolbarCustomActions: () => (
            <a 
                href="/Concur/SolicitudRH" 
                style={{ 
                    backgroundColor: companyColor, 
                    color: 'white', 
                    padding: '8px 16px', 
                    borderRadius: '6px', 
                    textDecoration: 'none', 
                    fontWeight: 'bold',
                    display: 'flex',
                    alignItems: 'center',
                    gap: '8px',
                    transition: 'background-color 0.2s',
                    boxShadow: '0 2px 5px rgba(0,159,245,0.3)'
                }}
                onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#0086d1'}
                onMouseOut={(e) => e.currentTarget.style.backgroundColor = companyColor}
            >
                <i className="fa fa-plus"></i> NUEVA ALTA CONCUR
            </a>
        ),
    });

    return (
        // Se cambió maxWidth: '1200px' a width: '100%' para que ocupe todo el espacio de la vista MVC
        <div style={{ width: '100%', fontFamily: 'sans-serif' }}>
            <div style={{ marginBottom: '20px' }}>
                <h3 style={{ color: '#003366', margin: '0 0 5px 0', fontSize: '24px' }}>
                    <i className="fa fa-list-alt mr-2" style={{ color: companyColor }}></i> 
                    Mis Solicitudes de Alta
                </h3>
                <p style={{ color: '#6c757d', margin: 0 }}>
                    Historial y estado de seguimiento de cuentas Concur solicitadas al equipo de IT.
                </p>
            </div>

            <MaterialReactTable table={table} />
        </div>
    );
}