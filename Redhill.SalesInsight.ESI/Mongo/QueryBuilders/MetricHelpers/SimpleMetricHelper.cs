using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redhill.SalesInsight.ESI.Mongo.QueryBuilders;
using RedHill.SalesInsight.DAL;
using Redhill.SalesInsight.ESI.Enumerations;
using Redhill.SalesInsight.ESI.ReportModels;

namespace Redhill.SalesInsight.ESI.MetricHelpers
{
    public class SimpleMetricHelper : IMetricHelper
    {
        public List<Aggregation> Aggregations { get; set; }
        public MetricDefinition MetricDefinition { get; set; }
        public SimpleMetricHelper(MetricDefinition definition)
        {
            Aggregation agg = new Aggregation(definition);
            this.Aggregations = new List<Aggregation>();
            this.Aggregations.Add(agg);

            this.MetricDefinition = definition;
        }

        public List<Aggregation> GetAggregations()
        {
            return Aggregations;
        }

        public dynamic ProcessValue(List<Aggregation> data)
        {
            return data.FirstOrDefault(x=>x.MetricDefinition.MetricName == this.MetricDefinition.MetricName).QueryValue;
        }

        public MetricDefinition HelperFor()
        {
            return MetricDefinition;
        }
        public List<MetricGroupBucket> ProcessGroupValues(List<Aggregation> dataResults)
        {
            return dataResults.FirstOrDefault(x => x.MetricDefinition.MetricName == this.MetricDefinition.MetricName).BucketValues;
        }
    }
}
