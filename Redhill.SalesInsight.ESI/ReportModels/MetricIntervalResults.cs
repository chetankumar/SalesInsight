using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redhill.SalesInsight.ESI.ReportModels
{
    public class MetricGroupResults
    {
        public int MetricDefinitionId { get; set; }
        public List<MetricGroupBucket> MetricGroupBuckets { get; set; }
        
    }
}
