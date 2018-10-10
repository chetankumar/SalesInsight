using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redhill.SalesInsight.ESI.Mongo.QueryBuilders
{
    public class MongoQuery
    {
        #region Properties

        public string CollectionName { get; set; }
        public List<MongoFilter> Filters { get; set; }
        public List<Aggregation> AggregationList { get; set; }

        #endregion
    }
}
