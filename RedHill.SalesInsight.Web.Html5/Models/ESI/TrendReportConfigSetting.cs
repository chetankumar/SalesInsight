using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class TrendReportConfigSetting
    {
        public TrendAnalysisReportConfig TrendAnalysisReportConfig { get; set; }
        public TrendReportConfigSetting()
        { }

        public TrendReportConfigSetting(long reportId)
        {
            this.TrendAnalysisReportConfig = SIDAL.GetTrendReportConfig(reportId);
        }
    }
}