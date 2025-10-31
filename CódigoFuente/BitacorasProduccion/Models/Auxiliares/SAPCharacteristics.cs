using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models.Auxiliares
{
    public enum SAPCharacteristics
    {
        COMMODITY,
        GRADE,
        SHAPE,
        SURFACE,
        COATING_WEIGHT,
        WIDTH_M,
        GAUGE_M,
        FLATNESS_M,
        SURFACE_TREATMENT,
        MILL,
        CUSTOMER_NUMBER,
        CUSTOMER_PART_NUMBER,
        CUSTOMER_PART2,
        LENGTH_M,
        ID_COIL_M,
        OD_COIL_M
    }

    public static class EnumExtensions
    {
        public static string GetCharName(this Enum value)
        {
            // Esta función obtiene el nombre del miembro del enum.
            return value.ToString();
        }
    }
}