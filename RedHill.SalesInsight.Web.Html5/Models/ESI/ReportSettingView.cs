using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.DAL;
using Newtonsoft.Json;
using RedHill.SalesInsight.DAL.Constants;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class ReportSettingView
    {
        public long ReportId { get; set; }
        public string ReportName { get; set; }
        public string AccessType { get; set; }
        public string Type { get; set; }
        public bool IsDefault { get; set; }
        public bool IsFavourite { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int> MetricDefinitionIds { get; set; }
        public List<int> ColumnDefinitionIds { get; set; }
        public ReportFilterSettingView ReportFilterSetting { get; set; }
        public ReportConfigSettingView ReportConfigSetting { get; set; }
        public BenchmarkMetricConfigSettingView BenchMarkMetricConfig { get; set; }
        public DrillinReportConfigSetting DrillinReportConfigSetting { get; set; }
        public TrendReportConfigSetting TrendReportConfigSetting { get; set; }
        public List<SelectListItem> MetricTypeDefinitionList { get; set; }
        public List<SelectListItem> ColumnTypeDefinitionList { get; set; }
        public List<MetricDefinition> MetricDefinitions { get; set; }

        public ReportSettingView()
        {

        }
       
        public ReportSettingView(ReportSetting reportSetting, Guid userId, bool? persistFilter = false, long? widgetId = 0, ReportFilterSettingView reportFilterSetting = null,UserReportSetting userReportSetting = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            this.ReportId = reportSetting.Id;
            this.ReportName = reportSetting.ReportName;
            this.AccessType = reportSetting.AccessType;
            this.Type = reportSetting.Type;
            if (userReportSetting == null)
            {
                this.IsDefault = false;
                this.IsFavourite = false;
            }
            else
            {
                this.IsDefault = userReportSetting.IsDefault.GetValueOrDefault();
                this.IsFavourite = userReportSetting.IsFavourite.GetValueOrDefault();
            }

            this.UserId = userId;
            this.CreatedAt = reportSetting.CreatedAt;
            var todayDate = DateTime.Today;
            if (startDate != null && persistFilter.GetValueOrDefault())
            {
                this.StartDate = startDate;
            }
            else
            {
                var defaultStartDate = todayDate.AddDays(-31);
                if (this.Type == DAL.Constants.ESIReportType.GOAL_ANALYSIS)
                {
                    defaultStartDate = todayDate.AddDays(-1);
                }

                this.StartDate = reportSetting.StartDate.GetValueOrDefault(defaultStartDate);
            }

            if (endDate != null && persistFilter.GetValueOrDefault())
            {
                this.EndDate = endDate;
            }
            else
            {
                this.EndDate = reportSetting.EndDate.GetValueOrDefault(todayDate.AddDays(-1));
            }

            if (persistFilter == true && widgetId != null && widgetId != 0)
            {
                DateTime[] startEnd = SIDAL.GetWidgetStartEndDate(widgetId, reportSetting.Type);
                this.StartDate = startEnd[0];
                this.EndDate = startEnd[1];
                var json = JsonConvert.SerializeObject((DashboardFilterSettingView)HttpContext.Current.Session["DashboardFilters"]);
                this.ReportFilterSetting = JsonConvert.DeserializeObject<ReportFilterSettingView>(json);
            }
            else if (persistFilter == true)
            {
                this.ReportFilterSetting = new ReportFilterSettingView(reportFilterSetting,this.UserId);
            }
            if (this.ReportFilterSetting == null)
            {
                this.ReportFilterSetting = new ReportFilterSettingView(this.UserId, this.ReportId);
            }
            this.ReportConfigSetting = new ReportConfigSettingView(this.ReportId);
            this.DrillinReportConfigSetting = new DrillinReportConfigSetting(this.ReportId);
            this.BenchMarkMetricConfig = new BenchmarkMetricConfigSettingView(this.UserId, this.ReportId);
            this.TrendReportConfigSetting = new TrendReportConfigSetting(this.ReportId);

            MetricDefinitionIds = ReportConfigSetting.ReportRowConfigList.Select(x => x.MetricDefinitionId.GetValueOrDefault()).ToList();

            MetricTypeDefinitionList = new List<SelectListItem>();
            this.MetricDefinitions = SIDAL.GetAllMetricDefinition(true);
            //foreach (MetricDefinition metricDef in metricDefinitions)
            //{
            //    SelectListItem item = new SelectListItem();
            //    item.Text = metricDef.DisplayName ?? metricDef.MetricName;
            //    item.Value = metricDef.Id.ToString();
            //    item.Selected = MetricDefinitionIds.Contains(metricDef.Id);
            //    this.MetricTypeDefinitionList.Add(item);
            //}

            ColumnDefinitionIds = DrillinReportConfigSetting.ReportColumnConfigList.Select(x => x.MetricDefinitionId.GetValueOrDefault()).ToList();
            ColumnTypeDefinitionList = new List<SelectListItem>();
            foreach (MetricDefinition metricDef in this.MetricDefinitions)
            {
                SelectListItem item = new SelectListItem();
                item.Text = metricDef.DisplayName ?? metricDef.MetricName;
                item.Value = metricDef.Id.ToString();
                item.Selected = ColumnDefinitionIds.Contains(metricDef.Id);
                this.ColumnTypeDefinitionList.Add(item);
            }
        }
    }
}