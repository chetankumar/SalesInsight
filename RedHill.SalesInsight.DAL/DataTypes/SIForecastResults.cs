using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIForecastResults
    {
        public List<SIForecastProject> Forecasts { get; set; }
        public int RowCount { get; set; }
        public int CurrentPage { get; set; }
        public int NumRows { get; set; }
    }
}
