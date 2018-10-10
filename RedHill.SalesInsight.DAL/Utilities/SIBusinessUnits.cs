using RedHill.SalesInsight.DAL.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedHill.SalesInsight.DAL.Utilities
{
    public static class SIBusinessUnits
    {
        public static Dictionary<string, string> PeriodType { get; set; }

        static SIBusinessUnits()
        {
            Initialize();
        }

        static void Initialize()
        {
            PeriodType = new Dictionary<string, string>();

            PeriodType.Add(ESIPeriodType.Day.ToString(), "Day");
            PeriodType.Add(ESIPeriodType.WTD.ToString(), "WTD");
            PeriodType.Add(ESIPeriodType.MTD.ToString(), "MTD");
            PeriodType.Add(ESIPeriodType.QTD.ToString(), "QTD");
            PeriodType.Add(ESIPeriodType.YTD.ToString(), "YTD");
            PeriodType.Add(ESIPeriodType.LM.ToString(), "LM");
            PeriodType.Add(ESIPeriodType.PYYTD.ToString(), "PY-YTD");

        }
    }
}
