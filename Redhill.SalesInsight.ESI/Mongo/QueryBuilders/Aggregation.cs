using Redhill.SalesInsight.ESI.Enumerations;
using Redhill.SalesInsight.ESI.ReportModels;
using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;

namespace Redhill.SalesInsight.ESI.Mongo.QueryBuilders
{
    public class Aggregation
    {
        public Aggregation()
        {

        }

        public Aggregation(MetricDefinition definition)
        {
            this.AggregationType = (AggregationType)Enum.Parse(typeof(AggregationType), definition.DefaultAggregation, true);
            this.DataSource = definition.DataSource;
            this.ColumnName = definition.ColumnProperty;
            this.ColumnName2 = definition.ColumnProperty2;
            this.MetricDefinition = definition;
        }

        public MetricDefinition MetricDefinition { get; set; }
        public string DataSource { get; set; }
        public string ColumnName { get; set; }
        public string ColumnName2 { get; set; }
        public AggregationType AggregationType { get; set; }
        public dynamic QueryValue {get;set;}
        public List<MetricGroupBucket> BucketValues{ get; set; }

    }
}
