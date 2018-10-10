using Redhill.SalesInsight.ESI.ReportModels;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.Web.Html5.Models.ESI.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class DrillInReportView : ReportSettingView
    {
        public List<SelectListItem> DimensionNameList { get; set; }
        public string DimensionName { get; set; }
        public string DimensionTitle { get; set; }

        public List<SortItem> order { get; set; }

        public DrillInReportView()
        {
        }

        public DrillInReportView(ReportSetting reportSetting, Guid userId, bool? persistFilter, long? widgetId, ReportFilterSettingView reportFilterSetting, UserReportSetting userReportSetting, DateTime? startDate, DateTime? endDate)
            : base(reportSetting, userId, persistFilter, widgetId, reportFilterSetting, userReportSetting, startDate, endDate)
        {
            LoadMetricList();
        }

        public void LoadMetricList()
        {
            DimensionName = DrillinReportConfigSetting?.DrillinReportConfig?.DimensionName;

            if (DimensionName == null)
                DimensionName = "";

            string[] labels = { "Plant", "Customer", "Market Segment", "Sales Staff", "District", "Region", "Driver" };
            string[] values = { "PlantName", "CustomerName", "CustomerSegmentName", "SalesStaffName", "DistrictName", "RegionName", "DriverNumber" };
            DimensionNameList = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                item.Selected = DimensionName.Contains(values[i]);
                this.DimensionNameList.Add(item);
            }
            DimensionTitle = DrillinReportConfigSetting?.DrillinReportConfig?.DimensionDisplayTitle;
        }
        public List<DrillInResponsePayload> Response { get; set; }

        

        public void LoadData()
        {
            this.DrillinReportConfigSetting = new DrillinReportConfigSetting(this.ReportId);
            var esiReportBroker = new EsiReportBroker(this.StartDate.GetValueOrDefault(DateTime.Now), this.EndDate, this.ReportFilterSetting, this.ReportConfigSetting);
            this.order = new List<SortItem>();
            this.order.Add(new SortItem());
            this.order[0].SortBy = this.DrillinReportConfigSetting.SpecialReportConfig.CustomFilterDimension;
            this.order[0].IsDescending = this.DrillinReportConfigSetting.SpecialReportConfig.SortDirection=="bottom"?"true":"false";
            esiReportBroker.DrillInReportConfiguration = this.DrillinReportConfigSetting.DrillinReportConfig;
            esiReportBroker.SpecialReportConfig = this.DrillinReportConfigSetting.SpecialReportConfig;
            esiReportBroker.order = this.order;
            this.Response = esiReportBroker.GetDrillInResponse(this.ReportId,UserId);
        }
    }
}