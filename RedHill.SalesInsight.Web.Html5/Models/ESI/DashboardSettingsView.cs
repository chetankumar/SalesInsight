using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedHill.SalesInsight.DAL.Constants;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class DashboardSettingsView
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string AccessType { get; set; }
        public bool Favorite { get; set; }
        public bool Default { get; set; }
        public int MaxColumn { get; set; }
        public DateTime? StartDate { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<WidgetSettingsView> Widgets { get; set; }
        public WidgetSettingsView Widget { get; set; }

        public DashboardFilterSettingView DashboardFilter { get; set; }
        public Dictionary<string, List<ReportSetting>> GoalAnalysisSavedReports
        {
            get
            {
                return SIDAL.GetSavedReports(this.UserId, ESIReportType.GOAL_ANALYSIS);
            }
        }
        public Dictionary<string, List<ReportSetting>> TrendAnalysisSavedReports
        {
            get
            {
                return SIDAL.GetSavedReports(this.UserId, ESIReportType.TREND_ANALYSIS);
            }
        }
        public Dictionary<string, List<ReportSetting>> BenchmarkAnalysisSavedReports
        {
            get
            {
                return SIDAL.GetSavedReports(this.UserId, ESIReportType.BENCHMARK_ANALYSIS);
            }
        }
        public Dictionary<string, List<ReportSetting>> DrillInAnalysisSavedReports
        {
            get
            {
                return SIDAL.GetSavedReports(this.UserId, ESIReportType.DRILL_IN);
            }
        }
        public DashboardSettingsView()
        {
        }
        public DashboardSettingsView(DashboardSetting widgetSetting, DashboardFilterSettingView dashboardFilterView,Guid userId, UserDashboardSetting userDashSetting = null,DateTime? startDate=null)
        {
            this.Id = widgetSetting.Id;
            this.Name = widgetSetting.Name;
            this.AccessType = widgetSetting.AccessType;
            if (userDashSetting != null)
            {
                this.Favorite = userDashSetting.IsFavourite.GetValueOrDefault();
                this.Default = userDashSetting.IsDefault.GetValueOrDefault();
                this.MaxColumn = userDashSetting.MaxColumn;
            }
            else
            {
                this.Favorite = false;
                this.Default = false;
                this.MaxColumn = 7;
            }

            this.UserId = userId;
            if (startDate == null)
            {
                this.StartDate = widgetSetting.StartDate.GetValueOrDefault(DateTime.Today.AddDays(-1));
            }
            else
            {
                this.StartDate = startDate;
            }
            this.CreatedDate = widgetSetting.CreatedAt.GetValueOrDefault();
            this.Widget = new WidgetSettingsView();
            List<WidgetSetting> widSetList = SIDAL.GetUserWidgetSettingsList(this.UserId, this.Id);
            if (widSetList != null)
            {
                Widgets = new List<WidgetSettingsView>();
                WidgetSettingsView widgetSettingView = null;
                foreach (var widget in widSetList)
                {
                    widgetSettingView = new WidgetSettingsView(widget);
                    this.Widgets.Add(widgetSettingView);
                }
            }
            if (dashboardFilterView != null)
            {
                this.DashboardFilter = new DashboardFilterSettingView(dashboardFilterView, this.UserId);
            }
            else
            {
                this.DashboardFilter = new DashboardFilterSettingView(this.UserId, this.Id);
            }
        }
    }
}