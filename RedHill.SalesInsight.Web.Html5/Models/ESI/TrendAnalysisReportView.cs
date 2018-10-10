using RedHill.SalesInsight.Web.Html5.Models.ESI.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedHill.SalesInsight.DAL;
using System.Web.Script.Serialization;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class TrendAnalysisReportView : ReportSettingView
    {
        public List<SelectListItem> TargetMetricList { get; set; }
        public int TargetDimensionId { get; set; }
        public List<SelectListItem> MetricDefinitionList { get; set; }
        public int MetricDefinitionId { get; set; }
        public double UpperControlLimit { get; set; }
        public double LowerControlLimit { get; set; }
        public bool OmitPeriodsWithNoData { get; set; }
        public bool IsScallingAutoFit { get; set; }

        public string PrimaryMetric { get; set; }


        public string ToJson()
        {
            if (this.Response == null)
            {
                //this.Response.ForEach(x => x.Values.OrderBy(xi => xi.Date));
                return "";
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(this.Response);
        }

        public TrendAnalysisReportView()
        {

        }

        public TrendAnalysisReportView(ReportSetting reportSetting, Guid userId, bool? persistFilter, long? widgetId, ReportFilterSettingView reportFilterSetting, UserReportSetting userReportSetting, DateTime? startDate, DateTime? endDate) : base(reportSetting, userId, persistFilter, widgetId,reportFilterSetting, userReportSetting, startDate, endDate)
        {
            //Initialize();
        }

        public TrendAnalysisReportView(DateTime startDate, DateTime? endDate, ReportSetting reportSetting, Guid userId) : base(reportSetting, userId)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            //Initialize();
        }

        public List<TrendAnalysisResponsePayload> Response { get; set; }

        public void Initialize()
        {
            var metricDefs = SIDAL.GetAllMetricDefinition(true);

            // Get id of Target Dimension From TrendAnalysis Report Configuration Setting
            if (TrendReportConfigSetting.TrendAnalysisReportConfig != null)
            {
                // Get id of Dimension From TrendAnalysis Report Configuration Setting 
                MetricDefinitionId = TrendReportConfigSetting.TrendAnalysisReportConfig.MetricDefinitionId;

                this.PrimaryMetric = metricDefs.Where(x => x.Id == MetricDefinitionId).Select(x => x.DisplayName).FirstOrDefault();

                TargetDimensionId = TrendReportConfigSetting.TrendAnalysisReportConfig.TargetMetricDefinitionId.GetValueOrDefault();

                // Get details (Upper/Lower Limit and other information)
                UpperControlLimit = TrendReportConfigSetting.TrendAnalysisReportConfig.UpperControlLimit.GetValueOrDefault();
                LowerControlLimit = TrendReportConfigSetting.TrendAnalysisReportConfig.LowerControlLimit.GetValueOrDefault();
                IsScallingAutoFit = TrendReportConfigSetting.TrendAnalysisReportConfig.IsScallingAutoFit.GetValueOrDefault();
                OmitPeriodsWithNoData = TrendReportConfigSetting.TrendAnalysisReportConfig.OmitPeriodsWithNoData.GetValueOrDefault();
            }

            TargetMetricList = new List<SelectListItem>();
            foreach (MetricDefinition metricDef in metricDefs)
            {
                SelectListItem item = new SelectListItem();
                item.Text = metricDef.DisplayName ?? metricDef.MetricName;
                item.Value = metricDef.Id.ToString();
                item.Selected = TargetDimensionId == metricDef.Id ? true : false;
                this.TargetMetricList.Add(item);
            }

            MetricDefinitionList = new List<SelectListItem>();
            foreach (MetricDefinition metricDef in metricDefs)
            {
                SelectListItem item = new SelectListItem();
                item.Text = metricDef.DisplayName ?? metricDef.MetricName;
                item.Value = metricDef.Id.ToString();
                item.Selected = MetricDefinitionId == metricDef.Id ? true : false;
                this.MetricDefinitionList.Add(item);
            }
        }

        public void LoadData()
        {
            var esiReportBroker = new EsiReportBroker(this.StartDate.GetValueOrDefault(), this.EndDate, this.ReportFilterSetting, this.ReportConfigSetting);

            this.Response = esiReportBroker.GetTrendAnalysisResponse(this.TrendReportConfigSetting.TrendAnalysisReportConfig,this.ReportId,UserId);
        }
    }
}