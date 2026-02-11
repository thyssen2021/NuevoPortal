import type { TheoreticalRule } from '../types';

export const calculateTheoreticalLine = (
    formData: any, 
    rules: TheoreticalRule[]
): { lineId: number; lineName: string } | null => {

    // 1. Validar que exista el Material Type
    const currentMatType = parseInt(formData.ID_Material_type);
    if (!currentMatType) return null;

    // 2. Parsear valores del formulario (asegurar números)
    // Nota: Usamos Number() y validamos isNaN para evitar errores con strings vacíos
    const thickness = parseFloat(formData.Thickness);
    const width = parseFloat(formData.Width);
    const pitch = parseFloat(formData.Pitch);
    const tensile = parseFloat(formData.Tensile_Strenght);

    // 3. Filtrar reglas candidatas (Solo del mismo material)
    const candidates = rules.filter(r => r.materialTypeId === currentMatType);

    if (candidates.length === 0) return null;

    // 4. Evaluar y Puntuar (Scoring Logic idéntica al Legacy C#)
    const scoredRules = candidates.map(rule => {
        let score = 0;

        // Evalúa Thickness
        if (!isNaN(thickness) && rule.thicknessMin !== null && rule.thicknessMax !== null) {
            if (thickness >= rule.thicknessMin && thickness <= rule.thicknessMax) {
                score++;
            }
        }

        // Evalúa Width
        if (!isNaN(width) && rule.widthMin !== null && rule.widthMax !== null) {
            if (width >= rule.widthMin && width <= rule.widthMax) {
                score++;
            }
        }

        // Evalúa Pitch
        if (!isNaN(pitch) && rule.pitchMin !== null && rule.pitchMax !== null) {
            if (pitch >= rule.pitchMin && pitch <= rule.pitchMax) {
                score++;
            }
        }

        // Evalúa Tensile
        if (!isNaN(tensile) && rule.tensileMin !== null && rule.tensileMax !== null) {
            if (tensile >= rule.tensileMin && tensile <= rule.tensileMax) {
                score++;
            }
        }

        return { rule, score };
    });

    // 5. Ordenar: Mayor Score primero, luego Menor Prioridad (Desempate)
    scoredRules.sort((a, b) => {
        if (b.score !== a.score) {
            return b.score - a.score; // Descendente por Score
        }
        return a.rule.priority - b.rule.priority; // Ascendente por Prioridad
    });

    // 6. Seleccionar el mejor
    const bestMatch = scoredRules[0];

    // Opcional: Puedes poner un umbral mínimo de score si lo deseas, 
    // pero el legacy retornaba el primero de la lista ordenada.
    if (bestMatch) {
        return {
            lineId: bestMatch.rule.resultingLineId,
            lineName: bestMatch.rule.resultingLineName
        };
    }

    return null;
};