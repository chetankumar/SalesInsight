using Redhill.SalesInsight.ESI.Mongo.QueryBuilders;
using Redhill.SalesInsight.ESI.ReportModels;
using RedHill.SalesInsight.DAL;
using System.Collections.Generic;

namespace Redhill.SalesInsight.ESI.MetricHelpers
{
    public interface IMetricHelper
    {
        MetricDefinition HelperFor();
        List<Aggregation> GetAggregations();
        dynamic ProcessValue(List<Aggregation> dataResults);
        List<MetricGroupBucket> ProcessGroupValues(List<Aggregation> dataResults);
    }
}
