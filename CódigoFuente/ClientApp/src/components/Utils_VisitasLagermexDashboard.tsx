import { useEffect, useMemo, useState } from 'react';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js';
import { Bar } from 'react-chartjs-2';
import { MaterialReactTable, type MRT_ColumnDef } from 'material-react-table';

// Registramos los módulos de Chart.js necesarios para una gráfica de barras
ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
);

// Interfaces basadas en la respuesta de tu controlador C#
interface RegistroTabla {
  Id: number;
  Fecha: string;
  Ubicacion: string;
  Navegador: string;
  Usuario: string;
  Estatus: string;
}

interface RegistroGrafica {
  Fecha: string;
  Cantidad: number;
}

// Funciones de ayuda para establecer las fechas por defecto en los inputs
const getTodayStr = () => new Date().toISOString().split('T')[0];
const get30DaysAgoStr = () => {
  const d = new Date();
  d.setDate(d.getDate() - 30);
  return d.toISOString().split('T')[0];
};

export default function Utils_RegistrosDashboard() {
  // Estados para almacenar los datos
  const [dataTabla, setDataTabla] = useState<RegistroTabla[]>([]);
  const [dataGrafica, setDataGrafica] = useState<RegistroGrafica[]>([]);
  const [loading, setLoading] = useState(true);
  
  // Estados para los filtros de fecha
  const [fechaInicio, setFechaInicio] = useState(get30DaysAgoStr());
  const [fechaFin, setFechaFin] = useState(getTodayStr());

  // Función principal para consultar la API con los parámetros de fecha
  const fetchRegistros = async (inicio: string, fin: string) => {
    setLoading(true);
    try {
      const response = await fetch(`/Utils/ObtenerRegistrosReales?fechaInicio=${inicio}&fechaFin=${fin}`);
      const result = await response.json();
      
      if (result.success) {
        setDataTabla(result.tabla);
        setDataGrafica(result.grafica);
      } else {
        console.error("Error del servidor:", result.message);
      }
    } catch (error) {
      console.error('Error de red obteniendo registros:', error);
    } finally {
      setLoading(false);
    }
  };

  // Efecto inicial: Carga los datos por defecto (últimos 30 días) al montar el componente
  useEffect(() => {
    fetchRegistros(fechaInicio, fechaFin);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // Función que se ejecuta al presionar el botón de "Aplicar Filtro"
  const handleBuscar = () => {
    fetchRegistros(fechaInicio, fechaFin);
  };

  // Configuración de las columnas para Material React Table
  const columns = useMemo<MRT_ColumnDef<RegistroTabla>[]>(
    () => [
      { accessorKey: 'Id', header: 'ID', size: 60 },
      { accessorKey: 'Fecha', header: 'Fecha / Hora', size: 160 },
      { accessorKey: 'Ubicacion', header: 'Ubicación' },
      { accessorKey: 'Navegador', header: 'Navegador' },
      { accessorKey: 'Usuario', header: 'Tipo Usuario' },
      {
        accessorKey: 'Estatus',
        header: 'Estatus',
        size: 120,
        Cell: ({ cell }) => {
          const valor = cell.getValue<string>();
          return (
            <span
              style={{
                padding: '4px 10px',
                borderRadius: '12px',
                backgroundColor: valor === 'Completado' ? '#e8f5e9' : '#e3f2fd',
                color: valor === 'Completado' ? '#2e7d32' : '#1565c0',
                fontWeight: '600',
                fontSize: '0.80rem',
                display: 'inline-block'
              }}
            >
              {valor}
            </span>
          );
        },
      },
    ],
    []
  );

  // Configuración dinámica de los datos para la gráfica de barras de Chart.js
  const chartData = useMemo(() => {
    return {
      labels: dataGrafica.map((d) => d.Fecha),
      datasets: [
        {
          label: 'Visitas Diarias',
          data: dataGrafica.map((d) => d.Cantidad),
          backgroundColor: '#00A0F5', // Azul oficial de Thyssenkrupp sólido para las barras
          borderRadius: 6, // Le da las esquinas redondeadas modernas a cada barra
          hoverBackgroundColor: '#0084cc', // Azul ligeramente más oscuro al pasar el mouse
        },
      ],
    };
  }, [dataGrafica]);

  // Opciones visuales de la gráfica
  const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false }, // Ocultamos la leyenda para mayor limpieza visual
      tooltip: {
        backgroundColor: '#333',
        titleFont: { size: 14 },
        bodyFont: { size: 14, weight: 'bold' as const },
        padding: 12,
        cornerRadius: 8,
      }
    },
    scales: {
      y: { 
        beginAtZero: true,
        grid: { color: '#f0f0f0' },
        ticks: { stepSize: 1 } // Fuerza a mostrar números enteros
      },
      x: {
        grid: { display: false } // Oculta las líneas verticales de la cuadrícula
      }
    },
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
      
      {/* --- BARRA SUPERIOR: CONTROLES DE FILTRO --- */}
      <div style={{ 
        display: 'flex', gap: '16px', alignItems: 'flex-end', backgroundColor: '#ffffff', 
        padding: '16px 24px', borderRadius: '12px', border: '1px solid #e0e0e0', boxShadow: '0 2px 10px rgba(0,0,0,0.03)' 
      }}>
        <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
          <label style={{ fontSize: '0.85rem', fontWeight: 600, color: '#555' }}>Desde:</label>
          <input 
            type="date" 
            value={fechaInicio} 
            onChange={(e) => setFechaInicio(e.target.value)}
            style={{ padding: '8px 12px', borderRadius: '6px', border: '1px solid #ccc', outline: 'none' }} 
          />
        </div>
        <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
          <label style={{ fontSize: '0.85rem', fontWeight: 600, color: '#555' }}>Hasta:</label>
          <input 
            type="date" 
            value={fechaFin} 
            onChange={(e) => setFechaFin(e.target.value)}
            style={{ padding: '8px 12px', borderRadius: '6px', border: '1px solid #ccc', outline: 'none' }} 
          />
        </div>
        <button 
          onClick={handleBuscar}
          disabled={loading}
          style={{ 
            padding: '10px 20px', backgroundColor: '#00A0F5', color: 'white', fontWeight: 600, 
            border: 'none', borderRadius: '6px', cursor: loading ? 'not-allowed' : 'pointer', transition: 'background 0.2s'
          }}
        >
          {loading ? 'Filtrando...' : 'Aplicar Filtro'}
        </button>
      </div>

      {/* --- PANEL INTERMEDIO: GRÁFICA DE BARRAS --- */}
      <div 
        style={{ 
          height: '320px', 
          backgroundColor: '#ffffff', 
          padding: '24px', 
          borderRadius: '12px', 
          border: '1px solid #e0e0e0',
          boxShadow: '0 2px 10px rgba(0,0,0,0.03)' 
        }}
      >
        <h3 style={{ marginTop: 0, marginBottom: '20px', color: '#111', fontSize: '1.25rem', fontWeight: 600 }}>
          Tráfico del periodo seleccionado
        </h3>
        
        {loading ? (
          <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%', color: '#666' }}>
            <div className="fa fa-spinner fa-spin fa-2x" style={{ marginRight: '10px', color: '#00A0F5' }}></div>
            Cargando analíticas...
          </div>
        ) : (
          <div style={{ height: '240px' }}>
            {dataGrafica.length > 0 ? (
              <Bar data={chartData} options={chartOptions} />
            ) : (
              <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%', color: '#999' }}>
                No hay datos suficientes para graficar en este periodo.
              </div>
            )}
          </div>
        )}
      </div>

      {/* --- PANEL INFERIOR: TABLA DE DATOS MATERIAL REACT TABLE --- */}
      <div style={{ 
        borderRadius: '12px', 
        overflow: 'hidden', 
        border: '1px solid #e0e0e0',
        boxShadow: '0 2px 10px rgba(0,0,0,0.03)' 
      }}>
        <MaterialReactTable 
          columns={columns} 
          data={dataTabla} 
          state={{ isLoading: loading }}
          enableGlobalFilter={true}
          enableColumnFilters={true}
          enablePagination={true}
          enableSorting={true}
          enableDensityToggle={false}
          initialState={{
            showGlobalFilter: true,
            pagination: { pageSize: 10, pageIndex: 0 },
            density: 'comfortable',
            sorting: [{ id: 'Fecha', desc: true }]
          }}
          muiTablePaperProps={{
            elevation: 0,
            sx: { borderRadius: '0' }
          }}
          muiTopToolbarProps={{
            sx: { backgroundColor: '#f8f9fa', padding: '10px' }
          }}
        />
      </div>

    </div>
  );
}