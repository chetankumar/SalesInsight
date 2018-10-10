using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redhill.SalesInsight.ESI.Mongo.QueryBuilders;
using RedHill.SalesInsight.DAL;
using Redhill.SalesInsight.ESI.Enumerations;
using System.Text.RegularExpressions;
using Redhill.SalesInsight.ESI.ReportModels;

namespace Redhill.SalesInsight.ESI.MetricHelpers
{
    public class FormulaMetricHelper : IMetricHelper
    {
        public List<Aggregation> Aggregations { get; set; }
        public MetricDefinition MetricDefinition { get; set; }
        public FormulaMetricHelper(MetricDefinition metricDefinition)
        {
            MatchCollection matchedDataLabels = Regex.Matches(metricDefinition.ColumnProperty, IMRegexPatterns.DATA_LABEL_COMPILER_PATTERN, RegexOptions.IgnoreCase);
            this.Aggregations = new List<Aggregation>();
            if (matchedDataLabels != null && matchedDataLabels.Count > 0)
            {
                foreach (var matchedItem in matchedDataLabels)
                {
                    string expression = ((System.Text.RegularExpressions.Capture)matchedItem).Value.Replace("@", "");
                    var tmpMetric = SIDAL.GetMetricDefinitionByName(expression);
                    if (!this.Aggregations.Where(x => x.ColumnName == tmpMetric.ColumnProperty).Any())
                    {
                        var aggregation = new Aggregation(tmpMetric);
                        this.Aggregations.Add(aggregation);
                    }
                }
            }
            this.MetricDefinition = metricDefinition;
        }

        public List<Aggregation> GetAggregations()
        {
            return Aggregations;
        }

        public dynamic ProcessValue(List<Aggregation> data)
        {
            var metricValuesDict = data.ToDictionary(x => x.MetricDefinition.MetricName, x => Convert.ToDouble(x.QueryValue));
            dynamic output = RubyManager.ProcessExpression(this.MetricDefinition.ColumnProperty, metricValuesDict);
            return output;
        }

        public MetricDefinition HelperFor()
        {
            return MetricDefinition;
        }

        public List<MetricGroupBucket> ProcessGroupValues(List<Aggregation> dataResults)
        {
            List<MetricGroupBucket> bucketList = new List<MetricGroupBucket>();
            List<dynamic> groupNames = new List<dynamic>();
            foreach (var agg in dataResults)
            {
                if (agg.BucketValues != null && agg.BucketValues.Any())
                    groupNames.AddRange(agg.BucketValues.Select(x => x.GroupName));
            }
            if (groupNames != null && groupNames.Any())
            {
                groupNames = groupNames.Distinct().ToList();

                foreach (var group in groupNames)
                {
                    Dictionary<string, dynamic> dictValues = new Dictionary<string, dynamic>();
                    foreach (var agg in dataResults)
                    {
                        var aggValue = agg.BucketValues.FirstOrDefault(x => x.GroupName == group);
                        if (aggValue == null)
                            dictValues.Add(agg.MetricDefinition.MetricName, 0);
                        else
                            dictValues.Add(agg.MetricDefinition.MetricName, Convert.ToDouble(aggValue.Value));
                    }
                    dynamic output = RubyManager.ProcessExpression(this.MetricDefinition.ColumnProperty, dictValues);
                    MetricGroupBucket bucket = new MetricGroupBucket();
                    bucket.GroupName = group;
                    bucket.Value = output;
                    bucketList.Add(bucket);
                }
            }
            return bucketList;
        }
    }
}
