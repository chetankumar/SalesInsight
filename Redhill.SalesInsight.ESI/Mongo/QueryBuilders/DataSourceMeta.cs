using RedHill.SalesInsight.ESI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redhill.SalesInsight.ESI.Mongo.QueryBuilders
{
    public class DataSourceMeta
    {
        public static string[] TicketLevelDimensions = new string[] { "PlantId", "PlantName", "DistrictId", "DistrictName", "RegionId", "RegionName", "Date","SalesStaffId","SalesStaffName","CustomerId","CustomerName","CustomerSegmentId" };
        public static string[] PlantLevelDimensions = new string[] {"PlantId","PlantName","DistrictId","DistrictName","RegionId", "RegionName","Date" };
        public static List<MongoFilter> ScrubFilters(string dataSource,List<MongoFilter> filters)
        {
            if (filters == null || !filters.Any())
                return filters;

            switch (dataSource)
            {
                case ESIDataManager.TicketStatsTable:
                    return filters.Where(x=> TicketLevelDimensions.Contains(x.PropertyName)).ToList();
                case ESIDataManager.PlantDayStatsTable:
                    return filters.Where(x => PlantLevelDimensions.Contains(x.PropertyName)).ToList();
                case ESIDataManager.DailyPlantSummaryTable:
                    return filters.Where(x => PlantLevelDimensions.Contains(x.PropertyName)).ToList();
                default:
                    return filters.Where(x => PlantLevelDimensions.Contains(x.PropertyName)).ToList();
            }
        }
    }
}
