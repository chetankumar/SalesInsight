using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.Web.Html5.Models.ESI.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class BenchmarkAnalysisReportView : ReportSettingView
    {
        public BenchmarkAnalysisReportView()
        {

        }

        public BenchmarkAnalysisReportView(ReportSetting reportSetting, Guid userId, bool? persistFilter, long? widgetId, ReportFilterSettingView reportFilterSetting, UserReportSetting userReportSetting, DateTime? startDate, DateTime? endDate)
            : base(reportSetting, userId, persistFilter, widgetId, reportFilterSetting, userReportSetting, startDate, endDate)
        {

        }

        //public BenchmarkAnalysisReportView(DateTime startDate, DateTime endDate, ReportSetting reportSetting, Guid userId)
        //    : base(reportSetting, userId)
        //{
        //    this.StartDate = startDate;
        //    this.EndDate = endDate;
        //}

        public List<BenchmarkAnalysisResponsePayload> Response { get; set; }

        public void LoadData()
        {
            var esiReportBroker = new EsiReportBroker(this.StartDate.GetValueOrDefault(DateTime.Today), this.EndDate.GetValueOrDefault(DateTime.Today), this.ReportFilterSetting, this.ReportConfigSetting);

            this.Response = esiReportBroker.GetBenchmarkAnalysisReponse(this.ReportId,UserId);
        }
    }
}