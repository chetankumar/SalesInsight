using Redhill.SalesInsight.ESI.Enumerations;
using Redhill.SalesInsight.ESI.MetricHelpers;
using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redhill.SalesInsight.ESI.Mongo.QueryBuilders.MetricHelpers
{
    public class MetricHelperFactory
    {
        public static IMetricHelper GetHelperFor(MetricDefinition md)
        {
            if (md.DefaultAggregation == AggregationType.custom.ToString())
            {
                return null;
            }
            else if (md.DefaultAggregation == AggregationType.formula.ToString())
            {
                FormulaMetricHelper helper = new FormulaMetricHelper(md);
                return helper;
            }
            else
            {
                SimpleMetricHelper helper = new SimpleMetricHelper(md);
                return helper;
            }
        }
    }
}
