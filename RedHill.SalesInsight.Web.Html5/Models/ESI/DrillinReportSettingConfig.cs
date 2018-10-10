using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.DAL;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class DrillinReportConfigSetting
    {
        public List<ReportColumnConfig> ReportColumnConfigList { get; set; }
        public DrillinReportConfig DrillinReportConfig { get; set; }

        public List<SelectListItem> ReportColumnDefinitionList { get; set; }

        public SpecialReportConfig SpecialReportConfig { get; set; }

        public List<SelectListItem> SortDirectionList { get; set; }

        public List<SelectListItem> SortTypeList { get; set; }

        public DrillinReportConfigSetting()
        {

        }

        public DrillinReportConfigSetting(long reportId)
        {
            this.ReportColumnConfigList = SIDAL.GetReportColumnConfig(reportId);
            this.DrillinReportConfig = SIDAL.GetDrillInReportConfig(reportId);
            this.SpecialReportConfig = SIDAL.GetDrillInCustomConfig(reportId);
            ReportColumnDefinitionList = new List<SelectListItem>();
            
            
            if (SpecialReportConfig != null)
            {
                foreach (var element in ReportColumnConfigList)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = element.DisplayName;
                    item.Value = element.Id.ToString();

                    item.Selected = SpecialReportConfig != null ? SpecialReportConfig.CustomFilterDimension == item.Text : false;
                    this.ReportColumnDefinitionList.Add(item);
                }

            }
            else
            {
                foreach (var element in ReportColumnConfigList)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = element.DisplayName;
                    item.Value = element.Id.ToString();

                    item.Selected = SpecialReportConfig != null ? SpecialReportConfig.CustomFilterDimension == item.Text : false;
                    this.ReportColumnDefinitionList.Add(item);
                }
                this.SpecialReportConfig = new SpecialReportConfig();
                this.SpecialReportConfig.CustomFilterDimension = "";
                this.SpecialReportConfig.SortDirection = "";
                this.SpecialReportConfig.SortType = "";
            }

            SortDirectionList = GetSortDirection();
            SortTypeList = GetSortType();
        }

        public List<SelectListItem> GetSortDirection()
        {
            string[] labels = { "Top", "Bottom" };
            string[] values = { "top", "bottom" };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.SpecialReportConfig.SortDirection == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }
        public List<SelectListItem> GetSortType()
        {
            string[] labels = { "%", "Records" };
            string[] values = { "percent", "records" };
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i < labels.Count(); i++)
            {
                SelectListItem item = new SelectListItem();
                item.Text = labels[i];
                item.Value = values[i];
                if (this.SpecialReportConfig.SortType == item.Value)
                    item.Selected = true;
                items.Add(item);
            }
            return items;
        }

    }
}