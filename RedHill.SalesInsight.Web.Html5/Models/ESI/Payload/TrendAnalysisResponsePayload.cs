using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI.Payload
{
    public class TrendAnalysisResponsePayload
    {
        public int MetricDefinitionId { get; set; }
        public string MetricName { get; set; }

        public List<JArray> Values { get; set; }
    }

    public class ChartItem
    {
        public DateTime Date { get; set; }
        public dynamic Value { get; set; }
    }
}