using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class Calculation
    {
        private const string DateFormat = "yyyy-MM-dd";

        private readonly List<string> _holidays;
        private readonly OpenHours _openHours;

        public Calculation(IEnumerable<DateTime> holidays, OpenHours openHours)
        {
            _holidays = dateListToStringList(holidays);
            _openHours = openHours;
        }

        public double getElapsedMinutes(DateTime startDate, DateTime endDate)
        {
            if (_openHours.StartHour == 0 || _openHours.EndHour == 0)
                throw new InvalidOperationException("Open hours cannot be started with zero hours or ended with zero hours");

            // Asegurarse de que las fechas están dentro del horario laboral
            startDate = nextOpenDay(startDate);
            endDate = prevOpenDay(endDate);

            if (startDate > endDate)
                return 0;

            // Si son la misma fecha, diferente hora
            if (startDate.ToString(DateFormat).Equals(endDate.ToString(DateFormat)))
            {
                if (!isWorkingDay(startDate))
                    return 0;

                if (isDateBeforeOpenHours(startDate))
                {
                    startDate = getStartOfDay(startDate);
                }
                if (isDateAfterOpenHours(endDate))
                {
                    endDate = getEndOfDay(endDate);
                }

                // Si es viernes y la hora de fin es después de las 15:00, ajusta la hora de fin
                if (endDate.DayOfWeek == DayOfWeek.Friday && endDate.Hour >= 15)
                {
                    endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 15, 0, 0);
                }

                var endminutes = (endDate.Hour * 60) + endDate.Minute;
                var startminutes = (startDate.Hour * 60) + startDate.Minute;

                return Math.Max(0, endminutes - startminutes);
            }

            // Son diferentes fechas
            var endOfDay = getEndOfDay(startDate);
            var startOfDay = getStartOfDay(endDate);
            var usedMinutesinEndDate = endDate.Subtract(startOfDay).TotalMinutes;
            var usedMinutesinStartDate = endOfDay.Subtract(startDate).TotalMinutes;
            var tempStartDate = startDate.AddDays(1);
            var workingHoursInMinutes = (_openHours.EndHour - _openHours.StartHour) * 60;

            // Si la fecha final es viernes y es después de las 15:00
            if (endDate.DayOfWeek == DayOfWeek.Friday && endDate.Hour >= 15)
            {
                endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 15, 0, 0);
                usedMinutesinEndDate = endDate.Subtract(startOfDay).TotalMinutes;
            }

            // Si la fecha inicial es viernes
            if (startDate.DayOfWeek == DayOfWeek.Friday)
            {
                usedMinutesinStartDate = Math.Max(0, new DateTime(endOfDay.Year, endOfDay.Month, endOfDay.Day, 15, 0, 0).Subtract(startDate).TotalMinutes);
            }

            var totalUsedMinutes = usedMinutesinEndDate + usedMinutesinStartDate;

            // Iterar sobre los días intermedios
            for (DateTime day = tempStartDate.Date; day < endDate.Date; day = day.AddDays(1.0))
            {
                if (isWorkingDay(day))
                {
                    totalUsedMinutes += (day.DayOfWeek == DayOfWeek.Friday) ? 450 : workingHoursInMinutes; // 450 minutos son 7.5 horas para los viernes
                }
            }

            return totalUsedMinutes;
        }
        public DateTime add(DateTime date, int minutes)
        {
            if (_openHours != null)
            {
                if (_openHours.StartHour == 0 || _openHours.EndHour == 0)
                    throw new InvalidOperationException("Open hours cannot be started with zero hours or ended with zero hours");

                date = nextOpenDay(date);
                var endOfDay = getEndOfDay(date);
                var minutesLeft = (int)endOfDay.Subtract(date).TotalMinutes;

                if (minutesLeft < minutes)
                {
                    date = nextOpenDay(endOfDay.AddMinutes(1));
                    date = nextOpenDay(date);
                    minutes -= minutesLeft;
                }
                var workingHoursInMinutes = (_openHours.EndHour - _openHours.StartHour) * 60;
                while (minutes > workingHoursInMinutes)
                {
                    date = getStartOfDay(date.AddDays(1));
                    date = nextOpenDay(date);
                    minutes -= workingHoursInMinutes;
                }
            }

            return date.AddMinutes(minutes);

        }


        private List<string> dateListToStringList(IEnumerable<DateTime> dates)
        {
            return dates.Select(piDate => piDate.ToString(DateFormat)).ToList();
        }


        private DateTime prevOpenDay(DateTime endDate)
        {
            if (_holidays.Contains(endDate.ToString(DateFormat)))
            {
                return prevOpenDayAfterHoliday(endDate);
            }
            if (endDate.DayOfWeek == DayOfWeek.Saturday)
            {
                return prevOpenDayAfterHoliday(endDate);
            }
            if (endDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return prevOpenDayAfterHoliday(endDate);
            }
            if (isDateBeforeOpenHours(endDate))
            {
                return getStartOfDay(endDate);
            }
            if (isDateAfterOpenHours(endDate))
            {
                return getEndOfDay(endDate);
            }
            return endDate;
        }

        private bool isWorkingDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday &&
                   !_holidays.Contains(date.ToString(DateFormat));
        }




        private DateTime nextOpenDay(DateTime startDate)
        {
            if (_holidays.Contains(startDate.ToString(DateFormat)))
            {
                return nextOpenDayAfterHoliday(startDate);
            }
            if (startDate.DayOfWeek == DayOfWeek.Saturday)
            {
                return nextOpenDayAfterHoliday(startDate);
            }
            if (startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return nextOpenDayAfterHoliday(startDate);
            }
            if (isDateBeforeOpenHours(startDate))
            {
                return getStartOfDay(startDate);
            }
            if (isDateAfterOpenHours(startDate))
            {

                var nextDate = startDate.AddDays(1);

                if (_holidays.Contains(nextDate.ToString(DateFormat)))
                {
                    return nextOpenDayAfterHoliday(nextDate);
                }
                return getStartOfDay(nextDate);
            }
            return startDate;
        }

        private DateTime nextOpenDayAfterHoliday(DateTime holiday)
        {
            var nextDay = holiday.AddDays(1);
            if (nextDay.DayOfWeek == DayOfWeek.Saturday)
                nextDay = nextDay.AddDays(2);
            if (nextDay.DayOfWeek == DayOfWeek.Sunday)
                nextDay = nextDay.AddDays(1);
            while (_holidays.Contains(nextDay.ToString(DateFormat)))
            {
                nextDay = nextDay.AddDays(1);
            }
            return getStartOfDay(nextDay);
        }

        private DateTime prevOpenDayAfterHoliday(DateTime holiday)
        {
            var prevDay = holiday.AddDays(-1);
            if (prevDay.DayOfWeek == DayOfWeek.Saturday)
                prevDay = prevDay.AddDays(-1);
            if (prevDay.DayOfWeek == DayOfWeek.Sunday)
                prevDay = prevDay.AddDays(-2);
            while (_holidays.Contains(prevDay.ToString(DateFormat)))
            {
                prevDay = prevDay.AddDays(-1);
            }
            return getEndOfDay(prevDay);
        }

        private DateTime getStartOfDay(DateTime nextDate)
        {
            return DateTime.Parse(string.Format("{0} {1}:{2}", nextDate.ToString(DateFormat), _openHours.StartHour, _openHours.StartMinute));
        }

        private DateTime getEndOfDay(DateTime startDate)
        {
            if (startDate.DayOfWeek == DayOfWeek.Friday)
            {
                // Si es viernes y la hora es mayor a las 15:00, ajusta el fin del día laboral a las 15:00
                return new DateTime(startDate.Year, startDate.Month, startDate.Day, 15, 0, 0);
            }
            else
            {
                return new DateTime(startDate.Year, startDate.Month, startDate.Day, _openHours.EndHour, _openHours.EndMinute, 0);
            }
        }

        private bool isDateBeforeOpenHours(DateTime startDate)
        {
            return startDate.Hour < _openHours.StartHour || (startDate.Hour == _openHours.StartHour && startDate.Minute < _openHours.StartMinute);
        }
        private bool isDateAfterOpenHours(DateTime startDate)
        {
            return startDate.Hour > _openHours.EndHour || (startDate.Hour == _openHours.EndHour && startDate.Minute > _openHours.EndMinute);
        }

    }
}