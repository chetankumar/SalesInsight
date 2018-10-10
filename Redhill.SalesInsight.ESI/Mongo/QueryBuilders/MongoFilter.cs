using Redhill.SalesInsight.ESI.Enumerations;
using Redhill.SalesInsight.ESI.ReportModels;
using System.Collections.Generic;

namespace Redhill.SalesInsight.ESI.Mongo.QueryBuilders
{
    public class MongoFilter
    {
        public string PropertyName { get; set; }
        public ComparisionType ComparisionType { get; set; }
        public dynamic Value { get; set; }
        public dynamic Value2 { get; set; }
        public int SkipRecords { get; set; }
        public int ItemsPerPage { get; set; }

        public string search { get; set; }

        public string SortType { get; set; }

        public List<SortItem>  order { get; set; }
    }
}
