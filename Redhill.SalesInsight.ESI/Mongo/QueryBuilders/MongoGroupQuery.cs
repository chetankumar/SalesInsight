using Redhill.SalesInsight.ESI.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redhill.SalesInsight.ESI.Mongo.QueryBuilders
{
    public class MongoGroupQuery: MongoQuery
    {
        #region Properties

        public bool IsDateGroup { get; set; }
        public string ColumnProperty { get; set; }

        public RepeatPeriod Granularity { get; set; }

        #endregion
    }
}
