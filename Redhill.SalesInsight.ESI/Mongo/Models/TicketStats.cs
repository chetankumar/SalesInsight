using MongoDB.Bson.Serialization.Attributes;
using System;

namespace RedHill.SalesInsight.DAL.Mongo.Models
{
    public class TicketStats
    {
        [BsonId]
        public long Id { get; set; }
        public bool IsVoid { get; set; }
        public string TicketId { get; set; }
        public string TicketNumber { get; set; }
        public DateTime Date { get; set; }

        // Common for all tickets for the same driver and day
        public double StartupMinutes { get; set; }
        public double ShutdownMinutes { get; set; }
        // Arrival on Plant - Time Ticketed
        public double TotalMinutes { get; set; }

        // TotalMinutes * Utilization Factor --> Hours (in decimal)
        // As discussed, if 6 hours is the total times for the tickets, and 10 hours is the total time for the driver,
        // the utilization factor is going to be 10/6 = 1.66. And if this Ticket's TotalMinutes is 2 hours, the Estimated clock 
        // hours is going be 2 * 1.66 = 3.2 hours.
        public double EstimatedClockHours { get; set; }
        
        // Begin Load - Ticket Time
        public double TicketingMinutes { get; set; }
        // End Load - Begin Load
        public double LoadMinutes { get; set; }
        // Leave Plant - EndLoad
        public double Temper { get; set; }
        // Arrive at job - Leave Plant
        public double ToJobMinutes { get; set; }
        // Job Begin unload - Arrive at job
        public double WaitMinutes { get; set; }
        // Job End Unload - Job Begin Load
        public double UnloadMinutes { get; set; }
        // Job Leave - Job End Unload
        public double WashMinutes { get; set; }
        // Plant Arrival - Job Leave
        public double FromJobMinutes { get; set; }
        // Plant Arrival - Time Due on Job
        public double LateMinutes { get; set; }

        // Time before the last ticket end and this ticket's ticketing time
        // 0 for the first ticket.
        public double InYardMinutes { get; set; }

        public long PlantId { get; set; }
        public string PlantName { get; set; }
        public long DistrictId { get; set; }
        public string DistrictName { get; set; }
        public long RegionId { get; set; }
        public string RegionName { get; set; }
        public bool FOB { get; set; }
        public long DriverId { get; set; }
        public string DriverNumber { get; set; }
        public string DriverDescription { get; set; }
        public string TruckNumber { get; set;}
        public string TruckType { get; set; }
        public long CustomerId { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public long CustomerSegmentId { get; set; }
        public string CustomerSegmentName { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerZipCode { get; set; }
        public long JobId { get; set; }
        public string JobName { get; set; }
        public long JobSegmentId { get; set; }
        public long SalesStaffId { get; set; }
        public string SalesStaffName { get; set; }
        public string SalesStaffNumber { get; set; }
        public string BatchmanNumber { get; set; }
        public string BatchmanDescription { get; set; }
        public double Volume { get; set; }
        public double Revenue { get; set; }
        public double MaterialCost { get; set; }

        public double Spread { get; set; }
        public double VariableDelivery { get; set; }

        public double Contribution { get; set; }
        public double FixedDelivery { get; set; }
        public double FixedPlant { get; set; }
        public double SGA { get; set; }

        public double Profit { get; set; }
    }
}
