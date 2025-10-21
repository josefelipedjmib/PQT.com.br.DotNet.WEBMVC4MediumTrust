using System;
using System.Globalization;

namespace Auxiliar.Helper
{
    public static class CalendarioHelper
    {

        public static DateTime PrimeiraQuintaDaSemana(int ano, int semanaDoAno)
        {
            var data = new DateTime(ano, 1, 1);
            int firstWeek = PegarSemanaDaData(data);

            var weekNum = semanaDoAno;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            return PegarPrimeiraQuinta(data).AddDays(weekNum * 7);
        }

        public static int PegarSemanaDaData(DateTime data)
        {
            var cal = CultureInfo.CurrentCulture.Calendar;
            return cal.GetWeekOfYear(PegarPrimeiraQuinta(data), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime PegarPrimeiraQuinta(DateTime data)
        {
            int daysOffset = DayOfWeek.Thursday - data.DayOfWeek;
            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            return data.AddDays(daysOffset);
        }

    }
}
