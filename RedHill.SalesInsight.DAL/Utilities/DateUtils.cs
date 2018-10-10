using RedHill.SalesInsight.DAL.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Utils
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
            string date = "01/"+(month)+"/"+year;
            DateTime startDate = DateTime.ParseExact(date, "dd/M/yyyy", CultureInfo.InvariantCulture);
            return startDate;
        }

        public static DateTime GetFirstOf(DateTime dateTime)
        {
            return GetFirstOf(dateTime.Month, dateTime.Year);
        }

        public static DateTime GetDateFromMonthString(string monthYear, string startDate)
        {
            return DateTime.ParseExact(startDate+" " + monthYear, "dd MMM, yyyy", CultureInfo.InvariantCulture);
        }

        public static DateTime[] GetStartAndEndDateForPeriodType(string periodType, DateTime date)
        {
            //TODO: Refine cases
            var startEnd = new DateTime[2];
            ESIPeriodType pType;
            Enum.TryParse<ESIPeriodType>(periodType, true, out pType);
            switch (pType)
            {
                case ESIPeriodType.Day:
                    startEnd[0] = date;
                    startEnd[1] = date;
                    break;
                case ESIPeriodType.LM:
                    startEnd[0] = GetFirstOf(date).AddMonths(-1);
                    startEnd[1] = startEnd[0].AddMonths(1).AddDays(-1);
                    break;
                case ESIPeriodType.QTD:
                    startEnd[0] = GetQuarterStartDate(date);
                    startEnd[1] = date;
                    break;
                case ESIPeriodType.YTD:
                    startEnd[0] = date.AddDays(-(date.DayOfYear - 1));
                    startEnd[1] = date;
                    break;
                case ESIPeriodType.PYYTD:
                    var pyDate = date.AddYears(-1);
                    startEnd[0] = pyDate.AddDays(-(pyDate.DayOfYear - 1));
                    startEnd[1] = pyDate;
                    break;
                case ESIPeriodType.MTD:
                    startEnd[0] = GetFirstOf(date);
                    startEnd[1] = date;
                    break;
                case ESIPeriodType.WTD:
                    startEnd[0] = date.AddDays(-((int)date.DayOfWeek - 1));
                    startEnd[1] = date;
                    break;
                default:
                    startEnd[0] = date;
                    startEnd[1] = date;
                    break;
            }
            return startEnd;
        }

        public static DateTime[] GetStartAndEndDateForPreviousPeriod(string periodType, DateTime startDate, DateTime endDate)
        {
            DateTime[] startEnd = new DateTime[2];
            ESIPeriodType pType = (ESIPeriodType)Enum.Parse(typeof(ESIPeriodType), periodType);

            switch (pType)
            {
                case ESIPeriodType.Day:
                    startEnd[0] = startDate.AddDays(-1);
                    startEnd[1] = endDate.AddDays(-1);
                    break;
                case ESIPeriodType.WTD:
                    startEnd[0] = startDate.AddDays(-7);
                    startEnd[1] = endDate.AddDays(-7);
                    break;
                case ESIPeriodType.MTD:
                    startEnd[0] = startDate.AddMonths(-1);
                    startEnd[1] = endDate.AddMonths(-1);
                    break;
                case ESIPeriodType.QTD:
                    startEnd[0] = startDate.AddDays(-90);
                    startEnd[1] = endDate.AddDays(-90);
                    break;
                case ESIPeriodType.YTD:
                    startEnd[0] = startDate.AddYears(-1);
                    startEnd[1] = endDate.AddYears(-1);
                    break;
                case ESIPeriodType.LM:
                    startEnd[0] = startDate.AddMonths(-1);
                    startEnd[1] = endDate.AddMonths(-1);
                    break;
                case ESIPeriodType.PYYTD:
                    startEnd[0] = startDate.AddYears(-1);
                    startEnd[1] = endDate.AddYears(-1);
                    break;
            }
            return startEnd;
        }

        public static DateTime GetQuarterStartDate(DateTime date)
        {
            return new DateTime(date.Year, (3 * GetQuarterName(date)) - 2, 1);
        }

        public static int GetQuarterName(DateTime date)
        {
            return (int)Math.Ceiling(date.Month / 3.0);
        }

        /// <summary>
        /// Substract two Dates and get result in minutes
        /// </summary>
        public static double SubstractDates(DateTime firstDate, DateTime lastDate)
        {
            return (lastDate - firstDate).TotalMinutes;
        }

        public static bool SanitizeDates(List<DateTime?> listOfDates, DateTime comparisionDate)
        {
            List<int> invalidIndexes = new List<int>();
            if (listOfDates != null)
            {
                #region Find indexes of Invalid dates

                for (int counter = 0; counter < listOfDates.Count; counter++)
                {
                    var currentDate = listOfDates[counter];
                    if (!currentDate.HasValue || Math.Abs((currentDate.Value - comparisionDate).Days) > 2)
                    {
                        invalidIndexes.Add(counter);
                    }
                }

                #endregion
                #region Clean Invalid Dates

                if (invalidIndexes.Count > 0)
                {
                    foreach (var index in invalidIndexes)
                    {
                        bool hasUpdated = false;
                        var tempIndex = index - 1;
                        while (tempIndex >= 0)
                        {
                            if (invalidIndexes.Contains(tempIndex))
                            {
                                tempIndex--;
                                continue;
                            }
                            else
                            {
                                listOfDates[index] = listOfDates[tempIndex];
                                hasUpdated = true;
                                break;
                            }
                        }
                        if (!hasUpdated)
                        {
                            tempIndex = index + 1;
                            while (tempIndex < listOfDates.Count)
                            {
                                if (invalidIndexes.Contains(tempIndex))
                                {
                                    tempIndex++;
                                    continue;
                                }
                                else
                                {
                                    listOfDates[index] = listOfDates[tempIndex];
                                    hasUpdated = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                #endregion
            }
            return listOfDates == null || invalidIndexes.Count > 0;
        }

        public static DateTime? ParseSafe(string value, string format = null)
        {
            try
            {
                if (format != null && (format == "m/y" || format == "mm/yyyy" || format == "mm/yy"))
                    format = "MM/yyyy";
                else
                    format = "M/d/yyyy";

                return TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(value, format, CultureInfo.InvariantCulture), TimeZoneInfo.Utc);
            }
            catch
            {
                try
                {
                    return TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(value, "M/d/yyyy", CultureInfo.InvariantCulture), TimeZoneInfo.Utc);
                }
                catch
                {
                    try
                    {
                        return TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(value, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture), TimeZoneInfo.Utc);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }

        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

        public static double GetMillis(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddHours(DateTimeOffset.Now.Hour);
            return (date - epoch).TotalMilliseconds;
        }
    }
}
