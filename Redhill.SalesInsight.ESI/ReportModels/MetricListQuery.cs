using Redhill.SalesInsight.ESI.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redhill.SalesInsight.ESI.ReportModels
{
    public class MetricListRequest : MetricRequest
    {
        // Additional Input Requirements
        // The which period you want to repeat on
        public bool IsDateGroup { get; set; }
        public string ColumnProperty { get; set; }

        public RepeatPeriod Granularity { get; set; }
        public int NumberOfResults { get; set; }

        // output will be in the below
        public List<MetricGroupResults> BucketValues { get; set; }

        


    }
}
