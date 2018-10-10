using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI.Payload
{
    public class GoalAnalysisResponsePayload
    {
        public string MetricName { get; set; }
        public string MetricType { get; set; }

        public string DisplayFormat { get; set; }

        public bool HasHorizontalSeparator { get; set; }

        public bool ShowActionIcons { get; set; }

        public decimal OkLimit { get; set; }
        public decimal WarningLimit { get; set; }
        public decimal CautionLimit { get; set; }

        public int ComparisonMetricId { get; set; }

        public double ComparisonValue { get; set; }

        public Dictionary<string, dynamic> Values { get; set; }
    }
}