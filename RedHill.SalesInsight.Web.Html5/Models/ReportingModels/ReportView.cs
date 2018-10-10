using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ReportView
    {
        public ReportFilter SalesForecastFilter { get; set; }
        public ReportFilter WonLostReportFilter { get; set; }

        public ReportView(Guid userId)
        {
            this.SalesForecastFilter = new ReportFilter(userId);
            this.WonLostReportFilter = new ReportFilter(userId);
        }
    }
}