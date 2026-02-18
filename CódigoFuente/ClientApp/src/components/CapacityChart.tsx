import React from 'react';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler,
} from 'chart.js';
import type { ChartData, ChartOptions } from 'chart.js';
import { Line } from 'react-chartjs-2';
import annotationPlugin from 'chartjs-plugin-annotation';

// Registrar componentes de Chart.js
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler,
  annotationPlugin
);

// --- PLUGIN PERSONALIZADO: TOTAL LABELS (Igual al Legacy) ---
// Dibuja el porcentaje total (ej: "67.7%") encima de la barra apilada
const totalLabelPlugin = {
  id: 'totalLabels',
  afterDatasetsDraw(chart: any) {
    const { ctx, scales: { x, y } } = chart;
    ctx.save();
    ctx.font = 'bold 12px sans-serif';
    ctx.fillStyle = '#555555';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'bottom';

    const datasets = chart.data.datasets;
    const meta = chart.getDatasetMeta(datasets.length - 1); // Usamos el último dataset para coordenadas X

    // Usamos '_' en lugar de 'element' para indicar que no se usa
    meta.data.forEach((_: any, index: number) => {
      // Sumar el total de este índice (Año Fiscal)
      let total = 0;
      datasets.forEach((dataset: any) => {
        if (!dataset.hidden) {
            total += dataset.data[index] || 0;
        }
      });

      if (total > 0) {
        const xPos = x.getPixelForValue(index);
        const yPos = y.getPixelForValue(total);
        // Dibujamos el texto 5 pixeles arriba del punto
        ctx.fillText(`${total.toFixed(1)}%`, xPos, yPos - 5);
      }
    });
    ctx.restore();
  }
};

interface Props {
  data: ChartData<"line">;
  maxPercentage: number;
  fyStart?: string; // Ej: "FY 26/27" (Para línea vertical de inicio)
  fyEnd?: string;   // Ej: "FY 30/31" (Para línea vertical de fin)
  title?: string;
}

export const CapacityChart: React.FC<Props> = ({ data, maxPercentage, fyStart, fyEnd, title }) => {

  // --- REGLA LEGACY: SPARE PARTS TRANSPARENTE ---
  // Modificamos los datasets al vuelo para aplicar el estilo "Outline" a Spare Parts
  const processedDatasets = data.datasets.map(ds => {
    const isSpareParts = ds.label === "Spare Parts";
    return {
      ...ds,
      backgroundColor: isSpareParts ? 'transparent' : ds.backgroundColor,
      borderColor: ds.backgroundColor, // El borde mantiene el color
      borderWidth: isSpareParts ? 2 : 1, // Borde más grueso para Spare Parts
      pointRadius: 0, // Sin puntos, como en legacy
      lineTension: 0, // Líneas rectas
      fill: true
    };
  });

  const chartData = { ...data, datasets: processedDatasets };

 // --- CONFIGURACIÓN DE OPCIONES (Réplica exacta de Legacy) ---
  const options: ChartOptions<"line"> = {
    responsive: true,
    maintainAspectRatio: false,
    interaction: {
      mode: 'index' as const,
      intersect: false,
    },
    scales: {
      x: {
        stacked: true,
        grid: { display: false }
      },
      y: {
        stacked: true,
        beginAtZero: true,
        max: maxPercentage,
        title: { display: true, text: 'Capacity (%)' },
        border: { dash: [4, 4] }
      },
    },
    plugins: {
        title: {
        display: !!title,
        text: title,
        font: { size: 16, weight: 'bold' },
        color: '#333',
        padding: { bottom: 20 }
      },
      legend: {
        position: 'top' as const,
        labels: { usePointStyle: true, boxWidth: 10 }
      },
      tooltip: {
        callbacks: {
          footer: (tooltipItems) => {
            // (e.parsed.y ?? 0) evita error de objeto posiblemente nulo
            const total = tooltipItems.reduce((a, e) => a + (e.parsed.y ?? 0), 0);
            return 'Total: ' + total.toFixed(2) + '%';
          }
        }
      },
      // --- ANOTACIONES (Líneas Rojas y Grises) ---
      annotation: {
        // 👇 AQUÍ AGREGAMOS 'as any' PARA EVITAR EL ERROR DE TIPADO
        annotations: {
          // Líneas de Capacidad (Rojas)
          line100: {
            type: 'line',
            yMin: 100, yMax: 100,
            borderColor: 'red', borderWidth: 2, borderDash: [6, 6],
            label: { display: true, content: '100% Cap.', position: 'end', color: 'red', backgroundColor: 'rgba(255,255,255,0.8)', font: { size: 10 } }
          },
          line85: {
            type: 'line',
            yMin: 85, yMax: 85,
            borderColor: 'red', borderWidth: 2, borderDash: [6, 6],
            label: { display: true, content: '85% Cap.', position: 'end', color: 'red', backgroundColor: 'rgba(255,255,255,0.8)', font: { size: 10 } }
          },
          // Líneas de Turnos (Grises)
          lineShift1: {
            type: 'line', yMin: 25, yMax: 25,
            borderColor: '#666', borderWidth: 1, borderDash: [2, 2],
            label: { display: true, content: '1st Shift', position: 'start', color: '#666', backgroundColor: 'transparent', font: { size: 9 } }
          },
          lineShift2: {
            type: 'line', yMin: 53, yMax: 53,
            borderColor: '#666', borderWidth: 1, borderDash: [2, 2],
            label: { display: true, content: '2nd Shift', position: 'start', color: '#666', backgroundColor: 'transparent', font: { size: 9 } }
          },
          lineShift3: {
            type: 'line', yMin: 80, yMax: 80,
            borderColor: '#666', borderWidth: 1, borderDash: [2, 2],
            label: { display: true, content: '3rd Shift', position: 'start', color: '#666', backgroundColor: 'transparent', font: { size: 9 } }
          },
          // Líneas Verticales (SOP / EOP)
          ...(fyStart ? {
            lineSOP: {
              type: 'line', scaleID: 'x', value: fyStart,
              borderColor: '#333', borderWidth: 2,
              label: { display: true, content: 'FY Start', position: 'top', backgroundColor: '#333', color: '#fff' }
            }
          } : {}),
          ...(fyEnd ? {
            lineEOP: {
              type: 'line', scaleID: 'x', value: fyEnd,
              borderColor: '#333', borderWidth: 2,
              label: { display: true, content: 'FY End', position: 'top', backgroundColor: '#333', color: '#fff' }
            }
          } : {})
        } as any // 👈 EL 'as any' MÁGICO VA AQUÍ AL CIERRE DE LA LLAVE
      }
    },
  };
  return (
    <div style={{ height: '350px', width: '100%' }}>
      <Line data={chartData} options={options} plugins={[totalLabelPlugin]} />
    </div>
  );
};