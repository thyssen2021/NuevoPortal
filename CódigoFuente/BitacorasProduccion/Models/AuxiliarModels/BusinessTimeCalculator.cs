using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public static class BusinessTimeCalculator
    {
        /// <summary>
        /// Devuelve la cantidad de tiempo hábil transcurrido entre start y end,
        /// sólo contando L–J 8:00–18:00 y V 8:00–15:00, excluyendo fines de semana y festivos.
        /// </summary>
        public static TimeSpan GetBusinessTime(
            DateTime start,
            DateTime end,
            IEnumerable<DateTime> holidays
        )
        {
            if (end <= start) return TimeSpan.Zero;
            var total = TimeSpan.Zero;
            var today = start.Date;
            var lastDay = end.Date;

            for (var day = today; day <= lastDay; day = day.AddDays(1))
            {
                // 1) Skip fines de semana
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                // 2) Skip festivos (hoy exacto)
                if (holidays.Any(h =>
                    h.Date == day.Date || (/* si Es recurrente */ false)))
                    continue;

                // 3) Define ventana laboral de este día
                TimeSpan workStart = new TimeSpan(8, 0, 0);
                TimeSpan workEnd = (day.DayOfWeek == DayOfWeek.Friday)
                                    ? new TimeSpan(15, 0, 0)
                                    : new TimeSpan(18, 0, 0);

                var windowStart = day + workStart;
                var windowEnd = day + workEnd;

                // 4) Cálculo de overlap con [start,end]
                var segmentStart = start > windowStart ? start : windowStart;
                var segmentEnd = end < windowEnd ? end : windowEnd;

                if (segmentEnd > segmentStart)
                    total += segmentEnd - segmentStart;
            }

            return total;
        }
    }
}