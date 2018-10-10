using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class WidgetSettingsView
    {
        private List<SelectListItem> _metricDefinitionList;

        public long WidgetId { get; set; }
        public long DashboardId { get; set; }
        public string Title { get; set; }
        public string MetricType { get; set; }
        public int? DecimalPlaces { get; set; }
        public string BackgroundColor { get; set; }
        public int PrimaryMetricDefinitionId { get; set; }
        public string PrimaryMetricPeriod { get; set; }
        public int ComparisonMetricDefinitionId { get; set; }
        public string ComparisonMetricPeriod { get; set; }
        public bool ShowActionIcon { get; set; }
        public decimal? SuccessLimitPercent { get; set; }
        public decimal? AlertLimitPercent { get; set; }
        public bool HasBarGraph { get; set; }
        public int? BarGraphDaysPerBar { get; set; }
        public bool HasFrequencyDistribution { get; set; }
        public bool HasLineGraph { get; set; }
        public int? LineGraphRangeInDays { get; set; }
        public bool HasPOPSummary { get; set; }
        public bool HasStaticMessage { get; set; }
        public string StaticMessage { get; set; }

        public string VisualIndicationType { get; set; }

        public long GoalAnalysisReportId { get; set; }
        public long TrendAnalysisReportId { get; set; }
        public long BenchmarkReportId { get; set; }
        public long DrillInReportId { get; set; }
        public int Position { get; set; }
        public double Value { get; set; }

        public WidgetSettingsView()
        {

        }

        public WidgetSettingsView(WidgetSetting widgetSetting)
        {
            this.WidgetId = widgetSetting.WidgetId;
            this.DashboardId = widgetSetting.DashboardId;
            this.Title = widgetSetting.Title;
            this.MetricType = widgetSetting.MetricType;
            this.DecimalPlaces = widgetSetting.DecimalPlaces;
            this.BackgroundColor = widgetSetting.BackgroundColor;
            this.PrimaryMetricDefinitionId = widgetSetting.PrimaryMetricDefinitionId.GetValueOrDefault();
            this.PrimaryMetricPeriod = widgetSetting.PrimaryMetricPeriod;
            this.ComparisonMetricDefinitionId = widgetSetting.ComparisonMetricDefinitionId.GetValueOrDefault();
            this.ComparisonMetricPeriod = widgetSetting.ComparisonMetricPeriod;
            this.ShowActionIcon = widgetSetting.ShowActionIcon.GetValueOrDefault(false);
            this.SuccessLimitPercent = widgetSetting.SuccessLimitPercent;
            this.AlertLimitPercent = widgetSetting.AlertLimitPercent;
            this.HasBarGraph = widgetSetting.HasBarGraph.GetValueOrDefault(false);
            this.BarGraphDaysPerBar = widgetSetting.BarGraphDaysPerBar;
            this.HasFrequencyDistribution = widgetSetting.HasFrequencyDistribution.GetValueOrDefault(false);
            this.HasLineGraph = widgetSetting.HasLineGraph.GetValueOrDefault(false);
            this.LineGraphRangeInDays = widgetSetting.LineGraphRangeInDays;
            this.HasPOPSummary = widgetSetting.HasPOPSummary.GetValueOrDefault(false);
            this.HasStaticMessage = widgetSetting.HasStaticMessage.GetValueOrDefault(false);
            this.StaticMessage = widgetSetting.StaticMessage;
            this.GoalAnalysisReportId = widgetSetting.GoalAnalysisReportId.GetValueOrDefault();
            this.TrendAnalysisReportId = widgetSetting.TrendAnalysisReportId.GetValueOrDefault();
            this.BenchmarkReportId = widgetSetting.BenchmarkReportId.GetValueOrDefault();
            this.DrillInReportId = widgetSetting.DrillInReportId.GetValueOrDefault();
            this.Position = widgetSetting.Position.GetValueOrDefault();
        }

        public IEnumerable<SelectListItem> MetricTypeList
        {
            get
            {
                string[] labels = { "higher better", "lower better" };
                string[] values = { "higherbetter", "lowerbetter" };
                List<SelectListItem> items = new List<SelectListItem>();
                for (int i = 0; i < labels.Count(); i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = labels[i];
                    item.Value = values[i];
                    items.Add(item);
                }
                return items;
            }
        }


        public IEnumerable<SelectListItem> MetricPeriodList
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (var kvP in SIBusinessUnits.PeriodType)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = kvP.Value;
                    item.Value = kvP.Key;
                    items.Add(item);
                }
                return items;
            }
        }

        public IEnumerable<SelectListItem> RoundingOptions
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();

                for (int i = 0; i <= 5; i++)
                {
                    list.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
                }
                return list;
            }
        }

        public IEnumerable<SelectListItem> MetricDefinitionList
        {
            get
            {
                if (_metricDefinitionList == null)
                {
                    List<SelectListItem> list = new List<SelectListItem>();

                    var metricDefintions = SIDAL.GetAllMetricDefinition(true);

                    if (metricDefintions != null)
                    {
                        foreach (var item in metricDefintions)
                        {
                            list.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.DisplayName ?? item.MetricName });
                        }
                    }

                    _metricDefinitionList = list;
                }

                return _metricDefinitionList;
            }
        }
    }
}