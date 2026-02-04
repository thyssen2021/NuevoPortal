import { useMemo, useState } from 'react';
import {
    MaterialReactTable,
    useMaterialReactTable,
    type MRT_ColumnDef
} from 'material-react-table';
import { Box, IconButton, Tooltip } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import type { Material } from '../types';
import { materialFields } from '../config/material-fields';
import { toast } from 'react-toastify';
import ConfirmationModal from './ConfirmationModal';

interface Props {
    materials: Material[];
    onEdit: (material: Material) => void;
    onDeleted: (id: number) => void;
}

export default function MaterialTable({ materials, onEdit, onDeleted }: Props) {

    // 1. ESTADO DEL MODAL DE BORRADO (Igual que antes)
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
    const [materialToDelete, setMaterialToDelete] = useState<Material | null>(null);
    const [isDeleting, setIsDeleting] = useState(false);

    // --- LÓGICA DE BORRADO (Intacta) ---
    const promptDelete = (material: Material) => {
        setMaterialToDelete(material);
        setIsDeleteModalOpen(true);
    };

    const closeDeleteModal = () => {
        setIsDeleteModalOpen(false);
        setMaterialToDelete(null);
    };

    const handleConfirmDelete = async () => {
        if (!materialToDelete) return;
        setIsDeleting(true);
        const deleteUrl = (window as any).reactInitialData?.urls?.deleteUrl;

        if (!deleteUrl) {
            toast.error("Configuration Error: Delete URL not found.");
            setIsDeleting(false);
            return;
        }

        try {
            const response = await fetch(deleteUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id: materialToDelete.ID_Material })
            });
            const result = await response.json();

            if (result.success) {
                toast.success("Material deleted successfully");
                closeDeleteModal();
                onDeleted(materialToDelete.ID_Material);
            } else {
                toast.error("Error: " + result.message);
                setIsDeleting(false);
            }
        } catch (error) {
            console.error(error);
            toast.error("Network error while deleting.");
            setIsDeleting(false);
        }
    };

    // 2. CONSTRUCCIÓN DINÁMICA DE COLUMNAS
    // Mapeamos tu 'materialFields' a la estructura que usa MRT
    const columns = useMemo<MRT_ColumnDef<Material>[]>(() => {
        return materialFields
            .filter(field => field.showInTable) // Solo las que marcaste para mostrar
            .map(field => ({
                accessorKey: field.name, // El nombre de la propiedad en el objeto Material
                header: field.label || field.name,
                size: field.name === 'Part_Number' ? 150 : 120, // Ajuste de anchos opcional

                // FORMATO DE CELDAS PERSONALIZADO
                Cell: ({ cell }) => {
                    let value = cell.getValue<any>();

                    // A. Formato de Fechas (YYYY-MM)
                    if (['SOP_SP', 'EOP_SP', 'Real_SOP', 'Real_EOP'].includes(field.name)) {
                        if (typeof value === 'string' && value.length > 7) return value.substring(0, 7);
                    }

                    // B. Formato Numérico
                    if (field.type === 'number' && typeof value === 'number') {
                        return value.toLocaleString();
                    }

                    // C. Formato Booleano (Yes/No con estilo)
                    if (typeof value === 'boolean') {
                        return value
                            ? <span style={{ color: 'green', fontWeight: 'bold' }}>Yes</span>
                            : <span style={{ color: '#aaa' }}>No</span>;
                    }

                    // D. Valor por defecto si es nulo
                    if (value === null || value === undefined || value === '') {
                        return <span style={{ opacity: 0.5 }}>—</span>;
                    }

                    return value;
                }
            }));
    }, []);


    // 3. CONFIGURACIÓN DE LA TABLA (ESTILO PRO & FIX VISUAL)
    // 3. CONFIGURACIÓN DE LA TABLA (SIN CORTE DE TEXTO)
    const table = useMaterialReactTable({
        columns,
        data: materials || [],

        // --- FUNCIONALIDADES ---
        enableRowActions: true,
        positionActionsColumn: 'first',
        enableStickyHeader: true,
        enableColumnResizing: true, // Permite ajustar manualmente si se quiere

        // 👇 CRÍTICO: 'semantic' permite que la tabla HTML se comporte natural y se ensanche
        layoutMode: 'semantic',

        initialState: {
            pagination: { pageSize: 10, pageIndex: 0 },
            density: 'compact',
        },

        // --- CONFIGURACIÓN POR DEFECTO DE COLUMNAS ---
        defaultColumn: {
            minSize: 40,
            maxSize: 1000,
            size: 180,
        },

        // --- ESTILOS VISUALES ---

        // 1. Contenedor Principal
        muiTablePaperProps: {
            elevation: 0,
            sx: {
                borderRadius: '12px',
                border: '1px solid #e0e0e0',
                overflow: 'hidden',
            }
        },

        // 2. Contenedor de Scrollbar
        muiTableContainerProps: {
            sx: {
                maxHeight: '600px',
                // Estilos del scrollbar
                '&::-webkit-scrollbar': { width: '8px', height: '8px' },
                '&::-webkit-scrollbar-track': { backgroundColor: '#f1f1f1' },
                '&::-webkit-scrollbar-thumb': { backgroundColor: '#c1c1c1', borderRadius: '4px' },
                '&::-webkit-scrollbar-thumb:hover': { backgroundColor: '#a0a0a0' },
            }
        },

        // 3. CABECERA (HEADER) - COLOR SÓLIDO
        muiTableHeadCellProps: {
            sx: {
                backgroundColor: '#009ff5', // 👈 COLOR SÓLIDO (Sin degradado)
                color: 'white',
                fontWeight: 'bold',
                fontSize: '0.85rem',
                textTransform: 'uppercase',
                letterSpacing: '0.5px',
                padding: '12px',
                verticalAlign: 'bottom',

                // 👇 ESTO EVITA QUE LOS TÍTULOS LARGOS SE CORTEN
                whiteSpace: 'nowrap',

                // Filtros e Inputs en blanco
                '& .MuiInputBase-root': {
                    color: 'white',
                    '&:before': { borderBottomColor: 'rgba(255,255,255,0.4)' },
                    '&:after': { borderBottomColor: 'white' },
                    '&:hover:not(.Mui-disabled):before': { borderBottomColor: 'rgba(255,255,255,0.7)' },
                },
                '& .MuiInputBase-input::placeholder': {
                    color: 'rgba(255,255,255,0.7)',
                    opacity: 1,
                },
                '& .MuiSvgIcon-root': {
                    color: 'rgba(255,255,255,0.7)',
                },

                // Iconos de ordenamiento en blanco
                '& .MuiTableSortLabel-icon': {
                    color: 'white !important',
                    opacity: 0.5
                },
                '& .MuiTableSortLabel-root:hover .MuiTableSortLabel-icon': {
                    opacity: 1,
                    color: 'white !important'
                },
                '& .MuiTableSortLabel-root.Mui-active .MuiTableSortLabel-icon': {
                    opacity: 1,
                    color: 'white !important'
                },
                '& .MuiTableSortLabel-root': {
                    color: 'white !important'
                },
                '& .MuiTableHeadCell-Content-Actions .MuiIconButton-root': {
                    color: 'white !important',
                    opacity: 0.8
                }
            },
        },

        // 4. Filas del Cuerpo
        muiTableBodyProps: {
            sx: {
                '& tr:nth-of-type(odd)': { backgroundColor: '#ffffff' },
                '& tr:nth-of-type(even)': { backgroundColor: '#f8fbff' },
            }
        },

        // 5. Celdas del Cuerpo - AJUSTE AL CONTENIDO
        muiTableBodyCellProps: {
            sx: {
                // 👇 ESTA ES LA CLAVE PARA QUE NO SE CORTE
                whiteSpace: 'nowrap',  // Obliga al texto a ir en una línea
                width: 'auto',         // Permite que la celda crezca
                minWidth: 'min-content',

                borderRight: '1px solid #f0f0f0',
                fontSize: '0.9rem',
                padding: '8px 12px', // Un poco más de aire
            }
        },

        muiTableBodyRowProps: () => ({
            sx: {
                '&:hover td': { backgroundColor: '#eef9ff !important' },
                transition: 'background-color 0.2s',
            }
        }),

        // Acciones
        renderRowActions: ({ row }) => (
            <Box sx={{ display: 'flex', gap: '0px' }}>
                <Tooltip title="Edit">
                    <IconButton color="warning" onClick={() => onEdit(row.original)} size="small">
                        <EditIcon fontSize="small" />
                    </IconButton>
                </Tooltip>
                <Tooltip title="Delete">
                    <IconButton color="error" onClick={() => promptDelete(row.original)} size="small">
                        <DeleteIcon fontSize="small" />
                    </IconButton>
                </Tooltip>
            </Box>
        ),
    });

    return (
        <>
            <div style={{ borderRadius: '8px', overflow: 'hidden', border: '1px solid #e0e0e0' }}>
                {/* RENDERIZADO DE LA TABLA MÁGICA */}
                <MaterialReactTable table={table} />
            </div>

            {/* Modal de Borrado (Intacto) */}
            <ConfirmationModal
                isOpen={isDeleteModalOpen}
                title="Delete Material"
                message={`Are you sure you want to delete the part "${materialToDelete?.Part_Number || 'Unknown'}"?`}
                onConfirm={handleConfirmDelete}
                onCancel={closeDeleteModal}
                isLoading={isDeleting}
            />
        </>
    );
}