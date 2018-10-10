using RedHill.SalesInsight.Web.Html5.Models.ESI.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.DAL;
using System.Web.Mvc;
using RedHill.SalesInsight.DAL.Utilities;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class GoalAnalysisReportView : ReportSettingView
    {
        public List<SelectListItem> MetricPeriodList { get; set; }
        public List<string> MetricPeriods { get; set; }

        public GoalAnalysisReportView()
        {

        }

        public GoalAnalysisReportView(ReportSetting reportSetting, Guid userId, bool? persistFilter, long? widgetId, ReportFilterSettingView reportFilterSetting, UserReportSetting userReportSetting, DateTime? startDate, DateTime? endDate) : base(reportSetting, userId, persistFilter, widgetId, reportFilterSetting, userReportSetting, startDate, endDate)
        {
            LoadMetricPeriodList();
        }

        //public GoalAnalysisReportView(DateTime startDate, DateTime? endDate, ReportSetting reportSetting, Guid userId) : base(reportSetting, userId)
        //{

        //}

        public List<GoalAnalysisResponsePayload> Response { get; set; }

        public void LoadMetricPeriodList()
        {
            MetricPeriods = ReportConfigSetting.ReportColumnConfigList.Select(x => x.ColumnName).ToList();

            MetricPeriodList = new List<SelectListItem>();
            foreach (KeyValuePair<string, string> kvP in SIBusinessUnits.PeriodType)
            {
                SelectListItem item = new SelectListItem();
                item.Text = kvP.Value;
                item.Value = kvP.Key;
                item.Selected = MetricPeriods.Contains(kvP.Key);
                this.MetricPeriodList.Add(item);
            }
        }
        public void LoadData()
        {
            var esiReportBroker = new EsiReportBroker(this.StartDate.GetValueOrDefault(DateTime.Today), this.EndDate, this.ReportFilterSetting, this.ReportConfigSetting);

            this.Response = esiReportBroker.GetGoalAnalysisResponse(this.ReportId,UserId);
        }
    }
}