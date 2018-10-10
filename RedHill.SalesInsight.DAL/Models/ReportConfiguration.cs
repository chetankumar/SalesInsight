using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.DAL
{
    public class ReportConfiguration
    {
        public long ReportId { get; set; }
        public int Order { get; set; }
        public int? ColumnId { get; set; }
        public long? EntityRefId { get; set; }
        public string EntityName { get; set; }
        public string DisplayName { get; set; }
        public string ColumnName { get; set; }
        public string DimensionName { get; set; }
        public string DimensionTitle { get; set; }
        public string CustomFilterDimension { get; set; }
        public bool ShowActionIcons { get; set; }
        public int OkLimit { get; set; }
        public int CautionLimit { get; set; }
        public int WarningLimit { get; set; }
        public int ComparisonMetricId { get; set; }
        public bool IsVarianceColumn { get; set; }
        public string SortDirection { get; set; }
        public int SortCount { get; set; }
        public string SortType { get; set; }
    }
}