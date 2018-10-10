using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.DAL;
 
namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class ReportConfigSettingView
    {
        public List<ReportRowConfig> ReportRowConfigList { get; set; }
        public List<ReportColumnConfig> ReportColumnConfigList { get; set; }
        public ReportConfigSettingView()
        {

        }
        public ReportConfigSettingView(long reportId)
        {
            ReportRowConfigList = SIDAL.GetReportRowConfig(reportId);
            ReportColumnConfigList = SIDAL.GetReportColumnConfig(reportId);
        }
    }
}