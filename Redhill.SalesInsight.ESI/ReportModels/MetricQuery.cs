using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redhill.SalesInsight.ESI.ReportModels
{
    // AKA. MetricCombo
    public class MetricRequest
    {
        // The below 2 fields are for the Data client's reference.
        // When they request the Metric values, they can fill up the below 2 Refs for easy access via linq
        public string ClientRefId { get; set; }
        public string ClientRefCategory { get; set; }

        // The below are critical Input Requirements required to 
        // Fill the value for the output. Each is nullable individually
        public List<MetricDefinition> MetricDefinitions { get; set; }

        public List<long> PlantIds { get; set; }
        public List<long> DistrictIds { get; set; }
        public List<long> RegionIds { get; set; }
        public List<long> CustomerIds { get; set; }
        public List<long> MarketSegmentIds { get; set; }
        public List<long> SalesStaffIds { get; set; }
        public List<long> DriverIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int Skip { get; set; }

        public int Limit { get; set; }

        public string search { get; set; }

        public string  SortType { get; set; }

        public List<SortItem>  order { get; set; }

        // The output that will be calculated and returned back.
        public List<MetricWiseBucket> Values { get; set; }

    }

}
