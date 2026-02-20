import type { EngineeringRange } from '../types';

export const getActiveRanges = (
    formData: any, 
    allRanges: EngineeringRange[]
): EngineeringRange[] => {
    // 1. Determinar la Línea Activa (Real mata a Teórica)
    // Usamos Number() para asegurar comparación correcta
    const realLineId = Number(formData.ID_Real_Blanking_Line);
    const theoreticalLineId = Number(formData.ID_Theoretical_Blanking_Line);
    
    // Si hay línea real seleccionada (y no es 0), esa manda. Si no, la teórica.
    const activeLineId = (realLineId && realLineId !== 0) ? realLineId : theoreticalLineId;

    const materialTypeId = Number(formData.ID_Material_type);

    if (!activeLineId || !materialTypeId || !allRanges) return [];

    // 2. Filtrar los rangos que coincidan con Línea y Material
    return allRanges.filter(r => 
        r.lineId === activeLineId && 
        r.materialTypeId === materialTypeId
    );
};