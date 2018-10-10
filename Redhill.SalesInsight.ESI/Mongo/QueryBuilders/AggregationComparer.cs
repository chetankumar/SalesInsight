using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redhill.SalesInsight.ESI.Mongo.QueryBuilders
{
    class AggregationComparer : IEqualityComparer<Aggregation>
    {
        public bool Equals(Aggregation x, Aggregation y)
        {
            return x.MetricDefinition.MetricName == y.MetricDefinition.MetricName &&
                    x.MetricDefinition.DefaultAggregation == y.MetricDefinition.DefaultAggregation;
        }

        public int GetHashCode(Aggregation obj)
        {
            return obj.MetricDefinition.MetricName.GetHashCode() * 17 + obj.MetricDefinition.DefaultAggregation.GetHashCode();
        }
    }
}
