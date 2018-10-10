using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class ActualsHistory
    {
        public ActualsHistory()
        {
            this.LYTrend = new List<DateValue>();
            this.LSActuals = new List<DateValue>();
        }

        public List<DateValue> LYTrend { get; set; }
        public List<DateValue> LSActuals { get; set; }
    }

    public class DateValue
    {
        public string Date { get; set; }
        public int Value { get; set; }

        public DateValue(DateTime d, int value)
        {
            this.Value = value;
            this.Date = d.ToString("MMM, yyyy");
        }
    }
}
