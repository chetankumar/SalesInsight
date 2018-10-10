using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIProject
    {
        public int      ProjectId { get; set; }
        public string   Name { get; set; }
        public string   CustomerRefName { get; set; }
        public string   ProjectRefId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? BidDate { get; set; }
        public DateTime? WonLostDate { get; set; }

        public string   Address { get; set; }
        public string   Latitude { get; set; }
        public string   Longitude { get; set; }
        public string   City { get; set; }
        public string   State { get; set; }
        public string   ZipCode { get; set; }

        public ProjectStatus    ProjectStatus { get; set; }
        public Plant            Plant { get; set; }
        public MarketSegment    MarketSegment { get; set; }
        public Customer         Customer { get; set; }
        public Contractor       Contractor { get; set; }
        public ReasonsForLoss   ReasonForLoss { get; set; }

        public int?     ToJobMinutes { get; set; }
        public int?     WashMinutes { get; set; }
        public int?     ReturnMinutes { get; set; }
        public int?     Unload { get; set; }
        public int?     WaitOnJob { get; set; }
        public double?  DistanceToJob { get; set; }
        public string   DeliveryInstructions { get; set; }

        public double?  AverageLoadSize { get; set; }
        public int?     Valuation { get; set; }
        public int?     Volume { get; set; }
        public string   Mix { get; set; }
        public decimal? Price { get; set; }
        public decimal? Spread { get; set; }
        public decimal? Profit { get; set; }

        public int?     WinningCompetitorId { get; set; }
        public string   LossNotes { get; set; }
        public decimal? PriceLost { get; set; }

        public string UDF1 { get; set; }
        public string UDF2 { get; set; }

        public bool Active { get; set; }
        public bool DistrictQcRequirement { get; set; }
        public bool ExcludeFromReports { get; set; }
        public long? BackupPlantId { get; set; }

        public SIProject(Project project)
        {
            this.ProjectId = project.ProjectId;
            this.BackupPlantId = project.BackupPlantId;
            this.Name = project.Name;
            this.CustomerRefName = project.CustomerRefName;
            this.ProjectRefId = project.ProjectRefId;
            this.City = project.City;
            this.Address = project.Address;
            this.Latitude = project.Latitude;
            this.Longitude = project.Longitude;
            this.State = project.State;
            this.ZipCode = project.ZipCode;

            this.StartDate = project.StartDate;
            this.BidDate = project.BidDate;
            this.WonLostDate = project.WonLostDate;

            this.Plant = project.Plant;
            this.ProjectStatus = (project.ProjectStatus);
            this.MarketSegment = (project.MarketSegment);
            this.Customer = (project.Customer);
            this.Contractor = (project.Contractor);

            this.ToJobMinutes = project.ToJobMinutes;
            this.WashMinutes = project.WashMinutes;
            this.ReturnMinutes = project.ReturnMinutes;
            this.Unload = project.Unload;
            this.WaitOnJob = project.WaitOnJob;
            this.AverageLoadSize = project.AverageLoadSize;
            this.DistanceToJob = project.DistanceToJob;
            this.DeliveryInstructions = project.DeliveryInstructions;

            this.UDF1 = project.UDF1;
            this.UDF2 = project.UDF2;

            this.Valuation = project.Valuation;
            this.Volume = project.Volume;
            this.Mix = project.Mix;
            this.Price = project.Price;
            this.Spread = project.Spread;
            this.Profit = project.Profit;

            this.ReasonForLoss = project.ReasonsForLoss;
            this.WinningCompetitorId = project.WinningCompetitorId;
            this.LossNotes = project.NotesOnLoss;
            this.PriceLost = project.PriceLost;

            this.Active = project.Active.GetValueOrDefault(false);
            this.DistrictQcRequirement = SIDAL.GetProjectDistrictQcRequirement(ProjectId);
            this.ExcludeFromReports = project.ExcludeFromReports.GetValueOrDefault(false);
        }
    }
}
