using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace RedHill.SalesInsight.DAL.Mongo.Models
{
    public class PlantDayStats
    {
        [BsonId]
        public string Id
        {
            get
            {
                return String.Format("{0}_{1}", this.PlantId, this.Date.ToString("yyyy-MM-dd"));
            }
        }
        
        public long PlantId { get; set; }
        public string PlantName { get; set; }
        public long DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string RegionName { get; set; }
        public long RegionId { get; set; }

        public DateTime Date { get; set; }

        public double BudgetTicketingMinutes { get; set; }
        public double BudgetLoadMinutes { get; set; }
        public double BudgetTemperMinutes { get; set; }
        public double BudgetToJobMinutes { get; set; }
        public double BudgetWaitMinutes { get; set; }
        public double BudgetUnloadMinutes { get; set; }
        public double BudgetWashMinutes { get; set; }
        public double BudgetFromJobMinutes { get; set; }
        public double BudgetLateMinutes { get; set; }
        public double BudgetStartupMinutes { get; set; }
        public double BudgetShutdownMinutes { get; set; }
        public double BudgetInYardMinutes { get; set; }
        public double BudgetTotalClocked { get; set; }
        public double BudgetVolume { get; set; }
        public double BudgetTrucks { get; set; }
        public double BudgetRevenue { get; set; }
        public double BudgetMaterialCost { get; set; }
        public double BudgetSpread { get; set; }
        public double BudgetContribution { get; set; }
        public double BudgetProfit { get; set; }
        public double BudgetAvgLoad { get; set; }
        public double BudgetAccidents { get;  set; }
        public double BudgetBatchTolerance { get; set; }
        public double BudgetCydHr { get; set; }
        public double BudgetFirstLoadOnTimePercent { get; set; }
        public double BudgetTrucksPercentOperable { get; set; }
        public double BudgetTrucksDown { get; set; }
        public double BudgetPlantInterruptions { get; set; }
        public double BudgetSGA { get; set; }
        public double BudgetDeliveryVariable { get; set; }
        public double BudgetPlantVariable { get; set; }
        public double BudgetDeliveryFixed { get; set; }
        public double BudgetPlantFixed { get; set; }
    }
}
