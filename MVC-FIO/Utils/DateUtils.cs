using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MVC_FIO.Utils
{
    public class DateUtils
    {
        public static DateTime[] GetFirstLastDateOfYear(int year)
        {
            string date = "01/01/" + year;
            DateTime startDate = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = startDate.AddYears(1);
            endDate = endDate.AddDays(-1);
            return new DateTime[] { startDate, endDate };
        }

        public static DateTime GetFirstOf(int month, int year)
        {
            string date = "01/"+month+"/year";
            DateTime startDate = DateTime.ParseExact(date, "dd/M/yyyy", CultureInfo.InvariantCulture);
            return startDate;
        }

        public static DateTime GetFirstOf(DateTime date)
        {
            return GetFirstOf(date.Month, date.Year);
        }
    }
}
