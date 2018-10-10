using MongoDB.Bson.Serialization.Attributes;
using RedHill.SalesInsight.DAL;
using System;

namespace Redhill.SalesInsight.ESI.Mongo.Models
{
    public class DailyPlantSummaryStats
    {
        #region Properties

        [BsonId]
        public long Id { get; set; }

        public long PlantId { get; set; }
        public string PlantName { get; set; }
        public long DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string RegionName { get; set; }
        public long RegionId { get; set; }
        public DateTime Date { get; set; }

        public double? ProducedVolume { get; set; }
        public double? TrucksAssigned { get; set; }
        public double? TruckAvailable { get; set; }
        public double? DriversOnPayroll { get; set; }
        public double? DriversAvailable { get; set; }
        public double? PlantInterruptions { get; set; }
        public double? BadOrRejectedLoads { get; set; }
        public double? Accidents { get; set; }
        public double? TotalLoads { get; set; }
        public double? TotalOrders { get; set; }
        public double? FirstLoadOnTimePercent { get; set; }
        public double? DriverDeliveredVolume { get; set; }
        public double? ScheduledVolume { get; set; }
        public double? ScheduledTrucks { get; set; }
        public double? TotalClockHours { get; set; }
        public double? DriversUtilized { get; set; }
        public double? AverageLoadSize { get; set; }
        public double? StartUpTime { get; set; }
        public double? ShutdownTime { get; set; }
        public double? InYardTime { get; set; }
        public double? TicketTime { get; set; }
        public double? LoadTime { get; set; }
        public double? TemperingTime { get; set; }
        public double? ToJobTime { get; set; }
        public double? WaitOnJobTime { get; set; }
        public double? PourTime { get; set; }
        public double? WashOnJobTime { get; set; }
        public double? FromJobTime { get; set; }
        public double? TruckBreakdowns { get; set; }
        public double? NonDeliveryHours { get; set; }
        public long? RefId { get; set; }
        #endregion
        #region Constructors

        public DailyPlantSummaryStats()
        {

        }
        public DailyPlantSummaryStats(DailyPlantSummary dailyPlantSummary)
        {
            this.Id = dailyPlantSummary.Id;
            this.Date = dailyPlantSummary.DayDateTime;
            this.PlantId = dailyPlantSummary.PlantId;
            var plant = SIDAL.GetPlant(dailyPlantSummary.PlantId);
            if(plant!=null)
            {
                this.PlantName = plant.Name;
                this.DistrictId = plant.DistrictId;
                this.DistrictName = plant.District.Name;
                this.RegionId = plant.District.RegionId;
                this.RegionName = plant.District.Region.Name;
            }

            this.ProducedVolume = dailyPlantSummary.ProducedVolume;
            this.TrucksAssigned = dailyPlantSummary.TrucksAssigned;
            this.TruckAvailable = dailyPlantSummary.TruckAvailable;
            this.DriversOnPayroll = dailyPlantSummary.DriversOnPayroll;
            this.DriversAvailable = dailyPlantSummary.DriversAvailable;
            this.PlantInterruptions = dailyPlantSummary.PlantInterruptions;
            this.BadOrRejectedLoads = dailyPlantSummary.BadOrRejectedLoads;
            this.Accidents = dailyPlantSummary.Accidents;
            this.TotalLoads = dailyPlantSummary.TotalLoads;
            this.TotalOrders = dailyPlantSummary.TotalOrders;
            this.FirstLoadOnTimePercent = dailyPlantSummary.FirstLoadOnTimePercent;
            this.DriverDeliveredVolume = dailyPlantSummary.DriverDeliveredVolume;
            this.ScheduledVolume = dailyPlantSummary.ScheduledVolume;
            this.ScheduledTrucks = dailyPlantSummary.ScheduledTrucks;
            this.TotalClockHours = dailyPlantSummary.TotalClockHours;
            this.DriversUtilized = dailyPlantSummary.DriversUtilized;
            this.AverageLoadSize = dailyPlantSummary.AverageLoadSize;
            this.StartUpTime = dailyPlantSummary.StartUpTime;
            this.ShutdownTime = dailyPlantSummary.ShutdownTime;
            this.InYardTime = dailyPlantSummary.InYardTime;
            this.TicketTime = dailyPlantSummary.TicketTime;
            this.LoadTime = dailyPlantSummary.LoadTime;
            this.TemperingTime = dailyPlantSummary.TemperingTime;
            this.ToJobTime = dailyPlantSummary.ToJobTime;
            this.WaitOnJobTime = dailyPlantSummary.WaitOnJobTime;
            this.PourTime = dailyPlantSummary.PourTime;
            this.WashOnJobTime = dailyPlantSummary.WashOnJobTime;
            this.FromJobTime = dailyPlantSummary.FromJobTime;
            this.TruckBreakdowns = dailyPlantSummary.TruckBreakdowns;
            this.NonDeliveryHours = dailyPlantSummary.NonDeliveryHours;
            this.RefId = dailyPlantSummary.RefId;
        } 
        #endregion
    }
}
